Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class PagesLogin_Promotions_VoucherDetails
    Inherits TalentBase01

#Region "Private Members"
    Private _wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private vouchers As New Vouchers()
#End Region

#Region "Public Members"
    Public Property ImagePath() As String
    Public Property VoucherTitle() As String
    Public Property VoucherDescription() As String
    Public Property VoucherCode() As String
    Public Property VoucherId() As String
#End Region

#Region "Protected Methods and Events"

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With

        Dim promotionToShow As Boolean = False
        Dim retrieveUsedVoucher As Boolean = False
        If Request.QueryString("VoucherId") IsNot Nothing Then
            VoucherId = CheckForDBNull_Int(Request.QueryString("VoucherId"))
            If Request.QueryString("VoucherCode") IsNot Nothing Then
                VoucherCode = CheckForDBNull_String(Request.QueryString("VoucherCode"))
                If (Not Regex.IsMatch(VoucherCode, _wfr.Attribute("InvalidVoucherCode"))) Then
                    promotionToShow = False
                Else
                    If Request.QueryString("returnUrl").Contains("CheckoutOrderConfirmation.aspx") Then
                        retrieveUsedVoucher = True
                    Else
                        retrieveUsedVoucher = False
                    End If
                    promotionToShow = vouchers.GetVoucherDetails(Profile.UserName, VoucherId, ImagePath, VoucherTitle, VoucherDescription, retrieveUsedVoucher)
                End If
            End If
        End If

        If promotionToShow Then
            plhVoucherNotFound.Visible = False
            plhVoucherFound.Visible = True
            imgVoucher.Visible = (ImagePath.Length > 0)
        Else
            plhVoucherNotFound.Visible = True
            plhVoucherFound.Visible = False
            ltlVoucherNotFound.Text = _wfr.Content("NoVoucherFound", _languageCode, True)
        End If
    End Sub

#End Region


End Class
