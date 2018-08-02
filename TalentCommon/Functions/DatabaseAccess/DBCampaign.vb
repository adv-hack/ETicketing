Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Products 
'
'       Date                        08/01/2008
'
'       Author                      Ben Ford
'
'       � CS Group 2006             All rights reserved.
'                                    
'       Error Number Code base      TACDBCD- 
'                                   
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBCampaign
    Inherits DBAccess

    Private _de As New DECampaignDetails
    Private Const CampaignDetails As String = "CampaignDetails"

    Public Property De() As DECampaignDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DECampaignDetails)
            _de = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        '   Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing

        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/WRITECAM(@PARAM1, @PARAM2)"
        Dim parmInput1, parmOutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty

        If Not err.HasError Then
            Try

                cmdSELECT = New iDB2Command(strHEADER, conTALENTCRM)

                parmInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                parmInput1.Value = BuildParm1CRM()
                parmInput1.Direction = ParameterDirection.Input

                parmOutput = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                parmOutput.Value = String.Empty
                parmOutput.Direction = ParameterDirection.InputOutput

                cmdSELECT.ExecuteNonQuery()

                PARMOUT = cmdSELECT.Parameters(Param5).Value.ToString

            Catch ex As Exception
                Const strError As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBCD-01"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function
    Private Function BuildParm1CRM() As String

        Dim myString As String
        Dim sb As New StringBuilder
        Dim parmCompany = Utilities.FixStringLength(Settings.Company, 10)
        Dim parmCampaignCode = Utilities.FixStringLength(De.CampaignCode, 10)
        With sb
            .Append(parmCompany).Append(parmCampaignCode)
        End With

        myString = sb.ToString
        Return myString
    End Function


End Class
