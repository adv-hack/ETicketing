Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Order Enquiry 
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PLOENQ- 
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
Partial Class PagesLogin_orderEnquiry
    Inherits TalentBase01
    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private Const LinkCssClassActive As String = "link active"
    Private Const LinkCssClassInActive As String = "link inactive"


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "orderEnquiry.aspx"
        End With

        Dim orderTemplate As String = String.Empty
        Dim orderTemplateSubType As String = String.Empty

        If Not String.IsNullOrEmpty(Request.QueryString("OrderType")) Then
            orderTemplate = Request.QueryString("OrderType").Trim
        ElseIf Session("OrderTemplateType") IsNot Nothing Then
            orderTemplate = Session("OrderTemplateType").ToString.Trim
        End If
        If Not String.IsNullOrWhiteSpace(Request.QueryString("OrderTemplateSubType")) Then
            orderTemplateSubType = Request.QueryString("OrderTemplateSubType").Trim
        ElseIf Session("OrderTemplateSubType") IsNot Nothing Then
            orderTemplateSubType = Session("OrderTemplateSubType")
        End If

        If orderTemplate.Length <= 0 Then
            orderTemplate = MyBase.ModuleDefaults.Order_Enquiry_Template_Type
        End If

        Select Case orderTemplate
            Case Is = "2"
                plhOrderEnquiry2.Visible = True
                OrderEnquiry2.Visible = True
                plhOrderEnquiry1.Visible = False
                OrderEnquiry1.Visible = False
            Case Else
                plhOrderEnquiry2.Visible = False
                OrderEnquiry2.Visible = False
                plhOrderEnquiry1.Visible = True
                OrderEnquiry1.Visible = True
        End Select
    End Sub
End Class
