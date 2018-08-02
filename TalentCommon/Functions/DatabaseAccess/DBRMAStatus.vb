Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with RMA Status Requests
'
'       Date                        10th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBRM- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBRMAStatus
    Inherits DBAccess

    Private _dep As DEOrder
    Private _parmTRAN As String
    Private _parmITEM(10) As String

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
    Private Property ParmITEM(ByVal order As Integer) As String
        Get
            Return _parmITEM(order)
        End Get
        Set(ByVal value As String)
            _parmITEM(order) = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim iItem As Integer = 0
        Dim cmdSELECT As iDB2Command = Nothing
        Dim parmTRANS, parmITEMS, Paramoutput As iDB2Parameter
        ' Const strHEADER As String = "CALL WESTCOAST.RMASTATUS(@PARAM1, @PARAM2, @PARAM3)"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/RMASTATUS(@PARAM1, @PARAM2, @PARAM3)"
        '
        If Not err.HasError Then
            Try
                For iItem = 1 To 500
                    If ParmITEM(iItem).Length > 0 Then
                        cmdSELECT = New iDB2Command(strHEADER, conSystem21)
                        '------------------------------------------------------------------------------  
                        parmTRANS = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        parmTRANS.Value = ParmTRAN
                        parmTRANS.Direction = ParameterDirection.Input
                        '------------------------------------------------------------------------------
                        parmITEMS = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        parmITEMS.Value = ParmITEM(iItem)
                        parmITEMS.Direction = ParameterDirection.Input
                        '------------------------------------------------------------------------------
                        Paramoutput = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                        Paramoutput.Value = "Outgoing"
                        Paramoutput.Direction = ParameterDirection.Output
                        cmdSELECT.ExecuteNonQuery()
                    Else
                        Exit For
                    End If
                Next iItem
                '----------------------------------------------------------------
            Catch ex As Exception
                Const strError2 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError2
                    .ErrorNumber = "TACDBRM-02"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
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
        Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
        '
        Try
            With Dep
                detr = Dep.CollDETrans.Item(1)
                deos = Dep.CollDEOrders.Item(1)
            End With
            deoh = deos.DEOrderHeader
            '---------------------------------------------------------------------------------------------
            Const sTran As String = "SenderID = {0}, ReceiverID = {1}, CountryCode = {2}, TransactionID = {3}, ShowDetail = {4}"
            With detr
                ParmTRAN = String.Format(sTran, _
                                    .SenderID, _
                                    .ReceiverID, _
                                    .CountryCode, _
                                    .TransactionID, _
                                    .ShowDetail)
            End With
            '----------------------------------------------------------------------------------
            Const sItem As String = "BranchOrderNumber = {0}, OrderSuffix = {1}, CustomerPO = {2}"
            ParmITEM(1) = String.Format(sItem, _
                                    deoh.BranchOrderNumber, _
                                    deoh.OrderSuffix, _
                                    deoh.CustomerPO)

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & " Error"
                .ErrorNumber = "TACDBRM-99"
                .HasError = True
            End With
        End Try
        Return err
    End Function

End Class
