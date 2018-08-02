Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Templates Class
'
'       Date                        19th Jan 2007
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      APTEMP- 
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.eCommerce

    Public Class Templates
        Public Shared Function GetPageTemplate(ByVal businessUnit As String, ByVal partner As String) As String
            '-----------------------------------------------------------------------
            '   GetPageTemplate - Retreive the correct masterpage for the current page
            '-----------------------------------------------------------------------
            Dim pageTemplate As String = String.Empty
            Dim err As New ErrorObj
            Dim conTalent As SqlConnection = Nothing
            Dim currentPage As String = Talent.eCommerce.Utilities.GetCurrentPageName().ToLower
            Dim cacheKey As String = "TEMPLATE" & Talent.Common.Utilities.FixStringLength(businessUnit, 50) & currentPage

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
                pageTemplate = HttpContext.Current.Cache.Item(cacheKey)
            Else
                '--------------------------------------------------------------------
                Try
                    Const SqlServer2005 As String = "TalentEBusinessDBConnectionString"
                    conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                    conTalent.Open()
                Catch ex As Exception
                    Const strError1 As String = "Could not establish connection to the database"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError1
                        .ErrorNumber = "APTEMP-01"
                        .HasError = True
                    End With
                End Try
                '---------------------------------------------------------------------
                If Not err.HasError Then
                    Try
                        '------------------------
                        ' Get Masterpage for Page
                        '------------------------
                        Const strSelect As String = "SELECT TEMPLATE_NAME FROM TBL_TEMPLATE_PAGE WITH (NOLOCK)  WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER = @PARTNER AND PAGE_NAME = @PAGE"
                        Const strSelect4 As String = "SELECT TEMPLATE_NAME FROM TBL_TEMPLATE WITH (NOLOCK)  WHERE DEFAULT_FLAG = 'Y'"
                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conTalent)

                        '----------------------------------
                        ' Try for business unit and partner
                        '----------------------------------
                        cmdSelect.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                        cmdSelect.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = partner
                        cmdSelect.Parameters.Add(New SqlParameter("@PAGE", SqlDbType.Char, 50)).Value = currentPage
                        Dim dtrTemplates As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtrTemplates.HasRows Then
                            dtrTemplates.Read()
                            pageTemplate = dtrTemplates("TEMPLATE_NAME")
                            dtrTemplates.Close()
                        Else
                            dtrTemplates.Close()
                            '---------------------------------------
                            ' Try for business unit and all partners
                            '---------------------------------------
                            cmdSelect = New SqlCommand(strSelect, conTalent)
                            cmdSelect.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = businessUnit
                            cmdSelect.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                            cmdSelect.Parameters.Add(New SqlParameter("@PAGE", SqlDbType.Char, 50)).Value = currentPage
                            dtrTemplates = cmdSelect.ExecuteReader
                            If dtrTemplates.HasRows Then
                                dtrTemplates.Read()
                                pageTemplate = dtrTemplates("TEMPLATE_NAME")
                                dtrTemplates.Close()
                            Else
                                dtrTemplates.Close()
                                '--------------------------------------------
                                ' Try for all business units and all partners
                                '--------------------------------------------
                                cmdSelect = New SqlCommand(strSelect, conTalent)
                                cmdSelect.Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect.Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Talent.Common.Utilities.GetAllString
                                cmdSelect.Parameters.Add(New SqlParameter("@PAGE", SqlDbType.Char, 50)).Value = currentPage
                                dtrTemplates = cmdSelect.ExecuteReader
                                If dtrTemplates.HasRows Then
                                    dtrTemplates.Read()
                                    pageTemplate = dtrTemplates("TEMPLATE_NAME")
                                    dtrTemplates.Close()
                                Else
                                    dtrTemplates.Close()
                                    '----------------------
                                    ' Try and get a default
                                    '----------------------
                                    cmdSelect = New SqlCommand(strSelect4, conTalent)
                                    dtrTemplates = cmdSelect.ExecuteReader
                                    If dtrTemplates.HasRows Then
                                        dtrTemplates.Read()
                                        pageTemplate = dtrTemplates("TEMPLATE_NAME")
                                    End If
                                    dtrTemplates.Close()
                                End If
                            End If
                        End If

                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "APTEMP-06"
                            .HasError = True
                        End With
                    End Try
                End If
                '------
                ' Close
                '------
                Try
                    conTalent.Close()
                Catch ex As Exception
                    Const strError9 As String = "Failed to close database connection"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError9
                        .ErrorNumber = "APTEMP-07"
                        .HasError = True
                    End With
                End Try

                If pageTemplate <> String.Empty Then
                    pageTemplate = pageTemplate.Trim & ".master"
                End If

                HttpContext.Current.Cache.Insert(cacheKey, _
                                                 pageTemplate, _
                                                 Nothing, _
                                                 System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), _
                                                 Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

            Return pageTemplate
        End Function

    End Class
End Namespace

