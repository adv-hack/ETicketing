Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlExternalSmartcardReprintResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndResponse, ndSmartcardReprints, ndSmartcardReprint As XmlNode
        Private ndReturnCode, ndCustomerNo, ndRealCustomerNumber, ndSmartcardNumber, ndStatus, ndStatusInformation As XmlNode
        Private dtExternalReprintDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()
            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndSmartcardReprints = .CreateElement("SmartcardReprints")
                    ndResponse = .CreateElement("Response")
                    ndSmartcardReprint = .CreateElement("SmartcardReprint")
                End With

                Try
                    createResponseSection()
                    createExternalReprintList()
                Catch ex As Exception
                    Const strError As String = "Failed to create the response xml"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError
                        .ErrorNumber = "SCEXTPRT-03"
                        .HasError = True
                    End With
                End Try

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndSmartcardReprints, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "SCEXTPRT-04"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Sub createResponseSection()
            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            'Read the values for the response section
            dtStatusDetails = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
            dr = dtStatusDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccurred") = "E" Then
                errorOccurred = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With

            ' add to the list
            With ndSmartcardReprints
                .AppendChild(ndResponse)
            End With
        End Sub

        Protected Sub createExternalReprintList()

            dtExternalReprintDetails = ResultDataSet.Tables("SmartcardExternalReprintResults")

            For Each dr As DataRow In dtExternalReprintDetails.Rows
                If Not errorOccurred Then

                    With MyBase.xmlDoc
                        ndCustomerNo = .CreateElement("CustomerNumber")
                        ndRealCustomerNumber = .CreateElement("RealCustomerNumber")
                        ndSmartcardNumber = .CreateElement("SmartcardNumber")
                        ndStatus = .CreateElement("Status")
                        ndStatusInformation = .CreateElement("StatusInformation")
                    End With

                    'Populate the nodes
                    ndCustomerNo.InnerText = dr("CustomerNumberWithPrefix").ToString.Trim
                    ndRealCustomerNumber.InnerText = dr("CustomerNumber").ToString.Trim
                    ndSmartcardNumber.InnerText = dr("SmartcardNumber").ToString.Trim
                    ndStatus.InnerText = dr("ReprintStatusCode").ToString.Trim
                    ndStatusInformation.InnerText = dr("ReprintStatusDescription").ToString.Trim

                    'Set the xml nodes
                    With ndSmartcardReprint
                        .AppendChild(ndCustomerNo)
                        .AppendChild(ndRealCustomerNumber)
                        .AppendChild(ndSmartcardNumber)
                        .AppendChild(ndStatus)
                        .AppendChild(ndStatusInformation)
                    End With

                    ' add to the list
                    With ndSmartcardReprints
                        .AppendChild(ndSmartcardReprint)
                    End With
                End If
            Next
        End Sub

    End Class

End Namespace