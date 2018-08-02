
Partial Class _Default
    Inherits TalentBase01

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If Not Profile.IsAnonymous AndAlso CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.PARTNER_TYPE = "HOME" AndAlso isQuickOrderInUse() Then
                Response.Redirect("~/PagesLogin/QuickOrder/QuickOrder.aspx")
            Else
                Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Get the page "in use" flag for for the Quick Order page
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function isQuickOrderInUse() As Boolean
        Dim quickOrderInUse As Boolean = False
        Dim dtPage As New System.Data.DataTable
        dtPage = TDataObjects.PageSettings.TblPage.GetByPageCode("QuickOrder.aspx", TalentCache.GetBusinessUnit())
        If dtPage.Rows.Count > 0 Then
            quickOrderInUse = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dtPage.Rows(0)("IN_USE"))
        End If
        Return quickOrderInUse
    End Function

#End Region

End Class