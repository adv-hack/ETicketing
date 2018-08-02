Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Alerts
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBPA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBProductAlert
    Inherits DBAccess

    Private _dep As DEProductAlert
    Private alert_ID As String = String.Empty
    Private Dt As DataTable
    Public Property Dep() As DEProductAlert
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProductAlert)
            _dep = value
        End Set
    End Property

    Public Function ProductAlerts() As ErrorObj
        Dim err As New ErrorObj
        err = Sql2005Open()
        If Not err.HasError Then err = AccessDataBaseSQL2005()
        err = Sql2005Close()
        '
        Return err
    End Function
    Public Function ProductAlertsOutbound() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------------
        '   Get any Product Alert Requests for the company selected
        '
        err = Sql2005Open()
        If Not err.HasError Then err = ReadProductAlertsSQL2005()
        '------------------------------------------------------------------------------------
        '   For Product Alert Requests found check on System21 if stock available
        '
        If Not err.HasError Then
            err = System21Open()
            If Not err.HasError Then err = ValidateAgainstDatabaseSystem21()
            System21Close()
        End If
        If Not err.HasError Then err = UpdateProductAlertsSQL2005()
        err = Sql2005Close()
        '
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------
        Try
            Dim iCounter As Integer = 0
            Dim dea As DEAlerts                     ' Items
            '------------------------------------------------------------------------
            '   Execute  
            '
            Const SQLInsert As String = "INSERT INTO  tbl_product_alerts " & _
                                        "( COMPANY, PRODUCT_CODE, EMAIL_ADDRESS, FIRST_NAME, LAST_NAME, EXPIRY_DATE, ALERTED ) " & _
                                        " VALUES ( @PARAM1, @PARAM2, @PARAM3,  @PARAM4, @PARAM5, @PARAM6, 0  )"
            Dim cmdInsert As SqlCommand = Nothing
            '
            For iCounter = 1 To Dep.CollDEAlerts.Count
                dea = Dep.CollDEAlerts.Item(iCounter)
                '     
                ' If dea.Status = "Y" Then
                cmdInsert = New SqlCommand(SQLInsert, conSql2005)
                With cmdInsert
                    With .Parameters
                        .Add(New SqlParameter(Param1, SqlDbType.Char, 20)).Value = Settings.Company.Trim
                        .Add(New SqlParameter(Param2, SqlDbType.Char, 50)).Value = dea.ProductCode.Trim
                        .Add(New SqlParameter(Param3, SqlDbType.Char, 100)).Value = dea.EMailAddress.Trim
                        .Add(New SqlParameter(Param4, SqlDbType.Char, 20)).Value = dea.FirstName.Trim
                        .Add(New SqlParameter(Param5, SqlDbType.Char, 20)).Value = dea.LastName.Trim
                        .Add(New SqlParameter(Param6, SqlDbType.DateTime)).Value = dea.ExpiryDate
                    End With
                    .ExecuteNonQuery()
                End With
                cmdInsert = Nothing
                '  End If
            Next iCounter
            '--------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPA-02"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Function ReadProductAlertsSQL2005() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------
        Try
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim iCounter As Integer = 0
            '------------------------------------------------------------------------
            '   Execute , order by email addressso we can split out into indiviual emails
            '   at a later stage
            '
            Const SQLString1 As String = "SELECT A.*,  'xxx' as PRODUCT_DESCRIPTION_1  " & _
                                        " FROM tbl_product_alerts as A WITH (NOLOCK)   " & _
                                        " WHERE A.COMPANY = @PARAM1 AND A.ALERTED = 0 " & _
                                        " ORDER BY A.EMAIL_ADDRESS "
            '
            If Not err.HasError Then
                cmdSelect = New SqlCommand(SQLString1, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 20)).Value = Settings.Company
                    dtrProducts = .ExecuteReader()
                End With
                If Not dtrProducts.HasRows Then
                    With err
                        .ErrorMessage = Settings.Company
                        .ErrorStatus = "No Product Alerts found for " & Settings.Company
                        .ErrorNumber = "TACDBPA-35"
                        .HasError = True
                    End With
                Else
                    '   Load the Datareader into the DataSet to return
                    Dt = New DataTable
                    Dt.Load(dtrProducts)
                    dtrProducts.Close()
                End If

            End If
            dtrProducts.Close()
            '--------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPA-36"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Private Function UpdateProductAlertsSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Try
            '------------------------------------------------------------------------
            '   Execute  
            '   Alert_ID has an array of codes seperated by commas
            '   '1010101' deals with last comma on Alert_ID
            '
            If alert_ID.Length > 5 Then

                Dim SQLUpdate As String = " UPDATE tbl_product_alerts       " & _
                                            " SET  ALERTED = -1,            " & _
                                            "      ALERTED_DATE = @PARAM1   " & _
                                            " WHERE (ALERT_ID IN (@PARAM2)) "
                '
                SQLUpdate = SQLUpdate.Replace(Param2, alert_ID & "000")
                Dim cmdSelect = New SqlCommand(SQLUpdate, conSql2005)
                cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.DateTime)).Value = Now
                cmdSelect.ExecuteNonQuery()
                cmdSelect = Nothing
            End If
            '--------------------------------------------------------------------
        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPA-02"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
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
                cmdSelect = New SqlCommand(SQLString1, conSql2005)
                cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = dea.ProductCode
                dtrProducts = cmdSelect.ExecuteReader()
                If Not dtrProducts.HasRows Then
                    With err
                        .ErrorMessage = dea.ProductCode
                        .ErrorStatus = "Product " & dea.ProductCode & " not found"
                        .ErrorNumber = "TACDBPA-25"
                        '.HasError = True
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
                .ErrorNumber = "TACDBPA-26"
                .HasError = True
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function
    Protected Overrides Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------------
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrProducts As SqlDataReader = Nothing
        Dim paraminput As iDB2Parameter
        Dim paramoutput As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        ' Const SQLString1 As String = "CALL WESTCOAST/STOCKENQ(@PARAM1, @PARAM2) "
        Dim SQLString1 As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                   "/STOCKENQ(@PARAM1, @PARAM2) "
        '
        Dim Dr As DataRow = Nothing
        If Not err.HasError Then
            Try
                '-------------------------------------------------------------
                alert_ID = String.Empty
                '            Dt = New DataTable
                Dt.Columns.Add("QUANTITY", GetType(Double))
                ' 
                For Each Dr In Dt.Rows
                    cmdSelect = New iDB2Command(SQLString1, conSystem21)
                    With cmdSelect
                        paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                            Utilities.FixStringLength(String.Format("{0,-15}", Dr("PRODUCT_CODE")), 30) & _
                                            Utilities.FixStringLength(Settings.AccountNo4, 10)
                        paraminput.Direction = ParameterDirection.Input
                        paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        paramoutput.Value = Param2
                        paramoutput.Direction = ParameterDirection.InputOutput
                        .ExecuteNonQuery()
                        '--------------------------------------------------------------------
                        PARAMOUT = .Parameters(Param2).Value.ToString
                        With PARAMOUT
                            If .Substring(1023, 1) = "Y" Then
                                Dr("QUANTITY") = 0
                            Else
                                Dr("QUANTITY") = CType(.Substring(0, 10) & "." & .Substring(11, 3), Double)
                                alert_ID &= Dr("ALERT_ID") & ", "
                            End If
                        End With
                        .Dispose()
                    End With
                Next
                ResultDataSet = New DataSet
                ResultDataSet.Tables.Add(Dt)
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBPA-62"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

End Class
