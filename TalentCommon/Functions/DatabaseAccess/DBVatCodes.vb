Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBVatCodes
    Inherits DBAccess

    Private Const RetrieveVatCodes As String = "RetrieveVatCodes"

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case Settings.ModuleName
            Case Is = RetrieveVatCodes : err = AccessDatabaseEM006S()
        End Select

        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function AccessDatabaseEM006S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtVATCodeListResults As New DataTable("VATCodeListResults")
        ResultDataSet.Tables.Add(DtVATCodeListResults)
        With DtVATCodeListResults.Columns
            .Add("VATUniqueID", GetType(Double))
            .Add("VATCode", GetType(String))
            .Add("VATRate", GetType(Decimal))
            .Add("DefaultVATCode", GetType(Boolean))
        End With

        Try
            CallEM006S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "DBCompany-EM006S"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Sub CallEM006S()
        Dim cmd As New iDB2Command
        Dim commandText As New StringBuilder
        cmd = conTALENTTKT.CreateCommand()
        commandText.Append("CALL EM006S(@PARAM0)")
        cmd.CommandText = commandText.ToString()
        cmd.CommandType = CommandType.Text

        Dim pErrorCode As New iDB2Parameter

        pErrorCode = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty

        Dim cmdAdapter As New IBM.Data.DB2.iSeries.iDB2DataAdapter
        cmdAdapter.SelectCommand = cmd
        cmdAdapter.Fill(ResultDataSet, "VATCodeListResults")

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ReturnCode") = CStr(cmd.Parameters(0).Value).Trim
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ReturnCode") = String.Empty
            drStatus("ErrorOccurred") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub

#End Region

End Class