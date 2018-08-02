
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


	' AddressLine encapsulates data associated with an address line of a formatted address.
	Public Class AddressLine



		' -- Public Constants --



		' Enumeration of available line types returned by getLineType()
		Enum Types
			None = LineContentType.None
			Address = LineContentType.Address
			Name = LineContentType.Name
			Ancillary = LineContentType.Ancillary
			DataPlus = LineContentType.DataPlus
		End Enum



		' -- Private Members --



		Private m_sLabel As String
		Private m_sLine As String
		Private m_eLineType As Types
		Private m_bIsTruncated As Boolean
		Private m_bIsOverflow As Boolean



		' -- Public Methods --



		' Construct from SOAP-layer object
		Public Sub New(ByVal t As AddressLineType)

			m_sLabel = t.Label
			m_sLine = t.Line
			m_eLineType = CType(t.LineContent, Types)
			m_bIsTruncated = t.Truncated
			m_bIsOverflow = t.Overflow

		End Sub



		' -- Read-only Properties --



		' Returns the label of the line, probably the name of the address element fixed to it
		Public ReadOnly Property Label() As String
			Get
				Return m_sLabel
			End Get
		End Property


		' Returns the contents of the address line itself
		ReadOnly Property Line() As String
			Get
				Return m_sLine
			End Get
		End Property


		' Returns the type of the address line (Types enumeration)
		ReadOnly Property LineType() As Types
			Get
				Return m_eLineType
			End Get
		End Property


		' Returns the flag indicating whether the line was truncated
		ReadOnly Property IsTruncated() As Boolean
			Get
				Return m_bIsTruncated
			End Get
		End Property


		' Returns the flag indicating whether some address elements were lost from this line
		ReadOnly Property IsOverflow() As Boolean
			Get
				Return m_bIsOverflow
			End Get
		End Property


	End Class


End Namespace
