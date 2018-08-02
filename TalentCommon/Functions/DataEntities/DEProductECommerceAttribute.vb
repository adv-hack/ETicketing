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
Public Class DEProductEcommerceAttribute

    Private _mode As String = ""
    Private _category As String = ""
    Private _subCategory As String = ""
    Private _displaySequence As String = ""
    Private _attribute As String = ""

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property Category() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property

    Public Property SubCategory() As String
        Get
            Return _subCategory
        End Get
        Set(ByVal value As String)
            _subCategory = value
        End Set
    End Property

    Public Property Attribute() As String
        Get
            Return _attribute
        End Get
        Set(ByVal value As String)
            _attribute = value
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

End Class
