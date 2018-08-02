Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web

<Serializable()> _
Public Class TalentPromotions
    Inherits TalentBase

#Region "Public Properties"

    Public Property Dep() As DEPromotions
    Public Property ResultDataSet() As DataSet

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Process Promotion code
    ''' </summary>
    ''' <param name="intMode">Set to <c>0</c>  for Ticketing promotions, Set to <c>1</c> for Retail promotions.</param>
    ''' <returns>Error Object</returns>
    Public Function ProcessPromotions(Optional ByVal intMode As Integer = 99) As ErrorObj
        Const ModuleName As String = "ProcessPromotions"

        Dim sModuleNameForDatabaseConnection As String = "ProcessPromotions"
        Select Case intMode
            Case Is = 0
                sModuleNameForDatabaseConnection = "ProcessPromotionsTicketing"
            Case Is = 1
                sModuleNameForDatabaseConnection = "ProcessPromotionsRetail"
            Case Is = 99
                sModuleNameForDatabaseConnection = "ProcessPromotions"
        End Select

        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", sModuleNameForDatabaseConnection)
            Dim dbPromo As New DBPromotions
            With dbPromo
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    Else
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                End If
            End With
        End If
        Return err
    End Function

    Public Function GetPromotionDetails() As ErrorObj
        Const ModuleName As String = "PromotionDetails"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '

        Dim cacheKey As String = ModuleName & Settings.BusinessUnit & Settings.Partner & Dep.PromotionId
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBPromo As New DBPromotions
            With DBPromo
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    'HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                '
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        'HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                End If
            End With
        End If

        Return err
    End Function

    Public Function RetrievePromotionHistory() As ErrorObj
        Dim ph As New DBPromotions
        Const ModuleName As String = "RetrievePromotionHistory"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & Dep.CustomerNumber

        ' TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.CustomerNumber)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        '     If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        ' ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        ' Else

        With ph
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Dep = Dep

            err = .AccessDatabase()

            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                AddItemToCache(cacheKey, ResultDataSet, Settings)
            ElseIf Settings.Cacheing Then
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

        End With

        Return err
    End Function

    Public Function RetrievePromotionHistoryDetail() As ErrorObj
        Dim ph As New DBPromotions
        Const ModuleName As String = "RetrievePromotionHistoryDetail"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & Dep.CustomerNumber

        ' TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.CustomerNumber)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        '     If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        ' ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        ' Else

        With ph
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Dep = Dep

            '.CustomerNumber = Me.CustomerNumber
            '.Agent = Me.Agent
            err = .AccessDatabase()

            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                AddItemToCache(cacheKey, ResultDataSet, Settings)
            ElseIf Settings.Cacheing Then
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

        End With

        Return err
    End Function

    ''' <summary>
    ''' Get the partner promotions data and populate the resultDataSet
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function GetPartnerPromotions() As ErrorObj
        Const ModuleName As String = "GetPartnerPromotions"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Dim DBPromo As New DBPromotions
        With DBPromo
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function

#End Region

End Class



