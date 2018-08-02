Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlAddLoyaltyPointsRequest
        Inherits XmlRequest

        Private loyaltyCollection As New DELoyaltyList

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlAddLoyaltyPointsResponse = CType(xmlResp, XmlAddLoyaltyPointsResponse)
            Dim err As ErrorObj = Nothing
            Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)

            xmlAction.ResponseDirectory = Settings.ResponseDirectory
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            Dim loyalty As New TalentLoyalty
            If err.HasError Then
                xmlResp.Err = err
            Else
                With loyalty
                    .LoyaltyList = loyaltyCollection
                    Settings.RequestCount = loyalty.LoyaltyList.LoyaltyList.Count
                    .Settings = Settings
                    err = .AddLoyaltyPoints()
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = loyalty.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim loyaltyItem As New DELoyalty
            Dim err As New ErrorObj
            Dim Node1, Node2, Node3 As XmlNode

            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AddLoyaltyPointsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "LoyaltyPoints"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "Item"
                                        loyaltyItem = New DELoyalty
                                        With loyaltyItem
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CustomerNumber"
                                                        .CustomerNumber = Node3.InnerText
                                                    Case Is = "ProductCode"
                                                        .ProductCode = Node3.InnerText
                                                    Case Is = "Date"
                                                        .TheDate = Convert.ToDateTime(Node3.InnerText)
                                                    Case Is = "Time"
                                                        .TheTime = Node3.InnerText
                                                    Case Is = "StandCode"
                                                        .Stand = Node3.InnerText
                                                    Case Is = "AreaCode"
                                                        .Area = Node3.InnerText
                                                    Case Is = "Row"
                                                        .Row = Node3.InnerText
                                                    Case Is = "Seat"
                                                        .Seat = Node3.InnerText
                                                    Case Is = "Type"
                                                        If Node3.InnerText = "Attendance" Then
                                                            .Type = LoyaltyPointsType.Attendance
                                                        ElseIf Node3.InnerText = "Adhoc" Then
                                                            .Type = LoyaltyPointsType.Adhoc
                                                        End If
                                                    Case Is = "Points"
                                                        .Points = Node3.InnerText
                                                End Select
                                            Next Node3
                                        End With
                                        loyaltyCollection.LoyaltyList.Add(loyaltyItem)
                                End Select
                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQLY-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace