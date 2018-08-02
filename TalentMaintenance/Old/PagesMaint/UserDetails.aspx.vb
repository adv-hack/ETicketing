Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Detail Page
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
Partial Class PagesMaint_UserDetails
    Inherits System.Web.UI.Page
    Protected NavigateUrl As String = String.Empty
    Protected Partner As String = String.Empty
    Protected LoginID As String = String.Empty

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoginID = Request.QueryString("LoginID")
        Partner = Request.QueryString("Partner")
        NavigateUrl = "~/PagesMaint/UserAddressSelect.aspx" & _
                      "?Partner=" & Partner & "&LoginID=" & LoginID
    End Sub

End Class
