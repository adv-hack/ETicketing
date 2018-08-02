Imports Talent.eCommerce
Partial Class PagesLogin_Orders_TransactionDetails
    Inherits TalentBase01

#Region "Class Level Members"
    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = String.Empty
#End Region

#Region "Public Methods"

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TransactionDetails.aspx"
        End With
        If Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Show_OrderEnquiry2_Control")) Then
            OrderEnquiry21.Visible = True
            TransactionDetails1.Visible = False
        Else
            OrderEnquiry21.Visible = False
            TransactionDetails1.Visible = True
        End If

        Dim _isTicketCollection As Boolean = False
        If Session("TicketCollectionMode") IsNot Nothing _
                AndAlso Not String.IsNullOrWhiteSpace(Session("TicketCollectionMode")) Then
            _isTicketCollection = CType(Session("TicketCollectionMode"), Boolean)
        End If

        If Not _isTicketCollection Then
            hlnkTransactionLinkTop.NavigateUrl = "~/PagesLogin/Orders/TransactionEnquiry.aspx"
            hlnkTransactionLinkBottom.NavigateUrl = "~/PagesLogin/Orders/TransactionEnquiry.aspx"
            hlnkTransactionLinkTop.Text = Utilities.CheckForDBNull_String(_wfr.Content("TransactionLinkText", _languageCode, True))
            hlnkTransactionLinkBottom.Text = Utilities.CheckForDBNull_String(_wfr.Content("TransactionLinkText", _languageCode, True))
        Else
            hlnkTransactionLinkTop.NavigateUrl = "~/PagesAgent/Orders/TicketCollection.aspx"
            hlnkTransactionLinkBottom.NavigateUrl = "~/PagesAgent/Orders/TicketCollection.aspx"
            hlnkTransactionLinkTop.Text = Utilities.CheckForDBNull_String(_wfr.Content("TicketCollectionLinkText", _languageCode, True))
            hlnkTransactionLinkBottom.Text = Utilities.CheckForDBNull_String(_wfr.Content("TicketCollectionLinkText", _languageCode, True))
        End If
    End Sub
#End Region

#Region "Private Methods"

#End Region


End Class