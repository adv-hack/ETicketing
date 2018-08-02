
Imports System
Imports com.qas.prowebondemand.soap


Namespace com.qas.prowebondemand


    ' Simple class to encapsulate Picklist data
    Public Class Picklist



        ' -- Private Members --



        Private m_sMoniker As String
        Private m_aItems As PicklistItem()
        Private m_sPrompt As String
        Private m_iTotal As Integer

        Private m_bAutoStepinSafe As Boolean
        Private m_bAutoStepinPastClose As Boolean
        Private m_bAutoFormatSafe As Boolean
        Private m_bAutoFormatPastClose As Boolean
        Private m_bLargePotential As Boolean
        Private m_bMaxMatches As Boolean
        Private m_bMoreOtherMatches As Boolean
        Private m_bOverThreshold As Boolean
        Private m_bTimeout As Boolean



        ' -- Public Methods --



        ' Construct from SOAP-layer object
        Public Sub New(ByVal p As QAPicklistType)

            m_iTotal = System.Convert.ToInt32(p.Total)
            m_sMoniker = p.FullPicklistMoniker
            m_sPrompt = p.Prompt
            m_bAutoStepinSafe = p.AutoStepinSafe
            m_bAutoStepinPastClose = p.AutoStepinPastClose
            m_bAutoFormatSafe = p.AutoFormatSafe
            m_bAutoFormatPastClose = p.AutoFormatPastClose
            m_bLargePotential = p.LargePotential
            m_bMaxMatches = p.MaxMatches
            m_bMoreOtherMatches = p.MoreOtherMatches
            m_bOverThreshold = p.OverThreshold
            m_bTimeout = p.Timeout

            'Convert the lines in the picklist
            m_aItems = Nothing
            Dim aItems As PicklistEntryType() = p.PicklistEntry

            'Check for null as we can have an empty picklist
            If Not aItems Is Nothing Then

                Dim iSize As Integer = aItems.GetLength(0)

                If iSize > 0 Then

                    ReDim m_aItems(iSize - 1)
                    Dim i As Integer
                    For i = 0 To iSize - 1
                        m_aItems(i) = New PicklistItem(aItems(i))
                    Next i

                End If

            End If

        End Sub



        ' -- Read-only Properties --



        ' Returns the full picklist moniker; that is, the moniker that describes the entire picklist
        ReadOnly Property Moniker() As String
            Get
                Return m_sMoniker
            End Get
        End Property


        ' Returns the array of PicklistItem objects
        ReadOnly Property Items() As PicklistItem()
            Get
                Return m_aItems
            End Get
        End Property


        ' Returns the number of items in the picklist
        ReadOnly Property Length() As Integer
            Get
                If m_aItems Is Nothing Then
                    Return 0
                Else
                    Return m_aItems.Length()
                End If
            End Get
        End Property


        ' Returns the prompt indicating what should be entered next by the user
        ReadOnly Property Prompt() As String
            Get
                Return m_sPrompt
            End Get
        End Property


        ' Returns the total number of addresses (excluding informationals) within this address location (approx)
        ReadOnly Property Total() As Integer
            Get
                Return m_iTotal
            End Get
        End Property



        ' -- Read-only Property Flags --



        ' Indicates that it is safe to automatically step-in to the first (and only) picklist item
        ReadOnly Property IsAutoStepinSafe() As Boolean
            Get
                Return m_bAutoStepinSafe
            End Get
        End Property


        ' Indicates that you may wish to automaticaly step-in to the first item, as 
        ' there was only one exact match, and other close matches
        ReadOnly Property IsAutoStepinPastClose() As Boolean
            Get
                Return m_bAutoStepinPastClose
            End Get
        End Property


        ' Indicates whether the picklist contains a single non-informational step-in item
        ' which you may wish to automatically step into after a refinement
        ReadOnly Property IsAutoStepinSingle() As Boolean
            Get
                Return (Length = 1 And Items(0).CanStep And Not Items(0).IsInformation)
            End Get
        End Property


        ' Indicates that it is safe to automatically format the first (and only) picklist item
        ReadOnly Property IsAutoFormatSafe() As Boolean
            Get
                Return m_bAutoFormatSafe
            End Get
        End Property


        ' Indicates that you may wish to automatically format the first item, as
        ' there was only one exact match, and other close matches
        ReadOnly Property IsAutoFormatPastClose() As Boolean
            Get
                Return m_bAutoFormatPastClose
            End Get
        End Property


        ' Indicates that the picklist contains a single non-informational final-address item
        ' which you may wish to automatically format after a refinement
        ReadOnly Property IsAutoFormatSingle() As Boolean
            Get
                Return (Length = 1 And Items(0).IsFullAddress And Not Items(0).IsInformation)
            End Get
        End Property


        ' Indicates that the picklist potentially contains too many items to display
        ReadOnly Property IsLargePotential() As Boolean
            Get
                Return m_bLargePotential
            End Get
        End Property


        ' Indicates that the number of matches exceeded the maximum allowed
        ReadOnly Property IsMaxMatches() As Boolean
            Get
                Return m_bMaxMatches
            End Get
        End Property


        ' Indicates that there are additional matches that can be displayed
        ' Only exact matches to the refinement text have been shown, as including all matches would be over threshold
        ' They can be shown by stepping into the informational at the bottom of the picklist
        ReadOnly Property IsMoreOtherMatches() As Boolean
            Get
                Return m_bMoreOtherMatches
            End Get
        End Property


        ' Indicates that the number of matches exceeded the threshold
        ReadOnly Property IsOverThreshold() As Boolean
            Get
                Return m_bOverThreshold
            End Get
        End Property


        ' Indicates that the search timed out
        ReadOnly Property IsTimeout() As Boolean
            Get
                Return m_bTimeout
            End Get
        End Property


    End Class


End Namespace
