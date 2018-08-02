Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Xml load responses
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRXML- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlLoadResponse
        Inherits XmlResponse

        Private ndDocRoot, ndDocHeaderRoot, ndErrorStatus As XmlNode
        Private ndEcrmInputs, ndEcrmInput, ndXmlLine As XmlNode
        Private atId, atXmlId, atTitle, atErrorNumber As XmlAttribute

        Protected Overrides Sub InsertBodyV1()
            '------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet    
            '
            Dim iCounter As Integer = 0
            Try
                With MyBase.xmlDoc
                    ndEcrmInputs = .CreateElement("ecrminputs")
                    '---------------------------------------------------------------------------------
                    ndErrorStatus = .CreateElement("ErrorStatus")
                    If Not Err2 Is Nothing Then
                        For iCounter = 1 To 500
                            If Not Err2.ItemErrorStatus(iCounter) = Nothing Then
                                ndErrorStatus.InnerText = Err2.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err2.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                                ndEcrmInputs.AppendChild(ndErrorStatus)
                            Else
                                Exit For
                            End If
                        Next
                    End If
                    If Not Err Is Nothing Then
                        For iCounter = 1 To 500
                            If Not Err.ItemErrorStatus(iCounter) = Nothing Then
                                ndErrorStatus.InnerText = Err.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                                ndEcrmInputs.AppendChild(ndErrorStatus)
                            Else
                                Exit For
                            End If
                        Next
                    End If
                    '---------------------------------------------------------------------------------
                    If Not Err.HasError Then
                        dtHeader = ResultDataSet.Tables(0)      ' Header
                        dtDetail = ResultDataSet.Tables(1)      ' Item
                        Err2 = InsertHeader()
                    End If
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndDocRoot = .SelectSingleNode(c1 & RootElement())
                    ndDocHeaderRoot = .SelectSingleNode(c1 & RootElement() & c2)
                    ndDocRoot.InsertAfter(ndEcrmInputs, ndDocHeaderRoot)
                    'Insert the XSD reference & namespace as an attribute within the root node
                    Dim atXmlNsXsi As XmlAttribute = CreateNamespaceAttribute()
                    ndDocRoot.Attributes.Append(atXmlNsXsi)
                End With

            Catch ex As Exception
            End Try

        End Sub
        Private Function InsertHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '-------------------------------------------------------------------------------------
            Dim dr As DataRow
            Dim sWork As String = String.Empty
            Try
                '---------------------------------------------------------------------------------
                If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                    For Each dr In dtHeader.Rows
                        err = CreateHeader()
                        Dim eCrmInputId As String = dr("eCrmInputId")
                        atId.Value = eCrmInputId
                        err = InsertDetails(eCrmInputId)
                        err = AppendHeader()
                    Next
                Else
                    err = CreateHeader()
                    err = AppendHeader()
                End If
                'ndecrminputs.AppendChild(ndecrminput)
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Insert xml Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRXML-11"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndEcrmInput = .CreateElement("ecrminput")
                    atId = .CreateAttribute("id")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRXML-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndEcrmInputs.AppendChild(ndEcrmInput)
                ndEcrmInput.Attributes.Append(atId)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRXML-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails(ByVal eCrmInputId As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                '--------------------------------------------------------------------------
                '   Add details lines from table 2
                '   
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDetail.Rows
                        If dr("eCrmInputId").Equals(eCrmInputId) Then
                            err = CreateDetail()
                            ndXmlLine.InnerText = dr("xml")
                            atXmlId.Value = dr("Id")
                            atTitle.Value = dr("Title")
                            err = AppendDetail()
                        End If
                    Next dr
                Else
                    '--------------------------------------------------------------------------
                    '   No details, write dummy
                    ' 
                    err = CreateDetail()
                    err = AppendDetail()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRXML-17"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateDetail() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndXmlLine = .CreateElement("xml")
                    atXmlId = .CreateAttribute("id")
                    atTitle = .CreateAttribute("title")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRXML-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetail() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndEcrmInput.AppendChild(ndXmlLine)
                With ndXmlLine
                    .Attributes.Append(atXmlId)
                    .Attributes.Append(atTitle)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to append detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRXML-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace
