Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic
Imports System.Globalization
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports System.Linq
Imports TCUtilities = Talent.Common.Utilities
Imports Talent.Common.UtilityExtension
Imports Microsoft.VisualBasic

Partial Class UserControls_Package_PackageSummary
    Inherits ControlBase

#Region "Private fields"

    Const COMPLETED As String = "Completed"
    Const INCOMPLETE As String = "Incomplete"
    Private ucr As New Talent.Common.UserControlResource
    Private errorList As New BulletedList
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _settings As New DESettings()
    Private dsCustomerPackageInformation As New DataSet
    Private wfr As New WebFormResource
    Private IsNetTotalAndVATVisible As Boolean = False
    Private packageType As String

#End Region

#Region "Constants"
    Const KEYCODE As String = "PackageSummary.ascx"
#End Region

#Region "Public Fields"
    Public errMsg As Talent.Common.TalentErrorMessages
    Public Property ProductCode() As String = ""
    Public Property PackageId() As Long = 0
    Public Property Display() As Boolean = False

    Public Property TicketsText As String
    Public Property PriceText As String
    Public Property DiscountText As String
    Public Property TotalText As String
    Public Property IsDiscountVisible As Boolean = False

    Public Property Mode As PackageSummaryMode
    Public Property TBI As TalentBasketItem

#End Region

#Region "Protected Methods and Handlers"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With

    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Display Then

            plhPackageSummary.Visible = True


            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = KEYCODE
            End With
            _settings = Talent.eCommerce.Utilities.GetSettingsObject()
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                            TalentCache.GetBusinessUnit, _
                                                            TalentCache.GetPartner(Profile), _
                                                            ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

            'Retrieve the basket so we can compare it against the session values to be certain the session contains the correct values.
            Dim packageBasketPrice As Decimal = 0.0
            For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                If tbi.PACKAGE_ID = PackageId AndAlso tbi.Product = ProductCode Then
                    packageBasketPrice = tbi.Gross_Price
                    Exit For
                End If
            Next

            Dim package As New Talent.Common.TalentPackage
            Dim err As ErrorObj = package.GetCustomerPackageInformation(_settings, Profile.Basket.Basket_Header_ID, PackageId, ProductCode, True, packageBasketPrice)
            If err.HasError OrElse package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                                Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                "FailedGettingPackageDetails").ERROR_MESSAGE
                DisplayError(errMessage)
            Else
                SetControlText()
                dsCustomerPackageInformation = package.ResultDataSet
                BindOuterRepeater()
                DisplayPackageInformation()
                SetButtonVisiblity()
                IsDiscountVisible = IsDiscountDropDownVisible()
            End If
        Else
            plhPackageSummary.Visible = False
        End If

    End Sub


    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Dim strId As String = String.Empty
        If TypeOf sender Is Label Then
            DirectCast(sender, Label).Text = ucr.Content(DirectCast(sender, Label).ID, _languageCode, True)
        ElseIf TypeOf sender Is Button Then
            DirectCast(sender, Button).Text = ucr.Content(DirectCast(sender, Button).ID, _languageCode, True)
        Else
            strId = DirectCast(sender, WebControl).ID

        End If
    End Sub

    ''' <summary>
    ''' component Group (Outer repeater) item data bound handler.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub rptComponentGroups_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptComponentGroups.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As RepeaterItem = e.Item
            Dim drv As DataRowView
            Dim rptComponents As Repeater
            Dim ComponentGroupId As Long
            Dim lblComponentGroup As Label = CType(e.Item.FindControl("lblComponentGroup"), Label)
            Dim plhComponentGroupTitle As PlaceHolder = CType(e.Item.FindControl("plhComponentGroupTitle"), PlaceHolder)

            drv = DirectCast(item.DataItem, DataRowView)
            ComponentGroupId = Long.Parse(drv.Row("ComponentGroupId").ToString())
            Dim dvComponentsInComponentGroup As New DataView(dsCustomerPackageInformation.Tables("Component"))
            dvComponentsInComponentGroup.RowFilter = "Type = 'C' " & " and ComponentGroupId=" & ComponentGroupId.ToString()

            'Should we display the group header
            plhComponentGroupTitle.Visible = False
            If ComponentGroupId > 0 Then
                'Component group headers look a bit rubbish at the moment.  Commented out until I decide whether they are needed at all.
                'lblComponentGroup.Text = wfr.Content(String.Format("{0}_HeaderText", ComponentGroupId), _languageCode, True)
                If Not String.IsNullOrWhiteSpace(lblComponentGroup.Text) AndAlso dvComponentsInComponentGroup.Count > 0 Then plhComponentGroupTitle.Visible = True
            End If

            rptComponents = item.FindControl("rptComponents")
            rptComponents.DataSource = dvComponentsInComponentGroup
            rptComponents.DataBind()
        End If

    End Sub

    ''' <summary>
    ''' Component repeater (Inner repeater) item data bound handler.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub rptComponents_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As RepeaterItem = e.Item
            Dim drv As DataRowView
            Dim txtBDiscount As TextBox
            Dim lblPrice As Label
            Dim PriceBeforeVAT As String
            Dim priceIncludingVAT As String
            Dim Discount As String

            drv = DirectCast(item.DataItem, DataRowView)

            PriceBeforeVAT = drv.Row("PriceBeforeVAT").ToString()
            priceIncludingVAT = drv.Row("PriceIncludingVAT").ToString()
            Discount = drv.Row("Discount").ToString()

            txtBDiscount = item.FindControl("compDiscTB")
            lblPrice = item.FindControl("lblPrice")

            txtBDiscount.Text = Discount

            lblPrice.Text = PriceBeforeVAT
            Dim PackageType = (From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                        Select r.Field(Of String)("packageType")).FirstOrDefault

        End If
    End Sub

#End Region

#Region "Private Methods"
    Private Sub SetButtonText()
        If Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) Then
            'AndAlso Profile.Basket.CAT_MODE.Trim.ToUpper = GlobalConstants.CATMODE_AMEND Then
            btnDeletePackage.Visible = False
            Select Case Profile.Basket.CAT_MODE.Trim
                Case GlobalConstants.CATMODE_AMEND
                    btnDeletePackage.Text = TCUtilities.CheckForDBNull_String(ucr.Content("DeletePackage_Amend", _languageCode, True))
                Case GlobalConstants.CATMODE_TRANSFER
                    btnDeletePackage.Text = TCUtilities.CheckForDBNull_String(ucr.Content("DeletePackage_Transfer", _languageCode, True))
            End Select
        Else
            btnDeletePackage.Text = TCUtilities.CheckForDBNull_String(ucr.Content("DeletePackage", _languageCode, True))
        End If
        btnUpdatePackage.Text = TCUtilities.CheckForDBNull_String(ucr.Content("UpdatePackage", _languageCode, True)).Trim()
        btnShowBasket.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ViewBasket", _languageCode, True))
        btnAmendBasket.Text = TCUtilities.CheckForDBNull_String(ucr.Content("AmendPackage", _languageCode, True))
    End Sub
    Private Sub SetControlText()
        TicketsText = TCUtilities.CheckForDBNull_String(ucr.Content("TicketsText", _languageCode, True))
        PriceText = TCUtilities.CheckForDBNull_String(ucr.Content("PriceText", _languageCode, True))
        DiscountText = TCUtilities.CheckForDBNull_String(ucr.Content("DiscountText", _languageCode, True))
        TotalText = TCUtilities.CheckForDBNull_String(ucr.Content("TotalText", _languageCode, True))
        ltlPackageStatus.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ltlPackageStatus", _languageCode, True))
        lblPackageDiscount.Text = TCUtilities.CheckForDBNull_String(ucr.Content("lblPackageDiscount", _languageCode, True))
        ltlNetTotal.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ltlNetTotal", _languageCode, True))
        ltlVatTotal.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ltlVatTotal", _languageCode, True))
        ltlTotal.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ltlTotal", _languageCode, True))
        ltlSummaryTotals.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ltlSummaryTotals", _languageCode, True))
        ltlPackageLevelDiscount.Text = TCUtilities.CheckForDBNull_String(ucr.Content("ltlPackageLevelDiscount", _languageCode, True))
        btnUpdatePackage.Visible = IsVisible()
        SetButtonText()
    End Sub

    ''' <summary>
    ''' Binds the outer repeater which repeats through the component groups.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BindOuterRepeater()
        Dim view As DataView = New DataView(dsCustomerPackageInformation.Tables("Component"))
        Dim dt As DataTable = view.ToTable(True, "ComponentGroupId")
        lblStatus.Text = IIf((From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                         Select r.Field(Of Boolean)("Completed")).FirstOrDefault(), COMPLETED, INCOMPLETE)

        lblStatus.Visible = IIf((From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                         Select r.Field(Of String)("PackageType")).FirstOrDefault() = GlobalConstants.TICKETINGPRODUCTTYPE, True, False)
        ltlPackageStatus.Visible = lblStatus.Visible

        rptComponentGroups.DataSource = dt
        rptComponentGroups.DataBind()
    End Sub

    Private Sub DisplayPackageInformation()


        Dim dtPackage As DataTable = dsCustomerPackageInformation.Tables("Package")
        If dtPackage IsNot Nothing AndAlso dtPackage.Rows.Count > 0 Then

            Dim PackageType = (From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                        Select r.Field(Of String)("packageType")).FirstOrDefault

            If PackageType <> "T" Then
                IsNetTotalAndVATVisible = True
            End If

            If IsNetTotalAndVATVisible Then
                plhNetAndVAT.Visible = True
                lblPackageNetTotal.Text = GetFormattedPrice((From r In dtPackage.AsEnumerable
                                            Select r.Field(Of Decimal)("PriceBeforeVAT")).FirstOrDefault())

                lblPackageVATTotal.Text = GetFormattedPrice((From r In dtPackage.AsEnumerable
                                            Select r.Field(Of Decimal)("VATPrice")).FirstOrDefault())
            Else
                plhNetAndVAT.Visible = False
            End If

            Dim packageDiscount As Decimal = (From r In dtPackage.AsEnumerable
                                            Select r.Field(Of Decimal)("packageDiscountedByValue")).FirstOrDefault()
            If packageDiscount <> 0 Then
                lblPackageLevelDiscount.Text = GetFormattedPrice(packageDiscount * -1)
                plhPackageLevelDiscount.Visible = True
            Else
                plhPackageLevelDiscount.Visible = False
            End If

                lblPackageTotal.Text = GetFormattedPrice((From r In dtPackage.AsEnumerable
                                            Select r.Field(Of Decimal)("PriceIncludingVAT")).FirstOrDefault())

                'If the discount retreived for the packages does not match one of the values in the drop down (fetched from frontend database), no exception is thrown.
                If AgentProfile.IsAgent AndAlso Me.Mode = PackageSummaryMode.Edit And (AgentProfile.AgentPermissions.CanApplyTicketingPackageDiscounts) Then
                    plhPackageDiscount.Visible = True
                    If Not IsPostBack Then
                        packageDiscTB.Text = (From r In dtPackage.AsEnumerable
                                                            Select r.Field(Of Decimal)("Discount")).FirstOrDefault()
                    End If
                Else
                    plhPackageDiscount.Visible = False
                End If
                SetOutstandingTotal()
            Else
                plhPackageDiscount.Visible = False
                plhNetAndVAT.Visible = False
            End If
    End Sub

    Private Function PreparePackageObjectForUpdateOperation() As Talent.Common.TalentPackage
        Dim rptComponents As Repeater
        Dim ComponentId As String
        Dim ComponentGroupId As String
        Dim Quantity As String
        Dim AmendComponent As AmendComponent
        Dim Discount As String
        Dim possibleDec As Decimal


        Dim package As New Talent.Common.TalentPackage
        package.Settings = _settings
        package.DePackages.PackageID = PackageId
        package.DePackages.ProductCode = ProductCode
        package.DePackages.BasketId = Profile.Basket.Basket_Header_ID
        package.DePackages.BoxOfficeUser = AgentProfile.Name
        package.DePackages.Mode = OperationMode.Amend
        package.DePackages.MarkAsCompleted = True
        If Decimal.TryParse(packageDiscTB.Text, possibleDec) Then
            package.DePackages.Discount = packageDiscTB.Text
        Else
            package.DePackages.Discount = 0
        End If
        package.DePackages.UpdateDiscount = True

        For Each item As RepeaterItem In rptComponentGroups.Items
            If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then
                rptComponents = item.FindControl("rptComponents")
                For Each innerItem As RepeaterItem In rptComponents.Items
                    If innerItem.ItemType = ListItemType.Item OrElse innerItem.ItemType = ListItemType.AlternatingItem Then
                        Discount = DirectCast(innerItem.FindControl("compDiscTB"), TextBox).Text
                        ComponentId = DirectCast(innerItem.FindControl("hdnComponentId"), HiddenField).Value
                        ComponentGroupId = DirectCast(innerItem.FindControl("hdnComponentGroupId"), HiddenField).Value
                        Quantity = DirectCast(innerItem.FindControl("hdnQuantity"), HiddenField).Value
                        AmendComponent = New AmendComponent
                        AmendComponent.Quantity = Quantity
                        AmendComponent.ComponentGroupId = ComponentGroupId
                        AmendComponent.ComponentId = ComponentId
                        AmendComponent.Discount = IIf(String.IsNullOrEmpty(Discount), 0, Discount)
                        package.DePackages.AmendComponents.Add(AmendComponent)
                    End If

                Next
            End If
        Next

        Return package
    End Function

    Private Sub DisplayError(ByVal errMessage As String)
        Dim errorList As BulletedList = Me.Parent.FindControl("ErrorList")
        If Not errorList Is Nothing Then
            If errorList.Items.FindByText(errMessage) Is Nothing Then errorList.Items.Add(errMessage)
        End If
    End Sub

    Private Sub SetOutstandingTotal()
        If Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) Then
            If TEBUtilities.GetCurrentPageName(False).ToLower() <> "basket.aspx" Then
                plhPackageOutstandTotal.Visible = True
                lblPackageOutstandTotal.Text = GetFormattedPrice(Profile.Basket.BasketSummary.TotalTicketPrice)
                ltlOutstandTotal.Text = ucr.Content("OutstandTotalLabel", _languageCode, True)
            End If
        End If
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Formats the text for the component and its quantity.
    ''' </summary>
    ''' <param name="Quantity">No. of tickets.</param>
    ''' <param name="ComponentDescription">Component description</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetComponentText(ByVal Quantity As String, ByVal ComponentDescription As String) As String
        Return (String.Format("{0} X {1}", TCUtilities.CheckForDBNull_String(Quantity).ConvertAndTrimStringToInteger().ToString().Trim(), TCUtilities.CheckForDBNull_String(ComponentDescription)).Trim())
    End Function

    Public Function GetFormattedPrice(ByVal Price As String) As String
        Return TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Double.Parse(Price), 0.01, True), ucr.BusinessUnit, ucr.PartnerCode)
    End Function

#End Region

#Region "Protected Methods"
    Protected Sub btnUpdatePackage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdatePackage.Click
        Dim package As Talent.Common.TalentPackage = PreparePackageObjectForUpdateOperation()
        Dim err As ErrorObj
        err = package.UpdateCustomerComponentDetails()
        If err.HasError OrElse package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            "FailedUpdatingPackage").ERROR_MESSAGE
            DisplayError(errMessage)
        Else
            'Redirect back to this page so all of the values are refreshed i.e. mini basket.
            Response.Redirect(Request.RawUrl)
        End If
    End Sub

    ''' <summary>
    ''' Makes the control visible/invisible
    ''' </summary>
    ''' <returns>True only when its box office user</returns>
    ''' <remarks></remarks>
    Protected Function IsVisible() As Boolean
        Dim retValue As Boolean
        If _settings.IsAgent AndAlso Me.Mode = PackageSummaryMode.Edit Then
            retValue = True
        Else
            retValue = False
        End If
        Return retValue
    End Function

    ''' <summary>
    ''' Makes the control visible/invisible
    ''' </summary>
    ''' <returns>True only when its box office user</returns>
    ''' <remarks></remarks>
    Public Function IsMeVisible() As Boolean
        Dim retValue As Boolean
        If _settings.IsAgent Then
            retValue = True
        Else
            retValue = False
        End If
        Return retValue
    End Function


    Protected Sub btnShowBasket_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnShowBasket.Click
        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

    Protected Sub btnAmendBasket_Click(sender As Object, e As System.EventArgs) Handles btnAmendBasket.Click
        Response.Redirect(String.Format("~/PagesPublic/ProductBrowse/ComponentSelection.aspx?Product={0}&PackageId={1}&stage=1", ProductCode, PackageId))
    End Sub

    Protected Sub SetRegEx(ByVal sender As Object, ByVal e As EventArgs)
        Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
        Dim errorMessage = ucr.Content(regex.ID.Trim + "ErrMessage", _languageCode, True)
        Dim regExpres = ucr.Attribute(regex.ID.Trim, True)

        If Not errorMessage Is String.Empty Then
            regex.ErrorMessage = errorMessage
        End If
        If Not regExpres Is String.Empty Then
            regex.ValidationExpression = regExpres
        End If

    End Sub

    ''' <summary>
    ''' The discount drop down would be visible only to the box office user when there are any discounts available and also when the mode is edit.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function IsDiscountDropDownVisible() As Boolean
        Dim retValue As Boolean = False
        If _settings.IsAgent AndAlso Me.Mode = PackageSummaryMode.Edit And (AgentProfile.AgentPermissions.CanApplyTicketingPackageDiscounts) Then
            retValue = True
        End If
        Return retValue
    End Function

    ''' <summary>
    ''' The component value drop down would be visible when the sale was for a component priced package.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function IsTotalValueVisible() As Boolean
        Return IIf((From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                         Select r.Field(Of String)("PricingMethod")).FirstOrDefault() = GlobalConstants.PACKAGE_PRICING, False, True)
    End Function

    ''' <summary>
    ''' The View Basket button would be visible to the web customer only when the package status is complete and the control is in edit mode.
    ''' Also the update package should be visible only when the control is in edit mode.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub SetButtonVisiblity()
        Dim PackageCompleted = (From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                               Select r.Field(Of Boolean)("Completed")).FirstOrDefault()
        Dim PackageType = (From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                          Select r.Field(Of String)("packageType")).FirstOrDefault
        Dim allowComments = (From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                          Select r.Field(Of Boolean)("allowComments")).FirstOrDefault
        Dim packageID = (From r In dsCustomerPackageInformation.Tables("Package").AsEnumerable
                         Select r.Field(Of Long)("PackageID")).FirstOrDefault
        If PackageCompleted AndAlso _settings.IsAgent AndAlso Me.Mode = PackageSummaryMode.Edit Then
            btnShowBasket.Visible = True
        Else
            btnShowBasket.Visible = False
        End If

        If Me.Mode = PackageSummaryMode.Edit AndAlso IsDiscountDropDownVisible() Then
            btnUpdatePackage.Visible = True
        Else
            btnUpdatePackage.Visible = False
        End If

        If Me.Mode = PackageSummaryMode.Output Then
            btnDeletePackage.Visible = False
        Else
            btnDeletePackage.Visible = True
        End If

        If Me.Mode = PackageSummaryMode.Basket Then
            btnAmendBasket.Visible = True
        Else
            btnAmendBasket.Visible = False
        End If

        If PackageType <> "T" Then
            btnAmendBasket.Visible = False
            btnUpdatePackage.Visible = False
        End If


        If allowComments AndAlso Not Profile.IsAnonymous AndAlso Me.Mode = PackageSummaryMode.Basket Then
            hplComments.Visible = True
            ltlCommentsContent.Text = ucr.Content("AddCommentsLabel", _languageCode, True)
            hplComments.HRef = "~/PagesPublic/Basket/Comments.aspx" + "?PackageID=" + CStr(packageID) + "&Customer=" + Profile.User.Details.Account_No_1 + "&Product=" + Me.ProductCode
        Else
            hplComments.Visible = False
        End If

    End Sub

#End Region

    Protected Sub btnDeletePackage_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeletePackage.Click
        Dim package As Talent.Common.TalentPackage = PreparePackageObjectForUpdateOperation()
        package.DePackages.Mode = OperationMode.Delete
        Dim err As ErrorObj
        err = package.DeleteCustomerPackage()
        'Display the basket
        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

End Class

Public Enum PackageSummaryMode
    Output
    Basket
    Edit
End Enum