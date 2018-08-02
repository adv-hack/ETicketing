Imports System.Xml
Imports System.IO

Public Class TalentDBAdmin

    Private _conString As String = ""
    Private _xmlFile As String = ""
    Private _xmlPath As String = ""
    Private _businessUnit As String = "*ALL"
    Private _partner As String = "*ALL"
    Private _language As String = "ENG"
    Private _currentVersion As New WebSiteVersion
    Private _newVersion As New WebSiteVersion
    Private _client As String = ""

    Public Property ConnectionString() As String
        Get
            Return _conString
        End Get
        Set(ByVal value As String)
            _conString = value
        End Set
    End Property

    Public Property XmlFile() As String
        Get
            Return _xmlFile
        End Get
        Set(ByVal value As String)
            _xmlFile = value
        End Set
    End Property
    Public Property XmlPath() As String
        Get
            Return _xmlPath
        End Get
        Set(ByVal value As String)
            _xmlPath = value
        End Set
    End Property

    Public Property Client() As String
        Get
            Return _client
        End Get
        Set(ByVal value As String)
            _client = value
        End Set
    End Property

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property
    Public Property Language() As String
        Get
            Return _language
        End Get
        Set(ByVal value As String)
            _language = value
        End Set
    End Property

    Public Property CurrentVersion() As WebSiteVersion
        Get
            Return _currentVersion
        End Get
        Set(ByVal value As WebSiteVersion)
            _currentVersion = value
        End Set
    End Property

    Public Property NewVersion() As WebSiteVersion
        Get
            Return _newVersion
        End Get
        Set(ByVal value As WebSiteVersion)
            _newVersion = value
        End Set
    End Property

    Public Sub ProcessDirectory()


        Try

            'Read in all the file names in the ifs directory
            Dim files() As String = Directory.GetFiles(_xmlPath, "*.xml")

            'Process each file name individually
            Dim iLoopCount As Integer = 0
            Do While iLoopCount < files.Length


                'Update the table
                _xmlFile = files(iLoopCount).ToString
                UpdateTable()

                'Process the next file
                iLoopCount = iLoopCount + 1
            Loop
            DBVersionRecorder("ProcessDirectory")

        Catch ex As Exception
            Dim tta As New TalentTableAdapter("")
            tta.WriteToLogFile("ProcessDirectoryError", ex.Message, "DBA0007", "")
        End Try

    End Sub

    Public Sub ProcessFile()
        Try
            If _xmlPath.EndsWith("\") Then
                XmlFile = _xmlPath + _xmlFile
            Else
                XmlFile = _xmlPath + "\" + _xmlFile
            End If
            UpdateTable()
            DBVersionRecorder("ProcessFile")
        Catch ex As Exception
            Dim tta As New TalentTableAdapter("")
            tta.WriteToLogFile("ProcessDirectoryError", ex.Message, "DBA0008", "")
        End Try
    End Sub


    Public Sub UpdateTable()

        Dim tta As New TalentTableAdapter(ConnectionString)
        If Not tta.HasError Then

            tta.Client = _client

            'Retrieve the xml document
            Dim XmlDoc As New XmlDocument
            '--------------------------------------------------------------------------
            Dim Node1 As XmlNode
            '-------------------------------------------------------------------------------------
            '   Loop through the xml configuration document for this table
            Try
                XmlDoc.Load(XmlFile)
                For Each Node1 In XmlDoc.SelectSingleNode("//EbusinessConfiguration").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TableDetails" : TableDetails(tta, Node1)
                        Case Is = "TableMods" : TableMods(tta, Node1)
                        Case Is = "TableData" : TableData(tta, Node1)
                    End Select
                Next Node1

            Catch ex As Exception
                tta.WriteToLogFile("UpdateTableError", ex.Message, "DBA0001", "")
            End Try
            tta.CloseConnection()
        End If
    End Sub

    Private Sub TableDetails(ByRef tta As TalentTableAdapter, ByVal Node1 As XmlNode)
        Dim Node2 As XmlNode
        Dim nodeName As String = String.Empty
        Dim Key As String = String.Empty
        Try
            For Each Node2 In Node1.ChildNodes
                nodeName = Node2.Name
                Key = String.Empty
                Select Case Node2.Name
                    Case Is = "Table"
                        tta.Table = Node2.InnerText
                    Case Is = "Keys"
                        'Extract the key information
                        Dim Node3 As XmlNode
                        For Each Node3 In Node2.ChildNodes
                            Key = Node3.InnerText
                            If ValidKeyVersion(tta, Node3) Then
                                tta.Keys.Add(Node3.InnerText)
                            End If
                        Next Node3
                End Select
            Next Node2
        Catch ex As Exception
            tta.WriteToLogFile("TableDetailsError", ex.Message, "DBA0002", nodeName)
        End Try
        
    End Sub

    Private Sub TableMods(ByRef tta As TalentTableAdapter, ByVal Node1 As XmlNode)
        Dim Node2 As XmlNode
        Dim nodeInnerText As String = String.Empty
        Try
            For Each Node2 In Node1.ChildNodes
                nodeInnerText = Node2.InnerText
                If ValidVersion(tta, Node2) Then
                    tta.ExecuteCommand(Node2.InnerText)
                End If
            Next Node2
        Catch ex As Exception
            tta.WriteToLogFile("TableModsError", ex.Message, "DBA0003", nodeInnerText)
        End Try
        

        'All of the db updates have happened so retrieve the column attributes
        tta.SetColumnAttributes()

    End Sub

    Private Sub TableData(ByRef tta As TalentTableAdapter, ByVal Node1 As XmlNode)
        Dim Node2 As XmlNode
        Dim nodeAction As String = String.Empty
        Dim nodeField As String = String.Empty
        Try
            For Each Node2 In Node1.ChildNodes
                nodeAction = Node2.Attributes("Action").Value
                nodeField = String.Empty

                'Retrieve the version
                If ValidVersion(tta, Node2) Then

                    'Perform the required action 
                    Select Case Node2.Attributes("Action").Value
                        Case Is = "Insert"
                            'Extract the field information
                            Dim info As New Generic.Dictionary(Of String, String)
                            Dim Node3 As XmlNode
                            For Each Node3 In Node2.ChildNodes
                                nodeField = Node3.Attributes("Field").Value
                                info.Add(Node3.Attributes("Field").Value, ReplaceStrings(tta, Node3.InnerText))
                            Next Node3

                            tta.InsertRecord(info)

                        Case Is = "Update"
                            'Extract the field information
                            Dim info As New Generic.Dictionary(Of String, String)
                            Dim keys As New Generic.Dictionary(Of String, String)

                            Dim Node3 As XmlNode
                            For Each Node3 In Node2.ChildNodes

                                Select Case Node3.Name
                                    Case "Keys"
                                        For Each node4 As XmlNode In Node3.ChildNodes
                                            keys.Add(node4.Attributes("Field").Value, ReplaceStrings(tta, node4.InnerText))
                                        Next
                                    Case "DataItem"
                                        info.Add(Node3.Attributes("Field").Value, ReplaceStrings(tta, Node3.InnerText))
                                        nodeField = Node3.Attributes("Field").Value
                                End Select
                            Next Node3

                            tta.UpdateRecord(keys, info)

                        Case Is = "Delete"
                            'Extract the field information
                            Dim keys As New Generic.Dictionary(Of String, String)

                            Dim Node3 As XmlNode
                            For Each Node3 In Node2.ChildNodes

                                Select Case Node3.Name
                                    Case "Keys"
                                        For Each node4 As XmlNode In Node3.ChildNodes
                                            keys.Add(node4.Attributes("Field").Value, ReplaceStrings(tta, node4.InnerText))
                                        Next
                                End Select
                            Next Node3

                            tta.DeleteRecord(keys)

                    End Select

                End If

            Next Node2
        Catch ex As Exception
            tta.WriteToLogFile("TableDataError", ex.Message, "DBA0004", nodeAction & nodeField)
        End Try
        
    End Sub

    Private Function ValidVersion(ByRef tta As TalentTableAdapter, ByVal Node1 As XmlNode) As Boolean

        Dim configVersion As New WebSiteVersion
        Try

            'Retrieve the version for this config
            configVersion.Version = CType(Node1.Attributes("Version").Value, Integer)
            configVersion.SubVersion = CType(Node1.Attributes("SubVersion").Value, Integer)
            If Node1.Attributes("PTF").Value.Trim = "" Then
                configVersion.PTF = 0
            Else
                configVersion.PTF = CType(Node1.Attributes("PTF").Value, Integer)
            End If
            configVersion.Client = Node1.Attributes("Client").Value

            'Client specific mods need to be an exact match
            If configVersion.Client.Trim <> "" Then
                If configVersion.Version <> _newVersion.Version Or _
                    configVersion.SubVersion <> _newVersion.SubVersion Or _
                    configVersion.PTF <> _newVersion.PTF Or _
                    configVersion.Client <> _newVersion.Client Then
                    Return False
                End If
            End If

            'Check Version
            If configVersion.Version > _newVersion.Version Or configVersion.Version < _currentVersion.Version Then
                Return False
            End If

            'Check Sub version
            If (configVersion.Version = _newVersion.Version AndAlso configVersion.SubVersion > _newVersion.SubVersion) _
                Or (configVersion.Version = _currentVersion.Version AndAlso configVersion.SubVersion < _currentVersion.SubVersion) Then
                Return False
            End If

            'Check PTF
            If (configVersion.Version = _newVersion.Version AndAlso configVersion.SubVersion = _newVersion.SubVersion AndAlso configVersion.PTF > _newVersion.PTF) _
                Or (configVersion.Version = _currentVersion.Version AndAlso configVersion.SubVersion = _currentVersion.SubVersion AndAlso configVersion.PTF <= _currentVersion.PTF) Then
                Return False
            End If

            'Success
            Return True
        Catch ex As Exception
            tta.WriteToLogFile("ValidVersionError", ex.Message, "DBA0005", configVersion.Version.ToString & configVersion.SubVersion.ToString & configVersion.PTF.ToString & configVersion.Client)
            Return False
        End Try

    End Function

    Private Function ValidKeyVersion(ByRef tta As TalentTableAdapter, ByVal Node1 As XmlNode) As Boolean

        Dim configVersion As New WebSiteVersion
        Try

            'Retrieve the version for this config
            configVersion.Version = CType(Node1.Attributes("Version").Value, Integer)
            configVersion.SubVersion = CType(Node1.Attributes("SubVersion").Value, Integer)
            If Node1.Attributes("PTF").Value.Trim = "" Then
                configVersion.PTF = 0
            Else
                configVersion.PTF = CType(Node1.Attributes("PTF").Value, Integer)
            End If
            configVersion.Client = Node1.Attributes("Client").Value

            'Client specific mods need to be an exact match
            If configVersion.Client.Trim <> "" Then
                If configVersion.Version <> _newVersion.Version Or _
                    configVersion.SubVersion <> _newVersion.SubVersion Or _
                    configVersion.PTF <> _newVersion.PTF Or _
                    configVersion.Client <> _newVersion.Client Then
                    Return False
                End If
            End If

            'Check Version
            If configVersion.Version > _newVersion.Version Or _
                (configVersion.Version = _newVersion.Version AndAlso configVersion.SubVersion > _newVersion.SubVersion) Or _
                (configVersion.Version = _newVersion.Version AndAlso configVersion.SubVersion = _newVersion.SubVersion AndAlso configVersion.PTF > _newVersion.PTF) Then
                Return False
            End If

            'Success
            Return True
        Catch ex As Exception
            tta.WriteToLogFile("ValidKeyVersionError", ex.Message, "DBA0007", configVersion.Version.ToString & configVersion.SubVersion.ToString & configVersion.PTF.ToString & configVersion.Client)
            Return False
        End Try

    End Function

    Private Function ReplaceStrings(ByRef tta As TalentTableAdapter, ByVal inputString As String) As String
        Try

            Dim cr As Char = Chr(13)
            Dim lf As Char = Chr(10)

            'Strip any blanks or carriage returns from the front and end of the string of the 
            Dim inputSaved As String = ""
            Do While inputSaved <> inputString
                inputSaved = inputString
                inputString = inputString.TrimStart(cr, "")
                inputString = inputString.TrimEnd(lf, "")
                inputString = inputString.TrimStart(lf, "")
                inputString = inputString.TrimEnd(cr, "")
                inputString = inputString.TrimStart(" ", "")
                inputString = inputString.TrimEnd(" ", "")
            Loop

            'Replace the pre-defined replace functions
            inputString = inputString.Replace("@@@Business Unit@@@", _businessUnit)
            inputString = inputString.Replace("@@@Partner@@@", _partner)
            inputString = inputString.Replace("@@@Language@@@", _language)
            inputString = inputString.Replace("@@@vbCrLf@@@", vbCrLf)
            inputString = inputString.Replace("@@@space@@@", " ")
        Catch ex As Exception
            tta.WriteToLogFile("ReplaceStrings", ex.Message, "DBA0006", inputString)
        End Try
        Return inputString
    End Function

    Private Sub DBVersionRecorder(ByVal processType As String)

        Dim tta As New TalentTableAdapter(ConnectionString)
        Dim statement As String = String.Empty

        Try

            Dim fullVersion As String = _newVersion.Version & "_" & _newVersion.SubVersion & "_" & _newVersion.PTF
            statement = "INSERT INTO TBL_VERSION_DEPLOYED (PROCESSTYPE, VERSION, DATEUPDATED) VALUES ('" & processType & "','" & fullVersion & "'," & "getdate())"
            tta.ExecuteCommand(statement)

        Catch ex As Exception
            tta.WriteToLogFile("DBVersionRecorderError", ex.Message, "DBA0009", statement)
        End Try
    End Sub



End Class
