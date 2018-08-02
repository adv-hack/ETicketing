Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from tbl_country
    ''' </summary>
    <Serializable()> _
    Public Class tbl_country
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_country"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_country" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get True or False value based on whether the provided country code can use gift aid
        ''' </summary>
        ''' <param name="countryCode">The country code provided</param>
        ''' <param name="cacheing">Cacheing On/Off - Default=True</param>
        ''' <param name="cacheTimeMinutes">Cache Time In Minutes - Default=30</param>
        ''' <returns>Boolean</returns>
        Public Function CanCountryUseGiftAid(ByVal countryCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean

            Dim canUseGiftAid As Boolean = False
            Dim outputDataSet As New DataSet
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "CanCountryUseGiftAid")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & countryCode.ToUpper
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [CAN_USE_GIFT_AID_OPTION] FROM tbl_country WITH(NOLOCK) WHERE [COUNTRY_CODE] = @countryCode OR [COUNTRY_DESCRIPTION] = @countryCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@countryCode", countryCode))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError) And (talentSqlAccessDetail.ResultDataSet IsNot Nothing)) Then
                    outputDataSet = talentSqlAccessDetail.ResultDataSet
                    If outputDataSet.Tables.Count = 1 Then
                        If outputDataSet.Tables(0).Rows.Count = 1 Then
                            canUseGiftAid = CBool(outputDataSet.Tables(0).Rows(0)("CAN_USE_GIFT_AID_OPTION"))
                        End If
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the result
            Return canUseGiftAid

        End Function

        ''' <summary>
        ''' Gets all the business unit records from the table tbl_country
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
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_COUNTRY"

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
        ''' Get the country code by the given country description
        ''' </summary>
        ''' <param name="countryDescription">The country description provided</param>
        ''' <param name="cacheing">Cacheing On/Off - Default=True</param>
        ''' <param name="cacheTimeMinutes">Cache Time In Minutes - Default=30</param>
        ''' <returns>The country code</returns>
        Public Function GetCountryCodeByDescription(ByVal countryDescription As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim countryCode As String = String.Empty
            Dim outputDataSet As New DataSet
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetCountryCodeByDescription")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & countryDescription.ToUpper()
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [COUNTRY_CODE] FROM tbl_country WITH(NOLOCK) WHERE [COUNTRY_DESCRIPTION] = @CountryDescription"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CountryDescription", countryDescription))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError) And (talentSqlAccessDetail.ResultDataSet IsNot Nothing)) Then
                    outputDataSet = talentSqlAccessDetail.ResultDataSet
                    If outputDataSet.Tables.Count = 1 Then
                        If outputDataSet.Tables(0).Rows.Count = 1 Then
                            countryCode = outputDataSet.Tables(0).Rows(0)("COUNTRY_CODE")
                        End If
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the result
            Return countryCode
        End Function

        Public Function CanCountryAllowVATExemption(ByVal countryCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean

            Dim canAllow As Boolean = False
            Dim outputDataSet As New DataSet
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "CanCountryAllowVATExemption")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & countryCode.ToUpper
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [IS_VAT_EXEMPTED] FROM tbl_country WITH(NOLOCK) WHERE [COUNTRY_CODE] = @countryCode OR [COUNTRY_DESCRIPTION] = @countryCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@countryCode", countryCode))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError) And (talentSqlAccessDetail.ResultDataSet IsNot Nothing)) Then
                    outputDataSet = talentSqlAccessDetail.ResultDataSet
                    If outputDataSet.Tables.Count = 1 Then
                        If outputDataSet.Tables(0).Rows.Count = 1 Then
                            canAllow = Utilities.CheckForDBNull_Boolean_DefaultFalse(outputDataSet.Tables(0).Rows(0)("IS_VAT_EXEMPTED"))
                        End If
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the result
            Return canAllow

        End Function
        ''' <summary>
        ''' Get ISO Alpha 3 code for given country code or description
        ''' </summary>
        ''' <param name="countryCodeOrDescription"></param>
        ''' <param name="cacheing"></param>
        ''' <param name="cacheTimeMinutes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function GetISOAlpha3ByCodeOrDescription(ByVal countryCodeOrDescription As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim countryCode As String = String.Empty
            If String.IsNullOrWhiteSpace(countryCodeOrDescription) Then countryCodeOrDescription = ""

            Dim moduleName As String = "GetISOAlpha3ByDescription"
            Dim dtOutput As DataTable = Nothing
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & countryCodeOrDescription.ToUpper)
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
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT [ISO_ALPHA_3] FROM tbl_country WITH(NOLOCK) WHERE ([COUNTRY_CODE] = @countryCodeOrDescription OR [COUNTRY_DESCRIPTION] = @countryCodeOrDescription)"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@countryCodeOrDescription", countryCodeOrDescription))
                    Dim err As New ErrorObj
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, cacheing, cacheTimeMinutes)
                    If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                        Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                    End If
                End If

                If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 AndAlso Me.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    countryCode = Utilities.CheckForDBNull_String(Me.ResultDataSet.Tables(0).Rows(0)("ISO_ALPHA_3")).ToUpper
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return countryCode


        End Function

#End Region

    End Class

End Namespace