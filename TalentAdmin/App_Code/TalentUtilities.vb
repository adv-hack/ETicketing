Imports Microsoft.VisualBasic
Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.Data.SqlClient
Imports System.Data

Public Class TalentUtilities

    Private Shared Sub CreateZipFile(ByVal Filename As String)
        'Create a blank zip file the same as right clicking and choosing
        'new > Compressed (Zipped) folder, it must have the extension .zip
        Dim Encoder As New System.Text.ASCIIEncoding
        Dim Header As String = "PK" & Chr(5) & Chr(6)
        Header = Header.PadRight(22, Chr(0))
        My.Computer.FileSystem.WriteAllBytes(Filename, Encoder.GetBytes(Header), False)
        
    End Sub

    Public Shared Sub ZipFile(ByVal Input As String, ByVal Filename As String)

        Try

            'Remove the zip file if it exists
            If IO.File.Exists(Filename) Then
                IO.File.Delete(Filename)
            End If

            'Create the empty zip file
            CreateZipFile(Filename)

            'Copy the contents
            Dim Shell As New Shell32.Shell
            Shell.NameSpace(Filename).CopyHere(Input)

        Catch ex As Exception

        End Try
        
    End Sub

    Public Shared Sub UnZipFile(ByVal FileName As String, ByVal Destination As String)

        Try
            'Rename any directory that already exists
            If IO.Directory.Exists(Destination) Then
                IO.Directory.Move(Destination, Destination & Now.Date.ToString & Now.TimeOfDay.ToString)
            End If

            'Create the new directory
            IO.Directory.CreateDirectory(Destination)

            'Extract the values to the new directory
            Dim myShell As New Shell32.Shell
            Dim Compressed As Shell32.Folder = myShell.NameSpace(FileName)
            Dim ExtractTo As Shell32.Folder = myShell.NameSpace(Destination)
            ExtractTo.CopyHere(Compressed.Items())

        Catch ex As Exception

        End Try
        
    End Sub

    Public Shared Sub FTPPutFile(ByVal ftpPath As String, _
                                  ByVal userName As String, _
                                  ByVal password As String, _
                                  ByVal localPath As String)

        Try

            ' Set up the ftp put request
            Dim ftp As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(ftpPath), System.Net.FtpWebRequest)
            ftp.Credentials = New System.Net.NetworkCredential(userName, password)
            ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile
            ftp.KeepAlive = False
            ftp.UseBinary = True

            ' Read the local file
            Dim bFile() As Byte = System.IO.File.ReadAllBytes(localPath)

            ' Perform the ftp put
            Dim clsStream As System.IO.Stream = ftp.GetRequestStream()
            clsStream.Write(bFile, 0, bFile.Length)
            clsStream.Close()
            clsStream.Dispose()

        Catch ex As Exception

        End Try

    End Sub

    Public Shared Sub FTPGetFile(ByVal ftpPath As String, _
                                  ByVal userName As String, _
                                  ByVal password As String, _
                                  ByVal localPath As String)


        ' THIS SUB HASN'T BEEN TESTED.

        'Values to use
        Const localFile As String = "C:\myfile.bin"

        ' Set up the ftp put request
        Dim ftp As System.Net.FtpWebRequest = DirectCast(System.Net.WebRequest.Create(ftpPath), System.Net.FtpWebRequest)
        ftp.Credentials = New System.Net.NetworkCredential(userName, password)
        ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile
        ftp.KeepAlive = False
        ftp.UseBinary = True

        ' Get the remote file  
        Using response As System.Net.FtpWebResponse = _
              CType(ftp.GetResponse, System.Net.FtpWebResponse)
            Using responseStream As IO.Stream = response.GetResponseStream
                'loop to read & write to file

                Using fs As New IO.FileStream(localFile, IO.FileMode.Create)
                    Dim buffer(2047) As Byte
                    Dim read As Integer = 0
                    Do
                        read = responseStream.Read(buffer, 0, buffer.Length)
                        fs.Write(buffer, 0, read)
                    Loop Until read = 0 'see Note(1)

                    responseStream.Close()
                    fs.Flush()
                    fs.Close()
                End Using
                responseStream.Close()
            End Using
            response.Close()
        End Using

    End Sub

    Public Shared Function CallWebPage(ByVal sUrl As String) As String

        Dim sResponse As String

        'Call the server page to create the web site
        Dim Request As HttpWebRequest = WebRequest.Create(sUrl)
        Dim Response As HttpWebResponse = Request.GetResponse
        Dim SR As StreamReader
        SR = New StreamReader(Response.GetResponseStream)

        'Ouput the results
        sResponse = SR.ReadToEnd
        SR.Close()

        Return sResponse

    End Function

    Public Shared Function ListItemDef(ByVal Text As String, ByVal Value As String) As ListItem

        Dim li As New ListItem(Text, Value)
        Return li

    End Function

    Public Shared Function getSQLVal(ByVal sql As String, Optional ByVal conStr As String = "") As String
        If conStr = "" Then conStr = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        Dim conn As SqlConnection
        Dim comm As SqlCommand
        Dim drSrv As SqlClient.SqlDataReader
        Dim rtn As New StringBuilder
        Try
            conn = New SqlConnection(conStr)
            conn.Open()
            comm = New SqlCommand(sql, conn)
            drSrv = comm.ExecuteReader
            Dim i As Integer = 0
            While drSrv.Read
                rtn.Append(drSrv.Item(0))
            End While
        Finally
            drSrv.Close()
            comm.Dispose()
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
            End If
            conn.Dispose()
        End Try
        Return rtn.ToString
    End Function

End Class
