''' <summary>
''' this data entity class hold data for tbl_alert_critera 
''' Related to Customer Alerts
''' <para>This class is used as object mapper any change in the property has to be take care of this</para>
''' </summary>
<Serializable()> _
Public Class DEAlertCritera

    Public Property ALERT_ID() As Long
    Public Property ATTR_ID() As String
    Public Property ALERT_OPERATOR() As String
    Public Property SEQUENCE() As Long
    Public Property CLAUSE() As Long
    Public Property CLAUSE_TYPE() As String

End Class
