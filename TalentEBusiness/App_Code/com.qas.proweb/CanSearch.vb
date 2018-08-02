
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


	' Simple class to encapsulate the result of a CanSearch operation:
	' searching availability, and the reasons when unavailable
	Public Class CanSearch



		' -- Private Members --



		Private m_bOk As Boolean
		Private m_sErrorMessage As String
		Private m_iError As Integer



		' -- Public Methods --



		' Construct from SOAP-layer object
		Public Sub New(ByVal tResult As QASearchOk)

			m_bOk = tResult.IsOk

			If (Not tResult.ErrorCode Is Nothing) Then
				m_iError = System.Convert.ToInt32(tResult.ErrorCode)
			End If

			If (Not tResult.ErrorMessage Is Nothing) Then
				m_sErrorMessage = tResult.ErrorMessage & " [" & m_iError & "]"
			End If

		End Sub



		' -- Read-only Properties --



		' Returns whether searching is possible for the requested data-engine-layout combination
		ReadOnly Property IsOk() As Boolean
			Get
				Return m_bOk
			End Get
		End Property



		' Returns error information relating why it is not possible to search the requested data-engine-layout
		ReadOnly Property ErrorMessage() As String
			Get
				Return m_sErrorMessage
			End Get
		End Property


	End Class


End Namespace
