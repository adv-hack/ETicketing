
Imports System
Imports com.qas.prowebondemand.soap


Namespace com.qas.prowebondemand


    ' Simple class to encapsulate layout data
    Public Class Layout



        ' -- Private Members --



        Private m_sName As String
        Private m_sComment As String



        ' -- Public Methods --



        ' Construct from SOAP-layer object
        Public Sub New(ByVal l As QALayout)

            m_sName = l.Name
            m_sComment = l.Comment

        End Sub



        ' Create array from SOAP-layer array
        Public Shared Function CreateArray(ByVal aLayouts As QALayout()) As Layout()

            Dim aResults() As Layout = Nothing
            If Not aLayouts Is Nothing Then

                Dim iSize As Integer = aLayouts.GetLength(0)
                ReDim aResults(iSize - 1)
                If iSize > 0 Then

                    Dim i As Integer
                    For i = 0 To iSize - 1
                        aResults(i) = New Layout(aLayouts(i))
                    Next i

                End If

            End If

            Return aResults

        End Function



        ' Returns the Layout which matches the name, otherwise Nothing
        '    aLayouts = Array of layouts to search</param>
        '    sLayoutName = Layout name to search for</param>
        Public Function FindByName(ByVal aLayouts As Layout(), ByVal sLayoutName As String) As Layout

            Dim i As Integer
            For i = 0 To aLayouts.GetLength(0)

                If aLayouts(i).Name = sLayoutName Then
                    Return aLayouts(i)
                End If

            Next
            Return Nothing

        End Function




        ' -- Read-only Properties --



        ' Returns the name of the layout
        ReadOnly Property Name() As String
            Get
                Return m_sName
            End Get
        End Property


        ' Returns any comment asscoiated with this layout
        ReadOnly Property Comment() As String
            Get
                Return m_sComment
            End Get
        End Property


    End Class


End Namespace
