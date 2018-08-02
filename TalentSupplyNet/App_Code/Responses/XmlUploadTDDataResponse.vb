Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with verify password response
'
'       Date                        20/04/09
'
'       Author                      Alex C
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSUTDD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlUploadTDDataResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndUploadTDData, ndResponse, ndReturnCode As XmlNode
        Private ndServerName, ndTDDataUploadOK As XmlNode
        Private dtUploadTDDataResponseResults, dtStatusResults As DataTable

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndUploadTDData = .CreateElement("UploadTDData")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Populate the xml document
                With ndUploadTDData
                    .AppendChild(ndResponse)
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndUploadTDData, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSUTDD-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndTDDataUploadOK = .CreateElement("TDDataUploadOK")
                ndServerName = .CreateElement("ServerName")
            End With

            If Err.HasError Then
                ndTDDataUploadOK.InnerText = "FALSE"
            Else
                ndTDDataUploadOK.InnerText = "TRUE"
            End If
            ndServerName.InnerText = HttpContext.Current.Request.ServerVariables("SERVER_NAME")

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndTDDataUploadOK)
                .AppendChild(ndServerName)
            End With

        End Sub

    End Class
End Namespace
