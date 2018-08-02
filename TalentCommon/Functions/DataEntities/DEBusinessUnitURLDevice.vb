''' <summary>
''' This entity will hold the relation between url, business unit, queue url and device type
''' tbl_url_bu
''' </summary>
<Serializable()> _
Public Class DEBusinessUnitURLDevice
    Public Property BusinessUnit() As String = String.Empty
    Public Property BusinessUnitGroup() As String = String.Empty
    Public Property BusinessUnitContent() As String = String.Empty
    Public Property Application() As String = String.Empty
    Public Property DeviceType() As String = String.Empty
    Public Property QueueURL() As String = String.Empty
    Public Property URL() As String = String.Empty
    Public Property URLGroup() As String = String.Empty
End Class
