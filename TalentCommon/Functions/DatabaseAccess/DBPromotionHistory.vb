Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBPromotionHistory
    Inherits DBAccess
    Public Property Company As String
    Public Property Member As String
    ' Public Property dePriceBands As DEPriceBands
    Public Property conTalent As iDB2Connection
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Select Case Settings.ModuleName
            Case Is = "RetrieveTalentPromotionHistory" : err = DBRetrieveTalentPromotionHistory()
        End Select
        Return err
    End Function

    
    Protected Function DBRetrieveTalentPromotionHistory() As ErrorObj
        Dim err As New ErrorObj

        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim AgentCompany As String = Me.Company
        Dim dtPromotionHistory As New DataTable
        ResultDataSet = New DataSet
        dtPromotionHistory.TableName = "PromotionHistory"

        dtPromotionHistory.Columns.Add("Date", GetType(String))
        dtPromotionHistory.Columns.Add("Reference", GetType(Decimal))
        dtPromotionHistory.Columns.Add("Product", GetType(String))
        dtPromotionHistory.Columns.Add("Description", GetType(String))
        dtPromotionHistory.Columns.Add("Value", GetType(Decimal))
        dtPromotionHistory.Columns.Add("Payref", GetType(Decimal))
        dtPromotionHistory.Columns.Add("Status", GetType(String))

        Try

            Dim strSelect As New StringBuilder
            strSelect.Append("SELECT date11, PYID11, MTCD11, COMM20, valu11, PYRF11, ACTR11 FROM py011l5 left outer join MD020L6 on digits(IDID11) = TRRF20")
            strSelect.Append(" WHERE CONO11 = CONO20 and MTCD11 = mTCD20  AND MEMB11 = @MEMBER AND CONO11 = @COMPANY and actr11 <> 'D'")

            Dim sqlselect As String = strSelect.ToString
            cmdSelect = New iDB2Command(sqlselect, conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Me.Company
            cmdSelect.Parameters.Add("@MEMBER", iDB2DbType.iDB2VarChar, 12).Value = Me.Member

            dtr = cmdSelect.ExecuteReader


            While dtr.Read
                Dim dRow As DataRow = Nothing
                dRow = dtPromotionHistory.NewRow
                dRow("Date") = dtr.Item("date11")
                dRow.Item("Reference") = dtr.Item("PYID11")
                dRow.Item("Product") = dtr.Item("MTCD11")
                dRow.Item("Description") = dtr.Item("COMM20")
                dRow.Item("Value") = dtr.Item("valu11")
                dRow.Item("Payref") = dtr.Item("PYRF11")
                dRow.Item("Status") = dtr.Item("ACTR11")
                dtPromotionHistory.Rows.Add(dRow)
            End While
            ResultDataSet.Tables.Add(dtPromotionHistory)

        Catch ex As Exception
            Const strError As String = "Error Retrieving Promotion History"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "BRetrieveTalentPromotionHistory-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function


End Class
