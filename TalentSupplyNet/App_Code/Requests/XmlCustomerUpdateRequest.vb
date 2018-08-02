Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Customer Update Requests
'
'       Date                        13/02/09
'
'       Author                      Ben    
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQCU- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerUpdateRequest
        Inherits XmlRequest
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        Private _decust As New DECustomerV11
        Public Property Decust() As DECustomerV11
            Get
                Return _decust
            End Get
            Set(ByVal value As DECustomerV11)
                _decust = value
            End Set
        End Property
        Private _useEncryptedPassword As Boolean
        Public Property UseEncryptedPassword() As Boolean
            Get
                Return _useEncryptedPassword
            End Get
            Set(ByVal value As Boolean)
                _useEncryptedPassword = value
            End Set
        End Property

        Private _encryptionType As String
        Public Property EncryptionType() As String
            Get
                Return _encryptionType
            End Get
            Set(ByVal value As String)
                _encryptionType = value
            End Set
        End Property
        Private _saltString As String
        Public Property SaltString() As String
            Get
                Return _saltString
            End Get
            Set(ByVal value As String)
                _saltString = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlCustomerUpdateResponse = CType(xmlResp, XmlCustomerUpdateResponse)
            Dim err As ErrorObj = Nothing

            '-------------------------------------------
            ' Check whether need to hash password or not
            '-------------------------------------------
            Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
            Dim hashedPassword As String = String.Empty

            Try
                UseEncryptedPassword = CBool(def.GetDefault("USE_ENCRYPTED_PASSWORD"))
            Catch
            End Try
            Try
                EncryptionType = def.GetDefault("ENCRYPTION_TYPE")
            Catch
            End Try
            Try
                SaltString = def.GetDefault("CLIENT_SALT")
            Catch
            End Try
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

                Case Is = "1.1"
                    err = LoadXmlV1_1()
            End Select

            '-----------------
            ' Test attributes
            '----------------
            '  Decust.Attribute01 = "RANK-1"
            '  Decust.Attribute01Action = "D"
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim CUSTOMER As New TalentCustomer
            If err.HasError Then
                xmlResp.Err = err
            Else
                With CUSTOMER
                    Select Case MyBase.DocumentVersion
                        Case Is = "1.0", "1.1"
                            .DeV11 = Decust
                            .Settings = Settings
                            err = .SetCustomer
                    End Select
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If
            xmlAction.ResultDataSet = CUSTOMER.ResultDataSet
            xmlAction.DestinationDatabase = CUSTOMER.Settings.DestinationDatabase
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_2"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            Dim count As Integer = 0
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                Dim businessUnit As String = String.Empty

                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerUpdateRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Defaults"
                            For Each n As XmlNode In Node1.ChildNodes
                                Select Case n.Name
                                    Case "BusinessUnit"
                                        businessUnit = n.InnerText
                                End Select
                            Next
                        Case Is = "CustomerUpdate"
                            With Decust
                                .BusinessUnit = businessUnit
                                For Each sitesNode As XmlNode In Node1.ChildNodes

                                    For Each site As XmlNode In sitesNode.ChildNodes
                                        Dim newSite As New DECustomerV11.DECustomerSite
                                        Decust.Sites.Add(newSite)
                                        newSite.UpdateMode = site.Attributes("Mode").Value

                                        For Each sNode As XmlNode In site.ChildNodes
                                            Select Case sNode.Name

                                                Case "Name"
                                                    newSite.Name = sNode.InnerText
                                                    newSite.ProcessName = "1"
                                                Case "AccountNumber1"
                                                    newSite.AccountNumber1 = sNode.InnerText
                                                    newSite.ProcessAccountNumber1 = "1"
                                                Case "AccountNumber2"
                                                    newSite.AccountNumber2 = sNode.InnerText
                                                    newSite.ProcessAccountNumber2 = "1"
                                                Case "AccountNumber3"
                                                    newSite.AccountNumber3 = sNode.InnerText
                                                    newSite.ProcessAccountNumber3 = "1"
                                                Case "AccountNumber4"
                                                    newSite.AccountNumber4 = sNode.InnerText
                                                    newSite.ProcessAccountNumber4 = "1"
                                                Case "AccountNumber5"
                                                    newSite.AccountNumber5 = sNode.InnerText
                                                    newSite.ProcessAccountNumber5 = "1"

                                                Case "Address"
                                                    Dim newAddress As New DECustomerV11.DECustomerSiteAddress
                                                    newSite.Address = newAddress
                                                    newSite.ProcessAddress = "1"
                                                    newAddress.UpdateMode = newSite.UpdateMode

                                                    For Each aNode As XmlNode In sNode.ChildNodes
                                                        Select Case aNode.Name
                                                            Case "Line1"
                                                                newAddress.Line1 = aNode.InnerText
                                                            Case "Line2"
                                                                newAddress.Line2 = aNode.InnerText
                                                            Case "Line3"
                                                                newAddress.Line3 = aNode.InnerText
                                                            Case "Line4"
                                                                newAddress.Line4 = aNode.InnerText
                                                            Case "Line5"
                                                                newAddress.Line5 = aNode.InnerText
                                                            Case "PostCode"
                                                                newAddress.Postcode = aNode.InnerText
                                                            Case "Country"
                                                                newAddress.Country = aNode.InnerText
                                                        End Select
                                                    Next

                                                Case "TelephoneNumber"
                                                    newSite.TelephoneNumber = sNode.InnerText
                                                    newSite.ProcessTelephoneNumber = "1"
                                                Case "FaxNumber"
                                                    newSite.FaxNumber = sNode.InnerText
                                                    newSite.ProcessFaxNumber = "1"
                                                Case "VATNumber"
                                                    newSite.VATNumber = sNode.InnerText
                                                    newSite.ProcessVATNumber = "1"
                                                Case "URL"
                                                    newSite.URL = sNode.InnerText
                                                    newSite.ProcessURL = "1"
                                                Case "ID"
                                                    newSite.ID = sNode.InnerText
                                                    newSite.ProcessID = "1"
                                                Case "CRMBranch"
                                                    newSite.CRMBranch = sNode.InnerText
                                                    newSite.ProcessCRMBranch = "1"
                                                Case "Contacts"

                                                    For Each cNode As XmlNode In sNode.ChildNodes
                                                        Dim newContact As New DECustomerV11.DECustomerSiteContact
                                                        newSite.Contacts.Add(newContact)
                                                        newContact.UpdateMode = cNode.Attributes("Mode").Value

                                                        Dim contactV1 As New DECustomer
                                                        .DECustomersV1.Add(contactV1)
                                                        contactV1.UseOptionalFields = "Y"
                                                        contactV1.UseEncryptedPassword = UseEncryptedPassword
                                                        contactV1.EncryptionType = EncryptionType
                                                        contactV1.CompanyName = newSite.Name
                                                        contactV1.ProcessCompanyName = "1"

                                                        For Each conNode As XmlNode In cNode.ChildNodes
                                                            Select Case conNode.Name
                                                                Case "Title"
                                                                    newContact.Title = conNode.InnerText
                                                                    newContact.ProcessTitle = "1"
                                                                    contactV1.ContactTitle = conNode.InnerText
                                                                    contactV1.ProcessContactTitle = "1"

                                                                Case "Initials"
                                                                    newContact.Initials = conNode.InnerText
                                                                    newContact.ProcessInitials = "1"
                                                                    contactV1.ContactInitials = conNode.InnerText
                                                                    contactV1.ProcessContactInitials = "1"

                                                                Case "Forename"
                                                                    newContact.Forename = conNode.InnerText
                                                                    newContact.ProcessForeName = "1"
                                                                    contactV1.ContactForename = conNode.InnerText
                                                                    contactV1.ProcessContactForename = "1"

                                                                Case "Surname"
                                                                    newContact.Surname = conNode.InnerText
                                                                    newContact.ProcessSurname = "1"
                                                                    contactV1.ContactSurname = conNode.InnerText
                                                                    contactV1.ProcessContactSurname = "1"

                                                                Case "FullName"
                                                                    newContact.FullName = conNode.InnerText
                                                                    newContact.ProcessFullName = "1"
                                                                Case "Salutation"
                                                                    newContact.Salutation = conNode.InnerText
                                                                    newContact.ProcessSalutation = "1"
                                                                    contactV1.Salutation = conNode.InnerText
                                                                    contactV1.ProcessSalutation = "1"

                                                                Case "EmailAddress"
                                                                    newContact.EmailAddress = conNode.InnerText
                                                                    newContact.ProcessEmailAddress = "1"
                                                                    contactV1.EmailAddress = conNode.InnerText
                                                                    contactV1.ProcessEmailAddress = "1"

                                                                Case "LoginID"
                                                                    newContact.LoginID = conNode.InnerText
                                                                    newContact.ProcessLoginID = "1"
                                                                    contactV1.CustomerNumber = conNode.InnerText
                                                                    contactV1.ProcessCustomerNumber = "1"
                                                                    contactV1.ProcessLoginID = "1"
                                                                Case "Password"
                                                                    newContact.Password = conNode.InnerText
                                                                    newContact.ProcessPassword = "1"
                                                                    contactV1.Password = conNode.InnerText
                                                                    contactV1.ProcessPassword = "1"
                                                                    If UseEncryptedPassword Then
                                                                        Dim passHash As New PasswordHash()
                                                                        contactV1.NewHashedPassword = passHash.HashSalt(conNode.InnerText, SaltString)
                                                                    End If
                                                                Case "AccountNumber1"
                                                                    newContact.AccountNumber1 = conNode.InnerText
                                                                    newContact.ProcessAccountNumber1 = "1"
                                                                Case "AccountNumber2"
                                                                    newContact.AccountNumber2 = conNode.InnerText
                                                                    newContact.ProcessAccountNumber2 = "1"
                                                                Case "AccountNumber3"
                                                                    newContact.AccountNumber3 = conNode.InnerText
                                                                    newContact.ProcessAccountNumber3 = "1"
                                                                Case "AccountNumber4"
                                                                    newContact.AccountNumber4 = conNode.InnerText
                                                                    newContact.ProcessAccountNumber4 = "1"
                                                                Case "AccountNumber5"
                                                                    newContact.AccountNumber5 = conNode.InnerText
                                                                    newContact.ProcessAccountNumber5 = "1"
                                                                Case "Addresses"
                                                                    For Each aNode As XmlNode In conNode.ChildNodes
                                                                        Dim newAddress As New DECustomerV11.DECustomerSiteAddress
                                                                        newContact.Addresses.Add(newAddress)
                                                                        newAddress.UpdateMode = aNode.Attributes("Mode").Value

                                                                        For Each adNode As XmlNode In aNode.ChildNodes
                                                                            Select Case adNode.Name
                                                                                Case "SequenceNumber"
                                                                                    newAddress.SequenceNumber = adNode.InnerText
                                                                                    newAddress.ProcessSequenceNumber = "1"
                                                                                Case "Default"

                                                                                    Select Case adNode.InnerText.ToUpper
                                                                                        Case "Y", "TRUE", "1", "YES"
                                                                                            newAddress.IsDefault = True
                                                                                        Case Else
                                                                                            newAddress.IsDefault = False
                                                                                    End Select
                                                                                    newAddress.ProcessDefault = "1"
                                                                                Case "Line1"
                                                                                    newAddress.Line1 = adNode.InnerText
                                                                                    newAddress.ProcessLine1 = "1"
                                                                                    contactV1.AddressLine1 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine1 = "1"

                                                                                Case "Line2"
                                                                                    newAddress.Line2 = adNode.InnerText
                                                                                    newAddress.ProcessLine2 = "1"
                                                                                    contactV1.AddressLine2 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine2 = "1"

                                                                                Case "Line3"
                                                                                    newAddress.Line3 = adNode.InnerText
                                                                                    newAddress.ProcessLine3 = "1"
                                                                                    contactV1.AddressLine3 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine3 = "1"

                                                                                Case "Line4"
                                                                                    newAddress.Line4 = adNode.InnerText
                                                                                    newAddress.ProcessLine4 = "1"
                                                                                    contactV1.AddressLine4 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine4 = "1"

                                                                                Case "Line5"
                                                                                    newAddress.Line5 = adNode.InnerText
                                                                                    newAddress.ProcessLine5 = "1"
                                                                                    contactV1.AddressLine5 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine5 = "1"

                                                                                Case "PostCode"
                                                                                    newAddress.Postcode = adNode.InnerText
                                                                                    newAddress.ProcessPostCode = "1"
                                                                                    contactV1.PostCode = adNode.InnerText
                                                                                    contactV1.ProcessPostCode = "1"

                                                                                Case "Country"
                                                                                    newAddress.Country = adNode.InnerText
                                                                                    newAddress.ProcessCountry = "1"
                                                                            End Select
                                                                        Next
                                                                    Next

                                                                Case "Position"
                                                                    newContact.Position = conNode.InnerText
                                                                    newContact.ProcessPosition = "1"
                                                                    contactV1.PositionInCompany = conNode.InnerText
                                                                    contactV1.ProcessPositionInCompany = "1"

                                                                Case "Gender"
                                                                    newContact.Gender = conNode.InnerText
                                                                    newContact.ProcessGender = "1"
                                                                    contactV1.Gender = conNode.InnerText
                                                                    contactV1.ProcessGender = "1"

                                                                Case "TelephoneNumber1"
                                                                    newContact.TelephoneNumber1 = conNode.InnerText
                                                                    newContact.ProcessTelephoneNumber1 = "1"
                                                                    contactV1.HomeTelephoneNumber = conNode.InnerText
                                                                    contactV1.ProcessHomeTelephoneNumber = "1"

                                                                Case "TelephoneNumber2"
                                                                    newContact.TelephoneNumber2 = conNode.InnerText
                                                                    newContact.ProcessTelephoneNumber2 = "1"
                                                                    contactV1.WorkTelephoneNumber = conNode.InnerText
                                                                    contactV1.ProcessWorkTelephoneNumber = "1"

                                                                Case "TelephoneNumber3"
                                                                    newContact.TelephoneNumber3 = conNode.InnerText
                                                                    newContact.ProcessTelephoneNumber3 = "1"
                                                                    contactV1.MobileNumber = conNode.InnerText
                                                                    contactV1.ProcessMobileNumber = "1"

                                                                Case "TelephoneNumber4"
                                                                    newContact.TelephoneNumber4 = conNode.InnerText
                                                                    newContact.ProcessTelephoneNumber4 = "1"
                                                                Case "TelephoneNumber5"
                                                                    newContact.TelephoneNumber5 = conNode.InnerText
                                                                    newContact.ProcessTelephoneNumber5 = "1"
                                                                Case "DateOfBirth"

                                                                    Dim myD As Date = New Date(CInt(conNode.InnerText.Substring(0, 4)), _
                                                                                                CInt(conNode.InnerText.Substring(4, 2)), _
                                                                                                CInt(conNode.InnerText.Substring(6, 2)))

                                                                    newContact.DateOfBirth = myD
                                                                    newContact.ProcessDateOfBirth = "1"
                                                                    contactV1.DateBirth = conNode.InnerText
                                                                    contactV1.ProcessDateBirth = "1"

                                                                Case "ContactViaEmail"

                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.ContactViaEmail = True
                                                                        Case Else
                                                                            newContact.ContactViaEmail = False
                                                                    End Select
                                                                    newContact.ProcessContactViaEmail = "1"

                                                                    'contactV1.ContactViaMail = conNode.InnerText
                                                                    'contactV1.ProcessContactViaMail = "1"
                                                                    contactV1.ContactViaEmail = newContact.ContactViaEmail
                                                                    contactV1.ProcessContactViaEmail = "1"
                                                                Case "ContactViaMail"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.ContactViaMail = True
                                                                        Case Else
                                                                            newContact.ContactViaMail = False
                                                                    End Select
                                                                    newContact.ProcessContactViaMail = "1"

                                                                    contactV1.ContactViaMail = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail = "1"
                                                                Case "HTMLNewsletter"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.HTMLNewsletter = True
                                                                        Case Else
                                                                            newContact.HTMLNewsletter = False
                                                                    End Select
                                                                    newContact.ProcessHTMLNewsletter = "1"
                                                                Case "Subscription1"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Subscription1 = True
                                                                        Case Else
                                                                            newContact.Subscription1 = False
                                                                    End Select
                                                                    newContact.ProcessSubscription1 = "1"
                                                                    contactV1.Subscription1 = conNode.InnerText
                                                                    contactV1.ProcessSubscription1 = "1"

                                                                Case "Subscription2"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Subscription2 = True
                                                                        Case Else
                                                                            newContact.Subscription2 = False
                                                                    End Select
                                                                    newContact.ProcessSubscription2 = "1"
                                                                    contactV1.Subscription2 = conNode.InnerText
                                                                    contactV1.ProcessSubscription2 = "1"

                                                                Case "Subscription3"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Subscription3 = True
                                                                        Case Else
                                                                            newContact.Subscription3 = False
                                                                    End Select
                                                                    newContact.ProcessSubscription3 = "1"
                                                                    contactV1.Subscription3 = conNode.InnerText
                                                                    contactV1.ProcessSubscription3 = "1"

                                                                Case "MailFlag1"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.MailFlag1 = True
                                                                        Case Else
                                                                            newContact.MailFlag1 = False
                                                                    End Select
                                                                    newContact.ProcessMailFlag = "1"
                                                                Case "ExternalId1"
                                                                    newContact.ExternalId1 = conNode.InnerText
                                                                    newContact.ProcessExternal1 = "1"
                                                                    contactV1.SLNumber1 = conNode.InnerText
                                                                    contactV1.ProcessSLNumber1 = "1"

                                                                Case "ExternalId2"
                                                                    newContact.ExternalId2 = conNode.InnerText
                                                                    newContact.ProcessExternal2 = "1"
                                                                    contactV1.SLNumber2 = conNode.InnerText
                                                                    contactV1.ProcessSLNumber2 = "1"

                                                                Case "MessagingID"
                                                                    newContact.MessagingID = conNode.InnerText
                                                                    newContact.ProcessMessagingField = "1"
                                                                Case "Boolean1"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean1 = True
                                                                        Case Else
                                                                            newContact.Boolean1 = False
                                                                    End Select
                                                                    newContact.ProcessBoolean1 = "1"
                                                                    contactV1.ContactViaMail1 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail1 = "1"

                                                                Case "Boolean2"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean2 = True
                                                                        Case Else
                                                                            newContact.Boolean2 = False
                                                                    End Select
                                                                    newContact.ProcessBoolean2 = "1"
                                                                    contactV1.ContactViaMail2 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail2 = "1"

                                                                Case "Boolean3"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean3 = True
                                                                        Case Else
                                                                            newContact.Boolean3 = False
                                                                    End Select
                                                                    newContact.ProcessBoolean3 = "1"
                                                                    contactV1.ContactViaMail3 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail3 = "1"

                                                                Case "Boolean4"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean4 = True
                                                                        Case Else
                                                                            newContact.Boolean4 = False
                                                                    End Select
                                                                    newContact.ProcessBoolean4 = "1"
                                                                    contactV1.ContactViaMail4 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail4 = "1"

                                                                Case "Boolean5"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean5 = True
                                                                        Case Else
                                                                            newContact.Boolean5 = False
                                                                    End Select
                                                                    newContact.ProcessBoolean5 = "1"
                                                                    contactV1.ContactViaMail5 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail5 = "1"

                                                                Case "ID"
                                                                    newContact.ID = conNode.InnerText
                                                                    newContact.ProcessID = "1"
                                                                Case "USERID1"
                                                                    contactV1.PassportNumber = conNode.InnerText
                                                                Case "USERID2"
                                                                    contactV1.GreenCardNumber = conNode.InnerText
                                                                Case "USERID3"
                                                                    contactV1.PIN_Number = conNode.InnerText
                                                                Case "USERID4"
                                                                    contactV1.User_ID_4 = conNode.InnerText
                                                                Case "USERID5"
                                                                    contactV1.User_ID_5 = conNode.InnerText
                                                                Case "USERID6"
                                                                    contactV1.User_ID_6 = conNode.InnerText
                                                                Case "USERID7"
                                                                    contactV1.User_ID_7 = conNode.InnerText
                                                                Case "USERID8"
                                                                    contactV1.User_ID_8 = conNode.InnerText
                                                                Case "USERID9"
                                                                    contactV1.User_ID_9 = conNode.InnerText
                                                                Case "RestrictedPaymentTypes"
                                                                    For Each rNode As XmlNode In conNode.ChildNodes
                                                                        Dim rpt As New DECustomerV11.DECustomerSiteRestrictedPaymentType
                                                                        newContact.RestrictedPaymentTypes.Add(rpt)

                                                                        rpt.UpdateMode = rNode.Attributes("Mode").Value
                                                                        rpt.PaymentType = rNode.InnerText
                                                                    Next
                                                                    newContact.ProcessRestrictedPaymentTypes = "1"
                                                                Case "Attributes"
                                                                    Dim i As Integer = 0
                                                                    For Each attNode As XmlNode In conNode.ChildNodes
                                                                        i += 1
                                                                        Dim att As New DECustomerV11.DECustomerSiteAttributes
                                                                        newContact.Attributes.Add(att)
                                                                        att.Attribute = attNode.InnerText
                                                                        att.UpdateMode = attNode.Attributes("Mode").Value

                                                                        Select Case i
                                                                            Case 1
                                                                                contactV1.Attribute01 = attNode.InnerText
                                                                                ' contactV1.Attribute01Action = SetAttributeAction(attNode.Attributes("action").Value)
                                                                            Case 2
                                                                                contactV1.Attribute02 = attNode.InnerText
                                                                                ' contactV1.Attribute02Action = SetAttributeAction(attNode.Attributes("action").Value)
                                                                            Case 3
                                                                                contactV1.Attribute03 = attNode.InnerText
                                                                                ' contactV1.Attribute03Action = SetAttributeAction(attNode.Attributes("action").Value)
                                                                        End Select

                                                                    Next
                                                                    newContact.ProcessAttributes = "1"
                                                                    contactV1.ProcessAttributes = "1"
                                                                Case "LoyaltyPoints"
                                                                    newContact.LoyaltyPoints = conNode.InnerText
                                                                    newContact.ProcessLoyaltyPoints = "1"
                                                                Case "IsLockedOut"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.IsLockedOut = True
                                                                        Case Else
                                                                            newContact.IsLockedOut = False
                                                                    End Select
                                                                    newContact.ProcessIsLockedOut = "1"
                                                                Case "CustomerPurchaseHistory"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.CustomerPurchaseHistory = True
                                                                        Case Else
                                                                            newContact.CustomerPurchaseHistory = False
                                                                    End Select
                                                                    newContact.ProcessCustomerPurchaseHistory = "1"

                                                                Case Is = "ContactSource"
                                                                    contactV1.ContactSource = conNode.InnerText
                                                                Case Is = "ContactNickName"
                                                                    contactV1.Nickname = conNode.InnerText
                                                                    contactV1.ProcessNickname = "1"
                                                                Case Is = "ContactUserName"
                                                                    contactV1.AltUserName = conNode.InnerText
                                                                    contactV1.ProcessAltUserName = "1"
                                                                Case Is = "ContactSuffix"
                                                                    contactV1.Suffix = conNode.InnerText
                                                                    contactV1.ProcessSuffix = "1"
                                                                Case Is = "ContactSLAccount"
                                                                    contactV1.ContactSLAccount = conNode.InnerText
                                                                    contactV1.ProcessContactSLAccount = "1"
                                                                Case Is = "MinimalRegistration"
                                                                    contactV1.MinimalRegistration = CBool(conNode.InnerText)
                                                            End Select
                                                        Next
                                                    Next
                                            End Select
                                        Next
                                    Next
                                Next
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCC-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3 As XmlNode
            Dim count As Integer = 0
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerUpdateRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerUpdate"
                            Decust = New DECustomerV11
                            Dim deCustV1 As New DECustomer
                            Decust.DECustomersV1.Add(deCustV1)
                            With deCustV1
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "CustomerNo"
                                            .CustomerNumber = Node2.InnerText
                                        Case Is = "ContactTitle"
                                            .ContactTitle = Node2.InnerText
                                            .ProcessContactTitle = "1"
                                        Case Is = "ContactInitials"
                                            .ContactInitials = Node2.InnerText
                                            .ProcessContactInitials = "1"
                                        Case Is = "ContactForename"
                                            .ContactForename = Node2.InnerText
                                            .ProcessContactForename = "1"
                                        Case Is = "ContactSurname"
                                            .ContactSurname = Node2.InnerText
                                            .ProcessContactSurname = "1"
                                        Case Is = "Salutation"
                                            .Salutation = Node2.InnerText
                                            .ProcessSalutation = "1"
                                        Case Is = "CompanyName"
                                            .CompanyName = Node2.InnerText
                                            .ProcessCompanyName = "1"
                                            .UpdateCompanyInformation = "Y"
                                        Case Is = "PositionInCompany"
                                            .PositionInCompany = Node2.InnerText
                                            .ProcessPositionInCompany = "1"
                                            .UpdateCompanyInformation = "Y"
                                        Case Is = "AddressLine1"
                                            .AddressLine1 = Node2.InnerText
                                            .ProcessAddressLine1 = "1"
                                        Case Is = "AddressLine2"
                                            .AddressLine2 = Node2.InnerText
                                            .ProcessAddressLine2 = "1"
                                        Case Is = "AddressLine3"
                                            .AddressLine3 = Node2.InnerText
                                            .ProcessAddressLine3 = "1"
                                        Case Is = "AddressLine4"
                                            .AddressLine4 = Node2.InnerText
                                            .ProcessAddressLine4 = "1"
                                        Case Is = "AddressLine5"
                                            .AddressLine5 = Node2.InnerText
                                            .ProcessAddressLine5 = "1"
                                        Case Is = "PostCode"
                                            .PostCode = Node2.InnerText
                                            .ProcessPostCode = "1"
                                        Case Is = "Gender"
                                            .Gender = Node2.InnerText
                                            .ProcessGender = "1"
                                        Case Is = "HomeTelephoneNumber"
                                            .HomeTelephoneNumber = Node2.InnerText
                                            .ProcessHomeTelephoneNumber = "1"
                                        Case Is = "WorkTelephoneNumber"
                                            .WorkTelephoneNumber = Node2.InnerText
                                            .ProcessWorkTelephoneNumber = "1"
                                        Case Is = "MobileNumber"
                                            .MobileNumber = Node2.InnerText
                                            .ProcessMobileNumber = "1"
                                        Case Is = "EmailAddress"
                                            .EmailAddress = Node2.InnerText
                                            .ProcessEmailAddress = "1"
                                        Case Is = "DateOfBirth"
                                            .DateBirth = Node2.InnerText
                                            .ProcessDateBirth = "1"
                                        Case Is = "ContactViaMail"
                                            .ContactViaMail = Node2.InnerText
                                            .ProcessContactViaMail = "1"
                                        Case Is = "Subscription1"
                                            .Subscription1 = Node2.InnerText
                                            .ProcessSubscription1 = "1"
                                        Case Is = "Subscription2"
                                            .Subscription2 = Node2.InnerText
                                            .ProcessSubscription2 = "1"
                                        Case Is = "Subscription3"
                                            .Subscription3 = Node2.InnerText
                                            .ProcessSubscription3 = "1"
                                        Case Is = "ContactViaMail1"
                                            .ContactViaMail1 = Node2.InnerText
                                            .ProcessContactViaMail1 = "1"
                                        Case Is = "ContactViaMail2"
                                            .ContactViaMail2 = Node2.InnerText
                                            .ProcessContactViaMail2 = "1"
                                        Case Is = "ContactViaMail3"
                                            .ContactViaMail3 = Node2.InnerText
                                            .ProcessContactViaMail3 = "1"
                                        Case Is = "ContactViaMail4"
                                            .ContactViaMail4 = Node2.InnerText
                                            .ProcessContactViaMail4 = "1"
                                        Case Is = "ContactViaMail5"
                                            .ContactViaMail5 = Node2.InnerText
                                            .ProcessContactViaMail5 = "1"
                                        Case Is = "ExternalId1"
                                            .SLNumber1 = Node2.InnerText
                                            .ProcessSLNumber1 = "1"
                                        Case Is = "ExternalId2"
                                            .SLNumber2 = Node2.InnerText
                                            .ProcessSLNumber2 = "1"
                                        Case Is = "Password"
                                            .Password = Node2.InnerText
                                        Case Is = "Attributes"
                                            .ProcessAttributes = "1"
                                            '----------------------------------------
                                            ' Currently only 3 attributes catered for
                                            '----------------------------------------
                                            count = 1
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case count
                                                    Case Is = 1
                                                        .Attribute01 = Node3.InnerText
                                                        .Attribute01Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                    Case Is = 2
                                                        .Attribute04 = Node3.InnerText
                                                        .Attribute04Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                    Case Is = 3
                                                        .Attribute05 = Node3.InnerText
                                                        .Attribute05Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                End Select
                                                count += 1
                                            Next
                                    End Select

                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCU-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


    End Class

End Namespace