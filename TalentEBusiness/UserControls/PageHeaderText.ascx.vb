
Partial Class UserControls_PageHeaderText
    Inherits ControlBase

    Public Property AddH1Tag As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If CType(Me.Page, TalentBase01).PageHeaderText IsNot Nothing Then
            If AddH1Tag Then
                ltlHeader.Text = "<h1>" & CType(Me.Page, TalentBase01).PageHeaderText & "</h1>"
            Else
                ltlHeader.Text = CType(Me.Page, TalentBase01).PageHeaderText
            End If
        End If
        If CType(Me.Page, TalentBase01).PageProductBrowseIntroText IsNot Nothing Then ltlProductBrowseIntro.Text = CType(Me.Page, TalentBase01).PageProductBrowseIntroText
        If CType(Me.Page, TalentBase01).PageProductBrowseHtml IsNot Nothing Then ltlProductBrowseHTML.Text = CType(Me.Page, TalentBase01).PageProductBrowseHtml

        plhProductBrowseIntro.Visible = (ltlProductBrowseIntro.Text.Length > 0)
        plhProductBrowseHTML.Visible = (ltlProductBrowseHTML.Text.Length > 0)
    End Sub

End Class
