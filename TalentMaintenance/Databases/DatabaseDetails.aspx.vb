
Partial Class Databases_DatabaseDetails
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        If Request("ID") Is Nothing OrElse Request("ID").ToString = "new" Then
            'master.HeaderText = "Add Database Connection Details"
        Else
            'master.HeaderText = "Edit Databse Connection Details"
        End If
    End Sub
End Class
