Imports System.Data.SqlClient
Imports System.Text
Imports Talent.Common.DataObjects.TableObjects
Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to interface with the order tables
    ''' </summary>
    <Serializable()> _
    Public Class Order
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblOrderStatus As tbl_order_status
        Private _tblOrderheader As tbl_order_header
        Private _tblOrderDetail As tbl_order_detail
        Private _tblGiftMessage As tbl_gift_message

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Order"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Order" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the _tblOrderStatus instance with DESettings
        ''' </summary>
        ''' <value>tbl_order_status instance</value>
        Public ReadOnly Property TblOrderStatus() As tbl_order_status
            Get
                If (_tblOrderStatus Is Nothing) Then
                    _tblOrderStatus = New tbl_order_status(_settings)
                End If
                Return _tblOrderStatus
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the _tblOrderHeader instance with DESettings
        ''' </summary>
        ''' <value>tbl_order_header instance</value>
        Public ReadOnly Property TblOrderHeader() As tbl_order_header
            Get
                If (_tblOrderheader Is Nothing) Then
                    _tblOrderheader = New tbl_order_header(_settings)
                End If
                Return _tblOrderheader
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_order_detail instance with DESettings
        ''' </summary>
        ''' <value>tbl_order_header instance</value>
        Public ReadOnly Property TblOrderDetail() As tbl_order_detail
            Get
                If (_tblOrderDetail Is Nothing) Then
                    _tblOrderDetail = New tbl_order_detail(_settings)
                End If
                Return _tblOrderDetail
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the _tblGiftMessage instance with DESettings
        ''' </summary>
        ''' <value>tbl_gift_message instance</value>
        Public ReadOnly Property TblGiftMessage() As tbl_gift_message
            Get
                If (_tblGiftMessage Is Nothing) Then
                    _tblGiftMessage = New tbl_gift_message(_settings)
                End If
                Return _tblGiftMessage
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all orders in the system that are status 90 or above by a given from and to date, regardless of partner and business unit
        ''' </summary>
        ''' <param name="fromDate">YYYY-MM-DD from date string</param>
        ''' <param name="toDate">YYYY-MM-DD to date string</param>
        ''' <returns>A data table of orders</returns>
        ''' <remarks></remarks>
        Public Function GetAllCompleteOrdersByDate(ByVal fromDate As String, ByVal toDate As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT * FROM [tbl_order_header] H, [tbl_order_detail] D WITH(NOLOCK) ")
                sqlStatement.Append("WHERE H.[STATUS] >= '90' ")
                sqlStatement.Append("AND H.[TEMP_ORDER_ID] = D.[ORDER_ID] ")
                sqlStatement.Append("AND H.[LAST_ACTIVITY_DATE] >= @FromDate ")
                sqlStatement.Append("AND H.[LAST_ACTIVITY_DATE] <= @ToDate ")
                sqlStatement.Append("ORDER BY H.[CREATED_DATE]")

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromDate", fromDate))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToDate", toDate))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable
        End Function

        ''' <summary>
        ''' Retrieve retail orders based on a given order status
        ''' </summary>
        ''' <param name="partner">The given partner</param>
        ''' <param name="status">The given order status</param>
        ''' <param name="fromDate">The orders from the date</param>
        ''' <param name="toDate">The orders to the date</param>
        ''' <returns>A dataset containing records from tbl_order_header and tbl_order_detail based on the order status</returns>
        ''' <remarks></remarks>
        Public Function GetOrdersByStatus(ByVal partner As String, ByVal status As String, Optional ByVal fromDate As String = "", Optional ByVal toDate As String = "") As DataTable
            Dim outputDataTable As New DataTable
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT * FROM [tbl_order_header] H, [tbl_order_detail] D WITH(NOLOCK) ")
                sqlStatement.Append("WHERE H.[STATUS] = @Status ")
                sqlStatement.Append("AND H.[PARTNER] = @partner ")
                sqlStatement.Append("AND H.[TEMP_ORDER_ID] = D.[ORDER_ID] ")
                sqlStatement.Append("ORDER BY H.[CREATED_DATE]")

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", status))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable
        End Function

        ''' Retrieve retail orders based on a given order status
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="status">The given order status</param>
        ''' <param name="fromDate">The orders from the date</param>
        ''' <param name="toDate">The orders to the date</param>
        ''' <returns>A dataset containing records from tbl_order_header and tbl_order_detail based on the order status</returns>
        ''' <remarks></remarks>
        Public Function GetOrdersById(ByVal orderId As String, ByVal tempOrderId As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try

                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT H.*, D.*, P.ALTERNATE_SKU, P.PRODUCT_GLCODE_1, P.PRODUCT_GLCODE_2, P.PRODUCT_GLCODE_3, P.PRODUCT_GLCODE_4, P.PRODUCT_GLCODE_5 FROM [tbl_order_header] AS H ")
                sqlStatement.Append("INNER JOIN [tbl_order_detail] AS D ")
                sqlStatement.Append("ON H.[TEMP_ORDER_ID] = D.[ORDER_ID] ")
                sqlStatement.Append("LEFT OUTER JOIN [tbl_product] AS P ")
                sqlStatement.Append("ON D.[PRODUCT_CODE] = P.[PRODUCT_CODE] ")
                If String.IsNullOrEmpty(orderId) Then
                    sqlStatement.Append("WHERE H.[TEMP_ORDER_ID] = @OrderId ")
                    orderId = tempOrderId
                Else
                    sqlStatement.Append("WHERE H.[PROCESSED_ORDER_ID] = @OrderId ")
                End If

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderId", orderId))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable
        End Function

        ''' <summary>
        ''' Retrieve retail orders based on a given order status
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="status">The given order status</param>
        ''' <returns>A dataset containing records from tbl_order_header and tbl_order_detail based on the order status</returns>
        ''' <remarks></remarks>
        Public Function MarkOrdersAsComplete(ByVal businessUnit As String, ByVal partner As String, ByVal status As String, ByVal listOfOrderIds As Collection) As Integer
            Dim affectedRows As Integer = 0
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("UPDATE [tbl_order_header] SET [STATUS] = @Status ")
                sqlStatement.Append("WHERE [PARTNER] = @partner ")
                sqlStatement.Append("AND [TEMP_ORDER_ID] IN (")
                Dim addComma As Boolean = False
                For Each item As Object In listOfOrderIds
                    If addComma Then sqlStatement.Append(",")
                    sqlStatement.Append("'")
                    sqlStatement.Append(item)
                    sqlStatement.Append("'")
                    If listOfOrderIds.Count > 1 Then addComma = True
                    TblOrderStatus.Insert(businessUnit, item, Utilities.GetOrderStatus("ORDER PROCESSED"), "Order Processed with SupplyNet")
                Next
                sqlStatement.Append(")")

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", Utilities.GetOrderStatus(status)))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return affectedRows
        End Function

        Public Function GetOrderLinesByOrderID(ByVal orderId As String) As DataTable

            Dim outputDataTable As New DataTable
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT d.ADDRESS_LINE_1, d.ADDRESS_LINE_2, d.ADDRESS_LINE_3, d.ADDRESS_LINE_4, d.ADDRESS_LINE_5, ")
                sqlStatement.Append("d.BACK_OFFICE_STATUS, d.CONTACT_EMAIL, d.CONTACT_NAME, d.CONTACT_PHONE, d.COUNTRY, d.CURRENCY, d.DATE_SHIPPED, ")
                sqlStatement.Append("d.DELIVERY_GROSS, d.DELIVERY_NET, d.DELIVERY_TAX, d.GROUP_LEVEL_01, d.GROUP_LEVEL_02, d.GROUP_LEVEL_03, d.GROUP_LEVEL_04, ")
                sqlStatement.Append("d.GROUP_LEVEL_05, d.GROUP_LEVEL_06,d.GROUP_LEVEL_07, d.GROUP_LEVEL_08, d.GROUP_LEVEL_09, d.GROUP_LEVEL_10, d.LANGUAGE, ")
                sqlStatement.Append("d.LINE_NUMBER, d.LINE_PRICE_GROSS,d.LINE_PRICE_NET, d.LINE_PRICE_TAX, d.ORDER_DETAIL_ID, d.ORDER_ID, d.POSTCODE, ")
                sqlStatement.Append("d.PRODUCT_CODE, d.PRODUCT_DESCRIPTION_1,d.PRODUCT_DESCRIPTION_2, d.PRODUCT_SUPPLIER, d.PURCHASE_PRICE_GROSS, ")
                sqlStatement.Append("d.PURCHASE_PRICE_NET, d.PURCHASE_PRICE_TAX,d.QUANTITY, d.QUANTITY_SHIPPED, d.SHIPMENT_NUMBER, d.SHIPPING_CODE, ")
                sqlStatement.Append("d.TARIFF_CODE, d.TAX_AMOUNT_1, d.TAX_AMOUNT_2,d.TAX_AMOUNT_3, d.TAX_AMOUNT_4, d.TAX_AMOUNT_5, d.TAX_CODE, d.TRACKING_NO, ")
                sqlStatement.Append("d.WAREHOUSE, h.BUSINESS_UNIT, d.HEADER_ORDER_ID,d.MASTER_PRODUCT, d.PROMOTION_VALUE, d.PROMOTION_PERCENTAGE, ")
                sqlStatement.Append("d.INSTRUCTIONS, d.COST_CENTRE, d.ACCOUNT_CODE, h.DELIVERY_TYPE_DESCRIPTION, h.TOTAL_DELIVERY_GROSS ")
                sqlStatement.Append("FROM tbl_order_detail AS d WITH (NOLOCK) INNER JOIN tbl_order_header AS h WITH (NOLOCK) ON d.ORDER_ID = h.TEMP_ORDER_ID ")
                sqlStatement.Append("WHERE     (d.ORDER_ID = @OrderId) ")
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderId", orderId))

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable
        End Function

        ''' <summary>
        ''' Deletes the un paid order by temp order ID whose status is less than or equal to 40 in order header and detail
        ''' </summary>
        ''' <param name="tempOrderID">The temp order ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function DeleteUnPaidOrderByTempOrderID(ByVal tempOrderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            'delete order header
            affectedRows = Me.TblOrderHeader.DeleteByTempOrderID(tempOrderID, givenTransaction)
            'delete order detail
            affectedRows = Me.TblOrderDetail.DeleteByTempOrderID(tempOrderID, givenTransaction)

            Return affectedRows

        End Function

#End Region

    End Class
End Namespace