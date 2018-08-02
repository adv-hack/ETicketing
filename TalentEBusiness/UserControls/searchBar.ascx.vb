Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Search bar
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCPLISB- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_searchBar
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource()
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Public Search As String

    Public ReadOnly Property DialogTitle() As String
        Get
            Return "Title Message"
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If CheckSearchType() = "OCELLUZ" Then
            hypAdvancedSearch.Visible = False
        End If

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = ProfileHelper.GetPageName
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "searchBar.ascx"
        End With

        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ShowSearchBar")) Then
            Search = ucr.Content("labelSearchText", _languageCode, True)
            btnSubmit.Text = ucr.Content("submitButtonText", _languageCode, True)
            hypAdvancedSearch.Text = ucr.Content("linkAdvancedSearch", _languageCode, True)
            aceProductSearch.Enabled = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("EnableACEProductSearch"))
            aceProductSearch.CompletionSetCount = Talent.eCommerce.Utilities.CheckForDBNull_Int(ucr.Attribute("ACEProductSearchCompletionSetCount"))

            ' wont work on quick order page as both will search for Options
            If aceProductSearch.Enabled AndAlso Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper = "QUICKORDER.ASPX" Then
                Me.Visible = False
            End If

            If ModuleDefaults.UseEPOSOptions Then
                plhAddToBasket.Visible = True
                'pnlSearchBar.DefaultButton = "btnAddToBasket"
                btnAddToBasket.Text = ucr.Content("AddToBasketButtonText", _languageCode, True)
            Else
                plhAddToBasket.Visible = False
                'pnlSearchBar.DefaultButton = "btnSubmit"
            End If
           plhAdvancedSearch.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ShowAdvancedSearch")) 
        Else
        Visible = False
        End If
    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Not txbSearch.Text.Equals(String.Empty) Then
            Dim redirectString As New StringBuilder
            '------------------
            ' Check search type
            '------------------
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Select Case def.AdditionalSearchType
                Case Is = "OCELLUZ"
                    '---------------
                    ' Ocelluz Search
                    '---------------
                    redirectString.Append("~/PagesPublic/Search/SearchResults1.aspx?source=quick")
                Case Else
                    '----------------
                    ' Standard search
                    '----------------
                    redirectString.Append("~/PagesPublic/Search/searchResults.aspx?source=quick")
            End Select
            If aceProductSearch.Enabled Then
                redirectString.Append("&keyword1=" & splitSearchTerm(txbSearch.Text))
            Else

                Dim keywordArray() As String = txbSearch.Text.Split(" ")
                Dim i As Integer
                For i = 0 To keywordArray.Length - 1
                    redirectString.Append("&keyword" & (i + 1).ToString & "=" & keywordArray(i))
                Next
                redirectString.Append("&searchterm=" & txbSearch.Text.Trim)
            End If
            Response.Redirect(redirectString.ToString)
        End If
    End Sub

    Protected Sub btnAddTobasket_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddToBasket.Click
        If Not String.IsNullOrEmpty(txbSearch.Text.Trim) Then
            Dim tbi As New TalentBasketItem
            With tbi
                .Product = txbSearch.Text.Trim

                Dim products As Data.DataTable = Utilities.GetProductInfo(txbSearch.Text.Trim)
                If products IsNot Nothing Then
                    If products.Rows.Count > 0 Then
                        .ALTERNATE_SKU = Utilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                        .PRODUCT_DESCRIPTION1 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                        .PRODUCT_DESCRIPTION2 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                        .PRODUCT_DESCRIPTION3 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                        .PRODUCT_DESCRIPTION4 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                        .PRODUCT_DESCRIPTION5 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                        .Xml_Config = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PERSONALISABLE"))
                    End If
                End If

                .Quantity = ModuleDefaults.Default_Add_Quantity

                If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                    .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                    .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                End If

                If String.IsNullOrWhiteSpace(.MASTER_PRODUCT) Then
                    .MASTER_PRODUCT = .Product
                End If

                Select Case ModuleDefaults.PricingType
                    Case 2
                        Dim prices As Data.DataTable = Talent.eCommerce.Utilities.GetChorusPrice(.Product, .Quantity)
                        If prices.Rows.Count > 0 Then
                            .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                            .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                            .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                        End If
                    Case Else
                        Dim deWp As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(.Product, .Quantity, .MASTER_PRODUCT)
                        .Gross_Price = deWp.Purchase_Price_Gross
                        .Net_Price = deWp.Purchase_Price_Net
                        .Tax_Price = deWp.Purchase_Price_Tax
                End Select

                .GROUP_LEVEL_01 = String.Empty
                .GROUP_LEVEL_02 = String.Empty
                .GROUP_LEVEL_03 = String.Empty
                .GROUP_LEVEL_04 = String.Empty
                .GROUP_LEVEL_05 = String.Empty
                .GROUP_LEVEL_06 = String.Empty
                .GROUP_LEVEL_07 = String.Empty
                .GROUP_LEVEL_08 = String.Empty
                .GROUP_LEVEL_09 = String.Empty
                .GROUP_LEVEL_10 = String.Empty
            End With
            Profile.Basket.AddItem(tbi)
            Response.Redirect("../../pagesPublic/basket/Basket.aspx")
        End If
    End Sub

    Private Function CheckSearchType() As String
        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim searchType As String = String.Empty
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim currentLanguage = Talent.Common.Utilities.GetDefaultLanguage()

        Dim searchCriteria As String() = Nothing
        If Not def.AdditionalSearchTypeCriteria Is Nothing Then

            searchCriteria = def.AdditionalSearchTypeCriteria.Split(",")
        End If


        Select Case def.AdditionalSearchType
            Case Is = "OCELLUZ"
                '---------------------------------
                ' Check if search criteria matches
                '---------------------------------
                For Each item As String In searchCriteria
                    If item.Equals(currentLanguage) Then
                        searchType = "OCELLUZ"
                        Exit For
                    End If
                Next
            Case Else

        End Select

        Return searchType
    End Function

    Private Function splitSearchTerm(ByVal searchText As String) As String
        Dim editedSearchText As String = String.Empty
        If String.IsNullOrEmpty(Talent.eCommerce.Utilities.CheckForDBNull_String(ucr.Attribute("SearchSplitString"))) Then
            editedSearchText = searchText
        Else
            Dim searchStringArray() As String = searchText.Split(ucr.Attribute("SearchSplitString"))
            editedSearchText = searchStringArray(0).Trim()
        End If
        Return editedSearchText
    End Function

End Class
