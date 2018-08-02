Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Remittance Requests
'
'       Date                        Apr 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAPRA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentRemittance

    Private _der As New DeRemittances
    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Public Property Der() As DeRemittances
        Get
            Return _der
        End Get
        Set(ByVal value As DeRemittances)
            _der = value
        End Set
    End Property
    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
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
    Public Function CustRemittances() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim DBCustRemittances As New DBCustRemittances
        With DBCustRemittances
            .Der = Der
            .Settings = Settings
            err = .AccessDatabase
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    Public Function SuppRemittances() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim DBSuppRemittances As New DBSuppRemittances
        With DBSuppRemittances
            .Der = Der
            .Settings = Settings
            err = .AccessDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

End Class
