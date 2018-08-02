Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBCRM
    Inherits DBAccess

#Region "Class Level Fields"

    Private Const RetrieveAttributeCategories As String = "RetrieveAttributeCategories"
    Private Const RetrieveAttributeDefinitions As String = "RetrieveAttributeDefinitions"
    Private Const RetrieveCustomerAttributes As String = "RetrieveCustomerAttributes"
    Private Const ProcessAttribute As String = "ProcessAttribute"
    Private Const RefreshEmailTemplates As String = "RefreshEmailTemplates"

#End Region

#Region "Public Properties"

    Public Property DeCRM() As New DETalentCRM

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = RetrieveAttributeCategories : err = AccessDatabaseAC001S()
            Case Is = RetrieveAttributeDefinitions : err = AccessDatabaseAT001S()
            Case Is = RetrieveCustomerAttributes : err = AccessDatabaseAX001S()
            Case Is = ProcessAttribute : err = AccessDatabaseAX001S()
            Case Is = RefreshEmailTemplates : err = AccessDatabaseEMID01R()
        End Select
        Return err
    End Function
#End Region

#Region "Private Functions"

    Private Function AccessDatabaseAC001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Attribute Definition data table
        Dim DtAttributeCategories As New DataTable("AttributeCategories")
        ResultDataSet.Tables.Add(DtAttributeCategories)

        Try
            Dim cmd As iDB2Command = conTALENTCRM.CreateCommand()

            cmd.CommandText = "Call AC001S(@PARAM0, @PARAM1)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
            parmIO_0.Value = "R"
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
            parmIO_1.Value = " "
            parmIO_1.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "AttributeCategories")
            ConvertISeriesTables(ResultDataSet)


            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(1).Value).Trim
                drStatus("ReturnCode") = "E"
            End If
            DtStatusResults.Rows.Add(drStatus)


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-AC001S"
                .HasError = True
            End With
        End Try

        Return err


    End Function

    Private Function AccessDatabaseAT001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Attribute Definition data table
        Dim DtAttributeDefinitions As New DataTable("AttributeDefinitions")
        ResultDataSet.Tables.Add(DtAttributeDefinitions)

        Try
            Dim cmd As iDB2Command = conTALENTCRM.CreateCommand()

            cmd.CommandText = "Call AT001S(@PARAM0, @PARAM1, @PARAM2)"
            cmd.CommandType = CommandType.Text

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
            parmIO_0.Value = "R"
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
            parmIO_1.Value = Utilities.PadLeadingZeros(DeCRM.AttributeCategoryCode, 3)
            parmIO_1.Direction = ParameterDirection.InputOutput

            Dim parmIO_2 As iDB2Parameter
            parmIO_2 = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
            parmIO_2.Value = " "
            parmIO_2.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "AttributeDefinitions")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(1).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(1).Value).Trim
                drStatus("ReturnCode") = "E"
            End If
            DtStatusResults.Rows.Add(drStatus)


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-AT001S"
                .HasError = True
            End With
        End Try

        Return err


    End Function

    Private Function AccessDatabaseAX001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim DtCustomerAttributes As New DataTable("CustomerAttributes")
        ResultDataSet.Tables.Add(DtCustomerAttributes)

        Try
            Dim cmd As iDB2Command = conTALENTCRM.CreateCommand()
            cmd.CommandText = "Call AX001S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5)"
            cmd.CommandType = CommandType.Text

            Dim pMode As iDB2Parameter
            Dim pCustomerNo As iDB2Parameter
            Dim pAttributeCode As iDB2Parameter
            Dim pAttributeID As iDB2Parameter
            Dim pAttributeCategoryCode As iDB2Parameter
            Dim pErrorCode As iDB2Parameter

            pMode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
            pCustomerNo = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 12)
            pAttributeCode = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 12)
            pAttributeID = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 13)
            pAttributeCategoryCode = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 3)
            pErrorCode = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 10)

            pMode.Value = DeCRM.AttributeOperation
            pCustomerNo.Value = DeCRM.CustomerNumber
            pAttributeCode.Value = DeCRM.AttributeCode
            pAttributeID.Value = Utilities.PadLeadingZeros(DeCRM.AttributeID, 13)
            pAttributeCategoryCode.Value = Utilities.PadLeadingZeros(DeCRM.AttributeCategoryCode, 3)
            pErrorCode.Value = String.Empty
            pErrorCode.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "CustomerAttributes")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(5).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
                drStatus("ReturnCode") = CStr(cmd.Parameters(5).Value).Trim
            End If
            DtStatusResults.Rows.Add(drStatus)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-AX001S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Calls EMID01R in CRM to publish records from tbl_email_templates to T#EMID 
    ''' </summary>
    ''' <returns>Error object based on the results</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseEMID01R() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim cmd As New iDB2Command
            Dim strProgram = "EMID01R"
            Dim strHeader As New StringBuilder
            strHeader.Append("CALL ")
            strHeader.Append(Settings.StoredProcedureGroup.Trim).Append("/")
            strHeader.Append(strProgram).Append("(@Param1)")
            Dim parm1 As New iDB2Parameter
            Dim nextRecord As Integer
            Dim first, last As Integer
            Dim numberOfItems As Integer = DeCRM.emailTemplateDefinition.Count
            Dim numberOfCallsToProgram As Integer = Convert.ToInt32(numberOfItems / 100) + 1

            For nextRecord = 0 To (numberOfCallsToProgram - 1)
                first = nextRecord * 100
                If (numberOfItems - first) < 99 Then
                    last = numberOfItems - 1
                Else
                    last = first + 99
                End If
                cmd = New iDB2Command(strHeader.ToString(), conTALENTCRM)
                parm1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 21405)
                parm1.Value = EMID01RParm(first, last)

                cmd.ExecuteNonQuery()
            Next
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-EMID01R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameter for EMID01R containing 100 records of email tempaltes and lastly nextrecord
    ''' </summary>
    ''' <param name="first">The index of the first template record in the list</param>
    ''' <param name="last">The index of the last template record in the list</param>
    ''' <returns>Input paramter for EMID01R program call</returns>
    ''' <remarks></remarks>
    Private Function EMID01RParm(ByVal first As Integer, ByVal last As Integer) As String
        Dim myString As New StringBuilder
        Dim emailTemplateDefinition As DEEmailTemplateDefinition
        For counter As Integer = first To last
            emailTemplateDefinition = DeCRM.emailTemplateDefinition.Item(counter)
            myString.Append(Utilities.FixStringLength(emailTemplateDefinition.EmailTemplateID, 13))
            myString.Append(Utilities.FixStringLength(emailTemplateDefinition.Name, 100))
            myString.Append(Utilities.FixStringLength(emailTemplateDefinition.BusinessUnit, 50))
            If Utilities.CheckForDBNull_Boolean_DefaultFalse(emailTemplateDefinition.Active) Then
                myString.Append(Utilities.FixStringLength(GlobalConstants.BACKEND_ACTIVE_FLAG_VALUE, 1))
            Else
                myString.Append(Utilities.FixStringLength(GlobalConstants.BACKEND_DISABLED_FLAG_VALUE, 1))
            End If
            myString.Append(Utilities.FixStringLength(emailTemplateDefinition.TemplateType, 50))
        Next
        Dim blank As Integer = 21400 - myString.Length
        myString.Append(Utilities.FixStringLength(String.Empty, blank))
        myString.Append(Utilities.FixStringLength((first + 1).ToString(), 5))
        Return myString.ToString()
    End Function

#End Region


End Class
