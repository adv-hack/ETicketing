Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce

Public Class TalentTicketingMenu


    Private _TicketingMenuItems As Generic.List(Of TalentTicketingMenuItem)
    Public Property TicketingMenuItems() As Generic.List(Of TalentTicketingMenuItem)
        Get
            Return _TicketingMenuItems
        End Get
        Set(ByVal value As Generic.List(Of TalentTicketingMenuItem))
            _TicketingMenuItems = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()
        Me.TicketingMenuItems = New Generic.List(Of TalentTicketingMenuItem)
    End Sub

    Public Function TicketingProductIsActive(ByVal ProductType As String, Optional ByVal location As String = "") As Boolean

        Dim active As Boolean = False

        For Each ttmi As TalentTicketingMenuItem In Me.TicketingMenuItems
            If UCase(ttmi.Product_Type) = UCase(ProductType) _
            And Utilities.CheckForDBNull_Boolean_DefaultFalse(ttmi.Master) _
            And ttmi.Location = "" Then
                active = True
                Exit For
            End If
        Next

        Return active
    End Function

    Public Sub LoadTicketingProducts(ByVal businessUnit As String, _
                                    ByVal partner As String, _
                                    ByVal languageCode As String)

        Dim cacheKey As String = "TicketingProductsMenu_" & businessUnit & "_" & partner & "_" & languageCode
        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Me.TicketingMenuItems = CType(HttpContext.Current.Cache.Item(cacheKey), Generic.List(Of TalentTicketingMenuItem))
        Else
            Dim products As New DataTable
            products = GetTicketingProducts(businessUnit, partner, languageCode)

            If products.Rows.Count > 0 Then

                Dim allStr As String = Utilities.GetAllString

                'Filter the table
                '================================
                Dim myRows() As DataRow = products.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                                            " AND PARTNER = '" & partner & "'" & _
                                                            " AND LANGUAGE_CODE = '" & languageCode & "'")

                If myRows.Length = 0 Then
                    myRows = products.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND LANGUAGE_CODE = '" & languageCode & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = products.Select("BUSINESS_UNIT = '" & allStr & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND LANGUAGE_CODE = '" & languageCode & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = products.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                            " AND PARTNER = '" & partner & "'" & _
                                            " AND LANGUAGE_CODE = '" & allStr & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = products.Select("BUSINESS_UNIT = '" & businessUnit & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND LANGUAGE_CODE = '" & allStr & "'")
                End If
                If myRows.Length = 0 Then
                    myRows = products.Select("BUSINESS_UNIT = '" & allStr & "'" & _
                                            " AND PARTNER = '" & allStr & "'" & _
                                            " AND LANGUAGE_CODE = '" & allStr & "'")
                End If

                If myRows.Length > 0 Then

                    Dim ttmi As New TalentTicketingMenuItem
                    Dim ttmiProperties As ArrayList = Utilities.GetPropertyNames(ttmi)
                    'Add Active menu items to the TicketingMenuItems array
                    For Each prodRow2 As DataRow In myRows
                        ttmi = New TalentTicketingMenuItem
                        ttmi = Utilities.PopulateProperties(ttmiProperties, prodRow2, ttmi)
                        Me.TicketingMenuItems.Add(ttmi)
                    Next
                End If
            End If

            HttpContext.Current.Cache.Insert(cacheKey, Me.TicketingMenuItems, Nothing, System.DateTime.Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

        End If

    End Sub

    Protected Function GetTicketingProducts(ByVal businessUnit As String, _
                                            ByVal partner As String, _
                                            ByVal languageCode As String) As DataTable

        Dim productsMaster As New DataTable
        Dim selectStr As String = _
        "    SELECT" & _
        "    	(   select isnull(ACTIVE, 'TRUE')" & _
        "           from tbl_ticketing_products" & _
        "    		where isnull(LOCATION, '') = ''" & _
        "    		and PRODUCT_TYPE = tpl.PRODUCT_TYPE" & _
        "    	) MASTER," & _
        "    	tp.ACTIVE," & _
        "    	tp.LOCATION," & _
        "    	tp.DISPLAY_SEQUENCE," & _
        "    	tpl.*" & _
        "    FROM tbl_ticketing_products tp" & _
        "    INNER JOIN tbl_ticketing_products_lang tpl" & _
        "    	ON tp.BUSINESS_UNIT = tpl.BUSINESS_UNIT" & _
        "    	AND tp.PARTNER = tpl.PARTNER" & _
        "    	AND tp.PRODUCT_TYPE = tpl.PRODUCT_TYPE" & _
        "    WHERE tp.BUSINESS_UNIT IN (@AllStr, @BusinessUnit)" & _
        "    AND tp.PARTNER IN (@AllStr, @Partner)" & _
        "    AND tpl.LANGUAGE_CODE IN (@AllStr, @LanguageCode)" & _
        "    ORDER BY DISPLAY_SEQUENCE"

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
                .Add("@LanguageCode", SqlDbType.NVarChar).Value = languageCode
                .Add("@AllStr", SqlDbType.NVarChar).Value = Utilities.GetAllString
            End With
            Dim menuTA As New SqlDataAdapter(cmd)
            menuTA.Fill(productsMaster)
        End If

        Try
            cmd.Connection.Close()
        Catch ex As Exception
        End Try

        Return productsMaster
    End Function

End Class
