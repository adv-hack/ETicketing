Imports System.Data.SqlClient

Partial Class UserControls_ProductAssociations
    Inherits ControlBase

    Private _pagePosition As Integer
    Public Property PagePosition() As Integer
        Get
            Return _pagePosition
        End Get
        Set(ByVal value As Integer)
            _pagePosition = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        BindRepeater()
    End Sub

    Protected Sub BindRepeater()
        Dim dt As New Data.DataTable
        Dim cacheString As New StringBuilder
        cacheString.Append("ProductAssociationsRepeater - ")
        cacheString.Append(TalentCache.GetBusinessUnit)
        cacheString.Append("|")
        cacheString.Append(Talent.eCommerce.Utilities.GetCurrentPageName.ToLower)
        cacheString.Append("|")
        cacheString.Append(PagePosition.ToString())
        Dim cacheKey As String = cacheString.ToString()

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            dt = HttpContext.Current.Cache.Item(cacheKey)
        Else
            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Const SelectCmd As String = " SELECT * FROM tbl_product_relations_defaults WITH (NOLOCK) " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "  AND PARTNER = @PARTNER " & _
                                        "  AND PAGE_CODE = @PAGE_CODE " & _
                                        "  AND ONOFF = 'True' " & _
                                        "  AND PAGE_POSITION = @PAGE_POSITION " & _
                                        " ORDER BY SEQUENCE "

            cmd.CommandText = SelectCmd
            With cmd.Parameters
                .Add("@BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                .Add("@PAGE_CODE", Data.SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentPageName
                .Add("@PAGE_POSITION", Data.SqlDbType.NVarChar).Value = PagePosition.ToString
            End With
            Try
                cmd.Connection.Open()
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)
                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                        cmd.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                        da.Fill(dt)
                        If dt.Rows.Count = 0 Then
                            cmd.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                            cmd.Parameters("@PARTNER").Value = Talent.Common.Utilities.GetAllString
                            da.Fill(dt)
                        End If
                    End If
                End If
            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try
            'Cache the result
            TalentCache.AddPropertyToCache(cacheKey, dt, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        End If

        If dt.Rows.Count > 0 Then
            AssociatedProductsRepeater.DataSource = dt
            AssociatedProductsRepeater.DataBind()
            Me.Visible = True
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub AssociatedProductsRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles AssociatedProductsRepeater.ItemDataBound
        Dim ri As RepeaterItem = e.Item
        Dim prg As UserControls_ProductRelationsGraphical = CType(e.Item.FindControl("prg"), UserControls_ProductRelationsGraphical)
        Dim prg2 As UserControls_ProductRelationsGraphical2 = CType(e.Item.FindControl("prg2"), UserControls_ProductRelationsGraphical2)
        Dim uscProductRelationsGraphical3 As UserControls_ProductRelationsGraphical3 = CType(e.Item.FindControl("uscProductRelationsGraphical3"), UserControls_ProductRelationsGraphical3)
        Dim dr As Data.DataRow = CType(e.Item.DataItem, Data.DataRowView).Row
        Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        Dim eComDefs As New Talent.eCommerce.ECommerceModuleDefaults
        def = eComDefs.GetDefaults

        prg.Visible = False
        prg2.Visible = False
        uscProductRelationsGraphical3.Visible = False
        Select Case def.ProductRelationshipsTemplateType
            Case Is = 1
                prg.Usage = dr("QUALIFIER")
                prg.Display = dr("ONOFF")
                prg.PagePosition = Me.PagePosition
                prg.TemplateType = dr("TEMPLATE_TYPE")
                prg.Visible = True
            Case Is = 2
                prg2.Usage = dr("QUALIFIER")
                prg2.Display = dr("ONOFF")
                prg2.PagePosition = Me.PagePosition
                prg2.TemplateType = dr("TEMPLATE_TYPE")
                prg2.Visible = True
            Case Is = 3
                uscProductRelationsGraphical3.PagePosition = Me.PagePosition
                uscProductRelationsGraphical3.TemplateType = dr("TEMPLATE_TYPE")
                uscProductRelationsGraphical3.Visible = True
            Case Else
                prg.Usage = dr("QUALIFIER")
                prg.Display = dr("ONOFF")
                prg.PagePosition = Me.PagePosition
                prg.TemplateType = dr("TEMPLATE_TYPE")
                prg.Visible = True
        End Select
    End Sub
End Class