'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Attribute Details
'
'       Date                        13th Oct 2008
'
'       Author                      Stuart Atkinson
'
'       � CS Group 2008               All rights reserved.
'
'       Error Number Code base      - 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEAttributeDetails

    Private _mode As String = ""
    Private _code As String = ""
    Private _displaySequence As String = ""
    Private _collDEProductDescriptions As New Collection
    Private _attributeValue As Decimal = 0
    Private _attributeDate As Date = Nothing
    Private _attributeBoolean As Boolean = False

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Property DisplaySequence() As String
        Get
            Return _displaySequence
        End Get
        Set(ByVal value As String)
            _displaySequence = value
        End Set
    End Property

    Public Property CollDEProductDescriptions() As Collection
        Get
            Return _collDEProductDescriptions
        End Get
        Set(ByVal value As Collection)
            _collDEProductDescriptions = value
        End Set
    End Property

    Public Property AttributeValue() As Decimal
        Get
            Return _attributeValue
        End Get
        Set(ByVal value As Decimal)
            _attributeValue = value
        End Set
    End Property

    Public Property AttributeDate() As Date
        Get
            Return _attributeDate
        End Get
        Set(ByVal value As Date)
            _attributeDate = value
        End Set
    End Property

    Public Property AttributeBoolean() As Boolean
        Get
            Return _attributeBoolean
        End Get
        Set(ByVal value As Boolean)
            _attributeBoolean = value
        End Set
    End Property

End Class
