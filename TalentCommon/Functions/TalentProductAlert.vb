Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Alerts
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAPA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentProductAlert
    '
    Private _dep As DEProductAlert
    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Public Property Dep() As DEProductAlert
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProductAlert)
            _dep = value
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
    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property
    Public Function ProductAlert() As ErrorObj
        Dim err As New ErrorObj
        Dim pa As New DBProductAlert
        '-------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then err = .ProductAlerts
            Dep = .Dep
        End With
        Return err
    End Function
    Public Function ProductAlertOutbound() As ErrorObj
        Dim err As New ErrorObj
        Dim pa As New DBProductAlert
        '-------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Settings = Settings
            err = .ProductAlertsOutbound
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function

End Class
