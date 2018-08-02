Option Strict On
Imports Microsoft.VisualBasic
Imports System.Data
Imports Talent.Common

Partial Class MasterPage1
    Inherits System.Web.UI.MasterPage

    Public Property HeaderText() As String
        Get
            Return String.Empty
        End Get
        Set(ByVal value As String)
            Page.Header.Title = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As Talent.Common.DESettings = New DESettings()
        Dim dtMaintenanceModules As New DataTable
        Dim dtBusinessUnits As New DataTable
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        tDataObjects.Settings = settings

        If Not Page.IsPostBack Then
            dtBusinessUnits = tDataObjects.AppVariableSettings.TblBu.GetAll()
            If dtBusinessUnits IsNot Nothing AndAlso dtBusinessUnits.Rows.Count > 0 Then
                ddlBusinessUnits.DataSource = dtBusinessUnits
                ddlBusinessUnits.DataTextField = "BUSINESS_UNIT_DESC"
                ddlBusinessUnits.DataValueField = "BUSINESS_UNIT"
                ddlBusinessUnits.DataBind()
                If Request.QueryString("bu") IsNot Nothing Then
                    Dim BUQueryStringValue As String = Request.QueryString("bu").ToString()
                    For Each item As ListItem In ddlBusinessUnits.Items
                        If item.Value = BUQueryStringValue Then item.Selected = True
                    Next
                End If
                If dtBusinessUnits.Rows.Count = 1 Then plhBusinessUnits.Visible = False
            End If
        End If

        dtMaintenanceModules = tDataObjects.MaintenanceSettings.TblMaintenanceModules.GetMaintenanceMenuOptions()
        If dtMaintenanceModules IsNot Nothing AndAlso dtMaintenanceModules.Rows.Count > 0 Then
            rptNavigationMenu.DataSource = dtMaintenanceModules
            rptNavigationMenu.DataBind()
        End If
        tDataObjects = Nothing

    End Sub

    Protected Sub rptNavigationMenu_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptNavigationMenu.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim hplNavigateOption As HyperLink = CType(e.Item.FindControl("hplNavigateOption"), HyperLink)
            Dim itemData As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim navigateUrl As String = Utilities.CheckForDBNull_String(itemData("NAVIGATE_URL"))
            hplNavigateOption.NavigateUrl = String.Format(navigateUrl, ddlBusinessUnits.SelectedValue)
            hplNavigateOption.Text = Utilities.CheckForDBNull_String(itemData("MODULE"))
        End If
    End Sub
End Class