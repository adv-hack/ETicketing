Imports Microsoft.VisualBasic

Namespace Talent.eCommerce

    Public Class PaymentSelectionChangedEventArgs : Inherits EventArgs
        Public Property NewSelection() As String
        Public Sub New(ByVal PaymentOption As String)
            NewSelection = PaymentOption
        End Sub
    End Class
    Public Delegate Sub PaymentSelectionChangedEventHandler(ByVal sender As Object, ByVal e As PaymentSelectionChangedEventArgs)


    Public Delegate Sub PaymentOptionsRestrictedByBasketMixEventHandler(ByVal sender As Object, ByVal e As EventArgs)

End Namespace