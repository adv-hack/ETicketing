Imports Microsoft.VisualBasic
Imports Talent.TradingPortal
Imports Talent.common
Imports System.Xml

Namespace Talent.TradingPortal
    Public Class XmlRetrieveTransactionProgressRequest
        Inherits XmlRequest

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRetrieveTransactionProgressResponse = CType(xmlResp, XmlRetrieveTransactionProgressResponse)
            Dim err As ErrorObj = Nothing            



            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            'Sets the progress
            getProgress()
            xmlAction.ProgressTransactionID = ProgressTransactionID
            xmlAction.ProcessedRecords = ProcessedRecords
            xmlAction.TotalRecords = TotalRecords
            xmlAction.Complete = Complete

            '   Place the Request
            '
            If err.HasError Then
                xmlAction.Err = err
            Else
                If err.HasError Or Not err Is Nothing Then
                    xmlAction.Err = err
                End If

            End If

            xmlAction.SenderID = Settings.SenderID
            xmlAction.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function


        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"

            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            '-------------------------------------------------------------------------------------
            '  Retrieve the transaction id of the request we are tracking (the transaction id contained within the
            '  body of the xml not the TransationHeader.
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveTransactionProgressRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                        Case Is = "RetrieveTransactionProgress"
                            For Each Node2 As XmlNode In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "TransactionID"
                                        ProgressTransactionID = Node2.InnerText
                                End Select
                            Next
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPL-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace