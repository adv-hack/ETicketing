Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to load Product relations
'
'       Date                        09/10/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPRLR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductRelationsLoadRequest
        Inherits XmlRequest
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        Dim productSettings As DEProductSettings

        Private _deProductRelations As New DEProductRelations
        Public Property DeProductRelations() As DEProductRelations
            Get
                Return _deProductRelations
            End Get
            Set(ByVal value As DEProductRelations)
                _deProductRelations = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlLoadResponse As XmlProductRelationsLoadResponse = CType(xmlResp, XmlProductRelationsLoadResponse)
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
            If err.HasError Then
                xmlResp.Err = err
            Else
                Settings.BackOfficeConnectionString = ""
                With PRODUCT
                    .Settings = Settings
                    .DeProductRelations = DeProductRelations
                    err = .ProductRelationsLoad
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlLoadResponse.ResultDataSet = PRODUCT.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlLoadResponse, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim node1, node2, node3, node4 As XmlNode
            Dim total As Integer = 0
            Dim mode As String = String.Empty
            Dim detailsLanguage As String = String.Empty
            DeProductRelations.ProductRelationCollection = New Collection
            Dim deProductRelationCollection As DEProductRelationCollection
            Dim relationsCollectionsCount As Integer = 0
            Dim deProductRelation As DEProductRelation

            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '-------------------------------------------------------------------------------------
            Try
                For Each node1 In xmlDoc.SelectSingleNode("//ProductRelationsLoadRequest").ChildNodes
                    Select Case node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "ProductRelations"
                            DeProductRelations.Total = CInt(node1.Attributes("Total").Value)
                            relationsCollectionsCount += 1
                            For Each node2 In node1.ChildNodes
                                Select Case node2.Name
                                    Case Is = "ProductRelation"
                                        deProductRelationCollection = New DEProductRelationCollection
                                        deProductRelationCollection.ProductRelations = New Collection
                                        deProductRelationCollection.BusinessUnit = node2.Attributes("BusinessUnit").Value
                                        deProductRelationCollection.Partner = node2.Attributes("Partner").Value
                                        deProductRelationCollection.Mode = node2.Attributes("Mode").Value
                                        '-------------------------------
                                        ' Loop through Product Relations
                                        '-------------------------------
                                        For Each node3 In node2.ChildNodes

                                            Select Case node3.Name
                                                Case Is = "Defaults"
                                                    For Each node4 In node3.ChildNodes
                                                        Select Case node4.Name
                                                            Case Is = "LastProductAfterLevel"
                                                                deProductRelationCollection.LastProductAfterLevel = node4.InnerText
                                                        End Select
                                                    Next
                                                Case Is = "Relations"
                                                    deProductRelationCollection.TotalRelations = CInt(node3.Attributes("Total").Value)

                                                    For Each node4 In node3.ChildNodes
                                                        Select Case node4.Name
                                                            Case Is = "Relation"
                                                                '------------------------------------------------
                                                                ' Extract relation details then add to collection
                                                                '------------------------------------------------
                                                                deProductRelation = ExtractProductRelation(node4)
                                                                deProductRelationCollection.ProductRelations.Add(deProductRelation)
                                                        End Select
                                                    Next

                                                    If deProductRelationCollection.TotalRelations <> deProductRelationCollection.ProductRelations.Count Then
                                                        With err
                                                            .ErrorMessage = "Invalid number of product relations supplied: " & _
                                                                            deProductRelationCollection.BusinessUnit & "/" & _
                                                                            deProductRelationCollection.Partner
                                                            .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                            .ErrorNumber = "TTPRQPRLR-07"
                                                            .HasError = True
                                                        End With
                                                    End If

                                            End Select

                                        Next node3
                                        DeProductRelations.ProductRelationCollection.Add(deProductRelationCollection)
                                End Select


                            Next

                            If DeProductRelations.Total <> DeProductRelations.ProductRelationCollection.Count Then
                                With err
                                    .ErrorMessage = "Invalid number of product relation collections supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPRLR-05"
                                    .HasError = True
                                End With
                            End If
                    End Select
                Next node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error" & ex.Message
                    .ErrorNumber = "TTPRQPRLR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        '--------------------------------------------------
        ' Extract a product group from a product group node
        '--------------------------------------------------
        Private Function ExtractProductRelation(ByVal productGroupNode As XmlNode) As DEProductRelation
            '  Const ModuleName As String = "ExtractProductRelation"
            Dim deProductRelation As New DEProductRelation
            Dim node1 As XmlNode
            Dim deProduct As DEProductEcommerceDetails
            If Not productGroupNode.Attributes("Mode") Is Nothing Then
                deProductRelation.Mode = productGroupNode.Attributes("Mode").Value
            End If
            If Not productGroupNode.Attributes("Qualifier") Is Nothing Then
                deProductRelation.Qualifier = productGroupNode.Attributes("Qualifier").Value
            End If

            deProductRelation.ProductInfo = New DEProductGroupHierarchyGroup
            deProductRelation.RelatedProductInfo = New DEProductGroupHierarchyGroup

            For Each node1 In productGroupNode.ChildNodes
                Select Case node1.Name
                    Case Is = "Product"
                        deProduct = New DEProductEcommerceDetails
                        If Not node1.Attributes("Level1") Is Nothing Then
                            deProductRelation.ProductInfo.L01Group = node1.Attributes("Level1").Value
                        End If
                        If Not node1.Attributes("Level2") Is Nothing Then
                            deProductRelation.ProductInfo.L02Group = node1.Attributes("Level2").Value
                        End If
                        If Not node1.Attributes("Level3") Is Nothing Then
                            deProductRelation.ProductInfo.L03Group = node1.Attributes("Level3").Value
                        End If
                        If Not node1.Attributes("Level4") Is Nothing Then
                            deProductRelation.ProductInfo.L04Group = node1.Attributes("Level4").Value
                        End If
                        If Not node1.Attributes("Level5") Is Nothing Then
                            deProductRelation.ProductInfo.L05Group = node1.Attributes("Level5").Value
                        End If
                        If Not node1.Attributes("Level6") Is Nothing Then
                            deProductRelation.ProductInfo.L06Group = node1.Attributes("Level6").Value
                        End If
                        If Not node1.Attributes("Level7") Is Nothing Then
                            deProductRelation.ProductInfo.L07Group = node1.Attributes("Level7").Value
                        End If
                        If Not node1.Attributes("Level8") Is Nothing Then
                            deProductRelation.ProductInfo.L08Group = node1.Attributes("Level8").Value
                        End If
                        If Not node1.Attributes("Level9") Is Nothing Then
                            deProductRelation.ProductInfo.L09Group = node1.Attributes("Level9").Value
                        End If
                        If Not node1.Attributes("Level10") Is Nothing Then
                            deProductRelation.ProductInfo.L10Group = node1.Attributes("Level10").Value
                        End If
                        deProduct.Sku = node1.InnerText
                        deProductRelation.ProductInfo.Products = New Collection
                        deProductRelation.ProductInfo.Products.Add(deProduct)

                    Case Is = "RelatedProduct"
                        deProduct = New DEProductEcommerceDetails
                        If Not node1.Attributes("Level1") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L01Group = node1.Attributes("Level1").Value
                        End If
                        If Not node1.Attributes("Level2") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L02Group = node1.Attributes("Level2").Value
                        End If
                        If Not node1.Attributes("Level3") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L03Group = node1.Attributes("Level3").Value
                        End If
                        If Not node1.Attributes("Level4") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L04Group = node1.Attributes("Level4").Value
                        End If
                        If Not node1.Attributes("Level5") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L05Group = node1.Attributes("Level5").Value
                        End If
                        If Not node1.Attributes("Level6") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L06Group = node1.Attributes("Level6").Value
                        End If
                        If Not node1.Attributes("Level7") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L07Group = node1.Attributes("Level7").Value
                        End If
                        If Not node1.Attributes("Level8") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L08Group = node1.Attributes("Level8").Value
                        End If
                        If Not node1.Attributes("Level9") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L09Group = node1.Attributes("Level9").Value
                        End If
                        If Not node1.Attributes("Level10") Is Nothing Then
                            deProductRelation.RelatedProductInfo.L10Group = node1.Attributes("Level10").Value
                        End If
                        deProduct.Sku = node1.InnerText
                        deProductRelation.RelatedProductInfo.Products = New Collection
                        deProductRelation.RelatedProductInfo.Products.Add(deProduct)
                End Select
            Next node1

            Return deProductRelation
        End Function
    End Class
End Namespace