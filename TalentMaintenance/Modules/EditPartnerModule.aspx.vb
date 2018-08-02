Option Strict On

Partial Class Partners_EditPartnerModule
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Edit Partner Module"

        If Not Page.IsPostBack Then
            '-------------------
            ' Set to change mode
            '-------------------
            Dim partnerModuleTA As New ModulesTableAdapters.tbl_supplynet_module_partner_defaultsTableAdapter
            DetailsView1.ChangeMode(DetailsViewMode.Edit)
            DetailsView1.AutoGenerateEditButton = True

            DetailsView1.DataSource = partnerModuleTA.GetByID(CInt(Request("ID")))
            DetailsView1.DataBind()
        End If
    End Sub
    Protected Sub DetailsView1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles DetailsView1.DataBound
        Dim firstRow As DetailsViewRow = DetailsView1.Rows(0)
        Dim secondRow As DetailsViewRow = DetailsView1.Rows(1)
        Dim thirdRow As DetailsViewRow = DetailsView1.Rows(2)

        Dim idBox As TextBox = CType(firstRow.Controls(1).Controls(0), TextBox)
        Dim idBox2 As TextBox = CType(secondRow.Controls(1).Controls(0), TextBox)
        Dim idBox3 As TextBox = CType(thirdRow.Controls(1).Controls(0), TextBox)

        idBox.Enabled = False
        idBox2.Enabled = False
        idBox3.Enabled = False

    End Sub

    Protected Sub DetailsView1_ItemUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdateEventArgs) Handles DetailsView1.ItemUpdating
        Dim defaultsTA As New ModulesTableAdapters.tbl_supplynet_module_partner_defaultsTableAdapter
        Dim moduleDefault As Modules.tbl_supplynet_module_partner_defaultsRow = defaultsTA.GetByID(CInt(Request("ID")))(0)

        Dim dv As DetailsView = CType(sender, DetailsView)
        Dim row As DetailsViewRow
        Dim rowName As String

        Try
            For i As Integer = 0 To dv.Rows.Count - 2
                row = dv.Rows.Item(i)
                rowName = CType(row.Controls(0), System.Web.UI.WebControls.DataControlFieldCell).Text
                Dim rowValue As Object = row.Controls(1).Controls(0)
                Select Case rowValue.GetType.ToString
                    Case Is = "System.Web.UI.WebControls.TextBox"
                        Try
                            CallByName(moduleDefault, rowName, CallType.Set, CType(rowValue, TextBox).Text)
                        Catch ex As Exception
                        End Try
                    Case Is = "System.Web.UI.WebControls.CheckBox"
                        Try
                            CallByName(moduleDefault, rowName, CallType.Set, CType(rowValue, CheckBox).Checked)
                        Catch ex As Exception
                        End Try
                    Case Is = "System.Web.UI.WebControls.DropDownList"
                        Try
                            CallByName(moduleDefault, rowName, CallType.Set, CType(rowValue, DropDownList).SelectedValue)
                        Catch ex As Exception
                        End Try
                End Select

            Next
            defaultsTA.UpdateByID(moduleDefault.PARTNER, moduleDefault._MODULE, moduleDefault.INCOMING_STYLE_SHEET, _
            moduleDefault.OUTGOING_STYLE_SHEET, moduleDefault.RESPONSE_VERSION, moduleDefault.DESTINATION_DATABASE, _
            moduleDefault.XML_SCHEMA_DOCUMENT, moduleDefault.CACHE_TIME_MINUTES, moduleDefault.LOGGING_ENABLED, _
            moduleDefault.STORE_XML, moduleDefault.OUTGOING_XML_RESPONSE, moduleDefault.AUTO_PROCESS, moduleDefault.EMAIL, _
            moduleDefault.EMAIL_XML_RESPONSE, moduleDefault.AUTO_PROCESS_SPLIT, moduleDefault.STORED_PROCEDURE_GROUP, _
            CInt(moduleDefault.SUPPLYNET_MP_ID))
        Catch ex As Exception
        End Try

    End Sub

    Protected Sub DetailsView1_ModeChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewModeEventArgs) Handles DetailsView1.ModeChanging
        If e.CancelingEdit Then
            Response.Redirect("~/Modules/AddModulesToPartner.aspx?PartnerID=" & Request("PartnerID"))
        End If
    End Sub
End Class
