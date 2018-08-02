Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Error objects
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'  
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class ErrorObj

    Private _hasError As Boolean = False            ' true false flag

    Private _errorMessage As String = String.Empty  ' System generated error message
    Private _errorNumber As String = String.Empty   ' program generated error number
    Private _errorStatus As String = String.Empty   ' program generated error code

    Private _itemErrorCode(500) As String           ' Error code for multiple items (e.g orders) per transaction
    Private _itemErrorMessage(500) As String        ' Error message for multiple items (e.g orders) per transaction
    Private _itemErrorStatus(500) As String         ' Error status for multiple items (e.g orders) per transaction

    Public Property HasError() As Boolean
        Get
            Return _hasError
        End Get
        Set(ByVal value As Boolean)
            _hasError = value
        End Set
    End Property

    Public Property ErrorMessage() As String
        Get
            Return _errorMessage
        End Get
        Set(ByVal value As String)
            _errorMessage = value
        End Set
    End Property
    Public Property ErrorNumber() As String
        Get
            Return _errorNumber
        End Get
        Set(ByVal value As String)
            _errorNumber = value
        End Set
    End Property
    Public Property ErrorStatus() As String
        Get
            Return _errorStatus
        End Get
        Set(ByVal value As String)
            _errorStatus = value
        End Set
    End Property

    Public Property ItemErrorCode(ByVal item As Integer) As String
        Get
            Return _itemErrorCode(item)
        End Get
        Set(ByVal value As String)
            _itemErrorCode(item) = value
        End Set
    End Property
    Public Property ItemErrorMessage(ByVal item As Integer) As String
        Get
            Return _itemErrorMessage(item)
        End Get
        Set(ByVal value As String)
            _itemErrorMessage(item) = value
        End Set
    End Property
    Public Property ItemErrorStatus(ByVal item As Integer) As String
        Get
            Return _itemErrorStatus(item)
        End Get
        Set(ByVal value As String)
            _itemErrorStatus(item) = value
        End Set
    End Property

End Class
