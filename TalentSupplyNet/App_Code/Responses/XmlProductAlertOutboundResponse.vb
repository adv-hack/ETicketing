Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with  Product Alert Output Responses
'
'       Date                        3rd Dec 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPO- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductAlertOutboundResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private ndHeader, ndProductAlert, ndProductCode, ndDescription, _
                ndCompany, ndEmailAddress, ndFirstName, _
                ndLastName, ndQuantity As XmlNode
        Private atXmlNsXsi As XmlAttribute

        Protected Overrides Sub InsertBodyV1()
            Dim dr As DataRow
            Dim dt As New DataTable
            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("ProductAlerts")
                If Not Err.HasError Then
                    dt = ResultDataSet.Tables(0)
                    If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                        For Each dr In dt.Rows
                            '--------------------------------------------------------------
                            Err2 = CreateHeader()
                            With dr
                                ndQuantity.InnerText = .Item(0)             '"Quantity")
                                ndProductCode.InnerText = .Item(1).trim     '"PRODUCT_CODE")
                                ndCompany.InnerText = .Item(2).trim         '"Company")
                                ndEmailAddress.InnerText = .Item(3).trim    '"EMailAddress")
                                ndFirstName.InnerText = .Item(4).trim       '"FirstName")
                                ndLastName.InnerText = .Item(5).trim        '"LastName")
                                ndDescription.InnerText = .Item(9).trim     '"Description")
                            End With
                            Err2 = AppendHeader()
                            '--------------------------------------------------------------
                        Next
                    Else
                        '------------------------------------------------------------------
                        '  No data so write dummy
                        '
                        Err2 = CreateHeader()
                        Err2 = AppendHeader()
                    End If
                End If
                '----------------------------------------------------------------------
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
                    ndProductCode = .CreateElement("ProductCode")
                    ndDescription = .CreateElement("Description")
                    ndCompany = .CreateElement("Company")
                    ndEmailAddress = .CreateElement("EMailAddress")
                    ndFirstName = .CreateElement("FirstName")
                    ndLastName = .CreateElement("LastName")
                    ndQuantity = .CreateElement("Quantity")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-15"
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
                    .AppendChild(ndQuantity)
                End With
                ndHeader.AppendChild(ndProductAlert)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace