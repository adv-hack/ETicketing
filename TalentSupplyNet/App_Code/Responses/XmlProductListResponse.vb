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

    Public Class XmlProductListResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndProductList, ndResponse, ndProducts, ndReturnCode As XmlNode
        Private ndProduct, ndProductCode, ndProductDescription, ndProductDate, _
                ndProductTime, ndProductDateISO, ndProductTime24H, _
                ndProductPriceBand, ndProductApplicable, ndProductAllocation As XmlNode
        Private ndProductType, ndProductRequiredMembership, ndProductRequiredMembershipDescription, _
                ndProductRequiredMembershipPurchased, ndProductEntryTime, ndProductTicketLimit, _
                ndProductStadiumCode, ndPriceCode, ndProductAssociatedTravelProductCode, ndProductReqdLoyaltyPoints, ndProductLoyaltyRequirementMet, ndProductLimitRequirementMet As XmlNode
        Private _deProductDetails As DEProductDetails
        Private dtProductListDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Public Property deProductDeatils() As DEProductDetails
            Get
                Return _deProductDetails
            End Get
            Set(ByVal value As DEProductDetails)
                _deProductDetails = value
            End Set
        End Property

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
                CreateProductList(False)

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

        Protected Overrides Sub InsertBodyV1_1()

            Try

                ' Create the six xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndProductList = .CreateElement("ProductList")
                    ndResponse = .CreateElement("Response")
                    ndProducts = .CreateElement("Products")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateProductList(True)

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

        Protected Sub CreateLoyaltySection()

        End Sub

        Protected Sub CreateProductList(ByVal isVersion1_1 As Boolean)

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
                        ndProductDescription = .CreateElement("ProductDescription")
                        ndProductDate = .CreateElement("ProductDate")
                        ndProductTime = .CreateElement("ProductTime")
                        ndProductDateISO = .CreateElement("ProductDateISO")
                        ndProductTime24H = .CreateElement("ProductTime24H")
                        ndProductPriceBand = .CreateElement("ProductPriceBand")
                        ndProductApplicable = .CreateElement("ProductApplicable")
                        ndProductAllocation = .CreateElement("ProductAllocation")
                        ndProductType = .CreateElement("ProductType")
                        ndProductRequiredMembership = .CreateElement("ProductRequiredMembership")
                        ndProductRequiredMembershipDescription = .CreateElement("ProductRequiredMembershipDescription")
                        ndProductRequiredMembershipPurchased = .CreateElement("ProductRequiredMembershipPurchased")
                        ndProductEntryTime = .CreateElement("ProductEntryTime")
                        ndProductTicketLimit = .CreateElement("ProductTicketLimit")
                        ndProductStadiumCode = .CreateElement("ProductStadiumCode")
                        ndPriceCode = .CreateElement("PriceCode")

                        ndProductAssociatedTravelProductCode = .CreateElement("ProductAssociatedTravelProductCode")
                        If isVersion1_1 Then
                            ndProductReqdLoyaltyPoints = .CreateElement("ProductRequiredLoyaltyPoints")
                            ndProductLoyaltyRequirementMet = .CreateElement("ProductLoyaltyRequirementMet")
                            ndProductLimitRequirementMet = .CreateElement("ProductLimitRequirementMet")
                        End If
                    End With

                    'Populate the nodes
                    ndProductCode.InnerText = dr("ProductCode")
                    ndProductDescription.InnerText = dr("ProductDescription")
                    ndProductDate.InnerText = dr("ProductDate")
                    ndProductTime.InnerText = dr("ProductTime")
                    Try
                        Dim sDate As String = dr("ProductMDTE08")
                        ndProductDateISO.InnerText = "20" & sDate.Substring(1, 2) & "-" & sDate.Substring(3, 2) & "-" & sDate.Substring(5, 2)
                    Catch ex As Exception
                        ndProductDateISO.InnerText = "invalid"
                    End Try
                    Try
                        ndProductTime24H.InnerText = Format("hh:mm", dr("ProductTime").ToString)
                        ndProductTime24H.InnerText = CType(dr("ProductTime"), DateTime).ToLongTimeString
                    Catch ex As Exception
                        ndProductTime24H.InnerText = "invalid"
                    End Try
                    ndProductPriceBand.InnerText = dr("ProductPriceBand")
                    '                    ndProductApplicable.InnerText = dr("ProductAvailForSale")
                    If dr("ProductAvailForSale").ToString.Trim = "Y" And dr("ProductOnSale").ToString.Trim = "Y" Then
                        ndProductApplicable.InnerText = "Y"
                    End If
                    ndProductType.InnerText = dr("ProductType")
                    ndProductRequiredMembership.InnerText = dr("ProductReqdMem")
                    ndProductRequiredMembershipDescription.InnerText = dr("ProductReqdMemDesc")
                    ndProductRequiredMembershipPurchased.InnerText = dr("ProductReqdMemOK")
                    ndProductEntryTime.InnerText = dr("ProductEntryTime")
                    Try
                        '  ndProductTicketLimit.InnerText = FormatNumber(dr("ProductTicketLimit").ToString.Trim, 0, TriState.False)
                        ndProductTicketLimit.InnerText = Utilities.CheckForDBNull_String(dr("ProductTicketLimit")).TrimStart("0")

                    Catch ex As Exception
                        ndProductTicketLimit.InnerText = String.Empty
                    End Try
                    ndProductStadiumCode.InnerText = Utilities.CheckForDBNull_String(dr("ProductStadium"))
                    ndPriceCode.InnerText = Utilities.CheckForDBNull_String(dr("PriceCode"))

                    ndProductAssociatedTravelProductCode.InnerText = Utilities.CheckForDBNull_String(dr("ProductAssocTravelProd"))

                    If isVersion1_1 Then
                        ndProductReqdLoyaltyPoints.InnerText = dr("ProductReqdLoyalityPoints")
                        ndProductLoyaltyRequirementMet.InnerText = Utilities.CheckForDBNull_String(dr("LoyaltyRequirementMet"))
                        ndProductLimitRequirementMet.InnerText = Utilities.CheckForDBNull_String(dr("LimitRequirementMet"))
                    End If
                    'Set the xml nodes
                    With ndProduct
                        .AppendChild(ndProductCode)
                        .AppendChild(ndProductDescription)
                        .AppendChild(ndProductDate)
                        .AppendChild(ndProductTime)
                        .AppendChild(ndProductDateISO)
                        .AppendChild(ndProductTime24H)
                        .AppendChild(ndProductPriceBand)
                        .AppendChild(ndProductApplicable)
                        '.AppendChild(ndProductAllocation)
                        .AppendChild(ndProductType)
                        .AppendChild(ndProductRequiredMembership)
                        .AppendChild(ndProductRequiredMembershipDescription)
                        .AppendChild(ndProductRequiredMembershipPurchased)
                        .AppendChild(ndProductEntryTime)
                        .AppendChild(ndProductTicketLimit)
                        .AppendChild(ndProductStadiumCode)
                        .AppendChild(ndPriceCode)
                        .AppendChild(ndProductAssociatedTravelProductCode)
                        If isVersion1_1 Then
                            .AppendChild(ndProductReqdLoyaltyPoints)
                            .AppendChild(ndProductLoyaltyRequirementMet)
                            .AppendChild(ndProductLimitRequirementMet)
                        End If
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