Imports Microsoft.VisualBasic

Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Data

Public Class Zip
    
    Private ColFiles As New CollectionOfFiles
    Private strParentDirectory As String
    Public Sub New()
    End Sub
    Public Class CollectionOfFiles : Inherits CollectionBase
        Public Sub Add(ByVal FileOrFolder As objectFileOrFolder)
            Me.List.Add(FileOrFolder)
        End Sub
        Default Public Property Item(ByVal Index As Integer) As objectFileOrFolder
            Get
                Return DirectCast(Me.List(Index), objectFileOrFolder)
            End Get
            Set(ByVal FileOrFolder As objectFileOrFolder)
                Me.List(Index) = FileOrFolder
            End Set
        End Property
    End Class
    Public Class objectFileOrFolder
        Public Path As String
        Public PathWithoutParentDirec As String
        Public Type As New EnumObjTypes
    End Class
    Public Enum EnumObjTypes
        File
        Folder
    End Enum
    Public Sub ZipOneFile(ByVal strSourceFile As String, ByVal strDestinationFile As String)
        ColFiles.Clear()
        Dim objFileOrFolder As New objectFileOrFolder
        objFileOrFolder.Path = strSourceFile
        objFileOrFolder.PathWithoutParentDirec = Path.GetFileName(strSourceFile)
        objFileOrFolder.Type = EnumObjTypes.File
        ColFiles.Add(objFileOrFolder)
        ZipTheFilesTo(strDestinationFile)
    End Sub
    Public Sub ZipThisArrayOfFiles(ByVal strPathsToFiles() As String, ByVal strPathToDestFile As String)
        Dim NoFolders() As String = Nothing
        ZipThisArrayOfFilesAndThisArrayOfFolders(strPathsToFiles, NoFolders, strPathToDestFile)
    End Sub
    Public Sub ZipThisArrayOfFolders(ByVal strPathsToFolders() As String, ByVal strPathToDestFile As String)
        Dim NoFiles() As String = Nothing
        ZipThisArrayOfFilesAndThisArrayOfFolders(NoFiles, strPathsToFolders, strPathToDestFile)
    End Sub
    Public Sub ZipThisArrayOfFilesAndThisArrayOfFolders(ByVal strPathsToFiles() As String, ByVal strPathsToFolders() As String, ByVal strPathToDestFile As String)
        ColFiles.Clear()
        If Not IsNothing(strPathsToFiles) Then
            For Each strFile As String In strPathsToFiles
                Dim objFileOrFolder As New objectFileOrFolder
                objFileOrFolder.Type = EnumObjTypes.File
                objFileOrFolder.Path = strFile
                objFileOrFolder.PathWithoutParentDirec = Path.GetFileName(strFile)
                ColFiles.Add(objFileOrFolder)
            Next
        End If
        If Not IsNothing(strPathsToFolders) Then
            For Each str As String In strPathsToFolders
                Dim strFolder As String = str.TrimEnd("\")
                Dim objFileOrFolder As New objectFileOrFolder
                objFileOrFolder.Path = strFolder
                objFileOrFolder.Type = EnumObjTypes.Folder
                ColFiles.Add(objFileOrFolder) 'insures that the outermost folder is created/added to the list. 
                Me.strParentDirectory = System.IO.Path.GetDirectoryName(strFolder)
                objFileOrFolder.PathWithoutParentDirec = Mid(strFolder, 1 + strParentDirectory.Length)
                For Each objFileOrFolder In GetAListOfFilesFromThisFolder(strFolder)
                    ColFiles.Add(objFileOrFolder)
                Next
            Next
        End If
        ZipTheFilesTo(strPathToDestFile)
    End Sub
    Private Function funcRemoveDirectory() As String
        'removes the whole 
        Return ""
    End Function
    Public Sub ZipThisFolder(ByVal strSourceFolder As String, ByVal strPathToDestFile As String)
        ColFiles.Clear()
        Dim objFileOrFolder As New objectFileOrFolder
        objFileOrFolder.Path = strSourceFolder
        objFileOrFolder.Type = EnumObjTypes.Folder
        ColFiles.Add(objFileOrFolder) 'insures that the outermost folder is created/added to the list. 
        Me.strParentDirectory = System.IO.Path.GetDirectoryName(strSourceFolder)
        objFileOrFolder.PathWithoutParentDirec = Mid(strSourceFolder, 1 + strParentDirectory.Length)
        For Each objFileOrFolder In GetAListOfFilesFromThisFolder(strSourceFolder)
            ColFiles.Add(objFileOrFolder)
        Next
        ZipTheFilesTo(strPathToDestFile)
    End Sub
    Private Function GetAListOfFilesFromThisFolder(ByVal strSourceFolder As String) As CollectionOfFiles
        Dim FilesOrFolders As New CollectionOfFiles
        'recursively forms a collection of filenames and subdirectory names. 
        For Each strFile As String In Directory.GetFiles(strSourceFolder)
            Dim FileOrFolder As New objectFileOrFolder
            FileOrFolder.Path = strFile
            FileOrFolder.PathWithoutParentDirec = Mid(FileOrFolder.Path, 1 + strParentDirectory.Length)
            FileOrFolder.Type = EnumObjTypes.File
            FilesOrFolders.Add(FileOrFolder)
        Next
        For Each strSubDir As String In Directory.GetDirectories(strSourceFolder)
            Dim FileOrFolder As New objectFileOrFolder
            FileOrFolder.Path = strSubDir
            FileOrFolder.PathWithoutParentDirec = Mid(FileOrFolder.Path, 1 + strParentDirectory.Length)
            FileOrFolder.Type = EnumObjTypes.Folder
            FilesOrFolders.Add(FileOrFolder)
            For Each objFileOrFolder As objectFileOrFolder In GetAListOfFilesFromThisFolder(FileOrFolder.Path)
                FilesOrFolders.Add(objFileOrFolder)
            Next
        Next
        Return FilesOrFolders
    End Function

    Private Sub ZipTheFilesTo(ByVal strPathToDestFile As String)
        Dim DestDir As String = Path.GetDirectoryName(strPathToDestFile)
        If Not Directory.Exists(DestDir) Then Directory.CreateDirectory(DestDir)
        Dim streamZipOutputStream As New ZipOutputStream(File.Create(strPathToDestFile))
        streamZipOutputStream.SetLevel(5)  REM Compression Level: 0-9
        Dim objZipEntry As ZipEntry
        For Each FileOrFolder As objectFileOrFolder In ColFiles
            If FileOrFolder.Type = EnumObjTypes.Folder Then 'use a slash to mark it as a folder 
                objZipEntry = New ZipEntry(FileOrFolder.PathWithoutParentDirec & "/") 'distinct from files. 
                objZipEntry.DateTime = DateTime.Now
                streamZipOutputStream.PutNextEntry(objZipEntry)
            Else
                objZipEntry = New ZipEntry(FileOrFolder.PathWithoutParentDirec)
                objZipEntry.DateTime = DateTime.Now
                Dim strmFile As FileStream = File.OpenRead(FileOrFolder.Path) 'no slash, threfore a file. 
                Dim byteBuffer(strmFile.Length - 1) As Byte
                strmFile.Read(byteBuffer, 0, byteBuffer.Length)
                objZipEntry.Size = strmFile.Length
                strmFile.Close()
                streamZipOutputStream.PutNextEntry(objZipEntry)
                streamZipOutputStream.Write(byteBuffer, 0, byteBuffer.Length)
            End If
        Next
        streamZipOutputStream.Finish()
        streamZipOutputStream.Close()
        ColFiles.Clear()
    End Sub
    Public Sub UnzipThisStream(ByVal InpStream As System.IO.Stream, ByVal DestFolder As String)
        Dim ZipInputStream1 As New ZipInputStream(InpStream)
        Dim Entry As ZipEntry
        Do
            Entry = ZipInputStream1.GetNextEntry
            If Entry Is Nothing Then Exit Do
            Dim DirName As String = Path.GetDirectoryName(Entry.Name)
            Directory.CreateDirectory(DestFolder & "\" & DirName) 'creates subDirs as necessary to create this dir path. 
            If Not Entry.IsDirectory Then
                Dim FileName As String = DestFolder & "\" & Entry.Name
                Dim streamWriter1 As FileStream = File.Create(FileName)
                Dim Size As Integer = 2048
                Dim buffer(2047) As Byte
                Do
                    Size = ZipInputStream1.Read(buffer, 0, buffer.Length)
                    If Size > 0 Then
                        streamWriter1.Write(buffer, 0, Size)
                    Else : Exit Do
                    End If
                Loop
                streamWriter1.Close()
            End If
        Loop
        ZipInputStream1.Close()

    End Sub

    Public Sub UnZipThisFile(ByVal strSource As String, ByVal DestFolder As String)
        Dim InputStream As System.IO.FileStream = File.OpenRead(strSource)
        UnzipThisStream(InputStream, DestFolder)
        InputStream.Close()
    End Sub
    Public Sub UnzipThisByteArray(ByVal zippedBytes() As Byte, ByVal DestFolder As String)
        Dim InpStream As New System.IO.MemoryStream(zippedBytes)
        UnzipThisStream(InpStream, DestFolder)
        InpStream.Close()
    End Sub

    Public Sub ExtractThisFile(ByVal strFileToExtract As String, ByVal strSource As String, ByVal strDestFileName As String)
        strFileToExtract = strFileToExtract.Trim("\ ".ToCharArray).ToLower
        Dim InputStream As New ZipInputStream(File.OpenRead(strSource))
        Dim Entry As ZipEntry
        Do
            Entry = InputStream.GetNextEntry
            If Entry Is Nothing Then Exit Do
            Dim EntryNameToCompare As String = Entry.Name.Trim("\/ ".ToCharArray).ToLower
            If Not Entry.IsDirectory AndAlso EntryNameToCompare = strFileToExtract Then
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(strDestFileName))
                Dim streamWriter1 As FileStream = File.Create(strDestFileName)
                Dim Size As Integer = 2048
                Dim buffer(2047) As Byte
                Do
                    Size = InputStream.Read(buffer, 0, buffer.Length)
                    If Size > 0 Then
                        streamWriter1.Write(buffer, 0, Size)
                    Else : Exit Do
                    End If
                Loop
                streamWriter1.Close()
            End If
        Loop
        InputStream.Close()
    End Sub

    Public Sub ExtractThisFolder(ByVal strFolderToExtract As String, ByVal strSource As String, ByVal strDestFolder As String)
        If Not Directory.Exists(strDestFolder) Then Directory.CreateDirectory(strDestFolder)
        strFolderToExtract = strFolderToExtract.Trim("\ ".ToCharArray).ToLower
        Dim InputStream As New ZipInputStream(File.OpenRead(strSource))
        Dim Entry As ZipEntry
        Do
            Entry = InputStream.GetNextEntry
            If Entry Is Nothing Then Exit Do
            If Entry.Name.Length < 2 Then GoTo NxEnt
            Dim EntryNameToCompare As String = Entry.Name.Trim("\/ ".ToCharArray).ToLower()
            If EntryNameToCompare.IndexOf(strFolderToExtract) <> 0 Then GoTo NxEnt 'next Entry
            Directory.CreateDirectory(strDestFolder & "\" & Path.GetDirectoryName(Entry.Name)) 'creates subDirs as necessary to create this dir path. 
            If Not Entry.IsDirectory Then
                Dim FileName As String = strDestFolder & "\" & Entry.Name
                Dim streamWriter1 As FileStream = File.Create(FileName)
                Dim Size As Integer = 2048
                Dim buffer(2047) As Byte
                Do
                    Size = InputStream.Read(buffer, 0, buffer.Length)
                    If Size > 0 Then
                        streamWriter1.Write(buffer, 0, Size)
                    Else : Exit Do
                    End If
                Loop
                streamWriter1.Close()
            End If
NxEnt:  Loop
        InputStream.Close()
    End Sub

    Public Function funcListOfFilesAsJaggedArray(ByVal PathToZippedFile As String) As String()()
        Me.subPopulateTheFileCollection(PathToZippedFile)
        Dim TotalRows As Integer = ColFiles.Count
        Dim table(ColFiles.Count - 1)() As String
        For rowIndex As Integer = 0 To TotalRows - 1
            ReDim table(rowIndex)(1)
            table(rowIndex)(0) = ColFiles(rowIndex).Type.ToString
            table(rowIndex)(1) = ColFiles(rowIndex).PathWithoutParentDirec
        Next
        Return table
    End Function

    Private Sub subPopulateTheFileCollection(ByVal PathToZippedFile As String)
        Me.ColFiles = New CollectionOfFiles
        Dim InputStream As New ZipInputStream(File.OpenRead(PathToZippedFile))
        Dim Entry As ZipEntry
        Do
            Entry = InputStream.GetNextEntry
            If Entry Is Nothing Then Exit Do
            If Entry.Name.Length < 2 Then GoTo NxEnt
            Dim objFile As New objectFileOrFolder
            If Entry.IsDirectory Then objFile.Type = EnumObjTypes.Folder _
            Else objFile.Type = EnumObjTypes.File
            objFile.PathWithoutParentDirec = Entry.Name.Replace("/", "\").TrimEnd("\")
            ColFiles.Add(objFile)
NxEnt:  Loop
        InputStream.Close()
    End Sub


    Public Sub ZipThisByteArrayToOutputStream(ByVal bytesIn() As Byte, ByRef OutputStream As Stream, _
    ByVal NameOfFile As String)
        Dim streamZipOutputStream As New ZipOutputStream(OutputStream)
        streamZipOutputStream.SetLevel(5)  REM Compression Level: 0-9
        Dim objZipEntry As New ZipEntry(NameOfFile)
        objZipEntry.DateTime = DateTime.Now
        objZipEntry.Size = bytesIn.Length
        streamZipOutputStream.PutNextEntry(objZipEntry)
        streamZipOutputStream.Write(bytesIn, 0, bytesIn.Length)
        streamZipOutputStream.Finish()
        streamZipOutputStream.Close()
        OutputStream.Close()
    End Sub
    Public Sub ZipThisByteArrayToThisFile(ByVal bytesIn() As Byte, ByVal PathToDestFile As String)
        Dim fs As New FileStream(PathToDestFile, FileMode.Create)
        Dim NameOfFile As String = System.IO.Path.GetFileName(PathToDestFile)
        ZipThisByteArrayToOutputStream(bytesIn, fs, NameOfFile)
    End Sub
    Public Function ZipThisByteArrayToDestByteArray(ByVal bytesIn() As Byte, _
    ByVal NameOfDestFile As String) As Byte()
        Dim bytesOut(bytesIn.Length - 1) As Byte
        Dim OutPutStream As New MemoryStream(bytesOut)
        ZipThisByteArrayToOutputStream(bytesIn, OutPutStream, NameOfDestFile)
        Return bytesOut
    End Function
End Class
