Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Template Requests
'
'       Date                        Mar 2007
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTATAS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentTemplate
    Inherits TalentBase

    Private _resultDataSet As DataSet
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    '
    Private _dep As DEAmendTemplate
    Public Property Dep() As DEAmendTemplate
        Get
            Return _dep
        End Get
        Set(ByVal value As DEAmendTemplate)
            _dep = value
        End Set
    End Property
    Private _orderTemplate As DEOrderTemplate
    Public Property OrderTemplate() As DEOrderTemplate
        Get
            Return _orderTemplate
        End Get
        Set(ByVal value As DEOrderTemplate)
            _orderTemplate = value
        End Set
    End Property

    Private _DEuploadTemplates As DEUploadTemplates
    Public Property DEUploadTemplates() As DEUploadTemplates
        Get
            Return _DEuploadTemplates
        End Get
        Set(ByVal value As DEUploadTemplates)
            _DEuploadTemplates = value
        End Set
    End Property

    Public Function AddNewTemplate() As ErrorObj
        Const ModuleName As String = "AddNewTemplate"
        Me.GetConnectionDetails(Me.Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim pa As New DBAmendTemplate
        '-------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
                Dep = .Dep
            End If
        End With
        Return err
    End Function
    Public Function AddToTemplate() As ErrorObj
        Const ModuleName As String = "AddToTemplate"
        Me.GetConnectionDetails(Me.Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim pa As New DBAmendTemplate
        '-------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
                Dep = .Dep
            End If
        End With
        Return err
    End Function
    Public Function DeleteFromTemplate() As ErrorObj
        Const ModuleName As String = "DeleteFromTemplate"
        Me.GetConnectionDetails(Me.Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim pa As New DBAmendTemplate
        '-------------------------------------------------------------------------------------
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
    Public Function ReplaceTemplate() As ErrorObj
        Const ModuleName As String = "ReplaceTemplate"
        Me.GetConnectionDetails(Me.Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim pa As New DBAmendTemplate
        '-------------------------------------------------------------------------------------
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
    Public Function UploadTemplates() As ErrorObj
        Const ModuleName As String = "UploadTemplates"
        Me.GetConnectionDetails(Me.Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim pa As New DBUploadTemplates
        '-------------------------------------------------------------------------------------
        '   No Cache possible due to way the process works
        '
        With pa
            .DEUploadTemplates = DEUploadTemplates
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase
            End If
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function

End Class
