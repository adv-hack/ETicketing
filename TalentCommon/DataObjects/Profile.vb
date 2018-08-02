Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access profile data related table objects
    ''' </summary>
    <Serializable()> _
        Public Class Profile

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tbl_authorized_users As tbl_authorized_users
        Private _tbl_partner_user As tbl_partner_user
        Private _tbl_address As tbl_address
        Private _tblAddressFormat1Data As tbl_address_format_1_data
        Private _tbl_country As tbl_country
        Private _tbl_forgotten_password As tbl_forgotten_password
        Private _tbl_registration_type As tbl_registration_type

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Profile" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_partner_user instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property tblPartnerUser() As tbl_partner_user
            Get
                If (_tbl_partner_user Is Nothing) Then
                    _tbl_partner_user = New tbl_partner_user(_settings)
                End If
                Return _tbl_partner_user
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_address instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property tblAddress() As tbl_address
            Get
                If (_tbl_address Is Nothing) Then
                    _tbl_address = New tbl_address(_settings)
                End If
                Return _tbl_address
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_agent instance with DESettings
        ''' </summary>
        ''' <value>tbl_address_format_1_data instance</value>
        Public ReadOnly Property TblAddressFormat1Data() As tbl_address_format_1_data
            Get
                If (_tblAddressFormat1Data Is Nothing) Then
                    _tblAddressFormat1Data = New tbl_address_format_1_data(_settings)
                End If
                Return _tblAddressFormat1Data
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_authorized_users instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property tblAuthorizedUsers() As tbl_authorized_users
            Get
                If (_tbl_authorized_users Is Nothing) Then
                    _tbl_authorized_users = New tbl_authorized_users(_settings)
                End If
                Return _tbl_authorized_users
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_country instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property tblCountry() As tbl_country
            Get
                If (_tbl_country Is Nothing) Then
                    _tbl_country = New tbl_country(_settings)
                End If
                Return _tbl_country
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_forgotten_password instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property tblForgottenPassword() As tbl_forgotten_password
            Get
                If (_tbl_forgotten_password Is Nothing) Then
                    _tbl_forgotten_password = New tbl_forgotten_password(_settings)
                End If
                Return _tbl_forgotten_password
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_registration_type instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property tblRegistrationType() As tbl_registration_type
            Get
                If (_tbl_registration_type Is Nothing) Then
                    _tbl_registration_type = New tbl_registration_type(_settings)
                End If
                Return _tbl_registration_type
            End Get
        End Property


#End Region

    End Class

End Namespace
