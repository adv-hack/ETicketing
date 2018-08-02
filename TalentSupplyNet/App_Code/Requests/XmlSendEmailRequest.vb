Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Send Emails Requests
'
'       Date                        Nov 2010
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlSendEmailRequest
        Inherits XmlRequest

        Private Structure EmailTypes
            Public Const SendTicketingConfirmation As String = "SendTicketingConfirmation"
            Public Const TicketExchangeSaleConfirmation As String = "TicketExchangeSaleConfirmation"
            Public Const CustomerRegistration As String = "CustomerRegistration"
            Public Const PPSPaymentConfirmation As String = "PPSPaymentConfirmation"
            Public Const PPSPaymentFailure As String = "PPSPaymentFailure"
            Public Const BuybackSeatRebook As String = "BuybackSeatRebook"
            Public Const BuybackConfirmation As String = "BuybackConfirmation"
            Public Const TicketingCancel As String = "TicketingCancel"
            Public Const TicketingTransfer As String = "TicketingTransfer"
            Public Const TicketingUpgrade As String = "TicketingUpgrade"
        End Structure
        Private EmailType As String = ""

        Private _customerIDFromXmlV1_1 As String = String.Empty
        Private MailSettings As New DEEmailSettings()

        Public Sub New(ByVal webserviceName As String)
            MyBase.New(webserviceName)
        End Sub

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlSendEmailResponse = CType(xmlResp, XmlSendEmailResponse)
            Dim err As ErrorObj = Nothing

            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            '-------------------------------
            ' Place the Request if no errors
            '-------------------------------

            If err.HasError Then
                xmlResp.Err = err
            Else
                If _customerIDFromXmlV1_1.Length > 0 Then
                    Settings.LoginId = _customerIDFromXmlV1_1
                    _customerIDFromXmlV1_1 = String.Empty
                End If


                Dim talEmail As New TalentEmail
                Dim tdo As New Talent.Common.TalentDataObjects()
                tdo.Settings = Settings

                Select Case EmailType
                    Case EmailTypes.SendTicketingConfirmation
                        Dim xmlDoc As String = talEmail.CreateTicketingXmlDocument(MailSettings.FromAddress, _
                                                    MailSettings.ToAddress, "", _
                                                    MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, "", _
                                                    MailSettings.OrderConfirmation.PaymentReference, MailSettings.OrderConfirmation.Customer, _
                                                    Settings.OriginatingSourceCode)
                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "TicketingConfirmation", xmlDoc, "")


                    Case EmailTypes.CustomerRegistration
                        Dim xmlDoc As String = talEmail.CreateCustomerRegistrationXmlDocument(MailSettings.FromAddress, _
                                                        MailSettings.ToAddress, _
                                                        MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, _
                                                        MailSettings.CustomerRegistration.Customer, _
                                                        MailSettings.CustomerRegistration.WebsiteAddress)

                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "CustomerRegistration", xmlDoc, "")


                    Case EmailTypes.PPSPaymentConfirmation
                        Dim xmlDoc As String = talEmail.CreatePPSPaymentXmlDocument(MailSettings.FromAddress, _
                                                        MailSettings.ToAddress, _
                                                        MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, _
                                                        MailSettings.PPSPayment.Customer, _
                                                        MailSettings.PPSPayment.Description, _
                                                        MailSettings.PPSPayment.Turnstiles, _
                                                        MailSettings.PPSPayment.Gates, _
                                                        MailSettings.PPSPayment.Seat, _
                                                        MailSettings.PPSPayment.PaymentValue)

                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "PPSPaymentConfirmation", xmlDoc, "")

                    Case EmailTypes.PPSPaymentFailure
                        Dim xmlDoc As String = talEmail.CreatePPSPaymentXmlDocument(MailSettings.FromAddress, _
                                                        MailSettings.ToAddress, _
                                                        MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, _
                                                        MailSettings.PPSPayment.Customer, _
                                                        MailSettings.PPSPayment.Description, _
                                                        MailSettings.PPSPayment.Turnstiles, _
                                                        MailSettings.PPSPayment.Gates, _
                                                        MailSettings.PPSPayment.Seat, _
                                                        MailSettings.PPSPayment.PaymentValue)

                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "PPSPaymentFailure", xmlDoc, "")

                    Case EmailTypes.BuybackSeatRebook
                        Dim xmlDoc As String = talEmail.CreateOrderReturnConfirmationXmlDocument(MailSettings.FromAddress, _
                                                        MailSettings.ToAddress, _
                                                        MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, _
                                                        MailSettings.OrderReturnConfirmation.Customer, _
                                                        MailSettings.OrderReturnConfirmation.OrderReturnReference, _
                                                        MailSettings.OrderReturnConfirmation.Mode, _
                                                        Settings.OriginatingSourceCode)

                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "OrderReturnConfirmation", xmlDoc, "")

                    Case EmailTypes.BuybackConfirmation
                        Dim xmlDoc As String = talEmail.CreateOrderReturnConfirmationXmlDocument(MailSettings.FromAddress, _
                                                        MailSettings.ToAddress, _
                                                        MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, _
                                                        MailSettings.OrderReturnConfirmation.Customer, _
                                                        MailSettings.OrderReturnConfirmation.OrderReturnReference, _
                                                        MailSettings.OrderReturnConfirmation.Mode, _
                                                        Settings.OriginatingSourceCode)

                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "OrderReturnConfirmation", xmlDoc, "")


                    Case EmailTypes.TicketingCancel, EmailTypes.TicketingTransfer, EmailTypes.TicketingUpgrade
                        Dim xmlDoc As String = talEmail.CreateTicketingXmlDocument(MailSettings.FromAddress, _
                                                    MailSettings.ToAddress, "", _
                                                    MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, "", _
                                                    MailSettings.OrderConfirmation.PaymentReference, MailSettings.OrderConfirmation.Customer, _
                                                    Settings.OriginatingSourceCode)
                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", EmailType.ToString(), xmlDoc, "")


                    Case EmailTypes.TicketExchangeSaleConfirmation
                        Dim xmlDoc As String = talEmail.CreateTicketExchangeSaleXmlDocument(MailSettings.FromAddress,
                                                    MailSettings.ToAddress, "",
                                                    MailSettings.SmtpServer, MailSettings.SmtpServerPort, Settings.Partner, "",
                                                    MailSettings.TicketExchangeConfirmation.TicketExchangeReference, MailSettings.TicketExchangeConfirmation.Customer,
                                                    Settings.OriginatingSourceCode)
                        tdo.AppVariableSettings.TblOfflineProcessing.Insert(Settings.BusinessUnit, "*ALL", "Pending", 0, "", _
                                                                            "EmailMonitor", "TicketExchangeSaleConfirmation", xmlDoc, "")



                End Select

                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = Nothing
            xmlAction.SenderID = Settings.SenderID
            xmlAction.CreateResponse()

            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dep = New DEOrder
            MailSettings.SmtpServer = ConfigurationManager.AppSettings("EmailSMTP")
            MailSettings.SmtpServerPort = ConfigurationManager.AppSettings("EmailSMTPPort")

            Try
                For Each SendEmailRequestNode As XmlNode In xmlDoc.SelectSingleNode("//SendEmailRequest").ChildNodes
                    Select Case SendEmailRequestNode.Name
                        Case "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(SendEmailRequestNode))

                        Case "SendEmail"

                            For Each SendEmailNode As XmlNode In SendEmailRequestNode.ChildNodes

                                Select Case SendEmailNode.Name
                                    Case "FromAddress"
                                        MailSettings.FromAddress = SendEmailNode.InnerText

                                    Case "ToAddress"
                                        MailSettings.ToAddress = SendEmailNode.InnerText

                                    Case "EmailType"
                                        EmailType = SendEmailNode.InnerText

                                        Select Case EmailType
                                            Case EmailTypes.SendTicketingConfirmation
                                                MailSettings.OrderConfirmation.WebsiteURL = GetCurrentApplicationUrl()

                                            Case EmailTypes.CustomerRegistration
                                                MailSettings.CustomerRegistration.WebsiteAddress = GetCurrentApplicationUrl() & _
                                                                                                   "/PagesPublic/Home/Home.aspx"

                                            Case EmailTypes.BuybackSeatRebook
                                                MailSettings.OrderReturnConfirmation.Mode = "2"

                                            Case EmailTypes.BuybackConfirmation
                                                MailSettings.OrderReturnConfirmation.Mode = "1"

                                        End Select


                                    Case "Parameters"

                                        For Each ParametersNode As XmlNode In SendEmailNode.ChildNodes

                                            Select Case ParametersNode.Name
                                                Case "CustomerNumber"
                                                    Select Case EmailType
                                                        Case EmailTypes.SendTicketingConfirmation
                                                            MailSettings.OrderConfirmation.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.CustomerRegistration
                                                            MailSettings.CustomerRegistration.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentConfirmation
                                                            MailSettings.PPSPayment.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentFailure
                                                            MailSettings.PPSPayment.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.BuybackSeatRebook
                                                            MailSettings.OrderReturnConfirmation.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.TicketingCancel, EmailTypes.TicketingTransfer, EmailTypes.TicketingUpgrade
                                                            MailSettings.OrderConfirmation.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.BuybackConfirmation
                                                            MailSettings.OrderReturnConfirmation.Customer = ParametersNode.InnerText

                                                        Case EmailTypes.TicketExchangeSaleConfirmation
                                                            MailSettings.TicketExchangeConfirmation.Customer = ParametersNode.InnerText

                                                    End Select

                                                Case "PaymentReference"
                                                    Select Case EmailType
                                                        Case EmailTypes.SendTicketingConfirmation
                                                            MailSettings.OrderConfirmation.PaymentReference = ParametersNode.InnerText

                                                        Case EmailTypes.TicketingCancel, EmailTypes.TicketingTransfer, EmailTypes.TicketingUpgrade
                                                            MailSettings.OrderConfirmation.PaymentReference = ParametersNode.InnerText

                                                        Case EmailTypes.TicketExchangeSaleConfirmation
                                                            MailSettings.TicketExchangeConfirmation.TicketExchangeReference = ParametersNode.InnerText
                                                    End Select

                                                Case "ProductDescription"
                                                    Select Case EmailType
                                                        Case EmailTypes.PPSPaymentConfirmation
                                                            MailSettings.PPSPayment.Description = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentFailure
                                                            MailSettings.PPSPayment.Description = ParametersNode.InnerText

                                                    End Select

                                                Case "Turnstiles"
                                                    Select Case EmailType
                                                        Case EmailTypes.PPSPaymentConfirmation
                                                            MailSettings.PPSPayment.Turnstiles = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentFailure
                                                            MailSettings.PPSPayment.Turnstiles = ParametersNode.InnerText

                                                    End Select

                                                Case "Gates"
                                                    Select Case EmailType
                                                        Case EmailTypes.PPSPaymentConfirmation
                                                            MailSettings.PPSPayment.Gates = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentFailure
                                                            MailSettings.PPSPayment.Gates = ParametersNode.InnerText

                                                    End Select

                                                Case "Seat"
                                                    Select Case EmailType
                                                        Case EmailTypes.PPSPaymentConfirmation
                                                            MailSettings.PPSPayment.Seat = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentFailure
                                                            MailSettings.PPSPayment.Seat = ParametersNode.InnerText

                                                    End Select

                                                Case "PaymentValue"
                                                    Select Case EmailType
                                                        Case EmailTypes.PPSPaymentConfirmation
                                                            MailSettings.PPSPayment.PaymentValue = ParametersNode.InnerText

                                                        Case EmailTypes.PPSPaymentFailure
                                                            MailSettings.PPSPayment.PaymentValue = ParametersNode.InnerText

                                                    End Select

                                                Case "BuybackGroupId"
                                                    Select Case EmailType
                                                        Case EmailTypes.BuybackSeatRebook
                                                            MailSettings.OrderReturnConfirmation.OrderReturnReference = ParametersNode.InnerText

                                                        Case EmailTypes.BuybackConfirmation
                                                            MailSettings.OrderReturnConfirmation.OrderReturnReference = ParametersNode.InnerText

                                                    End Select

                                            End Select
                                        Next ParametersNode

                                End Select

                            Next SendEmailNode
                    End Select
                Next SendEmailRequestNode

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOR-01"
                    .HasError = True
                End With
            End Try

            Return err

        End Function

        Private Function GetCurrentApplicationUrl() As String

            Dim s As String = HttpContext.Current.Request.Url.AbsoluteUri.Remove(HttpContext.Current.Request.Url.AbsoluteUri.LastIndexOf("/"))

            s = s.Remove(s.LastIndexOf("/"))        ' Remove the web service name
            Return s.Remove(s.LastIndexOf(":"))     ' Remove the port number


            'Dim relativeUrl As String = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.TrimStart("~")
            'Dim currentUrl As String = HttpContext.Current.Request.Url.AbsolutePath.Split("?")(0)

            'Return currentUrl.Remove(currentUrl.Length - relativeUrl.Length, relativeUrl.Length)
        End Function

    End Class

End Namespace