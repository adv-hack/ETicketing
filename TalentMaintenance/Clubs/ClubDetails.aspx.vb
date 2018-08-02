Option Strict On
Partial Class Clubs_ClubDetails
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)

        If Request("clubID") Is Nothing OrElse Request("clubID").ToString = "new" Then
            'master.HeaderText = "Add Club"
        Else
            'master.HeaderText = "Change Club Details"
        End If
    End Sub
End Class
