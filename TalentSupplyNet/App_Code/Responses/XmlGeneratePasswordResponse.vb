Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with generate password response
'
'       Date                        22/08/10
'
'       Author                      Ben
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSGP- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlGeneratePasswordResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndGeneratePassword, ndResponse, ndCustomerDetails, ndReturnCode As XmlNode
        Private ndPassword As XmlNode
        Private dtGeneratePasswordResults, dtStatusResults As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndGeneratePassword = .CreateElement("GeneratePassword")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Populate the xml document
                With ndGeneratePassword
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
                ndHeader.InsertAfter(ndGeneratePassword, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSGP-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            Dim dr, drStatus As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
                ndPassword = .CreateElement("Password")
            End With

            'Read the values for the response section
            dtGeneratePasswordResults = ResultDataSet.Tables("GeneratePasswordResults")
            dtStatusResults = ResultDataSet.Tables("StatusResults")
            dr = dtGeneratePasswordResults.Rows(0)
            drStatus = dtStatusResults.Rows(0)


            'Populate the nodes
            ndReturnCode.InnerText = drStatus("ReturnCode").ToString
            ndPassword.InnerText = dr("Password").ToString

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndPassword)
            End With

        End Sub
    End Class

End Namespace