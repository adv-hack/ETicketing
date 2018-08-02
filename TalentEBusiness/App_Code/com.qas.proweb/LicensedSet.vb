
Imports System
Imports com.qas.proweb.soap


Namespace com.qas.proweb


	' Simple class to encapsulates data for a single licensed set
	Public Class LicensedSet



		' -- Public Constants --



		' Enumeration of warning levels that can be returned
		Public Enum WarningLevels
			None = LicenceWarningLevel.None
			DataExpiring = LicenceWarningLevel.DataExpiring
			LicenceExpiring = LicenceWarningLevel.LicenceExpiring
			ClicksLow = LicenceWarningLevel.ClicksLow
			Evaluation = LicenceWarningLevel.Evaluation
			NoClicks = LicenceWarningLevel.NoClicks
			DataExpired = LicenceWarningLevel.DataExpired
			EvalLicenceExpired = LicenceWarningLevel.EvalLicenceExpired
			FullLicenceExpired = LicenceWarningLevel.FullLicenceExpired
			LicenceNotFound = LicenceWarningLevel.LicenceNotFound
			DataUnreadable = LicenceWarningLevel.DataUnreadable
		End Enum



		' -- Private Members -- 



		Private m_sID As String
		Private m_sDescription As String
		Private m_sCopyright As String
		Private m_sVersion As String
		Private m_sBaseCountry As String
		Private m_sStatus As String
		Private m_sServer As String
		Private m_eWarningLevel As WarningLevels
		Private m_iDaysLeft As Integer			'non-negative
		Private m_iDataDaysLeft As Integer		'non-negative
		Private m_iLicenceDaysLeft As Integer	'non-negative



		' -- Public Methods --



		' Construct from SOAP-layer object
		Public Sub New(ByVal s As QALicensedSet)

			m_sID = s.ID
			m_sDescription = s.Description
			m_sCopyright = s.Copyright
			m_sVersion = s.Version
			m_sBaseCountry = s.BaseCountry
			m_sStatus = s.Status
			m_sServer = s.Server
			m_eWarningLevel = s.WarningLevel
			m_iDaysLeft = System.Convert.ToInt32(s.DaysLeft)
			m_iDataDaysLeft = System.Convert.ToInt32(s.DataDaysLeft)
			m_iLicenceDaysLeft = System.Convert.ToInt32(s.LicenceDaysLeft)

		End Sub



		' Create array of LicensedSets given a SOAP-layer QALicenceInfo object.
		' We do not directly represent the QALicenceInfo structure, so lose it's warningLevel member
		' We simply return an array of LicensedSets.
		Public Shared Function CreateArray(ByVal info As QALicenceInfo) As LicensedSet()

            Dim aResults As LicensedSet() = Nothing
			Dim aInfo As QALicensedSet() = info.LicensedSet

			If Not aInfo Is Nothing Then

				Dim iSize As Integer = aInfo.GetLength(0)
				If iSize > 0 Then

					ReDim aResults(iSize - 1)
					Dim i As Integer
					For i = 0 To iSize - 1
						aResults(i) = New LicensedSet(aInfo(i))
					Next i

				End If

			End If

			Return aResults

		End Function



		' -- Read-only Properties --



		' Returns the ID of the licensed data
		ReadOnly Property ID() As String
			Get
				Return m_sID
			End Get
		End Property


		' Returns the description of the licensed data
		ReadOnly Property Description() As String
			Get
				Return m_sDescription
			End Get
		End Property


		' Returns the owner of the copyright for the licensed data
		ReadOnly Property Copyright() As String
			Get
				Return m_sCopyright
			End Get
		End Property


		' Returns the version of the licensed data
		ReadOnly Property Version() As String
			Get
				Return m_sVersion
			End Get

		End Property


		' Returns the DataId of the country with which this licensed dataset is associated
		ReadOnly Property BaseCountry() As String
			Get
				Return m_sBaseCountry
			End Get
		End Property


		' Returns status text detailing the amount of time left on the licensed set
		ReadOnly Property Status() As String
			Get
				Return m_sStatus
			End Get
		End Property


		' Returns the name of the QAS server where the data is being used
		ReadOnly Property Server() As String
			Get
				Return m_sServer
			End Get
		End Property


		' Returns the enumeration of the state of the data set
		ReadOnly Property WarningLevel() As WarningLevels
			Get
				Return m_eWarningLevel
			End Get
		End Property


		' Returns the number of days that the data will remain usable
		ReadOnly Property DaysLeft() As Integer
			Get
				Return m_iDaysLeft
			End Get
		End Property


		' Returns the number of days until the data expires
		ReadOnly Property DataDaysLeft() As Integer
			Get
				Return m_iDataDaysLeft
			End Get
		End Property


		' Returns the number of days until the license expires for this data
		ReadOnly Property LicenceDaysLeft() As Integer
			Get
				Return m_iLicenceDaysLeft
			End Get
		End Property


	End Class


End Namespace
