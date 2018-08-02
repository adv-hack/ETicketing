Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlRetrieveOrdersByStatusRequest
        Inherits XmlRequest

#Region "Class Level Fields"

        Private _markOrdersAsComplete As Boolean = False
        Private _businessUnit As String = String.Empty
        Private _partner As String = String.Empty
        Private _orderStatusToFindOrdersFrom As String = String.Empty
        Private _fromDateToFindOrders As String = String.Empty
        Private _toDateToFindOrders As String = String.Empty

#End Region

#Region "Public Methods"

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlRetrieveOrdersByStatusResponse = CType(xmlResp, XmlRetrieveOrdersByStatusResponse)
            Dim err As ErrorObj = Nothing
            Dim resultSet As New DataSet

            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV1_1()
            End Select
            If err.HasError Then
                xmlResp.Err = err
            Else
                Dim tDataObjects As New TalentDataObjects
                Dim dtRetrievedOrders As New DataTable("RetrievedOrders")
                tDataObjects.Settings = Settings
                dtRetrievedOrders = tDataObjects.OrderSettings.GetOrdersByStatus(_partner, _orderStatusToFindOrdersFrom, _fromDateToFindOrders, _toDateToFindOrders)


                Select Case MyBase.DocumentVersion
                    Case Is = "1.1"
                        dtRetrievedOrders.Columns.Add("BILLING_ADDRESS_LINE_1", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_ADDRESS_LINE_2", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_ADDRESS_LINE_3", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_ADDRESS_LINE_4", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_ADDRESS_LINE_5", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_POST_CODE", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_COUNTRY", Type.GetType("System.String"))

                        dtRetrievedOrders.Columns.Add("BILLING_FULL_NAME", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("MOBILE_NUMBER", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("TELEPHONE_NUMBER", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("WORK_NUMBER", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("FAX_NUMBER", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("OTHER_NUMBER", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_CONTACT_TITLE", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_CONTACT_INITIALS", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_CONTACT_FORENAME", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_CONTACT_SURNAME", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_CONTACT_COMPANYNAME", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("BILLING_CONTACT_EMAIL", Type.GetType("System.String"))

                        dtRetrievedOrders.Columns.Add("MESSAGE", Type.GetType("System.String"))
                        dtRetrievedOrders.Columns.Add("RECIPIENT_NAME", Type.GetType("System.String"))

                        Dim myLoginId, tempOrderId As String
                        Dim dtRetrievedAddresses As New DataTable("RetrievedAddresses")
                        Dim dtRetrievedPhoneNums As New DataTable("RetrievedNumbers")
                        Dim dtRetrievedGiftMsg As New DataTable("Retrieved Gift Messages")


                        For Each myRow As DataRow In dtRetrievedOrders.Rows
                            myLoginId = myRow("LOGINID")
                            tempOrderId = myRow("TEMP_ORDER_ID")

                            dtRetrievedAddresses = tDataObjects.ProfileSettings.tblAddress.GetByLoginIdPartnerSequence(myLoginId, _partner, 0)
                            If (dtRetrievedAddresses.Rows.Count > 0) Then
                                myRow("BILLING_ADDRESS_LINE_1") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("ADDRESS_LINE_1")).ToString
                                myRow("BILLING_ADDRESS_LINE_2") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("ADDRESS_LINE_2")).ToString
                                myRow("BILLING_ADDRESS_LINE_3") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("ADDRESS_LINE_3")).ToString
                                myRow("BILLING_ADDRESS_LINE_4") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("ADDRESS_LINE_4")).ToString
                                myRow("BILLING_ADDRESS_LINE_5") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("ADDRESS_LINE_5")).ToString
                                myRow("BILLING_POST_CODE") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("POST_CODE")).ToString
                                myRow("BILLING_COUNTRY") = Utilities.CheckForDBNull_String(dtRetrievedAddresses.Rows(0)("COUNTRY")).ToString
                            End If
        
                            dtRetrievedPhoneNums = tDataObjects.ProfileSettings.tblPartnerUser.GetByLoginId(myLoginId, _partner)
                            If (dtRetrievedPhoneNums.Rows.Count > 0) Then
                                myRow("BILLING_FULL_NAME") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("FULL_NAME")).ToString
                                myRow("MOBILE_NUMBER") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("MOBILE_NUMBER")).ToString
                                myRow("TELEPHONE_NUMBER") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("TELEPHONE_NUMBER")).ToString
                                myRow("WORK_NUMBER") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("WORK_NUMBER")).ToString
                                myRow("FAX_NUMBER") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("FAX_NUMBER")).ToString
                                myRow("OTHER_NUMBER") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("OTHER_NUMBER")).ToString
                                myRow("BILLING_CONTACT_TITLE") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("TITLE")).ToString
                                myRow("BILLING_CONTACT_INITIALS") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("INITIALS")).ToString
                                myRow("BILLING_CONTACT_FORENAME") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("FORENAME")).ToString
                                myRow("BILLING_CONTACT_SURNAME") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("SURNAME")).ToString
                                myRow("BILLING_CONTACT_COMPANYNAME") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("COMPANYNAME")).ToString
                                myRow("BILLING_CONTACT_EMAIL") = Utilities.CheckForDBNull_String(dtRetrievedPhoneNums.Rows(0)("EMAIL")).ToString
                            End If
                            

                            dtRetrievedGiftMsg = tDataObjects.OrderSettings.TblGiftMessage.GetAllByTempOrderID(tempOrderId, _partner)
                            If (dtRetrievedGiftMsg.Rows.Count > 0) Then
                                myRow("RECIPIENT_NAME") = Utilities.CheckForDBNull_String(dtRetrievedGiftMsg.Rows(0)("RECIPIENT_NAME")).ToString
                                myRow("MESSAGE") = Utilities.CheckForDBNull_String(dtRetrievedGiftMsg.Rows(0)("MESSAGE")).ToString
                            End If
                        Next
                End Select
                resultSet.Tables.Add(dtRetrievedOrders.Copy)
            End If

            xmlAction.ResultDataSet = resultSet
            xmlAction.MarkOrdersAsComplete = _markOrdersAsComplete
            xmlAction.OrderCompleteStatus = "ORDER PROCESSED"
            xmlAction.Settings = Settings
            xmlAction.Settings.BusinessUnit = _businessUnit
            xmlAction.Settings.Partner = _partner
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

#End Region

#Region "Private Methods"

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dim Node1, Node2 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveOrdersByStatusRequest").ChildNodes
                    Select Case Node1.Name
                        Case "TransactionHeader"
                        Case "Orders"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case "MarkOrdersAsComplete"
                                        _markOrdersAsComplete = CBool(Node2.InnerText)
                                    Case "BusinessUnit"
                                        _businessUnit = Node2.InnerText
                                    Case "Partner"
                                        _partner = Node2.InnerText
                                    Case "Status"
                                        _orderStatusToFindOrdersFrom = Node2.InnerText
                                    Case "FromDate"
                                        _fromDateToFindOrders = Node2.InnerText
                                    Case "ToDate"
                                        _toDateToFindOrders = Node2.InnerText
                                End Select
                            Next Node2
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "XmlRetrieveOrdersByStatusRequest-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_1"
            Dim err As New ErrorObj
            Dim Node1, Node2 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveOrdersByStatusRequest").ChildNodes
                    Select Case Node1.Name
                        Case "TransactionHeader"
                        Case "Orders"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case "MarkOrdersAsComplete"
                                        _markOrdersAsComplete = CBool(Node2.InnerText)
                                    Case "BusinessUnit"
                                        _businessUnit = Node2.InnerText
                                    Case "Partner"
                                        _partner = Node2.InnerText
                                    Case "Status"
                                        _orderStatusToFindOrdersFrom = Node2.InnerText
                                    Case "FromDate"
                                        _fromDateToFindOrders = Node2.InnerText
                                    Case "ToDate"
                                        _toDateToFindOrders = Node2.InnerText
                                End Select
                            Next Node2
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "XmlRetrieveOrdersByStatusRequest-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

#End Region
    End Class

End Namespace