Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_user_attribute
    ''' </summary>
    <Serializable()> _
    Public Class tbl_user_attribute
        Inherits DBObjectBase
#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_user_attribute"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_user_attribute" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Deletes existing user attributes and Inserts the given attributes to tbl_user_attribute
        ''' </summary>
        ''' <param name="loginId">The given login id string</param>
        ''' <param name="userAttributesTable">The user attributes table listed from TALENT</param>
        ''' <param name="specialAttributesTable">The special user attributes table listed from TALENT</param>
        ''' <returns>true if the delete and insert succeeded</returns>
        Public Function PopulateUserAttributes(ByVal userAttributesTable As DataTable, ByVal specialAttributesTable As DataTable, ByVal loginId As String) As Boolean
            Dim successfulUpdate As Boolean = False
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            loginId = Utilities.PadLeadingZeros(loginId, 12)
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_user_attribute] WHERE [LOGINID_ID]=@loginId"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@loginId", loginId))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing
            Try
                Using sqlBulkCopyObject As New SqlBulkCopy(_settings.FrontEndConnectionString)
                    sqlBulkCopyObject.DestinationTableName = "tbl_user_attribute"
                    sqlBulkCopyObject.ColumnMappings.Add("LoginId", "LOGINID_ID")
                    sqlBulkCopyObject.ColumnMappings.Add("AttributeName", "ATTR_NAME")
                    sqlBulkCopyObject.ColumnMappings.Add("AttributeId", "ATTR_ID")
                    sqlBulkCopyObject.ColumnMappings.Add("AttributeData", "ATTR_DATA")
                    sqlBulkCopyObject.WriteToServer(userAttributesTable)
                    sqlBulkCopyObject.WriteToServer(specialAttributesTable)
                    sqlBulkCopyObject.Close()
                    successfulUpdate = True
                End Using
            Catch ex As Exception
                successfulUpdate = False
            End Try
            Return successfulUpdate
        End Function

#End Region

    End Class
End Namespace