Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_googlecheckout_serialnumber_status based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_googlecheckout_serialnumber_status
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_googlecheckout_serialnumber_status"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_googlecheckout_serialnumber_status" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

#End Region

#Region "Public Methods"

        Public Function Insert(ByVal serialNumber As String, ByVal comments As String, ByVal status As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO tbl_googlecheckout_serialnumber_status (" & _
                        "SERIAL_NUMBER, COMMENTS, STATUS, TIMESTAMP) VALUES (" & _
                        "@SerialNumber, @Comments, @Status, @Timestamp) "
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SerialNumber", serialNumber))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Comments", comments))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", status))
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


