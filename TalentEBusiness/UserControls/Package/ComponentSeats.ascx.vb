Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic
Imports System.Globalization
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports System.Linq

Partial Class UserControls_Package_ComponentSeats
    Inherits ControlBase

#Region "Public Properties"
    Public Property ProductCode() As String = ""
    Public Property PackageId() As Long = 0
    Public Property ComponentId() As Long = 0
    Public Property Display() As Boolean = False
    Public Property Stage() As Integer = 0
    Public Property LastStage() As Boolean = False
#End Region

#Region "Class Level Fields"
    Private ucr As New Talent.Common.UserControlResource
    Private dicPriceCodeDetails As New Dictionary(Of String, String)
    Private dicPriceBandDetails As New Dictionary(Of String, String)
    Private dicValidPriceBands As New Dictionary(Of String, String)
    Private dicCustomers As New Dictionary(Of String, String)
    Private DtPackage As DataTable
    Private DtComponent As DataTable
    Private DtSeat As DataTable
    Private errMsg As Talent.Common.TalentErrorMessages
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private errorList As New BulletedList
    Private talPackage As New Talent.Common.TalentPackage
    Private Const KEYCODE As String = "ComponentSeats.ascx"
    Private _settings As DESettings
    Private err As ErrorObj
    Private stadiumCode As String = ""
    Private campaignCode As String = ""
    Private productType As String = ""
    Private restrictGraphical As Boolean = False
    Private productSubType As String = ""
    Private productIsHomeAsaway As String = ""
    Private _canHideSeatRemoveButton As Boolean = False
    Private previousStand As String = String.Empty
    Private previousArea As String = String.Empty
#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Display Then
            plhComponentSeats.Visible = True
            _settings = Talent.eCommerce.Utilities.GetSettingsObject()

            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = KEYCODE
            End With
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                            TalentCache.GetBusinessUnit, _
                                                            TalentCache.GetPartner(Profile), _
                                                            ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

            BindRepeaterAndControls()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorList.Items.Count > 0)
    End Sub

    Protected Function GetText(ByVal id As String) As String
        Dim textValue As String = ucr.Content(id, _languageCode, True)
        Return textValue
    End Function

    Protected Sub rptSeats_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptSeats.ItemCommand
        If e.CommandName = "DELETE" Then

            Dim seat As String() = e.CommandArgument.ToString.Split(",")
            If seat.Length = 2 Then

                'Delete the seat
                Dim catPackageDetail As String = CATHelper.GetFormattedCATPackageDetail()
                err = talPackage.UpdateCustomerComponentDetails(_settings, PrepareDEPackageObjectForUpdateOrDelete(OperationMode.Delete, seat(0), seat(1)))
                If err.HasError OrElse talpackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                    'TODO - Show error
                Else
                    ' We need to return to the seat selection process if we have removed the last seat
                    If talPackage.ResultDataSet.Tables("Seat").Rows.Count = 0 Then
                        Response.Redirect(setProductReturnUrl(catPackageDetail))
                    Else
                        'Redirect back to this page so all of the values are refreshed i.e. mini basket. The rawurl will redirect us back to this stage
                        Response.Redirect(Request.RawUrl)
                    End If
                End If
            End If

        End If

    End Sub

    Protected Sub rptSeats_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSeats.ItemDataBound
        Dim descriptionsFound As Boolean
        Dim seat As New DESeatDetails
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As RepeaterItem = e.Item
            Dim drv As System.Data.DataRowView = DirectCast((e.Item.DataItem), System.Data.DataRowView)
            seat.Stand = TEBUtilities.GetStandFromSeatDetails(drv.Row("SeatDetails").ToString())
            seat.Area = TEBUtilities.GetAreaFromSeatDetails(drv.Row("SeatDetails").ToString())
            Dim plhStandAreaDescriptions As PlaceHolder = CType(e.Item.FindControl("plhStandAreaDescriptions"), PlaceHolder)
            plhStandAreaDescriptions.Visible = False
            If seat.Stand <> previousStand OrElse seat.Area <> previousArea Then
                Dim ltlStandDescriptionLabel As Literal = CType(e.Item.FindControl("ltlStandDescriptionLabel"), Literal)
                Dim ltlAreaDescriptionLabel As Literal = CType(e.Item.FindControl("ltlAreaDescriptionLabel"), Literal)
                Dim ltlStandDescriptionValue As Literal = CType(e.Item.FindControl("ltlStandDescriptionValue"), Literal)
                Dim ltlAreaDescriptionValue As Literal = CType(e.Item.FindControl("ltlAreaDescriptionValue"), Literal)
                ltlStandDescriptionLabel.Text = ucr.Content("StandDescriptionLabel", _languageCode, True)
                ltlAreaDescriptionLabel.Text = ucr.Content("AreaDescriptionLabel", _languageCode, True)
                setStandAreaDescriptions(seat.Stand, seat.Area, ltlStandDescriptionValue.Text, ltlAreaDescriptionValue.Text, descriptionsFound)
                previousArea = seat.Area
                previousStand = seat.Stand
                plhStandAreaDescriptions.Visible = descriptionsFound
            End If
            Dim ddlcustomer As DropDownList = DirectCast(item.FindControl("ddlcustomer"), DropDownList),
                hidFFRegURL As HiddenField = CType(e.Item.FindControl("hidFFRegURL"), HiddenField),
                plhSeatRow As PlaceHolder = CType(e.Item.FindControl("plhSeatRow"), PlaceHolder),
                plhQuantityRow As PlaceHolder = CType(e.Item.FindControl("plhQuantityRow"), PlaceHolder),
                plhSeatDetailsRow As PlaceHolder = CType(e.Item.FindControl("plhSeatDetailsRow"), PlaceHolder)
            'Display quantity rather than seat in Bulk mode
            If AgentProfile.IsAgent AndAlso AgentProfile.BulkSalesMode Then
                plhSeatRow.Visible = False
                plhQuantityRow.Visible = True
                plhSeatDetailsRow.Visible = True
            Else
                plhSeatRow.Visible = True
                plhQuantityRow.Visible = False
                plhSeatDetailsRow.Visible = False
            End If
            'Bind Customer drop down list
            Dim strName As String = drv.Row("CustomerNumber").ToString()
            If Not Profile.IsAnonymous Then
                ddlcustomer.DataSource = dicCustomers
                ddlcustomer.DataTextField = "Value"
                ddlcustomer.DataValueField = "Key"
                If dicCustomers.ContainsKey(strName) Then ddlcustomer.SelectedValue = strName
                ddlcustomer.DataBind()
            Else
                CType(e.Item.FindControl("customerItemCol"), HtmlTableCell).Visible = False
            End If

            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("FriendsAndFamilyRegOption")) Then
                Dim friendsAndFamilyRegOptionText As String = String.Empty
                friendsAndFamilyRegOptionText = TEBUtilities.CheckForDBNull_String(ucr.Content("FriendsAndFamilyRegOptionText", _languageCode, True))
                If Not String.IsNullOrEmpty(friendsAndFamilyRegOptionText) Then
                    Dim listItem, tempListItem As New ListItem
                    listItem.Text = friendsAndFamilyRegOptionText
                    listItem.Value = ResolveUrl(TEBUtilities.CheckForDBNull_String(ucr.Attribute("FriendsAndFamilyRegUrl")))
                    tempListItem = ddlcustomer.Items.FindByValue(listItem.Value)
                    If tempListItem Is Nothing Then
                        ddlcustomer.Items.Add(listItem)
                    End If
                    hidFFRegURL.Value = listItem.Value
                    ddlcustomer.Attributes.Add("onchange", "RegFriendsAndFamily('" & ddlcustomer.ClientID & "','" & hidFFRegURL.ClientID & "')")
                End If
            End If

            'Bind price code drop down list
            If AgentProfile.IsAgent Then
                strName = drv.Row("PriceCode").ToString()
                Dim ddlPriceCode As DropDownList = DirectCast(item.FindControl("ddlPriceCode"), DropDownList)
                ddlPriceCode.DataSource = dicPriceCodeDetails
                ddlPriceCode.DataTextField = "Value"
                ddlPriceCode.DataValueField = "Key"
                If dicPriceCodeDetails.ContainsKey(strName) Then ddlPriceCode.SelectedValue = strName
                ddlPriceCode.DataBind()
            Else
                CType(e.Item.FindControl("priceCodeItemCol"), HtmlTableCell).Visible = False
            End If

            'Bind price band drop down list
            strName = drv.Row("PriceBand").ToString()
            Dim ddlPriceBand As DropDownList = DirectCast(item.FindControl("ddlPriceBand"), DropDownList)
            ddlPriceBand.DataSource = GetPriceBandsForSeat(drv.Row("ValidPriceBands").ToString())
            ddlPriceBand.DataTextField = "Value"
            ddlPriceBand.DataValueField = "Key"
            If dicValidPriceBands.ContainsKey(strName) Then ddlPriceBand.SelectedValue = strName
            ddlPriceBand.DataBind()

            If _canHideSeatRemoveButton AndAlso e.Item.FindControl("removeButtonItemCol") IsNot Nothing Then
                CType(e.Item.FindControl("removeButtonItemCol"), HtmlTableCell).Visible = False
                DirectCast(item.FindControl("btnRemove"), Button).Visible = False
            End If

        ElseIf e.Item.ItemType = ListItemType.Header Then

            Dim plhSeatHeader As PlaceHolder = CType(e.Item.FindControl("plhSeatHeader"), PlaceHolder),
                plhQuantityHeader As PlaceHolder = CType(e.Item.FindControl("plhQuantityHeader"), PlaceHolder),
                plhSeatDetailsHeader As PlaceHolder = CType(e.Item.FindControl("plhSeatDetailsHeader"), PlaceHolder)

            If Profile.IsAnonymous Then CType(e.Item.FindControl("customerHeaderCol"), HtmlTableCell).Visible = False
            If Not AgentProfile.IsAgent Then CType(e.Item.FindControl("priceCodeHeaderCol"), HtmlTableCell).Visible = False
            If _canHideSeatRemoveButton AndAlso e.Item.FindControl("removeButtonHeaderCol") IsNot Nothing Then CType(e.Item.FindControl("removeButtonHeaderCol"), HtmlTableCell).Visible = False
            'Display quantity rather than seat in Bulk mode
            If AgentProfile.IsAgent AndAlso AgentProfile.BulkSalesMode Then
                plhSeatHeader.Visible = False
                plhQuantityHeader.Visible = True
                plhSeatDetailsHeader.Visible = True
            Else
                plhSeatHeader.Visible = True
                plhQuantityHeader.Visible = False
                plhSeatDetailsHeader.Visible = False
            End If


        End If

    End Sub
#End Region

#Region "Private Methods"

    Private Sub DisplayError(ByVal errMessage As String)
        Dim errorList As BulletedList = Me.Parent.FindControl("ErrorList")
        If Not errorList Is Nothing Then
            If errorList.Items.FindByText(errMessage) Is Nothing Then errorList.Items.Add(errMessage)
        End If
    End Sub

    Private Sub BindRepeaterAndControls()
        'Retrieve the pacakage information from session
        err = talPackage.GetCustomerPackageInformation(_settings, Profile.Basket.Basket_Header_ID, PackageId, ProductCode)
        If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            "FailedGettingSeatDetails").ERROR_MESSAGE
            DisplayError(errMessage)
        Else


            'Retrieve pricing information
            GetProductDetails()


            'Retrieve customer information
            If Not Profile.IsAnonymous Then
                Dim cus As New Talent.Common.TalentCustomer
                Dim customer As New DECustomer
                If AgentProfile.IsAgent Then
                    customer.IncludeBoxOfficeLinks = True
                End If
                dicCustomers = cus.GetCustomerAssociationDictionary(Talent.eCommerce.Utilities.GetSettingsObject, Profile.UserName, Profile.UserName.TrimStart("0") & " - " & Profile.User.Details.Full_Name, customer)
            End If

            DtPackage = talPackage.ResultDataSet.Tables("Package")
            DtComponent = talPackage.ResultDataSet.Tables("Component")
            DtSeat = talPackage.ResultDataSet.Tables("Seat")
            If Not DtSeat.Columns.Contains("SeatText") Then
                DtSeat.Columns.Add("SeatText", GetType(String))
            End If



            For Each drSeat As Data.DataRow In DtSeat.Rows
                If drSeat("RovingOrUnreserved") = "R" Then
                    drSeat("SeatText") = ucr.Content("RovingAreaText", _languageCode, True)
                ElseIf drSeat("RovingOrUnreserved") = "U" Then
                    drSeat("SeatText") = ucr.Content("UnreservedAreaText", _languageCode, True)
                Else
                    drSeat("SeatText") = formatSeatDetails(drSeat("SeatDetails").ToString)
                End If
            Next

            ' We need to create a row filter as we only want seats for this component
            DtSeat.DefaultView.RowFilter = "ComponentId= " & ComponentId.ToString


            If Not String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) AndAlso Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_AMEND Then
                'only one seat and amend cat mode
                If DtSeat.DefaultView.Count = 1 Then
                    _canHideSeatRemoveButton = True
                End If
                btnAddMoreSeats.Visible = False
            End If

            rptSeats.DataSource = DtSeat.DefaultView
            rptSeats.DataBind()
            btnChangeSeats.Text = ucr.Content("btnChangeSeats", _languageCode, True)
            btnAddMoreSeats.Text = ucr.Content("btnAddMoreSeats", _languageCode, True)
            btnUpdateSeats.Text = ucr.Content("btnUpdateSeats", _languageCode, True)
            btnProceed.Text = ucr.Content("btnProceed", _languageCode, True)

            'Display Basket Errors
            DisplayBasketErrors()
        End If
    End Sub

    Private Sub DisplayBasketErrors()

        Dim ComponentDescription As String = (From r In DtComponent.AsEnumerable()
                                  Where r.Field(Of Long)("ComponentId") = ComponentId And r.Field(Of String)("Type") = "C"
                                  Select r.Field(Of String)("ComponentDescription")).FirstOrDefault()


        'Loop through the seats that are in error
        DtSeat.DefaultView.RowFilter = "ErrorCode <> ' ' and ComponentId= " & ComponentId.ToString
        For Each rowView As DataRowView In DtSeat.DefaultView

            'Populate the basket item
            Dim tbi As New TalentBasketItem
            tbi.MODULE_ = "TICKETING"
            tbi.PRODUCT_TYPE = "C"
            tbi.STOCK_ERROR_CODE = rowView.Row("ErrorCode")
            tbi.STOCK_LIMIT = rowView.Row("MaxLimit")
            tbi.STOCK_REQUESTED = rowView.Row("UserLimit")
            tbi.STOCK_ERROR_DESCRIPTION = rowView.Row("ErrorInformation")
            tbi.PRODUCT_DESCRIPTION1 = ComponentDescription
            tbi.LOGINID = rowView.Row("CustomerNumber")
            tbi.SEAT = rowView.Row("SeatDetails") & rowView.Row("AlphaSuffix")

            'Output the error message
            Dim errMessage As String = Talent.eCommerce.Utilities.TicketingBasketErrors(errMsg, tbi)
            If blErrorList.Items.FindByText(errMessage) Is Nothing Then
                plhErrorList.Visible = True
                blErrorList.Items.Add(errMessage)
            End If
        Next

    End Sub

    Protected Sub btnUpdateSeats_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateSeats.Click
        'Update the seat
        err = talPackage.UpdateCustomerComponentDetails(_settings, PrepareDEPackageObjectForUpdateOrDelete(OperationMode.Amend, "", ""))
        BindRepeaterAndControls()
        If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            "FailedSeatUpdate").ERROR_MESSAGE
            DisplayError(errMessage)
        Else
            'Redirect back to this page so all of the values are refreshed i.e. mini basket. The rawurl will redirect us back to this stage
            Response.Redirect(Request.RawUrl)
        End If
    End Sub

    Protected Sub btnProceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProceed.Click
        err = talPackage.UpdateCustomerComponentDetails(_settings, PrepareDEPackageObjectForUpdateOrDelete(OperationMode.Proceed, "", ""))
        If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            "FailedSeatUpdate").ERROR_MESSAGE
            DisplayError(errMessage)
        Else

            'Retrieve the pacakage information from session
            err = talPackage.GetCustomerPackageInformation(_settings, Profile.Basket.Basket_Header_ID, PackageId, ProductCode)
            If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                Dim errMessage As String = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                                Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                "FailedGettingSeatDetails").ERROR_MESSAGE
                DisplayError(errMessage)
            Else
                'Is this stage now complete
                Dim stageComplete As Boolean = (From r In talPackage.ResultDataSet.Tables("Component").AsEnumerable()
                    Where r.Field(Of Long)("ComponentId") = ComponentId And r.Field(Of Boolean)("Completed") = True).Any()

                'Redirect to the current page so we can display the relevant repeater
                If stageComplete Then
                    Response.Redirect(TalentPackage.RedirectUrl(TalentPackage.RedirectMode.Proceed, ProductCode, PackageId, LastStage, Stage, productSubType))
                Else
                    Response.Redirect(Request.RawUrl)
                End If

            End If
        End If

    End Sub

    Protected Sub btnChangeSeats_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnChangeSeats.Click
        Dim catPackageDetail As String = CATHelper.GetFormattedCATPackageDetail()
        talPackage.Settings = _settings
        talPackage.DePackages.PackageID = PackageId
        talPackage.DePackages.ProductCode = ProductCode
        talPackage.DePackages.BasketId = Profile.Basket.Basket_Header_ID
        talPackage.DePackages.BoxOfficeUser = AgentProfile.Name
        talPackage.DePackages.Mode = OperationMode.Amend
        talPackage.DePackages.MarkAsCompleted = True
        talPackage.DePackages.Mode = OperationMode.Delete
        Dim err As ErrorObj = talPackage.DeleteCustomerPackage()
        If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            'TODO: error handling
        Else
            Response.Redirect(setProductReturnUrl(catPackageDetail))
        End If
    End Sub
    Protected Sub btnAddMoreSeats_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddMoreSeats.Click
        Dim catPackageDetail As String = CATHelper.GetFormattedCATPackageDetail()
        Response.Redirect(setProductReturnUrl(catPackageDetail))
    End Sub

    Private Function PrepareDEPackageObjectForUpdateOrDelete(ByVal action As String, ByVal seatToDelete As String, ByVal alphaSuffix As String) As DEPackages

        Dim listAmendComponents As New List(Of AmendComponent)
        Dim listSeatAllocations As New List(Of SeatAllocation)

        'Add the component details
        listAmendComponents.Add(PopulateComponentDetails(action))

        'Populate seat details based on action type
        If action = OperationMode.Delete Then
            listSeatAllocations = PopulateSeatDetailsDelete(seatToDelete, alphaSuffix)
        ElseIf action = OperationMode.Amend OrElse action = OperationMode.Proceed Then
            listSeatAllocations = PopulateSeatDetailsAmend()
        End If


        Dim dePackage As New DEPackages
        dePackage.PackageID = PackageId
        dePackage.ProductCode = ProductCode
        dePackage.BasketId = Profile.Basket.Basket_Header_ID
        dePackage.BoxOfficeUser = AgentProfile.Name
        If action = OperationMode.StartAgain Then
            dePackage.Mode = OperationMode.Delete
        Else
            dePackage.Mode = OperationMode.Amend
        End If
        dePackage.ComponentGroupID = 0
        dePackage.ComponentID = ComponentId
        dePackage.AmendComponents = listAmendComponents
        dePackage.SeatAllocations = listSeatAllocations
        If action = OperationMode.Proceed Then dePackage.MarkAsCompleted = True

        Return dePackage
    End Function

    Private Function PopulateComponentDetails(ByVal action As String) As AmendComponent
        Dim amendComponent As New AmendComponent()
        'Populate the component details
        Dim componentDetails = (From r In DtComponent.AsEnumerable()
                               Where r.Field(Of Long)("ComponentId") = ComponentId
                               Select New With
                                        {
                                            .ComponentGroupId = r.Field(Of Long)("ComponentGroupId"),
                                            .Quantity = r.Field(Of String)("Quantity"),
                                            .Discount = r.Field(Of Decimal)("Discount")
                                        }).FirstOrDefault()
        amendComponent.ComponentGroupId = componentDetails.ComponentGroupId
        amendComponent.ComponentId = ComponentId
        amendComponent.Discount = componentDetails.Discount
        amendComponent.Quantity = componentDetails.Quantity
        If action = OperationMode.Delete Then amendComponent.Quantity -= 1
        Return amendComponent
    End Function

    Private Function PopulateSeatDetailsDelete(ByVal seatToDelete As String, ByVal alphaSuffix As String) As List(Of SeatAllocation)
        Dim listSeatAllocations As New List(Of SeatAllocation)
        Dim seatAllocation As New SeatAllocation()
        Dim seatDetails = (From r In DtSeat.AsEnumerable
                                  Where r.Field(Of String)("SeatDetails") = seatToDelete And r.Field(Of String)("AlphaSuffix") = alphaSuffix
                                  Select New With
                                        {
                                            .ProductCode = r.Field(Of String)("ProductCode"),
                                            .AlphaSuffix = r.Field(Of String)("AlphaSuffix"),
                                            .PriceBand = r.Field(Of String)("PriceBand"),
                                            .PriceCode = r.Field(Of String)("PriceCode"),
                                            .CustomerNumber = r.Field(Of String)("CustomerNumber"),
                                            .Seat = r.Field(Of String)("SeatDetails"),
                                            .BulkID = r.Field(Of Int64)("BulkID")
                                        }).FirstOrDefault()

        seatAllocation.ComponentId = ComponentId
        seatAllocation.ProductCode = seatDetails.ProductCode
        seatAllocation.Seat = seatDetails.Seat
        seatAllocation.AlphaSuffix = seatDetails.AlphaSuffix
        seatAllocation.PriceBand = seatDetails.PriceBand
        seatAllocation.PriceCode = seatDetails.PriceCode
        seatAllocation.BulkId = seatDetails.BulkID

        If Profile.IsAnonymous Then
            seatAllocation.CustomerNumber = 0
        Else
            seatAllocation.CustomerNumber = seatDetails.CustomerNumber
        End If

        seatAllocation.Action = OperationMode.Delete
        listSeatAllocations.Add(seatAllocation)
        Return listSeatAllocations
    End Function

    Private Function PopulateSeatDetailsAmend() As List(Of SeatAllocation)
        Dim listSeatAllocations As New List(Of SeatAllocation)
        Dim seatAllocation As SeatAllocation
        For Each item As RepeaterItem In rptSeats.Items
            If item.ItemType = ListItemType.Item OrElse item.ItemType = ListItemType.AlternatingItem Then
                seatAllocation = New SeatAllocation()
                seatAllocation.ComponentId = ComponentId
                seatAllocation.ProductCode = CType(item.FindControl("hdfProductCode"), HiddenField).Value
                seatAllocation.Seat = CType(item.FindControl("hdfSeat"), HiddenField).Value
                seatAllocation.AlphaSuffix = CType(item.FindControl("ltlAlpha"), Literal).Text
                seatAllocation.PriceBand = CType(item.FindControl("ddlPriceBand"), DropDownList).SelectedValue
                seatAllocation.BulkId = CType(item.FindControl("hdfBulkID"), HiddenField).Value
                If AgentProfile.IsAgent Then
                    seatAllocation.PriceCode = CType(item.FindControl("ddlPriceCode"), DropDownList).SelectedValue
                Else
                    seatAllocation.PriceCode = CType(item.FindControl("hdfPriceCode"), HiddenField).Value
                End If
                If Profile.IsAnonymous Then
                    seatAllocation.CustomerNumber = 0
                Else
                    seatAllocation.CustomerNumber = CType(item.FindControl("ddlcustomer"), DropDownList).SelectedValue
                End If
                seatAllocation.Action = OperationMode.Amend
                listSeatAllocations.Add(seatAllocation)
            End If
        Next
        Return listSeatAllocations
    End Function

    Private Function GetPriceBandsForSeat(ByVal ValidPriceBands As String) As IDictionary(Of String, String)
        Dim dicPriceBands As New Dictionary(Of String, String)

        For i As Integer = 0 To ValidPriceBands.Length - 1
            Dim item As String = ValidPriceBands.Substring(i, 1)
            If dicPriceBandDetails.ContainsKey(item) Then
                dicPriceBands.Add(item, dicPriceBandDetails(item))
            End If
        Next
        dicValidPriceBands = dicPriceBands

        Return dicPriceBands
    End Function

    Private Sub GetProductDetails()

        'Retrieve product details
        Dim _talentProduct As New TalentProduct
        Dim err As ErrorObj
        err = _talentProduct.ProductDetails(Talent.eCommerce.Utilities.GetSettingsObject, ProductCode)
        If Not err.HasError AndAlso _talentProduct.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" Then
            Dim dsProductDetails As DataSet = _talentProduct.ResultDataSet


            For Each dr As DataRow In dsProductDetails.Tables(1).Rows
                'TODO: Why are we getting duplicates
                If Not dicPriceBandDetails.ContainsKey(dr("PriceBand")) Then
                    dicPriceBandDetails.Add(dr("PriceBand"), dr("PriceBandDescription"))
                End If
            Next


            Dim dtPriceCodes As DataTable = Talent.eCommerce.Utilities.GetProductPriceCodesDescription(ProductCode, Talent.Common.Utilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0)("ProductType")).Trim(), Talent.Common.Utilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0)("ProductStadium")), dsProductDetails.Tables("PriceCodes"), dsProductDetails.Tables("CampaignCodes"))
            For Each dr As DataRow In dtPriceCodes.Rows
                'TODO: Why are we getting duplicates
                If Not dicPriceCodeDetails.ContainsKey(dr("PriceCode")) Then
                    dicPriceCodeDetails.Add(dr("PriceCode"), dr("PriceCodeDescription"))
                End If
            Next


            stadiumCode = dsProductDetails.Tables(2).Rows(0).Item("ProductStadium")
            productType = dsProductDetails.Tables(2).Rows(0).Item("ProductType")
            productSubType = dsProductDetails.Tables(2).Rows(0).Item("ProductSubType")
            productIsHomeAsaway = dsProductDetails.Tables(2).Rows(0).Item("HomeAsAway")
            restrictGraphical = dsProductDetails.Tables(2).Rows(0).Item("RestrictGraphical")
            'TODO: Need to work this out via the price code of the seats, but season tickets are not supported yet!
            campaignCode = ""

        End If

    End Sub

    Private Function setProductReturnUrl(ByVal catPackageDetail As String) As String
        Dim redirectUrl As String = String.Empty
        Dim stadiumName As String = TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(stadiumCode, ucr.BusinessUnit)
        If stadiumName.Length > 0 AndAlso Not restrictGraphical Then
            redirectUrl = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
        Else
            redirectUrl = "~/PagesPublic/ProductBrowse/StandAndAreaSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
        End If
        redirectUrl = String.Format(redirectUrl, stadiumCode, ProductCode, campaignCode, productType, productSubType, productIsHomeAsaway)
        Return ResolveUrl(CATHelper.GetCATPackageURLWithParam(redirectUrl, catPackageDetail))
    End Function

    ''' <summary>
    ''' Set the stand and area descriptions text if enabled
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setStandAreaDescriptions(stand As String, area As String, ByRef standDescription As String, ByRef areaDescription As String, ByRef Found As Boolean)
        Found = False
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowStandAreaDescriptions")) Then
            If DtSeat IsNot Nothing AndAlso DtSeat.Rows.Count > 0 Then
                If TEBUtilities.CheckForDBNull_String(DtSeat.Rows(0)("SeatDetails")).Length > 0 Then
                    Dim dtStandAreaDescriptions As DataTable = retrieveStandAndAreaDescriptions()
                    If Not dtStandAreaDescriptions Is Nothing Then
                        For Each row As DataRow In dtStandAreaDescriptions.Rows
                            If row("StandCode").Equals(stand) Then
                                If row("AreaCode").Equals(area) Then
                                    standDescription = row("StandDescription").ToString()
                                    areaDescription = row("AreaDescription").ToString()
                                    Found = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Call TALENT to retrieve the stand and area descriptions
    ''' </summary>
    ''' <remarks></remarks>
    Private Function retrieveStandAndAreaDescriptions() As DataTable
        Dim err As New ErrorObj
        Dim product As New TalentProduct
        Dim dtStandAreaDescriptions As DataTable = Nothing
        _settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _settings.BusinessUnit = TalentCache.GetBusinessUnit()
        _settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        _settings.Cacheing = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(ucr.Attribute("Cacheing"))
        _settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(ucr.Attribute("CacheTimeMinutes"))
        _settings.CacheTimeMinutesSecondFunction = TEBUtilities.CheckForDBNull_Int(ucr.Attribute("StandDescriptionsCacheTimeMinutes"))
        _settings.OriginatingSource = TEBUtilities.GetOriginatingSource(Session.Item("Agent"))
        product.Settings = _settings
        product.De.StadiumCode = stadiumCode
        product.ResultDataSet = Nothing
        err = product.StandDescriptions()
        If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing Then
            If product.ResultDataSet.Tables.Count > 1 Then
                If product.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    If String.IsNullOrWhiteSpace(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                        If product.ResultDataSet.Tables(1).Rows.Count > 0 Then
                            dtStandAreaDescriptions = product.ResultDataSet.Tables(1)
                        End If
                    End If
                End If
            End If
        End If
        Return dtStandAreaDescriptions
    End Function

    Private Function formatSeatDetails(ByVal seatDetails As String) As String
        Dim formattedSeat As String = GetText("SeatFormat")
        If formattedSeat = String.Empty Then
            formattedSeat = seatDetails
        Else
            Dim stand As String = seatDetails.Substring(0, 3)
            Dim area As String = seatDetails.Substring(3, 4)
            Dim row As String = seatDetails.Substring(7, 4)
            Dim seatN As String = seatDetails.Substring(11, 4).TrimStart("0"c)
            Dim seatSuffix As String = String.Empty
            If seatDetails.Length > 15 Then
                seatSuffix = seatDetails.Substring(15, 1)
            End If

            formattedSeat = formattedSeat.Replace("<<<Stand>>>", stand)
            formattedSeat = formattedSeat.Replace("<<<Area>>>", area)
            formattedSeat = formattedSeat.Replace("<<<Row>>>", row)
            formattedSeat = formattedSeat.Replace("<<<Seat>>>", seatN)
            formattedSeat = formattedSeat.Replace("<<<SeatSuffix>>>", seatSuffix)
        End If
        Return formattedSeat
    End Function

#End Region

#Region "Public Functions"
    Public Function GetSeatDetailsURL(basketID As String, bulkID As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append("~/PagesPublic/ProductBrowse/PackageSeatDetails.aspx?BasketID=")
        navigateUrl.Append(basketID)
        navigateUrl.Append("&BulkID=")
        navigateUrl.Append(bulkID)
        Return navigateUrl.ToString()
    End Function
#End Region

End Class