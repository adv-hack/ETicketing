''' <summary>
''' this data entity class hold data for tbl_alert_critera 
''' Related to Customer Alerts
''' </summary>
<Serializable()> _
Public Class DEPageTextLang
    Public Property ID() As Integer
    Public Property BusinessUnit() As String
    Public Property PartnerCode() As String
    Public Property PageCode() As String
    Public Property TextCode() As String
    Public Property LanguageCode() As String
    Public Property TextContent() As String
End Class
