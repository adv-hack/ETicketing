Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Profile
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Retrieve Password Requests
'
'       Date                        04/02/08
'
'       Author                      Ben
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQRP- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRetrievePasswordRequest
        Inherits XmlRequest

        Private SendForgottenPasswordEmail As Boolean = False

        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
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

            Dim xmlAction As XmlRetrievePasswordResponse = CType(xmlResp, XmlRetrievePasswordResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV11()
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
                    err = .CustomerRetrieval
                End With

                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = CUSTOMER.ResultDataSet
            xmlAction.EmailAddress = Decust.EmailAddress
            xmlAction.SendForgottenPasswordEmail = SendForgottenPasswordEmail


            xmlResp.Settings = Settings
            xmlResp.SenderID = Settings.SenderID
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
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrievePasswordRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RetrievePassword"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "UserName"
                                            If Node2.InnerText.Contains("@") Then
                                                .EmailAddress = Node2.InnerText
                                            Else
                                                .UserName = Node2.InnerText
                                                .CustomerNumber = Node2.InnerText
                                            End If

                                        Case Is = "EmailPasswordToUser"
                                            SendForgottenPasswordEmail = Convert.ToBoolean(Node2.InnerText)

                                    End Select

                                Next Node2
                            End With

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQRP-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function LoadXmlV11() As ErrorObj
            Const ModuleName As String = "LoadXmlV11"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrievePasswordRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RetrievePassword"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "UserName"
                                            If Node2.InnerText.Contains("@") Then
                                                .EmailAddress = Node2.InnerText
                                            Else
                                                .UserName = Node2.InnerText
                                                .CustomerNumber = Node2.InnerText
                                            End If

                                        Case Is = "EmailPasswordToUser"
                                            SendForgottenPasswordEmail = Convert.ToBoolean(Node2.InnerText)

                                    End Select

                                Next Node2
                            End With

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQRP-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace