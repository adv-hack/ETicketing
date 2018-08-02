Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Text
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentPayment
    Inherits TalentBase

#Region "Class Level Fields"

    Private _de As New DEPayments
    Private _derpay As New DERefundPayment
    Private _deO As New DEOrder
    Private _deID As New DETicketingItemDetails
    Private _dePED As New DEPaymentExternalDetails
    Private _deDelDetails As New DEDeliveryDetails
    Private _resultDataSet As DataSet
    Private _payDetails As Generic.Dictionary(Of String, DEPayments)

#End Region

#Region "Public Properties"

    Public Property De() As DEPayments
        Get
            Return _de
        End Get
        Set(ByVal value As DEPayments)
            _de = value
        End Set
    End Property
    Public Property DePED() As DEPaymentExternalDetails
        Get
            Return _dePED
        End Get
        Set(ByVal value As DEPaymentExternalDetails)
            _dePED = value
        End Set
    End Property
    Public Property DeDelDetails() As DEDeliveryDetails
        Get
            Return _deDelDetails
        End Get
        Set(ByVal value As DEDeliveryDetails)
            _deDelDetails = value
        End Set
    End Property
    Public Property Derpay() As DERefundPayment
        Get
            Return _derpay
        End Get
        Set(ByVal value As DERefundPayment)
            _derpay = value
        End Set
    End Property
    Public Property DeID() As DETicketingItemDetails
        Get
            Return _deID
        End Get
        Set(ByVal value As DETicketingItemDetails)
            _deID = value
        End Set
    End Property
    Public Property DeO() As DEOrder
        Get
            Return _deO
        End Get
        Set(ByVal value As DEOrder)
            _deO = value
        End Set
    End Property
    Public Property PayDetails() As Generic.Dictionary(Of String, DEPayments)
        Get
            Return _payDetails
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, DEPayments))
            _payDetails = value
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
    Public Property RetreiveEPurseCardNumberFromCache As Boolean = False
    Public Property OrderLevelFulfilmentMethod() As String = String.Empty
    Public Property DDScheduleAdjusted() As Boolean = False
    Public Property SendEmail() As Boolean = True

#End Region

#Region "Public Functions"

    Public Function TakePayment() As ErrorObj
        Const ModuleName As String = "TakePayment"

        If DePED Is Nothing Then
            Dim newDePED As New DEPaymentExternalDetails
            Me.DePED = newDePED
        End If

        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPay As New DBPayment
        With dbPay
            .Settings = Settings
            .De = De
            .DePED = DePED
            .DeDelDetails = DeDelDetails
            .PayDetails = PayDetails
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    SendEmail = .SendEmail
                    OrderLevelFulfilmentMethod = .OrderLevelFulfilmentMethod
                    DDScheduleAdjusted = .DDScheduleAdjusted
                End If
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function TakePaymentReadOrder() As ErrorObj
        Const ModuleName As String = "TakePaymentReadOrder"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment
        With dbProduct
            .Settings = Settings
            .De = De
            .DePED = DePED
            .PayDetails = PayDetails
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                'Return the result set
                ResultDataSet = .ResultDataSet
                If .ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" And _
                    .ResultDataSet.Tables.Count > 1 Then
                    'Cache the result set so we can pick the order details up later
                    Dim cacheKey As String = "OrderDetails" & Settings.Company & .ResultDataSet.Tables(0).Rows(0).Item("PaymentReference")
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RefundPayment() As ErrorObj
        Const ModuleName As String = "RefundPayment"
        TalentCommonLog(ModuleName, Derpay.RefundCustomerNo, "Talent.Common Request = Derpay=" & Derpay.LogString)

        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment
        With dbProduct
            .Settings = Settings
            .Derpay = Derpay
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
        TalentCommonLog(ModuleName, Derpay.RefundCustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function CancelAllPayments() As ErrorObj
        Const ModuleName As String = "CancelAllPayments"
        TalentCommonLog(ModuleName, Derpay.RefundCustomerNo, "Talent.Common Request = Derpay=" & Derpay.LogString)

        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment
        With dbProduct
            .Settings = Settings
            .Derpay = Derpay
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
        TalentCommonLog(ModuleName, Derpay.RefundCustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function TakePaymentViaBackend() As ErrorObj
        Const ModuleName As String = "TakePaymentViaBackend"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment
        With dbProduct
            .Settings = Settings
            .De = De
            .DePED = DePED
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
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function PaymentPending() As ErrorObj
        Const ModuleName As String = "PaymentPending"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment
        With dbProduct
            .Settings = Settings
            .De = De
            .DePED = DePED
            .PayDetails = PayDetails
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function DirectDebitDDIRef() As ErrorObj
        Const ModuleName As String = "DirectDebitDDIRef"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.CustomerNumber & De.PaymentStage
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBPayment
            With dbProduct
                .Settings = Settings
                .De = De
                .DePED = DePED
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function DirectDebitPaymentDays() As ErrorObj
        Const ModuleName As String = "DirectDebitPaymentDays"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.CustomerNumber & De.PaymentStage
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBPayment
            With dbProduct
                .Settings = Settings
                .De = De
                .DePED = DePED
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function DirectDebitSummary() As ErrorObj
        Const ModuleName As String = "DirectDebitSummary"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment
        With dbProduct
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function GenerateTransactionID() As ErrorObj
        Const ModuleName As String = "GenerateTransactionID"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBPayment

        With dbProduct
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveCashback() As ErrorObj
        Const ModuleName As String = "RetrieveCashback"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.CustomerNumber
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function UpdateCashback() As ErrorObj
        Const ModuleName As String = "UpdateCashback"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = "RetrieveCashback" & Settings.Company & De.CustomerNumber
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    RemoveItemFromCache(cacheKey)
                End If
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function UpdateAndRetrievePaymentCharges() As ErrorObj
        Const ModuleName As String = "UpdateAndRetrievePaymentCharges"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveAndUpdateAdHocFees() As ErrorObj
        Const ModuleName As String = "RetrieveAndUpdateAdHocFees"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        Else
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)

                    'Update tbl_adhoc_fees only when a TALENT call has been made
                    'Doing the following function within a user control means additional uncessary SQL updates.
                    If ResultDataSet.Tables.Count = 2 Then
                        If ResultDataSet.Tables(0).Rows.Count = 0 Then
                            If ResultDataSet.Tables(1).Rows.Count > 0 Then
                                Dim tDataObjects = New TalentDataObjects()
                                Dim affectedRows As Integer = 0
                                tDataObjects.Settings = Settings
                                tDataObjects.Settings.Cacheing = False
                                tDataObjects.Settings.DestinationDatabase = "SQL2005"
                                affectedRows = tDataObjects.PaymentSettings.TblAdhocFees.RefreshAdhocFeesFromTalent( _
                                    ResultDataSet.Tables(1), Settings.BusinessUnit)
                            End If
                        End If
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveMySavedCards() As ErrorObj
        Const ModuleName As String = "RetrieveMySavedCards"
        Const RetrieveMode As String = "R"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.CustomerNumber
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                De.SaveMyCardMode = RetrieveMode
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function SaveMyCard() As ErrorObj
        Const ModuleName As String = "SaveMyCard"
        Const SaveMode As String = "S"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            De.SaveMyCardMode = SaveMode
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function DeleteMyCard() As ErrorObj
        Const ModuleName As String = "DeleteMyCard"
        Const DeleteMode As String = "D"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            De.SaveMyCardMode = DeleteMode
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function SetMyCardAsDefault() As ErrorObj
        Const ModuleName As String = "SetMyCardAsDefault"
        Const DefaultMode As String = "A"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString & ", DePED=" & DePED.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            De.SaveMyCardMode = DefaultMode
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function


    Public Function OnAccountAdjustment() As ErrorObj
        Const ModuleName As String = "OnAccountAdjustment"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            De.CashbackMode = "A"
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Sub RetrieveMySavedCardsClearCache()
        Const ModuleName As String = "RetrieveMySavedCards"
        Dim cacheKey As String = ModuleName & Settings.Company & De.CustomerNumber
        HttpContext.Current.Cache.Remove(cacheKey)
    End Sub




    Public Function RetrieveEPurseTotal() As ErrorObj
        Const ModuleName As String = "RetrieveEPurseTotal"
        TalentCommonLog(ModuleName, String.Empty, "Talent.Common Request = De=" & De.CardNumber)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim cacheKeyBuilder As New StringBuilder
        cacheKeyBuilder.Append(ModuleName).Append("-").Append(De.CustomerNumber)
        Dim cacheKey As String = cacheKeyBuilder.ToString()
        Dim cacheKey2 As String = cacheKeyBuilder.Append("-Card").ToString()

        If Settings.Cacheing AndAlso TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            'The card number is not part of the result set so it has it's own cache key
            If RetreiveEPurseCardNumberFromCache AndAlso TalentThreadSafe.ItemIsInCache(cacheKey2) Then
                De.CardNumber = CType(HttpContext.Current.Cache.Item(cacheKey2), String)
            End If
        Else
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    If De.CardNumber.Length > 0 Then AddItemToCache(cacheKey2, De.CardNumber, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, String.Empty, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveVariablePricedProducts() As ErrorObj
        Const ModuleName As String = "RetrieveVariablePricedProducts"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, "", "Talent.Common Request = RetrieveVariablePricedProducts")
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim cacheKey As String = ModuleName + "type=" + De.variablePricedProductType
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function


    Public Function RetrieveOnAccountDetails() As ErrorObj
        Const ModuleName As String = "RetrieveOnAccountDetails"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.CustomerNumber)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim cacheKey As String = RetrieveOnAccountDetailsCacheKey()
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveOnAccountDetailsClearCache() As ErrorObj
        Dim cacheKey As String = RetrieveOnAccountDetailsCacheKey()
        HttpContext.Current.Cache.Remove(cacheKey)
        Return New ErrorObj
    End Function

    Public Function RetrieveOnAccountDetailsCacheKey() As String
        Return "RetrieveOnAccountDetails" & De.CustomerNumber
    End Function

    Public Function TakePartPayment() As ErrorObj
        Const ModuleName As String = "TakePartPayment"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet

                'Remove the part payment cache for this customer as we should refresh the part payment section even if the call has failed.
                RetrievePartPaymentsClearSession()

                'Add the part payment section to cache when the call was successful
                If ResultDataSet.Tables.Count > 1 Then
                    AddItemToSession(RetrievePartPaymentsSessionKey, ResultDataSet)
                End If
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function CancelPartPayment() As ErrorObj
        Const ModuleName As String = "CancelPartPayment"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                'Return the result set
                ResultDataSet = .ResultDataSet

                'Remove the part payment cache for this customer as we should refresh the part payment section even if the call has failed.
                RetrievePartPaymentsClearSession()

                'Add the part payment section to cache when the call was successful
                If ResultDataSet.Tables.Count > 1 Then
                    AddItemToSession(RetrievePartPaymentsSessionKey, ResultDataSet)
                End If
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrievePartPayments() As ErrorObj
        Const ModuleName As String = "RetrievePartPayments"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        Dim err As New ErrorObj

        If De.CustomerNumber Is Nothing OrElse De.CustomerNumber.Length > 12 Then De.CustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        Dim sessionKey As String = RetrievePartPaymentsSessionKey()
        If Utilities.IsSessionActive AndAlso HttpContext.Current.Session.Item(sessionKey) IsNot Nothing Then
            ResultDataSet = CType(HttpContext.Current.Session.Item(sessionKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbPayment As New DBPayment
            With dbPayment
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToSession(sessionKey, ResultDataSet)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrievePartPaymentsSessionKey() As String
        Dim cacheKey As String = "RetrievePartPayments-BasketId=" & De.SessionId & "CustomerId=" & De.CustomerNumber
        Return cacheKey
    End Function

    Public Sub RetrievePartPaymentsClearSession()
        Dim sessionKey As String = RetrievePartPaymentsSessionKey()
        If Utilities.IsSessionActive AndAlso HttpContext.Current.Session.Item(sessionKey) IsNot Nothing Then
            HttpContext.Current.Session.Remove(sessionKey)
        End If
    End Sub

    ''' <summary>
    ''' Function called at the end of a sale to action any customer updates
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function OrderCompletionUpdates() As ErrorObj
        Const ModuleName As String = "OrderCompletionUpdates"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbPayment As New DBPayment
        With dbPayment
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

#End Region

End Class