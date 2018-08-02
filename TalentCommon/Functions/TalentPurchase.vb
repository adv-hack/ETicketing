Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Purchases
'
'       Date                        June 2007
'
'       Author                       
'
'       © CS Group 2006             All rights reserved.
'
'       Error Number Code base      TACTAPU- 
'                                   application.code(3) + object code(4) + number(2)
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentPurchase
    '
    Private _de As New DEPurchaseDetails
    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Public Property De() As DEPurchaseDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DEPurchaseDetails)
            _de = value
        End Set
    End Property
    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Function PurchaseDetails() As ErrorObj
        Const ModuleName As String = "Purchase Details"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Not HttpContext.Current.Cache.Item(cacheKey) Is Nothing Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbPurchase As New DBPurchase
            With dbPurchase
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                End If
            End With
        End If
        Return err
    End Function
End Class
