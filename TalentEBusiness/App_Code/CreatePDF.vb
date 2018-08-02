Imports System.Drawing.Printing
Imports System.IO
Imports System.Xml
Imports Microsoft.Win32
Imports System.Drawing
Imports Talent.Common
Imports iTextSharp.text.pdf
Imports iTextSharp.text

Namespace Talent.eCommerce

    Public Class CreatePDF

#Region "Class Level Fields"


        Private _ucr As UserControlResource = Nothing
        Private _moduleDefaults As ECommerceModuleDefaults = Nothing
        Private _def As ECommerceModuleDefaults.DefaultValues = Nothing
        Private _loopItemsPerPage As Integer
        Private _documentType As String
        Private _spacing As String
        Private _xElements As XmlNodeList
        Private _pdfFolder As String = String.Empty
        Private _sXMLFile As String = String.Empty
        Private _sFileName As String = String.Empty
        Private _sAttachmentFile As String = String.Empty
        Private _tempFolder As String = String.Empty
        Private _bSuccess As Boolean = True
        Private _curPage As Integer = 1
        Private _totalPages As Integer = 0
        Private _loopItemsPrinted As Integer = 0
        Private _sPDFFile As String = String.Empty
        Private _emailSubject As String
        Private _emailMessage As String
        Private _defaultDict As Generic.Dictionary(Of String, String)
        Private _loopDT As New Data.DataTable

#End Region

#Region "Constructor"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal DefaultDict As Generic.Dictionary(Of String, String), ByVal LoopItemsDT As Data.DataTable, ByVal EmailSubject As String, ByVal EmailMessage As String)
            MyBase.New()
            Me._defaultDict = DefaultDict
            Me._loopDT = LoopItemsDT
            Me._emailSubject = EmailSubject
            Me._emailMessage = EmailMessage
        End Sub

#End Region

#Region "Public Methods"

        Public Sub Main(ByVal file As String, ByVal workingFolder As String)

            _tempFolder = workingFolder

            setup()

            'Retrieve the xml file and generate the pdf name
            _sXMLFile = file

            'File and directory setup
            FileAndDirectory()

            'Set the registry
            SetRegistry()

            'Load the xml file
            Dim xDoc As XmlDocument = New XmlDocument()

            'config
            Dim xConfig As XmlNodeList = xDoc.GetElementsByTagName("Config")
            xDoc.Load(_sXMLFile)

            'config
            _xElements = xDoc.GetElementsByTagName("Item")
            _loopItemsPerPage = xConfig.Item(0).Attributes("loopItemsPerPage").InnerText
            _documentType = xConfig.Item(0).Attributes("docType").InnerText
            _spacing = xConfig.Item(0).Attributes("spacing").InnerText

            ' Make a PrintDocument object.
            Dim PdfDocument As PrintDocument = PreparePrintDocument()

            'Hide print dialog
            Dim printControl = New StandardPrintController
            PdfDocument.PrintController = printControl

            ' Print immediately
            PdfDocument.PrinterSettings.PrinterName = "CreatePDF"
            PdfDocument.Print()

            'Close the relevant files
            Talent.Common.Utilities.Email_Send(_ucr.Attribute("FromEmailAddress"), CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email, _emailSubject, _emailMessage, _sPDFFile)

        End Sub

#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Create a PDF document in the given location based on the parameters
        ''' </summary>
        ''' <param name="fileName">The PDF file name</param>
        ''' <param name="filePath">The path to create the file in</param>
        ''' <param name="htmlContent">The HTML Content used to create the file</param>
        ''' <param name="cssContent">The CSS Content used to create the file</param>
        ''' <returns>A created PDF document local file and path name</returns>
        ''' <remarks></remarks>
        Public Function CreateFile(ByVal fileName As String, ByVal filePath As String, ByVal htmlContent As String, ByVal cssContent As String) As String
            Dim createdLocalFilePathAndName As String = String.Empty
            Dim doc As New Document
            Dim writer As PdfWriter = Nothing
            Dim bytes As Byte()
            Try
                Using ms As New MemoryStream
                    writer = PdfWriter.GetInstance(doc, ms)
                    doc.Open()
                    Using msHtml As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent))
                        Using msCss As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(cssContent))
                            iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msHtml, msCss)
                        End Using
                    End Using
                    doc.Close()
                    bytes = ms.ToArray
                End Using
                If bytes.Length > 0 Then
                    Dim filePathAndName = String.Empty
                    If filePath.EndsWith("\") Then
                        filePathAndName = filePath & fileName
                    Else
                        filePathAndName = filePath & "\" & fileName
                    End If
                    File.WriteAllBytes(filePathAndName, bytes)
                    createdLocalFilePathAndName = filePathAndName
                End If
            Catch ex As Exception
                Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "CreatePDF-Error", ex.StackTrace, ex.Message)
            Finally
                writer.Dispose()
                doc.Dispose()
            End Try
            Return createdLocalFilePathAndName
        End Function

#End Region

#Region "Private Methods"

        Private Sub setup()
            _def = _moduleDefaults.GetDefaults()
            _ucr = New UserControlResource
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "InvoiceEnquiryDetail.ascx"
            End With
        End Sub

        Private Sub FileAndDirectory()

            'create bizunit folder
            Dim businessUnitFolder As String
            businessUnitFolder = HttpContext.Current.Server.MapPath("~\PDF" & _def.pdfUrl & _ucr.BusinessUnit & "\")

            If System.IO.Directory.Exists(businessUnitFolder) Then
                _pdfFolder = businessUnitFolder
            Else
                _pdfFolder = "~\PDF" & _def.pdfUrl
            End If

            'create temp folder
            If Not System.IO.Directory.Exists(_tempFolder) Then
                System.IO.Directory.CreateDirectory(_tempFolder)
            End If

            ' Construct the PDF name
            _sAttachmentFile = _tempFolder + "PDF\"

            ' The PDF directory must exist
            If Not System.IO.Directory.Exists(_sAttachmentFile) Then
                System.IO.Directory.CreateDirectory(_sAttachmentFile)
            End If

        End Sub

        Private Sub SetRegistry()

            ' Create the sub key CreatePDF 
            Dim key As RegistryKey = Registry.CurrentUser.OpenSubKey("Software", True)
            Dim newkey As RegistryKey = key.CreateSubKey("CreatePDF")

            'Make Unique PDF Name
            Dim guidStr As String = System.Guid.NewGuid.ToString

            ' Set value of the two string values
            newkey.SetValue("OutputFile", _sAttachmentFile & guidStr & ".pdf")
            newkey.SetValue("BypassSaveAs", "1")

            ' Save PDF File Name
            _sPDFFile = _sAttachmentFile & guidStr & ".pdf"

        End Sub

        Private Sub CalculatePaging()
            'Calculate Number of Elements Per Page
            If _loopDT.Rows.Count Mod _loopItemsPerPage = 0 Then
                _totalPages = (_loopDT.Rows.Count \ _loopItemsPerPage)
            Else
                _totalPages = (_loopDT.Rows.Count \ _loopItemsPerPage) + 1
            End If
        End Sub

        Private Sub Print_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs)

            'set canvas
            Dim canvas As Graphics = e.Graphics
            canvas.PageUnit = GraphicsUnit.Millimeter
            canvas.PageScale = 1.0F

            'Loop the table adding the elements to the canvas
            Dim i As Integer = 0

            Do While i < _xElements.Count
                If (_xElements.Item(i).Attributes("Type").InnerText = "DefaultText") Then
                    'Add Default Element
                    AddTextElement(getValue(_xElements.Item(i).Attributes("Value").InnerText, i, "default"), _xElements.Item(i).Attributes("Xpos").InnerText, _xElements.Item(i).Attributes("Ypos").InnerText, _xElements.Item(i).Attributes("Font").InnerText, _xElements.Item(i).Attributes("Size").InnerText, _xElements.Item(i).Attributes("Colour").InnerText, canvas)
                ElseIf (_xElements.Item(i).Attributes("Type").InnerText = "LoopText") Then
                    'Add Loop Table
                    If (_loopItemsPrinted < _loopDT.Rows.Count) Then
                        'store loop attributes
                        Dim loopStart As Integer = 0
                        Dim loopEnd As Integer = 0

                        loopStart = _loopItemsPrinted
                        loopEnd = _loopItemsPrinted + _loopItemsPerPage - 1

                        Dim Column As String = _xElements.Item(i).Attributes("Value").InnerText
                        Dim Xpos As String = _xElements.Item(i).Attributes("Xpos").InnerText
                        Dim Ypos As String = _xElements.Item(i).Attributes("Ypos").InnerText
                        Dim Font As String = _xElements.Item(i).Attributes("Font").InnerText
                        Dim Size As String = _xElements.Item(i).Attributes("Size").InnerText
                        Dim Colour As String = _xElements.Item(i).Attributes("Colour").InnerText

                        Dim yPosCounter As Integer = Ypos
                        For iCount As Integer = loopStart To loopEnd

                            AddTextElement(getValue(Column, iCount, "loop"), _
                                                    Xpos, _
                                                    yPosCounter, _
                                                    Font, _
                                                    Size, _
                                                    Colour, _
                                                    canvas)

                            yPosCounter += _spacing
                            If i = _xElements.Count - 1 Then _loopItemsPrinted += 1
                        Next
                    End If
                    '  loopItemsPrinted += 1
                ElseIf (_xElements.Item(i).Attributes("Type").InnerText = "Image") Then
                    'Add Image
                    AddImageElement(_xElements.Item(i).Attributes("Value").InnerText, _xElements.Item(i).Attributes("Xpos").InnerText, _xElements.Item(i).Attributes("Ypos").InnerText, canvas)
                End If
                i += 1
            Loop

            ' Print Products
            ' Loop the table adding the elements to the canvas
            Do While _loopItemsPrinted < _loopItemsPerPage * _curPage
                Try
                    Dim curRowType2 = _loopDT.Rows(_loopItemsPrinted).Item("Type")
                    Select Case curRowType2
                        Case "LoopText"
                            AddTextElement(_loopDT.Rows(_loopItemsPrinted).Item("Value"), _loopDT.Rows(_loopItemsPrinted).Item("Xpos"), _loopDT.Rows(_loopItemsPrinted).Item("Ypos"), _loopDT.Rows(_loopItemsPrinted).Item("Font"), _loopDT.Rows(_loopItemsPrinted).Item("Size"), _loopDT.Rows(_loopItemsPrinted).Item("Colour"), canvas)
                    End Select
                Catch ex As Exception
                End Try
                _loopItemsPrinted += 1
            Loop

            ' Check for anymore pages left.
            If (_curPage < _totalPages) Then
                e.HasMorePages = True

                Dim myBrush As Brush = New SolidBrush(Color.Black)
                Dim myFont As System.Drawing.Font = New System.Drawing.Font("Arial", 10)
                canvas.DrawString("Page " & _curPage & " of " & _totalPages, myFont, myBrush, 180, 280)

                _curPage += 1
            Else
                e.HasMorePages = False
                If (_totalPages = 0) Then
                    _totalPages = 1
                End If
                Dim myBrush As Brush = New SolidBrush(Color.Black)
                Dim myFont As System.Drawing.Font = New System.Drawing.Font("Arial", 10)
                canvas.DrawString("Page " & _curPage & " of " & _totalPages, myFont, myBrush, 180, 280)
            End If

        End Sub

        Private Sub AddTextElement(ByVal drawText As String, ByVal drawX As String, ByVal drawY As String, ByVal drawFont As String, ByVal drawSize As String, ByVal drawColour As String, ByRef Canvas As System.Drawing.Graphics)

            'Set the Font
            Dim myFont As System.Drawing.Font = New System.Drawing.Font(drawFont, CInt(drawSize))

            'Set the colour
            Dim myBrush As Brush = New SolidBrush(Color.Black)
            Select Case drawColour
                Case "vbRed" : myBrush = New SolidBrush(Color.Red)
                Case "vbBlue" : myBrush = New SolidBrush(Color.Blue)
                Case "vbWhite" : myBrush = New SolidBrush(Color.White)
                Case "vbGreen" : myBrush = New SolidBrush(Color.Green)
                Case "vbYellow" : myBrush = New SolidBrush(Color.Yellow)
                Case "vbMagenta" : myBrush = New SolidBrush(Color.Magenta)
                Case "vbCyan" : myBrush = New SolidBrush(Color.Cyan)
            End Select

            'Write the text element to the string
            Canvas.DrawString(drawText, myFont, myBrush, CInt(drawX), CInt(drawY))

        End Sub

        Private Sub AddImageElement(ByVal drawImage As String, ByVal drawX As String, ByVal drawY As String, ByRef Canvas As System.Drawing.Graphics)
            Try
                ' Map Image
                Dim imageFile As String = HttpContext.Current.Server.MapPath("~\PDF\" & _def.pdfUrl & _ucr.BusinessUnit & "\Layout\" & drawImage)

                'Load the image
                Dim myImage As Bitmap = New Bitmap(imageFile)

                'Paint the picture
                Canvas.DrawImage(myImage, CInt(drawX), CInt(drawY))
            Catch ex As Exception
            End Try
        End Sub

#End Region

#Region "Private Functions"

        ''' <summary>
        ''' Make and return a PrintDocument object that's ready to print the paragraphs.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function PreparePrintDocument() As PrintDocument

            ' Prepare Print Document
            CalculatePaging()

            ' Make the PrintDocument object.
            Dim PdfDocument As New PrintDocument

            If _documentType = "Landscape" Then
                PdfDocument.DefaultPageSettings.Landscape = True
            End If

            ' Install the PrintPage event handler.
            AddHandler PdfDocument.PrintPage, AddressOf Print_PrintPage

            ' Return the object.
            Return PdfDocument
        End Function

        Private Function getValue(ByVal KEY As String, ByVal ID As Integer, ByVal DT As String) As String
            Dim NEWVALUE = String.Empty
            If DT = "loop" Then

                Try
                    NEWVALUE = _loopDT.Rows(ID).Item(KEY)
                Catch ex As Exception
                End Try

            ElseIf (DT = "default") Then

                Dim NEWVALUE2 As String = ""
                If _defaultDict.TryGetValue(KEY, NEWVALUE2) Then
                    Return NEWVALUE2
                Else
                    Return String.Empty
                End If

            End If
            Return NEWVALUE
        End Function

#End Region

    End Class
End Namespace
