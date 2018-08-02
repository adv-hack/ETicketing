Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
Imports Talent.eCommerce
Imports System.Collections.Generic
Imports TEBUtilities = Talent.eCommerce.Utilities

Namespace Talent.eCommerce

    Public Class Order_Email

        Inherits ClassBase01

        Private _usage As String = Talent.Common.Utilities.GetAllString

        Private languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        Private ucr As New Talent.Common.UserControlResource
        Private dth As DataTable = Nothing
        Private strField(100) As String                         ' position 1 to 49  Strings or integers
        Private strData(100) As String                          ' position 50 to 79 Decimal values
        '                                                         position 80 to 100 Dates
        '
        Private strCode(10) As String                           ' paragraph body text
        Public Property Usage() As String
            Get
                Return _usage
            End Get
            Set(ByVal value As String)
                _usage = value
            End Set
        End Property

        Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

        Public Function SendConfirmationEmail(ByVal SiteEmail As String) As ErrorObj
            Dim err As New ErrorObj
            '----------------------------------------------------------------------------------
            '   Load info etc required by the Email_Send function
            '
            Dim OrderId As String = CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID
            Dim OrderHeader As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim iCounter As Integer = 0
            '------------------------------------------------------------------------------------------------
            '
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "OrderConfirmEmail.vb"
                .PageCode = UCase(Usage)
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            End With
            '------------------------------------------------------------------------------------------------
            dth = OrderHeader.Get_Header_By_Temp_Order_Id(OrderId)
            Dim strTo As String = dth.Rows(0)("CONTACT_EMAIL")
            If defaults.SendOrderConfToCustServ Then
                If defaults.CustomerServicesEmailRetail.Trim.Length > 0 Then
                    strTo &= ";" & defaults.CustomerServicesEmailRetail
                Else
                    strTo &= ";" & defaults.CustomerServicesEmail
                End If
            End If
            Dim strFrom As String = SiteEmail
            Dim strSubject As String = ucr.Content("EmailSubject", languageCode, True)
            If Utilities.IsBasketHomeDelivery(HttpContext.Current.Profile) Then
                strSubject = strSubject.Replace("<<<OrderType>>>", "Home Delivery")
            Else
                strSubject = strSubject.Replace("<<<OrderType>>>", "Trade")
            End If
            'For retail email confirmation the template will be from EmailMessage config 
            'if it Is blank Then template from emailmergecode01 to emailmergecode10
            Dim strMessage As String = ucr.Content("EmailMessage", languageCode, True).Trim

            err = OrderHeaderValues(OrderId)
            '----------------------------------------------------------------------------------
            '   Join all the paragraphs together only when strMessage is blank
            '
            If strMessage.Trim.Length <= 0 Then
                For iCounter = 0 To 10
                    strMessage &= strCode(iCounter) & vbCrLf & vbCrLf
                Next
            End If
            '
            strMessage = Replace(strMessage, "<<<email>>>", SiteEmail)
            '----------------------------------------------------------------------------------
            '   Replace the field delimiters with data
            '
            For iCounter = 0 To 100
                If strField(iCounter) > Nothing Then
                    strMessage = Replace(strMessage, strField(iCounter), strData(iCounter))
                    strSubject = Replace(strSubject, strField(iCounter), strData(iCounter))
                End If
            Next
            If Utilities.IsBasketHomeDelivery(HttpContext.Current.Profile) Then
                strMessage = strMessage.Replace("<<<OrderType>>>", "Home Delivery")
            Else
                strMessage = strMessage.Replace("<<<OrderType>>>", "Trade")
            End If
            ' Add the order lines
            strMessage = Replace(strMessage, "<<<OrderItems>>>", OrderDetails(OrderId))

            '    Perform the send operation
            '
            Dim htmlFormat As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(defaults.SendEmailAsHTML)
            If htmlFormat Then strMessage = Replace(strMessage, vbCrLf, "<br />")
            Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
            Talent.Common.Utilities.SMTPPortNumber = Utilities.GetSMTPPortNumber
            Talent.Common.Utilities.Email_Send(strFrom, strTo, strSubject, strMessage, "", False, htmlFormat)

            Return err
        End Function

        ''' <summary>
        ''' Send the order confirmation E-Mail
        ''' </summary>
        ''' <param name="paymentRef">The given payment reference</param>
        ''' <param name="dtCustomerDetails">The customer details data table</param>
        ''' <remarks></remarks>
        Public Sub SendTicketingConfirmationEmail(ByVal paymentRef As String, Optional ByVal dtCustomerDetails As DataTable = Nothing, Optional ByVal emailAddress As String = Nothing, Optional ByVal attachments As List(Of String) = Nothing)
            Dim strTo As String = ""
            Dim customer As String = ""
            Dim title As String = String.Empty
            Dim forename As String = String.Empty
            Dim surname As String = String.Empty

            If dtCustomerDetails Is Nothing Then
                If Not HttpContext.Current.Profile.IsAnonymous Then
                    If Not CType(HttpContext.Current.Profile, TalentProfile).User Is Nothing _
                        AndAlso Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing _
                        AndAlso Not CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email Is Nothing Then
                        strTo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email
                        customer = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                        title = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Title
                        forename = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Forename
                        surname = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Surname
                    End If
                End If
            Else
                If dtCustomerDetails IsNot Nothing AndAlso dtCustomerDetails.Rows.Count > 0 Then
                    strTo = Talent.eCommerce.Utilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("EmailAddress")).Trim
                    customer = Talent.eCommerce.Utilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("CustomerNo")).Trim
                    title = Talent.eCommerce.Utilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactTitle")).Trim
                    forename = Talent.eCommerce.Utilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactForename")).Trim
                    surname = Talent.eCommerce.Utilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactSurname")).Trim
                End If
            End If

            If emailAddress IsNot Nothing Then
                strTo = emailAddress
            End If

            If customer.Trim.Length > 0 Then
                'Create the xmlDocument
                Dim talEmail As New Talent.Common.TalentEmail
                Dim attachmentStringValue As String = String.Empty
                If attachments IsNot Nothing AndAlso attachments.Count > 0 Then
                    Dim i As Integer = 0
                    For Each attachment As String In attachments
                        If i > 0 Then attachmentStringValue &= ";"
                        attachmentStringValue &= attachment
                        i += 1
                    Next
                End If
                Dim xmlDoc As String = talEmail.CreateTicketingXmlDocument(defaults.OrdersFromEmail, strTo, "", ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, Utilities.GetSMTPPortNumber, ucr.PartnerCode, attachmentStringValue, paymentRef, customer, "W")

                'Create the email request in the offline processing table
                Dim agent As New Agent
                If agent.IsAgent AndAlso agent.BulkSalesMode Then
                    TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Batch", 0, "", "EmailMonitor", "TicketingConfirmation", xmlDoc, "")
                Else
                    TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", "EmailMonitor", "TicketingConfirmation", xmlDoc, "")
                End If
            End If
        End Sub

        Public Function SendOrderReturnConfirmationEmail(ByVal SiteEmail As String,
                                                            ByVal ds As Data.DataSet,
                                                            Optional ByVal mode As String = "1") As ErrorObj
            Dim err As New ErrorObj
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "OrderReturnConfirmEmail.vb"
                .PageCode = UCase(Usage)
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            End With

            Dim strTo As String = ""
            If Not HttpContext.Current.Profile.IsAnonymous Then
                If Not CType(HttpContext.Current.Profile, TalentProfile).User Is Nothing _
                    AndAlso Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing _
                    AndAlso Not CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email Is Nothing Then
                    strTo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email
                End If
            End If

            If Not String.IsNullOrEmpty(strTo) Then

                Dim talEmail As New Talent.Common.TalentEmail
                Dim xmlDoc As String = talEmail.CreateOrderReturnConfirmationXmlDocument(SiteEmail,
                                                                                strTo,
                                                                                ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim,
                                                                                Utilities.GetSMTPPortNumber,
                                                                                ucr.PartnerCode,
                                                                                CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID,
                                                                                ds.Tables(0).Rows(0).Item("OrderReturnReference").ToString(),
                                                                                mode,
                                                                                "W")
                'Create the email request in the offline processing table
                TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "",
                                                        "EmailMonitor", "OrderReturnConfirmation", xmlDoc, "")
            End If

            Return err
        End Function

        Public Sub SendCATConfirmationEmail(ByVal paymentRef As String, ByVal catMode As String, Optional ByVal emailAddress As String = Nothing, Optional ByVal attachments As List(Of String) = Nothing)

            'Format the To address
            Dim strTo As String = ""
            Dim customer As String = ""
            Dim title As String = String.Empty
            Dim forename As String = String.Empty
            Dim surname As String = String.Empty
            If Not HttpContext.Current.Profile.IsAnonymous Then
                If Not CType(HttpContext.Current.Profile, TalentProfile).User Is Nothing _
                    AndAlso Not CType(HttpContext.Current.Profile, TalentProfile).User.Details Is Nothing _
                    AndAlso Not CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email Is Nothing Then
                    strTo = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email
                    customer = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                    title = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Title
                    forename = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Forename
                    surname = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Surname
                End If
            End If

            If emailAddress IsNot Nothing Then
                strTo = emailAddress
            End If

            'Create the xmlDocument
            Dim talEmail As New Talent.Common.TalentEmail
            Dim emailType As String = String.Empty

            If Not String.IsNullOrWhiteSpace(catMode) Then
                If catMode = GlobalConstants.CATMODE_CANCEL Then
                    emailType = GlobalConstants.EMAIL_CATMODE_CANCEL
                ElseIf catMode = GlobalConstants.CATMODE_AMEND Then
                    emailType = GlobalConstants.EMAIL_CATMODE_AMEND
                ElseIf catMode = GlobalConstants.CATMODE_TRANSFER Then
                    emailType = GlobalConstants.EMAIL_CATMODE_TRANSFER
                End If
            End If

            Dim attachmentStringValue As String = String.Empty
            If attachments IsNot Nothing AndAlso attachments.Count > 0 Then
                Dim i As Integer = 0
                For Each attachment As String In attachments
                    If i > 0 Then attachmentStringValue &= ";"
                    attachmentStringValue &= attachment
                    i += 1
                Next
            End If

            Dim xmlDoc As String = talEmail.CreateTicketingXmlDocument(defaults.OrdersFromEmail, strTo, "", ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, Utilities.GetSMTPPortNumber, ucr.PartnerCode, attachmentStringValue, paymentRef, customer, GlobalConstants.SOURCE)
            Me.TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", "EmailMonitor", emailType, xmlDoc, "")
        End Sub

        Private Function OrderDetails(ByVal OrderID As String) As String
            Dim OrderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
            Dim order As TalentBasketDataset.tbl_order_detailDataTable = OrderLines.Get_Order_Lines(OrderID)
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim partnerCode As String = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
            Dim insertString As String = ""
            For Each line As TalentBasketDataset.tbl_order_detailRow In order.Rows
                insertString += Utilities.CheckForDBNull_Decimal(line("QUANTITY")).ToString & " x "
                If Utilities.CheckForDBNull_String(line("PRODUCT_DESCRIPTION_1")).Length > 100 Then
                    insertString += Utilities.CheckForDBNull_String(line("PRODUCT_DESCRIPTION_1")).Substring(0, 100) & vbTab
                Else
                    insertString += Utilities.CheckForDBNull_String(line("PRODUCT_DESCRIPTION_1")) & vbTab
                End If
                If Utilities.CheckForDBNull_String(line("PRODUCT_CODE")).Length > 0 Then
                    insertString += Utilities.CheckForDBNull_String(line("PRODUCT_CODE")) & vbTab
                End If

                insertString += TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(line("LINE_PRICE_GROSS")), businessUnit, partnerCode) & vbTab
                insertString += Utilities.CheckForDBNull_String(line("INSTRUCTIONS"))
                insertString += Environment.NewLine
            Next

            Return insertString
        End Function

        Private Function productTypeText(ByVal ds As Data.DataSet) As String
            Dim rtnStr = String.Empty
            Try

                Dim productType As New Generic.List(Of String)
                Dim row As Data.DataRow
                Dim dt As Data.DataTable

                'Loop through the basket adding the configurable text
                dt = ds.Tables(1)
                For Each row In dt.Rows

                    ' Product must be of a valid type
                    If row.Item("FeeType") <> "Y" AndAlso _
                        Not String.IsNullOrEmpty(row.Item("ProductType")) Then

                        'Only display the prouct specific text once
                        Dim key As String = "ProductTypeKey" & row.Item("ProductType")
                        If productType.IndexOf(key) < 0 Then
                            productType.Add(key)

                            'Display any configurable text
                            If Not String.IsNullOrEmpty(ucr.Content(key, languageCode, True)) Then
                                rtnStr += ucr.Content(key, languageCode, True)
                            End If
                        End If
                    End If

                Next

            Catch ex As Exception
            End Try

            Return rtnStr
        End Function

        Private Function paymentMethodText(ByVal ds As Data.DataSet) As String
            Dim rtnStr = String.Empty
            Try

                Dim row As Data.DataRow = ds.Tables(0).Rows(0)
                Dim key As String = "PaymentMethodKey" & row.Item("PaymentMethod")

                ' Display any configurable text
                If Not String.IsNullOrEmpty(ucr.Content(key, languageCode, True)) Then
                    rtnStr = ucr.Content(key, languageCode, True)
                End If

            Catch ex As Exception
            End Try

            Return rtnStr
        End Function

        Private Function orderticketingdetails(ByVal ds As Data.DataSet) As String

            'To avoid calling ucr cache collection inside loop
            Dim ucrCarriageReturn As String = String.Empty
            If ucr.Content("TicketingEmailCarriageReturn", languageCode, True).Trim = "" Then
                ucrCarriageReturn = vbCrLf
            Else
                ucrCarriageReturn = ucr.Content("TicketingEmailCarriageReturn", languageCode, True)
            End If
            Dim ucrTicketingSeperator1 As String = ucr.Content("TicketingSeperator1", languageCode, True)
            Dim ucrTicketingSeperator4 As String = ucr.Content("TicketingSeperator4", languageCode, True)
            Dim ucrTicketingSeperator5 As String = ucr.Content("TicketingSeperator5", languageCode, True)
            Dim ucrTicketingSeperator6 As String = ucr.Content("TicketingSeperator6", languageCode, True)
            Dim ucrTicketingSeperator7 As String = ucr.Content("TicketingSeperator7", languageCode, True)
            Dim ucrTicketingSeperator8 As String = ucr.Content("TicketingSeperator8", languageCode, True)
            Dim ucrTicketingSmartcardSymbol As String = ucr.Content("TicketingSmartcardSymbol", languageCode, True)

            'To avoid calling ecommerce defaults cache collection inside loop
            Dim displayFreeTicketingItems As Boolean = defaults.DISPLAY_FREE_TICKETING_ITEMS

            Dim tempProductDateTime As String = String.Empty

            'Message builders
            Dim freeItemsExists As Boolean = False
            Dim ppsItemsExists As Boolean = False
            Dim sbPPSItems As New StringBuilder
            Dim sbFreeItems As New StringBuilder
            Dim sbOtherItems As New StringBuilder

            Dim finalMessage As String = String.Empty
            Try
                Dim row As Data.DataRow
                Dim dt As Data.DataTable

                dt = ds.Tables(1)
                For Each row In dt.Rows
                    Try
                        'pps items
                        If row.Item("ProductType").ToString.Trim = "P" Then
                            ppsItemsExists = True
                            sbPPSItems.Append(row.Item("ProductDescription").ToString.Trim)
                            ' Add seat information if applicable
                            If row.Item("Seat").ToString.Trim <> "" Then
                                sbPPSItems.Append(ucrTicketingSeperator4 & getSeatInfo(row))
                            End If
                            ' Add customer and pricing infomation
                            sbPPSItems.Append(ucrTicketingSeperator5 & row.Item("ContactForename").ToString().Trim())
                            sbPPSItems.Append(ucrTicketingSeperator6 & row.Item("ContactSurname").ToString().Trim())
                            sbPPSItems.Append(ucrTicketingSeperator7 & row.Item("CustomerNo").ToString().Trim())
                            sbPPSItems.Append(ucrTicketingSeperator8 & row.Item("Price").ToString().Trim())
                            ' Add the carriage return
                            sbPPSItems.Append(ucrCarriageReturn)

                            'free item
                        ElseIf row.Item("Price") = "0.00" And row.Item("FeeType") <> "Y" Then
                            If displayFreeTicketingItems Then
                                freeItemsExists = True
                                sbFreeItems.Append(row.Item("ProductDescription").ToString.Trim)
                                sbFreeItems.Append(ucrTicketingSeperator5 & row.Item("PriceBand").ToString.Trim)
                                sbFreeItems.Append(ucrTicketingSeperator5 & row.Item("ContactForename").ToString.Trim)
                                sbFreeItems.Append(ucrTicketingSeperator6 & row.Item("ContactSurname").ToString.Trim)
                                sbFreeItems.Append(ucrTicketingSeperator7 & row.Item("CustomerNo").ToString.Trim)
                                sbFreeItems.Append(ucrTicketingSeperator8)
                                ' Add the carriage return
                                sbFreeItems.Append(ucrCarriageReturn)
                            End If

                            'purchased or other items
                        Else

                            sbOtherItems.Append(row.Item("ProductDescription").ToString.Trim)
                            ' Add Date and time if applicable
                            tempProductDateTime = getProductDateTime(row.Item("ProductDate").ToString.Trim, row.Item("ProductTime").ToString.Trim)
                            If tempProductDateTime.Length > 0 Then
                                sbOtherItems.Append(ucrTicketingSeperator1 & tempProductDateTime)
                            End If
                            tempProductDateTime = String.Empty
                            ' Add seat information if applicable
                            If row.Item("Seat").ToString.Trim <> "" Then
                                sbOtherItems.Append(ucrTicketingSeperator4 & getSeatInfo(row))
                            End If
                            ' Add customer and pricing infomation
                            sbOtherItems.Append(ucrTicketingSeperator5 & row.Item("ContactForename").ToString().Trim())
                            sbOtherItems.Append(ucrTicketingSeperator6 & row.Item("ContactSurname").ToString().Trim())
                            sbOtherItems.Append(ucrTicketingSeperator7 & row.Item("CustomerNo").ToString().Trim())
                            sbOtherItems.Append(ucrTicketingSeperator8 & row.Item("Price").ToString().Trim())
                            ' Have we uploaded this game to a smartcard
                            If row.Item("SmartcardUploaded") = "Y" Then
                                sbOtherItems.Append(ucrTicketingSmartcardSymbol)
                            End If
                            ' Add the carriage return
                            sbOtherItems.Append(ucrCarriageReturn)

                        End If
                    Catch ex As Exception
                    End Try
                Next

                'message sections order appending
                'purchased products message
                finalMessage = sbOtherItems.ToString()
                sbOtherItems = Nothing
                'pps products message
                If ppsItemsExists Then
                    finalMessage += ucrCarriageReturn
                    finalMessage += sbPPSItems.ToString()
                    sbPPSItems = Nothing
                End If

                'free products message
                If displayFreeTicketingItems Then
                    finalMessage += ucrCarriageReturn
                End If
                If freeItemsExists Then
                    finalMessage += sbFreeItems.ToString()
                    sbFreeItems = Nothing
                End If
            Catch ex As Exception
            End Try
            Return finalMessage
        End Function

        Private Function getProductDateTime(ByVal inDate As String, ByVal inTime As String) As String
            Dim prodDateTime As String = String.Empty
            Dim year As String
            Dim month As String
            Dim day As String
            Try
                year = inDate.Substring(1, 2)
                month = inDate.Substring(3, 2)
                day = inDate.Substring(5, 2)
                If day = "00" Then
                    prodDateTime = ""
                Else
                    prodDateTime = day & "/" & month & "/" & year & ucr.Content("TicketingSeperator2", languageCode, True)
                End If
                If inTime.Trim <> "" Then
                    prodDateTime += inTime & ucr.Content("TicketingSeperator3", languageCode, True)
                End If
            Catch ex As Exception
            End Try
            Return prodDateTime
        End Function

        Private Function getSeatInfo(ByVal row As Data.DataRow) As String

            Dim seatInfo As String = String.Empty

            Select Case ucr.Attribute("SeatType")
                Case Is = "1"
                    seatInfo = getTurnstileInfo(row.Item("Turnstiles").ToString) + _
                                getGateInfo(row.Item("Gates").ToString) + _
                                getPartSeatInfo("Row", row.Item("Seat").ToString.Trim)
                    If Not defaults.SeatDisplay = 1 Then
                        seatInfo = seatInfo + getPartSeatInfo("Seat", row.Item("Seat").ToString.Trim)
                    End If
                Case Is = "2"
                    seatInfo = getTurnstileInfo(row.Item("Turnstiles").ToString) + _
                                getPartSeatInfo("Stand", row.Item("Seat").ToString.Trim) + _
                                getPartSeatInfo("Area", row.Item("Seat").ToString.Trim) + _
                                getPartSeatInfo("Row", row.Item("Seat").ToString.Trim)
                    If Not defaults.SeatDisplay = 1 Then
                        seatInfo = seatInfo + getPartSeatInfo("Seat", row.Item("Seat").ToString.Trim)
                    End If
                Case Is = "3"
                    seatInfo = row.Item("Seat").ToString.Trim & _
                                getTurnstileInfo(row.Item("Turnstiles").ToString) + _
                                 getGateInfo(row.Item("Gates").ToString)

                Case Else
                    Select Case defaults.SeatDisplay
                        Case Is = 1
                            'Show stand, area and row ( stand is first 3 chars, area is next 4, and row is the next 4 )
                            seatInfo = Utilities.CheckForDBNull_String(row.Item("Seat")).Substring(0, 11)
                        Case Is = 2
                            'Show stand, area, row and seat number (seat number is next 4 chars after above)
                            seatInfo = Utilities.CheckForDBNull_String(row.Item("Seat")).Substring(0, 15)
                        Case Else
                            'Show all (including alpha suffix)
                            seatInfo = Utilities.CheckForDBNull_String(row.Item("Seat"))
                    End Select
            End Select

            Return seatInfo

        End Function

        Private Function getTurnstileInfo(ByVal turnstiles As String) As String

            Dim turnstilesInfo As String = String.Empty
            Dim index As Integer = 0

            If turnstiles.Trim <> String.Empty Then
                turnstilesInfo = ucr.Content("TurnstileSeperator1", languageCode, True)
                Do While index < 10 And (index + 3) <= turnstiles.Length
                    If turnstiles.Substring(index, 3).Trim <> String.Empty Then
                        If index <> 0 Then
                            turnstilesInfo += ucr.Content("TurnstileSeperator2", languageCode, True)
                        End If
                        turnstilesInfo += turnstiles.Substring(index, 3).Trim
                    End If
                    index += 3
                Loop
            End If

            Return turnstilesInfo

        End Function

        Private Function getGateInfo(ByVal gates As String) As String

            Dim gateInfo As String = String.Empty
            Dim index As Integer = 0

            If gates.Trim <> String.Empty Then
                gateInfo = ucr.Content("GateSeperator1", languageCode, True)
                Do While index < 10 And (index + 3) <= gates.Length
                    If gates.Substring(index, 3).Trim <> String.Empty Then
                        If index <> 0 Then
                            gateInfo += ucr.Content("GateSeperator2", languageCode, True)
                        End If
                        gateInfo += gates.Substring(index, 3).Trim
                    End If
                    index += 3
                Loop
            End If

            Return gateInfo

        End Function

        Private Function getPartSeatInfo(ByVal part As String, ByVal seat As String) As String

            Dim seatInfo As String = String.Empty
            Dim standSep As Integer = seat.IndexOf("/")
            Dim areaSep As Integer = seat.IndexOf("/", standSep + 1)
            Dim rowSep As Integer = seat.IndexOf("/", areaSep + 1)
            Dim start As Integer = 0
            Dim length As Integer = 0

            Try
                Select Case part
                    Case Is = "Stand"
                        start = 0
                        length = standSep
                        If (start + length) < seat.Length And length > 0 Then
                            seatInfo = ucr.Content("StandSeperator1", languageCode, True) + seat.Substring(start, length)
                        End If
                    Case Is = "Area"
                        start = standSep + 1
                        length = areaSep - 1 - standSep
                        If (start + length) < seat.Length And length > 0 Then
                            seatInfo = ucr.Content("AreaSeperator1", languageCode, True) + seat.Substring(start, length)
                        End If
                    Case Is = "Row"
                        start = areaSep + 1
                        length = rowSep - 1 - areaSep
                        If (start + length) < seat.Length And length > 0 Then
                            seatInfo = ucr.Content("RowSeperator1", languageCode, True) + seat.Substring(start, length)
                        End If
                    Case Is = "Seat"
                        start = rowSep + 1
                        If (start) < seat.Length Then
                            seatInfo = ucr.Content("SeatSeperator1", languageCode, True) + seat.Substring(start)
                        End If
                End Select
            Catch ex As Exception
            End Try

            Return seatInfo

        End Function

        Private Function DirectDebitSchedule(ByVal ds As Data.DataSet) As String

            Dim rtnStr = String.Empty
            Try

                'Was this a direct debit sale
                If ds.Tables(0).Rows(0).Item("PaymentMethod").ToString = "DD" Then

                    'Retrieve the payment schedule
                    Dim err As New Talent.Common.ErrorObj
                    Dim returnErrorCode As String = String.Empty

                    'Create the payment object
                    Dim payment As New Talent.Common.TalentPayment
                    Dim dePayment As New Talent.Common.DEPayments
                    With dePayment
                        .PaymentRef = ds.Tables(0).Rows(0).Item("PaymentReference").ToString.TrimStart("0")
                        .Source = "W"
                        .CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                    End With

                    payment.De = dePayment
                    payment.Settings = Utilities.GetSettingsObject()
                    payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                    payment.Settings.StoredProcedureGroup = Utilities.GetStoredProcedureGroup
                    payment.Settings.Cacheing = CType(ucr.Attribute("Cacheing"), Boolean)
                    payment.Settings.CacheTimeMinutes = CType(ucr.Attribute("CacheTimeMinutes"), Integer)
                    payment.Settings.CacheDependencyPath = defaults.CacheDependencyPath

                    'Retrieve the direct debit summary
                    err = payment.DirectDebitSummary

                    'Populate the email with the direct debit summary 
                    If Not err.HasError _
                            AndAlso Not payment.ResultDataSet Is Nothing _
                            AndAlso payment.ResultDataSet.Tables.Count > 1 Then

                        With ucr

                            rtnStr += .Content("DirectDebitTopSeparator", languageCode, True) & TicketingCarriageReturn()

                            'Title and Guarantee
                            rtnStr += .Content("DirectDebitTitle", languageCode, True) & TicketingCarriageReturn()
                            rtnStr += .Content("DirectDebitGuarantee", languageCode, True) & TicketingCarriageReturn()
                            rtnStr += TicketingCarriageReturn()

                            'DDI Ref
                            rtnStr += Talent.Common.Utilities.FixStringLength(.Content("DirectDebitDDIRef", languageCode, True), 40) _
                              & payment.ResultDataSet.Tables(0).Rows(0).Item("DirectDebitDDIRef").ToString.Trim & TicketingCarriageReturn()

                            'Account Name
                            rtnStr += Talent.Common.Utilities.FixStringLength(.Content("DirectDebitAccountName", languageCode, True), 40) _
                              & payment.ResultDataSet.Tables(0).Rows(0).Item("AccountName").ToString.Trim & TicketingCarriageReturn()

                            'Sort Code
                            rtnStr += Talent.Common.Utilities.FixStringLength(.Content("DirectDebitSortCode", languageCode, True), 40) _
                              & payment.ResultDataSet.Tables(0).Rows(0).Item("SortCode").ToString.Trim & TicketingCarriageReturn()

                            'Account Number
                            rtnStr += Talent.Common.Utilities.FixStringLength(.Content("DirectDebitAccountNumber", languageCode, True), 40) _
                              & payment.ResultDataSet.Tables(0).Rows(0).Item("AccountNumber").ToString.Trim & TicketingCarriageReturn()
                            rtnStr += TicketingCarriageReturn()

                            ' Direct Debit Schedule Header Records
                            rtnStr += Talent.Common.Utilities.FixStringLength(.Content("DirectDebitPaymentDate", languageCode, True), 40) _
                               & .Content("DirectDebitPaymentAmount", languageCode, True) & TicketingCarriageReturn()

                            ' Direct Debit Schedule 
                            Dim row As Data.DataRow

                            For Each row In payment.ResultDataSet.Tables(1).Rows
                                ' Direct Debit Schedule Detail Records
                                rtnStr += Talent.Common.Utilities.FixStringLength(row.Item("PaymentDate"), 40) _
                                   & row.Item("PaymentAmount") & TicketingCarriageReturn()
                            Next

                            rtnStr += ucr.Content("DirectDebitBottomSeparator", languageCode, True) & TicketingCarriageReturn()

                        End With

                    End If

                End If

            Catch ex As Exception
            End Try

            Return rtnStr

        End Function

        Private Function TicketingCarriageReturn() As String
            If ucr.Content("TicketingEmailCarriageReturn", languageCode, True).Trim = "" Then
                Return vbCrLf
            Else
                Return ucr.Content("TicketingEmailCarriageReturn", languageCode, True)
            End If
        End Function


        Private Function OrderHeaderValues(ByVal OrderId As String) As ErrorObj
            Dim err As New ErrorObj
            '----------------------------------------------------------------------------------
            '   Load header bits
            '
            With ucr
                '------------------------------------------------------------------------------------------------
                '    Allow up to ten paragraphs in the email
                '
                strCode(0) = .Content("EmailMergeCode00", languageCode, True)
                strCode(1) = .Content("EmailMergeCode01", languageCode, True)          '  
                strCode(2) = .Content("EmailMergeCode02", languageCode, True)          ' 	 
                strCode(3) = .Content("EmailMergeCode03", languageCode, True)          ' 
                strCode(4) = .Content("EmailMergeCode04", languageCode, True)          ' 	Your items will be delivered to :
                strCode(5) = .Content("EmailMergeCode05", languageCode, True)          ' 	<<<Address>>> 
                strCode(6) = .Content("EmailMergeCode06", languageCode, True)          ' 	<<<Lines>>>
                strCode(7) = .Content("EmailMergeCode07", languageCode, True)          ' 	And have been confirmed and will be dispatched to you shortly.
                strCode(8) = .Content("EmailMergeCode08", languageCode, True)          ' 	If we can be of assistance please do not hesitate to contact or customer support department: <<<telephone>>> or by email at <<<email>>>
                strCode(9) = .Content("EmailMergeCode09", languageCode, True)          ' 	Yours sincerely Fred Bloggs <br> Ace IT Suppliers<br>  
                strCode(10) = .Content("EmailMergeCode10", languageCode, True)         ' 	Email  Footer info
                '------------------------------------------------------------------------------------------------
                '    Get data field delimiters
                '
                strField(0) = .Content("NewLine", languageCode, True)
                strField(1) = .Content("ProcessedOrderId", languageCode, True)
                strField(2) = .Content("Loginid", languageCode, True)
                strField(3) = .Content("UserNumber", languageCode, True)
                strField(4) = .Content("Status", languageCode, True)
                strField(5) = .Content("Comment", languageCode, True)
                strField(6) = .Content("ContactName", languageCode, True)
                strField(7) = .Content("AddressLine1", languageCode, True)
                strField(8) = .Content("AddressLine2", languageCode, True)
                strField(9) = .Content("AddressLine3", languageCode, True)
                strField(10) = .Content("AddressLine4", languageCode, True)
                strField(11) = .Content("AddressLine5", languageCode, True)
                strField(12) = .Content("Postcode", languageCode, True)
                strField(13) = .Content("Country", languageCode, True)
                strField(14) = .Content("ContactPhone", languageCode, True)
                strField(15) = .Content("ContactEmail", languageCode, True)
                strField(16) = .Content("PromotionDescription", languageCode, True)
                strField(17) = .Content("SpecialInstructions1", languageCode, True)
                strField(18) = .Content("SpecialInstructions2", languageCode, True)
                strField(19) = .Content("TrackingNo", languageCode, True)
                strField(20) = .Content("Currency", languageCode, True)
                strField(21) = .Content("PaymentType", languageCode, True)
                strField(22) = .Content("BackOfficeOrderId", languageCode, True)
                strField(23) = .Content("BackOfficeStatus", languageCode, True)
                strField(24) = .Content("BackOfficeReference", languageCode, True)
                strField(25) = .Content("Language", languageCode, True)
                strField(26) = .Content("Warehouse", languageCode, True)
                strField(27) = .Content("PurchaseOrder", languageCode, True)

                strField(50) = .Content("TotalOrderItemsValueGross", languageCode, True)
                strField(51) = .Content("TotalOrderItemsValueNet", languageCode, True)
                strField(52) = .Content("TotalOrderItemsTax", languageCode, True)
                strField(53) = .Content("TotalDeliveryGross", languageCode, True)
                strField(54) = .Content("TotalDeliveryTax", languageCode, True)
                strField(55) = .Content("TotalDeliveryNet", languageCode, True)
                strField(56) = .Content("PromotionValue", languageCode, True)
                strField(57) = .Content("TotalOrderValue", languageCode, True)
                strField(58) = .Content("TotalAmountCharged", languageCode, True)
                strField(59) = .Content("TotalValueIncPromo", languageCode, True)

                strField(80) = .Content("CreatedDate", languageCode, True)
                strField(81) = .Content("DeliveryDate", languageCode, True)

                strField(90) = .Content("RegisteredAddressLine1", languageCode, True)
                strField(91) = .Content("RegisteredAddressLine2", languageCode, True)
                strField(92) = .Content("RegisteredAddressLine3", languageCode, True)
                strField(93) = .Content("RegisteredAddressLine4", languageCode, True)
                strField(94) = .Content("RegisteredAddressLine5", languageCode, True)
                strField(95) = .Content("RegisteredPostcode", languageCode, True)
                strField(96) = .Content("RegisteredCountry", languageCode, True)
            End With
            '--------------------------------------------------------------------------------------------------
            '    Get data field values from [tbl_order_header]
            '
            strData(0) = vbCrLf
            strData(1) = dth.Rows(0)("PROCESSED_ORDER_ID")
            strData(2) = dth.Rows(0)("LOGINID")
            ''  strData(3) = dth.Rows(0)("UserNumber")
            strData(4) = dth.Rows(0)("STATUS")
            strData(5) = dth.Rows(0)("COMMENT")
            strData(6) = dth.Rows(0)("CONTACT_NAME")
            strData(7) = dth.Rows(0)("ADDRESS_LINE_1")
            strData(8) = dth.Rows(0)("ADDRESS_LINE_2")
            strData(9) = dth.Rows(0)("ADDRESS_LINE_3")
            strData(10) = dth.Rows(0)("ADDRESS_LINE_4")
            strData(11) = dth.Rows(0)("ADDRESS_LINE_5")
            strData(12) = dth.Rows(0)("POSTCODE")
            strData(13) = dth.Rows(0)("COUNTRY")
            strData(14) = dth.Rows(0)("CONTACT_PHONE")
            strData(15) = dth.Rows(0)("CONTACT_EMAIL")
            strData(16) = dth.Rows(0)("PROMOTION_DESCRIPTION")

            ''strData(17) = dth.Rows(0)("SPECIAL_INSTRUCTIONS_1")
            ''strData(18) = dth.Rows(0)("SPECIAL_INSTRUCTIONS_2")
            ''strData(19) = dth.Rows(0)("TrackingNo")
            ''strData(20) = dth.Rows(0)("CURRENCY")
            ''strData(21) = dth.Rows(0)("PAYMENT_TYPE")
            ''strData(22) = dth.Rows(0)("BACK_OFFICE_ORDER_ID")

            strData(22) = Utilities.CheckForDBNull_String(dth.Rows(0)("BACK_OFFICE_ORDER_ID"))


            ''strData(23) = dth.Rows(0)("BACK_OFFICE_STATUS")
            ''strData(24) = dth.Rows(0)("BACK_OFFICE_REFERENCE")
            ''strData(25) = dth.Rows(0)("LANGUAGE")
            ''strData(26) = dth.Rows(0)("WAREHOUSE")
            strData(27) = Utilities.CheckForDBNull_String(dth.Rows(0)("PURCHASE_ORDER"))

            Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            Dim partner As String = TalentCache.GetPartner(talProfile)
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim currencyCode As String = String.Empty
            currencyCode = TDataObjects.PaymentSettings.GetCurrencyCode(businessUnit, partner)
            strData(50) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS")), businessUnit, partner)
            strData(51) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_NET")), businessUnit, partner)
            strData(52) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_ORDER_ITEMS_TAX")), businessUnit, partner)
            strData(53) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_DELIVERY_GROSS")), businessUnit, partner)
            strData(54) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_DELIVERY_TAX")), businessUnit, partner)
            strData(55) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_DELIVERY_NET")), businessUnit, partner)
            strData(56) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("PROMOTION_VALUE")), businessUnit, partner)
            strData(57) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_ORDER_VALUE")), businessUnit, partner)
            strData(58) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_AMOUNT_CHARGED")), businessUnit, partner)
            strData(59) = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(dth.Rows(0)("TOTAL_ORDER_VALUE")), businessUnit, partner)
            strData(80) = dth.Rows(0)("CREATED_DATE").ToString  ' ("dd mmm yyyy")
            strData(81) = dth.Rows(0)("PROJECTED_DELIVERY_DATE").ToString.Substring(0, 10)  ' ("dd mmm yyyy")
            '------------------------------------------------------------------------------------------------
            Try
                With ProfileHelper.ProfileAddressEnumerator(0, CType(HttpContext.Current.Profile, TalentProfile).User.Addresses)
                    strData(90) = .Address_Line_1
                    strData(91) = .Address_Line_2
                    strData(92) = .Address_Line_3
                    strData(93) = .Address_Line_4
                    strData(94) = .Address_Line_5
                    strData(95) = .Post_Code
                    strData(96) = .Country
                End With
            Catch ex As Exception
                strData(90) = ""
                strData(91) = ""
                strData(92) = ""
                strData(93) = ""
                strData(94) = ""
                strData(95) = ""
            End Try

            Return err
        End Function

        Private Function ConfirmationEmailHeader2(ByVal OrderId As String) As String
            '--------------------------------------------------------------------------------------------------
            '   Load header bits in to an HTML table
            '
            Dim OrderHeader As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            dth = OrderHeader.Get_Header_By_Temp_Order_Id(OrderId)
            '------------------------------------------------------------------------------------------------
            Dim strHead As New StringBuilder
            With strHead
                .Append("<table border=""0""><tr><td>")
                .Append(dth.Rows(0)("PROCESSED_ORDER_ID") & "</td><td></td></tr>")
                .Append("<tr><td>" & ucr.Content("ContactHeaderText", languageCode, True) & "</td>")
                .Append("<td>" & dth.Rows(0)("CONTACT_NAME") & "</td></tr>")
                .Append("<tr><td>" & ucr.Content("AddressHeaderText", languageCode, True) & "</td>")
                .Append("<td>" & dth.Rows(0)("ADDRESS_LINE_1") & "</td></tr>")
                .Append("<tr><td></td><td>" & dth.Rows(0)("ADDRESS_LINE_2") & "</td></tr>")
                .Append("<tr><td></td><td>" & dth.Rows(0)("ADDRESS_LINE_3") & "</td></tr>")
                .Append("<tr><td></td><td>" & dth.Rows(0)("ADDRESS_LINE_4") & "</td></tr>")
                .Append("<tr><td></td><td>" & dth.Rows(0)("ADDRESS_LINE_5") & "</td></tr>")
                .Append("<tr><td></td><td>" & dth.Rows(0)("POSTCODE") & "</td></tr>")
                .Append("<tr><td></td><td>" & dth.Rows(0)("COUNTRY") & "</td></tr></table>")
            End With
            '------------------------------------------------------------------------------------------------
            Return strHead.ToString
        End Function

        Private Function ConfirmationEmailItems(ByVal OrderId As String) As String
            '--------------------------------------------------------------------------------------------------
            '   Load items lines in to an HTML table
            '
            Dim strItem As New StringBuilder
            Dim OrderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
            Dim dtl As DataTable = OrderLines.Get_Order_Lines(OrderId)
            ' 
            With strItem
                '----------------------------------------------------------------------------------------------
                '   Item column headers
                '
                .Append("<table border=""1""><tr><td>")
                .Append(ucr.Content("LineHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("ProductHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("DescriptionHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("QuantityHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("TaxCodeHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("NetHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("TaxHeaderText", languageCode, True) & "</td><td>")
                .Append(ucr.Content("GrossHeaderText", languageCode, True) & "</td></tr>")
                '----------------------------------------------------------------------------------------------
                '   Item column details
                '
                For Each row As Data.DataRow In dtl.Rows
                    .Append("<tr><td>")
                    .Append(row("LINE_NUMBER ") & "</td><td>")
                    .Append(row("PRODUCT_CODE") & "</td><td>")
                    .Append(row("PRODUCT_DESCRIPTION_1") & "</td><td>")
                    .Append(row("QUANTITY") & "</td><td>")
                    .Append(row("TAX_CODE") & "</td><td>")
                    .Append(FormatNumber(row("LINE_PRICE_NET "), 2) & "</td><td>")
                    .Append(FormatNumber(row("LINE_PRICE_TAX"), 2) & "</td><td>")
                    .Append(FormatNumber(row("LINE_PRICE_GROSS"), 2) & "</td></tr>")
                Next
                '----------------------------------------------------------------------------------------------
                '   Net Totals
                '
                .Append("<tr><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append(ucr.Content("TotalNetHeaderText", languageCode, True) & "</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_ORDER_ITEMS_TAX"), 2) & "</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_NET"), 2) & "</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS"), 2) & "</td></tr>")
                '----------------------------------------------------------------------------------------------
                '   Delivery Amounts
                '
                .Append("<tr><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append(ucr.Content("DeliveryText", languageCode, True) & "</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_DELIVERY_TAX"), 2) & "</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_DELIVERY_NET"), 2) & "</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_DELIVERY_GROSS"), 2) & "</td></tr>")
                '----------------------------------------------------------------------------------------------
                '   total Amounts
                '
                .Append("<tr><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append(ucr.Content("TotalHeaderText", languageCode, True) & "</td><td>")
                .Append("</td><td>")
                .Append("</td><td>")
                .Append(FormatNumber(dth.Rows(0)("TOTAL_AMOUNT_CHARGED"), 2) & "</td></tr>")
            End With
            '--------------------------------------------------------------------------------------------------
            Return strItem.ToString
        End Function

    End Class

    Public Class Order
        Inherits ClassBase01
#Region "Private variables"
        Private _deliveryInstructions As String
        Private _merchandiseOrderValue As String
        Private _deliveryContact As String
        Private _building As String
        Private _address2 As String
        Private _address3 As String
        Private _address4 As String
        Private _address5 As String
        Private _postCode As String
        Private _country As String
        Private _countryCode As String
        Private _purchaseOrder As String
        Private _shippingCode As String
        Private _deliveryDate As Date
        Private _cardNumber As String
        Private _expiryDate As String
        Private _startDate As String
        Private _cv2Number As String
        Private _issueNumber As String
        Private _ucr As UserControlResource
        Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        Private _defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
        Private _merchandiseBackOfficeOrdRef As String = String.Empty
#End Region

#Region "Public properties"
        Public Property DeliveryInstructions() As String
            Get
                Return _deliveryInstructions
            End Get
            Set(ByVal value As String)
                _deliveryInstructions = value
            End Set
        End Property
        Public Property DeliveryContact() As String
            Get
                Return _deliveryContact
            End Get
            Set(ByVal value As String)
                _deliveryContact = value
            End Set
        End Property
        Public Property Building() As String
            Get
                Return _building
            End Get
            Set(ByVal value As String)
                _building = value
            End Set
        End Property
        Public Property Address2() As String
            Get
                Return _address2
            End Get
            Set(ByVal value As String)
                _address2 = value
            End Set
        End Property
        Public Property Address3() As String
            Get
                Return _address3
            End Get
            Set(ByVal value As String)
                _address3 = value
            End Set
        End Property
        Public Property Address4() As String
            Get
                Return _address4
            End Get
            Set(ByVal value As String)
                _address4 = value
            End Set
        End Property
        Public Property Address5() As String
            Get
                Return _address5
            End Get
            Set(ByVal value As String)
                _address5 = value
            End Set
        End Property
        Public Property PostCode() As String
            Get
                Return _postCode
            End Get
            Set(ByVal value As String)
                _postCode = value
            End Set
        End Property
        Public Property Country() As String
            Get
                Return _country
            End Get
            Set(ByVal value As String)
                _country = value
            End Set
        End Property

        Public Property CountryCode() As String
            Get
                Return _countryCode
            End Get
            Set(ByVal value As String)
                _countryCode = value
            End Set
        End Property

        Public Property purchaseOrder() As String
            Get
                Return _purchaseOrder
            End Get
            Set(ByVal value As String)
                _purchaseOrder = value
            End Set
        End Property
        Public Property ShippingCode() As String
            Get
                Return _shippingCode
            End Get
            Set(ByVal value As String)
                _shippingCode = value
            End Set
        End Property
        Public Property DeliveryDate() As Date
            Get
                Return _deliveryDate
            End Get
            Set(ByVal value As Date)
                _deliveryDate = value
            End Set
        End Property
        Public Property Installation As Boolean
        Public Property RemoveOld As Boolean
        Public Property MerchandiseOrderReference As String
#End Region

#Region "Contructors"
        Public Sub New()
            MyBase.New()
            _ucr = New Talent.Common.UserControlResource
            _languageCode = Talent.Common.Utilities.GetDefaultLanguage
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "Order.vb"
            End With
        End Sub
        Public Sub New(ByVal deliveryInstructions As String, _
                        ByVal deliveryContact As String, _
                        ByVal building As String, _
                        ByVal address2 As String, _
                        ByVal address3 As String, _
                        ByVal address4 As String, _
                        ByVal address5 As String, _
                        ByVal postCode As String, _
                        ByVal country As String)
            MyBase.New()
            Me.DeliveryInstructions() = deliveryInstructions
            Me.DeliveryContact() = deliveryContact
            Me.Building() = building
            Me.Address2() = address2
            Me.Address3() = address3
            Me.Address4() = address4
            Me.Address5() = address5
            Me.PostCode() = postCode
            Me.Country() = country
        End Sub
        Public Sub New(ByVal deliveryInstructions As String, _
                        ByVal deliveryContact As String, _
                        ByVal building As String, _
                        ByVal address2 As String, _
                        ByVal address3 As String, _
                        ByVal address4 As String, _
                        ByVal address5 As String, _
                        ByVal postCode As String, _
                        ByVal country As String, _
                        ByVal purchaseOrder As String)
            MyBase.New()
            Me.DeliveryInstructions() = deliveryInstructions
            Me.DeliveryContact() = deliveryContact
            Me.Building() = building
            Me.Address2() = address2
            Me.Address3() = address3
            Me.Address4() = address4
            Me.Address5() = address5
            Me.PostCode() = postCode
            Me.Country() = country
            Me.purchaseOrder() = purchaseOrder
        End Sub
        Public Sub New(ByVal deliveryInstructions As String, _
                        ByVal deliveryContact As String, _
                        ByVal building As String, _
                        ByVal address2 As String, _
                        ByVal address3 As String, _
                        ByVal address4 As String, _
                        ByVal address5 As String, _
                        ByVal postCode As String, _
                        ByVal country As String, _
                        ByVal purchaseOrder As String, _
                        ByVal shippingCode As String)
            MyBase.New()
            Me.DeliveryInstructions() = deliveryInstructions
            Me.DeliveryContact() = deliveryContact
            Me.Building() = building
            Me.Address2() = address2
            Me.Address3() = address3
            Me.Address4() = address4
            Me.Address5() = address5
            Me.PostCode() = postCode
            Me.Country() = country
            Me.purchaseOrder() = purchaseOrder
            Me.ShippingCode = shippingCode
        End Sub
        Public Sub New(ByVal deliveryInstructions As String, _
                        ByVal deliveryContact As String, _
                        ByVal building As String, _
                        ByVal address2 As String, _
                        ByVal address3 As String, _
                        ByVal address4 As String, _
                        ByVal address5 As String, _
                        ByVal postCode As String, _
                        ByVal country As String, _
                        ByVal purchaseOrder As String, _
                        ByVal shippingCode As String, _
                        ByVal deliveryDate As Date, ByVal countryCode As String)
            MyBase.New()
            Me.DeliveryInstructions() = deliveryInstructions
            Me.DeliveryContact() = deliveryContact
            Me.Building() = building
            Me.Address2() = address2
            Me.Address3() = address3
            Me.Address4() = address4
            Me.Address5() = address5
            Me.PostCode() = postCode
            Me.Country() = country
            Me.purchaseOrder() = purchaseOrder
            Me.ShippingCode = shippingCode
            Me.DeliveryDate = deliveryDate
            Me.CountryCode() = countryCode
        End Sub

        Public Sub New(ByVal deliveryInstructions As String, _
                       ByVal deliveryContact As String, _
                       ByVal building As String, _
                       ByVal address2 As String, _
                       ByVal address3 As String, _
                       ByVal address4 As String, _
                       ByVal address5 As String, _
                       ByVal postCode As String, _
                       ByVal country As String, _
                       ByVal purchaseOrder As String, _
                       ByVal shippingCode As String, _
                       ByVal deliveryDate As Date, _
                       ByVal installation As Boolean, _
                       ByVal removeOld As Boolean)
            MyBase.New()
            Me.DeliveryInstructions() = deliveryInstructions
            Me.DeliveryContact() = deliveryContact
            Me.Building() = building
            Me.Address2() = address2
            Me.Address3() = address3
            Me.Address4() = address4
            Me.Address5() = address5
            Me.PostCode() = postCode
            Me.Country() = country
            Me.purchaseOrder() = purchaseOrder
            Me.ShippingCode = shippingCode
            Me.DeliveryDate = deliveryDate
            Me.Installation = installation
            Me.RemoveOld = removeOld
        End Sub
#End Region


        Public Function CreateOrder() As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            If TEBUtilities.IsOrderAlreadyPaid() Then
                HttpContext.Current.Session("OrderAlreadyPaid") = "YES"
                Return False
            End If

            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                    Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                    Dim orderid As String = String.Empty
                    Dim dt As New Data.DataTable

                    Dim pr As TalentProfile
                    pr = CType(HttpContext.Current.Profile, TalentProfile)

                    'Check to see if the order exists
                    Try
                        dt = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, pr.UserName)
                    Catch ex As Exception
                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "UCCACO-010", ex.Message, "Error contacting DB for UNPROCESSED order in CreateOrder()", TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                    End Try
                    If dt.Rows.Count > 0 Then
                        'If it does, remove all items and re-add what is in the Basket Files
                        Try
                            orderid = Utilities.CheckForDBNull_String(dt.Rows(0)("TEMP_ORDER_ID"))
                            pr.Basket.TempOrderID = orderid

                            Try
                                orders.Delete_Order_By_Temp_OrderID(orderid)
                            Catch ex As Exception
                                Logging.WriteLog(pr.UserName, "UCCACO-040", ex.Message, "Error deleting UNPROCESSED order header in CreateOrder() - TempOrderID: " & orderid, TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                                Return False
                            End Try

                            Try
                                orderLines.Empty_Order(orderid)
                            Catch ex As Exception
                                Logging.WriteLog(pr.UserName, "UCCACO-030", ex.Message, "Error deleting UNPROCESSED order details in CreateOrder() - TempOrderID: " & orderid, TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                                Return False
                            End Try

                            Return BuildOrder(pr.Basket.TempOrderID, orders, orderLines)

                        Catch ex As Exception
                            Logging.WriteLog(pr.UserName, "UCCACO-020", ex.Message, "Error getting TempOrderID order in CreateOrder()", TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                            Return False
                        End Try
                    Else
                        'Otherwise create the order and add the Basket
                        Utilities.TalentLogging.LoadTestLog("Order.vb", "CreateOrder", timeSpan)
                        Return BuildOrder(pr.Basket.TempOrderID, orders, orderLines)
                    End If
                Case Else
                    Utilities.TalentLogging.LoadTestLog("Order.vb", "CreateOrder", timeSpan)
                    Return True
            End Select

        End Function

        Private Function BuildOrder(ByVal orderid As String, ByVal orders As TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter, ByVal orderLines As TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter) As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
                Case "M", "C"
                    Dim pr As TalentProfile
                    pr = CType(HttpContext.Current.Profile, TalentProfile)

                    Try
                        Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults

                        Dim liveBasketItems As DataTable = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)

                        Dim totals As TalentWebPricing

                        Select Case defaults.PricingType
                            Case 2
                                totals = Utilities.GetPrices_Type2

                            Case Else
                                Dim qtyAndCodes As New Generic.Dictionary(Of String, WebPriceProduct)
                                For Each bItem As Data.DataRow In liveBasketItems.Rows
                                    If UCase(Utilities.CheckForDBNull_String(bItem("MODULE"))) <> "TICKETING" Then
                                        If Not Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(bItem("IS_FREE")) Then
                                            If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))) Then
                                                'Check to see if the multibuys are configured for this master product
                                                Dim myPrice As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(bItem("MASTER_PRODUCT"), 0, bItem("MASTER_PRODUCT"))
                                                If myPrice.PRICE_BREAK_QUANTITY_2 > myPrice.PRICE_BREAK_QUANTITY_1 Then
                                                    'multibuys are configured
                                                    If qtyAndCodes.ContainsKey(bItem("MASTER_PRODUCT")) Then
                                                        qtyAndCodes(bItem("MASTER_PRODUCT")).Quantity += Utilities.CheckForDBNull_Decimal(bItem("QUANTITY"))
                                                    Else
                                                        ' Pass in product otherwise Promotions don't work properly
                                                        'qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("MASTER_PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
                                                        qtyAndCodes.Add(bItem("MASTER_PRODUCT"), New Talent.Common.WebPriceProduct(bItem("PRODUCT"), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
                                                    End If
                                                Else
                                                    If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
                                                        qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), bItem("MASTER_PRODUCT")))
                                                    End If
                                                End If
                                            Else
                                                If Not qtyAndCodes.ContainsKey(Utilities.CheckForDBNull_String(bItem("PRODUCT"))) Then
                                                    qtyAndCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")), New Talent.Common.WebPriceProduct(Utilities.CheckForDBNull_String(bItem("PRODUCT")), Utilities.CheckForDBNull_Decimal(bItem("QUANTITY")), Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))))
                                                End If
                                            End If
                                        End If
                                    End If
                                Next

                                totals = Utilities.GetWebPrices_WithTotals(qtyAndCodes, orderid, defaults.PromotionPriority)
                                'modify the web price totals
                                totals = Utilities.GetModifiedWebPrices(defaults.WebPricesModifyingMode, CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id, liveBasketItems, totals, Utilities.GetCurrentPageName())

                        End Select

                        If Not totals Is Nothing Then

                            Dim currencyCode As String = String.Empty
                            Try
                                currencyCode = TDataObjects.PaymentSettings.GetCurrencyCode(TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                            Catch ex As Exception
                            End Try

                            Try

                                'Attempt to return the projected delivery date so that it can be
                                'stamped onto the order header
                                '-----------------------------------------
                                Dim projectedDate As Date = Date.MinValue
                                If String.IsNullOrEmpty(DeliveryDate.ToString) Then
                                    projectedDate = Utilities.GetDeliveryDate(HttpContext.Current.Profile)
                                    If projectedDate = Date.MinValue Then
                                        projectedDate = Now.AddDays(5)
                                    End If
                                Else
                                    If DeliveryDate = Date.MinValue Then
                                        projectedDate = Utilities.GetDeliveryDate(HttpContext.Current.Profile)
                                        If projectedDate = Date.MinValue Then
                                            projectedDate = Now.AddDays(5)
                                        End If
                                    Else
                                        projectedDate = DeliveryDate
                                    End If
                                End If
                                '----------------------------------------
                                Dim delG As Decimal = 0, _
                                    delN As Decimal = 0, _
                                    delT As Decimal = 0, _
                                    tot As Decimal = 0

                                If defaults.DeliveryCalculationInUse Then

                                    Select Case UCase(defaults.DeliveryPriceCalculationType)
                                        Case "UNIT", "WEIGHT"
                                            tot = totals.Total_Items_Value_Gross
                                        Case Else
                                            '-----------------------------------------------
                                            ' Free delivery PROMOTIONS must still be written 
                                            ' with delivery value to balance totals
                                            '-----------------------------------------------
                                            If Not totals.Qualifies_For_Free_Delivery OrElse totals.FreeDeliveryPromotion Then
                                                delG = totals.Max_Delivery_Gross
                                                delN = totals.Max_Delivery_Net
                                                delT = totals.Max_Delivery_Tax
                                            End If
                                            tot = totals.Total_Order_Value_Gross
                                    End Select
                                Else
                                    tot = totals.Total_Items_Value_Gross
                                End If

                                If totals.Total_Promotions_Value > 0 Then
                                    tot = totals.Total_Items_Value_Gross - totals.Total_Promotions_Value
                                End If

                                orders.Create_Order_Declare_All(orderid, _
                                                                TalentCache.GetBusinessUnit(), _
                                                                TalentCache.GetPartner(pr), _
                                                                pr.UserName, _
                                                                Talent.Common.Utilities.GetOrderStatus("DELIVERY"), _
                                                                DeliveryInstructions(), _
                                                                DeliveryContact(), _
                                                                Building(), _
                                                                Address2(), _
                                                                Address3(), _
                                                                Address4(), _
                                                                Address5(), _
                                                                UCase(PostCode()), _
                                                                Country(), _
                                                                pr.User.Details.Telephone_Number, _
                                                                pr.User.Details.Email, _
                                                                Now, _
                                                                Now, _
                                                                Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Gross, 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Net, 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(totals.Total_Items_Value_Tax, 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(delG, 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(delN, 0.01, False), _
                                                                Talent.eCommerce.Utilities.RoundToValue(delT, 0.01, False), _
                                                                "", _
                                                                0, _
                                                                Talent.eCommerce.Utilities.RoundToValue(tot, 0.01, False), _
                                                                0, _
                                                                projectedDate, _
                                                                currencyCode, _
                                                                purchaseOrder(), _
                                                                ShippingCode, CountryCode)

                            Catch ex As Exception
                                Logging.WriteLog(pr.UserName, "UCCABO-030", ex.Message, "Error creating new order in BuildOrder() - TempOrderID: " & orderid, TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                                Return False
                            End Try
                            Try
                                Return Add_OrderLines_To_Order(totals, orderid, currencyCode)
                                'Return True
                            Catch ex As Exception
                                Logging.WriteLog(pr.UserName, "UCCABO-040", ex.Message, "Error adding order lines to the order in BuildOrder() - TempOrderID: " & orderid, TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                                Return False
                            End Try
                        Else
                            Return False
                        End If
                    Catch ex As Exception
                        Logging.WriteLog(pr.UserName, "UCCABO-010", ex.Message, "Error retrieving basket details to create order in BuildOrder() - TempOrderID: " & orderid, TalentCache.GetBusinessUnit, TalentCache.GetPartner(pr), ProfileHelper.GetPageName, "CheckoutDeliverAddress.ascx")
                        Utilities.TalentLogging.LoadTestLog("Order.vb", "BuildOrder", timeSpan)
                        Return False
                    End Try

                Case Else
                    Utilities.TalentLogging.LoadTestLog("Order.vb", "BuildOrder", timeSpan)
                    Return True

            End Select


        End Function


        Public Function Add_OrderLines_To_Order(ByVal totals As Talent.Common.TalentWebPricing, ByVal tempOrderID As String, ByVal currencyCode As String) As Boolean
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay

            Add_OrderLines_To_Order = False

            Dim ProductInfo As DataTable = Me.GetProductInformation()
            Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            Dim liveBasketItems As DataTable = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)

            Dim lineNo As Integer = 0

            Const insertStr As String = "   INSERT INTO [dbo].[tbl_order_detail] " & _
                                        "   ( " & _
                                        "       [ORDER_ID], " & _
                                        "       [LINE_NUMBER], " & _
                                        "       [PRODUCT_CODE], " & _
                                        "       [QUANTITY], " & _
                                        "       [PRODUCT_DESCRIPTION_1], " & _
                                        "       [PRODUCT_DESCRIPTION_2], " & _
                                        "       [GROUP_LEVEL_01], " & _
                                        "       [GROUP_LEVEL_02], " & _
                                        "       [GROUP_LEVEL_03], " & _
                                        "       [GROUP_LEVEL_04], " & _
                                        "       [GROUP_LEVEL_05], " & _
                                        "       [GROUP_LEVEL_06], " & _
                                        "       [GROUP_LEVEL_07], " & _
                                        "       [GROUP_LEVEL_08], " & _
                                        "       [GROUP_LEVEL_09], " & _
                                        "       [GROUP_LEVEL_10], " & _
                                        "       [PRODUCT_SUPPLIER], " & _
                                        "       [PURCHASE_PRICE_GROSS], " & _
                                        "       [PURCHASE_PRICE_NET], " & _
                                        "       [PURCHASE_PRICE_TAX], " & _
                                        "       [DELIVERY_GROSS], " & _
                                        "       [DELIVERY_NET], " & _
                                        "       [DELIVERY_TAX], " & _
                                        "       [TAX_CODE], " & _
                                        "       [LINE_PRICE_GROSS], " & _
                                        "       [LINE_PRICE_NET], " & _
                                        "       [LINE_PRICE_TAX], " & _
                                        "       [CURRENCY], " & _
                                        "       [MASTER_PRODUCT], " & _
                                        "       [INSTRUCTIONS], " & _
                                        "       [COST_CENTRE], " & _
                                        "       [ACCOUNT_CODE] " & _
                                        "   ) " & _
                                        " "

            Dim commandStr As String = ""


            Dim cmd As New SqlClient.SqlCommand(commandStr, _
                                            New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            With cmd.Parameters
                .Add("@OrderID", SqlDbType.NVarChar).Value = tempOrderID
                .Add("@Currency", SqlDbType.NVarChar).Value = currencyCode
            End With

            If Not totals.RetrievedPrices Is Nothing Then
                Dim pDesc1 As String = "", _
                    pDesc2 As String = "", _
                    groupL01 As String = "", _
                    groupL02 As String = "", _
                    groupL03 As String = "", _
                    groupL04 As String = "", _
                    groupL05 As String = "", _
                    groupL06 As String = "", _
                    groupL07 As String = "", _
                    groupL08 As String = "", _
                    groupL09 As String = "", _
                    groupL10 As String = "", _
                    supplier As String = "", _
                    taxCode As String = "", _
                    masterProduct As String = "", _
                    wp As New Talent.Common.DEWebPrice, _
                    count As Integer = 0


                If liveBasketItems.Rows.Count > 0 Then
                    For Each bItem As DataRow In liveBasketItems.Rows
                        count += 1
                        lineNo += 1

                        Dim prodCode As String = Utilities.CheckForDBNull_String(bItem("PRODUCT"))
                        Dim masterCode As String = Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))
                        Dim prodQty As Decimal = Utilities.CheckForDBNull_Decimal(bItem("QUANTITY"))
                        Dim prodIsFree As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(bItem("IS_FREE"))
                        Dim xmlConfig As String = Utilities.CheckForDBNull_String(bItem("XML_CONFIG"))
                        Dim costCentre As String = Utilities.CheckForDBNull_String(bItem("COST_CENTRE"))
                        Dim accountCode As String = Utilities.CheckForDBNull_String(bItem("ACCOUNT_CODE"))
                        Dim instructions As String = String.Empty
                        If xmlConfig = "1" And Not HttpContext.Current.Session("personalisationTransactions") Is Nothing Then
                            Dim dt As Data.DataTable = New Data.DataTable
                            dt = HttpContext.Current.Session("personalisationTransactions")
                            'Loop Session("personalisationTransactions") and look for ProdCode
                            'then stringbuild them into instruction variable
                            For Each dr As DataRow In dt.Rows
                                If dr("PRODUCT_CODE") = prodCode Then instructions += "(" & dr("COMPONENT_NAME") & "=" & dr("COMPONENT_VALUE") & ")"
                            Next
                        End If
                        commandStr += insertStr
                        commandStr += String.Format( _
                                        " VALUES (     " & _
                                        "   @OrderID, " & _
                                        "   @LineNo{0}, " & _
                                        "   @ProductCode{0}, " & _
                                        "   @Quantity{0}, " & _
                                        "   @ProductDescription1_{0}, " & _
                                        "   @ProductDescription2_{0}, " & _
                                        "   @GroupLevel01_{0}, " & _
                                        "   @GroupLevel02_{0}, " & _
                                        "   @GroupLevel03_{0}, " & _
                                        "   @GroupLevel04_{0}, " & _
                                        "   @GroupLevel05_{0}, " & _
                                        "   @GroupLevel06_{0}, " & _
                                        "   @GroupLevel07_{0}, " & _
                                        "   @GroupLevel08_{0}, " & _
                                        "   @GroupLevel09_{0}, " & _
                                        "   @GroupLevel10_{0}, " & _
                                        "   @Supplier{0}, " & _
                                        "   @PurchasePriceGross{0}, " & _
                                        "   @PurchasePriceNet{0}, " & _
                                        "   @PurchasePriceTax{0}, " & _
                                        "   @DeliveryGross{0}, " & _
                                        "   @DeliveryNet{0}, " & _
                                        "   @DeliveryTax{0}, " & _
                                        "   @TaxCode{0}, " & _
                                        "   @LinePriceGross{0}, " & _
                                        "   @LinePriceNet{0}, " & _
                                        "   @LinePriceTax{0}, " & _
                                        "   @Currency, " & _
                                        "   @MasterProduct{0}, " & _
                                        "   @Instructions{0}, " & _
                                        "   @CostCentre{0}, " & _
                                        "   @AccountCode{0} " & _
                                        ");  " _
                                        , count)

                        'empty the variables for each itteration
                        '---------------------------------------
                        pDesc1 = ""
                        pDesc2 = ""
                        groupL01 = ""
                        groupL02 = ""
                        groupL03 = ""
                        groupL04 = ""
                        groupL05 = ""
                        groupL06 = ""
                        groupL07 = ""
                        groupL08 = ""
                        groupL09 = ""
                        groupL10 = ""
                        supplier = ""
                        taxCode = ""
                        wp = New Talent.Common.DEWebPrice
                        '---------------------------------------

                        'try to extract the product from the product prices that were retreived only if it is not free
                        '-------------------------------------------------------------------------
                        If Not prodIsFree Then
                            If totals.RetrievedPrices.ContainsKey(prodCode) Then
                                wp = totals.RetrievedPrices(prodCode)
                            Else
                                If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(bItem("MASTER_PRODUCT"))) Then
                                    'if the master product is populated it will have been used to get the prices
                                    If totals.RetrievedPrices.ContainsKey(bItem("MASTER_PRODUCT")) Then
                                        wp = totals.RetrievedPrices(bItem("MASTER_PRODUCT"))
                                    Else
                                        Select Case _defaults.PricingType
                                            Case 2
                                                'do nothing... this should never be reached for type 2
                                            Case Else
                                                'if no record found, try to get the record individually
                                                Try

                                                    Dim newTotals As Talent.Common.TalentWebPricing
                                                    newTotals = Utilities.GetWebPrices_WithTotals(prodCode, _
                                                                                                    prodQty, _
                                                                                                    tempOrderID, _
                                                                                                    _defaults.PromotionPriority, _
                                                                                                    bItem("MASTER_PRODUCT"), _
                                                                                                    "")
                                                    If newTotals IsNot Nothing Then
                                                        If newTotals.RetrievedPrices.ContainsKey(prodCode) Then
                                                            wp = newTotals.RetrievedPrices(prodCode)
                                                        ElseIf newTotals.RetrievedPrices.ContainsKey(prodCode) Then
                                                            wp = newTotals.RetrievedPrices(bItem("MASTER_PRODUCT"))
                                                        End If
                                                    End If
                                                Catch ex As Exception
                                                End Try
                                        End Select
                                    End If
                                End If
                            End If
                        End If

                        '-------------------------------------------------------------------------

                        If Not ProductInfo Is Nothing Then
                            For Each product As DataRow In ProductInfo.Rows
                                If UCase(Utilities.CheckForDBNull_String(product("PRODUCT_CODE"))) = UCase(prodCode) Then
                                    pDesc1 = Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_1"))
                                    pDesc2 = Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_2"))
                                    groupL01 = Utilities.CheckForDBNull_String(product("GROUP_L01_GROUP"))
                                    groupL02 = Utilities.CheckForDBNull_String(product("GROUP_L02_GROUP"))
                                    groupL03 = Utilities.CheckForDBNull_String(product("GROUP_L03_GROUP"))
                                    groupL04 = Utilities.CheckForDBNull_String(product("GROUP_L04_GROUP"))
                                    groupL05 = Utilities.CheckForDBNull_String(product("GROUP_L05_GROUP"))
                                    groupL06 = Utilities.CheckForDBNull_String(product("GROUP_L06_GROUP"))
                                    groupL07 = Utilities.CheckForDBNull_String(product("GROUP_L07_GROUP"))
                                    groupL08 = Utilities.CheckForDBNull_String(product("GROUP_L08_GROUP"))
                                    groupL09 = Utilities.CheckForDBNull_String(product("GROUP_L09_GROUP"))
                                    groupL10 = Utilities.CheckForDBNull_String(product("GROUP_L10_GROUP"))
                                    supplier = Utilities.CheckForDBNull_String(product("PRODUCT_SUPPLIER"))
                                    taxCode = Utilities.CheckForDBNull_String(wp.TAX_CODE)

                                    Exit For
                                End If
                            Next
                        End If


                        With cmd.Parameters

                            .Add("@ProductCode" & count, SqlDbType.NVarChar).Value = prodCode
                            .Add("@LineNo" & count, SqlDbType.Int).Value = lineNo
                            .Add("@Quantity" & count, SqlDbType.Decimal).Value = prodQty
                            .Add("@ProductDescription1_" & count, SqlDbType.NVarChar).Value = pDesc1
                            .Add("@ProductDescription2_" & count, SqlDbType.NVarChar).Value = pDesc2
                            .Add("@GroupLevel01_" & count, SqlDbType.NVarChar).Value = groupL01
                            .Add("@GroupLevel02_" & count, SqlDbType.NVarChar).Value = groupL02
                            .Add("@GroupLevel03_" & count, SqlDbType.NVarChar).Value = groupL03
                            .Add("@GroupLevel04_" & count, SqlDbType.NVarChar).Value = groupL04
                            .Add("@GroupLevel05_" & count, SqlDbType.NVarChar).Value = groupL05
                            .Add("@GroupLevel06_" & count, SqlDbType.NVarChar).Value = groupL06
                            .Add("@GroupLevel07_" & count, SqlDbType.NVarChar).Value = groupL07
                            .Add("@GroupLevel08_" & count, SqlDbType.NVarChar).Value = groupL08
                            .Add("@GroupLevel09_" & count, SqlDbType.NVarChar).Value = groupL09
                            .Add("@GroupLevel10_" & count, SqlDbType.NVarChar).Value = groupL10
                            .Add("@Supplier" & count, SqlDbType.NVarChar).Value = supplier
                            .Add("@MasterProduct" & count, SqlDbType.NVarChar).Value = masterProduct
                            .Add("@Instructions" & count, SqlDbType.NVarChar).Value = instructions
                            .Add("@costCentre" & count, SqlDbType.NVarChar).Value = costCentre
                            .Add("@accountCode" & count, SqlDbType.NVarChar).Value = accountCode

                            If prodIsFree Then
                                .Add("@DeliveryGross" & count, SqlDbType.Decimal).Value = 0
                                .Add("@DeliveryNet" & count, SqlDbType.Decimal).Value = 0
                                .Add("@DeliveryTax" & count, SqlDbType.Decimal).Value = 0
                                .Add("@TaxCode" & count, SqlDbType.NVarChar).Value = ""
                                .Add("@PurchasePriceGross" & count, SqlDbType.Decimal).Value = 0
                                .Add("@PurchasePriceNet" & count, SqlDbType.Decimal).Value = 0
                                .Add("@PurchasePriceTax" & count, SqlDbType.Decimal).Value = 0
                                .Add("@LinePriceGross" & count, SqlDbType.Decimal).Value = 0
                                .Add("@LinePriceNet" & count, SqlDbType.Decimal).Value = 0
                                .Add("@LinePriceTax" & count, SqlDbType.Decimal).Value = 0
                            Else
                                .Add("@DeliveryGross" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.DELIVERY_GROSS_PRICE, 0.01, False)
                                .Add("@DeliveryNet" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.DELIVERY_NET_PRICE, 0.01, False)
                                .Add("@DeliveryTax" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.DELIVERY_TAX_AMOUNT, 0.01, False)
                                .Add("@TaxCode" & count, SqlDbType.NVarChar).Value = Utilities.CheckForDBNull_String(wp.TAX_CODE)
                                .Add("@PurchasePriceGross" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.Purchase_Price_Gross, 0.01, False)
                                .Add("@PurchasePriceNet" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.Purchase_Price_Net, 0.01, False)
                                .Add("@PurchasePriceTax" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.Purchase_Price_Tax, 0.01, False)
                                .Add("@LinePriceGross" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.Purchase_Price_Gross * prodQty, 0.01, False)
                                .Add("@LinePriceNet" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.Purchase_Price_Net * prodQty, 0.01, False)
                                .Add("@LinePriceTax" & count, SqlDbType.Decimal).Value = Talent.eCommerce.Utilities.RoundToValue(wp.Purchase_Price_Tax * prodQty, 0.01, False)
                            End If

                        End With
                    Next
                End If



                Try
                    cmd.Connection.Open()
                Catch ex As Exception
                End Try

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.CommandText = commandStr
                        cmd.ExecuteNonQuery()
                        Add_OrderLines_To_Order = True
                    End If
                Catch ex As Exception
                End Try

                Try
                    If cmd.Connection.State = ConnectionState.Open Then
                        cmd.Connection.Close()
                    End If
                Catch ex As Exception
                End Try
            End If

            Utilities.TalentLogging.LoadTestLog("Order.vb", "Add_OrderLines_To_Order", timeSpan)
            Return Add_OrderLines_To_Order

        End Function


        Public Shared Function GetLastAccountNo(ByVal loginId As String) As String
            Dim lastAccountNo As String
            lastAccountNo = String.Empty

            Const commandStr As String = "Select MAX(ACCOUNT_CODE) " & _
                                        "FROM tbl_order_detail AS A " & _
                                        "WHERE EXISTS ( " & _
                                        "SELECT   * FROM tbl_order_header AS B " & _
                                        "WHERE " & _
                                        "(A.HEADER_ORDER_ID = B.PROCESSED_ORDER_ID) AND " & _
                                        "(B.CREATED_DATE = (SELECT MAX(CREATED_DATE) FROM tbl_order_header AS C WHERE " & _
                                        "(C.LOGINID = @LOGINID))))"

            Dim dtr As SqlClient.SqlDataReader = Nothing
            Dim cmd As New SqlClient.SqlCommand(commandStr, _
                                            New SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

            With cmd.Parameters
                .Add("@LOGINID", SqlDbType.NVarChar).Value = loginId
            End With

            Try
                cmd.Connection.Open()
            Catch ex As Exception
            End Try

            Try
                If cmd.Connection.State = ConnectionState.Open Then
                    cmd.CommandText = commandStr
                    dtr = cmd.ExecuteReader()

                    If dtr.HasRows Then
                        dtr.Read()
                        If Not dtr.Item(0) Is Nothing Then
                            lastAccountNo = dtr.Item(0).ToString
                        End If

                        dtr.Close()
                    End If
                End If
            Catch ex As Exception
                Dim test As String = ex.ToString
            End Try

            Try
                If cmd.Connection.State = ConnectionState.Open Then
                    cmd.Connection.Close()
                End If
            Catch ex As Exception
            End Try

            Return lastAccountNo
        End Function

        Private Function GetProductInformation() As DataTable
            Dim err As Talent.Common.ErrorObj
            Dim products As DataTable
            Dim productCodes As New ArrayList

            Dim liveBasketItems As DataTable = TDataObjects.BasketSettings.TblBasketDetail.GetNonTicketingDetailByBasketHeaderID(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)


            For Each bItem As DataRow In liveBasketItems.Rows
                'no "is free" check because we want details of all products
                productCodes.Add(Utilities.CheckForDBNull_String(bItem("PRODUCT")))
            Next

            Dim prodInfo As New Talent.Common.DEProductInfo(TalentCache.GetBusinessUnit, _
                                                                 TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                 productCodes, _
                                                                 Talent.Common.Utilities.GetDefaultLanguage)

            Dim DBProdInfo As New Talent.Common.DBProductInfo(prodInfo)
            DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

            'Get the product info
            '------------------------
            err = DBProdInfo.AccessDatabase()

            If Not err.HasError Then
                products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
            Else
                'ERROR: could not retrieve product info
                products = Nothing
            End If

            Return products
        End Function

        Public Sub ProcessMerchandiseInBackend(ByVal isOrderAlreadyFinalised As Boolean, ByVal isInvoicePayType As Boolean, ByVal generatedWebOrderNo As String, ByVal needsReview As Boolean, Optional paymentReference As String = "")
            If CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE OrElse CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                HttpContext.Current.Session("BackendProcessInCheckout") = True
                Dim exceptionList As List(Of Exception) = Nothing
                If Not isOrderAlreadyFinalised Then
                    Logging.WriteLog(HttpContext.Current.Profile.UserName, "PLCOC-200", HttpContext.Current.Profile.UserName & " Invoice or External Gateway, Finalising Order...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                    If FinaliseOrder(isInvoicePayType, generatedWebOrderNo) Then
                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "PLCOC-210", HttpContext.Current.Profile.UserName & " - Order Finalised", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                        Insert_Order_Status_Flow("ORDER COMPLETE")
                    Else
                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "PLCOC-220", HttpContext.Current.Profile.UserName & " - Failed to Finalise Order - Front-End", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                        Insert_Order_Status_Flow("ORDER FAILED", "Payment recieved but order creation failed")
                    End If
                End If

                ' Included this try/catch so that the finally statement will be hit, as we ALWAYS want the 
                ' basket to be updated as complete regardless of whether the order was placed at the back end
                Dim ORDER As New TalentOrder

                Try
                    Logging.WriteLog(HttpContext.Current.Profile.UserName, "COC-230", HttpContext.Current.Profile.UserName & " - Creating Back-End Order", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                    Dim tempOrderId As String = CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID
                    Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                    Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(tempOrderId)

                    If dt.Rows.Count > 0 Then
                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "COC-240", HttpContext.Current.Profile.UserName & " - Front_end Order Retrieved", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                        _merchandiseOrderValue = (Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE")) - Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE")))
                        MerchandiseOrderReference = dt.Rows(0)("PROCESSED_ORDER_ID")

                        ' Setup the Gift Message
                        '------------------------
                        ProcessGiftMessage(dt.Rows(0)("TEMP_ORDER_ID"), dt.Rows(0)("PROCESSED_ORDER_ID"), _defaults)

                        '-------------------------
                        ' Place the order in Sys21
                        ' Only if status Complete
                        '-------------------------

                        If dt.Rows(0)("STATUS") = Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE") Then
                            Logging.WriteLog(HttpContext.Current.Profile.UserName, "COC-250", HttpContext.Current.Profile.UserName & " - Front-End Order appears correct", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                            Dim err As ErrorObj = Nothing
                            Dim dep As DEOrder = New DEOrder
                            Dim settings As New DESettings
                            Dim index As Integer = 0
                            Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                            Dim dt2 As Data.DataTable = orderLines.Get_Order_Lines(tempOrderId)
                            Dim xmlOrderAttributes As New Dictionary(Of String, String)

                            If dt2.Rows.Count > 0 Then
                                Logging.WriteLog(HttpContext.Current.Profile.UserName, "COC-260", HttpContext.Current.Profile.UserName & " - Front-End Order Lines retrieved, checking for Promotions", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                ' Check if any promotions used
                                Dim promoCode As String = String.Empty
                                Dim promoDescription As String = String.Empty
                                Dim promoRedeemed As New Data.DataTable
                                promoRedeemed = Talent.eCommerce.Utilities.GetPromotions_Redeemed(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, HttpContext.Current.Profile.UserName)
                                Dim promoCodesAll As String = String.Empty
                                If promoRedeemed.Rows.Count > 0 Then
                                    For Each promo As Data.DataRow In promoRedeemed.Rows
                                        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(promo("SUCCESS")) Then
                                            promoCode = Talent.eCommerce.Utilities.CheckForDBNull_String(promo("PROMOTION_CODE"))
                                            promoDescription = Talent.eCommerce.Utilities.CheckForDBNull_String(promo("PROMOTION_DISPLAY_NAME"))
                                            promoCodesAll = promoCodesAll & promoCode & ";"
                                        End If
                                    Next promo
                                End If

                                '--------------------------------------------------------------------------
                                ' Set account from user if filled. If not pick up from defaults for partner
                                '--------------------------------------------------------------------------
                                If _defaults.SetBackendAccountNoFromUserDetails Then
                                    settings.AccountNo1 = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1
                                    settings.AccountNo2 = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_2
                                End If
                                If String.IsNullOrEmpty(settings.AccountNo1) Then
                                    settings.AccountNo1 = CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.Account_No_1
                                    settings.AccountNo2 = CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.Account_No_2
                                End If

                                settings.DestinationDatabase = ModuleDefaults.OrderDestinationDatabase
                                Select Case settings.DestinationDatabase
                                    Case Is = "SYSTEM21"
                                        settings.BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                                End Select

                                settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                                settings.RetryFailures = _defaults.OrderCreationRetry
                                settings.RetryAttempts = _defaults.OrderCreationRetryAttempts
                                settings.RetryWaitTime = _defaults.OrderCreationRetryWait
                                settings.RetryErrorNumbers = _defaults.OrderCreationRetryErrors
                                settings.BusinessUnit = TalentCache.GetBusinessUnit()
                                settings.LoginId = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                                Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-270", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Setting Header Values on Back-end Datatable", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                xmlOrderAttributes = TDataObjects.AppVariableSettings.TblDefaults.GetXmlOrderAttributes(TalentCache.GetBusinessUnit, _languageCode)
                                '-----------------------------------------------
                                ' Build DataStructure for order creation routine
                                '-----------------------------------------------

                                Dim detr As New DETransaction           ' Items
                                Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
                                Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
                                Dim dead1 As New DeAddress              ' Single Line Item
                                Dim dead2 As New DeAddress              ' Single Line Item
                                Dim deop As New DEPayments              ' Multiple Line Item
                                Dim deoc As New DECharges               ' Multiple Line Item
                                Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
                                Dim depr As DeProductLines              ' Multiple Line Item
                                Dim decl As DeCommentLines              ' Multiple Comment lines

                                deos = New DeOrders
                                deoh = New DeOrderHeader
                                dead1 = New DeAddress
                                dead1.Category = "ShipTo"
                                dead1.ContactName = dt.Rows(0)("CONTACT_NAME")
                                Try
                                    dead1.PhoneNumber = dt.Rows(0)("CONTACT_PHONE")
                                Catch ex As Exception

                                End Try
                                dead1.Line1 = dt.Rows(0)("ADDRESS_LINE_1")
                                dead1.Line2 = dt.Rows(0)("ADDRESS_LINE_2")
                                dead1.Line3 = dt.Rows(0)("ADDRESS_LINE_3")
                                dead1.City = dt.Rows(0)("ADDRESS_LINE_4")
                                dead1.Province = dt.Rows(0)("ADDRESS_LINE_5")
                                Dim s As String = dt.Rows(0)("COUNTRY")
                                dead1.Country = dt.Rows(0)("COUNTRY")
                                dead1.PostalCode = dt.Rows(0)("POSTCODE")
                                dead1.Title = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Title
                                dead1.Forename = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Forename
                                dead1.Surname = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Surname
                                dead1.PhoneNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Telephone_Number
                                ' Set delivery email from registration
                                dead1.Email = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email
                                deoh.AutoRelease = ""
                                deoh.BackOrderFlag = ""
                                deoh.BillToSuffix = ""
                                deoh.BranchOrderNumber = ""
                                deoh.CarrierCode = ""
                                deoh.CarrierCodeValue = ""
                                deoh.Category = ""
                                deoh.CustomerPO = dt.Rows(0)("PURCHASE_ORDER")
                                deoh.EndUserPO = ""
                                deoh.MessageLines = ""
                                deoh.NewCustomerPO = ""
                                deoh.OrderActionCode = ""
                                deoh.OrderDueDate = ""
                                deoh.OrderSuffix = ""
                                deoh.PaymentType = Talent.Common.Utilities.CheckForDBNull_String(dt.Rows(0)("PAYMENT_TYPE"))
                                deoh.PaymentSubType = Talent.Common.Utilities.CheckForDBNull_String(dt.Rows(0)("CREDIT_CARD_TYPE"))
                                deoh.ProjectedDeliveryDate = dt.Rows(0)("PROJECTED_DELIVERY_DATE")
                                deoh.SalesPerson = ""
                                deoh.ShipFromBranches = ""
                                deoh.ShipToSuffix = ""
                                deoh.ShippingCode = Talent.Common.Utilities.CheckForDBNull_String(dt.Rows(0)("SHIPPING_CODE"))
                                deoh.SplitLine = ""
                                deoh.SplitShipmentFlag = ""
                                'related to HSBC??
                                If needsReview Then
                                    deoh.SuspendCode = "HS"
                                Else
                                    deoh.SuspendCode = ""
                                End If
                                '-------------------------
                                ' Set payment details HERE
                                '-------------------------
                                deop.CardNumber = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
                                deop.ExpiryDate = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
                                deop.StartDate = Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE)
                                deop.CV2Number = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)
                                deop.IssueNumber = Checkout.RetrievePaymentItemFromSession("IssueNumber", GlobalConstants.CHECKOUTASPXSTAGE)

                                deoh.CollDEPayments.Add(deop)
                                deoh.WebOrderNumber = dt.Rows(0)("PROCESSED_ORDER_ID")
                                deoh.TaxDisplay1 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_DISPLAY_1"))
                                deoh.TaxDisplay2 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_DISPLAY_2"))
                                deoh.TaxDisplay3 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_DISPLAY_3"))
                                deoh.TaxDisplay4 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_DISPLAY_4"))
                                deoh.TaxDisplay5 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_DISPLAY_5"))
                                deoh.TaxInclusive1 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_INCLUSIVE_1"))
                                deoh.TaxInclusive2 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_INCLUSIVE_2"))
                                deoh.TaxInclusive3 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_INCLUSIVE_3"))
                                deoh.TaxInclusive4 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_INCLUSIVE_4"))
                                deoh.TaxInclusive5 = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("TAX_INCLUSIVE_5"))
                                deoh.TaxCode1 = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("TAX_CODE_1"))
                                deoh.TaxCode2 = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("TAX_CODE_2"))
                                deoh.TaxCode3 = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("TAX_CODE_3"))
                                deoh.TaxCode4 = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("TAX_CODE_4"))
                                deoh.TaxCode5 = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("TAX_CODE_5"))
                                deoh.TaxAmount1 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TAX_AMOUNT_1"))
                                deoh.TaxAmount2 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TAX_AMOUNT_2"))
                                deoh.TaxAmount3 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TAX_AMOUNT_3"))
                                deoh.TaxAmount4 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TAX_AMOUNT_4"))
                                deoh.TaxAmount5 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TAX_AMOUNT_5"))
                                deoh.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.User_Number.ToString
                                deoh.CustomerNumberPrefix = CType(HttpContext.Current.Profile, TalentProfile).User.Details.User_Number_Prefix
                                deoh.Currency = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("CURRENCY"))

                                '
                                ' Current record set contains header before the gift message was updated, so check again
                                If TestForGiftMessage(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID) Then
                                    deoh.Message = True
                                Else
                                    deoh.Message = False
                                End If
                                deoh.BasketNumber = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID.ToString
                                deoh.PromotionCode = promoCode
                                deoh.PromotionCodeDescription = promoDescription
                                Dim charSeparators() As Char = {";"c}
                                deoh.PromotionCodes = promoCodesAll.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries)
                                Dim dtBasketPayment As DataTable = TDataObjects.BasketSettings.TblBasketPayment.GetPaymentRecordByBasketHeaderID(deoh.BasketNumber)
                                If dtBasketPayment.Rows.Count = 1 Then
                                    deoh.BasketPaymentID = TEBUtilities.CheckForDBNull_Long(dtBasketPayment.Rows(0)("BASKET_PAYMENT_ID"))
                                End If

                                '--------------------------------------
                                ' Set bill to address from registration
                                '--------------------------------------
                                Dim registrationAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, CType(HttpContext.Current.Profile, TalentProfile).User.Addresses)
                                dead2 = New DeAddress
                                dead2.Category = "BillTo"
                                '----------------------------------------
                                ' New default to deternmine whether to 
                                ' set the billing address the same as the 
                                ' ship to address
                                '----------------------------------------
                                If _defaults.OrderPassRegistrationAddress Then
                                    dead2.Line1 = registrationAddress.Address_Line_1
                                    dead2.Line2 = registrationAddress.Address_Line_2
                                    dead2.Line3 = registrationAddress.Address_Line_3
                                    dead2.City = registrationAddress.Address_Line_4
                                    dead2.Province = registrationAddress.Address_Line_5
                                    dead2.Country = registrationAddress.Country
                                    dead2.PostalCode = registrationAddress.Post_Code
                                Else
                                    dead2.Line1 = dead1.Line1
                                    dead2.Line2 = dead1.Line2
                                    dead2.Line3 = dead1.Line3
                                    dead2.City = dead1.City
                                    dead2.Province = dead1.Province
                                    dead2.Country = dead1.Country
                                    dead2.PostalCode = dead1.PostalCode
                                End If

                                deoh.OrderCustomerName = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Full_Name
                                dead2.ContactName = dead1.ContactName
                                deoh.HomeDelivery = False
                                ' Check for Home Delivery - if set then concatenate home delivery options to contact name 
                                If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(CType(HttpContext.Current.Profile, TalentProfile)) Then
                                    deoh.HomeDelivery = True
                                    Dim strHomeDel As String = "HD"
                                    Dim deliveryOptions As String = dt.Rows(0)("SPECIAL_INSTRUCTIONS_1").ToString
                                    Dim deliveryOptionsCode As String = String.Empty
                                    If deliveryOptions <> String.Empty Then
                                        ' Installation
                                        If deliveryOptions.Substring(0, 1) = "Y" Then
                                            deliveryOptionsCode = deliveryOptionsCode + "I"
                                        End If
                                        ' Collect old 
                                        If deliveryOptions.Substring(1, 1) = "Y" Then
                                            deliveryOptionsCode = deliveryOptionsCode + "X"
                                        End If
                                    End If
                                    dead1.ContactName = strHomeDel + deliveryOptionsCode + " " + dead2.ContactName
                                    dead2.ContactName = dead1.ContactName
                                End If
                                dead2.Title = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Title
                                dead2.Forename = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Forename
                                dead2.Surname = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Surname
                                dead2.Email = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email
                                dead2.PhoneNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Telephone_Number
                                deoh.TotalOrderItemsValue = dt.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS")
                                deoh.TotalOrderItemsValueNet = dt.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_NET")
                                deoh.TotalOrderItemsValueTax = dt.Rows(0)("TOTAL_ORDER_ITEMS_TAX")
                                deoh.TotalValueCharged = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_AMOUNT_CHARGED"))
                                deoh.PromotionValue = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
                                deoh.TotalOrderValue = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))

                                '------------------------------------
                                ' Set delivery type from order header 
                                '------------------------------------
                                deoh.CarrierCode = Talent.Common.Utilities.CheckForDBNull_String(dt.Rows(0)("DELIVERY_TYPE"))
                                If deoh.CarrierCode = String.Empty Then
                                    ' Default to '01' to cater for existing clients
                                    If _defaults.OrderCarrierCode.Equals(String.Empty) Then
                                        deoh.CarrierCode = "01"
                                    Else
                                        deoh.CarrierCode = _defaults.OrderCarrierCode
                                    End If
                                End If

                                '-
                                ' If carrier code in defaults is a single space then set carrier code to blanks
                                If _defaults.OrderCarrierCode = " " Then
                                    deoh.CarrierCode = String.Empty
                                End If


                                deoh.CarrierCodeValue = dt.Rows(0)("TOTAL_DELIVERY_GROSS")
                                deoh.CarrierCodeVAT = dt.Rows(0)("TOTAL_DELIVERY_TAX")
                                deoh.CarrierCodeNet = dt.Rows(0)("TOTAL_DELIVERY_NET")
                                deoh.CollDEAddress.Add(dead1)
                                deoh.CollDEAddress.Add(dead2)
                                dep.CollDETrans.Add(detr)
                                deos.DEOrderHeader = deoh
                                dep.CollDEOrders.Add(deos)
                                If xmlOrderAttributes.ContainsKey("DIST_CHAN") Then dep.DistChan = xmlOrderAttributes("DIST_CHAN")
                                If xmlOrderAttributes.ContainsKey("PO_METHOD") Then dep.PoMethod = xmlOrderAttributes("PO_METHOD")
                                If xmlOrderAttributes.ContainsKey("SEND_CUSTOMER_EMAIL_ADDRESS_TO_SAP") Then dep.SendCustomerEmailAddressToSAP = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(xmlOrderAttributes("SEND_CUSTOMER_EMAIL_ADDRESS_TO_SAP"))
                                If xmlOrderAttributes.ContainsKey("SEND_CUSTOMER_POSTCODE_TO_SAP") Then dep.SendCustomerPostCodeToSAP = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(xmlOrderAttributes("SEND_CUSTOMER_POSTCODE_TO_SAP"))

                                deoi = New DEOrderInfo
                                If Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("COMMENT")) <> "" Then
                                    '--------------
                                    ' Order Header Comment lines
                                    '--------------
                                    decl = New DeCommentLines
                                    decl.CommentText = dt.Rows(0)("COMMENT")
                                    deoi.CollDECommentLines.Add(decl)
                                End If

                                '-------------------------
                                ' Loop through order items
                                '-------------------------
                                Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-280", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Setting Line Values on Back-end Datatable", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                Do While index < dt2.Rows.Count
                                    depr = New DeProductLines
                                    depr.SKU = dt2.Rows(index)("PRODUCT_CODE").ToString

                                    dep.EnableAlternativeSKU = CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.Enable_Alternate_SKU
                                    If CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.Enable_Alternate_SKU Then
                                        Dim dtItems As Data.DataTable = TDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderID(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                                        If dtItems.Rows.Count > 0 Then
                                            Dim itemIndex As Integer = 0
                                            Do While itemIndex < dtItems.Rows.Count
                                                If (dtItems.Rows(itemIndex)("PRODUCT").Equals(depr.SKU)) Then
                                                    depr.AlternateSKU = dtItems.Rows(itemIndex)("ALTERNATE_SKU")
                                                End If
                                                itemIndex += 1
                                            Loop
                                        End If
                                    End If
                                    depr.CustomerLineNumber = dt2.Rows(index)("LINE_NUMBER").ToString
                                    depr.Quantity = CType(dt2.Rows(index)("QUANTITY"), Double)
                                    depr.FixedPrice = dt2.Rows(index)("PURCHASE_PRICE_GROSS")
                                    depr.FixedPriceNet = dt2.Rows(index)("PURCHASE_PRICE_NET")
                                    depr.FixedPriceTax = dt2.Rows(index)("PURCHASE_PRICE_TAX")
                                    depr.NetLineValue = dt2.Rows(index)("LINE_PRICE_NET")
                                    depr.TaxLineValue = dt2.Rows(index)("LINE_PRICE_TAX")
                                    depr.GrossLineValue = dt2.Rows(index)("LINE_PRICE_GROSS")
                                    depr.ProductTaxValue = dt2.Rows(index)("PURCHASE_PRICE_TAX")
                                    depr.ProductDescription = dt2.Rows(index)("PRODUCT_DESCRIPTION_1").ToString
                                    depr.TaxAmount1 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("TAX_AMOUNT_1"))
                                    depr.TaxAmount2 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("TAX_AMOUNT_2"))
                                    depr.TaxAmount3 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("TAX_AMOUNT_3"))
                                    depr.TaxAmount4 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("TAX_AMOUNT_4"))
                                    depr.TaxAmount5 = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("TAX_AMOUNT_5"))
                                    depr.TaxCode1 = String.Empty
                                    depr.TaxCode2 = String.Empty
                                    depr.TaxCode3 = String.Empty
                                    depr.TaxCode4 = String.Empty
                                    depr.TaxCode5 = String.Empty
                                    depr.Currency = Talent.eCommerce.Utilities.CheckForDBNull_String(dt2.Rows(index)("CURRENCY").ToString)
                                    depr.PromotionValue = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("PROMOTION_VALUE"))
                                    depr.PromotionPercentage = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt2.Rows(index)("PROMOTION_PERCENTAGE"))
                                    depr.CostCentre = Talent.eCommerce.Utilities.CheckForDBNull_String(dt2.Rows(index)("COST_CENTRE"))
                                    depr.AccountCode = Talent.eCommerce.Utilities.CheckForDBNull_String(dt2.Rows(index)("ACCOUNT_CODE"))

                                    deoi.CollDEProductLines.Add(depr)
                                    If Talent.eCommerce.Utilities.CheckForDBNull_String(dt2.Rows(index)("INSTRUCTIONS")) <> "" Then
                                        '--------------
                                        ' Order Detail Comment Lines
                                        '--------------
                                        decl = New DeCommentLines
                                        decl.CommentLineNumber = index + 1
                                        decl.CommentText = dt2.Rows(index)("INSTRUCTIONS")
                                        deoi.CollDECommentLines.Add(decl)
                                    End If
                                    depr.SetlineDelDate = False
                                    Dim calculateLineDel As Boolean = True
                                    Dim deliveryZoneCode As String = String.Empty
                                    If Not HttpContext.Current.Session("DeliveryZoneCode") Is Nothing Then
                                        deliveryZoneCode = HttpContext.Current.Session("DeliveryZoneCode").ToString
                                    End If
                                    Dim deliveryZoneType As String = "1"

                                    deliveryZoneType = Talent.eCommerce.Utilities.GetDeliveryZoneType(deliveryZoneCode)
                                    Dim deliveryZoneDate As Date = Talent.eCommerce.Utilities.GetDeliveryDate(CType(HttpContext.Current.Profile, TalentProfile), deliveryZoneCode, deliveryZoneType)
                                    If calculateLineDel AndAlso Not Talent.eCommerce.Utilities.IsBasketHomeDelivery(CType(HttpContext.Current.Profile, TalentProfile)) Then
                                        depr.lineDelDate = Talent.eCommerce.Utilities.GetDeliveryDateLine(CType(HttpContext.Current.Profile, TalentProfile), depr.SKU, deliveryZoneCode, deliveryZoneType)
                                        depr.SetlineDelDate = True
                                    End If
                                    index += 1
                                Loop

                                deos.DEOrderInfo = deoi
                                '----------------------------------------------
                                ' Update status to just about to create backend
                                '----------------------------------------------
                                Try
                                    orders.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus("PROCESS ORDER"), _
                                                                Now, _
                                                                TalentCache.GetBusinessUnit, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                                    Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                                    status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                        CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                        Talent.Common.Utilities.GetOrderStatus("PROCESS ORDER"), _
                                                        Now.ToString(), _
                                                        "")
                                Catch ex As Exception
                                    Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-001", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                End Try
                                '----------------------
                                ' Create Back End order
                                '----------------------
                                Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-290", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Creating Back-end Order", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                settings.FrontEndConnectionString = _ucr.FrontEndConnectionString

                                With ORDER
                                    .Dep = dep
                                    .Settings = settings
                                    .GetConnectionDetails(TalentCache.GetBusinessUnit)
                                End With

                                If Talent.eCommerce.Utilities.IsSiteInMaintenance() Then
                                    If err Is Nothing Then
                                        err = New ErrorObj
                                    End If
                                    err.HasError = True
                                    err.ErrorMessage = "Site is under maintenance"
                                    err.ErrorNumber = "COC-MAINT"
                                Else
                                    '
                                    ' We will call a webservice to complete the order in the ticketing
                                    ' database
                                    If Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                                        If err Is Nothing Then
                                            err = New ErrorObj
                                            err.HasError = False
                                            Dim rowsAffected As Integer = 0
                                            For iItems = 1 To deoi.CollDEProductLines.Count
                                                depr = deoi.CollDEProductLines.Item(iItems)
                                                With depr
                                                    rowsAffected = TDataObjects.ProductsSettings.TblProductStock.DecrementStockByProductCode(.SKU, .Quantity, False)
                                                End With
                                            Next
                                        End If
                                    Else
                                        err = ORDER.Create()
                                    End If
                                End If

                                '------------------------------
                                ' Check if index 1 has an error
                                '------------------------------
                                If Not err.HasError AndAlso Not String.IsNullOrEmpty(err.ItemErrorCode(1)) Then
                                    Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, err.ErrorNumber, err.ErrorMessage, err.ErrorStatus, TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), "CheckoutOrderConfirmation.aspx")
                                    err.HasError = True
                                    err.ErrorMessage = err.ItemErrorStatus(1)
                                End If

                                '************************************************

                                '*************************************************

                                If err.HasError Then
                                    Try
                                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-300", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Error Creating Back-End Order, Serializing...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                        Dim sendEmail As Boolean = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("SendBackEndFailureEmail"))
                                        Talent.eCommerce.Utilities.SerializeObject(ORDER, _
                                                                                ORDER.GetType, _
                                                                                TalentCache.GetBusinessUnit, _
                                                                                TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
                                                                                CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                                "ORDER", _
                                                                                sendEmail, _
                                                                                HttpContext.Current, _
                                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id)

                                    Catch ex As Exception
                                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-002", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                        err.ErrorMessage = ex.Message
                                    End Try
                                    '----------------------
                                    ' Order status to error
                                    '---------------------- 
                                    Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, err.ErrorNumber, TalentCache.GetBusinessUnit & " - " & err.ErrorMessage, CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.Partner, ProfileHelper.GetPageName)
                                    Try
                                        Talent.Common.Utilities.Email_Send(ConfigurationManager.AppSettings("ErrorFromEmail"), _
                                                                             ConfigurationManager.AppSettings("ErrorToEmail"), _
                                                                             ConfigurationManager.AppSettings("ErrorSubjectField") & " Error writing order to SYSTEM 21!", _
                                                                             String.Format("Business Unit: {1} {0} Partner: {2} {0} Login ID: {3} {0} Error Number: {4} {0} Error Message: {5} {0} Error Status: {6} {0}", _
                                                                                                    vbCrLf, _
                                                                                                    TalentCache.GetBusinessUnit, _
                                                                                                    TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
                                                                                                    CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                                                    err.ErrorNumber, _
                                                                                                    err.ErrorMessage, _
                                                                                                    err.ErrorStatus) _
                                                                            )
                                    Catch ex As Exception
                                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-003", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                    End Try
                                    orders.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus("PROCESS FAILED"), _
                                                                Now, _
                                                                TalentCache.GetBusinessUnit, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                                    Try
                                        Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                                        status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                            CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                            Talent.Common.Utilities.GetOrderStatus("PROCESS FAILED"), _
                                                            Now.ToString(), _
                                                            "")
                                    Catch ex As Exception
                                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-004", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                    End Try
                                Else
                                    If _defaults.OrderDestinationDatabase = "SQL2005" AndAlso Not Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-305", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Website Only Order Complete", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                        Me.TDataObjects.OrderSettings.TblOrderStatus.Insert(TalentCache.GetBusinessUnit, CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id, Talent.Common.Utilities.GetOrderStatus("ORDER PENDING"), "Awaiting Completion")
                                    Else
                                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-310", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Back-end Order Created Successfully", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
                                        Dim orderStatus As String = "ORDER PROCESSED"
                                        If _defaults.SetRetailOrderToPending Then
                                            orderStatus = "ORDER PENDING"
                                        End If

                                        '--------------------------
                                        ' Order status to processed
                                        '--------------------------
                                        Try
                                            orders.Update_Order_Status(Talent.Common.Utilities.GetOrderStatus(orderStatus), _
                                                                                                  Now, _
                                                                                                  TalentCache.GetBusinessUnit, _
                                                                                                  CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                                                  CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)

                                        Catch ex As Exception
                                            Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-005", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                        End Try
                                        Try
                                            Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                                            status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                                Talent.Common.Utilities.GetOrderStatus(orderStatus), _
                                                                Now.ToString(), _
                                                                "")
                                        Catch ex As Exception
                                            Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-006", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                        End Try
                                        '---------------------------
                                        ' Update BackOffice order no    
                                        '---------------------------
                                        Dim backOfficeOrderNo As String = String.Empty
                                        If Not String.IsNullOrWhiteSpace(paymentReference) AndAlso Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                                            backOfficeOrderNo = paymentReference
                                        Else
                                            If ORDER.ResultDataSet IsNot Nothing AndAlso ORDER.ResultDataSet.Tables.Count > 0 Then
                                                If ORDER.ResultDataSet.Tables(0).Rows.Count > 0 Then
                                                    Dim drOrder As Data.DataRow = ORDER.ResultDataSet.Tables(0).Rows(0)
                                                    backOfficeOrderNo = drOrder("OrderNumber")
                                                End If
                                            End If
                                        End If

                                        If Not String.IsNullOrWhiteSpace(backOfficeOrderNo) Then
                                            If Not String.IsNullOrWhiteSpace(backOfficeOrderNo) Then
                                                _merchandiseBackOfficeOrdRef = backOfficeOrderNo
                                            End If
                                            For i As Integer = 1 To 3
                                                Try
                                                    orders.Update_Backend_Order_ID(backOfficeOrderNo, _
                                                                                      Now, _
                                                                                      TalentCache.GetBusinessUnit, _
                                                                                      CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                                      CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                                                    Exit For
                                                Catch ex As Exception
                                                    If i = 3 Then
                                                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "UCCCPL-010", ex.Message, "Error writing backend order No. to SQL DB", "", "", ProfileHelper.GetPageName)
                                                        Talent.Common.Utilities.Email_Send(ConfigurationManager.AppSettings("ErrorFromEmail"), _
                                                                                                                            ConfigurationManager.AppSettings("ErrorToEmail"), _
                                                                                                                            ConfigurationManager.AppSettings("ErrorSubjectField") & " Error updating backend order ID on SQL Server", _
                                                                                                                            String.Format("Business Unit: {1} {0} Partner: {2} {0} Login ID: {3} {0} Error Number: {4} {0} Error Message: {5} {0} Error Status: {6} {0}", _
                                                                                                                                                   vbCrLf, _
                                                                                                                                                   TalentCache.GetBusinessUnit, _
                                                                                                                                                   TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
                                                                                                                                                   CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                                                                                                   "UCCCPL-010", _
                                                                                                                                                   ex.Message, _
                                                                                                                                                   "Error writing backend order No. to SQL DB"))
                                                    Else
                                                        System.Threading.Thread.Sleep(50)
                                                    End If
                                                End Try
                                            Next
                                        End If
                                        Try
                                            '------------------
                                            'Check credit limit
                                            '------------------
                                            If _defaults.PerformCreditCheck Then
                                                Dim deCred As New Talent.Common.DECreditCheck(CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1)
                                                Dim dbCred As New Talent.Common.DBCreditCheck
                                                Dim credCheck As New Talent.Common.TalentCreditCheck
                                                Dim credSettings As New Talent.Common.DESettings

                                                settings.AccountNo1 = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1
                                                settings.AccountNo2 = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_2
                                                deCred.TotalOrderValue = deoh.TotalOrderValue
                                                settings.BusinessUnit = TalentCache.GetBusinessUnit
                                                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                                                settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                                                credCheck.Settings = settings
                                                credCheck.CreditCheck = deCred
                                                Dim credErr As Talent.Common.ErrorObj = credCheck.PerformCreditLimitCheck()

                                                If Not credErr.HasError Then
                                                    Try
                                                        Dim credDt As Data.DataTable = credCheck.ResultDataSet.Tables("CreditCheckHeader")
                                                        If credDt.Rows.Count > 0 Then
                                                            Dim credStatus As String = credDt.Rows(0)("CreditStatus")
                                                            Select Case credStatus
                                                                Case Is = "1"
                                                                    'Credit will not be exceeded by new order
                                                                Case Is = "2"
                                                                    'Order will cause credit limit to be exceeded
                                                                    If _defaults.SendEmail_IfOrderCreditLimitExceeded Then
                                                                        SendCreditExceededEmail(backOfficeOrderNo, _
                                                                                                deoh.WebOrderNumber, _
                                                                                                deoh.TotalOrderItemsValue, _
                                                                                                _defaults)
                                                                    End If
                                                                Case Is = "3"
                                                                    'Credit limit has already been exceeded
                                                                    If _defaults.SendEmail_IfOrderCreditLimitExceeded Then
                                                                        SendCreditExceededEmail(backOfficeOrderNo, _
                                                                                                deoh.WebOrderNumber, _
                                                                                                deoh.TotalOrderItemsValue, _
                                                                                                _defaults)
                                                                    End If
                                                            End Select
                                                        End If
                                                    Catch ex As Exception
                                                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-008", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                                    End Try
                                                End If

                                            End If
                                        Catch ex As Exception
                                            Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-009", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                                        End Try

                                    End If
                                End If
                            End If

                        End If
                    Else
                        Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "COC-330", CType(HttpContext.Current.Profile, TalentProfile).UserName & " - Failed to retrieve front end order", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))


                    End If
                    '-----------------------------------------------------------------------------
                    '   create Confirmation email
                    '
                    ' We now have a combined confirmation email when retail and ticketing are together
                    If Not Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                        If _defaults.ConfirmationEmail OrElse CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.ORDER_CONFIRMATION_EMAIL Then
                            Dim Order_Email As New Order_Email
                            Order_Email.SendConfirmationEmail(_defaults.OrdersFromEmail)
                        End If
                    End If


                Catch ex As Exception
                    Dim sendEmail As Boolean = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("SendBackEndFailureEmail"))
                    Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-010", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                    Talent.eCommerce.Utilities.SerializeObject(ORDER, _
                                                                ORDER.GetType, _
                                                                TalentCache.GetBusinessUnit, _
                                                                TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), _
                                                                CType(HttpContext.Current.Profile, TalentProfile).UserName, _
                                                                "ORDER", _
                                                                sendEmail, _
                                                                HttpContext.Current, _
                                                                CType(HttpContext.Current.Profile, TalentProfile).Basket.Temp_Order_Id)

                Finally
                    Try

                        'Update the promotions_users table to increase the promotion usage counts
                        '------------------------------------------------------------------------
                        Dim promoRedeemed As New Data.DataTable
                        promoRedeemed = Talent.eCommerce.Utilities.GetPromotions_Redeemed(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, CType(HttpContext.Current.Profile, TalentProfile).UserName)
                        Dim promoUsers As New Data.DataTable
                        If promoRedeemed.Rows.Count > 0 Then
                            For Each promo As Data.DataRow In promoRedeemed.Rows

                                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(promo("SUCCESS")) Then
                                    promoUsers = Talent.eCommerce.Utilities.GetPromotions_Users(Talent.eCommerce.Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_String(promo("LOGINID")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(promo("ACTIVE_FROM_DATE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(promo("ACTIVE_TO_DATE")))
                                    If promoUsers.Rows.Count > 0 Then
                                        Talent.eCommerce.Utilities.UpdatePromotions_Users(Talent.eCommerce.Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_String(promo("LOGINID")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(promo("ACTIVE_FROM_DATE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(promo("ACTIVE_TO_DATE")))
                                    Else
                                        Talent.eCommerce.Utilities.InsertPromotions_Users(Talent.eCommerce.Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_String(promo("LOGINID")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(promo("ACTIVE_FROM_DATE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(promo("ACTIVE_TO_DATE")), _
                                                                                                Talent.eCommerce.Utilities.CheckForDBNull_Int(promo("APPLICATION_COUNT")))
                                    End If
                                    Talent.eCommerce.Utilities.UpdatePromotions_RedeemCount(Talent.eCommerce.Utilities.CheckForDBNull_String(promo("PROMOTION_CODE")))
                                End If
                            Next
                        End If
                        '------------------------------------------------------------------------
                    Catch ex As Exception
                        Logging.WriteLog(HttpContext.Current.Profile.UserName, "MER-011", ex.StackTrace, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
                    End Try
                End Try
            End If
        End Sub

        Protected Sub ProcessGiftMessage(ByVal tempOrderID As String, _
                                        ByVal webOrderID As String, _
                                        ByVal def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues)
            Dim isGiftMessage As Boolean = False
            Try
                If TestForGiftMessage(tempOrderID) Then
                    isGiftMessage = True
                    Dim giftMsgUCR As New Talent.Common.UserControlResource
                    With giftMsgUCR
                        .BusinessUnit = TalentCache.GetBusinessUnit
                        .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                        .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .KeyCode = "GiftMessage.ascx"
                    End With
                    Try
                        Dim htmlMsg As New HTMLGiftMessage(giftMsgUCR.Content("HTMLTitleText", _languageCode, True), _
                                                            giftMsgUCR.Content("HTMLNameIntroText", _languageCode, True), _
                                                            giftMsgUCR.Content("HTMLNameOutroText", _languageCode, True), _
                                                            giftMsgUCR.Content("HTMLFontSize", _languageCode, True), _
                                                            giftMsgUCR.Content("HTMLFont", _languageCode, True))

                        htmlMsg.writeHTMLMessage(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                    Catch ex As Exception
                    End Try
                    Try
                        Dim GiftEmail As New Talent.eCommerce.GiftMessageEmail(def.GiftMessageEmail_From, _
                                                                                def.GiftMessageEmail_To, _
                                                                                giftMsgUCR.Content("WarehouseEmailSubjectText", _languageCode, True), _
                                                                                giftMsgUCR.Content("WarehouseEmailBodyText", _languageCode, True), _
                                                                                webOrderID, _
                                                                                tempOrderID)
                        GiftEmail.SendMail()
                    Catch ex As Exception
                    End Try

                End If
            Catch ex As Exception

            End Try
            Try
                Dim ordersTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                ordersTA.UpdateGiftMessageStatus(isGiftMessage, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, TalentCache.GetBusinessUnit, CType(HttpContext.Current.Profile, TalentProfile).UserName)
            Catch ex As Exception
            End Try
        End Sub

        Protected Function TestForGiftMessage(ByVal tempOrderId As String) As Boolean
            Dim exists As Boolean = False
            Dim SQLString As String = " SELECT * " & _
                      " FROM tbl_gift_message WITH (NOLOCK)  " & _
                      " WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                      " AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                      " AND PARTNER = @PARTNER "

            Dim cmd As New Data.SqlClient.SqlCommand(SQLString, New Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString))
            Dim dr As Data.SqlClient.SqlDataReader = Nothing

            '--------------------
            ' Add the parameters
            '--------------------
            With cmd.Parameters
                .Clear()
                .Add("TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = tempOrderId
                .Add("BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
            End With

            Try
                ' ------------------------------------------------------------
                ' Open connection to DB
                ' ------------------------------------------------------------
                cmd.Connection.Open()
                dr = cmd.ExecuteReader()

                If dr.HasRows Then
                    exists = True
                End If
                dr.Close()
            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try

            Return exists
        End Function

        Protected Function SendCreditExceededEmail(ByVal backOfficeOrderNo As String, _
                                                ByVal WebOrderNumber As String, _
                                                ByVal TotalOrderItemsValue As Decimal, _
                                                ByVal def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues) As Boolean

            Try
                Dim subject, body As String
                subject = _ucr.Content("CreditLimitExceededEmailSubject", _languageCode, True)
                body = _ucr.Content("CreditLimitExceededBodyText", _languageCode, True)

                body = body.Replace("<<LineBreak>>", vbCrLf)
                body = body.Replace("<<Username>>", HttpContext.Current.Profile.UserName)
                body = body.Replace("<<UserNumber>>", CType(HttpContext.Current.Profile, TalentProfile).User.Details.User_Number.ToString)
                body = body.Replace("<<OrderDate>>", Now.ToString)
                body = body.Replace("<<OrderValue>>", TotalOrderItemsValue.ToString)
                body = body.Replace("<<WebOrderID>>", WebOrderNumber.ToString)
                body = body.Replace("<<BackOfficeOrderNo>>", backOfficeOrderNo.ToString)
                body = body.Replace("<<AccountNumber>>", CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1.ToString)

                Talent.Common.Utilities.Email_Send(def.EmailFromAddress_OrderCreditLimitExceeded, _
                                                    def.EmailToAddress_OrderCreditLimitExceeded, _
                                                    subject, _
                                                    body)
            Catch ex As Exception
                Return False
            End Try
            Return True
        End Function

        Private Function FinaliseOrder(ByVal isInvoicePayType As Boolean, ByVal generatedWebOrderNo As String) As Boolean
            Try
                Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                Dim orderDets As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

                Dim webOrderID As String = String.Empty
                If generatedWebOrderNo <> String.Empty Then
                    webOrderID = generatedWebOrderNo
                Else
                    webOrderID = Checkout.GenNewOrderID()
                End If
                For i As Integer = 1 To 3
                    Try
                        orders.Complete_Order(webOrderID, Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), Now, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                        orderDets.UpdateHeaderOrderId(webOrderID, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                        If i > 1 Then Logging.WriteLog(HttpContext.Current.Profile.UserName, _
                                                        "UCCPFO-030", _
                                                        String.Format("Finalise order failed initially but was completed at attempt {0}", i.ToString), _
                                                        String.Format("Order successfully finalised at attempt {0} - TempOrderID: {1} - WebOrderID: {2}", i.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, webOrderID), _
                                                        TalentCache.GetBusinessUnit, _
                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                        ProfileHelper.GetPageName, _
                                                        "OrderSummary.ascx")
                        Exit For
                    Catch ex As Exception
                        If i = 3 Then
                            Logging.WriteLog(HttpContext.Current.Profile.UserName, "UCCPFO-020", ex.Message, String.Format("Error finalising order, attempt {0} - TempOrderID: {1}", i.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                        Else
                            System.Threading.Thread.Sleep(50)
                        End If
                    End Try
                Next
                Try
                    If isInvoicePayType Then
                        orders.SetPaymentType("INV", CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                        Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                        Dim total As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
                        Dim promo As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
                        total = total - promo
                        orders.Set_Total_Amount_Charged(Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), _
                                                        Now, _
                                                        Talent.eCommerce.Utilities.RoundToValue(total, 0.01, False), _
                                                        CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                        TalentCache.GetBusinessUnit, _
                                                        HttpContext.Current.Profile.UserName)
                    Else
                        orders.SetPaymentType("Credit Card", CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
                    End If
                Catch ex As Exception
                    Logging.WriteLog(HttpContext.Current.Profile.UserName, "COC-111", ex.Message, "Error setting payment type as cc or inv - TempOrderID: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "")
                End Try

                UpdateTempOrderIDOnBasket()

            Catch ex As Exception
                Logging.WriteLog(HttpContext.Current.Profile.UserName, "UCCPFO-010", ex.Message, "Error marking order header as finalised - TempOrderID: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                Return False
            End Try
            Return True
        End Function

        Private Sub UpdateTempOrderIDOnBasket()
            Try
                Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
                For i As Integer = 1 To 3
                    Try
                        basket.Update_Temp_order_id(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                        Exit For
                    Catch ex As Exception
                        If i = 3 Then
                            Logging.WriteLog(CType(HttpContext.Current.Profile, TalentProfile).UserName, "UCCPUB-010", ex.Message, String.Format("Error finalising basket, attempt {0} - TempOrderID: {1}", i.ToString, CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)), ProfileHelper.GetPageName, "OrderSummary.ascx")
                        Else
                            System.Threading.Thread.Sleep(50)
                        End If
                    End Try
                Next
            Catch ex As Exception
            End Try
        End Sub

        Private Sub Insert_Order_Status_Flow(ByVal StatusName As String, Optional ByVal comment As String = "")
            Try
                Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
                status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                    CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID, _
                                                    Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                    Now, _
                                                    comment)
            Catch ex As Exception
            End Try
        End Sub
    End Class

End Namespace

