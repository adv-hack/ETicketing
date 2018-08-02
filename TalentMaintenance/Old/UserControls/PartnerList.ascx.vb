Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Partner Selection Control  
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
Partial Class UserControls_PartnerList
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
            Response.Redirect("~/PagesMaint/PartnerDetails.aspx?ID=" & Me.grdDataTable.SelectedRow.Cells(1).Text & "&Mode=Delete")
    End Sub
    Protected Sub cmdEdit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
        If Me.grdDataTable.SelectedRow IsNot Nothing Then _
            Response.Redirect("~/PagesMaint/PartnerDetails.aspx?ID=" & Me.grdDataTable.SelectedRow.Cells(1).Text & "&Mode=Edit")
    End Sub
    Protected Sub cmdInsert_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdInsert.Click
        Response.Redirect("~/PagesMaint/PartnerDetails.aspx?ID=-99&Mode=Insert")
    End Sub

End Class
