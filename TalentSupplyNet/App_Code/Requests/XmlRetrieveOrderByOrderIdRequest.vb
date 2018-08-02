Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal
    Public Class XmlRetrieveOrderByOrderIdRequest
        Inherits XmlRequest

#Region "Class Level Fields"

        Private _markOrdersAsComplete As Boolean = False
        Private _businessUnit As String = String.Empty
        Private _partner As String = String.Empty
        Private _orderId As String = String.Empty
        Private _tempOrderId As String = String.Empty
        Private _toDateToFindOrders As String = String.Empty
        Private _ExcludeCustInfo As String = String.Empty

#End Region

#Region "Public Methods"

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlRetrieveOrderByOrderIdResponse = CType(xmlResp, XmlRetrieveOrderByOrderIdResponse)
            Dim err As ErrorObj = Nothing
            Dim resultSet As New DataSet

            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV11()
            End Select
            If err.HasError Then
                xmlResp.Err = err
            Else
                Dim tDataObjects As New TalentDataObjects
                Dim dtRetrievedOrders As New DataTable("RetrievedOrders")
                tDataObjects.Settings = Settings
                dtRetrievedOrders = tDataObjects.OrderSettings.GetOrdersById(_orderId, _tempOrderId)
                resultSet.Tables.Add(dtRetrievedOrders.Copy)
            End If

            xmlAction.ResultDataSet = resultSet
            xmlAction.Settings = Settings
            'The settings.stadium property has been re-used here for ExcludeCustInfo flag
            xmlAction.Settings.Stadium = _ExcludeCustInfo
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
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveOrderByOrderIdRequest").ChildNodes
                    Select Case Node1.Name
                        Case "TransactionHeader"
                        Case "Orders"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case "WebOrderNo"
                                        _orderId = Node2.InnerText
                                    Case "WebTempOrderNo"
                                        _tempOrderId = Node2.InnerText
                                End Select
                            Next Node2
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "XmlRetrieveOrdersByIdRequest-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV11() As ErrorObj
            Const ModuleName As String = "LoadXmlV11"
            Dim err As New ErrorObj
            Dim Node1, Node2 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveOrderByOrderIdRequest").ChildNodes
                    Select Case Node1.Name
                        Case "TransactionHeader"
                        Case "Orders"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case "WebOrderNo"
                                        _orderId = Node2.InnerText
                                    Case "WebTempOrderNo"
                                        _tempOrderId = Node2.InnerText
                                    Case "ExcludeCustInfo"
                                        _ExcludeCustInfo = Node2.InnerText
                                End Select
                            Next Node2
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "XmlRetrieveOrdersByIdRequest-011"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


#End Region
    End Class
End Namespace

