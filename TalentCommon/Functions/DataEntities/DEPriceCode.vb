<Serializable()> _
Public Class DEPriceCode
    Public Property PriceCode() As String = String.Empty
    Public Property ShortDescription() As String = String.Empty
    Public Property LongDescription() As String = String.Empty
    Public Property FreeOfCharge() As Boolean = False
    
    Public Sub New(ByVal pCode As String, shortDesc As String, ByVal longDesc As String, ByVal foc As Boolean)
        PriceCode = pCode
        ShortDescription = shortDesc
        LongDescription = longDesc
        FreeOfCharge = foc
    End Sub
End Class
