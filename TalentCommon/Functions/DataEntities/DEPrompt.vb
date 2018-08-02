'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for displaying Prompts
'
'       Date                        11/02/08
'
'       Author                      Ben Ford
'
'       � CS Group 2006             All rights reserved.
'
'--------------------------------------------------------------------------------------------------
'
<Serializable()> _
Public Class DEPrompt

    '---------------------------------------------------------------------------------
    '   General Fields
    '
    Private _promptType As String = String.Empty
    Private _promptSource As String = String.Empty

    Public Property PromptType() As String
        Get
            Return _promptType
        End Get
        Set(ByVal value As String)
            _promptType = value
        End Set
    End Property
    Public Property PromptSource() As String
        Get
            Return _promptSource
        End Get
        Set(ByVal value As String)
            _promptSource = value
        End Set
    End Property
    
End Class
