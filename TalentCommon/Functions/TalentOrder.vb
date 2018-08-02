Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAOR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentOrder
    Inherits TalentBase
    '
    Private _dep As New DEOrder
    Private _resultDataSet As DataSet
    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
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
    Public Property DeAmendTicketingOrder As DEAmendTicketingOrder

    ''' <summary>
    ''' Get the order details and pass it to web service or create xml or to database
    ''' based on the destinationtype of settings object 
    ''' </summary>
    ''' <returns>Error Object</returns>
    Public Function Create() As ErrorObj
        Const ModuleName As String = "CreateOrder"

        Dim err As New ErrorObj
        Dim err2 As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type of cache, Company name and all relevent 
        '   incoming unique keys. If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database.
        '
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Select Case Settings.DestinationType.ToUpper
                Case "XML"

                    Dim xmlOrder As New XmlOrder
                    With xmlOrder
                        .Dep = Dep
                        .Settings = Settings
                        err = .WriteToXML()
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                        End If
                    End With
                  

                Case Else
                    Dim dbOrder As New DBOrder
                    With dbOrder
                        .Dep = Dep
                        .Settings = Settings
                        err = .ValidateAgainstDatabase()
                        If Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                            ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                        End If
                        If Not err.HasError And ResultDataSet Is Nothing Then
                            err = .WriteToDatabase()

                            If Not err.HasError AndAlso Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE Then
                                Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, "CreateOrder2")
                                err2 = .WriteToDatabase()
                            End If
                            '
                            ' Start Ignore certain errors
                            If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                                err = Nothing
                            Else
                                '
                                ' Start RETRY
                                '
                                ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                                ' error number generated is defined to be retried then ...
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
                                            err = .WriteToDatabase()
                                            intCount += 1
                                        Loop
                                    End If
                                End If
                                ' End   RETRY
                                '
                            End If
                            ' End Ignore certain errors
                            '
                            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                                ResultDataSet = .ResultDataSet
                                AddItemToCache(cacheKey, ResultDataSet, Settings)
                                ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                            End If
                        End If
                    End With
            End Select


        End If
        Return err
    End Function
    Public Function Update() As ErrorObj
        Const ModuleName As String = "UpdateOrder"

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
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBOrderChange As New DBOrderChange
            With DBOrderChange
                .Dep = Dep
                .Settings = Settings
                .AccessDatabase()
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then _
                    ResultDataSet = .ResultDataSet
                If Not err.HasError And ResultDataSet Is Nothing Then
                    '      err = .AccessDatabase()
                    If Not err.HasError And Not ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                End If
            End With
        End If
        Return err
    End Function
    Public Function OrderDetails() As ErrorObj
        Const ModuleName As String = "OrderDetails"
        Dim err As New ErrorObj

        '--------------------------------------------------------------------------
        Dim cacheKey As String = OrderDetailsCacheKey()
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim DBOrderDetail As New DBOrderDetail
            With DBOrderDetail
                .Settings = Settings
                .Dep = Dep
                err = .ReadOrder
                '
                ' Start Ignore certain errors
                If err.HasError And InStr(Settings.IgnoreErrors, err.ErrorNumber.Trim) > 0 Then
                    err = Nothing
                Else
                    '
                    ' Start RETRY
                    '
                    ' If there is an error AND Retry is switched AND RetryAttempts is > 0 AND the
                    ' error number generated is defined to be retried then ...
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
                                err = .ReadOrder()
                                intCount += 1
                            Loop
                        End If
                    End If
                    ' End   RETRY
                    '
                End If
                ' End Ignore certain errors
                '
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
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

    Public Function OrderDetailsClearCache() As ErrorObj
        HttpContext.Current.Cache.Remove(OrderDetailsCacheKey)
        Return New ErrorObj
    End Function

    Private Function OrderDetailsCacheKey() As String
        Return "OrderDetails" & Settings.Company & Settings.CacheStringExtension
    End Function

    Public Function OrderEnquiryDetails() As ErrorObj
        Const ModuleName As String = "OrderEnquiryDetail"
        Dim err As New ErrorObj
        Dim cacheKey As String = OrderEnquiryDetailsCacheKey()

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBOrderEnquiryDetail As New DBOrderEnquiryDetail
            With DBOrderEnquiryDetail
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

    Public Function OrderProductsSpecificText() As ErrorObj
        Const ModuleName As String = "OrderProductsSpecificText"
        Dim err As New ErrorObj
        Dim cacheKey As String = OrderProductsSpecificTextCacheKey()

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBOrderDetail As New DBOrderDetail
            With DBOrderDetail
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

    Public Function GetBulkSeats() As ErrorObj
        Const ModuleName As String = "GetBulkSeats"
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBOrderDetail As New DBOrderDetail
            With DBOrderDetail
                .Dep = Dep
                Settings.ModuleName = ModuleName
                .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With


        Return err
    End Function

    Public Function GetComponentBulkSeats() As ErrorObj
        Const ModuleName As String = "GetComponentBulkSeats"
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBOrderDetail As New DBOrderDetail
        With DBOrderDetail
            .Dep = Dep
            Settings.ModuleName = ModuleName
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With

        Return err
    End Function

    Public Function OrderStatus() As ErrorObj
        Const ModuleName As String = "OrderStatus"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DbOrderStatus As New DBOrderStatus
            With DbOrderStatus
                .Settings = Settings
                .Dep = Dep
                err = .ReadOrder
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
                                err = .ReadOrder()
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
    Public Function OrderTracking() As ErrorObj
        Const ModuleName As String = "Order Tracking"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim DbOrderTracking As New DBOrderTracking
            With DbOrderTracking
                .Settings = Settings
                .Dep = Dep
                err = .ReadOrder
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
                                err = .ReadOrder()
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

    Public Function DispatchAdvice() As ErrorObj
        Const ModuleName As String = "DispatchAdvice"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim DBDispatchAdvice As New DBDispatchAdvice
            With DBDispatchAdvice
                .Settings = Settings
                .Dep = Dep
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
                        HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                End If
            End With
        End If

        Return err
    End Function

    Public Function RMAStatus() As ErrorObj
        Const ModuleName As String = "RMAStatus"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim DBRMAStatus As New DBRMAStatus
            With DBRMAStatus
                .Settings = Settings
                .Dep = Dep
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

    Public Function RetrieveSCOrderDetails()
        Const ModuleName As String = "RetrieveSCOrderDetails"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim DBOrderDetail As New DBOrderDetail
            With DBOrderDetail
                .Settings = Settings
                .Dep = Dep
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
    Public Function TransactionEnquiryDetails()
        Const ModuleName As String = "TransactionEnquiryDetails"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBOrderEnq As New DBOrderEnquiryDetail
        With DBOrderEnq
            .Settings = Settings
            .Dep = Dep
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

    Public Function OrderEnquiryDetailsClearCache() As ErrorObj
        HttpContext.Current.Cache.Remove(OrderEnquiryDetailsCacheKey)
        Return New ErrorObj
    End Function

    Private Function OrderEnquiryDetailsCacheKey() As String
        Return "OrderEnquiryDetail" & Settings.Company & Settings.AccountNo1
    End Function

    Private Function OrderProductsSpecificTextCacheKey() As String
        Dim key As String = "OrderProductsSpecificText" & Settings.Company & Settings.BusinessUnit & Settings.OriginatingSourceCode
        If Dep.CollDEOrders IsNot Nothing AndAlso Dep.CollDEOrders.Count > 0 Then
            Dim ticketingItem As DETicketingItemDetails = Dep.CollDEOrders.Item(1)
            key &= ticketingItem.PaymentReference
        End If
        Return (key)
    End Function
   

    Public Function AmendTicketingOrder()
        Const ModuleName As String = "AmendTicketingOrder"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBOrderChg As New DBOrderChange
        With DBOrderChg
            .Settings = Settings
            .DeAmendTicketingOrder = DeAmendTicketingOrder
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

End Class
