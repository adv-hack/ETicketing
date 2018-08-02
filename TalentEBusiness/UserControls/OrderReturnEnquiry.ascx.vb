Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports System.Xml
Imports System.Globalization
Imports System.Threading

Partial Class UserControls_OrderReturnEnquiry
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _ucr As New Talent.Common.UserControlResource
    Private _errMsg As Talent.Common.TalentErrorMessages
    Private _ds1 As New DataSet()

#End Region

#Region "Constants"

    Const _cStatusTable = 0
    Const _cOrderTable = 1

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Check if agent has access on BuyBacks menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessBuybacks) Or Not AgentProfile.IsAgent Then
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = String.Empty
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "OrderReturnEnquiry.ascx"
            End With
            _errMsg = New Talent.Common.TalentErrorMessages(_languageCode,
                                                            TalentCache.GetBusinessUnitGroup,
                                                            TalentCache.GetPartner(Profile),
                                                            ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

            ContinueButton.Text = _ucr.Content("ContinueButtonText", _languageCode, True)
            If Not Page.IsPostBack Then
                ' Display any errors from ticketing gateway
                If Not Session("errormsg") = "" Then
                    showError(Session("errormsg").ToString)
                    Session("errormsg") = ""
                End If
            End If
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Visible Then
            Try
                If Not Page.IsPostBack Then
                    showData()
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (errorlist.Items.Count > 0)
        CustomerRepeater.Visible = ButtonsPanel.Visible
    End Sub

    Protected Sub CustomerRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CustomerRepeater.ItemDataBound
        If e.Item.ItemIndex <> -1 Then
            Dim ordRpt As Repeater = CType(e.Item.FindControl("OrderRepeater"), Repeater)
            Dim ltlCustomerLabel As Literal = CType(e.Item.FindControl("ltlCustomerLabel"), Literal)
            Dim ltlCustomerValue As Literal = CType(e.Item.FindControl("ltlCustomerValue"), Literal)
            Dim hdfCustomerNumber As HiddenField = CType(e.Item.FindControl("hdfCustomerNumber"), HiddenField)
            ltlCustomerLabel.Text = _ucr.Content("CustomerLabel", _languageCode, True)
            Dim customerValue As String = _ucr.Content("CustomerValue", _languageCode, True)
            customerValue = customerValue.Replace("<<CUSTOMER_NUMBER>>", e.Item.DataItem("CustomerNo").ToString.TrimStart("0"))
            customerValue = customerValue.Replace("<<CUSTOMER_INITIAL>>", e.Item.DataItem("ContactInitial"))
            customerValue = customerValue.Replace("<<CUSTOMER_SURNAME>>", e.Item.DataItem("ContactSurname"))
            hdfCustomerNumber.Value = e.Item.DataItem("CustomerNo").ToString()
            ltlCustomerValue.Text = customerValue
            If Not ordRpt Is Nothing Then
                Dim mydt As DataTable = _ds1.Tables(_cOrderTable)
                Dim myRows() As DataRow = mydt.Select("CustomerNo = '" & e.Item.DataItem("CustomerNo") & "'")
                If myRows.Length > 0 Then
                    ordRpt.DataSource = myRows
                    ordRpt.DataBind()
                End If
            End If
        End If
    End Sub

    Protected Sub DoOrdersRepeaterItemDatabound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemIndex <> -1 Then
            Dim dateLabel As Literal = CType(e.Item.FindControl("dateLabel"), Literal)
            Dim matchLabel As Literal = CType(e.Item.FindControl("matchLabel"), Literal)
            Dim seatLabel As Literal = CType(e.Item.FindControl("seatLabel"), Literal)
            Dim statusLabel As Literal = CType(e.Item.FindControl("statusLabel"), Literal)
            Dim checkBox As CheckBox = CType(e.Item.FindControl("OrderSelectionCBox"), CheckBox)
            Dim hfProductCode As HiddenField = CType(e.Item.FindControl("hfProductCode"), HiddenField)
            Dim hfStatus As HiddenField = CType(e.Item.FindControl("hfStatus"), HiddenField)
            Dim hfSeat As HiddenField = CType(e.Item.FindControl("hfSeat"), HiddenField)
            Dim dr As DataRow = CType(e.Item.DataItem, DataRow)

            dateLabel.Text = dr("ProductDate")
            matchLabel.Text = dr("ProductDescription")

            Select Case ModuleDefaults.SeatDisplay
                Case Is = 1
                    'Show stand, area and row ( stand is first 3 chars, area is next 4, and row is the next 4 )
                    seatLabel.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("Seat")).Substring(0, 11)
                Case Is = 2
                    'Show stand, area, row and seat number (seat number is next 4 chars after above)
                    seatLabel.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("Seat")).Substring(0, 15)
                Case Else
                    'Show all (including alpha suffix)
                    seatLabel.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("Seat"))
            End Select
            hfSeat.Value = seatLabel.Text
            If dr("Unreserved") = True Then
                seatLabel.Text = _ucr.Content("UnreservedAreaText", _languageCode, True)
            End If
            statusLabel.Text = dr("Status")
            hfStatus.Value = dr("Status")
            hfProductCode.Value = dr("ProductCode")

            'Order Returns should only be available at a 'Booked' status
            If hfStatus.Value.Trim <> "Booked" Then
                checkBox.Checked = False
                checkBox.Visible = False
            End If

            'If the defaults are set to allow the return of booked seats, the checkbox should 
            ' also be available for 'Returned' seats.
            If ModuleDefaults.AllowRebookReturnedSeats And hfStatus.Value.Trim = "Returned" Then
                checkBox.Visible = True
            End If

            'translate the status
            Dim translateStatus As String = _ucr.Content(UCase(hfStatus.Value.Trim), _languageCode, True)
            If translateStatus <> String.Empty Then
                statusLabel.Text = translateStatus
            End If
        End If
    End Sub

    Protected Sub ContinueButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ContinueButton.Click
        Dim myList As New Generic.List(Of String())
        Dim status As String = String.Empty
        errorlist.Items.Clear()

        For Each ri As RepeaterItem In CustomerRepeater.Items

            'Exist loop if an error has occured.
            If errorlist.Items.Count > 0 Then
                Exit For
            End If

            Try
                Dim orderRepeater As Repeater = CType(ri.FindControl("OrderRepeater"), Repeater)
                Dim custNo As HiddenField = CType(ri.FindControl("hdfCustomerNumber"), HiddenField)
                If Not orderRepeater Is Nothing Then
                    For Each subri As RepeaterItem In orderRepeater.Items
                        Dim dateLabel As Literal = CType(subri.FindControl("dateLabel"), Literal)
                        Dim matchLabel As Literal = CType(subri.FindControl("matchLabel"), Literal)
                        Dim seatLabel As Literal = CType(subri.FindControl("seatLabel"), Literal)
                        Dim statusLabel As Literal = CType(subri.FindControl("statusLabel"), Literal)
                        Dim hfProductCode As HiddenField = CType(subri.FindControl("hfProductCode"), HiddenField)
                        Dim hfStatus As HiddenField = CType(subri.FindControl("hfStatus"), HiddenField)
                        Dim hfSeat As HiddenField = CType(subri.FindControl("hfSeat"), HiddenField)

                        Dim orderSelection As CheckBox = CType(subri.FindControl("OrderSelectionCBox"), CheckBox)

                        If orderSelection.Checked Then

                            'Save the status of the seat, cannot process Order Returns and Rebooks at the same time.
                            If status = String.Empty Then
                                status = hfStatus.Value
                            End If

                            If status <> hfStatus.Value Then
                                Dim eli As New ListItem(GetText("OrderReturnRebookError"))
                                errorlist.Items.Add(eli)
                                Exit For
                            End If

                            Dim myEntry(6) As String
                            myEntry(0) = custNo.Value
                            myEntry(1) = dateLabel.Text
                            myEntry(2) = matchLabel.Text
                            myEntry(3) = hfProductCode.Value
                            myEntry(4) = hfSeat.Value
                            myEntry(5) = hfStatus.Value
                            myList.Add(myEntry)
                        End If
                    Next
                End If

            Catch ex As Exception
                showError("XX")
                Exit For
            End Try
        Next

        If myList.Count > 0 And errorlist.Items.Count = 0 Then
            Session.Add("SelectedOrderList", myList)
            Response.Redirect("~/PagesLogin/Orders/OrderReturn.aspx")
        Else
            'Output 'No orders selected' Text.
            If myList.Count = 0 Then
                Dim eli As New ListItem(GetText("NoOrdersSelectedText"))
                If Not errorlist.Items.Contains(eli) Then
                    errorlist.Items.Add(eli)
                End If
            End If
        End If
    End Sub

    Protected Sub showError(ByVal errCode As String)
        Dim eli As ListItem
        Dim myError As String = CStr(Session("TalentErrorCode"))
        eli = New ListItem(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errCode).ERROR_MESSAGE)
        If Not errorlist.Items.Contains(eli) Then
            errorlist.Items.Add(eli)
        End If
    End Sub

#End Region

#Region "Protected Functions"

    Protected Function GetText(ByVal PValue As String) As String
        Dim str As String = _ucr.Content(PValue, _languageCode, True)
        Return str
    End Function

    Protected Function showDate(ByVal Pvalue As DateTime) As String
        Dim str As String
        str = Pvalue.ToString("dd/MM/yyyy")
        Return str
    End Function

#End Region

#Region "Private Methods"

    Private Sub showData()
        Try
            ButtonsPanel.Visible = True
            Dim order As New Talent.Common.TalentOrder
            Dim settings As New Talent.Common.DESettings
            Dim err As New Talent.Common.ErrorObj
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues
            def = moduleDefaults.GetDefaults()

            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)

            ' PLEASE READ: OrderReturnConfirmation.aspx needs to clear this cache.  
            ' Any changes below must be reflected in both places.
            settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
            settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
            settings.CacheStringExtension = _ucr.KeyCode & "customer=" & settings.AccountNo1

            order.Settings() = settings

            Dim deTicketingItemDetails As New Talent.Common.DETicketingItemDetails

            With deTicketingItemDetails
                .CustomerNo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                .Src = "W"
                .Type = "FutureBooked"
            End With

            order.Dep.CollDEOrders.Add(deTicketingItemDetails)
            err = order.OrderDetails()
            _ds1 = order.ResultDataSet()

            If Not err.HasError Then

                If _ds1.Tables(_cStatusTable).Rows(0).Item("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                    showError(_ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString())
                Else
                    Dim mydt As DataTable = _ds1.Tables(_cOrderTable)
                    Dim customerView As New DataView(mydt)
                    CustomerRepeater.DataSource = customerView.ToTable(True, "CustomerNo", "ContactSurname", "ContactInitial")
                    CustomerRepeater.DataBind()
                End If
            Else
                showError("XX")
                ButtonsPanel.Visible = False
            End If

        Catch ex As Exception
            showError("XX")
            ButtonsPanel.Visible = False
        End Try
    End Sub

#End Region

End Class