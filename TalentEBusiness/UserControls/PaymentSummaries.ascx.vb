
Partial Class UserControls_PaymentSummaries
    Inherits ControlBase

    Private _displayTicketingSummary As Boolean = True
    Public Property DisplayTicketingSummary() As Boolean
        Get
            Return _displayTicketingSummary
        End Get
        Set(ByVal value As Boolean)
            _displayTicketingSummary = value
        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        'We do not want to show the ticketing summary on certain payment screens
        If Not DisplayTicketingSummary Then
            Me.BasketSummary1.Visible = False
        End If

        Select Case Profile.Basket.BasketContentType
            Case "C"
                Me.Payment_SummaryTotals1.Visible = False
                Me.BasketSummary1.Visible = False

            Case "M"
                Me.BasketSummary1.Visible = False

            Case "T"
                'Me.Payment_CombinedOverallTotal1.Visible = False
                Me.Payment_SummaryTotals1.Visible = False

        End Select
    End Sub
End Class
