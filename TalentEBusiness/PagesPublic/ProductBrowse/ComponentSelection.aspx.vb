Imports System.Data.SqlClient
Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports System.Linq

Partial Class PagesPublic_ProductBrowse_ComponentSelection
    Inherits TalentBase01

    Private talPackage As New TalentPackage()
    Private _settings As New DESettings()
    Private PackageId As Long
    Private ProductCode As String
    Private PackageQuantity As String
    Private Stage As Integer = 0
    Private packageBasketPrice As Decimal = 0.0
    Private ShowContent As Boolean
    Private repeaterCounter As Integer = 0
    Private TotalRepeaterItems As Integer
    Private StageText As String = ""
    Private CompletedText As String = ""
    Private NotCompletedText As String = ""
    Private _wfrPage As WebFormResource = Nothing
    Dim err As New ErrorObj
    Public errMsg As Talent.Common.TalentErrorMessages
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage


    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        _wfrPage = New WebFormResource
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
        End With

        _settings = Talent.eCommerce.Utilities.GetSettingsObject()
        PackageId = If(String.IsNullOrEmpty(Request.QueryString("PackageId")), 0, Long.Parse(Request.QueryString("PackageId")))
        ProductCode = If(String.IsNullOrEmpty(Request.QueryString("product")), String.Empty, Request.QueryString("product"))
        Stage = If(String.IsNullOrEmpty(Request.QueryString("Stage")), 0, Long.Parse(Request.QueryString("Stage")))

        'We must first check if this package and product exists.  The ticket monitor may have released these tickets but they will remain in session
        Dim found As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.PACKAGE_ID = PackageId AndAlso tbi.Product = ProductCode Then
                found = True
                packageBasketPrice = tbi.Gross_Price
                Exit For
            End If
        Next

        If Not found Then
            'Redirect to the basket page so the user understands that they have nothing in their basket
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        Else
            'Populate the Text
            Settext()

            ' Populate the repeater
            uscPackageSummary.ProductCode = ProductCode
            uscPackageSummary.PackageId = PackageId
            uscPackageSummary.Mode = PackageSummaryMode.Edit
            BindRepeaterAndControls()
        End If


    End Sub

    Private Sub SetText()
        CompletedText = _wfrPage.Content("Completed", _languageCode, True)
        NotCompletedText = _wfrPage.Content("Completed", _languageCode, True)
        StageText = _wfrPage.Content("StageText", _languageCode, True)
    End Sub


    Private Sub BindRepeaterAndControls()

        Dim AllCompleted As Boolean = True
        err = talPackage.GetCustomerPackageInformation(_settings, Profile.Basket.Basket_Header_ID, PackageId, ProductCode, True, packageBasketPrice)
        If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then

            'TODO: Handle errors
            'Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
            'Talent.eCommerce.Utilities.GetCurrentPageName, _
            '"FailedGettingSeatDetails").ERROR_MESSAGE
            'DisplayError(errMessage)
        Else



            Dim PackageDetails = (From r In talPackage.ResultDataSet.Tables("Package").AsEnumerable()
                                  Select New With
                                        {
                                            .Quantity = r.Field(Of String)("Quantity"),
                                            .PackageDescription = r.Field(Of String)("PackageDescription")
                                        }).FirstOrDefault()

            PackageQuantity = PackageDetails.Quantity
            ltlPackageHeader.Text = PackageDetails.PackageDescription

            Dim _talentProduct As New TalentProduct
            Dim err As ErrorObj = _talentProduct.ProductDetails(Talent.eCommerce.Utilities.GetSettingsObject, ProductCode)
            If Not err.HasError AndAlso _talentProduct.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" Then
                ltlProductHeader.Text = _talentProduct.ResultDataSet.Tables(2).Rows(0).Item("ProductDescription")
            End If

            'Add Package/Product specific content from tbl_product_specific_content
            Dim dtSpecificContent As DataTable = TDataObjects.ProductsSettings.TblProductSpecificContent.GetPackageContent("Package", PackageId, ProductCode)
            If dtSpecificContent.Rows.Count > 0 Then
                ltlSpecificContent1.Text = dtSpecificContent.Rows(0).Item("Product_Content").ToString
            Else
                dtSpecificContent = TDataObjects.ProductsSettings.TblProductSpecificContent.GetPackageContent("Package", PackageId)
                If dtSpecificContent.Rows.Count > 0 Then
                    ltlSpecificContent1.Text = dtSpecificContent.Rows(0).Item("Product_Content").ToString
                End If
            End If

            ShowContent = True
            Dim DtComponent As DataTable = talPackage.ResultDataSet.Tables("Component")
            Dim filterCondition As New StringBuilder
            filterCondition.Append("Type= 'G'")
            filterCondition.Append("Or (Type= 'C' and ComponentGroupId=0)")

            DtComponent.DefaultView.RowFilter = filterCondition.ToString()
            DtComponent.DefaultView.Sort = "ComponentGroupSequence Desc"

            TotalRepeaterItems = DtComponent.DefaultView.Count
            rptComponents.DataSource = DtComponent.DefaultView
            rptComponents.DataBind()
        End If

    End Sub

    Private Sub DisplayError(ByVal errMessage As String)
        Dim errorList As BulletedList = Me.Parent.FindControl("ErrorList")
        If Not errorList Is Nothing Then
            If errorList.Items.FindByText(errMessage) Is Nothing Then errorList.Items.Add(errMessage)
        End If
    End Sub

    Protected Sub rptComponents_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptComponents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then

            'Set the completed text and icons
            Dim icnComplete As HtmlGenericControl = CType(e.Item.FindControl("icnCompleted"), HtmlGenericControl)
            Dim icnNotComplete As HtmlGenericControl = CType(e.Item.FindControl("icnNotCompleted"), HtmlGenericControl)
            If e.Item.DataItem("Completed") Then
                icnComplete.Visible = True
                icnNotComplete.Visible = False
                CType(e.Item.FindControl("ltlCompleted"), Literal).Text = _wfrPage.Content("Completed", _languageCode, True)
            Else
                icnComplete.Visible = False
                icnNotComplete.Visible = True
                CType(e.Item.FindControl("ltlCompleted"), Literal).Text = _wfrPage.Content("NotCompleted", _languageCode, True)
            End If


            'Add anchor
            Dim lnkStage As HtmlAnchor = CType(e.Item.FindControl("lnkStage"), HtmlAnchor)
            lnkStage.Name = "Stage" & repeaterCounter.ToString

            'Populate the dictionary objects on the first item
            If e.Item.DataItem("Type") = "C" Then

                'Populate the properties
                repeaterCounter += 1
                CType(e.Item.FindControl("uscComponentSeatsDetails"), UserControls_Package_ComponentSeats).ComponentId = e.Item.DataItem("ComponentId")
                CType(e.Item.FindControl("uscComponentSeatsDetails"), UserControls_Package_ComponentSeats).PackageId = PackageId
                CType(e.Item.FindControl("uscComponentSeatsDetails"), UserControls_Package_ComponentSeats).ProductCode = e.Item.DataItem("ProductCode")
                CType(e.Item.FindControl("uscComponentSeatsDetails"), UserControls_Package_ComponentSeats).Stage = repeaterCounter
                If TotalRepeaterItems = repeaterCounter Then
                    CType(e.Item.FindControl("uscComponentSeatsDetails"), UserControls_Package_ComponentSeats).LastStage = True
                End If

                'Populate the header
                CType(e.Item.FindControl("ltlComponentHeader"), Literal).Text = StageText.Replace("<<<stage_number>>>", repeaterCounter.ToString) & e.Item.DataItem("ComponentDescription")

                'Should we display this detail
                If ShowContent Then
                    If Not e.Item.DataItem("Completed") Or Stage = repeaterCounter Then
                        CType(e.Item.FindControl("uscComponentSeatsDetails"), UserControls_Package_ComponentSeats).Display = True
                        ShowContent = False
                    End If
                End If


            ElseIf e.Item.DataItem("Type") = "G" Then

                'Populate the properties
                repeaterCounter += 1
                CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).ComponentGroupId = e.Item.DataItem("ComponentGroupId")
                CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).AvailableComponentQuantity = PackageQuantity
                CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).PackageId = PackageId
                CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).ProductCode = e.Item.DataItem("ProductCode")
                CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).TicketingProductCode = ProductCode
                CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).Stage = repeaterCounter
                If TotalRepeaterItems = repeaterCounter Then
                    CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).LastStage = True
                End If

                'Populate the header
                Dim groupDescription As String = _wfrPage.Content(e.Item.DataItem("ComponentGroupId") & "_HeaderText", _languageCode, True)
                CType(e.Item.FindControl("ltlComponentHeader"), Literal).Text = StageText.Replace("<<<stage_number>>>", repeaterCounter.ToString) & groupDescription

                'Should we display this detail
                If ShowContent Then
                    If Not e.Item.DataItem("Completed") Or Stage = repeaterCounter Then
                        CType(e.Item.FindControl("uscTravelAndAccommodationGroup"), UserControls_Package_TravelAndAccommodationGroup).Display = True
                        ShowContent = False
                        PagePosition(lnkStage)
                    End If
                End If

            End If
        End If
    End Sub

    Protected Sub Page_Load1(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True
        
    End Sub

    Private Sub PagePosition(ByVal anchor As HtmlAnchor)
        'Build the javascript
        Dim sbJavaScript As New StringBuilder
        sbJavaScript.Append("<script language=""javascript"" type=""text/javascript"">")
        sbJavaScript.Append(" " & "function ScrollView() { ")
        sbJavaScript.Append(" " & "var el = document.getElementById('" & anchor.ClientID & "'); ")
        sbJavaScript.Append(" " & "if (el != null) {")
        sbJavaScript.Append(" " & "el.scrollIntoView();")
        sbJavaScript.Append(" " & "el.focus(); }}")
        sbJavaScript.Append(" " & "window.onload = ScrollView;")
        sbJavaScript.Append(" " & "</script>")
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "PageScrollViewJS", sbJavaScript.ToString(), False)
    End Sub
End Class
