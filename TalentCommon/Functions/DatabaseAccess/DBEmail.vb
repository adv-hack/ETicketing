Imports System.Data.SqlClient
Imports System.Xml

Public Class DBEmail
    Inherits DBAccess

    Private Const FetchFailedEmails As String = "FetchFailedEmails"
    Private Const SetFailedEmailsReportedOK As String = "SetFailedEmailsReportedOK"
    Private Const SetFailedEmailsReportedFailed As String = "SetFailedEmailsReportedFailed"

    Private _SupplyNetBusinessUnit As String
    Private _IDs As String

    Public Property SupplyNetBusinessUnit() As String
        Get
            Return _SupplyNetBusinessUnit
        End Get
        Set(ByVal value As String)
            _SupplyNetBusinessUnit = value
        End Set
    End Property

    Public Property IDs() As String
        Get
            Return _IDs
        End Get
        Set(ByVal value As String)
            _IDs = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        If Not Me.conSql2005.State = ConnectionState.Open Then
            Me.Sql2005Open()
        End If

        If Me.conSql2005.State = ConnectionState.Open Then

            Select Case _settings.ModuleName
                Case Is = FetchFailedEmails : err = FetchTheFailedEmails()
                Case Is = SetFailedEmailsReportedOK : err = SetTheFailedEmailsReported("InValidEmailAddressReportedOK")
                Case Is = SetFailedEmailsReportedFailed : err = SetTheFailedEmailsReported("InValidEmailAddressReportedFailed")

            End Select

            If Me.conSql2005.State <> ConnectionState.Closed Then
                Me.Sql2005Close()
            End If

        End If

        Return err
    End Function

    Protected Function FetchTheFailedEmails() As ErrorObj
        Dim err As New ErrorObj

        Me.ResultDataSet = New DataSet
        Dim DtFailedEmails As New DataTable("FailedEmails")
        Me.ResultDataSet.Tables.Add(DtFailedEmails)
        AddFailedEmailsColumns(DtFailedEmails)

        Const SQL As String = "UPDATE tbl_offline_processing " & _
                              "SET status = 'InValidEmailAddressReportingInProgress', timestamp_last_updated = GETDATE() " & _
                              "WHERE business_unit IN (@businessUnit, @supplyNetBusinessUnit) " & _
                              "AND status = 'InValidEmailAddress' " & _
                              "AND monitor_name = 'EmailMonitor'; " & _
                              "SELECT ID, PARAMETER, ERROR_INFORMATION, TIMESTAMP_ADDED FROM tbl_offline_processing " & _
                              "WHERE business_unit IN (@businessUnit, @supplyNetBusinessUnit) " & _
                              "AND status = 'InValidEmailAddressReportingInProgress' " & _
                              "AND monitor_name = 'EmailMonitor'"

        Dim cmd As New SqlCommand(SQL, conSql2005)

        With cmd.Parameters
            .Clear()
            .Add("@businessUnit", SqlDbType.NVarChar).Value = Settings.BusinessUnit
            .Add("@supplyNetBusinessUnit", SqlDbType.NVarChar).Value = SupplyNetBusinessUnit
        End With

        Try
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet()
            da.Fill(ds)

            If ds.Tables.Count > 0 Then
                Dim dt As DataTable = ds.Tables(0)

                For Each dr As DataRow In dt.Rows
                    Dim FeRow As DataRow = DtFailedEmails.NewRow()
                    FeRow.Item("ID") = dr.Item("ID")
                    FeRow.Item("CustomerNumber") = ExtractCustomerFromXML(dr.Item("PARAMETER"))
                    FeRow.Item("EmailAddress") = ExtractEmailAddressFromXML(dr.Item("PARAMETER"))
                    FeRow.Item("ReasonForFailure") = dr.Item("ERROR_INFORMATION")
                    FeRow.Item("DateSent") = dr.Item("TIMESTAMP_ADDED")
                    DtFailedEmails.Rows.Add(FeRow)
                Next

            End If

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error Retrieving Failed Emails details"
                .ErrorNumber = "TACDBEMAIL-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function ExtractCustomerFromXML(ByVal param As String) As String
        Dim Value As String = ""
        Dim xPar As XmlDocument = New XmlDocument()

        Try
            xPar.LoadXml(param)
            Dim Node As XmlNode = xPar.SelectSingleNode("EmailMonitor").SelectSingleNode("Parameters").SelectSingleNode("Customer")
            Value = Node.InnerText

        Catch ex As Exception
        End Try

        Return Value

    End Function

    Private Function ExtractEmailAddressFromXML(ByVal param As String) As String
        Dim Value As String = ""
        Dim xPar As XmlDocument = New XmlDocument()

        Try
            xPar.LoadXml(param)
            Dim Node As XmlNode = xPar.SelectSingleNode("EmailMonitor").SelectSingleNode("Settings").SelectSingleNode("ToAddress")
            Value = Node.InnerText
            Dim Address As String() = Value.Split(";")
            If Address.GetLength(0) > 0 Then
                Value = Address(0)
            End If

        Catch ex As Exception
        End Try

        Return Value

    End Function

    Private Sub AddFailedEmailsColumns(ByRef dtFailedEmails As DataTable)

        With dtFailedEmails.Columns
            .Add("ID", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("EmailAddress", GetType(String))
            .Add("ReasonForFailure", GetType(String))
            .Add("DateSent", GetType(String))
        End With

    End Sub

    Protected Function SetTheFailedEmailsReported(ByVal Status As String) As ErrorObj
        Dim err As New ErrorObj

        Const SQL As String = "UPDATE tbl_offline_processing " & _
                              "SET status = @status, timestamp_last_updated = GETDATE() " & _
                              "WHERE business_unit IN (@businessUnit, @supplyNetBusinessUnit) " & _
                              "AND status = 'InValidEmailAddressReportingInProgress' " & _
                              "AND monitor_name = 'EmailMonitor' " & _
                              "AND ID IN ({0})"

        Dim cmd As New SqlCommand(String.Format(SQL, IDs), conSql2005)

        With cmd.Parameters
            .Clear()
            .Add("@businessUnit", SqlDbType.NVarChar).Value = Settings.BusinessUnit
            .Add("@supplyNetBusinessUnit", SqlDbType.NVarChar).Value = SupplyNetBusinessUnit
            .Add("@status", SqlDbType.NVarChar).Value = Status
        End With

        Try
            cmd.ExecuteNonQuery()

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error Setting Failed Emails status to " & Status
                .ErrorNumber = "TACDBEMAIL-02"
                .HasError = True
            End With
        End Try

        Return err
    End Function


End Class
