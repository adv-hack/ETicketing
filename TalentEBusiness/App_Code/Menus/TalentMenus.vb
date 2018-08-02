Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce


Public Class TalentMenus


    Private _Menus As Generic.List(Of TalentMenu)
    Public Property Menus() As Generic.List(Of TalentMenu)
        Get
            Return _Menus
        End Get
        Set(ByVal value As Generic.List(Of TalentMenu))
            _Menus = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        Me.Menus = New Generic.List(Of TalentMenu)
    End Sub

    Public Sub LoadMenus(ByVal businessUnit As String, _
                         ByVal partner As String, _
                         ByVal page As String, _
                         ByVal location As String, _
                         ByVal position As String)

        Dim isAgent As Boolean = Utilities.IsAgent
        Dim cacheKey As String = "TalentMenus_" & businessUnit & "_" & partner & "_" & page & "_" & location & "_" & position & "_" & isAgent.ToString()
        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
            Me.Menus = CType(HttpContext.Current.Cache.Item(cacheKey), Generic.List(Of TalentMenu))
        Else
            
            Dim myMenus As DataTable = GetMenus(businessUnit, partner, page, location, position, isAgent)
            If myMenus.Rows.Count > 0 Then

                Dim allStr As String = Utilities.GetAllString

                'Filter the table
                '================================
                Dim myRows() As DataRow = myMenus.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                                            " AND PARTNER = '" & partner & "'" & _
                                                            " AND PAGE = '" & page & "'")

                If myRows.Length = 0 Then
                    myRows = myMenus.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND PAGE = '" & page & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = myMenus.Select("BUSINESS_UNIT = '" & allStr & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND PAGE = '" & page & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = myMenus.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                            " AND PARTNER = '" & partner & "'" & _
                                            " AND PAGE = '" & allStr & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = myMenus.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND PAGE = '" & allStr & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = myMenus.Select("BUSINESS_UNIT = '" & allStr & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND PAGE = '" & allStr & "'")
                End If

                If myRows.Length > 0 Then
                    Dim tm As New TalentMenu
                    Dim tmProperties As ArrayList = Utilities.GetPropertyNames(tm)
                    For Each menuRow As DataRow In myRows
                        tm = New TalentMenu
                        tm = Utilities.PopulateProperties(tmProperties, menuRow, tm)
                        tm.LoadMenu(tm.Menu_ID, Utilities.GetCurrentLanguage)
                        Me.Menus.Add(tm)
                    Next
                End If
            End If

            HttpContext.Current.Cache.Insert(cacheKey, Me.Menus, Nothing, System.DateTime.Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

        End If

    End Sub

    Protected Function GetMenus(ByVal businessUnit As String, _
                                 ByVal partner As String, _
                                 ByVal page As String, _
                                 ByVal location As String, _
                                 ByVal position As String, _
                                 ByVal isAgent As Boolean) As DataTable

        Dim myMenuItems As New DataTable
        Dim selectStr As String = " SELECT * " & _
                                    " FROM tbl_navigation_site_menus WITH (NOLOCK)   " & _
                                    " WHERE BUSINESS_UNIT IN (@AllStr, @BusinessUnit) " & _
                                    " AND PARTNER IN (@AllStr, @Partner) " & _
                                    " AND PAGE IN (@AllStr, @Page) " & _
                                    " AND LOCATION = @Location " & _
                                    " AND POSITION = @Position "
        If isAgent Then
            selectStr += " AND (isnull(AGENT_MODE_BEHAVIOUR,'') = 'ON' OR isnull(AGENT_MODE_BEHAVIOUR,'') = '') "
        Else
            selectStr += " AND isnull(AGENT_MODE_BEHAVIOUR, '') <> 'ON' "
        End If
        selectStr += " ORDER BY SEQUENCE "

        Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            With cmd.Parameters
                .Clear()
                .Add("@BusinessUnit", SqlDbType.NVarChar).Value = businessUnit
                .Add("@Partner", SqlDbType.NVarChar).Value = partner
                .Add("@Page", SqlDbType.NVarChar).Value = page
                .Add("@Location", SqlDbType.NVarChar).Value = location
                .Add("@Position", SqlDbType.NVarChar).Value = position
                .Add("@AllStr", SqlDbType.NVarChar).Value = Utilities.GetAllString
            End With
            Dim menuTA As New SqlDataAdapter(cmd)
            menuTA.Fill(myMenuItems)
        End If

        Try
            cmd.Connection.Close()
        Catch ex As Exception
        End Try

        Return myMenuItems
    End Function

End Class
