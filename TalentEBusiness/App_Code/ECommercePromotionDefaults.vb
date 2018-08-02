Imports Microsoft.VisualBasic
Imports System.Data
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                   
'
'       Date                        Nov 08
'
'       Author                       
'
'       � CS Group 2007               All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'-------------------------------------------------------------------------------------------------

Namespace Talent.eCommerce

    Public Class ECommercePromotionDefaults

        Dim businessUnit As String = TalentCache.GetBusinessUnit()
        Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)

        Public Function GetPromotionDefaults() As DataTable

            Dim dtbl As New DataTable
            Dim cachename As String = "ECommercePromotionDefaults" & businessUnit & partner

            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then
                Dim promoAttributes As New TalentPromotionsDatSetTableAdapters.tbl_promotionsTableAdapter
                dtbl = promoAttributes.GetDistinctAttributeData
                HttpContext.Current.Cache.Insert(cachename, _
                                                 dtbl, _
                                                 Nothing, _
                                                 System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                 Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)
            Else
                dtbl = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If

            Return dtbl

        End Function

    End Class

End Namespace

