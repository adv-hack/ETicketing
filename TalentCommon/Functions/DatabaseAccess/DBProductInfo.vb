Imports System.Data.SqlClient

Public Class DBProductInfo
    Inherits DBAccess


    Private _products As DEProductInfo
    Public Property DEProducts() As DEProductInfo
        Get
            Return _products
        End Get
        Set(ByVal value As DEProductInfo)
            _products = value
        End Set
    End Property

    Public Sub New(ByVal products As DEProductInfo)
        MyBase.new()
        _products = products
    End Sub

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        Dim conTalent As SqlConnection = Nothing
        Dim cmd As New SqlCommand

        Try
            'Create a new connection with the SQL connection string
            conTalent = New SqlConnection(Settings.FrontEndConnectionString)
            cmd.Connection = conTalent
            cmd.Connection.Open()
        Catch ex As Exception
            If Not err.HasError Then
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ""
                    .ErrorNumber = ""
                    .HasError = True
                End With
            End If
        End Try

        err = GetProductInformationSQL2005(cmd)

        Try
            ' Close the connection
            cmd.Connection.Close()
        Catch ex As Exception
            If Not err.HasError Then
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ""
                    .ErrorNumber = "TACDBOR07"
                    .HasError = True
                End With
            End If
        End Try

        Return err
    End Function

    Protected Function GetProductInformationSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj

        Const SqlText As String = " SELECT DISTINCT " &
                                "          tbl_product.PRODUCT_DESCRIPTION_1, tbl_group_product.GROUP_L01_GROUP, tbl_group_product.GROUP_L02_GROUP, " &
                                "          tbl_group_product.GROUP_L03_GROUP, tbl_group_product.GROUP_L04_GROUP,  " &
                                "          tbl_group_product.GROUP_L05_GROUP, tbl_group_product.GROUP_L06_GROUP, tbl_group_product.GROUP_L07_GROUP,  " &
                                "          tbl_group_product.GROUP_L08_GROUP, tbl_group_product.GROUP_L09_GROUP, tbl_group_product.GROUP_L10_GROUP,  " &
                                "          tbl_product.PRODUCT_CODE, tbl_product.ALTERNATE_SKU, tbl_product.PRODUCT_DESCRIPTION_2,  " &
                                "          tbl_product.PRODUCT_DESCRIPTION_3, tbl_product.PRODUCT_DESCRIPTION_4, tbl_product.PRODUCT_DESCRIPTION_5,  " &
                                "          tbl_product.PRODUCT_LENGTH, tbl_product.PRODUCT_LENGTH_UOM, tbl_product.PRODUCT_WIDTH, tbl_product.PRODUCT_WIDTH_UOM,  " &
                                "          tbl_product.PRODUCT_DEPTH, tbl_product.PRODUCT_DEPTH_UOM, tbl_product.PRODUCT_HEIGHT, tbl_product.PRODUCT_HEIGHT_UOM,  " &
                                "          tbl_product.PRODUCT_SIZE, tbl_product.PRODUCT_SIZE_UOM, tbl_product.PRODUCT_WEIGHT, tbl_product.PRODUCT_WEIGHT_UOM,  " &
                                "          tbl_product.PRODUCT_VOLUME, tbl_product.PRODUCT_VOLUME_UOM, tbl_product.PRODUCT_COLOUR, tbl_product.PRODUCT_PACK_SIZE,  " &
                                "          tbl_product.PRODUCT_PACK_SIZE_UOM, tbl_product.PRODUCT_SUPPLIER_PART_NO, tbl_product.PRODUCT_CUSTOMER_PART_NO,  " &
                                "          tbl_product.PRODUCT_TASTING_NOTES_1, tbl_product.PRODUCT_TASTING_NOTES_2, tbl_product.PRODUCT_ABV, tbl_product.PRODUCT_VINTAGE,  " &
                                "          tbl_product.PRODUCT_SUPPLIER, tbl_product.PRODUCT_COUNTRY, tbl_product.PRODUCT_REGION, tbl_product.PRODUCT_AREA,  " &
                                "          tbl_product.PRODUCT_GRAPE, tbl_product.PRODUCT_CLOSURE, tbl_product.PRODUCT_CATALOG_CODE, tbl_product.PRODUCT_VEGETARIAN,  " &
                                "          tbl_product.PRODUCT_VEGAN, tbl_product.PRODUCT_ORGANIC, tbl_product.PRODUCT_BIODYNAMIC, tbl_product.PRODUCT_LUTTE,  " &
                                "          tbl_product.PRODUCT_MINIMUM_AGE, tbl_product.PRODUCT_HTML_1, tbl_product.PRODUCT_HTML_2, tbl_product.PRODUCT_HTML_3,  " &
                                "          tbl_product.PRODUCT_SEARCH_KEYWORDS, tbl_product.PRODUCT_PAGE_TITLE, tbl_product.PRODUCT_META_DESCRIPTION,  " &
                                "          tbl_product.PRODUCT_META_KEYWORDS, tbl_product.PRODUCT_SEARCH_RANGE_01, tbl_product.PRODUCT_SEARCH_RANGE_02,  " &
                                "          tbl_product.PRODUCT_SEARCH_RANGE_03, tbl_product.PRODUCT_SEARCH_RANGE_04, tbl_product.PRODUCT_SEARCH_RANGE_05,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_01, tbl_product.PRODUCT_SEARCH_CRITERIA_02, tbl_product.PRODUCT_SEARCH_CRITERIA_03,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_04, tbl_product.PRODUCT_SEARCH_CRITERIA_05, tbl_product.PRODUCT_SEARCH_CRITERIA_06,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_07, tbl_product.PRODUCT_SEARCH_CRITERIA_08, tbl_product.PRODUCT_SEARCH_CRITERIA_09,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_10, tbl_product.PRODUCT_SEARCH_CRITERIA_11, tbl_product.PRODUCT_SEARCH_CRITERIA_12,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_13, tbl_product.PRODUCT_SEARCH_CRITERIA_14, tbl_product.PRODUCT_SEARCH_CRITERIA_15,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_16, tbl_product.PRODUCT_SEARCH_CRITERIA_17, tbl_product.PRODUCT_SEARCH_CRITERIA_18,  " &
                                "          tbl_product.PRODUCT_SEARCH_CRITERIA_19, tbl_product.PRODUCT_SEARCH_CRITERIA_20, tbl_product.PRODUCT_SEARCH_SWITCH_01,  " &
                                "          tbl_product.PRODUCT_SEARCH_SWITCH_02, tbl_product.PRODUCT_SEARCH_SWITCH_03, tbl_product.PRODUCT_SEARCH_SWITCH_04,  " &
                                "          tbl_product.PRODUCT_SEARCH_SWITCH_05, tbl_product.PRODUCT_SEARCH_SWITCH_06, tbl_product.PRODUCT_SEARCH_SWITCH_07,  " &
                                "          tbl_product.PRODUCT_SEARCH_SWITCH_08, tbl_product.PRODUCT_SEARCH_SWITCH_09, tbl_product.PRODUCT_SEARCH_SWITCH_10,  " &
                                "          tbl_product.PRODUCT_SEARCH_DATE_01, tbl_product.PRODUCT_SEARCH_DATE_02, tbl_product.PRODUCT_SEARCH_DATE_03,  " &
                                "          tbl_product.PRODUCT_SEARCH_DATE_04, tbl_product.PRODUCT_SEARCH_DATE_05, tbl_product.DISCONTINUED, " & _
                                "          tbl_product.PRODUCT_OPTION_MASTER, tbl_product.PERSONALISABLE, tbl_group_product.SEQUENCE " & _
                                "           " &
                                " FROM      tbl_product WITH (NOLOCK) INNER JOIN " &
                                "           tbl_group_product WITH (NOLOCK) ON tbl_product.PRODUCT_CODE = tbl_group_product.PRODUCT " &
                                " WHERE    (tbl_group_product.GROUP_BUSINESS_UNIT = @BUSINESS_UNIT) " &
                                "           AND (tbl_group_product.GROUP_PARTNER = @PARTNER) " &
                                "           AND ({0}) "

        Try

            With cmd.Parameters
                .Clear()
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = DEProducts.BUSINESS_UNIT
                .Add("@PARTNER", SqlDbType.NVarChar).Value = DEProducts.PARTNER
            End With

            Dim where As String = ""

            Select Case DEProducts.Product_Codes.Count
                Case Is = 0
                    err.HasError = True
                    err.ErrorNumber = ""
                    err.ErrorMessage = "No Product Codes Supplied"
                Case Is = 1
                    where = "tbl_product.PRODUCT_CODE = @PRODUCT_CODE OR tbl_product.ALTERNATE_SKU = @PRODUCT_CODE"
                    cmd.Parameters.Add("@PRODUCT_CODE", SqlDbType.NVarChar).Value = DEProducts.Product_Codes(0)
                Case Else
                    For i As Integer = 0 To DEProducts.Product_Codes.Count - 1
                        where += "tbl_product.PRODUCT_CODE = @PRODUCT_CODE" & (i + 1).ToString & " OR tbl_product.ALTERNATE_SKU = @PRODUCT_CODE" & (i + 1).ToString
                        cmd.Parameters.Add("@PRODUCT_CODE" & (i + 1).ToString, SqlDbType.NVarChar).Value = DEProducts.Product_Codes(i)
                        If i < DEProducts.Product_Codes.Count - 1 Then
                            where += " OR "
                        End If
                    Next
            End Select

            If Not err.HasError Then
                cmd.CommandText = String.Format(SqlText, where)
                Dim da As New SqlDataAdapter
                da.SelectCommand = cmd
                Dim ds As New DataSet
                da.Fill(ds)
                If ds.Tables(0).Rows.Count = 0 Then
                    da.SelectCommand.Parameters.Item("@PARTNER").Value = Utilities.GetAllString
                    ds = New DataSet
                    da.Fill(ds)
                    If ds.Tables(0).Rows.Count = 0 Then
                        da.SelectCommand.Parameters.Item("@BUSINESS_UNIT").Value = Utilities.GetAllString
                        ds = New DataSet
                        da.Fill(ds)
                    End If
                End If
                ds.Tables(0).TableName = "ProductInformation"
                Me.ResultDataSet = FilterProductInfoResults(ds)
                da.Dispose()
                ds.Dispose()

            End If

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = "Error retrieving data"
        End Try

        Return err
    End Function


    Protected Function FilterProductInfoResults(ByVal ds As DataSet) As DataSet
        Dim inputTable As DataTable = ds.Tables("ProductInformation")
        Dim exists As Boolean = False
        Dim rows As New List(Of String)
        Dim rowsToRemove As New List(Of DataRow)

        For Each row As DataRow In inputTable.Rows
            exists = False
            For Each code As String In rows
                If row("PRODUCT_CODE") = code Then
                    exists = True
                    Exit For
                End If
            Next
            If exists Then
                rowsToRemove.Add(row)
            Else
                rows.Add(row("PRODUCT_CODE"))
            End If
        Next
        For Each row As DataRow In rowsToRemove
            inputTable.Rows.Remove(row)
        Next
        Return ds
    End Function

End Class
