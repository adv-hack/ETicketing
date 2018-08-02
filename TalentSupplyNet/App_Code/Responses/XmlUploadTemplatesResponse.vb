Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Upload Templates response
'
'       Date                        310707
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSUT - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlUploadTemplatesResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private ndHeader, ndProductCode, _
                ndLastName, ndExpiryDate As XmlNode

        Private atXmlNsXsi As XmlAttribute


        Protected Overrides Sub InsertBodyV1()
            Dim iCounter As Int32 = 0
            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("UploadTemplates")
                If Not Err.HasError Then
                    '-----------------------------------------------------------------------------
                    Err2 = CreateHeader()
                    ndProductCode.InnerText = "Hello"
                    Err2 = AppendHeader()
                    '-----------------------------------------------------------------------------
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
                    ndProductCode = .CreateElement("SKU")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSUT-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ' .AppendChild(ndProductCode)
                ' ndHeader.AppendChild(ndProductAlert)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSUT-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace