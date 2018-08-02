Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Data
Imports System.Configuration
Imports System.Web

Namespace CardProcessing.PayPal

    ''' <summary>
    ''' Support class for PayPal API class
    ''' </summary>
    Public NotInheritable Class NVPCodec
        Inherits NameValueCollection
        Private Const AMPERSAND As String = "&"
        Private Const EQUALSIGN As String = "="
        Private Shared ReadOnly AMPERSAND_CHAR_ARRAY As Char() = AMPERSAND.ToCharArray()
        Private Shared ReadOnly EQUALS_CHAR_ARRAY As Char() = EQUALSIGN.ToCharArray()

        ''' <summary> 
        ''' Returns the built NVP string of all name/value pairs in the Hashtable 
        ''' </summary> 
        ''' <returns></returns> 
        Public Function Encode() As String
            Dim sb As New StringBuilder()
            Dim firstPair As Boolean = True
            For Each kv As String In AllKeys
                Dim name As String = UrlEncode(kv)
                Dim value As String = UrlEncode(Me(kv))
                If Not firstPair Then
                    sb.Append(AMPERSAND)
                End If
                sb.Append(name).Append(EQUALSIGN).Append(value)
                firstPair = False
            Next
            Return sb.ToString()
        End Function

        ''' <summary> 
        ''' Decoding the string 
        ''' </summary> 
        ''' <param name="nvpstring"></param> 
        Public Sub Decode(ByVal nvpstring As String)
            Clear()
            For Each nvp As String In nvpstring.Split(AMPERSAND_CHAR_ARRAY)
                Dim tokens As String() = nvp.Split(EQUALS_CHAR_ARRAY)
                If tokens.Length >= 2 Then
                    Dim name As String = UrlDecode(tokens(0))
                    Dim value As String = UrlDecode(tokens(1))
                    Add(name, value)
                End If
            Next
        End Sub

        Private Shared Function UrlDecode(ByVal s As String) As String
            Return HttpUtility.UrlDecode(s)
        End Function

        Private Shared Function UrlEncode(ByVal s As String) As String
            Return HttpUtility.UrlEncode(s)
        End Function

#Region "Array methods"

        Public Overloads Sub Add(ByVal name As String, ByVal value As String, ByVal index As Integer)
            Me.Add(GetArrayName(index, name), value)
        End Sub

        Public Overloads Sub Remove(ByVal arrayName As String, ByVal index As Integer)
            Me.Remove(GetArrayName(index, arrayName))
        End Sub

        Default Public Overloads Property Item(ByVal name As String, ByVal index As Integer) As String
            Get
                Return Me(GetArrayName(index, name))
            End Get
            Set(ByVal value As String)
                Me(GetArrayName(index, name)) = value
            End Set
        End Property

        Private Shared Function GetArrayName(ByVal index As Integer, ByVal name As String) As String
            If index < 0 Then
                Throw New ArgumentOutOfRangeException("index", "index can not be negative : " & index)
            End If
            Return name + index
        End Function

#End Region

    End Class
End Namespace