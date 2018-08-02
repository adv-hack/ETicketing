<Serializable()> _
Public Class DEOrderTemplates

    Private _templates As Generic.List(Of DEOrderTemplate)
    Public Property OrderTemplates() As Generic.List(Of DEOrderTemplate)
        Get
            Return _templates
        End Get
        Set(ByVal value As Generic.List(Of DEOrderTemplate))
            _templates = value
        End Set
    End Property


    Private _purpose As String
    ''' <summary>
    ''' The current purpose of the DEOrderTemplates class, 
    ''' to be used by the DBAccess class to decide what action 
    ''' to take when AccessDatabase() is called.
    ''' Possible Values: SELECT, INSERT, UPDATE, DELETE, DELETE-LINE, EMPTY-TEMPLATE, SET-AS-DEFAULT
    ''' </summary>
    ''' <value>Options: SELECT, INSERT, UPDATE, DELETE, DELETE-LINE, EMPTY-TEMPLATE, SET-AS-DEFAULT</value>
    ''' <returns>Current purpose</returns>
    ''' <remarks></remarks>
    Public Property Purpose() As String
        Get
            Return _purpose
        End Get
        Set(ByVal value As String)
            _purpose = value
        End Set
    End Property

    ''' <summary>
    ''' Creates a new instance of the DEOrderTemplates Class
    ''' and takes the PURPOSE of the class as an argument, 
    ''' to be used by the DBAccess class to decide what action 
    ''' to take when AccessDatabase() is called.
    ''' </summary>
    ''' <param name="DBAccessPurpose">Options: SELECT, INSERT, UPDATE, DELETE, DELETE-LINE, EMPTY-TEMPLATE, SET-AS-DEFAULT
    ''' (Can be changed at any time by accessing the class' property Purpose)
    ''' </param>
    ''' <remarks></remarks>
    Public Sub New(ByVal DBAccessPurpose As String)
        MyBase.New()
        _templates = New Generic.List(Of DEOrderTemplate)
        Purpose = DBAccessPurpose
    End Sub

End Class

Public Class DEOrderTemplate

    Private _HeaderID As Long
    Public Property Template_Header_ID() As Long
        Get
            Return _HeaderID
        End Get
        Set(ByVal value As Long)
            _HeaderID = value
        End Set
    End Property

    Private _Name As String
    Public Property Template_Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Private __Description As String
    Public Property Description() As String
        Get
            Return __Description
        End Get
        Set(ByVal value As String)
            __Description = value
        End Set
    End Property

    Private _BU As String
    Public Property Business_Unit() As String
        Get
            Return _BU
        End Get
        Set(ByVal value As String)
            _BU = value
        End Set
    End Property

    Private _Partner As String
    Public Property Partner() As String
        Get
            Return _Partner
        End Get
        Set(ByVal value As String)
            _Partner = value
        End Set
    End Property

    Private _LoginID As String
    Public Property LoginID() As String
        Get
            Return _LoginID
        End Get
        Set(ByVal value As String)
            _LoginID = value
        End Set
    End Property

    Private _created As DateTime
    Public Property Created_Date() As DateTime
        Get
            Return _created
        End Get
        Set(ByVal value As DateTime)
            _created = value
        End Set
    End Property

    Private _modified As DateTime
    Public Property Last_Modified_Date() As DateTime
        Get
            Return _modified
        End Get
        Set(ByVal value As DateTime)
            _modified = value
        End Set
    End Property

    Private _used As DateTime
    Public Property Last_Used_Date() As DateTime
        Get
            Return _used
        End Get
        Set(ByVal value As DateTime)
            _used = value
        End Set
    End Property

    Private _IsDefault As Boolean
    Public Property Is_Default_Template() As Boolean
        Get
            Return _IsDefault
        End Get
        Set(ByVal value As Boolean)
            _IsDefault = value
        End Set
    End Property

    Private _allowFFToView As Boolean
    Public Property AllowFFToView() As Boolean
        Get
            Return _allowFFToView
        End Get
        Set(ByVal value As Boolean)
            _allowFFToView = value
        End Set
    End Property


    Private _items As Generic.List(Of OrderTemplateDetail)
    Public Property OrderTemplateItems() As Generic.List(Of OrderTemplateDetail)
        Get
            Return _items
        End Get
        Set(ByVal value As Generic.List(Of OrderTemplateDetail))
            _items = value
        End Set
    End Property

    Public Function GetItemByDetailID(ByVal DetailID As String) As OrderTemplateDetail
        For Each item As OrderTemplateDetail In Me.OrderTemplateItems
            If item.Template_Detail_ID = DetailID Then
                Return item
            End If
        Next
        Return New OrderTemplateDetail("", "", "")
    End Function

    ''' <summary>
    ''' Use this new to select a specific template, delete a specific template, or delete a specific
    ''' template-line
    ''' </summary>
    ''' <param name="TemplateHeaderID">The header ID of the template you wish to return</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal TemplateHeaderID As Long)
        MyBase.New()
        Me.Template_Header_ID = TemplateHeaderID
        _items = New Generic.List(Of OrderTemplateDetail)()
    End Sub

    ''' <summary>
    ''' Use this to perform all other actions.
    ''' * For SET-AS_DEFAULT you MUST supply the Template Header ID
    ''' </summary>
    ''' <param name="loginID"></param>
    ''' <param name="businessUnit"></param>
    ''' <param name="partner"></param>
    ''' <param name="created"></param>
    ''' <param name="lastUsed"></param>
    ''' <param name="lastModified"></param>
    ''' <param name="name"></param>
    ''' <param name="description"></param>
    ''' <param name="isdefault"></param>
    ''' <param name="TemplateHeaderID"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal loginID As String, _
                    ByVal businessUnit As String, _
                    ByVal partner As String, _
                    ByVal created As DateTime, _
                    ByVal lastUsed As DateTime, _
                    ByVal lastModified As DateTime, _
                    Optional ByVal name As String = "", _
                    Optional ByVal description As String = "", _
                    Optional ByVal isdefault As Boolean = False, _
                    Optional ByVal allowFFToView As Boolean = False, _
                    Optional ByVal TemplateHeaderID As Long = 0)

        MyBase.New()

        With Me
            .Template_Name = name
            .Description = description
            .Is_Default_Template = isdefault
            .LoginID = loginID
            .Business_Unit = businessUnit
            .Partner = partner
            .Created_Date = created
            .Last_Used_Date = lastUsed
            .Last_Modified_Date = lastModified
            .Template_Header_ID = TemplateHeaderID
            .AllowFFToView = allowFFToView
        End With

        _items = New Generic.List(Of OrderTemplateDetail)()

    End Sub


End Class

Public Class OrderTemplateDetail

    Private _ID As Long
    Public Property Template_Detail_ID() As Long
        Get
            Return _ID
        End Get
        Set(ByVal value As Long)
            _ID = value
        End Set
    End Property

    Private _headerID As Long
    Public Property Template_Header_ID() As Long
        Get
            Return _headerID
        End Get
        Set(ByVal value As Long)
            _headerID = value
        End Set
    End Property

    Private _productCode As String
    Public Property Product_Code() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _qty As Decimal
    Public Property Quantity() As Decimal
        Get
            Return _qty
        End Get
        Set(ByVal value As Decimal)
            _qty = value
        End Set
    End Property

    Private _masterProduct As String
    Public Property MasterProduct() As String
        Get
            Return _masterProduct
        End Get
        Set(ByVal value As String)
            _masterProduct = value
        End Set
    End Property


    ''' <summary>
    ''' Used to create a new item to populate into an Order Template object
    ''' USE THIS ONE WHEN DELETING SPECIFIC TEMPLATE-ORDER-LINES
    ''' </summary>
    ''' <param name="templateDetailID"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal templateDetailID As Long)
        MyBase.New()
        Me.Template_Detail_ID = templateDetailID
    End Sub

    ''' <summary>
    ''' Used to create a new item to populate into an Order Template object
    ''' Header ID and Detail ID are optional
    ''' </summary>
    ''' <param name="productCode"></param>
    ''' <param name="quantity"></param>
    ''' <param name="MasterProduct"></param>
    ''' <param name="templateDetailID"></param>
    ''' <param name="templateHeaderID"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal productCode As String, _
                    ByVal quantity As Decimal, _
                    ByVal masterProduct As String, _
                    Optional ByVal templateDetailID As Long = 0, _
                    Optional ByVal templateHeaderID As Long = 0)
        MyBase.New()

        Me.Product_Code = productCode
        Me.Quantity = quantity
        Me.Template_Detail_ID = templateDetailID
        Me.Template_Header_ID = templateHeaderID
        Me.MasterProduct = masterProduct
    End Sub


End Class
