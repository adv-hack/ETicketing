Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to log the activity in tbl_log_header table
    ''' </summary>
    <Serializable()> _
        Public Class tbl_logs
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_logs" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Insert the given values to tbl_logs table for the given log header id 
        ''' </summary>
        ''' <param name="logHeaderId">The log header id.</param>
        ''' <param name="sourceClass">The source class.</param>
        ''' <param name="sourceMethod">The source method.</param>
        ''' <param name="logCode">The log code.</param>
        ''' <param name="logFilter1">The log filter1.</param>
        ''' <param name="logFilter2">The log filter2.</param>
        ''' <param name="logFilter3">The log filter3.</param>
        ''' <param name="logFilter4">The log filter4.</param>
        ''' <param name="logFilter5">The log filter5.</param>
        ''' <param name="logContent">Content of the log.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function WriteLog(ByVal logHeaderId As Integer, _
                                    ByVal sourceClass As String, _
                                    ByVal sourceMethod As String, _
                                    ByVal logCode As String, _
                                    ByVal logFilter1 As String, _
                                    ByVal logFilter2 As String, _
                                    ByVal logFilter3 As String, _
                                    ByVal logFilter4 As String, _
                                    ByVal logFilter5 As String, _
                                    ByVal logContent As String, _
                                    Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO TBL_LOGS (" & _
                "LOG_SOURCE_CLASS, LOG_SOURCE_METHOD, LOG_CODE, " & _
                "LOG_FILTER_1, LOG_FILTER_2, LOG_FILTER_3, LOG_FILTER_4, LOG_FILTER_5, " & _
                "LOG_CONTENT, LOG_HEADER_ID) VALUES (" & _
                "@LogSourceClass, @LogSourceMethod, @LogCode, " & _
                "@LogFilter1, @LogFilter2, @LogFilter3, @LogFilter4, @LogFilter5, " & _
                "@LogContent, @LogHeaderId)"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogHeaderId", logHeaderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogSourceClass", sourceClass))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogSourceMethod", sourceMethod))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogCode", logCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogFilter1", logFilter1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogFilter2", logFilter2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogFilter3", logFilter3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogFilter4", logFilter4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogFilter5", logFilter5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogContent", logContent))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return logHeaderId
        End Function
#End Region

    End Class

End Namespace
