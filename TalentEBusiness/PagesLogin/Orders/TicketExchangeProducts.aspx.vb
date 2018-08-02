Imports TalentBusinessLogic.ModelBuilders
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.BusinessObjects.Definitions
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common

Partial Class PagesLogin_Orders_TicketExchangeProducts
    Inherits TalentBase01
#Region "Class Level Fields"
    Private _viewModel As TicketExchangeProductsViewModel
    Private _productDetailHelper As New ProductDetail
    Private _selectText As String
#End Region

#Region "Public Properties"
    Public ProductDateHeader As String = String.Empty
    Public ProductDescriptionHeader As String = String.Empty
    Public CompetitionDescriptionHeader As String = String.Empty
    Public TotalWithCustomerHeader As String = String.Empty
    Public TotalPendingOnTicketExchangeHeader As String = String.Empty
    Public TotalSoldOnTicketExchangeHeader As String = String.Empty
    Public TotalResalePriceSoldHeader As String = String.Empty
    Public TotalHandlingFeeSoldHeader As String = String.Empty
    Public ActionHeader As String = String.Empty
    Public ActionHeaderInfo As String = String.Empty
    Public YouWillEarnHeader As String = String.Empty
    Public ProductDateHeaderInfo As String = String.Empty
    Public ShowProductDate As Boolean
    Public ProductDescriptionHeaderinfo As String = String.Empty
    Public ShowProductDescription As Boolean
    Public CompetitionDescriptionHeaderinfo As String = String.Empty
    Public ShowCompetitionDescription As Boolean
    Public TotalWithCustomerHeaderinfo As String = String.Empty
    Public ShowTotalWithCustomer As Boolean
    Public TotalPendingOnTicketExchangeHeaderinfo As String = String.Empty
    Public ShowTotalPendingOnTicketExchange As Boolean
    Public TotalSoldOnTicketExchangeHeaderinfo As String = String.Empty
    Public ShowTotalSoldOnTicketExchange As Boolean
    Public TotalResalePriceSoldHeaderinfo As String = String.Empty
    Public ShowTotalResalePriceSold As Boolean
    Public TotalHandlingFeeSoldHeaderinfo As String = String.Empty
    Public ShowTotalHandlingFeeSold As Boolean
    Public ltlnotAvailable As String

    Public TotalPurchasedHeader As String = String.Empty
    Public TotalPurchasedHeaderinfo As String = String.Empty
    Public ShowTotalPurchased As Boolean

    Public TotalPurchasedPriceHeader As String = String.Empty
    Public TotalPurchasedPriceHeaderinfo As String = String.Empty
    Public ShowTotalPurchasedPrice As Boolean

    Public TotalResalePricePendingHeader As String = String.Empty
    Public TotalResalePricePendingHeaderinfo As String = String.Empty
    Public ShowTotalResalePricePending As Boolean

    Public TotalHandlingFeePendingHeader As String = String.Empty
    Public TotalHandlingFeePendingHeaderinfo As String = String.Empty
    Public ShowTotalHandlingFeePending As Boolean


#End Region

#Region "Protected Page Events"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If TalentDefaults.TicketExchangeEnabled Then
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ticket-exchange-products.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("ticket-exchange-products.js", "/Module/TicketExchange/"), False)
        Else
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        'check if agent has access on Ticket Exchange menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessTicketExchange) Or Not AgentProfile.IsAgent Then
            progressBar()
            Dim wfrPage As WebFormResource = New WebFormResource
            Dim DefaultDaysHistory As Integer = TEBUtilities.CheckForDBNull_Int(("DefaultDaysHistory"))
            If Not IsPostBack Then
                FromDate.Text = Now.AddDays(-DefaultDaysHistory).ToString("dd/MM/yyyy")
            End If
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub
    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        ' Dave - had to move this from page_load as cant get FromDate.Text in page_load   
        Dim inputModel As TicketExchangeProductsInputModel = SetInputModel()
        processController(inputModel)
        CreateView()
        plhErrorList.Visible = (blErrorList.Items.Count > 0)

    End Sub
#End Region

#Region "Private Methods"
    Private Function SetInputModel() As TicketExchangeProductsInputModel
        Dim inputModel As TicketExchangeProductsInputModel = New TicketExchangeProductsInputModel()
        inputModel.CustomerNumber = Profile.UserName
        inputModel.FromDate = Utilities.DateToIseriesFormat(FromDate.Text)
        Return inputModel
    End Function

    Private Sub processController(inputModel As TicketExchangeProductsInputModel)

        Dim ticketExchangeProductsBuilder As New TicketExchangeProductsBuilder()
        _viewModel = ticketExchangeProductsBuilder.GetTicketExchangeProductsListForCustomer(inputModel)
    End Sub
    Private Sub CreateView()
        ProcessErrors()
        PopulateText()
        PopulateData()
    End Sub
    Private Function getFormattedDateString(ByVal dateString As String) As String
        Dim formattedDateString As String = String.Empty
        If dateString.Length >= 10 Then
            formattedDateString = dateString.Substring(0, 2) & dateString.Substring(3, 2) & dateString.Substring(6, 4)
        End If
        Return formattedDateString
    End Function

    Private Sub progressBar()
        If ModuleDefaults.ShowTicketExchangeProgressBar Then
            ProgressBar1.HTMLInclude = "TEProgressBarSelectProducts.html"
        End If
    End Sub

#End Region

#Region "View Methods"
    Protected Sub PopulateText()
        ProductDateHeader = _viewModel.GetPageText("ProductDateHeader")
        ProductDescriptionHeader = _viewModel.GetPageText("ProductDescriptionHeader")
        CompetitionDescriptionHeader = _viewModel.GetPageText("CompetitionDescriptionHeader")
        TotalWithCustomerHeader = _viewModel.GetPageText("TotalWithCustomerHeader")
        TotalPurchasedHeader = _viewModel.GetPageText("TotalPurchasedHeader")
        TotalPurchasedPriceHeader = _viewModel.GetPageText("TotalPurchasedPriceHeader")
        TotalPendingOnTicketExchangeHeader = _viewModel.GetPageText("TotalPendingOnTicketExchangeHeader")
        TotalHandlingFeeSoldHeader = _viewModel.GetPageText("TotalHandlingFeeSoldHeader")
        TotalHandlingFeePendingHeader = _viewModel.GetPageText("TotalHandlingFeePendingHeader")
        TotalSoldOnTicketExchangeHeader = _viewModel.GetPageText("TotalSoldOnTicketExchangeHeader")
        ActionHeader = _viewModel.GetPageText("ActionHeader")
        YouWillEarnHeader = _viewModel.GetPageText("YouWillEarnHeader")
        TotalResalePriceSoldHeader = _viewModel.GetPageText("TotalResalePriceSoldHeader")
        TotalResalePricePendingHeader = _viewModel.GetPageText("TotalResalePricePendingHeader")
        ltlTicketingExchangeProductsHeader.Text = _viewModel.GetPageText("ltlTicketingExchangeProductsHeader")
        ltlSellYourTickets.Text = _viewModel.GetPageText("ltlSellYourTickets")
        _selectText = _viewModel.GetPageText("ltlSelect")
        FromDateLabel.Text = _viewModel.GetPageText("ltlFromDate")
        ltlnotAvailable = _viewModel.GetPageText("ltlnotAvailable")

        ' Column visability and more info 
        ActionHeaderInfo = _viewModel.GetPageText("ActionHeaderInfo")

        ProductDateHeaderInfo = _viewModel.GetPageText("ProductDateHeaderInfo")
        ShowProductDate = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowProductDate"))

        ProductDescriptionHeaderinfo = _viewModel.GetPageText("ProductDescriptionHeaderInfo")
        ShowProductDescription = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowProductDescription"))

        CompetitionDescriptionHeaderinfo = _viewModel.GetPageText("CompetitionDescriptionHeaderInfo")
        ShowCompetitionDescription = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowCompetitionDescription"))

        TotalWithCustomerHeaderinfo = _viewModel.GetPageText("TotalWithCustomerHeaderinfo")
        ShowTotalWithCustomer = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalWithCustomer"))

        TotalPendingOnTicketExchangeHeaderinfo = _viewModel.GetPageText("TotalPendingOnTicketExchangeHeaderinfo")
        ShowTotalPendingOnTicketExchange = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalPendingOnTicketExchange"))

        TotalSoldOnTicketExchangeHeaderinfo = _viewModel.GetPageText("TotalSoldOnTicketExchangeHeaderinfo")
        ShowTotalSoldOnTicketExchange = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalSoldOnTicketExchange"))

        TotalResalePriceSoldHeaderinfo = _viewModel.GetPageText("TotalResalePriceSoldHeaderinfo")
        ShowTotalResalePriceSold = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalResalePriceSold"))

        TotalResalePricePendingHeaderinfo = _viewModel.GetPageText("TotalResalePricePendingHeaderinfo")
        ShowTotalResalePricePending = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalResalePricePending"))

        TotalHandlingFeeSoldHeaderinfo = _viewModel.GetPageText("TotalHandlingFeeSoldHeaderinfo")
        ShowTotalHandlingFeeSold = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalHandlingFeeSold"))

        TotalHandlingFeePendingHeaderinfo = _viewModel.GetPageText("TotalHandlingFeePendingHeaderinfo")
        ShowTotalHandlingFeePending = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalHandlingFeePending"))

        TotalPurchasedHeaderinfo = _viewModel.GetPageText("TotalPurchasedHeaderinfo")
        ShowTotalPurchased = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalPurchased"))

        TotalPurchasedPriceHeaderinfo = _viewModel.GetPageText("TotalPurchasedPriceHeaderinfo")
        ShowTotalPurchasedPrice = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_viewModel.GetPageAttribute("ShowTotalPurchasedPrice"))


        btnFromDate.Text = _viewModel.GetPageText("btnFromDateText")
        If Not IsPostBack Then
            FromDate.Text = Now.ToString("dd/MM/yyyy")
            Dim DefaultDaysHistory As Integer = TEBUtilities.CheckForDBNull_Int(_viewModel.GetPageAttribute("DefaultDaysHistory"))
            FromDate.Text = Now.AddDays(-DefaultDaysHistory).ToString("dd/MM/yyyy")
        End If

        ' 3 lines of sumamry is user defined as a mask   
        ltlSummaryLine1.Text = setTEProductsMask(_viewModel.GetPageText("ltlSummaryLine1"), _viewModel)
        ltlSummaryLine2.Text = setTEProductsMask(_viewModel.GetPageText("ltlSummaryLine2"), _viewModel)
        ltlSummaryLine3.Text = setTEProductsMask(_viewModel.GetPageText("ltlSummaryLine3"), _viewModel)
    End Sub

    Protected Sub PopulateData()
        rptTicketExchangeProducts.DataSource = _viewModel.TicketExchangeProductSummaryList
        _viewModel.TicketExchangeProductSummaryList.Sort(AddressOf SortDate)
        rptTicketExchangeProducts.DataBind()
        rptTicketExchangeProducts.Visible = True
    End Sub

    Private Function SortDate(ByVal x As TicketExchangeProductSummary, ByVal y As TicketExchangeProductSummary) As Integer
        Dim xDate As DateTime
        Dim yDate As DateTime
        If DateTime.TryParse(x.ProductDate, xDate) Then
            If DateTime.TryParse(y.ProductDate, yDate) Then
                Return xDate.CompareTo(yDate)
            End If
        Else
            Return -1
        End If
        Return 1
    End Function

    Private Sub ProcessErrors()
        blErrorList.Items.Clear()
        If _viewModel.Error IsNot Nothing AndAlso _viewModel.Error.HasError Then
            blErrorList.Items.Add(_viewModel.Error.ErrorMessage)
        End If
    End Sub

    Private Function setTEProductsMask(ByVal TicketExchangeMask As String, ByVal _tep As TicketExchangeProductsViewModel) As String

        Dim TicketExchangeString As String = TicketExchangeMask
        If TicketExchangeMask.Contains("<<") Then
            TicketExchangeString = TicketExchangeString.Replace("<<TotalExpiredOnTicketExchange>>", _tep.TotalExpiredOnTicketExchange)
            TicketExchangeString = TicketExchangeString.Replace("<<TotalPendingOnTicketExchange>>", _tep.TotalPendingOnTicketExchange.ToString)
            TicketExchangeString = TicketExchangeString.Replace("<<TotalSoldOnTicketExchange>>", _tep.TotalSoldOnTicketExchange.ToString)
            TicketExchangeString = TicketExchangeString.Replace("<<TotalReSalePriceExpired>>", FormatCurrency(_tep.TotalReSalePriceExpired.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalReSalePricePending>>", FormatCurrency(_tep.TotalReSalePricePending.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalReSalePriceSold>>", FormatCurrency(_tep.TotalReSalePriceSold.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalPotentialEarningPriceExpired>>", FormatCurrency(_tep.TotalPotentialEarningPriceExpired.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalPotentialEarningPricePending>>", FormatCurrency(_tep.TotalPotentialEarningPricePending.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalPotentialEarningPriceSold>>", FormatCurrency(_tep.TotalPotentialEarningPriceSold.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalHandlingFeePending>>", FormatCurrency(_tep.TotalHandlingFeePending.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalHandlingFeeExpired>>", FormatCurrency(_tep.TotalHandlingFeeExpired.ToString))
            TicketExchangeString = TicketExchangeString.Replace("<<TotalHandlingFeeSold>>", FormatCurrency(_tep.TotalHandlingFeeSold.ToString))
        End If

        Return TicketExchangeString
    End Function

    Protected Sub rptTicketExchangeProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTicketExchangeProducts.ItemDataBound
        Dim hplSelect As HyperLink = CType(e.Item.FindControl("hplSelect"), HyperLink)

        Dim plhproductDateInfo As PlaceHolder = CType(e.Item.FindControl("plhproductDateInfo"), PlaceHolder)
        Dim plhproductDate As PlaceHolder = CType(e.Item.FindControl("plhproductDate"), PlaceHolder)
        Dim plhproductDateHeader As PlaceHolder = CType(e.Item.FindControl("plhproductDateHeader"), PlaceHolder)

        Dim plhproductDescriptionInfo As PlaceHolder = CType(e.Item.FindControl("plhproductDescriptionInfo"), PlaceHolder)
        Dim plhproductDescription As PlaceHolder = CType(e.Item.FindControl("plhproductDescription"), PlaceHolder)
        Dim plhproductDescriptionHeader As PlaceHolder = CType(e.Item.FindControl("plhproductDescriptionHeader"), PlaceHolder)

        Dim plhCompetitionDescriptionInfo As PlaceHolder = CType(e.Item.FindControl("plhCompetitionDescriptionInfo"), PlaceHolder)
        Dim plhCompetitionDescription As PlaceHolder = CType(e.Item.FindControl("plhCompetitionDescription"), PlaceHolder)
        Dim plhCompetitionDescriptionHeader As PlaceHolder = CType(e.Item.FindControl("plhCompetitionDescriptionHeader"), PlaceHolder)

        Dim plhTotalWithCustomerInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalWithCustomerInfo"), PlaceHolder)
        Dim plhTotalWithCustomer As PlaceHolder = CType(e.Item.FindControl("plhTotalWithCustomer"), PlaceHolder)
        Dim plhTotalWithCustomerHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalWithCustomerHeader"), PlaceHolder)

        Dim plhTotalPendingOnTicketExchangeInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalPendingOnTicketExchangeInfo"), PlaceHolder)
        Dim plhTotalPendingOnTicketExchange As PlaceHolder = CType(e.Item.FindControl("plhTotalPendingOnTicketExchange"), PlaceHolder)
        Dim plhTotalPendingOnTicketExchangeHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalPendingOnTicketExchangeHeader"), PlaceHolder)

        Dim plhTotalSoldOnTicketExchangeInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalSoldOnTicketExchangeInfo"), PlaceHolder)
        Dim plhTotalSoldOnTicketExchange As PlaceHolder = CType(e.Item.FindControl("plhTotalSoldOnTicketExchange"), PlaceHolder)
        Dim plhTotalSoldOnTicketExchangeHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalSoldOnTicketExchangeHeader"), PlaceHolder)

        Dim plhTotalResalePriceSoldInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalResalePriceSoldInfo"), PlaceHolder)
        Dim plhTotalResalePriceSold As PlaceHolder = CType(e.Item.FindControl("plhTotalResalePriceSold"), PlaceHolder)
        Dim plhTotalResalePriceSoldHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalResalePriceSoldHeader"), PlaceHolder)

        Dim plhTotalResalePricePendingInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalResalePricePendingInfo"), PlaceHolder)
        Dim plhTotalResalePricePending As PlaceHolder = CType(e.Item.FindControl("plhTotalResalePricePending"), PlaceHolder)
        Dim plhTotalResalePricePendingHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalResalePricePendingHeader"), PlaceHolder)

        Dim plhTotalHandlingFeeSoldInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalHandlingFeeSoldInfo"), PlaceHolder)
        Dim plhTotalHandlingFeeSold As PlaceHolder = CType(e.Item.FindControl("plhTotalHandlingFeeSold"), PlaceHolder)
        Dim plhTotalHandlingFeeSoldHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalHandlingFeeSoldHeader"), PlaceHolder)

        Dim plhTotalHandlingFeePendingInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalHandlingFeePendingInfo"), PlaceHolder)
        Dim plhTotalHandlingFeePending As PlaceHolder = CType(e.Item.FindControl("plhTotalHandlingFeePending"), PlaceHolder)
        Dim plhTotalHandlingFeePendingHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalHandlingFeePendingHeader"), PlaceHolder)

        Dim hdfStatusCode As HiddenField = CType(e.Item.FindControl("hdfStatusCode"), HiddenField)
        Dim plhSelect As PlaceHolder = CType(e.Item.FindControl("plhSelect"), PlaceHolder)
        Dim plhnotAvailable As PlaceHolder = CType(e.Item.FindControl("plhnotAvailable"), PlaceHolder)

        Dim plhTotalPurchasedInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalPurchasedInfo"), PlaceHolder)
        Dim plhTotalPurchased As PlaceHolder = CType(e.Item.FindControl("plhTotalPurchased"), PlaceHolder)
        Dim plhTotalPurchasedHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalPurchasedHeader"), PlaceHolder)

        Dim plhTotalPurchasedPriceInfo As PlaceHolder = CType(e.Item.FindControl("plhTotalPurchasedPriceInfo"), PlaceHolder)
        Dim plhTotalPurchasedPrice As PlaceHolder = CType(e.Item.FindControl("plhTotalPurchasedPrice"), PlaceHolder)
        Dim plhTotalPurchasedPriceHeader As PlaceHolder = CType(e.Item.FindControl("plhTotalPurchasedPriceHeader"), PlaceHolder)


        If e.Item.ItemType = ListItemType.Header Then
            plhproductDateHeader.Visible = ShowProductDate
            If ProductDateHeaderInfo.Trim = String.Empty Then
                plhproductDateInfo.Visible = False
            End If
            plhproductDescriptionHeader.Visible = ShowProductDescription
            If ProductDescriptionHeaderinfo.Trim = String.Empty Then
                plhproductDescriptionInfo.Visible = False
            End If
            plhproductDescriptionHeader.Visible = ShowProductDescription
            If ProductDescriptionHeaderinfo.Trim = String.Empty Then
                plhproductDescriptionInfo.Visible = False
            End If
            plhCompetitionDescriptionHeader.Visible = ShowCompetitionDescription
            If CompetitionDescriptionHeaderinfo.Trim = String.Empty Then
                plhCompetitionDescriptionInfo.Visible = False
            End If
            plhTotalWithCustomerHeader.Visible = ShowTotalWithCustomer
            If TotalWithCustomerHeaderinfo.Trim = String.Empty Then
                plhTotalWithCustomerInfo.Visible = False
            End If

            plhTotalPurchasedHeader.Visible = ShowTotalPurchased
            If TotalPurchasedHeaderinfo.Trim = String.Empty Then
                plhTotalPurchasedInfo.Visible = False
            End If

            plhTotalPurchasedPriceHeader.Visible = ShowTotalPurchasedPrice
            If TotalPurchasedPriceHeaderinfo.Trim = String.Empty Then
                plhTotalPurchasedPriceInfo.Visible = False
            End If

            plhTotalPendingOnTicketExchangeHeader.Visible = ShowTotalPendingOnTicketExchange
            If TotalPendingOnTicketExchangeHeaderinfo.Trim = String.Empty Then
                plhTotalPendingOnTicketExchangeInfo.Visible = False
            End If
            plhTotalSoldOnTicketExchangeHeader.Visible = ShowTotalSoldOnTicketExchange
            If TotalSoldOnTicketExchangeHeaderinfo.Trim = String.Empty Then
                plhTotalSoldOnTicketExchangeInfo.Visible = False
            End If
            plhTotalResalePriceSoldHeader.Visible = ShowTotalResalePriceSold
            If TotalResalePriceSoldHeaderinfo.Trim = String.Empty Then
                plhTotalResalePriceSoldInfo.Visible = False
            End If

            plhTotalResalePricePendingHeader.Visible = ShowTotalResalePricePending
            If TotalResalePricePendingHeaderinfo.Trim = String.Empty Then
                plhTotalResalePricePendingInfo.Visible = False
            End If
            plhTotalHandlingFeeSoldHeader.Visible = ShowTotalHandlingFeeSold
            If TotalHandlingFeeSoldHeaderinfo.Trim = String.Empty Then
                plhTotalHandlingFeeSoldInfo.Visible = False
            End If

            plhTotalHandlingFeePendingHeader.Visible = ShowTotalHandlingFeePending
            If TotalHandlingFeePendingHeaderinfo.Trim = String.Empty Then
                plhTotalHandlingFeePendingInfo.Visible = False
            End If
        End If

        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim ticketExchangeProduct As TicketExchangeProductSummary = DirectCast(e.Item.DataItem, TicketExchangeProductSummary)
            hplSelect.NavigateUrl = "~/PagesLogin/Orders/TicketExchangeSelection.aspx?stage=1&product=" & CType(e.Item.DataItem, TicketExchangeProductSummary).ProductCode
            hplSelect.Text = _selectText
            plhproductDate.Visible = ShowProductDate
            plhproductDescription.Visible = ShowProductDescription
            plhCompetitionDescription.Visible = ShowCompetitionDescription
            plhTotalWithCustomer.Visible = ShowTotalWithCustomer
            plhTotalPurchased.Visible = ShowTotalPurchased
            plhTotalPurchasedPrice.Visible = ShowTotalPurchasedPrice
            plhTotalPendingOnTicketExchange.Visible = ShowTotalPendingOnTicketExchange
            plhTotalSoldOnTicketExchange.Visible = ShowTotalSoldOnTicketExchange
            plhTotalResalePriceSold.Visible = ShowTotalResalePriceSold
            plhTotalResalePricePending.Visible = ShowTotalResalePricePending
            plhTotalHandlingFeeSold.Visible = ShowTotalHandlingFeeSold
            plhTotalHandlingFeePending.Visible = ShowTotalHandlingFeePending
            If Trim(ticketExchangeProduct.StatusCode) <> String.Empty Then
                plhSelect.Visible = False
                plhnotAvailable.Visible = True
            Else
                plhSelect.Visible = True
                plhnotAvailable.Visible = False
            End If
        End If
    End Sub
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        Dim xValue As Decimal
        If Decimal.TryParse(value, xValue) Then
            formattedString = TDataObjects.PaymentSettings.FormatCurrency(xValue, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        End If
        Return formattedString
    End Function
    Public Function getStatusDescription(ByVal errorCode As String) As String
        Dim errorDescription As String = String.Empty
        If errorCode.Trim <> String.Empty Then
            errorDescription = _viewModel.GetPageText("ActionErrorDesc_" + errorCode)
        End If
        Return errorDescription
    End Function
#End Region
End Class
