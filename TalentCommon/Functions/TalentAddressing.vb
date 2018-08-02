Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Addressing interfaces
'
'       Date                        January 2008
'
'       Author                       
'
'       � CS Group 2006             All rights reserved.
'
'       Error Number Code base      TACTAAD- 
'                                   application.code(3) + object code(4) + number(2)
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentAddressing
    Inherits TalentBase
    '
    Private _de As New DeAddress
    'Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Public Property De() As DeAddress
        Get
            Return _de
        End Get
        Set(ByVal value As DeAddress)
            _de = value
        End Set
    End Property
    'Public Property Settings() As DESettings
    '    Get
    '        Return _settings
    '    End Get
    '    Set(ByVal value As DESettings)
    '        _settings = value
    '    End Set
    'End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Function AddressList() As ErrorObj

        Const ModuleName As String = "AddressList"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        Dim dbAddressing As New DBAddressing
        With dbAddressing
            .Settings = Settings
            .De = De
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
    Public Function PrintAddressLabel() As ErrorObj
        Const ModuleName As String = "PrintAddressLabel"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        Dim dbAddressing As New DBAddressing
        With dbAddressing
            .Settings = Settings
            .De = De
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError AndAlso ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function


    Public Function AddressListTest() As ErrorObj

        Const ModuleName As String = "AddressList"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        Dim dbAddressing As New DBAddressing
        With dbAddressing
            .Settings = Settings
            .De = De
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabaseWS802RTest
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With

        Return err

    End Function

End Class
