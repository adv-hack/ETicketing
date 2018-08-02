'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for displaying CRM Notes
'
'       Date                        19/07/07
'
'       Author                      Ben Ford
'
'       � CS Group 2006             All rights reserved.
'
'       Error Number Code base      TACDECN- 
'                                   
'--------------------------------------------------------------------------------------------------
'
<Serializable()> _
Public Class DECrmNote

    '---------------------------------------------------------------------------------
    '   General Fields
    '
    Private _noteId As Int64 = 0
    Private _customerID As String = String.Empty
    Private _contactID As String = String.Empty
    Private _customerNumber As String = String.Empty
    Private _branchCode As String = String.Empty
    Private _action As String = String.Empty
    Private _manualDeDupe As String = String.Empty
    Private _manualDeDupeBranch As String = String.Empty
    Private _thirdPartyContactRef As String = String.Empty
    Private _dateFormat As String = String.Empty
    Private _thirdPartyCompanyRef1 As String = String.Empty
    Private _thirdPartyCompanyRef2 As String = String.Empty

    Private _deCrmNoteDetail As DECrmNote11Detail = Nothing
    Public Property NoteID() As Int64
        Get
            Return _noteId
        End Get
        Set(ByVal value As Int64)
            _noteId = value
        End Set
    End Property
    Public Property CustomerID() As String
        Get
            Return _customerID
        End Get
        Set(ByVal value As String)
            _customerID = value
        End Set
    End Property
    Public Property ContactID() As String
        Get
            Return _contactID
        End Get
        Set(ByVal value As String)
            _contactID = value
        End Set
    End Property
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property
    Public Property BranchCode() As String
        Get
            Return _branchCode
        End Get
        Set(ByVal value As String)
            _branchCode = value
        End Set
    End Property
    Public Property Action() As String
        Get
            Return _action
        End Get
        Set(ByVal value As String)
            _action = value
        End Set
    End Property
    Public Property ManualDeDupe() As String
        Get
            Return _manualDeDupe
        End Get
        Set(ByVal value As String)
            _manualDeDupe = value
        End Set
    End Property
    Public Property ManualDeDupeBranch() As String
        Get
            Return _manualDeDupeBranch
        End Get
        Set(ByVal value As String)
            _manualDeDupeBranch = value
        End Set
    End Property
    Public Property ThirdPartyContactRef() As String
        Get
            Return _thirdPartyContactRef
        End Get
        Set(ByVal value As String)
            _thirdPartyContactRef = value
        End Set
    End Property
    Public Property DateFormat() As String
        Get
            Return _dateFormat
        End Get
        Set(ByVal value As String)
            _dateFormat = value
        End Set
    End Property
    Public Property ThirdPartyCompanyRef1() As String
        Get
            Return _thirdPartyCompanyRef1
        End Get
        Set(ByVal value As String)
            _thirdPartyCompanyRef1 = value
        End Set
    End Property
    Public Property ThirdPartyCompanyRef2() As String
        Get
            Return _thirdPartyCompanyRef2
        End Get
        Set(ByVal value As String)
            _thirdPartyCompanyRef2 = value
        End Set
    End Property
End Class

Public Class DECrmNoteHeader

End Class
Public Class DECrmNote11Detail

End Class
