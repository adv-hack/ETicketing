Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities

Partial Class Products_EditProductRelationships
    Inherits PageControlBase

#Region "Constants"

    Const PAGECODE As String = "EditProductRelationships.aspx"
    Const STARTSYMBOL As String = "*"
    Const NOSELECTION As String = "--"
    Const DEFAULTQUANTITYPERCENT As String = "100"
    Const COMMASEPERATED As String = ","
    Const ZERODOTZEROZERO As String = "0.00"

#End Region

#Region "Private Properties"

    Private Shared _partner As String = String.Empty
    Private Shared _businessUnit As String = String.Empty
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()
    Private _ticketingProductTypes() As String = {"H", "A", "C", "E", "T", "S"}
    Private _editingATicketingLink As Boolean = False
    Private _tempSelectedTicketingProductType1 As String = String.Empty
    Private _productCodeFrom As String = String.Empty

#End Region

#Region "Public Properties"

    Public ProductColumnHeader As String
    Public TypeSubTypeColumnHeader As String
    Public PriceCodesColumnHeader As String
    Public LinkedToColumnHeader As String
    Public ValidateProductLinksMessage As String
    Public Shared NothingFoundText As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        plhErrorMessage.Visible = False
        lblErrorMessage.Text = String.Empty
        setPageText()

        Dim okToEditOrAdd As Boolean = False
        If Request.QueryString("BU") IsNot Nothing Then
            _businessUnit = Utilities.CheckForDBNull_String(Request.QueryString("BU"))
            If Request.QueryString("Partner") IsNot Nothing Then
                _partner = Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
                okToEditOrAdd = True
            End If
        End If

        hplMenuLink.NavigateUrl = "ProductRelationships.aspx?BU=" & _businessUnit & "&Partner=" & GlobalConstants.STAR_ALL

        If okToEditOrAdd Then
            Dim ticketingProducts As DataTable = getTicketingData()
            plhRelationshipEditingFields.Visible = True
            If Request.QueryString("id") IsNot Nothing AndAlso Not Page.IsPostBack Then
                Dim linkId As String = Utilities.CheckForDBNull_String(Request.QueryString("id"))
                Dim productCode As String = String.Empty
                Dim ticketingProductType As String = String.Empty
                Dim ticketingProductSubType As String = String.Empty
                setFormForEditing(linkId, productCode, ticketingProductType, ticketingProductSubType)
                If plhRelationshipEditingFields.Visible Then loadExistingRelationships(productCode, ticketingProductType, ticketingProductSubType)
                btnCreateNewLink.Visible = False
            End If
            btnUpdateLink.Visible = Not btnCreateNewLink.Visible

            If ddlProductType1.SelectedValue = GlobalConstants.RETAILTYPE Then
                aceSearch1.ContextKey = GlobalConstants.RETAILTYPE
                aceCodeSearch1.ContextKey = GlobalConstants.RETAILTYPE
            Else
                aceSearch1.ContextKey = GlobalConstants.TICKETINGTYPE
                aceCodeSearch1.ContextKey = GlobalConstants.TICKETINGTYPE
            End If
            If ddlProductType2.SelectedValue = GlobalConstants.RETAILTYPE Then
                aceSearch2.ContextKey = GlobalConstants.RETAILTYPE
                aceCodeSearch2.ContextKey = GlobalConstants.RETAILTYPE
            Else
                aceSearch2.ContextKey = GlobalConstants.TICKETINGTYPE
                aceCodeSearch2.ContextKey = GlobalConstants.TICKETINGTYPE
            End If

            plhComponentValue.Visible = False
            lblComponentValue.Text = _wfrPage.Content("ComponentValueText", _languageCode, True)
            If ddlTicketingLinkType.SelectedValue = 3 Then
                plhComponentValue.Visible = Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfrPage.Attribute("ShowComponentValue"))
                If btnCreateNewLink.Visible AndAlso txtComponentValue1.Text.Trim.Length = 0 Then txtComponentValue1.Text = ZERODOTZEROZERO
                If btnCreateNewLink.Visible AndAlso txtComponentValue2.Text.Trim.Length = 0 Then txtComponentValue2.Text = ZERODOTZEROZERO
                If btnCreateNewLink.Visible AndAlso txtComponentValue3.Text.Trim.Length = 0 Then txtComponentValue3.Text = ZERODOTZEROZERO
                If btnCreateNewLink.Visible AndAlso txtComponentValue4.Text.Trim.Length = 0 Then txtComponentValue4.Text = ZERODOTZEROZERO
                If btnCreateNewLink.Visible AndAlso txtComponentValue5.Text.Trim.Length = 0 Then txtComponentValue5.Text = ZERODOTZEROZERO

            End If
        Else
            plhRelationshipEditingFields.Visible = False
            pageinstructionsLabel.Text = _wfrPage.Content("PageInstructions_NoBusinessUnit", _languageCode, True)
        End If
    End Sub

    Protected Sub ddlTicketingLinkType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTicketingLinkType.SelectedIndexChanged
        If ddlTicketingLinkType.SelectedValue <> GlobalConstants.PRODUCTLINKTYPEPHASE1ONLY Then
            ddlProductType1.SelectedValue = GlobalConstants.TICKETINGTYPE
            ddlProductType2.SelectedValue = GlobalConstants.TICKETINGTYPE
            ddlProductType1.Enabled = False
            ddlProductType2.Enabled = False
            plhReverseLink.Visible = False
            chkReverseLink.Checked = False
            rfvProductCode2.Enabled = True
            rfvProductDescription2.Enabled = True
            btnUpdateLink.CausesValidation = True
            btnCreateNewLink.CausesValidation = True
            If ddlTicketingLinkType.SelectedValue = GlobalConstants.PRODUCTLINKTYPEPHASE3ONLY Then
                txtDefaultQuantityMin.Visible = False
                txtDefaultQuantityMax.Visible = False
                ltlDefaultQuantityMin.Visible = False
                ltlDefaultQuantityMax.Visible = False
                chkCopyToAllBU.Checked = True
                chkCopyToAllBU.Enabled = False
                plhQuantityDefinition.Visible = False
            Else
                txtDefaultQuantityMin.Visible = True
                txtDefaultQuantityMax.Visible = True
                ltlDefaultQuantityMin.Visible = True
                ltlDefaultQuantityMax.Visible = True
                chkCopyToAllBU.Checked = False
                chkCopyToAllBU.Enabled = True
                plhQuantityDefinition.Visible = True
            End If

        Else
            rfvProductCode2.Enabled = False
            rfvProductDescription2.Enabled = False
            If _editingATicketingLink Then
                ddlProductType1.SelectedValue = GlobalConstants.TICKETINGTYPE
                ddlProductType2.SelectedValue = GlobalConstants.TICKETINGTYPE
            Else
                ddlProductType1.SelectedValue = GlobalConstants.RETAILTYPE
                ddlProductType2.SelectedValue = GlobalConstants.RETAILTYPE
            End If
            ddlProductType1.Enabled = True
            ddlProductType2.Enabled = True
            plhReverseLink.Visible = True
            btnUpdateLink.CausesValidation = False
            btnCreateNewLink.CausesValidation = False
        End If
        ddlProductType1_SelectedIndexChanged(ddlTicketingLinkType, New System.EventArgs)
        ddlProductType2_SelectedIndexChanged(ddlTicketingLinkType, New System.EventArgs)
    End Sub

    Protected Sub ddlProductType1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProductType1.SelectedIndexChanged
        plhTicketingOptions1.Visible = (ddlProductType1.SelectedValue = GlobalConstants.TICKETINGTYPE)
        If plhTicketingOptions1.Visible Then
            ddlTicketingProductType1.DataSource = _ticketingProductTypes
            ddlTicketingProductType1.DataBind()
            If _editingATicketingLink AndAlso _tempSelectedTicketingProductType1.Length > 0 Then
                ddlTicketingProductType1.SelectedValue = _tempSelectedTicketingProductType1
            End If
            getProductSubTypes(ddlTicketingProductType1, ddlTicketingProductSubType1)
            aceSearch1.ContextKey = GlobalConstants.TICKETINGTYPE
            aceCodeSearch1.ContextKey = GlobalConstants.TICKETINGTYPE
            ddlTicketingPriceCode1.Items.Add(NOSELECTION)
        Else
            aceSearch1.ContextKey = GlobalConstants.RETAILTYPE
            aceCodeSearch1.ContextKey = GlobalConstants.RETAILTYPE
        End If
        btnValidateProductCode1.Visible = plhTicketingOptions1.Visible
        btnValidateProductDescription1.Visible = plhTicketingOptions1.Visible
    End Sub

    Protected Sub ddlProductType2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlProductType2.SelectedIndexChanged
        plhTicketingOptions2.Visible = (ddlProductType2.SelectedValue = GlobalConstants.TICKETINGTYPE)
        If plhTicketingOptions2.Visible Then
            ddlTicketingProductType2.DataSource = _ticketingProductTypes
            ddlTicketingProductType2.DataBind()
            getProductSubTypes(ddlTicketingProductType2, ddlTicketingProductSubType2)
            aceSearch2.ContextKey = GlobalConstants.TICKETINGTYPE
            aceCodeSearch2.ContextKey = GlobalConstants.TICKETINGTYPE
            If ddlTicketingLinkType.SelectedValue = GlobalConstants.PRODUCTLINKTYPEPHASE1ONLY Then
                plhTicketingPriceCode2.Visible = False
                plhRelatedTicketingCampaignCode.Visible = False
                plhDefaultTicketingStadiumOptions.Visible = False
                plhAllQuantitySettings.Visible = False
                plhMandatoryProductOption.Visible = False
            Else
                plhAllQuantitySettings.Visible = True
                plhMandatoryProductOption.Visible = True
            End If
        Else
            aceSearch2.ContextKey = GlobalConstants.RETAILTYPE
            aceCodeSearch2.ContextKey = GlobalConstants.RETAILTYPE
        End If
        btnValidateProductCode2.Visible = plhTicketingOptions2.Visible
        btnValidateProductDescription2.Visible = plhTicketingOptions2.Visible
    End Sub

    Protected Sub ddlTicketingProductType1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTicketingProductType1.SelectedIndexChanged
        getProductSubTypes(ddlTicketingProductType1, ddlTicketingProductSubType1)
    End Sub

    Protected Sub ddlTicketingProductType2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTicketingProductType2.SelectedIndexChanged
        getProductSubTypes(ddlTicketingProductType2, ddlTicketingProductSubType2)
    End Sub

    Protected Sub btnCreateNewLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateNewLink.Click
        plhErrorMessage.Visible = False
        Dim errorMessageContent As String = String.Empty
        Page.Validate()
        If btnValidateProductCode1.Visible Then validateProductLinkingFrom(True)
        If btnValidateProductCode2.Visible Then validateProductLinkingTo(True)
        If Not plhErrorMessage.Visible AndAlso Page.IsValid Then
            If ddlProductType1.SelectedValue = GlobalConstants.RETAILTYPE AndAlso String.IsNullOrWhiteSpace(txtProductCode1.Text) AndAlso String.IsNullOrWhiteSpace(txtProductDescription1.Text) Then
                plhErrorMessage.Visible = True
                lblErrorMessage.Text = _wfrPage.Content("NoRetailProductCodeSpecifiedText", _languageCode, True)
            ElseIf String.IsNullOrWhiteSpace(txtProductCode2.Text) AndAlso String.IsNullOrWhiteSpace(txtProductDescription2.Text) Then
                plhErrorMessage.Visible = True
                lblErrorMessage.Text = _wfrPage.Content("NoRelatingProductCodeSpecifiedText", _languageCode, True)
            Else
                Dim productCode As String = String.Empty
                Dim ticketingProductType As String = String.Empty
                Dim ticketingProductSubType As String = String.Empty
                Dim hasError As Boolean = False

                If ddlTicketingLinkType.SelectedValue = GlobalConstants.PRODUCTLINKTYPEPHASE1ONLY Then
                    hasError = createLinkType1Relationship(productCode, ticketingProductType, ticketingProductSubType)
                    If hasError Then
                        plhErrorMessage.Visible = True
                        lblErrorMessage.Text = _wfrPage.Content("RelationshipAlreadyExistsText", _languageCode, True)
                    Else
                        plhErrorMessage.Visible = True
                        lblErrorMessage.Text = _wfrPage.Content("RelationshipCreatedText", _languageCode, True)
                        loadExistingRelationships(productCode, ticketingProductType, ticketingProductSubType)
                        plhRelationshipEditingFields.Visible = False
                    End If
                Else
                    If validateDefaultQuantitySettings() Then
                        If txtProductCode2.Text.Length > 0 AndAlso (ddlTicketingProductSubType1.SelectedValue <> NOSELECTION OrElse txtProductCode1.Text.Length > 0) Then
                            If ddlTicketingProductType2.SelectedValue = GlobalConstants.TRAVELPRODUCTTYPE AndAlso ddlDefaultTicketingExtraTravelOptions.SelectedValue.Length = 0 Then
                                plhErrorMessage.Visible = True
                                lblErrorMessage.Text = _wfrPage.Content("ProductDetailItemMandatory", _languageCode, True)
                            Else
                                If plhRelatedTicketingCampaignCode.Visible AndAlso ddlRelatedTicketingCampaignCode.Items.Count > 1 AndAlso ddlRelatedTicketingCampaignCode.SelectedIndex = 0 Then
                                    plhErrorMessage.Visible = True
                                    lblErrorMessage.Text = _wfrPage.Content("MissingCampaignCodeErrorText", _languageCode, True)
                                Else
                                    If ddlTicketingProductType2.SelectedValue <> GlobalConstants.HOMEPRODUCTTYPE AndAlso plhTicketingPriceCode2.Visible AndAlso ddlTicketingPriceCode2.Items.Count > 1 AndAlso ddlTicketingPriceCode2.SelectedIndex = 0 Then
                                        plhErrorMessage.Visible = True
                                        lblErrorMessage.Text = _wfrPage.Content("MissingPriceCodeErrorText", _languageCode, True)
                                    Else
                                        If _productCodeFrom.Length = 0 AndAlso ddlTicketingProductSubType1.SelectedIndex = 0 Then
                                            plhErrorMessage.Visible = True
                                            lblErrorMessage.Text = _wfrPage.Content("InvalidProductCodeText", _languageCode, True)
                                        Else
                                            If AreComponentValuesValidated(errorMessageContent) Then
                                                hasError = createLinkType2OrType3OrBothRelationship(productCode, ticketingProductType, ticketingProductSubType, errorMessageContent)
                                            Else
                                                hasError = True
                                            End If
                                            If hasError Then
                                                plhErrorMessage.Visible = True
                                                lblErrorMessage.Text = _wfrPage.Content(errorMessageContent, _languageCode, True)
                                            Else
                                                plhErrorMessage.Visible = True
                                                lblErrorMessage.Text = _wfrPage.Content("RelationshipCreatedText", _languageCode, True)
                                                loadExistingRelationships(productCode, ticketingProductType, ticketingProductSubType)
                                                plhRelationshipEditingFields.Visible = False
                                                btnAddNewRelationship.Visible = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            plhErrorMessage.Visible = True
                            lblErrorMessage.Text = _wfrPage.Content("MissingProductCodesErrorText", _languageCode, True)
                        End If
                    Else
                        plhErrorMessage.Visible = True
                        lblErrorMessage.Text = _wfrPage.Content("NotAValidQuantityErrorText", _languageCode, True)
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub btnUpdateLink_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateLink.Click
        plhErrorMessage.Visible = False
        Page.Validate()
        validateProductLinkingTo(True)
        validateProductLinkingFrom(True)
        If Not plhErrorMessage.Visible AndAlso Page.IsValid Then
            If ddlProductType1.SelectedValue = GlobalConstants.RETAILTYPE AndAlso String.IsNullOrWhiteSpace(txtProductCode1.Text) AndAlso String.IsNullOrWhiteSpace(txtProductDescription1.Text) Then
                plhErrorMessage.Visible = True
                lblErrorMessage.Text = _wfrPage.Content("NoRetailProductCodeSpecifiedText", _languageCode, True)
            ElseIf String.IsNullOrWhiteSpace(txtProductCode2.Text) AndAlso String.IsNullOrWhiteSpace(txtProductDescription2.Text) Then
                plhErrorMessage.Visible = True
                lblErrorMessage.Text = _wfrPage.Content("NoRelatingProductCodeSpecifiedText", _languageCode, True)
            Else
                Dim productCode As String = String.Empty
                Dim ticketingProductType As String = String.Empty
                Dim ticketingProductSubType As String = String.Empty
                Dim hasError As Boolean = False
                Dim errorMessageText As String = String.Empty
                If validateDefaultQuantitySettings() Then
                    If txtProductCode2.Text.Length > 0 AndAlso (ddlTicketingProductSubType1.SelectedValue <> NOSELECTION OrElse txtProductCode1.Text.Length > 0) Then
                        If ddlTicketingProductType2.SelectedValue = GlobalConstants.TRAVELPRODUCTTYPE AndAlso ddlDefaultTicketingExtraTravelOptions.SelectedValue.Length = 0 Then
                            plhErrorMessage.Visible = True
                            lblErrorMessage.Text = _wfrPage.Content("ProductDetailItemMandatory", _languageCode, True)
                        Else
                            If plhRelatedTicketingCampaignCode.Visible AndAlso ddlRelatedTicketingCampaignCode.Items.Count > 1 AndAlso ddlRelatedTicketingCampaignCode.SelectedIndex = 0 Then
                                plhErrorMessage.Visible = True
                                lblErrorMessage.Text = _wfrPage.Content("MissingCampaignCodeErrorText", _languageCode, True)
                            Else
                                If ddlTicketingProductType2.SelectedValue <> GlobalConstants.HOMEPRODUCTTYPE AndAlso plhTicketingPriceCode2.Visible AndAlso ddlTicketingPriceCode2.Items.Count > 1 AndAlso ddlTicketingPriceCode2.SelectedIndex = 0 Then
                                    plhErrorMessage.Visible = True
                                    lblErrorMessage.Text = _wfrPage.Content("MissingPriceCodeErrorText", _languageCode, True)
                                Else
                                    If AreComponentValuesValidated(errorMessageText) Then
                                        hasError = updateExistingProductRelationship(productCode, ticketingProductType, ticketingProductSubType, errorMessageText)
                                    Else
                                        hasError = True
                                    End If
                                    If hasError Then
                                        plhErrorMessage.Visible = True
                                        lblErrorMessage.Text = _wfrPage.Content(errorMessageText, _languageCode, True)
                                    Else
                                        plhErrorMessage.Visible = True
                                        lblErrorMessage.Text = _wfrPage.Content("RelationshipUpdatedText", _languageCode, True)
                                        loadExistingRelationships(productCode, ticketingProductType, ticketingProductSubType)
                                    End If
                                End If
                            End If
                        End If
                    Else
                        plhErrorMessage.Visible = True
                        lblErrorMessage.Text = _wfrPage.Content("MissingProductCodesErrorText", _languageCode, True)
                    End If
                Else
                    plhErrorMessage.Visible = True
                    lblErrorMessage.Text = _wfrPage.Content("NotAValidQuantityErrorText", _languageCode, True)
                End If
            End If
        End If
    End Sub

    Protected Sub btnGoBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGoBack.Click
        Dim redirectUrl As String = "ProductRelationships.aspx?BU={0}&Partner={1}"
        redirectUrl = String.Format(redirectUrl, _businessUnit, _partner)
        Response.Redirect(redirectUrl)
    End Sub

    Protected Sub btnAddNewRelationship_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewRelationship.Click
        Dim redirectUrl As New StringBuilder
        redirectUrl.Append("~/Products/EditProductRelationships.aspx?BU=")
        redirectUrl.Append(Request.QueryString("BU"))
        redirectUrl.Append("&Partner=")
        redirectUrl.Append(Request.QueryString("Partner"))
        Response.Redirect(redirectUrl.ToString())
    End Sub

    Protected Sub btnValidateProductCode1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnValidateProductCode1.Click
        txtProductDescription1.Text = String.Empty
        validateProductLinkingFrom(False)
    End Sub

    Protected Sub btnValidateProductDescription1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnValidateProductDescription1.Click
        txtProductCode1.Text = String.Empty
        validateProductLinkingFrom(False)
    End Sub

    Protected Sub btnValidateProductCode2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnValidateProductCode2.Click
        txtProductDescription2.Text = String.Empty
        validateProductLinkingTo(False)
    End Sub

    Protected Sub btnValidateProductDescription2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnValidateProductDescription2.Click
        txtProductCode2.Text = String.Empty
        validateProductLinkingTo(False)
    End Sub

    Protected Sub ddlDefaultTicketingStand_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlDefaultTicketingStand.SelectedIndexChanged
        If hdfStadiumCode.Value.Length > 0 AndAlso ddlDefaultTicketingStand.SelectedIndex > 0 Then
            Dim dsStadiumData As DataSet = getStadiumDetails(hdfStadiumCode.Value)
            If dsStadiumData.Tables.Count = 3 AndAlso dsStadiumData.Tables(1).Rows.Count > 0 Then
                Dim validAreas As DataRow() = dsStadiumData.Tables(1).Select("StandCode = '" & ddlDefaultTicketingStand.SelectedValue & "'")
                If validAreas.Length > 0 Then
                    ddlDefaultTicketingArea.DataSource = validAreas.CopyToDataTable
                    ddlDefaultTicketingArea.DataTextField = "AreaDescription"
                    ddlDefaultTicketingArea.DataValueField = "AreaCode"
                    ddlDefaultTicketingArea.DataBind()
                    ddlDefaultTicketingArea.Items.Insert(0, NOSELECTION)
                End If
            End If
        End If
    End Sub

    Protected Sub chkQuantityDefintion_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkQuantityDefintion.CheckedChanged
        If chkQuantityDefintion.Checked Then
            plhQuantityRoundUp.Visible = True
            txtDefaultQuantity.Text = DEFAULTQUANTITYPERCENT
            txtDefaultQuantityMax.Text = DEFAULTQUANTITYPERCENT
            txtDefaultQuantityMin.Text = DEFAULTQUANTITYPERCENT
            lblQuantityPercentage.Visible = True
            lblMaxPercentage.Visible = True
            lblMinPercentage.Visible = True
        Else
            plhQuantityRoundUp.Visible = False
            txtDefaultQuantity.Text = String.Empty
            txtDefaultQuantityMax.Text = String.Empty
            txtDefaultQuantityMin.Text = String.Empty
            lblQuantityPercentage.Visible = False
            lblMaxPercentage.Visible = False
            lblMinPercentage.Visible = False
        End If
    End Sub

    Protected Sub chkDefaultQuantityReadonlyOption_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDefaultQuantityReadonlyOption.CheckedChanged
        plhDefaultQuantityMinMax.Visible = (Not chkDefaultQuantityReadonlyOption.Checked)
        If Not plhDefaultQuantityMinMax.Visible Then
            txtDefaultQuantityMax.Text = String.Empty
            txtDefaultQuantityMin.Text = String.Empty
        End If
    End Sub

    Protected Sub chkQuantitySettings_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkQuantitySettings.CheckedChanged
        plhQuantitySettings.Visible = chkQuantitySettings.Checked
        If Not plhQuantitySettings.Visible Then
            chkQuantityDefintion.Checked = False
            chkQuantityDefintion_CheckedChanged(chkQuantityDefintion, New System.EventArgs)
            chkQuantityRoundUp.Checked = False
            chkDefaultQuantityReadonlyOption.Checked = False
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the text properties for the page controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageText()
        Page.Title = _wfrPage.Content("PageTitle", _languageCode, True)
        pagetitleLabel.Text = _wfrPage.Content("PageTitle", _languageCode, True)
        pageinstructionsLabel.Text = _wfrPage.Content("PageInstructions", _languageCode, True)
        ltlEditingInstructions1.Text = _wfrPage.Content("EditingInstructions1", _languageCode, True)
        ltlEditingInstructions2.Text = _wfrPage.Content("EditingInstructions2", _languageCode, True)
        lblProductType1.Text = _wfrPage.Content("ProductTypeText", _languageCode, True)
        lblProductType2.Text = _wfrPage.Content("ProductTypeText", _languageCode, True)
        Dim retailListItem As New ListItem
        retailListItem.Value = GlobalConstants.RETAILTYPE
        retailListItem.Text = _wfrPage.Content("RetailTypeText", _languageCode, True)
        Dim ticketingListItem As New ListItem
        ticketingListItem.Value = GlobalConstants.TICKETINGTYPE
        ticketingListItem.Text = _wfrPage.Content("TicketingTypeText", _languageCode, True)
        If Not Page.IsPostBack Then
            ddlProductType1.Items.Clear()
            ddlProductType2.Items.Clear()
            ddlProductType1.Items.Add(retailListItem)
            ddlProductType1.Items.Add(ticketingListItem)
            ddlProductType2.Items.Add(retailListItem)
            ddlProductType2.Items.Add(ticketingListItem)
        End If

        lblProductCode1.Text = _wfrPage.Content("ProductCodeText", _languageCode, True)
        lblProductCode2.Text = _wfrPage.Content("ProductCodeText", _languageCode, True)
        lblProductDescription1.Text = _wfrPage.Content("ProductDescriptionText", _languageCode, True)
        lblProductDescription2.Text = _wfrPage.Content("ProductDescriptionText", _languageCode, True)
        lblTicketingProductType1.Text = _wfrPage.Content("TicketingProductTypeText", _languageCode, True)
        lblTicketingProductType2.Text = _wfrPage.Content("TicketingProductTypeText", _languageCode, True)
        lblTicketingProductSubType1.Text = _wfrPage.Content("TicketingProductSubTypeText", _languageCode, True)
        lblTicketingProductSubType2.Text = _wfrPage.Content("TicketingProductSubTypeText", _languageCode, True)
        lblTicketingPriceCode1.Text = _wfrPage.Content("TicketingPriceCodeText", _languageCode, True)
        lblTicketingPriceCode2.Text = _wfrPage.Content("TicketingPriceCodeText", _languageCode, True)
        lblRelatedTicketingCampaignCode.Text = _wfrPage.Content("TicketingCampaignCodeText", _languageCode, True)
        lblTicketingLinkType.Text = _wfrPage.Content("TicketingLinkTypeText", _languageCode, True)
        Dim linkTypeBoth As New ListItem
        linkTypeBoth.Value = GlobalConstants.PRODUCTLINKTYPEBOTH
        linkTypeBoth.Text = _wfrPage.Content("ProductLinkTypeBothText", _languageCode, True)
        Dim linkTypePhase1 As New ListItem
        linkTypePhase1.Value = GlobalConstants.PRODUCTLINKTYPEPHASE1ONLY
        linkTypePhase1.Text = _wfrPage.Content("ProductLinkTypePhase1", _languageCode, True)
        Dim linkTypePhase2 As New ListItem
        linkTypePhase2.Value = GlobalConstants.PRODUCTLINKTYPEPHASE2ONLY
        linkTypePhase2.Text = _wfrPage.Content("ProductLinkTypePhase2", _languageCode, True)
        Dim linkTypePhase3 As New ListItem
        linkTypePhase3.Value = GlobalConstants.PRODUCTLINKTYPEPHASE3ONLY
        linkTypePhase3.Text = _wfrPage.Content("ProductLinkTypePhase3", _languageCode, True)
        If Not Page.IsPostBack Then
            ddlTicketingLinkType.Items.Clear()
            ddlTicketingLinkType.Items.Add(linkTypePhase1)
            ddlTicketingLinkType.Items.Add(linkTypePhase2)
            ddlTicketingLinkType.Items.Add(linkTypePhase3)
            ddlTicketingLinkType.Items.Add(linkTypeBoth)
        End If
        ltlTicketingLinkTypeInstructions.Text = _wfrPage.Content("TicketingLinkTypeInstructionsText", _languageCode, True)

        lblDefaultTicketingStand.Text = _wfrPage.Content("DefaultTicketingStandText", _languageCode, True)
        lblDefaultTicketingStandReadonlyOption.Text = _wfrPage.Content("SetDefaultValueToReadonlyText", _languageCode, True)
        lblDefaultTicketingArea.Text = _wfrPage.Content("DefaultTicketingAreaText", _languageCode, True)
        lblDefaultTicketingAreaReadonlyOption.Text = _wfrPage.Content("SetDefaultValueToReadonlyText", _languageCode, True)
        lblDefaultTicketingExtraTravelOptions.Text = _wfrPage.Content("DefaultTicketingExtraTravelOptionsText", _languageCode, True)
        lblQuantitySettings.Text = _wfrPage.Content("QuantitySettingsText", _languageCode, True)
        lblQuantityDefinition.Text = _wfrPage.Content("QuantityDefinitionText", _languageCode, True)
        ltlQuantityDefinitionInstructions.Text = _wfrPage.Content("QuantityDefinitionInstructionsText", _languageCode, True)
        lblQuantityRoundUp.Text = _wfrPage.Content("QuantityRoundUpText ", _languageCode, True)
        ltlQuantityRoundUpInstructions.Text = _wfrPage.Content("QuantityRoundUpInstructionsText", _languageCode, True)
        lblDefaultQuantity.Text = _wfrPage.Content("DefaultQuantityText", _languageCode, True)
        lblDefaultQuantityReadonlyOption.Text = _wfrPage.Content("SetDefaultValueToReadonlyText", _languageCode, True)
        ltlDefaultQuantityMin.Text = _wfrPage.Content("DefaultQuantityMinText", _languageCode, True)
        ltlDefaultQuantityMax.Text = _wfrPage.Content("DefaultQuantityMaxText", _languageCode, True)
        lblCssClass.Text = _wfrPage.Content("CssClassText", _languageCode, True)
        lblInstructions.Text = _wfrPage.Content("InstructionsText", _languageCode, True)
        lblMandatoryProduct.Text = _wfrPage.Content("MandatoryProductText", _languageCode, True)
        lblCopyToAllBU.Text = _wfrPage.Content("CopyToAllBUText", _languageCode, True)
        ltlMandatoryProductInstructions.Text = _wfrPage.Content("MandatoryProductInstructionsText", _languageCode, True)
        lblReverseLink.Text = _wfrPage.Content("ReverseLinkLabelText", _languageCode, True)
        ltlReverseLinkInstructions.Text = _wfrPage.Content("ReverseLinkInstructions", _languageCode, True)

        btnValidateProductCode1.Text = _wfrPage.Content("ValidateButtonText", _languageCode, True)
        btnValidateProductDescription1.Text = _wfrPage.Content("ValidateButtonText", _languageCode, True)
        btnValidateProductCode2.Text = _wfrPage.Content("ValidateButtonText", _languageCode, True)
        btnValidateProductDescription2.Text = _wfrPage.Content("ValidateButtonText", _languageCode, True)
        btnCreateNewLink.Text = _wfrPage.Content("CreateNewLinkButtonText", _languageCode, True)
        btnUpdateLink.Text = _wfrPage.Content("UpdateLinkButtonText", _languageCode, True)
        btnGoBack.Text = _wfrPage.Content("GoBackButtonText", _languageCode, True)
        btnAddNewRelationship.Text = _wfrPage.Content("AddNewRelationshipButtonText", _languageCode, True)
        ProductColumnHeader = _wfrPage.Content("ProductColumnHeaderText", _languageCode, True)
        LinkedToColumnHeader = _wfrPage.Content("LinkedToColumnHeaderText", _languageCode, True)
        TypeSubTypeColumnHeader = _wfrPage.Content("TypeSubTypeColumnHeaderText", _languageCode, True)
        PriceCodesColumnHeader = _wfrPage.Content("PriceCodesColumnHeaderText", _languageCode, True)
        NothingFoundText = _wfrPage.Content("NothingFoundText", _languageCode, True)
        If Not IsPostBack Then chkReverseLink.Checked = Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfrPage.Attribute("ReverseLinkDefaultValue"))
        aceCodeSearch1.CompletionSetCount = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("ACECompletionSetCount"))
        aceCodeSearch2.CompletionSetCount = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("ACECompletionSetCount"))
        aceSearch1.CompletionSetCount = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("ACECompletionSetCount"))
        aceSearch2.CompletionSetCount = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("ACECompletionSetCount"))
        ValidateProductLinksMessage = _wfrPage.Content("ValidateProductLinksMessage", _languageCode, True)

        rfvProductCode2.ErrorMessage = _wfrPage.Content("ValidateProductCode2Error", _languageCode, True)
        rfvProductDescription2.ErrorMessage = _wfrPage.Content("ValidateProductDescription2Error", _languageCode, True)


    End Sub

    ''' <summary>
    ''' Load the relations into the fields based on the given product code
    ''' </summary>
    ''' <param name="productCode">Product code</param>
    ''' <remarks></remarks>
    Private Sub loadExistingRelationships(ByVal productCode As String, ByVal ticketingProductType As String, ByVal ticketingProductSubType As String)
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        Dim productRelationsTable As New DataTable
        productRelationsTable = tDataObjects.ProductsSettings.TblProductRelations.GetAllProductRelationsByBUPartnerAndProductInfo(_businessUnit, _partner, productCode, ticketingProductType, ticketingProductSubType, False)

        If productRelationsTable.Rows.Count > 0 Then
            If String.IsNullOrWhiteSpace(ticketingProductType) Then
                ticketingProductType = productRelationsTable.Rows(0)("TICKETING_PRODUCT_TYPE").ToString()
                If Not String.IsNullOrWhiteSpace(ticketingProductType) Then
                    plhTicketingOptions1.Visible = True
                    ddlTicketingProductType1.DataSource = _ticketingProductTypes
                    ddlTicketingProductType1.DataBind()
                    getProductSubTypes(ddlTicketingProductType1, ddlTicketingProductSubType1)
                    ddlTicketingProductType1.SelectedValue = ticketingProductType
                End If
                ticketingProductSubType = productRelationsTable.Rows(0)("TICKETING_PRODUCT_SUB_TYPE").ToString()
                If Not String.IsNullOrWhiteSpace(ticketingProductSubType) Then
                    ddlTicketingProductSubType1.SelectedValue = ticketingProductSubType
                End If
            End If
            Dim matchedRows() As DataRow = Nothing
            Dim allProductsTable As New DataTable
            Dim ticketingTable As New DataTable
            Dim retailTable As New DataTable
            ticketingTable = getTicketingData()
            retailTable = getRetailData()
            If ticketingProductType <> String.Empty Then
                matchedRows = ticketingTable.Select("ProductCode = '" & productCode & "'")
                If matchedRows.Length > 0 Then txtProductDescription1.Text = matchedRows(0)("ProductDescription").ToString()
                ddlProductType1.SelectedIndex = 1
            Else
                matchedRows = retailTable.Select("PRODUCT_CODE = '" & productCode & "'")
                If matchedRows.Length > 0 Then txtProductDescription1.Text = matchedRows(0)("PRODUCT_DESCRIPTION_1").ToString()
                ddlProductType1.SelectedIndex = 0
            End If
            txtProductCode1.Text = productCode
            matchedRows = productRelationsTable.Select()
            allProductsTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
            plhRelationships.Visible = bindRepeater(matchedRows, allProductsTable)
            txtProductCode1.ReadOnly = True
            txtProductDescription1.ReadOnly = True
            ddlProductType1.Enabled = False
            If txtProductDescription1.Text.Length > 0 Then
                ltlProductsRelatingTo.Text = _wfrPage.Content("ProductsRelatingToText", _languageCode, True).Replace("<<ProductDescription>>", txtProductDescription1.Text)
            ElseIf txtProductCode1.Text.Length > 0 Then
                ltlProductsRelatingTo.Text = _wfrPage.Content("ProductsRelatingToText", _languageCode, True).Replace("<<ProductDescription>>", txtProductCode1.Text)
            ElseIf ddlProductType1.SelectedValue.Length > 0 Then
                ltlProductsRelatingTo.Text = _wfrPage.Content("ProductsRelatingToText", _languageCode, True).Replace("<<ProductDescription>>", ddlProductType1.SelectedValue)
            Else
                ltlProductsRelatingTo.Visible = ""
            End If


        End If
    End Sub

    ''' <summary>
    ''' Setup the page based on editing an existing link
    ''' </summary>
    ''' <param name="linkId">The link id to be edited</param>
    ''' <param name="productCode">The product code being linked from</param>
    ''' <param name="ticketingProductType">The ticketing product type being linked from</param>
    ''' <param name="ticketingProductSubType">The ticketing product sub type being linked from</param>
    ''' <remarks></remarks>
    Private Sub setFormForEditing(ByVal linkId As String, ByRef productCode As String, ByRef ticketingProductType As String, ByRef ticketingProductSubType As String)
        Dim dtProductRelations As DataTable = TDataObjects.ProductsSettings.TblProductRelations.GetAllProductRelationsById(linkId)
        If dtProductRelations.Rows.Count > 0 Then
            plhRelationshipEditingFields.Visible = True

            'Product Linking From
            ddlTicketingLinkType.SelectedValue = dtProductRelations.Rows(0)("LINK_TYPE").ToString()
            If Not String.IsNullOrEmpty(TCUtilities.CheckForDBNull_String(dtProductRelations.Rows(0)("TICKETING_PRODUCT_TYPE"))) Then
                _editingATicketingLink = True
                _tempSelectedTicketingProductType1 = dtProductRelations.Rows(0)("TICKETING_PRODUCT_TYPE")
            End If
            ddlTicketingLinkType_SelectedIndexChanged(ddlTicketingLinkType, New System.EventArgs)
            ddlProductType1_SelectedIndexChanged(ddlTicketingLinkType, New System.EventArgs)
            ddlTicketingLinkType.Enabled = False
            txtProductCode1.Text = dtProductRelations.Rows(0)("PRODUCT")
            validateProductLinkingFrom(False)
            ddlTicketingProductSubType1.SelectedValue = dtProductRelations.Rows(0)("TICKETING_PRODUCT_SUB_TYPE").ToString()
            ddlTicketingPriceCode1.SelectedValue = dtProductRelations.Rows(0)("TICKETING_PRODUCT_PRICE_CODE").ToString()
            productCode = txtProductCode1.Text
            ticketingProductType = ddlTicketingProductType1.SelectedValue
            ticketingProductSubType = ddlTicketingProductSubType1.SelectedValue

            'Product Linking To
            ddlProductType2_SelectedIndexChanged(ddlTicketingLinkType, New System.EventArgs)
            txtProductCode2.Text = dtProductRelations.Rows(0)("RELATED_PRODUCT").ToString()
            validateProductLinkingTo(False)
            ddlTicketingProductSubType2.SelectedValue = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString()
            ddlTicketingPriceCode2.SelectedValue = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString()
            ddlRelatedTicketingCampaignCode.SelectedValue = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString()
            If plhDefaultTicketingStadiumOptions.Visible Then
                ddlDefaultTicketingStand.SelectedValue = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_STAND").ToString()
                chkDefaultTicketingStandReadonlyOption.Checked = CBool(dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_STAND_READONLY"))
                ddlDefaultTicketingStand_SelectedIndexChanged(ddlDefaultTicketingStand, New System.EventArgs)
                ddlDefaultTicketingArea.SelectedValue = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_AREA").ToString()
                chkDefaultTicketingAreaReadonlyOption.Checked = CBool(dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_AREA_READONLY"))
            End If
            Dim travelProductDetailId As String = String.Empty
            travelProductDetailId = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_STAND").ToString() & dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_AREA").ToString()
            If travelProductDetailId.Length = 0 Then
                ddlDefaultTicketingExtraTravelOptions.SelectedIndex = 0
            Else
                ddlDefaultTicketingExtraTravelOptions.SelectedValue = travelProductDetailId
            End If
            chkMandatoryProduct.Checked = CBool(dtProductRelations.Rows(0)("RELATED_PRODUCT_MANDATORY"))
            txtCssClass.Text = dtProductRelations.Rows(0)("RELATED_CSS_CLASS")
            txtInstructions.Text = dtProductRelations.Rows(0)("RELATED_INSTRUCTIONS")

            'Quantity Settings
            Dim useRatios As Boolean = CBool(dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_RATIO"))
            Dim roundUp As Boolean = CBool(dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_ROUND_UP"))
            Dim quantityReadOnly As Boolean = CBool(dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_READONLY"))
            Dim quantity As String = String.Empty
            Dim min As String = String.Empty
            Dim max As String = String.Empty
            If Not dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY").Equals(DBNull.Value) Then quantity = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY").ToString()
            If Not dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_MIN").Equals(DBNull.Value) Then min = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_MIN").ToString()
            If Not dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_MAX").Equals(DBNull.Value) Then max = dtProductRelations.Rows(0)("RELATED_TICKETING_PRODUCT_QTY_MAX").ToString()

            If useRatios OrElse roundUp OrElse quantityReadOnly OrElse quantity.Length > 0 OrElse min.Length > 0 OrElse max.Length > 0 Then
                plhQuantitySettings.Visible = True
            Else
                plhQuantitySettings.Visible = False
            End If
            If plhQuantitySettings.Visible Then
                chkQuantitySettings.Checked = True
                chkQuantitySettings_CheckedChanged(chkQuantitySettings, New System.EventArgs)
                If useRatios Then
                    chkQuantityDefintion.Checked = True
                    chkQuantityDefintion_CheckedChanged(chkQuantityDefintion, New System.EventArgs)
                    chkQuantityRoundUp.Checked = roundUp
                End If
                If quantity.Length > 0 Then txtDefaultQuantity.Text = CInt(quantity)
                If min.Length > 0 Then txtDefaultQuantityMin.Text = CInt(min)
                If max.Length > 0 Then txtDefaultQuantityMax.Text = CInt(max)
                chkQuantityRoundUp.Checked = roundUp
                chkDefaultQuantityReadonlyOption.Checked = quantityReadOnly
            End If

            If dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_01").Equals(DBNull.Value) Then
                txtComponentValue1.Text = "0.00"
            Else
                txtComponentValue1.Text = dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_01").ToString()
            End If

            If dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_02").Equals(DBNull.Value) Then
                txtComponentValue2.Text = "0.00"
            Else
                txtComponentValue2.Text = dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_02").ToString()
            End If

            If dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_03").Equals(DBNull.Value) Then
                txtComponentValue3.Text = "0.00"
            Else
                txtComponentValue3.Text = dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_03").ToString()
            End If

            If dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_04").Equals(DBNull.Value) Then
                txtComponentValue4.Text = "0.00"
            Else
                txtComponentValue4.Text = dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_04").ToString()
            End If

            If dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_05").Equals(DBNull.Value) Then
                txtComponentValue5.Text = "0.00"
            Else
                txtComponentValue5.Text = dtProductRelations.Rows(0)("PACKAGE_COMPONENT_VALUE_05").ToString()
            End If

            If dtProductRelations.Rows(0)("PACKAGE_COMPONENT_PRICE_BANDS").Equals(DBNull.Value) OrElse _
                dtProductRelations.Rows(0)("PACKAGE_COMPONENT_PRICE_BANDS").Equals(String.Empty) OrElse _
                Not dtProductRelations.Rows(0)("PACKAGE_COMPONENT_PRICE_BANDS").ToString().Contains(",") Then
                txtComponentPriceBand1.Text = String.Empty
                txtComponentPriceBand2.Text = String.Empty
                txtComponentPriceBand3.Text = String.Empty
                txtComponentPriceBand4.Text = String.Empty
                txtComponentPriceBand5.Text = String.Empty
            Else
                Dim priceBands() As String = dtProductRelations.Rows(0)("PACKAGE_COMPONENT_PRICE_BANDS").ToString.Split(",")
                txtComponentPriceBand1.Text = priceBands(0)
                txtComponentPriceBand2.Text = priceBands(1)
                txtComponentPriceBand3.Text = priceBands(2)
                txtComponentPriceBand4.Text = priceBands(3)
                txtComponentPriceBand5.Text = priceBands(4)
            End If
            'Disable fields that can't be changed when editing a link
            txtProductCode1.Enabled = False
            txtProductCode2.Enabled = False
            btnValidateProductCode1.Enabled = False
            btnValidateProductCode2.Enabled = False
            txtProductDescription1.Enabled = False
            txtProductDescription2.Enabled = False
            btnValidateProductDescription1.Enabled = False
            btnValidateProductDescription2.Enabled = False
        Else
            plhRelationshipEditingFields.Visible = False
            plhRelationships.Visible = False
            plhErrorMessage.Visible = True
            lblErrorMessage.Text = _wfrPage.Content("ErrorWithTheSelectedLinkId", _languageCode, True)
        End If
    End Sub

    ''' <summary>
    ''' Populate the product codes based on the descriptions in the description fields.
    ''' </summary>
    ''' <param name="productCode">The linking from product code</param>
    ''' <param name="relatingProductCode">The linking to product code</param>
    ''' <remarks></remarks>
    Private Sub populateProductCodesFromDescriptions(ByRef productCode As String, ByRef relatingProductCode As String)
        If productCode.Length = 0 AndAlso txtProductDescription1.Text.Length > 0 Then
            If ddlProductType1.SelectedValue = GlobalConstants.RETAILTYPE Then
                Dim productRelationsWithDescriptionsTable As DataTable = getRetailData()
                For Each row As DataRow In productRelationsWithDescriptionsTable.Rows
                    If row("PRODUCT_DESCRIPTION_1") = txtProductDescription1.Text Then
                        productCode = row("PRODUCT_CODE").ToString()
                        Exit For
                    End If
                Next
            ElseIf ddlProductType1.SelectedValue = GlobalConstants.TICKETINGTYPE Then
                Dim ticketingProducts As DataTable = getTicketingData()
                For Each row As DataRow In ticketingProducts.Rows
                    If row("ProductDescription") = txtProductDescription1.Text Then
                        productCode = row("ProductCode").ToString()
                        Exit For
                    End If
                Next
            End If
        End If

        If relatingProductCode.Length = 0 AndAlso txtProductDescription2.Text.Length > 0 Then
            If ddlProductType2.SelectedValue = GlobalConstants.RETAILTYPE Then
                Dim productRelationsWithDescriptionsTable As DataTable = getRetailData()
                For Each row As DataRow In productRelationsWithDescriptionsTable.Rows
                    If row("PRODUCT_DESCRIPTION_1") = txtProductDescription2.Text Then
                        relatingProductCode = row("PRODUCT_CODE").ToString()
                        Exit For
                    End If
                Next
            ElseIf ddlProductType2.SelectedValue = GlobalConstants.TICKETINGTYPE Then
                Dim ticketingProducts As DataTable = getTicketingData()
                For Each row As DataRow In ticketingProducts.Rows
                    If row("ProductDescription") = txtProductDescription2.Text Then
                        relatingProductCode = row("ProductCode").ToString()
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' Ensure the product type and sub type are valid against the product code
    ''' </summary>
    ''' <param name="productCode">The valid product code</param>
    ''' <param name="productType">The corrected product type</param>
    ''' <param name="productSubType">The corrected priduct sub type</param>
    ''' <remarks></remarks>
    Private Sub populateTicketingProductTypeAndSubType(ByVal productCode As String, ByRef productType As String, ByRef productSubType As String)
        Dim ticketingProducts As DataTable = getTicketingData()
        For Each row As DataRow In ticketingProducts.Rows
            If row("ProductCode") = productCode Then
                productType = Utilities.CheckForDBNull_String(row("ProductType"))
                productSubType = Utilities.CheckForDBNull_String(row("ProductSubType"))
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Bind the product sub type drop down list based on the selected product type
    ''' </summary>
    ''' <param name="ddlTicketingProductType">The reference to the product type drop down list</param>
    ''' <param name="ddlTicketingProductSubType">The reference to the product sub type drop down list</param>
    ''' <remarks></remarks>
    Private Sub getProductSubTypes(ByRef ddlTicketingProductType As DropDownList, ByRef ddlTicketingProductSubType As DropDownList)
        Dim ticketingProducts As DataTable = getTicketingData()
        Dim matchedRows() As DataRow = Nothing
        Dim tableToBind As New DataTable
        If ticketingProducts IsNot Nothing AndAlso ticketingProducts.Rows.Count > 0 Then
            tableToBind.Columns.Add("ID", GetType(String))
            tableToBind.Columns.Add("PRODUCT_SUB_TYPE", GetType(String))
            tableToBind.Rows.Add(NOSELECTION, _wfrPage.Content("NoSubTypeText", _languageCode, True))
            matchedRows = ticketingProducts.Select("ProductType='" & ddlTicketingProductType.SelectedValue & "'")
            For Each row As DataRow In matchedRows
                If Not String.IsNullOrWhiteSpace(row("ProductSubType")) Then tableToBind.Rows.Add(row("ProductSubType"), row("ProductSubType"))
            Next
            ddlTicketingProductSubType.DataSource = tableToBind.DefaultView.ToTable(True, "ID", "PRODUCT_SUB_TYPE")
            ddlTicketingProductSubType.DataTextField = "PRODUCT_SUB_TYPE"
            ddlTicketingProductSubType.DataValueField = "ID"
            ddlTicketingProductSubType.DataBind()
        End If
    End Sub

    ''' <summary>
    ''' Validate the product details entered and pre-populate the form elements acordingly
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub validateProductLinkingFrom(ByVal creatingLink As Boolean)
        hdfProductCodeLinkingFrom.Value = txtProductCode1.Text.ToUpper()
        Dim productCode As String = validateProductCode(txtProductCode1.Text.ToUpper())
        _productCodeFrom = productCode
        populateProductCodesFromDescriptions(productCode, String.Empty)
        If productCode.Length > 0 Then txtProductCode1.Text = productCode

        If productCode.Length > 0 Then
            If plhTicketingOptions1.Visible Then
                btnUpdateLink.OnClientClick = "return productCheckLinkingFrom(" & hdfProductCodeLinkingFrom.ClientID & ".value);"
                btnCreateNewLink.OnClientClick = btnUpdateLink.OnClientClick
                Dim productType As String = String.Empty
                Dim productSubType As String = String.Empty
                For Each row As DataRow In getTicketingData().Rows
                    If row("ProductCode").Equals(productCode) Then
                        txtProductDescription1.Text = row("ProductDescription").ToString()
                        productType = row("ProductType").ToString()
                        productSubType = row("ProductSubType").ToString()
                        Exit For
                    End If
                Next
                ddlTicketingProductType1.SelectedValue = productType
                ddlTicketingProductType1.Enabled = False
                If productSubType.Length > 0 Then
                    getProductSubTypes(ddlTicketingProductType1, ddlTicketingProductSubType1)
                    ddlTicketingProductSubType1.SelectedValue = productSubType
                Else
                    ddlTicketingProductSubType1.SelectedValue = NOSELECTION
                End If
                ddlTicketingProductSubType1.Enabled = False

                Dim showPriceCodeOptions As Boolean = (productType = GlobalConstants.AWAYPRODUCTTYPE OrElse productType = GlobalConstants.MEMBERSHIPPRODUCTTYPE OrElse productType = GlobalConstants.SEASONTICKETPRODUCTTYPE)

                If Not creatingLink Then
                    Dim dsProductDetails As DataSet = getProductDetails(productCode, productType)
                    If showPriceCodeOptions Then
                        ddlTicketingPriceCode1.Items.Clear()
                        If dsProductDetails.Tables.Contains("PriceCodes") AndAlso dsProductDetails.Tables.Contains("CampaignCodes") Then
                            Dim dtCombinedCampaignAndPriceCodes As New DataTable
                            Dim dRow As DataRow = Nothing
                            dtCombinedCampaignAndPriceCodes.Columns.Add("Codes", GetType(String))
                            For Each row As DataRow In dsProductDetails.Tables("PriceCodes").Rows
                                dRow = dtCombinedCampaignAndPriceCodes.NewRow
                                dRow("Codes") = row("PriceCode").ToString()
                                dtCombinedCampaignAndPriceCodes.Rows.Add(dRow)
                                dRow = Nothing
                            Next
                            For Each row As DataRow In dsProductDetails.Tables("CampaignCodes").Rows
                                dRow = dtCombinedCampaignAndPriceCodes.NewRow
                                dRow("Codes") = row("CampaignCode").ToString()
                                dtCombinedCampaignAndPriceCodes.Rows.Add(dRow)
                                dRow = Nothing
                            Next
                            ddlTicketingPriceCode1.DataSource = dtCombinedCampaignAndPriceCodes
                            ddlTicketingPriceCode1.DataTextField = "Codes"
                            ddlTicketingPriceCode1.DataValueField = "Codes"
                            ddlTicketingPriceCode1.DataBind()
                            If ddlTicketingPriceCode1.Items.Count = 1 AndAlso productType <> GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso productType <> GlobalConstants.HOMEPRODUCTTYPE Then
                                ddlTicketingPriceCode1.SelectedIndex = 0
                                ddlTicketingPriceCode1.Enabled = False
                            End If
                        End If
                        ddlTicketingPriceCode1.Items.Insert(0, NOSELECTION)
                    End If
                    plhTicketingPriceCode1.Visible = (showPriceCodeOptions AndAlso ddlTicketingPriceCode1.Items.Count > 1)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Validate the product details entered and pre-populae the form elements acordingly
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub validateProductLinkingTo(ByVal creatingLink As Boolean)
        hdfProductCodeLinkingTo.Value = txtProductCode2.Text.ToUpper()
        Dim productCode As String = validateProductCode(txtProductCode2.Text.ToUpper())
        populateProductCodesFromDescriptions(String.Empty, productCode)
        If productCode.Length > 0 Then txtProductCode2.Text = productCode

        If String.IsNullOrEmpty(productCode) Then
            plhErrorMessage.Visible = True
            lblErrorMessage.Text = _wfrPage.Content("NotAValidProductErrorText", _languageCode, True)
        Else
            If plhTicketingOptions2.Visible Then
                btnUpdateLink.OnClientClick = "return productCheckLinkingTo(" & hdfProductCodeLinkingTo.ClientID & ".value);"
                btnCreateNewLink.OnClientClick = btnUpdateLink.OnClientClick
                Dim productType As String = String.Empty
                Dim productSubType As String = String.Empty
                Dim productStadium As String = String.Empty
                For Each row As DataRow In getTicketingData().Rows
                    If row("ProductCode").Equals(productCode) Then
                        txtProductDescription2.Text = row("ProductDescription").ToString()
                        productType = row("ProductType").ToString()
                        productSubType = row("ProductSubType").ToString()
                        productStadium = row("ProductStadium").ToString()
                        hdfStadiumCode.Value = productStadium
                        Exit For
                    End If
                Next

                ddlTicketingProductType2.SelectedValue = productType
                ddlTicketingProductType2.Enabled = False
                If productSubType.Length > 0 Then
                    getProductSubTypes(ddlTicketingProductType2, ddlTicketingProductSubType2)
                    ddlTicketingProductSubType2.SelectedValue = productSubType
                Else
                    ddlTicketingProductSubType2.SelectedValue = NOSELECTION
                End If
                ddlTicketingProductSubType2.Enabled = False

                If Not creatingLink AndAlso ddlTicketingLinkType.SelectedValue <> GlobalConstants.PRODUCTLINKTYPEPHASE1ONLY Then
                    Dim dsProductDetails As DataSet = getProductDetails(productCode, productType)
                    Dim showPriceCodeOptions As Boolean = (productType = GlobalConstants.AWAYPRODUCTTYPE OrElse productType = GlobalConstants.MEMBERSHIPPRODUCTTYPE OrElse productType = GlobalConstants.HOMEPRODUCTTYPE OrElse productType = GlobalConstants.SEASONTICKETPRODUCTTYPE)
                    Dim showCampaignCodeOptions As Boolean = (productType = GlobalConstants.SEASONTICKETPRODUCTTYPE)
                    If showPriceCodeOptions Then
                        ddlTicketingPriceCode2.Items.Clear()
                        If dsProductDetails IsNot Nothing AndAlso dsProductDetails.Tables.Contains("PriceCodes") AndAlso dsProductDetails.Tables("PriceCodes").Rows.Count > 0 Then
                            ddlTicketingPriceCode2.DataSource = dsProductDetails.Tables("PriceCodes")
                            ddlTicketingPriceCode2.DataTextField = "PriceCode"
                            ddlTicketingPriceCode2.DataValueField = "PriceCode"
                            ddlTicketingPriceCode2.DataBind()
                            If dsProductDetails.Tables("PriceCodes").Rows.Count > 1 AndAlso (productType = GlobalConstants.AWAYPRODUCTTYPE OrElse productType = GlobalConstants.MEMBERSHIPPRODUCTTYPE) Then
                                ddlTicketingPriceCode2.Items.Insert(0, New ListItem(_wfrPage.Content("AllowCustomerToChooseOption", _languageCode, True), String.Empty))
                            End If
                        End If
                        ddlTicketingPriceCode2.Items.Insert(0, NOSELECTION)
                    Else
                        ddlTicketingPriceCode2.Items.Clear()
                    End If
                    plhTicketingPriceCode2.Visible = (showPriceCodeOptions AndAlso ddlTicketingPriceCode2.Items.Count > 1)

                    If showCampaignCodeOptions Then
                        ddlRelatedTicketingCampaignCode.Items.Clear()
                        If dsProductDetails.Tables.Contains("CampaignCodes") AndAlso dsProductDetails.Tables("CampaignCodes").Rows.Count > 0 Then
                            ddlRelatedTicketingCampaignCode.DataSource = dsProductDetails.Tables("CampaignCodes")
                            ddlRelatedTicketingCampaignCode.DataTextField = "CampaignCode"
                            ddlRelatedTicketingCampaignCode.DataValueField = "CampaignCode"
                            ddlRelatedTicketingCampaignCode.DataBind()
                        End If
                        If ddlRelatedTicketingCampaignCode.Items.Count = 1 Then
                            ddlRelatedTicketingCampaignCode.SelectedIndex = 0
                            ddlRelatedTicketingCampaignCode.Enabled = False
                        Else
                            ddlRelatedTicketingCampaignCode.Enabled = True
                        End If
                        ddlRelatedTicketingCampaignCode.Items.Insert(0, NOSELECTION)
                    Else
                        ddlRelatedTicketingCampaignCode.Items.Clear()
                    End If
                    plhRelatedTicketingCampaignCode.Visible = showCampaignCodeOptions

                    plhDefaultTicketingStadiumOptions.Visible = False
                    plhDefaultTicketingExtraTravelOptions.Visible = False
                    If productType = GlobalConstants.SEASONTICKETPRODUCTTYPE OrElse productType = GlobalConstants.HOMEPRODUCTTYPE Then
                        Dim dsStadiumData As DataSet = getStadiumDetails(productStadium)
                        If dsStadiumData.Tables.Count = 3 AndAlso dsStadiumData.Tables(1).Rows.Count > 0 Then
                            ddlDefaultTicketingStand.DataSource = getFormattedStandData(dsStadiumData.Tables(1))
                            ddlDefaultTicketingStand.DataTextField = "StandDescription"
                            ddlDefaultTicketingStand.DataValueField = "StandCode"
                            ddlDefaultTicketingStand.DataBind()
                            ddlDefaultTicketingStand.Items.Insert(0, NOSELECTION)
                            ddlDefaultTicketingArea.DataSource = getFormattedAreaData(dsStadiumData.Tables(1))
                            ddlDefaultTicketingArea.DataTextField = "AreaDescription"
                            ddlDefaultTicketingArea.DataValueField = "AreaCode"
                            ddlDefaultTicketingArea.DataBind()
                            ddlDefaultTicketingArea.Items.Insert(0, NOSELECTION)
                            plhDefaultTicketingStadiumOptions.Visible = True
                        End If
                        ddlDefaultTicketingExtraTravelOptions.Items.Clear()
                    ElseIf productType = GlobalConstants.TRAVELPRODUCTTYPE Then
                        plhDefaultTicketingExtraTravelOptions.Visible = True
                        ddlDefaultTicketingExtraTravelOptions.DataSource = getTravelDetailCodeList(productCode)
                        ddlDefaultTicketingExtraTravelOptions.DataBind()
                        If ddlTicketingLinkType.SelectedValue = GlobalConstants.PRODUCTLINKTYPEBOTH OrElse ddlTicketingLinkType.SelectedValue = GlobalConstants.PRODUCTLINKTYPEPHASE2ONLY Then
                            ddlDefaultTicketingExtraTravelOptions.Items.Insert(0, New ListItem(_wfrPage.Content("AllowCustomerToChooseOption", _languageCode, True), "0"))
                        End If
                        ddlDefaultTicketingStand.Items.Clear()
                        ddlDefaultTicketingArea.Items.Clear()
                        chkDefaultTicketingStandReadonlyOption.Checked = False
                        chkDefaultTicketingAreaReadonlyOption.Checked = False
                    Else
                        ddlDefaultTicketingStand.Items.Clear()
                        ddlDefaultTicketingArea.Items.Clear()
                        chkDefaultTicketingStandReadonlyOption.Checked = False
                        chkDefaultTicketingAreaReadonlyOption.Checked = False
                        ddlDefaultTicketingExtraTravelOptions.Items.Clear()
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Bind the repeater based on the given temp table
    ''' </summary>
    ''' <param name="rowsToBind">rows required to bind to the repeater</param>
    ''' <param name="allProductsTable">All ticketing and retail products combined with descriptions</param>
    ''' <returns>True if product relations have been bound to the repeater</returns>
    ''' <remarks></remarks>
    Private Function bindRepeater(ByVal rowsToBind As DataRow(), ByVal allProductsTable As DataTable) As Boolean
        Dim repeaterBoundSuccessfully As Boolean = False
        Try
            Dim tempTableForRepeater As New DataTable
            Dim tempRow As DataRow = Nothing
            With tempTableForRepeater.Columns
                .Add("PRODUCT_RELATIONS_ID", GetType(Integer))
                .Add("PRODUCT_CODE", GetType(String))
                .Add("TICKETING_PRODUCT_TYPE", GetType(String))
                .Add("TICKETING_PRODUCT_SUB_TYPE", GetType(String))
                .Add("TICKETING_PRODUCT_PRICE_CODE", GetType(String))
                .Add("PRODUCT_DESCRIPTION", GetType(String))
                .Add("LINKED_TO", GetType(String))
                .Add("RELATED_PRODUCT_CODE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_TYPE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_SUB_TYPE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_PRICE_CODE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE", GetType(String))
            End With
            For Each row As DataRow In rowsToBind
                tempRow = Nothing
                tempRow = tempTableForRepeater.NewRow
                tempRow("PRODUCT_RELATIONS_ID") = row("PRODUCT_RELATIONS_ID")
                tempRow("PRODUCT_CODE") = row("PRODUCT").ToString()
                tempRow("TICKETING_PRODUCT_TYPE") = row("TICKETING_PRODUCT_TYPE").ToString()
                tempRow("TICKETING_PRODUCT_SUB_TYPE") = row("TICKETING_PRODUCT_SUB_TYPE").ToString()
                tempRow("TICKETING_PRODUCT_PRICE_CODE") = row("TICKETING_PRODUCT_PRICE_CODE").ToString()
                tempRow("PRODUCT_DESCRIPTION") = getProductDescriptionByProductCode(allProductsTable, row("PRODUCT").ToString())
                tempRow("LINKED_TO") = getProductDescriptionByProductCode(allProductsTable, row("RELATED_PRODUCT").ToString())
                tempRow("RELATED_PRODUCT_CODE") = row("RELATED_PRODUCT").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_TYPE") = row("RELATED_TICKETING_PRODUCT_TYPE").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_SUB_TYPE") = row("RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_PRICE_CODE") = row("RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE") = row("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString()
                tempTableForRepeater.Rows.Add(tempRow)
            Next
            If tempTableForRepeater.Rows.Count > 0 Then
                repeaterBoundSuccessfully = True
                rptProductRelationships.DataSource = tempTableForRepeater
                rptProductRelationships.DataBind()
            End If
        Catch ex As Exception
            repeaterBoundSuccessfully = False
        End Try
        Return repeaterBoundSuccessfully
    End Function

    ''' <summary>
    ''' Retrieves the product description by the given product code
    ''' </summary>
    ''' <param name="allProductsTable">The table of product codes and descriptions</param>
    ''' <param name="productCode">The product code the description is needed for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getProductDescriptionByProductCode(ByRef allProductsTable As DataTable, ByVal productCode As String) As String
        Dim productDescription As String = String.Empty
        For Each row As DataRow In allProductsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                productDescription = row("PRODUCT_DESCRIPTION").ToString()
                Exit For
            End If
        Next
        Return productDescription
    End Function

    ''' <summary>
    ''' Get the retail data (product codes and descriptions)
    ''' </summary>
    ''' <returns>A data table of product codes and descriptions</returns>
    ''' <remarks></remarks>
    Private Shared Function getRetailData() As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        Dim productRelationsWithDescriptionsTable As DataTable = tDataObjects.ProductsSettings.TblProduct.GetAllProductDescriptionsByBUAndPartner(_businessUnit, _partner, False)
        tDataObjects = Nothing
        Return productRelationsWithDescriptionsTable
    End Function

    ''' <summary>
    ''' Get the ticketing data (product codes and descriptions)
    ''' </summary>
    ''' <returns>A data table of product codes and descriptions</returns>
    ''' <remarks></remarks>
    Private Shared Function getTicketingData() As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim feeds As New TalentFeeds
        Dim deFeeds As New DEFeeds
        Dim err As New ErrorObj
        Dim topUpProduct As String = String.Empty
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        settings.OriginatingSource = "W"
        settings.StoredProcedureGroup = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
        settings.CacheDependencyPath = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "CACHE_DEPENDENCY_PATH")
        deFeeds.Corporate_Stadium = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "CORPORATE_STADIUM")
        deFeeds.Ticketing_Stadium = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "TICKETING_STADIUM")
        deFeeds.Product_Type = "ALL"
        deFeeds.Online_Products_Only = False
        feeds.FeedsEntity = deFeeds
        settings.Cacheing = True
        feeds.Settings = settings
        err = feeds.GetXMLFeed

        If err.HasError Then
            tDataObjects = Nothing
            Return New DataTable
        Else
            If feeds.ProductsDataView IsNot Nothing Then
                topUpProduct = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "EPURSE_TOP_UP_PRODUCT_CODE")
                Dim dvFeeds As DataView = feeds.ProductsDataView
                If topUpProduct.Length > 0 Then
                    If dvFeeds.RowFilter.Length > 0 Then
                        dvFeeds.RowFilter = dvFeeds.RowFilter & " AND ProductCode <> '" & topUpProduct & "'"
                    Else
                        dvFeeds.RowFilter = "ProductCode <> '" & topUpProduct & "'"
                    End If
                End If
                tDataObjects = Nothing
                Return dvFeeds.ToTable
            Else
                tDataObjects = Nothing
                Return New DataTable
            End If
        End If
    End Function

    ''' <summary>
    ''' Combine the ticketing and retail data tables together
    ''' </summary>
    ''' <param name="retailTable">retail data table</param>
    ''' <param name="ticketingTable">ticketing data table</param>
    ''' <returns>combined data table</returns>
    ''' <remarks></remarks>
    Private Shared Function combineTicketingAndRetailTables(ByVal retailTable As DataTable, ByVal ticketingTable As DataTable) As DataTable
        Dim allProductsTable As New DataTable
        Dim dRow As DataRow = Nothing
        With allProductsTable.Columns
            .Add("PRODUCT_CODE", GetType(String))
            .Add("PRODUCT_DESCRIPTION", GetType(String))
        End With
        For Each row As DataRow In retailTable.Rows
            dRow = allProductsTable.NewRow
            dRow("PRODUCT_CODE") = row("PRODUCT_CODE").ToString()
            dRow("PRODUCT_DESCRIPTION") = row("PRODUCT_DESCRIPTION_1").ToString()
            allProductsTable.Rows.Add(dRow)
        Next
        For Each row As DataRow In ticketingTable.Rows
            dRow = allProductsTable.NewRow
            dRow("PRODUCT_CODE") = row("ProductCode").ToString()
            dRow("PRODUCT_DESCRIPTION") = row("ProductDescription").ToString()
            allProductsTable.Rows.Add(dRow)
        Next
        Return allProductsTable
    End Function

    ''' <summary>
    ''' Validate the product code entered based on the ticketing and retail data.
    ''' </summary>
    ''' <param name="productCode">The product code to validate</param>
    ''' <returns>A valid product code or a blank string</returns>
    ''' <remarks></remarks>
    Private Function validateProductCode(ByVal productCode As String) As String
        Dim newProductCode As String = String.Empty
        Dim ticketingProducts As DataTable = getTicketingData()
        For Each row As DataRow In ticketingProducts.Rows
            If productCode = row("ProductCode").ToString() Then
                newProductCode = productCode
                Exit For
            End If
        Next
        If String.IsNullOrEmpty(newProductCode) Then
            Dim productRelationsWithDescriptionsTable As DataTable = getRetailData()
            For Each row As DataRow In productRelationsWithDescriptionsTable.Rows
                If productCode = row("PRODUCT_CODE").ToString() Then
                    newProductCode = productCode
                    Exit For
                End If
            Next
        End If
        Return newProductCode
    End Function

    ''' <summary>
    ''' Get the ticketing product details for a given product code and product type
    ''' </summary>
    ''' <param name="productCode">The ticketing product code</param>
    ''' <param name="productType">The ticketing product type</param>
    ''' <returns>A dataset containing, details, price codes, price band and campaign codes</returns>
    ''' <remarks></remarks>
    Private Function getProductDetails(ByVal productCode As String, productType As String) As DataSet
        Dim deSettings As New DESettings
        Dim deProductDetails As New Talent.Common.DEProductDetails
        Dim product As New TalentProduct
        Dim err As New ErrorObj
        deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        deSettings.BusinessUnit = _businessUnit
        deSettings.StoredProcedureGroup = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
        deSettings.Cacheing = False
        deProductDetails.ProductCode = productCode
        deProductDetails.Src = GlobalConstants.SOURCE
        deProductDetails.ProductType = productType
        deProductDetails.AllowPriceException = True
        product.Settings() = deSettings
        product.De = deProductDetails

        err = product.ProductDetails
        If Not err.HasError Then
            Return product.ResultDataSet
        Else
            Return New DataSet
        End If
    End Function

    ''' <summary>
    ''' Get the ticketing stadium data for a given stadium code
    ''' </summary>
    ''' <param name="stadiumCode">The stadium code to use</param>
    ''' <returns>A dataset of stadium data</returns>
    ''' <remarks></remarks>
    Private Function getStadiumDetails(ByVal stadiumCode As String) As DataSet
        Dim deSettings As New DESettings
        Dim deProductDetails As New Talent.Common.DEProductDetails
        Dim product As New TalentProduct
        Dim err As New ErrorObj
        deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        deSettings.BusinessUnit = _businessUnit
        deSettings.StoredProcedureGroup = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
        deSettings.Cacheing = False
        deSettings.OriginatingSource = GlobalConstants.SOURCE
        deProductDetails.Src = GlobalConstants.SOURCE
        deProductDetails.StadiumCode = stadiumCode
        product.Settings() = deSettings
        product.De = deProductDetails

        err = product.StandDescriptions()
        If Not err.HasError Then
            Return product.ResultDataSet
        Else
            Return New DataSet
        End If
    End Function

    ''' <summary>
    ''' Format the given stadium datatable to unique stand code and descriptions table
    ''' </summary>
    ''' <param name="dtStadiumData">The full stadium data table</param>
    ''' <returns>A formatted table</returns>
    ''' <remarks></remarks>
    Private Function getFormattedStandData(ByVal dtStadiumData As DataTable) As DataTable
        Dim dtStandData As New DataTable
        Dim standCodeCollection As New List(Of String)
        Dim dRow As DataRow = Nothing
        With dtStandData.Columns
            .Add("StandCode", GetType(String))
            .Add("StandDescription", GetType(String))
        End With
        For Each row As DataRow In dtStadiumData.Rows
            If Not standCodeCollection.Contains(row("StandCode")) Then
                dRow = Nothing
                dRow = dtStandData.NewRow
                dRow("StandCode") = row("StandCode").ToString()
                dRow("StandDescription") = row("StandDescription").ToString()
                standCodeCollection.Add(row("StandCode").ToString())
                dtStandData.Rows.Add(dRow)
            End If
        Next
        Return dtStandData
    End Function

    ''' <summary>
    ''' Format the given stadium datatable to unique area code and descriptions table
    ''' </summary>
    ''' <param name="dtStadiumData">The full stadium data table</param>
    ''' <returns>A formatted table</returns>
    ''' <remarks></remarks>
    Private Function getFormattedAreaData(ByVal dtStadiumData As DataTable) As DataTable
        Dim dtAreaData As New DataTable
        Dim AreaCodeCollection As New List(Of String)
        Dim dRow As DataRow = Nothing
        With dtAreaData.Columns
            .Add("AreaCode", GetType(String))
            .Add("AreaDescription", GetType(String))
        End With
        For Each row As DataRow In dtStadiumData.Rows
            If Not AreaCodeCollection.Contains(row("AreaCode")) Then
                dRow = Nothing
                dRow = dtAreaData.NewRow
                dRow("AreaCode") = row("AreaCode").ToString()
                dRow("AreaDescription") = row("AreaDescription").ToString()
                AreaCodeCollection.Add(row("AreaCode").ToString())
                dtAreaData.Rows.Add(dRow)
            End If
        Next
        Return dtAreaData
    End Function

    ''' <summary>
    ''' Validate the quantity defaults entered
    ''' </summary>
    ''' <returns>True if quantity defaults are valid</returns>
    ''' <remarks></remarks>
    Private Function validateDefaultQuantitySettings() As Boolean
        Dim validQuantitySettings As Boolean = False
        If plhQuantitySettings.Visible Then
            If txtDefaultQuantity.Text.Length > 0 Then
                Dim quantity As Decimal = 0
                If Decimal.TryParse(txtDefaultQuantity.Text, quantity) Then
                    validQuantitySettings = True
                End If
            Else
                validQuantitySettings = True
            End If

            If plhDefaultQuantityMinMax.Visible AndAlso validQuantitySettings Then
                validQuantitySettings = False
                Dim minValid As Boolean = False
                Dim maxValid As Boolean = False
                Dim minQuantity As Decimal = 0
                Dim maxQuantity As Decimal = 0

                If txtDefaultQuantityMin.Text.Length > 0 Then
                    If Decimal.TryParse(txtDefaultQuantityMin.Text, minQuantity) Then
                        minValid = True
                    Else
                        validQuantitySettings = False
                    End If
                Else
                    validQuantitySettings = True
                End If

                If txtDefaultQuantityMax.Text.Length > 0 Then
                    If Decimal.TryParse(txtDefaultQuantityMax.Text, maxQuantity) Then
                        maxValid = True
                    Else
                        validQuantitySettings = False
                    End If
                Else
                    validQuantitySettings = True
                End If

                If minValid AndAlso maxValid Then
                    If minQuantity <= maxQuantity OrElse maxQuantity >= minQuantity Then
                        validQuantitySettings = True
                    End If
                End If
            End If
        Else
            validQuantitySettings = True
        End If
        Return validQuantitySettings
    End Function

    ''' <summary>
    ''' Create a type 1 relationship based on the given product code
    ''' </summary>
    ''' <param name="productCode">The given product code to relate from</param>
    ''' <param name="ticketingProductType">The given product type to relate from</param>
    ''' <param name="ticketingProductSubType">The given product sub type to relate from</param>
    ''' <returns>True if the link has been created successfully</returns>
    ''' <remarks></remarks>
    Private Function createLinkType1Relationship(ByRef productCode As String, ByRef ticketingProductType As String, ByRef ticketingProductSubType As String) As Boolean
        Dim hasError As Boolean = False
        Dim relatingProductCode As String = String.Empty
        Dim relatedTicketingProductType As String = String.Empty
        Dim relatedTicketingProductSubType As String = String.Empty
        Dim affectedRows As Integer = 0
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        settings.ConnectionStringList = Utilities.GetConnectionStringList()
        tDataObjects.Settings = settings

        productCode = validateProductCode(txtProductCode1.Text)
        relatingProductCode = validateProductCode(txtProductCode2.Text)
        If String.IsNullOrEmpty(productCode) Then txtProductCode1.Text = String.Empty
        If String.IsNullOrEmpty(relatingProductCode) Then txtProductCode2.Text = String.Empty

        If String.IsNullOrWhiteSpace(txtProductCode1.Text) And Not String.IsNullOrWhiteSpace(txtProductDescription1.Text) OrElse _
            String.IsNullOrWhiteSpace(txtProductCode2.Text) And Not String.IsNullOrWhiteSpace(txtProductDescription2.Text) Then
            populateProductCodesFromDescriptions(productCode, relatingProductCode)
        End If
        If plhTicketingOptions1.Visible Then
            ticketingProductType = ddlTicketingProductType1.SelectedValue
            If ddlTicketingProductSubType1.SelectedValue = NOSELECTION Then
                ticketingProductSubType = String.Empty
            Else
                ticketingProductSubType = ddlTicketingProductSubType1.SelectedValue
            End If
            If productCode.Length > 0 Then
                populateTicketingProductTypeAndSubType(productCode.ToUpper(), ticketingProductType, ticketingProductSubType)
                ddlTicketingProductType1.SelectedValue = ticketingProductType
                getProductSubTypes(ddlTicketingProductType1, ddlTicketingProductSubType1)
                If String.IsNullOrEmpty(ticketingProductSubType) Then
                    ddlTicketingProductSubType1.SelectedValue = NOSELECTION
                Else
                    ddlTicketingProductSubType1.SelectedValue = ticketingProductSubType
                End If
            End If
        End If
        If plhTicketingOptions2.Visible Then
            relatedTicketingProductType = ddlTicketingProductType2.SelectedValue
            If ddlTicketingProductSubType2.SelectedValue = NOSELECTION Then
                relatedTicketingProductSubType = String.Empty
            Else
                relatedTicketingProductSubType = ddlTicketingProductSubType2.SelectedValue
            End If
            If relatingProductCode.Length > 0 Then
                populateTicketingProductTypeAndSubType(relatingProductCode.ToUpper(), relatedTicketingProductType, relatedTicketingProductSubType)
                ddlTicketingProductType2.SelectedValue = relatedTicketingProductType
                getProductSubTypes(ddlTicketingProductType2, ddlTicketingProductSubType2)
                If String.IsNullOrEmpty(relatedTicketingProductSubType) Then
                    ddlTicketingProductSubType2.SelectedValue = NOSELECTION
                Else
                    ddlTicketingProductSubType2.SelectedValue = relatedTicketingProductSubType
                End If
            End If
        End If

        affectedRows = tDataObjects.ProductsSettings.TblProductRelations.CreateType1ProductRelation(_businessUnit, _partner, productCode.ToUpper(), _
                    ticketingProductType, ticketingProductSubType, relatingProductCode.ToUpper(), relatedTicketingProductType, relatedTicketingProductSubType)

        If affectedRows > 0 AndAlso chkCopyToAllBU.Checked Then
            'copy to all business units
            Dim BUTable As DataTable = tDataObjects.AppVariableSettings.TblUrlBu.GetDistinctEBusinessBUs()
            For Each businessUnit As DataRow In BUTable.Rows
                Dim businessUnitTo As String = businessUnit.Item("BUSINESS_UNIT")
                If _businessUnit <> businessUnitTo Then
                    affectedRows = tDataObjects.ProductsSettings.TblProductRelations.CreateType1ProductRelation(businessUnitTo, _partner, productCode.ToUpper(), _
                                        ticketingProductType, ticketingProductSubType, relatingProductCode.ToUpper(), relatedTicketingProductType, relatedTicketingProductSubType)
                End If
            Next
        End If

        If affectedRows = -1 Then
            hasError = True
        Else
            If chkReverseLink.Checked AndAlso productCode.Length > 0 Then
                affectedRows = tDataObjects.ProductsSettings.TblProductRelations.CreateType1ProductRelation(_businessUnit, _partner, relatingProductCode.ToUpper(), _
                        relatedTicketingProductType, relatedTicketingProductSubType, productCode.ToUpper(), ticketingProductType, ticketingProductSubType)
                If affectedRows = -1 Then
                    hasError = True
                End If
            End If
        End If
        Return hasError
    End Function

    ''' <summary>
    ''' Create a type 2 or type 0 relationship based on the given product code
    ''' </summary>
    ''' <param name="productCode">The given product code to relate from</param>
    ''' <param name="ticketingProductType">The given product type to relate from</param>
    ''' <param name="ticketingProductSubType">The given product sub type to relate from</param>
    ''' <returns>True if the link has been created successfully</returns>
    ''' <remarks></remarks>
    Private Function createLinkType2OrType3OrBothRelationship(ByRef productCode As String, ByRef ticketingProductType As String, ByRef ticketingProductSubType As String, ByRef errorMessage As String) As Boolean
        Dim hasError As Boolean = False
        Dim productRelationsID As Integer = 0
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim ticketingProductPriceCode As String = String.Empty
        Dim relatedTicketingProductCampaignCode As String = String.Empty
        Dim relatedTicketingProductSubType As String = String.Empty
        Dim relatedTicketingProductPriceCode As String = String.Empty
        Dim relatedTicketingProductPriceBand As String = String.Empty
        Dim relatedTicketingProductStand As String = String.Empty
        Dim relatedTicketingProductArea As String = String.Empty

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        settings.ConnectionStringList = Utilities.GetConnectionStringList()
        tDataObjects.Settings = settings

        productCode = txtProductCode1.Text
        ticketingProductType = ddlTicketingProductType1.SelectedValue
        If ddlTicketingProductSubType1.SelectedValue <> NOSELECTION Then ticketingProductSubType = ddlTicketingProductSubType1.SelectedValue
        If ddlTicketingPriceCode1.SelectedValue <> NOSELECTION Then ticketingProductPriceCode = ddlTicketingPriceCode1.SelectedValue
        If ddlRelatedTicketingCampaignCode.SelectedValue <> NOSELECTION Then relatedTicketingProductCampaignCode = ddlRelatedTicketingCampaignCode.SelectedValue
        If ddlTicketingProductSubType2.SelectedValue <> NOSELECTION Then relatedTicketingProductSubType = ddlTicketingProductSubType2.SelectedValue
        If ddlTicketingPriceCode2.SelectedValue <> NOSELECTION Then relatedTicketingProductPriceCode = ddlTicketingPriceCode2.SelectedValue
        If plhDefaultTicketingStadiumOptions.Visible AndAlso ddlDefaultTicketingStand.SelectedValue <> NOSELECTION Then relatedTicketingProductStand = ddlDefaultTicketingStand.SelectedValue
        If plhDefaultTicketingStadiumOptions.Visible AndAlso ddlDefaultTicketingArea.SelectedValue <> NOSELECTION Then relatedTicketingProductArea = ddlDefaultTicketingArea.SelectedValue
        Dim productDetailCode As String = String.Empty
        Dim packageComponentPriceBands As String = formatPackageComponentPriceBands()
        Dim foreignProductRelationshipID As Integer = 0
        Dim talProduct As New TalentProduct
        Dim deProductDetails As New DEProductDetails
        Dim rowCount As Integer = 0
        If plhDefaultTicketingExtraTravelOptions.Visible Then
            If ddlDefaultTicketingExtraTravelOptions.SelectedValue <> "0" Then
                productDetailCode = TCUtilities.FixStringLength(ddlDefaultTicketingExtraTravelOptions.SelectedValue, 7)
                If ddlTicketingLinkType.SelectedValue <> 3 Then
                    relatedTicketingProductStand = productDetailCode.Substring(0, 3).Trim().ToUpper()
                    relatedTicketingProductArea = productDetailCode.Substring(3, 4).Trim().ToUpper()
                End If
            End If
        End If


        rowCount = tDataObjects.ProductsSettings.TblProductRelations.CheckForDuplicateRelationships(_businessUnit, _partner, ddlTicketingLinkType.SelectedValue, _
                        productCode, ticketingProductType, ticketingProductSubType, ticketingProductPriceCode, relatedTicketingProductCampaignCode, _
                        txtProductCode2.Text, ddlTicketingProductType2.SelectedValue, relatedTicketingProductSubType, relatedTicketingProductPriceCode)

        If ddlTicketingLinkType.SelectedValue = 3 Then
            If rowCount = 0 Then
                Dim numberOfMasterProductOfMasterProduct As Integer = tDataObjects.ProductsSettings.TblProductRelations.GetMasterProductCountByMasterProductCode(_businessUnit, _partner, productCode, ddlTicketingLinkType.SelectedValue)
                Dim err As New ErrorObj

                With deProductDetails
                    .ProductRelationsID = productRelationsID
                    .MasterPackageProduct = productCode
                    .RelatedProduct = txtProductCode2.Text
                    .MasterPriceCode = ticketingProductPriceCode
                    .PriceCode = relatedTicketingProductPriceCode
                    .StandCode = relatedTicketingProductStand
                    .AreaCode = relatedTicketingProductArea
                    .NumOfMasterProducts = numberOfMasterProductOfMasterProduct
                    .Quantity = If(String.IsNullOrEmpty(txtDefaultQuantity.Text), 1, CInt(txtDefaultQuantity.Text))
                    .Src = GlobalConstants.SOURCE
                    .CampaignCode = relatedTicketingProductCampaignCode
                    .ProductDetailID = productDetailCode
                    .PackageComponentValue1 = Utilities.ConvertStringToDecimal(txtComponentValue1.Text)
                    .PackageComponentValue2 = Utilities.ConvertStringToDecimal(txtComponentValue2.Text)
                    .PackageComponentValue3 = Utilities.ConvertStringToDecimal(txtComponentValue3.Text)
                    .PackageComponentValue4 = Utilities.ConvertStringToDecimal(txtComponentValue4.Text)
                    .PackageComponentValue5 = Utilities.ConvertStringToDecimal(txtComponentValue5.Text)
                    .PackageComponentPriceBands = packageComponentPriceBands
                End With
                talProduct.Settings = settings
                talProduct.De = deProductDetails
                err = talProduct.CreateLinkedProductPackage()

                If err.HasError OrElse talProduct.ResultDataSet Is Nothing OrElse talProduct.ResultDataSet.Tables("Error Status").Rows.Count = 0 OrElse talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0).ToString().Trim().Length > 0 Then
                    If talProduct.ResultDataSet Is Nothing Then
                        errorMessage = "ErrorCreatingRelationshipText"
                    Else
                        If talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "DR" Then
                            errorMessage = "RelationshipAlreadyExistsText"
                        ElseIf talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "MF" Then
                            errorMessage = "RelationshipMissingFieldText"
                        ElseIf talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "LC" Then
                            errorMessage = "DataErrorText"
                        ElseIf talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "IP" Then
                            errorMessage = "InvalidPackageText"
                        Else
                            errorMessage = "ErrorCreatingRelationshipText"
                        End If
                    End If
                    hasError = True
                End If

                If Not hasError AndAlso talProduct.ResultDataSet IsNot Nothing AndAlso talProduct.ResultDataSet.Tables("Relationship Details").Rows.Count > 0 Then
                    foreignProductRelationshipID = CInt(talProduct.ResultDataSet.Tables("Relationship Details").Rows(0)(0).ToString())
                End If
            Else
                hasError = True
                errorMessage = "RelationshipAlreadyExistsText"
            End If
        End If


        If Not hasError Then
            If rowCount = 0 Then
                productRelationsID = tDataObjects.ProductsSettings.TblProductRelations.CreateType2OrType3OrBothProductRelation(_businessUnit, _partner, ddlTicketingLinkType.SelectedValue, _
                      productCode, ticketingProductType, ticketingProductSubType, ticketingProductPriceCode, relatedTicketingProductCampaignCode, txtProductCode2.Text, ddlTicketingProductType2.SelectedValue, _
                      relatedTicketingProductSubType, chkMandatoryProduct.Checked, relatedTicketingProductPriceCode, relatedTicketingProductStand, chkDefaultTicketingStandReadonlyOption.Checked, relatedTicketingProductArea, _
                      chkDefaultTicketingAreaReadonlyOption.Checked, txtDefaultQuantity.Text.Trim(), txtDefaultQuantityMin.Text.Trim(), txtDefaultQuantityMax.Text.Trim(), _
                      chkDefaultQuantityReadonlyOption.Checked, chkQuantityDefintion.Checked, chkQuantityRoundUp.Checked, txtCssClass.Text.Trim(), txtInstructions.Text.Trim(), _
                      foreignProductRelationshipID, Utilities.ConvertStringToDecimal(txtComponentValue1.Text), Utilities.ConvertStringToDecimal(txtComponentValue2.Text), Utilities.ConvertStringToDecimal(txtComponentValue3.Text), _
                      Utilities.ConvertStringToDecimal(txtComponentValue4.Text), Utilities.ConvertStringToDecimal(txtComponentValue5.Text), formatPackageComponentPriceBandsCommaSeperated())

                If productRelationsID > 0 AndAlso chkCopyToAllBU.Checked Then
                    'copy to all business units
                    Dim BUTable As DataTable = tDataObjects.AppVariableSettings.TblUrlBu.GetDistinctEBusinessBUs()
                    For Each businessUnit As DataRow In BUTable.Rows
                        Dim businessUnitTo As String = businessUnit.Item("BUSINESS_UNIT")
                        If _businessUnit <> businessUnitTo Then
                            tDataObjects.ProductsSettings.TblProductRelations.CreateType2OrType3OrBothProductRelation(businessUnitTo, _partner, ddlTicketingLinkType.SelectedValue, _
                                  productCode, ticketingProductType, ticketingProductSubType, ticketingProductPriceCode, relatedTicketingProductCampaignCode, txtProductCode2.Text, ddlTicketingProductType2.SelectedValue, _
                                  relatedTicketingProductSubType, chkMandatoryProduct.Checked, relatedTicketingProductPriceCode, relatedTicketingProductStand, chkDefaultTicketingStandReadonlyOption.Checked, relatedTicketingProductArea, _
                                  chkDefaultTicketingAreaReadonlyOption.Checked, txtDefaultQuantity.Text.Trim(), txtDefaultQuantityMin.Text.Trim(), txtDefaultQuantityMax.Text.Trim(), _
                                  chkDefaultQuantityReadonlyOption.Checked, chkQuantityDefintion.Checked, chkQuantityRoundUp.Checked, txtCssClass.Text.Trim(), txtInstructions.Text.Trim(), _
                                  foreignProductRelationshipID, Utilities.ConvertStringToDecimal(txtComponentValue1.Text), Utilities.ConvertStringToDecimal(txtComponentValue2.Text), Utilities.ConvertStringToDecimal(txtComponentValue3.Text), _
                                  Utilities.ConvertStringToDecimal(txtComponentValue4.Text), Utilities.ConvertStringToDecimal(txtComponentValue5.Text), formatPackageComponentPriceBandsCommaSeperated())
                        End If
                    Next
                End If

                If productRelationsID = -1 AndAlso ddlTicketingLinkType.SelectedValue = 3 Then
                    With deProductDetails
                        .ProductRelationsID = foreignProductRelationshipID
                    End With
                    talProduct.Settings = settings
                    talProduct.De = deProductDetails
                    talProduct.DeleteLinkedProductPackage()
                    errorMessage = "ErrorCreatingRelationshipText"
                End If
            Else
                hasError = True
                errorMessage = "RelationshipAlreadyExistsText"
            End If         
        End If
        Return hasError
    End Function

    ''' <summary>
    ''' Update an existing product link
    ''' </summary>
    ''' <param name="productCode">The product code linking from</param>
    ''' <param name="ticketingProductType">The ticketing product type linking from</param>
    ''' <param name="ticketingProductSubType">The ticketing product sub type linking from</param>
    ''' <returns>The number of affected records</returns>
    ''' <remarks></remarks>
    Private Function updateExistingProductRelationship(ByRef productCode As String, ByRef ticketingProductType As String, ByRef ticketingProductSubType As String, ByRef errorMessage As String) As Boolean
        Dim hasError As Boolean = False
        Dim affectedRows As Integer = 0
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim ticketingProductPriceCode As String = String.Empty
        Dim relatedTicketingProductCampaignCode As String = String.Empty
        Dim relatedTicketingProductSubType As String = String.Empty
        Dim relatedTicketingProductPriceCode As String = String.Empty
        Dim relatedTicketingProductPriceBand As String = String.Empty
        Dim relatedTicketingProductStand As String = String.Empty
        Dim relatedTicketingProductArea As String = String.Empty
        Dim linkId As String = Request.QueryString("id")

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        settings.ConnectionStringList = Utilities.GetConnectionStringList()
        tDataObjects.Settings = settings

        productCode = txtProductCode1.Text
        ticketingProductType = ddlTicketingProductType1.SelectedValue
        If ddlTicketingProductSubType1.SelectedValue <> NOSELECTION Then ticketingProductSubType = ddlTicketingProductSubType1.SelectedValue
        If ddlTicketingPriceCode1.SelectedValue <> NOSELECTION Then ticketingProductPriceCode = ddlTicketingPriceCode1.SelectedValue
        If ddlRelatedTicketingCampaignCode.SelectedValue <> NOSELECTION Then relatedTicketingProductCampaignCode = ddlRelatedTicketingCampaignCode.SelectedValue
        If ddlTicketingProductSubType2.SelectedValue <> NOSELECTION Then relatedTicketingProductSubType = ddlTicketingProductSubType2.SelectedValue
        If ddlTicketingPriceCode2.SelectedValue <> NOSELECTION Then relatedTicketingProductPriceCode = ddlTicketingPriceCode2.SelectedValue
        If plhDefaultTicketingStadiumOptions.Visible AndAlso ddlDefaultTicketingStand.SelectedValue <> NOSELECTION Then relatedTicketingProductStand = ddlDefaultTicketingStand.SelectedValue
        If plhDefaultTicketingStadiumOptions.Visible AndAlso ddlDefaultTicketingArea.SelectedValue <> NOSELECTION Then relatedTicketingProductArea = ddlDefaultTicketingArea.SelectedValue
        Dim productDetailCode As String = String.Empty
        Dim packageComponentPriceBands As String = formatPackageComponentPriceBands()


        If plhDefaultTicketingExtraTravelOptions.Visible Then
            If ddlDefaultTicketingExtraTravelOptions.SelectedValue <> "0" Then
                productDetailCode = TCUtilities.FixStringLength(ddlDefaultTicketingExtraTravelOptions.SelectedValue, 7)
                If ddlTicketingLinkType.SelectedValue <> 3 Then
                    relatedTicketingProductStand = productDetailCode.Substring(0, 3).Trim().ToUpper()
                    relatedTicketingProductArea = productDetailCode.Substring(3, 4).Trim().ToUpper()
                End If
            End If
        End If
        Dim foreignProductRelationsID As String = tDataObjects.ProductsSettings.TblProductRelations.GetForiegnProductRelationsID(linkId)
        If ddlTicketingLinkType.SelectedValue = GlobalConstants.PRODUCTLINKTYPEPHASE3ONLY Then
            Dim numberOfMasterProductOfMasterProduct As Integer = tDataObjects.ProductsSettings.TblProductRelations.GetMasterProductCountByMasterProductCode(_businessUnit, _partner, productCode, ddlTicketingLinkType.SelectedValue)
            Dim talProduct As New TalentProduct
            Dim deProductDetails As New DEProductDetails
            With deProductDetails
                .ProductRelationsID = foreignProductRelationsID
                .MasterPackageProduct = productCode
                .RelatedProduct = txtProductCode2.Text
                .PriceCode = relatedTicketingProductPriceCode
                .StandCode = relatedTicketingProductStand
                .NumOfMasterProducts = numberOfMasterProductOfMasterProduct
                .AreaCode = relatedTicketingProductArea
                .Quantity = If(String.IsNullOrEmpty(txtDefaultQuantity.Text), 1, CInt(txtDefaultQuantity.Text))
                .Src = GlobalConstants.SOURCE
                .CampaignCode = relatedTicketingProductCampaignCode
                .ProductDetailID = productDetailCode
                .PackageComponentValue1 = Utilities.ConvertStringToDecimal(txtComponentValue1.Text)
                .PackageComponentValue2 = Utilities.ConvertStringToDecimal(txtComponentValue2.Text)
                .PackageComponentValue3 = Utilities.ConvertStringToDecimal(txtComponentValue3.Text)
                .PackageComponentValue4 = Utilities.ConvertStringToDecimal(txtComponentValue4.Text)
                .PackageComponentValue5 = Utilities.ConvertStringToDecimal(txtComponentValue5.Text)
                .PackageComponentPriceBands = packageComponentPriceBands
            End With
            talProduct.Settings = settings
            talProduct.De = deProductDetails
            talProduct.UpdateLinkedProductPackage()
            If talProduct.ResultDataSet Is Nothing OrElse talProduct.ResultDataSet.Tables(0).Rows.Count = 0 OrElse talProduct.ResultDataSet.Tables(0).Rows(0)(0).ToString().Trim().Length > 0 Then
                hasError = True
                If talProduct.ResultDataSet Is Nothing Then
                    errorMessage = "iSeriesRelationshipNotUpdatedText"
                    hasError = True
                Else
                    If talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "DR" Then
                        errorMessage = "RelationshipAlreadyExistsText"
                    ElseIf talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "MF" Then
                        errorMessage = "RelationshipMissingFieldText"
                    ElseIf talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "LC" Then
                        errorMessage = "DataErrorText"
                    ElseIf talProduct.ResultDataSet.Tables("Error Status").Rows(0)(0) = "IP" Then
                        errorMessage = "InvalidPackageText"
                    Else
                        errorMessage = "ErrorCreatingRelationshipText"
                    End If
                End If
            End If
        End If

        If Not hasError Then
            If foreignProductRelationsID > 0 AndAlso chkCopyToAllBU.Checked Then
                affectedRows = tDataObjects.ProductsSettings.TblProductRelations.UpateProductRelationByForeignId(foreignProductRelationsID, ddlTicketingLinkType.SelectedValue, _
                        productCode, ticketingProductType, ticketingProductSubType, ticketingProductPriceCode, relatedTicketingProductCampaignCode, txtProductCode2.Text, ddlTicketingProductType2.SelectedValue, _
                        relatedTicketingProductSubType, chkMandatoryProduct.Checked, relatedTicketingProductPriceCode, relatedTicketingProductStand, chkDefaultTicketingStandReadonlyOption.Checked, relatedTicketingProductArea, _
                        chkDefaultTicketingAreaReadonlyOption.Checked, txtDefaultQuantity.Text.Trim(), txtDefaultQuantityMin.Text.Trim(), txtDefaultQuantityMax.Text.Trim(), _
                        chkDefaultQuantityReadonlyOption.Checked, chkQuantityDefintion.Checked, chkQuantityRoundUp.Checked, txtCssClass.Text.Trim(), txtInstructions.Text.Trim(), Utilities.ConvertStringToDecimal(txtComponentValue1.Text), _
                        Utilities.ConvertStringToDecimal(txtComponentValue2.Text), Utilities.ConvertStringToDecimal(txtComponentValue3.Text), Utilities.ConvertStringToDecimal(txtComponentValue4.Text), Utilities.ConvertStringToDecimal(txtComponentValue5.Text), formatPackageComponentPriceBandsCommaSeperated())
            Else
                affectedRows = tDataObjects.ProductsSettings.TblProductRelations.UpateProductRelationById(linkId, ddlTicketingLinkType.SelectedValue, _
                        productCode, ticketingProductType, ticketingProductSubType, ticketingProductPriceCode, relatedTicketingProductCampaignCode, txtProductCode2.Text, ddlTicketingProductType2.SelectedValue, _
                        relatedTicketingProductSubType, chkMandatoryProduct.Checked, relatedTicketingProductPriceCode, relatedTicketingProductStand, chkDefaultTicketingStandReadonlyOption.Checked, relatedTicketingProductArea, _
                        chkDefaultTicketingAreaReadonlyOption.Checked, txtDefaultQuantity.Text.Trim(), txtDefaultQuantityMin.Text.Trim(), txtDefaultQuantityMax.Text.Trim(), _
                        chkDefaultQuantityReadonlyOption.Checked, chkQuantityDefintion.Checked, chkQuantityRoundUp.Checked, txtCssClass.Text.Trim(), txtInstructions.Text.Trim(), Utilities.ConvertStringToDecimal(txtComponentValue1.Text), _
                        Utilities.ConvertStringToDecimal(txtComponentValue2.Text), Utilities.ConvertStringToDecimal(txtComponentValue3.Text), Utilities.ConvertStringToDecimal(txtComponentValue4.Text), Utilities.ConvertStringToDecimal(txtComponentValue5.Text), formatPackageComponentPriceBandsCommaSeperated())
            End If
            If affectedRows < 1 Then
                hasError = True
                errorMessage = "RelationshipNotUpdatedText"
            End If
        End If
        Return hasError
    End Function

    Private Function getTravelDetailCodeList(ByVal productCode As String) As List(Of String)
        Dim travelDetailCodeList As New List(Of String)
        Dim tDataObjects = New TalentDataObjects()
        Dim tProduct As New TalentProduct
        Dim productDataEntity As New DEProductDetails
        Dim settings As DESettings = New DESettings()
        Dim err As New ErrorObj
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = _businessUnit
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        tDataObjects.Settings = settings
        settings.OriginatingSource = GlobalConstants.SOURCE
        settings.StoredProcedureGroup = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
        productDataEntity.StadiumCode = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "TICKETING_STADIUM")
        productDataEntity.ProductType = GlobalConstants.TRAVELPRODUCTTYPE
        tProduct.De = productDataEntity
        tProduct.Settings = settings
        err = tProduct.ProductList()

        If Not err.HasError AndAlso tProduct.ResultDataSet IsNot Nothing AndAlso tProduct.ResultDataSet.Tables.Count = 2 AndAlso tProduct.ResultDataSet.Tables(1).Rows.Count > 0 Then
            For Each row As DataRow In tProduct.ResultDataSet.Tables(1).Rows
                If productCode = row("ProductCode").ToString.Trim Then
                    If Not travelDetailCodeList.Contains(row("ProductDetailCode")) Then
                        travelDetailCodeList.Add(row("ProductDetailCode").ToString().Trim())
                    End If
                End If
            Next
        End If
        Return travelDetailCodeList
    End Function
    ''' <summary>
    ''' Validates component values and their corresponding price bands
    ''' </summary>
    ''' <param name="errorMessageText"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AreComponentValuesValidated(ByRef errorMessageText As String) As Boolean
        Dim errorPriceBand As Boolean = False
        If ddlTicketingLinkType.SelectedValue = 3 Then
            Dim PriceBands(4) As String
            Dim componentValue As Decimal
            PriceBands(0) = txtComponentPriceBand1.Text
            PriceBands(1) = txtComponentPriceBand2.Text
            PriceBands(2) = txtComponentPriceBand3.Text
            PriceBands(3) = txtComponentPriceBand4.Text
            PriceBands(4) = txtComponentPriceBand5.Text

            'Empty component value price band settings skips all validation
            If Not (String.IsNullOrEmpty(PriceBands(0)) AndAlso String.IsNullOrEmpty(PriceBands(1)) AndAlso String.IsNullOrEmpty(PriceBands(2)) _
                AndAlso String.IsNullOrEmpty(PriceBands(3)) AndAlso String.IsNullOrEmpty(PriceBands(4)) _
                AndAlso txtComponentValue1.Text.Equals(ZERODOTZEROZERO) AndAlso txtComponentValue2.Text.Equals(ZERODOTZEROZERO) _
                AndAlso txtComponentValue3.Text.Equals(ZERODOTZEROZERO) AndAlso txtComponentValue4.Text.Equals(ZERODOTZEROZERO) _
                AndAlso txtComponentValue5.Text.Equals(ZERODOTZEROZERO)) Then

                'Checks for a rouge empty price band in the list of price bands
                If Not String.IsNullOrEmpty(PriceBands(0)) AndAlso String.IsNullOrEmpty(errorMessageText) Then
                    Dim isEmptyPriceBand As Boolean = False
                    For i As Integer = 1 To 4
                        If String.IsNullOrEmpty(PriceBands(i)) Then
                            isEmptyPriceBand = True
                        Else
                            If isEmptyPriceBand Then
                                errorMessageText = "PriceBandEmptyError"
                                errorPriceBand = True
                                Exit For
                            End If
                        End If
                    Next
                End If



                'Makes sure each component has a corresponding price band and validates for "firstLineErrors"
                If Not errorPriceBand AndAlso String.IsNullOrEmpty(errorMessageText) Then
                    Dim firstLineSafe As Boolean = False
                    If Decimal.TryParse(txtComponentValue1.Text, componentValue) AndAlso componentValue >= 0 AndAlso PriceBands(0).ToString().Length = 0 Then firstLineSafe = True
                    If Not Decimal.TryParse(txtComponentValue1.Text, componentValue) OrElse componentValue < 0 OrElse PriceBands(0).ToString().Length = 0 Then errorPriceBand = True
                    If errorPriceBand OrElse Not Decimal.TryParse(txtComponentValue2.Text, componentValue) OrElse componentValue < 0 OrElse PriceBands(1).ToString().Length < 0 Then errorPriceBand = True
                    If errorPriceBand OrElse Not Decimal.TryParse(txtComponentValue3.Text, componentValue) OrElse componentValue < 0 OrElse PriceBands(2).ToString().Length < 0 Then errorPriceBand = True
                    If errorPriceBand OrElse Not Decimal.TryParse(txtComponentValue4.Text, componentValue) OrElse componentValue < 0 OrElse PriceBands(3).ToString().Length < 0 Then errorPriceBand = True
                    If errorPriceBand OrElse Not Decimal.TryParse(txtComponentValue5.Text, componentValue) OrElse componentValue < 0 OrElse PriceBands(4).ToString().Length < 0 Then errorPriceBand = True

                    'An empty price band setting is allowed on the first line if there is a component value setting on the first line and all other lines have are "empty"

                    If firstLineSafe Then
                        If Not (String.IsNullOrEmpty(PriceBands(1)) AndAlso String.IsNullOrEmpty(PriceBands(2)) _
                            AndAlso String.IsNullOrEmpty(PriceBands(3)) AndAlso String.IsNullOrEmpty(PriceBands(4)) _
                            AndAlso txtComponentValue2.Text.Equals(ZERODOTZEROZERO) AndAlso txtComponentValue3.Text.Equals(ZERODOTZEROZERO) _
                            AndAlso txtComponentValue4.Text.Equals(ZERODOTZEROZERO) AndAlso txtComponentValue5.Text.Equals(ZERODOTZEROZERO)) Then
                            errorMessageText = "FirstLineError"
                            firstLineSafe = False
                        Else
                            errorPriceBand = False
                        End If
                    End If

                    If errorPriceBand AndAlso String.IsNullOrEmpty(errorMessageText) Then
                        errorMessageText = "ComponentValuePriceBandError"
                    End If
                End If

                'Makes sure that price band is between A and Z
                If Not errorPriceBand AndAlso String.IsNullOrEmpty(errorMessageText) Then
                    If Not (Regex.IsMatch(PriceBands(0).ToString(), "^[a-zA-Z0-9]+$") OrElse String.IsNullOrEmpty(PriceBands(0).ToString())) _
                        OrElse Not (Regex.IsMatch(PriceBands(1).ToString(), "^[a-zA-Z0-9]+$") OrElse String.IsNullOrEmpty(PriceBands(1).ToString())) _
                        OrElse Not (Regex.IsMatch(PriceBands(2).ToString(), "^[a-zA-Z0-9]+$") OrElse String.IsNullOrEmpty(PriceBands(2).ToString())) _
                        OrElse Not (Regex.IsMatch(PriceBands(3).ToString(), "^[a-zA-Z0-9]+$") OrElse String.IsNullOrEmpty(PriceBands(3).ToString())) _
                        OrElse Not (Regex.IsMatch(PriceBands(4).ToString(), "^[a-zA-Z0-9]+$") OrElse String.IsNullOrEmpty(PriceBands(4).ToString())) Then
                        errorPriceBand = True
                        errorMessageText = "PriceBandOutOfBoundsError"
                    End If
                End If

                'Makes sure the list of price bands only contains unique values
                If Not errorPriceBand AndAlso String.IsNullOrEmpty(errorMessageText) Then
                    For i As Integer = 0 To 4
                        For priceBandCount As Integer = 0 To 4
                            If i <> priceBandCount AndAlso Not String.IsNullOrEmpty(PriceBands(i)) Then
                                If PriceBands(i).ToString().Equals(PriceBands(priceBandCount)) Then
                                    errorPriceBand = True
                                    Exit For
                                End If
                            End If
                        Next
                        If errorPriceBand Then
                            Exit For
                        End If
                    Next
                    If errorPriceBand Then
                        errorMessageText = "PriceBandNonUniquePriceBandError"
                    End If
                End If
            End If
        End If
        Return Not errorPriceBand
    End Function

    Private Function formatPackageComponentPriceBands() As String
        Dim packageComponentPriceBands As StringBuilder = New StringBuilder()
        If Not (String.IsNullOrEmpty(txtComponentPriceBand1.Text) AndAlso String.IsNullOrEmpty(txtComponentPriceBand2.Text) AndAlso _
             String.IsNullOrEmpty(txtComponentPriceBand3.Text) AndAlso String.IsNullOrEmpty(txtComponentPriceBand4.Text) AndAlso _
             String.IsNullOrEmpty(txtComponentPriceBand5.Text)) Then
            If Not String.IsNullOrEmpty(txtComponentPriceBand1.Text) Then
                packageComponentPriceBands.Append(txtComponentPriceBand1.Text)
                If Not String.IsNullOrEmpty(txtComponentPriceBand2.Text) Then
                    packageComponentPriceBands.Append(txtComponentPriceBand2.Text)
                    If Not String.IsNullOrEmpty(txtComponentPriceBand3.Text) Then
                        packageComponentPriceBands.Append(txtComponentPriceBand3.Text)
                        If Not String.IsNullOrEmpty(txtComponentPriceBand4.Text) Then
                            packageComponentPriceBands.Append(txtComponentPriceBand4.Text)
                            If Not String.IsNullOrEmpty(txtComponentPriceBand5.Text) Then
                                packageComponentPriceBands.Append(txtComponentPriceBand5.Text)
                            End If
                        End If
                    End If
                End If
            End If
        End If

        Return packageComponentPriceBands.ToString()
    End Function

    Private Function formatPackageComponentPriceBandsCommaSeperated() As String
        Dim packageComponentPriceBands As StringBuilder = New StringBuilder()
        If Not (String.IsNullOrEmpty(txtComponentPriceBand1.Text) AndAlso String.IsNullOrEmpty(txtComponentPriceBand2.Text) AndAlso _
           String.IsNullOrEmpty(txtComponentPriceBand3.Text) AndAlso String.IsNullOrEmpty(txtComponentPriceBand4.Text) AndAlso _
           String.IsNullOrEmpty(txtComponentPriceBand5.Text)) Then
            packageComponentPriceBands.Append(txtComponentPriceBand1.Text).Append(COMMASEPERATED).
                       Append(txtComponentPriceBand2.Text).Append(COMMASEPERATED).
                       Append(txtComponentPriceBand3.Text).Append(COMMASEPERATED).
                       Append(txtComponentPriceBand4.Text).Append(COMMASEPERATED).
                       Append(txtComponentPriceBand5.Text)
        End If
        Return packageComponentPriceBands.ToString()
    End Function
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the list of product descriptions for the auto complete extender
    ''' </summary>
    ''' <param name="prefixText">The text to filter on</param>
    ''' <param name="count">The number of items in the list</param>
    ''' <param name="contextKey">Retail or ticketing type</param>
    ''' <returns>List of strings to use</returns>
    ''' <remarks></remarks>
    <System.Web.Script.Services.ScriptMethod(), System.Web.Services.WebMethod()> _
    Public Shared Function GetProductDescList(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As List(Of String)
        Dim productDescList As List(Of String) = New List(Of String)
        Dim retailTable As New DataTable
        Dim ticketingTable As New DataTable
        If contextKey = GlobalConstants.RETAILTYPE Then
            retailTable = getRetailData()
        ElseIf contextKey = GlobalConstants.TICKETINGTYPE Then
            ticketingTable = getTicketingData()
        End If
        Dim allProductsTable As DataTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
        If allProductsTable IsNot Nothing AndAlso allProductsTable.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            If prefixText.StartsWith(STARTSYMBOL) Then
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_DESCRIPTION").Select("PRODUCT_DESCRIPTION LIKE'%" & prefixText.Replace(STARTSYMBOL, "") & "%'")
            Else
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_DESCRIPTION").Select("PRODUCT_DESCRIPTION LIKE'%" & prefixText & "%'")
            End If
            If matchedRows.Length > 0 Then
                Dim itemsToBind As Integer = matchedRows.Length - 1
                If count < matchedRows.Length - 1 Then itemsToBind = count - 1
                For rowIndex As Integer = 0 To itemsToBind
                    productDescList.Add(matchedRows(rowIndex)("PRODUCT_DESCRIPTION").ToString())
                Next
            Else
                productDescList.Add(NothingFoundText)
            End If
        End If
        Return productDescList
    End Function

    ''' <summary>
    ''' Get the list of product codes for the auto complete extender
    ''' </summary>
    ''' <param name="prefixText">The text to filter on</param>
    ''' <param name="count">The number of items in the list</param>
    ''' <param name="contextKey">Retail or ticketing type</param>
    ''' <returns>List of strings to use</returns>
    ''' <remarks></remarks>
    <System.Web.Script.Services.ScriptMethod(), System.Web.Services.WebMethod()> _
    Public Shared Function GetProductCodeList(ByVal prefixText As String, ByVal count As Integer, ByVal contextKey As String) As List(Of String)
        Dim productCodeList As List(Of String) = New List(Of String)
        Dim retailTable As New DataTable
        Dim ticketingTable As New DataTable
        If contextKey = GlobalConstants.RETAILTYPE Then
            retailTable = getRetailData()
        ElseIf contextKey = GlobalConstants.TICKETINGTYPE Then
            ticketingTable = getTicketingData()
        End If
        Dim allProductsTable As DataTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
        If allProductsTable IsNot Nothing AndAlso allProductsTable.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            If prefixText.StartsWith(STARTSYMBOL) Then
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_CODE").Select("PRODUCT_CODE LIKE'%" & prefixText.Replace(STARTSYMBOL, "") & "%'")
            Else
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_CODE").Select("PRODUCT_CODE LIKE'%" & prefixText & "%'")
            End If
            If matchedRows.Length > 0 Then
                Dim itemsToBind As Integer = matchedRows.Length - 1
                If count < matchedRows.Length - 1 Then itemsToBind = count - 1
                For rowIndex As Integer = 0 To itemsToBind
                    productCodeList.Add(matchedRows(rowIndex)("PRODUCT_CODE").ToString())
                Next
            Else
                productCodeList.Add(NothingFoundText)
            End If
        End If
        Return productCodeList
    End Function

    ''' <summary>
    ''' Format 2 strings with a forward slash if there are two strings
    ''' </summary>
    ''' <param name="string1">string 1</param>
    ''' <param name="string2">string 2</param>
    ''' <returns>Formatted concatenated string with a slash between the two</returns>
    ''' <remarks></remarks>
    Public Function Format2Strings(ByVal string1 As String, ByVal string2 As String) As String
        Dim formattedString As String = String.Empty
        Dim separator As String = "/"
        If string1.Length > 0 AndAlso string2.Length > 0 Then
            formattedString = string1 & separator & string2
        Else
            If string1.Length > 0 Then formattedString = string1
            If string2.Length > 0 Then formattedString = string2
        End If
        Return formattedString
    End Function

    ''' <summary>
    ''' If the product is no longer available, due to it no longer being for sale or has expired there is no description shown,
    ''' therefore show a generic description. No desciption is returned when the product is available.
    ''' When there is no product code, we have to assume that it is a link that has been created based on product type or sub type
    ''' therefore show an "N/A" type message.
    ''' </summary>
    ''' <param name="productDescription">The current product description</param>
    ''' <param name="productCode">The product code used for display purposes</param>
    ''' <returns>The new product description</returns>
    ''' <remarks></remarks>
    Public Function GetDescription(ByVal productDescription As String, ByVal productCode As String) As String
        Dim newProductDescription As String = _wfrPage.Content("ProductNotAvailableText", _languageCode, True).Replace("<<PRODUCT_CODE>>", productCode)
        If productDescription.Length > 0 Then
            newProductDescription = String.Empty
        End If
        If productCode.Length = 0 Then
            newProductDescription = _wfrPage.Content("NoProductCodeSpecifiedText", _languageCode, True)
        End If
        Return newProductDescription
    End Function

#End Region

End Class