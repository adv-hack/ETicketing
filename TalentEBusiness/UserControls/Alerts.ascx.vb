Imports System.Data
Imports TC = Talent.Common
Imports TEB = Talent.eCommerce
Partial Class UserControls_Alerts
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = String.Empty
    Private _defaults As TEB.ECommerceModuleDefaults.DefaultValues
    Private _settings As TC.DESettings = Nothing
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _removeAlertButtonText As String = String.Empty
    Private _dvUserAlerts As New DataView
    Private Const SQLSERVER2005 As String = "SqlServer2005"
    Private Const KEYCODE As String = "Alerts.ascx"
    Private Const DESTINATIONDATABASE As String = "SQL2005"

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _languageCode = TC.Utilities.GetDefaultLanguage
        _settings = New TC.DESettings
        _ucr = New Talent.Common.UserControlResource
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Alerts.ascx"
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        ProcessAlert()
        Me.Visible = (rptUserAlerts.Items.Count > 0 OrElse lblRedirectMessage.Visible)
    End Sub

    Protected Sub rptUserAlerts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptUserAlerts.ItemDataBound
        Select Case e.Item.ItemType
            Case Is = ListItemType.Header
                Dim ltlHeaderString As Literal = CType(e.Item.FindControl("ltlHeaderString"), Literal)
                Dim headerString As String = _ucr.Content("AlertHeaderString", _languageCode, True)
                headerString = headerString.Replace("<<NO_OF_ALERTS>>", _dvUserAlerts.Count)
                ltlHeaderString.Text = headerString

            Case Is = ListItemType.Item, ListItemType.AlternatingItem
                Try
                    Dim alertDataRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
                    Dim plhAlertString As PlaceHolder = CType(e.Item.FindControl("plhAlertString"), PlaceHolder)
                    Dim lbtnRemoveThisAlert As LinkButton = CType(e.Item.FindControl("lbtnRemoveThisAlert"), LinkButton)
                    Dim alertItemFullString As String = String.Empty
                    Dim alertItemStringPart1 As String = String.Empty
                    Dim alertItemStringPart2 As String = String.Empty
                    Dim removeAlertButtonPosition As Integer = 0
                    Dim imageUrl As String = alertDataRow("IMAGE_PATH").ToString()

                    If imageUrl.ToLower().StartsWith("http") Then
                        If Request.IsSecureConnection Then
                            imageUrl = imageUrl.Replace("http:", "https:")
                        End If
                    End If
                    If imageUrl.Length = 0 Then
                        alertItemFullString = _ucr.Content("AlertItemStringNoImage", _languageCode, True)
                    Else
                        alertItemFullString = _ucr.Content("AlertItemString", _languageCode, True)
                    End If

                    If alertItemFullString.Length > 0 Then
                        Select Case alertDataRow("ACTION").ToString.ToUpper
                            Case Is = "LINK", "CUSTOMERRESERVATION"
                                alertItemFullString = alertItemFullString.Replace("<<ACTION_DETAIL>>", ResolveUrl(alertDataRow("ACTION_DETAILS").ToString))
                            Case Is = "SENDEMAIL"
                                alertItemFullString = alertItemFullString.Replace("<<ACTION_DETAIL>>", "mailto:" & alertDataRow("ACTION_DETAILS").ToString)
                            Case Is = "INFO"
                                alertItemFullString = alertItemFullString.Replace("<<ACTION_DETAIL>>", "#")
                        End Select

                        If alertDataRow("ACTION_DETAILS_URL_OPTION") Then
                            alertItemFullString = alertItemFullString.Replace("<<ALERT_TARGET>>", "_blank")
                        Else
                            alertItemFullString = alertItemFullString.Replace("<<ALERT_TARGET>>", "_parent")
                        End If
                        alertItemFullString = alertItemFullString.Replace("<<ALERT_SUBJECT>>", alertDataRow("SUBJECT"))
                        alertItemFullString = alertItemFullString.Replace("<<ALERT_DESCRIPTION>>", alertDataRow("DESCRIPTION"))
                        alertItemFullString = alertItemFullString.Replace("<<ALERT_IMAGE_SRC>>", imageUrl)

                        lbtnRemoveThisAlert.Text = _ucr.Content("AlertItemRemoveTextString", _languageCode, True)
                        removeAlertButtonPosition = alertItemFullString.IndexOf("<<REMOVE_ALERT>>")
                        alertItemStringPart1 = alertItemFullString.Substring(0, removeAlertButtonPosition)
                        alertItemStringPart2 = alertItemFullString.Substring(removeAlertButtonPosition, alertItemFullString.Length - removeAlertButtonPosition)
                        alertItemStringPart2 = alertItemStringPart2.Replace("<<REMOVE_ALERT>>", String.Empty)
                        If alertItemStringPart2.Length > 0 Then
                            plhAlertString.Controls.Add(New LiteralControl(alertItemStringPart1))
                            plhAlertString.Controls.Add(lbtnRemoveThisAlert)
                            plhAlertString.Controls.Add(New LiteralControl(alertItemStringPart2))
                        Else
                            plhAlertString.Controls.Add(New LiteralControl(alertItemFullString))
                        End If
                    End If
                Catch ex As Exception
                    e.Item.Visible = False
                End Try
            Case Is = ListItemType.Footer
                Dim ltlFooterString As Literal = CType(e.Item.FindControl("ltlFooterString"), Literal)
                ltlFooterString.Text = _ucr.Content("AlertFooterString", _languageCode, True)
        End Select
    End Sub

    Protected Sub rptUserAlerts_ItemCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptUserAlerts.ItemCommand
        If e.CommandName = "RemoveThisAlert" Then
            Session("NumberOfUnreadAlerts") = Nothing
            Dim hdfAlertID As HiddenField = CType(e.Item.FindControl("hdfAlertID"), HiddenField)
            TDataObjects.AlertSettings.MarkAlertAsReadReturnAlerts(_ucr.BusinessUnit, _ucr.PartnerCode, Profile.UserName, CInt(hdfAlertID.Value))
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

#End Region

#Region "Private Methods"
    Private Sub ProcessAlert()
        Dim moduleDefaults As New TEB.ECommerceModuleDefaults
        Dim dtUserAlerts As New DataTable
        TDataObjects.Settings.BusinessUnit = _ucr.BusinessUnit
        TDataObjects.Settings.Partner = _ucr.PartnerCode
        _defaults = moduleDefaults.GetDefaults
        If _defaults.AlertsEnabled And Not Profile.IsAnonymous Then
            Dim redirectQSResult As String = HttpContext.Current.Request.QueryString("reInd")
            If String.IsNullOrWhiteSpace(redirectQSResult) Then
                Dim unAuthorisedRestrictQSResult As String = HttpContext.Current.Request.QueryString("unauthorisedMsg")
                If String.IsNullOrWhiteSpace(unAuthorisedRestrictQSResult) Then
                    getUserAlerts()
                Else
                    lblRedirectMessage.Visible = True
                    dtUserAlerts = TDataObjects.AlertSettings.TblAlertDefinition.GetByAlertName(unAuthorisedRestrictQSResult)
                    lblRedirectMessage.Text = dtUserAlerts.Rows(0).Item("DESCRIPTION").ToString()
                End If
            Else
                'display redirect message
                dtUserAlerts = TDataObjects.AlertSettings.TblAlertDefinition.GetByAlertID(redirectQSResult)
                Dim alertDescription As String = dtUserAlerts.Rows(0).Item("DESCRIPTION").ToString()
                lblRedirectMessage.Visible = True
                lblRedirectMessage.Text = alertDescription
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"
    Private Function getUserAlerts() As Boolean
        Dim userHasAlerts As Boolean = False
        Dim dtUserAlerts As New DataTable
        dtUserAlerts = TDataObjects.AlertSettings.GetUnReadUserAlertsByBUPartnerLoginID(_ucr.BusinessUnit, _ucr.PartnerCode, Profile.UserName, False)
        dtUserAlerts.DefaultView.RowFilter = "[ACTION_TYPE] = 0 OR [ACTION_TYPE] IS NULL"
        _dvUserAlerts = dtUserAlerts.DefaultView
        If _dvUserAlerts.ToTable().Rows.Count > 0 Then
            userHasAlerts = True
            _removeAlertButtonText = _ucr.Content("RemoveThisAlertButtonText", _languageCode, True)
            Try
                rptUserAlerts.DataSource = _dvUserAlerts
                rptUserAlerts.DataBind()
            Catch ex As Exception
                userHasAlerts = False
            End Try
        End If
        If userHasAlerts Then
            If Session("CanShowAlertOnPageLoad") Is Nothing AndAlso Not AgentProfile.IsAgent Then
                Dim sb As New StringBuilder
                sb.Append("$(function() {OpenAlertsWindow();});")
                Page.ClientScript.RegisterClientScriptBlock(Page.GetType, "OpenAlertsWindow", sb.ToString(), True)
                Session("CanShowAlertOnPageLoad") = "FALSE"
            End If
        End If
        Return userHasAlerts
    End Function

#End Region

End Class
