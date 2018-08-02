Option Strict On
Partial Class Modules_ModuleAdmin
    Inherits System.Web.UI.Page

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim buTA As New BUTableAdapters.tbl_buTableAdapter
        Dim dt As Data.DataTable = buTA.GetByBU(Request("BU"))
        Dim buName As String = String.Empty
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = "ModuleAdmin.aspx"
            .KeyCode = "ModuleAdmin.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        ltlHomeLink.Text = _wfrPage.Content("HomeLink", _languageCode, True)
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)

        If dt.Rows.Count > 0 Then
            buName = dt.Rows(0)("BUSINESS_UNIT").ToString
            master.HeaderText = _wfrPage.Content("HeaderText", _languageCode, True) & " for " & buName
        End If
    End Sub
    Protected Sub ModulesView_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ModulesView.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            '-----------------------------------------------
            ' Add a confirmation window when clicking delete
            '-----------------------------------------------
            Dim deleteButton As LinkButton = CType(e.Row.Controls(0).Controls(0), LinkButton)
            deleteButton.Attributes.Add("onclick", "javascript:return " & _
                "confirm('Are you sure you want to delete this record') ")
        End If
    End Sub

    Protected Sub ModulesView_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles ModulesView.PageIndexChanging
        ModulesView.PageIndex = e.NewPageIndex
        ModulesView.DataBind()
    End Sub

    Protected Sub ModulesView_RowDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeletedEventArgs) Handles ModulesView.RowDeleted
        ModulesView.DataBind()
    End Sub

    Protected Sub ModulesView_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles ModulesView.RowDeleting
        Dim modulesIdToDelete As Integer = CInt(ModulesView.DataKeys(e.RowIndex).Value)
        Dim modulesTA As New ModulesTableAdapters.tbl_supplynet_module_defaultsTableAdapter
        modulesTA.DeleteById(modulesIdToDelete)

        Response.Redirect(Request.Url.ToString)
    End Sub

    Protected Sub ModulesView_RowUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdatedEventArgs) Handles ModulesView.RowUpdated
        ModulesView.DataBind()
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Dim redirectURL As String = "~/Modules/ModuleDetails.aspx?ID=new"
        If Not Request("BU") Is Nothing Then
            redirectURL &= "&BU=" & Request("BU")
        End If
        Response.Redirect(redirectURL)
    End Sub

    Protected Sub ModulesView_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles ModulesView.Load
        Dim modules As New ModulesTableAdapters.tbl_supplynet_module_defaultsTableAdapter

        ModulesView.DataSource = modules.GetAllByBU(Request("BU"))
        ModulesView.DataBind()

        If Not Session("ModulesCurrentSortExpression") Is Nothing Then
            '--------------------
            ' Load sorted version
            ' Needs to always 
            ' sort here in case
            ' delete was clicked
            '--------------------
            Dim dataTable As Data.DataTable = CType(ModulesView.DataSource, Data.DataTable)
            If Not dataTable Is Nothing Then
                Dim dataView As Data.DataView = New Data.DataView(dataTable)
                dataView.Sort = Session("ModulesCurrentSortExpression").ToString + " " + ConvertSortDirectionToSql(CType(Session("ModulesCurrentSortDirection"), SortDirection))
                ModulesView.DataSource = dataView
                ModulesView.DataBind()
            End If
        End If
    End Sub


    Private Function ConvertSortDirectionToSql(ByVal sortDirection As SortDirection) As String
        Dim NewSortDirection As String = String.Empty

        Select Case sortDirection
            Case sortDirection.Ascending
                NewSortDirection = "ASC"

            Case sortDirection.Descending
                NewSortDirection = "DESC"
        End Select

        Return NewSortDirection
    End Function

    Protected Sub modulesView_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles ModulesView.Sorting
        Dim datatable As Data.DataTable = Nothing
        '------------------------------------------------------------
        ' If first time sorting then data source will be a data table
        ' Otherwise data source will be a data view
        '------------------------------------------------------------
        If Session("ModulesCurrentSortExpression") Is Nothing Then
            datatable = CType(ModulesView.DataSource, Data.DataTable)
        End If
        '------------------------------------------------
        ' If sorting by same column as before then switch
        ' between ascending and descending
        '------------------------------------------------
        If Not Session("ModulesCurrentSortExpression") Is Nothing AndAlso Session("ModulesCurrentSortExpression").ToString = e.SortExpression Then
            Dim currentSortDirection As SortDirection = CType(Session("ModulesCurrentSortDirection"), SortDirection)
            Select Case currentSortDirection
                Case Is = SortDirection.Ascending

                    e.SortDirection = SortDirection.Descending
                    currentSortDirection = SortDirection.Descending
                Case Is = SortDirection.Descending
                    e.SortDirection = SortDirection.Ascending
                    currentSortDirection = SortDirection.Ascending
            End Select
        End If
        If Not datatable Is Nothing Then
            Dim dataView As Data.DataView = New Data.DataView(datatable)
            dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection)

            ModulesView.DataSource = dataView
            ModulesView.DataBind()
            Session("ModulesCurrentSortDirection") = e.SortDirection
            Session("ModulesCurrentSortExpression") = e.SortExpression
        Else
            If Not Session("ModulesCurrentSortExpression") Is Nothing Then
                '--------------------
                ' Load from data view
                '--------------------
                Dim dataView As Data.DataView = CType(ModulesView.DataSource, Data.DataView)
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection)
                ModulesView.DataSource = dataView
                ModulesView.DataBind()
                Session("ModulesCurrentSortDirection") = e.SortDirection
                Session("ModulesCurrentSortExpression") = e.SortExpression
            End If
        End If

    End Sub


End Class
