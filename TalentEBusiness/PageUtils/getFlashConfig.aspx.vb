Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Xml.Serialization
Imports System.IO
Imports System.Data

Partial Class PageUtils_getFlashConfig
    Inherits Base01

    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _pageCode As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit
        _pageCode = "Personalise.aspx"
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        Response.Clear()
        Response.ClearContent()
        Response.ClearHeaders()
        Response.ContentType = "text/plain"
        Dim req As String = String.Empty
        If Request.QueryString("req") IsNot Nothing Then req = trapSQLInjection(Request.QueryString("req"))
        If req.Length > 0 Then
            Select Case req
                Case "playersXML"
                    Response.Write(buildXMLDoc("players"))
                Case "configXML"
                    If Request.QueryString("product") IsNot Nothing Then Response.Write(getConfigXML(trapSQLInjection(Request.QueryString("product"))))
            End Select
        Else
            Response.Write("<root></root>")
        End If
    End Sub

    Private Function getSetting(ByVal ATTRIBUTE_NAME As String, ByVal PRODUCT_CODE As String) As String
        Dim returnValue As String = String.Empty

        'TRY LOAD FROM SESSION XML
        returnValue = override_xml(ATTRIBUTE_NAME)
        If returnValue = "" Then
            'TRY LOAD DEFAULT FROM PRODUCT_PERSONALISE TABLE
            returnValue = tbl_product_personalise(ATTRIBUTE_NAME, PRODUCT_CODE)
            If returnValue = "" Then
                'TRY LOAD DEFAULT FROM FLASH SETTINGS TABLE
                returnValue = getAttributeValue(ATTRIBUTE_NAME)
            End If
        End If
        Return returnValue
    End Function

    Private Function getOverrides() As String
        Dim rtn As String = "<overrides>"
        Dim tbl As Data.DataTable = New Data.DataTable
        If Not Session("personalisationTransactions") Is Nothing AndAlso Not Request.QueryString("transactionId") Is Nothing Then
            tbl = Session("personalisationTransactions")
            For Each row As Data.DataRow In tbl.Rows
                If row.Item("TRANSACTION_ID") = Request.QueryString("transactionId") Then
                    rtn += "<override name='" & row.Item("COMPONENT_NAME") & "' val='" & row.Item("COMPONENT_VALUE") & "'/>"
                End If
            Next
        End If
        Return rtn + "</overrides>"
    End Function

    Private Function buildXMLDoc(ByVal xmlName As String) As String
        Dim sql As String = "   select " & _
                            "   x.[NAME] as x_NAME," & _
                            "   x.ROOT_NODE as x_ROOT_NODE," & _
                            "   n.SEQUENCE as n_SEQUENCE," & _
                            "   n.[NAME] as n_NAME," & _
                            "   n.[VALUE] as n_VALUE," & _
                            "   n.ATTR_NAME_1 as n_ATTR_NAME_1," & _
                            "   n.ATTR_VALUE_1 as n_ATTR_VALUE_1," & _
                            "   n.ATTR_NAME_2 as n_ATTR_NAME_2," & _
                            "   n.ATTR_VALUE_2 as n_ATTR_VALUE_2," & _
                            "   n.ATTR_NAME_3 as n_ATTR_NAME_3," & _
                            "   n.ATTR_VALUE_3 as n_ATTR_VALUE_3," & _
                            "   n.ATTR_NAME_4 as n_ATTR_NAME_4," & _
                            "   n.ATTR_VALUE_4 as n_ATTR_VALUE_4," & _
                            "   n.ATTR_NAME_5 as n_ATTR_NAME_5," & _
                            "   n.ATTR_VALUE_5 as n_ATTR_VALUE_5," & _
                            "   n.ATTR_NAME_6 as n_ATTR_NAME_6," & _
                            "   n.ATTR_VALUE_6 as n_ATTR_VALUE_6," & _
                            "   n.ATTR_NAME_7 as n_ATTR_NAME_7," & _
                            "   n.ATTR_VALUE_7 as n_ATTR_VALUE_7," & _
                            "   n.ATTR_NAME_8 as n_ATTR_NAME_8," & _
                            "   n.ATTR_VALUE_8 as n_ATTR_VALUE_8," & _
                            "   n.ATTR_NAME_9 as n_ATTR_NAME_9," & _
                            "   n.ATTR_VALUE_9 as n_ATTR_VALUE_9," & _
                            "   n.ATTR_NAME_10 as n_ATTR_NAME_10," & _
                            "   n.ATTR_VALUE_10 as n_ATTR_VALUE_10" & _
                            "   from tbl_flash_xml x " & _
                            "   inner join tbl_flash_xml_node n " & _
                            "       on n.PARENT_ID = x.ID " & _
                            "   where x.name = '" & xmlName & "'" & _
                            "   order by SEQUENCE"


        Dim cacheKey As String = xmlName
        Dim CacheTimeMinutes As Integer = 30

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Return CType(HttpContext.Current.Cache.Item(cacheKey), String)
        Else

            Dim conn As SqlConnection
            Dim comm As SqlCommand
            Dim drSrv As SqlClient.SqlDataReader
            Dim rtn As New StringBuilder
            Dim rootNode As String = "root"

            conn = New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString())
            conn.Open()
            comm = New SqlCommand(sql, conn)
            drSrv = comm.ExecuteReader
            Dim i As Integer = 0
            While drSrv.Read
                'set rootNode
                If i = 0 Then If ckNull(drSrv.Item("x_ROOT_NODE")) <> "" Then rootNode = ckNull(drSrv.Item("x_ROOT_NODE"))

                rtn.Append("<" & ckNull(drSrv.Item("n_NAME")))
                If ckNull(drSrv.Item("n_ATTR_NAME_1")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_1")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_1")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_2")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_2")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_2")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_3")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_3")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_3")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_4")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_4")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_4")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_5")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_5")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_5")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_6")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_6")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_6")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_7")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_7")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_7")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_8")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_8")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_8")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_9")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_9")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_9")) & "'")
                If ckNull(drSrv.Item("n_ATTR_NAME_10")) <> "" Then rtn.Append(" " & ckNull(drSrv.Item("n_ATTR_NAME_10")) & "='" & ckNull(drSrv.Item("n_ATTR_VALUE_10")) & "'")
                If ckNull(drSrv.Item("n_VALUE")) = "" Then
                    'create a closed node
                    rtn.Append("/>")
                Else
                    'create an open node
                    rtn.Append(">")
                    rtn.Append(ckNull(drSrv.Item("n_VALUE")))
                    rtn.Append("</" & ckNull(drSrv.Item("n_NAME")) & ">")
                End If
                i += 1
            End While

            drSrv.Close()
            comm.Dispose()
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
            End If
            conn.Dispose()
            HttpContext.Current.Cache.Insert(cacheKey, "<" & rootNode & ">" & rtn.ToString & "</" & rootNode & ">", Nothing, System.DateTime.Now.AddMinutes(CacheTimeMinutes), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            Return CType(HttpContext.Current.Cache.Item(cacheKey), String)
        End If
    End Function

    Private Function ckNull(ByVal str As String) As String
        Return Talent.eCommerce.Utilities.CheckForDBNull_String(str)
    End Function

    Private Function getConfigXML(ByVal PRODUCT_CODE As String) As String
        Dim rtn As String = "<root>" & _
                    "<config>" & _
                        "<cfg name='imgPath' val='" & getSetting("imgPath", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='bgColor' val='" & getSetting("bgColor", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='bgColor2' val='" & getSetting("bgColor2", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='scrollerWidthV' val='" & getSetting("scrollerWidthV", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='btnFontSize' val='" & getSetting("btnFontSize", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='btnBgColorOut' val='" & getSetting("btnBgColorOut", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='btnBgColorOver' val='" & getSetting("btnBgColorOver", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='btnBgColorDown' val='" & getSetting("btnBgColorDown", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='controlsBgColor' val='" & getSetting("controlsBgColor", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='btnFontColor' val='" & getSetting("btnFontColor", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='borderColor' val='" & getSetting("borderColor", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='font' val='" & getSetting("font", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='fontColor' val='" & getSetting("fontColor", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='fontSize' val='" & getSetting("fontSize", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='shadowColor' val='" & getSetting("shadowColor", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='choosePlayerTable' val='" & getSetting("choosePlayerTable", PRODUCT_CODE) & "'/>" & _
                        "<cfg name='displayInBasket' val='" & getSetting("displayInBasket", PRODUCT_CODE) & "'/>" & _
                    "</config>"
        rtn += getComponents(PRODUCT_CODE)
        rtn += getOverrides()
        rtn += getPlayers()
        rtn += getLang()
        rtn += "</root>"
        Return rtn
    End Function

    Private Function getComponents(ByVal PRODUCT_CODE As String) As String
        Dim rtn As String = "<components>"
        'Check Cache for this PRODUCT entry
        'If not found in cache then go get from db
        Dim sql As String = "select " & _
        "ID, " & _
        "LINKED_PRODUCT_CODE, " & _
        "QTY_RULE, " & _
        "COMP_TYPE, " & _
        "COMP_NAME, " & _
        "COMP_TEXT, " & _
        "COMP_FORCE_CASE, " & _
        "COMP_LABEL, " & _
        "COMP_FONT, " & _
        "COMP_FONT_COLOR, " & _
        "COMP_SIZE, " & _
        "COMP_MAX_CHARS, " & _
        "COMP_ARC_AMOUNT, " & _
        "COMP_ARC_ROTATION, " & _
        "COMP_ARC_WIDTH, " & _
        "COMP_POS_AT_PERCENT_WIDTH, " & _
        "COMP_POS_AT_PERCENT_HEIGHT, " & _
        "COMP_RESTRICT_INPUT, " & _
        "COMP_USE_IMAGE_TEXT " & _
        "from tbl_product_personalisation_component " & _
        "join tbl_product on PRODUCT_CODE = LINKED_PRODUCT_CODE " & _
        "where PRODUCT_PERSONALISATION_ID in (" & _
            "select top 1 PRODUCT_PERSONALISATION_ID from tbl_product_personalisation_xref " & _
            "where PRODUCT_CODE = '" & trapSQLInjection(PRODUCT_CODE) & "'" & _
        ")"
        rtn += returnXML(sql, "component")
        rtn += "</components>"
        Return rtn
    End Function

    Private Function getLang() As String
        Dim rtn As String = _
        "<lang>" & _
            "<l name='titlePersonalisations' val='" & getAttributeValue("lang.titlePersonalisations") & "' />" & _
            "<l name='titleControls' val='" & getAttributeValue("lang.titleControls") & "' />" & _
            "<l name='buttonChooseAPlayer' val='" & getAttributeValue("lang.buttonChooseAPlayer") & "' />" & _
            "<l name='buttonAddToBasket' val='" & getAttributeValue("lang.buttonAddToBasket") & "' />" & _
            "<l name='buttonUpdate' val='" & getAttributeValue("lang.buttonUpdate") & "' />" & _
            "<l name='buttonGoBack' val='" & getAttributeValue("lang.buttonGoBack") & "' />" & _
            "<l name='personalisationLoading' val='" & getAttributeValue("lang.personalisationLoading") & "' />" & _
        "</lang>"
        Return rtn
    End Function

    Private Function getPlayers() As String
        Dim rtn As String = getAttributeValue("tables")
        Return rtn
    End Function

    Private Function returnXML(ByVal sql As String, ByVal nodeName As String) As String
        Dim conn As SqlConnection = Nothing
        Dim comm As SqlCommand = Nothing
        Dim drSrv As SqlClient.SqlDataReader
        Dim rtn As New StringBuilder
        Dim i As Integer = 0

        Try
            conn = New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString())
            conn.Open()
            comm = New SqlCommand(sql, conn)
            drSrv = comm.ExecuteReader
            While drSrv.Read
                rtn.Append("<" & nodeName & " ")
                Dim ii As Integer = 0
                While ii < drSrv.FieldCount
                    rtn.Append(drSrv.GetName(ii).ToString().ToLower() & "='" & drSrv.Item(ii) & "' ")
                    ii += 1
                End While
                i += 1
                rtn.Append("/>")
            End While
        Finally
            comm.Dispose()
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
            End If
            conn.Dispose()
        End Try

        Return rtn.ToString

    End Function

    Private Function getXMLConfigValue(ByVal key As String, ByVal x As String) As String
        Dim rtn As String = String.Empty
        x = x.Replace("<![CDATA[", "")
        x = x.Replace("]]>", "")
        If x <> "" Then
            Dim xmlDoc As New XmlDocument
            Dim configNodes As XmlNodeList
            Dim configNode As XmlNode
            Dim baseDataNodes As XmlNodeList

            xmlDoc.LoadXml(x)

            configNodes = xmlDoc.GetElementsByTagName("config")
            For Each configNode In configNodes
                baseDataNodes = configNode.ChildNodes
                For Each baseDataNode As XmlNode In baseDataNodes
                    If baseDataNode.Attributes("name").Value.ToString = key Then
                        rtn = baseDataNode.Attributes("val").Value.ToString
                        Exit For
                    End If
                Next
            Next
        End If
        Return rtn
    End Function

    Private Function override_xml(ByVal ATTRIBUTE_NAME As String) As String
        Dim returnValue As String = String.Empty

        'Check for the setting in the Session("overrideXML")
        If Session("overrideXML") <> "" Then
            returnValue = getXMLConfigValue(ATTRIBUTE_NAME, Session("overrideXML"))
        End If

        Return returnValue
    End Function

    Private Function getAttributeValue(ByVal attributeName As String) As String
        Dim queryStringParm As String = String.Empty
        Dim attributeValue As String = String.Empty
        Dim tDataObjects As New Talent.Common.TalentDataObjects()
        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        attributeValue = tDataObjects.FlashSettings.TblFlashSettings.GetAttributeByBUPartnerPageCodeAttributeQueryString(_businessUnit, _partnerCode, _pageCode, attributeName, queryStringParm)
        Return attributeValue
    End Function

    Private Function tbl_product_personalise(ByVal ATTRIBUTE_NAME As String, ByVal PRODUCT_CODE As String) As String
        Dim returnValue As String = String.Empty
        Dim sql As String = _
        "select XML_CONFIG from tbl_product_personalisation pp " & _
        "inner join tbl_product_personalisation_xref xx on xx.PRODUCT_PERSONALISATION_ID = pp.ID " & _
        "where xx.PRODUCT_CODE = @PRODUCT_CODE"

        Dim cacheKey As String = "XML_CONFIG" & ATTRIBUTE_NAME & PRODUCT_CODE
        Dim CacheTimeMinutes As Integer = 30

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            returnValue = CType(HttpContext.Current.Cache.Item(cacheKey), String)
        Else
            Dim conn As SqlConnection = Nothing
            Dim comm As SqlCommand = Nothing
            Dim drSrv As SqlClient.SqlDataReader = Nothing
            Dim rtn As New StringBuilder
            Try
                conn = New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString())
                conn.Open()
                comm = New SqlCommand(sql, conn)
                comm.Parameters.Add("@PRODUCT_CODE", SqlDbType.NVarChar).Value = PRODUCT_CODE
                drSrv = comm.ExecuteReader
                While drSrv.Read
                    rtn.Append(drSrv.Item(0))
                End While
            Finally
                If Not drSrv Is Nothing Then
                    drSrv.Close()
                End If
                comm.Dispose()
                If (conn.State = ConnectionState.Open) Then
                    conn.Close()
                End If
                conn.Dispose()
            End Try

            HttpContext.Current.Cache.Insert(cacheKey, rtn.ToString, Nothing, System.DateTime.Now.AddMinutes(CacheTimeMinutes), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            returnValue = (CType(HttpContext.Current.Cache.Item(cacheKey), String))
        End If

        Return getXMLConfigValue(ATTRIBUTE_NAME, returnValue)
    End Function

    Private Function trapSQLInjection(ByVal checkStr As String) As String
        Dim rtn As String = String.Empty
        If checkStr <> "" Then rtn = checkStr.Replace("select", "").Replace("drop", "").Replace("union", "").Replace("join", "").Replace("delete", "").Replace("from", "").Replace("'", "").Replace(",", "").Replace(".", "")
        Return rtn
    End Function

End Class
