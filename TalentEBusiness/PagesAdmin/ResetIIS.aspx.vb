
Partial Class PagesAdmin_ResetIIS

    Inherits Base01

    Protected Sub loadIIS_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles loadIIS.Click

        Dim file As String
        file = Server.MapPath("../web.config")
        Dim sr As New System.IO.StreamReader(file)
        Session("webConfig") = sr.ReadToEnd
        sr.Close()
        sr.Dispose()

        lblWebConfig.Text = "IIS Has been loaded."

        loadIIS.Visible = False
        resetIIS.Visible = True


    End Sub
    Protected Sub resetIIS_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles resetIIS.Click

        Dim file As String
        file = Server.MapPath("../web.config")
        Dim sw As New System.IO.StreamWriter(file)
        sw.Write(Session("webConfig"))
        sw.Close()
        sw.Dispose()

        lblWebConfig.Text = "IIS Has been reset."
        resetIIS.Visible = False

    End Sub
End Class
