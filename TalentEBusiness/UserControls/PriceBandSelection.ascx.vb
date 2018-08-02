Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data
Imports System.Web.UI
Imports System.Collections.Generic

Partial Class UserControls_PriceBandSelection
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _ucr As New UserControlResource
    Private _quantityDropDownMaxValue As Integer = 0

#End Region

#Region "Public Properties"

    Public Property ProductCode() As String
    Public Property ProductType() As String
    Public Property ProductSubType() As String
    Public Property ProductDetailCode() As String
    Public Property PriceCode() As String
    Public Property QuantityAvailable() As Integer
    Public Property DSProductDetail() As DataSet
    Public Property ProductStadium() As String
    Public Property DescriptionHeaderText() As String
    Public Property PriceHeaderText() As String
    Public Property QuantityHeaderText() As String
    Public Property HasAssociatedTravelProduct() As Boolean
    Public Property IsLinkedProduct As Boolean

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = TEBUtilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PriceBandSelection.ascx"
        End With
    End Sub

    Protected Sub checkPriceBandList(ByVal productDefaultPriceBand As String, ByVal productAllowPBAlterations As String, ByVal productPriceBandMode As String)

        'Restricting PWS Price Band options at product level (customer or product default PB)
        If Not AgentProfile.IsAgent And productAllowPBAlterations = GlobalConstants.PRICE_BAND_ALTERATIONS_RESTRICTED Then
            Dim singlePriceBand As String = productDefaultPriceBand
            Dim customerPriceBand As String = String.Empty

            If productPriceBandMode = GlobalConstants.PRICE_BAND_DEFAULT_CUSTOMER AndAlso Not Profile.IsAnonymous AndAlso Profile.User.Details IsNot Nothing AndAlso Profile.User.Details.PRICE_BAND <> String.Empty Then
                customerPriceBand = Profile.User.Details.PRICE_BAND
            End If

            'Loop 1 - Check if we can match the customer price band if we are attempting to
            ' Price in customer mode.
            If customerPriceBand <> String.Empty Then
                For Each item As RepeaterItem In rptPriceBandSelection.Items
                    Dim hdfPriceBand As HiddenField = CType(item.FindControl("hdfPriceBand"), HiddenField)
                    If hdfPriceBand.Value = customerPriceBand Then
                        singlePriceBand = customerPriceBand
                    End If
                Next
            End If

            'Loop 2 - Hide all items apart from the single item allowed (product or customer)
            For Each item As RepeaterItem In rptPriceBandSelection.Items
                Dim hdfPriceBand As HiddenField = CType(item.FindControl("hdfPriceBand"), HiddenField)
                If hdfPriceBand.Value <> singlePriceBand Then
                    item.Visible = False
                End If
            Next
        End If
    End Sub
    Protected Sub rptPriceBandSelection_ItemDataBound(ByVal sender As Object, ByVal e As WebControls.RepeaterItemEventArgs) Handles rptPriceBandSelection.ItemDataBound
        If e.Item.ItemType = ListItemType.Header Then
            DescriptionHeaderText = _ucr.Content("DescriptionHeaderText", _languageCode, True)
            PriceHeaderText = _ucr.Content("PriceHeaderText", _languageCode, True)
            QuantityHeaderText = _ucr.Content("QuantityHeaderText", _languageCode, True)
        ElseIf e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim dtRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
            CType(e.Item.FindControl("hdfPriceBand"), HiddenField).Value = dtRow("PriceBand").ToString.Trim
            Dim ltlDescriptionText As Literal = CType(e.Item.FindControl("ltlDescriptionText"), Literal)
            ltlDescriptionText.Text = dtRow("PriceBandDescription").ToString.Trim
            'Add the availablity to the quantity drop down list
            'If PriceBandMinQuantity/PriceBandMaxQuantity is being used list only values in acordance with what is set
            Dim listQuantityWithoutRatio As Boolean = True
            Dim familyPriceMultiplyer As Integer = 0
            Dim ddlQuantity As DropDownList = CType(e.Item.FindControl("ddlQuantity"), DropDownList)
            Dim txtBulkSalesQuantity As TextBox = CType(e.Item.FindControl("txtBulkSalesQuantity"), TextBox)
            Dim rgxBulkSalesQuantity As RegularExpressionValidator = CType(e.Item.FindControl("rgxBulkSalesQuantity"), RegularExpressionValidator)
            Dim hdfQuantityMultiplier As HiddenField = CType(e.Item.FindControl("hdfQuantityMultiplier"), HiddenField)
            Dim priceBandValue As Decimal = 0
            priceBandValue = CCurrency(dtRow("PriceBandValue").ToString.Trim)
            Try
                CType(e.Item.FindControl("ltlPriceText"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(priceBandValue, _ucr.BusinessUnit, _ucr.PartnerCode)
            Catch ex As Exception
                CType(e.Item.FindControl("ltlPriceText"), Literal).Text = priceBandValue
            End Try

            If AgentProfile.BulkSalesMode Then
                ddlQuantity.Visible = False
                rgxBulkSalesQuantity.Enabled = True
                rgxBulkSalesQuantity.ValidationExpression = "^(0|[1-9][0-9]*)$"
                rgxBulkSalesQuantity.ErrorMessage = _ucr.Content("BulkSalesInvalidQuantityError", _languageCode, True).Replace("<<PRICE_BAND>>", ltlDescriptionText.Text)
            Else
                txtBulkSalesQuantity.Visible = False
                rgxBulkSalesQuantity.Visible = False
                ddlQuantity.Items.Add(0)
                If Not String.IsNullOrEmpty(dtRow("PriceBandMinQuantity").ToString.Trim) And Not String.IsNullOrEmpty(dtRow("PriceBandMaxQuantity")) Then
                    Try
                        listQuantityWithoutRatio = False
                        Dim PriceBandMinQuantity As Integer = CInt(dtRow("PriceBandMinQuantity").ToString.Trim)
                        Dim PriceBandMaxQuantity As Integer = CInt(dtRow("PriceBandMaxQuantity").ToString.Trim)
                        Dim PriceBandIsFamilyType As String = CStr(dtRow("PriceBandIsFamilyType").ToString.Trim)
                        If PriceBandIsFamilyType = "Y" Or PriceBandIsFamilyType = "y" Then
                            familyPriceMultiplyer = PriceBandMinQuantity
                            hdfQuantityMultiplier.Value = PriceBandMinQuantity
                            listQuantityWithoutRatio = True
                        Else
                            'Assign default min and max value for zero priced band if backend send
                            ' pricebandvalue = 0 ; min=0 ; max = 0
                            If (priceBandValue <= 0) AndAlso (PriceBandMinQuantity <= 0) AndAlso (PriceBandMaxQuantity <= 0) Then
                                PriceBandMinQuantity = 0
                                PriceBandMaxQuantity = _quantityDropDownMaxValue
                            End If
                            'Build the quantity drop down box based on "group" tickets. Eg. use the max and min values,
                            'start the count at the min value and only go upto the max value.
                            'Check to see the if max value is higher than the quantity available, as we only want to go as high as the availability

                            For i As Integer = 1 To PriceBandMaxQuantity
                                If i >= PriceBandMinQuantity Then
                                    ddlQuantity.Items.Add(i)
                                End If
                            Next
                            hdfQuantityMultiplier.Value = 0
                        End If
                    Catch ex As Exception
                        listQuantityWithoutRatio = True
                    End Try
                End If

                If listQuantityWithoutRatio Then
                    For i As Integer = 1 To _quantityDropDownMaxValue
                        ddlQuantity.Items.Add(i)
                    Next
                End If
            End If
            End If
    End Sub

    Protected Sub btnAddToBasket_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddToBasket.Click
        'Loop through the quantity drop down boxes and see if the total is greater than 0
        'If the quantity is greater than 1 for any price value, add the price and quantity to an array
        Dim quantityTotal As Integer = 0
        Dim priceBandArray(25, 1) As String
        Dim priceBandArrayIndex As Integer = 0
        For i As Integer = 0 To rptPriceBandSelection.Items.Count - 1
            Dim selectedValueInteger As Integer = 0
            Try
                If AgentProfile.BulkSalesMode Then
                    selectedValueInteger = TEBUtilities.CheckForDBNull_Int(CType(rptPriceBandSelection.Items(i).FindControl("txtBulkSalesQuantity"), TextBox).Text)
                Else
                    selectedValueInteger = CInt(CType(rptPriceBandSelection.Items(i).FindControl("ddlQuantity"), DropDownList).SelectedValue)
                End If
                Dim priceBand As String = CType(rptPriceBandSelection.Items(i).FindControl("hdfPriceBand"), HiddenField).Value
                Dim hdfQuantityMultiplierValue As String = CType(rptPriceBandSelection.Items(i).FindControl("hdfQuantityMultiplier"), HiddenField).Value.Trim
                Dim quantityMultiplier As Integer = 0
                If selectedValueInteger > 0 Then
                    priceBandArray(priceBandArrayIndex, 0) = priceBand
                    If Not String.IsNullOrEmpty(hdfQuantityMultiplierValue) Then
                        quantityMultiplier = CInt(hdfQuantityMultiplierValue)
                        If quantityMultiplier > 0 Then selectedValueInteger = selectedValueInteger * quantityMultiplier
                        priceBandArray(priceBandArrayIndex, 1) = selectedValueInteger.ToString
                    Else
                        priceBandArray(priceBandArrayIndex, 1) = selectedValueInteger.ToString
                    End If
                    priceBandArrayIndex += 1
                End If
            Catch ex As Exception
            End Try
            quantityTotal = quantityTotal + selectedValueInteger
        Next

        'clean up the priceBandArray so that each entity has an empty value
        While priceBandArrayIndex < 26
            priceBandArray(priceBandArrayIndex, 0) = String.Empty
            priceBandArray(priceBandArrayIndex, 1) = String.Empty
            priceBandArrayIndex += 1
        End While

        'Handle any errors, including a check for existing errors that need to be removed (doesn't apply for membership products)
        Dim errorlist As BulletedList = CType(Me.Parent.TemplateControl.FindControl("errorlist"), BulletedList)
        If ProductType <> GlobalConstants.MEMBERSHIPPRODUCTTYPE Then

            'Clear all error message first
            errorlist.Items.Clear()

            Dim errorAlreadyListed As Boolean = False
            For Each item As ListItem In errorlist.Items
                If item.Value = _ucr.Content("ZeroQuantityErrorText", _languageCode, True) Then
                    errorAlreadyListed = True
                    If quantityTotal > 0 Then
                        errorlist.Items.Remove(item)
                        Exit For
                    End If
                End If
            Next
            If Not errorAlreadyListed Then
                If quantityTotal = 0 Then
                    errorlist.Items.Add(_ucr.Content("ZeroQuantityErrorText", _languageCode, True))
                End If
            End If
            errorAlreadyListed = False
            For Each item As ListItem In errorlist.Items
                Dim QuantityTooHighErrorText As String = _ucr.Content("QuantityTooHighErrorText", _languageCode, True)
                QuantityTooHighErrorText = QuantityTooHighErrorText.Replace("<<<Quantity>>>", _QuantityAvailable)
                If item.Value = QuantityTooHighErrorText Then
                    errorAlreadyListed = True
                    If quantityTotal <= QuantityAvailable Then
                        errorlist.Items.Remove(item)
                        Exit For
                    End If
                End If
            Next
            If Not errorAlreadyListed Then
                If quantityTotal > QuantityAvailable Then
                    Dim quantityTooHighErrorText As String = _ucr.Content("QuantityTooHighErrorText", _languageCode, True)
                    quantityTooHighErrorText = quantityTooHighErrorText.Replace("<<<Quantity>>>", QuantityAvailable)
                    errorlist.Items.Add(quantityTooHighErrorText)
                End If
            End If
        End If

        'Proceed to add the selections to the basket if no errors are listed
        If errorlist.Items.Count = 0 Then
            Dim includeTravel As String = "N"
            Dim redirectURL As New StringBuilder
            If Not Session("PriceBandSelectionOptions") Is Nothing Then
                Session("PriceBandSelectionOptions") = Nothing
            End If
            Session("PriceBandSelectionOptions") = priceBandArray
            If ModuleDefaults.ShowTravelAsDDL AndAlso _ProductType = GlobalConstants.TRAVELPRODUCTTYPE Then
                ProductDetailCode = ddlTravelOptions.SelectedValue
            End If
            If chkIncludeTravel.Checked Then includeTravel = "Y"
            redirectURL.Append("~/Redirect/TicketingGateway.aspx?page=")
            redirectURL.Append(TEBUtilities.GetCurrentPageName(False))
            redirectURL.Append("&function=AddToBasket&product=").Append(ProductCode.Trim)
            redirectURL.Append("&type=").Append(ProductType)
            redirectURL.Append("&productStadium=").Append(ProductStadium)
            redirectURL.Append("&productSubType=").Append(ProductSubType)
            redirectURL.Append("&productDetailCode=").Append(Server.UrlEncode(ProductDetailCode))
            redirectURL.Append("&includetravel=").Append(includeTravel)
            redirectURL.Append("&priceCode=").Append(PriceCode)
            Response.Redirect(redirectURL.ToString())
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Sub LoadPriceBandSelection()
        PopulateHiddenFields()

        If IsLinkedProduct Then
            btnAddToBasket.Visible = False
        End If

        If ProductType = GlobalConstants.TRAVELPRODUCTTYPE Then
            If ModuleDefaults.ShowTravelAsDDL OrElse IsLinkedProduct Then
                SetUpTravelDDL()
            End If
        End If

        If _QuantityAvailable <= 0 AndAlso ProductType <> GlobalConstants.MEMBERSHIPPRODUCTTYPE Then
            plhProductAvailable.Visible = False
            plhProductSoldOut.Visible = True
            ltlSoldOutText.Text = _ucr.Content("SoldOutText", _languageCode, True)
        Else
            plhProductAvailable.Visible = True
            plhProductSoldOut.Visible = False
        End If
        If plhProductAvailable.Visible Then
            Try
                Dim deSettings As DESettings = TEBUtilities.GetSettingsObject()
                Dim deProductDetails As New Talent.Common.DEProductDetails
                Dim product As New TalentProduct
                Dim err As New ErrorObj
                Dim dsProductDetails As New DataSet
                deSettings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
                deSettings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
                deSettings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
                deSettings.OriginatingSource = TEBUtilities.GetOriginatingSource(Session.Item("Agent"))
                If HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("Agent") IsNot Nothing Then
                    deSettings.IsAgent = True
                End If

                deProductDetails.ProductCode = ProductCode
                deProductDetails.PriceCode = PriceCode
                deProductDetails.Src = GlobalConstants.SOURCE
                deProductDetails.ProductType = ProductType
                product.Settings() = deSettings
                product.De = deProductDetails

                'Make the call to Talent, check for errors and bind the dataset to the repeater
                err = product.ProductDetails
                dsProductDetails = product.ResultDataSet
                If Not err.HasError And dsProductDetails.Tables.Count = 5 Then
                    If dsProductDetails.Tables(0).Rows(0)("ErrorOccurred").ToString <> GlobalConstants.ERRORFLAG Then
                        Try
                            _quantityDropDownMaxValue = CInt(_ucr.Attribute("QuantityDropDownMaxValue").Trim())
                        Catch ex As Exception
                            _quantityDropDownMaxValue = 10
                        End Try

                        If dsProductDetails.Tables(2).Rows.Count > 0 Then
                            hdfProductSubType.Value = dsProductDetails.Tables(2).Rows(0)("ProductSubType").ToString
                        End If

                        'Add the extended text (the first item is intentionally missed as it used in the competition field)
                        If ProductType = GlobalConstants.TRAVELPRODUCTTYPE OrElse ProductType = GlobalConstants.EVENTPRODUCTTYPE Then
                            lblExtendedText1.Text = dsProductDetails.Tables(2).Rows(0)("ProductText1").ToString
                            lblExtendedText2.Text = dsProductDetails.Tables(2).Rows(0)("ProductText2").ToString
                            lblExtendedText3.Text = dsProductDetails.Tables(2).Rows(0)("ProductText3").ToString
                            lblExtendedText4.Text = dsProductDetails.Tables(2).Rows(0)("ProductText4").ToString
                            lblExtendedText5.Text = dsProductDetails.Tables(2).Rows(0)("ProductText5").ToString
                            plhExtendedText.Visible = True
                            If lblExtendedText2.Text.Trim().Length = 0 Then
                                If lblExtendedText3.Text.Trim().Length = 0 Then
                                    If lblExtendedText4.Text.Trim().Length = 0 Then
                                        If lblExtendedText5.Text.Trim().Length = 0 Then
                                            plhExtendedText.Visible = False
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            plhExtendedText.Visible = False
                        End If

                        'Bind the Price band data
                        If dsProductDetails.Tables(1).Rows.Count > 0 Then
                            Dim productDefaultPriceBand = dsProductDetails.Tables("ProductDetails").Rows(0)("DefaultPriceBand")
                            Dim productAllowPBAlterations = dsProductDetails.Tables("ProductDetails").Rows(0)("AllowPriceBandAlterations")
                            Dim productPriceBandMode = dsProductDetails.Tables("ProductDetails").Rows(0)("DefaultPriceBandForBasket")
                            rptPriceBandSelection.DataSource = dsProductDetails.Tables("ProductPriceBands")
                            rptPriceBandSelection.DataBind()
                            checkPriceBandList(productDefaultPriceBand, productAllowPBAlterations, productPriceBandMode)
                            If rptPriceBandSelection.Items.Count > 0 Then
                                btnAddToBasket.Text = _ucr.Content("AddToBasketButtonText", _languageCode, True)
                            End If
                        Else
                            Me.Visible = False
                        End If
                    Else
                        Me.Visible = False
                    End If
                Else
                    Me.Visible = False
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    ''' <summary>
    ''' This function is called to get the collection of price bands, with its quantity
    ''' </summary>
    ''' <returns>Collection of price bands</returns>
    ''' <remarks></remarks>
    Public Function GetPriceBands() As Dictionary(Of String, Integer)
        Dim priceBands As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)
        Dim category As String
        Dim quantity As Integer

        For Each item As RepeaterItem In rptPriceBandSelection.Items
            category = CType(item.FindControl("hdfPriceBand"), HiddenField).Value
            If AgentProfile.BulkSalesMode Then
                quantity = TEBUtilities.CheckForDBNull_Int(CType(item.FindControl("txtBulkSalesQuantity"), TextBox).Text)
            Else
                quantity = TEBUtilities.CheckForDBNull_Int(CType(item.FindControl("ddlQuantity"), DropDownList).SelectedValue)
            End If

            If (quantity > 0) Then
                priceBands.Add(category, quantity)
            End If
        Next
        Return priceBands
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Bind the product details (coaches/timeslots etc) to the options drop down list. 
    ''' This takes into acount the availability of each option and moves the available rows to a new table and uses that to bind on.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetUpTravelDDL()
        plhTravelDropdown.Visible = True
        ltlTravelDropDownDesciption.Text = _ucr.Content("TravelDropDownDescription", _languageCode, True)
        If Not DSProductDetail Is Nothing Then
            Dim productDetailHelper As New ProductDetail
            Dim dtTravelDDL As New Data.DataTable
            Dim dtTemp As New Data.DataTable
            dtTravelDDL = DSProductDetail.Tables(1).Copy
            dtTravelDDL.DefaultView.RowFilter = " ProductCode = '" & _ProductCode & "' AND IsSoldOut = 'False'"
            dtTravelDDL.DefaultView.Sort = "ProductCode, ProductDescription2"
            dtTravelDDL.Columns.Add("SelectionText", GetType(String))
            dtTemp = dtTravelDDL.Copy
            dtTemp.Clear()
            For Each row As DataRow In dtTravelDDL.DefaultView.ToTable().Rows

                Dim labelText As String
                Dim productTypeMask As String
                Dim prod_availabilityPercentage As Integer = 0
                Dim prod_availability As Integer = 0
                Dim prod_capacity As Integer = 0
                Dim prod_returned As Integer = 0
                Dim prod_sold As Integer = 0
                Dim prod_reserved As Integer = 0
                Dim prod_booked As Integer = 0
                Dim prod_unavailable As Integer = 0

                'Get availability figures for this sub-item
                productDetailHelper.getProductAvailabilityFigures_TAE(ProductCode, ProductType, String.Empty, row("ProductDetailCode"), prod_availability, prod_availabilityPercentage,
                                              prod_capacity, prod_returned, prod_sold, prod_reserved, prod_booked, prod_unavailable)
                If prod_availability > 0 Then

                    productTypeMask = _ucr.Content("travelDDLAvailabilityMaskLabel", _languageCode, True)
                    If Not productTypeMask = String.Empty Then
                        productDetailHelper.getAvailabilityProperties(_ucr.BusinessUnit, labelText, "", "", prod_availabilityPercentage, ProductStadium)
                        labelText = productDetailHelper.setAvailabilityMask(productTypeMask, labelText, _ProductType, prod_availability.ToString,
                                                                prod_availabilityPercentage.ToString, prod_capacity.ToString, prod_returned.ToString,
                                                                prod_sold.ToString, prod_reserved.ToString, prod_booked.ToString, prod_unavailable.ToString, row("ProductDescription2").ToString.Trim)
                        row("SelectionText") = labelText
                    Else
                        row("SelectionText") = row("ProductDescription2").ToString.Trim
                    End If

                    Dim dRow As Data.DataRow = row
                    dtTemp.ImportRow(row)
                End If
            Next

            ddlTravelOptions.DataSource = dtTemp
            ddlTravelOptions.DataValueField = "ProductDetailCode"
            ddlTravelOptions.DataTextField = "SelectionText"
            ddlTravelOptions.DataBind()
        End If
    End Sub

    Private Sub PopulateHiddenFields()
        If String.IsNullOrEmpty(hdfProductCode.Value) Then
            hdfProductCode.Value = ProductCode
        ElseIf String.IsNullOrEmpty(ProductCode) Then
            ProductCode = hdfProductCode.Value.Trim
        End If
        If String.IsNullOrEmpty(hdfProductType.Value) Then
            hdfProductType.Value = ProductType
        ElseIf String.IsNullOrEmpty(ProductType) Then
            ProductType = hdfProductType.Value.Trim
        End If
        If String.IsNullOrEmpty(hdfProductSubType.Value) Then
            hdfProductSubType.Value = ProductSubType
        ElseIf String.IsNullOrEmpty(ProductSubType) Then
            ProductSubType = hdfProductSubType.Value.Trim
        End If
        If String.IsNullOrEmpty(hdfProductStadium.Value) Then
            hdfProductStadium.Value = ProductStadium
        ElseIf String.IsNullOrEmpty(ProductStadium) Then
            ProductStadium = hdfProductStadium.Value.Trim
        End If
        If String.IsNullOrEmpty(hdfProductDetailCode.Value) Then
            hdfProductDetailCode.Value = ProductDetailCode
        ElseIf String.IsNullOrEmpty(ProductDetailCode) Then
            ProductDetailCode = hdfProductDetailCode.Value.Trim
        End If
        If String.IsNullOrEmpty(hdfQuantityAvailable.Value) Or hdfQuantityAvailable.Value = "0" Then
            hdfQuantityAvailable.Value = QuantityAvailable.ToString
        ElseIf QuantityAvailable = 0 Then
            Try
                QuantityAvailable = CInt(hdfQuantityAvailable.Value)
            Catch
                QuantityAvailable = 0
            End Try
        End If
        If HasAssociatedTravelProduct AndAlso ProductType = GlobalConstants.AWAYPRODUCTTYPE Then
            Try
                If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("displayIncludeTravel")) Then
                    plhTravelProduct.Visible = True
                    lblIncludeTravel.Text = _ucr.Content("includeTravelLabel", _languageCode, True)
                Else
                    plhTravelProduct.Visible = False
                End If
            Catch
                plhTravelProduct.Visible = False
            End Try
        Else
            plhTravelProduct.Visible = False
        End If
    End Sub

#End Region

#Region "Protected Functions"
    Protected Function GetLoggedInCustomerPriceBandCss(ByVal priceBand As String) As String
        Dim cssClass As String = String.Empty
        Dim profile As ProfileCommon = HttpContext.Current.Profile
        If Not profile.IsAnonymous AndAlso profile.User.Details IsNot Nothing AndAlso profile.User.Details.PRICE_BAND = priceBand AndAlso ModuleDefaults.HighlightLoggedInCustomerPriceBand Then
            cssClass = "ebiz-priceband-logged-in-customer"
        End If
        Return cssClass
    End Function
#End Region

#Region "Private Functions"

    Private Function CCurrency(ByVal value As String) As Decimal
        Dim priceAsCurrency As Decimal = 0
        Try
            Dim units As String = value.Substring(value.Length - 2, 2)
            value = value.Substring(0, value.Length - 2)
            value = value & "." & units
            priceAsCurrency = CDec(value)
        Catch ex As Exception
        End Try
        Return priceAsCurrency
    End Function

#End Region

End Class