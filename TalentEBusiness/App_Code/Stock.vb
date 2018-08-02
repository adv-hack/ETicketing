Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Stock - Handle stocl checks and allocations
'
'       Date                        160307
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------
Namespace Talent.eCommerce

    Public Class Stock
        '----------------
        ' GetStockBalance
        '----------------
        Public Shared Function GetStockBalance(ByVal product As String) As Integer
            Dim stockBalance As Integer = 0
            Dim _businnessUnit As String = TalentCache.GetBusinessUnit
            '---------------
            ' Get price list 
            '---------------
            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Dim stockLocation As String = def.StockLocation

            Dim productInfo As New TalentProductInformationTableAdapters.tbl_product_stockTableAdapter 
            Dim dt As Data.DataTable = productInfo.GetDataByProduct_stock_location(product, stockLocation)
            If dt.Rows.Count > 0 Then
                stockBalance = Utilities.CheckForDBNull_Int(dt.Rows(0)("AVAILABLE_QUANTITY"))
            End If

            Return stockBalance
        End Function


        Public Shared Function GetNoStockDescription(ByVal product As String, ByRef stockTimeValue As String, Optional ByRef reStocskCode As String = "") As String
            Dim stockDescription As String = ""
            Dim _businnessUnit As String = TalentCache.GetBusinessUnit
            '---------------
            ' Get price list 
            '---------------
            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Dim stockLocation As String = def.StockLocation

            Dim productInfo As New TalentProductInformationTableAdapters.tbl_product_stockTableAdapter
            Dim dt As Data.DataTable = productInfo.GetDataByProduct_stock_location(product, stockLocation)
            If dt.Rows.Count > 0 Then
                reStocskCode = Utilities.CheckForDBNull_String(dt.Rows(0)("RESTOCK_CODE"))
            End If

            Dim descriptionsTA As New TalentDescriptionsDataSetTableAdapters.tbl_ebusiness_descriptions_buTableAdapter
            Dim descriptions As New TalentDescriptionsDataSet.tbl_ebusiness_descriptions_buDataTable
            descriptions = descriptionsTA.GetDataByBUetcLang("Stock", _
                                                                TalentCache.GetBusinessUnitGroup, _
                                                                TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                "Restock", _
                                                                Talent.Common.Utilities.GetDefaultLanguage)

            If descriptions.Rows.Count = 0 Then
                descriptions = descriptionsTA.GetDataByBUetcLang("Stock", _
                                                                    TalentCache.GetBusinessUnitGroup, _
                                                                    Talent.Common.Utilities.GetAllString, _
                                                                    "Restock", _
                                                                    Talent.Common.Utilities.GetDefaultLanguage)
            End If
            If descriptions.Rows.Count = 0 Then
                descriptions = descriptionsTA.GetDataByBUetcLang("Stock", _
                                                                    Talent.Common.Utilities.GetAllString, _
                                                                    Talent.Common.Utilities.GetAllString, _
                                                                    "Restock", _
                                                                    Talent.Common.Utilities.GetDefaultLanguage)
            End If

            If descriptions.Rows.Count > 0 Then
                For Each description As TalentDescriptionsDataSet.tbl_ebusiness_descriptions_buRow In descriptions.Rows
                    If LCase(description.DESCRIPTION_CODE) = LCase(reStocskCode) Then
                        stockDescription = description.LANGUAGE_DESCRIPTION
                        stockTimeValue = description.DESCRIPTION_DESCRIPTION
                        Exit For
                    End If
                Next
            End If

            Return stockDescription
        End Function


    End Class
End Namespace

