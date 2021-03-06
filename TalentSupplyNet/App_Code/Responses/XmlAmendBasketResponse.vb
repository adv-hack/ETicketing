Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Basket responses
'
'       Date                        Mar 2007
'
'       Author                      Andy White 
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSABAS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAmendBasketResponse
        Inherits XmlResponse
        Private _dep As DEAmendBasket

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndErrorStatus, ndBasketHeader, ndBusinessUnit, ndPartner, ndUserID As XmlNode
        Private ndBasketLine, ndLineError, ndProduct, ndQuantity As XmlNode
        Private atXmlNsXsi, atErrorNumber As XmlAttribute

        Public Property Dep() As DEAmendBasket
            Get
                Return _dep
            End Get
            Set(ByVal value As DEAmendBasket)
                _dep = value
            End Set
        End Property
        Protected Overrides Sub InsertBodyV1()
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("BasketResponse")
                If Not Dep Is Nothing Then
                    '---------------------------------------------------------------------------------
                    CreateHeader()
                    ndBusinessUnit.InnerText = Dep.BusinessUnit.ToString
                    ndPartner.InnerText = Dep.PartnerCode.ToString
                    ndUserID.InnerText = Dep.UserID.ToString
                    If Not Err Is Nothing Then
                        ndErrorStatus.InnerText = Err.ErrorStatus
                        atErrorNumber = .CreateAttribute("ErrorNumber")
                        atErrorNumber.Value = Err.ErrorNumber
                        ndErrorStatus.Attributes.Append(atErrorNumber)
                    End If
                    AppendHeader()
                    '---------------------------------------------------------------------------------
                    If Dep.CollDEAlerts.Count > 0 Then

                        For iCounter = 1 To Dep.CollDEAlerts.Count
                            dea = Dep.CollDEAlerts.Item(iCounter)
                            Err = CreateDetails()
                            ndLineError.InnerText = dea.Description.ToString
                            ndProduct.InnerText = dea.ProductCode.ToString
                            ndQuantity.InnerText = dea.Quantity.ToString
                            Err = AppendDetails()
                        Next
                    End If
                Else
                    '---------------------------------------------------------------------------------
                    '   No data so write dummy
                    '
                    CreateHeader()
                    AppendHeader()
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
                    ndErrorStatus = .CreateElement("ErrorStatus")
                    ndBasketHeader = .CreateElement("BasketHeader")
                    ndBusinessUnit = .CreateElement("BusinessUnit")
                    ndPartner = .CreateElement("Partner")
                    ndUserID = .CreateElement("UserID")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Basket Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSABAS-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndHeader.AppendChild(ndBasketHeader)
                With ndBasketHeader
                    .AppendChild(ndErrorStatus)
                    .AppendChild(ndBusinessUnit)
                    .AppendChild(ndPartner)
                    .AppendChild(ndUserID)

                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append  Basket Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSABAS-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function CreateDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndBasketLine = .CreateElement("BasketLine")
                    ndLineError = .CreateElement("LineError")
                    ndProduct = .CreateElement("Product")
                    ndQuantity = .CreateElement("Quantity")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Invoice Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSABAS-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndHeader.AppendChild(ndBasketLine)
                With ndBasketLine
                    .AppendChild(ndLineError)
                    .AppendChild(ndProduct)
                    .AppendChild(ndQuantity)
                End With
                '-----------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSABAS-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace