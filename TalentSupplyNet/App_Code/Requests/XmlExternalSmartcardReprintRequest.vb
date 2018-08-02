Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
Imports System.Collections.Generic

Namespace Talent.TradingPortal

    Public Class XmlExternalSmartcardReprintRequest
        Inherits XmlRequest

        Private _productCode As String = String.Empty
        Private _smartCardProperties As DESmartcard

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlExternalSmartcardReprintResponse = CType(xmlResp, XmlExternalSmartcardReprintResponse)
            Dim smartCardInterface As New TalentSmartcard

            _smartCardProperties = New DESmartcard
            _smartCardProperties.ExternalReprintRequests = New List(Of DESmartcardExternalReprint)()

            Dim err As ErrorObj = Nothing

            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Else
                    err = New ErrorObj
                    With err
                        .ErrorMessage = "Invalid document version " & MyBase.DocumentVersion
                        .ErrorStatus = "XmlExternalSmartcardReprintRequest Error - Invalid Doc Version " & MyBase.DocumentVersion
                        .ErrorNumber = "SCEXTPRT-02"
                        .HasError = True
                    End With
            End Select

            If err.HasError Then
                xmlResp.Err = err
            Else
                smartCardInterface.Settings = Settings
                smartCardInterface.DE = _smartCardProperties
                err = smartCardInterface.ExternalSmartcardReprintRequest
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = smartCardInterface.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dim Node1, Node2, Node3 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ExternalSmartcardReprintRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                        Case Is = "SmartcardReprints"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "SmartcardReprint"
                                        Dim externalReprintReq As New DESmartcardExternalReprint
                                        With externalReprintReq
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerNumber"
                                                        .PrefixedCustomerNumber = Node3.InnerText
                                                    Case Is = "RealCustomerNumber"
                                                        .CustomerNumber = Node3.InnerText
                                                    Case Is = "SmartcardNumber"
                                                        .SmartcardNumber = Node3.InnerText
                                                    Case Is = "OldCardNumber"
                                                        .OldSmartcardNumber = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        .ProductCode = Node3.InnerText
                                                    Case Is = "StadiumCode"
                                                        .StadiumCode = Node3.InnerText
                                                    Case Is = "StandCode"
                                                        .StandCode = Node3.InnerText
                                                    Case Is = "AreaCode"
                                                        .AreaCode = Node3.InnerText
                                                    Case Is = "Row"
                                                        .Row = Node3.InnerText
                                                    Case Is = "SeatNumber"
                                                        .Seat = Node3.InnerText
                                                    Case Is = "SeatSuffix"
                                                        .AlphaSeat = Node3.InnerText
                                                End Select
                                            Next Node3
                                        End With
                                        _smartCardProperties.ExternalReprintRequests.Add(externalReprintReq)
                                End Select
                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "SCEXTPRT-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
    End Class

End Namespace