Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Product Information Template 2
'
'       Date                        07/08/07
'
'       Author                      Ben
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_ProductAdditionalImages
    Inherits ControlBase

    Private _businessUnit As String
    Private _partner As String
    Private _currentPage As String
    Private _PageNumber As Integer = 1
    Private _HasRows As Boolean = False
    Private _IsPaging As Boolean = False
    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private emptyString As String = String.Empty
    Private conTalent As SqlConnection = Nothing
    Private cmdSelect As SqlCommand = Nothing
    Private dtrProduct As SqlDataReader = Nothing
    Private _display As Boolean = True
    Private ucr As New Talent.Common.UserControlResource

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property
    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()
        Dim productCode As String = Request("product")

        With ucr

            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductAdditionalImages.ascx"

        End With

        Img1.ImageUrl = ImagePath.getImagePath("ALTIMAGE1", Request("product"), _businessUnit, _partner)
        If Img1.ImageUrl = "" Or Img1.ImageUrl = String.Empty Or Img1.ImageUrl = "/Product/Alt/1/" Or Img1.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img1.Visible = False
        End If
        Img2.ImageUrl = ImagePath.getImagePath("ALTIMAGE2", Request("product"), _businessUnit, _partner)
        If Img2.ImageUrl = "" Or Img2.ImageUrl = String.Empty Or Img2.ImageUrl = "/Product/Alt/2/" Or Img2.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img2.Visible = False
        End If
        Img3.ImageUrl = ImagePath.getImagePath("ALTIMAGE3", Request("product"), _businessUnit, _partner)
        If Img3.ImageUrl = "" Or Img3.ImageUrl = String.Empty Or Img3.ImageUrl = "/Product/Alt/3/" Or Img3.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img3.Visible = False
        End If
        Img4.ImageUrl = ImagePath.getImagePath("ALTIMAGE4", Request("product"), _businessUnit, _partner)
        If Img4.ImageUrl = "" Or Img4.ImageUrl = String.Empty Or Img4.ImageUrl = "/Product/Alt/4/" Or Img4.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img4.Visible = False
        End If
        Img5.ImageUrl = ImagePath.getImagePath("ALTIMAGE5", Request("product"), _businessUnit, _partner)
        If Img5.ImageUrl = "" Or Img5.ImageUrl = String.Empty Or Img5.ImageUrl = "/Product/Alt/5/" Or Img5.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img5.Visible = False
        End If
        Img6.ImageUrl = ImagePath.getImagePath("ALTIMAGE6", Request("product"), _businessUnit, _partner)
        If Img6.ImageUrl = "" Or Img6.ImageUrl = String.Empty Or Img6.ImageUrl = "/Product/Alt/6/" Or Img6.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img6.Visible = False
        End If
        Img7.ImageUrl = ImagePath.getImagePath("ALTIMAGE7", Request("product"), _businessUnit, _partner)
        If Img7.ImageUrl = "" Or Img7.ImageUrl = String.Empty Or Img7.ImageUrl = "/Product/Alt/7/" Or Img7.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img7.Visible = False
        End If
        Img8.ImageUrl = ImagePath.getImagePath("ALTIMAGE8", Request("product"), _businessUnit, _partner)
        If Img8.ImageUrl = "" Or Img8.ImageUrl = String.Empty Or Img8.ImageUrl = "/Product/Alt/8/" Or Img8.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img8.Visible = False
        End If
        Img9.ImageUrl = ImagePath.getImagePath("ALTIMAGE9", Request("product"), _businessUnit, _partner)
        If Img9.ImageUrl = "" Or Img9.ImageUrl = String.Empty Or Img9.ImageUrl = "/Product/Alt/9/" Or Img9.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img9.Visible = False
        End If
        Img10.ImageUrl = ImagePath.getImagePath("ALTIMAGE10", Request("product"), _businessUnit, _partner)
        If Img10.ImageUrl = "" Or Img10.ImageUrl = String.Empty Or Img10.ImageUrl = "/Product/Alt/10/" Or Img10.ImageUrl = ModuleDefaults.MissingImagePath Then
            Img10.Visible = False
        End If

        If (Img1.Visible = False And Img2.Visible = False And Img3.Visible = False And Img4.Visible = False And Img5.Visible = False And Img6.Visible = False And Img7.Visible = False And Img8.Visible = False And Img9.Visible = False And Img10.Visible = False) Then
            Display = False
            MoreImages.Text = ""
        Else
            MoreImages.Text = ucr.Content("MoreImagesText", _languageCode, True)
            Display = True
        End If


        If Display Then
            pnlProductAdditionalImages.Visible = True
        Else
            pnlProductAdditionalImages.Visible = False
        End If

        If Not Page.IsPostBack Then

        End If

    End Sub

End Class
