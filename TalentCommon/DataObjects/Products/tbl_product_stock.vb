Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_product_stock
    ''' </summary>
    <Serializable()> _
        Public Class tbl_product_stock
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product_stock"

        ''' <summary>
        ''' To decide whether to continue the multiple insert
        ''' </summary>
        Private _continueInsertMultiple As Boolean = True

        ''' <summary>
        ''' Gets or sets the product stock entity.
        ''' </summary>
        ''' <value>
        ''' The product stock entity.
        ''' </value>
        Public Property ProductStockEntityList() As Generic.List(Of DEStockProduct)

        ''' <summary>
        ''' to create only one instance of string when called by insert multiple
        ''' as it is a long string 
        ''' as well as to speed up the insert when called under transaction
        ''' </summary>
        Private ReadOnly _insertSQLStatement As String = String.Empty

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_product_stock" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
            _insertSQLStatement = "INSERT tbl_product_stock " & _
                                    "(PRODUCT, STOCK_LOCATION, QUANTITY, ALLOCATED_QUANTITY, AVAILABLE_QUANTITY, " & _
                                    "RESTOCK_CODE, WAREHOUSE) VALUES " & _
                                    "(@Product, @StockLocation, @Quantity, @AllocatedQuantity, @AvailableQuantity,  " & _
                                    "@RestockCode, @WareHouse) "
        End Sub
#End Region


#Region "Private Methods"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the stock records for a given product code
        ''' </summary>
        ''' <param name="productCode">the given product code</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetStockByProductCode(ByVal productCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetStockByProductCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT L.STOCK_LOCATION, L.STOCK_LOCATION_DESC," & _
                    "S.PRODUCT_STOCK_ID, S.PRODUCT, S.QUANTITY, S.ALLOCATED_QUANTITY, " & _
                    "S.AVAILABLE_QUANTITY, S.RESTOCK_CODE, S.WAREHOUSE " & _
                    "FROM tbl_stock_location L, tbl_product_stock S " & _
                    "WHERE S.PRODUCT = @ProductCode " & _
                    "AND S.STOCK_LOCATION = L.STOCK_LOCATION"

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
        ''' Decrements the stock for a given product code
        ''' </summary>
        ''' <param name="productCode">the given product code</param>
        ''' <param name="decrementBy">the given product code</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function DecrementStockByProductCode(ByVal productCode As String, ByVal decrementBy As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Integer

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "DecrementStockByProductCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim affectedRows As Integer = 0
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DecrementBy", decrementBy))
                talentSqlAccessDetail.CommandElements.CommandText = "UPDATE TBL_PRODUCT_STOCK SET AVAILABLE_QUANTITY = " & _
                    "AVAILABLE_QUANTITY - @DecrementBy " & _
                    "WHERE PRODUCT = @ProductCode "

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return affectedRows

        End Function

        ''' <summary>
        ''' Updates a product stock record with the given product stock id.
        ''' </summary>
        ''' <param name="productStockID">The product stock id we are updating</param>
        ''' <param name="quantity">The new quantity value</param>
        ''' <param name="availableQuantity">The new available quantity value</param>
        ''' <param name="allocatedQuantity">The new allocated quantity value</param>
        ''' <param name="restockCode">The new restock code</param>
        ''' <param name="warehouse">The new warehouse</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal productStockID As String, ByVal quantity As Decimal, ByVal availableQuantity As Decimal, _
                               ByVal allocatedQuantity As Decimal, ByVal restockCode As String, ByVal warehouse As String,
                               Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductStockID", productStockID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Quantity", quantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AvailableQuantity", availableQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AllocatedQuantity", allocatedQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RestockCode", restockCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Warehouse", warehouse))
            talentSqlAccessDetail.CommandElements.CommandText = "UPDATE tbl_product_stock SET " & _
                "[QUANTITY] = @Quantity, " & _
                "[ALLOCATED_QUANTITY] = @AllocatedQuantity, " & _
                "[AVAILABLE_QUANTITY] = @AvailableQuantity, " & _
                "[RESTOCK_CODE] = @RestockCode, " & _
                "[WAREHOUSE] = @Warehouse " & _
                "WHERE [PRODUCT_STOCK_ID] = @ProductStockID"

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

        Public Function Insert(ByVal partner As String, ByVal ProductStockEntity As DEStockProduct, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

            talentSqlAccessDetail.CommandElements.CommandText = _insertSQLStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", ProductStockEntity.ProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockLocation", ProductStockEntity.StockLocation))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Quantity", ProductStockEntity.Quantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AllocatedQuantity", ProductStockEntity.AllocatedQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AvailableQuantity", ProductStockEntity.AvailableQuantity))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RestockCode", ProductStockEntity.QuantityReStockCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@WareHouse", ProductStockEntity.QuantityWarehouse))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                ' this is to decide whether to continue the for loop in InsertMultiple method
                'if any error exit the for loop
                _continueInsertMultiple = False
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        Public Function InsertMultiple(ByVal partner As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim totalAffectedRows As Integer = 0
            If Not (ProductStockEntityList Is Nothing) Then
                If (ProductStockEntityList.Count > 0) Then
                    _continueInsertMultiple = True
                    Dim affectedRows As Integer = 0
                    For productStockEntityIndex As Integer = 0 To ProductStockEntityList.Count - 1
                        'to make sure there is no error in the Insert method
                        If (_continueInsertMultiple) Then
                            affectedRows = Insert(partner, ProductStockEntityList(productStockEntityIndex), givenTransaction)
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
        ''' <summary>
        ''' Remove product stock by product code
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveProductStockByProductCode(ByVal productCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM tbl_product_stock Where PRODUCT=@ProductCode"

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

