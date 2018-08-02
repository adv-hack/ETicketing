Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'        Function                   User Address Selection Control  
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
Partial Class UserControls_AddressList
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
            Response.Redirect("~/PagesMaint/UserAddress.aspx?ID=" & _
                        Me.grdDataTable.SelectedRow.Cells(1).Text & "&Mode=Delete&pa=" & _
                        Me.grdDataTable.SelectedRow.Cells(2).Text & "&lo=" & _
                        Me.grdDataTable.SelectedRow.Cells(3).Text)
    End Sub
    Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
            Response.Redirect("~/PagesMaint/UserAddress.aspx?ID=" & _
                        Me.grdDataTable.SelectedRow.Cells(1).Text & "&Mode=Edit&pa=" & _
                        Me.grdDataTable.SelectedRow.Cells(2).Text & "&lo=" & _
                        Me.grdDataTable.SelectedRow.Cells(3).Text)
    End Sub
    Protected Sub cmdInsert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdInsert.Click
        Response.Redirect("~/PagesMaint/UserAddress.aspx?ID=-99&Mode=Insert&pa=" & _
                        Me.grdDataTable.SelectedRow.Cells(2).Text & "&lo=" & _
                        Me.grdDataTable.SelectedRow.Cells(3).Text)

    End Sub
End Class
