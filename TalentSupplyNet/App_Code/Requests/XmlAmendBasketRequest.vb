Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Basket Requests
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQAB- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAmendBasketRequest
        Inherits XmlRequest

        Private _depa As New DEAmendBasket
        Public Property Depa() As DEAmendBasket
            Get
                Return _depa
            End Get
            Set(ByVal value As DEAmendBasket)
                _depa = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlAmendBasketResponse = CType(xmlResp, XmlAmendBasketResponse)
            Dim pa As New TalentBasket
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
                    If Depa.AddToBasket Then _
                        err = .AddToBasket
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
            Depa = New DEAmendBasket
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AmendBasketRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Depa.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "ProcessingOptions"
                            '---------------------------------------------------------------
                            '<ProcessingOptions>
                            '  <ReplaceBasket></ReplaceBasket>
                            '  <AddToBasket></AddToBasket>
                            '  <DeleteBasket></DeleteBasket>
                            '  <DeleteFromBasket></DeleteFromBasket>
                            '</ProcessingOptions>
                            '---------------------------------------------------------------
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "ReplaceBasket"
                                        Depa.ReplaceBasket = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "AddToBasket"
                                        Depa.AddToBasket = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "DeleteBasket"
                                        Depa.DeleteBasket = CType(Node2.InnerText & String.Empty, Boolean)
                                    Case Is = "DeleteFromBasket"
                                        Depa.DeleteFromBasket = CType(Node2.InnerText & String.Empty, Boolean)
                                End Select
                            Next
                        Case Is = "BasketHeader"
                            '---------------------------------------------------------------
                            '<BasketHeader>
                            '  <BusinessUnit></BusinessUnit>
                            '  <Partner></Partner>
                            '  <UserID></UserID>
                            '</BasketHeader>
                            '---------------------------------------------------------------
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "BusinessUnit"
                                        Depa.BusinessUnit = Node2.InnerText
                                    Case Is = "Partner"
                                        Depa.PartnerCode = Node2.InnerText
                                    Case Is = "UserID"
                                        Depa.UserID = Node2.InnerText
                                End Select
                            Next
                        Case Is = "BasketLine"
                            '---------------------------------------------------------------
                            '<BasketLine>
                            '  <Product></Product>
                            '  <Quantity></Quantity>
                            '</BasketLine>
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
                    .ErrorNumber = "TTPRQAB-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace