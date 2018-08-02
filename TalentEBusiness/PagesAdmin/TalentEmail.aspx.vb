Imports Microsoft.VisualBasic
Imports Talent.Common
Imports IBM.Data.DB2.iSeries
Imports System.Data
Imports System.Collections.Generic
'added
Imports iTextSharp.tool
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports Newtonsoft.Json

Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Routing
Imports System.Web.Mvc
Imports System.IO

Partial Class PagesAdmin_TalentEmail
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If HttpContext.Current.Request.QueryString().Count > 0 Then
        Else

        End If
        Dim tempDateStr As String = "30 09 2012"
        Dim tempDate As DateTime = Date.Parse(tempDateStr.ToString)
        Response.Write(GetFormattedDate(tempDate.ToString(), "Full", ""))
        Response.Write("<br>")
        Response.Write(GetFormattedDate(tempDate.ToString(), "truncated", ""))
        Response.Write("<br>")
        tempDateStr = "2 09 2012"
        tempDate = Date.Parse(tempDateStr.ToString)
        Response.Write(GetFormattedDate(tempDate.ToString(), "Full", ""))
        Response.Write("<br>")
        Response.Write(GetFormattedDate(tempDate.ToString(), "truncated", ""))
        Response.Write("<br>")
        tempDateStr = "3 09 2012"
        tempDate = Date.Parse(tempDateStr.ToString)
        Response.Write(GetFormattedDate(tempDate.ToString(), "Full", ""))
        Response.Write("<br>")
        Response.Write(GetFormattedDate(tempDate.ToString(), "truncated", ""))
        Response.Write("<br>")
        tempDateStr = "23 09 2012"
        tempDate = Date.Parse(tempDateStr.ToString)
        Response.Write(GetFormattedDate(tempDate.ToString(), "Full", ""))
        Response.Write("<br>")
        Response.Write(GetFormattedDate(tempDate.ToString(), "truncated", ""))
        Response.Write("<br>")
        tempDateStr = "24 09 2012"
        tempDate = Date.Parse(tempDateStr.ToString)
        Response.Write(GetFormattedDate(tempDate.ToString(), "Full", ""))
        Response.Write("<br>")
        Response.Write(GetFormattedDate(tempDate.ToString(), "truncated", ""))
        Response.Write("<br>")
        Response.Write("<br>")
        Response.Write("<br>")
        Response.Write("<br>")
        Dim s As Decimal = 1
        Dim write As String = s.ToString("F0")
        Response.Write(write)

        'Dim _cmd As New iDB2Command
        'Dim conTALENTTKT As New iDB2Connection
        'conTALENTTKT.ConnectionString = "DataSource=172.26.49.17;userid=TKT472TF;password=TKT472TF;LibraryList=*USRLIBL;Naming=System;MinimumPoolSize=2;MaximumPoolSize=100;"
        'conTALENTTKT.Open()
        '_cmd = conTALENTTKT.CreateCommand()
        '_cmd.CommandText = "CALL ARCTEST"
        '_cmd.CommandType = CommandType.Text

        'Dim _cmdAdapter As New iDB2DataAdapter
        'Dim ds As New DataSet

        'Dim dtUsers As New DataTable("Table")
        'ds.Tables.Add(dtUsers)
        'With dtUsers.Columns
        '    .Add("UserCode", GetType(String))
        '    .Add("UserName", GetType(String))
        'End With

        ''Create the Customer Activities Details data table
        'Dim dtQuestionsText As New DataTable("Table1")
        'With dtQuestionsText.Columns
        '    .Add("CustomerNumber", GetType(String))
        '    .Add("QuestionText", GetType(String))
        'End With
        'ds.Tables.Add(dtQuestionsText)

        '_cmdAdapter.SelectCommand = _cmd
        '_cmdAdapter.Fill(ds)

        'conTALENTTKT.Close()
        'printPDF()
        'RunAsync().GetAwaiter().GetResult()
        'callModelBuilder()
    End Sub

    Private Function GetFormattedDate(ByVal dateToFormat As String, ByVal formatType As String, ByVal dotnetFormat As String) As String
        Dim formattedDate As String = dateToFormat
        Try
            Dim tempDate As Date = CDate(dateToFormat)
            Select Case formatType.ToUpper()
                Case "FULL"
                    Dim fullFormatDate As String = tempDate.ToString("dddd")
                    fullFormatDate = fullFormatDate & " " & GetDateSuffix(CInt(tempDate.ToString("dd")))
                    fullFormatDate = fullFormatDate & " " & tempDate.ToString("MMMM")
                    fullFormatDate = fullFormatDate & " " & tempDate.ToString("yyyy")
                    formattedDate = fullFormatDate
                Case "TRUNCATED"
                    Dim fullFormatDate As String = tempDate.ToString("ddd")
                    fullFormatDate = fullFormatDate & " " & GetDateSuffix(CInt(tempDate.ToString("dd")))
                    fullFormatDate = fullFormatDate & " " & tempDate.ToString("MMM")
                    fullFormatDate = fullFormatDate & " " & tempDate.ToString("yyyy")
                    formattedDate = fullFormatDate
                Case Else
                    formattedDate = tempDate.ToString(dotnetFormat)
            End Select
        Catch ex As Exception
            formattedDate = dateToFormat
        End Try
        Return formattedDate
    End Function

    Private Function GetDateSuffix(ByVal dateValue As Integer) As String
        Dim dateSuffix As String = "th"
        Dim ones As Integer = dateValue Mod 10
        Dim tens As Integer = CInt(Math.Floor(dateValue / 10D)) Mod 10
        If tens = 1 Then
            dateSuffix = "th"
        Else
            Select Case ones
                Case 1
                    dateSuffix = "st"
                Case 2
                    dateSuffix = "nd"
                Case 3
                    dateSuffix = "rd"
                Case Else
                    dateSuffix = "th"
            End Select
        End If
        Return String.Format("{0}{1}", dateValue, dateSuffix)
    End Function
    Protected Sub btnTicketingEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTicketingEmail.Click
        Dim email As New TalentEmail
        Dim TCErr As New ErrorObj
        Dim mySettings As New DESettings
        Dim gl As New Generic.List(Of String)
        'gl.Add("G:\TalentEBusinessSuiteAssets\TalentDev\BOXOFFICE\HTML\HospitalityPDF\3311-11052018-1401.pdf")
        'mySettings.FrontEndConnectionString = "Data Source=talentserver2008test;Initial Catalog=TalentEBusinessDBTalentQANext; User ID=TalentAdminUser; password=Password2007;"
        'mySettings.FrontEndConnectionString = "Data Source=WIN-72A9D09Q7EG;Initial Catalog=TEST_TalentEBusinessDBTalentQATest; User ID=TEST_TalentEbusinessTalentQATestUser; password=Password2007;"
        'mySettings.FrontEndConnectionString = "Data Source=WIN-72A9D09Q7EG;Initial Catalog=TEST_TalentEBusinessDBTalentQAReady; User ID=TEST_TalentEbusinessTalentQAReadyUser; password=Password2007;"
        'mySettings.FrontEndConnectionString = "Data Source=WIN-72A9D09Q7EG;Initial Catalog=TEST_TalentEBusinessDBQABekoB2C; User ID=TEST_TalentEBusinessQABekoB2CUser; password=Password2007;"
        mySettings.FrontEndConnectionString = "Data Source=talentserver2008test;Initial Catalog=TalentEBusinessDBTalentDev; User ID=TalentAdminUser; password=Password2007;"
        'mySettings.BusinessUnit = "BEKO"
        mySettings.BusinessUnit = "UNITEDKINGDOM"
        mySettings.StoredProcedureGroup = "WS_V248T"
        mySettings.Partner = "IRIS"
        Dim De As New DEEmailSettings
        De.FromAddress = "iris-ticketing@iris.co.uk"
        De.ToAddress = "alexander.carr@oneadvanced.com"
        De.CCAddress = ""
        De.SmtpServer = "127.0.0.1"
        De.SmtpServerPort = "25"
        De.Attachments = gl
        With email
            .EmailSettings = De
            .Settings = mySettings
            .RetryFailure = True
            .TicketPath = ""
            .Settings.OriginatingSourceCode = "W"

            'PPS Emails
            .EmailSettings.PPSPayment.Customer = "000000010515"
            .EmailSettings.PPSPayment.Description = "Manchester United vs. Chelsea"
            .EmailSettings.PPSPayment.Turnstiles = "A-K"
            .EmailSettings.PPSPayment.Gates = "1-4"
            .EmailSettings.PPSPayment.Seat = "S  A   P   0005 "
            .EmailSettings.PPSPayment.PaymentValue = "50.00"
            '.EmailSettings.AmendPPS.Customer = "000000005450"
            '.EmailSettings.AmendPPS.ProductCode = "PPS2"
            'TCErr = .SendAmendPPSEmail()
            'TCErr = .SendAmendPPSPaymentConfirmationEmail()
            'TCErr = .SendPPSPaymentFailureEmail()
            TCErr = .SendPPSPaymentConfirmationEmail()

            'Order Confirmation Email
            '.EmailSettings.OrderConfirmation.Customer = "000000000318"
            '.EmailSettings.OrderConfirmation.PaymentReference = "4291"
            'TCErr = .SendTicketingConfirmationEmail

            '.EmailSettings.HospitalityQAReminderEmail.Customer = "317"
            '.EmailSettings.HospitalityQAReminderEmail.CallId = "6865"
            'TCErr = .SendHospitalityQAndAReminder

            'Forgotten password Email
            '.EmailSettings.ForgottenPassword.Customer = "5"
            'TCErr = .SendForgottenPasswordEmail()
            'TCErr = .SendOrderReturnConfirmationEmail()
            'TCErr = .SendTicketingCancelAmmendConfirmationEmail("TicketingCancel")

            'Customer registration Email
            '.EmailSettings.CustomerRegistration.Customer = "5"
            'TCErr = .SendCustomerRegistrationEmail()

            'Ticket exchange confirmation
            '.EmailSettings.TicketExchangeConfirmation.Customer = "000000011347"
            '.EmailSettings.TicketExchangeConfirmation.TicketExchangeReference = 902
            'TCErr = .SendTicketExchangeConfirmationEmail


            'Ticket exchange sale confirmation
            '.EmailSettings.TicketExchangeSaleConfirmation.Customer = "000000002627"
            '.EmailSettings.TicketExchangeSaleConfirmation.PaymentReference = 21086
            'TCErr = .SendTicketExchangeSaleConfirmationEmail


            'CAT
            '.EmailSettings.OrderConfirmation.PaymentReference = "29837"
            '.EmailSettings.OrderConfirmation.Customer = "000000010515"
            'TCErr = .SendTicketingCancelAmmendConfirmationEmail("TicketingUpgrade")


        End With
    End Sub

    Protected Sub btnRetailEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRetailEmail.Click
        Dim orderEmail As New Talent.eCommerce.Order_Email
        orderEmail.SendConfirmationEmail("karthikeyan.chokanathan@iris.co.uk")
    End Sub

    Private Sub printPDF(ByVal html As String)
        Dim bytes As Byte()
        'Dim dl As New WebClient
        Dim css As String = ".headline{font-size:200%; colour:red}"
        'Dim html As String = "<p>This <em>is </em><span class=""headline"" style=""text-decoration: underline;"">some</span> <strong>sample <em> text</em></strong><span style=""color: red;"">!!!</span></p>"
        Dim doc As New Document
        Dim writer As PdfWriter = Nothing
        Dim htmlWorking As iTextSharp.text.html.simpleparser.HTMLWorker


        'html = dl.DownloadString("http://www.qaready.talent-sport.co.uk")
        'html = dl.DownloadString("http://www.qaready.talent-sport.co.uk/pagesadmin/talentemail.aspx")

        'EXAMPLE 1:
        'Using ms As New MemoryStream
        '    writer = PdfWriter.GetInstance(doc, ms)
        '    doc.Open()
        '    Using reader As New StringReader(html)
        '        htmlWorking = New iTextSharp.text.html.simpleparser.HTMLWorker(doc)
        '        htmlWorking.Parse(reader)
        '    End Using
        '    doc.Close()
        '    bytes = ms.ToArray
        'End Using

        'EXAMPLE 2:
        Using ms As New MemoryStream
            writer = PdfWriter.GetInstance(doc, ms)
            doc.Open()
            Dim stream1 As Stream
            'stream1.Read(System.Text.Encoding.UTF8.GetBytes(html), 0, System.Text.Encoding.UTF8.GetBytes(html))
            'System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(html))

            Using msHtml As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(html))
                Using msCss As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(css))
                    'iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss)
                    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss)

                End Using
            End Using
            doc.Close()
            bytes = ms.ToArray
        End Using

        'File.WriteAllBytes("C:\Temp\test.pdf", bytes)


        'iTextSharp.tool.xml.
        'doc.Open()
        'htmlWorking = New html.simpleparser.HTMLWorker(doc)
        'Using htmlWorking As New html.simpleparser.HTMLWorker(doc)


        'End Using

    End Sub

    Private Async Function RunAsync() As Threading.Tasks.Task
        Dim client As New HttpClient
        Dim bookingInputModel As New TalentBusinessLogic.Models.HospitalityBookingInputModel
        Dim httpContent As Object
        Dim response As HttpResponseMessage
        Dim serialiser As New JsonSerializer
        Dim responseContent As HttpContent

        'bookingInputModel.SessionID = Session.SessionID
        httpContent = JsonConvert.SerializeObject(bookingInputModel)
        httpContent = New StringContent(httpContent)

        client.BaseAddress = New Uri("http://localhost:1340")
        client.DefaultRequestHeaders.Accept.Clear()
        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        response = Await client.PostAsync(New Uri("http://localhost:1340/HospitalityBooking/RetrieveBookingDetails"), httpContent)

        If response.IsSuccessStatusCode Then
            responseContent = response.Content
        End If
    End Function

    Private Sub callModelBuilder()
        Dim tgateway As New TicketingGatewayFunctions
        tgateway.GetHospitalityPDFAttachmentList("4270")

    End Sub

End Class
