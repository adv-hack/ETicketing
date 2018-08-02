Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Price Requests
'
'       Date                        Dec 2006
'
'       Author                       
'
'       � CS Group 2006             All rights reserved.
'
'       Error Number Code base      TACTAST- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentStock
    Inherits TalentBase

    Private _dep As New DePNA
    Private _settings As New DESettings
    Private _resultDataSet As DataSet


    Public Property Dep() As DePNA
        Get
            Return _dep
        End Get
        Set(ByVal value As DePNA)
            _dep = value
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


    Public Function GetSingleStock() As ErrorObj
        Const ModuleName As String = "GetSingleStock"

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbStock As New DBStock
            With dbStock
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    If Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                    If ResultDataSet Is Nothing Then
                        err = .AccessDatabase
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                            ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                        End If
                    End If
                End If
            End With
        End If
        Return err
    End Function
    Public Function GetMutlipleStock() As ErrorObj
        'Const ModuleName As String = "GetMutlipleStock"
        '! - 
        Const ModuleName As String = "GetMultipleStock"

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Select Case Settings.DestinationType.ToUpper
                Case "XML"
                    Dim xmlMultiAvail As New XmlMultipleAvailability
                    With xmlMultiAvail
                        .DePriceNAvailability = Dep
                        .Settings = Settings
                        err = .GetMultipleStock()
                        ResultDataSet = .ResultDataSet
                    End With
                Case Else
                    Dim DBMultipleAvailability As New DBMultipleAvailability
                    With DBMultipleAvailability
                        .Depa = Dep
                        .Usage = 1
                        .Settings = Settings
                        err = .ValidateAgainstDatabase()
                        If Not err.HasError Then
                            If Not .ResultDataSet Is Nothing Then
                                ResultDataSet = .ResultDataSet
                                AddItemToCache(cacheKey, ResultDataSet, Settings)
                                '  HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                            End If
                            If ResultDataSet Is Nothing Then
                                err = .AccessDatabase
                                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                                    ResultDataSet = .ResultDataSet
                                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                                End If
                            End If
                        End If
                    End With
            End Select
        End If

        Return err
    End Function

    'Public Function GetCompleteStock() As ErrorObj
    '    Const ModuleName As String = "GetCompleteStock"
    '    Dim err As New ErrorObj
    '    '--------------------------------------------------------------------------
    '    Dim cacheKey As String = ModuleName & Settings.Company
    '    If Settings.Cacheing And Not HttpContext.Current.Cache.Item(cacheKey) Is Nothing Then
    '        ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
    '    Else
    '        Dim DBMultipleAvailability As New DBMultipleAvailability
    '        With DBMultipleAvailability
    '            .Depa = Dep
    '            .Usage = 1
    '            .Settings = Settings
    '            err = .ValidateAgainstDatabase()
    '            If Not err.HasError Then
    '                If Not .ResultDataSet Is Nothing Then
    '                    ResultDataSet = .ResultDataSet
    '                    HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
    '                End If
    '                If ResultDataSet Is Nothing Then
    '                    err = .AccessDatabase
    '                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
    '                        ResultDataSet = .ResultDataSet
    '                        HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
    '                    End If
    '                End If
    '            End If
    '        End With
    '    End If
    '    Return err
    'End Function
End Class
