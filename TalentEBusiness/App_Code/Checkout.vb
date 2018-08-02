Imports Microsoft.VisualBasic
Imports System.Data
Imports Talent.eCommerce
Imports System.Data.SqlClient
Imports Talent.Common.TalentLogging

Namespace Talent.eCommerce

    Public Class Checkout
        Inherits ClassBase01

        Private Const paymentDetailsCachePrefix As String = "CheckoutStageDetails-"
        Private Const currentPaymentType As String = "CurrentPaymentType"
        Private Const lastPaymentCachePrefix As String = "LastPaymentDetails-"
        Private Const NoPaymentKey As String = "NoPayment"
        Private Const cache3dPrefix As String = "cache3dPrefix"
        Private Shared defaults As New Talent.eCommerce.ECommerceModuleDefaults
        Private Shared def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues

        'Public Shared Function CheckoutStages() As DataTable
        '    ' Declare this first! Used for Logging function duration
        '    Dim timeSpan As TimeSpan = Now.TimeOfDay

        '    Dim dt As New DataTable
        '    Dim cacheKey As String = "CheckoutStagesTable" & TalentCache.GetBusinessUnit

        '    'First check cache for the payment stages datatable
        '    If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        '        dt = HttpContext.Current.Cache.Item(cacheKey)
        '    Else

        '        Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        '        Dim SelectStr As String = " SELECT *  " & _
        '                                    " FROM tbl_checkout_stage WITH (NOLOCK)   " & _
        '                                    " WHERE (BUSINESS_UNIT = @BUSINESS_UNIT)  " & _
        '                                    " AND (PARTNER = @PARTNER)  " & _
        '                                    " ORDER BY CHECKOUT_STAGE "

        '        cmd.CommandText = SelectStr

        '        With cmd.Parameters
        '            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
        '            .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
        '        End With

        '        Try
        '            cmd.Connection.Open()

        '            Dim da As New SqlDataAdapter(cmd)
        '            da.Fill(dt)

        '            If dt.Rows.Count = 0 Then
        '                cmd.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
        '                da.Fill(dt)
        '                If dt.Rows.Count = 0 Then
        '                    cmd.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
        '                    da.Fill(dt)
        '                End If
        '            End If

        '            da.Dispose()
        '        Catch ex As Exception
        '        Finally
        '            If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
        '        End Try

        '        'Cache the results
        '        HttpContext.Current.Cache.Add(cacheKey, dt, Nothing, Now.AddMinutes(30), timeSpan.Zero, CacheItemPriority.Normal, Nothing)
        '        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        '    End If

        '    Utilities.TalentLogging.LoadTestLog("Checkout.vb", "CheckoutStages", timeSpan)
        '    Return dt

        'End Function

        'Public Shared Function CheckoutStage(Optional ByVal pageCode As String = "") As DataTable

        '    'Set the current page when not passed in
        '    If pageCode.Trim.Equals("") Then
        '        pageCode = Utilities.GetCurrentPageName.Trim
        '    End If

        '    ' Retrieve all of the payment stages
        '    Dim dt As DataTable = CheckoutStages()

        '    'Create a new table based on the table retrieved
        '    Dim dt2 As New DataTable
        '    For Each col As DataColumn In dt.Columns
        '        dt2.Columns.Add(New DataColumn(col.ColumnName, col.DataType))
        '    Next

        '    'Loop through all of the payment stages
        '    Dim loopIndex As Integer = 0
        '    Do While loopIndex < dt.Rows.Count

        '        'Add all rows to our new table that equals the page code
        '        If UCase(dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim) = UCase(pageCode.Trim) Then
        '            Dim dr As DataRow = dt2.NewRow
        '            For Each col As DataColumn In dt.Columns
        '                dr(col.ColumnName) = dt.Rows(loopIndex).Item(col.ColumnName)
        '            Next
        '            dt2.Rows.Add(dr)
        '        End If
        '        loopIndex = loopIndex + 1
        '    Loop

        '    Return dt2

        'End Function

        'Public Shared Function DDPaymentValid(Optional ByVal checkForDDProduct As Boolean = False, Optional ByRef errorCode As String = "") As Boolean
        '    Dim dt As DataTable = CheckoutStage("checkoutDirectDebitDetails.aspx")
        '    Dim paymentOptions As String = String.Empty
        '    Dim numberOfItemsValidForDD As Integer = 0

        '    DDPaymentValid = False
        '    If dt.Rows.Count > 0 Then

        '        If Not checkForDDProduct Then
        '            DDPaymentValid = True
        '        End If

        '        'All the products in the basket must be able to be completed by direct debits
        '        For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems

        '            'First check that the item isn't a retail item
        '            If Not String.IsNullOrEmpty(tbi.MODULE_.Trim) Then

        '                'PPS products and fees should be excluded from this check
        '                If Not (tbi.MODULE_.Trim = "Ticketing" And tbi.PRODUCT_TYPE.Trim = "P") And _
        '                    Not Utilities.IsTicketingFee(tbi.MODULE_.Trim, tbi.Product.Trim, tbi.FEE_CATEGORY) Then

        '                    'Loop through each valid type for this payment stage
        '                    Dim loopIndex As Integer = 0
        '                    Do While loopIndex < dt.Rows.Count

        '                        'Do we have a valid type
        '                        If (tbi.MODULE_.Trim = dt.Rows(loopIndex).Item("MODULE").ToString.Trim Or dt.Rows(loopIndex).Item("MODULE").ToString.Trim = "*ALL") And _
        '                            (tbi.PRODUCT_TYPE.Trim = dt.Rows(loopIndex).Item("PRODUCT_TYPE").ToString.Trim Or dt.Rows(loopIndex).Item("PRODUCT_TYPE").ToString.Trim = "*ALL") And _
        '                            (tbi.PRODUCT_SUB_TYPE.Trim = dt.Rows(loopIndex).Item("PRODUCT_SUB_TYPE").ToString.Trim Or dt.Rows(loopIndex).Item("PRODUCT_SUB_TYPE").ToString.Trim = "*ALL") Then
        '                            If checkForDDProduct Then
        '                                DDPaymentValid = True
        '                                Exit For
        '                            End If
        '                            numberOfItemsValidForDD += 1
        '                        Else
        '                            If Not checkForDDProduct Then
        '                                'Product can't be purchased by direct debit.  Direct debit payment option no longer valid
        '                                DDPaymentValid = False
        '                                Exit For
        '                            End If
        '                        End If

        '                        loopIndex = loopIndex + 1
        '                    Loop
        '                End If
        '            End If

        '            If (Not DDPaymentValid And Not checkForDDProduct) Or (DDPaymentValid And checkForDDProduct) Then
        '                Exit For
        '            End If

        '            'At this point we need to determine if we are allowing the user to checkout with more than 1 product by direct debit - Eg. 2 season tickets.
        '            'First ensure that DDPaymentValid is true upto this point and that the AllowMultipleProductsInDDCheckout is false.
        '            'Also check to see if we have more than 1 item that is valid for DD.
        '            'If all of these cases are true then we can't allow DD as a payment method.
        '            Dim def As ECommerceModuleDefaults.DefaultValues = defaults.GetDefaults
        '            If Not def.AllowMultipleProductsInDDCheckout And DDPaymentValid And numberOfItemsValidForDD > 1 Then
        '                DDPaymentValid = False
        '                errorCode = "MultiDDProductsNotValid"
        '                Exit For
        '            End If
        '        Next
        '    End If

        '    Return DDPaymentValid
        'End Function

        'Public Shared Function DDPaymentActive() As Boolean
        '    Dim dt As DataTable = CheckoutStage("checkoutDirectDebitDetails.aspx")
        '    Dim paymentOptions As String = String.Empty
        '    DDPaymentActive = False
        '    If dt.Rows.Count > 0 Then
        '        'Is direct debit a valid payment method
        '        Dim lic As ListItemCollection = TalentCache.GetDropDownControlText(Utilities.GetCurrentLanguageForDDLPopulation, "PAYMENT_TYPE", "PAYMENT_TYPE")
        '        For Each li As ListItem In lic
        '            If li.Value.ToString.Trim = "DD" Then
        '                DDPaymentActive = True
        '                Exit For
        '            End If
        '        Next
        '    End If

        '    'We must verify that we have one product in the shopping basket that
        '    'can be completed by direct debit
        '    If DDPaymentActive Then
        '        DDPaymentActive = DDPaymentValid(True)
        '    End If

        '    Return DDPaymentActive
        'End Function

        'Public Shared Function ValidProduct(ByVal pageCode As String, _
        '                                        ByVal module1 As String, _
        '                                        ByVal productType As String, _
        '                                        ByVal productSubType As String) As Boolean

        '    'Set the current page when not passed in
        '    If pageCode.Trim.Equals(String.Empty) Then
        '        pageCode = Utilities.GetCurrentPageName.Trim
        '    End If
        '    If String.IsNullOrEmpty(productSubType) Then productSubType = String.Empty
        '    Dim finalPage As Boolean = FinalCheckoutStage(pageCode.Trim)

        '    Dim ddPage As Boolean = False
        '    Dim ddValid As Boolean = False
        '    If UCase(pageCode.Trim) = UCase("checkoutDirectDebitDetails.aspx") Then
        '        ddPage = True
        '        ddValid = DDPaymentValid()
        '    End If

        '    'Are we on a defined checkout page
        '    ValidProduct = True
        '    If ValidCheckoutStage(pageCode.Trim) Then

        '        'All products are valid for the final checkout stage until proven otherwise
        '        If finalPage Then
        '            ValidProduct = True
        '        Else
        '            ValidProduct = False
        '        End If

        '        'Retrieve all of the checkout stage
        '        Dim dt As DataTable = CheckoutStages()

        '        'Can we take payment for this product on this page
        '        Dim loopIndex As Integer = 0
        '        For loopIndex = 0 To dt.Rows.Count - 1

        '            'Do we have a record for this product type
        '            If (module1.Trim = dt.Rows(loopIndex).Item("MODULE").ToString.Trim Or dt.Rows(loopIndex).Item("MODULE").ToString.Trim = "*ALL") And _
        '                (productType.Trim = dt.Rows(loopIndex).Item("PRODUCT_TYPE").ToString.Trim Or dt.Rows(loopIndex).Item("PRODUCT_TYPE").ToString.Trim = "*ALL") And _
        '                (productSubType.Trim = dt.Rows(loopIndex).Item("PRODUCT_SUB_TYPE").ToString.Trim Or dt.Rows(loopIndex).Item("PRODUCT_SUB_TYPE").ToString.Trim = "*ALL") Then

        '                'Are we on the right page for this product
        '                If UCase(dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim) = UCase(pageCode.Trim) Then

        '                    'Do not display the basket when on the direct debit page and direct debit payments are invalid
        '                    If ddPage AndAlso Not ddValid Then
        '                        ValidProduct = False
        '                    Else
        '                        ValidProduct = True
        '                    End If
        '                    Exit For
        '                Else

        '                    'Did we bypass the payment for this page
        '                    If RetrievePaymentItem(NoPaymentKey, dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim).ToString.Trim <> String.Empty Then
        '                        If Not finalPage Then
        '                            ValidProduct = False
        '                        End If
        '                        Exit For
        '                    Else

        '                        'The final stage will display all products that have not been paid for.  This check will determine these products.
        '                        If finalPage Then
        '                            If RetrievePaymentItem("PaymentType", dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim).ToString.Trim <> String.Empty Then
        '                                ValidProduct = False
        '                                Exit For
        '                            End If
        '                        End If
        '                    End If
        '                End If
        '            End If
        '        Next

        '    End If

        '    Return ValidProduct
        'End Function

        ''' <summary>
        ''' Sub for retrieving DDI and Originator
        ''' </summary>
        ''' <param name="productCodeForDD"></param>
        ''' <param name="ddiRef"></param>
        ''' <param name="originator"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDirectDebitIds(ByVal productCodeForDD As String, ByRef ddiRef As String, ByRef originator As String) As Boolean
            Dim timeSpan As TimeSpan = Now.TimeOfDay
            Dim productCanBePurchasedWithDirectDebit As Boolean = False
            Dim err As New Talent.Common.ErrorObj
            Dim returnErrorCode As String = String.Empty
            Dim ucr As New Talent.Common.UserControlResource

            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "Checkout.vb"
            End With

            'Create the payment object
            Dim payment As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            With dePayment
                .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                .Source = GlobalConstants.SOURCE
                .PaymentStage = Utilities.GetCurrentPageName.Trim
                .CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                .ProductCodeForDD = productCodeForDD
            End With

            payment.De = dePayment
            payment.Settings = Utilities.GetSettingsObject()
            payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
            payment.Settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup
            payment.Settings.Cacheing = False
            payment.Settings.CacheTimeMinutes = CType(ucr.Attribute("CacheTimeMinutes"), Integer)
            payment.Settings.CacheDependencyPath = def.CacheDependencyPath

            err = payment.DirectDebitDDIRef

            If Not err.HasError AndAlso Not payment.ResultDataSet Is Nothing AndAlso payment.ResultDataSet.Tables.Count > 1 Then
                productCanBePurchasedWithDirectDebit = True
                ddiRef = payment.ResultDataSet.Tables(1).Rows(0).Item("DirectDebitDDIRef").ToString
                originator = payment.ResultDataSet.Tables(1).Rows(0).Item("Originator").ToString
            Else
                productCanBePurchasedWithDirectDebit = False
            End If

            Utilities.TalentLogging.LoadTestLog("Checkout.vb", "RetrieveDirectDebitIds", timeSpan)
            Return productCanBePurchasedWithDirectDebit
        End Function

        'Public Shared Function ValidCheckoutStage(Optional ByVal pageCode As String = "") As Boolean

        '    'Set the current page when not passed in
        '    If pageCode.Trim.Equals("") Then
        '        pageCode = Utilities.GetCurrentPageName.Trim
        '    End If

        '    'Is this a valid checkout stage
        '    Dim dt As DataTable = CheckoutStages()
        '    ValidCheckoutStage = False
        '    If dt.Rows.Count > 0 Then
        '        Dim loopIndex As Integer = 0
        '        For loopIndex = 0 To dt.Rows.Count - 1
        '            If UCase(dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim) = UCase(pageCode.Trim) Then
        '                ValidCheckoutStage = True
        '            End If
        '        Next
        '    End If
        '    Return ValidCheckoutStage
        'End Function

        'Public Shared Function FinalCheckoutStage(Optional ByVal pageCode As String = "") As Boolean

        '    'Set the current page when not passed in
        '    If pageCode.Trim.Equals("") Then
        '        pageCode = Utilities.GetCurrentPageName.Trim
        '    End If

        '    'Is this the final checkout stage
        '    Dim dt As DataTable = CheckoutStages()
        '    FinalCheckoutStage = False
        '    If dt.Rows.Count > 0 Then
        '        If UCase(dt.Rows(dt.Rows.Count - 1).Item("PAGE_CODE").ToString.Trim) = UCase(pageCode.Trim) Then
        '            FinalCheckoutStage = True
        '        End If
        '    End If
        '    Return FinalCheckoutStage
        'End Function

        ''' <summary>
        ''' Store the EP details
        ''' </summary>
        ''' <param name="cardNumber"></param>
        ''' <remarks></remarks>
        Public Shared Sub StoreEPDetails(ByVal gcCardNumber As String, ByVal cardNumber As String, ByVal PIN As String, ByVal isGiftCard As Boolean)
            Dim epDetails As New Generic.Dictionary(Of String, String)
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim partner As String = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
            With epDetails
                .Add("CardNumber", EncodePaymentDetails(cardNumber))
                .Add("GCCardNumber", EncodePaymentDetails(gcCardNumber))
                .Add("PIN", EncodePaymentDetails(PIN))
                .Add("IsGiftCard", EncodePaymentDetails(isGiftCard))
                .Add("Currency", EncodePaymentDetails(tDataObjects.PaymentSettings.GetCurrencyCode(TalentCache.GetBusinessUnit, partner)))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.EPURSEPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = epDetails
        End Sub

        ''' <summary>
        ''' Store the Cash Details
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub StoreCSDetails()
            Dim csDetails As New Generic.Dictionary(Of String, String)
            With csDetails
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.CSPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = csDetails
        End Sub

        ''' <summary>
        ''' Store the On Account Details
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub StoreOADetails()
            Dim oaDetails As New Generic.Dictionary(Of String, String)
            With oaDetails
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.OAPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = oaDetails
        End Sub

        ''' <summary>
        ''' Store the PDQ Details
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub StorePDDetails()
            Dim pdDetails As New Generic.Dictionary(Of String, String)
            With pdDetails
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.PDQPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = pdDetails
        End Sub

        ''' <summary>
        ''' Store the credit card details
        ''' </summary>
        ''' <param name="cardType"></param>
        ''' <param name="cardNumber"></param>
        ''' <param name="expiryDate"></param>
        ''' <param name="startDate"></param>
        ''' <param name="issueNumber"></param>
        ''' <param name="cv2Number"></param>
        ''' <param name="cardHolderName"></param>
        ''' <param name="installments"></param>
        ''' <param name="paymentStage"></param>
        ''' <param name="saveTheseCardDetails"></param>
        ''' <remarks></remarks>
        Public Shared Sub StoreCCDetails(ByVal cardType As String, ByVal cardNumber As String, ByVal expiryDate As String, _
                                         ByVal startDate As String, ByVal issueNumber As String, ByVal cv2Number As String, _
                                         ByVal cardHolderName As String, ByVal installments As String, _
                                         ByVal vgTokenId As String, ByVal vgProcessingDB As String, _
                                         ByVal vgTokenDate As String, ByVal transactionID As String, _
                                         Optional ByVal paymentStage As String = GlobalConstants.CHECKOUTASPXSTAGE, _
                                         Optional ByVal saveTheseCardDetails As Boolean = False)
            'Create a dictionary of the credit card details
            Dim ccDetails As New Generic.Dictionary(Of String, String)
            With ccDetails
                .Add("CardType", EncodePaymentDetails(cardType))
                .Add("CardNumber", EncodePaymentDetails(cardNumber))
                .Add("ExpiryDate", EncodePaymentDetails(expiryDate))
                .Add("StartDate", EncodePaymentDetails(startDate))
                .Add("IssueNumber", EncodePaymentDetails(issueNumber))
                .Add("CV2Number", EncodePaymentDetails(cv2Number))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.CCPAYMENTTYPE))
                .Add("CardHolderName", EncodePaymentDetails(cardHolderName))
                .Add("Installments", EncodePaymentDetails(installments))
                .Add("VGTokenID", EncodePaymentDetails(vgTokenId))
                .Add("VGProcessingDB", EncodePaymentDetails(vgProcessingDB))
                .Add("VGTokenDate", EncodePaymentDetails(vgTokenDate))
                .Add("TransactionID", EncodePaymentDetails(transactionID))
                If saveTheseCardDetails Then
                    .Add("SaveTheseCardDetails", EncodePaymentDetails(GlobalConstants.SAVECARDSAVEMODE))
                Else
                    .Add("SaveTheseCardDetails", EncodePaymentDetails(String.Empty))
                End If
            End With

            'Store the details as the last credit card details used
            HttpContext.Current.Session(paymentStage) = ccDetails
        End Sub

        ''' <summary>
        ''' Store the invoice payment Details
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub StoreINVDetails()
            Dim csDetails As New Generic.Dictionary(Of String, String)
            With csDetails
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.INVPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.INVPAYMENTTYPE) = csDetails
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = csDetails

        End Sub
        ''' <summary>
        ''' Store the Others payment Details
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub StoreOTDetails(ByVal otherPaymentType As String, ByVal otherTextValue As String)
            Dim csDetails As New Generic.Dictionary(Of String, String)
            With csDetails
                .Add("OtherPaymentType", EncodePaymentDetails(otherPaymentType))
                .Add("OtherTextValue", EncodePaymentDetails(otherTextValue))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.OTHERSPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.OTHERSPAYMENTTYPE) = csDetails
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = csDetails
        End Sub

        ''' <summary>
        ''' Store the saved credit card details
        ''' </summary>
        ''' <param name="cardNumber"></param>
        ''' <param name="expiryDate"></param>
        ''' <param name="startDate"></param>
        ''' <param name="issueNumber"></param>
        ''' <param name="cv2Number"></param>
        ''' <param name="lastFourCardDigits"></param>
        ''' <param name="uniqueCardId"></param>
        ''' <param name="vgTokenId"></param>
        ''' <param name="vgProcessingDB"></param>
        ''' <param name="vgTokenDate"></param>
        ''' <param name="paymentStage"></param>
        ''' <remarks></remarks>
        Public Shared Sub StoreSCDetails(ByVal cardNumber As String, ByVal expiryDate As String, _
                                         ByVal startDate As String, ByVal issueNumber As String, ByVal cv2Number As String,
                                         ByVal lastFourCardDigits As String, ByVal uniqueCardId As String,
                                         ByVal vgTokenId As String, ByVal vgProcessingDB As String,
                                         ByVal vgTokenDate As String, ByVal transactionID As String,
                                         Optional ByVal paymentStage As String = GlobalConstants.CHECKOUTASPXSTAGE)
            'Create a dictionary of the credit card details
            Dim scDetails As New Generic.Dictionary(Of String, String)
            With scDetails
                .Add("CardNumber", EncodePaymentDetails(cardNumber))
                .Add("ExpiryDate", EncodePaymentDetails(expiryDate))
                .Add("StartDate", EncodePaymentDetails(startDate))
                .Add("IssueNumber", EncodePaymentDetails(issueNumber))
                .Add("CV2Number", EncodePaymentDetails(cv2Number))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.SAVEDCARDPAYMENTTYPE))
                .Add("LastFourCardDigits", EncodePaymentDetails(lastFourCardDigits))
                .Add("UniqueCardId", EncodePaymentDetails(uniqueCardId))
                .Add("VGTokenID", EncodePaymentDetails(vgTokenId))
                .Add("VGProcessingDB", EncodePaymentDetails(vgProcessingDB))
                .Add("VGTokenDate", EncodePaymentDetails(vgTokenDate))
                .Add("TransactionID", EncodePaymentDetails(transactionID))
                .Add("SaveTheseCardDetails", EncodePaymentDetails(GlobalConstants.SAVECARDUSEMODE))
            End With

            'Store the details as the last credit card details used
            HttpContext.Current.Session(paymentStage) = scDetails
        End Sub

        ''' <summary>
        ''' Store the direct debit details
        ''' </summary>
        ''' <param name="accountName"></param>
        ''' <param name="sortCode"></param>
        ''' <param name="accountNumber"></param>
        ''' <param name="bankName"></param>
        ''' <param name="paymentDay"></param>
        ''' <param name="productCodeForDD"></param>
        ''' <param name="paymentStage"></param>
        ''' <remarks></remarks>
        Public Shared Sub StoreDDDetails(ByVal accountName As String, ByVal sortCode As String, ByVal accountNumber As String, ByVal bankName As String, _
                                          ByVal paymentDay As String, Optional ByVal productCodeForDD As String = "", Optional ByVal paymentStage As String = GlobalConstants.CHECKOUTASPXSTAGE)

            'Retrieve the DDI and Originator number from the backend\cache
            Dim originator As String = String.Empty
            Dim ddiRef As String = String.Empty
            Checkout.GetDirectDebitIds(productCodeForDD, ddiRef, originator)
            'Create a dictionary of the direct debit details
            Dim ddDetails As New Generic.Dictionary(Of String, String)
            With ddDetails
                .Add("AccountName", EncodePaymentDetails(accountName))
                .Add("SortCode", EncodePaymentDetails(sortCode))
                .Add("AccountNumber", EncodePaymentDetails(accountNumber))
                .Add("BankName", EncodePaymentDetails(bankName))
                .Add("PaymentDay", EncodePaymentDetails(paymentDay))
                .Add("Originator", EncodePaymentDetails(originator))
                .Add("DDIReference", EncodePaymentDetails(ddiRef))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.DDPAYMENTTYPE))
            End With

            HttpContext.Current.Session(paymentStage) = ddDetails
        End Sub

        ''' <summary>
        ''' Store the credit finance details
        ''' </summary>
        ''' <param name="accountName"></param>
        ''' <param name="sortCode"></param>
        ''' <param name="accountNumber"></param>
        ''' <param name="bankName"></param>
        ''' <param name="InstallmentPlan"></param>
        ''' <param name="YearsAtAddress"></param>
        ''' <param name="AddressLine1"></param>
        ''' <param name="AddressLine2"></param>
        ''' <param name="AddressLine3"></param>
        ''' <param name="AddressLine4"></param>
        ''' <param name="PostCode"></param>
        ''' <remarks></remarks>
        Public Shared Sub StoreCFDetails(ByVal accountName As String, ByVal sortCode As String, ByVal accountNumber As String, ByVal bankName As String, _
                                  ByVal InstallmentPlan As String, ByVal YearsAtAddress As String, ByVal AddressLine1 As String, ByVal AddressLine2 As String, _
                                  ByVal AddressLine3 As String, ByVal AddressLine4 As String, ByVal PostCode As String, clubProductCodeForFinance As String, _
                                  ByVal MonthsAtAddress As String, ByVal HomeStatus As String, ByVal EmploymentStatus As String, ByVal GrossIncome As String)
            'Create a dictionary of the credit finance details
            Dim cfDetails As New Generic.Dictionary(Of String, String)
            With cfDetails
                .Add("AccountName", EncodePaymentDetails(accountName))
                .Add("SortCode", EncodePaymentDetails(sortCode))
                .Add("AccountNumber", EncodePaymentDetails(accountNumber))
                .Add("BankName", EncodePaymentDetails(bankName))
                .Add("InstallmentPlanCode", EncodePaymentDetails(InstallmentPlan))
                .Add("YearsAtAddress", EncodePaymentDetails(YearsAtAddress))
                .Add("AddressLine1", EncodePaymentDetails(AddressLine1))
                .Add("AddressLine2", EncodePaymentDetails(AddressLine2))
                .Add("AddressLine3", EncodePaymentDetails(AddressLine3))
                .Add("AddressLine4", EncodePaymentDetails(AddressLine4))
                .Add("PostCode", EncodePaymentDetails(PostCode))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.CFPAYMENTTYPE))
                .Add("clubProductCodeForFinance", EncodePaymentDetails(clubProductCodeForFinance))
                .Add("MonthsAtAddress", EncodePaymentDetails(MonthsAtAddress))
                .Add("HomeStatus", EncodePaymentDetails(HomeStatus))
                .Add("EmploymentStatus", EncodePaymentDetails(EmploymentStatus))
                .Add("GrossIncome", EncodePaymentDetails(GrossIncome))
            End With

            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = cfDetails
        End Sub

        ''' <summary>
        ''' Store the Chip and PIN details
        ''' </summary>
        ''' <param name="ipAddress">The chip and pin device that has been selected</param>
        ''' <remarks></remarks>
        Public Shared Sub StoreCPDetails(ByVal ipAddress As String)
            Dim cpDetails As New Generic.Dictionary(Of String, String)
            With cpDetails
                .Add("IPAddress", EncodePaymentDetails(ipAddress))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.CHIPANDPINPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = cpDetails
        End Sub

        ''' <summary>
        ''' Store the Point of Sale details
        ''' </summary>
        ''' <param name="ipAddress">The ip address of the POS Terminal that has been selected</param>
        ''' <param name="tcpPort">The TCP Port of the POS Terminal that has been selected</param>
        ''' <remarks></remarks>
        Public Shared Sub StorePSDetails(ByVal ipAddress As String, ByVal tcpPort As String)
            Dim psDetails As New Generic.Dictionary(Of String, String)
            With psDetails
                .Add("IPAddress", EncodePaymentDetails(ipAddress))
                .Add("TCPPort", EncodePaymentDetails(tcpPort))
                .Add("PaymentType", EncodePaymentDetails(GlobalConstants.POINTOFSALEPAYMENTTYPE))
            End With
            HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) = psDetails
        End Sub

        ''' <summary>
        ''' Store the 3D Secure details
        ''' </summary>
        ''' <param name="eci"></param>
        ''' <param name="cavv"></param>
        ''' <param name="XID"></param>
        ''' <param name="authenticationStatus"></param>
        ''' <param name="sID"></param>
        ''' <param name="atsData"></param>
        ''' <param name="talent3dSecureTransactionID"></param>
        ''' <param name="Enrolled"></param>
        ''' <param name="PAResStatus"></param>
        ''' <param name="SignatureVerification"></param>
        ''' <remarks></remarks>
        Public Shared Sub Store3dDetails(ByVal eci As String, ByVal cavv As String, ByVal XID As String, ByVal authenticationStatus As String, ByVal sID As String, _
                                          ByVal atsData As String, ByVal talent3dSecureTransactionID As String, ByVal Enrolled As String, ByVal PAResStatus As String, _
                                          ByVal SignatureVerification As String)
            HttpContext.Current.Session(cache3dPrefix & "eci") = eci
            HttpContext.Current.Session(cache3dPrefix & "cavv") = cavv
            HttpContext.Current.Session(cache3dPrefix & "XID") = XID
            HttpContext.Current.Session(cache3dPrefix & "authenticationStatus") = authenticationStatus
            HttpContext.Current.Session(cache3dPrefix & "sID") = sID
            HttpContext.Current.Session(cache3dPrefix & "atsData") = atsData
            HttpContext.Current.Session(cache3dPrefix & "talent3dSecureTransactionID") = talent3dSecureTransactionID
            HttpContext.Current.Session(cache3dPrefix & "enrolled") = Enrolled
            HttpContext.Current.Session(cache3dPrefix & "PAResStatus") = PAResStatus
            HttpContext.Current.Session(cache3dPrefix & "signatureVerification") = SignatureVerification
        End Sub

        ''' <summary>
        ''' Clear the 3D Secure details
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub Clear3dDetails()
            HttpContext.Current.Session(cache3dPrefix & "eci") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "cavv") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "XID") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "authenticationStatus") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "sID") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "atsData") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "talent3dSecureTransactionID") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "enrolled") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "PAResStatus") = Nothing
            HttpContext.Current.Session(cache3dPrefix & "signatureVerification") = Nothing

            HttpContext.Current.Session("MerchantReference") = Nothing
            HttpContext.Current.Session("PayerAuthRequestId") = Nothing
            HttpContext.Current.Session("Enrolled") = Nothing
            HttpContext.Current.Session("AcsUrl") = Nothing
            HttpContext.Current.Session("PAReq") = Nothing
            HttpContext.Current.Session("TermUrl") = Nothing
            HttpContext.Current.Session("MD") = Nothing
            HttpContext.Current.Session("WinbankECI") = Nothing
        End Sub

        ''' <summary>
        ''' Retrieve the 3D Secure details
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Retrieve3dDetails(ByVal key As String) As String
            Dim value As String = ""
            If Not HttpContext.Current.Session(cache3dPrefix & key.Trim) Is Nothing Then
                value = HttpContext.Current.Session(cache3dPrefix & key.Trim)
            End If
            Return value
        End Function

        ''' <summary>
        ''' Shoul dbe replaced by the below method
        ''' </summary>
        ''' <param name="paymentKey"></param>
        ''' <param name="checkoutPage"></param>
        ''' <param name="orCurrentUsed"></param>
        ''' <param name="orLastUsed"></param>
        ''' <param name="paymentType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrievePaymentItem(ByVal paymentKey As String, _
                                                    ByVal checkoutPage As String, _
                                                    Optional ByVal orCurrentUsed As Boolean = False, _
                                                    Optional ByVal orLastUsed As Boolean = False, _
                                                    Optional ByVal paymentType As String = "CC") As String

            def = defaults.GetDefaults
            Dim paymentItem As String = String.Empty

            'Construct the last payment keys
            Dim lastPayKey As String = lastPaymentCachePrefix
            Select Case paymentType
                Case Is = "CC" : lastPayKey += "CCDetails"
                Case Is = "DD" : lastPayKey += "DDDetails"
                Case Is = "CF" : lastPayKey += "CFDetails"
            End Select
            Dim currentPayKey As String = lastPaymentCachePrefix & UCase(Utilities.GetCurrentPageName.Trim)

            'Do the payment details exist
            If Not HttpContext.Current.Session(paymentDetailsCachePrefix & checkoutPage.Trim) Is Nothing Or _
                (orCurrentUsed = True AndAlso Not HttpContext.Current.Session(currentPayKey) Is Nothing) Or _
                (orLastUsed = True AndAlso Not HttpContext.Current.Session(lastPayKey) Is Nothing) Then

                'Retrieve the payment details
                Dim payDetails As Generic.Dictionary(Of String, String)
                If Not HttpContext.Current.Session(paymentDetailsCachePrefix & checkoutPage.Trim) Is Nothing Then
                    payDetails = HttpContext.Current.Session(paymentDetailsCachePrefix & checkoutPage.Trim)
                Else
                    If Not HttpContext.Current.Session(currentPayKey) Is Nothing Then
                        payDetails = HttpContext.Current.Session(currentPayKey)
                    Else
                        payDetails = HttpContext.Current.Session(lastPayKey)
                    End If
                End If

                'Retrieve the payment item
                If payDetails.ContainsKey(paymentKey) Then
                    paymentItem = DecodePaymentDetails(payDetails.Item(paymentKey))
                End If

                'Format the date fields
                If paymentKey = "ExpiryDate" Or paymentKey = "StartDate" Then
                    paymentItem = FormatDate(paymentItem)
                End If
            End If

            Return paymentItem

        End Function

        ''' <summary>
        ''' Replaces the above. Retrieve the payment item from session
        ''' </summary>
        ''' <param name="paymentKey">key in pay details collection as a string</param>
        ''' <param name="checkoutStage">checkout stage (PPS1/PPS2/CHECKOUT.ASPX) as a string</param>
        ''' <returns>The payment item value for the given key</returns>
        ''' <remarks></remarks>
        Public Shared Function RetrievePaymentItemFromSession(ByVal paymentKey As String, ByVal checkoutStage As String) As String
            Dim paymentItem As String = String.Empty

            'Do the payment details exist
            If Not HttpContext.Current.Session(checkoutStage) Is Nothing Then
                Dim payDetails As Generic.Dictionary(Of String, String)
                payDetails = HttpContext.Current.Session(checkoutStage)

                'Retrieve the payment item
                If payDetails.ContainsKey(paymentKey) Then
                    paymentItem = DecodePaymentDetails(payDetails.Item(paymentKey))
                End If

                'Format the date fields
                If paymentKey = "ExpiryDate" Or paymentKey = "StartDate" Then
                    paymentItem = FormatDate(paymentItem)
                End If
            End If
            Return paymentItem
        End Function

        Private Shared Function FormatDate(ByVal sDate As String) As String
            sDate = sDate.Replace(" ", String.Empty)
            Dim mm As String = String.Empty
            If sDate.Length = 4 Then
                mm = sDate.Substring(0, 2)
            Else
                mm = sDate.PadLeft(6, "0").Substring(0, 2)
            End If
            Dim yy As String = sDate.PadLeft(6, "0").Substring(4, 2)
            If mm.Equals(GlobalConstants.CARD_DDL_INITIAL_VALUE.Trim()) Then
                mm = "00"
            End If
            If yy.Equals(GlobalConstants.CARD_DDL_INITIAL_VALUE.Trim()) Then
                yy = "00"
            End If
            Return mm & yy
        End Function

        ''' <summary>
        ''' Replaces the above method to retreive payment details
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrievePaymentDetailsFromSession() As Generic.Dictionary(Of String, Talent.Common.DEPayments)
            Dim payDetails As New Generic.Dictionary(Of String, Talent.Common.DEPayments)
            Dim paymentStages As New Collection
            paymentStages.Add(GlobalConstants.PPS1STAGE)
            paymentStages.Add(GlobalConstants.PPS2STAGE)
            paymentStages.Add(GlobalConstants.CHECKOUTASPXSTAGE)

            For Each stage As String In paymentStages
                If Not HttpContext.Current.Session(stage) Is Nothing Then

                    'Populate the global fields
                    Dim dePayment As New Talent.Common.DEPayments
                    With dePayment
                        .SessionId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                        .PaymentType = RetrievePaymentItemFromSession("PaymentType", stage)
                        .Source = GlobalConstants.SOURCE
                    End With

                    'Populate the payment type specific fields
                    Select Case dePayment.PaymentType
                        Case Is = GlobalConstants.CCPAYMENTTYPE
                            With dePayment
                                .CardType = RetrievePaymentItemFromSession("CardType", stage)
                                .CardNumber = RetrievePaymentItemFromSession("CardNumber", stage)
                                .ExpiryDate = RetrievePaymentItemFromSession("ExpiryDate", stage)
                                .StartDate = RetrievePaymentItemFromSession("StartDate", stage)
                                .IssueNumber = RetrievePaymentItemFromSession("IssueNumber", stage)
                                .CV2Number = RetrievePaymentItemFromSession("CV2Number", stage)
                                .CardHolderName = RetrievePaymentItemFromSession("CardHolderName", stage)
                                .Installments = RetrievePaymentItemFromSession("Installments", stage)
                                .SaveMyCardMode = RetrievePaymentItemFromSession("SaveTheseCardDetails", stage)
                                .TokenID = RetrievePaymentItemFromSession("VGTokenID", stage)
                                .ProcessingDB = RetrievePaymentItemFromSession("VGProcessingDB", stage)
                                .TransactionID = RetrievePaymentItemFromSession("TransactionID", stage)
                                If HttpContext.Current.Session("ExternalGatewayPayType") IsNot Nothing AndAlso HttpContext.Current.Session("ExternalGatewayPayType").ToString() = GlobalConstants.PAYMENTTYPE_VANGUARD Then
                                    .PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
                                End If
                            End With

                        Case Is = GlobalConstants.SAVEDCARDPAYMENTTYPE
                            With dePayment
                                .CardNumber = RetrievePaymentItemFromSession("CardNumber", stage)
                                .ExpiryDate = RetrievePaymentItemFromSession("ExpiryDate", stage)
                                .StartDate = RetrievePaymentItemFromSession("StartDate", stage)
                                .IssueNumber = RetrievePaymentItemFromSession("IssueNumber", stage)
                                .CV2Number = RetrievePaymentItemFromSession("CV2Number", stage)
                                .LastFourCardDigits = RetrievePaymentItemFromSession("LastFourCardDigits", stage)
                                .UniqueCardId = RetrievePaymentItemFromSession("UniqueCardId", stage)
                                .SaveMyCardMode = RetrievePaymentItemFromSession("SaveTheseCardDetails", stage)
                                .TokenID = RetrievePaymentItemFromSession("VGTokenID", stage)
                                .ProcessingDB = RetrievePaymentItemFromSession("VGProcessingDB", stage)
                                .TransactionID = RetrievePaymentItemFromSession("TransactionID", stage)
                                .PaymentType = GlobalConstants.CCPAYMENTTYPE 'A saved card payment type is still "CC" in TALENT, overwrite it here
                                If HttpContext.Current.Session("ExternalGatewayPayType") IsNot Nothing AndAlso HttpContext.Current.Session("ExternalGatewayPayType").ToString() = GlobalConstants.PAYMENTTYPE_VANGUARD Then
                                    .PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
                                End If
                            End With

                        Case Is = GlobalConstants.DDPAYMENTTYPE
                            With dePayment
                                .AccountName = RetrievePaymentItemFromSession("AccountName", stage)
                                .SortCode = RetrievePaymentItemFromSession("SortCode", stage)
                                .AccountNumber = RetrievePaymentItemFromSession("AccountNumber", stage)
                                .Originator = RetrievePaymentItemFromSession("Originator", stage)
                                .DDIReference = RetrievePaymentItemFromSession("DDIReference", stage)
                                .PaymentDay = RetrievePaymentItemFromSession("PaymentDay", stage)
                                .Bank = RetrievePaymentItemFromSession("BankName", stage)
                            End With

                        Case Is = GlobalConstants.CFPAYMENTTYPE
                            With dePayment
                                .AccountName = RetrievePaymentItemFromSession("AccountName", stage)
                                .SortCode = RetrievePaymentItemFromSession("SortCode", stage)
                                .AccountNumber = RetrievePaymentItemFromSession("AccountNumber", stage)
                                .PaymentOptionCode = RetrievePaymentItemFromSession("InstallmentPlanCode", stage)
                                .Bank = RetrievePaymentItemFromSession("BankName", stage)
                                .YearsAtAddress = RetrievePaymentItemFromSession("YearsAtAddress", stage)
                                .clubProductCodeForFinance = RetrievePaymentItemFromSession("clubProductCodeForFinance", stage)
                                .MonthsAtAddress = RetrievePaymentItemFromSession("MonthsAtAddress", stage)
                                .YearsAtAddress = RetrievePaymentItemFromSession("YearsAtAddress", stage)
                                .HomeStatus = RetrievePaymentItemFromSession("HomeStatus", stage)
                                .EmploymentStatus = RetrievePaymentItemFromSession("EmploymentStatus", stage)
                                .GrossIncome = RetrievePaymentItemFromSession("GrossIncome", stage)
                            End With

                        Case Is = GlobalConstants.EPURSEPAYMENTTYPE
                            With dePayment
                                Dim giftCard As Boolean = CType(RetrievePaymentItemFromSession("IsGiftCard", stage), Boolean)
                                If giftCard Then
                                    .CardNumber = RetrievePaymentItemFromSession("GCCardNumber", stage)
                                Else
                                    .CardNumber = RetrievePaymentItemFromSession("CardNumber", stage)
                                End If
                                .PIN = RetrievePaymentItemFromSession("PIN", stage)
                                .Currency = RetrievePaymentItemFromSession("Currency", stage)
                            End With

                        Case Is = GlobalConstants.CHIPANDPINPAYMENTTYPE
                            With dePayment
                                .ChipAndPinIPAddress = RetrievePaymentItemFromSession("IPAddress", stage)
                            End With

                        Case Is = GlobalConstants.POINTOFSALEPAYMENTTYPE
                            With dePayment
                                .PointOfSaleIPAddress = RetrievePaymentItemFromSession("IPAddress", stage)
                                .PointOfSaleTCPPort = RetrievePaymentItemFromSession("TCPPort", stage)
                            End With

                        Case Is = GlobalConstants.OTHERSPAYMENTTYPE
                            With dePayment
                                .PaymentType = RetrievePaymentItemFromSession("OtherPaymentType", stage)
                                .OptionTokencode = RetrievePaymentItemFromSession("OtherTextValue", stage)
                            End With

                    End Select
                    payDetails.Add(stage, dePayment)
                End If
            Next
            Return payDetails
        End Function

        ''' <summary>
        ''' Remove payment details from session
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Sub RemovePaymentDetailsFromSession()
            Dim payDetails As New Generic.Dictionary(Of String, Talent.Common.DEPayments)
            Dim paymentStages As New Collection
            paymentStages.Add(GlobalConstants.PPS1STAGE)
            paymentStages.Add(GlobalConstants.PPS2STAGE)
            paymentStages.Add(GlobalConstants.CHECKOUTASPXSTAGE)
            For Each stage As String In paymentStages
                HttpContext.Current.Session.Remove(stage)
            Next
        End Sub

        ''' <summary>
        ''' Check the basket against the basket in session
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CheckBasketValidity() As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Dim valid As Boolean = True
            If Not HttpContext.Current.Session("CheckoutBasketState") Is Nothing Then
                Dim testBasket As TalentBasket = CType(HttpContext.Current.Session("CheckoutBasketState"), TalentBasket)
                Dim currentBasket As TalentBasket = CType(HttpContext.Current.Profile, TalentProfile).Basket
                If Not testBasket Is Nothing AndAlso Not currentBasket Is Nothing Then
                    Dim testBasketCount As Integer = 0
                    For Each tbi As TalentBasketItem In testBasket.BasketItems
                        If Not tbi.IS_FREE Then
                            If Not Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product, tbi.FEE_CATEGORY) Then testBasketCount += 1
                        End If
                    Next
                    Dim currentBasketCount As Integer = 0
                    For Each tbi As TalentBasketItem In currentBasket.BasketItems
                        If Not tbi.IS_FREE Then
                            If Not Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product, tbi.FEE_CATEGORY) Then currentBasketCount += 1
                        End If
                    Next

                    If testBasketCount = currentBasketCount Then
                        Dim found As Boolean = False
                        For Each ctbi As TalentBasketItem In currentBasket.BasketItems
                            found = False
                            If ctbi.IS_FREE Or Utilities.IsTicketingFee(ctbi.MODULE_, ctbi.Product, ctbi.FEE_CATEGORY) Then
                                found = True
                            Else
                                For Each ttbi As TalentBasketItem In testBasket.BasketItems
                                    If ctbi.Product = ttbi.Product Then
                                        If ctbi.MODULE_.ToUpper = "TICKETING" Then
                                            If ctbi.SEAT = ttbi.SEAT Then
                                                found = True
                                            End If
                                        Else
                                            If ctbi.Quantity = ttbi.Quantity Then
                                                found = True
                                            Else
                                                valid = False
                                            End If
                                        End If
                                    End If
                                    If found Or Not valid Then
                                        Exit For
                                    End If
                                Next
                                If Not found Then valid = False
                                If Not valid Then Exit For
                            End If
                            If Not found Then valid = False
                            If Not valid Then Exit For
                        Next
                    Else
                        valid = False
                    End If
                Else
                    valid = False
                End If
            Else
                valid = False
            End If

            If valid Then
                Dim reqTot As Decimal = CType(HttpContext.Current.Profile, TalentProfile).GetMinimumPurchaseAmount
                Dim tot As Decimal = 0

                If reqTot > 0 Then
                    For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                        If Not tbi.IS_FREE Then
                            tot += (tbi.Gross_Price * tbi.Quantity)
                            If tot >= reqTot Then Exit For
                        End If
                    Next
                    If tot < reqTot Then
                        HttpContext.Current.Session.Remove("CheckoutBreadCrumbTrail")
                        HttpContext.Current.Session("TalentErrorCode") = "AmountTooLow"
                        HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    End If
                End If
            Else
                HttpContext.Current.Session.Remove("CheckoutBreadCrumbTrail")
                HttpContext.Current.Session("TalentErrorCode") = "TEPCBV-1"
                HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If
            Utilities.TalentLogging.LoadTestLog("Checkout.vb", "CheckBasketValidity", timeSpan)
            Return valid
        End Function

        Public Shared Function EncodePaymentDetails(ByVal value As String) As String
            If def.PAYMENT_ENCRYPTION_KEY Is Nothing Then
                def = defaults.GetDefaults
            End If
            'Encode the payment details when an encryption key is present
            If def.PAYMENT_ENCRYPTION_KEY.Trim <> "" Then
                value = Talent.Common.Utilities.TripleDESEncode(value, def.PAYMENT_ENCRYPTION_KEY)
            End If
            Return value
        End Function

        Public Shared Function DecodePaymentDetails(ByVal value As String) As String
            If def.PAYMENT_ENCRYPTION_KEY Is Nothing Then
                def = defaults.GetDefaults
            End If
            'Decode the payment details when an encryption key is present
            If def.PAYMENT_ENCRYPTION_KEY.Trim <> "" Then
                value = Talent.Common.Utilities.TripleDESDecode(value, def.PAYMENT_ENCRYPTION_KEY)
            End If
            Return value
        End Function

        'Public Shared Function DDPaymentUsed() As Boolean
        '    DDPaymentUsed = False
        '    Dim dt As DataTable = CheckoutStages()

        '    'Is this the final checkout stage
        '    If dt.Rows.Count > 0 AndAlso UCase(dt.Rows(dt.Rows.Count - 1).Item("PAGE_CODE").ToString.Trim) = UCase(Utilities.GetCurrentPageName.Trim) Then
        '        Dim loopIndex As Integer = 0
        '        'loop through the checkout stages
        '        For loopIndex = 0 To dt.Rows.Count - 1
        '            'Have we taken payment with direct debit for this stage
        '            If RetrievePaymentItem("PaymentType", dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim).ToString.Trim = "DD" Then
        '                DDPaymentUsed = True
        '                Exit For
        '            End If
        '        Next
        '    End If
        '    Return DDPaymentUsed
        'End Function

        'Public Shared Function CFPaymentUsed() As Boolean
        '    CFPaymentUsed = False
        '    Dim dt As DataTable = CheckoutStages()

        '    'Is this the final checkout stage
        '    If dt.Rows.Count > 0 AndAlso UCase(dt.Rows(dt.Rows.Count - 1).Item("PAGE_CODE").ToString.Trim) = UCase(Utilities.GetCurrentPageName.Trim) Then
        '        Dim loopIndex As Integer = 0
        '        'loop through the checkout stages
        '        For loopIndex = 0 To dt.Rows.Count - 1
        '            'Have we taken payment with direct debit for this stage
        '            If RetrievePaymentItem("PaymentType", dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim).ToString.Trim = "CF" Then
        '                CFPaymentUsed = True
        '                Exit For
        '            End If
        '        Next
        '    End If
        '    Return CFPaymentUsed
        'End Function

        'Public Shared Function CCPaymentUsed() As Boolean
        '    CCPaymentUsed = False
        '    Dim dt As DataTable = CheckoutStages()

        '    'Is this the final checkout stage
        '    If dt.Rows.Count > 0 AndAlso UCase(dt.Rows(dt.Rows.Count - 1).Item("PAGE_CODE").ToString.Trim) = UCase(Utilities.GetCurrentPageName.Trim) Then
        '        Dim loopIndex As Integer = 0
        '        'loop through the checkout stages
        '        For loopIndex = 0 To dt.Rows.Count - 1
        '            'Have we taken payment with direct debit for this stage
        '            If RetrievePaymentItem("PaymentType", dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim).ToString.Trim = "CC" Then
        '                CCPaymentUsed = True
        '                Exit For
        '            End If
        '        Next
        '    End If
        '    Return CCPaymentUsed
        'End Function

        'Public Shared Function PPSUsed() As Boolean
        '    PPSUsed = False
        '    Dim dt As DataTable = CheckoutStages()

        '    'Is this the final checkout stage
        '    If dt.Rows.Count > 0 AndAlso UCase(dt.Rows(dt.Rows.Count - 1).Item("PAGE_CODE").ToString.Trim) = UCase(Utilities.GetCurrentPageName.Trim) Then
        '        Dim loopIndex As Integer = 0
        '        'loop through the checkout stages
        '        For loopIndex = 0 To dt.Rows.Count - 1
        '            'Have we taken payment for any schemes
        '            If dt.Rows(loopIndex).Item("PRODUCT_TYPE").ToString.Trim = "P" Then
        '                If RetrievePaymentItem("PaymentType", dt.Rows(loopIndex).Item("PAGE_CODE").ToString.Trim).ToString.Trim <> String.Empty Then
        '                    PPSUsed = True
        '                    Exit For
        '                End If
        '            End If
        '        Next
        '    End If
        '    Return PPSUsed
        'End Function

        ''' <summary>
        ''' Retrieve the payment type
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RetrieveCurrentPaymentType() As String
            Dim payType As String = String.Empty
            If Not HttpContext.Current.Session(GlobalConstants.CHECKOUTASPXSTAGE) Is Nothing Then
                payType = RetrievePaymentItemFromSession("PaymentType", GlobalConstants.CHECKOUTASPXSTAGE)
            End If
            If payType.Length = 0 Then
                If HttpContext.Current.Session(currentPaymentType) IsNot Nothing Then
                    payType = HttpContext.Current.Session(currentPaymentType)
                End If
            End If
            Return payType
        End Function

        ''' <summary>
        ''' Store the payment type
        ''' </summary>
        ''' <param name="payType"></param>
        ''' <remarks></remarks>
        Public Shared Sub StoreCurrentPaymentType(ByVal payType As String)
            HttpContext.Current.Session(currentPaymentType) = payType
        End Sub

        Public Shared Function MerchandiseTotalFromOrderHeader() As Decimal
            Dim amount As Decimal = 0
            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Try
                        '----------------------------------------
                        ' Get merchandise amount off order header
                        '----------------------------------------

                        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                        Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, CType(HttpContext.Current.Profile, TalentProfile).UserName)

                        If dt.Rows.Count > 0 Then
                            Dim total As Decimal = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
                            Dim promo As Decimal = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))

                            amount = total
                            If Not def.Call_Tax_WebService Then
                                amount = total - promo
                            End If
                        End If
                    Catch ex As Exception
                    End Try
            End Select
            Return Talent.eCommerce.Utilities.RoundToValue(amount, 0.01, False)
        End Function

        Public Shared Function FinaliseOrder(ByVal PaymentType As String, Optional ByRef webOrderID As String = "") As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay
            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Try
                        Dim _TDataObjects As New Talent.Common.TalentDataObjects
                        _TDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                        Dim orderDets As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

                        UpdateOrderStatus("PAYMENT ACCEPTED")
                        'Update the order with the total for the merchandise products
                        Try
                            orders.Set_Total_Amount_Charged(Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), _
                                                            Now, _
                                                            MerchandiseTotalFromOrderHeader, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).UserName)
                        Catch ex As Exception
                            Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "UCCPTP-010", ex.Message, "Error setting total amount charged on order header - TempOrderID: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                        End Try

                        'Complete the order
                        webOrderID = GenNewOrderID()
                        For i As Integer = 1 To 3
                            Try
                                orders.Complete_Order(webOrderID, Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), Now, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                                orderDets.UpdateHeaderOrderId(webOrderID, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                                If i > 1 Then Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                "UCCPFO-030", _
                                                                String.Format("Finalise order failed initially but was completed at attempt {0}", i.ToString), _
                                                                String.Format("Order successfully finalised at attempt {0} - TempOrderID: {1} - WebOrderID: {2}", i.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, webOrderID), _
                                                                TalentCache.GetBusinessUnit, _
                                                                TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
                                                                ProfileHelper.GetPageName, _
                                                                "OrderSummary.ascx")
                                Exit For
                            Catch ex As Exception
                                If i = 3 Then
                                    Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "UCCPFO-020", ex.Message, String.Format("Error finalising order, attempt {0} - TempOrderID: {1}", i.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                                Else
                                    System.Threading.Thread.Sleep(50)
                                End If
                            End Try
                        Next

                        'Update payment type
                        Dim paymentTypeDescription As String = String.Empty
                        paymentTypeDescription = _TDataObjects.PaymentSettings.TblPaymentTypeLang.GetDescriptionByTypeAndLang(PaymentType, Utilities.GetCurrentLanguage())
                        If paymentTypeDescription.Length = 0 Then paymentTypeDescription = "Credit Card"
                        orders.SetPaymentType(paymentTypeDescription, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)

                        'If Order is collected, the date and quantity shipped fields need to be updated
                        Dim dtOrderHeader As DataTable = orders.Get_Header_By_Temp_Order_Id(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                        If String.Equals("COLLECTED", dtOrderHeader.Rows(0)("DELIVERY_TYPE").ToString()) Then
                            Dim updatedProducts As Integer
                            updatedProducts = _TDataObjects.OrderSettings.TblOrderDetail.UpdateShippedProductsByTempOrderID(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                        End If

                        'Update the temp order id
                        UpdateTempOrderIDOnBasket()
                        UpdateOrderStatus("ORDER COMPLETE")
                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "CPD-0001", "Order Created... Transfering to CheckoutOrderConfirmation", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), "CheckoutPaymentDetails")

                    Catch ex As Exception
                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "UCCPFO-010", ex.Message, "Error marking order header as finalised - TempOrderID: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                        Return False
                    End Try
            End Select
            '
            ' Dont remove the payment details from the session if it's a credit Finance payment
            ' We need the details for the summary page
            '
            If PaymentType <> "CF" Then
                Checkout.RemovePaymentDetailsFromSession()
            End If

            Utilities.TalentLogging.LoadTestLog("Checkout.vb", "FinaliseOrder", timeSpan)
            Return True
        End Function

        Public Shared Function GenNewOrderID() As String
            'Start MBSTEB-1304
            Dim prefix As String = String.Empty
            Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            If def.WebOrderNumberPrefixOverride.ToString.Trim.Length > 0 Then
                prefix = def.WebOrderNumberPrefixOverride.ToString.Trim
            Else
                prefix = TalentCache.GetBusinessUnit
            End If
            If Utilities.IsBasketHomeDelivery(HttpContext.Current.Profile) Then
                prefix = "H" + prefix
            End If
            Return prefix & Talent.Common.Utilities.GetNextOrderNumber(TalentCache.GetBusinessUnit, _
                                    TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
                                    ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
            '            Return TalentCache.GetBusinessUnit & Talent.Common.Utilities.GetNextOrderNumber(TalentCache.GetBusinessUnit, _
            '                                    TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
            '                                    ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
            'End   MBSTEB-1304
        End Function

        Public Shared Sub UpdateTempOrderIDOnBasket()
            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Try
                        Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
                        For i As Integer = 1 To 3
                            Try
                                basket.Update_Temp_order_id(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                                Exit For
                            Catch ex As Exception
                                If i = 3 Then
                                    Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "UCCPUB-010", ex.Message, String.Format("Error finalising basket, attempt {0} - TempOrderID: {1}", i.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                                Else
                                    System.Threading.Thread.Sleep(50)
                                End If
                            End Try
                        Next
                    Catch ex As Exception
                    End Try
            End Select
        End Sub

        Public Shared Sub InsertOrderStatusForExternalPay(ByVal StatusName As String, Optional ByVal comment As String = "")

            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M"
                    Try
                        Dim orderStatus As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                        orderStatus.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                            Now, _
                                                            comment)
                    Catch ex As Exception
                    End Try
                Case "T"
                    Try
                        Dim basketStatus As New TalentBasketDatasetTableAdapters.tbl_basket_statusTableAdapter
                        basketStatus.Insert_Basket_Status_Flow(TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                            Now, _
                                                            comment)
                    Catch ex As Exception
                    End Try
                Case "C"
                    Try
                        Dim orderStatus As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                        orderStatus.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                            Now, _
                                                            comment)

                        Dim basketStatus As New TalentBasketDatasetTableAdapters.tbl_basket_statusTableAdapter
                        basketStatus.Insert_Basket_Status_Flow(TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                            Now, _
                                                            comment)
                    Catch ex As Exception
                    End Try
            End Select

        End Sub

        Public Shared Sub UpdateOrderStatusForExternalPay(ByVal StatusName As String, Optional ByVal comment As String = "")

            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Try
                        'Add default comment when blank
                        If comment.Trim.Equals("") Then
                            Select Case StatusName
                                Case Is = "ORDER FAILED" : comment = "Payment recieved but order creation failed"
                            End Select
                        End If

                        'Update order status
                        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                        orders.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                                Now, _
                                                                TalentCache.GetBusinessUnit, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                    Catch ex As Exception
                    End Try
                    InsertOrderStatusForExternalPay(StatusName, comment)
                Case "T"
                    InsertOrderStatusForExternalPay(StatusName, comment)
            End Select

        End Sub

        Public Shared Sub InsertOrderStatusFlow(ByVal StatusName As String, Optional ByVal comment As String = "")

            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Try
                        Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                        status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                            Now, _
                                                            comment)
                    Catch ex As Exception
                    End Try
            End Select

        End Sub

        ''' <summary>
        ''' Update the order status table
        ''' </summary>
        ''' <param name="StatusName"></param>
        ''' <param name="comment"></param>
        ''' <remarks></remarks>
        Public Shared Sub UpdateOrderStatus(ByVal StatusName As String, Optional ByVal comment As String = "")

            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Try
                        'Add default comment when blank
                        If comment.Trim.Equals("") Then
                            Select Case StatusName
                                Case Is = "ORDER FAILED" : comment = "Payment recieved but order creation failed"
                            End Select
                        End If

                        'Update order status
                        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                        orders.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                                Now, _
                                                                TalentCache.GetBusinessUnit, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                    Catch ex As Exception
                    End Try
                    InsertOrderStatusFlow(StatusName, comment)
            End Select

        End Sub

        Public Shared Function RetrieveAccountEnd() As String
            'Retrieve the account end of the order history
            Dim dePaymentsCollection As New Generic.Dictionary(Of String, Talent.Common.DEPayments)
            dePaymentsCollection = RetrievePaymentDetailsFromSession()
            Dim de As Talent.Common.DEPayments = dePaymentsCollection.Item(GlobalConstants.CHECKOUTASPXSTAGE)
            Dim accEnd As String = ""
            If de.PaymentType.Equals("CS") Then
                accEnd = "Cash"
            Else
                If de.PaymentType.Equals("CC") Then
                    If de.CardNumber.Length > 4 Then
                        accEnd = de.CardNumber.Substring(de.CardNumber.Length - 4)
                    End If
                Else
                    If de.AccountNumber.Length > 3 Then
                        accEnd = de.AccountNumber.Substring(de.AccountNumber.Length - 3)
                    End If
                End If
            End If
            Return accEnd
        End Function

        Public Shared Function is3DSecureInUse(ByVal isAgent As Boolean, ByVal basketContentType As String, ByVal moduleDefaults As Talent.Common.DEEcommerceModuleDefaults.DefaultValues) As Boolean
            Dim process3DSecure As Boolean = False
            If isAgent Then
                If moduleDefaults.PaymentProcess3dSecureForAgents AndAlso moduleDefaults.Payment3DSecureProvider.Length > 0 Then
                    process3DSecure = True
                End If
            Else
                If moduleDefaults.PaymentProcess3dSecure AndAlso moduleDefaults.Payment3DSecureProvider.Length > 0 Then
                    process3DSecure = True
                End If
            End If
            Return process3DSecure
        End Function

    End Class

End Namespace
