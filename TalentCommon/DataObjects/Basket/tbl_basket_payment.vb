Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    <Serializable()> _
    Public Class tbl_basket_payment
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_payment"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_payment" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            MyBase.New()
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get basket payment record by basket header ID
        ''' </summary>
        ''' <param name="basketHeaderID">The basket header ID</param>
        ''' <returns>Basket payment record table</returns>
        ''' <remarks></remarks>
        Public Function GetPaymentRecordByBasketHeaderID(ByVal basketHeaderID As String) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPaymentRecordByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP(1) * FROM [tbl_basket_payment] WHERE BASKET_HEADER_ID = @BasketHeaderID ORDER BY DATE_CREATED DESC"
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

        Public Function InsertPaymentDetails(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("INSERT INTO [tbl_basket_payment] (")
            sqlStatement.Append("[BASKET_HEADER_ID], [LOGIN_ID], [STATUS], [CHECKOUT_STAGE], [CAPTUREMETHOD], [CAN_SAVE_CARD], [AGENT_NAME], [PAYMENT_TYPE], [CARD_TYPE]")
            sqlStatement.Append(", [DATE_CREATED], [DATE_UPDATED],[PAYMENT_AMOUNT],[SAVEDCARD_UNIQUEID],[BASKET_AMOUNT],[TEMP_ORDER_ID]) ")
            sqlStatement.Append("VALUES (")
            sqlStatement.Append("@BasketHeaderID, @LoginID, @Status, @CheckoutStage, @CaptureMethod, @CanSaveCard, @AgentName, @PaymentType, @CardType")
            sqlStatement.Append(", @DateCreated, @DateUpdated,@PaymentAmount,@SavedCardUniqueID, @BasketAmount, @TempOrderID) ")
            sqlStatement.Append(" SELECT SCOPE_IDENTITY() ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketPaymentEntity.BASKET_HEADER_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", basketPaymentEntity.LOGIN_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", basketPaymentEntity.STATUS))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CheckoutStage", basketPaymentEntity.CHECKOUT_STAGE))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CaptureMethod", basketPaymentEntity.CAPTUREMETHOD))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CanSaveCard", basketPaymentEntity.CAN_SAVE_CARD, SqlDbType.Bit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", basketPaymentEntity.AGENT_NAME))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentType", basketPaymentEntity.PAYMENT_TYPE))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardType", basketPaymentEntity.CARD_TYPE))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateCreated", Now, SqlDbType.DateTime))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentAmount", basketPaymentEntity.PAYMENT_AMOUNT, SqlDbType.Decimal))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SavedCardUniqueID", basketPaymentEntity.SAVEDCARD_UNIQUEID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketAmount", basketPaymentEntity.BASKET_AMOUNT))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderID", basketPaymentEntity.TEMP_ORDER_ID))
            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows

        End Function

        Public Function DeleteByHeaderIDAndStage(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("DELETE [tbl_basket_payment] ")
            sqlStatement.Append(" WHERE BASKET_HEADER_ID = @BasketHeaderID AND CHECKOUT_STAGE = @CheckoutStage AND STATUS < 50 ")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketPaymentEntity.BASKET_HEADER_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", basketPaymentEntity.STATUS))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CheckoutStage", basketPaymentEntity.CHECKOUT_STAGE))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows
        End Function

        Public Function StartPaymentForStage(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            If DeleteByHeaderIDAndStage(basketPaymentEntity, givenTransaction) >= 0 Then
                affectedRows = InsertPaymentDetails(basketPaymentEntity, givenTransaction)
            Else
                affectedRows = -1
            End If
            Return affectedRows
        End Function

        Public Function UpdateSessionDetail(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append(" UPDATE [tbl_basket_payment] ")
            sqlStatement.Append(" SET STATUS = @Status, PROCESSING_DB = @ProcessingDB, SESSION_GUID = @SessionGUID, SESSION_PASSCODE = @SessionPasscode")
            sqlStatement.Append(", DATE_UPDATED = @DateUpdated WHERE BASKET_PAYMENT_ID = @BasketPaymentID")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketPaymentID", basketPaymentEntity.BASKET_PAYMENT_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", basketPaymentEntity.STATUS))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProcessingDB", basketPaymentEntity.PROCESSING_DB))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionGUID", basketPaymentEntity.SESSION_GUID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SessionPasscode", basketPaymentEntity.SESSION_PASSCODE))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows
        End Function

        Public Function UpdateCardDetail(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append(" UPDATE [tbl_basket_payment] ")
            sqlStatement.Append(" SET CARDNUMBER = @CardNumber, STARTMONTH = @StartMonth, STARTYEAR = @StartYear, EXPIRYMONTH = @ExpiryMonth")
            sqlStatement.Append(", EXPIRYYEAR = @ExpiryYear, ISSUENUMBER = @IssueNumber, ADDRESS_LINE_1 = @AddressLine1, POST_CODE = @PostCode")
            sqlStatement.Append(", CARD_HOLDER_NAME = @CardHolderName, STATUS = @Status, CARD_TYPE = @CardType")
            sqlStatement.Append(", DATE_UPDATED = @DateUpdated, TRANSACTION_TYPE = @TransactionType WHERE BASKET_PAYMENT_ID = @BasketPaymentID")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketPaymentID", basketPaymentEntity.BASKET_PAYMENT_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardNumber", basketPaymentEntity.CARDNUMBER))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StartMonth", basketPaymentEntity.STARTMONTH))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StartYear", basketPaymentEntity.STARTYEAR))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ExpiryMonth", basketPaymentEntity.EXPIRYMONTH))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ExpiryYear", basketPaymentEntity.EXPIRYYEAR))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IssueNumber", basketPaymentEntity.ISSUENUMBER))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AddressLine1", basketPaymentEntity.ADDRESS_LINE_1))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PostCode", basketPaymentEntity.POST_CODE))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardHolderName", basketPaymentEntity.CARD_HOLDER_NAME))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", basketPaymentEntity.STATUS))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TransactionType", basketPaymentEntity.TXNType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardType", basketPaymentEntity.CARD_TYPE))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows
        End Function

        Public Function UpdateTokenID(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append(" UPDATE [tbl_basket_payment] ")
            sqlStatement.Append(" SET STATUS = @Status, TOKENID = @TokenID")
            sqlStatement.Append(", DATE_UPDATED = @DateUpdated WHERE BASKET_PAYMENT_ID = @BasketPaymentID")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketPaymentID", basketPaymentEntity.BASKET_PAYMENT_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", basketPaymentEntity.STATUS))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TokenID", basketPaymentEntity.TOKENID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows
        End Function

        'Private Function UpdatePaymentDetails(ByVal basket_header_id As String, ByVal login_id As String, ByVal status As Integer, ByVal checkoutstage As String, ByVal capturemethod As Integer, ByVal savemycard As Boolean, ByVal agentname As String, ByVal paymenttype As String, ByVal cardtype As String, ByVal cardnumber As String, ByVal startmonth As Integer, ByVal startyear As Integer, ByVal expirymonth As Integer, ByVal expiryyear As Integer, ByVal tandcAccepted As Boolean, ByVal detail1 As String, ByVal detail2 As String, ByVal detail3 As String, ByVal paymentamount As Decimal, ByVal tokenid As String) As Integer

        '    Dim affectedRows As Integer = 0
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim err As New ErrorObj

        '    talentSqlAccessDetail.Settings = _settings
        '    talentSqlAccessDetail.Settings.Cacheing = False
        '    talentSqlAccessDetail.Settings.CacheStringExtension = ""
        '    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        '    Dim sqlStatement As New StringBuilder
        '    sqlStatement.Append(" UPDATE [tbl_basket_payment] ")
        '    sqlStatement.Append(" SET LOGIN_ID = @LoginID ")
        '    sqlStatement.Append(" WHERE BASKET_HEADER_ID = @BasketHeaderID AND CHECKOUT_STAGE = @CheckoutStage ")
        '    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basket_header_id))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", login_id))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", status, SqlDbType.SmallInt))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CheckoutStage", checkoutstage))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CaptureMethod", capturemethod))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SaveMyCard", savemycard, SqlDbType.Bit))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AgentName", agentname))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentType", paymenttype))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardType", cardtype))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardNumber", cardnumber))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StartMonth", startmonth, SqlDbType.TinyInt))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StartYear", startyear, SqlDbType.TinyInt))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ExpiryMonth", expirymonth, SqlDbType.TinyInt))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ExpiryYear", expirymonth, SqlDbType.TinyInt))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TAndCAccepted", tandcAccepted, SqlDbType.Bit))

        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Details1", detail1))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Details2", detail2))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Details3", detail3))

        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentAmount", paymentamount, SqlDbType.Decimal))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TokenID", tokenid))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))

        '    err = talentSqlAccessDetail.SQLAccess()

        '    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
        '    End If

        '    talentSqlAccessDetail = Nothing

        '    Return affectedRows
        'End Function

        'Private Function UpdatePaymentDetails(ByVal sessionguid As String, ByVal sessionpasscode As String, ByVal status As Integer, ByVal basket_header_id As String, ByVal login_id As String) As Integer

        '    Dim affectedRows As Integer = 0
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Dim err As New ErrorObj

        '    talentSqlAccessDetail.Settings = _settings
        '    talentSqlAccessDetail.Settings.Cacheing = False
        '    talentSqlAccessDetail.Settings.CacheStringExtension = ""
        '    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        '    Dim sqlStatement As String = "UPDATE [tbl_basket_payment]" & _
        '                                 "SET Status=@Status," & _
        '                                        "Details2 = @Details2," & _
        '                                        "Details3 = @Details3 where Basket_Header_ID = @BasketHeaderId and Login_ID=@LoginId"
        '    '"CheckoutStage = @CheckoutStage," & _

        '    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basket_header_id))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", login_id))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", status, SqlDbType.SmallInt))
        '    'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CheckoutStage", checkoutstage))

        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Details2", sessionguid))
        '    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Details3", sessionpasscode))

        '    err = talentSqlAccessDetail.SQLAccess()

        '    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '        affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
        '    End If

        '    talentSqlAccessDetail = Nothing

        '    Return affectedRows

        'End Function

        'Private Function GetSessionGUID(ByVal basket_header_id As String, ByVal login_id As String) As DEVanguard


        '    Dim outputDataTable As New DataTable
        '    Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetSessionGUID")
        '    Dim talentSqlAccessDetail As New TalentDataAccess
        '    Try
        '        'Construct The Call
        '        talentSqlAccessDetail.Settings = _settings
        '        talentSqlAccessDetail.Settings.Cacheing = False
        '        talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
        '        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
        '        talentSqlAccessDetail.CommandElements.CommandText = "SELECT Details2 FROM [tbl_basket_payment] WHERE BASKET_HEADER_ID = @BasketHeaderID and Login_ID=@LoginId"
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basket_header_id))
        '        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", login_id))

        '        'Execute
        '        Dim err As New ErrorObj
        '        err = talentSqlAccessDetail.SQLAccess()
        '        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
        '            outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
        '        End If
        '    Catch ex As Exception
        '        Throw
        '    Finally
        '        talentSqlAccessDetail = Nothing
        '    End Try

        '    Dim config As New DEVanguard
        '    config.SessionGUID = outputDataTable.Rows(0)("Details2")

        '    Return config

        'End Function


        Public Function UpdateSavedCardFlag(ByVal basketPaymentEntity As DEBasketPayment, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append(" UPDATE [tbl_basket_payment] ")
            sqlStatement.Append(" SET CAN_SAVE_CARD = @CanSaveCard")
            sqlStatement.Append(", DATE_UPDATED = @DateUpdated WHERE BASKET_PAYMENT_ID = @BasketPaymentID")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketPaymentID", basketPaymentEntity.BASKET_PAYMENT_ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CanSaveCard", basketPaymentEntity.CAN_SAVE_CARD))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows
        End Function

        Public Function UpdateStatus(ByVal basketPaymentID As Integer, ByVal status As Integer, ByVal merchantReference As String, Optional ByVal transactionID As Integer = Nothing, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append(" UPDATE [tbl_basket_payment] ")
            sqlStatement.Append(" SET STATUS = @STATUS")
            sqlStatement.Append(", DATE_UPDATED = @DateUpdated, TRANSACTION_ID = @TransactionID, MERCHANT_REFERENCE = @MerchantReference WHERE BASKET_PAYMENT_ID = @BasketPaymentID")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketPaymentID", basketPaymentID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@STATUS", status))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DateUpdated", Now, SqlDbType.DateTime))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TransactionID", IIf(IsNothing(transactionID), DBNull.Value, transactionID), SqlDbType.VarChar))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@MerchantReference", merchantReference))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing

            Return affectedRows
        End Function

        Public Function UpdateTicketingPaymentRef(ByVal basketHeaderID As Integer, ByVal ticketingPaymentRef As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = " UPDATE [tbl_basket_payment] SET TICKETING_PAYMENT_REF = @TicketingPayRef WHERE BASKET_HEADER_ID = @BasketHeaderID"

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TicketingPayRef", ticketingPaymentRef))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            Else
                affectedRows = -1
            End If
            talentSqlAccessDetail = Nothing
            Return affectedRows
        End Function

#End Region


    End Class
End Namespace