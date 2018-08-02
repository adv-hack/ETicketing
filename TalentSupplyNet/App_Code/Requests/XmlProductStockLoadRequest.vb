Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to load Product Groups and their hierarchy
'
'       Date                        09/10/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPSLR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductStockLoadRequest
        Inherits XmlRequest
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub

        Dim deStock As New DEStock

        Dim deWarehouses As DEWarehouses
        Dim deProducts As DEStockProducts
        Dim deDefaults As DEStockDefaults

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlLoadResponse As XmlProductStockLoadResponse = CType(xmlResp, XmlProductStockLoadResponse)
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
                With PRODUCT
                    Settings.BackOfficeConnectionString = ""
                    .Settings = Settings
                    .Stock = deStock
                    err = .ProductStockLoad()
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            XmlLoadResponse.ResultDataSet = PRODUCT.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlLoadResponse, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim node1 As XmlNode
            Dim total As Integer = 0
            Dim mode As String = String.Empty                    
            Dim hierarchiesTotal As Integer = 0
            Dim hierarchiesCount As Integer = 0

            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '-------------------------------------------------------------------------------------
            Try
                For Each node1 In xmlDoc.SelectSingleNode("//ProductStockLoadRequest").ChildNodes
                    Select Case node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "warehouses"
                            deWarehouses = ExtractWarehouses(node1)
                            deStock.deWarehouses.add(deWarehouses)
                        Case Is = "Products"
                            deProducts = ExtractProducts(node1)
                            deStock.Products.Add(deProducts)
                        Case Is = "Defaults"
                            deDefaults = ExtractDefaults(node1)
                            deStock.Defaults.Add(deDefaults)
                        
                            If hierarchiesTotal <> hierarchiesCount Then
                                With err
                                    .ErrorMessage = "Invalid number of hierarchies supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPSLR-04"
                                    .HasError = True
                                End With
                            End If
                    End Select
                Next node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error" & ex.Message
                    .ErrorNumber = "TTPRQPSLR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function ExtractWarehouses(ByVal warehousesNode As XmlNode) As DEWarehouses
            Dim deWarehouses As DEWarehouses = New DEWarehouses()
            If Not warehousesNode.Attributes("Total") Is Nothing Then
                deWarehouses.Total = Integer.Parse(warehousesNode.Attributes("Total").Value)
            End If
            If Not warehousesNode.Attributes("Mode") Is Nothing Then
                deWarehouses.Mode = warehousesNode.Attributes("Mode").Value
            End If

            For Each warehouseNode As XmlNode In warehousesNode.ChildNodes
                Select Case warehouseNode.Name
                    Case Is = "Warehouse"
                        Dim deWarehouse As DEWarehouse = New DEWarehouse()
                        If Not warehouseNode.Attributes("Mode") Is Nothing Then
                            deWarehouse.Mode = warehouseNode.Attributes("Mode").Value
                        End If
                        For Each child As XmlNode In warehouseNode.ChildNodes
                            Select Case child.Name
                                Case Is = "Code"
                                    deWarehouse.Code = child.InnerText
                                Case Is = "Description"
                                    deWarehouse.Description = child.InnerText
                                    If Not child.Attributes("Language") Is Nothing Then
                                        deWarehouse.DescriptionLanguage = child.Attributes("Language").Value
                                    End If
                                Case Else
                                    Throw New Exception("Unknown child node of warehouse node")
                            End Select
                        Next
                        deWarehouses.Warehouses.Add(deWarehouse)
                    Case Else
                        Throw New Exception("Unknown child node of warehouses node")
                End Select
            Next
            Return deWarehouses
        End Function

        Private Function ExtractProducts(ByVal productsNode As XmlNode) As DEStockProducts
            Dim products As DEStockProducts = New DEStockProducts()
            If Not productsNode.Attributes("Total") Is Nothing Then
                products.Total = Integer.Parse(productsNode.Attributes("Total").Value)
            End If
            If Not productsNode.Attributes("Mode") Is Nothing Then
                products.Mode = productsNode.Attributes("Mode").Value
            End If

            For Each productNode As XmlNode In productsNode.ChildNodes
                Select Case productNode.Name
                    Case Is = "Product"
                        Dim product As DEStockProduct = New DEStockProduct
                        If Not productNode.Attributes("Mode") Is Nothing Then
                            product.Mode = productNode.Attributes("Mode").Value
                        End If
                        For Each child As XmlNode In productNode.ChildNodes
                            Select Case child.Name
                                Case Is = "SKU"
                                    product.SKU = child.InnerText
                                Case Is = "Quantity"
                                    product.Quantity = Integer.Parse(child.InnerText)
                                    If Not child.Attributes("Warehouse") Is Nothing Then
                                        product.QuantityWarehouse = child.Attributes("Warehouse").Value
                                    End If
                                    If Not child.Attributes("ReStockCode") Is Nothing Then
                                        product.QuantityReStockCode = child.Attributes("ReStockCode").Value
                                    End If
                                Case Else
                                    Throw New Exception("Unknown child node of products node")
                            End Select                            
                        Next
                        products.StockProducts.Add(product)
                End Select
            Next
            Return products
        End Function

        Private Function ExtractDefaults(ByVal defaultsNode As XmlNode) As DEStockDefaults
            Dim defaults As DEStockDefaults = New DEStockDefaults()

            If Not defaultsNode.Attributes("Total") Is Nothing Then
                defaults.Total = Integer.Parse(defaultsNode.Attributes("Total").Value)
            End If
            If Not defaultsNode.Attributes("Mode") Is Nothing Then
                defaults.Mode = defaultsNode.Attributes("Mode").Value
            End If
            Dim stockDefault As DEStockDefault = New DEStockDefault
            For Each defaultNode As XmlNode In defaultsNode.ChildNodes                
                Select Case defaultNode.Name
                    Case Is = "Warehouse"
                        stockDefault.Warehouse = defaultNode.InnerText
                        If Not defaultNode.Attributes("Mode") Is Nothing Then
                            stockDefault.Mode = defaultNode.Attributes("Mode").Value
                        End If
                        If Not defaultNode.Attributes("BusinessUnit") Is Nothing Then
                            stockDefault.BusinessUnit = defaultNode.Attributes("BusinessUnit").Value
                        End If
                        If Not defaultNode.Attributes("Partner") Is Nothing Then
                            stockDefault.Partner = defaultNode.Attributes("Partner").Value
                        End If
                    Case Else
                End Select
            Next
            defaults.Defaults.Add(stockDefault)
            Return defaults
        End Function

        '--------------------------------------------------
        ' Extract a product group from a product group node
        '--------------------------------------------------
        Private Function ExtractProductGroup(ByVal productGroupNode As XmlNode) As DEProductGroup
            '  Const ModuleName As String = "ExtractProductGroup"
            Dim deProductGroup As New DEProductGroup
            Dim node1, node2 As XmlNode
            Dim deProductGroupDetails As DEProductGroupDetails
            If Not productGroupNode.Attributes("Mode") Is Nothing Then
                deProductGroup.Mode = productGroupNode.Attributes("Mode").Value
            End If
            deProductGroup.Details = New Collection

            For Each node1 In productGroupNode.ChildNodes
                Select Case node1.Name
                    Case Is = "Code"
                        deProductGroup.Code = node1.InnerText
                    Case Is = "Details"
                        '----------------------------------------
                        ' Loop through language specific settings 
                        ' and add to details collection
                        '----------------------------------------
                        deProductGroupDetails = New DEProductGroupDetails
                        deProductGroupDetails.Language = node1.Attributes("Language").Value

                        For Each node2 In node1.ChildNodes

                            Select Case node2.Name
                                Case Is = "Description1"
                                    deProductGroupDetails.Description1 = node2.InnerText
                                Case Is = "Description2"
                                    deProductGroupDetails.Description2 = node2.InnerText
                                Case Is = "HTML1"
                                    deProductGroupDetails.Html1 = node2.InnerText
                                Case Is = "HTML2"
                                    deProductGroupDetails.Html2 = node2.InnerText
                                Case Is = "HTML3"
                                    deProductGroupDetails.Html3 = node2.InnerText
                                Case Is = "PageTitle"
                                    deProductGroupDetails.PageTitle = node2.InnerText
                                Case Is = "MetaDescription"
                                    deProductGroupDetails.MetaDescription = node2.InnerText
                                Case Is = "MetaKeywords"
                                    deProductGroupDetails.MetaKeywords = node2.InnerText
                            End Select

                        Next node2
                        deProductGroup.Details.Add(deProductGroupDetails)
                End Select
            Next node1

            Return deProductGroup
        End Function
        '----------------------------------------------------------------------
        ' Extract a product group hierarchy from a product group hierarchy node
        '----------------------------------------------------------------------
        Private Function ExtractProductGroupHierarchy(ByVal productGroupHierarchyNode As XmlNode) As DEProductGroupHierarchy
            Const ModuleName As String = "ExtractProductGroupHierarchy"
            Dim deProductGroupHierarchy As New DEProductGroupHierarchy
            deProductGroupHierarchy.Err = New ErrorObj
            Dim node1, node2 As XmlNode
            Dim deProductGroupHierarchyGroup1 As DEProductGroupHierarchyGroup
            Dim level1groupsCount As Integer = 0

            deProductGroupHierarchy.BusinessUnit = productGroupHierarchyNode.Attributes("BusinessUnit").Value
            deProductGroupHierarchy.Partner = productGroupHierarchyNode.Attributes("Partner").Value
            deProductGroupHierarchy.Mode = productGroupHierarchyNode.Attributes("Mode").Value
            deProductGroupHierarchy.Level1Groups = New Collection
            For Each node1 In productGroupHierarchyNode.ChildNodes
                Select Case node1.Name
                    Case Is = "Defaults"
                        For Each node2 In node1.ChildNodes
                            Select Case node2.Name
                                Case Is = "LastProductAfterLevel"
                                    deProductGroupHierarchy.LastProductAfterLevel = node2.InnerText
                            End Select
                        Next
                    Case Is = "Level1"
                        deProductGroupHierarchy.Level1GroupsTotal = CInt(node1.Attributes("Total").Value)
                        For Each node2 In node1.ChildNodes
                            Select Case node2.Name
                                Case Is = "ProductGroup"
                                    level1groupsCount += 1
                                    deProductGroupHierarchyGroup1 = ExtractProductGroupHierarchyGroup(node2, deProductGroupHierarchy.BusinessUnit, deProductGroupHierarchy.Partner)
                                    If deProductGroupHierarchyGroup1.Err.HasError Then
                                        deProductGroupHierarchy.Err = deProductGroupHierarchyGroup1.Err
                                    End If

                                    deProductGroupHierarchy.Level1Groups.Add(deProductGroupHierarchyGroup1)
                            End Select
                        Next
                End Select

            Next node1
            If deProductGroupHierarchy.Level1GroupsTotal <> deProductGroupHierarchy.Level1Groups.Count Then
                With deProductGroupHierarchy.Err
                    .ErrorMessage = "Invalid number of Level 1 groups supplied"
                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                    .ErrorNumber = "TTPRQPSLR-06"
                    .HasError = True
                End With
            End If

            Return deProductGroupHierarchy

        End Function
        '----------------------------------------------------------------------------------
        ' Extract a product group hierarchy group from a product group hierarchy group node
        '----------------------------------------------------------------------------------
        Private Function ExtractProductGroupHierarchyGroup(ByVal productGroupHierarchyGroupNode As XmlNode, ByVal businessUnit As String, ByVal partner As String) As DEProductGroupHierarchyGroup
            Const ModuleName As String = "ExtractProductGroupHierarchyGroup"
            Dim deProductGroupHierarchyGroup As New DEProductGroupHierarchyGroup
            deProductGroupHierarchyGroup.Err = New ErrorObj
            Dim deProductGroupHierarchyGroup2 As DEProductGroupHierarchyGroup
            Dim product As New DEProductEcommerceDetails
            deProductGroupHierarchyGroup.NextLevelGroups = New Collection
            Dim node2, node3, node4 As XmlNode

            deProductGroupHierarchyGroup.BusinessUnit = businessUnit
            deProductGroupHierarchyGroup.Partner = partner
            deProductGroupHierarchyGroup.Products = New Collection

            If Not productGroupHierarchyGroupNode.Attributes("Mode") Is Nothing Then
                deProductGroupHierarchyGroup.Mode = productGroupHierarchyGroupNode.Attributes("Mode").Value
            End If
            deProductGroupHierarchyGroup.NextLevelGroups = New Collection

            For Each node2 In productGroupHierarchyGroupNode.ChildNodes
                Select Case node2.Name
                    Case Is = "Code"
                        deProductGroupHierarchyGroup.Code = node2.InnerText
                    Case Is = "DisplaySequence"
                        deProductGroupHierarchyGroup.DisplaySequence = node2.InnerText
                    Case Is = "AdvancedSearchTemplate"
                        deProductGroupHierarchyGroup.AdvancedSearchTemplate = node2.InnerText
                    Case Is = "ProductPageTemplate"
                        deProductGroupHierarchyGroup.ProductPageTemplate = node2.InnerText
                    Case Is = "ProductListTemplate"
                        deProductGroupHierarchyGroup.ProductListTemplate = node2.InnerText
                    Case Is = "ShowChildrenAsGroups"
                        deProductGroupHierarchyGroup.ShowChildrenAsGroups = node2.InnerText
                    Case Is = "ShowProductsAsList"
                        deProductGroupHierarchyGroup.ShowProductAsList = node2.InnerText
                    Case Is = "ShowInNavigation"
                        deProductGroupHierarchyGroup.ShowInNavigation = node2.InnerText
                    Case Is = "ShowInGroupedNavigation"
                        deProductGroupHierarchyGroup.ShowInGroupedNavigation = node2.InnerText
                    Case Is = "HTMLGroup"
                        deProductGroupHierarchyGroup.HtmlGroup = node2.InnerText
                    Case Is = "HTMLGroupType"
                        deProductGroupHierarchyGroup.HtmlGroupType = node2.InnerText
                    Case Is = "ShowProductDisplay"
                        deProductGroupHierarchyGroup.ShowProductDisplay = node2.InnerText
                    Case Is = "Theme"
                        deProductGroupHierarchyGroup.Theme = node2.InnerText
                    Case Is = "Products"
                        deProductGroupHierarchyGroup.ProductsTotal = CInt(node2.Attributes("Total").Value)

                        For Each node3 In node2.ChildNodes
                            Select Case node3.Name
                                Case Is = "Product"
                                    product = New DEProductEcommerceDetails
                                    If Not node3.Attributes("Mode") Is Nothing Then
                                        product.Mode = node3.Attributes("Mode").Value
                                    End If

                                    For Each node4 In node3.ChildNodes
                                        Select Case node4.Name
                                            Case Is = "SKU"
                                                product.Sku = node4.InnerText
                                            Case Is = "DisplaySequence"
                                                product.DisplaySequence = node4.InnerText
                                        End Select
                                    Next
                                    deProductGroupHierarchyGroup.Products.Add(product)
                            End Select
                        Next
                        If deProductGroupHierarchyGroup.ProductsTotal <> deProductGroupHierarchyGroup.Products.Count Then
                            With deProductGroupHierarchyGroup.Err
                                .ErrorMessage = "Invalid number of products groups supplied: " & deProductGroupHierarchyGroup.Code
                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                .ErrorNumber = "TTPRQPSLR-08"
                                .HasError = True
                            End With
                        End If
                    Case Is = "Level2", "Level3", "Level4", "Level5", "Level6", "Level7", "Level8", "Level9", "Level10"

                        deProductGroupHierarchyGroup.NextLevelGroupsTotal = CInt(node2.Attributes("Total").Value)
                        For Each node3 In node2.ChildNodes
                            Select Case node3.Name
                                Case Is = "ProductGroup"
                                    '------------------------------------------------------------------------
                                    ' Recursive call to extract the next group. Add to this groups collection
                                    ' of child groups
                                    '---------------------------------------------------------
                                    deProductGroupHierarchyGroup2 = New DEProductGroupHierarchyGroup

                                    deProductGroupHierarchyGroup2 = ExtractProductGroupHierarchyGroup(node3, businessUnit, partner)
                                    If deProductGroupHierarchyGroup2.Err.HasError Then
                                        deProductGroupHierarchyGroup.Err = deProductGroupHierarchyGroup2.Err
                                    End If

                                    deProductGroupHierarchyGroup2.BusinessUnit = businessUnit
                                    deProductGroupHierarchyGroup2.Partner = partner
                                    deProductGroupHierarchyGroup.NextLevelGroups.Add(deProductGroupHierarchyGroup2)
                            End Select
                        Next
                        If deProductGroupHierarchyGroup.NextLevelGroupsTotal <> deProductGroupHierarchyGroup.NextLevelGroups.Count Then
                            With deProductGroupHierarchyGroup.Err
                                .ErrorMessage = "Invalid number of Level groups supplied: " & deProductGroupHierarchyGroup.Code
                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                .ErrorNumber = "TTPRQPSLR-07"
                                .HasError = True
                            End With
                        End If
                End Select
            Next node2

            Return deProductGroupHierarchyGroup
        End Function

    End Class

End Namespace