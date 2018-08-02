
Partial Class Redirect_ProductSeasonRenewals
    Inherits TalentBase01

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Redirect("~/Redirect/TicketingGateway.aspx?page=home.aspx&function=AddSeasonTicketRenewalsToBasket")
    End Sub
End Class
