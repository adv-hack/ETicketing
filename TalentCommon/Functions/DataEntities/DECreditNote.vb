'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Credit Notes
'
'       Date                        6th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEIN- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DECreditNote

    Private _collDETrans As New Collection          ' Transaction details

    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property

    Private _finalised As Boolean
    Public Property Finalised() As Boolean
        Get
            Return _finalised
        End Get
        Set(ByVal value As Boolean)
            _finalised = value
        End Set
    End Property

    Private _bu As String
    Public Property BusinessUnit() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _usr As String
    Public Property Username() As String
        Get
            Return _usr
        End Get
        Set(ByVal value As String)
            _usr = value
        End Set
    End Property

    Private _frmDate As Date
    Public Property FromDate() As Date
        Get
            Return _frmDate
        End Get
        Set(ByVal value As Date)
            _frmDate = value
        End Set
    End Property

    Private _toDate As Date
    Public Property ToDate() As Date
        Get
            Return _toDate
        End Get
        Set(ByVal value As Date)
            _toDate = value
        End Set
    End Property

    Private _cnNo As String
    Public Property CreditNoteNumber() As String
        Get
            Return _cnNo
        End Get
        Set(ByVal value As String)
            _cnNo = value
        End Set
    End Property

    Private _ordNo As String
    Public Property OrderNumber() As String
        Get
            Return _ordNo
        End Get
        Set(ByVal value As String)
            _ordNo = value
        End Set
    End Property

    Sub New()
        MyBase.New()
    End Sub

    ''' <summary>
    ''' Used to create a new instance of DEInvoice that is suitable for 
    ''' returning credit note information for a specific user
    ''' </summary>
    ''' <param name="_businessUnit"></param>
    ''' <param name="_username"></param>
    ''' <remarks></remarks>
    Sub New(ByVal _businessUnit As String, ByVal _username As String)
        MyBase.New()
        Me.BusinessUnit = _businessUnit
        Me.Username = _username
    End Sub

    ''' <summary>
    ''' Use this one when requesting a specific Credit Note
    ''' </summary>
    ''' <param name="_creditNoteNumber"></param>
    ''' <remarks></remarks>
    Sub New(ByVal _creditNoteNumber As String)
        MyBase.New()
        Me.CreditNoteNumber = _creditNoteNumber
    End Sub


End Class
