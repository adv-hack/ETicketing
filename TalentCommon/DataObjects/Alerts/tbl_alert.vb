Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_alert
    ''' </summary>
    <Serializable()> _
    Public Class tbl_alert
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_alert"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_alert" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Mark an alert as read based on the given alert id
        ''' </summary>
        ''' <param name="alertId">The alert Id to mark as read</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function MarkAlertAsRead(ByVal alertId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim talentSqlAccessDetail0 As New TalentDataAccess
            Dim err As New ErrorObj

            ' Retrieve the list of SQL DB's to be updated from the settings object
            Dim connectionStringCount As Integer = _settings.ConnectionStringList.Count
            Dim connectionStringList As Generic.List(Of String) = _settings.ConnectionStringList


            ' Retrieve the relevant keys to identify the correct tbl_alert record on ANY web server as ID may chnage from box-to-box
            Dim alertDatatable As Data.DataTable
            talentSqlAccessDetail0.Settings = _settings
            talentSqlAccessDetail0.Settings.Cacheing = False
            talentSqlAccessDetail0.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement0 As String = "Select [BUSINESS_UNIT], [PARTNER], [LOGIN_ID], [ALERT_ID], [DESCRIPTION] from [tbl_alert] WHERE [ID]=@AlertId"
            talentSqlAccessDetail0.CommandElements.CommandText = sqlStatement0
            talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@AlertId", alertId))
            err = talentSqlAccessDetail0.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail0.ResultDataSet Is Nothing)) Then

                alertDatatable = talentSqlAccessDetail0.ResultDataSet.Tables(0)

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE [tbl_alert] SET [READ]=1 WHERE [BUSINESS_UNIT]=@Business_Unit AND [PARTNER]=@Partner " & _
                                             "AND [LOGIN_ID]=@Loginid AND [ALERT_ID]=@Alertid AND [DESCRIPTION]=@Description"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Business_Unit", alertDatatable.Rows(0)(0)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", alertDatatable.Rows(0)(1)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Loginid", alertDatatable.Rows(0)(2)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertId", alertDatatable.Rows(0)(3)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Description", alertDatatable.Rows(0)(4)))


                ''Construct The Call
                'talentSqlAccessDetail.Settings = _settings
                'talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                'Dim sqlStatement As String = "UPDATE [tbl_alert] SET [READ]=1 WHERE [ID]=@AlertId"
                'talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertId", alertId))

                ' Save frontEndConnectionString
                Dim saveFrontEndConnectionString As String = String.Empty
                saveFrontEndConnectionString = talentSqlAccessDetail.Settings.FrontEndConnectionString

                For connStringIndex As Integer = 0 To connectionStringCount - 1

                    talentSqlAccessDetail.Settings.FrontEndConnectionString = connectionStringList(connStringIndex)

                    'Execute
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail.SQLAccess()
                    Else
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                    End If
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If

                Next

                ' Restore frontEndConnectionString
                talentSqlAccessDetail.Settings.FrontEndConnectionString = saveFrontEndConnectionString

                talentSqlAccessDetail = Nothing

            End If

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Mark an alert as unread based on the given alert id
        ''' </summary>
        ''' <param name="alertId">The alert Id to mark as unread</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function MarkAlertAsUnRead(ByVal alertId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE [tbl_alert] SET [READ]=0 WHERE [ID]=@AlertId"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AlertId", alertId))

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
            Return affectedRows
        End Function

        Public Function DeleteUnReadAlertsByID(ByVal loginID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_alert] WHERE LOGIN_ID = @LoginID AND [READ] = 0"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
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
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function

        Public Function DeleteReservationAlertsByLoginID(ByVal loginID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Boolean
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_alert] WHERE LOGIN_ID = @LoginID AND [ALERT_ID] = 5"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
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
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function

        Public Function DeleteAlertByID(ByVal AlertTableId As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_alert] WHERE ID = @ID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", AlertTableId))
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
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function
#End Region

    End Class
End Namespace