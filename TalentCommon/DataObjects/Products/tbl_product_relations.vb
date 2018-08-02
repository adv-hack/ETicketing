Imports System.Data.SqlClient
Imports System.Text
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_product_relations
    ''' </summary>
    <Serializable()> _
    Public Class tbl_product_relations
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product_relations"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_product_relations" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Determine if the given product code for the business unit and partner has products linked to it. 
        ''' Always assume the product code will be unique to product type and subtype, therefore these are not checked here.
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="productCode">The product code linking from</param>
        ''' <param name="priceCode">The price code linking from</param>
        ''' <param name="productSubType">The product sub type</param>
        ''' <param name="cacheing">Is cacheing enabled, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time, default 30</param>
        ''' <returns>True if the given product code has related products</returns>
        ''' <remarks></remarks>
        Public Function DoesProductHaveType2RelatedProducts(ByVal businessUnit As String, ByVal partner As String, ByVal productCode As String, ByVal priceCode As String, ByVal productSubType As String, _
                                                       Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "DoesProductHaveType2RelatedProducts")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner) & ToUpper(productCode) & ToUpper(priceCode) & ToUpper(productSubType)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(productCode) & ToUpper(priceCode) & ToUpper(productSubType)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner) & ToUpper(productCode) & ToUpper(priceCode) & ToUpper(productSubType)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(productCode) & ToUpper(priceCode) & ToUpper(productSubType)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductSubType", productSubType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceCode", priceCode))

                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT * FROM [tbl_product_relations] WHERE [LINK_TYPE] <> 1 AND [LINK_TYPE] <> 3 ")
                If productSubType.Length > 0 Then
                    'when product sub type is available, match on sub type and where product code matches or is blank
                    sqlStatement.Append("AND ([PRODUCT]=@ProductCode OR [PRODUCT]='') AND [TICKETING_PRODUCT_SUB_TYPE]=@ProductSubType ")
                Else
                    'select where product code matches only
                    sqlStatement.Append("AND [PRODUCT]=@ProductCode ")
                End If
                sqlStatement.Append("AND ([TICKETING_PRODUCT_PRICE_CODE]=@PriceCode OR [TICKETING_PRODUCT_PRICE_CODE]='') AND ")

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                            outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
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
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get all the data from tbl_product_relations by the given business unit and partner
        ''' </summary>
        ''' <param name="businessUnit">The given business unit as a string</param>
        ''' <param name="partner">The given partner as a string</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllProductRelationsByBUAndPartner(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, _
                                                             Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllProductRelationsByBUAndPartner")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

                Dim sqlStatement As String = "SELECT * FROM [tbl_product_relations] WHERE "
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
        ''' Get all the data from tbl_product_relations by the given id
        ''' </summary>
        ''' <param name="linkId">The unique id to retrieve the product link from</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllProductRelationsById(ByVal linkId As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllProductRelationsById")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", linkId))

                Dim sqlStatement As String = "SELECT * FROM [tbl_product_relations] WHERE [PRODUCT_RELATIONS_ID]=@ID"
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
        ''' Get the product codes with descriptions by business unit and partner that are listed in tbl_product_relations
        ''' </summary>
        ''' <param name="businessUnit">The given business unit as a string</param>
        ''' <param name="partner">The given partner as a string</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetProductDescriptionsByBUAndPartner(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, _
                                                             Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetProductDescriptionsByBUAndPartner")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

                Dim sqlStatement1, sqlStatement2 As New StringBuilder
                sqlStatement1.Append("SELECT PRODUCT_CODE, PRODUCT_DESCRIPTION_1, PRODUCT_DESCRIPTION_2 FROM [tbl_product] ")
                sqlStatement1.Append("WHERE PRODUCT_CODE IN (SELECT PRODUCT FROM [tbl_product_relations] WHERE ")
                sqlStatement2.Append(") OR PRODUCT_CODE IN (SELECT RELATED_PRODUCT FROM [tbl_product_relations] WHERE ")
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    Dim fullSqlStatement As New StringBuilder
                    fullSqlStatement.Append(sqlStatement1.ToString())
                    fullSqlStatement.Append(whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter))
                    fullSqlStatement.Append(sqlStatement2.ToString())
                    fullSqlStatement.Append(whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter))
                    fullSqlStatement.Append(")")
                    talentSqlAccessDetail.CommandElements.CommandText = fullSqlStatement.ToString()
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
        ''' Get all product relations by product information, business unit and partner
        ''' </summary>
        ''' <param name="businessUnit">Business unit string</param>
        ''' <param name="partner">Partner string</param>
        ''' <param name="productCode">Product code relating from string</param>
        ''' <param name="productType">The product type relating from</param>
        ''' <param name="productSubType">The product sub type relating from</param>
        ''' <param name="cacheing">Boolean value to represent caching, default true</param>
        ''' <param name="cacheTimeMinutes">Integer value to represent the cache time in mins, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllProductRelationsByBUPartnerAndProductInfo(ByVal businessUnit As String, ByVal partner As String, ByVal productCode As String, ByVal productType As String, _
                                                                   ByVal productSubType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllProductRelationsByBUPartnerAndProductInfo")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = productCode & ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = productCode & ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = productCode & ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = productCode & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", productType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", productSubType))
                Dim sqlStatement1 As String = "SELECT * FROM [tbl_product_relations] WHERE ("
                Dim sqlStatement2 As New StringBuilder
                sqlStatement2.Append(") AND (")
                If Not String.IsNullOrEmpty(productCode) Then sqlStatement2.Append("PRODUCT='' AND ")
                sqlStatement2.Append("TICKETING_PRODUCT_TYPE=@TicketingProductType ")
                sqlStatement2.Append("AND TICKETING_PRODUCT_SUB_TYPE=@TicketingProductSubType ")
                If Not String.IsNullOrEmpty(productCode) Then sqlStatement2.Append("OR PRODUCT=@ProductCode")
                sqlStatement2.Append(")")

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & sqlStatement2.ToString()
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
        ''' Get type 1 product relations by product information, business unit and partner
        ''' </summary>
        ''' <param name="businessUnit">Business unit string</param>
        ''' <param name="partner">Partner string</param>
        ''' <param name="productCode">Product code relating from string</param>
        ''' <param name="productType">The product type relating from</param>
        ''' <param name="productSubType">The product sub type relating from</param>
        ''' <param name="cacheing">Boolean value to represent caching, default true</param>
        ''' <param name="cacheTimeMinutes">Integer value to represent the cache time in mins, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetType1ProductRelationsByBUPartnerAndProductInfo(ByVal businessUnit As String, ByVal partner As String, ByVal productCode As String, ByVal productType As String, _
                                                                   ByVal productSubType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetType1ProductRelationsByBUPartnerAndProductInfo")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = productCode & ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = productCode & ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = productCode & ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = productCode & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", productType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", productSubType))
                Dim sqlStatement1 As String = "SELECT * FROM [tbl_product_relations] WHERE [LINK_TYPE] <> 2 AND [LINK_TYPE] <> 3 AND ("
                Dim sqlStatement2 As New StringBuilder
                sqlStatement2.Append(") AND (")
                If Not String.IsNullOrEmpty(productCode) Then sqlStatement2.Append("PRODUCT='' AND ")
                sqlStatement2.Append("TICKETING_PRODUCT_TYPE=@TicketingProductType ")
                sqlStatement2.Append("AND TICKETING_PRODUCT_SUB_TYPE=@TicketingProductSubType ")
                If Not String.IsNullOrEmpty(productCode) Then sqlStatement2.Append("OR PRODUCT=@ProductCode")
                sqlStatement2.Append(")")

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & sqlStatement2.ToString()
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
        ''' Get type 2 product relations by product code, price code, business unit and partner
        ''' </summary>
        ''' <param name="businessUnit">Business unit string</param>
        ''' <param name="partner">Partner string</param>
        ''' <param name="productCode">Product code relating from string</param>
        ''' <param name="priceCode">Price code relating from string</param>
        ''' <param name="productSubType">Product Sub Type relating from string</param>
        ''' <param name="cacheing">Boolean value to represent caching, default true</param>
        ''' <param name="cacheTimeMinutes">Integer value to represent the cache time in mins, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetType2ProductRelationsByBUPartnerAndProductCode(ByVal businessUnit As String, ByVal partner As String, ByVal productCode As String, ByVal priceCode As String, ByVal productSubType As String, _
                                                                          Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetType2ProductRelationsByBUPartnerAndProductCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = productCode & priceCode & ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = productCode & priceCode & ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = productCode & priceCode & ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = productCode & priceCode & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceCode", priceCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductSubType", productSubType))

                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT * FROM [tbl_product_relations] WHERE [LINK_TYPE] <> 1 AND [LINK_TYPE] <> 3 ")
                If productSubType.Length > 0 Then
                    'when product sub type is available, match on sub type and where product code matches or is blank
                    sqlStatement.Append("AND ([PRODUCT]=@ProductCode OR [PRODUCT]='') AND [TICKETING_PRODUCT_SUB_TYPE]=@ProductSubType ")
                Else
                    'select where product code matches only
                    sqlStatement.Append("AND [PRODUCT]=@ProductCode ")
                End If
                sqlStatement.Append("AND ([TICKETING_PRODUCT_PRICE_CODE]=@PriceCode OR [TICKETING_PRODUCT_PRICE_CODE]='') AND ")

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
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
        ''' Deletes a product relation from tbl_product_relations
        ''' </summary>
        ''' <param name="productRelationsId">The given product relations ID that has been selected for deletion</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function DeleteProductRelation(ByVal productRelationsId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_product_relations] WHERE [PRODUCT_RELATIONS_ID]=@ProductRelationsId"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductRelationsId", productRelationsId))

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
        Public Function DeleteProductRelationAllBU(ByVal productRelationsId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim productRelationsDetails As DataTable = GetProductRelationDetailsByRelationshipID(productRelationsId)
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim masterProduct As String = productRelationsDetails.Rows(0).Item("PRODUCT")
            Dim relatedProduct As String = productRelationsDetails.Rows(0).Item("RELATED_PRODUCT")
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_product_relations] WHERE [PRODUCT]=@MasterProduct AND [RELATED_PRODUCT]=@Product AND [LINK_TYPE] = '3'"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MasterProduct", masterProduct))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", relatedProduct))
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
        Public Function DeleteProductRelationByForiegnId(ByVal foriegnRelationId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_product_relations] WHERE [FOREIGN_PRODUCT_RELATIONS_ID]=@ForiegnRelationID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ForiegnRelationID", foriegnRelationId))
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
        ''' Create a link type 1 entry into tbl_product_relations based on the given product details
        ''' </summary>
        ''' <param name="businessUnit">The given Business Unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="productCode">The product code linking from</param>
        ''' <param name="ticketingProductType">The ticketing product type linking from</param>
        ''' <param name="ticketingProductSubType">The ticketing product sub type linking from</param>
        ''' <param name="relatedProductCode">The product code linking to</param>
        ''' <param name="relatedTicketingProductType">The ticketing product type linking to</param>
        ''' <param name="relatedTicketingProductSubType">The ticketing product sub type linking to</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The rows affected</returns>
        ''' <remarks></remarks>
        Public Function CreateType1ProductRelation(ByVal businessUnit As String, ByVal partner As String, ByVal productCode As String, ByVal ticketingProductType As String, _
                                              ByVal ticketingProductSubType As String, ByVal relatedProductCode As String, ByVal relatedTicketingProductType As String, _
                                              ByVal relatedTicketingProductSubType As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call, check to see the link doesn't already exist
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement1 As New StringBuilder
            sqlStatement1.Append("SELECT COUNT(*) FROM [tbl_product_relations] WHERE [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner AND [PRODUCT]=@Product ")
            sqlStatement1.Append("AND [RELATED_PRODUCT]=@RelatedProduct AND [TICKETING_PRODUCT_TYPE]=@TicketingProductType AND [TICKETING_PRODUCT_SUB_TYPE]=@TicketingProductSubType ")
            sqlStatement1.Append("AND [RELATED_TICKETING_PRODUCT_TYPE]=@RelatedTicketingProductType AND [RELATED_TICKETING_PRODUCT_SUB_TYPE]=@RelatedTicketingProductSubType")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProduct", relatedProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", ticketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", ticketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductType", relatedTicketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductSubType", relatedTicketingProductSubType))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            If affectedRows = 0 Then
                Dim sqlStatement2 As New StringBuilder
                sqlStatement2.Append("INSERT INTO [tbl_product_relations] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[GROUP_L01_GROUP],[GROUP_L02_GROUP],[GROUP_L03_GROUP]")
                sqlStatement2.Append(",[GROUP_L04_GROUP],[GROUP_L05_GROUP],[GROUP_L06_GROUP],[GROUP_L07_GROUP],[GROUP_L08_GROUP],[GROUP_L09_GROUP],[GROUP_L10_GROUP]")
                sqlStatement2.Append(",[PRODUCT],[RELATED_GROUP_L01_GROUP],[RELATED_GROUP_L02_GROUP],[RELATED_GROUP_L03_GROUP],[RELATED_GROUP_L04_GROUP],[RELATED_GROUP_L05_GROUP]")
                sqlStatement2.Append(",[RELATED_GROUP_L06_GROUP],[RELATED_GROUP_L07_GROUP],[RELATED_GROUP_L08_GROUP],[RELATED_GROUP_L09_GROUP],[RELATED_GROUP_L10_GROUP]")
                sqlStatement2.Append(",[RELATED_PRODUCT],[SEQUENCE],[TICKETING_PRODUCT_TYPE],[TICKETING_PRODUCT_SUB_TYPE],[RELATED_TICKETING_PRODUCT_TYPE],[RELATED_TICKETING_PRODUCT_SUB_TYPE]")
                sqlStatement2.Append(") VALUES (")
                sqlStatement2.Append(" '1',@BusinessUnit,@Partner,'*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY'")
                sqlStatement2.Append(",@Product,'*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY'")
                sqlStatement2.Append(",@RelatedProduct,'',@TicketingProductType,@TicketingProductSubType,@RelatedTicketingProductType,@RelatedTicketingProductSubType)")
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement2.ToString()

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
            Else
                affectedRows = -1
            End If

            'Return results
            Return affectedRows
        End Function
        ''' <summary>
        '''  See if the current product relationship exists based on BU, Partner, link type, product code, product type, sub type and price code
        ''' </summary>
        ''' <param name="businessUnit"></param>
        ''' <param name="partner"></param>
        ''' <param name="linkType"></param>
        ''' <param name="productCode"></param>
        ''' <param name="ticketingProductType"></param>
        ''' <param name="ticketingProductSubType"></param>
        ''' <param name="ticketingProductPriceCode"></param>
        ''' <param name="relatedTicketingProductCampaignCode"></param>
        ''' <param name="relatedProductCode"></param>
        ''' <param name="relatedTicketingProductType"></param>
        ''' <param name="relatedTicketingProductSubType"></param>
        ''' <param name="relatedTicketingProductPriceCode"></param>
        ''' <param name="givenTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckForDuplicateRelationships(ByVal businessUnit As String, ByVal partner As String, ByVal linkType As Integer, ByVal productCode As String, _
                                                       ByVal ticketingProductType As String, ByVal ticketingProductSubType As String, ByVal ticketingProductPriceCode As String, _
                                                       ByVal relatedTicketingProductCampaignCode As String, ByVal relatedProductCode As String, ByVal relatedTicketingProductType As String, _
                                                       ByVal relatedTicketingProductSubType As String, ByVal relatedTicketingProductPriceCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement1 As New StringBuilder
            sqlStatement1.Append("SELECT COUNT(*) FROM [tbl_product_relations] WHERE [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner AND [PRODUCT]=@Product ")
            sqlStatement1.Append("AND [RELATED_PRODUCT]=@RelatedProduct AND [TICKETING_PRODUCT_TYPE]=@TicketingProductType AND [TICKETING_PRODUCT_SUB_TYPE]=@TicketingProductSubType ")
            sqlStatement1.Append("AND [RELATED_TICKETING_PRODUCT_TYPE]=@RelatedTicketingProductType AND [RELATED_TICKETING_PRODUCT_SUB_TYPE]=@RelatedTicketingProductSubType ")
            sqlStatement1.Append("AND [LINK_TYPE]=@LinkType AND [TICKETING_PRODUCT_PRICE_CODE]=@TicketingProductPriceCode AND [RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE]=@RelatedTicketingProductCampaignCode")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString()

            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkType", linkType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", ticketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", ticketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductPriceCode", ticketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductCampaignCode", relatedTicketingProductCampaignCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProduct", relatedProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductType", relatedTicketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductSubType", relatedTicketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductPriceCode", relatedTicketingProductPriceCode))

            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            Return affectedRows
        End Function
        ''' <summary>
        ''' Returns number of records for a master product
        ''' </summary>
        ''' <param name="businessUnit"></param>
        ''' <param name="partner"></param>
        ''' <param name="masterProductCode"></param>
        ''' <param name="linkType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetMasterProductCountByMasterProductCode(ByVal businessUnit As String, ByVal partner As String, ByVal masterProductCode As String, ByVal linkType As Integer) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement1 As New StringBuilder
            sqlStatement1.Append("SELECT COUNT(*) FROM [tbl_product_relations] WHERE [PRODUCT]=@Product AND [LINK_TYPE]=@LinkType AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkType", linkType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", masterProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            Return affectedRows
        End Function

        Public Function GetProductRelationDetailsByRelationshipID(ByVal relationshipID As Integer) As DataTable
            Dim results As DataTable = Nothing
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement1 As New StringBuilder
            sqlStatement1.Append("SELECT PRODUCT, RELATED_PRODUCT, LINK_TYPE FROM [tbl_product_relations] WHERE [PRODUCT_RELATIONS_ID]=@relationshipID ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@relationshipID", relationshipID))
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                results = talentSqlAccessDetail.ResultDataSet.Tables(0)
            End If

            Return results

        End Function
        Public Function GetLinkTypeByProductRelationshipID(ByVal relationshipID As Integer) As Integer
            Dim linkType As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement1 As New StringBuilder
            sqlStatement1.Append("SELECT [LINK_TYPE] FROM [tbl_product_relations] WHERE [PRODUCT_RELATIONS_ID]=@relationshipID ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement1.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@relationshipID", relationshipID))
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                linkType = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            Return linkType
        End Function
        ''' <summary>
        ''' Create a link type 2 entry into tbl_product_relations based on the given product details
        ''' </summary>
        ''' <param name="businessUnit">The given Business Unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="linkType">The product link type as 0, 1 or 2</param>
        ''' <param name="productCode">The product code linking from</param>
        ''' <param name="ticketingProductType">The ticketing product type linking from</param>
        ''' <param name="ticketingProductSubType">The ticketing product sub type linking from</param>
        ''' <param name="ticketingProductPriceCode">The ticketing product price code linking from</param>
        ''' <param name="relatedTicketingProductCampaignCode">The related ticketing product campaign code</param>
        ''' <param name="relatedProductCode">The product code linking to</param>
        ''' <param name="relatedTicketingProductType">The ticketing product type linking to</param>
        ''' <param name="relatedTicketingProductSubType">The ticketing product sub type linking to</param>
        ''' <param name="relatedProductMandatory">Is the related product mandatory for purchase</param>
        ''' <param name="relatedTicketingProductPriceCode">The related ticketing product price code</param>
        ''' <param name="relatedTicketingProductStand">The ticketing product stand code linking to</param>
        ''' <param name="relatedTicketingProductStandReadOnly">Is the stand information readonly</param>
        ''' <param name="relatedTicketingProductArea">The ticketing product area code linking to</param>
        ''' <param name="relatedTicketingProductAreaReadOnly">Is the area information readonly</param>
        ''' <param name="relatedTicketingProductQty">The default quantity value for the prduct linking to</param>
        ''' <param name="relatedTicketingProductQtyMin">The minium quantity allowed</param>
        ''' <param name="relatedTicketingProductQtyMax">The maximum quantity allowed</param>
        ''' <param name="relatedTicketingProductQtyReadOnly">Is the quantity box readonly</param>
        ''' <param name="relatedTicketingProductQtyRatio">Is the quantity defined as a ratio</param>
        ''' <param name="relatedTicketingProductQtyRoundUp">Does the quantity as a ratio need to be rounded up</param>
        ''' <param name="relatedCssClass">The related CSS Classname</param>
        ''' <param name="relatedInstructions">The related instructions string</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The rows affected</returns>
        ''' <remarks></remarks>
        Public Function CreateType2OrType3OrBothProductRelation(ByVal businessUnit As String, ByVal partner As String, ByVal linkType As Integer, ByVal productCode As String, ByVal ticketingProductType As String, _
                ByVal ticketingProductSubType As String, ByVal ticketingProductPriceCode As String, ByVal relatedTicketingProductCampaignCode As String, ByVal relatedProductCode As String, ByVal relatedTicketingProductType As String, _
                ByVal relatedTicketingProductSubType As String, ByVal relatedProductMandatory As Boolean, ByVal relatedTicketingProductPriceCode As String, ByVal relatedTicketingProductStand As String, ByVal relatedTicketingProductStandReadOnly As Boolean, _
                ByVal relatedTicketingProductArea As String, ByVal relatedTicketingProductAreaReadOnly As Boolean, ByVal relatedTicketingProductQty As String, _
                ByVal relatedTicketingProductQtyMin As String, ByVal relatedTicketingProductQtyMax As String, ByVal relatedTicketingProductQtyReadOnly As Boolean, _
                ByVal relatedTicketingProductQtyRatio As Boolean, ByVal relatedTicketingProductQtyRoundUp As Boolean, ByVal relatedCssClass As String, ByVal relatedInstructions As String, _
                ByRef foreignProductRelationsID As Integer, ByRef packageComponentValue1 As Decimal, ByRef packageComponentValue2 As Decimal, ByRef packageComponentValue3 As Decimal, ByRef packageComponentValue4 As Decimal, _
                ByRef packageComponentValue5 As Decimal, ByRef packageComponentPriceBands As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.Settings.Cacheing = False
            Dim productRelationsID As Integer = 0
            Dim sqlStatement2 As New StringBuilder
            sqlStatement2.Append("INSERT INTO [tbl_product_relations] ([QUALIFIER],[BUSINESS_UNIT],[PARTNER],[GROUP_L01_GROUP],[GROUP_L02_GROUP],[GROUP_L03_GROUP]")
            sqlStatement2.Append(",[GROUP_L04_GROUP],[GROUP_L05_GROUP],[GROUP_L06_GROUP],[GROUP_L07_GROUP],[GROUP_L08_GROUP],[GROUP_L09_GROUP],[GROUP_L10_GROUP]")
            sqlStatement2.Append(",[PRODUCT],[RELATED_GROUP_L01_GROUP],[RELATED_GROUP_L02_GROUP],[RELATED_GROUP_L03_GROUP],[RELATED_GROUP_L04_GROUP],[RELATED_GROUP_L05_GROUP]")
            sqlStatement2.Append(",[RELATED_GROUP_L06_GROUP],[RELATED_GROUP_L07_GROUP],[RELATED_GROUP_L08_GROUP],[RELATED_GROUP_L09_GROUP],[RELATED_GROUP_L10_GROUP]")
            sqlStatement2.Append(",[RELATED_PRODUCT],[SEQUENCE],[TICKETING_PRODUCT_TYPE],[TICKETING_PRODUCT_SUB_TYPE],[RELATED_TICKETING_PRODUCT_TYPE],[RELATED_TICKETING_PRODUCT_SUB_TYPE]")
            sqlStatement2.Append(",[LINK_TYPE],[TICKETING_PRODUCT_PRICE_CODE],[RELATED_PRODUCT_MANDATORY]")
            sqlStatement2.Append(",[RELATED_TICKETING_PRODUCT_STAND],[RELATED_TICKETING_PRODUCT_STAND_READONLY],[RELATED_TICKETING_PRODUCT_AREA],[RELATED_TICKETING_PRODUCT_AREA_READONLY]")
            If relatedTicketingProductQty.Length > 0 Then sqlStatement2.Append(",[RELATED_TICKETING_PRODUCT_QTY]")
            If relatedTicketingProductQtyMin.Length > 0 Then sqlStatement2.Append(",[RELATED_TICKETING_PRODUCT_QTY_MIN]")
            If relatedTicketingProductQtyMax.Length > 0 Then sqlStatement2.Append(",[RELATED_TICKETING_PRODUCT_QTY_MAX]")
            sqlStatement2.Append(",[RELATED_TICKETING_PRODUCT_QTY_READONLY],[RELATED_TICKETING_PRODUCT_QTY_RATIO],[RELATED_TICKETING_PRODUCT_QTY_ROUND_UP]")
            sqlStatement2.Append(",[RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE],[RELATED_TICKETING_PRODUCT_PRICE_CODE],[RELATED_CSS_CLASS],[RELATED_INSTRUCTIONS], [FOREIGN_PRODUCT_RELATIONS_ID], [PACKAGE_COMPONENT_VALUE_01] ")
            sqlStatement2.Append(",[PACKAGE_COMPONENT_VALUE_02], [PACKAGE_COMPONENT_VALUE_03], [PACKAGE_COMPONENT_VALUE_04], [PACKAGE_COMPONENT_VALUE_05], [PACKAGE_COMPONENT_PRICE_BANDS] ")
            sqlStatement2.Append(") VALUES (")
            sqlStatement2.Append(" '1',@BusinessUnit,@Partner,'*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY'")
            sqlStatement2.Append(",@Product,'*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY','*EMPTY'")
            sqlStatement2.Append(",@RelatedProduct,'',@TicketingProductType,@TicketingProductSubType,@RelatedTicketingProductType,@RelatedTicketingProductSubType")
            sqlStatement2.Append(",@LinkType,@TicketingProductPriceCode,@RelatedProductMandatory")
            sqlStatement2.Append(",@RelatedTicketingProductStand,@RelatedTicketingProductStandReadOnly,@RelatedTicketingProductArea,@RelatedTicketingProductAreaReadOnly")
            If relatedTicketingProductQty.Length > 0 Then sqlStatement2.Append(",@RelatedTicketingProductQty")
            If relatedTicketingProductQtyMin.Length > 0 Then sqlStatement2.Append(",@RelatedTicketingProductQtyMin")
            If relatedTicketingProductQtyMax.Length > 0 Then sqlStatement2.Append(",@RelatedTicketingProductQtyMax")
            sqlStatement2.Append(",@RelatedTicketingProductQtyReadOnly,@RelatedTicketingProductQtyRatio,@RelatedTicketingProductQtyRoundUp")
            sqlStatement2.Append(",@RelatedTicketingProductCampaignCode,@RelatedTicketingProductPriceCode,@RelatedCssClass,@RelatedInstructions,@ForeignProductRelationsID,@PackageComponentValue1,@PackageComponentValue2,@PackageComponentValue3,@PackageComponentValue4,@PackageComponentValue5,@PackageComponentPriceBands")
            sqlStatement2.Append(") SELECT SCOPE_IDENTITY()")

            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement2.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProductMandatory", relatedProductMandatory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductStand", relatedTicketingProductStand))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductStandReadOnly", relatedTicketingProductStandReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductArea", relatedTicketingProductArea))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductAreaReadOnly", relatedTicketingProductAreaReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQty", relatedTicketingProductQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyMin", relatedTicketingProductQtyMin))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyMax", relatedTicketingProductQtyMax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyReadOnly", relatedTicketingProductQtyReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyRatio", relatedTicketingProductQtyRatio))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyRoundUp", relatedTicketingProductQtyRoundUp))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedCssClass", relatedCssClass))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedInstructions", relatedInstructions))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ForeignProductRelationsID", foreignProductRelationsID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkType", linkType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", ticketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", ticketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductPriceCode", ticketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductCampaignCode", relatedTicketingProductCampaignCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProduct", relatedProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductType", relatedTicketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductSubType", relatedTicketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductPriceCode", relatedTicketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue1", packageComponentValue1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue2", packageComponentValue2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue3", packageComponentValue3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue4", packageComponentValue4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue5", packageComponentValue5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentPriceBands", packageComponentPriceBands))

            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                productRelationsID = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                productRelationsID = -1
            End If
            Return productRelationsID
        End Function

        ''' <summary>
        ''' Update a product link based on the give link id
        ''' </summary>
        ''' <param name="linkId">The link id we are updating</param>
        ''' <param name="linkType">The product link type as 0, 1 or 2</param>
        ''' <param name="productCode">The product code linking from</param>
        ''' <param name="ticketingProductType">The ticketing product type linking from</param>
        ''' <param name="ticketingProductSubType">The ticketing product sub type linking from</param>
        ''' <param name="ticketingProductPriceCode">The ticketing product price code linking from</param>
        ''' <param name="relatedProductCode">The product code linking to</param>
        ''' <param name="relatedTicketingProductType">The ticketing product type linking to</param>
        ''' <param name="relatedTicketingProductSubType">The ticketing product sub type linking to</param>
        ''' <param name="relatedProductMandatory">Is the related product mandatory for purchase</param>
        ''' <param name="relatedTicketingProductStand">The ticketing product stand code linking to</param>
        ''' <param name="relatedTicketingProductStandReadOnly">Is the stand information readonly</param>
        ''' <param name="relatedTicketingProductArea">The ticketing product area code linking to</param>
        ''' <param name="relatedTicketingProductAreaReadOnly">Is the area information readonly</param>
        ''' <param name="relatedTicketingProductQty">The default quantity value for the prduct linking to</param>
        ''' <param name="relatedTicketingProductQtyMin">The minium quantity allowed</param>
        ''' <param name="relatedTicketingProductQtyMax">The maximum quantity allowed</param>
        ''' <param name="relatedTicketingProductQtyReadOnly">Is the quantity box readonly</param>
        ''' <param name="relatedTicketingProductQtyRatio">Is the quantity defined as a ratio</param>
        ''' <param name="relatedTicketingProductQtyRoundUp">Does the quantity as a ratio need to be rounded up</param>
        ''' <param name="relatedTicketingProductPriceCode">The related product price code value</param>
        ''' <param name="relatedTicketingProductCampaignCode">The campaign code of the product linking to</param>
        ''' <param name="relatedCssClass">The related CSS Classname</param>
        ''' <param name="relatedInstructions">The related instructions string</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The rows affected</returns>
        ''' <remarks></remarks>
        Public Function UpateProductRelationById(ByVal linkId As String, ByVal linkType As Integer, ByVal productCode As String, ByVal ticketingProductType As String, _
                ByVal ticketingProductSubType As String, ByVal ticketingProductPriceCode As String, ByVal relatedTicketingProductCampaignCode As String, ByVal relatedProductCode As String, ByVal relatedTicketingProductType As String, _
                ByVal relatedTicketingProductSubType As String, ByVal relatedProductMandatory As Boolean, ByVal relatedTicketingProductPriceCode As String, ByVal relatedTicketingProductStand As String, ByVal relatedTicketingProductStandReadOnly As Boolean, _
                ByVal relatedTicketingProductArea As String, ByVal relatedTicketingProductAreaReadOnly As Boolean, ByVal relatedTicketingProductQty As String, _
                ByVal relatedTicketingProductQtyMin As String, ByVal relatedTicketingProductQtyMax As String, ByVal relatedTicketingProductQtyReadOnly As Boolean, _
                ByVal relatedTicketingProductQtyRatio As Boolean, ByVal relatedTicketingProductQtyRoundUp As Boolean, ByVal relatedCssClass As String, ByVal relatedInstructions As String, ByVal packageComponentValue1 As Decimal, _
                ByVal packageComponentValue2 As Decimal, ByVal packageComponentValue3 As Decimal, ByVal packageComponentValue4 As Decimal, ByVal packageComponentValue5 As Decimal, ByVal packageComponentPriceBands As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlStatement As New StringBuilder

            sqlStatement.Append("UPDATE [tbl_product_relations] SET [PRODUCT]=@Product, [RELATED_PRODUCT]=@RelatedProduct, [TICKETING_PRODUCT_TYPE]=@TicketingProductType")
            sqlStatement.Append(", [TICKETING_PRODUCT_SUB_TYPE]=@TicketingProductSubType, [RELATED_TICKETING_PRODUCT_TYPE]=@RelatedTicketingProductType")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_SUB_TYPE]=@RelatedTicketingProductSubType, [LINK_TYPE]=@LinkType, [TICKETING_PRODUCT_PRICE_CODE]=@TicketingProductPriceCode")
            sqlStatement.Append(", [RELATED_PRODUCT_MANDATORY]=@RelatedProductMandatory")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_STAND]=@RelatedTicketingProductStand, [RELATED_TICKETING_PRODUCT_STAND_READONLY]=@RelatedTicketingProductStandReadOnly")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_AREA]=@RelatedTicketingProductArea, [RELATED_TICKETING_PRODUCT_AREA_READONLY]=@RelatedTicketingProductAreaReadOnly")
            If relatedTicketingProductQty.Length > 0 Then
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY]=@RelatedTicketingProductQty")
            Else
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY]=NULL")
            End If
            If relatedTicketingProductQtyMin.Length > 0 Then
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MIN]=@RelatedTicketingProductQtyMin")
            Else
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MIN]=NULL")
            End If
            If relatedTicketingProductQtyMax.Length > 0 Then
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MAX]=@RelatedTicketingProductQtyMax")
            Else
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MAX]=NULL")
            End If
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_READONLY]=@RelatedTicketingProductQtyReadOnly")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_RATIO]=@RelatedTicketingProductQtyRatio")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_ROUND_UP]=@RelatedTicketingProductQtyRoundUp")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE]=@RelatedTicketingProductCampaignCode")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_PRICE_CODE]=@RelatedTicketingProductPriceCode")
            sqlStatement.Append(", [RELATED_CSS_CLASS]=@RelatedCssClass")
            sqlStatement.Append(", [RELATED_INSTRUCTIONS]=@RelatedInstructions")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_01]=@PackageComponentValue1")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_02]=@PackageComponentValue2")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_03]=@PackageComponentValue3")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_04]=@PackageComponentValue4")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_05]=@PackageComponentValue5")
            sqlStatement.Append(", [PACKAGE_COMPONENT_PRICE_BANDS]=@PackageComponentPriceBands")
            sqlStatement.Append(" WHERE [PRODUCT_RELATIONS_ID]=@LinkId")

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkId", linkId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkType", linkType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", ticketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", ticketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductPriceCode", ticketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductCampaignCode", relatedTicketingProductCampaignCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProduct", relatedProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductType", relatedTicketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductSubType", relatedTicketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductPriceCode", relatedTicketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProductMandatory", relatedProductMandatory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductStand", relatedTicketingProductStand))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductStandReadOnly", relatedTicketingProductStandReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductArea", relatedTicketingProductArea))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductAreaReadOnly", relatedTicketingProductAreaReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQty", relatedTicketingProductQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyMin", relatedTicketingProductQtyMin))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyMax", relatedTicketingProductQtyMax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyReadOnly", relatedTicketingProductQtyReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyRatio", relatedTicketingProductQtyRatio))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyRoundUp", relatedTicketingProductQtyRoundUp))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedCssClass", relatedCssClass))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedInstructions", relatedInstructions))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue1", packageComponentValue1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue2", packageComponentValue2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue3", packageComponentValue3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue4", packageComponentValue4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue5", packageComponentValue5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentPriceBands", packageComponentPriceBands))

            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            talentSqlAccessDetail = Nothing
            Return affectedRows
        End Function
        ''' <summary>
        ''' Update a product link based on the give link id
        ''' </summary>
        ''' <param name="foreignLinkId">The link id we are updating</param>
        ''' <param name="linkType">The product link type as 0, 1 or 2</param>
        ''' <param name="productCode">The product code linking from</param>
        ''' <param name="ticketingProductType">The ticketing product type linking from</param>
        ''' <param name="ticketingProductSubType">The ticketing product sub type linking from</param>
        ''' <param name="ticketingProductPriceCode">The ticketing product price code linking from</param>
        ''' <param name="relatedProductCode">The product code linking to</param>
        ''' <param name="relatedTicketingProductType">The ticketing product type linking to</param>
        ''' <param name="relatedTicketingProductSubType">The ticketing product sub type linking to</param>
        ''' <param name="relatedProductMandatory">Is the related product mandatory for purchase</param>
        ''' <param name="relatedTicketingProductStand">The ticketing product stand code linking to</param>
        ''' <param name="relatedTicketingProductStandReadOnly">Is the stand information readonly</param>
        ''' <param name="relatedTicketingProductArea">The ticketing product area code linking to</param>
        ''' <param name="relatedTicketingProductAreaReadOnly">Is the area information readonly</param>
        ''' <param name="relatedTicketingProductQty">The default quantity value for the prduct linking to</param>
        ''' <param name="relatedTicketingProductQtyMin">The minium quantity allowed</param>
        ''' <param name="relatedTicketingProductQtyMax">The maximum quantity allowed</param>
        ''' <param name="relatedTicketingProductQtyReadOnly">Is the quantity box readonly</param>
        ''' <param name="relatedTicketingProductQtyRatio">Is the quantity defined as a ratio</param>
        ''' <param name="relatedTicketingProductQtyRoundUp">Does the quantity as a ratio need to be rounded up</param>
        ''' <param name="relatedTicketingProductPriceCode">The related product price code value</param>
        ''' <param name="relatedTicketingProductCampaignCode">The campaign code of the product linking to</param>
        ''' <param name="relatedCssClass">The related CSS Classname</param>
        ''' <param name="relatedInstructions">The related instructions string</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The rows affected</returns>
        ''' <remarks></remarks>
        Public Function UpateProductRelationByForeignId(ByVal foreignLinkId As String, ByVal linkType As Integer, ByVal productCode As String, ByVal ticketingProductType As String, _
                ByVal ticketingProductSubType As String, ByVal ticketingProductPriceCode As String, ByVal relatedTicketingProductCampaignCode As String, ByVal relatedProductCode As String, ByVal relatedTicketingProductType As String, _
                ByVal relatedTicketingProductSubType As String, ByVal relatedProductMandatory As Boolean, ByVal relatedTicketingProductPriceCode As String, ByVal relatedTicketingProductStand As String, ByVal relatedTicketingProductStandReadOnly As Boolean, _
                ByVal relatedTicketingProductArea As String, ByVal relatedTicketingProductAreaReadOnly As Boolean, ByVal relatedTicketingProductQty As String, _
                ByVal relatedTicketingProductQtyMin As String, ByVal relatedTicketingProductQtyMax As String, ByVal relatedTicketingProductQtyReadOnly As Boolean, _
                ByVal relatedTicketingProductQtyRatio As Boolean, ByVal relatedTicketingProductQtyRoundUp As Boolean, ByVal relatedCssClass As String, ByVal relatedInstructions As String, _
                ByVal packageComponentValue1 As Decimal, ByVal packageComponentValue2 As Decimal, ByVal packageComponentValue3 As Decimal, ByVal packageComponentValue4 As Decimal, ByVal packageComponentValue5 As Decimal, ByVal packageComponentPriceBands As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlStatement As New StringBuilder

            sqlStatement.Append("UPDATE [tbl_product_relations] SET [PRODUCT]=@Product, [RELATED_PRODUCT]=@RelatedProduct, [TICKETING_PRODUCT_TYPE]=@TicketingProductType")
            sqlStatement.Append(", [TICKETING_PRODUCT_SUB_TYPE]=@TicketingProductSubType, [RELATED_TICKETING_PRODUCT_TYPE]=@RelatedTicketingProductType")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_SUB_TYPE]=@RelatedTicketingProductSubType, [LINK_TYPE]=@LinkType, [TICKETING_PRODUCT_PRICE_CODE]=@TicketingProductPriceCode")
            sqlStatement.Append(", [RELATED_PRODUCT_MANDATORY]=@RelatedProductMandatory")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_STAND]=@RelatedTicketingProductStand, [RELATED_TICKETING_PRODUCT_STAND_READONLY]=@RelatedTicketingProductStandReadOnly")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_AREA]=@RelatedTicketingProductArea, [RELATED_TICKETING_PRODUCT_AREA_READONLY]=@RelatedTicketingProductAreaReadOnly")
            If relatedTicketingProductQty.Length > 0 Then
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY]=@RelatedTicketingProductQty")
            Else
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY]=NULL")
            End If
            If relatedTicketingProductQtyMin.Length > 0 Then
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MIN]=@RelatedTicketingProductQtyMin")
            Else
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MIN]=NULL")
            End If
            If relatedTicketingProductQtyMax.Length > 0 Then
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MAX]=@RelatedTicketingProductQtyMax")
            Else
                sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_MAX]=NULL")
            End If
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_READONLY]=@RelatedTicketingProductQtyReadOnly")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_RATIO]=@RelatedTicketingProductQtyRatio")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_QTY_ROUND_UP]=@RelatedTicketingProductQtyRoundUp")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE]=@RelatedTicketingProductCampaignCode")
            sqlStatement.Append(", [RELATED_TICKETING_PRODUCT_PRICE_CODE]=@RelatedTicketingProductPriceCode")
            sqlStatement.Append(", [RELATED_CSS_CLASS]=@RelatedCssClass")
            sqlStatement.Append(", [RELATED_INSTRUCTIONS]=@RelatedInstructions")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_01]=@PackageComponentValue1")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_02]=@PackageComponentValue2")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_03]=@PackageComponentValue3")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_04]=@PackageComponentValue4")
            sqlStatement.Append(", [PACKAGE_COMPONENT_VALUE_05]=@PackageComponentValue5")
            sqlStatement.Append(", [PACKAGE_COMPONENT_PRICE_BANDS]=@PackageComponentPriceBands")
            sqlStatement.Append(" WHERE [FOREIGN_PRODUCT_RELATIONS_ID]=@ForeignLinkId")

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ForeignLinkId", foreignLinkId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkType", linkType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductType", ticketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductSubType", ticketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingProductPriceCode", ticketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductCampaignCode", relatedTicketingProductCampaignCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProduct", relatedProductCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductType", relatedTicketingProductType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductSubType", relatedTicketingProductSubType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductPriceCode", relatedTicketingProductPriceCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedProductMandatory", relatedProductMandatory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductStand", relatedTicketingProductStand))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductStandReadOnly", relatedTicketingProductStandReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductArea", relatedTicketingProductArea))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductAreaReadOnly", relatedTicketingProductAreaReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQty", relatedTicketingProductQty))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyMin", relatedTicketingProductQtyMin))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyMax", relatedTicketingProductQtyMax))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyReadOnly", relatedTicketingProductQtyReadOnly))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyRatio", relatedTicketingProductQtyRatio))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedTicketingProductQtyRoundUp", relatedTicketingProductQtyRoundUp))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedCssClass", relatedCssClass))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RelatedInstructions", relatedInstructions))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue1", packageComponentValue1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue2", packageComponentValue2))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue3", packageComponentValue3))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue4", packageComponentValue4))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentValue5", packageComponentValue5))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PackageComponentPriceBands", packageComponentPriceBands))
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            talentSqlAccessDetail = Nothing
            Return affectedRows
        End Function
        ''' <summary>
        ''' Deletes a product relation from tbl_product_relations
        ''' </summary>
        ''' <param name="productRelationsId">The given product relations ID that has been selected for deletion</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function GetForiegnProductRelationsID(ByVal productRelationsId As Integer, Optional ByVal givenTransaction As SqlTransaction = Nothing) As String
            Dim foriegnProductRelationsID As String = String.Empty
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT [FOREIGN_PRODUCT_RELATIONS_ID] FROM [tbl_product_relations] WHERE [PRODUCT_RELATIONS_ID]=@ProductRelationsId"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductRelationsId", productRelationsId))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                foriegnProductRelationsID = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            talentSqlAccessDetail = Nothing

            'Return results
            Return foriegnProductRelationsID
        End Function

        ''' <summary>
        ''' Deletes relations by product - wipes away trace of product from tbl_product_relations
        ''' </summary>
        ''' <param name="productCode">The given product relations ID that has been selected for deletion</param>
        ''' <param name="givenTransaction">The given transaction</param>
        ''' <returns>The number of affected rows</returns>
        ''' <remarks></remarks>
        Public Function RemoveRelationsByProduct(ByVal productCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As String
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False

                'Execute
                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM tbl_product_relations Where PRODUCT=@ProductCode AND RELATED_PRODUCT=@ProductCode"

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