Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to retrieve a list of eligible customers for a
'                                   particular product
'
'       Date                        09/06/08
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQRETC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRetrieveEligibleTicketingCustomersRequest
        Inherits XmlRequest
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        Dim productSettings As DEProductSettings
        Private _deprod As New DEProductDetails
        Public Property Deprod() As DEProductDetails
            Get
                Return _deprod
            End Get
            Set(ByVal value As DEProductDetails)
                _deprod = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlCustomerList As XmlRetrieveEligibleTicketingCustomersResponse = CType(xmlResp, XmlRetrieveEligibleTicketingCustomersResponse)
            Dim err As ErrorObj = Nothing

            productSettings = CType(Me.Settings, DEProductSettings)

            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim PRODUCT As New TalentProduct
            If err.HasError Then
                xmlResp.Err = err
            Else
                With PRODUCT
                    .Settings = Settings
                    Deprod.Src = "S"
                    PRODUCT.De = Deprod
                    err = .RetrieveEligibleTicketingCustomers
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlCustomerList.ResultDataSet = PRODUCT.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlCustomerList, XmlResponse)

        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, node2 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrieveEligibleTicketingCustomersRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RetrieveEligibleCustomers"
                            Deprod = New DEProductDetails
                            For Each node2 In Node1.ChildNodes

                                Select Case node2.Name                                 
                                    Case Is = "ProductCode"
                                        Deprod.ProductCode = node2.InnerText
                                    Case Is = "FromCustomerNumber"
                                        Deprod.CustomerNumber = node2.InnerText
                                    Case Is = "IncludeProductPurchasers"
                                        productSettings.IncludeProductPurchasers = node2.InnerText

                                End Select
                            Next

                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQRETC-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace