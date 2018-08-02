<Serializable()>
Public Class TalentTemplateOverride
    Inherits TalentBase

    Public Property DeTemplate() As New DETemplate
    Public Property ResultDataSet() As DataSet
    Private Property err As New ErrorObj

    Const MODULE_GET_TEMPLATE_OVERRIDE_LIST As String = "GetTemplateOverrideList"
    Const MODULE_CREATE_TEMPLATE_OVERRIDE As String = "CreateTemplateOverride"
    Const MODULE_GET_TEMPLATE_OVERRIDE_CRITERIAS As String = "GetTemplateOverrideCriterias"
    Const MODULE_UPDATE_TEMPLATE_OVERRIDE As String = "UpdateTemplateOverride"
    Const MODULE_DELETE_TEMPLATE_OVERRIDE As String = "DeleteTemplateOverride"

    ''' <summary>
    ''' Retrieve template override list for businessunit.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetTemplateOverrideList() As ErrorObj
        Settings.ModuleName = MODULE_GET_TEMPLATE_OVERRIDE_LIST
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbTemplate As New DBTemplate
        With dbTemplate
            .DeTemp = DeTemplate
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' create template override for businessunit.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateTemplateOverride() As ErrorObj
        Settings.ModuleName = MODULE_CREATE_TEMPLATE_OVERRIDE
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbTemplate As New DBTemplate
        With dbTemplate
            .DeTemp = DeTemplate
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' update template override for businessunit.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function UpdateTemplateOverride() As ErrorObj
        Settings.ModuleName = MODULE_UPDATE_TEMPLATE_OVERRIDE
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbTemplate As New DBTemplate
        With dbTemplate
            .DeTemp = DeTemplate
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' This method deletes the template for a specified business unit, and template id
    ''' </summary>
    ''' <remarks></remarks>
    Public Function DeleteTemplateOverride() As ErrorObj
        Settings.ModuleName = MODULE_DELETE_TEMPLATE_OVERRIDE
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbTemplate As New DBTemplate
        With dbTemplate
            .DeTemp = DeTemplate
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' get template override criterias.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetTemplateOverrideCriterias() As ErrorObj
        Settings.ModuleName = MODULE_GET_TEMPLATE_OVERRIDE_CRITERIAS
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbTemplate As New DBTemplate
        With dbTemplate
            .DeTemp = DeTemplate
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
End Class
