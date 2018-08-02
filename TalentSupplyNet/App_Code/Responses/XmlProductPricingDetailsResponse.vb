Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with product pricing details responses
'
'       Date                        Sept 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPPD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductPricingDetailsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndProductPricingDetails, ndResponse, ndPrices, ndReturnCode As XmlNode
        Private ndPriceDetails, ndStandCode, ndStandDescription, ndAreaCode, ndAreaDescription As XmlNode
        Private ndPriceBandDetails, ndPriceBand, ndPrice As XmlNode
        Private ndPriceBands, ndPriceBandDescription, ndPriceBandPriority As XmlNode
        Private dtPriceDetails As DataTable
        Private dtStandDetails As DataTable
        Private dtPriceBandDetails As DataTable
        Private dtStatusDetails, dtStatusDetails2, dtStatusDetails4 As DataTable
        Private errorOccurred As Boolean = False
        Private priceBandsExist As Boolean = False
        Private arrayStandAreas(200) As String

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndProductPricingDetails = .CreateElement("ProductPricingDetails")
                    ndResponse = .CreateElement("Response")
                    ndPrices = .CreateElement("Prices")
                    ndPriceBands = .CreateElement("PriceBands")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the stand details section
                CreatePriceDetailsSection()

                'Create the price band section
                CreatePriceBandSection()

                'Populate the xml document
                With ndProductPricingDetails
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndPrices)
                        If priceBandsExist Then
                            .AppendChild(ndPriceBands)
                        End If
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
                ndHeader.InsertAfter(ndProductPricingDetails, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPPD-01"
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

            '
            'Read the first response section (Prices)
            dtStatusDetails = ResultDataSet.Tables(0)
            dr = dtStatusDetails.Rows(0)
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccurred") = "E" Then
                errorOccurred = True
            Else
                '
                'Read the Bands response section (Bands)
                dtStatusDetails4 = ResultDataSet.Tables(4)
                dr = dtStatusDetails4.Rows(0)
                If dr("ErrorOccurred") = "E" Then
                    ndReturnCode.InnerText = dr("ReturnCode")
                    errorOccurred = True
                End If

            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Protected Sub CreatePriceDetailsSection()

            Dim dr As DataRow
            Dim saveStand As String = ""
            Dim saveArea As String = ""

            If Not errorOccurred Then

                'Read the values for the response section
                dtPriceDetails = ResultDataSet.Tables(1)

                'Loop around the stands
                For Each dr In dtPriceDetails.Rows

                    'Create the stand codes if required
                    If saveStand <> dr("StandCode") Or saveArea <> dr("AreaCode") Then

                        'Add the xml section when the stnd has changed
                        If saveStand.Trim <> "" Or saveArea.Trim <> "" Then
                            With ndPrices
                                .AppendChild(ndPriceDetails)
                            End With
                        End If

                        'Create a new xml section for the new stand
                        With MyBase.xmlDoc
                            ndPriceDetails = .CreateElement("PriceDetails")
                            ndStandCode = .CreateElement("StandCode")
                            ndStandDescription = .CreateElement("StandDescription")
                            ndAreaCode = .CreateElement("AreaCode")
                            ndAreaDescription = .CreateElement("AreaDescription")
                        End With

                        'Populate the stand and area nodes
                        ndStandCode.InnerText = dr("StandCode")
                        ndStandDescription.InnerText = GetStandDescription(dr("StandCode"))
                        ndAreaCode.InnerText = dr("AreaCode")
                        ndAreaDescription.InnerText = GetAreaDescription(dr("StandCode"), dr("AreaCode"))

                        'Add the stand and area nodes
                        With ndPriceDetails
                            .AppendChild(ndStandCode)
                            .AppendChild(ndStandDescription)
                            .AppendChild(ndAreaCode)
                            .AppendChild(ndAreaDescription)
                        End With

                        'Save the stand code for the next iteration
                        saveStand = dr("StandCode")
                        saveArea = dr("AreaCode")
                    End If

                    'Create the area xml nodes
                    With MyBase.xmlDoc
                        ndPriceBandDetails = .CreateElement("PriceBandDetails")
                        ndPriceBand = .CreateElement("PriceBand")
                        ndPrice = .CreateElement("Price")
                    End With

                    'Populate the PriceBandDetails nodes
                    ndPriceBand.InnerText = dr("PriceBand")
                    ndPrice.InnerText = dr("Price").ToString()

                    'Set the xml nodes
                    With ndPriceBandDetails
                        .AppendChild(ndPriceBand)
                        .AppendChild(ndPrice)
                    End With

                    'Add the area to the stand list
                    With ndPriceDetails
                        .AppendChild(ndPriceBandDetails)
                    End With

                Next dr
            End If

            'Close the last stand section
            If saveStand.Trim <> "" Then
                With ndPrices
                    .AppendChild(ndPriceDetails)
                End With
            End If

        End Sub

        Protected Sub CreatePriceBandSection()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                dtPriceBandDetails = ResultDataSet.Tables(5)

                'Loop around the stands
                For Each dr In dtPriceBandDetails.Rows

                    priceBandsExist = True

                    'Create the area xml nodes
                    With MyBase.xmlDoc
                        ndPriceBandDetails = .CreateElement("PriceBandDetails")
                        ndPriceBand = .CreateElement("PriceBand")
                        ndPriceBandDescription = .CreateElement("PriceBandDescription")
                        ndPriceBandPriority = .CreateElement("PriceBandPriority")
                    End With

                    'Populate the area nodes
                    ndPriceBand.InnerText = dr("PriceBand")
                    ndPriceBandDescription.InnerText = dr("PriceBandDescription")
                    ndPriceBandPriority.InnerText = dr("PriceBandPriority")

                    'Set the xml nodes
                    With ndPriceBandDetails
                        .AppendChild(ndPriceBand)
                        .AppendChild(ndPriceBandDescription)
                        .AppendChild(ndPriceBandPriority)
                    End With

                    'Add the price band section
                    With ndPriceBands
                        .AppendChild(ndPriceBandDetails)
                    End With

                Next dr

            End If

        End Sub

        Protected Function GetStandDescription(ByVal sStandCode As String) As String

            Dim sStandDescription As String = String.Empty

            'Read the values for the response section
            dtStandDetails = ResultDataSet.Tables(3)
            Dim selectedRows As DataRow()
            selectedRows = dtStandDetails.Select("StandCode = '" & sStandCode & "'")
            If Not selectedRows(0) Is Nothing Then
                sStandDescription = selectedRows(0)("StandDescription")
            End If
            Return sStandDescription
        End Function

        Protected Function GetAreaDescription(ByVal sStandCode As String, ByVal sAreaCode As String) As String

            Dim sAreaDescription As String = String.Empty

            'Read the values for the response section
            dtStandDetails = ResultDataSet.Tables(3)
            Dim selectedRows As DataRow()
            selectedRows = dtStandDetails.Select("StandCode = '" & sStandCode & "' AND AreaCode = '" & sAreaCode & "'")
            If Not selectedRows(0) Is Nothing Then
                sAreaDescription = selectedRows(0)("AreaDescription")
            End If
            Return sAreaDescription
        End Function
    End Class

End Namespace