<Serializable()> _
Public Class DEProductOptions

    Private _optionDefBU As String
    Public Property OptionDefinitionDefaultBusinessUnit() As String
        Get
            Return _optionDefBU
        End Get
        Set(ByVal value As String)
            _optionDefBU = value
        End Set
    End Property

    Private _optionDefPartner As String
    Public Property OptionDefinitionDefaultPartner() As String
        Get
            Return _optionDefPartner
        End Get
        Set(ByVal value As String)
            _optionDefPartner = value
        End Set
    End Property

    Private _optionsDefinitionsLoadMode As String
    Public Property OptionsDefinitionsLoadMode() As String
        Get
            Return _optionsDefinitionsLoadMode
        End Get
        Set(ByVal value As String)
            _optionsDefinitionsLoadMode = value
        End Set
    End Property

    Private _optionDefs As Generic.List(Of DEProductOptionDefinition)
    Public Property OptionDefinitionDEs() As Generic.List(Of DEProductOptionDefinition)
        Get
            Return _optionDefs
        End Get
        Set(ByVal value As Generic.List(Of DEProductOptionDefinition))
            _optionDefs = value
        End Set
    End Property

    Private _optionTypeBU As String
    Public Property OptionTypeDefaultBusinessUnit() As String
        Get
            Return _optionTypeBU
        End Get
        Set(ByVal value As String)
            _optionTypeBU = value
        End Set
    End Property

    Private _optionTypePartner As String
    Public Property OptionTypeDefaultPartner() As String
        Get
            Return _optionTypePartner
        End Get
        Set(ByVal value As String)
            _optionTypePartner = value
        End Set
    End Property

    Private _optionsTypeLoadMode As String
    Public Property OptionsTypeLoadMode() As String
        Get
            Return _optionsTypeLoadMode
        End Get
        Set(ByVal value As String)
            _optionsTypeLoadMode = value
        End Set
    End Property

    Private _optionTypes As Generic.List(Of DEProductOptionType)
    Public Property OptionTypesDEs() As Generic.List(Of DEProductOptionType)
        Get
            Return _optionTypes
        End Get
        Set(ByVal value As Generic.List(Of DEProductOptionType))
            _optionTypes = value
        End Set
    End Property

    Private _optionDefsAndOptions As List(Of DEProductOptionDefaultsAndOptions)
    Public Property OptionDefaultsAndOptionsDEs() As List(Of DEProductOptionDefaultsAndOptions)
        Get
            Return _optionDefsAndOptions
        End Get
        Set(ByVal value As List(Of DEProductOptionDefaultsAndOptions))
            _optionDefsAndOptions = value
        End Set
    End Property


    Sub New()
        MyBase.New()

        Me.OptionsTypeLoadMode = ""
        Me.OptionTypeDefaultBusinessUnit = ""
        Me.OptionTypeDefaultPartner = ""
        Me.OptionTypesDEs = New List(Of DEProductOptionType)

        Me.OptionsDefinitionsLoadMode = ""
        Me.OptionDefinitionDefaultBusinessUnit = ""
        Me.OptionDefinitionDefaultPartner = ""
        Me.OptionDefinitionDEs = New List(Of DEProductOptionDefinition)

        Me.OptionDefaultsAndOptionsDEs = New List(Of DEProductOptionDefaultsAndOptions)

    End Sub

End Class

Public Class DEProductOptionType

    Private _mode As String = ""
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _optionType As String = ""
    Public Property OptionType() As String
        Get
            Return _optionType
        End Get
        Set(ByVal value As String)
            _optionType = value
        End Set
    End Property

    Private _description As String = ""
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Private _langs As List(Of DEProductOptionTypeLang)
    Public Property LanguageSpecificDEs() As List(Of DEProductOptionTypeLang)
        Get
            Return _langs
        End Get
        Set(ByVal value As List(Of DEProductOptionTypeLang))
            _langs = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        Me.LanguageSpecificDEs = New List(Of DEProductOptionTypeLang)
    End Sub

End Class

Public Class DEProductOptionTypeLang
    Private _bu As String = ""
    Public Property BusinessUnit() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _partner As String = ""
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _langCode As String = ""
    Public Property LanguageCode() As String
        Get
            Return _langCode
        End Get
        Set(ByVal value As String)
            _langCode = value
        End Set
    End Property

    Private _displayName As String = ""
    Public Property DisplayName() As String
        Get
            Return _displayName
        End Get
        Set(ByVal value As String)
            _displayName = value
        End Set
    End Property

    Private _labeltext As String = ""
    Public Property LabelText() As String
        Get
            Return _labeltext
        End Get
        Set(ByVal value As String)
            _labeltext = value
        End Set
    End Property
End Class

Public Class DEProductOptionDefinition

    Private _mode As String = ""
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _optionCode As String = ""
    Public Property OptionCode() As String
        Get
            Return _optionCode
        End Get
        Set(ByVal value As String)
            _optionCode = value
        End Set
    End Property

    Private _description As String = ""
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Private _langs As List(Of DEProductOptionDefinitionLang)
    Public Property LanguageSpecificDEs() As List(Of DEProductOptionDefinitionLang)
        Get
            Return _langs
        End Get
        Set(ByVal value As List(Of DEProductOptionDefinitionLang))
            _langs = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        Me.LanguageSpecificDEs = New List(Of DEProductOptionDefinitionLang)
    End Sub

End Class

Public Class DEProductOptionDefinitionLang
    Private _bu As String = ""
    Public Property BusinessUnit() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _partner As String = ""
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _langCode As String = ""
    Public Property LanguageCode() As String
        Get
            Return _langCode
        End Get
        Set(ByVal value As String)
            _langCode = value
        End Set
    End Property

    Private _displayName As String = ""
    Public Property DisplayName() As String
        Get
            Return _displayName
        End Get
        Set(ByVal value As String)
            _displayName = value
        End Set
    End Property
End Class


Public Class DEProductOptionDefaultsAndOptions

    Private _mode As String = ""
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _bu As String = ""
    Public Property BusinessUnit() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _partner As String = ""
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _optionsDefaults As List(Of DEProductOptionDefault)
    Public Property OptionDefaultDEs() As List(Of DEProductOptionDefault)
        Get
            Return _optionsDefaults
        End Get
        Set(ByVal value As List(Of DEProductOptionDefault))
            _optionsDefaults = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        Me.OptionDefaultDEs = New List(Of DEProductOptionDefault)
    End Sub

End Class


Public Class DEProductOptionDefault

    Private _action As String = ""
    Public Property Action() As String
        Get
            Return _action
        End Get
        Set(ByVal value As String)
            _action = value
        End Set
    End Property

    Private _masterProduct As String = ""
    Public Property MasterProduct() As String
        Get
            Return _masterProduct
        End Get
        Set(ByVal value As String)
            _masterProduct = value
        End Set
    End Property

    Private _optionType As String = ""
    Public Property OptionType() As String
        Get
            Return _optionType
        End Get
        Set(ByVal value As String)
            _optionType = value
        End Set
    End Property

    Private _optionCode As String = ""
    Public Property OptionCode() As String
        Get
            Return _optionCode
        End Get
        Set(ByVal value As String)
            _optionCode = value
        End Set
    End Property

    Private _matchaction As String = ""
    Public Property MatchAction() As String
        Get
            Return _matchaction
        End Get
        Set(ByVal value As String)
            _matchaction = value
        End Set
    End Property

    Private _appendSeq As Integer = 0
    Public Property AppendSequence() As Integer
        Get
            Return _appendSeq
        End Get
        Set(ByVal value As Integer)
            _appendSeq = value
        End Set
    End Property

    Private _displaySeq As Integer = 0
    Public Property DisplaySequence() As Integer
        Get
            Return _displaySeq
        End Get
        Set(ByVal value As Integer)
            _displaySeq = value
        End Set
    End Property

    Private _displayType As String = 0
    Public Property DisplayType() As String
        Get
            Return _displayType
        End Get
        Set(ByVal value As String)
            _displayType = value
        End Set
    End Property

    Private _ProductOptions As List(Of DEProductOption)
    Public Property ProductOptions() As List(Of DEProductOption)
        Get
            Return _ProductOptions
        End Get
        Set(ByVal value As List(Of DEProductOption))
            _ProductOptions = value
        End Set
    End Property

    Private _isDefault As Boolean = False

    Public Property IsDefault() As Boolean
        Get
            Return _isDefault
        End Get
        Set(ByVal value As Boolean)
            _isDefault = value
        End Set
    End Property

    Sub New()
        MyBase.New()
        Me.ProductOptions = New List(Of DEProductOption)
    End Sub

End Class

Public Class DEProductOption

    Private _optionCode As String = ""
    Public Property OptionCode() As String
        Get
            Return _optionCode
        End Get
        Set(ByVal value As String)
            _optionCode = value
        End Set
    End Property

    Private _prodCode As String = ""
    Public Property ProductCode() As String
        Get
            Return _prodCode
        End Get
        Set(ByVal value As String)
            _prodCode = value
        End Set
    End Property

    Private _displayOrder As String = ""
    Public Property DisplayOrder() As String
        Get
            Return _displayOrder
        End Get
        Set(ByVal value As String)
            _displayOrder = value
        End Set
    End Property

    Private _masterProduct As String = ""
    Public Property MasterProduct() As String
        Get
            Return _masterProduct
        End Get
        Set(ByVal value As String)
            _masterProduct = value
        End Set
    End Property

    Private _optionType As String = ""
    Public Property OptionType() As String
        Get
            Return _optionType
        End Get
        Set(ByVal value As String)
            _optionType = value
        End Set
    End Property

End Class


