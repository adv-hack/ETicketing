Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Address Detail Page
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
Partial Class PagesMaint_UserAddress
    Inherits System.Web.UI.Page
    Protected NavigateUrl As String = String.Empty
 
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        NavigateUrl = "~/PagesMaint/UserAddressSelect.aspx" & _
                    "?Partner=" & Request.QueryString("Partner") & _
                    "&LoginID=" & Request.QueryString("LoginID")
    End Sub

End Class
