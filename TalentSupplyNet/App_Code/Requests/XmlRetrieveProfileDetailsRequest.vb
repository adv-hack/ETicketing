Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
Namespace Talent.TradingPortal
    Public Class XmlRetrieveProfileDetailsRequest
        Inherits XmlRequest


        Private _de As New DETicketingItemDetails
        Private _deO As New DEOrder
        Private _deCust As New DECustomer

        Public Property DeCust() As DECustomer
            Get
                Return _deCust
            End Get
            Set(ByVal value As DECustomer)
                _deCust = value
            End Set
        End Property

        Public Property De() As DETicketingItemDetails
            Get
                Return _de
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _de = value
            End Set
        End Property
        Public Property DeO() As DEOrder
            Get
                Return _deO
            End Get
            Set(ByVal value As DEOrder)
                _deO = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRetrieveProfileDetailsResponse = CType(xmlResp, XmlRetrieveProfileDetailsResponse)
            Dim err As New ErrorObj
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select

            Dim deCustV11 As New DECustomerV11
            deCustV11.DECustomersV1.Add(DeCust)


            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim Customer As New TalentCustomer
            If err.HasError Then
                xmlResp.Err = err
            Else
                With Customer
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerProfileRetrieval
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlAction.Err = err
                End If

            End If

            xmlAction.ResultDataSet = Customer.ResultDataSet
            xmlAction.SenderID = Settings.SenderID
            xmlAction.CreateResponse()
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
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveProfileDetailsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "ProfileDetails"
                            DeCust = New DECustomer
                            With DeCust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Print Request Details
                                        '
                                        Case Is = "CustomerNo"
                                            .CustomerNumber = Node2.InnerText
                                        Case Is = "Category"
                                            .Category = Node2.InnerText
                                        Case Is = "EmailAddress"
                                            .EmailAddress = Node2.InnerText
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

                'Set the xml information to the order data entity
                DeO = New DEOrder
                DeO.CollDEOrders.Add(De)

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOD-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace
