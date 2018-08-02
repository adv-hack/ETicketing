Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Order Enquiry 
'
'       Date                        Jun 2007
'
'       Author                      Ben
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PLIENQ- 
'                                    
'       User Controls
'           orderEnquiry
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_CreditNoteEnquiryDetail
    Inherits TalentBase01

    Protected Sub Page_PreLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreLoad
        Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim myDefaults As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        myDefaults = defaults.GetDefaults

        SetAllInvisible()

        Select Case myDefaults.CreditNoteDetailTemplate
            Case Is = "1"
                CreditNoteEnquiryDetail1.Visible = True
            Case Else
                CreditNoteEnquiryDetail1.Visible = True
        End Select
    End Sub

    Protected Sub SetAllInvisible()
        CreditNoteEnquiryDetail1.Visible = False
    End Sub

End Class
