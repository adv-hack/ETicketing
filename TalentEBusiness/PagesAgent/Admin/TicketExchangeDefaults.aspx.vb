Imports TalentBusinessLogic.ModelBuilders
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.BusinessObjects.Definitions
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports TalentBusinessLogic.DataTransferObjects
Imports System.Collections.Generic

Partial Class PagesAgent_Admin_TicketExchangeDefaults
    Inherits TalentBase01
#Region "Class Level Fields"
    Private _viewModel As TicketExchangeDefaultsViewModel
    Private _confirmInputModel As New TicketExchangeDefaultsConfirmInputModel
    Private _confirmViewModel As TicketExchangeConfirmViewModel
    Private _productDetailHelper As New ProductDetail
    Private _selectText As String
#End Region

#Region "Public Properties"
    Public StandAreaMaskHeader As String = String.Empty
    Public SumOfTicketsAllocatedSoldHeader As String = String.Empty
    Public SumOfTicketsBookedHeader As String = String.Empty
    Public SumOfTicketsPendingOnTicketExchangeHeader As String = String.Empty
    Public SumOfTicketsExpiredOnTicketExchangeHeader As String = String.Empty
    Public SumOfTicketsSoldOnTicketExchangeHeader As String = String.Empty
    Public ValueOfTicketsPendingOnTicketExchangeHeader As String = String.Empty
    Public ValueOfTicketsExpiredOnTicketExchangeHeader As String = String.Empty
    Public ValueOfTicketsSoldOnTicketExchangeHeader As String = String.Empty
    Public ValueOfTEFeesPendingOnTicketExchangeHeader As String = String.Empty
    Public ValueOfTEFeesExpiredOnTicketExchangeHeader As String = String.Empty
    Public ValueOfTEFeesSoldOnTicketExchangeHeader As String = String.Empty
    Public AllowPlaceOnTEHeader As String = String.Empty
    Public AllowPurchaseTEHeader As String = String.Empty
    Public ltlAllProducts As String = String.Empty
#End Region


#Region "Protected Page Events"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If TalentDefaults.TicketExchangeEnabled Then
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ticket-exchange-defaults.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("ticket-exchange-defaults.js", "/Module/TicketExchange/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "table-functions.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("table-functions.js", "/Application/Elements/"), False)
        Else
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
        Dim wfrPage As WebFormResource = New WebFormResource
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        blErrorList.Items.Clear()
        blSuccessList.Items.Clear()
        If Not String.IsNullOrWhiteSpace(Request.Params(btnUpdate.UniqueID)) Then
            Update()
        End If
        Dim inputModel As TicketExchangeDefaultsInputModel = SetInputModel()
        processController(inputModel)
        CreateView()
    End Sub
    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorList.Items.Count > 0)
        plhSuccessList.Visible = (blSuccessList.Items.Count > 0)
    End Sub
#End Region

#Region "Private Methods"
    Private Function SetInputModel() As TicketExchangeDefaultsInputModel
        Dim inputModel As TicketExchangeDefaultsInputModel = New TicketExchangeDefaultsInputModel()
        inputModel.ProductCode = UCase(ddlProductCode.SelectedValue)
        inputModel.ReturnAllProducts = False

        '1st entry is for all products 
        If ddlProductCode.SelectedIndex = -1 Or ddlProductCode.SelectedIndex = 0 Then
            inputModel.ReturnAllProducts = True
            inputModel.ProductCode = String.Empty
        End If
        Return inputModel
    End Function

    Private Sub processController(inputModel As TicketExchangeDefaultsInputModel)
        Dim TicketExchangeDefaultsBuilder As New TicketExchangeDefaultsBuilder()
        _viewModel = TicketExchangeDefaultsBuilder.GetTicketExchangeDefaults(inputModel)
    End Sub
    Private Sub CreateView()
        ProcessErrors()
        PopulateText()
        If Not _viewModel.Error.HasError Then
            PopulateData()
        End If
    End Sub
#End Region

#Region "View Methods"
    Protected Sub PopulateText()
        ProductCodeLabel.Text = _viewModel.GetPageText("ltlProductCode")
        btnUpdate.Text = _viewModel.GetPageText("btnUpdate")
        lblAllowPlaceOnSale.Text = _viewModel.GetPageText("ltlAllowPlaceOnSale")
        lblAllowPurchase.Text = _viewModel.GetPageText("ltlAllowPurchase")
        lblProductMinPrice.Text = _viewModel.GetPageText("ltlProductMinPrice")
        lblProductmaxPrice.Text = _viewModel.GetPageText("ltlProductMaxPrice")
        lblDefaults.Text = _viewModel.GetPageText("ltlDefaults")
        lblClubFee.Text = _viewModel.GetPageText("lblClubFee")
        lblClubFeeType.Text = _viewModel.GetPageText("lblClubFeeType")
        lblMinMaxBoundaryType.Text = _viewModel.GetPageText("lblMinMaxBoundaryType")
        lblSumOfTicketsAllocatedSold.Text = _viewModel.GetPageText("lblSumOfTicketsAllocatedSold")
        lblCustomerRetainsPrerequisite.Text = _viewModel.GetPageText("lblCustomerRetainsPrerequisite")
        lblCustomerRetainsMaximumLimit.Text = _viewModel.GetPageText("lblCustomerRetainsMaximumLimit")
        ltlTicketingExchangeSummaryHeader.Text = _viewModel.GetPageText("ltlTicketingExchangeSummaryHeader")
        ltlTicketingExchangeDefaultHeader.Text = _viewModel.GetPageText("ltlTicketingExchangeDefaultHeader")
        ltlSummarySoldHeader.Text = _viewModel.GetPageText("ltlSummarySoldHeader")
        ltlSummaryPendingHeader.Text = _viewModel.GetPageText("ltlSummaryPendingHeader")
        ltlSummaryExpiredHeader.Text = _viewModel.GetPageText("ltlSummaryExpiredHeader")
        hdfValidateProductMinPriceMsg.Value = _viewModel.GetPageText("ValidateProductMinPrice")
        hdfValidateProductMaxPriceMsg.Value = _viewModel.GetPageText("ValidateProductMaxPrice")


        lblSumOfTicketsBooked.Text = _viewModel.GetPageText("lblSumOfTicketsBooked")
        lblSumOfTicketsPendingOnTicketExchange.Text = _viewModel.GetPageText("lblSumOfTicketsPendingOnTicketExchange")
        lblSumOfTicketsExpiredOnTicketExchange.Text = _viewModel.GetPageText("lblSumOfTicketsExpiredOnTicketExchange")
        lblSumOfTicketsSoldOnTicketExchange.Text = _viewModel.GetPageText("lblSumOfTicketsSoldOnTicketExchange")
        lblValueOfTicketsPendingOnTicketExchange.Text = _viewModel.GetPageText("lblValueOfTicketsPendingOnTicketExchange")
        lblValueOfTicketsExpiredOnTicketExchange.Text = _viewModel.GetPageText("lblValueOfTicketsExpiredOnTicketExchange")
        lblValueOfTicketsSoldOnTicketExchange.Text = _viewModel.GetPageText("lblValueOfTicketsSoldOnTicketExchange")
        lblValueOfTEFeesPendingOnTicketExchange.Text = _viewModel.GetPageText("lblValueOfTEFeesPendingOnTicketExchange")
        lblValueOfTEFeesExpiredOnTicketExchange.Text = _viewModel.GetPageText("lblValueOfTEFeesExpiredOnTicketExchange")
        lblValueOfTEFeesSoldOnTicketExchange.Text = _viewModel.GetPageText("lblValueOfTEFeesSoldOnTicketExchange")
        ltlAllProducts = _viewModel.GetPageText("ltlAllProducts")

        If ddlProductCode.SelectedIndex = -1 Or ddlProductCode.SelectedIndex = 0 Then
            btnUpdate.Visible = False
            plhProductLevelFields.Visible = False

        Else
            btnUpdate.Visible = True
            plhProductLevelFields.Visible = True

            If ddlClubFeeType.Items.Count = 0 Then
                Dim ddlItem As New ListItem(_viewModel.GetPageText("FeeTypeP"), "P")
                Dim ddlItem2 As New ListItem(_viewModel.GetPageText("FeeTypeF"), "F")
                ddlClubFeeType.Items.Insert(0, ddlItem)
                ddlClubFeeType.Items.Insert(0, ddlItem2)
                ddlMinMaxBoundaryType.Items.Insert(0, ddlItem)
                ddlMinMaxBoundaryType.Items.Insert(0, ddlItem2)
            End If
        End If

        'column headings for stand/area  
        StandAreaMaskHeader = _viewModel.GetPageText("StandAreaMaskHeader")
        SumOfTicketsAllocatedSoldHeader = _viewModel.GetPageText("SumOfTicketsAllocatedSoldHeader")
        SumOfTicketsBookedHeader = _viewModel.GetPageText("SumOfTicketsBookedHeader")
        SumOfTicketsPendingOnTicketExchangeHeader = _viewModel.GetPageText("SumOfTicketsPendingOnTicketExchangeHeader")
        SumOfTicketsExpiredOnTicketExchangeHeader = _viewModel.GetPageText("SumOfTicketsExpiredOnTicketExchangeHeader")
        SumOfTicketsSoldOnTicketExchangeHeader = _viewModel.GetPageText("SumOfTicketsSoldOnTicketExchangeHeader")
        ValueOfTicketsPendingOnTicketExchangeHeader = _viewModel.GetPageText("ValueOfTicketsPendingOnTicketExchangeHeader")
        ValueOfTicketsExpiredOnTicketExchangeHeader = _viewModel.GetPageText("ValueOfTicketsExpiredOnTicketExchangeHeader")
        ValueOfTicketsSoldOnTicketExchangeHeader = _viewModel.GetPageText("ValueOfTicketsSoldOnTicketExchangeHeader")
        ValueOfTEFeesPendingOnTicketExchangeHeader = _viewModel.GetPageText("ValueOfTEFeesPendingOnTicketExchangeHeader")
        ValueOfTEFeesExpiredOnTicketExchangeHeader = _viewModel.GetPageText("ValueOfTEFeesExpiredOnTicketExchangeHeader")
        ValueOfTEFeesSoldOnTicketExchangeHeader = _viewModel.GetPageText("ValueOfTEFeesSoldOnTicketExchangeHeader")
        AllowPlaceOnTEHeader = _viewModel.GetPageText("AllowPlaceOnSaleHeader")
        AllowPurchaseTEHeader = _viewModel.GetPageText("AllowPurchaseHeader")

    End Sub
    Protected Sub Update()
        _confirmInputModel.AllowTicketExchangePurchase = cbAllowPurchase.Checked
        _confirmInputModel.AllowTicketExchangeReturn = cbAllowPlaceOnSale.Checked
        _confirmInputModel.MaximumResalePrice = Utilities.CheckForDBNull_Decimal(txtProductmaxPrice.Text)
        _confirmInputModel.MinimumResalePrice = Utilities.CheckForDBNull_Decimal(txtProductMinPrice.Text)
        _confirmInputModel.CustomerRetainsMaxLimit = cbCustomerRetainsMaximumLimit.Checked
        _confirmInputModel.CustomerRetainsPrerequisite = cbCustomerRetainsPrerequisite.Checked
        _confirmInputModel.ClubFee = Utilities.CheckForDBNull_Decimal(txtClubFee.Text)

        _confirmInputModel.ClubFeePercentageOrFixed = ddlClubFeeType.SelectedItem.Value
        _confirmInputModel.MinMaxBoundaryPercentageOrFixed = ddlMinMaxBoundaryType.SelectedItem.Value
        _confirmInputModel.StandAreaDefaults = New List(Of TicketExchangeStandAreaDefaults)
        _confirmInputModel.ProductCode = hdfProductCode.Value
        Dim count As Integer = 0
        For Each item As RepeaterItem In rptTicketExchangeDefaults.Items
            Dim Sad As New TicketExchangeStandAreaDefaults
            Dim cbAllowTESales As CheckBox = CType(item.FindControl("cbAllowTESales"), CheckBox)
            Dim cbAllowTEReturns As CheckBox = CType(item.FindControl("cbAllowTEReturns"), CheckBox)
            Dim hdfAllowTESales As HiddenField = CType(item.FindControl("hdfAllowTESales"), HiddenField)
            Dim hdfAllowTEReturns As HiddenField = CType(item.FindControl("hdfAllowTEReturns"), HiddenField)
            Dim hdfStand As HiddenField = CType(item.FindControl("hdfStand"), HiddenField)
            Dim hdfArea As HiddenField = CType(item.FindControl("hdfArea"), HiddenField)
            count = count + 1
            If (cbAllowTESales.Checked And hdfAllowTESales.Value = "False") Or (Not cbAllowTESales.Checked And hdfAllowTESales.Value = "True") _
            Or (cbAllowTEReturns.Checked And hdfAllowTEReturns.Value = "False") Or (Not cbAllowTEReturns.Checked And hdfAllowTEReturns.Value = "True") Then
                Sad.AreaCode = hdfArea.Value
                Sad.StandCode = hdfStand.Value
                Sad.AllowTicketExchangePurchase = cbAllowTESales.Checked
                Sad.AllowTicketExchangeReturn = cbAllowTEReturns.Checked
                _confirmInputModel.StandAreaDefaults.Add(Sad)
            End If
        Next

        Dim TicketExchangeDefaultsBuilder As New TicketExchangeDefaultsBuilder()
        _confirmViewModel = TicketExchangeDefaultsBuilder.TicketingExchangeDefaultsConfirm(_confirmInputModel)
        If _confirmViewModel.Error.HasError Then
            blErrorList.Items.Add(_confirmViewModel.Error.ErrorMessage)
        Else
            blSuccessList.Items.Add(_confirmViewModel.GetPageText("SuccesfulUpdate"))
        End If
    End Sub
    Protected Sub PopulateData()

        ' updatable fields
        If ddlProductCode.SelectedIndex > 0 Then
            hdfProductCode.Value = ddlProductCode.SelectedItem.Value
        End If
        cbAllowPlaceOnSale.Checked = Utilities.convertToBool(_viewModel.AllowTicketExchangeReturn)
        cbAllowPurchase.Checked = Utilities.convertToBool(_viewModel.AllowTicketExchangePurchase)
        txtProductMinPrice.Text = Utilities.CheckForDBNull_Decimal(_viewModel.MinimumResalePrice)
        txtProductmaxPrice.Text = Utilities.CheckForDBNull_Decimal(_viewModel.MaximumResalePrice)
        txtClubFee.Text = Utilities.CheckForDBNull_Decimal(_viewModel.ClubFee)
        cbCustomerRetainsPrerequisite.Checked = Utilities.convertToBool(_viewModel.CustomerRetainsPrerequisite)
        cbCustomerRetainsMaximumLimit.Checked = Utilities.convertToBool(_viewModel.CustomerRetainsMaxLimit)


        If ddlClubFeeType.Items.FindByValue(_viewModel.ClubFeePercentageOrFixed) IsNot Nothing Then
            ddlClubFeeType.SelectedValue = _viewModel.ClubFeePercentageOrFixed
        End If

        If ddlClubFeeType.Items.FindByValue(_viewModel.MinMaxBoundaryPercentageOrFixed) IsNot Nothing Then
            ddlMinMaxBoundaryType.SelectedValue = _viewModel.MinMaxBoundaryPercentageOrFixed
        End If
        'output only
        SumOfTicketsAllocatedSold.Text = Utilities.CheckForDBNull_Int(_viewModel.SumOfTicketsAllocatedSold) 'PYRF09 <> 0 AND STAT09 = '1' - product level only 
        SumOfTicketsBooked.Text = Utilities.CheckForDBNull_Int(_viewModel.SumOfTicketsBooked) ' CATG09 = '02' - products level only 
        SumOfTicketsPendingOnTicketExchange.Text = Utilities.CheckForDBNull_Int(_viewModel.SumOfTicketsPendingOnTicketExchange) 'ACTL90 <> 'R' AND MD8.MDTE08 < wToday
        SumOfTicketsExpiredOnTicketExchange.Text = Utilities.CheckForDBNull_Int(_viewModel.SumOfTicketsExpiredOnTicketExchange)
        SumOfTicketsSoldOnTicketExchange.Text = Utilities.CheckForDBNull_Int(_viewModel.SumOfTicketsSoldOnTicketExchange)

        ValueOfTicketsPendingOnTicketExchange.Text = FormatCurrency(_viewModel.ValueOfTicketsPendingOnTicketExchange)
        ValueOfTicketsExpiredOnTicketExchange.Text = FormatCurrency(_viewModel.ValueOfTicketsExpiredOnTicketExchange)
        ValueOfTicketsSoldOnTicketExchange.Text = FormatCurrency(_viewModel.ValueOfTicketsSoldOnTicketExchange)
        ValueOfTEFeesPendingOnTicketExchange.Text = FormatCurrency(_viewModel.ValueOfTeFeesPendingOnTicketExchange)
        ValueOfTEFeesExpiredOnTicketExchange.Text = FormatCurrency(_viewModel.ValueOfTeFeesExpiredOnTicketExchange)
        ValueOfTEFeesSoldOnTicketExchange.Text = FormatCurrency(_viewModel.ValueOfTeFeesSoldOnTicketExchange)

        If Not IsPostBack Then
            ddlProductCode.DataSource = _viewModel.TicketExchangeProductsList
            ddlProductCode.DataTextField = "productMask"
            ddlProductCode.DataValueField = "productCode"
            ddlProductCode.DataBind()
            Dim ddlItem As New ListItem(ltlAllProducts, ltlAllProducts)
            ddlProductCode.Items.Insert(0, ddlItem)
        End If

        ' Populate repeater of stand and areas 
        rptTicketExchangeDefaults.DataSource = _viewModel.TicketExchangeStandAreaDefaultsList
        rptTicketExchangeDefaults.DataBind()
        rptTicketExchangeDefaults.Visible = True

    End Sub

    Private Sub ProcessErrors()
        If _viewModel.Error IsNot Nothing AndAlso _viewModel.Error.HasError Then
            blErrorList.Items.Add(_viewModel.Error.ErrorMessage)
        End If
    End Sub

    Protected Sub rptTicketExchangeDefaults_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTicketExchangeDefaults.ItemDataBound

        Dim TicketExchangeStandAreaDefaults As TicketExchangeStandAreaDefaults = DirectCast(e.Item.DataItem, TicketExchangeStandAreaDefaults)
        If e.Item.ItemType = ListItemType.Header Then

        End If

        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim cbAllowTESales As CheckBox = CType(e.Item.FindControl("cbAllowTESales"), CheckBox)
            Dim cbAllowTERetrurns As CheckBox = CType(e.Item.FindControl("cbAllowTEReturns"), CheckBox)
            Dim hdfStand As HiddenField = CType(e.Item.FindControl("hdfStand"), HiddenField)
            Dim hdfArea As HiddenField = CType(e.Item.FindControl("hdfArea"), HiddenField)
            Dim hdfAllowTESales As HiddenField = CType(e.Item.FindControl("hdfAllowTESales"), HiddenField)
            Dim hdfAllowTEReturns As HiddenField = CType(e.Item.FindControl("hdfAllowTEReturns"), HiddenField)
            Dim lblStandAreaMask As Label = CType(e.Item.FindControl("lblStandAreaMask"), Label)
            Dim StandAreaMask As New StringBuilder(_viewModel.GetPageText("lblStandAreaMask"))
            StandAreaMask.Replace("<<StandCode>>", Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.StandCode))
            StandAreaMask.Replace("<<StandDescription>>", Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.StandDescription))
            StandAreaMask.Replace("<<AreaCode>>", Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.AreaCode))
            StandAreaMask.Replace("<<AreaDescription>>", Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.AreaDescription))
            StandAreaMask.Replace("<<AreaDescriptionAdditional>>", Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.AreaDescriptionAdditional))
            lblStandAreaMask.Text = StandAreaMask.ToString
            cbAllowTESales.Checked = Utilities.convertToBool(TicketExchangeStandAreaDefaults.AllowTicketExchangePurchase)
            cbAllowTERetrurns.Checked = Utilities.convertToBool(TicketExchangeStandAreaDefaults.AllowTicketExchangeReturn)
            hdfStand.Value = Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.StandCode)
            hdfArea.Value = Utilities.CheckForDBNull_String(TicketExchangeStandAreaDefaults.AreaCode)
            hdfAllowTESales.Value = Utilities.convertToBool(TicketExchangeStandAreaDefaults.AllowTicketExchangePurchase)
            hdfAllowTEReturns.Value = Utilities.convertToBool(TicketExchangeStandAreaDefaults.AllowTicketExchangeReturn)

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
#End Region

End Class
