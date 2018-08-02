Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Xml.Schema
Imports System.IO
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Utilities for dealing with XML Bits
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'  
'       Error Number Code base      TTPXMLU- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlUtils

        Private _errorText As String = String.Empty
        Private _errorObj As ErrorObj

        Public Property ErrorText() As String
            Get
                Return _errorText
            End Get
            Set(ByVal value As String)
                _errorText = value
            End Set
        End Property
        Public Property XsltErr() As ErrorObj
            Get
                Return _errorObj
            End Get
            Set(ByVal errStatus As ErrorObj)
                _errorObj = errStatus
            End Set
        End Property

        Public Function ApplyXsltStyleSheet(ByVal InputXMLDoc As XmlDocument, ByVal XsltDoc As XmlDocument) As XmlDocument
            ' Takes an Xml Document, and a path to an XSLT document, performs the transformation using the two
            ' and returns the transformed document as an XmlDocument object
            ' Create the variables
            Dim err As New ErrorObj
            Dim transformedDoc As New XmlDocument
            Dim xslt As New Xsl.XslCompiledTransform
            Dim sw As System.IO.StringWriter = New System.IO.StringWriter
            Dim xw As XmlTextWriter = New XmlTextWriter(sw)

            Try
                ' Load the XSL file into the xslt object
                xslt.Load(XsltDoc)

                ' Perform the transformation and output it to the XmlTextWriter
                xslt.Transform(InputXMLDoc, xw)

                ' Load the generated XML into the new XMLDocument
                transformedDoc.LoadXml(sw.ToString)

            Catch ex As Exception
                ' If the transformation fails, populate the Error Object with
                ' the details
                With err
                    .ErrorMessage = ex.Message
                    .HasError = True
                    .ErrorNumber = "TTPXMLU-01"
                    .ErrorStatus = ex.Message
                End With
                XsltErr = err
            End Try

            Return transformedDoc
        End Function
        Public Function ApplyXsltStyleSheet(ByVal InputXMLDoc As XmlDocument, ByVal styleSheetPath As String) As XmlDocument
            ' Takes an Xml Document, and a path to an XSLT document,
            ' loads the xslt file, and calls the XSLTTransform func
            ' Create the variables
            Dim transformedDoc As New XmlDocument
            Dim xsltDoc As New XmlDocument

            ' Load the XSL file into the xsltDoc object
            xsltDoc.Load(styleSheetPath)

            ' Call Transform func
            transformedDoc = ApplyXsltStyleSheet(InputXMLDoc, xsltDoc)

            Return transformedDoc
        End Function
        Public Function ApplyXsltStyleSheet(ByVal XmlDocLocation As String, ByVal XsltDoc As XmlDocument) As XmlDocument
            ' Takes an Xml Document path, and an XSLT document object,
            ' loads the xml file, and calls the XSLTTransform func
            ' Create the variables
            Dim transformedDoc As New XmlDocument
            Dim xmlDoc As New XmlDocument

            ' Load the XML file into the xmlDoc object
            xmlDoc.Load(XmlDocLocation)

            ' Call Transform func
            transformedDoc = ApplyXsltStyleSheet(xmlDoc, XsltDoc)

            Return transformedDoc
        End Function
        Public Function ApplyXsltStyleSheet(ByVal XmlDocLocation As String, ByVal styleSheetPath As String) As XmlDocument
            ' Takes a path to an input Xml Document, Loads the document, then calls the
            ' above method to perform the transformation, before returning the result
            ' as an XmlDocument Object
            Dim xDoc As New XmlDocument
            Dim transformedDoc As New XmlDocument

            ' Load in the document from the path provided
            xDoc.Load(XmlDocLocation)

            ' Call the transform function
            transformedDoc = ApplyXsltStyleSheet(xDoc, styleSheetPath)

            Return transformedDoc
        End Function

        Public Function GetXmlDocAsString(ByVal xmlDoc As XmlDocument) As String
            'Takes an XML Document object and returns its string representation
            Dim sw As StringWriter = New StringWriter
            Dim xw As XmlTextWriter = New XmlTextWriter(sw)
            xmlDoc.WriteTo(xw)
            Return sw.ToString
        End Function
        Public Function GetXSDSettings(ByVal XSDfileLocation As String, ByVal SchemaTargetNameSpace As String) As XmlReaderSettings
            ' Takes an XML file, and an XSD file and associates the two and returns 
            ' an XML Reader object containing the XML with its schema

            ' Create the XML reader object and set the
            ' validation settings
            Dim settings As XmlReaderSettings = New XmlReaderSettings()
            settings.ValidationType = ValidationType.Schema
            Dim xss As New XmlSchemaSet
            xss.Add(SchemaTargetNameSpace, XSDfileLocation)
            settings.Schemas = xss

            AddHandler settings.ValidationEventHandler, AddressOf ValidationCallBack

            Return settings
        End Function
        Public Function GetXMLwithXSD(ByVal XMLfileLocation As String, ByVal XSDfileLocation As String, ByVal SchemaTargetNameSpace As String) As XmlReader
            ' Takes an XML file, and an XSD file and associates the two and returns 
            ' an XML Reader object containing the XML with its schema

            ' Get validation settings
            Dim settings As XmlReaderSettings = GetXSDSettings(XSDfileLocation, SchemaTargetNameSpace)

            ' Create the XML Reader object and fill with the
            ' XML file and validation settings
            Dim vreader As XmlReader = XmlReader.Create(XMLfileLocation, settings)

            Return vreader
        End Function
        Public Function GetXMLwithXSD(ByVal xmlDoc As XmlDocument, ByVal XSDfileLocation As String, ByVal SchemaTargetNameSpace As String) As XmlReader
            ' Takes an XML Document object, and an XSD file and associates the two and returns 
            ' an XML Reader object containing the XML with its schema

            ' Get validation settings
            Dim settings As XmlReaderSettings = GetXSDSettings(XSDfileLocation, SchemaTargetNameSpace)

            ' Create text reader containing contents of XML Document to use within XmlReader
            Dim xr As New XmlTextReader(New StringReader(GetXmlDocAsString(xmlDoc)))

            ' Create the XML Reader object and fill with the
            ' XML file and validation settings
            Dim vreader As XmlReader = XmlReader.Create(xr, settings)

            Return vreader
        End Function

        Private Sub ValidationCallBack(ByVal sender As Object, ByVal e As ValidationEventArgs)
            ' Display any validation errors.
            ErrorText = "Validation Error: " & e.Message.ToString & " Error line number: " & e.Exception.LineNumber & " Error line position: " & e.Exception.LinePosition
        End Sub
        Public Sub writeXml(ByVal xmlDoc As XmlDocument, ByVal folder As String)
            Dim folderPath As String = ConfigurationManager.AppSettings("TxtLogPath") & "\" & ConfigurationManager.AppSettings("XMLLogFolder") & "\" & folder
            folderPath = HttpContext.Current.Server.MapPath(folderPath)
            xmlDoc.Save(folderPath)
        End Sub

    End Class

End Namespace