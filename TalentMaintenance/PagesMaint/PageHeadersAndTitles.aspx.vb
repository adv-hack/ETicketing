Imports PageHeaderAndTitlesTableAdapters
Imports Talent.Common
Imports System.Data

Partial Class PagesMaint_PageHeaderAndTitles
    Inherits System.Web.UI.Page
    Dim productsAdapter As New tbl_page_langTableAdapter
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public Sub RowDelete(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(source, Button)
        productsAdapter.DeleteById(CType(btn.CommandName, Integer))
        Gridview1.DataBind()
    End Sub
    Public Sub Editdata(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(source, Button)
        Dim id As String = Convert.ToString(btn.CommandName)

        Response.Redirect("../PagesMaint/PageAddHeadersAndTitles.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Mode=Edit" + "&Id=" + id)

    End Sub
    Public Sub Editdata2(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(source, Button)
        Dim id As String = Convert.ToString(btn.CommandName)

        Session("PreviousPage") = Request.RawUrl
        Response.Redirect("../PagesMaint/PageTextLang.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))

    End Sub
    Public Sub Editdata3(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As Button = TryCast(source, Button)
        Dim id As String = Convert.ToString(btn.CommandName)

        Session("PreviousPage") = Request.RawUrl
        Response.Redirect("../PagesMaint/PageAttributes.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))

    End Sub
    Protected Sub Gridview1_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles Gridview1.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onmouseover", "this.className='hightlighrow'")
            e.Row.Attributes.Add("onmouseout", "this.className='normalrow'")
        End If
    End Sub
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
        Add_Page_Header_and_Titlebtn.Text = wfrPage.Content("add_Page_Header_and_Titlebtn", _languageCode, True)
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PageHeadersAndTitles.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "PageHeadersAndTitles.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        IsAuthorizedRole()
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
                Descriptionlbl1.Text = Convert.ToString(productsAdapter.GetDescription(Request.QueryString("BusinessUnit").ToString, Request.QueryString("Partner").ToString, Request.QueryString("PageName").ToString))
                setLabel()
                TitleLabel.Text &= " " + Descriptionlbl1.Text
            End If
        End If
    End Sub
    Protected Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
        Dim dg As New DataGrid
        dg.DataSource = SearchDataSource1
        dg.DataBind()
        If dg.Items.Count > 0 Then
            Add_Page_Header_and_Titlebtn.Enabled = True
        Else
            Add_Page_Header_and_Titlebtn.Enabled = False
        End If
    End Sub
    Protected Sub Return_To_Page_Detailbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Return_To_Page_Detailbtn.Click
        Response.Redirect("../PagesMaint/Page-Details.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub
    Protected Sub Return_To_Pagesbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Return_To_Pagesbtn.Click
        Response.Redirect("../PagesMaint/PageOverview.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub
    Protected Sub Add_Page_Header_and_Titlebtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Add_Page_Header_and_Titlebtn.Click
        Response.Redirect("../PagesMaint/PageAddHeadersAndTitles.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "&Mode=Add")
    End Sub
    Protected Sub Define_HTML_Includesbtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Define_HTML_Includesbtn.Click
        Response.Redirect("../PagesMaint/PageHtmlIncludes.aspx?PageName=" + Request.QueryString("PageName") + "&PageId=" + Request.QueryString("PageId") + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Private Sub IsAuthorizedRole()
        If Not Utilities.isEnabled("AllowPageHeadersAndTitles") Then
            Response.Redirect("../Error.aspx?Type=UNAUTH")
        End If
    End Sub
End Class
