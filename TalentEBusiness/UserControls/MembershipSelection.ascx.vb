Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class UserControls_MembershipSelection
    Inherits ControlBase

    Private _product As New TalentProduct
    Private _settings As New DESettings
    Private _err As New ErrorObj
    Private _ucr As New UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _errorList As New BulletedList
    Private _ProductCode As String
    Private _PriceCode As String
    Private _Enabled As Boolean
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
    Public Property PriceCode() As String
        Get
            Return _PriceCode
        End Get
        Set(ByVal value As String)
            _PriceCode = value
        End Set
    End Property

    Public Sub setHiddenFields()
        hfProductCode.Value = ProductCode
        hfPriceCode.Value = PriceCode
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "MembershipSelection.ascx"
        End With
        If ModuleDefaults.DefaultMembershipQuantity > 0 Then qtyTextBox.Text = ModuleDefaults.DefaultMembershipQuantity
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myLibrary As String = String.Empty
        qtyLabel.Text = _ucr.Content("qtyLabel", _languageCode, True)
        buyButton.Text = _ucr.Content("buyButton", _languageCode, True)
        myLibrary = _ucr.Attribute("websalesVLDLLibrary")
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
            Dim errorMessage As String = Talent.Common.Utilities.getErrorDescription(_ucr, _languageCode, errCode, False)
            If errorMessage = "" Then
                errorMessage = "There was an undefined error. Error Code:" & errCode
            End If
            CType(Me.Parent.TemplateControl.FindControl("errorlist"), BulletedList).Items.Clear()
            CType(Me.Parent.TemplateControl.FindControl("errorlist"), BulletedList).Items.Add(errorMessage)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub AddTicketingItems(ByVal sender As Object, ByVal e As EventArgs)
        ' Error if quantity not valid
        If Not (IsNumeric(qtyTextBox.Text.Trim)) Then
            showError("QV")
            Exit Sub
        Else
            If CType(qtyTextBox.Text, Integer) = 0 Then
                showError("QZ")
                Exit Sub
            End If
        End If
        Dim redirectString As New StringBuilder
        redirectString.Append("~/Redirect/TicketingGateway.aspx?page=ProductMembership.aspx&function=AddToBasket")
        redirectString.Append("&product=").Append(hfProductCode.Value.Trim)
        redirectString.Append("&quantity=").Append(qtyTextBox.Text.Trim)
        redirectString.Append("&priceCode=").Append(hfPriceCode.Value.Trim)
        If Request.QueryString("productsubtype") IsNot Nothing Then
            redirectString.Append("&productsubtype=").Append(Request.QueryString("productsubtype"))
        End If
        Response.Redirect(redirectString.ToString())
    End Sub
End Class