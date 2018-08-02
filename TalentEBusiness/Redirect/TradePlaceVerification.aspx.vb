Imports Talent.Common
Partial Class Redirect_TradePlaceVerification
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim username As String = String.Empty
        Dim signatureReceived As String = String.Empty
        Dim userpassword As String = String.Empty
        If Request.QueryString("userid") IsNot Nothing _
            And Request.QueryString("signature") IsNot Nothing _
            And Request.QueryString("password") IsNot Nothing Then
            username = Request.QueryString("userid")
            signatureReceived = Request.QueryString("signature").Replace(" ", "+")
            userpassword = Request.QueryString("password")

            Dim ecomModuleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim ecomModuleDefaultValues As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = ecomModuleDefaults.GetDefaults
            Dim securityKey As String = ecomModuleDefaultValues.TradePlaceEncryptionKey
            Dim valueToCreateSignature As String = username & userpassword
            Dim signatureCreated As String = String.Empty
            signatureCreated = Utilities.SHA1TripleDESEncode(valueToCreateSignature, securityKey)
            If signatureReceived = signatureCreated Then
                Dim validUser As Boolean = False
                validUser = Talent.eCommerce.Utilities.loginUser(username, userpassword)
                If validUser Then
                    Response.Write("ed=""True""")
                End If
            Else
                'ToDo: What to do
            End If
        End If
    End Sub
End Class
