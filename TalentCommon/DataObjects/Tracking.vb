Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access tracking provider details 
    ''' </summary>
    <Serializable()> _
        Public Class Tracking
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblTrackingProviders As tbl_tracking_providers
        Private _tblTrackingUserDetails As tbl_tracking_user_details
        Private _tblPageTracking As tbl_page_tracking
        Private _tblTrackingSettingsValues As tbl_tracking_settings_values

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Tracking"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Clubs" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_tracking_providers instance with DESettings
        ''' </summary>
        ''' <value>tbl_tracking_providers instance</value>
        Public ReadOnly Property TblTrackingProviders() As tbl_tracking_providers
            Get
                If (_tblTrackingProviders Is Nothing) Then
                    _tblTrackingProviders = New tbl_tracking_providers(_settings)
                End If
                Return _tblTrackingProviders
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_page_tracking instance with DESettings
        ''' </summary>
        ''' <value>tbl_tracking_providers instance</value>
        Public ReadOnly Property TblPageTracking() As tbl_page_tracking
            Get
                If (_tblPageTracking Is Nothing) Then
                    _tblPageTracking = New tbl_page_tracking(_settings)
                End If
                Return _tblPageTracking
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_tracking_settings_values instance with DESettings
        ''' </summary>
        ''' <value>tbl_tracking_settings_values instance</value>
        Public ReadOnly Property TblTrackingSettingsValues() As tbl_tracking_settings_values
            Get
                If (_tblTrackingSettingsValues Is Nothing) Then
                    _tblTrackingSettingsValues = New tbl_tracking_settings_values(_settings)
                End If
                Return _tblTrackingSettingsValues
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_tracking_user_details instance with DESettings
        ''' </summary>
        ''' <value>tbl_tracking_user_details instance</value>
        Public ReadOnly Property TblTrackingUserDetails() As tbl_tracking_user_details
            Get
                If (_tblTrackingUserDetails Is Nothing) Then
                    _tblTrackingUserDetails = New tbl_tracking_user_details(_settings)
                End If
                Return _tblTrackingUserDetails
            End Get
        End Property
#End Region

#Region "Public Methods"

#End Region

    End Class
End Namespace
