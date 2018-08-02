#Region "Enum TicketType"

''' <summary>
''' Ticket Document Type
''' </summary>
Public Enum TicketType
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
''' Gateway class for ticket creation based on the document type
''' Error Code : TACTDTKT-01
''' </summary>
<Serializable()> _
Public Class TicketDesignerTicket
    Inherits TalentBase

#Region "Class Level Fields"

    Private _ticketType As TicketType = TicketType.PDF
    Private _existingTicketType As TicketType = TicketType.PDF
    Private _ticketCreator As TicketDesignerBase
    Private _fileExtension As String = String.Empty
    Private _layoutLabelName As String = String.Empty
    Private ucr As UserControlResource

    Private _ticketPath As String = String.Empty
    Private _ticketFileName As String = String.Empty
#End Region

#Region "Constructor"
    Public Sub New(ByVal documentType As TicketType)
        Me.DocumentType = documentType
        AssignCreator()
    End Sub
#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the type of the document.
    ''' </summary>
    ''' <value>The type of the document.</value>
    Public Property DocumentType() As TicketType
        Get
            Return _ticketType
        End Get
        Set(ByVal value As TicketType)
            _ticketType = value
        End Set
    End Property

    ''' <summary>
    ''' Sets the ticket path.
    ''' </summary>
    ''' <value>The ticket path.</value>
    Public WriteOnly Property TicketPath() As String
        Set(ByVal value As String)
            _ticketPath = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the name of the ticket file.
    ''' </summary>
    ''' <value>The name of the ticket file.</value>
    Public ReadOnly Property TicketFileName() As String
        Get
            Return _ticketFileName
        End Get
    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Creates the ticket based on the assigned document type (default:PDF)
    ''' </summary>
    ''' <param name="orderDetails">The order details.</param>
    ''' <param name="PaymentReference">The payment reference.</param>
    ''' <returns>Error Object</returns>
    Public Function CreateTicket(ByVal orderDetails As DataTable, ByVal PaymentReference As String) As ErrorObj
        Dim ticketCreationError As New ErrorObj
        Dim totalRows As Integer = orderDetails.Rows.Count
        Dim ticketCounter As Integer = 0
        Try
            If (totalRows > 0) And (_ticketPath.Length > 0) Then
                For rowCounter As Integer = 0 To (totalRows - 1)
                    If IsProductRequiresTKT(orderDetails.Rows(rowCounter)) Then
                        'if it is first page create the object otherwise add a page to existing object
                        If ticketCounter = 0 Then
                            'create respective object
                            ticketCreationError = AssignCreator()

                            'If no error assign path, filename and general properties
                            If (Not ticketCreationError.HasError) Then
                                _ticketCreator.Settings = Settings
                                ticketCreationError = AssignGeneralProperties(PaymentReference)
                            End If
                        Else
                            ticketCreationError = _ticketCreator.AddPageToTicket()
                        End If

                        'If no error assign TKT properties from orderDetails table
                        If (Not ticketCreationError.HasError) Then
                            ticketCreationError = AssignTicketProperties(orderDetails.Rows(rowCounter))
                        End If

                        'If no error then create the ticket
                        If (Not ticketCreationError.HasError) Then
                            ticketCreationError = _ticketCreator.CreateTicket()
                        End If

                        'If there is error in creation then exit the for loop else increment the ticketCounter
                        If (ticketCreationError.HasError) Then
                            Exit For
                        Else
                            ticketCounter = ticketCounter + 1
                        End If

                    End If 'product requires ticket if ends
                Next
                If (Not ticketCreationError.HasError) And ticketCounter > 0 Then
                    ticketCreationError = _ticketCreator.CloseTicket()
                End If
                'no ticket created as there is no required product type in the order
                If ticketCounter = 0 Then
                    _ticketFileName = String.Empty
                End If
            Else
                ticketCreationError.HasError = True
                ticketCreationError.ErrorNumber = "TACTDTKT-02"
                ticketCreationError.ErrorMessage = "PurchaseResult from Order details is empty or ticket target path is empty"
            End If
        Catch ex As Exception
            ticketCreationError.HasError = True
            ticketCreationError.ErrorNumber = "TACTDTKT-01"
            ticketCreationError.ErrorMessage = "Error while creating the ticket object : " & ex.Message
        Finally
            _ticketCreator = Nothing
        End Try
        Return ticketCreationError
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Assigns the respective object and its specific properties based on the document type
    ''' </summary>
    Private Function AssignCreator() As ErrorObj
        Dim assignCreatorError As New ErrorObj
        Try
            If (Not _ticketType.Equals(_existingTicketType)) Or (_ticketCreator Is Nothing) Then
                _ticketCreator = Nothing
                Select Case _ticketType
                    Case TicketType.PDF
                        'assign the class specific properties
                        _existingTicketType = _ticketType
                        _fileExtension = ".PDF"
                        Dim ticketCreatorPDF As New TicketDesignerPdf
                        _ticketCreator = CType(ticketCreatorPDF, TicketDesignerBase)
                    Case TicketType.WORD
                        _existingTicketType = _ticketType
                        _fileExtension = ".DOCX"
                    Case Else
                        _existingTicketType = _ticketType
                        _fileExtension = ".PDF"
                        Dim ticketCreatorPDF As New TicketDesignerPdf
                        ticketCreatorPDF.PDFPageSize = PageSizeType.A4
                        _ticketCreator = CType(ticketCreatorPDF, TicketDesignerBase)
                End Select
            End If
        Catch ex As Exception
            assignCreatorError.HasError = True
            assignCreatorError.ErrorNumber = "TACTDTKT-03"
            assignCreatorError.ErrorMessage = "Error while creating the ticket object based on document type : " & ex.Message
        End Try
        Return assignCreatorError
    End Function

    ''' <summary>
    ''' Assigns the general properties for the ticket.
    ''' </summary>
    ''' <param name="PaymentReference">The payment reference.</param>
    Private Function AssignGeneralProperties(ByVal PaymentReference As String) As ErrorObj
        Dim assignGenPropError As New ErrorObj
        Try
            ucr = New UserControlResource
            With ucr
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .PageCode = ""
                .KeyCode = "TicketDesignerTicket.vb"
            End With
            Dim langCode As String = Settings.Language
            _ticketCreator.TicketPath = _ticketPath
            Dim ticketNamePrefix As String = ucr.Content("Tickets", langCode, True)
            If (ticketNamePrefix.Length > 0) Then
                _ticketFileName = ticketNamePrefix & "-" & PaymentReference & _fileExtension
            Else
                _ticketFileName = "Tickets" & "-" & PaymentReference & _fileExtension
            End If
            _ticketCreator.TicketName = _ticketFileName
            _ticketCreator.Author = ucr.Content("Author", langCode, True)
            _ticketCreator.Subject = ucr.Content("Subject", langCode, True)
            _ticketCreator.Title = ucr.Content("Title", langCode, True)
            _ticketCreator.ApplicationName = ucr.Content("ApplicationName", langCode, True)
            ucr = Nothing
        Catch ex As Exception
            assignGenPropError.HasError = True
            assignGenPropError.ErrorNumber = "TACTDTKT-04"
            assignGenPropError.ErrorMessage = "Error while assigning the general properties to ticket object : " & ex.Message
        End Try
        Return assignGenPropError
    End Function

    ''' <summary>
    ''' Assigns the ticket properties.
    ''' </summary>
    ''' <param name="dr">orderDetails DataRow</param>
    ''' <returns>Error Object</returns>
    Private Function AssignTicketProperties(ByVal dr As DataRow) As ErrorObj
        Dim assignTKTPropError As New ErrorObj
        Try
            _ticketCreator.LabelName = _layoutLabelName
            _ticketCreator.VariableText.CurrentDate = Now().ToString("dd/MM/yyyy")
            _ticketCreator.VariableText.CurrentTime = Now().ToString("HH:mm:ss")
            _ticketCreator.VariableText.Area = dr("Area")
            _ticketCreator.VariableText.AreaText = dr("AreaText")
            _ticketCreator.VariableText.Barcode = dr("BarcodeValue")
            _ticketCreator.VariableText.CustomerNumber = dr("CustomerNo")
            _ticketCreator.VariableText.FirstName = dr("ContactForename")
            _ticketCreator.VariableText.Gate = dr("Gates")
            _ticketCreator.VariableText.PayAmount = dr("PaymentAmnt")
            _ticketCreator.VariableText.PayReference = dr("PayRef")
            _ticketCreator.VariableText.PayType = dr("PayType")
            _ticketCreator.VariableText.PayTypeDescription = dr("PayTypeDesc")
            _ticketCreator.VariableText.PriceBand = dr("PriceBand")
            _ticketCreator.VariableText.PriceBandDesc = dr("PriceBDesc")
            _ticketCreator.VariableText.PriceCode = dr("PriceCode")
            _ticketCreator.VariableText.PriceCodeDesc = dr("PriceCDesc")
            _ticketCreator.VariableText.ProductCode = dr("ProductCode")
            _ticketCreator.VariableText.ProductDate = GetFormattedProductDate(Utilities.CheckForDBNull_String(dr("ProductDate")))
            _ticketCreator.VariableText.ProductDateInWords = GetFormattedProductDate(Utilities.CheckForDBNull_String(dr("ProductDate")))
            _ticketCreator.VariableText.ProductDescription = dr("ProductDescription")
            _ticketCreator.VariableText.ProductTime = dr("ProductTime")
            _ticketCreator.VariableText.RestText = dr("RestText")
            _ticketCreator.VariableText.RowN = dr("RowN")
            _ticketCreator.VariableText.SalePrice = dr("Price")
            _ticketCreator.VariableText.Seat = dr("Seat")
            _ticketCreator.VariableText.SeatN = dr("SeatN")
            _ticketCreator.VariableText.SeatSuffix = dr("SeatSuffix")
            _ticketCreator.VariableText.SeatText = dr("SeatText")
            _ticketCreator.VariableText.Stand = dr("Stand")
            _ticketCreator.VariableText.StandDesc = dr("StandDesc")
            _ticketCreator.VariableText.Surname = dr("ContactSurname")
            _ticketCreator.VariableText.TicketID = dr("TicketId")
            _ticketCreator.VariableText.TicketText = dr("TicketText")
            _ticketCreator.VariableText.Turnstile = dr("Turnstiles")
            _ticketCreator.VariableText.Title = dr("Title")
            _ticketCreator.VariableText.AddressLine1 = dr("AddressLine1")
            _ticketCreator.VariableText.AddressLine2 = dr("AddressLine2")
            _ticketCreator.VariableText.AddressLine3 = dr("AddressLine3")
            _ticketCreator.VariableText.AddressLine4 = dr("AddressLine4")
            _ticketCreator.VariableText.AddressLine5 = dr("AddressLine5")
            _ticketCreator.VariableText.PostCode = dr("PostCode")
            _ticketCreator.VariableText.TotalVATValue = dr("TotalVATValue")
            _ticketCreator.VariableText.GoodsValue = dr("GoodsValue")
            _ticketCreator.VariableText.NumberOfUnits = dr("NumberOfUnits")
            _ticketCreator.VariableText.PackageDescription = dr("PackageDescription")
            _ticketCreator.VariableText.CallReference = dr("CallReference")
            _ticketCreator.VariableText.ClientReferenceName = dr("ClientReferenceName")
            _ticketCreator.VariableText.CostPerPackage = dr("CostPerPackage")
            _ticketCreator.VariableText.VoucherCode = dr("VoucherCode")
            _ticketCreator.VariableText.ComponentDescription = dr("ComponentDescription")
            _ticketCreator.VariableText.VoucherExpiryDate = GetFormattedProductDate(Utilities.CheckForDBNull_String(dr("VoucherExpiryDate")))

        Catch ex As Exception
            assignTKTPropError.HasError = True
            assignTKTPropError.ErrorNumber = "TACTDTKT-05"
            assignTKTPropError.ErrorMessage = "Error while assigning the ticket properties to ticket object : " & ex.Message
        End Try
        Return assignTKTPropError
    End Function

    ''' <summary>
    ''' Determines whether [is product requires Ticket] [the specified dr].
    ''' </summary>
    ''' <param name="dr">orderDetails DataRow</param>
    ''' <returns>
    ''' <c>true</c> if [is product requires TKT] [the specified dr]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsProductRequiresTKT(ByVal dr As DataRow) As Boolean
        Dim canCreate As Boolean = False
        'Check for Print at Home
        '        If dr("ProductType").ToString.Trim = "H" Then
        If dr("PrintLayout").ToString.Length > 0 Then
            If dr("CarriageMethod").ToString.Trim = "H" Then
                _layoutLabelName = dr("PrintLayout").ToString
                canCreate = True
            End If
        End If
        '        End If
        'Check for Corporate Match Day Hospitality
        If Not canCreate Then
            If dr("CorporateLayout").ToString.Length > 0 Then
                _layoutLabelName = dr("CorporateLayout").ToString
                canCreate = True
            End If
        End If
        Return canCreate
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
#End Region

End Class
