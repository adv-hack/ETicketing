Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_offline_printing based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_offline_processing
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_offline_processing"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_offline_processing" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Inserts a record into the  table tbl_offline_printing table
        ''' </summary>
        ''' <param name="businessUnit">Business unit.</param>
        ''' <param name="partner">Partner</param>
        ''' <param name="status">Status</param>
        ''' <param name="logHeaderId">Log Header Id</param>
        ''' <param name="serverName">Server Name</param>
        ''' <param name="monitorName">Monitor Name</param>
        ''' <param name="requestType">Request Type</param>
        ''' <param name="parameter">Parameter</param>
        ''' <param name="errorInformation">Error Information</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function Insert(ByVal businessUnit As String, _
                        ByVal partner As String, _
                        ByVal status As String, _
                        ByVal logHeaderId As Integer, _
                        ByVal serverName As String, _
                        ByVal monitorName As String, _
                        ByVal requestType As String, _
                        ByVal parameter As String, _
                        ByVal errorInformation As String, _
                        Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "Insert into tbl_offline_processing " & _
                                            " (business_unit, partner, status, log_header_id, server_name, monitor_name, " & _
                                            " request_type, parameter, error_information, timestamp_added, timestamp_last_updated) " & _
                                            " Values (@businessUnit, @partner, @status, @logHeaderId, @serverName, @monitorName, " & _
                                            "@requestType, @parameter, @errorInformation, @timestampAdded, @timestampLastUpdated) "


            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@businessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@status", status))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@logHeaderId", logHeaderId, SqlDbType.BigInt))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@serverName", serverName))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@monitorName", monitorName))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@requestType", requestType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@parameter", parameter, SqlDbType.NText))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@errorInformation", errorInformation))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@timestampAdded", Now, SqlDbType.DateTime))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@timestampLastUpdated", Now, SqlDbType.DateTime))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            'Return the results 
            Return affectedRows

        End Function

#End Region

    End Class

End Namespace

