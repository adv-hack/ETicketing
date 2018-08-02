''' <summary>
''' Holds the agent related details
''' </summary>
<Serializable()> _
Public Class DEAgent

    Public Property AgentUsername() As String = String.Empty
    Public Property AgentPassword() As String = String.Empty
    Public Property AgentType() As String = String.Empty
    Public Property PrinterNameDefault() As String = String.Empty
    Public Property PrinterGrp() As String = String.Empty
    Public Property DftTKTPrtr() As String = String.Empty
    Public Property DftSCPrtr() As String = String.Empty
    Public Property Tkt1_Home() As String = String.Empty
    Public Property Tkt2_Event() As String = String.Empty
    Public Property Tkt3_Travel() As String = String.Empty
    Public Property Tkt4_STkt() As String = String.Empty
    Public Property Tkt5_Addr() As String = String.Empty
    Public Property PrintAddrYN() As String = String.Empty
    Public Property PrintRcptYN() As String = String.Empty
    Public Property Source() As String = String.Empty
    Public Property AgentCompany() As String = String.Empty
    Public Property ErrorCode() As String = String.Empty
    Public Property Department() As String = String.Empty
    Public Property BulkSalesMode() As Boolean = False
    Public Property SavedSearchLimit() As Integer
    Public Property SavedSearchMode() As String = String.Empty
    Public Property SavedSearchType() As String = String.Empty
    Public Property SavedSearchKeyword() As String = String.Empty
    Public Property SavedSearchCategory() As String = String.Empty
    Public Property SavedSearchLocation() As String = String.Empty
    Public Property SavedSearchDate() As String = String.Empty
    Public Property SavedSearchProductType() As String = String.Empty
    Public Property SavedSearchStadium() As String = String.Empty
    Public Property SavedSearchUniqueID As Integer
    Public Property SessionID() As String
    Public Property DeleteSessionRecord() As Boolean
    Public Property DefaultCaptureMethod As String = String.Empty
    Public Property PrintAlways() As Boolean = False
    Public Property AgentAuthorityGroupID() As Integer
    Public Property NewAgent() As String = String.Empty
    Public Property NewAgentPassword() As String = String.Empty
    Public Property NewAgentDescription() As String = String.Empty
    Public Property CorporateHospitalityMode() As Boolean = False

End Class