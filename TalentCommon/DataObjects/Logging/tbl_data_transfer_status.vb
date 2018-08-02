Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to log the activity in tbl_data_transfer_status table
    ''' </summary>
    <Serializable()> _
        Public Class tbl_data_transfer_status
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
        ''' Initializes a new instance of the <see cref="tbl_data_transfer_status" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Select all success records from tbl_data_transfer_status
        ''' </summary>
        ''' <returns>No of affected rows</returns>
        Public Function SelectAllRecords() As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                Dim sqlStatement As String = "SELECT * FROM [tbl_data_transfer_status] ORDER BY [START_TIME] DESC"
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

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

            'Return results
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Insert the given values to tbl_data_transfer_status table
        ''' </summary>
        ''' <param name="filename">the table name we are extracting</param>
        ''' <param name="status">The status of type DataTransferStatusEnum</param>
        ''' <param name="dataTransferStatusId">The data transfer status id as reference</param>
        ''' <returns>data transfer status id as reference and no of affected rows</returns>
        Public Function Insert(ByVal filename As String, ByVal status As DataTransferStatusEnum, ByRef dataTransferStatusId As Integer) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO [tbl_data_transfer_status] (" & _
                "[FILENAME], [START_TIME], [SUCCESS], [STATUS]) VALUES (" & _
                "@Filename, @StartTime, @Success, @Status)"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            Dim statusString As String = String.Empty
            Try
                statusString = System.Enum.GetName(GetType(DataTransferStatusEnum), status)
            Catch
                statusString = DataTransferStatusEnum.FAILED.ToString
            End Try
            Dim startTime As DateTime = DateTime.Now
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Filename", filename))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StartTime", startTime, SqlDbType.DateTime))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Success", False))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", statusString))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                If (affectedRows > 0) Then
                    sqlStatement = "SELECT TOP 1 [ID] FROM [tbl_data_transfer_status] WHERE " & _
                    "[FILENAME]=@Filename AND [START_TIME]=@StartTime AND " & _
                    "[SUCCESS]=@Success AND [STATUS]=@Status " & _
                    "ORDER BY [ID] DESC "
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.ResultDataSet.Tables.Clear()
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        dataTransferStatusId = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                End If
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Update the given values to tbl_data_transfer_status table with the supplied data transfer status ID
        ''' </summary>
        ''' <param name="dataTransferStatusID">the data transfer status ID</param>
        ''' <param name="filename">the table name we are extracting</param>
        ''' <param name="endTime">the transfer status end time</param>
        ''' <param name="success">success true or false value</param>
        ''' <param name="numberOfRecords">the number of records being transfered</param>
        ''' <param name="status">the data transfer status as DataTransferStatusEnum</param>
        ''' <param name="message">the exception message</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal dataTransferStatusID As Integer, ByVal status As DataTransferStatusEnum, _
            ByVal endTime As DateTime, ByVal success As Boolean, Optional ByVal filename As String = "", _
            Optional ByVal message As String = "", _
            Optional ByVal numberOfRecords As Integer = 0) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim statusString As String = String.Empty
            Try
                statusString = System.Enum.GetName(GetType(DataTransferStatusEnum), status)
            Catch
                statusString = DataTransferStatusEnum.FAILED.ToString
            End Try

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", statusString))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Success", success))
            Dim sqlStatement As String = "UPDATE [tbl_data_transfer_status] SET [STATUS] = @Status, [SUCCESS] = @Success"
            If Not String.IsNullOrEmpty(filename) Then
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Filename", filename))
                sqlStatement += ", [FILENAME] = @Filename"
            End If
            If endTime <> DateTime.MinValue Then
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@EndTime", endTime, SqlDbType.DateTime))
                sqlStatement += ", [END_TIME] = @EndTime"
            End If
            If Not String.IsNullOrEmpty(message) Then
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Message", message))
                sqlStatement += ", [MESSAGE] = @Message"
            End If
            If numberOfRecords > 0 Then
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NumberOfRecords", numberOfRecords))
                sqlStatement += ", [NUMBER_OF_RECORDS] = @NumberOfRecords"
            End If
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", dataTransferStatusID))
            sqlStatement += " WHERE [ID] = @ID"
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Delete records matching the given ID from tbl_data_transfer_status
        ''' </summary>
        ''' <param name="dataTransferStatusID">the data transfer status ID</param>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteByID(ByVal dataTransferStatusID As Integer) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_data_transfer_status] WHERE [ID] = @ID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", dataTransferStatusID))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Delete all success records from tbl_data_transfer_status
        ''' </summary>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteSuccessRecords() As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_data_transfer_status] WHERE [SUCCESS] = @Success"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Success", True))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Delete all failed records from tbl_data_transfer_status
        ''' </summary>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteFailedRecords() As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_data_transfer_status] WHERE [SUCCESS] = @Success AND [END_TIME] IS NOT NULL"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Success", False))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Delete all records from tbl_data_transfer_status
        ''' </summary>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteAllRecords() As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_data_transfer_status]"
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function
#End Region

    End Class

End Namespace
