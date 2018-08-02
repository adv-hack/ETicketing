Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common

<Serializable()> _
Public Class TalentNoise
    Inherits TalentBase

#Region "Class Level Fields"

    Private _deNoise As DENoise
    Private _resultDataSet As DataSet
    Private _SuccessfullCall As Boolean
    Private _rowsAffected As Integer
    Private _usersOnLine As Integer
    Private _SqlConnectionStrings As New Generic.List(Of String)

#End Region

#Region "Public Properties"

    Public Property De_Noise() As DENoise
        Get
            Return _deNoise
        End Get
        Set(ByVal value As DENoise)
            _deNoise = value
        End Set
    End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Property SuccessfullCall() As Boolean
        Get
            Return _SuccessfullCall
        End Get
        Set(ByVal value As Boolean)
            _SuccessfullCall = value
        End Set
    End Property
    Public Property RowsAffected() As Integer
        Get
            Return _rowsAffected
        End Get
        Set(ByVal value As Integer)
            _rowsAffected = value
        End Set
    End Property
    Public Property UsersOnLine() As Integer
        Get
            Return _usersOnLine
        End Get
        Set(ByVal value As Integer)
            _usersOnLine = value
        End Set
    End Property
    Public Property MultipleSQLConnectionStrings() As Generic.List(Of String)
        Get
            Return _SqlConnectionStrings
        End Get
        Set(ByVal value As Generic.List(Of String))
            _SqlConnectionStrings = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New(ByVal settings As DESettings, _
                        ByVal sessionId As String, _
                        ByVal lastActivity As DateTime, _
                        ByVal lastActivityThreshold As DateTime, _
                        Optional ByVal maxSessionKeepAliveMins As Integer = 0, _
                        Optional ByVal isAgent As Boolean = False, _
                        Optional ByVal usage As TblActiveNoiseSessions_Usage = TblActiveNoiseSessions_Usage.TICKETING, _
                        Optional ByVal agentType As Integer = 0, _
                        Optional ByVal agentName As String = "")

        MyBase.New()

        _resultDataSet = New DataSet

        _deNoise = New DENoise
        _deNoise.SessionID = sessionId
        _deNoise.LastActivity = lastActivity
        _deNoise.LastActivityThreshold = lastActivityThreshold
        _deNoise.MaxSessionKeepAliveMinutes = maxSessionKeepAliveMins
        _deNoise.IsAgent = isAgent
        _deNoise.AgentName = agentName
        _deNoise.AgentType = agentType
        _deNoise.Usage = usage

        Me.Settings = settings

    End Sub

#End Region

#Region "Public Functions"

    Public Function AddOrUpdateNoiseSession(Optional ByVal MaxConcurrentUsers As Integer = -1) As ErrorObj
        Const ModuleName As String = "AddOrUpdateNoiseSession"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.SessionID) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            De_Noise.MaxConcurrentUsers = MaxConcurrentUsers
            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.RowsAffected = .RowsAffected
            End With
        End If

        Return err
    End Function

    Public Function GetNoiseSessionBySessionID() As ErrorObj
        Const ModuleName As String = "GetNoiseSessionBySessionID"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.SessionID) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.ResultDataSet = .Results
            End With
        End If

        Return err
    End Function
    Public Function UpdateNoiseSessionBU(Optional ByVal MaxConcurrentUsers As Integer = -1) As ErrorObj
        Const ModuleName As String = "UpdateNoiseSessionBU"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.SessionID) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            De_Noise.MaxConcurrentUsers = MaxConcurrentUsers
            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.RowsAffected = .RowsAffected
            End With
        End If

        Return err
    End Function

    Public Function CheckAndUpdateNoiseSession() As ErrorObj
        Const ModuleName As String = "CheckAndUpdateNoiseSession"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.SessionID) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.RowsAffected = .RowsAffected
            End With
        End If

        Return err
    End Function

    Public Function CheckAndUpdateAgentNoiseSession() As ErrorObj
        Const ModuleName As String = "CheckAndUpdateAgentNoiseSession"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.SessionID) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.RowsAffected = .RowsAffected
            End With
        End If

        Return err
    End Function

    Public Function RemoveExpiredNoiseSessions() As ErrorObj
        Const ModuleName As String = "RemoveExpiredNoiseSessions"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.LastActivity.ToString) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.RowsAffected = .RowsAffected
                Me.UsersOnLine = .UsersOnLine
            End With
        End If

        Return err
    End Function

    Public Function RemoveSpecificNoiseSession() As ErrorObj
        Const ModuleName As String = "RemoveSpecificNoiseSession"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.LastActivity.ToString) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Dim db_noise As New DBNoise(De_Noise, ModuleName)
            With db_noise
                .Settings = Settings
                err = .AccessDatabase
                Me.SuccessfullCall = Not err.HasError
                Me.RowsAffected = .RowsAffected
                Me.UsersOnLine = .UsersOnLine
            End With
        End If

        Return err
    End Function

    Public Function RemoveSpecificNoiseSessionByAgentName_MultiDBs() As ErrorObj
        Const ModuleName As String = "RemoveSpecificNoiseSessionByAgentName"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.LastActivity.ToString) Then
            If Me.MultipleSQLConnectionStrings.Count > 0 Then
                Dim db_noise As New DBNoise(De_Noise, ModuleName)
                For Each connStr As String In Me.MultipleSQLConnectionStrings
                    Settings.DatabaseType1 = "1"
                    Settings.DestinationDatabase = "SQL2005"
                    Settings.BackOfficeConnectionString = connStr
                    With db_noise
                        .Settings = Settings
                        err = .AccessDatabase
                        Me.SuccessfullCall = Not err.HasError
                        Me.RowsAffected += .RowsAffected
                    End With
                Next
            End If
        End If

        Return err
    End Function

    Public Function RemoveExpiredNoiseSessions_MultiDBs() As ErrorObj
        Const ModuleName As String = "RemoveExpiredNoiseSessions"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.LastActivity.ToString) Then
            If Me.MultipleSQLConnectionStrings.Count > 0 Then
                Dim db_noise As New DBNoise(De_Noise, ModuleName)
                For Each connStr As String In Me.MultipleSQLConnectionStrings
                    Settings.DatabaseType1 = "1"
                    Settings.DestinationDatabase = "SQL2005"
                    Settings.BackOfficeConnectionString = connStr
                    With db_noise
                        .Settings = Settings
                        err = .AccessDatabase
                        Me.SuccessfullCall = Not err.HasError
                        Me.RowsAffected += .RowsAffected
                        Me.UsersOnLine += .UsersOnLine
                    End With
                Next
            End If
        End If

        Return err
    End Function

    Public Function CheckForExistingAgentNoiseSession_MultiDBs() As ErrorObj
        Const ModuleName As String = "CheckForExistingAgentNoiseSession"
        Dim err As New ErrorObj

        If Not String.IsNullOrEmpty(_deNoise.LastActivity.ToString) Then
            If Me.MultipleSQLConnectionStrings.Count > 0 Then
                Dim db_noise As New DBNoise(De_Noise, ModuleName)
                For Each connStr As String In Me.MultipleSQLConnectionStrings
                    Settings.DatabaseType1 = "1"
                    Settings.DestinationDatabase = "SQL2005"
                    Settings.BackOfficeConnectionString = connStr
                    With db_noise
                        .Settings = Settings
                        err = .AccessDatabase
                        Me.SuccessfullCall = Not err.HasError
                        Me.RowsAffected += .RowsAffected
                        Me.UsersOnLine += .UsersOnLine
                    End With
                Next
            End If
        End If

        Return err
    End Function

#End Region

End Class