Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Imports System.Text
Imports Talent.Common
Imports System.Collections.Specialized


Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access product related details like stock and prices
    ''' </summary>
    <Serializable()> _
    Public Class Products
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblGroupProduct As tbl_group_product
        Private _tblProductStock As tbl_product_stock
        Private _tblPriceListDetail As tbl_price_list_detail
        Private _tblProductOptionTypes As tbl_product_option_types
        Private _tblProductOptionDefaults As tbl_product_option_defaults
        Private _tblProductOptionDefinition As tbl_product_option_definitions
        Private _tblProductOptions As tbl_product_options
        Private _tblProduct As tbl_product
        Private _tblProductRelations As tbl_product_relations
        Private _tblProductRelationsAttributeValues As tbl_product_relations_attribute_values
        Private _tblProductRelationsTextLang As tbl_product_relations_text_lang
        Private _tblEventCategory As tbl_event_category
        Private _tblProductSpecificContent As tbl_product_specific_content
        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Products"

        Private Const SOURCEAPPLICATION As String = "MAINTENANCE"
        Private Const SOURCECLASS As String = "PRODUCTS"

        ''' <summary>
        ''' Have a same source method name for adding product with options as well as without options
        ''' this will stop concurrent adding of same master product 
        ''' </summary>
        Private Const SOURCEMETHOD_ADDPRODUCT = "ADDPRODUCT"
        Private Const SOURCEMETHOD_REMOVE_PRODUCT_WITH_OPTION = "REMOVEPRODUCTWITHOPTION"
        Private Const SOURCEMETHOD_REMOVE_PRODUCT_WITHOUT_OPTION = "REMOVEPRODUCTWITHOUTOPTION"
        Private requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2", _
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4", _
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6", _
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8", _
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}
        Private pageLevel() As String = {"browse01.aspx", "browse02.aspx", "browse03.aspx", "browse04.aspx", "browse05.aspx", _
                                    "browse06.aspx", "browse07.aspx", "browse08.aspx", "browse09.aspx", "browse10.aspx"}

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Products" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Create and Gets the tbl_group_product instance with DESettings
        ''' </summary>
        ''' <value>tbl_group_product instance</value>
        Public ReadOnly Property TblGroupProduct() As tbl_group_product
            Get
                If (_tblGroupProduct Is Nothing) Then
                    _tblGroupProduct = New tbl_group_product(_settings)
                End If
                Return _tblGroupProduct
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_stock instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_stock instance</value>
        Public ReadOnly Property TblProductStock() As tbl_product_stock
            Get
                If (_tblProductStock Is Nothing) Then
                    _tblProductStock = New tbl_product_stock(_settings)
                End If
                Return _tblProductStock
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_price_list_detail instance with DESettings
        ''' </summary>
        ''' <value>tbl_price_list_detail instance</value>
        Public ReadOnly Property TblPriceListDetail() As tbl_price_list_detail
            Get
                If (_tblPriceListDetail Is Nothing) Then
                    _tblPriceListDetail = New tbl_price_list_detail(_settings)
                End If
                Return _tblPriceListDetail
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_option_types instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_option_types instance</value>
        Public ReadOnly Property TblProductOptionTypes() As tbl_product_option_types
            Get
                If (_tblProductOptionTypes Is Nothing) Then
                    _tblProductOptionTypes = New tbl_product_option_types(_settings)
                End If
                Return _tblProductOptionTypes
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_option_defaults instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_option_defaults instance</value>
        Public ReadOnly Property TblProductOptionDefaults() As tbl_product_option_defaults
            Get
                If (_tblProductOptionDefaults Is Nothing) Then
                    _tblProductOptionDefaults = New tbl_product_option_defaults(_settings)
                End If
                Return _tblProductOptionDefaults
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_option_definitions instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_option_definitions instance</value>
        Public ReadOnly Property TblProductOptionDefinition() As tbl_product_option_definitions
            Get
                If (_tblProductOptionDefinition Is Nothing) Then
                    _tblProductOptionDefinition = New tbl_product_option_definitions(_settings)
                End If
                Return _tblProductOptionDefinition
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_options instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_options instance</value>
        Public ReadOnly Property TblProductOptions() As tbl_product_options
            Get
                If (_tblProductOptions Is Nothing) Then
                    _tblProductOptions = New tbl_product_options(_settings)
                End If
                Return _tblProductOptions
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product instance with DESettings
        ''' </summary>
        ''' <value>tbl_product instance</value>
        Public ReadOnly Property TblProduct() As tbl_product
            Get
                If (_tblProduct Is Nothing) Then
                    _tblProduct = New tbl_product(_settings)
                End If
                Return _tblProduct
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_relations instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_relations instance</value>
        Public ReadOnly Property TblProductRelations() As tbl_product_relations
            Get
                If (_tblProductRelations Is Nothing) Then
                    _tblProductRelations = New tbl_product_relations(_settings)
                End If
                Return _tblProductRelations
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_relations_attribute_values instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_relations_attribute_values instance</value>
        Public ReadOnly Property TblProductRelationsAttributeValues() As tbl_product_relations_attribute_values
            Get
                If (_tblProductRelationsAttributeValues Is Nothing) Then
                    _tblProductRelationsAttributeValues = New tbl_product_relations_attribute_values(_settings)
                End If
                Return _tblProductRelationsAttributeValues
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_relations_text_lang instance with DESettings
        ''' </summary>
        ''' <value>tbl_product_relations_text_lang instance</value>
        Public ReadOnly Property TblProductRelationsTextLang() As tbl_product_relations_text_lang
            Get
                If (_tblProductRelationsTextLang Is Nothing) Then
                    _tblProductRelationsTextLang = New tbl_product_relations_text_lang(_settings)
                End If
                Return _tblProductRelationsTextLang
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_event_category instance with DESettings
        ''' </summary>
        ''' <value>tbl_event_category instance</value>
        Public ReadOnly Property TblEventCategory() As tbl_event_category
            Get
                If (_tblEventCategory Is Nothing) Then
                    _tblEventCategory = New tbl_event_category(_settings)
                End If
                Return _tblEventCategory
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_product_specific_content with DESettings
        ''' </summary>
        ''' <value>tbl_product_specific_content</value>
        Public ReadOnly Property TblProductSpecificContent() As tbl_product_specific_content
            Get
                If (_tblProductSpecificContent Is Nothing) Then
                    _tblProductSpecificContent = New tbl_product_specific_content(_settings)
                End If
                Return _tblProductSpecificContent
            End Get
        End Property
#End Region

        ''' <summary>
        ''' Get the datatable of records to be used in the auto complete product search
        ''' </summary>
        ''' <param name="fieldToSearchOn">The data field we are using to display and search on</param>
        ''' <param name="productOptionMaster">Filter the results by master/child product</param>
        ''' <param name="cacheing">Cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time, default 30 mins</param>
        ''' <returns>A datatable of results based on the criteria</returns>
        ''' <remarks></remarks>
        Public Function GetDataForAutoCompleteProductSearch(ByVal fieldToSearchOn As String, ByVal productOptionMaster As Boolean, ByVal businessUnit As String, ByVal partner As String, ByVal priceList As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDataForAutoCompleteProductSearch" & fieldToSearchOn & productOptionMaster.ToString())
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix

                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT [PRODUCT_SEARCH_KEYWORDS] FROM tbl_group_product AS gp WITH (NOLOCK)   JOIN tbl_product AS p WITH (NOLOCK)  ON gp.PRODUCT = p.PRODUCT_CODE JOIN tbl_price_list_detail AS pld WITH (NOLOCK)")
                sqlStatement.Append(" On gp.PRODUCT = pld.PRODUCT  WHERE p.[PRODUCT_OPTION_MASTER] = @ProductOptionMaster AND (gp.GROUP_BUSINESS_UNIT ='@BusinessUnit' OR gp.GROUP_BUSINESS_UNIT = '*ALL')")
                sqlStatement.Append(" And (gp.GROUP_PARTNER = '@Partner' OR gp.GROUP_PARTNER='*ALL') AND pld.PRICE_LIST ='@PriceList'")

                If productOptionMaster Then
                    sqlStatement.Replace("@ProductOptionMaster", 1)
                Else
                    sqlStatement.Replace("@ProductOptionMaster", 0)
                End If

                sqlStatement.Replace("@BusinessUnit", businessUnit)
                sqlStatement.Replace("@Partner", partner)
                sqlStatement.Replace("@PriceList", priceList)
                Dim err As New ErrorObj

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
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

        Public Function GetProducts(ByVal businessUnit As String, ByVal partner As String, ByVal intMaxNoOfGroupLevels As Integer, ByVal currentPageLevel As Integer, ByVal grouplist As Generic.Dictionary(Of String, String), Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetProducts")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
                For Each group As KeyValuePair(Of String, String) In grouplist
                    If group.Value <> String.Empty Then
                        talentSqlAccessDetail.Settings.CacheStringExtension += group.ToString
                    End If
                Next

                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

                Dim intCount As Integer = 0
                Dim sqlStatement As New StringBuilder
                Dim whereClause As New StringBuilder
                Dim selectStatement As New StringBuilder
                selectStatement.Append("SELECT * FROM TBL_GROUP_PRODUCT AS GP WITH (NOLOCK) INNER JOIN TBL_PRODUCT AS PD WITH (NOLOCK) ON GP.PRODUCT = PD.PRODUCT_CODE WHERE ")
                With whereClause
                    While intCount < intMaxNoOfGroupLevels
                        .Append(requestGroups(intCount))
                        .Append(" AND ")
                        intCount += 1
                        If intCount >= currentPageLevel Then
                            Exit While
                        End If
                    End While
                    .Append(" (GP.GROUP_BUSINESS_UNIT = @BusinessUnit OR GP.GROUP_BUSINESS_UNIT = '*ALL') AND")
                    .Append(" GP.GROUP_PARTNER = @Partner AND ")
                    .Append(" PD.PRODUCT_OPTION_MASTER = 'True' ")
                End With

                sqlStatement.Append("IF ((")
                sqlStatement.Append("SELECT COUNT(*) FROM TBL_GROUP_PRODUCT AS GP WITH (NOLOCK) INNER JOIN TBL_PRODUCT AS PD WITH (NOLOCK) ON GP.PRODUCT = PD.PRODUCT_CODE WHERE ")
                sqlStatement.Append(whereClause)
                sqlStatement.Append(" AND ISNUMERIC(SEQUENCE) = 0) > 0) ")
                sqlStatement.Append("BEGIN ")
                sqlStatement.Append(selectStatement)
                sqlStatement.Append(whereClause)
                sqlStatement.Append(" ORDER BY GP.SEQUENCE, GP.PRODUCT ")
                sqlStatement.Append(" END ")
                sqlStatement.Append(" ELSE ")
                sqlStatement.Append(selectStatement)
                sqlStatement.Append(whereClause)
                sqlStatement.Append(" ORDER BY CAST(GP.SEQUENCE AS INT), GP.PRODUCT ")

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                For Each group As KeyValuePair(Of String, String) In grouplist
                    If sqlStatement.ToString().Contains("@" & group.Key.ToUpper) Then
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter(group.Key, group.Value))
                    End If
                Next

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

        Public Function AddProductWithOptions(ByVal businessUnit As String, ByVal partner As String, _
                                   ByVal masterProductCode As String, _
                                   ByVal deProductOptionEntityList As Generic.List(Of DEProductOption), _
                                   ByVal deStockProductEntityList As Generic.List(Of DEStockProduct), _
                                   ByVal deProductPriceEntityList As Generic.List(Of DEProductPrice), _
                                   ByVal deProductOptionDefaultEntity As DEProductOptionDefault, _
                                   ByVal deProductDescriptionEntity As DEProductDescriptions) As String
            Dim returnMessage As String = String.Empty


            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlTrans As SqlTransaction
            Dim affectedRows As Integer
            Dim logContent As String = String.Empty
            Dim sourceClassName As String = String.Empty
            Dim SOURCEMETHOD As String = SOURCEMETHOD_ADDPRODUCT
            Dim logHeaderId As Integer = 0

            'Logging Instance
            _settings.Cacheing = False
            Dim loggingSettings As New Logging(_settings)

            Try
                talentSqlAccessDetail.Settings = _settings

                'check functionality is active or not
                If Not (loggingSettings.TblLogHeader.IsActive(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS)) Then


                    Dim isErrorinTransaction As Boolean = False

                    Dim additionalDetails As String = "Adding Product with options " & _
                        " BusinessUnit : " & businessUnit & _
                        " Partner : " & partner & _
                        " Master Product Code : " & masterProductCode

                    affectedRows = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

                    Me.TblProductOptionDefaults.ProductOptionDefaultEntity = deProductOptionDefaultEntity
                    Me.TblProduct.ProductDescEntity = deProductDescriptionEntity
                    Me.TblProductOptions.ProductOptionEntityList = deProductOptionEntityList
                    Me.TblProductStock.ProductStockEntityList = deStockProductEntityList
                    Me.TblPriceListDetail.ProductPriceEntityList = deProductPriceEntityList

                    'Have You Finished all kinds validation and initialise any common settings
                    'make sure everything ready so that under transaction just open and execute
                    'transaction starts

                    If (logHeaderId > 0) Then

                        'create and get the transaction object
                        sqlTrans = talentSqlAccessDetail.BeginTransaction(err)

                        'no error then execute the statements using transaction object
                        If (Not (err.HasError)) Then

                            'tbl_product_option_defaults
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_option_defaults"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProductOptionDefaults.Insert(businessUnit, partner, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Insert", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If

                            'tbl_product_options
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_options"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProductOptions.InsertMultiple(businessUnit, partner, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "InsertMultiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If

                            'tbl_product_stock
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_stock"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProductStock.InsertMultiple(partner, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "InsertMultiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If

                            'tbl_price_list_detail
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_price_list_detail"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblPriceListDetail.InsertMultiple(partner, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "InsertMultiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If

                            'This table is gateway for product so do the action always in the last
                            'tbl_product inserting only master product
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProduct.Insert(masterProductCode, True, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Insert", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting master product records to " & sourceClassName)
                                End If
                            End If

                            'tbl_product inserting only options
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProduct.InsertMultiple(deProductOptionEntityList, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "InsertMultiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If
                            'no error commit the transaction
                            If Not (isErrorinTransaction) Then
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    sqlTrans.Commit()
                                    talentSqlAccessDetail.EndTransaction(sqlTrans)
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, "Successfully Added Product With Options")
                                    returnMessage = "SUCCESS"
                                End If
                            End If
                        Else
                            'transaction object not able to create error

                        End If 'begin transaction error checking if ends
                    Else
                        'failed to get log header id inform user

                    End If ' logHeaderId checking if ends
                Else
                    returnMessage = "Functionality is busy, Please try again."
                End If ' functionality is active if ends
            Catch ex As Exception
                loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting master product records to " & sourceClassName)
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return returnMessage
        End Function

        Public Function AddProductWithoutOptions(ByVal businessUnit As String, ByVal partner As String, _
                                   ByVal masterProductCode As String, _
                                   ByVal deStockProductEntity As DEStockProduct, _
                                   ByVal deProductPriceEntity As DEProductPrice, _
                                   ByVal deProductDescriptionEntity As DEProductDescriptions) As String
            Dim returnMessage As String = String.Empty


            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlTrans As SqlTransaction
            Dim affectedRows As Integer
            Dim logContent As String = String.Empty
            Dim sourceClassName As String = String.Empty
            Dim SOURCEMETHOD As String = SOURCEMETHOD_ADDPRODUCT
            Dim logHeaderId As Integer = 0

            'Logging Instance
            _settings.Cacheing = False
            Dim loggingSettings As New Logging(_settings)

            Try
                talentSqlAccessDetail.Settings = _settings

                'check functionality is active or not
                If Not (loggingSettings.TblLogHeader.IsActive(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS)) Then


                    Dim isErrorinTransaction As Boolean = False

                    Dim additionalDetails As String = "Adding Product without options" & _
                        " BusinessUnit : " & businessUnit & _
                        " Partner : " & partner & _
                        " Master Product Code : " & masterProductCode

                    affectedRows = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

                    Me.TblProduct.ProductDescEntity = deProductDescriptionEntity

                    'Have You Finished all kinds validation and initialise any common settings
                    'make sure everything ready so that under transaction just open and execute
                    'transaction starts

                    If (logHeaderId > 0) Then

                        'create and get the transaction object
                        sqlTrans = talentSqlAccessDetail.BeginTransaction(err)

                        'no error then execute the statements using transaction object
                        If (Not (err.HasError)) Then

                            'tbl_product_stock
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_stock"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProductStock.Insert(partner, deStockProductEntity, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Insert", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If

                            'tbl_price_list_detail
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_price_list_detail"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblPriceListDetail.Insert(partner, deProductPriceEntity, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "InsertMultiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If

                            'This table is gateway for product so do the action always in the last
                            'tbl_product
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.TblProduct.Insert(masterProductCode, True, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Insert", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
                                End If
                            End If
                            'no error commit the transaction
                            If Not (isErrorinTransaction) Then
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    sqlTrans.Commit()
                                    talentSqlAccessDetail.EndTransaction(sqlTrans)
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, "Successfully Added Product Without Options")
                                    returnMessage = "SUCCESS"
                                End If
                            End If
                        Else
                            'transaction object not able to create error

                        End If 'begin transaction error checking if ends
                    Else
                        'failed to get log header id inform user

                    End If ' logHeaderId checking if ends
                Else
                    returnMessage = "Functionality is busy, Please try again."
                End If ' functionality is active if ends
            Catch ex As Exception
                loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while inserting records to " & sourceClassName)
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return returnMessage
        End Function

        Public Function GetBCTDetails(ByVal groups As List(Of String), ByVal pageCode As String, ByVal partner As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataRow
            Dim outputDataTable As DataTable = Nothing
            Dim outputDataRow As DataRow = Nothing
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetBCTDetails")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim strSelect As New StringBuilder

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String



            Try
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                'Add where clause
                Dim browseNumber As Integer = 0
                With strSelect
                    Select Case pageCode
                        Case "browse02.aspx"
                            browseNumber = 1
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_01 WITH (NOLOCK)  WHERE ")
                            .Append("GROUP_L01_L01_GROUP = @GROUP1")
                        Case "browse03.aspx"
                            browseNumber = 2
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_02 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L02_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L02_L02_GROUP = @GROUP2")
                        Case "browse04.aspx"
                            browseNumber = 3
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_03 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L03_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L03_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L03_L03_GROUP = @GROUP3")
                        Case "browse05.aspx"
                            browseNumber = 4
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_04 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L04_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L04_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L04_L03_GROUP = @GROUP3 ")
                            .Append("AND GROUP_L04_L04_GROUP = @GROUP4")
                        Case "browse06.aspx"
                            browseNumber = 5
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_05 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L05_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L05_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L05_L03_GROUP = @GROUP3 ")
                            .Append("AND GROUP_L05_L04_GROUP = @GROUP4 ")
                            .Append("AND GROUP_L05_L05_GROUP = @GROUP5")
                        Case "browse07.aspx"
                            browseNumber = 6
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_06 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L06_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L06_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L06_L03_GROUP = @GROUP3 ")
                            .Append("AND GROUP_L06_L04_GROUP = @GROUP4 ")
                            .Append("AND GROUP_L06_L05_GROUP = @GROUP5 ")
                            .Append("AND GROUP_L06_L06_GROUP = @GROUP6")
                        Case "browse08.aspx"
                            browseNumber = 7
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_07 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L07_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L07_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L07_L03_GROUP = @GROUP3 ")
                            .Append("AND GROUP_L07_L04_GROUP = @GROUP4 ")
                            .Append("AND GROUP_L07_L05_GROUP = @GROUP5 ")
                            .Append("AND GROUP_L07_L06_GROUP = @GROUP6 ")
                            .Append("AND GROUP_L07_L07_GROUP = @GROUP7")
                        Case "browse09.aspx"
                            browseNumber = 8
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_08 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L08_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L08_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L08_L03_GROUP = @GROUP3 ")
                            .Append("AND GROUP_L08_L04_GROUP = @GROUP4 ")
                            .Append("AND GROUP_L08_L05_GROUP = @GROUP5 ")
                            .Append("AND GROUP_L08_L06_GROUP = @GROUP6 ")
                            .Append("AND GROUP_L08_L07_GROUP = @GROUP7 ")
                            .Append("AND GROUP_L08_L08_GROUP = @GROUP8")
                        Case "browse10.aspx"
                            browseNumber = 9
                            .Append("SELECT * FROM TBL_GROUP_LEVEL_09 WITH (NOLOCK)  WHERE ")
                            .Append("AND GROUP_L09_L01_GROUP = @GROUP1 ")
                            .Append("AND GROUP_L09_L02_GROUP = @GROUP2 ")
                            .Append("AND GROUP_L09_L03_GROUP = @GROUP3 ")
                            .Append("AND GROUP_L09_L04_GROUP = @GROUP4 ")
                            .Append("AND GROUP_L09_L05_GROUP = @GROUP5 ")
                            .Append("AND GROUP_L09_L06_GROUP = @GROUP6 ")
                            .Append("AND GROUP_L09_L07_GROUP = @GROUP7 ")
                            .Append("AND GROUP_L09_L08_GROUP = @GROUP8 ")
                            .Append("AND GROUP_L09_L09_GROUP = @GROUP9")
                    End Select
                End With

                whereClauseFetchHierarchy(0) = " AND GROUP_L0" & browseNumber & "_BUSINESS_UNIT=@BUSINESS_UNIT AND GROUP_L0" & browseNumber & "_PARTNER=@PARTNER"
                cacheKeyHierarchyBased(0) = ToUpper(_settings.BusinessUnit) & ToUpper(_settings.Partner)

                whereClauseFetchHierarchy(1) = " AND GROUP_L0" & browseNumber & "_BUSINESS_UNIT=@BUSINESS_UNIT AND GROUP_L0" & browseNumber & "_PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
                cacheKeyHierarchyBased(1) = ToUpper(_settings.BusinessUnit) & ToUpper(Utilities.GetAllString)

                whereClauseFetchHierarchy(2) = " AND GROUP_L0" & browseNumber & "_BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND GROUP_L0" & browseNumber & "_PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
                cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

                For Each groupItem As String In groups
                    If Not String.IsNullOrEmpty(groupItem) Then
                        Dim index As Integer = groups.IndexOf(groupItem) + 1
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROUP" & index, groupItem))
                    Else
                        Exit For
                    End If
                Next
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = strSelect.ToString & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
                            outputDataRow = outputDataTable.Rows(0)
                            Exit For
                        End If
                    Else
                        outputDataRow = Nothing
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try
            Return outputDataRow
        End Function

        Public Function GetBCTGroupDescription(ByVal group As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetBCTGroupDescription")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim description As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@group_name", group))
                Dim sqlStatement As String = "SELECT * FROM tbl_group WHERE group_name = @group_name"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                    If outputDataTable.Rows.Count > 0 Then
                        description = outputDataTable.Rows(0).Item("GROUP_DESCRIPTION_1")
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return description

        End Function

        Public Function GetGroupProducts(ByVal groups As List(Of String), ByVal queryString As NameValueCollection, ByVal partner As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataRow
            Dim outputDataTable As DataTable = Nothing
            Dim outputDataRow As DataRow = Nothing
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetGroupProducts")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = " AND GP.[GROUP_BUSINESS_UNIT]=@BUSINESS_UNIT AND GP.[GROUP_PARTNER]=@PARTNER"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = " AND GP.[GROUP_BUSINESS_UNIT]=@BUSINESS_UNIT AND GP.[GROUP_PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = " AND GP.[GROUP_BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND GP.[GROUP_PARTNER]='" & _
                ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Dim whereString As New StringBuilder
            With whereString
                For value As Integer = 1 To 10
                    Dim groupText As String = queryString.Item("group" & value)
                    If Not groupText Is Nothing Then
                        If Not groupText.Equals("*EMPTY") AndAlso Not String.IsNullOrEmpty(groupText) Then
                            If value = 1 Then
                                .Append(" GP.GROUP_L0" & value & "_GROUP = @GROUP" & value)
                            Else
                                .Append(" AND GP.GROUP_L0" & value & "_GROUP = @GROUP" & value)
                            End If

                        Else
                            Exit For
                        End If
                    End If
                Next
                If Not queryString.Item("product") Is Nothing Then
                    .Append(" AND GP.PRODUCT = @PRODUCT")
                End If
            End With

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                For Each groupItem As String In groups
                    If Not String.IsNullOrEmpty(groupItem) Then
                        Dim index As Integer = groups.IndexOf(groupItem) + 1
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GROUP" & index, groupItem))
                    Else
                        Exit For
                    End If
                Next
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PRODUCT", queryString.Item("product")))
                Dim sqlStatement As String = "SELECT * FROM TBL_GROUP_PRODUCT AS GP WITH (NOLOCK)  INNER JOIN TBL_PRODUCT AS P WITH (NOLOCK)  ON GP.PRODUCT = P.PRODUCT_CODE WHERE"
                sqlStatement = sqlStatement & whereString.ToString()
                'Execute
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count > 0 Then
                            outputDataRow = outputDataTable.Rows(0)
                            Exit For
                        End If
                    Else
                        outputDataRow = Nothing
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataRow


        End Function

        Public Function RemoveProductWithOptions(ByVal businessUnit As String, ByVal partner As String, ByVal masterProductCode As String) As String
            Dim returnMessage As String = String.Empty


            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlTrans As SqlTransaction
            Dim affectedRows As Integer
            Dim logContent As String = String.Empty
            Dim sourceClassName As String = String.Empty
            Dim SOURCEMETHOD As String = SOURCEMETHOD_REMOVE_PRODUCT_WITH_OPTION
            Dim logHeaderId As Integer = 0
            Dim loggingSettings As New Logging(_settings)
            Dim productSettings As New Products(_settings)
            Try
                _settings.Cacheing = False
                talentSqlAccessDetail.Settings = _settings
                'check functionality is active or not
                If Not (loggingSettings.TblLogHeader.IsActive(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS)) Then
                    Dim isErrorinTransaction As Boolean = False

                    Dim additionalDetails As String = "Removing Product with options " & _
                        " BusinessUnit : " & businessUnit & _
                        " Partner : " & partner & _
                        " Master Product Code : " & masterProductCode

                    affectedRows = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

                    If (logHeaderId > 0) Then

                        sqlTrans = talentSqlAccessDetail.BeginTransaction(err)

                        If (Not (err.HasError)) Then

                            'deletes options from tbl_product_stock
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_stock"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.RemoveOptionsFromProductStockTable(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleting records to " & sourceClassName)
                                End If
                            End If

                            'deletes master from tbl_product_stock
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_stock"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProductStock.RemoveProductStockByProductCode(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleting records to " & sourceClassName)
                                End If
                            End If

                            'deletes options tbl_price_list_detail
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_price_list_detail"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.RemoveOptionsFromPriceListTable(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleting records to " & sourceClassName)
                                End If
                            End If

                            'deletes master tbl_price_list_detail
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_price_list_detail"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblPriceListDetail.RemovePriceDetailByProductCode(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleting records to " & sourceClassName)
                                End If
                            End If

                            'Deletes options from tbl_product 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.RemoveProductOptionsFromProductTable(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes master product from tbl_product 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProduct.RemoveProductByProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes product options products from tbl_product_relations 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_relations"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.RemoveRelationsByProductOptionsProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes product options related products from tbl_product_relations 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_relations"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = Me.RemoveRelationsByProductOptionsRelatedProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes product from tbl_product_relations 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_relations"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProductRelations.RemoveRelationsByProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes from tbl_product_options
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_options"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProductOptions.RemoveProductAndOptions(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes from tbl_product_option_defaults
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_option_defaults"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProductOptionDefaults.RemoveByMasterProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'no error commit the transaction
                            If Not (isErrorinTransaction) Then
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    sqlTrans.Commit()
                                    talentSqlAccessDetail.EndTransaction(sqlTrans)
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, "Successfully Deleted Product With Options")
                                    returnMessage = "SUCCESS"
                                End If
                            End If
                        End If
                    End If
                Else
                    returnMessage = "Functionality is busy, Please try again."
                End If ' functionality is active if ends
            Catch ex As Exception
                loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleteing product records to " & sourceClassName)
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return returnMessage
        End Function

        Public Function RemoveProductWithoutOptions(ByVal businessUnit As String, ByVal partner As String, ByVal masterProductCode As String) As String
            Dim returnMessage As String = String.Empty


            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlTrans As SqlTransaction
            Dim affectedRows As Integer
            Dim logContent As String = String.Empty
            Dim sourceClassName As String = String.Empty
            Dim SOURCEMETHOD As String = SOURCEMETHOD_REMOVE_PRODUCT_WITHOUT_OPTION
            Dim logHeaderId As Integer = 0
            Dim loggingSettings As New Logging(_settings)
            Dim productSettings As New Products(_settings)
            Try
                _settings.Cacheing = False
                talentSqlAccessDetail.Settings = _settings
                'check functionality is active or not
                If Not (loggingSettings.TblLogHeader.IsActive(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS)) Then
                    Dim isErrorinTransaction As Boolean = False

                    Dim additionalDetails As String = "Removing Product with options " & _
                        " BusinessUnit : " & businessUnit & _
                        " Partner : " & partner & _
                        " Master Product Code : " & masterProductCode

                    affectedRows = loggingSettings.TblLogHeader.Create(SOURCEAPPLICATION, SOURCECLASS, SOURCEMETHOD, ActivityStatusEnum.INPROGRESS, additionalDetails, logHeaderId)

                    If (logHeaderId > 0) Then

                        sqlTrans = talentSqlAccessDetail.BeginTransaction(err)

                        If (Not (err.HasError)) Then

                            'deletes master from tbl_product_stock
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_stock"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProductStock.RemoveProductStockByProductCode(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleting records to " & sourceClassName)
                                End If
                            End If

                            'deletes master tbl_price_list_detail
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_price_list_detail"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblPriceListDetail.RemovePriceDetailByProductCode(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows inserted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleting records to " & sourceClassName)
                                End If
                            End If

                            'Deletes product from tbl_product_relations 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product_relations"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProductRelations.RemoveRelationsByProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'Deletes master product from tbl_product 
                            If Not (isErrorinTransaction) Then
                                sourceClassName = "tbl_product"
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    affectedRows = productSettings.TblProduct.RemoveProductByProduct(masterProductCode, sqlTrans)
                                End If
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    logContent = "Number of rows deleted: " & affectedRows
                                    affectedRows = loggingSettings.TblLogs.WriteLog(logHeaderId, sourceClassName, "Delete Multiple", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, logContent, sqlTrans)
                                Else
                                    isErrorinTransaction = True
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while removing records from " & sourceClassName)

                                End If
                            End If

                            'no error commit the transaction
                            If Not (isErrorinTransaction) Then
                                If (Not (sqlTrans.Connection Is Nothing)) Then
                                    sqlTrans.Commit()
                                    talentSqlAccessDetail.EndTransaction(sqlTrans)
                                    affectedRows = loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.SUCCESS, "Successfully Deleted Product With Options")
                                    returnMessage = "SUCCESS"
                                End If
                            End If
                        End If
                    End If
                Else
                    returnMessage = "Functionality is busy, Please try again."
                End If ' functionality is active if ends
            Catch ex As Exception
                loggingSettings.TblLogHeader.Update(logHeaderId, ActivityStatusEnum.FAILED, "Failed while deleteing product records to " & sourceClassName)
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return returnMessage
        End Function
        ''' <summary>
        ''' Removes product options from product table
        ''' </summary>
        ''' <param name="masterProductCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveProductOptionsFromProductTable(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE p FROM tbl_product p " & _
                                                                    "INNER JOIN tbl_product_options op  ON op.PRODUCT_CODE = p.PRODUCT_CODE " & _
                                                                    "WHERE op.MASTER_PRODUCT=@MasterProductCode"

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
        ''' Removes product options from stock table
        ''' </summary>
        ''' <param name="masterProductCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveOptionsFromProductStockTable(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE ps FROM tbl_product_stock ps " & _
                                                                    "INNER JOIN tbl_product_options op  ON op.PRODUCT_CODE = ps.PRODUCT " & _
                                                                    "WHERE op.MASTER_PRODUCT=@MasterProductCode"

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
        ''' Removes options from price list table
        ''' </summary>
        ''' <param name="masterProductCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RemoveOptionsFromPriceListTable(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE pl FROM tbl_price_list_detail pl " & _
                                                                    "INNER JOIN tbl_product_options op  ON op.PRODUCT_CODE = pl.PRODUCT " & _
                                                                    "WHERE op.MASTER_PRODUCT=@MasterProductCode"

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
        ''' Removes relations by product relation from tbl_product_options master product -
        ''' </summary>
        ''' <param name="masterProductCode">Product options master product code</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function RemoveRelationsByProductOptionsRelatedProduct(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As String
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE pr FROM tbl_product_relations pr " & _
                                                                    "INNER JOIN tbl_product_options op  ON op.PRODUCT_CODE = pr.RELATED_PRODUCT " & _
                                                                    "WHERE op.MASTER_PRODUCT = @ProductCode"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", masterProductCode))
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
        ''' Deletes relations by product relation from tbl_product_options master product -
        ''' </summary>
        ''' <param name="masterProductCode">Product options master product code</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function RemoveRelationsByProductOptionsProduct(ByVal masterProductCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As String
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE pr FROM tbl_product_relations pr " & _
                                                                    "INNER JOIN tbl_product_options op  ON op.PRODUCT_CODE = pr.PRODUCT " & _
                                                                    "WHERE op.MASTER_PRODUCT = @ProductCode"

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", masterProductCode))
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
    End Class
End Namespace