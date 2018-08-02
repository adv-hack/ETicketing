Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Checkout Order Summary
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCOSPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_CheckoutOrderSummary
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private _promoVal As Decimal
    Public Property PromotionValue() As Decimal
        Get
            Return _promoVal
        End Get
        Set(ByVal value As Decimal)
            _promoVal = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        If (Session("RedirectByPromotionBox") IsNot Nothing) Then
            Session("CheckoutBasketState") = Profile.Basket
            Session.Remove("RedirectByPromotionBox")
        End If

        If ModuleDefaults.DeliveryCalculationInUse Then
            Select Case UCase(ModuleDefaults.DeliveryPriceCalculationType)
                Case "UNIT", "WEIGHT"
                    DeliverySelection1.Display = True
                Case Else
                    DeliverySelection1.Display = False
            End Select
        Else
            DeliverySelection1.Display = False
        End If

        If ModuleDefaults.DeliveryDateType = 1 Then
            DeliveryDateCalendarPanel.Visible = True
            DeliveryDateTextPanel.Visible = False
        Else
            DeliveryDateCalendarPanel.Visible = False
            DeliveryDateTextPanel.Visible = True
        End If

        If Not Page.IsPostBack Then
            'Test to ensure that an address has been written to the order header
            '-------------------------------------------------------------------
            Dim validOrder As Boolean = True
            Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim orders As TalentBasketDataset.tbl_order_headerDataTable = orderHeaderTA.Get_Header_By_Temp_Order_Id(Profile.Basket.TempOrderID)
            If orders.Rows.Count > 0 Then
                Try
                    Dim order As TalentBasketDataset.tbl_order_headerRow = orders.Rows(0)

                    'Check to ensure then entries for the address are not null or blank
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(order.ADDRESS_LINE_1)) _
                        AndAlso String.IsNullOrEmpty(Utilities.CheckForDBNull_String(order.ADDRESS_LINE_2)) _
                            AndAlso String.IsNullOrEmpty(Utilities.CheckForDBNull_String(order.POSTCODE)) Then

                        'There no address lines or postcode so the order is not valid
                        validOrder = False
                    End If
                Catch ex As Exception
                    'If we hit an exception then we must have hit an unhandled null for one of the
                    'entries, so redirect to delivery details
                    validOrder = False
                End Try
            Else
                'There are no orderlines so the order is invalid
                validOrder = False
            End If

            If Not validOrder Then
                Response.Redirect("~/PagesLogin/Checkout/CheckoutDeliveryDetails.aspx")
            End If
            '-------------------------------------------------------------------
        End If

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            SetLabelText()
            SetDeliveryValues()
            SetDDLValues()
        End If

        Dim minPurchaseQuantity = ModuleDefaults.MinimumPurchaseQuantity
        Dim minPurchaseAmount = ModuleDefaults.MinimumPurchaseAmount
        Dim useMinQuantity = ModuleDefaults.UseMinimumPurchaseQuantity
        Dim useMinAmount = ModuleDefaults.UseMinimumPurchaseAmount

        If Profile.User.Details.Use_Minimum_Purchase_Quantity Then
            useMinQuantity = True
            minPurchaseQuantity = Profile.User.Details.Minimum_Purchase_Quantity
        ElseIf Profile.PartnerInfo.Details.Use_Minimum_Purchase_Quantity Then
            useMinQuantity = True
            minPurchaseQuantity = Profile.PartnerInfo.Details.Minimum_Purchase_Quantity
        End If
        If Profile.User.Details.Use_Minimum_Purchase_Amount Then
            useMinAmount = True
            minPurchaseAmount = Profile.User.Details.Minimum_Purchase_Amount
        ElseIf Profile.PartnerInfo.Details.Use_Minimum_Purchase_Amount Then
            useMinAmount = True
            minPurchaseAmount = Profile.PartnerInfo.Details.Minimum_Purchase_Amount
        End If

        If useMinQuantity AndAlso Profile.Basket.BasketItems.Count < minPurchaseQuantity Then
            Session("TalentErrorCode") = "QuantityTooLow"
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
        If useMinAmount AndAlso Profile.Basket.BasketSummary.TotalBasket < ModuleDefaults.MinimumPurchaseAmount Then
            Session("TalentErrorCode") = "AmountTooLow"
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If

        Select Case Profile.Basket.BasketContentType
            Case "M"
                Me.summaryDeliveryDetails.Visible = True
                'If PaymentTypeDDL.Items.Count > 1 Then
                '    Me.paymentMethodWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ShowPaymentMethodSelect"))
                'Else
                paymentMethodWrap.Visible = False
                'End If
            Case "C"
                Me.summaryDeliveryDetails.Visible = True
                Me.paymentMethodWrap.Visible = False
            Case "T"
                'Me.summaryDeliveryDetails.Visible = False
                'Me.paymentMethodWrap.Visible = False
                Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
            Case Else
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")

        End Select
        
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CheckoutOrderSummary.ascx"
        End With
        If Not Page.IsPostBack Then
            If ModuleDefaults.Show_Gift_Message_Option Then
                GiftMessage.Visible = True
            Else
                GiftMessage.Visible = False
            End If
            SetDDLValues()
            CreditCheck()
        End If
        '-------------------------------------------------------
        ' If not performing credit check then there's no need to 
        ' postback when changing the payment type
        '-------------------------------------------------------
        If Not ModuleDefaults.PerformCreditCheck Then
            PaymentTypeDDL.AutoPostBack = False
        End If
    End Sub
    Protected Sub CreditCheck()

        ErrorList.Items.Clear()

        If ModuleDefaults.PerformCreditCheck Then
            '--------------------------------------------------------------
            ' Check if credit check is relevant for particular payment type
            '--------------------------------------------------------------
            Dim payTypesTA As New TalentApplicationVariablesTableAdapters.PaymentTypesTA
            Dim payTypes As TalentApplicationVariables.PaymentTypesDataTable = _
                                  payTypesTA.GetDataBy_PaymentTypeCode(PaymentTypeDDL.SelectedValue.ToString.Trim)

            If payTypes.Rows.Count > 0 _
                AndAlso Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(payTypes.Rows(0)("CALL_CREDIT_LIMIT_CHECK")) Then

                Dim deCred As New Talent.Common.DECreditCheck(Profile.User.Details.Account_No_1)
                Dim dbCred As New Talent.Common.DBCreditCheck
                Dim credCheck As New Talent.Common.TalentCreditCheck
                Dim settings As New Talent.Common.DESettings

                settings.AccountNo1 = Profile.User.Details.Account_No_1
                settings.AccountNo2 = Profile.User.Details.Account_No_2

                deCred.TotalOrderValue = Profile.GetMinimumPurchaseAmount

                settings.BusinessUnit = TalentCache.GetBusinessUnit
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup()

                credCheck.Settings = settings
                credCheck.CreditCheck = deCred

                Dim err As Talent.Common.ErrorObj = credCheck.PerformCreditLimitCheck()

                If Not err.HasError Then
                    Try
                        Dim dt As Data.DataTable = credCheck.ResultDataSet.Tables("CreditCheckHeader")
                        If dt.Rows.Count > 0 Then
                            Dim credStatus As String = dt.Rows(0)("CreditStatus")
                            Select Case credStatus
                                Case Is = "1"
                                    'Credit will not be exceeded by new order
                                Case Is = "2"
                                    'Order will cause credit limit to be exceeded
                                    If ModuleDefaults.DisableCheckoutIfOverCreditLimit Then proceed.Enabled = False
                                    ErrorList.Items.Add(ucr.Content("CreditWillBeExceededError", _languageCode, True))
                                Case Is = "3"
                                    'Credit limit has already been exceeded
                                    If ModuleDefaults.DisableCheckoutIfOverCreditLimit Then proceed.Enabled = False
                                    ErrorList.Items.Add(ucr.Content("CreditExceededError", _languageCode, True))
                            End Select
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End If

    End Sub

    Protected Sub SetDDLValues()
        PaymentTypeDDL.Items.Clear()

        'if a restricted payment method for this user is in the DDL then remove it
        If ModuleDefaults.RestrictUserPaymentType Then
            Dim mixedBasket As Boolean = False
            Dim retailBasket As Boolean = True
            If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                retailBasket = True
            ElseIf Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                mixedBasket = True
            End If
            Dim dtPaymentTypes As Data.DataTable = TDataObjects.PaymentSettings.TblPaymentTypeBu.GetByBasketTypeAndBU(ucr.BusinessUnit, False, retailBasket, mixedBasket, Utilities.IsAgent(), Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE), Profile.IsAnonymous)

            Dim lic As New ListItemCollection
            Dim li As ListItem
            Dim exists As Boolean = False
            For Each row As Data.DataRow In dtPaymentTypes.Rows
                For Each item As ListItem In lic
                    If item.Value = row("PAYMENT_TYPE_CODE") Then
                        exists = True
                        Exit For
                    End If
                Next
                If Not exists Then
                    li = New ListItem(row("PAYMENT_TYPE_DESCRIPTION"), row("PAYMENT_TYPE_CODE"))
                    lic.Add(li)
                Else
                    exists = False
                End If
            Next
            If lic.Count > 0 Then
                lic.Insert(0, New ListItem(" -- ", " -- "))
            End If

            Dim i = Profile.User.Details.RESTRICTED_PAYMENT_METHOD
            Dim a As String()
            Dim j As Integer
            If i = Nothing Or i = "" Then
                i = "remove,nothing"
            End If
            Try
                a = i.Split(",")
            Catch ex As Exception
                ReDim a(0)
                a(0) = i
            End Try
            'RESTRICTED_PAYMENT_METHOD
            Dim count1 = 0
            For Each li1 As ListItem In lic
                If count1 > 0 Then
                    PaymentTypeDDL.Items.Add(li1)
                    Try
                        If a.Length > 0 Then
                            For j = 0 To a.GetUpperBound(0)
                                '  Dim str1 As String = li1.Text.ToString.Trim
                                Dim str1 As String = li1.Value.ToString.Trim
                                Dim str2 As String = a(j).ToString.Trim
                                If str1 = str2 Then
                                    PaymentTypeDDL.Items.Remove(li1)
                                End If
                            Next
                        Else
                            'no items to remove
                        End If
                    Catch ex As Exception
                    End Try
                End If
                count1 += 1
            Next
            If PaymentTypeDDL.Items.Count = 1 Then
                'force use of the one payment type
                PaymentTypeDDL.SelectedIndex = 0
                PaymentTypeDDL.Enabled = False

            ElseIf PaymentTypeDDL.Items.Count = 0 Then
                'Get Default Item
                'clear cache key
                Try
                    Cache.Remove(TalentCache.GetBusinessUnit & "*ALL" & "*ALL" & "PAYMENT_TYPE")
                Catch ex As Exception
                End Try
                Try
                    Cache.Remove("*ALL" & "*ALL" & "*ALL" & "PAYMENT_TYPE")
                Catch ex As Exception
                End Try
                Try
                    Cache.Remove(TalentCache.GetBusinessUnit & Profile.PartnerInfo.Details.Partner & "*ALL" & "PAYMENT_TYPE")
                Catch ex As Exception
                End Try

                Dim licDef As ListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "DEFAULT_PAYMENT_TYPE", "PAYMENT_TYPE")
                If licDef.Count > 0 Then
                    Dim defaultPM As String = licDef.Item(1).Text
                    PaymentTypeDDL.Items.Add(defaultPM)
                Else
                    PaymentTypeDDL.Items.Add(ucr.Content("NoDefaultPaymentMethod", _languageCode, True))
                End If
            End If
        Else
            'If we are not using restricted payment types
            Dim basketItemsCollection As Generic.List(Of Talent.Common.DEBasketItem) = Profile.Basket.BasketItems
            Dim isRetailBasket As Boolean = False
            Dim isTicketingBasket As Boolean = False
            Dim isMixedBasket As Boolean = False

            'Determine a mixed, ticketing or retail basket
            For Each basketItem As TalentBasketItem In basketItemsCollection
                If String.IsNullOrEmpty(basketItem.MODULE_) Then
                    isRetailBasket = True
                ElseIf basketItem.MODULE_ = "Ticketing" Then
                    isTicketingBasket = True
                End If
                If isRetailBasket And isTicketingBasket Then
                    isMixedBasket = True
                    isRetailBasket = False
                    isTicketingBasket = False
                    Exit For
                End If
            Next

            'Get the payment options depending on the basket type
            Dim tDataObjects As New Talent.Common.TalentDataObjects()
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            Dim paymentTypeDataTable As New Data.DataTable
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            tDataObjects.Settings = settings
            paymentTypeDataTable = tDataObjects.PaymentSettings.TblPaymentTypeBu.GetByBasketTypeAndBU(TalentCache.GetBusinessUnit, isTicketingBasket, isRetailBasket, isMixedBasket, Utilities.IsAgent(), True, Profile.IsAnonymous)

            'Build drop down list
            If paymentTypeDataTable.Rows.Count > 0 Then
                For Each row As Data.DataRow In paymentTypeDataTable.Rows
                    Try
                        Dim listItem As New ListItem
                        listItem.Value = Utilities.CheckForDBNull_String(row("PAYMENT_TYPE_CODE"))
                        listItem.Text = Utilities.CheckForDBNull_String(row("PAYMENT_TYPE_DESCRIPTION"))
                        listItem.Selected = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(row("DEFAULT_PAYMENT_TYPE"))
                        PaymentTypeDDL.Items.Add(listItem)
                    Catch ex As Exception
                    End Try
                Next
            End If

            If PaymentTypeDDL.Items.Count = 1 Then
                PaymentTypeDDL.SelectedIndex = 0
                PaymentTypeDDL.Enabled = False
            ElseIf PaymentTypeDDL.Items.Count = 0 Then
                PaymentTypeDDL.Items.Add(ucr.Content("NoDefaultPaymentMethod", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub SetDeliveryValues()
        Try
            Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter 
            Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, Profile.UserName)

            If dt.Rows.Count > 0 Then
                building.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_1"))
                Address2.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_2"))
                Address3.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_3"))
                Address4.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_4"))
                postcode.Text = UCase(Utilities.CheckForDBNull_String(dt.Rows(0)("POSTCODE")))
                Address5.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_5"))
                country.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("COUNTRY"))
                DeliveryContact.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("CONTACT_NAME"))
                DeliveryInstructions.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("COMMENT"))
                Try
                    DeliveryProjectedDate.Text = CType(Utilities.CheckForDBNull_String(dt.Rows(0)("PROJECTED_DELIVERY_DATE")), Date).ToString(ModuleDefaults.GlobalDateFormat)
                Catch ex As Exception
                    'unfit delivery date format
                    DeliveryProjectedDate.Text = ucr.Content("DeliveryDateFailText", _languageCode, True)
                End Try
                If DeliveryProjectedDate.Text = "" Or Not CType(ucr.Attribute("showDeliveryDate"), Boolean) Then
                    DeliveryProjectedDateLabel.Visible = False
                    DeliveryProjectedDate.Visible = False
                End If
            End If
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "OrderSummary - 00110", ex.Message, "Error retrieving order header to populate Delivery Details", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
        End Try
    End Sub
    Protected Sub SetLabelText()
        With ucr
            DeliveryDetailsTitle.Text = "<h2>" & .Content("TitleText", _languageCode, True) & "</h2>"
            BuildingLabel.Text = .Content("HouseNoLabel", _languageCode, True)
            PostcodeLabel.Text = .Content("PostcodeLabel", _languageCode, True)
            AddressLabel2.Text = .Content("AddressLabel2", _languageCode, True)
            AddressLabel3.Text = .Content("AddressLabel3", _languageCode, True)
            AddressLabel4.Text = .Content("AddressLabel4", _languageCode, True)
            AddressLabel5.Text = .Content("AddressLabel5", _languageCode, True)
            countryLabel.Text = .Content("CountryLabel", _languageCode, True)
            DeliveryContactLabel.Text = .Content("DeliveryContactLabel", _languageCode, True)
            DeliveryInsructionsLabel.Text = .Content("DeliveryInsructionsLabel", _languageCode, True)
            proceed.Text = .Content("ProceedButtonText", _languageCode, True)
            GiftMessage.Text = .Content("GiftMessageText", _languageCode, True)
            lblPayBy.Text = .Content("PayByLabel", _languageCode, True)
            lblSelectPayMethod.Text = .Content("SelectPayMethodLabel", _languageCode, True)
            DeliveryProjectedDateLabel.Text = .Content("DeliveryProjectedDateLabel", _languageCode, True)
        End With
    End Sub

    Protected Sub proceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles proceed.Click
        ProcessProceed()
    End Sub

    Protected Sub CheckPromotions_AND_AddTaxValuesToOrder()
        Try
            If ModuleDefaults.Call_Tax_WebService Then
                UpdateValues_TaxWebService()
            Else
                UpdateValues_Standard()
            End If
        Catch ex As Exception
            LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "ADE-TEST-FAILEDTAXCALC2" & ex.Message & ex.StackTrace)
        End Try

    End Sub

    Protected Function GetSelectedDeliveryCharge(ByVal totalOrder As Decimal, ByVal freeDel As Boolean) As Talent.Common.DEDeliveryCharges.DEDeliveryCharge
        Dim dcs As Talent.Common.DEDeliveryCharges = Nothing
        Select Case ModuleDefaults.DeliveryPriceCalculationType
            Case "UNIT"
                dcs = Utilities.GetDeliveryOptions(totalOrder, freeDel)
            Case "WEIGHT"
                Dim countryCode As String = String.Empty
                Dim countryName As String = String.Empty
                Utilities.GetDeliveryCountry(Profile.Basket.Temp_Order_Id, countryCode, countryName)
                dcs = Utilities.GetDeliveryOptions(Utilities.CheckForDBNull_Int(ucr.Attribute("CacheTimeMinutes")), totalOrder, freeDel, Utilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
        End Select

        Return dcs.GetDeliveryCharge(DeliverySelection1.SelectedDeliveryOption)
    End Function

    Protected Sub UpdateValues_Standard()
        Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter 
        Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter 

        'We are not using the Tax Web Service, so check the values in the standard manner
        Dim promoCode As String = ""
        Try
            Dim promoBox As Control = Utilities.FindWebControl("PromotionsBox", Me.Page.Controls)
            promoCode = CallByName(promoBox, "PromotionCode", CallType.Get)
        Catch ex As Exception
        End Try


        Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim liveBasketItems As Data.DataTable = basketAdapter.GetBasketItems_ByHeaderID_NonTicketing( _
                                                   CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)

        Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
        Dim totals As Talent.Common.TalentWebPricing

        'Select Case defaults.PricingType
        '    Case 2
        '        totals = Utilities.GetPrices_Type2

        '    Case Else
        '        Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
        '        For Each bItem As Data.DataRow In liveBasketItems.Rows
        '            If UCase(Utilities.CheckForDBNull_String(bItem("MODULE"))) <> "TICKETING" Then
        '                If Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
        '                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))) Then
        '                        'Check to see if the multibuys are configured for this master product
        '                        Dim myPrice As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(bItem("MASTER_PRODUCT"), 0, bItem("MASTER_PRODUCT"))
        '                        If myPrice.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse myPrice.PRICE_BREAK_QUANTITY_1 > 0 Then
        '                            'multibuys are configured
        '                            If qtyAndCodes.ContainsKey(bItem("MASTER_PRODUCT")) Then
        '                                qtyAndCodes(bItem("MASTER_PRODUCT")).Quantity += Utilities.CheckForDBNull_Decimal(bItem("QUANTITY"))
        '                            Else
        '                                ' Pass in product otherwise Promotions don't work properly
        '                                ' qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("MASTER_PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
        '                                qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
        '                            End If
        '                        Else
        '                            If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
        '                                qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
        '                            End If
        '                        End If
        '                    Else
        '                        If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
        '                            qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))))
        '                        End If
        '                    End If
        '                End If
        '            End If
        '        Next

        '        totals = Utilities.GetWebPrices_WithTotals(qtyAndCodes, _
        '                                                    Profile.Basket.TempOrderID, _
        '                                                    defaults.PromotionPriority, _
        '                                                    promoCode)
        'End Select

        totals = Profile.Basket.WebPrices

        If Not totals Is Nothing Then

            Dim delG As Decimal = 0, _
                delN As Decimal = 0, _
                delT As Decimal = 0, _
                delType As String = "", _
                delDesc As String = ""

            If defaults.DeliveryCalculationInUse Then
                Select Case UCase(defaults.DeliveryPriceCalculationType)
                    Case "UNIT", "WEIGHT"
                        Dim dc As New Talent.Common.DEDeliveryCharges.DEDeliveryCharge
                        If defaults.ShowPricesExVAT Then
                            dc = Me.GetSelectedDeliveryCharge(totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery)
                        Else
                            dc = Me.GetSelectedDeliveryCharge(totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery)
                        End If
                        delG = dc.GROSS_VALUE
                        delN = dc.NET_VALUE
                        delT = dc.TAX_VALUE
                        delType = dc.DELIVERY_TYPE
                        delDesc = dc.DESCRIPTION1
                    Case Else
                        '---------------------------------------------
                        ' If free delivery is from promotion then keep
                        ' delivery values. This is for balancing on
                        ' the backend.
                        '---------------------------------------------
                        If Not totals.Qualifies_For_Free_Delivery OrElse totals.FreeDeliveryPromotion Then
                            delG = totals.Max_Delivery_Gross
                            delN = totals.Max_Delivery_Net
                            delT = totals.Max_Delivery_Tax
                        End If
                End Select
            End If




            orderHeaderTA.UpdatePriceAndTaxValues(False, False, _
                                                   False, False, _
                                                   False, False, _
                                                   False, False, _
                                                   False, False, _
                                                   "", 0, _
                                                   "", 0, _
                                                   "", 0, _
                                                   "", 0, _
                                                   "", 0, _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Net, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Tax, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(delG, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(delN, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(delT, 0.01, False), _
                                                   Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross + delG, 0.01, False), _
                                                   delType, _
                                                   delDesc, _
                                                   Profile.Basket.TempOrderID, _
                                                   TalentCache.GetBusinessUnit, _
                                                   Profile.UserName)



            Select Case defaults.PricingType
                Case 2 'Only re-add the order lines if not using pricing type 2
                Case Else
                    Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                    orderLines.Empty_Order(Profile.Basket.TempOrderID)

                    'create an order obj and call the Add_OrderLines_To_Order function
                    '------------------------------------------------------------
                    Dim myOrder As New Talent.eCommerce.Order
                    myOrder.Add_OrderLines_To_Order(totals, Profile.Basket.TempOrderID, Talent.eCommerce.Utilities.GetCurrencyCode)

                    Try
                        Dim promoVal As Decimal = 0
                        Dim promoPercentage As Decimal = 0
                        If Not totals Is Nothing Then
                            promoVal = totals.Total_Promotions_Value
                            If Not totals.PromotionsResultsTable Is Nothing _
                                AndAlso totals.PromotionsResultsTable.Rows.Count > 0 Then
                                For Each promo As Data.DataRow In totals.PromotionsResultsTable.Rows
                                    If CDec(promo("PromotionPercentageValue")) > 0 Then
                                        promoPercentage = CDec(promo("PromotionPercentageValue"))
                                        Exit For
                                    End If
                                Next
                            End If
                        End If
                        orderHeaderTA.Set_Promotion_Value(Talent.eCommerce.Utilities.RoundToValue(promoVal, 0.01, False), promoPercentage, Profile.Basket.TempOrderID)
                        orderLines.Update_Promotion_Line_Values(0, promoPercentage, Profile.Basket.TempOrderID)
                    Catch ex As Exception
                    End Try
            End Select




        End If

    End Sub

    Protected Sub UpdateValues_TaxWebService()
        Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter 
        Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter 

        Try 'to set the order tax values
            Dim taxCalc As New TaxWebService
            Dim promoCode As String = ""
            Try
                Dim promoBox As Control = Utilities.FindWebControl("PromotionsBox", Me.Page.Controls)
                promoCode = CallByName(promoBox, "PromotionCode", CallType.Get)
            Catch ex As Exception
            End Try

            Dim taxValues As Data.DataSet = taxCalc.CallTaxWebService("ORDER", _
                                                                        Profile.Basket.TempOrderID, _
                                                                        promoCode)

            If Not HttpContext.Current.Session.Item("DunhillWSError") Is Nothing Then
                If CBool(HttpContext.Current.Session.Item("DunhillWSError")) Then
                    Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC2", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                    Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
            End If

            If taxValues.Tables.Count > 0 Then
                Dim header As Data.DataTable = taxValues.Tables(0)
                Dim detail As Data.DataTable = taxValues.Tables(1)

                If header.Rows.Count > 0 AndAlso detail.Rows.Count > 0 Then
                    Dim rw As Data.DataRow = header.Rows(0)

                    If CBool(rw("Success")) Then
                        Dim s1 As String = Utilities.CheckForDBNull_String(rw("TaxTypeCode1"))
                        Dim s5 As String = Utilities.CheckForDBNull_String(rw("TaxTypeCode5"))
                        orderHeaderTA.UpdatePriceAndTaxValues(Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(rw("DisplayInclusive1")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax1")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive2")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax2")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive3")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax3")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive4")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax4")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayInclusive5")), _
                                                                Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(rw("DisplayTax5")), _
                                                                Utilities.CheckForDBNull_String(rw("TaxTypeCode1")), _
                                                                Utilities.CheckForDBNull_String(rw("TotalTax1")), _
                                                                Utilities.CheckForDBNull_String(rw("TaxTypeCode2")), _
                                                                Utilities.CheckForDBNull_String(rw("TotalTax2")), _
                                                                Utilities.CheckForDBNull_String(rw("TaxTypeCode3")), _
                                                                Utilities.CheckForDBNull_String(rw("TotalTax3")), _
                                                                Utilities.CheckForDBNull_String(rw("TaxTypeCode4")), _
                                                                Utilities.CheckForDBNull_String(rw("TotalTax4")), _
                                                                Utilities.CheckForDBNull_String(rw("TaxTypeCode5")), _
                                                                Utilities.CheckForDBNull_String(rw("TotalTax5")), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("TotalGross")) - Utilities.CheckForDBNull_Decimal(rw("DeliveryGROSS")), 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("GoodsTotalNet")), 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("TotalTax1")) _
                                                                    + Utilities.CheckForDBNull_Decimal(rw("TotalTax2")) _
                                                                        + Utilities.CheckForDBNull_Decimal(rw("TotalTax3")) _
                                                                            + Utilities.CheckForDBNull_Decimal(rw("TotalTax4")) _
                                                                                + Utilities.CheckForDBNull_Decimal(rw("TotalTax5")), 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("DeliveryGROSS")), 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("DeliveryNET")), 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("DeliveryTAX")), 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(Utilities.CheckForDBNull_Decimal(rw("TotalGross")), 0.01, False), _
                                                                "", _
                                                                "", _
                                                                Profile.Basket.TempOrderID, _
                                                                TalentCache.GetBusinessUnit, _
                                                                Profile.UserName)

                        For Each dr As Data.DataRow In detail.Rows
                            Dim purchaseGross As Decimal = 0, _
                                purchaseNet As Decimal = 0, _
                                purchaseTax As Decimal = 0, _
                                deliveryGross As Decimal = 0, _
                                deliveryNet As Decimal = 0, _
                                deliveryTax As Decimal = 0, _
                                tariffCode As String = "", _
                                lineGross As Decimal = 0, _
                                lineNet As Decimal = 0, _
                                lineTax As Decimal = 0, _
                                taxAmount1 As Decimal = 0, _
                                taxAmount2 As Decimal = 0, _
                                taxAmount3 As Decimal = 0, _
                                taxAmount4 As Decimal = 0, _
                                taxAmount5 As Decimal = 0

                            Try
                                tariffCode = dr("TariffCode")
                            Catch ex As Exception
                            End Try
                            Try
                                taxAmount1 = CDec(dr("taxAmount1"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxAmount2 = CDec(dr("taxAmount2"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxAmount3 = CDec(dr("taxAmount3"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxAmount4 = CDec(dr("taxAmount4"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxAmount5 = CDec(dr("taxAmount5"))
                            Catch ex As Exception
                            End Try
                            Try
                                purchaseGross = CDec(dr("GrossAmount")) / CDec(dr("Quantity"))
                            Catch ex As Exception
                            End Try
                            Try
                                purchaseNet = CDec(dr("NetAmount")) / CDec(dr("Quantity"))
                            Catch ex As Exception
                            End Try
                            Try
                                purchaseTax = (taxAmount1 + taxAmount2 + taxAmount3 + taxAmount4 + taxAmount5) / CDec(dr("Quantity"))
                            Catch ex As Exception
                            End Try
                            Try
                                lineGross = purchaseGross * CDec(dr("Quantity"))
                            Catch ex As Exception
                            End Try
                            Try
                                lineNet = purchaseNet * CDec(dr("Quantity"))
                            Catch ex As Exception
                            End Try
                            Try
                                lineTax = purchaseTax * CDec(dr("Quantity"))
                            Catch ex As Exception
                            End Try
                            Try
                                deliveryGross = CDec(dr("deliveryGross"))
                            Catch ex As Exception
                            End Try
                            Try
                                deliveryNet = CDec(dr("deliveryNet"))
                            Catch ex As Exception
                            End Try
                            Try
                                deliveryTax = CDec(dr("deliveryTax"))
                            Catch ex As Exception
                            End Try

                            orderDetailTA.Update_Detail_Price_And_Tax_Values(Talent.eCommerce.Utilities.RoundToValue(purchaseGross, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(purchaseNet, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(purchaseTax, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(deliveryGross, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(deliveryNet, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(deliveryTax, 0.01, False), _
                                                                            "", _
                                                                            tariffCode, _
                                                                            Talent.eCommerce.Utilities.RoundToValue(lineGross, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(lineNet, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(lineTax, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(taxAmount1, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(taxAmount2, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(taxAmount3, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(taxAmount4, 0.01, False), _
                                                                            Talent.eCommerce.Utilities.RoundToValue(taxAmount5, 0.01, False), _
                                                                            Profile.Basket.TempOrderID, _
                                                                            dr("SKUCode"))

                        Next
                    Else
                        Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC3", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If
                Else
                    Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC4", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                    Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
            Else
                Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC5", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If

        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "ADE-TEST-FAILEDTAXCALC1", "TAX CALCULATOR ERROR", "", TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End Try
    End Sub

    Protected Sub PaymentTypeDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PaymentTypeDDL.SelectedIndexChanged
        CreditCheck()
    End Sub

    Protected Sub DeliveryDateCalendar_DayRender(ByVal sender As Object, ByVal e As DayRenderEventArgs) Handles DeliveryDateCalendar.DayRender
        If e.Day.Date.DayOfWeek = DayOfWeek.Sunday Then
            e.Cell.Controls.Clear()
            e.Cell.Text = e.Day.DayNumberText
            e.Cell.BackColor = Drawing.Color.Gainsboro
        End If

        If e.Day.Date.CompareTo(Date.Today.AddDays(2)) < 0 Then            
            e.Cell.Controls.Clear()
            e.Cell.Text = e.Day.DayNumberText
            e.Cell.BackColor = Drawing.Color.Gainsboro
        End If
    End Sub

    Protected Sub DeliveryDateCalendar_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeliveryDateCalendar.Load
        DeliveryDateCalendar.PrevMonthText = ""
    End Sub

    Protected Sub DeliveryDateCalendar_VisibleMonthChanged(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MonthChangedEventArgs) Handles DeliveryDateCalendar.VisibleMonthChanged
        If e.NewDate < DateTime.Today Then
            DeliveryDateCalendar.PrevMonthText = ""
        Else
            DeliveryDateCalendar.PrevMonthText = "<"
        End If
    End Sub

    Private Sub ProcessProceed()
        Utilities.CheckBasketFreeItemHasOptions()
        ErrorList.Items.Clear()
        Session("HSBCRequest") = Nothing
        Dim deliverySelected As Boolean = True

        If ModuleDefaults.DeliveryCalculationInUse Then
            If UCase(ModuleDefaults.DeliveryPriceCalculationType) = "UNIT" OrElse (UCase(ModuleDefaults.DeliveryPriceCalculationType) = "WEIGHT") Then
                If String.IsNullOrEmpty(DeliverySelection1.SelectedDeliveryOption) Then
                    deliverySelected = False
                End If
            End If
        End If

        If deliverySelected Then
            Try
                Dim orderHeaderTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim orderDetailTA As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

                CheckPromotions_AND_AddTaxValuesToOrder()

                orderHeaderTA.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus("SUMMARY"), _
                                                    Now, _
                                                    TalentCache.GetBusinessUnit, _
                                                    Profile.UserName, _
                                                    Profile.Basket.TempOrderID)
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCOSPR-010", ex.Message, "Error updating order status", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
            End Try
            Try
                Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                Profile.Basket.TempOrderID, _
                                                Talent.Common.Utilities.GetOrderStatus("SUMMARY"), _
                                                Now, _
                                                "")
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "UCOSPR-020", ex.Message, "Error adding order status flow", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
            End Try


            '-----------------------------------------------------------------------------
            ' BF - Commented out check - This assumes if there is only 1 option then it is 
            ' credit card payment which isn't necessarily correct
            '-----------------------------------------------------------------------------
            'If PaymentTypeDDL.Enabled = False Then
            'If GiftMessage.Checked Then
            ' Response.Redirect("~/PagesLogin/Checkout/CheckoutGiftMessage.aspx?RedirectTarget=~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            ' Else
            '    Response.Redirect("~/PagesLogin/Checkout/CheckoutPaymentDetails.aspx")
            '  End If
            '  Else

            Dim redirectUrl As String = String.Empty
            Dim payTypesTA As New TalentApplicationVariablesTableAdapters.PaymentTypesTA
            Dim payTypes As TalentApplicationVariables.PaymentTypesDataTable = _
                                    payTypesTA.GetDataBy_PaymentTypeCode(PaymentTypeDDL.SelectedValue.ToString.Trim)

            If payTypes.Rows.Count > 0 Then
                Dim payType As TalentApplicationVariables.PaymentTypesRow = payTypes.Rows(0)
                'redirectUrl = payType.NAVIGATE_URL.Trim & "?code=" & PaymentTypeDDL.SelectedValue.ToString.Trim
                redirectUrl = payType.NAVIGATE_URL.Trim
                If GiftMessage.Checked Then
                    Response.Redirect("~/PagesLogin/Checkout/CheckoutGiftMessage.aspx?RedirectTarget=" & redirectUrl)
                Else
                    Response.Redirect(redirectUrl)
                End If
            Else
                Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")
            End If
            ' End If
        Else
            ErrorList.Items.Add(ucr.Content("DeliveryTypeNotSelectedText", _languageCode, True))
        End If
    End Sub

   
End Class
