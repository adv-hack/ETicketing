Imports System.Data.SqlClient
Imports System.Text
Imports System.Web

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_agent based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_agent
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_agent"
        Const CACHEKEY_GETBYAGENTNAME As String = "_GetByAgentName"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_agent" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets the details from tbl_agent for the given agent name.
        ''' </summary>
        ''' <param name="agentName">Name of the agent.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function GetByAgentName_MultiDB(ByVal agentName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Integer
            Dim outputDataTable As New DataTable
            Dim numberOfAgentRecords As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim connectionStringCount As Integer = _settings.ConnectionStringList.Count
            Dim connectionStringList As Generic.List(Of String) = _settings.ConnectionStringList
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT COUNT(*) FROM [tbl_agent] WHERE AGENT_NAME = @AgentName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))

                Dim saveFrontEndConnectionString As String = String.Empty
                saveFrontEndConnectionString = talentSqlAccessDetail.Settings.FrontEndConnectionString
                For connStringIndex As Integer = 0 To connectionStringCount - 1
                    talentSqlAccessDetail.Settings.FrontEndConnectionString = connectionStringList(connStringIndex)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        numberOfAgentRecords += talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                Next
                talentSqlAccessDetail.Settings.FrontEndConnectionString = saveFrontEndConnectionString
                talentSqlAccessDetail = Nothing
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return numberOfAgentRecords
        End Function

        ''' <summary>
        ''' Gets the details from tbl_agent for the given agent name and session.
        ''' </summary>
        ''' <param name="sessionID">Given Session ID</param>
        ''' <param name="agentName">Name of the agent.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function GetByAgentName(ByVal sessionID As String, ByVal agentName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.ModuleNameForCacheDependency = GlobalConstants.AGENT_PROFILE_CACHEKEY
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, CACHEKEY_GETBYAGENTNAME) & "_" & agentName
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_agent] WHERE SESSIONID = @SessionID AND AGENT_NAME = @AgentName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionID", sessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Inserts or update the details to tbl_agent table for the given agent name
        ''' </summary>
        ''' <param name="agentName">Name of the agent.</param>
        ''' <param name="passwordEncrypted">The password encrypted.</param>
        ''' <param name="printerNameDefault">The printer name default.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function InsertOrUpdate(ByVal sessionID As String, ByVal agentName As String, ByVal passwordEncrypted As String, ByVal printerNameDefault As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("IF EXISTS(SELECT AGENT_NAME FROM tbl_agent WHERE SESSIONID = @SessionID AND AGENT_NAME=@AgentName) ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append("   UPDATE tbl_agent SET PASSWORD_ENCRYPTED = @PasswordEncrypted, ")
                sqlStatement.Append("   PRINTER_NAME_DEFAULT = @PrinterNameDefault, ")
                sqlStatement.Append("   TIMESTAMP_UPDATED = @Timestamp ")
                sqlStatement.Append("   WHERE SESSIONID = @SessionID AND AGENT_NAME = @AgentName ")
                sqlStatement.Append("END ")
                sqlStatement.Append("ELSE ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append("   INSERT INTO tbl_agent (SESSIONID, AGENT_NAME, PASSWORD_ENCRYPTED, PRINTER_NAME_DEFAULT, TIMESTAMP_ADDED, TIMESTAMP_UPDATED) ")
                sqlStatement.Append("   VALUES (@SessionID, @AgentName, @PasswordEncrypted, @PrinterNameDefault, @Timestamp, @Timestamp) ")
                sqlStatement.Append("END ")
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionID", sessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PasswordEncrypted", passwordEncrypted))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrinterNameDefault", printerNameDefault))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Timestamp", Now, SqlDbType.DateTime))
                'Execute Insert
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If
                ClearAgentDetailsCache(agentName)
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Inserts or update the details to tbl_agent table for the given agent name
        ''' </summary>
        ''' <param name="agentName">Name of the agent.</param>
        ''' <param name="passwordEncrypted">The password encrypted.</param>
        ''' <param name="DEAgent">Agent details data entity.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function InsertOrUpdate(ByVal sessionID As String, ByVal agentName As String, ByVal passwordEncrypted As String, ByVal DEAgent As DEAgent, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("IF EXISTS(SELECT AGENT_NAME FROM tbl_agent WHERE SESSIONID = @SessionID AND AGENT_NAME=@AgentName) ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append("   UPDATE tbl_agent SET PASSWORD_ENCRYPTED = @PasswordEncrypted, ")
                sqlStatement.Append("   PRINTER_NAME_DEFAULT = @PrinterNameDefault, ")
                sqlStatement.Append("   COMPANY = @Company, ")
                sqlStatement.Append("   PRINTER_GROUP = @PrinterGroup, ")
                sqlStatement.Append("   DEFAULT_TICKET_PRINTER = @DefaultTicketPrinter, ")
                sqlStatement.Append("   DEFAULT_SMARTCARD_PRINTER = @DefaultSmartcardPrinter, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_HOME = @OverridePrinterHome, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_EVENT = @OverridePrinterEvent, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_TRAVEL = @OverridePrinterTravel, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_SEASONTICKET = @OverridePrinterSeasonTicket, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_ADDRESS = @OverridePrinterAddress, ")
                sqlStatement.Append("   PRINT_ADDRESS_LABELS_DEFAULT = @PrintAddressLabelDefault, ")
                sqlStatement.Append("   PRINT_TRANSACTION_RECEIPTS_DEFAULT = @PrintTransactionReceiptsDefault, ")
                sqlStatement.Append("   TIMESTAMP_UPDATED = @Timestamp, ")
                sqlStatement.Append("   DEPARTMENT = @Department, ")
                sqlStatement.Append("   BULK_SALES_MODE = @BulkSalesMode, ")
                sqlStatement.Append("   GROUP_ID = @GroupID, ")
                sqlStatement.Append("   PRINT_ALWAYS = @PrintAlways, ")
                sqlStatement.Append("   CAPTURE_METHOD = @CaptureMethod, ")
                sqlStatement.Append("   CORPORATE_HOSPITALITY_MODE = @CorporateHospitalityMode ")
                sqlStatement.Append("   WHERE AGENT_NAME = @AgentName ")
                sqlStatement.Append("END ")
                sqlStatement.Append("ELSE ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append("   INSERT INTO tbl_agent (SESSIONID, AGENT_NAME, PASSWORD_ENCRYPTED, PRINTER_NAME_DEFAULT, TIMESTAMP_ADDED, TIMESTAMP_UPDATED, COMPANY, PRINTER_GROUP,")
                sqlStatement.Append("   DEFAULT_TICKET_PRINTER, DEFAULT_SMARTCARD_PRINTER, OVERRIDE_PRINTER_HOME, OVERRIDE_PRINTER_EVENT, OVERRIDE_PRINTER_TRAVEL, OVERRIDE_PRINTER_SEASONTICKET,")
                sqlStatement.Append("   OVERRIDE_PRINTER_ADDRESS, PRINT_ADDRESS_LABELS_DEFAULT, PRINT_TRANSACTION_RECEIPTS_DEFAULT, DEPARTMENT, BULK_SALES_MODE, GROUP_ID, PRINT_ALWAYS, CAPTURE_METHOD, CORPORATE_HOSPITALITY_MODE) ")
                sqlStatement.Append("   VALUES (@SessionID, @AgentName, @PasswordEncrypted, @PrinterNameDefault, @Timestamp, @Timestamp, @Company, ")
                sqlStatement.Append("   @PrinterGroup, @DefaultTicketPrinter, @DefaultSmartcardPrinter, @OverridePrinterHome, ")
                sqlStatement.Append("   @OverridePrinterEvent, @OverridePrinterTravel, @OverridePrinterSeasonTicket, ")
                sqlStatement.Append("   @OverridePrinterAddress, @PrintAddressLabelDefault, @PrintTransactionReceiptsDefault, @Department, @BulkSalesMode, @GroupID, @PrintAlways, @CaptureMethod, @CorporateHospitalityMode) ")
                sqlStatement.Append("END ")
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionID", sessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PasswordEncrypted", passwordEncrypted))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrinterNameDefault", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@company", DEAgent.AgentCompany))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrinterGroup", DEAgent.PrinterGrp))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultTicketPrinter", DEAgent.DftTKTPrtr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultSmartcardPrinter", DEAgent.DftSCPrtr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterHome", DEAgent.Tkt1_Home))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterEvent", DEAgent.Tkt2_Event))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterTravel", DEAgent.Tkt3_Travel))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterSeasonTicket", DEAgent.Tkt4_STkt))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterAddress", DEAgent.Tkt5_Addr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrintAddressLabelDefault", Utilities.convertToBool(DEAgent.PrintAddrYN)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrintTransactionReceiptsDefault", Utilities.convertToBool(DEAgent.PrintRcptYN)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Timestamp", Now, SqlDbType.DateTime))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Department", DEAgent.Department))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BulkSalesMode", DEAgent.BulkSalesMode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupID", DEAgent.AgentAuthorityGroupID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrintAlways", DEAgent.PrintAlways))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CaptureMethod", DEAgent.DefaultCaptureMethod))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CorporateHospitalityMode", DEAgent.CorporateHospitalityMode))
                'Execute Insert
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If
                ClearAgentDetailsCache(agentName)
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Inserts or update the details to tbl_agent table for the given agent name
        ''' </summary>
        ''' <param name="agentName">Name of the agent.</param>
        ''' <param name="DEAgent">Agent details data entity.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function Update(ByVal agentName As String, ByVal DEAgent As DEAgent, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("IF EXISTS(SELECT AGENT_NAME FROM tbl_agent WHERE AGENT_NAME=@AgentName) ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append("   UPDATE tbl_agent SET ")
                sqlStatement.Append("   PRINTER_NAME_DEFAULT = @PrinterNameDefault, ")
                sqlStatement.Append("   COMPANY = @Company, ")
                sqlStatement.Append("   PRINTER_GROUP = @PrinterGroup, ")
                sqlStatement.Append("   DEFAULT_TICKET_PRINTER = @DefaultTicketPrinter, ")
                sqlStatement.Append("   DEFAULT_SMARTCARD_PRINTER = @DefaultSmartcardPrinter, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_HOME = @OverridePrinterHome, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_EVENT = @OverridePrinterEvent, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_TRAVEL = @OverridePrinterTravel, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_SEASONTICKET = @OverridePrinterSeasonTicket, ")
                sqlStatement.Append("   OVERRIDE_PRINTER_ADDRESS = @OverridePrinterAddress, ")
                sqlStatement.Append("   PRINT_ADDRESS_LABELS_DEFAULT = @PrintAddressLabelDefault, ")
                sqlStatement.Append("   PRINT_TRANSACTION_RECEIPTS_DEFAULT = @PrintTransactionReceiptsDefault, ")
                sqlStatement.Append("   TIMESTAMP_UPDATED = @Timestamp, ")
                sqlStatement.Append("   DEPARTMENT = @Department, ")
                sqlStatement.Append("   BULK_SALES_MODE = @BulkSalesMode, ")
                sqlStatement.Append("   GROUP_ID = @GroupID, ")
                sqlStatement.Append("   PRINT_ALWAYS = @PrintAlways, ")
                sqlStatement.Append("   CAPTURE_METHOD = @CaptureMethod, ")
                sqlStatement.Append("   CORPORATE_HOSPITALITY_MODE = @CorporateHospitalityMode ")
                sqlStatement.Append("WHERE AGENT_NAME = @AgentName ")
                sqlStatement.Append("END ")
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrinterNameDefault", ""))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@company", DEAgent.AgentCompany))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrinterGroup", DEAgent.PrinterGrp))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultTicketPrinter", DEAgent.DftTKTPrtr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultSmartcardPrinter", DEAgent.DftSCPrtr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterHome", DEAgent.Tkt1_Home))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterEvent", DEAgent.Tkt2_Event))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterTravel", DEAgent.Tkt3_Travel))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterSeasonTicket", DEAgent.Tkt4_STkt))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OverridePrinterAddress", DEAgent.Tkt5_Addr))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrintAddressLabelDefault", Utilities.convertToBool(DEAgent.PrintAddrYN)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrintTransactionReceiptsDefault", Utilities.convertToBool(DEAgent.PrintRcptYN)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Timestamp", Now, SqlDbType.DateTime))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Department", DEAgent.Department))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BulkSalesMode", DEAgent.BulkSalesMode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GroupID", DEAgent.AgentAuthorityGroupID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PrintAlways", DEAgent.PrintAlways))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CaptureMethod", DEAgent.DefaultCaptureMethod))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CorporateHospitalityMode", DEAgent.CorporateHospitalityMode))

                'Execute Insert
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If
                ClearAgentDetailsCache(agentName)
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Delete the agent record from tbl_agent based on the given agent name and session ID
        ''' </summary>
        ''' <param name="sessionID">The given session ID</param>
        ''' <param name="agentName">The given agent name</param>
        ''' <param name="givenTransaction">Option sql transaction object</param>
        ''' <returns>The number of affected rows, should always be 1 or 0</returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal sessionID As String, ByVal agentName As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_agent] WHERE SESSIONID = @SessionID AND [AGENT_NAME] = @AgentName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionID", sessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If
                ClearAgentDetailsCache(agentName)
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Delete the agent record from tbl_agent over multiple databases with the given agent name
        ''' </summary>
        ''' <param name="agentName"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete_MultiDB(ByVal agentName As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim connectionStringCount As Integer = _settings.ConnectionStringList.Count
            Dim connectionStringList As Generic.List(Of String) = _settings.ConnectionStringList
            Dim err As New ErrorObj
            Try
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_agent] WHERE [AGENT_NAME] = @AgentName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentName))
                
                Dim saveFrontEndConnectionString As String = String.Empty
                saveFrontEndConnectionString = talentSqlAccessDetail.Settings.FrontEndConnectionString
                For connStringIndex As Integer = 0 To connectionStringCount - 1
                    talentSqlAccessDetail.Settings.FrontEndConnectionString = connectionStringList(connStringIndex)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        affectedRows += talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                Next
                talentSqlAccessDetail.Settings.FrontEndConnectionString = saveFrontEndConnectionString
                talentSqlAccessDetail = Nothing
                ClearAgentDetailsCache(agentName)
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Update Bulksales mode and clear cache 
        ''' </summary>
        ''' <param name="agentName">The given Agent name</param>
        ''' <remarks></remarks>
        Public Sub UpdateBulkMode(ByVal AgentName As String, BulkSalesMode As Boolean)
            Dim cacheKey As New StringBuilder
            cacheKey.Append(GlobalConstants.DBACCESS_SQL)
            cacheKey.Append(GetCacheKeyPrefix(CACHEKEY_CLASSNAME, CACHEKEY_GETBYAGENTNAME))
            cacheKey.Append("_").Append(AgentName)
            Dim talBase As New TalentBase



            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess


            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "UPDATE tbl_agent SET BULK_SALES_MODE = 'FALSE' WHERE [AGENT_NAME] = @AgentName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", AgentName))
                err = talentSqlAccessDetail.SQLAccess()


            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            ClearAgentDetailsCache(AgentName)
        End Sub


        ''' <summary>
        ''' Remove the Agent Details record from cache.
        ''' </summary>
        ''' <param name="agentName">The given Agent name</param>
        ''' <remarks></remarks>
        Public Sub ClearAgentDetailsCache(ByVal agentName As String)
            Dim cacheKey As New StringBuilder
            cacheKey.Append(GlobalConstants.DBACCESS_SQL)
            cacheKey.Append(GetCacheKeyPrefix(CACHEKEY_CLASSNAME, CACHEKEY_GETBYAGENTNAME))
            cacheKey.Append("_").Append(agentName)
            Dim talBase As New TalentBase
            talBase.RemoveItemFromCache(cacheKey.ToString().ToLower())
        End Sub


        ''' <summary>
        ''' Get the agent record from tbl_agent based on the given session ID
        ''' </summary>
        ''' <param name="sessionID">The session ID</param>
        ''' <returns>Table of results with session, agent name and company</returns>
        ''' <remarks></remarks>
        Public Function RetrieveAgentDetailsFromSessionID(ByVal sessionID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "RetrieveAgentDetailsFromSessionID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim results As New DataTable
            Dim commandText As String = String.Format("SELECT SESSIONID, AGENT_NAME, COMPANY FROM tbl_agent a WHERE a.SESSIONID = '{0}' AND  NOT EXISTS(SELECT 1 FROM tbl_agent x WHERE x.AGENT_NAME = a.AGENT_NAME AND x.TIMESTAMP_UPDATED > a.TIMESTAMP_UPDATED AND x.SESSIONID <> a.SESSIONID) ", sessionID)
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = commandText
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionID", sessionID))


                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    results = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If

            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return results
        End Function
#End Region

    End Class

End Namespace


