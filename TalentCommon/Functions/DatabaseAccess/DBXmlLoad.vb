Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports System.text
Imports System.Collections.Generic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to import generic xml data
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBXML- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'-------------------------------------------------------------------------------------------------
'   Process
'   -------
'   1.	Check logon, password being used, this is the same as all the web services.
'   2.	Load the new table [tbl_xml_field_xref] in to the program.
'   3.	Put the XML data to a data entity [DEXmlLoad].
'   4.	Validate the XML data using the [tbl_xml_field_xref] constraints; also populate the extra fields 
'       with the data entity [DEXmlLoad].
'   5.	Accept or reject the XML document.
'   6.	If the xml document is rejected, create a response xml document and pass back to the user.
'   7.	If the xml document is accepted, sequentially pass through the data entity creating dynamic sequel 
'       insert statements and post each to the front end database. 
'       a.	The email address given is used as the Login identifier for all user table
'       b.	If a record is posted to the user table the process automatically creates an [authorized_users 
'           record] with a password of password.
'       c.	The [LAST_LOGIN_DATE] field on the [authorized_users record] is set to [191111] 
'           (that is 11th November 1911). This is so that the new users can be extracted and posted to the 
'           back end database.
'   8.	Extract all new users and post one by one to the backend database using the process from the 
'       registration form [TalentCustomer.SetCustomer] 
'   9.	Set the [LAST_LOGIN_DATE] to today�s date.
'   10.	Create a response xml document and pass back to the user, only include data lines that have 
'       Active fields, as per the [ACTIVE] flag on the [tbl_xml_field_xref] table.
'-------------------------------------------------------------------------------------------------
'   General XML Details     These are ID's 1-99
'   Customer Details        These are ID's 100-199
'   Contact Details         These are ID's 200-299
'   Attributes              These are ID's 500-599.
'                               Attributes are matched against TALENT by name and associated 
'                               against the Client/Contact as defined in T#AT
'                               Attributes can only be Added or Deleted.
'   Actions                 These are ID's 600-699
'                               Actions can only be ADDED
'   Notes                   Note are not currently supported
'                               Notes can only be ADDED
'   Brochure Requests       These are catered for by using the Contact Caption fields in 
'                               the Contact Section
'
'-------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBXmlLoad
    Inherits DBAccess
    '------------------------------------------------
    ' Nested class for returning and storing in cache
    ' 
    Public Class XmlField

        Private _xmlId As String = String.Empty
        Private _Active As Boolean = False
        Private _xmlType As String = String.Empty
        Private _sqlTable As String = String.Empty
        Private _sqlField As String = String.Empty
        Private _sqlDescription As String = String.Empty
        Private _sqlLength As String = String.Empty
        Private _sqlType As String = String.Empty
        Private _sqlComments As String = String.Empty
        Private _sqlMandatory As String = String.Empty
        Private _sqlDefault As String = String.Empty

        Public Property XmlId() As String
            Get
                Return _xmlId
            End Get
            Set(ByVal value As String)
                _xmlId = value
            End Set
        End Property

        Public Property Active() As Boolean
            Get
                Return _Active
            End Get
            Set(ByVal value As Boolean)
                _Active = value
            End Set
        End Property
        Public Property SqlTable() As String
            Get
                Return _sqlTable
            End Get
            Set(ByVal value As String)
                _sqlTable = value
            End Set
        End Property
        Public Property SqlField() As String
            Get
                Return _sqlField
            End Get
            Set(ByVal value As String)
                _sqlField = value
            End Set
        End Property
        Public Property SqlDescription() As String
            Get
                Return _sqlDescription
            End Get
            Set(ByVal value As String)
                _sqlDescription = value
            End Set
        End Property
        Public Property Sqllength() As String
            Get
                Return _sqlLength
            End Get
            Set(ByVal value As String)
                _sqlLength = value
            End Set
        End Property
        Public Property SqlType() As String
            Get
                Return _sqlType
            End Get
            Set(ByVal value As String)
                _sqlType = value
            End Set
        End Property
        Public Property SqlComments() As String
            Get
                Return _sqlComments
            End Get
            Set(ByVal value As String)
                _sqlComments = value
            End Set
        End Property
        Public Property SqlMandatory() As String
            Get
                Return _sqlMandatory
            End Get
            Set(ByVal value As String)
                _sqlMandatory = value
            End Set
        End Property
        Public Property SqlDefault() As String
            Get
                Return _sqlDefault
            End Get
            Set(ByVal value As String)
                _sqlDefault = value
            End Set
        End Property
    End Class

    Private _businessUnit As String = String.Empty
    Private _dtHead As New DataTable
    Private _dtDetails As New DataTable
    Private _eCrminputId As String = String.Empty
    Private _fieldXref As New Collection
    Private _partnerCode As String = String.Empty
    Private _sqlFields As StringBuilder = Nothing
    Private _sqlString As String = String.Empty
    Private _sqlTable As String = String.Empty
    Private _sqlValues As StringBuilder = Nothing
    Private _xml As Collection
    Private _xmlId As String = String.Empty
    Private _xmlLoad As New DEXmlLoad
    Private _newUserNumberB2B As String = String.Empty
    Private _tDataObjects As New TalentDataObjects

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property dtHead() As DataTable
        Get
            Return _dtHead
        End Get
        Set(ByVal value As DataTable)
            _dtHead = value
        End Set
    End Property
    Public Property dtDetails() As DataTable
        Get
            Return _dtDetails
        End Get
        Set(ByVal value As DataTable)
            _dtDetails = value
        End Set
    End Property
    Public Property PartnerCode() As String
        Get
            Return _partnerCode
        End Get
        Set(ByVal value As String)
            _partnerCode = value
        End Set
    End Property

    Public Property Xml() As Collection
        Get
            Return _xml
        End Get
        Set(ByVal value As Collection)
            _xml = value
        End Set
    End Property
    Public Function Xml_Find(ByVal Value As String, ByVal XmlId As String) As String
        Dim XmlLoad As New DEXmlLoad
        Dim Result As String = String.Empty
        For Each XmlLoad In Xml
            If XmlLoad.EcrminputId = Value Then
                If XmlLoad.XmlId = XmlId Then
                    Result = XmlLoad.XmlValue
                    Exit For
                End If
            End If
        Next
        Return Result
    End Function
    Public Function Xml_Find_EcrminputId(ByVal Value As String) As String
        Dim XmlLoad As New DEXmlLoad
        Dim Result As String = String.Empty
        For Each XmlLoad In Xml
            If XmlLoad.XmlValue = Value Then
                Result = XmlLoad.EcrminputId
                Exit For
            End If
        Next
        Return Result
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        Dim dHead As DataRow = Nothing
        Dim dRow As DataRow = Nothing
        Dim detr As New DEXmlLoad
        Dim iCounter As Integer = 0
        Dim XmlField As New XmlField
        '----------------------------------------------------------------------------------------
        _sqlTable = String.Empty
        _xmlId = String.Empty
        _sqlString = String.Empty
        _eCrminputId = String.Empty
        _sqlFields = New StringBuilder
        _sqlValues = New StringBuilder
        '----------------------------------------------------------------------------------------
        Try
            ResultDataSet = New DataSet
            ResultDataSet.Tables.Add(dtHead)
            ResultDataSet.Tables.Add(dtDetails)
            AddColumnsToDataTables()
            '-----------------------------------------------------------------------------------
            '   We now have checked each xml input line and set the SQL field etc.
            '

            '1) Loop through all _xmlLoad objects in the XML object
            '   1.1) Create a list of all distinct tables in the _xmlLoad objects
            '   1.2) Create a list of all distinct ecrmInputIDs in the _xmlLoad objects
            Dim tables As New Generic.List(Of String)
            Dim ecrmInputIDs As New Generic.List(Of String)
            Dim tableExists As Boolean = False
            Dim ecrmExists As Boolean = False

            For Each _xmlLoad In Xml
                tableExists = False
                ecrmExists = False

                'Check to see if the table has already been added
                For Each str As String In tables
                    If str = _xmlLoad.SqlTable Then
                        tableExists = True
                        Exit For
                    End If
                Next
                'If the table has not been added then add it now
                If Not tableExists Then
                    tables.Add(_xmlLoad.SqlTable)
                End If

                'Check to see if the ecrmInputID has already been added
                For Each str As String In ecrmInputIDs
                    If str = _xmlLoad.EcrminputId Then
                        ecrmExists = True
                        Exit For
                    End If
                Next
                'If the ecrmInputID has not been added then add it now
                If Not ecrmExists Then
                    ecrmInputIDs.Add(_xmlLoad.EcrminputId)
                End If

            Next


            For Each ecrmInputID As String In ecrmInputIDs
                '-------------------------------
                ' Reset new user id if generated
                '-------------------------------
                _newUserNumberB2B = String.Empty

                For Each table As String In tables
                    For Each _xmlLoad In Xml
                        If _xmlLoad.Active _
                                And table = _xmlLoad.SqlTable _
                                And table.Length > 0 _
                                And ecrmInputID = _xmlLoad.EcrminputId Then

                            If Not _xmlLoad.XmlId = "10301" Then

                                '----------------------------------------------------------------------------
                                '   Start new section in xml Response but only for xml lines that have been
                                '   reconised 
                                '
                                If _eCrminputId <> _xmlLoad.EcrminputId Then
                                    dHead = Nothing
                                    dHead = dtHead.NewRow()
                                    dHead("eCrmInputId") = _xmlLoad.EcrminputId
                                    dtHead.Rows.Add(dHead)
                                End If

                                '---------------------------------------------------------------------------
                                '   Set info that is passed back for the xmlResponse
                                '
                                dRow = Nothing
                                dRow = dtDetails.NewRow()
                                dRow("eCrmInputId") = _xmlLoad.EcrminputId
                                dRow("id") = _xmlLoad.XmlId
                                dRow("xml") = _xmlLoad.XmlValue
                                dRow("title") = _xmlLoad.XmlDescription
                                dtDetails.Rows.Add(dRow)
                                '---------------------------------------------------------------------------
                                '   Set  XmlLoad info to Sequel string ready to process  
                                '
                                '   ie.    INSERT INTO _sqlTable ( _sqlFields ) VALUES ( _sqlValues )
                                '
                                _eCrminputId = _xmlLoad.EcrminputId
                                _xmlId = _xmlLoad.XmlId
                                _sqlTable = _xmlLoad.SqlTable
                                _sqlFields.Append(_xmlLoad.SqlField & ",")
                                '
                                Select Case _xmlLoad.SqlType                ' format as per type of info
                                    Case Is = "CHAR"
                                        _sqlValues.Append(Utilities.Quote(_xmlLoad.XmlValue) & ",")
                                    Case Is = "DECI", "INTE", "CURR"
                                        _sqlValues.Append(_xmlLoad.XmlValue.ToString & ",")
                                    Case Is = "DATE"
                                        _sqlValues.Append(Utilities.Quote(_xmlLoad.XmlValue) & ",")
                                End Select

                            End If
                        End If
                    Next 'xmlLoad loop

                    If table.Length > 0 Then err = InsertRecord()
                    _sqlFields = New StringBuilder
                    _sqlValues = New StringBuilder
                Next 'tables loop
            Next 'ecrmInputIDs loop


            'For Each _xmlLoad In Xml
            '    If _xmlLoad.Active Then
            '        '----------------------------------------------------------------------------
            '        '   Start new section in xml Response but only for xml lines that have been
            '        '   reconised 
            '        '
            '        If _eCrminputId <> _xmlLoad.EcrminputId Then
            '            dHead = Nothing
            '            dHead = dtHead.NewRow()
            '            dHead("eCrmInputId") = _xmlLoad.EcrminputId
            '            dtHead.Rows.Add(dHead)
            '        End If
            '        '---------------------------------------------------------------------------
            '        '   table has changed so put to Sequel what we have so far
            '        '
            '        If _xmlLoad.SqlTable <> _sqlTable Then
            '            If _sqlTable.Length > 0 Then err = InsertRecord()
            '            _sqlFields = New StringBuilder
            '            _sqlValues = New StringBuilder
            '        End If
            '        '---------------------------------------------------------------------------
            '        '   Set info that is passed back for the xmlResponse
            '        '
            '        dRow = Nothing
            '        dRow = dtDetails.NewRow()
            '        dRow("eCrmInputId") = _xmlLoad.EcrminputId
            '        dRow("id") = _xmlLoad.XmlId
            '        dRow("xml") = _xmlLoad.XmlValue
            '        dRow("title") = _xmlLoad.XmlDescription
            '        dtDetails.Rows.Add(dRow)
            '        '---------------------------------------------------------------------------
            '        '   Set  XmlLoad info to Sequel string ready to process  
            '        '
            '        '   ie.    INSERT INTO _sqlTable ( _sqlFields ) VALUES ( _sqlValues )
            '        '
            '        _eCrminputId = _xmlLoad.EcrminputId
            '        _xmlId = _xmlLoad.XmlId
            '        _sqlTable = _xmlLoad.SqlTable
            '        _sqlFields.Append(_xmlLoad.SqlField & ",")
            '        '
            '        Select Case _xmlLoad.SqlType                ' format as per type of info
            '            Case Is = "CHAR"
            '                _sqlValues.Append(Utilities.Quote(_xmlLoad.XmlValue) & ",")
            '            Case Is = "DECI", "INTE", "CURR"
            '                _sqlValues.Append(_xmlLoad.XmlValue.ToString & ",")
            '            Case Is = "DATE"
            '                _sqlValues.Append(Utilities.Quote(_xmlLoad.XmlValue) & ",")
            '        End Select
            '    End If
            'Next
            '-----------------------------------------------------------------------------------
            '   End of data so put to Sequel last record, update to CRM and reset date field
            '
            'If _sqlTable.Length > 0 Then _
            '    err = InsertRecord()
            ''
            If Not err.HasError Then
                ' BF - no longer needed. Wait until accounts are activated to create the link.
                ' Customer Manager will no longer create duplicates if SL account exists
                ' err = SendDetailsToCRM()
                If Not err.HasError Then _
                    err = Update_Authorized_Users()
            End If
            '
        Catch ex As Exception
            Const strError As String = "Error during database load "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBXML-06"
                .HasError = True
            End With
        End Try
        '---------------------------------------------------------------------------
        Return err
    End Function
    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------------------
        '   Check input xml data for validity
        '
        Dim detr As New DEXmlLoad
        Dim XmlLoad As New DEXmlLoad
        Dim XmlField As New XmlField
        Dim iCounter As Integer = 0
        Dim eCrmInputId As String = String.Empty
        Dim Mandatory001 As String = String.Empty                                   ' One of 001, 002, 008 or 010 mandatory for CRM Import
        Dim Mandatory202 As Boolean = False                                         ' Mandatory if 8 supplied
        Dim Mandatory302 As Boolean = False                                         ' Mandatory if 8 supplied
        Dim EmailAddress As Boolean = False                                         ' Mandatory used as login id

        Try
            err = Check_Xml_Field_XRef()
            If Not err.HasError Then
                '---------------------------------------------------------------------------------------------
                For Each XmlLoad In Xml
                    If _fieldXref.Contains(XmlLoad.XmlId) Then
                        XmlField = _fieldXref(XmlLoad.XmlId)
                        '-------------------------------------------------------------------------------------
                        If eCrmInputId > String.Empty And eCrmInputId <> XmlLoad.EcrminputId Then
                            '---------------------------------------------------------------------------------
                            If Mandatory001.Equals(String.Empty) Then
                                iCounter += 1
                                With err
                                    .ItemErrorMessage(iCounter) = String.Empty
                                    .ItemErrorCode(iCounter) = "TACDBOR-40"
                                    .ItemErrorStatus(iCounter) = eCrmInputId & " One of 001, 002, 008 or 010 mandatory "
                                    .HasError = True
                                End With
                            End If
                            '---------------------------------------------------------------------------------
                            If Not EmailAddress Then
                                iCounter += 1
                                With err
                                    .ItemErrorMessage(iCounter) = String.Empty
                                    .ItemErrorCode(iCounter) = "TACDBOR-41"
                                    .ItemErrorStatus(iCounter) = eCrmInputId & " Email Address mandatory "
                                    .HasError = True
                                End With
                            End If
                            '---------------------------------------------------------------------------------
                            If Mandatory001.Equals("008") And Not Mandatory202 Then
                                iCounter += 1
                                With err
                                    .ItemErrorMessage(iCounter) = String.Empty
                                    .ItemErrorCode(iCounter) = "TACDBOR-42"
                                    .ItemErrorStatus(iCounter) = eCrmInputId & " 202 Contact Surname Mandatory if 008 supplied "
                                    .HasError = True
                                End With
                            End If
                            '---------------------------------------------------------------------------------
                            If Mandatory001.Equals("008") And Not Mandatory302 Then
                                iCounter += 1
                                With err
                                    .ItemErrorMessage(iCounter) = String.Empty
                                    .ItemErrorCode(iCounter) = "TACDBOR-43"
                                    .ItemErrorStatus(iCounter) = eCrmInputId & " 302 Contact Surname Mandatory if 008 supplied "
                                    .HasError = True
                                End With
                            End If
                            '---------------------------------------------------------------------------------
                            Mandatory001 = String.Empty
                            Mandatory202 = False
                            Mandatory302 = False
                            EmailAddress = False
                            '---------------------------------------------------------------------------------
                        End If
                        eCrmInputId = XmlLoad.EcrminputId
                        '-------------------------------------------------------------------------------------
                        Select Case XmlLoad.XmlId
                            Case "001", "002", "008", "010"                         ' One of 001, 002, 008 or 010 mandatory for CRM Import
                                Mandatory001 = XmlLoad.XmlId

                            Case "004"                                              ' If not set, a default will be used

                                ''XmlLoad.XmlValue = ConfigurationManager.AppSettings("DefaultBusinessUnit")

                            Case "202"                                              ' Contact Surname Mandatory if 008 supplied
                                Mandatory202 = True

                            Case "227"                                              ' Email Address
                                EmailAddress = True

                            Case "302"                                              ' Contact Surname Mandatory if 008 supplied
                                Mandatory302 = True

                            Case "305", "307", "309", "311", "313", "326", "328"    ' Y/N/blank allowed - default applied if not sent	
                                If XmlLoad.XmlValue.Trim > "" Then
                                    If InStr("Y~N", XmlLoad.XmlValue.Trim) = 0 Then
                                        iCounter += 1
                                        With err
                                            .ItemErrorMessage(iCounter) = String.Empty
                                            .ItemErrorCode(iCounter) = "TACDBOR-50"
                                            .ItemErrorStatus(iCounter) = eCrmInputId & " Only Y/N/blank allowed - " & _
                                                                XmlLoad.XmlId & " / " & _
                                                                XmlLoad.XmlValue
                                            .HasError = True
                                        End With
                                    End If
                                    XmlLoad.XmlValue = "N"
                                End If

                        End Select
                        '-------------------------------------------------------------------------------------
                        '
                        If XmlField.Active Then
                            '
                            XmlLoad.Active = XmlField.Active
                            XmlLoad.SqlTable = XmlField.SqlTable
                            XmlLoad.SqlField = XmlField.SqlField
                            XmlLoad.SqlDescription = XmlField.SqlDescription
                            XmlLoad.Sqllength = XmlField.Sqllength
                            XmlLoad.SqlType = XmlField.SqlType
                            XmlLoad.SqlComments = XmlField.SqlComments
                            XmlLoad.SqlMandatory = XmlField.SqlMandatory
                            XmlLoad.SqlDefault = XmlField.SqlDefault
                            '
                            Select Case XmlLoad.SqlType
                                '-----------------------------------------------------------------------------
                                Case Is = "CHAR"
                                    If XmlLoad.XmlValue.Length > XmlField.Sqllength Then
                                        iCounter += 1
                                        With err
                                            .ItemErrorMessage(iCounter) = String.Empty
                                            .ItemErrorCode(iCounter) = "TACDBOR-51"
                                            .ItemErrorStatus(iCounter) = eCrmInputId & " Error data to long - " & _
                                                                XmlLoad.XmlId & " / " & _
                                                                XmlLoad.SqlTable & " / " & _
                                                                XmlLoad.SqlField & " / " & _
                                                                XmlLoad.XmlValue
                                            .HasError = True
                                        End With
                                    End If
                                    '-------------------------------------------------------------------------
                                Case Is = "DECI", "INTE", "CURR"
                                    Try
                                        Dim xxxxxx As Double = CType(XmlLoad.XmlValue, Double)
                                    Catch ex As Exception
                                        With err
                                            iCounter += 1
                                            .ItemErrorMessage(iCounter) = String.Empty
                                            .ItemErrorCode(iCounter) = "TACDBOR-52"
                                            .ItemErrorStatus(iCounter) = eCrmInputId & " Error data is not numerical - " & _
                                                                XmlLoad.XmlId & " / " & _
                                                                XmlLoad.SqlTable & " / " & _
                                                                XmlLoad.SqlField & " / " & _
                                                                XmlLoad.XmlValue
                                            .HasError = True
                                        End With
                                    End Try
                                    '-------------------------------------------------------------------------
                                    '   Export always in YYYY/MM/DD format (ie: 20060821)
                                    '
                                Case Is = "DATE"
                                    Dim di As String = Left(XmlLoad.XmlValue, 4) & "-" & XmlLoad.XmlValue.Substring(4, 2) & "-" & XmlLoad.XmlValue.Substring(6)
                                    If Not IsDate(di) Then
                                        iCounter += 1
                                        With err
                                            .ItemErrorMessage(iCounter) = String.Empty
                                            .ItemErrorCode(iCounter) = "TACDBOR-53"
                                            .ItemErrorStatus(iCounter) = eCrmInputId & " Invalid Date - " & _
                                                                XmlLoad.XmlId & " / " & _
                                                                XmlLoad.SqlTable & " / " & _
                                                                XmlLoad.SqlField & " / " & _
                                                                XmlLoad.XmlValue
                                            .HasError = True
                                        End With
                                    End If
                            End Select
                        End If
                    Else
                        Const strError2 As String = "Error Checking item - "
                        With err
                            .ErrorMessage = XmlLoad.XmlValue
                            .ErrorNumber = "TACDBXML-60"
                            .ErrorStatus = strError2 & eCrmInputId & "/" & XmlLoad.XmlId
                            .HasError = True
                        End With
                    End If
                Next
            End If
            '-------------------------------------------------------------------------------------------------
        Catch ex As Exception
            Const strError3 As String = "Error during database validation access "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError3 & " - " & eCrmInputId
                .ErrorNumber = "TACDBXML-07"
                .HasError = True
            End With
        End Try
        '---------------------------------------------------------------------------
        Return err
    End Function
    Private Sub AddColumnsToDataTables()
        '--------------------------------------------------------------------------
        With dtHead.Columns
            .Add("eCrmInputId", GetType(String))
        End With
        '--------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("eCrmInputId", GetType(String))
            .Add("xml", GetType(String))
            .Add("id", GetType(String))
            .Add("title", GetType(String))
        End With
        '--------------------------------------------------------------------------
    End Sub
    Private Function InsertRecord() As ErrorObj
        Dim err As New ErrorObj
        '-------------------------------------------------------------------------------------------
        Dim emailAddress As String = String.Empty
        Dim AccountNo1 As String = String.Empty
        Dim customerAccountNo1 As String = String.Empty
        Dim customerAccountNo2 As String = String.Empty
        Dim crmBranch As String = String.Empty
        Dim sqlString As String = String.Empty
        Dim cmdSelect As SqlCommand = Nothing
        Dim b2bMode As Boolean = False
        '
        Try
            If _sqlTable.Length > 0 Then
                _tDataObjects.Settings = Settings
                Select Case _sqlTable
                    '-------------------------------------------------------------------------------
                    Case Is = "tbl_address"
                        emailAddress = Xml_Find(_eCrminputId, "227")
                        Dim _ptnr As String = Xml_Find(_eCrminputId, "10302")
                        Dim _bu As String = Xml_Find(_eCrminputId, "10301")
                        If String.IsNullOrEmpty(_ptnr) Then
                            _ptnr = BusinessUnit
                        End If

                        b2bMode = _tDataObjects.AppVariableSettings.TblAuthorizedPartners.CheckB2BMode(_bu)
                        Dim alreadyExists As Boolean = False
                        '---------------------------------------------------------------
                        ' If B2B mode then check if partner user record exists for email
                        ' and partner. If so assume address exists
                        '---------------------------------------------------------------
                        If b2bMode Then
                            alreadyExists = CheckPartnerUserB2B(_bu, _ptnr, emailAddress)
                            If Not alreadyExists Then
                                ' Generate new user number if haven't already got one
                                If _newUserNumberB2B = String.Empty Then
                                    _newUserNumberB2B = Utilities.GetNextUserNumber(_bu, _ptnr, Settings.FrontEndConnectionString).ToString
                                End If
                                sqlString = "INSERT INTO tbl_address ( PARTNER, LOGINID, " & _
                                           _sqlFields.ToString & ") VALUES ('" & _ptnr & "','" & _
                                           _newUserNumberB2B & "'," & _sqlValues.ToString & ")"
                                sqlString = sqlString.Replace(",)", ")")
                                cmdSelect = New SqlCommand(sqlString, conSql2005)
                                cmdSelect.ExecuteNonQuery()
                            End If

                        Else
                            '--------
                            'B2C mode
                            '--------
                            alreadyExists = Check_User_Has_Address(_ptnr, emailAddress)
                            'If Not Check_User_Has_Address(_ptnr, emailAddress) Then
                            If Not alreadyExists Then
                                sqlString = "INSERT INTO tbl_address ( PARTNER, LOGINID, " & _
                                            _sqlFields.ToString & ") VALUES ('" & _ptnr & "','" & _
                                            emailAddress & "'," & _sqlValues.ToString & ")"
                                sqlString = sqlString.Replace(",)", ")")
                                cmdSelect = New SqlCommand(sqlString, conSql2005)
                                cmdSelect.ExecuteNonQuery()
                            End If

                        End If
                        '---------------------------------------------------------------------------
                    Case Is = "tbl_partner"
                        Dim partnerNumer As Long = Utilities.GetNextPartnerNumber(BusinessUnit, PartnerCode, Settings.FrontEndConnectionString)

                        ''IF partnerNumer = -1 THEN COCK UP

                        sqlString = "INSERT INTO tbl_partner ( PARTNER_NUMBER, " & _
                                       _sqlFields.ToString & ") VALUES (" & partnerNumer & "," & _
                                       _sqlValues.ToString & ")"
                        '
                        sqlString = sqlString.Replace(",)", ")")
                        cmdSelect = New SqlCommand(sqlString, conSql2005)
                        cmdSelect.ExecuteNonQuery()
                        '---------------------------------------------------------------------------
                    Case Is = "tbl_partner_user"
                        emailAddress = Xml_Find(_eCrminputId, "227")
                        AccountNo1 = Xml_Find(_eCrminputId, "232")
                        customerAccountNo1 = Xml_Find(_eCrminputId, "118")
                        customerAccountNo2 = Xml_Find(_eCrminputId, "119")

                        crmBranch = Xml_Find(_eCrminputId, "004")

                        Dim _bu As String = Xml_Find(_eCrminputId, "10301")
                        Dim _ptnr As String = Xml_Find(_eCrminputId, "10302")

                        If String.IsNullOrEmpty(_bu) Then
                            _bu = BusinessUnit
                        End If
                        If String.IsNullOrEmpty(_ptnr) Then
                            _ptnr = BusinessUnit
                        End If

                        '---------------------------------------------
                        ' If partner doesn't exist then need to create 
                        '---------------------------------------------
                        If Not CheckPartnerExists(_ptnr) Then
                            err = CreatePartner(_ptnr, _bu, customerAccountNo1, customerAccountNo2, crmBranch)
                        End If

                        b2bMode = _tDataObjects.AppVariableSettings.TblAuthorizedPartners.CheckB2BMode(_bu)
                        '----------------------------------------------------------------------------
                        ' For B2B mode, check tbl_partner_user by EMAIL and ACCOUNT to see if already
                        ' exists
                        '----------------------------------------------------------------------------
                        If b2bMode Then
                            If Not CheckPartnerUserB2B(_bu, _ptnr, emailAddress) Then
                                CreatePartnerUserB2B(_bu, _ptnr, emailAddress)
                            End If
                        Else
                            '---------
                            ' B2C Mode
                            '---------
                            If Not Check_Authorized_Users(_bu, _ptnr, emailAddress) Then
                                Dim userNumer As Long = Utilities.GetNextUserNumber(_bu, _ptnr, Settings.FrontEndConnectionString)

                                ''IF userNumer = -1 THEN COCK UP

                                sqlString = "INSERT INTO tbl_partner_user (LOGINID, USER_NUMBER, " & _
                                                 _sqlFields.ToString & ") VALUES ('" & _
                                                 emailAddress & "'," & userNumer & "," & _sqlValues.ToString & ")"
                                sqlString = sqlString.Replace(",)", ")")
                                cmdSelect = New SqlCommand(sqlString, conSql2005)
                                cmdSelect.ExecuteNonQuery()
                                '-----------------------------------------------------------------------
                                '   We are inserting a new user so create authorized_users record
                                '
                                '   LAST_LOGIN_DATE used by SendDetailsToCRMn
                                '
                                sqlString = "INSERT INTO tbl_authorized_users ( BUSINESS_UNIT, PARTNER, LOGINID, PASSWORD, " & _
                                                                                " AUTO_PROCESS_DEFAULT_USER, IS_APPROVED,  " & _
                                                                                " IS_LOCKED_OUT, " & _
                                                                                " CREATED_DATE, LAST_LOGIN_DATE,  " & _
                                                                                " LAST_PASSWORD_CHANGED_DATE, LAST_LOCKED_OUT_DATE ) " & _
                                                                                "VALUES (" & _
                                                                                " @Param1, @Param2, @Param3, @Password, @Auto, " & _
                                                                                " @Approved, @Locked, @Param4, '19111111', " & _
                                                                                " @PwChanged, @LastLocked)"
                                cmdSelect = New SqlCommand(sqlString, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = _bu
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = _ptnr
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = emailAddress
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.DateTime)).Value = Now
                                    .Parameters.Add(New SqlParameter("@Password", SqlDbType.Char)).Value = AccountNo1
                                    .Parameters.Add(New SqlParameter("@Auto", SqlDbType.Bit)).Value = False
                                    .Parameters.Add(New SqlParameter("@Approved", SqlDbType.Bit)).Value = True
                                    .Parameters.Add(New SqlParameter("@Locked", SqlDbType.Bit)).Value = False
                                    .Parameters.Add(New SqlParameter("@PwChanged", SqlDbType.DateTime)).Value = Now
                                    .Parameters.Add(New SqlParameter("@LastLocked", SqlDbType.DateTime)).Value = Now
                                End With
                                cmdSelect.ExecuteNonQuery()
                            End If
                        End If
                        '---------------------------------------------------------------------------
                    Case Else
                        sqlString = "INSERT INTO " & _sqlTable & "(" & _sqlFields.ToString & ") VALUES (" & _
                                                                       _sqlValues.ToString & ")"
                        sqlString = sqlString.Replace(",)", ")")
                        cmdSelect = New SqlCommand(sqlString, conSql2005)
                        cmdSelect.ExecuteNonQuery()
                        '---------------------------------------------------------------------------
                End Select
            End If
            '---------------------------------------------------------------------------------------
        Catch ex As Exception
            Const strError As String = "Error during database insert "
            With err
                .ErrorMessage = sqlString
                .ErrorStatus = strError & " - " & _sqlTable
                .ErrorNumber = "TACDBXML-13"
                .HasError = True
            End With
        End Try
        '------------------------------------------------------------------------------------------
        Return err
    End Function
    Private Function SendDetailsToCRM() As ErrorObj
        Dim Err As New ErrorObj
        '---------------------------------------------------------------------------------------
        '   Set the Customer object and invoke the SetCustomer method to post details to TALENT 
        '   Customer(Manager)
        '
        Dim TalentCustomer As New TalentCustomer
        Dim deCustV1 As New DECustomer
        Dim deCustv11 As New DECustomerV11
        TalentCustomer.DeV11 = deCustv11
        deCustv11.DECustomersV1.Add(deCustV1)

        Dim cmdSelect As SqlCommand = Nothing
        Dim dtr As SqlDataReader = Nothing

        Dim b2bMode As Boolean = False
        Dim _bu As String = String.Empty
        '-----------------------------------------------------------------------------------
        Const SQLSelect As String = "SELECT  bb.*, cc.*                       " & _
                                     " FROM tbl_authorized_users aa WITH (NOLOCK)          " & _
                                     " INNER JOIN  tbl_address bb WITH (NOLOCK)            " & _
                                     "      ON aa.LOGINID = bb.LOGINID      " & _
                                     " INNER JOIN tbl_partner_user cc WITH (NOLOCK)        " & _
                                     "      ON aa.LOGINID = cc.LOGINID      " & _
                                     " WHERE  aa.LAST_LOGIN_DATE =          " & _
                                     "          CONVERT(DATETIME, '1911-11-11', 102)   "
        Try
            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
            dtr = cmdSelect.ExecuteReader()
            cmdSelect = Nothing
            '-------------------------------------------------------------------------------
            Dim ucr As New UserControlResource
            Dim LCID As Integer = Utilities.GetLCID
            With TalentCustomer.Settings
                .Company = "EBUSINESS" '' ???????????ucr.Content("crmExternalKeyName", LCID, True)    ' RegistrationForm.ascx
                .Cacheing = False
                .DestinationDatabase = "TALENTCRM" ' ????????ucr.Content("crmDestinationDatabase", LCID, True)  'ContactUsForm.ascx
                .BackOfficeConnectionString = Settings.BackOfficeConnectionString
                .StoredProcedureGroup = Settings.StoredProcedureGroup
            End With

            '-----------------
            ' Check if in B2B mode
            '-------------------------------------------------------------------------------
            _bu = Xml_Find(_eCrminputId, "10301")
            _tDataObjects.Settings = Settings
            b2bMode = _tDataObjects.AppVariableSettings.TblAuthorizedPartners.CheckB2BMode(_bu)
            While dtr.Read
                With deCustV1
                    .Action = ""
                    '-----------------------------------------------------------------------
                    _eCrminputId = Xml_Find_EcrminputId(dtr("LOGINID"))

                    .CustomerID = Xml_Find(_eCrminputId, "001")
                    .ContactID = Xml_Find(_eCrminputId, "002")
                    ' .ThirdPartyContactRef = Xml_Find(_eCrminputId, "008")
                    .ThirdPartyContactRef = dtr("USER_NUMBER")
                    .ThirdPartyCompanyRef1 = dtr("USER_NUMBER")
                    .ThirdPartyCompanyRef2 = String.Empty
                    .DateFormat = "1"

                    .ContactSurname = dtr("Surname").ToString
                    .ContactForename = dtr("Forename").ToString
                    .ContactTitle = dtr("Title").ToString
                    .ContactInitials = dtr("Initials").ToString
                    If Not (Utilities.CheckIsDBNull(dtr("DOB"))) Then
                        .DateBirth = dtr("DOB")
                    Else
                        .DateBirth = String.Empty
                    End If
                    .EmailAddress = dtr("Email").ToString
                    .SLNumber1 = dtr("Account_No_1").ToString
                    .SLNumber2 = dtr("Account_No_2").ToString

                    .ProcessContactSurname = "1"
                    .ProcessContactForename = "1"
                    .ProcessContactTitle = "1"
                    .ProcessContactInitials = "1"
                    .ProcessDateBirth = "1"
                    .ProcessEmailAddress = "1"
                    .ProcessSLNumber1 = "1"
                    .ProcessSLNumber2 = "1"
                    If b2bMode Then
                        .ProcessCompanyName = "1"
                        .CompanyName = dtr("Address_Line_1").ToString
                        .AddressLine1 = dtr("Address_Line_2").ToString
                        .AddressLine2 = dtr("Address_Line_3").ToString
                        .AddressLine3 = dtr("Address_Line_4").ToString
                        .AddressLine4 = dtr("Address_Line_5").ToString
                        .AddressLine5 = dtr("Country").ToString
                    Else
                        .AddressLine1 = dtr("Address_Line_1").ToString & " " & dtr("Address_Line_2").ToString
                        .AddressLine2 = dtr("Address_Line_3").ToString
                        .AddressLine3 = dtr("Address_Line_4").ToString
                        .AddressLine4 = dtr("Address_Line_5").ToString
                        .AddressLine5 = dtr("Country").ToString
                    End If

                    .PostCode = dtr("Post_Code").ToString
                    .HomeTelephoneNumber = dtr("Telephone_Number").ToString
                    .WorkTelephoneNumber = dtr("Work_Number").ToString
                    .MobileNumber = dtr("Mobile_Number").ToString

                    .ProcessAddressLine1 = "1"
                    .ProcessAddressLine2 = "1"
                    .ProcessAddressLine3 = "1"
                    .ProcessAddressLine4 = "1"
                    .ProcessAddressLine5 = "1"
                    .ProcessPostCode = "1"
                    .ProcessHomeTelephoneNumber = "1"
                    .ProcessWorkTelephoneNumber = "1"
                    .ProcessMobileNumber = "1"
                End With
                '--------------------------------------------------------------------
                '    
                Err = TalentCustomer.SetCustomer()
            End While
            dtr.Close()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError As String = "Error during Send Details To CRM "
            With Err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBXML-36"
                .HasError = True
            End With
        End Try
        Return Err
    End Function

    Private ReadOnly Property Check_Authorized_Users(ByVal _bu As String, ByVal _ptnr As String, Optional ByVal _emailAddress As String = "") As Boolean
        Get
            Dim err As New ErrorObj
            Dim cmdSelect As SqlCommand = Nothing
            Dim XmlField As New XmlField
            Dim bExists As Boolean = False
            Dim dtr As SqlDataReader = Nothing
            '--------------------------------------------------------------------------
            Try
                Const SQLSelect2 As String = "SELECT * FROM tbl_authorized_users WITH (NOLOCK)  " & _
                                                   " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                   " AND    PARTNER     	= @Param2   " & _
                                                   " AND    LOGINID         = @Param3   "
                cmdSelect = New SqlCommand(SQLSelect2, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = _bu
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = _ptnr
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = _emailAddress
                    dtr = .ExecuteReader()
                End With

                If dtr.HasRows Then bExists = True
                dtr.Close()
                cmdSelect = Nothing
                '----------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during Check Authorized Users"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBXML-15"
                    .HasError = True
                End With
            End Try
            '--------------------------------------------------------------------------
            Return bExists
        End Get
    End Property
    Private ReadOnly Property Check_User_Has_Address(ByVal _ptnr As String, Optional ByVal _emailAddress As String = "") As Boolean
        Get
            Dim err As New ErrorObj
            Dim cmdSelect As SqlCommand = Nothing
            Dim XmlField As New XmlField
            Dim bExists As Boolean = False
            Dim dtr As SqlDataReader = Nothing
            '--------------------------------------------------------------------------
            Try
                Const SQLSelect2 As String = "SELECT * FROM tbl_address WITH (NOLOCK)  " & _
                                                   " WHERE  PARTNER     	= @Partner   " & _
                                                   " AND    LOGINID         = @loginID   "
                cmdSelect = New SqlCommand(SQLSelect2, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter("@Partner", SqlDbType.Char)).Value = _ptnr
                    .Parameters.Add(New SqlParameter("@loginID", SqlDbType.Char)).Value = _emailAddress
                    dtr = .ExecuteReader()
                End With

                If dtr.HasRows Then bExists = True
                dtr.Close()
                cmdSelect = Nothing
                '----------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during Check Authorized Users"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBXML-15"
                    .HasError = True
                End With
            End Try
            '--------------------------------------------------------------------------
            Return bExists
        End Get
    End Property
    Private ReadOnly Property Check_Partner_User(ByVal LoginId As String) As Boolean
        Get
            Dim err As New ErrorObj
            Dim cmdSelect As SqlCommand = Nothing
            Dim XmlField As New XmlField
            Dim bExists As Boolean = False
            Dim dtr As SqlDataReader = Nothing
            '--------------------------------------------------------------------------
            Try
                Const SQLSelect As String = "SELECT * FROM tbl_partner_user WITH (NOLOCK)  " & _
                                                    " WHERE  PARTNER    = @Param1   " & _
                                                    " AND    LOGINID    = @Param2   "
                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = PartnerCode
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = LoginId
                    dtr = .ExecuteReader()
                End With
                If dtr.HasRows Then bExists = True
                dtr.Close()
                cmdSelect = Nothing
                '----------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during Check Partner User"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBXML-15"
                    .HasError = True
                End With
            End Try
            '--------------------------------------------------------------------------
            Return bExists
        End Get
    End Property
    Private ReadOnly Property Check_Xml_Field_XRef() As ErrorObj
        Get
            Dim err As New ErrorObj
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader = Nothing
            Dim XmlField As New XmlField
            '--------------------------------------------------------------------------
            Try
                Const SQLString As String = "SELECT * FROM tbl_xml_field_xref WITH (NOLOCK)  ORDER BY SQL_TABLE, XML_ID"
                cmdSelect = New SqlCommand(SQLString, conSql2005)
                dtr = cmdSelect.ExecuteReader()
                If dtr.HasRows Then
                    While dtr.Read
                        XmlField = New XmlField
                        With XmlField
                            .Active = dtr("ACTIVE").ToString
                            .XmlId = dtr("XML_ID").ToString
                            .SqlTable = dtr("SQL_TABLE").ToString
                            .SqlField = dtr("SQL_FIELD").ToString
                            .SqlType = dtr("SQL_TYPE").ToString
                            .Sqllength = dtr("SQL_LENGTH").ToString
                            _fieldXref.Add(XmlField, .XmlId)
                        End With
                    End While
                    dtr.Close()
                End If
                '----------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during Check Xml Field XRef"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBXML-12"
                    .HasError = True
                End With
            End Try
            '--------------------------------------------------------------------------
            Return err
        End Get
    End Property
    Private Function Update_Authorized_Users() As ErrorObj
        Dim err As New ErrorObj
        '-----------------------------------------------------------------------
        '   Reset the SendDetailsto CRM flag
        '
        Dim cmdSelect As SqlCommand = Nothing
        '--------------------------------------------------------------------------
        Try
            Const sqlString = " UPDATE tbl_authorized_users                 " & _
                                     " SET LAST_LOGIN_DATE = @Param1        " & _
                                     " WHERE LAST_LOGIN_DATE = '19111111'   "
            cmdSelect = New SqlCommand(sqlString, conSql2005)
            With cmdSelect
                .Parameters.Add(New SqlParameter(Param1, SqlDbType.DateTime)).Value = Now
            End With
            cmdSelect.ExecuteNonQuery()

        Catch ex As Exception
            Const strError As String = "Error during Update Authorized Users"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBXML-37"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CheckPartnerExists(ByVal partnerCode As String) As Boolean
        Dim cmdSelect As SqlCommand = Nothing
        Dim partnerExists As Boolean = False
        Dim dtr As SqlDataReader = Nothing
        Dim err As New ErrorObj
        Try
            Const SQLSelect As String = "SELECT * FROM tbl_partner WITH (NOLOCK)  " & _
                                                      " WHERE  PARTNER    = @Param1   "
            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
            With cmdSelect
                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = partnerCode
                dtr = .ExecuteReader()
            End With
            If dtr.HasRows Then
                partnerExists = True
            End If
            dtr.Close()
            cmdSelect = Nothing

        Catch ex As Exception
            Const strError As String = "Error during Update Authorized Users"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBXML-37"
                .HasError = True
            End With
        End Try
        Return partnerExists
    End Function

    Private Function CreatePartner(ByVal parmPartner As String, ByVal parmBU As String, ByVal parmAcc1 As String, ByVal parmAcc2 As String, ByVal parmCrmBranch As String) As ErrorObj
        Dim cmdInsert As SqlCommand = Nothing
        Dim err As New ErrorObj
        Try
            Const SQLInsert As String = "INSERT INTO tbl_partner ( PARTNER, PARTNER_DESC, DESTINATION_DATABASE, CACHEING_ENABLED, " & _
                                        " CACHE_TIME_MINUTES,LOGGING_ENABLED,STORE_XML,ACCOUNT_NO_1,ACCOUNT_NO_2,ACCOUNT_NO_3,ACCOUNT_NO_4, " & _
                                        " ACCOUNT_NO_5, EMAIL, TELEPHONE_NUMBER, FAX_NUMBER, PARTNER_URL, PARTNER_NUMBER, ORIGINATING_BUSINESS_UNIT, " & _
                                        " CRM_BRANCH, VAT_NUMBER) VALUES " & _
                                        "(@PARTNER, @PARTNER_DESC, @DESTINATION_DATABASE, @CACHEING_ENABLED, " & _
                                        " @CACHE_TIME_MINUTES,@LOGGING_ENABLED,@STORE_XML,@ACCOUNT_NO_1,@ACCOUNT_NO_2,@ACCOUNT_NO_3,@ACCOUNT_NO_4, " & _
                                        " @ACCOUNT_NO_5, @EMAIL, @TELEPHONE_NUMBER, @FAX_NUMBER, @PARTNER_URL, @PARTNER_NUMBER, @ORIGINATING_BUSINESS_UNIT, " & _
                                        " @CRM_BRANCH, @VAT_NUMBER)"

            Dim partnerNumber As Long = Utilities.GetNextPartnerNumber(parmBU, parmPartner, Settings.FrontEndConnectionString)
            cmdInsert = New SqlCommand(SQLInsert, conSql2005)
            cmdInsert.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char)).Value = parmPartner
            cmdInsert.Parameters.Add(New SqlParameter("@PARTNER_DESC", SqlDbType.Char)).Value = parmPartner
            cmdInsert.Parameters.Add(New SqlParameter("@DESTINATION_DATABASE", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@CACHEING_ENABLED", SqlDbType.Bit)).Value = False
            cmdInsert.Parameters.Add(New SqlParameter("@CACHE_TIME_MINUTES", SqlDbType.Int)).Value = 0
            cmdInsert.Parameters.Add(New SqlParameter("@LOGGING_ENABLED", SqlDbType.Bit)).Value = False
            cmdInsert.Parameters.Add(New SqlParameter("@STORE_XML", SqlDbType.Int)).Value = False
            cmdInsert.Parameters.Add(New SqlParameter("@ACCOUNT_NO_1", SqlDbType.Char)).Value = parmAcc1
            cmdInsert.Parameters.Add(New SqlParameter("@ACCOUNT_NO_2", SqlDbType.Char)).Value = parmAcc2
            cmdInsert.Parameters.Add(New SqlParameter("@ACCOUNT_NO_3", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@ACCOUNT_NO_4", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@ACCOUNT_NO_5", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@EMAIL", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@TELEPHONE_NUMBER", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@FAX_NUMBER", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@PARTNER_URL", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@PARTNER_NUMBER", SqlDbType.BigInt)).Value = partnerNumber
            cmdInsert.Parameters.Add(New SqlParameter("@ORIGINATING_BUSINESS_UNIT", SqlDbType.Char)).Value = String.Empty
            cmdInsert.Parameters.Add(New SqlParameter("@CRM_BRANCH", SqlDbType.Char)).Value = parmCrmBranch
            cmdInsert.Parameters.Add(New SqlParameter("@VAT_NUMBER", SqlDbType.Char)).Value = String.Empty
            cmdInsert.ExecuteNonQuery()

            cmdInsert = Nothing

        Catch ex As Exception
            Const strError As String = "Error during partner creation"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBXML-38"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function CheckPartnerUserB2B(ByVal _bu As String, ByVal _ptnr As String, ByVal _emailAddress As String) As Boolean
        Dim err As New ErrorObj
        Dim cmdSelect As SqlCommand = Nothing
        Dim XmlField As New XmlField
        Dim partnerUserExists As Boolean = False
        Dim dtr As SqlDataReader = Nothing
        '--------------------------------------------------------------------------
        Try
            Const SQLSelect2 As String = "SELECT * FROM tbl_partner_user WITH (NOLOCK)  " & _
                                               " WHERE    PARTNER = @Param1   " & _
                                               " AND    EMAIL = @Param2   "
            cmdSelect = New SqlCommand(SQLSelect2, conSql2005)
            With cmdSelect
                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = _ptnr
                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = _emailAddress
                dtr = .ExecuteReader()
            End With

            If dtr.HasRows Then
                partnerUserExists = True
            End If
            dtr.Close()
            cmdSelect = Nothing
            '----------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Check Parter User B2B"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBXML-39"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------------
        Return partnerUserExists
    End Function
    Private Function CreatePartnerUserB2B(ByVal _bu As String, ByVal _ptnr As String, ByVal _emailAddress As String) As Boolean
        If _newUserNumberB2B = String.Empty Then
            _newUserNumberB2B = Utilities.GetNextUserNumber(_bu, _ptnr, Settings.FrontEndConnectionString).ToString
        End If

        Dim sqlString As String = "INSERT INTO tbl_partner_user (LOGINID, USER_NUMBER, " & _
                         _sqlFields.ToString & ") VALUES ('" & _
                         _newUserNumberB2B & "'," & _newUserNumberB2B & "," & _sqlValues.ToString & ")"
        Dim cmdInsert As New SqlCommand
        sqlString = sqlString.Replace(",)", ")")
        cmdInsert = New SqlCommand(sqlString, conSql2005)
        cmdInsert.ExecuteNonQuery()
        '-----------------------------------------------------------------------
        '   We are inserting a new user so create authorized_users record
        '
        '   LAST_LOGIN_DATE used by SendDetailsToCRM
        '
        sqlString = "INSERT INTO tbl_authorized_users ( BUSINESS_UNIT, PARTNER, LOGINID, PASSWORD, " & _
                                                        " AUTO_PROCESS_DEFAULT_USER, IS_APPROVED,  " & _
                                                        " IS_LOCKED_OUT, " & _
                                                        " CREATED_DATE, LAST_LOGIN_DATE,  " & _
                                                        " LAST_PASSWORD_CHANGED_DATE, LAST_LOCKED_OUT_DATE ) " & _
                                                        "VALUES (" & _
                                                        " @Param1, @Param2, @Param3, @Password, @Auto, " & _
                                                        " @Approved, @Locked, @Param4, '19111111', " & _
                                                        " @PwChanged, @LastLocked)"
        cmdInsert = New SqlCommand(sqlString, conSql2005)
        With cmdInsert
            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = _bu
            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = _ptnr
            '.Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = _emailAddress
            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = _newUserNumberB2B
            .Parameters.Add(New SqlParameter(Param4, SqlDbType.DateTime)).Value = Now
            '.Parameters.Add(New SqlParameter("@Password", SqlDbType.Char)).Value = AccountNo1
            .Parameters.Add(New SqlParameter("@Password", SqlDbType.Char)).Value = String.Empty
            .Parameters.Add(New SqlParameter("@Auto", SqlDbType.Bit)).Value = False
            .Parameters.Add(New SqlParameter("@Approved", SqlDbType.Bit)).Value = True
            .Parameters.Add(New SqlParameter("@Locked", SqlDbType.Bit)).Value = False
            .Parameters.Add(New SqlParameter("@PwChanged", SqlDbType.DateTime)).Value = Now
            .Parameters.Add(New SqlParameter("@LastLocked", SqlDbType.DateTime)).Value = Now
        End With
        cmdInsert.ExecuteNonQuery()
    End Function

End Class
