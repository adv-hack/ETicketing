Imports Microsoft.VisualBasic

Namespace Talent.CustomControls
    Public Class Literal
        Inherits System.Web.UI.WebControls.Literal

        Public Property HTMLFileName As String = ""

        Public Sub New()
            Me.ViewStateMode = Web.UI.ViewStateMode.Disabled
        End Sub

    End Class

End Namespace
