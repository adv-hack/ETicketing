Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce

Public Class TalentMenu


    Private _id As Long
    Public Property ID() As Long
        Get
            Return _id
        End Get
        Set(ByVal value As Long)
            _id = value
        End Set
    End Property

    Private _menuID As String
    Public Property Menu_ID() As String
        Get
            Return _menuID
        End Get
        Set(ByVal value As String)
            _menuID = value
        End Set
    End Property


    Private _DESC As String
    Public Property Description() As String
        Get
            Return _DESC
        End Get
        Set(ByVal value As String)
            _DESC = value
        End Set
    End Property


    Private _MenuItems As Generic.List(Of TalentMenuItem)
    Public Property MenuItems() As Generic.List(Of TalentMenuItem)
        Get
            Return _MenuItems
        End Get
        Set(ByVal value As Generic.List(Of TalentMenuItem))
            _MenuItems = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        Me.MenuItems = New Generic.List(Of TalentMenuItem)
    End Sub

    Public Sub LoadMenu(ByVal menuID As String, _
                        ByVal languageCode As String)

        Dim cacheKey As String = "MenuItems_" & menuID & "_" & languageCode
        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey)  Then
            Me.MenuItems = CType(HttpContext.Current.Cache.Item(cacheKey), Generic.List(Of TalentMenuItem))
        Else
            Dim items As DataTable = GetMenu(menuID, languageCode)

            If items.Rows.Count > 0 Then
                Dim tmi As New TalentMenuItem
                Dim tmiProperties As ArrayList = Utilities.GetPropertyNames(tmi)
                For Each item As DataRow In items.Rows
                    tmi = New TalentMenuItem
                    tmi = Utilities.PopulateProperties(tmiProperties, item, tmi)
                    Me.MenuItems.Add(tmi)
                Next
            End If

            HttpContext.Current.Cache.Insert(cacheKey, Me.MenuItems, Nothing, System.DateTime.Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

        End If



    End Sub

    Protected Function GetMenu(ByVal menuID As String, _
                                ByVal languageCode As String) As DataTable

        Dim myMenuItems As New DataTable
        Const selectStr As String = " SELECT * " & _
                                    " FROM tbl_navigation_menu_item_lang WITH (NOLOCK)   " & _
                                    " WHERE MENU_ID = @MenuID " & _
                                    " AND LANGUAGE_CODE IN (@AllStr, @LanguageCode) " & _
                                    " ORDER BY MENU_SEQUENCE "

        Dim cmd As New SqlCommand(selectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            With cmd.Parameters
                .Clear()
                .Add("@MenuID", SqlDbType.NVarChar).Value = menuID
                .Add("@LanguageCode", SqlDbType.NVarChar).Value = languageCode
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
