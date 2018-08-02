Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with customer retrieval responses
'
'       Date                        April 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSCR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRetrieveTransactionProgressResponse
        Inherits XmlResponse

        'Nodes
        Private TransactionProgressNode, ProgressTransactionIDNode, ReturnCodeNode, CompleteNode, TotalRecordsNode, ProcessedRecordsNode As XmlNode

        Private HeaderNode, HeaderRootNode As XmlNode


        Protected Overrides Sub InsertBodyV1()

            Try
                With MyBase.xmlDoc
                    TransactionProgressNode = .CreateElement("TransactionProgress")
                    createTransactionProgress()                    
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                HeaderNode = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                HeaderRootNode = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                HeaderNode.InsertAfter(TransactionProgressNode, HeaderRootNode)

            Catch ex As Exception

            End Try

            'add the nodes to the root document
        End Sub


        ''' <summary>
        ''' Constructs the transaction progress node which contains the progress details
        ''' 
        ''' </summary>        
        ''' <remarks></remarks>
        Private Sub createTransactionProgress()

            'Create the nodes
            With MyBase.xmlDoc
                ProgressTransactionIDNode = .CreateElement("TransactionID")
                ReturnCodeNode = .CreateElement("ReturnCode")
                CompleteNode = .CreateElement("Complete")
                TotalRecordsNode = .CreateElement("TotalRecords")
                ProcessedRecordsNode = .CreateElement("ProcessedRecords")
            End With

            'Set the values
            ProgressTransactionIDNode.InnerText = ProgressTransactionID
            ReturnCodeNode.InnerText = ""
            CompleteNode.InnerText = Complete
            TotalRecordsNode.InnerText = TotalRecords
            ProcessedRecordsNode.InnerText = ProcessedRecords

            'Add them to the transaction progress node
            TransactionProgressNode.AppendChild(ProgressTransactionIDNode)
            TransactionProgressNode.AppendChild(ReturnCodeNode)
            TransactionProgressNode.AppendChild(CompleteNode)
            TransactionProgressNode.AppendChild(TotalRecordsNode)
            TransactionProgressNode.AppendChild(ProcessedRecordsNode)
        End Sub


    End Class

End Namespace
