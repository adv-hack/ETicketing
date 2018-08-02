Imports System.Data.SqlClient
Imports System.Text
Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_order_header
    ''' </summary>
    <Serializable()> _
    Public Class tbl_order_header
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_order_header"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_order_header" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Update the order status
        ''' </summary>
        ''' <param name="businessUnit">The business unit to delete.</param>
        ''' <param name="orderId">The retail web order id</param>
        ''' <param name="newStatus">The new order status level</param>
        ''' <param name="loginId">The current user loginId</param>
        ''' <returns>No of affected rows</returns>
        Public Function UpdateOrderStatus(ByVal businessUnit As String, ByVal orderId As String, ByVal newStatus As String, ByVal loginId As String) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_order_header] SET STATUS = @NewStatus, LAST_ACTIVITY_DATE = getdate() ")
            sqlStatement.Append("WHERE BUSINESS_UNIT = @BusinessUnit ")
            sqlStatement.Append("AND LOGINID = @LoginId ")
            sqlStatement.Append("AND PROCESSED_ORDER_ID = @OrderId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderId", orderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NewStatus", newStatus))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", loginId))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        ''' <summary>
        ''' Get Order Header by TEMP_ORDER_ID
        ''' </summary>
        ''' <param name="tempOrderID">The temporary order id to retrieve</param>
        ''' <returns>Data Table</returns>
        Public Function GetOrderHeaderByTempOrderID(ByVal tempOrderId As String) As Data.DataTable

            Dim outputDataTable As New Data.DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("SELECT * FROM [tbl_order_header] ")
            sqlStatement.Append("WHERE TEMP_ORDER_ID = @TempOrderID ")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderID", tempOrderId))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Get the retail order history from order_header and order_details
        ''' </summary>
        ''' <param name="businessUnit">The current business unit</param>
        ''' <param name="businessUnitGroup">The business unit group if multiple business units are in use</param>
        ''' <param name="loginID">The current login id (may not always be customer number)</param>
        ''' <param name="partner">The partner code</param>
        ''' <param name="b2bMode">The b2b mode value</param>
        ''' <param name="showPartnerDetails">The show partner details flag</param>
        ''' <param name="orderNo">The current order number if specified</param>
        ''' <param name="fromDate">The from date if specified</param>
        ''' <param name="toDate">The to date if specified</param>
        ''' <param name="status">The order status if specified</param>
        ''' <param name="caching">Is caching in use flag</param>
        ''' <param name="cacheTimeInMins">The cache time in mins</param>
        ''' <returns>Data table of order history based on the search criteria</returns>
        ''' <remarks></remarks>
        Public Function GetRetailPurchaseHistory(ByVal businessUnit As String, ByVal businessUnitGroup As String, ByVal loginID As String, ByVal partner As String, ByVal b2bMode As Boolean,
                                                 ByVal showPartnerDetails As Boolean, Optional ByVal orderNo As String = "", Optional ByVal fromDate As String = "", Optional ByVal toDate As String = "",
                                                 Optional ByVal status As String = "", Optional ByVal caching As Boolean = True, Optional ByVal cacheTimeInMins As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetRetailPurchaseHistory")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = caching
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeInMins
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(loginID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderNo", orderNo))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", status))

                Dim sqlStatement As New StringBuilder
                With sqlStatement
                    .Append("SELECT o.*, ")
                    .Append("(SELECT COUNT(ORDER_ID) ")
                    .Append("FROM tbl_order_detail WITH (NOLOCK)  ")
                    .Append("WHERE ORDER_ID = o.TEMP_ORDER_ID) ")
                    .Append("AS LINES ")
                    .Append("FROM tbl_order_header AS o WITH (NOLOCK)  ")
                    If b2bMode Then
                        ' B2B mode
                        If showPartnerDetails Then
                            .Append("WHERE o.BUSINESS_UNIT = @BusinessUnit AND o.partner = @Partner ")
                        Else
                            .Append("WHERE o.BUSINESS_UNIT = @BusinessUnit AND o.LOGINID = @LoginID ")
                        End If
                    Else
                        ' B2C mode
                        ' If these are different, there's a business unit group defined - show all
                        If showPartnerDetails Then
                            .Append("WHERE o.BUSINESS_UNIT = @BusinessUnit AND o.partner = @Partner ")
                        Else
                            .Append("WHERE o.LOGINID = @LoginID ")
                        End If
                    End If
                    .Append("AND o.STATUS >= 50 ")
                    If Not String.IsNullOrEmpty(orderNo) Then .Append(" AND PROCESSED_ORDER_ID = @OrderNo ")
                    If Not String.IsNullOrEmpty(fromDate) Then
                        Dim d As DateTime
                        Try
                            d = CType(fromDate, DateTime)
                            .Append(" AND CREATED_DATE >= @FromDate ")
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromDate", d, SqlDbType.DateTime))
                        Catch ex As Exception
                            'Catch date error
                        End Try
                    End If
                    If Not String.IsNullOrEmpty(toDate) Then
                        Dim d As DateTime
                        Try
                            d = CType(toDate, DateTime)
                            d = d.AddDays(1) 'This is done so that orders upto midnight on the created date are included
                            .Append(" AND CREATED_DATE <= @ToDate ")
                            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToDate", d, SqlDbType.DateTime))
                        Catch ex As Exception
                            'Catch date error
                        End Try
                    End If
                    If status.Length > 0 AndAlso status <> "--" Then .Append(" AND STATUS = @Status ")
                End With
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

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
        ''' Gets the un paid order header whose status is less than or equal to 40
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="loginID">The login ID.</param><returns></returns>
        Public Function GetUnPaidOrder(ByVal businessUnit As String, ByVal loginID As String) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUnPaidOrder")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(loginID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_order_header WHERE (BUSINESS_UNIT = @BusinessUnit) AND (LOGINID = @LoginID) AND (PROCESSED_ORDER_ID IS NULL) AND (STATUS <= 40)"

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
        ''' Deletes the order header by given temp order ID.
        ''' </summary>
        ''' <param name="tempOrderID">The temp order ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function DeleteByTempOrderID(ByVal tempOrderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_order_header WHERE TEMP_ORDER_ID=@TempOrderID "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderID", tempOrderID))

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

        ''' <summary>
        ''' Update the order header table with the status, smartcard number and date
        ''' </summary>
        ''' <param name="businessUnit">The business unit relating to the order</param>
        ''' <param name="orderId">The processed order id to update</param>
        ''' <param name="newStatus">The new status to put on the order</param>
        ''' <param name="loginId">The login id being updated</param>
        ''' <param name="SmartcardNumber">The smartcard number to put on the order</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdateOrderStatusandSmartcard(ByVal businessUnit As String, ByVal orderId As String, ByVal newStatus As String, ByVal loginId As String, ByVal SmartcardNumber As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_order_header] SET STATUS = @NewStatus, LAST_ACTIVITY_DATE = getdate(), SMARTCARD_NUMBER = @SmartcardNumber ")
            sqlStatement.Append("WHERE BUSINESS_UNIT = @BusinessUnit ")
            sqlStatement.Append("AND LOGINID = @LoginId ")
            sqlStatement.Append("AND PROCESSED_ORDER_ID = @OrderId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderId", orderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NewStatus", newStatus))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", loginId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@smartcardnumber", SmartcardNumber))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Gets the paid order failed process.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function GetPaidOrderFailedProcess(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPaidOrderFailedProcess")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = " SELECT TOP 50 * FROM tbl_order_header with (NOLOCK) WHERE  CAST(STATUS AS INTEGER) >= 50 AND CAST(STATUS AS INTEGER) < 100 ORDER BY CREATED_DATE"

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
        ''' Update the order
        ''' </summary>
        ''' <param name="TempOrderId">Temporary Order ID</param>
        ''' <param name="ContactName">Contact Name</param>
        ''' <param name="AddressLine1">Address Line 1</param>
        ''' <param name="AddressLine2">Address Line 2</param>
        ''' <param name="AddressLine3">Address Line 3</param>
        ''' <param name="AddressLine4">Address Line 4</param>
        ''' <param name="AddressLine5">Address Line 5</param>
        ''' <param name="PostCode">Post Code</param>
        ''' <param name="Country">Country</param>
        ''' <param name="GiftMessage">GiftMessage</param>
        ''' <param name="DeliveryInstructions">DeliveryInstructions</param>
        ''' <param name="PurchaseOrder">PurchaseOrder</param>
        ''' <param name="addressExternalID">The external ID</param>
        ''' <param name="CountryCode">The order country code</param>
        ''' <param name="setRetailHomeDeliveryOptions">Set the retail delivery options when true otherwise blank</param>
        ''' <param name="installationOptions">The retail installation available option</param>
        ''' <param name="collectOption">The retail collection option</param>
        ''' <returns>No of affected rows</returns>
        Public Function UpdateOrder(ByVal TempOrderId As String, ByVal ContactName As String, ByVal AddressLine1 As String, ByVal AddressLine2 As String, _
                                    ByVal AddressLine3 As String, ByVal AddressLine4 As String, ByVal AddressLine5 As String, ByVal PostCode As String, _
                                    ByVal Country As String, ByVal GiftMessage As Boolean, ByVal DeliveryInstructions As String, _
                                    ByVal PurchaseOrder As String, ByVal addressExternalID As String, ByVal CountryCode As String, _
                                    ByVal setRetailHomeDeliveryOptions As Boolean, ByVal installationOptions As Boolean, ByVal collectOption As Boolean) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim specialInstructions1 As String = String.Empty
            If setRetailHomeDeliveryOptions Then
                If installationOptions Then
                    specialInstructions1 = "Y"
                Else
                    specialInstructions1 = "N"
                End If
                If collectOption Then
                    specialInstructions1 &= "Y"
                Else
                    specialInstructions1 &= "N"
                End If
            End If
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_order_header] SET CONTACT_NAME = @ContactName, ADDRESS_LINE_1 = @AddressLine1, ")
            sqlStatement.Append("ADDRESS_LINE_2 = @AddressLine2, ADDRESS_LINE_3 = @AddressLine3, ADDRESS_LINE_4 = @AddressLine4, ")
            sqlStatement.Append("ADDRESS_LINE_5 = @AddressLine5, POSTCODE = @PostCode, COUNTRY  = @Country, COUNTRY_CODE = @CountryCode, ")
            sqlStatement.Append("GIFT_MESSAGE = @GiftMessage, COMMENT = @DeliveryInstructions, PURCHASE_ORDER  = @PurchaseOrder, ")
            sqlStatement.Append("SHIPPING_CODE = @ShippingCode, ")
            sqlStatement.Append("LAST_ACTIVITY_DATE = getdate(), ")
            sqlStatement.Append("SPECIAL_INSTRUCTIONS_1 = @SpecialInstructions1 ")
            sqlStatement.Append("WHERE TEMP_ORDER_ID = @TempOrderId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderId", TempOrderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ContactName", ContactName))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AddressLine1", AddressLine1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AddressLine2", AddressLine2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AddressLine3", AddressLine3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AddressLine4", AddressLine4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AddressLine5", AddressLine5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PostCode", PostCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Country", Country))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CountryCode", CountryCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GiftMessage", GiftMessage))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryInstructions", DeliveryInstructions))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PurchaseOrder", PurchaseOrder))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ShippingCode", addressExternalID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SpecialInstructions1", specialInstructions1))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        ''' <summary>
        ''' Update the order header with the delivery cost values
        ''' </summary>
        ''' <param name="TempOrderId">The temp order id to update</param>
        ''' <param name="totalDeliveryGross">The delivery gross value</param>
        ''' <param name="totalDeliveryNet">The delivery net value</param>
        ''' <param name="totalDeliveryTax">The delivery tax value</param>
        ''' <returns>The affected rows from the update</returns>
        ''' <remarks></remarks>
        Public Function UpdateOrderDeliveryValues(ByVal TempOrderId As String, ByVal totalDeliveryGross As Decimal, ByVal totalDeliveryNet As Decimal, ByVal totalDeliveryTax As Decimal) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_order_header] SET TOTAL_DELIVERY_GROSS = @TotalDeliveryGross, TOTAL_DELIVERY_NET = @TotalDeliveryNet, TOTAL_DELIVERY_TAX = @TotalDeliveryTax, ")
            sqlStatement.Append(" LAST_ACTIVITY_DATE = getdate() ")
            sqlStatement.Append(" WHERE TEMP_ORDER_ID = @TempOrderId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderId", TempOrderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TotalDeliveryGross", totalDeliveryGross))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TotalDeliveryNet", totalDeliveryNet))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TotalDeliveryTax", totalDeliveryTax))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Update the order header with the projected delivery date
        ''' </summary>
        ''' <param name="tempOrderId">The temp order id to update</param>
        ''' <param name="projectedDeliveryDate">The projected delivery date to put on the order</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdateOrderProjectedDeliveryDate(ByVal tempOrderId As String, ByVal projectedDeliveryDate As Date) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_order_header] SET [PROJECTED_DELIVERY_DATE] = @ProjectedDeliveryDate WHERE [TEMP_ORDER_ID] = @TempOrderId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderId", tempOrderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProjectedDeliveryDate", projectedDeliveryDate, SqlDbType.DateTime))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

#End Region

    End Class
End Namespace