''' <summary>
''' this data entity class hold data for tbl_feeds_template, tbl_feeds_text_lang 
''' Related to RSS / Atom Feeds
''' </summary>
<Serializable()> _
Public Class DEFeeds
    Public Property Feed_Type() As String = String.Empty
    Public Property Product_Type() As String = String.Empty
    Public Property Product_Sub_Type() As String = String.Empty
    Public Property Corporate_Stadium() As String = String.Empty
    Public Property Ticketing_Stadium() As String = String.Empty
    Public Property Site_Url() As String = String.Empty
    Public Property Product_Type_All_Filters() As String = String.Empty
    Public Property Online_Products_Only As Boolean = True
End Class
