
Imports System
Imports com.qas.prowebondemand.soap


Namespace com.qas.prowebondemand


    ' Simple class to encapsulate the data associated with one line of a picklist
    Public Class PicklistItem



        ' -- Private Members --



        Private m_sText As String
        Private m_sPartialAddress As String
        Private m_sPostcode As String
        Private m_iScore As Integer         'non-negative
        Private m_sMoniker As String

        Private m_bIsFullAddress As Boolean
        Private m_bIsMultiples As Boolean
        Private m_bCanStep As Boolean
        Private m_bIsAliasMatch As Boolean
        Private m_bIsPostcodeRecoded As Boolean
        Private m_bIsCrossBorderMatch As Boolean
        Private m_bIsDummyPOBox As Boolean
        Private m_bIsName As Boolean
        Private m_bIsInformation As Boolean
        Private m_bIsWarnInformation As Boolean
        Private m_bIsIncompleteAddress As Boolean
        Private m_bIsUnresolvableRange As Boolean
        Private m_bIsPhantomPrimaryPoint As Boolean



        ' -- Public Methods --



        ' Construct from SOAP-layer object
        Public Sub New(ByVal tItem As PicklistEntryType)

            m_sText = tItem.Picklist
            m_sPostcode = tItem.Postcode
            m_iScore = System.Convert.ToInt32(tItem.Score)
            m_sMoniker = tItem.Moniker
            m_sPartialAddress = tItem.PartialAddress

            'Flags
            m_bIsFullAddress = tItem.FullAddress
            m_bIsMultiples = tItem.Multiples
            m_bCanStep = tItem.CanStep
            m_bIsAliasMatch = tItem.AliasMatch
            m_bIsPostcodeRecoded = tItem.PostcodeRecoded
            m_bIsCrossBorderMatch = tItem.CrossBorderMatch
            m_bIsDummyPOBox = tItem.DummyPOBox
            m_bIsName = tItem.Name
            m_bIsInformation = tItem.Information
            m_bIsWarnInformation = tItem.WarnInformation
            m_bIsIncompleteAddress = tItem.IncompleteAddr
            m_bIsUnresolvableRange = tItem.UnresolvableRange
            m_bIsPhantomPrimaryPoint = tItem.PhantomPrimaryPoint

        End Sub



        ' -- Read-only Properties --



        ' Returns the picklist text for display
        ReadOnly Property Text() As String
            Get
                Return m_sText
            End Get
        End Property


        ' Returns the postcode for display; may be empty
        ReadOnly Property Postcode() As String
            Get
                Return m_sPostcode
            End Get
        End Property


        ' Returns the percentage score of this item; 0 if not applicable
        ReadOnly Property Score() As Integer
            Get
                Return m_iScore
            End Get
        End Property


        ' Returns the score of this item as a "%"-suffixed string
        ReadOnly Property ScoreAsString() As String
            Get
                If Score > 0 Then
                    Return Score.ToString() + "%"
                Else
                    Return ""
                End If
            End Get
        End Property


        ' Returns the moniker representing this item 
        ReadOnly Property Moniker() As String
            Get
                Return m_sMoniker
            End Get
        End Property


        ' Returns the full address details captured thus far 
        ReadOnly Property PartialAddress() As String
            Get
                Return m_sPartialAddress
            End Get
        End Property



        ' -- Read-only Property Flags --



        ' Indicates whether this item represents a full deliverable address, so can be formatted
        ReadOnly Property IsFullAddress() As Boolean
            Get
                Return m_bIsFullAddress
            End Get
        End Property


        ' Indicates whether this item represents multiple addresses (for display purposes)
        ReadOnly Property IsMultipleAddresses() As Boolean
            Get
                Return m_bIsMultiples
            End Get
        End Property


        ' Indicates whether the item can be stepped into
        ReadOnly Property CanStep() As Boolean
            Get
                Return m_bCanStep
            End Get
        End Property


        ' Indicates whether this entry is an alias match, which you may wish to highlight to the user
        ReadOnly Property IsAliasMatch() As Boolean
            Get
                Return m_bIsAliasMatch
            End Get
        End Property


        ' Indicates whether this entry has a recoded postcode, which you may wish to highlight to the user
        ReadOnly Property IsPostcodeRecoded() As Boolean
            Get
                Return m_bIsPostcodeRecoded
            End Get
        End Property


        ' Indicates whether this entry is a dummy (for DataSets without premise information)
        ' It can neither be stepped into nor formatted, but must be refined against with premise details
        ReadOnly Property IsIncompleteAddress() As Boolean
            Get
                Return m_bIsIncompleteAddress
            End Get
        End Property


        ' Indicates whether this entry is a range dummy (for DataSets with only ranges of premise information)
        ' It can neither be stepped into nor formatted, but must be refined against with premise details
        ReadOnly Property IsUnresolvableRange() As Boolean
            Get
                Return m_bIsUnresolvableRange
            End Get
        End Property


        ' Indicates whether this entry represents a nearby area, outside the strict initial
        ' boundaries of the search, which you may wish to highlight to the user
        ReadOnly Property IsCrossBorderMatch() As Boolean
            Get
                Return m_bIsCrossBorderMatch
            End Get
        End Property


        ' Indicates whether this entry is a dummy PO Box (which you may wish to display differently)
        ReadOnly Property IsDummyPOBox() As Boolean
            Get
                Return m_bIsDummyPOBox
            End Get
        End Property


        ' Indicates whether this entry is a Names item (which you may wish to display differently)
        ReadOnly Property IsName() As Boolean
            Get
                Return m_bIsName
            End Get
        End Property


        ' Indicates whether this entry is an informational prompt, rather than an address
        ReadOnly Property IsInformation() As Boolean
            Get
                Return m_bIsInformation
            End Get
        End Property


        ' Indicates whether this entry is a warning prompt, indicating that it is not possible to
        ' proceed any further (due to no matches, too many matches, etc.)
        ReadOnly Property IsWarnInformation() As Boolean
            Get
                Return m_bIsWarnInformation
            End Get
        End Property


        ' Is phantom primary point
        ReadOnly Property IsPhantomPrimaryPoint() As Boolean
            Get
                Return m_bIsPhantomPrimaryPoint
            End Get
        End Property


    End Class


End Namespace
