Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common


Namespace Talent.TradingPortal


    Public Class XmlAddOrderTrackingRequest
        Inherits XmlRequest

        Private deDespatch As New DEDespatch

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlAddOrderTrackingResponse = CType(xmlResp, XmlAddOrderTrackingResponse)

            Dim err As ErrorObj = Nothing
            
            xmlAction.ResponseDirectory = Settings.ResponseDirectory
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            Dim talentDespatch As New TalentDespatch
            If err.HasError Then
                xmlResp.Err = err
            Else
                With talentDespatch
                    .DeDespatch = deDespatch
                    .DeDespatch.Source = GlobalConstants.SOURCESUPPLYNET
                    .Settings = Settings

                    err = .CreateTrackingReferences()
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = talentDespatch.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dim Node1, Node2 As XmlNode

            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AddOrderTrackingRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "OrderDetails"
                            With deDespatch
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name

                                        Case Is = "PaymentReference"
                                            .PaymentRef = Node2.InnerText.Trim()

                                        Case Is = "TrackingReference"
                                            .TrackingReference = Node2.InnerText

                                    End Select
                                Next Node2
                             End With

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "XmlAddOrderTrackingRequest.vb-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


    End Class

End Namespace


