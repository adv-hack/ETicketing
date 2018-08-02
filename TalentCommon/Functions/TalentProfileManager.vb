Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Profile Manager Requests
'
'       Date                        24/11/08
'
'       Author                      Ben Ford
'
'       CS Group 2007               All rights reserved.
'
'       Error Number Code base      TACPMR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentProfileManager

    Inherits TalentBase

    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property


    Private _profileManagerTransactions As DEProfileManagerTransactions
    Public Property ProfileManagerTransactions() As DEProfileManagerTransactions
        Get
            Return _profileManagerTransactions
        End Get
        Set(ByVal value As DEProfileManagerTransactions)
            _profileManagerTransactions = value
        End Set
    End Property

    Public Function ReceiveProfileManagerTransactions() As ErrorObj
        Const ModuleName As String = "ReceiveProfileManagerTransactions"

        Settings.ModuleName = ModuleName
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        ' No cache     
        Dim DbProfileManager As New dbprofilemanager
        With DbProfileManager
            .ProfileManagerTransactions = ProfileManagerTransactions
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            '
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
