Option Strict On

Partial Class Partners_AddPartnersToBu
    Inherits System.Web.UI.Page

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = "AddPartnersToBU.aspx"
            .KeyCode = "AddPartnersToBU.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        ltlHomeLink.Text = _wfrPage.Content("HomeLink", _languageCode, True)
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = _wfrPage.Content("HeaderText", _languageCode, True)

        If Request("BU") Is Nothing Then
            Response.Redirect("~/default.aspx")
        Else
            Dim buTA As New BUTableAdapters.tbl_buTableAdapter
            Dim dt As Data.DataTable = buTA.GetByBU(Request("BU"))
            lblHeader.Text = "Add Partners to Business Unit : "
            lblHeader2.Text = "Remove Partners from Business Unit : "
            If dt.Rows.Count > 0 Then
                lblHeader.Text &= dt.Rows(0)("BUSINESS_UNIT_DESC").ToString
                lblHeader2.Text &= dt.Rows(0)("BUSINESS_UNIT_DESC").ToString
                SqlPartnersNotInBU.SelectCommand = "Select * from tbl_partner where partner not in " & _
                                               " (select partner from tbl_authorized_partners " & _
                                               " where business_unit = '" & _
                                               dt.Rows(0)("BUSINESS_UNIT").ToString & _
                                               "')"
                SqlRemovePartners.SelectCommand = "select * from tbl_authorized_partners as a " & _
                                               " inner join tbl_partner as b on a.partner = b.partner " & _
                                               " where business_unit = '" & _
                                               dt.Rows(0)("BUSINESS_UNIT").ToString & _
                                               "'"
                btnRemove.Attributes.Add("onclick", "javascript:return " & _
                                 "confirm('Are you sure you want to delete the selected records?') ")
            End If

        End If

    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        ' Select the checkboxes from the GridView control
        Dim i As Integer
        For i = 0 To grvPartners.Rows.Count - 1 Step i + 1
            Dim row As GridViewRow = grvPartners.Rows(i)
            Dim isChecked As Boolean = (CType(row.FindControl("chkSelect"), CheckBox)).Checked

            If (isChecked) Then
                Dim authPartnersTA As New PartnersTableAdapters.tbl_authorized_partnersTableAdapter
                authPartnersTA.InsertQuery(Request("BU").ToString, row.Cells(0).Text, False)
            End If
        Next
        Response.Redirect(Request.Url.ToString)
    End Sub


    Protected Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        ' Select the checkboxes from the GridView control
        Dim i As Integer
        For i = 0 To grvRemovePartners.Rows.Count - 1 Step i + 1
            Dim row As GridViewRow = grvRemovePartners.Rows(i)
            Dim isChecked As Boolean = (CType(row.FindControl("chkSelect"), CheckBox)).Checked

            If (isChecked) Then
                Dim authPartnersTA As New PartnersTableAdapters.tbl_authorized_partnersTableAdapter
                authPartnersTA.DeleteQuery(Request("BU").ToString, row.Cells(1).Text)
            End If
        Next
        Response.Redirect(Request.Url.ToString)
    End Sub
End Class
