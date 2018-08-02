Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Menu Settings related data table objects
    ''' </summary>
    <Serializable()> _
        Public Class Menus

#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Menus" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

       

#End Region

    End Class

End Namespace
