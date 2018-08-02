Imports System.io
Imports System.Xml
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Set Name Space
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'--------------------------------------------------------------------------------------------------

Partial Class setNamespace
    Inherits System.Web.UI.Page
    Protected Overrides Sub OnPreInit(ByVal e As EventArgs)

        ''Dim iPosition1 As Integer = 0
        ''Dim iPosition2 As Integer = 0
        ''Dim strBody As String = String.Empty

        ''iPosition1 = InStr(Page.body, "<input type=""hidden"" name=""__VIEWSTATE"" id=""__VIEWSTATE""")
        ''iPosition2 = InStr(iPosition1, Page.body, "</Div>")

        ''strBody = Page.body.remove(iPosition1, iPosition2 - iPosition1 - 6)

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then _
            namespaceText.Text = ConfigurationManager.AppSettings("XMLNamespace")
        namespaceText.Width = namespaceText.Text.Length * 7
        listFiles()
    End Sub
    Protected Function drawTable(ByVal t As Table, ByVal rows As Integer, ByVal cells As Integer) As Table
        '--------------------------------------------------------------------------------------------
        '   Create the table
        '
        For row As Integer = 0 To rows - 1
            t.Rows.Add(New TableRow)
            For cell As Integer = 0 To cells - 1
                t.Rows(row).Cells.Add(New TableCell)
                t.Rows(row).Cells(cell).BorderColor = Drawing.Color.Black
                t.Rows(row).Cells(cell).BorderStyle = BorderStyle.Dotted
                t.Rows(row).Cells(cell).BorderWidth = Unit.Pixel(1)
                t.Rows(row).Cells(cell).VerticalAlign = VerticalAlign.Top
            Next
        Next
        Return t
    End Function
    Protected Sub listFiles()
        Dim t As New Table
        t.ID = "filesTable"
        t.CellPadding = 2

        t = drawTable(t, 2, 3)
        '------------------------------------------------------------------------
        '   ADD THE WARNING ROW
        '
        t.Rows.AddAt(0, New TableRow)
        t.Rows(0).Cells.Add(New TableCell)
        t.Rows(0).Cells(0).ColumnSpan = 3
        t.Rows(0).Cells(0).Text = "<STRONG>The following files will be affected:</STRONG>"
        t.Rows(0).Cells(0).ForeColor = Drawing.Color.Red
        '------------------------------------------------------------------------
        '   Call generate and display the lists of files that will be affected
        '
        t = generateFileList(ConfigurationManager.AppSettings("WebServicesDir"), ".vb", t, 0)
        t = generateFileList(ConfigurationManager.AppSettings("XMLResponseDir"), ".xml", t, 1)
        t = generateFileList(ConfigurationManager.AppSettings("XSDResponseDir"), ".xsd", t, 2)
        '------------------------------------------------------------------------
        '   Add and bind the table to the page
        '
        Page.Controls.Add(t)
        Page.DataBind()
        '----------------------------------------
    End Sub
    Protected Function generateFileList(ByVal Location As String, ByVal fileExe As String, ByVal t As Table, ByVal column As Integer) As Table
        '--------------------------------------------------------------------------------------------
        '   Enter the directory as the column header
        '
        Dim files As FileInfo()
        Dim fileInf As FileInfo

        t.Rows(1).Cells(column).Text = Location & ": <br/>"

        '   Get the file list and add each to the table cell below the relevant header
        files = getFiles(Server.MapPath(dir), fileExe)
        For Each fileInf In files
            t.Rows(2).Cells(column).Text += fileInf.Name & "<br/>"
        Next

        Return t
    End Function
    Protected Sub runSetupBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles runSetupBtn.Click
        updateWebServices(ConfigurationManager.AppSettings("WebServicesDir"), ".vb", 1)
        updateXMLResponse(ConfigurationManager.AppSettings("XMLResponseDir"), ".xml")
        updateXMLResponse(ConfigurationManager.AppSettings("XSDResponseDir"), ".xsd")
    End Sub
    Protected Function getFiles(ByVal Location As String, ByVal fileExe As String) As FileInfo()
        '--------------------------------------------------------------------------------------------
        '   Get a list of files from the directory
        '
        Dim storefile As New DirectoryInfo(Location)
        Dim files As FileInfo()

        '   Populate the list with the files that match the pattern
        files = storefile.GetFiles("*" & fileExe)
        Return files
    End Function
    Protected Sub updateXMLResponse(ByVal xmlPath As String, ByVal fileExe As String)
        Dim files As FileInfo()
        Dim fileInf As FileInfo

        files = getFiles(Server.MapPath(xmlPath), fileExe)

        'Loop through all files
        For Each fileInf In files
            Dim fileName As String = fileInf.Name
            fileName = fileName.Remove(fileName.Length - 4, 4) & fileExe
            'Load the file into an XmlDocument object
            Dim xmlDoc As New XmlDocument
            xmlDoc.Load(fileInf.FullName)
            Dim xa As XmlAttribute

            'Loop through each attribute of the document's root element (the OrderResponse tag)
            For Each xa In xmlDoc.DocumentElement.Attributes

                'Match the names and replace the values accordingly
                Select Case xa.Name
                    Case "xmlns"
                        xa.Value = namespaceText.Text
                    Case "xsi:schemaLocation"
                        xa.Value = namespaceText.Text & " " & namespaceText.Text & "/Documents/XSD/Responses/" & fileName
                    Case "targetNamespace"
                        xa.Value = namespaceText.Text
                End Select
            Next

            'Save the document
            fileInf.IsReadOnly = False
            xmlDoc.Save(fileInf.FullName)
            fileInf.IsReadOnly = True
        Next
    End Sub
    Protected Sub updateWebServices(ByVal filePath As String, ByVal fileExe As String, ByVal patternMatcherCase As Integer)
        Dim files As FileInfo()
        Dim fileInf As FileInfo

        files = getFiles(Server.MapPath(filePath), fileExe)

        'Loop through all files
        For Each fileInf In files

            'Make the file writeable
            fileInf.IsReadOnly = False

            'Read the current file
            Dim reader As StreamReader
            reader = fileInf.OpenText


            'Create an array conatining, with each item containing 
            'a seperate line from the current file
            Dim lines As Array = reader.ReadToEnd.Split(vbLf)
            Dim ln As String

            reader.Close()
            Dim writer As StreamWriter = fileInf.CreateText
            'Loop through the file's lines
            For Each ln In lines

                'Call the pattern matcher and pass the case selection
                patternMatcher(ln, patternMatcherCase)

                'Write the line to the file
                writer.Write(ln)
            Next

            'Close the writer
            writer.Close()

            'Make the file Read Only again
            fileInf.IsReadOnly = True
        Next
    End Sub
    Protected Function patternMatcher(ByVal ln As String, ByVal matchCase As Integer) As String
        Select Case matchCase
            Case 1
                ln = webService(ln)
        End Select

        Return ln
    End Function
    Protected Function webService(ByVal ln As String) As String
        '--------------------------------------------------------------------------------------------
        '   Look for the web service namespace setting
        '
        If InStr(ln, "<WebService(Namespace:=") Then
            'If found, replace the line with the string below
            'adding the new namespace specified
            ln = "<WebService(Namespace:=""" & namespaceText.Text & """)> _" & vbNewLine
        Else
            'To ensure the line endings are consistent
            'check the current value and amend as necessary
            If ln.EndsWith(vbCr) Then
                ln += vbLf
            ElseIf ln.EndsWith(vbLf) Then
                ' do nothing
            Else
                ln += vbNewLine
            End If
        End If
        Return ln
    End Function

End Class
