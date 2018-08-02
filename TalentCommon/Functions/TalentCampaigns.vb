Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Products
'
'       Date                        8th Jan 2008
'
'       Author                      Ben Ford
'
'       � CS Group 2006             All rights reserved.
'
'       Error Number Code base      - 
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentCampaigns
    Inherits TalentBase
    '
    Private _de As New DECampaignDetails
    '    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Public Property De() As DECampaignDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DECampaignDetails)
            _de = value
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

    '-------------------------------------------------------------
    ' WriteCampaign - Write a campaign to the backend CRM database
    ' (initially for Simon Jersey catalog request)
    ' NO LONGER USED!
    '-------------------------------------------------------------
    Public Function WriteCampaign() As ErrorObj
        Const ModuleName As String = "WriteCampaign"

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCampaign As New DBCampaign
            With dbCampaign
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
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

End Class
