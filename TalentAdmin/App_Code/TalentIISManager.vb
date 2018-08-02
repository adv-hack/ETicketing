Imports System.DirectoryServices
Imports System.Management

Public Class TalentIISManager

    Private Structure SiteBinding
        Public HostHeader As String
        Public Port As String
        Public IP As String
    End Structure

    Private Structure SiteProperty
        Public PropertyName As String
        Public PropertyValue As Object
        Public AddRange As Boolean
    End Structure

    Private _siteId As String = String.Empty
    Private _webSiteName As String = String.Empty
    Private _webSitePath As String = String.Empty
    Private _siteBindings As New ArrayList
    Private _rootProperties As New ArrayList
    Private _siteProperties As New ArrayList
    Private _appPoolProperties As New ArrayList
    Private _virDirProperties As New ArrayList
    Private _dirProperties As New ArrayList
    Private _aspVersion As String = String.Empty
    Private _applicationPoolName As String = String.Empty
    Private _virDirName As String = String.Empty
    Private _virDirCreate As Boolean = True
    Private _dirName As String = String.Empty
    Private _sslPfxPath As String = String.Empty
    Private _sslPassword As String = String.Empty
    Private _success As Boolean = True
    Private _sendMessages As Boolean = False

    Public Property WebSiteName() As String
        Get
            Return _webSiteName
        End Get
        Set(ByVal value As String)
            _webSiteName = value
        End Set
    End Property

    Public Property WebSitePath() As String
        Get
            Return _webSitePath
        End Get
        Set(ByVal value As String)
            _webSitePath = value
        End Set
    End Property

    Public Property AspVersion() As String
        Get
            Return _aspVersion
        End Get
        Set(ByVal value As String)
            _aspVersion = value
        End Set
    End Property

    Public Property ApplicationPoolName() As String
        Get
            Return _applicationPoolName
        End Get
        Set(ByVal value As String)
            _applicationPoolName = value
        End Set
    End Property

    Public Property VirtualDirectoryName() As String
        Get
            Return _virDirName
        End Get
        Set(ByVal value As String)
            _virDirName = value
        End Set
    End Property

    Public Property VirDirCreate() As String
        Get
            Return _virDirCreate
        End Get
        Set(ByVal value As String)
            _virDirCreate = value
        End Set
    End Property

    Public Property SslPfxPath() As String
        Get
            Return _sslPfxPath
        End Get
        Set(ByVal value As String)
            _sslPfxPath = value
        End Set
    End Property

    Public Property SslPassword() As String
        Get
            Return _sslPassword
        End Get
        Set(ByVal value As String)
            _sslPassword = value
        End Set
    End Property
    Public Property DirectoryName() As String
        Get
            Return _dirName
        End Get
        Set(ByVal value As String)
            _dirName = value
        End Set
    End Property

    Public ReadOnly Property Success() As Boolean
        Get
            Return _success
        End Get
    End Property

    Public Property SendMessages() As Boolean
        Get
            Return _sendMessages
        End Get
        Set(ByVal value As Boolean)
            _sendMessages = value
        End Set
    End Property

    Property SiteID() As String
        Get
            Return _siteId
        End Get
        Set(ByVal value As String)
            _siteId = value
        End Set
    End Property

    Public Sub SetSiteProperty(ByVal Name As String, ByVal Value As Object, ByVal Type As String, Optional ByVal AddRange As Boolean = False)

        Try
            'Create the structure
            Dim newProperty As New SiteProperty
            newProperty.PropertyName = Name
            newProperty.PropertyValue = Value
            newProperty.AddRange = AddRange

            'Add to the Array list
            Select Case Type
                Case Is = "Site"
                    _siteProperties.Add(newProperty)
                Case Is = "Root"
                    _rootProperties.Add(newProperty)
                Case Is = "AppPool"
                    _appPoolProperties.Add(newProperty)
                Case Is = "VirDir"
                    _virDirProperties.Add(newProperty)
                Case Is = "Dir"
                    _dirProperties.Add(newProperty)
            End Select
        Catch ex As Exception
            SendMessage("SetSiteProperty Error: Name=" & Name & " Value=" & Value & " Type=" & Type & " Error=" & ex.Message, True)
        End Try

    End Sub

    Public Sub SetSiteBinding(ByVal IP As String, ByVal Port As String, ByVal HostHeader As String)

        Try
            Dim binding As SiteBinding
            binding.IP = IP
            binding.Port = Port
            binding.HostHeader = HostHeader
            _siteBindings.Add(binding)
        Catch ex As Exception
            SendMessage("SetSiteBinding Error: IP=" & IP & " Port=" & Port & " Host=" & HostHeader & " Error=" & ex.Message, True)
        End Try
       
    End Sub

    Public Function GetSiteBinding() As Object()

        Dim bindingObj() As Object
        bindingObj = New Object() {}

        Try
            ' Set the array elements 
            If _siteBindings.Count > 0 Then
                ReDim bindingObj(_siteBindings.Count - 1)
            End If

            ' Construct the binding arrays
            Dim index As Integer = 0
            For Each binding As SiteBinding In _siteBindings
                bindingObj(index) = String.Format("{0}:{1}:{2}", binding.IP, binding.Port, binding.HostHeader)
                index += 1
            Next

        Catch ex As Exception
            SendMessage("GetSiteBinding Error: " & ex.Message, True)
        End Try
        
        Return bindingObj

    End Function

    Public Sub CreateWebSite()

        Try

            'Create the web site
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC")
            _siteId = _ServiceEntry1.Invoke("CreateNewSite", _webSiteName, GetSiteBinding, _webSitePath)

            'Create a directory entry for both the web site and the root
            Dim ws = New DirectoryEntry(_ServiceEntry1.Path & "\" & _siteId)
            Dim wr = New DirectoryEntry(CStr(ws.Path & "\Root"))

            'Set the AspVersion
            SetScriptMaps1(wr.Path, _aspVersion)

            'Set Root Properties 
            SetProperties(_rootProperties, wr)

            'Set site Properties 
            SetProperties(_siteProperties, ws)

        Catch ex As Exception
            SendMessage("CreateWebSite Error: " & _webSiteName & " - " & ex.Message, True)
        Finally
            _siteProperties.Clear()
            _siteBindings.Clear()
        End Try

    End Sub

    Private Sub SetProperties(ByVal Properties As ArrayList, ByVal DirEntry As DirectoryEntry)

        Try
            For Each struct As SiteProperty In Properties
                SetProperty(struct.PropertyName, struct.PropertyValue, DirEntry, struct.AddRange)
            Next

            DirEntry.CommitChanges()
        Catch ex As Exception
            SendMessage("SetProperties Error: " & ex.Message, True)
        End Try

    End Sub

    Private Sub SetProperty(ByVal PropertyName As String, _
                            ByVal PropertyValue As Object, _
                            ByVal DirEntry As DirectoryEntry, _
                            Optional ByVal AddRange As Boolean = False)

        Try
            If DirEntry.Properties(PropertyName).Value Is Nothing Then
                DirEntry.Properties(PropertyName).Value = PropertyValue
            Else
                If AddRange = True Then
                    DirEntry.Properties(PropertyName).AddRange(PropertyValue)
                Else
                    DirEntry.Properties(PropertyName).Item(0) = PropertyValue
                End If
            End If
        Catch ex As Exception
            SendMessage("SetProperty Error: " & PropertyName & " - " & ex.Message, True)
        End Try

    End Sub

    Private Sub SetScriptMaps1(ByVal ApplicationPath As String, ByVal ASPNETVersion As String)

        Try

            Dim SystemRoot = System.Environment.GetEnvironmentVariable("SystemRoot")
            Dim AppRoot = ApplicationPath.Replace("IIS://LocalHost/", String.Empty)

            Using p As New System.Diagnostics.Process

                p.StartInfo.FileName = String.Format("{0}\Microsoft.NET\Framework\v{1}\aspnet_regiis.exe", SystemRoot, ASPNETVersion)
                p.StartInfo.Arguments = String.Format("-s {0}", AppRoot)
                p.StartInfo.CreateNoWindow = True
                p.StartInfo.UseShellExecute = False

                p.Start()
                p.WaitForExit()

            End Using

        Catch ex As Exception
            SendMessage("SetScriptMaps1 Error: " & _webSiteName & " - " & ex.Message, True)
        End Try

    End Sub

    Public Sub CreateApplicationPool()

        Try

            'Create the directory entry for the application pools
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/AppPools")

            'Do we have an application pool already called this
            Dim foundApp As Boolean = False
            Dim appPools As DirectoryEntries = _ServiceEntry1.Children
            Dim app As DirectoryEntry
            For Each app In appPools
                If UCase(app.Name) = UCase(_applicationPoolName) Then
                    foundApp = True
                    Exit For
                End If
            Next

            'Create the application pool if it doesn't exist
            If Not foundApp Then
                Dim AppPool As DirectoryEntry = _ServiceEntry1.Invoke("Create", "IIsApplicationPool", _applicationPoolName)
                'Set The properties of the application pool
                SetProperties(_appPoolProperties, AppPool)
            End If

        Catch ex As Exception
            SendMessage("CreateApplicationPool Error: " & _applicationPoolName & " - " & ex.Message, True)
        Finally
            _appPoolProperties.Clear()
        End Try

    End Sub

    Public Sub CreateVirtualDirectory()

        Try

            'Create the directory entry for the virtual directory
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/" & _siteId & "/Root")

            'Does the virtual directory already exist
            Dim foundDir As Boolean = False
            For Each VD As DirectoryEntry In _ServiceEntry1.Children
                If UCase(VD.Name) = UCase(_virDirName) Then
                    foundDir = True
                    Exit For
                End If
            Next VD

            'Create the virtual directory if it doesn't exist
            If Not foundDir Then
                Dim VDir As DirectoryEntry = _ServiceEntry1.Children.Add(_virDirName, "IIsWebVirtualDir")

                'Set the properties of the virtual directory
                If _virDirCreate Then
                    VDir.Invoke("AppCreate", True)
                End If
                SetProperties(_virDirProperties, VDir)
            End If

        Catch ex As Exception
            SendMessage("CreateVirtualDirectory Error: " & _virDirName & " - " & ex.Message, True)
        Finally
            _virDirProperties.Clear()
            _virDirCreate = True
        End Try

    End Sub

    Private Sub ReadProperties(ByVal DirEntry As DirectoryEntry)

        Try
            Dim PN As String
            For Each PN In DirEntry.Properties.PropertyNames
                Console.Write(PN)
                Dim pvc As PropertyValueCollection = DirEntry.Properties(PN)
                Dim value As Object
                For Each value In pvc
                    Console.WriteLine(" " + value.ToString)
                Next
            Next
        Catch ex As Exception
            SendMessage("ReadProperties Error: " & _webSiteName & " - " & ex.Message, True)
        End Try

    End Sub

    Public Sub InstallSSLCertificate()

        Try

            Dim iisCertObj As Object
            iisCertObj = CreateObject("IIS.CertObj")
            iisCertObj.InstanceName = "W3SVC/" & _siteId
            iisCertObj.Import(_sslPfxPath, _sslPassword, True, True)

            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/" & _siteId)
            SetSiteProperty("SecureBindings", GetSiteBinding, "Site")
            SetProperties(_siteProperties, _ServiceEntry1)

        Catch ex As Exception
            SendMessage("InstallSSLCertificate Error: " & _webSiteName & " - " & ex.Message, True)
        End Try

    End Sub

    Public Function StartWebSite() As String
        Dim response As String = ""
        Try
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/" & _siteId)
            _ServiceEntry1.Invoke("start", New Object() {})
            response = "Success"
        Catch ex As Exception
            response = "StartWebSite Error: " & _webSiteName & " - " & ex.Message
        End Try
        Return response
    End Function

    Public Function StopWebSite() As String
        Dim response As String = ""
        Try
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/" & _siteId)
            _ServiceEntry1.Invoke("stop", New Object() {})
            response = "Success"
        Catch ex As Exception
            response = "StopWebSite Error: " & _webSiteName & " - " & ex.Message
        End Try
        Return response
    End Function

    Public Function StartAppPool() As String
        Dim response As String = ""
        Try
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/AppPools/" & _applicationPoolName)
            _ServiceEntry1.Invoke("start", New Object() {})
            response = "Success"
        Catch ex As Exception
            response = "StartAppPool Error: " & _applicationPoolName & " - " & ex.Message
        End Try
        Return response
    End Function

    Public Function StopAppPool() As String
        Dim response As String = ""

        Try
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/AppPools/" & _applicationPoolName)
            _ServiceEntry1.Invoke("stop", New Object() {})
            response = "Success"

        Catch ex As Exception
            response = "StopAppPool Error: " & _applicationPoolName & " - " & ex.Message
        End Try

        Return response
    End Function

    Public Sub WebSiteLoop()
        Try
            'Create the directory entry for the virtual directory
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/" & _siteId & "/Root")

            Dim foundDir As Boolean = False
            For Each VD As DirectoryEntry In _ServiceEntry1.Children
                Console.WriteLine(" " + VD.Name.ToString)
            Next VD
        Catch ex As Exception
            SendMessage("WebSiteLoop Error: " & _webSiteName & " - " & ex.Message, True)
        End Try
        
    End Sub

    Public Sub SetDirectoryProperties()

        Try
            'Create the directory in the metabase
            Dim _ServiceEntry1 As New DirectoryEntry("IIS://localhost/W3SVC/" & _siteId & "/Root")
            Dim _ServiceEntry2 As New DirectoryEntry
            _ServiceEntry2 = _ServiceEntry1.Children.Add(_dirName, "IIsWebDirectory")
            _ServiceEntry1.CommitChanges()

            'Set the properties
            SetProperties(_dirProperties, _ServiceEntry2)

        Catch ex As Exception
            SendMessage("SetDirectoryProperties Error: " & _dirName & " - " & ex.Message, True)
        Finally
            _dirProperties.Clear()
        End Try

    End Sub

    Public Sub SendMessage(ByVal message As String, ByVal errorFlag As Boolean)

        'Send the error message
        If _sendMessages Then
            HttpContext.Current.Response.Write(message.Trim & ";")
        End If

        'Set the global error flag
        If errorFlag = True Then
            _success = False
        End If

    End Sub

    Public Function GetWebsiteID(ByVal friendlyName As String) As String
        Dim mySiteId As String = ""

        Dim myRootEntry As New DirectoryEntry("IIS://localhost/W3SVC")
        Dim myEntries As DirectoryEntries = myRootEntry.Children

        For Each entry As DirectoryEntry In myEntries
            If entry.Properties("AppFriendlyName").ToString.ToLower = friendlyName.ToLower _
                OrElse entry.Properties("ServerComment").ToString.ToLower = friendlyName.ToLower Then
                mySiteId = entry.Name
                Exit For
            End If
        Next

        Return mySiteId
    End Function


End Class
