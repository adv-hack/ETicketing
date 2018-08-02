
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


	' This class encapsulates one line of a search prompt set.
    Public Class PromptLine



		' -- Private Members --



        Private m_sPrompt As String
        Private m_sExample As String
        Private m_iSuggestedInputLength As Integer = 0



		' -- Public Methods --



        '  Construct from SOAP-layer object
        Sub New(ByVal t As com.qas.proweb.soap.PromptLine)

            m_sPrompt = t.Prompt
            m_sExample = t.Example
            m_iSuggestedInputLength = System.Convert.ToInt32(t.SuggestedInputLength)

        End Sub



		' -- Read-only Properties --



		' Returns the prompt for this input line (e.g. "Town" or "Street")
        ReadOnly Property Prompt() As String
            Get
                Return m_sPrompt
            End Get
        End Property


		' Returns an example of what is expected for this input line (e.g. "London")
        ReadOnly Property Example() As String
            Get
                Return m_sExample
            End Get
        End Property


		' Returns the length in characters that is suggested for an input field for this line
        ReadOnly Property SuggestedInputLength() As Integer
            Get
                Return m_iSuggestedInputLength
            End Get
        End Property


    End Class


End Namespace
