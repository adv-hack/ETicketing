Imports TalentBusinessLogic.ModelBuilders
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.BusinessObjects.Definitions
Imports System.Collections.Generic
Imports TalentBusinessLogic.DataTransferObjects
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class TalentEBusiness_UserControls_TicketExchangeSeatList
    Inherits ControlBase

#Region "Public Properties"

    Public ViewModel As TicketExchangeSelectionViewModel
    Public ListType As String
    Public SeatList As List(Of TicketExchangeItem)
    Public TEStage As Integer

    Public StatusHeader As String = String.Empty
    Public StatusInfo As String = String.Empty
    Public SelectHeader As String = String.Empty
    Public ProductHeader As String = String.Empty
    Public SeatHeader As String = String.Empty
    Public SeatedCustomerHeader As String = String.Empty
    Public PaymentOwnerHeader As String = String.Empty
    Public FaceValueHeader As String = String.Empty
    Public PriceHeader As String = String.Empty
    Public SalePriceHeader As String = String.Empty
    Public ClubFeeHeader As String = String.Empty
    Public YouWillEarnHeader As String = String.Empty
    Public PaymentRefHeader As String = String.Empty
    Public TicketExchangeIdHeader As String = String.Empty

    Public GenericCol1HeaderInfo As String = String.Empty
    Public GenericCol2HeaderInfo As String = String.Empty
    Public GenericCol3HeaderInfo As String = String.Empty
    Public StatusHeaderInfo As String = String.Empty
    Public SelectHeaderInfo As String = String.Empty
    Public ProductHeaderInfo As String = String.Empty
    Public SeatHeaderInfo As String = String.Empty
    Public SeatedCustomerHeaderInfo As String = String.Empty
    Public PaymentOwnerHeaderInfo As String = String.Empty
    Public FaceValueHeaderInfo As String = String.Empty
    Public PriceHeaderInfo As String = String.Empty
    Public SalePriceHeaderInfo As String = String.Empty
    Public ClubFeeHeaderInfo As String = String.Empty
    Public YouWillEarnHeaderInfo As String = String.Empty
    Public PaymentRefHeaderInfo As String = String.Empty
    Public TicketExchangeIdHeaderInfo As String = String.Empty

    Public SliderStepNumber As String = String.Empty
    Public MainSliderInitialStart As String = String.Empty
    Public MainSliderMinPrice As String = String.Empty
    Public MainSliderMaxPrice As String = String.Empty

#End Region

#Region "Class Level Fields"
    Private _showProduct As Boolean
    Private _showSeat As Boolean
    Private _showCustomer As Boolean
    Private _showOwner As Boolean
    Private _showFaceValue As Boolean
    Private _showSalePrice As Boolean
    Private _showClubFee As Boolean
    Private _showYouWillEarn As Boolean
    Private _showSelect As Boolean
    Private _showTicketExchangeId As Boolean
    Private _showStatusInfo As Boolean
    Private _showPaymentRef As Boolean

    Private _soldStatus As String
    Private _currentlyOnSaleStatus As String
    Private _currentlyNotOnSaleStatus As String
    Private _takingOffSaleStatus As String
    Private _placingOnSaleStatus As String
    Private _priceChangeStatus As String

    Private _minMainBoundaryPlaceholder As String = String.Empty
    Private _maxMainBoundaryPlaceholder As String = String.Empty
    Private _feeTypeAndValuePlaceholder As String = String.Empty
    Private _productMask As String = String.Empty
    Private _seatedCustomerMask As String = String.Empty
    Private _paymentOwnerMask As String = String.Empty
    Private _seatMask As String = String.Empty

    Structure controlType
        'Type of club fee for Ticket Exchange

        Public Const Selection As String = "Selection"
        Public Const PlaceOnSale As String = "PlaceOnSale"
        Public Const TakeOffSale As String = "TakeOffSale"
        Public Const PriceChange As String = "PriceChange"
    End Structure

    Structure valueType
        'Type of club fee or boundary value for Ticket Exchange
        Public Const FixedPrice As String = "F"
        Public Const percentage As String = "P"
    End Structure

    Private Enum stage
        None = 0
        SelectTickets = 1
        Review = 2
        Confirm = 3
    End Enum

#End Region

#Region "Protected Page Events"


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.Visible Then
            If ViewModel IsNot Nothing Then
                populateTextAndAttributes()
                ConfigureTableForListMode()
                setExchangeSelectionRepeater()
            End If
        End If
    End Sub

    Protected Sub rptTicketExchangeSelection_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTicketExchangeSeatList.ItemDataBound

        Dim TEseat As TicketExchangeItem = CType(e.Item.DataItem, TicketExchangeItem)
        Dim lblFormattedSeat As Label = CType(e.Item.FindControl("lblFormattedSeat"), Label)
        Dim chkStatus As CheckBox = CType(e.Item.FindControl("chkStatus"), CheckBox)

        Dim txtResaleSlider As TextBox = CType(e.Item.FindControl("txtResaleSlider"), TextBox)
        Dim lblStatus As Label = CType(e.Item.FindControl("lblStatus"), Label)
        Dim lblSeatedCustomer As Label = CType(e.Item.FindControl("lblSeatedCustomer"), Label)
        Dim lblPaymentOwner As Label = CType(e.Item.FindControl("lblPaymentOwner"), Label)
        Dim lblProduct As Label = CType(e.Item.FindControl("lblProduct"), Label)
        Dim lblSeat As Label = CType(e.Item.FindControl("lblSeat"), Label)
        Dim lblClubFee As Label = CType(e.Item.FindControl("lblClubFee"), Label)
        Dim lblYouWillEarn As Label = CType(e.Item.FindControl("lblYouWillEarn"), Label)
        Dim lblFaceValuePrice As Label = CType(e.Item.FindControl("lblFaceValuePrice"), Label)

        Dim lblSalePrice As Label = CType(e.Item.FindControl("lblSalePrice"), Label)
        Dim lblPaymentRef As Label = CType(e.Item.FindControl("lblPaymentRef"), Label)
        Dim lblTicketExchangeId As Label = CType(e.Item.FindControl("lblTicketExchangeId"), Label)

        Dim plhPriceSelection As PlaceHolder = CType(e.Item.FindControl("plhPriceSelection"), PlaceHolder)
        Dim divPriceSlider As HtmlGenericControl = CType(e.Item.FindControl("divPriceSlider"), HtmlGenericControl)
        Dim spanSliderHandle As HtmlGenericControl = CType(e.Item.FindControl("spanSliderHandle"), HtmlGenericControl)

        Dim lblResaleValue As Label = CType(e.Item.FindControl("lblResaleValue"), Label)

        Dim plhProductHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhProductHeaderInfo"), PlaceHolder)
        Dim plhProduct As PlaceHolder = CType(e.Item.FindControl("plhProduct"), PlaceHolder)
        Dim plhProductHeader As PlaceHolder = CType(e.Item.FindControl("plhProductHeader"), PlaceHolder)

        Dim plhSeatHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhSeatHeaderInfo"), PlaceHolder)
        Dim plhSeat As PlaceHolder = CType(e.Item.FindControl("plhSeat"), PlaceHolder)
        Dim plhSeatHeader As PlaceHolder = CType(e.Item.FindControl("plhSeatHeader"), PlaceHolder)

        Dim plhSelectHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhSelectHeaderInfo"), PlaceHolder)
        Dim plhSelect As PlaceHolder = CType(e.Item.FindControl("plhSelect"), PlaceHolder)
        Dim plhSelectHeader As PlaceHolder = CType(e.Item.FindControl("plhSelectHeader"), PlaceHolder)

        Dim plhCustomerHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhCustomerHeaderInfo"), PlaceHolder)
        Dim plhCustomer As PlaceHolder = CType(e.Item.FindControl("plhCustomer"), PlaceHolder)
        Dim plhSeatedCustomerHeader As PlaceHolder = CType(e.Item.FindControl("plhSeatedCustomerHeader"), PlaceHolder)

        Dim plhOwnerHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhOwnerHeaderInfo"), PlaceHolder)
        Dim plhOwner As PlaceHolder = CType(e.Item.FindControl("plhOwner"), PlaceHolder)
        Dim plhPaymentOwnerHeader As PlaceHolder = CType(e.Item.FindControl("plhPaymentOwnerHeader"), PlaceHolder)

        Dim plhFaceValueHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhFaceValueHeaderInfo"), PlaceHolder)
        Dim plhFaceValue As PlaceHolder = CType(e.Item.FindControl("plhFaceValue"), PlaceHolder)
        Dim plhFaceValueHeader As PlaceHolder = CType(e.Item.FindControl("plhFaceValueHeader"), PlaceHolder)

        Dim plhPriceHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhPriceHeaderInfo"), PlaceHolder)
        Dim plhPrice As PlaceHolder = CType(e.Item.FindControl("plhPrice"), PlaceHolder)
        Dim plhPriceHeader As PlaceHolder = CType(e.Item.FindControl("plhPriceHeader"), PlaceHolder)

        Dim plhclubfeeHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhClubFeeHeaderInfo"), PlaceHolder)
        Dim plhclubfee As PlaceHolder = CType(e.Item.FindControl("plhClubFee"), PlaceHolder)
        Dim plhclubfeeHeader As PlaceHolder = CType(e.Item.FindControl("plhClubFeeHeader"), PlaceHolder)

        Dim plhYouWillEarnHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhYouWillEarnHeaderInfo"), PlaceHolder)
        Dim plhYouWillEarn As PlaceHolder = CType(e.Item.FindControl("plhYouWillEarn"), PlaceHolder)
        Dim plhYouWillEarnHeader As PlaceHolder = CType(e.Item.FindControl("plhYouWillEarnHeader"), PlaceHolder)

        Dim plhSalepriceHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhSalePriceHeaderInfo"), PlaceHolder)
        Dim plhSaleprice As PlaceHolder = CType(e.Item.FindControl("plhSalePrice"), PlaceHolder)
        Dim plhSalepriceHeader As PlaceHolder = CType(e.Item.FindControl("plhSalePriceHeader"), PlaceHolder)

        Dim plhPaymentRefHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhPaymentRefHeaderInfo"), PlaceHolder)
        Dim plhPaymentRef As PlaceHolder = CType(e.Item.FindControl("plhPaymentRef"), PlaceHolder)
        Dim plhPaymentRefHeader As PlaceHolder = CType(e.Item.FindControl("plhPaymentRefHeader"), PlaceHolder)

        Dim plhResaleValueHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhResaleValueHeaderInfo"), PlaceHolder)
        Dim plhResaleValue As PlaceHolder = CType(e.Item.FindControl("plhResaleValue"), PlaceHolder)
        Dim plhResaleValueHeader As PlaceHolder = CType(e.Item.FindControl("plhResaleValueHeader"), PlaceHolder)

        Dim plhTicketExchangeIdHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhTicketExchangeIdHeaderInfo"), PlaceHolder)
        Dim plhTicketExchangeId As PlaceHolder = CType(e.Item.FindControl("plhTicketExchangeId"), PlaceHolder)
        Dim plhTicketExchangeIdHeader As PlaceHolder = CType(e.Item.FindControl("plhTicketExchangeIdHeader"), PlaceHolder)

        Dim plhStatusHeaderInfo As PlaceHolder = CType(e.Item.FindControl("plhStatusHeaderInfo"), PlaceHolder)
        Dim plhStatusInfo As PlaceHolder = CType(e.Item.FindControl("plhStatusInfo"), PlaceHolder)

        Dim hdfOriginalResaleValue As HiddenField = CType(e.Item.FindControl("hdfOriginalResaleValue"), HiddenField)
        Dim hdfOriginalStatus As HiddenField = CType(e.Item.FindControl("hdfOriginalStatus"), HiddenField)
        Dim hdfOriginalChecked As HiddenField = CType(e.Item.FindControl("hdfOriginalChecked"), HiddenField)
        Dim hdfOriginalPrice As HiddenField = CType(e.Item.FindControl("hdfOriginalPrice"), HiddenField)
        Dim hdfFaceValue As HiddenField = CType(e.Item.FindControl("hdfFaceValue"), HiddenField)
        Dim hdfProductFeeValue As HiddenField = CType(e.Item.FindControl("hdfProductFeeValue"), HiddenField)
        Dim hdfProductFeeType As HiddenField = CType(e.Item.FindControl("hdfProductFeeType"), HiddenField)

        ' Set visibility and informaiton text for headers.
        If e.Item.ItemType = ListItemType.Header Then

            plhSelectHeader.Visible = _showSelect
            If Trim(SelectHeaderInfo) = String.Empty Then
                plhSelectHeaderInfo.Visible = False
            End If

            plhProductHeader.Visible = _showProduct
            If Trim(ProductHeaderInfo) = String.Empty Then
                plhProductHeaderInfo.Visible = False
            End If

            plhSeatHeader.Visible = _showSeat
            If Trim(SeatHeaderInfo) = String.Empty Then
                plhSeatHeaderInfo.Visible = False
            End If

            plhSeatedCustomerHeader.Visible = _showCustomer
            If Trim(SeatedCustomerHeaderInfo) = String.Empty Then
                plhCustomerHeaderInfo.Visible = False
            End If

            plhPaymentOwnerHeader.Visible = _showOwner
            If Trim(PaymentOwnerHeaderInfo) = String.Empty Then
                plhOwnerHeaderInfo.Visible = False
            End If

            plhFaceValueHeader.Visible = _showFaceValue
            If Trim(FaceValueHeaderInfo) = String.Empty Then
                plhFaceValueHeaderInfo.Visible = False
            End If

            If Trim(StatusHeaderInfo) = String.Empty Then
                plhStatusHeaderInfo.Visible = False
            End If

            If Trim(SelectHeaderInfo) = String.Empty Then
                plhSelectHeaderInfo.Visible = False
            End If

            plhclubfeeHeader.Visible = _showClubFee
            If Trim(ClubFeeHeaderInfo) = String.Empty Then
                plhclubfeeHeaderInfo.Visible = False
            End If

            plhTicketExchangeIdHeader.Visible = _showTicketExchangeId
            If Trim(TicketExchangeIdHeaderInfo) = String.Empty Then
                plhTicketExchangeIdHeaderInfo.Visible = False
            End If

            plhPaymentRefHeader.Visible = _showPaymentRef
            If Trim(PaymentRefHeaderInfo) = String.Empty Then
                plhPaymentRefHeaderInfo.Visible = False
            End If

            plhSalepriceHeader.Visible = _showSalePrice
            If Trim(SalePriceHeaderInfo) = String.Empty Then
                plhSalepriceHeaderInfo.Visible = False
            End If

            plhYouWillEarnHeader.Visible = _showYouWillEarn
            If Trim(YouWillEarnHeaderInfo) = String.Empty Then
                plhYouWillEarnHeaderInfo.Visible = False
            End If

            plhTicketExchangeIdHeader.Visible = _showTicketExchangeId
            If Trim(TicketExchangeIdHeaderInfo) = String.Empty Then
                plhTicketExchangeIdHeaderInfo.Visible = False
            End If

        End If
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then

            Dim seatDetails As New DESeatDetails

            Select Case TEseat.Status
                Case GlobalConstants.TicketExchangeItemStatus.Sold
                    chkStatus.Checked = True
                    lblStatus.Text = _soldStatus
                Case GlobalConstants.TicketExchangeItemStatus.OnSale
                    chkStatus.Checked = True
                    lblStatus.Text = _currentlyOnSaleStatus
                Case GlobalConstants.TicketExchangeItemStatus.NotOnSale
                    chkStatus.Checked = False
                    lblStatus.Text = _currentlyNotOnSaleStatus
                Case GlobalConstants.TicketExchangeItemStatus.PlacingOnSale
                    chkStatus.Checked = True
                    lblStatus.Text = _placingOnSaleStatus
                Case GlobalConstants.TicketExchangeItemStatus.PriceChanged
                    chkStatus.Checked = True
                    lblStatus.Text = _priceChangeStatus
                Case GlobalConstants.TicketExchangeItemStatus.TakingOffSale
                    chkStatus.Checked = False
                    lblStatus.Text = _takingOffSaleStatus
            End Select

            plhStatusInfo.Visible = _showStatusInfo AndAlso (Not TEseat.AdditionalInfo Is Nothing AndAlso TEseat.AdditionalInfo.Trim <> String.Empty)

            If _showSelect Then
                chkStatus.Attributes.Add("onClick", "flagChange('" + lblStatus.ClientID + "');")
            End If

            'column visability control
            plhSelect.Visible = _showSelect
            plhProduct.Visible = _showProduct
            plhSeat.Visible = _showSeat
            plhOwner.Visible = _showOwner
            plhPaymentRef.Visible = _showPaymentRef
            plhSaleprice.Visible = _showSalePrice
            plhCustomer.Visible = _showCustomer
            plhFaceValue.Visible = _showFaceValue
            plhclubfee.Visible = _showClubFee
            plhYouWillEarn.Visible = _showYouWillEarn
            plhTicketExchangeId.Visible = _showTicketExchangeId
            plhPriceSelection.Visible = True

            Dim MaskString As String
            MaskString = _seatMask
            MaskString = MaskString.Replace("<<Stand>>", TEseat.StandCode.Trim)
            MaskString = MaskString.Replace("<<Area>>", TEseat.AreaCode.Trim)
            MaskString = MaskString.Replace("<<RowNo>>", TEseat.RowNo.Trim)
            MaskString = MaskString.Replace("<<Seat>>", TEseat.SeatNo.Trim)
            MaskString = MaskString.Replace("<<Alpha>>", TEseat.AlphaSuffix.Trim)
            MaskString = MaskString.Replace("<<AreaDescription>>", TEseat.AreaDescription.Trim)
            MaskString = MaskString.Replace("<<StandDescription>>", TEseat.StandDescription.Trim)
            lblSeat.Text = MaskString

            MaskString = _productMask
            MaskString = MaskString.Replace("<<ProductCode>>", TEseat.ProductCode.Trim)
            MaskString = MaskString.Replace("<<ProductDescription>>", TEseat.ProductDescription.Trim)
            MaskString = MaskString.Replace("<<ProductDate>>", TEseat.ProductDate.Trim)
            lblProduct.Text = MaskString

            MaskString = _seatedCustomerMask
            MaskString = MaskString.Replace("<<CustomerNumber>>", TEseat.SeatedCustomerNo.TrimStart("0"c).Trim)
            MaskString = MaskString.Replace("<<CustomerName>>", TEseat.SeatedCustomerName.Trim)
            lblSeatedCustomer.Text = MaskString

            MaskString = _paymentOwnerMask
            MaskString = MaskString.Replace("<<CustomerNumber>>", TEseat.PaymentOwnerCustomerNo.TrimStart("0"c).Trim)
            MaskString = MaskString.Replace("<<CustomerName>>", TEseat.PaymentOwnerCustomerName.Trim)
            lblPaymentOwner.Text = MaskString

            ' TE Records which are previously confirmed or resold needs to retrieve the fee and resale values from what was previously confirmed.
            ' Otherwise these need to be calculated based on current system values.
            ' Also display if we are confirming a new sale or price change
            If TEseat.Status = GlobalConstants.TicketExchangeItemStatus.OnSale Or TEseat.Status = GlobalConstants.TicketExchangeItemStatus.Sold Or
                TEseat.Status = GlobalConstants.TicketExchangeItemStatus.PlacingOnSale Or TEseat.Status = GlobalConstants.TicketExchangeItemStatus.PriceChanged Then
                lblClubFee.Text = FormatCurrency(TEseat.Fee)
                lblYouWillEarn.Text = FormatCurrency(TEseat.PotentialEarning)
            Else
                lblClubFee.Text = String.Empty
                lblYouWillEarn.Text = String.Empty
            End If

            If ListType = controlType.Selection AndAlso TEseat.AllowPriceChange Then
                lblResaleValue.Visible = False
                divPriceSlider.Attributes.Add("data-start", TEseat.MinPrice)
                divPriceSlider.Attributes.Add("data-end", TEseat.MaxPrice)
                divPriceSlider.Attributes.Add("data-step", SliderStepNumber)
                divPriceSlider.Attributes.Add("data-decimal", 2)
                divPriceSlider.Attributes.Add("class", "slider ebiz-price-slider ebiz-price-slider_" & e.Item.ItemIndex.ToString())
                spanSliderHandle.Attributes.Add("aria-controls", txtResaleSlider.ClientID)
                txtResaleSlider.Attributes.Add("class", "ebiz-price-slider-value ebiz-price-slider-value_" & e.Item.ItemIndex.ToString())

                If TEseat.RequestedPrice <> 0 Then
                    txtResaleSlider.Text = TEseat.RequestedPrice.ToString()
                    divPriceSlider.Attributes.Add("data-initial-start", TEseat.RequestedPrice)
                Else
                    txtResaleSlider.Text = TEseat.FaceValuePrice.ToString()
                    divPriceSlider.Attributes.Add("data-initial-start", TEseat.FaceValuePrice)
                End If

            Else
                lblResaleValue.Visible = True
                divPriceSlider.Attributes.Add("class", "slider disabled ebiz-price-slider ebiz-price-slider_" & e.Item.ItemIndex.ToString())
                divPriceSlider.Style.Add("display", "none")
                txtResaleSlider.Enabled = False
                txtResaleSlider.Visible = False
                txtResaleSlider.ReadOnly = True
            End If

            'There are certain item level conditions to restric the checkbox and/or price slider.
            If Not TEseat.AllowStatusChange Then
                chkStatus.Enabled = False
            End If

            lblResaleValue.Text = FormatCurrency(TEseat.RequestedPrice)
            lblFaceValuePrice.Text = FormatCurrency(TEseat.FaceValuePrice)
            lblSalePrice.Text = FormatCurrency(TEseat.OriginalSalePrice)
            lblPaymentRef.Text = TEseat.PaymentRef
            lblTicketExchangeId.Text = TEseat.TicketExchangeId.ToString.TrimStart("0")

            'Store these so we know what seats have been changed 
            hdfOriginalPrice.Value = TEseat.OriginalSalePrice
            hdfFaceValue.Value = TEseat.FaceValuePrice
            hdfProductFeeValue.Value = TEseat.ProductClubHandlingFee
            hdfProductFeeType.Value = TEseat.ProductClubHandlingFeeType
            hdfOriginalStatus.Value = TEseat.OriginalStatus
            hdfOriginalResaleValue.Value = TEseat.OriginalResalePrice

            Select Case TEseat.OriginalStatus
                Case GlobalConstants.TicketExchangeItemStatus.Sold
                    hdfOriginalChecked.Value = True
                Case GlobalConstants.TicketExchangeItemStatus.OnSale
                    hdfOriginalChecked.Value = True
                Case GlobalConstants.TicketExchangeItemStatus.NotOnSale
                    hdfOriginalChecked.Value = False
            End Select

        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Populate the text properties
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateTextAndAttributes()

        If ViewModel.OverallProductFeeType = valueType.FixedPrice Then
            _feeTypeAndValuePlaceholder = GetCurrencySymbol() + ViewModel.OverallProductFeeValue.ToString()
        End If
        If ViewModel.OverallProductFeeType = valueType.percentage Then
            _feeTypeAndValuePlaceholder = ViewModel.OverallProductFeeValue.ToString() + "%"
        End If

        _minMainBoundaryPlaceholder = GetCurrencySymbol() + ViewModel.MainSliderMinPrice.ToString()
        _maxMainBoundaryPlaceholder = GetCurrencySymbol() + ViewModel.MainSliderMaxPrice.ToString()

        _soldStatus = ViewModel.GetPageText("ltlSold")
        _currentlyOnSaleStatus = ViewModel.GetPageText("ltlCurrentlyOnSale")
        _currentlyNotOnSaleStatus = ViewModel.GetPageText("ltlCurrentlyOffSale")
        _placingOnSaleStatus = ViewModel.GetPageText("ltlPlacingOnSale")
        _takingOffSaleStatus = ViewModel.GetPageText("ltlTakingOffSale")
        _priceChangeStatus = ViewModel.GetPageText("ltlPriceChanged")

        hdfCurrencySymbol.Value = GetCurrencySymbol()
        hdfCurrentlyOffSaleText.Value = _currentlyNotOnSaleStatus
        hdfCurrentlyOnSaleText.Value = _currentlyOnSaleStatus
        hdfTakingOffSaleText.Value = _takingOffSaleStatus
        hdfPlacingOnSaleText.Value = _placingOnSaleStatus
        hdfPriceChangeText.Value = _priceChangeStatus
        hdfSoldText.Value = _soldStatus

        ProductHeader = ViewModel.GetPageText("ProductHeader")
        SeatHeader = ViewModel.GetPageText("SeatHeader")
        SeatedCustomerHeader = ViewModel.GetPageText("SeatedCustomerHeader")
        PaymentOwnerHeader = ViewModel.GetPageText("PaymentOwnerHeader")
        StatusHeader = ViewModel.GetPageText("StatusHeader")
        FaceValueHeader = ViewModel.GetPageText("FaceValueHeader")
        SelectHeader = ViewModel.GetPageText("SelectHeader")
        SalePriceHeader = ViewModel.GetPageText("SalepriceHeader")
        PaymentRefHeader = ViewModel.GetPageText("PaymentRefHeader")
        YouWillEarnHeader = ViewModel.GetPageText("YouWillEarnHeader")
        TicketExchangeIdHeader = ViewModel.GetPageText("TicketExchangeIdHeader")

        'Heraders with Masks
        PriceHeader = ViewModel.GetPageText("PriceHeader")
        PriceHeader = PriceHeader.Replace("<<ProductMinPrice>>", _minMainBoundaryPlaceholder)
        PriceHeader = PriceHeader.Replace("<<ProductMaxPrice>>", _maxMainBoundaryPlaceholder)
        ClubFeeHeader = ViewModel.GetPageText("ClubFeeHeader")
        ClubFeeHeader = ClubFeeHeader.Replace("<<FeeValueOrPercentage>>", _feeTypeAndValuePlaceholder)

        _showProduct = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowProduct"))
        _showSeat = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowSeat"))
        _showCustomer = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowCustomer"))
        _showOwner = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowOwner"))
        _showFaceValue = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowFaceValue"))
        _showClubFee = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowClubFee"))
        _showYouWillEarn = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("Showyouwillearn"))
        _showSalePrice = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowSaleprice"))
        _showPaymentRef = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowPaymentRef"))
        _showTicketExchangeId = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ViewModel.GetPageAttribute("ShowTicketExchangeId"))

        ProductHeaderInfo = ViewModel.GetPageText("ProductHeaderInfo")
        SeatHeaderInfo = ViewModel.GetPageText("SeatHeaderInfo")
        SeatedCustomerHeaderInfo = ViewModel.GetPageText("SeatedCustomerHeaderInfo")
        PaymentOwnerHeaderInfo = ViewModel.GetPageText("PaymentOwnerHeaderInfo")
        FaceValueHeaderInfo = ViewModel.GetPageText("FaceValueHeaderInfo")

        YouWillEarnHeaderInfo = ViewModel.GetPageText("youwillearnHeaderInfo")
        SalePriceHeaderInfo = ViewModel.GetPageText("SalepriceHeaderInfo")
        PaymentRefHeaderInfo = ViewModel.GetPageText("PaymentRefHeaderInfo")
        StatusHeaderInfo = ViewModel.GetPageText("StatusHeaderInfo")
        SelectHeaderInfo = ViewModel.GetPageText("SelectHeaderInfo")
        TicketExchangeIdHeaderInfo = ViewModel.GetPageText("TicketExchangeIdHeaderInfo")

        'Info with Masks
        PriceHeaderInfo = ViewModel.GetPageText("PriceHeaderInfo")
        PriceHeaderInfo = PriceHeaderInfo.Replace("<<ProductMinPrice>>", _minMainBoundaryPlaceholder)
        PriceHeaderInfo = PriceHeaderInfo.Replace("<<ProductMaxPrice>>", _maxMainBoundaryPlaceholder)
        ClubFeeHeaderInfo = ViewModel.GetPageText("ClubFeeHeaderInfo")
        ClubFeeHeaderInfo = ClubFeeHeaderInfo.Replace("<<FeeValueOrPercentage>>", _feeTypeAndValuePlaceholder)

        ltlSetAllTicketsToThisPrice.Text = ViewModel.GetPageText("ltlSetAllTicketsToThisPrice")
        SliderStepNumber = ViewModel.GetPageAttribute("SliderStepNumber")

        'Seat, seated Customer and payment owner are masks
        _seatedCustomerMask = ViewModel.GetPageText("SeatedCustomerMask")
        _paymentOwnerMask = ViewModel.GetPageText("PaymentOwnerMask")
        _seatMask = ViewModel.GetPageText("SeatMask")
        _productMask = ViewModel.GetPageText("ProductMask")

    End Sub

    ''' <summary>
    ''' Set the table based on the mode of the list.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ConfigureTableForListMode()

        'Certain List Modes require fields to be hidden.
        Dim plhMasterSlider As PlaceHolder = CType(Me.FindControl("plhMasterSlider"), PlaceHolder)
        plhMasterSlider.Visible = False
        _showStatusInfo = False
        _showSelect = False
        _showTicketExchangeId = False

        Select Case ListType
            Case controlType.Selection
                _showSelect = True
                _showStatusInfo = True
                _showTicketExchangeId = True
                plhMasterSlider.Visible = True
                ltlHeader.Text = ViewModel.GetPageText("SelectTicketsHeaderText")
            Case controlType.PlaceOnSale
                ltlHeader.Text = ViewModel.GetPageText("PlacingOnSaleHeaderText")
            Case controlType.TakeOffSale
                ltlHeader.Text = ViewModel.GetPageText("TakingOffSaleHeaderText")
                _showClubFee = False
                _showYouWillEarn = False
                _showFaceValue = False
            Case controlType.PriceChange
                ltlHeader.Text = ViewModel.GetPageText("PriceChangeHeaderText")
        End Select
    End Sub

    ''' <summary>
    ''' Set the select seats repeater based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setExchangeSelectionRepeater()
        MainSliderInitialStart = ViewModel.MainSliderInitialValue
        MainSliderMinPrice = ViewModel.MainSliderMinPrice
        MainSliderMaxPrice = ViewModel.MainSliderMaxPrice

        If SeatList IsNot Nothing AndAlso SeatList.Count > 0 Then
            SeatList.Sort(Function(x, y) x.ProductAndSeatCodes.CompareTo(y.ProductAndSeatCodes))
            rptTicketExchangeSeatList.DataSource = SeatList
            rptTicketExchangeSeatList.DataBind()
        Else
            plhTicketExchangeSeatList.Visible = False
        End If
    End Sub

#End Region

#Region "Public Function"
    ''' <summary>
    ''' Accepts a price string as a value and uses the pre-existing Utilities function FormatCurrency which only accepts a Decimal type
    ''' </summary>
    ''' <param name="value">Price as string</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As String) As String
        Dim xValue As Decimal
        If Decimal.TryParse(value, xValue) Then
            value = TDataObjects.PaymentSettings.FormatCurrency(xValue, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        End If
        Return value
    End Function

    Public Function GetCurrencySymbol() As String
        Return TDataObjects.PaymentSettings.GetCurrencySymbol(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
    End Function

#End Region

#Region "Public Procedures"
    Public Sub getAlteredTicketExchangeItems(ByRef ticketxchangeItemList As List(Of TicketExchangeItem))
        Dim selectedItemCount As Integer = 0
        For Each rptItem As RepeaterItem In rptTicketExchangeSeatList.Items
            Dim txtResaleSlider As TextBox = CType(rptItem.FindControl("txtResaleSlider"), TextBox)
            Dim hdfOriginalChecked As HiddenField = CType(rptItem.FindControl("hdfOriginalChecked"), HiddenField)
            Dim chkStatus As CheckBox = CType(rptItem.FindControl("chkStatus"), CheckBox)
            Dim changedStatus As New GlobalConstants.TicketExchangeItemStatus
            Dim itemHasBeenUpdated As Boolean = True
            Dim concessionPriceDifference As Decimal = 0

            Dim TESeat As TicketExchangeItem = ticketxchangeItemList.Item(selectedItemCount)

            'Status changed
            If hdfOriginalChecked.Value <> chkStatus.Checked Then
                If chkStatus.Checked Then
                    changedStatus = GlobalConstants.TicketExchangeItemStatus.PlacingOnSale
                Else
                    changedStatus = GlobalConstants.TicketExchangeItemStatus.TakingOffSale
                End If
            Else
                'Price changed and it is on sale
                If TESeat.AllowPriceChange AndAlso TESeat.OriginalStatus = GlobalConstants.TicketExchangeItemStatus.OnSale AndAlso txtResaleSlider.Text <> TESeat.OriginalResalePrice Then
                    changedStatus = GlobalConstants.TicketExchangeItemStatus.PriceChanged
                Else
                    itemHasBeenUpdated = False
                End If
            End If

            If TESeat.AllowPriceChange Then
                If IsNumeric(txtResaleSlider.Text) Then
                    If CDec(txtResaleSlider.Text) < TESeat.MinPrice Then
                        txtResaleSlider.Text = TESeat.MinPrice
                    End If
                    If CDec(txtResaleSlider.Text) > TESeat.MaxPrice Then
                        txtResaleSlider.Text = TESeat.MaxPrice
                    End If
                Else
                    txtResaleSlider.Text = TESeat.MaxPrice
                End If
            End If

            If itemHasBeenUpdated Then

                If TESeat.AllowPriceChange Then
                    ' Handling Fee is either % of resale or fixed price
                    If TESeat.ProductClubHandlingFeeType = valueType.percentage Then
                        TESeat.Fee =
                            Talent.eCommerce.Utilities.RoundToValue((CDec(txtResaleSlider.Text) * TESeat.ProductClubHandlingFee) / 100, 0.01, True)
                        TESeat.FeeType = valueType.percentage
                    End If
                    If TESeat.ProductClubHandlingFeeType = valueType.FixedPrice Then
                        TESeat.Fee = TESeat.ProductClubHandlingFee
                        TESeat.FeeType = valueType.FixedPrice
                    End If

                    ' If there is a difference between the face value (what the ticket will be resold based upon) and the value the customer purchased it at, then this gets factored into
                    ' what the customer can potentially earn.
                    If TESeat.FaceValuePrice > TESeat.OriginalSalePrice Then
                        concessionPriceDifference = TESeat.FaceValuePrice - TESeat.OriginalSalePrice
                    End If
                    TESeat.PotentialEarning = Math.Max(0, CDec(txtResaleSlider.Text) - concessionPriceDifference - TESeat.Fee)
                    TESeat.RequestedPrice = CDec(txtResaleSlider.Text)
                End If
                TESeat.Status = changedStatus
                TESeat.ListedByCustomerNo = Profile.User.Details.Account_No_1
                TESeat.HasChanged = True
            Else
                TESeat.HasChanged = False
                TESeat.Status = TESeat.OriginalStatus
                TESeat.RequestedPrice = TESeat.OriginalResalePrice
            End If
            selectedItemCount = selectedItemCount + 1
        Next
    End Sub

#End Region

End Class