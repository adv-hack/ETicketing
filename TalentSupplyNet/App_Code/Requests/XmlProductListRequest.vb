Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product List Retrieval Requests
'
'       Date                        May 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPL- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductListRequest
        Inherits XmlRequest

        Private _deProductDetails As DEProductDetails

        Public Property DEProductDetails() As DEProductDetails
            Get
                Return _deProductDetails
            End Get
            Set(ByVal value As DEProductDetails)
                _deProductDetails = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlProductListResponse = CType(xmlResp, XmlProductListResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV1_1()
            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim PRODUCT As New TalentProduct
            If err.HasError Then
                xmlResp.Err = err
            Else
                With PRODUCT
                    .Settings = Settings
                    If MyBase.DocumentVersion = "1.1" Then
                        .De = DEProductDetails
                    End If
                    PRODUCT.De.Src = "S"
                    err = .ProductList
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = PRODUCT.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductListRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "ProductList"
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPL-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_1"
            Dim err As New ErrorObj
            DEProductDetails = New DEProductDetails

            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductListRequest").ChildNodes                    
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "ProductList"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "StadiumCodes"
                                        DEProductDetails.StadiumCode = Node2.InnerText
                                    Case Is = "CustomerNo"
                                        DEProductDetails.CustomerNumber = Node2.InnerText
                                    Case Is = "ProductType"
                                        DEProductDetails.ProductType = Node2.InnerText
                                End Select
                            Next
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPL-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
    End Class

End Namespace