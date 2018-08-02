Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class UserControls_AwaySeatSelection
    Inherits ControlBase

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _ProductCode As String = String.Empty
    Private _ProductType As String = String.Empty
    Private _ProductPriceBand As String = String.Empty
    Private _HasAssociatedTravelProduct As Boolean = False
    Private _Enabled As Boolean = True

    Public Property Enabled() As Boolean
        Get
            Return _Enabled
        End Get
        Set(ByVal value As Boolean)
            _Enabled = value
        End Set
    End Property
    Public Property ProductCode() As String
        Get
            Return _ProductCode
        End Get
        Set(ByVal value As String)
            _ProductCode = value
        End Set
    End Property
    Public Property ProductType() As String
        Get
            Return _ProductType
        End Get
        Set(ByVal value As String)
            _ProductType = value
        End Set
    End Property
    Public Property ProductPriceBand() As String
        Get
            Return _ProductPriceBand
        End Get
        Set(ByVal value As String)
            _ProductPriceBand = value
        End Set
    End Property
    Public Property HasAssociatedTravelProduct() As Boolean
        Get
            Return _HasAssociatedTravelProduct
        End Get
        Set(ByVal value As Boolean)
            Try
                _HasAssociatedTravelProduct = value
            Catch ex As Exception
                _HasAssociatedTravelProduct = False
            End Try
        End Set
    End Property
    Private _PriceCode As String
    Public Property PriceCode() As String
        Get
            Return _PriceCode
        End Get
        Set(ByVal value As String)
            _PriceCode = value
        End Set
    End Property
    Dim errorLabel As New Label

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AwaySeatSelection.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        qtyLabel.Text = ucr.Content("qtyLabel", _languageCode, True)
        buyButton.Text = ucr.Content("buyButton", _languageCode, True)
        Dim tGatewayFunctions As New TicketingGatewayFunctions
        Dim minQuantity As Integer = 0
        Dim maxQuantity As Integer = 0
        Dim defaultQuantity As String = String.Empty
        Dim isReadOnly As Boolean = False
        tGatewayFunctions.GetQuantityDefintions(ProductCode, minQuantity, maxQuantity, defaultQuantity, isReadOnly)
        qtyTextBox.Attributes.Add("min", minQuantity)
        qtyTextBox.Attributes.Add("max", maxQuantity)
        qtyTextBox.MaxLength = maxQuantity.ToString().Length
    End Sub

    Protected Sub showError(ByVal errCode As String)
        Try
            Dim errorMessage As String = getErrText("error" & errCode)
            If errorMessage = "" Then
                errorMessage = "There was an undefined error. Error Code:" & errCode
            End If
            CType(Me.Parent.TemplateControl.FindControl("errorlist"), BulletedList).Items.Clear()
            CType(Me.Parent.TemplateControl.FindControl("errorlist"), BulletedList).Items.Add(errorMessage)
        Catch ex As Exception
        End Try
    End Sub

    Protected Function getErrText(ByVal pCode As String) As String
        Dim wfrPage As New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        Dim s As String
        s = wfrPage.Content(pCode, _languageCode, True)
        Return s
    End Function

    Public Sub SetHiddenFields()
        hfProductCode.Value = ProductCode
        hfProductPriceBand.Value = ProductPriceBand
        hfPriceCode.Value = PriceCode
        If HasAssociatedTravelProduct Then
            Try
                If CBool(ucr.Attribute("displayIncludeTravel")) Then
                    plhTravelProduct.Visible = True
                    lblIncludeTravel.Text = ucr.Content("includeTravelLabel", _languageCode, True)
                Else
                    plhTravelProduct.Visible = False
                End If
            Catch
                plhTravelProduct.Visible = False
            End Try
        Else
            plhTravelProduct.Visible = False
        End If
    End Sub

    Protected Sub AddTicketingItems(ByVal sender As Object, ByVal e As EventArgs)
        If Not (IsNumeric(qtyTextBox.Text.Trim)) Then
            showError("QV")
            Exit Sub
        Else
            If CType(qtyTextBox.Text, Integer) = 0 Then
                showError("QZ")
                Exit Sub
            End If
        End If
        Dim includeTravel As String = "N"
        If chkIncludeTravel.Checked = True Then includeTravel = "Y"
        Dim redirectString As New StringBuilder
        redirectString.Append("~/Redirect/TicketingGateway.aspx?page=ProductAway.aspx&function=AddToBasket")
        redirectString.Append("&product=").Append(hfProductCode.Value.Trim)
        redirectString.Append("&quantity=").Append(qtyTextBox.Text.Trim)
        redirectString.Append("&includetravel=").Append(includeTravel)
        redirectString.Append("&priceCode=").Append(hfPriceCode.Value.Trim)
        If Request.QueryString("productsubtype") IsNot Nothing Then
            redirectString.Append("&productsubtype=").Append(Request.QueryString("productsubtype"))
        End If
        Response.Redirect(redirectString.ToString())
    End Sub

End Class