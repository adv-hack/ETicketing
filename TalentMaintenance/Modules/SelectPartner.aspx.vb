Option Strict On
Partial Class Modules_SelectPartner
    Inherits System.Web.UI.Page

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = "SelectPartner.aspx"
            .KeyCode = "SelectPartner.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        ltlHomeLink.Text = _wfrPage.Content("HomeLink", _languageCode, True)
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = _wfrPage.Content("HeaderText", _languageCode, True)
    End Sub

    Protected Sub PartnersView_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles PartnersView.Load
        Dim partners As New PartnersTableAdapters.tbl_partnerTableAdapter

        PartnersView.DataSource = partners.GetAllPartners
        PartnersView.DataBind()

        If Not Session("PartnerCurrentSortExpression") Is Nothing Then
            '--------------------
            ' Load sorted version
            ' Needs to always 
            ' sort here in case
            ' delete was clicked
            '--------------------
            Dim dataTable As Data.DataTable = CType(PartnersView.DataSource, Data.DataTable)
            If Not dataTable Is Nothing Then
                Dim dataView As Data.DataView = New Data.DataView(dataTable)
                dataView.Sort = Session("PartnerCurrentSortExpression").ToString + " " + ConvertSortDirectionToSql(CType(Session("PartnerCurrentSortDirection"), SortDirection))
                PartnersView.DataSource = dataView
                PartnersView.DataBind()
            End If
        End If
    End Sub

  
    Protected Sub UsersView_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles PartnersView.PageIndexChanging
        PartnersView.PageIndex = e.NewPageIndex
        PartnersView.DataBind()
    End Sub

    Protected Sub PartnersView_RowDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeletedEventArgs) Handles PartnersView.RowDeleted
        PartnersView.DataBind()
    End Sub

    Protected Sub PartnersView_RowDeleting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewDeleteEventArgs) Handles PartnersView.RowDeleting
        Dim partnerIdToDelete As Integer = CInt(PartnersView.DataKeys(e.RowIndex).Value)
        Dim partnersTA As New PartnersTableAdapters.tbl_partnerTableAdapter
        partnersTA.DeleteByPartnerId(partnerIdToDelete)

        Response.Redirect(Request.Url.ToString)
    End Sub

    Protected Sub PartnersView_RowUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdatedEventArgs) Handles PartnersView.RowUpdated
        PartnersView.DataBind()
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

    Protected Sub PartnersView_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles PartnersView.Sorting
        Dim datatable As Data.DataTable = Nothing
        '------------------------------------------------------------
        ' If first time sorting then data source will be a data table
        ' Otherwise data source will be a data view
        '------------------------------------------------------------
        If Session("PartnerCurrentSortExpression") Is Nothing Then
            datatable = CType(PartnersView.DataSource, Data.DataTable)
        End If
        '------------------------------------------------
        ' If sorting by same column as before then switch
        ' between ascending and descending
        '------------------------------------------------
        If Not Session("PartnerCurrentSortExpression") Is Nothing AndAlso Session("PartnerCurrentSortExpression").ToString = e.SortExpression Then
            Dim currentSortDirection As SortDirection = CType(Session("PartnerCurrentSortDirection"), SortDirection)
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

            PartnersView.DataSource = dataView
            PartnersView.DataBind()
            Session("PartnerCurrentSortDirection") = e.SortDirection
            Session("PartnerCurrentSortExpression") = e.SortExpression
        Else
            If Not Session("PartnerCurrentSortExpression") Is Nothing Then
                '--------------------
                ' Load from data view
                '--------------------
                Dim dataView As Data.DataView = CType(PartnersView.DataSource, Data.DataView)
                dataView.Sort = e.SortExpression + " " + ConvertSortDirectionToSql(e.SortDirection)
                PartnersView.DataSource = dataView
                PartnersView.DataBind()
                Session("PartnerCurrentSortDirection") = e.SortDirection
                Session("PartnerCurrentSortExpression") = e.SortExpression
            End If
        End If

    End Sub

End Class
