Imports Talent.eCommerce

Partial Class UserControls_OrderEnquiry
    Inherits ControlBase
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim ucr As New Talent.Common.UserControlResource
    Dim pds As New PagedDataSource

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OrderEnquiry.ascx"
        End With

        If ucr.Content("StatusLabel", _languageCode, True) = String.Empty Then
            StatusLabel.Visible = False
            Status.Visible = False

        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '<a href="javascript:;" onclick="calendarPicker('aspnetForm.ctl00_ContentPlaceHolder1_OrderEnquiry1_FromDate');" title="Pick Date from Calendar">Select Date</a><br />
        If Visible Then
            If Not Page.IsPostBack Then
                SetText()
                BindStatusDDL()
            End If
            BindOrderHistoryView()
            If Not Page.IsPostBack Then
                DisplayPagerNav(pds)
            End If
        End If

    End Sub

    Protected Sub BindStatusDDL()
        Dim descAdapter As New TalentDescriptionsDataSetTableAdapters.tbl_ebusiness_descriptions_buTableAdapter
        Dim descs As TalentDescriptionsDataSet.tbl_ebusiness_descriptions_buDataTable =
                                                                        descAdapter.GetDatabyBUetc("OrderEnquiry",
                                                                                                    TalentCache.GetBusinessUnit,
                                                                                                    TalentCache.GetPartner(Profile),
                                                                                                    "BackOfficeStatus")

        If descs.Rows.Count > 0 Then
            Status.DataSource = descs
            Status.DataValueField = "DESCRIPTION_CODE"
            Status.DataTextField = "LANGUAGE_DESCRIPTION"
            Status.DataBind()
            Status.Items.Insert(0, "--")
        Else
            descs = descAdapter.GetDatabyBUetc("OrderEnquiry",
                                                TalentCache.GetBusinessUnit,
                                                Talent.Common.Utilities.GetAllString,
                                                "BackOfficeStatus")
            If descs.Rows.Count > 0 Then
                Status.DataSource = descs
                Status.DataValueField = "DESCRIPTION_CODE"
                Status.DataTextField = "LANGUAGE_DESCRIPTION"
                Status.DataBind()
                Status.Items.Insert(0, "--")
            End If
        End If

    End Sub

    Protected Sub SetText()
        With ucr
            InstructionsLabel.Text = .Content("InstructionsText", _languageCode, True)
            OrderNoLabel.Text = .Content("OrderNumberLabel", _languageCode, True)
            FromDateLabel.Text = .Content("FromDateLabel", _languageCode, True)
            ToDateLabel.Text = .Content("ToDateLabel", _languageCode, True)
            StatusLabel.Text = .Content("StatusLabel", _languageCode, True)
            filterButton.Text = .Content("FilterButtonText", _languageCode, True)
        End With
    End Sub

    Protected Sub BindOrderHistoryView()
        Dim cmd As New Data.SqlClient.SqlCommand()

        cmd.Parameters.Clear()
        cmd.Connection = New Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        '-----------------------------------------------
        ' check if in b2b mode - if so select by Partner
        '-----------------------------------------------
        Dim b2bMode As Boolean = TDataObjects.AppVariableSettings.TblAuthorizedPartners.CheckB2BMode(TalentCache.GetBusinessUnit())
        Dim def As ECommerceModuleDefaults.DefaultValues = New ECommerceModuleDefaults().GetDefaults()
        Dim isShowPartnerDetails As Boolean = def.ORDER_ENQUIRY_SHOW_PARTNER_ORDERS And (Not Profile.IsAnonymous AndAlso Boolean.Parse(Profile.PartnerInfo.Details.Order_Enquiry_Show_Partner_Orders))

        Dim sb As New StringBuilder
        With sb
            .Append("SELECT o.*, 'STATUSTEXT'=(CASE WHEN o.[STATUS] >= 50 THEN 'PAYMENT ACCEPTED' ELSE o.[STATUS] END), ")
            .Append("(SELECT COUNT(ORDER_ID) ")
            .Append("FROM tbl_order_detail WITH (NOLOCK)  ")
            .Append("WHERE ORDER_ID = o.TEMP_ORDER_ID) ")
            .Append("AS LINES ")
            .Append("FROM tbl_order_header AS o WITH (NOLOCK)  ")
            If Not b2bMode Then
                '---------
                ' B2C mode
                '---------
                ' If these are different, there's a business unit group defined - show all
                If isShowPartnerDetails Then
                    .Append("WHERE o.BUSINESS_UNIT = @bu ")
                    .Append("AND o.partner = @partner ")
                Else
                    If Not TalentCache.GetBusinessUnit.Equals(TalentCache.GetBusinessUnitGroup) Then
                        .Append("WHERE o.LOGINID = @loginID ")
                    Else
                        .Append("WHERE o.BUSINESS_UNIT = @bu ")
                        .Append("AND o.LOGINID = @loginID ")
                    End If
                End If
            Else
                '---------
                ' B2B mode
                '---------
                ''----- ORIGINAL -----
                '.Append("WHERE o.partner = @partner ")

                ' --- new ----
                If isShowPartnerDetails Then
                    .Append("WHERE o.BUSINESS_UNIT = @bu ")
                    .Append("AND o.partner = @partner ")
                Else
                    .Append("WHERE o.BUSINESS_UNIT = @bu ")
                    .Append("AND o.LOGINID = @loginID ")
                End If
            End If

            .Append("AND o.STATUS >= 50 ")
        End With
        cmd.CommandText = sb.ToString

        cmd = BuildWhereClause(cmd)
        cmd.CommandText += " ORDER BY CREATED_DATE DESC "
        With cmd.Parameters
            If Not b2bMode Then
                If isShowPartnerDetails Then
                    .Add("@partner", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                    .Add("@bu", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                Else
                    '---------
                    ' B2C mode
                    '---------  
                    .Add("@bu", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@loginID", Data.SqlDbType.NVarChar).Value = Profile.UserName
                End If
            Else
                '---------
                ' B2B mode
                '--------- 
                ''----- ORIGINAL -----
                '.Add("@partner", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)

                ' --- new ----
                If isShowPartnerDetails Then
                    .Add("@bu", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@partner", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                Else
                    .Add("@bu", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@loginID", Data.SqlDbType.NVarChar).Value = Profile.UserName
                End If
            End If

        End With

        Try
            cmd.Connection.Open()

            OrderHistoryView.DataSource = SetUpPagedView(cmd)
            OrderHistoryView.DataBind()
        Catch ex As Exception
            'error with DB connection/command execution
        Finally
            cmd.Connection.Close()
            cmd.Connection.Dispose()
            cmd.Dispose()
        End Try

    End Sub

    Protected Function GetCustomerRefField() As String
        If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("CustomerRefIsEmail")) Then
            Return "CONTACT_EMAIL"
        Else
            If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("CustomerRefIsPO")) Then
                Return "PURCHASE_ORDER"
            Else
                Return "LOGINID"
            End If
        End If
    End Function

    Protected Function SetUpPagedView(ByVal cmd As Data.SqlClient.SqlCommand) As PagedDataSource
        Dim SqlTA As New Data.SqlClient.SqlDataAdapter(cmd)
        Dim ds As New Data.DataSet
        pds.AllowPaging = True
        pds.PageSize = CInt(ucr.Attribute("ResultsPageSize"))

        SqlTA.Fill(ds)
        pds.DataSource = ds.Tables(0).DefaultView

        Return pds
    End Function

    Protected Sub DisplayPagerNav(ByVal pds As PagedDataSource)
        If pds.PageCount > 1 Then

            Dim startPage As Integer = 0,
                loopToPage As Integer = 0,
                currentPage As Integer = 0

            currentPage = pds.CurrentPageIndex + 1

            'If there are more than 10 pages we need to sort out which 10 pages either side
            'of the current page to display in the navigation:
            'E.g. if page = 8 and ther are 15 pages, the navigation should be:
            'First 3 4 5 6 7 8 9 10 11 12 Last
            If pds.PageCount > 10 Then
                If Not pds.CurrentPageIndex = 0 Then
                    For i As Integer = 3 To 0 Step -1
                        If currentPage + i <= pds.PageCount Then
                            startPage = currentPage - (9 - i)
                            If startPage < 0 Then
                                startPage = 0
                            End If
                            Exit For
                        End If
                    Next
                End If
                loopToPage = startPage + 9
            Else
                loopToPage = pds.PageCount
            End If

            If loopToPage = pds.PageCount Then
                If pds.PageCount > 10 Then
                    startPage -= 1
                    loopToPage -= 1
                Else
                    loopToPage -= 1
                End If
            End If


            If pds.PageCount > 10 Then
                FirstTop.Text = ucr.Content("FirstItemNavigationText", _languageCode, True)
                FirstBottom.Text = ucr.Content("FirstItemNavigationText", _languageCode, True)
            End If
            Dim count As Integer = 0
            For i As Integer = startPage To loopToPage
                count += 1
                CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Text = (i + 1).ToString
                CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Text = (i + 1).ToString

                'Set the font to bold if is the selected page
                'otherwise set it back to normal
                If pds.CurrentPageIndex = i Then
                    CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Font.Bold = True
                    CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Font.Bold = True
                Else
                    CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Font.Bold = False
                    CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Font.Bold = False
                End If
            Next

            If pds.PageCount > 10 Then
                LastTop.Text = ucr.Content("LastItemNavigationText", _languageCode, True)
                LastBottom.Text = ucr.Content("LastItemNavigationText", _languageCode, True)
            End If

            count += 1
            For i As Integer = count To 10
                CType(Me.FindControl("Nav" & i.ToString & "Top"), LinkButton).Text = String.Empty
                CType(Me.FindControl("Nav" & i.ToString & "Bottom"), LinkButton).Text = String.Empty
            Next

            Dim arg0, arg1, arg2 As Integer
            arg0 = pds.PageSize * pds.CurrentPageIndex + 1
            If pds.IsLastPage Then
                arg1 = pds.DataSourceCount
            Else
                arg1 = (arg0 - 1) + pds.PageSize
            End If
            arg2 = pds.DataSourceCount

            CurrentResultsDisplaying.Text = String.Format(ucr.Content("ResultsCurrentViewText", _languageCode, True), arg0.ToString, arg1.ToString, arg2.ToString)
        Else
        End If
    End Sub

    Protected Sub ChangePage(ByVal sender As Object, ByVal e As EventArgs)
        Dim lb As LinkButton = CType(sender, LinkButton)
        Dim pageIndex As Integer = 0
        Try
            pageIndex = CInt(lb.Text)
            pageIndex -= 1
        Catch ex As Exception
            If lb.Text = ucr.Content("FirstItemNavigationText", _languageCode, True) Then
                pageIndex = 0
            ElseIf lb.Text = ucr.Content("LastItemNavigationText", _languageCode, True) Then
                pageIndex = pds.PageCount - 1
            End If
        End Try

        pds.CurrentPageIndex = pageIndex
        BindOrderHistoryView()
        DisplayPagerNav(pds)
    End Sub

    Protected Function BuildWhereClause(ByVal cmd As Data.SqlClient.SqlCommand) As Data.SqlClient.SqlCommand
        With cmd
            If Not String.IsNullOrEmpty(OrderNo.Text) Then
                .CommandText += " AND PROCESSED_ORDER_ID = @orderNo "
                .Parameters.Add("@orderNo", Data.SqlDbType.NVarChar).Value = OrderNo.Text
            End If
            If Not String.IsNullOrEmpty(FromDate.Text) Then
                Dim d As DateTime
                Try
                    d = CType(FromDate.Text, DateTime)
                    .CommandText += " AND CREATED_DATE >= @FromDate "
                    .Parameters.Add("@FromDate", Data.SqlDbType.DateTime).Value = d
                Catch ex As Exception
                    'Catch date error
                End Try
            End If
            If Not String.IsNullOrEmpty(ToDate.Text) AndAlso Not FromDate.Text = ToDate.Text Then
                Dim d As DateTime
                Try
                    d = CType(ToDate.Text, DateTime)
                    .CommandText += " AND CREATED_DATE <= @ToDate "
                    .Parameters.Add("@ToDate", Data.SqlDbType.DateTime).Value = d
                Catch ex As Exception
                    'Catch date error
                End Try
            End If
            If Status.SelectedValue.ToString.Length > 0 AndAlso Not Status.SelectedValue.Contains("--") Then
                .CommandText += " AND BACK_OFFICE_STATUS = @status "
                .Parameters.Add("@status", Data.SqlDbType.NVarChar).Value = Status.SelectedValue
            End If
        End With
        Return cmd
    End Function

    Protected Sub filterButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles filterButton.Click
        BindOrderHistoryView()
        DisplayPagerNav(pds)
    End Sub

    Protected Sub OpenOrderDetails(ByVal sender As Object, ByVal e As EventArgs)
        Dim ri As RepeaterItem = CType(CType(sender, HyperLink).Parent, RepeaterItem)
        Dim orderID As String = CType(ri.FindControl("WebOrderID"), Label).Text
        If Not String.IsNullOrEmpty(orderID) Then
            CType(sender, HyperLink).NavigateUrl = "~/PagesLogin/Orders/OrderDetails.aspx?wid=" & orderID
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Try
            With ucr
                Select Case sender.ID
                    Case Is = "OrderIDHeader"
                        sender.Text = .Content("OrderIDHeader", _languageCode, True)
                    Case Is = "WebOrderIDHeader"
                        sender.Text = .Content("WebOrderIDHeader", _languageCode, True)
                    Case Is = "OrderDateHeader"
                        sender.Text = .Content("OrderDateHeader", _languageCode, True)
                    Case Is = "CustomerRefHeader"
                        sender.Text = .Content("CustomerRefHeader", _languageCode, True)
                    Case Is = "LinesHeader"
                        sender.Text = .Content("LinesHeader", _languageCode, True)
                    Case Is = "OrderValueHeader"
                        sender.Text = .Content("OrderValueHeader", _languageCode, True)
                    Case Is = "OrderStatusHeader"
                        sender.Text = .Content("OrderStatusHeader", _languageCode, True)
                    Case Is = "ViewDetailsLinkHeader"
                        sender.Text = .Content("ViewDetailsLinkHeader", _languageCode, True)
                    Case Is = "ViewDetailsLink"
                        sender.Text = .Content("ViewDetailsLinkText", _languageCode, True)
                End Select
            End With
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Get the formatted order value for the total amount charged
    ''' </summary>
    ''' <param name="totalAmountCharged">The total amount charged</param>
    ''' <returns>The formatted order value</returns>
    ''' <remarks></remarks>
    Public Function GetOrderValue(ByVal totalAmountCharged As Decimal) As String
        Dim orderValue As String = String.Empty
        Dim value As Double = Talent.eCommerce.Utilities.RoundToValue(totalAmountCharged, 0.01, False)
        orderValue = TDataObjects.PaymentSettings.FormatCurrency(value, ucr.BusinessUnit, ucr.PartnerCode)
        Return orderValue
    End Function

    Protected Sub CanDisplayThisColumn(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Select Case sender.id
                Case "plhShowOrderValue"
                    sender.Visible = Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowOrderHeaderColumn"))
                Case "plhShowOrderHeader"
                    sender.Visible = Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowOrderHeaderColumn"))
            End Select
        Catch ex As Exception
            sender.Visible = False
        End Try
    End Sub
End Class
