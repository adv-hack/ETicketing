Imports System.Text
Imports System.Xml
Imports Atp.Net
Imports Talent.Common.Utilities

<Serializable()>
Public Class TalentEmail


    Inherits TalentBase
    Private Const ValidateEmailAddressError = "ValidateEmailAddress"        ' Do not alter this without also changing TalentMonitor !

    Private ucr As New UserControlResource
    Private strField(100) As String
    Private strData(100) As String
    Private strCode(10) As String

    Private _emailSettings As New DEEmailSettings
    Private _ucrDictionary As Generic.Dictionary(Of String, String) = Nothing
    Private _ticketPath As String = String.Empty
    Private _retryFailure As Boolean = False
    Private _ucrPackageHeaders() As String = Nothing
    Private _ucrPackageItems() As String = Nothing
    Private _totalColumns As Integer = 0
    Private _totalColumnData As Integer = 0
    Private _resultDataSet As DataSet

    Private _emailFromAddress As String = String.Empty
    Private _emailSubject As String = String.Empty
    Private _emailBody As String = String.Empty
    Private _emailHTML As Boolean = False
    Private _emailTemplateID As Integer = 0
    Private _emailNoDataRetrieval As Boolean = False
    Private _dtBulkTickets As DataTable



    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Public Property EmailSettings() As DEEmailSettings
        Get
            Return _emailSettings
        End Get
        Set(ByVal value As DEEmailSettings)
            _emailSettings = value
        End Set
    End Property

    Public Property TicketPath() As String
        Get
            Return _ticketPath
        End Get
        Set(ByVal value As String)
            _ticketPath = value
        End Set
    End Property

    Public Property RetryFailure() As Boolean
        Get
            Return _retryFailure
        End Get
        Set(ByVal value As Boolean)
            _retryFailure = value
        End Set
    End Property

    Public Function SendFailedEmailsReport() As ErrorObj
        Dim err As New ErrorObj

        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = ucr.Content("SendFailedEmailsReportSubject", Settings.Language, False)
            Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_EMAIL_AS_HTML"))
            Dim emailText As String
            Dim MailFailureTemplate As String

            If HtmlFormat Then
                emailText = ucr.Content("SendFailedEmailsReportBodyHTML", Settings.Language, False)
                MailFailureTemplate = ucr.Content("SendFailedEmailsReportTableHTML", Settings.Language, False)
            Else
                emailText = ucr.Content("SendFailedEmailsReportBody", Settings.Language, False)
                MailFailureTemplate = ucr.Content("SendFailedEmailsReportTable", Settings.Language, False)
            End If

            '
            ' Get the details of the failed emails from all the front end databases
            '
            Dim FailedEmailIDs(EmailSettings.FailedEmails.FEConnectionStrings.Count) As String
            Dim i As Integer = 0
            Dim FailedEmailDetails As New List(Of DEFailedEmailDetails)

            For Each FEConnString As String In EmailSettings.FailedEmails.FEConnectionStrings
                '
                ' Get the failed emails from this database
                '
                Dim FailedEmailID As New StringBuilder("")
                ResultDataSet = Nothing
                err = FetchFailedEmails(FEConnString)

                If Not err.HasError AndAlso Not ResultDataSet Is Nothing AndAlso ResultDataSet.Tables.Count > 0 Then
                    '
                    ' Populate our FailedEmailDetails class and add it to our list
                    '
                    For Each dr As DataRow In ResultDataSet.Tables("FailedEmails").Rows
                        Dim fed As New DEFailedEmailDetails()
                        FailedEmailID.Append(dr.Item("ID") & ", ")
                        fed.CustomerNumber = dr.Item("CustomerNumber")
                        fed.EmailAddress = dr.Item("EmailAddress")
                        fed.ReasonForFailure = dr.Item("ReasonForFailure")
                        fed.DateSent = dr.Item("DateSent")
                        fed.Title = ""
                        fed.Forename = ""
                        fed.Surname = ""

                        FailedEmailDetails.Add(fed)
                    Next

                Else

                    If Not err.HasError Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-09b"
                        err.ErrorMessage = String.Format("Error Retrieving the failed emails.")
                    End If

                    Exit For

                End If

                If FailedEmailID.Length > 2 Then
                    FailedEmailID.Remove(FailedEmailID.Length - 2, 2)   ' Remove the last ", "
                    FailedEmailIDs(i) = FailedEmailID.ToString()
                End If

                i = i + 1
            Next
            '
            ' Now for each failed email, go to the backend and retrieve the customers details
            '
            If Not err.HasError Then
                For Each fed As DEFailedEmailDetails In FailedEmailDetails
                    Dim DeCust As New DECustomer()
                    DeCust.CustomerNumber = fed.CustomerNumber
                    DeCust.UserName = fed.CustomerNumber

                    Dim deCustV11 As New DECustomerV11()
                    deCustV11.DECustomersV1.Add(DeCust)

                    Dim tc As New TalentCustomer()
                    With tc
                        .DeV11 = deCustV11
                        .Settings = Settings
                        err = .CustomerRetrieval()
                    End With

                    If err.HasError OrElse tc.ResultDataSet Is Nothing OrElse tc.ResultDataSet.Tables(0).Rows.Count = 0 Then

                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-09a"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                          fed.CustomerNumber)

                    ElseIf tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then

                        fed.Title = ""
                        fed.Forename = "Unknown"
                        fed.Surname = "User"

                    Else

                        fed.Title = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim()
                        fed.Forename = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim()
                        fed.Surname = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim()

                    End If

                Next
            End If

            '
            ' if we have any failures, build the email
            '
            If Not err.HasError And FailedEmailDetails.Count > 0 Then

                Dim FailureDetails As New StringBuilder("")

                For Each fed As DEFailedEmailDetails In FailedEmailDetails
                    Dim FailureDetail As String = MailFailureTemplate

                    FailureDetail = Replace(FailureDetail, "<<Title>>", fed.Title)
                    FailureDetail = Replace(FailureDetail, "<<Forename>>", fed.Forename)
                    FailureDetail = Replace(FailureDetail, "<<Surname>>", fed.Surname)
                    FailureDetail = Replace(FailureDetail, "<<CustomerNumber>>", fed.CustomerNumber)
                    FailureDetail = Replace(FailureDetail, "<<EmailAddress>>", fed.EmailAddress)
                    FailureDetail = Replace(FailureDetail, "<<ReasonForFailure>>", fed.ReasonForFailure)
                    FailureDetail = Replace(FailureDetail, "<<DateSent>>", fed.DateSent)

                    FailureDetails.Append(FailureDetail)

                Next

                emailText = emailText.Replace("<<FailureDetails>>", FailureDetails.ToString())

                If HtmlFormat Then
                    emailText = Replace(emailText, "<<NewLine>>", "<br>")
                Else
                    emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                End If

                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         emailText, "", False, HtmlFormat)

                i = 0
                ResultDataSet = Nothing
                If Not err.HasError Then
                    '
                    ' Update the status of the records to say that we have reported on them
                    '
                    For Each FEConnString As String In EmailSettings.FailedEmails.FEConnectionStrings
                        SetFailedEmailsReported("SetFailedEmailsReportedOK", FEConnString, FailedEmailIDs(i))
                        i = i + 1
                    Next

                Else
                    '
                    ' Update the status of the records to say that we have failed to reported on them
                    '
                    For Each FEConnString As String In EmailSettings.FailedEmails.FEConnectionStrings
                        SetFailedEmailsReported("SetFailedEmailsReportedFailed", FEConnString, FailedEmailIDs(i))
                        i = i + 1
                    Next

                End If

            End If

        End If

        Return err

    End Function

    Public Function CreatePPSPaymentXmlDocument(ByVal fromAddress As String,
                                                ByVal toAddress As String,
                                                ByVal smtpServer As String,
                                                ByVal smtpServerPort As String,
                                                ByVal Partner As String,
                                                ByVal Customer As String,
                                                ByVal Description As String,
                                                ByVal Turnstiles As String,
                                                ByVal Gates As String,
                                                ByVal Seat As String,
                                                ByVal PaymentValue As String,
                                                Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode
        Dim ndParameters, ndCustomer, ndDescription, ndTurnstiles, ndGates, ndSeat, ndTemplateID, ndPaymentValue As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")
                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndDescription = .CreateElement("Description")
                ndTurnstiles = .CreateElement("Turnstiles")
                ndGates = .CreateElement("Gates")
                ndSeat = .CreateElement("Seat")
                ndPaymentValue = .CreateElement("PaymentValue")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID
            ndCustomer.InnerText = Customer
            ndDescription.InnerText = Description
            ndTurnstiles.InnerText = Turnstiles
            ndGates.InnerText = Gates
            ndSeat.InnerText = Seat
            ndPaymentValue.InnerText = PaymentValue

            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndDescription)
                .AppendChild(ndTurnstiles)
                .AppendChild(ndGates)
                .AppendChild(ndSeat)
                .AppendChild(ndPaymentValue)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)


        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function

    Public Function SendPPSPaymentFailureEmail() As ErrorObj

        ' True
        Dim err As New ErrorObj

        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_PPS_PAYMENT_FAILURE, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            Dim HtmlFormat As Boolean = _emailHTML
            Dim emailText As String = _emailBody

            'Dim emailSubject As String = ucr.Content("PPSPaymentFailureEmailSubject", Settings.Language, False)
            'Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_EMAIL_AS_HTML"))
            'If HtmlFormat Then
            '    emailText = ucr.Content("PPSPaymentFailureEmailBodyHTML", Settings.Language, False)
            'Else
            '    emailText = ucr.Content("PPSPaymentFailureEmailBody", Settings.Language, False)
            'End If

            If Not _emailNoDataRetrieval And Not err.HasError Then

                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.PPSPayment.Customer
                DeCust.UserName = EmailSettings.PPSPayment.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With


                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                    Else

                        emailText = ReplaceTableRowContentFields(emailText, EmailSettings.PPSPayment.Description, Nothing, Nothing,
                            tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim(),
                            tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim(),
                            tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim(),
                            tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim().TrimStart("0"),
                            Nothing,
                            EmailSettings.PPSPayment.PaymentValue,
                            Nothing,
                            Nothing,
                            EmailSettings.PPSPayment.Gates.Replace("/", ""),
                            EmailSettings.PPSPayment.Turnstiles.Replace("/", ""), getPartSeatInfo("Stand", EmailSettings.PPSPayment.Seat, False),
                            getPartSeatInfo("Area", EmailSettings.PPSPayment.Seat, False), getPartSeatInfo("Row", EmailSettings.PPSPayment.Seat, False),
                            getPartSeatInfo("SeatNo", EmailSettings.PPSPayment.Seat, False), getPartSeatInfo("Alpha", EmailSettings.PPSPayment.Seat, False),
                            Nothing, Nothing, Nothing, String.Empty, False, False, False, 0, 0, 0, 0, 0, 0, 0, 0)

                        If HtmlFormat Then
                            emailText = Replace(emailText, "<<NewLine>>", "<br>")
                        Else
                            emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                        End If

                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                 emailText, "", False, HtmlFormat)

                    End If

                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-07a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.PPSPayment.Customer)
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If
        End If

        Return err

    End Function

    Public Function SendPPSPaymentConfirmationEmail() As ErrorObj

        ' True
        Dim err As New ErrorObj
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_PPS_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_PPS_PAYMENT_CONFIRMATION, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            Dim HtmlFormat As Boolean = _emailHTML
            Dim emailText As String = _emailBody

            'Dim emailSubject As String = ucr.Content("PPSPaymentConfirmationEmailSubject", Settings.Language, False)
            'Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_EMAIL_AS_HTML"))
            'If HtmlFormat Then
            '    emailText = ucr.Content("PPSPaymentConfirmationEmailBodyHTML", Settings.Language, False)
            'Else
            '    emailText = ucr.Content("PPSPaymentConfirmationEmailBody", Settings.Language, False)
            'End If

            If Not _emailNoDataRetrieval Then
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.PPSPayment.Customer
                DeCust.UserName = EmailSettings.PPSPayment.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-06"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.PPSPayment.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    Else

                        emailText = ReplaceTableRowContentFields(emailText, EmailSettings.PPSPayment.Description, Nothing, Nothing,
                                            tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim(),
                                            tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim(),
                                            tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim(),
                                            tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim().TrimStart("0"),
                                            Nothing,
                                            EmailSettings.PPSPayment.PaymentValue,
                                            Nothing,
                                            Nothing,
                                            EmailSettings.PPSPayment.Gates.Replace("/", ""),
                                            EmailSettings.PPSPayment.Turnstiles.Replace("/", ""), getPartSeatInfo("Stand", EmailSettings.PPSPayment.Seat, False),
                                            getPartSeatInfo("Area", EmailSettings.PPSPayment.Seat, False), getPartSeatInfo("Row", EmailSettings.PPSPayment.Seat, False),
                                            getPartSeatInfo("SeatNo", EmailSettings.PPSPayment.Seat, False), getPartSeatInfo("Alpha", EmailSettings.PPSPayment.Seat, False),
                                            Nothing, Nothing, Nothing, String.Empty, False, False, False, 0, 0, 0, 0, 0, 0, 0, 0)

                        If HtmlFormat Then
                            emailText = Replace(emailText, "<<NewLine>>", "<br>")
                        Else
                            emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                        End If

                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                 emailText, "", False, HtmlFormat)

                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-06a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.PPSPayment.Customer)
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If
        End If

        Return err

    End Function

    Public Function SendAmendPPSPaymentConfirmationEmail() As ErrorObj

        ' True
        Dim err As New ErrorObj
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_AMMEND_PPS_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_PPS_AMEND_PAYMENT, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            Dim HtmlFormat As Boolean = _emailHTML
            Dim emailText As String = _emailBody

            '            Dim emailSubject As String = ucr.Content("AmendPPSPaymentConfirmationEmailSubject", Settings.Language, False)
            '            Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_EMAIL_AS_HTML"))
            'If HtmlFormat Then
            '    emailText = ucr.Content("AmendPPSPaymentConfirmationEmailBodyHTML", Settings.Language, False)
            'Else
            '    emailText = ucr.Content("AmendPPSPaymentConfirmationEmailBody", Settings.Language, False)
            'End If


            If Not _emailNoDataRetrieval Then

                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.PPSPayment.Customer
                DeCust.UserName = EmailSettings.PPSPayment.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-13"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.PPSPayment.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    Else
                        Dim customerTitle As String = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim()
                        Dim customerForename As String = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim()
                        Dim customerSurname As String = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim()
                        emailText = emailText.Replace("<<Title>>", customerTitle)
                        emailText = emailText.Replace("<<Forename>>", customerForename)
                        emailText = emailText.Replace("<<Surname>>", customerSurname)
                        emailText = emailText.Replace("<<CustomerNumber>>", EmailSettings.PPSPayment.Customer)

                        If HtmlFormat Then
                            emailText = Replace(emailText, "<<NewLine>>", "<br>")
                        Else
                            emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                        End If

                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                 emailText, "", False, HtmlFormat)
                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-13a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.PPSPayment.Customer)
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), "", False, HtmlFormat)

            End If

        End If
        Return err
    End Function

    Public Function CreateOrderReturnConfirmationXmlDocument(ByVal fromAddress As String,
                                                             ByVal toAddress As String,
                                                             ByVal smtpServer As String,
                                                             ByVal smtpServerPort As String,
                                                             ByVal Partner As String,
                                                             ByVal Customer As String,
                                                             ByVal OrderReturnReference As String,
                                                             ByVal mode As String,
                                                             ByVal OriginatingSourceCode As String,
                                                             Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode

        Dim ndParameters, ndCustomer, ndOrderReturnReference, ndMode, ndOriginatingSourceCode, ndTemplateID As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")

                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndOrderReturnReference = .CreateElement("OrderReturnReference")
                ndMode = .CreateElement("Mode")
                ndOriginatingSourceCode = .CreateElement("OriginatingSourceCode")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID

            ndCustomer.InnerText = Customer
            ndOrderReturnReference.InnerText = OrderReturnReference
            ndMode.InnerText = mode
            ndOriginatingSourceCode.InnerText = OriginatingSourceCode

            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndOrderReturnReference)
                .AppendChild(ndMode)
                .AppendChild(ndOriginatingSourceCode)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)

        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function
    Public Function CreateTicketExchangeConfirmationXmlDocument(ByVal fromAddress As String,
                                                             ByVal toAddress As String,
                                                             ByVal smtpServer As String,
                                                             ByVal smtpServerPort As String,
                                                             ByVal Partner As String,
                                                             ByVal Customer As String,
                                                             ByVal TicketExchangeReference As String,
                                                             ByVal OriginatingSourceCode As String,
                                                             Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode

        Dim ndParameters, ndCustomer, ndTicketExchangeReference, ndMode, ndOriginatingSourceCode, ndTemplateID As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")
                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndTicketExchangeReference = .CreateElement("TicketExchangeReference")
                ndOriginatingSourceCode = .CreateElement("OriginatingSourceCode")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID
            ndCustomer.InnerText = Customer
            ndTicketExchangeReference.InnerText = TicketExchangeReference
            ndOriginatingSourceCode.InnerText = OriginatingSourceCode

            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndTicketExchangeReference)
                .AppendChild(ndOriginatingSourceCode)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)

        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function
    Public Function SendOrderReturnConfirmationEmail() As ErrorObj
        Dim err As New ErrorObj
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_RETURN_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            If EmailSettings.OrderReturnConfirmation.Mode = "1" Then
                RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM, err)
            Else
                RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM_REBOOK, err)
            End If
            If err.HasError Then Return err


            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                'If EmailSettings.OrderReturnConfirmation.Mode = "1" Then
                '    .KeyCode = "OrderReturnConfirmEmail.vb"
                'Else
                '    .KeyCode = "OrderReturnConfirmEmailRebook.vb"
                'End If
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            'Dim emailSubject As New StringBuilder(ucr.Content("EmailSubject", Settings.Language, False))
            'Dim emailText As New StringBuilder()
            '            Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML"))
            Dim HtmlFormat As Boolean = _emailHTML
            Dim emailSubject As New StringBuilder(_emailSubject)
            Dim emailText As New StringBuilder()

            Dim OrderReturnTableHeader As String = ucr.Content("OrderReturnTableHeader", Settings.Language, False)
            Dim OrderReturnTableRow As String = ucr.Content("OrderReturnTableRow", Settings.Language, False)
            Dim OrderReturnSeatSeparator As String = ucr.Content("OrderReturnSeatSeparator", Settings.Language, False)

            If Not _emailNoDataRetrieval Then
                '
                ' Get the customer information
                '
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.OrderReturnConfirmation.Customer
                DeCust.UserName = EmailSettings.OrderReturnConfirmation.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-05"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.OrderReturnConfirmation.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))

                    Else

                        Dim SendOrderConfToCustServ As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_CONF_TO_CUST_SERV"))
                        If SendOrderConfToCustServ Then
                            EmailSettings.ToAddress = EmailSettings.ToAddress & ";" &
                                                      TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "CUSTOMER_SERVICES_EMAIL")
                        End If

                        err = OrderHeaderTicketingValues()


                        If HtmlFormat Then
                            strData(0) = "<br />"
                        Else
                            strData(0) = vbCrLf
                        End If
                        strData(1) = "" ' PaymentReference
                        strData(2) = tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim()
                        strData(6) = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim() & " " &
                                  tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim()
                        strData(7) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine1").ToString().Trim()
                        strData(8) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine2").ToString().Trim()
                        strData(9) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine3").ToString().Trim()
                        strData(10) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine4").ToString().Trim()
                        strData(11) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine5").ToString().Trim()
                        strData(12) = tc.ResultDataSet.Tables(1).Rows(0).Item("PostCode").ToString().Trim()
                        strData(13) = "" ' Country
                        strData(14) = tc.ResultDataSet.Tables(1).Rows(0).Item("HomeTelephoneNumber").ToString().Trim()
                        strData(15) = EmailSettings.ToAddress

                        strData(57) = "" ' TotalOrderValue
                        strData(58) = "" ' TotalOrderValue Formatted as currency
                        strData(59) = "" ' TotalOrderValue Formatted as currency

                        '
                        '   Join all the paragraphs together
                        '
                        If HtmlFormat Then
                            For i As Integer = 0 To 10
                                'Leave the <br> tag for better formatting
                                emailText.Append(strCode(i))
                            Next
                        Else
                            For i As Integer = 0 To 10
                                emailText.Append(strCode(i) & vbCrLf)
                            Next
                        End If

                        'Check content is exists in strCode array if not then get it from EmailMessage
                        'Mostly EmailMessage content is used for retail confirmation and 
                        'StrCode array content used for ticketing confirmation
                        ''If emailText.ToString().Trim.Length = 0 Then
                        ''    emailText.Clear()
                        ''    emailText.Append(ucr.Content("EmailMessage", Settings.Language, False))
                        ''End If

                        emailText.Replace("<<email>>", EmailSettings.ToAddress)
                        emailText.Replace("<<Title>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText.Replace("<<Forename>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim())
                        emailText.Replace("<<Surname>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())

                        If EmailSettings.OrderReturnConfirmation.Mode = "1" Then
                            emailText.Replace("<<OrderReturnReference>>", ucr.Content("OrderReturnRefText", Settings.Language, False) & EmailSettings.OrderReturnConfirmation.OrderReturnReference)
                            emailText.Replace("<<OrderReturnDescription>>", ucr.Content("OrderReturnText", Settings.Language, False))
                        Else
                            emailText.Replace("<<OrderReturnReference>>", ucr.Content("OrderReturnRefText", Settings.Language, False) & EmailSettings.OrderReturnConfirmation.OrderReturnReference)
                            emailText.Replace("<<OrderReturnDescription>>", ucr.Content("OrderRebookText", Settings.Language, False))
                        End If

                        '
                        ' Add the order lines
                        '
                        Dim deTicketExchange As New DETicketExchange()
                        deTicketExchange.OriginatingSourceCode = Settings.OriginatingSourceCode
                        deTicketExchange.TicketExchangeReference = EmailSettings.OrderReturnConfirmation.OrderReturnReference

                        Dim tTicketExchange As New TalentTicketExchange()
                        With tTicketExchange
                            .Dep = deTicketExchange
                            .Settings = Settings
                            .Settings.CacheStringExtension = ""
                            err = .TicketExchangeEnquiry()
                        End With

                        If Not err.HasError AndAlso Not tTicketExchange.ResultDataSet Is Nothing AndAlso tTicketExchange.ResultDataSet.Tables(0).Rows.Count > 0 Then

                            If tTicketExchange.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-10"
                                err.ErrorMessage = String.Format("Error Retrieving the order details. Return Reference : {0}, Return Code : {1}",
                                                                  EmailSettings.OrderReturnConfirmation.OrderReturnReference, tTicketExchange.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
                            Else

                                Dim OrderItems As New StringBuilder("")
                                If HtmlFormat Then
                                    OrderItems.Append("<table " & ucr.Attribute("TicketingTableStyle") & ">" & OrderReturnTableHeader)
                                End If

                                For Each row As DataRow In tTicketExchange.ResultDataSet.Tables("TicketExchangeEnquiry").Rows

                                    If Not HtmlFormat Then
                                        OrderItems.Append(row.Item("MemberNumber").ToString().Trim())
                                        OrderItems.Append(ucr.Content("TicketingSeperator1", Settings.Language, False) & row.Item("ProductDescription").ToString().Trim())
                                        OrderItems.Append(ucr.Content("TicketingSeperator2", Settings.Language, False) & getProductDateTime(row.Item("BuyBackDate"), ""))
                                        If row.Item("Unreserved") = True Then
                                            OrderItems.Append(ucr.Content("TicketingSeperator3", Settings.Language, False) & ucr.Content("UnreservedAreaText", Settings.Language, True))
                                        Else

                                            OrderItems.Append(ucr.Content("TicketingSeperator3", Settings.Language, False) & row.Item("Stand").ToString().Trim() & " " &
                                                                                                                        row.Item("Area").ToString().Trim() & " " &
                                                                                                                        row.Item("Row").ToString().Trim() & " " &
                                                                                                                        row.Item("Seat").ToString().Trim() &
                                                                                                                        row.Item("AlphaSuffix").ToString().Trim())
                                        End If
                                        OrderItems.Append(vbCrLf)

                                    Else
                                        If row.Item("Unreserved") = True Then
                                            OrderItems.Append(ReplaceTableRowContentFields(OrderReturnTableRow,
                                                                                   row.Item("ProductDescription").ToString.Trim(),
                                                                                   getProductDate(row.Item("BuyBackDate")),
                                                                                   "",
                                                                                   row.Item("Title").ToString().Trim(),
                                                                                   row.Item("ContactForename").ToString().Trim(),
                                                                                   row.Item("ContactSurname").ToString().Trim(),
                                                                                   row.Item("MemberNumber").ToString().TrimStart("0"),
                                                                                   "",
                                                                                   "",
                                                                                   "",
                                                                                   "",
                                                                                   row.Item("Gate").ToString().Trim(),
                                                                                   row.Item("Turnstile").ToString().Trim(),
                                                                                   "",
                                                                                   "",
                                                                                   "",
                                                                                   "",
                                                                                   ucr.Content("UnreservedAreaText", Settings.Language, True),
                                                                                   row.Item("SeatRestriction").ToString().Trim(),
                                                                                   row.Item("RestText").ToString().Trim(),
                                                                                   String.Empty,
                                                                                   String.Empty,
                                                                                   False,
                                                                                   False,
                                                                                   False,
                                                                                   0, 0, 0, 0, 0, 0, 0, 0))
                                        Else
                                            OrderItems.Append(ReplaceTableRowContentFields(OrderReturnTableRow,
                                                                                   row.Item("ProductDescription").ToString.Trim(),
                                                                                   getProductDate(row.Item("BuyBackDate")),
                                                                                   "",
                                                                                   row.Item("Title").ToString().Trim(),
                                                                                   row.Item("ContactForename").ToString().Trim(),
                                                                                   row.Item("ContactSurname").ToString().Trim(),
                                                                                   row.Item("MemberNumber").ToString().TrimStart("0"),
                                                                                   "",
                                                                                   "",
                                                                                   "",
                                                                                   "",
                                                                                   row.Item("Gate").ToString().Trim(),
                                                                                   row.Item("Turnstile").ToString().Trim(),
                                                                                   row.Item("Stand").ToString().Trim(),
                                                                                   row.Item("Area").ToString().Trim(),
                                                                                   row.Item("Row").ToString().Trim(),
                                                                                   row.Item("Seat").ToString().Trim(),
                                                                                   row.Item("AlphaSuffix").ToString().Trim(),
                                                                                   row.Item("SeatRestriction").ToString().Trim(),
                                                                                   row.Item("RestText").ToString().Trim(),
                                                                                   String.Empty,
                                                                                   String.Empty,
                                                                                   False,
                                                                                   False,
                                                                                   False,
                                                                                   0, 0, 0, 0, 0, 0, 0, 0))
                                        End If
                                    End If

                                Next

                                If HtmlFormat Then
                                    OrderItems.Append("</table>")
                                End If


                                emailText.Replace("<<OrderReturnItems>>", OrderItems.ToString())

                                '
                                ' Replace the field delimiters with data
                                '
                                For i As Integer = 0 To 100
                                    If strField(i) > Nothing Then
                                        emailSubject.Replace(strField(i), strData(i))
                                        emailText.Replace(strField(i), strData(i))
                                    End If
                                Next

                                If HtmlFormat Then
                                    emailText.Replace("<<NewLine>>", "<br />")
                                Else
                                    emailText.Replace("<<NewLine>>", vbCrLf)
                                End If

                                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject.ToString(),
                                                                         emailText.ToString(), "", False, HtmlFormat)
                            End If

                        Else
                            err.HasError = True
                            err.ErrorNumber = "TACTEMAIL-10a"
                            err.ErrorMessage = String.Format("Error Retrieving the order details. Return Reference : {0}",
                                                              EmailSettings.OrderReturnConfirmation.OrderReturnReference)
                        End If

                    End If

                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-05a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.OrderReturnConfirmation.Customer)
                End If
            Else
                emailText.Append(_emailBody)
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject.ToString(),
                                                         RemoveMergeBrackets(emailText.ToString()), "", False, HtmlFormat)
            End If

        End If

        Return err

    End Function

    Public Function SendTicketExchangeConfirmationEmail() As ErrorObj
        Dim err As New ErrorObj
        Dim seatsPutOnSale As Boolean = False
        Dim seatsTakenOffSale As Boolean = False
        Dim seatsRepriced As Boolean = False
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_RETURN_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_TICKET_EXCHANGE_CONFIRM, err)

            If err.HasError Then Return err


            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            'Look for controltextlang with control set to email ID (used for other emails) and if not found for  
            Dim TicketExchangePutOnSaleHeader As String
            TicketExchangePutOnSaleHeader = ucr.Content("TicketExchangePutOnSaleHeader", Settings.Language, False)
            If TicketExchangePutOnSaleHeader = String.Empty Then
                ucr.KeyCode = "TalentEmail.vb.TicketExchangeConfirm"
            End If

            Dim HtmlFormat As Boolean = _emailHTML
            Dim emailSubject As New StringBuilder(_emailSubject)
            Dim emailText As New StringBuilder()

            TicketExchangePutOnSaleHeader = ucr.Content("TicketExchangePutOnSaleHeader", Settings.Language, False)
            Dim TicketExchangePutOnSaleRow As String = ucr.Content("TicketExchangePutOnSaleRow", Settings.Language, False)
            Dim TicketExchangeRebookHeader As String = ucr.Content("TicketExchangeRebookHeader", Settings.Language, False)
            Dim TicketExchangeRebookRow As String = ucr.Content("TicketExchangeRebookRow", Settings.Language, False)
            Dim TicketExchangePriceChangeHeader As String = ucr.Content("TicketExchangePriceChangeHeader", Settings.Language, False)
            Dim TicketExchangePriceChangeRow As String = ucr.Content("TicketExchangePriceChangeRow", Settings.Language, False)
            Dim TicketExchangePutOnSaleHeading As String = ucr.Content("TicketExchangePutOnSaleHeading", Settings.Language, False)
            Dim TicketExchangeRebookHeading As String = ucr.Content("TicketExchangeRebookHeading", Settings.Language, False)
            Dim TicketExchangePriceChangeHeading As String = ucr.Content("TicketExchangePriceChangeHeading", Settings.Language, False)


            If Not _emailNoDataRetrieval Then
                '
                ' Get the customer information
                '
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.TicketExchangeConfirmation.Customer
                DeCust.UserName = EmailSettings.TicketExchangeConfirmation.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-05"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.TicketExchangeConfirmation.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))

                    Else

                        err = OrderHeaderTicketingValues()

                        If HtmlFormat Then
                            strData(0) = "<br />"
                        Else
                            strData(0) = vbCrLf
                        End If
                        strData(1) = "" ' PaymentReference
                        strData(2) = tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim()
                        strData(6) = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim() & " " &
                                  tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim()
                        strData(7) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine1").ToString().Trim()
                        strData(8) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine2").ToString().Trim()
                        strData(9) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine3").ToString().Trim()
                        strData(10) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine4").ToString().Trim()
                        strData(11) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine5").ToString().Trim()
                        strData(12) = tc.ResultDataSet.Tables(1).Rows(0).Item("PostCode").ToString().Trim()
                        strData(13) = "" ' Country
                        strData(14) = tc.ResultDataSet.Tables(1).Rows(0).Item("HomeTelephoneNumber").ToString().Trim()
                        strData(15) = EmailSettings.ToAddress

                        strData(57) = "" ' TotalOrderValue
                        strData(58) = "" ' TotalOrderValue Formatted as currency
                        strData(59) = "" ' TotalOrderValue Formatted as currency

                        '
                        '   Join all the paragraphs together
                        '
                        If HtmlFormat Then
                            For i As Integer = 0 To 10
                                'Leave the <br> tag for better formatting
                                emailText.Append(strCode(i))
                            Next
                        Else
                            For i As Integer = 0 To 10
                                emailText.Append(strCode(i) & vbCrLf)
                            Next
                        End If

                        emailText.Replace("<<email>>", EmailSettings.ToAddress)
                        emailText.Replace("<<Title>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText.Replace("<<Forename>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim())
                        emailText.Replace("<<Surname>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())
                        emailText.Replace("<<TicketExchangeReference>>", ucr.Content("TicketExchangeRefText", Settings.Language, False) & EmailSettings.TicketExchangeConfirmation.TicketExchangeReference)

                        '
                        ' Add the order lines
                        '
                        Dim deTicketExchange As New DETicketExchange()
                        deTicketExchange.OriginatingSourceCode = Settings.OriginatingSourceCode
                        deTicketExchange.TicketExchangeReference = EmailSettings.TicketExchangeConfirmation.TicketExchangeReference

                        Dim tTicketExchange As New TalentTicketExchange()
                        With tTicketExchange
                            .Dep = deTicketExchange
                            .Settings = Settings
                            .Settings.CacheStringExtension = ""
                            err = .TicketExchangeEnquiry()
                        End With

                        If Not err.HasError AndAlso Not tTicketExchange.ResultDataSet Is Nothing AndAlso tTicketExchange.ResultDataSet.Tables(0).Rows.Count > 0 Then

                            If tTicketExchange.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-16"
                                err.ErrorMessage = String.Format("Error Retrieving the order details. Ticket Exchange Reference : {0}, Return Code : {1}",
                                                              EmailSettings.TicketExchangeConfirmation.TicketExchangeReference, tTicketExchange.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
                            Else

                                Dim OrderItems As New StringBuilder("")
                                Dim PutOnSaleItems As New StringBuilder(TicketExchangePutOnSaleHeading)
                                Dim RebookedItems As New StringBuilder(TicketExchangeRebookHeading)
                                Dim PriceChangeItems As New StringBuilder(TicketExchangePriceChangeHeading)


                                If HtmlFormat Then
                                    PutOnSaleItems.Append("<br> <table " & ucr.Attribute("TicketingTableStyle") & ">" & TicketExchangePutOnSaleHeader)
                                    RebookedItems.Append("<br> <table " & ucr.Attribute("TicketingTableStyle") & ">" & TicketExchangeRebookHeader)
                                    PriceChangeItems.Append("<br> <table " & ucr.Attribute("TicketingTableStyle") & ">" & TicketExchangePriceChangeHeader)
                                End If

                                For Each row As DataRow In tTicketExchange.ResultDataSet.Tables("TicketExchangeEnquiry").Rows

                                    Dim ltlStatus As String = String.Empty
                                    If Not HtmlFormat Then
                                        If row.Item("OnSale") = True Then
                                            ltlStatus = ucr.Content("ltlOnSale", Settings.Language, False)
                                        Else
                                            ltlStatus = ucr.Content("ltlRebooked", Settings.Language, False)
                                        End If
                                        OrderItems.Append(row.Item("MemberNumber").ToString().Trim())
                                        OrderItems.Append(ucr.Content("TicketingSeperator1", Settings.Language, False) & row.Item("ProductDescription").ToString().Trim())
                                        OrderItems.Append(ucr.Content("TicketingSeperator2", Settings.Language, False) & getProductDateTime(row.Item("BuyBackDate"), ""))
                                        OrderItems.Append(ucr.Content("TicketingSeperator3", Settings.Language, False) & row.Item("Stand").ToString().Trim() & " " &
                                                                                                                    row.Item("Area").ToString().Trim() & " " &
                                                                                                                    row.Item("Row").ToString().Trim() & " " &
                                                                                                                    row.Item("Seat").ToString().Trim() &
                                                                                                                    row.Item("AlphaSuffix").ToString().Trim() & " " &
                                                                                                                    row.Item("RequestedPrice").ToString().Trim() & " " &
                                                                                                                    row.Item("ClubHandlingFee").ToString().Trim() & " " &
                                                                                                                    row.Item("PreviousRequestedPrice").ToString().Trim() & " " &
                                                                                                                    row.Item("FaceValue").ToString().Trim() & " " &
                                                                                                                    row.Item("OriginalSalePayref").ToString().Trim() & " " &
                                                                                                                    row.Item("Earnings").ToString().Trim() & " " &
                                                                                                                    row.Item("TicketExchangeRef").ToString().Trim() & " " & ltlStatus)

                                        OrderItems.Append(vbCrLf)

                                    Else

                                        If row.Item("OnSale") = True Then
                                            If row.Item("RequestedPrice") <> row.Item("PreviousRequestedPrice") AndAlso row.Item("PreviousRequestedPrice") > 0 Then
                                                seatsRepriced = True
                                                PriceChangeItems.Append(ReplaceTableRowContentFields(TicketExchangePriceChangeRow,
                                                                                           row.Item("ProductDescription").ToString.Trim(),
                                                                                           getProductDate(row.Item("BuyBackDate")),
                                                                                           "",
                                                                                           row.Item("Title").ToString().Trim(),
                                                                                           row.Item("ContactForename").ToString().Trim(),
                                                                                           row.Item("ContactSurname").ToString().Trim(),
                                                                                           row.Item("MemberNumber").ToString().TrimStart("0"),
                                                                                           "",
                                                                                           "",
                                                                                           "",
                                                                                           "",
                                                                                           row.Item("Gate").ToString().Trim(),
                                                                                           row.Item("Turnstile").ToString().Trim(),
                                                                                           row.Item("Stand").ToString().Trim(),
                                                                                           row.Item("Area").ToString().Trim(),
                                                                                           row.Item("Row").ToString().Trim(),
                                                                                           row.Item("Seat").ToString().Trim(),
                                                                                           row.Item("AlphaSuffix").ToString().Trim(),
                                                                                           row.Item("SeatRestriction").ToString().Trim(),
                                                                                           row.Item("RestText").ToString().Trim(),
                                                                                           String.Empty,
                                                                                           String.Empty,
                                                                                           False,
                                                                                           False,
                                                                                           False,
                                                                                           0,
                                                                                           row.Item("RequestedPrice").ToString().Trim(),
                                                                                           row.Item("ClubHandlingFee").ToString().Trim(),
                                                                                           row.Item("PreviousRequestedPrice").ToString().Trim(),
                                                                                           row.Item("FaceValue").ToString().Trim(),
                                                                                           row.Item("OriginalSalePayref").ToString().Trim(),
                                                                                           row.Item("Earnings").ToString().Trim(),
                                                                                           row.Item("TicketExchangeRef").ToString().Trim()
                                                                                           ))
                                            Else
                                                seatsPutOnSale = True
                                                PutOnSaleItems.Append(ReplaceTableRowContentFields(TicketExchangePutOnSaleRow,
                                                                                           row.Item("ProductDescription").ToString.Trim(),
                                                                                           getProductDate(row.Item("BuyBackDate")),
                                                                                           "",
                                                                                           row.Item("Title").ToString().Trim(),
                                                                                           row.Item("ContactForename").ToString().Trim(),
                                                                                           row.Item("ContactSurname").ToString().Trim(),
                                                                                           row.Item("MemberNumber").ToString().TrimStart("0"),
                                                                                           "",
                                                                                           "",
                                                                                           "",
                                                                                           "",
                                                                                           row.Item("Gate").ToString().Trim(),
                                                                                           row.Item("Turnstile").ToString().Trim(),
                                                                                           row.Item("Stand").ToString().Trim(),
                                                                                           row.Item("Area").ToString().Trim(),
                                                                                           row.Item("Row").ToString().Trim(),
                                                                                           row.Item("Seat").ToString().Trim(),
                                                                                           row.Item("AlphaSuffix").ToString().Trim(),
                                                                                           row.Item("SeatRestriction").ToString().Trim(),
                                                                                           row.Item("RestText").ToString().Trim(),
                                                                                           String.Empty,
                                                                                           String.Empty,
                                                                                           False,
                                                                                           False,
                                                                                           False,
                                                                                           0,
                                                                                           row.Item("RequestedPrice").ToString().Trim(),
                                                                                           row.Item("ClubHandlingFee").ToString().Trim(),
                                                                                           row.Item("PreviousRequestedPrice").ToString().Trim(),
                                                                                           row.Item("FaceValue").ToString().Trim(),
                                                                                           row.Item("OriginalSalePayref").ToString().Trim(),
                                                                                           row.Item("Earnings").ToString().Trim(),
                                                                                           row.Item("TicketExchangeRef").ToString().Trim()
                                                                                           ))


                                            End If
                                        End If
                                        If row.Item("OnSale") = False Then
                                            seatsTakenOffSale = True
                                            RebookedItems.Append(ReplaceTableRowContentFields(TicketExchangeRebookRow,
                                                                                       row.Item("ProductDescription").ToString.Trim(),
                                                                                       getProductDate(row.Item("BuyBackDate")),
                                                                                       "",
                                                                                       row.Item("Title").ToString().Trim(),
                                                                                       row.Item("ContactForename").ToString().Trim(),
                                                                                       row.Item("ContactSurname").ToString().Trim(),
                                                                                       row.Item("MemberNumber").ToString().TrimStart("0"),
                                                                                       "",
                                                                                       "",
                                                                                       "",
                                                                                       "",
                                                                                       row.Item("Gate").ToString().Trim(),
                                                                                       row.Item("Turnstile").ToString().Trim(),
                                                                                       row.Item("Stand").ToString().Trim(),
                                                                                       row.Item("Area").ToString().Trim(),
                                                                                       row.Item("Row").ToString().Trim(),
                                                                                       row.Item("Seat").ToString().Trim(),
                                                                                       row.Item("AlphaSuffix").ToString().Trim(),
                                                                                       row.Item("SeatRestriction").ToString().Trim(),
                                                                                       row.Item("RestText").ToString().Trim(),
                                                                                       String.Empty,
                                                                                       String.Empty,
                                                                                       False,
                                                                                       False,
                                                                                       False,
                                                                                      0,
                                                                                       row.Item("RequestedPrice").ToString().Trim(),
                                                                                       row.Item("ClubHandlingFee").ToString().Trim(),
                                                                                       row.Item("PreviousRequestedPrice").ToString().Trim(),
                                                                                       row.Item("FaceValue").ToString().Trim(),
                                                                                       row.Item("OriginalSalePayref").ToString().Trim(),
                                                                                       String.Empty,
                                                                                       row.Item("TicketExchangeRef").ToString().Trim()
                                                                                       ))
                                        End If


                                    End If


                                Next

                                If HtmlFormat Then
                                    If seatsPutOnSale Then
                                        PutOnSaleItems.Append("</table>")
                                    Else
                                        PutOnSaleItems.Clear()
                                    End If
                                    If seatsTakenOffSale Then
                                        RebookedItems.Append("</table>")
                                    Else
                                        RebookedItems.Clear()
                                    End If
                                    If seatsRepriced Then
                                        PriceChangeItems.Append("</table>")
                                    Else
                                        PriceChangeItems.Clear()
                                    End If
                                End If


                                emailText.Replace("<<TicketExchangePutOnSaleItems>>", PutOnSaleItems.ToString())
                                emailText.Replace("<<TicketExchangeRebookedItems>>", RebookedItems.ToString())
                                emailText.Replace("<<TicketExchangePriceChangeItems>>", PriceChangeItems.ToString())

                                '
                                ' Replace the field delimiters with data
                                '
                                For i As Integer = 0 To 100
                                    If strField(i) > Nothing Then
                                        emailSubject.Replace(strField(i), strData(i))
                                        emailText.Replace(strField(i), strData(i))
                                    End If
                                Next

                                If HtmlFormat Then
                                    emailText.Replace("<<NewLine>>", "<br />")
                                Else
                                    emailText.Replace("<<NewLine>>", vbCrLf)
                                End If

                                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject.ToString(),
                                                                     emailText.ToString(), "", False, HtmlFormat)
                            End If

                        Else
                            err.HasError = True
                            err.ErrorNumber = "TACTEMAIL-17"
                            err.ErrorMessage = String.Format("Error Retrieving the ticket exchange details. Ticket Exchange Reference : {0}",
                                                          EmailSettings.TicketExchangeConfirmation.TicketExchangeReference)
                        End If

                    End If

                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-18"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.TicketExchangeConfirmation.Customer)
                End If
            Else
                emailText.Append(_emailBody)
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject.ToString(),
                                                         RemoveMergeBrackets(emailText.ToString()), "", False, HtmlFormat)
            End If

        End If

        Return err

    End Function

    Public Function SendTicketExchangeSaleConfirmationEmail() As ErrorObj
        Dim err As New ErrorObj


        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_RETURN_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_TICKET_EXCHANGE_SALE_CONFIRM, err)

            If err.HasError Then Return err


            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            'Look for controltextlang with control set to email ID (as used for other emails) and if not found for records with the name of the template  
            Dim TicketExchangeSoldHeader As String
            TicketExchangeSoldHeader = ucr.Content("TicketExchangeSoldHeader", Settings.Language, False)
            If TicketExchangeSoldHeader = String.Empty Then
                ucr.KeyCode = "TalentEmail.vb.TicketExchangeSaleConfirm"
            End If

            Dim HtmlFormat As Boolean = _emailHTML
            Dim emailSubject As New StringBuilder(_emailSubject)
            Dim emailText As New StringBuilder()

            TicketExchangeSoldHeader = ucr.Content("TicketExchangeSoldHeader", Settings.Language, False)
            Dim TicketExchangeSoldHeading As String = ucr.Content("TicketExchangeSoldHeading", Settings.Language, False)
            Dim TicketExchangeSoldRow As String = ucr.Content("TicketExchangeSoldRow", Settings.Language, False)



            If Not _emailNoDataRetrieval Then
                '
                ' Get the customer information
                '
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.TicketExchangeSaleConfirmation.Customer
                DeCust.UserName = EmailSettings.TicketExchangeSaleConfirmation.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-05"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.TicketExchangeSaleConfirmation.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))

                    Else

                        err = OrderHeaderTicketingValues()

                        If HtmlFormat Then
                            strData(0) = "<br />"
                        Else
                            strData(0) = vbCrLf
                        End If
                        strData(1) = "" ' PaymentReference
                        strData(2) = tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim()
                        strData(6) = tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim() & " " &
                                  tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim()
                        strData(7) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine1").ToString().Trim()
                        strData(8) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine2").ToString().Trim()
                        strData(9) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine3").ToString().Trim()
                        strData(10) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine4").ToString().Trim()
                        strData(11) = tc.ResultDataSet.Tables(1).Rows(0).Item("AddressLine5").ToString().Trim()
                        strData(12) = tc.ResultDataSet.Tables(1).Rows(0).Item("PostCode").ToString().Trim()
                        strData(13) = "" ' Country
                        strData(14) = tc.ResultDataSet.Tables(1).Rows(0).Item("HomeTelephoneNumber").ToString().Trim()
                        strData(15) = EmailSettings.ToAddress

                        strData(57) = "" ' TotalOrderValue
                        strData(58) = "" ' TotalOrderValue Formatted as currency
                        strData(59) = "" ' TotalOrderValue Formatted as currency

                        '
                        '   Join all the paragraphs together
                        '
                        If HtmlFormat Then
                            For i As Integer = 0 To 10
                                'Leave the <br> tag for better formatting
                                emailText.Append(strCode(i))
                            Next
                        Else
                            For i As Integer = 0 To 10
                                emailText.Append(strCode(i) & vbCrLf)
                            Next
                        End If

                        emailText.Replace("<<email>>", EmailSettings.ToAddress)
                        emailText.Replace("<<Title>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText.Replace("<<Forename>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim())
                        emailText.Replace("<<Surname>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())

                        '
                        ' Add the order lines
                        '
                        Dim deTicketExchange As New DETicketExchange()
                        deTicketExchange.OriginatingSourceCode = Settings.OriginatingSourceCode
                        deTicketExchange.Customer = EmailSettings.TicketExchangeSaleConfirmation.Customer
                        deTicketExchange.ResoldPayref = EmailSettings.TicketExchangeSaleConfirmation.PaymentReference

                        Dim tTicketExchange As New TalentTicketExchange()
                        With tTicketExchange
                            .Dep = deTicketExchange
                            .Settings = Settings
                            .Settings.CacheStringExtension = ""
                            err = .TicketExchangeEnquiry()
                        End With

                        If Not err.HasError AndAlso Not tTicketExchange.ResultDataSet Is Nothing AndAlso tTicketExchange.ResultDataSet.Tables(0).Rows.Count > 0 Then

                            If tTicketExchange.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-16"
                                err.ErrorMessage = String.Format("Error Retrieving the order details. Payment Ref : {0}, Return Code : {1}",
                                                              EmailSettings.TicketExchangeSaleConfirmation.PaymentReference, tTicketExchange.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
                            Else

                                Dim OrderItems As New StringBuilder("")
                                Dim SoldItems As New StringBuilder(TicketExchangeSoldHeading)

                                If HtmlFormat Then
                                    SoldItems.Append("<br> <table " & ucr.Attribute("TicketingTableStyle") & ">" & TicketExchangeSoldHeader)
                                End If

                                For Each row As DataRow In tTicketExchange.ResultDataSet.Tables("TicketExchangeEnquiry").Rows

                                    Dim ltlSold As String = String.Empty
                                    If Not HtmlFormat Then

                                        ltlSold = ucr.Content("ltlSold", Settings.Language, False)

                                        OrderItems.Append(row.Item("MemberNumber").ToString().Trim())
                                        OrderItems.Append(ucr.Content("TicketingSeperator1", Settings.Language, False) & row.Item("ProductDescription").ToString().Trim())
                                        OrderItems.Append(ucr.Content("TicketingSeperator2", Settings.Language, False) & getProductDateTime(row.Item("BuyBackDate"), ""))
                                        OrderItems.Append(ucr.Content("TicketingSeperator3", Settings.Language, False) & row.Item("Stand").ToString().Trim() & " " &
                                                                                                                        row.Item("Area").ToString().Trim() & " " &
                                                                                                                        row.Item("Row").ToString().Trim() & " " &
                                                                                                                        row.Item("Seat").ToString().Trim() &
                                                                                                                        row.Item("AlphaSuffix").ToString().Trim() & " " &
                                                                                                                        row.Item("RequestedPrice").ToString().Trim() & " " &
                                                                                                                        row.Item("ClubHandlingFee").ToString().Trim() & " " &
                                                                                                                        row.Item("FaceValue").ToString().Trim() & " " &
                                                                                                                        row.Item("Earnings").ToString().Trim())

                                        OrderItems.Append(vbCrLf)

                                    Else


                                        SoldItems.Append(ReplaceTableRowContentFields(TicketExchangeSoldRow,
                                                                               row.Item("ProductDescription").ToString.Trim(),
                                                                               getProductDate(row.Item("BuyBackDate")),
                                                                               "",
                                                                               row.Item("Title").ToString().Trim(),
                                                                               row.Item("ContactForename").ToString().Trim(),
                                                                               row.Item("ContactSurname").ToString().Trim(),
                                                                               row.Item("MemberNumber").ToString().TrimStart("0"),
                                                                               "",
                                                                               "",
                                                                               "",
                                                                               "",
                                                                               row.Item("Gate").ToString().Trim(),
                                                                               row.Item("Turnstile").ToString().Trim(),
                                                                               row.Item("Stand").ToString().Trim(),
                                                                               row.Item("Area").ToString().Trim(),
                                                                               row.Item("Row").ToString().Trim(),
                                                                               row.Item("Seat").ToString().Trim(),
                                                                               row.Item("AlphaSuffix").ToString().Trim(),
                                                                               row.Item("SeatRestriction").ToString().Trim(),
                                                                               row.Item("RestText").ToString().Trim(),
                                                                               String.Empty,
                                                                               String.Empty,
                                                                               False,
                                                                               False,
                                                                               False,
                                                                               0,
                                                                               row.Item("RequestedPrice").ToString().Trim(),
                                                                               row.Item("ClubHandlingFee").ToString().Trim(),
                                                                               String.Empty,
                                                                               row.Item("FaceValue").ToString().Trim(),
                                                                               row.Item("OriginalSalePayref").ToString().Trim(),
                                                                               row.Item("Earnings").ToString().Trim(),
                                                                               row.Item("TicketExchangeRef").ToString().Trim()
                                                                               ))
                                    End If


                                Next

                                If HtmlFormat Then
                                    SoldItems.Append("</table>")
                                End If



                                emailText.Replace("<<TicketExchangeSoldItems>>", SoldItems.ToString())

                                '
                                ' Replace the field delimiters with data
                                '
                                For i As Integer = 0 To 100
                                    If strField(i) > Nothing Then
                                        emailSubject.Replace(strField(i), strData(i))
                                        emailText.Replace(strField(i), strData(i))
                                    End If
                                Next

                                If HtmlFormat Then
                                    emailText.Replace("<<NewLine>>", "<br />")
                                Else
                                    emailText.Replace("<<NewLine>>", vbCrLf)
                                End If

                                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject.ToString(),
                                                                         emailText.ToString(), "", False, HtmlFormat)
                            End If

                        Else
                            err.HasError = True
                            err.ErrorNumber = "TACTEMAIL-17"
                            err.ErrorMessage = String.Format("Error Retrieving the ticket exchange details. Payment Reference : {0}",
                                                          EmailSettings.TicketExchangeSaleConfirmation.PaymentReference)
                        End If

                    End If

                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-18"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.TicketExchangeSaleConfirmation.Customer)
                End If
            Else
                emailText.Append(_emailBody)
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject.ToString(),
                                                         RemoveMergeBrackets(emailText.ToString()), "", False, HtmlFormat)
            End If

        End If

        Return err

    End Function

    Public Function CreateCustomerRegistrationXmlDocument(ByVal fromAddress As String,
                                                          ByVal toAddress As String,
                                                          ByVal smtpServer As String,
                                                          ByVal smtpServerPort As String,
                                                          ByVal Partner As String,
                                                          ByVal Customer As String,
                                                          ByVal WebsiteAddress As String,
                                                          Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode

        Dim ndParameters, ndCustomer, ndWebsiteAddress, ndTemplateID As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")

                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndWebsiteAddress = .CreateElement("WebsiteAddress")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID
            ndCustomer.InnerText = Customer
            ndWebsiteAddress.InnerText = WebsiteAddress


            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndWebsiteAddress)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)


        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function

    Public Function SendCustomerRegistrationEmail() As ErrorObj
        Dim err As New ErrorObj
        Dim HtmlFormat As Boolean = False
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_TICKETING_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_CUSTOMER_REGISTRATION, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                '                .KeyCode = "RegistrationForm.ascx"
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            HtmlFormat = _emailHTML
            Dim emailText As String = _emailBody

            'Dim emailSubject As String = ucr.Content("ConfirmationEmailSubject", Settings.Language, False)
            'Dim emailFormat As String = ucr.Attribute("emailFormat")
            'If emailFormat.ToUpper() = "HTML" Then
            '    HtmlFormat = True
            'End If
            'Dim emailText As String
            'If HtmlFormat Then
            '    emailText = ucr.Content("ConfirmationEmailBodyHTML", Settings.Language, False)
            'Else
            '    emailText = ucr.Content("ConfirmationEmailBody", Settings.Language, False)
            'End If


            If Not _emailNoDataRetrieval Then
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.CustomerRegistration.Customer
                DeCust.UserName = EmailSettings.CustomerRegistration.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-03"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.CustomerRegistration.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    End If

                    If Not err.HasError Then

                        emailText = Replace(emailText, "<<Title>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText = Replace(emailText, "<<To>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim())
                        emailText = Replace(emailText, "<<Surname>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())
                        emailText = emailText.Replace("<<CustomerNumber>>", tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim().TrimStart("0"))

                        Dim loginType As String = Me.TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "LOGINID_TYPE", False)

                        If loginType.Equals("1") Then
                            emailText = emailText.Replace("<<UserName>>", tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim().TrimStart("0"))
                        Else
                            emailText = emailText.Replace("<<UserName>>", tc.ResultDataSet.Tables(1).Rows(0).Item("CustomerNumber").ToString().Trim())
                        End If

                        tc.ResultDataSet = Nothing
                        tc.RetrievePassword()

                        emailText = Replace(emailText, "<<Password>>", tc.ResultDataSet.Tables("RetrievePasswordResults").Rows(0).Item("Password").ToString().Trim())

                        emailText = emailText.Replace("<<WebSiteAddress>>", EmailSettings.CustomerRegistration.WebsiteAddress)

                        If HtmlFormat Then
                            emailText = Replace(emailText, "<<NewLine>>", "<br>")
                        Else
                            emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                        End If

                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                 emailText, "", False, HtmlFormat)

                    End If

                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-03a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.CustomerRegistration.Customer)
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If

        End If

        Return err

    End Function

    Public Function CreateChangePasswordXmlDocument(ByVal fromAddress As String,
                                                    ByVal toAddress As String,
                                                    ByVal smtpServer As String,
                                                    ByVal smtpServerPort As String,
                                                    ByVal Partner As String,
                                                    ByVal Customer As String,
                                                    ByVal Token As String) As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode

        Dim ndParameters, ndCustomer, ndToken As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")

                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")

                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndToken = .CreateElement("Token")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner

            ndCustomer.InnerText = Customer
            ndToken.InnerText = Token


            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndToken)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)


        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function

    Public Function SendChangePasswordEmail() As ErrorObj

        ' True
        Dim err As New ErrorObj
        Dim HtmlFormat As Boolean = False
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_TICKETING_CANCEL_AMMEND_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_CHANGE_PASSWORD, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                '                .KeyCode = "ForgottenPasswordForm.ascx"
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            HtmlFormat = _emailHTML
            Dim emailText As String = _emailBody

            'Dim emailSubject As String = ucr.Content("emailSubject", Settings.Language, False)
            'Dim emailFormat As String = ucr.Attribute("ConfimationEMailBodyHTML")
            'If emailFormat.ToUpper() = "HTML" Then
            '    HtmlFormat = True
            'End If


            If Not _emailNoDataRetrieval Then
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.ChangePassword.Customer
                DeCust.UserName = EmailSettings.ChangePassword.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-02"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.ChangePassword.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    End If

                    If Not err.HasError Then

                        emailText = Replace(emailText, "<<To>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText = Replace(emailText, "<<Name>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim() & " " &
                                                                   tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())

                        emailText = Replace(emailText, "<<LoginURL>>", EmailSettings.ChangePassword.LoginUrl)

                        If HtmlFormat Then
                            emailText = Replace(emailText, "<<NewLine>>", "<br>")
                        Else
                            emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                        End If

                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                 emailText, "", False, HtmlFormat)
                    End If

                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-02a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}", EmailSettings.ChangePassword.Customer)

                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If

        End If

        Return err

    End Function

    Public Function CreateForgottenPasswordXmlDocument(ByVal fromAddress As String,
                                                       ByVal toAddress As String,
                                                       ByVal smtpServer As String,
                                                       ByVal smtpServerPort As String,
                                                       ByVal Partner As String,
                                                       ByVal Customer As String,
                                                       ByVal loginUrl As String,
                                                       Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode

        Dim ndParameters, ndCustomer, ndloginUrl, ndTemplateID As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")

                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndloginUrl = .CreateElement("LoginUrl")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID

            ndCustomer.InnerText = Customer
            ndloginUrl.InnerText = loginUrl


            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndloginUrl)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)


        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function

    Public Function SendForgottenPasswordEmail() As ErrorObj
        Dim err As New ErrorObj
        Dim HtmlFormat As Boolean = False

        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_FORGOTTEN_PASSWORD, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                '                .KeyCode = "ForgottenPasswordForm.ascx"
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            HtmlFormat = _emailHTML
            Dim emailText As String = _emailBody

            'Dim emailSubject As String = ucr.Content("emailSubject", Settings.Language, False)
            'Dim emailFormat As String = ucr.Attribute("ConfimationEMailBodyHTML")
            'If emailFormat.ToUpper() = "HTML" Then
            '    HtmlFormat = True
            'End If

            'Dim emailText As String
            'If HtmlFormat Then
            '    emailText = ucr.Content("emailHTMLText", Settings.Language, False)
            'Else
            '    emailText = ucr.Content("emailText", Settings.Language, False)
            'End If


            If Not _emailNoDataRetrieval Then
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.ForgottenPassword.Customer
                DeCust.UserName = EmailSettings.ForgottenPassword.Customer

                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)

                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With

                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-04"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.ForgottenPassword.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    End If

                    If Not err.HasError Then

                        emailText = Replace(emailText, "<<To>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText = Replace(emailText, "<<Title>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                        emailText = Replace(emailText, "<<Name>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim() & " " &
                                                                   tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())
                        emailText = Replace(emailText, "<<Surname>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())
                        emailText = Replace(emailText, "<<Password>>", tc.ResultDataSet.Tables(1).Rows(0).Item("PasswordHint").ToString().Trim())
                        emailText = Replace(emailText, "<<LoginUrl>>", EmailSettings.ForgottenPassword.LoginUrl)
                        emailText = Replace(emailText, "<<CustomerNumber>>", EmailSettings.ForgottenPassword.Customer)
                        If HtmlFormat Then
                            emailText = Replace(emailText, "<<NewLine>>", "<br>")
                        Else
                            emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                        End If

                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                 emailText, "", False, HtmlFormat)

                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-04a"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.ForgottenPassword.Customer)
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If
        End If

        Return err

    End Function

    Public Function CreateAmendPPSXmlDocument(ByVal fromAddress As String,
                                                       ByVal toAddress As String,
                                                       ByVal smtpServer As String,
                                                       ByVal smtpServerPort As String,
                                                       ByVal Partner As String,
                                                       ByVal Customer As String,
                                                       ByVal ProductCode As String,
                                                       Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode
        Dim ndParameters, ndCustomer, ndProductCode, ndTemplateID As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndProductCode = .CreateElement("ProductCode")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID
            ndCustomer.InnerText = Customer
            ndProductCode.InnerText = ProductCode

            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndProductCode)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)


        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)
    End Function

    Public Function SendAmendPPSEmail() As ErrorObj

        ' True
        Dim err As New ErrorObj
        Dim HtmlFormat As Boolean = True

        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_PPS_AMEND, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            Dim emailSubject As String = _emailSubject
            Dim emailText As String = _emailBody

            'Dim emailSubject As String = ucr.Content("AmendPPSEmailSubject", Settings.Language, False)
            'Dim emailText As String = ucr.Content("AmendPPSEmailBody", Settings.Language, False)

            Dim emailPPSDetails As String = String.Empty
            Dim customerName As String = String.Empty

            If Not _emailNoDataRetrieval Then
                Dim product As New TalentProduct
                product.Settings = Settings
                product.De.ProductType = "P"
                product.De.StadiumCode = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "TICKETING_STADIUM")
                product.De.CustomerNumber = EmailSettings.AmendPPS.Customer
                product.De.PPSType = "*"
                product.De.Src = "W"
                err = product.ProductList()

                If Not err.HasError AndAlso Not product.ResultDataSet Is Nothing AndAlso product.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If product.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-12a"
                        err.ErrorMessage = String.Format("Error Retrieving the PPS products (WS006R). Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.AmendPPS.Customer, product.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    End If

                    If Not err.HasError Then
                        Dim productList As DataTable = product.ResultDataSet.Tables(1)
                        If String.IsNullOrEmpty(EmailSettings.AmendPPS.ProductCode) Then
                            productList.DefaultView.RowFilter = "RelatingSeasonTicket<>''"
                        Else
                            productList.DefaultView.RowFilter = "RelatingSeasonTicket='" & EmailSettings.AmendPPS.ProductCode & "'"
                        End If
                        Dim filteredProductList As DataTable = productList.DefaultView.ToTable

                        If filteredProductList.Rows.Count = 0 Then
                            err.HasError = True
                            err.ErrorNumber = "TACTEMAIL-12b"
                            err.ErrorMessage = String.Format("Error Retrieving the PPS products (WS006R). Customer Number : {0}, no PPS products relating to this season ticket : {1}",
                                                              EmailSettings.AmendPPS.Customer, EmailSettings.AmendPPS.ProductCode)
                        Else
                            Dim okToSendEmail As Boolean = False
                            emailPPSDetails = ucr.Content("PPSProductHeader", Settings.Language, False)
                            For Each ppsRow As DataRow In filteredProductList.Rows

                                Dim ppsProduct As String = ppsRow("ProductCode").ToString.Trim
                                Dim ppsProductDescription As String = ppsRow("ProductDescription").ToString.Trim
                                Dim AmendPPSEnrolmentIgnoreFF As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "AMEND_PPS_ENROLMENT_IGNORE_FF"))
                                Dim amendPPSEnrolmentScheme As New TalentPPS
                                Dim dEPPSEnrolmentScheme As New DEPPSEnrolmentScheme
                                Dim PPSProductRow As String = ucr.Content("PPSProductRow", Settings.Language, False)

                                amendPPSEnrolmentScheme.Settings = Settings
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme = dEPPSEnrolmentScheme
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.AmendPPSEnrolmentIgnoreFF = AmendPPSEnrolmentIgnoreFF
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.CustomerNumber = EmailSettings.AmendPPS.Customer
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.ProductCode = ppsProduct
                                err = amendPPSEnrolmentScheme.AmendPPS()

                                If Not err.HasError AndAlso Not amendPPSEnrolmentScheme.ResultDataSet Is Nothing Then

                                    If amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows.Count > 0 Then
                                        If amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                                            err.HasError = True
                                            err.ErrorNumber = "TACTEMAIL-12c"
                                            err.ErrorMessage = String.Format("Error Retrieving the customer PPS Enrolments (WS617R). Customer Number : {0}, Return Code : {1}",
                                                                              EmailSettings.AmendPPS.Customer, amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                                        End If
                                    End If

                                    If Not err.HasError Then

                                        Dim enrolmentsTable As DataTable = amendPPSEnrolmentScheme.ResultDataSet.Tables(1)
                                        If enrolmentsTable.Rows.Count = 0 Then
                                            err.HasError = True
                                            err.ErrorNumber = "TACTEMAIL-12d"
                                            err.ErrorMessage = String.Format("Error Retrieving the customer PPS Enrolments (WS617R). Customer Number : {0}, Return Code : {1}",
                                                                                  EmailSettings.AmendPPS.Customer, amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                                        Else
                                            Dim insertPPSProductHeader As Boolean = True
                                            For Each enrolmentRow As DataRow In enrolmentsTable.Rows
                                                If Not String.IsNullOrEmpty(enrolmentRow("SeatDetails").ToString.Trim) Then
                                                    okToSendEmail = True
                                                    If insertPPSProductHeader Then
                                                        PPSProductRow = Replace(PPSProductRow, "<<PPSProduct>>", ppsProduct)
                                                        PPSProductRow = Replace(PPSProductRow, "<<PPSDescription>>", ppsProductDescription)
                                                        emailPPSDetails &= PPSProductRow
                                                        emailPPSDetails &= ucr.Content("PPSEnrolmentHeader", Settings.Language, False)
                                                        insertPPSProductHeader = False
                                                    End If
                                                    If Utilities.PadLeadingZeros(enrolmentRow("CustomerNumber").ToString, 12) = Utilities.PadLeadingZeros(EmailSettings.AmendPPS.Customer, 12) Then
                                                        customerName = enrolmentRow("Name")
                                                    End If
                                                    Dim ppsEnrolmentRow As String = ucr.Content("PPSEnrolmentRow", Settings.Language, False)
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<CustomerNumber>>", enrolmentRow("CustomerNumber").ToString().TrimStart("0"))
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<FullSeatDetails>>", enrolmentRow("SeatDetails").ToString().Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<StandDetails>>", enrolmentRow("SeatDetails").ToString().Substring(0, 3).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<AreaDetails>>", enrolmentRow("SeatDetails").ToString().Substring(3, 4).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<RowDetails>>", enrolmentRow("SeatDetails").ToString().Substring(7, 4).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<SeatDetails>>", enrolmentRow("SeatDetails").ToString().Substring(11, 5).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Name>>", enrolmentRow("Name").ToString().Trim())
                                                    Dim enrolled As Boolean = False
                                                    Try
                                                        enrolled = CBool(enrolmentRow("Enrolled"))
                                                        If enrolled Then
                                                            ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Enrolled>>", ucr.Content("EnrolledText", Settings.Language, False))
                                                        Else
                                                            ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Enrolled>>", ucr.Content("NotEnrolledText", Settings.Language, False))
                                                        End If
                                                    Catch ex As Exception
                                                        ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Enrolled>>", ucr.Content("NotEnrolledText", Settings.Language, False))
                                                    End Try
                                                    emailPPSDetails &= ppsEnrolmentRow
                                                End If
                                            Next
                                            If Not insertPPSProductHeader Then emailPPSDetails &= ucr.Content("PPSEnrolmentFooter", Settings.Language, False)
                                        End If

                                    End If
                                Else
                                    err.HasError = True
                                    err.ErrorNumber = "TACTEMAIL-12e"
                                    err.ErrorMessage = String.Format("Error Retrieving the customer PPS Enrolments (WS617R). Customer Number : {0}, Nothing Returned", EmailSettings.AmendPPS.Customer)
                                End If
                            Next

                            If okToSendEmail Then
                                emailPPSDetails &= ucr.Content("PPSProductFooter", Settings.Language, False)
                                emailText = Replace(emailText, "<<Name>>", customerName)
                                emailText = Replace(emailText, "<<PPSDetails>>", emailPPSDetails)

                                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                     emailText, "", False, HtmlFormat)
                            Else
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-12f"
                                err.ErrorMessage = String.Format("There is no PPS Enrolments (WS617R) for customer number : {0}", EmailSettings.AmendPPS.Customer)
                            End If
                        End If
                    End If
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                     RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If

        End If

        Return err
    End Function

    Public Function CreateAmendPPSPaymentsXmlDocument(ByVal fromAddress As String,
                                                       ByVal toAddress As String,
                                                       ByVal smtpServer As String,
                                                       ByVal smtpServerPort As String,
                                                       ByVal Partner As String,
                                                       ByVal Customer As String,
                                                       ByVal ProductCode As String,
                                                       ByVal SeatDetails As String,
                                                       Optional ByVal TemplateID As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode
        Dim ndParameters, ndCustomer, ndProductCode, ndSeatDetails, ndTemplateID As XmlNode

        Try

            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndProductCode = .CreateElement("ProductCode")
                ndSeatDetails = .CreateElement("SeatDetails")
            End With

            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID
            ndCustomer.InnerText = Customer
            ndProductCode.InnerText = ProductCode
            ndSeatDetails.InnerText = SeatDetails

            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndProductCode)
                .AppendChild(ndSeatDetails)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)

        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)
    End Function

    Public Function SendAmendPPSPaymentsEmail() As ErrorObj

        ' (This is an obsolete function.)
        Dim err As New ErrorObj
        Dim HtmlFormat As Boolean = True

        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_PPS_AMEND_PAYMENT, err)
            If err.HasError Then Return err

            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            'Dim emailSubject As String = ucr.Content("AmendPPSPaymentsEmailSubject", Settings.Language, False)
            'Dim emailText As String = ucr.Content("AmendPPSPaymentsEmailBody", Settings.Language, False)
            Dim emailSubject As String = _emailSubject
            Dim emailText As String = _emailBody

            Dim emailPPSDetails As String = String.Empty
            Dim customerName As String = String.Empty

            If Not _emailNoDataRetrieval Then
                Dim product As New TalentProduct
                product.Settings = Settings
                product.De.ProductType = "P"
                product.De.StadiumCode = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "TICKETING_STADIUM")
                product.De.CustomerNumber = EmailSettings.AmendPPS.Customer
                product.De.PPSType = "*"
                product.De.Src = "W"
                err = product.ProductList()

                If Not err.HasError AndAlso Not product.ResultDataSet Is Nothing AndAlso product.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If product.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-12a"
                        err.ErrorMessage = String.Format("Error Retrieving the PPS products (WS006R). Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.AmendPPS.Customer, product.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    End If

                    If Not err.HasError Then
                        Dim productList As DataTable = product.ResultDataSet.Tables(1)
                        If String.IsNullOrEmpty(EmailSettings.AmendPPS.ProductCode) Then
                            productList.DefaultView.RowFilter = "RelatingSeasonTicket<>''"
                        Else
                            productList.DefaultView.RowFilter = "RelatingSeasonTicket='" & EmailSettings.AmendPPS.ProductCode & "'"
                        End If
                        Dim filteredProductList As DataTable = productList.DefaultView.ToTable

                        If filteredProductList.Rows.Count = 0 Then
                            err.HasError = True
                            err.ErrorNumber = "TACTEMAIL-12b"
                            err.ErrorMessage = String.Format("Error Retrieving the PPS products (WS006R). Customer Number : {0}, no PPS products relating to this season ticket : {1}",
                                                              EmailSettings.AmendPPS.Customer, EmailSettings.AmendPPS.ProductCode)
                        Else
                            Dim okToSendEmail As Boolean = False
                            emailPPSDetails = ucr.Content("PPSProductHeader", Settings.Language, False)
                            For Each ppsRow As DataRow In filteredProductList.Rows

                                Dim ppsProduct As String = ppsRow("ProductCode").ToString.Trim
                                Dim ppsProductDescription As String = ppsRow("ProductDescription").ToString.Trim
                                Dim AmendPPSEnrolmentIgnoreFF As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "AMEND_PPS_ENROLMENT_IGNORE_FF"))
                                Dim amendPPSEnrolmentScheme As New TalentPPS
                                Dim dEPPSEnrolmentScheme As New DEPPSEnrolmentScheme
                                Dim PPSProductRow As String = ucr.Content("PPSProductRow", Settings.Language, False)

                                amendPPSEnrolmentScheme.Settings = Settings
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme = dEPPSEnrolmentScheme
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.AmendPPSEnrolmentIgnoreFF = AmendPPSEnrolmentIgnoreFF
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.CustomerNumber = EmailSettings.AmendPPS.Customer
                                amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.ProductCode = ppsProduct
                                err = amendPPSEnrolmentScheme.AmendPPS()

                                If Not err.HasError AndAlso Not amendPPSEnrolmentScheme.ResultDataSet Is Nothing Then

                                    If amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows.Count > 0 Then
                                        If amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                                            err.HasError = True
                                            err.ErrorNumber = "TACTEMAIL-12c"
                                            err.ErrorMessage = String.Format("Error Retrieving the customer PPS Enrolments (WS617R). Customer Number : {0}, Return Code : {1}",
                                                                              EmailSettings.AmendPPS.Customer, amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                                        End If
                                    End If

                                    If Not err.HasError Then

                                        Dim enrolmentsTable As DataTable = amendPPSEnrolmentScheme.ResultDataSet.Tables(1)
                                        If enrolmentsTable.Rows.Count = 0 Then
                                            err.HasError = True
                                            err.ErrorNumber = "TACTEMAIL-12d"
                                            err.ErrorMessage = String.Format("Error Retrieving the customer PPS Enrolments (WS617R). Customer Number : {0}, Return Code : {1}",
                                                                                  EmailSettings.AmendPPS.Customer, amendPPSEnrolmentScheme.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                                        Else
                                            Dim insertPPSProductHeader As Boolean = True
                                            For Each enrolmentRow As DataRow In enrolmentsTable.Rows
                                                If Not String.IsNullOrEmpty(enrolmentRow("SeatDetails").ToString.Trim) Then
                                                    okToSendEmail = True
                                                    If insertPPSProductHeader Then
                                                        PPSProductRow = Replace(PPSProductRow, "<<PPSProduct>>", ppsProduct)
                                                        PPSProductRow = Replace(PPSProductRow, "<<PPSDescription>>", ppsProductDescription)
                                                        emailPPSDetails &= PPSProductRow
                                                        emailPPSDetails &= ucr.Content("PPSEnrolmentHeader", Settings.Language, False)
                                                        insertPPSProductHeader = False
                                                    End If
                                                    If Utilities.PadLeadingZeros(enrolmentRow("CustomerNumber").ToString, 12) = Utilities.PadLeadingZeros(EmailSettings.AmendPPS.Customer, 12) Then
                                                        customerName = enrolmentRow("Name")
                                                    End If
                                                    Dim ppsEnrolmentRow As String = ucr.Content("PPSEnrolmentRow", Settings.Language, False)
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<CustomerNumber>>", enrolmentRow("CustomerNumber").ToString().TrimStart("0"))
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<FullSeatDetails>>", enrolmentRow("SeatDetails").ToString().Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<StandDetails>>", enrolmentRow("SeatDetails").ToString().Substring(0, 3).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<AreaDetails>>", enrolmentRow("SeatDetails").ToString().Substring(3, 4).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<RowDetails>>", enrolmentRow("SeatDetails").ToString().Substring(7, 4).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<SeatDetails>>", enrolmentRow("SeatDetails").ToString().Substring(11, 5).Trim())
                                                    ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Name>>", enrolmentRow("Name").ToString().Trim())
                                                    Dim enrolled As Boolean = False
                                                    Try
                                                        enrolled = CBool(enrolmentRow("Enrolled"))
                                                        If enrolled Then
                                                            ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Enrolled>>", ucr.Content("EnrolledText", Settings.Language, False))
                                                        Else
                                                            ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Enrolled>>", ucr.Content("NotEnrolledText", Settings.Language, False))
                                                        End If
                                                    Catch ex As Exception
                                                        ppsEnrolmentRow = Replace(ppsEnrolmentRow, "<<Enrolled>>", ucr.Content("NotEnrolledText", Settings.Language, False))
                                                    End Try
                                                    emailPPSDetails &= ppsEnrolmentRow
                                                End If
                                            Next
                                            If Not insertPPSProductHeader Then emailPPSDetails &= ucr.Content("PPSEnrolmentFooter", Settings.Language, False)
                                        End If

                                    End If
                                Else
                                    err.HasError = True
                                    err.ErrorNumber = "TACTEMAIL-12e"
                                    err.ErrorMessage = String.Format("Error Retrieving the customer PPS Enrolments (WS617R). Customer Number : {0}, Nothing Returned", EmailSettings.AmendPPS.Customer)
                                End If
                            Next

                            If okToSendEmail Then
                                emailPPSDetails &= ucr.Content("PPSProductFooter", Settings.Language, False)
                                emailText = Replace(emailText, "<<Name>>", customerName)
                                emailText = Replace(emailText, "<<PPSDetails>>", emailPPSDetails)

                                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                     emailText, "", False, HtmlFormat)
                            Else
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-12f"
                                err.ErrorMessage = String.Format("There is no PPS Enrolments (WS617R) for customer number : {0}", EmailSettings.AmendPPS.Customer)
                            End If
                        End If
                    End If
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                     RemoveMergeBrackets(emailText), "", False, HtmlFormat)
            End If

        End If

        Return err
    End Function

    Public Function CreateTicketExchangeSaleXmlDocument(ByVal fromAddress As String,
                                                ByVal toAddress As String,
                                                ByVal ccAddress As String,
                                                ByVal smtpServer As String,
                                                ByVal smtpServerPort As String,
                                                ByVal Partner As String,
                                                ByVal attachments As String,
                                                ByVal paymentRef As String,
                                                ByVal customer As String,
                                                ByVal originatingSourceCode As String,
                                                Optional ByVal TemplateID As String = "") As String
        'Create the email xml document
        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor, ndSettings, ndFromAddress, ndToAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort,
            ndAttachments, ndPartner, ndParameters, ndPaymentReference, ndOriginatingSourceCode, ndCustomer, ndTemplateID As XmlNode

        Try
            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndFromAddress = .CreateElement("FromAddress")
                ndToAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndPaymentReference = .CreateElement("PaymentReference")
                ndOriginatingSourceCode = .CreateElement("OriginatingSourceCode")
                ndCustomer = .CreateElement("Customer")
            End With

            'Populate the values
            ndFromAddress.InnerText = fromAddress
            ndToAddress.InnerText = toAddress
            ndCCAddress.InnerText = ccAddress
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = attachments
            ndPaymentReference.InnerText = paymentRef
            ndOriginatingSourceCode.InnerText = originatingSourceCode
            ndCustomer.InnerText = customer
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID

            'Add the settings value
            With ndSettings
                .AppendChild(ndFromAddress)
                .AppendChild(ndToAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndPaymentReference)
                .AppendChild(ndCustomer)
                .AppendChild(ndOriginatingSourceCode)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)

        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function
    Public Function CreateTicketingXmlDocument(ByVal fromAddress As String,
                                                ByVal toAddress As String,
                                                ByVal ccAddress As String,
                                                ByVal smtpServer As String,
                                                ByVal smtpServerPort As String,
                                                ByVal Partner As String,
                                                ByVal attachments As String,
                                                ByVal paymentRef As String,
                                                ByVal customer As String,
                                                ByVal originatorSourceCode As String,
                                                Optional ByVal TemplateID As String = "") As String
        'Create the email xml document
        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor, ndSettings, ndFromAddress, ndToAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort,
            ndAttachments, ndPartner, ndParameters, ndPaymentReference, ndOriginatorSourceCode, ndCustomer, ndTemplateID As XmlNode

        Try
            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndFromAddress = .CreateElement("FromAddress")
                ndToAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")

                ndParameters = .CreateElement("Parameters")
                ndPaymentReference = .CreateElement("PaymentReference")
                ndOriginatorSourceCode = .CreateElement("OriginatorSourceCode")
                ndCustomer = .CreateElement("Customer")
            End With

            'Populate the values
            ndFromAddress.InnerText = fromAddress
            ndToAddress.InnerText = toAddress
            ndCCAddress.InnerText = ccAddress
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = attachments
            ndPaymentReference.InnerText = paymentRef
            ndOriginatorSourceCode.InnerText = originatorSourceCode
            ndCustomer.InnerText = customer
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = TemplateID

            'Add the settings value
            With ndSettings
                .AppendChild(ndFromAddress)
                .AppendChild(ndToAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With

            'Add the paramaters
            With ndParameters
                .AppendChild(ndPaymentReference)
                .AppendChild(ndCustomer)
                .AppendChild(ndOriginatorSourceCode)
            End With

            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)

        Catch ex As Exception
        End Try

        Return Utilities.GetXmlDocAsString(xmlDoc)

    End Function

    Private Function OrderRetailDetails(ByVal OrderID As String, ByVal HtmlFormat As Boolean, ByVal RetailOrderConfirmationTableHeader As String, ByVal RetailOrderConfirmationTableRow As String, ByRef RetailFeeDescription As String, ByRef RetailFee As String) As String
        Dim finalResult As String = String.Empty

        finalResult = RetailOrderConfirmationTableHeader

        Dim orderLines As DataTable = TDataObjects.OrderSettings.GetOrderLinesByOrderID(OrderID)
        If orderLines.Rows.Count > 0 Then
            For Each line As DataRow In orderLines.Rows
                finalResult += ReplaceRetailTableRowContentFields(RetailOrderConfirmationTableRow, line("PRODUCT_DESCRIPTION_1"),
                                                                 CDec(line("QUANTITY")).ToString("F0"), line("PRODUCT_CODE"), CDec(line("LINE_PRICE_GROSS")).ToString("F2"),
                                                                 line("INSTRUCTIONS"))
                If String.IsNullOrEmpty(RetailFee) Then
                    RetailFee = CDec(Utilities.CheckForDBNull_String(line("TOTAL_DELIVERY_GROSS"))).ToString("F2")
                End If
                If String.IsNullOrEmpty(RetailFeeDescription) Then
                    RetailFeeDescription = Utilities.CheckForDBNull_String(line("DELIVERY_TYPE_DESCRIPTION"))
                End If
            Next
        Else
            Return String.Empty
        End If

        If Not String.IsNullOrEmpty(finalResult) Then
            finalResult += "</table>"
        End If

        Return finalResult
    End Function

    Private Function FeeOrderRetail(ByVal retailFee As String, ByVal retailFeeDescription As String, ByVal RetailOrderFeeTableRow As String) As String
        If Not String.IsNullOrEmpty(retailFee) Then
            Return String.Format(RetailOrderFeeTableRow,
                                             retailFee,
                                             retailFeeDescription)
        Else
            Return String.Empty
        End If
    End Function
    Public Function SendTicketingConfirmationEmail() As ErrorObj

        Dim err As New ErrorObj
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_TICKETING_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_ORDER_CONFIRMATION, err)
            If err.HasError Then Return err

            '----------------------------------------------------------------------------------
            '   Load info etc required by the Email_Send function
            Dim iCounter As Integer = 0
            '------------------------------------------------------------------------------------------------
            '
            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()

            'Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML"))
            Dim HtmlFormat As Boolean = _emailHTML
            Dim strSubject As String = _emailSubject
            Dim strMessage As String = String.Empty

            If Not _emailNoDataRetrieval Then

                '----------------------------------------------------
                'Load the order the backend
                '----------------------------------------------------
                Dim order As New Talent.Common.TalentOrder
                Dim deTicketingItemDetails As New Talent.Common.DETicketingItemDetails
                With deTicketingItemDetails
                    .PaymentReference = EmailSettings.OrderConfirmation.PaymentReference
                    .UnprintedRecords = ""
                    .SetAsPrinted = "N"
                    .EndOfSale = "Y"
                    .RetryFailure = RetryFailure
                    .Src = Settings.OriginatingSourceCode
                    .BusinessUnit = Settings.BusinessUnit
                End With
                order.Settings = Settings
                order.Settings.Cacheing = False
                order.Dep.CollDEOrders.Add(deTicketingItemDetails)
                err = order.OrderDetails

                If Not err.HasError AndAlso Not order.ResultDataSet Is Nothing AndAlso order.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-08"
                        err.ErrorMessage = String.Format("Error Retrieving the order details. payment Reference : {0}, Return Code : {1}",
                                                          EmailSettings.OrderConfirmation.PaymentReference, order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
                    Else

                        ' Get bulk seats 
                        If order.ResultDataSet.Tables("PurchaseResults").Rows.Count > 0 AndAlso order.ResultDataSet.Tables("PurchaseResults").Rows(0).Item("BulkID") > "0" Then
                            Dim BulkorderSeats As New Talent.Common.TalentOrder
                            BulkorderSeats.Settings = Settings
                            BulkorderSeats.Settings.Cacheing = False
                            BulkorderSeats.Dep.CollDEOrders.Add(deTicketingItemDetails)
                            err = BulkorderSeats.GetBulkSeats
                            If Not err.HasError Then
                                If BulkorderSeats.ResultDataSet.Tables("BulkSeats").Rows.Count > 0 Then
                                    _dtBulkTickets = BulkorderSeats.ResultDataSet.Tables("BulkSeats")
                                Else
                                    err.HasError = True
                                    err.ErrorNumber = "TACTEMAIL-15"
                                    err.ErrorMessage = String.Format("Error Retrieving the bulk seat details. payment Reference : {0}, Return Code : {1}", EmailSettings.OrderConfirmation.PaymentReference, order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
                                End If

                            End If
                        End If


                        Dim ds As Data.DataSet = order.ResultDataSet
                        If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then
                            _emailTemplateID = ds.Tables(0).Rows(0)("EMailTemplateID")
                            With ucr
                                .BusinessUnit = Settings.BusinessUnit
                                .FrontEndConnectionString = Settings.FrontEndConnectionString
                                '                .KeyCode = "OrderConfirmEmail.vb"
                                .PageCode = ""
                                .PartnerCode = Settings.Partner
                                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
                            End With
                            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_ORDER_CONFIRMATION, err)
                            HtmlFormat = _emailHTML
                            strSubject = _emailSubject
                            PopulateUcrDictionary(HtmlFormat)

                            '------------------------------------------------------------------------------------------------
                            'Dim strSubject As String = ucr.Content("EmailSubject", Settings.Language, False)
                            Dim OrderConfirmationTableHeader As String = ucr.Content("OrderConfirmationTableHeader", Settings.Language, False)
                            Dim OrderCorporateTableHeader As String = ucr.Content("OrderCorporateTableHeader", Settings.Language, False)
                            Dim OrderConfirmationTableRow As String = ucr.Content("OrderConfirmationTableRow", Settings.Language, False)
                            Dim OrderPackageBundleTableHeader As String = ucr.Content("OrderPackageBundleTableHeader", Settings.Language, False)
                            Dim OrderPackageBundleTableRow As String = ucr.Content("OrderPackageBundleTableRow", Settings.Language, False)

                            Dim RetailOrderConfirmationTableHeader As String = ucr.Content("RetailOrderConfirmationTableHeader", Settings.Language, False)
                            Dim RetailOrderConfirmationTableRow As String = ucr.Content("RetailOrderConfirmationTableRow", Settings.Language, False)
                            Dim RetailOrderFeeTableRow As String = ucr.Content("RetailOrderFeeTableRow", Settings.Language, False)

                            Dim OrderCorporateTableRow As String = ucr.Content("OrderCorporateTableRow", Settings.Language, False)
                            Dim orderConfirmationSeparatorList(8) As String
                            orderConfirmationSeparatorList(0) = ucr.Content("OrderConfirmationSeatSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(1) = ucr.Content("orderConfirmationAlphaSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(2) = ucr.Content("orderConfirmationRowSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(3) = ucr.Content("orderConfirmationStandSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(4) = ucr.Content("orderConfirmationAreaSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(5) = ucr.Content("orderConfirmationTurnStyleSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(6) = ucr.Content("orderConfirmationGateSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(7) = ucr.Content("orderConfirmationSeatRestrictionCodeSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(8) = ucr.Content("orderConfirmationSeatRestrictionTextSeparator", Settings.Language, False)

                            '------------------------------------------------------------------------------------------------
                            '   get details and data
                            '
                            err = OrderHeaderTicketingValues()

                            'Retrieve User Details
                            Dim DeCust As New DECustomer()
                            DeCust.CustomerNumber = EmailSettings.OrderConfirmation.Customer
                            DeCust.UserName = EmailSettings.OrderConfirmation.Customer

                            Dim deCustV11 As New DECustomerV11()
                            deCustV11.DECustomersV1.Add(DeCust)

                            Dim tc As New TalentCustomer()
                            With tc
                                .DeV11 = deCustV11
                                .Settings = Settings
                                err = .CustomerRetrieval()
                            End With

                            Dim tempOrderId As String = ds.Tables(0).Rows(0)("TempOrderId")


                            If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                                If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                                    err.HasError = True
                                    err.ErrorNumber = "TACTEMAIL-11"
                                    err.ErrorMessage = String.Format("Error Retrieving the customer details. customer Number : {0}, Return Code : {1}",
                                                                      EmailSettings.OrderConfirmation.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                                Else

                                    Dim drUser As DataRow = tc.ResultDataSet.Tables(1).Rows(0)
                                    If HtmlFormat Then
                                        strData(0) = "<br />"
                                    Else
                                        strData(0) = vbCrLf
                                    End If
                                    strData(1) = Utilities.CheckForDBNull_String(ds.Tables(0).Rows(0)("PaymentReference"))
                                    strData(2) = EmailSettings.OrderConfirmation.Customer
                                    strData(6) = drUser("ContactForename").ToString().Trim() & " " & drUser("ContactSurname").ToString().Trim()
                                    strData(14) = drUser("HomeTelephoneNumber").ToString().Trim()
                                    strData(15) = drUser("EmailAddress").ToString().Trim()

                                    strData(7) = drUser("AddressLine1").ToString().Trim()
                                    strData(8) = drUser("AddressLine2").ToString().Trim()
                                    strData(9) = drUser("AddressLine3").ToString().Trim()
                                    strData(10) = drUser("AddressLine4").ToString().Trim()
                                    strData(11) = drUser("AddressLine5").ToString().Trim()
                                    strData(12) = drUser("PostCode").ToString().Trim()

                                    Dim deliveryDR As Data.DataRow = order.ResultDataSet.Tables("StatusResults").Rows(0)
                                    If Utilities.CheckForDBNull_String(deliveryDR("deliveryContactName")).Trim.Length > 0 _
                                        Or Utilities.CheckForDBNull_String(deliveryDR("deliveryAddress1")).Trim.Length > 0 Then
                                        strData(6) = Utilities.CheckForDBNull_String(deliveryDR("deliveryContactName")).Trim
                                        strData(7) = Utilities.CheckForDBNull_String(deliveryDR("deliveryAddress1")).Trim
                                        strData(8) = Utilities.CheckForDBNull_String(deliveryDR("deliveryAddress2")).Trim
                                        strData(9) = Utilities.CheckForDBNull_String(deliveryDR("deliveryAddress3")).Trim
                                        strData(10) = Utilities.CheckForDBNull_String(deliveryDR("deliveryAddress4")).Trim
                                        strData(11) = Utilities.CheckForDBNull_String(deliveryDR("deliveryAddress5")).Trim
                                        strData(12) = Utilities.CheckForDBNull_String(deliveryDR("deliveryPostCode")).Trim
                                    End If


                                    'Order Value Settings
                                    strData(58) = ""
                                    strData(59) = ""
                                    strData(57) = Utilities.CheckForDBNull_Decimal(ds.Tables(0).Rows(0)("TotalPrice"))

                                    strData(58) = getPriceWithFormattedCurrency(ds.Tables(0).Rows(0)("TotalPrice"))
                                    strData(59) = getPriceWithFormattedCurrency(ds.Tables(0).Rows(0)("TotalPrice"))

                                    strData(60) = drUser("ContactTitle").ToString().Trim()
                                    strData(61) = drUser("ContactForename").ToString().Trim()
                                    strData(62) = drUser("ContactSurname").ToString().Trim()

                                    ' Build text if on-account spent or refunded   
                                    Dim OnAccountAmount As Decimal = ds.Tables(0).Rows(0)("OnAccountAmount")
                                    If OnAccountAmount <> 0 Then
                                        If ds.Tables(0).Rows(0)("OnAccountRefunded") = True Then
                                            strData(63) = Web.HttpUtility.HtmlEncode(getPriceWithFormattedCurrency(OnAccountAmount) & " " & ucr.Content("OnAccountRefundText", Settings.Language, False))
                                        Else
                                            strData(63) = Web.HttpUtility.HtmlEncode(getPriceWithFormattedCurrency(OnAccountAmount) & " " & ucr.Content("OnAccountSpendText", Settings.Language, False))
                                        End If
                                    End If
                                    '----------------------------------------------------------------------------------
                                    '   Join all the paragraphs together
                                    '
                                    For iCounter = 0 To 10
                                        If HtmlFormat Then
                                            'The <br> tage can be added manually for better formatting
                                            strMessage &= strCode(iCounter)
                                        Else
                                            strMessage &= strCode(iCounter) & vbCrLf
                                        End If
                                    Next

                                    'Check content is exists in strCode array if not then get it from EmailMessage
                                    'Mostly EmailMessage content is used for retail confirmation and 
                                    'StrCode array content used for ticketing confirmation
                                    If strMessage.Trim.Length = 0 Then
                                        strMessage = ucr.Content("EmailMessage", Settings.Language, False)
                                    End If

                                    strMessage = Replace(strMessage, "<<<email>>>", EmailSettings.ToAddress)

                                    ' Payment type text
                                    strMessage = Replace(strMessage, "<<<PaymentTypeText>>>", paymentMethodText(ds))

                                    ' Product type text
                                    strMessage = Replace(strMessage, "<<<ProductTypeText>>>", productTypeText(ds))

                                    ' Ticket Exchange type text
                                    strMessage = Replace(strMessage, "<<<TicketExchangeText>>>", ticketExchangeText(ds))

                                    'Product specific text
                                    strMessage = Replace(strMessage, "<<<ProductSpecificText>>>", ProductSpecificText(ds))

                                    'Add Payment Reference as Image
                                    strMessage = Replace(strMessage, "<<<PaymentRefAsImage>>>", getPaymentReferenceAsImage(EmailSettings.OrderConfirmation.PaymentReference))

                                    ' Add the order lines
                                    'Total Items
                                    Dim totalOrderDetails As String = String.Empty
                                    Dim RetailFeeDescription As String = String.Empty
                                    Dim RetailFee As String = String.Empty



                                    'Ticketing Items
                                    totalOrderDetails += OrderTicketingDetails(ds.Tables(1).Select("FEETYPE <> 'Y' AND CALLREFERENCE = 0 AND RelatingBundleProduct = '' AND PACKAGEPART = 'False'"),
                                                                              HtmlFormat, OrderConfirmationTableRow, orderConfirmationSeparatorList, OrderConfirmationTableHeader, err).Trim()

                                    'Retail Items
                                    totalOrderDetails += OrderRetailDetails(tempOrderId, HtmlFormat, RetailOrderConfirmationTableHeader, RetailOrderConfirmationTableRow, RetailFee, RetailFeeDescription)

                                    'Bundle Products
                                    Dim keyList As Generic.List(Of String) = GetBundleProductKeys(ds.Tables(1).Select("FeeType <> 'Y' AND CallReference = 0 AND RelatingBundleProduct <> ''"))
                                    If keyList.Count > 0 Then
                                        For Each item As String In keyList
                                            Dim key() As String = item.Split(",")
                                            Dim relatingBundleProduct As String = key(0)
                                            Dim relatingBundleSeat As String = key(1)
                                            Dim selectString As New StringBuilder
                                            selectString.Append("FeeType <> 'Y' AND CallReference = 0 AND PACKAGEPART = 'False' AND RelatingBundleProduct = '").Append(relatingBundleProduct).Append("' ")
                                            selectString.Append("AND RelatingBundleSeat = '").Append(relatingBundleSeat).Append("'")
                                            Dim headerString As String = _ucrDictionary.Item("BundleSeatsHeader")
                                            totalOrderDetails += headerString.Replace("<<RELATING_BUNDLE_PRODUCT>>", GetProductDescriptionFromProductCode(ds.Tables(1), relatingBundleProduct)).Replace("<<SEAT>>", relatingBundleSeat)
                                            totalOrderDetails += OrderTicketingDetails(ds.Tables(1).Select(selectString.ToString()), HtmlFormat, OrderConfirmationTableRow, orderConfirmationSeparatorList, OrderConfirmationTableHeader, err).Trim()
                                        Next
                                    End If




                                    'Corporate Items
                                    totalOrderDetails += OrderCorporateDetails(ds.Tables(1).Select("CALLREFERENCE > 0  AND PACKAGEPART = 'False'"), HtmlFormat, OrderCorporateTableHeader, OrderCorporateTableRow)

                                    'f1 sale items
                                    totalOrderDetails += OrderBundlePackageDetails(ds.Tables(1).Select("CALLREFERENCE > 0 AND PACKAGEPART = 'True'"), HtmlFormat, OrderPackageBundleTableHeader, OrderPackageBundleTableRow, order)
                                    'Fee Items
                                    totalOrderDetails += OrderFeesDetails(ds.Tables(1).Select("FEETYPE = 'Y'"), HtmlFormat, RetailOrderFeeTableRow, RetailFee, RetailFeeDescription)

                                    strMessage = Replace(strMessage, "<<<OrderItems>>>", totalOrderDetails, )

                                    'Add the direct debit summary
                                    If ds.Tables(0).Rows(0).Item("PaymentMethod").ToString = "DD" Then 'Was this a direct debit sale
                                        strMessage = Replace(strMessage, "<<<DirectDebitSummary>>>", DirectDebitSchedule(ds, HtmlFormat))
                                    Else
                                        strMessage = Replace(strMessage, "<<<DirectDebitSummary>>>", String.Empty)
                                    End If


                                    '   Replace the field delimiters with data

                                    For iCounter = 0 To 100
                                        If strField(iCounter) > Nothing Then
                                            strSubject = Replace(strSubject, strField(iCounter), strData(iCounter))
                                            strMessage = Replace(strMessage, strField(iCounter), strData(iCounter))
                                        End If
                                    Next
                                    Dim purchaseDetails As DataTable = ds.Tables("PurchaseResults")
                                    Dim attachmentFileAndPath As String = String.Empty
                                    If String.IsNullOrEmpty(tempOrderId) AndAlso purchaseDetails.Rows.Count > 0 Then
                                        If purchaseDetails.Rows.Count > 0 And _ticketPath.Length > 0 Then
                                            Dim ticketDesignTicket As New TicketDesignerTicket(TicketType.PDF)
                                            ticketDesignTicket.Settings = Settings
                                            ticketDesignTicket.TicketPath = _ticketPath
                                            err = ticketDesignTicket.CreateTicket(purchaseDetails, EmailSettings.OrderConfirmation.PaymentReference)
                                            If Not err.HasError Then
                                                'filename exists if any required product type exist 
                                                If ticketDesignTicket.TicketFileName.Length > 0 Then
                                                    attachmentFileAndPath = _ticketPath & ticketDesignTicket.TicketFileName
                                                End If
                                            End If
                                            ticketDesignTicket = Nothing
                                        Else
                                            err.HasError = True
                                            err.ErrorNumber = "TACTEMAIL-01"
                                            If String.IsNullOrEmpty(tempOrderId) Then
                                                err.ErrorMessage = "TempOrderID is empty or ticket target path is empty"
                                            Else
                                                err.ErrorMessage = "PurchaseResult from Order details is empty or ticket target path is empty"
                                            End If
                                        End If
                                    End If

                                    If EmailSettings.Attachments.Count > 0 Then
                                        If attachmentFileAndPath.Length > 0 Then attachmentFileAndPath &= ";"
                                        Dim i As Integer = 0
                                        For Each attachment As String In EmailSettings.Attachments
                                            If i > 0 Then attachmentFileAndPath &= ";"
                                            attachmentFileAndPath &= attachment
                                            i += 1
                                        Next
                                    End If


                                    'Perform the send operation if there is no error
                                    If Not err.HasError Then
                                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, strSubject, strMessage, attachmentFileAndPath, False, HtmlFormat)
                                    End If
                                End If

                            Else
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-08a"
                                err.ErrorMessage = String.Format("Error Retrieving customer details. Customer Number : {0}",
                                                                  EmailSettings.OrderConfirmation.Customer)
                            End If
                        End If
                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-08"
                    err.ErrorMessage = String.Format("Error Retrieving the order details. Payment Reference : {0}",
                                                      EmailSettings.OrderConfirmation.PaymentReference)
                End If
            Else
                strMessage = _emailBody
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, strSubject, RemoveMergeBrackets(strMessage), "", False, HtmlFormat)
            End If

        End If

        Return err
    End Function

    Public Function SendTicketingCancelAmmendConfirmationEmail(ByVal emailType As String) As ErrorObj

        'Sends emails TicketingUpgrade, TIcketingTransfer and TicketingCancel


        Dim err As New ErrorObj
        AddClientAddressToOrderConfirmation(Utilities.CheckForDBNull_Boolean_DefaultFalse(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_TKT_CAT_CONFIRMATION_TO_CUSTOMER_SERVICES")))
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If

        If Not err.HasError Then

            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()

            ' Retrieve email template details
            Select Case emailType
                Case Is = "TicketingCancel"
                    RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_TICKETING_CANCEL, err)
                Case Is = "TicketingTransfer"
                    RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_TICKETING_TRANSFER, err)
                Case Is = "TicketingUpgrade"
                    RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_TICKETING_UPGRADE, err)
            End Select
            If err.HasError Then Return err

            '----------------------------------------------------------------------------------
            '   Load info etc required by the Email_Send function

            Dim iCounter As Integer = 0
            '------------------------------------------------------------------------------------------------
            '
            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()

            '  Get content common to all confirmation emails (placeholder fields, separators etc)
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                '                .KeyCode = "OrderConfirmEmail.vb"
                .PageCode = ""
                .PartnerCode = Settings.Partner
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With

            '            Dim HtmlFormat As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEND_ORDER_CONFIRMATION_EMAIL_AS_HTML"))
            Dim HtmlFormat As Boolean = _emailHTML
            Dim strSubject As String = _emailSubject
            Dim strMessage As String = String.Empty

            If Not _emailNoDataRetrieval Then
                '----------------------------------------------------
                'Load the order the backend
                '----------------------------------------------------
                Dim order As New Talent.Common.TalentOrder
                Dim deTicketingItemDetails As New Talent.Common.DETicketingItemDetails
                With deTicketingItemDetails
                    .PaymentReference = EmailSettings.OrderConfirmation.PaymentReference
                    .UnprintedRecords = ""
                    .SetAsPrinted = "N"
                    .EndOfSale = "Y"
                    .RetryFailure = RetryFailure
                    .Src = Settings.OriginatingSourceCode
                    Select Case emailType
                        Case Is = "TicketingCancel"
                            .RequestType = "4"
                        Case Is = "TicketingTransfer"
                            .RequestType = "5"
                        Case Is = "TicketingUpgrade"
                            .RequestType = "6"
                    End Select
                End With

                order.Settings = Settings
                order.Settings.Cacheing = False
                order.Dep.CollDEOrders.Add(deTicketingItemDetails)
                err = order.OrderDetails

                If Not err.HasError AndAlso Not order.ResultDataSet Is Nothing AndAlso order.ResultDataSet.Tables(0).Rows.Count > 0 Then

                    If order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-08"
                        err.ErrorMessage = String.Format("Error Retrieving the order details. payment Reference : {0}, Return Code : {1}",
                                                          EmailSettings.OrderConfirmation.PaymentReference, order.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode"))
                    Else

                        Dim ds As Data.DataSet = order.ResultDataSet
                        If ds.Tables.Count > 0 AndAlso ds.Tables(0).Rows.Count > 0 Then

                            PopulateUcrDictionary(HtmlFormat)

                            '------------------------------------------------------------------------------------------------
                            'Dim strSubject As String = ucr.Content("EmailSubject", Settings.Language, False)
                            'Select Case emailType
                            '    Case Is = "TicketingCancel"
                            '        strSubject = ucr.Content("EmailSubjectTicketingCancel", Settings.Language, False)
                            '    Case Is = "TicketingTransfer"
                            '        strSubject = ucr.Content("EmailSubjectTicketingTransfer", Settings.Language, False)
                            '    Case Is = "TicketingUpgrade"
                            '        strSubject = ucr.Content("EmailSubjectTicketingUpgrade", Settings.Language, False)
                            'End Select
                            'Dim strMessage As String = String.Empty
                            Dim OrderConfirmationTableHeader As String = ucr.Content("OrderConfirmationTableHeader", Settings.Language, False)
                            Dim OrderCorporateTableHeader As String = ucr.Content("OrderCorporateTableHeader", Settings.Language, False)
                            Dim OrderConfirmationTableRow As String = ucr.Content("OrderConfirmationTableRow", Settings.Language, False)
                            Dim OrderCorporateTableRow As String = ucr.Content("OrderCorporateTableRow", Settings.Language, False)

                            Dim OrderPackageBundleTableHeader As String = ucr.Content("OrderPackageBundleTableHeader", Settings.Language, False)
                            Dim OrderPackageBundleTableRow As String = ucr.Content("OrderPackageBundleTableRow", Settings.Language, False)

                            Dim orderConfirmationSeparatorList(8) As String
                            orderConfirmationSeparatorList(0) = ucr.Content("OrderConfirmationSeatSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(1) = ucr.Content("orderConfirmationAlphaSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(2) = ucr.Content("orderConfirmationRowSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(3) = ucr.Content("orderConfirmationStandSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(4) = ucr.Content("orderConfirmationAreaSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(5) = ucr.Content("orderConfirmationTurnStyleSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(6) = ucr.Content("orderConfirmationGateSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(7) = ucr.Content("orderConfirmationSeatRestrictionCodeSeparator", Settings.Language, False)
                            orderConfirmationSeparatorList(8) = ucr.Content("orderConfirmationSeatRestrictionTextSeparator", Settings.Language, False)

                            '------------------------------------------------------------------------------------------------
                            '   get details and data

                            err = OrderHeaderTicketingValues()

                            'Retrieve User Details
                            Dim DeCust As New DECustomer()
                            DeCust.CustomerNumber = EmailSettings.OrderConfirmation.Customer
                            DeCust.UserName = EmailSettings.OrderConfirmation.Customer

                            Dim deCustV11 As New DECustomerV11()
                            deCustV11.DECustomersV1.Add(DeCust)

                            Dim tc As New TalentCustomer()
                            With tc
                                .DeV11 = deCustV11
                                .Settings = Settings
                                err = .CustomerRetrieval()
                            End With

                            If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then

                                If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                                    err.HasError = True
                                    err.ErrorNumber = "TACTEMAIL-11"
                                    err.ErrorMessage = String.Format("Error Retrieving the customer details. customer Number : {0}, Return Code : {1}",
                                                                      EmailSettings.OrderConfirmation.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                                Else

                                    Dim drUser As DataRow = tc.ResultDataSet.Tables(1).Rows(0)
                                    If HtmlFormat Then
                                        strData(0) = "<br />"
                                    Else
                                        strData(0) = vbCrLf
                                    End If
                                    strData(1) = Utilities.CheckForDBNull_String(ds.Tables(0).Rows(0)("PaymentReference"))
                                    strData(2) = EmailSettings.OrderConfirmation.Customer
                                    strData(6) = drUser("ContactForename").ToString().Trim() & " " & drUser("ContactSurname").ToString().Trim()
                                    strData(14) = drUser("HomeTelephoneNumber").ToString().Trim()
                                    strData(15) = drUser("EmailAddress").ToString().Trim()

                                    strData(7) = drUser("AddressLine1").ToString().Trim()
                                    strData(8) = drUser("AddressLine2").ToString().Trim()
                                    strData(9) = drUser("AddressLine3").ToString().Trim()
                                    strData(10) = drUser("AddressLine4").ToString().Trim()
                                    strData(11) = drUser("AddressLine5").ToString().Trim()
                                    strData(12) = drUser("PostCode").ToString().Trim()

                                    'Order Value Settings
                                    strData(58) = ""
                                    strData(59) = ""
                                    strData(57) = Utilities.CheckForDBNull_Decimal(ds.Tables(0).Rows(0)("TotalPrice"))

                                    strData(58) = getPriceWithFormattedCurrency(ds.Tables(0).Rows(0)("TotalPrice"))
                                    strData(59) = getPriceWithFormattedCurrency(ds.Tables(0).Rows(0)("TotalPrice"))

                                    strData(60) = drUser("ContactTitle").ToString().Trim()
                                    strData(61) = drUser("ContactForename").ToString().Trim()
                                    strData(62) = drUser("ContactSurname").ToString().Trim()

                                    ' If a refund is due put some text to indicate a refund after the amount 
                                    If strData(57) Is Nothing OrElse Utilities.CheckForDBNull_Decimal(ds.Tables(0).Rows(0)("TotalPrice")) = 0 Then
                                        strData(57) = Utilities.CheckForDBNull_Decimal(ds.Tables(0).Rows(0)("RefundAmount"))
                                        strData(57) = strData(57).Trim + " " + ucr.Content("RefundText", Settings.Language, False)
                                    End If

                                    ' Build text if on-account spent or refunded for <<<SpendorRefundOnAccountMessage>>>
                                    Dim OnAccountAmount As Decimal = ds.Tables(0).Rows(0)("OnAccountAmount")
                                    If OnAccountAmount <> 0 Then
                                        If ds.Tables(0).Rows(0)("OnAccountRefunded") = True Then
                                            strData(63) = Web.HttpUtility.HtmlEncode(getPriceWithFormattedCurrency(OnAccountAmount) & " " & ucr.Content("OnAccountRefundText", Settings.Language, False))
                                        Else
                                            strData(63) = Web.HttpUtility.HtmlEncode(getPriceWithFormattedCurrency(OnAccountAmount) & " " & ucr.Content("OnAccountSpendText", Settings.Language, False))
                                        End If
                                    End If

                                    '----------------------------------------------------------------------------------
                                    '   Join all the paragraphs together
                                    '
                                    For iCounter = 0 To 10
                                        If HtmlFormat Then
                                            'The <br> tage can be added manually for better formatting
                                            strMessage &= strCode(iCounter)
                                        Else
                                            strMessage &= strCode(iCounter) & vbCrLf
                                        End If
                                    Next

                                    'Check content is exists in strCode array if not then get it from EmailMessage
                                    'Mostly EmailMessage content is used for retail confirmation and 
                                    'StrCode array content used for ticketing confirmation
                                    ''If strMessage.Trim.Length = 0 Then
                                    ''    strMessage = ucr.Content("EmailMessage", Settings.Language, False)
                                    ''End If

                                    strMessage = Replace(strMessage, "<<<email>>>", EmailSettings.ToAddress)
                                    '----------------------------------------------------------------------------------
                                    ' Payment type text
                                    strMessage = Replace(strMessage, "<<<PaymentTypeText>>>", paymentMethodText(ds))
                                    '----------------------------------------------------------------------------------
                                    ' Product type text
                                    strMessage = Replace(strMessage, "<<<ProductTypeText>>>", productTypeText(ds))
                                    '----------------------------------------------------------------------------------
                                    'Product specific text
                                    strMessage = Replace(strMessage, "<<<ProductSpecificText>>>", ProductSpecificText(ds))

                                    'Product specific text
                                    strMessage = Replace(strMessage, "<<<ProductSpecificText>>>", ProductSpecificText(ds))
                                    '----------------------------------------------------------------------------------

                                    'Add Payment Reference as Image
                                    strMessage = Replace(strMessage, "<<<PaymentRefAsImage>>>", getPaymentReferenceAsImage(EmailSettings.OrderConfirmation.PaymentReference))

                                    ' Add the order lines
                                    'Total Items
                                    Dim totalOrderDetails As String = String.Empty

                                    'Ticketing Items
                                    totalOrderDetails = OrderTicketingDetails(ds.Tables(1).Select("FEETYPE <> 'Y' AND CALLREFERENCE = 0 AND PACKAGEPART = 'False'"),
                                                                              HtmlFormat,
                                                                              OrderConfirmationTableRow,
                                                                              orderConfirmationSeparatorList,
                                                                              OrderConfirmationTableHeader, err).Trim()

                                    'Corporate Items
                                    totalOrderDetails += OrderCorporateDetails(ds.Tables(1).Select("CALLREFERENCE > 0 AND PACKAGEPART = 'False'"), HtmlFormat, OrderCorporateTableHeader, OrderCorporateTableRow)

                                    'F1 items
                                    totalOrderDetails += OrderBundlePackageDetails(ds.Tables(1).Select("CALLREFERENCE > 0 AND PACKAGEPART = 'True'"), HtmlFormat, OrderPackageBundleTableHeader, OrderPackageBundleTableRow, order)

                                    'Fee Items
                                    'totalOrderDetails += OrderFeesDetails(ds.Tables(1).Select("FEETYPE = 'Y'"), HtmlFormat)

                                    strMessage = Replace(strMessage, "<<<OrderItems>>>", totalOrderDetails)

                                    '----------------------------------------------------------------------------------
                                    ' Add the direct debit summary
                                    strMessage = Replace(strMessage, "<<<DirectDebitSummary>>>", DirectDebitSchedule(ds, HtmlFormat))
                                    '----------------------------------------------------------------------------------
                                    '   Replace the field delimiters with data
                                    '
                                    For iCounter = 0 To 100
                                        If strField(iCounter) > Nothing Then
                                            strSubject = Replace(strSubject, strField(iCounter), strData(iCounter))
                                            strMessage = Replace(strMessage, strField(iCounter), strData(iCounter))
                                        End If
                                    Next


                                    Dim attachmentFileAndPath As String = String.Empty
                                    Dim purchaseDetails As DataTable = ds.Tables("PurchaseResults")
                                    If purchaseDetails.Rows.Count > 0 And _ticketPath.Length > 0 Then
                                        Dim ticketDesignTicket As New TicketDesignerTicket(TicketType.PDF)
                                        ticketDesignTicket.Settings = Settings
                                        ticketDesignTicket.TicketPath = _ticketPath
                                        err = ticketDesignTicket.CreateTicket(purchaseDetails, EmailSettings.OrderConfirmation.PaymentReference)
                                        If Not err.HasError Then
                                            'filename exists if any required product type exist 
                                            If ticketDesignTicket.TicketFileName.Length > 0 Then
                                                attachmentFileAndPath = _ticketPath & ticketDesignTicket.TicketFileName
                                            End If
                                        End If
                                        ticketDesignTicket = Nothing
                                    Else
                                        err.HasError = True
                                        err.ErrorNumber = "TACTEMAIL-01"
                                        err.ErrorMessage = "PurchaseResult from Order details is empty or ticket target path is empty"
                                    End If

                                    If EmailSettings.Attachments.Count > 0 Then
                                        If attachmentFileAndPath.Length > 0 Then attachmentFileAndPath &= ";"
                                        Dim i As Integer = 0
                                        For Each attachment As String In EmailSettings.Attachments
                                            If i > 0 Then attachmentFileAndPath &= ";"
                                            attachmentFileAndPath &= attachment
                                            i += 1
                                        Next
                                    End If

                                    'Perform the send operation if there is no error
                                    If Not err.HasError Then
                                        Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                                        Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                                        err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, strSubject, strMessage, attachmentFileAndPath, False, HtmlFormat)
                                    End If
                                End If

                            Else
                                err.HasError = True
                                err.ErrorNumber = "TACTEMAIL-08a"
                                err.ErrorMessage = String.Format("Error Retrieving customer details. Customer Number : {0}",
                                                                  EmailSettings.OrderConfirmation.Customer)
                            End If
                        End If
                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-08"
                    err.ErrorMessage = String.Format("Error Retrieving the order details. Payment Reference : {0}",
                                                      EmailSettings.OrderConfirmation.PaymentReference)
                End If
            Else
                strMessage = _emailBody
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, strSubject, RemoveMergeBrackets(strMessage), "", False, HtmlFormat)
            End If
        End If

        Return err
    End Function

    ''' <summary>
    ''' Create XML document for QAndA reminder email
    ''' </summary>
    ''' <param name="fromAddress">Email address of sender</param>
    ''' <param name="toAddress">Email address of receiver</param>
    ''' <param name="smtpServer">SMTP server name</param>
    ''' <param name="smtpServerPort">SMTP server port</param>
    ''' <param name="Partner">Partner</param>
    ''' <param name="Customer">Customer number</param>
    ''' <param name="customerBookingUrl">Booking page url for customer</param>
    ''' <param name="callId">Booking reference id</param>
    ''' <param name="templateId">Template id of QAndA reminder request type</param>
    ''' <returns>XML document for QAndA reminder</returns>
    Public Function CreateHospitalityQAReminderXmlDocument(ByVal fromAddress As String,
                                                       ByVal toAddress As String,
                                                       ByVal smtpServer As String,
                                                       ByVal smtpServerPort As String,
                                                       ByVal Partner As String,
                                                       ByVal Customer As String,
                                                       ByVal customerBookingUrl As String,
                                                       ByVal callId As String,
                                                       ByVal templateId As String) As String

        Dim xmlDoc As New XmlDocument
        Dim ndEmailMonitor As XmlNode
        Dim ndSettings, ndfromAddress, ndtoAddress, ndCCAddress, ndSmtpServer, ndSmtpServerPort, ndAttachments, ndPartner As XmlNode
        Dim ndParameters, ndCustomer, ndBookingUrl, ndTemplateID, ndCallID As XmlNode
        Try
            'Create the xml nodes
            With xmlDoc
                ndEmailMonitor = .CreateElement("EmailMonitor")
                ndSettings = .CreateElement("Settings")
                ndfromAddress = .CreateElement("FromAddress")
                ndtoAddress = .CreateElement("ToAddress")
                ndCCAddress = .CreateElement("CCAddress")
                ndSmtpServer = .CreateElement("SmtpServer")
                ndSmtpServerPort = .CreateElement("SmtpServerPort")
                ndAttachments = .CreateElement("Attachments")
                ndPartner = .CreateElement("Partner")
                ndTemplateID = .CreateElement("TemplateID")
                ndParameters = .CreateElement("Parameters")
                ndCustomer = .CreateElement("Customer")
                ndBookingUrl = .CreateElement("BookingUrl")
                ndCallID = .CreateElement("CallId")
            End With
            'Populate the values
            ndfromAddress.InnerText = fromAddress
            ndtoAddress.InnerText = toAddress
            ndCCAddress.InnerText = ""
            ndSmtpServer.InnerText = smtpServer
            ndSmtpServerPort.InnerText = smtpServerPort
            ndAttachments.InnerText = ""
            ndPartner.InnerText = Partner
            ndTemplateID.InnerText = templateId
            ndCustomer.InnerText = Customer
            ndBookingUrl.InnerText = customerBookingUrl
            ndCallID.InnerText = callId
            'Add the settings value
            With ndSettings
                .AppendChild(ndfromAddress)
                .AppendChild(ndtoAddress)
                .AppendChild(ndCCAddress)
                .AppendChild(ndSmtpServer)
                .AppendChild(ndSmtpServerPort)
                .AppendChild(ndAttachments)
                .AppendChild(ndPartner)
                .AppendChild(ndTemplateID)
            End With
            'Add the paramaters
            With ndParameters
                .AppendChild(ndCustomer)
                .AppendChild(ndBookingUrl)
                .AppendChild(ndCallID)
            End With
            'Create the xml document
            With ndEmailMonitor
                .AppendChild(ndSettings)
                .AppendChild(ndParameters)
            End With
            xmlDoc.AppendChild(ndEmailMonitor)
        Catch ex As Exception
        End Try
        Return Utilities.GetXmlDocAsString(xmlDoc)
    End Function

    ''' <summary>
    ''' Send QAndA Reminder email
    ''' </summary>
    ''' <returns>Error object - if any error occured</returns>
    Public Function SendHospitalityQAndAReminder() As ErrorObj
        Dim err As New ErrorObj
        Dim HtmlFormat As Boolean = False
        Dim talPackage As New TalentPackage()
        Dim DoEmailAddressValidation As Boolean = Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DO_EMAIL_ADDRESS_VALIDATION"))
        If DoEmailAddressValidation Then
            Dim EmailAddressValidationLevel As String = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "EMAIL_ADDRESS_VALIDATION_LEVEL")
            err = ValidateEmailAddress(EmailSettings.ToAddress, EmailAddressValidationLevel)
        End If
        If Not err.HasError Then
            Settings.Language = Talent.Common.Utilities.GetDefaultLanguage()
            ' Retrieve email template details
            RetrieveEmailTemplateDetails(GlobalConstants.EMAIL_HOSPITALITY_Q_AND_A_REMINDER, err)
            If err.HasError Then Return err
            'The user control cache must be cleared when cache is not active. 
            If Not Utilities.IsCacheActive Then ClearHashTableCache()
            With ucr
                .BusinessUnit = Settings.BusinessUnit
                .PartnerCode = Settings.Partner
                .FrontEndConnectionString = Settings.FrontEndConnectionString
                .KeyCode = "TalentEmail.vb." + _emailTemplateID.ToString
            End With
            Dim emailSubject As String = _emailSubject
            HtmlFormat = _emailHTML
            Dim emailText As String = _emailBody
            If Not _emailNoDataRetrieval Then
                Dim DeCust As New DECustomer()
                DeCust.CustomerNumber = EmailSettings.HospitalityQAReminderEmail.Customer
                DeCust.UserName = EmailSettings.HospitalityQAReminderEmail.Customer
                Dim deCustV11 As New DECustomerV11()
                deCustV11.DECustomersV1.Add(DeCust)
                Dim tc As New TalentCustomer()
                With tc
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval()
                End With
                If Not err.HasError AndAlso Not tc.ResultDataSet Is Nothing AndAlso tc.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    If tc.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = "E" Then
                        err.HasError = True
                        err.ErrorNumber = "TACTEMAIL-19"
                        err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}, Return Code : {1}",
                                                          EmailSettings.HospitalityQAReminderEmail.Customer, tc.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                    End If
                    If Not err.HasError Then
                        err = retrieveBookings(EmailSettings.HospitalityQAReminderEmail.CallId, EmailSettings.HospitalityQAReminderEmail.Customer, Settings)
                        If Not err.HasError AndAlso Not ResultDataSet Is Nothing AndAlso ResultDataSet.Tables("Package").Rows.Count > 0 Then
                            Dim productCode As String = ResultDataSet.Tables("Component").Rows(0).Item("ProductCode")
                            If EmailSettings.HospitalityQAReminderEmail.BookingPageURL Is Nothing Or EmailSettings.HospitalityQAReminderEmail.BookingPageURL = String.Empty Then
                                EmailSettings.HospitalityQAReminderEmail.BookingPageURL = String.Concat("/PagesPublic/Hospitality/HospitalityBooking.aspx", "?callid=", EmailSettings.HospitalityQAReminderEmail.CallId, "&product=", productCode, "&status=", GlobalConstants.SOLD_BOOKING_STATUS)
                            End If
                            ResultDataSet = Nothing
                            emailText = Replace(emailText, " <<To>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                            emailText = Replace(emailText, "<<Title>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactTitle").ToString().Trim())
                            emailText = Replace(emailText, "<<Name>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactForename").ToString().Trim() & " " &
                                                                       tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())
                            emailText = Replace(emailText, "<<Surname>>", tc.ResultDataSet.Tables(1).Rows(0).Item("ContactSurname").ToString().Trim())
                            emailText = Replace(emailText, "<<BookingPageURL>>", EmailSettings.HospitalityQAReminderEmail.BookingPageURL)
                            emailText = Replace(emailText, "<<CustomerNumber>>", EmailSettings.HospitalityQAReminderEmail.Customer)
                            If HtmlFormat Then
                                emailText = Replace(emailText, "<<NewLine>>", "<br>")
                            Else
                                emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                            End If
                            Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                            Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                            err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                                     emailText, String.Empty, False, HtmlFormat)
                        Else
                            err.HasError = True
                            err.ErrorNumber = "TACTEMAIL-19a"
                            err.ErrorMessage = String.Format("Error Retrieving the booking details details. call Id : {0}",
                                                                  EmailSettings.HospitalityQAReminderEmail.CallId)
                        End If
                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "TACTEMAIL-19b"
                    err.ErrorMessage = String.Format("Error Retrieving the customer details. Customer Number : {0}",
                                                      EmailSettings.HospitalityQAReminderEmail.Customer)
                End If
            Else
                Talent.Common.Utilities.SMTP = EmailSettings.SmtpServer
                Talent.Common.Utilities.SMTPPortNumber = EmailSettings.SmtpServerPort
                err = Talent.Common.Utilities.Email_Send(EmailSettings.FromAddress, EmailSettings.ToAddress, emailSubject,
                                                         RemoveMergeBrackets(emailText), String.Empty, False, HtmlFormat)
            End If
        End If
        Return err
    End Function

    ''' <summary>
    ''' Retrieve booking based on customer number and booking reference id
    ''' </summary>
    ''' <param name="callId">Booking reference id</param>
    ''' <param name="customer">Customer number</param>
    ''' <param name="settings">Object of DESettings</param>
    ''' <returns>Error object - if any error occured</returns>
    Private Function retrieveBookings(ByVal callId As String, ByVal customer As String, ByVal settings As DESettings) As ErrorObj
        Dim talPackage As New TalentPackage()
        Dim err As New ErrorObj()
        talPackage.Settings = settings
        talPackage.DePackages.CallId = Convert.ToInt64(callId)
        talPackage.DePackages.Source = GlobalConstants.SOURCE
        err = talPackage.GetSoldHospitalityBookingDetails()
        ResultDataSet = talPackage.ResultDataSet
        Return err
    End Function

    Private Function OrderHeaderTicketingValues() As ErrorObj

        Dim err As New ErrorObj

        '----------------------------------------------------------------------------------
        '   Load header bits
        '
        With ucr
            ' ''------------------------------------------------------------------------------------------------
            ' ''    Allow up to ten paragraphs in the email
            ' ''
            ''strCode(0) = .Content("EmailMergeCode00", Settings.Language, True)
            ''strCode(1) = .Content("EmailMergeCode01", Settings.Language, True)          '  
            ''strCode(2) = .Content("EmailMergeCode02", Settings.Language, True)          ' 	 
            ''strCode(3) = .Content("EmailMergeCode03", Settings.Language, True)          ' 
            ''strCode(4) = .Content("EmailMergeCode04", Settings.Language, True)          ' 	Your items will be delivered to :
            ''strCode(5) = .Content("EmailMergeCode05", Settings.Language, True)          ' 	<<Address>> 
            ''strCode(6) = .Content("EmailMergeCode06", Settings.Language, True)          ' 	<<Lines>>
            ''strCode(7) = .Content("EmailMergeCode07", Settings.Language, True)          ' 	And have been confirmed and will be dispatched to you shortly.
            ''strCode(8) = .Content("EmailMergeCode08", Settings.Language, True)          ' 	If we can be of assistance please do not hesitate to contact or customer support department: <<telephone>> or by email at <<email>>
            ''strCode(9) = .Content("EmailMergeCode09", Settings.Language, True)          ' 	Yours sincerely Fred Bloggs <br> Ace IT Suppliers<br>  
            ''strCode(10) = .Content("EmailMergeCode10", Settings.Language, True)         ' 	Email  Footer info

            strCode(0) = _emailBody

            '------------------------------------------------------------------------------------------------
            '    Get data field delimiters
            '
            strField(0) = .Content("NewLine", Settings.Language, True)
            strField(1) = .Content("ProcessedOrderId", Settings.Language, True)
            strField(2) = .Content("Loginid", Settings.Language, True)
            strField(3) = .Content("UserNumber", Settings.Language, True)
            strField(4) = .Content("Status", Settings.Language, True)
            strField(5) = .Content("Comment", Settings.Language, True)
            strField(6) = .Content("ContactName", Settings.Language, True)
            strField(7) = .Content("AddressLine1", Settings.Language, True)
            strField(8) = .Content("AddressLine2", Settings.Language, True)
            strField(9) = .Content("AddressLine3", Settings.Language, True)
            strField(10) = .Content("AddressLine4", Settings.Language, True)
            strField(11) = .Content("AddressLine5", Settings.Language, True)
            strField(12) = .Content("Postcode", Settings.Language, True)
            strField(13) = .Content("Country", Settings.Language, True)
            strField(14) = .Content("ContactPhone", Settings.Language, True)
            strField(15) = .Content("ContactEmail", Settings.Language, True)
            strField(16) = .Content("PromotionDescription", Settings.Language, True)
            strField(17) = .Content("SpecialInstructions1", Settings.Language, True)
            strField(18) = .Content("SpecialInstructions2", Settings.Language, True)
            strField(19) = .Content("TrackingNo", Settings.Language, True)
            strField(20) = .Content("Currency", Settings.Language, True)
            strField(21) = .Content("PaymentType", Settings.Language, True)
            strField(22) = .Content("BackOfficeOrderId", Settings.Language, True)
            strField(23) = .Content("BackOfficeStatus", Settings.Language, True)
            strField(24) = .Content("BackOfficeReference", Settings.Language, True)
            strField(25) = .Content("Language", Settings.Language, True)
            strField(26) = .Content("Warehouse", Settings.Language, True)

            strField(50) = .Content("TotalOrderItemsValueGross", Settings.Language, True)
            strField(51) = .Content("TotalOrderItemsValueNet", Settings.Language, True)
            strField(52) = .Content("TotalOrderItemsTax", Settings.Language, True)
            strField(53) = .Content("TotalDeliveryGross", Settings.Language, True)
            strField(54) = .Content("TotalDeliveryTax", Settings.Language, True)
            strField(55) = .Content("TotalDeliveryNet", Settings.Language, True)
            strField(56) = .Content("PromotionValue", Settings.Language, True)
            strField(57) = .Content("TotalOrderValue", Settings.Language, True)
            strField(58) = .Content("TotalAmountCharged", Settings.Language, True)
            strField(59) = .Content("TotalValueIncPromo", Settings.Language, True)
            strField(60) = .Content("CustomerTitle", Settings.Language, True)
            strField(61) = .Content("CustomerForename", Settings.Language, True)
            strField(62) = .Content("CustomerSurname", Settings.Language, True)

            strField(63) = "<<<SpendorRefundOnAccountMessage>>>"

            strField(80) = .Content("CreatedDate", Settings.Language, True)
            strField(81) = .Content("DeliveryDate", Settings.Language, True)
        End With

        Return err
    End Function

    Private Function paymentMethodText(ByVal ds As Data.DataSet) As String
        Dim rtnStr = String.Empty
        Try

            Dim row As Data.DataRow = ds.Tables(0).Rows(0)
            Dim key As String = "PaymentMethodKey" & row.Item("PaymentMethod")

            ' Display any configurable text
            If Not String.IsNullOrEmpty(ucr.Content(key, Settings.Language, True)) Then
                rtnStr = ucr.Content(key, Settings.Language, True)
            End If

        Catch ex As Exception
        End Try

        Return rtnStr
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
                If row.Item("FeeType") <> "Y" AndAlso
                    Not String.IsNullOrEmpty(row.Item("ProductType")) Then

                    'Only display the prouct specific text once
                    Dim corpSalesString As String = String.Empty
                    If Not String.IsNullOrEmpty(row.Item("CallReference")) Then
                        Try
                            Dim callRef As Long = CType(row.Item("CallReference").ToString.Trim, Long)
                            If callRef > 0 Then
                                corpSalesString = "C"
                            Else
                                corpSalesString = String.Empty
                            End If
                        Catch ex As Exception
                            corpSalesString = String.Empty
                        End Try
                    End If
                    Dim key As String = "ProductTypeKey" & corpSalesString & row.Item("ProductType")
                    If productType.IndexOf(key) < 0 Then
                        productType.Add(key)

                        'Display any configurable text
                        If Not String.IsNullOrEmpty(ucr.Content(key, Settings.Language, True)) Then
                            rtnStr += ucr.Content(key, Settings.Language, True)
                        End If
                    End If
                End If

            Next

        Catch ex As Exception
        End Try

        Return rtnStr
    End Function

    Private Function ticketExchangeText(ByVal ds As Data.DataSet) As String
        Dim rtnStr = String.Empty
        Try
            Dim row As Data.DataRow
            Dim dt As Data.DataTable

            'Loop through the basket check for any items purchased from Ticket Exchange 
            dt = ds.Tables(1)
            For Each row In dt.Rows
                If Not String.IsNullOrEmpty(row.Item("BuybackOrTicketExchange")) AndAlso row.Item("BuybackOrTicketExchange") = GlobalConstants.RETURN_TYPE_TICKET_EXCHANGE Then
                    If Not String.IsNullOrEmpty(ucr.Content("TicketExchangeText", Settings.Language, True)) Then
                        rtnStr += ucr.Content("TicketExchangeText", Settings.Language, True)
                        Return rtnStr
                    End If
                End If
            Next

        Catch ex As Exception
        End Try

        Return rtnStr
    End Function

    Private Function ProductSpecificText(ByVal ds As Data.DataSet) As String
        Dim err As New ErrorObj
        Dim order As New Talent.Common.TalentOrder
        Dim deTicketingItemDetails As New Talent.Common.DETicketingItemDetails
        With deTicketingItemDetails
            .PaymentReference = EmailSettings.OrderConfirmation.PaymentReference
            .UnprintedRecords = ""
            .SetAsPrinted = "N"
            .EndOfSale = "Y"
            .RetryFailure = RetryFailure
            .Src = Settings.OriginatingSourceCode
        End With
        order.Settings = Settings
        order.Settings.Cacheing = False
        order.Dep.CollDEOrders.Add(deTicketingItemDetails)

        Dim productCodes As List(Of String) = New List(Of String)

        'Filter distinct products
        Dim purchaseResults As Data.DataRowCollection = ds.Tables("PurchaseResults").Rows

        For i As Integer = 0 To purchaseResults.Count - 1
            Dim productCode As String = purchaseResults(i).Item("ProductCode")

            If Not productCodes.Contains(productCode) Then
                productCodes.Add(productCode)
            End If
        Next

        Dim strProductCodes(productCodes.Count) As String
        strProductCodes = productCodes.ToArray

        order.Dep.ProductCodes = strProductCodes

        err = order.OrderProductsSpecificText

        Dim specificText As StringBuilder = New StringBuilder()
        Dim productCodeSpecific As String = Nothing
        Dim dtSpecificContent As DataTable

        If Not err.HasError AndAlso order.ResultDataSet IsNot Nothing AndAlso order.ResultDataSet.Tables.Count > 0 AndAlso order.ResultDataSet.Tables("ProductSpecificText").Rows.Count > 0 Then
            Dim specificTexts As Data.DataRowCollection = order.ResultDataSet.Tables("ProductSpecificText").Rows

            If specificTexts.Count > 0 Then
                For i As Integer = 0 To specificTexts.Count - 1
                    specificText.Append(specificTexts(i).Item("SpecificText"))
                    productCodeSpecific = specificTexts(i).Item("ProductCode")
                    'Add Product specific content from tbl_product_specific_content
                    dtSpecificContent = TDataObjects.ProductsSettings.TblProductSpecificContent.GetProductContent("Email", productCodeSpecific)
                    If dtSpecificContent.Rows.Count > 0 Then
                        specificText.Append(dtSpecificContent.Rows(0).Item("Product_Content").ToString)
                    End If
                Next
            End If
        Else
            For i As Integer = 0 To productCodes.Count - 1
                productCodeSpecific = productCodes.Item(i)
                'Add Product specific content from tbl_product_specific_content
                dtSpecificContent = TDataObjects.ProductsSettings.TblProductSpecificContent.GetProductContent("Email", productCodeSpecific)
                If dtSpecificContent.Rows.Count > 0 Then
                    specificText.Append(dtSpecificContent.Rows(0).Item("Product_Content").ToString)
                End If
            Next
        End If

        Return specificText.ToString
    End Function

    Private Function OrderTicketingDetails(ByVal drTicketingOrderRows() As DataRow,
                                           ByVal HtmlFormat As Boolean,
                                           ByVal OrderConfirmationTableRow As String,
                                           ByVal orderConfirmationSeparatorList() As String,
                                           ByVal OrderConfirmationTableHeader As String, ByRef err As ErrorObj) As String

        Dim finalMessage As String = String.Empty

        If drTicketingOrderRows.Length > 0 Then
            Dim displayFreeItems As Boolean = CType(Me.TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "DISPLAY_FREE_TICKETING_ITEMS"), Boolean)

            'Message builders
            Dim freeItemsExists As Boolean = False
            Dim feeItemsExists As Boolean = False
            Dim ppsItemsExists As Boolean = False
            Dim sbPPSItems As New StringBuilder
            Dim sbFreeItems As New StringBuilder
            Dim sbOtherItems As New StringBuilder
            Dim sbFeeItems As New StringBuilder

            Try
                Dim row As Data.DataRow

                For Each row In drTicketingOrderRows
                    Try
                        'pps items
                        If row.Item("ProductType").ToString.Trim = "P" Then

                            ppsItemsExists = True
                            If OrderConfirmationTableRow = "" Then
                                sbPPSItems.Append(DetailRow(row, _ucrDictionary, HtmlFormat))
                            Else
                                sbPPSItems.Append(DetailRow(row, _ucrDictionary, HtmlFormat, OrderConfirmationTableRow, orderConfirmationSeparatorList))
                            End If

                        ElseIf row.Item("Price") = "0.00" And row.Item("FeeType") <> "Y" Then

                            If displayFreeItems Then

                                freeItemsExists = True
                                If OrderConfirmationTableRow = "" Then
                                    sbFreeItems.Append(DetailRow(row, _ucrDictionary, HtmlFormat))
                                Else
                                    sbFreeItems.Append(DetailRow(row, _ucrDictionary, HtmlFormat, OrderConfirmationTableRow, orderConfirmationSeparatorList))
                                End If

                            End If
                            'ElseIf row.Item("FeeType") = "Y" Then
                            '    feeItemsExists = True
                            '    sbFeeItems.Append(DetailRow(row, ucrDictionary))
                        Else

                            If OrderConfirmationTableRow = "" Then
                                sbOtherItems.Append(DetailRow(row, _ucrDictionary, HtmlFormat))
                            Else
                                sbOtherItems.Append(DetailRow(row, _ucrDictionary, HtmlFormat, OrderConfirmationTableRow, orderConfirmationSeparatorList))
                            End If

                        End If
                    Catch ex As Exception
                        err.HasError = True
                        err.ErrorMessage = "Order ticketing Error: " & ex.Message
                        err.ErrorNumber = "TACTEMAIL-00"
                    End Try
                Next

                'Constrtuct the table of purchased items.
                If Not HtmlFormat Then
                    finalMessage = sbOtherItems.ToString()
                    If ppsItemsExists Then finalMessage += _ucrDictionary.Item("TicketingEmailCarriageReturn") & sbPPSItems.ToString()
                    If freeItemsExists Then finalMessage += _ucrDictionary.Item("TicketingEmailCarriageReturn") & sbFreeItems.ToString()
                    If feeItemsExists Then finalMessage += _ucrDictionary.Item("TicketingEmailCarriageReturn") & sbFeeItems.ToString()
                Else
                    If OrderConfirmationTableHeader = "" Then
                        finalMessage = "<table " & _ucrDictionary.Item("TicketingTableStyle") & ">" &
                                        "<tr><th>" & _ucrDictionary.Item("TicketingHead1") & "</th>" &
                                        "<th>" & _ucrDictionary.Item("TicketingHead2") & "</th>" &
                                        "<th>" & _ucrDictionary.Item("TicketingHead3") & "</th>" &
                                        "<th>" & _ucrDictionary.Item("TicketingHead4") & "</th>" &
                                        "<th>" & _ucrDictionary.Item("TicketingHead5") & "</th>" &
                                        "<th>" & _ucrDictionary.Item("TicketingHead6") & "</th></tr>"
                    Else
                        finalMessage = "<table " & _ucrDictionary.Item("TicketingTableStyle") & ">" & OrderConfirmationTableHeader
                    End If
                    finalMessage += sbOtherItems.ToString()
                    If ppsItemsExists Then finalMessage += sbPPSItems.ToString()
                    If freeItemsExists Then finalMessage += sbFreeItems.ToString()
                    If feeItemsExists Then finalMessage += sbFeeItems.ToString()
                    finalMessage += "</table>"
                End If

                sbOtherItems = Nothing
                sbPPSItems = Nothing
                sbFreeItems = Nothing
                sbFeeItems = Nothing

            Catch ex As Exception
                err.HasError = True
                err.ErrorMessage = "Order ticketing Error: " & ex.Message
                err.ErrorNumber = "TACTEMAIL-00"
            End Try
        End If

        Return finalMessage.Trim

    End Function

    Private Function DetailRow(ByVal row As DataRow, ByVal ucrDictionary As Generic.Dictionary(Of String, String), ByVal HtmlFormat As Boolean) As String

        Dim sbItem As New StringBuilder

        Try

            Dim tempProductDateTime As String = getProductDateTime(row.Item("ProductDate").ToString.Trim, row.Item("ProductTime").ToString.Trim)

            If Not HtmlFormat Then
                sbItem.Append(row.Item("ProductDescription").ToString.Trim)
                ' Add Date and time if applicable
                If tempProductDateTime.Length > 0 Then
                    sbItem.Append(ucrDictionary.Item("TicketingSeperator1") & tempProductDateTime)
                End If
                If row.Item("Price") = "0.00" And row.Item("FeeType") <> "Y" Then
                    sbItem.Append(ucrDictionary.Item("TicketingSeperator4") & row.Item("PriceBand").ToString.Trim)
                Else
                    ' Add seat information if applicable
                    If row.Item("ProductType").ToString.Trim = "T" Then
                        If row.Item("Seat").ToString.Trim <> "" Then
                            Dim seatInfo As String = row.Item("Seat").ToString.Trim
                            sbItem.Append(ucrDictionary.Item("TicketingSeperator4"))
                            sbItem.Append(getPartSeatInfo("Stand", seatInfo, False))
                            sbItem.Append(getPartSeatInfo("Area", seatInfo, False))
                        End If
                    ElseIf row.Item("ProductType").ToString.Trim = "E" Then
                        If row.Item("Seat").ToString.Trim <> "" Then
                            sbItem.Append(ucrDictionary.Item("TicketingSeperator4"))
                            sbItem.Append(row.Item("Seat").ToString.Trim().Replace("/", ""))
                        End If
                    Else
                        If row.Item("Seat").ToString.Trim <> "" Then
                            sbItem.Append(ucrDictionary.Item("TicketingSeperator4") & getSeatInfo(row))
                        End If
                    End If
                End If
                sbItem.Append(ucrDictionary.Item("TicketingSeperator5") & row.Item("ContactForename").ToString().Trim())
                sbItem.Append(ucrDictionary.Item("TicketingSeperator6") & row.Item("ContactSurname").ToString().Trim())
                sbItem.Append(ucrDictionary.Item("TicketingSeperator7") & row.Item("CustomerNo").ToString().Trim())
                sbItem.Append(ucrDictionary.Item("TicketingSeperator8") & row.Item("Price").ToString().Trim())


                'Add the carriage text
                If row.Item("FeeType").ToString.Trim <> "Y" AndAlso Not row.Item("ProductType").ToString.Trim = "P" _
                   AndAlso Not row.Item("ProductType").ToString.Trim = "C" Then
                    If row.Item("SmartcardUploaded") = "Y" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriageUpload"))
                    ElseIf row.Item("CarriageMethod") = "H" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriagePrintAtHome"))
                    ElseIf row.Item("CarriageMethod") = "P" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriagePost"))
                    ElseIf row.Item("CarriageMethod") = "C" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriageCollect"))
                    ElseIf row.Item("CarriageMethod") = "R" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriageRegPost"))
                    End If
                End If



                ' Add the carriage return
                sbItem.Append(ucrDictionary.Item("TicketingEmailCarriageReturn"))
            Else
                sbItem.Append("<tr>")
                sbItem.Append("<td>" & row.Item("ProductDescription").ToString.Trim & "</td>")
                If tempProductDateTime.Length > 0 Then
                    sbItem.Append("<td>" & tempProductDateTime & "</td>")
                Else
                    sbItem.Append("<td></td>")
                End If
                If (row.Item("Price") = "0.00" AndAlso row.Item("FeeType") <> "Y" AndAlso row.Item("ProductType").ToString.Trim <> "P") _
                    Or row.Item("Seat").ToString.Trim = "" Then
                    sbItem.Append("<td></td>")
                Else
                    If row.Item("ProductType").ToString.Trim = "T" Then
                        If row.Item("Seat").ToString.Trim <> "" Then
                            Dim seatInfo As String = row.Item("Seat").ToString.Trim
                            sbItem.Append("<td>")
                            sbItem.Append(getPartSeatInfo("Stand", seatInfo, False))
                            sbItem.Append(getPartSeatInfo("Area", seatInfo, False))
                            sbItem.Append("</td>")
                        End If
                    ElseIf row.Item("ProductType").ToString.Trim = "E" Then
                        If row.Item("Seat").ToString.Trim <> "" Then
                            sbItem.Append("<td>")
                            sbItem.Append(row.Item("Seat").ToString.Trim().Replace("/", ""))
                            sbItem.Append("</td>")
                        End If
                    Else
                        If row.Item("Seat").ToString.Trim <> "" Then
                            sbItem.Append("<td>" & getSeatInfo(row))
                            If Not String.IsNullOrEmpty(row.Item("RestText").ToString.Trim()) Then
                                sbItem.Append(" (" & row.Item("RestText").ToString.Trim() & ")")
                            End If
                            sbItem.Append("</td>")
                        End If
                    End If
                End If
                sbItem.Append("<td>" & row.Item("CustomerNo").ToString().TrimStart("0") & " - " & row.Item("ContactForename").ToString().Trim() & " " & row.Item("ContactSurname").ToString().Trim() & "</td>")
                'Add the carriage text
                If row.Item("FeeType").ToString.Trim = "Y" Or row.Item("ProductType").ToString.Trim = "P" Or
                   row.Item("ProductType").ToString.Trim = "C" Then
                    sbItem.Append("<td></td>")
                Else
                    If row.Item("SmartcardUploaded") = "Y" Then
                        sbItem.Append("<td>" & ucrDictionary.Item("TicketingCarriageUpload") & "</td>")
                    ElseIf row.Item("CarriageMethod") = "H" Then
                        sbItem.Append("<td>" & ucrDictionary.Item("TicketingCarriagePrintAtHome") & "</td>")
                    ElseIf row.Item("CarriageMethod") = "P" Then
                        sbItem.Append("<td>" & ucrDictionary.Item("TicketingCarriagePost") & "</td>")
                    ElseIf row.Item("CarriageMethod") = "C" Then
                        sbItem.Append("<td>" & ucrDictionary.Item("TicketingCarriageCollect") & "</td>")
                    ElseIf row.Item("CarriageMethod") = "R" Then
                        sbItem.Append("<td>" & ucrDictionary.Item("TicketingCarriageRegPost") & "</td>")
                    Else
                        sbItem.Append("<td></td>")
                    End If
                End If
                sbItem.Append("<td>" & row.Item("Price").ToString().Trim() & "</td>")
                sbItem.Append("</tr>")
            End If

        Catch ex As Exception
        End Try

        Return sbItem.ToString

    End Function

    Private Function DetailRow(ByVal row As DataRow, ByVal ucrDictionary As Generic.Dictionary(Of String, String),
                               ByVal HtmlFormat As Boolean,
                               ByVal OrderConfirmationTableRow As String,
                               ByVal orderConfirmationSeparatorList() As String) As String

        Dim sbItem As New StringBuilder("")

        Try

            If Not HtmlFormat Then
                '
                ' This non-html should be the same as the non-html section in the other DetailRow() method above.
                '
                Dim ProductDate As String = getProductDateTime(row.Item("ProductDate").ToString.Trim, row.Item("ProductTime").ToString.Trim)
                '
                ' Plain text
                '
                sbItem.Append(row.Item("ProductDescription").ToString.Trim)
                ' Add Date and time if applicable
                If ProductDate.Length > 0 Then
                    sbItem.Append(ucrDictionary.Item("TicketingSeperator1") & ProductDate)
                End If
                If row.Item("Price") = "0.00" And row.Item("FeeType") <> "Y" Then
                    sbItem.Append(ucrDictionary.Item("TicketingSeperator4") & row.Item("PriceBand").ToString.Trim)
                Else
                    ' Add seat information if applicable
                    If row.Item("ProductType").ToString.Trim = "T" Then
                        If row.Item("Seat").ToString.Trim <> "" Then
                            Dim seatInfo As String = row.Item("Seat").ToString.Trim
                            sbItem.Append(ucrDictionary.Item("TicketingSeperator4"))
                            sbItem.Append(getPartSeatInfo("Stand", seatInfo, False))
                            sbItem.Append(getPartSeatInfo("Area", seatInfo, False))
                        End If
                    ElseIf row.Item("ProductType").ToString.Trim = "E" Then
                        If row.Item("Seat").ToString.Trim <> "" Then
                            sbItem.Append(ucrDictionary.Item("TicketingSeperator4"))
                            sbItem.Append(row.Item("Seat").ToString.Trim().Replace("/", ""))
                        End If
                    Else
                        If row.Item("Seat").ToString.Trim <> "" Then
                            sbItem.Append(ucrDictionary.Item("TicketingSeperator4") & getSeatInfo(row))
                        End If
                    End If
                End If
                sbItem.Append(ucrDictionary.Item("TicketingSeperator5") & row.Item("ContactForename").ToString().Trim())
                sbItem.Append(ucrDictionary.Item("TicketingSeperator6") & row.Item("ContactSurname").ToString().Trim())
                sbItem.Append(ucrDictionary.Item("TicketingSeperator7") & row.Item("CustomerNo").ToString().Trim())
                sbItem.Append(ucrDictionary.Item("TicketingSeperator8") & row.Item("Price").ToString().Trim())


                'Add the carriage text
                If row.Item("FeeType").ToString.Trim <> "Y" AndAlso
                   Not row.Item("ProductType").ToString.Trim = "P" AndAlso
                   Not row.Item("ProductType").ToString.Trim = "C" Then

                    If row.Item("SmartcardUploaded") = "Y" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriageUpload"))
                    ElseIf row.Item("CarriageMethod") = "H" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriagePrintAtHome"))
                    ElseIf row.Item("CarriageMethod") = "P" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriagePost"))
                    ElseIf row.Item("CarriageMethod") = "C" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriageCollect"))
                    ElseIf row.Item("CarriageMethod") = "R" Then
                        sbItem.Append(ucrDictionary.Item("TicketingSeperator9") & ucrDictionary.Item("TicketingCarriageRegPost"))
                    End If

                End If



                ' Add the carriage return
                sbItem.Append(ucrDictionary.Item("TicketingEmailCarriageReturn"))

            Else
                '
                ' HTML format
                '
                Dim ProductDate As String = getProductDate(row.Item("ProductDate").ToString.Trim())
                Dim ProductTime As String = getProductTime(row.Item("ProductTime").ToString.Trim())
                Dim ProductDateRange As String = getProductDate(row.Item("BundleStartDate").ToString.Trim()) & " - " & getProductDate(row.Item("BundleEndDate").ToString.Trim())
                Dim Carriage As String = ""
                Dim Gate As String = ""
                Dim Turnstile As String = ""
                Dim Stand As String = ""
                Dim Area As String = ""
                Dim TheRow As String = ""
                Dim Seat As String = ""
                Dim Alpha As String = ""
                Dim SeatRestrictionCode As String = ""
                Dim SeatRestrictionText As String = ""
                Dim Quantity As String = row.Item("Quantity").ToString()
                Dim ProductDescription As String = row.Item("ProductDescription").ToString.Trim

                If Convert.ToBoolean(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SHOW_BUNDLE_DATE_AS_RANGE")) And
                Convert.ToBoolean(row.Item("bundle").ToString.Trim()) Then
                    ProductDate = ProductDateRange
                End If

                If row.Item("FeeType").ToString.Trim = "Y" Or row.Item("ProductType").ToString.Trim = "P" Or
                   row.Item("ProductType").ToString.Trim = "C" Then

                    Carriage = ""

                Else

                    If row.Item("SmartcardUploaded") = "Y" Then

                        Carriage = ucrDictionary.Item("TicketingCarriageUpload")

                    ElseIf row.Item("CarriageMethod") = "H" Then

                        Carriage = ucrDictionary.Item("TicketingCarriagePrintAtHome")

                    ElseIf row.Item("CarriageMethod") = "P" Then

                        Carriage = ucrDictionary.Item("TicketingCarriagePost")

                    ElseIf row.Item("CarriageMethod") = "C" Then

                        Carriage = ucrDictionary.Item("TicketingCarriageCollect")

                    ElseIf row.Item("CarriageMethod") = "R" Then

                        Carriage = ucrDictionary.Item("TicketingCarriageRegPost")
                    End If

                End If


                If row.Item("Seat").ToString.Trim() = "" Then
                    '
                    ' No seating information
                    '
                    Gate = ""
                    Turnstile = ""
                    Stand = ""
                    Area = ""
                    TheRow = ""
                    Seat = ""
                    Alpha = ""
                    If row.Item("ProductType").ToString.Trim() <> "C" Then
                        ProductDate = _ucrDictionary.Item("EmptyDateAndTimeValue")
                        ProductTime = _ucrDictionary.Item("EmptyDateAndTimeValue")
                    End If


                ElseIf (row.Item("Price") = "0.00" AndAlso row.Item("FeeType") <> "Y" _
                        AndAlso row.Item("ProductType").ToString.Trim() <> "P" _
                        AndAlso row.Item("ProductType").ToString.Trim() <> "H" _
                        AndAlso row.Item("ProductType").ToString.Trim() <> "S" _
                        AndAlso row.Item("ProductType").ToString.Trim() <> "T") Then
                    '
                    ' It's a free, non-PPS product, not a home ticket and not a season ticket
                    ' and not a travel product (as req stand/area (timeslot) to be displayed for these)
                    '
                    Gate = ""
                    Turnstile = ""
                    Stand = ""
                    Area = ""
                    TheRow = ""
                    Seat = ""
                    Alpha = ""

                ElseIf row.Item("FeeType") = "Y" Or
                    row.Item("ProductType").ToString.Trim() = "C" Then  ' C = Club Membership 
                    '
                    ' It's a fee or membership
                    '
                    Gate = ""
                    Turnstile = ""
                    Stand = ""
                    Area = ""
                    TheRow = ""
                    Seat = ""
                    Alpha = ""
                    '
                    ' Show the date and time for away games
                    '
                    If row.Item("ProductType").ToString.Trim() <> "A" AndAlso row.Item("ProductType").ToString.Trim() <> "C" Then
                        ProductDate = _ucrDictionary.Item("EmptyDateAndTimeValue")
                        ProductTime = _ucrDictionary.Item("EmptyDateAndTimeValue")
                    End If


                ElseIf row.Item("Roving").ToString = "Y" OrElse row.Item("Unreserved").ToString = "Y" Then


                    Gate = row.Item("Gates").ToString.Trim()
                    If Gate <> "" Then
                        Gate = orderConfirmationSeparatorList(6) & Gate
                    End If

                    Turnstile = row.Item("Turnstiles").ToString.Trim()
                    If Turnstile <> "" Then
                        Turnstile = orderConfirmationSeparatorList(5) & Turnstile
                    End If

                    If row.Item("Roving").ToString = "Y" Then
                        Stand = _ucrDictionary.Item("RovingAreaText")
                    ElseIf row.Item("Unreserved") = "Y" Then
                        Stand = _ucrDictionary.Item("UnreservedAreaText")
                    End If


                Else


                    If row.Item("ProductType").ToString.Trim() = "A" Then               ' Away

                        Gate = ""
                        Turnstile = ""
                        Dim seatText As String = row.Item("SeatText").ToString()
                        If seatText.Trim().Length > 0 AndAlso seatText.Trim().Length < 15 Then
                            Seat = orderConfirmationSeparatorList(0) & seatText.Trim()

                        ElseIf seatText.Trim().Length > 14 Then
                            Stand = seatText.Substring(0, 2).Trim()
                            If Stand <> "" Then
                                Stand = orderConfirmationSeparatorList(3) & Stand
                            End If

                            Area = seatText.Substring(3, 4).Trim()
                            If Area <> "" Then
                                Area = orderConfirmationSeparatorList(4) & Area
                            End If

                            TheRow = seatText.Substring(7, 4).Trim()
                            If TheRow <> "" Then
                                TheRow = orderConfirmationSeparatorList(2) & TheRow
                            End If

                            Seat = seatText.Substring(11, 4).Trim()
                            If seatText.Trim().Length > 15 Then Alpha = seatText.Substring(15, 1).Trim()
                            If Seat <> "" Then
                                Seat = orderConfirmationSeparatorList(0) & Seat
                                If Alpha <> "" Then
                                    Alpha = orderConfirmationSeparatorList(1) & Alpha
                                End If
                            End If
                        End If
                    ElseIf row.Item("ProductType").ToString.Trim() = "T" Then           ' Travel

                        Gate = ""
                        Turnstile = ""
                        Stand = row.Item("Stand").ToString.Trim()
                        Area = row.Item("Area").ToString.Trim()
                        TheRow = ""
                        Seat = ""
                        Alpha = ""

                    ElseIf row.Item("ProductType").ToString.Trim() = "E" Then           ' Events

                        Gate = ""
                        Turnstile = ""

                        'Trim any leading zeros
                        Stand = row.Item("Stand").ToString.Trim().TrimStart("0")
                        If String.IsNullOrEmpty(Stand) Then
                            Area = row.Item("Area").ToString.Trim().TrimStart("0")
                        Else
                            Area = row.Item("Area").ToString.Trim()
                        End If
                        If String.IsNullOrEmpty(Area) Then
                            TheRow = row.Item("RowN").ToString.Trim().TrimStart("0")
                        Else
                            TheRow = row.Item("RowN").ToString.Trim()
                        End If
                        If String.IsNullOrEmpty(TheRow) Then
                            Seat = row.Item("SeatN").ToString.Trim().TrimStart("0")
                        Else
                            Seat = row.Item("SeatN").ToString.Trim()
                        End If
                        Alpha = row.Item("SeatSuffix").ToString.Trim()

                    Else

                        Gate = row.Item("Gates").ToString.Trim()
                        If Gate <> "" Then
                            Gate = orderConfirmationSeparatorList(6) & Gate
                        End If

                        Turnstile = row.Item("Turnstiles").ToString.Trim()
                        If Turnstile <> "" Then
                            Turnstile = orderConfirmationSeparatorList(5) & Turnstile
                        End If

                        Stand = row.Item("Stand").ToString.Trim()
                        If Stand <> "" Then
                            Stand = orderConfirmationSeparatorList(3) & Stand
                        End If

                        Area = row.Item("Area").ToString.Trim()
                        If Area <> "" Then
                            Area = orderConfirmationSeparatorList(4) & Area
                        End If

                        TheRow = row.Item("RowN").ToString.Trim()
                        If TheRow <> "" Then
                            TheRow = orderConfirmationSeparatorList(2) & TheRow
                        End If

                        Seat = row.Item("SeatN").ToString.Trim()
                        Alpha = row.Item("SeatSuffix").ToString.Trim()
                        SeatRestrictionCode = row.Item("SeatRestriction").ToString.Trim()
                        If Seat <> "" Then
                            Seat = orderConfirmationSeparatorList(0) & Seat
                            If Alpha <> "" Then
                                Alpha = orderConfirmationSeparatorList(1) & Alpha
                            End If
                            If SeatRestrictionCode <> "" Then
                                SeatRestrictionCode = orderConfirmationSeparatorList(7) & SeatRestrictionCode
                                SeatRestrictionText = row.Item("RestText").ToString.Trim()

                                '
                                ' Cannot add the orderConfirmationSeparatorList(8) here as it's value is encoded
                                ' but is often '<br> or similar which then ends up as '&lt;br &gt;' 
                                ' 
                                'If SeatRestrictionText <> "" Then
                                '    SeatRestrictionText = orderConfirmationSeparatorList(8) & SeatRestrictionText
                                'End If
                            End If
                        End If

                    End If

                End If

                If row.Item("CancelledSeat") Then
                    Seat = Seat.Trim + " " + ucr.Content("SeatCancelledText", Settings.Language, False)
                End If

                sbItem.Append(ReplaceTableRowContentFields(OrderConfirmationTableRow,
                                                           ProductDescription,
                                                           ProductDate,
                                                           ProductTime,
                                                           row.Item("Title").ToString().Trim(),
                                                           row.Item("ContactForename").ToString().Trim(),
                                                           row.Item("ContactSurname").ToString().Trim(),
                                                           row.Item("CustomerNo").ToString().TrimStart("0"),
                                                           Carriage,
                                                           row.Item("Price").ToString().Trim(),
                                                           row.Item("PriceBDesc").ToString.Trim(),
                                                           row.Item("PriceCDesc").ToString.Trim(),
                                                           Gate,
                                                           Turnstile,
                                                           Stand,
                                                           Area,
                                                           TheRow,
                                                           Seat,
                                                           Alpha,
                                                           SeatRestrictionCode,
                                                           SeatRestrictionText,
                                                           orderConfirmationSeparatorList(8),
                                                           Quantity,
                                                           row.Item("HideDate"),
                                                           row.Item("HideTime"),
                                                           row.Item("SummariseRecordsOnEmail"),
                                                           row.Item("BulkID"),
                                                           0, 0, 0, 0, 0, 0, 0))

            End If

        Catch ex As Exception
        End Try

        Return sbItem.ToString

    End Function

    Private Function getProductDateTime(ByVal inDate As String, ByVal inTime As String) As String

        Return getProductDate(inDate) + getProductTime(inTime)

    End Function

    Private Function getProductDate(ByVal inDate As String) As String
        Dim prodDate As String = String.Empty
        Dim year As String
        Dim month As String
        Dim day As String
        Try
            year = inDate.Substring(1, 2)
            month = inDate.Substring(3, 2)
            day = inDate.Substring(5, 2)
            If day = "00" Then
                prodDate = ""
            Else
                prodDate = day & "/" & month & "/" & year & ucr.Content("TicketingSeperator2", Settings.Language, True)
            End If
        Catch ex As Exception
        End Try
        Return prodDate
    End Function

    Private Function getProductTime(ByVal inTime As String) As String
        Dim prodDateTime As String = String.Empty
        Try
            If inTime.Trim <> "" Then
                prodDateTime = inTime & ucr.Content("TicketingSeperator3", Settings.Language, True)
            End If
        Catch ex As Exception
        End Try
        Return prodDateTime
    End Function

    Private Function getSeatInfo(ByVal row As Data.DataRow) As String

        Dim seatInfo As String = String.Empty
        Dim seatDisplay As Integer = CType(Me.TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "SEAT_DISPLAY"), Integer)

        Select Case ucr.Attribute("SeatType")
            Case Is = "1"   '
                seatInfo = getTurnstileInfo(row.Item("Turnstiles").ToString) +
                            getGateInfo(row.Item("Gates").ToString) +
                            getPartSeatInfo("Row", row.Item("Seat").ToString.Trim)
                If Not seatDisplay = 1 Then
                    seatInfo = seatInfo + getPartSeatInfo("Seat", row.Item("Seat").ToString.Trim)
                End If
            Case Is = "2"
                seatInfo = getTurnstileInfo(row.Item("Turnstiles").ToString) +
                            getPartSeatInfo("Stand", row.Item("Seat").ToString.Trim) +
                            getPartSeatInfo("Area", row.Item("Seat").ToString.Trim) +
                            getPartSeatInfo("Row", row.Item("Seat").ToString.Trim)
                If Not seatDisplay = 1 Then
                    seatInfo = seatInfo + getPartSeatInfo("Seat", row.Item("Seat").ToString.Trim)
                End If
            Case Is = "3"
                seatInfo = row.Item("Seat").ToString.Trim &
                            getTurnstileInfo(row.Item("Turnstiles").ToString) +
                             getGateInfo(row.Item("Gates").ToString)

            Case Else
                Select Case seatDisplay
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
            turnstilesInfo = ucr.Content("TurnstileSeperator1", Settings.Language, True)
            Do While index < 10 And (index + 3) <= turnstiles.Length
                If turnstiles.Substring(index, 3).Trim <> String.Empty Then
                    If index <> 0 Then
                        turnstilesInfo += ucr.Content("TurnstileSeperator2", Settings.Language, True)
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
            gateInfo = ucr.Content("GateSeperator1", Settings.Language, True)
            Do While index < 10 And (index + 3) <= gates.Length
                If gates.Substring(index, 3).Trim <> String.Empty Then
                    If index <> 0 Then
                        gateInfo += ucr.Content("GateSeperator2", Settings.Language, True)
                    End If
                    gateInfo += gates.Substring(index, 3).Trim
                End If
                index += 3
            Loop
        End If

        Return gateInfo

    End Function

    Private Function getPartSeatInfo(ByVal part As String, ByVal seat As String, Optional ByVal showLabel As Boolean = True) As String

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
                        If showLabel Then seatInfo = ucr.Content("StandSeperator1", Settings.Language, True)
                        seatInfo += seat.Substring(start, length)
                    End If
                Case Is = "Area"
                    start = standSep + 1
                    length = areaSep - 1 - standSep
                    If (start + length) < seat.Length And length > 0 Then
                        If showLabel Then seatInfo = ucr.Content("AreaSeperator1", Settings.Language, True)
                        seatInfo += seat.Substring(start, length)
                    End If
                Case Is = "Row"
                    start = areaSep + 1
                    length = rowSep - 1 - areaSep
                    If (start + length) < seat.Length And length > 0 Then
                        If showLabel Then seatInfo = ucr.Content("RowSeperator1", Settings.Language, True)
                        seatInfo += seat.Substring(start, length)
                    End If
                Case Is = "Seat"
                    start = rowSep + 1
                    If (start) < seat.Length Then
                        If showLabel Then seatInfo = ucr.Content("SeatSeperator1", Settings.Language, True)
                        seatInfo += seat.Substring(start)
                    End If
                Case Is = "SeatNo"
                    start = rowSep + 1
                    If (start) < seat.Length Then
                        If showLabel Then seatInfo = ucr.Content("SeatSeperator1", Settings.Language, True)
                        seatInfo += seat.Substring(start, 4)
                    End If
                Case Is = "Alpha"
                    start = rowSep + 1
                    If (start) < seat.Length Then
                        seatInfo += seat.Substring(start)
                        If seatInfo.Length = 6 Then
                            seatInfo = seatInfo.Substring(5)
                        Else
                            seatInfo = ""
                        End If
                    End If
            End Select
        Catch ex As Exception
        End Try

        Return seatInfo

    End Function

    Private Function DirectDebitSchedule(ByVal ds As Data.DataSet, ByVal HtmlFormat As Boolean) As String
        Dim rtnStr As New StringBuilder

        'Retrieve the payment schedule
        Dim err As New Talent.Common.ErrorObj
        Dim sortCode As String = String.Empty
        Dim accountNumber As String = String.Empty
        Dim totAmount As String = String.Empty
        Dim totAmountPaid As String = String.Empty
        Dim returnErrorCode As String = String.Empty
        Dim payment As New Talent.Common.TalentPayment
        Dim dePayment As New Talent.Common.DEPayments

        With dePayment
            .PaymentRef = EmailSettings.OrderConfirmation.PaymentReference
            .Source = Settings.OriginatingSourceCode
            .CustomerNumber = EmailSettings.OrderConfirmation.Customer
        End With

        payment.De = dePayment
        payment.Settings = Settings
        err = payment.DirectDebitSummary

        'Populate the email with the direct debit summary 
        If Not err.HasError AndAlso Not payment.ResultDataSet Is Nothing AndAlso payment.ResultDataSet.Tables.Count > 1 Then

            Dim ddStatusDictionary As New Generic.Dictionary(Of String, String)
            ddStatusDictionary = populateDDStatusDescriptions()

            With ucr
                rtnStr.Append(.Content("DirectDebitTopSeparator", Settings.Language, True)).Append(TicketingCarriageReturn(HtmlFormat))

                'Title and Guarantee
                rtnStr.Append(.Content("DirectDebitTitle", Settings.Language, True)).Append(TicketingCarriageReturn(HtmlFormat))
                rtnStr.Append(.Content("DirectDebitGuarantee", Settings.Language, True)).Append(TicketingCarriageReturn(HtmlFormat))
                rtnStr.Append(TicketingCarriageReturn(HtmlFormat))

                'DDI Ref
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitDDIRef", Settings.Language, True), 40))
                rtnStr.Append(payment.ResultDataSet.Tables(0).Rows(0).Item("DirectDebitDDIRef").ToString.Trim).Append(TicketingCarriageReturn(HtmlFormat))

                'Account Name
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitAccountName", Settings.Language, True), 40))
                rtnStr.Append(payment.ResultDataSet.Tables(0).Rows(0).Item("AccountName").ToString.Trim).Append(TicketingCarriageReturn(HtmlFormat))

                'Sort Code
                sortCode = payment.ResultDataSet.Tables(0).Rows(0).Item("SortCode").ToString.Trim
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitSortCode", Settings.Language, True), 40))
                If sortCode.Length = 6 Then
                    rtnStr.Append("****").Append(sortCode.Substring(4, 2)).Append(TicketingCarriageReturn(HtmlFormat))
                Else
                    rtnStr.Append(sortCode).Append(TicketingCarriageReturn(HtmlFormat))
                End If

                'Account Number
                accountNumber = payment.ResultDataSet.Tables(0).Rows(0).Item("AccountNumber").ToString.Trim
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitAccountNumber", Settings.Language, True), 40))
                If accountNumber.Length = 8 Then
                    rtnStr.Append("******").Append(accountNumber.Substring(6, 2)).Append(TicketingCarriageReturn(HtmlFormat))
                Else
                    rtnStr.Append(accountNumber).Append(TicketingCarriageReturn(HtmlFormat))
                End If
                rtnStr.Append(TicketingCarriageReturn(HtmlFormat))

                'Total Amount Paid
                totAmountPaid = payment.ResultDataSet.Tables(0).Rows(0).Item("TotalAmountPaid").ToString.Trim
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitTotAmountPaid", Settings.Language, True), 40))
                If totAmountPaid IsNot String.Empty Then
                    rtnStr.Append(Talent.Common.Utilities.FixStringLength(getPriceWithFormattedCurrency(totAmountPaid), 40))
                End If
                rtnStr.Append(TicketingCarriageReturn(HtmlFormat))

                ' Direct Debit Schedule Header Records
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitPaymentDate", Settings.Language, True), 40))
                rtnStr.Append(Talent.Common.Utilities.FixStringLength(.Content("DirectDebitPaymentAmount", Settings.Language, True), 40))
                rtnStr.Append(.Content("DirectDebitPaymentStatus", Settings.Language, True)).Append(TicketingCarriageReturn(HtmlFormat))

                Dim row As Data.DataRow
                For Each row In payment.ResultDataSet.Tables(1).Rows
                    ' Direct Debit Schedule Detail Records
                    rtnStr.Append(Talent.Common.Utilities.FixStringLength(row.Item("PaymentDate"), 40))
                    rtnStr.Append(Talent.Common.Utilities.FixStringLength(getPriceWithFormattedCurrency(row.Item("PaymentAmount")), 40))

                    Dim statusDesc As String
                    If ddStatusDictionary.TryGetValue(Utilities.CheckForDBNull_String(row.Item("ScheduledPaymentStatus")), statusDesc) Then
                        rtnStr.Append(statusDesc)
                    Else
                        rtnStr.Append(row.Item("ScheduledPaymentStatus"))
                    End If
                    rtnStr.Append(TicketingCarriageReturn(HtmlFormat))
                Next

                rtnStr.Append(ucr.Content("DirectDebitBottomSeparator", Settings.Language, True)).Append(TicketingCarriageReturn(HtmlFormat))
            End With
        End If


        Return rtnStr.ToString()
    End Function

    Private Function TicketingCarriageReturn(ByVal HtmlFormat As Boolean) As String
        If ucr.Content("TicketingEmailCarriageReturn", Settings.Language, True).Trim = "" Then
            If HtmlFormat Then
                Return "<br />"
            Else
                Return vbCrLf
            End If
        Else
            Return ucr.Content("TicketingEmailCarriageReturn", Settings.Language, True)
        End If
    End Function

    Private Function OrderCorporateDetails(ByVal drPackageOrderRows() As DataRow,
                                            ByVal HtmlFormat As Boolean,
                                            ByVal OrderCorporateTableHeader As String,
                                            ByVal OrderCorporateTableRow As String) As String

        Dim sbFinalMessage As New StringBuilder
        Dim row As Data.DataRow
        If drPackageOrderRows.Length > 0 Then
            If _totalColumns > 0 AndAlso _totalColumnData > 0 Then

                sbFinalMessage.Append("<table " & ucr.Attribute("PackageTableStyle") & ">")
                sbFinalMessage.Append(OrderCorporateTableHeader)

                For Each row In drPackageOrderRows
                    Dim corporateDataRow As String = OrderCorporateTableRow
                    Try
                        Dim packageDescription As String = row.Item("PackageDescription").ToString.Trim()
                        Dim packageDetailsLink As String = _ucrDictionary.Item("PackageDetailsLinkForEmail")
                        If EmailSettings.OrderConfirmation.WebsiteURL.Trim.Length > 0 AndAlso packageDetailsLink.Length > 0 Then
                            Dim tempLink As String = String.Empty
                            Dim tempPackageDescription As String = String.Empty
                            tempLink = EmailSettings.OrderConfirmation.WebsiteURL.Trim & packageDetailsLink & "?ProductCode=" & row.Item("ProductCode") & "&PackageID=" & row.Item("PackageID")
                            tempPackageDescription = "<a "
                            tempPackageDescription += _ucrDictionary.Item("PackageDetailsLinkForEmail")
                            tempPackageDescription += " href='" & tempLink & "' target='_blank'>"
                            tempPackageDescription += packageDescription
                            tempPackageDescription += "</a>"
                            packageDescription = tempPackageDescription
                        End If
                        corporateDataRow = corporateDataRow.Replace("<<PackageReference>>", row.Item("PackageID").ToString.Trim())
                        corporateDataRow = corporateDataRow.Replace("<<PackageDescription>>", packageDescription)
                        corporateDataRow = corporateDataRow.Replace("<<NumberOfUnits>>", row.Item("NumberOfUnits").ToString.Trim())
                        corporateDataRow = corporateDataRow.Replace("<<GoodsValue>>", row.Item("GoodsValue").ToString.Trim())
                        corporateDataRow = corporateDataRow.Replace("<<TotalVATValue>>", row.Item("TotalVATValue").ToString.Trim())
                        corporateDataRow = corporateDataRow.Replace("<<TotalValue>>", row.Item("PaymentAmnt").ToString.Trim())
                        corporateDataRow = corporateDataRow.Replace("<<CallReference>>", row.Item("CallReference").ToString.Trim())
                        corporateDataRow = ReplaceTableRowContentFields(corporateDataRow,
                                                       row.Item("ProductDescription").ToString.Trim(),
                                                       getProductDate(row.Item("ProductDate").ToString.Trim()),
                                                       getProductTime(row.Item("ProductTime").ToString.Trim()),
                                                       row.Item("Title").ToString().Trim(),
                                                       row.Item("ContactForename").ToString().Trim(),
                                                       row.Item("ContactSurname").ToString().Trim(),
                                                       row.Item("CustomerNo").ToString().TrimStart("0"),
                                                       row.Item("CarriageMethod").ToString(),
                                                       row.Item("Price").ToString().Trim(),
                                                       row.Item("PriceBDesc").ToString.Trim(),
                                                       row.Item("PriceCDesc").ToString.Trim(),
                                                       row.Item("Gates").ToString.Trim(),
                                                       row.Item("Turnstiles").ToString.Trim(),
                                                       row.Item("Stand").ToString.Trim(),
                                                       row.Item("Area").ToString.Trim(),
                                                       row.Item("RowN").ToString.Trim(),
                                                       row.Item("SeatN").ToString.Trim(),
                                                       row.Item("SeatSuffix").ToString.Trim(),
                                                       row.Item("SeatRestriction").ToString.Trim(),
                                                       row.Item("RestText").ToString.Trim(),
                                                       String.Empty,
                                                       String.Empty,
                                                       row.Item("HideDate"),
                                                       row.Item("HideTime"),
                                                       row.Item("SummariseRecordsOnEmail"),
                                                       row.Item("BulkID"), 0, 0, 0, 0, 0, 0, 0)
                    Catch ex As Exception
                        corporateDataRow = String.Empty
                    End Try
                    sbFinalMessage.Append(corporateDataRow)
                Next

                sbFinalMessage.Append("</table>")
            End If
        End If
        Return sbFinalMessage.ToString.Trim
    End Function

    Private Function OrderBundlePackageDetails(ByVal drPackageOrderRows() As DataRow,
                                          ByVal HtmlFormat As Boolean,
                                          ByVal OrderPackageBundleTableHeader As String,
                                          ByVal OrderPackageBundleTableRow As String,
                                         ByVal order As TalentOrder) As String

        Dim sbFinalMessage As New StringBuilder
        Dim packageAlreadyProcessed As Collection = New Collection
        Dim row As Data.DataRow
        Dim haveWrittenBulkHeader As Boolean
        Dim packagePricingMode As String = String.Empty

        If drPackageOrderRows.Length > 0 Then
            If _totalColumns > 0 AndAlso _totalColumnData > 0 Then


                For Each row In drPackageOrderRows
                    'Dim corporateDataRow As String = OrderCorporateTableRow

                    Dim corporateDataRow As String = OrderPackageBundleTableRow
                    If Not packageAlreadyProcessed.Contains(row.Item("CallReference").ToString.Trim()) Then
                        Try
                            packageAlreadyProcessed.Add(row.Item("CallReference"), row.Item("CallReference"))
                            ' get package dets via purchase histore
                            order.Dep.LastRecordNumber = 0
                            order.Dep.TotalRecords = 0
                            order.Settings.AccountNo1 = EmailSettings.OrderConfirmation.Customer
                            order.Dep.PaymentReference = EmailSettings.OrderConfirmation.PaymentReference
                            order.Dep.CallId = row.Item("CallReference").ToString.Trim
                            order.ResultDataSet = Nothing
                            Dim err As New ErrorObj
                            err = order.OrderEnquiryDetails()
                            Dim dtStatus As New Data.DataTable
                            Dim dtPackageDetail As New Data.DataTable
                            Dim dtComponentSummary As New Data.DataTable
                            If Not err.HasError AndAlso Not order.ResultDataSet Is Nothing Then
                                dtStatus = order.ResultDataSet.Tables("StatusResults")
                                dtPackageDetail = order.ResultDataSet.Tables("PackageDetail")
                                dtComponentSummary = order.ResultDataSet.Tables("ComponentSummary")
                                sbFinalMessage.Append("<p>").Append(dtStatus.Rows(0)("PackageDescription").ToString.Trim).Append(" / ")
                                sbFinalMessage.Append(dtStatus.Rows(0)("ProductDescription").ToString.Trim)

                                sbFinalMessage.Append("<br />")
                                Dim bundleStartDate As String = dtStatus.Rows(0)("BundleStartDate").ToString.Trim
                                Dim bundleEndDate As String = dtStatus.Rows(0)("BundleEndDate").ToString.Trim
                                If bundleStartDate <> bundleEndDate And Not bundleStartDate Is String.Empty Then
                                    sbFinalMessage.Append(getProductDate(bundleStartDate)).Append(" - ").Append(getProductDate(bundleEndDate))
                                Else
                                    sbFinalMessage.Append(getProductDate(dtStatus.Rows(0)("ProductDate").ToString.Trim))
                                End If
                                sbFinalMessage.Append("</p>")

                                ' loop through components..
                                sbFinalMessage.Append("<table " & ucr.Attribute("PackageTableStyle") & ">")
                                sbFinalMessage.Append(OrderPackageBundleTableHeader)
                                Dim seatsDetails As StringBuilder = New StringBuilder
                                Dim componentId As String = String.Empty

                                For Each drComponent As DataRow In dtPackageDetail.Rows
                                    If packagePricingMode Is String.Empty AndAlso drComponent("PackagePriceMode") IsNot String.Empty Then
                                        packagePricingMode = drComponent("PackagePriceMode")
                                    End If
                                    corporateDataRow = OrderPackageBundleTableRow
                                    seatsDetails.Clear()
                                    corporateDataRow = corporateDataRow.Replace("<<ComponentDescription>>", drComponent.Item("ComponentDescription").ToString.Trim())
                                    corporateDataRow = corporateDataRow.Replace("<<Quantity>>", drComponent.Item("Quantity").ToString.Trim())
                                    corporateDataRow = corporateDataRow.Replace("<<Price>>", drComponent.Item("TotalPrice").ToString.Trim())
                                    ' Get seat details
                                    componentId = drComponent.Item("ComponentID").ToString.Trim()
                                    For Each drSeat As DataRow In dtComponentSummary.Rows
                                        If drSeat("ComponentID").ToString.Trim = componentId Then
                                            If drSeat("AvailabilityComponent") = True AndAlso drSeat("BulkID") > 0 Then
                                                haveWrittenBulkHeader = False
                                                For Each drBulkRow As DataRow In _dtBulkTickets.Rows
                                                    If drSeat("BulkID") = drBulkRow("BulkId") Then
                                                        If Not haveWrittenBulkHeader Then
                                                            seatsDetails.Append(drBulkRow("SeatCount"))
                                                            seatsDetails.Append(" ")
                                                            seatsDetails.Append(Trim((drBulkRow("PriceBand"))))
                                                            seatsDetails.Append(" ")
                                                            seatsDetails.Append("Tickets")
                                                            seatsDetails.Append("  ")
                                                            seatsDetails.Append(getPriceWithFormattedCurrency(drBulkRow("Price")))
                                                            seatsDetails.Append("<br />")
                                                            seatsDetails.Append(drSeat("StandDesc"))
                                                            seatsDetails.Append("/")
                                                            seatsDetails.Append(drSeat("AreaDesc"))
                                                            seatsDetails.Append("<br />")
                                                            haveWrittenBulkHeader = True
                                                        End If
                                                        seatsDetails.Append("Row: ")
                                                        seatsDetails.Append(drBulkRow("Rown"))
                                                        seatsDetails.Append("Seats: ")
                                                        seatsDetails.Append(Utilities.FormatBulkSeats(drBulkRow("Seats")) & "<br />")
                                                    End If
                                                Next
                                            Else

                                                If drSeat("Seat").ToString.Trim = String.Empty Then
                                                    seatsDetails.Append(drSeat("Quantity").ToString & " x ")
                                                    seatsDetails.Append(drSeat("StandDesc").ToString & "/" & drSeat("AreaDesc").ToString & "<br />")
                                                Else
                                                    seatsDetails.Append(drSeat("Seat").ToString & "<br />")
                                                End If
                                            End If
                                        End If
                                    Next
                                    corporateDataRow = corporateDataRow.Replace("<<Details>>", seatsDetails.ToString)
                                    sbFinalMessage.Append(corporateDataRow)
                                Next
                            End If
                            sbFinalMessage.Append("</table>")


                        Catch ex As Exception
                            corporateDataRow = String.Empty
                        End Try
                    End If
                Next

            End If
        End If
        Dim FinalTable As String = sbFinalMessage.ToString.Trim
        If packagePricingMode = GlobalConstants.PACKAGE_PRICING Then
            FinalTable = FinalTable.Replace("<<PriceColumnVisible>>", " style=""display:none;""")
        Else
            FinalTable = FinalTable.Replace("<<PriceColumnVisible>>", String.Empty)
        End If
        Return FinalTable
    End Function

    Private Function OrderFeesDetails(ByVal drOrderFeesRows() As DataRow, ByVal HtmlFormat As Boolean, ByVal RetailOrderFeeTableRow As String, ByVal retailFee As String, retailFeeDescription As String) As String

        Dim sbFinalMessage As New StringBuilder
        Dim adHocFeesMessage As New StringBuilder
        Dim combinedFees As String = String.Empty

        If drOrderFeesRows.Length > 0 Then
            Dim messagePrefix As String = String.Empty
            Dim messageSuffix As String = String.Empty
            Dim adhocFee As Boolean = False
            Dim _TDataObjects As New TalentDataObjects
            Dim settings As New DESettings
            settings.FrontEndConnectionString = ucr.FrontEndConnectionString
            settings.DestinationDatabase = "SQL2005"
            _TDataObjects.Settings = settings
            Dim dtAdhocFees As DataTable = _TDataObjects.PaymentSettings.TblAdhocFees.GetByBUPartnerLang(ucr.BusinessUnit, ucr.PartnerCode, Utilities.GetDefaultLanguage, True)

            Try
                Dim row As Data.DataRow
                If Not HtmlFormat Then
                    sbFinalMessage.Append(_ucrDictionary.Item("TicketingEmailCarriageReturn"))
                    messageSuffix = _ucrDictionary.Item("TicketingEmailCarriageReturn")
                Else
                    sbFinalMessage.Append("<table " & _ucrDictionary.Item("OrderFeesTableStyle") & ">")
                    If Not _ucrDictionary.Item("OrderFeesHeader").Trim.Equals("") Then
                        sbFinalMessage.Append("<tr><th>" & _ucrDictionary.Item("OrderFeesHeader") & "</th><td>&nbsp;</td></tr>")
                    End If
                    messagePrefix = "<tr><td>"
                    messageSuffix = "</td></tr>"
                End If

                For Each row In drOrderFeesRows

                    'Check to see if this fee is an adhoc fee
                    Dim adHocFeeDescription As String = String.Empty
                    Dim adHocFeeIsNegative As Boolean = False
                    If dtAdhocFees.Rows.Count > 0 Then
                        For Each tblAdhocFeesRow As DataRow In dtAdhocFees.Rows
                            If row("ProductCode").ToString.Trim.Equals(tblAdhocFeesRow("FEE_CODE").ToString.Trim) Then
                                adHocFeeDescription = tblAdhocFeesRow("FEE_DESCRIPTION").ToString
                                Try
                                    adHocFeeIsNegative = CBool(tblAdhocFeesRow("IS_NEGATIVE_FEE"))
                                Catch
                                    adHocFeeIsNegative = False
                                End Try
                                adhocFee = True
                                Exit For
                            Else
                                adhocFee = False
                            End If
                        Next
                    End If

                    If adhocFee Then
                        Dim adhocFeeItem As String = _ucrDictionary.Item("TicketingAdHocFeesRow")
                        adhocFeeItem = adhocFeeItem.Replace("<<FEE_CODE>>", row("ProductCode").ToString.Trim)
                        adhocFeeItem = adhocFeeItem.Replace("<<FEE_DESCRIPTION>>", adHocFeeDescription)
                        adhocFeeItem = adhocFeeItem.Replace("<<CUSTOMER_NUMBER>>", row("CustomerNo").ToString.Trim)
                        adhocFeeItem = adhocFeeItem.Replace("<<CUSTOMER_NAME>>", row("ContactForename").ToString.Trim & " " & row("ContactSurname").ToString.Trim)
                        If adHocFeeIsNegative Then
                            adhocFeeItem = adhocFeeItem.Replace("<<FEE_VALUE>>", row("Price").ToString.Trim)
                        Else
                            adhocFeeItem = adhocFeeItem.Replace("<<FEE_VALUE>>", row("Price").ToString.Trim)
                        End If
                        adHocFeesMessage.Append(adhocFeeItem)
                    Else
                        Try
                            sbFinalMessage.Append(messagePrefix)
                            If Not HtmlFormat Then
                                sbFinalMessage.Append(row("PRODUCTDESCRIPTION").ToString.Trim & _ucrDictionary.Item("FeesSeperator1") & _ucrDictionary.Item("FeesSeperator2") & row("PRICE").ToString)
                            Else
                                sbFinalMessage.Append("<tr><th scope='row'>" & row("PRODUCTDESCRIPTION").ToString.Trim & _ucrDictionary.Item("FeesSeperator1").Trim & "</th><td>" & _ucrDictionary.Item("FeesSeperator2") & row("PRICE").ToString & "</td></tr>")
                            End If
                            sbFinalMessage.Append(messageSuffix)
                        Catch ex As Exception
                        End Try
                    End If
                Next
            Catch ex As Exception
                adHocFeesMessage.Clear()
            End Try
            If HtmlFormat Then
                sbFinalMessage.Append(FeeOrderRetail(retailFee, retailFeeDescription, RetailOrderFeeTableRow))
                sbFinalMessage.Append("</table>")
            End If
        End If

        'if we have any adhoc fees add them to the return string before the normal fees
        If adHocFeesMessage.Length > 0 Then
            combinedFees = _ucrDictionary.Item("TicketingAdHocFeesHeader")
            combinedFees = combinedFees & adHocFeesMessage.ToString.Trim
            combinedFees = combinedFees & _ucrDictionary.Item("TicketingAdHocFeesFooter")
            combinedFees = combinedFees & sbFinalMessage.ToString.Trim
        Else
            combinedFees = sbFinalMessage.ToString.Trim
        End If

        Return combinedFees

    End Function

    Private Sub PopulateUcrDictionary(ByVal HtmlFormat As Boolean)

        'Is cache active i.e. are we called from from the monitor program
        If Not Utilities.IsCacheActive Then
            _ucrDictionary = Nothing
            _ucrPackageHeaders = Nothing
            _ucrPackageItems = Nothing
        End If

        If _ucrDictionary Is Nothing Then
            _ucrDictionary = New Generic.Dictionary(Of String, String)
            'To avoid calling ucr cache collection inside loop. 
            _ucrDictionary.Add("TicketingSeperator1", ucr.Content("TicketingSeperator1", Settings.Language, True))
            _ucrDictionary.Add("TicketingSeperator4", ucr.Content("TicketingSeperator4", Settings.Language, True))
            _ucrDictionary.Add("TicketingSeperator5", ucr.Content("TicketingSeperator5", Settings.Language, True))
            _ucrDictionary.Add("TicketingSeperator6", ucr.Content("TicketingSeperator6", Settings.Language, True))
            _ucrDictionary.Add("TicketingSeperator7", ucr.Content("TicketingSeperator7", Settings.Language, True))
            _ucrDictionary.Add("TicketingSeperator8", ucr.Content("TicketingSeperator8", Settings.Language, True))
            _ucrDictionary.Add("TicketingSeperator9", ucr.Content("TicketingSeperator9", Settings.Language, True))
            _ucrDictionary.Add("TicketingCarriageUpload", ucr.Content("TicketingCarriageUpload", Settings.Language, True))
            _ucrDictionary.Add("TicketingCarriagePost", ucr.Content("TicketingCarriagePost", Settings.Language, True))
            _ucrDictionary.Add("TicketingCarriageCollect", ucr.Content("TicketingCarriageCollect", Settings.Language, True))
            _ucrDictionary.Add("TicketingCarriagePrintAtHome", ucr.Content("TicketingCarriagePrintAtHome", Settings.Language, True))
            _ucrDictionary.Add("TicketingCarriageRegPost", ucr.Content("TicketingCarriageRegPost", Settings.Language, True))
            If ucr.Content("TicketingEmailCarriageReturn", Settings.Language, True).Trim = "" Then
                If HtmlFormat Then
                    _ucrDictionary.Add("TicketingEmailCarriageReturn", "<br />")
                Else
                    _ucrDictionary.Add("TicketingEmailCarriageReturn", vbCrLf)
                End If
            Else
                _ucrDictionary.Add("TicketingEmailCarriageReturn", ucr.Content("TicketingEmailCarriageReturn", Settings.Language, True))
            End If
            _ucrDictionary.Add("TicketingHead1", ucr.Content("TicketingHead1", Settings.Language, True))
            _ucrDictionary.Add("TicketingHead2", ucr.Content("TicketingHead2", Settings.Language, True))
            _ucrDictionary.Add("TicketingHead3", ucr.Content("TicketingHead3", Settings.Language, True))
            _ucrDictionary.Add("TicketingHead4", ucr.Content("TicketingHead4", Settings.Language, True))
            _ucrDictionary.Add("TicketingHead5", ucr.Content("TicketingHead5", Settings.Language, True))
            _ucrDictionary.Add("TicketingHead6", ucr.Content("TicketingHead6", Settings.Language, True))
            _ucrDictionary.Add("TicketingHead7", ucr.Content("TicketingHead7", Settings.Language, True))
            _ucrDictionary.Add("TicketingTableStyle", ucr.Attribute("TicketingTableStyle"))
            _ucrDictionary.Add("EmptyDateAndTimeValue", ucr.Content("EmptyDateAndTimeValue", Settings.Language, True))

            _ucrDictionary.Add("OrderFeesTableStyle", ucr.Attribute("OrderFeesTableStyle"))
            _ucrDictionary.Add("OrderFeesHeader", ucr.Content("OrderFeesHeader", Settings.Language, True))
            _ucrDictionary.Add("FeesSeperator1", ucr.Content("FeesSeperator1", Settings.Language, True))
            _ucrDictionary.Add("FeesSeperator2", ucr.Content("FeesSeperator2", Settings.Language, True))

            _ucrDictionary.Add("BundleSeatsHeader", ucr.Content("BundleSeatsHeader", Settings.Language, True))
            _ucrDictionary.Add("PackageSeperator1", ucr.Content("PackageSeperator1", Settings.Language, True))
            _ucrDictionary.Add("packageColumnSeparator", ucr.Content("packageColumnSeparator", Settings.Language, True))
            _ucrDictionary.Add("PackageColumnHeaders", ucr.Content("PackageColumnHeaders", Settings.Language, True).Trim)
            _ucrDictionary.Add("PackageColumnData", ucr.Content("PackageColumnData", Settings.Language, True).Trim)
            _ucrDictionary.Add("PackageDetailsLinkForEmail", ucr.Content("PackageDetailsLinkForEmail", Settings.Language, True).Trim)
            _ucrDictionary.Add("PackageDetailsLinkForEmailStyle", ucr.Content("PackageDetailsLinkForEmailStyle", Settings.Language, True).Trim)
            _ucrDictionary.Add("PackageTableStyle", ucr.Attribute("PackageTableStyle"))

            _ucrDictionary.Add("TicketingAdHocFeesHeader", ucr.Content("TicketingAdHocFeesHeader", Settings.Language, True).Trim)
            _ucrDictionary.Add("TicketingAdHocFeesRow", ucr.Content("TicketingAdHocFeesRow", Settings.Language, True).Trim)
            _ucrDictionary.Add("TicketingAdHocFeesFooter", ucr.Content("TicketingAdHocFeesFooter", Settings.Language, True).Trim)

            _ucrDictionary.Add("RovingAreaText", ucr.Content("RovingAreaText", Settings.Language, True).Trim)
            _ucrDictionary.Add("UnreservedAreaText", ucr.Content("UnreservedAreaText", Settings.Language, True).Trim)

            _totalColumns = 0
            _totalColumnData = 0
            If (_ucrDictionary.Item("PackageColumnHeaders").Length > 0) Then
                _ucrPackageHeaders = (_ucrDictionary.Item("PackageColumnHeaders")).Split(_ucrDictionary.Item("packageColumnSeparator"))
                _totalColumns = _ucrPackageHeaders.Length - 1
            End If
            If (_ucrDictionary.Item("PackageColumnData").Length > 0) Then
                _ucrPackageItems = (_ucrDictionary.Item("PackageColumnData")).Split(_ucrDictionary.Item("packageColumnSeparator"))
                _totalColumnData = _ucrPackageItems.Length - 1
            End If
        End If

    End Sub

    Public Shared Function ValidateEmailAddress(ByVal EmailAddress As String, ByVal ValidationLevel As String) As ErrorObj
        Dim err As New ErrorObj()

        If EmailAddress.Trim() <> "" Then
            '
            ' There may be one or more addresses in to To: list, for now we'll just validate the first one
            ' as this should always be the customers address. Subsequent addresses would be for customer services
            '
            Dim Address As String() = EmailAddress.Split(";")

            If Address.GetLength(0) > 0 Then

                If ValidationLevel Is Nothing Or ValidationLevel = "" Then
                    ValidationLevel = "3"
                End If

                Dim ev As New EmailValidator()
                Select Case ValidationLevel
                    Case "1"
                        ev.ValidationLevel = Atp.Net.ValidationLevel.Syntax
                    Case "2"
                        ev.ValidationLevel = Atp.Net.ValidationLevel.Lists
                    Case "3"
                        ev.ValidationLevel = Atp.Net.ValidationLevel.MailExchangeRecords
                    Case "4"
                        ev.ValidationLevel = Atp.Net.ValidationLevel.SmtpConnection
                    Case Else
                        ev.ValidationLevel = Atp.Net.ValidationLevel.Mailbox
                End Select

                Dim Result As Atp.Net.ValidationLevel = ev.Validate(Address(0))

                If Result = Atp.Net.ValidationLevel.Success Then
                    err.HasError = False
                Else
                    err.HasError = True
                    err.ErrorNumber = ValidateEmailAddressError
                    err.ErrorMessage = ev.SmtpTranscript
                End If

            Else
                err.HasError = True
                err.ErrorNumber = ValidateEmailAddressError
                err.ErrorMessage = "No Email Address specified"
            End If
        Else
            err.HasError = True
            err.ErrorNumber = ValidateEmailAddressError
            err.ErrorMessage = "No Email Address specified"
        End If

        Return err

    End Function

    Public Function FetchFailedEmails(ByVal conStr As String) As ErrorObj
        Const ModuleName As String = "FetchFailedEmails"
        Dim err As New ErrorObj

        If conStr.Trim() <> "" Then

            TalentCommonLog(ModuleName, conStr, "Talent.Common Request")
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            '-------------------------------------------------------------------------------------
            '   No Cache possible due to way the process works
            '
            Settings.ModuleName = ModuleName
            Settings.FrontEndConnectionString = conStr

            Dim dbEmail As New DBEmail()
            With dbEmail
                .SupplyNetBusinessUnit = EmailSettings.FailedEmails.SupplyNetBusinessUnit
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, conStr, ResultDataSet, err)
        End If

        Return err
    End Function
    Public Function SetFailedEmailsReported(ByVal ModuleName As String, ByVal conStr As String, ByVal FailedEmailIDs As String) As ErrorObj
        Dim err As New ErrorObj

        If conStr.Trim() <> "" Then

            TalentCommonLog(ModuleName, conStr, "Talent.Common Request")
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            '-------------------------------------------------------------------------------------
            '   No Cache possible due to way the process works
            '
            Settings.ModuleName = ModuleName
            Settings.FrontEndConnectionString = conStr

            Dim dbEmail As New DBEmail()
            With dbEmail
                .SupplyNetBusinessUnit = EmailSettings.FailedEmails.SupplyNetBusinessUnit
                .IDs = FailedEmailIDs
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, conStr, ResultDataSet, err)
        End If

        Return err
    End Function

    Private Function ReplaceTableRowContentFields(ByVal RowTemplate As String,
                                                    ByVal ProductDescription As String,
                                                    ByVal TheDate As String,
                                                    ByVal TheTime As String,
                                                    ByVal CustomerTitle As String,
                                                    ByVal CustomerForename As String,
                                                    ByVal CustomerSurname As String,
                                                    ByVal CustomerNumber As String,
                                                    ByVal CarriageMethod As String,
                                                    ByVal Price As String,
                                                    ByVal PriceBandDescription As String,
                                                    ByVal PriceCodeDescription As String,
                                                    ByVal Gate As String,
                                                    ByVal Turnstile As String,
                                                    ByVal Stand As String,
                                                    ByVal Area As String,
                                                    ByVal Row As String,
                                                    ByVal Seat As String,
                                                    ByVal Alpha As String,
                                                    ByVal SeatRestrictionCode As String,
                                                    ByVal SeatRestrictionText As String,
                                                    ByVal SeatRestrictionSeperator As String,
                                                    ByVal Quantity As String,
                                                    ByVal hideDate As Boolean,
                                                    ByVal hideTime As Boolean,
                                                    ByVal summariseRecordsOnEmail As Boolean,
                                                    ByVal bulkID As Integer,
                                                    ByVal RequestedPrice As String,
                                                    ByVal ClubHandlingFee As String,
                                                    ByVal PreviousRequestedPrice As String,
                                                    ByVal FaceValue As String,
                                                    ByVal OriginalSalePayref As String,
                                                    ByVal Earnings As String,
                                                    ByVal TicketExchangeRef As String)


        Dim RowText As New StringBuilder(RowTemplate)

        If ProductDescription Is Nothing Then ProductDescription = ""
        If TheDate Is Nothing OrElse hideDate Then TheDate = ""
        If TheTime Is Nothing OrElse hideTime Then TheTime = ""
        If CustomerTitle Is Nothing Then CustomerTitle = ""
        If CustomerForename Is Nothing Then CustomerForename = ""
        If CustomerSurname Is Nothing Then CustomerSurname = ""
        If CustomerNumber Is Nothing Then CustomerNumber = ""
        If CarriageMethod Is Nothing Then CarriageMethod = ""
        If Price Is Nothing Then Price = ""
        If (PriceBandDescription Is Nothing) AndAlso (Price Is Nothing) Then Price = ""
        If Gate Is Nothing Then Gate = ""
        If Turnstile Is Nothing Then Turnstile = ""
        If Stand Is Nothing Then Stand = ""
        If Area Is Nothing Then Area = ""
        If Row Is Nothing Then Row = ""
        If Seat Is Nothing Then Seat = ""
        If Alpha Is Nothing Then Alpha = ""
        If SeatRestrictionCode Is Nothing Then SeatRestrictionCode = ""
        If SeatRestrictionText Is Nothing Then SeatRestrictionText = ""


        ' ProductDescription holds the product description followed by product specific text (done this way because adding an extra parameter is a nightmare)
        ' Backend max length for each substitution value is 160 (becausae is held on file is T#75) so max length of product description and product specific text will be 160 
        ' A solution in the future may be to send product code to this function and get product specific text here (T#NM75 is backend) 
        Dim splitProductDescription() As String = ProductDescription.Split(";")
        If splitProductDescription.Length > 0 Then
            RowText.Replace("<<ProductDescription>>", EncodeSpecialCharacters(splitProductDescription(0)))
        End If
        If splitProductDescription.Length > 1 Then
            RowText.Replace("<<ProductSpecificText>>", EncodeSpecialCharacters(splitProductDescription(1)))
        Else
            RowText.Replace("<<ProductSpecificText>>", "")
        End If

        RowText.Replace("<<TheDate>>", TheDate)
        RowText.Replace("<<TheTime>>", TheTime)
        RowText.Replace("<<CustomerTitle>>", CustomerTitle)
        RowText.Replace("<<CustomerForename>>", CustomerForename)
        RowText.Replace("<<CustomerSurname>>", CustomerSurname)
        RowText.Replace("<<CustomerNumber>>", CustomerNumber)
        RowText.Replace("<<CarriageMethod>>", CarriageMethod)
        RowText.Replace("<<Price>>", Price)
        RowText.Replace("<<TicketType>>", EncodeSpecialCharacters(PriceBandDescription))
        RowText.Replace("<<PriceCodeDescription>>", EncodeSpecialCharacters(PriceCodeDescription))
        RowText.Replace("<<RequestedPrice>>", RequestedPrice)
        RowText.Replace("<<ClubHandlingFee>>", ClubHandlingFee)
        RowText.Replace("<<PreviousRequestedPrice>>", PreviousRequestedPrice)
        RowText.Replace("<<FaceValue>>", FaceValue)
        RowText.Replace("<<OriginalSalePayref>>", OriginalSalePayref)
        RowText.Replace("<<Earnings>>", Earnings)
        RowText.Replace("<<TicketExchangeRef>>", TicketExchangeRef)


        'When a quantity is given, the seat details cannot be displayed.
        'Quantity will be needed when summariseRecordsOnEmail is true or if there is a bulk sales ID
        'If a quantity is not given, the placeholder needs to be blanked out.
        If (Quantity.Length > 0 AndAlso summariseRecordsOnEmail) OrElse (Quantity.Length > 0 AndAlso bulkID > 0) Then
            RowText.Replace("<<Quantity>>", Quantity & "&nbsp;x&nbsp;")
            RowText.Replace("<<Gate>>", String.Empty)
            RowText.Replace("<<Turnstile>>", String.Empty)
            RowText.Replace("<<Stand>>", String.Empty)
            RowText.Replace("<<Area>>", String.Empty)
            RowText.Replace("<<Row>>", String.Empty)
            RowText.Replace("<<Seat>>", String.Empty)
            RowText.Replace("<<Alpha>>", String.Empty)
            RowText.Replace("<<SeatRestrictionCode>>", String.Empty)
        Else
            RowText.Replace("<<Quantity>>", String.Empty)
            RowText.Replace("<<Gate>>", Gate)
            RowText.Replace("<<Turnstile>>", Turnstile)
            RowText.Replace("<<Stand>>", Stand)
            RowText.Replace("<<Area>>", Area)
            RowText.Replace("<<Row>>", Row)
            RowText.Replace("<<Seat>>", Seat)
            RowText.Replace("<<Alpha>>", Alpha)
            RowText.Replace("<<SeatRestrictionCode>>", SeatRestrictionCode)
        End If

        If SeatRestrictionText = "" Then
            RowText.Replace("<<SeatRestrictionText>>", EncodeSpecialCharacters(SeatRestrictionText))
        Else
            RowText.Replace("<<SeatRestrictionText>>", SeatRestrictionSeperator & EncodeSpecialCharacters(SeatRestrictionText))
        End If

        Return RowText.ToString()

    End Function

    Private Function ReplaceRetailTableRowContentFields(ByVal RowTemplate As String,
                                                    ByVal ProductDescription As String,
                                                    ByVal Quantity As String,
                                                    ByVal ProductCode As String,
                                                    ByVal GrossPrice As String,
                                                    ByVal Instructions As String) As String

        Dim RowText As New StringBuilder(RowTemplate)

        RowText.Replace("<<ProductDescription>>", ProductDescription)
        RowText.Replace("<<Quantity>>", Quantity)
        RowText.Replace("<<ProductCode>>", ProductCode)
        RowText.Replace("<<GrossPrice>>", GrossPrice)
        RowText.Replace("<<Instructions>>", Instructions)

        Return RowText.ToString()

    End Function

    Private Sub RetrieveEmailTemplateDetails(ByVal sEmailType As String, ByRef ErrObj As Talent.Common.ErrorObj)

        ' Determine if a specific template ID is to be used.
        Dim sEmailTemplateID As String = _emailSettings.EmailTemplateID
        If sEmailTemplateID.Trim.Length > 0 Then
            If IsNumeric(sEmailTemplateID) Then
                _emailTemplateID = CType(sEmailTemplateID, Integer)
            End If
        End If

        ' Retrieve all templates and use either the active template for this type of email, or the specific template if an ID has been specified.
        Dim _dsemailTemplatesDefinition As DataSet = Nothing
        Dim _dtemailTemplatesDefinition As DataTable = Nothing
        TDataObjects.Settings.BusinessUnit = Settings.BusinessUnit
        TDataObjects.Settings.Partner = Settings.Partner
        _dsemailTemplatesDefinition = TDataObjects.EmailTemplateSettings.TblEmailTemplates.GetByBUAndPartner()
        If Not _dsemailTemplatesDefinition Is Nothing AndAlso _dsemailTemplatesDefinition.Tables.Count > 0 Then
            _dtemailTemplatesDefinition = _dsemailTemplatesDefinition.Tables(0)
            If _dtemailTemplatesDefinition.Rows.Count > 0 Then
                For Each dr As Data.DataRow In _dtemailTemplatesDefinition.Rows
                    If (dr("TEMPLATE_TYPE").ToString.Trim.ToLower = sEmailType.Trim.ToLower) Then
                        If _emailTemplateID > 0 Then
                            If dr("EMAILTEMPLATE_ID") = _emailTemplateID Then
                                If dr("Active") = True Then
                                    _emailHTML = dr("EMAIL_HTML")
                                    _emailFromAddress = dr("EMAIL_FROM_ADDRESS")
                                    _emailSubject = dr("EMAIL_SUBJECT")
                                    _emailBody = dr("EMAIL_BODY")
                                Else
                                    Dim masterTemplateDataRow As DataRow() = _dtemailTemplatesDefinition.Select("TEMPLATE_TYPE ='" & sEmailType & "' AND MASTER = True")
                                    If Not masterTemplateDataRow Is Nothing Then
                                        _emailTemplateID = masterTemplateDataRow(0).Item("EMAILTEMPLATE_ID")
                                        _emailHTML = masterTemplateDataRow(0).Item("EMAIL_HTML")
                                        _emailFromAddress = masterTemplateDataRow(0).Item("EMAIL_FROM_ADDRESS")
                                        _emailSubject = masterTemplateDataRow(0).Item("EMAIL_SUBJECT")
                                        _emailBody = masterTemplateDataRow(0).Item("EMAIL_BODY")
                                    End If
                                End If
                                Exit For
                            End If
                        Else
                            If dr("MASTER") = True Then
                                _emailTemplateID = dr("EMAILTEMPLATE_ID")
                                _emailHTML = dr("EMAIL_HTML")
                                _emailFromAddress = dr("EMAIL_FROM_ADDRESS")
                                _emailSubject = dr("EMAIL_SUBJECT")
                                _emailBody = dr("EMAIL_BODY")
                                Exit For
                            End If
                        End If
                    End If
                Next
            End If
        Else
            ErrObj.HasError = True
            ErrObj.ErrorMessage = "Error retrieving email templates in RetrieveEmailTemplateDetails. (BU=" + Settings.BusinessUnit.ToString + ", Partner=" + Settings.Partner + ")"
            ErrObj.ErrorNumber = "TACTEMAIL-14a"
        End If

        ' General error handling
        If _emailTemplateID = 0 And Not ErrObj.HasError Then
            ErrObj.HasError = True
            ErrObj.ErrorMessage = "Error retrieving email template ID in RetrieveEmailTemplateDetails. (sEmailType=" + sEmailType.ToString + ")"
            ErrObj.ErrorNumber = "TACTEMAIL-14b"
        End If
        If _emailBody = String.Empty And Not ErrObj.HasError Then
            ErrObj.HasError = True
            ErrObj.ErrorMessage = "Error retrieving email template body in RetrieveEmailTemplateDetails. (_emailTemplateID=" + _emailTemplateID.ToString + ")"
            ErrObj.ErrorNumber = "TACTEMAIL-14c"
        End If
        If _emailFromAddress = String.Empty And Not ErrObj.HasError Then
            ErrObj.HasError = True
            ErrObj.ErrorMessage = "Error retrieving email template body in RetrieveEmailTemplateDetails. (_emailTemplateID=" + _emailTemplateID.ToString + ")"
            ErrObj.ErrorNumber = "TACTEMAIL-14d"
        End If


        ' Set the From Address
        If _emailFromAddress.Trim.Length > 0 Then EmailSettings.FromAddress = _emailFromAddress


        ' Determine if no data is to be retrieved and merged, i.e. this is a test email being sent.
        Select Case sEmailType.Trim.ToLower
            Case Is = GlobalConstants.EMAIL_ORDER_CONFIRMATION.ToLower
                If EmailSettings.OrderConfirmation.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_TICKETING_UPGRADE.ToLower
                If EmailSettings.OrderConfirmation.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_TICKETING_TRANSFER.ToLower
                If EmailSettings.OrderConfirmation.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_TICKETING_CANCEL.ToLower
                If EmailSettings.OrderConfirmation.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_ORDER_RETURN_CONFIRM.ToLower
                If EmailSettings.OrderReturnConfirmation.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_FORGOTTEN_PASSWORD.ToLower
                If EmailSettings.ForgottenPassword.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_CHANGE_PASSWORD.ToLower
                If EmailSettings.ChangePassword.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_CUSTOMER_REGISTRATION.ToLower
                If EmailSettings.CustomerRegistration.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_PPS_PAYMENT_CONFIRMATION.ToLower
                If EmailSettings.PPSPayment.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_PPS_PAYMENT_FAILURE.ToLower
                If EmailSettings.PPSPayment.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_PPS_AMEND.ToLower
                If EmailSettings.AmendPPS.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_PPS_AMEND_PAYMENT.ToLower
                If EmailSettings.PPSPayment.Customer = "null" Then _emailNoDataRetrieval = True
            Case Is = GlobalConstants.EMAIL_HOSPITALITY_Q_AND_A_REMINDER.ToLower
                If EmailSettings.HospitalityQAReminderEmail.Customer = "null" Then _emailNoDataRetrieval = True
        End Select

    End Sub

    Private Function RemoveMergeBrackets(ByVal sStr As String) As String
        Dim ret As String = sStr

        If ret.IndexOf("<<<") > 0 Then
            ret = ret.Replace("<<<", "[[[")
            ret = ret.Replace(">>>", "]]]")
        Else
            ret = ret.Replace("<<", "[[")
            ret = ret.Replace(">>", "]]")
        End If

        Return ret
    End Function
    Private Function InsertMergeBrackets(ByVal sStr As String) As String
        Dim ret As String = sStr

        If ret.IndexOf("[[[") > 0 Then
            ret = ret.Replace("[[[", "<<<")
            ret = ret.Replace("]]]", ">>>")
        Else
            ret = ret.Replace("[[", "<<")
            ret = ret.Replace("]]", ">>")
        End If

        Return ret
    End Function

    Private Function GetBundleProductKeys(ByVal purchasedItems() As DataRow) As Generic.List(Of String)
        Dim keyList As New Generic.List(Of String)
        If purchasedItems.Length > 0 Then
            For Each row As DataRow In purchasedItems
                If Not String.IsNullOrEmpty(row("RelatingBundleProduct")) Then
                    Dim keyString As String = row("RelatingBundleProduct") & "," & row("RelatingBundleSeat")
                    If Not keyList.Contains(keyString) Then
                        keyList.Add(keyString)
                    End If
                End If
            Next
        End If
        Return keyList
    End Function

    Private Function GetProductDescriptionFromProductCode(ByRef dtPurchaseItems As DataTable, ByVal productCode As String) As String
        Dim productDescription As String = String.Empty
        For Each row As DataRow In dtPurchaseItems.Rows
            If row("ProductCode").ToString().Trim() = productCode Then
                productDescription = row("ProductDescription").ToString().Trim()
                Exit For
            End If
        Next
        Return productDescription
    End Function
    ''' <summary>
    ''' Adds a client address to order confirmation email
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AddClientAddressToOrderConfirmation(ByVal SendOrderConfirmationToClient As Boolean)
        If SendOrderConfirmationToClient Then
            Dim customerServiceEmail As String = Utilities.CheckForDBNull_String(TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(Settings.BusinessUnit, "CUSTOMER_SERVICES_EMAIL"))
            If Not String.IsNullOrEmpty(customerServiceEmail) Then
                EmailSettings.ToAddress = EmailSettings.ToAddress & ";" & customerServiceEmail
            End If
        End If
    End Sub

    Private Function getPaymentReferenceAsImage(ByVal paymentReference As String) As String
        Dim QRCodeURLTemplate As String = Utilities.CheckForDBNull_String(ucr.Attribute("QRCodeURLTemplate"))
        Return Replace(QRCodeURLTemplate, "<<<PaymentReference>>>", paymentReference)
    End Function

    Private Function populateDDStatusDescriptions() As Generic.Dictionary(Of String, String)
        Dim err As New ErrorObj
        Dim ddStatusDictionary As New Generic.Dictionary(Of String, String)
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities

        utilitites.DescriptionKey = "BAST"
        utilitites.Settings = Settings

        err = utilitites.RetrieveDescriptionEntries()

        If Not err.HasError AndAlso Not utilitites.ResultDataSet Is Nothing AndAlso utilitites.ResultDataSet.Tables.Count > 1 Then
            For Each dr As DataRow In utilitites.ResultDataSet.Tables(1).Rows
                ddStatusDictionary.Add(dr.Item("Code").ToString.Trim, dr.Item("Description").ToString.Trim)
            Next
        End If

        Return ddStatusDictionary
    End Function

    Private Function getPriceWithFormattedCurrency(ByVal price As String) As String
        Return TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(price), Settings.BusinessUnit, Settings.Partner)
    End Function
End Class