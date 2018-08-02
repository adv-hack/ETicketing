Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentReservations
    Inherits TalentBase
    Public Property DataEntity() As DEReservations
    Public Property ResultDataSet() As DataSet
    ''' <summary>
    ''' Returns list of all customer reservations
    ''' </summary>
    ''' <returns>List of customer reservations</returns>
    ''' <remarks></remarks>
    Public Function RetrieveCustomerReservations() As ErrorObj
        Const ModuleName As String = "RetrieveCustomerReservations"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Reserve all items in basket
    ''' </summary>
    ''' <returns>Reservation reference and/or error details</returns>
    ''' <remarks></remarks>
    Public Function ReserveAllBasketItems() As ErrorObj
        Const ModuleName As String = "ReserveAllBasketItems"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Reserve all items in basket and return basket
    ''' </summary>
    ''' <returns>Reservation reference and/or error details</returns>
    ''' <remarks></remarks>
    Public Function ReserveAllBasketItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "ReserveAllBasketItemsReturnBasket"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            err = ProcessNewBasket(DataEntity.Source, DataEntity.SessionID, ResultDataSet)
        End With
        Return err
    End Function
    ''' <summary>
    ''' Unreserve all items in basket 
    ''' </summary>
    ''' <returns>Number of affected items and error details</returns>
    ''' <remarks></remarks>
    Public Function UnreserveAllBasketItems() As ErrorObj
        Const ModuleName As String = "UnreserveAllBasketItems"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Unreserve all items in basket and return basket
    ''' </summary>
    ''' <returns>Number of affected items and error details</returns>
    ''' <remarks></remarks>
    Public Function UnreserveAllBasketItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "UnreserveAllBasketItemsReturnBasket"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            err = ProcessNewBasket(DataEntity.Source, DataEntity.SessionID, ResultDataSet)

        End With
        Return err
    End Function
    ''' <summary>
    ''' Unreserves seperate basket items
    ''' </summary>
    ''' <returns>Number of affected items and error details</returns>
    ''' <remarks></remarks>
    Public Function UnreserveSeparateBasketItems() As ErrorObj
        Const ModuleName As String = "UnreserveSeparateBasketItems"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Unreserves seperate basket items and return basket
    ''' </summary>
    ''' <returns>Number of affected items and error details</returns>
    ''' <remarks></remarks>
    Public Function UnreserveSeparateBasketItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "UnreserveSeparateBasketItemsReturnBasket"
        Dim err As New ErrorObj

        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            err = ProcessNewBasket(DataEntity.Source, DataEntity.SessionID, ResultDataSet)
        End With
        Return err
    End Function
    ''' <summary>
    ''' Add reservations to basket and returns updated basket
    ''' </summary>
    ''' <returns>Updated basket</returns>
    ''' <remarks></remarks>
    Public Function AddReservationToBasketReturnBasket() As ErrorObj
        Const ModuleName As String = "AddReservationToBasketReturnBasket"
        TalentCommonLog(ModuleName, DataEntity.CustomerNumber, "Talent.Common Request")
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Dim err As New ErrorObj

        Settings.ModuleName = ModuleName
        Dim dbReservations As New DBReservations
        With dbReservations
            .Settings = Settings
            .DeReservations = DataEntity
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            err = ProcessNewBasket(DataEntity.Source, DataEntity.SessionID, ResultDataSet)
        End With

        TalentCommonLog(ModuleName, DataEntity.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

End Class
