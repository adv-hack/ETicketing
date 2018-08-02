''' <summary>
''' this data entity class hold data for tbl_alert_critera 
''' Related to Customer Alerts
''' </summary>
<Serializable()> _
Public Class DEAlertDefinition
    Public Property BusinessUnit() As String
    Public Property Partner() As String
    Public Property AlertID() As Long
    Public Property Name() As String
    Public Property Description() As String
    Public Property Subject() As String
    Public Property ImagePath() As String
    Public Property Action() As String
    Public Property ActionDetail() As String
    Public Property ActionDetailURLOption() As Boolean
    Public Property ActivationStartDate() As String
    Public Property ActivationEndDate() As String
    Public Property Enabled() As Boolean
    Public Property NonStandard() As Boolean
    Public Property PageCode() As String
    Public Property BCTURL() As String
    Public Property ActionType() As Integer
    Public Property URL() As String
    Public Property AlertType() As String
End Class
