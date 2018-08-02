Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with customer retrieval responses
'
'       Date                        April 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSCR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerRetrievalResponse
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
                ndExternalId1, ndExternalId2, ndATSReady, ndPriceBand, ndLoyaltyPoints, _
                ndSTHolder, ndSCHolder, ndWebReady, ndStopCode As XmlNode
        Private dtCustomerDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerRetrieval")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateCustomerDetail(False)

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
                    .ErrorNumber = "TTPRSCR-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Overrides Sub InsertBodyV1_1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerRetrieval")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateCustomerDetail(True)

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
                    .ErrorNumber = "TTPRSCR-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Overrides Sub InsertBodyV1_2()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerRetrieval")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateCustomerDetail(True)

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
                    .ErrorNumber = "TTPRSCR-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Overrides Sub InsertBodyV1_3()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndCustomerInfo = .CreateElement("CustomerRetrieval")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateCustomerDetail(False)

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
                    .ErrorNumber = "TTPRSCR-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            'Read the values for the response section
            dtStatusDetails = ResultDataSet.Tables(0)
            dr = dtStatusDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccurred") = "E" Then
                errorOccurred = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Protected Sub CreateCustomerDetail(ByVal includeLoyaltyPoints As Boolean)

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
                    ndATSReady = .CreateElement("ATSReady")
                    ndPriceBand = .CreateElement("PriceBand")
                    If includeLoyaltyPoints Then
                        ndLoyaltyPoints = .CreateElement("LoyaltyPoints")
                    End If
                    ndWebReady = .CreateElement("WebReady")
                    ndSTHolder = .CreateElement("STHolder")
                    ndSCHolder = .CreateElement("SCHolder")
                    ndStopCode = .CreateElement("StopCode")
                End With

                'Read the values for the response section
                dtCustomerDetails = ResultDataSet.Tables(1)
                dr = dtCustomerDetails.Rows(0)

                'Populate the nodes
                ndCustomerNo.InnerText = dr("CustomerNumber")
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
                '-------------------------------
                ' Override ticketing default DOB
                '-------------------------------
                If ndDateBirth.InnerText = "19000000" Then
                    ndDateBirth.InnerText = String.Empty
                End If
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
                ndATSReady.InnerText = dr("ATSReady")
                ndPriceBand.InnerText = dr("PriceBand")
                If includeLoyaltyPoints Then
                    ndLoyaltyPoints.InnerText = dr("LoyaltyPoints")
                End If
                ndWebReady.InnerText = dr("WebReady")
                ndSTHolder.InnerText = dr("STHolder")
                ndSCHolder.InnerText = dr("SCHolder")
                ndStopCode.InnerText = dr("StopCode")

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
                    .AppendChild(ndATSReady)
                    .AppendChild(ndPriceBand)
                    If includeLoyaltyPoints Then
                        .AppendChild(ndLoyaltyPoints)
                    End If
                    .AppendChild(ndWebReady)
                    .AppendChild(ndSTHolder)
                    .AppendChild(ndSCHolder)
                    .AppendChild(ndStopCode)
                End With

            End If

        End Sub

    End Class

End Namespace