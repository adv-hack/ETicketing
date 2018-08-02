Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with D Type Order Requests
'
'       Date                        7th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBTO- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBDTypeOrder
    Inherits DBAccess

    Private _dep As New DEOrder

    Private _parmTRAN As String
    Private _parmHEAD(10) As String
    Private _parmADDR(10, 10) As String
    Private _parmITEM(10, 100) As String
    Private _parmCOMM(10, 100) As String

    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
            _dep = value
        End Set
    End Property
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
    Private Property ParmADDR(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmADDR(order, index)
        End Get
        Set(ByVal value As String)
            _parmADDR(order, index) = value
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
    Private Property ParmCOMM(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmCOMM(order, index)
        End Get
        Set(ByVal value As String)
            _parmCOMM(order, index) = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        Dim conTalent As iDB2Connection = Nothing
        '----------------------------------------------------------------------------------
        '   Open up database
        '
        err = DataEntityUnPack()
        If Not err.HasError Then
            Try
                conTalent = New iDB2Connection(Settings.BackOfficeConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TACDBTO-01"
                    .HasError = True
                End With
            End Try
            '----------------------------------------------------------------------------------
            Dim sProcError As String = String.Empty
            Dim iOrder As Integer = 0
            Dim iAddress As Integer = 0
            Dim iItems As Integer = 0
            Dim iComments As Integer = 0
            '
            If Not err.HasError Then
                Try
                    '
                    ' Const strSql As String = "CALL WESTCOAST.ORDERINSERT(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6)"
                    Dim strSql As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                        "/ORDERINSERT(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6)"
                    Dim cmdSelect As iDB2Command = Nothing
                    cmdSelect = New iDB2Command(strSql, conTalent)
                    '
                    Dim parmTRANRANSACTION, parmHEADER, parmADDRESS, _
                        parmITEMLINE, parmCOMMENT, Paramoutput As iDB2Parameter
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
                    '----------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TACDBTO-02"
                        .HasError = True
                    End With
                End Try
            End If
            '--------------------------------------------------------------------
            '   Close
            '
            Try
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TACDBTO-03"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function

    Private Function DataEntityUnPack() As ErrorObj
        Const ModuleName As String = "DataEntityUnPack"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim detr As New DETransaction           ' Items
        Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
        '-------------------------------------------------------------------------------------
        Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
        Dim dead As New DeAddress               ' Items
        '
        Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
        Dim depr As DeProductLines              ' Items
        Dim decl As DeCommentLines              ' Items

        Dim iOrder As Integer = 0
        Dim iAddress As Integer = 0
        Dim iItems As Integer = 0
        Dim iComments As Integer = 0
        '
        Try
            With Dep
                '
                detr = .CollDETrans.Item(1)
                Const sTran As String = "SenderID = {0}, ReceiverID = {1}, CountryCode = {2}, TransactionID = {3}, ShowDetail = {4}"
                With detr
                    ParmTRAN = String.Format(sTran, _
                                                .SenderID, _
                                                .ReceiverID, _
                                                .CountryCode, _
                                                .TransactionID, _
                                                .ShowDetail)
                End With

                '---------------------------------------------------------------------------------------------
                For iOrder = 1 To .CollDEOrders.Count
                    '
                    deos = .CollDEOrders.Item(iOrder)
                    deoh = deos.DEOrderHeader
                    '-----------------------------------------------------------------------------------------
                    With deoh
                        ParmHEAD(iOrder) = String.Format("CustomerPO = {0}, EndUserPO = {2}, CarrierCode = {3}, BackOrderFlag = {4}, ShipFromBranches = {5}", _
                                                .CustomerPO, _
                                                .EndUserPO, _
                                                .CarrierCode, _
                                                .BackOrderFlag, _
                                                .ShipFromBranches)
                        '-------------------------------------------------------------------------------------
                        '   May have one or more adresses need to check and act accordingly
                        '
                        For iAddress = 1 To .CollDEAddress.Count
                            dead = .CollDEAddress.Item(iAddress)
                            With dead
                                Select Case .Category
                                    Case Is = "ShipTo"
                                        ParmADDR(iOrder, iAddress) = String.Format("Category = {0}" & _
                                                        ", ContactName = {1}, Contact = {2}" & _
                                                        ", Line1 = {3}, Line2 = {4}" & _
                                                        ", Line3 = {5}, City = {6}" & _
                                                        ", Province = {7}, PostalCode = {8}", _
                                                        .Category, .ContactName, .ContactName, _
                                                        .Line1, .Line2, .Line3, .City, .Province, .PostalCode)

                                    Case Is = "EndUserInformation"
                                        ParmADDR(iOrder, iAddress) = String.Format("Category = {0}" & _
                                                        ", ContactName = {1}, PhoneNumber = {2}" & _
                                                        ", ExtensionNumber = {3} , FaxNumber = {4}" & _
                                                        ", Line1 = {5}, Line2 = {6}" & _
                                                        ", City = {7}, Province = {8}" & _
                                                        ", PostalCode = {9},CountryCode  = {10}", _
                                                        ", CompanyName = {11},VATNumber  = {12}", _
                                                        ", AuthorizationNumber = {13},PricingLevel  = {14}", _
                                                        ", EmailAddress = {15}", _
                                                        .Category, .ContactName, .PhoneNumber, _
                                                        .ExtensionNumber, .FaxNumber, _
                                                        .Line1, .Line2, .City, .Province, .PostalCode, _
                                                        .CountryCode, .CompanyName, .VATNumber, _
                                                        .AuthorizationNumber, .PricingLevel, .Email)

                                    Case Is = "ResellerInformation"
                                        ParmADDR(iOrder, iAddress) = String.Format("Category = {0}" & _
                                                        ", RepName = {1}, ContactName = {2} " & _
                                                        ", PhoneNumber = {3}, FaxNumber = {4} " & _
                                                        ", Line1 = {5}, Line2 = {6}" & _
                                                        ", City = {7}, Province = {8}" & _
                                                        ", PostalCode = {9}, CompanyName = {10}", _
                                                        ", EmailAddress = {11} ", _
                                                        .Category, .ContactName, .PhoneNumber, _
                                                        .ExtensionNumber, .FaxNumber, _
                                                        .Line1, .Line2, .City, .Province, .PostalCode, _
                                                        .CompanyName, .Email)

                                End Select
                            End With
                        Next iAddress
                    End With
                    '---------------------------------------------------------------------------------------------
                    '   May have one or more items and comments
                    '
                    deoi = deos.DEOrderInfo
                    With deoi
                        For iItems = 1 To .CollDEProductLines.Count
                            depr = .CollDEProductLines.Item(iItems)
                            With depr
                                ParmITEM(iOrder, iItems) = String.Format("SKU ={0}, Quantity = {1}, CustomerLineNumber = {2}", _
                                                 .SKU, _
                                                 .Quantity, _
                                                 .CustomerLineNumber)

                            End With
                        Next
                        '-----------------------------------------------------------------------------------------
                        '   May have one or more comments 
                        '
                        For iComments = 1 To .CollDECommentLines.Count
                            decl = .CollDECommentLines.Item(iComments)
                            With decl
                                ParmCOMM(iOrder, iComments) = String.Format( _
                                                " CommentText = {0}, CommentLine = {1}", _
                                                .CommentText, _
                                                .CommentLine)

                            End With
                        Next iComments
                    End With
                    '--------------------------------------------------------------------------------------------
                Next iOrder
            End With
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & " Error"
                .ErrorNumber = "TACDBTO-99"
                .HasError = True
            End With
        End Try
        Return err
    End Function

End Class
