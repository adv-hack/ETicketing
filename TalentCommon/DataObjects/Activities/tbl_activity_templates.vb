Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_activity_templates
    ''' </summary>
    <Serializable()> _
    Public Class tbl_activity_templates
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_activity_templates"
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
        ''' Initializes a new instance of the <see cref="tbl_activity_templates" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the activity template records by the given business unit
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByBU(ByVal businessUnit As String, ByVal templateType As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBU")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & businessUnit & templateType
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT * FROM [tbl_activity_templates] WHERE [BUSINESS_UNIT]=@BusinessUnit "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                If templateType > 0 Then
                    sqlStatement += "AND [TEMPLATE_TYPE]=@TemplateType"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateType", templateType))
                End If
                sqlStatement += " ORDER BY [TEMPLATE_ID] DESC"
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

            'Return the results
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get the activity template records based on the given business unit and template ID
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="templateId">The given template ID</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByBUTemplateID(ByVal businessUnit As String, ByVal templateId As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBUTemplateID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & templateId & businessUnit
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_templates] WHERE [BUSINESS_UNIT]=@BusinessUnit AND [TEMPLATE_ID]=@TemplateID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateId))

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
        ''' Get the activity template records based on the template ID
        ''' </summary>
        ''' <param name="templateId">The given template ID</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByTemplateID(ByVal templateId As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByTemplateID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & templateId
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_templates] WHERE [TEMPLATE_ID]=@TemplateID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateId))

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
        ''' Delete records matching the given ID from tbl_activity_templates
        ''' </summary>
        ''' <param name="ID">tbl_activity_templates ID</param>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteByID(ByVal ID As Integer) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_activity_templates] WHERE [TEMPLATE_ID] = @ID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", ID))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        ''' Insert the activity template record with the given business unit and template name
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="templateName">The given template name</param>
        ''' <param name="templateType">The given template type</param>
        ''' <returns>The identity of the new inserted record</returns>
        ''' <remarks></remarks>
        Public Function InsertBUName(ByVal businessUnit As String, ByVal templateName As String, ByVal templateType As Integer) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "INSERT INTO [tbl_activity_templates] ([BUSINESS_UNIT], [NAME], [TEMPLATE_TYPE]) VALUES (@BusinessUnit, @Name, @TemplateType) SELECT SCOPE_IDENTITY()"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", templateName))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateType", templateType))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        ''' Update the activity template name based on the given template ID
        ''' </summary>
        ''' <param name="templateID">The template ID to update</param>
        ''' <param name="templateName">The template name to update to</param>
        ''' <param name="templateType">The template type to update to</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdateNameByID(ByVal templateID As String, ByVal templateName As String, ByVal templateType As Integer, ByVal isTemplatePerProduct As Boolean) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "UPDATE [tbl_activity_templates] SET [NAME]=@TemplateName, [TEMPLATE_TYPE]=@TemplateType, [TEMPLATE_PER_PRODUCT]=@TemplatePerProduct WHERE [TEMPLATE_ID]=@TemplateID "
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateName", templateName, SqlDbType.NVarChar))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateType", templateType, SqlDbType.NVarChar))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplatePerProduct", isTemplatePerProduct, SqlDbType.Bit))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        ''' Get the template based on the given template name and business unit
        ''' </summary>
        ''' <param name="templateName">The given template name</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByTemplateName(ByVal templateName As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByTemplateName")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & businessUnit
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_activity_templates WHERE [BUSINESS_UNIT]=@BusinessUnit AND [NAME]=@TemplateName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateName", templateName))

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
#End Region

    End Class
End Namespace
