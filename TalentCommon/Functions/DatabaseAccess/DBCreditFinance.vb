Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Credit Finance 
'
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBCreditFinance
    Inherits DBAccess

    Private _de As New DECreditFinance

    Private Const CreditFinanceOptions As String = "CreditFinanceOptions"
    Private Const CreditFinanceOptionDetails As String = "CreditFinanceOptionDetails"

    Public Property De() As DECreditFinance
        Get
            Return _de
        End Get
        Set(ByVal value As DECreditFinance)
            _de = value
        End Set
    End Property
 
    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        If Not Me.conSql2005.State = ConnectionState.Open Then
            Me.Sql2005Open()
        End If

        If Me.conSql2005.State = ConnectionState.Open Then

            Select Case _settings.ModuleName
                Case Is = CreditFinanceOptions : err = GetCreditFinanceOptions(De.CreditFinanceCompanyCode)
                Case Is = CreditFinanceOptionDetails : err = GetCreditFinanceOptionDetails()

            End Select

            If Me.conSql2005.State <> ConnectionState.Closed Then
                Me.Sql2005Close()
            End If

        End If

        Return err

    End Function

    Private Function GetCreditFinanceOptions(ByVal CompanyCode As String) As ErrorObj
        Dim err As New ErrorObj

        Me.ResultDataSet = New DataSet
        Dim DtCreditFinanceOptions As New DataTable("CreditFinanceOptions")
        Me.ResultDataSet.Tables.Add(DtCreditFinanceOptions)
        AddCreditFinanceColumns(DtCreditFinanceOptions)

        Const SQL As String = "SELECT PaymentOptionCode, NumberOfPayments, ShortDescription, Example FROM tbl_credit_finance_options " & _
                              "WHERE FinanceCompanyCode = @FinanceCompanyCode " & _
                              "AND Active = 1 " & _
                              "ORDER BY OrderBy ASC"

        Dim cmd As New SqlCommand(SQL, conSql2005)

        With cmd.Parameters
            .Clear()
            .Add("@FinanceCompanyCode", SqlDbType.NVarChar).Value = CompanyCode
        End With

        Try
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet()
            da.Fill(ds)

            If ds.Tables.Count > 0 Then
                Dim dt As DataTable = ds.Tables(0)
                Dim strOnlyAvailablePlan As String = String.Empty
                If De.OnlyAvailablePlan IsNot Nothing Then
                    strOnlyAvailablePlan = De.OnlyAvailablePlan
                End If


                For Each dr As DataRow In dt.Rows
                    Dim FeRow As DataRow = DtCreditFinanceOptions.NewRow()
                    FeRow.Item("PaymentOptionCode") = dr.Item("PaymentOptionCode")
                    FeRow.Item("NumberOfPayments") = dr.Item("NumberOfPayments")
                    FeRow.Item("ShortDescription") = dr.Item("ShortDescription")
                    FeRow.Item("Example") = dr.Item("Example")
                    If strOnlyAvailablePlan = String.Empty Or Trim(strOnlyAvailablePlan) = Trim(dr.Item("PaymentOptionCode")) Then
                        DtCreditFinanceOptions.Rows.Add(FeRow)
                    End If

                Next

            End If

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error Retrieving Credit Finance Details"
                .ErrorNumber = "TACDBCF-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function
    Private Function GetCreditFinanceOptionDetails() As ErrorObj
        Dim err As New ErrorObj

        Me.ResultDataSet = New DataSet
        Dim DtCreditFinanceOptions As New DataTable("CreditFinanceOptions")
        Me.ResultDataSet.Tables.Add(DtCreditFinanceOptions)
        AddCreditFinanceColumns(DtCreditFinanceOptions)

        Const SQL As String = "SELECT PaymentOptionCode, NumberOfPayments, ShortDescription, Example FROM tbl_credit_finance_options " & _
                              "WHERE FinanceCompanyCode = @FinanceCompanyCode " & _
                              "AND PaymentOptionCode = @PaymentOptionCode " & _
                              "AND Active = 1 "

        Dim cmd As New SqlCommand(SQL, conSql2005)

        With cmd.Parameters
            .Clear()
            .Add("@FinanceCompanyCode", SqlDbType.NVarChar).Value = De.CreditFinanceCompanyCode
            .Add("@PaymentOptionCode", SqlDbType.NVarChar).Value = De.CreditFinanceOptionCode
        End With

        Try
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet()
            da.Fill(ds)

            If ds.Tables.Count > 0 Then
                Dim dt As DataTable = ds.Tables(0)

                For Each dr As DataRow In dt.Rows
                    Dim FeRow As DataRow = DtCreditFinanceOptions.NewRow()
                    FeRow.Item("PaymentOptionCode") = dr.Item("PaymentOptionCode")
                    FeRow.Item("NumberOfPayments") = dr.Item("NumberOfPayments")
                    FeRow.Item("ShortDescription") = dr.Item("ShortDescription")
                    FeRow.Item("Example") = dr.Item("Example")
                    DtCreditFinanceOptions.Rows.Add(FeRow)
                Next

            End If

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error Retrieving Credit Finance Specific Option Details"
                .ErrorNumber = "TACDBCF-02"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Sub AddCreditFinanceColumns(ByRef dtCreditFinance As DataTable)

        With dtCreditFinance.Columns
            .Add("PaymentOptionCode", GetType(String))
            .Add("NumberOfPayments", GetType(String))
            .Add("ShortDescription", GetType(String))
            .Add("Example", GetType(String))
        End With

    End Sub

End Class

