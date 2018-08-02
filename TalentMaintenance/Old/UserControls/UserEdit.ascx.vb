Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Details Control  
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TMAINS1- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_UserEdit
    Inherits System.Web.UI.UserControl
    Private WithEvents cboBusinessUnit As DropDownList
    Private WithEvents cboPartner As DropDownList
    Private WithEvents txtBusinessUnit As TextBox
    Private WithEvents txtPartner As TextBox
    Private WithEvents InsertButton As LinkButton

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim mode As String = Request.QueryString("mode")
            Select Case mode
                Case Is = "Edit"
                    Me.frmDataTable.ChangeMode(FormViewMode.Edit)
                Case Is = "Delete"
                    Me.frmDataTable.ChangeMode(FormViewMode.ReadOnly)
                Case Is = "Insert"
                    Me.frmDataTable.ChangeMode(FormViewMode.Insert)
                Case Else
                    Me.frmDataTable.ChangeMode(FormViewMode.Insert)
            End Select
        End If
    End Sub
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        SetUpUCR()
        If Not Page.IsPostBack Then
            SetLabelText()
        End If
    End Sub
    Protected Sub SetUpUCR()
        ''With ucr
        ''    .BusinessUnit = TalentCache.GetBusinessUnit
        ''    .UserCode = TalentCache.GetUser(HttpContext.Current.Profile)

        ''End With
    End Sub
    Private Sub SetLabelText()
        Try
            ''With ucr
            ''    lblUser.Text = .Content("User", LCID, True)
            ''    lblDescription.Text = .Content("User_DESC", LCID, True)
            ''    lblDATABASE.Text = .Content("DESTINATION_DATABASE", LCID, True)
            ''    lblEMAIL.Text = .Content("EMAIL", LCID, True)
            ''    lblACCOUNT_NO_5.Text = .Content("ACCOUNT_NO_5", LCID, True)
            ''    lblACCOUNT_NO_4.Text = .Content("ACCOUNT_NO_4", LCID, True)
            ''    lblACCOUNT_NO_3.Text = .Content("ACCOUNT_NO_3", LCID, True)
            ''    lblACCOUNT_NO_2.Text = .Content("ACCOUNT_NO_2", LCID, True)
            ''    lblACCOUNT_NO_1.Text = .Content("ACCOUNT_NO_1", LCID, True)
            ''    lblTELEPHONE.Text = .Content("TELEPHONE_NUMBER", LCID, True)
            ''    lblFAX_NUMBER.Text = .Content("FAX_NUMBER", LCID, True)
            ''    lblUserURL.Text = .Content("User_URL", LCID, True)
            ''    lblUserNUMBER.Text = .Content("User_NUMBER", LCID, True)
            ''    lblORIGINATINGBUSINESS_UNIT.Text = .Content("ORIGINATING_BUSINESS_UNIT", LCID, True)
            ''    lblSTOREXML.Text = .Content("STORE_XML", LCID, True)
            ''    lblLOGGINGENABLED.Text = .Content("LOGGING_ENABLED", LCID, True)
            ''    lblCACHE_TIME_MINUTES.Text = .Content("CACHE_TIME_MINUTES", LCID, True)
            ''    lblCACHEING_ENABLED.Text = .Content("CACHEING_ENABLED", LCID, True)
            ''End With
            '
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub SetupRequiredFieldValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        rfv.ErrorMessage = rfv.ControlToValidate.Substring(3) & " is a Compulsary Field"
    End Sub

    Protected Sub cboBusinessUnit_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboBusinessUnit.Load
        cboBusinessUnit = CType(frmDataTable.FindControl("cboBusinessUnit"), DropDownList)
        txtBusinessUnit = CType(frmDataTable.FindControl("txtBusinessUnit"), TextBox)
        With cboBusinessUnit
            .DataSource = SqlDataSource1
            .DataTextField = "BUSINESS_UNIT"
            .DataValueField = "BUSINESS_UNIT"
        End With
    End Sub
    Protected Sub cboBusinessUnit_Change(ByVal sender As Object, ByVal e As EventArgs) Handles cboBusinessUnit.SelectedIndexChanged
        txtBusinessUnit.Text = cboBusinessUnit.SelectedValue
    End Sub

    Protected Sub cboPartner_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPartner.Load
        cboPartner = CType(frmDataTable.FindControl("cboPartner"), DropDownList)
        txtPartner = CType(frmDataTable.FindControl("txtPartner"), TextBox)
        With cboPartner
            .DataSource = SqlDataSource2
            .DataTextField = "PARTNER"
            .DataValueField = "PARTNER"
        End With
    End Sub
    Protected Sub CboPartner_Change(ByVal sender As Object, ByVal e As EventArgs) Handles cboPartner.SelectedIndexChanged
        txtPartner.Text = cboPartner.SelectedValue
    End Sub

    Protected Sub txtBusinessUnit_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtBusinessUnit.TextChanged
        If (txtBusinessUnit.Text).ToString > String.Empty Then
            txtBusinessUnit.Text = txtBusinessUnit.Text.ToUpper
            cboBusinessUnit.Text = txtBusinessUnit.Text
        End If
    End Sub
    Protected Sub txtPartner_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtPartner.TextChanged
        If (txtPartner.Text).ToString > String.Empty Then
            txtPartner.Text = txtPartner.Text.ToUpper
            cboPartner.Text = txtPartner.Text
        End If
    End Sub
    Protected Sub InsertButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles InsertButton.Click

        If (txtBusinessUnit.Text).ToString = String.Empty Then txtBusinessUnit.Text = cboBusinessUnit.Text
        If (txtPartner.Text).ToString = String.Empty Then txtPartner.Text = cboPartner.Text

    End Sub

End Class
