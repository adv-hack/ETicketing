Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access serialized data related table objects
    ''' </summary>
    <Serializable()> _
    Public Class Serialized
#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tbl_serialized_objects As tbl_serialized_objects

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Serialized" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_serialized_objects instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblSerializedObjects() As tbl_serialized_objects
            Get
                If (_tbl_serialized_objects Is Nothing) Then
                    _tbl_serialized_objects = New tbl_serialized_objects(_settings)
                End If
                Return _tbl_serialized_objects
            End Get
        End Property

#End Region
    End Class
End Namespace
