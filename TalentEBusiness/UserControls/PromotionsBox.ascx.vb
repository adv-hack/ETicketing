Imports System.Data
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Promotions Box
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCCODA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_PromotionsBox
    Inherits ControlBase
    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage


    Public Property PromotionCode() As String
        Get
            If (Session("CodeFromPromotionsBox") IsNot Nothing) Then
                Return Session("CodeFromPromotionsBox").ToString().Trim()
            Else
                Return promoBox.Text
            End If
        End Get
        Set(ByVal value As String)
            promoBox.Text = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PromotionsBox.ascx"
        End With
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If (Session("CodeFromPromotionsBox") IsNot Nothing) Then
                promoBox.Text = Session("CodeFromPromotionsBox").ToString().Trim()
            End If
            promoLabel.Text = ucr.Content("LabelText", _languageCode, True)
            promoButton.Text = ucr.Content("ButtonText", _languageCode, True)
            Dim successPromoCodes As String = String.Empty
            If Session("AllPrmotionCodesEnteredByUser") IsNot Nothing AndAlso Profile.Basket.WebPrices.PromotionsResultsTable IsNot Nothing Then
                Dim dtPromoCodesResult As DataTable = Profile.Basket.WebPrices.PromotionsResultsTable
                Dim drSuccessPromoCodes() As DataRow = dtPromoCodesResult.Select("Success=1 AND ActivationMechanism='CODE'")
                If drSuccessPromoCodes.Length > 0 Then
                    For rowIndex As Integer = 0 To drSuccessPromoCodes.Length - 1
                        If rowIndex = 0 Then
                            successPromoCodes = drSuccessPromoCodes(rowIndex)("PromotionCode")
                        Else
                            successPromoCodes = successPromoCodes & ";" & drSuccessPromoCodes(rowIndex)("PromotionCode")
                        End If
                    Next
                    Session("AllPrmotionCodesEnteredByUser") = successPromoCodes
                Else
                    Session("AllPrmotionCodesEnteredByUser") = ""
                End If
            End If
            If successPromoCodes.Length <= 0 Then
                promoBox.Text = ""
            End If
        End If
    End Sub

    Protected Sub promoButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles promoButton.Click
        Try
            Dim totals As Talent.Common.TalentWebPricing = Profile.Basket.WebPrices
            Dim currentPromoCode As String = promoBox.Text.Trim()
            If (currentPromoCode.Length > 0) Then
                Session("UserFromPromotionsBox") = Profile.UserName.Trim().ToLower()
                Session("CodeFromPromotionsBox") = promoBox.Text.Trim()
                If (Session("AllPrmotionCodesEnteredByUser") IsNot Nothing) Then
                    Dim PromotionCodes As String()
                    Dim charSeparators() As Char = {";"c}
                    PromotionCodes = (CStr(Session("AllPrmotionCodesEnteredByUser"))).Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries)
                    Dim totalCodesIndex As Integer = (PromotionCodes.Length - 1)
                    Dim addCode As Boolean = True
                    For codeCount As Integer = totalCodesIndex To 0 Step -1
                        If UCase(currentPromoCode) = UCase(PromotionCodes(codeCount)) Then
                            addCode = False
                            Exit For
                        End If
                    Next
                    If addCode Then
                        Session("AllPrmotionCodesEnteredByUser") = Session("AllPrmotionCodesEnteredByUser") & ";" & currentPromoCode
                    End If
                Else
                    Session("AllPrmotionCodesEnteredByUser") = currentPromoCode
                End If
            End If
            Session("RedirectByPromotionBox") = True

            Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderSummary.aspx")

        Catch ex As Exception

        End Try
    End Sub
    Protected Sub SetEnterKeyAction(ByVal sender As Object, ByVal e As EventArgs)
        Dim tbox As TextBox = CType(sender, TextBox)
        Const onKeyDown As String = "onkeydown"
        Dim EventText As String = "if(event.which || event.keyCode) " & _
                                    "{ " & _
                                        "if ((event.which == 13) || (event.keyCode == 13))" & _
                                        "{ " & _
                                            "document.getElementById('" & promoButton.ClientID & "').click();" & _
                                            "return false;" & _
                                        "} " & _
                                    "} " & _
                                    "else " & _
                                    "{ " & _
                                        "return true" & _
                                    "}; "
        tbox.Attributes.Add(onKeyDown, EventText)
    End Sub
End Class
