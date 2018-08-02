'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Amend Template Requests
'
'       Date                        1st Nov 2006
'
'       Author                      Jonathan Williamson
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEATSK- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEAmendTemplate
    '
    Private _collDETrans As New Collection      ' Transaction details
    Private _collDEAlerts As New Collection     ' Amend Template details
    '
    Private _addToTemplate As Boolean = False
    Private _TemplateId As Long = 0
    Private _businessUnit As String = String.Empty
    Private _deleteTemplate As Boolean = False
    Private _deleteFromTemplate As Boolean = False
    Private _partnerCode As String = String.Empty
    Private _default As Boolean = False
    Private _replaceTemplate As Boolean = False
    Private _userID As String = String.Empty
    '

    Private _AddNewTemplate As Boolean
    Public Property AddNewTemplate() As Boolean
        Get
            Return _AddNewTemplate
        End Get
        Set(ByVal value As Boolean)
            _AddNewTemplate = value
        End Set
    End Property


    Private _name As String
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _desc As String
    Public Property Description() As String
        Get
            Return _desc
        End Get
        Set(ByVal value As String)
            _desc = value
        End Set
    End Property


    Private _created As DateTime
    Public Property CreatedDate() As DateTime
        Get
            Return _created
        End Get
        Set(ByVal value As DateTime)
            _created = value
        End Set
    End Property

    Private _mod As DateTime
    Public Property LastModifiedDate() As DateTime
        Get
            Return _mod
        End Get
        Set(ByVal value As DateTime)
            _mod = value
        End Set
    End Property

    Private _accessed As DateTime
    Public Property LastAccessedDate() As DateTime
        Get
            Return _accessed
        End Get
        Set(ByVal value As DateTime)
            _accessed = value
        End Set
    End Property


    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property
    Public Property CollDEAlerts() As Collection
        Get
            Return _collDEAlerts
        End Get
        Set(ByVal value As Collection)
            _collDEAlerts = value
        End Set
    End Property

    Public Property AddToTemplate() As Boolean
        Get
            Return _addToTemplate
        End Get
        Set(ByVal value As Boolean)
            _addToTemplate = value
        End Set
    End Property
    Public Property TemplateId() As Long
        Get
            Return _TemplateId
        End Get
        Set(ByVal value As Long)
            _TemplateId = value
        End Set
    End Property
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property DeleteTemplate() As Boolean
        Get
            Return _deleteTemplate
        End Get
        Set(ByVal value As Boolean)
            _deleteTemplate = value
        End Set
    End Property
    Public Property DeleteFromTemplate() As Boolean
        Get
            Return _deleteFromTemplate
        End Get
        Set(ByVal value As Boolean)
            _deleteFromTemplate = value
        End Set
    End Property
    Public Property PartnerCode() As String
        Get
            Return _partnerCode
        End Get
        Set(ByVal value As String)
            _partnerCode = value
        End Set
    End Property
    Public Property IsDefault() As Boolean
        Get
            Return _default
        End Get
        Set(ByVal value As Boolean)
            _default = value
        End Set
    End Property
    Public Property ReplaceTemplate() As Boolean
        Get
            Return _replaceTemplate
        End Get
        Set(ByVal value As Boolean)
            _replaceTemplate = value
        End Set
    End Property
    Public Property UserID() As String
        Get
            Return _userID
        End Get
        Set(ByVal value As String)
            _userID = value
        End Set
    End Property

End Class
