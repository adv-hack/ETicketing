Option Strict On
Partial Class Partners_PartnerDetails
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        If Request("ID") Is Nothing OrElse Request("ID").ToString = "new" Then
            '----------------
            ' Set to add mode
            '----------------
            master.HeaderText = "Add Partner"
        Else
            '-------------------
            ' Set to change mode
            '-------------------
            master.HeaderText = "Change Partner"
        End If

    End Sub

End Class
