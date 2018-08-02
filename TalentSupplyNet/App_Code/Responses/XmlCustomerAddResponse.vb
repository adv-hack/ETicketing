Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with add customer responses
'
'       Date                        April 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSCC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerAddResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndCustomerInfo, ndResponse, ndCustomerDetails, ndReturnCode As XmlNode
        Private ndCustomerNo, ndContactTitle, ndContactInitials, ndContactForename, ndContactSurname, _
                ndSalutation, ndCompanyName, ndPositionInCompany, ndAddressLine1, _
                ndAddressLine2, ndAddressLine3, ndAddressLine4, ndAddressLine5, _
                ndPostCode, ndGender, ndHomeTelephoneNumber, ndWorkTelephoneNumber, _
                ndMobileNumber, ndEmailAddress, ndDateBirth, ndContactViaMail, _
                ndSubscription1, ndSubscription2, ndSubscription3, ndContactViaMail1, _
                ndContactViaMail2, ndContactViaMail3, ndContactViaMail4, ndContactViaMail5, _
                ndExternalId1, ndExternalId2 As XmlNode
        Private dtCustomerDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False


        Private _databaseType As String
        Public Property DestinationDatabase() As String
            Get
                Return _databaseType
            End Get
            Set(ByVal value As String)
                _databaseType = value
            End Set
        End Property


        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerAddDetails")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection(False)

                'Create the customer detail section
                CreateCustomerDetail()

                'Populate the xml document
                With ndCustomerInfo
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndCustomerDetails)
                    End If
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndCustomerInfo, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSCC-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Overrides Sub InsertBodyV1_1()

            Try

                Dim ndCustomerAddDetails As XmlNode

                Select Case Me.DestinationDatabase.ToUpper
                    Case "SQL2005"
                        ndCustomerAddDetails = Me.PopulateCustomerDetails_SQL2005
                    Case Else
                        ndCustomerAddDetails = Me.PopulateCustomerDetails_TALENTTKT
                End Select
               

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndCustomerAddDetails, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSCC-01"
                    .HasError = True
                End With
            End Try

        End Sub


        Protected Function PopulateCustomerDetails_SQL2005() As XmlNode
            Dim ndCustomerAddDetails As XmlNode, _
                   ndSiteActionSuccess As XmlNode, _
                   ndSiteAddressActionSuccess As XmlNode, _
                   ndSites As XmlNode, _
                   ndSite As XmlNode, _
                   ndSiteName As XmlNode, _
                   ndContacts As XmlNode, _
                   ndContact As XmlNode, _
                   ndAddresses As XmlNode, _
                   ndAddress As XmlNode, _
                   ndContactAuthoriseSuccess As XmlNode, _
                   ndContactUserSuccess As XmlNode, _
                   ndAddressPostcode As XmlNode, _
                   ndAddressSuccess As XmlNode


            With MyBase.xmlDoc
                ndCustomerAddDetails = .CreateElement("CustomerAddDetails")
                ndSites = .CreateElement("Sites")
                ndResponse = .CreateElement("Response")
            End With


            'Create the response xml section
            CreateResponseSection(True)

            If Not errorOccurred Then


                Dim pt As DataTable = Me.ResultDataSet.Tables("Partners")
                Dim ut As DataTable = Me.ResultDataSet.Tables("Users")
                Dim at As DataTable = Me.ResultDataSet.Tables("Addresses")

                If pt.Rows.Count > 0 Then

                    For Each pr As DataRow In pt.Rows

                        With MyBase.xmlDoc
                            ndSite = .CreateElement("Site")
                            ndSiteName = .CreateElement("Name")
                            ndContacts = .CreateElement("Contacts")
                            ndSiteActionSuccess = .CreateElement("SiteActionSuccess")
                            ndSiteAddressActionSuccess = .CreateElement("SiteAddressActionSuccess")
                        End With

                        ndSites.AppendChild(ndSite)
                        With ndSite
                            .AppendChild(ndSiteName)
                            .AppendChild(ndSiteActionSuccess)
                            .AppendChild(ndSiteAddressActionSuccess)
                            .AppendChild(ndContacts)
                        End With
                        ndSiteName.InnerText = pr("Partner")
                        ndSiteActionSuccess.InnerText = pr("PartnerSuccess")
                        ndSiteAddressActionSuccess.InnerText = pr("AddressSuccess")



                        Dim urs() As DataRow = ut.Select("Partner = '" & pr("Partner") & "'")

                        If urs.Length > 0 Then
                            For i As Integer = 0 To urs.Length - 1
                                Dim ur As DataRow = urs(i)


                                With MyBase.xmlDoc
                                    ndContact = .CreateElement("Contact")
                                    ndAddresses = .CreateElement("Addresses")
                                    ndCustomerNo = .CreateElement("CustomerNo")
                                    ndContactAuthoriseSuccess = .CreateElement("AuthoriseActionSuccess")
                                    ndContactUserSuccess = .CreateElement("UserActionSuccess")
                                End With

                                ndContacts.AppendChild(ndContact)
                                With ndContact
                                    .AppendChild(ndCustomerNo)
                                    .AppendChild(ndContactAuthoriseSuccess)
                                    .AppendChild(ndContactUserSuccess)
                                    .AppendChild(ndAddresses)
                                End With

                                ndContactAuthoriseSuccess.InnerText = ur("AuthoriseSuccess")
                                ndContactUserSuccess.InnerText = ur("UserSuccess")
                                ndCustomerNo.InnerText = ur("LoginID")

                                Dim ars() As DataRow = at.Select("Partner = '" & pr("Partner") & "' AND LoginID = '" & ur("LoginID") & "'")
                                If ars.Length > 0 Then
                                    For j As Integer = 0 To ars.Length - 1

                                        Dim ar As DataRow = ars(j)
                                        With MyBase.xmlDoc
                                            ndAddress = .CreateElement("Address")
                                            ndAddressPostcode = .CreateElement("PostCode")
                                            ndAddressSuccess = .CreateElement("AddressActionSuccess")
                                        End With

                                        ndAddresses.AppendChild(ndAddress)
                                        ndAddress.AppendChild(ndAddressPostcode)
                                        ndAddress.AppendChild(ndAddressSuccess)
                                        ndAddressSuccess.InnerText = ar("AddressSuccess")
                                        ndAddressPostcode.InnerText = ar("Postcode")
                                    Next
                                End If
                            Next
                        End If

                    Next

                End If
            End If

            'Populate the xml document
            With ndCustomerAddDetails
                .AppendChild(ndResponse)
                If Not errorOccurred Then
                    .AppendChild(ndSites)
                End If
            End With

            Return ndCustomerAddDetails
        End Function


        Protected Function PopulateCustomerDetails_TALENTTKT() As XmlNode
            Dim ndCustomerAddDetails As XmlNode, _
                   ndSiteActionSuccess As XmlNode, _
                   ndSiteAddressActionSuccess As XmlNode, _
                   ndSites As XmlNode, _
                   ndSite As XmlNode, _
                   ndSiteName As XmlNode, _
                   ndContacts As XmlNode, _
                   ndContact As XmlNode, _
                   ndAddresses As XmlNode, _
                   ndAddress As XmlNode, _
                   ndContactAuthoriseSuccess As XmlNode, _
                   ndContactUserSuccess As XmlNode, _
                   ndAddressPostcode As XmlNode, _
                   ndAddressSuccess As XmlNode


            With MyBase.xmlDoc
                ndCustomerAddDetails = .CreateElement("CustomerAddDetails")
                ndSiteActionSuccess = .CreateElement("SiteActionSuccess")
                ndSiteAddressActionSuccess = .CreateElement("SiteAddressActionSuccess")
                ndSites = .CreateElement("Sites")
                ndSite = .CreateElement("Site")
                ndSiteName = .CreateElement("Name")
                ndContacts = .CreateElement("Contacts")
                ndResponse = .CreateElement("Response")
            End With

            ndSites.AppendChild(ndSite)
            With ndSite
                .AppendChild(ndSiteName)
                .AppendChild(ndSiteActionSuccess)
                .AppendChild(ndSiteAddressActionSuccess)
                .AppendChild(ndContacts)
            End With


            'Create the response xml section
            CreateResponseSection(False)

            If Not errorOccurred Then

                ndSiteActionSuccess.InnerText = "True"
                ndSiteAddressActionSuccess.InnerText = "True"

                'Read the values for the response section
                'Create the customer xml nodes

                Dim i As Integer
                i = 1
                Do While (i < ResultDataSet.Tables.Count)

                    dtCustomerDetails = ResultDataSet.Tables(i)
                    Dim dr As DataRow = dtCustomerDetails.Rows.Item(0)

                    With MyBase.xmlDoc
                        ndContact = .CreateElement("Contact")
                        ndCustomerNo = .CreateElement("CustomerNo")
                        ndContactAuthoriseSuccess = .CreateElement("AuthoriseActionSuccess")
                        ndContactUserSuccess = .CreateElement("UserActionSuccess")
                        ndAddresses = .CreateElement("Addresses")
                        ndAddress = .CreateElement("Address")
                        ndAddressPostcode = .CreateElement("PostCode")
                        ndAddressSuccess = .CreateElement("AddressActionSuccess")
                    End With

                    ndContactAuthoriseSuccess.InnerText = "True"
                    ndContactUserSuccess.InnerText = "True"
                    ndAddressSuccess.InnerText = "True"

                    'Populate the nodes
                    ndCustomerNo.InnerText = dr("CustomerNo")
                    ndAddressPostcode.InnerText = dr("Postcode")

                    ndContacts.AppendChild(ndContact)
                    With ndContact
                        .AppendChild(ndCustomerNo)
                        .AppendChild(ndContactAuthoriseSuccess)
                        .AppendChild(ndContactUserSuccess)
                        .AppendChild(ndAddresses)
                    End With

                    With ndAddresses
                        .AppendChild(ndAddress)
                    End With
                    With ndAddress
                        .AppendChild(ndAddressPostcode)
                        .AppendChild(ndAddressSuccess)
                    End With
                    i += 1
                Loop

            End If

            'Populate the xml document
            With ndCustomerAddDetails
                .AppendChild(ndResponse)
                If Not errorOccurred Then
                    .AppendChild(ndSites)
                End If
            End With

            Return ndCustomerAddDetails
        End Function


        Protected Sub CreateResponseSection(ByVal isSql2005 As Boolean)

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            If Not isSql2005 Then
                'Read the values for the response section
                dtStatusDetails = ResultDataSet.Tables(0)
                dr = dtStatusDetails.Rows(0)

                'Populate the nodes
                ndReturnCode.InnerText = dr("ReturnCode")
                If dr("ErrorOccurred") = "E" Then
                    errorOccurred = True
                End If
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Protected Sub CreateCustomerDetail()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Create the customer xml nodes
                With MyBase.xmlDoc
                    ndCustomerNo = .CreateElement("CustomerNo")
                    ndCustomerDetails = .CreateElement("CustomerDetails")
                    ndContactTitle = .CreateElement("ContactTitle")
                    ndContactInitials = .CreateElement("ContactInitials")
                    ndContactForename = .CreateElement("ContactForename")
                    ndContactSurname = .CreateElement("ContactSurname")
                    ndSalutation = .CreateElement("Salutation")
                    ndCompanyName = .CreateElement("CompanyName")
                    ndPositionInCompany = .CreateElement("PositionInCompany")
                    ndAddressLine1 = .CreateElement("AddressLine1")
                    ndAddressLine2 = .CreateElement("AddressLine2")
                    ndAddressLine3 = .CreateElement("AddressLine3")
                    ndAddressLine4 = .CreateElement("AddressLine4")
                    ndAddressLine5 = .CreateElement("AddressLine5")
                    ndPostCode = .CreateElement("PostCode")
                    ndGender = .CreateElement("Gender")
                    ndHomeTelephoneNumber = .CreateElement("HomeTelephoneNumber")
                    ndWorkTelephoneNumber = .CreateElement("WorkTelephoneNumber")
                    ndMobileNumber = .CreateElement("MobileNumber")
                    ndEmailAddress = .CreateElement("EmailAddress")
                    ndDateBirth = .CreateElement("DateOfBirth")
                    ndContactViaMail = .CreateElement("ContactViaMail")
                    ndSubscription1 = .CreateElement("Subscription1")
                    ndSubscription2 = .CreateElement("Subscription2")
                    ndSubscription3 = .CreateElement("Subscription3")
                    ndContactViaMail1 = .CreateElement("ContactViaMail1")
                    ndContactViaMail2 = .CreateElement("ContactViaMail2")
                    ndContactViaMail3 = .CreateElement("ContactViaMail3")
                    ndContactViaMail4 = .CreateElement("ContactViaMail4")
                    ndContactViaMail5 = .CreateElement("ContactViaMail5")
                    ndExternalId1 = .CreateElement("ExternalId1")
                    ndExternalId2 = .CreateElement("ExternalId2")
                End With

                'Read the values for the response section
                dtCustomerDetails = ResultDataSet.Tables(1)
                dr = dtCustomerDetails.Rows(0)

                'Populate the nodes
                ndCustomerNo.InnerText = dr("CustomerNo")
                ndContactTitle.InnerText = dr("ContactTitle")
                ndContactInitials.InnerText = dr("ContactInitials")
                ndContactForename.InnerText = dr("ContactForename")
                ndContactSurname.InnerText = dr("ContactSurname")
                ndSalutation.InnerText = dr("Salutation")
                ndCompanyName.InnerText = dr("CompanyName")
                ndPositionInCompany.InnerText = dr("PositionInCompany")
                ndAddressLine1.InnerText = dr("AddressLine1")
                ndAddressLine2.InnerText = dr("AddressLine2")
                ndAddressLine3.InnerText = dr("AddressLine3")
                ndAddressLine4.InnerText = dr("AddressLine4")
                ndAddressLine5.InnerText = dr("AddressLine5")
                ndPostCode.InnerText = dr("Postcode")
                ndGender.InnerText = dr("Gender")
                ndHomeTelephoneNumber.InnerText = dr("HomeTelephoneNumber")
                ndWorkTelephoneNumber.InnerText = dr("WorkTelephoneNumber")
                ndMobileNumber.InnerText = dr("MobileNumber")
                ndEmailAddress.InnerText = dr("EmailAddress")
                ndDateBirth.InnerText = dr("DateBirth")
                ndContactViaMail.InnerText = dr("ContactViaMail")
                ndSubscription1.InnerText = dr("Subscription1")
                ndSubscription2.InnerText = dr("Subscription2")
                ndSubscription3.InnerText = dr("Subscription3")
                ndContactViaMail1.InnerText = dr("ContactViaMail1")
                ndContactViaMail2.InnerText = dr("ContactViaMail2")
                ndContactViaMail3.InnerText = dr("ContactViaMail3")
                ndContactViaMail4.InnerText = dr("ContactViaMail4")
                ndContactViaMail5.InnerText = dr("ContactViaMail5")
                ndExternalId1.InnerText = dr("ExternalId1")
                ndExternalId2.InnerText = dr("ExternalId2")

                'Set the xml nodes
                With ndCustomerDetails
                    .AppendChild(ndCustomerNo)
                    .AppendChild(ndContactTitle)
                    .AppendChild(ndContactInitials)
                    .AppendChild(ndContactForename)
                    .AppendChild(ndContactSurname)
                    .AppendChild(ndSalutation)
                    .AppendChild(ndCompanyName)
                    .AppendChild(ndPositionInCompany)
                    .AppendChild(ndAddressLine1)
                    .AppendChild(ndAddressLine2)
                    .AppendChild(ndAddressLine3)
                    .AppendChild(ndAddressLine4)
                    .AppendChild(ndAddressLine5)
                    .AppendChild(ndPostCode)
                    .AppendChild(ndGender)
                    .AppendChild(ndHomeTelephoneNumber)
                    .AppendChild(ndWorkTelephoneNumber)
                    .AppendChild(ndMobileNumber)
                    .AppendChild(ndEmailAddress)
                    .AppendChild(ndDateBirth)
                    .AppendChild(ndContactViaMail)
                    .AppendChild(ndSubscription1)
                    .AppendChild(ndSubscription2)
                    .AppendChild(ndSubscription3)
                    .AppendChild(ndContactViaMail1)
                    .AppendChild(ndContactViaMail2)
                    .AppendChild(ndContactViaMail3)
                    .AppendChild(ndContactViaMail4)
                    .AppendChild(ndContactViaMail5)
                    .AppendChild(ndExternalId1)
                    .AppendChild(ndExternalId2)
                End With

            End If

        End Sub

    End Class

End Namespace