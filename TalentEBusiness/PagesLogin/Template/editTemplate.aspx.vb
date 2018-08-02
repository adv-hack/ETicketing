Imports Microsoft.VisualBasic
Imports Talent.eCommerce

Partial Class PagesLogin_editTemplate
    Inherits TalentBase01

    Private _showOptions As Boolean = False

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If Not HttpContext.Current.Profile.IsAnonymous Then
            If Not Page.IsPostBack Then
                Dim templateAdapter As New OrderTemplatesDataSetTableAdapters.tbl_order_template_headerTableAdapter
                Dim templates As OrderTemplatesDataSet.tbl_order_template_headerDataTable
                templates = templateAdapter.GetBy_HeaderID(Request("hid"))
                Dim template As OrderTemplatesDataSet.tbl_order_template_headerRow

                If templates.Rows.Count > 0 Then
                    template = templates.Rows(0)
                    If template.LOGINID = Profile.UserName Then
                        If Not String.IsNullOrEmpty(template.NAME) Then
                            If CType(Me.Page, TalentBase01).PageHeaderText.Contains("</") Then
                                Dim index As Integer = CType(Me.Page, TalentBase01).PageHeaderText.IndexOf("</")
                                CType(Me.Page, TalentBase01).PageHeaderText = CType(Me.Page, TalentBase01).PageHeaderText.Insert(index, ": " & template.NAME)
                            Else
                                CType(Me.Page, TalentBase01).PageHeaderText = CType(Me.Page, TalentBase01).PageHeaderText & ": " & template.NAME
                            End If
                        End If
                        _showOptions = True
                    Else
                        If template.ALLOW_FF_TO_VIEW Then
                            CType(Me.Page, TalentBase01).PageHeaderText = "<h1>" & template.NAME & "</h1>"
                            Page.Title = template.NAME
                            _showOptions = False
                        Else
                            Response.Redirect("~/PagesLogin/Template/ViewTemplates.aspx")
                        End If
                    End If
                Else
                    Response.Redirect("~/PagesLogin/Template/ViewTemplates.aspx")
                End If
            End If
        Else
            Response.Redirect("~/PagesPublic/Login/login.aspx" & "?ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?" & Request.QueryString.ToString))
        End If
    End Sub

    Protected Sub Page_Load1(sender As Object, e As System.EventArgs) Handles Me.Load
        OrderTemplateDetails1.ShowOptions = _showOptions
    End Sub

End Class