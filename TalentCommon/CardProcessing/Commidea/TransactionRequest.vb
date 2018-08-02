Imports System
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    TransactionRequest.vb
'
'       Date                        Apr 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      COMMTRQ- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       03/04/06    /000    Ben     Created. Imported from Orion De
'       19/04/06    /001    Ben     Add in AVS details
'
'--------------------------------------------------------------------------------------------------
Namespace CardProcessing.Commidea

    Public Class TransactionRequest

        Private _accountID As String = String.Empty
        Private _accountNumber As String = String.Empty
        Private _addressLine1 As String = String.Empty
        Private _allowPartialAvs As Boolean = False
        Private _cNP As Boolean = False
        Private _cSC As String = String.Empty
        Private _debug As Boolean = False
        Private _eCom As Boolean = False
        Private _expiryDate As String = String.Empty
        Private _gUID As String = String.Empty
        Private _Issue As String = String.Empty
        Private _MerchantData As String = String.Empty
        Private _pan As String = String.Empty
        Private _Reference As String = String.Empty
        Private _postCode As String = String.Empty
        Private _rejectAdAvs As Boolean = False
        Private _rejectPcAvs As Boolean = False
        Private _rejectCsc As Boolean = False
        Private _StartDate As String = String.Empty
        Private _TxnType As String = String.Empty
        Private _TxnValue As String = String.Empty

        Public Property AccountID() As String
            Get
                Return _AccountID
            End Get
            Set(ByVal Value As String)
                _AccountID = Value
            End Set
        End Property
        Public Property AccountNumber() As String
            Get
                Return _AccountNumber
            End Get
            Set(ByVal Value As String)
                _AccountNumber = Value
            End Set
        End Property
        Public Property AddressLine1() As String
            Get
                Return _addressLine1
            End Get
            Set(ByVal Value As String)
                _addressLine1 = Value
            End Set
        End Property
        Public Property AllowPartialAvs() As Boolean
            Get
                Return _allowPartialAvs
            End Get
            Set(ByVal Value As Boolean)
                _allowPartialAvs = Value
            End Set
        End Property
        Public Property CNP() As Boolean
            Get
                Return _CNP
            End Get
            Set(ByVal Value As Boolean)
                _CNP = Value
            End Set
        End Property
        Public Property CSC() As String
            Get
                Return _CSC
            End Get
            Set(ByVal Value As String)
                _CSC = Value
            End Set
        End Property
        Public Property Debug() As Boolean
            Get
                Return _debug
            End Get
            Set(ByVal Value As Boolean)
                _debug = Value
            End Set
        End Property
        Public Property ECom() As Boolean
            Get
                Return _ECom
            End Get
            Set(ByVal Value As Boolean)
                _ECom = Value
            End Set
        End Property
        Public Property ExpiryDate() As String
            Get
                Return _ExpiryDate
            End Get
            Set(ByVal Value As String)
                _ExpiryDate = Value
            End Set
        End Property
        Public Property GUID() As String
            Get
                Return _GUID
            End Get
            Set(ByVal Value As String)
                _GUID = Value
            End Set
        End Property
        Public Property Issue() As String
            Get
                Return _Issue
            End Get
            Set(ByVal Value As String)
                _Issue = Value
            End Set
        End Property
        Public Property MerchantData() As String
            Get
                Return _MerchantData
            End Get
            Set(ByVal Value As String)
                _MerchantData = Value
            End Set
        End Property
        Public Property Pan() As String
            Get
                Return _Pan
            End Get
            Set(ByVal Value As String)
                _Pan = Value
            End Set
        End Property
        Public Property PostCode() As String
            Get
                Return _postCode
            End Get
            Set(ByVal Value As String)
                _postCode = Value
            End Set
        End Property
        Public Property Reference() As String
            Get
                Return _Reference
            End Get
            Set(ByVal Value As String)
                _Reference = Value
            End Set
        End Property
        Public Property RejectAdAvs() As Boolean
            Get
                Return _rejectAdAvs
            End Get
            Set(ByVal Value As Boolean)
                _rejectAdAvs = Value
            End Set
        End Property
        Public Property RejectPcAvs() As Boolean
            Get
                Return _rejectPcAvs
            End Get
            Set(ByVal Value As Boolean)
                _rejectPcAvs = Value
            End Set
        End Property
        Public Property RejectCsc() As Boolean
            Get
                Return _rejectCsc
            End Get
            Set(ByVal Value As Boolean)
                _rejectCsc = Value
            End Set
        End Property
        Public Property StartDate() As String
            Get
                Return _StartDate
            End Get
            Set(ByVal Value As String)
                _StartDate = Value
            End Set
        End Property
        Public Property TxnType() As String
            Get
                Return _TxnType
            End Get
            Set(ByVal Value As String)
                _TxnType = Value
            End Set
        End Property
        Public Property TxnValue() As String
            Get
                Return _TxnValue
            End Get
            Set(ByVal Value As String)
                _TxnValue = Value
            End Set
        End Property

    End Class
End Namespace
