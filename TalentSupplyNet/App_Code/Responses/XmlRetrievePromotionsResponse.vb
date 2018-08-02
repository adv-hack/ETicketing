Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
Namespace Talent.TradingPortal
    Public Class XmlRetrievePromotionsResponse
        Inherits XmlResponse

        Private dtPomotionsListDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False
        Private ndsPriceCode(10) As XmlNode
        Private ndPromotionList, ndPromotions, ndPromotion, ndPromotionType, ndPriority, ndMatchType, _
        ndProductCode, ndStand, ndArea, ndPreReq, ndPriceCodes, ndPriceBand, ndShortDescription, _
        ndLongDescription, ndCompetitionCode, ndMaxDiscountPerProduct, ndMaxDiscountPerPromotion, _
        ndStartDate, ndEndDate, ndReturnCode, ndResponse As XmlNode



        Protected Overrides Sub InsertBodyV1()
            Dim ndHeaderRoot, ndHeaderRootHeader As XmlNode

            With MyBase.xmlDoc
                ndPromotionList = .CreateElement("PromotionsList")
                ndPromotion = .CreateElement("Promotion")
            End With

            If Not Err.HasError Then
                'Create the response xml section
                CreateResponseSection()

                'Create the promotions list
                CreatePromotionsList()
            End If

            With MyBase.xmlDoc
                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '


                ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                ndHeaderRoot.InsertAfter(ndPromotionList, ndHeaderRootHeader)

                'Insert the XSD reference & namespace as an attribute within the root node
            End With


        End Sub

        Protected Sub CreateResponseSection()

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            'Read the values for the response section
            If Not ResultDataSet Is Nothing AndAlso ResultDataSet.Tables.Count > 0 Then
                dtStatusDetails = ResultDataSet.Tables(0)
                If dtStatusDetails.Rows.Count > 0 Then
                    dr = dtStatusDetails.Rows(0)
                End If
            End If

        End Sub

        Protected Sub CreatePromotionsList()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                dtPomotionsListDetails = ResultDataSet.Tables(0)

                'Loop around the products
                For Each dr In dtPomotionsListDetails.Rows
                    ndPromotion = MyBase.xmlDoc.CreateElement("Promotion")
                    'Create the customer xml nodes
                    With MyBase.xmlDoc
                        ndPromotionType = .CreateElement("PromotionType")
                        ndPriority = .CreateElement("Priority")
                        ndMatchType = .CreateElement("MatchType")
                        ndProductCode = .CreateElement("ProductCode")
                        ndStand = .CreateElement("Stand")
                        ndArea = .CreateElement("Area")
                        ndPreReq = .CreateElement("PreReq")
                        ndPriceCodes = .CreateElement("PriceCodes")
                        For i As Integer = 1 To 10
                            ndsPriceCode(i) = .CreateElement("PriceCode" & i)
                        Next
                        ndPriceBand = .CreateElement("PriceBand")
                        ndShortDescription = .CreateElement("ShortDescription")
                        ndLongDescription = .CreateElement("LongDescription")
                        ndCompetitionCode = .CreateElement("CompetitionCode")
                        ndMaxDiscountPerProduct = .CreateElement("MaxDiscountPerProduct")
                        ndMaxDiscountPerPromotion = .CreateElement("MaxDiscountPerPromotion")
                        ndStartDate = .CreateElement("StartDate")
                        ndEndDate = .CreateElement("EndDate")
                    End With

                    'Populate the nodes
                    ndPromotionType.InnerText = CType(dr("PromotionType"), String)
                    ndPriority.InnerText = dr("Priority")
                    ndMatchType.InnerText = dr("MatchType")
                    ndProductCode.InnerText = dr("ProductCode")
                    ndStand.InnerText = dr("Stand")
                    ndArea.InnerText = dr("Area")
                    ndPreReq.InnerText = dr("PreReq")
                    For i As Integer = 1 To 10
                        ndsPriceCode(i).InnerText = dr("PriceCode" & i)
                        ndPriceCodes.AppendChild(ndsPriceCode(i))
                    Next

                    ndPriceBand.InnerText = dr("PriceBand")
                    ndShortDescription.InnerText = dr("ShortDescription")
                    ndLongDescription.InnerText = dr("Description1")
                    ndCompetitionCode.InnerText = dr("CompetitionCode")
                    ndMaxDiscountPerProduct.InnerText = dr("MaxPerProduct")
                    ndMaxDiscountPerPromotion.InnerText = dr("MaxPerPromotion")

                    Try
                        Dim sDate As String = Format(dr("StartDate"), "dd/MM/yyyy").ToString
                        ndStartDate.InnerText = sDate
                    Catch ex As Exception
                        ndStartDate.InnerText = ""
                    End Try
                    Try
                        Dim eDate As String = Format(dr("EndDate"), "dd/MM/yyyy")
                        ndEndDate.InnerText = eDate
                    Catch ex As Exception
                        ndStartDate.InnerText = ""
                    End Try

                    'Set the xml nodes
                    With ndPromotion
                        .AppendChild(ndPromotionType)
                        .AppendChild(ndPriority)
                        .AppendChild(ndMatchType)
                        .AppendChild(ndProductCode)
                        .AppendChild(ndStand)
                        .AppendChild(ndArea)
                        .AppendChild(ndPreReq)
                        .AppendChild(ndPriceCodes)
                        .AppendChild(ndPriceBand)
                        .AppendChild(ndShortDescription)
                        .AppendChild(ndLongDescription)
                        .AppendChild(ndCompetitionCode)
                        .AppendChild(ndMaxDiscountPerProduct)
                        .AppendChild(ndMaxDiscountPerPromotion)
                        .AppendChild(ndStartDate)
                        .AppendChild(ndEndDate)
                    End With

                    'Add the product to the product list
                    With ndPromotionList
                        .AppendChild(ndPromotion)
                    End With

                Next dr
            End If

        End Sub

    End Class

End Namespace
