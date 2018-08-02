Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal
    Public Class XmlRetrieveProfileDetailsResponse
        Inherits XmlResponse
        Private ndsAttributes(10) As XmlAttribute
        Private ndProfileDetails, ndHeaderRootHeader, ndResponse, ndReturnCode, ndHeaderRoot As XmlNode
        Private atXmlNsXsi As XmlAttribute
        Private errorOccured As Boolean
        Private _dtProfile As DataSet
        Private dtProfileDetails, dtAttr As DataTable

        Public Property ProfileResultDataSet() As DataSet
            Get
                Return _dtProfile
            End Get
            Set(ByVal value As DataSet)
                _dtProfile = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()
            '--------------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet  
            ' 
            Try
                '--------------------------------------------------------------------------------------
                With MyBase.xmlDoc
                    ndProfileDetails = .CreateElement("ProfileDetails")
                    If Not Err.HasError Then
                        dtProfileDetails = ResultDataSet.Tables.Item(0)                        
                        dtAttr = ResultDataSet.Tables.Item(1)

                        ndResponse = .CreateElement("Response")
                        'Create the response xml section
                        CreateResponseSection()

                        If Not errorOccured Then
                            Err = InsertDetails()
                        End If
                    End If

                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                    ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                    ndHeaderRoot.InsertAfter(ndResponse, ndHeaderRootHeader)
                    ndHeaderRoot.InsertAfter(ndProfileDetails, ndResponse)

                    'Insert the XSD reference & namespace as an attribute within the root node
                    atXmlNsXsi = CreateNamespaceAttribute()
                    ndHeaderRoot.Attributes.Append(atXmlNsXsi)
                End With
            Catch ex As Exception
            End Try
        End Sub

        Protected Sub CreateResponseSection()

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            dr = dtProfileDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccured") = "E" Then
                errorOccured = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Private Function InsertDetails() As ErrorObj
            Dim err2 As ErrorObj = Nothing

            For Each row As DataRow In dtAttr.Rows
                Dim ndAttribute As XmlNode = xmlDoc.CreateElement("Attribute")
                If row.Table.Columns.Contains("Attribute") Then
                    ndAttribute.InnerText = row("Attribute").ToString
                    ndProfileDetails.AppendChild(ndAttribute)
                End If
            Next
            Return err2
        End Function
    End Class
End Namespace
