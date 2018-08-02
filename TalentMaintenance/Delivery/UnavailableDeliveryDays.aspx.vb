Imports Talent.Common
Imports System.Data

Partial Class Delivery_UnavailableDeliveryDays
    Inherits PageControlBase

#Region "Class Level Fields"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty

#End Region

#Region "Constants"

    Const PAGECODE As String = "UnavailableDeliveryDays.aspx"

#End Region

#Region "Public Properties"

    Public DeleteButtonText As String

#End Region

#Region "Proteceted Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage()

        ltlPageTitle.Text = _wfrPage.Content("PageTitleText", _languageCode, True)
        ltlPageInstructions.Text = _wfrPage.Content("PageInstructionsText", _languageCode, True)
        lblAddDate.Text = _wfrPage.Content("AddDateLabelText", _languageCode, True)
        lblDate.Text = _wfrPage.Content("DateLabelText", _languageCode, True)
        lblCarrier.Text = _wfrPage.Content("CarrierLabelText", _languageCode, True)
        btnAddDate.Text = _wfrPage.Content("AddDateButtonText", _languageCode, True)
        DeleteButtonText = _wfrPage.Content("DeleteButtonText", _languageCode, True)
        plhErrorMessage.Visible = False
        If Not Page.IsPostBack Then bindDatesRepeater(True)
    End Sub

    Protected Sub rptCurrentDates_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptCurrentDates.ItemCommand
        If e.CommandName = "Delete" Then
            Dim rowsAffected As Integer = TDataObjects.DeliverySettings.TblDeliveryUnavailableDates.DeleteDateById(e.CommandArgument)
            If rowsAffected > 0 Then
                bindDatesRepeater(False)
            Else
                ltlErrorMessage.Text = _wfrPage.Content("ErrorDeletingDate", _languageCode, True)
                plhErrorMessage.Visible = True
            End If
        End If
    End Sub

    Protected Sub btnAddDate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddDate.Click
        Dim errorAddingDate As Boolean = False
        If txtAddDate.Text.Length > 0 Then
            Dim dateToAdd As Date
            Try
                dateToAdd = CDate(txtAddDate.Text)
                ' check if it already exists
                Dim dt As Data.DataTable = TDataObjects.DeliverySettings.TblDeliveryUnavailableDates.GetUnavailableDatesByCarrierDate(txtCarrier.Text, dateToAdd)
                If dt.Rows.Count = 0 Then
                    Dim rowsAffected As Integer = TDataObjects.DeliverySettings.TblDeliveryUnavailableDates.AddDate(dateToAdd, txtCarrier.Text)

                    If rowsAffected > 0 Then
                        bindDatesRepeater(False)
                    Else
                        errorAddingDate = True
                    End If
                Else
                    errorAddingDate = True
                End If
            Catch ex As Exception
                errorAddingDate = True
            End Try
        Else
            errorAddingDate = True
        End If
        If errorAddingDate Then
            ltlErrorMessage.Text = _wfrPage.Content("ErrorAddingDate", _languageCode, True)
            plhErrorMessage.Visible = True
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Bind the unavailable dates repeater
    ''' </summary>
    ''' <param name="fromCache">Determines whether the data bound is from cache or not</param>
    ''' <remarks></remarks>
    Private Sub bindDatesRepeater(ByVal fromCache As Boolean)
        Dim dtUnavailableDates As DataTable = TDataObjects.DeliverySettings.TblDeliveryUnavailableDates.GetAllUnavailableDates(fromCache)
        If dtUnavailableDates IsNot Nothing AndAlso dtUnavailableDates.Rows.Count > 0 Then
            rptCurrentDates.DataSource = dtUnavailableDates
            rptCurrentDates.DataBind()
            plhCurrentDates.Visible = True
            plhNoDates.Visible = False
            ltlCurrentDates.Text = _wfrPage.Content("CurrentDatesText", _languageCode, True)
        Else
            plhCurrentDates.Visible = False
            plhNoDates.Visible = True
            ltlNoDatesAdded.Text = _wfrPage.Content("NoDatesAddedText", _languageCode, True)
        End If
    End Sub

#End Region

End Class