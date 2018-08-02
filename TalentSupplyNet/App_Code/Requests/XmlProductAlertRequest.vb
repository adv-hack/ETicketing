Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Alerts
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White   
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQPAR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlProductAlertRequest
        Inherits XmlRequest
        Private _depa As DEProductAlert

        Public Property Depa() As DEProductAlert
            Get
                Return _depa
            End Get
            Set(ByVal value As DEProductAlert)
                _depa = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlProductAlertResponse = CType(xmlResp, XmlProductAlertResponse)
            Dim pa As New TalentProductAlert
            Dim err As ErrorObj = Nothing
            '
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With pa
                    .Dep = Depa
                    .Settings = Settings
                    err = .ProductAlert
                    Depa = .Dep
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .Depa = Depa
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1, Node2 As XmlNode
            Dim dea As New DEAlerts                 ' Items
            Depa = New DEProductAlert
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//ProductAlertRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Depa.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "ProductAlert"
                            '----------------------------------------------------------------------------------
                            '   <ProductAlert>
                            '       <SKU>123A321</SKU>
                            '       <EMailAddress>marksmith@domain.com</EMailAddress>
                            '       <FirstName>Mark</FirstName>
                            '       <LastName>Smith</LastName>
                            '       <ExpiryDate>2009-01-01</ExpiryDate>
                            '   </ProductAlert>
                            '---------------------------------------------------------------------------------
                            dea = New DEAlerts
                            With dea
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "SKU" : .ProductCode = Node2.InnerText
                                        Case Is = "EMailAddress" : .EMailAddress = Node2.InnerText
                                        Case Is = "FirstName" : .FirstName = Node2.InnerText
                                        Case Is = "LastName" : .LastName = Node2.InnerText
                                        Case Is = "ExpiryDate" : .ExpiryDate = Node2.InnerText
                                    End Select
                                Next Node2
                            End With
                            Depa.CollDEAlerts.Add(dea)
                            '---------------------------------------------------------------------------------
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPAR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace