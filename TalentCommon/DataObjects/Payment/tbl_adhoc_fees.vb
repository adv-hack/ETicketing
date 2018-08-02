Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_adhoc_fees based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_adhoc_fees
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_adhoc_fees"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_adhoc_fees" /> class.
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
        ''' Gets the fee records based on the given business unit, partner and language code.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partner">The partner</param>
        ''' <param name="languageCode">The langauge code</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBUPartnerLang(ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, _
                                           Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBUPartnerLang")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'LanguageCode - Always Given
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))

                Dim sqlStatement As String = "SELECT * FROM [tbl_adhoc_fees] WHERE "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If (outputDataTable.Rows.Count > 0) Then
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Gets the fee records based on the given business unit, partner, language code, fee type and fee source.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partner">The partner</param>
        ''' <param name="languageCode">The langauge code</param>
        ''' <param name="feeType">The fee type</param>
        ''' <param name="feeSource">The fee source</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBUPartnerLangFeeTypeFeeSource(ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, _
                                           ByVal feeType As String, Optional ByVal feeSource As String = "", _
                                           Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBUPartnerLangFeeTypeFeeSource")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'LanguageCode - Always Given
            'FeeType - Always Given
            'FeeSource - Always Given
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner AND LANGUAGE_CODE=@LanguageCode"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeSource", feeSource))

                Dim sqlStatement As String = "SELECT * FROM [tbl_adhoc_fees] WHERE FEE_IN_USE=1 AND "
                If Not String.IsNullOrEmpty(feeSource) Then
                    sqlStatement &= "FEE_SOURCE=@FeeSource AND "
                End If
                Dim feeTypeArray As String() = Split(feeType, ",")
                Dim i As Integer = 0
                sqlStatement &= "FEE_TYPE IN ("
                For Each item As String In feeTypeArray
                    If i > 0 Then sqlStatement &= ", "
                    sqlStatement &= "'" & item & "'"
                    i += 1
                Next
                sqlStatement &= ") AND "

                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If (outputDataTable.Rows.Count > 0) Then
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next
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
            Dim sqlStatement As String = "DELETE TBL_ADHOC_FEES " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_adhoc_fees
        ''' irrespective of whether destination business exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromBusinessUnit">From business unit.</param>
        ''' <param name="toBusinessUnit">To business unit.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "INSERT INTO tbl_adhoc_fees (" & _
                    "BUSINESS_UNIT, PARTNER, LANGUAGE_CODE, FEE_CODE, FEE_DESCRIPTION, IS_NEGATIVE_FEE) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, PARTNER, LANGUAGE_CODE, FEE_CODE, FEE_DESCRIPTION, IS_NEGATIVE_FEE " & _
                    "FROM tbl_adhoc_fees " & _
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
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Copies the fees from Talent and updates tbl_adhoc_fees
        ''' </summary>
        ''' <param name="dtTalentAdhocFees">Talent Adhoc Fees Table</param>
        ''' <param name="businessUnit">Business Unit value</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function RefreshAdhocFeesFromTalent(ByVal dtTalentAdhocFees As DataTable, ByVal businessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "UPDATE tbl_adhoc_fees SET FEE_PRICE=@FeePrice " & _
                    "WHERE FEE_CODE=@FeeCode AND BUSINESS_UNIT=@BusinessUnit AND FEE_SOURCE='TALENTTKT'"

                For Each row As DataRow In dtTalentAdhocFees.Rows
                    talentSqlAccessDetail.CommandElements.CommandParameter.Clear()
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeeCode", row("FeeCode").ToString))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FeePrice", row("FeePrice").ToString))
                    'Execute
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail.SQLAccess()
                    Else
                        err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                    End If
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        affectedRows += talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                Next
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

