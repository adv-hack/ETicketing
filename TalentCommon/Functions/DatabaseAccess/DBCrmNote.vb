Option Strict On
Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with CreditNote Requests
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBCN- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
#Region "DBCrmNote"
<Serializable()> _
Public Class DBCrmNote
    Inherits DBAccess

    Private _de As New DECrmNote
    Private _dtHeaderResults As New DataTable
    Private _dtItemResults As New DataTable


    Public Property De() As DECrmNote
        Get
            Return _de
        End Get
        Set(ByVal value As DECrmNote)
            _de = value
        End Set
    End Property


    Public Property DtHeaderResults() As DataTable
        Get
            Return _dtHeaderResults
        End Get
        Set(ByVal value As DataTable)
            _dtHeaderResults = value
        End Set
    End Property
    Public Property DtItemResults() As DataTable
        Get
            Return _dtItemResults
        End Get
        Set(ByVal value As DataTable)
            _dtItemResults = value
        End Set
    End Property


    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
       
        Return err
    End Function


End Class
#End Region

#Region "DBCrmNote11"
<Serializable()> _
Public Class DBCrmNote11
    Inherits DBAccess

    Private _de As New DECrmNote
    Private _dtCaptions As New DataTable("Captions")


    Public Property De() As DECrmNote
        Get
            Return _de
        End Get
        Set(ByVal value As DECrmNote)
            _de = value
        End Set
    End Property

    Public Property DtCaptions() As DataTable
        Get
            Return _dtCaptions
        End Get
        Set(ByVal value As DataTable)
            _dtCaptions = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj

        AddColumnsToDataTables()

        If ResultDataSet Is Nothing Then
            ResultDataSet = New DataSet
        End If
        '----------------------
        ' Create command object
        '----------------------
        Dim cmdSELECT As iDB2Command = Nothing

        Dim parmInput, parmOutput1, parmOutput2 As iDB2Parameter
        Dim PARMOUT1 As String = String.Empty
        Dim PARMOUT2 As String = String.Empty
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/NOTE11R(@PARAM1, @PARAM2,@PARAM3)"

        If Not err.HasError Then
            Try
                cmdSELECT = New iDB2Command(strHEADER, conTALENTCRM)

                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput.Value = De.NoteID.ToString.PadLeft(13)
                parmInput.Direction = ParameterDirection.Input

                parmOutput1 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 2048)
                parmOutput1.Value = String.Empty
                parmOutput1.Direction = ParameterDirection.InputOutput

                parmOutput2 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
                parmOutput2.Value = String.Empty
                parmOutput2.Direction = ParameterDirection.InputOutput

                cmdSELECT.ExecuteNonQuery()
                PARMOUT1 = cmdSELECT.Parameters(Param2).Value.ToString
                PARMOUT2 = cmdSELECT.Parameters(Param3).Value.ToString

                ' Branch
                AddResultsRow("001", PARMOUT2.Substring(0, 10), PARMOUT1.Substring(0, 3))
                
                ' Customer Ref
                AddResultsRow("002", PARMOUT2.Substring(10, 10), PARMOUT1.Substring(3, 30))
                
                ' Note type
                AddResultsRow("003", PARMOUT2.Substring(20, 10), PARMOUT1.Substring(33, 12))

                ' User
                AddResultsRow("004", PARMOUT2.Substring(30, 10), PARMOUT1.Substring(45, 8))

                ' Dept
                AddResultsRow("005", PARMOUT2.Substring(40, 10), PARMOUT1.Substring(53, 12))

                ' Contact Ref
                AddResultsRow("006", PARMOUT2.Substring(50, 10), PARMOUT1.Substring(65, 30))

                ' Product ref
                AddResultsRow("007", PARMOUT2.Substring(60, 10), PARMOUT1.Substring(95, 12))

                ' Report flag
                AddResultsRow("008", PARMOUT2.Substring(70, 10), PARMOUT1.Substring(107, 1))

                ' Probability
                AddResultsRow("009", PARMOUT2.Substring(80, 10), PARMOUT1.Substring(108, 3))

                ' Site customer
                AddResultsRow("010", PARMOUT2.Substring(90, 10), PARMOUT1.Substring(111, 30))

                ' New business flag
                AddResultsRow("011", PARMOUT2.Substring(100, 10), PARMOUT1.Substring(141, 1))

                ' Status
                AddResultsRow("012#1", PARMOUT2.Substring(110, 10), PARMOUT1.Substring(142, 10))

                ' Status Mandatory
                AddResultsRow("012#2", String.Empty, PARMOUT1.Substring(152, 1))

                ' Activity Flag
                AddResultsRow("013", PARMOUT2.Substring(120, 10), PARMOUT1.Substring(153, 1))

                ' Customer address
                AddResultsRow("014#1", String.Empty, PARMOUT1.Substring(154, 190))

                ' Site address
                AddResultsRow("014#2", String.Empty, PARMOUT1.Substring(344, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Quote
                AddResultsRow("015", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))

                ' Contact address
                AddResultsRow("014#3", String.Empty, PARMOUT1.Substring(534, 190))


                '----------------------------
                ' Add table to result dataset
                '----------------------------
                ResultDataSet.Tables.Add(DtCaptions)
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TACDBxx-02"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function

    Private Sub AddResultsRow(ByVal key As String, ByVal label As String, ByVal content As String)
        Dim dr As Data.DataRow
        dr = DtCaptions.NewRow
        dr("Key") = key.Trim
        dr("Label") = label.Trim
        dr("Content") = content.Trim
        DtCaptions.Rows.Add(dr)
    End Sub

    Private Sub AddColumnsToDataTables()
        With DtCaptions.Columns
            .Add("Key", GetType(String))
            .Add("Label", GetType(String))
            .Add("Content", GetType(String))
        End With
    End Sub

End Class
#End Region


