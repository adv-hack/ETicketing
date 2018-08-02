Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common


Namespace Talent.TradingPortal
    Public Class XmlRemoveExpiredNoiseSessionsRequest
        Inherits XmlRequest

        Dim TimeoutThreshold As Integer = 0
        Dim HTTPAddresses As New Generic.List(Of String)
        Dim SQLConnectionStrings As New Generic.List(Of String)

        Private _NoiseCounters As Generic.Dictionary(Of String, Integer)
        Public Property NoiseCounters() As Generic.Dictionary(Of String, Integer)
            Get
                Return _NoiseCounters
            End Get
            Set(ByVal value As Generic.Dictionary(Of String, Integer))
                _NoiseCounters = value
            End Set
        End Property

        Private _failedNoiseUpdates As Generic.List(Of String)
        Public Property FailedNoiseUpdates() As Generic.List(Of String)
            Get
                Return _failedNoiseUpdates
            End Get
            Set(ByVal value As Generic.List(Of String))
                _failedNoiseUpdates = value
            End Set
        End Property




        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Dim xmlAction As XmlRemoveExpiredNoiseSessionsResponse = CType(xmlResp, XmlRemoveExpiredNoiseSessionsResponse)
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            Dim mySettings As New DESettings
            mySettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            mySettings.DestinationDatabase = "SQL2005"

            Dim Noise As New TalentNoise(mySettings, "", Now, Now.AddMinutes(-TimeoutThreshold))

            If err.HasError Then
                xmlResp.Err = err
            Else
                With Noise
                    .Settings = Settings
                    If Me.SQLConnectionStrings.Count > 0 Then
                        .MultipleSQLConnectionStrings = Me.SQLConnectionStrings
                        err = .RemoveExpiredNoiseSessions_MultiDBs
                    Else
                        err = .RemoveExpiredNoiseSessions
                    End If
                End With
                If err.HasError Or Not err Is Nothing Then
                    xmlResp.Err = err
                End If

            End If

            If Not err.HasError Then
                err = ResetNoiseServerCount(Noise.UsersOnLine)
            End If

            If Not err.HasError Then
                CType(xmlResp, XmlRemoveExpiredNoiseSessionsResponse).UsersOnLine = Noise.UsersOnLine
                CType(xmlResp, XmlRemoveExpiredNoiseSessionsResponse).SessionsRemoved = Noise.RowsAffected
                CType(xmlResp, XmlRemoveExpiredNoiseSessionsResponse).NoiseCounters = Me.NoiseCounters
                CType(xmlResp, XmlRemoveExpiredNoiseSessionsResponse).FailedNoiseUpdates = Me.FailedNoiseUpdates
            End If

            xmlResp.SenderID = Settings.SenderID
            xmlResp.CreateResponse()
            Return CType(xmlAction, XmlResponse)

        End Function


        Private Function ResetNoiseServerCount(ByVal usersOnLine As Integer) As ErrorObj
            Dim err As New ErrorObj
            Try
                NoiseCounters = New Generic.Dictionary(Of String, Integer)
                FailedNoiseUpdates = New Generic.List(Of String)
                Dim noiseSplit As Integer = Utilities.RoundToValue(usersOnLine / HTTPAddresses.Count, 1, False)

                Dim lastServerVal As Integer = usersOnLine - (noiseSplit * (HTTPAddresses.Count - 1))

                For i As Integer = 0 To HTTPAddresses.Count - 1
                    If Not String.IsNullOrEmpty(HTTPAddresses(i)) Then
                        If Not i = HTTPAddresses.Count - 1 Then
                            NoiseCounters.Add(HTTPAddresses(i), noiseSplit)
                            HTTPAddresses(i) += "?count=" & noiseSplit
                        Else
                            NoiseCounters.Add(HTTPAddresses(i), lastServerVal)
                            HTTPAddresses(i) += "?count=" & lastServerVal
                        End If
                    End If
                Next

                Dim req As System.Net.WebRequest
                Dim resp As System.Net.WebResponse
                For Each url As String In HTTPAddresses
                    Try
                        req = System.Net.WebRequest.Create(url)
                        'req.Method = "POST"
                        resp = req.GetResponse()
                    Catch ex As Exception
                        FailedNoiseUpdates.Add(url)
                    End Try
                Next
            Catch ex As Exception
                err.HasError = True
                err.ErrorMessage = ex.Message
                err.ErrorNumber = "TTPRENS-02"
            End Try



            Return err
        End Function


        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities
            '
            Try
                For Each masterNode As XmlNode In xmlDoc.SelectSingleNode("//RemoveExpiredNoiseSessionsRequest").ChildNodes
                    Select Case masterNode.Name
                        Case Is = "TransactionHeader"

                        Case Is = "RemoveExpiredNoiseSessions"

                            'loop through the sub nodes
                            For Each subNode As XmlNode In masterNode.ChildNodes

                                Select Case subNode.Name
                                    Case Is = "TimeoutThreshold"
                                        'attempt to set the timeout threshold
                                        Try
                                            TimeoutThreshold = CInt(subNode.InnerText)
                                        Catch ex As Exception
                                            TimeoutThreshold = 20
                                        End Try

                                    Case Is = "NoiseServerURLs"
                                        'loop through and add the ip addresses
                                        For Each ipNode As XmlNode In subNode.ChildNodes
                                            Select Case ipNode.Name
                                                Case Is = "HTTPAddress"
                                                    If Not HTTPAddresses.Contains(ipNode.InnerText) Then
                                                        HTTPAddresses.Add(ipNode.InnerText)
                                                    End If
                                            End Select
                                        Next

                                    Case Is = "SQLConnectionStrings"
                                        'loop through and add the ip addresses
                                        For Each ipNode As XmlNode In subNode.ChildNodes
                                            Select Case ipNode.Name
                                                Case Is = "ConnectionString"
                                                    If Not SQLConnectionStrings.Contains(ipNode.InnerText) Then
                                                        SQLConnectionStrings.Add(ipNode.InnerText)
                                                    End If
                                            End Select
                                        Next
                                End Select
                            Next


                    End Select
                Next

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRENS-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace


