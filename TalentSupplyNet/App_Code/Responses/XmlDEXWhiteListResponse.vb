Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlDEXWhiteListResponse
        Inherits XmlResponse

        Private ndProcessingInstruction, ndRoot, ndErrorOccurred, ndReturnCode As XmlNode
        Private ndWhiteListEntry As XmlNode
        Private ndEventName, ndEventDate, ndEventId, ndVenueName, ndVenueID, ndCustomerName, ndBasketRef As XmlNode
        Private ndTicketID, ndTicketSaleDate, ndTicketPrintDate, ndTicketSaleTerminal As XmlNode
        Private ndBarcode, ndBarcodeType, ndAreaName, ndAreaID, ndSectionName, ndSectionID, ndRowNo, ndSeatNo, ndEntranceDesc, ndGateDesc, ndPriceType As XmlNode
        Private atClearList As XmlAttribute
        Private dtWhiteList As New DataTable

        Protected Overrides Sub InsertHeaderV1()
            Dim atXmlNs As XmlAttribute
            With xmlDoc
                Const str1 As String = "xml"
                Const str2 As String = "version=""1.0"" encoding=""utf-8"""
                Const str3 As String = "xmlns"
                Const str4 As String = "XMLNamespace"
                ndProcessingInstruction = .CreateProcessingInstruction(str1, str2)
                ndRoot = .CreateElement("WhiteList")
                atXmlNs = .CreateAttribute(str3)
                atXmlNs.Value = ConfigurationManager.AppSettings(str4).ToString()
                ndRoot.Attributes.Append(atXmlNs)
                .AppendChild(ndProcessingInstruction)
                .AppendChild(ndRoot)
            End With
        End Sub

        Protected Overrides Sub InsertBodyV1()
            Try
                CreateWhiteList()
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "DEXWHITE-03"
                    .HasError = True
                End With
            End Try

            If Err.HasError Then
                ndErrorOccurred = xmlDoc.CreateElement("ErrorOccurred")
                ndReturnCode = xmlDoc.CreateElement("ReturnCode")
                Dim errorCodeAttribute As XmlAttribute
                errorCodeAttribute = xmlDoc.CreateAttribute("ErrorCode")
                errorCodeAttribute.Value = Err.ErrorNumber
                ndErrorOccurred.Attributes.Append(errorCodeAttribute)
                ndErrorOccurred.InnerText = Err.ErrorMessage
                If ResultDataSet IsNot Nothing AndAlso ResultDataSet.Tables.Count = 2 Then
                    If ResultDataSet.Tables(1).Rows.Count > 0 AndAlso ResultDataSet.Tables(1).Rows(0)(0).ToString() = "E" Then
                        ndReturnCode.InnerText = ResultDataSet.Tables(1).Rows(0)(1).ToString()
                    End If
                End If
                With ndRoot
                    .AppendChild(ndErrorOccurred)
                    .AppendChild(ndReturnCode)
                End With
            End If
        End Sub

        Protected Sub CreateWhiteList()
            Dim dr As DataRow = Nothing
            If Not Err.HasError Then
                dtWhiteList = ResultDataSet.Tables(1)

                For Each dr In dtWhiteList.Rows
                    With xmlDoc
                        ndWhiteListEntry = .CreateElement("WhiteListEntry")
                        ndEventName = .CreateElement("EventName")
                        ndEventDate = .CreateElement("EventDate")
                        ndEventId = .CreateElement("EventID")
                        ndVenueName = .CreateElement("VenueName")
                        ndVenueID = .CreateElement("VenueID")
                        ndCustomerName = .CreateElement("CustomerName")
                        ndBasketRef = .CreateElement("BasketRef")
                        ndTicketID = .CreateElement("TicketID")
                        ndTicketSaleDate = .CreateElement("TicketSaleDate")
                        ndTicketPrintDate = .CreateElement("TicketPrintDate")
                        ndTicketSaleTerminal = .CreateElement("TicketSaleTerminal")
                        ndBarcode = .CreateElement("Barcode")
                        ndBarcodeType = .CreateElement("BarcodeType")
                        ndAreaName = .CreateElement("AreaName")
                        ndAreaID = .CreateElement("AreaID")
                        ndSectionName = .CreateElement("SectionName")
                        ndSectionID = .CreateElement("SectionID")
                        ndRowNo = .CreateElement("RowNo")
                        ndSeatNo = .CreateElement("SeatNo")
                        ndEntranceDesc = .CreateElement("EntranceDesc")
                        ndGateDesc = .CreateElement("GateDesc")
                        ndPriceType = .CreateElement("PriceType")
                    End With

                    ndEventName.InnerText = dr("EventName").ToString()
                    ndEventDate.InnerText = dr("EventDate").ToString()
                    ndEventId.InnerText = dr("EventID").ToString()
                    ndVenueName.InnerText = dr("VenueName").ToString()
                    ndVenueID.InnerText = dr("VenueID").ToString()
                    ndCustomerName.InnerText = dr("CustomerName").ToString()
                    ndBasketRef.InnerText = dr("BasketRef").ToString()
                    ndTicketID.InnerText = dr("TicketID").ToString()
                    ndTicketSaleDate.InnerText = dr("TicketSaleDate").ToString()
                    ndTicketPrintDate.InnerText = dr("TicketPrintDate").ToString()
                    ndTicketSaleTerminal.InnerText = dr("TicketSaleTerminal").ToString()
                    ndBarcode.InnerText = dr("Barcode").ToString()
                    ndBarcodeType.InnerText = dr("BarcodeType").ToString()
                    ndAreaName.InnerText = dr("AreaName").ToString()
                    ndAreaID.InnerText = dr("AreaID").ToString()
                    ndSectionName.InnerText = dr("SectionName").ToString()
                    ndSectionID.InnerText = dr("SectionID").ToString()
                    ndRowNo.InnerText = dr("RowNo").ToString()
                    ndSeatNo.InnerText = dr("SeatNo").ToString()
                    ndEntranceDesc.InnerText = dr("EntranceDesc").ToString()
                    ndGateDesc.InnerText = dr("GateDesc").ToString()
                    ndPriceType.InnerText = dr("PriceType").ToString()

                    With ndWhiteListEntry
                        .AppendChild(ndEventName)
                        .AppendChild(ndEventDate)
                        .AppendChild(ndEventId)
                        .AppendChild(ndVenueName)
                        .AppendChild(ndVenueID)
                        .AppendChild(ndCustomerName)
                        .AppendChild(ndBasketRef)
                        .AppendChild(ndTicketID)
                        .AppendChild(ndTicketSaleDate)
                        .AppendChild(ndTicketPrintDate)
                        .AppendChild(ndTicketSaleTerminal)
                        .AppendChild(ndBarcode)
                        .AppendChild(ndBarcodeType)
                        .AppendChild(ndAreaName)
                        .AppendChild(ndAreaID)
                        .AppendChild(ndSectionName)
                        .AppendChild(ndSectionID)
                        .AppendChild(ndRowNo)
                        .AppendChild(ndSeatNo)
                        .AppendChild(ndEntranceDesc)
                        .AppendChild(ndGateDesc)
                        .AppendChild(ndPriceType)
                    End With

                    With ndRoot
                        .AppendChild(ndWhiteListEntry)
                    End With

                Next dr
            End If
        End Sub

    End Class

End Namespace