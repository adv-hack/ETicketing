Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Price Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentPricing
    Inherits TalentBase

    Private _de As New DEProductDetails
    Private _dep As New DePNA
    Private _depnarequest As New DEPNARequest
    Private _settings As New DESettings
    Private _stockSettings As New DESettings
    Private _resultDataSet As DataSet
    'Private _dicPriceCodeDetails As Generic.Dictionary(Of String, String) = Nothing

    'Public ReadOnly Property PriceCodeDesc() As Generic.Dictionary(Of String, String)
    '    Get
    '        Return _dicPriceCodeDetails
    '    End Get
    'End Property


    Public Property De() As DEProductDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DEProductDetails)
            _de = value
        End Set
    End Property

    Public Property Dep() As DePNA
        Get
            Return _dep
        End Get
        Set(ByVal value As DePNA)
            _dep = value
        End Set
    End Property
    Public Property Depnarequest() As DEPNARequest
        Get
            Return _depnarequest
        End Get
        Set(ByVal value As DEPNARequest)
            _depnarequest = value
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
    Public Property StockSettings() As DESettings
        Get
            Return _stockSettings
        End Get
        Set(ByVal value As DESettings)
            _stockSettings = value
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

    Public Function GetSinglePrice() As ErrorObj
        Const ModuleName As String = "GetSinglePrice"
        Dim err As New ErrorObj
        ''--------------------------------------------------------------------------
        'Dim cacheKey As String = ModuleName & Settings.Company
        'If Settings.Cacheing And Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
        '    ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        'Else
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim dbPricing As New DBPricing
        With dbPricing
            .Depnarequest = Depnarequest
            .dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                'AddItemToCache(cacheKey, ResultDataSet, Settings)
                ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    'AddItemToCache(cacheKey, ResultDataSet, Settings)
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
            End If
        End With
        'End If
        Return err
    End Function

    Public Function PnaRequest() As ErrorObj
        Const ModuleName As String = "PnaRequest"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else

            Dim dbPricing As New DBPricing
            With dbPricing
                .Depnarequest = Depnarequest
                .dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    '  HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase
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

    Public Function ProductPriceCodeDescriptions(ByVal dtProductPriceCodes As DataTable, ByVal dtCampaignPriceCodes As DataTable) As ErrorObj
        Const ModuleName As String = "PriceCodeDescriptions"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & De.StadiumCode & De.ProductType & De.ProductCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            err = PriceCodeDetails()
            If Not err.HasError AndAlso TalDataSet IsNot Nothing Then
                If TalDataSet.ResultDataSet IsNot Nothing AndAlso TalDataSet.ResultDataSet.Tables("Status").Rows.Count > 0 AndAlso TalDataSet.ResultDataSet.Tables("Status").Rows(0)(0) = "" Then
                    ResultDataSet = TalDataSet.ResultDataSet.Copy
                    ResultDataSet.Tables.Add(PopulateDtProductPriceCode(TalDataSet.DictionaryOfPriceCodes, dtProductPriceCodes, dtCampaignPriceCodes))
                    'Price Codes should be associated with Productlist cache
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    ResultDataSet = Nothing
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            Else
                Me.ResultDataSet = Nothing
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End If
        Return err
    End Function

    Public Function StadiumPriceCodeDescriptions() As ErrorObj
        Const ModuleName As String = "StadiumPriceCodeDescriptions"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & De.StadiumCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            err = PriceCodeDetails()
            If Not err.HasError AndAlso TalDataSet IsNot Nothing Then
                If TalDataSet.ResultDataSet IsNot Nothing AndAlso TalDataSet.ResultDataSet.Tables("Status").Rows.Count > 0 AndAlso TalDataSet.ResultDataSet.Tables("Status").Rows(0)(0) = "" Then
                    ResultDataSet = TalDataSet.ResultDataSet.Copy
                    ResultDataSet.Tables.Add(PopulateDtStadiumPriceCode(TalDataSet.DictionaryOfPriceCodes))
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    ResultDataSet = Nothing
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            Else
                Me.ResultDataSet = Nothing
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End If
        Return err
    End Function

    Public Function PriceCodeDetails() As ErrorObj
        Const ModuleName As String = "PriceCodeDetails"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & De.StadiumCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            TalDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), TalentDataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbPricing As New DBPricing
            With dbPricing
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    'ResultDataSet = .ResultDataSet
                    '_dicPriceCodeDetails = .PriceCodeDesc
                    TalDataSet.ResultDataSet = .ResultDataSet
                    TalDataSet.DictionaryOfPriceCodes = .PriceCodeDesc
                    'Price Codes should be associated with Productlist cache
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, TalDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        'ResultDataSet = .ResultDataSet
                        '_dicPriceCodeDetails = .PriceCodeDesc
                        TalDataSet.ResultDataSet = .ResultDataSet
                        TalDataSet.DictionaryOfPriceCodes = .PriceCodeDesc
                        AddItemToCache(cacheKey, TalDataSet, Settings)
                    End If
                End If
            End With
        End If
        Return err
    End Function

    Private Function PopulateDtProductPriceCode(ByVal dicPriceCodeDesc As Generic.Dictionary(Of String, DEPriceCode), ByVal dtProductPriceCodes As DataTable, ByVal dtCampaignPriceCodes As DataTable) As DataTable
        Dim dRow As DataRow = Nothing
        Dim dtProductPriceCode As New DataTable("ProductPriceCodes")
        Dim tempPriceCode As DEPriceCode = New DEPriceCode("", "", "", False)
        If dtProductPriceCodes.Rows.Count > 0 Then
            If (dicPriceCodeDesc IsNot Nothing) AndAlso (dicPriceCodeDesc.Count > 0) Then
                With dtProductPriceCode.Columns
                    .Add("StadiumCode", GetType(String))
                    .Add("ProductCode", GetType(String))
                    .Add("ProductType", GetType(String))
                    .Add("PriceCode", GetType(String))
                    .Add("PriceCodeDescription", GetType(String))
                    .Add("PriceCodeLongDescription", GetType(String))
                    .Add("FreeOfCharge", GetType(String))
                End With
                Dim productPriceCodes As String = String.Empty
                For rowIndex As Integer = 0 To dtProductPriceCodes.Rows.Count - 1
                    If dicPriceCodeDesc.TryGetValue(dtProductPriceCodes.Rows(rowIndex)("PriceCode"), tempPriceCode) _
                        AndAlso Not IsPriceCodeAlsoCampaignCode(dtProductPriceCodes.Rows(rowIndex)("PriceCode"), dtCampaignPriceCodes) Then
                        dRow = Nothing
                        dRow = dtProductPriceCode.NewRow
                        dRow("ProductCode") = De.ProductCode
                        dRow("ProductType") = De.ProductType
                        dRow("StadiumCode") = De.StadiumCode
                        dRow("PriceCode") = dtProductPriceCodes.Rows(rowIndex)("PriceCode")
                        dRow("PriceCodeDescription") = tempPriceCode.ShortDescription
                        dRow("PriceCodeLongDescription") = tempPriceCode.LongDescription
                        dRow("FreeOfCharge") = tempPriceCode.FreeOfCharge.ToString
                        dtProductPriceCode.Rows.Add(dRow)
                    End If
                Next
            End If
        End If
        Return dtProductPriceCode
    End Function

    Private Function PopulateDtStadiumPriceCode(ByVal dicPriceCodeDesc As Generic.Dictionary(Of String, DEPriceCode)) As DataTable
        Dim dRow As DataRow = Nothing
        Dim dtStadiumPriceCodes As New DataTable("StadiumPriceCodes")


        If (dicPriceCodeDesc IsNot Nothing) AndAlso (dicPriceCodeDesc.Count > 0) Then
            With dtStadiumPriceCodes.Columns
                .Add("StadiumCode", GetType(String))
                .Add("PriceCode", GetType(String))
                .Add("PriceCodeDescription", GetType(String))
                .Add("PriceCodeLongDescription", GetType(String))
                .Add("FreeOfCharge", GetType(String))
            End With
            Dim productPriceCodes As String = String.Empty
            For Each kvp As KeyValuePair(Of String, DEPriceCode) In dicPriceCodeDesc
                Dim tempPriceCode As DEPriceCode = kvp.Value
                dRow = Nothing
                dRow = dtStadiumPriceCodes.NewRow
                dRow("StadiumCode") = De.StadiumCode
                dRow("PriceCode") = tempPriceCode.PriceCode
                dRow("PriceCodeDescription") = tempPriceCode.ShortDescription
                dRow("PriceCodeLongDescription") = tempPriceCode.LongDescription
                dRow("FreeOfCharge") = tempPriceCode.FreeOfCharge
                dtStadiumPriceCodes.Rows.Add(dRow)
            Next
        End If

        Return dtStadiumPriceCodes
    End Function

    Private Function IsPriceCodeAlsoCampaignCode(ByVal productPriceCode As String, ByVal dtCampaignPriceCodes As DataTable) As Boolean
        Dim isCampaignCode As Boolean = False
        If dtCampaignPriceCodes.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To dtCampaignPriceCodes.Rows.Count - 1
                If dtCampaignPriceCodes.Rows(rowIndex)("CampaignCode") = productPriceCode Then
                    isCampaignCode = True
                    Exit For
                End If
            Next
        End If
        Return isCampaignCode
    End Function

End Class
