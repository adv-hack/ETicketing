Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Prompts 
'                                   (initially from Note type 11)
'
'       Date                        11/02/08
'
'       Author                       Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentPrompt
    Inherits TalentBase

    Private _de As New DEPrompt
    Private _resultDataSet As DataSet
    Public Property De() As DEPrompt
        Get
            Return _de
        End Get
        Set(ByVal value As DEPrompt)
            _de = value
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
    Public Function GetUserPrompt() As ErrorObj
        Const ModuleName As String = "GetUserPrompt"

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbUserPrompt As New DBUserPrompt
            With dbUserPrompt
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    '  HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
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
    Public Function GetDepartmentPrompt() As ErrorObj
        Const ModuleName As String = "GetDepartmentPrompt"

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbDepartmentPrompt As New DBDepartmentPrompt
            With dbDepartmentPrompt
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

                    End If
                End If
            End With
        End If
        Return err
    End Function

End Class
