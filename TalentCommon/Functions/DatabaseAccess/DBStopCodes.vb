Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBStopcodes
    Inherits DBAccess
    Public Property Company As String
    Public Property Customer As String
    Public Property ConTalent As iDB2Connection


    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case Settings.ModuleName
            Case Is = "RetrieveTalentStopCodes" : err = DBRetrieveStopcodes()
            Case Is = "RetrieveTalentStopCodeAudit" : err = DBRetrieveTalentStopcodeAudit()
        End Select

        Return err
    End Function

    Protected Function DBRetrieveStopcodes() As ErrorObj
        Dim err As New ErrorObj

        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim AgentCompany As String = Me.Company
        Dim dtStopcodes As New DataTable
        ResultDataSet = New DataSet
        dtStopcodes.TableName = "StopCodes"
        dtStopcodes.Columns.Add("Code", GetType(String))
        dtStopcodes.Columns.Add("Description", GetType(String))

        Try
            Const sqlSelect As String = "SELECT TRIM(CODE23) AS Code, TRIM(CODE23) CONCAT ' - ' CONCAT TRIM(STXT23) AS Description FROM CD023L1"
            cmdSelect = New iDB2Command(sqlSelect, conTALENTTKT)
            dtr = cmdSelect.ExecuteReader
            Dim dRow As DataRow = Nothing
            dRow = dtStopcodes.NewRow
            While dtr.Read
                dRow = Nothing
                dRow = dtStopcodes.NewRow
                dRow("Code") = dtr.Item("Code")
                dRow.Item("Description") = dtr.Item("Description")
                dtStopcodes.Rows.Add(dRow)
            End While

            ResultDataSet.Tables.Add(dtStopcodes)

        Catch ex As Exception
            Const strError As String = "Error Retrieving Stop Codes"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBRetrieveStopcodes-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function


    Protected Function DBRetrieveTalentStopcodeAudit() As ErrorObj
        Dim err As New ErrorObj

        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim AgentCompany As String = Me.Company
        Dim Member As String = Me.Customer
        Dim dtStopcodeAudit As New DataTable
        ResultDataSet = New DataSet
        dtStopcodeAudit.TableName = "StopCodeAudit"
        dtStopcodeAudit.Columns.Add("Date", GetType(Decimal))
        dtStopcodeAudit.Columns.Add("Time", GetType(Decimal))
        dtStopcodeAudit.Columns.Add("User", GetType(String))
        dtStopcodeAudit.Columns.Add("FromCode", GetType(String))
        dtStopcodeAudit.Columns.Add("ToCode", GetType(String))
        Try
            Const sqlSelect As String = "SELECT UPDT24, TIME24, USER24, CD023.CODE23 || ' - ' || CD023.STXT23 FROMCODE , CD023L1.CODE23 || ' - ' || CD023L1.STXT23 TOCODE FROM CD024L1 LEFT OUTER JOIN CD023 ON FSCD24 = CD023.CODE23 LEFT OUTER JOIN CD023L1 ON TSCD24 = CD023L1.CODE23 WHERE MEMB24 = @MEMBER"
            cmdSelect = New iDB2Command(sqlSelect, conTALENTTKT)
            cmdSelect.Parameters.Add("@MEMBER", iDB2DbType.iDB2VarChar, 12).Value = Me.Customer
            dtr = cmdSelect.ExecuteReader

            While dtr.Read
                Dim dRow As DataRow = Nothing
                dRow = dtStopcodeAudit.NewRow
                dRow("User") = dtr.Item("user24").ToString().Trim()
                dRow.Item("Date") = dtr.Item("updt24").ToString().Trim()
                dRow.Item("Time") = dtr.Item("time24").ToString().Trim()
                dRow.Item("FromCode") = dtr.Item("fromcode").ToString().Trim()
                dRow.Item("ToCode") = dtr.Item("tocode").ToString().Trim()
                dtStopcodeAudit.Rows.Add(dRow)
            End While

            ResultDataSet.Tables.Add(dtStopcodeAudit)

        Catch ex As Exception
            Const strError As String = "Error Retrieving Stop Code Audit"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBRetrieveStopcodes-02"
                .HasError = True
            End With
        End Try

        Return err
    End Function
End Class
