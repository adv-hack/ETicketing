Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Clear Cache
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPCCCAH- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_ClearCache
    Inherits TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BL1.Items.Clear()
        If String.IsNullOrWhiteSpace(Request.QueryString("CountOnly")) Then
            DisplayCache()
        Else
            Dim cacheCount As Integer = HttpContext.Current.Cache.Count
            BL1.Items.Add(cacheCount.ToString())
        End If
    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Page.Title = "Clear Cache"
    End Sub

    Protected Sub btnClearCache_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearCache.Click
        Dim objItem As DictionaryEntry
        For Each objItem In Cache
            Cache.Remove(objItem.Key.ToString())
        Next
        Dim cms As New Talent.Common.Cms
        cms.Clear_Cache()
        Try
            Dim objItem2 As DictionaryEntry
            For Each objItem2 In Application
                Application.Remove(objItem2.Key.ToString())
            Next
        Catch ex As Exception
        End Try
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

    Protected Sub DisplayCache()
        Dim e As IDictionaryEnumerator = HttpContext.Current.Cache.GetEnumerator
        While e.MoveNext
            BL1.Items.Add(e.Current.Key.ToString)
        End While

    End Sub
End Class
