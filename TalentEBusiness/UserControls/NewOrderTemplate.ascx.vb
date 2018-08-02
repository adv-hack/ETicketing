Imports Talent.Common

Partial Class UserControls_NewOrderTemplate
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Utilities.GetDefaultLanguage
    Private _ucr As New UserControlResource
    Private _pds As New PagedDataSource

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "NewOrderTemplate.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            setText()
            Dim DE_OrdTemplates As New DEOrderTemplates("SELECT")
            Dim ordTemplate As New DEOrderTemplate(Profile.UserName, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), Now, Now, Now)
            DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
            Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
            DB_OrdTemplates.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            DB_OrdTemplates.AccessDatabase()

            Dim currentTemplates As Data.DataTable = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesHeader")
            If currentTemplates.Rows.Count > 0 Then
                ddlSelectTemplate.DataSource = currentTemplates
                ddlSelectTemplate.DataTextField = "NAME"
                ddlSelectTemplate.DataValueField = "TEMPLATE_HEADER_ID"
                ddlSelectTemplate.DataBind()
                plhUpdateTemplate.Visible = True
            Else
                plhUpdateTemplate.Visible = False
            End If
        End If
    End Sub

    Protected Sub UpdateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateButton.Click
        errLabel.Text = String.Empty
        If Not Profile.Basket.IsEmpty Then

            'Retrieve the information on the currently selected template
            Dim DE_OrdTemplates As New DEOrderTemplates("SELECT")
            Dim ordTemplate As New DEOrderTemplate(CType(ddlSelectTemplate.SelectedValue, Long))
            DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
            Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
            DB_OrdTemplates.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            DB_OrdTemplates.AccessDatabase()

            'Re-create the template, with the header info retrieved,
            'add the items to the template, and save the deftails
            Dim header As Data.DataTable = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesHeader")
            If header.Rows.Count > 0 Then
                Dim row As Data.DataRow = header.Rows(0)
                ordTemplate = New DEOrderTemplate(row("LOGINID"), row("BUSINESS_UNIT"), row("PARTNER"), CDate(row("CREATED_DATE")), _
                                                                CDate(row("LAST_USED_DATE")), Now, row("NAME"), row("DESCRIPTION"), row("IS_DEFAULT"), _
                                                                CType(row("TEMPLATE_HEADER_ID"), Long))

                'Populate each item in the basket into a OrderTemplateDetail Object
                'and add each into the order template object's items collection
                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                    ordTemplate.OrderTemplateItems.Add(New OrderTemplateDetail(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
                Next

                DE_OrdTemplates.OrderTemplates.Clear()
                DE_OrdTemplates.Purpose = "UPDATE"
                DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
                DB_OrdTemplates = New DBOrderTemplates(DE_OrdTemplates)
                DB_OrdTemplates.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                DB_OrdTemplates.AccessDatabase()
            End If
            Response.Redirect("~/PagesLogin/Template/ViewTemplates.aspx")
        Else
            errLabel.Text = _ucr.Content("BasketIsEmptyErrorText", _languageCode, True)
        End If
        plhErrorMessage.Visible = (errLabel.Text.Length > 0)
    End Sub

    Protected Sub btnSaveTemplate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveTemplate.Click
        errLabel.Text = String.Empty
        If Not Profile.Basket.IsEmpty Then
            Dim DE_OrdTemplates As New DEOrderTemplates("INSERT")
            Dim ordTemplate As New DEOrderTemplate(Profile.UserName, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), Now, _
                                                                    Now, Now, txtTemplateName.Text, txtTemplateDescription.Text, False, chkAllowFFToView.Checked)

            'Populate each item in the basket into a OrderTemplateDetail Object
            'and add each into the order template object's items collection
            For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                ordTemplate.OrderTemplateItems.Add(New OrderTemplateDetail(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            Next
            DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
            Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
            DB_OrdTemplates.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim err As ErrorObj = DB_OrdTemplates.AccessDatabase()

            If Not err.HasError AndAlso Not chkLeaveOrderInBasket.Checked Then
                Profile.Basket.EmptyBasket()
            End If
            Response.Redirect("~/PagesLogin/Template/ViewTemplates.aspx")
        Else
            errLabel.Text = _ucr.Content("BasketIsEmptyErrorText", _languageCode, True)
        End If
        plhErrorMessage.Visible = (errLabel.Text.Length > 0)
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setText()
        With _ucr
            btnSaveTemplate.Text = .Content("SaveAsButtonText", _languageCode, True)
            UpdateButton.Text = .Content("OverwriteButtonText", _languageCode, True)
            chkLeaveOrderInBasket.Text = .Content("LeaveBasketCheckBoxText", _languageCode, True)
            lblTemplateName.Text = .Content("TemplateNameLabel", _languageCode, True)
            lblTemplateDescription.Text = .Content("TemplateDescriptionLabel", _languageCode, True)
            lblOverwriteLabel.Text = .Content("OverwirteOrderTitle", _languageCode, True)
            lblNewTemplate.Text = .Content("SaveAsNewOrderTitle", _languageCode, True)
            ltlUpdateTemplateFieldsetLegend.Text = .Content("UpdateTemplateFieldsetLegend", _languageCode, True)
            ltlNewTemplateFieldetLegend.Text = .Content("NewTemplateFieldsetLegend", _languageCode, True)
            lblSelectTemplate.Text = .Content("SelectTemplateText", _languageCode, True)
            InstructionsLabel.Text = .Content("SaveAsNewInstructions", _languageCode, True)
            rfvTemplateName.Enabled = True
            rfvTemplateName.ErrorMessage = .Content("MissingNameErrorText", _languageCode, True)
            plhAllowFriendsAndFamilyToView.Visible = (ModuleDefaults.FriendsAndFamily AndAlso ModuleDefaults.ShowFFOrderTemplates)
            If plhAllowFriendsAndFamilyToView.Visible Then chkAllowFFToView.Text = .Content("AllowFFToViewWishList", _languageCode, True)
        End With
    End Sub

#End Region

End Class