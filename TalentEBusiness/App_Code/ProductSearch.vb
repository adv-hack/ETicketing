Imports Microsoft.VisualBasic
Imports System.Data

Namespace Talent.eCommerce
    Public Class ProductSearch

        Public Function GetCriteria(ByVal businessUnit As String, ByVal partner As String, ByVal group As String, ByVal number As Integer) As DataTable
            Dim dtCriteria As New DataTable
            Dim cachename As String = "ProductSearchCriteria" & number.ToString & group & businessUnit & partner
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then
                Select Case number
                    Case Is = 1
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search01TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria01By_BU_Partner(businessUnit, partner, group)
                    Case Is = 2
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search02TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria02By_BU_Partner(businessUnit, partner, group)
                    Case Is = 3
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search03TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria03By_BU_Partner(businessUnit, partner, group)
                    Case Is = 4
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search04TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria04By_BU_Partner(businessUnit, partner, group)
                    Case Is = 5
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search05TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria05By_BU_Partner(businessUnit, partner, group)
                    Case Is = 6
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search06TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria06By_BU_Partner(businessUnit, partner, group)
                    Case Is = 7
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search07TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria07By_BU_Partner(businessUnit, partner, group)
                    Case Is = 8
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search08TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria08By_BU_Partner(businessUnit, partner, group)
                    Case Is = 9
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search09TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria09By_BU_Partner(businessUnit, partner, group)
                    Case Is = 10
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search10TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria10By_BU_Partner(businessUnit, partner, group)
                    Case Is = 11
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search11TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria11By_BU_Partner(businessUnit, partner, group)
                    Case Is = 12
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search12TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria12By_BU_Partner(businessUnit, partner, group)
                    Case Is = 13
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search13TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria13By_BU_Partner(businessUnit, partner, group)
                    Case Is = 14
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search14TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria14By_BU_Partner(businessUnit, partner, group)
                    Case Is = 15
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search15TableAdapter 
                        dtCriteria = productSearch.GetDataCriteria15By_BU_Partner(businessUnit, partner, group)
                    Case Is = 16
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search16TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria16By_BU_Partner(businessUnit, partner, group)
                    Case Is = 17
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search17TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria17By_BU_Partner(businessUnit, partner, group)
                    Case Is = 18
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search18TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria18By_BU_Partner(businessUnit, partner, group)
                    Case Is = 19
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search19TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria19By_BU_Partner(businessUnit, partner, group)
                    Case Is = 20
                        Dim productSearch As New TalentProductInformationTableAdapters.tbl_product_search20TableAdapter 
                        dtCriteria = ProductSearch.GetDataCriteria20By_BU_Partner(businessUnit, partner, group)
                End Select

                If dtCriteria.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtCriteria, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtCriteria = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtCriteria
        End Function

    End Class

End Namespace

