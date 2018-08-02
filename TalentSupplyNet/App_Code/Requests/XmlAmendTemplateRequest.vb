Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Template Requests
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQAT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAmendTemplateRequest
        Inherits XmlRequest

        Private _depa As New DEAmendTemplate
        Public Property Depa() As DEAmendTemplate
            Get
                Return _depa
            End Get
            Set(ByVal value As DEAmendTemplate)
                _depa = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlAmendTemplateResponse = CType(xmlResp, XmlAmendTemplateResponse)
            Dim pa As New TalentTemplate
            Dim err As ErrorObj = Nothing
            '------------------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '------------------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With pa
                    .Dep = Depa
                    .Settings = Settings
                    If Depa.AddNewTemplate Then
                        err = .AddNewTemplate
                    ElseIf Depa.AddToTemplate Then
                        err = .AddToTemplate
                    End If
                    Depa = .Dep
                End With
            End If
            '------------------------------------------------------------------------------
            With xmlAction
                .Err = err
                .SenderID = Settings.SenderID
                .Dep = Depa
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1, Node2 As XmlNode
            Dim dea As New DEAlerts                 ' Items
            Depa = New DEAmendTemplate
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AmendTemplateRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Depa.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "ProcessingOptions"
                            '---------------------------------------------------------------
                            '<ProcessingOptions>
                            '  <ReplaceTemplate></ReplaceTemplate>
                            '  <AddToTemplate></AddToTemplate>
                            '  <DeleteTemplate></DeleteTemplate>
                            '  <DeleteFromTemplate></DeleteFromTemplate>
                            '</ProcessingOptions>
                            '---------------------------------------------------------------
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "AddNewTemplate"
                                        Depa.AddNewTemplate = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "ReplaceTemplate"
                                        Depa.ReplaceTemplate = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "AddToTemplate"
                                        Depa.AddToTemplate = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "DeleteTemplate"
                                        Depa.DeleteTemplate = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "DeleteFromTemplate"
                                        Depa.DeleteFromTemplate = CType(Node2.InnerText & String.Empty, Boolean)
                                End Select
                            Next
                        Case Is = "TemplateHeader"
                            '---------------------------------------------------------------
                            '<TemplateHeader>
                            '  <BusinessUnit></BusinessUnit>
                            '  <Partner></Partner>
                            '  <UserID></UserID>
                            '  <Name></Name>
                            '  <Description></Description>
                            '  <IsDefault></IsDefault>
                            '</TemplateHeader>
                            '---------------------------------------------------------------
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "BusinessUnit"
                                        Depa.BusinessUnit = Node2.InnerText
                                    Case Is = "Partner"
                                        Depa.PartnerCode = Node2.InnerText
                                    Case Is = "UserID"
                                        Depa.UserID = Node2.InnerText
                                    Case Is = "Name"
                                        Depa.Name = Node2.InnerText
                                    Case Is = "Description"
                                        Depa.Description = Node2.InnerText
                                    Case Is = "IsDefault"
                                        Depa.IsDefault = CBool(Node2.InnerText)
                                End Select
                            Next
                        Case Is = "TemplateLine"
                            '---------------------------------------------------------------
                            '<TemplateLine>
                            '  <Product></Product>
                            '  <Quantity></Quantity>
                            '</TemplateLine>
                            '---------------------------------------------------------------
                            dea = New DEAlerts
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "Product"
                                        dea.ProductCode = Node2.InnerText
                                    Case Is = "Quantity"
                                        dea.Quantity = Node2.InnerText
                                End Select
                            Next
                            Depa.CollDEAlerts.Add(dea)
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQAT-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace