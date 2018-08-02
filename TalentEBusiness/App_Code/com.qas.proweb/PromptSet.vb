
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


	' Simple class to encapsulate data representing a search prompt set
    Public Class PromptSet



		' -- Public Constants --



        '  Enumeration of available search prompt sets
        Public Enum Types
			OneLine = PromptSetType.OneLine
			[Default] = PromptSetType.Default
            Generic = PromptSetType.Generic
            Optimal = PromptSetType.Optimal
            Alternate = PromptSetType.Alternate
            Alternate2 = PromptSetType.Alternate2
            Alternate3 = PromptSetType.Alternate3
		End Enum



		' -- Private Members --



		Private m_bDynamic As Boolean
		Private m_aLines As PromptLine()



		' -- Public Methods --



		'  Construct from SOAP-layer object; throws QasException
		Public Sub New(ByVal tPromptSet As QAPromptSet)

			m_bDynamic = tPromptSet.Dynamic

			m_aLines = Nothing
			If Not tPromptSet.Line Is Nothing Then

				Dim iSize As Integer = tPromptSet.Line.Length
				If iSize > 0 Then

					ReDim m_aLines(iSize - 1)
					Dim i As Integer = 0
					For i = 0 To (iSize - 1)
						m_aLines(i) = New PromptLine(tPromptSet.Line(i))
					Next i

				End If

			End If

		End Sub



		'  Returns a String() of prompts (from the search prompt line array)
		Public Function GetLinePrompts() As String()

			Dim iSize As Integer = m_aLines.GetLength(0)
			Dim asResults(iSize) As String
			Dim i As Integer

			For i = 0 To iSize - 1
				asResults(i) = m_aLines(i).Prompt
			Next i

			Return asResults

		End Function



		' -- Read-only Properties -- 


		' Returns whether dynamic searching should be used (submitting the search as they type)
		ReadOnly Property IsDynamic() As Boolean
			Get
				Return m_bDynamic
			End Get
		End Property



		' Returns the array of search prompt lines that make up this search prompt set
		ReadOnly Property Lines() As PromptLine()
			Get
				Return m_aLines
			End Get
		End Property


	End Class


End Namespace
