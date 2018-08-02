Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Availability Requests
'
'       Date                        Dec 2006
' 
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQAV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAvailabilityRequest
        Inherits XmlRequest

        'Stuart20070529
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        'Stuart20070529

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
            Dim xmlAction As XmlAvailabilityResponse = CType(xmlResp, XmlAvailabilityResponse)
            Dim pa As New TalentAvailibilty
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
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
                    err = .Availability
                    Depa = .Dep
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .SenderID = Settings.SenderID
                .Dep = Depa
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1 As XmlNode
            Dim dea As New DEAlerts                 ' Items
            Depa = New DEProductAlert
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//AvailabilityRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Depa.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = "AvailabilityInformation"
                            '   <AvailabilityInformation SKU="123A321" ManufacturerPartNumber="" ReservedInventory ="Y"/> 
                            '-----------------------------------------------------------------------
                            dea = New DEAlerts
                            With dea
                                .ProductCode = Node1.Attributes("SKU").Value
                                .ManufacturerPartNumber = Node1.Attributes("ManufacturerPartNumber").Value & String.Empty
                                .ReservedInventory = Node1.Attributes("ReservedInventory").Value & String.Empty
                            End With
                            Depa.CollDEAlerts.Add(dea)
                            '---------------------------------------------------------------------------------
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQAV-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace