Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Imports TalentBusinessLogic.DataTransferObjects

Partial Class UserControls_AdditionalProductInformation
    Inherits ControlBase

#Region "Private variables"

    Private _ucr As Talent.Common.UserControlResource
    Private _languageCode As String
    Private _settings As DESettings = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _validationGroups As List(Of String) = New List(Of String)
    Private _ptIndex As Integer
    Private _collectiveId As String = String.Empty
    Private _questionAnswers As List(Of KeyValuePair(Of String, String))
    Private _dtCurrentQuestionsAnswers As DataTable
    Private _dtPreviousQuestionsAnswers As DataTable
    Private _productQuestionsCount As Integer = 0
    Private _basketPriceBand As String
    Private _err As New Talent.Common.ErrorObj
    Private _templateCount As Integer = 0
    Private _fastCash As Boolean
    Private _dtProductQuestions As New DataTable
    Private _dtoActivityTemplateQA As ActivityTemplateQA

#End Region

#Region "Public properties"

    Public Property Validators() As String

#End Region

#Region "Constants"

    Private Const ADDITIONALINFOVALIDATION As String = "AdditionalInfoValidation"

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Request.QueryString("FastCash") Is Nothing Then
            _fastCash = False
        Else
            _fastCash = True
        End If
        _settings = Talent.eCommerce.Utilities.GetSettingsObject()
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _ucr = New Talent.Common.UserControlResource
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _businessUnit = TalentCache.GetBusinessUnit()
        TDataObjects.Settings = _settings
        With _ucr
            .BusinessUnit = _businessUnit
            .PartnerCode = _partnerCode
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AdditionalProductInformation.ascx"
        End With
        With _settings
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .DestinationDatabase = "SQL2005"
            .BusinessUnit = _businessUnit
            .StoredProcedureGroup = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
            .Cacheing = False
        End With
        PopulateTextAndAttributes()
        ScriptRegistration()

        If HttpContext.Current.Session("TemplateIDs") Is Nothing Then
            CType(Me.Page, TalentBase01).BasketContainsItemLinkedToTemplate(CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems, ModuleDefaults.CacheDependencyPath, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID, False)
        End If

        'retrieve Template information and Ids from basket buttons control, return to basket page if session string empty
        Dim templateIDData As List(Of ActivityTemplateQA) = HttpContext.Current.Session("TemplateIDs")
        If templateIDData Is Nothing OrElse templateIDData IsNot Nothing AndAlso templateIDData.Count = 0 Then
            Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
        End If

        'retrieve existing additional information answers for basket
        Dim resultDataSet As New DataSet
        Dim productEntity As DEProduct = New DEProduct
        Dim questionAnswers As DEProductQuestionsAnswers = New DEProductQuestionsAnswers
        Dim template As ActivityTemplateQA = templateIDData(0)
        _templateCount = templateIDData.Count
        questionAnswers.BasketID = template.BasketHeaderID
        productEntity.ProductQuestionAnswers = questionAnswers
        productEntity.CollDEProductQuestionAnswers = populateTheQuestionIDs()
        productEntity.ProductQuestionAnswers.CustomerNumber = Profile.User.Details.LoginID
        resultDataSet = RetrieveAdditionalInformation(productEntity)

        'determine whether or not we are officially retrieving answers from the iSeries by allocation id,
        'At this point answers could be previously given answers from QA002 or temporary basket answers from WS003B
        If resultDataSet IsNot Nothing Then
            _dtCurrentQuestionsAnswers = resultDataSet.Tables("CurrentQuestionsAndAnswers")
            _dtPreviousQuestionsAnswers = resultDataSet.Tables("PreviousQuestionsAndAnswers")
        Else
            _dtCurrentQuestionsAnswers = New DataTable
            _dtPreviousQuestionsAnswers = New DataTable
        End If

        'open first item in accordion menu if not post back
        If Not IsPostBack Then
            RegisterAcordionMenuScript(0)
        End If

        'call repeator with template information string array
        If templateIDData.Count > 0 Then
            rptTemplateIDs.DataSource = templateIDData
            rptTemplateIDs.DataBind()
        End If

        'creation of validator string value, used for validating multiple validators on save
        For Each valdationGroup As String In _validationGroups
            Validators &= valdationGroup & "_"
        Next
    End Sub

    Protected Sub rptTemplateIDs_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptTemplateIDs.ItemCommand
        Dim nextItemToOpen = Convert.ToInt32(e.CommandArgument) + 1
        If e.CommandName = _ucr.Content("NextText", _languageCode, True) Then
            RegisterAcordionMenuScript(nextItemToOpen)
        End If
    End Sub

    Protected Sub rptTemplateIDs_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTemplateIDs.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            'ptIndex acts as templateID specific identifier code
            _ptIndex = Convert.ToInt32(e.Item.ItemIndex)

            _dtoActivityTemplateQA = e.Item.DataItem

            'data saved in hiddenfields for the purpose of populating the questionAnswers data entity on save
            Dim hdfDataTemplateID As HiddenField = CType(e.Item.FindControl("hdfDataTemplateID"), HiddenField)
            Dim hdfDataLoginID As HiddenField = CType(e.Item.FindControl("hdfDataLoginID"), HiddenField)
            Dim hdfDataUserName As HiddenField = CType(e.Item.FindControl("hdfDataUserName"), HiddenField)
            Dim hdfDataProductDescription1 As HiddenField = CType(e.Item.FindControl("hdfDataProductDescription1"), HiddenField)
            Dim hdfDataProductDescription2 As HiddenField = CType(e.Item.FindControl("hdfDataProductDescription2"), HiddenField)
            Dim hdfDataProductDescription3 As HiddenField = CType(e.Item.FindControl("hdfDataProductDescription3"), HiddenField)
            Dim hdfDataProductDescription4 As HiddenField = CType(e.Item.FindControl("hdfDataProductDescription4"), HiddenField)
            Dim hdfDataProductDescription5 As HiddenField = CType(e.Item.FindControl("hdfDataProductDescription5"), HiddenField)
            Dim hdfDataProduct As HiddenField = CType(e.Item.FindControl("hdfDataProduct"), HiddenField)
            Dim hdfDataSeat As HiddenField = CType(e.Item.FindControl("hdfDataSeat"), HiddenField)
            Dim hdfDataPriceBand As HiddenField = CType(e.Item.FindControl("hdfDataPriceBand"), HiddenField)
            Dim hdfDataBasketHeaderID As HiddenField = CType(e.Item.FindControl("hdfDataBasketHeaderID"), HiddenField)
            Dim hdfDataFullName As HiddenField = CType(e.Item.FindControl("hdfDataFullName"), HiddenField)
            Dim hdfDataIsTemplatePerProduct As HiddenField = CType(e.Item.FindControl("hdfDataIsTemplatePerProduct"), HiddenField)


            hdfDataTemplateID.Value = _dtoActivityTemplateQA.TemplateID
            hdfDataLoginID.Value = _dtoActivityTemplateQA.LoginID
            hdfDataUserName.Value = _dtoActivityTemplateQA.UserName
            hdfDataProductDescription1.Value = _dtoActivityTemplateQA.ProductDescription1
            hdfDataProductDescription2.Value = _dtoActivityTemplateQA.ProductDescription2
            hdfDataProductDescription3.Value = _dtoActivityTemplateQA.ProductDescription3
            hdfDataProductDescription4.Value = _dtoActivityTemplateQA.ProductDescription4
            hdfDataProductDescription5.Value = _dtoActivityTemplateQA.ProductDescription5
            hdfDataProduct.Value = _dtoActivityTemplateQA.Product
            hdfDataSeat.Value = _dtoActivityTemplateQA.Seat
            hdfDataPriceBand.Value = _dtoActivityTemplateQA.PriceBand
            hdfDataBasketHeaderID.Value = _dtoActivityTemplateQA.BasketHeaderID
            hdfDataFullName.Value = _dtoActivityTemplateQA.FullName
            hdfDataIsTemplatePerProduct.Value = _dtoActivityTemplateQA.IsTemplatePerProduct


            If Not _dtoActivityTemplateQA Is Nothing Then

                'populate accordion header text
                Dim lblGameDetailsLabel As Label = CType(e.Item.FindControl("lblGameDetailsLabel"), Label)
                Dim lblGameTimeLabel As Label = CType(e.Item.FindControl("lblGameTimeLabel"), Label)
                Dim lblProfileIDLabel As Label = CType(e.Item.FindControl("lblProfileIDLabel"), Label)
                Dim vlsAdditionalInfoErrors As ValidationSummary = CType(e.Item.FindControl("vlsAdditionalInfoErrors"), ValidationSummary)
                Dim LoginIdCustomerName As KeyValuePair(Of String, String) = GetAllocatedCustomerName(_dtoActivityTemplateQA.LoginID, _dtoActivityTemplateQA.FullName)
                lblGameDetailsLabel.Text = _dtoActivityTemplateQA.ProductDescription1.Trim()

                Dim isTemplatePerProduct As Boolean = CType(_dtoActivityTemplateQA.IsTemplatePerProduct.ToString, Boolean)
                If Not isTemplatePerProduct Then
                    lblProfileIDLabel.Text = LoginIdCustomerName.Value & " " & "(" & LoginIdCustomerName.Key.ToString.Trim().TrimStart("0") & ")"
                End If

                lblGameTimeLabel.Text = _dtoActivityTemplateQA.ProductDescription4 & " " & _dtoActivityTemplateQA.ProductDescription5
                vlsAdditionalInfoErrors.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                vlsAdditionalInfoErrors.ID = "vlsAdditionalInfoErrors" & _ptIndex
                vlsAdditionalInfoErrors.HeaderText = _ucr.Content("ErrorHeaderText", _languageCode, True)
                'basket price band saved for the purpose of restriction
                _basketPriceBand = _dtoActivityTemplateQA.PriceBand

                'repeater populated based on template associate questions
                _dtProductQuestions = TDataObjects.ActivitiesSettings.GetExistingActivityQuestionByTemplateID(_dtoActivityTemplateQA.TemplateID)
                If Not _dtProductQuestions Is Nothing AndAlso _dtProductQuestions.Rows.Count > 0 Then
                    Dim rptProductQuestions As Repeater = CType(e.Item.FindControl("rptProductQuestions"), Repeater)
                    rptProductQuestions.Visible = True
                    rptProductQuestions.DataSource = _dtProductQuestions
                    rptProductQuestions.DataBind()
                Else
                    Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
                End If

                'next button made invisible on last accordion item
                Dim btnNext As Button = CType(e.Item.FindControl("btnNext"), Button)
                If _ptIndex < (_templateCount - 1) Then
                    btnNext.Visible = True
                Else
                    btnNext.Visible = False
                End If
                btnNext.CommandName = _ucr.Content("NextText", _languageCode, True)
                btnNext.Text = _ucr.Content("NextText", _languageCode, True)
                btnNext.CommandArgument = _ptIndex

                'accordion section specific validation added to next button
                btnNext.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex

                'validation group names saved for page wide validation on save/confirm
                _validationGroups.Add(ADDITIONALINFOVALIDATION & _ptIndex)
            End If
        End If
    End Sub

    Protected Sub rptProductQuestions_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim productQuestion As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim addAnswer As Boolean = False
            Dim dvAnswers As DataView = New DataView
            Dim answer As String = String.Empty
            Dim answerCollection As New List(Of String)
            Dim showQnAPriceBand As Boolean = False
            Dim complimentaryTemplateID As String = "0"
            Dim complimentaryTemplateTable As DataTable = TDataObjects.ActivitiesSettings.TblActivityTemplates.GetByTemplateName("Complimentary", _businessUnit)

            If complimentaryTemplateTable.Rows.Count > 0 Then
                complimentaryTemplateID = (complimentaryTemplateTable.Rows(0)("TEMPLATE_ID")).ToString()
            End If

            'answer are being retrieve from iseries, answer text is populated
            If _dtCurrentQuestionsAnswers.Rows.Count > 0 Then
                dvAnswers = New DataView(_dtCurrentQuestionsAnswers)
                If _dtoActivityTemplateQA.TemplateID = complimentaryTemplateID Then
                    dvAnswers.RowFilter = "[AllocationID]='" & _dtoActivityTemplateQA.LoginID & "' AND [ProductCode]='" & _dtoActivityTemplateQA.Product & "' AND [QuestionID]='" & productQuestion("QUESTION_ID") & "'"
                Else
                    dvAnswers.RowFilter = "[AllocationID]='" & _dtoActivityTemplateQA.LoginID & "' AND [SeatDetails]='" & _dtoActivityTemplateQA.Seat & "' AND [QuestionID]='" & productQuestion("QUESTION_ID") & "'"
                End If
                If dvAnswers.Count > 0 Then
                    addAnswer = True
                    answer = dvAnswers(0).Item("Answer").ToString
                End If
            End If
            If _dtPreviousQuestionsAnswers.Rows.Count > 0 Then
                dvAnswers = New DataView(_dtPreviousQuestionsAnswers)
                dvAnswers.RowFilter = "[QuestionID]='" & productQuestion("QUESTION_ID") & "'"
                If dvAnswers.Count > 0 AndAlso Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(productQuestion("REMEMBERED_ANSWER")) Then
                    addAnswer = True
                    For Each row As DataRow In dvAnswers.ToTable.Rows
                        answerCollection.Add(row("Answer").ToString)
                    Next
                End If
            End If

            'price band restriction
            If Not IsDBNull(productQuestion("PRICE_BAND_LIST")) Then
                If Not String.IsNullOrEmpty(productQuestion("PRICE_BAND_LIST")) Then
                    If isPriceBandAllowed(_basketPriceBand, productQuestion("PRICE_BAND_LIST")) Then
                        showQnAPriceBand = True
                    End If
                Else
                    showQnAPriceBand = True
                End If
            End If

            'collectiveId acts as templateID and Question specific identifier code
            _collectiveId = _ptIndex & Convert.ToInt32(e.Item.ItemIndex)

            'question id necessary for on save click
            Dim questionID As HiddenField = CType(e.Item.FindControl("hdfQuestionID"), HiddenField)
            Dim hdfRememberedAnswer As HiddenField = CType(e.Item.FindControl("hdfRememberedAnswer"), HiddenField)
            questionID.Value = productQuestion("QUESTION_ID")
            hdfRememberedAnswer.Value = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(productQuestion("REMEMBERED_ANSWER"))
            If showQnAPriceBand Then
                'answer type display mech
                Select Case productQuestion("ANSWER_TYPE")
                    Case Is = GlobalConstants.FREE_TEXT_FIELD
                        Dim plhFreeTextField As PlaceHolder = CType(e.Item.FindControl("plhFreeTextField"), PlaceHolder)
                        Dim txtQuestionText As TextBox = CType(e.Item.FindControl("txtQuestionText"), TextBox)
                        Dim lblQuestionText As Label = CType(e.Item.FindControl("lblQuestionText"), Label)
                        Dim revQuestionText As RegularExpressionValidator = CType(e.Item.FindControl("revQuestionText"), RegularExpressionValidator)
                        Dim rfvQuestionText As RequiredFieldValidator = CType(e.Item.FindControl("rfvQuestionText"), RequiredFieldValidator)
                        rfvQuestionText.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                        revQuestionText.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                        rfvQuestionText.ErrorMessage = productQuestion("QUESTION_TEXT")
                        revQuestionText.ErrorMessage = productQuestion("QUESTION_TEXT")
                        If productQuestion("MANDATORY") = True Then
                            rfvQuestionText.Enabled = True
                        End If
                        plhFreeTextField.Visible = True
                        If addAnswer Then
                            txtQuestionText.Text = answer
                        End If
                        lblQuestionText.Text = productQuestion("QUESTION_TEXT")
                        If Not IsDBNull(productQuestion("REGULAR_EXPRESSION")) Then
                            If Not String.IsNullOrEmpty(productQuestion("REGULAR_EXPRESSION")) Then
                                revQuestionText.Enabled = True
                                revQuestionText.ValidationExpression = productQuestion("REGULAR_EXPRESSION")
                            End If
                        End If
                        If Not IsDBNull(productQuestion("HYPERLINK")) Then
                            If Not String.IsNullOrEmpty(productQuestion("HYPERLINK")) Then
                                Dim hyperlinkString As String = productQuestion("HYPERLINK")
                                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                                Dim hplFreeTextFieldExternalLink As HyperLink = CType(e.Item.FindControl("hplFreeTextFieldExternalLink"), HyperLink)
                                hplFreeTextFieldExternalLink.Visible = True
                                hplFreeTextFieldExternalLink.NavigateUrl = hyperlinkData(1)
                                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                                    hplFreeTextFieldExternalLink.Text = hyperlinkData(1)
                                Else
                                    hplFreeTextFieldExternalLink.Text = hyperlinkData(0)
                                End If
                            End If
                        End If
                    Case Is = GlobalConstants.CHECKBOX
                        Dim plhCheckbox As PlaceHolder = CType(e.Item.FindControl("plhCheckbox"), PlaceHolder)
                        Dim chkQuestionCheck As CheckBox = CType(e.Item.FindControl("chkQuestionCheckText"), CheckBox)
                        Dim lblQuestionCheckText As Label = CType(e.Item.FindControl("lblQuestionCheckText"), Label)
                        Dim cvCheckbox As CustomValidator = CType(e.Item.FindControl("cvCheckbox"), CustomValidator)

                        cvCheckbox.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                        chkQuestionCheck.ID = "chkQuestionCheck" & _collectiveId
                        lblQuestionCheckText.AssociatedControlID = "chkQuestionCheck" & _collectiveId
                        Page.ClientScript.RegisterExpandoAttribute(cvCheckbox.ClientID, "QuestionCheck", "chkQuestionCheck" & _collectiveId, False)
                        If productQuestion("MANDATORY") = True Then
                            cvCheckbox.Enabled = True
                        End If
                        plhCheckbox.Visible = True
                        lblQuestionCheckText.Text = productQuestion("QUESTION_TEXT")
                        cvCheckbox.ErrorMessage = productQuestion("QUESTION_TEXT")
                        If addAnswer Then
                            If Not String.IsNullOrEmpty(answer) Then
                                chkQuestionCheck.Checked = CType(answer, Boolean)
                            End If
                        End If
                        If Not IsDBNull(productQuestion("HYPERLINK")) Then
                            If Not String.IsNullOrEmpty(productQuestion("HYPERLINK")) Then
                                Dim hyperlinkString As String = productQuestion("HYPERLINK")
                                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                                Dim hplCheckBoxExternalLink As HyperLink = CType(e.Item.FindControl("hplCheckBoxExternalLink"), HyperLink)
                                hplCheckBoxExternalLink.Visible = True
                                hplCheckBoxExternalLink.NavigateUrl = hyperlinkData(1)
                                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                                    hplCheckBoxExternalLink.Text = hyperlinkData(1)
                                Else
                                    hplCheckBoxExternalLink.Text = hyperlinkData(0)
                                End If
                            End If
                        End If
                    Case Is = GlobalConstants.QUESTION_DATE
                        Dim rfvDate As RequiredFieldValidator = CType(e.Item.FindControl("rfvDate"), RequiredFieldValidator)
                        Dim revDate As RegularExpressionValidator = CType(e.Item.FindControl("revDate"), RegularExpressionValidator)
                        Dim plhDate As PlaceHolder = CType(e.Item.FindControl("plhDate"), PlaceHolder)
                        Dim txtDate As TextBox = CType(e.Item.FindControl("txtDate"), TextBox)
                        Dim lblDate As Label = CType(e.Item.FindControl("lblDate"), Label)
                        rfvDate.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                        revDate.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                        revDate.ValidationExpression = _ucr.Attribute("DateFieldRegex")
                        revDate.ErrorMessage = productQuestion("QUESTION_TEXT")
                        rfvDate.ErrorMessage = productQuestion("QUESTION_TEXT")
                        If productQuestion("MANDATORY") = True Then
                            rfvDate.Enabled = True
                        End If
                        plhDate.Visible = True
                        If addAnswer Then
                            txtDate.Text = answer
                        End If
                        lblDate.Text = productQuestion("QUESTION_TEXT")
                        If Not IsDBNull(productQuestion("HYPERLINK")) Then
                            If Not String.IsNullOrEmpty(productQuestion("HYPERLINK")) Then
                                Dim hyperlinkString As String = productQuestion("HYPERLINK")
                                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                                Dim hplDateExternalLink As HyperLink = CType(e.Item.FindControl("hplDateExternalLink"), HyperLink)
                                hplDateExternalLink.Visible = True
                                hplDateExternalLink.NavigateUrl = hyperlinkData(1)
                                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                                    hplDateExternalLink.Text = hyperlinkData(1)
                                Else
                                    hplDateExternalLink.Text = hyperlinkData(0)
                                End If
                            End If
                        End If
                    Case Is = GlobalConstants.LIST_OF_ANSWERS
                        Dim plhListOfAnswers As PlaceHolder = CType(e.Item.FindControl("plhListOfAnswers"), PlaceHolder)
                        Dim ddlAnswers As DropDownList = CType(e.Item.FindControl("ddlAnswers"), DropDownList)
                        Dim lblListOfAnswers As Label = CType(e.Item.FindControl("lblListOfAnswers"), Label)
                        Dim populatePleaseSpecifyBox As Boolean = True
                        plhListOfAnswers.Visible = True
                        lblListOfAnswers.Text = productQuestion("QUESTION_TEXT")

                        ddlAnswers.DataSource = TDataObjects.ActivitiesSettings.GetAnswerByQuestionID(productQuestion("QUESTION_ID").ToString())
                        ddlAnswers.DataValueField = "ANSWER_ID"
                        ddlAnswers.DataTextField = "ANSWER_TEXT"
                        ddlAnswers.DataBind()

                        If addAnswer Then
                            If _dtPreviousQuestionsAnswers.Rows.Count > 0 Then
                                For Each item As ListItem In ddlAnswers.Items
                                    item.Attributes("OptionGroup") = _ucr.Content("AllOtherAnswersOptionGroup", _languageCode, True)
                                Next
                                SetAnswers(ddlAnswers, answerCollection, Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(productQuestion.Item("ALLOW_SELECT_OTHER_OPTION")), answer)
                            ElseIf _dtCurrentQuestionsAnswers.Rows.Count > 0 AndAlso answer.Length > 0 Then
                                Dim li As ListItem = ddlAnswers.Items.FindByText(answer)
                                If li IsNot Nothing Then ddlAnswers.SelectedItem.Text = answer
                            End If
                        End If

                        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(productQuestion.Item("ALLOW_SELECT_OTHER_OPTION")) Then
                            Dim txtSpecify As TextBox = CType(e.Item.FindControl("txtSpecify"), TextBox)
                            Dim lblSpecify As Label = CType(e.Item.FindControl("lblSpecify"), Label)
                            'specify div generated to contain specify text/label - important for validation on specify text box
                            Dim specifyDiv As HtmlGenericControl = New HtmlGenericControl("div")
                            lblSpecify.Visible = True
                            txtSpecify.Visible = True
                            lblSpecify.Text = _ucr.Content("SpecifyText", _languageCode, True)

                            Dim rfvSpecify As RequiredFieldValidator = CType(e.Item.FindControl("rfvSpecify"), RequiredFieldValidator)
                            rfvSpecify.ID = "rfvSpecify" & _collectiveId
                            rfvSpecify.ValidationGroup = ADDITIONALINFOVALIDATION & _ptIndex
                            rfvSpecify.ErrorMessage = productQuestion("QUESTION_TEXT")
                            'rfv enabled in javacript based on mandatory settings; see boolean last parameter
                            If productQuestion("MANDATORY") = True Then
                                ddlAnswers.Attributes.Add("onchange", "showOtherOptionTextbox('" & _collectiveId & "', this, true);")
                            Else
                                ddlAnswers.Attributes.Add("onchange", "showOtherOptionTextbox('" & _collectiveId & "', this, false);")
                            End If
                            ddlAnswers.Items.Insert(ddlAnswers.Items.Count, _ucr.Content("OtherText", _languageCode, True))

                            'doesn't contain answer in ddl, meaning it has a non-standard answer (specify answer)
                            If Not ddlContainsAnswerText(answer, ddlAnswers.Items) Then
                                If populatePleaseSpecifyBox Then txtSpecify.Text = answer
                                If String.IsNullOrWhiteSpace(answer) Then
                                    specifyDiv.Attributes.Add("class", "row ebiz-specify D" & _collectiveId)
                                Else
                                    ddlAnswers.Items.FindByText(_ucr.Content("OtherText", _languageCode, True)).Selected = True
                                    specifyDiv.Attributes.Add("class", "row D" & _collectiveId)
                                End If
                            Else
                                specifyDiv.Attributes.Add("class", "row ebiz-specify D" & _collectiveId)
                            End If

                            rfvSpecify.ClientIDMode = System.Web.UI.ClientIDMode.Static
                            Dim specifyInputDiv As HtmlGenericControl = New HtmlGenericControl("div")
                            specifyInputDiv.Attributes.Add("class", "large-4 columns")
                            specifyInputDiv.Controls.Add(txtSpecify)
                            Dim specifyTextDiv As HtmlGenericControl = New HtmlGenericControl("div")
                            specifyTextDiv.Attributes.Add("class", "large-8 columns")
                            specifyTextDiv.Controls.Add(lblSpecify)
                            specifyDiv.Controls.Add(specifyTextDiv)
                            specifyDiv.Controls.Add(specifyInputDiv)
                            plhListOfAnswers.Controls.Add(specifyDiv)

                            If ddlAnswers.Items.Count = 1 AndAlso ddlAnswers.SelectedItem.Text = _ucr.Content("OtherText", _languageCode, True) Then
                                ddlAnswers.Visible = False
                                specifyDiv.Style.Add("display", "block")
                            Else
                                ddlAnswers.Visible = True
                                specifyDiv.Style.Add("display", "none")
                            End If
                        Else
                            If addAnswer Then
                                If _dtCurrentQuestionsAnswers.Rows.Count > 0 Then
                                    ddlAnswers.Items.FindByText(answer).Selected = True
                                End If
                            End If
                        End If

                        If Not IsDBNull(productQuestion("HYPERLINK")) Then
                            If Not String.IsNullOrEmpty(productQuestion("HYPERLINK")) Then
                                Dim hyperlinkString As String = productQuestion("HYPERLINK")
                                Dim hyperlinkData() As String = hyperlinkString.Split("^")
                                Dim hplListOfAnswersExternalLink As HyperLink = CType(e.Item.FindControl("hplListOfAnswersExternalLink"), HyperLink)
                                hplListOfAnswersExternalLink.Visible = True
                                hplListOfAnswersExternalLink.NavigateUrl = hyperlinkData(1)
                                If String.IsNullOrEmpty(hyperlinkData(0)) Then
                                    hplListOfAnswersExternalLink.Text = hyperlinkData(1)
                                Else
                                    hplListOfAnswersExternalLink.Text = hyperlinkData(0)
                                End If
                            End If
                        End If
                End Select
            End If

            Dim answerType As HiddenField = CType(e.Item.FindControl("answerType"), HiddenField)
            'price band restricted questions still create repeater items, this negatively affects the on save functionality 
            '-5 signals that the question is priceband restricted
            If showQnAPriceBand Then
                answerType.Value = productQuestion("ANSWER_TYPE")
            Else
                answerType.Value = -5
            End If
        End If
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim questionAnswersList As List(Of DEProductQuestionsAnswers) = New List(Of DEProductQuestionsAnswers)
        For Each templateIDItem As RepeaterItem In rptTemplateIDs.Items
            Dim hdfDataTemplateID As HiddenField = CType(templateIDItem.FindControl("hdfDataTemplateID"), HiddenField)
            Dim hdfDataLoginID As HiddenField = CType(templateIDItem.FindControl("hdfDataLoginID"), HiddenField)
            Dim hdfDataUserName As HiddenField = CType(templateIDItem.FindControl("hdfDataUserName"), HiddenField)
            Dim hdfDataProductDescription1 As HiddenField = CType(templateIDItem.FindControl("hdfDataProductDescription1"), HiddenField)
            Dim hdfDataProductDescription2 As HiddenField = CType(templateIDItem.FindControl("hdfDataProductDescription2"), HiddenField)
            Dim hdfDataProductDescription3 As HiddenField = CType(templateIDItem.FindControl("hdfDataProductDescription3"), HiddenField)
            Dim hdfDataProductDescription4 As HiddenField = CType(templateIDItem.FindControl("hdfDataProductDescription4"), HiddenField)
            Dim hdfDataProductDescription5 As HiddenField = CType(templateIDItem.FindControl("hdfDataProductDescription5"), HiddenField)
            Dim hdfDataProduct As HiddenField = CType(templateIDItem.FindControl("hdfDataProduct"), HiddenField)
            Dim hdfDataSeat As HiddenField = CType(templateIDItem.FindControl("hdfDataSeat"), HiddenField)
            Dim hdfDataPriceBand As HiddenField = CType(templateIDItem.FindControl("hdfDataPriceBand"), HiddenField)
            Dim hdfDataBasketHeaderID As HiddenField = CType(templateIDItem.FindControl("hdfDataBasketHeaderID"), HiddenField)
            Dim hdfDataFullName As HiddenField = CType(templateIDItem.FindControl("hdfDataFullName"), HiddenField)
            Dim hdfDataIsTemplatePerProduct As HiddenField = CType(templateIDItem.FindControl("hdfDataIsTemplatePerProduct"), HiddenField)

            Dim rptproductQuestions As Repeater = CType(templateIDItem.FindControl("rptProductQuestions"), Repeater)

            For Each productQuestionItem As RepeaterItem In rptproductQuestions.Items
                Dim questionID As HiddenField = CType(productQuestionItem.FindControl("hdfQuestionID"), HiddenField)
                Dim answerType As HiddenField = CType(productQuestionItem.FindControl("answerType"), HiddenField)
                Dim hdfRememberedAnswer As HiddenField = CType(productQuestionItem.FindControl("hdfRememberedAnswer"), HiddenField)
                If answerType.Value <> -5 Then
                    Dim questionAnswers As DEProductQuestionsAnswers = New DEProductQuestionsAnswers
                    questionAnswers.QuestionID = questionID.Value
                    questionAnswers.RememberedAnswer = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(hdfRememberedAnswer.Value)
                    Select Case Convert.ToInt32(answerType.Value)
                        Case Is = GlobalConstants.FREE_TEXT_FIELD
                            Dim txtQuestionText As String = CType(productQuestionItem.FindControl("txtQuestionText"), TextBox).Text
                            Dim lblQuestionText As String = CType(productQuestionItem.FindControl("lblQuestionText"), Label).Text
                            questionAnswers.QuestionText = lblQuestionText
                            questionAnswers.AnswerText = txtQuestionText
                        Case Is = GlobalConstants.CHECKBOX
                            Dim lblQuestionCheckText As Label = CType(productQuestionItem.FindControl("lblQuestionCheckText"), Label)
                            Dim plhCheckbox As PlaceHolder = CType(productQuestionItem.FindControl("plhCheckbox"), PlaceHolder)
                            For Each Control As Object In plhCheckbox.Controls
                                If TypeOf Control Is CheckBox Then
                                    Dim questionCheck As CheckBox = CType(Control, CheckBox)
                                    questionAnswers.AnswerText = CType(questionCheck.Checked, String)
                                    Exit For
                                End If
                            Next
                            questionAnswers.QuestionText = lblQuestionCheckText.Text
                        Case Is = GlobalConstants.QUESTION_DATE
                            Dim txtDate As TextBox = CType(productQuestionItem.FindControl("txtDate"), TextBox)
                            Dim lblDate As Label = CType(productQuestionItem.FindControl("lblDate"), Label)
                            questionAnswers.QuestionText = lblDate.Text
                            Try
                                Dim result As Date = Date.ParseExact(txtDate.Text, "d", New System.Globalization.CultureInfo(ModuleDefaults.Culture))
                                questionAnswers.AnswerText = Convert.ToDateTime(txtDate.Text.Trim, New System.Globalization.CultureInfo(ModuleDefaults.Culture)).ToString("dd/MM/yyyy")
                            Catch err As FormatException
                                questionAnswers.AnswerText = String.Empty
                            End Try
                        Case Is = GlobalConstants.LIST_OF_ANSWERS
                            Dim ddlAnswers As DropDownList = CType(productQuestionItem.FindControl("ddlAnswers"), DropDownList)
                            Dim lblListOfAnswers As Label = CType(productQuestionItem.FindControl("lblListOfAnswers"), Label)
                            questionAnswers.QuestionText = lblListOfAnswers.Text
                            If ddlAnswers.SelectedItem.Text = _ucr.Content("OtherText", _languageCode, True) Then
                                Dim txtSpecify As TextBox = CType(productQuestionItem.FindControl("txtSpecify"), TextBox)
                                If String.IsNullOrWhiteSpace(txtSpecify.Text) Then
                                    questionAnswers.AnswerText = _ucr.Content("OtherText", _languageCode, True)
                                Else
                                    questionAnswers.AnswerText = txtSpecify.Text
                                End If
                            Else
                                questionAnswers.AnswerText = ddlAnswers.SelectedItem.Text
                            End If

                    End Select
                    'Encoding conversion
                    questionAnswers.AnswerText = Talent.Common.Utilities.ConvertASCIIHexValue(questionAnswers.AnswerText, _businessUnit, _partnerCode, _languageCode, _settings)
                    questionAnswers.AllocationCustomerNumber = hdfDataLoginID.Value.ToString
                    questionAnswers.ProductCode = hdfDataProduct.Value.ToString
                    questionAnswers.PriceBand = hdfDataPriceBand.Value.ToString
                    Dim isPerProduct As Boolean = CType(hdfDataIsTemplatePerProduct.Value, Boolean)
                    If Not isPerProduct Then
                        questionAnswers.CustomerNumber = Profile.User.Details.LoginID
                        Dim SeatDetails As String = hdfDataSeat.Value.ToString
                        questionAnswers.SeatData = SeatDetails.Substring(0, SeatDetails.Length - 1)
                        questionAnswers.AlphaSeat = SeatDetails(SeatDetails.Length - 1)
                    Else
                        'Setting Alpha seat to T because if Alpha Seat isn't assigned, WS146R won't return anything
                        questionAnswers.AlphaSeat = "T"
                        questionAnswers.CustomerNumber = hdfDataLoginID.Value.ToString
                    End If
                    questionAnswers.BasketID = hdfDataBasketHeaderID.Value
                    questionAnswersList.Add(questionAnswers)
                End If
            Next
        Next

        Dim productEntity As DEProduct = New DEProduct
        productEntity.CollDEProductQuestionAnswers = questionAnswersList
        SetAdditionalInformation(productEntity)

        'ok to redirect to check out?
        HttpContext.Current.Session("AddInfoCompleted") = True
        If _fastCash Then
            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
            ticketingGatewayFunctions.CheckoutPayment()
        Else
            Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx?qna=true")
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Javascript accordion menu open item on page load code.
    ''' </summary>
    ''' <param name="itemToOpen">Item number of accordion menu to open</param>
    ''' <remarks></remarks>
    Private Sub RegisterAcordionMenuScript(ByVal itemToOpen As Integer)
        Dim javascriptString As New StringBuilder
        javascriptString.Append("$(function () {")
        javascriptString.Append("    $("".ebiz-additional-information"").accordion({ icons: false, heightStyle: ""content"", header: ""div.ebiz-header"" });")
        javascriptString.Append("    $("".ebiz-additional-information"").accordion(""option"", ""active"", ")
        javascriptString.Append(itemToOpen)
        javascriptString.Append(");")
        javascriptString.Append("});")
        ScriptManager.RegisterStartupScript(rptTemplateIDs, Me.GetType(), "AccordionMenu", javascriptString.ToString(), True)
    End Sub

    ''' <summary>
    ''' Call to iSeries to save the user QnA entries
    ''' </summary>
    ''' <param name="productEntity">Product entity contains list of DEProductQuestionsAnswers</param>
    ''' <remarks></remarks>
    Private Sub SetAdditionalInformation(ByVal productEntity As DEProduct)
        Dim productDetails As New DEProductDetails
        Dim product As New TalentProduct
        productDetails.SessionId = Profile.Basket.Basket_Header_ID
        product.Settings = _settings
        product.Dep = productEntity
        product.De = productDetails
        Dim outputResult As DataTable = Nothing
        _err = product.AddProductQuestionAnswers()
        If Not _err.HasError AndAlso product.ResultDataSet.Tables.Count() > 0 Then
            outputResult = product.ResultDataSet.Tables(0)
        End If
    End Sub

    ''' <summary>
    ''' Retrieves already entered QnAs for a basket
    ''' </summary>
    ''' <param name="productEntity">productEntity contains DEProductQuestionsAnswers which has the BasketID set</param>
    ''' <returns>Save QnA data</returns>
    ''' <remarks></remarks>
    Private Function RetrieveAdditionalInformation(ByVal productEntity As DEProduct) As DataSet
        Dim err As New Talent.Common.ErrorObj
        Dim productDetails As New DEProductDetails
        Dim product As New TalentProduct
        product.Settings = _settings
        product.Dep = productEntity
        Dim outputResult As DataSet = Nothing
        err = product.RetrieveProductQuestionAnswers()
        If Not err.HasError AndAlso product.ResultDataSet.Tables.Count() > 0 AndAlso product.ResultDataSet.Tables("StatusResults").Rows.Count > 0 _
            AndAlso Talent.eCommerce.Utilities.CheckForDBNull_String(product.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred")) <> GlobalConstants.ERRORFLAG Then
            outputResult = product.ResultDataSet
        End If
        Return outputResult
    End Function

    ''' <summary>
    ''' Verifies if accepted price band allows for a question to be shown
    ''' </summary>
    ''' <param name="basketPriceBand">Accepted basket price band</param>
    ''' <param name="questionPriceBands">priceband(s) of the question</param>
    ''' <returns>True if accepted price band is in lst of question price bands</returns>
    ''' <remarks></remarks>
    Private Function isPriceBandAllowed(ByVal basketPriceBand As String, ByVal questionPriceBands As String) As Boolean
        Dim questionPriceBandsArray As String() = questionPriceBands.Split(",")
        For Each questionPriceBand As String In questionPriceBandsArray
            If questionPriceBand = basketPriceBand Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Generic JavaScript registeration Sub
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ScriptRegistration()
        'Date format for date picker
        Dim initJavaScript As New StringBuilder
        Const SEMICOLON As String = ";"
        Const SPEECHMARKS As String = """"
        initJavaScript.Append("var talentDateFormat = """).Append(ModuleDefaults.GlobalDateFormat).Append(SPEECHMARKS).Append(SEMICOLON)
        ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "initial-js", initJavaScript.ToString(), True)
        'Operates on drop down list change to show/hide the specify box and to activiate/deactivate the required validators on specify box
        Dim sbJavaScript As New StringBuilder
        sbJavaScript.Append("<script language=""javascript"" type=""text/javascript"">")
        sbJavaScript.Append(" " & "function showOtherOptionTextbox(index, ddl, manditory) { ")
        sbJavaScript.Append("   " & "var sInd = ddl.selectedIndex;")
        'sbJavaScript.Append("     " & "debugger;")
        sbJavaScript.Append("   " & "var sVal = ddl.options[sInd].text;")
        sbJavaScript.Append("   " & "var rfv = document.getElementById(""rfvSpecify"" + index);")
        sbJavaScript.Append("   " & "if( sVal == """ & _ucr.Content("OtherText", _languageCode, True) & """){ ")
        sbJavaScript.Append("       " & "$("".D"" + index).show(); ")
        sbJavaScript.Append("       " & "if(manditory){")
        sbJavaScript.Append("           " & "rfv.enabled = true;}")
        sbJavaScript.Append("       " & "}else{ ")
        sbJavaScript.Append("           " & "$("".D"" + index).hide(); if(manditory) rfv.enabled = false;}")
        sbJavaScript.Append(" " & "} ")
        sbJavaScript.Append("</script>")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "TemplateJS", sbJavaScript.ToString())

        sbJavaScript = Nothing
        'Validates validator for each accordion menu section and adds header css class to accordion menu headers based on success of validation
        Dim validateJavaScript As New StringBuilder
        validateJavaScript.Append("<script language=""javascript"" type=""text/javascript"">")
        validateJavaScript.Append(" " & "function multiValidator() { ")
        validateJavaScript.Append("     " & "var isValid = true;")
        'validateJavaScript.Append("     " & "debugger;")
        validateJavaScript.Append("     " & "var listOfValidators = document.getElementById(""hdfValidators"").defaultValue;")
        validateJavaScript.Append("     " & "var arrayOfValidators = listOfValidators.split(""_"");")
        validateJavaScript.Append("     " & "var firstFail = false;")
        validateJavaScript.Append("     " & "for (var i=0,len=arrayOfValidators.length-1; i<len; i++){")
        validateJavaScript.Append("     " & "var validator = arrayOfValidators[i];")
        validateJavaScript.Append("     " & "isValid = Page_ClientValidate(validator);")
        validateJavaScript.Append("         " & "if (!isValid) {")
        validateJavaScript.Append("             " & "if(!firstFail){")
        validateJavaScript.Append("                 " & "firstFail = true;")
        validateJavaScript.Append("                 " & "$("".ebiz-additional-information"").accordion(""option"", ""active"", i); break;}")
        validateJavaScript.Append("          " & "}")
        validateJavaScript.Append("     " & "}")
        validateJavaScript.Append("     " & "return isValid;")
        validateJavaScript.Append(" " & "} ")
        validateJavaScript.Append("</script>")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "MultiValidatorScripts", validateJavaScript.ToString())
        validateJavaScript = Nothing

        'checkbox validation for checkboxes with generated ids
        Dim sbCheckboxValidate As New StringBuilder
        sbCheckboxValidate.Append("<script language=""javascript"" type=""text/javascript"">")
        sbCheckboxValidate.AppendLine(" function validateCheckbox(oSrc, args){ ")
        sbCheckboxValidate.AppendLine(" var chk =document.getElementById(oSrc.QuestionCheck);")
        sbCheckboxValidate.AppendLine(" if(chk.checked) args.IsValid = true; else args.IsValid = false;  } ")
        sbCheckboxValidate.Append("</script>")
        Page.ClientScript.RegisterStartupScript(Me.GetType(), "CheckboxValidateJS", sbCheckboxValidate.ToString())
        sbCheckboxValidate = Nothing
    End Sub

    Private Sub PopulateTextAndAttributes()
        btnSave.Text = _ucr.Content("SaveButtonText", _languageCode, True)
    End Sub

    Private Function ddlContainsAnswerText(ByVal answer As String, ByVal items As ListItemCollection) As Boolean
        For Each item As ListItem In items
            If item.Text = answer Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function GetAllocatedCustomerName(ByVal allocatedLoginId As String, ByVal customerName As String) As KeyValuePair(Of String, String)
        If allocatedLoginId <> Profile.UserName Then
            Dim customer As New DECustomer
            customer.UserName = Profile.UserName
            customer.CustomerNumber = Profile.User.Details.LoginID
            customer.Source = "W"

            Dim talentCustomer As New TalentCustomer
            Dim deCustV11 As New DECustomerV11
            deCustV11.DECustomersV1.Add(customer)

            talentCustomer.DeV11 = deCustV11
            talentCustomer.Settings = _settings

            talentCustomer.Settings.Cacheing = True

            _err = talentCustomer.CustomerAssociations
            Dim dtFriendsAndFamily As DataTable = talentCustomer.ResultDataSet.Tables("FriendsAndFamily")

            Dim ffStore As DataRow = Nothing
            For Each friendOrFamily As DataRow In dtFriendsAndFamily.Rows
                If friendOrFamily.Item("AssociatedCustomerNumber") = allocatedLoginId Then
                    ffStore = friendOrFamily
                    Exit For
                End If
            Next
            If Not ffStore Is Nothing Then
                Return New KeyValuePair(Of String, String)(ffStore.Item("AssociatedCustomerNumber"), ffStore.Item("Forename").trim & " " & ffStore.Item("Surname").trim)
            Else
                Return New KeyValuePair(Of String, String)(Profile.User.Details.LoginID, customerName)
            End If
        Else
            Return New KeyValuePair(Of String, String)(Profile.User.Details.LoginID, customerName)
        End If
    End Function

    ''' <summary>
    ''' Populate the question IDs from the current template of questions, into the data entity collection. This is so we can retrieve previously given answers for these question IDs
    ''' </summary>
    ''' <returns>A collection of the DEProductQuestionsAnswers data entity populated with Question IDs in the current template</returns>
    ''' <remarks></remarks>
    Private Function populateTheQuestionIDs() As List(Of DEProductQuestionsAnswers)
        Dim listOfDEProductQuestionsAnswers As New List(Of DEProductQuestionsAnswers)
        For Each row As DataRow In _dtProductQuestions.Rows
            Dim de As New DEProductQuestionsAnswers
            de.QuestionID = Talent.eCommerce.Utilities.CheckForDBNull_Int(row("QUESTION_ID"))
            listOfDEProductQuestionsAnswers.Add(de)
        Next
        Return listOfDEProductQuestionsAnswers
    End Function

    ''' <summary>
    ''' Reorder the drop down list of answers based on the list of answers in the given collection, add the option group and change the selected value
    ''' </summary>
    ''' <param name="ddlAnswers">The answers drop down list</param>
    ''' <param name="answerCollection">The list of previous answers in a collection</param>
    ''' <param name="canSelectOtherOption">Is the select "other" option available</param>
    ''' <param name="otherAnswerGiven">The last answer given that isn't listed or the current answer to select</param>
    ''' <remarks></remarks>
    Private Sub SetAnswers(ByRef ddlAnswers As DropDownList, ByRef answerCollection As List(Of String), ByVal canSelectOtherOption As Boolean, ByRef otherAnswerGiven As String)
        For Each item As String In answerCollection
            Dim answerListItem As ListItem = ddlAnswers.Items.FindByText(item)
            If answerListItem IsNot Nothing Then
                'Answer already listed that is being moved to the top of the list
                answerListItem.Attributes("OptionGroup") = _ucr.Content("PreviouslyGivenAnswersOptionGroup", _languageCode, True)
                ddlAnswers.Items.Remove(answerListItem)
                ddlAnswers.Items.Insert(0, answerListItem)
                If otherAnswerGiven.Length = 0 Then ddlAnswers.SelectedIndex = 0
            Else
                'Answer isn't listed therefore being added as a new one
                answerListItem = New ListItem
                answerListItem.Text = item
                answerListItem.Value = item
                answerListItem.Attributes("OptionGroup") = _ucr.Content("PreviouslyGivenAnswersOptionGroup", _languageCode, True)
                ddlAnswers.Items.Insert(0, answerListItem)
            End If
        Next
        If otherAnswerGiven.Length > 0 Then
            Dim found As Boolean = False
            Dim i As Integer = 0
            For Each item As ListItem In ddlAnswers.Items
                If item.Text = otherAnswerGiven Then
                    found = True
                    Exit For
                End If
                i += 1
            Next
            ddlAnswers.SelectedIndex = i
        End If
    End Sub

#End Region

End Class
