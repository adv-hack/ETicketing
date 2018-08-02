Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Availability responses
'
'       Date                        Dec 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSAV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAvailabilityResponse
        Inherits XmlResponse
        Private _dep As DEProductAlert

        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private ndHeader, ndSection, ndManufacturerPartNumber, ndManufacturerPartNumberOccurs, _
                ndVendorNumber, ndDescription, ndReserveInventoryFlag, ndAvailableRebQty, _
                ndBranch, ndAvailability, ndErrorStatus, ndOnOrder, ndETADate As XmlNode
        Private atSku, atQuantity, atBranch, atName, atXmlNsXsi, atErrorNumber As XmlAttribute

        Public Property Dep() As DEProductAlert
            Get
                Return _dep
            End Get
            Set(ByVal value As DEProductAlert)
                _dep = value
            End Set
        End Property
        Protected Overrides Sub InsertBodyV1()
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            '
            With MyBase.xmlDoc
                ndHeader = .CreateElement("Availability")
                If Not Dep Is Nothing Then
                    If Dep.CollDEAlerts.Count > 0 Then
                        For iCounter = 1 To Dep.CollDEAlerts.Count
                            '-----------------------------------------------------------
                            CreateHeader()
                            Try
                                dea = Dep.CollDEAlerts.Item(iCounter)
                                atSku.Value = dea.ProductCode.ToString
                                atQuantity.Value = dea.Quantity.ToString
                                '-------------------------------------------------------
                                '   Curently not set
                                '
                                ndDescription.InnerText = dea.Description.ToString
                            Catch ex As Exception
                            End Try
                            '-----------------------------------------------------------
                            If Not Err2 Is Nothing Then
                                ndErrorStatus.InnerText = Err2.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err2.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                            ElseIf Not Err Is Nothing Then
                                ndErrorStatus.InnerText = Err.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                            End If
                            '-----------------------------------------------------------
                            AppendHeader()
                        Next
                    Else
                        '---------------------------------------------------------------------------------
                        '  No data so write dummy
                        '
                        CreateHeader()
                        AppendHeader()
                    End If
                Else
                    '---------------------------------------------------------------------------------
                    '  No data so write dummy
                    '
                    CreateHeader()
                    AppendHeader()
                End If
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
                    ndSection.Attributes.Append(atSku)
                    ndSection.Attributes.Append(atQuantity)
                    '-----------------------------------------------------------------
                    ndErrorStatus = .CreateElement("ErrorStatus")
                    ndManufacturerPartNumber = .CreateElement("ManufacturerPartNumber")
                    ndManufacturerPartNumberOccurs = .CreateElement("ManufacturerPartNumberOccurs")
                    ndVendorNumber = .CreateElement("VendorNumber")
                    ndDescription = .CreateElement("Description")
                    ndReserveInventoryFlag = .CreateElement("ReserveInventoryFlag")
                    ndAvailableRebQty = .CreateElement("AvailableRebQty")
                    '-----------------------------------------------------------------
                    ndBranch = .CreateElement("Branch")
                    atBranch = .CreateAttribute("ID")
                    atName = .CreateAttribute("Name")
                    ndBranch.Attributes.Append(atBranch)
                    ndBranch.Attributes.Append(atName)
                    '-----------------------------------------------------------------
                    ndAvailability = .CreateElement("Availability")
                    ndOnOrder = .CreateElement("OnOrder")
                    ndETADate = .CreateElement("ETADate")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
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
                With ndSection
                    .AppendChild(ndErrorStatus)
                    .AppendChild(ndManufacturerPartNumber)
                    .AppendChild(ndManufacturerPartNumberOccurs)
                    .AppendChild(ndVendorNumber)
                    .AppendChild(ndDescription)
                    .AppendChild(ndReserveInventoryFlag)
                    .AppendChild(ndAvailableRebQty)
                    .AppendChild(ndBranch)
                    .AppendChild(ndAvailability)
                    .AppendChild(ndOnOrder)
                    .AppendChild(ndETADate)
                End With
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