Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Web


<Serializable()> _
Public Class TalentBase

#Region "Class Level Fields"

    Private _destinationDatabase As String
    Private _connectionString As String
    Private _settings As New DESettings
    Private _TDataObjects As TalentDataObjects
    Private _talentDataSet As TalentDataSet
    Private _talentLogger As TalentLogging = Nothing
    Private _cacheUtility As CacheUtility

#End Region

#Region "Public Properties"

    Public Property DestinationDatabase() As String
        Get
            Return _destinationDatabase
        End Get
        Set(ByVal value As String)
            _destinationDatabase = value
        End Set
    End Property

    Public Property ConnectionString() As String
        Get
            Return _connectionString
        End Get
        Set(ByVal value As String)
            _connectionString = value
        End Set
    End Property

    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property

    Public Property TDataObjects() As TalentDataObjects
        Get
            If _TDataObjects Is Nothing Then
                _TDataObjects = New Talent.Common.TalentDataObjects()
                _TDataObjects.Settings = Settings
                _TDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            End If
            Return _TDataObjects
        End Get
        Set(ByVal value As TalentDataObjects)
            _TDataObjects = value
        End Set
    End Property

    Public Property TalDataSet() As TalentDataSet
        Get
            If _talentDataSet Is Nothing Then
                _talentDataSet = New Talent.Common.TalentDataSet
            End If
            Return _talentDataSet
        End Get
        Set(ByVal value As TalentDataSet)
            _talentDataSet = value
        End Set
    End Property

    Protected ReadOnly Property TalentLogger() As TalentLogging
        Get
            If _talentLogger Is Nothing Then
                _talentLogger = New TalentLogging
                _talentLogger.FrontEndConnectionString = Settings.FrontEndConnectionString
            End If
            Return _talentLogger
        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the fulfilment fee category dictionary (fulfilment method, fee category)
    ''' key - fulfilment method
    ''' value - fee category
    ''' </summary>
    ''' <value>
    ''' The fulfilment fee category.
    ''' </value>
    Public Property FulfilmentFeeCategory() As Dictionary(Of String, String) = Nothing
    Public Property CardTypeFeeCategory() As Dictionary(Of String, String) = Nothing

    Public ReadOnly Property CacheUtility As CacheUtility
        Get
            If (_cacheUtility Is Nothing) Then
                _cacheUtility = New CacheUtility()
            End If
            Return _cacheUtility
        End Get
    End Property
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Gets the connection details and populating the DESettings object
    ''' </summary>
    ''' <param name="businessUnit">The business unit.</param>
    ''' <param name="application">The application.</param>
    ''' <param name="moduleName">Name of the module.</param>
    ''' <param name="sectionName">Name of the section.</param>
    ''' <param name="subSectionName">Name of the sub section.</param>
    ''' <param name="partner">The partner.</param>
    Public Sub GetConnectionDetails(ByVal businessUnit As String, Optional ByVal application As String = "", Optional ByVal moduleName As String = "", _
                                    Optional ByVal sectionName As String = "", Optional ByVal subSectionName As String = "", Optional ByVal partner As String = "")
        If String.IsNullOrEmpty(application) Then application = Utilities.GetAllString
        If String.IsNullOrEmpty(moduleName) Then moduleName = Utilities.GetAllString
        If String.IsNullOrEmpty(sectionName) Then sectionName = Utilities.GetAllString
        If String.IsNullOrEmpty(subSectionName) Then subSectionName = Utilities.GetAllString
        If String.IsNullOrEmpty(partner) Then partner = Utilities.GetAllString


        If Not Settings.FrontEndConnectionString Is Nothing Then
            Dim conDets As connectionDets = Nothing
            Dim canOverrideCacheAttributes As Boolean = False
            Dim isCacheEnabled As Boolean = False
            Dim cacheSeconds As Integer = 0
            Dim cacheKey As String = "TalentCommon - GetConnectionDetails - " & businessUnit & "|" & partner & "|" & application & "|" & moduleName & "|" & sectionName & "|" & subSectionName

            'Check that the cache key is in cache, but the conDets object is set correctly before using it
            If Utilities.IsCacheActive AndAlso HttpContext.Current.Cache.Item(cacheKey) IsNot Nothing Then
                conDets = CType(HttpContext.Current.Cache.Item(cacheKey), connectionDets)
                If conDets Is Nothing OrElse conDets.DestinationDatabase Is Nothing _
                    OrElse conDets.DatabaseType1 Is Nothing OrElse conDets.BackOfficeConnectionString Is Nothing _
                    OrElse conDets.DestinationType Is Nothing Then
                    HttpContext.Current.Cache.Remove(cacheKey)
                End If
            End If

            If Utilities.IsCacheActive AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                conDets = CType(HttpContext.Current.Cache.Item(cacheKey), connectionDets)
                Settings.DestinationDatabase = conDets.DestinationDatabase
                Settings.DatabaseType1 = conDets.DatabaseType1
                Settings.BackOfficeConnectionString = conDets.BackOfficeConnectionString
                Settings.DestinationType = conDets.DestinationType
                Settings.XmlSettings = conDets.XmlSettings
                If conDets.CanOverrideCacheAttributes Then
                    Settings.Cacheing = conDets.IsCacheEnabled
                    Settings.CacheTimeSeconds = conDets.CacheSeconds
                    Settings.CacheTimeMinutes = 0
                End If
            Else
                Dim dr As SqlClient.SqlDataReader
                Dim drDest As SqlClient.SqlDataReader
                Dim cmd As New SqlClient.SqlCommand
                cmd.Connection = New SqlClient.SqlConnection(Settings.FrontEndConnectionString)
                Const SqlSelect As String = _
                                " SELECT mdb.*, dt.* " & _
                                " FROM tbl_bu_module_database mdb " & _
                                "	INNER JOIN tbl_destination_type dt " & _
                                "		ON mdb.DESTINATION_DATABASE = dt.DESTINATION_TYPE_LINK " & _
                                " WHERE mdb.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                "	AND mdb.PARTNER = @PARTNER " & _
                                "	AND mdb.APPLICATION = @APPLICATION " & _
                                "	AND mdb.MODULE = @MODULE "

                Const sectionClause As String = "   AND mdb.SECTION = @SECTION "
                Const subSectionClause As String = "   AND mdb.SUB_SECTION = @SUB_SECTION "
                Try
                    With cmd.Parameters
                        .Clear()
                        .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = businessUnit
                        .Add("@APPLICATION", SqlDbType.NVarChar).Value = application
                        .Add("@MODULE", SqlDbType.NVarChar).Value = moduleName
                        .Add("@SECTION", SqlDbType.NVarChar).Value = sectionName
                        .Add("@SUB_SECTION", SqlDbType.NVarChar).Value = subSectionName
                        .Add("@PARTNER", SqlDbType.NVarChar).Value = partner
                    End With

                    cmd.Connection.Open()
                    cmd.CommandText = SqlSelect & sectionClause & subSectionClause
                    dr = cmd.ExecuteReader
                    If Not dr.HasRows Then
                        dr.Close()
                        cmd.CommandText = SqlSelect & sectionClause
                        dr = cmd.ExecuteReader
                        If Not dr.HasRows Then
                            dr.Close()
                            cmd.CommandText = SqlSelect
                            dr = cmd.ExecuteReader
                            If Not dr.HasRows Then
                                dr.Close()
                                cmd.CommandText = SqlSelect
                                cmd.Parameters.Item("@PARTNER").Value = Utilities.GetAllString
                                dr = cmd.ExecuteReader
                                If Not dr.HasRows Then
                                    dr.Close()
                                    cmd.CommandText = SqlSelect
                                    cmd.Parameters.Item("@BUSINESS_UNIT").Value = Utilities.GetAllString
                                    dr = cmd.ExecuteReader
                                    If Not dr.HasRows Then
                                        dr.Close()
                                        cmd.CommandText = SqlSelect
                                        cmd.Parameters.Item("@MODULE").Value = Utilities.GetAllString
                                        dr = cmd.ExecuteReader
                                    End If
                                End If
                            End If
                        End If
                    End If

                    If dr.HasRows Then
                        dr.Read()
                        Settings.DestinationType = Utilities.CheckForDBNull_String(dr("DESTINATION_TYPE"))
                        Dim mydest As String = Utilities.CheckForDBNull_String(dr("DESTINATION_DATABASE"))
                        Try
                            If (Not dr("CACHE_ENABLED").Equals(DBNull.Value)) AndAlso (dr("CACHE_ENABLED") IsNot Nothing) AndAlso (Not String.IsNullOrWhiteSpace(dr("CACHE_ENABLED"))) Then
                                canOverrideCacheAttributes = True
                                isCacheEnabled = Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("CACHE_ENABLED"))
                                cacheSeconds = Utilities.CheckForDBNull_Int(dr("CACHE_SECONDS"))
                            Else
                                canOverrideCacheAttributes = False
                                isCacheEnabled = False
                                cacheSeconds = 0
                            End If
                        Catch ex As Exception
                            canOverrideCacheAttributes = False
                        End Try
                        dr.Close()

                        With cmd.Parameters
                            .Clear()
                            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = businessUnit
                            .Add("@PARTNER", SqlDbType.NVarChar).Value = partner
                        End With
                        Select Case Settings.DestinationType
                            Case "DB", "DATABASE"
                                cmd.CommandText = " SELECT * FROM tbl_database_version " & _
                                                    " 	WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                                    " 	AND [PARTNER] = @PARTNER " & _
                                                    " 	AND DESTINATION_DATABASE = @DESTINATION_DATABASE "
                                cmd.Parameters.Add("@DESTINATION_DATABASE", SqlDbType.NVarChar).Value = mydest
                            Case "XML"
                                cmd.CommandText = " SELECT * FROM tbl_destination_xml " & _
                                                    " 	WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                                    " 	AND [PARTNER] = @PARTNER " & _
                                                    " 	AND DESTINATION_TYPE = @DESTINATION_TYPE "
                                cmd.Parameters.Add("@DESTINATION_TYPE", SqlDbType.NVarChar).Value = mydest
                            Case Else
                                cmd.CommandText = " SELECT * FROM tbl_database_version " & _
                                                    " 	WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                                    " 	AND [PARTNER] = @PARTNER " & _
                                                    " 	AND DESTINATION_DATABASE = @DESTINATION_DATABASE "
                                cmd.Parameters.Add("@DESTINATION_DATABASE", SqlDbType.NVarChar).Value = mydest
                        End Select

                        drDest = cmd.ExecuteReader
                        If Not drDest.HasRows Then
                            drDest.Close()
                            cmd.Parameters.Item("@PARTNER").Value = Utilities.GetAllString
                            drDest = cmd.ExecuteReader
                            If Not drDest.HasRows Then
                                drDest.Close()
                                cmd.Parameters.Item("@BUSINESS_UNIT").Value = Utilities.GetAllString
                                drDest = cmd.ExecuteReader
                            End If
                        End If

                        If drDest.HasRows Then
                            While drDest.Read
                                Select Case Settings.DestinationType
                                    Case "DB", "DATABASE"
                                        Settings.DestinationDatabase = Utilities.CheckForDBNull_String(drDest("DESTINATION_DATABASE"))
                                        Settings.DatabaseType1 = Utilities.CheckForDBNull_String(drDest("DATABASE_TYPE1"))
                                        Settings.BackOfficeConnectionString = Utilities.CheckForDBNull_String(drDest("CONNECTION_STRING"))
                                        'deciding whether to override cache settings or not
                                        If canOverrideCacheAttributes Then
                                            Settings.Cacheing = isCacheEnabled
                                            If isCacheEnabled AndAlso cacheSeconds > 0 Then
                                                Settings.CacheTimeMinutes = 0
                                                Settings.CacheTimeSeconds = cacheSeconds
                                            Else
                                                Settings.CacheTimeMinutes = 0
                                                Settings.CacheTimeSeconds = 0
                                            End If
                                        End If
                                    Case "XML"
                                        With Settings.XmlSettings
                                            .ArchiveXml = Utilities.CheckForDBNull_Boolean_DefaultFalse(drDest("ARCHIVE_XML"))
                                            .ArchiveXmlLocation = Utilities.CheckForDBNull_String(drDest("ARCHIVE_XML_LOCATION"))
                                            .DestinationType = Utilities.CheckForDBNull_String(drDest("DESTINATION_TYPE"))
                                            .EmailXmlAttach = Utilities.CheckForDBNull_Boolean_DefaultFalse(drDest("EMAIL_XML_ATTACH"))
                                            .EmailXmlContent = Utilities.CheckForDBNull_Boolean_DefaultFalse(drDest("EMAIL_XML_CONTENT"))
                                            .EmailXmlRecipient = Utilities.CheckForDBNull_String(drDest("EMAIL_XML_RECIPIENT"))
                                            .InputXmlLocation = Utilities.CheckForDBNull_String(drDest("INPUT_XML_LOCATION"))
                                            .PostXml = Utilities.CheckForDBNull_Boolean_DefaultFalse(drDest("POST_XML"))
                                            .PostXmlUrl = Utilities.CheckForDBNull_String(drDest("POST_XML_URL"))
                                            .StoreXml = Utilities.CheckForDBNull_Boolean_DefaultFalse(drDest("STORE_XML"))
                                            .StoreXmlLocation = Utilities.CheckForDBNull_String(drDest("STORE_XML_LOCATION"))
                                            .ValidateXsd = Utilities.CheckForDBNull_Boolean_DefaultFalse(drDest("VALIDATE_XSD"))
                                            .ValidateXsdLocation = Utilities.CheckForDBNull_String(drDest("VALIDATE_XSD_LOCATION"))
                                            .XmlVersion = Utilities.CheckForDBNull_String(drDest("XML_VERSION"))
                                            .UserName = Utilities.CheckForDBNull_String(drDest("USERNAME"))
                                            .Password = Utilities.CheckForDBNull_String(drDest("PASSWORD"))
                                            .DomainName = Utilities.CheckForDBNull_String(drDest("DOMAIN_NAME"))
                                        End With
                                    Case Else
                                        Settings.DestinationDatabase = Utilities.CheckForDBNull_String(drDest("DESTINATION_DATABASE"))
                                        Settings.DatabaseType1 = Utilities.CheckForDBNull_String(drDest("DATABASE_TYPE1"))
                                        Settings.BackOfficeConnectionString = Utilities.CheckForDBNull_String(drDest("CONNECTION_STRING"))
                                End Select

                                If Settings.DestinationType <> "XML" Then
                                    'Decode the connection string
                                    Settings.BackOfficeConnectionString = Utilities.ExternalDecryption(Utilities.CheckForDBNull_String(Settings.BackOfficeConnectionString),
                                                                                                Utilities.CheckForDBNull_String(drDest("ENCRYPTION_KEY_KEY")),
                                                                                                Utilities.RetrieveCommonDefault(Settings.FrontEndConnectionString, "EXTERNAL_ENCRYPTION_URL"))
                                End If
                            End While
                            drDest.Close()
                        Else
                            drDest.Close()
                        End If
                    Else
                        dr.Close()
                    End If
                Catch ex As Exception
                Finally
                    cmd.Connection.Close()
                End Try
                ' Insert to cache
                Try
                    conDets = New connectionDets(Settings.DestinationDatabase, Settings.DatabaseType1, Settings.BackOfficeConnectionString, Settings.DestinationType, canOverrideCacheAttributes, isCacheEnabled, cacheSeconds, Settings.XmlSettings)
                    If Utilities.IsCacheActive Then
                        HttpContext.Current.Cache.Insert(cacheKey, conDets, Nothing, System.DateTime.Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                Catch ex As Exception
                End Try
            End If



        End If
    End Sub

    Public Sub AddItemToSession(ByVal sessionKey As String, ByVal cacheItem As Object)
        CacheUtility.AddItemToSession(sessionKey, cacheItem)
    End Sub

    Public Sub RemoveItemFromSession(ByVal sessionKey As String)
        CacheUtility.RemoveItemFromSession(sessionKey)
    End Sub

    Public Sub AddItemToCache(ByVal cacheKey As String, ByVal cacheItem As Object, ByVal settings As DESettings)
        CacheUtility.AddItemToCache(cacheKey, cacheItem, settings, settings.ModuleName)
    End Sub

    Public Sub AddItemToCache(ByVal cacheKey As String, ByVal cacheItem As Object, ByVal settings As DESettings, ByVal cacheDependencyName As String)
        CacheUtility.AddItemToCache(cacheKey, cacheItem, settings, cacheDependencyName)
    End Sub

    Public Sub RemoveItemFromCache(ByVal cacheKey As String)
        CacheUtility.RemoveItemFromCache(cacheKey)
    End Sub

    Private Function CheckResponseForError(ByVal ds As DataSet, ByVal err As ErrorObj) As String
        Dim errorCode As String = String.Empty
        ' The dataset will give us errors from the back end
        If Not ds Is Nothing Then
            Dim errorTable As Data.DataTable = Nothing
            Try
                errorTable = ds.Tables(0)
            Catch ex As Exception
                errorCode = "AAC"
            End Try
            If Not errorTable Is Nothing Then
                For Each row As Data.DataRow In errorTable.Rows
                    If Not row("ReturnCode").Equals(String.Empty) Then
                        errorCode = row("ReturnCode")
                        Exit For
                    End If
                Next
            Else
                errorCode = "AAD"
            End If
        Else
            errorCode = "AAB"
        End If
        ' The error object will give us any errors with comms to the back end.
        If Not errorCode.Length > 0 Then
            If err.HasError Then
                errorCode = err.ErrorStatus
            End If
        End If
        Return errorCode
    End Function

    ''' <summary>
    ''' Process basket which received from backend and move it to frontend
    ''' </summary>
    ''' <param name="applicationSource"></param>
    ''' <param name="basketHeaderID"></param>
    ''' <param name="dsResultDataSet"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ProcessNewBasket(ByVal applicationSource As String, ByVal basketHeaderID As String, ByRef dsResultDataSet As DataSet) As ErrorObj
        Dim errObj As New ErrorObj
        Dim returnErrorCode As String = CheckResponseForError(dsResultDataSet, errObj)
        If returnErrorCode.Trim.Length = 0 Then
            If applicationSource = GlobalConstants.SOURCESUPPLYNET Then
                ProcessNewBasketSupplyNet(basketHeaderID, errObj, dsResultDataSet)
            Else
                ProcessNewBasketWebSales(basketHeaderID, errObj, dsResultDataSet)
            End If
        End If
        Return errObj
    End Function

    Private Sub ProcessNewBasketWebSales(ByVal basketHeaderID As String, ByRef errObj As ErrorObj, ByRef dsResultDataSet As DataSet)
        Dim talBasketProcessor As New TalentBasketProcessor
        talBasketProcessor.Settings = Settings
        errObj = talBasketProcessor.ProcessBasket(basketHeaderID, dsResultDataSet)
        dsResultDataSet = talBasketProcessor.ResultDataSet
    End Sub

    Private Sub ProcessNewBasketSupplyNet(ByVal basketHeaderID As String, ByRef errObj As ErrorObj, ByRef dsResultDataSet As DataSet)
        If (dsResultDataSet Is Nothing) OrElse (dsResultDataSet IsNot Nothing AndAlso
                dsResultDataSet.Tables.Count > 1 AndAlso
                dsResultDataSet.Tables("BasketHeader") IsNot Nothing AndAlso
                dsResultDataSet.Tables("BasketHeader").Rows.Count = 0) Then
            Dim basketHeaderIDToDelete As Long = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketSessionID(basketHeaderID, Settings.BusinessUnit)
            If basketHeaderIDToDelete <> Nothing AndAlso basketHeaderIDToDelete > 0 Then
                TDataObjects.BasketSettings.TblBasketDetail.DeleteBasketDetail(basketHeaderIDToDelete, GlobalConstants.BASKETMODULETICKETING)
                TDataObjects.BasketSettings.TblBasketHeader.DeleteBasketHeader(basketHeaderIDToDelete, Settings.BusinessUnit)
            End If
        Else
            basketHeaderID = GetSupplyNetBasketHeaderID(basketHeaderID, dsResultDataSet)
            Dim talBasketProcessor As New TalentBasketProcessor
            talBasketProcessor.Settings = Settings
            errObj = talBasketProcessor.ProcessBasket(basketHeaderID, dsResultDataSet)
            ProcessSupplyNetSet(basketHeaderID, errObj, talBasketProcessor.ResultDataSet)
        End If
    End Sub

    Private Sub ProcessSupplyNetSet(ByVal basketHeaderID As String, ByRef errObj As ErrorObj, ByRef dsResultDataSet As DataSet)
        'now add the fees to resultset

        ' Initialise the table with values 
        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyBookingFee") = "N"
        dsResultDataSet.Tables("BasketHeader").Rows(0)("BookingFee") = 0
        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyCarriageFee") = "N"
        dsResultDataSet.Tables("BasketHeader").Rows(0)("CarriageFee") = 0
        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyWebServicesFee") = "N"
        dsResultDataSet.Tables("BasketHeader").Rows(0)("WebServicesFee") = 0
        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyWebSalesFee") = "N"
        dsResultDataSet.Tables("BasketHeader").Rows(0)("WebSalesFee") = 0
        dsResultDataSet.Tables("BasketHeader").Rows(0)("TotalPrice") = 0

        Try
            Dim dtBasketHeader As DataTable = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID)
            Dim dtBasketDetail As DataTable = TDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderIDModule(basketHeaderID, GlobalConstants.BASKETMODULETICKETING)

            For detailRowIndex As Integer = 0 To dtBasketDetail.Rows.Count - 1

                'Total Price calculated form basket detail records.
                dsResultDataSet.Tables("BasketHeader").Rows(0)("TotalPrice") += Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(detailRowIndex)("GROSS_PRICE")).ToString() * Utilities.CheckForDBNull_Int(dtBasketDetail.Rows(detailRowIndex)("QUANTITY")).ToString()

                If Utilities.CheckForDBNull_String(dtBasketDetail.Rows(detailRowIndex)("FEE_CATEGORY")).Trim.Length > 0 Then
                    If dtBasketDetail.Rows(detailRowIndex)("PRODUCT") = GlobalConstants.BKFEE Then
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyBookingFee") = "Y"
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("BookingFee") = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(detailRowIndex)("GROSS_PRICE")).ToString()
                    ElseIf dtBasketDetail.Rows(detailRowIndex)("PRODUCT") = GlobalConstants.CRFEE Then
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyCarriageFee") = "Y"
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("CarriageFee") = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(detailRowIndex)("GROSS_PRICE")).ToString()
                    ElseIf dtBasketDetail.Rows(detailRowIndex)("PRODUCT") = GlobalConstants.ATFEE Then
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyWebServicesFee") = "Y"
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("WebServicesFee") = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(detailRowIndex)("GROSS_PRICE")).ToString()
                    ElseIf dtBasketDetail.Rows(detailRowIndex)("PRODUCT") = GlobalConstants.WSFEE Then
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("ApplyWebSalesFee") = "Y"
                        dsResultDataSet.Tables("BasketHeader").Rows(0)("WebSalesFee") = Utilities.CheckForDBNull_Decimal(dtBasketDetail.Rows(detailRowIndex)("GROSS_PRICE")).ToString()
                    End If
                End If
            Next

        Catch ex As Exception
            errObj.HasError = True
            errObj.ErrorNumber = "TCTB-001"
            errObj.ErrorMessage = "Error occured while processing supplynet basket - " & ex.Message
        End Try


    End Sub

    Private Function GetSupplyNetBasketHeaderID(ByVal sessionID As String, ByRef dsResultDataSet As DataSet) As String
        Dim basketHeaderID As String = String.Empty
        Dim tDataObjects As New Talent.Common.TalentDataObjects
        tDataObjects.Settings = Settings
        tDataObjects.BasketSettings.BasketHelperEntity.BusinessUnit = Settings.BusinessUnit
        tDataObjects.BasketSettings.BasketHelperEntity.CampaignCode = ""
        tDataObjects.BasketSettings.BasketHelperEntity.CatMode = Utilities.CheckForDBNull_String(dsResultDataSet.Tables("BasketHeader").Rows(0)("CATMode"))
        tDataObjects.BasketSettings.BasketHelperEntity.CatPrice = Utilities.CheckForDBNull_String(dsResultDataSet.Tables("BasketHeader").Rows(0)("CATPrice"))
        tDataObjects.BasketSettings.BasketHelperEntity.ExternalPaymentToken = ""
        tDataObjects.BasketSettings.BasketHelperEntity.IsAnonymous = True
        tDataObjects.BasketSettings.BasketHelperEntity.IsAuthenticated = False
        tDataObjects.BasketSettings.BasketHelperEntity.LoginId = sessionID
        tDataObjects.BasketSettings.BasketHelperEntity.MarketingCampaign = Utilities.CheckForDBNull_Boolean_DefaultFalse(Utilities.convertToBool(dsResultDataSet.Tables("BasketHeader").Rows(0)("MarketingCampaign")))
        tDataObjects.BasketSettings.BasketHelperEntity.RestrictPaymentOptions = Utilities.CheckForDBNull_String(dsResultDataSet.Tables("BasketHeader").Rows(0)("RestrictPaymentOptions")).ConvertFromISeriesYesNoToBoolean()
        tDataObjects.BasketSettings.BasketHelperEntity.Partner = Settings.Partner
        tDataObjects.BasketSettings.BasketHelperEntity.PaymentAccountID = Utilities.CheckForDBNull_String(dsResultDataSet.Tables("BasketHeader").Rows(0)("PaymentAccountId"))
        tDataObjects.BasketSettings.BasketHelperEntity.PaymentOptions = Utilities.CheckForDBNull_String(dsResultDataSet.Tables("BasketHeader").Rows(0)("PaymentOptions"))
        tDataObjects.BasketSettings.BasketHelperEntity.Processed = False
        tDataObjects.BasketSettings.BasketHelperEntity.SessionID = sessionID
        tDataObjects.BasketSettings.BasketHelperEntity.StockError = False
        tDataObjects.BasketSettings.BasketHelperEntity.UserSelectFulfilment = Utilities.CheckForDBNull_String(dsResultDataSet.Tables("BasketHeader").Rows(0)("UserSelectFulfilment"))
        tDataObjects.BasketSettings.BasketHelperEntity.CatPrice = "0"
        basketHeaderID = tDataObjects.BasketSettings.GetHeaderIDBySupplyNetSessionID()
        Return basketHeaderID
    End Function

#End Region

    Private Class connectionDets

        Private _DestinationDatabase As String
        Private _DatabaseType1 As String
        Private _BackOfficeConnectionString As String
        Private _destinationType As String
        Private _canOverrideCacheAttributes As Boolean = False
        Private _isCacheEnabled As Boolean = False
        Private _cacheSeconds As Integer = 0
        Private _xmlSettings As DEXmlSettings = Nothing

        Public Property DestinationDatabase() As String
            Get
                Return _DestinationDatabase
            End Get
            Set(ByVal value As String)
                _DestinationDatabase = value
            End Set
        End Property
        Public Property DatabaseType1() As String
            Get
                Return _DatabaseType1
            End Get
            Set(ByVal value As String)
                _DatabaseType1 = value
            End Set
        End Property
        Public Property BackOfficeConnectionString() As String
            Get
                Return _BackOfficeConnectionString
            End Get
            Set(ByVal value As String)
                _BackOfficeConnectionString = value
            End Set
        End Property
        Public Property DestinationType() As String
            Get
                Return _destinationType
            End Get
            Set(ByVal value As String)
                _destinationType = value
            End Set
        End Property
        Public Property CanOverrideCacheAttributes() As Boolean
            Get
                Return _canOverrideCacheAttributes
            End Get
            Set(ByVal value As Boolean)
                _canOverrideCacheAttributes = value
            End Set
        End Property
        Public Property IsCacheEnabled() As Boolean
            Get
                Return _isCacheEnabled
            End Get
            Set(ByVal value As Boolean)
                _isCacheEnabled = value
            End Set
        End Property
        Public Property CacheSeconds() As Integer
            Get
                Return _cacheSeconds
            End Get
            Set(ByVal value As Integer)
                _cacheSeconds = value
            End Set
        End Property
        Public Property XmlSettings() As DEXmlSettings
            Get
                Return _xmlSettings
            End Get
            Set(ByVal value As DEXmlSettings)
                _xmlSettings = value
            End Set
        End Property

        Public Sub New(ByVal destinationDatabase As String,
                        ByVal databaseType1 As String,
                        ByVal backOfficeConnectionString As String,
                        ByVal destinationType As String,
                        ByVal canOverrideCacheAttributes As Boolean,
                        ByVal isCacheEnabled As Boolean,
                        ByVal cacheSeconds As Integer,
                        ByVal xmlSettings As DEXmlSettings)

            _DestinationDatabase = destinationDatabase
            _DatabaseType1 = databaseType1
            _BackOfficeConnectionString = backOfficeConnectionString
            _destinationType = destinationType
            _canOverrideCacheAttributes = canOverrideCacheAttributes
            _isCacheEnabled = isCacheEnabled
            _cacheSeconds = cacheSeconds
            _xmlSettings = xmlSettings
        End Sub

    End Class

End Class
