Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access agent
    ''' </summary>
    <Serializable()> _
    Public Class Agent
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblAgent As tbl_agent

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Agent"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Agent" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_agent instance with DESettings
        ''' </summary>
        ''' <value>tbl_agent instance</value>
        Public ReadOnly Property TblAgent() As tbl_agent
            Get
                If (_tblAgent Is Nothing) Then
                    _tblAgent = New tbl_agent(_settings)
                End If
                Return _tblAgent
            End Get
        End Property

#End Region

#Region "Public Methods"
        
#End Region

    End Class
End Namespace

