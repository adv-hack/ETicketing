Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with  Availability Responses
'
'       Date                        5th Jan 2007
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBAR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBAvailability
    Inherits DBAccess
    Private _dep As DEProductAlert
    Public Property Dep() As DEProductAlert
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProductAlert)
            _dep = value
        End Set
    End Property

    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------
        If Not err.HasError Then
            Try
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtrProducts As SqlDataReader = Nothing
                Dim iCounter As Integer = 0
                Dim dea As DEAlerts                     ' Items
                '
                Const SQLString1 As String = " SELECT PRODUCT_DESCRIPTION_1 FROM tbl_product WITH (NOLOCK)  " & _
                                                " WHERE PRODUCT_CODE = @PARAM1"
                '------------------------------------------------------------------------
                '   Execute  
                '
                For iCounter = 1 To Dep.CollDEAlerts.Count
                    dea = Dep.CollDEAlerts.Item(iCounter)
                    '
                    cmdSelect = New SqlCommand(SQLString1, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = dea.ProductCode
                        dtrProducts = .ExecuteReader()
                    End With
                    If Not dtrProducts.HasRows Then
                        With err
                            .ItemErrorMessage(iCounter) = String.Empty
                            .ItemErrorStatus(iCounter) = "Error pricing item - " & dea.ProductCode
                            .ItemErrorCode(iCounter) = "TACDBAR-25"
                            dea.Status = "N"
                            dea.Description = .ErrorStatus
                        End With
                    Else
                        dtrProducts.Read()
                        dea.Status = "Y"
                        dea.Description = dtrProducts("PRODUCT_DESCRIPTION_1").Trim
                    End If
                    dtrProducts.Close()
                Next
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBAR-26"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Overrides Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim PARAMOUT As String = String.Empty
            '
            '   Const SQLString1 As String = "CALL WESTCOAST/STOCKENQ(@PARAM1, @PARAM2) "
            Dim SQLString1 As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                            "/STOCKENQ(@PARAM1, @PARAM2) "
            Try
                '-------------------------------------------------------------
                '   Execute
                '
                For iCounter = 1 To Dep.CollDEAlerts.Count
                    dea = Dep.CollDEAlerts.Item(iCounter)
                    cmdSelect = New iDB2Command(SQLString1, conSystem21)
                    With cmdSelect
                        paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                            Utilities.FixStringLength(String.Format("{0,-15}", dea.ProductCode), 30) & _
                                            Utilities.FixStringLength(Settings.AccountNo4, 10)
                        paraminput.Direction = ParameterDirection.Input
                        paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char).Value = Param2
                        paramoutput.Direction = ParameterDirection.InputOutput
                        .ExecuteNonQuery()
                        '-----------------------------------------------------
                        PARAMOUT = .Parameters(Param2).Value.ToString
                        With PARAMOUT
                            If PARAMOUT.Substring(1023, 1) = "Y" Then
                                dea.Quantity = 0
                                With err
                                    .ItemErrorMessage(iCounter) = PARAMOUT
                                    .ItemErrorStatus(iCounter) = "Error checking item - " & dea.ProductCode
                                    .ItemErrorCode(iCounter) = PARAMOUT.Substring(1019, 4)
                                End With
                            Else
                                dea.Quantity = CType(.Substring(0, 10) & "." & .Substring(11, 3), Double)
                                dea.Description = PARAMOUT
                            End If
                        End With
                        .Dispose()
                    End With
                Next
                '------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBAR-62"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

End Class
