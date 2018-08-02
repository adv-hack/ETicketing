Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Partial Class UserControls_UnreserveTickets
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
    Public ReadOnly Property StandardBasketDetails() As UserControls_BasketDetails
        Get
            Dim basketDetailsControl As New UserControls_BasketDetails
            basketDetailsControl = CType(Talent.eCommerce.Utilities.FindWebControl("BasketDetails1", Me.Page.Controls), UserControls_BasketDetails)
            Return basketDetailsControl
        End Get
    End Property
#End Region
#Region "Constants"
    Const GENERAL_ERROR_CODE = "GenericReservationError"
    Const KEYCODE As String = "UnreserveTickets.ascx"
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
            .PageCode = "UnreserveTickets.aspx"
            .KeyCode = KEYCODE
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        plhUnreserveTickets.Visible = True
        lblComment.Text = _ucr.Content("CommentText", _languageCode, True)
        btnUnreserve.Text = _ucr.Content("UnreservedButtonText", _languageCode, True)
        btnCancel.Text = _ucr.Content("CancelButtonText", _languageCode, True)
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
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)

        ' Retrieve the number of (non-reservation) basket errors
        Dim hdfNoneReservationErrorCount As New HiddenField
        hdfNoneReservationErrorCount = CType(Talent.eCommerce.Utilities.FindWebControl("hdfNoneReservationErrorCount", Me.Page.Controls), HiddenField)

        Dim errorGenerated As Boolean = False

        ' Retail or Season Ticket reservations not allowed
        If Profile.Basket.BasketSummary.MerchandiseTotalItems > 0 Or hasBasketInvalidReservationItems() Then
            StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("InvalidReservationItemsErrorMessage", _languageCode, True))
            StandardBasketDetails.Error_List.Items.Add(_ucr.Content("InvalidReservationItemsErrorMessage", _languageCode, True))
            errorGenerated = True
        End If

        '        If StandardBasketDetails.Error_List.Items.Count = 0 Then
        If hdfNoneReservationErrorCount.Value = 0 Then
            If AgentProfile.IsAgent AndAlso AgentProfile.Name.Length > 0 AndAlso AgentProfile.Type = "2" Then
                DeReservation.ByPassPreReqCheck = True
            Else
                DeReservation.ByPassPreReqCheck = False
            End If
            _err = reservation.UnreserveAllBasketItemsReturnBasket()

            If reservation.ResultDataSet IsNot Nothing AndAlso reservation.ResultDataSet.Tables("ErrorStatus") IsNot Nothing AndAlso _
                reservation.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 Then
                Dim errorCode As String = reservation.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode")
                StandardBasketDetails.Error_List.Items.Remove(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
                StandardBasketDetails.Error_List.Items.Add(errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE)
                errorGenerated = True
            End If

            '            If StandardBasketDetails.Error_List.Items.Count = 0 Then
            If hdfNoneReservationErrorCount.Value = 0 AndAlso Not errorGenerated Then
                Session("ReservationSuccessful") = "True"
                Response.Redirect(ResolveUrl("~/PagesPublic/Basket/Basket.aspx"))
            Else
                StandardBasketDetails.Error_List.Items.Add(_ucr.Content("InvalidReservationBasketErrorMessage", _languageCode, True))
            End If
        Else
            StandardBasketDetails.Error_List.Items.Remove(_ucr.Content("InvalidReservationBasketErrorMessage", _languageCode, True))
            StandardBasketDetails.Error_List.Items.Add(_ucr.Content("InvalidReservationBasketErrorMessage", _languageCode, True))
        End If
    End Sub
#End Region

#Region "Private Methods"
    Private Function hasBasketInvalidReservationItems() As Boolean
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.PRODUCT_TYPE = "S" Or item.PRODUCT_TYPE = "P" Then
                Return True
            End If
        Next
        Return False
    End Function
#End Region

End Class
