Imports Microsoft.VisualBasic
Imports System.Security.Cryptography
Namespace Talent.eCommerce
    Public Class clsKFWSecurity
        Public Function EncryptData(ByVal key As String, ByVal plaintext As String) As String

            Dim TripleDes As New TripleDESCryptoServiceProvider
            ' Initialize the crypto provider.
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize \ 8)
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize \ 8)
            ' Convert the plaintext string to a byte array.
            Dim plaintextBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(plaintext)
            ' Create the stream.
            Dim ms As New System.IO.MemoryStream
            ' Create the encoder to write to the stream.
            Dim encStream As New CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
            ' Use the crypto stream to write the byte array to the stream.
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
            encStream.FlushFinalBlock()
            ' Convert the encrypted stream to a printable string.
            Return Convert.ToBase64String(ms.ToArray)

        End Function
        Private Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()

            Dim sha1 As New SHA1CryptoServiceProvider
            ' Hash the key.
            Dim keyBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(key)
            Dim hash() As Byte = sha1.ComputeHash(keyBytes)
            ' Truncate or pad the hash.
            ReDim Preserve hash(length - 1)
            Return hash

        End Function
    End Class
End Namespace
