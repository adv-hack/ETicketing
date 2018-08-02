Option Strict On
Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Prompt Requests
'
'       Date                        11/02/08
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

#Region "DBUserPrompt"
<Serializable()> _
Public Class DBUserPrompt
    Inherits DBAccess

    Private _de As New DEPrompt
    Private _dtResults As New DataTable("Users")


    Public Property De() As DEPrompt
        Get
            Return _de
        End Get
        Set(ByVal value As DEPrompt)
            _de = value
        End Set
    End Property

    Public Property DtResults() As DataTable
        Get
            Return _dtResults
        End Get
        Set(ByVal value As DataTable)
            _dtResults = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
      
        AddColumnsToDataTables()

        If ResultDataSet Is Nothing Then
            ResultDataSet = New DataSet
        End If

        '-----------------------
        ' Load user data from db
        '-----------------------
        Dim dtrReader As iDB2DataReader = Nothing
        Dim drRow As Data.DataRow
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strSELECT As String = "SELECT AGAGNT,AGAGNM FROM T#AG " & _
                                   " WHERE AGACTV = 'A' "
        ' If txtUserFilter.Text <> String.Empty Then
        '   sqlSelect &= " AND AGAGNT >= @PARAM1 "
        ' End If
        strSELECT &= "ORDER BY AGAGNT"
        
        Try
            cmdSELECT = New iDB2Command(strSELECT, conTALENTCRM)
            ' If txtUserFilter.Text <> String.Empty Then
            '  cmdSelect.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 8).Value = txtUserFilter.Text
            ' End If
            dtrReader = cmdSELECT.ExecuteReader()
            While dtrReader.Read
                drRow = DtResults.NewRow()
                drRow("User") = dtrReader("AGAGNT").ToString
                drRow("Description") = dtrReader("AGAGNM").ToString
                DtResults.Rows.Add(drRow)
            End While

        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBxx-02"
                .HasError = True
            End With
        Finally
            Try
                dtrReader.Close()
                conSystem21.Close()
            Catch ex As Exception
            End Try
        End Try

        ResultDataSet.Tables.Add(DtResults)

        Return err
    End Function



    Private Sub AddColumnsToDataTables()
        With DtResults.Columns
            .Add("User", GetType(String))
            .Add("Description", GetType(String))
        End With
    End Sub

End Class
#End Region

#Region "DBDepartmentPrompt"
<Serializable()> _
Public Class DBDepartmentPrompt
    Inherits DBAccess

    Private _de As New DEPrompt
    Private _dtResults As New DataTable("Departments")


    Public Property De() As DEPrompt
        Get
            Return _de
        End Get
        Set(ByVal value As DEPrompt)
            _de = value
        End Set
    End Property

    Public Property DtResults() As DataTable
        Get
            Return _dtResults
        End Get
        Set(ByVal value As DataTable)
            _dtResults = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj

        AddColumnsToDataTables()

        If ResultDataSet Is Nothing Then
            ResultDataSet = New DataSet
        End If

        '-----------------------
        ' Load user data from db
        '-----------------------
        Dim dtrReader As iDB2DataReader = Nothing
        Dim drRow As Data.DataRow
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strSELECT As String = " SELECT DDNAME, DDDDDS FROM T#DD " & _
                                           " WHERE DDACTV = 'A' ORDER BY DDNAME"
        ' If txtUserFilter.Text <> String.Empty Then
        '   sqlSelect &= " AND AGAGNT >= @PARAM1 "
        ' End If

        Try
            cmdSELECT = New iDB2Command(strSELECT, conTALENTCRM)
            ' If txtUserFilter.Text <> String.Empty Then
            '  cmdSelect.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 8).Value = txtUserFilter.Text
            ' End If
            dtrReader = cmdSELECT.ExecuteReader()
            While dtrReader.Read
                drRow = DtResults.NewRow()
                drRow("Department") = dtrReader("DDNAME").ToString
                drRow("Description") = dtrReader("DDDDDS").ToString
                DtResults.Rows.Add(drRow)
            End While

        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBxx-02"
                .HasError = True
            End With
        Finally
            Try
                dtrReader.Close()
                conSystem21.Close()
            Catch ex As Exception
            End Try
        End Try

        ResultDataSet.Tables.Add(DtResults)

        Return err
    End Function



    Private Sub AddColumnsToDataTables()
        With DtResults.Columns
            .Add("Department", GetType(String))
            .Add("Description", GetType(String))
        End With
    End Sub

End Class
#End Region




