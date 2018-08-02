Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports System.Text.RegularExpressions
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBSearch
    Inherits DBAccess

#Region "Class Level Fields"

    Private _deV11 As New DECustomerV11
    Private _customers As New List(Of DECustomerV11)
    Private _customerSearch As DECustomerSearch
    Private _companySearch As DECompanySearch

#End Region

#Region "Public Properties"
    Public Property CompanySearch() As DECompanySearch
        Get
            Return _companySearch
        End Get
        Set(ByVal value As DECompanySearch)
            _companySearch = value
        End Set
    End Property
    Public Property CustomerSearch() As DECustomerSearch
        Get
            Return _customerSearch
        End Get
        Set(ByVal value As DECustomerSearch)
            _customerSearch = value
        End Set
    End Property
    Public Property Customers() As ICollection(Of DECustomerV11)
        Get
            Return _customers
        End Get
        Set(ByVal value As ICollection(Of DECustomerV11))
            _customers = value
        End Set
    End Property

#End Region

#Region "Protected Methods"

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Return New ErrorObj
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Return New ErrorObj
    End Function

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = "CustomerSearch" : err = AccessDatabaseWS615S()
            Case Is = "CompanySearch" : err = AccessDatabaseEM001S()
        End Select

        Return err
    End Function

#End Region

#Region "Priavate Functions"

    Private Function AccessDatabaseEM001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("RecordsReturned", GetType(Integer))
        End With

        'Create the customer search data table
        Dim DtCompanySearchResults As New DataTable("SearchResults")
        ResultDataSet.Tables.Add(DtCompanySearchResults)
        With DtCompanySearchResults.Columns
            .Add("CompanyNumber", GetType(String))
            .Add("CompanyName", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("PostCode", GetType(String))
            .Add("Telephone", GetType(String))
            .Add("WebAddress", GetType(String))
        End With

        Try
            CallEM001S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBSearch-EM001S"
                .HasError = True
            End With
        End Try

        Return err
    End Function
    ''' <summary>
    ''' Retrieves customer search results from CD020 (with added joins for Membership details retrieval)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS615S() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("RecordsReturned", GetType(Integer))
        End With

        'Create the customer search data table
        Dim DtCustomerSearchResults As New DataTable("SearchResults")
        ResultDataSet.Tables.Add(DtCustomerSearchResults)
        With DtCustomerSearchResults.Columns
            .Add("CustomerNo", GetType(String))
            .Add("ContactTitle", GetType(String))
            .Add("ContactForename", GetType(String))
            .Add("ContactSurname", GetType(String))
            .Add("MembershipNumber", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("AddressLine2", GetType(String))
            .Add("AddressLine3", GetType(String))
            .Add("AddressLine4", GetType(String))
            .Add("AddressLine5", GetType(String))
            .Add("DateBirth", GetType(String))
            .Add("PassportNumber", GetType(String))
            .Add("EmailAddress", GetType(String))
            .Add("PostCode", GetType(String))
            .Add("UserID4", GetType(String))
            .Add("UserID5", GetType(String))
            .Add("UserID6", GetType(String))
            .Add("UserID7", GetType(String))
            .Add("UserID8", GetType(String))
            .Add("UserID9", GetType(String))
            .Add("PINNumber", GetType(String))
            .Add("GreenCardNumber", GetType(String))
            .Add("PhoneNumber1", GetType(String))
            .Add("PhoneNumber2", GetType(String))
            .Add("PhoneNumber3", GetType(String))
            .Add("WebReady", GetType(String))
            .Add("OnWatchList", GetType(String))
        End With

        Try
            CallWS615S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBSearch-WS615S"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Sub CallEM001S()
        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL EM001S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5,@PARAM6,@PARAM7,@PARAM8,@PARAM9,@PARAM10, @PARAM11, @PARAM12, @PARAM13, @PARAM14)")
        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pErrorCode As New iDB2Parameter
        Dim pSource As New iDB2Parameter
        Dim pCompanyName As New iDB2Parameter
        Dim pAddressLine1 As New iDB2Parameter
        Dim pPostCode As New iDB2Parameter
        Dim pTelephone As New iDB2Parameter
        Dim pWebAddress As New iDB2Parameter
        Dim pParentCompanyID As New iDB2Parameter
        Dim pStart As New iDB2Parameter
        Dim pLength As New iDB2Parameter
        Dim pRecordCount As New iDB2Parameter
        Dim pDraw As New iDB2Parameter
        Dim pSortField As New iDB2Parameter
        Dim pChildCompanyID As New iDB2Parameter
        Dim pSearchType As New iDB2Parameter

        pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pSource = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pCompanyName = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 50)
        pAddressLine1 = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 30)
        pPostCode = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 10)
        pTelephone = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 20)
        pWebAddress = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 60)
        pParentCompanyID = cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 13)
        pStart = cmd.Parameters.Add(Param8, iDB2DbType.iDB2Decimal, 10)
        pLength = cmd.Parameters.Add(Param9, iDB2DbType.iDB2Decimal, 10)
        pSortField = cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 50)
        pDraw = cmd.Parameters.Add(Param11, iDB2DbType.iDB2Decimal, 3)
        pRecordCount = cmd.Parameters.Add(Param12, iDB2DbType.iDB2Decimal, 10)
        pChildCompanyID = cmd.Parameters.Add(Param13, iDB2DbType.iDB2Char, 13)
        pSearchType = cmd.Parameters.Add(Param14, iDB2DbType.iDB2Char, 20)

        pErrorCode.Direction = ParameterDirection.InputOutput
        pRecordCount.Direction = ParameterDirection.InputOutput

        pSource.Value = _companySearch.Company.Source
        pErrorCode.Value = String.Empty
        pCompanyName.Value = _companySearch.Company.CompanyName
        pAddressLine1.Value = _companySearch.Company.AddressLine1
        pPostCode.Value = _companySearch.Company.PostCode
        pTelephone.Value = _companySearch.Company.TelephoneNumber1
        pWebAddress.Value = _companySearch.Company.WebAddress
        pParentCompanyID.Value = _companySearch.Company.ParentCompanyNumber
        pSortField.Value = _companySearch.SortOrder
        pDraw.Value = _companySearch.Draw
        pStart.Value = _companySearch.Start
        pLength.Value = _companySearch.Length
        pChildCompanyID.Value = _companySearch.Company.ChildCompanyNumber
        pSearchType.Value = _companySearch.Company.SearchType

        cmd.ExecuteNonQuery()

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd
        cmdAdapter.Fill(ResultDataSet, "SearchResults")

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            drStatus("RecordsReturned") = CInt(cmd.Parameters(12).Value)
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub

    Private Sub CallWS615S()
        Dim dateOfBirth As String = String.Empty
        ' Pass in DOB if set
        If _customerSearch.Customer.DateBirth <> String.Empty Then
            ' format to ticketing 
            dateOfBirth = (CInt(_customerSearch.Customer.DateBirth.Substring(6, 4) + _customerSearch.Customer.DateBirth.Substring(3, 2) + _customerSearch.Customer.DateBirth.Substring(0, 2)) - 19000000).ToString
        End If

        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL WS615S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5,")
        commandText.Append("@PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10, @PARAM11, @PARAM12,")
        commandText.Append("@PARAM13, @PARAM14, @PARAM15, @PARAM16, @PARAM17, @PARAM18, @PARAM19)")

        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pForename As New iDB2Parameter
        Dim pSurname As New iDB2Parameter
        Dim pPassportNumber As New iDB2Parameter
        Dim pAddressLine1 As New iDB2Parameter
        Dim pAddressLine2 As New iDB2Parameter
        Dim pAddressLine3 As New iDB2Parameter
        Dim pAddressLine4 As New iDB2Parameter
        Dim pPostcode As New iDB2Parameter
        Dim pDateOfBirth As New iDB2Parameter
        Dim pEMail As New iDB2Parameter
        Dim pAnyPhoneNumber As New iDB2Parameter
        Dim pContactNumber As New iDB2Parameter
        Dim pResultsLimit As New iDB2Parameter
        Dim pSource As New iDB2Parameter
        Dim pErrorCode As New iDB2Parameter
        Dim pStart As New iDB2Parameter
        Dim pLength As New iDB2Parameter
        Dim pRecordCount As New iDB2Parameter
        Dim pSortField As New iDB2Parameter
        Dim pDraw As New iDB2Parameter

        pSource = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
        pErrorCode = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
        pForename = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 20)
        pSurname = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 30)
        pAddressLine1 = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 30)
        pAddressLine2 = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 30)
        pAddressLine3 = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 30)
        pAddressLine4 = cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 30)
        pPostcode = cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 8)
        pDateOfBirth = cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 7)
        pEMail = cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 60)
        pAnyPhoneNumber = cmd.Parameters.Add(Param11, iDB2DbType.iDB2Char, 30)
        pResultsLimit = cmd.Parameters.Add(Param12, iDB2DbType.iDB2Decimal, 3)
        pStart = cmd.Parameters.Add(Param13, iDB2DbType.iDB2Decimal, 10)
        pLength = cmd.Parameters.Add(Param14, iDB2DbType.iDB2Decimal, 10)
        pSortField = cmd.Parameters.Add(Param15, iDB2DbType.iDB2Char, 50)
        pDraw = cmd.Parameters.Add(Param16, iDB2DbType.iDB2Char, 50)
        pRecordCount = cmd.Parameters.Add(Param17, iDB2DbType.iDB2Decimal, 10)
        pContactNumber = cmd.Parameters.Add(Param18, iDB2DbType.iDB2Char, 13)
        pPassportNumber = cmd.Parameters.Add(Param19, iDB2DbType.iDB2Char, 20)

        pErrorCode.Direction = ParameterDirection.InputOutput
        pRecordCount.Direction = ParameterDirection.InputOutput

        pSource.Value = _customerSearch.Customer.Source
        pErrorCode.Value = String.Empty
        pForename.Value = _customerSearch.Customer.ContactForename
        pSurname.Value = _customerSearch.Customer.ContactSurname
        pPassportNumber.Value = _customerSearch.Customer.PassportNumber
        pAddressLine1.Value = _customerSearch.Customer.AddressLine1
        pAddressLine2.Value = _customerSearch.Customer.AddressLine2
        pAddressLine3.Value = _customerSearch.Customer.AddressLine3
        pAddressLine4.Value = _customerSearch.Customer.AddressLine4
        pPostcode.Value = _customerSearch.Customer.PostCode
        pDateOfBirth.Value = dateOfBirth
        pEMail.Value = _customerSearch.Customer.WebAddress
        pAnyPhoneNumber.Value = _customerSearch.Customer.PhoneNumber
        pResultsLimit.Value = _customerSearch.Customer.SearchResultLimit
        If _customerSearch.Customer.SortOrder <> "" Then
            pSortField.Value = _customerSearch.Customer.SortOrder
        Else
            pSortField.Value = _customerSearch.SortOrder
        End If
        If _customerSearch.Customer.Start <> "" Then
            pStart.Value = _customerSearch.Customer.Start
        Else
            pStart.Value = _customerSearch.Start
        End If
        If _customerSearch.Customer.Length <> "" Then
            pLength.Value = _customerSearch.Customer.Length
        Else
            pLength.Value = _customerSearch.Length
        End If
        If _customerSearch.Customer.Draw <> "" Then
            pDraw.Value = _customerSearch.Customer.Draw
        Else
            pDraw.Value = _customerSearch.Draw
        End If

        Dim contactNumber As String = String.Empty
        If _customerSearch.Customer.ContactNumber Is Nothing OrElse _customerSearch.Customer.ContactNumber = String.Empty Then
            contactNumber = String.Empty
        Else
            contactNumber = Utilities.PadLeadingZeros(_customerSearch.Customer.ContactNumber, 12)
        End If
        pContactNumber.Value = contactNumber

        cmd.ExecuteNonQuery()

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd
        cmdAdapter.Fill(ResultDataSet, "SearchResults")

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(1).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            drStatus("RecordsReturned") = CInt(cmd.Parameters(17).Value)
            ResultDataSet.Tables(0).TableName = "StatusResults"
            ResultDataSet.Tables(1).TableName = "SearchResults"
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
#End Region

End Class