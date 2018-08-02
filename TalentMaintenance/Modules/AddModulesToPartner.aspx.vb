Option Strict On

Partial Class Modules_AddModulesToPartner
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Add/Remove Modules from Partner"
        Dim partnerName As String = String.Empty
        If Request("PartnerID") Is Nothing Then
            Response.Redirect("~/default.aspx")
        Else
 
            '------------------
            ' Find partner name
            '------------------
            Dim partnerTA As New PartnersTableAdapters.tbl_partnerTableAdapter
            Dim partnerDT As Data.DataTable = partnerTA.GetPartnerById(CInt(Request("PartnerID")))
            If partnerDT.Rows.Count > 0 Then
                partnerName = partnerDT.Rows(0)("PARTNER").ToString
            End If

            lblHeader.Text = "Add Modules to Partner: " & partnerName
            lblHeader2.Text = "Remove Modules from Partner: " & partnerName

            '----------------------------
            ' Build select for Add Module
            '----------------------------
            Dim sb As New StringBuilder
            With sb
                .Append("select distinct module from tbl_supplynet_module_defaults where module not ")
                .Append(" in (select module from tbl_supplynet_module_partner_defaults where ")
                .Append(" 	partner = '").Append(partnerName).Append("')")
                .Append(" and business_unit in (select distinct business_unit from tbl_authorized_partners ")
                .Append("  where partner = '").Append(partnerName).Append("')")
            End With
            SqlModulesNotForPartner.SelectCommand = sb.ToString
            '-------------------------------
            ' Build select for remove module
            '-------------------------------
            Dim sb2 As New StringBuilder
            With sb2
                .Append("select * from tbl_supplynet_module_partner_defaults as a ")
                .Append(" inner join tbl_partner as b on a.partner = b.partner where a.partner = '")
                .Append(partnerName).Append("'")
            End With
            SqlRemoveModules.SelectCommand = sb2.ToString

            btnRemove.Attributes.Add("onclick", "javascript:return " & _
                             "confirm('Are you sure you want to delete the selected records?') ")
        End If


    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        '------------------
        ' Find partner name
        '------------------
        Dim partnerName As String = String.Empty
        Dim partnerTA As New PartnersTableAdapters.tbl_partnerTableAdapter
        Dim partnerDT As Data.DataTable = partnerTA.GetPartnerById(CInt(Request("PartnerID")))
        If partnerDT.Rows.Count > 0 Then
            partnerName = partnerDT.Rows(0)("PARTNER").ToString
        End If
        ' Select the checbkoxes from the GridView control
        Dim i As Integer
        For i = 0 To grvModules.Rows.Count - 1 Step i + 1
            Dim row As GridViewRow = grvModules.Rows(i)
            Dim isChecked As Boolean = (CType(row.FindControl("chkSelect"), CheckBox)).Checked

            If (isChecked) Then
                Dim partnerModuleTA As New ModulesTableAdapters.tbl_supplynet_module_partner_defaultsTableAdapter
                partnerModuleTA.InsertQuery(partnerName, row.Cells(0).Text, String.Empty, String.Empty, String.Empty, _
                String.Empty, String.Empty, 0, True, True, String.Empty, True, String.Empty, False, False, _
                String.Empty)
            End If
        Next
        Response.Redirect(Request.Url.ToString)
    End Sub


    Protected Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRemove.Click
        '------------------
        ' Find partner name
        '------------------
        Dim partnerName As String = String.Empty
        Dim partnerTA As New PartnersTableAdapters.tbl_partnerTableAdapter
        Dim partnerDT As Data.DataTable = partnerTA.GetPartnerById(CInt(Request("PartnerID")))
        If partnerDT.Rows.Count > 0 Then
            partnerName = partnerDT.Rows(0)("PARTNER").ToString
        End If
        ' Select the checkboxes from the GridView control
        Dim i As Integer
        For i = 0 To grvRemoveModules.Rows.Count - 1 Step i + 1
            Dim row As GridViewRow = grvRemoveModules.Rows(i)
            Dim isChecked As Boolean = (CType(row.FindControl("chkSelect"), CheckBox)).Checked

            If (isChecked) Then
                Dim partnerModuleTA As New ModulesTableAdapters.tbl_supplynet_module_partner_defaultsTableAdapter
                partnerModuleTA.DeleteByModulePartner(row.Cells(1).Text, partnerName)
            End If
        Next
        Response.Redirect(Request.Url.ToString)
    End Sub


End Class
