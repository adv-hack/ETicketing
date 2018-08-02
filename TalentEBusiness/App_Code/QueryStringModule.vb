#Region "Using"

Imports System.IO
Imports System.Web
Imports System.Text
Imports System.Security.Cryptography

#End Region

''' <summary>
''' Summary description for QueryStringModule
''' </summary>
Public Class QueryStringModule
    Implements IHttpModule



#Region "IHttpModule Members"

    Public Sub Dispose() Implements IHttpModule.Dispose
        ' Nothing to dispose
    End Sub

    Public Sub Init(ByVal app As HttpApplication) Implements IHttpModule.Init
        AddHandler app.BeginRequest, AddressOf Me.context_BeginRequest
    End Sub

#End Region

    Private Sub context_BeginRequest(sender As Object, e As EventArgs)

        Dim context As HttpContext = HttpContext.Current
        If context.Request.Url.OriginalString.Contains("aspx") AndAlso context.Request.RawUrl.Contains("?") Then
            Dim query As String = Talent.Common.Utilities.ExtractURLQuery(context.Request.RawUrl)
            Dim path As String = Talent.Common.Utilities.GetURLVirtualPath()
            Dim decryptedQuery As String
            If query.StartsWith(GlobalConstants.ENCRYPTED_QUERYSTRING_PARAMETER_NAME, StringComparison.OrdinalIgnoreCase) Then

                Dim talentDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
                Dim dt As Data.DataTable
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                settings.DestinationDatabase = "SQL2005"
                talentDataObjects.Settings = settings
                dt = talentDataObjects.AppVariableSettings.TblQuerystring.GetActiveByObfuscatedQueryString(query)
                If dt.Rows.Count > 0 Then
                    ' Use replacement querystring form file.
                    decryptedQuery = dt.Rows.Item(0)("QUERYSTRING_VALUE")
                    context.RewritePath(path, String.Empty, decryptedQuery)
                Else
                    ' Decrypt Querystring
                    Try
                        Dim rawQuery As String = query.Replace(GlobalConstants.ENCRYPTED_QUERYSTRING_PARAMETER_NAME, String.Empty)
                        decryptedQuery = Talent.Common.Utilities.QueryStringDecrypt(settings, TalentCache.GetBusinessUnit, rawQuery)
                        context.RewritePath(path, String.Empty, decryptedQuery)
                    Catch ex As Exception
                    End Try
                End If

                ' Encrypt the query string and redirects to the encrypted URL.
                ' Remove if you don't want all query strings to be encrypted automatically.
            ElseIf context.Request.HttpMethod = "GET" Then
                'Dim encryptedQuery As String = Talent.Common.Utilities.QueryStringEncrypt(TalentCache.GetBusinessUnit, query)
                'context.Response.Redirect(path & "?" & encryptedQuery)
            End If
        End If
    End Sub



End Class