Imports Talent.Common.Utilities
Imports System.Web

<Serializable()> _
Public Class TalentActivities
    Inherits TalentBase

#Region "Public Properties"

    Public Property ResultDataSet() As DataSet
    Public Property De As DEActivities

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Create a customer activities entry and return customer activities populated in a resultset
    ''' Always update the cache as the resultset is being changed
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function AddCustomerActivity() As ErrorObj
        Const ModuleName As String = "AddCustomerActivity"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesHeaderMode = "C"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                RemoveItemFromCache(getCacheKey())
                AddItemToCache(getCacheKey(), ResultDataSet, Settings)
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve customer activities entry
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function CustomerActivitiesRetrieval() As ErrorObj
        Const ModuleName As String = "CustomerActivitiesRetrieval"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)
        Dim cacheKey As String = getCacheKey()

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            Dim activities As New DBActivities
            With activities
                De.CustomerActivitiesHeaderMode = "R"
                .ActivitiesDataEntity = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Update a customer activities entry and return customer activities populated in a resultset
    ''' Always update the cache as the resultset is being changed
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function UpdateCustomerActivity() As ErrorObj
        Const ModuleName As String = "UpdateCustomerActivity"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesHeaderMode = "U"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                RemoveItemFromCache(getCacheKey())
                AddItemToCache(getCacheKey(), ResultDataSet, Settings)
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Delete a customer activities entry and return customer activities populated in a resultset
    ''' Always update the cache as the resultset is being changed
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function DeleteCustomerActivity() As ErrorObj
        Const ModuleName As String = "DeleteCustomerActivity"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesHeaderMode = "D"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                RemoveItemFromCache(getCacheKey())
                AddItemToCache(getCacheKey(), ResultDataSet, Settings)
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the question and answer data for a particular customer activity entry
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ActivitiesQNARetrieval()
        Const ModuleName As String = "ActivitiesQNARetrieval"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesHeaderMode = "R"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Search for customer activities
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function CustomerActivitiesSearch() As ErrorObj
        Const ModuleName As String = "CustomerActivitiesSearch"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)
        Dim cacheKey As String = getCacheKey()

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            Dim activities As New DBActivities
            With activities
                .ActivitiesDataEntity = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Create a customer activities comment record
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function CreateActivityComment() As ErrorObj
        Const ModuleName As String = "CreateActivityComment"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesCommentsMode = "C"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Update a customer activities comment record
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function UpdateActivityComment() As ErrorObj
        Const ModuleName As String = "UpdateActivityComment"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesCommentsMode = "U"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Delete a customer activities comment record
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function DeleteActivityComment() As ErrorObj
        Const ModuleName As String = "DeleteActivityComment"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesCommentsMode = "D"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Create a customer activities file attachment record
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function CreateActivityFileAttachment() As ErrorObj
        Const ModuleName As String = "CreateActivityFileAttachment"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesAttachmentsMode = "C"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Delete a customer activities file attachment record
    ''' </summary>
    ''' <returns>Any errors</returns>
    ''' <remarks></remarks>
    Public Function DeleteActivityFileAttachment() As ErrorObj
        Const ModuleName As String = "DeleteActivityFileAttachment"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = DE=" & De.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim activities As New DBActivities
        With activities
            De.CustomerActivitiesAttachmentsMode = "D"
            .ActivitiesDataEntity = De
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Get the customer activities cache key based on data entity properties
    ''' </summary>
    ''' <returns>The formatted customer specific cache key for customer activities</returns>
    ''' <remarks></remarks>
    Private Function getCacheKey() As String
        Dim cacheKey As String = String.Empty
        cacheKey = "CustomerActivities-" & Settings.Company & De.CustomerNumber
        Return cacheKey
    End Function

#End Region

End Class
