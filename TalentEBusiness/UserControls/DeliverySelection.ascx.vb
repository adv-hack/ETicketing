Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_DeliverySelection
    Inherits ControlBase

#Region "Class Level fields"
    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _dcs As New Talent.Common.DEDeliveryCharges
    Private _display As Boolean = True
    Private _isCountryTaxExempted As Boolean = False
#End Region

#Region "Properties"
    Public Property IsDeliverySelectionUpdated() As Boolean = False
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Public ReadOnly Property SelectedDeliveryOption() As String
        Get
            LoadDeliveryCharges()
            Dim val As String = ""
            For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
                If dc.DELIVERY_TYPE = DeliveryDDL1.SelectedValue Then
                    If dc.HasChildNodes Then
                        val = DeliveryDDL2.SelectedValue
                        Exit For
                    Else
                        val = DeliveryDDL1.SelectedValue
                        Exit For
                    End If
                End If
            Next
            Return val
        End Get
    End Property
#End Region

#Region "Public Methods"
    Public Sub ReSetupDeliveryCharges(ByVal countryCode As String, ByVal countryName As String)
        SetupDeliveryCharges(countryCode, countryName)
    End Sub

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If ModuleDefaults.DeliveryCalculationInUse Then
            Session("DeliverySelectionRequiresResetCCForm") = False
            Select Case Profile.Basket.BasketContentType
                Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE, GlobalConstants.COMBINEDBASKETCONTENTTYPE
                    With _ucr
                        .BusinessUnit = TalentCache.GetBusinessUnit
                        .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                        .PageCode = TEBUtilities.GetCurrentPageName()
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .KeyCode = "DeliverySelection.ascx"
                    End With
                    Dim countryCode As String = String.Empty
                    Dim countryName As String = String.Empty
                    If Not Page.IsPostBack Then
                        TEBUtilities.GetDeliveryCountry(Profile.Basket.Temp_Order_Id, countryCode, countryName)
                        SetupDeliveryCharges(countryCode, countryName)
                    Else
                        If DeliveryDDL1.Visible Then
                            DeliveryDDL1.AutoPostBack = True
                        End If
                    End If
                Case Else
                    Me.Display = False
            End Select
        Else
            Me.Display = False
            Me.Visible = False
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack Then
            PostBackHackLoad()
        End If
        If Not Page.IsPostBack Then
            If Display Then
                SetupTexts()
                FormatDeliveryCharges()
            Else
                Me.Visible = False
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        RefreshDeliveryValueInSummary()
        SetDeliveryValueInSession()
        Session("DeliverySelectionRequiresResetCCForm") = True
    End Sub

    Protected Sub DeliveryDDL1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeliveryDDL1.SelectedIndexChanged
        If Display Then
            DeliveryDDL2.Items.Clear()
            For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
                If dc.AVAILABLE Then
                    If dc.DELIVERY_TYPE = DeliveryDDL1.SelectedValue Then
                        If dc.HasChildNodes Then
                            For Each childDc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dc.ChildNodes
                                If childDc.AVAILABLE Then
                                    Dim li As ListItem
                                    If Not String.IsNullOrEmpty(childDc.LANG_DESCRIPTION1) Then
                                        li = New ListItem(childDc.LANG_DESCRIPTION1, childDc.DELIVERY_TYPE)
                                    Else
                                        li = New ListItem(childDc.DESCRIPTION1, childDc.DELIVERY_TYPE)
                                    End If
                                    If Not childDc.HasChildNodes Then
                                        If ModuleDefaults.ShowPricesExVAT Then
                                            li.Text += " - " & Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(childDc.NET_VALUE, 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode))
                                        Else
                                            li.Text += " - " & Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(childDc.GROSS_VALUE, 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode))
                                        End If
                                    End If
                                    DeliveryDDL2.Items.Add(li)
                                End If
                            Next

                            DeliveryDDL2_SelectedIndexChanged(DeliveryDDL2, New EventArgs)
                            DeliveryPanel2.Visible = True
                        Else
                            DeliveryPanel2.Visible = False
                            RefreshDeliveryValueInSummary()
                            'SetDeliveryValueInSession()
                        End If

                        'Set the description
                        If Not String.IsNullOrEmpty(dc.LANG_DESCRIPTION2) Then
                            ltlDesc1.Text = dc.LANG_DESCRIPTION2
                        Else
                            ltlDesc1.Text = dc.DESCRIPTION2
                        End If
                    End If
                End If
            Next
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub DeliveryDDL2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DeliveryDDL2.SelectedIndexChanged
        'Populate the description text
        For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
            If dc.DELIVERY_TYPE = DeliveryDDL1.SelectedValue Then
                If dc.HasChildNodes Then
                    For Each childDc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dc.ChildNodes
                        If childDc.DELIVERY_TYPE = DeliveryDDL2.SelectedValue Then
                            If Not String.IsNullOrEmpty(childDc.LANG_DESCRIPTION1) Then
                                ltlDesc2.Text = childDc.LANG_DESCRIPTION2
                            Else
                                ltlDesc2.Text = childDc.DESCRIPTION2
                            End If
                            Exit For
                        End If
                    Next
                End If
            End If
        Next
        RefreshDeliveryValueInSummary()
        'SetDeliveryValueInSession()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SelectDefaultDeliveryCharge()
        Dim found As Boolean = False
        Dim ddl1count As Integer = 0
        Dim ddl2count As Integer = 0

        For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
            If dc.IS_DEFAULT Then
                found = True
            Else
                For Each childDc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dc.ChildNodes
                    If childDc.IS_DEFAULT Then
                        found = True
                    End If
                    If found Then
                        Exit For
                    Else
                        ddl2count += 1
                    End If
                Next
            End If
            If found Then
                Exit For
            Else
                ddl1count += 1
            End If
        Next

        If found Then
            Dim deliveryChargeEntity As DEDeliveryCharges.DEDeliveryCharge = TEBUtilities.GetDeliveryCharges(ModuleDefaults.DeliveryCalculationInUse)
            If deliveryChargeEntity IsNot Nothing AndAlso DeliveryDDL1.Items.FindByValue(deliveryChargeEntity.DELIVERY_TYPE) IsNot Nothing Then
                DeliveryDDL1.SelectedIndex = DeliveryDDL1.Items.IndexOf(DeliveryDDL1.Items.FindByValue(deliveryChargeEntity.DELIVERY_TYPE))
            Else
                DeliveryDDL1.SelectedIndex = ddl1count
            End If

            DeliveryDDL1_SelectedIndexChanged(DeliveryDDL1, New EventArgs)
            If DeliveryDDL2.Visible Then DeliveryDDL2.SelectedIndex = ddl2count
            DeliveryDDL2_SelectedIndexChanged(DeliveryDDL1, New EventArgs)
        Else
            DeliveryDDL1_SelectedIndexChanged(DeliveryDDL1, New EventArgs)
        End If
    End Sub

    Private Sub SetupTexts()
        titleLabel.Text = _ucr.Content("DeliverySelection_TitleLabel", _languageCode, True)
        ddl1Label.Text = _ucr.Content("DeliverySelection_DDL1Label", _languageCode, True)
        ddl2Label.Text = _ucr.Content("DeliverySelection_DDL2Label", _languageCode, True)
        infoLabel.Text = _ucr.Content("DeliverySelection_InfoLabel", _languageCode, True)
        plhTitle.Visible = (titleLabel.Text.Length > 0)
        plhDeliverySelectionInfo.Visible = (infoLabel.Text.Length > 0)
    End Sub

    Private Sub RefreshDeliveryValueInSummary()
        Dim summary As Object = TEBUtilities.FindWebControl("SummaryTotals1", Me.Page.Controls)
        If Not summary Is Nothing Then
            Try
                Dim gross, net, tax As Decimal
                Dim found As Boolean = False

                For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
                    If dc.DELIVERY_TYPE = DeliveryDDL1.SelectedValue Then
                        If dc.HasChildNodes Then
                            For Each childDc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dc.ChildNodes
                                If childDc.DELIVERY_TYPE = DeliveryDDL2.SelectedValue Then
                                    gross = childDc.GROSS_VALUE
                                    net = childDc.NET_VALUE
                                    tax = childDc.TAX_VALUE
                                    found = True
                                    Exit For
                                End If
                            Next
                        Else
                            gross = dc.GROSS_VALUE
                            net = dc.NET_VALUE
                            tax = dc.TAX_VALUE
                            found = True
                            Exit For
                        End If
                    End If
                    If found Then Exit For
                Next
                If found Then
                    CallByName(summary, "RefreshDeliveryValue", CallType.Method, gross, net, tax)
                    Try
                        Dim overallTotal As Object = TEBUtilities.FindWebControl("CombinedOverallTotal1", Me.Page.Controls)
                        CallByName(overallTotal, "DisplayOverallTotal", CallType.Method)
                    Catch ex As Exception
                    End Try
                End If

            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub SetupDeliveryCharges(ByVal countryCode As String, ByVal countryName As String)
        Dim totals As Talent.Common.TalentWebPricing = Profile.Basket.WebPrices
        If UCase(ModuleDefaults.DeliveryPriceCalculationType) = "UNIT" Then
            If ModuleDefaults.ShowPricesExVAT Then
                _dcs = TEBUtilities.GetDeliveryOptions(totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery)
            Else
                _dcs = TEBUtilities.GetDeliveryOptions(totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery)
            End If
        ElseIf UCase(ModuleDefaults.DeliveryPriceCalculationType) = "WEIGHT" Then
            If ModuleDefaults.ShowPricesExVAT Then
                _dcs = TEBUtilities.GetDeliveryOptions(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes")), totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery, TEBUtilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
            Else
                _dcs = TEBUtilities.GetDeliveryOptions(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes")), totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery, TEBUtilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
            End If
        End If
        FormatDeliveryCharges()
    End Sub

    Private Sub FormatDeliveryCharges()
        Dim isExempted As Boolean = TEBUtilities.IsValidForTaxExemption(Profile.Basket.Temp_Order_Id)
        Dim grossValue As Decimal = 0
        If _dcs.DeliveryCharges.Count > 0 Then
            DeliveryDDL1.Items.Clear()
            DeliveryDDL2.Items.Clear()
            For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
                If dc.AVAILABLE Then
                    Dim li As ListItem
                    If Not String.IsNullOrEmpty(dc.LANG_DESCRIPTION1) Then
                        li = New ListItem(dc.LANG_DESCRIPTION1, dc.DELIVERY_TYPE)
                    Else
                        li = New ListItem(dc.DESCRIPTION1, dc.DELIVERY_TYPE)
                    End If
                    If Not dc.HasChildNodes Then
                        grossValue = dc.GROSS_VALUE
                        If isExempted Then grossValue = dc.NET_VALUE
                        If ModuleDefaults.ShowPricesExVAT Then
                            li.Text += " - " & Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(dc.NET_VALUE, 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode))
                        Else
                            li.Text += " - " & Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(grossValue, 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode))
                        End If
                    End If
                    DeliveryDDL1.Items.Add(li)
                End If
            Next
            SelectDefaultDeliveryCharge()
        End If
    End Sub

    Private Sub SetDeliveryValueInSession()
        Try
            Dim gross, net, tax As Decimal
            Dim deliveryType As String = String.Empty
            Dim found As Boolean = False

            For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In _dcs.DeliveryCharges
                If dc.DELIVERY_TYPE = DeliveryDDL1.SelectedValue Then
                    If dc.HasChildNodes Then
                        For Each childDc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In dc.ChildNodes
                            If childDc.DELIVERY_TYPE = DeliveryDDL2.SelectedValue Then
                                gross = childDc.GROSS_VALUE
                                net = childDc.NET_VALUE
                                tax = childDc.TAX_VALUE
                                deliveryType = childDc.DELIVERY_TYPE
                                found = True
                                Exit For
                            End If
                        Next
                    Else
                        gross = dc.GROSS_VALUE
                        net = dc.NET_VALUE
                        tax = dc.TAX_VALUE
                        deliveryType = dc.DELIVERY_TYPE
                        found = True
                        Exit For
                    End If
                End If
                If found Then Exit For
            Next
            If found Then
                If TEBUtilities.IsValidForTaxExemption(Profile.Basket.Temp_Order_Id) Then
                    gross = net
                    net = net
                    tax = 0
                End If
                Dim deliveryChargeEntity As New Talent.Common.DEDeliveryCharges.DEDeliveryCharge
                deliveryChargeEntity.GROSS_VALUE = gross
                deliveryChargeEntity.NET_VALUE = net
                deliveryChargeEntity.TAX_VALUE = tax
                deliveryChargeEntity.DELIVERY_TYPE = deliveryType
                Session("DeliveryChargeRetail") = deliveryChargeEntity
                TDataObjects.OrderSettings.TblOrderHeader.UpdateOrderDeliveryValues(Profile.Basket.Temp_Order_Id, gross, net, tax)
                If Page.IsPostBack Then
                    If IsValidPostBack() Then
                        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, True)
                    End If
                Else
                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, True)
                End If
            End If

        Catch ex As Exception
        End Try
    End Sub


    Private Sub LoadDeliveryCharges()
        Dim countryCode As String = String.Empty
        Dim countryName As String = String.Empty
        TEBUtilities.GetDeliveryCountry(Profile.Basket.Temp_Order_Id, countryCode, countryName)
        Dim totals As Talent.Common.TalentWebPricing = Profile.Basket.WebPrices
        If UCase(ModuleDefaults.DeliveryPriceCalculationType) = "UNIT" Then
            If ModuleDefaults.ShowPricesExVAT Then
                _dcs = TEBUtilities.GetDeliveryOptions(totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery)
            Else
                _dcs = TEBUtilities.GetDeliveryOptions(totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery)
            End If
        ElseIf UCase(ModuleDefaults.DeliveryPriceCalculationType) = "WEIGHT" Then
            If ModuleDefaults.ShowPricesExVAT Then
                _dcs = TEBUtilities.GetDeliveryOptions(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes")), totals.Total_Items_Value_Net, totals.Qualifies_For_Free_Delivery, TEBUtilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
            Else
                _dcs = TEBUtilities.GetDeliveryOptions(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes")), totals.Total_Items_Value_Gross, totals.Qualifies_For_Free_Delivery, TEBUtilities.GetBasketItemsTotalWeight(Profile.Basket.BasketItems), countryCode, countryName)
            End If
        End If
    End Sub
    Private Sub PostBackHackLoad()
        If IsValidPostBack() Then
            LoadDeliveryCharges()
            SetDeliveryValueInSession()
        End If
    End Sub

    Private Function IsValidPostBack() As Boolean
        Dim isValid As Boolean = False
        Dim postBackControl As Object = TEBUtilities.GetPostbackControl(Me.Page)
        If postBackControl IsNot Nothing Then
            If postBackControl.GetType() IsNot Nothing AndAlso postBackControl.GetType().Name.ToUpper() = "DROPDOWNLIST" Then
                If CType(postBackControl, DropDownList).ID = "DeliveryDDL1" Then
                    isValid = True
                End If
            End If
        End If
        Return isValid
    End Function


#End Region

End Class
