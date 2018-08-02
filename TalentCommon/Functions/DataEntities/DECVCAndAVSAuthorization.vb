<Serializable()> _
Public Class DECVCAndAVSAuthorization
    Public Property CVCEnabled As Boolean
    Public Property CVCMandatory As Boolean
    Public Property CVCAccept_NotProvided As Boolean
    Public Property CVCAccept_NotChecked As Boolean
    Public Property CVCAccept_Matched As Boolean
    Public Property CVCAccept_NotMatched As Boolean
    Public Property CVCAccept_PartialMatch As Boolean

    Public Property AVS_Addr_Enabled As Boolean
    Public Property AVSAccept_Addr_NotProvided As Boolean
    Public Property AVSAccept_Addr_NotChecked As Boolean
    Public Property AVSAccept_Addr_Matched As Boolean
    Public Property AVSAccept_Addr_NotMatched As Boolean
    Public Property AVSAccept_Addr_PartialMatch As Boolean

    Public Property AVS_PC_Enabled As Boolean
    Public Property AVSAccept_PC_NotProvided As Boolean
    Public Property AVSAccept_PC_NotChecked As Boolean
    Public Property AVSAccept_PC_Matched As Boolean
    Public Property AVSAccept_PC_NotMatched As Boolean
    Public Property AVSAccept_PC_PartialMatch As Boolean
End Class
