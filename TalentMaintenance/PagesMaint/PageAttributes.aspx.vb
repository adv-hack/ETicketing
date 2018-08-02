Imports Talent.Common
Imports System.Data

Partial Class PagesMaint_PageAttributes
    Inherits System.Web.UI.Page
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Public Sub setLabel()
        TitleLabel.Text = wfrPage.Content("titleLabel", _languageCode, True)
        instructionsLabel.Text = wfrPage.Content("instructionsLabel", _languageCode, True)
        BusinessUnitlbl.Text = wfrPage.Content("businessUnitlbl", _languageCode, True)
        Partnerlbl.Text = wfrPage.Content("partnerlbl", _languageCode, True)
        PageNamelbl.Text = wfrPage.Content("PageNamelbl", _languageCode, True)
        Descriptionlbl.Text = wfrPage.Content("descriptionlbl", _languageCode, True)
        Return_To_Pagesbtn.Text = wfrPage.Content("return_To_Pagesbtn", _languageCode, True)
        Return_To_Page_Detailbtn.Text = wfrPage.Content("return_To_Page_Detailbtn", _languageCode, True)
        Define_HTML_Includesbtn.Text = wfrPage.Content("define_HTML_Includesbtn", _languageCode, True)
        Add_Page_Header_and_Titlebtn.Text = wfrPage.Content("return_To_Page_Header_and_Titlebtn", _languageCode, True)
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PageAttributes.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "PageAttributes.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then

            Dim temp1 As String = Request.QueryString("BusinessUnit")
            Dim temp2 As String = Request.QueryString("Partner")
            Dim temp3 As String = Request.QueryString("PageName")
            If String.IsNullOrEmpty(temp1) Or String.IsNullOrEmpty(temp2) Or String.IsNullOrEmpty(temp3) Then
                Response.Redirect("../MaintenancePortal.aspx")
            Else
                BusinessUnitlbl1.Text = Request.QueryString("BusinessUnit")
                Partnerlbl1.Text = Request.QueryString("Partner")
                PageNamelbl1.Text = Request.QueryString("PageName")
                setLabel()
                TitleLabel.Text &= " " + Descriptionlbl1.Text
            End If

            gvEditPageAttributes.DataBind()
        End If

    End Sub

    Protected Sub gvEditPageAttributes_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gvEditPageAttributes.RowCommand
        If e.CommandName = "Edit" Then
            hdfRowID.Value = CStr(e.CommandArgument)
            Dim dvEditPageAttributes As DataView
            dvEditPageAttributes = CType(SqlDataSource2.Select(DataSourceSelectArguments.Empty), DataView)
            If dvEditPageAttributes.Table.Rows.Count = 1 Then
                txtEditor.Text = dvEditPageAttributes.Table.Rows(0)("ATTR_VALUE").ToString
                gvEditPageAttributes.Visible = False
                plhTextEditor.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs)
        SqlDataSource1.Update()
        gvEditPageAttributes.DataBind()
        gvEditPageAttributes.Visible = True
        plhTextEditor.Visible = False
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        gvEditPageAttributes.Visible = True
        plhTextEditor.Visible = False
    End Sub

    Protected Sub Return_To_Page_Detailbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Return_To_Page_Detailbtn.Click
        Response.Redirect("../PagesMaint/Page-Details.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub Return_To_Pagesbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Return_To_Pagesbtn.Click
        Response.Redirect("../PagesMaint/PageOverview.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub Add_Page_Header_and_Titlebtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Add_Page_Header_and_Titlebtn.Click
        Response.Redirect("../PagesMaint/PageHeadersAndTitles.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Mode=Add")
    End Sub

    Protected Sub Define_HTML_Includesbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Define_HTML_Includesbtn.Click
        Response.Redirect("../PagesMaint/PageHtmlIncludes.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub
End Class
