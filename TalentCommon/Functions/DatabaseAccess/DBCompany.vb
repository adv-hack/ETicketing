Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common
Imports Talent.Common.Utilities
Imports Talent.Common.UtilityExtension

<Serializable()> _
Public Class DBCompany
    Inherits DBAccess
#Region "Class Level Fields"
    Private Const FilterCompanyByCustomerDetails As String = "FilterCompanyByCustomerDetails"
    Private Const RetrieveCustomersByCompanyId As String = "CompanyContactsResults"
    Private Const ProcessCompanyContacts As String = "ProcessCompanyContacts"
    Private Const ParentCompanyOperations As String = "ParentCompanyOperations"
    Private Const CompanyOperations As String = "CompanyOperations"
    Private _deCustomer As DECustomer
    Private _deCompany As DECompany
#End Region

#Region "Protected Functions"
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = CompanyOperations : err = AccessDatabaseEM002S()
            Case Is = ParentCompanyOperations : err = AccessDatabaseEM005S()
            Case Is = RetrieveCustomersByCompanyId : err = AccessDatabaseEM003S()
            Case Is = ProcessCompanyContacts : err = AccessDatabaseEM004S()
        End Select

        Return err
    End Function
#End Region

    Public Property Company() As DECompany
        Get
            Return _deCompany
        End Get
        Set(ByVal value As DECompany)
            _deCompany = value
        End Set
    End Property
    Private Function AccessDatabaseEM005S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallEM005S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBCompany-EM005S"
                .HasError = True
            End With
        End Try

        Return err
    End Function
    Private Function AccessDatabaseEM003S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtCompanyContactsResults As New DataTable("CompanyContactsResults")
        ResultDataSet.Tables.Add(DtCompanyContactsResults)
        With DtCompanyContactsResults.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("Forename", GetType(String))
            .Add("Surname", GetType(String))
            .Add("Salutation", GetType(String))
            .Add("Telephone", GetType(String))
        End With

        Try
            CallEM003S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBCompany-EM003S"
                .HasError = True
            End With
        End Try

        Return err
    End Function
    Private Function AccessDatabaseEM004S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallEM004S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBCompany-EM004S"
                .HasError = True
            End With
        End Try

        Return err
    End Function
    Private Function AccessDatabaseEM002S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtCompanySearchResults As New DataTable("CompanySearchResults")
        ResultDataSet.Tables.Add(DtCompanySearchResults)
        With DtCompanySearchResults.Columns
            .Add("CompanyName", GetType(String))
            .Add("TelephoneNumber1", GetType(String))
            .Add("Telephone1Use", GetType(Boolean))
            .Add("TelephoneNumber2", GetType(String))
            .Add("Telephone2Use", GetType(Boolean))
            .Add("SalesLedgerAccount", GetType(String))
            .Add("OwningAgent", GetType(String))
            .Add("RegisteredDate", GetType(String))
            .Add("WebAddress", GetType(String))
            .Add("AddressType", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("AddressLine2", GetType(String))
            .Add("AddressLine3", GetType(String))
            .Add("County", GetType(String))
            .Add("Country", GetType(String))
            .Add("PostCode", GetType(String))
            .Add("VatCodeId", GetType(String))
            .Add("ParentCompanyNumber", GetType(String))
            .Add("ParentCompanyName", GetType(String))
            .Add("ParentFlag", GetType(String))
        End With

        'Create the Status data table
        Dim DtCompanyDetails As New DataTable("CompanyDetails")
        ResultDataSet.Tables.Add(DtCompanyDetails)
        With DtCompanyDetails.Columns
            .Add("CompanyNumber", GetType(String))
        End With
        Try
            CallEM002S()
            'Filter Parent on a EM002S
            'If _deCompany.CompanyOperationMode.ToString() = "Read" AndAlso _deCompany.IsParentCompany AndAlso _
            '    ResultDataSet.Tables("CompanySearchResults").Rows.Count > 0 Then
            '    Dim dvParentCompanyDetails As DataView = New DataView(ResultDataSet.Tables("CompanySearchResults"))
            '    dvParentCompanyDetails.RowFilter = "[ParentFlag]='P'"
            '    ResultDataSet.Tables("CompanySearchResults").Clear()
            '    ResultDataSet.Tables("CompanySearchResults").Merge(dvParentCompanyDetails.ToTable())
            'End If
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBCompany-EM002S"
                .HasError = True
            End With
        End Try

        Return err
    End Function


    Private Sub CallEM003S()
        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL EM003S(@PARAM0, @PARAM1)")
        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pErrorCode As New iDB2Parameter
        Dim pCompanyId As New iDB2Parameter

        pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pCompanyId = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)

        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty

        pCompanyId.Value = _deCompany.CompanyNumber

        'cmd.ExecuteNonQuery()

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd
        cmdAdapter.Fill(ResultDataSet, "CompanyContactsResults")

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ReturnCode") = String.Empty
            drStatus("ErrorOccurred") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
    Private Sub CallEM004S()
        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL EM004S(@PARAM0, @PARAM1 ,@PARAM2 ,@PARAM3 ,@PARAM4, @PARAM5)")
        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pErrorCode As New iDB2Parameter
        Dim pSource As New iDB2Parameter
        Dim pCompanyID As New iDB2Parameter
        Dim pTicketingID As New iDB2Parameter
        Dim pOperationMode As New iDB2Parameter
        Dim pUser As New iDB2Parameter

        pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pSource = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pCompanyID = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 13)
        pTicketingID = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 12)
        pOperationMode = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 6)
        pUser = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 10)

        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = _deCompany.Source
        pCompanyID.Value = _deCompany.CompanyNumber
        pTicketingID.Value = _deCompany.CustomerNumber
        pOperationMode.Value = _deCompany.CompanyOperationMode.ToString()
        pUser.Value = _deCompany.AgentName

        cmd.ExecuteNonQuery()

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
    Private Sub CallEM002S()
        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL EM002S(@PARAM0, @PARAM1 ,@PARAM2 ,@PARAM3 ,@PARAM4 ,@PARAM5, @PARAM6)")
        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pCompanyDetails As New iDB2Parameter
        Dim pCompanyAddressDetails As New iDB2Parameter
        Dim pSource As New iDB2Parameter
        Dim pErrorCode As New iDB2Parameter
        Dim pAgent As New iDB2Parameter
        Dim pOperationMode As New iDB2Parameter
        Dim pCompanyID As New iDB2Parameter

        pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pSource = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pCompanyDetails = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 250)
        pCompanyAddressDetails = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 210)
        pOperationMode = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 6)
        pAgent = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 10)
        pCompanyID = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 13)

        pErrorCode.Direction = ParameterDirection.InputOutput
        pCompanyID.Direction = ParameterDirection.Output

        pErrorCode.Value = String.Empty
        pCompanyID.Value = String.Empty

        pSource.Value = _deCompany.Source
        pCompanyDetails.Value = CreateCompanyDetailsParameter()
        pCompanyAddressDetails.Value = CreateCompanyAddressDetailsParameter()
        pOperationMode.Value = _deCompany.CompanyOperationMode.ToString()
        pAgent.Value = Settings.OriginatingSource

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd
        cmdAdapter.Fill(ResultDataSet, "CompanySearchResults")

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)

        Dim drCompanyDetail As DataRow = ResultDataSet.Tables("CompanyDetails").NewRow
        If cmd.Parameters(6).Value IsNot Nothing AndAlso Not IsDBNull(cmd.Parameters(6).Value) AndAlso CStr(cmd.Parameters(6).Value).Trim.Length > 0 Then
            drCompanyDetail("CompanyNumber") = CStr(cmd.Parameters(6).Value).Trim
        Else
            drCompanyDetail("CompanyNumber") = String.Empty
        End If
        ResultDataSet.Tables("CompanyDetails").Rows.Add(drCompanyDetail)
    End Sub
    Private Sub CallEM005S()
        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL EM005S(@PARAM0, @PARAM1 ,@PARAM2 ,@PARAM3, @PARAM4)")
        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pErrorCode As New iDB2Parameter
        Dim pOperationMode As New iDB2Parameter
        Dim pCompanyID As New iDB2Parameter
        Dim pParentCompanyID As New iDB2Parameter
        Dim pAgent As New iDB2Parameter

        pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pOperationMode = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 6)
        pCompanyID = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 13)
        pParentCompanyID = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 13)
        pAgent = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Decimal, 10)

        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pOperationMode.Value = _deCompany.CompanyOperationMode.ToString()
        pCompanyID.Value = _deCompany.CompanyNumber
        pParentCompanyID.Value = _deCompany.ParentCompanyNumber
        pAgent.Value = Settings.OriginatingSource

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd

        cmd.ExecuteNonQuery()

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub

    Private Function CreateCompanyDetailsParameter() As String
        Dim companyDetails As New StringBuilder
        companyDetails.Append(Utilities.FixStringLength(_deCompany.CompanyNumber, 13))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.CompanyName, 40))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.TelephoneNumber1, 20))
        companyDetails.Append(Utilities.FixStringLength(ConvertToYN(_deCompany.Telephone1Use), 1))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.TelephoneNumber2, 20))
        companyDetails.Append(Utilities.FixStringLength(ConvertToYN(_deCompany.Telephone2Use), 1))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.SalesLedgerAccount, 10))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.OwningAgent, 8))
        ' Safehold placment (8 char CRM agent vs 10 char agent name
        companyDetails.Append(Utilities.FixStringLength("  ", 2))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.WebAddress, 60))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.VATCodeID, 13))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.ChildCompanyNumber, 13))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.ParentCompanyNumber, 13))
        Return companyDetails.ToString
    End Function

    Private Function CreateCompanyAddressDetailsParameter() As String
        Dim companyDetails As New StringBuilder
        companyDetails.Append(Utilities.FixStringLength(_deCompany.AddressID, 13))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.AddressLine1, 40))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.AddressLine2, 40))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.AddressLine3, 40))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.County, 40))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.Country, 20))
        companyDetails.Append(Utilities.FixStringLength(_deCompany.PostCode, 10))
        Return companyDetails.ToString
    End Function
End Class
