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

    Public Class XmlRetrievePromotionsRequest
        Inherits XmlRequest
        Private promotionSettings As New DEPromotionSettings
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
      
            Dim xmlAction As XmlRetrievePromotionsResponse = CType(xmlResp, XmlRetrievePromotionsResponse)
            Dim err As ErrorObj = Nothing
            Dim PROMOTIONS As New TalentPromotions

            promotionSettings = CType(Me.Settings, DEPromotionSettings)
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If err.HasError Then
                xmlResp.Err = err
            Else
                With PROMOTIONS
                    .Settings = Settings
                    .Dep = New DEPromotions
                    err = .GetPromotionDetails
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = PROMOTIONS.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//RetrievePromotionsRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "PromotionList"
                            For Each Node2 As XmlNode In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "ReturnOnlyValidPromotions"
                                        promotionSettings.IncludeProductPurchasers = Node2.InnerText
                                End Select
                            Next
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQPL-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace