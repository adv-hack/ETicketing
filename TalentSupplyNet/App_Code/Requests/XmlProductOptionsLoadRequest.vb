Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Requests
'
'       Date                        Oct 2008
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPORQOR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlProductOptionsLoadRequest
        Inherits XmlRequest

        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub


        Private _depo As DEProductOptions
        Public Property Depo() As DEProductOptions
            Get
                Return _depo
            End Get
            Set(ByVal value As DEProductOptions)
                _depo = value
            End Set
        End Property

        Private _deProd As DEProduct
        Public Property MyDEProduct() As DEProduct
            Get
                Return _deProd
            End Get
            Set(ByVal value As DEProduct)
                _deProd = value
            End Set
        End Property



        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlProductOptionsLoadResponse = CType(xmlResp, XmlProductOptionsLoadResponse)
            Dim err As ErrorObj = Nothing

            MyDEProduct = New DEProduct

            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim product As New TalentProduct
            If err.HasError Then
                xmlResp.Err = err
            Else
                With product
                    .Dep = MyDEProduct
                    .DEProductOptions = Depo
                    .Settings = Settings
                    err = .ProductOptionsLoad
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

            Depo = New DEProductOptions

            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode

            Dim depot As DEProductOptionType
            Dim depod As DEProductOptionDefinition

            Dim ActualOptionTypeCount As Integer = 0
            Dim ActualOptionDefinitionCount As Integer = 0

            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductOptionsLoadRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            MyDEProduct.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "OptionTypeDefinitions"
                            Dim optionTypeCount As Integer = CInt(Node1.Attributes("Total").Value)
                            Depo.OptionsTypeLoadMode = Node1.Attributes("Mode").Value
                            If Not Depo.OptionsTypeLoadMode.Equals("REPLACE") And Not Depo.OptionsTypeLoadMode.Equals("UPDATE") Then
                                With err
                                    .ErrorMessage = "Invalid OptionTypeDefinition 'Mode' Attribute. Valid Modes are REPLACE and UPDATE."
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPORQOR-05"
                                    .HasError = True
                                End With
                                Return err
                            End If
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case "DefaultBusinessUnit"
                                        Depo.OptionTypeDefaultBusinessUnit = Node2.InnerText
                                    Case "DefaultPartner"
                                        Depo.OptionTypeDefaultPartner = Node2.InnerText
                                    Case "OptionType"
                                        ActualOptionTypeCount += 1
                                        depot = New DEProductOptionType
                                        depot.Mode = Node2.Attributes("Mode").Value
                                        If Not depot.Mode.Equals("ADD") And Not depot.Mode.Equals("UPDATE") And Not depot.Mode.Equals("DELETE") Then
                                            With err
                                                .ErrorMessage = "Invalid OptionType 'Mode' Attribute. Valid Modes are ADD, DELETE and UPDATE."
                                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                .ErrorNumber = "TTPORQOR-06"
                                                .HasError = True
                                            End With
                                            Return err
                                        End If
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "Code" : depot.OptionType = Node3.InnerText
                                                Case Is = "Details"
                                                    Dim depotl As New DEProductOptionTypeLang
                                                    depotl.LanguageCode = Node3.Attributes("Language").Value
                                                    If Not Node3.Attributes("BusinessUnit") Is Nothing Then
                                                        depotl.BusinessUnit = Node3.Attributes("BusinessUnit").Value
                                                    Else
                                                        depotl.BusinessUnit = Depo.OptionTypeDefaultBusinessUnit
                                                    End If
                                                    If Not Node3.Attributes("Partner") Is Nothing Then
                                                        depotl.Partner = Node3.Attributes("Partner").Value
                                                    Else
                                                        depotl.Partner = Depo.OptionTypeDefaultPartner
                                                    End If
                                                    For Each Node4 In Node3.ChildNodes
                                                        Select Case Node4.Name
                                                            Case "Description"
                                                                depotl.DisplayName = Node4.InnerText
                                                                depotl.LabelText = Node4.InnerText
                                                        End Select
                                                    Next
                                                    depot.LanguageSpecificDEs.Add(depotl)
                                            End Select
                                        Next
                                        Depo.OptionTypesDEs.Add(depot)
                                End Select
                            Next
                            If optionTypeCount <> ActualOptionTypeCount Then
                                With err
                                    .ErrorMessage = "Invalid number of option types supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPORQOR-02"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "OptionDefinitions"
                            Dim defsTotal As Integer = Node1.Attributes("Total").Value
                            Depo.OptionsDefinitionsLoadMode = Node1.Attributes("Mode").Value
                            If Not Depo.OptionsDefinitionsLoadMode.Equals("REPLACE") And Not Depo.OptionsDefinitionsLoadMode.Equals("UPDATE") Then
                                With err
                                    .ErrorMessage = "Invalid OptionDefinition 'Mode' Attribute. Valid Modes are REPLACE and UPDATE."
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPORQOR-07"
                                    .HasError = True
                                End With
                                Return err
                            End If
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case "DefaultBusinessUnit"
                                        Depo.OptionDefinitionDefaultBusinessUnit = Node2.InnerText
                                    Case "DefaultPartner"
                                        Depo.OptionDefinitionDefaultPartner = Node2.InnerText
                                    Case Is = "Option"
                                        ActualOptionDefinitionCount += 1
                                        depod = New DEProductOptionDefinition
                                        depod.Mode = Node2.Attributes("Mode").Value
                                        If Not depod.Mode.Equals("ADD") And Not depod.Mode.Equals("UPDATE") And Not depod.Mode.Equals("DELETE") Then
                                            With err
                                                .ErrorMessage = "Invalid Option 'Mode' Attribute. Valid Modes are ADD, DELETE and UPDATE."
                                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                .ErrorNumber = "TTPORQOR-08"
                                                .HasError = True
                                            End With
                                            Return err
                                        End If
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "Code" : depod.OptionCode = Node3.InnerText
                                                    If depod.OptionCode.Length > 50 Then
                                                        depod.OptionCode = depod.OptionCode.Substring(0, 50)
                                                    End If

                                                Case Is = "Details"
                                                    Dim depodl As New DEProductOptionDefinitionLang
                                                    depodl.LanguageCode = Node3.Attributes("Language").Value
                                                    If Not Node3.Attributes("BusinessUnit") Is Nothing Then
                                                        depodl.BusinessUnit = Node3.Attributes("BusinessUnit").Value
                                                    Else
                                                        depodl.BusinessUnit = Depo.OptionDefinitionDefaultBusinessUnit
                                                    End If
                                                    If Not Node3.Attributes("Partner") Is Nothing Then
                                                        depodl.Partner = Node3.Attributes("Partner").Value
                                                    Else
                                                        depodl.Partner = Depo.OptionDefinitionDefaultPartner
                                                    End If
                                                    For Each Node4 In Node3.ChildNodes
                                                        Select Case Node4.Name
                                                            Case "Description"
                                                                depodl.DisplayName = Node4.InnerText
                                                        End Select
                                                    Next
                                                    depod.LanguageSpecificDEs.Add(depodl)
                                            End Select
                                        Next
                                        Depo.OptionDefinitionDEs.Add(depod)
                                End Select
                            Next
                            If ActualOptionDefinitionCount <> defsTotal Then
                                With err
                                    .ErrorMessage = "Invalid number of option definitions supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPORQOR-03"
                                    .HasError = True
                                End With
                            End If

                        Case Is = "ProductOptions"
                            Dim defsAndOptsTotal As Integer = Node1.Attributes("Total").Value
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name

                                    Case "ProductOption"
                                        Dim defsTotal As Integer = Node2.Attributes("Total").Value
                                        Dim mypodo As New DEProductOptionDefaultsAndOptions
                                        mypodo.BusinessUnit = Node2.Attributes("BusinessUnit").Value
                                        mypodo.Partner = Node2.Attributes("Partner").Value
                                        mypodo.Mode = Node2.Attributes("Mode").Value
                                        If Not mypodo.Mode.Equals("REPLACE") And Not mypodo.Mode.Equals("UPDATE") Then
                                            With err
                                                .ErrorMessage = "Invalid ProductOption 'Mode' Attribute. Valid Modes are REPLACE and UPDATE."
                                                .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                .ErrorNumber = "TTPORQOR-09"
                                                .HasError = True
                                            End With
                                            Return err
                                        End If
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case "Options"
                                                    For Each Node4 In Node3.ChildNodes
                                                        Select Case Node4.Name
                                                            Case "OptionType"
                                                                Dim mypod As New DEProductOptionDefault
                                                                mypod.Action = Node3.Attributes("Action").Value
                                                                If Not mypod.Action.Equals("ADD") And Not mypod.Action.Equals("UPDATE") And Not mypod.Action.Equals("DELETE") Then
                                                                    With err
                                                                        .ErrorMessage = "Invalid Options 'Action' Attribute. Valid Actions are ADD, DELETE and UPDATE."
                                                                        .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                                                        .ErrorNumber = "TTPORQOR-10"
                                                                        .HasError = True
                                                                    End With
                                                                    Return err
                                                                End If
                                                                mypod.MasterProduct = Node3.Attributes("MasterProduct").Value
                                                                mypod.MatchAction = Node3.Attributes("SKUAction").Value

                                                                Select Case mypod.MatchAction
                                                                    Case "APPEND"
                                                                        mypod.AppendSequence = Node4.Attributes("AppendSequence").Value
                                                                    Case Else
                                                                        mypod.AppendSequence = 0
                                                                End Select

                                                                mypod.DisplaySequence = Node4.Attributes("DisplayOrder").Value
                                                                mypod.DisplayType = Node4.Attributes("DisplayType").Value
                                                                mypod.OptionType = Node4.Attributes("OptionType").Value
                                                                If mypod.OptionType.Length > 50 Then
                                                                    mypod.OptionType = mypod.OptionType.Substring(0, 50)
                                                                End If

                                                                For Each Node5 In Node4.ChildNodes
                                                                    Select Case Node5.Name
                                                                        Case "Option"
                                                                            Dim mypo As New DEProductOption
                                                                            mypo.DisplayOrder = Node5.Attributes("DisplayOrder").Value
                                                                            mypo.OptionCode = Node5.InnerText
                                                                            If mypo.OptionCode.Length > 50 Then
                                                                                mypo.OptionCode = mypo.OptionCode.Substring(0, 50)
                                                                            End If

                                                                            Select Case mypod.MatchAction
                                                                                Case "APPEND"
                                                                                    mypo.ProductCode = Node5.Attributes("SKUSuffix").Value
                                                                                Case "SUBSTITUTE"
                                                                                    mypo.ProductCode = Node5.Attributes("SubstituteSKU").Value
                                                                            End Select
                                                                            mypod.ProductOptions.Add(mypo)
                                                                    End Select
                                                                Next
                                                                mypodo.OptionDefaultDEs.Add(mypod)
                                                        End Select
                                                    Next

                                            End Select
                                        Next
                                        Depo.OptionDefaultsAndOptionsDEs.Add(mypodo)
                                End Select
                            Next
                            If Depo.OptionDefaultsAndOptionsDEs.Count <> defsAndOptsTotal Then
                                With err
                                    .ErrorMessage = "Invalid number of option defaults supplied"
                                    .ErrorStatus = ModuleName & " Error: " & .ErrorMessage
                                    .ErrorNumber = "TTPORQOR-04"
                                    .HasError = True
                                End With
                            End If
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace


