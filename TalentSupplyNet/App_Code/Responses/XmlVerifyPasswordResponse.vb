Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with verify password response
'
'       Date                        04/02/09
'
'       Author                      Ben
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSVP- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlVerifyPasswordResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndVerifyPassword, ndResponse, ndCustomerDetails, ndReturnCode As XmlNode
        Private ndUserName, ndPasswordOK, _ndEncryptedPassword As XmlNode
        Private dtVerifyPasswordResults, dtStatusResults As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndVerifyPassword = .CreateElement("VerifyPassword")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Populate the xml document
                With ndVerifyPassword
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
                ndHeader.InsertAfter(ndVerifyPassword, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSVP-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            Dim dr, drStatus As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
                ndPasswordOK = .CreateElement("PasswordOK")
                ndUserName = .CreateElement("UserName")
            End With

            'Read the values for the response section
            dtVerifyPasswordResults = ResultDataSet.Tables("VerifyPasswordResults")
            dtStatusResults = ResultDataSet.Tables("StatusResults")
            dr = dtVerifyPasswordResults.Rows(0)
            drStatus = dtStatusResults.Rows(0)


            'Populate the nodes
            ndReturnCode.InnerText = drStatus("ReturnCode").ToString
            ndPasswordOK.InnerText = dr("Success").ToString
            ndUserName.InnerText = dr("UserName").ToString
            'If dr("ErrorOccurred") = "E" Then
            '    errorOccurred = True
            'End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndPasswordOK)
                .AppendChild(ndUserName)
            End With


        End Sub
        Protected Overrides Sub InsertBodyV1_1()

            Try

                ' Create the xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndVerifyPassword = .CreateElement("VerifyPassword")
                    ndResponse = .CreateElement("Response")
                End With

                'Create the response xml section
                CreateResponseSection11()

                'Populate the xml document
                With ndVerifyPassword
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
                ndHeader.InsertAfter(ndVerifyPassword, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSVP-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Sub CreateResponseSection11()

            Dim dr, drStatus As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
                ndPasswordOK = .CreateElement("PasswordOK")
                ndUserName = .CreateElement("UserName")
                _ndEncryptedPassword = .CreateElement("EncryptedPassword")
            End With

            'Read the values for the response section
            dtVerifyPasswordResults = ResultDataSet.Tables("VerifyPasswordResults")
            dtStatusResults = ResultDataSet.Tables("StatusResults")
            dr = dtVerifyPasswordResults.Rows(0)
            drStatus = dtStatusResults.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = drStatus("ReturnCode").ToString
            ndPasswordOK.InnerText = dr("Success").ToString
            ndUserName.InnerText = dr("UserName").ToString
            _ndEncryptedPassword.InnerText = dr("EncryptedPassword").ToString().Trim()
            'If dr("ErrorOccurred") = "E" Then
            '    errorOccurred = True
            'End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndPasswordOK)
                .AppendChild(ndUserName)
                .AppendChild(_ndEncryptedPassword)
            End With

        End Sub

    End Class

End Namespace