
Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Partial Class PagesAgent_Reservations_UnreserveTickets
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
            .PageCode = "UnreserveTickets.aspx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Profile.Basket.BasketSummary.MerchandiseTotalItems > 0 Then
            blErrorDetails.Items.Add(_wfr.Content("MerchandiseItemsErrorMessage", _languageCode, True))
        Else
            plhUnreserveTickets.Visible = True
            lblComment.Text = _wfr.Content("CommentText", _languageCode, True)
            btnUnreserve.Text = _wfr.Content("UnreservedButtonText", _languageCode, True)
        End If   
    End Sub

    ''' <summary>
    ''' Unreserves all items in basket and handles response from back end
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnUnreserve_Click(sender As Object, e As EventArgs) Handles btnUnreserve.Click
        Dim reservation As TalentReservations = New TalentReservations
        Dim DeReservation As New DEReservations
        Dim reservationDate As New Date
        Dim reservationReference As String = String.Empty
        DeReservation.SessionID = Profile.Basket.Basket_Header_ID
        DeReservation.Agent = AgentProfile.Name
        DeReservation.Comment = txtComment.Text
        DeReservation.Source = GlobalConstants.SOURCE
        DeReservation.CustomerNumber = Profile.UserName
        DeReservation.UnreserveAll = True
        DeReservation.NumberOfSeatsReserved = Profile.Basket.BasketSummary.TotalItemsTicketing
        reservation.DataEntity = DeReservation
        reservation.Settings = _settings
        Dim errMsg As TalentErrorMessages
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), _wfr.FrontEndConnectionString)
        If AgentProfile.IsAgent AndAlso AgentProfile.Name.Length > 0 AndAlso AgentProfile.Type = "2" Then
            DeReservation.ByPassPreReqCheck = True
        Else
            DeReservation.ByPassPreReqCheck = False
        End If
        _err = reservation.UnreserveAllBasketItemsReturnBasket()

        If reservation.ResultDataSet IsNot Nothing AndAlso reservation.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso _
            reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
            Dim errorCode As String = reservation.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")
            blErrorDetails.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _wfr.PageCode, errorCode).ERROR_MESSAGE)
        End If

        If blErrorDetails.Items.Count = 0 Then
            plhConfirmationMessage.Visible = True
            ltlConfirmationDetails.Text = _wfr.Content("SuccessfulUnreserveText", _languageCode, True)

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
End Class
