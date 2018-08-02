Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Upload Order Templates Requests
'
'       Date                        31/07/07
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBUT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBUploadTemplates
    Inherits DBAccess

    Private _deUploadTemplates As New DEUploadTemplates

    Public Property DEUploadTemplates() As DEUploadTemplates
        Get
            Return _deUploadTemplates
        End Get
        Set(ByVal value As DEUploadTemplates)
            _deUploadTemplates = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Open up database
        '
        Dim cmdSELECT As iDB2Command = Nothing
        Dim iOrder As Integer = 0
        Dim iItem As Integer = 0
        Dim parmInput, Paramoutput As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        '
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/XXXXXXXXX(@PARAM1, @PARAM2)"
        Try
            cmdSELECT = New iDB2Command(strHEADER, conSystem21)
            '----------------------------------------------------------------------------------
            parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            With parmInput
                .Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                    Utilities.FixStringLength(Settings.AccountNo2, 8) & _
                                    Utilities.FixStringLength(Settings.AccountNo3, 3)

                .Direction = ParameterDirection.Input
            End With
            '----------------------------------------------------------------------------------
            Paramoutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
            With Paramoutput
                .Value = String.Empty
                .Direction = ParameterDirection.InputOutput
            End With
            '----------------------------------------------------------------------------------
            cmdSELECT.ExecuteNonQuery()
            PARAMOUT = cmdSELECT.Parameters(Param2).Value.ToString

            If PARAMOUT.Substring(1023, 1) = "Y" Then
                '--------------------------------------------
                ' Check for errors with the invoice selection
                '--------------------------------------------
                With err
                    .ErrorMessage = String.Empty
                    .ErrorNumber = "TACDBUT-04"
                    .ErrorStatus = "Error selecting invoices -  " & _
                                        PARAMOUT.Substring(1019, 4) & "-" & _
                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                        "ENG", "ERRORCODE", PARAMOUT.Substring(1019, 4))
                    .HasError = True
                End With
            Else
                '---------------------------------
                ' No errors - Build return dataset 
                ' 
                ResultDataSet = New DataSet
                Dim DtStatusResults As New DataTable
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("XXXXXXXXXX", GetType(String))
                    .Add("XXXXXXXXXX", GetType(String))
                End With

                Dim dRow As DataRow = Nothing
                Dim strResult As String = String.Empty
                Dim intIndex As Int16 = 0
                Dim strResultSet As String = PARAMOUT.Substring(36, 160)
                strResult = strResultSet.Substring(0, 8)
                '---------------------------------------------------------------------
                ' Loop through results set
                ' Results are in the format Invoice No(7char) + Invoice Status (1char)
                '---------------------------------------------------------------------
                While strResult.Trim <> String.Empty And intIndex < 20
                    dRow = DtStatusResults.NewRow
                    dRow("XXXXXXXXXXX") = strResult.Substring(0, 7)
                    dRow("XXXXXXXXXXX") = strResult.Substring(7, 1)
                    DtStatusResults.Rows.Add(dRow)

                    intIndex += 1
                    If intIndex < 20 Then _
                        strResult = strResultSet.Substring(intIndex * 8, 8)
                End While

            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBUT-02"
                .HasError = True
                Return err
            End With
        End Try
        '--------------------------------------------------------------------
        Return err
    End Function

    Private Function DataEntityUnPack() As ErrorObj
        Dim err As New ErrorObj

        Return err
    End Function

End Class
