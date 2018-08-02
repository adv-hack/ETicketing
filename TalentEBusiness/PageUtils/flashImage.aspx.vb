Imports System.IO.MemoryStream
Imports System.Drawing
Imports System.Convert

Partial Class flashImage
    Inherits Base01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Clear()
        Try
            Dim surname As String = Request.QueryString("surname")
			Dim number As String = Request.QueryString("number")
            Dim streamStr As String = Request.Form.Item(0)
            Dim stream As Byte() = Convert.FromBase64String(streamStr)
            Dim ms As System.IO.MemoryStream = New System.IO.MemoryStream(stream)
            Dim img As System.Drawing.Image = Image.FromStream(ms)
            Response.ContentType = "image/jpeg"
            img.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg)
        Catch ex As Exception
            Response.Write("Error: " & ex.Message)
        End Try
    End Sub
End Class
