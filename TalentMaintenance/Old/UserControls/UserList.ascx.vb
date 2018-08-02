Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'        Function                   User Selection Control  
'
'       Date                        Mar 2007
'
'       Author                      Andy White 
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCPRTDIS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_UserList
    Inherits System.Web.UI.UserControl
    Protected BusinessUnit As String = String.Empty
    Protected Partner As String = String.Empty
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            '--------------------------------------------------------------------
            Try
                BusinessUnit = Request.QueryString("bu")
                If BusinessUnit.Length > 0 Then _
                    cboBusinessUnit.SelectedValue = BusinessUnit
            Catch ex As Exception
            End Try
            '--------------------------------------------------------------------
            Try
                Partner = Request.QueryString("pa")
                If Partner.Length > 0 Then _
                    cboPartner.SelectedValue = Partner
            Catch ex As Exception
            End Try
            '--------------------------------------------------------------------
        End If
    End Sub
    Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
            Response.Redirect("~/PagesMaint/UserDetails.aspx?ID=" & Me.grdDataTable.SelectedRow.Cells(1).Text & "&Mode=Delete")
    End Sub
    Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
            Response.Redirect("~/PagesMaint/UserDetails.aspx?ID=" & Me.grdDataTable.SelectedRow.Cells(1).Text & "&Mode=Edit")
    End Sub
    Protected Sub cmdInsert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdInsert.Click
        Response.Redirect("~/PagesMaint/UserDetails.aspx?ID=-99&Mode=Insert")
    End Sub
    Protected Sub cmdAddress_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddress.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
                Response.Redirect("~/PagesMaint/UserAddressSelect.aspx?pa=" & _
                        Me.grdDataTable.SelectedRow.Cells(3).Text & "&lo=" & _
                        Me.grdDataTable.SelectedRow.Cells(4).Text)
    End Sub
    Protected Sub cmdGo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdgo.Click

        Response.Redirect("~/PagesMaint/UserSelect.aspx?bu=" & cboBusinessUnit.SelectedValue & _
                            "&pa=" & cboPartner.SelectedValue)
    End Sub
End Class
