
Partial Class PagesPublic_DatePicker
    Inherits System.Web.UI.Page

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Nothing to do, here
    End Sub

    Private Sub Calendar1_DayRender(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DayRenderEventArgs) Handles Calendar1.DayRender
        '// Clear the link from this day
        e.Cell.Controls.Clear()

        '// Add the custom link
        Dim Link As System.Web.UI.HtmlControls.HtmlGenericControl
        Link = New System.Web.UI.HtmlControls.HtmlGenericControl
        Link.TagName = "a"
        Link.InnerText = e.Day.DayNumberText
        Link.Attributes.Add("href", String.Format("JavaScript:window.opener.document.{0}.value = '{1:d}'; window.close();", Request.QueryString("field"), e.Day.Date))

        '// By default, this will highlight today's date.
        If e.Day.IsSelected Then
            Link.Attributes.Add("style", Me.Calendar1.SelectedDayStyle.ToString())
        End If

        '// Now add our custom link to the page
        e.Cell.Controls.Add(Link)

    End Sub
End Class
