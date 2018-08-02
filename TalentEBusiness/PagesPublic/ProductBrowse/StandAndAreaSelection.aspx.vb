Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.eCommerce.CATHelper
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports System.Data
Imports TalentBusinessLogic.Models
Imports System.Web.Script.Serialization
Imports TalentBusinessLogic.ModelBuilders.Products

Partial Class PagesPublic_ProductBrowse_standAndAreaSelection
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As New WebFormResource
    Private _languageCode As String = String.Empty
    Private _errMsg As Talent.Common.TalentErrorMessages
    Private _businessUnit As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _stadiumCode As String = String.Empty
    Private _visualSeatSelection As Boolean
    Private _alternativeSeatSelection As Boolean
    Private _alertnativeSeatSelectionAcrossStands As Boolean
    Private _homeAsAway As String
    Private _productCode As String
    Private _campaignCode As String
    Private _productType As String
    Private _productSubType As String
    Private Shared _defaultPriceBand As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Do we need to update the profile first for season tickets. You can get redirected straight to this page by the flash movie
        If Talent.eCommerce.Utilities.DecodeString(Request.QueryString("type")) = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
            If Me.ModuleDefaults.UpdateDetailsForSeasonTickets And Not Profile.IsAnonymous Then
                Dim updatedWithin24Hours As Boolean = False
                Try
                    If CType(Membership.Provider, TalentMembershipProvider).GetUser(Profile.UserName, True).LastPasswordChangedDate > Now.AddDays(-1) Then
                        updatedWithin24Hours = True
                    End If
                Catch ex As Exception
                End Try
                If Not updatedWithin24Hours Then
                    Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx?ReturnUrl=" & Server.UrlEncode("~/PagesPublic/ProductBrowse/ProductSeason.aspx"))
                End If
            End If
        End If
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)

        With _wfr
            .BusinessUnit = _businessUnit
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "standAndAreaSelection.aspx"
        End With

        If Request.QueryString("campaign") IsNot Nothing Then
            _campaignCode = Request.QueryString("campaign").ToString()
            hdfCampaignCode.Value = Request.QueryString("campaign").ToString()
        End If

        If Request.QueryString("product") IsNot Nothing Then
            _productCode = Request.QueryString("product").ToString()
            hdfProductCode.Value = Request.QueryString("product").ToString()
        End If

        If Request.QueryString("type") IsNot Nothing Then
            _productType = Request.QueryString("type").ToString()
            hdfProductType.Value = Request.QueryString("type").ToString()
        End If

        If Request.QueryString("callId") IsNot Nothing Then
            hdfCallId.Value = Request.QueryString("callId").ToString()
        End If

        If Request.QueryString("productsubtype") IsNot Nothing Then _productSubType = Request.QueryString("productsubtype").ToString()

        If (Not String.IsNullOrWhiteSpace(TEBUtilities.DecodeString(Request.QueryString("catmode")))) Then
            hdfCATMode.Value = TEBUtilities.DecodeString(Request.QueryString("catmode"))
        End If

        If Request.QueryString("stadium") Is Nothing Then
            Response.Redirect(TEBUtilities.GetSiteHomePage())
        Else
            _stadiumCode = Request.QueryString("stadium").ToString()
            hdfStadiumCode.Value = Request.QueryString("stadium").ToString()
            setSeatingPlanImage()
            setStandAndAreaSelectionProperties()
        End If

        setBackLink()
        _errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        hdfPriceAndAreaSelection.Value = ModuleDefaults.PriceAndAreaSelection.ToString().ToLower()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ProcessCATSeatValidation()
        loadAdditionalInformation()
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Try
            If Not Session("TalentErrorCode") Is Nothing Then
                Dim myError As String = CStr(Session("TalentErrorCode"))
                ErrorList.Items.Add(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, TEBUtilities.GetCurrentPageName, myError).ERROR_MESSAGE)
                Session("TalentErrorCode") = Nothing
            End If
        Catch ex As Exception
        End Try
        plhErrorMessages.Visible = (ErrorList.Items.Count > 0)
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "stand-and-area-selection.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("stand-and-area-selection.js", "/Module/SeatSelection/"), False)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the "back" button url based on the querystring
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setBackLink()
        Dim navigateUrl As New StringBuilder
        Dim campaign As String = String.Empty
        Dim productType As String = String.Empty
        Dim productSubType As String = String.Empty
        If Request.QueryString("campaign") IsNot Nothing Then campaign = Request.QueryString("campaign").ToString()
        If Request.QueryString("type") IsNot Nothing Then productType = Request.QueryString("type").ToString()
        If Request.QueryString("productsubtype") IsNot Nothing Then productSubType = Request.QueryString("productsubtype").ToString()
        If productType = "S" Then
            navigateUrl.Append("~/PagesPublic/ProductBrowse/ProductSeason.aspx")
            hplBack.Text = _wfr.Content("SeasonTicketBackLinkText", _languageCode, True)
        Else
            navigateUrl.Append("~/PagesPublic/ProductBrowse/ProductHome.aspx")
            hplBack.Text = _wfr.Content("HomeGamesBackLinkText", _languageCode, True)
        End If
        If Not String.IsNullOrEmpty(productSubType) Then
            navigateUrl.Append("?productsubtype=")
            navigateUrl.Append(productSubType)
        End If
        hplBack.NavigateUrl = navigateUrl.ToString()
        plhBackButton.Visible = (hplBack.Text.Length > 0)
    End Sub

    ''' <summary>
    ''' Set the seating plan image based on the stadium code
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setSeatingPlanImage()
        imgSeatingPlan.AlternateText = _wfr.Content("SeatingPlanAltText", _languageCode, True)
        imgSeatingPlan.ImageUrl = ImagePath.getImagePath("STADIUMCODE", _stadiumCode, _businessUnit, _partnerCode)
        plhSeatingPlanImage.Visible = (imgSeatingPlan.ImageUrl <> ModuleDefaults.MissingImagePath)
    End Sub

    ''' <summary>
    ''' Setup the stand and area selection options based the product settings. Hide the "Go" button when visual seat selection is diabled
    ''' Also you cannot use visual seat selection when Alternative seat selection is enabled
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setStandAndAreaSelectionProperties()
        uscStandAndAreaSelection.CampaignCode = String.Empty
        uscStandAndAreaSelection.CampaignCode = _campaignCode
        uscStandAndAreaSelection.ProductCode = _productCode
        uscStandAndAreaSelection.ProductType = _productType
        uscStandAndAreaSelection.ProductSubType = _productSubType
        uscStandAndAreaSelection.ProductStadium = _stadiumCode
        uscStandAndAreaSelection.ProductDescription = ltlProductTitle.Text
        uscStandAndAreaSelection.SoldOut = False
        RetrieveProductDetails(uscStandAndAreaSelection.ProductCode)
        If _visualSeatSelection AndAlso Not _alternativeSeatSelection AndAlso Not _alertnativeSeatSelectionAcrossStands Then
            uscStandAndAreaSelection.HideBuyingOptions = False
        Else
            uscStandAndAreaSelection.HideBuyingOptions = True
        End If
        uscStandAndAreaSelection.AlternativeSeatSelection = _alternativeSeatSelection
        uscStandAndAreaSelection.AlternativeSeatSelectionAcrossStands = _alertnativeSeatSelectionAcrossStands
        uscStandAndAreaSelection.ProductHomeAsAway = _homeAsAway
        uscStandAndAreaSelection.ProductPriceBand = _defaultPriceBand
    End Sub

    ''' <summary>
    ''' Retreive the product details in order to determine the seat selection setting
    ''' </summary>
    ''' <param name="productCode">The product code to retrieve details for</param>
    ''' <remarks></remarks>
    Private Sub RetrieveProductDetails(ByVal productCode As String)
        Dim _talentErrObj As New Talent.Common.ErrorObj
        Dim _talentProduct As New Talent.Common.TalentProduct
        Dim _deSettings As Talent.Common.DESettings = TEBUtilities.GetSettingsObject
        _deSettings.Cacheing = True
        _talentProduct.Settings() = _deSettings
        _talentProduct.De.ProductCode = productCode
        _talentProduct.De.Src = GlobalConstants.SOURCE
        _talentErrObj = _talentProduct.ProductDetails
        If Not _talentErrObj.HasError AndAlso _talentProduct.ResultDataSet.Tables.Count > 1 Then
            If _talentProduct.ResultDataSet.Tables(2).Rows.Count > 0 Then
                _visualSeatSelection = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_talentProduct.ResultDataSet.Tables(2).Rows(0).Item("UseVisualSeatLevelSelection"))
                _alternativeSeatSelection = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_talentProduct.ResultDataSet.Tables(2).Rows(0).Item("AlternativeSeatSelection"))
                _alertnativeSeatSelectionAcrossStands = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_talentProduct.ResultDataSet.Tables(2).Rows(0).Item("AlternativeSeatSelectionAcrossStands"))
                _homeAsAway = TEBUtilities.CheckForDBNull_String(_talentProduct.ResultDataSet.Tables(2).Rows(0).Item("HomeAsAway"))
                _defaultPriceBand = TEBUtilities.CheckForDBNull_String(_talentProduct.ResultDataSet.Tables(2).Rows(0).Item("DefaultPriceBand"))
                ltlProductTitle.Text = TEBUtilities.CheckForDBNull_String(_talentProduct.ResultDataSet.Tables(2).Rows(0).Item("ProductDescription"))
                plhProductTitle.Visible = (ltlProductTitle.Text.Length > 0)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Load the addtional information option to the product if it is available, this changes the css class names of the game selection div tag
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub loadAdditionalInformation()
        Dim IncludeFolderName As String = "Other"
        Dim HTMLFound As Boolean = False
        If TEBUtilities.DoesHtmlFileExists(_wfr.BusinessUnit & "\" & _wfr.PartnerCode & "\Product\" & IncludeFolderName & "\" & _productCode & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfr.BusinessUnit & "\" & _wfr.PartnerCode & "\Product\" & IncludeFolderName & "\" & _productCode & ".html") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfr.BusinessUnit & "\" & _wfr.PartnerCode & "\Product\" & IncludeFolderName & "\" & _productSubType & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfr.BusinessUnit & "\" & _wfr.PartnerCode & "\Product\" & IncludeFolderName & "\" & _productSubType & ".html") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfr.BusinessUnit & "\" & _wfr.PartnerCode & "\Product\" & IncludeFolderName & "\" & _productType & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfr.BusinessUnit & "\" & _wfr.PartnerCode & "\Product\" & IncludeFolderName & "\" & _productType & ".html") Then
            HTMLFound = True
        End If

        If HTMLFound Then
            plhAdditionalInformation.Visible = True
            hlkAdditionalInformation.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductSummary.aspx?ProductCode=" & _productCode & "&ProductType=" & _productType & "&ProductSubType=" & _productSubType & "&IncludeFolderName=" & IncludeFolderName
            ltlMoreInfoText.Text = _wfr.Content("MoreInformationText", _languageCode, True)
        Else
            Dim pdfFound As Boolean = False
            Dim pdfLink As String = TEBUtilities.PDFLinkAvailable(_productCode, _productType, _productSubType, _wfr.BusinessUnit, _wfr.PartnerCode)
            If Not String.IsNullOrEmpty(pdfLink) Then
                plhAdditionalInformation.Visible = True
                hlkAdditionalInformation.Target = "_blank"
                hlkAdditionalInformation.NavigateUrl = pdfLink
                ltlMoreInfoText.Text = _wfr.Content("MoreInformationText", _languageCode, True)
            Else
                plhAdditionalInformation.Visible = False
            End If
        End If
    End Sub

#End Region

#Region "Web Methods"
    <System.Web.Services.WebMethod()> _
    Public Shared Function RetrieveDynamicStandAndAreaOptions(ByVal data As String, ByVal productCode As String, ByVal stadiumCode As String, ByVal productType As String, ByVal campaignCode As String, ByVal catMode As String, ByVal callId As String, ByVal includeTicketExchangeSeats As String, ByVal minimumPrice As String, ByVal maximumPrice As String, ByVal selectedStand As String, ByVal selectedArea As String) As String
        Dim productAvailabilityModelBuilder As New ProductAvailabilityBuilder
        Dim inputModel As New ProductAvailabilityInputModel
        Dim viewModel As New ProductAvailabilityViewModel
        Dim serializer As New JavaScriptSerializer
        Dim priceBreakIdValue As Long = TEBUtilities.CheckForDBNull_Long(data)
        Dim webMethodDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim defaultValues As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = webMethodDefaults.GetDefaults()
        With inputModel
            .AgentLevelCacheForProductStadiumAvailability = defaultValues.AgentLevelCacheForProductStadiumAvailability
            .CampaignCode = campaignCode
            .CapacityByStadium = defaultValues.CapacityByStadium
            .CATMode = catMode
            .ComponentID = CATHelper.GetPackageComponentId(productCode, callId)
            .IncludeTicketExchangeSeats = includeTicketExchangeSeats
            .PriceBreakId = priceBreakIdValue
            .ProductCode = productCode
            .ProductType = productType
            .Source = GlobalConstants.SOURCE
            .StandCode = selectedStand
            .AreaCode = selectedArea
            .StadiumCode = stadiumCode
            .SelectedMinimumPrice = TEBUtilities.CheckForDBNull_Decimal(maximumPrice)
            .SelectedMaximumPrice = TEBUtilities.CheckForDBNull_Decimal(minimumPrice)
            .DefaultPriceBand = _defaultPriceBand
        End With
        viewModel = productAvailabilityModelBuilder.GetProductAvailability(inputModel)
        Return serializer.Serialize(viewModel)
    End Function
#End Region
End Class