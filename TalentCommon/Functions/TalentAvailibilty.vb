Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Error Number Code base      TACTAPA- 
'                                    
'       Modification Summary
'
'
'       Function                    This class is used to deal with Product Availibilties
'
'       Date                        Nov 2006
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       01/04/07            ADW     Added MultipleAvailability
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentAvailibilty
    'Stuart20070529
    Inherits TalentBase
    'Stuart20070529
    '
    Private _dep As DEProductAlert
    Private _resultDataSet As DataSet
    Private _dePNARequest As New DEPNARequest
    'Stuart20070529
    'Now held in base class
    'Private _settings As New DESettings
    'Stuart20070529
    Public Property Dep() As DEProductAlert
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProductAlert)
            _dep = value
        End Set
    End Property
    Public Property DePNARequest() As DEPNARequest
        Get
            Return _dePNARequest
        End Get
        Set(ByVal value As DEPNARequest)
            _dePNARequest = value
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
    'Stuart20070529
    'Now held in base class
    'Public Property Settings() As DESettings
    '    Get
    '        Return _settings
    '    End Get
    '    Set(ByVal value As DESettings)
    '        _settings = value
    '    End Set
    'End Property
    'Stuart20070529
    Public Function Availability() As ErrorObj

        'Stuart20070529
        'Example of reading a module specific setting
        Dim ss As DEStockSettings
        ss = CType(Settings(), DEStockSettings)
        Dim testString As String = ss.TestProperty
        'Stuart20070529

        Dim err As New ErrorObj
        Dim pa As New DBAvailability
        '------------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            Dep = .Dep
        End With
        Return err
    End Function
    Public Function MultipleAvailability() As ErrorObj
        Dim err As New ErrorObj
        Dim pa As New DBMultipleAvailability
        '-----------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            Dep = .Dep
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function
    Public Function CompleteAvailability() As ErrorObj
        Dim err As New ErrorObj
        Dim pa As New DBCompleteAvailability
        '-----------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            Dep = .Dep
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function

End Class
