Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Recommend this product to a friend Form
'
'       Date                        Mar 2007
'
'       Author                      Andy White  
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCRECFRI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'   todo currently DOB controls allow [31 February]  etc. so need date validation bit 
'
'--------------------------------------------------------------------------------------------------

Partial Class UserControls_RecommendForm
    Inherits System.Web.UI.UserControl
    Protected _emailSubject As String = String.Empty
    Protected _emailText As String = String.Empty
    Protected _emailHTMLText As String = String.Empty
    Protected ucr As New Talent.Common.UserControlResource
    Protected _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub recommendBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RecommendBtn.Click
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        Try
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RecommendForm.ascx"
                _emailSubject = .Content("emailSubject", _languageCode, True)
                _emailText = .Content("emailText", _languageCode, True)
                _emailHTMLText = .Content("emailHTMLText", _languageCode, True)
            End With

            Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
            Dim emailFormat As String = ucr.Attribute("ConfimationEMailBodyHTML")

            If emailFormat.ToUpper = "PLAINTEXT" Then
                Dim sLink As String = Left(Request.Url.AbsoluteUri, InStr(Request.Url.AbsoluteUri, "/PagesPublic") - 1) & "/PagesPublic/ProductBrowse/Product.aspx" & Request.Url.Query & vbCrLf
                _emailText = Replace(_emailText, "<<To>>", FriendsNameTextBox.Text)
                _emailText = Replace(_emailText, "<<From>>", YourNameTextBox.Text)
                _emailText = Replace(_emailText, "<<Link>>", vbCrLf & sLink)
                _emailText = Replace(_emailText, "<<Body>>", MessageTextBox.Text)
                _emailText = Replace(_emailText, "<<NewLine>>", vbCrLf)
                err = Talent.Common.Utilities.Email_Send(YourEmailTextBox.Text, FriendsEmailTextBox.Text, _emailSubject, _emailText)
            ElseIf emailFormat.ToUpper = "HTML" Then
                Dim sLink As String = Left(Request.Url.AbsoluteUri, InStr(Request.Url.AbsoluteUri, "/PagesPublic") - 1) & "/PagesPublic/ProductBrowse/Product.aspx" & Request.Url.Query & "<br>"
                _emailHTMLText = Replace(_emailHTMLText, "<<To>>", FriendsNameTextBox.Text)
                _emailHTMLText = Replace(_emailHTMLText, "<<From>>", YourNameTextBox.Text)
                _emailHTMLText = Replace(_emailHTMLText, "<<Link>>", "<br>" & sLink)
                _emailHTMLText = Replace(_emailHTMLText, "<<Body>>", MessageTextBox.Text)
                _emailHTMLText = Replace(_emailHTMLText, "<<NewLine>>", "<br>")
                err = Talent.Common.Utilities.Email_Send(YourEmailTextBox.Text, FriendsEmailTextBox.Text, _
                                                    _emailSubject, _emailHTMLText, "", False, True)
            End If
        Catch ex As Exception
            Exit Sub
        End Try

        If Request.QueryString("ReturnUrl") Is Nothing Then
            Response.Redirect("~/PagesPublic/ProductBrowse/recommendProductConfirmation.aspx")
        Else
            Response.Redirect(Request.QueryString("ReturnUrl"))
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            '
             With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RecommendForm.ascx"

                RecommendTitleLabel.Text = .Content("RecommendTitleLabel", _languageCode, True)

                '----------------------------------------------------------------------------
                YourNameLabel.Text = .Content("YourNameLabel", _languageCode, True)
                YourNameRFV.ErrorMessage = .Content("YourNameErrorText", _languageCode, True)
                YourEmailLabel.Text = .Content("YourEmailLabel", _languageCode, True)
                YourEmailRFV.ErrorMessage = .Content("YourEmailErrorText", _languageCode, True)
                YourEmailRegEx.ValidationExpression = .Attribute("YourEmailRegEx")
                YourEmailRegEx.ErrorMessage = .Content("YourEmailRegExErrorText", _languageCode, True)

                FriendsNameLabel.Text = .Content("FriendsNameLabel", _languageCode, True)
                FriendsNameRFV.ErrorMessage = .Content("FriendsNameErrorText", _languageCode, True)
                FriendsEmailLabel.Text = .Content("FriendsEmailLabel", _languageCode, True)
                FriendsEmailRFV.ErrorMessage = .Content("FriendsEmailErrorText", _languageCode, True)
                FriendsEmailRegEx.ValidationExpression = .Attribute("FriendsEmailRegEx")
                FriendsEmailRegEx.ErrorMessage = .Content("FriendsEmailRegExErrorText", _languageCode, True)

                MessageLabel.Text = .Content("MessageLabel", _languageCode, True)
                MessageTextBoxRFV.ErrorMessage = .Content("MessageTextBoxRFV", _languageCode, True)
                RecommendBtn.Text = .Content("RecommendBtn", _languageCode, True)

            End With
        End If
    End Sub
End Class
