Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_price_list_detail
    ''' </summary>
    <Serializable()> _
    Public Class tbl_price_list_detail
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_price_list_detail"


        ''' <summary>
        ''' To decide whether to continue the multiple insert
        ''' </summary>
        Private _continueInsertMultiple As Boolean = True

        ''' <summary>
        ''' to create only one instance of string when called by insert multiple
        ''' as it is a long string
        ''' as well as to speed up the insert when called under transaction 
        ''' </summary>
        Private ReadOnly _insertSQLStatement As String = String.Empty

        ''' <summary>
        ''' Gets or sets the product stock entity.
        ''' </summary>
        ''' <value>
        ''' The product stock entity.
        ''' </value>
        Public Property ProductPriceEntityList() As Generic.List(Of DEProductPrice)



#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_price_list_detail" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
            _insertSQLStatement = "INSERT tbl_price_list_detail " & _
                "(PRICE_LIST, PRODUCT, FROM_DATE, TO_DATE," & _
                "NET_PRICE, GROSS_PRICE, TAX_AMOUNT, " & _
                "SALE_NET_PRICE, SALE_GROSS_PRICE, SALE_TAX_AMOUNT, " & _
                "DELIVERY_NET_PRICE, DELIVERY_GROSS_PRICE, DELIVERY_TAX_AMOUNT," & _
                "PRICE_1, PRICE_2, PRICE_3, PRICE_4, PRICE_5," & _
                "TAX_CODE, TARIFF_CODE, PRICE_BREAK_CODE," & _
                "PRICE_BREAK_QUANTITY_1, SALE_PRICE_BREAK_QUANTITY_1," & _
                "NET_PRICE_2, GROSS_PRICE_2, TAX_AMOUNT_2, PRICE_BREAK_QUANTITY_2," & _
                "SALE_NET_PRICE_2, SALE_GROSS_PRICE_2, SALE_TAX_AMOUNT_2, SALE_PRICE_BREAK_QUANTITY_2," & _
                "NET_PRICE_3, GROSS_PRICE_3, TAX_AMOUNT_3, PRICE_BREAK_QUANTITY_3," & _
                "SALE_NET_PRICE_3, SALE_GROSS_PRICE_3, SALE_TAX_AMOUNT_3, SALE_PRICE_BREAK_QUANTITY_3," & _
                "NET_PRICE_4, GROSS_PRICE_4, TAX_AMOUNT_4, PRICE_BREAK_QUANTITY_4," & _
                "SALE_NET_PRICE_4, SALE_GROSS_PRICE_4, SALE_TAX_AMOUNT_4, SALE_PRICE_BREAK_QUANTITY_4," & _
                "NET_PRICE_5, GROSS_PRICE_5, TAX_AMOUNT_5, PRICE_BREAK_QUANTITY_5," & _
                "SALE_NET_PRICE_5, SALE_GROSS_PRICE_5, SALE_TAX_AMOUNT_5, SALE_PRICE_BREAK_QUANTITY_5," & _
                "NET_PRICE_6, GROSS_PRICE_6, TAX_AMOUNT_6, PRICE_BREAK_QUANTITY_6," & _
                "SALE_NET_PRICE_6, SALE_GROSS_PRICE_6, SALE_TAX_AMOUNT_6, SALE_PRICE_BREAK_QUANTITY_6," & _
                "NET_PRICE_7, GROSS_PRICE_7, TAX_AMOUNT_7, PRICE_BREAK_QUANTITY_7," & _
                "SALE_NET_PRICE_7, SALE_GROSS_PRICE_7, SALE_TAX_AMOUNT_7, SALE_PRICE_BREAK_QUANTITY_7," & _
                "NET_PRICE_8, GROSS_PRICE_8, TAX_AMOUNT_8, PRICE_BREAK_QUANTITY_8," & _
                "SALE_NET_PRICE_8, SALE_GROSS_PRICE_8, SALE_TAX_AMOUNT_8, SALE_PRICE_BREAK_QUANTITY_8," & _
                "NET_PRICE_9, GROSS_PRICE_9, TAX_AMOUNT_9, PRICE_BREAK_QUANTITY_9," & _
                "SALE_NET_PRICE_9, SALE_GROSS_PRICE_9, SALE_TAX_AMOUNT_9, SALE_PRICE_BREAK_QUANTITY_9," & _
                "NET_PRICE_10, GROSS_PRICE_10, TAX_AMOUNT_10, PRICE_BREAK_QUANTITY_10," & _
                "SALE_NET_PRICE_10, SALE_GROSS_PRICE_10, SALE_TAX_AMOUNT_10, SALE_PRICE_BREAK_QUANTITY_10," & _
                "DELIVERY_NET_PRICE_2, DELIVERY_GROSS_PRICE_2, DELIVERY_TAX_AMOUNT_2," & _
                "DELIVERY_SALE_NET_PRICE_2, DELIVERY_SALE_GROSS_PRICE_2, DELIVERY_SALE_TAX_AMOUNT_2," & _
                "DELIVERY_NET_PRICE_3, DELIVERY_GROSS_PRICE_3, DELIVERY_TAX_AMOUNT_3," & _
                "DELIVERY_SALE_NET_PRICE_3, DELIVERY_SALE_GROSS_PRICE_3, DELIVERY_SALE_TAX_AMOUNT_3," & _
                "DELIVERY_NET_PRICE_4, DELIVERY_GROSS_PRICE_4, DELIVERY_TAX_AMOUNT_4," & _
                "DELIVERY_SALE_NET_PRICE_4, DELIVERY_SALE_GROSS_PRICE_4, DELIVERY_SALE_TAX_AMOUNT_4," & _
                "DELIVERY_NET_PRICE_5, DELIVERY_GROSS_PRICE_5, DELIVERY_TAX_AMOUNT_5," & _
                "DELIVERY_SALE_NET_PRICE_5, DELIVERY_SALE_GROSS_PRICE_5, DELIVERY_SALE_TAX_AMOUNT_5," & _
                "DELIVERY_NET_PRICE_6, DELIVERY_GROSS_PRICE_6, DELIVERY_TAX_AMOUNT_6," & _
                "DELIVERY_SALE_NET_PRICE_6, DELIVERY_SALE_GROSS_PRICE_6, DELIVERY_SALE_TAX_AMOUNT_6," & _
                "DELIVERY_NET_PRICE_7, DELIVERY_GROSS_PRICE_7, DELIVERY_TAX_AMOUNT_7," & _
                "DELIVERY_SALE_NET_PRICE_7, DELIVERY_SALE_GROSS_PRICE_7, DELIVERY_SALE_TAX_AMOUNT_7," & _
                "DELIVERY_NET_PRICE_8, DELIVERY_GROSS_PRICE_8, DELIVERY_TAX_AMOUNT_8," & _
                "DELIVERY_SALE_NET_PRICE_8, DELIVERY_SALE_GROSS_PRICE_8, DELIVERY_SALE_TAX_AMOUNT_8," & _
                "DELIVERY_NET_PRICE_9, DELIVERY_GROSS_PRICE_9, DELIVERY_TAX_AMOUNT_9," & _
                "DELIVERY_SALE_NET_PRICE_9, DELIVERY_SALE_GROSS_PRICE_9, DELIVERY_SALE_TAX_AMOUNT_9," & _
                "DELIVERY_NET_PRICE_10, DELIVERY_GROSS_PRICE_10, DELIVERY_TAX_AMOUNT_10," & _
                "DELIVERY_SALE_NET_PRICE_10, DELIVERY_SALE_GROSS_PRICE_10, DELIVERY_SALE_TAX_AMOUNT_10) VALUES " & _
                "(@PRICE_LIST, @PRODUCT, @FROM_DATE, @TO_DATE," & _
                "@NET_PRICE, @GROSS_PRICE, @TAX_AMOUNT," & _
                "@SALE_NET_PRICE, @SALE_GROSS_PRICE, @SALE_TAX_AMOUNT," & _
                "@DELIVERY_NET_PRICE, @DELIVERY_GROSS_PRICE, @DELIVERY_TAX_AMOUNT," & _
                "@PRICE_1, @PRICE_2, @PRICE_3, @PRICE_4, @PRICE_5," & _
                "@TAX_CODE, @TARIFF_CODE, @PRICE_BREAK_CODE," & _
                "@PRICE_BREAK_QUANTITY_1, @SALE_PRICE_BREAK_QUANTITY_1," & _
                "@NET_PRICE_2, @GROSS_PRICE_2, @TAX_AMOUNT_2, @PRICE_BREAK_QUANTITY_2," & _
                "@SALE_NET_PRICE_2, @SALE_GROSS_PRICE_2, @SALE_TAX_AMOUNT_2, @SALE_PRICE_BREAK_QUANTITY_2," & _
                "@NET_PRICE_3, @GROSS_PRICE_3, @TAX_AMOUNT_3, @PRICE_BREAK_QUANTITY_3," & _
                "@SALE_NET_PRICE_3, @SALE_GROSS_PRICE_3, @SALE_TAX_AMOUNT_3, @SALE_PRICE_BREAK_QUANTITY_3," & _
                "@NET_PRICE_4, @GROSS_PRICE_4, @TAX_AMOUNT_4, @PRICE_BREAK_QUANTITY_4," & _
                "@SALE_NET_PRICE_4, @SALE_GROSS_PRICE_4, @SALE_TAX_AMOUNT_4, @SALE_PRICE_BREAK_QUANTITY_4," & _
                "@NET_PRICE_5, @GROSS_PRICE_5, @TAX_AMOUNT_5, @PRICE_BREAK_QUANTITY_5," & _
                "@SALE_NET_PRICE_5, @SALE_GROSS_PRICE_5, @SALE_TAX_AMOUNT_5, @SALE_PRICE_BREAK_QUANTITY_5," & _
                "@NET_PRICE_6, @GROSS_PRICE_6, @TAX_AMOUNT_6, @PRICE_BREAK_QUANTITY_6," & _
                "@SALE_NET_PRICE_6, @SALE_GROSS_PRICE_6, @SALE_TAX_AMOUNT_6, @SALE_PRICE_BREAK_QUANTITY_6," & _
                "@NET_PRICE_7, @GROSS_PRICE_7, @TAX_AMOUNT_7, @PRICE_BREAK_QUANTITY_7," & _
                "@SALE_NET_PRICE_7, @SALE_GROSS_PRICE_7, @SALE_TAX_AMOUNT_7, @SALE_PRICE_BREAK_QUANTITY_7," & _
                "@NET_PRICE_8, @GROSS_PRICE_8, @TAX_AMOUNT_8, @PRICE_BREAK_QUANTITY_8," & _
                "@SALE_NET_PRICE_8, @SALE_GROSS_PRICE_8, @SALE_TAX_AMOUNT_8, @SALE_PRICE_BREAK_QUANTITY_8," & _
                "@NET_PRICE_9, @GROSS_PRICE_9, @TAX_AMOUNT_9, @PRICE_BREAK_QUANTITY_9," & _
                "@SALE_NET_PRICE_9, @SALE_GROSS_PRICE_9, @SALE_TAX_AMOUNT_9, @SALE_PRICE_BREAK_QUANTITY_9," & _
                "@NET_PRICE_10, @GROSS_PRICE_10, @TAX_AMOUNT_10, @PRICE_BREAK_QUANTITY_10," & _
                "@SALE_NET_PRICE_10, @SALE_GROSS_PRICE_10, @SALE_TAX_AMOUNT_10, @SALE_PRICE_BREAK_QUANTITY_10," & _
                "@DELIVERY_NET_PRICE_2, @DELIVERY_GROSS_PRICE_2, @DELIVERY_TAX_AMOUNT_2," & _
                "@DELIVERY_SALE_NET_PRICE_2, @DELIVERY_SALE_GROSS_PRICE_2, @DELIVERY_SALE_TAX_AMOUNT_2," & _
                "@DELIVERY_NET_PRICE_3, @DELIVERY_GROSS_PRICE_3, @DELIVERY_TAX_AMOUNT_3," & _
                "@DELIVERY_SALE_NET_PRICE_3, @DELIVERY_SALE_GROSS_PRICE_3, @DELIVERY_SALE_TAX_AMOUNT_3," & _
                "@DELIVERY_NET_PRICE_4, @DELIVERY_GROSS_PRICE_4, @DELIVERY_TAX_AMOUNT_4," & _
                "@DELIVERY_SALE_NET_PRICE_4, @DELIVERY_SALE_GROSS_PRICE_4, @DELIVERY_SALE_TAX_AMOUNT_4," & _
                "@DELIVERY_NET_PRICE_5, @DELIVERY_GROSS_PRICE_5, @DELIVERY_TAX_AMOUNT_5," & _
                "@DELIVERY_SALE_NET_PRICE_5, @DELIVERY_SALE_GROSS_PRICE_5, @DELIVERY_SALE_TAX_AMOUNT_5," & _
                "@DELIVERY_NET_PRICE_6, @DELIVERY_GROSS_PRICE_6, @DELIVERY_TAX_AMOUNT_6," & _
                "@DELIVERY_SALE_NET_PRICE_6, @DELIVERY_SALE_GROSS_PRICE_6, @DELIVERY_SALE_TAX_AMOUNT_6," & _
                "@DELIVERY_NET_PRICE_7, @DELIVERY_GROSS_PRICE_7, @DELIVERY_TAX_AMOUNT_7," & _
                "@DELIVERY_SALE_NET_PRICE_7, @DELIVERY_SALE_GROSS_PRICE_7, @DELIVERY_SALE_TAX_AMOUNT_7," & _
                "@DELIVERY_NET_PRICE_8, @DELIVERY_GROSS_PRICE_8, @DELIVERY_TAX_AMOUNT_8," & _
                "@DELIVERY_SALE_NET_PRICE_8, @DELIVERY_SALE_GROSS_PRICE_8, @DELIVERY_SALE_TAX_AMOUNT_8," & _
                "@DELIVERY_NET_PRICE_9, @DELIVERY_GROSS_PRICE_9, @DELIVERY_TAX_AMOUNT_9," & _
                "@DELIVERY_SALE_NET_PRICE_9, @DELIVERY_SALE_GROSS_PRICE_9, @DELIVERY_SALE_TAX_AMOUNT_9," & _
                "@DELIVERY_NET_PRICE_10, @DELIVERY_GROSS_PRICE_10, @DELIVERY_TAX_AMOUNT_10," & _
                "@DELIVERY_SALE_NET_PRICE_10, @DELIVERY_SALE_GROSS_PRICE_10, @DELIVERY_SALE_TAX_AMOUNT_10) "

        End Sub
#End Region


#Region "Private Methods"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the price information for a given product code
        ''' </summary>
        ''' <param name="productCode">the given product code</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with max 1 record</returns>
        Public Function GetPricesByProductCode(ByVal productCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPricesByProductCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP 1 PRICE_LIST_DETAIL_ID, TAX_CODE, PRICE_LIST, NET_PRICE, " & _
                    "GROSS_PRICE, TAX_AMOUNT, SALE_NET_PRICE, SALE_GROSS_PRICE, SALE_TAX_AMOUNT, DELIVERY_NET_PRICE, " & _
                    "DELIVERY_GROSS_PRICE, DELIVERY_TAX_AMOUNT FROM tbl_price_list_detail WHERE PRODUCT = @ProductCode " & _
                    "ORDER BY PRICE_LIST_DETAIL_ID"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Updates a product price record with the given price list detail id.
        ''' </summary>
        ''' <param name="priceListDetailID">The price list detail id we are updating</param>
        ''' <param name="netPrice">The product net price</param>
        ''' <param name="grossPrice">The product gross price</param>
        ''' <param name="taxAmount">The product tax</param>
        ''' <param name="saleNetPrice">The product sale net price</param>
        ''' <param name="saleGrossPrice">The product sale gross price</param>
        ''' <param name="saleTaxAmount">The product sale tax amount</param>
        ''' <param name="deliveryNetPrice">The delivery charge net price</param>
        ''' <param name="deliveryGrossPrice">The delivery charge gross price</param>
        ''' <param name="deliveryTaxAmount">The delivery charge tax amount</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal priceListDetailID As String, ByVal netPrice As Decimal, ByVal grossPrice As Decimal, _
                               ByVal taxAmount As Decimal, ByVal saleNetPrice As Decimal, ByVal saleGrossPrice As Decimal, _
                               ByVal saleTaxAmount As Decimal, ByVal deliveryNetPrice As Decimal, ByVal deliveryGrossPrice As Decimal, _
                               ByVal deliveryTaxAmount As Decimal, ByVal VATCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceListDetailID", priceListDetailID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NetPrice", netPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GrossPrice", grossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TaxAmount", taxAmount))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SaleNetPrice", saleNetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SaleGrossPrice", saleGrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SaleTaxAmount", saleTaxAmount))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryNetPrice", deliveryNetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryGrossPrice", deliveryGrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryTaxAmount", deliveryTaxAmount))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TaxCode", VATCode))
            talentSqlAccessDetail.CommandElements.CommandText = "UPDATE tbl_price_list_detail SET " & _
                "[NET_PRICE] = @NetPrice, " & _
                "[GROSS_PRICE] = @GrossPrice, " & _
                "[TAX_AMOUNT] = @TaxAmount, " & _
                "[SALE_NET_PRICE] = @SaleNetPrice, " & _
                "[SALE_GROSS_PRICE] = @SaleGrossPrice, " & _
                "[SALE_TAX_AMOUNT] = @SaleTaxAmount, " & _
                "[DELIVERY_NET_PRICE] = @DeliveryNetPrice, " & _
                "[DELIVERY_GROSS_PRICE] = @DeliveryGrossPrice, " & _
                "[DELIVERY_TAX_AMOUNT] = @DeliveryTaxAmount, " & _
                "[TAX_CODE] = @TaxCode " & _
                "WHERE [PRICE_LIST_DETAIL_ID] = @PriceListDetailID"

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        Public Function Insert(ByVal partner As String, ByVal productPriceEntity As DEProductPrice, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

            '_insertSQLStatement initiated when instance is created

            talentSqlAccessDetail.CommandElements.CommandText = _insertSQLStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_LIST", productPriceEntity.PriceList))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT", productPriceEntity.ProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FROM_DATE", productPriceEntity.FromDate))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TO_DATE", productPriceEntity.ToDate))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE", productPriceEntity.PriceBreak1.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE", productPriceEntity.PriceBreak1.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT", productPriceEntity.PriceBreak1.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE", productPriceEntity.PriceBreak1.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE", productPriceEntity.PriceBreak1.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT", productPriceEntity.PriceBreak1.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE", productPriceEntity.PriceBreak1.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE", productPriceEntity.PriceBreak1.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT", productPriceEntity.PriceBreak1.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_1", productPriceEntity.Price1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_2", productPriceEntity.Price2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_3", productPriceEntity.Price3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_4", productPriceEntity.Price4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_5", productPriceEntity.Price5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_CODE", productPriceEntity.TaxCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TARIFF_CODE", productPriceEntity.TariffCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_CODE", productPriceEntity.PriceBreak1.PriceBreakCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_1", productPriceEntity.PriceBreak1.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_1", productPriceEntity.PriceBreak1.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_2", productPriceEntity.PriceBreak2.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_2", productPriceEntity.PriceBreak2.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_2", productPriceEntity.PriceBreak2.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_2", productPriceEntity.PriceBreak2.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_2", productPriceEntity.PriceBreak2.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_2", productPriceEntity.PriceBreak2.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_2", productPriceEntity.PriceBreak2.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_2", productPriceEntity.PriceBreak2.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_3", productPriceEntity.PriceBreak3.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_3", productPriceEntity.PriceBreak3.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_3", productPriceEntity.PriceBreak3.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_3", productPriceEntity.PriceBreak3.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_3", productPriceEntity.PriceBreak3.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_3", productPriceEntity.PriceBreak3.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_3", productPriceEntity.PriceBreak3.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_3", productPriceEntity.PriceBreak3.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_4", productPriceEntity.PriceBreak4.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_4", productPriceEntity.PriceBreak4.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_4", productPriceEntity.PriceBreak4.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_4", productPriceEntity.PriceBreak4.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_4", productPriceEntity.PriceBreak4.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_4", productPriceEntity.PriceBreak4.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_4", productPriceEntity.PriceBreak4.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_4", productPriceEntity.PriceBreak4.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_5", productPriceEntity.PriceBreak5.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_5", productPriceEntity.PriceBreak5.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_5", productPriceEntity.PriceBreak5.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_5", productPriceEntity.PriceBreak5.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_5", productPriceEntity.PriceBreak5.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_5", productPriceEntity.PriceBreak5.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_5", productPriceEntity.PriceBreak5.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_5", productPriceEntity.PriceBreak5.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_6", productPriceEntity.PriceBreak6.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_6", productPriceEntity.PriceBreak6.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_6", productPriceEntity.PriceBreak6.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_6", productPriceEntity.PriceBreak6.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_6", productPriceEntity.PriceBreak6.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_6", productPriceEntity.PriceBreak6.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_6", productPriceEntity.PriceBreak6.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_6", productPriceEntity.PriceBreak6.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_7", productPriceEntity.PriceBreak7.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_7", productPriceEntity.PriceBreak7.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_7", productPriceEntity.PriceBreak7.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_7", productPriceEntity.PriceBreak7.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_7", productPriceEntity.PriceBreak7.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_7", productPriceEntity.PriceBreak7.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_7", productPriceEntity.PriceBreak7.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_7", productPriceEntity.PriceBreak7.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_8", productPriceEntity.PriceBreak8.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_8", productPriceEntity.PriceBreak8.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_8", productPriceEntity.PriceBreak8.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_8", productPriceEntity.PriceBreak8.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_8", productPriceEntity.PriceBreak8.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_8", productPriceEntity.PriceBreak8.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_8", productPriceEntity.PriceBreak8.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_8", productPriceEntity.PriceBreak8.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_9", productPriceEntity.PriceBreak9.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_9", productPriceEntity.PriceBreak9.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_9", productPriceEntity.PriceBreak9.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_9", productPriceEntity.PriceBreak9.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_9", productPriceEntity.PriceBreak9.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_9", productPriceEntity.PriceBreak9.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_9", productPriceEntity.PriceBreak9.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_9", productPriceEntity.PriceBreak9.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NET_PRICE_10", productPriceEntity.PriceBreak10.NetPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROSS_PRICE_10", productPriceEntity.PriceBreak10.GrossPrice))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAX_AMOUNT_10", productPriceEntity.PriceBreak10.Tax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE_BREAK_QUANTITY_10", productPriceEntity.PriceBreak10.PriceBreakQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_NET_PRICE_10", productPriceEntity.PriceBreak10.SalePriceNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_GROSS_PRICE_10", productPriceEntity.PriceBreak10.SalePriceGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_TAX_AMOUNT_10", productPriceEntity.PriceBreak10.SalePriceTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SALE_PRICE_BREAK_QUANTITY_10", productPriceEntity.PriceBreak10.SalePriceBreakQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_2", productPriceEntity.PriceBreak2.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_2", productPriceEntity.PriceBreak2.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_2", productPriceEntity.PriceBreak2.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_2", productPriceEntity.PriceBreak2.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_2", productPriceEntity.PriceBreak2.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_2", productPriceEntity.PriceBreak2.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_3", productPriceEntity.PriceBreak3.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_3", productPriceEntity.PriceBreak3.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_3", productPriceEntity.PriceBreak3.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_3", productPriceEntity.PriceBreak3.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_3", productPriceEntity.PriceBreak3.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_3", productPriceEntity.PriceBreak3.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_4", productPriceEntity.PriceBreak4.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_4", productPriceEntity.PriceBreak4.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_4", productPriceEntity.PriceBreak4.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_4", productPriceEntity.PriceBreak4.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_4", productPriceEntity.PriceBreak4.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_4", productPriceEntity.PriceBreak4.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_5", productPriceEntity.PriceBreak5.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_5", productPriceEntity.PriceBreak5.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_5", productPriceEntity.PriceBreak5.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_5", productPriceEntity.PriceBreak5.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_5", productPriceEntity.PriceBreak5.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_5", productPriceEntity.PriceBreak5.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_6", productPriceEntity.PriceBreak6.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_6", productPriceEntity.PriceBreak6.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_6", productPriceEntity.PriceBreak6.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_6", productPriceEntity.PriceBreak6.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_6", productPriceEntity.PriceBreak6.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_6", productPriceEntity.PriceBreak6.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_7", productPriceEntity.PriceBreak7.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_7", productPriceEntity.PriceBreak7.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_7", productPriceEntity.PriceBreak7.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_7", productPriceEntity.PriceBreak7.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_7", productPriceEntity.PriceBreak7.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_7", productPriceEntity.PriceBreak7.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_8", productPriceEntity.PriceBreak8.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_8", productPriceEntity.PriceBreak8.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_8", productPriceEntity.PriceBreak8.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_8", productPriceEntity.PriceBreak8.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_8", productPriceEntity.PriceBreak8.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_8", productPriceEntity.PriceBreak8.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_9", productPriceEntity.PriceBreak9.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_9", productPriceEntity.PriceBreak9.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_9", productPriceEntity.PriceBreak9.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_9", productPriceEntity.PriceBreak9.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_9", productPriceEntity.PriceBreak9.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_9", productPriceEntity.PriceBreak9.SaleDeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_NET_PRICE_10", productPriceEntity.PriceBreak10.DeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_GROSS_PRICE_10", productPriceEntity.PriceBreak10.DeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_TAX_AMOUNT_10", productPriceEntity.PriceBreak10.DeliveryTax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_NET_PRICE_10", productPriceEntity.PriceBreak10.SaleDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_GROSS_PRICE_10", productPriceEntity.PriceBreak10.SaleDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DELIVERY_SALE_TAX_AMOUNT_10", productPriceEntity.PriceBreak10.SaleDeliveryTax))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        Public Function InsertMultiple(ByVal partner As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim totalAffectedRows As Integer = 0
            If Not (ProductPriceEntityList Is Nothing) Then
                If (ProductPriceEntityList.Count > 0) Then
                    _continueInsertMultiple = True
                    Dim affectedRows As Integer = 0
                    For productPriceEntityIndex As Integer = 0 To ProductPriceEntityList.Count - 1
                        'to make sure there is no error in the Insert method
                        If (_continueInsertMultiple) Then
                            affectedRows = Insert(partner, ProductPriceEntityList(productPriceEntityIndex), givenTransaction)
                            totalAffectedRows = totalAffectedRows + affectedRows
                        Else
                            totalAffectedRows = 0
                            Exit For
                        End If
                    Next
                End If
            End If
            'Return the results 
            Return totalAffectedRows
        End Function


        Public Function RemovePriceDetailByProductCode(ByVal productCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM tbl_price_list_detail Where PRODUCT=@ProductCode"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    affectedRows = 0
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function

#End Region

    End Class
End Namespace

