Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

<Serializable()> _
Public Module UtilityExtension

    <Extension()> _
    Public Function ConvertStringToDecimal(ByVal value As String) As Decimal
        Dim retval As Decimal
        Dim res As Boolean = Decimal.TryParse(value, retval)

        If res Then
            Return retval / 100
        Else
            Return 0D
        End If
    End Function

    <Extension()> _
    Public Function GetNumberFromString(ByVal value As String) As Integer
        Dim returnVal As String = String.Empty
        Dim collection As MatchCollection = Regex.Matches(value, "\d+")
        For Each m As Match In collection
            returnVal += m.ToString()
        Next
        Return Convert.ToInt32(returnVal)
    End Function

    <Extension()> _
    Public Function GetLongNumberFromString(ByVal value As String) As Long
        Dim returnVal As String = String.Empty
        Dim collection As MatchCollection = Regex.Matches(value, "\d+")
        For Each m As Match In collection
            returnVal += m.ToString()
        Next
        Return Convert.ToInt64(returnVal)
    End Function

    <Extension()> _
    Public Function GetAlphabetPartFromString(ByVal value As String) As String
        Dim returnVal As String = String.Empty
        Dim collection As MatchCollection = Regex.Matches(value, "[a-zA-Z]+")
        For Each m As Match In collection
            returnVal += m.ToString()
        Next
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertToISeriesIntegerValue(ByVal value As Decimal) As Integer
        Dim returnVal As Decimal = value
        returnVal = returnVal * 100
        returnVal = Convert.ToInt32(returnVal)
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertPhysicalPathToServerUNCPath(ByVal cacheDependencyPath As String, ByVal ipAddress As String, ByVal cacheDependencyName As String) As String
        Dim startIndex As String = cacheDependencyPath.IndexOf("ftproot")
        cacheDependencyPath = cacheDependencyPath.Substring(startIndex, cacheDependencyPath.Length - startIndex)

        If cacheDependencyPath.Trim.EndsWith("\") Then
            cacheDependencyPath = String.Format("\\{0}\{1}\{2}", ipAddress, cacheDependencyPath, cacheDependencyName)
        Else
            cacheDependencyPath = String.Format("\\{0}\{1}\{2}", ipAddress, cacheDependencyPath, cacheDependencyName)
        End If
        Return cacheDependencyPath
    End Function

    <Extension()> _
    Public Function GetISeriesOperationMode(ByVal value As OperationMode) As String
        Dim returnVal As String = "A"   'For Add & Amend (both cannot happen together)
        If (value = OperationMode.Delete) Then
            returnVal = "D"
        ElseIf (value = OperationMode.Edit) Then
            returnVal = "E"
        ElseIf (value = OperationMode.Sequence) Then
            returnVal = "S"
        ElseIf (value = OperationMode.Extra) Then
            returnVal = "X"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertToISeriesYesNo(ByVal value As Boolean) As String
        Dim returnVal As String = "N"
        If value Then
            returnVal = "Y"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertFromISeriesYesNoToBoolean(ByVal value As String) As Boolean
        Dim returnVal As Boolean = False
        If value = "Y" Then
            returnVal = True
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertDatabaseComponentGroupTypeToUIValues(ByVal value As String) As String
        Dim returnVal As String = "Component Group"
        If value = "TA" Then
            returnVal = "Travel & Accomodation"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertUIComponentGroupTypeToDatabaseValues(ByVal value As String) As String
        Dim returnVal As String = "CG"
        If value = "Travel & Accomodation" OrElse value = "TA" Then
            returnVal = "TA"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertSelectionToIntegerValue(ByVal value As String) As Integer
        Dim returnVal As Integer = 0
        If Not value = "Not Applicable" Then
            returnVal = Integer.Parse(value)
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertDatabaseComponentTypeToUIValues(ByVal value As String) As String
        Dim returnVal As String = "Availability"
        If value = "C" Then
            returnVal = "Car Park"
        ElseIf value = "S" Then
            returnVal = "Stadium"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertUIComponentTypeToDatabaseValues(ByVal value As String) As String
        Dim returnVal As String = "A"
        If value = "Car Park" Then
            returnVal = "C"
        ElseIf value = "Stadium" Then
            returnVal = "S"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function Delete(ByVal dtToDeleteRows As DataTable, ByVal filterForSelect As String) As DataTable
        dtToDeleteRows.[Select](filterForSelect).Delete()
        Return dtToDeleteRows
    End Function

    <Extension()> _
    Public Sub Delete(ByVal rows As IEnumerable(Of DataRow))
        For Each row As DataRow In rows
            row.Delete()
        Next
    End Sub

    <Extension()> _
    Public Function GetDoubleFromString(ByVal input As String) As Double
        Dim result As Decimal
        If String.IsNullOrWhiteSpace(input) Then
            result = 0D
        End If
        Const Numbers As String = "0123456789."
        Dim numberBuilder = New System.Text.StringBuilder()
        For Each c As Char In input
            If Numbers.IndexOf(c) > -1 Then
                numberBuilder.Append(c)
            End If
        Next
        result = Double.Parse(numberBuilder.ToString())
        Return result
    End Function

    <Extension()> _
    Public Function ConvertAndTrimStringToInteger(ByVal input As String) As Integer
        Dim result As Integer
        If String.IsNullOrWhiteSpace(input) Then
            result = 0
        ElseIf Integer.TryParse(input, result) Then

        Else
            result = 0
        End If
        
        Return result
    End Function

    <Extension()> _
    Public Function ConvertAndTrimStringToLong(ByVal input As String) As Long
        Dim result As Long
        If String.IsNullOrWhiteSpace(input) Then
            result = 0
        ElseIf Long.TryParse(input, result) Then

        Else
            result = 0
        End If

        Return result
    End Function

    <Extension()> _
    Public Function GetISeriesDefaultType(ByVal input As DefaultType) As String
        Dim result As String = "G"
        If input = DefaultType.PerTicket Then
            result = "T"
        End If
        Return result
    End Function

    <Extension()> _
    Public Function ConvertISeriesDefaultTypeToEnum(ByVal input As String) As DefaultType
        Dim result As DefaultType = DefaultType.PerGroup
        If input = "T" Then
            result = DefaultType.PerTicket
        End If
        Return result
    End Function

    <Extension()> _
    Public Function GetISeriesSequenceMode(ByVal value As SequenceMode) As String
        Dim returnVal As String = "U"
        If (value = SequenceMode.MovingDown) Then
            returnVal = "D"
        End If
        Return returnVal
    End Function

    <Extension()> _
    Public Function ConvertISeriesDateAdjustment(ByVal input As String) As String
        Dim result As String = "Extra" 'P
        If input = "N" Then
            result = "Less"
        End If
        Return result
    End Function

    <Extension()> _
    Public Function ConvertPartPaymentFlagToISeriesType(ByVal input As String) As String
        Dim iSeriesType As String = ""
        If input = GlobalConstants.FEE_PARTPAYMENTFLAG_CHARGE_ALL Then
            iSeriesType = "1"
        ElseIf input = GlobalConstants.FEE_PARTPAYMENTFLAG_HIGHEST_ONLY Then
            iSeriesType = "2"
        ElseIf input = GlobalConstants.FEE_PARTPAYMENTFLAG_FIRST_ONLY Then
            iSeriesType = "3"
        End If
        Return iSeriesType
    End Function

End Module
