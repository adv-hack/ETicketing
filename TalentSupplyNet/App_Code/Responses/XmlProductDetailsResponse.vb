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

    Public Class XmlProductDetailsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndProductDetails, ndResponse, ndStands, ndReturnCode As XmlNode
        Private ndStandDetails, ndStandCode, ndStandDescription, ndAreaDetails, ndAreaCode, _
                ndAreaDescription, ndLowestPrice, ndHighestPrice As XmlNode
        Private ndPriceBands, ndPriceBandDetails, ndPriceBand, ndPriceBandDescription, _
                ndPriceBandPriority As XmlNode
        Private dtStandDetails As DataTable
        Private dtStatusDetails, dtStatusDetails3 As DataTable
        Private dtPriceBandDetails As DataTable
        Private errorOccurred As Boolean = False
        Private priceBandsExist As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndProductDetails = .CreateElement("ProductDetails")
                    ndResponse = .CreateElement("Response")
                    ndStands = .CreateElement("Stands")
                    ndPriceBands = .CreateElement("PriceBands")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the stand details section
                CreateStandDetailsSection()

                'Create the price band section
                CreatePriceBandSection()

                'Populate the xml document
                With ndProductDetails
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndStands)
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
                ndHeader.InsertAfter(ndProductDetails, ndHeaderRootHeader)

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
            Else
                'Read the second response section
                dtStatusDetails3 = ResultDataSet.Tables(3)
                dr = dtStatusDetails3.Rows(0)
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

        Protected Sub CreateStandDetailsSection()

            Dim dr As DataRow
            Dim saveStand As String = ""

            If Not errorOccurred Then

                'Read the values for the response section
                dtStandDetails = ResultDataSet.Tables(1)

                'Loop around the stands
                For Each dr In dtStandDetails.Rows

                    'Create the stand codes if required
                    If saveStand <> dr("StandCode") Then

                        'Add the xml section when the stnd has changed
                        If saveStand.Trim <> "" Then
                            With ndStands
                                .AppendChild(ndStandDetails)
                            End With
                        End If

                        'Create a new xml section for the new stand
                        With MyBase.xmlDoc
                            ndStandDetails = .CreateElement("StandDetails")
                            ndStandCode = .CreateElement("StandCode")
                            ndStandDescription = .CreateElement("StandDescription")
                        End With

                        'Populate the stand nodes
                        ndStandCode.InnerText = dr("StandCode")
                        ndStandDescription.InnerText = dr("StandDescription")

                        'Add the stand nodes
                        With ndStandDetails
                            .AppendChild(ndStandCode)
                            .AppendChild(ndStandDescription)
                        End With

                        'Save the stand code for the next iteration
                        saveStand = dr("StandCode")
                    End If

                    'Create the area xml nodes
                    With MyBase.xmlDoc
                        ndAreaDetails = .CreateElement("AreaDetails")
                        ndAreaCode = .CreateElement("AreaCode")
                        ndAreaDescription = .CreateElement("AreaDescription")
                        '                        ndLowestPrice = .CreateElement("LowestPrice")
                        '                        ndHighestPrice = .CreateElement("HighestPrice")
                    End With

                    'Populate the area nodes
                    ndAreaCode.InnerText = dr("AreaCode")
                    ndAreaDescription.InnerText = dr("AreaDescription")
                    '                    ndLowestPrice.InnerText = dr("LowestPrice")
                    '                    ndHighestPrice.InnerText = dr("HighestPrice")

                    'Set the xml nodes
                    With ndAreaDetails
                        .AppendChild(ndAreaCode)
                        .AppendChild(ndAreaDescription)
                        '                        .AppendChild(ndLowestPrice)
                        '                        .AppendChild(ndHighestPrice)
                    End With

                    'Add the area to the stand list
                    With ndStandDetails
                        .AppendChild(ndAreaDetails)
                    End With

                Next dr
            End If

            'Close the last stand scetion
            If saveStand.Trim <> "" Then
                With ndStands
                    .AppendChild(ndStandDetails)
                End With
            End If

        End Sub

        Protected Sub CreatePriceBandSection()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                dtPriceBandDetails = ResultDataSet.Tables(4)

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

    End Class

End Namespace