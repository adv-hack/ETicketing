Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Send Email Requests
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBSendEmail
    Inherits DBAccess

    '---------------------------------------------------------------------------------------------
    Private _dep As New DEOrder
    Private _System21OrderNo As String
    Private _system21CompanyNo As String
    '
    Private _dtHeader As New DataTable("DtHeader")
    Private _dtDetail As New DataTable("DtDetail")
    Private _dtText As New DataTable("DtText")
    Private _dtCarrier As New DataTable("DtCarrier")
    Private _dtPackage As New DataTable("DtPackage")
    Private _dtProduct As New DataTable("DtProduct")
    '---------------------------------------------------------------------------------------------
    Friend ord As DEOrderReponse         ' otherwise we hold previous order info

    Private _parmTRAN As String
    Private _parmHEAD(500) As String
    Private _parmHEAD2(500) As Collection

    Private _parmADDR(500, 500) As String
    Private _parmPAY(500, 500) As String
    Private _parmCHGE(500, 500) As String
    ' Private _parmITEM(500, 500) As String
    Private _parmITEM(500, 1000) As String
    Private _parmITEM2(500, 500) As Collection
    Private _parmCOMM(500, 500) As String
    Private _parmCOMMLineNumber(500, 500) As String
    Private _parmPAYMENT(500, 500) As String
    Private _parmORDERREQ As String
    Private _parmMISC(500) As miscItems

    Private _carrierName As String
    Private _ConsignmentNumber As String

    Private _orderLineNo As Int32

    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
            _dep = value
        End Set
    End Property

    Public Property System21OrderNo() As String
        Get
            Return _System21OrderNo
        End Get
        Set(ByVal value As String)
            _System21OrderNo = value
        End Set
    End Property
    Public Property System21CompanyNo() As String
        Get
            Return _system21CompanyNo
        End Get
        Set(ByVal value As String)
            _system21CompanyNo = value
        End Set
    End Property

    Public Property DtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
        End Set
    End Property
    Public Property DtDetail() As DataTable
        Get
            Return _dtDetail
        End Get
        Set(ByVal value As DataTable)
            _dtDetail = value
        End Set
    End Property
    Public Property DtText() As DataTable
        Get
            Return _dtText
        End Get
        Set(ByVal value As DataTable)
            _dtText = value
        End Set
    End Property
    Public Property DtCarrier() As DataTable
        Get
            Return _dtCarrier
        End Get
        Set(ByVal value As DataTable)
            _dtCarrier = value
        End Set
    End Property
    Public Property DtPackage() As DataTable
        Get
            Return _dtPackage
        End Get
        Set(ByVal value As DataTable)
            _dtPackage = value
        End Set
    End Property
    Public Property DtProduct() As DataTable
        Get
            Return _dtProduct
        End Get
        Set(ByVal value As DataTable)
            _dtProduct = value
        End Set
    End Property
    Public Property CarrierName() As String
        Get
            Return _carrierName
        End Get
        Set(ByVal value As String)
            _carrierName = value
        End Set
    End Property
    Public Property ConsignmentNumber() As String
        Get
            Return _ConsignmentNumber
        End Get
        Set(ByVal value As String)
            _ConsignmentNumber = value
        End Set
    End Property
    Private _backOfficeOrderNumber As String
    Public Property BackOfficeOrderNumber() As String
        Get
            Return _backOfficeOrderNumber
        End Get
        Set(ByVal value As String)
            _backOfficeOrderNumber = value
        End Set
    End Property
    '----------------------------------------------------------------------------------------------
    ' Structure to store anything that is required by the final WRITEORD call from the dataentities
    ' but isn't needed in any of the other calls
    '----------------------------------------------------------------------------------------------
    Private Class miscItems

        Private _totvalueCharged As Decimal
        Public Property TotalValueCharged() As Decimal
            Get
                Return _totvalueCharged
            End Get
            Set(ByVal value As Decimal)
                _totvalueCharged = value
            End Set
        End Property


        Private _promotionValue As Decimal
        Public Property PromotionValue() As Decimal
            Get
                Return _promotionValue
            End Get
            Set(ByVal value As Decimal)
                _promotionValue = value
            End Set
        End Property


        Private _paymentType As String
        Public Property PaymentType() As String
            Get
                Return _paymentType
            End Get
            Set(ByVal value As String)
                _paymentType = value
            End Set
        End Property

        Private _projectedDeliveryDate As String
        Public Property ProjectedDeliveryDate() As String
            Get
                Return _projectedDeliveryDate
            End Get
            Set(ByVal value As String)
                _projectedDeliveryDate = value
            End Set
        End Property


        Private _totOrderValue As Decimal
        Public Property TotalOrderValue() As Decimal
            Get
                Return _totOrderValue
            End Get
            Set(ByVal value As Decimal)
                _totOrderValue = value
            End Set
        End Property

        Private _orderFailedInitialValidation As Boolean
        Public Property OrderFailedInitialValidation() As Boolean
            Get
                Return _orderFailedInitialValidation
            End Get
            Set(ByVal value As Boolean)
                _orderFailedInitialValidation = value
            End Set
        End Property

        Private _orderFailedInitialValidationMessage As String
        Public Property OrderFailedInitialValidationMessage() As String
            Get
                Return _orderFailedInitialValidationMessage
            End Get
            Set(ByVal value As String)
                _orderFailedInitialValidationMessage = value
            End Set
        End Property

        Public Sub New(ByVal pPaymentType As String, _
                        ByVal pProjectedDeliveryDate As String, _
                        ByVal pPromotion_Value As Decimal, _
                        ByVal pTotal_Value_Charged As Decimal, _
                        ByVal pTotal_Order_Value As Decimal, _
                        ByVal pOrderFailedInitialValidation As Boolean, _
                        ByVal pOrderFailedInitialValidationMessage As String)

            _paymentType = pPaymentType
            _projectedDeliveryDate = pProjectedDeliveryDate
            _promotionValue = pPromotion_Value
            _totvalueCharged = pTotal_Value_Charged
            _totOrderValue = pTotal_Order_Value
            _orderFailedInitialValidation = pOrderFailedInitialValidation
            _orderFailedInitialValidationMessage = pOrderFailedInitialValidationMessage
        End Sub

    End Class

    Public Function WriteToDatabase(Optional ByVal opendb As Boolean = True) As ErrorObj
        Dim err As New ErrorObj
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                '-------------------------------------------------------------------
                '   The communications error maybe caused by the database not being 
                '   open so we could change to :
                '
                '   If Not err.HasError and conSystem21.State = ConnectionState.Open Then _
                '       err = WriteDataBaseSystem21()
                '   else
                '       throw err
                '   End if
                '
                '-------------------------------------------------------------------
                If Not err.HasError() Then _
                    err = WriteDataBaseSystem21()
                '
                If opendb Then System21Close()
            Case Is = SQL2005
                err = WriteDataBaseSQL2005()
                'Stuart - Dunhill Specific section
            Case Is = ADL
                If opendb Then err = ADLOpen()
                If Not err.HasError() Then _
                    err = WriteDataBaseADL()
                If opendb Then ADLClose()
            Case Is = CHORUS
                If opendb Then err = ChorusOpen()
                err = WriteDataBaseChorus()
                If opendb Then ChorusClose()
        End Select
        Return err
    End Function

    Private Function WriteDataBaseChorus() As ErrorObj
        '---------------------------------------------------------------------------
        '   AccessDataBaseSystem21: Write the order to Chorus via Stored Procedures
        '
        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '---------------------------------------------------------------------------
        '   Open up collection and deal with by putting to stored procedures parameters 
        '
        Dim collDates As New Collection
        collDates.Add(Date.Now)
        err = DataEntityUnPackChorus()
        If Not err.HasError Then
            '------------------------------------------------------------------
            ' Setup Calls to As400
            ' 
            Dim strSqlORDERREQ As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/ORDERREQ(@PARAM1, @PARAM2)"
            Dim strSqlORDERREQ2 As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/ORDERREQ2(@PARAM1, @PARAM2)"
            Dim strSqlWRITETXT As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/WRITETXT(@PARAM1,@PARAM2,@PARAM3)"

            Dim strSqlWRITEORD As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/WRITEORD(@PARAM1, @PARAM2)"

            Dim cmdSelectORDERREQ As iDB2Command = Nothing
            Dim cmdSelectORDERREQ2 As iDB2Command = Nothing
            Dim cmdSelectWRITETXT As iDB2Command = Nothing
            Dim cmdSelectWRITEORD As iDB2Command = Nothing

            Dim iItems As Integer = 0
            Dim iOrder As Integer = 0
            Dim iComments As Integer = 0

            Dim parmInput, Paramoutput, parmComment As iDB2Parameter
            Dim PARMOUT As String = String.Empty

            Dim strTalentOrderNo As String = String.Empty
            '----------------------------------------------------------------
            '   Loop through orders in transaction
            '
            Try
                'Throw New System.DivideByZeroException
                collDates.Add(Date.Now)
                For iOrder = 1 To 500

                    If ParmHEAD(iOrder) > String.Empty Then
                        '----------------------------------------------------------------
                        '   Call order header stored procedure towrite to Talent (ORDERREQ)
                        '
                        Dim contimeOut As Integer = conChorus.ConnectionTimeout
                        cmdSelectORDERREQ = New iDB2Command(strSqlORDERREQ, conChorus)

                        parmInput = cmdSelectORDERREQ.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        parmInput.Value = ParmHEAD(iOrder)
                        parmInput.Direction = ParameterDirection.Input

                        Paramoutput = cmdSelectORDERREQ.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        Paramoutput.Value = String.Empty
                        Paramoutput.Direction = ParameterDirection.InputOutput
                        cmdSelectORDERREQ.CommandTimeout = 120
                        cmdSelectORDERREQ.ExecuteNonQuery()
                        ' Interpret results
                        PARMOUT = cmdSelectORDERREQ.Parameters(Param2).Value.ToString
                        strTalentOrderNo = Utilities.FixStringLength(PARMOUT.Substring(0, 15), 15)
                        ' Error? 
                        If PARMOUT.Substring(1023, 1) = "Y" Then
                            With err
                                .ItemErrorMessage(iOrder) = String.Empty
                                .ItemErrorCode(iOrder) = "TACDBOR-50"
                                .ItemErrorStatus(iOrder) = "Error creating order header - " & _
                                                    PARMOUT.Substring(1019, 4) & "-" & _
                                                    Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                    "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                            End With

                        Else
                            '----------------------------------------------------------------
                            '   Loop through items in current order
                            ' 
                            For iItems = 1 To 1000
                                'For iItems = 1 To 500
                                If ParmITEM(iOrder, iItems) > String.Empty Then
                                    '---------------------------------------------------------
                                    '   Call order detail stored procedure to write to Talent (ORDERREQ2)
                                    ' 
                                    cmdSelectORDERREQ2 = New iDB2Command(strSqlORDERREQ2, conChorus)
                                    Dim strItemLine As String = ParmITEM(iOrder, iItems)
                                    PARMOUT = String.Empty
                                    parmInput = cmdSelectORDERREQ2.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                    parmInput.Value = strTalentOrderNo & strItemLine.Substring(15)
                                    parmInput.Direction = ParameterDirection.Input
                                    Paramoutput = cmdSelectORDERREQ2.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                    Paramoutput.Value = String.Empty
                                    Paramoutput.Direction = ParameterDirection.InputOutput
                                    cmdSelectORDERREQ2.ExecuteNonQuery()
                                    PARMOUT = cmdSelectORDERREQ2.Parameters(Param2).Value.ToString
                                    If PARMOUT.Substring(1023, 1) = "Y" Then
                                        With err
                                            .ItemErrorMessage(iOrder) = String.Empty
                                            .ItemErrorCode(iOrder) = "TACDBOR-51"
                                            .ItemErrorStatus(iOrder) = "Invalid item line - " & PARMOUT.Substring(1019, 4) & "-" & _
                                                            Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4)) & _
                                                            " (" & strItemLine.Substring(15, 15).Trim & ")"
                                        End With
                                    End If
                                Else
                                    Exit For
                                End If
                            Next iItems
                            For iComments = 1 To 500
                                If ParmCOMM(iOrder, iComments) > String.Empty Then

                                    cmdSelectWRITETXT = New iDB2Command(strSqlWRITETXT, conChorus)
                                    PARMOUT = String.Empty
                                    parmInput = cmdSelectWRITETXT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                    If ParmCOMMLineNumber(iOrder, iComments) <> "" Then
                                        parmInput.Value = strTalentOrderNo & ParmCOMMLineNumber(iOrder, iComments).PadLeft(6, "0")
                                    Else
                                        parmInput.Value = strTalentOrderNo & "000000"
                                    End If

                                    parmInput.Direction = ParameterDirection.Input

                                    parmComment = cmdSelectWRITETXT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                    parmComment.Value = ParmCOMM(iOrder, iComments)
                                    parmComment.Direction = ParameterDirection.Input

                                    Paramoutput = cmdSelectWRITETXT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                                    Paramoutput.Value = String.Empty
                                    Paramoutput.Direction = ParameterDirection.InputOutput

                                    cmdSelectWRITETXT.ExecuteNonQuery()

                                    PARMOUT = cmdSelectWRITETXT.Parameters(Param3).Value.ToString
                                Else
                                    Exit For
                                End If
                            Next iComments

                            If err.ItemErrorCode(iOrder) = String.Empty Then

                                '-----------------------------------------------------------
                                '   Last item now written - if no error then write order to Chorus (WRITEORD)
                                ' 
                                cmdSelectWRITEORD = New iDB2Command(strSqlWRITEORD, conChorus)
                                Dim strWriteOrd As String = String.Empty
                                Dim strTotalOrderValue As String = (ParmMISC(iOrder).TotalOrderValue * 100).ToString("0000000000000000").PadLeft(16, "0")


                                PARMOUT = String.Empty
                                parmInput = cmdSelectWRITEORD.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                'Dim parmIn As String = Utilities.FixStringLength(Settings.AccountNo1, 15) & "  " & strTalentOrderNo & _
                                '                            ParmPAYMENT(iOrder, 1) & _
                                '                            ParmHEAD(iOrder).Substring(284, 15) & _
                                '                            ParmMISC(iOrder).PaymentType & _
                                '                            ParmMISC(iOrder).ProjectedDeliveryDate & _
                                '                            ParmHEAD(iOrder).Substring(107, 4) & _
                                '                            ParmMISC(iOrder).PromotionValue.ToString("0000000000000.00") & _
                                '                            ParmMISC(iOrder).TotalValueCharged.ToString("0000000000000.00") & _
                                '                            Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                '                            strTotalOrderValue & _
                                '                            Utilities.FixStringLength("", 801) & _
                                '                            Utilities.FixStringLength(Settings.LoginId, 12)

                                'parmInput.Value = Utilities.FixStringLength(parmIn, 1024)
                                parmInput.Value = Utilities.FixStringLength(Settings.AccountNo1, 8) & "  " & strTalentOrderNo & _
                                                                                           ParmPAYMENT(iOrder, 1) & _
                                                                                           ParmHEAD(iOrder).Substring(284, 15) & _
                                                                                           ParmMISC(iOrder).PaymentType & _
                                                                                           ParmMISC(iOrder).ProjectedDeliveryDate & _
                                                                                           ParmHEAD(iOrder).Substring(107, 4) & _
                                                                                           ParmMISC(iOrder).PromotionValue.ToString("0000000000000.00") & _
                                                                                           ParmMISC(iOrder).TotalValueCharged.ToString("0000000000000.00") & _
                                                                                           Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                                                                           strTotalOrderValue & _
                                                                                           Utilities.FixStringLength("", 801) & _
                                                                                           Utilities.FixStringLength(Settings.LoginId, 12)



                                parmInput.Direction = ParameterDirection.Input
                                Paramoutput = cmdSelectWRITEORD.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                Paramoutput.Value = String.Empty
                                Paramoutput.Direction = ParameterDirection.InputOutput
                                cmdSelectWRITEORD.CommandTimeout = 120
                                cmdSelectWRITEORD.ExecuteNonQuery()
                                PARMOUT = cmdSelectWRITEORD.Parameters(Param2).Value.ToString
                                '---------------------------------------------------------
                                '   If no error then create results data set
                                '
                                If PARMOUT.Substring(1023, 1).Equals("Y") Then
                                    Const strError2 As String = "Error writing to CHORUS"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError2
                                        .ErrorNumber = "TACDBOR-52"
                                        .HasError = True
                                    End With
                                Else
                                    ' This is the position it gets returned for Chorus Dist integration
                                    BackOfficeOrderNumber = PARMOUT.Substring(239, 7)
                                    If BackOfficeOrderNumber.Trim = String.Empty Then
                                        ' This is the position it gets returned for Chorus ERP integration
                                        BackOfficeOrderNumber = PARMOUT.Substring(0, 15)

                                    End If
                                End If
                            End If

                        End If

                    Else
                        Exit For
                    End If
                    If err.ItemErrorCode(iOrder) = String.Empty Then
                        '-------------------------------------------------------------
                        '   Read the order back out 
                        ' 
                        Dim err2 As ErrorObj = ReadOrder(False)
                    Else
                        '-------------------------------------------------------------
                        '   If error, add empty result set with Customer PO
                        '   for outbound XML doc
                        ' 
                        Dim deos As New DeOrders
                        Dim deoh As New DeOrderHeader

                        deos = Dep.CollDEOrders.Item(iOrder)
                        deoh = deos.DEOrderHeader
                        AddErrorResult(deoh)
                    End If

                Next iOrder

            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBOR-53"
                    .HasError = True
                    Return err
                End With
            End Try

        End If
        Return err
    End Function

    Private Function WriteDataBaseSystem21() As ErrorObj
        '---------------------------------------------------------------------------
        '   AccessDataBaseSystem21: Write the order to System 21 via Stored Procedures
        '
        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '---------------------------------------------------------------------------
        '   Open up collection and deal with by putting to stored procedures parameters 
        '
        Dim collDates As New Collection
        collDates.Add(Date.Now)
        err = DataEntityUnPackSystem21()
        If Not err.HasError Then
            '------------------------------------------------------------------
            ' Setup Calls to As400
            ' 
            Dim strSqlORDERREQ As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/ORDERREQ(@PARAM1, @PARAM2)"
            Dim strSqlORDERREQ2 As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/ORDERREQ2(@PARAM1, @PARAM2)"
            Dim strSqlWRITEORD As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/WRITEORD(@PARAM1, @PARAM2)"
            Dim strSqlWRITETXT As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/WRITETXT(@PARAM1,@PARAM2,@PARAM3)"

            Dim cmdSelectORDERREQ As iDB2Command = Nothing
            Dim cmdSelectORDERREQ2 As iDB2Command = Nothing
            Dim cmdSelectWRITEORD As iDB2Command = Nothing
            Dim cmdSelectWRITETXT As iDB2Command = Nothing

            Dim iItems As Integer = 0
            Dim iOrder As Integer = 0
            Dim iComments As Integer = 0

            Dim parmInput, Paramoutput, parmComment As iDB2Parameter
            Dim PARMOUT As String = String.Empty

            Dim strTalentOrderNo As String = String.Empty
            '----------------------------------------------------------------
            '   Loop through orders in transaction
            '
            Try
                'Throw New System.DivideByZeroException
                collDates.Add(Date.Now)
                For iOrder = 1 To 500

                    If ParmHEAD(iOrder) > String.Empty Then

                        '-----------------------------------------------------------------
                        ' Don't bother with any calls if already failed initial validation
                        ' (i.e. a supplynet pricing fail)
                        '-----------------------------------------------------------------
                        If ParmMISC(iOrder).OrderFailedInitialValidation Then
                            With err
                                .ItemErrorMessage(iOrder) = String.Empty
                                .ItemErrorCode(iOrder) = "TACDBOR-19"
                                .ItemErrorStatus(iOrder) = ParmMISC(iOrder).OrderFailedInitialValidationMessage
                            End With
                        Else

                            '----------------------------------------------------------------
                            '   Call order header stored procedure towrite to Talent (ORDERREQ)
                            '
                            cmdSelectORDERREQ = New iDB2Command(strSqlORDERREQ, conSystem21)

                            parmInput = cmdSelectORDERREQ.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                            parmInput.Value = ParmHEAD(iOrder)
                            parmInput.Direction = ParameterDirection.Input

                            Paramoutput = cmdSelectORDERREQ.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                            Paramoutput.Value = String.Empty
                            Paramoutput.Direction = ParameterDirection.InputOutput
                            cmdSelectORDERREQ.ExecuteNonQuery()
                            ' Interpret results
                            PARMOUT = cmdSelectORDERREQ.Parameters(Param2).Value.ToString
                            strTalentOrderNo = Utilities.FixStringLength(PARMOUT.Substring(0, 15), 15)
                            ' Error? 
                            If PARMOUT.Substring(1023, 1) = "Y" Then
                                With err
                                    .ItemErrorMessage(iOrder) = String.Empty
                                    .ItemErrorCode(iOrder) = "TACDBOR-16"
                                    .ItemErrorStatus(iOrder) = "Error creating order header - " & _
                                                        PARMOUT.Substring(1019, 4) & "-" & _
                                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                        "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                                End With

                            Else
                                '----------------------------------------------------------------
                                '   Loop through items in current order
                                ' 
                                For iItems = 1 To 1000
                                    'For iItems = 1 To 500
                                    If ParmITEM(iOrder, iItems) > String.Empty Then
                                        '---------------------------------------------------------
                                        '   Call order detail stored procedure to write to Talent (ORDERREQ2)
                                        ' 
                                        cmdSelectORDERREQ2 = New iDB2Command(strSqlORDERREQ2, conSystem21)
                                        Dim strItemLine As String = ParmITEM(iOrder, iItems)
                                        PARMOUT = String.Empty
                                        parmInput = cmdSelectORDERREQ2.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                        parmInput.Value = strTalentOrderNo & strItemLine.Substring(15)
                                        parmInput.Direction = ParameterDirection.Input
                                        Paramoutput = cmdSelectORDERREQ2.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                        Paramoutput.Value = String.Empty
                                        Paramoutput.Direction = ParameterDirection.InputOutput
                                        cmdSelectORDERREQ2.ExecuteNonQuery()
                                        PARMOUT = cmdSelectORDERREQ2.Parameters(Param2).Value.ToString
                                        If PARMOUT.Substring(1023, 1) = "Y" Then
                                            With err
                                                .ItemErrorMessage(iOrder) = String.Empty
                                                .ItemErrorCode(iOrder) = "TACDBOR-17"
                                                .ItemErrorStatus(iOrder) = "Invalid item line - " & PARMOUT.Substring(1019, 4) & "-" & _
                                                                Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4)) & _
                                                                " (" & strItemLine.Substring(15, 15).Trim & ")"
                                            End With
                                        End If
                                    Else
                                        Exit For
                                    End If
                                Next iItems
                                If err.ItemErrorCode(iOrder) = String.Empty Then
                                    '-----------------------------------------------------------
                                    '   Write order comment lines at header level (WRITETXT)
                                    '
                                    For iComments = 1 To 500
                                        If ParmCOMM(iOrder, iComments) > String.Empty Then

                                            cmdSelectWRITETXT = New iDB2Command(strSqlWRITETXT, conSystem21)
                                            PARMOUT = String.Empty
                                            parmInput = cmdSelectWRITETXT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                            parmInput.Value = strTalentOrderNo & "000000"
                                            parmInput.Direction = ParameterDirection.Input

                                            parmComment = cmdSelectWRITETXT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                            parmComment.Value = ParmCOMM(iOrder, iComments)
                                            parmComment.Direction = ParameterDirection.Input

                                            Paramoutput = cmdSelectWRITETXT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                                            Paramoutput.Value = String.Empty
                                            Paramoutput.Direction = ParameterDirection.InputOutput

                                            cmdSelectWRITETXT.ExecuteNonQuery()

                                            PARMOUT = cmdSelectWRITETXT.Parameters(Param3).Value.ToString
                                        Else
                                            Exit For
                                        End If
                                    Next iComments
                                    '-----------------------------------------------------------
                                    '   Last item now written - if no error then write order to System 21 (WRITEORD)
                                    ' 
                                    cmdSelectWRITEORD = New iDB2Command(strSqlWRITEORD, conSystem21)
                                    Dim strWriteOrd As String = String.Empty
                                    PARMOUT = String.Empty
                                    parmInput = cmdSelectWRITEORD.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                    parmInput.Value = Utilities.FixStringLength(Settings.AccountNo1, 8) & "  " & strTalentOrderNo & _
                                                                ParmPAYMENT(iOrder, 1) & _
                                                                ParmHEAD(iOrder).Substring(284, 15) & _
                                                                ParmMISC(iOrder).PaymentType & _
                                                                ParmMISC(iOrder).ProjectedDeliveryDate & _
                                                                ParmHEAD(iOrder).Substring(107, 4) & _
                                                                ParmMISC(iOrder).PromotionValue.ToString("0000000000000.00") & _
                                                                ParmMISC(iOrder).TotalValueCharged.ToString("0000000000000.00") & _
                                                                Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                                                   ParmMISC(iOrder).TotalOrderValue.ToString("0000000000000.00")


                                    parmInput.Direction = ParameterDirection.Input
                                    Paramoutput = cmdSelectWRITEORD.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                    Paramoutput.Value = String.Empty
                                    Paramoutput.Direction = ParameterDirection.InputOutput
                                    cmdSelectWRITEORD.ExecuteNonQuery()
                                    PARMOUT = cmdSelectWRITEORD.Parameters(Param2).Value.ToString
                                    '---------------------------------------------------------
                                    '   If no error then create results data set
                                    '
                                    If PARMOUT.Substring(1023, 1).Equals("Y") Then
                                        Const strError2 As String = "Error writing to System 21"
                                        With err
                                            .ErrorMessage = String.Empty
                                            .ErrorStatus = strError2
                                            .ErrorNumber = "TACDBOR02"
                                            .HasError = True
                                        End With
                                    Else
                                        '   these two get overwritten on each loop ???
                                        '
                                        System21CompanyNo = PARMOUT.Substring(237, 2)
                                        System21OrderNo = PARMOUT.Substring(0, 15).Trim
                                    End If
                                End If

                            End If
                        End If

                    Else
                        Exit For
                    End If
                    If err.ItemErrorCode(iOrder) = String.Empty Then
                        '-------------------------------------------------------------
                        '   Read the order back out 
                        ' 
                        Dim err2 As ErrorObj = ReadOrder(False)
                    Else
                        '-------------------------------------------------------------
                        '   If error, add empty result set with Customer PO
                        '   for outbound XML doc
                        ' 
                        Dim deos As New DeOrders
                        Dim deoh As New DeOrderHeader

                        deos = Dep.CollDEOrders.Item(iOrder)
                        deoh = deos.DEOrderHeader
                        AddErrorResult(deoh)
                    End If

                Next iOrder

            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBOR03"
                    .HasError = True
                    Return err
                End With
            End Try

        End If
        Return err
    End Function

    Private Function WriteDataBaseADL() As ErrorObj

        Dim err As New ErrorObj

        err = DataEntityUnPackADL()

        If Not err.HasError Then

            ' Setup SQL Select Statements
            Dim sbSqlXXCOP100 As New StringBuilder
            With sbSqlXXCOP100
                .Append("INSERT INTO XXCOP100 VALUES (")
                .Append("@C1CUS, ")
                .Append("@C1USR, ")
                .Append("@C1TIT, ")
                .Append("@C1FNM, ")
                .Append("@C1SNM, ")
                .Append("@C1NAM, ")
                .Append("@C1AD1, ")
                .Append("@C1AD2, ")
                .Append("@C1AD3, ")
                .Append("@C1AD4, ")
                .Append("@C1AD5, ")
                .Append("@C1PCD, ")
                .Append("@C1CCD, ")
                .Append("@C1RLF, ")
                .Append("@C1PRF, ")
                .Append("@C1ORD, ")
                .Append("@C1ODT, ")
                .Append("@C1SCD, ")
                .Append("@C1SCS, ")
                .Append("@C1SHP, ")
                .Append("@C1COR, ")
                .Append("@C1CDT, ")
                .Append("@C1OTP, ")
                .Append("@C1WHS, ")
                .Append("@C1TX1, ")
                .Append("@C1TX2, ")
                .Append("@C1PRM, ")
                .Append("@C1PCS, ")
                .Append("@C1ORM, ")
                .Append("@C1POA, ")
                .Append("@C1OAP, ")
                .Append("@C1CUR, ")
                .Append("@C1PYT, ")
                .Append("@C1STS, ")
                .Append("@C1BOR, ")
                .Append("@C1TEL, ")
                .Append("@C1PON, ")
                .Append("@C1RAF, ")
                .Append("@C1PTP, ")
                .Append("@C1CTP, ")
                .Append("@C1NVL, ")
                .Append("@C1TVL, ")
                .Append("@C1GVL, ")
                .Append("@C1POD, ")
                .Append("@C1CCR, ")
                .Append("@C1DVL, ")
                .Append("@C1SCT, ")
                .Append("@C1SCN, ")
                .Append("@C1T1C, ")
                .Append("@C1T1A, ")
                .Append("@C1T2C, ")
                .Append("@C1T2A, ")
                .Append("@C1T3C, ")
                .Append("@C1T3A, ")
                .Append("@C1T4C, ")
                .Append("@C1T4A, ")
                .Append("@C1T5C, ")
                .Append("@C1T5A, ")
                .Append("@C1LAN, ")
                .Append("@C1ULN")
                .Append(")")
            End With
            Dim strSqlXXCOP100 As String = sbSqlXXCOP100.ToString

            Dim sbSqlXXCOP200 As New StringBuilder
            With sbSqlXXCOP200
                .Append("INSERT INTO XXCOP200 VALUES (")
                .Append("@C2CUS, ")
                .Append("@C2SHP, ")
                .Append("@C2USR, ")
                .Append("@C2CTL, ")
                .Append("@C2GRP, ")
                .Append("@C2CAT, ")
                .Append("@C2SUB, ")
                .Append("@C2PRD, ")
                .Append("@C2DSC, ")
                .Append("@C2QTY, ")
                .Append("@C2PRC, ")
                .Append("@C2ORD, ")
                .Append("@C2LIN, ")
                .Append("@C2PRF, ")
                .Append("@C2SPC, ")
                .Append("@C2WGT, ")
                .Append("@C2LTR, ")
                .Append("@C2NAM, ")
                .Append("@C2ORG, ")
                .Append("@C2AD1, ")
                .Append("@C2AD2, ")
                .Append("@C2AD3, ")
                .Append("@C2AD4, ")
                .Append("@C2AD5, ")
                .Append("@C2PCD, ")
                .Append("@C2CCD, ")
                .Append("@C2CUR, ")
                .Append("@C2UOM, ")
                .Append("@C2TEL, ")
                .Append("@C2EMA, ")
                .Append("@C2CPR, ")
                .Append("@C2RAF, ")
                .Append("@C2POD, ")
                .Append("@C2NVL, ")
                .Append("@C2TVL, ")
                .Append("@C2GVL, ")
                .Append("@C2IVL, ")
                .Append("@C2ITV, ")
                .Append("@C2DVL, ")
                .Append("@C2T1C, ")
                .Append("@C2T1A, ")
                .Append("@C2T2C, ")
                .Append("@C2T2A, ")
                .Append("@C2T3C, ")
                .Append("@C2T3A, ")
                .Append("@C2T4C, ")
                .Append("@C2T4A, ")
                .Append("@C2T5C, ")
                .Append("@C2T5A, ")
                .Append("@C2ULN")
                .Append(")")
            End With
            Dim strSqlXXCOP200 As String = sbSqlXXCOP200.ToString

            Dim sbSqlXXCOP400 As New StringBuilder
            With sbSqlXXCOP400
                .Append("INSERT INTO XXCOP400 VALUES (")
                .Append("@C4CUS, ")
                .Append("@C4ORD, ")
                .Append("@C4GFM, ")
                .Append("@C4A11, ")
                .Append("@C4A12, ")
                .Append("@C4A13, ")
                .Append("@C4A14, ")
                .Append("@C4A15, ")
                .Append("@C4A16, ")
                .Append("@C4A17, ")
                .Append("@C4A18, ")
                .Append("@C4A19, ")
                .Append("@C4A10, ")
                .Append("@C4A21, ")
                .Append("@C4A22, ")
                .Append("@C4A23, ")
                .Append("@C4A24, ")
                .Append("@C4A25, ")
                .Append("@C4A26, ")
                .Append("@C4A27, ")
                .Append("@C4A28, ")
                .Append("@C4A29, ")
                .Append("@C4A20, ")
                .Append("@C4A51, ")
                .Append("@C4A52, ")
                .Append("@C4A53, ")
                .Append("@C4A54, ")
                .Append("@C4A55, ")
                .Append("@C4A56, ")
                .Append("@C4A57, ")
                .Append("@C4A58, ")
                .Append("@C4A59, ")
                .Append("@C4A50, ")
                .Append("@C4A61, ")
                .Append("@C4A62, ")
                .Append("@C4N11, ")
                .Append("@C4N12, ")
                .Append("@C4N13, ")
                .Append("@C4N14, ")
                .Append("@C4N15, ")
                .Append("@C4N91, ")
                .Append("@C4N92, ")
                .Append("@C4N93, ")
                .Append("@C4N94, ")
                .Append("@C4N95, ")
                .Append("@C4N96, ")
                .Append("@C4N97, ")
                .Append("@C4N98, ")
                .Append("@C4N99, ")
                .Append("@C4N90, ")
                .Append("@C4ULN")
                .Append(")")
            End With
            Dim strSqlXXCOP400 As String = sbSqlXXCOP400.ToString

            Dim cmdSelectXXCOP100 As iDB2Command = Nothing
            Dim cmdSelectXXCOP200 As iDB2Command = Nothing
            Dim cmdSelectXXCOP400 As iDB2Command = Nothing

            Dim iOrder As Integer = 0
            Dim iItems As Integer = 0

            ' Loop through orders in transaction
            Try

                For iOrder = 1 To 500
                    If Not ParmHEAD2(iOrder) Is Nothing Then

                        ' Write Order Header XXCOP100/XXCOP400
                        cmdSelectXXCOP100 = New iDB2Command(strSqlXXCOP100, conADL)
                        With cmdSelectXXCOP100.Parameters
                            .Add("@C1CUS", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("CustomerNumber")
                            .Add("@C1USR", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("CustomerNumber")
                            .Add("@C1TIT", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("Title")
                            .Add("@C1FNM", iDB2DbType.iDB2VarChar, 30).Value = ParmHEAD2(iOrder).Item("Forename")
                            .Add("@C1SNM", iDB2DbType.iDB2VarChar, 30).Value = ParmHEAD2(iOrder).Item("Surname")
                            .Add("@C1NAM", iDB2DbType.iDB2VarChar, 40).Value = ""
                            .Add("@C1AD1", iDB2DbType.iDB2VarChar, 40).Value = ParmHEAD2(iOrder).Item("billAddressLine1")
                            .Add("@C1AD2", iDB2DbType.iDB2VarChar, 40).Value = ParmHEAD2(iOrder).Item("billAddressLine2")
                            .Add("@C1AD3", iDB2DbType.iDB2VarChar, 40).Value = ParmHEAD2(iOrder).Item("billAddressLine3")
                            .Add("@C1AD4", iDB2DbType.iDB2VarChar, 25).Value = ParmHEAD2(iOrder).Item("billAddressCity")
                            .Add("@C1AD5", iDB2DbType.iDB2VarChar, 25).Value = ParmHEAD2(iOrder).Item("billAddressProvince")
                            .Add("@C1PCD", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("billAddressPostalCode")
                            .Add("@C1CCD", iDB2DbType.iDB2VarChar, 3).Value = ParmHEAD2(iOrder).Item("billAddressCountry")
                            .Add("@C1RLF", iDB2DbType.iDB2VarChar, 1).Value = "N"
                            .Add("@C1PRF", iDB2DbType.iDB2VarChar, 1).Value = "N"
                            .Add("@C1ORD", iDB2DbType.iDB2VarChar, 15).Value = ParmHEAD2(iOrder).Item("webOrderNumber")
                            .Add("@C1ODT", iDB2DbType.iDB2Date).Value = Date.Now
                            .Add("@C1SCD", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("carrierCode")
                            .Add("@C1SCS", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("carrierCodeValue")
                            .Add("@C1SHP", iDB2DbType.iDB2VarChar, 10).Value = ""
                            .Add("@C1COR", iDB2DbType.iDB2VarChar, 15).Value = ""
                            .Add("@C1CDT", iDB2DbType.iDB2Date).Value = ParmHEAD2(iOrder).Item("projectedDeliveryDate")
                            .Add("@C1OTP", iDB2DbType.iDB2VarChar, 3).Value = "WEB"
                            .Add("@C1WHS", iDB2DbType.iDB2VarChar, 2).Value = ""
                            .Add("@C1TX1", iDB2DbType.iDB2VarChar, 30).Value = ParmHEAD2(iOrder).Item("BasketNumber").ToString.PadRight(15, " ") & BoolToYN(ParmHEAD2(iOrder).Item("TaxInclusive1")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxInclusive2")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxInclusive3")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxInclusive4")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxInclusive5")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxDisplay1")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxDisplay2")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxDisplay3")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxDisplay4")) _
                                                                                                & BoolToYN(ParmHEAD2(iOrder).Item("TaxDisplay5"))
                            .Add("@C1TX2", iDB2DbType.iDB2VarChar, 30).Value = ParmHEAD2(iOrder).Item("PromotionDescription")
                            .Add("@C1PRM", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("PromotionCode")
                            .Add("@C1PCS", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C1ORM", iDB2DbType.iDB2VarChar, 5).Value = "WEB"
                            .Add("@C1POA", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C1OAP", iDB2DbType.iDB2VarChar, 1).Value = "N"
                            .Add("@C1CUR", iDB2DbType.iDB2VarChar, 3).Value = ParmHEAD2(iOrder).Item("Currency")
                            .Add("@C1PYT", iDB2DbType.iDB2VarChar, 3).Value = ""
                            ' Check whether order requires review
                            If ParmHEAD2(iOrder).Item("SuspendCode").ToString.Trim = "HS" Then
                                .Add("@C1STS", iDB2DbType.iDB2VarChar, 2).Value = "02"
                            Else
                                .Add("@C1STS", iDB2DbType.iDB2VarChar, 2).Value = "01"
                            End If
                            .Add("@C1STS", iDB2DbType.iDB2VarChar, 2).Value = "01"
                            .Add("@C1BOR", iDB2DbType.iDB2VarChar, 15).Value = ""
                            .Add("@C1TEL", iDB2DbType.iDB2VarChar, 30).Value = ParmHEAD2(iOrder).Item("TelNo")
                            .Add("@C1PON", iDB2DbType.iDB2VarChar, 15).Value = ""
                            .Add("@C1RAF", iDB2DbType.iDB2VarChar, 15).Value = ""
                            .Add("@C1PTP", iDB2DbType.iDB2VarChar, 15).Value = "Card"
                            Dim s As String = ParmHEAD2(iOrder).Item("CreditCardType")
                            .Add("@C1CTP", iDB2DbType.iDB2VarChar, 30).Value = ParmHEAD2(iOrder).Item("CreditCardType")
                            ' Add delivery values 
                            .Add("@C1NVL", iDB2DbType.iDB2Decimal).Value = CDec(ParmHEAD2(iOrder).Item("totalOrderItemsValueNet")) + CDec(ParmHEAD2(iOrder).Item("CarrierCodeNet"))
                            .Add("@C1TVL", iDB2DbType.iDB2Decimal).Value = CDec(ParmHEAD2(iOrder).Item("totalOrderItemsValueTax")) '' + CDec(ParmHEAD2(iOrder).Item("CarrierCodeVat"))
                            .Add("@C1GVL", iDB2DbType.iDB2Decimal).Value = CDec(ParmHEAD2(iOrder).Item("totalOrderItemsValue")) + CDec(ParmHEAD2(iOrder).Item("CarrierCodeValue"))
                            '.Add("@C1NVL", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("totalOrderItemsValueNet")
                            '.Add("@C1TVL", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("totalOrderItemsValueTax")
                            '.Add("@C1GVL", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("totalOrderItemsValue")
                            .Add("@C1POD", iDB2DbType.iDB2VarChar, 30).Value = ""
                            .Add("@C1CCR", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C1DVL", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C1SCT", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("carrierCodeVAT")
                            .Add("@C1SCN", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("carrierCodeNet")
                            .Add("@C1T1C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode1")
                            .Add("@C1T1A", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("TaxAmount1")
                            .Add("@C1T2C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode2")
                            .Add("@C1T2A", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("TaxAmount2")
                            .Add("@C1T3C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode3")
                            .Add("@C1T3A", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("TaxAmount3")
                            .Add("@C1T4C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode4")
                            .Add("@C1T4A", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("TaxAmount4")
                            .Add("@C1T5C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode5")
                            .Add("@C1T5A", iDB2DbType.iDB2Decimal).Value = ParmHEAD2(iOrder).Item("TaxAmount5")
                            .Add("@C1LAN", iDB2DbType.iDB2VarChar, 3).Value = "ENG"
                            .Add("@C1ULN", iDB2DbType.iDB2VarChar, 3).Value = "ENG"
                        End With
                        cmdSelectXXCOP100.ExecuteNonQuery()

                        cmdSelectXXCOP400 = New iDB2Command(strSqlXXCOP400, conADL)
                        With cmdSelectXXCOP400.Parameters
                            .Add("@C4CUS", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("CustomerNumber")
                            .Add("@C4ORD", iDB2DbType.iDB2VarChar, 15).Value = ParmHEAD2(iOrder).Item("webOrderNumber")
                            .Add("@C4GFM", iDB2DbType.iDB2VarChar, 1).Value = BoolToYN(ParmHEAD2(iOrder).Item("Message"))
                            .Add("@C4A11", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A12", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A13", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A14", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A15", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A16", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A17", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A18", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A19", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A10", iDB2DbType.iDB2VarChar, 1).Value = ""
                            .Add("@C4A21", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A22", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A23", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A24", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A25", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A26", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A27", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A28", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A29", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A20", iDB2DbType.iDB2VarChar, 20).Value = ""
                            .Add("@C4A51", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A52", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A53", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A54", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A55", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A56", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A57", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A58", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A59", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A50", iDB2DbType.iDB2VarChar, 50).Value = ""
                            .Add("@C4A61", iDB2DbType.iDB2VarChar, 256).Value = ""
                            .Add("@C4A62", iDB2DbType.iDB2VarChar, 256).Value = ""
                            .Add("@C4N11", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N12", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N13", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N14", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N15", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N91", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N92", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N93", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N94", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N95", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N96", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N97", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N98", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N99", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4N90", iDB2DbType.iDB2Decimal).Value = 0
                            .Add("@C4ULN", iDB2DbType.iDB2VarChar, 3).Value = "ENG"
                        End With
                        cmdSelectXXCOP400.ExecuteNonQuery()
                        ' Loop through items in current order
                        For iItems = 1 To 500
                            If Not ParmITEM2(iOrder, iItems) Is Nothing Then

                                ' Write Order Detail XXCOP200
                                cmdSelectXXCOP200 = New iDB2Command(strSqlXXCOP200, conADL)
                                With cmdSelectXXCOP200.Parameters
                                    .Add("@C2CUS", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("CustomerNumber")
                                    .Add("@C2SHP", iDB2DbType.iDB2VarChar, 10).Value = ""
                                    .Add("@C2USR", iDB2DbType.iDB2VarChar, 10).Value = ParmHEAD2(iOrder).Item("CustomerNumber")
                                    .Add("@C2CTL", iDB2DbType.iDB2VarChar, 3).Value = "ADE"
                                    .Add("@C2GRP", iDB2DbType.iDB2VarChar, 15).Value = ""
                                    .Add("@C2CAT", iDB2DbType.iDB2VarChar, 15).Value = ""
                                    .Add("@C2SUB", iDB2DbType.iDB2VarChar, 15).Value = ""
                                    .Add("@C2PRD", iDB2DbType.iDB2VarChar, 15).Value = ParmITEM2(iOrder, iItems).Item("SKU")
                                    .Add("@C2DSC", iDB2DbType.iDB2VarChar, 50).Value = ParmITEM2(iOrder, iItems).Item("ProductDescription")
                                    .Add("@C2QTY", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("quantity")
                                    .Add("@C2PRC", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("UnitPrice")
                                    .Add("@C2ORD", iDB2DbType.iDB2VarChar, 15).Value = ParmHEAD2(iOrder).Item("webOrderNumber")
                                    .Add("@C2LIN", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("LineNumber")
                                    .Add("@C2PRF", iDB2DbType.iDB2VarChar, 1).Value = "N"
                                    .Add("@C2SPC", iDB2DbType.iDB2VarChar, 30).Value = ""
                                    .Add("@C2WGT", iDB2DbType.iDB2Decimal).Value = 0
                                    .Add("@C2LTR", iDB2DbType.iDB2Decimal).Value = 0
                                    .Add("@C2NAM", iDB2DbType.iDB2VarChar, 40).Value = ParmITEM2(iOrder, iItems).Item("shipContactName")
                                    .Add("@C2ORG", iDB2DbType.iDB2VarChar, 40).Value = ""
                                    .Add("@C2AD1", iDB2DbType.iDB2VarChar, 40).Value = ParmITEM2(iOrder, iItems).Item("shipAddressLine1")
                                    .Add("@C2AD2", iDB2DbType.iDB2VarChar, 40).Value = ParmITEM2(iOrder, iItems).Item("shipAddressLine2")
                                    .Add("@C2AD3", iDB2DbType.iDB2VarChar, 40).Value = ParmITEM2(iOrder, iItems).Item("shipAddressLine3")
                                    .Add("@C2AD4", iDB2DbType.iDB2VarChar, 25).Value = ParmITEM2(iOrder, iItems).Item("shipCity")
                                    .Add("@C2AD5", iDB2DbType.iDB2VarChar, 25).Value = ParmITEM2(iOrder, iItems).Item("shipProvince")
                                    .Add("@C2PCD", iDB2DbType.iDB2VarChar, 10).Value = ParmITEM2(iOrder, iItems).Item("shipPostalCode")
                                    .Add("@C2CCD", iDB2DbType.iDB2VarChar, 3).Value = ParmITEM2(iOrder, iItems).Item("shipCountry")
                                    .Add("@C2CUR", iDB2DbType.iDB2VarChar, 3).Value = ParmITEM2(iOrder, iItems).Item("Currency")
                                    .Add("@C2UOM", iDB2DbType.iDB2VarChar, 3).Value = "EA"
                                    .Add("@C2TEL", iDB2DbType.iDB2VarChar, 30).Value = ParmITEM2(iOrder, iItems).Item("TelNo")
                                    .Add("@C2EMA", iDB2DbType.iDB2VarChar, 50).Value = ParmITEM2(iOrder, iItems).Item("shipEmail")
                                    .Add("@C2CPR", iDB2DbType.iDB2VarChar, 15).Value = ""
                                    .Add("@C2RAF", iDB2DbType.iDB2VarChar, 15).Value = ""
                                    .Add("@C2POD", iDB2DbType.iDB2VarChar, 30).Value = ""
                                    .Add("@C2NVL", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("NetLineValue")
                                    .Add("@C2TVL", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("TaxLineValue")
                                    .Add("@C2GVL", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("GrossLineValue")
                                    .Add("@C2IVL", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("UnitPrice")
                                    .Add("@C2ITV", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("ProductTaxValue")
                                    .Add("@C2DVL", iDB2DbType.iDB2Decimal).Value = 0
                                    .Add("@C2T1C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode1")
                                    .Add("@C2T1A", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("TaxAmount1")
                                    .Add("@C2T2C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode2")
                                    .Add("@C2T2A", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("TaxAmount2")
                                    .Add("@C2T3C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode3")
                                    .Add("@C2T3A", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("TaxAmount3")
                                    .Add("@C2T4C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode4")
                                    .Add("@C2T4A", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("TaxAmount4")
                                    .Add("@C2T5C", iDB2DbType.iDB2VarChar, 5).Value = ParmHEAD2(iOrder).Item("TaxCode5")
                                    .Add("@C2T5A", iDB2DbType.iDB2Decimal).Value = ParmITEM2(iOrder, iItems).Item("TaxAmount5")
                                    .Add("@C2ULN", iDB2DbType.iDB2VarChar, 5).Value = "ENG"
                                End With
                                cmdSelectXXCOP200.ExecuteNonQuery()
                            Else
                                Exit For
                            End If
                        Next iItems

                    Else
                        Exit For
                    End If
                Next iOrder

            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBOR04"
                    .HasError = True
                    Return err
                End With
            End Try

        End If
        Return err
    End Function

    Private Function WriteDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim conTalent As SqlConnection = Nothing
        '--------------------------------------------------------------------
        Try
            conTalent = New SqlConnection(Settings.BackOfficeConnectionString)
            conTalent.Open()
        Catch ex As Exception
            Const strError5 As String = "Could not establish connection to the database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError5
                .ErrorNumber = "TACDBOR05"
                .HasError = True
                Return err
            End With
        End Try
        '----------------------------------------------------------------------------------
        Const strSql As String = "CALL WESTCOAST.ORDERINSERT(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6)"
        Dim cmdSelect As SqlCommand = Nothing
        '
        Dim parmTRANRANSACTION, parmHEADER, parmADDRESS, parmITEMLINE, parmCOMMENT, Paramoutput As SqlParameter
        Dim iOrder As Integer = 0
        Dim iAddress As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0
        '
        Try
            cmdSelect = New SqlCommand(strSql, conTalent)
            parmTRANRANSACTION = cmdSelect.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmTRANRANSACTION.Value = ParmTRAN
            parmTRANRANSACTION.Direction = ParameterDirection.Input
            For iOrder = 1 To 10

                If ParmHEAD(iOrder).Length > 0 Then
                    parmHEADER = cmdSelect.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    parmHEADER.Value = ParmHEAD(iOrder)
                    parmHEADER.Direction = ParameterDirection.Input
                    '-----------------------------------------------------------------------------------
                    For iAddress = 1 To 10
                        If ParmADDR(iOrder, iAddress).Length > 0 Then
                            parmADDRESS = cmdSelect.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                            parmADDRESS.Value = ParmADDR(iOrder, iAddress)
                            parmADDRESS.Direction = ParameterDirection.Input
                        Else
                            Exit For
                        End If
                    Next iAddress
                    '-----------------------------------------------------------------------------------
                    For iItems = 1 To 100
                        If ParmITEM(iOrder, iItems).Length > 0 Then
                            parmITEMLINE = cmdSelect.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1024)
                            parmITEMLINE.Value = ParmITEM(iOrder, iItems)
                            parmITEMLINE.Direction = ParameterDirection.Input
                        Else
                            Exit For
                        End If
                    Next iItems
                    '-----------------------------------------------------------------------------------
                    For iComments = 1 To 100
                        If ParmCOMM(iOrder, iComments).Length > 0 Then
                            parmCOMMENT = cmdSelect.Parameters.Add(Param5, iDB2DbType.iDB2Char, 1024)
                            parmCOMMENT.Value = ParmCOMM(iOrder, iComments)
                            parmCOMMENT.Direction = ParameterDirection.Input
                        Else
                            Exit For
                        End If
                    Next iComments
                    '-----------------------------------------------------------------------------------
                Else
                    Exit For
                End If
            Next iOrder
            '------------------------------------------------------------------------------
            '   Add the Outbound parameters
            '
            Paramoutput = cmdSelect.Parameters.Add(Param6, iDB2DbType.iDB2Char, 1024)
            Paramoutput.Value = "Outgoing"
            Paramoutput.Direction = ParameterDirection.Output
            '
            cmdSelect.ExecuteNonQuery()
            '
        Catch ex As Exception
            Const strError6 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError6
                .ErrorNumber = "TACDBOR06"
                .HasError = True
                Return err
            End With
        End Try
        '--------------------------------------------------------------------
        '   Close
        '
        Try
            conTalent.Close()
        Catch ex As Exception
            Const strError7 As String = "Failed to close database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError7
                .ErrorNumber = "TACDBOR07"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function

    Public Function ReadOrder(Optional ByVal opendb As Boolean = True) As ErrorObj
        Dim err As New ErrorObj
        '-------------------------------------------------------------------
        If DtHeader.Columns.Count = 0 Then DtHeader_Columns()
        If DtDetail.Columns.Count = 0 Then DtDetail_Columns()
        If DtText.Columns.Count = 0 Then DtText_Columns()
        If DtCarrier.Columns.Count = 0 Then DtCarrier_Columns()
        If DtPackage.Columns.Count = 0 Then DtPackage_Columns()
        If DtProduct.Columns.Count = 0 Then DtProduct_Columns()
        '-------------------------------------------------------------------
        If ResultDataSet Is Nothing Then
            ResultDataSet = New DataSet
            ResultDataSet.Tables.Add(DtHeader)      ' 0
            ResultDataSet.Tables.Add(DtDetail)      ' 1
            ResultDataSet.Tables.Add(DtText)        ' 2
            ResultDataSet.Tables.Add(DtCarrier)     ' 3
            ResultDataSet.Tables.Add(DtPackage)     ' 4
            ResultDataSet.Tables.Add(DtProduct)     ' 5
            '
            ResultDataSet.Tables.Item(0).TableName = "DtHeader"
            ResultDataSet.Tables.Item(1).TableName = "DtDetail"
            ResultDataSet.Tables.Item(2).TableName = "DtText"
            ResultDataSet.Tables.Item(3).TableName = "DtCarrier"
            ResultDataSet.Tables.Item(4).TableName = "DtPackage"
            ResultDataSet.Tables.Item(5).TableName = "DtProduct"
        End If
        '-------------------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError() Then
                    err = ReadOrderSystem21()
                End If
                If opendb Then System21Close()
            Case Is = SQL2005
                'err = AccessDataBaseSQL2005()
            Case Is = CHORUS
                If opendb Then err = ChorusOpen()
                If Not err.HasError() Then
                    err = ReadOrderChorus()
                End If
                If opendb Then ChorusClose()
        End Select
        Return err
    End Function
    ''Public Function ReadOrderDetail() As ErrorObj
    ''    Dim err As ErrorObj = Nothing
    ''    '--------------------------------------------------------------------------
    ''    Dim detr As New DETransaction           ' Items
    ''    Dim deos As New DeOrders                ' DeOrderHeader as collections
    ''    Dim deoh As New DeOrderHeader           ' Items  as Collection
    ''    Dim iOrder As Integer = 0

    ''    detr = Dep.CollDETrans.Item(1)
    ''    System21CompanyNo = detr.Company

    ''    For iOrder = 1 To Dep.CollDEOrders.Count
    ''        '----------------------------------------------------------------------
    ''        deos = Dep.CollDEOrders.Item(iOrder)
    ''        deoh = deos.DEOrderHeader
    ''        System21OrderNo = deoh.CustomerPO
    ''        err = ReadOrderSystem21()

    ''    Next iOrder

    ''    Return err
    ''End Function

    Private Function DataEntityUnPackSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
        '-------------------------------------------------------------------------------------
        Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
        Dim dead As New DeAddress               ' Items
        Dim deop As New DEPayments              ' Items
        Dim deoc As New DECharges               ' Items
        ' 
        Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
        Dim depr As DeProductLines              ' Items
        Dim decl As DeCommentLines = Nothing    ' Items

        Dim iOrder As Integer = 0
        Dim iAddress As Integer = 0
        Dim iPayment As Integer = 0
        Dim iCharge As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0
        Dim orderFailedInitialValidation As Boolean = False
        Dim orderFailedInitialValidationMessage As String = String.Empty
        '
        Dim detr As New DETransaction           ' Items
        Try
            With Dep
                detr = .CollDETrans.Item(1)
                '-----------------------------------------------------------------------------------------
                For iOrder = 1 To .CollDEOrders.Count
                    orderFailedInitialValidation = False
                    orderFailedInitialValidationMessage = String.Empty
                    _orderLineNo = 1
                    '
                    deos = Dep.CollDEOrders.Item(iOrder)
                    deoh = deos.DEOrderHeader
                    '-------------------------------------------------------------------------------------
                    '   May have one or more adresses, such as Shipping and Billing, most cases
                    '   they will both be the same.
                    '
                    For iAddress = 1 To deoh.CollDEAddress.Count
                        dead = deoh.CollDEAddress.Item(iAddress)
                        With dead

                        End With
                    Next iAddress
                    '-------------------------------------------------------------------------------------
                    '   May have one or more payemnts, such as CC and SO, most cases
                    '   they will be one.
                    '
                    For iPayment = 1 To deoh.CollDEPayments.Count
                        deop = deoh.CollDEPayments.Item(iPayment)
                        With deop
                            '--------------------------------------------------
                            ' Validate card number / Start and Expiry Date
                            '
                            If deop.PaymentType.Equals("CC") Then
                                '----------------------------------------
                                If Not Utilities.ValidateCVV(deop.CV2Number) Then
                                    Const strError13 As String = "Invalid CV2 number"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError13
                                        .ErrorNumber = "TACDBOR13"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                                If Not Utilities.ValidateCardNumber(deop.CardNumber) Then
                                    Const strError14 As String = "Invalid Card number"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError14
                                        .ErrorNumber = "TACDBOR14"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                                If Not Utilities.ValidateExpiryDate(deop.ExpiryDate) Then
                                    Const strError15 As String = "Invalid Card Expiry Date"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError15
                                        .ErrorNumber = "TACDBOR15"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                                If Not Utilities.ValidateStartDate(deop.StartDate) Then
                                    Const strError16 As String = "Invalid Card Start Date"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError16
                                        .ErrorNumber = "TACDBOR16"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                            End If
                        End With

                        If Not err.HasError Then
                            ParmPAYMENT(iOrder, iPayment) = BuildParmPAYMENT( _
                                                deop.CardNumber, deop.ExpiryDate, _
                                                deop.StartDate, deop.IssueNumber, _
                                                deop.CV2Number, deop.CardpostCode, _
                                                deop.CardAddress, deop.CardType)
                        End If

                    Next iPayment
                    '-------------------------------------------------------------------------------------
                    '   May have one or more Charges, most cases they will be one.
                    '
                    For iCharge = 1 To deoh.CollDECharges.Count
                        deoc = deoh.CollDECharges.Item(iCharge)
                        With deoc
                            ParmCHGE(iOrder, iCharge) = .Charge
                        End With
                    Next iCharge
                    If Settings.AccountNo1.Length > 8 Then
                        Settings.AccountNo1 = Settings.AccountNo1.Substring(0, 8)
                    End If
                    If Settings.AccountNo2.Length > 3 Then
                        Settings.AccountNo2 = Settings.AccountNo2.Substring(0, 3)
                    End If
                    ParmHEAD(iOrder) = BuildParmHEAD(Settings.AccountNo1, Settings.AccountNo2, deoh, "", deoh.CollDECharges, deoh.CollDEAddress(2), Settings.AccountNo3, Settings.LoginId)
                    '-------------------------------------------------------------------------------------
                    '   May have one or more products 
                    '
                    deoi = deos.DEOrderInfo
                    For iItems = 1 To deoi.CollDEProductLines.Count
                        depr = deoi.CollDEProductLines.Item(iItems)
                        With depr
                            .Quantity = (.Quantity.ToString).PadLeft(9, "0")
                            .CustomerLineNumber = .CustomerLineNumber.PadLeft(6, "0")
                            ParmITEM(iOrder, iItems) = BuildParmITEM("", depr, Settings.AccountNo1, _
                                                            Settings.AccountNo2, deoh.CollDEAddress(1), Settings.AccountNo3)
                        End With
                        '-------------------------------------------
                        ' Check if the line has a problem which must 
                        ' cause the whole order to fail. Initially 
                        ' this can be just a supplynet price fail
                        '-------------------------------------------
                        If depr.LineError Then
                            orderFailedInitialValidation = True
                            orderFailedInitialValidationMessage = depr.SKU & " : " & depr.LineErrorMessage
                        End If
                    Next
                    '------------------------------------------------------------
                    ' May have one or more comments 
                    '
                    For iComments = 1 To deoi.CollDECommentLines.Count
                        decl = deoi.CollDECommentLines.Item(iComments)
                        With decl
                            ParmCOMM(iOrder, iComments) = .CommentText
                            ParmCOMMLineNumber(iOrder, iComments) = .CommentLineNumber
                            '              " CommentText = {0}" & _
                            '              ", CommentLine = {1}", _
                            '              .CommentText, _
                            '             .CommentLine)
                        End With
                    Next iComments
                    '--------------------------------
                    ' Additional items..
                    '--------------------------------
                    Dim parmDeliveryDate As String = deoh.ProjectedDeliveryDate.Year.ToString.PadLeft(4, "0") & _
                                                          deoh.ProjectedDeliveryDate.Month.ToString.PadLeft(2, "0") & _
                                                         deoh.ProjectedDeliveryDate.Day.ToString.PadLeft(2, "0")
                    ParmMISC(iOrder) = New miscItems(Utilities.FixStringLength(deoh.PaymentType, 20), _
                                                     parmDeliveryDate, _
                                                     deoh.PromotionValue, _
                                                     deoh.TotalValueCharged, _
                                                     deoh.TotalOrderValue, _
                                                     orderFailedInitialValidation, _
                                                     orderFailedInitialValidationMessage)

                Next iOrder
            End With
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBOR17"
                err.HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function DataEntityUnPackChorus() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
        '-------------------------------------------------------------------------------------
        Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
        Dim dead As New DeAddress               ' Items
        Dim deop As New DEPayments              ' Items
        Dim deoc As New DECharges               ' Items
        ' 
        Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
        Dim depr As DeProductLines              ' Items
        Dim decl As DeCommentLines = Nothing    ' Items

        Dim iOrder As Integer = 0
        Dim iAddress As Integer = 0
        Dim iPayment As Integer = 0
        Dim iCharge As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0
        Dim orderFailedInitialValidation As Boolean = False
        Dim orderFailedInitialValidationMessage As String = String.Empty
        '
        Dim detr As New DETransaction           ' Items
        Try
            With Dep
                detr = .CollDETrans.Item(1)
                '-----------------------------------------------------------------------------------------
                For iOrder = 1 To .CollDEOrders.Count
                    orderFailedInitialValidation = False
                    orderFailedInitialValidationMessage = String.Empty
                    _orderLineNo = 1
                    '
                    deos = Dep.CollDEOrders.Item(iOrder)
                    deoh = deos.DEOrderHeader
                    '-------------------------------------------------------------------------------------
                    '   May have one or more adresses, such as Shipping and Billing, most cases
                    '   they will both be the same.
                    '
                    For iAddress = 1 To deoh.CollDEAddress.Count
                        dead = deoh.CollDEAddress.Item(iAddress)
                        With dead

                        End With
                    Next iAddress
                    '-------------------------------------------------------------------------------------
                    '   May have one or more payemnts, such as CC and SO, most cases
                    '   they will be one.
                    '
                    For iPayment = 1 To deoh.CollDEPayments.Count
                        deop = deoh.CollDEPayments.Item(iPayment)
                        With deop
                            '--------------------------------------------------
                            ' Validate card number / Start and Expiry Date
                            '
                            If deop.PaymentType.Equals("CC") Then
                                '----------------------------------------
                                If Not Utilities.ValidateCVV(deop.CV2Number) Then
                                    Const strError13 As String = "Invalid CV2 number"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError13
                                        .ErrorNumber = "TACDBOR13"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                                If Not Utilities.ValidateCardNumber(deop.CardNumber) Then
                                    Const strError14 As String = "Invalid Card number"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError14
                                        .ErrorNumber = "TACDBOR14"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                                If Not Utilities.ValidateExpiryDate(deop.ExpiryDate) Then
                                    Const strError15 As String = "Invalid Card Expiry Date"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError15
                                        .ErrorNumber = "TACDBOR15"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                                If Not Utilities.ValidateStartDate(deop.StartDate) Then
                                    Const strError16 As String = "Invalid Card Start Date"
                                    With err
                                        .ErrorMessage = String.Empty
                                        .ErrorStatus = strError16
                                        .ErrorNumber = "TACDBOR16"
                                        .HasError = True
                                    End With
                                End If
                                '----------------------------------------
                            End If
                        End With

                        If Not err.HasError Then
                            ParmPAYMENT(iOrder, iPayment) = BuildParmPAYMENT( _
                                                deop.CardNumber, deop.ExpiryDate, _
                                                deop.StartDate, deop.IssueNumber, _
                                                deop.CV2Number, deop.CardpostCode, _
                                                deop.CardAddress, deop.CardType)
                        End If

                    Next iPayment
                    '-------------------------------------------------------------------------------------
                    '   May have one or more Charges, most cases they will be one.
                    '
                    For iCharge = 1 To deoh.CollDECharges.Count
                        deoc = deoh.CollDECharges.Item(iCharge)
                        With deoc
                            ParmCHGE(iOrder, iCharge) = .Charge
                        End With
                    Next iCharge
                    '-------------------------------------------------------
                    ' If it's ticketing retail environment then account no
                    ' will be 12 long and so useless to chorus. instead pass 
                    ' so chorus will use a default account no.
                    '-------------------------------------------------------
                    If Not Settings.AccountNo1 Is Nothing AndAlso Settings.AccountNo1.Length > 8 Then
                        ' Settings.AccountNo1 = Settings.AccountNo1.Substring(0, 8)
                        Settings.AccountNo1 = String.Empty
                        Settings.AccountNo2 = String.Empty
                    End If
                    If Not Settings.AccountNo2 Is Nothing AndAlso Settings.AccountNo2.Length > 3 Then
                        Settings.AccountNo2 = Settings.AccountNo2.Substring(0, 3)
                    End If
                    ParmHEAD(iOrder) = BuildParmHEAD_CHORUS(Settings.AccountNo1, Settings.AccountNo2, deoh, "", deoh.CollDECharges, deoh.CollDEAddress(2), Settings.AccountNo3, Settings.LoginId)
                    '-------------------------------------------------------------------------------------
                    '   May have one or more products 
                    '
                    deoi = deos.DEOrderInfo
                    For iItems = 1 To deoi.CollDEProductLines.Count
                        depr = deoi.CollDEProductLines.Item(iItems)
                        With depr
                            .Quantity = (.Quantity.ToString).PadLeft(9, "0")
                            .CustomerLineNumber = .CustomerLineNumber.PadLeft(6, "0")
                            ParmITEM(iOrder, iItems) = BuildParmITEM_CHORUS("", depr, Settings.AccountNo1, _
                                                            Settings.AccountNo2, deoh.CollDEAddress(1), Settings.AccountNo3)
                        End With
                    Next
                    '------------------------------------------------------------
                    ' May have one or more comments 
                    '
                    For iComments = 1 To deoi.CollDECommentLines.Count
                        decl = deoi.CollDECommentLines.Item(iComments)
                        With decl
                            ParmCOMM(iOrder, iComments) = .CommentText
                            ParmCOMMLineNumber(iOrder, iComments) = .CommentLineNumber
                        End With
                    Next iComments
                    '--------------------------------
                    ' Additional items..
                    '--------------------------------
                    Dim parmDeliveryDate As String = deoh.ProjectedDeliveryDate.Year.ToString.PadLeft(4, "0") & _
                                                          deoh.ProjectedDeliveryDate.Month.ToString.PadLeft(2, "0") & _
                                                         deoh.ProjectedDeliveryDate.Day.ToString.PadLeft(2, "0")
                    ParmMISC(iOrder) = New miscItems(Utilities.FixStringLength(deoh.PaymentType, 20), _
                                                     parmDeliveryDate, _
                                                    deoh.PromotionValue, _
                                                    deoh.TotalValueCharged, _
                                                    deoh.TotalOrderValue, _
                                                    orderFailedInitialValidation, _
                                                    orderFailedInitialValidationMessage)

                Next iOrder
            End With
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBOR17"
                err.HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function DataEntityUnPackADL() As ErrorObj
        Dim err As New ErrorObj

        Dim deos As New DeOrders
        Dim deoh As New DeOrderHeader
        Dim deop As New DEPayments
        Dim deoc As New DECharges
        Dim deoi As New DEOrderInfo
        Dim depr As DeProductLines
        Dim decl As DeCommentLines = Nothing

        Dim iOrder As Integer = 0
        Dim iPayment As Integer = 0
        Dim iCharge As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0

        Try

            For iOrder = 1 To Dep.CollDEOrders.Count
                _orderLineNo = 1

                deos = Dep.CollDEOrders.Item(iOrder)
                deoh = deos.DEOrderHeader

                ' Add to collection 
                ParmHEAD2(iOrder) = New Collection
                With ParmHEAD2(iOrder)
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Line1, "billAddressLine1")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Line2, "billAddressLine2")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Line3, "billAddressLine3")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).City, "billAddressCity")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Province, "billAddressProvince")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).PostalCode, "billAddressPostalCode")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Country, "billAddressCountry")
                    .Add(deoh.WebOrderNumber, "webOrderNumber")
                    .Add(deoh.ProjectedDeliveryDate, "projectedDeliveryDate")
                    .Add(deoh.CarrierCode, "carrierCode")
                    .Add(deoh.CarrierCodeValue, "carrierCodeValue")
                    .Add(deoh.CarrierCodeVAT, "carrierCodeVAT")
                    .Add(deoh.CarrierCodeNet, "carrierCodeNet")
                    .Add(deoh.TotalOrderItemsValueNet, "totalOrderItemsValueNet")
                    .Add(deoh.TotalOrderItemsValueTax, "totalOrderItemsValueTax")
                    .Add(deoh.TotalOrderItemsValue, "totalOrderItemsValue")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Title, "Title")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Forename, "Forename")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).Surname, "Surname")
                    .Add(CType(deoh.CollDEAddress(2), DeAddress).PhoneNumber, "TelNo")
                    .Add(deoh.TaxAmount1, "TaxAmount1")
                    .Add(deoh.TaxAmount2, "TaxAmount2")
                    .Add(deoh.TaxAmount3, "TaxAmount3")
                    .Add(deoh.TaxAmount4, "TaxAmount4")
                    .Add(deoh.TaxAmount5, "TaxAmount5")
                    .Add(deoh.TaxInclusive1, "TaxInclusive1")
                    .Add(deoh.TaxInclusive2, "TaxInclusive2")
                    .Add(deoh.TaxInclusive3, "TaxInclusive3")
                    .Add(deoh.TaxInclusive4, "TaxInclusive4")
                    .Add(deoh.TaxInclusive5, "TaxInclusive5")
                    .Add(deoh.TaxDisplay1, "TaxDisplay1")
                    .Add(deoh.TaxDisplay2, "TaxDisplay2")
                    .Add(deoh.TaxDisplay3, "TaxDisplay3")
                    .Add(deoh.TaxDisplay4, "TaxDisplay4")
                    .Add(deoh.TaxDisplay5, "TaxDisplay5")
                    .Add(deoh.TaxCode1, "TaxCode1")
                    .Add(deoh.TaxCode2, "TaxCode2")
                    .Add(deoh.TaxCode3, "TaxCode3")
                    .Add(deoh.TaxCode4, "TaxCode4")
                    .Add(deoh.TaxCode5, "TaxCode5")
                    .Add(deoh.CustomerNumberPrefix.Trim & deoh.CustomerNumber.ToString.PadLeft(9, "0"), "CustomerNumber")
                    .Add(deoh.Currency, "Currency")
                    .Add(deoh.Message, "Message")
                    .Add(deoh.BasketNumber, "BasketNumber")
                    .Add(deoh.PromotionCode, "PromotionCode")
                    .Add(deoh.PromotionCodeDescription, "PromotionDescription")
                    .Add(deoh.PaymentSubType, "CreditCardType")
                    .Add(deoh.SuspendCode, "SuspendCode")
                End With

                For iPayment = 1 To deoh.CollDEPayments.Count
                    deop = deoh.CollDEPayments.Item(iPayment)
                Next iPayment

                For iCharge = 1 To deoh.CollDECharges.Count
                    deoc = deoh.CollDECharges.Item(iCharge)
                Next iCharge

                deoi = deos.DEOrderInfo
                For iItems = 1 To deoi.CollDEProductLines.Count
                    depr = deoi.CollDEProductLines.Item(iItems)

                    ParmITEM2(iOrder, iItems) = New Collection
                    With ParmITEM2(iOrder, iItems)
                        .Add(depr.SKU, "SKU")
                        .Add(depr.Quantity, "quantity")
                        .Add(depr.FixedPrice, "UnitPrice")
                        .Add(_orderLineNo, "LineNumber")
                        _orderLineNo += 1
                        .Add(depr.NetLineValue, "NetLineValue")
                        .Add(depr.TaxLineValue, "TaxLineValue")
                        .Add(depr.GrossLineValue, "GrossLineValue")
                        .Add(depr.ProductTaxValue, "ProductTaxValue")
                        .Add(depr.ProductDescription, "ProductDescription")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).ContactName, "shipContactName")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).Line1, "shipAddressLine1")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).Line2, "shipAddressLine2")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).Line3, "shipAddressLine3")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).City, "shipCity")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).Province, "shipProvince")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).PostalCode, "shipPostalCode")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).Country, "shipCountry")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).Email, "shipEmail")
                        .Add(CType(deoh.CollDEAddress(1), DeAddress).PhoneNumber, "TelNo")
                        .Add(depr.TaxAmount1, "TaxAmount1")
                        .Add(depr.TaxAmount2, "TaxAmount2")
                        .Add(depr.TaxAmount3, "TaxAmount3")
                        .Add(depr.TaxAmount4, "TaxAmount4")
                        .Add(depr.TaxAmount5, "TaxAmount5")
                        .Add(depr.TaxCode1, "TaxCode1")
                        .Add(depr.TaxCode2, "TaxCode2")
                        .Add(depr.TaxCode3, "TaxCode3")
                        .Add(depr.TaxCode4, "TaxCode4")
                        .Add(depr.TaxCode5, "TaxCode5")
                        .Add(depr.Currency, "Currency")
                    End With

                Next

                For iComments = 1 To deoi.CollDECommentLines.Count
                    decl = deoi.CollDECommentLines.Item(iComments)
                Next iComments

            Next iOrder

        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBOR18"
                err.HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function BuildParmHEAD_CHORUS(ByVal customerNo As String, ByVal deliverySeq As String, ByVal orderHeader As DeOrderHeader, _
                                      ByVal shipToAttention As String, ByVal CollDeCharges As Collection, ByVal billAddress As DeAddress, ByVal companyNo As String, _
                                      ByVal loginId As String) _
                                       As String
        '------------------------------------------------------------------------------
        ' BuildParmHEAD - Build IN parameter for Order Header stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------

        Dim parmCustomerNo As String = Utilities.FixStringLength(customerNo, 15)
        Dim parmDeliverySeq As String = Utilities.FixStringLength(deliverySeq, 15)
        Dim parmCustomerPo As String = Utilities.FixStringLength(orderHeader.CustomerPO, 20)
        Dim parmEndUserPo As String = Utilities.FixStringLength(orderHeader.EndUserPO, 30)
        Dim parmCarrierCode As String = Utilities.FixStringLength(orderHeader.CarrierCode, 2)
        Dim parmCarrierCodeValue As String = Utilities.FixStringLength(orderHeader.CarrierCodeValue, 10)
        Dim parmAutoRelease As String = Utilities.FixStringLength(orderHeader.AutoRelease, 1)
        Dim parmSalesPerson As String = Utilities.FixStringLength(orderHeader.SalesPerson, 20)
        Dim parmOrderDueDate As String = Utilities.FixStringLength(orderHeader.OrderDueDate, 8)
        Dim parmSuspendCode As String = Utilities.FixStringLength(orderHeader.SuspendCode, 2)
        Dim parmBackOrderFlag As String = Utilities.FixStringLength(orderHeader.BackOrderFlag, 1)
        Dim parmSplitShip As String = Utilities.FixStringLength(orderHeader.SplitShipmentFlag, 1)
        Dim parmSplitLine As String = Utilities.FixStringLength(orderHeader.SplitLine, 1)
        Dim parmShipBranches As String = Utilities.FixStringLength(orderHeader.ShipFromBranches, 4)
        Dim parmShowDetail As String = Utilities.FixStringLength(" ", 1)
        Dim parmWebOrderNo As String = Utilities.FixStringLength(orderHeader.WebOrderNumber, 15)
        Dim parmOrderValue As String = Utilities.FixStringLength(orderHeader.TotalOrderItemsValue, 11)
        Dim parmOrderValueNet As String = Utilities.FixStringLength(orderHeader.TotalOrderItemsValueNet, 11)
        Dim parmOrderValueTax As String = Utilities.FixStringLength(orderHeader.TotalOrderItemsValueTax, 11)
        Dim parmDeliveryDate As String = orderHeader.ProjectedDeliveryDate.Year.ToString.PadLeft(4, "0") & _
                                         orderHeader.ProjectedDeliveryDate.Month.ToString.PadLeft(2, "0") & _
                                        orderHeader.ProjectedDeliveryDate.Day.ToString.PadLeft(2, "0")
        Dim parmCompanyNo As String = Utilities.FixStringLength(companyNo, 2)
        Dim parmLoginId As String = Utilities.FixStringLength(loginId, 100)
        Dim parmDeliveryType As String = Utilities.FixStringLength(orderHeader.CarrierCode, 10)

        If orderHeader.ShipToSuffix <> String.Empty Then _
            parmDeliverySeq = Utilities.FixStringLength(orderHeader.ShipToSuffix, 3)

        Dim parmCharge1 As String = String.Empty
        Dim parmCharge2 As String = String.Empty

        '------------------------------------------------------------
        ' Extract first 2 charges
        '------------------------------------------------------------
        Select Case CollDeCharges.Count
            Case Is = 2
                parmCharge1 = CType(CollDeCharges.Item(1), DECharges).Charge
                parmCharge2 = CType(CollDeCharges.Item(2), DECharges).Charge
            Case Is = 1
                parmCharge1 = CType(CollDeCharges.Item(1), DECharges).Charge
        End Select
        parmCharge1 = Utilities.FixStringLength(parmCharge1, 2)
        parmCharge2 = Utilities.FixStringLength(parmCharge2, 2)

        Dim parmBillAdd1 As String = Utilities.FixStringLength(billAddress.Line1, 35)
        Dim parmBillAdd2 As String = Utilities.FixStringLength(billAddress.Line2, 35)
        Dim parmBillAdd3 As String = Utilities.FixStringLength(billAddress.Line3, 35)
        Dim parmBillAddCity As String = Utilities.FixStringLength(billAddress.City, 25)
        Dim parmBillAddProv As String = Utilities.FixStringLength(billAddress.Province, 25)
        Dim parmBillAddPost As String = Utilities.FixStringLength(billAddress.PostalCode, 10)
        Dim parmBillCountry As String = Utilities.FixStringLength(billAddress.Country, 3)

        Dim sb As New StringBuilder
        With sb
            .Append(parmCustomerNo).Append(parmDeliverySeq).Append(parmCustomerPo)
            .Append(parmEndUserPo).Append(parmCarrierCode).Append(parmCarrierCodeValue)
            .Append(parmAutoRelease)
            .Append(parmSalesPerson).Append(parmOrderDueDate).Append(parmSuspendCode)
            .Append(parmBackOrderFlag).Append(parmSplitShip).Append(parmSplitLine)
            .Append(parmShipBranches).Append(parmShowDetail).Append(parmCharge1)
            .Append(parmCharge2).Append(parmBillAdd1).Append(parmBillAdd2)
            .Append(parmBillAdd3).Append(parmBillAddCity).Append(parmBillAddProv)
            .Append(parmBillAddPost).Append(parmBillCountry).Append(parmWebOrderNo)
            .Append(parmOrderValue).Append(parmOrderValueNet).Append(parmOrderValueTax)
            .Append(parmDeliveryDate).Append(parmCompanyNo).Append(parmLoginId).Append(parmDeliveryType)
        End With

        Return sb.ToString
    End Function

    Private Function BuildParmHEAD(ByVal customerNo As String, ByVal deliverySeq As String, ByVal orderHeader As DeOrderHeader, _
                                  ByVal shipToAttention As String, ByVal CollDeCharges As Collection, ByVal billAddress As DeAddress, ByVal companyNo As String, _
                                  ByVal loginId As String) _
                                   As String
        '------------------------------------------------------------------------------
        ' BuildParmHEAD - Build IN parameter for Order Header stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------

        Dim parmCustomerNo As String = Utilities.FixStringLength(customerNo, 8)
        Dim parmDeliverySeq As String = Utilities.FixStringLength(deliverySeq, 3)
        Dim parmCustomerPo As String = Utilities.FixStringLength(orderHeader.CustomerPO, 20)
        Dim parmEndUserPo As String = Utilities.FixStringLength(orderHeader.EndUserPO, 30)
        Dim parmCarrierCode As String = Utilities.FixStringLength(orderHeader.CarrierCode, 2)
        Dim parmCarrierCodeValue As String = Utilities.FixStringLength(orderHeader.CarrierCodeValue, 10)
        Dim parmAutoRelease As String = Utilities.FixStringLength(orderHeader.AutoRelease, 1)
        Dim parmSalesPerson As String = Utilities.FixStringLength(orderHeader.SalesPerson, 20)
        Dim parmOrderDueDate As String = Utilities.FixStringLength(orderHeader.OrderDueDate, 8)
        Dim parmSuspendCode As String = Utilities.FixStringLength(orderHeader.SuspendCode, 2)
        Dim parmBackOrderFlag As String = Utilities.FixStringLength(orderHeader.BackOrderFlag, 1)
        Dim parmSplitShip As String = Utilities.FixStringLength(orderHeader.SplitShipmentFlag, 1)
        Dim parmSplitLine As String = Utilities.FixStringLength(orderHeader.SplitLine, 1)
        Dim parmShipBranches As String = Utilities.FixStringLength(orderHeader.ShipFromBranches, 4)
        Dim parmShowDetail As String = Utilities.FixStringLength(" ", 1)
        Dim parmWebOrderNo As String = Utilities.FixStringLength(orderHeader.WebOrderNumber, 15)
        Dim parmOrderValue As String = Utilities.FixStringLength(orderHeader.TotalOrderItemsValue, 11)
        Dim parmOrderValueNet As String = Utilities.FixStringLength(orderHeader.TotalOrderItemsValueNet, 11)
        Dim parmOrderValueTax As String = Utilities.FixStringLength(orderHeader.TotalOrderItemsValueTax, 11)
        Dim parmDeliveryDate As String = orderHeader.ProjectedDeliveryDate.Year.ToString.PadLeft(4, "0") & _
                                         orderHeader.ProjectedDeliveryDate.Month.ToString.PadLeft(2, "0") & _
                                        orderHeader.ProjectedDeliveryDate.Day.ToString.PadLeft(2, "0")
        Dim parmCompanyNo As String = Utilities.FixStringLength(companyNo, 2)
        Dim parmLoginId As String = Utilities.FixStringLength(loginId, 100)
        Dim parmDeliveryChargeNet As String = Utilities.FixStringLength(orderHeader.CarrierCodeNet, 10)
        '--------------------------------
        ' Extension fields - header level
        '--------------------------------
        Dim parmExtReference1 As String = Utilities.FixStringLength(orderHeader.ExtensionReference1, 30)
        Dim parmExtReference2 As String = Utilities.FixStringLength(orderHeader.ExtensionReference2, 30)
        Dim parmExtReference3 As String = Utilities.FixStringLength(orderHeader.ExtensionReference3, 30)
        Dim parmExtReference4 As String = Utilities.FixStringLength(orderHeader.ExtensionReference4, 30)
        Dim parmExtFixedPrice1 As String = Utilities.FixStringLength(orderHeader.ExtensionFixedPrice1, 16)
        Dim parmExtFixedPrice2 As String = Utilities.FixStringLength(orderHeader.ExtensionFixedPrice2, 16)
        Dim parmExtFixedPrice3 As String = Utilities.FixStringLength(orderHeader.ExtensionFixedPrice3, 16)
        Dim parmExtFixedPrice4 As String = Utilities.FixStringLength(orderHeader.ExtensionFixedPrice4, 16)
        Dim parmExtDealID1 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID1, 15)
        Dim parmExtDealID2 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID2, 15)
        Dim parmExtDealID3 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID3, 15)
        Dim parmExtDealID4 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID4, 15)
        Dim parmExtDealID5 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID5, 15)
        Dim parmExtDealID6 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID6, 15)
        Dim parmExtDealID7 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID7, 15)
        Dim parmExtDealID8 As String = Utilities.FixStringLength(orderHeader.ExtensionDealID8, 15)
        Dim parmExtFlag1 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag1, 1)
        Dim parmExtFlag2 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag2, 1)
        Dim parmExtFlag3 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag3, 1)
        Dim parmExtFlag4 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag4, 1)
        Dim parmExtFlag5 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag5, 1)
        Dim parmExtFlag6 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag6, 1)
        Dim parmExtFlag7 As String = Utilities.FixStringLength(orderHeader.ExtensionFlag7, 1)
        Dim parmExtStatus As String = Utilities.FixStringLength(orderHeader.ExtensionStatus, 1)


        If orderHeader.ShipToSuffix <> String.Empty Then _
            parmDeliverySeq = Utilities.FixStringLength(orderHeader.ShipToSuffix, 3)

        Dim parmCharge1 As String = String.Empty
        Dim parmCharge2 As String = String.Empty

        '------------------------------------------------------------
        ' Extract first 2 charges
        '------------------------------------------------------------
        Select Case CollDeCharges.Count
            Case Is = 2
                parmCharge1 = CType(CollDeCharges.Item(1), DECharges).Charge
                parmCharge2 = CType(CollDeCharges.Item(2), DECharges).Charge
            Case Is = 1
                parmCharge1 = CType(CollDeCharges.Item(1), DECharges).Charge
        End Select
        parmCharge1 = Utilities.FixStringLength(parmCharge1, 2)
        parmCharge2 = Utilities.FixStringLength(parmCharge2, 2)

        Dim parmBillAdd1 As String = Utilities.FixStringLength(billAddress.Line1, 35)
        Dim parmBillAdd2 As String = Utilities.FixStringLength(billAddress.Line2, 35)
        Dim parmBillAdd3 As String = Utilities.FixStringLength(billAddress.Line3, 35)
        Dim parmBillAddCity As String = Utilities.FixStringLength(billAddress.City, 25)
        Dim parmBillAddProv As String = Utilities.FixStringLength(billAddress.Province, 25)
        Dim parmBillAddPost As String = Utilities.FixStringLength(billAddress.PostalCode, 10)
        Dim parmBillCountry As String = Utilities.FixStringLength(billAddress.Country, 3)

        Dim sb As New StringBuilder
        With sb
            .Append(parmCustomerNo).Append(parmDeliverySeq).Append(parmCustomerPo)
            .Append(parmEndUserPo).Append(parmCarrierCode).Append(parmCarrierCodeValue)
            .Append(parmAutoRelease)
            .Append(parmSalesPerson).Append(parmOrderDueDate).Append(parmSuspendCode)
            .Append(parmBackOrderFlag).Append(parmSplitShip).Append(parmSplitLine)
            .Append(parmShipBranches).Append(parmShowDetail).Append(parmCharge1)
            .Append(parmCharge2).Append(parmBillAdd1).Append(parmBillAdd2)
            .Append(parmBillAdd3).Append(parmBillAddCity).Append(parmBillAddProv)
            .Append(parmBillAddPost).Append(parmBillCountry).Append(parmWebOrderNo)
            .Append(parmOrderValue).Append(parmOrderValueNet).Append(parmOrderValueTax)
            .Append(parmDeliveryDate).Append(parmCompanyNo).Append(parmLoginId).Append(parmDeliveryChargeNet)
            .Append(parmExtReference1).Append(parmExtReference2).Append(parmExtReference3).Append(parmExtReference4)
            .Append(parmExtFixedPrice1).Append(parmExtFixedPrice2).Append(parmExtFixedPrice3).Append(parmExtFixedPrice4)
            .Append(parmExtDealID1).Append(parmExtDealID2).Append(parmExtDealID3).Append(parmExtDealID4)
            .Append(parmExtDealID5).Append(parmExtDealID6).Append(parmExtDealID7).Append(parmExtDealID8)
            .Append(parmExtFlag1).Append(parmExtFlag2).Append(parmExtFlag3).Append(parmExtFlag4)
            .Append(parmExtFlag5).Append(parmExtFlag6).Append(parmExtFlag7).Append(parmExtStatus)
        End With

        Return sb.ToString
    End Function
    Private Function BuildParmITEM(ByVal orderNo As String, ByVal depr As DeProductLines, ByVal customerNo As String, _
                                   ByVal deliverySeq As String, ByVal shipAddress As DeAddress, ByVal companyNo As String) _
                                    As String
        '------------------------------------------------------------------------------
        ' BuildParmITEM - Build IN parameter for Order Detail stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------
        Dim parmOrderNo As String = Utilities.FixStringLength(orderNo, 15)
        Dim parmSku As String = Utilities.FixStringLength(depr.SKU, 15)
        ' /001 - Change length to 36
        '  Dim parmAlternateSku As String = Utilities.FixStringLength(depr.AlternateSKU, 15)
        Dim parmAlternateSku As String = Utilities.FixStringLength(depr.AlternateSKU, 36)
        Dim parmQuantity As String = Utilities.FixStringLength(depr.Quantity, 9)
        Dim parmResCode As String = Utilities.FixStringLength(" ", 1)
        Dim parmResSeq As String = Utilities.FixStringLength(" ", 1)
        Dim parmCustomerNo As String = Utilities.FixStringLength(customerNo, 8)
        Dim parmDeliverySeq As String = Utilities.FixStringLength(deliverySeq, 3)
        Dim parmShipAdd1 As String = Utilities.FixStringLength(shipAddress.Line1, 35)
        Dim parmShipAdd2 As String = Utilities.FixStringLength(shipAddress.Line2, 35)
        Dim parmShipAdd3 As String = Utilities.FixStringLength(shipAddress.Line3, 35)
        Dim parmShipAddCity As String = Utilities.FixStringLength(shipAddress.City, 25)
        Dim parmShipAddProv As String = Utilities.FixStringLength(shipAddress.Province, 25)
        Dim parmShipAddPost As String = Utilities.FixStringLength(shipAddress.PostalCode, 10)
        Dim parmShipToAtt As String = Utilities.FixStringLength(shipAddress.ContactName, 35)
        Dim parmLineNo As String = String.Empty
        Dim parmPrice As String = Utilities.FixStringLength(depr.FixedPrice, 9)
        Dim parmShipCountry As String = Utilities.FixStringLength(shipAddress.Country, 3)
        Dim parmShipEmail As String = Utilities.FixStringLength(shipAddress.Email, 50)
        Dim parmPriceNet As String = Utilities.FixStringLength(depr.FixedPriceNet, 9)
        Dim parmPriceTax As String = Utilities.FixStringLength(depr.FixedPriceTax, 9)
        Dim parmLineComment As String = Utilities.FixStringLength(depr.LineComment, 15)
        Dim parmCompanyNo As String = Utilities.FixStringLength(companyNo, 2)
        '------------------------------
        ' Extension fields - line level
        '------------------------------
        Dim parmExtReference1 As String = Utilities.FixStringLength(depr.ExtensionReference1, 30)
        Dim parmExtReference2 As String = Utilities.FixStringLength(depr.ExtensionReference2, 30)
        Dim parmExtReference3 As String = Utilities.FixStringLength(depr.ExtensionReference3, 30)
        Dim parmExtReference4 As String = Utilities.FixStringLength(depr.ExtensionReference4, 30)
        Dim parmExtReference5 As String = Utilities.FixStringLength(depr.ExtensionReference5, 30)
        Dim parmExtReference6 As String = Utilities.FixStringLength(depr.ExtensionReference6, 30)
        Dim parmExtReference7 As String = Utilities.FixStringLength(depr.ExtensionReference7, 30)
        Dim parmExtReference8 As String = Utilities.FixStringLength(depr.ExtensionReference8, 30)
        Dim parmExtFlag1 As String = Utilities.FixStringLength(depr.ExtensionFlag1, 1)
        Dim parmExtFlag2 As String = Utilities.FixStringLength(depr.ExtensionFlag2, 1)
        Dim parmExtFlag3 As String = Utilities.FixStringLength(depr.ExtensionFlag3, 1)
        Dim parmExtFlag4 As String = Utilities.FixStringLength(depr.ExtensionFlag4, 1)
        Dim parmExtFlag5 As String = Utilities.FixStringLength(depr.ExtensionFlag5, 1)
        Dim parmExtFlag6 As String = Utilities.FixStringLength(depr.ExtensionFlag6, 1)
        Dim parmExtFlag7 As String = Utilities.FixStringLength(depr.ExtensionFlag7, 1)
        Dim parmExtFlag8 As String = Utilities.FixStringLength(depr.ExtensionFlag8, 1)
        Dim parmExtFlag9 As String = Utilities.FixStringLength(depr.ExtensionFlag9, 1)
        Dim parmExtFlag0 As String = Utilities.FixStringLength(depr.ExtensionFlag0, 1)
        Dim parmExtField1 As String = Utilities.FixStringLength(depr.ExtensionField1, 10)
        Dim parmExtField2 As String = Utilities.FixStringLength(depr.ExtensionField2, 10)
        Dim parmExtField3 As String = Utilities.FixStringLength(depr.ExtensionField3, 10)
        Dim parmExtField4 As String = Utilities.FixStringLength(depr.ExtensionField4, 10)
        Dim parmExtFixedPrice1 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice1, 16)
        Dim parmExtFixedPrice2 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice2, 16)
        Dim parmExtFixedPrice3 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice3, 16)
        Dim parmExtFixedPrice4 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice4, 16)
        Dim parmExtFixedPrice5 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice5, 16)
        Dim parmExtFixedPrice6 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice6, 16)
        Dim parmExtFixedPrice7 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice7, 16)
        Dim parmExtFixedPrice8 As String = Utilities.FixStringLength(depr.ExtensionFixedPrice8, 16)
        Dim parmExtDealID1 As String = Utilities.FixStringLength(depr.ExtensionDealID1, 15)
        Dim parmExtDealID2 As String = Utilities.FixStringLength(depr.ExtensionDealID2, 15)
        Dim parmExtDealID3 As String = Utilities.FixStringLength(depr.ExtensionDealID3, 15)
        Dim parmExtDealID4 As String = Utilities.FixStringLength(depr.ExtensionDealID4, 15)
        Dim parmExtDealID5 As String = Utilities.FixStringLength(depr.ExtensionDealID5, 15)
        Dim parmExtDealID6 As String = Utilities.FixStringLength(depr.ExtensionDealID6, 15)
        Dim parmExtDealID7 As String = Utilities.FixStringLength(depr.ExtensionDealID7, 15)
        Dim parmExtDealID8 As String = Utilities.FixStringLength(depr.ExtensionDealID8, 15)
        Dim parmExtStatus As String = Utilities.FixStringLength(depr.ExtensionStatus, 1)

        If depr.CustomerLineNumber = String.Empty Or depr.CustomerLineNumber = "000000" Then
            parmLineNo = _orderLineNo.ToString.PadLeft(6, "0")
            _orderLineNo += 1
        Else
            parmLineNo = depr.CustomerLineNumber.PadLeft(6, "0")
            _orderLineNo = CType(depr.CustomerLineNumber, Int32)
            _orderLineNo += 1
        End If

        parmLineNo = Utilities.FixStringLength(parmLineNo, 6)

        Dim sb As New StringBuilder
        With sb
            .Append(parmOrderNo).Append(parmSku).Append(parmAlternateSku).Append(parmQuantity).Append(parmResCode)
            .Append(parmResSeq).Append(parmCustomerNo).Append(parmDeliverySeq).Append(parmShipAdd1)
            .Append(parmShipAdd2).Append(parmShipAdd3).Append(parmShipAddCity).Append(parmShipAddProv)
            .Append(parmShipAddPost).Append(parmShipToAtt).Append(parmLineNo).Append(parmPrice)
            .Append(parmShipCountry).Append(parmShipEmail).Append(parmPriceNet).Append(parmPriceTax)
            .Append(parmLineComment).Append(parmCompanyNo)
            .Append(parmExtReference1).Append(parmExtReference2).Append(parmExtReference3).Append(parmExtReference4)
            .Append(parmExtReference5).Append(parmExtReference6).Append(parmExtReference7).Append(parmExtReference8)
            .Append(parmExtFlag1).Append(parmExtFlag2).Append(parmExtFlag3).Append(parmExtFlag4).Append(parmExtFlag5)
            .Append(parmExtFlag6).Append(parmExtFlag7).Append(parmExtFlag8).Append(parmExtFlag9).Append(parmExtFlag0)
            .Append(parmExtField1).Append(parmExtField2).Append(parmExtField3).Append(parmExtField4)
            .Append(parmExtFixedPrice1).Append(parmExtFixedPrice2).Append(parmExtFixedPrice3).Append(parmExtFixedPrice4)
            .Append(parmExtFixedPrice5).Append(parmExtFixedPrice6).Append(parmExtFixedPrice7).Append(parmExtFixedPrice8)
            .Append(parmExtDealID1).Append(parmExtDealID2).Append(parmExtDealID3).Append(parmExtDealID4)
            .Append(parmExtDealID5).Append(parmExtDealID6).Append(parmExtDealID7).Append(parmExtDealID8)
            .Append(parmExtStatus)
        End With

        Return sb.ToString

    End Function
    Private Function BuildParmITEM_CHORUS(ByVal orderNo As String, ByVal depr As DeProductLines, ByVal customerNo As String, _
                                  ByVal deliverySeq As String, ByVal shipAddress As DeAddress, ByVal companyNo As String) _
                                   As String
        '------------------------------------------------------------------------------
        ' BuildParmITEM - Build IN parameter for Order Detail stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------
        Dim parmOrderNo As String = Utilities.FixStringLength(orderNo, 15)
        Dim parmSku As String = Utilities.FixStringLength(depr.SKU, 30)
        ' /001 - Change length to 36
        '  Dim parmAlternateSku As String = Utilities.FixStringLength(depr.AlternateSKU, 15)
        Dim parmAlternateSku As String = Utilities.FixStringLength(depr.AlternateSKU, 36)
        Dim parmQuantity As String = Utilities.FixStringLength(depr.Quantity, 9)
        Dim parmResCode As String = Utilities.FixStringLength(" ", 1)
        Dim parmResSeq As String = Utilities.FixStringLength(" ", 1)
        Dim parmCustomerNo As String = Utilities.FixStringLength(customerNo, 15)
        Dim parmDeliverySeq As String = Utilities.FixStringLength(deliverySeq, 15)
        Dim parmShipAdd1 As String = Utilities.FixStringLength(shipAddress.Line1, 35)
        Dim parmShipAdd2 As String = Utilities.FixStringLength(shipAddress.Line2, 35)
        Dim parmShipAdd3 As String = Utilities.FixStringLength(shipAddress.Line3, 35)
        Dim parmShipAddCity As String = Utilities.FixStringLength(shipAddress.City, 25)
        Dim parmShipAddProv As String = Utilities.FixStringLength(shipAddress.Province, 25)
        Dim parmShipAddPost As String = Utilities.FixStringLength(shipAddress.PostalCode, 10)
        Dim parmShipToAtt As String = Utilities.FixStringLength(shipAddress.ContactName, 35)
        Dim parmLineNo As String = String.Empty
        Dim parmPrice As String = Utilities.FixStringLength(depr.FixedPrice, 9)
        Dim parmShipCountry As String = Utilities.FixStringLength(shipAddress.Country, 3)
        Dim parmShipEmail As String = Utilities.FixStringLength(shipAddress.Email, 50)
        Dim parmPriceNet As String = Utilities.FixStringLength(depr.FixedPriceNet, 9)
        Dim parmPriceTax As String = Utilities.FixStringLength(depr.FixedPriceTax, 9)
        Dim parmLineComment As String = Utilities.FixStringLength(depr.LineComment, 15)
        Dim parmCompanyNo As String = Utilities.FixStringLength(companyNo, 2)
        Dim promoPercentage As String = (depr.PromotionPercentage.ToString("000.00")).Replace(".", "")
        Dim parmCostCentre As String = Utilities.FixStringLength(depr.CostCentre, 20)
        Dim parmAccountCode As String = Utilities.FixStringLength(depr.AccountCode, 20)

        If depr.CustomerLineNumber = String.Empty Or depr.CustomerLineNumber = "000000" Then
            parmLineNo = _orderLineNo.ToString.PadLeft(6, "0")
            _orderLineNo += 1
        Else
            parmLineNo = depr.CustomerLineNumber.PadLeft(6, "0")
            _orderLineNo = CType(depr.CustomerLineNumber, Int32)
            _orderLineNo += 1
        End If

        parmLineNo = Utilities.FixStringLength(parmLineNo, 6)

        Dim sb As New StringBuilder
        With sb
            .Append(parmOrderNo).Append(parmSku).Append(parmAlternateSku).Append(parmQuantity).Append(parmResCode)
            .Append(parmResSeq).Append(parmCustomerNo).Append(parmDeliverySeq).Append(parmShipAdd1)
            .Append(parmShipAdd2).Append(parmShipAdd3).Append(parmShipAddCity).Append(parmShipAddProv)
            .Append(parmShipAddPost).Append(parmShipToAtt).Append(parmLineNo).Append(parmPrice)
            .Append(parmShipCountry).Append(parmShipEmail).Append(parmPriceNet).Append(parmPriceTax)
            .Append(parmLineComment).Append(parmCompanyNo).Append(promoPercentage)
        End With

        Return sb.ToString

    End Function
    Private Function BuildParmPAYMENT(ByVal cardNumber As String, ByVal expiryDate As String, ByVal startDate As String, _
                                      ByVal issueNumber As String, ByVal CV2 As String, ByVal cardPostcode As String, _
                                      ByVal cardAddress As String, ByVal cardType As String) As String
        '------------------------------------------------------------------------------
        ' BuildParmpayment - Build IN parameter for Write Order  stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------
        Dim parmCardNumber As String = Utilities.FixStringLength(cardNumber, 25)
        Dim parmExpiryDate As String = Utilities.FixStringLength(expiryDate, 4)
        Dim parmStartDate As String = Utilities.FixStringLength(startDate, 4)
        Dim parmIssueNumber As String = Utilities.FixStringLength(issueNumber, 2)
        Dim parmCV2 As String = Utilities.FixStringLength(CV2, 4)
        Dim parmCardPostcode As String = Utilities.FixStringLength(cardPostcode, 12)
        Dim parmCardAddress As String = Utilities.FixStringLength(cardAddress, 15)
        Dim parmCardType As String = Utilities.FixStringLength(cardType, 10)

        Dim sb As New StringBuilder
        With sb
            .Append(parmCardNumber).Append(parmExpiryDate).Append(parmStartDate).Append(parmIssueNumber)
            .Append(parmCV2).Append(parmCardPostcode).Append(parmCardAddress).Append(parmCardType)
        End With

        Return sb.ToString

    End Function


    Private Function ReadOrderChorus() As ErrorObj
        Dim err As New ErrorObj

        ord = New DEOrderReponse
        ord.BranchOrderNumber = BackOfficeOrderNumber
        System21OrderNo = BackOfficeOrderNumber

        '-------------------------------------------------
        ' Add record empty except for back office order no
        ' to update to sql file
        '-------------------------------------------------
        DtHeader.Rows.Add(DtHeader_Add)
        '' ''--------------------------------
        '' ''---------------------------------------------------------------------------------
        ' ''Dim libl As String = conSystem21.LibraryList
        ' ''ord = New DEOrderReponse                    ' Otherwise we still have previous order info
        ' ''ord.BranchOrderNumber = System21OrderNo
        ' ''err = ReadOrderHeaderSystem21()             ' DtHeader      Read order header 
        ' ''If Not err.HasError Then
        ' ''    err = ReadDeliveryAddressSystem21()         ' DtHeader      Read delivery address  
        ' ''End If
        ' ''If Not err.HasError Then
        ' ''    err = ReadCarriageDetailsSystem21()         ' DtCarrier     Read carriage details
        ' ''End If
        ' ''If Not err.HasError Then
        ' ''    err = ReadOrderLinesSystem21()              ' DtDetail      Read Order lines
        ' ''End If
        ' ''If Not err.HasError Then
        ' ''    err = ReadInvoiceSystem21()                 ' DtHeader      Read Invoice header
        ' ''End If
        ' ''If Not err.HasError Then
        ' ''    err = ReadSalesDispatchSystem21()           ' DtHeader      Read Sale Order header
        ' ''End If
        ' ''If Not err.HasError Then
        ' ''    err = ReadPackagesSystem21()                ' DtPackage     Package details 
        ' ''End If

        '' '' Calc totals
        ' ''Try
        ' ''    ord.SalePlusTax = (CDec(ord.TotalTax) + CDec(ord.TotalSales)).ToString
        ' ''    ord.GrandTotal = (CDec(ord.SalePlusTax) + CDec(ord.FreightRate)).ToString
        ' ''Catch ex As Exception

        ' ''End Try

        ' ''If Not err.HasError Then
        ' ''    err = ReadSerialNumbersSystem21()           ' DtProduct     serial numbers in a package
        ' ''End If
        '' ''------------------------------------------------------------------------------
        '' ''
        ' ''DtHeader.Rows.Add(DtHeader_Add)             ' Add to Header Table
        Return err
    End Function

    Private Function ReadOrderSystem21() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        Dim libl As String = conSystem21.LibraryList
        ord = New DEOrderReponse                    ' Otherwise we still have previous order info
        ord.BranchOrderNumber = System21OrderNo
        err = ReadOrderHeaderSystem21()             ' DtHeader      Read order header 
        If Not err.HasError Then
            err = ReadDeliveryAddressSystem21()         ' DtHeader      Read delivery address  
        End If
        If Not err.HasError Then
            err = ReadCarriageDetailsSystem21()         ' DtCarrier     Read carriage details
        End If
        If Not err.HasError Then
            err = ReadOrderLinesSystem21()              ' DtDetail      Read Order lines
        End If
        If Not err.HasError Then
            err = ReadInvoiceSystem21()                 ' DtHeader      Read Invoice header
        End If
        If Not err.HasError Then
            err = ReadSalesDispatchSystem21()           ' DtHeader      Read Sale Order header
        End If
        If Not err.HasError Then
            err = ReadPackagesSystem21()                ' DtPackage     Package details 
        End If

        ' Calc totals
        Try
            ord.SalePlusTax = (CDec(ord.TotalTax) + CDec(ord.TotalSales)).ToString
            ord.GrandTotal = (CDec(ord.SalePlusTax) + CDec(ord.FreightRate)).ToString
        Catch ex As Exception

        End Try

        If Not err.HasError Then
            err = ReadSerialNumbersSystem21()           ' DtProduct     serial numbers in a package
        End If
        '------------------------------------------------------------------------------
        '
        DtHeader.Rows.Add(DtHeader_Add)             ' Add to Header Table
        Return err
    End Function
    Private Function ReadCarriageDetailsSystem21() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        '   Read Order Charges OEP50
        ' 
        Const sqlSelect As String = "SELECT * FROM CHARGES " & _
                                    " WHERE CONO50 = @PARAM1 AND ORDN50 = @PARAM2 AND SQNO50 = 1 "
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"

        ord.FreightRate = 0


        Try
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)

            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
            End Select
            dtrReader = cmdSelect.ExecuteReader()
            With dtrReader
                While .Read

                    ord.CarrierCode = .GetString(.GetOrdinal("CHGC50"))
                    If ord.CarrierCode.Length = 1 Then _
                        ord.CarrierCode = "0" & ord.CarrierCode
                    ord.CarrierCodeDescription = .GetString(.GetOrdinal("ODES50"))
                    ord.FreightRate += .GetDecimal(.GetOrdinal("CHGV50"))
                    '-------------------------------------------------
                    dRow = Nothing
                    dRow = DtCarrier.NewRow()
                    dRow("DistributionWeight") = 0
                    dRow("CarrierCode") = ord.CarrierCode
                    If ord.CarrierCode.Length = 1 Then _
                        dRow("CarrierCode") = "0" & ord.CarrierCode
                    dRow("CarrierCodeDescription") = ord.CarrierCodeDescription
                    dRow("FreightRate") = ord.FreightRate
                    dRow("OrderNumber") = System21OrderNo
                    '-------------------------------------------------
                    dRow("InvoiceDate") = ord.InvoiceDate
                    dRow("OrderCreditMemoCode1") = String.Empty
                    dRow("OrderCreditMemoCode2") = String.Empty
                    dRow("OrderCreditMemo1") = String.Empty
                    dRow("OrderCreditMemo2") = String.Empty

                    dRow("OrderShipDate") = ord.ShippedDate
                    dRow("OrderStatus") = ord.OrderStatus
                    dRow("ShipFromBranch") = ord.ShipFromBranch
                    dRow("ShipFromBranchNumber") = ord.ShipFromBranchNumber
                    dRow("Suffix") = String.Empty
                    dRow("TotalSales") = 0
                    '-------------------------------------------------        
                    DtCarrier.Rows.Add(dRow)
                End While
                .Close()
            End With
        Catch ex As Exception
            Const strError As String = "Failed to Read Carriage Details"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR71"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadDeliveryAddressSystem21() As ErrorObj
        Dim err As New ErrorObj
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim param3 As String = "@PARAM3"
        Try
            '-------------------------------------------------------------------------------
            '   Read ship to address  
            '
            Const sqlSelect2 As String = "SELECT * FROM DELADDS WHERE CONO45 = @PARAM1 AND ORDN45 = @PARAM2 AND SEQN45 = 1"
            cmdSelect = New iDB2Command(sqlSelect2, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
            End Select
            '------------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            With dtrReader
                If .HasRows() Then
                    .Read()
                    ord.ShipTo = .GetString(.GetOrdinal("ORDN45")).Trim
                    ord.ShipToAttention = .GetString(.GetOrdinal("ONAM45")).Trim
                    ord.ShipToName = .GetString(.GetOrdinal("ONAM45")).Trim
                    ord.ShipToAddress1 = .GetString(.GetOrdinal("OAD145")).Trim
                    ord.ShipToAddress2 = .GetString(.GetOrdinal("OAD245")).Trim
                    ord.ShipToAddress3 = .GetString(.GetOrdinal("OAD345")).Trim
                    ord.ShipToAddress4 = .GetString(.GetOrdinal("OAD445")).Trim
                    ord.ShipToCity = .GetString(.GetOrdinal("OAD445")).Trim
                    ord.ShipToProvince = .GetString(.GetOrdinal("OAD545")).Trim
                    ord.ShipToPostalCode = .GetString(.GetOrdinal("OPST45")).Trim
                    ord.ShipToCountry = .GetString(.GetOrdinal("OAD545")).Trim
                End If
                .Close()
            End With
            '--------------------------------------------------------------------------
            '   Read Billing address, required for Order detail.
            '
            dtrReader.Close()
            Const sqlSelect4 As String = "SELECT * FROM CUSNAMES WHERE CONO05 = @PARAM1 AND CUSN05 = @PARAM2 AND DSEQ05 = @PARAM3"
            cmdSelect = New iDB2Command(sqlSelect4, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 8).Value = ord.CUSN40
                    cmdSelect.Parameters.Add(param3, iDB2DbType.iDB2CharBitData, 3).Value = ord.DSEQ40
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 8).Value = ord.CUSN40
                    cmdSelect.Parameters.Add(param3, iDB2DbType.iDB2Char, 3).Value = ord.DSEQ40
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 8).Value = ord.CUSN40
                    cmdSelect.Parameters.Add(param3, iDB2DbType.iDB2Char, 3).Value = ord.DSEQ40
            End Select
            '------------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            With dtrReader
                If .HasRows() Then
                    .Read()
                    ord.BillTo = .GetString(.GetOrdinal("DSEQ05")).Trim
                    ord.BillToName = .GetString(.GetOrdinal("CNAM05")).Trim
                    ord.BillToAttention = .GetString(.GetOrdinal("CNAM05")).Trim
                    ord.BillToAddress1 = .GetString(.GetOrdinal("CAD105")).Trim
                    ord.BillToAddress2 = .GetString(.GetOrdinal("CAD205")).Trim
                    ord.BillToAddress3 = .GetString(.GetOrdinal("CAD305")).Trim
                    ord.BillToAddress4 = .GetString(.GetOrdinal("CAD405")).Trim
                    ord.BillToCity = .GetString(.GetOrdinal("CAD405")).Trim
                    ord.BillToProvince = .GetString(.GetOrdinal("CAD505")).Trim
                    ord.BillToPostalCode = .GetString(.GetOrdinal("PCD105")).Trim & " " & .GetString(.GetOrdinal("PCD205")).Trim
                    ord.BillToCountry = .GetString(.GetOrdinal("CAD505")).Trim
                End If
                .Close()
            End With
            '------------------------------------------------------------------------------
            '   If no ship to address set to billing address as default
            '
            If ord.ShipToAddress1.Length = 0 Then
                ord.ShipTo = ord.BillTo
                ord.ShipToName = ord.BillToName
                ord.ShipToAttention = ord.BillToAttention
                ord.ShipToAddress1 = ord.BillToAddress1
                ord.ShipToAddress2 = ord.BillToAddress2
                ord.ShipToAddress3 = ord.BillToAddress3
                ord.ShipToAddress4 = ord.BillToAddress4
                ord.ShipToCity = ord.BillToCity
                ord.ShipToProvince = ord.BillToProvince
                ord.ShipToPostalCode = ord.BillToPostalCode
                ord.ShipToCountry = ord.BillToCountry
            End If
        Catch ex As Exception
            Dim strError As String = "Failed to Read Delivery Address:" & ex.Message.ToString & ord.ShipTo & ord.BillTo
            strError &= System21CompanyNo & ord.CUSN40 & ord.DSEQ40
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR72"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadInvoiceSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        ' Read Invoice Header OEP65
        ' 
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim fieldOrdinal(4) As String
        Dim totalInvoice As Decimal = 0
        Dim totalTax As Decimal = 0
        Dim totalWeight As Decimal = 0
        Try
            Const sqlSelect As String = "SELECT * FROM OEP65 WHERE CONO65 = @PARAM1 and ORDN65 = @PARAM2"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 15).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
            End Select
            dtrReader = cmdSelect.ExecuteReader()
            With dtrReader
                fieldOrdinal(1) = .GetOrdinal("TOWT65")
                fieldOrdinal(2) = .GetOrdinal("DTIN65")
                fieldOrdinal(3) = .GetOrdinal("INVV65")
                fieldOrdinal(4) = .GetOrdinal("INVT65")
                If .HasRows Then
                    .Read()
                    ord.TotalWeight = .GetDecimal(fieldOrdinal(1))
                    ' ord.InvoiceDate = .GetDateTime(fieldOrdinal(2))
                    ord.InvoiceDate = Utilities.ISeriesDate(.GetString(.GetOrdinal("DTIN65")))
                    If ord.OrderStatus = "C" Then
                        totalTax += .GetDecimal(fieldOrdinal(3))
                        totalInvoice += .GetDecimal(fieldOrdinal(4))
                    End If
                    While .Read
                        ' If order is complete then total up invoice
                        If ord.OrderStatus = "C" Then
                            totalTax += .GetDecimal(fieldOrdinal(3))
                            totalInvoice += .GetDecimal(fieldOrdinal(4))
                            ord.TotalWeight += .GetDecimal(fieldOrdinal(1))
                        End If
                    End While
                End If

                ord.TotalWeight = totalWeight
                ord.TotalTax = totalTax

                .Close()
            End With
            dtrReader.Close()
        Catch ex As Exception
            Const strError As String = "Failed to Read Invoice Header "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR73"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadOrderHeaderSystem21() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        ' Read Order Header OEP40
        ' 
        'Const sqlSelect As String = "SELECT * FROM ORDERHDR WHERE CONO40 = @PARAM1 AND ORDN40 = @PARAM2"
        Const sqlSelect As String = "SELECT * FROM OEP40 WHERE CONO40 = @PARAM1 AND ORDN40 = @PARAM2"
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Try
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            '-----------------------------------------------------------------------------
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
            End Select
            '-----------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            With dtrReader
                While dtrReader.Read
                    ord.CustomerPO = .GetString(.GetOrdinal("CUSO40"))          ' CUSO40 on ORDERHDR
                    ord.CUSN40 = .GetString(.GetOrdinal("CUSN40"))              ' CUSO40 on ORDERHDR
                    ord.DSEQ40 = .GetString(.GetOrdinal("DSEQ40"))              ' DSEQ40 on ORDERHDR
                    '---------------------------------------------------------------------
                    ''Const str0 As String = "OrderDetailResponse"
                    ''If Settings.WebServiceName = str0 Then
                    ord.HoldReason = .GetString(.GetOrdinal("SUSP40"))          ' SUSP40 on ORDERHDR
                    ord.OECarrier = .GetString(.GetOrdinal("CARR40"))           ' CARR40 on ORDERHDR 

                    ord.OrderWeight = .GetString(.GetOrdinal("TOWT40"))         ' Try TOWT40 and WUOM40 on OEP40 
                    ord.OrderEntryDate = Utilities.ISeriesDate(.GetString(.GetOrdinal("DTSO40")))   ' DTSO40 on ORDERHDR
                    ord.OrderType = .GetString(.GetOrdinal("ORTP40"))           ' ORTP40 on ORDERHDR 
                    ord.CurrencyCode = .GetString(.GetOrdinal("CURN40"))
                    ord.CurrencyRate = .GetString(.GetOrdinal("CURT40"))

                    If .GetString(.GetOrdinal("DTLP40")) = "0" Then
                        ord.PromiseDate = ord.OrderEntryDate
                    Else
                        ord.PromiseDate = Utilities.ISeriesDate(.GetString(.GetOrdinal("DTLP40")))         ' DTLP40 on ORDERHDR
                    End If
                    ord.TermsCode = .GetString(.GetOrdinal("PAYT40"))           ' PAYT40 on ORDERHDR
                    ord.OrderStatus = .GetString(.GetOrdinal("STAT40"))         ' STAT40 on ORDERHDR
                    ord.TotalSales = .GetDecimal(.GetOrdinal("BORV40"))         ' Base order value (in �. Use ORVL40 if need value in order currency)
                    If ord.OrderStatus = "C" Then
                        ord.ShipComplete = "Y"
                    End If
                    ''End If
                    '---------------------------------------------------------------------
                    ''Const str1 As String = "OrderStatusRequest"
                    ''If Settings.WebServiceName = str1 Then
                    ''ord.OrderStatus = .GetString(.GetOrdinal("STAT40"))         ' STAT40 on ORDERHDR 
                    ''ord.OrderEntryDate = Utilities.ISeriesDate(.GetString(.GetOrdinal("DTSO40")))   ' DTSO40 on ORDERHDR
                    ''End If
                    '---------------------------------------------------------------------
                End While
                .Close()
            End With
        Catch ex As Exception
            '      Dim strError As String = "Failed to Read Order Header:" & ex.Message
            Const strError As String = "Failed to Read Order Header:" ' & ex.Message
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR74"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadOrderLinesSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   Read Order Detail Lines OEP55
        ' 
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim fieldOrdinal(7) As String
        Try
            'Const sqlSelect5 As String = "SELECT * FROM OEP55 WHERE CONO55 = @PARAM1 AND ORDN55 = @PARAM2"
            Const sqlSelect5 As String = "SELECT LOCD55,CATN55,QTAL55,QTOR55,ORDL55,UPRC55,STAT55,QTDS55, PDES35, QTOS55, PODD55" & _
                                        " FROM OEP55 INNER JOIN INP35 ON " & _
                                        " CONO55 = CONO35 AND CATN55 = PNUM35 " & _
                                        " WHERE CONO55 = @PARAM1 AND ORDN55 = @PARAM2"
            cmdSelect = New iDB2Command(sqlSelect5, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                Case Is = "285"
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
            End Select
            '-----------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            fieldOrdinal(1) = dtrReader.GetOrdinal("LOCD55")
            fieldOrdinal(2) = dtrReader.GetOrdinal("CATN55")
            fieldOrdinal(3) = dtrReader.GetOrdinal("PDES35")
            fieldOrdinal(4) = dtrReader.GetOrdinal("PODD55")

            Dim lineNumber As Integer = 0
            While dtrReader.Read
                '-------------------------------------------------------------------------
                '   Add to Detail Table
                '
                dRow = Nothing
                dRow = DtDetail.NewRow()
                dRow("AllocatedQuantity") = dtrReader("QTAL55")
                dRow("BackOrderQuantity") = dtrReader("QTOS55")
                dRow("LineNumber") = dtrReader("ORDL55")
                lineNumber = CInt(dtrReader("ORDL55"))
                dRow("WestcoastLineNumber") = dtrReader("ORDL55")
                dRow("OrderNumber") = System21OrderNo
                dRow("OrderQuantity") = dtrReader("QTOR55")
                dRow("ShipFromBranch") = dtrReader.GetString(fieldOrdinal(1))
                dRow("Sku") = dtrReader.GetString(fieldOrdinal(2))
                dRow("UnitPrice") = dtrReader("UPRC55")
                dRow("LineStatus") = dtrReader("STAT55")
                dRow("SKUDescription") = dtrReader.GetString(fieldOrdinal(3))
                dRow("ShipQuantity") = dtrReader("QTDS55")

                Dim directDelivery As String = dtrReader.GetString(fieldOrdinal(4))

                If directDelivery = "1" Then
                    dRow("AllocatedQuantity") = dRow("OrderQuantity")
                    dRow("BackOrderQuantity") = 0
                End If

                '------------------------------------------------------------------------
                ''Const str0 As String = "OrderDetailResponse"
                ''If Settings.WebServiceName = str0 Then
                If dtrReader("QTOR55") <> dtrReader("QTDS55") Then
                    dRow("backOrderStatus") = "Y" ' QTDS55 <> QTOR55 on OEP55   
                End If
                '-----------------------
                ' Attempt to get ETADate
                '-----------------------
                Dim cmdSelectETA As iDB2Command = Nothing
                Dim dtrReaderETA As iDB2DataReader = Nothing

                Try
                    Const sqlSelectETA As String = "SELECT * FROM OEAFLAA WHERE " & _
                                                 " CONOAF = @CONO AND " & _
                                                 " ORDNAF = @ORDN AND " & _
                                                 " ORDLAF = @ORDL "
                    cmdSelectETA = New iDB2Command(sqlSelectETA, conSystem21)
                    Select Case Settings.DatabaseType1
                        Case Is = T65535
                            cmdSelectETA.Parameters.Add("CONO", iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                            cmdSelectETA.Parameters.Add("ORDN", iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                            cmdSelectETA.Parameters.Add("ORDL", iDB2DbType.iDB2Integer, 3).Value = lineNumber
                            'cmdSelectETA.Parameters.Add("CONO", iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                            'cmdSelectETA.Parameters.Add("ORDN", iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                            'cmdSelectETA.Parameters.Add("ORDL", iDB2DbType.iDB2Integer, 3).Value = lineNumber
                        Case Is = "285"
                            cmdSelectETA.Parameters.Add("CONO", iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                            cmdSelectETA.Parameters.Add("ORDN", iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                            cmdSelectETA.Parameters.Add("ORDL", iDB2DbType.iDB2Integer, 3).Value = lineNumber
                        Case Else
                            cmdSelectETA.Parameters.Add("CONO", iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                            cmdSelectETA.Parameters.Add("ORDN", iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                            cmdSelectETA.Parameters.Add("ORDL", iDB2DbType.iDB2Integer, 3).Value = lineNumber
                    End Select
                    '-----------------------------------------------------------------------------
                    dtrReaderETA = cmdSelectETA.ExecuteReader()
                    If dtrReaderETA.HasRows Then
                        With dtrReaderETA
                            .Read()
                            dRow("BackOrderETADate") = Utilities.ISeriesDate(.GetString(.GetOrdinal("REF2AF")))
                        End With
                    End If
                    '-------------------------------------------------------------------------
                Catch ex As Exception
                    ' Don't return an error..
                End Try
                '---------------------------
                ' Attempt to get CustomerSKU
                '---------------------------
                dRow("CustomerSKU") = String.Empty
                Dim cmdSelectCustomerSKU As iDB2Command = Nothing
                Dim dtrReaderCustomerSKU As iDB2DataReader = Nothing

                Try
                    Const sqlSelectCustomerSKU As String = "SELECT * FROM INITMRF WHERE " & _
                                                 " CONO38 = @CONO AND " & _
                                                 " PNUM38 = @PNUM AND " & _
                                                 " EXTA38 = @EXTA "
                    cmdSelectCustomerSKU = New iDB2Command(sqlSelectCustomerSKU, conSystem21)
                    Select Case Settings.DatabaseType1
                        Case Is = T65535
                            cmdSelectCustomerSKU.Parameters.Add("@CONO", iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                            cmdSelectCustomerSKU.Parameters.Add("@PNUM", iDB2DbType.iDB2Char, 15).Value = dRow("Sku").ToString.Trim
                            cmdSelectCustomerSKU.Parameters.Add("@EXTA", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1
                        Case Is = "285"
                            cmdSelectCustomerSKU.Parameters.Add("@CONO", iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                            cmdSelectCustomerSKU.Parameters.Add("@PNUM", iDB2DbType.iDB2Char, 15).Value = dRow("Sku").ToString.Trim
                            cmdSelectCustomerSKU.Parameters.Add("@EXTA", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1
                        Case Else
                            cmdSelectCustomerSKU.Parameters.Add("@CONO", iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                            cmdSelectCustomerSKU.Parameters.Add("@PNUM", iDB2DbType.iDB2Char, 15).Value = dRow("Sku").ToString.Trim
                            cmdSelectCustomerSKU.Parameters.Add("@EXTA", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1
                    End Select
                    '-----------------------------------------------------------------------------
                    dtrReaderCustomerSKU = cmdSelectCustomerSKU.ExecuteReader()
                    If dtrReaderCustomerSKU.HasRows Then
                        With dtrReaderCustomerSKU
                            .Read()
                            dRow("CustomerSKU") = .GetString(.GetOrdinal("ALTI38")).Trim
                        End With
                    End If
                    '-------------------------------------------------------------------------
                Catch ex As Exception
                    ' Don't return an error..
                End Try

                DtDetail.Rows.Add(dRow)
            End While
            '-----------------------------------------------------------------------------
            '   Read order text
            ' 
            If DtText.Columns.Count = 0 Then DtText_Columns()
            Dim text As String = String.Empty
            Dim textLineNo As Integer = 0
            Const sqlSelect6 As String = "SELECT * FROM INP40 WHERE CONO40 = @PARAM1 AND TTYP40 = 'O' AND TREF40 = @PARAM2 AND USGC40 = 'E'"
            cmdSelect = New iDB2Command(sqlSelect6, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 15).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
            End Select
            dtrReader = cmdSelect.ExecuteReader()
            fieldOrdinal(1) = dtrReader.GetOrdinal("TLIN40")
            While dtrReader.Read
                '-------------------------------------------------------------------------
                ' Add to Detail Table
                ' 
                dRow = Nothing
                dRow = DtText.NewRow()
                dRow("OrderNumber") = System21OrderNo
                dRow("Text") = dtrReader.GetString(fieldOrdinal(1))
                dRow("TextLineNumber") = dtrReader("TLNO40")
                DtText.Rows.Add(dRow)
            End While
            dtrReader.Close()
        Catch ex As Exception
            Const strError As String = "Failed to Read Order Lines "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR75"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function ReadSalesDispatchSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        ' Read Sales Dispatch Header INP56              (INP56 / INP57 are the header / details for sales despatch notes)
        ' 
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim fieldOrdinal(2) As String
        Try
            Const sqlSelect As String = "SELECT DSDT56, LOCD56 FROM INP56 WHERE CONO56 = @PARAM1 and ORDN56 = @PARAM2"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 15).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
            End Select
            dtrReader = cmdSelect.ExecuteReader()
            With dtrReader
                fieldOrdinal(1) = .GetOrdinal("DSDT56")
                fieldOrdinal(2) = .GetOrdinal("LOCD56")
                If .HasRows Then
                    .Read()
                    ord.ShippedDate = Utilities.ISeriesDate(.GetString(.GetOrdinal("DSDT56")))
                    ord.ShipFromBranchNumber = .GetString(fieldOrdinal(2))
                    ord.ItemsShipped = True
                End If
                .Close()
            End With
            dtrReader.Close()
        Catch ex As Exception
            Const strError As String = "Failed to Read Sales Dispatch Header "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR76"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadPackagesSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   Read Picking Dispatch Details INP57 
        '
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Try
            'Const sqlSelect As String = "SELECT ORDL57,DESN57,RDSQ57,TOWT57,DRTE57 " & _
            '                            " FROM INP57 " & _
            '                            " WHERE ORTP57 = '1' AND CONO57 = @PARAM1 AND ORDN57 = @PARAM2 " & _
            '                            " ORDER BY ORDL57"
            Const sqlSelect As String = "SELECT LOCD57,ORDL57,DESN57,CATN57,RDSQ57,TOWT57,DRTE57,QTDS57, DTIN65, INVN65, INVT65, DTAL65 " & _
                                      " FROM INP57 as A left join OEP65 as B on  " & _
                                      " CONO57 = CONO65 and ORDN57 = ORDN65 and DESN57 = DESN65 " & _
                                      " WHERE ORTP57 = '1' AND CONO57 = @PARAM1 AND ORDN57 = @PARAM2 " & _
                                      " ORDER BY DESN57, ORDL57"
            cmdSelect = New iDB2Command(sqlSelect, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 15).Value = System21OrderNo
                Case Is = T285
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = Settings.AccountNo3
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 15).Value = System21OrderNo
            End Select
            dtrReader = cmdSelect.ExecuteReader()

            Dim ordl57Ordinal As Integer = dtrReader.GetOrdinal("ORDL57")   ' Order line
            Dim desn57Ordinal As Integer = dtrReader.GetOrdinal("DESN57")   ' Despatch number
            Dim rdsq57Ordinal As Integer = dtrReader.GetOrdinal("RDSQ57")   ' Drop Sequence
            Dim catn57Ordinal As Integer = dtrReader.GetOrdinal("CATN57")   ' Part No
            Dim towt57Ordinal As Integer = dtrReader.GetOrdinal("TOWT57")   ' Total weight 
            Dim drte57Ordinal As Integer = dtrReader.GetOrdinal("DRTE57")   ' Delivery Route
            Dim qtds57Ordinal As Integer = dtrReader.GetOrdinal("QTDS57")   ' Quantity Despatched
            Dim invn65Ordinal As Integer = dtrReader.GetOrdinal("INVN65")   ' Invoice Number
            Dim dtin65Ordinal As Integer = dtrReader.GetOrdinal("DTIN65")   ' Date invoiced
            Dim locd57Ordinal As Integer = dtrReader.GetOrdinal("LOCD57")   ' Warehouse
            Dim dtal65Ordinal As Integer = dtrReader.GetOrdinal("DTAL65")   ' Date allocated
            Dim invt65Ordinal As Integer = dtrReader.GetOrdinal("INVT65")   ' Invoice total
            With dtrReader
                While .Read

                    dRow = Nothing
                    dRow = DtPackage.NewRow()
                    dRow("OrderNumber") = System21OrderNo
                    dRow("OrderLine") = .GetInt32(ordl57Ordinal)                               ' 
                    dRow("OrderSuffixID") = ord.CarrierCode
                    dRow("PackageID") = .GetString(desn57Ordinal).Trim
                    dRow("PartNumber") = .GetString(catn57Ordinal).Trim
                    dRow("BoxNumber") = .GetString(rdsq57Ordinal).Trim
                    dRow("BoxWeight") = .GetDecimal(towt57Ordinal)
                    dRow("ShipDate") = ord.ShippedDate
                    dRow("TrackingURL") = .GetString(drte57Ordinal).Trim

                    If Not .IsDBNull(invn65Ordinal) Then
                        dRow("InvoiceNumber") = .GetString(.GetOrdinal("INVN65"))
                    End If

                    If Not .IsDBNull(dtin65Ordinal) Then
                        dRow("InvoiceDate") = Utilities.ISeriesDate(.GetString(dtin65Ordinal))
                    End If

                    dRow("Stockroom") = .GetString(.GetOrdinal("LOCD57"))
                    If Not .IsDBNull(dtal65Ordinal) AndAlso .GetString(dtal65Ordinal) <> "0" Then
                        dRow("AllocationDate") = Utilities.ISeriesDate(.GetString(dtal65Ordinal))
                    End If
                    If Not .IsDBNull(invt65Ordinal) Then
                        dRow("InvoiceTotal") = .GetDecimal(invt65Ordinal)
                    End If
                    dRow("Quantity") = .GetDecimal(qtds57Ordinal)
                    '-------------------------------
                    ' Attempt to get carrier Details
                    '-------------------------------
                    CarrierName = String.Empty
                    ConsignmentNumber = String.Empty
                    ReadCarrierDetsSystem21(.GetInt16(desn57Ordinal))

                    dRow("CarrierName") = CarrierName
                    dRow("ConsignmentNumber") = ConsignmentNumber

                    '---------------------------------------------------------------------
                    DtPackage.Rows.Add(dRow)
                End While
                .Close()
            End With
            dtrReader.Close()
        Catch ex As Exception
            Const strError As String = "Failed to Read Package Dispatch Details "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOR76"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadSerialNumbersSystem21() As ErrorObj
        Dim err As New ErrorObj

        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim fieldOrdinal(7) As String

        Try
            Const sqlSelect5 As String = "SELECT * FROM DEAM WHERE " & _
                                         " CONOAM = @PARAM1 AND " & _
                                         " ORDNAM = @PARAM2  "

            cmdSelect = New iDB2Command(sqlSelect5, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                Case Is = "285"
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
            End Select
            '-----------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            If dtrReader.HasRows Then
                With dtrReader

                    While dtrReader.Read

                        dRow = Nothing
                        dRow = DtProduct.NewRow()
                        dRow("OrderNumber") = System21OrderNo
                        dRow("OrderLine") = .GetString(.GetOrdinal("ORDLAM")).Trim
                        dRow("PackageID") = .GetString(.GetOrdinal("DESNAM")).Trim
                        dRow("SKU") = .GetString(.GetOrdinal("ITEMAM")).Trim
                        dRow("SerialNumber") = .GetString(.GetOrdinal("SENOAM")).Trim
                        DtProduct.Rows.Add(dRow)
                    End While
                End With

            End If

            '-------------------------------------------------------------------------

        Catch ex As Exception
            ' Don't return an error..

            'Const strError As String = "Failed to Read Serial Numbers "
            'With err
            '    .ErrorMessage = ex.Message
            '    .ErrorStatus = strError
            '    .ErrorNumber = "TACDBOR79"
            '    .HasError = True
            'End With
        End Try
        Return err
    End Function
    Private Function ReadCarrierDetsSystem21(ByVal despatchNo As Int16) As ErrorObj
        Dim err As New ErrorObj

        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim dRow As DataRow = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim param3 As String = "@PARAM3"
        Dim fieldOrdinal(7) As String

        Try
            Const sqlSelect5 As String = "SELECT * FROM DEAK WHERE " & _
                                         " CONOAK = @PARAM1 AND " & _
                                         " ORDNAK = @PARAM2 AND " & _
                                         " DESNAK = @PARAM3 "

            cmdSelect = New iDB2Command(sqlSelect5, conSystem21)
            Select Case Settings.DatabaseType1
                Case Is = T65535
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = System21OrderNo
                    cmdSelect.Parameters.Add(param3, iDB2DbType.iDB2Integer, 2).Value = despatchNo
                Case Is = "285"
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                    cmdSelect.Parameters.Add(param3, iDB2DbType.iDB2Char, 7).Value = despatchNo
                Case Else
                    cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = System21CompanyNo
                    cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = System21OrderNo
                    cmdSelect.Parameters.Add(param3, iDB2DbType.iDB2Char, 7).Value = despatchNo
            End Select
            '-----------------------------------------------------------------------------
            dtrReader = cmdSelect.ExecuteReader()
            If dtrReader.HasRows Then
                With dtrReader
                    .Read()
                    CarrierName = .GetString(.GetOrdinal("CARRAK")).Trim
                    ConsignmentNumber = .GetString(.GetOrdinal("CONSAK")).Trim
                End With
            End If
            '-------------------------------------------------------------------------
        Catch ex As Exception
            ' Don't return an error..

            'Const strError As String = "Failed to Read Serial Numbers "
            'With err
            '    .ErrorMessage = ex.Message
            '    .ErrorStatus = strError
            '    .ErrorNumber = "TACDBOR79"
            '    .HasError = True
            'End With
        End Try
        Return err
    End Function

    Private Function DtHeader_Add() As DataRow
        '-----------------------------------------------------------------------------
        '   Add to Header Table
        ' 
        Dim dRow As DataRow
        dRow = Nothing
        dRow = DtHeader.NewRow()
        Try
            dRow("BranchOrderNumber") = ord.BranchOrderNumber.Trim
            dRow("CarrierCode") = ord.CarrierCode.Trim
            dRow("CarrierCodeDescription") = ord.CarrierCodeDescription.Trim
            dRow("CustomerPO") = ord.CustomerPO.Trim
            dRow("DistributionWeight") = ord.DistributionWeight.Trim
            dRow("FreightRate") = ord.FreightRate
            dRow("OrderNumber") = System21OrderNo.Trim
            '-------------------------------------------------
            dRow("ShipToAttention") = ord.ShipToAttention.Trim
            dRow("ShipToName") = ord.ShipToName.Trim
            dRow("ShipToSuffix") = ord.DSEQ40
            dRow("ShipToAddress1") = ord.ShipToAddress1.Trim
            dRow("ShipToAddress2") = ord.ShipToAddress2.Trim
            dRow("ShipToAddress3") = ord.ShipToAddress3.Trim
            dRow("ShipToAddress4") = ord.ShipToAddress3.Trim
            dRow("ShipToCity") = ord.ShipToCity.Trim
            dRow("ShipToProvince") = ord.ShipToProvince.Trim
            dRow("ShipToPostalCode") = ord.ShipToPostalCode.Trim
            dRow("ShipToCountry") = ord.ShipToCountry.Trim
            '-------------------------------------------------
            dRow("BillToAttention") = ord.BillToAttention.Trim
            dRow("BillToName") = ord.BillToName.Trim
            dRow("BillToAddress1") = ord.BillToAddress1.Trim
            dRow("BillToAddress2") = ord.BillToAddress2.Trim
            dRow("BillToAddress3") = ord.BillToAddress3.Trim
            dRow("BillToAddress4") = ord.BillToAddress3.Trim
            dRow("BillToCity") = ord.BillToCity.Trim
            dRow("BillToProvince") = ord.BillToProvince.Trim
            dRow("BillToPostalCode") = ord.BillToPostalCode.Trim
            dRow("BillToCountry") = ord.BillToCountry.Trim
            dRow("ThirdPartyFreight") = ord.ThirdPartyFreight.Trim
            '-------------------------------------------------
            ''Const str0 As String = "OrderDetailResponse"
            ''If Settings.WebServiceName = str0 Then
            dRow("BackOrderStatus") = ord.BackOrderStatus
            dRow("ConfigFlag") = ord.ConfigFlag
            dRow("ConfigTimeStamp") = ord.ConfigTimeStamp
            dRow("CreditCardSW") = ord.CreditCardSW
            dRow("CreditMemoReasonCode") = ord.CreditMemoReasonCode
            dRow("EndUserPO") = ord.EndUserPO
            dRow("EntryMethod") = ord.EntryMethod
            dRow("FrtOutCode") = ord.FrtOutCode
            dRow("FulfillmentFlag") = ord.FulfillmentFlag
            dRow("GovEndUserType") = ord.GovEndUserType
            dRow("HoldReason") = ord.HoldReason
            dRow("InvoiceDate") = ord.InvoiceDate
            dRow("NumberOfCartons") = ord.NumberOfCartons
            dRow("OECarrier") = ord.OECarrier
            dRow("OrderEntryDate") = ord.OrderEntryDate
            dRow("OrderWeight") = ord.OrderWeight
            dRow("OrderType") = ord.OrderType
            dRow("PromiseDate") = ord.PromiseDate
            dRow("ProNbr") = ord.ProNbr
            dRow("ProNbrSW") = ord.ProNbrSW
            dRow("ResellerNBR") = ord.ResellerNBR
            dRow("RMACode") = ord.RMACode
            dRow("SelSrcAcctnoHdr") = ord.SelSrcAcctnoHdr
            dRow("SelSrcSlsHdr") = ord.SelSrcSlsHdr
            dRow("ShipComplete") = ord.ShipComplete
            dRow("ShippableSW") = ord.ShippableSW
            dRow("SplitBillToSwitch") = ord.SplitBillToSwitch
            dRow("SplitFromOrderNumber") = ord.SplitFromOrderNumber
            dRow("TermsCode") = ord.TermsCode
            dRow("TermID") = ord.TermID
            ''End If
            '-------------------------------------------------
            'Const str1 As String = "OrderStatusRequest"
            'If Settings.WebServiceName = str1 Then
            ''dRow("InvoiceDate") = ord.InvoiceDate
            ''dRow("OrderCreditMemoCode") = ord.OrderCreditMemoCode
            ''dRow("OrderCreditMemo1") = ord.OrderCreditMemo1
            ''dRow("OrderCreditMemo2") = ord.OrderCreditMemo2
            ''dRow("OrderEntryDate") = ord.OrderEntryDate
            dRow("OrderStatus") = ord.OrderStatus
            dRow("ShippedDate") = ord.ShippedDate
            dRow("ItemsShipped") = ord.ItemsShipped
            dRow("ShipFromBranch") = ord.ShipFromBranch
            dRow("ShipFromBranchNumber") = ord.ShipFromBranchNumber
            dRow("SalesTotal") = ord.TotalSales
            ''End If
            '-------------------------------------------------
            ''Const str2 As String = "OrderTrackingRequest"
            ''If Settings.WebServiceName = str2 Then
            dRow("CartonCount") = ord.CartonCount
            dRow("TotalWeight") = ord.TotalWeight
            dRow("CurrencyCode") = ord.CurrencyCode
            dRow("CurrencyRate") = ord.CurrencyRate
            dRow("TaxTotal") = ord.TotalTax
            dRow("FreightTotal") = ord.FreightRate
            dRow("SalePlusTax") = ord.SalePlusTax
            dRow("GrandTotal") = ord.GrandTotal
            ''End If
            ' 
        Catch ex As Exception

        End Try

        Return dRow
    End Function

    Private Sub DtHeader_Columns()
        '---------------------------------------------------
        '   dtHeader            
        '   
        With DtHeader.Columns
            '   <OrderNumbers>
            .Add("BranchOrderNumber", GetType(String))
            .Add("CarrierCode", GetType(String))
            .Add("CarrierCodeDescription", GetType(String))
            .Add("ContractNumber", GetType(Decimal))
            .Add("CustomerPO", GetType(String))
            .Add("DistributionWeight", GetType(String))
            .Add("FreightRate", GetType(Decimal))
            .Add("OrderNumber", GetType(String))
            '   <Address Type="ShipTo">
            .Add("ShipToAttention", GetType(String))
            .Add("ShipToName", GetType(String))
            .Add("ShipToAddress1", GetType(String))
            .Add("ShipToAddress2", GetType(String))
            .Add("ShipToAddress3", GetType(String))
            .Add("ShipToAddress4", GetType(String))
            .Add("ShipToCity", GetType(String))
            .Add("ShipToProvince", GetType(String))
            .Add("ShipToPostalCode", GetType(String))
            .Add("ShipToSuffix", GetType(String))
            .Add("ShipToCountry", GetType(String))
            '   <Address Type="BillTo">
            .Add("BillToAttention", GetType(String))
            .Add("BillToName", GetType(String))
            .Add("BillToAddress1", GetType(String))
            .Add("BillToAddress2", GetType(String))
            .Add("BillToAddress3", GetType(String))
            .Add("BillToAddress4", GetType(String))
            .Add("BillToCity", GetType(String))
            .Add("BillToProvince", GetType(String))
            .Add("BillToPostalCode", GetType(String))
            .Add("BillToSuffix", GetType(String))
            .Add("BillToCountry", GetType(String))
            .Add("ThirdPartyFreight", GetType(String))
            '   <OrderInformation>
            .Add("BackOrderStatus", GetType(String))
            .Add("ConfigFlag", GetType(String))
            .Add("ConfigTimeStamp", GetType(String))
            .Add("CreditCardSW", GetType(String))
            .Add("CreditMemoReasonCode", GetType(String))
            .Add("EndUserPO", GetType(String))
            .Add("EntryMethod", GetType(String))
            .Add("FrtOutCode", GetType(String))
            .Add("FulfillmentFlag", GetType(String))
            .Add("GovEndUserType", GetType(String))
            .Add("HoldReason", GetType(String))
            .Add("InvoiceDate", GetType(Date))
            .Add("NumberOfCartons", GetType(String))
            .Add("OECarrier", GetType(String))
            .Add("OrderWeight", GetType(String))
            .Add("OrderEntryDate", GetType(Date))
            .Add("OrderType", GetType(String))
            .Add("PromiseDate", GetType(Date))
            .Add("ProNbrSW", GetType(String))
            .Add("ProNbr", GetType(String))
            .Add("ResellerNBR", GetType(String))
            .Add("RMACode", GetType(String))
            .Add("SelSrcAcctnoHdr", GetType(String))
            .Add("SelSrcSlsHdr", GetType(String))
            .Add("ShipComplete", GetType(String))
            .Add("ShippableSW", GetType(String))
            .Add("SplitBillToSwitch", GetType(String))
            .Add("SplitFromOrderNumber", GetType(String))
            .Add("TermsCode", GetType(String))
            .Add("TermID", GetType(String))
            '   <OrderTotals>
            .Add("SalesTotal", GetType(String))
            .Add("FreightTotal", GetType(String))
            .Add("TaxTotal", GetType(String))
            .Add("SalePlusTax", GetType(String))
            .Add("GrandTotal", GetType(String))
            .Add("CODAmount", GetType(String))
            .Add("DiscountAmount", GetType(String))
            .Add("CurrencyCode", GetType(String))
            .Add("CompanyCurrency", GetType(String))
            .Add("CurrencyRate", GetType(String))
            '   </OrderTotals>
            .Add("OrderStatus", GetType(String))
            .Add("ShippedDate", GetType(Date))
            .Add("ItemsShipped", GetType(Boolean))
            .Add("ShipFromBranch", GetType(String))
            .Add("ShipFromBranchNumber", GetType(String))

            .Add("CartonCount", GetType(String))
            .Add("TotalWeight", GetType(String))
            ''End If
            '-------------------------------------------------
        End With
        '
    End Sub
    Private Sub DtDetail_Columns()
        '---------------------------------------------------
        '   dtDetail        
        '   
        With DtDetail.Columns
            .Add("AllocatedQuantity", GetType(Integer))
            .Add("BackOrderQuantity", GetType(Double))
            .Add("BackOrderStatus", GetType(String))
            .Add("LineNumber", GetType(Integer))
            .Add("OrderNumber", GetType(String))
            .Add("OrderQuantity", GetType(Double))
            .Add("Sku", GetType(String))
            .Add("ShipFromBranch", GetType(String))
            .Add("Text", GetType(String))
            .Add("TextLineNumber", GetType(Integer))
            .Add("UnitPrice", GetType(Decimal))
            '-------------------------------------------------
            ''Const str0 As String = "OrderDetailResponse"
            ''If Settings.WebServiceName = str0 Then
            ''    '                                         <ProductLine>
            .Add("BackOrderETADate", GetType(Date))
            .Add("ComponentQty", GetType(Double))
            .Add("CustomerLineNumber", GetType(Integer))
            .Add("ExtendedLineSales", GetType(Decimal))
            .Add("FreeItemSwitch", GetType(String))
            .Add("LineSalesTotal", GetType(Decimal))
            .Add("LineTerms", GetType(Integer))
            .Add("NonWayPromiseDate", GetType(Date))
            .Add("ResellerUnitPrice", GetType(Decimal))
            .Add("ReserveSequenceNbr", GetType(String))
            .Add("ShipQuantity", GetType(Double))
            .Add("SelSrcSls", GetType(String))
            .Add("SelSrcAcctno", GetType(String))
            .Add("SKUDescription", GetType(String))
            .Add("SystemComponentSwitch", GetType(String))
            .Add("UnitOfMeasure", GetType(String))
            .Add("VendorName", GetType(String))
            .Add("VendorPartNumber", GetType(String))
            .Add("WestCoastLineNumber", GetType(Integer))
            '                                       <ConfigInformation>
            .Add("ConfigIndicator", GetType(String))
            .Add("ConfigStatus", GetType(String))
            .Add("ConfigAssemblyCode", GetType(String))
            .Add("ConfigLabCode", GetType(String))
            .Add("ConfigOnHoldSw", GetType(String))
            .Add("ConfigPcrCnt", GetType(String))
            .Add("ConfigPchCnt", GetType(String))
            .Add("ConfigStgCnt", GetType(String))
            .Add("ConfigSthCnt", GetType(String))
            .Add("ConfigWipCnt", GetType(String))
            .Add("ConfigQaaCnt", GetType(String))
            .Add("ConfigQahCnt", GetType(String))
            .Add("ConfigBinCnt", GetType(String))
            .Add("ConfigOshCnt", GetType(String))
            .Add("ConfigHoldReasonText", GetType(String))
            ''End If
            ' ''-------------------------------------------------
            ''Const str2 As String = "OrderTrackingRequest"
            ''If Settings.WebServiceName = str2 Then
            .Add("BoxNumber", GetType(String))
            .Add("BoxWeight", GetType(Double))
            .Add("Contents", GetType(String))
            .Add("PackageID", GetType(String))
            .Add("ShipDate", GetType(Date))
            .Add("TotalWeight", GetType(Double))
            .Add("TrackingURL", GetType(String))
            .Add("LineStatus", GetType(String))
            .Add("CustomerSKU", GetType(String))
            ''End If
            '-------------------------------------------------
        End With
        '
    End Sub
    Private Sub DtText_Columns()
        '---------------------------------------------------
        '   dtText              
        '
        With DtText.Columns                              ' CommentLine
            .Add("OrderNumber", GetType(String))
            .Add("Text", GetType(String))
            .Add("TextLineNumber", GetType(Integer))
        End With
        '
    End Sub
    Private Sub DtCarrier_Columns()
        '---------------------------------------------------
        '   dtCarrier            
        '   
        With DtCarrier.Columns

            .Add("Carrier", GetType(String))
            .Add("CarrierCode", GetType(String))
            .Add("CarrierCodeDescription", GetType(String))
            .Add("DistributionWeight", GetType(Double))
            .Add("FreightRate", GetType(Double))
            .Add("OrderNumber", GetType(String))
            .Add("SuffixErrorResponse", GetType(String))
            .Add("SuffixErrorType", GetType(String))
            .Add("InvoiceDate", GetType(Date))

            .Add("OrderCreditMemoCode1", GetType(String))
            .Add("OrderCreditMemoCode2", GetType(String))
            .Add("OrderCreditMemo1", GetType(String))
            .Add("OrderCreditMemo2", GetType(String))
            .Add("OrderShipDate", GetType(Date))
            .Add("OrderStatus", GetType(String))
            .Add("ShipFromBranch", GetType(String))
            .Add("ShipFromBranchNumber", GetType(String))
            .Add("Suffix", GetType(String))
            .Add("TotalSales", GetType(String))

        End With
        '
    End Sub
    Private Sub DtPackage_Columns()
        '---------------------------------------------------
        '   DtPackage              
        '
        With DtPackage.Columns                              ' Sku Serial Number
            .Add("OrderNumber", GetType(String))
            .Add("OrderLine", GetType(String))
            .Add("OrderSuffixID", GetType(String))
            .Add("PackageID", GetType(String))
            .Add("PartNumber", GetType(String))
            .Add("BoxNumber", GetType(String))
            .Add("BoxWeight", GetType(Double))
            .Add("ShipDate", GetType(Date))
            .Add("TrackingURL", GetType(String))
            .Add("InvoiceNumber", GetType(String))
            .Add("InvoiceDate", GetType(Date))
            .Add("Stockroom", GetType(String))
            .Add("AllocationDate", GetType(Date))
            .Add("InvoiceTotal", GetType(Double))
            .Add("Quantity", GetType(Double))
            .Add("CarrierName", GetType(String))
            .Add("ConsignmentNumber", GetType(String))
        End With
        '
    End Sub
    Private Sub DtProduct_Columns()
        '---------------------------------------------------
        '   DtPackage              
        '
        With DtProduct.Columns                              ' Sku Serial Number
            .Add("OrderNumber", GetType(String))
            .Add("OrderLine", GetType(String))
            .Add("PackageID", GetType(String))
            .Add("SKU", GetType(String))
            .Add("SerialNumber", GetType(String))
        End With
        '
    End Sub

    Private Sub AddErrorResult(ByVal deoh As DeOrderHeader)
        '--------------------------------------------------------------
        ' AddErrorResult - Write an empty order header result line when
        ' the order has failed
        ' 
        If DtHeader.Columns.Count = 0 Then
            DtHeader_Columns()
            DtDetail_Columns()
            DtText_Columns()
        End If
        If ResultDataSet Is Nothing Then
            ResultDataSet = New DataSet

            ResultDataSet.Tables.Add(DtHeader)
            ResultDataSet.Tables.Add(DtDetail)
            ResultDataSet.Tables.Add(DtText)
        End If

        '--------------------
        ' Add to Header Table
        '--------------------
        Dim dRow As DataRow = Nothing
        dRow = Nothing
        dRow = DtHeader.NewRow()
        dRow("OrderNumber") = String.Empty
        dRow("BranchOrderNumber") = String.Empty
        dRow("CustomerPO") = deoh.CustomerPO
        dRow("ShipToAttention") = String.Empty
        dRow("ShipToAddress1") = String.Empty
        dRow("ShipToAddress2") = String.Empty
        dRow("ShipToAddress3") = String.Empty
        dRow("ShipToCity") = String.Empty
        dRow("ShipToProvince") = String.Empty
        dRow("ShipToSuffix") = String.Empty
        dRow("ShipToPostalCode") = String.Empty
        dRow("CarrierCode") = String.Empty
        dRow("CarrierCodeDescription") = String.Empty
        dRow("FreightRate") = 0
        DtHeader.Rows.Add(dRow)

    End Sub

    Private Property ParmTRAN() As String
        Get
            Return _parmTRAN
        End Get
        Set(ByVal value As String)
            _parmTRAN = value
        End Set
    End Property
    Private Property ParmHEAD(ByVal order As Integer) As String
        Get
            Return _parmHEAD(order)
        End Get
        Set(ByVal value As String)
            _parmHEAD(order) = value
        End Set
    End Property
    Private Property ParmHEAD2(ByVal order As Integer) As Collection
        Get
            Return _parmHEAD2(order)
        End Get
        Set(ByVal value As Collection)
            _parmHEAD2(order) = value
        End Set
    End Property
    Private Property ParmADDR(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmADDR(order, index)
        End Get
        Set(ByVal value As String)
            _parmADDR(order, index) = value
        End Set
    End Property
    Private Property ParmPAY(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmPAY(order, index)
        End Get
        Set(ByVal value As String)
            _parmPAY(order, index) = value
        End Set
    End Property
    Private Property ParmCHGE(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCHGE(order, index)
        End Get
        Set(ByVal value As String)
            _parmCHGE(order, index) = value
        End Set
    End Property
    Private Property ParmITEM(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmITEM(order, index)
        End Get
        Set(ByVal value As String)
            _parmITEM(order, index) = value
        End Set
    End Property
    Private Property ParmITEM2(ByVal order As Integer, ByVal index As Integer) As Collection
        Get
            Return _parmITEM2(order, index)
        End Get
        Set(ByVal value As Collection)
            _parmITEM2(order, index) = value
        End Set
    End Property
    Private Property ParmPAYMENT(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmPAYMENT(order, index)
        End Get
        Set(ByVal value As String)
            _parmPAYMENT(order, index) = value
        End Set
    End Property
    Private Property ParmCOMM(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCOMM(order, index)
        End Get
        Set(ByVal value As String)
            _parmCOMM(order, index) = value
        End Set
    End Property
    Private Property ParmCOMMLineNumber(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCOMMLineNumber(order, index)
        End Get
        Set(ByVal value As String)
            _parmCOMMLineNumber(order, index) = value
        End Set
    End Property
    Private Property ParmORDERREQ() As String
        Get
            Return _parmORDERREQ
        End Get
        Set(ByVal value As String)
            _parmORDERREQ = value
        End Set
    End Property
    Private Property ParmMISC(ByVal order As Integer) As miscItems
        Get
            Return _parmMISC(order)
        End Get
        Set(ByVal value As miscItems)
            _parmMISC(order) = value
        End Set
    End Property

    Private Function BoolToYN(ByVal value As Boolean) As String
        Dim YNString As String = String.Empty
        Select Case value
            Case True
                YNString = "Y"
            Case False
                YNString = "N"
        End Select
        Return YNString
    End Function

End Class
