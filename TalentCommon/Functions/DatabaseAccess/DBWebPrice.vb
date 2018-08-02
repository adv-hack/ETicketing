Imports System.Data.SqlClient
Public Class DBWebPrice
    Inherits DBAccess

    Private _products As Generic.Dictionary(Of String, WebPriceProduct)
    Public Property Products() As Generic.Dictionary(Of String, WebPriceProduct)
        Get
            Return _products
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, WebPriceProduct))
            _products = value
        End Set
    End Property

    Private _priceList As String
    Public Property Price_List() As String
        Get
            Return _priceList
        End Get
        Set(ByVal value As String)
            _priceList = value
        End Set
    End Property


    Private _secondaryPriceList As String
    Public Property SecondaryPriceList() As String
        Get
            Return _secondaryPriceList
        End Get
        Set(ByVal value As String)
            _secondaryPriceList = value
        End Set
    End Property



    Public Sub New(ByVal __products As Generic.Dictionary(Of String, WebPriceProduct), _
                    ByVal priceList As String, _
                    ByVal secondary_pricelist As String)
        MyBase.new()
        Me.Products = __products
        Me.Price_List = priceList
        Me.SecondaryPriceList = secondary_pricelist
    End Sub

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        'Get distinct master product codes
        Dim myProducts As New Generic.List(Of String)
        For Each prod As WebPriceProduct In Me.Products.Values
            If Not myProducts.Contains(prod.ProductCode) Then
                myProducts.Add(prod.ProductCode)
            End If
            If Not myProducts.Contains(prod.MasterProductCode) Then
                myProducts.Add(prod.MasterProductCode)
            End If
        Next

        Dim dt, dt2 As New DataTable

        Dim combDT As New DataTable
        If myProducts.Count > 2100 Then
            Dim noOfIts As Integer = 0
            noOfIts = CInt(myProducts.Count / 2100)
            If ((myProducts.Count / 2100) - noOfIts) > 0 Then
                noOfIts += 1
            End If

            For itterations As Integer = 0 To noOfIts - 1
                'chop the product codes up per 2100 (max no of params per sql tranaction)
                'loop through each and return the dt
                'combine the dts and replace the db call below

                Dim prods As New Generic.List(Of String)
                Dim lowerIndex As Integer = 2100 * itterations
                Dim upperIndex As Integer = lowerIndex + 2099

                If lowerIndex + 2099 > myProducts.Count Then
                    upperIndex = myProducts.Count
                End If

                For intCount As Integer = lowerIndex To upperIndex
                    prods.Add(myProducts(intCount))
                Next

                dt2 = Nothing
                dt2 = AccessDBSQL2005_GetWebPrices(prods, Me.Price_List, err)

                dt.Merge(dt2)

                dt2 = Nothing

            Next
        Else
            dt = AccessDBSQL2005_GetWebPrices(myProducts, Me.Price_List, err)

        End If

        'Now that we have got the prices from the price_list check to see if any of the products
        'do not have a valid price. If so make a similar db call using the secondary pricelist
        'and only including the products that currently have no price
        If dt.Rows.Count < Products.Count _
            AndAlso Not String.IsNullOrEmpty(Me.SecondaryPriceList) _
                AndAlso UCase(Me.Price_List) <> UCase(Me.SecondaryPriceList) Then

            Dim codes As New Generic.List(Of String)
            For Each code As String In myProducts
                Dim rows() As DataRow = dt.Select("PRODUCT = '" & code & "'")
                If rows.Length = 0 Then
                    codes.Add(code)
                End If
            Next
            dt2 = AccessDBSQL2005_GetWebPrices(codes, Me.SecondaryPriceList, err)


            'If there are any results from the db call then duplicate the row and add it to
            'the original datatable
            If dt2.Rows.Count > 0 Then
                For Each rw As DataRow In dt2.Rows
                    Dim myRow As DataRow = dt.NewRow
                    '-------------------------
                    'JDW - Edit
                    '------------------------
                    'For Each col As DataColumn In dt2.Columns
                    '    myRow(col.ColumnName) = rw(col.ColumnName)
                    'Next
                    'If Not dt.Rows.Contains(myRow) Then
                    '    dt.Rows.Add(myRow)
                    'End If

                    For i As Integer = 0 To dt2.Columns.Count - 1
                        myRow(i) = rw(i)
                    Next

                    Dim dr() As DataRow = dt.Select("PRODUCT = '" & myRow(2).ToString & "'")

                    If dr.Length = 0 Then
                        dt.Rows.Add(myRow)
                    End If
                    '-----------------------------
                    'JDW - Edit end
                    '-----------------------------
                Next
            End If
        End If

        Me.ResultDataSet = New DataSet
        Me.ResultDataSet.Tables.Add(dt)

        Return err
    End Function

    Protected Function AccessDBSQL2005_GetWebPrices(ByVal productCodes As Generic.List(Of String), ByVal pricelist As String, ByVal err As ErrorObj) As DataTable
        Dim dt As New DataTable

        Dim cmd As New SqlCommand("", New SqlConnection(Me.Settings.BackOfficeConnectionString))

        Dim SelectDetail As String = " SELECT * FROM tbl_price_list_detail WITH (NOLOCK) " & _
                                    " WHERE PRICE_LIST = @PriceList " & _
                                    " AND ("
        Dim i As Integer = productCodes.Count
        Dim count As Integer = 0
        For Each key As String In productCodes
            count += 1
            If Not count = i Then
                SelectDetail += " PRODUCT = @ProductCode" & count.ToString & " OR "
                cmd.Parameters.Add("@ProductCode" & count.ToString, SqlDbType.NVarChar).Value = key
            Else
                SelectDetail += " PRODUCT = @ProductCode" & count.ToString
                cmd.Parameters.Add("@ProductCode" & count.ToString, SqlDbType.NVarChar).Value = key
                Exit For
            End If
        Next
        SelectDetail += " ) ORDER BY TO_DATE DESC  "


        With cmd.Parameters
            .Add("@PriceList", SqlDbType.NVarChar).Value = pricelist
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
            err.ErrorMessage = "Failed to connect to the SQL DB"
            err.ErrorNumber = "TWP001"
            err.HasError = True
        End Try

        Try
            If cmd.Connection.State = ConnectionState.Open Then
                cmd.CommandText = SelectDetail
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)
            End If
        Catch ex As Exception
        End Try

        Try
            cmd.Connection.Close()
        Catch ex As Exception

        End Try

        Return dt
    End Function

    Public Function GetPriceBreakDescription(ByVal qualifier As String, ByVal type As String) As DataTable
        Dim descriptions As New DataTable
        Try
            Dim SelectStr As String = " SELECT  tbl_ebusiness_descriptions_bu.QUALIFIER, tbl_ebusiness_descriptions_bu.BUSINESS_UNIT, " & _
                                        "       tbl_ebusiness_descriptions_bu.PARTNER, tbl_ebusiness_descriptions_bu.DESCRIPTION_TYPE, " & _
                                        "       tbl_ebusiness_descriptions_bu.DESCRIPTION_CODE, tbl_ebusiness_descriptions.DESCRIPTION_DESCRIPTION,  " & _
                                        "       tbl_ebusiness_descriptions_lang.DESCRIPTION_DESCRIPTION AS LANGUAGE_DESCRIPTION " & _
                                        " FROM  tbl_ebusiness_descriptions WITH (NOLOCK)  " & _
                                        "       INNER JOIN tbl_ebusiness_descriptions_bu WITH (NOLOCK)  " & _
                                        "       ON tbl_ebusiness_descriptions.DESCRIPTION_CODE = tbl_ebusiness_descriptions_bu.DESCRIPTION_CODE  " & _
                                        "       AND  tbl_ebusiness_descriptions.DESCRIPTION_TYPE = tbl_ebusiness_descriptions_bu.DESCRIPTION_TYPE  " & _
                                        "       LEFT OUTER JOIN tbl_ebusiness_descriptions_lang WITH (NOLOCK)  " & _
                                        "       ON tbl_ebusiness_descriptions_bu.DESCRIPTION_CODE = tbl_ebusiness_descriptions_lang.DESCRIPTION_CODE  " & _
                                        "       AND  tbl_ebusiness_descriptions_bu.DESCRIPTION_TYPE = tbl_ebusiness_descriptions_lang.DESCRIPTION_TYPE " & _
                                        " WHERE (tbl_ebusiness_descriptions_bu.QUALIFIER = @Qualifier)  " & _
                                        "       AND (tbl_ebusiness_descriptions_bu.BUSINESS_UNIT = @BusinessUnit)  " & _
                                        "       AND (tbl_ebusiness_descriptions_bu.PARTNER = @Partner)  " & _
                                        "       AND (tbl_ebusiness_descriptions_bu.DESCRIPTION_TYPE = @Type) " & _
                                        "       AND (tbl_ebusiness_descriptions_lang.DESCRIPTION_LANGUAGE = @LanguageCode) "
            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(Me.Settings.BackOfficeConnectionString))

            With cmd.Parameters
                .Add("@BusinessUnit", SqlDbType.NVarChar).Value = CType(Me.Settings, DEWebPriceSetting).BusinessUnit
                .Add("@Partner", SqlDbType.NVarChar).Value = CType(Me.Settings, DEWebPriceSetting).Partner
                .Add("@Qualifier", SqlDbType.NVarChar).Value = qualifier
                .Add("@Type", SqlDbType.NVarChar).Value = type
                .Add("@LanguageCode", SqlDbType.NVarChar).Value = CType(Me.Settings, DEWebPriceSetting).Language
            End With

            Try
                cmd.Connection.Open()
            Catch ex As Exception

            End Try

            Try
                If cmd.Connection.State = ConnectionState.Open Then

                    Dim da As New SqlDataAdapter(cmd)

                    da.Fill(descriptions)
                    If Not descriptions.Rows.Count > 0 Then
                        cmd.Parameters("@Partner").Value = Utilities.GetAllString
                        da.Fill(descriptions)
                        If Not descriptions.Rows.Count > 0 Then
                            cmd.Parameters("@BusinessUnit").Value = Utilities.GetAllString
                            da.Fill(descriptions)
                        End If
                    End If

                End If
            Catch ex As Exception
            End Try

            Try
                cmd.Connection.Close()
            Catch ex As Exception
            End Try
        Catch ex As Exception

        End Try
        Return descriptions
    End Function

    Public Function GetFreeDeliveryValue_SQL2005() As Decimal
        Dim freeVal As Decimal = 0
        Try
            Dim SelectStr As String = " SELECT * FROM tbl_price_list_header WITH (NOLOCK) " & _
                                                   " WHERE PRICE_LIST = @PriceList "
            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(Me.Settings.BackOfficeConnectionString))

            With cmd.Parameters
                .Add("@PriceList", SqlDbType.NVarChar).Value = Me.Price_List
            End With

            Try
                cmd.Connection.Open()
            Catch ex As Exception

            End Try

            Try
                If cmd.Connection.State = ConnectionState.Open Then

                    Dim dr As SqlDataReader = cmd.ExecuteReader

                    If dr.HasRows Then
                        dr.Read()
                        freeVal = Utilities.CheckForDBNull_Decimal(dr("FREE_DELIVERY_VALUE"))
                        dr.Close()
                    End If

                End If
            Catch ex As Exception
            End Try

            Try
                cmd.Connection.Close()
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try

        Return freeVal

    End Function



End Class
