Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_product_options
    ''' </summary>
    <Serializable()> _
    Public Class tbl_product_options
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product_options"

        ''' <summary>
        ''' To decide whether to continue the multiple insert
        ''' </summary>
        Private _continueInsertMultiple As Boolean = True

        ''' <summary>
        ''' Gets or sets the product option entity.
        ''' </summary>
        ''' <value>
        ''' The product option entity.
        ''' </value>
        Public Property ProductOptionEntityList() As Generic.List(Of DEProductOption)

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
        ''' Initializes a new instance of the <see cref="tbl_product_options" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
            _insertSQLStatement = "INSERT tbl_product_options " & _
                                    "(BUSINESS_UNIT,PARTNER, MASTER_PRODUCT, OPTION_TYPE, OPTION_CODE, " & _
                                    "PRODUCT_CODE, DISPLAY_ORDER) VALUES " & _
                                    "(@BusinessUnit, @Partner, @MasterProduct, @OptionType, @OptionCode,  " & _
                                    "@ProductCode, @DisplayOrder) "
        End Sub
#End Region

#Region "Public Methods"

        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, ByVal ProductOptionEntity As DEProductOption, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

            '_insertSQLStatement initiated when instance is created

            talentSqlAccessDetail.CommandElements.CommandText = _insertSQLStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProduct", ProductOptionEntity.MasterProduct))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OptionType", ProductOptionEntity.OptionType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OptionCode", ProductOptionEntity.OptionCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", ProductOptionEntity.ProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DisplayOrder", ProductOptionEntity.DisplayOrder))

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

        Public Function InsertMultiple(ByVal businessUnit As String, ByVal partner As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim totalAffectedRows As Integer = 0
            If Not (ProductOptionEntityList Is Nothing) Then
                If (ProductOptionEntityList.Count > 0) Then
                    _continueInsertMultiple = True
                    Dim affectedRows As Integer = 0
                    For productOptionEntityIndex As Integer = 0 To ProductOptionEntityList.Count - 1
                        'to make sure there is no error in the Insert method
                        If (_continueInsertMultiple) Then
                            affectedRows = Insert(businessUnit, partner, ProductOptionEntityList(productOptionEntityIndex), givenTransaction)
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
        ''' Remove options products by master product
        ''' </summary>
        ''' <param name="masterProductCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveProductAndOptions(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM tbl_product_options Where MASTER_PRODUCT=@MasterProductCode"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProductCode", masterProductCode))
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

        ''' <summary>
        ''' Retrieves options by master product - BUSINESS_UNIT hierachy
        ''' </summary>
        ''' <param name="businessUnit"></param>
        ''' <param name="masterProductCode"></param>
        ''' <param name="cacheing"></param>
        ''' <param name="cacheTimeMinutes"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetProductOptionByMasterProduct(ByVal businessUnit As String, ByVal masterProductCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetProductOptionByMasterProduct")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Dim whereClauseFetchHierarchy(2) As String

            whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit"

            whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT PRODUCT_CODE " & _
                               "FROM tbl_product_options WHERE MASTER_PRODUCT = @MasterProductCode "
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProductCode", masterProductCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                'Execute
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
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
        ''' Retrieves master product code based on the given child product code
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="productCode">The given child (option) product code</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMasterProductCodeByOptionCode(ByVal businessUnit As String, ByVal productCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim masterProductCode As String = String.Empty
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetMasterProductCodeByOptionCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Dim whereClauseFetchHierarchy(2) As String
            whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit"
            whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT MASTER_PRODUCT FROM tbl_product_options WHERE PRODUCT_CODE = @ProductCode "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                'Execute
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
                            masterProductCode = outputDataTable.Rows(0)("MASTER_PRODUCT").ToString()
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
            Return masterProductCode
        End Function

#End Region

    End Class
End Namespace



