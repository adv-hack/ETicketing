Imports System.Data.SqlClient
Imports System.Text
Imports System.Transactions
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to interface with the email templates tables
    ''' </summary>
    <Serializable()> _
    Public Class EmailTemplates
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblEmailTemplates As tbl_email_templates

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "EmailTemplates"

        'Used for logging
        Private Const SOURCEAPPLICATION As String = "MAINTENANCE"
        Private Const SOURCECLASS As String = "EMAILTEMPLATES"


#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Alerts" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the _tblEmailTemplates instance with DESettings
        ''' </summary>
        ''' <value>tbl_alerts_definition instance</value>
        Public ReadOnly Property TblEmailTemplates() As tbl_email_templates
            Get
                If (_tblEmailTemplates Is Nothing) Then
                    _tblEmailTemplates = New tbl_email_templates(_settings)
                End If
                Return _tblEmailTemplates
            End Get
        End Property

#End Region

#Region "Public Methods"

#End Region

    End Class
End Namespace
