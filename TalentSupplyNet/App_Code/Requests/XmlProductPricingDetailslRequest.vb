Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports System.Data
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Pricing Details Requests
'
'       Date                        Sept 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPPD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductPricingDetailsRequest
        Inherits XmlRequest

        Private _deprod As New DEProductDetails
        Public Property Deprod() As DEProductDetails
            Get
                Return _deprod
            End Get
            Set(ByVal value As DEProductDetails)
                _deprod = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlProductPricingDetailsResponse = CType(xmlResp, XmlProductPricingDetailsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim PRODUCT As New TalentProduct
            Dim STANDS As New TalentProduct
            Dim BANDS As New TalentProduct
            Dim ResultDataSet As New DataSet
            If err.HasError Then
                xmlResp.Err = err
            Else
                '
                ' 1. Retrieve the pricing details from Ticketing
                '------------------------------------------------
                With PRODUCT
                    .Settings = Settings
                    Deprod.Src = "S"
                    .De = Deprod
                    err = .ProductPricingDetails
                End With

                If Not err.HasError Then
                    ' (Extract any error information from the data set)
                    Dim dr1 As DataRow
                    Dim dtStatusDetails1 As New DataTable
                    dtStatusDetails1 = PRODUCT.ResultDataSet.Tables(0).Copy
                    dr1 = dtStatusDetails1.Rows(0)
                    If dr1("ErrorOccurred") <> "E" Then
                        '
                        ' 2. Retrieve the stand and area descriptions from Ticketing
                        '------------------------------------------------------------
                        With STANDS
                            .Settings = Settings
                            Deprod.StadiumCode = dr1("StadiumCode")
                            Deprod.Src = "S"
                            .De = Deprod
                            '                            err = .AvailableStands
                            err = .StandDescriptions
                        End With

                        If Not err.HasError Then

                            '
                            ' 3. Retrieve the price bands and descriptions from Ticketing
                            '-------------------------------------------------------------
                            With BANDS
                                .Settings = Settings
                                Deprod.Src = "S"
                                .De = Deprod
                                err = .ProductDetails
                            End With

                            If Not err.HasError Then
                                '
                                ' Merge the tables (from STANDS and BANDS) to single dataset
                                Dim dtStandDetails As New DataTable
                                dtStandDetails.Merge(STANDS.ResultDataSet.Tables(1).Copy)
                                Dim dtStatusDetailsBands As New DataTable
                                dtStatusDetailsBands.Merge(BANDS.ResultDataSet.Tables(0).Copy)
                                Dim dtBandDetails As New DataTable
                                dtBandDetails.Merge(BANDS.ResultDataSet.Tables(1).Copy)
                                '
                                ' Add the tables to PRODUCT dataset
                                PRODUCT.ResultDataSet.Tables.Add(dtStandDetails)
                                PRODUCT.ResultDataSet.Tables.Add(dtStatusDetailsBands)
                                PRODUCT.ResultDataSet.Tables.Add(dtBandDetails)
                            End If
                        End If
                    End If
                End If
            End If

            'Create the response
            xmlResp.Err = err
            xmlAction.ResultDataSet = PRODUCT.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductPricingDetailsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "PricingDetails"

                            Deprod = New DEProductDetails
                            With Deprod
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Product details
                                        '
                                        Case Is = "ProductCode"
                                            .ProductCode = Node2.InnerText

                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPPD-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace