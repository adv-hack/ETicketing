Imports Microsoft.VisualBasic
Imports Talent.eCommerce

Public Class BaseManager
    Public moduleDefaults As New ECommerceModuleDefaults
    Public def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()
    Public frontEndConnectionString As String = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
    Protected _businessUnit As String = TalentCache.GetBusinessUnit
    Protected _partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
    Protected _currentPage As String = Talent.eCommerce.Utilities.GetCurrentPageName()
End Class
