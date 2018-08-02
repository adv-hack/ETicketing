Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Invoice Requests
'
'       Date                        Mar 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTASI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentSupplier

    Inherits TalentBase

    Private _dep As New DESupplier
    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Public Property Dep() As DESupplier
        Get
            Return _dep
        End Get
        Set(ByVal value As DESupplier)
            _dep = value
        End Set
    End Property
    'Public Property Settings() As DESettings
    '    Get
    '        Return _settings
    '    End Get
    '    Set(ByVal value As DESettings)
    '        _settings = value
    '    End Set
    'End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Function WriteInvoice() As ErrorObj
        Const ModuleName As String = "WriteInvoice"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim DBSupplier As New DBSupplier
            With DBSupplier
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                '
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                End If
            End With
        End If

        Return err
    End Function
    Public Function AcceptInvoice() As ErrorObj
        Const ModuleName As String = "AcceptInvoice"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim DBSupplier As New DBSupplier
            With DBSupplier
                .Dep = Dep
                .Settings = Settings

                err = .ReadInvoiceStatus()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
            End With
        End If

        Return err
    End Function
    Public Function WriteOrderAcknowledgement() As ErrorObj
        Const ModuleName As String = "SupplierOrderAcknowledgement"

        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        ' No cache     
        Dim DBSupplier As New DBSupplier
        With DBSupplier
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            '
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With

        Return err
    End Function

End Class
