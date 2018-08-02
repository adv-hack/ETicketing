Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_basket_header based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_basket_header
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_header"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_header" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get basket header record by basket header ID
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header ID</param>
        ''' <returns>Basket header record table</returns>
        ''' <remarks></remarks>
        Public Function GetHeaderByBasketHeaderID(ByVal basketHeaderID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetHeaderByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_header] WHERE BASKET_HEADER_ID = @BasketHeaderID"
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

        Public Function GetHeaderByBasketSessionID(ByVal sessionID As String, ByVal businessUnit As String) As Long
            Dim basketHeaderID As New Long
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetHeaderByBasketSessionID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_basket_header] WHERE PROCESSED = 0 AND BUSINESS_UNIT = @BusinessUnit AND SESSION_ID = @SessionID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionID", sessionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not err.HasError) AndAlso
                    talentSqlAccessDetail.ResultDataSet IsNot Nothing AndAlso
                    talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 AndAlso
                    talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    basketHeaderID = Utilities.CheckForDBNull_BigInt(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)("BASKET_HEADER_ID"))
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the basket header id 
            Return basketHeaderID
        End Function

        ''' <summary>
        ''' Insert into the basket header table based on the given parameters
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="basketEntity">The basket data entity object</param>
        ''' <returns>Affected rows</returns>
        ''' <remarks></remarks>
        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, ByVal basketEntity As DEBasket) As Integer
            Dim affectedRows As Integer = 0
            If basketEntity IsNot Nothing Then
                Dim talentSqlAccessDetail As New TalentDataAccess
                Dim sqlStatement As New StringBuilder

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                sqlStatement.Append("INSERT INTO tbl_basket_header")
                sqlStatement.Append("(BUSINESS_UNIT, PARTNER, LOGINID, ")
                sqlStatement.Append(" CREATED_DATE, LAST_ACCESSED_DATE, PROCESSED, ")
                sqlStatement.Append(" STOCK_ERROR, USER_SELECT_FULFIL, PAYMENT_OPTIONS, ")
                sqlStatement.Append(" RESTRICT_PAYMENT_OPTIONS, CAMPAIGN_CODE, PAYMENT_ACCOUNT_ID) ")
                sqlStatement.Append(" VALUES (@BusinessUnit, @Partner, @LoginId, @CreatedDate, @LastAccessedDate, @Processed, ")
                sqlStatement.Append(" @StockError, @UserSelectFulfil, @PayOptions, @MixedPayType, @CampaignCode, @PaymentAccountId) ")

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CreatedDate", basketEntity.Created_Date, SqlDbType.DateTime))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LastAccessedDate", basketEntity.Last_Accessed_Date, SqlDbType.DateTime))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Processed", basketEntity.Processed))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StockError", basketEntity.STOCK_ERROR))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@UserSelectFulfil", basketEntity.USER_SELECT_FULFIL))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PayOptions", basketEntity.PAYMENT_OPTIONS))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MixedPayType", basketEntity.RESTRICT_PAYMENT_OPTIONS))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CampaignCode", basketEntity.CAMPAIGN_CODE))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentAccountId", basketEntity.PAYMENT_ACCOUNT_ID))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()

                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
                talentSqlAccessDetail = Nothing
            End If

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Update the payment type in the basket header table for the given basket header ID
        ''' </summary>
        ''' <param name="basketHeaderId">The basket header ID</param>
        ''' <param name="paymentType">The given payment type</param>
        ''' <returns>Affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdatePaymentType(ByVal basketHeaderId As String, ByVal paymentType As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_basket_header SET PAYMENT_TYPE = @PaymentType WHERE BASKET_HEADER_ID = @BasketHeaderID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentType", paymentType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Update payment type and card type code basket on the given basket header ID
        ''' </summary>
        ''' <param name="basketHeaderId">The basket header ID</param>
        ''' <param name="paymentType">The payment type</param>
        ''' <param name="cardTypeCode">The card type code</param>
        ''' <returns>Affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdatePaymentAndCardType(ByVal basketHeaderId As String, ByVal paymentType As String, ByVal cardTypeCode As String) As Integer
            Dim affectedRows As Integer = 0
            If cardTypeCode = GlobalConstants.FEE_BOOKING_CARDTYPE_EMPTY Then cardTypeCode = ""
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_basket_header SET PAYMENT_TYPE = @PaymentType, CARD_TYPE_CODE = @CardType WHERE BASKET_HEADER_ID = @BasketHeaderID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentType", paymentType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardType", cardTypeCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Process the basket update for the basket header record
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header ID</param>
        ''' <param name="dtBasketHeader">The basket header table</param>
        ''' <returns>Affected rows</returns>
        ''' <remarks></remarks>
        Public Function ProcessNewBasketUpdate(ByVal basketHeaderID As String, ByVal dtBasketHeader As DataTable) As Integer
            Dim affectedRows As Integer = 0
            If dtBasketHeader IsNot Nothing AndAlso dtBasketHeader.Rows.Count > 0 Then
                Dim talentSqlAccessDetail As New TalentDataAccess
                Dim sqlStatement As New StringBuilder

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                sqlStatement.Append("UPDATE tbl_basket_header SET MARKETING_CAMPAIGN = @MarketingCampaign,")
                sqlStatement.Append(" PAYMENT_TYPE = '', ")
                sqlStatement.Append(" CARD_TYPE_CODE = '', ")
                sqlStatement.Append(" USER_SELECT_FULFIL = @UserSelectFulfil,")
                sqlStatement.Append(" PAYMENT_OPTIONS = @PaymentOptions,")
                sqlStatement.Append(" RESTRICT_PAYMENT_OPTIONS = @restrictpaymentoptions,")
                sqlStatement.Append(" PAYMENT_ACCOUNT_ID = @PaymentAccountId, ")
                sqlStatement.Append(" EXTERNAL_PAYMENT_TOKEN = @PaymentExternalToken, ")
                sqlStatement.Append(" DELIVERY_COUNTRY_CODE = @DeliveryCountryCode, ")
                sqlStatement.Append(" DELIVERY_ZONE_CODE = @DeliveryZoneCode, ")
                sqlStatement.Append(" CAT_MODE = @CatMode, ")
                sqlStatement.Append(" CAT_PRICE = @CatPrice, ")
                sqlStatement.Append(" ORIGINAL_SALE_PAID_WITH_CF = @originalSalePaidWithCF ")
                sqlStatement.Append(" WHERE BASKET_HEADER_ID = @BasketHeaderID")

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@UserSelectFulfil", dtBasketHeader.Rows(0).Item("UserSelectFulfilment").ToString.Trim))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentOptions", dtBasketHeader.Rows(0).Item("PaymentOptions").ToString().Trim()))
                If dtBasketHeader.Rows(0).Item("MarketingCampaign").ToString.Trim = "Y" Then
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MarketingCampaign", 1))
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MarketingCampaign", 0))
                End If
                If dtBasketHeader.Rows(0).Item("restrictpaymentoptions").ToString.Trim = "Y" Then
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@restrictpaymentoptions", 1))
                Else
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@restrictpaymentoptions", 0))
                End If
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentAccountId", dtBasketHeader.Rows(0).Item("PaymentAccountId").ToString.Trim))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentExternalToken", dtBasketHeader.Rows(0).Item("PaymentExternalToken").ToString.Trim))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CatMode", dtBasketHeader.Rows(0).Item("CATMode").ToString.Trim))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CatPrice", CType(dtBasketHeader.Rows(0).Item("CATPrice").ToString.Trim, Decimal), SqlDbType.Decimal))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@originalSalePaidWithCF", dtBasketHeader.Rows(0).Item("OriginalSalePaidWithCF"), SqlDbType.Bit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryZoneCode", GetDeliveryZoneCode(_settings.DeliveryCountryCode)))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryCountryCode", _settings.DeliveryCountryCode))
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()

                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
                talentSqlAccessDetail = Nothing
            End If

            'Return the results 
            Return affectedRows
        End Function

        Public Function UpdateDeliveryZoneCode(ByVal basketHeaderID As String, ByVal deliveryCountryCode As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As New StringBuilder

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            sqlStatement.Append("UPDATE tbl_basket_header SET ")
            sqlStatement.Append(" DELIVERY_COUNTRY_CODE = @DeliveryCountryCode, ")
            sqlStatement.Append(" DELIVERY_ZONE_CODE = @DeliveryZoneCode ")
            sqlStatement.Append(" WHERE BASKET_HEADER_ID = @BasketHeaderID ")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryZoneCode", GetDeliveryZoneCode(deliveryCountryCode)))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliveryCountryCode", _settings.DeliveryCountryCode))
            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Update the linked master product code in the basket header table by the given basket ID
        ''' </summary>
        ''' <param name="basketHeaderID">The basket ID</param>
        ''' <param name="linkedMasterProduct">The linked master product</param>
        ''' <returns>Affected rows</returns>
        ''' <remarks></remarks>
        Public Function UpdateLinkedProductMaster(ByVal basketHeaderID As String, ByVal linkedMasterProduct As String) As Integer
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim affectedRows As Integer
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            If String.IsNullOrEmpty(linkedMasterProduct) Then
                sqlStatement.Append("UPDATE tbl_basket_header SET LINKED_MASTER_PRODUCT = NULL")
            Else
                sqlStatement.Append("UPDATE tbl_basket_header SET LINKED_MASTER_PRODUCT = @LinkedMasterProduct")
            End If
            sqlStatement.Append(" WHERE BASKET_HEADER_ID = @BasketHeaderID")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LinkedMasterProduct", linkedMasterProduct))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return the results 
            Return affectedRows
        End Function

        ''' <summary>
        ''' Retrieved the linked master product code from the basket header table by the given basket ID
        ''' </summary>
        ''' <param name="basketHeaderId">The basket header ID</param>
        ''' <returns>The linked master product code</returns>
        ''' <remarks></remarks>
        Public Function GetLinkedMasterProduct(ByVal basketHeaderId As String) As String
            Dim outputData As String = String.Empty
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [LINKED_MASTER_PRODUCT] FROM [tbl_basket_header] WHERE BASKET_HEADER_ID = @BasketHeaderID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputData = Utilities.CheckForDBNull_String(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0).ToString)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputData
        End Function

        Public Function DeleteBasketHeader(ByVal basketHeaderId As String, ByVal businessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0

            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE [tbl_basket_header] WHERE BASKET_HEADER_ID = @basketHeaderId "
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@basketHeaderId", basketHeaderId))

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
        ''' Update the basket header table for the ticketing payment reference
        ''' </summary>
        ''' <param name="basketHeaderId">The basket record to update</param>
        ''' <param name="ticketingPaymentRef">The payment reference to update with</param>
        ''' <returns>The number of affected records</returns>
        ''' <remarks></remarks>
        Public Function UpdateTicketingPaymentReference(ByVal basketHeaderId As String, ByVal ticketingPaymentRef As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE [tbl_basket_header] SET [TICKETING_PAYMENT_REF] = @TicketingPaymentRef WHERE [BASKET_HEADER_ID] = @BasketHeaderID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingPaymentRef", ticketingPaymentRef))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderId))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()

            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return the results 
            Return affectedRows
        End Function

#End Region

#Region "Private Methods"
        Private Function GetDeliveryZoneCode(ByVal countryCode As String) As String
            Dim deliveryZoneCode As String = String.Empty
            Dim talDefaults As New TalentDefaults
            talDefaults.Settings = _settings
            deliveryZoneCode = talDefaults.RetrieveGeographicalZone(countryCode)
            Return deliveryZoneCode
        End Function
#End Region

    End Class
End Namespace