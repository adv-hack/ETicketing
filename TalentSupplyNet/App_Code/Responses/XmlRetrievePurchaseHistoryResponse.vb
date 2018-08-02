Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Detail Response
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRetrievePurchaseHistoryResponse
        Inherits XmlResponse
        ' 
        Private ndProducts, ndHeaderRootHeader, ndHeaderRoot As XmlNode
        Private atXmlNsXsi As XmlAttribute

        Private dtProducts As DataTable
        '---------------------------------------------------------------------------------------------
        Private CarrierCode As String
        '---------------------------------------------------------------------------------------------

        Protected Overrides Sub InsertBodyV1()
            '--------------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet  
            ' 
            Try
                '--------------------------------------------------------------------------------------
                With MyBase.xmlDoc
                    ndProducts = .CreateElement("Products")
                    If Not Err.HasError Then
                        dtProducts = ResultDataSet.Tables.Item(0)
                        dtDetail = ResultDataSet.Tables.Item(1)
                        Err = InsertProducts()
                    End If
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                    ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                    ndHeaderRoot.InsertAfter(ndProducts, ndHeaderRootHeader)

                    'Insert the XSD reference & namespace as an attribute within the root node
                    atXmlNsXsi = CreateNamespaceAttribute()
                    ndHeaderRoot.Attributes.Append(atXmlNsXsi)
                End With
            Catch ex As Exception
            End Try
        End Sub

        Private Function InsertProducts() As ErrorObj
            Dim err2 As ErrorObj = Nothing
            For Each row As DataRow In dtProducts.Rows
                Dim ndProduct As XmlNode = xmlDoc.CreateElement("Product")
                Dim ndSaleDate As XmlNode = xmlDoc.CreateElement("SaleDate")
                Dim ndProductDescription As XmlNode = xmlDoc.CreateElement("ProductDescription")
                Dim ndSeat As XmlNode = xmlDoc.CreateElement("Seat")
                Dim ndSalesPrice As XmlNode = xmlDoc.CreateElement("SalesPrice")
                Dim ndBatchReference As XmlNode = xmlDoc.CreateElement("BatchReference")
                Dim ndPaymentReference As XmlNode = xmlDoc.CreateElement("PaymentReference")
                Dim ndStatusCode As XmlNode = xmlDoc.CreateElement("StatusCode")
                Dim ndLoyaltyPoints As XmlNode = xmlDoc.CreateElement("LoyaltyPoints")
                Dim ndPromotionID As XmlNode = xmlDoc.CreateElement("PromotionID")
                Dim ndProductCode As XmlNode = xmlDoc.CreateElement("ProductCode")
                Dim ndProductType As XmlNode = xmlDoc.CreateElement("ProductType")
                Dim ndProductDate As XmlNode = xmlDoc.CreateElement("ProductDate")

                ndSaleDate.InnerText = row("SaleDate").ToString
                ndProductDescription.InnerText = row("ProductDescription").ToString
                ndSeat.InnerText = row("Seat").ToString
                ndSalesPrice.InnerText = row("SalePrice").ToString
                ndBatchReference.InnerText = row("BatchReference").ToString
                ndPaymentReference.InnerText = row("PaymentReference").ToString
                ndStatusCode.InnerText = row("StatusCode").ToString
                ndLoyaltyPoints.InnerText = row("LoyaltyPoints").ToString
                ndPromotionID.InnerText = row("PromotionID1").ToString
                ndProductCode.InnerText = row("ProductCode").ToString
                ndProductType.InnerText = row("ProductType").ToString
                ndProductDate.InnerText = row("ProductDate").ToString

                ndProduct.AppendChild(ndSaleDate)
                ndProduct.AppendChild(ndProductDescription)
                ndProduct.AppendChild(ndSeat)
                ndProduct.AppendChild(ndSalesPrice)
                ndProduct.AppendChild(ndBatchReference)
                ndProduct.AppendChild(ndPaymentReference)
                ndProduct.AppendChild(ndStatusCode)
                ndProduct.AppendChild(ndLoyaltyPoints)
                ndProduct.AppendChild(ndPromotionID)
                ndProduct.AppendChild(ndProductCode)
                ndProduct.AppendChild(ndProductType)
                ndProduct.AppendChild(ndProductDate)

                ndProducts.AppendChild(ndProduct)
            Next
            Return err2
        End Function
    End Class

End Namespace