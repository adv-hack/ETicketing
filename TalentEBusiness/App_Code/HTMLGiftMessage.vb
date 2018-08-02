Imports Microsoft.VisualBasic
Imports System.Data.SqlClient

Public Class HTMLGiftMessage

    Private __TitleText As String
    Private __NameIntroText As String
    Private __NameOutroText As String
    Private __FontSize As String
    Private __Fonts As String

    Public Property TitleText() As String
        Get
            Return __TitleText
        End Get
        Set(ByVal value As String)
            __TitleText = value
        End Set
    End Property

    Public Property NameIntroText() As String
        Get
            Return __NameIntroText
        End Get
        Set(ByVal value As String)
            __NameIntroText = value
        End Set
    End Property

    Public Property NameOutroText() As String
        Get
            Return __NameOutroText
        End Get
        Set(ByVal value As String)
            __NameOutroText = value
        End Set
    End Property

    Public Property FontSize() As String
        Get
            Return __FontSize
        End Get
        Set(ByVal value As String)
            __FontSize = value
        End Set
    End Property

    Public Property Fonts() As String
        Get
            Return __Fonts
        End Get
        Set(ByVal value As String)
            __Fonts = value
        End Set
    End Property

    Public Sub New(ByVal _TitleText As String, _
                    ByVal _NameIntroText As String, _
                    ByVal _NameOutroText As String, _
                    ByVal _FontSize As String, _
                    ByVal _Fonts As String)

        MyBase.New()

        Me.TitleText = _TitleText
        Me.NameIntroText = _NameIntroText
        Me.NameOutroText = _NameOutroText
        Me.FontSize = _FontSize
        Me.Fonts = _Fonts
    End Sub

    Public Function writeHTMLMessage(ByVal tempOrderID As String, Optional ByVal WebOrderID As String = "") As Boolean

        If String.IsNullOrEmpty(WebOrderID) Then WebOrderID = tempOrderID
        Dim success As Boolean = True
        Dim moduleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        Dim path As String = ConfigurationManager.AppSettings("giftMessageHTMLPath")
        If Not path.EndsWith("/") Then
            path += "/"
        End If
        path += WebOrderID & ".html"

        Dim message As String = String.Empty
        Dim name As String = String.Empty
        Dim doubleQuote As String = """"
        Dim sw As System.IO.StreamWriter = Nothing

        sw = New System.IO.StreamWriter(HttpContext.Current.Server.MapPath(path.ToString), True, System.Text.Encoding.Unicode)


        Dim SQLString As String = " SELECT * " & _
                                  " FROM tbl_gift_message WITH (NOLOCK)  " & _
                                  " WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                  " AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                  " AND PARTNER = @PARTNER "

        Dim cmd As New SqlCommand(SQLString, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString))
        Dim dr As SqlDataReader = Nothing

        '--------------------
        ' Add the parameters
        '--------------------
        With cmd.Parameters
            .Clear()
            .Add("TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = tempOrderID
            .Add("BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
            .Add("PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With

        Try
            sw.WriteLine("<HEAD>")
            sw.WriteLine("<TITLE>" & TitleText & WebOrderID & "</TITLE>")
            sw.WriteLine("<style type=" & doubleQuote & "text/css" & doubleQuote & ">")
            sw.WriteLine("<!--")
            sw.WriteLine("body {")
            sw.WriteLine("	font: normal " & FontSize & " " & Fonts & ";")
            sw.WriteLine("	color: #000;")
            sw.WriteLine("}")
            sw.WriteLine("-->")
            sw.WriteLine("</style>")
            sw.WriteLine("</HEAD>")
            sw.WriteLine("<BODY>")

            ' ------------------------------------------------------------
            ' Open connection to DB
            ' ------------------------------------------------------------
            cmd.Connection.Open()
            dr = cmd.ExecuteReader()

            If dr.HasRows Then
                dr.Read()
                message = dr("MESSAGE").trim
                ' ------------------------------------------------------------
                ' Replace CR/LF with <br>
                ' ------------------------------------------------------------
                Dim crlf As String = Chr(13) & Chr(10)
                message = message.Replace(crlf, "<br>")
                name = dr("RECIPIENT_NAME")
            End If

            sw.WriteLine("<p>")
            sw.WriteLine(NameIntroText & name.Trim & NameOutroText)
            sw.WriteLine("</p>")
            sw.WriteLine("<p>")
            sw.WriteLine(message.Trim)
            sw.WriteLine("</p>")
            sw.WriteLine("</BODY>")
            sw.Flush()

        Catch ex As Exception
            success = False
        Finally
            Try
                sw.Close()
            Catch ex As Exception
            End Try
            Try
                dr.Close()
            Catch ex As Exception
            End Try
            Try
                cmd.Connection.Close()
            Catch ex As Exception
            End Try
        End Try

        Return success
    End Function

End Class
