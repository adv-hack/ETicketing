Imports Microsoft.VisualBasic

Public Class DataModuleClass
    Public Property ClassName As String
    Public Property ModuleTitle As String
    Public Property ShowAsModule As Boolean

    Public Sub New(ByVal className As String, ByVal moduleTitle As String, ByVal showAsModule As Boolean)
        Me.ClassName = className
        Me.ModuleTitle = moduleTitle
        Me.ShowAsModule = showAsModule
    End Sub
End Class
