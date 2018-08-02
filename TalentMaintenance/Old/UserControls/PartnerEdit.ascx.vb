Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Partner Details Control  
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
Partial Class UserControls_PartnerEdit
    Inherits System.Web.UI.UserControl

    Private WithEvents cboDatabase As DropDownList
    Private WithEvents txtDatabase As TextBox
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
        ''    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        ''    .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
        ''    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        ''    .KeyCode = "DetailPartner.ascx"
        ''End With
    End Sub
    Private Sub SetLabelText()
        Try
            ''With ucr
            ''    lblPARTNER.Text = .Content("PARTNER", LCID, True)
            ''    lblDescription.Text = .Content("PARTNER_DESC", LCID, True)
            ''    lblDATABASE.Text = .Content("DESTINATION_DATABASE", LCID, True)
            ''    lblEMAIL.Text = .Content("EMAIL", LCID, True)
            ''    lblACCOUNT_NO_5.Text = .Content("ACCOUNT_NO_5", LCID, True)
            ''    lblACCOUNT_NO_4.Text = .Content("ACCOUNT_NO_4", LCID, True)
            ''    lblACCOUNT_NO_3.Text = .Content("ACCOUNT_NO_3", LCID, True)
            ''    lblACCOUNT_NO_2.Text = .Content("ACCOUNT_NO_2", LCID, True)
            ''    lblACCOUNT_NO_1.Text = .Content("ACCOUNT_NO_1", LCID, True)
            ''    lblTELEPHONE.Text = .Content("TELEPHONE_NUMBER", LCID, True)
            ''    lblFAX_NUMBER.Text = .Content("FAX_NUMBER", LCID, True)
            ''    lblPARTNERURL.Text = .Content("PARTNER_URL", LCID, True)
            ''    lblPARTNERNUMBER.Text = .Content("PARTNER_NUMBER", LCID, True)
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
    Public Sub SetupRequiredFieldValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        rfv.ErrorMessage = rfv.ControlToValidate.Substring(3) & " is a Compulsary Field"
    End Sub
    Protected Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        Select Case rev.ControlToValidate
            Case Is = "txtPARTNER", _
                        "txtDescription"
                rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                rev.ErrorMessage = rev.ControlToValidate & " is a Compulsary Field"
            Case Is = "txtEmail"
                rev.ValidationExpression = "^.+@[^\.].*\.[a-zA-Z]{2,}$"
                rev.ErrorMessage = "Invalid Email Address"
            Case Is = "txtTelephone"
                rev.ValidationExpression = "^[+]?[0-9]{5,8}[\s]?[0-9]{6,9}$"
                rev.ErrorMessage = "Invalid Telephone Number"
            Case Is = "txtFax"
                rev.ValidationExpression = "^[+]?[0-9]{5,8}[\s]?[0-9]{6,9}$"
                rev.ErrorMessage = "Invalid Fax Number"
            Case Is = "txtCacheTime"
                rev.ValidationExpression = "^\d*\.{0,1}\d+$"
                rev.ErrorMessage = "Invalid Cache Time"

        End Select
    End Sub

    Protected Sub cboDatabase_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDatabase.Load
        cboDatabase = CType(frmDataTable.FindControl("cboDatabase"), DropDownList)
        txtDatabase = CType(frmDataTable.FindControl("txtDatabase"), TextBox)
    End Sub
    Protected Sub CboDatabase_Change(ByVal sender As Object, ByVal e As EventArgs) Handles cboDatabase.SelectedIndexChanged
        txtDatabase.Text = cboDatabase.SelectedValue
    End Sub
    Protected Sub txtDatabase_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDatabase.TextChanged
        If (txtDatabase.Text).ToString > String.Empty Then _
            cboDatabase.Text = txtDatabase.Text
    End Sub

End Class
