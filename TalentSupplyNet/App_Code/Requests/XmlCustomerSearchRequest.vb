Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlCustomerSearchRequest
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
            Dim xmlAction As XmlCustomerSearchResponse = CType(xmlResp, XmlCustomerSearchResponse)
            Dim err As ErrorObj = Nothing
            Select Case MyBase.DocumentVersion
                Case Is = "1.0" : err = LoadXmlV1()
                Case Is = "1.1" : err = LoadXmlV11()
                Case Else
                    err = New ErrorObj
                    With err
                        .ErrorMessage = "Invalid document version " & MyBase.DocumentVersion
                        .ErrorStatus = "CustomerSearchRequest Error - Invalid Doc Version " & MyBase.DocumentVersion
                        .ErrorNumber = "TTPRQCSR-02"
                        .HasError = True
                    End With
            End Select

            Dim talSearch As New TalentSearch
            If err.HasError Then
                xmlResp.Err = err
            Else
                Settings.Cacheing = False
                Dim deCustomerSearch As New DECustomerSearch
                deCustomerSearch.Customer = Decust
                Decust.Source = "S"
                With talSearch
                    .CustomerSearch = deCustomerSearch
                    .Settings = Settings
                    err = .PerformCustomerSearch
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If
            End If

            xmlAction.ResultDataSet = talSearch.ResultDataSet
            xmlResp.SenderID = Settings.SenderID
            xmlResp.DocumentVersion = MyBase.DocumentVersion
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)
        End Function

        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dim Node1, Node2 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerSearchRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                        Case Is = "CustomerSearch"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "Forename"
                                            .ContactForename = Node2.InnerText
                                        Case Is = "Surname"
                                            .ContactSurname = Node2.InnerText
                                        Case Is = "AddressLine1"
                                            .AddressLine1 = Node2.InnerText
                                        Case Is = "AddressLine2"
                                            .AddressLine2 = Node2.InnerText
                                        Case Is = "AddressLine3"
                                            .AddressLine3 = Node2.InnerText
                                        Case Is = "AddressLine4"
                                            .AddressLine4 = Node2.InnerText
                                        Case Is = "Postcode"
                                            .PostCode = Node2.InnerText
                                        Case Is = "DOB"
                                            .DateBirth = Node2.InnerText
                                        Case Is = "Email"
                                            .EmailAddress = Node2.InnerText
                                        Case Is = "PhoneNumber"
                                            .MobileNumber = Node2.InnerText
                                    End Select
                                    ' Check dob format
                                    If .DateBirth <> String.Empty Then
                                        Try
                                            Dim testDate As Date = CDate(.DateBirth)
                                            Dim testDob As Integer = CInt(.DateBirth.Substring(0, 2)) + CInt(.DateBirth.Substring(3, 2)) + CInt(.DateBirth.Substring(6, 4))
                                        Catch ex As Exception
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = ModuleName & " Date of birth invalid. Format dd/mm/yyyy"
                                                .ErrorNumber = "TTPRQCSR-03"
                                                .HasError = True
                                            End With
                                        End Try
                                    End If

                                Next Node2
                            End With
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCSR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV11() As ErrorObj
            Const ModuleName As String = "LoadXmlV11"
            Dim err As New ErrorObj
            Dim Node1, Node2 As XmlNode
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//CustomerSearchRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                        Case Is = "CustomerSearch"
                            Decust = New DECustomer
                            With Decust
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "ResultsLimit"
                                            .ResultsLimit = Node2.InnerText
                                        Case Is = "Start"
                                            .Start = Node2.InnerText
                                        Case Is = "Length"
                                            .Length = Node2.InnerText
                                        Case Is = "SortOrder"
                                            .SortOrder = Node2.InnerText
                                        Case Is = "Draw"
                                            .Draw = Node2.InnerText


                                            '-----------------------------------------------------------
                                            '   Contact details
                                            '
                                        Case Is = "CustomerNumber"
                                            .CustomerNumber = Node2.InnerText
                                        Case Is = "Forename"
                                            .ContactForename = Node2.InnerText
                                        Case Is = "Surname"
                                            .ContactSurname = Node2.InnerText
                                        Case Is = "AddressLine1"
                                            .AddressLine1 = Node2.InnerText
                                        Case Is = "AddressLine2"
                                            .AddressLine2 = Node2.InnerText
                                        Case Is = "AddressLine3"
                                            .AddressLine3 = Node2.InnerText
                                        Case Is = "AddressLine4"
                                            .AddressLine4 = Node2.InnerText
                                        Case Is = "Postcode"
                                            .PostCode = Node2.InnerText
                                        Case Is = "DOB"
                                            .DateBirth = Node2.InnerText
                                        Case Is = "Email"
                                            .EmailAddress = Node2.InnerText
                                        Case Is = "PhoneNumber"
                                            .MobileNumber = Node2.InnerText
                                    End Select
                                    ' Check dob format
                                    If .DateBirth <> String.Empty Then
                                        Try
                                            Dim testDate As Date = CDate(.DateBirth)
                                            Dim testDob As Integer = CInt(.DateBirth.Substring(0, 2)) + CInt(.DateBirth.Substring(3, 2)) + CInt(.DateBirth.Substring(6, 4))
                                        Catch ex As Exception
                                            With err
                                                .ErrorMessage = ex.Message
                                                .ErrorStatus = ModuleName & " Date of birth invalid. Format dd/mm/yyyy"
                                                .ErrorNumber = "TTPRQCSR-03"
                                                .HasError = True
                                            End With
                                        End Try
                                    End If

                                Next Node2
                            End With
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQCSR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace