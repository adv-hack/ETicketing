Imports System.Data.SqlClient
Imports System.Text
Imports Talent.Common.DataObjects.TableObjects

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access payment related details like currency, payments types etc.,
    ''' </summary>
    <Serializable()> _
        Public Class Payment
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblCreditCard As tbl_creditcard
        Private _tblCurrency As tbl_currency
        Private _tblCurrencyFormatBu As tbl_currency_format_bu
        Private _tblPaymentType As tbl_payment_type
        Private _tblPaymentTypeLang As tbl_payment_type_lang
        Private _tblPaymentTypeBu As tbl_payment_type_bu
        Private _tblAdhocFees As tbl_adhoc_fees
        Private _tblBasketDetail As tbl_basket_detail
        Private _tblPaymentDefaults As tbl_payment_defaults
        Private _tblChipAndPinDevices As tbl_chip_and_pin_devices
        Private _tblPointOfSaleTerminals As tbl_point_of_sale_terminals
        Private _tblCreditCardTypeControl As tbl_creditcard_type_control
        Private _tblCreditCardBu As tbl_creditcard_bu

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Payment"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Payment" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_creditcard instance with DESettings
        ''' </summary>
        ''' <value>tbl_creditcard instance</value>
        Public ReadOnly Property TblCreditCard() As tbl_creditcard
            Get
                If (_tblCreditCard Is Nothing) Then
                    _tblCreditCard = New tbl_creditcard(_settings)
                End If
                Return _tblCreditCard
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_currency instance with DESettings
        ''' </summary>
        ''' <value>tbl_currency instance</value>
        Public ReadOnly Property TblCurrency() As tbl_currency
            Get
                If (_tblCurrency Is Nothing) Then
                    _tblCurrency = New tbl_currency(_settings)
                End If
                Return _tblCurrency
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_currency_format_bu instance with DESettings
        ''' </summary>
        ''' <value>tbl_currency_format_bu instance</value>
        Public ReadOnly Property TblCurrencyFormatBu() As tbl_currency_format_bu
            Get
                If (_tblCurrencyFormatBu Is Nothing) Then
                    _tblCurrencyFormatBu = New tbl_currency_format_bu(_settings)
                End If
                Return _tblCurrencyFormatBu
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_payment_type with DESettings
        ''' </summary>
        ''' <value>The tbl_payment_type instance.</value>
        Public ReadOnly Property TblPaymentType() As tbl_payment_type
            Get
                If (_tblPaymentType Is Nothing) Then
                    _tblPaymentType = New tbl_payment_type(_settings)
                End If
                Return _tblPaymentType
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_payment_type_lang with DESettings
        ''' </summary>
        ''' <value>The tbl_payment_type_lang instance.</value>
        Public ReadOnly Property TblPaymentTypeLang() As tbl_payment_type_lang
            Get
                If (_tblPaymentTypeLang Is Nothing) Then
                    _tblPaymentTypeLang = New tbl_payment_type_lang(_settings)
                End If
                Return _tblPaymentTypeLang
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_payment_type_bu with DESettings
        ''' </summary>
        ''' <value>The tbl_payment_type_bu instance.</value>
        Public ReadOnly Property TblPaymentTypeBu() As tbl_payment_type_bu
            Get
                If (_tblPaymentTypeBu Is Nothing) Then
                    _tblPaymentTypeBu = New tbl_payment_type_bu(_settings)
                End If
                Return _tblPaymentTypeBu
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_adhoc_fees with DESettings
        ''' </summary>
        ''' <value>The tbl_adhoc_fees instance.</value>
        Public ReadOnly Property TblAdhocFees() As tbl_adhoc_fees
            Get
                If (_tblAdhocFees Is Nothing) Then
                    _tblAdhocFees = New tbl_adhoc_fees(_settings)
                End If
                Return _tblAdhocFees
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_basket_detail with DESettings
        ''' </summary>
        ''' <value>The tbl_basket_detail instance.</value>
        Public ReadOnly Property TblBasketDetail() As tbl_basket_detail
            Get
                If (_tblBasketDetail Is Nothing) Then
                    _tblBasketDetail = New tbl_basket_detail(_settings)
                End If
                Return _tblBasketDetail
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_payment_defaults with DESettings
        ''' </summary>
        ''' <value>The tbl_payment_defaults instance</value>
        Public ReadOnly Property TblPaymentDefaults() As tbl_payment_defaults
            Get
                If (_tblPaymentDefaults Is Nothing) Then
                    _tblPaymentDefaults = New tbl_payment_defaults(_settings)
                End If
                Return _tblPaymentDefaults
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_chip_and_pin_devices with DESettings
        ''' </summary>
        ''' <value>The tbl_chip_and_pin_devices instance</value>
        Public ReadOnly Property TblChipAndPinDevices() As tbl_chip_and_pin_devices
            Get
                If (_tblChipAndPinDevices Is Nothing) Then
                    _tblChipAndPinDevices = New tbl_chip_and_pin_devices(_settings)
                End If
                Return _tblChipAndPinDevices
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_point_of_sale_terminals with DESettings
        ''' </summary>
        ''' <value>The tbl_point_of_sale_terminals instance</value>
        Public ReadOnly Property TblPointOfSaleTerminals() As tbl_point_of_sale_terminals
            Get
                If (_tblPointOfSaleTerminals Is Nothing) Then
                    _tblPointOfSaleTerminals = New tbl_point_of_sale_terminals(_settings)
                End If
                Return _tblPointOfSaleTerminals
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_creditcard_type_control with DESettings
        ''' </summary>
        ''' <value>The tbl_creditcard_type_control instance</value>
        Public ReadOnly Property TblCreditCardTypeControl() As tbl_creditcard_type_control
            Get
                If (_tblCreditCardTypeControl Is Nothing) Then
                    _tblCreditCardTypeControl = New tbl_creditcard_type_control(_settings)
                End If
                Return _tblCreditCardTypeControl
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_creditcard_bu with DESettings
        ''' </summary>
        ''' <value>The tbl_creditcard_bu instance</value>
        Public ReadOnly Property TblCreditCardBu() As tbl_creditcard_bu
            Get
                If (_tblCreditCardBu Is Nothing) Then
                    _tblCreditCardBu = New tbl_creditcard_bu(_settings)
                End If
                Return _tblCreditCardBu
            End Get
        End Property

#End Region

#Region "Public Functions"

        Public Function GetCreditCardTypeRanges(ByVal dbConnectionString As String, Optional ByVal strCreditCardTypeCode As String = "") As DataSet
            Dim ds As DataSet = Nothing
            Dim strSelectQuery As String = String.Empty
            Dim cmd As SqlCommand = Nothing
            Dim da As SqlDataAdapter = Nothing
            Try
                strSelectQuery = "  select		a.CARD_CODE,a.CARD_FROM_RANGE,a.CARD_TO_RANGE,a.RANGE_DESCRIPTION,a.MAX_INSTALLMENTS,a.INSTALLMENTS_LIST" & _
                                 "  from		dbo.tbl_creditcard_type_control a " & _
                                 "  where       a.card_code = isnull(@creditCardType,a.CARD_CODE)" & _
                                 "  order by	a.CARD_CODE,a.CARD_FROM_RANGE,a.CARD_TO_RANGE" & _
                                 "  select		distinct a.CARD_CODE" & _
                                 "  from		dbo.tbl_creditcard_type_control a" & _
                                 "  order by	a.CARD_CODE"
                cmd = New SqlCommand(strSelectQuery, New SqlConnection(dbConnectionString))
                cmd.Connection.Open()
                With cmd.Parameters
                    .Add("@creditCardType", SqlDbType.NVarChar).Value = IIf(strCreditCardTypeCode = "", DBNull.Value, strCreditCardTypeCode)
                End With
                da = New SqlDataAdapter(cmd)
                ds = New DataSet()
                da.Fill(ds)
                Return ds
            Finally
                cmd.Connection.Close()
                da.Dispose()
                ds = Nothing
                strSelectQuery = Nothing
                cmd = Nothing
            End Try
        End Function

        ''' <summary>
        ''' Get the currency code for the system using the default price list
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="cacheing">The cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time in mins, default 30</param>
        ''' <returns>The currency code as a string, eg. "ENG" by default</returns>
        ''' <remarks></remarks>
        Public Function GetCurrencyCode(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim currencyCode As String = "GBP"
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetCurrencyCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As New StringBuilder
            Dim whereClauseFetchHierarchy(1) As String
            Dim cacheKeyHierarchyBased(1) As String

            whereClauseFetchHierarchy(0) = "AND (tbl_ecommerce_module_defaults_bu.PARTNER = @Partner)"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)
            whereClauseFetchHierarchy(1) = "AND (tbl_ecommerce_module_defaults_bu.PARTNER = '" & ReplaceSingleQuote(Utilities.GetAllString) & "')"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                sqlStatement.Append("SELECT tbl_price_list_header.CURRENCY_CODE ")
                sqlStatement.Append("FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)  INNER JOIN ")
                sqlStatement.Append("tbl_price_list_header WITH (NOLOCK)  ON tbl_ecommerce_module_defaults_bu.VALUE = tbl_price_list_header.PRICE_LIST ")
                sqlStatement.Append("WHERE (tbl_ecommerce_module_defaults_bu.DEFAULT_NAME = 'PRICE_LIST') AND (tbl_ecommerce_module_defaults_bu.BUSINESS_UNIT = @BusinessUnit) ")
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

                'Execute
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 1 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString() & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0) Then
                        currencyCode = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return currencyCode
        End Function

        ''' <summary>
        ''' Format the currency for the given value for the business unit and partner
        ''' </summary>
        ''' <param name="value">The value to format</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <returns>The formatted currency value</returns>
        ''' <remarks></remarks>
        Public Function FormatCurrency(ByVal value As Decimal, ByVal businessUnit As String, ByVal partner As String) As String
            Dim currencyCode As String = GetCurrencyCode(businessUnit, partner)
            Return appendCurrencyDetails(value, currencyCode, businessUnit)
        End Function

        ''' <summary>
        ''' Retrieve the currency code for a business unit and partner.
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <returns>The formatted currency value</returns>
        ''' <remarks></remarks>
        Public Function GetCurrencySymbol(ByVal businessUnit As String, ByVal partner As String) As String
            Dim currencySymbol As String
            Dim arbitaryValue As String = "0.00"
            Dim examplePrice As String
            Dim currencyCode As String

            currencyCode = GetCurrencyCode(businessUnit, partner)
            examplePrice = appendCurrencyDetails(0.0, currencyCode, businessUnit)
            currencySymbol = examplePrice.Substring(0, examplePrice.IndexOf(arbitaryValue))
            Return currencySymbol
        End Function

#End Region

#Region "Private Functions"

        ''' <summary>
        ''' Append the currency code to the value for the given business unit
        ''' </summary>
        ''' <param name="value">The decimal value</param>
        ''' <param name="currencyCode">The given currency code</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <returns>The formatted value with currency code</returns>
        ''' <remarks></remarks>
        Private Function appendCurrencyDetails(ByVal value As Decimal, ByVal currencyCode As String, ByVal businessUnit As String) As String
            Dim formattedValue As String = String.Empty
            Dim ruleType As String = String.Empty
            Dim newValue As Decimal = 0
            Dim dtCurrencyFormat As DataTable
            dtCurrencyFormat = TblCurrencyFormatBu.GetByBUAndCurrencyCode(businessUnit, currencyCode)
            If dtCurrencyFormat.Rows.Count > 0 Then
                ruleType = Utilities.CheckForDBNull_String(dtCurrencyFormat.Rows(0)("RULE_TYPE"))
                newValue = value
                If Utilities.CheckForDBNull_String(dtCurrencyFormat.Rows(0)("FORMAT_STRING")) <> String.Empty Then
                    formattedValue = newValue.ToString(dtCurrencyFormat.Rows(0)("FORMAT_STRING"))
                Else
                    formattedValue = newValue.ToString
                End If
                '------------------------
                ' Check Rules:
                ' 1) Replace '.' with ','
                ' 2) ...
                '------------------------
                Select Case ruleType
                    Case Is = "1"
                        formattedValue = formattedValue.Replace(".", ",")
                    Case Else
                End Select
            End If
            Return formattedValue
        End Function

#End Region

    End Class
End Namespace
