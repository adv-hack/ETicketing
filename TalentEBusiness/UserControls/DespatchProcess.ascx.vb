Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Linq
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities

Partial Class UserControls_DespatchProcess
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _deSeatDetails As DESeatDetails = Nothing
    Private _intTabIndex As Integer = 4
    Private _errMessage As TalentErrorMessages = Nothing
    Private _membershipLabel As String
    Private _sentLabel As String
    Private _notForDespatchLabel As String
    Private _agentSettingsObject As DESettings = Nothing
    Private _paymentOrReservationType As String
    Private _talentLogging As New Talent.Common.TalentLogging
    Private _talentLogging_LogPrefix As String = ""
    Private _printForDespatch As Boolean = False
    Private _sbJavaScript As New StringBuilder
    Const PAYMENT_TYPE As String = "P"
    Const RESERVATION_TYPE As String = "R"
    Const MEMBERSHIP_STATUS_CODE As String = "M"
    Const SENT_STATUS_CODE As String = "S"
    Const NOT_FOR_DESPATCH_STATUS_CODE As String = "N"


#End Region

#Region "Public Properties"

    Public Enum UsageType As Integer
        DESPATCH = 0
        ORDERCONFIRMATION = 1
        PURCHASEHISTORY = 2
        PRINT = 3
    End Enum
    Public EventHeaderText As String
    Public TicketTypeHeaderText As String
    Public PrefixHeaderText As String
    Public SeatHeaderText As String
    Public TicketNoHeaderText As String
    Public PrintAllHeaderText As String
    Public StatusHeaderText As String
    Public GiftMessageIcon As String
    Public ScanSeriesIcon As String
    Public TicketLimitError As String
    Public Property Usage() As UsageType
    Public Property PaymentReference As String
    Public Property PrintLabelIcon As String
    Public Property ConfirmLabelIcon As String
    Public Property CompleteLabelIcon As String
    Public Property RestrictedLabelIcon As String
    Public ReadOnly Property MembershipLabel As String
        Get
            Return _membershipLabel
        End Get
    End Property
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim loggingPrefix As New StringBuilder
        InitialiseClassLevelFields()
        loggingPrefix.Append("ASP.NET session ID=").Append(Session.SessionID)
        loggingPrefix.Append(", Basket Header ID=").Append(Profile.Basket.Basket_Header_ID.ToString)
        loggingPrefix.Append(", Agent=").Append(_agentSettingsObject.OriginatingSource)
        If Profile.IsAnonymous Then
            loggingPrefix.Append(", Customer Number=").Append(Profile.UserName)
        Else
            loggingPrefix.Append(", Customer Number=").Append("ANONYMOUS")
        End If
        PrintLabelIcon = TEBUtilities.CheckForDBNull_String(_ucr.Attribute("PrintLabelIcon"))
        ConfirmLabelIcon = TEBUtilities.CheckForDBNull_String(_ucr.Attribute("ConfirmLabelIcon"))
        CompleteLabelIcon = TEBUtilities.CheckForDBNull_String(_ucr.Attribute("CompleteLabelIcon"))
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "table-functions.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("table-functions.js", "/Application/Elements/"), False)
        If TalentDefaults.IsDespatchInPrintMode Then
            Usage = UsageType.PRINT
        End If
        loggingPrefix.Append(" - ")
        _talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _talentLogging_LogPrefix = loggingPrefix.ToString()
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Init", _talentLogging_LogPrefix & "Top of page initialisation", "DespatchLog")

        If Not IsPostBack Then
            If Request.UrlReferrer IsNot Nothing Then
                Session("PreviousPage") = Request.UrlReferrer.ToString
                btnBack.Visible = True
            Else
                btnBack.Visible = False
            End If
            If Not Session("PreviousPage").ToString.ToUpper.Contains("PURCHASEDETAILS.ASPX") Then
                Session("DespatchNoteID") = Nothing
                Session("PaymentReference") = Nothing
                Session("TicketNumber") = Nothing
            End If
            LoadInitialDespatchProcess()
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", _talentLogging_LogPrefix & "Top of page load", "DespatchLog")
        btnFinishOrder.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnFinishOrder))
        Session("ProductDetailsPath") = "DespatchProcess"
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_PreRender", _talentLogging_LogPrefix & "Top of page pre-render", "DespatchLog")
        ' Need to do this in render because need page_init code in Despatch/DespatchProcess.aspx.vb to have run to set useage if called from transaction history 
        SetFieldsDisplayText()

        If rptItemstoDespatch.Items.Count = 0 Then
            Dim newBatchID As Long

            ' If depatch process mode then show search options. If despatch note wil be entered exsiting PY107 records are loaded
            ' If payref entered a new set of PY107 records are created and used to load repeater 
            Select Case Usage
                Case UsageType.DESPATCH
                    pnlSearchOptions.Visible = True
                    txtDespatchNoteID.Focus()
                    btnBack.Visible = False
                    btnDespatchNoteGeneration.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DespatchNoteGenerationButtonVisibility"))

                    ' If from order confirmation or transaction history create a set of PY107 records (with new batch ID) and load the repeater  
                Case UsageType.ORDERCONFIRMATION
                    pnlSearchOptions.Visible = False
                    btnDespatchNoteGeneration.Visible = False
                    btnBack.Visible = False
                    btnClear.Visible = False
                    btnSkipOrder.Visible = False
                    If Not IsPostBack Then
                        txtPaymentReference.Text = PaymentReference.TrimStart(GlobalConstants.LEADING_ZEROS)
                        newBatchID = createDespatchTransactionItemsForPayref(txtPaymentReference.Text)
                        If newBatchID > 0 Then GetOrderDetails(newBatchID, txtPaymentReference.Text, PAYMENT_TYPE)
                    End If
                    plhDespatchProcessInfo.Visible = False
                    plhCourierOptions.Visible = False
                    btnDespatchNoteGeneration.Visible = False
                    plhPrintButton.Visible = False
                Case UsageType.PURCHASEHISTORY
                    If Not IsPostBack Then
                        txtPaymentReference.Text = PaymentReference.TrimStart(GlobalConstants.LEADING_ZEROS)
                        newBatchID = createDespatchTransactionItemsForPayref(txtPaymentReference.Text)
                        If newBatchID > 0 Then GetOrderDetails(newBatchID, txtPaymentReference.Text, PAYMENT_TYPE)
                    End If
                    btnDespatchNoteGeneration.Visible = False
                    txtDespatchNoteID.Focus()
            End Select

        End If

        'Both search options and customer info details are contained within an outter wrapper which needs to be hidden if nothing inside it is visible
        plhSearchAndProcessInfoWrapper.Visible = (pnlSearchOptions.Visible OrElse plhDespatchProcessInfo.Visible)

        'Display a success message for transaction that just been completed?
        ltlSuccessDetails.Text = String.Empty
        Dim boolSuccess As Boolean = False

        plhNotes.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowNotes"))

        Dim sSuccessMsg As String = TEBUtilities.CheckForDBNull_String(_ucr.Content("DespatchProcessSuccessMessage", _languageCode, True))
        Dim sFailureMsg As String = TEBUtilities.CheckForDBNull_String(_ucr.Content("DespatchProcessFailureMessage", _languageCode, True))
        If Session("DespatchProcessSuccess_pyrf") IsNot Nothing AndAlso Not Session("DespatchProcessSuccess_pyrf") = String.Empty Then

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_PreRender", _talentLogging_LogPrefix & "Has despatch process success pay ref = " & Session("DespatchProcessSuccess_pyrf").ToString(), "DespatchLog")
            ' Perform check against back-end TC043TBL records to ensure the token has been stamped against ALL records
            If Session("DespatchProcessSuccess_token") IsNot Nothing AndAlso Not Session("DespatchProcessSuccess_token") = String.Empty AndAlso Session("DespatchProcessSuccess_type") IsNot Nothing AndAlso Not Session("DespatchProcessSuccess_type") = String.Empty Then
                boolSuccess = OrderTokenCheck()
            End If
            If boolSuccess Then
                ltlSuccessDetails.Text = String.Format(sSuccessMsg, Session("DespatchProcessSuccess_pyrf"))
            Else
                ltlErrorDetails.Text = String.Format(sFailureMsg, Session("DespatchProcessSuccess_pyrf"))
            End If
        End If

        If Session("PrintSuccessMessage") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("PrintSuccessMessage")) Then
            ltlSuccessDetails.Text = Session("PrintSuccessMessage").ToString()
        End If
        Session("PrintSuccessMessage") = Nothing

        Session("DespatchProcessSuccess_pyrf") = Nothing
        Session("DespatchProcessSuccess_token") = Nothing
        Session("DespatchProcessSuccess_type") = Nothing

        plhErrorMessage.Visible = False
        If ltlErrorDetails.Text.Length > 0 Then
            plhErrorMessage.Visible = True
        End If
        plhSuccessMessage.Visible = False
        If ltlSuccessDetails.Text.Length > 0 Then
            plhSuccessMessage.Visible = True
        End If

        If Usage = UsageType.PRINT Then
            plhProcessButtons.Visible = False
            plhPrintButton.Visible = True
            plhDespatchOptionsButtons.Visible = False
        End If
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_PreRender", _talentLogging_LogPrefix & "End of Page Pre-Render, success message is = " & ltlSuccessDetails.Text, "DespatchLog")
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_PreRender", _talentLogging_LogPrefix & "End of Page Pre-Render, error message is = " & ltlErrorDetails.Text, "DespatchLog")
    End Sub

    Protected Sub rptItemstoDespatch_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptItemstoDespatch.ItemDataBound
        If e.Item.ItemType = ListItemType.Header Then
            Dim plhShowPrintHeader As PlaceHolder = CType(e.Item.FindControl("plhShowPrintHeader"), PlaceHolder)
            Dim plhShowTicketNumberHeader As PlaceHolder = CType(e.Item.FindControl("plhShowTicketNumberHeader"), PlaceHolder)
            Dim plhPrefixHeader As PlaceHolder = CType(e.Item.FindControl("plhPrefixHeader"), PlaceHolder)

            If Usage <> UsageType.PRINT Then
                plhShowPrintHeader.Visible = False
            Else
                plhShowTicketNumberHeader.Visible = False
                plhPrefixHeader.Visible = False
            End If
        End If
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim hdfProductCode As HiddenField = CType(e.Item.FindControl("hdfProductCode"), HiddenField)
            Dim plhGiftMessage As PlaceHolder = CType(e.Item.FindControl("plhGiftMessage"), PlaceHolder)
            Dim lblLabel As Label = CType(e.Item.FindControl("lblSeat"), Label)
            Dim hdfStand As HiddenField = CType(e.Item.FindControl("hdfStand"), HiddenField)
            Dim hdfStandDesc As HiddenField = CType(e.Item.FindControl("hdfStandDesc"), HiddenField)
            Dim hdfArea As HiddenField = CType(e.Item.FindControl("hdfArea"), HiddenField)
            Dim hdfAreaDesc As HiddenField = CType(e.Item.FindControl("hdfAreaDesc"), HiddenField)
            Dim hdfRowNumber As HiddenField = CType(e.Item.FindControl("hdfRowNumber"), HiddenField)
            Dim hdfSeat As HiddenField = CType(e.Item.FindControl("hdfSeat"), HiddenField)
            Dim hdfAlphaSuffix As HiddenField = CType(e.Item.FindControl("hdfAlphaSuffix"), HiddenField)
            Dim txtTicketNumber As TextBox = CType(e.Item.FindControl("txtTicketNumber"), TextBox)
            Dim plhScanSeries As PlaceHolder = CType(e.Item.FindControl("plhScanSeries"), PlaceHolder)
            Dim hplScanSeries As HyperLink = CType(e.Item.FindControl("hplScanSeries"), HyperLink)
            Dim plhPrint As PlaceHolder = CType(e.Item.FindControl("plhPrint"), PlaceHolder)
            Dim hplPrintLabelRequest As HyperLink = CType(e.Item.FindControl("hplPrintLabelRequest"), HyperLink)
            Dim hplPrintLabelConfirm As HyperLink = CType(e.Item.FindControl("hplPrintLabelConfirm"), HyperLink)
            Dim hplCompleteLabelIcon As HyperLink = CType(e.Item.FindControl("hplCompleteLabelIcon"), HyperLink)
            Dim hplPrintLabelRenew As HyperLink = CType(e.Item.FindControl("hplPrintLabelRenew"), HyperLink)
            Dim plhShowPrint As PlaceHolder = CType(e.Item.FindControl("plhShowPrint"), PlaceHolder)
            Dim plhPrefix As PlaceHolder = CType(e.Item.FindControl("plhPrefix"), PlaceHolder)
            Dim plhShowTicketNumber As PlaceHolder = CType(e.Item.FindControl("plhShowTicketNumber"), PlaceHolder)
            Dim hplRestrictedPrintLabelIcon As HyperLink = CType(e.Item.FindControl("hplRestrictedPrintLabelIcon"), HyperLink)
            Dim csvTicketNumber As CustomValidator = CType(e.Item.FindControl("csvTicketNumber"), CustomValidator)
            Dim hdfMembershipRFID As HiddenField = CType(e.Item.FindControl("hdfMembershipRFID"), HiddenField)
            Dim hdfMembershipMagScan As HiddenField = CType(e.Item.FindControl("hdfMembershipMagScan"), HiddenField)
            Dim hdfMembershipMetalBadge As HiddenField = CType(e.Item.FindControl("hdfMembershipMetalBadge"), HiddenField)
            Dim ltlPrintCardError As Literal = CType(e.Item.FindControl("ltlPrintCardError"), Literal)
            Dim ltlLoadingImg As Literal = CType(e.Item.FindControl("ltlLoadingImg"), Literal)
            Dim hdfSeatValidationCode As HiddenField = CType(e.Item.FindControl("hdfSeatValidationCode"), HiddenField)
            Dim attributeValue As New StringBuilder

            'ltlLoadingImg
            plhGiftMessage.Visible = (e.Item.DataItem("GiftMessage").ToString().Trim().Length > 0)
            hdfProductCode.Value = e.Item.DataItem("Product").ToString()
            hdfStand.Value = e.Item.DataItem("Stand").ToString()
            hdfStandDesc.Value = e.Item.DataItem("StandDesc").ToString()
            hdfArea.Value = e.Item.DataItem("Area").ToString()
            hdfAreaDesc.Value = e.Item.DataItem("AreaDesc").ToString()
            hdfRowNumber.Value = e.Item.DataItem("RowNumber").ToString()
            hdfSeat.Value = e.Item.DataItem("Seat").ToString()
            hdfAlphaSuffix.Value = e.Item.DataItem("AlphaSuffix").ToString()

            If Usage <> UsageType.PRINT Then
                plhShowPrint.Visible = False
            Else
                plhShowTicketNumber.Visible = False
                plhPrefix.Visible = False
            End If
            If ConvertFromISeriesYesNoToBoolean(e.Item.DataItem("NoPrintFlag").ToString()) Then
                plhShowPrint.Visible = False
            End If
            Dim PrintForDespatch As Boolean = ConvertFromISeriesYesNoToBoolean(e.Item.DataItem("PrintForDespatch").ToString())

            If Trim((e.Item.DataItem("Seat").ToString)) <> String.Empty Then
                If e.Item.DataItem("ProductType").ToString() = GlobalConstants.HOMEPRODUCTTYPE OrElse e.Item.DataItem("ProductType").ToString() = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                    _deSeatDetails.Stand = hdfStand.Value
                    _deSeatDetails.StandDescription = hdfStandDesc.Value
                    _deSeatDetails.Area = hdfArea.Value
                    _deSeatDetails.AreaDescription = hdfAreaDesc.Value
                    _deSeatDetails.Row = hdfRowNumber.Value
                    _deSeatDetails.Seat = hdfSeat.Value
                    _deSeatDetails.AlphaSuffix = hdfAlphaSuffix.Value
                    _deSeatDetails.SeatStringMask = TEBUtilities.CheckForDBNull_String(_ucr.Attribute("SeatStringMask"))

                    If _deSeatDetails.SeatStringMask <> "" Then
                        lblLabel.Text = _deSeatDetails.StringMaskFormattedSeat
                    Else
                        lblLabel.Text = _deSeatDetails.FormattedSeat
                    End If

                ElseIf e.Item.DataItem("ProductType").ToString() = GlobalConstants.TRAVELPRODUCTTYPE Then
                    lblLabel.Text = hdfStand.Value & hdfArea.Value
                Else
                    lblLabel.Text = _ucr.Content("UnreservedSeat", _languageCode, True)
                End If
            Else
                lblLabel.Text = _ucr.Content("UnreservedSeat", _languageCode, True)
            End If

            attributeValue.Append("return tabOnEnter(this, event, '").Append(csvTicketNumber.ClientID).Append("', '").Append(txtTicketNumber.ClientID).Append("')")
            txtTicketNumber.Attributes.Add("onkeydown", attributeValue.ToString())
            attributeValue = New StringBuilder
            attributeValue.Append("setCurrentTicketNumber('").Append(txtTicketNumber.ClientID).Append("', '").Append(csvTicketNumber.ClientID).Append("')")
            txtTicketNumber.Attributes.Add("onblur", attributeValue.ToString())
            Dim showMembershipAttributeValue As New StringBuilder
            If TCUtilities.convertToBool(TEBUtilities.CheckForDBNull_String(e.Item.DataItem("IsMembership"))) Then
                showMembershipAttributeValue.Append("showMembershipOptions('").Append(hdfMembershipRFID.ClientID).Append("', '").Append(hdfMembershipMagScan.ClientID).Append("', '").Append(hdfMembershipMetalBadge.ClientID).Append("', '")
                showMembershipAttributeValue.Append(txtTicketNumber.ClientID).Append("', '").Append(_membershipLabel).Append("', '").Append(PrintForDespatch).Append("')")
                txtTicketNumber.Attributes.Add("onfocus", showMembershipAttributeValue.ToString())
                hdfMembershipRFID.Value = e.Item.DataItem("RFID").ToString()
                hdfMembershipMagScan.Value = e.Item.DataItem("MagScan").ToString()
                hdfMembershipMetalBadge.Value = e.Item.DataItem("MetalBadge").ToString()
                txtTicketNumber.Text = _membershipLabel
            Else
                txtTicketNumber.Attributes.Add("onfocus", "hideMembershipOptions();")
                txtTicketNumber.Text = e.Item.DataItem("Barcode").ToString()
                Dim seatStatus As String = e.Item.DataItem("SeatStatus").ToString().Trim()
                If txtTicketNumber.Text.Trim().Length = 0 AndAlso seatStatus.Length > 0 Then
                    If seatStatus = SENT_STATUS_CODE Then
                        txtTicketNumber.Text = _sentLabel
                    ElseIf seatStatus = NOT_FOR_DESPATCH_STATUS_CODE Then
                        txtTicketNumber.Text = _notForDespatchLabel
                    End If
                End If
            End If
            _intTabIndex += 1
            txtTicketNumber.TabIndex = _intTabIndex
            csvTicketNumber.ErrorMessage = _ucr.Content("TicketPrefixErrorText", _languageCode, True)
            If Usage = UsageType.PRINT Then
                txtTicketNumber.Visible = False
            End If
            plhScanSeries.Visible = (TEBUtilities.CheckForDBNull_Decimal(e.Item.DataItem("TotalInSequence")) > 1)
            If plhScanSeries.Visible AndAlso txtTicketNumber.Text <> _membershipLabel AndAlso Usage <> UsageType.PRINT Then
                attributeValue = New StringBuilder
                attributeValue.Append("scanSeriesOpen('").Append(GetBarcodePrefix(e.Item.DataItem("BarcodePrefix").ToString().Trim())).Append("', '").Append(txtTicketNumber.ClientID)
                attributeValue.Append("', ").Append(e.Item.ItemIndex).Append(", ").Append(TEBUtilities.CheckForDBNull_Decimal(e.Item.DataItem("SequenceNumber"))).Append(", ")
                attributeValue.Append(TEBUtilities.CheckForDBNull_Decimal(e.Item.DataItem("TotalInSequence"))).Append(");")
                hplScanSeries.Attributes.Add("onclick", attributeValue.ToString())
                hplScanSeries.Attributes.Add("data-open", "scan-series")
                hplScanSeries.Attributes.Add("onmouseover", "hideMembershipOptions();")
            Else
                plhScanSeries.Visible = False
            End If

            If PrintForDespatch Then
                plhScanSeries.Visible = False
                plhPrint.Visible = True
                hplPrintLabelRequest.Style.Add("display", "inline")
                _printForDespatch = True
                If AgentProfile.AgentPermissions.CanPrintDespatchCards Then
                    Dim IsMembershipRenewal As Boolean = ConvertFromISeriesYesNoToBoolean(e.Item.DataItem("IsMembershipRenewal").ToString())
                    hplRestrictedPrintLabelIcon.Visible = False
                    If IsMembershipRenewal Then
                        hplPrintLabelRenew.Style.Add("display", "inline")
                        hplPrintLabelRenew.Attributes.Add("onclick", "javascript:CallTeamCardPrintWebAPI" + CStr(_intTabIndex) + "('X', this, undefined ,""#" + hplCompleteLabelIcon.ClientID + """,'" + CStr(_intTabIndex) + "', '" + hdfMembershipRFID.ClientID + "');")
                        hplPrintLabelRequest.Attributes.Add("onclick", "javascript:CallTeamCardPrintWebAPI" + CStr(_intTabIndex) + "('Y', this, ""#" + hplPrintLabelConfirm.ClientID + """,""#" + hplCompleteLabelIcon.ClientID + """,'" + CStr(_intTabIndex) + "', '');")
                    Else
                        hplPrintLabelRenew.Visible = False
                        hplPrintLabelRequest.Attributes.Add("onclick", "javascript:CallTeamCardPrintWebAPI" + CStr(_intTabIndex) + "('Y', this, ""#" + hplPrintLabelConfirm.ClientID + """,""#" + hplCompleteLabelIcon.ClientID + """,'" + CStr(_intTabIndex) + "', '');")
                    End If

                    hplPrintLabelConfirm.Attributes.Add("onclick", "javascript:CallTeamCardPrintWebAPI" + CStr(_intTabIndex) + "('C', ""#" + hplPrintLabelRequest.ClientID + """, ""#" + hplPrintLabelConfirm.ClientID + """,""#" + hplCompleteLabelIcon.ClientID + """,'" + CStr(_intTabIndex) + "', '" + hdfMembershipRFID.ClientID + "');")
                    ltlLoadingImg.Text = "<div id=""loading-image" + CStr(_intTabIndex) + """ style=""display:none""><img src=""" + TEBUtilities.CheckForDBNull_String(_ucr.Attribute("LoadingImagePath")) + """ style=""width:30;height:30;""></div>"
                    ltlPrintCardError.Text = "<div id=""print-card-error-text" + CStr(_intTabIndex) + """ style=""display:none"">" + _ucr.Content("PrintCardErrorText", _languageCode, True) + "</div>" &
                                            "<div id=""card-number-retrieval-error-text" + CStr(_intTabIndex) + """ style=""display:none"">" + _ucr.Content("CardNumberRetrievalErrorText", _languageCode, True) + "</div>"
                    ScriptRegistration(hdfProductCode.Value, hdfStand.Value, hdfArea.Value, hdfRowNumber.Value, hdfSeat.Value,
                                       hdfAlphaSuffix.Value, PaymentReference, CStr(_intTabIndex))
                    hplCompleteLabelIcon.Attributes.Add("onmouseover", "hideMembershipOptions();")
                    If TCUtilities.convertToBool(TEBUtilities.CheckForDBNull_String(e.Item.DataItem("IsMembership"))) Then
                        hplPrintLabelRequest.Attributes.Add("onmouseover", showMembershipAttributeValue.ToString())
                        hplPrintLabelConfirm.Attributes.Add("onmouseover", showMembershipAttributeValue.ToString())
                    Else
                        hplPrintLabelRequest.Attributes.Add("onmouseover", "hideMembershipOptions();")
                        hplPrintLabelConfirm.Attributes.Add("onmouseover", "hideMembershipOptions();")
                    End If
                Else
                    hplCompleteLabelIcon.Visible = False
                    hplPrintLabelConfirm.Visible = False
                    hplPrintLabelRequest.Visible = False
                    Dim IsMembershipRenewal As Boolean = ConvertFromISeriesYesNoToBoolean(e.Item.DataItem("IsMembershipRenewal").ToString())
                    If IsMembershipRenewal Then
                        hplPrintLabelRenew.Attributes.Add("onclick", "javascript:CallTeamCardPrintWebAPI" + CStr(_intTabIndex) + "('X', this, undefined ,""#" + hplCompleteLabelIcon.ClientID + """,'" + CStr(_intTabIndex) + "', '');")
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub ScriptRegistration(ByVal productCode As String, ByVal stand As String, ByVal area As String, ByVal row As String, _
                                   ByVal seat As String, ByVal alpha As String, ByVal paymentReference As String, ByVal index As String)

        Dim root As String = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)
        Dim URL As String = "DespatchProcess.aspx/CallSmartCardRequest"
        _sbJavaScript.Append(" " + "function CallTeamCardPrintWebAPI" + index + "(mode, requestHyperlink, confirmHyperlink, completeHyperlink, index, hdfMembershipRFID) { ")
        '_sbJavaScript.Append("   " + "confirmHyperlink = document.getElementById(confirmHyperlink);")
        _sbJavaScript.Append("   " + "var inputModel;")
        _sbJavaScript.Append("       " + "inputModel = {")
        _sbJavaScript.Append("         " + "SessionID:""" + CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID + """,")
        _sbJavaScript.Append("         " + "Mode: mode,")
        _sbJavaScript.Append("         " + "ProductCode:""" + productCode + """,")
        _sbJavaScript.Append("         " + "Stand:""" + stand + """,")
        _sbJavaScript.Append("         " + "Area:""" + area + """,")
        _sbJavaScript.Append("         " + "Row:""" + row + """,")
        _sbJavaScript.Append("         " + "Seat:""" + seat + """,")
        _sbJavaScript.Append("         " + "Alpha:""" + alpha + """,")
        _sbJavaScript.Append("         " + "PaymentReference:""" + paymentReference + """,")
        _sbJavaScript.Append("          " + "ExistingCardNumber: existingCardNumber")
        _sbJavaScript.Append("   " + "};")
        _sbJavaScript.Append("   " + "CallTeamCardPrintWebAPI(inputModel,""" + URL + """, requestHyperlink, confirmHyperlink, completeHyperlink, index, mode, hdfMembershipRFID);")
        _sbJavaScript.Append(" " + "}")
    End Sub
    Protected Sub rptItemstoDespatch_PreRender(sender As Object, e As EventArgs) Handles rptItemstoDespatch.PreRender
        If rptItemstoDespatch.Items.Count > 0 Then
            Dim hasTicketNumber As Boolean = False
            Dim hasEmptyTicketNumber As Boolean = False
            Dim despatchStatus As String = String.Empty
            Dim addFinishButtonConfirmFunction As Boolean = False
            Dim rptItem As RepeaterItem = rptItemstoDespatch.Items(rptItemstoDespatch.Items.Count - 1)
            ScriptManager.RegisterClientScriptBlock(rptItemstoDespatch, Me.GetType(), "TeamCard-Print", _sbJavaScript.ToString(), True)
            If rptItem IsNot Nothing Then
                Dim txtTicketNumber As TextBox = CType(rptItem.FindControl("txtTicketNumber"), TextBox)
                txtTicketNumber.Attributes.Remove("onkeydown")
            End If
            Dim printStatus As String = String.Empty
            Dim allPrinted As Boolean = True
            Dim allProcessed As Boolean = True
            For Each item As RepeaterItem In rptItemstoDespatch.Items
                Dim hdfSeatStatus As HiddenField = CType(item.FindControl("hdfSeatStatus"), HiddenField)
                Dim txtTicketNumber As TextBox = CType(item.FindControl("txtTicketNumber"), TextBox)
                Dim hdfMembershipRFID As HiddenField = CType(item.FindControl("hdfMembershipRFID"), HiddenField)
                Dim hdfMembershipMagScan As HiddenField = CType(item.FindControl("hdfMembershipMagScan"), HiddenField)
                Dim hdfMembershipMetalBadge As HiddenField = CType(item.FindControl("hdfMembershipMetalBadge"), HiddenField)

                If allPrinted AndAlso hdfSeatStatus.Value = _ucr.Content("ItemNotPrintedText", _languageCode, True) Then
                    allPrinted = False
                End If

                If allProcessed Then
                    If Not hdfSeatStatus.Value = _ucr.Content("DespatchedLabel", _languageCode, True) AndAlso Not hdfSeatStatus.Value = _ucr.Content("SentText", _languageCode, True) Then
                        allProcessed = False
                    End If
                End If

                If txtTicketNumber.Text.Trim().Length > 0 Then
                    If txtTicketNumber.Text <> _membershipLabel OrElse hdfMembershipRFID.Value.Trim().Length > 0 OrElse hdfMembershipMagScan.Value.Trim().Length > 0 OrElse hdfMembershipMetalBadge.Value.Trim().Length > 0 Then
                        hasTicketNumber = True
                    Else
                        hasEmptyTicketNumber = True
                    End If
                Else
                    If hdfSeatStatus.Value = SENT_STATUS_CODE Then
                        hasTicketNumber = True
                    ElseIf hdfSeatStatus.Value = _ucr.Content("NotDespatchedLabel", _languageCode, True) Then
                        hasEmptyTicketNumber = True
                    ElseIf hdfSeatStatus.Value = _ucr.Content("DespatchedLabel", _languageCode, True) Then
                        If hdfMembershipRFID.Value.Trim().Length > 0 OrElse hdfMembershipMagScan.Value.Trim().Length > 0 OrElse hdfMembershipMetalBadge.Value.Trim().Length > 0 Then
                            hasTicketNumber = True
                        Else
                            hasEmptyTicketNumber = True
                        End If
                    ElseIf hdfSeatStatus.Value.Trim() = String.Empty Then
                        hasEmptyTicketNumber = True
                    End If
                End If
            Next

            If hasTicketNumber AndAlso allProcessed Then despatchStatus = _ucr.Content("FullyDespatchedLabel", _languageCode, True)
            If hasEmptyTicketNumber Then
                addFinishButtonConfirmFunction = True
                despatchStatus = _ucr.Content("NotDespatchedLabel", _languageCode, True)
            End If
            If hasTicketNumber AndAlso Not allProcessed Then
                addFinishButtonConfirmFunction = True
                despatchStatus = _ucr.Content("PartDespatchedLabel", _languageCode, True)
            End If

            If addFinishButtonConfirmFunction Then
                btnFinishOrder.OnClientClick = "logConfirmButtonClick(); return confirmOrder('" & _ucr.Content("PartDespatchedWarningMessage", _languageCode, True) & "');"
            Else
                btnFinishOrder.OnClientClick = "logConfirmButtonClick();"
            End If

            If Usage = UsageType.PRINT Then
                If allPrinted Then
                    despatchStatus = _ucr.Content("ItemPrintedText", _languageCode, True)
                Else
                    despatchStatus = _ucr.Content("ItemNotPrintedText", _languageCode, True)
                End If
            End If

            ltlDespatchStatus.Text = despatchStatus

            If _ucr.Content("SkipOrderWarningMessage", _languageCode, True).ToString.Length > 0 Then
                btnSkipOrder.OnClientClick = "return confirm('" & _ucr.Content("SkipOrderWarningMessage", _languageCode, True) & "');"
            End If

            For Each item As RepeaterItem In rptItemstoDespatch.Items
                Dim txtTicketNumber As TextBox = CType(item.FindControl("txtTicketNumber"), TextBox)
                Dim hdfMembershipRFID As HiddenField = CType(item.FindControl("hdfMembershipRFID"), HiddenField)
                If txtTicketNumber.Text.Trim().Length = 0 Then
                    txtTicketNumber.Focus()
                    Exit For
                Else
                    If txtTicketNumber.Text = _membershipLabel AndAlso hdfMembershipRFID.Value.Trim().Length = 0 Then
                        txtTicketNumber.Focus()
                        Exit For
                    End If
                End If
            Next
        End If
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        ltlErrorDetails.Text = String.Empty
        ltlSuccessDetails.Text = String.Empty
        Page.Validate()
        If Page.IsValid() Then
            setSessionVariables()
            performSearch()
        End If
    End Sub

    Protected Sub csvSearchOptions_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = False
        If (txtDespatchNoteID.Text.Trim().Length > 0) Xor (txtPaymentReference.Text.Trim().Length > 0) Xor (txtTicketNumber.Text.Trim().Length > 0) Then
            'Exclusive or used so that only one value is entered
            args.IsValid = True
        End If
    End Sub

    Protected Sub csvDuplicateTickets_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = True
        If rptItemstoDespatch.Items.Count > 0 Then
            Dim ticketNumberList As New List(Of String)
            Dim membershipRFIDList As New List(Of String)
            Dim membershipMagScanList As New List(Of String)
            Dim membershipMetalBadgeList As New List(Of String)
            For Each item As RepeaterItem In rptItemstoDespatch.Items
                Dim txtTicketNumber As TextBox = CType(item.FindControl("txtTicketNumber"), TextBox)
                Dim hdfMembershipRFID As HiddenField = CType(item.FindControl("hdfMembershipRFID"), HiddenField)
                Dim hdfMembershipMagScan As HiddenField = CType(item.FindControl("hdfMembershipMagScan"), HiddenField)
                Dim hdfMembershipMetalBadge As HiddenField = CType(item.FindControl("hdfMembershipMetalBadge"), HiddenField)
                If txtTicketNumber.Text <> _sentLabel AndAlso txtTicketNumber.Text <> _notForDespatchLabel AndAlso txtTicketNumber.Text <> _membershipLabel Then
                    If txtTicketNumber.Text.Length > 0 Then ticketNumberList.Add(txtTicketNumber.Text)
                End If
                If hdfMembershipRFID.Value.Length > 0 Then membershipRFIDList.Add(hdfMembershipRFID.Value)
                If hdfMembershipMagScan.Value.Length > 0 Then membershipMagScanList.Add(hdfMembershipMagScan.Value)
                If hdfMembershipMetalBadge.Value.Length > 0 Then membershipMetalBadgeList.Add(hdfMembershipMetalBadge.Value)
            Next
            Dim duplicates = ticketNumberList.Where(Function(x) ticketNumberList.Where(Function(y) x = y).Count() > 1).Distinct()
            If duplicates.Count > 0 Then
                args.IsValid = False
            Else
                duplicates = membershipRFIDList.Where(Function(x) membershipRFIDList.Where(Function(y) x = y).Count() > 1).Distinct()
                If duplicates.Count > 0 Then
                    args.IsValid = False
                Else
                    duplicates = membershipMagScanList.Where(Function(x) membershipMagScanList.Where(Function(y) x = y).Count() > 1).Distinct()
                    If duplicates.Count > 0 Then
                        args.IsValid = False
                    Else
                        duplicates = membershipMetalBadgeList.Where(Function(x) membershipMetalBadgeList.Where(Function(y) x = y).Count() > 1).Distinct()
                        If duplicates.Count > 0 Then
                            args.IsValid = False
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub btnFinishOrder_Click(sender As Object, e As EventArgs) Handles btnFinishOrder.Click
        FinishOrder()
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "btnClear_Click", _talentLogging_LogPrefix & "About to performSearch()", "DespatchLog")
        ltlErrorDetails.Text = String.Empty
        ltlSuccessDetails.Text = String.Empty
        performSearch()
    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs) Handles btnBack.Click
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "btnBack_Click", _talentLogging_LogPrefix & "Start", "DespatchLog")
        If Session("PreviousPage") IsNot Nothing Then
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "btnBack_Click", _talentLogging_LogPrefix & "About to redirect to Session(PreviousPage)", "DespatchLog")
            Response.Redirect(Session("PreviousPage"))
        End If
    End Sub

    Protected Sub btnSkipOrder_Click(sender As Object, e As EventArgs) Handles btnSkipOrder.Click
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "btnSkipOrder_Click", _talentLogging_LogPrefix & "About to ~/PagesAgent/Despatch/DespatchProcess.aspx", "DespatchLog")
        Response.Redirect("~/PagesAgent/Despatch/DespatchProcess.aspx")
    End Sub

    Protected Sub btnDespatchNoteGeneration_Click(sender As Object, e As EventArgs) Handles btnDespatchNoteGeneration.Click
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "btnDespatchNoteGeneration_Click", _talentLogging_LogPrefix & "About to ~/PagesAgent/Despatch/DespatchNoteGeneration.aspx", "DespatchLog")
        Response.Redirect("~/PagesAgent/Despatch/DespatchNoteGeneration.aspx")
    End Sub

    Protected Sub btnCourier_Click(sender As Object, e As EventArgs) Handles btnCourier.Click
        Dim csvSavePath As String = _ucr.Attribute("CourierCSVSavePath")
        Dim csvDownloadPath As String = _ucr.Attribute("CourierCSVDownloadPath")
        ltlErrorDetails.Text = String.Empty
        ltlSuccessDetails.Text = String.Empty
        _PaymentReference = hplPaymentReference.Text
        If csvSavePath.Length > 0 AndAlso Directory.Exists(csvSavePath) AndAlso Directory.Exists(Server.MapPath(csvDownloadPath)) Then
            Dim fileName As New StringBuilder
            fileName.Append("P")
            fileName.Append(rdoCourierType.SelectedValue).Append(_PaymentReference).Append(".csv")
            If Not csvSavePath.EndsWith("\") Then csvSavePath = csvSavePath & "\"
            If Not csvDownloadPath.EndsWith("/") Then csvDownloadPath = csvDownloadPath & "/"

            If createCSVFile(csvSavePath, fileName.ToString()) Then
                hplCourierCSVFile.NavigateUrl = csvDownloadPath & fileName.ToString()
                hplCourierCSVFile.Text = _ucr.Content("CourierCSVLinkText", _languageCode, True)
                hplCourierCSVFile.Visible = True
            Else
                ltlErrorDetails.Text = _ucr.Content("CSVFileCreationError", _languageCode, True)
            End If
        Else
            ltlErrorDetails.Text = _ucr.Content("CSVPathError", _languageCode, True).Replace("<<SAVE_PATH>>", csvSavePath).Replace("<<DOWNLOAD_PATH>>", csvDownloadPath)
        End If
    End Sub

    Protected Function GetSeatValidationCode(ByVal BarcodePrefix As String) As String
        Return If(BarcodePrefix.Length > 0 AndAlso Not String.IsNullOrEmpty(BarcodePrefix.Substring(0, 1)), BarcodePrefix.Substring(0, 1), "0")
    End Function
    Protected Function GetBarcodePrefix(ByVal BarcodePrefix As String) As String
        Return If(BarcodePrefix.Length > 0 AndAlso Not String.IsNullOrEmpty(BarcodePrefix.Substring(0, 1)), BarcodePrefix.Substring(1, BarcodePrefix.Length - 1), String.Empty)
    End Function
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set instances of the class level objects
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitialiseClassLevelFields()
        _ucr = New UserControlResource
        _deSeatDetails = New DESeatDetails
        _agentSettingsObject = TEBUtilities.GetSettingsObjectForAgent()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "DespatchProcess.ascx"
        End With
        _errMessage = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
        _sentLabel = _ucr.Content("SentText", _languageCode, True)
        _notForDespatchLabel = _ucr.Content("NotForDespatchText", _languageCode, True)
        _membershipLabel = _ucr.Content("MembershipTicketNumberLabel", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Set text values for all text related objects
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetFieldsDisplayText()
        'Repeater column headings
        EventHeaderText = _ucr.Content("ltlEvent", _languageCode, True)
        TicketTypeHeaderText = _ucr.Content("ltlTicketType", _languageCode, True)
        PrefixHeaderText = _ucr.Content("ltlPrefix", _languageCode, True)
        SeatHeaderText = _ucr.Content("ltlSeat", _languageCode, True)
        TicketNoHeaderText = _ucr.Content("ltlTicketNo", _languageCode, True)
        StatusHeaderText = _ucr.Content("ltlStatus", _languageCode, True)
        PrintAllHeaderText = _ucr.Content("ltlPrintAll", _languageCode, True)

        lblDespatchNoteID.Text = _ucr.Content("lblDespatchNoteID", _languageCode, True)
        rgxDespatchNoteID.ErrorMessage = _ucr.Content("InvalidDespatchNoteID", _languageCode, True)
        lblPaymentReference.Text = _ucr.Content("lblPaymentReference", _languageCode, True)
        rgxPaymentReference.ErrorMessage = _ucr.Content("InvalidPaymentReference", _languageCode, True)
        lblTicketNumber.Text = _ucr.Content("lblTicketNumber", _languageCode, True)
        csvSearchOptions.ErrorMessage = _ucr.Content("EnterAtLeastOneSearchOption", _languageCode, True)
        csvDuplicateTickets.ErrorMessage = _ucr.Content("DuplicateTicketScannedError", _languageCode, True)
        lblFirst.Text = _ucr.Content("lblFirst", _languageCode, True)
        lblLast.Text = _ucr.Content("lblLast", _languageCode, True)
        csvFirstTicketNumber.ErrorMessage = _ucr.Content("TicketPrefixErrorText", _languageCode, True)
        csvLastTicketNumber.ErrorMessage = _ucr.Content("TicketPrefixErrorText", _languageCode, True)
        lblMembershipRFID.Text = _ucr.Content("lblMembershipRFID", _languageCode, True)
        lblNotes.Text = _ucr.Content("lblNotes", _languageCode, True)
        lblMembershipMagScan.Text = _ucr.Content("lblMembershipMagScan", _languageCode, True)
        lblMetalBadgeNumber.Text = _ucr.Content("lblMetalBadgeNumber", _languageCode, True)
        btnDespatchNoteGeneration.Text = _ucr.Content("btnDespatchNoteGeneration", _languageCode, True)
        btnCourier.Text = _ucr.Content("btnCourier", _languageCode, True)
        btnClear.Text = _ucr.Content("btnClear", _languageCode, True)
        btnSkipOrder.Text = _ucr.Content("btnSkipOrder", _languageCode, True)
        btnBack.Text = _ucr.Content("btnBack", _languageCode, True)
        If Usage = UsageType.PRINT Then
            btnFinishOrder.Text = _ucr.Content("btnPrint", _languageCode, True)
        Else
            btnFinishOrder.Text = _ucr.Content("btnFinishOrder", _languageCode, True)
        End If

        btnSearch.Text = _ucr.Content("btnSearch", _languageCode, True)
        btnSent.Value = _ucr.Content("btnSent", _languageCode, True)
        btnSkipBack.Value = _ucr.Content("btnSkipBack", _languageCode, True)
        btnSkipNext.Value = _ucr.Content("btnSkipItem", _languageCode, True)
        btnNotForDespatch.Value = _ucr.Content("btnNotForDespatch", _languageCode, True)
        GiftMessageIcon = _ucr.Content("GiftMessageIcon", _languageCode, True)
        ScanSeriesIcon = _ucr.Content("ScanSeriesIcon", _languageCode, True)
        TicketLimitError = _ucr.Content("TicketLimitError", _languageCode, True)

        Dim jScript As New StringBuilder
        jScript.Append("var _membershipNumberLabel = '").Append(_membershipLabel).Append("'")
        ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "despatch-process", jScript.ToString(), True)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "despatch-process.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("despatch-process.js", "/Module/Despatch/"), False)
    End Sub

    ''' <summary>
    ''' Load the initial display for the first time the page loads
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadInitialDespatchProcess()

        If Session("DespatchNoteID") IsNot Nothing Or Session("PaymentReference") IsNot Nothing Or Session("TicketNumber") IsNot Nothing Then
            txtDespatchNoteID.Text = Session("DespatchNoteID")
            txtPaymentReference.Text = Session("PaymentReference")
            txtTicketNumber.Text = Session("TicketNumber")
            performSearch()
        Else
            txtDespatchNoteID.Text = String.Empty
            txtPaymentReference.Text = String.Empty
            txtTicketNumber.Text = String.Empty
            plhDespatchProcessInfo.Visible = False
            plhDespatchOptions.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Load the display based on given order items to show
    ''' </summary>
    ''' <param name="batchId">The batch ID number to work with</param>
    ''' <remarks></remarks> 
    Private Sub SetPageWithOrderDetails(ByVal batchId As Long)
        plhDespatchProcessInfo.Visible = True
        plhDespatchOptions.Visible = True
        plhCourierOptions.Visible = ModuleDefaults.CourierTrackingReferences
        hdfBatchID.Value = batchId

        Const TERMINATE_LINE As String = "');"
        Dim attributeValue As New StringBuilder
        attributeValue.Append("focusMagScan(this, '").Append(txtMembershipMagScan.ClientID).Append(TERMINATE_LINE)
        txtMembershipRFID.Attributes.Add("onblur", attributeValue.ToString())
        rgxMembershipRFID.ErrorMessage = _ucr.Content("InvalidMembershipRFID", _languageCode, True)
        rgxMembershipRFID.ValidationExpression = _ucr.Attribute("MembershipRFIDValidationExpression")

        attributeValue = New StringBuilder
        attributeValue.Append("focusMetalBadge(this, '").Append(txtMetalBadgeNumber.ClientID).Append(TERMINATE_LINE)
        txtMembershipMagScan.Attributes.Add("onblur", attributeValue.ToString())
        rgxMembershipMagScan.ErrorMessage = _ucr.Content("InvalidMembershipMagScan", _languageCode, True)
        rgxMembershipMagScan.ValidationExpression = _ucr.Attribute("MembershipMagScanValidationExpression")

        attributeValue = New StringBuilder
        attributeValue.Append("focusNextTicketNumber(this, '").Append(txtMetalBadgeNumber.ClientID).Append(TERMINATE_LINE)
        txtMetalBadgeNumber.Attributes.Add("onblur", attributeValue.ToString())
        rgxMetalBadgeNumber.ErrorMessage = _ucr.Content("InvalidMetalBadgeNumber", _languageCode, True)
        rgxMetalBadgeNumber.ValidationExpression = _ucr.Attribute("MetalBadgeNumberValidationExpression")

        attributeValue = New StringBuilder
        attributeValue.Append("markAsNotForDespatch('").Append(_notForDespatchLabel).Append(TERMINATE_LINE)
        btnNotForDespatch.Attributes.Add("onclick", attributeValue.ToString())

        attributeValue = New StringBuilder
        attributeValue.Append("markAsNotForDespatch('").Append(_notForDespatchLabel).Append(TERMINATE_LINE)
        btnNotForDespatch.Attributes.Add("onclick", attributeValue.ToString())

        attributeValue = New StringBuilder
        attributeValue.Append("markAsSent('").Append(_sentLabel).Append(TERMINATE_LINE)
        btnSent.Attributes.Add("onclick", attributeValue.ToString())

        attributeValue = New StringBuilder
        attributeValue.Append("return tabOnEnter(this, event, '").Append(csvFirstTicketNumber.ClientID).Append("', '").Append(txtFirst.ClientID).Append("')")
        txtFirst.Attributes.Add("onkeydown", attributeValue.ToString())

        attributeValue = New StringBuilder
        attributeValue.Append("return scanSeriestabOnEnter(this, event, '").Append(csvLastTicketNumber.ClientID).Append("', '").Append(txtLast.ClientID).Append("')")
        txtLast.Attributes.Add("onkeydown", attributeValue.ToString())
    End Sub

    ''' <summary>
    ''' Common function to perform the despatch process search function
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub performSearch()
        Dim newBatchID As Long = 0
        Dim paymentReference As Long = 0
        Dim sType As String = PAYMENT_TYPE
        txtDespatchNoteID.Text = txtDespatchNoteID.Text.ToUpper().Trim()
        txtPaymentReference.Text = txtPaymentReference.Text.Trim()

        If txtDespatchNoteID.Text.Trim().Length > 0 Then
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "performSearch", _talentLogging_LogPrefix & "txtDespatchNoteID.Text= " & txtDespatchNoteID.Text.Trim, "DespatchLog")
            paymentReference = 0
            newBatchID = 0
        ElseIf txtPaymentReference.Text.Trim.Length > 0 Then
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "performSearch", _talentLogging_LogPrefix & "txtPaymentReference.Text= " & txtPaymentReference.Text.Trim, "DespatchLog")
            paymentReference = txtPaymentReference.Text
            newBatchID = createDespatchTransactionItemsForPayref(paymentReference, "1")
        ElseIf txtTicketNumber.Text.Trim().Length > 0 Then
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "performSearch", _talentLogging_LogPrefix & "txtTicketNumber.Text= " & txtTicketNumber.Text.Trim, "DespatchLog")
            Dim dtTicketSearchResults As Data.DataTable = getPaymentReferenceByTicketNumber(txtTicketNumber.Text.Trim())
            If ltlErrorDetails.Text.Length = 0 Then
                Dim strType As String = dtTicketSearchResults.Rows(0)("ReferenceType")
                paymentReference = dtTicketSearchResults.Rows(0)("ReferenceValue")
                newBatchID = createDespatchTransactionItemsForPayref(paymentReference, strType)
                If strType = "2" Then sType = RESERVATION_TYPE
            End If
        End If

        If ltlErrorDetails.Text.Length = 0 Then GetOrderDetails(newBatchID, paymentReference, sType)
    End Sub

    ''' <summary>
    ''' Get the order details based on the fiven batch ID and payemnt reference information
    ''' </summary>
    ''' <param name="newBatchID">The new batch ID</param>
    ''' <param name="paymentReference">The payment reference</param>
    ''' <remarks></remarks>
    Private Sub GetOrderDetails(ByVal newBatchID As Long, ByVal paymentReference As Long, ByVal sType As String)
        Dim TalentDespatch As New TalentDespatch
        Dim DETalentDespatch As New DEDespatch
        Dim err As New ErrorObj

        With DETalentDespatch
            If newBatchID > 0 AndAlso paymentReference > 0 Then
                .BatchID = newBatchID
                .PaymentRef = paymentReference
                .Type = sType
            Else
                If txtDespatchNoteID.Text.Length > 0 Then
                    Dim sParts As String() = txtDespatchNoteID.Text.Split("-")
                    .Type = sParts(0).ToString
                    .BatchID = CType(sParts(1), Long)
                    .PaymentRef = CType(sParts(2), Long)
                    .RefreshBatch = "Y"
                Else
                    err.HasError = True
                End If
            End If
            .Source = GlobalConstants.SOURCE
            .SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
            .CommandTimeout = Talent.Common.Utilities.CheckForDBNull_Int(_ucr.Attribute("SearchCommandTimeout"))
        End With

        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "GetOrderDetails", _talentLogging_LogPrefix & "Reference= " & DETalentDespatch.PaymentRef & ", Type=" & DETalentDespatch.Type & ", BatchID=" & DETalentDespatch.BatchID, "DespatchLog")

        If Not err.HasError Then
            _PaymentReference = DETalentDespatch.PaymentRef
            _paymentOrReservationType = DETalentDespatch.Type
            TalentDespatch.Settings = _agentSettingsObject
            TalentDespatch.DeDespatch = DETalentDespatch
            err = TalentDespatch.RetrieveDespatchProcessItems()
        End If

        If Not err.HasError AndAlso TalentDespatch.ResultDataSet IsNot Nothing AndAlso TalentDespatch.ResultDataSet.Tables("DespatchProcessItems") IsNot Nothing Then
            If TalentDespatch.ResultDataSet.Tables("DespatchProcessItems").Rows.Count > 0 Then
                _talentLogging.GeneralLog(ProfileHelper.GetPageName, "GetOrderDetails", _talentLogging_LogPrefix & "TalentDespatch.ResultDataSet.Tables(DespatchProcessItems).Rows.Count= " & TalentDespatch.ResultDataSet.Tables("DespatchProcessItems").Rows.Count.ToString, "DespatchLog")
                rptItemstoDespatch.DataSource = TalentDespatch.ResultDataSet.Tables("DespatchProcessItems")
                rptItemstoDespatch.DataBind()


                ltlCATpayrefsValue.Text = getCatPayrefs(TalentDespatch.ResultDataSet.Tables("DespatchProcessItems"))

                hdfTimeStampToken.Value = TalentDespatch.ResultDataSet.Tables("DespatchProcessItems").Rows(0)("TimeStampToken")
                hdfPaymentReservationType.Value = DETalentDespatch.Type

                SetPageWithOrderDetails(DETalentDespatch.BatchID)
                setCustomerDetails(TalentDespatch.ResultDataSet.Tables("DespatchProcessItems"), TalentDespatch.DeDespatch.PaymentRef, TalentDespatch.DeDespatch.Type)
                setCourierDetails()
                txtNotes.TabIndex = _intTabIndex + 1
                txtNotes.Text = TalentDespatch.ResultDataSet.Tables("DespatchProcessItems").Rows(0)("Notes").ToString().Trim()
                btnFinishOrder.TabIndex = _intTabIndex + 2
            Else
                If TalentDespatch.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso TalentDespatch.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 AndAlso TalentDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)(0) <> String.Empty Then
                    _talentLogging.GeneralLog(ProfileHelper.GetPageName, "GetOrderDetails", _talentLogging_LogPrefix & "ERROR1: TalentDespatch.ResultDataSet.Tables(ErrorStatus).Rows(0)(0)=" & TalentDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)(0).ToString, "DespatchLog")
                    If TalentDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)(0) = "CS" Then
                        ltlErrorDetails.Text = _ucr.Content("CancelledTicketSeatError", _languageCode, True)
                    End If
                Else
                    ltlErrorDetails.Text = _ucr.Content("NothingFoundError", _languageCode, True)
                    _talentLogging.GeneralLog(ProfileHelper.GetPageName, "GetOrderDetails", _talentLogging_LogPrefix & "ERROR2: ltlErrorDetails.Text=" & TalentDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)(0).ToString, "DespatchLog")
                End If

                LoadInitialDespatchProcess()
            End If
        Else
            ltlErrorDetails.Text = err.ErrorStatus + " (" + err.ErrorNumber + ": " + err.ErrorMessage + ")"
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "GetOrderDetails", _talentLogging_LogPrefix & "ERROR3: ltlErrorDetails.Text=" & ltlErrorDetails.Text, "DespatchLog")
        End If
    End Sub

    ''' <summary>
    ''' set DespatchNoteID,PaymentReference and TicketNumber session variables.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setSessionVariables()
        Session("DespatchNoteID") = txtDespatchNoteID.Text
        Session("PaymentReference") = txtPaymentReference.Text
        Session("TicketNumber") = txtTicketNumber.Text
    End Sub
    Private Function getCatPayrefs(ByVal dtDespatchProcessItems As DataTable) As String

        Dim dvCatpayrefs As DataView = dtDespatchProcessItems.AsDataView
        Dim Filter As String = "catpayref> 0"
        dvCatpayrefs.RowFilter = Filter
        dvCatpayrefs.Sort = "catpayref"
        Dim catref As String = String.Empty
        Dim previouscatref As String = String.Empty
        Dim strCatpayrefs As String = String.Empty
        Dim dtCatPayrefs = dvCatpayrefs.ToTable
        For Each row As DataRow In dtCatPayrefs.Rows
            catref = row("catpayref")
            If catref <> previouscatref Then
                previouscatref = catref
                If strCatpayrefs <> String.Empty Then
                    strCatpayrefs += ", "
                End If
                strCatpayrefs += catref
            End If
        Next
        Return strCatpayrefs
    End Function
    ''' <summary>
    ''' Set the customer details information panel
    ''' </summary>
    ''' <param name="dtDespatchProcessItems">The despatch process items table</param>
    ''' <param name="reference">The current payment or reservation reference for the order information</param>
    ''' <param name="type">The 'p'ayment type or 'r'eservation type</param>
    ''' <remarks></remarks>
    Private Sub setCustomerDetails(ByVal dtDespatchProcessItems As DataTable, ByVal reference As String, ByVal type As String)
        If plhDespatchProcessInfo.Visible Then
            ltlPostageLabel.Text = _ucr.Content("ltlPostage", _languageCode, True)
            ltlPostageValue.Text = _ucr.Content("ltlPostageValue", _languageCode, True)
            ltlCATpayrefsLabel.Text = _ucr.Content("ltlCATpayrefsLabel", _languageCode, True)
            Select Case TEBUtilities.CheckForDBNull_String(dtDespatchProcessItems.Rows(0)("FulfilmentMethod"))
                Case Is = GlobalConstants.POST_FULFILMENT
                    ltlPostageValue.Text = ltlPostageValue.Text.Replace("<<POSTAGE_SELECTED>>", _ucr.Content("StandardPostValue", _languageCode, True))
                    ltlPostageValue.Text = ltlPostageValue.Text.Replace("<<ZONE>>", dtDespatchProcessItems.Rows(0)("GeographicalZone"))
                Case Is = GlobalConstants.REG_POST_FULFILMENT
                    ltlPostageValue.Text = ltlPostageValue.Text.Replace("<<POSTAGE_SELECTED>>", _ucr.Content("RegisteredPostValue", _languageCode, True))
                    ltlPostageValue.Text = ltlPostageValue.Text.Replace("<<ZONE>>", dtDespatchProcessItems.Rows(0)("GeographicalZone"))
                Case Is = GlobalConstants.PRINT_FULFILMENT : ltlPostageValue.Text = _ucr.Content("PrintFulfilmentValue", _languageCode, True)
                Case Is = GlobalConstants.COLLECT_FULFILMENT : ltlPostageValue.Text = _ucr.Content("CollectFulfilmentValue", _languageCode, True)
                Case Is = GlobalConstants.PRINT_AT_HOME_FULFILMENT : ltlPostageValue.Text = _ucr.Content("PrintAtHomeFulfilmentValue", _languageCode, True)
                Case Is = GlobalConstants.SMARTCARD_UPLOAD_FULFILMENT : ltlPostageValue.Text = _ucr.Content("SmartcardUploadFulfilmentValue", _languageCode, True)
                Case Else : ltlPostageValue.Text = _ucr.Content("NoPostage", _languageCode, True)
            End Select

            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(TCUtilities.convertToBool(dtDespatchProcessItems.Rows(0)("CharityFee"))) Then
                plhCharityFeeAdded.Visible = True
                ltlCharityFeeAdded.Text = _ucr.Content("CharityFeeAdded", _languageCode, True)
            Else
                plhCharityFeeAdded.Visible = False
            End If
            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(TCUtilities.convertToBool(dtDespatchProcessItems.Rows(0)("GiftWrap"))) Then
                plhGiftWrappingAdded.Visible = True
                ltlGiftWrappingAdded.Text = _ucr.Content("GiftWrapFeeAdded", _languageCode, True)
            Else
                plhGiftWrappingAdded.Visible = False
            End If

            If type = PAYMENT_TYPE Then
                hplPaymentReference.NavigateUrl = ResolveUrl("~/PagesLogin/Orders/PurchaseDetails.aspx?payref=") & reference
                hplPaymentReference.Text = reference
            ElseIf type = RESERVATION_TYPE Then
                ltlReservationReference.Text = reference
            End If

            Dim customerNumber As String = dtDespatchProcessItems.Rows(0)("Customer")
            Dim nameFormat As String = _ucr.Content("CustomerNameFormat", _languageCode, True)
            nameFormat = nameFormat.Replace("<<NAME>>", dtDespatchProcessItems.Rows(0)("CustomerName"))
            nameFormat = nameFormat.Replace("<<CUSTOMER_NUMBER>>", customerNumber.TrimStart(GlobalConstants.LEADING_ZEROS))
            ltlCustomerLabel.Text = _ucr.Content("ltlCustomerLabel", _languageCode, True)
            hdfCustomerNumber.Value = customerNumber
            hplCustomerDetails.Text = nameFormat
            ltlGenericCustomer.Visible = False

            Dim navigateUrl As New StringBuilder
            navigateUrl.Append(ResolveUrl("~/Redirect/TicketingGateway.aspx?page=validatesession.aspx&function=validatesession"))
            navigateUrl.Append("&returnurl=").Append(Server.UrlEncode("~/PagesLogin/Profile/UpdateProfile.aspx"))
            navigateUrl.Append("&t=1")
            navigateUrl.Append("&l=").Append(Server.UrlEncode(TCUtilities.TripleDESEncode(customerNumber, ModuleDefaults.NOISE_ENCRYPTION_KEY)))
            navigateUrl.Append("&p=").Append(Server.UrlEncode(TCUtilities.TripleDESEncode(TCUtilities.RandomString(10), ModuleDefaults.NOISE_ENCRYPTION_KEY)))
            navigateUrl.Append("&y=N")
            If Profile.IsAnonymous Then
                If customerNumber = "*GENERIC" Then
                    hplCustomerDetails.Visible = False
                    ltlGenericCustomer.Visible = True
                    ltlGenericCustomer.Text = "*GENERIC"
                Else
                    hplCustomerDetails.NavigateUrl = navigateUrl.ToString()
                End If
            Else
                ltlGenericCustomer.Visible = False
                If Profile.User.Details.LoginID = customerNumber Then
                    hplCustomerDetails.NavigateUrl = ResolveUrl("~/PagesLogin/Profile/UpdateProfile.aspx")
                Else
                    hplCustomerDetails.NavigateUrl = navigateUrl.ToString()
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Set the courier default options
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setCourierDetails()
        If ModuleDefaults.CourierTrackingReferences AndAlso _paymentOrReservationType = PAYMENT_TYPE Then
            Dim courierServiceValues As New Dictionary(Of String, String)
            Dim courierPrefixValues As New Dictionary(Of String, String)
            TDataObjects.Settings = _agentSettingsObject
            Try
                courierServiceValues = TDataObjects.AppVariableSettings.TblDefaults.GetDespatchCourierServiceValues(_ucr.BusinessUnit, _ucr.PartnerCode, _languageCode)
                ddlCourierService.DataSource = courierServiceValues
                ddlCourierService.DataValueField = "Key"
                ddlCourierService.DataTextField = "Value"
                ddlCourierService.DataBind()

                courierPrefixValues = TDataObjects.AppVariableSettings.TblDefaults.GetDespatchCourierPrefixValues(_ucr.BusinessUnit, _ucr.PartnerCode, _languageCode)
                rdoCourierType.DataSource = courierPrefixValues
                rdoCourierType.DataValueField = "Key"
                rdoCourierType.DataTextField = "Value"
                rdoCourierType.DataBind()
                rdoCourierType.SelectedValue = String.Empty
            Catch ex As Exception
                ltlErrorDetails.Text = _ucr.Content("ErrorSettingCourierDefaults", _languageCode, True)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Format the ticket number and status string values for the despatch item data entity
    ''' </summary>
    ''' <param name="textBoxValue">The ticket line ticket number value</param>
    ''' <param name="ticketNumber">The ticket number to be used in the data entity</param>
    ''' <param name="status">The status code to be used in the data entity</param>
    ''' <remarks></remarks>
    Private Sub formatTicketNumberAndStatus(ByRef textBoxValue As String, ByRef ticketNumber As String, ByRef status As String)
        If textBoxValue = _membershipLabel Then
            ticketNumber = String.Empty
            status = MEMBERSHIP_STATUS_CODE
        ElseIf textBoxValue = _sentLabel Then
            ticketNumber = String.Empty
            status = SENT_STATUS_CODE
        ElseIf textBoxValue = _notForDespatchLabel Then
            ticketNumber = String.Empty
            status = NOT_FOR_DESPATCH_STATUS_CODE
        Else
            ticketNumber = textBoxValue
            status = String.Empty
        End If
    End Sub

    ''' <summary>
    ''' Loop through the repeater and the despatch item collection and find the error code. Set a CSS class name against the ticket number text box if an error has occurred
    ''' </summary>
    ''' <param name="despatchCollection">The despatch item collection</param>
    ''' <remarks></remarks>
    Private Sub highlightTicketInError(ByVal despatchCollection As List(Of DEDespatchItem))
        If rptItemstoDespatch.Items.Count > 0 AndAlso despatchCollection.Count > 0 Then
            Dim i As Integer = 0
            For Each despatchItem As DEDespatchItem In despatchCollection
                Dim rptItem As RepeaterItem = rptItemstoDespatch.Items(i)
                Dim txtTicketNumber As TextBox = CType(rptItem.FindControl("txtTicketNumber"), TextBox)
                If txtTicketNumber IsNot Nothing Then txtTicketNumber.CssClass = "ebiz-ticket-number-field"
                If despatchItem.ErrorCode IsNot Nothing AndAlso despatchItem.ErrorCode.Length > 0 Then
                    If rptItemstoDespatch.Items(i) IsNot Nothing Then
                        If txtTicketNumber IsNot Nothing AndAlso txtTicketNumber.Text = despatchItem.TicketNumber Then
                            txtTicketNumber.CssClass = "ebiz-ticket-number-field ebiz-invalid-ticket"
                        Else
                            Dim hdfMembershipRFID As HiddenField = CType(rptItem.FindControl("hdfMembershipRFID"), HiddenField)
                            If hdfMembershipRFID IsNot Nothing AndAlso hdfMembershipRFID.Value.Trim.Length > 0 AndAlso hdfMembershipRFID.Value = despatchItem.MembershipRFID Then
                                txtTicketNumber.CssClass = "ebiz-ticket-number-field ebiz-invalid-ticket"
                            End If
                        End If
                    End If
                End If
                i += 1
            Next
        End If
    End Sub


#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Create despatch process records for a payment (populates file PY070) and returns the new batch ID  
    ''' </summary>
    ''' <param name="strPayref"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function createDespatchTransactionItemsForPayref(ByVal strPayref As Long, Optional ByVal strType As String = "") As Long
        Dim newBatchId As Long = 0
        Dim err As New ErrorObj
        Dim despatch As New TalentDespatch
        Dim deDespatch As New DEDespatch

        despatch.Settings = _agentSettingsObject
        deDespatch.PaymentRef = strPayref
        deDespatch.Type = strType
        deDespatch.FromDate = String.Empty
        deDespatch.ToDate = String.Empty
        deDespatch.SuperType = String.Empty
        deDespatch.SubType = String.Empty
        deDespatch.Product = String.Empty
        deDespatch.DeliveryMethod = "0"
        deDespatch.Country = "0"
        deDespatch.Postcode = "0"
        deDespatch.GiftWrap = "N"
        deDespatch.IncludeGenericTransactions = True
        deDespatch.AttributeCategory = String.Empty
        deDespatch.SessionID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
        deDespatch.CommandTimeout = Talent.Common.Utilities.CheckForDBNull_Int(_ucr.Attribute("SearchCommandTimeout"))
        despatch.DeDespatch = deDespatch

        err = despatch.RetrieveDespatchTransactionItems()
        If Not err.HasError AndAlso despatch.ResultDataSet IsNot Nothing AndAlso despatch.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            If despatch.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim errorCode As String = despatch.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode")
                Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(errorCode)
                ltlErrorDetails.Text = talentErrorMsg.ERROR_MESSAGE
            Else
                If despatch.ResultDataSet.Tables("DespatchTransactionsItems").Rows.Count > 0 Then
                    newBatchId = despatch.ResultDataSet.Tables("DespatchTransactionsItems").Rows(0)("TransactionId")
                Else
                    ltlErrorDetails.Text = _ucr.Content("NothingFoundError", _languageCode, True)
                End If
            End If
        Else
            '            ltlErrorDetails.Text = _ucr.Content("GenericError", _languageCode, True)
            ltlErrorDetails.Text = err.ErrorStatus + " (" + err.ErrorNumber + ": " + err.ErrorMessage + ")"
        End If
        Return newBatchId
    End Function

    ''' <summary>
    ''' Return the payment reference for a given ticket number
    ''' </summary>
    ''' <param name="ticketNumber">The given ticket number</param>
    ''' <returns>The matching payment reference</returns>
    ''' <remarks></remarks>
    Private Function getPaymentReferenceByTicketNumber(ByVal ticketNumber As String) As Data.DataTable
        Dim err As New ErrorObj
        Dim despatch As New TalentDespatch
        Dim deDespatch As New DEDespatch
        Dim lngReferenceValue As Long = 0
        Dim dtDespatchSearchResults As New DataTable

        despatch.Settings = _agentSettingsObject
        deDespatch.TicketNumber = ticketNumber
        deDespatch.Source = GlobalConstants.SOURCE
        despatch.DeDespatch = deDespatch
        err = despatch.RetrievePaymentReferenceFromTicketNumber()

        If Not err.HasError AndAlso despatch.ResultDataSet IsNot Nothing AndAlso despatch.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            If TEBUtilities.CheckForDBNull_String(despatch.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred")) = GlobalConstants.ERRORFLAG Then
                ltlErrorDetails.Text = _ucr.Content("GenericError", _languageCode, True)
            Else
                If despatch.ResultDataSet.Tables("TicketSearchResults").Rows.Count > 0 Then
                    dtDespatchSearchResults = despatch.ResultDataSet.Tables("TicketSearchResults")
                    lngReferenceValue = despatch.ResultDataSet.Tables("TicketSearchResults").Rows(0)("ReferenceValue")
                End If
                If lngReferenceValue = 0 Then ltlErrorDetails.Text = _ucr.Content("NothingFoundError", _languageCode, True)
            End If
        Else
            ltlErrorDetails.Text = _ucr.Content("GenericError", _languageCode, True)
        End If
        Return dtDespatchSearchResults
    End Function

    ''' <summary>
    ''' Create the CSV file based on the given save path and file name
    ''' </summary>
    ''' <param name="savePath">The CSV file local save path</param>
    ''' <param name="fileName">The CSV filename</param>
    ''' <returns><c>True</c> if the file has been created successfully</returns>
    ''' <remarks></remarks>
    Private Function createCSVFile(ByVal savePath As String, ByVal fileName As String) As Boolean
        Dim fileCreated As Boolean = False
        Dim dsCourier As DataSet = getPurchaseOrderDetails()
        Try
            Const COMMA As String = ""","""
            Const HCOMMA As String = """"""","""""""

            Dim addressLine2 As String = String.Empty
            Dim sw As New StreamWriter(savePath & fileName)
            sw.Write("Registered On")
            sw.Write(",""""""")
            sw.Write("Delivery customer ref. 1")
            sw.Write(HCOMMA)
            sw.Write("Delivery Contact Name")
            sw.Write(HCOMMA)
            sw.Write("Delivery notification SMS number")
            sw.Write(HCOMMA)
            sw.Write("Delivery Organisation/Name")
            sw.Write(HCOMMA)
            sw.Write("Delivery Address line 1 (property/street)")
            sw.Write(HCOMMA)
            sw.Write("Delivery Address line 2 (locality)")
            sw.Write(HCOMMA)
            sw.Write("Delivery Address line 3 (city)")
            sw.Write(HCOMMA)
            sw.Write("Delivery Postcode")
            sw.Write(HCOMMA)
            sw.Write("Delivery Country Code")
            sw.Write(HCOMMA)
            sw.Write("Delivery notification email address")
            sw.Write(HCOMMA)
            sw.Write("Delivery customer ref. 2")
            sw.Write(HCOMMA)
            sw.Write("customer Value")
            sw.Write(HCOMMA)
            sw.Write("content")
            sw.Write(HCOMMA)
            sw.Write("Service")
            sw.Write(HCOMMA)
            sw.Write("Weight")
            sw.Write(HCOMMA)
            sw.Write("Package")
            sw.Write(HCOMMA)
            sw.Write("Telephone")
            sw.WriteLine("""""""")

            sw.Write("""")
            sw.Write(CDate(dsCourier.Tables("StatusResults").Rows(0)("SaleDate")).ToString("yyyy-MM-dd")) 'Registered On
            sw.Write(COMMA)
            sw.Write("P" & _PaymentReference) 'Delivery customer ref. 1
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("StatusResults").Rows(0)("deliveryContactName").ToString().Trim()) 'Delivery Contact Name
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("CustomerDetails").Rows(0)("MobileNumber").ToString().Trim()) 'Delivery notification SMS number
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("CustomerDetails").Rows(0)("CompanyName").ToString().Trim()) 'Delivery Organisation/Name
            sw.Write(COMMA)
            addressLine2 = dsCourier.Tables("StatusResults").Rows(0)("deliveryAddress2").ToString().Trim()
            If addressLine2.Length > 0 Then addressLine2 = ", " & addressLine2
            sw.Write(dsCourier.Tables("StatusResults").Rows(0)("deliveryAddress1").ToString().Trim() & addressLine2) 'Delivery Address line 1 (property/street)
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("StatusResults").Rows(0)("deliveryAddress3").ToString().Trim()) 'Delivery Address line 2 (locality)
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("StatusResults").Rows(0)("deliveryAddress4").ToString().Trim()) 'Delivery Address line 3 (city)
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("StatusResults").Rows(0)("deliveryPostCode").ToString().Trim()) 'Delivery Postcode
            sw.Write(COMMA)
            sw.Write(getCountryCodeFromDescription(dsCourier.Tables("StatusResults").Rows(0)("deliveryAddress5").ToString().Trim())) 'Delivery Country Code
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("CustomerDetails").Rows(0)("EmailAddress").ToString().Trim()) 'Delivery notification email address
            sw.Write(COMMA)
            sw.Write(String.Empty) 'Delivery customer ref. 2
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("StatusResults").Rows(0)("TotalPrice").ToString().Trim()) 'customer Value
            sw.Write(COMMA)
            sw.Write("Tickets") 'content
            sw.Write(COMMA)
            sw.Write(ddlCourierService.SelectedValue) 'Service
            sw.Write(COMMA)
            sw.Write("1") 'Weight
            sw.Write(COMMA)
            sw.Write("1") 'Package
            sw.Write(COMMA)
            sw.Write(dsCourier.Tables("CustomerDetails").Rows(0)("MobileNumber").ToString().Trim()) 'Telephone
            sw.Write("""")
            sw.Close()
            fileCreated = True
        Catch ex As Exception
            fileCreated = False
        End Try
        Return fileCreated
    End Function

    ''' <summary>
    ''' Get the purchase details for the current order
    ''' </summary>
    ''' <returns>Formatted dataset of records for the order information</returns>
    ''' <remarks></remarks>
    Private Function getPurchaseOrderDetails() As DataSet
        Dim order As New TalentOrder
        Dim deTicketingItemDetails As New DETicketingItemDetails
        Dim deCust As New DECustomer()
        Dim tc As New TalentCustomer()
        Dim deCustV11 As New DECustomerV11()
        Dim err As New ErrorObj
        Dim dsCourier As New DataSet

        deTicketingItemDetails.PaymentReference = _PaymentReference
        deTicketingItemDetails.UnprintedRecords = String.Empty
        deTicketingItemDetails.SetAsPrinted = "N"
        deTicketingItemDetails.EndOfSale = "Y"
        deTicketingItemDetails.RetryFailure = True
        deTicketingItemDetails.Src = GlobalConstants.SOURCE

        order.Settings = _agentSettingsObject
        order.Settings.Cacheing = False
        order.Dep.CollDEOrders.Add(deTicketingItemDetails)
        order.Dep.CustNumberPayRefShouldMatch = False
        order.Dep.LastRecordNumber = 0
        order.Dep.TotalRecords = 0
        order.Dep.PaymentReference = _PaymentReference
        order.Dep.CallId = 0

        deCust.CustomerNumber = hdfCustomerNumber.Value
        deCust.UserName = hdfCustomerNumber.Value
        deCustV11.DECustomersV1.Add(deCust)
        tc.DeV11 = deCustV11
        tc.Settings = _agentSettingsObject

        err = order.OrderDetails 'Call WS030R
        If Not err.HasError AndAlso order.ResultDataSet IsNot Nothing AndAlso order.ResultDataSet.Tables.Count > 1 Then
            Dim dtStatusResults As DataTable = order.ResultDataSet.Tables("StatusResults").Copy
            dsCourier.Tables.Add(dtStatusResults)
            order.ResultDataSet = Nothing
            err = tc.CustomerRetrieval() 'Call WS009R
            If Not err.HasError AndAlso tc.ResultDataSet IsNot Nothing AndAlso tc.ResultDataSet.Tables.Count = 2 Then
                Dim dtCustomerDetails As New DataTable
                dtCustomerDetails = tc.ResultDataSet.Tables(1).Copy
                dtCustomerDetails.TableName = "CustomerDetails"
                dsCourier.Tables.Add(dtCustomerDetails)
            End If
        End If
        Return dsCourier
    End Function

    ''' <summary>
    ''' Get the country code 'GBP' from the given country description
    ''' </summary>
    ''' <param name="countryDescription">The country description</param>
    ''' <returns>The country code string</returns>
    ''' <remarks></remarks>
    Private Function getCountryCodeFromDescription(ByVal countryDescription As String) As String
        Dim countryCode As String = String.Empty
        TDataObjects.Settings = _agentSettingsObject
        countryCode = TDataObjects.ProfileSettings.tblCountry.GetCountryCodeByDescription(countryDescription)
        Return countryCode
    End Function

    ''' <summary>
    ''' Check the order token that was created for the order to ensure the same order being processed is the one that was returned
    ''' </summary>
    ''' <returns>True if the token is correct</returns>
    ''' <remarks></remarks>
    Private Function OrderTokenCheck() As Boolean
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "CheckOrder", _talentLogging_LogPrefix & "Start ", "DespatchLog")
        Dim ret As Boolean = False
        Dim err As New ErrorObj
        Dim talDespatch As New TalentDespatch
        Dim despatchDataEntity As New DEDespatch
        Dim settings As DESettings = _agentSettingsObject

        despatchDataEntity.PaymentRef = HttpContext.Current.Session("DespatchProcessSuccess_pyrf")
        despatchDataEntity.TimeStampToken = HttpContext.Current.Session("DespatchProcessSuccess_token")
        despatchDataEntity.Mode = HttpContext.Current.Session("DespatchProcessSuccess_type")
        talDespatch.Settings = settings
        talDespatch.DeDespatch = despatchDataEntity
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "CheckOrder", _talentLogging_LogPrefix & "About to call DespatchOrderTokenCheck with despatchDataEntity.PaymentRef=" & despatchDataEntity.PaymentRef.ToString & ", despatchDataEntity.TimeStampToken=" & despatchDataEntity.TimeStampToken.ToString & ", despatchDataEntity.Mode=" & despatchDataEntity.Mode.ToString, "DespatchLog")
        err = talDespatch.DespatchOrderTokenCheck()

        If Not err.HasError Then
            If talDespatch.ResultDataSet IsNot Nothing AndAlso talDespatch.ResultDataSet.Tables("TokenCheckResults").Rows.Count = 1 Then
                If talDespatch.ResultDataSet.Tables("TokenCheckResults").Rows(0)(0).ToString.Trim = "" Then
                    ret = True
                End If
            End If
        End If

        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "CheckOrder", _talentLogging_LogPrefix & "Result of OrderTokenCheck = " & ret.ToString, "DespatchLog")
        Return ret
    End Function

    ''' <summary>
    ''' Returns a string that disables the print button and changes its text
    ''' </summary>
    ''' <param name="ConfirmButton">The button we are applying the change to</param>
    ''' <returns>The string we need to set on the onlick attribute</returns>
    ''' <remarks></remarks>
    Private Function getJSFunctionForConfirmButton(ByRef ConfirmButton As Button) As String
        Dim javascriptFunction As New StringBuilder()
        javascriptFunction.Append("if (typeof(Page_ClientValidate) == 'function') { ")
        javascriptFunction.Append("var oldPage_IsValid = Page_IsValid; var oldPage_BlockSubmit = Page_BlockSubmit;")
        javascriptFunction.Append("if (Page_ClientValidate('")
        javascriptFunction.Append(ConfirmButton.ValidationGroup)
        javascriptFunction.Append("') == false) {")
        javascriptFunction.Append(" Page_IsValid = oldPage_IsValid; Page_BlockSubmit = oldPage_BlockSubmit; return false; }} ")
        javascriptFunction.Append("this.value = '")
        javascriptFunction.Append(_ucr.Content("ProcessingText", _languageCode, True))
        javascriptFunction.Append("';")
        javascriptFunction.Append("this.disabled = true;")
        javascriptFunction.Append(Me.Page.ClientScript.GetPostBackEventReference(ConfirmButton, Nothing))
        javascriptFunction.Append(";")
        javascriptFunction.Append("return true;")
        Return javascriptFunction.ToString()
    End Function
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the foundation reveal unique ID name for each ticket line in the repeater
    ''' </summary>
    ''' <param name="index">The current item index</param>
    ''' <returns>The formatted reveal ID name</returns>
    ''' <remarks></remarks>
    Public Function GetGiftMessageRevealID(ByVal index As Integer) As String
        Dim id As String = "GiftMessage"
        id = id & index
        Return id
    End Function

    ''' <summary>
    ''' Get the line status descriptive value based on the status code and barcode
    ''' </summary>
    ''' <param name="seatStatus">The current seat status</param>
    ''' <param name="barcode">The current seat barcode</param>
    ''' <param name="membershipRFID">Membership ID</param>
    ''' <param name="PrintedCount">Number of times item has been printed</param>
    ''' <returns>The descriptive text</returns>
    ''' <remarks></remarks>
    Public Function GetStatusText(ByVal seatStatus As String, ByVal barcode As String, ByVal membershipRFID As String, PrintedCount As String, ByVal IsMembershipRenewal As String, ByVal NoPrintRequired As String) As String
        Dim statusText As String = String.Empty
        If Not AgentProfile.AgentPermissions.CanPrintDespatchCards AndAlso ConvertFromISeriesYesNoToBoolean(IsMembershipRenewal) Then
            If PrintedCount <> "0" Then
                statusText = _ucr.Content("ItemRenewedText", _languageCode, True)
            ElseIf PrintedCount = "0" Then
                statusText = _ucr.Content("ItemNotRenewedText", _languageCode, True)
            End If
        Else
        If seatStatus = NOT_FOR_DESPATCH_STATUS_CODE Then
            statusText = _notForDespatchLabel
        ElseIf Usage = UsageType.PRINT AndAlso PrintedCount <> "0" Then
                statusText = _ucr.Content("ItemPrintedText", _languageCode, True)
            ElseIf Usage = UsageType.PRINT AndAlso PrintedCount = "0" Then
                statusText = _ucr.Content("ItemNotPrintedText", _languageCode, True)
        ElseIf seatStatus = SENT_STATUS_CODE OrElse PrintedCount <> "0" Then
                statusText = _sentLabel
                ' If previously scanned ticket deleted show as not despatched    
                If Usage = UsageType.DESPATCH And barcode.Trim().Length = 0 Then
                    statusText = _ucr.Content("NotDespatchedLabel", _languageCode, True)
                End If
        ElseIf seatStatus = MEMBERSHIP_STATUS_CODE AndAlso Not String.IsNullOrEmpty(membershipRFID) Then
            statusText = _ucr.Content("DespatchedLabel", _languageCode, True)
            End If
        End If
       
        If ConvertFromISeriesYesNoToBoolean(NoPrintRequired) Then
            statusText = _ucr.Content("NoPrintRequired", _languageCode, True)
        End If

        Return statusText
    End Function
    ''' <summary>
    ''' Populates despatch items list for the FinishOrder() function in turn calls WS273R
    ''' </summary>
    ''' <param name="talentLogging_LogDespatchCompletion"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function populateDespatchItemsForFinishOrder(ByRef talentLogging_LogDespatchCompletion As String, ByRef haveAllItemsSelectedForPrint As Boolean) As List(Of DEDespatchItem)
        Dim despatchItems As New List(Of DEDespatchItem)
        haveAllItemsSelectedForPrint = True
        For Each item As RepeaterItem In rptItemstoDespatch.Items
            Dim hdfProductCode As HiddenField = CType(item.FindControl("hdfProductCode"), HiddenField)
            Dim hdfStand As HiddenField = CType(item.FindControl("hdfStand"), HiddenField)
            Dim hdfArea As HiddenField = CType(item.FindControl("hdfArea"), HiddenField)
            Dim hdfRowNumber As HiddenField = CType(item.FindControl("hdfRowNumber"), HiddenField)
            Dim hdfSeat As HiddenField = CType(item.FindControl("hdfSeat"), HiddenField)
            Dim hdfAlphaSuffix As HiddenField = CType(item.FindControl("hdfAlphaSuffix"), HiddenField)
            Dim txtTicketNumber As TextBox = CType(item.FindControl("txtTicketNumber"), TextBox)
            Dim hdfMembershipRFID As HiddenField = CType(item.FindControl("hdfMembershipRFID"), HiddenField)
            Dim hdfMembershipMagScan As HiddenField = CType(item.FindControl("hdfMembershipMagScan"), HiddenField)
            Dim hdfMembershipMetalBadge As HiddenField = CType(item.FindControl("hdfMembershipMetalBadge"), HiddenField)
            Dim chkPrint As CheckBox = CType(item.FindControl("chkPrint"), CheckBox)
            Dim despatchItem As New DEDespatchItem
            Dim despatchSeatDetails As New DESeatDetails
            despatchItem.Product = hdfProductCode.Value
            despatchSeatDetails.Stand = hdfStand.Value
            despatchSeatDetails.Area = hdfArea.Value
            despatchSeatDetails.Row = hdfRowNumber.Value
            despatchSeatDetails.Seat = hdfSeat.Value
            despatchSeatDetails.AlphaSuffix = hdfAlphaSuffix.Value
            despatchItem.SeatDetails = despatchSeatDetails
            formatTicketNumberAndStatus(txtTicketNumber.Text, despatchItem.TicketNumber, despatchItem.Status)
            despatchItem.MembershipRFID = hdfMembershipRFID.Value
            despatchItem.MembershipMagScan = hdfMembershipMagScan.Value
            despatchItem.MembershipMetalBadge = hdfMembershipMetalBadge.Value
            If chkPrint.Checked Then
                despatchItem.PrintTicket = "1"
            Else
                despatchItem.PrintTicket = "0"
                haveAllItemsSelectedForPrint = False
            End If
            despatchItems.Add(despatchItem)
            talentLogging_LogDespatchCompletion = talentLogging_LogDespatchCompletion & despatchItem.Product & despatchSeatDetails.Stand & despatchSeatDetails.Area & _
                                                    despatchSeatDetails.Row & despatchSeatDetails.Seat & despatchSeatDetails.AlphaSuffix & "," & despatchItem.TicketNumber & "|"
        Next

        Return despatchItems
    End Function
    ''' <summary>
    ''' Finish order processing
    ''' </summary>
    ''' <returns>Error object for any issues communicating with TALENT</returns>
    ''' <remarks></remarks>
    ''' 
    Public Function FinishOrder() As ErrorObj
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "Start", "DespatchLog")
        Dim err As New ErrorObj
        If Page.IsValid() AndAlso rptItemstoDespatch.Items.Count > 0 Then
            Dim talDespatch As New TalentDespatch
            Dim despatchDataEntity As New DEDespatch
            Dim settings As DESettings = _agentSettingsObject
            Dim despatchItems As New List(Of DEDespatchItem)

            despatchDataEntity.SessionID = Profile.Basket.Basket_Header_ID
            despatchDataEntity.BatchID = hdfBatchID.Value
            despatchDataEntity.TimeStampToken = hdfTimeStampToken.Value
            despatchDataEntity.Comments = TCUtilities.ConvertASCIIHexValue(txtNotes.Text, _ucr.BusinessUnit, _ucr.PartnerCode, _languageCode, settings)
            despatchDataEntity.AgentName = AgentProfile.Name
            If txtDespatchNoteID.Text.Trim().Length > 0 AndAlso txtDespatchNoteID.Text.Substring(0, 1) = RESERVATION_TYPE Then
                despatchDataEntity.Type = RESERVATION_TYPE
                despatchDataEntity.PaymentRef = CLng(ltlReservationReference.Text)
            Else
                despatchDataEntity.PaymentRef = CLng(hplPaymentReference.Text)
                despatchDataEntity.Type = PAYMENT_TYPE
            End If
            Dim talentLogging_LogDespatchCompletion As String = String.Empty
            despatchDataEntity.Source = GlobalConstants.SOURCE
            despatchItems = populateDespatchItemsForFinishOrder(talentLogging_LogDespatchCompletion, despatchDataEntity.PrintSelectAll)

            talDespatch.Settings = settings
            talDespatch.DespatchCollection = despatchItems
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "Start DespatchCompletion: _talentLogging_LogDespatchCompletion=" & talentLogging_LogDespatchCompletion, "DespatchLog")
            talDespatch.DeDespatch = despatchDataEntity
            err = talDespatch.DespatchCompletion()

            If Not err.HasError Then
                If talDespatch.ResultDataSet IsNot Nothing AndAlso talDespatch.ResultDataSet.Tables("ErrorStatus").Rows.Count = 1 AndAlso
                    talDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.SUCCESSFLAG AndAlso talDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode") = "" Then
                    _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End DespatchCompletion: talDespatch.ResultDataSet.Tables(ErrorStatus).Rows(0)(ErrorOccurred)=" & talDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred").ToString, "DespatchLog")
                    Select Case Usage
                        Case UsageType.PURCHASEHISTORY
                            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End DespatchCompletion: Response.Redirect(TEBUtilities.GetSiteHomePage())", "DespatchLog")
                            Response.Redirect(TEBUtilities.GetSiteHomePage())
                        Case UsageType.DESPATCH
                            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End Scan DespatchCompletion: Response.Redirect(~/PagesAgent/Despatch/DespatchProcess.aspx)", "DespatchLog")
                            Session("DespatchProcessSuccess_pyrf") = despatchDataEntity.PaymentRef.ToString
                            Session("DespatchProcessSuccess_token") = despatchDataEntity.TimeStampToken.ToString
                            Session("DespatchProcessSuccess_type") = hdfPaymentReservationType.Value
                            Response.Redirect("~/PagesAgent/Despatch/DespatchProcess.aspx")
                        Case UsageType.PRINT
                            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End Print DespatchCompletion: Response.Redirect(~/PagesAgent/Despatch/DespatchProcess.aspx)", "DespatchLog")
                            Session("PrintSuccessMessage") = _ucr.Content("PrintSuccessMessage", _languageCode, True)
                            Response.Redirect(Request.Url.AbsoluteUri)
                    End Select
                Else
                    If talDespatch.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 AndAlso talDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                        Dim errorCode As String = talDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode")
                        Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(errorCode)
                        ltlErrorDetails.Text = talentErrorMsg.ERROR_MESSAGE
                        highlightTicketInError(talDespatch.DespatchCollection)
                    End If
                    _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End DespatchCompletion: ERROR1 - ltlErrorDetails.Text=" & ltlErrorDetails.Text.ToString, "DespatchLog")
                End If
            Else
                ltlErrorDetails.Text = _ucr.Content("GenericError", _languageCode, True)
                _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End DespatchCompletion: ERROR2 - ltlErrorDetails.Text=" & ltlErrorDetails.Text.ToString, "DespatchLog")
            End If
        End If

        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "FinishOrder", _talentLogging_LogPrefix & "End - error=" & err.HasError.ToString & ", err.ErrorNumber=" & err.ErrorNumber.ToString & ", err.ErrorMessage=" & err.ErrorMessage.ToString, "DespatchLog")
        Return err
    End Function
#End Region

End Class
