Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Price Requests
'
'       Date                        Dec 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBST- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBStock
    Inherits DBAccess

    Private _dep As New DePNA

    Public Property Dep() As DePNA
        Get
            Return _dep
        End Get
        Set(ByVal value As DePNA)
            _dep = value
        End Set
    End Property

    Protected Overrides Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            ' Const SQLString As String = "CALL WESTCOAST/STOCKENQ(@PARAM1, @PARAM2) "
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                      "/STOCKENQ(@PARAM1, @PARAM2) "
            Try
                '-------------------------------------------------------------
                '   Execute
                '
                cmdSelect = New iDB2Command(SQLString, conSystem21)
                With cmdSelect
                    paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                        Utilities.FixStringLength(String.Format("{0,-15}", Dep.SKU), 30) & _
                                        Utilities.FixStringLength(Settings.AccountNo4, 10)
                    paraminput.Direction = ParameterDirection.Input
                    paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    paramoutput.Value = Param2
                    paramoutput.Direction = ParameterDirection.InputOutput
                    .ExecuteNonQuery()
                    '--------------------------------------------------------------------
                    PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                    If PARAMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ErrorMessage = PARAMOUT
                            .ErrorNumber = PARAMOUT.Substring(1019, 4)
                            .ErrorStatus = "Error pricing item - " & _
                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                            .HasError = True
                        End With
                    Else
                        ' Build the response DataSet
                        Dim quantity As Double = CType(PARAMOUT.Substring(0, 10) & "." & PARAMOUT.Substring(11, 3), Double)

                        ResultDataSet = New DataSet
                        Dim dt As New DataTable
                        dt.Columns.Add("Item", GetType(String))
                        dt.Columns.Add("Quantity", GetType(Double))
                        dt.Rows.Add(Dep.SKU, quantity)
                        ResultDataSet.Tables.Add(dt)
                    End If
                    .Dispose()
                End With
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBST-08"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overrides Function ValidateAgainstDatabaseChorus() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        ' '' ''If Not err.HasError Then
        ' '' ''    Dim PARAMOUT As String = String.Empty
        ' '' ''    Dim cmdSelect As iDB2Command = Nothing
        ' '' ''    Dim dtrProducts As SqlDataReader = Nothing
        ' '' ''    Dim paraminput As iDB2Parameter
        ' '' ''    Dim paramoutput As iDB2Parameter
        ' '' ''    ' Const SQLString As String = "CALL WESTCOAST/STOCKENQ(@PARAM1, @PARAM2) "
        ' '' ''    Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
        ' '' ''                              "/STOCKENQ(@PARAM1, @PARAM2) "
        ' '' ''    Try
        ' '' ''        '-------------------------------------------------------------
        ' '' ''        '   Execute
        ' '' ''        '
        ' '' ''        cmdSelect = New iDB2Command(SQLString, conSystem21)
        ' '' ''        With cmdSelect
        ' '' ''            paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        ' '' ''            paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
        ' '' ''                                Utilities.FixStringLength(String.Format("{0,-15}", Dep.SKU), 30) & _
        ' '' ''                                Utilities.FixStringLength(Settings.AccountNo4, 10)
        ' '' ''            paraminput.Direction = ParameterDirection.Input
        ' '' ''            paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
        ' '' ''            paramoutput.Value = Param2
        ' '' ''            paramoutput.Direction = ParameterDirection.InputOutput
        ' '' ''            .ExecuteNonQuery()
        ' '' ''            '--------------------------------------------------------------------
        ' '' ''            PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
        ' '' ''            If PARAMOUT.Substring(1023, 1) = "Y" Then
        ' '' ''                With err
        ' '' ''                    .ErrorMessage = PARAMOUT
        ' '' ''                    .ErrorNumber = PARAMOUT.Substring(1019, 4)
        ' '' ''                    .ErrorStatus = "Error pricing item - " & _
        ' '' ''                                Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
        ' '' ''                    .HasError = True
        ' '' ''                End With
        ' '' ''            Else
        ' '' ''                ' Build the response DataSet
        ' '' ''                Dim quantity As Double = CType(PARAMOUT.Substring(0, 10) & "." & PARAMOUT.Substring(11, 3), Double)

        ' '' ''                ResultDataSet = New DataSet
        ' '' ''                Dim dt As New DataTable
        ' '' ''                dt.Columns.Add("Item", GetType(String))
        ' '' ''                dt.Columns.Add("Quantity", GetType(Double))
        ' '' ''                dt.Rows.Add(Dep.SKU, quantity)
        ' '' ''                ResultDataSet.Tables.Add(dt)
        ' '' ''            End If
        ' '' ''            .Dispose()
        ' '' ''        End With
        ' '' ''        '--------------------------------------------------------------------
        ' '' ''    Catch ex As Exception
        ' '' ''        Const strError8 As String = "Error during database access "
        ' '' ''        With err
        ' '' ''            .ErrorMessage = ex.Message
        ' '' ''            .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
        ' '' ''            .ErrorNumber = "TACDBST-08"
        ' '' ''            .HasError = True
        ' '' ''        End With
        ' '' ''    End Try
        ' '' ''End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        '   Execute
        '
        Const SQLString As String = "SELECT * FROM tbl_product WITH (NOLOCK)  WHERE PRODUCT_CODE = @PARAM1"
        Dim cmdSelect As SqlCommand = Nothing
        Dim dtrProducts As SqlDataReader = Nothing
        '
        If Not err.HasError Then
            Try
                cmdSelect = New SqlCommand(SQLString, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 50))
                    .Parameters(Param1).Value = Dep.SKU.Trim
                    dtrProducts = .ExecuteReader()
                End With

                If Not dtrProducts.HasRows Then
                    With err
                        .ErrorMessage = String.Empty
                        .ErrorStatus = "Product " & Dep.SKU & " not found"
                        .ErrorNumber = "TACDBST13"
                        .HasError = True
                    End With
                    dtrProducts.Close()
                Else
                    '   Load the Datareader into the DataSet to return
                    Dim dt As New DataTable
                    dt.Load(dtrProducts)
                    ResultDataSet.Tables.Add(dt)
                    dtrProducts.Close()
                End If
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBST11"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

End Class
