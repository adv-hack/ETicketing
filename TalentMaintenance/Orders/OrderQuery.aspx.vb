Imports System.Data
Imports Talent.Common

Partial Class Orders_OrderQuery
    Inherits PageControlBase

    Const BUSINESSUNIT As String = "MAINTENANCE"
    Const PARTNER As String = "*ALL"
    Const PAGECODE As String = "OrderQuery.aspx"
    Const HYPHEN As String = "-"
    Const ERRORCSSCLASSNAME As String = "error"
    Const HTTPPATHTOCSVFILE As String = "~/Orders/OrdersCSV/orders.csv"
    Const LINKTODOWNLOADFILE As String = "OrdersCSV/orders.csv"
    Const COMMA As String = ","

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = BUSINESSUNIT
            .PartnerCode = PARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With

        Page.Title = _wfrPage.Content("PageTitleText", _languageCode, True)
        ltlPageHeader.Text = _wfrPage.Content("PageHeaderText", _languageCode, True)
        lblFromDate.Text = _wfrPage.Content("FromDateLabel", _languageCode, True)
        lblFromMonth.text = _wfrPage.Content("FromMonthLabel", _languageCode, True)
        lblFromYear.Text = _wfrPage.Content("FromYearLabel", _languageCode, True)
        lblToDate.Text = _wfrPage.Content("ToDateLabel", _languageCode, True)
        lblToMonth.Text = _wfrPage.Content("ToMonthLabel", _languageCode, True)
        lblToYear.Text = _wfrPage.Content("ToYearLabel", _languageCode, True)
        regFromDate.ErrorMessage = _wfrPage.Content("InvalidFromDate", _languageCode, True)
        rfvFromDate.ErrorMessage = _wfrPage.Content("InvalidFromDate", _languageCode, True)
        regFromMonth.ErrorMessage = _wfrPage.Content("InvalidFromMonth", _languageCode, True)
        rfvFromMonth.ErrorMessage = _wfrPage.Content("InvalidFromMonth", _languageCode, True)
        regFromYear.ErrorMessage = _wfrPage.Content("InvalidFromYear", _languageCode, True)
        rfvFromYear.ErrorMessage = _wfrPage.Content("InvalidFromYear", _languageCode, True)
        regToDate.ErrorMessage = _wfrPage.Content("InvalidToDate", _languageCode, True)
        rfvToDate.ErrorMessage = _wfrPage.Content("InvalidToDate", _languageCode, True)
        regToMonth.ErrorMessage = _wfrPage.Content("InvalidToMonth", _languageCode, True)
        rfvToMonth.ErrorMessage = _wfrPage.Content("InvalidToMonth", _languageCode, True)
        regToYear.ErrorMessage = _wfrPage.Content("InvalidToYear", _languageCode, True)
        rfvToYear.ErrorMessage = _wfrPage.Content("InvalidToYear", _languageCode, True)
        btnGetOrders.Text = _wfrPage.Content("SubmitButtonText", _languageCode, True)
    End Sub

    Protected Sub btnGetOrders_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetOrders.Click
        Dim ordersTable As New DataTable
        Dim fromDate As New StringBuilder
        Dim toDate As New StringBuilder
        plhResults.Visible = True
        fromDate.Append(txtFromYear.Text)
        fromDate.Append(HYPHEN)
        fromDate.Append(txtFromMonth.Text)
        fromDate.Append(HYPHEN)
        fromDate.Append(txtFromDate.Text)
        toDate.Append(txtToYear.Text)
        toDate.Append(HYPHEN)
        toDate.Append(txtToMonth.Text)
        toDate.Append(HYPHEN)
        toDate.Append(txtToDate.Text)
        ordersTable = TDataObjects.OrderSettings.GetAllCompleteOrdersByDate(fromDate.ToString(), toDate.ToString())
        If ordersTable.Rows.Count > 0 Then
            If createCSVFile(ordersTable) Then
                Dim csvFileCreatedString As New StringBuilder
                csvFileCreatedString.Append("<a href=""")
                csvFileCreatedString.Append(LINKTODOWNLOADFILE)
                csvFileCreatedString.Append(""">")
                csvFileCreatedString.Append(_wfrPage.Content("CSVFileCreatedMessage", _languageCode, True))
                csvFileCreatedString.Append("</a>")
                lblResults.Text = csvFileCreatedString.ToString()
            Else
                lblResults.Text = _wfrPage.Content("ErrorCreatingCSVFile", _languageCode, True)
                lblResults.CssClass = ERRORCSSCLASSNAME
            End If
        Else
            lblResults.Text = _wfrPage.Content("NoOrdersMessage", _languageCode, True)
            lblResults.CssClass = ERRORCSSCLASSNAME
        End If
    End Sub

    Private Function createCSVFile(ByVal ordersTable As DataTable) As Boolean
        Dim fileCreated As Boolean = False
        Dim writer As System.IO.StreamWriter = Nothing
        Try
            deleteLastFile()
            writer = New System.IO.StreamWriter(Server.MapPath(HTTPPATHTOCSVFILE))

            Dim builder As New System.Text.StringBuilder
            For Each col As DataColumn In ordersTable.Columns
                builder.Append(COMMA).Append(col.ColumnName)
            Next
            writer.WriteLine(builder.ToString())

            For Each row As DataRow In ordersTable.Rows
                builder = New System.Text.StringBuilder
                For Each col As DataColumn In ordersTable.Columns
                    builder.Append(COMMA).Append(row(col.ColumnName))
                Next
                writer.WriteLine(builder.ToString())
            Next
        Catch ex As Exception
            fileCreated = False
        Finally
            If writer IsNot Nothing Then
                writer.Close()
                fileCreated = True
            Else
                fileCreated = False
            End If
        End Try
        Return fileCreated
    End Function

    Private Sub deleteLastFile()
        Dim fileNameWithPath As String = String.Empty
        fileNameWithPath = Server.MapPath(HTTPPATHTOCSVFILE)
        If FileIO.FileSystem.FileExists(fileNameWithPath) Then
            FileIO.FileSystem.DeleteFile(fileNameWithPath)
        End If
    End Sub
End Class
