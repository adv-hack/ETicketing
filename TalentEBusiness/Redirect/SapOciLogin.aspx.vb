
Partial Class Redirect_SapOciLogin
    Inherits TalentBase01
   
    'implementation is in the method SapOciAutoLogin in TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        HttpContext.Current.Response.AddHeader("p3p", "CP=""IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT""")


        If Session("ParameterError") IsNot Nothing Then
            If Session("ParameterError").ToString() = "INVALID" Then
                lblError.Text = "Invalid username or password"
                Session.Remove("SAP_OCI_HOOK_URL")
                Session.Remove("SAP_OCI_REFERER_URL")
            ElseIf Session("ParameterError").ToString() = "TRUE" Then
                lblError.Text = "Required Parameter is missing"
            ElseIf Session("ParameterError").ToString() = "CONFIG" Then
                lblError.Text = "Configuration is missing"
                Session.Remove("SAP_OCI_HOOK_URL")
                Session.Remove("SAP_OCI_REFERER_URL")
            End If
            Session.Remove("ParameterError")
            lblError.Visible = True
            If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then
                lnkReferer.NavigateUrl = "test" & HttpContext.Current.Request.UrlReferrer.ToString()
                lnkReferer.Text = "BACK"
                lnkReferer.Visible = True
            End If
        End If
    End Sub
End Class
