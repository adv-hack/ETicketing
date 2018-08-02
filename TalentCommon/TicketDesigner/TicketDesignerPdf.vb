Imports System
Imports System.Xml
Imports System.Net.Mail
Imports System.IO
Imports Microsoft.Win32
Imports iTextSharp.text
Imports iTextSharp.text.pdf

''' <summary>
''' To create PDF tickets using TicketDesigner layout.
''' This instance is heavy as it holds so many child instances, always dispose it as early as possible 
''' Error Code : TACTDPDF-01
''' </summary>
''' <remarks>
''' Re-throw all the exceptions as base class has the error object
''' </remarks>
<Serializable()> _
Public Class TicketDesignerPdf
    Inherits TicketDesignerBase

#Region "Class Level Fields"

    Private _baseFontProperties As New Dictionary(Of String, BaseFont)
    Private _simulatedFontNames As New Dictionary(Of String, String)
    Private _writer As PdfWriter = Nothing
    Private _pdfDoc As Document = Nothing
    Private _topY As Single = 0
    Private _pageType As iTextSharp.text.Rectangle = PageSize.A4


    'Const related to TicketDesigner Logos
    Private Const IMAGE_SCALE As Single = 12
    'Const converting TD mm measurement to Points for iTextSharp
    '1 mm = 2.834645669 PostScripts Points
    Private Const MM_TO_POINTS As Single = 2.834645669

#End Region

#Region "Properties"

    ''' <summary>
    ''' Sets the size of the PDF page.
    ''' </summary>
    ''' <value>The size of the PDF page.</value>
    Public WriteOnly Property PDFPageSize() As PageSizeType
        Set(ByVal value As PageSizeType)
            _pageType = GetPageSize(value)
        End Set
    End Property



#End Region

#Region "Protected Methods"


    ''' <summary>
    ''' creating the ticket layout
    ''' </summary>
    ''' <param name="dr">LabelProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overrides Function NewTicket(ByVal dr As DataRow) As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Try
            'Create File
            If (_pdfDoc Is Nothing) Then

                Dim thisPageType As iTextSharp.text.Rectangle = PageSize.A4
                If Not DeDespatch Is Nothing AndAlso DeDespatch.DespatchNoteOrientation.ToUpper = "LANDSCAPE" Then
                    thisPageType = PageSize.A4.Rotate
                End If
                _pdfDoc = New Document(thisPageType)

                _writer = PdfWriter.GetInstance(_pdfDoc, New FileStream(TicketPath & TicketName, FileMode.Create))
                _pdfDoc.Open()
                _pdfDoc.SetMargins(0, 0, 0, 0)
                _pdfDoc.AddAuthor(Me.Author)
                _pdfDoc.AddCreator(Me.ApplicationName)
                _pdfDoc.AddSubject(Me.Subject)
                _pdfDoc.AddTitle(Me.Title)
                _pdfDoc.AddCreationDate()
            End If
            _topY = _pdfDoc.Top

            'Add background image
            Dim backgroundImage As Image = Image.GetInstance(TicketPath & "/Images/" & dr("BACKGROUNDIMAGE"))
            backgroundImage.RotationDegrees = GetAngle(dr("ROTATION"))
            backgroundImage.SetAbsolutePosition(0, 0)
            backgroundImage.ScaleAbsoluteHeight(_pdfDoc.Top)
            backgroundImage.ScaleAbsoluteWidth(_pdfDoc.Right)
            _pdfDoc.Add(backgroundImage)
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-01"
            tempPDFTicketError.ErrorMessage = "Error While creating new pdf ticket : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

    ''' <summary>
    ''' Adds the fixed text field details to the ticket
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overrides Function AddFixedText(ByVal dr As DataRow) As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Dim tempText As String = String.Empty

        'If dr("NAME") = "DespatchTable" Then

        'End If
        Try
            tempText = dr("CONTENTS").ToString
            AddText(dr("XLEFT"), dr("YBOTTOM"), dr("FONT"), dr("HEIGHT"), tempText, dr("ANGLE"), dr("FORMAT"))
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-02"
            tempPDFTicketError.ErrorMessage = "Error while adding fixed text fields in pdf : " & tempText & " : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

    ''' <summary>
    ''' Adds the variable text field details to the ticket
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overrides Function AddVariableText(ByVal dr As DataRow) As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Dim tempText As String = String.Empty
        Try
            tempText = dr("NAME").ToString
            AddText(dr("XLEFT"), dr("YBOTTOM"), dr("FONT"), dr("HEIGHT"), VariableText.GetValueByKey(dr("NAME")), dr("ANGLE"), dr("FORMAT"))
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-03"
            tempPDFTicketError.ErrorMessage = "Error while adding variable text fields in pdf : " & tempText & " : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

    ''' <summary>
    ''' Adds the barcode to the ticket based on the given barcode
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overrides Function AddBarcode(ByVal dr As DataRow) As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Try
            Dim cb As PdfContentByte = _writer.DirectContent
            Dim barcodeImage As Image = Nothing
            Dim barcodeImageExists As Boolean = False
            Select Case dr("NAME")
                'CODE39, FORT39, EAN128, FOR128, FOR228, SK1128, SK2128, PDF41*
                Case Is = "CODE39", "FORT39"
                    'Barcode 3 of 9
                    Dim code39 As New Barcode39
                    code39.Code = VariableText.Barcode
                    If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("HUMANREADABLE")) Then
                        code39.Font = Nothing
                    End If
                    barcodeImage = code39.CreateImageWithBarcode(cb, New BaseColor(0, 0, 0), New BaseColor(0, 0, 0))
                    barcodeImageExists = True
                    code39 = Nothing
                Case Is = "EAN128", "FOR128", "FOR228", "SK1128", "SK2128"
                    'Barcode 128
                    Dim code128 As New Barcode128
                    code128.Code = VariableText.Barcode
                    If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("HUMANREADABLE")) Then
                        code128.Font = Nothing
                    End If
                    barcodeImage = code128.CreateImageWithBarcode(cb, New BaseColor(0, 0, 0), New BaseColor(0, 0, 0))
                    barcodeImageExists = True
                    code128 = Nothing
                Case Else
                    If Utilities.CheckForDBNull_String(dr("NAME")).Contains("PDF41") Then
                        'Barcode PDF417
                        Dim pdf417 As New BarcodePDF417
                        pdf417.SetText(Utilities.HexToBinary(VariableText.Barcode))
                        barcodeImage = pdf417.GetImage()
                        barcodeImageExists = True
                        pdf417 = Nothing
                    End If
            End Select
            If barcodeImageExists Then
                Dim xPosition As Single = dr("XLEFT") * MM_TO_POINTS
                Dim yPosition As Single = _topY - (CDec(dr("YTOP")) * MM_TO_POINTS)
                Dim angle As Single = GetAngle(dr("ANGLE").ToString)
                Dim imgHeight As Single = barcodeImage.PlainHeight
                Dim imgWidth As Single = barcodeImage.PlainWidth
                If (Not dr("LENGTH").Equals(DBNull.Value)) AndAlso (Not dr("LENGTH").Equals(String.Empty)) Then
                    '4 * MM_TO_POINTS height correction factor
                    imgHeight = (dr("HEIGHT") * MM_TO_POINTS) + (4 * MM_TO_POINTS)
                    imgWidth = dr("LENGTH") * MM_TO_POINTS
                    barcodeImage.ScaleAbsolute(imgWidth, imgHeight)
                End If
                Select Case angle
                    Case 0
                        yPosition = yPosition - imgHeight
                    Case 90
                    Case 180
                        xPosition = xPosition - imgWidth
                    Case 270
                        xPosition = xPosition - imgHeight
                        yPosition = yPosition - imgWidth
                End Select
                barcodeImage.RotationDegrees = angle
                barcodeImage.SetAbsolutePosition(xPosition, yPosition)
                cb.AddImage(barcodeImage)
            End If
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-04"
            tempPDFTicketError.ErrorMessage = "Error while adding barcode in the pdf : " & VariableText.Barcode & " : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

    ''' <summary>
    ''' Adds the images to the ticket if the ticket layout has one
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overrides Function AddImage(ByVal dr As DataRow) As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Try
            Dim imgLogo As Image = Image.GetInstance(TicketPath & "/Images/" & dr("FILE"))
            Dim imgHeight As Single = (imgLogo.PlainHeight / IMAGE_SCALE) * CDec(dr("HEIGHT")) * MM_TO_POINTS
            Dim imgWidth As Single = (imgLogo.PlainWidth / IMAGE_SCALE) * CDec(dr("HEIGHT")) * MM_TO_POINTS
            Dim xPosition As Single = (dr("XLEFT") * MM_TO_POINTS)
            Dim yPosition As Single = _topY - (CDec(dr("YBOTTOM")) * MM_TO_POINTS)
            Dim angle As Single = GetAngle(dr("ANGLE").ToString)
            imgLogo.RotationDegrees = angle
            imgLogo.ScaleAbsoluteHeight(imgHeight)
            imgLogo.ScaleAbsoluteWidth(imgWidth)
            Select Case angle
                Case 0
                Case 90
                    xPosition = xPosition - imgHeight
                Case 180
                    xPosition = xPosition - imgWidth
                    yPosition = yPosition - imgHeight
                Case 270
                    yPosition = yPosition - imgWidth
            End Select
            imgLogo.SetAbsolutePosition(xPosition, yPosition)
            _pdfDoc.Add(imgLogo)
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-05"
            tempPDFTicketError.ErrorMessage = "Error while adding image in the pdf : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

    ''' <summary>
    ''' Adds the page to the current created ticket
    ''' </summary>
    ''' <returns>Error Object</returns>
    Protected Overrides Function AddPage() As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Try
            _pdfDoc.NewPage()
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-06"
            tempPDFTicketError.ErrorMessage = "Error while adding page to the pdf ticket : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

    ''' <summary>
    ''' Adds the table to the current created ticket
    ''' </summary>
    ''' <returns>Error Object</returns>
    Protected Overrides Function AddTable(ByVal dr0 As DataRow, dt As Data.DataTable, dtDataItems As Data.DataTable) As ErrorObj

        Dim tempPDFTicketError As New ErrorObj

        Try

            ' Create a table object with required number of columns (Summary is currently hard coded to 2 columns) 
            Dim nCols As Integer
            If (DeDespatch.DespatchNoteSummaryOrDetail).ToUpper = "SUMMARY" Then
                nCols = 2
            Else
                nCols = Talent.Common.Utilities.CheckForDBNull_Int(DeDespatch.DespatchNoteTableColumnCount)
            End If
            Dim _table As New PdfPTable(nCols)
            Dim i As Integer


            ' Set the table font and font size
            Dim baseFontForTable As BaseFont = Nothing
            baseFontForTable = GetBaseFont(dr0("FONT"))
            Dim actualFontForTableHeader As New Font(baseFontForTable, dr0("HEIGHT"), Font.NORMAL)
            Dim actualFontForTableDetails As New Font(baseFontForTable, dr0("HEIGHT"), Font.NORMAL)

            ' Set the width of the table columns 
            Dim widths() As String = DeDespatch.DespatchNoteTableColumnWidths.Split(",")
            If widths.Length < nCols Then
                Dim errortext As String = "Setup error - Despatch note has " + CStr(nCols) + " columns but only " + CStr(widths.Length) + " widths defined"
                Throw New Exception(errortext)
            End If
            Dim columnWidths(nCols - 1) As Single
            For i = 0 To nCols - 1
                columnWidths(i) = CType(widths(i), Single)
            Next

            _table.SetWidths(columnWidths)

            ' Set header text for columms 
            Dim headerTexts() As String = DeDespatch.DespatchNoteTableColumnHeaders.Split(",")
            If headerTexts.Length < nCols Then
                Dim errortext As String = "Setup error - Despatch note has " + CStr(nCols) + " columns but only " + CStr(headerTexts.Length) + " headings defined"
                Throw New Exception(errortext)
            End If
            Dim headerCells(nCols - 1) As PdfPCell
            For i = 0 To nCols - 1
                Dim headerCell = New PdfPCell(New Phrase(headerTexts(i), actualFontForTableHeader))
                headerCell.HorizontalAlignment = Element.ALIGN_LEFT
                _table.AddCell(headerCell)
            Next


            Dim sDescText As String = String.Empty
            Dim sQtyText As String = String.Empty
            Dim currentPageRowCount As Integer = 0

            '
            ' If there is a default first line and/or second line to be created then merge geographical location text, 
            ' postage text , charity text and giftwrap text strings into text and then create line(s) in table.
            '
            ' (This is processed for page 1 only)
            '

            '2 Only have Summary mode i.e. What Goodwood currently use 
            If (DeDespatch.DespatchNoteSummaryOrDetail).ToUpper = "SUMMARY" Then
                If DeDespatch.DespatchNotePageNumber = 1 Then
                    If DeDespatch.DespatchNoteFirstLineFormat.Trim.Length > 0 Or _
                        DeDespatch.DespatchNoteSecondLineFormat.Trim.Length > 0 Then
                        Dim sGeoZone As String = String.Empty
                        Dim sPostageOption As String = String.Empty
                        Dim sCharityOption As String = String.Empty
                        Dim sGiftwrapOption As String = String.Empty
                        Dim dr As Data.DataRow = VariableText.despatchItems.Rows(0)

                        If dr("SP1107") = "C" Then sPostageOption = DeDespatch.DespatchNoteCollectText
                        If dr("SP1107") = "R" Then sPostageOption = DeDespatch.DespatchNoteRegPostText
                        If dr("SP1107") = "P" Then sPostageOption = DeDespatch.DespatchNotePostText
                        If dr("FL0607") = "Y" Then sCharityOption = DeDespatch.DespatchNoteCharityFeeText
                        If dr("FL0707") = "Y" Then sGiftwrapOption = DeDespatch.DespatchNoteGiftwrapFeeText
                        If Not DeDespatch.DespatchNoteGeographicalZoneTable Is Nothing AndAlso DeDespatch.DespatchNoteGeographicalZoneTable.Rows.Count > 0 Then
                            For Each drCZON As Data.DataRow In DeDespatch.DespatchNoteGeographicalZoneTable.Rows
                                If drCZON("VALUES").ToString.Trim = dr("FL0807").ToString.Trim Then
                                    sGeoZone = String.Format(DeDespatch.DespatchNoteGeographicalZoneTextFormat, drCZON("DESCRIPTION").ToString.Trim)
                                End If
                            Next
                        End If
                        If sPostageOption = String.Empty AndAlso DeDespatch.DespatchNotePostageNotAvailableText.ToString.Trim.Length > 0 Then sPostageOption = DeDespatch.DespatchNotePostageNotAvailableText.ToString.Trim
                        If sCharityOption = String.Empty AndAlso DeDespatch.DespatchNoteCharityNotAvailableText.ToString.Trim.Length > 0 Then sCharityOption = DeDespatch.DespatchNoteCharityNotAvailableText.ToString.Trim
                        If sGiftwrapOption = String.Empty AndAlso DeDespatch.DespatchNoteGiftwrapNotAvailableText.ToString.Trim.Length > 0 Then sGiftwrapOption = DeDespatch.DespatchNoteGiftwrapNotAvailableText.ToString.Trim
                        If sGeoZone = String.Empty AndAlso DeDespatch.DespatchNoteGeoZoneNotAvailableText.ToString.Trim.Length > 0 Then sGeoZone = DeDespatch.DespatchNoteGeoZoneNotAvailableText.ToString.Trim

                        If dr("SP1107") <> "R" And dr("SP1107") <> "P" Then sGeoZone = ""




                        ' First Line required...?
                        If DeDespatch.DespatchNoteFirstLineFormat.Trim.Length > 0 Then
                            sDescText = String.Format(DeDespatch.DespatchNoteFirstLineFormat, sGeoZone, sPostageOption, sCharityOption, sGiftwrapOption)
                            sQtyText = ""
                            If sDescText.Trim.Length > 0 Then
                                Dim descDetailCell As PdfPCell = New PdfPCell(New Phrase(sDescText, actualFontForTableDetails))
                                descDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                                _table.AddCell(descDetailCell)

                                Dim qtyDetailCell As PdfPCell = New PdfPCell(New Phrase(sQtyText, actualFontForTableDetails))
                                qtyDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                                _table.AddCell(qtyDetailCell)

                                currentPageRowCount = currentPageRowCount + 1
                                DeDespatch.DespatchNoteTotalNumberOfRows = DeDespatch.DespatchNoteTotalNumberOfRows + 1

                            End If
                        End If

                        ' Second Line required...?
                        If DeDespatch.DespatchNoteSecondLineFormat.Trim.Length > 0 Then
                            sDescText = String.Format(DeDespatch.DespatchNoteSecondLineFormat, sGeoZone, sPostageOption, sCharityOption, sGiftwrapOption)
                            sQtyText = ""
                            If sDescText.Trim.Length > 0 Then
                                Dim descDetailCell As PdfPCell = New PdfPCell(New Phrase(sDescText, actualFontForTableDetails))
                                descDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                                _table.AddCell(descDetailCell)

                                Dim qtyDetailCell As PdfPCell = New PdfPCell(New Phrase(sQtyText, actualFontForTableDetails))
                                qtyDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                                _table.AddCell(qtyDetailCell)

                                currentPageRowCount = currentPageRowCount + 1
                                DeDespatch.DespatchNoteTotalNumberOfRows = DeDespatch.DespatchNoteTotalNumberOfRows + 1

                            End If
                        End If

                    End If
                End If
            End If



            ' Add all the depatch items to the table line-by-line
            '
            Dim intRowCount As Integer = currentPageRowCount
            Dim intLowerRowAlllowed = (DeDespatch.DespatchNotePageNumber - 1) * DeDespatch.DespatchNoteTableMaxRowsPerPage
            Dim intUpperRowAllowed As Integer = DeDespatch.DespatchNotePageNumber * DeDespatch.DespatchNoteTableMaxRowsPerPage

            If (DeDespatch.DespatchNoteSummaryOrDetail).ToUpper = "SUMMARY" Then
                For Each dr As Data.DataRow In dt.Rows
                    intRowCount = intRowCount + 1

                    If intRowCount >= intLowerRowAlllowed AndAlso intRowCount <= intUpperRowAllowed Then

                        If currentPageRowCount < DeDespatch.DespatchNoteTableMaxRowsPerPage Then
                            If DeDespatch.DespatchRetail Then
                                sDescText = dr("ProductDescription")
                            Else
                                If dr("ProductType") = "H" Then
                                    sDescText = (dr("ProductDescription") + " (" + dr("PriceBandDescription") + ") - " + _
                                                   dr("Stand").ToString.Trim + "/" + dr("Area").ToString.Trim + "/" + _
                                                   dr("RowNumber").ToString.Trim + "/" + dr("SeatNumber").ToString.Trim + _
                                                   dr("AlphaSuffix").ToString.Trim)

                                Else
                                    sDescText = dr("ProductDescription") + " (" + dr("PriceBandDescription") + ")"
                                End If
                            End If
                            sQtyText = dr("Quantity1")

                            Dim descDetailCell As PdfPCell = New PdfPCell(New Phrase(sDescText, actualFontForTableDetails))
                            descDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                            _table.AddCell(descDetailCell)

                            Dim qtyDetailCell As PdfPCell = New PdfPCell(New Phrase(sQtyText, actualFontForTableDetails))
                            qtyDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                            _table.AddCell(qtyDetailCell)

                            currentPageRowCount = currentPageRowCount + 1

                        End If
                    End If
                Next
            End If

            If (DeDespatch.DespatchNoteSummaryOrDetail).ToUpper = "DETAIL" Then


                For Each dr As Data.DataRow In dtDataItems.Rows
                    Dim sDetailCellText As String = String.Empty
                    intRowCount = intRowCount + 1
                    If intRowCount >= intLowerRowAlllowed AndAlso intRowCount <= intUpperRowAllowed Then
                        If currentPageRowCount < DeDespatch.DespatchNoteTableMaxRowsPerPage Then
                            Dim detailCellValue() As String = DeDespatch.DespatchNoteTableColumnDetailValues.Split(",")
                            For i = 0 To nCols - 1
                                sDetailCellText = GetColumnDetailValue(detailCellValue(i), dr)
                                Dim descDetailCell As PdfPCell = New PdfPCell(New Phrase(sDetailCellText, actualFontForTableDetails))
                                descDetailCell.HorizontalAlignment = Element.ALIGN_LEFT
                                _table.AddCell(descDetailCell)
                            Next

                        End If
                    End If
                Next
            End If



            ' Increment the total number of rows now written (across all pages so far)
            DeDespatch.DespatchNoteTotalNumberOfRowsDone = DeDespatch.DespatchNoteTotalNumberOfRowsDone + currentPageRowCount


            ' Finally set the table width and write all the table rows to the page
            Dim page As Rectangle = _pdfDoc.PageSize
            _table.TotalWidth = page.Width - (2 * dr0("XLEFT"))
            _table.WriteSelectedRows(0, -1, dr0("XLEFT"), (page.Height - dr0("YTOP")), _writer.DirectContent)


        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-08"
            tempPDFTicketError.ErrorMessage = "Error while adding table in the pdf : " & ex.Message
        End Try
        Return tempPDFTicketError

    End Function

    ''' <summary>
    ''' close the ticket creation and release the necessary resources
    ''' </summary>
    ''' <returns>Error Object</returns>
    Protected Overrides Function PrintTicket() As ErrorObj
        Dim tempPDFTicketError As New ErrorObj
        Try
            _pdfDoc.Close()
            _writer = Nothing
            _pdfDoc = Nothing
        Catch ex As Exception
            tempPDFTicketError.HasError = True
            tempPDFTicketError.ErrorNumber = "TACTDPDF-07"
            tempPDFTicketError.ErrorMessage = "Error while closing created pdf ticket : " & ex.Message
        End Try
        Return tempPDFTicketError
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Adds the fixed or variable text to the pdf
    ''' </summary>
    ''' <param name="xPos">The x pos.</param>
    ''' <param name="yPos">The y pos.</param>
    ''' <param name="fontName">Name of the font.</param>
    ''' <param name="fontSize">Size of the font.</param>
    ''' <param name="text">The text.</param>
    ''' <param name="Angle">The angle.</param>
    Private Sub AddText(ByVal xPos As Single, ByVal yPos As Single, _
                        ByVal fontName As String, ByVal fontSize As Single, _
                        ByVal text As String, ByVal Angle As String, ByVal formatType As String)
        Dim baseFontForPDF As BaseFont = Nothing
        baseFontForPDF = GetBaseFont(fontName)
        Dim cb As PdfContentByte = _writer.DirectContent
        cb.BeginText()
        cb.SetFontAndSize(baseFontForPDF, fontSize)
        Dim xPosition As Single = (xPos * MM_TO_POINTS)
        Dim yPosition As Single = _topY - (yPos * MM_TO_POINTS)
        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, GetFormattedText(text, formatType), xPosition, yPosition, GetAngle(Angle))
        cb.EndText()
    End Sub

    ''' <summary>
    ''' Gets the angle based on the given value from TD field properties
    ''' </summary>
    ''' <param name="Angle">The angle.</param>
    ''' <returns></returns>
    Private Function GetAngle(ByVal Angle As String) As Single
        Dim requestedAngle As Single = 0
        Select Case (Angle.Trim)
            Case "0"
                requestedAngle = 0
            Case "1"
                requestedAngle = 90
            Case "2"
                requestedAngle = 180
            Case "3"
                requestedAngle = 270
            Case Else
                requestedAngle = 0
        End Select
        Return requestedAngle
    End Function
    ''' <summary>
    ''' Returns value of column from 
    ''' <returns></returns>
    Private Function GetColumnDetailValue(ByVal columnName As String, dr As Data.DataRow) As String
        Dim columnValue As String = String.Empty


        ' Allow field namne or descriptive code to be entered     
        Select Case (columnName.Trim).ToUpper
            Case "PRODUCTCODE", "MTCD07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("MTCD07"))
            Case "PAYREF", "PYRF07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_Decimal(dr("PCAT07"))
                If columnValue = "0" Then
                    columnValue = Talent.Common.Utilities.CheckForDBNull_Decimal(dr("PYRF07"))
                End If
            Case "SOLDDATE", "SLDT07"
                columnValue = Talent.Common.Utilities.ISeriesDate(dr("SLDT07")).ToString("dd/MM/yy")
            Case "STANDCODE", "STND07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("STND07"))
            Case "AREACODE", "AREA07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("AREA07"))
            Case "ROWCODE", "ROWN07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("ROWN07"))
            Case "SEATNUMBER", "SNUM07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("SNUM07"))
            Case "SEATALPHASUFFIX", "ASFX07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("ASFX07"))
            Case "PRICEBAND", "PRCE07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("PRCE07"))
            Case "PRICECODE", "PRCD07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("PRCD07"))
            Case "MEMBER", "MEMB07"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("MEMB07"))
            Case "STANDDESCRIPTION"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("STANDDESCRIPTION"))
            Case "AREADESCRIPTION"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("AREADESCRIPTION"))
            Case "PRICECODEDESCRIPTION"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("PRICECODEDESCRIPTION"))
            Case "PRICEBANDDESCRIPTION"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("PRICEBANDDESCRIPTION"))
            Case "PRODUCTDESCRIPTION"
                columnValue = Talent.Common.Utilities.CheckForDBNull_String(dr("PRODUCTDESCRIPTION"))
            Case "FORMATTEDSEAT"
                Dim seatDetails As New DESeatDetails
                seatDetails.Stand = dr("STND07")
                seatDetails.Area = dr("AREA07")
                seatDetails.Row = dr("ROWN07")
                seatDetails.Seat = dr("SNUM07")
                seatDetails.AlphaSuffix = dr("ASFX07")
                columnValue = seatDetails.FormattedSeat
            Case "PRICE", "SALP07"
                columnValue = System.Web.HttpUtility.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(CDec(dr("SALP07")), Settings.BusinessUnit, Settings.Partner))
            Case Else
                columnValue = String.Empty
        End Select
        Return columnValue
    End Function
    ''' <summary>
    ''' Gets the base font object for PDF object based on the given font name.
    ''' </summary>
    ''' <param name="fontName">Name of the font.</param>
    ''' <returns></returns>
    Private Function GetBaseFont(ByVal fontName As String) As BaseFont
        Dim baseFontForPDF As BaseFont = Nothing
        Dim fontFilePath As String = String.Empty
        Dim baseFontAssigned As Boolean = False
        fontName = GetSimulatedFont(fontName)
        If Not _baseFontProperties.TryGetValue(fontName, baseFontForPDF) Then
            Dim dtFontProperties As DataTable = RetrievePDFFontProperties(fontName)
            If dtFontProperties.Rows.Count > 0 Then
                Dim drFontProperty As DataRow = dtFontProperties.Rows(0)
                fontFilePath = drFontProperty("FILEPHYSICALPATH")
                If (fontFilePath.Length > 0) Then
                    If File.Exists(fontFilePath) Then
                        baseFontForPDF = BaseFont.CreateFont(fontFilePath, drFontProperty("ENCODING"), CType(drFontProperty("ISEMBEDDED"), Boolean))
                        _baseFontProperties.Add(fontName, baseFontForPDF)
                        baseFontAssigned = True
                    Else
                        baseFontAssigned = False
                    End If
                Else
                    baseFontAssigned = False
                End If
            End If
        Else
            baseFontAssigned = True
        End If
        If Not baseFontAssigned Then
            baseFontForPDF = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
        End If
        Return baseFontForPDF
    End Function

    ''' <summary>
    ''' Gets the simulated font from tbl_td_fontsimulate based on the given fontname
    ''' </summary>
    ''' <param name="fontName">Name of the font.</param>
    ''' <returns></returns>
    Private Function GetSimulatedFont(ByVal fontName As String) As String
        Dim simulatedFontName As String = String.Empty
        If Not _simulatedFontNames.TryGetValue(fontName, simulatedFontName) Then
            simulatedFontName = FontOverride(fontName)
            _simulatedFontNames.Add(fontName, simulatedFontName)
        End If
        Return simulatedFontName
    End Function

    ''' <summary>
    ''' Gets the Rectangle object by assigning PageSize object based on the given PageSizeType
    ''' </summary>
    ''' <param name="requestedPageSize">Size of the requested page.</param>
    ''' <returns></returns>
    Private Function GetPageSize(ByVal requestedPageSize As PageSizeType) As Rectangle
        Dim tempPageSize As Rectangle
        Select Case requestedPageSize
            Case PageSizeType.A4
                tempPageSize = PageSize.A4
            Case PageSizeType.LETTER
                tempPageSize = PageSize.LETTER
            Case Else
                tempPageSize = PageSize.A4
        End Select
        Return tempPageSize
    End Function

#End Region

End Class

