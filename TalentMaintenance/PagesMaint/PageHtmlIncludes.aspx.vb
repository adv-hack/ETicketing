Imports testdatasetTableAdapters
Imports Talent.Common
Partial Class _Default
    Inherits System.Web.UI.Page
    Dim productsAdapter As New tbl_page_htmlTableAdapter
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _isHtmlIncludeEditEnabled As Boolean = True
    Private _isHtmlIncludeDeleteEnabled As Boolean = True


    Public Sub setLabel()
        TitleLabel.Text = wfrPage.Content("titleLabel", _languageCode, True)
        InstructionsLabel.Text = wfrPage.Content("instructionsLabel", _languageCode, True)
        BusinessUnitlbl.Text = wfrPage.Content("businessUnitlbl", _languageCode, True)
        Partnerlbl.Text = wfrPage.Content("partnerlbl", _languageCode, True)
        Descriptionlbl.Text = wfrPage.Content("descriptionlbl", _languageCode, True)

        PageNamelbl.Text = wfrPage.Content("PageNamelbl", _languageCode, True)
        Descriptionlbl1.Text = Convert.ToString(productsAdapter.GetDescription(Request.QueryString("BusinessUnit").ToString, Request.QueryString("Partner").ToString, Request.QueryString("PageName")))

        Return_To_Pagesbtn.Text = wfrPage.Content("return_To_Pagesbtn", _languageCode, True)
        Return_To_Page_Detailbtn.Text = wfrPage.Content("return_To_Page_Detailbtn", _languageCode, True)
        Page_Headers_and_Titlesbtn.Text = wfrPage.Content("page_Headers_and_Titlesbtn", _languageCode, True)
        Add_HTML_Includebtn.Text = wfrPage.Content("add_HTML_Includebtn", _languageCode, True)

    End Sub
    Public Sub RowDelete(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(source, Button)
        productsAdapter.DeletebyId(CType(btn.CommandName, Integer))
        GridView1.DataBind()
    End Sub
    Public Sub RowEdit(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As LinkButton = TryCast(source, LinkButton)
        productsAdapter.DeletebyId(CType(btn.CommandName, Integer))
        GridView1.DataBind()
    End Sub
    Public Sub Editdata(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(source, Button)
        Dim id As String = CType(btn.CommandName, String)

        Response.Redirect("../PagesMaint/PageAddHtmlInclude.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Mode=" + "Edit" + "&Id=" + id)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim PAGE_HTML_EDIT As Button = CType(e.Row.FindControl("PAGE_HTML_EDIT"), Button)
            Dim PAGE_HTML_DELETE As Button = CType(e.Row.FindControl("PAGE_HTML_DELETE"), Button)
            PAGE_HTML_EDIT.Enabled = _isHtmlIncludeEditEnabled
            PAGE_HTML_DELETE.Enabled = _isHtmlIncludeDeleteEnabled
            PAGE_HTML_EDIT.Visible = _isHtmlIncludeEditEnabled
            PAGE_HTML_DELETE.Visible = _isHtmlIncludeDeleteEnabled
        End If
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        With wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PageHtmlIncludes.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "PageHtmlIncludes.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        IsAuthorizedRole()
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsPostBack = False Then


            Dim temp1 As String = Request.QueryString("BusinessUnit")
            Dim temp2 As String = Request.QueryString("Partner")
            Dim temp3 As String = Request.QueryString("PageName")
            Dim temp4 As String = Request.QueryString("PageId")
            If String.IsNullOrEmpty(temp1) Or String.IsNullOrEmpty(temp2) Or String.IsNullOrEmpty(temp3) Or String.IsNullOrEmpty(temp4) Then
                Response.Redirect("../MaintenancePortal.aspx")
            Else
                BusinessUnitlbl1.Text = Request.QueryString("BusinessUnit")
                Partnerlbl1.Text = Request.QueryString("Partner")
                PageNamelbl1.Text = Request.QueryString("PageName")
                PageNamelbl1.Text = Request.QueryString("PageId")
                setLabel()
                TitleLabel.Text &= " " + Descriptionlbl1.Text
            End If
        End If
        DisableControls()

    End Sub

    Protected Sub Add_HTML_Includebtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Add_HTML_Includebtn.Click
        Response.Redirect("../PagesMaint/PageAddHtmlInclude.aspx?PageName=" + Request.QueryString("PageName") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Mode=" + "Add" + "&PageId=" + Request.QueryString("PageId"))
    End Sub

    Protected Sub Page_Headers_and_Titlesbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Page_Headers_and_Titlesbtn.Click
        Response.Redirect("../PagesMaint/PageHeadersAndTitles.aspx?PageName=" + Request.QueryString("PageName") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&PageId=" + Request.QueryString("PageId"))
    End Sub

    Protected Sub Return_To_Page_Detailbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Return_To_Page_Detailbtn.Click
        Response.Redirect("../PagesMaint/Page-Details.aspx?PageName=" + Request.QueryString("PageName") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&PageId=" + Request.QueryString("PageId"))
    End Sub

    Protected Sub Return_To_Pagesbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Return_To_Pagesbtn.Click
        Response.Redirect("../PagesMaint/PageOverview.aspx?PageName=" + Request.QueryString("PageName") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Private Sub DisableControls()
        Page_Headers_and_Titlesbtn.Enabled = Utilities.isEnabled("AllowPageHeadersAndTitles")
        Add_HTML_Includebtn.Enabled = Utilities.isEnabled("AllowAddHTMLInclude")
        Page_Headers_and_Titlesbtn.Visible = Page_Headers_and_Titlesbtn.Enabled
        Add_HTML_Includebtn.Visible = Add_HTML_Includebtn.Enabled
        _isHtmlIncludeEditEnabled = Utilities.isEnabled("AllowEditHTMLInclude")
        _isHtmlIncludeDeleteEnabled = Utilities.isEnabled("AllowDeleteHTMLInclude")
    End Sub
    Private Sub IsAuthorizedRole()
        If Not Utilities.isEnabled("DefineHTMLIncludes") Then
            Response.Redirect("../Error.aspx?Type=UNAUTH")
        End If
    End Sub
End Class
