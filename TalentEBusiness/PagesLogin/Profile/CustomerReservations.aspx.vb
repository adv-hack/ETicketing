Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Imports System.Linq
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class PagesLogin_Profile_CustomerReservations
    Inherits TalentBase01
#Region "Private Variables"
    Private _languageCode As String
    Private _settings As DESettings = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _wfr As New Talent.Common.WebFormResource
    Private _err As New Talent.Common.ErrorObj
    Private _reservations As DataTable = Nothing
    Private _groupedReservations As New DataTable("GroupedByCustomerReservations")
    Private _errMsg As Talent.Common.TalentErrorMessages
    Private _customerFormat As String = Nothing
    Private _productFormat As String = Nothing
#End Region

#Region "Public Properties"
    Public Property ProductColumnHeading() As String
    Public Property CustomerColumnHeading() As String
    Public Property PriceCodeColumnHeading() As String
    Public Property PriceBandColumnHeading() As String
    Public Property ExpiryDateColumnHeading() As String
    Public Property SeatColumnHeading() As String
    Public Property TicketNumberColumnHeading() As String
    Public Property QuantityColumnHeading() As String
    Public Property SelectColumnHeading() As String
    Public Property AlphaSuffixColumnHeading() As String
    Public Property RowNumberColumnHeading() As String
    Public Property SeatNumberColumnHeading() As String
    Public Property ReservationReferenceColumnHeading() As String
    Public Property DespatchDateColumnHeading() As String
    Public Property ltlNotDespatched As String
#End Region

#Region "Constants"
    Private Enum SearchOption
        OPTION_ZERO = 0
        RESERVATION_REFERENCE_ITEM_NUMBER = 1
        CUSTOMER_NUMBER_ITEM_NUMBER = 2
        PRODUCT_ITEM_NUMBER = 3
        PRICE_CODE_ITEM_NUMBER = 4
        PRICE_BAND_ITEM_NUMBER = 5
        EXPIRY_DATE = 6
    End Enum
    Private Const GENERAL_ERROR_CODE = "GenericReservationError"
    Private Const MIXED_CONTENT As String = "mixed"
#End Region

#Region "Protect Functions"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        'Check if agent has access on Reservations menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessReservations) Or Not AgentProfile.IsAgent Then
            _settings = Talent.eCommerce.Utilities.GetSettingsObject()
            _languageCode = Talent.Common.Utilities.GetDefaultLanguage
            _wfr = New Talent.Common.WebFormResource
            _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            _businessUnit = TalentCache.GetBusinessUnit()
            _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), _wfr.FrontEndConnectionString)
            TDataObjects.Settings = _settings

            With _wfr
                .BusinessUnit = _businessUnit
                .PartnerCode = _partnerCode
                .PageCode = "CustomerReservations.aspx"
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "CustomerReservations.aspx"
            End With
            _customerFormat = _wfr.Attribute("CustomerFormat")
            _productFormat = _wfr.Attribute("ProductFormat")

            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "table-functions.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("table-functions.js", "/Application/Elements/"), False)

            If AgentProfile.IsAgent AndAlso Not Profile.IsAnonymous Then
                PopulateTextAndAttributes()
                If loadReservations() Then
                    If Not IsPostBack Then
                        rptCustomerReservations.DataSource = _reservations
                        rptCustomerReservations.DataBind()
                    End If
                End If
            End If
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If blErrorDetails.Items.Count > 0 Then
            plhErrorMessage.Visible = True
        End If

        If ModuleDefaults.AlertsEnabled AndAlso _reservations Is Nothing Then
            Dim dtUserAlerts As New DataTable
            dtUserAlerts = TDataObjects.AlertSettings.GetUnReadUserAlertsByBUPartnerLoginID(_wfr.BusinessUnit, _wfr.PartnerCode, Profile.UserName, False)
            dtUserAlerts.DefaultView.RowFilter = "[ALERT_ID] = 5"
            Dim restrictionAlert As DataTable = dtUserAlerts.DefaultView.ToTable()
            If restrictionAlert.Rows.Count > 0 Then
                Dim alertTableId As String = restrictionAlert.Rows(0).Item("ID")
                TDataObjects.AlertSettings.TblAlert.DeleteAlertByID(alertTableId)
            End If
        End If

    End Sub

    Protected Sub rptCustomerReservations_ItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rptCustomerReservations.ItemCreated
        If Not IsPostBack Then
            If e.Item.ItemType = ListItemType.Header Then
                Dim chkSelectAll As CheckBox = CType(e.Item.FindControl("chkSelectAll"), CheckBox)
                If Not chkSelectAll Is Nothing Then
                    chkSelectAll.Checked = True
                End If
            End If
        End If
    End Sub
    Protected Sub rptCustomerReservations_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptCustomerReservations.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim reservationItem As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim ltlProductCode As Literal = CType(e.Item.FindControl("ltlProductCode"), Literal)
            Dim ltlQuantity As Literal = CType(e.Item.FindControl("ltlQuantity"), Literal)
            Dim ltlReservedTo As Literal = CType(e.Item.FindControl("ltlReservedTo"), Literal)
            Dim ltlPriceCode As Literal = CType(e.Item.FindControl("ltlPriceCode"), Literal)
            Dim ltlReservedToTime As Literal = CType(e.Item.FindControl("ltlReservedToTime"), Literal)
            Dim ltlSeat As Literal = CType(e.Item.FindControl("ltlSeat"), Literal)
            Dim ltlTicketNo As Literal = CType(e.Item.FindControl("ltlTicketNo"), Literal)
            Dim hdfSeat As HiddenField = CType(e.Item.FindControl("hdfSeat"), HiddenField)
            Dim hdfAlphaSuffix As HiddenField = CType(e.Item.FindControl("hdfAlphaSuffix"), HiddenField)
            Dim hdfRowNumber As HiddenField = CType(e.Item.FindControl("hdfRowNumber"), HiddenField)
            Dim hdfSeatNumber As HiddenField = CType(e.Item.FindControl("hdfSeatNumber"), HiddenField)
            Dim ltlReservationReference As Literal = CType(e.Item.FindControl("ltlReservationReference"), Literal)
            Dim ltlPriceBand As Literal = CType(e.Item.FindControl("ltlPriceBand"), Literal)
            Dim chkSelectedItem As CheckBox = CType(e.Item.FindControl("chkSelectedItem"), CheckBox)
            Dim hdfProductCode As HiddenField = CType(e.Item.FindControl("hdfProductCode"), HiddenField)
            Dim hdfCustomerNumber As HiddenField = CType(e.Item.FindControl("hdfCustomerNumber"), HiddenField)
            Dim hdfLinkedID As HiddenField = CType(e.Item.FindControl("hdfLinkedID"), HiddenField)
            Dim hdfLinkMandatory As HiddenField = CType(e.Item.FindControl("hdfLinkMandatory"), HiddenField)
            chkSelectedItem.Checked = True
            ltlTicketNo.Text = reservationItem("PriceBand").ToString()
            Dim ltlDespatchDate As Literal = CType(e.Item.FindControl("ltlDespatchDate"), Literal)
            ' If a mandatory linked product 
            If Not reservationItem("LinkedID") Is DBNull.Value Then
                hdfLinkedID.Value = reservationItem("LinkedID").ToString()
                hdfLinkMandatory.Value = reservationItem("LinkMandatory").ToString
            End If

            ltlProductCode.Text = String.Format(_productFormat, _
                                                reservationItem("ProductCode").ToString(), _
                                                reservationItem("ProductCodeDescription").ToString())

            ltlReservedTo.Text = String.Format(_customerFormat, _
                                               reservationItem("CustomerNumber").ToString(), _
                                               returnReservedToName(reservationItem("CustomerNumber").ToString()))

            ltlPriceCode.Text = reservationItem("PriceCode").ToString()
            If MIXED_CONTENT.Equals(reservationItem("ReservedUntilDate").ToString()) Then
                ltlReservedToTime.Text = MIXED_CONTENT
            Else
                ltlReservedToTime.Text = Utilities.ISeriesDate(reservationItem("ReservedUntilDate").ToString())
            End If
            If MIXED_CONTENT.Equals(reservationItem("SeatNumber").ToString()) Then
                ltlSeat.Text = MIXED_CONTENT
                hdfSeat.Value = MIXED_CONTENT
            Else
                Dim seat As DESeatDetails = New DESeatDetails
                seat.Stand = reservationItem("StandCode").ToString()
                seat.Area = reservationItem("AreaCode").ToString()
                seat.Row = reservationItem("RowNumber").ToString()
                seat.Seat = reservationItem("SeatNumber").ToString()
                seat.AlphaSuffix = reservationItem("AlphaSuffix").ToString()
                ltlSeat.Text = TEBUtilities.GetFormattedSeatForDisplay(seat.FormattedSeat, reservationItem("ProductType").ToString(), True)
                hdfSeat.Value = reservationItem("StandCode").ToString() & "-" & reservationItem("AreaCode").ToString() & reservationItem("AlphaSuffix").ToString()
            End If
            hdfCustomerNumber.Value = reservationItem("CustomerNumber").ToString()
            hdfProductCode.Value = reservationItem("ProductCode").ToString()
            hdfAlphaSuffix.Value = reservationItem("AlphaSuffix").ToString()
            hdfRowNumber.Value = reservationItem("RowNumber").ToString()
            hdfSeatNumber.Value = reservationItem("SeatNumber").ToString()
            ltlQuantity.Text = reservationItem("Quantity").ToString()
            ltlReservationReference.Text = reservationItem("ReservationReference").ToString()
            ltlPriceBand.Text = reservationItem("PriceBand").ToString()
            ltlTicketNo.Text = reservationItem("TicketNumber").ToString()
            ltlDespatchDate.Text = reservationItem("DespatchDate").ToString()
            If reservationItem("DespatchDate").ToString() = "01/01/01" Then
                ltlDespatchDate.Text = ltlNotDespatched
            End If
        End If

    End Sub

    ''' <summary>
    ''' Adds reservation to basket
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnAddToBasket_Click(sender As Object, e As EventArgs) Handles btnAddToBasket.Click
        Dim deReservations As New DEReservations
        Dim errMsg As TalentErrorMessages
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _wfr.FrontEndConnectionString)

        Dim selectedSearchOption As SearchOption = CType(ddlGroupBy.SelectedItem.Value, SearchOption)
        If selectedSearchOption = SearchOption.OPTION_ZERO Then
            deReservations.Seats = populateSeatsFromRepeater()
        Else
            deReservations.Seats = populateGroupBySeatsFromRepeater(selectedSearchOption)
        End If

        If deReservations.Seats.Count > 0 Then
            Dim reservation As TalentReservations = New TalentReservations
            Dim errorCode As String
            deReservations.SessionID = Profile.Basket.Basket_Header_ID
            deReservations.Source = GlobalConstants.SOURCE
            deReservations.ProductCode = String.Empty
            deReservations.CustomerNumber = Profile.UserName
            reservation.Settings = _settings
            If AgentProfile.IsAgent AndAlso AgentProfile.Name.Length > 0 AndAlso AgentProfile.Type = "2" Then
                deReservations.ByPassPreReqCheck = True
            Else
                deReservations.ByPassPreReqCheck = False
            End If
            deReservations.Agent = AgentProfile.Name
            deReservations.AddToBasket = True
            reservation.DataEntity = deReservations
            _err = reservation.AddReservationToBasketReturnBasket()

            If reservation.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
                errorCode = reservation.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")
                blErrorDetails.Items.Clear()
                blErrorDetails.Items.Add(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
            ElseIf reservation.ResultDataSet.Tables("BasketStatus") IsNot Nothing AndAlso reservation.ResultDataSet.Tables("BasketStatus").Rows.Count > 0 AndAlso reservation.ResultDataSet.Tables("BasketStatus").Rows(0)("ReturnCode").ToString.Trim.Length > 0 Then
                errorCode = reservation.ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode")
                blErrorDetails.Items.Clear()
                blErrorDetails.Items.Add(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
            ElseIf _err.HasError Then
                blErrorDetails.Items.Clear()
                blErrorDetails.Items.Add(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
            Else
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Unreserves reservations
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnUnreserve_Click(sender As Object, e As EventArgs) Handles btnUnreserve.Click
        Dim deReservations As New DEReservations
        Dim errMsg As TalentErrorMessages
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _wfr.FrontEndConnectionString)
        Dim selectedSearchOption As SearchOption = CType(ddlGroupBy.SelectedItem.Value, SearchOption)
        If selectedSearchOption = SearchOption.OPTION_ZERO Then
            deReservations.Seats = populateSeatsFromRepeater()
        Else
            deReservations.Seats = populateGroupBySeatsFromRepeater(selectedSearchOption)
        End If
        If deReservations.Seats.Count > 0 Then
            Dim reservation As TalentReservations = New TalentReservations
            Dim reservationReference As String = String.Empty
            deReservations.Source = GlobalConstants.SOURCE
            deReservations.Comment = txtCommentConfirm.Text
            deReservations.Agent = AgentProfile.Name
            deReservations.SessionID = Profile.Basket.Basket_Header_ID
            deReservations.Source = GlobalConstants.SOURCE
            deReservations.ProductCode = String.Empty
            deReservations.CustomerNumber = Profile.UserName
            deReservations.UnreserveAll = False
            deReservations.NumberOfSeatsReserved = Profile.Basket.BasketSummary.TotalItemsTicketing
            reservation.Settings = _settings
            If AgentProfile.IsAgent AndAlso AgentProfile.Name.Length > 0 AndAlso AgentProfile.Type = "2" Then
                deReservations.ByPassPreReqCheck = True
            Else
                deReservations.ByPassPreReqCheck = False
            End If

            reservation.Settings = _settings
            reservation.DataEntity = deReservations
            _err = reservation.UnreserveSeparateBasketItems()

            If Not _err.HasError AndAlso _
                 reservation.ResultDataSet IsNot Nothing AndAlso _
                reservation.ResultDataSet.Tables.Count() > 0 AndAlso _
                reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count = 0 Then
                'plhSuccessMessage.Visible = True
                'ltlSuccessMessage.Text = "Unreserve successful!"
            Else
                If reservation.ResultDataSet IsNot Nothing AndAlso reservation.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso _
                reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
                    Dim errorCode As String = reservation.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")
                    blErrorDetails.Items.Clear()
                    blErrorDetails.Items.Add(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
                Else
                    blErrorDetails.Items.Clear()
                    blErrorDetails.Items.Add(_wfr.Content("GeneraliSeriesErrorText", _languageCode, True))
                End If
            End If
            If blErrorDetails.Items.Count = 0 Then
                Response.Redirect("~/PagesLogin/Profile/CustomerReservations.aspx")
            End If
        End If
    End Sub

    Protected Sub ddlGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlGroupBy.SelectedIndexChanged
        If Not groupBy(CType(ddlGroupBy.SelectedItem.Value, SearchOption)) Then
            blErrorDetails.Items.Add(_wfr.Content("groupFailureErrorText", _languageCode, True))
        End If
    End Sub
    ''' <summary>
    ''' Set visibility of columns based on page attribute
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub CanDisplayThisColumn(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Select Case sender.id
                Case "plhReservationReference"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowReservationReferenceColumn"))
                Case "plhExpiryDate"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowExpiryDateColumn"))
                Case "plhCustomer"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowCustomerColumn"))
                Case "plhProduct"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowProductColumn"))
                Case "plhSeat"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowSeatColumn"))
                Case "plhTicketNumber"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowTicketNumberColumn"))
                Case "plhPriceCode"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowPriceCodeColumn"))
                Case "plhQuantity"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowQuantityColumn"))
                Case "plhchkSelectAll", "plhchkSelectedItem"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowSelectColumn"))
                Case "plhDespatchDate"
                    sender.Visible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("ShowDespatchDateColumn"))


            End Select
        Catch ex As Exception
            sender.Visible = False
        End Try
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Populates text and attributes, as well as setup list items for ddlGroup
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateTextAndAttributes()
        ProductColumnHeading = _wfr.Content("ProductColumnHeadingText", _languageCode, True)
        CustomerColumnHeading = _wfr.Content("CustomerColumnHeadingText", _languageCode, True)
        PriceCodeColumnHeading = _wfr.Content("PriceCodeColumnHeadingText", _languageCode, True)
        PriceBandColumnHeading = _wfr.Content("PriceBandColumnHeadingText", _languageCode, True)
        ExpiryDateColumnHeading = _wfr.Content("ExpiryDateColumnHeadingText", _languageCode, True)
        SeatColumnHeading = _wfr.Content("SeatColumnHeadingText", _languageCode, True)
        QuantityColumnHeading = _wfr.Content("QuantityColumnHeadingText", _languageCode, True)
        SelectColumnHeading = _wfr.Content("SelectColumnHeadingText", _languageCode, True)
        AlphaSuffixColumnHeading = _wfr.Content("AlphaSuffixColumnHeadingText", _languageCode, True)
        RowNumberColumnHeading = _wfr.Content("RowNumberColumnHeadingText", _languageCode, True)
        SeatNumberColumnHeading = _wfr.Content("SeatNumberColumnHeadingText", _languageCode, True)
        ReservationReferenceColumnHeading = _wfr.Content("ReservationReferenceColumnHeadingText", _languageCode, True)
        DespatchDateColumnHeading = _wfr.Content("DespatchDateColumnHeadingText", _languageCode, True)
        TicketNumberColumnHeading = _wfr.Content("TicketNumberColumnHeadingText", _languageCode, True)
        ltlNotDespatched = _wfr.Content("ltlNotDespatched", _languageCode, True)

        lblGroupBy.Text = _wfr.Content("lblGroupByText", _languageCode, True)
        btnAddToBasket.Text = _wfr.Content("AddToBasketButtonText", _languageCode, True)
        btnUnreserve.Text = _wfr.Content("UnreserveButtonText", _languageCode, True)
        hplCommentConfirm.Text = _wfr.Content("CommentConfirmButtonText", _languageCode, True)
        lblAddComment.Text = _wfr.Content("CommentButtonText", _languageCode, True)
        hlkCloseCommentBox.Text = _wfr.Content("CloseCommentBoxText", _languageCode, True)

        Dim groupByOptionZero As New ListItem
        groupByOptionZero.Text = _wfr.Content("GroupByOptionZeroText", _languageCode, True)
        groupByOptionZero.Value = SearchOption.OPTION_ZERO
        ddlGroupBy.Items.Add(groupByOptionZero)

        Dim groupByReservationReference As New ListItem
        groupByReservationReference.Text = ReservationReferenceColumnHeading
        groupByReservationReference.Value = SearchOption.RESERVATION_REFERENCE_ITEM_NUMBER
        ddlGroupBy.Items.Add(groupByReservationReference)

        Dim groupByCustomerName As New ListItem
        groupByCustomerName.Text = CustomerColumnHeading
        groupByCustomerName.Value = SearchOption.CUSTOMER_NUMBER_ITEM_NUMBER
        ddlGroupBy.Items.Add(groupByCustomerName)

        Dim groupByProductCode As New ListItem
        groupByProductCode.Text = ProductColumnHeading
        groupByProductCode.Value = SearchOption.PRODUCT_ITEM_NUMBER
        ddlGroupBy.Items.Add(groupByProductCode)

        Dim groupByPriceCode As New ListItem
        groupByPriceCode.Text = PriceCodeColumnHeading
        groupByPriceCode.Value = SearchOption.PRICE_CODE_ITEM_NUMBER
        ddlGroupBy.Items.Add(groupByPriceCode)

        Dim groupByPriceBand As New ListItem
        groupByPriceBand.Text = PriceBandColumnHeading
        groupByPriceBand.Value = SearchOption.PRICE_BAND_ITEM_NUMBER
        ddlGroupBy.Items.Add(groupByPriceBand)

        Dim groupByExpiryDate As New ListItem
        groupByExpiryDate.Text = ExpiryDateColumnHeading
        groupByExpiryDate.Value = SearchOption.EXPIRY_DATE
        ddlGroupBy.Items.Add(groupByExpiryDate)
    End Sub

    ''' <summary>
    ''' Retrieves reservations and handles 0 return results of reservations
    ''' </summary>
    ''' <returns>Boolean which indicates more than 0 retrieval of reservations</returns>
    ''' <remarks></remarks>
    Private Function loadReservations() As Boolean
        Dim hasReservations As Boolean = False
        'If returnFullReservations() AndAlso AgentProfile.AgentPermissions.CanReserveHospitalityBookings Then
        If returnFullReservations() Then
            hasReservations = True
            btnUnreserve.Visible = True
        Else
            blErrorDetails.Items.Add(_wfr.Content("NoReservationsErrorText", _languageCode, True))
            ddlGroupBy.Visible = False
            btnUnreserve.Visible = False
            btnAddToBasket.Visible = False
            rptCustomerReservations.Visible = False
            lblGroupBy.Visible = False
            hplCommentConfirm.Visible = False
        End If
        Return hasReservations
    End Function

    ''' <summary>
    ''' Performs GroupBy functionality
    ''' </summary>
    ''' <param name="searchOption"></param>
    ''' <returns>Boolean which indicates more than 0 results of groupBy query</returns>
    ''' <remarks></remarks>
    Private Function groupBy(ByVal searchOption As SearchOption) As Boolean
        Dim success As Boolean = False
        With _groupedReservations.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductCodeDescription", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("StandCode", GetType(String))
            .Add("AreaCode", GetType(String))
            .Add("RowNumber", GetType(String))
            .Add("SeatNumber", GetType(String))
            .Add("AlphaSuffix", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceCodeDescription", GetType(String))
            .Add("ReservationReference", GetType(String))
            .Add("ReservedUntilDate", GetType(String))
            .Add("ReservedUntilTime", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("PriceBandDescription", GetType(String))
            .Add("TicketNumber", GetType(String))
            .Add("ReservationCode", GetType(String))
            .Add("Quantity", GetType(Integer))
            .Add("LinkedID", GetType(Long))
            .Add("LinkMandatory", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("DespatchDate", GetType(String))
        End With
        Select Case searchOption
            Case searchOption.OPTION_ZERO
                _groupedReservations = _reservations
            Case searchOption.PRODUCT_ITEM_NUMBER
                Dim queryResults = From row In _reservations
                     Group row By ProductCode = row.Field(Of String)("ProductCode") Into ProductCodeGroup = Group
                     Select New With {Key ProductCode,
                         .Quantity = ProductCodeGroup.Sum(Function(r) r.Field(Of Int32)("Quantity"))
                     }


                For Each r In queryResults
                    populateGroupByTable(r.ProductCode, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.Quantity)
                Next
                completeGroupBy("ProductCode")
            Case searchOption.CUSTOMER_NUMBER_ITEM_NUMBER
                Dim queryResults = From row In _reservations
                    Group row By CustomerNumber = row.Field(Of String)("CustomerNumber") Into CustomerNumberGroup = Group
                    Select New With {Key CustomerNumber,
                        .Quantity = CustomerNumberGroup.Sum(Function(r) r.Field(Of Int32)("Quantity"))
                    }

                For Each r In queryResults
                    populateGroupByTable(MIXED_CONTENT, MIXED_CONTENT, r.CustomerNumber, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.Quantity)
                Next
                completeGroupBy("CustomerNumber")
            Case searchOption.PRICE_CODE_ITEM_NUMBER
                Dim queryResults = From row In _reservations
                    Group row By PriceCode = row.Field(Of String)("PriceCode") Into PriceCodeGroup = Group
                    Select New With {Key PriceCode,
                                    .Quantity = PriceCodeGroup.Sum(Function(r) r.Field(Of Int32)("Quantity"))
                    }
                For Each r In queryResults

                    populateGroupByTable(MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.PriceCode, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.Quantity)
                Next
                completeGroupBy("PriceCode")
            Case searchOption.RESERVATION_REFERENCE_ITEM_NUMBER
                Dim queryResults = From row In _reservations
                    Group row By ReservationReference = row.Field(Of String)("ReservationReference") Into ReservationReferenceGroup = Group
                    Select New With {Key ReservationReference,
                        .Quantity = ReservationReferenceGroup.Sum(Function(r) r.Field(Of Int32)("Quantity"))
                    }

                For Each r In queryResults
                    populateGroupByTable(MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.ReservationReference, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.Quantity)
                Next
                completeGroupBy("ReservationReference")
            Case searchOption.PRICE_BAND_ITEM_NUMBER
                Dim queryResults = From row In _reservations
                    Group row By PriceBand = row.Field(Of String)("PriceBand") Into PriceBandGroup = Group
                    Select New With {Key PriceBand,
                        .Quantity = PriceBandGroup.Sum(Function(r) r.Field(Of Int32)("Quantity"))
                    }

                For Each r In queryResults
                    populateGroupByTable(MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         r.PriceBand, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.Quantity)
                Next
                completeGroupBy("PriceBand")

            Case searchOption.EXPIRY_DATE
                Dim queryResults = From row In _reservations
                    Group row By ExpiryDate = row.Field(Of String)("ReservedUntilDate") Into PriceBandGroup = Group
                    Select New With {Key ExpiryDate,
                        .Quantity = PriceBandGroup.Sum(Function(r) r.Field(Of Int32)("Quantity"))
                    }

                For Each r In queryResults
                    populateGroupByTable(MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, r.ExpiryDate, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, _
                                         MIXED_CONTENT, MIXED_CONTENT, MIXED_CONTENT, r.Quantity)
                Next

                completeGroupBy("ReservedUntilDate")
        End Select
        If _groupedReservations.Rows.Count > 0 Then
            rptCustomerReservations.DataSource = _groupedReservations
            rptCustomerReservations.DataBind()
            success = True
        End If
        Return success
    End Function
    ''' <summary>
    ''' Compares the _reservations  and _groupedreservations data tables and assigns non unique values as MIXED_CONTENT
    ''' and unique values as true values.
    ''' </summary>
    ''' <param name="groupBycolumnName"></param>
    ''' <remarks></remarks>
    Private Sub completeGroupBy(ByVal groupBycolumnName As String)
        For Each groupedReservationColumn As DataColumn In _groupedReservations.Columns
            If Not groupedReservationColumn.ColumnName.Equals(groupBycolumnName) AndAlso Not String.IsNullOrEmpty(groupedReservationColumn.ColumnName) _
                AndAlso Not groupedReservationColumn.ColumnName.Equals("Select") AndAlso Not groupedReservationColumn.ColumnName.Equals("Quantity") _
                AndAlso Not groupedReservationColumn.ColumnName.Equals("LinkedID") Then
                Dim rows() As DataRow
                For Each groupedReservationRow As DataRow In _groupedReservations.Rows
                    rows = _reservations.Select(groupBycolumnName & " = '" & groupedReservationRow(groupBycolumnName) & "'")
                    If rows.Count = 1 Then
                        groupedReservationRow.Item(groupedReservationColumn.ColumnName) = rows(0).Item(groupedReservationColumn.ColumnName)
                    ElseIf rows.Count > 1 Then
                        Dim firstRow As String = rows(0).Item(groupedReservationColumn.ColumnName)
                        Dim isMixed As Boolean = False
                        For index As Integer = 1 To rows.Count - 1
                            If Not rows(index).Item(groupedReservationColumn.ColumnName).Equals(firstRow) Then
                                isMixed = True
                            End If
                        Next
                        If isMixed Then
                            groupedReservationRow.Item(groupedReservationColumn.ColumnName) = MIXED_CONTENT
                        Else
                            groupedReservationRow.Item(groupedReservationColumn.ColumnName) = firstRow
                        End If
                    Else
                        groupedReservationRow.Item(groupedReservationColumn.ColumnName) = MIXED_CONTENT
                    End If
                Next
            End If
        Next
    End Sub
    ''' <summary>
    ''' Adds queried groupBy data to DataTable
    ''' </summary>
    ''' <param name="productCode"></param>
    ''' <param name="productDescription"></param>
    ''' <param name="customer"></param>
    ''' <param name="reservationReference"></param>
    ''' <param name="reservationCode"></param>
    ''' <param name="priceband"></param>
    ''' <param name="priceBandDescription"></param>
    ''' <param name="expiryDate"></param>
    ''' <param name="expiryTime"></param>
    ''' <param name="seat"></param>
    ''' <param name="standCode"></param>
    ''' <param name="areaCode"></param>
    ''' <param name="TicketNumber"></param>
    ''' <param name="priceCode"></param>
    ''' <param name="priceCodeDescription"></param>
    ''' <param name="rowNumber"></param>
    ''' <param name="alphaSuffix"></param>
    ''' <param name="quantity"></param>
    ''' <remarks></remarks>
    Private Sub populateGroupByTable(ByVal productCode As String, ByVal productDescription As String, ByVal customer As String, _
                                     ByVal reservationReference As String, ByVal reservationCode As String, ByVal priceband As String, _
                                     ByVal priceBandDescription As String, ByVal expiryDate As String, ByVal expiryTime As String, _
                                     ByVal seat As String, ByVal standCode As String, ByVal areaCode As String, _
                                     ByVal TicketNumber As String, ByVal priceCode As String, ByVal priceCodeDescription As String, _
                                     ByVal rowNumber As String, ByVal alphaSuffix As String, despatchDate As String, ByVal quantity As Integer)
        Dim row As DataRow = Nothing
        row = _groupedReservations.NewRow
        row("CustomerNumber") = customer
        row("PriceCode") = priceCode
        row("PriceCodeDescription") = priceCodeDescription
        row("PriceBand") = priceband
        row("PriceBandDescription") = priceBandDescription
        row("ProductCode") = productCode
        row("ProductCodeDescription") = productDescription
        row("StandCode") = standCode
        row("AreaCode") = areaCode
        row("RowNumber") = rowNumber
        row("SeatNumber") = seat
        row("AlphaSuffix") = alphaSuffix
        row("TicketNumber") = TicketNumber
        row("ReservationCode") = reservationCode
        row("ReservationReference") = reservationReference
        row("ReservedUntilDate") = expiryDate
        row("ReservedUntilTime") = expiryTime
        row("DespatchDate") = despatchDate
        row("Quantity") = quantity
        _groupedReservations.Rows.Add(row)
    End Sub

    ''' <summary>
    ''' Returns all reservations and populates class level DataTable
    ''' </summary>
    ''' <returns>Boolean signaling successful call to WS018R</returns>
    ''' <remarks></remarks>
    Private Function returnFullReservations() As Boolean
        Dim deReservations As New DEReservations
        Dim reservations As New TalentReservations
        Dim success As Boolean = False
        deReservations.Agent = AgentProfile.Name
        deReservations.CustomerNumber = HttpContext.Current.Profile.UserName
        deReservations.Source = GlobalConstants.SOURCE
        deReservations.RetrieveCustomerFF = ModuleDefaults.FriendsAndFamily
        deReservations.SessionID = Profile.Basket.Basket_Header_ID
        reservations.Settings = _settings
        reservations.DataEntity = deReservations
        If AgentProfile.IsAgent AndAlso AgentProfile.Name.Length > 0 AndAlso AgentProfile.Type = "2" Then
            deReservations.ByPassPreReqCheck = True
        Else
            deReservations.ByPassPreReqCheck = False
        End If
        deReservations.Seats = New List(Of DESeatDetails)
        deReservations.ReturnSeatDetails = True
        deReservations.ProductCode = String.Empty
        Dim outputResult As DataTable = Nothing
        _err = reservations.RetrieveCustomerReservations()

        If Not _err.HasError AndAlso reservations.ResultDataSet IsNot Nothing AndAlso reservations.ResultDataSet.Tables.Count() > 0 AndAlso reservations.ResultDataSet.Tables("CustomerReservations").Rows.Count > 0 Then
            _reservations = reservations.ResultDataSet.Tables("CustomerReservations")
            Dim dvReservations As New DataView(_reservations)
            dvReservations.Sort = "ReservedUntilDate DESC"
            _reservations = dvReservations.ToTable
            success = True
        End If
        Return success
    End Function

    ''' <summary>
    ''' Populates list of seat details from the repeater used for non-GroupBy Unreserve and AddToBasket functionality
    ''' </summary>
    ''' <returns>List of seat details containing data from the repeater</returns>
    ''' <remarks></remarks>
    Private Function populateSeatsFromRepeater() As List(Of DESeatDetails)
        Dim Seats = New List(Of DESeatDetails)
        Dim LinkedIDs = New List(Of Long)
        For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
            Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
            If chkSelectedItem.Checked Then
                Dim ltlPriceCode As Literal = CType(customerReservationItem.FindControl("ltlPriceCode"), Literal)
                Dim ltlQuantity As Literal = CType(customerReservationItem.FindControl("ltlQuantity"), Literal)
                Dim hdfSeat As HiddenField = CType(customerReservationItem.FindControl("hdfSeat"), HiddenField)
                Dim hdfAlphaSuffix As HiddenField = CType(customerReservationItem.FindControl("hdfAlphaSuffix"), HiddenField)
                Dim hdfRowNumber As HiddenField = CType(customerReservationItem.FindControl("hdfRowNumber"), HiddenField)
                Dim hdfSeatNumber As HiddenField = CType(customerReservationItem.FindControl("hdfSeatNumber"), HiddenField)
                Dim hdfProductCode As HiddenField = CType(customerReservationItem.FindControl("hdfProductCode"), HiddenField)
                Dim hdfCustomerNumber As HiddenField = CType(customerReservationItem.FindControl("hdfCustomerNumber"), HiddenField)
                Dim hdfLinkedID As HiddenField = CType(customerReservationItem.FindControl("hdfLinkedID"), HiddenField)
                Dim seat As String()
                seat = hdfSeat.Value.Split("-")

                Dim seatDetails As DESeatDetails = New DESeatDetails
                seatDetails.AlphaSuffix = hdfAlphaSuffix.Value
                seatDetails.Stand = seat(0).ToString
                seatDetails.Area = seat(1).ToString
                seatDetails.Row = hdfRowNumber.Value
                seatDetails.ProductCode = hdfProductCode.Value
                seatDetails.Quantity = ltlQuantity.Text
                seatDetails.CustomerNumber = hdfCustomerNumber.Value
                seatDetails.PriceCode = ltlPriceCode.Text
                seatDetails.Seat = hdfSeatNumber.Value

                Seats.Add(seatDetails)
                If hdfLinkedID.Value <> 0 Then LinkedIDs.Add(hdfLinkedID.Value)
            End If
        Next
        populateSeatsForMandatoryLinkedProducts(Seats, LinkedIDs)

        Return Seats
    End Function

    ''' <summary>
    ''' Populates list of full seat details from grouped by data
    ''' </summary>
    ''' <param name="searchOption"></param>
    ''' <returns>List of full seat details</returns>
    ''' <remarks></remarks>
    Private Function populateGroupBySeatsFromRepeater(ByVal searchOption As SearchOption) As List(Of DESeatDetails)
        Dim Seats = New List(Of DESeatDetails)
        Dim LinkedIDs = New List(Of Long)
        Select Case searchOption
            Case searchOption.PRODUCT_ITEM_NUMBER
                For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
                    For Each reservation As DataRow In _reservations.Rows
                        Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
                        Dim hdfProductCode As HiddenField = CType(customerReservationItem.FindControl("hdfProductCode"), HiddenField)
                        If chkSelectedItem.Checked AndAlso hdfProductCode.Value = reservation("ProductCode").ToString Then
                            Seats.Add(getSeperateSeatDetailsFromReservations(reservation))
                            If reservation("LinkedID") <> 0 Then LinkedIDs.Add(reservation("LinkedID"))
                        End If
                    Next
                Next
            Case searchOption.CUSTOMER_NUMBER_ITEM_NUMBER
                For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
                    For Each reservation As DataRow In _reservations.Rows
                        Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
                        Dim hdfCustomerNumber As HiddenField = CType(customerReservationItem.FindControl("hdfCustomerNumber"), HiddenField)
                        If chkSelectedItem.Checked AndAlso hdfCustomerNumber.Value = reservation("CustomerNumber").ToString Then
                            Seats.Add(getSeperateSeatDetailsFromReservations(reservation))
                            If reservation("LinkedID") <> 0 Then LinkedIDs.Add(reservation("LinkedID"))
                        End If
                    Next
                Next
            Case searchOption.PRICE_BAND_ITEM_NUMBER
                For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
                    For Each reservation As DataRow In _reservations.Rows
                        Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
                        Dim ltlPriceBand As Literal = CType(customerReservationItem.FindControl("ltlPriceBand"), Literal)
                        If chkSelectedItem.Checked AndAlso ltlPriceBand.Text = reservation("PriceBand").ToString Then
                            Seats.Add(getSeperateSeatDetailsFromReservations(reservation))
                            If reservation("LinkedID") <> 0 Then LinkedIDs.Add(reservation("LinkedID"))
                        End If
                    Next
                Next
            Case searchOption.RESERVATION_REFERENCE_ITEM_NUMBER
                For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
                    For Each reservation As DataRow In _reservations.Rows
                        Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
                        Dim ltlReservationReference As Literal = CType(customerReservationItem.FindControl("ltlReservationReference"), Literal)
                        If chkSelectedItem.Checked AndAlso ltlReservationReference.Text = reservation("ReservationReference").ToString Then
                            Seats.Add(getSeperateSeatDetailsFromReservations(reservation))
                            If reservation("LinkedID") <> 0 Then LinkedIDs.Add(reservation("LinkedID"))
                        End If
                    Next
                Next
            Case searchOption.PRICE_CODE_ITEM_NUMBER
                For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
                    For Each reservation As DataRow In _reservations.Rows
                        Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
                        Dim ltlPriceCode As Label = CType(customerReservationItem.FindControl("ltlPriceCode"), Label)
                        If chkSelectedItem.Checked AndAlso ltlPriceCode.Text = reservation("PriceCode").ToString Then
                            Seats.Add(getSeperateSeatDetailsFromReservations(reservation))
                            If reservation("LinkedID") <> 0 Then LinkedIDs.Add(reservation("LinkedID"))
                        End If
                    Next
                Next
            Case searchOption.EXPIRY_DATE
                For Each customerReservationItem As RepeaterItem In rptCustomerReservations.Items
                    For Each reservation As DataRow In _reservations.Rows
                        Dim chkSelectedItem As CheckBox = CType(customerReservationItem.FindControl("chkSelectedItem"), CheckBox)
                        Dim ltlReservedToTime As Literal = CType(customerReservationItem.FindControl("ltlReservedToTime"), Literal)
                        If chkSelectedItem.Checked AndAlso Utilities.DateToIseriesFormat(ltlReservedToTime.Text) = reservation("ReservedUntilDate").ToString Then
                            Seats.Add(getSeperateSeatDetailsFromReservations(reservation))
                            If reservation("LinkedID") <> 0 Then LinkedIDs.Add(reservation("LinkedID"))
                        End If
                    Next
                Next
        End Select

        If LinkedIDs.Count > 0 Then populateSeatsForMandatoryLinkedProducts(Seats, LinkedIDs)

        Return Seats
    End Function

    ''' <summary>
    ''' Adds any mandatory linked products to the Seats collection for those products that have been selected for processing.
    ''' Ensure all mandatory items (as well as master ityem) for any selected linked products are included
    ''' </summary>
    ''' <param name="Seats"></param>
    ''' <param name="LinkedIDs"></param>
    ''' <remarks></remarks>
    Private Sub populateSeatsForMandatoryLinkedProducts(ByRef Seats As List(Of DESeatDetails), ByVal LinkedIDs As List(Of Long))
        Dim dvReservations As New DataView(_reservations)
        dvReservations.RowFilter = "LinkedID <> 0"
        If dvReservations.Count > 0 Then
            For Each dr In dvReservations.ToTable.Rows
                If dr("LinkMandatory") <> " " Then
                    If LinkedIDs.Contains(dr("LinkedID")) Then
                        Dim seatDetails As DESeatDetails = getSeperateSeatDetailsFromReservations(dr)
                        If Not Seats.Contains(seatDetails) Then
                            Seats.Add(seatDetails)
                        End If
                    End If
                End If
            Next
        End If
    End Sub


    ''' <summary>
    ''' Populates seat details for a seperate seat into DESeatDetails data entity
    ''' </summary>
    ''' <param name="customerReservationItem"></param>
    ''' <returns>Seat details data entity</returns>
    ''' <remarks></remarks>
    Private Function getSeperateSeatDetailsFromReservations(ByVal customerReservationItem As DataRow) As DESeatDetails
        Dim seatDetails As DESeatDetails = New DESeatDetails
        seatDetails.Stand = customerReservationItem("StandCode").ToString
        seatDetails.Area = customerReservationItem("AreaCode").ToString
        seatDetails.AlphaSuffix = customerReservationItem("AlphaSuffix").ToString
        seatDetails.Row = customerReservationItem("RowNumber").ToString
        seatDetails.ProductCode = customerReservationItem("ProductCode").ToString
        seatDetails.CustomerNumber = customerReservationItem("CustomerNumber").ToString
        seatDetails.PriceCode = customerReservationItem("PriceCode").ToString
        seatDetails.Seat = customerReservationItem("SeatNumber").ToString
        Return seatDetails
    End Function

    Private Function returnReservedToName(ByVal customerNumber As String) As String
        customerNumber = Utilities.PadLeadingZeros(customerNumber, 12)
        If customerNumber = Profile.User.Details.LoginID Then
            Return Profile.User.Details.Full_Name
        End If
        Dim customer As New DECustomer
        customer.UserName = Profile.UserName
        customer.CustomerNumber = Profile.User.Details.LoginID
        customer.Source = "W"
        Dim talentCustomer As New TalentCustomer
        Dim deCustV11 As New DECustomerV11
        deCustV11.DECustomersV1.Add(customer)
        talentCustomer.DeV11 = deCustV11
        talentCustomer.Settings = _settings
        talentCustomer.Settings.Cacheing = True
        _err = talentCustomer.CustomerAssociations

        Dim dtFriendsAndFamily As DataTable = talentCustomer.ResultDataSet.Tables("FriendsAndFamily")
        For Each friendOrFamily As DataRow In dtFriendsAndFamily.Rows
            If friendOrFamily.Item("AssociatedCustomerNumber") = customerNumber Then
                Return friendOrFamily.Item("Forename") & " " & friendOrFamily.Item("Surname")
                Exit For
            End If
        Next
        Return String.Empty
    End Function

#End Region
End Class
