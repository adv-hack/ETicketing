Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_activity_templates_defaults
    ''' </summary>
    <Serializable()> _
    Public Class tbl_activity_templates_defaults
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings
        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_activity_templates_defaults"
        ''' <summary>
        ''' to create only one instance of string when called by insert multiple
        ''' as it is a long string 
        ''' as well as to speed up the insert when called under transaction
        ''' </summary>
        Private ReadOnly _insertSQLStatement As String = String.Empty
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_activity_templates_detail" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Returns the default name/value pairs for a particular template
        ''' </summary>
        ''' <param name="templateID"></param>
        ''' <returns>A datatable with the defaults</returns>
        Public Function GetDefaultsByTemplateID(ByVal templateID As Integer) As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "SELECT DEFAULT_NAME, DEFAULT_VALUE FROM [tbl_activity_templates_defaults] WHERE [TEMPLATE_ID] = @templateID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@templateID", templateID))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return outputDataTable
        End Function

        Public Function InsertDefaultValue(ByVal templateID As Integer, ByVal defaultName As String, ByVal defaultValue As String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            Dim sqlStatement As String = "INSERT INTO [tbl_activity_templates_defaults] ([TEMPLATE_ID], [DEFAULT_NAME], [DEFAULT_VALUE]) VALUES (@templateID, @defaultName, @defaultValue)"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@templateID", templateID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@defaultName", defaultName))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@defaultValue", defaultValue))
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

        Public Function UpdateDefaultValue(ByVal templateID As Integer, ByVal defaultName As String, ByVal defaultValue As String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            Dim sqlStatement As String = "UPDATE [tbl_activity_templates_defaults] SET [DEFAULT_VALUE] = @defaultValue WHERE [TEMPLATE_ID] = @templateID AND [DEFAULT_NAME] = @defaultName"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@defaultValue", defaultValue))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@templateID", templateID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@defaultName", defaultName))
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
