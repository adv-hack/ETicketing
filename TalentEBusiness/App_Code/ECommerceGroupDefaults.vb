Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Collections.Generic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Bread crumb trail defaults
'
'       Date                        mar 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      APCBCT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'-------------------------------------------------------------------------------------------------

Namespace Talent.eCommerce
    Public Class ECommerceGroupDefaults

        Public Function GetGroupDefaults(ByVal businessUnit As String, ByVal partner As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_01TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner(businessUnit, partner)
                If dtGroupDefaults.Rows.Count > 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner(businessUnit, Talent.Common.Utilities.GetAllString)
                    If dtGroupDefaults.Rows.Count > 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner(Talent.Common.Utilities.GetAllString, partner)
                        If dtGroupDefaults.Rows.Count > 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_01TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then
                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner(businessUnit, partner)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner(businessUnit, Talent.Common.Utilities.GetAllString)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner(Talent.Common.Utilities.GetAllString, partner)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString)
                        End If
                    End If
                End If
                ' BF - cache whether found or not otherwise the itemsInCache call may hang
                'If dtGroupDefaults.Rows.Count > 0 Then
                '...
                HttpContext.Current.Cache.Insert(cachename, _
                                                 dtGroupDefaults, _
                                                 Nothing, _
                                                 System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                 Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                ' End If
            Else
            dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_02TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_03TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_04TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String, ByVal group04 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_05TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03 & group04
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03, group04)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03, group04)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String, ByVal group04 As String, ByVal group05 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_06TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03 & group04 & group05
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03, group04, group05)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03, group04, group05)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String, ByVal group04 As String, ByVal group05 As String, ByVal group06 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_07TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03 & group04 & group05 & group06
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03, group04, group05, group06)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03, group04, group05, group06)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String, ByVal group04 As String, ByVal group05 As String, ByVal group06 As String, ByVal group07 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_08TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03 & group04 & group05 & group06 & group07
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename) Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03, group04, group05, group06, group07)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06, group07)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03, group04, group05, group06, group07)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06, group07)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String, ByVal group04 As String, ByVal group05 As String, ByVal group06 As String, ByVal group07 As String, ByVal group08 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_09TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03 & group04 & group05 & group06 & group07 & group08
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03, group04, group05, group06, group07, group08)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06, group07, group08)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03, group04, group05, group06, group07, group08)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06, group07, group08)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

        Public Function GetGroups(ByVal businessUnit As String, ByVal partner As String, ByVal group01 As String, ByVal group02 As String, ByVal group03 As String, ByVal group04 As String, ByVal group05 As String, ByVal group06 As String, ByVal group07 As String, ByVal group08 As String, ByVal group09 As String) As DataTable
            Dim dtGroupDefaults As New DataTable
            Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_10TableAdapter 
            Dim cachename As String = "ECommerceGroupL1Defaults" & businessUnit & partner & group01 & group02 & group03 & group04 & group05 & group06 & group07 & group08 & group09
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cachename)  Then

                ' Get details
                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, partner, group01, group02, group03, group04, group05, group06, group07, group08, group09)
                If dtGroupDefaults.Rows.Count = 0 Then
                    ' Try for all partners
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(businessUnit, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06, group07, group08, group09)
                    If dtGroupDefaults.Rows.Count = 0 Then
                        ' Try for all business units
                        dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, partner, group01, group02, group03, group04, group05, group06, group07, group08, group09)
                        If dtGroupDefaults.Rows.Count = 0 Then
                            ' Try for all business units and partners
                            dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Groups(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group01, group02, group03, group04, group05, group06, group07, group08, group09)
                        End If
                    End If
                End If

                If dtGroupDefaults.Rows.Count > 0 Then
                    HttpContext.Current.Cache.Insert(cachename, _
                                                     dtGroupDefaults, _
                                                     Nothing, _
                                                     System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                     Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cachename)

                End If
            Else
                dtGroupDefaults = CType(HttpContext.Current.Cache.Item(cachename), DataTable)
            End If
            Return dtGroupDefaults
        End Function

    End Class

End Namespace

