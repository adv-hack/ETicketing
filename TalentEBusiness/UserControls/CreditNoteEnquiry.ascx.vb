Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_CreditNoteEnquiry
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
            .KeyCode = "CreditNoteEnquiry.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '<a href="javascript:;" onclick="calendarPicker('aspnetForm.ctl00_ContentPlaceHolder1_OrderEnquiry1_FromDate');" title="Pick Date from Calendar">Select Date</a><br />
        ToDateLinkButton.OnClientClick = "calendarPicker('" & Page.Form.ClientID & "." & ToDate.UniqueID & "');"
        ToDateLinkButton.Attributes.Add("href", "javascript:;")
        FromDateLinkButton.OnClientClick = "calendarPicker('" & Page.Form.ClientID & "." & FromDate.UniqueID & "');"
        FromDateLinkButton.Attributes.Add("href", "javascript:;")
        If Not Page.IsPostBack Then
            SetText()
            SetAttribute()
        End If
        BindCreditNoteHistoryView()
        If Not Page.IsPostBack Then
            DisplayPagerNav(pds)
        End If
        statusDiv.Visible = False
    End Sub

    Protected Sub SetText()
        With ucr
            ToDateLinkButton.Text = .Content("SelectDateText", _languageCode, True)
            FromDateLinkButton.Text = .Content("SelectDateText", _languageCode, True)
            InstructionsLabel.Text = .Content("InstructionsText", _languageCode, True)
            InvoiceNoLabel.Text = .Content("InvoiceNoLabel", _languageCode, True)
            OrderNoLabel.Text = .Content("OrderNumberLabel", _languageCode, True)
            FromDateLabel.Text = .Content("FromDateLabel", _languageCode, True)
            ToDateLabel.Text = .Content("ToDateLabel", _languageCode, True)
            StatusLabel.Text = .Content("StatusLabel", _languageCode, True)
            filterButton.Text = .Content("FilterButtonText", _languageCode, True)
        End With
    End Sub

    Protected Sub SetAttribute()
        With ucr
            FromDate.ReadOnly = CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("FromDateReadOnlyValue"))
            ToDate.ReadOnly = CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("ToDateReadOnlyValue"))
        End With
    End Sub

    Protected Sub BindCreditNoteHistoryView()
        CreditNoteHistoryView.DataSource = SetUpPagedView()
        CreditNoteHistoryView.DataBind()
    End Sub

    Protected Function SetUpPagedView() As PagedDataSource
        pds.AllowPaging = True
        pds.PageSize = CInt(ucr.Content("ResultsViewPageSize", _languageCode, True))

        Dim err As New Talent.Common.ErrorObj

        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        settings.Partner = Profile.PartnerInfo.Details.Partner

        'Get the credit notes
        '************************************
        Dim creditnote As New Talent.Common.TalentCreditNote
        Dim deC As New Talent.Common.DECreditNote(TalentCache.GetBusinessUnit, Profile.UserName)
        Try
            deC.FromDate = FromDate.Text
        Catch ex As Exception
        End Try
        Try
            deC.ToDate = ToDate.Text
        Catch ex As Exception
        End Try
        deC.OrderNumber = OrderNo.Text
        deC.Finalised = False

        creditnote.Dep = deC
        creditnote.Settings = Settings
        creditnote.GetConnectionDetails(TalentCache.GetBusinessUnit)

        err = creditnote.GetCreditNoteHeader

        If Not err.HasError Then
            Dim dtCred As Data.DataTable = creditnote.ResultDataSet.Tables("CreditNoteEnquiryHeader")
            pds.DataSource = dtCred.DefaultView
        End If

        Return pds
    End Function

    Protected Sub DisplayPagerNav(ByVal pds As PagedDataSource)
        If pds.PageCount > 1 Then

            Dim startPage As Integer = 0, _
                loopToPage As Integer = 0, _
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
        BindCreditNoteHistoryView()
        DisplayPagerNav(pds)
    End Sub

    Protected Sub filterButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles filterButton.Click
        BindCreditNoteHistoryView()
        DisplayPagerNav(pds)
    End Sub

    Protected Sub OpenCreditNoteDetails(ByVal sender As Object, ByVal e As EventArgs)
        Dim ri As RepeaterItem = CType(CType(sender, HyperLink).Parent, RepeaterItem)
        Dim CreditNoteID As String = CType(ri.FindControl("CreditNoteID"), Label).Text
        If Not String.IsNullOrEmpty(CreditNoteID) Then
            CType(sender, HyperLink).NavigateUrl = "~/PagesLogin/CreditNotes/CreditNoteEnquiryDetail.aspx?hid=" & CreditNoteID
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Try

            With ucr
                Select Case sender.ID
                    Case Is = "CustomerRefHeader"
                        CType(sender, Label).Text = .Content("CustomerRefHeader", _languageCode, True)
                    Case Is = "CompanyRefHeader"
                        CType(sender, Label).Text = .Content("CompanyRefHeader", _languageCode, True)
                    Case Is = "OrderDateHeader"
                        CType(sender, Label).Text = .Content("OrderDateHeader", _languageCode, True)
                    Case Is = "InvoiceIDHeader"
                        CType(sender, Label).Text = .Content("InvoiceIDHeader", _languageCode, True)
                    Case Is = "CreditNoHeader"
                        CType(sender, Label).Text = .Content("CreditNoHeader", _languageCode, True)
                    Case Is = "CreditDateHeader"
                        CType(sender, Label).Text = .Content("CreditDateHeader", _languageCode, True)
                    Case Is = "ViewDetailsLinkHeader"
                        CType(sender, Label).Text = .Content("ViewDetailsLinkHeader", _languageCode, True)
                    Case Is = "ViewDetailsLink"
                        CType(sender, HyperLink).Text = .Content("ViewDetailsLinkText", _languageCode, True)

                End Select
            End With
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Format the currency based on the given value
    ''' </summary>
    ''' <param name="value">The given value</param>
    ''' <returns>The formatted value</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedValue As String = value
        formattedValue = TDataObjects.PaymentSettings.FormatCurrency(value, ucr.BusinessUnit, ucr.PartnerCode)
        Return formattedValue
    End Function
End Class
