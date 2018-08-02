Imports System.Data.SqlClient
<Serializable()> _
Public Class DBQueue
    Inherits DBAccess

    Private Const UPDATE_SITE_ACTIVITY_TOTAL As String = "UpdateSiteActivityTotal"

#Region "TALENTQUEUE"

    Protected Overrides Function AccessDataBaseTALENTQUEUE() As ErrorObj
        Dim err As New ErrorObj
        Try
            Select Case _settings.ModuleName
                Case Is = UPDATE_SITE_ACTIVITY_TOTAL : err = UpdateSiteActivityTotal()
            End Select
        Catch sqlEx As SqlException
            err.HasError = True
            err.ErrorNumber = "TACDBQ-01"
            err.ErrorMessage = sqlEx.Message
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACDBQ-02"
            err.ErrorMessage = ex.Message
        End Try
        Return err
    End Function

#End Region

    Private Function UpdateSiteActivityTotal() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim sqlCmd As New SqlCommand()
            sqlCmd.Connection = conTALENTQUEUE
            sqlCmd.CommandType = CommandType.StoredProcedure
            sqlCmd.CommandText = "usp_Queue_SiteActivityTotal_Update"
            ResultDataSet = ExecuteDataSetForSQLDB(sqlCmd)
        Catch sqlEx As SqlException
            ResultDataSet = Nothing
            Throw
        Catch ex As Exception
            ResultDataSet = Nothing
            Throw
        End Try
        Return err
    End Function

End Class
