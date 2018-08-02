Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access logging data related table objects
    ''' </summary>
    <Serializable()> _
        Public Class Logging

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblLogHeader As tbl_log_header
        Private _tblLogs As tbl_logs
        Private _tblDataTransferStatus As tbl_data_transfer_status

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
        ''' Create and Gets the tbl_log_header instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblLogHeader() As tbl_log_header
            Get
                If (_tblLogHeader Is Nothing) Then
                    _tblLogHeader = New tbl_log_header(_settings)
                End If
                Return _tblLogHeader
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_logs instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblLogs() As tbl_logs
            Get
                If (_tblLogs Is Nothing) Then
                    _tblLogs = New tbl_logs(_settings)
                End If
                Return _tblLogs
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_logs instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblDataTransferStatus() As tbl_data_transfer_status
            Get
                If (_tblDataTransferStatus Is Nothing) Then
                    _tblDataTransferStatus = New tbl_data_transfer_status(_settings)
                End If
                Return _tblDataTransferStatus
            End Get
        End Property

#End Region

    End Class

End Namespace
