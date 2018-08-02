Imports Talent.Common
Imports System.Data

Partial Class UserControls_SmartcardOrderDetails
    Inherits ControlBase

    Private _languageCode As String = Utilities.GetDefaultLanguage
    Dim ucr As New UserControlResource

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SmartcardOrderDetails.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Visible = False
        If IsPageControlVisible(Request.QueryString("PaymentRef")) Then
            If Not String.IsNullOrEmpty(Request.QueryString("PaymentRef")) Then
                Dim paymentRef As String = Request.QueryString("PaymentRef")
                Dim deTickingItemDetails As New DETicketingItemDetails
                Dim scOrderDetails As New TalentOrder
                Dim errorObj As New ErrorObj
                Dim settings As New DESettings
                'Populate the payment reference in ticketing item details with the querystring payment ref
                deTickingItemDetails.PaymentReference = paymentRef
                deTickingItemDetails.Src = "W"

                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.BusinessUnit = TalentCache.GetBusinessUnit()
                settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                scOrderDetails.Dep.CollDEOrders.Add(deTickingItemDetails)
                scOrderDetails.Settings() = settings

                errorObj = scOrderDetails.RetrieveSCOrderDetails()

                If Not errorObj.HasError Then
                    'No errors
                    Dim rsSCOrderDetails As New DataSet
                    rsSCOrderDetails = scOrderDetails.ResultDataSet

                    If Not rsSCOrderDetails Is Nothing Then
                        If rsSCOrderDetails.Tables.Count > 1 Then
                            Dim dtSCOrderDetails As New DataTable
                            dtSCOrderDetails = rsSCOrderDetails.Tables("SCOrderDetails")

                            'Check to see if any rows have been returned
                            If dtSCOrderDetails.Rows.Count = 1 Then
                                'Only show the label and the item in the list if the item value is greater than this attribute: ShowLabelIfValueIsGreaterThan
                                Dim showLabelIfValueIsGreaterThan As Integer = 0
                                showLabelIfValueIsGreaterThan = Utilities.CheckForDBNull_Int(ucr.Attribute("ShowLabelIfValueIsGreaterThan"))

                                'Build the list
                                With lstSmartcardOrderDetails.Items
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfTickets")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfTicketsLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfTickets").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfCardsProduced")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfCardsProducedLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfCardsProduced").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfCardsUploaded")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfCardsUploadedLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfCardsUploaded").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfSeasonTicketsUploaded")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfSeasonTicketsUploadedLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfSeasonTicketsUploaded").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfRFIDTickets")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfRFIDTicketsLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfRFIDTickets").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfBarcodeRequests")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfBarcodeRequestsLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfBarcodeRequests").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfSCErrors")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfSCErrorsLabel", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfSCErrors").ToString))
                                    End If
                                    If Utilities.CheckForDBNull_Int(dtSCOrderDetails.Rows(0)("NoOfNonSCProducts")) > showLabelIfValueIsGreaterThan Then
                                        .Add((ucr.Content("NoOfNonSCProducts", _languageCode, True) + dtSCOrderDetails.Rows(0)("NoOfNonSCProducts").ToString))
                                    End If
                                End With

                                If lstSmartcardOrderDetails.Items.Count <= 0 Then
                                    Me.Visible = False
                                    lstSmartcardOrderDetails.Visible = False
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Else
            Me.Visible = False
            lstSmartcardOrderDetails.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Determines whether [is page control visible]. if visible then do the backend call
    ''' </summary>
    ''' <returns>
    ''' <c>true</c> if [is page control visible]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsPageControlVisible(ByVal paymentRef As String) As Boolean
        Dim isControlVisible As Boolean = False
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowOnPage")) AndAlso Not String.IsNullOrEmpty(Session("AgentType")) _
            AndAlso IsNumeric(paymentRef) AndAlso Profile.Basket.BasketContentType <> GlobalConstants.PAYPALPAYMENTTYPE Then
            Dim agentType As String = Session("AgentType")
            If Talent.eCommerce.Utilities.IsAgent And agentType = "2" And Profile.Basket.BasketItems.Count > 0 Then
                Me.Visible = True
                lstSmartcardOrderDetails.Visible = True
                isControlVisible = True
            End If
        End If
        Return isControlVisible
    End Function

End Class
