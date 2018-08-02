#Region "Enum PageSizeType"

''' <summary>
''' To Decide Print Page Size
''' </summary>
Public Enum PageSizeType
    ''' <summary>
    ''' Letter Size
    ''' </summary>
    LETTER
    ''' <summary>
    ''' A4 Size
    ''' </summary>
    A4
End Enum

#End Region

''' <summary>
''' Template base class for ticket creation with Ticket Designer Layout
''' Error Code : TACTDBASE-01
''' </summary>
<Serializable()> _
Public Class TicketDesignerBase
    Inherits TalentBase

#Region "Class Level Fields"

    Private _variableText As DETicketDesignerPrint
    Private _labelName As String = String.Empty
    Private _ticketPath As String = String.Empty
    Private _ticketName As String = String.Empty
    Private _authorName As String = String.Empty
    Private _applicationName As String = String.Empty
    Private _subject As String = String.Empty
    Private _title As String = String.Empty
    Private _formatRules As DataTable = Nothing

#End Region

#Region "Constructor"

    Public Sub New()
        _variableText = New DETicketDesignerPrint
    End Sub

#End Region

#Region "Properties"

    ''' <summary>
    ''' Create and Gets the ticket designer field values
    ''' </summary>
    ''' <value>DETicketDesignerPrint instance</value>
    Public Property VariableText() As DETicketDesignerPrint
        Get
            Return _variableText
        End Get
        Set(ByVal value As DETicketDesignerPrint)
            _variableText = value
        End Set
    End Property

    ''' <summary>
    ''' Set and gets the ticket design layout name
    ''' </summary>
    Public Property LabelName() As String
        Get
            Return _labelName
        End Get
        Set(ByVal value As String)
            _labelName = value
        End Set
    End Property

    ''' <summary>
    ''' Set and gets the ticket path 
    ''' </summary>
    Public Property TicketPath() As String
        Get
            Return _ticketPath
        End Get
        Set(ByVal value As String)
            _ticketPath = value
        End Set
    End Property

    ''' <summary>
    ''' Set and gets the ticket name
    ''' </summary>
    Public Property TicketName() As String
        Get
            Return _ticketName
        End Get
        Set(ByVal value As String)
            _ticketName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the author.
    ''' </summary>
    ''' <value>The author.</value>
    Public Property Author() As String
        Get
            Return _authorName
        End Get
        Set(ByVal value As String)
            _authorName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the creator.
    ''' </summary>
    ''' <value>The creator.</value>
    Public Property ApplicationName() As String
        Get
            Return _applicationName
        End Get
        Set(ByVal value As String)
            _applicationName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the subject.
    ''' </summary>
    ''' <value>The subject.</value>
    Public Property Subject() As String
        Get
            Return _subject
        End Get
        Set(ByVal value As String)
            _subject = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the title.
    ''' </summary>
    ''' <value>The title.</value>
    Public Property Title() As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    Public Property DeDespatch As DEDespatch

#End Region

#Region "Overridable Methods"

    ''' <summary>
    ''' creating the ticket layout
    ''' </summary>
    ''' <param name="dr">LabelProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overridable Function NewTicket(ByVal dr As DataRow) As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' Adds the fixed text field details to the ticket
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overridable Function AddFixedText(ByVal dr As DataRow) As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' Adds the variable text field details to the ticket
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overridable Function AddVariableText(ByVal dr As DataRow) As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' Adds the barcode to the ticket based on the given barcode
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overridable Function AddBarcode(ByVal dr As DataRow) As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' Adds the images to the ticket if the ticket layout has one
    ''' </summary>
    ''' <param name="dr">FieldProperties datarow</param>
    ''' <returns>Error Object</returns>
    Protected Overridable Function AddImage(ByVal dr As DataRow) As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' Adds table
    ''' </summary>
    ''' <param name="dt">Data table</param>
    ''' <returns>Error Object</returns>
    Protected Overridable Function AddTable(ByVal dr As DataRow, ByVal dt As DataTable, dtDetailItems As Data.DataTable) As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' Adds the page to the current created ticket
    ''' </summary>
    ''' <returns>Error Object</returns>
    Protected Overridable Function AddPage() As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

    ''' <summary>
    ''' close the ticket creation and release the necessary resources
    ''' </summary>
    ''' <returns>Error Object</returns>
    Protected Overridable Function PrintTicket() As ErrorObj
        Dim err As New ErrorObj
        If Not err.HasError Then
        End If
        Return err
    End Function

#End Region

#Region "Protected Methods"

    ''' <summary>
    ''' Retrieves the PDF font properties.
    ''' </summary>
    ''' <param name="simulatedFontName">Name of the simulated font.</param>
    ''' <returns>DataTable</returns>
    Protected Function RetrievePDFFontProperties(ByVal simulatedFontName As String) As DataTable
        Dim dtPDFFontProperties As New DataTable
        dtPDFFontProperties = TDataObjects.TicketDesignerSettings.TblTDPDFFontProperties.GetBySimulatedFontName(simulatedFontName)
        Return dtPDFFontProperties
    End Function

    ''' <summary>
    ''' Get the mapped font from the font simulated table
    ''' </summary>
    ''' <param name="fontName">Name of the font.</param>
    ''' <returns>String</returns>
    Protected Function FontOverride(ByVal fontName As String) As String
        Dim simulatedFontName As String = String.Empty
        Dim dtFontSimulate As New DataTable
        dtFontSimulate = TDataObjects.TicketDesignerSettings.TblTDFontSimulate.GetByFontName(fontName)
        If dtFontSimulate.Rows.Count > 0 Then
            simulatedFontName = dtFontSimulate.Rows(0)(0)
        End If
        Return simulatedFontName.Trim
    End Function

    ''' <summary>
    ''' Gets the formatted text.
    ''' </summary>
    ''' <param name="textToFormat">The text to format.</param>
    ''' <param name="formatType">Type of the format.</param>
    ''' <returns></returns>
    Protected Function GetFormattedText(ByVal textToFormat As String, ByVal formatType As String) As String
        Dim formattedText As String = textToFormat
        If (formatType.Length > 0) And (formatType <> "0") Then
            If (_formatRules Is Nothing) Then
                _formatRules = TDataObjects.TicketDesignerSettings.TblTDFormattingRules.GetAll()
            End If
            If (_formatRules.Rows.Count > 0) Then
                Dim formatRows() As DataRow
                formatRows = _formatRules.Select("TD_FORMAT='" & formatType.Trim & "'")
                If (formatRows.Length > 0) Then
                    Dim formRule As String = Utilities.CheckForDBNull_String(formatRows(0)("RULE_TO_APPLY"))
                    Select Case formRule
                        Case "DATETIMEFORMAT"
                            If (IsDate(textToFormat)) Then
                                formattedText = GetFormattedDate(textToFormat, formatType.Trim, Utilities.CheckForDBNull_String(formatRows(0)("DOTNET_FORMAT")))
                            End If
                        Case "FORMATPRICE"
                            formattedText = String.Format(Utilities.CheckForDBNull_String(formatRows(0)("DOTNET_FORMAT")), textToFormat)
                            If IsNumeric(textToFormat) Then
                                If CDec(textToFormat) > 0 Then
                                    formattedText = formattedText.Trim("-")
                                End If
                            End If
                        Case "FORMAT"
                            formattedText = String.Format(Utilities.CheckForDBNull_String(formatRows(0)("DOTNET_FORMAT")), textToFormat)
                        Case "REPLACE"
                            formattedText = textToFormat.Replace(Utilities.CheckForDBNull_String(formatRows(0)("REPLACE_STRING")), Utilities.CheckForDBNull_String(formatRows(0)("REPLACEWITH_STRING")))
                        Case "TRIMSTART"
                            formattedText = textToFormat.TrimStart(Utilities.CheckForDBNull_String(formatRows(0)("DOTNET_FORMAT")))
                    End Select
                End If
            End If
        End If
        Return formattedText
    End Function

#End Region

#Region "Private Method"
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
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Creates the ticket by processing all the fields
    ''' </summary>
    ''' <returns>Error Object</returns>
    Public Function CreateTicket() As ErrorObj

        Dim ticketCreationError As New ErrorObj
        'Create the layout details
        Try
            Dim dtLabelDetails As New DataTable
            dtLabelDetails = TDataObjects.TicketDesignerSettings.TblTDLabelProperties.GetByLabelName(_labelName)
            If dtLabelDetails.Rows.Count > 0 Then
                ticketCreationError = NewTicket(dtLabelDetails.Rows(0))
                Dim dtFieldDetails As New DataTable
                dtFieldDetails = TDataObjects.TicketDesignerSettings.TblTDFieldProperties.GetByLabelName(_labelName)
                For Each dr As DataRow In dtFieldDetails.Rows
                    If (ticketCreationError.HasError) Then
                        Exit For
                    Else
                        Select Case dr("FIELDTYPE")
                            Case Is = "text"
                                ticketCreationError = AddFixedText(dr)
                            Case Is = "varText"
                                ticketCreationError = AddVariableText(dr)
                            Case Is = "VariableBarcode"
                                ticketCreationError = AddBarcode(dr)
                            Case Is = "Logo"
                                ticketCreationError = AddImage(dr)
                            Case Is = "Table"
                                ticketCreationError = AddTable(dr, VariableText.despatchItemsSummary, VariableText.despatchItems)
                        End Select
                    End If

                Next
            Else
                ticketCreationError.HasError = True
                ticketCreationError.ErrorNumber = "TACTDBASE-02"
                ticketCreationError.ErrorMessage = "The given label name not exists in the table : " & _labelName
            End If
        Catch ex As Exception
            ticketCreationError.HasError = True
            ticketCreationError.ErrorNumber = "TACTDBASE-01"
            ticketCreationError.ErrorMessage = "Error while creating the ticket : " & ex.Message
        End Try

        Return ticketCreationError
    End Function

    ''' <summary>
    ''' Adds the page to ticket.
    ''' </summary>
    ''' <returns>Error Object</returns>
    Public Function AddPageToTicket() As ErrorObj
        Dim ticketPageAddError As New ErrorObj
        Try
            ticketPageAddError = AddPage()
        Catch ex As Exception
            ticketPageAddError.HasError = True
            ticketPageAddError.ErrorNumber = "TACTDBASE-03"
            ticketPageAddError.ErrorMessage = "Error while adding page to the ticket : " & ex.Message
        End Try
        Return ticketPageAddError
    End Function

    ''' <summary>
    ''' Closes the ticket.
    ''' </summary>
    ''' <returns>Error Object</returns>
    Public Function CloseTicket() As ErrorObj
        Dim ticketCloseError As New ErrorObj
        Try
            ticketCloseError = PrintTicket()
        Catch ex As Exception
            ticketCloseError.HasError = True
            ticketCloseError.ErrorNumber = "TACTDBASE-04"
            ticketCloseError.ErrorMessage = "Error while closing the ticket : " & ex.Message
        End Try
        Return ticketCloseError
    End Function
#End Region

End Class
