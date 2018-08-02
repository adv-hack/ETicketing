Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBPriceBands
    Inherits DBAccess
    Public Property company As String
    Public Property dePriceBands As DEPriceBands
    Public Property conTalent As iDB2Connection
    ' Public Property ResultDataSet As DataSet

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case Settings.ModuleName
            Case Is = "RetrieveTalentPriceBands" : err = DBRetrievePriceBands()
        End Select

        Return err
    End Function

    Protected Function DBRetrievePriceBands() As ErrorObj
        Dim err As New ErrorObj
       
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim AgentCompany As String = Me.company
        Dim dtPriceBands As New DataTable
        ResultDataSet = New DataSet
        dtPriceBands.TableName = "PriceBands"
        dtPriceBands.Columns.Add("Code")
        dtPriceBands.Columns.Add("Description")

        Try
            Const sqlSelect As String = "SELECT CODE12 as Code, CODE12 || ' - ' || STXT12 as Description FROM SD012L1,SD12AL1 WHERE CONO12 = @COMPANY AND CONO12 = CONO2A AND CODE2A = CODE12 order by code12 "
            cmdSelect = New iDB2Command(sqlSelect, conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = AgentCompany

            dtr = cmdSelect.ExecuteReader


            While dtr.Read
                Dim dRow As DataRow = Nothing
                dRow = dtPriceBands.NewRow
                dRow("Code") = dtr.Item("Code")
                dRow.Item("Description") = dtr.Item("Description")
                dtPriceBands.Rows.Add(dRow)
            End While

            ResultDataSet.Tables.Add(dtPriceBands)

        Catch ex As Exception
            Const strError As String = "Error Retrieving Price Bands"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBRetrievePriceBands-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

End Class
