Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web

<Serializable()>
Public Class TalentTicketExchange
    Inherits TalentBase
    '
    Private _dep As New DETicketExchange
    Private _resultDataSet As DataSet
    Public Property Dep() As DETicketExchange
        Get
            Return _dep
        End Get
        Set(ByVal value As DETicketExchange)
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

    Public Function TicketExchangeEnquiry() As ErrorObj
        Const ModuleName As String = "TicketExchangeEnquiry"
        Dim err As New ErrorObj
        
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBTicketExchange As New DBTicketExchange
        With DBTicketExchange
            .Dep = Dep
            Settings.ModuleName = ModuleName
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

    Public Function GetTicketExchangeProductsListForCustomer() As ErrorObj
        Const ModuleName As String = "GetTicketExchangeProductsListForCustomer"
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBTicketExchange As New DBTicketExchange
        With DBTicketExchange
            .Dep = Dep
            Settings.ModuleName = ModuleName
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

    Public Function GetTicketExchangeSeatSelectionForProduct() As ErrorObj
        Const ModuleName As String = "GetTicketExchangeSeatSelectionForProduct"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBTicketExchange As New DBTicketExchange
        With DBTicketExchange
            .Dep = Dep
            Settings.ModuleName = ModuleName
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

    Public Function SubmitTicketExchangeAction() As ErrorObj
        Const ModuleName As String = "SubmitTicketExchangeAction"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBTicketExchange As New DBTicketExchange
        With DBTicketExchange
            .Settings = Settings
            .Dep = Dep
            err = .ValidateAgainstDatabase
            If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                err = Nothing
            Else
                Dim intPos As Integer = 0
                Dim intCount As Integer = 0
                If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                    intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                    If intPos > 0 Then
                        intCount = 1
                        Do While intCount <= Settings.RetryAttempts And err.HasError
                            If Settings.RetryWaitTime > 0 Then
                                System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                            End If
                            err.ErrorMessage = ""
                            err.ErrorStatus = ""
                            err.ErrorNumber = ""
                            err.HasError = False
                            .ResetConnection = True
                            err = .AccessDatabase
                            intCount += 1
                        Loop
                    End If
                End If
            End If
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

        ' Clear visual seating info key for new prices.
        Dim distinctCacheKey As New List(Of String)
        Dim cacheKey As String
        For Each item As DETicketExchangeItem In Dep.TicketExchangeItems
            cacheKey = "PriceBreakSeatDetails" & item.ProductCode & item.SeatDetails.Stand & item.SeatDetails.Area
            If Not distinctCacheKey.Contains(cacheKey) Then
                distinctCacheKey.Add(cacheKey)
                If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    CacheUtility.RemoveItemFromCache(cacheKey)
                End If
            End If
        Next

        Return err
    End Function
    Public Function GetTicketExchangeDefaults() As ErrorObj
        Const ModuleName As String = "GetTicketExchangeDefaults"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & Dep.Customer & Dep.ProductCode & Settings.CacheStringExtension

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBTicketExchange As New DBTicketExchange
            With DBTicketExchange
                .Dep = Dep
                Settings.ModuleName = ModuleName
                .Settings = Settings
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
    Public Function UpdateTicketExchangeDefaults() As ErrorObj
        Const ModuleName As String = "UpdateTicketExchangeDefaults"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & Settings.CacheStringExtension

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim DBTicketExchange As New DBTicketExchange
            With DBTicketExchange
                .Settings = Settings
                .Dep = Dep
                err = .ValidateAgainstDatabase
                If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                    err = Nothing
                Else
                    Dim intPos As Integer = 0
                    Dim intCount As Integer = 0
                    If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                        intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                        If intPos > 0 Then
                            intCount = 1
                            Do While intCount <= Settings.RetryAttempts And err.HasError
                                If Settings.RetryWaitTime > 0 Then
                                    System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                                End If
                                err.ErrorMessage = ""
                                err.ErrorStatus = ""
                                err.ErrorNumber = ""
                                err.HasError = False
                                .ResetConnection = True
                                err = .AccessDatabase
                                intCount += 1
                            Loop
                        End If
                    End If
                End If
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

    Public Function OrderReturnEnquiry() As ErrorObj
        Const ModuleName As String = "OrderReturnEnquiry"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & Dep.TicketExchangeReference & Settings.CacheStringExtension

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBTicketExchange As New DBTicketExchange
            With DBTicketExchange
                .Dep = Dep
                Settings.ModuleName = ModuleName
                .Settings = Settings
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

    Public Function OrderReturn() As ErrorObj
        Const ModuleName As String = "OrderReturn"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & Settings.CacheStringExtension

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim DBTicketExchange As New DBTicketExchange
            With DBTicketExchange
                .Settings = Settings
                .Dep = Dep
                err = .AccessDatabase
                If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                    err = Nothing
                Else
                    Dim intPos As Integer = 0
                    Dim intCount As Integer = 0
                    If err.HasError And Settings.RetryFailures And Settings.RetryAttempts > 0 Then
                        intPos = InStr(Settings.RetryErrorNumbers, err.ErrorNumber.Trim)
                        If intPos > 0 Then
                            intCount = 1
                            Do While intCount <= Settings.RetryAttempts And err.HasError
                                If Settings.RetryWaitTime > 0 Then
                                    System.Threading.Thread.Sleep(Settings.RetryWaitTime)
                                End If
                                err.ErrorMessage = ""
                                err.ErrorStatus = ""
                                err.ErrorNumber = ""
                                err.HasError = False
                                .ResetConnection = True
                                err = .AccessDatabase()
                                intCount += 1
                            Loop
                        End If
                    End If
                End If
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
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
