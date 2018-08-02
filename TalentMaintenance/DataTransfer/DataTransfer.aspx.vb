Imports Talent.Common
Imports System.Data
Imports System.Data.SqlClient

Partial Class DataTransfer_DataTransfer
    Inherits PageControlBase

    Public InProgressRecords As Integer = 0
    Public SuccessRecords As Integer = 0
    Public FailedRecords As Integer = 0

    Dim _recordsAffected As Integer = 0

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = "DataTransfer.aspx"
            .KeyCode = "DataTransfer.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        ltlHomeLink.Text = _wfrPage.Content("HomeLink", _languageCode, True)
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = _wfrPage.Content("HeaderText", _languageCode, True)
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        rptDataTransfer.DataSource = TDataObjects.LoggingSettings.TblDataTransferStatus.SelectAllRecords
        rptDataTransfer.DataBind()
        If rptDataTransfer.Items.Count > 0 Then
            plhStatusMessages.Visible = Page.IsPostBack
            rptDataTransfer.Visible = True
            plhTableKey.Visible = True
            plhDeleteButtons.Visible = True
        Else
            plhStatusMessages.Visible = True
            If Not Page.IsPostBack Then ltlStatusMessage.Text = "No records have been found"
            rptDataTransfer.Visible = False
            plhTableKey.Visible = False
            plhDeleteButtons.Visible = False
        End If
    End Sub

    Protected Sub btnDeleteAllRecords_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteAllRecords.Click
        _recordsAffected = TDataObjects.LoggingSettings.TblDataTransferStatus.DeleteAllRecords
        ltlStatusMessage.Text = _recordsAffected & " record(s) have been deleted"
    End Sub

    Protected Sub btnDeleteAllFailedRecords_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteAllFailedRecords.Click
        _recordsAffected = TDataObjects.LoggingSettings.TblDataTransferStatus.DeleteFailedRecords
        ltlStatusMessage.Text = _recordsAffected & " record(s) have been deleted"
    End Sub

    Protected Sub btnDeleteAllSuccessRecords_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteAllSuccessRecords.Click
        _recordsAffected = TDataObjects.LoggingSettings.TblDataTransferStatus.DeleteSuccessRecords
        ltlStatusMessage.Text = _recordsAffected & " record(s) have been deleted"
    End Sub

    Protected Sub btnDeleteSelectedRecords_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteSelectedRecords.Click
        Dim recordsAffected As Integer = 0
        For Each rptDataTransferItem As RepeaterItem In rptDataTransfer.Items
            Dim chkDeleteRecord As CheckBox = CType(rptDataTransferItem.FindControl("chkDeleteRecord"), CheckBox)
            If Not chkDeleteRecord Is Nothing Then
                If chkDeleteRecord.Checked Then
                    Dim hdfID As HiddenField = CType(rptDataTransferItem.FindControl("hdfID"), HiddenField)
                    Try
                        _recordsAffected = TDataObjects.LoggingSettings.TblDataTransferStatus.DeleteByID(CInt(hdfID.Value))
                        recordsAffected += _recordsAffected
                    Catch ex As Exception
                    End Try
                End If
            End If
        Next
        ltlStatusMessage.Text = recordsAffected & " record(s) have been deleted"
    End Sub

    Public Function ShowCssClassBasedOnStatus(ByVal status As String) As String
        Dim cssClass As String = String.Empty
        If Not String.IsNullOrEmpty(status) Then
            Select Case status.ToUpper
                Case Is = DataTransferStatusEnum.CLEARING_WORK_TABLE.ToString
                    cssClass = " in-progress-row"
                    InProgressRecords += 1
                Case Is = DataTransferStatusEnum.EXTRACTING_FROM_ISERIES.ToString
                    cssClass = " in-progress-row"
                    InProgressRecords += 1
                Case Is = DataTransferStatusEnum.FAILED.ToString
                    cssClass = " failed-row"
                    FailedRecords += 1
                Case Is = DataTransferStatusEnum.FINISHED.ToString
                    cssClass = " success-row"
                    SuccessRecords += 1
                Case Is = DataTransferStatusEnum.UPDATING_SQL_TABLES.ToString
                    cssClass = " in-progress-row"
                    InProgressRecords += 1
                Case Else
                    cssClass = String.Empty
            End Select
        End If
        Return cssClass
    End Function

End Class
