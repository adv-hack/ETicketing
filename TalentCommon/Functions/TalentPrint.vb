Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Text
Imports Talent.Common.Utilities
''' <summary>
''' This class holds the functionality for handling print ticket
''' </summary>
<Serializable()> _
Public Class TalentPrint
    Inherits TalentBase

    Private _printEntity As DEPrint
    Private _resultDataSet As DataSet

    Public Property PrintEntity As DEPrint
        Get
            Return _printEntity
        End Get
        Set(ByVal value As DEPrint)
            _printEntity = value
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

    Public Function PrintTicketsByWeb() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "PrintTicketsByWeb"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim Print As New DBPrint
        With Print
            .Settings = Settings
            .PrintEntity = PrintEntity
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Prints collection of despatch items by calling WS633R
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DespatchTicketPrint() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "DespatchTicketPrint"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim Print As New DBPrint
        With Print
            .Settings = Settings
            .PrintEntity = PrintEntity
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function

End Class
