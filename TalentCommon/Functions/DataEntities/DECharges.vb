'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Charges
'
'       Date                        28th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       When passed to System21 must restrict _commentText to max length of 1024
'       so if greater multiple calls will be needed.
'
'       Error Number Code base      TACDECH- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DECharges
    '---------------------------------------------------------------------------------
    '    
    '
    Private _charge As String = String.Empty             ' 11
    Private _suffix As String = String.Empty             '
    Private _value As String = String.Empty              ' 11

    Public Property Charge() As String
        Get
            Return _charge
        End Get
        Set(ByVal value As String)
            _charge = value
        End Set
    End Property                  ' 
    Public Property Suffix() As String
        Get
            Return _suffix
        End Get
        Set(ByVal value As String)
            _suffix = value
        End Set
    End Property
    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

End Class
