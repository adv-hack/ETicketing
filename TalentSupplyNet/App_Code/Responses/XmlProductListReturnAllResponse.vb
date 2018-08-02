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

    Public Class XmlProductListReturnAllResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndProductList, ndResponse, ndProducts, ndReturnCode As XmlNode
        Private ndProduct, ndProductCode, ndProductDescription, ndProductDate, _
                ndProductTime, ndProductDateISO, ndProductTime24H, _
                ndProductPriceBand, ndProductApplicable, ndProductAllocation As XmlNode
        Private ndProductType, ndProductRequiredMembership, ndProductRequiredMembershipDescription, _
                ndProductRequiredMembershipPurchased, ndProductEntryTime, ndProductTicketLimit, _
                ndProductStadiumCode, ndPriceCode, ndProductAssociatedTravelProductCode, ndProductReqdLoyaltyPoints, _
                ndProductLoyaltyRequirementMet, ndProductLimitRequirementMet As XmlNode

        'Pre-Req Nodes
        Private ndPreReq, ndPreReqProductGroup, ndPreReqDescription, ndPreReqMultiGroup, ndPreReqStadium, _
                ndPreReqValidationRule, ndPreReqComments, ndPreReqProduct, ndPreReqProductCode, _
                ndPreReqProductDescription, ndPreReqProductType, ndPreReqProductDate As XmlNode

        'Loyalty details nodes
        Private ndLoyaltyDetails, ndLoyaltyDetailsApplyRestriction, ndLoyaltyDetailsNoOfPointsAwarded, _
                ndLoyaltyDetailsUpdatePreviouslyAwardedPoints, ndLoyaltyDetailsUpdateFromDate, _
                ndLoyaltyDetailsUpdateToDate, ndLoyaltyDetailsNoOfPurchasePointsAwarded, _
                ndLoyaltyDetailsAwardToSeasonTicketHolders, ndLoyaltyDetailsSeasonTicketID, _
                ndLoyaltyDetailsPointsSchedule, ndLoyaltyPointsSchedule, ndLoyaltyPointsScheduleFrom, _
                ndLoyaltyPointsScheduleRequiredPoints As XmlNode

        Private _deProductDetails As DEProductDetails
        Private dtProductListDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Public Property deProductDetails() As DEProductDetails
            Get
                Return _deProductDetails
            End Get
            Set(ByVal value As DEProductDetails)
                _deProductDetails = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()

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

        Protected Sub CreatePreReqSection(ByVal dr As DataRow)            

            If Not errorOccurred Then
                ndPreReq = xmlDoc.CreateElement("PreReq")

                ndPreReqProductGroup = xmlDoc.CreateElement("ProductGroup")
                ndPreReqDescription = xmlDoc.CreateElement("Description")
                ndPreReqMultiGroup = xmlDoc.CreateElement("MultiGroup")
                ndPreReqStadium = xmlDoc.CreateElement("Stadium")
                ndPreReqValidationRule = xmlDoc.CreateElement("ValidationRule")
                ndPreReqComments = xmlDoc.CreateElement("Comments")

                ndPreReqProductGroup.InnerText = dr("PreReqProductGroup").ToString().Trim()
                ndPreReqDescription.InnerText = dr("PreReqDescription").ToString().Trim()
                ndPreReqMultiGroup.InnerText = dr("PreReqMultiGroup").ToString().Trim()
                ndPreReqStadium.InnerText = dr("PreReqStadium").ToString().Trim()
                ndPreReqValidationRule.InnerText = dr("PreReqValidationRule").ToString().Trim()
                ndPreReqComments.InnerText = dr("PreReqComments").ToString().Trim()


                With ndPreReq
                    .AppendChild(ndPreReqProductGroup)
                    .AppendChild(ndPreReqDescription)
                    .AppendChild(ndPreReqMultiGroup)
                    .AppendChild(ndPreReqStadium)
                    .AppendChild(ndPreReqValidationRule)
                    .AppendChild(ndPreReqComments)
                End With
                Dim ndPreReqProducts As XmlNode = xmlDoc.CreateElement("PreReqProducts")
                ndPreReq.AppendChild(ndPreReqProducts)
                For Each productRow As DataRow In ResultDataSet.Tables(3).Rows
                    If productRow("ProductCode").Equals(dr("ProductCode")) Then
                        ndPreReqProduct = xmlDoc.CreateElement("PreReqProduct")
                        ndPreReqProductCode = xmlDoc.CreateElement("PreReqProductCode")
                        ndPreReqProductDescription = xmlDoc.CreateElement("PreReqProductDescription")
                        ndPreReqProductType = xmlDoc.CreateElement("PreReqProductType")
                        ndPreReqProductDate = xmlDoc.CreateElement("PreReqProductDate")

                        ndPreReqProductCode.InnerText = productRow("PreReqProductCode").ToString().Trim()
                        ndPreReqProductDescription.InnerText = productRow("PreReqProductDescription").ToString().Trim()
                        ndPreReqProductType.InnerText = productRow("PreReqProductType").ToString().Trim()

                        Try
                            Dim sDate As String = productRow("PreReqProductDate")
                            ndPreReqProductDate.InnerText = ConvertFromDate7(sDate)
                        Catch ex As Exception
                            ndPreReqProductDate.InnerText = "invalid"
                        End Try

                        ndPreReqProduct.AppendChild(ndPreReqProductCode)
                        ndPreReqProduct.AppendChild(ndPreReqProductDescription)
                        ndPreReqProduct.AppendChild(ndPreReqProductType)
                        ndPreReqProduct.AppendChild(ndPreReqProductDate)
                        ndPreReqProducts.AppendChild(ndPreReqProduct)
                    End If
                Next
                ndProduct.AppendChild(ndPreReq)
            End If
        End Sub

        Protected Sub CreateLoyaltySection(ByVal dr As DataRow)            

            If Not errorOccurred Then

                ndLoyaltyDetails = xmlDoc.CreateElement("LoyaltyDetails")

                ndLoyaltyDetailsApplyRestriction = xmlDoc.CreateElement("ApplyRestriction")
                ndLoyaltyDetailsNoOfPointsAwarded = xmlDoc.CreateElement("NoOfPointsAwarded")
                ndLoyaltyDetailsUpdatePreviouslyAwardedPoints = xmlDoc.CreateElement("UpdatePreviouslyAwardedPoints")
                ndLoyaltyDetailsUpdateFromDate = xmlDoc.CreateElement("UpdateFromDate")
                ndLoyaltyDetailsUpdateToDate = xmlDoc.CreateElement("UpdateToDate")
                ndLoyaltyDetailsNoOfPurchasePointsAwarded = xmlDoc.CreateElement("NoOfPurchasePointsAwarded")
                ndLoyaltyDetailsAwardToSeasonTicketHolders = xmlDoc.CreateElement("AwardToSeasonTicketHolders")
                ndLoyaltyDetailsSeasonTicketID = xmlDoc.CreateElement("SeasonTicketID")
                ndLoyaltyDetailsPointsSchedule = xmlDoc.CreateElement("PointsSchedule")

                ndLoyaltyDetailsApplyRestriction.InnerText = dr("LoyaltyDetailsApplyRestriction").ToString().Trim()
                ndLoyaltyDetailsNoOfPointsAwarded.InnerText = dr("LoyaltyDetailsNoOfPointsAwarded").ToString().Trim()
                ndLoyaltyDetailsUpdatePreviouslyAwardedPoints.InnerText = dr("LoyaltyDetailsUpdatePreviouslyAwardedPoints").ToString().Trim()

                Try
                    Dim sDate As String = dr("LoyaltyDetailsUpdateFromDate")
                    ndLoyaltyDetailsUpdateFromDate.InnerText = ConvertFromDate7(sDate)
                Catch ex As Exception
                    ndLoyaltyDetailsUpdateFromDate.InnerText = "invalid"
                End Try

                Try
                    Dim sDate As String = dr("LoyaltyDetailsUpdateToDate")
                    ndLoyaltyDetailsUpdateToDate.InnerText = ConvertFromDate7(sDate)
                Catch ex As Exception
                    ndLoyaltyDetailsUpdateToDate.InnerText = "invalid"
                End Try
                ndLoyaltyDetailsNoOfPurchasePointsAwarded.InnerText = dr("LoyaltyDetailsNoOfPurchasePointsAwarded").ToString().Trim()
                ndLoyaltyDetailsAwardToSeasonTicketHolders.InnerText = dr("LoyaltyDetailsAwardToSeasonTicketHolders").ToString().Trim()
                ndLoyaltyDetailsSeasonTicketID.InnerText = dr("LoyaltyDetailsSeasonTicketID").ToString().Trim()


                With ndLoyaltyDetails
                    .AppendChild(ndLoyaltyDetailsApplyRestriction)
                    .AppendChild(ndLoyaltyDetailsNoOfPointsAwarded)
                    .AppendChild(ndLoyaltyDetailsUpdatePreviouslyAwardedPoints)
                    .AppendChild(ndLoyaltyDetailsUpdateFromDate)
                    .AppendChild(ndLoyaltyDetailsUpdateToDate)
                    .AppendChild(ndLoyaltyDetailsNoOfPurchasePointsAwarded)
                    .AppendChild(ndLoyaltyDetailsAwardToSeasonTicketHolders)
                    .AppendChild(ndLoyaltyDetailsSeasonTicketID)
                    .AppendChild(ndLoyaltyDetailsPointsSchedule)
                End With
                For Each productRow As DataRow In ResultDataSet.Tables(2).Rows
                    If productRow("ProductCode").Equals(dr("ProductCode")) Then
                        ndLoyaltyPointsSchedule = xmlDoc.CreateElement("Schedule")
                        ndLoyaltyPointsScheduleFrom = xmlDoc.CreateElement("From")
                        ndLoyaltyPointsScheduleRequiredPoints = xmlDoc.CreateElement("RequiredPoints")

                        Try
                            Dim sDate As String = productRow("From")
                            ndLoyaltyPointsScheduleFrom.InnerText = ConvertFromDate7(sDate)
                        Catch ex As Exception
                            ndLoyaltyPointsScheduleFrom.InnerText = "invalid"
                        End Try

                        ndLoyaltyPointsScheduleRequiredPoints.InnerText = productRow("RequiredPoints").ToString().Trim()

                        ndLoyaltyDetailsPointsSchedule.AppendChild(ndLoyaltyPointsSchedule)
                        ndLoyaltyPointsSchedule.AppendChild(ndLoyaltyPointsScheduleFrom)
                        ndLoyaltyPointsSchedule.AppendChild(ndLoyaltyPointsScheduleRequiredPoints)
                    End If
                Next
                ndProduct.AppendChild(ndLoyaltyDetails)
            End If
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
                        ndPricecode = .CreateElement("PriceCode")
                        ndProductAssociatedTravelProductCode = .CreateElement("ProductAssociatedTravelProductCode")
                        ndProductReqdLoyaltyPoints = .CreateElement("ProductRequiredLoyaltyPoints")
                        ndProductLoyaltyRequirementMet = .CreateElement("ProductLoyaltyRequirementMet")
                        ndProductLimitRequirementMet = .CreateElement("ProductLimitRequirementMet")
                    End With

                    'Populate the nodes
                    ndProductCode.InnerText = dr("ProductCode").ToString().Trim()
                    ndProductDescription.InnerText = dr("ProductDescription").ToString().Trim()
                    ndProductDate.InnerText = dr("ProductDate").ToString().Trim()
                    ndProductTime.InnerText = dr("ProductTime").ToString().Trim()
                    Try
                        Dim sDate As String = dr("ProductMDTE08")
                        ndProductDateISO.InnerText = ConvertFromDate7(sDate)
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
                    ndProductType.InnerText = dr("ProductType").ToString().Trim()
                    ndProductRequiredMembership.InnerText = dr("ProductReqdMem").ToString().Trim()
                    ndProductRequiredMembershipDescription.InnerText = dr("ProductReqdMemDesc").ToString().Trim()
                    ndProductRequiredMembershipPurchased.InnerText = dr("ProductReqdMemOK").ToString().Trim()
                    ndProductEntryTime.InnerText = dr("ProductEntryTime").ToString().Trim()
                    ndProductTicketLimit.InnerText = FormatNumber(dr("ProductTicketLimit").ToString.Trim, 0, TriState.False)
                    ndProductStadiumCode.InnerText = dr("ProductStadium").ToString().Trim()
                    ndPriceCode.InnerText = Utilities.CheckForDBNull_String(dr("PriceCode").ToString().Trim())
                    ndProductAssociatedTravelProductCode.InnerText = dr("ProductAssocTravelProd").ToString().Trim()
                    ndProductReqdLoyaltyPoints.InnerText = dr("ProductReqdLoyalityPoints").ToString().Trim()
                    ndProductLoyaltyRequirementMet.InnerText = dr("LoyaltyRequirementMet").ToString().Trim()
                    ndProductLimitRequirementMet.InnerText = dr("LimitRequirementMet").ToString().Trim()
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
                        .AppendChild(ndProductReqdLoyaltyPoints)
                        .AppendChild(ndProductLoyaltyRequirementMet)
                        .AppendChild(ndProductLimitRequirementMet)
                    End With
                    CreatePreReqSection(dr)
                    CreateLoyaltySection(dr)

                    'Add the product to the product list
                    With ndProducts
                        .AppendChild(ndProduct)
                    End With
                    
                Next dr
                

            End If

        End Sub

    End Class

End Namespace