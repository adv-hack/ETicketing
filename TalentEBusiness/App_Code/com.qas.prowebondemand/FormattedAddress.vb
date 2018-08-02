
Imports System
Imports com.qas.prowebondemand.soap


Namespace com.qas.prowebondemand


    ' Simple class to encapsulate data associated with a formatted address
    Public Class FormattedAddress



        ' -- Private Members --



        Private m_aAddressLines As AddressLine()
        Private m_bIsOverflow As Boolean
        Private m_bIsTruncated As Boolean



        ' -- Public Methods --



        ' Construct from SOAP-layer object
        Public Sub New(ByVal t As QAAddressType)

            m_bIsOverflow = t.Overflow
            m_bIsTruncated = t.Truncated

            Dim aLines As AddressLineType() = t.AddressLine

            ' We must have lines in an address so aLines should never be null
            Dim iSize As Integer = aLines.GetLength(0)
            ReDim m_aAddressLines(iSize - 1)
            If iSize > 0 Then

                Dim i As Integer
                For i = 0 To iSize - 1
                    m_aAddressLines(i) = New AddressLine(aLines(i))
                Next i

            End If

        End Sub



        ' -- Read-only Properties --



        ' Returns the array of address line objects
        ReadOnly Property AddressLines() As AddressLine()
            Get
                Return m_aAddressLines
            End Get
        End Property


        ' Returns the number of lines in the address
        ReadOnly Property Length() As Integer
            Get
                If m_aAddressLines Is Nothing Then
                    Return 0
                Else
                    Return m_aAddressLines.Length()
                End If
            End Get
        End Property


        ' Flag that indicates there were not enough address lines configured to contain the address
        ReadOnly Property IsOverflow() As Boolean
            Get
                Return m_bIsOverflow
            End Get
        End Property


        ' Flag that indicates one or more address lines were truncated
        ReadOnly Property IsTruncated() As Boolean
            Get
                Return m_bIsTruncated
            End Get
        End Property


    End Class


End Namespace
