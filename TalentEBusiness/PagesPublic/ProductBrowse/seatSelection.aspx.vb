Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Web.Script.Serialization

Partial Class PagesPublic_ProductBrowse_seatSelection
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _commonWfrPage As WebFormResource = Nothing
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#End Region

#Region "Public Properties"

    Public errMsg As Talent.Common.TalentErrorMessages

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Talent.eCommerce.Utilities.DecodeString(Request.QueryString("type")) = "S" Then
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
        _commonWfrPage = New WebFormResource
        With _commonWfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = TEBUtilities.GetCurrentPageName()
            .PageCode = TEBUtilities.GetCurrentPageName()
        End With

        setBackLink()
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("product") IsNot Nothing AndAlso Request.QueryString("product").ToString().Length > 0 _
            AndAlso useVisualSeatLevelSelection(Request.QueryString("product").ToString()) Then
            If Request.QueryString("type") IsNot Nothing AndAlso Request.QueryString("type").ToString().Length > 0 Then
                If Request.QueryString("stadium") IsNot Nothing AndAlso Request.QueryString("stadium").ToString().Length > 0 Then
                    uscSeatSelection.ProductCode = Request.QueryString("product")
                    uscSeatSelection.ProductType = Request.QueryString("type")
                    uscSeatSelection.StadiumCode = Request.QueryString("stadium")
                    If Request.QueryString("productsubtype") IsNot Nothing AndAlso Request.QueryString("productsubtype").ToString().Length > 0 Then
                        uscSeatSelection.ProductSubType = Request.QueryString("productsubtype")
                    Else
                        uscSeatSelection.ProductSubType = String.Empty
                    End If
                    If Request.QueryString("campaign") IsNot Nothing AndAlso Request.QueryString("campaign").ToString().Length > 0 Then
                        uscSeatSelection.CampaignCode = Request.QueryString("campaign")
                    Else
                        uscSeatSelection.CampaignCode = String.Empty
                    End If
                    If (Not String.IsNullOrWhiteSpace(TEBUtilities.DecodeString(Request.QueryString("catmode")))) Then
                        uscSeatSelection.IsCatMode = True
                        uscSeatSelection.CATMode = TEBUtilities.DecodeString(Request.QueryString("catmode"))
                    Else
                        uscSeatSelection.IsCatMode = False
                        uscSeatSelection.CATMode = String.Empty
                    End If
                    hdfPriceAndAreaSelection.Value = ModuleDefaults.PriceAndAreaSelection.ToString().ToLower()
                Else
                    Response.Redirect(TEBUtilities.GetSiteHomePage())
                End If
            Else
                Response.Redirect(TEBUtilities.GetSiteHomePage())
            End If
        Else
            Response.Redirect(TEBUtilities.GetSiteHomePage())
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_commonWfrPage.Attribute("SeatSelectionMiniAutoGenerated")) Then
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "seat-selection-mini.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("seat-selection-mini.js", "/Module/SeatSelection/"), False)
        Else
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "RaphaelSeatSelectionSeating.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("RaphaelSeatSelectionSeating.js", "/Module/SeatSelection/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "RaphaelSeatSelectionStadium.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("RaphaelSeatSelectionStadium.js", "/Module/SeatSelection/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "RaphaelSeatSelectionZoom.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("RaphaelSeatSelectionZoom.js", "/Module/SeatSelection/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "RaphaelSeatSelectionDragSelect.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("RaphaelSeatSelectionDragSelect.js", "/Module/SeatSelection/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "stand-and-area-selection.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("stand-and-area-selection.js", "/Module/SeatSelection/"), False)
        End If

        Try
            If Not Session("TalentErrorCode") Is Nothing Then
                Dim myError As String = CStr(Session("TalentErrorCode"))
                myError = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, myError).ERROR_MESSAGE
                Dim listItem As New ListItem
                listItem.Text = myError
                If Not ErrorList.Items.Contains(listItem) Then
                    ErrorList.Items.Add(listItem)
                End If
                If Session("TalentErrorCode") = Session("TicketingGatewayError") Then
                    Session("TicketingGatewayError") = Nothing
                End If
                Session("TalentErrorCode") = Nothing
            End If
            If Session("OrphanSeatError") IsNot Nothing Then
                Dim myError As String = CStr(Session("OrphanSeatError"))
                myError = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, myError).ERROR_MESSAGE
                Dim listItem As New ListItem
                listItem.Text = myError
                If Not ErrorList.Items.Contains(listItem) Then
                    ErrorList.Items.Add(listItem)
                End If
                Session("OrphanSeatError") = Nothing
            End If

            If ErrorList.Items.Count > 0 Then
                plhErrorMessages.Visible = True
            Else
                plhErrorMessages.Visible = False
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setBackLink()
        Dim navigateUrl As New StringBuilder
        Dim productType As String = String.Empty
        Dim productSubType As String = String.Empty
        Dim stadium As String = String.Empty
        Dim product As String = String.Empty
        Dim campaign As String = String.Empty
        Dim packageId As String = String.Empty
        Dim componentId As String = String.Empty
        If Request.QueryString("stadium") IsNot Nothing Then stadium = Request.QueryString("stadium").ToString()
        If Request.QueryString("type") IsNot Nothing Then productType = Request.QueryString("type").ToString()
        If Request.QueryString("productsubtype") IsNot Nothing Then productSubType = Request.QueryString("productsubtype").ToString()
        If Request.QueryString("product") IsNot Nothing Then product = Request.QueryString("product").ToString()
        If Request.QueryString("campaign") IsNot Nothing Then campaign = Request.QueryString("campaign").ToString()
        If Request.QueryString("packageId") IsNot Nothing Then packageId = Request.QueryString("packageId").ToString()
        If Request.QueryString("componentId") IsNot Nothing Then componentId = Request.QueryString("componentId").ToString()

        navigateUrl.Append("~/PagesPublic/ProductBrowse/standAndAreaSelection.aspx?")
        If Not String.IsNullOrEmpty(stadium) Then navigateUrl.Append("stadium=").Append(stadium).Append("&")
        If Not String.IsNullOrEmpty(product) Then navigateUrl.Append("product=").Append(product).Append("&")
        If Not String.IsNullOrEmpty(campaign) Then navigateUrl.Append("campaign=").Append(campaign).Append("&")
        If Not String.IsNullOrEmpty(productType) Then navigateUrl.Append("type=").Append(productType).Append("&")
        If Not String.IsNullOrEmpty(productSubType) Then navigateUrl.Append("productsubtype=").Append(productSubType).Append("&")
        hplBack.Text = _commonWfrPage.Content("StandAndAreaSelectionBackLinkText", _languageCode, True)
        hplBack.NavigateUrl = navigateUrl.ToString()
        plhBackButton.Visible = (hplBack.Text.Length > 0 AndAlso packageId.Length = 0)
    End Sub
    Private Function useVisualSeatLevelSelection(ByVal productCode As String) As Boolean
        Dim visualSeatLevelSelection As Boolean = False
        If Not String.IsNullOrEmpty(productCode) Then
            Dim talProduct As New TalentProduct
            Dim err As ErrorObj
            err = talProduct.ProductDetails(Talent.eCommerce.Utilities.GetSettingsObject, productCode)
            If Not err.HasError AndAlso talProduct.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" _
                AndAlso talProduct.ResultDataSet.Tables.Count >= 3 Then
                visualSeatLevelSelection = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(talProduct.ResultDataSet.Tables(2).Rows(0).Item("UseVisualSeatLevelSelection"))
            End If
        End If
        Return visualSeatLevelSelection
    End Function
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Get the seats for the given stand and area
    ''' </summary>
    ''' <param name="data">The stand and area code</param>
    ''' <param name="productCode">The product being added</param>
    ''' <param name="stadiumCode">The Stadium code of the product</param>
    ''' <param name="campaignCode">The Campaign code of the product</param>
    ''' <param name="callId">CallId</param>
    ''' <param name="currentExceptionSeat">curentExceptionSeat</param>
    ''' <param name="priceBreakId">PriceBreakId for the particular product's seat</param>
    ''' <param name="includeTicketExchangeSeats">Including Ticket Exchange Seats</param>
    ''' <param name="selectedMinimumPrice">Minimum price which is selected</param>
    ''' <param name="selectedMaximumPrice">Maximum price which is selected</param>
    ''' <param name="ticketExchangeMin">Minimum of ticket Exchnage</param>
    ''' <param name="ticketExchangeMax">Maximum of Ticket Exchange</param>
    ''' <param name="packageId">packageId of particular package being added</param>
    ''' <param name="componentId">ComponentId of particular component</param>
    ''' <param name="changeAllSeats">changeAllSeats flag is determined whether user can change all seats or not</param>
    ''' <returns>The seats as an XML string</returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function GetSeating(ByVal data As String, ByVal productCode As String, ByVal stadiumCode As String, ByVal campaignCode As String, ByVal callId As String, ByVal currentExceptionSeat As String, ByVal priceBreakId As String, ByVal includeTicketExchangeSeats As String, ByVal selectedMinimumPrice As String, ByVal selectedMaximumPrice As String, ByVal ticketExchangeMin As String, ByVal ticketExchangeMax As String, ByVal packageId As String, ByVal componentId As String, ByVal changeAllSeats As String) As String
        Dim wfrPage As WebFormResource = New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SeatSelection.aspx" 'Use of TEBUtilities.GetCurrentPageName() here gives "GetSeating" as the page code
        End With
        Dim tempString() As String = data.Split("-")
        Dim seatingArea As New Seating(productCode, stadiumCode, tempString(0), tempString(1), campaignCode, callId, currentExceptionSeat, priceBreakId, includeTicketExchangeSeats, selectedMinimumPrice, selectedMaximumPrice, ticketExchangeMin, ticketExchangeMax, packageId, componentId, TCUtilities.convertToBool(changeAllSeats))
        seatingArea.AvailabilityCaching = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(wfrPage.Attribute("AvailabilityCacheing"))
        seatingArea.AvailabilityCacheTime = TEBUtilities.CheckForDBNull_Int(wfrPage.Attribute("AvailabilityCacheTimeSeconds"))
        seatingArea.SeatNumberCaching = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(wfrPage.Attribute("SeatNumberCacheing"))
        seatingArea.SeatNumberCacheTime = TEBUtilities.CheckForDBNull_Int(wfrPage.Attribute("SeatNumberCacheTimeMinutes"))
        seatingArea.SeatRestrictionsCaching = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(wfrPage.Attribute("RestrictionCacheing"))
        seatingArea.SeatRestrictionsCacheTime = TEBUtilities.CheckForDBNull_Int(wfrPage.Attribute("RestrictionCacheTimeMinutes"))
        seatingArea.ReservedAndSoldSeatsCaching = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(wfrPage.Attribute("ReservedAndSoldSeatsCacheing"))
        seatingArea.ReservedAndSoldSeatsCacheTime = TEBUtilities.CheckForDBNull_Int(wfrPage.Attribute("ReservedAndSoldSeatsCacheTimeMinutes"))
        seatingArea.ReservationCodesCaching = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(wfrPage.Attribute("ReservationCodesCacheing"))
        seatingArea.ReservationCodesCacheTime = TEBUtilities.CheckForDBNull_Int(wfrPage.Attribute("ReservationCodesCacheTimeMinutes"))
        Return seatingArea.GetSeating()
    End Function

    ''' <summary>
    ''' Retrieve seat information for the given seat data
    ''' </summary>
    ''' <param name="data">The current seat we are retrieving data for</param>
    ''' <param name="productCode">The product being added</param>
    ''' <param name="campaignCode">The Campaign code of the product</param>
    ''' <param name="stadiumCode">The Stadium code of the product</param>
    ''' <param name="changeAllSeats">changeAllSeats flag is determined whether user can change all seats or not</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()>
    Public Shared Function GetSeatInformation(ByVal data As String, ByVal productCode As String, ByVal campaignCode As String, ByVal stadiumCode As String, ByVal changeAllSeats As String) As String
        Dim wfrPage As WebFormResource = New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SeatSelection.aspx" 'Use of TEBUtilities.GetCurrentPageName() here gives "GetSeating" as the page code
        End With
        data = Replace(data, "-", "/")
        Dim seatString() As String = data.Split("/")
        Dim seatingDetails As New Seating(productCode, stadiumCode, seatString(0), seatString(1), campaignCode, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, TCUtilities.convertToBool(changeAllSeats))
        seatingDetails.SeatDetailsCaching = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(wfrPage.Attribute("SeatDetailsCaching"))
        seatingDetails.SeatDetailsCacheTime = TEBUtilities.CheckForDBNull_Int(wfrPage.Attribute("SeatDetailsCacheTime"))
        Return seatingDetails.GetSeatingDetails(seatString(2), seatString(3))
    End Function

#End Region

End Class