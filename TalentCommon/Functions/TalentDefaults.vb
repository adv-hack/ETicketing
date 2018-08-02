Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentDefaults
    Inherits TalentBase

#Region "Class level fields"
    Private Const TalentDefaults As String = "TALENTDEFAULTS"
#End Region

#Region "Public Properties"

    Public Property ResultDataSet() As DataSet

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Defaults retrieval for any default settings defined in the system defaults in TALENT
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RetrieveTalentDefaults() As ErrorObj
        Const ModuleName As String = "RetrieveTalentDefaults"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & "")
        Dim err As New ErrorObj
        Dim dbTalDefault As New DBTalentDefaults
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbTalDefault
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    Settings.ModuleName = TalentDefaults
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function


    Public Function RetrieveCountryDefinitions() As ErrorObj
        Const ModuleName As String = "RetrieveCountryDefinitions"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & "")
        Dim err As New ErrorObj
        Dim dbTalDefault As New DBTalentDefaults
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbTalDefault
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveGeographicalZone(ByVal countryCode As String) As String
        Dim geographicalZone As String = String.Empty
        Const ModuleName As String = "RetrieveGeographicalZone"
        Dim givenCacheingFlag As Boolean = Settings.Cacheing
        Settings.Cacheing = True
        Dim cacheKey As String = ModuleName & Settings.Company & countryCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            geographicalZone = CType(HttpContext.Current.Cache.Item(cacheKey), String)
        Else
            Dim err As ErrorObj = RetrieveCountryDefinitions()
            If Not err.HasError AndAlso ResultDataSet IsNot Nothing Then
                Dim countryDefinitions As DataTable = ResultDataSet.Tables("CountryDefinitions")
                Dim countryDefinitionSysArray() As DataRow
                countryDefinitionSysArray = countryDefinitions.Select("COUNTRYISOCODE = '" & countryCode & "'")
                If countryDefinitionSysArray.Length > 0 Then
                    geographicalZone = countryDefinitionSysArray(0).Item("ZONECODE")
                End If
                AddItemToCache(cacheKey, geographicalZone, Settings)
            Else
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End If
        Settings.Cacheing = givenCacheingFlag
        Return geographicalZone
    End Function
    Public Function RetrieveCustomerTitles() As ErrorObj
        Const ModuleName As String = "RetrieveCustomerTitles"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & "")
        Dim err As New ErrorObj
        Dim dbTalDefault As New DBTalentDefaults
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName & Settings.Company
        If Not Settings.IsAgent Then cacheKey = cacheKey.Trim & "PWS"

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbTalDefault
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function
    Public Function RetrieveSpecificCountryDefinition(ByVal countryCode As String) As DECountry
        Dim countryEntity As DECountry = Nothing
        Const ModuleName As String = "RetrieveSpecificCountryDefinition"
        Dim givenCacheingFlag As Boolean = Settings.Cacheing
        Settings.Cacheing = True
        Dim cacheKey As String = ModuleName & Settings.Company & countryCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            countryEntity = CType(HttpContext.Current.Cache.Item(cacheKey), DECountry)
        Else
            Dim err As ErrorObj = RetrieveCountryDefinitions()
            If Not err.HasError AndAlso ResultDataSet IsNot Nothing Then
                Dim countryDefinitions As DataTable = ResultDataSet.Tables("CountryDefinitions")
                Dim drCountryDefinition() As DataRow = countryDefinitions.Select("COUNTRYISOCODE = '" & countryCode & "'")
                If drCountryDefinition.Length > 0 Then
                    countryEntity = New DECountry
                    countryEntity.Country = Utilities.CheckForDBNull_String(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.CountryISOCode = Utilities.CheckForDBNull_String(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.ZoneNumber = Utilities.CheckForDBNull_Int(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.PostAllowed = Utilities.CheckForDBNull_Boolean_DefaultFalse(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.PostCodeMandatory = Utilities.CheckForDBNull_Boolean_DefaultFalse(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.PostDaysBefore = Utilities.CheckForDBNull_Int(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.RegPostDaysBefore = Utilities.CheckForDBNull_Int(drCountryDefinition(0).Item("ZONECODE"))
                    countryEntity.ZoneCode = Utilities.CheckForDBNull_String(drCountryDefinition(0).Item("ZONECODE"))
                    AddItemToCache(cacheKey, countryEntity, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            Else
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End If
        Settings.Cacheing = givenCacheingFlag
        Return countryEntity
    End Function

#End Region

End Class