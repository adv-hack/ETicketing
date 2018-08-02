<%@ Application Language="VB" %>
    
<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        '
        ' Set logging options in cache for use by Talent.Common - these are only reset by using ~/PagesAdmin/clearcache.aspx
        Const loggingOnOffCacheKey As String = "TalentCommonLoggingOnOff"
        Const logPathCacheKey As String = "TalentCommonLogPath"
        If ConfigurationManager.AppSettings("TalentCommonLoggingOnOff").ToString.Trim().ToUpper = "TRUE" Then
            HttpContext.Current.Cache.Insert(loggingOnOffCacheKey, True, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
            HttpContext.Current.Cache.Insert(logPathCacheKey, ConfigurationManager.AppSettings("TalentCommonLogPath").ToString.Trim, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
        Else
            HttpContext.Current.Cache.Insert(loggingOnOffCacheKey, False, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
            HttpContext.Current.Cache.Insert(logPathCacheKey, "", Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
        End If
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when an unhandled error occurs
        Dim ex As Exception = Server.GetLastError()
        Dim log As Talent.Common.TalentLogging = Talent.eCommerce.Utilities.TalentLogging
        log.ExceptionLog(ex.Message, ex, "GlobalExceptionLogging")
        If ex.InnerException IsNot Nothing Then log.ExceptionLog(ex.Message, ex.InnerException, "GlobalExceptionLogging")
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a session ends. 
        ' Note: The Session_End event is raised only when the sessionstate mode
        ' is set to InProc in the Web.config file. If session mode is set to StateServer 
        ' or SQLServer, the event is not raised.
    End Sub
    
    Sub Profile_MigrateAnonymous(ByVal sender As Object, ByVal e As ProfileMigrateEventArgs)
        Dim redirectUrl As String = Profile.Provider.MigrateBasketToUser(Profile.UserName, e.AnonymousID, Profile)
        AnonymousIdentificationModule.ClearAnonymousIdentifier()
        'Migrated Profile has to be refresh to have latest basket
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
        If Not String.IsNullOrEmpty(redirectUrl) Then
            Response.Redirect(redirectUrl)
        End If
        
    End Sub
       
</script>