
Partial Class Redirect_GoogleCheckoutGateway
    Inherits Base01

#Region "Class Level Fields"

    Private _googleCheckout As GoogleCheckout = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _googleCheckout = New GoogleCheckout("GoogleCheckoutGateway.aspx")
        _googleCheckout.ProcessGoogleCheckout()
    End Sub

#End Region

End Class
