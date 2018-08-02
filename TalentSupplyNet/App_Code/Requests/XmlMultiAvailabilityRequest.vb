Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Multiple Availability Requests
'
'       Date                        April 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQMAV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlMultiAvailabilityRequest
        Inherits XmlRequest
        Private _depa As New DEProductAlert

        Public Property Depa() As DEProductAlert
            Get
                Return _depa
            End Get
            Set(ByVal value As DEProductAlert)
                _depa = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlMultiAvailabilityResponse = CType(xmlResp, XmlMultiAvailabilityResponse)
            Dim pa As New TalentAvailibilty
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
            End Select

            '--------------------------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With pa
                    .Dep = Depa
                    .Settings = Settings
                    err = .MultipleAvailability
                    ResultDataSet = .ResultDataSet

                End With
            End If
            '--------------------------------------------------------------------------------------
            With xmlAction
                .Err = err
                .SenderID = Settings.SenderID
                .ResultDataSet = ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1 As XmlNode
            Dim dea As New DEAlerts                 ' Items
            Depa = New DEProductAlert
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//MultiAvailabilityRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Depa.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = "Availability"
                            '     <Availability SKU="350271" Warehouse="MK" />
                            '-----------------------------------------------------------------------
                            dea = New DEAlerts
                            With dea
                                .ProductCode = Node1.Attributes("SKU").Value
                                .BranchID = Node1.Attributes("Warehouse").Value
                            End With
                            Depa.CollDEAlerts.Add(dea)
                            '-----------------------------------------------------------------------
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQMAV-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace