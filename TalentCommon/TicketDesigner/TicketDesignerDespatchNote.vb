#Region "Enum DespatchNoteDocumentType"

''' <summary>
''' Despatch Note Document Type
''' </summary>
Public Enum DespatchNoteDocumentType
    ''' <summary>
    ''' PDF Document
    ''' </summary>
    PDF
    ''' <summary>
    ''' WORD Document
    ''' </summary>
    WORD
End Enum

#End Region

''' <summary>
''' Gateway class for despatch note creation based on the document type
''' Error Code : TACTD-DN-01
''' </summary>
<Serializable()> _
Public Class TicketDesignerDespatchNote
    Inherits TalentBase

#Region "Class Level Fields"

    Private _despatchNoteDocumentType As DespatchNoteDocumentType = DespatchNoteDocumentType.PDF
    Private _existingDespatchNoteDocumentType As DespatchNoteDocumentType = DespatchNoteDocumentType.PDF
    Private _despatchNoteCreator As TicketDesignerBase
    Private _fileExtension As String = String.Empty
    Private _layoutLabelName As String = String.Empty
    Private _despatchNotePath As String = String.Empty
    Private _despatchNotePrefix As String = String.Empty
    Private _despatchNoteFileName As String = String.Empty
    Private _boolReservations As Boolean = False
    Private _pageCounter As String = String.Empty
    Private ucr As UserControlResource
#End Region

#Region "Constructor"
    Public Sub New(ByVal documentType As TicketType)
        Me.DocumentType = documentType
        AssignCreator()
    End Sub
#End Region

#Region "Properties"

    Public Property DeDespatch As New DEDespatch

    ''' <summary>
    ''' Gets or sets the type of the document.
    ''' </summary>
    ''' <value>The type of the document.</value>
    Public Property DocumentType() As TicketType
        Get
            Return _despatchNoteDocumentType
        End Get
        Set(ByVal value As TicketType)
            _despatchNoteDocumentType = value
        End Set
    End Property

    ''' <summary>
    ''' Sets the ticket path.
    ''' </summary>
    ''' <value>The ticket path.</value>
    Public Property DespatchNotePath() As String
        Get
            Return _despatchNotePath
        End Get
        Set(ByVal value As String)
            _despatchNotePath = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the name of the ticket file.
    ''' </summary>
    ''' <value>The name of the ticket file.</value>
    Public ReadOnly Property DespatchNoteFileName() As String
        Get
            Return _despatchNoteFileName
        End Get
    End Property

    ''' <summary>
    ''' Gets the name of the ticket file.
    ''' </summary>
    ''' <value>The name of the ticket file.</value>
    Public ReadOnly Property PageCount() As String
        Get
            Return _pageCounter
        End Get
    End Property

#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Creates the ticket based on the assigned document type (default:PDF)
    ''' </summary>
    ''' <returns>Error Object</returns>
    Public Function CreateDespatchNote() As ErrorObj

        Dim despatchNoteCreationError As New ErrorObj
        Dim pageCounter As Integer = 0
        Dim dsOrderDetails As Data.DataSet = Nothing
        Dim dsDespatchItems As Data.DataSet = Nothing

        _layoutLabelName = DeDespatch.DespatchPDFLayoutName
        _despatchNotePath = DeDespatch.DespatchPDFDocumentPath
        Try
            ' Retrieve the batch records (summary and detail)
            If (Not despatchNoteCreationError.HasError) Then
                dsDespatchItems = Nothing
                despatchNoteCreationError = RetrieveDespatchItems(DeDespatch.BatchID, dsDespatchItems)
            End If

            For Each dr As Data.DataRow In DeDespatch.DataTable.Rows

                Dim dvDetail As New DataView(dsDespatchItems.Tables(1))
                If Not DeDespatch.DespatchReservations Then
                    dvDetail.RowFilter = ("PREF07=" & dr("PYRF"))
                Else
                    dvDetail.RowFilter = ("RSRF07=" & dr("PYRF"))
                End If
                Dim dtDetail As DataTable = dvDetail.ToTable

                Dim dvSummary As New DataView(dsDespatchItems.Tables(2))
                If Not DeDespatch.DespatchReservations Then
                    dvSummary.RowFilter = ("PayRef=" & dr("PYRF"))
                Else
                    dvSummary.RowFilter = ("ResRef=" & dr("PYRF"))
                End If

                Dim dtSummary As DataTable = dvSummary.ToTable



                ' Create first page (and document general properties, or add new page
                If pageCounter = 0 Then
                    'create respective object
                    despatchNoteCreationError = AssignCreator()

                    'If no error assign path, filename and general properties
                    If (Not despatchNoteCreationError.HasError) Then
                        _despatchNoteCreator.Settings = Settings
                        despatchNoteCreationError = AssignGeneralProperties(DeDespatch.BatchID)
                    End If
                Else
                    despatchNoteCreationError = _despatchNoteCreator.AddPageToTicket()
                End If


                'If no error assign properties 
                If (Not despatchNoteCreationError.HasError) Then
                    despatchNoteCreationError = AssignDespatchNoteProperties(dtDetail, dtSummary)
                End If


                'If no error then create the ticket
                If (Not despatchNoteCreationError.HasError) Then
                    _despatchNoteCreator.DeDespatch = DeDespatch
                    _despatchNoteCreator.DeDespatch.DespatchNotePageNumber = 1
                    If (DeDespatch.DespatchNoteSummaryOrDetail).ToUpper = "SUMMARY" Then
                        _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfRows = dtSummary.Rows.Count
                    End If
                    If (DeDespatch.DespatchNoteSummaryOrDetail).ToUpper = "DETAIL" Then
                        _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfRows = dtDetail.Rows.Count
                    End If
                    _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfRowsDone = 0
                    despatchNoteCreationError = _despatchNoteCreator.CreateTicket()

                    'Calculate the number of pages required to create in the PDF based on the number of rows shown on the despatch note and how many rows can be shown per page.
                    Dim rows As Integer = _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfRows
                    Dim rowsPerPage As Integer = DeDespatch.DespatchNoteTableMaxRowsPerPage
                    Dim remainder As Integer = 0
                    remainder = rows Mod rowsPerPage
                    If remainder = 0 Then
                        'Number of rows is exactly divisible by the number that can fit per page. Eg. 40 rows at 10 per page (4 pages)
                        _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfPages = rows / rowsPerPage
                    Else
                        'Number of rows leaves a remainder therefore increment the page number Eg. 43 rows at 10 per page (5 pages)
                        Dim totalPagesWithoutRemainder As Integer = 0
                        totalPagesWithoutRemainder = (rows - remainder) / rowsPerPage
                        _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfPages = totalPagesWithoutRemainder + 1
                    End If

                    Do Until _despatchNoteCreator.DeDespatch.DespatchNotePageNumber = _despatchNoteCreator.DeDespatch.DespatchNoteTotalNumberOfPages
                        despatchNoteCreationError = _despatchNoteCreator.AddPageToTicket()
                        If (Not despatchNoteCreationError.HasError) Then
                            _despatchNoteCreator.DeDespatch.DespatchNotePageNumber = _despatchNoteCreator.DeDespatch.DespatchNotePageNumber + 1
                            despatchNoteCreationError = _despatchNoteCreator.CreateTicket()
                        End If
                    Loop
                End If


                If (despatchNoteCreationError.HasError) Then
                    Exit For
                Else
                    pageCounter = pageCounter + 1
                End If

            Next

            'Close
            If (Not despatchNoteCreationError.HasError) And pageCounter > 0 Then
                _pageCounter = pageCounter
                despatchNoteCreationError = _despatchNoteCreator.CloseTicket()
            End If

            'No despatch note created
            If pageCounter = 0 Then
                _despatchNoteFileName = String.Empty
            End If

        Catch ex As Exception
            despatchNoteCreationError.HasError = True
            despatchNoteCreationError.ErrorNumber = "TACTD-DN-01"
            despatchNoteCreationError.ErrorMessage = "Error while creating the despatch note object : " & ex.Message
        Finally
            _despatchNoteCreator = Nothing
        End Try

        Return despatchNoteCreationError

    End Function
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Assigns the respective object and its specific properties based on the document type
    ''' </summary>
    Private Function AssignCreator() As ErrorObj
        Dim assignCreatorError As New ErrorObj
        Try
            If (Not _despatchNoteDocumentType.Equals(_existingDespatchNoteDocumentType)) Or (_despatchNoteCreator Is Nothing) Then
                _despatchNoteCreator = Nothing
                Select Case _despatchNoteDocumentType
                    Case TicketType.PDF
                        'assign the class specific properties
                        _existingDespatchNoteDocumentType = _despatchNoteDocumentType
                        _fileExtension = ".PDF"
                        Dim ticketCreatorPDF As New TicketDesignerPdf
                        _despatchNoteCreator = CType(ticketCreatorPDF, TicketDesignerBase)
                        _despatchNoteCreator.DeDespatch = DeDespatch
                    Case TicketType.WORD
                        _existingDespatchNoteDocumentType = _despatchNoteDocumentType
                        _fileExtension = ".DOCX"
                    Case Else
                        _existingDespatchNoteDocumentType = _despatchNoteDocumentType
                        _fileExtension = ".PDF"
                        Dim ticketCreatorPDF As New TicketDesignerPdf
                        ticketCreatorPDF.PDFPageSize = PageSizeType.A4
                        _despatchNoteCreator = CType(ticketCreatorPDF, TicketDesignerBase)
                End Select
            End If
        Catch ex As Exception
            assignCreatorError.HasError = True
            assignCreatorError.ErrorNumber = "TACTD-DN-03"
            assignCreatorError.ErrorMessage = "Error while creating the despatch note object based on document type : " & ex.Message
        End Try
        Return assignCreatorError
    End Function

    ''' <summary>
    ''' Assigns the general properties for the ticket.
    ''' </summary>
    ''' <param name="batchID">The payment reference.</param>
    Private Function AssignGeneralProperties(ByVal batchID As String) As ErrorObj
        Dim assignGenPropError As New ErrorObj
        Dim timestamp As String = Now.Year & Now.Month & Now.Day & Now.Hour & Now.Minute & Now.Second & Now.Millisecond
        Try
            ucr = New UserControlResource
            With ucr
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .PageCode = ""
                .KeyCode = "TicketDesignerDespatchNote.vb"
            End With
            Dim langCode As String = Settings.Language
            _despatchNoteCreator.TicketPath = _despatchNotePath
            _despatchNotePrefix = ucr.Content("despatchNoteNamePrefix", langCode, True)
            If (_despatchNotePrefix.Length > 0) Then
                _despatchNoteFileName = _despatchNotePrefix & batchID & timestamp & _fileExtension
            Else
                _despatchNoteFileName = "DespatchNote" & batchID & timestamp & _fileExtension
            End If
            _despatchNoteCreator.TicketName = _despatchNoteFileName
            _despatchNoteCreator.Author = ucr.Content("Author", langCode, True)
            _despatchNoteCreator.Subject = ucr.Content("Subject", langCode, True)
            _despatchNoteCreator.Title = ucr.Content("Title", langCode, True)
            _despatchNoteCreator.ApplicationName = ucr.Content("ApplicationName", langCode, True)
            ucr = Nothing
        Catch ex As Exception
            assignGenPropError.HasError = True
            assignGenPropError.ErrorNumber = "TACTD-DN-04"
            assignGenPropError.ErrorMessage = "Error while assigning the general properties to despatch note object : " & ex.Message
        End Try
        Return assignGenPropError
    End Function

    ''' <summary>
    ''' Assigns the ticket properties.
    ''' </summary>
    ''' <param name="dsOrderDetails">orderDetails DataSet</param>
    ''' <param name="dsDespatchItems">despatchItems DataSet</param>
    ''' <returns>Error Object</returns>
    Private Function AssignDespatchNotePropertiesOLD(ByVal dsOrderDetails As DataSet, dsDespatchItems As DataSet) As ErrorObj
        Dim assignDespatchNotePropError As New ErrorObj

        Try
            Dim dtOrderStatus As Data.DataTable = dsOrderDetails.Tables(0)
            Dim dr0 As Data.DataRow = dtOrderStatus.Rows(0)
            _despatchNoteCreator.VariableText.CurrentDate = Now().ToString("dd/MM/yyyy")
            _despatchNoteCreator.VariableText.CurrentTime = Now().ToString("HH:mm:ss")
            _despatchNoteCreator.LabelName = _layoutLabelName
            _despatchNoteCreator.VariableText.PaymentReference = dr0("PaymentReference")
            _despatchNoteCreator.VariableText.TotalPrice = dr0("TotalPrice")
            _despatchNoteCreator.VariableText.PaymentMethod = dr0("PaymentMethod")
            _despatchNoteCreator.VariableText.RefundAmount = dr0("RefundAmount")
            _despatchNoteCreator.VariableText.OnAccountAmount = dr0("OnAccountAmount")
            _despatchNoteCreator.VariableText.onAccountRefunded = dr0("onAccountRefunded")
            _despatchNoteCreator.VariableText.deliveryContactName = dr0("deliveryContactName")
            _despatchNoteCreator.VariableText.deliveryAddress1 = dr0("deliveryAddress1")
            _despatchNoteCreator.VariableText.deliveryAddress2 = dr0("deliveryAddress2")
            _despatchNoteCreator.VariableText.deliveryAddress3 = dr0("deliveryAddress3")
            _despatchNoteCreator.VariableText.deliveryAddress4 = dr0("deliveryAddress4")
            _despatchNoteCreator.VariableText.deliveryAddress5 = dr0("deliveryAddress5")
            _despatchNoteCreator.VariableText.deliveryPostCode = dr0("deliveryPostCode")
            _despatchNoteCreator.VariableText.orderDetails = dsOrderDetails.Tables(1)
            _despatchNoteCreator.VariableText.despatchItems = dsDespatchItems.Tables(1)


            Dim dtOrderDetails As Data.DataTable = dsOrderDetails.Tables(1)
            Dim dr As Data.DataRow = dtOrderDetails.Rows(0)
            _despatchNoteCreator.VariableText.Area = dr("Area")
            _despatchNoteCreator.VariableText.AreaText = dr("AreaText")
            _despatchNoteCreator.VariableText.Barcode = dr("BarcodeValue")
            _despatchNoteCreator.VariableText.CustomerNumber = dr("CustomerNo")
            _despatchNoteCreator.VariableText.FirstName = dr("ContactForename")
            _despatchNoteCreator.VariableText.Gate = dr("Gates")
            _despatchNoteCreator.VariableText.PayAmount = dr("PaymentAmnt")
            _despatchNoteCreator.VariableText.PayReference = dr("PayRef")
            _despatchNoteCreator.VariableText.PayType = dr("PayType")
            _despatchNoteCreator.VariableText.PayTypeDescription = dr("PayTypeDesc")
            _despatchNoteCreator.VariableText.PriceBand = dr("PriceBand")
            _despatchNoteCreator.VariableText.PriceBandDesc = dr("PriceBDesc")
            _despatchNoteCreator.VariableText.PriceCode = dr("PriceCode")
            _despatchNoteCreator.VariableText.PriceCodeDesc = dr("PriceCDesc")
            _despatchNoteCreator.VariableText.ProductCode = dr("ProductCode")
            _despatchNoteCreator.VariableText.ProductDate = GetFormattedProductDate(Utilities.CheckForDBNull_String(dr("ProductDate")))
            _despatchNoteCreator.VariableText.ProductDateInWords = GetFormattedProductDate(Utilities.CheckForDBNull_String(dr("ProductDate")))
            _despatchNoteCreator.VariableText.ProductDescription = dr("ProductDescription")
            _despatchNoteCreator.VariableText.ProductTime = dr("ProductTime")
            _despatchNoteCreator.VariableText.RestText = dr("RestText")
            _despatchNoteCreator.VariableText.RowN = dr("RowN")
            _despatchNoteCreator.VariableText.SalePrice = dr("Price")
            _despatchNoteCreator.VariableText.Seat = dr("Seat")
            _despatchNoteCreator.VariableText.SeatN = dr("SeatN")
            _despatchNoteCreator.VariableText.SeatSuffix = dr("SeatSuffix")
            _despatchNoteCreator.VariableText.SeatText = dr("SeatText")
            _despatchNoteCreator.VariableText.Stand = dr("Stand")
            _despatchNoteCreator.VariableText.StandDesc = dr("StandDesc")
            _despatchNoteCreator.VariableText.Surname = dr("ContactSurname")
            _despatchNoteCreator.VariableText.TicketID = dr("TicketId")
            _despatchNoteCreator.VariableText.TicketText = dr("TicketText")
            _despatchNoteCreator.VariableText.Turnstile = dr("Turnstiles")
            _despatchNoteCreator.VariableText.Title = dr("Title")
            _despatchNoteCreator.VariableText.AddressLine1 = dr("AddressLine1")
            _despatchNoteCreator.VariableText.AddressLine2 = dr("AddressLine2")
            _despatchNoteCreator.VariableText.AddressLine3 = dr("AddressLine3")
            _despatchNoteCreator.VariableText.AddressLine4 = dr("AddressLine4")
            _despatchNoteCreator.VariableText.AddressLine5 = dr("AddressLine5")
            _despatchNoteCreator.VariableText.PostCode = dr("PostCode")
            _despatchNoteCreator.VariableText.TotalVATValue = dr("TotalVATValue")
            _despatchNoteCreator.VariableText.GoodsValue = dr("GoodsValue")
            _despatchNoteCreator.VariableText.NumberOfUnits = dr("NumberOfUnits")
            _despatchNoteCreator.VariableText.PackageDescription = dr("PackageDescription")
            _despatchNoteCreator.VariableText.CallReference = dr("CallReference")
            _despatchNoteCreator.VariableText.ClientReferenceName = dr("ClientReferenceName")
            _despatchNoteCreator.VariableText.CostPerPackage = dr("CostPerPackage")
            _despatchNoteCreator.VariableText.VoucherCode = dr("VoucherCode")
            _despatchNoteCreator.VariableText.ComponentDescription = dr("ComponentDescription")

        Catch ex As Exception
            assignDespatchNotePropError.HasError = True
            assignDespatchNotePropError.ErrorNumber = "TACTD-DN-05"
            assignDespatchNotePropError.ErrorMessage = "Error while assigning the despatch note properties to despatch note object : " & ex.Message
        End Try
        Return assignDespatchNotePropError
    End Function

    ''' <summary>
    ''' Assigns the ticket properties.
    ''' </summary>
    ''' <param name="dtDetails">orderDetails DataTable</param>
    ''' <param name="dtSummary">orderSummary DataTable</param>
    ''' <returns>Error Object</returns>
    Private Function AssignDespatchNoteProperties(ByVal dtDetails As DataTable, ByVal dtSummary As DataTable) As ErrorObj
        Dim assignDespatchNotePropError As New ErrorObj

        ucr = New UserControlResource
        With ucr
            .FrontEndConnectionString = Settings.FrontEndConnectionString
            .BusinessUnit = Settings.BusinessUnit
            .PartnerCode = Settings.Partner
            .PageCode = ""
            .KeyCode = "TicketDesignerDespatchNote.vb"
        End With
        Dim langCode As String = Settings.Language

        Try
            Dim dr0 As Data.DataRow = dtDetails.Rows(0)
            _despatchNoteCreator.VariableText.despatchItems = dtDetails
            _despatchNoteCreator.VariableText.despatchItemsSummary = dtSummary
            _despatchNoteCreator.VariableText.CurrentDate = Now().ToString("dd/MM/yyyy")
            _despatchNoteCreator.VariableText.CurrentTime = Now().ToString("HH:mm:ss")
            _despatchNoteCreator.LabelName = _layoutLabelName
            If DeDespatch.DespatchReservations Then
                _despatchNoteCreator.VariableText.despatchPaymentReference = ucr.Content("NotApplicable", langCode, True)
            Else
                _despatchNoteCreator.VariableText.despatchPaymentReference = dr0("PREF07")
            End If
            _despatchNoteCreator.VariableText.despatchCustomerNumber = dr0("OWNR07")
            _despatchNoteCreator.VariableText.despatchDeliveryContactName = dr0("NAME07")
            _despatchNoteCreator.VariableText.despatchDeliveryAddress1 = dr0("ADD107")
            _despatchNoteCreator.VariableText.despatchDeliveryAddress2 = dr0("ADD207")
            _despatchNoteCreator.VariableText.despatchDeliveryAddress3 = dr0("ADD307")
            _despatchNoteCreator.VariableText.despatchDeliveryAddress4 = dr0("ADD407")
            _despatchNoteCreator.VariableText.despatchDeliveryAddress5 = dr0("ADD507")
            _despatchNoteCreator.VariableText.despatchDeliveryPostCode = dr0("POST07")
            'Attribute
            _despatchNoteCreator.VariableText.despatchCustomerAttribute = dr0("CH0407")
            'Company Name
            _despatchNoteCreator.VariableText.despatchCustomerCompanyName = dr0("CH1007")

            ' Bubble sort (sort of!) name07, ch1007, add107, add207, add307, add407, add507, post07 to remove blanks entries to ensure that address block are continous and do not contain blank lines.
            Dim arr() As String = New String() {dr0("NAME07"), dr0("CH1007"), dr0("ADD107"), dr0("ADD207"), dr0("ADD307"), dr0("ADD407"), dr0("POST07"), dr0("ADD507")}
            Dim i, j As Integer
            Dim n As Integer = arr.Length
            Dim boolAllDone As Boolean = False
            While boolAllDone = False

                For i = 0 To n - 1
                    If arr(i).Trim = "" Then
                        For j = i To n - 1
                            If j < n - 1 Then
                                arr(j) = arr(j + 1)
                            End If
                        Next
                        arr(n - 1) = ""
                    End If
                Next

                ' Do we need to go again...?  
                boolAllDone = True
                Dim firstBlank As Integer = 0
                Dim lastNonBlank As Integer = 0
                For i = 0 To n - 1
                    If arr(i).Trim = "" AndAlso firstBlank = 0 Then firstBlank = i
                    If arr(i).Trim <> "" Then lastNonBlank = i
                Next
                ' (Handle when all elements are blanks, no elements are blanks and if any blanks remain that are followed by non-blanks)
                If firstBlank <> 0 AndAlso firstBlank <> lastNonBlank AndAlso firstBlank < lastNonBlank Then boolAllDone = False
            End While

            ' Set the fields using the array values
            _despatchNoteCreator.VariableText.despatchDeliveryContactName = arr(0)
            _despatchNoteCreator.VariableText.despatchCustomerCompanyName = arr(1)
            _despatchNoteCreator.VariableText.despatchDeliveryAddress1 = arr(2)
            _despatchNoteCreator.VariableText.despatchDeliveryAddress2 = arr(3)
            _despatchNoteCreator.VariableText.despatchDeliveryAddress3 = arr(4)
            _despatchNoteCreator.VariableText.despatchDeliveryAddress4 = arr(5)
            _despatchNoteCreator.VariableText.despatchDeliveryAddress5 = arr(6)
            _despatchNoteCreator.VariableText.despatchDeliveryPostCode = arr(7)



            '            Dim salesDateString As String = Convert.ToString(dr0("SLDT07"))
            '            _despatchNoteCreator.VariableText.despatchSaleDate = Utilities.DateToIseries8Format(Utilities.ISeriesDate(salesDateString))
            If DeDespatch.DespatchReservations Then
                _despatchNoteCreator.VariableText.despatchSaleDate = ucr.Content("NotApplicable", langCode, True)
            Else
                _despatchNoteCreator.VariableText.despatchSaleDate = GetFormattedProductDate(dr0("SLDT07")).ToString.Substring(0, 10)
            End If

            If DeDespatch.DespatchReservations Then
                '                _despatchNoteCreator.VariableText.Barcode = "P-" & Utilities.PadLeadingZeros(dr0("BTCH07"), 8) & "-" & Utilities.PadLeadingZeros(dr0("PREF07"), 8)
                _despatchNoteCreator.VariableText.Barcode = "R-" & dr0("BTCH07") & "-" & dr0("RSRF07")
                _despatchNoteCreator.VariableText.despatchMode = "R"
            Else
                '               _despatchNoteCreator.VariableText.Barcode = "R-" & Utilities.PadLeadingZeros(dr0("BTCH07"), 8) & "-" & Utilities.PadLeadingZeros(dr0("PREF07"), 8)
                _despatchNoteCreator.VariableText.Barcode = "P-" & dr0("BTCH07") & "-" & dr0("PREF07")
                _despatchNoteCreator.VariableText.despatchMode = "P"
            End If

        Catch ex As Exception
            assignDespatchNotePropError.HasError = True
            assignDespatchNotePropError.ErrorNumber = "TACTD-DN-05"
            assignDespatchNotePropError.ErrorMessage = "Error while assigning the despatch note properties to despatch note object : " & ex.Message
        End Try
        Return assignDespatchNotePropError
    End Function



    ''' <summary>
    ''' Gets the formatted product date - IseriesDate is formatted to normal format
    ''' </summary>
    ''' <param name="productDate">The product date.</param><returns></returns>
    Private Function GetFormattedProductDate(ByVal productDate As String) As String
        Dim formattedProductDate As String = productDate
        If formattedProductDate.Trim("0").Length > 0 Then
            formattedProductDate = Utilities.ISeriesDate(formattedProductDate).ToString()
        Else
            formattedProductDate = formattedProductDate
        End If
        Return formattedProductDate
    End Function

    ''' <summary>
    ''' Retrieves order details
    ''' </summary>
    ''' <param name="payRef">Payment Reference</param><returns></returns>
    Private Function RetrieveOrderDetails(ByVal payRef As String, ByRef dsOrderDetails As Data.DataSet) As ErrorObj

        Dim Err As New ErrorObj

        '----------------------------------------------------
        ' Retrieve the order via WS030R
        '----------------------------------------------------
        Dim order As New Talent.Common.TalentOrder
        Dim deTicketingItemDetails As New Talent.Common.DETicketingItemDetails
        With deTicketingItemDetails
            .PaymentReference = payRef
            .UnprintedRecords = ""
            .SetAsPrinted = "N"
            .EndOfSale = "Y"
            .RetryFailure = False
            .Src = Settings.OriginatingSourceCode
        End With
        order.Settings = Settings
        order.Settings.Cacheing = False
        order.Dep.CollDEOrders.Add(deTicketingItemDetails)
        Err = order.OrderDetails

        If Not Err.HasError AndAlso Not order.ResultDataSet Is Nothing AndAlso order.ResultDataSet.Tables(0).Rows.Count > 0 Then

            If order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                Err.HasError = True
                Err.ErrorNumber = "TACTD-DN-07"
                Err.ErrorMessage = String.Format("Error Retrieving the order details. payment Reference : {0}, Return Code : {1}", payRef, order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
            Else

                If order.ResultDataSet.Tables.Count > 0 AndAlso order.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    dsOrderDetails = order.ResultDataSet
                Else
                    Err.HasError = True
                    Err.ErrorNumber = "TACTD-DN-08"
                    Err.ErrorMessage = "PurchaseResult from Order details is empty."
                End If

            End If
        Else
            Err.HasError = True
            Err.ErrorNumber = "TACTD-DN-09"
            Err.ErrorMessage = String.Format("Error Retrieving the order details. Payment Reference : {0}", payRef)
        End If

        Return Err

    End Function

    ''' <summary>
    ''' Retrieves order details
    ''' </summary>
    ''' <param name="batchID">Despatch Batch ID</param>
    ''' <param name="dsDespatchItems">DespatchItems DataSet</param>
    ''' <returns>Error Object</returns>
    Private Function RetrieveDespatchItems(ByVal batchID As String, ByRef dsDespatchItems As Data.DataSet) As ErrorObj

        Dim Err As New ErrorObj
        Dim talentDespatch As New TalentDespatch
        talentDespatch.Settings = Settings
        talentDespatch.Settings.Cacheing = False
        talentDespatch.DeDespatch = DeDespatch

        Err = talentDespatch.RetrieveDespatchNoteItems


        If Not Err.HasError AndAlso Not talentDespatch.ResultDataSet Is Nothing AndAlso talentDespatch.ResultDataSet.Tables(0).Rows.Count > 0 Then
            If talentDespatch.ResultDataSet.Tables("ErrorStatus").Rows(0).Item("ReturnCode") = GlobalConstants.ERRORFLAG Then
                Err.HasError = True
                Err.ErrorNumber = "TACTD-DN-10"
                Err.ErrorMessage = String.Format("Error Retrieving the despatch items. batch id : {0}, Return Code : {1}", batchID, talentDespatch.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
            Else
                If talentDespatch.ResultDataSet.Tables.Count > 0 AndAlso talentDespatch.ResultDataSet.Tables(1).Rows.Count > 0 Then
                    If talentDespatch.ResultDataSet.Tables.Count > 0 AndAlso talentDespatch.ResultDataSet.Tables(2).Rows.Count > 0 Then
                        dsDespatchItems = talentDespatch.ResultDataSet
                    Else
                        Err.HasError = True
                        Err.ErrorNumber = "TACTD-DN-11a"
                        Err.ErrorMessage = "Despatch items sumary from RetrieveDespatchNoteItems is empty."
                    End If
                Else
                    Err.HasError = True
                    Err.ErrorNumber = "TACTD-DN-11b"
                    Err.ErrorMessage = "Despatch items from RetrieveDespatchNoteItems is empty."

                End If
            End If
        Else
            Err.HasError = True
            Err.ErrorNumber = "TACTD-DN-12"
            Err.ErrorMessage = String.Format("Error Retrieving the despatch items. batch id : {0}", batchID)
        End If

        Return Err

    End Function

#End Region

End Class
