Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access application variable data related table objects
    ''' </summary>
    <Serializable()> _
    Public Class ApplicationVariables
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblAdhocFeesData As tbl_adhoc_fees
        Private _tblAuthorizedPartnersData As tbl_authorized_partners
        Private _tblBuData As tbl_bu
        Private _tblUrlBuData As tbl_url_bu
        Private _tblBuModuleDatabaseData As tbl_bu_module_database
        Private _tblDatabaseVersionData As tbl_database_version
        Private _tblEbusinessDescriptionsBuData As tbl_ebusiness_descriptions_bu
        Private _tblEcommerceModuleDefaultsBuData As tbl_ecommerce_module_defaults_bu
        Private _tblLanguageData As tbl_language
        Private _tblLanguageBuData As tbl_language_bu
        Private _tblQuerystring As tbl_querystring
        Private _tblUrlSecureData As tbl_url_secure
        Private _tbl_offline_processing As tbl_offline_processing
        Private _tblPartnerData As tbl_partner
        Private _tblCountryBuData As tbl_country_bu
        Private _tblDefaults As tbl_defaults
        Const CACHEKEY_CLASSNAME As String = "ApplicationVariables"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ApplicationVariables" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_adhoc_fees instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblAdhocFees() As tbl_adhoc_fees
            Get
                If (_tblAdhocFeesData Is Nothing) Then
                    _tblAdhocFeesData = New tbl_adhoc_fees(_settings)
                End If
                Return _tblAdhocFeesData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_authorized_partners instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblAuthorizedPartners() As tbl_authorized_partners
            Get
                If (_tblAuthorizedPartnersData Is Nothing) Then
                    _tblAuthorizedPartnersData = New tbl_authorized_partners(_settings)
                End If
                Return _tblAuthorizedPartnersData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_bu instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblBu() As tbl_bu
            Get
                If (_tblBuData Is Nothing) Then
                    _tblBuData = New tbl_bu(_settings)
                End If
                Return _tblBuData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_url_bu width DESettings
        ''' </summary>
        ''' <value>The tbl_url_bu instance</value>
        Public ReadOnly Property TblUrlBu() As tbl_url_bu
            Get
                If (_tblUrlBuData Is Nothing) Then
                    _tblUrlBuData = New tbl_url_bu(_settings)
                End If
                Return _tblUrlBuData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_bu_module_database instance with DESettings.
        ''' </summary>
        ''' <value>tbl_bu_module_database instance.</value>
        Public ReadOnly Property TblBuModuleDatabase() As tbl_bu_module_database
            Get
                If (_tblBuModuleDatabaseData Is Nothing) Then
                    _tblBuModuleDatabaseData = New tbl_bu_module_database(_settings)
                End If
                Return _tblBuModuleDatabaseData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_database_version instance with DESettings.
        ''' </summary>
        ''' <value>tbl_database_version instance.</value>
        Public ReadOnly Property TblDatabaseVersion() As tbl_database_version
            Get
                If (_tblDatabaseVersionData Is Nothing) Then
                    _tblDatabaseVersionData = New tbl_database_version(_settings)
                End If
                Return _tblDatabaseVersionData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_ebusiness_descriptions_bu instance with DESettings.
        ''' </summary>
        ''' <value>tbl_ebusiness_descriptions_bu instance.</value>
        Public ReadOnly Property TblEbusinessDescriptionsBu() As tbl_ebusiness_descriptions_bu
            Get
                If (_tblEbusinessDescriptionsBuData Is Nothing) Then
                    _tblEbusinessDescriptionsBuData = New tbl_ebusiness_descriptions_bu(_settings)
                End If
                Return _tblEbusinessDescriptionsBuData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_ecommerce_module_defaults_bu instance with DESettings.
        ''' </summary>
        ''' <value>tbl_ecommerce_module_defaults_bu instance.</value>
        Public ReadOnly Property TblEcommerceModuleDefaultsBu() As tbl_ecommerce_module_defaults_bu
            Get
                If (_tblEcommerceModuleDefaultsBuData Is Nothing) Then
                    _tblEcommerceModuleDefaultsBuData = New tbl_ecommerce_module_defaults_bu(_settings)
                End If
                Return _tblEcommerceModuleDefaultsBuData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_language instance with DESettings.
        ''' </summary>
        ''' <value>tbl_language instance.</value>
        Public ReadOnly Property TblLanguage() As tbl_language
            Get
                If (_tblLanguageData Is Nothing) Then
                    _tblLanguageData = New tbl_language(_settings)
                End If
                Return _tblLanguageData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_language_bu instance with DESettings.
        ''' </summary>
        ''' <value>tbl_language_bu instance.</value>
        Public ReadOnly Property TblLanguageBu() As tbl_language_bu
            Get
                If (_tblLanguageBuData Is Nothing) Then
                    _tblLanguageBuData = New tbl_language_bu(_settings)
                End If
                Return _tblLanguageBuData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_url_secure instance with DESettings.
        ''' </summary>
        ''' <value>tbl_url_secure instance.</value>
        Public ReadOnly Property TblUrlSecure() As tbl_url_secure
            Get
                If (_tblUrlSecureData Is Nothing) Then
                    _tblUrlSecureData = New tbl_url_secure(_settings)
                End If
                Return _tblUrlSecureData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_offline_processing instance with DESettings.
        ''' </summary>
        ''' <value>tbl_offline_processing instance.</value>
        Public ReadOnly Property TblOfflineProcessing() As tbl_offline_processing
            Get
                If (_tbl_offline_processing Is Nothing) Then
                    _tbl_offline_processing = New tbl_offline_processing(_settings)
                End If
                Return _tbl_offline_processing
            End Get
        End Property
        ''' <summary>
        ''' Create and Gets the tbl_partner instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblPartner() As tbl_partner
            Get
                If (_tblPartnerData Is Nothing) Then
                    _tblPartnerData = New tbl_partner(_settings)
                End If
                Return _tblPartnerData
            End Get
        End Property
        ''' <summary>
        ''' Create and Gets the tbl_querystring instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblQuerystring() As tbl_querystring
            Get
                If (_tblQuerystring Is Nothing) Then
                    _tblQuerystring = New tbl_querystring(_settings)
                End If
                Return _tblQuerystring
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_country_bu instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblCountryBu() As tbl_country_bu
            Get
                If (_tblCountryBuData Is Nothing) Then
                    _tblCountryBuData = New tbl_country_bu(_settings)
                End If
                Return _tblCountryBuData
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_country_bu instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblDefaults() As tbl_defaults
            Get
                If (_tblDefaults Is Nothing) Then
                    _tblDefaults = New tbl_defaults(_settings)
                End If
                Return _tblDefaults
            End Get
        End Property
#End Region

#Region "Public Methods"

#End Region

    End Class

End Namespace
