Imports Talent.Common
Partial Class UserControls_TransactionDetails
    Inherits ControlBase

    Private _payReference As String = String.Empty
    Private _payType As String = String.Empty
    Private _payDate As String = String.Empty
    Private _payValue As String = String.Empty
    Private _languageCode As String = Utilities.GetDefaultLanguage
    Private _businessUnit As String = String.Empty
    Private _ucr As UserControlResource
    Private _wfr As WebFormResource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Visible Then
            If IsRetrievedAndClearedSessions() Then

                _businessUnit = TalentCache.GetBusinessUnit()
                Dim pageCode As String = Talent.eCommerce.Utilities.GetCurrentPageName()
                Dim partnerCode As String = TalentCache.GetPartner(HttpContext.Current.Profile)
                _ucr = New UserControlResource
                _wfr = New WebFormResource

                With _ucr
                    .BusinessUnit = _businessUnit
                    .PartnerCode = partnerCode
                    .PageCode = pageCode
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "TransactionDetails.ascx"
                End With

                With _wfr
                    .BusinessUnit = _businessUnit
                    .PartnerCode = partnerCode
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = pageCode
                    .PageCode = pageCode
                End With

                PaymentRef.Text = _payReference
                PaymentType.Text = _payType
                PaymentValue.Text = _payValue
                PaymentDate.Text = _payDate
                GetTransactionDetails()
            Else
                Response.Redirect("TransactionEnquiry.aspx")

            End If
        End If

    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Select Case sender.ID
            Case Is = "PaymentRefLabel"
                CType(sender, Label).Text = _ucr.Content("PaymentRefLabel", _languageCode, True)
            Case Is = "PaymentTypeLabel"
                CType(sender, Label).Text = _ucr.Content("PaymentTypeLabel", _languageCode, True)
            Case Is = "PaymentDateLabel"
                CType(sender, Label).Text = _ucr.Content("PaymentDateLabel", _languageCode, True)
            Case Is = "PaymentValueLabel"
                CType(sender, Label).Text = _ucr.Content("PaymentValueLabel", _languageCode, True)
            Case Is = "ProductNameHeader"
                CType(sender, Label).Text = _ucr.Content("ProductNameHeader", _languageCode, True)
            Case Is = "ProductDescHeader"
                CType(sender, Label).Text = _ucr.Content("ProductDescHeader", _languageCode, True)
            Case Is = "SeatHeader"
                CType(sender, Label).Text = _ucr.Content("SeatHeader", _languageCode, True)
            Case Is = "MemberNumberHeader"
                CType(sender, Label).Text = _ucr.Content("MemberNumberHeader", _languageCode, True)
            Case Is = "PriceHeader"
                CType(sender, Label).Text = _ucr.Content("PriceHeader", _languageCode, True)
            Case Is = "ProductDateHeader"
                CType(sender, Label).Text = _ucr.Content("ProductDateHeader", _languageCode, True)
        End Select
    End Sub

    Private Function IsRetrievedAndClearedSessions() As Boolean
        Dim isSessionValueExists As Boolean = False
        If (Session("TransactionDetailPayRef")) Is Nothing Then
            isSessionValueExists = False
        Else
            _payReference = Session("TransactionDetailPayRef").ToString()
            Session.Remove("TransactionDetailPayRef")

            If (Session("TransactionDetailPayType")) IsNot Nothing Then
                _payType = Session("TransactionDetailPayType").ToString()
                Session.Remove("TransactionDetailPayType")
            End If
            If (Session("TransactionDetailDate")) IsNot Nothing Then
                _payDate = Session("TransactionDetailDate").ToString()
                Session.Remove("TransactionDetailDate")
            End If
            If (Session("TransactionDetailValue")) IsNot Nothing Then
                _payValue = Session("TransactionDetailValue").ToString()
                Session.Remove("TransactionDetailValue")
            End If
            isSessionValueExists = True
        End If
        Return isSessionValueExists
    End Function

    Private Sub GetTransactionDetails()
        Dim order As New Talent.Common.TalentOrder
        Dim deTicketingItemDetails As New Talent.Common.DETicketingItemDetails

        With deTicketingItemDetails
            .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
            .PaymentReference = _payReference
            .UnprintedRecords = ""
            .SetAsPrinted = "N"
            .EndOfSale = "N"
            .Src = "W"
        End With
        order.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        order.Settings.BusinessUnit = _businessUnit
        order.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        order.Settings.CacheStringExtension = _payReference
        order.Settings.Cacheing = CType(_wfr.Attribute("Cacheing"), Boolean)
        order.Settings.CacheTimeMinutes = CType(_wfr.Attribute("CacheTimeMinutes"), Integer)
        order.Dep.CollDEOrders.Add(deTicketingItemDetails)
        Dim err As New ErrorObj
        err = order.OrderDetails
        If Not err.HasError Then
            Dim ds As Data.DataSet = order.ResultDataSet
            Dim receivedTableCount As Integer = ds.Tables.Count
            If receivedTableCount > 0 Then
                If (ds.Tables(0).Rows(0)("ErrorOccurred").ToString().Trim().Length <= 0) Then
                    If (receivedTableCount > 1) Then
                        ' loop through seats - if any are roving then overwrite text
                        For Each dr As Data.DataRow In ds.Tables(1).Rows
                            If dr("Roving").ToString = "Y" Then
                                dr("Seat") = _ucr.Content("RovingAreaText", _languageCode, True)
                            End If
                        Next

                        rptProductsList.DataSource = ds.Tables(1)
                        rptProductsList.DataBind()
                    End If
                End If
            End If
        End If
    End Sub

    Protected Function GetFormattedProductDate(ByVal dateValue As String) As String
        Dim formattedDate As String = String.Empty
        If (dateValue.Trim.Length > 0 AndAlso dateValue.Trim <> "0000000") Then
            formattedDate = Utilities.ISeriesDate(dateValue).ToShortDateString()
        End If
        Return formattedDate
    End Function
End Class
