
Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class UserControls_BannerWrapper
    Inherits ControlBase
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public ucr As New Talent.Common.UserControlResource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()
        Dim pagename As String = Talent.eCommerce.Utilities.GetCurrentPageName()
        Dim businessUnit As String = TalentCache.GetBusinessUnit

        If Not Page.IsPostBack Then
            If CType(def.enableBannerControl, Boolean) Then
                bannersUC.Visible = True
            Else
                bannersUC.Visible = False
            End If
        End If

    End Sub

End Class
