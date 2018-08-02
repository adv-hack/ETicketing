Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with generating ticketing basket id responses
'
'       Date                        June 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRGTBID- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlGenerateTicketingBasketIDResponse
        Inherits XmlResponse
        Private _deTID As New DETicketingItemDetails

        Public Property DeTID() As DETicketingItemDetails
            Get
                Return _deTID
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _deTID = value
            End Set
        End Property

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndTicketingBasketID, ndReturnCode As XmlNode
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndTicketingBasketID = .CreateElement("TicketingBasketID")
                    ndTicketingBasketID.InnerText = DeTID.SessionId
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndTicketingBasketID, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRGTBID-01"
                    .HasError = True
                End With
            End Try

        End Sub

    End Class

End Namespace