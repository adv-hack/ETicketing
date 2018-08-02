Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Customer Retrieval Requests
'
'       Date                        April 2007
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQCR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlCustomerRetrievalRequest
        Inherits XmlRequest

        Private _decust As New DECustomer
        Public Property Decust() As DECustomer
            Get
                Return _decust
            End Get
            Set(ByVal value As DECustomer)
                _decust = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlCustomerRetrievalResponse = CType(xmlResp, XmlCustomerRetrievalResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV1_1()
                Case Is = "1.2"
                    err = LoadXmlV1_2()
                Case Is = "1.3"
                    err = LoadXmlV1_3()
                Case Else
                    err = New ErrorObj
                    With err
                        .ErrorMessage = "Invalid document version " & MyBase.DocumentVersion
                        .ErrorStatus = "CustomerRetrievalRequest Error - Invalid Doc Version " & MyBase.DocumentVersion
                        .ErrorNumber = "TTPRQCR-02"
                        .HasError = True
                    End With
            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim CUSTOMER As New TalentCustomer
            If err.HasError Then
                xmlResp.Err = err
            Else
                Dim deCustV11 As New DECustomerV11
                deCustV11.DECustomersV1.Add(Decust)

                With CUSTOMER
                    .DeV11 = deCustV11
                    .Settings = Settings
                    .Settings.Cacheing = False
                    err = .CustomerRetrieval
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = CUSTOMER.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerRetrievalRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerRetrieval"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "CustomerNo"
                                            If Node2.InnerText.Contains("@") Then
                                                .EmailAddress = Node2.InnerText
                                            Else
                                                .CustomerNumber = Node2.InnerText
                                            End If
                                    End Select

                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerRetrievalRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerRetrieval"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "CustomerNo"
                                            If Node2.InnerText.Contains("@") Then
                                                .EmailAddress = Node2.InnerText
                                            Else
                                                .CustomerNumber = Node2.InnerText
                                            End If
                                        Case Is = "UserID1"
                                            .PassportNumber = Node2.InnerText

                                        Case Is = "UserID2"
                                            .GreenCardNumber = Node2.InnerText

                                        Case Is = "UserID3"
                                            .PIN_Number = Node2.InnerText

                                        Case Is = "UserID4"
                                            .User_ID_4 = Node2.InnerText

                                        Case Is = "UserID5"
                                            .User_ID_5 = Node2.InnerText

                                        Case Is = "UserID6"
                                            .User_ID_6 = Node2.InnerText

                                        Case Is = "UserID7"
                                            .User_ID_7 = Node2.InnerText

                                        Case Is = "UserID8"
                                            .User_ID_8 = Node2.InnerText

                                        Case Is = "UserID9"
                                            .User_ID_9 = Node2.InnerText

                                    End Select

                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_2() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_2"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XML document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerRetrievalRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerRetrieval"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "CustomerNo"
                                            If Node2.InnerText.Contains("@") Then
                                                .EmailAddress = Node2.InnerText
                                                .ProcessEmailAddress = "1"
                                            Else
                                                .CustomerNumber = Node2.InnerText
                                                .ProcessCustomerNumber = "1"
                                            End If

                                        Case Is = "UserID1"
                                            .PassportNumber = Node2.InnerText
                                            .ProcessPassportNumber = "1"
                                        Case Is = "UserID2"
                                            .GreenCardNumber = Node2.InnerText
                                            .ProcessGreenCardNumber = "1"
                                        Case Is = "UserID3"
                                            .PIN_Number = Node2.InnerText
                                            .ProcessPinNumber = "1"
                                    End Select

                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV1_3() As ErrorObj
            Const ModuleName As String = "LoadXmlV1_3"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerRetrievalRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "CustomerRetrieval"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "ID" : .ID = Node2.InnerText
                                        Case Is = "IDType" : .IDType = Node2.InnerText
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace