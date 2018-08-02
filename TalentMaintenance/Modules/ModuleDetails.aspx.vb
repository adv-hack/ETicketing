Option Strict On
Partial Class Modules_ModuleDetails
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)

        If Request("ID") Is Nothing OrElse Request("ID").ToString = "new" Then
            master.HeaderText = "Add Module"
        Else
            master.HeaderText = "Change Module"
        End If


        If Not Page.IsPostBack Then

            If Request("ID") Is Nothing OrElse Request("ID").ToString = "new" Then
                '----------------
                ' Set to add mode
                '----------------
                master.HeaderText = "Add Module"
                Dim modulesTA As New ModulesTableAdapters.tbl_supplynet_module_defaultsTableAdapter
                DetailsView1.ChangeMode(DetailsViewMode.Insert)
                DetailsView1.AutoGenerateInsertButton = True

                DetailsView1.DataSource = modulesTA.GetFirstRecord(Request("BU"))
                DetailsView1.DataBind()
                If DetailsView1.Rows.Count > 0 Then
                    '  DetailsView1.DataBind()
                End If
                AddValidators()
                '-----------------------
                ' Check if default in BU
                '-----------------------
                If Not Request("BU") Is Nothing Then
                    Dim secondRow As DetailsViewRow = DetailsView1.Rows(1)
                    Dim buBox As TextBox = CType(secondRow.Controls(1).Controls(0), TextBox)
                    buBox.Text = Request("BU")
                    buBox.Enabled = False
                End If

            Else
                '-------------------
                ' Set to change mode
                '-------------------
                master.HeaderText = "Change Module"
                Dim modulesTA As New ModulesTableAdapters.tbl_supplynet_module_defaultsTableAdapter
                DetailsView1.ChangeMode(DetailsViewMode.Edit)
                DetailsView1.AutoGenerateEditButton = True

                DetailsView1.DataSource = modulesTA.GetDataByID(CInt(Request("ID")))
                DetailsView1.DataBind()
            End If
        End If

    End Sub
    '-------------------------------
    ' Add validator controls to rows
    '-------------------------------
    Protected Sub AddValidators()

        Dim row As DetailsViewRow
        Dim rowName As String
        Dim dv As DetailsView = DetailsView1
        Try
            For i As Integer = 0 To dv.Rows.Count - 2
                row = dv.Rows.Item(i)
                rowName = CType(row.Controls(0), System.Web.UI.WebControls.DataControlFieldCell).Text
                Dim rowValue As Object = row.Controls(1).Controls(0)
                Select Case rowName
                    Case Is = "PARTNER"
                        'Dim tb As TextBox = CType(row.Controls(1).Controls(0), TextBox)
                        'Dim cell As DataControlFieldCell
                        'If row.Controls(1).GetType.ToString = "System.Web.UI.WebControls.DataControlFieldCell" Then
                        '    cell = CType(row.Controls(1), DataControlFieldCell)
                        '    Dim rfvReq As New RequiredFieldValidator
                        '    If tb.ID Is Nothing Then
                        '        tb.ID = "txtPartner"
                        '    End If
                        '    rfvReq.ControlToValidate = tb.ID.ToString
                        '    rfvReq.ErrorMessage = "Partner must be entered"
                        '    rfvReq.Text = "*"
                        '    rfvReq.Display = ValidatorDisplay.Dynamic
                        '    cell.Controls.Add(rfvReq)
                        'End If
                    Case Is = "LOGINID"
                        'Dim tb As TextBox = CType(row.Controls(1).Controls(0), TextBox)
                        'Dim cell As DataControlFieldCell
                        'If row.Controls(1).GetType.ToString = "System.Web.UI.WebControls.DataControlFieldCell" Then
                        '    cell = CType(row.Controls(1), DataControlFieldCell)
                        '    Dim rfvReq As New RequiredFieldValidator
                        '    If tb.ID Is Nothing Then
                        '        tb.ID = "txtLoginID"
                        '    End If
                        '    rfvReq.ControlToValidate = tb.ID.ToString
                        '    rfvReq.ErrorMessage = "Login ID must be entered"
                        '    rfvReq.Text = "*"
                        '    rfvReq.Display = ValidatorDisplay.Dynamic
                        '    cell.Controls.Add(rfvReq)
                        'End If

                End Select
            Next
        Catch ex As Exception

        End Try

    End Sub
    Protected Sub DetailsView1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles DetailsView1.DataBound
        Dim firstRow As DetailsViewRow = DetailsView1.Rows(0)
        Dim secondRow As DetailsViewRow = DetailsView1.Rows(1)
        Dim idBox As TextBox = CType(firstRow.Controls(1).Controls(0), TextBox)
        Dim idBox2 As TextBox = CType(secondRow.Controls(1).Controls(0), TextBox)
        idBox.Enabled = False
        idBox2.Enabled = False
    End Sub


    Protected Sub DetailsView1_ItemInserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles DetailsView1.ItemInserting
        Dim modulesTA As New ModulesTableAdapters.tbl_supplynet_module_defaultsTableAdapter
        Dim moduleDefault As Modules.tbl_supplynet_module_defaultsRow = CType(modulesTA.GetFirstRecord(Request("BU")).Rows(0), Modules.tbl_supplynet_module_defaultsRow)

        Dim row As DetailsViewRow
        Dim rowName As String
        Dim dv As DetailsView = CType(sender, DetailsView)
        If Page.IsValid Then
            Try
                For i As Integer = 0 To dv.Rows.Count - 2
                    row = dv.Rows.Item(i)
                    rowName = CType(row.Controls(0), System.Web.UI.WebControls.DataControlFieldCell).Text
                    Dim rowValue As Object = row.Controls(1).Controls(0)
                    Select Case rowValue.GetType.ToString
                        Case Is = "System.Web.UI.WebControls.TextBox"
                            Try
                                If rowName = "MODULE" Then
                                    rowName = "_MODULE"
                                End If
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
                '-------------------
                ' Validation section
                '-------------------
                Dim isValid As Boolean = ValidateFields(moduleDefault)

                If isValid Then
                    modulesTA.AddModuleDefault(moduleDefault.BUSINESS_UNIT, moduleDefault._MODULE, moduleDefault.REQUEST_CURRENT_VERSION, _
                    moduleDefault.XML_SCHEMA_DOCUMENT, moduleDefault.MODULE_TYPE, moduleDefault.LOGGING_ENABLED, moduleDefault.STORE_XML, _
                    moduleDefault.OUTGOING_XML_RESPONSE, moduleDefault.AUTO_PROCESS, moduleDefault.AUTO_PROCESS_WAIT_PERIOD_MINUTES, _
                    moduleDefault.RETRY_FAILURES, moduleDefault.RETRY_ATTEMPTS, moduleDefault.RETRY_WAIT_TIME, moduleDefault.RETRY_ERROR_NUMBERS)

                    Dim redirectURL As String = String.Empty
                    redirectURL = "~/Modules/ModuleAdmin.aspx"
                    If Not Request("BU") Is Nothing Then
                        redirectURL &= "?BU=" & Request("BU")
                    End If
                    Response.Redirect(redirectURL)


                End If
            Catch ex As Exception
            End Try

        End If

    End Sub
    Protected Function ValidateFields(ByVal moduleDefault As Modules.tbl_supplynet_module_defaultsRow) As Boolean
        Dim addMode As Boolean = False
        If Request("ID") Is Nothing Or Request("ID") = "new" Then
            addMode = True
        End If
        Dim isValid As Boolean = True
        Dim vld As New Talent.Maintenance.CustomValidationSummary
        'Dim usersTA As New UsersTableAdapters.tbl_partner_userTableAdapter
        Try
            '-----------------------------
            ' Check partner name is filled
            '-----------------------------
            If moduleDefault._MODULE = String.Empty Then
                vld.AddErrorMessage("Module name cannot be blank", Me.Page)
                isValid = False
            End If
            '    '----------------------
            '    ' Check login is filled
            '    '----------------------
            '    If user.LOGINID = String.Empty Then
            '        vld.AddErrorMessage("Login Id cannot be blank", Me.Page)
            '        isValid = False
            '    End If
            '    '---------------------
            '    ' Check partner exists
            '    '---------------------
            '    Dim partnersTA As New PartnersTableAdapters.tbl_partnerTableAdapter
            '    Dim dtPartners As Data.DataTable
            '    If user.PARTNER <> String.Empty Then
            '        dtPartners = partnersTA.GetPartnerByPartner(user.PARTNER)
            '        If dtPartners.Rows.Count = 0 Then
            '            vld.AddErrorMessage("The partner does not exist", Me.Page)
            '            isValid = False
            '        End If
            '    End If
            '    '-----------------------------------------
            '    ' Check partner/user doesn't already exist
            '    '-----------------------------------------
            '    Dim dtUsers As Data.DataTable
            '    If user.PARTNER <> String.Empty AndAlso user.LOGINID <> String.Empty Then
            '        dtUsers = usersTA.GetDataByPartnerUser(user.PARTNER, user.LOGINID)
            '        Select Case addMode
            '            Case True
            '                If dtUsers.Rows.Count > 0 Then
            '                    vld.AddErrorMessage("The partner/user already exists", Me.Page)
            '                    isValid = False
            '                End If
            '            Case False
            '                For Each row As Data.DataRow In dtUsers.Rows
            '                    If CInt(row("PARTNER_USER_ID")) <> user.PARTNER_USER_ID Then
            '                        vld.AddErrorMessage("The partner/user already exists", Me.Page)
            '                        isValid = False
            '                    End If
            '                Next
            '        End Select
            '    End If
            '    '----------------
            '    ' Check DOB valid
            '    '----------------
            '    Dim dv As DetailsView = DetailsView1
            '    For Each row As DetailsViewRow In dv.Rows

            '        If CType(row.Controls(0), System.Web.UI.WebControls.DataControlFieldCell).Text = "DOB" Then
            '            Dim dobString As String = CType(row.Controls(1).Controls(0), TextBox).Text
            '            If dobString <> String.Empty Then
            '                Try
            '                    Date.Parse(dobString)
            '                Catch ex As Exception
            '                    vld.AddErrorMessage("Invalid date of birth entered", Me.Page)
            '                    isValid = False
            '                End Try
            '            End If
            '            Exit For
            '        End If
            '    Next

        Catch ex As Exception
            isValid = False
        End Try

        Return isValid
    End Function


    Protected Sub DetailsView1_ItemUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdateEventArgs) Handles DetailsView1.ItemUpdating
        Dim modulesTA As New ModulesTableAdapters.tbl_supplynet_module_defaultsTableAdapter
        Dim moduleDefault As Modules.tbl_supplynet_module_defaultsRow = modulesTA.GetDataByID(CInt(Request("ID")))(0)

        Dim tab As New Modules.tbl_supplynet_module_defaultsDataTable
        '    user.BeginEdit()

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
            '-------------------
            ' Validation section
            '-------------------
            Dim isValid As Boolean = ValidateFields(moduleDefault)

            If isValid Then
                modulesTA.UpdateModuleDefaultByID(moduleDefault.BUSINESS_UNIT, moduleDefault._MODULE, moduleDefault.REQUEST_CURRENT_VERSION, _
                    moduleDefault.XML_SCHEMA_DOCUMENT, moduleDefault.MODULE_TYPE, moduleDefault.LOGGING_ENABLED, moduleDefault.STORE_XML, _
                    moduleDefault.OUTGOING_XML_RESPONSE, moduleDefault.AUTO_PROCESS, moduleDefault.AUTO_PROCESS_WAIT_PERIOD_MINUTES, _
                    moduleDefault.RETRY_FAILURES, moduleDefault.RETRY_ATTEMPTS, moduleDefault.RETRY_WAIT_TIME, moduleDefault.RETRY_ERROR_NUMBERS, _
                    CInt(moduleDefault.SUPPLYNET_MODULE_ID))
            End If

        Catch ex As Exception
        End Try

    End Sub

    Protected Sub DetailsView1_ModeChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewModeEventArgs) Handles DetailsView1.ModeChanging
        If e.CancelingEdit Then
            Dim redirectURL As String = String.Empty
            redirectURL = "~/Modules/ModuleAdmin.aspx"
            If Not Request("BU") Is Nothing Then
                redirectURL &= "?BU=" & Request("BU")
            End If
            Response.Redirect(redirectURL)

        End If
    End Sub
End Class

