Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with product list responses
'
'       Date                        May 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductStadiumAvailabilityResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndProductList, ndResponse, ndProducts, ndReturnCode As XmlNode
        Private ndProduct, ndProductCode, ndStandCode, ndAreaCode, _
                ndAdditionalText, ndAvailability, ndCapacity, ndReserved As XmlNode
        Private dtProductListDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndProductList = .CreateElement("ProductList")
                    ndResponse = .CreateElement("Response")
                    ndProducts = .CreateElement("Products")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateProductList()

                'Populate the xml document
                With ndProductList
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndProducts)
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
                ndHeader.InsertAfter(ndProductList, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPD-01"
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

        Protected Sub CreateProductList()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                dtProductListDetails = ResultDataSet.Tables(1)

                'Loop around the products
                For Each dr In dtProductListDetails.Rows

                    'Create the customer xml nodes


                    With MyBase.xmlDoc
                        ndProduct = .CreateElement("Product")
                        ndProductCode = .CreateElement("ProductCode")
                        ndStandCode = .CreateElement("StandCode")
                        ndAreaCode = .CreateElement("AreaCode")
                        ndAvailability = .CreateElement("Availability")
                        ndAdditionalText = .CreateElement("AdditionalText")
                        ndCapacity = .CreateElement("Capacity")
                        ndReserved = .CreateElement("Reserved")
                    End With

                    'Populate the nodes
                    ndProductCode.InnerText = dr("ProductCode")
                    ndStandCode.InnerText = dr("StandCode")
                    ndAreaCode.InnerText = dr("AreaCode")
                    ndAvailability.InnerText = dr("Availability")
                    ndAdditionalText.InnerText = dr("AdditionalText")
                    ndCapacity.InnerText = dr("Capacity")
                    ndReserved.InnerText = dr("Reserved")

                    'Set the xml nodes
                    With ndProduct
                        .AppendChild(ndProductCode)
                        .AppendChild(ndStandCode)
                        .AppendChild(ndAreaCode)
                        .AppendChild(ndAvailability)
                        .AppendChild(ndAdditionalText)
                        .AppendChild(ndCapacity)
                        .AppendChild(ndReserved)
                    End With

                    'Add the product to the product list
                    With ndProducts
                        .AppendChild(ndProduct)
                    End With

                Next dr
            End If

        End Sub

    End Class

End Namespace