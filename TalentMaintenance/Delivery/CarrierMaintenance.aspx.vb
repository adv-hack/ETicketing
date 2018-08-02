Imports Talent.Common
Imports System.Data

Partial Class Delivery_CarrierMaintenance
    Inherits PageControlBase

#Region "Class Level Fields"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty

#End Region

#Region "Constants"

    Const PAGECODE As String = "CarrierMaintenance.aspx"

#End Region

#Region "Public Properties"

    Public CarrierCodeHeaderText As String
    Public InstallationAvailableHeaderText As String
    Public CollectOldAvailableHeaderText As String
    Public DeliverMondayHeaderText As String
    Public DeliverTuesdayHeaderText As String
    Public DeliverWednesdayHeaderText As String
    Public DeliverThursdayHeaderText As String
    Public DeliverFridayHeaderText As String
    Public DeliverSaturdayHeaderText As String
    Public DeliverSundayHeaderText As String
    Public SaveButtonText As String
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
        plhErrorMessage.Visible = False
        setPageText()
        If Not IsPostBack Then bindRepeater(False)
    End Sub

    Protected Sub rptCarrier_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptCarrier.ItemCommand
        plhErrorMessage.Visible = True
        If e.CommandName = "Save" Then
            Dim txtCarrierCode As TextBox = CType(e.Item.FindControl("txtCarrierCode"), TextBox)
            If String.IsNullOrEmpty(txtCarrierCode.Text.Trim) Then
                ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeRequired", _languageCode, True)
                bindRepeater(False)
            Else
                Dim chkInstallationAvailable As CheckBox = CType(e.Item.FindControl("chkInstallationAvailable"), CheckBox)
                Dim chkCollectOldAvailable As CheckBox = CType(e.Item.FindControl("chkCollectOldAvailable"), CheckBox)
                Dim chkDeliverMonday As CheckBox = CType(e.Item.FindControl("chkDeliverMonday"), CheckBox)
                Dim chkDeliverTuesday As CheckBox = CType(e.Item.FindControl("chkDeliverTuesday"), CheckBox)
                Dim chkDeliverWednesday As CheckBox = CType(e.Item.FindControl("chkDeliverWednesday"), CheckBox)
                Dim chkDeliverThursday As CheckBox = CType(e.Item.FindControl("chkDeliverThursday"), CheckBox)
                Dim chkDeliverFriday As CheckBox = CType(e.Item.FindControl("chkDeliverFriday"), CheckBox)
                Dim chkDeliverSaturday As CheckBox = CType(e.Item.FindControl("chkDeliverSaturday"), CheckBox)
                Dim chkDeliverSunday As CheckBox = CType(e.Item.FindControl("chkDeliverSunday"), CheckBox)
                Dim carrierId As Integer = CInt(e.CommandArgument)
                Dim rowsAffected As Integer = 0
                rowsAffected = TDataObjects.DeliverySettings.TblCarrier.UpdateCarrierRecordById(carrierId, txtCarrierCode.Text, _
                            chkInstallationAvailable.Checked, chkCollectOldAvailable.Checked, chkDeliverMonday.Checked, _
                            chkDeliverTuesday.Checked, chkDeliverWednesday.Checked, chkDeliverThursday.Checked, _
                            chkDeliverFriday.Checked, chkDeliverSaturday.Checked, chkDeliverSunday.Checked)
                If rowsAffected > 0 Then
                    ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeUpdated", _languageCode, True)
                    bindRepeater(False)
                Else
                    ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeNotUpdated", _languageCode, True)
                End If
            End If
        ElseIf e.CommandName = "Delete" Then
            Dim carrierId As Integer = CInt(e.CommandArgument)
            Dim rowsAffected As Integer = 0
            rowsAffected = TDataObjects.DeliverySettings.TblCarrier.Delete(carrierId)
            If rowsAffected > 0 Then
                ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeDeleted", _languageCode, True)
                bindRepeater(False)
            Else
                ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeNotDeleted", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub btnAddCarrier_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddCarrier.Click
        plhErrorMessage.Visible = True
        If String.IsNullOrEmpty(txtAddCarrierCode.Text.Trim) Then
            ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeRequired", _languageCode, True)
        Else
            Dim rowsAffected As Integer = 0
            rowsAffected = TDataObjects.DeliverySettings.TblCarrier.AddNewCarrierRecord(txtAddCarrierCode.Text, chkAddInstallationAvailable.Checked, _
                            chkAddCollectOldAvailable.Checked, chkAddDeliverMonday.Checked, chkAddDeliverTuesday.Checked, chkAddDeliverWednesday.Checked, _
                            chkAddDeliverThursday.Checked, chkAddDeliverFriday.Checked, chkAddDeliverSaturday.Checked, chkAddDeliverSunday.Checked)

            If rowsAffected > 0 Then
                ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeAdded", _languageCode, True)
                bindRepeater(False)
                txtAddCarrierCode.Text = String.Empty
                chkAddInstallationAvailable.Checked = False
                chkAddCollectOldAvailable.Checked = False
                chkAddDeliverMonday.Checked = False
                chkAddDeliverTuesday.Checked = False
                chkAddDeliverWednesday.Checked = False
                chkAddDeliverThursday.Checked = False
                chkAddDeliverFriday.Checked = False
                chkAddDeliverSaturday.Checked = False
                chkAddDeliverSunday.Checked = False
            Else
                ltlErrorMessage.Text = _wfrPage.Content("CarrierCodeNotAdded", _languageCode, True)
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the text properties of the asp.net objects
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageText()
        ltlPageTitle.Text = _wfrPage.Content("PageTitleText", _languageCode, True)
        ltlPageInstructions.Text = _wfrPage.Content("PageInstructionsText", _languageCode, True)
        CarrierCodeHeaderText = _wfrPage.Content("CarrierCodeHeaderText", _languageCode, True)
        InstallationAvailableHeaderText = _wfrPage.Content("InstallationAvailableHeaderText", _languageCode, True)
        CollectOldAvailableHeaderText = _wfrPage.Content("CollectOldAvailableHeaderText", _languageCode, True)
        DeliverMondayHeaderText = _wfrPage.Content("DeliverMondayHeaderText", _languageCode, True)
        DeliverTuesdayHeaderText = _wfrPage.Content("DeliverTuesdayHeaderText", _languageCode, True)
        DeliverWednesdayHeaderText = _wfrPage.Content("DeliverWednesdayHeaderText", _languageCode, True)
        DeliverThursdayHeaderText = _wfrPage.Content("DeliverThursdayHeaderText", _languageCode, True)
        DeliverFridayHeaderText = _wfrPage.Content("DeliverFridayHeaderText", _languageCode, True)
        DeliverSaturdayHeaderText = _wfrPage.Content("DeliverSaturdayHeaderText", _languageCode, True)
        DeliverSundayHeaderText = _wfrPage.Content("DeliverSundayHeaderText", _languageCode, True)
        SaveButtonText = _wfrPage.Content("SaveButtonText", _languageCode, True)
        DeleteButtonText = _wfrPage.Content("DeleteButtonText", _languageCode, True)
        ltlAddCarrierLegend.Text = _wfrPage.Content("AddCarrierLegend", _languageCode, True)
        lblAddCarrierCode.Text = CarrierCodeHeaderText
        lblAddInstallationAvailable.Text = InstallationAvailableHeaderText
        lblAddCollectOldAvailable.Text = CollectOldAvailableHeaderText
        lblAddDeliverMonday.Text = DeliverMondayHeaderText
        lblAddDeliverTuesday.Text = DeliverTuesdayHeaderText
        lblAddDeliverWednesday.Text = DeliverWednesdayHeaderText
        lblAddDeliverThursday.Text = DeliverThursdayHeaderText
        lblAddDeliverFriday.Text = DeliverFridayHeaderText
        lblAddDeliverSaturday.Text = DeliverSaturdayHeaderText
        lblAddDeliverSunday.Text = DeliverSundayHeaderText
        btnAddCarrier.Text = _wfrPage.Content("AddCarrierButtonText", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Bind the carrier repeater list from the database records
    ''' </summary>
    ''' <param name="fromCache">Get the records from the cache or from database table</param>
    ''' <remarks></remarks>
    Private Sub bindRepeater(ByVal fromCache As Boolean)
        Dim dtCarrier As New DataTable
        dtCarrier = TDataObjects.DeliverySettings.TblCarrier.GetAll(fromCache)
        If dtCarrier IsNot Nothing AndAlso dtCarrier.Rows.Count > 0 Then
            plhNoCarriers.Visible = False
            rptCarrier.Visible = True
            rptCarrier.DataSource = dtCarrier
            rptCarrier.DataBind()
        Else
            plhNoCarriers.Visible = True
            ltlNoCarriers.Text = _wfrPage.Content("NoCarriers", _languageCode, True)
            rptCarrier.Visible = False
        End If
    End Sub

#End Region

End Class