Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Verify Password Requests
'
'       Date                        04/02/08
'
'       Author                           
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQVP- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlVerifyPasswordRequest
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

            Dim xmlAction As XmlVerifyPasswordResponse = CType(xmlResp, XmlVerifyPasswordResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()
                Case Is = "1.1"
                    err = LoadXmlV11()
            End Select
            '-------------------------------------------
            ' Check whether need to hash password or not
            '-------------------------------------------
            Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
            Dim hashedPassword As String = String.Empty
            Dim hashPassword As Boolean = False
            Dim saltString As String = ""
            Dim encryptionType As String = String.Empty
            Try
                hashPassword = CBool(def.GetDefault("USE_ENCRYPTED_PASSWORD"))
                saltString = def.GetDefault("CLIENT_SALT")
            Catch
            End Try
            Decust.UseEncryptedPassword = hashPassword
            If Decust.UseEncryptedPassword Then
                Dim passHash As New PasswordHash
                Decust.HashedPassword = passHash.HashSalt(Decust.Password, saltString)
            End If
            ' Moved to DBCustomer
            'If hashPassword Then
            '    Decust.HashedPassword = Utilities.aMD5Hash(Decust.Password)
            'End If

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
                    err = .VerifyPassword
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
                For Each Node1 In xmlDoc.SelectSingleNode("//VerifyPasswordRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "VerifyPassword"
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
                                        Case Is = "Password"
                                            .Password = Node2.InnerText
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQVP-01"
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
                For Each Node1 In xmlDoc.SelectSingleNode("//VerifyPasswordRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"

                        Case Is = "VerifyPassword"
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
                                        Case Is = "Password"
                                            .Password = Node2.InnerText
                                    End Select
                                Next Node2
                            End With
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQVP-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
    End Class

End Namespace