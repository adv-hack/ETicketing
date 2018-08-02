Imports Talent.Common
Imports System.Data

Partial Class UserControls_InvoiceEnquiryDetail
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim ucr As New Talent.Common.UserControlResource
    Dim pds As New PagedDataSource

    'Public DataTable for use by the PDF Build Code
    Public LoopItemsDT As Data.DataTable

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "InvoiceEnquiryDetail.ascx"
        End With
        CType(Me.Page, TalentBase01).PageHeaderText = "<h1>" & ucr.Content("PageHeaderText", _languageCode, True) & Request("hid") & "</h1>"
    End Sub

    Protected Sub GetDeliveryDetails()
        Dim invoice As New TalentInvoicing
        Dim dei As New Talent.Common.DEInvoice(TalentCache.GetBusinessUnit, Profile.UserName)

        dei.InvoiceNumber = Request("hid")

        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        settings.Partner = Profile.PartnerInfo.Details.Partner
        invoice.Dep = dei
        invoice.Settings = settings
        'invoice.GetConnectionDetails(TalentCache.GetBusinessUnit)

        Dim err As ErrorObj = invoice.InvoiceEnquiryHeader

        Dim dt As Data.DataTable = invoice.ResultDataSet.Tables("InvoiceEnquiryHeader")
        Dim dr As Data.DataRow
        If dt.Rows.Count > 0 Then
            dr = dt.Rows(0)
            invDate.Text = Utilities.CheckForDBNull_DateTime(dr("InvoiceDateTime"))
            custAccNo.Text = Utilities.CheckForDBNull_String(dr("AccountNumber"))
            custRef.Text = Utilities.CheckForDBNull_String(dr("CustomerPO"))

            invname.Text = Utilities.CheckForDBNull_String(dr("CustomerName"))
            invAdd1.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress1"))
            invAdd2.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress2"))
            invAdd3.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress3"))
            invAdd4.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress4"))
            invAdd5.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress5"))
            invAdd6.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress6"))
            invAdd7.Text = Utilities.CheckForDBNull_String(dr("CustomerAddress7"))

            delName.Text = Utilities.CheckForDBNull_String(dr("ShipToName"))
            delAdd1.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress1"))
            delAdd2.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress2"))
            delAdd3.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress3"))
            delAdd4.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress4"))
            delAdd5.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress5"))
            delAdd6.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress6"))
            delAdd7.Text = Utilities.CheckForDBNull_String(dr("ShipToAddress7"))

            '--------------------------------------------------------
            ' Check whether invoice amounts from back end include Tax
            '--------------------------------------------------------
            If ModuleDefaults.InvoiceAmountIncludesTax Then
                Dim decSubTotal As Decimal = 0
                vat.Text = FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("VatAmount")))
                If Not LoopItemsDT Is Nothing Then
                    For Each rw As DataRow In LoopItemsDT.Rows
                        decSubTotal += CDec(rw("InvoiceLineNetAmount").ToString)
                    Next
                End If
                subTotal.Text = decSubTotal
                total.Text = FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("InvoiceAmount")))
            Else
                vat.Text = FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("VatAmount")))
                subTotal.Text = FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("InvoiceAmount")))
                total.Text = FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("InvoiceAmount")) + Utilities.CheckForDBNull_String(dr("VatAmount")))
            End If

        End If

    End Sub

    Protected Function SetUpPagedView() As PagedDataSource
        pds.AllowPaging = True
        pds.PageSize = 10 'CInt(ucr.Attribute("ResultsPageSize"))

        Dim invoice As New TalentInvoicing
        Dim dei As New DEInvoice(Request("hid"))

        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        settings.AccountNo1 = Profile.User.Details.Account_No_1
        settings.AccountNo2 = Profile.User.Details.Account_No_2
        If Not Profile.PartnerInfo.Details.Account_No_3 Is Nothing Then
            settings.AccountNo3 = Utilities.CheckForDBNull_String(Profile.PartnerInfo.Details.Account_No_3)
        End If

        settings.Cacheing = Profile.PartnerInfo.Details.Caching_Enabled
        settings.CacheTimeMinutes = Profile.PartnerInfo.Details.Cache_Time_Minutes
        settings.Partner = Profile.PartnerInfo.Details.Partner
        settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath

        If String.IsNullOrEmpty(settings.AccountNo1) Then
            settings.AccountNo1 = Profile.PartnerInfo.Details.Account_No_1
            settings.AccountNo2 = Profile.PartnerInfo.Details.Account_No_2
        End If

        If String.IsNullOrEmpty(settings.AccountNo3.Trim) Then
            settings.AccountNo3 = ModuleDefaults.DEFAULT_COMPANY_CODE
        End If

        invoice.Dep = dei
        invoice.Settings = settings
        'Moved to inside the nvoice.InvoiceEnquiryDetail() call
        '-----------------------------------------------------
        'invoice.GetConnectionDetails(TalentCache.GetBusinessUnit)

        Dim err As Talent.Common.ErrorObj = invoice.InvoiceEnquiryDetail
        Dim dt As Data.DataTable
        If Not err.HasError Then

            Try
                dt = invoice.ResultDataSet.Tables("InvoiceEnquiryDetail")
                LoopItemsDT = invoice.ResultDataSet.Tables("InvoiceEnquiryDetail")
                'Store the Invoice Products Table in a Public Table for access by the pdf builder
                pds.DataSource = dt.DefaultView

            Catch ex As Exception
            End Try
        Else

            dt = New Data.DataTable
            pds.DataSource = dt.DefaultView
            LoopItemsDT = dt

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
        BindInvoiceDetailView()
        DisplayPagerNav(pds)
    End Sub

    Protected Sub BindInvoiceDetailView()
        InvoiceHistoryView.DataSource = SetUpPagedView()
        InvoiceHistoryView.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BindInvoiceDetailView()
        If Not Page.IsPostBack Then
            DisplayPagerNav(pds)
            GetDeliveryDetails()
        End If
        SetText()
    End Sub

    Protected Sub SetText()
        With ucr
            invDateLabel.Text = .Content("invoiceDateLabel", _languageCode, True)
            custAccLabel.Text = .Content("CustomerAccountLabel", _languageCode, True)
            custRefLabel.Text = .Content("CustomerReferenceLabel", _languageCode, True)
            invAddressLabel.Text = .Content("InvoiceAddressLabel", _languageCode, True)
            delAddressLabel.Text = .Content("DeliveryAddressLabel", _languageCode, True)
            subTotalLabel.Text = .Content("subTotalLabel", _languageCode, True)
            vatLabel.Text = .Content("vatLabel", _languageCode, True)
            totalLabel.Text = .Content("totalLabel", _languageCode, True)
        End With

    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Try

            With ucr
                Select Case sender.ID
                    Case Is = "InvoiceIDHeader"
                        CType(sender, Label).Text = .Content("InvoiceIDHeader", _languageCode, True)
                    Case Is = "ProductCodeHeader"
                        CType(sender, Label).Text = .Content("ProductCodeHeader", _languageCode, True)
                    Case Is = "DescriptionHeader"
                        CType(sender, Label).Text = .Content("DescriptionHeader", _languageCode, True)
                    Case Is = "PriceHeader"
                        CType(sender, Label).Text = .Content("PriceHeader", _languageCode, True)
                    Case Is = "QuantityHeader"
                        CType(sender, Label).Text = .Content("QuantityHeader", _languageCode, True)
                    Case Is = "ValueHeader"
                        CType(sender, Label).Text = .Content("ValueHeader", _languageCode, True)

                End Select
            End With
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub btnCreatePDF_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreatePDF.Click

        Dim DefaultDict As New Generic.Dictionary(Of String, String)

        'fix currency problem
        Dim fix = subTotal.Text
        Dim subTotalText = Server.HtmlDecode(fix)

        Dim fix2 = vat.Text
        Dim vatText = Server.HtmlDecode(fix2)

        Dim fix3 = total.Text
        Dim totalText = Server.HtmlDecode(fix3)

        Dim layoutFile As String = "..\..\PDF" & ModuleDefaults.pdfUrl & ucr.BusinessUnit & "\Layout\Invoice1.bmp"
        'check fileExists
        If Not System.IO.File.Exists(Server.MapPath(layoutFile)) Then
            layoutFile = "..\..\PDF" & ModuleDefaults.pdfUrl & "\Layout\Invoice1.bmp"
        End If

        'Add Values
        With DefaultDict
            .Add("*BGIMAGE*", Server.MapPath(layoutFile))
            .Add("*INVOICENUMBER*", "INVOICE: " & Request("hid"))
            .Add("*INVOICEDATE*", invDate.Text)
            .Add("*CUSTACNO*", custAccNo.Text)
            .Add("*CUSTOMERREF*", custRef.Text)
            .Add("*INVOICEADD0*", invName.Text)
            .Add("*INVOICEADD1*", invAdd1.Text)
            .Add("*INVOICEADD2*", invAdd2.Text)
            .Add("*INVOICEADD3*", invAdd3.Text)
            .Add("*INVOICEADD4*", invAdd4.Text)
            .Add("*INVOICEADD5*", invAdd5.Text)
            .Add("*INVOICEADD6*", invAdd6.Text)
            .Add("*INVOICEADD7*", invAdd7.Text)
            .Add("*DELADD0*", delName.Text)
            .Add("*DELADD1*", delAdd1.Text)
            .Add("*DELADD2*", delAdd2.Text)
            .Add("*DELADD3*", delAdd3.Text)
            .Add("*DELADD4*", delAdd4.Text)
            .Add("*DELADD5*", delAdd5.Text)
            .Add("*DELADD6*", delAdd6.Text)
            .Add("*DELADD7*", delAdd7.Text)
            .Add("*SUBTOTAL*", subTotalText)
            .Add("*VAT*", vatText)
            .Add("*TOTAL*", totalText)
        End With

        ' Save as Text file
        Dim xmlfilename As String = Server.MapPath("..\..\PDF" & ModuleDefaults.pdfUrl & ucr.BusinessUnit & "\XML\Invoice.xml")
        'check fileExists
        If Not System.IO.File.Exists(xmlfilename) Then
            xmlfilename = "..\..\PDF" & ModuleDefaults.pdfUrl & "\XML\Invoice.xml"
        End If

        ' createPDF.Main(args)
        Dim workingFolder = ucr.Attribute("TempFolder")
        Dim PDF As New Talent.eCommerce.CreatePDF(DefaultDict, LoopItemsDT, ucr.Content("EmailSubject", _languageCode, True), ucr.Content("EmailMessage", _languageCode, True))
        PDF.Main(xmlfilename, workingFolder)

        FeedbackLabel.Text = ucr.Content("EmailConfirmation", _languageCode, True)
        btnCreatePDF.Visible = False

    End Sub

    Protected Function CalcLineValue(ByVal calcLineAmount As Decimal, ByVal netLineAmount As String) As Decimal
        If netLineAmount > 0 Then
            Return netLineAmount
        Else
            Return calcLineAmount
        End If
    End Function

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
