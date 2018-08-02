Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with removing expired noise sessions
'
'       Date                        March 2008
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSRE- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRemoveExpiredNoiseSessionsResponse
        Inherits XmlResponse


        Private _NoiseCounters As Generic.Dictionary(Of String, Integer)
        Public Property NoiseCounters() As Generic.Dictionary(Of String, Integer)
            Get
                Return _NoiseCounters
            End Get
            Set(ByVal value As Generic.Dictionary(Of String, Integer))
                _NoiseCounters = value
            End Set
        End Property

        Private _usersOnLine As Integer
        Public Property UsersOnLine() As Integer
            Get
                Return _usersOnLine
            End Get
            Set(ByVal value As Integer)
                _usersOnLine = value
            End Set
        End Property

        Private _sessionsRemoved As Integer
        Public Property SessionsRemoved() As Integer
            Get
                Return _sessionsRemoved
            End Get
            Set(ByVal value As Integer)
                _sessionsRemoved = value
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




        Protected Overrides Sub InsertBodyV1()

            Try

                Dim nRemoveSessions, nSuccessCount, nFailureCount, nTotRemoved, nOnLine, nNoiseServers, _
                    nNoiseServer, nHeader, nHeaderRoot As XmlNode

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    nRemoveSessions = .CreateElement("RemoveExpiredNoiseSessions")
                    nTotRemoved = .CreateElement("TotalSessionsRemoved")
                    nOnLine = .CreateElement("RemainingUsersOnLine")
                    nSuccessCount = .CreateElement("SuccessCount")
                    nFailureCount = .CreateElement("FailureCount")
                    nNoiseServers = .CreateElement("NoiseServerResults")
                End With

                nSuccessCount.InnerText = Me.NoiseCounters.Keys.Count - Me.FailedNoiseUpdates.Count
                nFailureCount.InnerText = Me.FailedNoiseUpdates.Count
                nTotRemoved.InnerText = Me.SessionsRemoved
                nOnLine.InnerText = Me.UsersOnLine

                'Populate the xml document
                With nRemoveSessions
                    .AppendChild(nSuccessCount)
                    .AppendChild(nFailureCount)
                    .AppendChild(nTotRemoved)
                    .AppendChild(nOnLine)
                    .AppendChild(nNoiseServers)
                End With

                If Not Me.NoiseCounters Is Nothing Then
                    Dim xAt As XmlAttribute

                    For Each noiseServer As String In Me.NoiseCounters.Keys
                        nNoiseServer = MyBase.xmlDoc.CreateElement("NoiseServer")
                        xAt = MyBase.xmlDoc.CreateAttribute("HttpAddress")
                        xAt.Value = noiseServer
                        nNoiseServer.Attributes.Append(xAt)
                        xAt = MyBase.xmlDoc.CreateAttribute("Count")
                        xAt.Value = NoiseCounters(noiseServer)
                        nNoiseServer.Attributes.Append(xAt)
                        xAt = MyBase.xmlDoc.CreateAttribute("Success")
                        xAt.Value = (Not Me.FailedNoiseUpdates.Contains(noiseServer & "?count=" & NoiseCounters(noiseServer))).ToString
                        nNoiseServer.Attributes.Append(xAt)
                        nNoiseServers.AppendChild(nNoiseServer)
                    Next
                End If

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                nHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                nHeaderRoot = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                nHeader.InsertAfter(nRemoveSessions, nHeaderRoot)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRE-01"
                    .HasError = True
                End With
            End Try

        End Sub

       

    End Class

End Namespace


