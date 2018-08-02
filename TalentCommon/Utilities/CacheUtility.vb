Imports System.Web

<Serializable()> _
Public Class CacheUtility

    Private _sessionId As String
    Public ReadOnly Property SessionId() As String
        Get
            If Utilities.IsSessionActive AndAlso Not (HttpContext.Current.Session Is Nothing) AndAlso Not (HttpContext.Current.Session.SessionID Is Nothing) Then
                _sessionId = HttpContext.Current.Session.SessionID
            Else
                _sessionId = String.Empty
            End If
            Return _sessionId
        End Get
    End Property

#Region "Public Methods"

    Public Sub AddItemToSession(ByVal sessionKey As String, ByVal cacheItem As Object)
        If Utilities.IsSessionActive Then
            HttpContext.Current.Session.Add(sessionKey, cacheItem)
        End If
    End Sub

    Public Sub RemoveItemFromSession(ByVal sessionKey As String)
        If Utilities.IsSessionActive Then
            HttpContext.Current.Session.Remove(sessionKey)
        End If
    End Sub

    Public Function GetItemFromSession(ByVal sessionKey As String) As Object
        Dim obj As Object = Nothing
        If Utilities.IsSessionActive Then
            obj = HttpContext.Current.Session(sessionKey)
        End If
        Return obj
    End Function

    Public Sub AddItemToCache(ByVal cacheKey As String, ByVal cacheItem As Object, ByVal settings As DESettings)
        AddItemToCache(cacheKey, cacheItem, settings, settings.ModuleName)
    End Sub

    Public Sub AddItemToCache(ByVal cacheKey As String, ByVal cacheItem As Object, ByVal settings As DESettings, ByVal cacheDependencyName As String)
        If Utilities.IsCacheActive AndAlso settings.Cacheing Then

            'How long are we cacheing the item for?
            Dim cacheDateTime As Date
            If settings.CacheTimeMinutes > 0 Then
                cacheDateTime = System.DateTime.Now.AddMinutes(settings.CacheTimeMinutes)
            ElseIf settings.CacheTimeSeconds > 0 Then
                cacheDateTime = System.DateTime.Now.AddSeconds(settings.CacheTimeSeconds)
            Else
                'No time set so cache for default of 30 minutes
                cacheDateTime = System.DateTime.Now.AddMinutes(30)
            End If
            '-------------------------------
            ' Check Dependency folder exists
            '-------------------------------
            If System.IO.Directory.Exists(settings.CacheDependencyPath) Then
                '------------------------------------------------
                ' Build path. Format is <RootPath> + <ModuleName>
                '------------------------------------------------
                Dim dependency As String = String.Empty
                If settings.CacheDependencyPath.Trim.EndsWith("\") Then
                    dependency = settings.CacheDependencyPath & cacheDependencyName
                Else
                    dependency = settings.CacheDependencyPath & "\" & cacheDependencyName
                End If
                HttpContext.Current.Cache.Insert(cacheKey, cacheItem, New Caching.CacheDependency(dependency), cacheDateTime, Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            Else
                HttpContext.Current.Cache.Insert(cacheKey, cacheItem, Nothing, cacheDateTime, Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End If
    End Sub

    Public Sub AddIemToCache(ByVal cacheKey As String, ByVal cacheItem As Object, ByVal cacheDateTime As Date)
        HttpContext.Current.Cache.Insert(cacheKey, cacheItem, Nothing, cacheDateTime, Caching.Cache.NoSlidingExpiration)
        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
    End Sub

    Public Sub RemoveItemFromCache(ByVal cacheKey As String)
        If Utilities.IsCacheActive Then
            Try
                HttpContext.Current.Cache.Remove(cacheKey)
                TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Function GetItemFromCache(ByVal cacheKey) As Object
        Return HttpContext.Current.Cache.Get(cacheKey)
    End Function

    Public Shared Function ClearCacheDependencyOnAllServers(ByVal BusinessUnit As String, ByVal CacheDependencyName As String) As Boolean
        Dim result As Boolean = False

        Return result
    End Function

#End Region

End Class
