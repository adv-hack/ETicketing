Imports Microsoft.VisualBasic
Imports Talent.Common.TalentLogging
Imports System.IO

Public Class ControlBase
    Inherits System.Web.UI.UserControl

#Region "Class Level Fields"

    Private pageLogging As New Talent.Common.TalentLogging
    Private timeSpan As TimeSpan
    Private _moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _talentDefaults As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues
    Private _TDataObjects As Talent.Common.TalentDataObjects
    Private _TalAdminDataObjects As Talent.Common.TalentAdminDataObjects
    Private _agentProfile As Talent.eCommerce.Agent

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

    Public Property TalAdminDataObjects() As Talent.Common.TalentAdminDataObjects
        Get
            If _TalAdminDataObjects Is Nothing Then
                _TalAdminDataObjects = New Talent.Common.TalentAdminDataObjects()
                _TalAdminDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
                _TalAdminDataObjects.Settings.DestinationDatabase = GlobalConstants.TALENTADMINDESTINATIONDATABASE
                _TalAdminDataObjects.Settings.BackOfficeConnectionString = TDataObjects.AppVariableSettings.TblDatabaseVersion.TalentAdminDatabaseConnectionString()
            End If
            Return _TalAdminDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentAdminDataObjects)
            _TalAdminDataObjects = value
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


    Public Property AgentProfile() As Talent.eCommerce.Agent
        Get
            If _agentProfile Is Nothing Then
                _agentProfile = New Talent.eCommerce.Agent
            End If
            Return _agentProfile
        End Get
        Set(ByVal value As Talent.eCommerce.Agent)
            _agentProfile = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Sub AddFaceBookLikeMetaTag(ByVal propertyValue As String, ByVal content As String)
        Dim htmMeta As New HtmlMeta
        htmMeta.Attributes.Add("property", propertyValue)
        htmMeta.Content = content
        Me.Page.Header.Controls.Add(htmMeta)
        htmMeta = Nothing
    End Sub

#End Region

#Region "Public Function"

    ''' <summary>
    ''' Return the content of a given HTML include file name in the reserved HTML includes folder
    ''' </summary>
    ''' <param name="fileName">The given filename</param>
    ''' <returns>The file content</returns>
    ''' <remarks></remarks>
    Public Function GetStaticHTMLInclude(ByVal fileName As String) As String
        Dim includeFolderName As String = "Includes\"
        Dim fileContent As String = String.Empty
        Dim filePath As String = String.Empty
        If ModuleDefaults.HtmlPathAbsolute IsNot Nothing Then
            If Not ModuleDefaults.HtmlPathAbsolute.EndsWith("\") Then includeFolderName = "\" & includeFolderName
            filePath = ModuleDefaults.HtmlPathAbsolute & includeFolderName & fileName
            If File.Exists(filePath) Then
                Dim line As String = String.Empty
                Dim fStream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                Using fStream
                    Dim fReader As New StreamReader(fStream)
                    Using fReader
                        Do While fReader.Peek >= 0
                            line = line & fReader.ReadLine() & vbNewLine
                        Loop
                    End Using
                End Using
                fileContent = line
            End If
        End If
        Return fileContent
    End Function

#End Region

#Region "Private Methods"

    Private Sub Control_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        pageLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
    End Sub

#End Region

End Class
