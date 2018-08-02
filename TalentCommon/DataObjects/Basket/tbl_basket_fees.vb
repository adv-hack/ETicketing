Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_basket_fees based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_basket_fees
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_fees"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_fees" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"



#End Region


#Region "Public Methods"

        Public Function GetExcludedFeesByBasketHeaderID(ByVal basketHeaderID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetExcludedFeesByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_fees] WHERE IS_EXCLUDED = 1 AND BASKET_HEADER_ID = @BasketHeaderID"
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

        Public Function GetFeesByBasketHeaderID(ByVal basketHeaderID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetFeesByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_fees] WHERE IS_EXCLUDED = 0 AND BASKET_HEADER_ID = @BasketHeaderID"
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

        Public Function GetFeeByHeaderIDCodeCategory(ByVal basketHeaderID As String, ByVal feeCategeory As String, ByVal feeCode As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetFeeByHeaderIDCodeCategory")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_fees] WHERE IS_EXCLUDED = 0 AND BASKET_HEADER_ID = @BasketHeaderID AND FEE_CATEGORY = @FeeCategory AND FEE_CODE = @FeeCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategeory))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", feeCode))

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

        Public Function GetFeeByHeaderIDCodeCategory(ByVal basketHeaderID As String, ByVal feeCategeory As String, ByVal feeCode As String, ByVal cardTypeCode As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetFeeByHeaderIDCodeCategory")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_fees] WHERE IS_EXCLUDED = 0 AND BASKET_HEADER_ID = @BasketHeaderID AND FEE_CATEGORY = @FeeCategory AND FEE_CODE = @FeeCode AND CARD_TYPE_CODE = @CardTypeCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategeory))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", feeCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardTypeCode", cardTypeCode))

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

        Public Function InsertExcludedFees(ByVal basketHeaderID As String, ByVal feesExcludedString As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim sqlStatement As String = String.Empty
            If TryGetInsertExcFeesSqlStatement(basketHeaderID, feesExcludedString, sqlStatement) Then
                Dim talentSqlAccessDetail As New TalentDataAccess
                Dim err As New ErrorObj
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
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
            End If
            'Return results
            Return affectedRows
        End Function

        Public Function Insert(ByVal basketHeaderID As String, ByVal basketFeesEntity As DEBasketFees, ByVal isExcluded As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO tbl_basket_fees " & _
                " (BASKET_HEADER_ID, FEE_CODE, FEE_DESCRIPTION, FEE_CATEGORY, FEE_APPLY_TYPE, CARD_TYPE_CODE, FEE_VALUE, IS_SYSTEM_FEE, IS_TRANSACTIONAL, IS_NEGATIVE_FEE, IS_EXCLUDED) " & _
                " VALUES (@BasketHeaderId, @FeeCode, @FeeDescription, @FeeCategory, @FeeApplyType, @CardTypeCode, @FeeValue, @IsSystemFee, @IsTransactional, @IsNegativeFee, @IsExcluded) "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", basketFeesEntity.FeeCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeDescription", basketFeesEntity.FeeDescription))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", basketFeesEntity.FeeCategory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeApplyType", basketFeesEntity.FeeApplyType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardTypeCode", basketFeesEntity.CardType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeValue", basketFeesEntity.FeeValue))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsSystemFee", basketFeesEntity.IsSystemFee, SqlDbType.Bit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsTransactional", basketFeesEntity.IsTransactional, SqlDbType.Bit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsNegativeFee", basketFeesEntity.IsNegativeFee, SqlDbType.Bit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsExcluded", isExcluded, SqlDbType.Bit))
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

        Public Function Insert(ByVal basketHeaderID As String, ByVal basketFeesEntityList As List(Of DEBasketFees), ByVal isExcluded As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0

            If basketFeesEntityList IsNot Nothing AndAlso basketFeesEntityList.Count > 0 Then

                For itemIndex As Integer = 0 To basketFeesEntityList.Count - 1
                    affectedRows = Insert(basketHeaderID, basketFeesEntityList(itemIndex), isExcluded, givenTransaction)
                    If affectedRows <= 0 Then
                        affectedRows = -1
                        Exit For
                    End If
                Next

            End If

            'Return results
            Return affectedRows

        End Function

        Public Function Delete(ByVal basketHeaderID As String, ByVal isExcluded As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_basket_fees " & _
                " WHERE BASKET_HEADER_ID = @BasketHeaderId AND IS_EXCLUDED = @IsExcluded"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsExcluded", isExcluded, SqlDbType.Bit))
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

        Public Function DeleteAll(ByVal basketHeaderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_basket_fees " & _
                " WHERE BASKET_HEADER_ID = @BasketHeaderId "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
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

        Public Function DeleteByHeaderIDCodeCategory(ByVal basketHeaderID As String, ByVal feeCategory As String, ByVal feeCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_basket_fees " & _
                " WHERE BASKET_HEADER_ID = @BasketHeaderId AND FEE_CATEGORY = @FeeCategory AND FEE_CODE = @FeeCode"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCategory", feeCategory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", feeCode))
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

        Public Function DeleteExceptBookingFee(ByVal basketHeaderID As String, ByVal isExcluded As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_basket_fees " & _
                " WHERE BASKET_HEADER_ID = @BasketHeaderId AND IS_EXCLUDED = @IsExcluded AND FEE_CATEGORY <> '" & GlobalConstants.FEECATEGORY_BOOKING & "'"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsExcluded", isExcluded, SqlDbType.Bit))
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

        Public Function DeleteBookingFee(ByVal basketHeaderID As String, ByVal isExcluded As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_basket_fees " & _
                " WHERE BASKET_HEADER_ID = @BasketHeaderId AND IS_EXCLUDED = @IsExcluded AND FEE_CATEGORY = '" & GlobalConstants.FEECATEGORY_BOOKING & "'"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsExcluded", isExcluded, SqlDbType.Bit))
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

        'Public Function DeleteAndInsertByBasketHeaderId(ByVal basketHeaderID As String, ByVal basketFeesEntityList As List(Of DEBasketFees), Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
        '    Dim affectedRows As Integer = 0

        '    If basketFeesEntityList IsNot Nothing AndAlso basketFeesEntityList.Count > 0 Then
        '        affectedRows = Delete(basketHeaderID, givenTransaction)
        '    End If
        '    If affectedRows >= 0 Then
        '        affectedRows = Insert(basketHeaderID, basketFeesEntityList, givenTransaction)
        '    End If
        '    'Return results
        '    Return affectedRows
        'End Function

#End Region

#Region "Private Methods"
        Private Function TryGetInsertExcFeesSqlStatement(ByVal basketHeaderID As String, ByVal feesExcludedString As String, ByRef sqlStatement As String) As Boolean
            Dim isExists As Boolean = False
            Dim sbInsertStatement As New StringBuilder
            sbInsertStatement.Append(" INSERT INTO tbl_basket_fees (BASKET_HEADER_ID, FEE_CODE, FEE_DESCRIPTION, FEE_CATEGORY, FEE_APPLY_TYPE, CARD_TYPE_CODE, FEE_VALUE, IS_SYSTEM_FEE, IS_TRANSACTIONAL, IS_NEGATIVE_FEE, IS_EXCLUDED) VALUES ")
            While feesExcludedString.Length > 0
                Dim feeCodeLength As Integer = 6
                If feesExcludedString.Length < 6 Then
                    feeCodeLength = feesExcludedString.Length
                End If
                If isExists Then
                    sbInsertStatement.Append(", (")
                    sbInsertStatement.Append(basketHeaderID)
                    sbInsertStatement.Append(",'" & feesExcludedString.Substring(0, feeCodeLength).Trim & "'")
                    'FEE_DESCRIPTION, FEE_CATEGORY, FEE_APPLY_TYPE, CARD_TYPE_CODE, FEE_VALUE
                    sbInsertStatement.Append(",'','','','','0'")
                    'IS_SYSTEM_FEE, IS_TRANSACTIONAL, IS_NEGATIVE_FEE, IS_EXCLUDED
                    sbInsertStatement.Append(",0,0,0,1")
                    sbInsertStatement.Append(")")
                Else
                    'BASKET_HEADER_ID, FEE_CODE
                    sbInsertStatement.Append(" (")
                    sbInsertStatement.Append(basketHeaderID)
                    sbInsertStatement.Append(",'" & feesExcludedString.Substring(0, feeCodeLength).Trim & "'")
                    'FEE_DESCRIPTION, FEE_CATEGORY, FEE_APPLY_TYPE, CARD_TYPE_CODE, FEE_VALUE
                    sbInsertStatement.Append(",'','','','','0'")
                    'IS_SYSTEM_FEE, IS_TRANSACTIONAL, IS_NEGATIVE_FEE, IS_EXCLUDED
                    sbInsertStatement.Append(",0,0,0,1")
                    sbInsertStatement.Append(")")
                    isExists = True
                End If
                feesExcludedString = feesExcludedString.Substring(feeCodeLength)
            End While
            If isExists Then sqlStatement = sbInsertStatement.ToString
            Return isExists
        End Function
#End Region

    End Class
End Namespace
