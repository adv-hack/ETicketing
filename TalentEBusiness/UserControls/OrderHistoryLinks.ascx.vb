Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Partial Class UserControls_OrderHistoryLinks
    Inherits ControlBase

    Private _ucr As Talent.Common.UserControlResource
    Private _languageCode As String = String.Empty
    Private _orderTemplate As String = String.Empty
    Private _orderTemplateSubType As String = String.Empty
    Private _linkCssClassActive As String = "linkactive"
    Private _linkCssClassInActive As String = "linkinactive"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OrderHistoryLinks.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PopulateTemplateIds()
        If CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DisplayPurchaseHistoryLink")) Then
            liPurchaseHistoryLink.Visible = True
            PurchaseHistoryLinkText.Text = _ucr.Content("PurchaseHistoryText", _languageCode, True)
        End If
        If CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DisplayTransactionHistoryLink")) Then
            liTransactionHistoryLink.Visible = True
            TransactionHistoryLinkText.Text = _ucr.Content("TransactionHistoryText", _languageCode, True)
        End If
        If CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DisplayRetailHistoryLink")) Then
            liRetailHistoryLink.Visible = True
            RetailHistoryLinkText.Text = _ucr.Content("RetailHistoryText", _languageCode, True)
        End If
        If (Not liPurchaseHistoryLink.Visible) AndAlso (Not liTransactionHistoryLink.Visible) AndAlso (Not liRetailHistoryLink.Visible) Then
            plhHistoryLinks.Visible = False
        End If
        Select Case _orderTemplate
            Case Is = "2"
                If _orderTemplateSubType = "1" Then
                    AssignHeaderAndCssClass(_ucr.Content("PurchaseHistoryText", _languageCode, True), _ucr.Content("PurchaseHistoryInstruction", _languageCode, True), _linkCssClassActive, _linkCssClassInActive, _linkCssClassInActive)
                Else
                    AssignHeaderAndCssClass(_ucr.Content("TransactionHistoryText", _languageCode, True), _ucr.Content("TransactionHistoryInstruction", _languageCode, True), _linkCssClassInActive, _linkCssClassActive, _linkCssClassInActive)
                End If
            Case Else
                AssignHeaderAndCssClass(_ucr.Content("RetailHistoryText", _languageCode, True), _ucr.Content("RetailHistoryInstruction", _languageCode, True), _linkCssClassInActive, _linkCssClassInActive, _linkCssClassActive)
        End Select
    End Sub

    Private Sub PopulateTemplateIds()
        If Not String.IsNullOrEmpty(Request.QueryString("OrderType")) Then
            _orderTemplate = Request.QueryString("OrderType").Trim
        ElseIf Session("OrderTemplateType") IsNot Nothing Then
            _orderTemplate = Session("OrderTemplateType").ToString.Trim
        End If
        If Not String.IsNullOrWhiteSpace(Request.QueryString("OrderTemplateSubType")) Then
            _orderTemplateSubType = Request.QueryString("OrderTemplateSubType").Trim
        ElseIf Session("OrderTemplateSubType") IsNot Nothing Then
            _orderTemplateSubType = Session("OrderTemplateSubType")
        Else
            _orderTemplateSubType = "1"
        End If
        If _orderTemplate.Length <= 0 Then
            _orderTemplate = MyBase.ModuleDefaults.Order_Enquiry_Template_Type
        End If
        Session("OrderTemplateType") = _orderTemplate
        Session("OrderTemplateSubType") = _orderTemplateSubType
    End Sub

    Private Sub AssignHeaderAndCssClass(ByVal headerText As String, ByVal instruction As String, ByVal purchaseCss As String, ByVal transactionCss As String, ByVal retailCss As String)
        litHistoryHeader.Text = headerText
        ltlInstruction.Text = instruction
        liPurchaseHistoryLink.Attributes.Add("class", purchaseCss)
        liTransactionHistoryLink.Attributes.Add("class", transactionCss)
        liRetailHistoryLink.Attributes.Add("class", retailCss)
    End Sub
End Class
