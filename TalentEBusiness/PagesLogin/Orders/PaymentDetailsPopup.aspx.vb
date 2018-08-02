Imports Talent.eCommerce
Partial Class PagesLogin_Orders_PaymentdetailsPopup
    Inherits TalentBase01

#Region "Class Level Members"
    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = String.Empty
#End Region

#Region "Public Methods"

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage

        OrderEnquiry21.Visible = False
        TransactionDetails1.Visible = False
        If Utilities.IsAgent() Then

            Dim payRef As String = String.Empty
            payRef = Request("payref")
            Session("filterPayRef") = payRef

            With _wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "TransactionDetails.aspx"
            End With
            If Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Show_OrderEnquiry2_Control")) Then
                OrderEnquiry21.Visible = True
                TransactionDetails1.Visible = False
            Else
                OrderEnquiry21.Visible = False
                TransactionDetails1.Visible = True
            End If

        End If
    End Sub

#End Region

#Region "Private Methods"

#End Region


End Class