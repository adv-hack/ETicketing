Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Data
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlAddOrderTrackingResponse
        Inherits XmlResponse

        Private ndAddOrderTracking, ndheader, ndHeaderRootHeader As XmlNode
        Private ndResponse, ndReturnCode As XmlNode
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()
            Try
                If ResultDataSet IsNot Nothing Then
                    If ResultDataSet.Tables.Count > 0 Then
                        If ResultDataSet.Tables(0).Rows.Count > 0 Then
                            With MyBase.xmlDoc
                                ndAddOrderTracking = .CreateElement("OrderTrackingResponse")
                                ndResponse = .CreateElement("Response")
                            End With
                        End If
                    End If
                End If

                CreateResponseSection()

                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                ndheader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndheader.InsertAfter(ndAddOrderTracking, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "XmlAddOrderTrackingRequest.vb-01"
                    .HasError = True
                End With
            End Try
        End Sub

        

        Protected Sub CreateResponseSection()

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            'Read the values for the response section
            dtStatusDetails = ResultDataSet.Tables(0)
            dr = dtStatusDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                errorOccurred = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With

        End Sub

    End Class


End Namespace