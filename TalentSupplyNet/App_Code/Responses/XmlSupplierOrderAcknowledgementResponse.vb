Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Order Ack Responses
'
'       Date                        10/11/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlSupplierOrderAcknowledgementResponse

        Inherits XmlResponse
        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private atXmlNsXsi As XmlAttribute
        '
        Private ndHeader As XmlNode
        '
        Private atRMAReason As XmlAttribute

        Protected Overrides Sub InsertBodyV1()
            With MyBase.xmlDoc
                ndHeader = .CreateElement("SupplierOrderAcknowledgementResponse")

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

    End Class
End Namespace