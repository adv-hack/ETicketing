Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Multiple Availability responses
'
'       Date                        April 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSMAV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlMultiAvailabilityResponse
        Inherits XmlResponse
        Private _dep As DEProductAlert

        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private ndHeader, ndSection, ndManufacturerPartNumber, ndManufacturerPartNumberOccurs, _
                ndWareHouse, ndAvailability, ndErrorStatus, ndOnOrder, ndETADate As XmlNode
        Private atSku, atQuantity, atWareHouse, atName, atXmlNsXsi, atErrorNumber As XmlAttribute

        Public Property Dep() As DEProductAlert
            Get
                Return _dep
            End Get
            Set(ByVal value As DEProductAlert)
                _dep = value
            End Set
        End Property
        Protected Overrides Sub InsertBodyV1()
            '
            Dim iCounter As Integer = 0
            Dim dtDetail As DataTable = Nothing
            Dim drDetail As DataRow
            ' Check for error (otherwise crashes if fails on invalid document)
            If Not ResultDataSet Is Nothing And ResultDataSet.Tables.Count > 0 Then
                dtDetail = ResultDataSet.Tables(0)     ' Detail
            Else
                dtDetail = New DataTable
            End If

            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("MultiAvailability")

                For Each drDetail In dtDetail.Rows
                    '-----------------------------------------------------------
                    CreateHeader()
                    Try
                        atSku.Value = (drDetail("ProductNumber").ToString).Trim
                        atQuantity.Value = (drDetail("Quantity").ToString).Trim
                        atWareHouse.Value = (drDetail("WareHouse").ToString).Trim

                    Catch ex As Exception
                    End Try
                    '-----------------------------------------------------------
                    If Not Err2 Is Nothing Then
                        If Not Err2.ItemErrorStatus(iCounter) Is Nothing Then
                            ndErrorStatus.InnerText = Err2.ItemErrorStatus(iCounter)
                            atErrorNumber = .CreateAttribute("ErrorNumber")
                            atErrorNumber.Value = Err2.ItemErrorCode(iCounter)
                            ndErrorStatus.Attributes.Append(atErrorNumber)
                            ndSection.AppendChild(ndErrorStatus)
                        End If
                    End If
                    If Not Err Is Nothing Then
                        If Not Err.ItemErrorStatus(iCounter) Is Nothing Then
                            ndErrorStatus.InnerText = Err.ItemErrorStatus(iCounter)
                            atErrorNumber = .CreateAttribute("ErrorNumber")
                            atErrorNumber.Value = Err.ItemErrorCode(iCounter)
                            ndErrorStatus.Attributes.Append(atErrorNumber)
                            ndSection.AppendChild(ndErrorStatus)
                        End If
                    End If
                    '-----------------------------------------------------------
                    AppendHeader()
                Next

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                ndHeaderRoot.InsertAfter(ndHeader, ndHeaderRootHeader)
                'Insert the XSD reference & namespace as an attribute within the root node
                atXmlNsXsi = CreateNamespaceAttribute()
                ndHeaderRoot.Attributes.Append(atXmlNsXsi)
            End With
        End Sub

        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndSection = .CreateElement("Availability")
                    atSku = .CreateAttribute("SKU")
                    atQuantity = .CreateAttribute("Quantity")
                    atWareHouse = .CreateAttribute("WareHouse")
                    ndSection.Attributes.Append(atSku)
                    ndSection.Attributes.Append(atQuantity)
                    ndSection.Attributes.Append(atWareHouse)
                    ndErrorStatus = .CreateElement("ErrorStatus")
                    ndHeader.AppendChild(ndSection)
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSAV-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndHeader.AppendChild(ndSection)
            Catch ex As Exception
                Const strError As String = "Failed to Append  Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSAV-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class
End Namespace