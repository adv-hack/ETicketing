Imports System.Data
Imports System.Xml

Namespace Talent.TradingPortal

    Public Class XmlAddLoyaltyPointsResponse
        Inherits XmlResponse

        Private ndLoyaltyPoints, ndItem, ndheader, ndHeaderRootHeader As XmlNode
        Private ndCustomerNo, ndProductCode, ndDate, ndTime, ndStand, ndArea, ndRow, ndSeat, ndPoints, ndSuccess, ndReturnCode As XmlNode

        Protected Overrides Sub InsertBodyV1()
            Try
                If ResultDataSet IsNot Nothing Then
                    If ResultDataSet.Tables.Count > 0 Then
                        If ResultDataSet.Tables(0).Rows.Count > 0 Then
                            With MyBase.xmlDoc
                                ndLoyaltyPoints = .CreateElement("LoyaltyPoints")
                            End With
                            CreateItemNodes()
                        End If
                    End If
                End If

                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                ndheader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndheader.InsertAfter(ndLoyaltyPoints, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSLY-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Sub CreateItemNodes()
            For Each row As DataRow In ResultDataSet.Tables(0).Rows
                With MyBase.xmlDoc
                    ndItem = .CreateElement("Item")
                    ndCustomerNo = .CreateElement("CustomerNumber")
                    ndProductCode = .CreateElement("ProductCode")
                    ndDate = .CreateElement("Date")
                    ndTime = .CreateElement("Time")
                    ndStand = .CreateElement("StandCode")
                    ndArea = .CreateElement("AreaCode")
                    ndRow = .CreateElement("Row")
                    ndSeat = .CreateElement("Seat")
                    ndPoints = .CreateElement("Points")
                    ndReturnCode = .CreateElement("ReturnCode")
                    ndSuccess = .CreateElement("Success")
                End With
                ndCustomerNo.InnerText = row("CustomerNumber").ToString
                ndProductCode.InnerText = row("ProductCode").ToString
                ndDate.InnerText = row("Date").ToString
                ndTime.InnerText = row("Time").ToString
                ndStand.InnerText = row("Stand").ToString
                ndArea.InnerText = row("Area").ToString
                ndRow.InnerText = row("Row").ToString
                ndSeat.InnerText = row("Seat").ToString
                ndPoints.InnerText = row("Points").ToString
                ndReturnCode.InnerText = row("ReturnCode").ToString
                ndSuccess.InnerText = row("Success").ToString
                With ndItem
                    .AppendChild(ndCustomerNo)
                    .AppendChild(ndProductCode)
                    .AppendChild(ndDate)
                    .AppendChild(ndTime)
                    .AppendChild(ndStand)
                    .AppendChild(ndArea)
                    .AppendChild(ndRow)
                    .AppendChild(ndSeat)
                    .AppendChild(ndPoints)
                    .AppendChild(ndReturnCode)
                    .AppendChild(ndSuccess)
                End With
                ndLoyaltyPoints.AppendChild(ndItem)
            Next
        End Sub

    End Class

End Namespace