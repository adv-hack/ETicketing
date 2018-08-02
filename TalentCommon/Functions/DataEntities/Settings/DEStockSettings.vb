Public Class DEStockSettings
    Inherits DESettings

    Private _testProperty As String
    Public Property TestProperty() As String
        Get
            Return _testProperty
        End Get
        Set(ByVal value As String)
            _testProperty = value
        End Set
    End Property

End Class
