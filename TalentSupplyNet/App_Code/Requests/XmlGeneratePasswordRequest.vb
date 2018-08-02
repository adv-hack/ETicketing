Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Generate Password Requests
'
'       Date                        22/08/10
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQGP- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlGeneratePasswordRequest
        Inherits XmlRequest
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

            Dim xmlAction As XmlGeneratePasswordResponse = CType(xmlResp, XmlGeneratePasswordResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '-------------------------------------------
            ' Check whether need to hash password or not
            '-------------------------------------------
            Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
            Dim hashedPassword As String = String.Empty
            Dim encryptionType As String = String.Empty
            decust.UseEncryptedPassword = False
            decust.EncryptionType = String.Empty
            Try
                Decust.UseEncryptedPassword = CBool(def.GetDefault("HASH_PASSWORD"))
            Catch
            End Try
            Try
                Decust.EncryptionType = def.GetDefault("ENCRYPTION_TYPE")
            Catch
            End Try


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
                    err = .GeneratePassword
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            xmlAction.ResultDataSet = CUSTOMER.ResultDataSet
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
                For Each Node1 In xmlDoc.SelectSingleNode("//GeneratePasswordRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "GeneratePassword"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "UserName"
                                            If Node2.InnerText.Contains("@") AndAlso Node2.InnerText.Contains(".") Then
                                                .EmailAddress = Node2.InnerText
                                            Else
                                                .UserName = Node2.InnerText
                                            End If
                                        Case Is = "PasswordLength"
                                            .PasswordLength = 0
                                            If Node2.InnerText <> String.Empty Then
                                                Try
                                                    .PasswordLength = CInt(Node2.InnerText)
                                                Catch
                                                End Try
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
                    .ErrorNumber = "TTPRQGP-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace