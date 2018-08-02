Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Public - Registration Confirmation
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PPRECON- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_registrationConfirmation
    Inherits TalentBase01

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Select Case MyBase.ModuleDefaults.RegistrationTemplate
            Case Is = "2"
                RegistrationConfirmationPartner1.Visible = True
                RegistrationConfirmationUser1.Visible = False
            Case Else
                RegistrationConfirmationPartner1.Visible = False
                RegistrationConfirmationUser1.Visible = True
        End Select
    End Sub
End Class
