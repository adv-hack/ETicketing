Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class PagesPublic_productHome
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
        If myTicketingMenu.TicketingProductIsActive("HOME") Then
            With wfrPage
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = String.Empty
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
                .PageCode = .KeyCode
            End With
            errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        Else
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ucr As New Talent.Common.UserControlResource

        'Display the multi stadium text where applicable
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(wfrPage.Attribute("ShowMultiStadiumMainTextForHomeGames")) Then
            Me.multiStadiumMainTextLabel.Text = wfrPage.Content("MultiStadiumMainTextForHomeGames", _languageCode, True)
        Else
            Me.plhMultiStadiumText.Visible = False
        End If

        If Not Page.IsPostBack Then
            Try
                If Session("TicketingGatewayError") IsNot Nothing Then
                    ErrorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, wfrPage.PageCode, CStr(Session("TicketingGatewayError"))).ERROR_MESSAGE)
                    If Session("TalentErrorCode") = Session("TicketingGatewayError") Then Session("TalentErrorCode") = Nothing
                    Session("TicketingGatewayError") = Nothing
                End If
                If Not Session("TalentErrorCode") Is Nothing Then
                    Dim myError As String = CStr(Session("TalentErrorCode"))
                    ErrorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, wfrPage.PageCode, myError).ERROR_MESSAGE)
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