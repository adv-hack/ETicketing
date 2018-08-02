Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports System.Data
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Details Requests
'
'       Date                        May 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductDetailsRequest
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

            Dim xmlAction As XmlProductDetailsResponse = CType(xmlResp, XmlProductDetailsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim STANDS As New TalentProduct
            Dim PRODUCT As New TalentProduct
            Dim ResultDataSet As New DataSet
            If err.HasError Then
                xmlResp.Err = err
            Else
                'Retrieve the available stands
                With STANDS
                    .Settings = Settings
                    .De = Deprod
                    .De.Src = "S"
                    err = .AvailableStands
                End With
                If Not err.HasError Then

                    'Extract the information from the data set
                    Dim dr As DataRow
                    Dim dtStatusDetails As New DataTable
                    dtStatusDetails = STANDS.ResultDataSet.Tables(0).Copy

                    'Check for an internal error code
                    dr = dtStatusDetails.Rows(0)
                    If dr("ErrorOccurred") <> "E" Then

                        'Retrieve the product details
                        With PRODUCT
                            .Settings = Settings
                            Deprod.Src = "S"
                            .De = Deprod
                            err = .ProductDetails
                        End With
                        If Not err.HasError Then

                            'Merge the two data sets
                            Dim dtStatusDetails2 As New DataTable
                            dtStatusDetails2.Merge(PRODUCT.ResultDataSet.Tables(0).Copy)
                            Dim dtPriceDetails As New DataTable
                            dtPriceDetails.Merge(PRODUCT.ResultDataSet.Tables(1).Copy)

                            'Add the product details into the stands data set
                            STANDS.ResultDataSet.Tables.Add(dtStatusDetails2)
                            STANDS.ResultDataSet.Tables.Add(dtPriceDetails)
                        End If
                    End If
                End If
            End If

            'Create the response
            xmlResp.Err = err
            xmlAction.ResultDataSet = STANDS.ResultDataSet
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
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductDetailsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "ProductDetails"

                            Deprod = New DEProductDetails
                            With Deprod
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Product details
                                        '
                                        Case Is = "SessionId"
                                            .SessionId = Node2.InnerText

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
                    .ErrorNumber = "TTPRQPD-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace