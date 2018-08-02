Imports Microsoft.VisualBasic

Public Class ClassBase01

#Region "Private Properties"

    Private _TDataObjects As Talent.Common.TalentDataObjects
    Private _moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _talentDefaults As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues

#End Region

#Region "Public Properties"

    Public Property TDataObjects() As Talent.Common.TalentDataObjects
        Get
            If _TDataObjects Is Nothing Then
                _TDataObjects = New Talent.Common.TalentDataObjects()
                _TDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            End If
            Return _TDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentDataObjects)
            _TDataObjects = value
        End Set
    End Property

    Public Property ModuleDefaults() As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        Get
            If _moduleDefaults Is Nothing Then
                Dim defaultsObject As New Talent.eCommerce.ECommerceModuleDefaults
                _moduleDefaults = defaultsObject.GetDefaults
            End If
            Return _moduleDefaults
        End Get
        Set(ByVal value As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues)
            _moduleDefaults = value
        End Set
    End Property

    Public Property TalentDefaults() As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues
        Get
            If _talentDefaults Is Nothing Then
                Dim defaultsObject As New Talent.eCommerce.ECommerceTalentDefaults
                _talentDefaults = defaultsObject.GetDefaults
            End If
            Return _talentDefaults
        End Get
        Set(ByVal value As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues)
            _talentDefaults = value
        End Set
    End Property

#End Region
End Class
