<Serializable()> _
Public Class DEDeliveryCharges

    Public Class DEDeliveryCharge
        Private _deliveryType As String
        Public Property DELIVERY_TYPE() As String
            Get
                Return _deliveryType
            End Get
            Set(ByVal value As String)
                _deliveryType = value
            End Set
        End Property

        Private _desc1 As String
        Public Property DESCRIPTION1() As String
            Get
                Return _desc1
            End Get
            Set(ByVal value As String)
                _desc1 = value
            End Set
        End Property

        Private _DESC2 As String
        Public Property DESCRIPTION2() As String
            Get
                Return _DESC2
            End Get
            Set(ByVal value As String)
                _DESC2 = value
            End Set
        End Property


        Private _langdesc1 As String
        Public Property LANG_DESCRIPTION1() As String
            Get
                Return _langdesc1
            End Get
            Set(ByVal value As String)
                _langdesc1 = value
            End Set
        End Property

        Private _langdesc2 As String
        Public Property LANG_DESCRIPTION2() As String
            Get
                Return _langdesc2
            End Get
            Set(ByVal value As String)
                _langdesc2 = value
            End Set
        End Property


        Private _bu As String
        Public Property BUSINESS_UNIT() As String
            Get
                Return _bu
            End Get
            Set(ByVal value As String)
                _bu = value
            End Set
        End Property

        Private _partner As String
        Public Property PARTNER() As String
            Get
                Return _partner
            End Get
            Set(ByVal value As String)
                _partner = value
            End Set
        End Property

        Private _seq As Integer
        Public Property SEQUENCE() As Integer
            Get
                Return _seq
            End Get
            Set(ByVal value As Integer)
                _seq = value
            End Set
        End Property

        Private _DELIVERY_PARENT As String
        Public Property DELIVERY_PARENT() As String
            Get
                Return _DELIVERY_PARENT
            End Get
            Set(ByVal value As String)
                _DELIVERY_PARENT = value
            End Set
        End Property

        Private _DEFAULT As Boolean
        Public Property IS_DEFAULT() As Boolean
            Get
                Return _DEFAULT
            End Get
            Set(ByVal value As Boolean)
                _DEFAULT = value
            End Set
        End Property

        Private _UPPERBREAK As Decimal
        Public Property UPPER_BREAK() As Decimal
            Get
                Return _UPPERBREAK
            End Get
            Set(ByVal value As Decimal)
                _UPPERBREAK = value
            End Set
        End Property

        Private _GREATER As Boolean
        Public Property GREATER() As Boolean
            Get
                Return _GREATER
            End Get
            Set(ByVal value As Boolean)
                _GREATER = value
            End Set
        End Property

        Private _AVAILABLE As Boolean
        Public Property AVAILABLE() As Boolean
            Get
                Return _AVAILABLE
            End Get
            Set(ByVal value As Boolean)
                _AVAILABLE = value
            End Set
        End Property

        Private _GROSS_VALUE As Decimal
        Public Property GROSS_VALUE() As Decimal
            Get
                Return _GROSS_VALUE
            End Get
            Set(ByVal value As Decimal)
                _GROSS_VALUE = value
            End Set
        End Property

        Private _NET_VALUE As Decimal
        Public Property NET_VALUE() As Decimal
            Get
                Return _NET_VALUE
            End Get
            Set(ByVal value As Decimal)
                _NET_VALUE = value
            End Set
        End Property

        Private _TAX_VALUE As Decimal
        Public Property TAX_VALUE() As Decimal
            Get
                Return _TAX_VALUE
            End Get
            Set(ByVal value As Decimal)
                _TAX_VALUE = value
            End Set
        End Property

        Public Property DELIVERY_TYPE_ZONE_CODE() As String = String.Empty
        Public Property COUNTRY_CODE As String = String.Empty
        Public Property COUNTRY_DESCRIPTION As String = String.Empty
        Public Property UPPER_BREAK_MODE() As Integer = 0

        Public ReadOnly Property HasChildNodes() As Boolean
            Get
                If Me.ChildNodes.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Private _ChildNodes As Generic.List(Of DEDeliveryCharge)
        Public Property ChildNodes() As Generic.List(Of DEDeliveryCharge)
            Get
                Return _ChildNodes
            End Get
            Set(ByVal value As Generic.List(Of DEDeliveryCharge))
                _ChildNodes = value
            End Set
        End Property

        Public Sub New()
            MyBase.New()
            _ChildNodes = New Generic.List(Of DEDeliveryCharge)
        End Sub
    End Class


    Private _deliveryCharges As Generic.List(Of DEDeliveryCharge)
    Public Property DeliveryCharges() As Generic.List(Of DEDeliveryCharge)
        Get
            Return _deliveryCharges
        End Get
        Set(ByVal value As Generic.List(Of DEDeliveryCharge))
            _deliveryCharges = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        _deliveryCharges = New Generic.List(Of DEDeliveryCharge)
    End Sub

    Public Function GetDeliveryCharge(ByVal deliveryType As String) As DEDeliveryCharge
        Return TraverseCharges_FindCharge(deliveryType, Me.DeliveryCharges)
    End Function

    Private Function TraverseCharges_FindCharge(ByVal deliveryType As String, ByVal charges As Generic.List(Of DEDeliveryCharge)) As DEDeliveryCharge

        For Each dc As Talent.Common.DEDeliveryCharges.DEDeliveryCharge In charges
            If dc.DELIVERY_TYPE = deliveryType Then
                Return dc
            Else
                If dc.HasChildNodes Then
                    Return TraverseCharges_FindCharge(deliveryType, dc.ChildNodes)
                End If
            End If
        Next

        'Finally, return if nothing found
        Return New DEDeliveryCharge
    End Function


End Class
