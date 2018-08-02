Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_payment_type_bu based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_payment_type_bu
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_payment_type_bu"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_payment_type_bu" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Private Methods"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the business unit records from the table tbl_payment_type_bu
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAll")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAYMENT_TYPE_BU"

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
        ''' Gets the business unit records based on the given business unit.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBU(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBU")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_PAYMENT_TYPE_BU Where BUSINESS_UNIT=@BusinessUnitName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))

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
        ''' Gets the business unit records based on the given business unit and the type of basket (retail/ticketing/mixed) and Agent mode.
        ''' Selects from tbl_payment_type_bu and tbl_payment_type
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="isMixedType">The boolean value to specify mixed type</param>
        ''' <param name="isRetailType">The boolean value to specify retail type</param>
        ''' <param name="isTicketingType">The boolean value to specify ticketing type</param>
        ''' <param name="isAgent">The boolean value to specify if the current user is in agent mode</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBasketTypeAndBU(ByVal businessUnit As String, ByVal isTicketingType As Boolean, ByVal isRetailType As Boolean, ByVal isMixedType As Boolean, _
                                             ByVal isAgent As Boolean, ByVal isRefundMode As Boolean, ByVal isGenericSales As Boolean,
                                             Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30,
                                                Optional ByVal isOtherType As Boolean = False) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBasketTypeAndBU")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim commandText As System.Text.StringBuilder = New System.Text.StringBuilder

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(isTicketingType.ToString) & _
                                                                    ToUpper(isRetailType.ToString) & ToUpper(isMixedType.ToString) & ToUpper(isAgent.ToString) & _
                                                                    ToUpper(isRefundMode.ToString) & ToUpper(isGenericSales.ToString) & ToUpper(isOtherType.ToString)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsOtherType", isOtherType, SqlDbType.Bit))
                commandText.Append(" SELECT PTBU.QUALIFIER, PTBU.PARTNER, PTBU.PAYMENT_TYPE_CODE, PTBU.DEFAULT_PAYMENT_TYPE, ")
                commandText.Append(" PT.PAYMENT_TYPE_DESCRIPTION, PT.SEQUENCE, PT.NAVIGATE_URL, PT.CALL_CREDIT_LIMIT_CHECK, PPS_TYPE, IS_GIFT_CARD ")
                commandText.Append(" FROM tbl_payment_type_bu PTBU, tbl_payment_type PT ")
                commandText.Append(" WHERE PTBU.BUSINESS_UNIT = @BusinessUnitName ")
                commandText.Append(" AND PT.IS_OTHER_TYPE = @IsOtherType ")
                commandText.Append(" AND PTBU.PAYMENT_TYPE_CODE = PT.PAYMENT_TYPE_CODE")
                If isAgent Then
                    If isRefundMode AndAlso isGenericSales Then
                        commandText.Append(" AND PTBU.GENERIC_SALES_REFUND = 1")
                    ElseIf isRefundMode Then
                        commandText.Append(" AND PTBU.AGENT_TICKETING_TYPE_REFUND = 1")
                    ElseIf isGenericSales Then
                        commandText.Append(" AND PTBU.GENERIC_SALES = 1")
                    Else
                        If isMixedType Then
                            commandText.Append(" AND PTBU.AGENT_RETAIL_TYPE = 1 AND PTBU.AGENT_TICKETING_TYPE = 1")
                        ElseIf isRetailType Then
                            commandText.Append(" AND PTBU.AGENT_RETAIL_TYPE = 1")
                        ElseIf isTicketingType Then
                            commandText.Append(" AND PTBU.AGENT_TICKETING_TYPE = 1")
                        End If
                    End If
                Else
                    If isRefundMode Then
                        commandText.Append(" AND PTBU.TICKETING_TYPE_REFUND = 1")
                    Else
                        If isMixedType Then
                            commandText.Append(" AND PTBU.RETAIL_TYPE = 1 AND PTBU.TICKETING_TYPE = 1")
                        ElseIf isRetailType Then
                            commandText.Append(" AND PTBU.RETAIL_TYPE = 1")
                        ElseIf isTicketingType Then
                            commandText.Append(" AND PTBU.TICKETING_TYPE = 1")
                        End If
                    End If

                End If
                talentSqlAccessDetail.CommandElements.CommandText = commandText.Append(" ORDER BY PT.SEQUENCE").ToString()

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
        ''' Deletes the specified business unit.
        ''' </summary>
        ''' <param name="businessUnitToDelete">The business unit to delete.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Delete(ByVal businessUnitToDelete As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_PAYMENT_TYPE_BU " & _
                "WHERE BUSINESS_UNIT=@BusinessUnitToDelete "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitToDelete", businessUnitToDelete))

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
        ''' Copies the records from one business unit to another business unit inside the table tbl_payment_type_bu
        ''' irrespective of whether destination business exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromBusinessUnit">From business unit.</param>
        ''' <param name="toBusinessUnit">To business unit.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess


            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO TBL_PAYMENT_TYPE_BU (BUSINESS_UNIT, QUALIFIER, PARTNER, " & _
                        "PAYMENT_TYPE_CODE, DEFAULT_PAYMENT_TYPE) " & _
                        "SELECT @ToBusinessUnit As BUSINESS_UNIT, QUALIFIER, PARTNER, " & _
                        "PAYMENT_TYPE_CODE, DEFAULT_PAYMENT_TYPE FROM TBL_PAYMENT_TYPE_BU " & _
                        "WHERE BUSINESS_UNIT = @FromBusinessUnit"

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromBusinessUnit", fromBusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToBusinessUnit", toBusinessUnit))

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

            'Return the results 
            Return affectedRows

        End Function

#End Region

    End Class
End Namespace