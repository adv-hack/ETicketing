Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common

<Serializable()> _
Public Class TalentPPS
    Inherits TalentBase

    Private _resultDataSet As DataSet
    Private _dePPS As DEPPS
    Private _dePPSEnrolmentScheme As DEPPSEnrolmentScheme

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Public Property DEPPS() As DEPPS
        Get
            Return _dePPS
        End Get
        Set(ByVal value As DEPPS)
            _dePPS = value
        End Set
    End Property

    Public Property DEPPSEnrolmentScheme() As DEPPSEnrolmentScheme
        Get
            Return _dePPSEnrolmentScheme
        End Get
        Set(ByVal value As DEPPSEnrolmentScheme)
            _dePPSEnrolmentScheme = value
        End Set
    End Property

    Public Function AddPPSRequest() As ErrorObj
        Const ModuleName As String = "AddPPSRequest"
        Dim err As New ErrorObj
        Dim dbPPS As New DBPPS
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With dbPPS
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .dePPS = DEPPS
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function

    Public Function AmendPPS() As ErrorObj
        Const ModuleName As String = "AmendPPS"
        Dim err As New ErrorObj
        Dim dbPPS As New DBPPS
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With dbPPS
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .dePPSEnrolmentScheme = DEPPSEnrolmentScheme
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function
    Public Function CancelPPSEnrollment() As ErrorObj
        Const ModuleName As String = "CancelPPSEnrollment"
        Dim err As New ErrorObj
        Dim dbPPS As New DBPPS
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With dbPPS
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .dePPSEnrolmentScheme = DEPPSEnrolmentScheme
            .dePPS = DEPPS
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function


End Class
