Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_serialized_objects based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_serialized_objects
        Inherits DBObjectBase
#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_serialized_objects"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_serialized_objects" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        Public Function Delete(ByVal objectType As String, ByVal loginID As String, ByVal uniqueID As String, ByVal createFlag As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE FROM tbl_serialized_objects " & _
                "WHERE OBJECT_TYPE=@ObjectType AND BUSINESS_UNIT = @BusinessUnit AND LOGINID=@LoginID AND UNIQUE_ID=@UniqueID AND CREATE_FLAG = @CreateFlag"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", _settings.BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ObjectType", objectType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@UniqueID", uniqueID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CreateFlag", createFlag, SqlDbType.Bit))
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

#End Region

    End Class

End Namespace
