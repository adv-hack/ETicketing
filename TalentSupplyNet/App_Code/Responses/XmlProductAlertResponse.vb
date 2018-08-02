Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with  Product Alert Responses
'
'       Date                        15th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductAlertResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private ndHeader, ndProductAlert, ndProductCode, ndDescription, _
                ndCompany, ndEmailAddress, ndFirstName, _
                ndLastName, ndExpiryDate As XmlNode
        Private atXmlNsXsi As XmlAttribute
        Private _depa As DEProductAlert

        Public Property Depa() As DEProductAlert
            Get
                Return _depa
            End Get
            Set(ByVal value As DEProductAlert)
                _depa = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()
            Dim dea As DEAlerts                     ' Items
            Dim iCounter As Int32 = 0
            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("ProductAlerts")
                If Not Err.HasError Then
                    For iCounter = 1 To Depa.CollDEAlerts.Count
                        dea = Depa.CollDEAlerts.Item(iCounter)
                        '-----------------------------------------------------------------------------
                        Err2 = CreateHeader()
                        With dea
                            ndProductCode.InnerText = dea.ProductCode
                            ndDescription.InnerText = dea.Description
                            ndCompany.InnerText = dea.BranchName
                            ndEmailAddress.InnerText = dea.EMailAddress
                            ndFirstName.InnerText = dea.FirstName
                            ndLastName.InnerText = dea.LastName
                            ndExpiryDate.InnerText = dea.ExpiryDate
                        End With
                        Err2 = AppendHeader()
                        '-----------------------------------------------------------------------------
                    Next
                End If
                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                ndHeaderRoot.InsertAfter(ndHeader, ndHeaderRootHeader)

                'Insert the XSD reference & namespace as an attribute within the root node
                atXmlNsXsi = CreateNamespaceAttribute()
                ndHeaderRoot.Attributes.Append(atXmlNsXsi)
            End With
        End Sub

        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndProductAlert = .CreateElement("ProductAlert")
                    ndProductCode = .CreateElement("SKU")
                    ndDescription = .CreateElement("Description")
                    ndCompany = .CreateElement("Company")
                    ndEmailAddress = .CreateElement("EMailAddress")
                    ndFirstName = .CreateElement("FirstName")
                    ndLastName = .CreateElement("LastName")
                    ndExpiryDate = .CreateElement("ExpiryDate")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPA-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndProductAlert
                    .AppendChild(ndProductCode)
                    .AppendChild(ndDescription)
                    .AppendChild(ndCompany)
                    .AppendChild(ndEmailAddress)
                    .AppendChild(ndFirstName)
                    .AppendChild(ndLastName)
                    .AppendChild(ndExpiryDate)
                End With
                ndHeader.AppendChild(ndProductAlert)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPA-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace