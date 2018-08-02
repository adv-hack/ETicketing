Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.IO
Imports System.Data
Imports System.Text
Imports System.Xml
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBAmendBasket
    Inherits DBAccess

#Region "Constants"

    Private Const AddTicketingItems As String = "AddTicketingItems"
    Private Const AddTicketingItemsReturnBasket As String = "AddTicketingItemsReturnBasket"
    Private Const AddMultipleTicketingItemsReturnBasket As String = "AddMultipleTicketingItemsReturnBasket"
    Private Const AddTicketingReservedItems As String = "AddTicketingReservedItems"
    Private Const AddTicketingReservedItemsReturnBasket As String = "AddTicketingReservedItemsReturnBasket"
    Private Const AddseasonTicketRenewalsReturnBasket As String = "AddSeasonTicketRenewalsReturnBasket"
    Private Const RemoveTicketingItems As String = "RemoveTicketingItems"
    Private Const RemoveTicketingItemsReturnBasket As String = "RemoveTicketingItemsReturnBasket"
    Private Const RetrieveTicketingItems As String = "RetrieveTicketingItems"
    Private Const AmendTicketingItems As String = "AmendTicketingItems"
    Private Const AmendTicketingItemsReturnBasket As String = "AmendTicketingItemsReturnBasket"
    Private Const RemoveTicketingExpiredBaskets As String = "RemoveTicketingExpiredBaskets"
    Private Const AddPPSItemsToBasket As String = "AddPPSItemsToBasket"
    Private Const AddAutomaticTicketingItemsReturnBasket As String = "AddAutomaticTicketingItemsReturnBasket"
    Private Const ClearAndAddTicketingItemsReturnBasket As String = "ClearAndAddTicketingItemsReturnBasket"
    Private Const VerifyAndUpdateExtOrderNumberToBasket As String = "VerifyAndUpdateExtOrderNumberToBasket"
    Private Const AddRetailToTicketingBasket As String = "AddRetailToTicketingBasket"
    Private Const GetBasketExceptionSeats As String = "GetBasketExceptionSeats"
    Private Const PACKAGEPRODUCT As String = "P"


#End Region

#Region "Private Properties"

    Private _basketFunctions As New DBBasketFunctions
    Private _bulkSalesMode As Boolean = False
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Public Properties"

    Public Property De_AddPPS() As New DEAddPPS
    Public Property Dep() As New DEAmendBasket
    Public Property DeAddTicketingItems() As New DEAddTicketingItems
    Public Property DeAmendTicketingItems() As New DEAmendTicketingItems
    Public Property De() As New DETicketingItemDetails
    Public Property OrphanSeatRemaining() As Boolean = False
    Public Property ListOfDEAddTicketingItems() As List(Of DEAddTicketingItems)
    Public Property AlternativeSeatSelected() As Boolean = False
    Public Property ClearAvailableStandAreaCache() As Boolean = False
    Public Property BasketRequiresRedirectToBookingOrComponentPage() As String = String.Empty
    Public Property WS036RFirstParm() As String = String.Empty
    Public Property DERetailToTicketing() As New DERetailToTicketing
    Public Property BasketHasExceptionSeats() As Boolean = False

#End Region

#Region "SQL2005 Methods"

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        '-------------------------------------------------------------------------------
        Try
            err = Header_Exists()                                       ' Get header Basket_id if it exists
            If Not err.HasError Then
                '-----------------------------------------------------------------------
                If Dep.AddToBasket Then
                    If Dep.BasketId = 0 Then
                        err = Header_Insert()                           ' Create header
                        If Not err.HasError Then err = Header_Exists() ' Get header basket_id it should now exist
                    End If
                    If Dep.BasketId > 0 Then err = Detail_Insert() ' Create details
                End If
                '-----------------------------------------------------------------------
                If Dep.DeleteBasket And Dep.BasketId > 0 Then
                    err = Detail_Delete(1)                              ' Remove all details
                    If Not err.HasError Then err = Header_Delete(2) ' Remove header
                End If
                '-----------------------------------------------------------------------
                If Dep.DeleteFromBasket And Dep.BasketId > 0 Then
                    err = Detail_Delete(2)                              ' Remove specific details
                End If
                '-----------------------------------------------------------------------
                If Dep.DeleteFreeItems And Dep.BasketId > 0 Then
                    err = Detail_Delete(3)                              ' Remove free items
                End If
                '-----------------------------------------------------------------------
                If Dep.ReplaceBasket Then
                    If Dep.BasketId > 0 Then                            ' there is a basket so kill, it
                        err = Detail_Delete(1)                          ' Remove all details
                        If Not err.HasError Then err = Header_Delete(2) ' Remove header
                    End If
                    err = Header_Insert()                               ' Create header
                    If Dep.BasketId > 0 Then err = Detail_Insert() ' Create details
                End If
                '-----------------------------------------------------------------------
                If Dep.DeleteMultipleBaskets Then
                    err = Detail_Delete(4)
                End If
                '-----------------------------------------------------------------------
                '-----------------------------------------------------------------------
                If Dep.AddFreeItems Then
                    If Dep.BasketId = 0 Then
                        err = Header_Insert()                           ' Create header
                        If Not err.HasError Then err = Header_Exists() ' Get header basket_id it should now exist
                    End If
                    If Dep.BasketId > 0 Then err = Detail_Free_Insert() ' Create details
                End If
                '-----------------------------------------------------------------------
            End If
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-21"
                .HasError = True
            End With
        End Try
        '-------------------------------------------------------------------------------
        Return err
    End Function

    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        Dim iCounter As Integer = 0
        '-------------------------------------------------------------------------------
        '   Check valid logon etc.
        '
        If Not Check_Logon Then
            Const strError As String = "Business Unit / Partner / user combination not found"
            With err
                .ErrorMessage = String.Empty
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-11"
                .HasError = True
            End With
        End If
        '-----------------------------------------------------------------------
        '   Check each product exists and has enough stock
        '
        For iCounter = 1 To Dep.CollDEAlerts.Count
            Dim dea As DEAlerts = Dep.CollDEAlerts.Item(iCounter)
            With dea
                .Price = Check_Price(.ProductCode)
                .AvailabilQty = Check_Stock(.ProductCode)
                .Status = "Y"
                If .Price < 0.01 Then
                    err.HasError = True
                    .Description = "Product code not found in price list"
                    .Status = "N"
                End If
                Select Case .AvailabilQty
                    Case Is = -99
                        err.HasError = True
                        .Description = "Product code not found"
                        .Status = "N"
                    Case Is < .Quantity
                        err.HasError = True
                        .Description = "Not enough stock available"
                        .Status = "N"
                End Select
            End With
        Next
        '-------------------------------------------------------------------------------
        Return err
    End Function

    Protected Function Header_Exists(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString1 As String = " SELECT * FROM tbl_basket_header WITH (NOLOCK)   " & _
                                                 " WHERE BUSINESS_UNIT = @Param1    " & _
                                                 " AND   PARTNER   = @Param2        " & _
                                                 " AND   LOGINID   = @Param3        " & _
                                                 " AND   PROCESSED = 0              "
                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 1
                    Const SQLString2 As String = " SELECT * FROM tbl_basket_header WITH (NOLOCK)   " & _
                                                " WHERE BUSINESS_UNIT = @Param1     " & _
                                                " AND   LOGINID   = @Param2         " & _
                                                " AND   PROCESSED = 0               "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 2
                    Const SQLString3 As String = " SELECT * FROM tbl_basket_header WITH (NOLOCK)   " & _
                                                " WHERE BASKET_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString3, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId

            End Select
            dtr = cmdSelect.ExecuteReader()

            If dtr.HasRows Then
                dtr.Read()
                Dep.BasketId = dtr("BASKET_HEADER_ID")
            End If
            '
            dtr.Close()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError8 As String = "Error during Select Basket Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-31"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Header_Insert() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            Const SQLString As String = " INSERT INTO tbl_basket_header(BUSINESS_UNIT, PARTNER, LOGINID, " & _
                                                " CREATED_DATE, LAST_ACCESSED_DATE, PROCESSED) " & _
                                                " VALUES ( @Param1, @Param2, @Param3, @Param4, @Param5, 0 ) "
            '-----------------------------------------------------------------------
            cmdSelect = New SqlCommand(SQLString, conSql2005)
            With cmdSelect
                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                .Parameters.Add(New SqlParameter(Param4, SqlDbType.DateTime)).Value = Now
                .Parameters.Add(New SqlParameter(Param5, SqlDbType.DateTime)).Value = Now
                .ExecuteNonQuery()
            End With
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Insert Basket Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-32"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Header_Update(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            '
            Select Case Action
                Case Is = 0
                    Const SQLString2 As String = " UPDATE tbl_basket_header SET" & _
                                              "   PROCESSED = @Param4 " & _
                                              "  ,LAST_ACCESSED_DATE = @Param5 " & _
                                              " WHERE BASKET_HEADER_ID = @Param6 "

                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Dep.Processed
                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.DateTime)).Value = Now
                        .Parameters.Add(New SqlParameter(Param6, SqlDbType.BigInt)).Value = Dep.BasketId
                        .ExecuteNonQuery()
                    End With

                Case Is = 1
                    Const SQLString1 As String = " UPDATE tbl_basket_header SET" & _
                                          "   BUSINESS_UNIT = @Param1 " & _
                                          "  ,PARTNER = @Param2 " & _
                                          "  ,LOGINID = @Param3 " & _
                                          "  ,PROCESSED = @Param4 " & _
                                          "  ,LAST_ACCESSED_DATE = @Param5 " & _
                                          " WHERE BASKET_HEADER_ID = @Param6 "

                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Dep.Processed
                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.DateTime)).Value = Now
                        .Parameters.Add(New SqlParameter(Param6, SqlDbType.BigInt)).Value = Dep.BasketId
                        .ExecuteNonQuery()
                    End With
            End Select
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Update Basket Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-33"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Header_Delete(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString1 As String = " DELETE FROM tbl_basket_header    " & _
                                                 " WHERE BUSINESS_UNIT = @Param1    " & _
                                                 " AND   PARTNER   = @Param2        " & _
                                                 " AND   LOGINID   = @Param3        " & _
                                                 " AND   PROCESSED = 0              "
                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 1
                    Const SQLString2 As String = " DELETE FROM tbl_basket_header    " & _
                                                " WHERE BUSINESS_UNIT = @Param1     " & _
                                                " AND   LOGINID   = @Param2         " & _
                                                " AND   PROCESSED = 0               "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.UserID
                    End With

                Case Is = 2
                    Const SQLString3 As String = " DELETE FROM tbl_basket_header    " & _
                                                " WHERE BASKET_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString3, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId

            End Select
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing
        Catch ex As Exception
            Const strError8 As String = "Error during Select Basket Header"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-34"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Detail_Select(ByVal id As Long, Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim dtr As SqlDataReader = Nothing
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString0 As String = " SELECT * FROM tbl_basket_detail WITH (NOLOCK)  WHERE BASKET_HEADER_ID= @Param1   "
                    cmdSelect = New SqlCommand(SQLString0, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = id
                        dtr = .ExecuteReader
                    End With

                Case Is = 1
                    Const SQLString1 As String = " SELECT * FROM tbl_basket_detail WITH (NOLOCK)  WHERE BASKET_DETAIL_ID= @Param1   "
                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = id
                        dtr = .ExecuteReader
                    End With

                Case Is = 2
                    Const SQLString2 As String = _
                        " SELECT COUNT(PR.PRODUCT_CODE) AS NO_OF_ITEMS, " & _
                        "        SUM(PL.GROSS_PRICE * BD.QUANTITY) AS TOTAL_VALUE_GROSS, " & _
                        "        SUM(PL.NET_PRICE * BD.QUANTITY) AS TOTAL_VALUE_NET" & _
                        " FROM tbl_basket_detail BD WITH (NOLOCK)  " & _
                        "   INNER Join tbl_product PR WITH (NOLOCK)  " & _
                        "       ON BD.PRODUCT = PR.PRODUCT_CODE " & _
                        "   INNER Join tbl_price_list_detail PL WITH (NOLOCK)  " & _
                        "       ON PR.PRODUCT_CODE = PL.PRODUCT" & _
                        " WHERE  PRICE_LIST = " & _
                                " (SELECT VALUE FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)   " & _
                                "   WHERE  BUSINESS_UNIT = @Param1 AND PARTNER = @Param2 " & _
                                "   AND DEFAULT_NAME = 'PRICE_LIST') " & _
                        " AND BD.BASKET_HEADER_ID = @Param3  "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.BasketId
                        dtr = .ExecuteReader
                    End With

                Case Is = 3
                    Const SQLString2 As String = _
                        " SELECT PR.PRODUCT_CODE, PR.PRODUCT_DESCRIPTION_1 AS PRODUCT_TITLE, PL.PRICE_LIST, " & _
                        "        PL.FROM_DATE AS PRICE_LIST_FROM_DATE, PL.TO_DATE AS PRICE_LIST_TO_DATE," & _
                        "        PL.NET_PRICE AS PRICE_NET, PL.GROSS_PRICE AS PRICE_GROSS, BD.BASKET_HEADER_ID, " & _
                        "        BD.QUANTITY AS QTY, PL.GROSS_PRICE * BD.QUANTITY AS VALUE_GROSS, " & _
                        "        PL.NET_PRICE * BD.QUANTITY AS VALUE_NET  " & _
                        " FROM tbl_basket_detail BD WITH (NOLOCK)  " & _
                        "   INNER Join tbl_product PR WITH (NOLOCK)  " & _
                        "       ON BD.PRODUCT = PR.PRODUCT_CODE " & _
                        "   INNER Join tbl_price_list_detail PL WITH (NOLOCK)  " & _
                        "       ON PR.PRODUCT_CODE = PL.PRODUCT" & _
                        " WHERE  PRICE_LIST = " & _
                                " (SELECT VALUE FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)   " & _
                                "   WHERE  BUSINESS_UNIT = @Param1 AND PARTNER = @Param2 " & _
                                "   AND DEFAULT_NAME = 'PRICE_LIST') " & _
                        " AND BD.BASKET_HEADER_ID = @Param3 "

                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.BasketId
                        dtr = .ExecuteReader
                    End With
            End Select
            If dtr.HasRows Then
                '   Load the Datareader into what ever to return

            End If
            dtr.Close()
            cmdSelect = Nothing
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Select Basket Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-41"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Detail_Insert() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing

            Dim SQLString As New StringBuilder

            SQLString.Append("IF EXISTS ")
            SQLString.Append(" (SELECT * FROM tbl_basket_detail WITH (NOLOCK) WHERE BASKET_HEADER_ID = @Param0 AND PRODUCT = @Param9 AND IS_FREE = @IS_FREE)")
            SQLString.Append(" UPDATE tbl_basket_detail")
            SQLString.Append(" SET QUANTITY = QUANTITY + @Param1, GROSS_PRICE = @Param2, PRODUCT_DESCRIPTION1 = @PRODUCT_DESCRIPTION1, PRODUCT_DESCRIPTION2 = @PRODUCT_DESCRIPTION2, PRODUCT_DESCRIPTION3 = @PRODUCT_DESCRIPTION3, PRODUCT_DESCRIPTION4 = @PRODUCT_DESCRIPTION4, PRODUCT_DESCRIPTION5 = @PRODUCT_DESCRIPTION5,")
            SQLString.Append(" GROUP_LEVEL_01 = @GROUP_LEVEL_01, GROUP_LEVEL_02 = @GROUP_LEVEL_02, GROUP_LEVEL_03 = @GROUP_LEVEL_03, GROUP_LEVEL_04 = @GROUP_LEVEL_04, GROUP_LEVEL_05 = @GROUP_LEVEL_05, GROUP_LEVEL_06 = @GROUP_LEVEL_06, GROUP_LEVEL_07 = @GROUP_LEVEL_07, GROUP_LEVEL_08 = @GROUP_LEVEL_08, GROUP_LEVEL_09 = @GROUP_LEVEL_09, GROUP_LEVEL_10 = @GROUP_LEVEL_10,")
            SQLString.Append(" SIZE = @SIZE, QUANTITY_AVAILABLE = @QUANTITY_AVAILABLE, NET_PRICE = @NET_PRICE, TAX_PRICE = @TAX_PRICE, MODULE = ''")
            SQLString.Append(" WHERE BASKET_HEADER_ID = @Param3 AND PRODUCT = @Param4 AND IS_FREE = @IS_FREE ")
            SQLString.Append("ELSE ")
            SQLString.Append(" INSERT INTO tbl_basket_detail ")
            SQLString.Append(" (BASKET_HEADER_ID, PRODUCT, QUANTITY, GROSS_PRICE, IS_FREE, MASTER_PRODUCT, STOCK_ERROR_CODE, ALTERNATE_SKU, XML_CONFIG, COST_CENTRE, ACCOUNT_CODE, PRODUCT_SUB_TYPE, PRODUCT_DESCRIPTION1, PRODUCT_DESCRIPTION2, PRODUCT_DESCRIPTION3, PRODUCT_DESCRIPTION4, PRODUCT_DESCRIPTION5,")
            SQLString.Append(" GROUP_LEVEL_01, GROUP_LEVEL_02, GROUP_LEVEL_03, GROUP_LEVEL_04, GROUP_LEVEL_05, GROUP_LEVEL_06, GROUP_LEVEL_07, GROUP_LEVEL_08, GROUP_LEVEL_09, GROUP_LEVEL_10,")
            SQLString.Append(" SIZE, QUANTITY_AVAILABLE, NET_PRICE, TAX_PRICE, MODULE )")
            SQLString.Append(" VALUES ( @Param5, @Param6, @Param7, @Param8, @IS_FREE, @MASTER_PRODUCT, '', @ALTERNATE_SKU, '', @COST_CENTRE, @ACCOUNT_CODE, @PRODUCT_SUB_TYPE, @PRODUCT_DESCRIPTION1, @PRODUCT_DESCRIPTION2, @PRODUCT_DESCRIPTION3, @PRODUCT_DESCRIPTION4, @PRODUCT_DESCRIPTION5,")
            SQLString.Append(" @GROUP_LEVEL_01, @GROUP_LEVEL_02, @GROUP_LEVEL_03, @GROUP_LEVEL_04, @GROUP_LEVEL_05, @GROUP_LEVEL_06, @GROUP_LEVEL_07, @GROUP_LEVEL_08, @GROUP_LEVEL_09, @GROUP_LEVEL_10,")
            SQLString.Append(" @SIZE, @QUANTITY_AVAILABLE, @NET_PRICE, @TAX_PRICE, '')")

            '-----------------------------------------------------------------------
            '   Check each product exists and has enough stock
            '
            For iCounter = 1 To Dep.CollDEAlerts.Count
                dea = Dep.CollDEAlerts.Item(iCounter)
                cmdSelect = New SqlCommand(SQLString.ToString(), conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param0, SqlDbType.BigInt)).Value = Dep.BasketId
                    .Parameters.Add(New SqlParameter(Param9, SqlDbType.Char)).Value = dea.ProductCode
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Decimal)).Value = dea.Quantity
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Decimal)).Value = dea.Price
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.BasketId
                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = dea.ProductCode
                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.BigInt)).Value = Dep.BasketId
                    .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = dea.ProductCode
                    .Parameters.Add(New SqlParameter(Param7, SqlDbType.Decimal)).Value = dea.Quantity
                    .Parameters.Add(New SqlParameter(Param8, SqlDbType.Decimal)).Value = dea.Price
                    .Parameters.Add(New SqlParameter("@IS_FREE", SqlDbType.Decimal)).Value = Dep.IsFreeItem
                    Dim masterProduct As String = String.Empty
                    If Not dea.MasterProduct Is Nothing Then
                        masterProduct = dea.MasterProduct
                    End If
                    .Parameters.Add(New SqlParameter("@MASTER_PRODUCT", SqlDbType.Char)).Value = masterProduct
                    Dim alternateSKU As String = String.Empty
                    If Not dea.AlternateSKU Is Nothing Then
                        alternateSKU = dea.AlternateSKU
                    End If
                    .Parameters.Add(New SqlParameter("@ALTERNATE_SKU", SqlDbType.Char)).Value = alternateSKU
                    Dim costCentre As String = String.Empty
                    If Not dea.CostCentre Is Nothing Then
                        costCentre = dea.CostCentre
                    End If
                    .Parameters.Add(New SqlParameter("@COST_CENTRE", SqlDbType.Char)).Value = costCentre

                    Dim accountCode As String = String.Empty
                    If Not dea.AccountCode Is Nothing Then
                        accountCode = dea.AccountCode
                    End If
                    .Parameters.Add(New SqlParameter("@ACCOUNT_CODE", SqlDbType.Char)).Value = accountCode
                    Dim productSubType As String = String.Empty
                    If dea.ProductSubType IsNot Nothing Then productSubType = dea.ProductSubType
                    .Parameters.Add(New SqlParameter("@PRODUCT_SUB_TYPE", SqlDbType.Char)).Value = productSubType
                    .Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION1", SqlDbType.NVarChar)).Value = dea.ProductDescription1
                    .Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION2", SqlDbType.NVarChar)).Value = dea.ProductDescription2
                    .Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION3", SqlDbType.NVarChar)).Value = dea.ProductDescription3
                    .Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION4", SqlDbType.NVarChar)).Value = dea.ProductDescription4
                    .Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION5", SqlDbType.NVarChar)).Value = dea.ProductDescription5
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_01", SqlDbType.NVarChar)).Value = dea.GroupL01Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_02", SqlDbType.NVarChar)).Value = dea.GroupL02Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_03", SqlDbType.NVarChar)).Value = dea.GroupL03Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_04", SqlDbType.NVarChar)).Value = dea.GroupL04Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_05", SqlDbType.NVarChar)).Value = dea.GroupL05Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_06", SqlDbType.NVarChar)).Value = dea.GroupL06Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_07", SqlDbType.NVarChar)).Value = dea.GroupL07Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_08", SqlDbType.NVarChar)).Value = dea.GroupL08Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_09", SqlDbType.NVarChar)).Value = dea.GroupL09Group
                    .Parameters.Add(New SqlParameter("@GROUP_LEVEL_10", SqlDbType.NVarChar)).Value = dea.GroupL10Group
                    .Parameters.Add(New SqlParameter("@SIZE", SqlDbType.NVarChar)).Value = dea.Size
                    .Parameters.Add(New SqlParameter("@QUANTITY_AVAILABLE", SqlDbType.Decimal)).Value = dea.QuantityAvailable
                    .Parameters.Add(New SqlParameter("@NET_PRICE", SqlDbType.Decimal)).Value = dea.NetPrice
                    .Parameters.Add(New SqlParameter("@TAX_PRICE", SqlDbType.Decimal)).Value = dea.TaxPrice
                    .ExecuteNonQuery()
                End With
            Next
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Insert Basket Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-42"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Detail_Free_Insert() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing
            Dim validMasterProducts As String = String.Empty

            Dim SQLString As New StringBuilder
            SQLString.Append(" INSERT INTO tbl_basket_detail (BASKET_HEADER_ID, PRODUCT, QUANTITY, GROSS_PRICE, MODULE, IS_FREE, ")
            SQLString.Append(" GROUP_LEVEL_01, GROUP_LEVEL_02, GROUP_LEVEL_03, GROUP_LEVEL_04, GROUP_LEVEL_05,")
            SQLString.Append(" GROUP_LEVEL_06, GROUP_LEVEL_07, GROUP_LEVEL_08, GROUP_LEVEL_09, GROUP_LEVEL_10,")
            SQLString.Append(" MASTER_PRODUCT, STOCK_ERROR_CODE, ALTERNATE_SKU, XML_CONFIG, COST_CENTRE, ACCOUNT_CODE, ALLOW_SELECT_OPTION ) ")
            SQLString.Append(" VALUES ( @BASKET_HEADER_ID, @PRODUCT, @QUANTITY, @GROSS_PRICE, '', @IS_FREE, ")
            SQLString.Append(" @GROUP_LEVEL_01, @GROUP_LEVEL_02, @GROUP_LEVEL_03, @GROUP_LEVEL_04, @GROUP_LEVEL_05, ")
            SQLString.Append(" @GROUP_LEVEL_06, @GROUP_LEVEL_07, @GROUP_LEVEL_08, @GROUP_LEVEL_09, @GROUP_LEVEL_10, ")
            SQLString.Append(" @MASTER_PRODUCT, '', @ALTERNATE_SKU, '', @COST_CENTRE, @ACCOUNT_CODE, @ALLOW_SELECT_OPTION ) ")

            '-----------------------------------------------------------------------
            '   Check each product exists and has enough stock
            '
            For iCounter = 1 To Dep.CollDEAlerts.Count
                dea = Dep.CollDEAlerts.Item(iCounter)
                If validMasterProducts.Length > 0 Then
                    validMasterProducts = validMasterProducts & ",'" & dea.MasterProduct & "'"
                Else
                    validMasterProducts = "'" & dea.MasterProduct & "'"
                End If
                Dim dtBalanceFreeItems As DataTable = GetBalanceFreeItems(Dep.BasketId, Dep.IsFreeItem, dea.Quantity, dea.ProductCode, dea.MasterProduct, dea.AllowSelectOption)
                Dim freeItemBalance As Integer = 0
                If dtBalanceFreeItems.Rows.Count > 0 Then
                    freeItemBalance = Utilities.CheckForDBNull_Int(dtBalanceFreeItems.Rows(0)(0))
                End If
                If freeItemBalance > 0 Then
                    For quantityCounter As Integer = 1 To freeItemBalance
                        cmdSelect = New SqlCommand(SQLString.ToString(), conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter("@BASKET_HEADER_ID", SqlDbType.BigInt)).Value = Dep.BasketId
                            .Parameters.Add(New SqlParameter("@PRODUCT", SqlDbType.NVarChar)).Value = dea.ProductCode
                            .Parameters.Add(New SqlParameter("@QUANTITY", SqlDbType.Decimal)).Value = 1
                            .Parameters.Add(New SqlParameter("@GROSS_PRICE", SqlDbType.Decimal)).Value = dea.Price
                            .Parameters.Add(New SqlParameter("@IS_FREE", SqlDbType.Decimal)).Value = Dep.IsFreeItem
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_01", SqlDbType.NVarChar)).Value = dea.GroupL01Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_02", SqlDbType.NVarChar)).Value = dea.GroupL02Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_03", SqlDbType.NVarChar)).Value = dea.GroupL03Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_04", SqlDbType.NVarChar)).Value = dea.GroupL04Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_05", SqlDbType.NVarChar)).Value = dea.GroupL05Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_06", SqlDbType.NVarChar)).Value = dea.GroupL06Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_07", SqlDbType.NVarChar)).Value = dea.GroupL07Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_08", SqlDbType.NVarChar)).Value = dea.GroupL08Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_09", SqlDbType.NVarChar)).Value = dea.GroupL09Group
                            .Parameters.Add(New SqlParameter("@GROUP_LEVEL_10", SqlDbType.NVarChar)).Value = dea.GroupL10Group

                            Dim masterProduct As String = String.Empty
                            If Not dea.MasterProduct Is Nothing Then
                                masterProduct = dea.MasterProduct
                            End If
                            .Parameters.Add(New SqlParameter("@MASTER_PRODUCT", SqlDbType.NVarChar)).Value = masterProduct
                            Dim alternateSKU As String = String.Empty
                            If Not dea.AlternateSKU Is Nothing Then
                                alternateSKU = dea.AlternateSKU
                            End If
                            .Parameters.Add(New SqlParameter("@ALTERNATE_SKU", SqlDbType.Char)).Value = alternateSKU
                            Dim costCentre As String = String.Empty
                            If Not dea.CostCentre Is Nothing Then
                                costCentre = dea.CostCentre
                            End If
                            .Parameters.Add(New SqlParameter("@COST_CENTRE", SqlDbType.Char)).Value = costCentre

                            Dim accountCode As String = String.Empty
                            If Not dea.AccountCode Is Nothing Then
                                accountCode = dea.AccountCode
                            End If
                            .Parameters.Add(New SqlParameter("@ACCOUNT_CODE", SqlDbType.Char)).Value = accountCode
                            .Parameters.Add(New SqlParameter("@ALLOW_SELECT_OPTION", SqlDbType.Bit)).Value = dea.AllowSelectOption

                            .ExecuteNonQuery()
                        End With
                    Next
                End If
            Next

            'Delete the free items which are exists in basket but currently not in promotions
            DeleteInValidFreeItemsInBasket(Dep.BasketId, Dep.IsFreeItem, validMasterProducts)

        Catch ex As Exception
            Const strError8 As String = "Error during Insert Basket Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-42"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Detail_Update(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                Case Is = 0
                    Const SQLString1 As String = " UPDATE tbl_basket_detail SET " & _
                                                "    BASKET_HEADER_ID = @Param1 " & _
                                                "   ,PRODUCT = @Param2 " & _
                                                "   ,QUANTITY = @Param3 " & _
                                                "   ,GROSS_PRICE = @Param4  " & _
                                                "   ,IS_FREE = @IS_FREE  " & _
                                                " WHERE BASKET_DETAIL_ID = @Param5 "
                    '----------------------------------------------------------------------
                    For iCounter = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iCounter)
                        cmdSelect = New SqlCommand(SQLString1, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BasketId
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = dea.ProductCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = dea.Quantity
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Decimal)).Value = dea.Price
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.BigInt)).Value = dea.Id
                            .Parameters.Add(New SqlParameter("@IS_FREE", SqlDbType.Decimal)).Value = Dep.IsFreeItem
                            .ExecuteNonQuery()
                        End With
                    Next
                    '--------------------------------------------------------------------------
                Case Is = 1
                    Const SQLString2 As String = " UPDATE tbl_basket_detail SET " & _
                                                "    QUANTITY = @Param1 " & _
                                                "   ,GROSS_PRICE = @Param2 " & _
                                                " WHERE BASKET_HEADER_ID = @Param3 " & _
                                                " AND   PRODUCT = @Param42 " & _
                                                " AND   IS_FREE = @IS_FREE "
                    '-----------------------------------------------------------------------
                    For iCounter = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iCounter)
                        cmdSelect = New SqlCommand(SQLString2, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = dea.Quantity
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Decimal)).Value = dea.Price
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = Dep.BasketId
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = dea.ProductCode
                            .Parameters.Add(New SqlParameter("@IS_FREE", SqlDbType.Decimal)).Value = Dep.IsFreeItem
                            .ExecuteNonQuery()
                        End With
                    Next
                    '--------------------------------------------------------------------------
            End Select
            cmdSelect.ExecuteNonQuery()
            cmdSelect = Nothing

        Catch ex As Exception
            Const strError8 As String = "Error during Update Basket Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-43"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function Detail_Delete(Optional ByVal Action As Integer = 0) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim cmdSelect As SqlCommand = Nothing
            Select Case Action
                ''Case Is = 0
                ''    Const SQLString1 As String = " DELETE FROM tbl_basket_detail   " & _
                ''                                 " WHERE BASKET_DETAIL_ID = @Param1  "
                ''    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                ''    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = id
                ''    cmdSelect.ExecuteNonQuery()

                Case Is = 1
                    Const SQLString2 As String = " DELETE FROM tbl_basket_detail    " & _
                                                 " WHERE BASKET_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId
                    cmdSelect.ExecuteNonQuery()
                    cmdSelect = Nothing
                    Const SQLString3 As String = " DELETE FROM tbl_basket_fees    " & _
                                                 " WHERE BASKET_HEADER_ID = @Param1  "
                    cmdSelect = New SqlCommand(SQLString3, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId
                    cmdSelect.ExecuteNonQuery()

                Case Is = 2
                    Const SQLString2 As String = " DELETE FROM tbl_basket_detail    " & _
                                                 " WHERE BASKET_HEADER_ID = @Param1  " & _
                                                 " AND PRODUCT = @Param2 " & _
                                                 " AND IS_FREE = @IS_FREE  "
                    '-----------------------------------------------------------------------
                    For iCounter = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iCounter)
                        cmdSelect = New SqlCommand(SQLString2, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = dea.ProductCode
                            .Parameters.Add(New SqlParameter("@IS_FREE", SqlDbType.Decimal)).Value = Dep.IsFreeItem
                            .ExecuteNonQuery()
                        End With
                    Next

                Case Is = 3
                    Const SQLString2 As String = " DELETE FROM tbl_basket_detail    " & _
                                                 " WHERE BASKET_HEADER_ID = @Param1  " & _
                                                 " AND IS_FREE = @IS_FREE  "
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId
                    cmdSelect.Parameters.Add(New SqlParameter("@IS_FREE", SqlDbType.Decimal)).Value = Dep.IsFreeItem
                    cmdSelect.ExecuteNonQuery()

                Case Is = 4
                    'Get the basket ids as comma spearated string
                    Dim basketIDs As String = String.Empty
                    Dim intLoopCount As Integer = 0
                    Do While intLoopCount < Dep.BasketIdList.Count
                        If intLoopCount <> 0 Then
                            basketIDs = basketIDs + ", "
                        End If
                        basketIDs = basketIDs + Dep.BasketIdList.Item(intLoopCount).ToString
                        intLoopCount = intLoopCount + 1
                    Loop
                    'Remove the basket detail
                    Dim SQLString2 As String = " DELETE FROM tbl_basket_detail WHERE BASKET_HEADER_ID IN (" & basketIDs & ")"
                    If Dep.DeleteModule.Trim <> "" Then
                        SQLString2 = SQLString2 + " AND MODULE = '" + Dep.DeleteModule.Trim + "'"
                    End If
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    cmdSelect.ExecuteNonQuery()
                    'Remove the basket fees
                    cmdSelect = Nothing
                    SQLString2 = " DELETE FROM tbl_basket_fees WHERE BASKET_HEADER_ID IN (" & basketIDs & ")"
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.BigInt)).Value = Dep.BasketId
                    cmdSelect.ExecuteNonQuery()

            End Select
            cmdSelect = Nothing
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during Delete Basket Detail"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-44"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Function IsGUID(ByVal str As String) As Boolean
        Try
            Dim re As New System.Text.RegularExpressions.Regex("^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", Text.RegularExpressions.RegexOptions.IgnoreCase)
            Return re.IsMatch(str)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function GetBalanceFreeItems(ByVal basketHeaderID As Long, ByVal isFree As Boolean,
                                         ByVal quantityAllowed As Double, ByVal productCode As String, ByVal masterProduct As String,
                                         ByVal allowSelectOption As Boolean) As DataTable
        Dim dtBalanceFreeItems As New DataTable

        Const sqlStatement As String = " DECLARE @COUNTEXISTS AS INTEGER " & vbCrLf & _
                        " DECLARE @COUNTREMAINING AS INTEGER " & vbCrLf & _
                        " SET NOCOUNT ON " & vbCrLf & _
                        " DELETE TBL_BASKET_DETAIL WHERE BASKET_HEADER_ID=@BASKET_HEADER_ID AND IS_FREE=@IS_FREE AND ALLOW_SELECT_OPTION<>@ALLOW_SELECT_OPTION AND (PRODUCT = @PRODUCT_CODE OR MASTER_PRODUCT=@MASTER_PRODUCT) " & vbCrLf & _
                        " SELECT @COUNTEXISTS = COUNT(*) FROM TBL_BASKET_DETAIL WHERE BASKET_HEADER_ID=@BASKET_HEADER_ID AND IS_FREE=@IS_FREE AND ALLOW_SELECT_OPTION=@ALLOW_SELECT_OPTION AND (PRODUCT = @PRODUCT_CODE OR MASTER_PRODUCT=@MASTER_PRODUCT) " & vbCrLf & _
                        " SET @COUNTREMAINING = @QUANTITYALLOWED - @COUNTEXISTS " & vbCrLf & _
                        " IF @COUNTREMAINING<0  " & vbCrLf & _
                        " BEGIN " & vbCrLf & _
                        " DELETE TBL_BASKET_DETAIL WHERE BASKET_DETAIL_ID IN (SELECT TOP (-(@COUNTREMAINING)) BASKET_DETAIL_ID FROM TBL_BASKET_DETAIL  " & vbCrLf & _
                        " WHERE BASKET_HEADER_ID=@BASKET_HEADER_ID  " & vbCrLf & _
                        " AND IS_FREE=@IS_FREE AND ALLOW_SELECT_OPTION=@ALLOW_SELECT_OPTION " & vbCrLf & _
                        " AND (PRODUCT = @PRODUCT_CODE OR MASTER_PRODUCT=@MASTER_PRODUCT)  " & vbCrLf & _
                        " ORDER BY OPTION_SELECTED, BASKET_DETAIL_ID DESC)  " & vbCrLf & _
                        " End " & vbCrLf & _
                        " SET NOCOUNT OFF " & vbCrLf & _
                        " SELECT @COUNTREMAINING 'BalanceFreeItems' "

        Dim cmdBalance As New SqlClient.SqlCommand(sqlStatement, conSql2005)
        With cmdBalance.Parameters
            .Clear()
            .Add(New SqlParameter("@BASKET_HEADER_ID", SqlDbType.BigInt)).Value = basketHeaderID
            .Add(New SqlParameter("@IS_FREE", SqlDbType.NVarChar)).Value = isFree
            .Add(New SqlParameter("@QUANTITYALLOWED", SqlDbType.NVarChar)).Value = quantityAllowed
            .Add(New SqlParameter("@PRODUCT_CODE", SqlDbType.NVarChar)).Value = productCode
            .Add(New SqlParameter("@MASTER_PRODUCT", SqlDbType.NVarChar)).Value = masterProduct
            .Add(New SqlParameter("@ALLOW_SELECT_OPTION", SqlDbType.Bit)).Value = allowSelectOption
        End With

        Dim dta As New SqlDataAdapter(cmdBalance)
        dta.Fill(dtBalanceFreeItems)
        dta.Dispose()
        cmdBalance.Dispose()
        Return dtBalanceFreeItems
    End Function

    Private Sub DeleteInValidFreeItemsInBasket(ByVal basketHeaderID As Long, ByVal isFree As Boolean,
                                         ByVal validFreeItemMasterProducts As String)

        Dim sqlStatement As String = " DELETE TBL_BASKET_DETAIL" & _
                        " WHERE BASKET_HEADER_ID=@BASKET_HEADER_ID AND IS_FREE=@IS_FREE" & _
                        " AND MASTER_PRODUCT NOT IN ({0}) "

        sqlStatement = String.Format(sqlStatement, validFreeItemMasterProducts)

        Dim cmdDelete As New SqlClient.SqlCommand(sqlStatement, conSql2005)
        With cmdDelete.Parameters
            .Clear()
            .Add(New SqlParameter("@BASKET_HEADER_ID", SqlDbType.BigInt)).Value = basketHeaderID
            .Add(New SqlParameter("@IS_FREE", SqlDbType.Bit)).Value = isFree
        End With
        cmdDelete.ExecuteNonQuery()
        cmdDelete.Dispose()
    End Sub

    Protected ReadOnly Property Check_Logon() As Boolean
        Get
            Dim LogonOk As Boolean = False
            '--------------------------------------------------------------------
            If IsGUID(Dep.UserID) Then
                Return Not LogonOk
            Else
                Try
                    Dim dtr As SqlDataReader = Nothing
                    Dim cmdSelect As SqlCommand = Nothing
                    Const SQLString As String = " SELECT * FROM tbl_authorized_users WITH (NOLOCK)     " & _
                                                     " WHERE BUSINESS_UNIT = @Param1    " & _
                                                     " AND   PARTNER   = @Param2        " & _
                                                     " AND   LOGINID   = @Param3        "
                    cmdSelect = New SqlCommand(SQLString, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Dep.UserID
                        dtr = .ExecuteReader
                    End With
                    If dtr.HasRows Then LogonOk = True
                    dtr.Close()
                    cmdSelect = Nothing
                    '--------------------------------------------------------------------------
                Catch ex As Exception

                End Try
            End If
            '--------------------------------------------------------------------
            Return LogonOk
        End Get
    End Property

    Protected ReadOnly Property Check_Price(ByVal ProductCode As String) As Double
        Get
            Dim dPrice As Double = 0
            '--------------------------------------------------------------------
            Try
                Dim dtr As SqlDataReader = Nothing
                Dim cmdSelect As SqlCommand = Nothing

                Const SQLString2 As String = _
                            " SELECT CASE WHEN SALE_GROSS_PRICE <> 0 THEN SALE_GROSS_PRICE ELSE GROSS_PRICE END AS PRICE " & _
                            " FROM tbl_price_list_detail WITH (NOLOCK)  WHERE  PRICE_LIST = " & _
                                    " (SELECT VALUE FROM tbl_ecommerce_module_defaults_bu WITH (NOLOCK)   " & _
                                "   WHERE  BUSINESS_UNIT = @Param1 AND PARTNER = @Param2 " & _
                                "   AND DEFAULT_NAME = 'PRICE_LIST') " & _
                            " AND PRODUCT = @Param3 "

                cmdSelect = New SqlCommand(SQLString2, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Dep.PartnerCode
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = ProductCode
                    dtr = .ExecuteReader
                End With
                If dtr.HasRows Then
                    dtr.Read()
                    dPrice = dtr("PRICE")
                Else
                    cmdSelect = New SqlCommand(SQLString2, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Dep.BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = ProductCode
                        dtr = .ExecuteReader
                    End With
                    If dtr.HasRows Then
                        dtr.Read()
                        dPrice = dtr("PRICE")
                    Else
                        cmdSelect = New SqlCommand(SQLString2, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = ProductCode
                            dtr = .ExecuteReader
                        End With
                        If dtr.HasRows Then
                            dtr.Read()
                            dPrice = dtr("PRICE")
                        End If
                    End If
                End If
                dtr.Close()
                cmdSelect = Nothing
                '--------------------------------------------------------------------------
            Catch ex As Exception

            End Try
            '--------------------------------------------------------------------
            Return dPrice
        End Get
    End Property

    Protected ReadOnly Property Check_Stock(ByVal ProductCode As String) As Double
        Get
            Dim Available As Double = 0
            '--------------------------------------------------------------------
            Try
                Dim dtr As SqlDataReader = Nothing
                Dim cmdSelect As SqlCommand = Nothing
                Const SQLString As String = " SELECT AVAILABLE_QUANTITY FROM tbl_product_stock WITH (NOLOCK)   WHERE PRODUCT = @PARAM1"
                cmdSelect = New SqlCommand(SQLString, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = ProductCode
                    dtr = .ExecuteReader
                End With
                If dtr.HasRows Then
                    dtr.Read()
                    Available = dtr("AVAILABLE_QUANTITY")
                Else
                    Available = -99
                End If
                dtr.Close()
                cmdSelect = Nothing
                '--------------------------------------------------------------------------
            Catch ex As Exception

            End Try
            '--------------------------------------------------------------------
            Return Available
        End Get
    End Property

    Protected Overrides Function AccessDataBaseDev() As ErrorObj
        Dim err As New ErrorObj

        'Construct the dummy data and insert into the correct dataentities here
        Select Case _settings.ModuleName
            Case Is = AddTicketingItemsReturnBasket
                'Create the Status data table
                ResultDataSet = New DataSet

                'Create the basket data tables
                _basketFunctions.CreateBasketTables(ResultDataSet)

                BasketDevData.AddDevDataToBasketHeaderTable(ResultDataSet.Tables("BasketHeader"))
                BasketDevData.AddDevDataToBasketDetailsTable(ResultDataSet.Tables("BasketDetail"))

            Case Else
                ' Throw an error?  We have not yet set up dunmmy data for the requested module.
                Dim errorString As String = "There has not yet been any dummy data set up for the requested module: " & _settings.ModuleName
                Const errorStatus As String = "No Dummy data set up for module"
                With err
                    .ErrorMessage = errorString
                    .ErrorStatus = errorStatus
                    .ErrorNumber = "TACDABAS-DEV-1"
                    .HasError = True
                End With
        End Select
        Return err
    End Function

#End Region

#Region "TALENTTKT Methods"

#Region "AccessDataBaseTALENTTKT"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Try

            'Set up the basket functions object
            _basketFunctions.conTALENTTKT = conTALENTTKT
            _basketFunctions.StoredProcedureGroup = Settings.StoredProcedureGroup
            _basketFunctions.OriginatingSource = Settings.OriginatingSource
            _basketFunctions.CustomerNo = Settings.LoginId
            If Settings.AgentEntity IsNot Nothing Then
                _bulkSalesMode = Settings.AgentEntity.BulkSalesMode
                _basketFunctions.BulkSalesMode = _bulkSalesMode
            End If

            Select Case _settings.ModuleName
                Case Is = AddPPSItemsToBasket
                    _basketFunctions.CustomerNo = De_AddPPS.CustomerNumber
                    _basketFunctions.SessionId = De_AddPPS.SessionId
                    _basketFunctions.Source = De_AddPPS.Source
                Case Is = RemoveTicketingItemsReturnBasket, RemoveTicketingItems, RetrieveTicketingItems, AddMultipleTicketingItemsReturnBasket
                    _basketFunctions.CustomerNo = De.CustomerNo
                    _basketFunctions.SessionId = De.SessionId
                    _basketFunctions.Source = De.Src
                Case Is = AmendTicketingItemsReturnBasket, AmendTicketingItems
                    _basketFunctions.SessionId = DeAmendTicketingItems.SessionID
                    _basketFunctions.CustomerNo = DeAmendTicketingItems.CustomerNo
                    _basketFunctions.Source = DeAmendTicketingItems.Src
                Case Is = RemoveTicketingExpiredBaskets
                Case Is = VerifyAndUpdateExtOrderNumberToBasket
                Case Is = AddRetailToTicketingBasket
                Case Is = GetBasketExceptionSeats
                Case Is = AddTicketingReservedItemsReturnBasket
                    _basketFunctions.SessionId = DeAddTicketingItems.SessionId
                    _basketFunctions.Source = DeAddTicketingItems.Source
                Case Else
                    _basketFunctions.CustomerNo = DeAddTicketingItems.CustomerNumber
                    _basketFunctions.SessionId = DeAddTicketingItems.SessionId
                    _basketFunctions.Source = DeAddTicketingItems.Source
            End Select

            'Brach to the required functions
            Select Case _settings.ModuleName
                Case Is = AddTicketingItems : err = AccessDatabaseWS501R()
                Case Is = AddTicketingReservedItems : err = AccessDatabaseWS218R()
                Case Is = AddMultipleTicketingItemsReturnBasket : err = AccessDatabaseWS621R()
                Case Is = RemoveTicketingItems : err = AccessDatabaseWS602R(False)
                Case Is = AmendTicketingItems : err = AccessDatabaseWS038R()
                Case Is = RetrieveTicketingItems : err = AccessDatabaseWS036R()
                Case Is = AddTicketingItemsReturnBasket : err = AccessDatabaseWS601R(False)
                Case Is = RemoveTicketingItemsReturnBasket : err = AccessDatabaseWS602R()
                Case Is = AmendTicketingItemsReturnBasket : err = AccessDatabaseWS603R()
                Case Is = AddTicketingReservedItemsReturnBasket : err = AccessDatabaseWS604R()
                Case Is = AddseasonTicketRenewalsReturnBasket : err = AccessDatabaseWS605R()
                Case Is = RemoveTicketingExpiredBaskets : err = AccessDatabaseWS904R()
                Case Is = AddPPSItemsToBasket : err = AccessDatabaseWS606R()
                Case Is = AddAutomaticTicketingItemsReturnBasket : err = AccessDatabaseWS609R()
                Case Is = ClearAndAddTicketingItemsReturnBasket : err = AccessDatabaseWS601R(True)
                Case Is = VerifyAndUpdateExtOrderNumberToBasket : err = AccessDatabaseWS125R()
                Case Is = AddRetailToTicketingBasket : err = AccessDatabaseRT001S()
                Case Is = GetBasketExceptionSeats : err = AccessDatabaseWS006S()
            End Select

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDABAS-51"
                .HasError = True
            End With
        End Try
        Return err
    End Function

#End Region

#Region "AddPPSItemsToBasket"

    Protected Function AccessDatabaseWS606R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = ""

        Try

            'Get a list of all distinct schemes, and distinct seats (hold the customer number 
            'with the seat because a customer may buy more than 1 seat)
            '-----------------------------------------------------------
            Dim schemes As New Generic.List(Of String)

            'dictionary - key = seat number
            '           - value = customer number
            Dim seats As New Generic.Dictionary(Of String, String)

            For Each item As DePPSItem In De_AddPPS.PPSItems
                If Not schemes.Contains(item.ProductCode) Then
                    schemes.Add(item.ProductCode)
                End If
                If Not seats.ContainsKey(item.Seat) Then
                    seats.Add(item.Seat, item.CustomerNumber)
                End If
            Next
            '-----------------------------------------------------------

            Dim regPost As String = "N"
            If Me.De_AddPPS.RegisteredPost Then regPost = "Y"

            'scheme dictionary  - Key = scheme name (product code)
            '                   - Value = scheme array value
            Dim schemeDic As New Generic.Dictionary(Of String, String)

            'seats dictionary   - Key = seat number
            '                   - Value = customer number
            Dim seatsDic As New Generic.Dictionary(Of String, String)

            Dim schemeCount As Integer = 0
            Dim seatCount As Integer = 0
            Dim append As Boolean = False

            'loop through all disctinct schemes
            For Each scheme As String In schemes

                schemeCount += 1



                'build the scheme array
                schemeDic.Add(scheme, Utilities.FixStringLength(scheme, 6) _
                                + Utilities.FixStringLength(regPost, 1) _
                                + Utilities.FixStringLength("0", 1))

                'if the scheme dictionary has 12 entries, or we are on the last scheme in the
                'collection then we want to build the customer list
                If schemeDic.Keys.Count = 12 OrElse schemeCount = schemes.Count Then

                    'loop through each distinct customer (member)
                    For Each seat As String In seats.Keys

                        seatCount += 1

                        'build the seat and customer dictionary for the current call
                        seatsDic.Add(seat, seats(seat))

                        'if the customer list has 8 entries, or we are on the last customer
                        'in the collection then we need to proccess the schemes
                        If seatsDic.Keys.Count = 8 OrElse seatCount = seats.Count Then

                            'now we have our scheme dictionary and our customer list  we need
                            'to indicate which customers to enroll into which schemes, 
                            'build the scheme parameter string and process the call
                            PARAMOUT = ProcessSchemeCollection(schemeDic, seatsDic, append)

                            'only set to append mode after the first call
                            append = True

                            'We have made the call so clear down the seats dictionary
                            seatsDic.Clear()
                        End If
                    Next

                    'We have made the calls for all seats so clear down the schemes dictionary
                    'and reset the seat counter
                    schemeDic.Clear()
                    seatCount = 0

                End If

            Next

            Try
                PARAMOUTBasket = PARAMOUT.Substring("2048")
                PARAMOUT = PARAMOUT.Substring("0", "2048")
                ResultDataSet = New DataSet

                'Create the basket data tables
                _basketFunctions.CreateBasketTables(ResultDataSet)

                ' Is there an error code from AddTicketingItems
                If PARAMOUT.ToString.Trim.Length >= 2046 Then
                    Dim dRow As DataRow
                    dRow = ResultDataSet.Tables("BasketStatus").NewRow
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(2045, 2)
                    dRow("ValidationError") = PARAMOUT.Substring(2031, 1)
                    ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                    err.ErrorNumber = PARAMOUT.Substring(2045, 2)
                    err.HasError = True
                Else
                    ' Retrieve the ticketing shopping basket
                    _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS606R")
                End If

                DeAddTicketingItems.ErrorCode = ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode").ToString
                BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))
            Catch ex As Exception
            End Try

            'If PARAMOUT.Length >= 1023 Then
            '    Me.De_AddPPS.ErrorCode = PARAMOUT.Substring(1021, 2)
            'End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDAPPS-53"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function ProcessSchemeCollection(ByVal schemes As Generic.Dictionary(Of String, String), _
                                                ByVal seats As Generic.Dictionary(Of String, String), _
                                                ByVal append As Boolean) As String

        Dim PARAMOUT As String = ""
        Dim inParam As String = GetWS606RParam_Start(append)
        Dim customerAdded As Boolean = False
        Dim schemeParam As String = ""
        Dim tempSchemeParam As String = ""
        Dim seatParam As String = ""
        Dim custParam As String = ""
        Dim noCustomersAdded As Boolean = True

        'now we have our scheme dictionary and our customer list  we need
        'to indicate which customers to enroll into which schemes, 
        'build the scheme parameter string and process the call

        'to do this we need to:
        '   1) Loop through each of the schemes in our current collection
        '   2) Then for each scheme, loop through each customer in our customers list
        '   3) Then loop through each of the PPSItems in our data entity
        '       - if the scheme and the customer match we add a "1" to the current scheme array
        '       - otherwise we add a "0"

        For Each scheme As String In schemes.Keys
            tempSchemeParam = schemes(scheme)
            For Each seat As String In seats.Keys

                'reset the customer added flag
                customerAdded = False

                'loop through each PPSItem and test for a match
                For Each item As DePPSItem In Me.De_AddPPS.PPSItems

                    'if the product code, customer number and seat numbers
                    'match then set the entry to "1"
                    If item.ProductCode = scheme _
                        AndAlso item.CustomerNumber = seats(seat) _
                            AndAlso item.Seat = seat Then
                        tempSchemeParam += "Y"
                        customerAdded = True
                        noCustomersAdded = False
                        Exit For
                    End If
                Next

                'if we don't find a match for the current customer mark the entry as "0"
                If Not customerAdded Then tempSchemeParam += "N"
            Next

            'ensure the string is the correct length and append to the "in parameter"
            schemeParam += Utilities.FixStringLength(tempSchemeParam, 50)
        Next

        If Not noCustomersAdded Then
            'create the customer and seat parameters
            For Each seat As String In seats.Keys
                custParam += Utilities.PadLeadingZeros(seats(seat), 12)
                seatParam += Utilities.FixStringLength(seat, 16)
            Next

            'add the scheme, customer, and seat parameters and ensure they are the correct length
            inParam += Utilities.FixStringLength(schemeParam, 600)              '   e#schemeA       40      639     dim(12)
            inParam += Utilities.FixStringLength(custParam, 96)                 '   e#membA         640     735     dim(8)
            inParam += Utilities.FixStringLength(seatParam, 128)                '   e#seatA         736     863     dim(8)
            inParam += Utilities.FixStringLength(" ", 132)                      '   blank           864     995
            inParam += Utilities.FixStringLength(De_AddPPS.ByPassPreReqCheck, 1) '                  996     996
            inParam += Utilities.FixStringLength(De_AddPPS.SeasonTicket, 6)     '   e#mtcd          997     1002
            inParam += Utilities.FixStringLength(" ", 1)                        '   blank           1003    1003
            inParam += Utilities.PadLeadingZeros(seats.Keys.Count, 3)           '   e#total         1004    1006    
            inParam += Utilities.FixStringLength(" ", 18)                       '   blank           1007    1024
            inParam += Utilities.FixStringLength(" ", 1)                        '   e#82mode        1025    1025
            inParam += Utilities.FixStringLength("01", 2)                       '   e#payType       1026    1027
            inParam += Utilities.PadLeadingZeros(De_AddPPS.PPSStage, 2)         '   e#PPSPayStage   1028    1029
            inParam += Utilities.FixStringLength(" ", 94)                       '   blank           1030    1123



            inParam += Utilities.FixStringLength(" ", 898)                      '   blank           1124    2021
            inParam += Utilities.FixStringLength(Settings.OriginatingSource, 10) '  e#orginating    2022    2031
            inParam += Utilities.FixStringLength(" ", 1)                        '   blank           2032    2032
            inParam += Utilities.PadLeadingZeros(De_AddPPS.CustomerNumber, 12)  '   e#memb          2033    2044
            inParam += Utilities.FixStringLength(De_AddPPS.Source, 1)           '   e#source        2045    2045
            inParam += Utilities.FixStringLength(" ", 2)                        '   e#errc          2046    2047                        
            inParam += Utilities.FixStringLength(" ", 1)                        '   e#errf          2048    2048


            PARAMOUT = CallWS606R(inParam)
        End If



        Return PARAMOUT

    End Function

    Private Function GetWS606RParam_Start(ByVal append As Boolean) As String
        Dim param As String = ""
        param = Utilities.FixStringLength(De_AddPPS.SessionId, 36)      '   e#uniq      1       36
        If append Then
            param += Utilities.FixStringLength("P", 1)                  '   e#83mode    37      37
        Else
            param += Utilities.FixStringLength(" ", 1)                  '   e#83mode    37      37
        End If
        param += Utilities.PadLeadingZeros(De_AddPPS.PPSStage, 2)       '   e#PPSStage  38      39

        Return param
    End Function

    Private Function GetWS606RParam2() As String
        Dim param As String = ""
        'param = Utilities.FixStringLength("", 5062)
        'param += Utilities.FixStringLength(De_AddPPS.SessionId, 36)
        'param += Utilities.PadLeadingZeros(De_AddPPS.CustomerNumber, 12)
        'param += Utilities.FixStringLength(" ", 3)
        'param += Utilities.FixStringLength(" ", 3)
        'param += Utilities.FixStringLength(De_AddPPS.Source, 1)
        'param += Utilities.FixStringLength(" ", 2)
        'param += Utilities.FixStringLength(" ", 1)
        param += Utilities.FixStringLength(" ", 5120)

        Return param
    End Function

    Protected Function CallWS606R(ByVal inParam As String) As String
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS606R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 2048)
        parmIO.Value = inParam
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIO2.Value = GetWS606RParam2()
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS606R", De_AddPPS.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT = Utilities.FixStringLength(PARAMOUT, "2048") & PARAMOUTBasket

        TalentCommonLog("CallWS606R", De_AddPPS.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)
        Return PARAMOUT
    End Function

#End Region

#Region "AddAutomaticTicketingItemsReturnBasket"

    Protected Function AccessDatabaseWS609R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True

        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Try
            '
            'Call WS609R
            PARAMOUT = CallWS609R()
            PARAMOUTBasket = PARAMOUT.Substring("1024")
            PARAMOUT = PARAMOUT.Substring("0", "1024")

            '
            ' Is there an error code from AddTicketingItems

            If PARAMOUT.ToString.Trim.Length >= 1023 Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                err.ErrorNumber = PARAMOUT.Substring(1021, 2)
                err.HasError = True
            Else
                ' Retrieve the ticketing shopping basket
                _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS609R")
            End If

            DeAddTicketingItems.ErrorCode = ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode").ToString
            BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAA-53"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS609R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS609R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS609Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS601R", DeAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT = Utilities.FixStringLength(PARAMOUT, "1024") & PARAMOUTBasket

        TalentCommonLog("CallWS601R", DeAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)

        Return PARAMOUT

    End Function

    Private Function WS609Parm() As String

        Dim myString As String

        Dim autoAdd As String = ""

        If Settings.AutoAddMembership Then
            autoAdd = "Y"
        Else
            autoAdd = "N"
        End If

        'Construct the parameter
        myString = Utilities.FixStringLength( _
                                            Utilities.FixStringLength(DeAddTicketingItems.SessionId, 36) & _
                                            Utilities.PadLeadingZeros(DeAddTicketingItems.CustomerNumber, 12) & _
                                            Utilities.FixStringLength(autoAdd, 1) _
                                            , 1024)


        Return myString

    End Function

#End Region

#Region "AddTicketingItems"

    Protected Function AccessDatabaseWS501R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty

        Try

            'Call WS501R
            PARAMOUT = CallWS501R()

            If PARAMOUT.Length >= 1023 Then
                DeAddTicketingItems.ErrorCode = PARAMOUT.Substring(1021, 2)
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-53"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS501R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS501R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS501Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS501R", DeAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS501R", DeAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)
        Return PARAMOUT

    End Function

    Private Function WS501Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(DeAddTicketingItems.SessionId, 36) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ProductCode, 6) & _
                    Utilities.FixStringLength(DeAddTicketingItems.StandCode, 3) & _
                    Utilities.FixStringLength(DeAddTicketingItems.AreaCode, 4) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand01, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand02, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand03, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand04, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand05, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand06, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand07, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand08, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand09, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand10, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand11, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand12, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand13, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand14, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand15, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand16, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand17, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand18, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand19, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand20, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand21, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand22, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand23, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand24, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand25, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceBand26, 1) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity01, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity02, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity03, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity04, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity05, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity06, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity07, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity08, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity09, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity10, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity11, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity12, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity13, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity14, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity15, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity16, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity17, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity18, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity19, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity20, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity21, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity22, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity23, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity24, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity25, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity26, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.PriceCode, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ReservationCode, 2) & _
                    Utilities.FixStringLength("", 876) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ExcludeRestrictedSeats, 1) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.CustomerNumber, 12) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Source, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ErrorCode, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ErrorFlag, 1)


        Return myString

    End Function

#End Region

#Region "AddTicketingItemsReturnBasket"

    Protected Function AccessDatabaseWS601R(ByVal isClearAndAdd As Boolean) As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Try
            PARAMOUT = CallWS601R(isClearAndAdd, PARAMOUTBasket)

            ' Is there an error code from AddTicketingItems
            dRow = ResultDataSet.Tables("BasketStatus").NewRow
            If Not String.IsNullOrEmpty(PARAMOUT.Substring(1023, 1).Trim()) OrElse Not String.IsNullOrEmpty(PARAMOUT.Substring(1021, 2).Trim()) Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                err.ErrorNumber = PARAMOUT.Substring(1021, 2)
                err.HasError = True
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                err.HasError = False
            End If
            ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)

            If Not err.HasError Then
                ' Retrieve the ticketing shopping basket
                _basketFunctions.LinkedProductId = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(1095, 13).Trim())
                If PARAMOUTBasket.Substring(12, 6).Trim <> String.Empty Then _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS601R")
            End If

            AlternativeSeatSelected = convertToBool(PARAMOUT.Substring(1002, 1))
            ClearAvailableStandAreaCache = convertToBool(PARAMOUT.Substring(1003, 1))
            OrphanSeatRemaining = convertToBool(PARAMOUT.Substring(1004, 1))
            BasketRequiresRedirectToBookingOrComponentPage = PARAMOUT.Substring(665, 1)
            DeAddTicketingItems.PackageID = If(String.IsNullOrEmpty(PARAMOUT.Substring(639, 13).Trim()), 0, Long.Parse(PARAMOUT.Substring(639, 13).Trim()))
            BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))

            DeAddTicketingItems.ProductTransactionTicketLimitExceeded = False
            If Not String.IsNullOrEmpty(PARAMOUTBasket.Substring(4894, 3).Trim()) AndAlso PARAMOUTBasket.Substring(4894, 3).Trim() <> "000" Then
                DeAddTicketingItems.ProductTransactionTicketLimitExceeded = True
                Integer.TryParse(PARAMOUTBasket.Substring(4894, 3).Trim(), DeAddTicketingItems.ProductTransactionTicketLimit)
            End If


            If ResultDataSet.Tables.Count = 3 AndAlso ResultDataSet.Tables(0).Rows.Count > 0 Then
                DeAddTicketingItems.ErrorCode = ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode").ToString
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-53B"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS601R(ByVal isClearAndAdd As Boolean, ByRef PARAMOUTBasket As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS601R(@PARAM1, @PARAM2, @PARAM3)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim paramSeatArray As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS601Parm(isClearAndAdd)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        paramSeatArray = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 32765)
        paramSeatArray.Value = WS601ParmSeatArray()
        paramSeatArray.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS601R", DeAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value & " paramSeatArray.Value=" & paramSeatArray.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        TalentCommonLog("CallWS601R", DeAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)
        Return PARAMOUT
    End Function

    Private Function WS601Parm(ByVal isClearAndAdd As Boolean) As String
        Dim myString As New StringBuilder
        Dim productTypeAway As String = String.Empty
        If (DeAddTicketingItems.ProductType <> Nothing) Then
            If (DeAddTicketingItems.ProductType.Equals("A")) Then
                productTypeAway = "Y"
            Else
                productTypeAway = "N"
            End If
        Else
            productTypeAway = "N"
        End If

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SessionId, 36))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.StandCode, 3))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.AreaCode, 4)) '49

        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand01, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand02, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand03, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand04, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand05, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand06, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand07, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand08, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand09, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand10, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand11, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand12, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand13, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand14, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand15, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand16, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand17, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand18, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand19, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand20, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand21, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand22, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand23, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand24, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand25, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceBand26, 1)) '75

        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.BulkSalesId, 13))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.STExceptionSeasonTicketProductCode, 6))
        If DeAddTicketingItems.PickingNewComponentSeat Then
            myString.Append(Utilities.FixStringLength("C", 1))
        Else
            myString.Append(Utilities.FixStringLength(DeAddTicketingItems.STExceptionChangeRemoveMode, 1))
        End If
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.PickingNewComponentSeat), 1)) '96
        myString.Append(Utilities.FixStringLength(String.Empty, 27))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.IncludeTicketExchangeSeats), 1))
        myString.Append(Utilities.FixStringLength(Settings.DeliveryCountryCode, 3))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.PriceCode, 2))
        myString.Append(Utilities.FixStringLength("", 40)) '169

        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat01, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat02, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat03, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat04, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat05, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat06, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat07, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat08, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat09, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat10, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat11, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat12, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat13, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat14, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat15, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat16, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat17, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat18, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat19, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat20, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat21, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat22, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat23, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat24, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat25, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat26, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat27, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat28, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat29, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat30, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat31, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat32, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat33, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat34, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat35, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat36, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat37, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat38, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat39, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat40, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat41, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat42, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat43, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat44, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat45, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat46, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat47, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat48, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat49, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.RowSeat50, 9)) '619

        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.FailOption, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.CampaignCode, 2))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ExcludeRestrictedSeats, 1)) '623
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ProductDetailCode, 6))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.PackageID, 13))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.SeatComponentID, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.FavouriteSeatSelected), 1)) '667

        myString.Append(Utilities.FixStringLength("", 181))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.VariablePricedProductPrice, 9))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.SmartcardAmount, 9))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SmartcardNumber, 14))
        myString.Append(Utilities.FixStringLength("", 109))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.AllMandatoryLinkedProductsAdded, True), 1))
        myString.Append(Utilities.PadLeadingZeros((DeAddTicketingItems.DefaultPrice) * 100, 11)) '991
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) 'clear cache Y/N
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) 'alternative seat selected Y/N
        myString.Append(Utilities.FixStringLength(ConvertToYN(isClearAndAdd), 1)) '1004 to clear before add basket
        myString.Append(Utilities.FixStringLength("", 1)) 'Single seat flag Y/N
        myString.Append(Utilities.FixStringLength(ConvertToYN(Settings.PerformWatchListCheck), 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ByPassPreReqCheck, 1))
        myString.Append(Utilities.FixStringLength(productTypeAway, 1))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Source, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ErrorCode, 2))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ErrorFlag, 1)) '1025

        myString.Append(Utilities.FixStringLength("", 13))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.IncludeTravelProduct, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.CATMode, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.CATSeatDetails, 40))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.CATPayRef, 15)) '1095

        myString.Append(Utilities.FixStringLength("", 1))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.LinkedProductID, 13))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.ProductIsMandatory), 1))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.ProductHasMandtoryRelatedProducts), 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.TicketText, 60))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.CallID.ToString(), 13))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.LinkedMasterProduct, 6)) '1190

        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity01, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity02, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity03, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity04, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity05, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity06, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity07, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity08, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity09, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity10, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity11, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity12, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity13, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity14, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity15, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity16, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity17, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity18, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity19, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity20, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity21, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity22, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity23, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity24, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity25, 5))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.Quantity26, 5))

        myString.Append(Utilities.FixStringLength(String.Empty, 6)) '1324
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.SelectedPriceMinimum.ToString().Replace(".", String.Empty), 9))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.SelectedPriceMaximum.ToString().Replace(".", String.Empty), 9))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.SelectedPriceBreakId, 13))
        myString.Append(ConvertToYN(DeAddTicketingItems.AgentCanGiveDirectDebitRefund, 1))
        Return myString.ToString()
    End Function

    Private Function WS601ParmSeatArray() As String
        Dim myString As New StringBuilder
        Dim myPriceBands As New StringBuilder
        Dim productCodes As New StringBuilder
        Dim myBlanks As Integer = 0
        If DeAddTicketingItems.SeatSelectionArray IsNot Nothing AndAlso DeAddTicketingItems.SeatSelectionArray.Count > 0 Then
            Dim seatCounter As Integer = 0
            Do While seatCounter < DeAddTicketingItems.SeatSelectionArray.Count AndAlso seatCounter <= 500
                myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).Stand, 3))
                myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).Area, 4))
                myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).Row, 4))
                myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).Seat, 4))
                myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).AlphaSuffix, 1))
                myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).CATSeatStatus, 1))

                'Need to add code here for the price band selected.
                myPriceBands.Append(DeAddTicketingItems.SeatSelectionArray(seatCounter).PriceBand)

                productCodes.Append(Utilities.FixStringLength(DeAddTicketingItems.SeatSelectionArray(seatCounter).ProductCode, 6))

                seatCounter += 1
            Loop
        Else
            myString.Append("")
        End If
        myBlanks = 8500 - myString.Length
        myString.Append(Utilities.FixStringLength(String.Empty, myBlanks)) 'Add required blanks to mystring
        myString.Append(myPriceBands)
        myBlanks = 9000 - myString.Length
        myString.Append(Utilities.FixStringLength(String.Empty, myBlanks)) 'Add required blanks to mystring
        myString.Append(productCodes)
        Return myString.ToString()
    End Function

#End Region

#Region "AddMultipleTicketingItemsReturnBasket"

    Protected Function AccessDatabaseWS621R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)
        Try
            PARAMOUT = CallWS621R(PARAMOUTBasket)

            If PARAMOUT.Substring(10238, 1) = "E" Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(10236, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                err.ErrorNumber = PARAMOUT.Substring(10236, 2)
                err.HasError = True
            Else
                If PARAMOUTBasket.Substring(12, 6).Trim <> String.Empty Then _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS621R")
            End If

            'Add any error codes from the list of products back into the list object
            Dim iPosition As Integer = 0
            For Each product As DEAddTicketingItems In ListOfDEAddTicketingItems
                product.ErrorCode = PARAMOUT.Substring(iPosition + 28, 2).Trim()
                iPosition += 100
            Next
            BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-WS621R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS621R(ByRef PARAMOUTBasket As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS621R(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS621Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        TalentCommonLog("CallWS621R", De.CustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        TalentCommonLog("CallWS621R", De.CustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)
        Return PARAMOUT
    End Function

    Private Function WS621Parm() As String
        Dim myString As New StringBuilder

        For Each product As DEAddTicketingItems In ListOfDEAddTicketingItems
            myString.Append(Utilities.FixStringLength(product.ProductCode, 6))
            myString.Append(Utilities.FixStringLength(ConvertToYN(product.ProductIsMandatory), 1))
            myString.Append(Utilities.FixStringLength(product.PriceCode, 2))
            myString.Append(Utilities.FixStringLength(product.PriceBand01, 1))
            myString.Append(Utilities.FixStringLength(product.StandCode, 3))
            myString.Append(Utilities.FixStringLength(product.AreaCode, 4))
            myString.Append(Utilities.PadLeadingZeros(product.Quantity01, 5))
            myString.Append(Utilities.FixStringLength(product.ProductDetailCode, 6))
            myString.Append(Utilities.FixStringLength(product.CampaignCode, 2))
            myString.Append(Utilities.FixStringLength(String.Empty, 2)) 'Error Code
            myString.Append(Utilities.FixStringLength(product.LinkedParentProductCode, 6))
            myString.Append(Utilities.FixStringLength(String.Empty, 62)) 'Spare
        Next
        Dim counter As Integer = ListOfDEAddTicketingItems.Count
        Do Until counter = 100
            myString.Append(Utilities.FixStringLength(String.Empty, 100))
            counter += 1
        Loop
        '10150
        myString.Append(Utilities.FixStringLength(String.Empty, 163))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) 'Package Mode
        myString.Append(Utilities.FixStringLength(De.SessionId, 36))
        myString.Append(Utilities.PadLeadingZeros(De.LinkedProductId, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) 'Allocation Error
        myString.Append(Utilities.PadLeadingZeros(De.CustomerNo, 12))
        myString.Append(Utilities.FixStringLength(De.Src, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3)) 'Error code and flag

        Return myString.ToString()
    End Function

#End Region

#Region "AddTicketingReservedItems"

    Protected Function AccessDatabaseWS218R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty

        Try

            'Call WS218R
            PARAMOUT = CallWS218R()

            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                DeAddTicketingItems.ErrorCode = PARAMOUT.Substring(1021, 2)
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-WS018R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS218R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS218R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS218Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS218R", DeAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS218R", DeAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS218Parm() As String

        Dim myString As String

        Dim mode As String = String.Empty
        If DeAddTicketingItems.ProductType <> String.Empty Then
            mode = DeAddTicketingItems.ProductType
        End If

        'Construct the parameter
        myString = Utilities.FixStringLength(DeAddTicketingItems.SessionId, 36) & _
                    Utilities.FixStringLength("", 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.SignedInCustomer, 12) & _
                    Utilities.FixStringLength(Settings.OriginatingSource, 10) & _
                    Utilities.FixStringLength("", 20) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Quantity01, 7) & _
                    Utilities.FixStringLength("", 902) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ByPassPreReqCheck, 1) & _
                    Utilities.FixStringLength(mode, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Stadium1, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Stadium2, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Stadium3, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Stadium4, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Stadium5, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Stadium6, 2) & _
                    Utilities.PadLeadingZeros(DeAddTicketingItems.CustomerNumber, 12) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ProductCode, 6) & _
                    Utilities.FixStringLength(DeAddTicketingItems.Source, 1) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ErrorCode, 2) & _
                    Utilities.FixStringLength(DeAddTicketingItems.ErrorFlag, 1)

        Return myString

    End Function

#End Region

#Region "AddTicketingReservedItemsReturnBasket"

    Protected Function AccessDatabaseWS604R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing


        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Try
            '
            'Call WS604R
            PARAMOUT = CallWS604R()
            PARAMOUTBasket = PARAMOUT.Substring("1024")
            PARAMOUT = PARAMOUT.Substring("0", "1024")

            '
            ' Is there an error code from AddTicketingReservedItems

            If PARAMOUT.ToString.Trim.Length >= 1023 Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)

                err.ErrorNumber = PARAMOUT.Substring(1021, 2)
                err.HasError = True
            Else
                ' Retrieve the ticketing shopping basket
                _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS604R")

                ' Has a product been added to the basket
                DeAddTicketingItems.ReservedSeats = PARAMOUT.Substring(36, 1)
                BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))
            End If

            DeAddTicketingItems.ErrorCode = ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode").ToString

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-WS604R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS604R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS604R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS218Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS604R", DeAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT = Utilities.FixStringLength(PARAMOUT, "1024") & PARAMOUTBasket

        TalentCommonLog("CallWS604R", DeAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)

        Return PARAMOUT

    End Function

#End Region

#Region "AddSeasonTicketRenewalsReturnBasket"

    Protected Function AccessDatabaseWS605R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing


        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        '
        ' If we are logging specific supplyNetRequests then create a new
        ' log record for this call. Note - do not set overwrite flag = true
        ' as this routine is called multiple times for a supplynet request
        '
        createSupplyNetRequest(Settings.Partner, Settings.LoginId, Settings.SupplyNetRequestName, Settings.TransactionID, Settings.RequestCount, 0, Now, Nothing, False)

        Try
            '
            'Call WS605R
            PARAMOUT = CallWS605R()
            PARAMOUTBasket = PARAMOUT.Substring("1024")
            PARAMOUT = PARAMOUT.Substring("0", "1024")

            '
            ' If we are logging specific supplyNetRequests then update the progress count for
            ' this call. 
            '
            If incrementSupplyNetProgressCount(Settings.TransactionID) = Settings.RequestCount Then
                markSupplyNetTransactionAsCompleted(Settings.TransactionID)
            End If

            '
            ' Is there an error code from AddSeasonTicketRenewals
            If PARAMOUT.ToString.Trim.Length >= 1023 Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                err.ErrorNumber = PARAMOUT.Substring(1021, 2)
                err.HasError = True
            Else
                ' Retrieve the ticketing shopping basket
                _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS605R")

                ' Has a product been added to the basket
                DeAddTicketingItems.ReservedSeats = PARAMOUT.Substring(36, 1)
                BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))
            End If

            DeAddTicketingItems.ErrorCode = ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode").ToString

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-WS605R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS605R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS605R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS605Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS605R", DeAddTicketingItems.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUT = Utilities.FixStringLength(PARAMOUT, "1024") & PARAMOUTBasket

        TalentCommonLog("CallWS605R", DeAddTicketingItems.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)

        Return PARAMOUT

    End Function

    Private Function WS605Parm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.SessionId, 36))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.IgnoreFriendsAndFamily, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.StandCode, 3))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.AreaCode, 4))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Row, 4))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Seat, 4))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Suffix, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.AllocatedCustNo, 12))
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) '67 bypass WS036R flag
        myString.Append(Utilities.FixStringLength(String.Empty, 13)) '68 Linked Product ID
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) '82 Product mandatory
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeAddTicketingItems.ProductHasMandtoryRelatedProducts), 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Stadium2, 2)) '83
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Stadium3, 2))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Stadium4, 2))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Stadium5, 2))
        myString.Append(Utilities.FixStringLength("", 900))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10)) '991
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Stadium1, 2))
        myString.Append(Utilities.PadLeadingZeros(DeAddTicketingItems.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(String.Empty, 6))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.Source, 1))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ErrorCode, 2))
        myString.Append(Utilities.FixStringLength(DeAddTicketingItems.ErrorFlag, 1))

        Return myString.ToString()
    End Function

#End Region

#Region "RemoveTicketingItemsReturnBasket"

    Private Function AccessDatabaseWS602R(Optional ByVal returnBasket As Boolean = True) As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Try

            'Call WS602R
            CallWS602R(PARAMOUT, PARAMOUTBasket)

            ' Set the Status table indicating any errors
            If PARAMOUT.ToString.Trim.Length >= 4999 Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(4997, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                err.ErrorNumber = PARAMOUT.Substring(4997, 2)
                err.HasError = True
            Else
                ' Retrieve the ticketing shopping basket
                If returnBasket Then
                    _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS602R")
                End If
            End If
            OrphanSeatRemaining = convertToBool(PARAMOUT.Substring(49, 1))
            BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-54B"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS602R(ByRef PARAMOUT As String, ByRef PARAMOUTBasket As String) As String
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS602R(@PARAM1, @PARAM2, @PARAM3)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = Utilities.FixStringLength("", 1024)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5000)
        parmIO2.Value = WS602Parm(PARAMOUT, PARAMOUTBasket)
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS602R", De.CustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param3).Value.ToString

        TalentCommonLog("CallWS602R", De.CustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)

        Return PARAMOUT
    End Function

    Private Function WS602Parm(ByRef PARAMOUT As String, ByRef PARAMOUTBasket As String) As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(De.SessionId, 36))
        myString.Append(Utilities.FixStringLength("X", 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        myString.Append(Utilities.FixStringLength(String.Empty, 446))
        myString.Append(Utilities.FixStringLength(De.ProductCode, 6))
        myString.Append(Utilities.PadLeadingZeros(De.CustomerNo, 12)) '518

        'Package or Ticketing
        If Utilities.CheckForDBNull_Decimal(De.PackageID) > 0 Then
            myString.Append(Utilities.PadLeadingZeros(De.PackageID, 13))
            myString.Append(Utilities.FixStringLength(PACKAGEPRODUCT, 1))
            myString.Append(Utilities.FixStringLength(String.Empty, 2))
        Else
            If _bulkSalesMode Then
                myString.Append(Utilities.FixStringLength(String.Empty, 16))
            Else
                myString.Append(Utilities.FixStringLength(De.SeatDetails1.Stand, 3))
                myString.Append(Utilities.FixStringLength(De.SeatDetails1.Area, 4))
                myString.Append(Utilities.FixStringLength(De.SeatDetails1.Row, 4))
                myString.Append(Utilities.FixStringLength(De.SeatDetails1.Seat, 4))
                myString.Append(Utilities.FixStringLength(De.SeatDetails1.AlphaSuffix, 1)) '534
            End If
        End If

        myString.Append(Utilities.FixStringLength(De.PriceCode, 2))
        myString.Append(Utilities.FixStringLength(String.Empty, 465)) '1001


        myString.Append(Utilities.FixStringLength(De.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(De.ByPassPreReqCheck, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 12))
        myString.Append(Utilities.PadLeadingZeros(String.Empty, 5))
        myString.Append(Utilities.PadLeadingZeros(De.BulkSalesID, 13)) '1038
        myString.Append(Utilities.FixStringLength(De.SavePackageMode, 1)) '1039
        myString.Append(Utilities.FixStringLength(String.Empty, 2954))
        myString.Append(Utilities.FixStringLength(Settings.DeliveryCountryCode, 3))

        myString.Append(Utilities.FixStringLength(String.Empty, 1000))
        myString.Append(Utilities.FixStringLength(De.Src, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        Return myString.ToString()

    End Function

#End Region

#Region "AmendTicketingItems"

    Protected Function AccessDatabaseWS038R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty

        Try

            'Call WS038R
            PARAMOUT = CallWS038R()

            If PARAMOUT.Length >= 3069 Then
                DeAmendTicketingItems.ErrorCode = PARAMOUT.Substring(3069, 2)
                '                err = AccessDatabaseWS036R()
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-52"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS038R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS038R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS038Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS038R", DeAmendTicketingItems.CustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS038R", DeAmendTicketingItems.CustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS038Parm() As String

        Dim myString As String
        Dim intCount As Integer = 1
        Dim intDetails As Integer = 0
        Dim DEtbi As New DETicketingBasketItem
        Dim DEtid As New DETicketingItemDetails

        'Construct the parameter
        myString = Utilities.FixStringLength(DeAmendTicketingItems.SessionID, 36) & _
                    Utilities.FixStringLength("02", 2)

        Do While intCount <= 50
            If DeAmendTicketingItems.CollAmendItems.Count > 0 And intCount <= DeAmendTicketingItems.CollAmendItems.Count Then
                DEtbi = CType(DeAmendTicketingItems.CollAmendItems(intCount), DETicketingBasketItem)
                DEtid.SeatDetails1.FormattedSeat = DEtbi.Seat
                myString = myString & Utilities.FixStringLength(DEtbi.PurchaseMemberNo, 12) & _
                                        Utilities.FixStringLength(DEtbi.AllocatedMemberNo, 12) & _
                                        Utilities.FixStringLength(DEtbi.ProductCode, 6)

                ' Add Seat details 
                If DEtbi.Seat.Contains("/") Then
                    myString += Utilities.FixStringLength(DEtid.SeatDetails1.Stand, "3") & _
                                            Utilities.FixStringLength(DEtid.SeatDetails1.Area, "4") & _
                                            Utilities.FixStringLength(DEtid.SeatDetails1.Row, "4") & _
                                            Utilities.FixStringLength(DEtid.SeatDetails1.Seat, "4") & _
                                            Utilities.FixStringLength(DEtid.SeatDetails1.AlphaSuffix, "1")
                Else
                    myString += Utilities.FixStringLength(DEtbi.Seat, "16")
                End If

                myString += Utilities.FixStringLength(DEtbi.PriceCode, 2) & _
                                        Utilities.FixStringLength(DEtbi.PriceBand, 1) & _
                                        Utilities.FixStringLength(DEtbi.FulfilmentMethod, 1)
                intDetails = intDetails + 1
            Else
                myString = myString & Utilities.FixStringLength("", 50)
            End If
            intCount = intCount + 1
        Loop

        myString = myString & Utilities.FixStringLength("", 516) & _
                                Utilities.PadLeadingZeros(intDetails, 2) & _
                                Utilities.PadLeadingZeros(DeAmendTicketingItems.CustomerNo, 12) & _
                                Utilities.FixStringLength(DeAmendTicketingItems.Src, 1) & _
                                Utilities.FixStringLength("", 2) & _
                                Utilities.FixStringLength("", 1)


        Return myString

    End Function

#End Region

#Region "AmendTicketingItemsReturnBasket"

    Protected Function AccessDatabaseWS603R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)
        Try
            'Call WS603R (== WS038R + ws036R)
            CallWS603R(PARAMOUT, PARAMOUTBasket)

            ' Set the Status table indicating any errors
            If PARAMOUT.ToString.Trim.Length >= 3070 Then
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                err.ErrorNumber = PARAMOUT.Substring(3069, 2)
                err.HasError = True
            Else
                ' Retrieve the ticketing shopping basket
                _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS603R")
                BasketHasExceptionSeats = convertToBool(PARAMOUTBasket.Substring(4893, 1))
            End If
            DeAmendTicketingItems.ErrorCode = ResultDataSet.Tables("BasketStatus").Rows(0).Item("ReturnCode").ToString
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDABAS-52B"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS603R(ByRef PARAMOUT As String, ByRef PARAMOUTBasket As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS603R(@PARAM1, @PARAM2, @PARAM3)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter
        Dim parm3 As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS603Parm()
        parmIO.Direction = ParameterDirection.InputOutput
        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput
        parm3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 15000)
        parm3.Value = W603Parm3()
        parm3.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS603R", DeAmendTicketingItems.CustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString
        TalentCommonLog("CallWS603R", DeAmendTicketingItems.CustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT & ", PARAMOUTBasket=" & PARAMOUTBasket)

        Return PARAMOUT
    End Function

    Private Function W603Parm3() As String
        Dim myString As New StringBuilder
        Dim intCount As Integer = 1
        Dim intDetails As Integer = 0
        Dim DEtbi As New DETicketingBasketItem
        Dim DEtid As New DETicketingItemDetails

        Do While intCount <= 50
            If DeAmendTicketingItems.CollAmendItems.Count > 0 And intCount <= DeAmendTicketingItems.CollAmendItems.Count Then
                DEtbi = CType(DeAmendTicketingItems.CollAmendItems(intCount), DETicketingBasketItem)
                If Not _bulkSalesMode Then DEtid.SeatDetails1.FormattedSeat = DEtbi.Seat
                myString.Append(Utilities.FixStringLength(DEtbi.PurchaseMemberNo, 12))
                myString.Append(Utilities.FixStringLength(DEtbi.AllocatedMemberNo, 12))
                myString.Append(Utilities.FixStringLength(DEtbi.ProductCode, 6))

                ' Add Seat or Package  details 
                If DEtbi.Seat.Trim.Length > 0 Then
                    If DEtbi.Seat.Contains("/") Then
                        myString.Append(Utilities.FixStringLength(DEtid.SeatDetails1.Stand, 3))
                        myString.Append(Utilities.FixStringLength(DEtid.SeatDetails1.Area, 4))
                        myString.Append(Utilities.FixStringLength(DEtid.SeatDetails1.Row, 4))
                        myString.Append(Utilities.FixStringLength(DEtid.SeatDetails1.Seat, 4))
                        myString.Append(Utilities.FixStringLength(DEtid.SeatDetails1.AlphaSuffix, 1))
                    Else
                        myString.Append(Utilities.FixStringLength(DEtbi.Seat, 16))
                    End If
                ElseIf DEtbi.PackageID > 0 Then
                    myString.Append(Utilities.PadLeadingZeros(DEtbi.PackageID.ToString, 13))
                    myString.Append(Utilities.FixStringLength(PACKAGEPRODUCT, 1))
                    myString.Append(Utilities.FixStringLength(String.Empty, 2))
                Else
                    myString.Append(Utilities.FixStringLength(String.Empty, 16))
                End If
                myString.Append(Utilities.FixStringLength(DEtbi.PriceCode, 2))
                myString.Append(Utilities.FixStringLength(DEtbi.PriceBand, 1))
                myString.Append(Utilities.FixStringLength(DEtbi.FulfilmentMethod, 1))
                myString.Append(Utilities.FixStringLength(DEtbi.PriceCodeOverridden, 2))
                myString.Append(Utilities.FixStringLength(DEtbi.TicketText, 60))
                myString.Append(Utilities.PadLeadingZeros(DEtbi.BulkSalesID, 13))
                myString.Append(Utilities.PadLeadingZeros(DEtbi.BulkSalesQuantity, 5))
                myString.Append(Utilities.FixStringLength(DEtbi.AllocatedSeat.Stand, 3))
                myString.Append(Utilities.FixStringLength(DEtbi.AllocatedSeat.Area, 4))
                myString.Append(Utilities.FixStringLength(DEtbi.AllocatedSeat.Row, 4))
                myString.Append(Utilities.FixStringLength(DEtbi.AllocatedSeat.Seat, 4))
                myString.Append(Utilities.FixStringLength(DEtbi.AllocatedSeat.AlphaSuffix, 1))
                myString.Append(Utilities.FixStringLength(String.Empty, 154))
                intDetails = intDetails + 1
            Else
                myString.Append(Utilities.FixStringLength(String.Empty, 300))
            End If
            intCount = intCount + 1
        Loop

        Return myString.ToString()
    End Function

    Private Function WS603Parm() As String
        Dim myString As New StringBuilder
        Dim intCount As Integer = 1
        Dim intDetails As Integer = 0
        Dim DEtbi As New DETicketingBasketItem
        Dim DEtid As New DETicketingItemDetails

        Do While intCount <= 50
            If DeAmendTicketingItems.CollAmendItems.Count > 0 And intCount <= DeAmendTicketingItems.CollAmendItems.Count Then
                intDetails = intDetails + 1
            End If
            intCount = intCount + 1
        Loop
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.SessionID, 36))
        If intDetails <= 0 Then
            'only country code changed
            myString.Append(Utilities.FixStringLength("04", 2))
        Else
            'seats are changed
            myString.Append(Utilities.FixStringLength("02", 2))
        End If
        myString.Append(Utilities.FixStringLength(String.Empty, 48))
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.CustomerSelection, 1))
        myString.Append(Utilities.FixStringLength(Settings.DeliveryCountryCode, 3)) '90
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.OverrideBasketErrorCode, 1))
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.ProductCodeInError, 6))
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.StandCodeInError, 3))
        myString.Append(Utilities.FixStringLength(String.Empty, 2938))
        myString.Append(Utilities.FixStringLength(ConvertToYN(Settings.PerformWatchListCheck), 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 4))
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.ByPassPreReqCheck, 1))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        myString.Append(Utilities.PadLeadingZeros(intDetails, 2))
        myString.Append(Utilities.PadLeadingZeros(DeAmendTicketingItems.CustomerNo, 12))
        myString.Append(Utilities.FixStringLength(DeAmendTicketingItems.Src, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 2))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))

        Return myString.ToString()
    End Function


#End Region

#Region "RetreiveTicketingItems"

    Private Function AccessDatabaseWS036R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Try

            ' Retrieve the ticketing shopping basket
            _basketFunctions.RetrieveTicketingShoppingBasketValues(WS036RFirstParm, ResultDataSet, err, "WS036R")
            If Not String.IsNullOrEmpty(WS036RFirstParm) Then
                BasketHasExceptionSeats = convertToBool(WS036RFirstParm.Substring(4893, 1))
            Else
                BasketHasExceptionSeats = ResultDataSet.Tables("BasketDetail").Rows(0).Item("BasketHasExceptionSeats")
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBRS-55"
                .HasError = True
            End With
        End Try

        Return err

    End Function

#End Region

#Region "RemoveTicketingExpiredItems"

    Private Function AccessDatabaseWS904R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim DtBasketIdResults As New DataTable
        ResultDataSet.Tables.Add(DtBasketIdResults)
        With DtBasketIdResults.Columns
            .Add("BasketId", GetType(String))
        End With

        Try

            'Call WS904R
            PARAMOUT = CallWS904R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                Dim strOut As String = PARAMOUT.Substring(0, 3050)
                Dim charHash As String = "#"
                Dim intLoopCount As Integer
                Dim rowArray() As String = Split(strOut, charHash)
                Do While intLoopCount < rowArray.Length

                    ' Write new data table row
                    If rowArray(intLoopCount).Trim <> "" Then
                        dRow = Nothing
                        dRow = DtBasketIdResults.NewRow
                        dRow("BasketId") = rowArray(intLoopCount).Trim
                        DtBasketIdResults.Rows.Add(dRow)

                    End If

                    'Increment and loop again
                    intLoopCount = intLoopCount + 1
                Loop
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBRS-WS904R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS904R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS904R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS904Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        'TalentCommonLog("CallWS904R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        'TalentCommonLog("CallWS904R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS904Parm() As String

        Dim myString As String
        Dim stadium0 As String = String.Empty
        Dim stadium1 As String = String.Empty
        Dim stadium2 As String = String.Empty
        Dim stadium3 As String = String.Empty
        Dim stadium4 As String = String.Empty
        Dim stadium5 As String = String.Empty

        Dim sStadia() As String = De.StadiumCode.Split(",")
        Dim count As Integer = 0
        Do While count < sStadia.Length
            Select Case count
                Case Is = 0
                    stadium0 = sStadia(0)
                Case Is = 1
                    stadium1 = sStadia(1)
                Case Is = 2
                    stadium2 = sStadia(2)
                Case Is = 3
                    stadium3 = sStadia(3)
                Case Is = 4
                    stadium4 = sStadia(4)
                Case Is = 5
                    stadium5 = sStadia(5)
            End Select
            count = count + 1
        Loop

        'Construct the parameter
        myString = Utilities.FixStringLength("", 3050) & _
                 Utilities.FixStringLength(stadium0, 2) & _
                 Utilities.FixStringLength(stadium1, 2) & _
                 Utilities.FixStringLength(stadium2, 2) & _
                 Utilities.FixStringLength(stadium3, 2) & _
                 Utilities.FixStringLength(stadium4, 2) & _
                 Utilities.FixStringLength(stadium5, 2) & _
                 Utilities.PadLeadingZeros(De.ReservationPeriod, "6") & _
                 Utilities.FixStringLength(De.Src, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

#End Region

#Region "Verify And Update Basket"

    Private Function AccessDatabaseWS125R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        'Create the Status data table
        Dim DtBasketIdResults As New DataTable
        ResultDataSet.Tables.Add(DtBasketIdResults)
        With DtBasketIdResults.Columns
            .Add("BasketId", GetType(String))
            .Add("Mode", GetType(String))
            .Add("ExternalOrderNumber", GetType(String))
        End With
        Try

            'Call WS125R
            PARAMOUT = CallWS125R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            If PARAMOUT.Substring(1023, 1) <> "E" And PARAMOUT.Substring(1021, 2).Trim = "" Then
                Dim iPosition As Integer = 0
                dRow = Nothing
                dRow = DtBasketIdResults.NewRow
                dRow("BasketId") = PARAMOUT.Substring(iPosition, 36)
                dRow("Mode") = PARAMOUT.Substring(iPosition + 36, 1)
                dRow("ExternalOrderNumber") = PARAMOUT.Substring(iPosition + 37, 30).Trim
                DtBasketIdResults.Rows.Add(dRow)
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBRS-WS125R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS125R() As String
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS125R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS125Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        'TalentCommonLog("CallWS125R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        'TalentCommonLog("CallWS125R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS125Parm() As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(Dep.BasketId, 36) & _
                 Utilities.FixStringLength(Dep.BasketVerificationMode, 1) & _
                 Utilities.FixStringLength(Dep.ExternalOrderNumber, 30) & _
                 Utilities.FixStringLength("", 954) & _
                 Utilities.FixStringLength("", 2) & _
                 Utilities.FixStringLength("", 1)

        Return myString

    End Function

#End Region

#Region "AddRetailToTicketingBasket"

    Private Function AccessDatabaseRT001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("Error Status")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()
            Dim cmdAdapter As iDB2DataAdapter = Nothing

            cmd = conTALENTTKT.CreateCommand()
            cmd.CommandText = "CALL RT001S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5,@PARAM6)"
            cmd.CommandType = CommandType.Text

            Dim pErrorCode As New iDB2Parameter
            Dim pRetailProducts As iDB2Parameter
            Dim pBasketId As iDB2Parameter
            Dim pTempOrderId As iDB2Parameter
            Dim pDeleteWS003 As New iDB2Parameter
            Dim pCustomerNumber As iDB2Parameter
            Dim pPaymentTaken As iDB2Parameter
            Dim drStatus As DataRow

            pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
            pErrorCode.Direction = ParameterDirection.InputOutput
            pRetailProducts = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 20000)
            pBasketId = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 36)
            pTempOrderId = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 100)
            pDeleteWS003 = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1)
            pCustomerNumber = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 12)
            pPaymentTaken = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Decimal, 1)

            pErrorCode.Value = String.Empty
            pBasketId.Value = DERetailToTicketing.BasketId
            pTempOrderId.Value = DERetailToTicketing.TempOrderId
            pDeleteWS003.Value = "Y"
            pCustomerNumber.Value = Settings.LoginId
            pPaymentTaken.Value = Utilities.ConvertToYN(DERetailToTicketing.PaymentTaken)

            Dim retailProductString As New StringBuilder
            Dim loopCount As Integer = 0
            For Each retailProducts As DERetailToTicketingProduct In DERetailToTicketing.ListOfDERetailToTicketingProducts
                retailProductString.Append("R")
                retailProductString.Append(Utilities.PadLeadingZeros(retailProducts.Quantity.ToString, 5))
                retailProductString.Append(Utilities.PadLeadingZeros(retailProducts.LineNumber.ToString, 6))
                retailProductString.Append(Utilities.PadLeadingZeros(CType((retailProducts.Price * 100), Integer).ToString, 9))
                retailProductString.Append(Utilities.FixStringLength("", 29))

                ' We can handle 400 retail products but we need to save the last two places for the delivery and discount
                loopCount += 1
                If loopCount >= 398 Then
                    pRetailProducts.Value = retailProductString.ToString
                    cmd.ExecuteNonQuery()

                    'Leave the subroutine if we have an error
                    If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
                        drStatus = DtStatusResults.NewRow
                        drStatus("ErrorOccurred") = CStr(cmd.Parameters(0).Value).Trim
                        drStatus("ReturnCode") = "E"
                        DtStatusResults.Rows.Add(drStatus)
                        Return err
                    End If

                    'We can now add the other retail products
                    retailProductString.Clear()
                    loopCount = 0
                    pDeleteWS003.Value = "N"
                End If

            Next

            'Add delivery value.
            If DERetailToTicketing.DeliveryPrice > 0 Then
                retailProductString.Append("D")
                retailProductString.Append(Utilities.PadLeadingZeros("1", 5))
                retailProductString.Append(Utilities.PadLeadingZeros("1", 6))
                retailProductString.Append(Utilities.PadLeadingZeros(CType((DERetailToTicketing.DeliveryPrice * 100), Integer).ToString, 9))
                retailProductString.Append(Utilities.FixStringLength("", 29))
            End If

            'Add promotion value.
            If DERetailToTicketing.PromotionPrice > 0 Then
                retailProductString.Append("P")
                retailProductString.Append(Utilities.PadLeadingZeros("1", 5))
                retailProductString.Append(Utilities.PadLeadingZeros("1", 6))
                retailProductString.Append(Utilities.PadLeadingZeros(CType((DERetailToTicketing.PromotionPrice * 100), Integer).ToString, 9))
                retailProductString.Append(Utilities.FixStringLength("", 29))
            End If

            pRetailProducts.Value = retailProductString.ToString
            cmd.ExecuteNonQuery()

            drStatus = DtStatusResults.NewRow
            If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = "E"
                drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If
            DtStatusResults.Rows.Add(drStatus)

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPD-RT001S"
                .HasError = True
            End With
        End Try

        Return err


    End Function

#End Region

#Region "GetBasketExceptionSeats"

    ''' <summary>
    ''' Functionality for retrieving basket season ticket exception seats stored in WS006
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS006S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the basket detail exceptions table
        Dim dtBasketDetailExceptions As New DataTable("BasketDetailExceptions")
        ResultDataSet.Tables.Add(dtBasketDetailExceptions)

        Try
            CallWS006S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBAmendBasket-WS006S"
                .HasError = True
                BasketHasExceptionSeats = False
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallWS006S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS006S(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pBasketId As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.Output
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pBasketId = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 36)

        pSource.Value = _basketFunctions.Source
        pErrorCode.Value = String.Empty
        pBasketId.Value = _basketFunctions.SessionId

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "BasketDetailExceptions")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#End Region

#End Region

End Class