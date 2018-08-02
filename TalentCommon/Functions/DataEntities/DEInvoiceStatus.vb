'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Invoice Status Requests
'
'       Date                        03/01/06
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEIN- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEInvoiceStatus

    Private _collDETrans As New Collection          ' Transaction details
    Private _customerRef As String
    Private _invoiceNumber As String
    Private _JBARef As String

    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property
    Public Property CustomerRef() As String
        Get
            Return _customerRef
        End Get
        Set(ByVal value As String)
            _customerRef = value
        End Set
    End Property
    Public Property InvoiceNumber() As String
        Get
            Return _invoiceNumber
        End Get
        Set(ByVal value As String)
            _invoiceNumber = value
        End Set
    End Property
    Public Property JBARef() As String
        Get
            Return _JBARef
        End Get
        Set(ByVal value As String)
            _JBARef = value
        End Set
    End Property


End Class
