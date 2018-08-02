Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class PagesPublic_ProductBrowse_TicketingPrePayments
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As New WebFormResource
    Private _errMsg As TalentErrorMessages
    Private _ppsPage As String

#End Region

#Region "Public Properties"

    Public Property PPSPage() As String
        Get
            Return _ppsPage
        End Get
        Set(ByVal value As String)
            _ppsPage = value
        End Set
    End Property

    Public ReadOnly Property SendRegisteredPost() As Boolean
        Get
            Return Me.RegisteredPost.Checked
        End Get
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim hasSeasonTickets As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (UCase(tbi.MODULE_) = "TICKETING") Then
                If tbi.PRODUCT_TYPE = "S" Then
                    hasSeasonTickets = True
                    Exit For
                End If
            End If
        Next
        If hasSeasonTickets Then
            With _wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = String.Empty
                .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            End With
            If String.IsNullOrEmpty(Request.QueryString("ppspage")) Then
                PPSPage = "1"
            Else
                PPSPage = Request.QueryString("ppspage")
            End If
            ProductDetail1.PPSType = PPSPage
            ProductDetail1.ProductType = "P"
            If Not Page.IsPostBack Then SetText()
        Else
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
        _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
    End Sub

    Protected Sub Page_PreRender1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            If Not Session("TalentErrorCode") Is Nothing Then
                Dim myError As String = CStr(Session("TalentErrorCode"))
                ErrorList.Items.Add(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                    Talent.eCommerce.Utilities.GetCurrentPageName, _
                                    myError).ERROR_MESSAGE)
                Session("TalentErrorCode") = Nothing
            End If
        End If
        plhInstructionsText.Visible = (InstructionText.Text.Length > 0)
    End Sub

    Protected Sub ContinueButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ContinueButton.Click
        ErrorList.Items.Clear()

        'Scheme Name, Tickets List
        Dim noSelection As Boolean = False
        Dim itemsSelected As Boolean = False
        Dim priceCode As String = String.Empty
        If Request.QueryString("pricecode") IsNot Nothing Then priceCode = Request.QueryString("pricecode")

        'Loop through all schemes
        For Each ri As RepeaterItem In ProductDetail1.ProductsRepeater.Items
            Dim pps As UserControls_TicketingPPS = CType(ri.FindControl("TicketingPPS1"), UserControls_TicketingPPS)
            Dim loopItemsSelected As Boolean = False
            If pps.SeasonTicketsList.Items.Count > 0 Then

                'otherwise check for selected season tickets in the current scheme
                Dim loopCount As Integer = 0
                For Each item As ListItem In pps.SeasonTicketsList.Items
                    loopCount += 1
                    If item.Selected Then
                        'if selected add to the tickets list
                        itemsSelected = True
                        loopItemsSelected = True
                        Exit For
                    End If
                Next

                'if the user must select not to have any pps subscriptions
                'but no selection has been made, set a flag to indicate this
                If ModuleDefaults.PPS_MUST_TICK_FOR_NO_SCHEMES AndAlso _
                        Not loopItemsSelected AndAlso _
                        Not pps.NoSchemesRequired.Checked Then
                    noSelection = True
                End If
            End If
            If noSelection Then Exit For
        Next

        If noSelection Then
            'Display error message to indicate that the user has failed to select any option
            ErrorList.Items.Add(_wfrPage.Content("noSelectionError", _languageCode, True))
        Else
            'if the user has selected schemes then forward on to the ticketing gateway
            Dim productSubType As String = String.Empty
            If Request.QueryString("productsubtype") IsNot Nothing Then productSubType = Request.QueryString("productsubtype")
            If itemsSelected Then
                Dim redirectUrl As String = "~/Redirect/TicketingGateway.aspx?page=TicketingPrePayments.aspx&function=AddToBasket&ppspage=" & PPSPage.ToString & "&product=" & Request.QueryString("product") & "&pricecode=" & priceCode & "&productsubtype=" & productSubType
                Server.Transfer(redirectUrl)
            Else
                'otherwise forward onto page 2 if it is enabled
                If ModuleDefaults.PPS_ENABLE_2 AndAlso PPSPage <> "2" Then
                    Response.Redirect("~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & Request.QueryString("product") & "&pricecode=" & priceCode & "&productsubtype=" & productSubType)
                Else
                    'if not, check for linked products and forward on to basket
                    Dim redirectUrl As String = "~/PagesPublic/Basket/Basket.aspx"
                    If Request.QueryString("product") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("product").Trim()) Then
                        Dim productHasRelatedProducts As Boolean = False
                        Dim productHasMandatoryRelatedProducts As Boolean = False
                        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                        Dim linkedMasterProduct As String = TDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(Profile.Basket.Basket_Header_ID)
                        If String.IsNullOrEmpty(linkedMasterProduct) OrElse linkedMasterProduct = HttpContext.Current.Request.QueryString("product").Trim() Then
                            ticketingGatewayFunctions.CheckForLinkedProducts(Request.QueryString("product").Trim(), priceCode, String.Empty, productSubType, productHasRelatedProducts, productHasMandatoryRelatedProducts)
                        End If
                        ticketingGatewayFunctions.ProductHasRelatedProducts = productHasRelatedProducts
                        If Not String.IsNullOrEmpty(Request.QueryString("pricecode")) Then
                            Session("priceCode") = Request.QueryString("pricecode")
                        Else
                            Session("priceCode") = String.Empty
                        End If
                        redirectUrl = ticketingGatewayFunctions.HandleRedirect(redirectUrl, True, Request.QueryString("product").Trim(), priceCode, productSubType, False, False, 0, linkedMasterProduct)
                    End If
                    Response.Redirect(redirectUrl)
                End If
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SetText()
        If Not PPSPage.ToLower.Equals("2") Then
            PPSPage = "1"
        End If

        ' Retrieve the general page text
        ContinueButton.Text = _wfrPage.Content("ContinueButtonText_" & PPSPage, _languageCode, True)
        RegisteredPost.Text = _wfrPage.Content("RegisteredPostCheckBoxText_" & PPSPage, _languageCode, True)
        plhRegisteredPost.Visible = CheckForDBNull_Boolean_DefaultTrue(_wfrPage.Attribute("ShowRegisteredPost"))

        ' Construct the new instructions text
        Dim inst As New StringBuilder
        Dim processedStands As New ArrayList
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (UCase(tbi.MODULE_) = "TICKETING") Then
                If UCase(tbi.PRODUCT_TYPE.Trim) = "S" Then

                    'Have we processed this stand before
                    If processedStands.IndexOf(tbi.SEAT.Substring(0, 3)) < 0 Then

                        ' Query the database for the bespoke instruction text
                        inst.Append(_wfrPage.Content("InstructionText" & PPSPage & tbi.SEAT.Substring(0, 7), _languageCode, True))
                        inst.Append(_wfrPage.Content("InstructionText" & PPSPage & tbi.SEAT.Substring(0, 3) & "*ALL", _languageCode, True))

                        ' Query the database for the bespoke instruction text - Bondholder specific
                        inst.Append(_wfrPage.Content("InstructionText" & PPSPage & tbi.SEAT.Substring(0, 7) & Profile.User.Details.BOND_HOLDER.ToString, _languageCode, True))
                        inst.Append(_wfrPage.Content("InstructionText" & PPSPage & tbi.SEAT.Substring(0, 3) & "*ALL" & Profile.User.Details.BOND_HOLDER.ToString, _languageCode, True))

                        ' Se the stand as processed
                        processedStands.Add(tbi.SEAT.Substring(0, 3))
                    End If
                End If
            End If
        Next

        ' Set the bespoke stand and area instruction text
        InstructionText.Text = inst.ToString
        InstructionText.Visible = (InstructionText.Text.Length > 0)
    End Sub

#End Region

End Class