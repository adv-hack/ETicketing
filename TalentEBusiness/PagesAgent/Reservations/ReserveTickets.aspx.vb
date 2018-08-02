
Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Partial Class PagesAgent_Reservations_ReserveTickets
    Inherits TalentBase01

#Region "Private Variables"
    Private _languageCode As String
    Private _settings As DESettings = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _wfr As New WebFormResource
    Private _err As New ErrorObj
    Private _reservations As DataTable = Nothing
#End Region

#Region "Constants"
    Const GENERAL_ERROR_CODE = "GenericReservationError"
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _settings = Talent.eCommerce.Utilities.GetSettingsObject()
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _wfr = New Talent.Common.WebFormResource
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _businessUnit = TalentCache.GetBusinessUnit()
        TDataObjects.Settings = _settings
        With _wfr
            .BusinessUnit = _businessUnit
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .PageCode = "ReserveTickets.aspx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Profile.Basket.BasketSummary.MerchandiseTotalItems > 0 Then
            blErrorDetails.Items.Add(_wfr.Content("MerchandiseItemsErrorMessage", _languageCode, True))
        Else
            plhReserveTickets.Visible = True
            lblAgentName.Text = AgentProfile.Name
            SetTextValues()
        End If
        If chkSaleOrReturn.Checked Then
            txtExpiryDate.Enabled = False
            rfvExpiryDate.Enabled = False
        Else
            txtExpiryDate.Enabled = True
            rfvExpiryDate.Enabled = True
        End If
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
        reservation.DataEntity = DeReservation
        reservation.Settings = _settings
        Dim dateTimeString() As String
        Dim errMsg As TalentErrorMessages
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner(), _wfr.FrontEndConnectionString)
        If Not chkSaleOrReturn.Checked Then
            If Date.TryParse(txtExpiryDate.Text, reservationTimeDate) Then
                dateTimeString = txtExpiryDate.Text.Split(" ")
                If Date.TryParse(dateTimeString(0), reservationDate) Then
                    DeReservation.ExpiryDate = Utilities.DateToIseries8Format(reservationDate)
                Else
                    blErrorDetails.Items.Add(_wfr.Content("DateTimeFormatErrorText", _languageCode, True))
                End If
                If dateTimeString.Length > 1 AndAlso Date.TryParse(dateTimeString(1), reservationTime) Then
                    DeReservation.ExpiryTime = reservationTime.TimeOfDay().ToString("t")
                Else
                    blErrorDetails.Items.Add(_wfr.Content("DateTimeFormatErrorText", _languageCode, True))
                End If
            Else
                blErrorDetails.Items.Add(_wfr.Content("DateTimeFormatErrorText", _languageCode, True))
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
        If blErrorDetails.Items.Count = 0 Then
            _err = reservation.ReserveAllBasketItemsReturnBasket()

            If Not _err.HasError AndAlso _
                    reservation.ResultDataSet IsNot Nothing AndAlso _
                    reservation.ResultDataSet.Tables.Count() > 0 AndAlso _
                    reservation.ResultDataSet.Tables("ReservationsInfo") IsNot Nothing AndAlso _
                    reservation.ResultDataSet.Tables("ReservationsInfo").Rows.Count > 0 Then

                _reservations = reservation.ResultDataSet.Tables("ReservationsInfo")
                reservationReference = _reservations.Rows(0).Item("ReservationReference")
                If String.IsNullOrEmpty(reservationReference.Trim) Then
                    blErrorDetails.Items.Add(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
                End If
            Else
                If reservation.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso _
                    reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
                    Dim errorCode As String = reservation.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")
                    blErrorDetails.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _wfr.PageCode, errorCode).ERROR_MESSAGE)
                Else
                    blErrorDetails.Items.Add(errMsg.GetErrorMessage(GENERAL_ERROR_CODE).ERROR_MESSAGE)
                End If
            End If
        End If

    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If blErrorDetails.Items.Count > 0 Then
            plhErrorMessage.Visible = True
        Else
            plhErrorMessage.Visible = False
        End If
    End Sub

#End Region

#Region "Private Functions"
    ''' <summary>
    ''' Sets text values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetTextValues()
        revExpiryDate.ValidationExpression = _wfr.Attribute("DateFieldRegex")
        revExpiryDate.ErrorMessage = _wfr.Content("RegularExpressionExpiryDateErrorMessage", _languageCode, True)
        rfvExpiryDate.ErrorMessage = _wfr.Content("RequiredFieldExpiryDateErrorMessage", _languageCode, True)
        lblReservedBy.Text = _wfr.Content("ReservedByText", _languageCode, True)
        lblComment.Text = _wfr.Content("CommentText", _languageCode, True)
        lblSaleOrReturn.Text = _wfr.Content("SaleOrReturnText", _languageCode, True)
        lblExpiryDate.Text = _wfr.Content("ExpiryDateText", _languageCode, True)
        btnReserve.Text = _wfr.Content("ReservedButtonText", _languageCode, True)
    End Sub
#End Region

End Class
