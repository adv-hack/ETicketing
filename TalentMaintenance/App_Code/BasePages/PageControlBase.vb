Imports Microsoft.VisualBasic

Public Class PageControlBase
    Inherits System.Web.UI.Page

    Private _TDataObjects As Talent.Common.TalentDataObjects
    Protected Property TDataObjects() As Talent.Common.TalentDataObjects
        Get
            Return _TDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentDataObjects)
            _TDataObjects = value
        End Set
    End Property

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If _TDataObjects Is Nothing Then
            _TDataObjects = New Talent.Common.TalentDataObjects()
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            _TDataObjects.Settings = settings
        End If
    End Sub
End Class
