Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlDEXWhiteListRequest
        Inherits XmlRequest

        Private _productCode As String = String.Empty

        Public Property ProductCode() As String
            Get
                Return _productCode
            End Get
            Set(ByVal value As String)
                _productCode = value
            End Set
        End Property

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlDEXWhiteListResponse = CType(xmlResp, XmlDEXWhiteListResponse)
            Dim smartCardInterface As New TalentSmartcard
            Dim smartCardProperties As New DESmartcard
            Dim err As ErrorObj = Nothing

            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Else
                    err = New ErrorObj
                    With err
                        .ErrorMessage = "Invalid document version " & MyBase.DocumentVersion
                        .ErrorStatus = "DEXWhiteListRequest Error - Invalid Doc Version " & MyBase.DocumentVersion
                        .ErrorNumber = "DEXWHITE-02"
                        .HasError = True
                    End With
            End Select

            If err.HasError Then
                xmlResp.Err = err
            Else
                smartCardProperties.DEXListMode = "G"
                smartCardProperties.ProductCode = _productCode
                smartCardInterface.Settings = Settings
                smartCardInterface.DE = smartCardProperties
                err = smartCardInterface.RetrieveDEXList()
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = smartCardInterface.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dim Node1 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//DEXWhiteListRequest").ChildNodes
                    If Node1.Name = "ProductCode" Then
                        ProductCode = Node1.InnerText
                        Exit For
                    End If
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "DEXWHITE-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace