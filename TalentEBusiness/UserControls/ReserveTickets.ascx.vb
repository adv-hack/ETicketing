Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Partial Class UserControls_ReserveTickets
    Inherits ControlBase

#Region "Private Variables"
    Private _languageCode As String
    Private _settings As DESettings = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _ucr As New UserControlResource
    Private _err As New ErrorObj
    Private _reservations As DataTable = Nothing
#End Region

#Region "Public Properties"
    ''' <summary>
    ''' BasketDetails usercontrol object for date error
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property StandardBasketDetails() As UserControls_BasketDetails
        Get
            Dim basketDetailsControl As New UserControls_BasketDetails
            basketDetailsControl = CType(Talent.eCommerce.Utilities.FindWebControl("BasketDetails1", Me.Page.Controls), UserControls_BasketDetails)
            Return basketDetailsControl
        End Get
    End Property

    ''' <summary>
    ''' HospitalityBooking literal control for date error
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property HospitalityBookingError() As Literal
        Get
            Dim bookingErrorLiteralControl As New Literal
            bookingErrorLiteralControl = CType(Talent.eCommerce.Utilities.FindWebControl("ltlBookingErrors", Me.Page.Controls), Literal)
            Return bookingErrorLiteralControl
        End Get
    End Property

    ''' <summary>
    ''' Parent page of this user control
    ''' </summary>
    ''' <returns></returns>
    Public Property ParentPage() As String = String.Empty

    ''' <summary>
    ''' Hospitality booking reference
    ''' </summary>
    ''' <returns></returns>
    Public Property CallID() As String = String.Empty

    ''' <summary>
    ''' Cancel button text
    ''' </summary>
    ''' <returns></returns>
    Public Property CancelButtonText As String
#End Region

#Region "Constants"
    Const GENERAL_ERROR_CODE = "GenericReservationError"
    Private Const KEYCODE As String = "ReserveTickets.ascx"
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _settings = Talent.eCommerce.Utilities.GetSettingsObject()
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _ucr = New UserControlResource
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _businessUnit = TalentCache.GetBusinessUnit()
        TDataObjects.Settings = _settings
        With _ucr
            .BusinessUnit = _businessUnit
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .KeyCode = KEYCODE
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ParentPage = Page.Title.Replace(" ", String.Empty).ToUpper()
        If ParentPage = "HOSPITALITYBOOKING" Then
            plhSaleOrReturn.Visible = False
        End If
        plhReserveTickets.Visible = True
        lblAgentName.Text = AgentProfile.Name
        SetTextValues()
    End Sub

    ''' <summary>
    ''' Reserves basket items and handles return results
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnReserve_Click(sender As Object, e As EventArgs) Handles btnReserve.Click
        Dim reservation As TalentReservations = New TalentReservations
        Dim DeReservation As New DEReservations
        Dim reservationDate As New Date
        Dim reservationTime As New Date
        Dim reservationTimeDate As New Date
        Dim reservationReference As String = String.Empty
        DeReservation.SessionID = Profile.Basket.Basket_Header_ID
        DeReservation.Agent = lblAgentName.Text
        DeReservation.Comment = txtComment.Text
        DeReservation.SaleOrReturn = chkSaleOrReturn.Checked

        DeReservation.Source = GlobalConstants.SOURCE
        DeReservation.CustomerNumber = Profile.UserName
        DeReservation.NumberOfSeatsReserved = Profile.Basket.BasketSummary.TotalItemsTicketing
        DeReservation.CallId = IIf(String.IsNullOrEmpty(CallID), "0", CallID)
        reservation.DataEntity = DeReservation
        reservation.Settings = _settings
        Dim dateTimeString() As String
        Dim errMsg As TalentErrorMessages
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner(), _ucr.FrontEndConnectionString)

        ' Retrieve the number of (non-reservation) basket errors
        Dim hdfNoneReservationErrorCount As New HiddenField
        hdfNoneReservationErrorCount = CType(Talent.eCommerce.Utilities.FindWebControl("hdfNoneReservationErrorCount", Me.Page.Controls), HiddenField)

        Dim errorGenerated As Boolean = False

        ' Retail or Season Ticket reservations not allowed
        If Profile.Basket.BasketSummary.MerchandiseTotalItems > 0 Or hasBasketInvalidReservationItems() Then
            If ParentPage = "HOSPITALITYBOOKING" Then
                HospitalityBookingError.Text = String.Empty
                HospitalityBookingError.Text = _ucr.Content("InvalidReservationItemsErrorMessage", _languageCode, True)
            Else
                StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("InvalidReservationItemsErrorMessage", _languageCode, True))
                StandardBasketDetails.Error_List.Items.Add(_ucr.Content("InvalidReservationItemsErrorMessage", _languageCode, True))
            End If
            errorGenerated = True
        End If

        If hdfNoneReservationErrorCount.Value = 0 Then
            If Not chkSaleOrReturn.Checked Then
                If Date.TryParse(txtExpiryDate.Text, reservationTimeDate) Then
                    dateTimeString = txtExpiryDate.Text.Split(" ")
                    If Date.TryParse(dateTimeString(0), reservationDate) Then
                        DeReservation.ExpiryDate = Utilities.DateToIseries8Format(reservationDate)
                    Else
                        If ParentPage = "HOSPITALITYBOOKING" Then
                            HospitalityBookingError.Text = String.Empty
                            HospitalityBookingError.Text = _ucr.Content("DateTimeFormatErrorText", _languageCode, True)
                        Else
                            StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("DateTimeFormatErrorText", _languageCode, True))
                            StandardBasketDetails.Error_List.Items.Add(_ucr.Content("DateTimeFormatErrorText", _languageCode, True))
                        End If
                        errorGenerated = True
                    End If
                    If dateTimeString.Length > 1 AndAlso Date.TryParse(dateTimeString(1), reservationTime) Then
                        DeReservation.ExpiryTime = reservationTime.TimeOfDay().ToString("t")
                    Else
                        If ParentPage = "HOSPITALITYBOOKING" Then
                            HospitalityBookingError.Text = String.Empty
                            HospitalityBookingError.Text = _ucr.Content("DateTimeFormatErrorText", _languageCode, True)
                        Else
                            StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("DateTimeFormatErrorText", _languageCode, True))
                            StandardBasketDetails.Error_List.Items.Add(_ucr.Content("DateTimeFormatErrorText", _languageCode, True))
                        End If
                        errorGenerated = True
                    End If
                Else
                    If ParentPage = "HOSPITALITYBOOKING" Then
                        HospitalityBookingError.Text = String.Empty
                        HospitalityBookingError.Text = _ucr.Content("DateTimeFormatErrorText", _languageCode, True)
                    Else
                        StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("DateTimeFormatErrorText", _languageCode, True))
                        StandardBasketDetails.Error_List.Items.Add(_ucr.Content("DateTimeFormatErrorText", _languageCode, True))
                    End If
                    errorGenerated = True
                End If
            Else
                DeReservation.ExpiryDate = Utilities.DateToIseries8Format(CType("01/01/2099", Date))
                DeReservation.ExpiryTime = CType("23:59:00", Date).TimeOfDay().ToString("t")
            End If
            If AgentProfile.IsAgent AndAlso AgentProfile.Name.Length > 0 AndAlso AgentProfile.Type = "2" Then
                DeReservation.ByPassPreReqCheck = True
            Else
                DeReservation.ByPassPreReqCheck = False
            End If
        Else
            If ParentPage = "HOSPITALITYBOOKING" Then
                HospitalityBookingError.Text = String.Empty
                HospitalityBookingError.Text = _ucr.Content("InvalidReservationBasketErrorMessage", _languageCode, True)
            Else
                StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("InvalidReservationBasketErrorMessage", _languageCode, True))
                StandardBasketDetails.Error_List.Items.Add(_ucr.Content("InvalidReservationBasketErrorMessage", _languageCode, True))
            End If
        End If

        If hdfNoneReservationErrorCount.Value = 0 And Not errorGenerated Then
            _err = reservation.ReserveAllBasketItemsReturnBasket()

            If Not _err.HasError AndAlso _
                    reservation.ResultDataSet IsNot Nothing AndAlso _
                    reservation.ResultDataSet.Tables.Count() > 0 AndAlso _
                    reservation.ResultDataSet.Tables("ReservationsInfo") IsNot Nothing AndAlso _
                    reservation.ResultDataSet.Tables("ReservationsInfo").Rows.Count > 0 Then

                _reservations = reservation.ResultDataSet.Tables("ReservationsInfo")
                reservationReference = _reservations.Rows(0).Item("ReservationReference")
                If String.IsNullOrEmpty(reservationReference.Trim) Then
                    If ParentPage = "HOSPITALITYBOOKING" Then
                        HospitalityBookingError.Text = String.Empty
                        HospitalityBookingError.Text = errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE
                    Else
                        StandardBasketDetails.Error_List.Items.Remove(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
                        StandardBasketDetails.Error_List.Items.Add(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
                    End If
                    errorGenerated = True
                End If
            Else
                If reservation.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso
                    reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
                    Dim errorCode As String = reservation.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")
                    If ParentPage = "HOSPITALITYBOOKING" Then
                        HospitalityBookingError.Text = String.Empty
                        HospitalityBookingError.Text = errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE
                    Else
                        StandardBasketDetails.Error_List.Items.Remove(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
                        StandardBasketDetails.Error_List.Items.Add(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
                    End If
                    errorGenerated = True
                Else
                    If ParentPage = "HOSPITALITYBOOKING" Then
                        HospitalityBookingError.Text = String.Empty
                        HospitalityBookingError.Text = errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE
                    Else
                        StandardBasketDetails.Error_List.Items.Remove(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
                        StandardBasketDetails.Error_List.Items.Add(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
                    End If
                    errorGenerated = True
                End If
            End If
        End If

        If hdfNoneReservationErrorCount.Value = 0 AndAlso Not errorGenerated AndAlso Not String.IsNullOrEmpty(reservationReference) Then
            If ParentPage = "HOSPITALITYBOOKING" Then
                Response.Redirect(ResolveUrl("~/PagesAgent/Reservations/ReservationConfirmation.aspx?ReservationRef=" & reservationReference & "&CallId=" & CallID))
            Else
                Response.Redirect(ResolveUrl("~/PagesAgent/Reservations/ReservationConfirmation.aspx?ReservationRef=" & reservationReference))
            End If
        End If
    End Sub
#End Region

#Region "Private Functions"
    ''' <summary>
    ''' Sets text values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetTextValues()
        revExpiryDate.ValidationExpression = _ucr.Attribute("DateFieldRegex")
        revExpiryDate.ErrorMessage = _ucr.Content("RegularExpressionExpiryDateErrorMessage", _languageCode, True)
        rfvExpiryDate.ErrorMessage = _ucr.Content("RequiredFieldExpiryDateErrorMessage", _languageCode, True)
        lblReservedBy.Text = _ucr.Content("ReservedByText", _languageCode, True)
        lblComment.Text = _ucr.Content("CommentText", _languageCode, True)
        lblSaleOrReturn.Text = _ucr.Content("SaleOrReturnText", _languageCode, True)
        lblExpiryDate.Text = _ucr.Content("ExpiryDateText", _languageCode, True)
        btnReserve.Text = _ucr.Content("ReservedButtonText", _languageCode, True)
        CancelButtonText = _ucr.Content("CancelButtonText", _languageCode, True)
        txtComment.Focus()
    End Sub


    Private Function hasBasketInvalidReservationItems() As Boolean
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If ParentPage = "HOSPITALITYBOOKING" Then
                If item.PRODUCT_TYPE_ACTUAL = "S" Then
                    Return True
                End If
            Else
                If item.PRODUCT_TYPE_ACTUAL = "S" Or item.PRODUCT_TYPE_ACTUAL = "P" Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function
#End Region
End Class
