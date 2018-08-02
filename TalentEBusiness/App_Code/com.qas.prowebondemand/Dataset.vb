
Imports System
Imports com.qas.prowebondemand.soap


Namespace com.qas.prowebondemand


    ' Simple class to encapsulate a Dataset - a searchable 'country'
    Public Class Dataset



        ' -- Private Members --



        Private m_sID As String
        Private m_sName As String



        ' -- Public Methods --



        ' Construct from SOAP-layer object
        Public Sub New(ByVal d As QADataSet)

            m_sID = d.ID
            m_sName = d.Name

        End Sub



        ' Create array from SOAP-layer array
        Public Shared Function CreateArray(ByVal aDatasets() As QADataSet) As Dataset()

            Dim aResults() As Dataset
            If Not aDatasets Is Nothing Then

                Dim iSize As Integer = aDatasets.GetLength(0)
                If iSize > 0 Then

                    ReDim aResults(iSize - 1)
                    Dim i As Integer
                    For i = 0 To iSize - 1
                        aResults(i) = New Dataset(aDatasets(i))
                    Next i
                End If
            End If

            Return aResults

        End Function



        ' Returns the Dataset which matches the data ID, otherwise null
        '   aDatasets = Dataset array to search
        '   sID = Data identifier to search for
        Public Shared Function FindByID(ByVal aDatasets() As Dataset, ByVal sDataID As String) As Dataset

            Dim i As Integer
            For i = 0 To aDatasets.GetLength(0) - 1
                If aDatasets(i).ID = sDataID Then
                    Return aDatasets(i)
                End If
            Next

        End Function



        ' -- Read-only Properties --



        ' Returns the name of the data set
        ReadOnly Property [Name]() As String
            Get
                Return m_sName
            End Get
        End Property


        ' Returns the ID of the data set (DataId)
        ReadOnly Property ID() As String
            Get
                Return m_sID
            End Get
        End Property


    End Class


End Namespace
