
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


	' Simple class to encapsulate example address data
	Public Class ExampleAddress



		' -- Private Members --



		Private m_sComment As String
		Private m_Address As FormattedAddress



		' -- Public Methods --



		' Construct from SOAP-layer object
		Public Sub New(ByVal a As QAExampleAddress)

			m_sComment = a.Comment
			m_Address = New FormattedAddress(a.Address)

		End Sub



		' Create array from SOAP-layer array
		Public Shared Function createArray(ByVal aAddresses() As QAExampleAddress) As ExampleAddress()

            Dim aResults() As ExampleAddress = Nothing
			If Not aAddresses Is Nothing Then

				Dim iSize As Integer = aAddresses.GetLength(0)
				If iSize > 0 Then

					ReDim aResults(iSize - 1)
					Dim i As Integer
					For i = 0 To iSize - 1
						aResults(i) = New ExampleAddress(aAddresses(i))
					Next i

				End If

			End If

			Return aResults

		End Function



		' -- Read-only Properties --



		' Returns a comment describing the example address
		ReadOnly Property Comment() As String
			Get
				Return m_sComment
			End Get
		End Property


		' Returns the formatted example address
		ReadOnly Property AddressLines() As AddressLine()
			Get
				Return m_Address.AddressLines
			End Get
		End Property


	End Class


End Namespace
