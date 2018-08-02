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
Partial Class UserControls_ProductMagicZoomAdditionalImages
    Inherits System.Web.UI.UserControl

    Public MZDefault As String = String.Empty
    Public MZ1S As String = String.Empty
    Public MZ1M As String = String.Empty
    Public MZ1L As String = String.Empty
    Public MZ2S As String = String.Empty
    Public MZ2M As String = String.Empty
    Public MZ2L As String = String.Empty
    Public MZ3S As String = String.Empty
    Public MZ3M As String = String.Empty
    Public MZ3L As String = String.Empty
    Public MZ4S As String = String.Empty
    Public MZ4M As String = String.Empty
    Public MZ4L As String = String.Empty
    Public MZ5S As String = String.Empty
    Public MZ5M As String = String.Empty
    Public MZ5L As String = String.Empty
    Public MZ6S As String = String.Empty
    Public MZ6M As String = String.Empty
    Public MZ6L As String = String.Empty
    Public MZ7S As String = String.Empty
    Public MZ7M As String = String.Empty
    Public MZ7L As String = String.Empty
    Public MZ8S As String = String.Empty
    Public MZ8M As String = String.Empty
    Public MZ8L As String = String.Empty
    Public MZ9S As String = String.Empty
    Public MZ9M As String = String.Empty
    Public MZ9L As String = String.Empty
    Public MZ10S As String = String.Empty
    Public MZ10M As String = String.Empty
    Public MZ10L As String = String.Empty
    Public MZ11S As String = String.Empty
    Public MZ11M As String = String.Empty
    Public MZ11L As String = String.Empty
    Public MZ12S As String = String.Empty
    Public MZ12M As String = String.Empty
    Public MZ12L As String = String.Empty
    Public MagicZoomVisibility As Boolean = False

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

    Dim moduleDefaults As ECommerceModuleDefaults
    Dim def As ECommerceModuleDefaults.DefaultValues
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

        moduleDefaults = New ECommerceModuleDefaults
        def = moduleDefaults.GetDefaults

        With ucr

            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductMagicZoomAdditionalImages.ascx"

        End With

        MZ1S = ImagePath.getImagePath("MAGICZOOM1S", Request("product"), _businessUnit, _partner)
        MZ1M = ImagePath.getImagePath("MAGICZOOM1M", Request("product"), _businessUnit, _partner)
        MZ1L = ImagePath.getImagePath("MAGICZOOM1L", Request("product"), _businessUnit, _partner)
        If MZ1M = "" Or MZ1M = String.Empty Or MZ1M = "/Product/MagicZoom/1/M/" Or MZ1M = def.MissingImagePath Then
            MZMain.Visible = False
        End If

        If (MZ1S = "" Or MZ1S = String.Empty Or MZ1S = "/Product/MagicZoom/1/S/" Or MZ1S = def.MissingImagePath) And
            (MZ1M = "" Or MZ1M = String.Empty Or MZ1M = "/Product/MagicZoom/1/M/" Or MZ1M = def.MissingImagePath) And
            (MZ1L = "" Or MZ1L = String.Empty Or MZ1L = "/Product/MagicZoom/1/L/" Or MZ1L = def.MissingImagePath) Then
            MZ1.Visible = False
        End If

        MZ2S = ImagePath.getImagePath("MAGICZOOM2S", Request("product"), _businessUnit, _partner)
        MZ2M = ImagePath.getImagePath("MAGICZOOM2M", Request("product"), _businessUnit, _partner)
        MZ2L = ImagePath.getImagePath("MAGICZOOM2L", Request("product"), _businessUnit, _partner)
        If (MZ2S = "" Or MZ2S = String.Empty Or MZ2S = "/Product/MagicZoom/2/S/" Or MZ2S = def.MissingImagePath) And
            (MZ2M = "" Or MZ2M = String.Empty Or MZ2M = "/Product/MagicZoom/2/M/" Or MZ2M = def.MissingImagePath) And
            (MZ2L = "" Or MZ2L = String.Empty Or MZ2L = "/Product/MagicZoom/2/L/" Or MZ2L = def.MissingImagePath) Then
            MZ2.Visible = False
        End If

        MZ3S = ImagePath.getImagePath("MAGICZOOM3S", Request("product"), _businessUnit, _partner)
        MZ3M = ImagePath.getImagePath("MAGICZOOM3M", Request("product"), _businessUnit, _partner)
        MZ3L = ImagePath.getImagePath("MAGICZOOM3L", Request("product"), _businessUnit, _partner)

        If (MZ3S = "" Or MZ3S = String.Empty Or MZ3S = "/Product/MagicZoom/3/S/" Or MZ3S = def.MissingImagePath) And
            (MZ3M = "" Or MZ3M = String.Empty Or MZ3M = "/Product/MagicZoom/3/M/" Or MZ3M = def.MissingImagePath) And
            (MZ3L = "" Or MZ3L = String.Empty Or MZ3L = "/Product/MagicZoom/3/L/" Or MZ3L = def.MissingImagePath) Then
            MZ3.Visible = False
        End If

        MZ4S = ImagePath.getImagePath("MAGICZOOM4S", Request("product"), _businessUnit, _partner)
        MZ4M = ImagePath.getImagePath("MAGICZOOM4M", Request("product"), _businessUnit, _partner)
        MZ4L = ImagePath.getImagePath("MAGICZOOM4L", Request("product"), _businessUnit, _partner)
        If (MZ4S = "" Or MZ4S = String.Empty Or MZ4S = "/Product/MagicZoom/4/S/" Or MZ4S = def.MissingImagePath) And
            (MZ4M = "" Or MZ4M = String.Empty Or MZ4M = "/Product/MagicZoom/4/M/" Or MZ4M = def.MissingImagePath) And
            (MZ4L = "" Or MZ4L = String.Empty Or MZ4L = "/Product/MagicZoom/4/L/" Or MZ4L = def.MissingImagePath) Then
            MZ4.Visible = False
        End If

        MZ5S = ImagePath.getImagePath("MAGICZOOM5S", Request("product"), _businessUnit, _partner)
        MZ5M = ImagePath.getImagePath("MAGICZOOM5M", Request("product"), _businessUnit, _partner)
        MZ5L = ImagePath.getImagePath("MAGICZOOM5L", Request("product"), _businessUnit, _partner)
        If (MZ5S = "" Or MZ5S = String.Empty Or MZ5S = "/Product/MagicZoom/5/S/" Or MZ5S = def.MissingImagePath) And
            (MZ5M = "" Or MZ5M = String.Empty Or MZ5M = "/Product/MagicZoom/5/M/" Or MZ5M = def.MissingImagePath) And
            (MZ5L = "" Or MZ5L = String.Empty Or MZ5L = "/Product/MagicZoom/5/L/" Or MZ5L = def.MissingImagePath) Then
            MZ5.Visible = False
        End If

        MZ6S = ImagePath.getImagePath("MAGICZOOM6S", Request("product"), _businessUnit, _partner)
        MZ6M = ImagePath.getImagePath("MAGICZOOM6M", Request("product"), _businessUnit, _partner)
        MZ6L = ImagePath.getImagePath("MAGICZOOM6L", Request("product"), _businessUnit, _partner)
        If (MZ6S = "" Or MZ6S = String.Empty Or MZ6S = "/Product/MagicZoom/6/S/" Or MZ6S = def.MissingImagePath) And
            (MZ6M = "" Or MZ6M = String.Empty Or MZ6M = "/Product/MagicZoom/6/M/" Or MZ6M = def.MissingImagePath) And
            (MZ6L = "" Or MZ6L = String.Empty Or MZ6L = "/Product/MagicZoom/6/L/" Or MZ6L = def.MissingImagePath) Then
            MZ6.Visible = False
        End If

        MZ7S = ImagePath.getImagePath("MAGICZOOM7S", Request("product"), _businessUnit, _partner)
        MZ7M = ImagePath.getImagePath("MAGICZOOM7M", Request("product"), _businessUnit, _partner)
        MZ7L = ImagePath.getImagePath("MAGICZOOM7L", Request("product"), _businessUnit, _partner)
        If (MZ7S = "" Or MZ7S = String.Empty Or MZ7S = "/Product/MagicZoom/7/S/" Or MZ7S = def.MissingImagePath) And
            (MZ7M = "" Or MZ7M = String.Empty Or MZ7M = "/Product/MagicZoom/7/M/" Or MZ7M = def.MissingImagePath) And
            (MZ7L = "" Or MZ7L = String.Empty Or MZ7L = "/Product/MagicZoom/7/L/" Or MZ7L = def.MissingImagePath) Then
            MZ7.Visible = False
        End If

        MZ8S = ImagePath.getImagePath("MAGICZOOM8S", Request("product"), _businessUnit, _partner)
        MZ8M = ImagePath.getImagePath("MAGICZOOM8M", Request("product"), _businessUnit, _partner)
        MZ8L = ImagePath.getImagePath("MAGICZOOM8L", Request("product"), _businessUnit, _partner)
        If (MZ8S = "" Or MZ8S = String.Empty Or MZ8S = "/Product/MagicZoom/8/S/" Or MZ8S = def.MissingImagePath) And
            (MZ8M = "" Or MZ8M = String.Empty Or MZ8M = "/Product/MagicZoom/8/M/" Or MZ8M = def.MissingImagePath) And
            (MZ8L = "" Or MZ8L = String.Empty Or MZ8L = "/Product/MagicZoom/8/L/" Or MZ8L = def.MissingImagePath) Then
            MZ8.Visible = False
        End If

        MZ9S = ImagePath.getImagePath("MAGICZOOM9S", Request("product"), _businessUnit, _partner)
        MZ9M = ImagePath.getImagePath("MAGICZOOM9M", Request("product"), _businessUnit, _partner)
        MZ9L = ImagePath.getImagePath("MAGICZOOM9L", Request("product"), _businessUnit, _partner)
        If (MZ9S = "" Or MZ9S = String.Empty Or MZ9S = "/Product/MagicZoom/9/S/" Or MZ9S = def.MissingImagePath) And
            (MZ9M = "" Or MZ9M = String.Empty Or MZ9M = "/Product/MagicZoom/9/M/" Or MZ9M = def.MissingImagePath) And
            (MZ9L = "" Or MZ9L = String.Empty Or MZ9L = "/Product/MagicZoom/9/L/" Or MZ9L = def.MissingImagePath) Then
            MZ9.Visible = False
        End If

        MZ10S = ImagePath.getImagePath("MAGICZOOM10S", Request("product"), _businessUnit, _partner)
        MZ10M = ImagePath.getImagePath("MAGICZOOM10M", Request("product"), _businessUnit, _partner)
        MZ10L = ImagePath.getImagePath("MAGICZOOM10L", Request("product"), _businessUnit, _partner)
        If (MZ10S = "" Or MZ10S = String.Empty Or MZ10S = "/Product/MagicZoom/10/S/" Or MZ10S = def.MissingImagePath) And
            (MZ10M = "" Or MZ10M = String.Empty Or MZ10M = "/Product/MagicZoom/10/M/" Or MZ10M = def.MissingImagePath) And
            (MZ10L = "" Or MZ10L = String.Empty Or MZ10L = "/Product/MagicZoom/10/L/" Or MZ10L = def.MissingImagePath) Then
            MZ10.Visible = False
        End If

        MZ11S = ImagePath.getImagePath("MAGICZOOM11S", Request("product"), _businessUnit, _partner)
        MZ11M = ImagePath.getImagePath("MAGICZOOM11M", Request("product"), _businessUnit, _partner)
        MZ11L = ImagePath.getImagePath("MAGICZOOM11L", Request("product"), _businessUnit, _partner)
        If (MZ11S = "" Or MZ11S = String.Empty Or MZ11S = "/Product/MagicZoom/11/S/" Or MZ11S = def.MissingImagePath) And
            (MZ11M = "" Or MZ11M = String.Empty Or MZ11M = "/Product/MagicZoom/11/M/" Or MZ11M = def.MissingImagePath) And
            (MZ11L = "" Or MZ11L = String.Empty Or MZ11L = "/Product/MagicZoom/11/L/" Or MZ11L = def.MissingImagePath) Then
            MZ11.Visible = False
        End If

        MZ12S = ImagePath.getImagePath("MAGICZOOM12S", Request("product"), _businessUnit, _partner)
        MZ12M = ImagePath.getImagePath("MAGICZOOM12M", Request("product"), _businessUnit, _partner)
        MZ12L = ImagePath.getImagePath("MAGICZOOM12L", Request("product"), _businessUnit, _partner)
        If (MZ12S = "" Or MZ12S = String.Empty Or MZ12S = "/Product/MagicZoom/12/S/" Or MZ12S = def.MissingImagePath) And
            (MZ12M = "" Or MZ12M = String.Empty Or MZ12M = "/Product/MagicZoom/12/M/" Or MZ12M = def.MissingImagePath) And
            (MZ12L = "" Or MZ12L = String.Empty Or MZ12L = "/Product/MagicZoom/12/L/" Or MZ12L = def.MissingImagePath) Then
            MZ12.Visible = False
        End If

        If (MZDefaultImage.Visible = False _
            And MZ1.Visible = False _
            And MZ2.Visible = False _
            And MZ3.Visible = False _
            And MZ4.Visible = False _
            And MZ5.Visible = False _
            And MZ6.Visible = False _
            And MZ7.Visible = False _
            And MZ8.Visible = False _
            And MZ9.Visible = False _
            And MZ10.Visible = False _
            And MZ11.Visible = False _
            And MZ12.Visible = False) Then
            Display = False
        Else
            Display = True
        End If

        If (MZMain.Visible = False) Then
            Display = False
            ImageZoom.Text = ""
        Else
            ImageZoom.Text = ucr.Content("MagicZoomImageZoomLabel", _languageCode, True)
            Display = True
            If String.IsNullOrEmpty(ImageZoom.Text) Then
                ImageZoomLabel.Visible = False
            End If
        End If


        If (MZ1.Visible = False _
            And MZ2.Visible = False _
            And MZ3.Visible = False _
            And MZ4.Visible = False _
            And MZ5.Visible = False _
            And MZ6.Visible = False _
            And MZ7.Visible = False _
            And MZ8.Visible = False _
            And MZ9.Visible = False _
            And MZ10.Visible = False _
            And MZ11.Visible = False _
            And MZ12.Visible = False) Then
            AlternativeView.Text = ""
            AlternativeImages.Visible = False
        Else
            AlternativeView.Text = ucr.Content("MagicZoomAlternativeViewLabel", _languageCode, True)
            If String.IsNullOrEmpty(AlternativeView.Text) Then
                AlternativeViewLabel.Visible = False
            End If
        End If

            If Display Then
            pnlProductMagicZoomAdditionalImages.Visible = True
            Else
            pnlProductMagicZoomAdditionalImages.Visible = False
            End If

            If Not Page.IsPostBack Then

        End If

        MZDefault = ImagePath.getImagePath("MAGICZOOMDEFAULT", Request("product"), _businessUnit, _partner)
        If MZDefault = "" Or MZDefault = String.Empty Or MZDefault = "/Product/MagicZoom/Default/" Or MZDefault = def.MissingImagePath Then
            MZDefaultImage.Visible = False
        Else
            MZImages.Visible = False
        End If

        MagicZoomVisibility = MZMain.Visible
    End Sub


End Class
