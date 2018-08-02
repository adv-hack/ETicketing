Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Order Ack  
'
'       Date                        11/10/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlSupplierOrderAcknowledgementRequest
        Inherits XmlRequest

        Private _din As DESupplier
        Public Property Din() As DESupplier
            Get
                Return _din
            End Get
            Set(ByVal value As DESupplier)
                _din = value
            End Set
        End Property
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlSupplierOrderAcknowledgementResponse = CType(xmlResp, XmlSupplierOrderAcknowledgementResponse)
            Dim invoicing As New TalentSupplier()
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With invoicing
                    .Dep = Din
                    .Settings = Settings
                    err = .WriteOrderAcknowledgement
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = invoicing.ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            Dim detr As New DETransaction           ' Items
            Dim deoh As DESupplierOrderAcknowledgement = Nothing
            Dim deol As DESupplierOrderAcknowledgementLines = Nothing
            Dim PoNumber As String = String.Empty
            Din = New DESupplier
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//SupplierOrderAcknowledgementRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Din.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "OrderAcknowledgements"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "OrderAcknowledgement"
                                        deoh = New DESupplierOrderAcknowledgement
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "OrderAcknowledgementHeader"
                                                    '----------------------------------------------------------
                                                    ' <OrderAcknowledgementHeader>
                                                    '  <VendorNumber>ag_vendor001</VendorNumber>
                                                    '  <PurchaseOrderNumber>ag_PO001</PurchaseOrderNumber>
                                                    '  <DepatchDate>20070323</DepatchDate>
                                                    '</OrderAcknowledgementHeader>
                                                    '----------------------------------------------------------
                                                    deoh = New DESupplierOrderAcknowledgement
                                                    With deoh
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "CompanyCode" : .CompanyCode = Node4.InnerText
                                                                Case Is = "VendorNumber" : .VendorNumber = Node4.InnerText
                                                                Case Is = "PurchaseOrderNumber" : .PoNumber = Node4.InnerText
                                                                Case Is = "DespatchDate" : .DespatchDate = Node4.InnerText

                                                            End Select
                                                        Next Node4
                                                        PoNumber = .PoNumber
                                                        Din.collDEHeader.Add(deoh)
                                                    End With

                                                Case Is = "OrderAcknowledgementLines"
                                                    '----------------------------------------------------------
                                                    ' <OrderAcknowledgementLines>
                                                    '  <OrderAcknowledgementLine LineNumber="1">
                                                    '    <ProductCode>ABC123</ProductCode>
                                                    '    <OrderQuantity>5000</OrderQuantity>
                                                    '    <UnitPrice>12.99</UnitPrice>
                                                    '    <DeliveryDate>20081110</DeliveryDate>         
                                                    '  </OrderAcknowledgementLine>
                                                    '  <OrderAcknowledgementLine LineNumber="2">
                                                    '    <ProductCode>DEF123</ProductCode>
                                                    '    <OrderQuantity>5000</OrderQuantity>
                                                    '    <UnitPrice>12.99</UnitPrice>
                                                    '    <DeliveryDate>20081110</DeliveryDate>
                                                    '  </OrderAcknowledgementLine>
                                                    '</OrderAcknowledgementLines>
                                                    '----------------------------------------------------------
                                                    For Each Node4 In Node3.ChildNodes
                                                        Select Case Node4.Name
                                                            Case Is = "OrderAcknowledgementLine"
                                                                deol = New DESupplierOrderAcknowledgementLines
                                                                With deol
                                                                    .PoNumber = PoNumber
                                                                    .LineNumber = Node4.Attributes("LineNumber").Value
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "ProductCode" : .ProductCode = Node5.InnerText
                                                                            Case Is = "OrderQuantity" : .OrderQuantity = CInt(Node5.InnerText)
                                                                            Case Is = "UnitPrice" : .UnitPrice = Node5.InnerText
                                                                            Case Is = "DeliveryDate" : .DeliveryDate = Node5.InnerText
                                                                        End Select
                                                                    Next Node5
                                                                    Din.collDEInfo.Add(deol)
                                                                End With
                                                        End Select
                                                    Next Node4
                                                    '----------------------------------------------------------------------------------------------
                                            End Select
                                        Next Node3
                                End Select
                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRSOA-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


    End Class

End Namespace