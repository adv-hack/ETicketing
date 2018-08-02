Imports Microsoft.VisualBasic
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Talent Home Page  
'
'       Date                        Feb 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
' 
'       Error Number Code base      PPUHOME- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_Home
    Inherits TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
           
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            If Session("FromRightBarLogin") IsNot Nothing AndAlso Session("FromRightBarLogin").ToString() = "TRUE" Then
                Session.Remove("FromRightBarLogin")
                Dim basketMini As Object = Talent.eCommerce.Utilities.FindWebControl("MiniBasket1", Me.Page.Controls)
                If basketMini IsNot Nothing Then
                    CallByName(basketMini, "ReBindBasket", CallType.Method)
                End If
            End If
        End If
    End Sub
End Class
