Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Requests
'
'       Date                        Nov 2006
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQOR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       30/08/07    /001    Ben     Add 'LineComment' to product lines
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductLoadRequest
        Inherits XmlRequest

        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub

        Private _dep As DEProduct

        Public Overloads Property Dep() As DEProduct
            Get
                Return _dep
            End Get
            Set(ByVal value As DEProduct)
                _dep = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlProductLoadResponse = CType(xmlResp, XmlProductLoadResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV1_1()
            End Select


            Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
            Dim updateDescriptions As Boolean = False

            Try
                updateDescriptions = CBool(def.GetDefault("UPDATE_DESCRIPTIONS"))
            Catch
            End Try

            Dep.UpdateDescriptions = updateDescriptions
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim product As New TalentProduct
            If err.HasError Then
                xmlResp.Err = err
            Else
                With product
                    .Dep = Dep()
                    .Settings = Settings
                    err = .ProductLoad
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = product.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj

            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            Dim decd As DECategoryDetails
            Dim dead As DEAttributeDetails
            Dim depd As DEProductDescriptions
            Dim depr As DEProductEcommerceDetails
            Dim depa As DEProductEcommerceAttribute
            Dep = New DEProduct

            Dim CategoryDefinitionCount As Integer = 0
            Dim AttributeDefinitionCount As Integer = 0
            Dim ProductDefinitionCount As Integer = 0
            Dim ProductAttributeCount As Integer = 0

            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductLoadRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "CategoryDefinitions"
                            Dim categoryTotal As Integer = Node1.Attributes("Total").Value
                            Dep.CategoryMode = Node1.Attributes("Mode").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "CategoryDefinition"
                                        CategoryDefinitionCount += 1
                                        decd = New DECategoryDetails
                                        decd.Mode = Node2.Attributes("Mode").Value
                                        With decd
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .Code = Node3.InnerText
                                                    Case Is = "DisplaySequence" : .DisplaySequence = Node3.InnerText
                                                    Case Is = "Description"
                                                        depd = New DEProductDescriptions
                                                        depd.Language = Node3.Attributes("Language").Value
                                                        depd.Description1 = Node3.InnerText
                                                        decd.CollDEProductDescriptions.Add(depd)
                                                End Select
                                            Next
                                        End With
                                        Dep.CollDECategoryDefinitions.Add(decd)
                                End Select
                            Next
                            If categoryTotal <> CategoryDefinitionCount Then
                                With err
                                    .ErrorMessage = "Invalid number of categories supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPR-02-A"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "AttributeDefinitions"
                            Dim AttributeTotal As Integer = Node1.Attributes("Total").Value
                            Dep.AttributeMode = Node1.Attributes("Mode").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "AttributeDefinition"
                                        AttributeDefinitionCount += 1
                                        dead = New DEAttributeDetails
                                        dead.Mode = Node2.Attributes("Mode").Value
                                        With dead
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .Code = Node3.InnerText
                                                    Case Is = "DisplaySequence" : .DisplaySequence = Node3.InnerText
                                                    Case Is = "Description"
                                                        depd = New DEProductDescriptions
                                                        depd.Language = Node3.Attributes("Language").Value
                                                        depd.Description1 = Node3.InnerText
                                                        dead.CollDEProductDescriptions.Add(depd)
                                                    Case Is = "Value"
                                                        If Not Node3.InnerText.Equals(String.Empty) Then
                                                            .AttributeValue = CType(Node3.InnerText, Decimal)
                                                        End If
                                                    Case Is = "Date"
                                                        If Not Node3.InnerText.Equals(String.Empty) Then
                                                            .AttributeDate = CType(Node3.InnerText, Date)
                                                        Else
                                                            .AttributeDate = "01/01/1900"
                                                        End If
                                                    Case Is = "Boolean"
                                                        Select Case Node3.InnerText
                                                            Case Is = "Y"
                                                                .AttributeBoolean = True
                                                            Case Else
                                                                .AttributeBoolean = False
                                                        End Select
                                                End Select
                                            Next
                                        End With
                                        Dep.CollDEAttributeDefinitions.Add(dead)
                                End Select
                            Next
                            If AttributeTotal <> AttributeDefinitionCount Then
                                With err
                                    .ErrorMessage = "Invalid number of attributes supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPR-03-A"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "ProductDefinitions"
                            Dim ProductTotal As Integer = Node1.Attributes("Total").Value
                            Dep.ProductMode = Node1.Attributes("Mode").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "ProductDefinition"
                                        ProductDefinitionCount += 1
                                        depr = New DEProductEcommerceDetails
                                        depr.Mode = Node2.Attributes("Mode").Value
                                        With depr
                                            For Each Node3 In Node2.ChildNodes
                                                ProductAttributeCount = 0
                                                Select Case Node3.Name
                                                    Case Is = "SKU" : .Sku = Node3.InnerText
                                                    Case Is = "AlternativeSKU" : .AlternateSku = Node3.InnerText
                                                    Case Is = "MasterProduct"
                                                        Select Case Node3.InnerText.ToLower
                                                            Case Is = "y", "true", "1"
                                                                .MasterProduct = True
                                                            Case Else
                                                                .MasterProduct = False
                                                        End Select
                                                    Case Is = "Details"
                                                        depd = New DEProductDescriptions
                                                        depd.Weight = 0
                                                        depd.Language = Node3.Attributes("Language").Value
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Description1" : depd.Description1 = Node4.InnerText
                                                                Case Is = "Description2" : depd.Description2 = Node4.InnerText
                                                                Case Is = "Description3" : depd.Description3 = Node4.InnerText
                                                                Case Is = "Description4" : depd.Description4 = Node4.InnerText
                                                                Case Is = "Description5" : depd.Description5 = Node4.InnerText
                                                                Case Is = "HTML1" : depd.Html1 = Node4.InnerText
                                                                Case Is = "HTML2" : depd.Html2 = Node4.InnerText
                                                                Case Is = "HTML3" : depd.Html3 = Node4.InnerText
                                                                Case Is = "HTML4" : depd.Html4 = Node4.InnerText
                                                                Case Is = "HTML5" : depd.Html5 = Node4.InnerText
                                                                Case Is = "HTML6" : depd.Html6 = Node4.InnerText
                                                                Case Is = "HTML7" : depd.Html7 = Node4.InnerText
                                                                Case Is = "HTML8" : depd.Html8 = Node4.InnerText
                                                                Case Is = "HTML9" : depd.Html9 = Node4.InnerText
                                                                Case Is = "SearchKeywords" : depd.SearchKeywords = Node4.InnerText
                                                                Case Is = "PageTitle" : depd.PageTitle = Node4.InnerText
                                                                Case Is = "MetaDescription" : depd.MetaDescription = Node4.InnerText
                                                                Case Is = "MetaKeywords" : depd.MetaKeywords = Node4.InnerText
                                                                Case Is = "AvailableOnline" : depr.AvailableOnline = ConvertFromISeriesYesNoToBoolean(Node4.InnerText)
                                                                Case Is = "Weight"
                                                                    If Node4.InnerText <> String.Empty Then
                                                                        Try
                                                                            depd.Weight = Node4.InnerText()
                                                                        Catch ex As Exception

                                                                        End Try

                                                                    End If

                                                            End Select
                                                        Next
                                                        depr.CollDEProductDescriptions.Add(depd)
                                                    Case Is = "Attributes"
                                                        Dim productAttributesTotal As Integer = Node3.Attributes("Total").Value
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Attribute"
                                                                    ProductAttributeCount += 1
                                                                    depa = New DEProductEcommerceAttribute

                                                                    depa.Category = Node4.Attributes("Category").Value
                                                                    depa.SubCategory = Node4.Attributes("SubCategory").Value

                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "Code" : depa.Attribute = Node5.InnerText
                                                                            Case Is = "DisplaySequence" : depa.DisplaySequence = Node5.InnerText
                                                                        End Select
                                                                    Next
                                                                    depr.CollDEProductAttributes.Add(depa, depa.Category)
                                                            End Select
                                                        Next
                                                        If productAttributesTotal <> ProductAttributeCount Then
                                                            With err
                                                                .ErrorMessage = "Invalid number of product attributes supplied"
                                                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                                .ErrorNumber = "TTPRQPR-05-A"
                                                                .HasError = True
                                                            End With
                                                        End If
                                                End Select
                                            Next
                                        End With
                                        Dep.CollDEProducts.Add(depr)
                                End Select
                            Next
                            If ProductTotal <> ProductDefinitionCount Then
                                With err
                                    .ErrorMessage = "Invalid number of products supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPR-04-A"
                                    .HasError = True
                                End With
                            End If
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPR-01-A"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj

            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            Dim decd As DECategoryDetails
            Dim dead As DEAttributeDetails
            Dim depd As DEProductDescriptions
            Dim depr As DEProductEcommerceDetails
            Dim depa As DEProductEcommerceAttribute
            Dep = New DEProduct

            Dim CategoryDefinitionCount As Integer = 0
            Dim AttributeDefinitionCount As Integer = 0
            Dim ProductDefinitionCount As Integer = 0
            Dim ProductAttributeCount As Integer = 0

            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductLoadRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "CategoryDefinitions"
                            Dim categoryTotal As Integer = Node1.Attributes("Total").Value
                            Dep.CategoryMode = Node1.Attributes("Mode").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "CategoryDefinition"
                                        CategoryDefinitionCount += 1
                                        decd = New DECategoryDetails
                                        decd.Mode = Node2.Attributes("Mode").Value
                                        With decd
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .Code = Node3.InnerText
                                                    Case Is = "DisplaySequence" : .DisplaySequence = Node3.InnerText
                                                    Case Is = "Description"
                                                        depd = New DEProductDescriptions
                                                        depd.Language = Node3.Attributes("Language").Value
                                                        depd.Description1 = Node3.InnerText
                                                        decd.CollDEProductDescriptions.Add(depd)
                                                End Select
                                            Next
                                        End With
                                        Dep.CollDECategoryDefinitions.Add(decd)
                                End Select
                            Next
                            If categoryTotal <> CategoryDefinitionCount Then
                                With err
                                    .ErrorMessage = "Invalid number of categories supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPR-02-B"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "AttributeDefinitions"
                            Dim AttributeTotal As Integer = Node1.Attributes("Total").Value
                            Dep.AttributeMode = Node1.Attributes("Mode").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "AttributeDefinition"
                                        AttributeDefinitionCount += 1
                                        dead = New DEAttributeDetails
                                        dead.Mode = Node2.Attributes("Mode").Value
                                        With dead
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "Code" : .Code = Node3.InnerText
                                                    Case Is = "DisplaySequence" : .DisplaySequence = Node3.InnerText
                                                    Case Is = "Description"
                                                        depd = New DEProductDescriptions
                                                        depd.Language = Node3.Attributes("Language").Value
                                                        depd.Description1 = Node3.InnerText
                                                        dead.CollDEProductDescriptions.Add(depd)
                                                    Case Is = "Value"
                                                        If Not Node3.InnerText.Equals(String.Empty) Then
                                                            .AttributeValue = CType(Node3.InnerText, Decimal)
                                                        End If
                                                    Case Is = "Date"
                                                        If Not Node3.InnerText.Equals(String.Empty) Then
                                                            .AttributeDate = CType(Node3.InnerText, Date)
                                                        Else
                                                            .AttributeDate = "01/01/1900"
                                                        End If
                                                    Case Is = "Boolean"
                                                        Select Case Node3.InnerText
                                                            Case Is = "Y"
                                                                .AttributeBoolean = True
                                                            Case Else
                                                                .AttributeBoolean = False
                                                        End Select
                                                End Select
                                            Next
                                        End With
                                        Dep.CollDEAttributeDefinitions.Add(dead)
                                End Select
                            Next
                            If AttributeTotal <> AttributeDefinitionCount Then
                                With err
                                    .ErrorMessage = "Invalid number of attributes supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPR-03-B"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "ProductDefinitions"
                            Dim ProductTotal As Integer = Node1.Attributes("Total").Value
                            Dep.ProductMode = Node1.Attributes("Mode").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "ProductDefinition"
                                        ProductDefinitionCount += 1
                                        depr = New DEProductEcommerceDetails
                                        depr.Mode = Node2.Attributes("Mode").Value
                                        With depr
                                            For Each Node3 In Node2.ChildNodes
                                                ProductAttributeCount = 0
                                                Select Case Node3.Name
                                                    Case Is = "SKU" : .Sku = Node3.InnerText
                                                    Case Is = "AlternativeSKU" : .AlternateSku = Node3.InnerText
                                                    Case Is = "MasterProduct"
                                                        Select Case Node3.InnerText.ToLower
                                                            Case Is = "y", "true", "1"
                                                                .MasterProduct = True
                                                            Case Else
                                                                .MasterProduct = False
                                                        End Select
                                                    Case Is = "Details"
                                                        depd = New DEProductDescriptions
                                                        depd.Weight = 0
                                                        depd.Language = Node3.Attributes("Language").Value
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Description1" : depd.Description1 = Node4.InnerText
                                                                Case Is = "Description2" : depd.Description2 = Node4.InnerText
                                                                Case Is = "Description3" : depd.Description3 = Node4.InnerText
                                                                Case Is = "Description4" : depd.Description4 = Node4.InnerText
                                                                Case Is = "Description5" : depd.Description5 = Node4.InnerText
                                                                Case Is = "HTML1" : depd.Html1 = Node4.InnerText
                                                                Case Is = "HTML2" : depd.Html2 = Node4.InnerText
                                                                Case Is = "HTML3" : depd.Html3 = Node4.InnerText
                                                                Case Is = "SearchKeywords" : depd.SearchKeywords = Node4.InnerText
                                                                Case Is = "PageTitle" : depd.PageTitle = Node4.InnerText
                                                                Case Is = "MetaDescription" : depd.MetaDescription = Node4.InnerText
                                                                Case Is = "MetaKeywords" : depd.MetaKeywords = Node4.InnerText
                                                                Case Is = "AvailableOnline" : depr.AvailableOnline = Node4.InnerText
                                                                Case Is = "Weight"
                                                                    If Node4.InnerText <> String.Empty Then
                                                                        Try
                                                                            depd.Weight = Node4.InnerText()
                                                                        Catch ex As Exception

                                                                        End Try

                                                                    End If
                                                                Case Is = "ProductGLCode1" : depd.GLCode1 = Node4.InnerText
                                                                Case Is = "ProductGLCode2" : depd.GLCode2 = Node4.InnerText
                                                                Case Is = "ProductGLCode3" : depd.GLCode3 = Node4.InnerText
                                                                Case Is = "ProductGLCode4" : depd.GLCode4 = Node4.InnerText
                                                                Case Is = "ProductGLCode5" : depd.GLCode5 = Node4.InnerText
                                                            End Select
                                                        Next
                                                        depr.CollDEProductDescriptions.Add(depd)
                                                    Case Is = "Attributes"
                                                        Dim productAttributesTotal As Integer = Node3.Attributes("Total").Value
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "Attribute"
                                                                    ProductAttributeCount += 1
                                                                    depa = New DEProductEcommerceAttribute

                                                                    depa.Category = Node4.Attributes("Category").Value
                                                                    depa.SubCategory = Node4.Attributes("SubCategory").Value

                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "Code" : depa.Attribute = Node5.InnerText
                                                                            Case Is = "DisplaySequence" : depa.DisplaySequence = Node5.InnerText
                                                                        End Select
                                                                    Next
                                                                    depr.CollDEProductAttributes.Add(depa, depa.Category)
                                                            End Select
                                                        Next
                                                        If productAttributesTotal <> ProductAttributeCount Then
                                                            With err
                                                                .ErrorMessage = "Invalid number of product attributes supplied"
                                                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                                .ErrorNumber = "TTPRQPR-05-B"
                                                                .HasError = True
                                                            End With
                                                        End If
                                                End Select
                                            Next
                                        End With
                                        Dep.CollDEProducts.Add(depr)
                                End Select
                            Next
                            If ProductTotal <> ProductDefinitionCount Then
                                With err
                                    .ErrorMessage = "Invalid number of products supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPRQPR-04-B"
                                    .HasError = True
                                End With
                            End If
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPR-01-B"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
    End Class

End Namespace
