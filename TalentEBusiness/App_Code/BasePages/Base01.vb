Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
' This base page is designed to handle how the application behaves.  Functionality like: -
'
'       1) Browser Caching headers
'       2) Storing view state server side 
'       3) Application logging 
'
' is handled here.  
'--------------------------------------------------------------------------------------------------

Public Class Base01

    Inherits System.Web.UI.Page

    Public Property pageLogging As New Talent.Common.TalentLogging
    Private pagetimeSpan As TimeSpan

    Private Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        pagetimeSpan = Now.TimeOfDay
        pageLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
    End Sub

    Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        pageLogging.LoadTestLog(Talent.eCommerce.Utilities.GetCurrentPageName.Trim, "PageLoad", pagetimeSpan)
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load        'Set cache header values
        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1))
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()
    End Sub

End Class
