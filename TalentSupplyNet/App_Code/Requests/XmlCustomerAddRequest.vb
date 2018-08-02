Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Customer Add Requests
'
'       Date                        April 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQCC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerAddRequest
        Inherits XmlRequest

        Private _decust As New DECustomerV11
        Public Property Decust() As DECustomerV11
            Get
                Return _decust
            End Get
            Set(ByVal value As DECustomerV11)
                _decust = value
            End Set
        End Property

        Private _hashPassword As Boolean
        Public Property HashPassword() As Boolean
            Get
                Return _hashPassword
            End Get
            Set(ByVal value As Boolean)
                _hashPassword = value
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

            Dim xmlAction As XmlCustomerAddResponse = CType(xmlResp, XmlCustomerAddResponse)
            xmlAction.ResponseDirectory = Settings.ResponseDirectory

            Dim err As ErrorObj = Nothing
            '-------------------------------------------
            ' Check whether need to hash password or not
            '-------------------------------------------
            Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
            Dim hashedPassword As String = String.Empty

            Try
                HashPassword = CBool(def.GetDefault("HASH_PASSWORD"))
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

            Dim CUSTOMER As New TalentCustomer
            If err.HasError Then
                xmlResp.Err = err
            Else

                With CUSTOMER
                    .DeV11 = Decust
                    .Settings = Settings
                    err = .SetCustomer
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = CUSTOMER.ResultDataSet
            xmlAction.DestinationDatabase = CUSTOMER.Settings.DestinationDatabase
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            Dim count As Integer = 0
            Settings.SupplyNetRequestName = "CustomerAdd11"
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                Decust = New DECustomerV11
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerAddRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Defaults"
                            For Each n As XmlNode In Node1.ChildNodes
                                Select Case n.Name
                                    Case "BusinessUnit"
                                        Decust.BusinessUnit = n.InnerText
                                End Select
                            Next
                        Case Is = "CustomerAdd"
                            With Decust
                                For Each sitesNode As XmlNode In Node1.ChildNodes

                                    For Each site As XmlNode In sitesNode.ChildNodes
                                        Dim newSite As New DECustomerV11.DECustomerSite
                                        Decust.Sites.Add(newSite)

                                        For Each sNode As XmlNode In site.ChildNodes
                                            Select Case sNode.Name

                                                Case "Name"
                                                    newSite.Name = sNode.InnerText
                                                Case "AccountNumber1"
                                                    newSite.AccountNumber1 = sNode.InnerText
                                                Case "AccountNumber2"
                                                    newSite.AccountNumber2 = sNode.InnerText
                                                Case "AccountNumber3"
                                                    newSite.AccountNumber3 = sNode.InnerText
                                                Case "AccountNumber4"
                                                    newSite.AccountNumber4 = sNode.InnerText
                                                Case "AccountNumber5"
                                                    newSite.AccountNumber5 = sNode.InnerText

                                                Case "Address"
                                                    Dim newAddress As New DECustomerV11.DECustomerSiteAddress
                                                    newSite.Address = newAddress

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
                                                Case "FaxNumber"
                                                    newSite.FaxNumber = sNode.InnerText
                                                Case "VATNumber"
                                                    newSite.VATNumber = sNode.InnerText
                                                Case "URL"
                                                    newSite.URL = sNode.InnerText
                                                Case "ID"
                                                    newSite.ID = sNode.InnerText
                                                Case "CRMBranch"
                                                    newSite.CRMBranch = sNode.InnerText
                                                Case "Contacts"

                                                    For Each cNode As XmlNode In sNode.ChildNodes
                                                        Dim newContact As New DECustomerV11.DECustomerSiteContact
                                                        newSite.Contacts.Add(newContact)

                                                        Dim contactV1 As New DECustomer
                                                        .DECustomersV1.Add(contactV1)
                                                        contactV1.UseEncryptedPassword = HashPassword
                                                        contactV1.EncryptionType = EncryptionType
                                                        contactV1.CompanyName = newSite.Name
                                                        contactV1.ProcessCompanyName = "1"
                                                        contactV1.UpdateCompanyInformation = "Y"
                                                        contactV1.ProcessUpdateCompanyInformation = "Y"

                                                        For Each conNode As XmlNode In cNode.ChildNodes
                                                            Select Case conNode.Name
                                                                Case "Title"
                                                                    newContact.Title = conNode.InnerText

                                                                    contactV1.ContactTitle = conNode.InnerText
                                                                    contactV1.ProcessContactTitle = "1"

                                                                Case "Initials"
                                                                    newContact.Initials = conNode.InnerText

                                                                    contactV1.ContactInitials = conNode.InnerText
                                                                    contactV1.ProcessContactInitials = "1"

                                                                Case "Forename"
                                                                    newContact.Forename = conNode.InnerText

                                                                    contactV1.ContactForename = conNode.InnerText
                                                                    contactV1.ProcessContactForename = "1"

                                                                Case "Surname"
                                                                    newContact.Surname = conNode.InnerText

                                                                    contactV1.ContactSurname = conNode.InnerText
                                                                    contactV1.ProcessContactSurname = "1"

                                                                Case "FullName"
                                                                    newContact.FullName = conNode.InnerText
                                                                Case "Salutation"
                                                                    newContact.Salutation = conNode.InnerText

                                                                    contactV1.Salutation = conNode.InnerText
                                                                    contactV1.ProcessSalutation = "1"

                                                                Case "MothersName"
                                                                    newContact.MothersName = conNode.InnerText

                                                                    contactV1.MothersName = conNode.InnerText
                                                                    contactV1.ProcessMothersName = "1"

                                                                Case "FathersName"
                                                                    newContact.FathersName = conNode.InnerText

                                                                    contactV1.FathersName = conNode.InnerText
                                                                    contactV1.ProcessFathersName = "1"

                                                                Case "EmailAddress"
                                                                    newContact.EmailAddress = conNode.InnerText

                                                                    contactV1.EmailAddress = conNode.InnerText
                                                                    contactV1.ProcessEmailAddress = "1"

                                                                Case "LoginID"
                                                                    newContact.LoginID = conNode.InnerText
                                                                Case "Password"
                                                                    newContact.Password = conNode.InnerText
                                                                    contactV1.Password = conNode.InnerText
                                                                    If contactV1.UseEncryptedPassword Then
                                                                        newContact.Password = "**ENCRYPTED**"
                                                                        contactV1.Password = "**ENCRYPTED**"
                                                                        Dim passHash As New PasswordHash()
                                                                        contactV1.HashedPassword = passHash.HashSalt(conNode.InnerText, SaltString)
                                                                        contactV1.NewHashedPassword = contactV1.HashedPassword
                                                                    End If

                                                                Case "AccountNumber1"
                                                                    newContact.AccountNumber1 = conNode.InnerText
                                                                Case "AccountNumber2"
                                                                    newContact.AccountNumber2 = conNode.InnerText
                                                                Case "AccountNumber3"
                                                                    newContact.AccountNumber3 = conNode.InnerText
                                                                Case "AccountNumber4"
                                                                    newContact.AccountNumber4 = conNode.InnerText
                                                                Case "AccountNumber5"
                                                                    newContact.AccountNumber5 = conNode.InnerText
                                                                Case "Addresses"
                                                                    For Each aNode As XmlNode In conNode.ChildNodes
                                                                        Dim newAddress As New DECustomerV11.DECustomerSiteAddress
                                                                        newContact.Addresses.Add(newAddress)
                                                                        For Each adNode As XmlNode In aNode.ChildNodes
                                                                            Select Case adNode.Name
                                                                                Case "SequenceNumber"
                                                                                    newAddress.SequenceNumber = adNode.InnerText
                                                                                Case "Default"

                                                                                    Select Case adNode.InnerText.ToUpper
                                                                                        Case "Y", "TRUE", "1", "YES"
                                                                                            newAddress.IsDefault = True
                                                                                        Case Else
                                                                                            newAddress.IsDefault = False
                                                                                    End Select

                                                                                Case "Line1"
                                                                                    newAddress.Line1 = adNode.InnerText

                                                                                    contactV1.AddressLine1 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine1 = "1"

                                                                                Case "Line2"
                                                                                    newAddress.Line2 = adNode.InnerText

                                                                                    contactV1.AddressLine2 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine2 = "1"

                                                                                Case "Line3"
                                                                                    newAddress.Line3 = adNode.InnerText

                                                                                    contactV1.AddressLine3 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine3 = "1"

                                                                                Case "Line4"
                                                                                    newAddress.Line4 = adNode.InnerText

                                                                                    contactV1.AddressLine4 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine4 = "1"

                                                                                Case "Line5"
                                                                                    newAddress.Line5 = adNode.InnerText

                                                                                    contactV1.AddressLine5 = adNode.InnerText
                                                                                    contactV1.ProcessAddressLine5 = "1"

                                                                                Case "PostCode"
                                                                                    newAddress.Postcode = adNode.InnerText

                                                                                    contactV1.PostCode = adNode.InnerText
                                                                                    contactV1.ProcessPostCode = "1"

                                                                                Case "Country"
                                                                                    newAddress.Country = adNode.InnerText

                                                                            End Select
                                                                        Next
                                                                    Next

                                                                Case "Position"
                                                                    newContact.Position = conNode.InnerText

                                                                    contactV1.PositionInCompany = conNode.InnerText
                                                                    contactV1.ProcessPositionInCompany = "1"

                                                                Case "Gender"
                                                                    newContact.Gender = conNode.InnerText

                                                                    contactV1.Gender = conNode.InnerText
                                                                    contactV1.ProcessGender = "1"

                                                                Case "TelephoneNumber1"
                                                                    newContact.TelephoneNumber1 = conNode.InnerText

                                                                    contactV1.HomeTelephoneNumber = conNode.InnerText
                                                                    contactV1.ProcessHomeTelephoneNumber = "1"

                                                                Case "TelephoneNumber2"
                                                                    newContact.TelephoneNumber2 = conNode.InnerText

                                                                    contactV1.WorkTelephoneNumber = conNode.InnerText
                                                                    contactV1.ProcessWorkTelephoneNumber = "1"

                                                                Case "TelephoneNumber3"
                                                                    newContact.TelephoneNumber3 = conNode.InnerText

                                                                    contactV1.MobileNumber = conNode.InnerText
                                                                    contactV1.ProcessMobileNumber = "1"

                                                                Case "TelephoneNumber4"
                                                                    newContact.TelephoneNumber4 = conNode.InnerText
                                                                Case "TelephoneNumber5"
                                                                    newContact.TelephoneNumber5 = conNode.InnerText
                                                                Case "DateOfBirth"

                                                                    Dim myD As Date = New Date(CInt(conNode.InnerText.Substring(0, 4)), _
                                                                                                CInt(conNode.InnerText.Substring(4, 2)), _
                                                                                                CInt(conNode.InnerText.Substring(6, 2)))

                                                                    newContact.DateOfBirth = myD

                                                                    contactV1.DateBirth = conNode.InnerText
                                                                    contactV1.ProcessDateBirth = "1"

                                                                Case "ContactViaMail"

                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.ContactViaMail = True
                                                                        Case Else
                                                                            newContact.ContactViaMail = False
                                                                    End Select

                                                                    contactV1.ContactViaMail = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail = "1"

                                                                Case "HTMLNewsletter"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.HTMLNewsletter = True
                                                                        Case Else
                                                                            newContact.HTMLNewsletter = False
                                                                    End Select

                                                                Case "Subscription1"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Subscription1 = True
                                                                        Case Else
                                                                            newContact.Subscription1 = False
                                                                    End Select

                                                                    contactV1.Subscription1 = conNode.InnerText
                                                                    contactV1.ProcessSubscription1 = "1"

                                                                Case "Subscription2"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Subscription2 = True
                                                                        Case Else
                                                                            newContact.Subscription2 = False
                                                                    End Select

                                                                    contactV1.Subscription2 = conNode.InnerText
                                                                    contactV1.ProcessSubscription2 = "1"

                                                                Case "Subscription3"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Subscription3 = True
                                                                        Case Else
                                                                            newContact.Subscription3 = False
                                                                    End Select

                                                                    contactV1.Subscription3 = conNode.InnerText
                                                                    contactV1.ProcessSubscription3 = "1"

                                                                Case "MailFlag1"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.MailFlag1 = True
                                                                        Case Else
                                                                            newContact.MailFlag1 = False
                                                                    End Select
                                                                Case "ExternalId1"
                                                                    newContact.ExternalId1 = conNode.InnerText

                                                                    contactV1.SLNumber1 = conNode.InnerText
                                                                    contactV1.ProcessSLNumber1 = "1"

                                                                Case "ExternalId2"
                                                                    newContact.ExternalId2 = conNode.InnerText

                                                                    contactV1.SLNumber2 = conNode.InnerText
                                                                    contactV1.ProcessSLNumber2 = "1"

                                                                Case "MessagingID"
                                                                    newContact.MessagingID = conNode.InnerText
                                                                Case "Boolean1"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean1 = True
                                                                        Case Else
                                                                            newContact.Boolean1 = False
                                                                    End Select

                                                                    contactV1.ContactViaMail1 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail1 = "1"

                                                                Case "Boolean2"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean2 = True
                                                                        Case Else
                                                                            newContact.Boolean2 = False
                                                                    End Select

                                                                    contactV1.ContactViaMail2 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail2 = "1"

                                                                Case "Boolean3"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean3 = True
                                                                        Case Else
                                                                            newContact.Boolean3 = False
                                                                    End Select

                                                                    contactV1.ContactViaMail3 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail3 = "1"

                                                                Case "Boolean4"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean4 = True
                                                                        Case Else
                                                                            newContact.Boolean4 = False
                                                                    End Select

                                                                    contactV1.ContactViaMail4 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail4 = "1"

                                                                Case "Boolean5"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.Boolean5 = True
                                                                        Case Else
                                                                            newContact.Boolean5 = False
                                                                    End Select

                                                                    contactV1.ContactViaMail5 = conNode.InnerText
                                                                    contactV1.ProcessContactViaMail5 = "1"

                                                                Case "ID"
                                                                    newContact.ID = conNode.InnerText
                                                                
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
                                                                        rpt.PaymentType = rNode.InnerText
                                                                    Next
                                                                Case "Attributes"
                                                                    contactV1.ProcessAttributes = "1"
                                                                    Dim i As Integer = 0
                                                                    For Each attNode As XmlNode In conNode.ChildNodes
                                                                        i += 1
                                                                        Dim att As New DECustomerV11.DECustomerSiteAttributes
                                                                        newContact.Attributes.Add(att)
                                                                        att.Attribute = attNode.InnerText
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
                                                                Case "LoyaltyPoints"
                                                                    newContact.LoyaltyPoints = conNode.InnerText                                                                
                                                                Case "IsLockedOut"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.IsLockedOut = True
                                                                        Case Else
                                                                            newContact.IsLockedOut = False
                                                                    End Select
                                                                Case "CustomerPurchaseHistory"
                                                                    Select Case conNode.InnerText.ToUpper
                                                                        Case "Y", "TRUE", "1", "YES"
                                                                            newContact.CustomerPurchaseHistory = True
                                                                        Case Else
                                                                            newContact.CustomerPurchaseHistory = False
                                                                    End Select
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
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerAddRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerAdd"
                            Decust = New DECustomerV11
                            Dim deCustV1 As New DECustomer
                            Decust.DECustomersV1.Add(deCustV1)

                            With deCustV1
                                For Each Node2 In Node1.ChildNodes

                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
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
                                                        .Attribute02 = Node3.InnerText
                                                        .Attribute02Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                    Case Is = 3
                                                        .Attribute03 = Node3.InnerText
                                                        .Attribute03Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                End Select
                                                count += 1
                                            Next
                                        Case Is = "BranchCode"
                                            .BranchCode = Node2.InnerText
                                    End Select

                                Next Node2
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

    End Class

End Namespace