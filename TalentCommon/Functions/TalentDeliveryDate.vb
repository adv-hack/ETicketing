Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web

<Serializable()> _
Public Class TalentDeliveryDate
    Inherits TalentBase

    Private _deDelDate As DEDeliveryDate
    Private _resultDataSet As DataSet

    Public Property DeliveryDate() As DEDeliveryDate
        Get
            Return _deDelDate
        End Get
        Set(ByVal value As DEDeliveryDate)
            _deDelDate = value
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


    Public Function GetDeliveryDate(Optional ByVal deliveryZoneCode As String = "", Optional ByVal deliveryZoneType As String = "") As ErrorObj
        Const ModuleName As String = "GetDeliveryDate"
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbDelDate As New DBDeliveryDate
            With dbDelDate
                .deDelDate = DeliveryDate
                .Settings = Settings
                .DeliveryZoneCode = deliveryZoneCode
                .DeliveryZoneType = deliveryZoneType
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        Return err
    End Function

    Public Function GetPreferredDeliveryDates(ByVal deliveryZoneCode As String, ByVal deliveryZoneType As String) As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "GetPreferredDeliveryDates"
        Settings.ModuleName = ModuleName

        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbDelDate As New DBDeliveryDate
            With dbDelDate
                .deDelDate = DeliveryDate
                .Settings = Settings
                .DeliveryZoneCode = deliveryZoneCode
                .DeliveryZoneType = deliveryZoneType
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        Return err
    End Function
End Class
