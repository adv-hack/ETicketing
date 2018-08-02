Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' 
    ''' </summary>
    <Serializable()> _
    Public Class tbl_defaults
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_defaults"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_defaults" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        Public Function GetHexConversionValues(ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, ByVal cacheTimeMinutes As Integer, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim conversionValuesList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetHexConversionValues") & businessUnit & partner & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='HEXCONVERSION' AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetHexConversionValues")
                'Execute
                If dicObject IsNot Nothing Then
                    conversionValuesList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            'Return results
            Return conversionValuesList
        End Function

        ''' <summary>
        ''' Get the despatch courier service reference values and the text to display against each value
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">The optional cache time in mins, default 30</param>
        ''' <param name="cacheing">The optional cache value, default true</param>
        ''' <returns>A string dictionary of despatch service values and their text equivalents</returns>
        ''' <remarks></remarks>
        Public Function GetDespatchCourierServiceValues(ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim despatchCourierServiceList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDespatchCourierServiceValues") & businessUnit & partner & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='DESPATCH_COURIER_SERVICE' AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetDespatchCourierServiceValues")
                'Execute
                If dicObject IsNot Nothing Then
                    despatchCourierServiceList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            'Return results
            Return despatchCourierServiceList
        End Function

        ''' <summary>
        ''' Get the list of filename prefix values for the Despatch courier CSV file
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins default 30</param>
        ''' <param name="cacheing">Optionval cache enabled default true</param>
        ''' <returns>Dictionary of string values</returns>
        ''' <remarks></remarks>
        Public Function GetDespatchCourierPrefixValues(ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim despatchCourierPrefixList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDespatchCourierPrefixValues") & businessUnit & partner & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='DESPATCH_COURIER_PREFIX' AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetDespatchCourierPrefixValues")
                'Execute
                If dicObject IsNot Nothing Then
                    despatchCourierPrefixList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            'Return results
            Return despatchCourierPrefixList
        End Function

        ''' <summary>
        ''' Get the vangaurd attributes based on business unit, partner and language code
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <returns>Dictionary of vanguard attributes</returns>
        ''' <remarks></remarks>
        Public Function GetVanguardAttributes(ByVal businessUnit As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim dicVanguardAttributes As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVanguardAttributes") & businessUnit & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='VANGUARD_ATTRIBUTES' AND [BUSINESS_UNIT]=@BusinessUnit AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetVanguardAttributes")
                'Execute
                If dicObject IsNot Nothing Then
                    dicVanguardAttributes = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            'Return results
            Return dicVanguardAttributes
        End Function

        ''' <summary>
        ''' Get the vangaurd capture method based on business unit, partner and language code
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <returns>Dictionary of vanguard capture method</returns>
        ''' <remarks></remarks>
        Public Function GetVanguardCaptureMethod(ByVal businessUnit As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim dicVanguardCaptureMethod As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVanguardCaptureMethod") & businessUnit & languageCode
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='VANGUARD_CAPTUREMETHOD' AND [BUSINESS_UNIT]=@BusinessUnit AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetVanguardCaptureMethod")
                'Execute
                If dicObject IsNot Nothing Then
                    dicVanguardCaptureMethod = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            'Return results
            Return dicVanguardCaptureMethod
        End Function

        ''' <summary>
        ''' Get the vanguard configuration based on business unit, partner and language code
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <returns>Dictionary of vanguard attributes</returns>
        ''' <remarks></remarks>
        Public Function GetVanguardConfiguration(ByVal businessUnit As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim dicVanguardConfiguration As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVanguardConfiguration") & businessUnit & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='VANGUARD_CONFIGURATION' AND [BUSINESS_UNIT]=@BusinessUnit AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetVanguardConfiguration")

                'Execute
                If dicObject IsNot Nothing Then
                    dicVanguardConfiguration = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try
            Return dicVanguardConfiguration
        End Function

        ''' <summary>
        ''' Get the vangaurd payment information based on business unit, partner and language code
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <returns>Dictionary of vanguard payment details</returns>
        ''' <remarks></remarks>
        Public Function GetVanguardPayAccountConfiguration(ByVal businessUnit As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim dicVanguardPayAccount As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVanguardPayAccount") & businessUnit & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='VANGUARD_PAYACCOUNT' AND [BUSINESS_UNIT]=@BusinessUnit AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetVanguardPayAccount")

                'Execute
                If dicObject IsNot Nothing Then
                    dicVanguardPayAccount = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try
            Return dicVanguardPayAccount
        End Function


        ''' <summary>
        ''' Get the XML Order attributes based on business unit, partner and language code
        ''' Usually use for processing orders into an SAP system from XmlOrder.vb
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="languageCode">The given language code</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <returns>Dictionary of xml order attributes</returns>
        ''' <remarks></remarks>
        Public Function GetXmlOrderAttributes(ByVal businessUnit As String, ByVal languageCode As String, Optional ByVal cacheTimeMinutes As Integer = 30, Optional ByVal cacheing As Boolean = True) As Dictionary(Of String, String)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim dicVanguardPayAccount As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetXmlOrderAttributes") & businessUnit & languageCode
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT [APPLY_CODE], [APPLY_VALUE] FROM TBL_DEFAULTS WHERE [APPLY_TYPE]='XML_ORDER_ATTRIBUTES' AND [BUSINESS_UNIT]=@BusinessUnit AND [LANGUAGE_CODE]=@LanguageCode"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetXmlOrderAttributes")

                'Execute
                If dicObject IsNot Nothing Then
                    dicVanguardPayAccount = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try
            Return dicVanguardPayAccount
        End Function

#End Region

#Region "Protected Methods"
        ''' <summary>
        ''' Populates the custom dictionary entities. Overridden method called from DBObjectBase
        ''' </summary>
        ''' <param name="dtSourceToPopulate">The dt source to populate.</param>
        Protected Overrides Sub PopulateCustomDictionaryEntities(ByVal dtSourceToPopulate As System.Data.DataTable, ByVal callingModuleName As String)
            If dtSourceToPopulate IsNot Nothing AndAlso dtSourceToPopulate.Rows.Count > 0 Then
                Dim conversionValuesList As New Dictionary(Of String, String)
                For rowIndex As Integer = 0 To dtSourceToPopulate.Rows.Count - 1
                    Dim keyString As String = (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("APPLY_CODE"))).Trim.ToUpper
                    If Not conversionValuesList.ContainsKey(keyString) Then
                        conversionValuesList.Add(keyString, (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("APPLY_VALUE"))).Trim)
                    End If
                Next
                If conversionValuesList.Count > 0 Then
                    MyBase.CustomObject = conversionValuesList
                End If
            End If
        End Sub
#End Region
    End Class
End Namespace

