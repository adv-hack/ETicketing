Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Imports System.Text

<Serializable()> _
Public Class PasswordHash
    Private ReadOnly hashAlgorithm As HashAlgorithm

    ''' <summary>
    ''' Default constructor that creates defaul hash algorithm to use
    ''' </summary>
    Public Sub New()
        hashAlgorithm = New SHA512Managed()
    End Sub

    ''' <summary>
    ''' Constractor to pass a hash algorithm to using in hashing process
    ''' </summary>
    ''' <param name="hashAlgorithm"></param>
    Public Sub New(ByVal hashAlgorithm As HashAlgorithm)
        Me.hashAlgorithm = hashAlgorithm
    End Sub

    ''' <summary>
    ''' hashes the given string with given salt to base 64 string
    ''' </summary>
    ''' <param name="plainText">plain text to be hashed</param>
    ''' <param name="salt">Salt that will be used in hashing</param>
    ''' <returns>hashed base 64 string</returns>
    Public Function HashSalt(ByVal plainText As String, ByVal salt As String) As String
        Dim hashBytes As Byte() = GetHashBytes(plainText, salt)
        Dim hashValue As String = Convert.ToBase64String(hashBytes)
        Return hashValue
    End Function

    Private Function GetHashBytes(ByVal plainText As String, ByVal salt As String) As Byte()
        Dim plainTextWithSaltString As String = plainText + salt
        Dim plainTextWithSaltBytes As Byte() = Encoding.UTF8.GetBytes(plainTextWithSaltString)
        Return hashAlgorithm.ComputeHash(plainTextWithSaltBytes)
    End Function
    ''' <summary>
    ''' Takes a string as input and hashes it using the system default hash algorithm
    ''' </summary>
    ''' <param name="input">The string to be hashed</param>
    ''' <returns>The hashed string from the current encryption algorithm.</returns>
    ''' <remarks></remarks>
    Public Function HashTokenWithCurrentAlgorithm(ByVal input As String) As String
        Dim inputBytes As Byte()
        Dim outputBytes As Byte()
        Dim outputString As String

        inputBytes = Convert.FromBase64String(input)
        outputBytes = hashAlgorithm.ComputeHash(inputBytes)
        outputString = Convert.ToBase64String(outputBytes)

        Return outputString
    End Function
    ''' <summary>
    ''' Generates a 64 Byte long salt of random characters, using a cryptographic random number generator
    ''' </summary>
    ''' <returns>The salt string to be used for passwords</returns>
    ''' <remarks>Should be used whenever a new password is created</remarks>
    Public Function GenerateSalt()
        Dim saltHash(63) As Byte
        Dim saltString As String
        Dim rng As New RNGCryptoServiceProvider
        rng.GetBytes(saltHash)
        saltString = Encoding.UTF8.GetString(saltHash)
        Return saltString
    End Function
    ''' <summary>
    ''' Generates the token of length 10 and hash for use in password reset.
    ''' </summary>
    ''' <returns>Token and its hashed version</returns>
    Public Function ResetPasswordToken() As List(Of String)
        Dim tokenList As New List(Of String)
        Dim tokenHash As Byte()
        Dim tokenBytes(9) As Byte
        Dim token As String
        Dim hashToken As String
        Dim rng As New RNGCryptoServiceProvider
        rng.GetBytes(tokenBytes)
        token = Convert.ToBase64String(tokenBytes)
        'Need to remove some protected characters since this will be part of a query string
        token = token.Replace("+", "w")
        token = token.Replace("?", "x")
        token = token.Replace("&", "y")
        token = token.Replace("=", "z")
        tokenBytes = Convert.FromBase64String(token)
        tokenHash = hashAlgorithm.ComputeHash(tokenBytes)
        hashToken = Convert.ToBase64String(tokenHash)
        tokenList.Add(token)
        tokenList.Add(hashToken)
        Return tokenList
    End Function
End Class

