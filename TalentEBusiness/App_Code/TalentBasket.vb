Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Web.Profile
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities

Public Class TalentBasket
    Inherits DEBasket

    Private _orderID As String
    ''' <summary>
    ''' This version tests to see if a TempOrderID has been assigned to the profile,
    ''' and returns it if present. However, if not, it will generate a TempOrderID and
    ''' return that.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TempOrderID() As String
        Get
            If Not String.IsNullOrEmpty(Temp_Order_Id) Then
                Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
                Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, HttpContext.Current.Profile.UserName)
                If dt.Rows.Count > 0 Then
                    _orderID = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("TEMP_ORDER_ID"))
                    basket.Update_Temp_order_id(_orderID, Me.Basket_Header_ID)
                    Me.Temp_Order_Id = _orderID
                End If
                Return Temp_Order_Id
            Else
                Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
                Try
                    Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                    Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, HttpContext.Current.Profile.UserName)
                    If dt.Rows.Count > 0 Then
                        _orderID = dt.Rows(0)("TEMP_ORDER_ID")
                        basket.Update_Temp_order_id(_orderID, Me.Basket_Header_ID)
                    End If
                Catch
                End Try
                If String.IsNullOrEmpty(_orderID) Then
                    Try
                        Dim prefix As String = String.Empty
                        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                        If def.WebOrderNumberPrefixOverride.ToString.Trim.Length > 0 Then
                            prefix = def.WebOrderNumberPrefixOverride.ToString.Trim
                        Else
                            prefix = TalentCache.GetBusinessUnit
                        End If
                        If Talent.eCommerce.Utilities.IsBasketHomeDelivery(HttpContext.Current.Profile) Then
                            prefix = "H" + prefix
                        End If
                        _orderID = prefix & _
                                        Talent.Common.Utilities.GetNextTempOrderNumber(TalentCache.GetBusinessUnit, _
                                            TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString).ToString
                        'End   MBSTEB-1304

                        CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id = _orderID

                        basket.Update_Temp_order_id(_orderID, Me.Basket_Header_ID)
                        If Not HttpContext.Current.Profile.IsAnonymous Then
                            Talent.eCommerce.Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                "NEW TEMP ORDER ID", _
                                                                "New Temp Order ID Generated - " & _orderID, _
                                                                "", _
                                                                TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                        End If
                    Catch
                    End Try
                End If
                Me.Temp_Order_Id = _orderID
                Return _orderID
            End If

        End Get
        Set(ByVal value As String)
            _orderID = value
            Me.Temp_Order_Id = value
        End Set
    End Property

    Public Sub AddItem(ByVal tbi As TalentBasketItem, Optional ByVal forceNewLine As Boolean = False, Optional ByVal isOverrideCATRestriction As Boolean = False)
        If (isOverrideCATRestriction OrElse (Talent.eCommerce.CATHelper.IsNotCATRequestOrBasketNotHasCAT("", "", -1))) Then
            If CType(HttpContext.Current.Profile, TalentProfile).Basket.IsEmpty Then
                Me.BasketItems.Add(tbi)
            Else
                Dim itemExists As Boolean = False
                If Not forceNewLine Then
                    For Each item As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                        If item.Product = tbi.Product AndAlso item.IS_FREE = tbi.IS_FREE Then
                            itemExists = True
                            item.Quantity += tbi.Quantity
                            item.Gross_Price = tbi.Gross_Price
                            item.Net_Price = tbi.Net_Price
                            item.Tax_Price = tbi.Tax_Price
                            item.Size = tbi.Size
                            item.WEIGHT = tbi.WEIGHT
                            Exit For
                        End If
                    Next
                End If

                If Not itemExists Then
                    Me.BasketItems.Add(tbi)
                End If
            End If
            Me.IsDirty = True
            TempOrderID = String.Empty
            HttpContext.Current.Profile.Save()
            Dim talBasketProcessor As New TalentBasketProcessor
            talBasketProcessor.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            'todo ck: this additem method requires an overload with below commented attributes passed as method parameters
            'talBasketProcessor.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talBasketProcessor.Settings.BusinessUnit)
            'talBasketProcessor.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talBasketProcessor.Settings.BusinessUnit)
            Dim errObj As ErrorObj = talBasketProcessor.ProcessSummaryForUpdatedBasket(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
        End If
    End Sub

    Public ReadOnly Property BasketContentType() As String
        Get
            Dim returnStr As String = ""
            If Me.BasketItems.Count > 0 Then
                Dim hasTickets As Boolean = False
                Dim hasProducts As Boolean = False
                For Each tbi As TalentBasketItem In Me.BasketItems
                    If Not TEBUtilities.IsTicketingFee(tbi.MODULE_, tbi.Product, tbi.FEE_CATEGORY) Then
                        Select Case tbi.MODULE_.ToUpper
                            Case "TICKETING"
                                hasTickets = True
                            Case "ECOMMERCE"
                                hasProducts = True
                            Case Else
                                hasProducts = True
                        End Select
                    End If
                    If hasProducts AndAlso hasTickets Then
                        Exit For
                    End If
                Next

                If hasProducts AndAlso hasTickets Then
                    returnStr = "C"
                ElseIf hasProducts AndAlso Not hasTickets Then
                    returnStr = "M"
                ElseIf Not hasProducts AndAlso hasTickets Then
                    returnStr = "T"
                End If
            End If

            Return returnStr
        End Get
    End Property

    Public Sub EmptyBasket(Optional isAgent As Boolean = False)
        Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        basketAdapter.Empty_Basket(Me.Basket_Header_ID)
        Me.BasketItems = New List(Of DEBasketItem)
        basketAdapter.Dispose()
        Dim basketHeaderAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        basketHeaderAdapter.Update_Cat_Mode(Me.Basket_Header_ID)
        basketHeaderAdapter.Dispose()
        Dim talDataObjects As New Talent.Common.TalentDataObjects()
        If isAgent Then
            talDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObjectForAgent()
        Else
            talDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        End If
        'now delete fees which are allocated for this basket
        talDataObjects.BasketSettings.TblBasketFees.DeleteAll(Me.Basket_Header_ID)
        'now delete order header which has any unpaid order
        Dim dtOrderHeader As System.Data.DataTable = talDataObjects.OrderSettings.TblOrderHeader.GetUnPaidOrder(TalentCache.GetBusinessUnit, HttpContext.Current.Profile.UserName)
        If dtOrderHeader IsNot Nothing AndAlso dtOrderHeader.Rows.Count > 0 Then
            talDataObjects.OrderSettings.DeleteUnPaidOrderByTempOrderID(Talent.eCommerce.Utilities.CheckForDBNull_String(dtOrderHeader.Rows(0)("TEMP_ORDER_ID")).Trim)
        End If
    End Sub

    Public Function DoesBasketHavePPS() As Boolean
        Dim hasPPS As Boolean = False
        For Each basketItem As TalentBasketItem In Me.BasketItems
            If basketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso basketItem.PRODUCT_TYPE = GlobalConstants.PPSPRODUCTTYPE AndAlso basketItem.PRODUCT_SUB_TYPE = GlobalConstants.PPSTYPE1 Then
                hasPPS = True
                Exit For
            ElseIf basketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso basketItem.PRODUCT_TYPE = GlobalConstants.PPSPRODUCTTYPE AndAlso basketItem.PRODUCT_SUB_TYPE = GlobalConstants.PPSTYPE2 Then
                hasPPS = True
                Exit For
            End If
        Next
        Return hasPPS
    End Function

    Public Function DoesBasketContainProductCode(ByVal ProductCode As String) As Boolean
        For Each basketitem As TalentBasketItem In Me.BasketItems
            If basketitem.Product = ProductCode Then
                Return True
            End If
        Next
        Return False
    End Function

End Class

Public Class TalentBasketItem
    Inherits DEBasketItem

End Class
