Imports Microsoft.VisualBasic

Public Class UserControlBase
    Inherits System.Web.UI.UserControl

#Region "Private Properties"

    Private _TDataObjects As Talent.Common.TalentDataObjects
    Private _deAlertDefinition As Talent.Common.DEAlertDefinition

#End Region

#Region "Public Properties"

    Public Property TDataObjects() As Talent.Common.TalentDataObjects
        Get
            Return _TDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentDataObjects)
            _TDataObjects = value
        End Set
    End Property

    Public Property DEAlertDefinition As Talent.Common.DEAlertDefinition
        Get
            Return _deAlertDefinition
        End Get
        Set(ByVal value As Talent.Common.DEAlertDefinition)
            _deAlertDefinition = value
        End Set
    End Property

#End Region

#Region "Private Methods"

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If _TDataObjects Is Nothing Then
            _TDataObjects = New Talent.Common.TalentDataObjects()
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            _TDataObjects.Settings = settings
        End If
        If _deAlertDefinition Is Nothing Then _deAlertDefinition = New Talent.Common.DEAlertDefinition
    End Sub

#End Region

#Region "Public Methods"

    Public Sub PopulateAlertSessionVariable()
        If _deAlertDefinition IsNot Nothing Then
            Session("AlertDefintionDataEntity") = _deAlertDefinition
        End If
    End Sub

    Public Sub PopulateAlertDataEntity()
        If Session("AlertDefintionDataEntity") IsNot Nothing Then
            _deAlertDefinition = Session("AlertDefintionDataEntity")
        End If
    End Sub

#End Region
End Class
