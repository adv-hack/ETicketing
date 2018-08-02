Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_basket_detail based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_basket_detail
        Inherits DBObjectBase
#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_detail"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_detail" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            MyBase.New()
            _settings = settings
        End Sub
#End Region

#Region "Properties"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets the fee records based on the given business unit, partner and language code.
        ''' </summary>
        ''' <param name="feeCode">The fee to add.</param>
        ''' <param name="basketheaderID">The basket Header ID to insert under</param>
        ''' <param name="loginID">The current login id</param>
        ''' <param name="price">The total fee ammount</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        Public Function InsertFee(ByVal feeCode As String, ByVal basketheaderID As String, ByVal loginID As String, ByVal price As Decimal, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0

            affectedRows = InsertFee(feeCode, "", basketheaderID, loginID, price, givenTransaction)

            'Return results
            Return affectedRows
        End Function

        Public Function InsertFee(ByVal feeCode As String, ByVal feeCodeDescription As String, ByVal basketheaderID As String, ByVal loginID As String, ByVal price As Decimal, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO [tbl_basket_detail] (" & _
                                    "[BASKET_HEADER_ID], " & _
                                    "[PRODUCT], " & _
                                    "[QUANTITY], " & _
                                    "[GROSS_PRICE], " & _
                                    "[MODULE], " & _
                                    "[LOGINID], " & _
                                    "[SEAT], " & _
                                    "[RESERVED_SEAT], " & _
                                    "[PRICE_BAND], " & _
                                    "[PRODUCT_DESCRIPTION1], " & _
                                    "[PRODUCT_DESCRIPTION2], " & _
                                    "[PRODUCT_DESCRIPTION3], " & _
                                    "[PRODUCT_DESCRIPTION4], " & _
                                    "[PRODUCT_DESCRIPTION5], " & _
                                    "[PRODUCT_DESCRIPTION6], " & _
                                    "[PRODUCT_DESCRIPTION7], " & _
                                    "[GROUP_LEVEL_01], " & _
                                    "[GROUP_LEVEL_02], " & _
                                    "[GROUP_LEVEL_03], " & _
                                    "[GROUP_LEVEL_04], " & _
                                    "[GROUP_LEVEL_05], " & _
                                    "[GROUP_LEVEL_06], " & _
                                    "[GROUP_LEVEL_07], " & _
                                    "[GROUP_LEVEL_08], " & _
                                    "[GROUP_LEVEL_09], " & _
                                    "[GROUP_LEVEL_10], " & _
                                    "[STOCK_ERROR], " & _
                                    "[IS_FREE], " & _
                                    "[STOCK_ERROR_CODE], " & _
                                    "[STOCK_LIMIT], " & _
                                    "[STOCK_REQUESTED], " & _
                                    "[PRICE_CODE], " & _
                                    "[PRODUCT_TYPE], " & _
                                    "[SIZE], " & _
                                    "[QUANTITY_AVAILABLE], " & _
                                    "[MASTER_PRODUCT], " & _
                                    "[STOCK_ERROR_DESCRIPTION], " & _
                                    "[PRODUCT_SUB_TYPE], " & _
                                    "[ALTERNATE_SKU], " & _
                                    "[NET_PRICE], " & _
                                    "[TAX_PRICE], " & _
                                    "[RESTRICTION_CODE], " & _
                                    "[FULFIL_OPT_POST], " & _
                                    "[FULFIL_OPT_COLL], " & _
                                    "[FULFIL_OPT_PAH], " & _
                                    "[FULFIL_OPT_UPL], " & _
                                    "[FULFIL_OPT_PRINT], " & _
                                    "[CURR_FULFIL_SLCTN], " & _
                                    "[PACKAGE_ID], " & _
                                    "[TRAVEL_PRODUCT_SELECTED], " & _
                                    "[CAT_QUANTITY], " & _
                                    "[CAT_FULFILMENT]) " & _
                                    "VALUES (" & _
                                    "'" & basketheaderID & "', " & _
                                    "'" & feeCode & "', " & _
                                    "'1.000', " & _
                                    "'" & price & "', " & _
                                    "'Ticketing', " & _
                                    "'" & loginID & "', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'" & feeCodeDescription & "', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "0, " & _
                                    "0, " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'0.000', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'" & price & "', " & _
                                    "'0.000', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'', " & _
                                    "'0', " & _
                                    "'0', " & _
                                    "'0', " & _
                                    "'')"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

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

        Public Function InsertFee(ByVal basketheaderID As String, ByVal loginID As String, ByVal basketFeesEntity As DEBasketFees, ByVal isExternal As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            affectedRows = InsertFee(basketheaderID, loginID, basketFeesEntity, isExternal, True, givenTransaction)
            Return affectedRows
        End Function

        Public Function InsertFee(ByVal basketheaderID As String, ByVal loginID As String, ByVal basketFeesEntity As DEBasketFees, ByVal isExternal As Boolean, ByVal isIncluded As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlStatement As New StringBuilder

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            sqlStatement.Append("INSERT INTO [tbl_basket_detail] ([BASKET_HEADER_ID],[PRODUCT],[QUANTITY],[GROSS_PRICE],[MODULE],[LOGINID],[SEAT],[RESERVED_SEAT],")
            sqlStatement.Append("[PRICE_BAND],[PRODUCT_DESCRIPTION1],[PRODUCT_DESCRIPTION2],[PRODUCT_DESCRIPTION3],[PRODUCT_DESCRIPTION4],[PRODUCT_DESCRIPTION5],[PRODUCT_DESCRIPTION6],")
            sqlStatement.Append("[PRODUCT_DESCRIPTION7],[GROUP_LEVEL_01],[GROUP_LEVEL_02],[GROUP_LEVEL_03],[GROUP_LEVEL_04],[GROUP_LEVEL_05],[GROUP_LEVEL_06],[GROUP_LEVEL_07],[GROUP_LEVEL_08],")
            sqlStatement.Append("[GROUP_LEVEL_09],[GROUP_LEVEL_10],[STOCK_ERROR],[IS_FREE],[STOCK_ERROR_CODE],[STOCK_LIMIT],[STOCK_REQUESTED],[PRICE_CODE],[PRODUCT_TYPE],[SIZE],[QUANTITY_AVAILABLE],")
            sqlStatement.Append("[MASTER_PRODUCT],[STOCK_ERROR_DESCRIPTION],[PRODUCT_SUB_TYPE],[ALTERNATE_SKU],[NET_PRICE],[TAX_PRICE],[RESTRICTION_CODE],[FULFIL_OPT_POST],[FULFIL_OPT_COLL],[FULFIL_OPT_PAH],")
            sqlStatement.Append("[FULFIL_OPT_UPL],[FULFIL_OPT_PRINT],[FULFIL_OPT_REGPOST],[CURR_FULFIL_SLCTN],[PACKAGE_ID],[TRAVEL_PRODUCT_SELECTED],[IS_SYSTEM_FEE],[IS_EXTERNAL],[IS_INCLUDED],[IS_TRANSACTIONAL],")
            sqlStatement.Append("[FEE_CATEGORY],[CAT_QUANTITY],[CAT_FULFILMENT]")
            sqlStatement.Append(") VALUES (")
            sqlStatement.Append("'").Append(basketheaderID).Append("', ")
            sqlStatement.Append("'").Append(basketFeesEntity.FeeCode).Append("', '1.000', ")
            sqlStatement.Append("'").Append(basketFeesEntity.FeeValue).Append("', 'Ticketing', '").Append(loginID).Append("', '', '', '', '").Append(basketFeesEntity.FeeDescription).Append("', ")
            sqlStatement.Append("'', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', 0, 0, '', '', '', '', '', '', '0.000', '', '', '', '', '").Append(basketFeesEntity.FeeValue).Append("', ")
            sqlStatement.Append("'0.000', '', '', '', '', '', '', '', '', '0', 0, ")
            sqlStatement.Append(Utilities.ConvertBoolToInt(basketFeesEntity.IsSystemFee)).Append(", ")
            sqlStatement.Append(Utilities.ConvertBoolToInt(isExternal)).Append(", ")
            sqlStatement.Append(Utilities.ConvertBoolToInt(isIncluded)).Append(", ")
            sqlStatement.Append(Utilities.ConvertBoolToInt(basketFeesEntity.IsTransactional)).Append(", ")
            sqlStatement.Append("'").Append(basketFeesEntity.FeeCategory).Append("', ")
            sqlStatement.Append("0, '')")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

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

        Public Function UpdateFee(ByVal basketHeaderID As String, ByVal loginID As String, ByVal basketFeesEntity As DEBasketFees, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_basket_detail SET GROSS_PRICE = @Price, NET_PRICE = @Price, PRODUCT_DESCRIPTION1 = @FeeDescription, IS_TRANSACTIONAL = @IsTransactional WHERE BASKET_HEADER_ID = @BasketHeaderId AND PRODUCT = @FeeCode AND FEE_CATEGORY = @FeeCategory AND IS_EXTERNAL = 0"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", basketFeesEntity.FeeCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", basketFeesEntity.FeeCategory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Price", basketFeesEntity.FeeValue, SqlDbType.Decimal))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeDescription", basketFeesEntity.FeeDescription))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsTransactional", basketFeesEntity.IsTransactional, SqlDbType.Bit))

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

            'Return affected rows count
            Return affectedRows
        End Function

        Public Function UpdateFeeValue(ByVal basketHeaderID As String, ByVal basketFeesEntity As DEBasketFees, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_basket_detail SET GROSS_PRICE = @Price, NET_PRICE = @Price WHERE BASKET_HEADER_ID = @BasketHeaderId AND PRODUCT = @FeeCode AND FEE_CATEGORY = @FeeCategory AND IS_EXTERNAL = 0"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", basketFeesEntity.FeeCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", basketFeesEntity.FeeCategory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Price", basketFeesEntity.FeeValue, SqlDbType.Decimal))

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

        Public Function UpdateOrInsertFee(ByVal basketHeaderID As String, ByVal loginID As String, ByVal basketFeesEntity As DEBasketFees, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            affectedRows = UpdateFee(basketHeaderID, loginID, basketFeesEntity, givenTransaction)
            If affectedRows <= 0 Then
                affectedRows = InsertFee(basketHeaderID, loginID, basketFeesEntity, False, givenTransaction)
            End If
            Return affectedRows
        End Function

        ''' <summary>
        ''' Deletes the specified fee.
        ''' </summary>
        ''' <param name="feeToDelete">The fee to delete.</param>
        ''' <param name="basketHeaderID">The basket header id</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteFee(ByVal feeToDelete As String, ByVal basketHeaderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE PRODUCT=@feeToDelete AND BASKET_HEADER_ID=@basketHeaderID "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeToDelete", feeToDelete))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))

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

        Public Function DeleteFee(ByVal feeToDelete As String, ByVal basketHeaderID As String, ByVal feeCategory As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE IS_EXTERNAL = 0 AND PRODUCT=@feeToDelete AND BASKET_HEADER_ID=@basketHeaderID AND FEE_CATEGORY = @FeeCategory"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeToDelete", feeToDelete))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategory))

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

        Public Function DeleteBasketDetail(ByVal basketHeaderID As Long, ByVal deletingModule As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@BasketHeaderID "
            If deletingModule = GlobalConstants.BASKETMODULETICKETING AndAlso _settings.IsAgent Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID AND MODULE = 'Ticketing' AND ((IS_SYSTEM_FEE = 1) OR (FEE_CATEGORY IS NULL OR FEE_CATEGORY = '')) AND IS_EXTERNAL = 1"
            ElseIf deletingModule = GlobalConstants.BASKETMODULETICKETING Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID AND MODULE = 'Ticketing'"
            ElseIf deletingModule = GlobalConstants.BASKETMODULEMERCHANDISE Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID AND MODULE = ''"
            ElseIf deletingModule = GlobalConstants.BASKETMODULEALL Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID"
            End If
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))

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

        Public Function ClearBasketDetailByModule(ByVal basketHeaderID As Long, ByVal deletingModule As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@BasketHeaderID "
            If deletingModule = GlobalConstants.BASKETMODULETICKETING Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID AND MODULE = 'Ticketing'"
            ElseIf deletingModule = GlobalConstants.BASKETMODULEMERCHANDISE Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID AND MODULE = ''"
            ElseIf deletingModule = GlobalConstants.BASKETMODULEALL Then
                sqlStatement = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID=@basketHeaderID"
            End If
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))

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

        Public Function GetDetailByBasketHeaderID(ByVal basketHeaderID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDetailByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail] WHERE BASKET_HEADER_ID = @BasketHeaderID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))

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

        Public Function GetBasketHeaderIDByBasketDetail(ByVal basketHeaderID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDetailByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail] WHERE BASKET_HEADER_ID = @BasketHeaderID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))

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

        Public Function GetDetailByBasketHeaderIDModule(ByVal basketHeaderID As String, ByVal moduleName As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDetailByBasketHeaderIDModule")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail] WHERE BASKET_HEADER_ID = @BasketHeaderID AND MODULE = @Module"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Module", moduleName))
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

        Public Function GetDetailByHeaderIDFeeCode(ByVal basketHeaderID As String, ByVal feeCategory As String, ByVal feeCode As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDetailByHeaderIDFeeCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_detail] " & _
                    "WHERE BASKET_HEADER_ID = @BasketHeaderID AND FEE_CATEGORY = @FeeCategory AND PRODUCT = @Product"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategory))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", feeCode))

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

        Public Function UpdateSystemFeeIncludeStatusLatest(ByVal basketHeaderID As String, ByVal product As String, ByVal feeCategory As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Const sqlStatementUpdateFee As String = " IF EXISTS(SELECT BASKET_DETAIL_ID FROM tbl_basket_detail " & _
                    " WHERE BASKET_HEADER_ID = @BasketHeaderId AND PRODUCT = @Product AND FEE_CATEGORY =@FeeCategory AND IS_EXTERNAL = 0 AND IS_SYSTEM_FEE = 1 AND IS_INCLUDED IS NOT NULL) " & _
                        " BEGIN " & _
                            " UPDATE tbl_basket_detail SET [IS_INCLUDED] = [IS_INCLUDED] ^ 1 " & _
                            " WHERE BASKET_HEADER_ID = @BasketHeaderId AND PRODUCT = @Product AND FEE_CATEGORY =@FeeCategory AND IS_EXTERNAL = 0 AND IS_SYSTEM_FEE = 1 AND IS_INCLUDED IS NOT NULL " & _
                        " END "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatementUpdateFee
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", product))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategory))

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

        Public Function UpdateOrDeleteFeeForIncludeStatus(ByVal basketHeaderID As String, ByVal product As String, ByVal feeCategory As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Const sqlStatementUpdateFee As String = " IF EXISTS(SELECT BASKET_DETAIL_ID FROM tbl_basket_detail " & _
                    " WHERE BASKET_HEADER_ID = @BasketHeaderId AND PRODUCT = @Product AND FEE_CATEGORY =@FeeCategory AND IS_SYSTEM_FEE = 1 AND IS_INCLUDED IS NOT NULL) " & _
                        " BEGIN " & _
                            " UPDATE tbl_basket_detail SET [IS_INCLUDED] = [IS_INCLUDED] ^ 1 " & _
                            " WHERE BASKET_HEADER_ID = @BasketHeaderId AND PRODUCT = @Product AND FEE_CATEGORY =@FeeCategory AND IS_SYSTEM_FEE = 1 AND IS_INCLUDED IS NOT NULL " & _
                        " END " & _
                    " ELSE " & _
                        " BEGIN " & _
                            " DELETE tbl_basket_detail " & _
                            " WHERE BASKET_HEADER_ID = @BasketHeaderId AND ((PRODUCT = @Product AND FEE_CATEGORY = @FeeCategory) OR (FEE_CATEGORY = @FeeCategory)) AND IS_EXTERNAL = 0 AND IS_SYSTEM_FEE = 0 AND IS_INCLUDED IS NOT NULL " & _
                        " END "

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatementUpdateFee
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", product))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategory))

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

        Public Function DeleteAllPayTypeFees(ByVal basketHeaderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim payTypeCategories As String = "'" & GlobalConstants.FEECATEGORY_BOOKING & "'"
            payTypeCategories = payTypeCategories & ",'" & GlobalConstants.FEECATEGORY_DIRECTDEBIT & "'"
            payTypeCategories = payTypeCategories & ",'" & GlobalConstants.FEECATEGORY_FINANCE & "'"
            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE IS_EXTERNAL = 0 AND FEE_CATEGORY IN (" & payTypeCategories & ") AND BASKET_HEADER_ID=@basketHeaderID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))

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

        Public Function DeleteAllSystemFees(ByVal basketHeaderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            
            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE IS_SYSTEM_FEE = 1 AND BASKET_HEADER_ID=@basketHeaderID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))

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

        Public Function DeleteFeesByFeeCategory(ByVal basketHeaderID As String, ByVal feeCategory As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

            Dim sqlStatement As String = "DELETE [tbl_basket_detail] WHERE BASKET_HEADER_ID = @basketHeaderID AND FEE_CATEGORY = @feeCategory"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeCategory", feeCategory))
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

        Public Function TryGetFeeValue(ByRef feeValue As Decimal, ByVal basketHeaderID As String, ByVal feeCode As String, ByVal feeCategory As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Boolean
            Dim isFeeCategoryExists As Boolean = False

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As String = "SELECT * FROM [tbl_basket_detail] WHERE PRODUCT = @feeCode AND FEE_CATEGORY = @feeCategory AND BASKET_HEADER_ID = @basketHeaderID "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeCode", feeCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeCategory", feeCategory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not err.HasError) AndAlso
                talentSqlAccessDetail.ResultDataSet IsNot Nothing AndAlso
                talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 AndAlso
                talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                feeValue = Utilities.CheckForDBNull_Decimal(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)("GROSS_PRICE"))
                isFeeCategoryExists = True
            End If
            talentSqlAccessDetail = Nothing

            Return isFeeCategoryExists
        End Function

        Public Function UpdateRetailBasketItems(ByVal basketHeaderID As String, ByVal webPrices As TalentWebPricing, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_basket_detail SET GROSS_PRICE = @GrossPrice, NET_PRICE = @NetPrice, TAX_PRICE = @TaxPrice " &
                " WHERE IS_FREE = 0 AND BASKET_HEADER_ID = @BasketHeaderId AND LTRIM(RTRIM(UPPER(PRODUCT))) = @ProductCode "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            If webPrices.RetrievedPrices.Count > 0 Then
                For Each productCode As String In webPrices.RetrievedPrices.Keys
                    talentSqlAccessDetail.CommandElements.CommandParameter.Clear()
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode.ToUpper.Trim))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GrossPrice", webPrices.RetrievedPrices(productCode).Purchase_Price_Gross, SqlDbType.Decimal))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@NetPrice", webPrices.RetrievedPrices(productCode).Purchase_Price_Net, SqlDbType.Decimal))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TaxPrice", webPrices.RetrievedPrices(productCode).Purchase_Price_Tax, SqlDbType.Decimal))
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail.SQLAccess()
                    Else
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                    End If
                    If err.HasError Then
                        affectedRows = 0
                        Exit For
                    Else
                        affectedRows += 1
                    End If
                Next
            End If

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return Affected Rows Count
            Return affectedRows

        End Function

        Public Function GetNonTicketingDetailByBasketHeaderID(ByVal basketHeaderID As String) As DataTable
            Dim isFeeCategoryExists As Boolean = False
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As String = "SELECT * FROM  tbl_basket_detail WITH (NOLOCK)" & _
            "WHERE  (BASKET_HEADER_ID = @basketHeaderID) AND (MODULE <> 'Ticketing') ORDER BY XML_CONFIG"

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))

            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
            End If

            Return outputDataTable
        End Function

        Public Function IsSystemFeeInBasketAlready(ByVal basketHeaderID As String, ByVal loginID As String, ByVal basketFeesEntity As DEBasketFees, ByVal isExternal As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Boolean
            Dim feeExists As Boolean = False
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("SELECT * FROM [tbl_basket_detail] WHERE ")
            sqlStatement.Append("LOGINID = @loginId AND ")
            sqlStatement.Append("PRODUCT = @feeCode AND ")
            sqlStatement.Append("PRODUCT_DESCRIPTION1 = @feeDescription AND ")
            sqlStatement.Append("GROSS_PRICE = @feeValue AND ")
            sqlStatement.Append("IS_SYSTEM_FEE = @isSystemFee AND ")
            sqlStatement.Append("IS_EXTERNAL = @isExternal AND ")
            sqlStatement.Append("IS_INCLUDED = 1 AND ")
            sqlStatement.Append("IS_TRANSACTIONAL = @isTransactional AND ")
            sqlStatement.Append("MODULE = 'Ticketing' AND ")
            sqlStatement.Append("FEE_CATEGORY = @feeCategory AND ")
            sqlStatement.Append("BASKET_HEADER_ID = @basketHeaderID ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@loginId", loginID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeCode", basketFeesEntity.FeeCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeDescription", basketFeesEntity.FeeDescription))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeValue", basketFeesEntity.FeeValue))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@isSystemFee", Utilities.ConvertBoolToInt(basketFeesEntity.IsSystemFee)))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@isExternal", Utilities.ConvertBoolToInt(isExternal)))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@isTransactional", Utilities.ConvertBoolToInt(basketFeesEntity.IsTransactional)))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@feeCategory", basketFeesEntity.FeeCategory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderID", basketHeaderID))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not err.HasError) AndAlso talentSqlAccessDetail.ResultDataSet IsNot Nothing AndAlso talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 AndAlso talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                feeExists = True
            End If
            talentSqlAccessDetail = Nothing

            Return feeExists
        End Function

#End Region

    End Class
End Namespace
