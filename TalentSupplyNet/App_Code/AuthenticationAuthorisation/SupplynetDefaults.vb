Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Suplynet Defaults Class
'
'       Date                        09/05/09
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'  
'       Error Number Code base      TTPSPDF- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    '-----------------------------------------
    ' Defaults stored on new defaults_bu files
    '-----------------------------------------
    Public Class SupplynetDefaults

        Private _businessUnit As String
        Public Property BusinessUnit() As String
            Get
                Return _businessUnit
            End Get
            Set(ByVal value As String)
                _businessUnit = value
            End Set
        End Property

        Private _partner As String
        Public Property Partner() As String
            Get
                Return _partner
            End Get
            Set(ByVal value As String)
                _partner = value
            End Set
        End Property


        Public Sub New(ByVal businessUnit As String, _
                       ByVal partner As String)
            _businessUnit = businessUnit
            _partner = partner

        End Sub

        Public Function GetDefault(ByVal defaultName As String) As String
            Dim defaultValue As String = String.Empty

            Dim conTalent As SqlConnection = Nothing
            Const SqlServer2005 As String = "SqlServer2005"
            conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
            conTalent.Open()            '
            '   
            Try
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtrDefaults As SqlDataReader = Nothing
                Dim dtrDatabase As SqlDataReader = Nothing

                Const strSelect1 As String = "SELECT VALUE FROM tbl_supplynet_module_defaults_bu WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                             "  AND PARTNER = @PARTNER AND DEFAULT_NAME = @DEFAULT_NAME "

                cmdSelect = New SqlCommand(strSelect1, conTalent)

                With cmdSelect
                    .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = BusinessUnit
                    .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = Partner
                    .Parameters.Add(New SqlParameter("@DEFAULT_NAME", SqlDbType.Char, 50)).Value = defaultName
                End With

                dtrDefaults = cmdSelect.ExecuteReader()
                If Not dtrDefaults.HasRows Then
                    '-----------------
                    ' Try all partners
                    '-----------------
                    dtrDefaults.Close()
                    cmdSelect = New SqlCommand(strSelect1, conTalent)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.Char, 50)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.Char, 50)).Value = "*ALL"
                        .Parameters.Add(New SqlParameter("@DEFAULT_NAME", SqlDbType.Char, 50)).Value = defaultName
                    End With
                    dtrDefaults = cmdSelect.ExecuteReader()
                End If
                If dtrDefaults.HasRows Then
                    dtrDefaults.Read()
                    defaultValue = dtrDefaults("VALUE").ToString.Trim
                End If
                dtrDefaults.Close()
                conTalent.Close()

            Catch ex As Exception
                Try
                    conTalent.Close()
                Catch ex2 As Exception

                End Try
            End Try


            Return defaultValue
        End Function

    End Class

End Namespace