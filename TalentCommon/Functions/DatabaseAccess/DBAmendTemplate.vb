Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Template xml thingies
'
'       Date                        Mar 2007
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDATAS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBAmendTemplate
    Inherits DBAccess
    Private _dep As DEAmendTemplate
    Public Property Dep() As DEAmendTemplate
        Get
            Return _dep
        End Get
        Set(ByVal value As DEAmendTemplate)
            _dep = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        '-------------------------------------------------------------------------------
        Try
            If Dep.AddNewTemplate Then
                err = Header_Exists(3) 'Check to see if the specific Header exists
            Else
                err = Header_Exists()                                       ' Get header Template_id if it exists
            End If
            If Not err.HasError Then
                '-----------------------------------------------------------------------
                If Dep.AddNewTemplate Then
                    If Dep.TemplateId = 0 Then
                        Dep.CreatedDate = Now
                        Dep.LastAccessedDate = Now
                        Dep.LastModifiedDate = Now
                        err = Header_Insert()                           ' Create header
                        If Not err.HasError Then err = Header_Exists() ' Get header Template_id it should now exist
                    End If
                    If Dep.TemplateId > 0 Then err = Detail_Insert() ' Create details
                End If
                '-----------------------------------------------------------------------
                If Dep.AddToTemplate Then
                    If Dep.TemplateId = 0 Then
                        err = Header_Insert()                           ' Create header
                        If Not err.HasError Then err = Header_Exists() ' Get header Template_id it should now exist
                    End If
                    If Dep.TemplateId > 0 Then err = Detail_Insert() ' Create details
                End If
                '-----------------------------------------------------------------------
                If Dep.DeleteTemplate And Dep.TemplateId > 0 Then
                    err = Detail_Delete(1)                              ' Remove all details
                    If Not err.HasError Then err = Header_Delete(2) ' Remove header
                End If
                '-----------------------------------------------------------------------
                If Dep.DeleteFromTemplate And Dep.TemplateId > 0 Then
                    err = Detail_Delete(2)                              ' Remove specific details
                End If
                '-----------------------------------------------------------------------
                If Dep.ReplaceTemplate Then
                    If Dep.TemplateId > 0 Then                            ' there is a Template so kill, it
                        err = Detail_Delete(1)                          ' Remove all details
                        If Not err.HasError Then err = Header_Delete(2) ' Remove header
                    End If
                    err = Header_Insert()                               ' Create header
                    If Dep.TemplateId > 0 Then err = Detail_Insert() ' Create details
                End If
                '-----------------------------------------------------------------------
            End If
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-21"
                .HasError = True
            End With
        End Try
        '-------------------------------------------------------------------------------
        Return err
    End Function
    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Integer = 0
        '-------------------------------------------------------------------------------
        '   Check valid logon etc.
        '
        If Not Check_Logon Then
            Const strError As String = "Business Unit / Partner / user combination not found"
            With err
                .ErrorMessage = String.Empty
                .ErrorStatus = strError
                .ErrorNumber = "TACDATAS-11"
                .HasError = True
            End With
        End If
        '-----------------------------------------------------------------------
        '   Check each product exists and has enough stock
        '
        For iCounter = 1 To Dep.CollDEAlerts.Count
            Dim dea As DEAlerts = Dep.CollDEAlerts.Item(iCounter)
            With dea
                .Price = Check_Price(.ProductCode)
                '.AvailabilQty = Check_Stock(.ProductCode)
                .Status = "Y"
                If .Price < 0.01 Then
                    err.HasError = True
                    .Description = "Product code not found in price list"
                    .Status = "N"
                End If
                Select Case .AvailabilQty
                    Case Is = -99
                        err.HasError = True
                        .Description = "Product code not found"
                        .Status = "N"
                        'Case Is < .Quantity
                        '    err.HasError = True
                        '    .Description = "Not enough stock available"
                        '    .Status = "N"
                End Select
            End With
        Next
        '-------------------------------------------------------------------------------
        Return err
    End Function

    Protected Function Header_Exists(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString1 As String = " SELECT * FROM tbl_order_template_header WITH (NOLOCK)   " & _
                                                 " WHERE BUSINESS_UNIT = @Param1    " & _
                                                 " AND   PARTNER   = @Param2        " & _
                                                 " AND   LOGINID   = @Param3        "

                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 1
                    Const SQLString2 As String = " SELECT * FROM tbl_order_template_header WITH (NOLOCK)   " & _
                                                " WHERE BUSINESS_UNIT = @Param1     " & _
                                                " AND   LOGINID   = @Param2         "

                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 2
                    Const SQLString3 As String = " SELECT * FROM tbl_order_template_header WITH (NOLOCK)   " & _
                                                " WHERE Template_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString3, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.TemplateId

                Case Is = 3
                    Const SQLString3 As String = " SELECT * FROM tbl_order_template_header WITH (NOLOCK)   " & _
                                                 " WHERE BUSINESS_UNIT = @Param1    " & _
                                                 " AND   PARTNER   = @Param2        " & _
                                                 " AND   LOGINID   = @Param3        " & _
                                                 " AND   NAME   = @Param4        " & _
                                                 " AND DESCRIPTION = @Param5  "

                    cmdSelect = New SqlCommand(SQLString3, conSql2005)
                    With cmdSelect.Parameters
                        .Add(New SqlParameter(Param1, SqlDbType.NVarChar)).Value = Dep.BusinessUnit
                        .Add(New SqlParameter(Param2, SqlDbType.NVarChar)).Value = Dep.PartnerCode
                        .Add(New SqlParameter(Param3, SqlDbType.NVarChar)).Value = Dep.UserID
                        .Add(New SqlParameter(Param4, SqlDbType.NVarChar)).Value = Dep.Name
                        .Add(New SqlParameter(Param5, SqlDbType.NVarChar)).Value = Dep.Description
                    End With

            End Select
            dtr = cmdSelect.ExecuteReader()

            If dtr.HasRows Then
                dtr.Read()
                Dep.TemplateId = dtr("Template_HEADER_ID")
            End If
            '
            dtr.Close()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError8 As String = "Error during Select Template Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-31"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Function Header_Insert() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing

            'If the new template is to be the default, set all the others to false
            If Dep.IsDefault Then
                Dim updateText As String = "  UPDATE tbl_order_template_header " & _
                                            " SET IS_DEFAULT = 'False' " & _
                                            " WHERE BUSINESS_UNIT = @BU " & _
                                            " AND PARTNER = @PARTNER " & _
                                            " AND LOGINID = @LOGINID "
                cmdSelect = New SqlCommand(updateText, conSql2005)
                With cmdSelect
                    .Parameters.Clear()
                    .Parameters.Add(New SqlParameter("@BU", SqlDbType.Char)).Value = Dep.BusinessUnit
                    .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char)).Value = Dep.PartnerCode
                    .Parameters.Add(New SqlParameter("@LOGINID", SqlDbType.Char)).Value = Dep.UserID

                    .ExecuteNonQuery()
                End With
            End If

            Const SQLString As String = " INSERT INTO tbl_order_template_header(NAME,DESCRIPTION,BUSINESS_UNIT, PARTNER, LOGINID, " & _
                                                " CREATED_DATE, LAST_MODIFIED_DATE, LAST_USED_DATE, IS_DEFAULT) " & _
                                                " VALUES ( @NAME,@DESCRIPTION,@BU,@PARTNER,@LOGINID,@CREATED_DATE,@LAST_MOD_DATE,@LAST_ACCESSED,@IS_DEFAULT ) "
            '-----------------------------------------------------------------------
            cmdSelect = New SqlCommand(SQLString, conSql2005)
            With cmdSelect
                .Parameters.Clear()
                .Parameters.Add(New SqlParameter("@NAME", SqlDbType.Char)).Value = Dep.Name
                .Parameters.Add(New SqlParameter("@DESCRIPTION", SqlDbType.Char)).Value = Dep.Description
                .Parameters.Add(New SqlParameter("@BU", SqlDbType.Char)).Value = Dep.BusinessUnit
                .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char)).Value = Dep.PartnerCode
                .Parameters.Add(New SqlParameter("@LOGINID", SqlDbType.Char)).Value = Dep.UserID
                .Parameters.Add(New SqlParameter("@CREATED_DATE", SqlDbType.DateTime)).Value = Dep.CreatedDate
                .Parameters.Add(New SqlParameter("@LAST_MOD_DATE", SqlDbType.DateTime)).Value = Dep.LastModifiedDate
                .Parameters.Add(New SqlParameter("@LAST_ACCESSED", SqlDbType.DateTime)).Value = Dep.LastAccessedDate
                .Parameters.Add(New SqlParameter("@IS_DEFAULT", SqlDbType.Bit)).Value = Dep.IsDefault

                .ExecuteNonQuery()
            End With
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Insert Template Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-32"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Function Header_Update(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            '
            Select Case Action
                Case Is = 0
                    Const SQLString2 As String = " UPDATE tbl_order_template_header SET" & _
                                              "   NAME = @Param1 " & _
                                              "  ,DESCRIPTION = @Param2 " & _
                                              "  ,IS_DEFAULT = @Param3 " & _
                                              "  ,LAST_MODIFIED_DATE = @Param4 " & _
                                              "  ,LAST_USED_DATE = @Param5 " & _
                                              " WHERE Template_HEADER_ID = @Param6 "

                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.Name
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.Description
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.IsDefault
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.DateTime)).Value = Now
                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.DateTime)).Value = Now
                        .Parameters.Add(New SqlParameter(Param6, SqlDbType.BigInt)).Value = Dep.TemplateId
                        .ExecuteNonQuery()
                    End With

            End Select
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Update Template Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-33"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Function Header_Delete(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString1 As String = " DELETE FROM tbl_order_template_header    " & _
                                                 " WHERE BUSINESS_UNIT = @Param1    " & _
                                                 " AND   PARTNER   = @Param2        " & _
                                                 " AND   LOGINID   = @Param3        "
                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 1
                    Const SQLString2 As String = " DELETE FROM tbl_order_template_header    " & _
                                                " WHERE BUSINESS_UNIT = @Param1     " & _
                                                " AND   LOGINID   = @Param2         "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 2
                    Const SQLString3 As String = " DELETE FROM tbl_order_template_header    " & _
                                                " WHERE Template_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString3, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.TemplateId

            End Select
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError8 As String = "Error during Select Template Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-34"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Detail_Select(ByVal id As Long, Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim dtr As SqlDataReader = Nothing
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString0 As String = " SELECT * FROM tbl_order_template_detail WITH (NOLOCK)  WHERE Template_HEADER_ID= @Param1   "
                    cmdSelect = New SqlCommand(SQLString0, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = id
                        dtr = .ExecuteReader
                    End With

                Case Is = 1
                    Const SQLString1 As String = " SELECT * FROM tbl_order_template_detail WITH (NOLOCK)  WHERE Template_DETAIL_ID= @Param1   "
                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = id
                        dtr = .ExecuteReader
                    End With

                Case Is = 2
                    Const SQLString2 As String = _
                        " SELECT COUNT(PR.PRODUCT_CODE) AS NO_OF_ITEMS, " & _
                        "        SUM(PL.GROSS_PRICE * BD.QUANTITY) AS TOTAL_VALUE_GROSS, " & _
                        "        SUM(PL.NET_PRICE * BD.QUANTITY) AS TOTAL_VALUE_NET" & _
                        " FROM tbl_order_template_detail BD WITH (NOLOCK)  " & _
                        "   INNER Join tbl_product PR WITH (NOLOCK)  " & _
                        "       ON BD.PRODUCT = PR.PRODUCT_CODE " & _
                        "   INNER Join tbl_price_list_detail PL WITH (NOLOCK)  " & _
                        "       ON PR.PRODUCT_CODE = PL.PRODUCT" & _
                        " WHERE  PRICE_LIST = " & _
                                 " (SELECT VALUE FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)   " & _
                                "   WHERE  BUSINESS_UNIT = @Param1 AND PARTNER = @Param2 " & _
                                "   AND DEFAULT_NAME = 'PRICE_LIST') " & _
                        " AND BD.Template_HEADER_ID = @Param3  "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.TemplateId
                        dtr = .ExecuteReader
                    End With

                Case Is = 3
                    Const SQLString2 As String = _
                        " SELECT PR.PRODUCT_CODE, PR.PRODUCT_DESCRIPTION_1 AS PRODUCT_TITLE, PL.PRICE_LIST, " & _
                        "        PL.FROM_DATE AS PRICE_LIST_FROM_DATE, PL.TO_DATE AS PRICE_LIST_TO_DATE," & _
                        "        PL.NET_PRICE AS PRICE_NET, PL.GROSS_PRICE AS PRICE_GROSS, BD.Template_HEADER_ID, " & _
                        "        BD.QUANTITY AS QTY, PL.GROSS_PRICE * BD.QUANTITY AS VALUE_GROSS, " & _
                        "        PL.NET_PRICE * BD.QUANTITY AS VALUE_NET  " & _
                        " FROM tbl_order_template_detail BD WITH (NOLOCK)  " & _
                        "   INNER Join tbl_product PR WITH (NOLOCK)  " & _
                        "       ON BD.PRODUCT = PR.PRODUCT_CODE " & _
                        "   INNER Join tbl_price_list_detail PL WITH (NOLOCK)  " & _
                        "       ON PR.PRODUCT_CODE = PL.PRODUCT" & _
                        " WHERE  PRICE_LIST = " & _
                                " (SELECT VALUE FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)   " & _
                                "   WHERE  BUSINESS_UNIT = @Param1 AND PARTNER = @Param2 " & _
                                "   AND DEFAULT_NAME = 'PRICE_LIST') " & _
                        " AND BD.Template_HEADER_ID = @Param3 "

                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.TemplateId
                        dtr = .ExecuteReader
                    End With
            End Select
            If dtr.HasRows Then
                '   Load the Datareader into what ever to return

            End If
            dtr.Close()
            cmdSelect = Nothing
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Select Template Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-41"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Function Detail_Insert() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing

            Const SQLString As String = _
                            " IF EXISTS " & vbCrLf & _
                                " ( SELECT * FROM tbl_order_template_detail WITH (NOLOCK)  " & vbCrLf & _
                                "   WHERE NAME = @Param0 AND PRODUCT_CODE = @Param9 ) " & vbCrLf & _
                                    " UPDATE tbl_order_template_detail " & vbCrLf & _
                                    " SET QUANTITY = QUANTITY + @Param1 " & vbCrLf & _
                                    " WHERE NAME = @Param3 AND PRODUCT_CODE = @Param4 " & vbCrLf & _
                            " ELSE " & vbCrLf & _
                                        " INSERT INTO tbl_order_template_detail (Template_HEADER_ID, PRODUCT_CODE, QUANTITY,MASTER_PRODUCT, NAME ) " & vbCrLf & _
                                        " VALUES ( @Param5, @Param6, @Param7,'', @Param8 ) " & vbCrLf
            '-----------------------------------------------------------------------
            '   Check each product exists and has enough stock
            '
            For iCounter = 1 To Dep.CollDEAlerts.Count
                dea = Dep.CollDEAlerts.Item(iCounter)
                cmdSelect = New SqlCommand(SQLString, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param0, SqlDbType.NVarChar)).Value = Dep.Name

                    .Parameters.Add(New SqlParameter(Param9, SqlDbType.Char)).Value = dea.ProductCode

                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Decimal)).Value = dea.Quantity
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.NVarChar)).Value = Dep.Name
                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = dea.ProductCode
                    '
                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.BigInt)).Value = Dep.TemplateId
                    .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = dea.ProductCode
                    .Parameters.Add(New SqlParameter(Param7, SqlDbType.Decimal)).Value = dea.Quantity
                    .Parameters.Add(New SqlParameter("@PARAM8", SqlDbType.NVarChar)).Value = Dep.Name
                    .ExecuteNonQuery()
                End With
            Next
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Insert Template Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-42"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Function Detail_Update(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString1 As String = " UPDATE tbl_order_template_detail SET " & _
                                                "    Template_HEADER_ID = @Param1 " & _
                                                "   ,PRODUCT_CODE = @Param2 " & _
                                                "   ,QUANTITY = @Param3 " & _
                                                " WHERE Template_DETAIL_ID = @Param5 "
                    '----------------------------------------------------------------------
                    For iCounter = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iCounter)
                        cmdSelect = New SqlCommand(SQLString1, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.TemplateId
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = dea.ProductCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = dea.Quantity
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.BigInt)).Value = dea.Id
                            .ExecuteNonQuery()
                        End With
                    Next
                    '--------------------------------------------------------------------------
                Case Is = 1
                    Const SQLString2 As String = " UPDATE tbl_order_template_detail SET " & _
                                                "    QUANTITY = @Param1 " & _
                                                " WHERE Template_HEADER_ID = @Param3 " & _
                                                " AND   PRODUCT_CODE = @Param4 "
                    '-----------------------------------------------------------------------
                    For iCounter = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iCounter)
                        cmdSelect = New SqlCommand(SQLString2, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = dea.Quantity
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.TemplateId
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = dea.ProductCode
                            .ExecuteNonQuery()
                        End With
                    Next
                    '--------------------------------------------------------------------------
            End Select
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing

        Catch ex As Exception
            Const strError8 As String = "Error during Update Template Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-43"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Function Detail_Delete(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                ''Case Is = 0
                ''    Const SQLString1 As String = " DELETE FROM tbl_order_template_detail   " & _
                ''                                 " WHERE Template_DETAIL_ID = @Param1  "
                ''    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                ''    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = id
                ''    cmdSelect.ExecuteNonQuery()

                Case Is = 1
                    Const SQLString2 As String = " DELETE FROM tbl_order_template_detail    " & _
                                                 " WHERE Template_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.TemplateId
                    cmdSelect.ExecuteNonQuery()

                Case Is = 2
                    Const SQLString2 As String = " DELETE FROM tbl_order_template_detail    " & _
                                                 " WHERE NAME = @Param1  " & _
                                                 " AND PRODUCT_CODE = @Param2 "
                    '-----------------------------------------------------------------------
                    For iCounter = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iCounter)
                        cmdSelect = New SqlCommand(SQLString2, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.NVarChar)).Value = Dep.Name
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = dea.ProductCode
                            .ExecuteNonQuery()
                        End With
                    Next

            End Select
            cmdSelect = Nothing
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Delete Template Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDATAS-44"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function IsGUID(ByVal str As String) As Boolean
        Try
            Dim re As New System.Text.RegularExpressions.Regex("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", Text.RegularExpressions.RegexOptions.IgnoreCase)
            Return re.IsMatch(str)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Protected ReadOnly Property Check_Logon() As Boolean
        Get
            Dim LogonOk As Boolean = False
            '--------------------------------------------------------------------
            If IsGUID(Dep.UserID) Then
                Return Not LogonOk
            Else
                Try
                    Dim dtr As SqlDataReader = Nothing
                    Dim cmdSelect As SqlCommand = Nothing
                    Const SQLString As String = " SELECT * FROM tbl_authorized_users WITH (NOLOCK)     " & _
                                                     " WHERE BUSINESS_UNIT = @Param1    " & _
                                                     " AND   PARTNER   = @Param2        " & _
                                                     " AND   LOGINID   = @Param3        "
                    cmdSelect = New SqlCommand(SQLString, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                        dtr = .ExecuteReader
                    End With
                    If dtr.HasRows Then LogonOk = True
                    dtr.Close()
                    cmdSelect = Nothing
                    '--------------------------------------------------------------------------
                Catch ex As Exception

                End Try
            End If
            '--------------------------------------------------------------------
            Return LogonOk
        End Get
    End Property
    Protected ReadOnly Property Check_Price(ByVal ProductCode As String) As Double
        Get
            Dim dPrice As Double = 0
            '--------------------------------------------------------------------
            Try
                Dim dtr As SqlDataReader = Nothing
                Dim cmdSelect As SqlCommand = Nothing

                Const SQLString2 As String = _
                            " SELECT CASE WHEN SALE_GROSS_PRICE <> 0 THEN SALE_GROSS_PRICE ELSE GROSS_PRICE END AS PRICE " & _
                            " FROM tbl_price_list_detail WITH (NOLOCK)  WHERE  PRICE_LIST = " & _
                                    " (SELECT VALUE FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)   " & _
                                    "   WHERE  BUSINESS_UNIT = @Param1 AND PARTNER = @Param2 " & _
                                    "   AND DEFAULT_NAME = 'PRICE_LIST') " & _
                            " AND PRODUCT = @Param3 "

                cmdSelect = New SqlCommand(SQLString2, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = ProductCode
                    dtr = .ExecuteReader
                End With
                If dtr.HasRows Then
                    dtr.Read()
                    dPrice = dtr("PRICE")
                Else
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = ProductCode
                        dtr = .ExecuteReader
                    End With
                    If dtr.HasRows Then
                        dtr.Read()
                        dPrice = dtr("PRICE")
                    Else
                        cmdSelect = New SqlCommand(SQLString2, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = ProductCode
                            dtr = .ExecuteReader
                        End With
                        If dtr.HasRows Then
                            dtr.Read()
                            dPrice = dtr("PRICE")
                        End If
                    End If
                End If
                dtr.Close()
                cmdSelect = Nothing
                '--------------------------------------------------------------------------
            Catch ex As Exception

            End Try
            '--------------------------------------------------------------------
            Return dPrice
        End Get
    End Property

End Class
