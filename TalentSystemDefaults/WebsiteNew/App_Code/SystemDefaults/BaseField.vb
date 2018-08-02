Imports Microsoft.VisualBasic
Imports TalentSystemDefaults

Public MustInherit Class BaseField
    Inherits System.Web.UI.UserControl

    Protected config As ModuleConfiguration

    Protected MustOverride ReadOnly Property UpdatedValue() As String

    Public MustOverride Sub Update()

    Public Overridable Sub Initialise(ByRef config As ModuleConfiguration)
        Me.config = config
    End Sub


End Class
