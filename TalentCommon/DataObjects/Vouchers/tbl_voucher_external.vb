Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page_attribute based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_vouchers_external
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings
        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_vouchers_external"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_page_attribute" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Deletes the specified external voucher.
        ''' </summary>
        ''' <param name="deVouchersExternal">The external voucher to delete.</param>
        ''' <param name="affectedRows">The number of deleted records.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Delete(ByVal deVouchersExternal As DEVouchersExternal, Optional ByRef affectedRows As Integer = 0,
                               Optional ByVal givenTransaction As SqlTransaction = Nothing) As ErrorObj

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            sqlStatement.Append("DELETE TBL_VOUCHERS_EXTERNAL ")
            sqlStatement.Append("WHERE BUSINESS_UNIT= @BUSINESS_UNIT AND VOUCHER_ID = @VOUCHER_ID ")
            sqlStatement.Append("AND COMPANY= @COMPANY AND PRICE = @PRICE ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", deVouchersExternal.BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", deVouchersExternal.VoucherId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", deVouchersExternal.Company))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE", deVouchersExternal.Price))

            'Execute
            Try
                If (givenTransaction Is Nothing) Then
                    errObj = talentSqlAccessDetail.SQLAccess()
                Else
                    errObj = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    affectedRows = 0
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TVOUCEXT-01"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return errObj

        End Function

        Public Function Delete(ByVal ExternalVoucherId As Integer, Optional ByRef affectedRows As Integer = 0,
                               Optional ByVal givenTransaction As SqlTransaction = Nothing) As ErrorObj

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            sqlStatement.Append("DELETE TBL_VOUCHERS_EXTERNAL ")
            sqlStatement.Append("WHERE ID= @ID")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", ExternalVoucherId))

            'Execute
            Try
                If (givenTransaction Is Nothing) Then
                    errObj = talentSqlAccessDetail.SQLAccess()
                Else
                    errObj = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    affectedRows = 0
                End If
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TVOUCEXT-01"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return errObj

        End Function

        ''' <summary>
        ''' Inserts a record in the tbl_vouchers_external table
        ''' </summary>
        ''' <returns>Error Object.</returns>
        ''' <remarks></remarks>
        Public Function Insert(ByVal BusinessUnit As String, ByVal Partner As String, ByVal VoucherId As Integer, ByVal Company As String,
                                        ByVal Price As String, ByVal AgreementCode As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", deVouchersExternal.Id))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", Partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", Company))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE", Price))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AGREEMENT_CODE", AgreementCode))



            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder


            sqlStatement.Append("SELECT * FROM tbl_vouchers_external ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER=@PARTNER AND VOUCHER_ID=@VOUCHER_ID AND ")
            sqlStatement.Append("COMPANY=@COMPANY AND PRICE=@PRICE AND AGREEMENT_CODE=@AGREEMENT_CODE")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("IF @@ROWCOUNT <> 0 BEGIN ")
            sqlStatement.Append("SELECT 0 as NC ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("END")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("ELSE IF @@ROWCOUNT = 0")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("BEGIN ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("INSERT INTO TBL_VOUCHERS_EXTERNAL(BUSINESS_UNIT, PARTNER, ")
            sqlStatement.Append(" VOUCHER_ID, COMPANY, PRICE, AGREEMENT_CODE) VALUES ")
            sqlStatement.Append("(@BUSINESS_UNIT, @PARTNER, @VOUCHER_ID, @COMPANY, @PRICE, @AGREEMENT_CODE) ")
            sqlStatement.Append(" SELECT 1 as NC END")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            Try
                errObj = talentSqlAccessDetail.SQLAccess()

                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(1).Rows(0)("NC")
                Else
                    affectedRows = 0
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TVOUCEXT-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return errObj

        End Function

        ''' <summary>
        ''' Updates AgreementCode in the tbl_vouchers_external table
        ''' </summary>
        ''' <returns>Error Object.</returns>
        ''' <remarks></remarks>
        Public Function UpdateAgreementCode(ByVal BusinessUnit As String, ByVal Partner As String, ByVal VoucherId As Integer, ByVal Company As String,
                                        ByVal Price As String, ByVal AgreementCode As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", deVouchersExternal.Id))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", Company))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE", Price))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AGREEMENT_CODE", AgreementCode))



                Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
                'SET NOCOUNT ON
                'sqlStatement.Append("SET NOCOUNT ON ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append("IF NOT EXISTS (SELECT * FROM TBL_VOUCHERS_EXTERNAL WHERE AGREEMENT_CODE=@AGREEMENT_CODE AND PRICE = @PRICE and COMPANY = @COMPANY) ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append("UPDATE TBL_VOUCHERS_EXTERNAL SET AGREEMENT_CODE = @AGREEMENT_CODE ")
                sqlStatement.Append("WHERE BUSINESS_UNIT =  @BUSINESS_UNIT AND PARTNER=@PARTNER ")
                sqlStatement.Append("AND COMPANY = @COMPANY AND PRICE = @PRICE AND VOUCHER_ID=@VOUCHER_ID ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append("END")

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                errObj = talentSqlAccessDetail.SQLAccess()

                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    affectedRows = -5
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TVOUCEXT-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return errObj

        End Function

        ''' <summary>
        ''' Updates price in the tbl_vouchers_external table
        ''' </summary>
        ''' <returns>Error Object.</returns>
        ''' <remarks></remarks>
        Public Function UpdatePrice(ByVal BusinessUnit As String, ByVal Partner As String, ByVal VoucherId As Integer, ByVal Company As String,
                                        ByVal Price As String, ByVal AgreementCode As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", deVouchersExternal.Id))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", Company))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRICE", Price))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AGREEMENT_CODE", AgreementCode))



                Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
                'SET NOCOUNT ON
                'sqlStatement.Append("SET NOCOUNT ON ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append("IF NOT EXISTS (SELECT * FROM TBL_VOUCHERS_EXTERNAL WHERE COMPANY=@COMPANY AND PRICE=@PRICE AND AGREEMENT_CODE=@AGREEMENT_CODE) ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append("UPDATE TBL_VOUCHERS_EXTERNAL SET PRICE = @PRICE ")
                sqlStatement.Append("WHERE BUSINESS_UNIT =  @BUSINESS_UNIT AND PARTNER=@PARTNER ")
                sqlStatement.Append("AND COMPANY = @COMPANY AND VOUCHER_ID=@VOUCHER_ID ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append("END")

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                errObj = talentSqlAccessDetail.SQLAccess()

                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    affectedRows = -5
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TVOUCEXT-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return errObj

        End Function

        ''' <summary>
        ''' Gets the voucher prices using voucer id and business unit
        ''' </summary>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Function GetVoucherPricesByVoucherIdAndBU(ByVal BusinessUnit As String, ByVal VoucherId As Integer,
                                                         Optional ByVal cacheing As Boolean = True,
                                                         Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherPricesByVoucherIdAndBU")
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

            sqlStatement.Append("SELECT ID, COMPANY, PRICE, AGREEMENT_CODE FROM TBL_VOUCHERS_EXTERNAL WHERE VOUCHER_ID = @VOUCHER_ID ")
            sqlStatement.Append(" AND BUSINESS_UNIT = @BUSINESS_UNIT ")

            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
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
        ''' Gets the voucher prices using business unit
        ''' </summary>
        ''' <param name="deVouchersExternal">An instance of entity class DEVouchersExternal.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Function GetVoucherPricesByBU(ByVal deVouchersExternal As DEVouchersExternal,
                                                         Optional ByVal cacheing As Boolean = True,
                                                         Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherPricesByBU")
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & deVouchersExternal.BusinessUnit
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

            sqlStatement.Append("SELECT COMPANY, PRICE FROM TBL_VOUCHERS_EXTERNAL WHERE BUSINESS_UNIT = @BUSINESS_UNIT ")

            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", deVouchersExternal.BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
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
        ''' Gets all the distinct companies in the TBL_VOUCHERS_EXTERNAL table
        ''' </summary>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Function GetVoucherCompanies(ByVal BusinessUnit As String, ByVal VoucherId As Integer, ByVal Partner As String,
                                            Optional ByVal cacheing As Boolean = True,
                                            Optional ByVal cacheTimeMinutes As Integer = 30,
                                            Optional ByVal RetreiveAllCompanies As Boolean = False) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherCompanies")
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & BusinessUnit & VoucherId
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet


            sqlStatement.Append("SELECT DISTINCT COMPANY FROM TBL_VOUCHERS_EXTERNAL ")
            If Not RetreiveAllCompanies Then
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", Partner))
                sqlStatement.Append(" WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER = @PARTNER_CODE AND VOUCHER_ID = @VOUCHER_ID")

            End If


            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If

            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable

        End Function

        Public Function GetVoucherCompaniesByBU(ByVal BusinessUnit As String, Optional ByVal cacheing As Boolean = True,
                                                         Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherCompaniesByBU")

            'Construct The Call
            Try
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & BusinessUnit
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                sqlStatement.Append("SELECT DISTINCT COMPANY FROM TBL_VOUCHERS_EXTERNAL WHERE  BUSINESS_UNIT=@BUSINESS_UNIT OR BUSINESS_UNIT='*ALL'")

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                errObj = talentSqlAccessDetail.SQLAccess()

                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable

        End Function

        Public Function GetVoucherIDByBUAndCompany(ByVal BusinessUnit As String, ByVal Company As String,
                                                         Optional ByVal cacheing As Boolean = True,
                                                         Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherIDByBUAndCompany")


            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & BusinessUnit & Company
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                sqlStatement.Append("SELECT DISTINCT VOUCHER_ID FROM TBL_VOUCHERS_EXTERNAL WHERE COMPANY = @COMPANY AND (BUSINESS_UNIT=@BUSINESS_UNIT OR BUSINESS_UNIT='*ALL')")

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", Company))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                errObj = talentSqlAccessDetail.SQLAccess()

                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable

        End Function

        Public Function GetVoucherPrice(ByVal BusinessUnit As String, ByVal Company As String, ByVal VoucherId As Integer,
                                                         Optional ByVal cacheing As Boolean = True,
                                                         Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherPrice")
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & BusinessUnit & Company & VoucherId.ToString
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

            sqlStatement.Append("SELECT PRICE FROM TBL_VOUCHERS_EXTERNAL WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND COMPANY = @COMPANY")
            sqlStatement.Append(" AND VOUCHER_ID = @VOUCHER_ID")

            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", Company))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If

            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable

        End Function

        Public Function GetVoucherPriceAndAgreement(ByVal BusinessUnit As String, ByVal Company As String, ByVal VoucherId As Integer,
                                                         Optional ByVal cacheing As Boolean = True,
                                                         Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable
            Dim errObj As New ErrorObj
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetVoucherPriceAndAgreement")

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & BusinessUnit & Company & VoucherId.ToString
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                sqlStatement.Append("SELECT PRICE, AGREEMENT_CODE FROM TBL_VOUCHERS_EXTERNAL WHERE COMPANY = @COMPANY")
                sqlStatement.Append(" AND VOUCHER_ID = @VOUCHER_ID AND (BUSINESS_UNIT=@BUSINESS_UNIT OR BUSINESS_UNIT='*ALL')")

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@COMPANY", Company))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VOUCHER_ID", VoucherId))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                errObj = talentSqlAccessDetail.SQLAccess()

                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable

        End Function
#End Region

    End Class

End Namespace
