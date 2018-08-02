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

    Public Class XmlProductSeatAvailabilityResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndProductList, ndResponse, ndRows, ndErrorOccurred, _
                ndReturnCode, ndColumnTotal, ndRowTotal As XmlNode
        Private ndRow As XmlNode
        Private atRowName, atRowCapacity, atRowSequence, atRowDetail As XmlAttribute
        Private dtProductListDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndProductList = .CreateElement("ProductList")
                    ndResponse = .CreateElement("Response")
                    ndRows = .CreateElement("Rows")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateProductList()

                'Populate the xml document
                With ndProductList
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndRows)
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
                ndColumnTotal = .CreateElement("ColumnTotal")
                ndRowTotal = .CreateElement("RowTotal")
            End With

            'Read the values for the response section
            dtStatusDetails = ResultDataSet.Tables(0)
            dr = dtStatusDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            ndRowTotal.InnerText = dr("RowTotal")
            ndColumnTotal.InnerText = dr("ColTotal")

            If dr("ErrorOccurred") = "E" Then
                errorOccurred = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndRowTotal)
                .AppendChild(ndColumnTotal)
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
                        ndRow = .CreateElement("Row")
                        atRowName = .CreateAttribute("RowName")
                        atRowCapacity = .CreateAttribute("RowCapacity")
                        atRowSequence = .CreateAttribute("RowSequence")
                        atRowDetail = .CreateAttribute("RowDetail")
                    End With

                    'Populate the nodes
                    atRowName.Value = dr("RowName")
                    atRowCapacity.Value = dr("RowCapacity")
                    atRowSequence.Value = dr("RowSequence")
                    atRowDetail.Value = dr("RowDetail")

                    'Add Attributes to Row Element
                    ndRow.Attributes.Append(atRowName)
                    ndRow.Attributes.Append(atRowCapacity)
                    ndRow.Attributes.Append(atRowSequence)
                    ndRow.Attributes.Append(atRowDetail)


                    'Set the xml nodes
                    With ndRows
                        .AppendChild(ndRow)
                    End With

                    'Add the product to the product list
                    With ndProductList
                        .AppendChild(ndRows)
                    End With

                Next dr
            End If

        End Sub

    End Class

End Namespace