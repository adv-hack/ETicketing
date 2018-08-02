Option Strict On
Imports Talent.Common
Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data

Public Class Utilities

    Public Shared Function GetPropertyNames(ByVal obj As Object) As ArrayList
        Dim al As New ArrayList
        Dim inf() As System.Reflection.PropertyInfo = obj.GetType.GetProperties
        For Each info As System.Reflection.PropertyInfo In inf
            al.Add(info.Name)
        Next
        Return al

    End Function

    Public Shared Function CheckForDBNull_Int(ByVal obj As Object) As Integer
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CInt(obj)
    End Function
    Public Shared Function CheckForDBNull_String(ByVal obj As Object) As String
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return String.Empty Else Return CStr(obj)
    End Function
    Public Shared Function CheckForDBNull_Decimal(ByVal obj As Object) As Decimal
        If obj.Equals(DBNull.Value) Then Return 0 Else Return CDec(obj)
    End Function
    Public Shared Function CheckForDBNull_Boolean_DefaultFalse(ByVal obj As Object) As Boolean
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return False Else Return CBool(obj)
    End Function
    Public Shared Function CheckForDBNull_Boolean_DefaultTrue(ByVal obj As Object) As Boolean
        If obj.Equals(DBNull.Value) Then Return True Else Return CBool(obj)
    End Function
    Public Shared Function CheckIsDBNull(ByVal obj As Object) As Boolean
        If obj.Equals(DBNull.Value) Then Return True Else Return False
    End Function

    Public Shared Function CheckForEmptyString(ByVal input As String, ByVal defaultReturnValue As Object) As String
        If String.IsNullOrEmpty(input) Then Return defaultReturnValue.ToString Else Return input
    End Function

    Public Shared Function GetDropDownControlText(ByVal language As String, _
                                                    ByVal Qualifier As String, _
                                                    ByVal control As String, _
                                                    ByVal BusinessUnit As String, _
                                                    ByVal Partner As String, _
                                                    Optional ByVal type As String = "") _
                                                            As ListItemCollection
        Dim lic As New ListItemCollection
        Dim li As ListItem
        Dim dt As New Data.DataTable

        Dim exists As Boolean = False

        dt = GetDDLDataTable(dt, control, Qualifier, BusinessUnit, Partner, type)

        For Each row As Data.DataRow In dt.Rows
            For Each item As ListItem In lic
                If item.Value = row(control & "_CODE").ToString Then
                    exists = True
                    Exit For
                End If
            Next
            If Not exists Then
                li = New ListItem(row(control & "_DESCRIPTION").ToString, row(control & "_CODE").ToString)
                lic.Add(li)
            Else
                exists = False
            End If
        Next

        For Each item As ListItem In lic
            For Each row As Data.DataRow In dt.Rows

                If row(control & "_CODE").ToString = item.Value AndAlso Not String.IsNullOrEmpty(language) AndAlso CheckIsDBNull(row(control & "_LANGUAGE").ToString = language) Then
                    item.Text = row("LANGUAGE_DESCRIPTION").ToString
                End If
            Next
        Next
        If lic.Count > 0 Then
            lic.Insert(0, New ListItem(" -- ", " -- "))
        End If
        Return lic
    End Function

    Public Shared Function SQLDataValue(ByVal Value As Object, ByVal DataType As System.Type) As String

        Select Case DataType.ToString()
            Case "System.Char"
                Return "'" & Value.ToString() & "'"

            Case "System.String"
                Return "'" & Value.ToString().Replace("'", "''") & "'"

            Case "System.DateTime"
                Dim sDate As DateTime = CType(Value, DateTime)
                Return "'" & sDate.ToString("dd-MMM-yyyy HH:mm:ss") & "'"

            Case "System.Boolean"
                Return "'" & Value.ToString() & "'"

            Case Else
                Return Value.ToString()

        End Select

        Return ""

    End Function

    Protected Shared Function GetDDLDataTable(ByVal dt As Data.DataTable, _
                                                ByVal control As String, _
                                                ByVal qualifier As String, _
                                                ByVal BusinessUnit As String, _
                                                ByVal Partner As String, _
                                                Optional ByVal type As String = "") As Data.DataTable
        control = UCase(control)
        Dim all As String = Talent.Common.Utilities.GetAllString
        Return GetPropertyFromDB(BusinessUnit, Partner, control, qualifier, type)

    End Function

    Protected Shared Function GetPropertyFromDB(ByVal bu As String, ByVal part As String, ByVal control As String, ByVal qualifier As String, Optional ByVal type As String = "") As Data.DataTable
        Dim all As String = Talent.Common.Utilities.GetAllString
        Dim titles As New TalentApplicationVariablesTableAdapters.tbl_title_buTableAdapter
        Dim countries As New TalentApplicationVariablesTableAdapters.tbl_country_buTableAdapter
        Dim desc As New TalentDescriptionsDataSetTableAdapters.tbl_ebusiness_descriptions_buTableAdapter
        Dim card As New TalentApplicationVariablesTableAdapters.tbl_creditcard_buTableAdapter
        Dim dt As New Data.DataTable


        'If no result, try to get from DB
        If control = "TITLE" Then
            dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(qualifier, bu, part)
        ElseIf control = "COUNTRY" Then
            dt = countries.Get_By_BU_Partner_Country_Lang(qualifier, bu, part)
        ElseIf control = "DESCRIPTION" Then
            dt = desc.GetDatabyBUetc(qualifier, bu, part, type)
        ElseIf control = "CARD" Then
            dt = card.Get_CreditCards(qualifier, bu, part)
        End If

        If dt.Rows.Count > 0 Then
        Else

            'If this is no cache here either
            'Try to get from DB using *ALL as qualifier
            If control = "TITLE" Then
                dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(all, bu, part)
            ElseIf control = "COUNTRY" Then
                dt = countries.Get_By_BU_Partner_Country_Lang(all, bu, part)
            ElseIf control = "DESCRIPTION" Then
                dt = desc.GetDatabyBUetc(all, bu, part, type)
            ElseIf control = "CARD" Then
                dt = card.Get_CreditCards(all, bu, part)
            End If

            If dt.Rows.Count > 0 Then
            Else

                'If this is no cache here either
                'Try to get from DB using *ALL as qualifier and partner
                If control = "TITLE" Then
                    dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(all, bu, all)
                ElseIf control = "COUNTRY" Then
                    dt = countries.Get_By_BU_Partner_Country_Lang(all, bu, all)
                ElseIf control = "DESCRIPTION" Then
                    dt = desc.GetDatabyBUetc(all, bu, all, type)
                ElseIf control = "CARD" Then
                    dt = card.Get_CreditCards(all, bu, all)
                End If

                If dt.Rows.Count > 0 Then
                Else
                    'If this is no cache here either
                    'Try to get from DB using *ALL as qualifier and partner
                    If control = "TITLE" Then
                        dt = titles.Get_Titles_By_Lang_BU_Partner_Qualifier(all, all, all)
                    ElseIf control = "COUNTRY" Then
                        dt = countries.Get_By_BU_Partner_Country_Lang(all, all, all)
                    ElseIf control = "DESCRIPTION" Then
                        dt = desc.GetDatabyBUetc(all, all, all, type)
                    ElseIf control = "CARD" Then
                        dt = card.Get_CreditCards(all, all, all)
                    End If
                End If
            End If
        End If
        Return dt
    End Function


    Public Shared Function Serialize(ByVal objectToSerialize As Object) As Byte()
        Dim ms As New System.IO.MemoryStream
        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        bf.Serialize(ms, objectToSerialize)

        Dim enc As System.Text.Encoding = New System.Text.ASCIIEncoding
        Dim bytes(CInt(ms.Length)) As Byte
        bytes = ms.ToArray

        Return bytes
    End Function

    Public Shared Function DeSerialize(ByVal objectByteArray As Byte(), ByVal ObjectToPopulate As Object) As Object
        Dim ms As New System.IO.MemoryStream(objectByteArray)
        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        ObjectToPopulate = bf.Deserialize(ms)
        Return ObjectToPopulate
    End Function

    Public Shared Sub SerializeObject(ByVal ObjectToStore As Object, _
                                        ByVal objectType As Type, _
                                        ByVal BusinessUnit As String, _
                                        ByVal Partner As String, _
                                        ByVal LoginID As String, _
                                        ByVal Origin As String, _
                                        Optional ByVal UniqueID As String = "")

        Dim serializer As New SerializedObjectsDataSetTableAdapters.tbl_serialized_objectsTableAdapter
        Dim serializedObject As Byte() = Serialize(ObjectToStore)

        Select Case objectType.Name
            Case Is = "TalentOrder"
                Dim orders As SerializedObjectsDataSet.tbl_serialized_objectsDataTable
                orders = serializer.Get_Order_Unprocessed(objectType.Name, _
                                                            Origin, _
                                                            BusinessUnit, _
                                                            Partner, _
                                                            LoginID, _
                                                            UniqueID)
                If orders.Rows.Count > 0 Then
                    serializer.Update_Order_Object(serializedObject, _
                                                    Now, _
                                                    objectType.Name, _
                                                    Origin, _
                                                    BusinessUnit, _
                                                    Partner, _
                                                    LoginID, _
                                                    UniqueID)
                Else
                    serializer.AddSerializedOrder(objectType.Name, _
                                                    Origin, _
                                                    BusinessUnit, _
                                                    Partner, _
                                                    LoginID, _
                                                    UniqueID, _
                                                    serializedObject, _
                                                    Now)

                End If

            Case Is = "TalentCustomer"
                Dim customers As SerializedObjectsDataSet.tbl_serialized_objectsDataTable
                customers = serializer.Get_Customer_Unprocessed(objectType.Name, _
                                                                Origin, _
                                                                BusinessUnit, _
                                                                Partner, _
                                                                LoginID)
                If customers.Rows.Count > 0 Then
                    serializer.Update_Customer_Object(serializedObject, _
                                                        Now, _
                                                        objectType.Name, _
                                                        Origin, _
                                                        BusinessUnit, _
                                                        Partner, _
                                                        LoginID)
                Else
                    serializer.AddSerializedCustomer(objectType.Name, _
                                                    Origin, _
                                                    BusinessUnit, _
                                                    Partner, _
                                                    LoginID, _
                                                    serializedObject, _
                                                    Now)
                End If

            Case Else
                Dim serialized As SerializedObjectsDataSet.tbl_serialized_objectsDataTable
                serialized = serializer.GetBy_ObjType_Origin_BU_Partner_Login_UniqueID_Unprocessed(objectType.Name, _
                                                                                                    Origin, _
                                                                                                    BusinessUnit, _
                                                                                                    Partner, _
                                                                                                    LoginID, _
                                                                                                    UniqueID)

                If serialized.Rows.Count > 0 Then
                    serializer.Update_Object(serializedObject, _
                                                Now, _
                                                objectType.Name, _
                                                Origin, _
                                                BusinessUnit, _
                                                Partner, _
                                                LoginID, _
                                                UniqueID)

                Else
                    serializer.AddSerializedObject(objectType.Name, _
                                                Origin, _
                                                BusinessUnit, _
                                                Partner, _
                                                LoginID, _
                                                UniqueID, _
                                                serializedObject, _
                                                Now, _
                                                False, _
                                                Nothing, _
                                                0)

                End If

        End Select

    End Sub

    Public Shared Function DeserializeObject(ByVal ObjectToPopulate As Object, _
                                                ByVal objectType As Type, _
                                                ByVal BusinessUnit As String, _
                                                ByVal Partner As String, _
                                                ByVal LoginID As String, _
                                                ByVal Origin As String, _
                                                Optional ByVal UniqueID As String = "") As Object

        Dim serializer As New SerializedObjectsDataSetTableAdapters.tbl_serialized_objectsTableAdapter
        Dim serializedObjects As New SerializedObjectsDataSet.tbl_serialized_objectsDataTable
        Dim serializedObject As SerializedObjectsDataSet.tbl_serialized_objectsRow

        Select Case objectType.Name
            Case Is = "TalentOrder"
                ObjectToPopulate = New TalentOrder
                serializedObjects = serializer.Get_Order_Unprocessed(objectType.Name, _
                                                                        Origin, _
                                                                        BusinessUnit, _
                                                                        Partner, _
                                                                        LoginID, _
                                                                        UniqueID)
            Case Is = "TalentCustomer"
                ObjectToPopulate = New TalentCustomer
                serializedObjects = serializer.Get_Customer_Unprocessed(objectType.Name, _
                                                                        Origin, _
                                                                        BusinessUnit, _
                                                                        Partner, _
                                                                        LoginID)

            Case Else
                serializedObjects = serializer.GetBy_ObjType_Origin_BU_Partner_Login_UniqueID_Unprocessed(objectType.Name, _
                                                                                                            Origin, _
                                                                                                            BusinessUnit, _
                                                                                                            Partner, _
                                                                                                            LoginID, _
                                                                                                            UniqueID)
        End Select

        If serializedObjects.Rows.Count > 0 Then
            serializedObject = CType(serializedObjects.Rows(0), SerializedObjectsDataSet.tbl_serialized_objectsRow)
            ObjectToPopulate = DeSerialize(serializedObject.SERIALIZED_OBJECT, ObjectToPopulate)
        End If

        Return ObjectToPopulate

    End Function

    Public Shared Function getSQLVal(ByVal sql As String, Optional ByVal conStr As String = "") As String
        If conStr = "" Then conStr = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        Dim conn As SqlConnection = Nothing
        Dim comm As SqlCommand = Nothing
        Dim drSrv As SqlClient.SqlDataReader = Nothing
        Dim rtn As New StringBuilder
        Try
            conn = New SqlConnection(conStr)
            conn.Open()
            comm = New SqlCommand(sql, conn)
            drSrv = comm.ExecuteReader
            Dim i As Integer = 0
            While drSrv.Read
                rtn.Append(drSrv.Item(0).ToString)
            End While
        Finally
            drSrv.Close()
            comm.Dispose()
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
            End If
            conn.Dispose()
        End Try
        Return rtn.ToString
    End Function

    Public Shared Function GetBusinessUnit() As String
        Dim url() As String
        'Get the url and split it on the "/" char
        url = HttpContext.Current.Request.Url.ToString.Split(CChar("/"))

        ' Check for localhost:port (debugging locally) and treat as localhost
        Dim j As Integer = 0
        If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
            Do While j < url.Length
                If url(j).Contains("localhost:") Then
                    url(j) = "localhost"
                    Exit Do
                End If
                j += 1
            Loop
        End If
        '-------------------------
        ' Try to get BU from DB
        '-------------------------
        Return GetBusinessUnitFromDB(url)
    End Function

    Protected Shared Function GetBusinessUnitFromDB(ByVal url() As String) As String
        Dim buTbl As New TalentApplicationVariablesTableAdapters.tbl_url_buTableAdapter
        Dim cacheString As String
        'Loop backwards through the length of the array
        For i As Integer = url.Length - 1 To 0 Step -1
            cacheString = GetNextURLString(url, i)
            If HttpContext.Current.Cache("BU" & cacheString) Is Nothing Then
                Dim dt As System.Data.DataTable = buTbl.GetData_By_URL(cacheString)
                If dt.Rows.Count > 0 Then
                    AddPropertyToCache("BU" & cacheString, dt.Rows(0).Item("BUSINESS_UNIT"), 30, TimeSpan.Zero, CacheItemPriority.Normal)
                    Return dt.Rows(0).Item("BUSINESS_UNIT").ToString
                End If
            Else
                Return CType(HttpContext.Current.Cache("BU" & cacheString), String)
            End If
            
        Next
        Return String.Empty
    End Function

    Protected Shared Function GetNextURLString(ByVal url() As String, ByVal i As Integer) As String
        Dim cacheString As String = String.Empty
        'Contruct the url string
        cacheString = String.Empty
        For j As Integer = 0 To i
            If url(j) = "http:" OrElse url(j) = "https:" OrElse url(j) = "" Then
            Else
                cacheString += url(j) & "/"
            End If
        Next

        'Remove the end "/"
        If cacheString.EndsWith("/") Then
            cacheString = cacheString.TrimEnd(CChar("/"))
        End If

        Return cacheString
    End Function

    Public Shared Function GetSettingsObject(Optional ByVal withConnStringList As Boolean = True) As Talent.Common.DESettings
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As New Talent.Common.DESettings
        settings.BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.OriginatingSourceCode = GlobalConstants.SOURCE
        tDataObjects.Settings = settings

        settings.CacheDependencyPath = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(GlobalConstants.MAINTENANCEBUSINESSUNIT, "CACHE_DEPENDENCY_PATH")
        settings.StoredProcedureGroup = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(GlobalConstants.MAINTENANCEBUSINESSUNIT, "STORED_PROCEDURE_GROUP")
        settings.Cacheing = False
        settings.CacheStringExtension = String.Empty

        Return settings
    End Function

    Public Shared Function GetDefaultPartner() As String

        Dim dt As New System.Data.DataTable

        Try

            ' Try to retrieve the table from cache
            dt = CType(HttpContext.Current.Cache("DefaultPartnersTable"), System.Data.DataTable)
            If dt Is Nothing Then

                ' Populate from the database when the cache retrieval fails
                Dim partners As New TalentApplicationVariablesTableAdapters.tbl_authorized_partnersTableAdapter
                dt = partners.Get_Default_Partners
                If Not dt Is Nothing Then

                    ' Add the table to cache
                    AddPropertyToCache("DefaultPartnersTable", dt, 30, TimeSpan.Zero, CacheItemPriority.Normal)
                End If
            End If

            'Retrieve the default partner for this business unit
            If Not dt Is Nothing Then
                For Each row As System.Data.DataRow In dt.Rows
                    If row("BUSINESS_UNIT").ToString = GetBusinessUnit() Then
                        Return row("PARTNER").ToString
                    End If
                Next
            End If

            Return Talent.Common.Utilities.GetAllString

        Catch ex As Exception
            Return Talent.Common.Utilities.GetAllString
        End Try

    End Function

    Public Shared Sub AddPropertyToCache(ByVal CacheName As String, ByVal value As Object, ByVal ExpireInXMinutes As Integer, ByVal SlidingTime As System.TimeSpan, ByVal Priority As CacheItemPriority)
        Try
            If HttpContext.Current.Cache(CacheName) Is Nothing Then
                HttpContext.Current.Cache.Add(CacheName, value, Nothing, Now.AddMinutes(ExpireInXMinutes), SlidingTime, Priority, Nothing)
            End If
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(CacheName)
        Catch ex As Exception
        End Try

    End Sub


    ''' <summary>
    ''' Gets the SMTP Port number from web.config
    ''' </summary>
    ''' <returns>Integer</returns>
    Public Shared Function GetSMTPPortNumber() As Integer
        Dim _smtpPortNumber As Integer = 25
        Try
            Dim _smtpPortNumberString As String = String.Empty
            _smtpPortNumberString = ConfigurationManager.AppSettings("EmailSMTPPortNumber").ToString.Trim
            If Not String.IsNullOrEmpty(_smtpPortNumberString) Then
                _smtpPortNumber = CInt(_smtpPortNumberString)
            Else
                _smtpPortNumber = 25
            End If
        Catch ex As Exception
            _smtpPortNumber = 25
        End Try
        Return _smtpPortNumber
    End Function
    Public Shared Function FindWebControl(ByVal controlID As String, ByVal controls As System.Web.UI.ControlCollection) As Control
        Dim foundControl As Control = Nothing
        Try
            Dim found As Boolean = False
            For Each ctrl As Control In controls
                If ctrl.ID = controlID Then
                    found = True
                    foundControl = ctrl
                    Exit For
                End If
            Next
            If Not found Then
                For Each ctrl As Control In controls
                    If ctrl.Controls.Count > 0 Then
                        foundControl = FindWebControl(controlID, ctrl.Controls)
                        If Not foundControl Is Nothing Then
                            Exit For
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            foundControl = Nothing
        End Try

        Return foundControl
    End Function

    Public Shared Function GetConnectionStringList() As Generic.List(Of String)
        Dim connStringList As New Generic.List(Of String)
        Dim tempConnectionString As String = String.Empty
        If TryGetConnectionString("liveUpdateDatabase01", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase02", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase03", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase04", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase05", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase06", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase07", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase08", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase09", tempConnectionString) Then connStringList.Add(tempConnectionString)
        If TryGetConnectionString("liveUpdateDatabase10", tempConnectionString) Then connStringList.Add(tempConnectionString)
        Return connStringList
    End Function

    Private Shared Function TryGetConnectionString(ByVal appSetConnVariableName As String, ByRef connectionString As String) As Boolean
        Dim isExists As Boolean = False
        connectionString = ConfigurationManager.AppSettings(appSetConnVariableName).Trim()
        If connectionString.Length <= 0 Then
            connectionString = String.Empty
            isExists = False
        Else
            isExists = True
        End If
        Return isExists
    End Function

    Public Shared Function isEnabled(ByVal webConfigKey As String) As Boolean
        Dim canEnable As Boolean = True
        Try
            Dim settingsValue As String = ConfigurationManager.AppSettings(webConfigKey)
            If (Not String.IsNullOrWhiteSpace(settingsValue)) Then
                If Not CBool(settingsValue) Then
                    canEnable = False
                End If
            End If
        Catch ex As Exception
            canEnable = True
        End Try
        Return canEnable
    End Function

    Public Shared Function TranslateEmailTemplateType(ByVal sInput As String, ByVal mode As String) As String


        Dim sOutput As String = String.Empty

        Dim _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        Dim _ucr1 As New Talent.Common.UserControlResource
        With _ucr1
            .BusinessUnit = "MAINTENANCE"
            .PageCode = String.Empty
            .PartnerCode = "*ALL"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "GlobalEmailTemplateTypes"
        End With

        ' Translate from global constant to variable
        If mode = "0" Then
            Select Case sInput.ToString.ToLower
                Case Is = GlobalConstants.EMAIL_ORDER_CONFIRMATION.ToLower
                    sOutput = _ucr1.Content("EMAIL_ORDER_CONFIRMATION", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_TICKETING_UPGRADE.ToLower
                    sOutput = _ucr1.Content("EMAIL_TICKETING_UPGRADE", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_TICKETING_TRANSFER.ToLower
                    sOutput = _ucr1.Content("EMAIL_TICKETING_TRANSFER", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_TICKETING_CANCEL.ToLower
                    sOutput = _ucr1.Content("EMAIL_TICKETING_CANCEL", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_FORGOTTEN_PASSWORD.ToLower
                    sOutput = _ucr1.Content("EMAIL_FORGOTTEN_PASSWORD", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_CHANGE_PASSWORD.ToLower
                    sOutput = _ucr1.Content("EMAIL_CHANGE_PASSWORD", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_CUSTOMER_REGISTRATION.ToLower
                    sOutput = _ucr1.Content("EMAIL_CUSTOMER_REGISTRATION", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM.ToLower
                    sOutput = _ucr1.Content("EMAIL_ORDER_RETURN_CONFIRM", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM_REBOOK.ToLower
                    sOutput = _ucr1.Content("EMAIL_ORDER_RETURN_CONFIRM_REBOOK", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_PPS_PAYMENT_CONFIRMATION.ToLower
                    sOutput = _ucr1.Content("EMAIL_PPS_PAYMENT_CONFIRMATION", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_PPS_PAYMENT_FAILURE.ToLower
                    sOutput = _ucr1.Content("EMAIL_PPS_PAYMENT_FAILURE", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_PPS_AMEND.ToLower
                    sOutput = _ucr1.Content("EMAIL_PPS_AMEND", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_PPS_AMEND_PAYMENT.ToLower
                    sOutput = _ucr1.Content("EMAIL_PPS_AMEND_PAYMENT", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_RETAIL_ORDER_CONFIRMATION.ToLower
                    sOutput = _ucr1.Content("EMAIL_RETAIL_ORDER_CONFIRMATION", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_CONTACT_US.ToLower
                    sOutput = _ucr1.Content("EMAIL_CONTACT_US", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_TICKET_EXCHANGE_CONFIRM.ToLower
                    sOutput = _ucr1.Content("EMAIL_TICKET_EXCHANGE_CONFIRM", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_TICKET_EXCHANGE_SALE_CONFIRM.ToLower
                    sOutput = _ucr1.Content("EMAIL_TICKET_EXCHANGE_SALE_CONFIRM", _languageCode, True)
                Case Is = GlobalConstants.EMAIL_HOSPITALITY_Q_AND_A_REMINDER.ToLower
                    sOutput = _ucr1.Content("EMAIL_HOSPITALITY_Q_AND_A_REMINDER", _languageCode, True)
            End Select
        End If

        ' Translate from variable to global constant
        If mode = "1" Then
            Select Case sInput.ToString.ToLower
                Case Is = _ucr1.Content("EMAIL_ORDER_CONFIRMATION", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_ORDER_CONFIRMATION
                Case Is = _ucr1.Content("EMAIL_TICKETING_UPGRADE", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_TICKETING_UPGRADE
                Case Is = _ucr1.Content("EMAIL_TICKETING_TRANSFER", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_TICKETING_TRANSFER
                Case Is = _ucr1.Content("EMAIL_TICKETING_CANCEL", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_TICKETING_CANCEL
                Case Is = _ucr1.Content("EMAIL_FORGOTTEN_PASSWORD", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_FORGOTTEN_PASSWORD
                Case Is = _ucr1.Content("EMAIL_CHANGE_PASSWORD", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_CHANGE_PASSWORD
                Case Is = _ucr1.Content("EMAIL_CUSTOMER_REGISTRATION", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_CUSTOMER_REGISTRATION
                Case Is = _ucr1.Content("EMAIL_ORDER_RETURN_CONFIRM", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM
                Case Is = _ucr1.Content("EMAIL_ORDER_RETURN_CONFIRM_REBOOK", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM_REBOOK
                Case Is = _ucr1.Content("EMAIL_PPS_PAYMENT_CONFIRMATION", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_PPS_PAYMENT_CONFIRMATION
                Case Is = _ucr1.Content("EMAIL_PPS_PAYMENT_FAILURE", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_PPS_PAYMENT_FAILURE
                Case Is = _ucr1.Content("EMAIL_PPS_AMEND", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_PPS_AMEND
                Case Is = _ucr1.Content("EMAIL_PPS_AMEND_PAYMENT", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_PPS_AMEND_PAYMENT
                Case Is = _ucr1.Content("EMAIL_RETAIL_ORDER_CONFIRMATION", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_RETAIL_ORDER_CONFIRMATION
                Case Is = _ucr1.Content("EMAIL_CONTACT_US", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_CONTACT_US
                Case Is = _ucr1.Content("EMAIL_TICKET_EXCHANGE_CONFIRM", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_TICKET_EXCHANGE_CONFIRM
                Case Is = _ucr1.Content("EMAIL_TICKET_EXCHANGE_SALE_CONFIRM", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_TICKET_EXCHANGE_SALE_CONFIRM
                Case Is = _ucr1.Content("EMAIL_HOSPITALITY_Q_AND_A_REMINDER", _languageCode, True).ToLower
                    sOutput = GlobalConstants.EMAIL_HOSPITALITY_Q_AND_A_REMINDER
            End Select
        End If

        Return sOutput

    End Function

    Public Shared Function ConvertStringToDecimal(ByVal value As String) As Decimal
        Dim retval As Decimal
        Dim res As Boolean = Decimal.TryParse(value, retval)

        If res Then
            Return retval
        Else
            Return 0D
        End If


    End Function

End Class
