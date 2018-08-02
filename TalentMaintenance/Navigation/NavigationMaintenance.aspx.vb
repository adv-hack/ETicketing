Partial Class Navigation_NavigationMaintenance
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Page.Title = "Group Navigation Maintenance"
            Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
            master.HeaderText = "Group Navigation Maintenance"
            setLabel()
        End If
    End Sub

    Public Sub setLabel()
        Dim liGroupHomeLink As New ListItem
        liGroupHomeLink.Value = "NavigationMaintenance.aspx?BU="
        liGroupHomeLink.Value += Request.QueryString("BU")
        liGroupHomeLink.Value += "&partner="
        liGroupHomeLink.Value += Request.QueryString("partner")
        liGroupHomeLink.Text = "Navigation Overview"
        navigationOptions.Items.Add(liGroupHomeLink)
    End Sub
End Class
