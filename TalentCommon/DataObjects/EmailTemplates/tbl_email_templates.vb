Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_alert_definition
    ''' </summary>
    <Serializable()> _
    Public Class tbl_email_templates
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_email_templates"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_email_templates" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets all the records from tbl_email_templates
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAll")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_email_templates]"

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
        ''' Gets the email definition records based on the given business unit and partner.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBUAndPartner(Optional ByVal emailTemplateId As Decimal = 0, Optional ByVal active As Boolean = True, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataSet

            Dim outputDataTable As New DataTable
            Dim dsReturn As Data.DataSet = Nothing
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            ' Setup call to stored procedure usp_EmailTemplates_SelectAllByBUPartner
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_EmailTemplates_SelectAllByBUPartner"
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_BUSINESS_UNIT", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_PARTNER", _settings.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ACTIVE ", active))
            Catch ex As Exception
                _settings.Logging.ExceptionLog("EmailTemplates.vb|SelectAllByBUPartner", ex.Message)
            End Try

            ' Execute call to stored procedure
            If (Not errObj.HasError) Then
                Try
                    errObj = talentSqlAccessDetail.SQLAccess()
                Catch ex As Exception
                    _settings.Logging.ExceptionLog("EmailTemplates.vb|SelectAllByBUPartnerEtc", ex.Message)
                End Try
            End If

            ' Retrieve results of stored procedure
            If (Not errObj.HasError) Then
                If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                    dsReturn = talentSqlAccessDetail.ResultDataSet
                End If
            End If

            talentSqlAccessDetail = Nothing

            Return dsReturn


        End Function

        Public Function GetByEmailTemplateID(ByVal emailTemplateId As Decimal, Optional ByVal active As Boolean = True, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataSet

            Dim outputDataTable As New DataTable
            Dim dsReturn As Data.DataSet = Nothing
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            ' Setup call to stored procedure usp_EmailTemplates_SelectAllByBUPartner
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_EmailTemplates_SelectAllByBUPartner"
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_BUSINESS_UNIT", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_PARTNER", _settings.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAILTEMPLATE_ID ", emailTemplateId))
            Catch ex As Exception
                _settings.Logging.ExceptionLog("EmailTemplates.vb|GetByEmailTemplateID", ex.Message)
            End Try

            ' Execute call to stored procedure
            If (Not errObj.HasError) Then
                Try
                    errObj = talentSqlAccessDetail.SQLAccess()
                Catch ex As Exception
                    _settings.Logging.ExceptionLog("EmailTemplates.vb|GetByEmailTemplateID", ex.Message)
                End Try
            End If

            ' Retrieve results of stored procedure
            If (Not errObj.HasError) Then
                If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                    dsReturn = talentSqlAccessDetail.ResultDataSet
                End If
            End If

            talentSqlAccessDetail = Nothing

            Return dsReturn


        End Function
        Public Function DeleteByEmailTemplateID(ByVal emailTemplateId As Decimal, Optional ByVal active As Boolean = True, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As ErrorObj

            Dim outputDataTable As New DataTable
            Dim dsReturn As Data.DataSet = Nothing
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            ' Setup call to stored procedure usp_EmailTemplates_SelectAllByBUPartner
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_EmailTemplates_delEmailTemplate"
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAILTEMPLATE_ID ", emailTemplateId))
            Catch ex As Exception
                _settings.Logging.ExceptionLog("EmailTemplates.vb|DeletetByEmailTemplateID", ex.Message)
            End Try

            ' Execute call to stored procedure
            If (Not errObj.HasError) Then
                Try
                    errObj = talentSqlAccessDetail.SQLAccess()
                Catch ex As Exception
                    _settings.Logging.ExceptionLog("EmailTemplates.vb|DeletetByEmailTemplateID", ex.Message)
                End Try
            End If

            ' Retrieve results of stored procedure
            If (Not errObj.HasError) Then
                If (talentSqlAccessDetail.ResultDataSet IsNot Nothing) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables.Count > 0) Then
                    '       dsReturn = talentSqlAccessDetail.ResultDataSet
                End If
            End If

            talentSqlAccessDetail = Nothing

            Return errObj


        End Function


        Public Function InsertEmailTemplate(ByVal DEemailTemplate As Talent.Common.DEEmailTemplateDefinition) As Integer


            Dim intReturn As Integer
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            ' Setup call to stored procedure usp_EmailTemplates_InsertOrUpdateEmailTemplate
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_EmailTemplates_InsorUpdEmailTemplate"
                '                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ID", 0))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ACTIVE", DEemailTemplate.Active))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_NAME", DEemailTemplate.Name))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_DESCRIPTION", DEemailTemplate.Description))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_TEMPLATE_TYPE", DEemailTemplate.TemplateType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_HTML", DEemailTemplate.HTMLFormat))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_FROM_ADDRESS", DEemailTemplate.FromAddress))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_SUBJECT", DEemailTemplate.EmailSubject))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_BODY", DEemailTemplate.EmailBody))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_BUSINESS_UNIT", _settings.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_PARTNER", _settings.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_MASTER", DEemailTemplate.Master))
            Catch ex As Exception
                _settings.Logging.ExceptionLog("EmailTemplates.vb|usp_EmailTemplates_InsertOrUpdateEmailTemplate", ex.Message)
            End Try

            ' Execute call to stored procedure
            If (Not errObj.HasError) Then
                Try
                    errObj = talentSqlAccessDetail.SQLAccess()
                Catch ex As Exception
                    _settings.Logging.ExceptionLog("EmailTemplates.vb|usp_EmailTemplates_InsertOrUpdateEmailTemplate", ex.Message)
                End Try
            End If

            ' Retrieve results of stored procedure
            If (Not errObj.HasError) Then
                ' intReturn = talentSqlAccessDetail.CommandElements.CommandParameter("@PA_ID").ParamValue
                If Not talentSqlAccessDetail.ResultDataSet Is Nothing Then
                    If talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 Then
                        intReturn = CType(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0), Integer)
                    End If
                End If
            End If


            talentSqlAccessDetail = Nothing

            Return intReturn


        End Function


        Public Function UpdateEmailTemplate(ByVal DEemailTemplate As Talent.Common.DEEmailTemplateDefinition) As Integer


            Dim intReturn As Integer
            Dim errObj As New ErrorObj
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing

            ' Setup call to stored procedure usp_EmailTemplates_InsertOrUpdateEmailTemplate
            Try
                talentSqlAccessDetail = New TalentDataAccess
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandText = "usp_EmailTemplates_InsorUpdEmailTemplate"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ID", DEemailTemplate.EmailTemplateID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_NAME", DEemailTemplate.Name))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_DESCRIPTION", DEemailTemplate.Description))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_HTML", DEemailTemplate.HTMLFormat))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_FROM_ADDRESS", DEemailTemplate.FromAddress))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_SUBJECT", DEemailTemplate.EmailSubject))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_EMAIL_BODY", DEemailTemplate.EmailBody))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_ACTIVE", DEemailTemplate.Active))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PA_MASTER", DEemailTemplate.Master))


            Catch ex As Exception
                _settings.Logging.ExceptionLog("EmailTemplates.vb|usp_EmailTemplates_InsertOrUpdateEmailTemplate", ex.Message)
            End Try

            ' Execute call to stored procedure
            If (Not errObj.HasError) Then
                Try
                    errObj = talentSqlAccessDetail.SQLAccess()
                Catch ex As Exception
                    _settings.Logging.ExceptionLog("EmailTemplates.vb|usp_EmailTemplates_InsertOrUpdateEmailTemplate", ex.Message)
                End Try
            End If

            ' Retrieve results of stored procedure
            If (Not errObj.HasError) Then
                '                intReturn = CType(talentSqlAccessDetail.CommandElements.CommandParameter(0).ParamValue, Integer)
                If Not talentSqlAccessDetail.ResultDataSet Is Nothing Then
                    If talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 Then
                        intReturn = CType(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0), Integer)
                    End If
                End If
            End If

            talentSqlAccessDetail = Nothing

            Return intReturn


        End Function



#End Region

    End Class
End Namespace