'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Payments
'
'       Date                        
'
'       Author                      
'
'       � CS Group 2007             All rights reserved.
'
'       
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEPaymentExternalDetails
    '---------------------------------------------------------------------------------
    '
    Private _extPaymentReference As String = String.Empty
    Private _extPaymentCompany As String = String.Empty
    Private _extPaymentName As String = String.Empty
    Private _extPaymentAddress1 As String = String.Empty
    Private _extPaymentAddress2 As String = String.Empty
    Private _extPaymentAddress3 As String = String.Empty
    Private _extPaymentAddress4 As String = String.Empty
    Private _extPaymentCountry As String = String.Empty
    Private _extPaymentPostCode As String = String.Empty
    Private _extPaymentTel1 As String = String.Empty
    Private _extPaymentTel2 As String = String.Empty
    Private _extPaymentTel3 As String = String.Empty
    Private _extPaymentEmail As String = String.Empty

    Public Property ExtPaymentReference() As String
        Get
            Return _extPaymentReference
        End Get
        Set(ByVal value As String)
            _extPaymentReference = value
        End Set
    End Property

    Public Property ExtPaymentCompany() As String
        Get
            Return _extPaymentCompany
        End Get
        Set(ByVal value As String)
            _extPaymentCompany = value
        End Set
    End Property

    Public Property ExtPaymentName() As String
        Get
            Return _extPaymentName
        End Get
        Set(ByVal value As String)
            _extPaymentName = value
        End Set
    End Property

    Public Property ExtPaymentAddress1() As String
        Get
            Return _extPaymentAddress1
        End Get
        Set(ByVal value As String)
            _extPaymentAddress1 = value
        End Set
    End Property
    Public Property ExtPaymentAddress2() As String
        Get
            Return _extPaymentAddress2
        End Get
        Set(ByVal value As String)
            _extPaymentAddress2 = value
        End Set
    End Property
    Public Property ExtPaymentAddress3() As String
        Get
            Return _extPaymentAddress3
        End Get
        Set(ByVal value As String)
            _extPaymentAddress3 = value
        End Set
    End Property
    Public Property ExtPaymentAddress4() As String
        Get
            Return _extPaymentAddress4
        End Get
        Set(ByVal value As String)
            _extPaymentAddress4 = value
        End Set
    End Property

    Public Property ExtPaymentCountry() As String
        Get
            Return _extPaymentCountry
        End Get
        Set(ByVal value As String)
            _extPaymentCountry = value
        End Set
    End Property

    Public Property ExtPaymentPostCode() As String
        Get
            Return _extPaymentPostCode
        End Get
        Set(ByVal value As String)
            _extPaymentPostCode = value
        End Set
    End Property

    Public Property ExtPaymentTel1() As String
        Get
            Return _extPaymentTel1
        End Get
        Set(ByVal value As String)
            _extPaymentTel1 = value
        End Set
    End Property
    Public Property ExtPaymentTel2() As String
        Get
            Return _extPaymentTel2
        End Get
        Set(ByVal value As String)
            _extPaymentTel2 = value
        End Set
    End Property
    Public Property ExtPaymentTel3() As String
        Get
            Return _extPaymentTel3
        End Get
        Set(ByVal value As String)
            _extPaymentTel3 = value
        End Set
    End Property

    Public Property ExtPaymentEmail() As String
        Get
            Return _extPaymentEmail
        End Get
        Set(ByVal value As String)
            _extPaymentEmail = value
        End Set
    End Property


    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder

        With sb
            .Append(ExtPaymentReference & ",")
            .Append(ExtPaymentCompany & ",")
            .Append(ExtPaymentName & ",")
            .Append(ExtPaymentAddress1 & ",")
            .Append(ExtPaymentAddress2 & ",")
            .Append(ExtPaymentAddress3 & ",")
            .Append(ExtPaymentAddress4 & ",")
            .Append(ExtPaymentCountry & ",")
            .Append(ExtPaymentPostCode & ",")
            .Append(ExtPaymentTel1 & ",")
            .Append(ExtPaymentTel2 & ",")
            .Append(ExtPaymentTel3 & ",")
            .Append(ExtPaymentEmail)
        End With

        Return sb.ToString.Trim

    End Function

End Class
