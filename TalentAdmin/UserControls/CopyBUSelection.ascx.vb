Option Strict On
Partial Class UserControls_copyBUSelection
    Inherits System.Web.UI.UserControl

    Protected Sub btnCopy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCopy.Click
        Response.Redirect("~/Server/WebSite/CopyBU/CopyBU.aspx?ID=" & ddlBU.SelectedValue)
    End Sub

End Class
