Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class PagesPublic_ProductBrowse_ProductSeason
    Inherits TalentBase01

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private wfrPage As New WebFormResource
    Private errMsg As TalentErrorMessages
    Private _productSubTypeFromQS As String = String.Empty

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        AssignQueryStringValues()
        ProductDetail1.ProductSubType = _productSubTypeFromQS.ToUpper()
        Dim myTicketingMenu As New TalentTicketingMenu
        myTicketingMenu.LoadTicketingProducts(TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), Talent.eCommerce.Utilities.GetCurrentLanguage)
        If myTicketingMenu.TicketingProductIsActive("SEASONSALES") Then
            If ModuleDefaults.UpdateDetailsForSeasonTickets And Not Profile.IsAnonymous Then
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

            With wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = String.Empty
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            End With
            errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        Else
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Display the multi stadium text where applicable
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(wfrPage.Attribute("ShowMultiStadiumMainTextForSeasonTickets")) Then
            Me.multiStadiumMainTextLabel.Text = wfrPage.Content("MultiStadiumMainTextForSeasonTickets", _languageCode, True)
        Else
            Me.plhMultiStadiumText.Visible = False
        End If

        If Not Page.IsPostBack Then
            Try
                If Session("TicketingGatewayError") IsNot Nothing Then
                    ErrorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, CStr(Session("TicketingGatewayError"))).ERROR_MESSAGE)
                    If Session("TalentErrorCode") = Session("TicketingGatewayError") Then Session("TalentErrorCode") = Nothing
                    Session("TicketingGatewayError") = Nothing
                End If
                If Not Session("TalentErrorCode") Is Nothing Then
                    Dim myError As String = CStr(Session("TalentErrorCode"))
                    ErrorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, myError).ERROR_MESSAGE)
                    Session("TalentErrorCode") = Nothing
                End If
            Catch ex As Exception
            End Try
        End If
        ErrorList.Visible = (ErrorList.Items.Count > 0)
    End Sub

    Private Sub AssignQueryStringValues()
        If Not String.IsNullOrEmpty(Request.QueryString("ProductSubType")) Then
            _productSubTypeFromQS = Request.QueryString("ProductSubType").Trim
        End If
    End Sub

End Class