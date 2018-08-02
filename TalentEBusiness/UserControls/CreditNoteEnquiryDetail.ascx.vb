Imports Talent.eCommerce

Partial Class UserControls_CreditNoteEnquiryDetail
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
            .KeyCode = "CreditNoteEnquiryDetail.ascx"
        End With
        CType(Me.Page, TalentBase01).PageHeaderText = "<h1>" & ucr.Content("PageHeaderText", _languageCode, True) & Request("hid") & "</h1>"
    End Sub

    Protected Sub GetDeliveryDetails()
        Dim creditnote As New Talent.Common.TalentCreditNote
        Dim dei As New Talent.Common.DECreditNote(TalentCache.GetBusinessUnit, Profile.UserName)

        dei.CreditNoteNumber = Request("hid")

        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        settings.Partner = Profile.PartnerInfo.Details.Partner
        creditnote.Dep = dei
        creditnote.Settings = settings
        'CreditNote.GetConnectionDetails(TalentCache.GetBusinessUnit)

        Dim err As Talent.Common.ErrorObj = creditnote.GetCreditNoteHeader

        Dim dt As Data.DataTable = creditnote.ResultDataSet.Tables("CreditNoteEnquiryHeader")
        Dim dr As Data.DataRow
        If dt.Rows.Count > 0 Then
            dr = dt.Rows(0)
            invDate.Text = Talent.Common.Utilities.CheckForDBNull_DateTime(dr("CreditNoteDateTime"))
            custAccNo.Text = Utilities.CheckForDBNull_String(dr("AccountNumber"))
            custOrderNo.Text = Utilities.CheckForDBNull_String(dr("OriginalOrderNo"))
            invDate.Text = Utilities.CheckForDBNull_Date(dr("OriginalOrderDate")).ToString
            invNo.Text = Utilities.CheckForDBNull_String(dr("CreditNoteNumber"))
            buRef.Text = Utilities.CheckForDBNull_String(dr("OrderNumber"))
            despDate.Text = ""
            salesArea.Text = ""
            shipNo.Text = ""

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

            subTotal.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("CreditNoteAmount")) - Utilities.CheckForDBNull_String(dr("VatAmount")), ucr.BusinessUnit, ucr.PartnerCode)
            total.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("CreditNoteAmount")), ucr.BusinessUnit, ucr.PartnerCode)
            vat.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dr("VatAmount")), ucr.BusinessUnit, ucr.PartnerCode)

        End If

    End Sub

    Protected Function SetUpPagedView() As PagedDataSource
        pds.AllowPaging = True
        pds.PageSize = CInt(ucr.Content("ResultsViewPageSize", _languageCode, True))

        Dim CreditNote As New Talent.Common.TalentCreditNote
        Dim dei As New Talent.Common.DECreditNote(Request("hid"))

        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        settings.AccountNo1 = Profile.User.Details.Account_No_1
        settings.AccountNo2 = Profile.User.Details.Account_No_2
        If Not Profile.PartnerInfo.Details.Account_No_3 Is Nothing Then
            settings.AccountNo3 = Profile.PartnerInfo.Details.Account_No_3
        End If
        settings.Cacheing = Profile.PartnerInfo.Details.Caching_Enabled
        settings.CacheTimeMinutes = Profile.PartnerInfo.Details.Cache_Time_Minutes
        settings.Partner = Profile.PartnerInfo.Details.Partner

        If String.IsNullOrEmpty(settings.AccountNo1) Then
            settings.AccountNo1 = Profile.PartnerInfo.Details.Account_No_1
            settings.AccountNo2 = Profile.PartnerInfo.Details.Account_No_2
        End If

        If String.IsNullOrEmpty(settings.AccountNo3.Trim) Then
            settings.AccountNo3 = ModuleDefaults.DEFAULT_COMPANY_CODE
        End If
        CreditNote.Dep = dei
        CreditNote.Settings = settings

        Dim err As Talent.Common.ErrorObj = CreditNote.CreditNoteEnquiryDetail
        ' If Not err.HasError Then
        Dim dt As Data.DataTable = CreditNote.ResultDataSet.Tables("CreditNoteEnquiryDetail")
        pds.DataSource = dt.DefaultView
        'End If

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
        BindCreditNoteDetailView()
        DisplayPagerNav(pds)
    End Sub


    Protected Sub BindCreditNoteDetailView()
        Try
            CreditNoteHistoryView.DataSource = SetUpPagedView()
            CreditNoteHistoryView.DataBind()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BindCreditNoteDetailView()
        If Not Page.IsPostBack Then
            DisplayPagerNav(pds)
            GetDeliveryDetails()
        End If
        SetText()
    End Sub

    Protected Sub SetText()
        With ucr
            invDateLabel.Text = .Content("CreditNoteDateLabel", _languageCode, True)
            custAccLabel.Text = .Content("CustomerAccountLabel", _languageCode, True)
            custOrderNoLabel.Text = .Content("CustomerOrderNoLabel", _languageCode, True)
            invNoLabel.Text = .Content("CreditNoteInvoiceNumber", _languageCode, True)
            buRefLabel.Text = .Content("webReferenceLabel", _languageCode, True)
            despDateLabel.Text = .Content("DespatchDateLabel", _languageCode, True)
            salesAreaLabel.Text = .Content("SalesAreaLabel", _languageCode, True)
            shipNoLabel.Text = .Content("ShipmentNoLabel", _languageCode, True)
            invAddressLabel.Text = .Content("CreditNoteAddressLabel", _languageCode, True)
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

    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        FormatCurrency = TDataObjects.PaymentSettings.FormatCurrency(value, ucr.BusinessUnit, ucr.PartnerCode)
        Return formattedString
    End Function

End Class
