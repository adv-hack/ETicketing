Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class PagesPublic_browse09
    Inherits TalentBase01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '--------------------------------------------------------------
        ' If only *EMPTY groups exist then redirect to product list
        '--------------------------------------------------------------
        ProductList.Display = False
        If Not Page.IsPostBack Then
            CheckForEmptyGroup()
        End If

        GroupGraphical.Display = False
        ProductList.Display = False
        ProductList2.Display = False
        ProductListGraphical.Display = False
        ProductListGraphical2.Display = False

        '--------------------------------------------------------------
        ' Check whether to display groups or products 
        ' and, if products whether, to display as list or graphics
        '--------------------------------------------------------------
        ' - Use existing routine which will use a higher level group 
        '   if the group is *EMPTY (BF)
        Dim groupDetails As Talent.eCommerce.GroupLevelDetails
        Dim dets As Talent.eCommerce.GroupLevelDetails.GroupDetails
        groupDetails = New Talent.eCommerce.GroupLevelDetails
        dets = groupDetails.GetGroupLevelDetails(Talent.eCommerce.Utilities.GetCurrentPageName)
        If dets.ShowProductDisplay Then
            If ShowChildrenAsGroups() Then
                GroupGraphical.Display = True
            Else
                If Utilities.ShowProductsAsLists() Then
                    Select Case UCase(Utilities.GetProductListTemplateType)
                        Case "2"
                            ProductList2.Display = True
                        Case Else
                            ProductList.Display = True
                    End Select
                Else
                    Select Case ModuleDefaults.Product_List_Graphical_Template_Type
                        Case Is = 1
                            ProductListGraphical.Display = True
                        Case Is = 2, 3
                            ProductListGraphical2.Display = True
                        Case Else
                            ProductListGraphical.Display = True
                    End Select
                End If
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not ProductList.HasRows AndAlso Not ProductList2.HasRows AndAlso Not ProductListGraphical.HasRows _
            AndAlso Not ProductListGraphical2.HasRows AndAlso Not GroupGraphical.HasRows Then
            Dim wfr As New Talent.Common.WebFormResource
            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "browse02.aspx"
            End With
            ltlError.Text = wfr.Content("NoProductsText", Utilities.GetCurrentLanguage, True)
            plhProductList.Visible = False
        Else
            plhProductList.Visible = True
        End If

        If ltlError.Text.Length = 0 AndAlso ProductListGraphical2.ErrorMessage.Length > 0 Then
            ltlError.Text = ProductListGraphical2.ErrorMessage
        End If
        plhErrorMessage.Visible = (ltlError.Text.Length > 0)
    End Sub

End Class