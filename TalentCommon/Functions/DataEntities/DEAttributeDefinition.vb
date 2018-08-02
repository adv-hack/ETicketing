''' <summary>
''' this data entity class hold data for tbl_attribute_definition 
''' Related to Customer Alerts
''' <para>This class is used as object mapper any change in the property has to be take care of this</para>
''' </summary>
<Serializable()> _
Public Class DEAttributeDefinition
    Public Property NAME() As String
    Public Property DESCRIPTION() As String
    Public Property CATEGORY() As String
    Public Property TYPE() As String
    Public Property FOREIGN_KEY() As String
End Class
