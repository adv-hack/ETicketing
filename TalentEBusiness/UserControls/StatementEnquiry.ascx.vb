Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_StatementEnquiry
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim ucr As New Talent.Common.UserControlResource
    Dim pds As New PagedDataSource

    'Public DataTable for use by the PDF Build Code
    Public loopItemsDT As New Data.DataTable

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "StatementEnquiry.ascx"
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
        BindStatementHistoryView()
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

    Protected Sub BindStatementHistoryView()
        StatementHistoryView.DataSource = SetUpPagedView()
        StatementHistoryView.DataBind()
    End Sub

    Protected Function SetUpPagedView() As PagedDataSource
        pds.AllowPaging = True
        pds.PageSize = CInt(ucr.Attribute("ResultsPageSize"))

        Dim statement As New TalentStatement
        Dim deS As New Talent.Common.DEStatement(TalentCache.GetBusinessUnit, Profile.UserName)
        Try
            deS.FromDate = FromDate.Text
        Catch ex As Exception
        End Try
        Try
            deS.ToDate = ToDate.Text
        Catch ex As Exception
        End Try
        deS.Finalised = False
        deS.OrderNumber = OrderNo.Text

        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        settings.Partner = Profile.PartnerInfo.Details.Partner

        statement.deS = deS
        statement.Settings = settings
        statement.GetConnectionDetails(TalentCache.GetBusinessUnit)

        Dim err As ErrorObj = statement.GetStatement
        Try
            loopItemsDT = statement.ResultDataSet.Tables("StatementsHeader")
        Catch ex As Exception
        End Try
        
        Try
            Dim dt As Data.DataTable = statement.ResultDataSet.Tables("StatementsHeader")
            pds.DataSource = dt.DefaultView
        Catch ex As Exception
        End Try

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
        BindStatementHistoryView()
        DisplayPagerNav(pds)
    End Sub

    Protected Sub filterButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles filterButton.Click
        BindStatementHistoryView()
        DisplayPagerNav(pds)
    End Sub

    Protected Sub OpenStatementDetails(ByVal sender As Object, ByVal e As EventArgs)
        Dim ri As RepeaterItem = CType(CType(sender, HyperLink).Parent, RepeaterItem)
        Dim ItemID As String = CType(ri.FindControl("ItemID"), Label).Text
        Dim type As String = CType(ri.FindControl("Type"), Label).Text

        Select Case type
            Case Is = ucr.Content("InvoiceTypeText", _languageCode, True)
                CType(sender, HyperLink).NavigateUrl = "~/PagesLogin/Invoices/InvoiceEnquiryDetail.aspx?hid=" & ItemID
            Case Is = ucr.Content("CreditNoteTypeText", _languageCode, True)
                CType(sender, HyperLink).NavigateUrl = "~/PagesLogin/CreditNotes/CreditNoteEnquiryDetail.aspx?hid=" & ItemID
        End Select

        If Not String.IsNullOrEmpty(ItemID) Then
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Try

            With ucr
                Select Case sender.ID
                    Case Is = "TransactionDateHeader"
                        CType(sender, Label).Text = .Content("TransactionDateHeader", _languageCode, True)
                    Case Is = "DueDateHeader"
                        CType(sender, Label).Text = .Content("DueDateHeader", _languageCode, True)
                    Case Is = "TypeHeader"
                        CType(sender, Label).Text = .Content("TypeHeader", _languageCode, True)
                    Case Is = "RefHeader"
                        CType(sender, Label).Text = .Content("RefHeader", _languageCode, True)
                    Case Is = "ValueHeader"
                        CType(sender, Label).Text = .Content("ValueHeader", _languageCode, True)
                    Case Is = "OutstandingValueHeader"
                        CType(sender, Label).Text = .Content("OutstandingValueHeader", _languageCode, True)
                    Case Is = "ViewDetailsLinkHeader"
                        CType(sender, Label).Text = .Content("ViewDetailsLinkHeader", _languageCode, True)
                    Case Is = "ViewDetailsLink"
                        CType(sender, HyperLink).Text = .Content("ViewDetailsLinkText", _languageCode, True)

                End Select
            End With
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetStatementType(ByVal str As Object) As String
        Dim type As String = ""
        Try
            type = CStr(str)
            Select Case ucase(type)
                Case Is = "CREDITNOTE"
                    type = ucr.Content("CreditNoteTypeText", _languageCode, True)
                Case Is = "INVOICE"
                    type = ucr.Content("InvoiceTypeText", _languageCode, True)
            End Select
        Catch ex As Exception

        End Try
        Return type
    End Function

    Protected Sub btnCreatePDF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreatePDF.Click

        Dim DefaultDict As New Generic.Dictionary(Of String, String)

        Dim layoutFile As String = Server.MapPath("..\..\PDF" & ModuleDefaults.pdfUrl & ucr.BusinessUnit & "\Layout\Statement1.bmp")
        'check fileExists
        If Not System.IO.File.Exists(layoutFile) Then
            layoutFile = "..\..\PDF" & ModuleDefaults.pdfUrl & "\Layout\Statement1.bmp"
        End If

        'Add Values
        With DefaultDict
            .Add("*BGIMAGE*", layoutFile)
        End With

        ' Save as Text file
        Dim xmlfilename As String = Server.MapPath("..\..\PDF" & ModuleDefaults.pdfUrl & ucr.BusinessUnit & "\XML\Statement.xml")
        'check fileExists
        If Not System.IO.File.Exists(xmlfilename) Then
            xmlfilename = "..\..\PDF" & ModuleDefaults.pdfUrl & "\XML\Statement.xml"
        End If

        ' createPDF.Main(args)
        Dim workingFolder = ucr.Attribute("TempFolder")
        Dim PDF As New Talent.eCommerce.CreatePDF(DefaultDict, loopItemsDT, ucr.Content("EmailSubject", _languageCode, True), ucr.Content("EmailMessage", _languageCode, True))
        PDF.Main(xmlfilename, workingFolder)

        FeedbackLabel.Text = ucr.Content("EmailConfirmation", _languageCode, True)
        btnCreatePDF.Visible = False

    End Sub

    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        formattedString = TDataObjects.PaymentSettings.FormatCurrency(value, ucr.BusinessUnit, ucr.PartnerCode)
        Return formattedString
    End Function

End Class
