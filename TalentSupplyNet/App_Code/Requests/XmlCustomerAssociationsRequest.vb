Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Customer Retrieval Requests
'
'       Date                        April 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQCR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerAssociationsRequest
        Inherits XmlRequest

        Private _decust As New DECustomer
        Public Property Decust() As DECustomer
            Get
                Return _decust
            End Get
            Set(ByVal value As DECustomer)
                _decust = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlCustomerAssociationsResponse = CType(xmlResp, XmlCustomerAssociationsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim CUSTOMER As New TalentCustomer
            If err.HasError Then
                xmlResp.Err = err
            Else
                Dim deCustV11 As New DECustomerV11
                deCustV11.DECustomersV1.Add(Decust)
                Decust.Source = "S"
                With CUSTOMER
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerAssociations
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = CUSTOMER.ResultDataSet
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
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerAssociationsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "Customer"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "CustomerNo"
                                            .CustomerNumber = Node2.InnerText
                                    End Select

                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace