Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_product
    ''' </summary>
    <Serializable()> _
    Public Class tbl_product
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product"

        ''' <summary>
        ''' To decide whether to continue the multiple insert
        ''' </summary>
        Private _continueInsertMultiple As Boolean = True

        ''' <summary>
        ''' Gets or sets the product description entity.
        ''' </summary>
        ''' <value>
        ''' The product description entity.
        ''' </value>
        Public Property ProductDescEntity() As DEProductDescriptions

        ''' <summary>
        ''' to create only one instance of string when called by insert multiple
        ''' as it is a long string
        ''' as well as to speed up the insert when called under transaction 
        ''' </summary>
        Private ReadOnly _insertSQLStatement As String = String.Empty

        Private ReadOnly _updateSQLStatement As String = String.Empty

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_product" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
            _insertSQLStatement = "INSERT tbl_product (PRODUCT_CODE," &
                " PRODUCT_DESCRIPTION_1, PRODUCT_DESCRIPTION_2, PRODUCT_DESCRIPTION_3, PRODUCT_DESCRIPTION_4, PRODUCT_DESCRIPTION_5," &
                " PRODUCT_LENGTH, PRODUCT_WIDTH, PRODUCT_DEPTH, PRODUCT_HEIGHT, PRODUCT_SIZE, PRODUCT_WEIGHT, PRODUCT_VOLUME," &
                " PRODUCT_VINTAGE, PRODUCT_VEGETARIAN, PRODUCT_VEGAN, PRODUCT_ORGANIC, PRODUCT_BIODYNAMIC, PRODUCT_LUTTE, PRODUCT_MINIMUM_AGE," &
                " PRODUCT_HTML_1, PRODUCT_HTML_2, PRODUCT_HTML_3," &
                " PRODUCT_SEARCH_KEYWORDS," &
                " PRODUCT_SEARCH_RANGE_01, PRODUCT_SEARCH_RANGE_02, PRODUCT_SEARCH_RANGE_03, PRODUCT_SEARCH_RANGE_04, PRODUCT_SEARCH_RANGE_05," &
                " PRODUCT_SEARCH_SWITCH_01, PRODUCT_SEARCH_SWITCH_02, PRODUCT_SEARCH_SWITCH_03, PRODUCT_SEARCH_SWITCH_04, PRODUCT_SEARCH_SWITCH_05," &
                " PRODUCT_SEARCH_SWITCH_06, PRODUCT_SEARCH_SWITCH_07, PRODUCT_SEARCH_SWITCH_08, PRODUCT_SEARCH_SWITCH_09, PRODUCT_SEARCH_SWITCH_10," &
                " PRODUCT_SEARCH_DATE_01, PRODUCT_SEARCH_DATE_02, PRODUCT_SEARCH_DATE_03, PRODUCT_SEARCH_DATE_04, PRODUCT_SEARCH_DATE_05," &
                " PRODUCT_OPTION_MASTER, AVAILABLE_ONLINE, PERSONALISABLE, DISCONTINUED," &
                " PRODUCT_LENGTH_UOM, PRODUCT_WIDTH_UOM, PRODUCT_DEPTH_UOM, PRODUCT_HEIGHT_UOM, PRODUCT_SIZE_UOM, PRODUCT_WEIGHT_UOM, PRODUCT_VOLUME_UOM," &
                " PRODUCT_COLOUR, PRODUCT_PACK_SIZE, PRODUCT_PACK_SIZE_UOM, PRODUCT_SUPPLIER_PART_NO, PRODUCT_CUSTOMER_PART_NO," &
                " PRODUCT_TASTING_NOTES_1, PRODUCT_TASTING_NOTES_2, PRODUCT_ABV, PRODUCT_SUPPLIER, PRODUCT_COUNTRY, PRODUCT_REGION, PRODUCT_AREA, " &
                " PRODUCT_GRAPE, PRODUCT_CLOSURE, PRODUCT_CATALOG_CODE, PRODUCT_PAGE_TITLE, PRODUCT_META_DESCRIPTION, PRODUCT_META_KEYWORDS, " &
                " PRODUCT_SEARCH_CRITERIA_01, PRODUCT_SEARCH_CRITERIA_02, PRODUCT_SEARCH_CRITERIA_03, PRODUCT_SEARCH_CRITERIA_04, PRODUCT_SEARCH_CRITERIA_05, " &
                " PRODUCT_SEARCH_CRITERIA_06, PRODUCT_SEARCH_CRITERIA_07, PRODUCT_SEARCH_CRITERIA_08, PRODUCT_SEARCH_CRITERIA_09, PRODUCT_SEARCH_CRITERIA_10, " &
                " PRODUCT_SEARCH_CRITERIA_11, PRODUCT_SEARCH_CRITERIA_12, PRODUCT_SEARCH_CRITERIA_13, PRODUCT_SEARCH_CRITERIA_14, PRODUCT_SEARCH_CRITERIA_15, " &
                " PRODUCT_SEARCH_CRITERIA_16, PRODUCT_SEARCH_CRITERIA_17, PRODUCT_SEARCH_CRITERIA_18, PRODUCT_SEARCH_CRITERIA_19, PRODUCT_SEARCH_CRITERIA_20, " &
                " PRODUCT_TARIFF_CODE, ALTERNATE_SKU, PRODUCT_GLCODE_1, PRODUCT_GLCODE_2, PRODUCT_GLCODE_3, PRODUCT_GLCODE_4, PRODUCT_GLCODE_5, PRODUCT_HTML_4, PRODUCT_HTML_5, PRODUCT_HTML_6, PRODUCT_HTML_7, PRODUCT_HTML_8, PRODUCT_HTML_9) VALUES " &
                "(@PRODUCT_CODE," &
                " @PRODUCT_DESCRIPTION_1, @PRODUCT_DESCRIPTION_2, @PRODUCT_DESCRIPTION_3, @PRODUCT_DESCRIPTION_4, @PRODUCT_DESCRIPTION_5," &
                " @PRODUCT_LENGTH, @PRODUCT_WIDTH, @PRODUCT_DEPTH, @PRODUCT_HEIGHT, @PRODUCT_SIZE, @PRODUCT_WEIGHT, @PRODUCT_VOLUME," &
                " @PRODUCT_VINTAGE, @PRODUCT_VEGETARIAN, @PRODUCT_VEGAN, @PRODUCT_ORGANIC, @PRODUCT_BIODYNAMIC, @PRODUCT_LUTTE, @PRODUCT_MINIMUM_AGE," &
                " @PRODUCT_HTML_1, @PRODUCT_HTML_2, @PRODUCT_HTML_3," &
                " @PRODUCT_SEARCH_KEYWORDS," &
                " @PRODUCT_SEARCH_RANGE_01, @PRODUCT_SEARCH_RANGE_02, @PRODUCT_SEARCH_RANGE_03, @PRODUCT_SEARCH_RANGE_04, @PRODUCT_SEARCH_RANGE_05," &
                " @PRODUCT_SEARCH_SWITCH_01, @PRODUCT_SEARCH_SWITCH_02, @PRODUCT_SEARCH_SWITCH_03, @PRODUCT_SEARCH_SWITCH_04, @PRODUCT_SEARCH_SWITCH_05," &
                " @PRODUCT_SEARCH_SWITCH_06, @PRODUCT_SEARCH_SWITCH_07, @PRODUCT_SEARCH_SWITCH_08, @PRODUCT_SEARCH_SWITCH_09, @PRODUCT_SEARCH_SWITCH_10," &
                " @PRODUCT_SEARCH_DATE_01, @PRODUCT_SEARCH_DATE_02, @PRODUCT_SEARCH_DATE_03, @PRODUCT_SEARCH_DATE_04, @PRODUCT_SEARCH_DATE_05," &
                " @PRODUCT_OPTION_MASTER, @AVAILABLE_ONLINE, @PERSONALISABLE, @DISCONTINUED," &
                " '','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','',''," &
                " @PRODUCT_GLCODE_1, @PRODUCT_GLCODE_2, @PRODUCT_GLCODE_3, @PRODUCT_GLCODE_4, @PRODUCT_GLCODE_5, @PRODUCT_HTML_4, @PRODUCT_HTML_5, @PRODUCT_HTML_6, @PRODUCT_HTML_7, @PRODUCT_HTML_8, @PRODUCT_HTML_9)"

            _updateSQLStatement = "UPDATE tbl_product " &
                "SET PRODUCT_CODE = @ProductCode, PRODUCT_DESCRIPTION_1 = @ProductDesc1, " &
                "PRODUCT_DESCRIPTION_2 = @ProductDesc2, PRODUCT_DESCRIPTION_3 = @ProductDesc3, PRODUCT_DESCRIPTION_4 = @ProductDesc4, " &
                "PRODUCT_DESCRIPTION_5 = @ProductDesc5, PRODUCT_HTML_1 = @ProductHTML1, PRODUCT_HTML_2 = @ProductHTML2, PRODUCT_HTML_3 = @ProductHTML3, " &
                "PRODUCT_WEIGHT = @ProductWeight, PRODUCT_WEIGHT_UOM = @ProductWeightUnit, PRODUCT_GLCODE_1 = @ProductGLCode1, PRODUCT_GLCODE_2 = @ProductGLCode2, " &
                "PRODUCT_GLCODE_3 = @ProductGLCode3, PRODUCT_GLCODE_4 = @ProductGLCode4, PRODUCT_GLCODE_5 = @ProductGLCode5, PRODUCT_HTML_4 = @ProductHTML4, PRODUCT_HTML_5 = @ProductHTML5, PRODUCT_HTML_6 = @ProductHTML6, PRODUCT_HTML_7 = @ProductHTML7, PRODUCT_HTML_8 = @ProductHTML8, PRODUCT_HTML_9 = @ProductHTML9, PRODUCT_SEARCH_KEYWORDS=@SearchKeywords  WHERE PRODUCT_ID = @ProductID"
        End Sub
#End Region

#Region "Public Methods"

        Public Function Insert(ByVal productCode As String, ByVal isMaster As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            
            '_insertSQLStatement initiated when instance is created

            talentSqlAccessDetail.CommandElements.CommandText = _insertSQLStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_CODE", productCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_DESCRIPTION_1", ProductDescEntity.Description1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_DESCRIPTION_2", ProductDescEntity.Description2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_DESCRIPTION_3", ProductDescEntity.Description3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_DESCRIPTION_4", ProductDescEntity.Description4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_DESCRIPTION_5", ProductDescEntity.Description5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_LENGTH", ProductDescEntity.Length))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_WIDTH", ProductDescEntity.Width))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_DEPTH", ProductDescEntity.Depth))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HEIGHT", ProductDescEntity.Height))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SIZE", ProductDescEntity.Size))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_WEIGHT", ProductDescEntity.Weight))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_VOLUME", ProductDescEntity.Volume))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_VINTAGE", ProductDescEntity.Vintage))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_VEGETARIAN", ProductDescEntity.Vegetarian))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_VEGAN", ProductDescEntity.Vegan))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_ORGANIC", ProductDescEntity.Organic))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_BIODYNAMIC", ProductDescEntity.BioDynamic))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_LUTTE", ProductDescEntity.Lutte))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_MINIMUM_AGE", ProductDescEntity.MinimumAge))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_1", ProductDescEntity.Html1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_2", ProductDescEntity.Html2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_3", ProductDescEntity.Html3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_KEYWORDS", ProductDescEntity.SearchKeywords))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_RANGE_01", ProductDescEntity.SearchRange1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_RANGE_02", ProductDescEntity.SearchRange2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_RANGE_03", ProductDescEntity.SearchRange3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_RANGE_04", ProductDescEntity.SearchRange4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_RANGE_05", ProductDescEntity.SearchRange5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_01", ProductDescEntity.SearchSwitch1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_02", ProductDescEntity.SearchSwitch2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_03", ProductDescEntity.SearchSwitch3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_04", ProductDescEntity.SearchSwitch4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_05", ProductDescEntity.SearchSwitch5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_06", ProductDescEntity.SearchSwitch6))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_07", ProductDescEntity.SearchSwitch7))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_08", ProductDescEntity.SearchSwitch8))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_09", ProductDescEntity.SearchSwitch9))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_SWITCH_10", ProductDescEntity.SearchSwitch10))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_DATE_01", ProductDescEntity.SearchDate1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_DATE_02", ProductDescEntity.SearchDate2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_DATE_03", ProductDescEntity.SearchDate3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_DATE_04", ProductDescEntity.SearchDate4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_SEARCH_DATE_05", ProductDescEntity.SearchDate5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_OPTION_MASTER", isMaster))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AVAILABLE_ONLINE", ProductDescEntity.AvailableOnline))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PERSONALISABLE", ProductDescEntity.Personalisable))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DISCONTINUED", ProductDescEntity.Discontinued))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_GLCODE_1", ProductDescEntity.GLCode1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_GLCODE_2", ProductDescEntity.GLCode2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_GLCODE_3", ProductDescEntity.GLCode3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_GLCODE_4", ProductDescEntity.GLCode4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_GLCODE_5", ProductDescEntity.GLCode5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_4", ProductDescEntity.Html4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_5", ProductDescEntity.Html5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_6", ProductDescEntity.Html6))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_7", ProductDescEntity.Html7))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_8", ProductDescEntity.Html8))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT_HTML_9", ProductDescEntity.Html9))

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

        Public Function InsertMultiple(ByVal productOptionEntityList As Generic.List(Of DEProductOption), Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim totalAffectedRows As Integer = 0
            If (productOptionEntityList IsNot Nothing) AndAlso (productOptionEntityList.Count > 0) Then
                _continueInsertMultiple = True
                Dim affectedRows As Integer = 0
                For productOptionEntityIndex As Integer = 0 To productOptionEntityList.Count - 1
                    'to make sure there is no error in the Insert method
                    If (_continueInsertMultiple) Then
                        affectedRows = Insert(productOptionEntityList(productOptionEntityIndex).ProductCode, False, givenTransaction)
                        totalAffectedRows = totalAffectedRows + affectedRows
                    Else
                        totalAffectedRows = 0
                        Exit For
                    End If
                Next
            End If
            'Return the results 
            Return totalAffectedRows
        End Function

        Public Function GetProductByProductCode(ByVal productCode As String, ByVal isMaster As Boolean, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetProductByProductCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP 1 PRODUCT_ID, PRODUCT_CODE, PRODUCT_OPTION_MASTER" &
                    " FROM tbl_product WHERE PRODUCT_CODE = @ProductCode AND PRODUCT_OPTION_MASTER = @ProductOptionMaster"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductOptionMaster", isMaster))

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

        Public Function GetProductByProductID(ByVal productID As String) As DataTable

            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_product WHERE PRODUCT_ID = @ProductID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductID", productID))

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

        Public Function UpdateProduct(ByVal productCode As String, ByVal productDesc1 As String, ByVal productDesc2 As String, ByVal productDesc3 As String, ByVal productDesc4 As String, ByVal productDesc5 As String, ByVal productHTML1 As String, ByVal productHTML2 As String, ByVal productHTML3 As String, ByVal searchKeywords As String, ByVal productWeight As String, ByVal productWeightUnit As String, ByVal productGLCode1 As String, ByVal productGLCode2 As String, ByVal productGLCode3 As String, ByVal productGLCode4 As String, ByVal productGLCode5 As String, ByVal productID As String, Optional ByVal productHTML4 As String = "", Optional ByVal productHTML5 As String = "", Optional ByVal productHTML6 As String = "", Optional ByVal productHTML7 As String = "", Optional ByVal productHTML8 As String = "", Optional ByVal productHTML9 As String = "") As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = _updateSQLStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDesc1", productDesc1))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDesc2", productDesc2))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDesc3", productDesc3))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDesc4", productDesc4))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductDesc5", productDesc5))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML1", productHTML1))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML2", productHTML2))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML3", productHTML3))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductWeight", productWeight))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductWeightUnit", productWeightUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductGLCode1", productGLCode1))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductGLCode2", productGLCode2))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductGLCode3", productGLCode3))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductGLCode4", productGLCode4))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductGLCode5", productGLCode5))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML4", productHTML4))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML5", productHTML5))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML6", productHTML6))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML7", productHTML7))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML8", productHTML8))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductHTML9", productHTML9))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductID", productID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SearchKeywords", searchKeywords))

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

            'Return the count
            Return affectedRows
        End Function

        ''' <summary>
        ''' Get the product codes with descriptions by business unit and partner
        ''' </summary>
        ''' <param name="businessUnit">The given business unit as a string</param>
        ''' <param name="partner">The given partner as a string</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllProductDescriptionsByBUAndPartner(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, _
                                                             Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllProductDescriptionsByBUAndPartner")
            Dim talentSqlAccessDetail As New TalentDataAccess


            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                Dim sqlStatement As String = "SELECT PRODUCT_CODE, PRODUCT_DESCRIPTION_1, PRODUCT_DESCRIPTION_2 FROM [tbl_product] "
                Dim err As New ErrorObj

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
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
        ''' Removes product by product code
        ''' </summary>
        ''' <param name="productCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveProductByProduct(ByVal productCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM tbl_product WHERE PRODUCT_CODE=@ProductCode"

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




