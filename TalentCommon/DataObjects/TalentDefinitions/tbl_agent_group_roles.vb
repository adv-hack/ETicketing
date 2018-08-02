Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    <Serializable()> _
    Public Class tbl_agent_group_roles
        Inherits DBObjectBase

#Region "Contants"
        Const CACHEKEY_CLASSNAME As String = "tbl_agent_group_roles"
#End Region

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
        ''' Initializes a new instance of the <see cref="tbl_agent_group_roles" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the list of all Permission Groups
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetAllGroups(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim moduleName As String = "GetAllGroups"
            Dim dtOutput As DataTable = Nothing
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner)
            Dim isCacheNotFound As Boolean = False
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Try
                Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
                If isCacheNotFound Then
                    'Construct The Call
                    talentSqlAccessDetail = New TalentDataAccess
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT GROUP_ID,GROUP_NAME FROM [tbl_agent_group_roles] ORDER BY GROUP_NAME"
                    Dim err As New ErrorObj
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.TALENT_DEFINITION, cacheing, cacheTimeMinutes)
                    If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                        Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                    End If
                End If
                If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 Then
                    dtOutput = Me.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return dtOutput

        End Function

#End Region

    End Class

End Namespace





