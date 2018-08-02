Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic
Imports System.Globalization
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports System.Linq
Imports TCUtilities = Talent.Common.Utilities

Partial Class UserControls_Package_TravelAndAccommodationGroup
    Inherits ControlBase

#Region "Child Class"
    Public Class ComponentChildHolder
        Public Property KeyCode As String = String.Empty
        Public Property ControlID As String = String.Empty
        Public Property KeyValue As String = String.Empty
        Public Property KeyType As String = String.Empty

        Sub New(ByVal Code As String, ByVal clientID As String, ByVal value As String, ByVal controlType As String)
            KeyCode = Code
            ControlID = clientID
            KeyValue = value
            KeyType = controlType
        End Sub
    End Class
#End Region

#Region "Private Fields"
    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private errorList As New BulletedList
    Private dsComponentGroupDetails As New DataSet
    Private ComponentId As Long
    Private dsComponentDetails As New DataSet
    Private dsCustomerPackageDetails As New DataSet
    Private dicComponentQuantity As New Dictionary(Of Long, Integer)
    Private Area2 As String = String.Empty
    Private updateMode As Boolean = False
    Private componentsAdded As Boolean = False
    Private groupInError As Boolean = False
    Private errorComponentid As Long = 0
    Private stageCompleted As Boolean = False
    Private _availableCarQuantity As Integer = 0
    Private _availableRoomQuantity As Integer = 0
    Private _productSubType As String = String.Empty
    Private _dicCompChildEntities As New Dictionary(Of String, ComponentChildHolder)

    Private agent As New Agent
#End Region

#Region "Constants"
    Const KEYCODE As String = "TravelAndAccommodationGroup.ascx"
    Const IMAGETEXT As String = "_Image"
    Const RADIOBUTTONTEXT As String = "_RadioButtonText"
    Const RADIOBUTTONID As String = "rdoComponent"
    Const RADIOBUTTONGROUPID As String = "rdogrpComponent"
    Const LABELID As String = "lblComponent"
    Const HIDDENID As String = "hdnComponentId"
#End Region

#Region "Public Fields"
    Public errMsg As Talent.Common.TalentErrorMessages
    Public Property ComponentGroupId() As Long
    Public Property AvailableComponentQuantity() As Integer
    Public Property Display() As Boolean = False
    Public Property ProductCode() As String = ""
    Public Property TicketingProductCode() As String = ""
    Public Property PackageId() As Long = 0
    Public Property Stage() As Integer = 0
    Public Property LastStage() As Boolean = False

#End Region

#Region "Protected Methods and Handlers"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With


        'This user control is active when this hidden field is set.  We need to bind the controls values
        'again because viewstate is disabled.  This hidden field will be set when the user changes the 
        'component radio button or sumbits an add request i.e. a postback
        If Not String.IsNullOrWhiteSpace(Request.Params(hdnSelectedComponentId.UniqueID)) AndAlso
                CType(Request.Params(hdnSelectedComponentId.UniqueID).ToString, Long) > 0 Then

            'Populate the properties with the hidden field values.  
            ComponentId = CType(Request.Params(hdnSelectedComponentId.UniqueID).ToString, Long)
            ProductCode = Request.Params(hdnProductCode.UniqueID).ToString
            TicketingProductCode = Request.Params(hdnTicketingProductCode.UniqueID).ToString
            ComponentGroupId = CType(Request.Params(hdnComponentGroupId.UniqueID).ToString, Long)
            AvailableComponentQuantity = CType(Request.Params(hdnAvailableComponentQuantity.UniqueID).ToString, Integer)
            PackageId = CType(Request.Params(hdnPackageId.UniqueID).ToString, Long)
            Stage = CType(Request.Params(hdnStage.UniqueID).ToString, Integer)
            LastStage = CType(Request.Params(hdnLastStage.UniqueID).ToString, Boolean)

            'Retrieve the customer package details if we haven't already done so
            GetCustomerPackageDetails()

            'Repopulate the control field
            ControlsLoad()
            ControlsPopulateValues()
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Display Then
            plhTravelAndAccommodation.Visible = True

            'We need to set the availability component to a default value when selling travel and accommodation
            'options on there own.
            If AvailableComponentQuantity = 0 Then
                AvailableComponentQuantity = ucr.Attribute("DefaultAvailabilityQuantity")
            End If

            'Retrieve the customer package details if we haven't already done so
            If dsCustomerPackageDetails Is Nothing OrElse dsCustomerPackageDetails.Tables.Count < 5 Then
                GetCustomerPackageDetails()
            End If

            'We only need to perform this action when the component id is not set.  This will happen when 
            ' the user control is loaded for the initial time.  We cannot place this in page_init with the
            ' viewstate work, because the properties are only set in page_load.
            If ComponentId = 0 Then
                ControlsLoad()
                If Not IsDisplayedSystemError(True) Then
                    ControlsPopulateValues()
                End If
            End If

            'Display any validation errors that relate to this travel and accommodation group
            DisplayLocalErrors()

            'Display any system errors
            IsDisplayedSystemError(False)

        Else
            plhTravelAndAccommodation.Visible = False
        End If

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If dsComponentDetails IsNot Nothing AndAlso dsComponentDetails.Tables("Stand") IsNot Nothing Then
            Dim dtStand As DataTable = dsComponentDetails.Tables("Stand")
            For rowIndex As Integer = 0 To dtStand.Rows.Count - 1
                _dicCompChildEntities.Add(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")) & "!" & divLblExtra1.ClientID, New ComponentChildHolder(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")), divLblExtra1.ClientID, TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtStand.Rows(rowIndex)("Extra1Available")).ToString, "DIV"))
                _dicCompChildEntities.Add(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")) & "!" & divLblExtra2.ClientID, New ComponentChildHolder(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")), divLblExtra2.ClientID, TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtStand.Rows(rowIndex)("Extra2Available")).ToString, "DIV"))
                _dicCompChildEntities.Add(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")) & "!" & divLblExtra3.ClientID, New ComponentChildHolder(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")), divLblExtra3.ClientID, TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtStand.Rows(rowIndex)("Extra3Available")).ToString, "DIV"))
                _dicCompChildEntities.Add(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")) & "!" & divLblExtra4.ClientID, New ComponentChildHolder(TEBUtilities.CheckForDBNull_String(dtStand.Rows(rowIndex)("StandCode")), divLblExtra4.ClientID, TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtStand.Rows(rowIndex)("Extra4Available")).ToString, "DIV"))
            Next
        End If
        Dim i As Integer = _dicCompChildEntities.Count
        If _dicCompChildEntities.Count > 0 Then
            Dim sbComponentChild As New StringBuilder
            sbComponentChild.AppendLine("<script language=""javascript"" type=""text/javascript"">")
            sbComponentChild.AppendLine("var componentChilds = [];")
            For Each key As String In _dicCompChildEntities.Keys
                sbComponentChild.AppendLine("componentChilds.push({ keyCode: '" & _dicCompChildEntities(key).KeyCode & "', controlID: '" & _dicCompChildEntities(key).ControlID & "', keyValue: '" & _dicCompChildEntities(key).KeyValue & "', keyType: '" & _dicCompChildEntities(key).KeyType & "' });")
            Next
            sbComponentChild.AppendLine("$(document).ready(function(){ assignChildControlValues('" & ddlDestination.ClientID & "'); });")
            sbComponentChild.AppendLine("</script>")
            ltlComponentChildControlValues.Text = sbComponentChild.ToString()
            ddlDestination.Attributes.Add("onchange", "assignChildControlValues('" & ddlDestination.ClientID & "')")
        End If
        If ddlDestination.Items.Count < 2 Then
            ddlDestination.Visible = False
        End If
    End Sub

    Protected Sub rdoComponent_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)

        Dim NewComponentId As Long = DirectCast(sender, RadioButton).ID.ToString().GetLongNumberFromString()
        CheckedChanged(NewComponentId)

    End Sub

    Protected Sub rptPriceBands_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPriceBands.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim item As RepeaterItem = e.Item
            Dim ddl As DropDownList = DirectCast(item.FindControl("ddlPriceBand"), DropDownList)
            Dim tb As TextBox = DirectCast(item.FindControl("tbPriceBand"), TextBox)
            If AgentProfile.BulkSalesMode Then
                ddl.Visible = False
                tb.Visible = True
                PopulateTheSelectedItem(Area2, tb, item.DataItem("PriceBand"))
            Else
                BindDropDown(ddl, 0, AvailableComponentQuantity, "", 0, 0, 0, "", False, True)
                PopulateTheSelectedItem(Area2, ddl, item.DataItem("PriceBand"))
            End If

        End If
    End Sub

    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        If Not String.IsNullOrWhiteSpace(hdnSelectedComponentId.Value) Then
            ComponentId = hdnSelectedComponentId.Value
            AddTravelAndAccommodationComponent()
        Else
            'TODO: error
        End If
    End Sub

    Protected Sub btnStartAgain_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnStartAgain.Click
        ComponentId = 0
        DeleteTravelAndAccomodationComponent()
    End Sub

    Protected Sub btnProceed_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProceed.Click

        'Update the complete flag on all components
        Dim package As New Talent.Common.TalentPackage
        Dim err As ErrorObj
        package.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        package.DePackages.PackageID = PackageId
        package.DePackages.ComponentID = ComponentId
        package.DePackages.ProductCode = ProductCode
        package.DePackages.TicketingProductCode = TicketingProductCode
        package.DePackages.BasketId = Profile.Basket.Basket_Header_ID
        package.DePackages.BoxOfficeUser = AgentProfile.Name
        package.DePackages.Proceed = True
        package.DePackages.ComponentGroupID = ComponentGroupId
        err = package.UpdateTravelAndAccomodationComponentDetails()

        If err.HasError OrElse package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            'Populate the session variables and redirect to the same stage.  This will reinit the form and select the no ticket  
            'radio button and stop the user resubmitting the same request via and F5.
            Dim errorCode As String = ""
            If Not err.HasError Then errorCode = package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")
            Session("TravelAndAccommodation_ErrorCode") = errorCode
            Session("TravelAndAccommodation_GenericErrorCode") = "FailedProceeding"
        Else

            'Navigate to the correct place
            GetCustomerPackageDetails()
            If stageCompleted Then
                'Move on
                Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                Dim redirectUrl As String = String.Empty
                Dim priceCode As String = String.Empty
                Dim productSubType As String = String.Empty
                If Request.QueryString("pricecode") IsNot Nothing Then priceCode = Request.QueryString("pricecode")
                If Request.QueryString("productsubtype") IsNot Nothing Then productSubType = Request.QueryString("productsubtype")
                redirectUrl = TalentPackage.RedirectUrl(TalentPackage.RedirectMode.Proceed, TicketingProductCode, PackageId, LastStage, Stage, productSubType)
                If ticketingGatewayFunctions.DoesProductHaveRelatedProducts(TicketingProductCode, New DataTable, priceCode, productSubType) Then
                    ticketingGatewayFunctions.ProductHasRelatedProducts = True
                    redirectUrl = ticketingGatewayFunctions.HandleRedirect(redirectUrl, True, TicketingProductCode, priceCode, productSubType, False, False, 0)
                End If
                Response.Redirect(redirectUrl)
            Else
                'Display the same stage
                Response.Redirect(Request.RawUrl)
            End If

        End If
    End Sub

    Protected Sub SetRegEx(ByVal sender As Object, ByVal e As EventArgs)
        Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))

        Dim errorMessage = ucr.Content(regex.ID.Trim + "ErrMessage", _languageCode, True)
        Dim regExpres = ucr.Attribute(regex.ID.Trim, True)

        If Not errorMessage Is String.Empty Then
            regex.ErrorMessage = errorMessage
        End If
        If regExpres Is String.Empty Then
            regExpres = "^(0|[1-9][0-9]*)$"
        End If
        regex.ValidationExpression = regExpres

    End Sub

#End Region

#Region "Private Methods"

    Private Sub CheckedChanged(ByVal newComponentid As Long)

        If newComponentid <> ComponentId AndAlso errorComponentid = 0 Then
            ComponentId = newComponentid
            SetHiddenFields()

            'Populate the drop downs
            PopulateTheDropDowns()
        Else
            'We always need to display the component in error.  We therefore need to deselect the newly selected item
            'and select the error one.
            SetHiddenFields()
            If errorComponentid > 0 Then
                Dim rbtncomponent As RadioButton = Me.FindControl(RADIOBUTTONID & newComponentid.ToString().Trim())
                rbtncomponent.Checked = False
                Dim rbtncomponent2 As RadioButton = Me.FindControl(RADIOBUTTONID & errorComponentid.ToString().Trim())
                rbtncomponent2.Checked = True
            End If
        End If

        ButtonVisibility()
    End Sub

    Private Sub SetDates(ByVal dtDate As DataTable, ByVal DisplayDates As Boolean, ByVal fromDate As Date, ByVal toDate As Date)
        If DisplayDates Then
            plhDates.Visible = True

            dtDate.DefaultView.Sort = "ProductDate"
            Dim culture As New System.Globalization.CultureInfo(ModuleDefaults.Culture)

            ' Load the from Dates
            Dim dicFromValues As New Dictionary(Of Date, String)
            For Each rowView As DataRowView In dtDate.DefaultView
                If rowView.Row("ProductDate") <= fromDate Then
                    If Not dicFromValues.ContainsKey(rowView.Row("ProductDate")) Then
                        Dim productDate As Date = CType(rowView.Row("ProductDate"), Date)
                        dicFromValues.Add(rowView.Row("ProductDate"), productDate.ToString("dddd dd MMMM yyyy", culture))
                    End If
                End If
            Next

            'Populate the drop down list
            ddlFromDate.DataSource = dicFromValues
            ddlFromDate.DataTextField = "Value"
            ddlFromDate.DataValueField = "Key"
            ddlFromDate.DataBind()

            ' Load the To Dates
            Dim dicToValues As New Dictionary(Of Date, String)
            For Each rowView As DataRowView In dtDate.DefaultView
                If rowView.Row("ProductDate") >= toDate Then
                    Dim tempDate As Date = CType(rowView.Row("ProductDate"), Date).AddDays(1)
                    If Not dicToValues.ContainsKey(tempDate) Then
                        dicToValues.Add(tempDate, tempDate.ToString("dddd dd MMMM yyyy", culture))
                    End If
                End If
            Next

            'Populate the drop down list
            ddlToDate.DataSource = dicToValues
            ddlToDate.DataTextField = "Value"
            ddlToDate.DataValueField = "Key"
            ddlToDate.DataBind()

            'Populate the selected item
            PopulateTheDateSelectedItem(fromDate, toDate)
        Else
            plhDates.Visible = False
            hdnFromProductDate.Value = dtDate.Rows(0)("ProductDate")
            If dtDate.Rows.Count > 1 Then
                hdnToProductdate.Value = dtDate.Rows(1)("ProductDate")
            Else
                hdnToProductdate.Value = dtDate.Rows(0)("ProductDate")
            End If
        End If

    End Sub

    Private Sub PopulateTheDateSelectedItem(ByVal FromDate As Date, ByVal ToDate As Date)

        Dim userSelectedDateExists As Boolean = False
        If dsCustomerPackageDetails IsNot Nothing AndAlso dsCustomerPackageDetails.Tables.Count >= 5 Then
            Dim dtComponent As DataTable = dsCustomerPackageDetails.Tables("Component")
            dtComponent.DefaultView.RowFilter = "ComponentId = " & ComponentId.ToString & " And Type = 'C'"
            For Each rowView As DataRowView In dtComponent.DefaultView
                FromDate = rowView.Row("FromDate")
                ToDate = CType(rowView.Row("ToDate"), Date).AddDays(1)
                userSelectedDateExists = True
                Exit For
            Next
        End If
        'Populate the ddl if the value exists in the drop down, otherwise default to the first
        ddlFromDate.SelectedIndex = ddlFromDate.Items.IndexOf(ddlFromDate.Items.FindByValue(FromDate.ToString))
        ddlToDate.SelectedIndex = ddlToDate.Items.IndexOf(ddlToDate.Items.FindByValue(ToDate.ToString))
        If userSelectedDateExists AndAlso ddlDestination.Items IsNot Nothing AndAlso ddlDestination.Items.Count > 1 Then
            ddlFromDate.Visible = False
            ddlToDate.Visible = False
            lblFromDateValue.Text = ddlFromDate.SelectedItem.Text
            lblToDateValue.Text = ddlToDate.SelectedItem.Text
            lblFromDateValue.Visible = True
            lblToDateValue.Visible = True
        Else
            ddlFromDate.Visible = True
            ddlToDate.Visible = True
            lblFromDateValue.Visible = False
            lblToDateValue.Visible = False
        End If
    End Sub

    Private Sub ControlsLoad()
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        SetText()
        GetComponentGroupDetails()
        CreateImageControls()
        plhButtons.Visible = True
    End Sub

    Private Sub ControlsPopulateValues()
        PopulateTheDropDowns()
        'Set button visibility
        ButtonVisibility()
        If Request.QueryString("productsubtype") IsNot Nothing Then _productSubType = Request.QueryString("productsubtype")
    End Sub

    Private Sub ButtonVisibility()

        'The last stage proceed text should say basket
        If LastStage Then
            btnProceed.Text = ucr.Content("btnProceed_BasketText", _languageCode, True)
        End If

        'Back button is not visible when it's the first stage
        If Stage = 1 Then
            btnBack.Visible = False
        Else
            btnBack.PostBackUrl = TalentPackage.RedirectUrl(TalentPackage.RedirectMode.Back, TicketingProductCode, PackageId, LastStage, Stage, _productSubType)
        End If

        If groupInError Then
            btnProceed.Visible = False
        Else
            btnProceed.Visible = True
        End If

        If Not componentsAdded Then
            btnStartAgain.Visible = False
        Else
            btnStartAgain.Visible = True
        End If

        If ComponentId = 0 Then
            btnAdd.Visible = False
            btnProceed.Visible = True
        Else
            btnAdd.Visible = True
            btnProceed.Visible = False
        End If

    End Sub

    Private Sub SetText()
        btnBack.Text = ucr.Content("btnBackText", _languageCode, True)
        btnAdd.Text = ucr.Content("btnAdd_AddText", _languageCode, True)
        btnStartAgain.Text = ucr.Content("btnStartAgainText", _languageCode, True)
        btnProceed.Text = ucr.Content("btnProceed_ProceedText", _languageCode, True)
    End Sub

    Private Sub PopulateTheDropDowns()

        If ComponentId > 0 Then

            plhComponentDetails.Visible = True
            Dim package As New Talent.Common.TalentPackage
            GetControlTextFromSQLDatabase(ComponentId)
            GetComponentDetails()

            ddlDestination.DataSource = package.GetStands(dsComponentDetails.Tables("Stand"))
            ddlDestination.DataTextField = "Value"
            ddlDestination.DataValueField = "Key"
            ddlDestination.DataBind()

            If ddlDestination.Items.Count = 1 Then
                ddlDestination.Visible = False
                ltlDestinationValue.Visible = True
                ltlDestinationValue.Text = ddlDestination.SelectedItem.Text
            Else
                ddlDestination.Visible = True
                ltlDestinationValue.Visible = False
            End If

            'Select Areacode of the second row returned by ISeries
            Area2 = (From k In (From r In dsComponentDetails.Tables("Area").AsEnumerable
                    Select r).Skip(1)
                    Select k.Field(Of String)("AreaCode")).FirstOrDefault()

            Dim dv As New DataView(dsComponentDetails.Tables("PriceBand"))
            dv.RowFilter = "AreaCode='" & Area2 & "'"

            BindRepeater(dv.ToTable())   'Bind the Price band Repeater which load the values in the repeater dropdowns.
            ShowHideAreas(dsComponentDetails.Tables("Area"))       'Load values in the other dropdowns.

            'Display the date fields
            Dim displayDates As Boolean = False
            Dim fromDate As Date
            Dim ToDate As Date
            If dsComponentDetails.Tables("StatusResults") IsNot Nothing AndAlso
                dsComponentDetails.Tables("StatusResults").Rows.Count > 0 Then
                displayDates = dsComponentDetails.Tables("StatusResults").Rows(0).Item("DisplayDateFields")
                If displayDates Then
                    fromDate = dsComponentDetails.Tables("StatusResults").Rows(0).Item("TicketingFromDate")
                    ToDate = dsComponentDetails.Tables("StatusResults").Rows(0).Item("TicketingToDate")
                End If
            End If
            SetDates(dsComponentDetails.Tables("Date"), displayDates, fromDate, ToDate)
        Else
            plhComponentDetails.Visible = False
        End If
    End Sub

    Private Sub GetComponentGroupDetails()
        Dim err As ErrorObj
        Dim package As New Talent.Common.TalentPackage
        Dim dePackage As New DEPackages
        dePackage.ComponentGroupID = ComponentGroupId
        If agent.IsAgent Then
            dePackage.AvailableToSell03 = agent.IsAvailableToSell03
            dePackage.AvailableToSellAvailableTickets = agent.SellAvailableTickets
        Else
            dePackage.AvailableToSell03 = True
            dePackage.AvailableToSellAvailableTickets = False
        End If
        err = package.GetComponentGroupDetails(Talent.eCommerce.Utilities.GetSettingsObject, dePackage, dsComponentGroupDetails)
        If err.HasError OrElse dsComponentGroupDetails.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errorCode As String = ""
            If Not err.HasError Then errorCode = dsComponentGroupDetails.Tables("StatusResults").Rows(0).Item("ReturnCode")
            DisplayError(errorCode, "FailedGettingComponentGroupDetails")
        End If

    End Sub

    Private Sub GetComponentDetails()
        Dim dePackage As New DEPackages
        Dim err As ErrorObj
        Dim package As New Talent.Common.TalentPackage
        dePackage.ComponentGroupID = ComponentGroupId
        dePackage.ComponentID = ComponentId
        dePackage.ProductCode = ProductCode
        dePackage.IsCATBasket = Profile.Basket.CAT_MODE.Trim.Length > 0
        If agent.IsAgent Then
            dePackage.AvailableToSell03 = agent.IsAvailableToSell03
            dePackage.AvailableToSellAvailableTickets = agent.SellAvailableTickets
        Else
            dePackage.AvailableToSell03 = True
            dePackage.AvailableToSellAvailableTickets = False
        End If

        err = package.GetComponentDetails(Talent.eCommerce.Utilities.GetSettingsObject, dePackage, dsComponentDetails)
        If err.HasError OrElse dsComponentDetails.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errorCode As String = ""
            If Not err.HasError Then errorCode = dsComponentDetails.Tables("StatusResults").Rows(0).Item("ReturnCode")
            Session("TravelAndAccommodation_ErrorCode") = errorCode
            Session("TravelAndAccommodation_GenericErrorCode") = "FailedGettingComponentDetails"

            'Display the page again but with none selected.
            hdnSelectedComponentId.Value = 0
            Response.Redirect(Request.RawUrl)
        End If


    End Sub

    Private Sub GetCustomerPackageDetails()

        componentsAdded = False
        groupInError = False
        errorComponentid = 0
        stageCompleted = False
        dicComponentQuantity.Clear()

        'Retrieve the package information from session
        Dim talPackage As New Talent.Common.TalentPackage
        Dim err As ErrorObj = talPackage.GetCustomerPackageInformation(Talent.eCommerce.Utilities.GetSettingsObject, Profile.Basket.Basket_Header_ID, PackageId, TicketingProductCode)
        If err.HasError OrElse talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            Dim errorCode As String = ""
            If Not err.HasError Then errorCode = talPackage.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")
            DisplayError(errorCode, "FailedGettingPackageDetails")
        Else
            dsCustomerPackageDetails = talPackage.ResultDataSet


            Dim dtComponent As DataTable = dsCustomerPackageDetails.Tables("Component")
            dtComponent.DefaultView.RowFilter = "ComponentGroupId = " & ComponentGroupId.ToString
            For Each rowView As DataRowView In dtComponent.DefaultView
                Dim dr As DataRow = rowView.Row

                If dr("Type") = "C" Then componentsAdded = True
                If dr("ErrorCode") <> "" AndAlso dr("ErrorCode") <> "RW" Then groupInError = True
                If dr("ErrorCode") <> "" AndAlso dr("ErrorCode") <> "RW" AndAlso dr("Type") = "C" AndAlso errorComponentid = 0 Then errorComponentid = dr("ComponentId")
                If dr("Type") = "G" AndAlso dr("Completed") Then stageCompleted = True
                If dr("Type") = "C" Then dicComponentQuantity.Add(dr("ComponentId"), CType(dr("Quantity"), Integer))
            Next
        End If
    End Sub

    Private Sub DisplayError(ByVal errorCode As String, ByVal genericErrorCode As String)
        'Specific error message
        Dim errMessage As String = ""
        If Not String.IsNullOrWhiteSpace(errorCode) Then
            errMessage = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            errorCode, False).ERROR_MESSAGE
        End If

        'General error message
        If String.IsNullOrWhiteSpace(errMessage) Then
            errMessage = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                            genericErrorCode).ERROR_MESSAGE
        End If
        plhErrorList.Visible = True
        blErrorList.Items.Add(errMessage)
    End Sub

    Private Sub DisplayLocalErrors()

        If dsCustomerPackageDetails IsNot Nothing AndAlso dsCustomerPackageDetails.Tables.Count >= 5 Then
            Dim errComponentid As Long = 0
            Dim lessThanMessage As String = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                "LessThanMessage", False).ERROR_MESSAGE
            Dim rangeMessage As String = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                "RangeMessage", False).ERROR_MESSAGE
            Dim equalMessage As String = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                "EqualMessage", False).ERROR_MESSAGE
            Dim notEligible As String = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                "NotEligible", False).ERROR_MESSAGE

            Dim dtComponent As DataTable = dsCustomerPackageDetails.Tables("Component")
            dtComponent.DefaultView.RowFilter = "ErrorCode <> '' And ComponentGroupId = " & ComponentGroupId.ToString
            For Each rowView As DataRowView In dtComponent.DefaultView
                Dim dr As DataRow = rowView.Row
                Dim errMessage As String = String.Empty

                'We currently have two types of errors for TA: -
                ' 1) An RE error is rasied when the user selects more TA compnenets than they are allowed to.  The user cannot
                'proceed with these errors and they must remove them.  A message will be displayed
                ' 2) An RW error is a warning that the user has not yet selected enough components to satisy the rules.
                ' We will never display a message for an RW error as the user can proceed through these warnings.
                If dr("ErrorCode") = "RE" AndAlso Not String.IsNullOrWhiteSpace(dr("AreaInError")) Then
                    If dr("Type") = "G" Then
                        errMessage = RetrieveErrorMessage("G", dr("ComponentGroupId"), 0, dr("AreaInError"))
                    ElseIf dr("Type") = "C" AndAlso (errorComponentid = dr("ComponentId")) Then
                        errMessage = RetrieveErrorMessage("T", dr("ComponentGroupId"), 0, dr("AreaInError"))
                    End If
                End If

                If Not String.IsNullOrWhiteSpace(errMessage) Then

                    'Replace the ratio text which depends on the min and max fields
                    If dr("MinQty") = 0 AndAlso dr("MaxQty") = 0 Then
                        errMessage = errMessage.Replace("<<<ratio_message>>>", notEligible)
                    ElseIf dr("MinQty") = 0 AndAlso dr("MaxQty") > 0 Then
                        errMessage = errMessage.Replace("<<<ratio_message>>>", lessThanMessage)
                    ElseIf dr("MinQty") = dr("MaxQty") Then
                        errMessage = errMessage.Replace("<<<ratio_message>>>", equalMessage)
                    Else
                        errMessage = errMessage.Replace("<<<ratio_message>>>", rangeMessage)
                    End If

                    'Replace the reserved words
                    errMessage = errMessage.Replace("<<<component_description>>>", dr("ComponentDescription"))
                    errMessage = errMessage.Replace("<<<min_limit>>>", dr("MinQty"))
                    errMessage = errMessage.Replace("<<<max_limit>>>", dr("MaxQty"))
                    errMessage = errMessage.Replace("<<<err_msg>>>", dr("ErrorDescription"))

                    'Replace the current value message
                    Dim currentValue As String = ""
                    Dim startPos As Integer = dr("ErrorDescription").ToString.IndexOf("CurrVal=")
                    Dim endPos As Integer = 0
                    If startPos >= 0 Then
                        endPos = dr("ErrorDescription").ToString.IndexOf(";", startPos)
                        startPos += 8
                        If endPos > 0 AndAlso (endPos - startPos) > 0 Then currentValue = dr("ErrorDescription").ToString.Substring(startPos, endPos - startPos)
                    End If
                    errMessage = errMessage.Replace("<<<current_limit>>>", currentValue)

                    'Soft code the areas for better quality error messages.
                    If dr("ComponentGroupId") > 0 AndAlso dr("ComponentId") Then
                        errMessage = errMessage.Replace("<<<destination_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "DestinationText", _languageCode, True))
                        errMessage = errMessage.Replace("<<<cars_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "CarsText", _languageCode, True))
                        errMessage = errMessage.Replace("<<<roomtype1_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "RoomType1Text", _languageCode, True))
                        errMessage = errMessage.Replace("<<<roomtype2_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "RoomType2Text", _languageCode, True))
                        errMessage = errMessage.Replace("<<<extra1_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "Extra1Text", _languageCode, True))
                        errMessage = errMessage.Replace("<<<extra2_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "Extra2Text", _languageCode, True))
                        errMessage = errMessage.Replace("<<<extra3_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "Extra3Text", _languageCode, True))
                        errMessage = errMessage.Replace("<<<extra4_text>>>", ucr.Content(dr("ComponentGroupId") & "_" & dr("ComponentId") & "_" & "Extra4Text", _languageCode, True))
                    End If

                    If blErrorList.Items.FindByText(errMessage) Is Nothing Then
                        plhErrorList.Visible = True
                        blErrorList.Items.Add(errMessage)
                    End If

                End If
            Next
        End If
    End Sub

    Private Function IsDisplayedSystemError(ByVal hideBTNandDDLVisibility As Boolean) As Boolean
        Dim isDisplayed As Boolean = False
        'Display any system errors
        plhButtons.Visible = True
        If Session("TravelAndAccommodation_ErrorCode") IsNot Nothing AndAlso
             Session("TravelAndAccommodation_GenericErrorCode") IsNot Nothing AndAlso
             Not String.IsNullOrWhiteSpace(Session("TravelAndAccommodation_GenericErrorCode")) Then
            DisplayError(Session("TravelAndAccommodation_ErrorCode"), Session("TravelAndAccommodation_GenericErrorCode"))
            Session.Remove("TravelAndAccommodation_ErrorCode")
            Session.Remove("TravelAndAccommodation_GenericErrorCode")
            isDisplayed = True
            If hideBTNandDDLVisibility Then
                plhComponentDetails.Visible = False
                plhButtons.Visible = False
            End If
        End If
        Return isDisplayed
    End Function

    Private Function RetrieveErrorMessage(ByVal Type As String, ByVal ComponetGroupId As Long, ByVal ComponentId As Long, ByVal ErrorCode As String) As String

        Dim errMessage As String = String.Empty
        Dim typeMsg As String
        If Type = "G" Then
            typeMsg = "Group"
        Else
            typeMsg = "Component"
        End If

        'Retrieve the general error message when no group type message exists
        If String.IsNullOrWhiteSpace(errMessage) Then
            errMessage = errMsg.GetErrorMessage("TravelAndAccommodationGroup", _
                                 Talent.eCommerce.Utilities.GetCurrentPageName, _
                                       typeMsg & "_" & ErrorCode).ERROR_MESSAGE
        End If

        Return errMessage

    End Function

    Private Sub SetHiddenFields()
        hdnSelectedComponentId.Value = ComponentID
        hdnComponentGroupId.Value = ComponentGroupId
        hdnProductCode.Value = ProductCode
        hdnAvailableComponentQuantity.Value = AvailableComponentQuantity
        hdnTicketingProductCode.Value = TicketingProductCode
        hdnPackageId.Value = PackageId
        hdnStage.Value = Stage
        hdnLastStage.Value = LastStage
    End Sub

    Private Sub GetControlTextFromSQLDatabase(ByVal ComponentId As Long)
        lblDestination.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "DestinationText", _languageCode, True)
        lblFromDate.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "ArrivalDateText", _languageCode, True)
        lblToDate.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "DepartureDateText", _languageCode, True)
        lblCars.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "CarsText", _languageCode, True)
        lblRoomType1.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "RoomType1Text", _languageCode, True)
        lblRoomType2.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "RoomType2Text", _languageCode, True)
        lblExtra1.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "Extra1Text", _languageCode, True)
        lblExtra2.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "Extra2Text", _languageCode, True)
        lblExtra3.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "Extra3Text", _languageCode, True)
        lblExtra4.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "Extra4Text", _languageCode, True)
        ltlComponentText.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "GroupText", _languageCode, True)
        'btnMoreInfo.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "MoreInfoButtonText", _languageCode, True)
        hplMoreInfo.Text = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & "MoreInfoButtonText", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Shows/Hides the drop down Area control for which details are passed.
    ''' </summary>
    ''' <param name="AreaCode"></param>
    ''' <param name="FromQuantity"></param>
    ''' <param name="ToQuantity"></param>
    ''' <param name="ddl"></param>
    ''' <param name="lbl"></param>
    ''' <remarks></remarks>
    Private Sub ShowHideArea(ByVal AreaCode As String, ByVal FromQuantity As Integer, ByVal ToQuantity As Integer, ByVal DefaultType As String, ByRef ddl As DropDownList, ByRef lbl As Label, ByVal isCar As Boolean, ByVal isRoom As Boolean, ByVal availableQuantity As Integer, ByVal rulesNotMetFor As String)
        If Not String.IsNullOrEmpty(AreaCode) Then
            If FromQuantity = 0 Then FromQuantity = 1
            Dim setupMinQty As Integer = FromQuantity
            Dim setupMaxQty As Integer = ToQuantity
            Dim isRulesNotMet As Boolean = False
            If (TCUtilities.CheckForDBNull_Int(FromQuantity) > 0) AndAlso (TCUtilities.CheckForDBNull_Int(ToQuantity) > 0) Then
                Dim minQty As Integer = 0
                Dim maxQty As Integer = 0
                If DefaultType = "G" Then
                    If availableQuantity >= FromQuantity Then
                        minQty = 1

                        ' When the two and from quantities are equal no rounding can occur.  This is because a 
                        ' rule of 2 to 2 means the get an extra unit when they have 2,4,6,8.  If rounding occurs
                        ' they would get the unit every 2,3,5,7
                        If FromQuantity = ToQuantity Then
                            maxQty = Math.Floor(availableQuantity / ToQuantity)
                        Else
                            'Rounding is correct for ranges though.  As a 2 - 4 rule means they get an extra
                            ' unit at 2,5,9 etc
                            maxQty = Math.Ceiling(availableQuantity / ToQuantity)
                        End If
                    Else
                        isRulesNotMet = True
                    End If
                ElseIf DefaultType = "T" AndAlso (isCar OrElse isRoom) Then
                    'Fixed validation for car and rooms
                    If availableQuantity >= FromQuantity Then
                        minQty = 1
                        maxQty = availableQuantity
                        ' The max quantity is the ticket qty / min people allowed in a room or car
                        ' No rounding can occur as it uses the minumum.
                        maxQty = Math.Floor(availableQuantity / FromQuantity)
                    Else
                        isRulesNotMet = True
                    End If
                ElseIf DefaultType = "T" Then
                    minQty = availableQuantity * FromQuantity
                    maxQty = availableQuantity * ToQuantity
                    If maxQty <= 0 Then isRulesNotMet = True
                End If
                BindDropDown(ddl, minQty, maxQty, DefaultType, availableQuantity, setupMinQty, setupMaxQty, rulesNotMetFor, isRulesNotMet, False)
                If isCar Then
                    _availableCarQuantity = maxQty
                ElseIf isRoom Then
                    _availableRoomQuantity = maxQty
                End If
            Else
                BindDropDown(ddl, 0, availableQuantity, DefaultType, availableQuantity, setupMinQty, setupMaxQty, rulesNotMetFor, isRulesNotMet, False)
            End If
            If Not isRulesNotMet Then
                MakeVisible(lbl, ddl)
            Else
                lbl.Visible = True
            End If

            PopulateTheSelectedItem(AreaCode, ddl)
        Else
            MakeInVisible(lbl, ddl)
        End If
    End Sub

    ''' <summary>
    ''' Shows/Hides the drop down Area control for which details are passed. Bulk Mode
    ''' </summary>
    ''' <param name="AreaCode"></param>
    ''' <param name="tb"></param>
    ''' <param name="lbl"></param>
    ''' <remarks></remarks>
    Private Sub ShowHideAreaBulk(ByVal AreaCode As String, ByRef tb As TextBox, ByRef lbl As Label)
        If Not String.IsNullOrEmpty(AreaCode) Then
            MakeVisible(lbl, tb)
            PopulateTheSelectedItem(AreaCode, tb)
        Else
            MakeInVisible(lbl, tb)
        End If
    End Sub

    Private Sub BindDropDown(ByRef ddl As DropDownList, ByVal MinDropDownValue As Integer,
                             ByVal MaxDropDownValue As Integer,
                             ByVal componentValidationType As String,
                             ByVal availableQuantity As Integer,
                             ByVal setupMinQty As Integer,
                             ByVal setupMaxQty As Integer,
                             ByVal rulesNotMetFor As String,
                             ByVal isRulesNotMet As Boolean,
                             ByVal isItPriceBandDDL As Boolean)

        Dim dicDropDownValues As New Dictionary(Of String, String)
        If MinDropDownValue > 0 Then dicDropDownValues.Add(0, 0)
        For i As Integer = MinDropDownValue To MaxDropDownValue
            dicDropDownValues.Add(i, i)
        Next
        ddl.DataSource = dicDropDownValues
        ddl.DataTextField = "Value"
        ddl.DataValueField = "Key"
        ddl.DataBind()

        If Not isItPriceBandDDL Then
            If ddl.Items.Count = 1 Then
                If ddl.Items(0).Value = "0" Then
                    'there is no option to select so hide this ddl and show rule not met error
                    SetRulesNotMetMessage(ddl, componentValidationType, availableQuantity, setupMinQty, setupMaxQty, rulesNotMetFor)
                End If
            End If
        End If
        If ddl.Visible Then
            Select Case rulesNotMetFor
                Case "CARS"
                    plhRNM_Car.Visible = False
                Case "ROOMTYPE1"
                    plhRNM_RoomType1.Visible = False
                Case "ROOMTYPE2"
                    plhRNM_RoomType2.Visible = False
                Case "EXTRA1"
                    plhRNM_Extra1.Visible = False
                Case "EXTRA2"
                    plhRNM_Extra2.Visible = False
                Case "EXTRA3"
                    plhRNM_Extra3.Visible = False
                Case "EXTRA4"
                    plhRNM_Extra4.Visible = False
            End Select
        End If
    End Sub

    Private Sub SetRulesNotMetMessage(ByVal ddl As DropDownList,
                             ByVal componentValidationType As String,
                             ByVal availableQuantity As Integer,
                             ByVal setupMinQty As Integer,
                             ByVal setupMaxQty As Integer,
                             ByVal rulesNotMetFor As String)
        ddl.Visible = False

        Dim errorBrief As String = String.Empty
        Dim errorDetail As String = String.Empty
        Dim errorMessage As String = String.Empty
        Dim errorMessageFound As Boolean = False
        If componentValidationType = "G" AndAlso availableQuantity < setupMinQty Then
            errorBrief = "RNM_Range_Brief" & "_" & rulesNotMetFor
            errorDetail = "RNM_Range_Detail" & "_" & rulesNotMetFor
        ElseIf componentValidationType = "T" AndAlso availableQuantity < setupMinQty Then
            errorBrief = "RNM_LessThan_Brief" & "_" & rulesNotMetFor
            errorDetail = "RNM_LessThan_Detail" & "_" & rulesNotMetFor
        End If
        TryGetRulesNotMessage(errorBrief, errorBrief)
        errorMessageFound = TryGetRulesNotMessage(errorDetail, errorDetail)

        If errorMessageFound Then
            errorDetail = errorDetail.Replace("<<<min_qty>>>", setupMinQty.ToString)
            errorDetail = errorDetail.Replace("<<<max_qty>>>", setupMaxQty.ToString)
            errorDetail = errorDetail.Replace("<<<available_qty>>>", availableQuantity.ToString)

            errorDetail = errorDetail.Replace("<<<cars_text>>>", lblCars.Text)
            errorDetail = errorDetail.Replace("<<<roomtype1_text>>>", lblRoomType1.Text)
            errorDetail = errorDetail.Replace("<<<roomtype2_text>>>", lblRoomType2.Text)
            errorDetail = errorDetail.Replace("<<<extra1_text>>>", lblExtra1.Text)
            errorDetail = errorDetail.Replace("<<<extra2_text>>>", lblExtra2.Text)
            errorDetail = errorDetail.Replace("<<<extra3_text>>>", lblExtra3.Text)
            errorDetail = errorDetail.Replace("<<<extra4_text>>>", lblExtra4.Text)
        Else
            errorDetail = String.Empty
        End If

        Select Case rulesNotMetFor
            Case "CARS"
                plhRNM_Car.Visible = True
                ltlRNM_Brief_Car.Text = errorBrief
                ltlRNM_Detail_Car.Text = errorDetail
            Case "ROOMTYPE1"
                plhRNM_RoomType1.Visible = True
                ltlRNM_Brief_RoomType1.Text = errorBrief
                ltlRNM_Detail_RoomType1.Text = errorDetail
            Case "ROOMTYPE2"
                plhRNM_RoomType2.Visible = True
                ltlRNM_Brief_RoomType2.Text = errorBrief
                ltlRNM_Detail_RoomType2.Text = errorDetail
            Case "EXTRA1"
                plhRNM_Extra1.Visible = True
                ltlRNM_Brief_Extra1.Text = errorBrief
                ltlRNM_Detail_Extra1.Text = errorDetail
            Case "EXTRA2"
                plhRNM_Extra2.Visible = True
                ltlRNM_Brief_Extra2.Text = errorBrief
                ltlRNM_Detail_Extra2.Text = errorDetail
            Case "EXTRA3"
                plhRNM_Extra3.Visible = True
                ltlRNM_Brief_Extra3.Text = errorBrief
                ltlRNM_Detail_Extra3.Text = errorDetail
            Case "EXTRA4"
                plhRNM_Extra4.Visible = True
                ltlRNM_Brief_Extra4.Text = errorBrief
                ltlRNM_Detail_Extra4.Text = errorDetail
        End Select
    End Sub

    Private Function TryGetRulesNotMessage(ByVal textCode As String, ByRef rulesNotMessage As String) As Boolean
        If Not String.IsNullOrWhiteSpace(textCode) Then
            If TEBUtilities.CheckForDBNull_String(ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & textCode, _languageCode, True)).Trim.Length > 0 Then
                rulesNotMessage = ucr.Content(ComponentGroupId & "_" & ComponentId & "_" & textCode, _languageCode, True).Trim
            Else
                rulesNotMessage = TEBUtilities.CheckForDBNull_String(ucr.Content(textCode, _languageCode, True))
            End If
        End If
        If Not String.IsNullOrWhiteSpace(rulesNotMessage) Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary>
    ''' Populate the selected item with what is currently in the basket
    ''' </summary>
    ''' <param name="AreaCode">Area being processed.</param>
    ''' <param name="wc">WebComponent to select quantity</param>
    Private Sub PopulateTheSelectedItem(ByVal AreaCode As String, ByRef wc As WebControl, Optional ByVal PriceBand As String = "")

        If dsCustomerPackageDetails IsNot Nothing AndAlso dsCustomerPackageDetails.Tables.Count >= 5 Then

            Dim standAndArea As String = Talent.Common.Utilities.FixStringLength(ddlDestination.SelectedValue, 3) & Talent.Common.Utilities.FixStringLength(AreaCode, 4)

            'Retrieve the selected value for this drop down list
            Dim value As Integer = 0
            Dim dtSeats As DataTable = dsCustomerPackageDetails.Tables("Seat")
            For Each dr As DataRow In dtSeats.Rows
                If dr("ComponentId") = ComponentId Then
                    If dr("SeatDetails").ToString.Substring(0, 7) = standAndArea Then
                        If String.IsNullOrWhiteSpace(PriceBand) OrElse PriceBand.Trim = dr("PriceBand") Then
                            'Bulk Sales mode returns summarised entries for T&A selections
                            If AgentProfile.BulkSalesMode Then
                                value += dr("BulkQuantity")
                            Else
                                value += 1
                            End If
                        End If
                    End If
                End If
            Next

            If TypeOf wc Is DropDownList Then
                'Populate the ddl if the value exists in the drop down, otherwise default to the first
                Dim ddl As DropDownList = CType(wc, DropDownList)
                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(value))
            ElseIf TypeOf wc Is TextBox Then
                CType(wc, TextBox).Text = value
            End If

            If value > 0 AndAlso Not (updateMode) Then
                btnAdd.Text = ucr.Content("btnAdd_UpdateText", _languageCode, True)
                updateMode = True
            End If
            SetChildControlValuesInClient(AreaCode, wc, PriceBand)
        End If

    End Sub

    ''' <summary>
    ''' Shows/Hides the Area controls.
    ''' </summary>
    ''' <param name="dtArea">Area table.</param>
    ''' <remarks>Shows/Hides the Specific Areas. If the AreaCode is empty, the controls will not be shown.</remarks>
    Private Sub ShowHideAreas(ByVal dtArea As DataTable)

        ' If we are in bulk mode, then the drop down lists for quantities need to be replaced with text boxes for input of larger numbers.

        If AgentProfile.BulkSalesMode Then
            ddlCars.Visible = False
            ddlRoomType1.Visible = False
            ddlRoomType2.Visible = False
            ddlExtra1.Visible = False
            ddlExtra2.Visible = False
            ddlExtra3.Visible = False
            ddlExtra4.Visible = False
            ShowHideAreaBulk(dtArea(0)("AreaCode"), tbCars, lblCars)
            ShowHideAreaBulk(dtArea(4)("AreaCode"), tbRoomType1, lblRoomType1)
            ShowHideAreaBulk(dtArea(5)("AreaCode"), tbRoomType2, lblRoomType2)
            ShowHideAreaBulk(dtArea(6)("AreaCode"), tbExtra1, lblExtra1)
            ShowHideAreaBulk(dtArea(7)("AreaCode"), tbExtra2, lblExtra2)
            ShowHideAreaBulk(dtArea(8)("AreaCode"), tbExtra3, lblExtra3)
            ShowHideAreaBulk(dtArea(9)("AreaCode"), tbExtra4, lblExtra4)
        Else
            plhRNM_Car.Visible = False
            plhRNM_RoomType1.Visible = False
            plhRNM_RoomType2.Visible = False
            plhRNM_Extra1.Visible = False
            plhRNM_Extra2.Visible = False
            plhRNM_Extra3.Visible = False
            plhRNM_Extra4.Visible = False
            ShowHideArea(dtArea(0)("AreaCode"), dtArea(0)("FromQuantity"), dtArea(0)("ToQuantity"), dtArea(0)("DefaultType"), ddlCars, lblCars, True, False, AvailableComponentQuantity, "CARS")
            ShowHideArea(dtArea(4)("AreaCode"), dtArea(4)("FromQuantity"), dtArea(4)("ToQuantity"), dtArea(4)("DefaultType"), ddlRoomType1, lblRoomType1, False, True, AvailableComponentQuantity, "ROOMTYPE1")
            'This is not a mistake: The second room option has the same validation as the first room option so use row number 4 and not 5.
            ShowHideArea(dtArea(5)("AreaCode"), dtArea(4)("FromQuantity"), dtArea(4)("ToQuantity"), dtArea(4)("DefaultType"), ddlRoomType2, lblRoomType2, False, True, AvailableComponentQuantity, "ROOMTYPE2")
            ShowHideArea(dtArea(6)("AreaCode"), dtArea(6)("FromQuantity"), dtArea(6)("ToQuantity"), dtArea(6)("DefaultType"), ddlExtra1, lblExtra1, False, False, AvailableComponentQuantity, "EXTRA1")
            ShowHideArea(dtArea(7)("AreaCode"), dtArea(7)("FromQuantity"), dtArea(7)("ToQuantity"), dtArea(7)("DefaultType"), ddlExtra2, lblExtra2, False, False, AvailableComponentQuantity, "EXTRA2")
            ShowHideArea(dtArea(8)("AreaCode"), dtArea(8)("FromQuantity"), dtArea(8)("ToQuantity"), dtArea(8)("DefaultType"), ddlExtra3, lblExtra3, False, False, _availableCarQuantity, "EXTRA3")
            ShowHideArea(dtArea(9)("AreaCode"), dtArea(9)("FromQuantity"), dtArea(9)("ToQuantity"), dtArea(9)("DefaultType"), ddlExtra4, lblExtra4, False, False, _availableRoomQuantity, "EXTRA4")
        End If
    End Sub

    Private Sub MakeVisible(ByRef lbl As Label, ByRef wc As WebControl)
        lbl.Visible = True
        wc.Visible = True
    End Sub

    Private Sub MakeInVisible(ByRef lbl As Label, ByRef wc As WebControl)
        lbl.Visible = False
        wc.Visible = False
    End Sub

    Private Sub BindRepeater(ByVal dtPriceBands As DataTable)
        If dtPriceBands.Rows.Count > 0 Then
            rptPriceBands.DataSource = dtPriceBands
            rptPriceBands.DataBind()
            plhPriceBands.Visible = True
        Else
            plhPriceBands.Visible = False
        End If
    End Sub

    Private Sub DeleteTravelAndAccomodationComponent()
        Dim package As New Talent.Common.TalentPackage
        Dim err As ErrorObj
        package.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        package.DePackages.PackageID = PackageId
        package.DePackages.ProductCode = ProductCode
        package.DePackages.TicketingProductCode = TicketingProductCode
        package.DePackages.BasketId = Profile.Basket.Basket_Header_ID
        package.DePackages.BoxOfficeUser = AgentProfile.Name
        package.DePackages.Mode = OperationMode.Delete
        package.DePackages.ComponentGroupID = ComponentGroupId
        package.DePackages.ComponentID = ComponentId
        package.DePackages.MarkAsCompleted = False
        err = package.UpdateCustomerComponentDetails()
        If err.HasError OrElse package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            'Populate the session variables and redirect to the same stage.  This will reinit the form and select the no ticket  
            'radio button and stop the user resubmitting the same request via and F5.
            Dim errorCode As String = ""
            If Not err.HasError Then errorCode = package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")
            Session("TravelAndAccommodation_ErrorCode") = errorCode
            Session("TravelAndAccommodation_GenericErrorCode") = "FailedDeletingThisStage"
        End If

        'Redirect to the same page. This repeater will be displayed again as it is not yet completed.
        Response.Redirect(Request.RawUrl)

    End Sub

    Private Sub AddTravelAndAccommodationComponent()

        Dim package As New Talent.Common.TalentPackage
        Dim err As ErrorObj
        package.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        package.DePackages.PackageID = PackageId
        package.DePackages.ComponentID = ComponentId
        package.DePackages.ProductCode = ProductCode
        package.DePackages.TicketingProductCode = TicketingProductCode
        package.DePackages.BasketId = Profile.Basket.Basket_Header_ID
        package.DePackages.BoxOfficeUser = AgentProfile.Name
        If Not String.IsNullOrWhiteSpace(hdnFromProductDate.Value) Then
            package.DePackages.Fromdate = CType(hdnFromProductDate.Value, Date)
            package.DePackages.ToDate = CType(hdnToProductdate.Value, Date)
        Else
            package.DePackages.Fromdate = CType(ddlFromDate.SelectedValue, Date)
            package.DePackages.ToDate = CType(ddlToDate.SelectedValue, Date).AddDays(-1)
        End If
        package.DePackages.StandCode = ddlDestination.SelectedValue
        If agent.IsAgent Then
            package.DePackages.AvailableToSell03 = agent.IsAvailableToSell03
            package.DePackages.AvailableToSellAvailableTickets = agent.SellAvailableTickets
        Else
            package.DePackages.AvailableToSell03 = True
            package.DePackages.AvailableToSellAvailableTickets = False
        End If

        '********Prepare the Component Areas*****************
        If AgentProfile.BulkSalesMode Then
            AddOtherComponentAreaToDEPackageObject(tbCars, package, 0)
            AddOtherComponentAreaToDEPackageObject(tbRoomType1, package, 4)
            AddOtherComponentAreaToDEPackageObject(tbRoomType2, package, 5)
            AddOtherComponentAreaToDEPackageObject(tbExtra1, package, 6)
            AddOtherComponentAreaToDEPackageObject(tbExtra2, package, 7)
            AddOtherComponentAreaToDEPackageObject(tbExtra3, package, 8)
            AddOtherComponentAreaToDEPackageObject(tbExtra4, package, 9)
        Else
            AddOtherComponentAreaToDEPackageObject(ddlCars, package, 0)
            AddOtherComponentAreaToDEPackageObject(ddlRoomType1, package, 4)
            AddOtherComponentAreaToDEPackageObject(ddlRoomType2, package, 5)
            AddOtherComponentAreaToDEPackageObject(ddlExtra1, package, 6)
            AddOtherComponentAreaToDEPackageObject(ddlExtra2, package, 7)
            AddOtherComponentAreaToDEPackageObject(ddlExtra3, package, 8)
            AddOtherComponentAreaToDEPackageObject(ddlExtra4, package, 9)
        End If


        '**********************Now prepare the People Area*****************************
        Dim ComponentArea2 As New TAComponentArea2
        Dim ddlPriceBand As New DropDownList
        Dim tbPriceBand As New TextBox
        Dim hdfPriceBand As New HiddenField
        Dim PBAndQuantity As PriceBandAndQuantity

        ComponentArea2.Area = (From k In (From r In dsComponentDetails.Tables("Area").AsEnumerable
                        Select r).Skip(1)
                        Select k.Field(Of String)("AreaCode")).FirstOrDefault()

        For Each item In rptPriceBands.Items
            PBAndQuantity = New PriceBandAndQuantity()

            ddlPriceBand = item.FindControl("ddlPriceBand")
            tbPriceBand = item.FindControl("tbPriceBand")
            hdfPriceBand = item.FindControl("hdfPriceBand")

            PBAndQuantity.PriceBand = hdfPriceBand.Value
            If ddlPriceBand.Visible Then
                PBAndQuantity.Quantity = ddlPriceBand.SelectedValue
            ElseIf tbPriceBand.Visible Then
                PBAndQuantity.Quantity = tbPriceBand.Text
            End If
            ComponentArea2.PriceBandAndQuantities.Add(PBAndQuantity)
        Next

        package.DePackages.TAComponentArea2 = ComponentArea2
        '**********************************************

        '**************Call the ISeries Program to Add the component****************
        package.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        package.Settings.Cacheing = False
        package.DePackages.MarkAsCompleted = True
        package.DePackages.ComponentGroupID = ComponentGroupId
        err = package.UpdateTravelAndAccomodationComponentDetails()
        '**********************************************

        If err.HasError OrElse package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            'Populate the session variables and redirect to the same stage.  This will reinit the form and select the no ticket  
            'radio button and stop the user resubmitting the same request via and F5.
            Dim errorCode As String = ""
            If Not err.HasError Then errorCode = package.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")
            Session("TravelAndAccommodation_ErrorCode") = errorCode
            Session("TravelAndAccommodation_GenericErrorCode") = "FailedAddingAComponent"

            'Clear the cahe when there is a no seat error
            'TODO: Check that NC is being returned properly and then remove the NS from this statement
            If errorCode = "NS" OrElse errorCode = "NC" Then
                package.RemoveComponentDetailsCache()
            End If

            Response.Redirect(Request.RawUrl)
        Else
            'Navigate to the correct place
            GetCustomerPackageDetails()
            If stageCompleted Then
                'Move on
                Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                Dim redirectUrl As String = String.Empty
                Dim priceCode As String = String.Empty
                Dim productSubType As String = String.Empty
                If Request.QueryString("pricecode") IsNot Nothing Then priceCode = Request.QueryString("pricecode")
                If Request.QueryString("productsubtype") IsNot Nothing Then productSubType = Request.QueryString("productsubtype")
                redirectUrl = TalentPackage.RedirectUrl(TalentPackage.RedirectMode.Proceed, TicketingProductCode, PackageId, LastStage, Stage, _productSubType)
                If ticketingGatewayFunctions.DoesProductHaveRelatedProducts(TicketingProductCode, New DataTable, priceCode, productSubType) Then
                    ticketingGatewayFunctions.ProductHasRelatedProducts = True
                    redirectUrl = ticketingGatewayFunctions.HandleRedirect(redirectUrl, True, TicketingProductCode, priceCode, productSubType, False, False, 0)
                End If
                Response.Redirect(redirectUrl)
            Else
                'Display the same stage
                Response.Redirect(Request.RawUrl)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Gets the Area details against the index passed and adds it to the package object
    ''' </summary>
    ''' <param name="ddl"></param>
    ''' <param name="package"></param>
    ''' <param name="IndexInRow">The row index in the Area table to which the drop down control maps.</param>
    ''' <remarks></remarks>
    Private Sub AddOtherComponentAreaToDEPackageObject(ByRef wc As WebControl, ByRef package As Talent.Common.TalentPackage, ByVal IndexInRow As Integer)
        If wc.Visible Then
            Dim OtherComponentArea As New OtherTAComponentArea
            OtherComponentArea.Area = (From k In (From r In dsComponentDetails.Tables("Area").AsEnumerable
                                    Select r).Skip(IndexInRow)
                                    Select k.Field(Of String)("AreaCode")).FirstOrDefault()

            OtherComponentArea.PriceBandAndQuantity.PriceBand = (From r In dsComponentDetails.Tables("PriceBand")
                                                                Where r.Field(Of String)("AreaCode") = OtherComponentArea.Area
                                                               Select r.Field(Of String)("PriceBand")).FirstOrDefault()

            If TypeOf wc Is DropDownList Then
                OtherComponentArea.PriceBandAndQuantity.Quantity = CType(wc, DropDownList).SelectedValue
            ElseIf TypeOf wc Is TextBox Then
                OtherComponentArea.PriceBandAndQuantity.Quantity = CType(wc, TextBox).Text
            End If

            package.DePackages.OtherTAComponentAreas.Add(OtherComponentArea)
        End If

    End Sub

    ''' <summary>
    ''' Creating the radio buttons from code behind.
    ''' </summary>
    ''' <remarks>Due to a microsoft bug, the radiobuttons inside a repeater cannot belong to a single group. So creating it through a loop from code behind.</remarks>
    Private Sub CreateImageControls()
        Dim itemText As String

        ulComponents.Controls.Clear()
        Dim noneReqOpt As Integer = TEBUtilities.CheckForDBNull_Int(dsComponentGroupDetails.Tables("ComponentGroupDetails").Rows(0).Item("NoneReqOpt"))

        Dim dvComponentGroupDetails = (From r In dsComponentGroupDetails.Tables("ComponentDetails").AsEnumerable
                                       Order By r.Field(Of Integer)("ComponentSequence") Ascending
                                       Select r)

        ' Test
        ' addRadioButtonItem(1234, "Test", True, ucr.Attribute("NoMoreRequiredImage", True))

        ' The 'No-More should always appear first if the user has added a component.
        If componentsAdded Then
            addRadioButtonItem(0, ucr.Content("lblComponentNoMoreRequired", _languageCode, True), True, ucr.Attribute("NoMoreRequiredImage", True))
        End If

        ' Determine the position of the non-required option (First, last or not displayed)
        If noneReqOpt = NoneReqOptMode.first And Not componentsAdded Then
            addRadioButtonItem(0, ucr.Content("lblComponentNoneRequired", _languageCode, True), True, ucr.Attribute("NoneRequiredImage", True))
        End If

        'Loop Components for this group and add to the radio button list.
        For Each r As DataRow In dvComponentGroupDetails
            itemText = GetValueFromControlTable(r("ComponentId").ToString().Trim())
            If dicComponentQuantity.ContainsKey(r("ComponentId")) Then
                itemText = itemText.Trim & " (" & dicComponentQuantity.Item(r("ComponentId")) & ")"
            End If

            addRadioButtonItem(r("ComponentId"), itemText, TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(r("IsAvailable")), GetValueFromAttributeTable(r("ComponentId").ToString().Trim()))
        Next

        If noneReqOpt = NoneReqOptMode.last And Not componentsAdded Then
            addRadioButtonItem(0, ucr.Content("lblComponentNoneRequired", _languageCode, True), True, ucr.Attribute("NoneRequiredImage", True))
        End If

    End Sub

    Private Sub addRadioButtonItem(itemComponentId As Long, itemComponentText As String, itemIsAvailable As Boolean, imageURL As String)
        Dim li As HtmlGenericControl
        Dim anchor1 As HtmlGenericControl
        Dim image As Image
        Dim h2 As HtmlGenericControl
        Dim small As HtmlGenericControl
        Dim anchor2 As HtmlGenericControl
        Dim rbtncomponent As RadioButton
        Dim lblcomponent As Label
        Dim hdnComponentId As HiddenField
        Dim btn As New Button

        li = New HtmlGenericControl("li")
        anchor1 = New HtmlGenericControl("a")
        anchor1.Attributes.Add("href", "#")
        image = New Image
        image.ImageUrl = imageURL

        anchor1.Controls.Add(image)
        li.Controls.Add(anchor1)

        h2 = New HtmlGenericControl("h2")
        'h2.Attributes.Add("class", "subheader")

        small = New HtmlGenericControl("span")

        anchor2 = New HtmlGenericControl("a")
        anchor2.Attributes.Add("href", "#")

        rbtncomponent = New RadioButton
        rbtncomponent.ID = RADIOBUTTONID & itemComponentId.ToString().Trim()
        rbtncomponent.GroupName = RADIOBUTTONGROUPID
        rbtncomponent.CssClass = "u-radio-postback__container"
        rbtncomponent.AutoPostBack = True
        rbtncomponent.Attributes.Remove("checked")
        plhComponentDetails.Visible = False

        AddHandler rbtncomponent.CheckedChanged, AddressOf rdoComponent_CheckedChanged
        rbtncomponent.EnableViewState = True
        rbtncomponent.ViewStateMode = UI.ViewStateMode.Enabled

        'We always need to display the error component id.
        If errorComponentid > 0 Then rbtncomponent.Enabled = False
        If ComponentId = itemComponentId AndAlso errorComponentid > 0 AndAlso errorComponentid <> ComponentId Then
            rbtncomponent.Checked = False
        ElseIf errorComponentid = itemComponentId AndAlso errorComponentid > 0 Then
            rbtncomponent.Checked = True
            ComponentId = errorComponentid
            SetHiddenFields()
            plhComponentDetails.Visible = True
        End If

        'Default the first component in the group to be the default
        If ulComponents.Controls.Count = 0 And errorComponentid = 0 Then
            rbtncomponent.Checked = True

            If itemComponentId <> 0 And ComponentId = 0 Then
                ComponentId = itemComponentId
                SetHiddenFields()
                plhComponentDetails.Visible = True
            End If
        End If

        lblcomponent = New Label
        lblcomponent.ID = LABELID & itemComponentId.ToString().Trim()
        lblcomponent.AssociatedControlID = RADIOBUTTONID & itemComponentId.ToString().Trim()

        If Not itemIsAvailable Then
            rbtncomponent.Enabled = False
        End If

        lblcomponent.Text = itemComponentText

        anchor2.Controls.Add(rbtncomponent)
        anchor2.Controls.Add(lblcomponent)

        small.Controls.Add(anchor2)
        h2.Controls.Add(small)

        hdnComponentId = New HiddenField
        hdnComponentId.ID = HIDDENID & itemComponentId.ToString().Trim()
        hdnComponentId.Value = itemComponentId.ToString().Trim()
        li.Controls.Add(h2)
        li.Controls.Add(hdnComponentId)
        li.Attributes.Add("class", GetComponentListItemCSSClass(image.ImageUrl, itemIsAvailable))
        ulComponents.Controls.Add(li)

    End Sub
    Private Function GetComponentListItemCSSClass(ByVal imageURL As String, ByVal isAvailable As Boolean) As String
        Dim cssClass As String = String.Empty
        If Not String.IsNullOrWhiteSpace(imageURL) Then
            If imageURL.LastIndexOf("/") > 0 AndAlso imageURL.LastIndexOf(".") > 0 Then
                cssClass = imageURL.Substring(imageURL.LastIndexOf("/") + 1, (imageURL.LastIndexOf(".") - imageURL.LastIndexOf("/")) - 1)
            End If
        End If
        If Not String.IsNullOrWhiteSpace(cssClass) Then
            cssClass = cssClass.Replace(" ", "-")
        Else
            cssClass = "no-comp-img"
        End If
        If Not isAvailable Then
            cssClass = "ebiz-sold-out ebiz-" & cssClass
        Else
            cssClass = "ebiz-" & cssClass
        End If
        Return cssClass
    End Function

    Private Sub SetChildControlValuesInClient(ByVal AreaCode As String, ByRef wc As WebControl, Optional ByVal PriceBand As String = "")

        If dsCustomerPackageDetails IsNot Nothing AndAlso dsCustomerPackageDetails.Tables.Count >= 5 Then

            Dim dtSeats As DataTable = dsCustomerPackageDetails.Tables("Seat")

            Dim keyCode As String = String.Empty
            For listIndex As Integer = 0 To ddlDestination.Items.Count - 1
                keyCode = ddlDestination.Items(listIndex).Value
                Dim standAndArea As String = Talent.Common.Utilities.FixStringLength(keyCode, 3) & Talent.Common.Utilities.FixStringLength(AreaCode, 4)
                Dim value As Integer = 0

                For Each dr As DataRow In dtSeats.Rows
                    If dr("ComponentId") = ComponentId Then
                        If dr("SeatDetails").ToString.Substring(0, 7) = standAndArea Then
                            If String.IsNullOrWhiteSpace(PriceBand) OrElse PriceBand.Trim = dr("PriceBand") Then
                                value += 1
                            End If
                        End If
                    End If
                Next
                AddComponentChildEntityCollection(keyCode & "!" & wc.ClientID, keyCode, wc.ClientID, value, "DDL")
            Next

        End If
    End Sub

    Private Sub AddComponentChildEntityCollection(ByVal entityKey As String, ByVal keyCode As String, ByVal clientID As String, ByVal keyValue As String, ByVal keyType As String)
        If Not _dicCompChildEntities.ContainsKey(entityKey) Then
            _dicCompChildEntities.Add(entityKey, New ComponentChildHolder(keyCode, clientID, keyValue, keyType))
        End If
    End Sub



#End Region

#Region "Public Methods"
    Public Function GetValueFromControlTable(ByVal ComponentId As String) As String
        Return ucr.Content(ComponentGroupId & "_" & ComponentId & RADIOBUTTONTEXT, _languageCode, True)
    End Function

    Public Function GetValueFromAttributeTable(ByVal ComponentId As String) As String
        'Return ("http://placehold.it/400x300&amp;text=[img]") 
        Return ucr.Attribute(ComponentGroupId & "_" & ComponentId & IMAGETEXT, True)
    End Function

#End Region

End Class