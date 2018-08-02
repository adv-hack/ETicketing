Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Business Unit Details Control  
'
'       Date                        Mar 2007
'
'       Author                      Andy White 
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCPRTLST- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_BusinessUnitEditl
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Dim mode As String = Request.QueryString("mode")
            Select Case mode
                Case Is = "Edit"
                    Me.InsertBusinessUnitForm.ChangeMode(FormViewMode.Edit)
                Case Is = "Delete"
                    Me.InsertBusinessUnitForm.ChangeMode(FormViewMode.ReadOnly)
                Case Is = "Insert"
                    Me.InsertBusinessUnitForm.ChangeMode(FormViewMode.Insert)
                Case Else
                    Me.InsertBusinessUnitForm.ChangeMode(FormViewMode.Insert)
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
        Try
            'With ucr

            'End With
            '
        Catch ex As Exception
        End Try
    End Sub
    Private Sub SetLabelText()
        Try
            'With ucr

            'End With
            '
        Catch ex As Exception
        End Try
    End Sub
    Public Sub SetupRequiredFieldValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        rfv.ErrorMessage = rfv.ControlToValidate.Substring(3) & " is a Compulsary Field"
    End Sub

End Class
