''' <summary>
''' this data entity class hold data for tbl_alert_critera 
''' Related to Customer Alerts
''' </summary>
<Serializable()> _
Public Class DEPageAttribute
    Public Property ID() As Integer
    Public Property BusinessUnit() As String
    Public Property PartnerCode() As String
    Public Property PageCode() As String
    Public Property AttributeName() As String
    Public Property AttributeValue() As String
    Public Property Description() As String
End Class

Public Enum Operation
    Insert
    Update
End Enum
