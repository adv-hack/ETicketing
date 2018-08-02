Imports MaintenancePortalDataSetTableAdapters
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports System.Data

Partial Class PageOverview
    Inherits System.Web.UI.Page

#Region "Class Level Fields"

    Private _objBUTableAdapter As New BUSINESS_UNIT_TableAdapter
    Private _objPTableAdapter As New PARTNER_TableAdapter
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty
    Private objPage1 As New PageDataSetTableAdapters.PageDateTableAdapter

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PageCode = String.Empty
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PageOverview.aspx"
            .PageCode = "PageOverview.aspx"
        End With
        _languageCode = TCUtilities.GetDefaultLanguage()

        Select Case Request.QueryString("ShowBy")
            Case "All"
                ObjectDataSource1.SelectMethod = "GetDataByBUPT"
                AddNewPageButton.Visible = True
                titleLabel.Text = "Pages"
                rdoAllPages.Checked = True
            Case "NotDefined"
                ObjectDataSource1.SelectMethod = "GetDataByPageNotDefine"
                AddNewPageButton.Visible = False
                titleLabel.Text = "Standard Pages"
                rdoStandardPages.Checked = True
            Case "Defined"
                ObjectDataSource1.SelectMethod = "GetDataByPageDefined"
                AddNewPageButton.Visible = True
                titleLabel.Text = "User Defined Pages"
                rdoUserDefinedPages.Checked = True
            Case Else
                ObjectDataSource1.SelectMethod = "GetDataByBUPT"
                AddNewPageButton.Visible = True
                titleLabel.Text = "Pages"
                rdoAllPages.Checked = True
        End Select

        DisableControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If String.IsNullOrEmpty(Request.QueryString("BusinessUnit")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If
        If String.IsNullOrEmpty(Request.QueryString("Partner")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If
        If IsPostBack = False Then
            setLabel()
            fillBusinessUnitDropDown()
            businessunitDropDownList.SelectedValue = Request.QueryString("BusinessUnit")
            fillPageDDL()
        End If
    End Sub

    Protected Sub PageGridView_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles PageGridView.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim i As String = e.Row.Cells(0).Text
            Dim la As New LinkButton
            la = CType(e.Row.FindControl("DeleteLinkButton"), LinkButton)
            If e.Row.Cells(1).Text.ToLower.StartsWith("userdefined") Then
                e.Row.Cells(1).Text = "<a class='page' href='Page-Details.aspx?PageId=" + e.Row.Cells(0).Text + "&PageName=" + e.Row.Cells(1).Text + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "'>" + e.Row.Cells(1).Text + "</a>"
                la.Text = "delete"
            Else
                e.Row.Cells(1).Text = "<a class='page' href='Page-Details.aspx?PageId=" + e.Row.Cells(0).Text + "&PageName=" + e.Row.Cells(1).Text + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit") + "'>" + e.Row.Cells(1).Text + "</a>"
                la.Text = ""
            End If
        End If
    End Sub

    Protected Sub rdoStandardPages_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdoStandardPages.CheckedChanged
        Response.Redirect("../PagesMaint/PageOverview.aspx?ShowBy=NotDefined&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub rdoUserDefinedPages_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdoUserDefinedPages.CheckedChanged
        Response.Redirect("../PagesMaint/PageOverview.aspx?ShowBy=Defined&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub rdoAllPages_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdoAllPages.CheckedChanged
        Response.Redirect("../PagesMaint/PageOverview.aspx?ShowBy=All&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub AddNewPageButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddNewPageButton.Click
        Response.Redirect("../PagesMaint/Page-Details.aspx?Status=New&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub EditControls_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EditControlsButton.Click
        Response.Redirect("../PagesMaint/EditControls.aspx?Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
    End Sub

    Protected Sub PageDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PageDDL.SelectedIndexChanged
        Dim dt As New DataTable
        dt = objPage1.GetDataByPageDetails(Request.QueryString("BusinessUnit"), Request.QueryString("Partner"), PageDDL.SelectedValue)
        If dt.Rows.Count = 1 Then
            Dim pageID As String
            pageID = dt.Rows(0).Item("ID")
            Response.Redirect("Page-Details.aspx?PageId=" + pageID + "&PageName=" + PageDDL.SelectedValue + "&Partner=" + Request.QueryString("Partner") + "&BusinessUnit=" + Request.QueryString("BusinessUnit"))
        End If
    End Sub

    Protected Sub businessunitDropDownList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles businessunitDropDownList.SelectedIndexChanged
        Response.Redirect("PageOverview.aspx?usage=promotion&Partner=*ALL&BusinessUnit=" + businessunitDropDownList.SelectedValue + "&ShowBy=" + Request.QueryString("ShowBy"))
    End Sub

    Protected Sub PageGridView_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles PageGridView.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onmouseover", "this.className='hightlighrow'")
            e.Row.Attributes.Add("onmouseout", "this.className='normalrow'")
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub setLabel()
        Page.Title = _wfrPage.Content("PageTitle", _languageCode, True)
        instructionsLabel.Text = _wfrPage.Content("instructionsLabel", _languageCode, True)
        businessunitLabel.Text = _wfrPage.Content("businessunitLabel", _languageCode, True)
        pageLabel.Text = _wfrPage.Content("selectPageLabel", _languageCode, True)
        lblPageTypeLabel.Text = _wfrPage.Content("PageTypeLabel", _languageCode, True)
        rdoAllPages.Text = _wfrPage.Content("AllPagesRadioButton", _languageCode, True)
        rdoStandardPages.Text = _wfrPage.Content("StandardPagesRadioButton", _languageCode, True)
        rdoUserDefinedPages.Text = _wfrPage.Content("UserDefinedPagesRadioButton", _languageCode, True)
        AddNewPageButton.Text = _wfrPage.Content("AddNewPageButton", _languageCode, True)
        EditControlsButton.Text = _wfrPage.Content("EditControlsButton", _languageCode, True)
        plhErrorMessage.Visible = False
        ltlHomeLink.Text = _wfrPage.Content("HomeLink", _languageCode, True)
    End Sub

    Private Sub fillBusinessUnitDropDown()
        Dim dtBusinessUnit As DataTable = _objBUTableAdapter.GetDatadistinctBusinessUnit()
        Dim dtBusinessUnitFiltered As DataTable = Nothing
        If dtBusinessUnit.Rows.Count > 0 AndAlso (Not String.IsNullOrWhiteSpace(CStr(ConfigurationManager.AppSettings("ShowBusinessUnitInMatrix")))) Then
            Dim sFilter As String = String.Empty
            Dim BU() As String
            BU = CStr(ConfigurationManager.AppSettings("ShowBusinessUnitInMatrix")).ToString.Split(",")
            For Each sBU As String In BU
                If sFilter = String.Empty Then
                    sFilter = "BUSINESS_UNIT = '" + sBU + "'"
                Else
                    sFilter = sFilter + " OR BUSINESS_UNIT = '" + sBU + "'"
                End If
            Next
            dtBusinessUnitFiltered = dtBusinessUnit.Select(sFilter).CopyToDataTable
            '            dtBusinessUnitFiltered = dtBusinessUnit.Select("BUSINESS_UNIT='" & CStr(ConfigurationManager.AppSettings("ShowBusinessUnitInMatrix")) & "'").CopyToDataTable
        Else
            dtBusinessUnitFiltered = dtBusinessUnit
        End If
        businessunitDropDownList.DataSource = dtBusinessUnitFiltered
        businessunitDropDownList.DataTextField = "BUSINESS_UNIT"
        businessunitDropDownList.DataValueField = "BUSINESS_UNIT"
        businessunitDropDownList.DataBind()
    End Sub

    Private Sub fillPageDDL()
        Select Case Request.QueryString("ShowBy")
            Case "All"
                PageDDL.DataSource = objPage1.GetDataByBUPT(businessunitDropDownList.SelectedValue, GlobalConstants.STARALLPARTNER, False)
            Case "NotDefined"
                PageDDL.DataSource = objPage1.GetDataByPageNotDefine(businessunitDropDownList.SelectedValue, GlobalConstants.STARALLPARTNER, False)
            Case "Defined"
                PageDDL.DataSource = objPage1.GetDataByPageDefined(businessunitDropDownList.SelectedValue, GlobalConstants.STARALLPARTNER, False)
            Case Else
                'Table adaptor GetDataByBUPT function replicated as a data object
                PageDDL.DataSource = objPage1.GetDataByBUPT(businessunitDropDownList.SelectedValue, GlobalConstants.STARALLPARTNER, False)
        End Select
        PageDDL.DataTextField = "DESCRIPTION"
        PageDDL.DataValueField = "PAGE_CODE"
        PageDDL.DataBind()
        PageDDL.Items.Insert(0, " -- ")
    End Sub

    Private Sub DisableControls()
        If Not Utilities.isEnabled("AllowAddNewPage") Then
            AddNewPageButton.Enabled = False
            AddNewPageButton.Visible = False
        End If
        If Not Utilities.isEnabled("AllowEditControls") Then
            EditControlsButton.Enabled = False
            EditControlsButton.Visible = False
        End If
    End Sub

#End Region

#Region "Public Methods"

    Public Sub RowDelete(ByVal source As Object, ByVal e As EventArgs)
        Dim btn As LinkButton = TryCast(source, LinkButton)
        Dim retVal As Integer = objPage1.CheckParent(btn.CommandArgument)
        ErrorLabel.Text = ""
        If retVal = 0 Then
            objPage1.PageDelete(CType(btn.CommandName, Int32))
            PageGridView.DataBind()
        Else
            plhErrorMessage.Visible = True
            ErrorLabel.Text = _wfrPage.Content("ErrorLabel", _languageCode, True)
        End If
    End Sub

#End Region

End Class