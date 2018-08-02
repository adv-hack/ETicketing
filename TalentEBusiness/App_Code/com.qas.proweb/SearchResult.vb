
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


    '  Class to encapsulate data returned by a search
    Public Class SearchResult



		' -- Public Constants --



		' Enumeration of verification levels
        Public Enum VerificationLevels
			' No verified matches found
			None = VerifyLevelType.None
            ' High confidence match found
            Verified = VerifyLevelType.Verified
            ' Single match found, but user confirmation is recommended
            InteractionRequired = VerifyLevelType.InteractionRequired
            ' Address was verified to premises level only (a picklist may be present)
            PremisesPartial = VerifyLevelType.PremisesPartial
            ' Address was verified to street level only (a picklist may be present)
            StreetPartial = VerifyLevelType.StreetPartial
            ' Address was verified to multiple addresses (picklist returned)
			Multiple = VerifyLevelType.Multiple
		End Enum



		' -- Private Members --



		Dim m_Address As FormattedAddress
		Dim m_Picklist As Picklist
		Private m_eVerifyLevel As VerificationLevels = VerificationLevels.None



		' -- Public Methods --



		' Construct from SOAP-layer object 
		Public Sub New(ByVal sr As QASearchResult)

			Dim address As QAAddressType = sr.QAAddress
			If Not address Is Nothing Then
				m_Address = New FormattedAddress(address)
			End If

			Dim PL As QAPicklistType = sr.QAPicklist
			If Not PL Is Nothing Then
				m_Picklist = New Picklist(PL)
			End If

			m_eVerifyLevel = sr.VerifyLevel

		End Sub



		' -- Read-only Properties --



		' Returns the address (may be null)
		ReadOnly Property Address() As FormattedAddress
			Get
				Return m_Address
			End Get
		End Property


		' Returns the picklist (may be null)
		ReadOnly Property Picklist() As Picklist
			Get
				Return m_Picklist
			End Get
		End Property


		' Returns the verification level of the result (only relavant when using the verification engine)
		ReadOnly Property VerifyLevel() As VerificationLevels
			Get
				Return m_eVerifyLevel
			End Get
		End Property


	End Class


End Namespace
