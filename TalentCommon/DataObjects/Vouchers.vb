Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Voucher related data table objects
    ''' </summary>
    <Serializable()> _
    Public Class Vouchers

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblVoucherExternal As tbl_vouchers_external


#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Vouchers" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_page instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblVoucherExternal() As tbl_vouchers_external
            Get
                If (_tblVoucherExternal Is Nothing) Then
                    _tblVoucherExternal = New tbl_vouchers_external(_settings)
                End If
                Return _tblVoucherExternal
            End Get
        End Property


#End Region

    End Class

End Namespace
