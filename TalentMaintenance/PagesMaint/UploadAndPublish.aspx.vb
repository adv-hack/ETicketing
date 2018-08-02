Imports Talent.Common
Imports System.Data
Imports System.Net
Imports System.IO
Imports Talent.Maintenance
Imports System.Data.SqlClient
Imports System.Xml
Imports System.Collections.Generic
Imports System.Collections
Imports System.Linq

Partial Class PagesMaint_UploadAndPublish
    Inherits PageControlBase

#Region "Class Level Fields"

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _objPage As New pageselectionTableAdapters.PageTemplateTableAdapter
    Private _objPageOpt As New PageDataSetTableAdapters.PageDateTableAdapter
    Private _objPageTempate As New PageDataSetTableAdapters.TemplatePageTableAdapter
    Private _myFtp As Talent.Maintenance.FTPclient
    Private _errorMsg As String = String.Empty
    Private lblLastUpdated As String = "Last Updated: "
    Private _talentLogging As TalentLogging = Nothing

    Private _QSBusinessUnit As String
    Private _QSPartner As String
    Private _liveUpdateServer01 As String = String.Empty
    Private _liveUpdateServer02 As String = String.Empty
    Private _liveUpdateServer03 As String = String.Empty
    Private _liveUpdateServer04 As String = String.Empty
    Private _liveUpdateServer05 As String = String.Empty
    Private _liveUpdateServer06 As String = String.Empty
    Private _liveUpdateServer07 As String = String.Empty
    Private _liveUpdateServer08 As String = String.Empty
    Private _liveUpdateServer09 As String = String.Empty
    Private _liveUpdateServer10 As String = String.Empty
    Private _liveUpdateDatabase01 As String = String.Empty
    Private _liveUpdateDatabase02 As String = String.Empty
    Private _liveUpdateDatabase03 As String = String.Empty
    Private _liveUpdateDatabase04 As String = String.Empty
    Private _liveUpdateDatabase05 As String = String.Empty
    Private _liveUpdateDatabase06 As String = String.Empty
    Private _liveUpdateDatabase07 As String = String.Empty
    Private _liveUpdateDatabase08 As String = String.Empty
    Private _liveUpdateDatabase09 As String = String.Empty
    Private _liveUpdateDatabase10 As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PageCode = String.Empty
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Rollout.aspx"
            .PageCode = "Rollout.aspx"
        End With
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _talentLogging = New Talent.Common.TalentLogging
        _talentLogging.FrontEndConnectionString = _wfrPage.FrontEndConnectionString
        _QSBusinessUnit = Request.QueryString("BU")
        _QSPartner = Request.QueryString("Partner")

        If Not IsPostBack Then LoadTypes()

        RetrieveCMSSettings()

    End Sub
    Protected Sub rptType_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptType.ItemCommand
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim sArg As String = CType(e.CommandArgument, String)
            Select Case e.CommandName
                Case Is = "UploadImages"
                    Dim ImageOverwriteCheck As CheckBox = CType(e.Item.FindControl("ImageOverwriteCheck"), CheckBox)
                    Dim ImageUpload1 As FileUpload = CType(e.Item.FindControl("ImageUpload1"), FileUpload)
                    Dim blImageErrorList As BulletedList = CType(e.Item.FindControl("blImageErrList"), BulletedList)
                    UploadImages(sArg, ImageUpload1, ImageOverwriteCheck.Checked, blImageErrorList)
                Case Is = "PublishImages"
                    Dim ImageProcessChildDirectories As CheckBox = CType(e.Item.FindControl("ImageProcessChildDirectories"), CheckBox)
                    Dim blImageErrorList As BulletedList = CType(e.Item.FindControl("blImageErrList"), BulletedList)
                    Dim hidImagesName As HiddenField = CType(e.Item.FindControl("hidImagesName"), HiddenField)
                    Dim lbLastUpdated As Label = CType(e.Item.FindControl("imagesDate"), Label)
                    PublishFiles(sArg, blImageErrorList, hidImagesName.Value, lbLastUpdated, ImageProcessChildDirectories.Checked)
                Case Is = "UploadFiles"
                    Dim FileOverwriteCheck As CheckBox = CType(e.Item.FindControl("FileOverwriteCheck"), CheckBox)
                    Dim FileUpload1 As FileUpload = CType(e.Item.FindControl("FileUpload1"), FileUpload)
                    Dim blFileErrList As BulletedList = CType(e.Item.FindControl("blFileErrList"), BulletedList)
                    UploadFiles(sArg, FileUpload1, FileOverwriteCheck.Checked, blFileErrList)
                Case Is = "PublishFiles"
                    Dim FileProcessChildDirectories As CheckBox = CType(e.Item.FindControl("FileProcessChildDirectories"), CheckBox)
                    Dim blFileErrList As BulletedList = CType(e.Item.FindControl("blFileErrList"), BulletedList)
                    Dim hidFilesName As HiddenField = CType(e.Item.FindControl("hidFilesName"), HiddenField)
                    Dim lbLastUpdated As Label = CType(e.Item.FindControl("fileDate"), Label)
                    PublishFiles(sArg, blFileErrList, hidFilesName.Value, lbLastUpdated, FileProcessChildDirectories.Checked)
                Case Is = "ClearCacheCMS"
                    Dim blCMSErrList As BulletedList = CType(e.Item.FindControl("blCMSErrList"), BulletedList)
                    ClearCache(blCMSErrList)
                Case Is = "PublishCMS"
                    Dim blCMSErrList As BulletedList = CType(e.Item.FindControl("blCMSErrList"), BulletedList)
                    Dim hidCMSName As HiddenField = CType(e.Item.FindControl("hidCMSName"), HiddenField)
                    Dim lbLastUpdated As Label = CType(e.Item.FindControl("cmsDate"), Label)

                    Dim chkSystemDefaults As CheckBox = CType(e.Item.FindControl("chkSystemDefaults"), CheckBox)
                    Dim chkRetailProducts As CheckBox = CType(e.Item.FindControl("chkRetailProducts"), CheckBox)
                    Dim chkRetailNavigation As CheckBox = CType(e.Item.FindControl("chkRetailNavigation"), CheckBox)
                    Dim chkProductRelations As CheckBox = CType(e.Item.FindControl("chkProductRelations"), CheckBox)
                    Dim chkProductPersonalisation As CheckBox = CType(e.Item.FindControl("chkProductPersonalisation"), CheckBox)
                    Dim chkPromotions As CheckBox = CType(e.Item.FindControl("chkPromotions"), CheckBox)
                    Dim chkPageContent As CheckBox = CType(e.Item.FindControl("chkPageContent"), CheckBox)
                    Dim chkControlContent As CheckBox = CType(e.Item.FindControl("chkControlContent"), CheckBox)
                    Dim chkFlashContent As CheckBox = CType(e.Item.FindControl("chkFlashContent"), CheckBox)
                    Dim chkTicketingMenus As CheckBox = CType(e.Item.FindControl("chkTicketingMenus"), CheckBox)
                    Dim chkPageTracking As CheckBox = CType(e.Item.FindControl("chkPageTracking"), CheckBox)
                    Dim chkEmailTemplates As CheckBox = CType(e.Item.FindControl("chkEmailTemplates"), CheckBox)
                    Dim chkAlerts As CheckBox = CType(e.Item.FindControl("chkAlerts"), CheckBox)
                    Dim chkVouchers As CheckBox = CType(e.Item.FindControl("chkVouchers"), CheckBox)
                    Dim chkQueryStrings As CheckBox = CType(e.Item.FindControl("chkQueryStrings"), CheckBox)
                    Dim chkActivities As CheckBox = CType(e.Item.FindControl("chkActivities"), CheckBox)
                    Dim btnClearCacheCMS As Button = CType(e.Item.FindControl("btnClearCacheCMS"), Button)
                    sArg = "" +
                            CType(chkSystemDefaults.Checked, String) + ";" +
                            CType(chkRetailProducts.Checked, String) + ";" +
                            CType(chkRetailNavigation.Checked, String) + ";" +
                            CType(chkProductRelations.Checked, String) + ";" +
                            CType(chkProductPersonalisation.Checked, String) + ";" +
                            CType(chkPromotions.Checked, String) + ";" +
                            CType(chkPageContent.Checked, String) + ";" +
                            CType(chkControlContent.Checked, String) + ";" +
                            CType(chkFlashContent.Checked, String) + ";" +
                            CType(chkTicketingMenus.Checked, String) + ";" +
                            CType(chkPageTracking.Checked, String) + ";" +
                            CType(chkEmailTemplates.Checked, String) + ";" +
                            CType(chkAlerts.Checked, String) + ";" +
                            CType(chkVouchers.Checked, String) + ";" +
                            CType(chkQueryStrings.Checked, String) + ";" +
                            CType(chkActivities.Checked, String)
                    Dim success As Boolean = PublishCMS(sArg, blCMSErrList, hidCMSName.Value, lbLastUpdated)

                    ' Set all checkboxes to unchecked if success
                    If success Then
                        chkSystemDefaults.Checked = False
                        chkRetailProducts.Checked = False
                        chkRetailNavigation.Checked = False
                        chkProductRelations.Checked = False
                        chkProductPersonalisation.Checked = False
                        chkPromotions.Checked = False
                        chkPageContent.Checked = False
                        chkControlContent.Checked = False
                        chkFlashContent.Checked = False
                        chkTicketingMenus.Checked = False
                        chkPageTracking.Checked = False
                        chkEmailTemplates.Checked = False
                        chkAlerts.Checked = False
                        chkVouchers.Checked = False
                        chkQueryStrings.Checked = False
                        chkActivities.Checked = False
                    End If
            End Select
        End If
    End Sub
    Protected Sub rptType_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptType.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim itemData As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim sType As String = Utilities.CheckForDBNull_String(itemData("TYPE"))
            Select Case sType
                Case Is = "IMAGES"

                    Dim pnlImageTypeWrapper As Panel = CType(e.Item.FindControl("pnlImageTypeWrapper"), Panel)
                    If Not pnlImageTypeWrapper Is Nothing Then

                        ' Panel visible
                        pnlImageTypeWrapper.Visible = True

                        ' Date label
                        Dim imagesDate As Label = CType(e.Item.FindControl("imagesDate"), Label)
                        imagesDate.Text = Utilities.CheckForDBNull_String(itemData("LAST_UPDATED"))
                        If imagesDate.Text.Trim.Length = 0 Then
                            Dim d As Date = Now
                            imagesDate.Text = d.ToString("dd/MM/yyyy" + " 00:00:00")
                        End If
                        imagesDate.Text = lblLastUpdated + imagesDate.Text

                        ' Accordion text
                        Dim lblImageTypeTitle As Label = CType(e.Item.FindControl("lblImageTypeTitle"), Label)
                        If Not lblImageTypeTitle Is Nothing Then lblImageTypeTitle.Text = Utilities.CheckForDBNull_String(itemData("NAME"))
                        Dim ajaxImageType As AjaxControlToolkit.CollapsiblePanelExtender = CType(e.Item.FindControl("ajaxImageType"), AjaxControlToolkit.CollapsiblePanelExtender)
                        If Not ajaxImageType Is Nothing Then
                            ajaxImageType.Collapsed = True
                            ajaxImageType.CollapsedText = Utilities.CheckForDBNull_String(itemData("NAME"))
                            ajaxImageType.ExpandedText = Utilities.CheckForDBNull_String(itemData("NAME"))
                        End If

                        ' Upload button
                        Dim ImageUploadLink As Button = CType(e.Item.FindControl("ImageUploadLink"), Button)
                        Dim ImageOverwriteCheck As CheckBox = CType(e.Item.FindControl("ImageOverwriteCheck"), CheckBox)
                        ImageUploadLink.CommandName = "UploadImages"
                        ImageUploadLink.CommandArgument = Utilities.CheckForDBNull_String(itemData("LOCAL_ROOT_DIRECTORY")) + ";" + _
                                                            Utilities.CheckForDBNull_String(itemData("MAX_FILE_UPLOAD_SIZE")) + ";" + _
                                                            Utilities.CheckForDBNull_String(itemData("ALLOWABLE_FILE_TYPES")) + ";" + _
                                                            CType(ImageOverwriteCheck.Checked, String)

                        ' Publish button
                        Dim btnFtpImages As Button = CType(e.Item.FindControl("btnFtpImages"), Button)
                        btnFtpImages.CommandName = "PublishImages"
                        btnFtpImages.CommandArgument = Utilities.CheckForDBNull_String(itemData("LOCAL_ROOT_DIRECTORY")) + ";" + _
                                                        Utilities.CheckForDBNull_String(itemData("REMOTE_ROOT_DIRECTORY"))


                        ' Hidden field(s)
                        Dim hidImagesName As HiddenField = CType(e.Item.FindControl("hidImagesName"), HiddenField)
                        hidImagesName.Value = Utilities.CheckForDBNull_String(itemData("NAME"))

                    End If

                Case Is = "CMS"

                    Dim pnlCMSTypeWrapper As Panel = CType(e.Item.FindControl("pnlCMSTypeWrapper"), Panel)
                    If Not pnlCMSTypeWrapper Is Nothing Then

                        ' Panel visible
                        pnlCMSTypeWrapper.Visible = True

                        ' Date label
                        Dim cmsDate As Label = CType(e.Item.FindControl("cmsDate"), Label)
                        cmsDate.Text = Utilities.CheckForDBNull_String(itemData("LAST_UPDATED"))
                        If cmsDate.Text.Trim.Length = 0 Then
                            Dim d As Date = Now
                            cmsDate.Text = d.ToString("dd/MM/yyyy" + " 00:00:00")
                        End If
                        cmsDate.Text = lblLastUpdated + cmsDate.Text

                        ' Accordion text
                        Dim lblCMSTypeTitle As Label = CType(e.Item.FindControl("lblCMSTypeTitle"), Label)
                        If Not lblCMSTypeTitle Is Nothing Then lblCMSTypeTitle.Text = Utilities.CheckForDBNull_String(itemData("NAME"))
                        Dim ajaxCMSType As AjaxControlToolkit.CollapsiblePanelExtender = CType(e.Item.FindControl("ajaxCMSType"), AjaxControlToolkit.CollapsiblePanelExtender)
                        If Not ajaxCMSType Is Nothing Then
                            ajaxCMSType.Collapsed = True
                            ajaxCMSType.CollapsedText = Utilities.CheckForDBNull_String(itemData("NAME"))
                            ajaxCMSType.ExpandedText = Utilities.CheckForDBNull_String(itemData("NAME"))
                        End If

                        ' Publish button
                        Dim btnUploadCMS As Button = CType(e.Item.FindControl("btnUploadCMS"), Button)

                        ' Checkboxes
                        Dim chkSystemDefaults As CheckBox = CType(e.Item.FindControl("chkSystemDefaults"), CheckBox)
                        Dim plhchkSystemDefaults As PlaceHolder = CType(e.Item.FindControl("plhchkSystemDefaults"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowSystemDefaultsInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowSystemDefaultsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowSystemDefaultsInMatrix")) Then
                                plhchkSystemDefaults.Visible = True
                            End If
                        End If

                        Dim chkRetailProducts As CheckBox = CType(e.Item.FindControl("chkRetailProducts"), CheckBox)
                        Dim plhChkRetailProducts As PlaceHolder = CType(e.Item.FindControl("plhChkRetailProducts"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowProductDescriptionsInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowProductRelationsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowProductDescriptionsInMatrix")) Then
                                plhChkRetailProducts.Visible = True
                            End If
                        End If

                        Dim chkRetailNavigation As CheckBox = CType(e.Item.FindControl("chkRetailNavigation"), CheckBox)
                        Dim plhChkRetailNavigation As PlaceHolder = CType(e.Item.FindControl("plhChkRetailNavigation"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowEditNavigationInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowProductRelationsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowEditNavigationInMatrix")) Then
                                plhChkRetailNavigation.Visible = True
                            End If
                        End If

                        Dim chkProductRelations As CheckBox = CType(e.Item.FindControl("chkProductRelations"), CheckBox)
                        Dim plhChkProductRelations As PlaceHolder = CType(e.Item.FindControl("plhChkProductRelations"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowProductRelationsInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowProductRelationsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowProductRelationsInMatrix")) Then
                                plhChkProductRelations.Visible = True
                            End If
                        End If

                        Dim chkProductPersonalisation As CheckBox = CType(e.Item.FindControl("chkProductPersonalisation"), CheckBox)
                        Dim plhChkProductPersonalisation As PlaceHolder = CType(e.Item.FindControl("plhChkProductPersonalisation"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowProductPersonalisationInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowProductPersonalisationInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowProductPersonalisationInMatrix")) Then
                                plhChkProductPersonalisation.Visible = True
                            End If
                        End If

                        Dim chkPromotions As CheckBox = CType(e.Item.FindControl("chkPromotions"), CheckBox)
                        Dim plhChkPromotions As PlaceHolder = CType(e.Item.FindControl("plhChkPromotions"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowPromotionsInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowPromotionsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowPromotionsInMatrix")) Then
                                plhChkPromotions.Visible = True
                            End If
                        End If

                        Dim chkPageContent As CheckBox = CType(e.Item.FindControl("chkPageContent"), CheckBox)
                        Dim plhChkPageContent As PlaceHolder = CType(e.Item.FindControl("plhChkPageContent"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowEditPagesInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowProductRelationsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowEditPagesInMatrix")) Then
                                plhChkPageContent.Visible = True
                            End If
                        End If

                        Dim chkControlContent As CheckBox = CType(e.Item.FindControl("chkControlContent"), CheckBox)
                        Dim plhChkControlContent As PlaceHolder = CType(e.Item.FindControl("plhChkControlContent"), PlaceHolder)
                        If Utilities.isEnabled("AllowEditControls") Then
                            If CBool(ConfigurationManager.AppSettings("ShowEditPagesInMatrix")) Then
                                plhChkControlContent.Visible = True
                            End If
                        End If

                        Dim chkFlashContent As CheckBox = CType(e.Item.FindControl("chkFlashContent"), CheckBox)
                        Dim plhChkFlashContent As PlaceHolder = CType(e.Item.FindControl("plhChkFlashContent"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowFlashMaintenanceInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowFlashMaintenanceInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowFlashMaintenanceInMatrix")) Then
                                plhChkFlashContent.Visible = True
                            End If
                        End If

                        Dim chkTicketingMenus As CheckBox = CType(e.Item.FindControl("chkTicketingMenus"), CheckBox)
                        Dim plhChkTicketingMenus As PlaceHolder = CType(e.Item.FindControl("plhChkTicketingMenus"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowEditNavigationInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowEditNavigationInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowEditNavigationInMatrix")) Then
                                plhChkTicketingMenus.Visible = True
                            End If
                        End If

                        Dim chkPageTracking As CheckBox = CType(e.Item.FindControl("chkPageTracking"), CheckBox)
                        Dim plhChkPageTracking As PlaceHolder = CType(e.Item.FindControl("plhChkPageTracking"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowPageTrackingInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowPageTrackingInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowPageTrackingInMatrix")) Then
                                plhChkPageTracking.Visible = True
                            End If
                        End If

                        Dim chkEmailTemplates As CheckBox = CType(e.Item.FindControl("chkEmailTemplates"), CheckBox)
                        Dim plhChkEmailTemplates As PlaceHolder = CType(e.Item.FindControl("plhChkEmailTemplates"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowEmailTemplatesInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowEmailTemplatesInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowEmailTemplatesInMatrix")) Then
                                plhChkEmailTemplates.Visible = True
                            End If
                        End If

                        Dim chkAlerts As CheckBox = CType(e.Item.FindControl("chkAlerts"), CheckBox)
                        Dim plhChkAlerts As PlaceHolder = CType(e.Item.FindControl("plhChkAlerts"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowCustomerAlertsInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowCustomerAlertsInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowCustomerAlertsInMatrix")) Then
                                plhChkAlerts.Visible = True
                            End If
                        End If

                        Dim chkVouchers As CheckBox = CType(e.Item.FindControl("chkVouchers"), CheckBox)
                        Dim plhVouchers As PlaceHolder = CType(e.Item.FindControl("plhVouchers"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowVouchersInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowVouchersInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowVouchersInMatrix")) Then
                                plhVouchers.Visible = True
                            End If
                        End If

                        Dim chkQueryStrings As CheckBox = CType(e.Item.FindControl("chkQueryStrings"), CheckBox)
                        Dim plhQueryStrings As PlaceHolder = CType(e.Item.FindControl("plhQueryStrings"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowQueryStringInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowQueryStringInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowVouchersInMatrix")) Then
                                plhQueryStrings.Visible = True
                            End If
                        End If

                        Dim chkActivities As CheckBox = CType(e.Item.FindControl("chkActivities"), CheckBox)
                        Dim plhActivities As PlaceHolder = CType(e.Item.FindControl("plhActivities"), PlaceHolder)
                        If Not ConfigurationManager.AppSettings("ShowActivitiesInMatrix") = Nothing AndAlso ConfigurationManager.AppSettings("ShowActivitiesInMatrix").ToString.Trim.Length > 0 Then
                            If CBool(ConfigurationManager.AppSettings("ShowActivitiesInMatrix")) Then
                                plhActivities.Visible = True
                            End If
                        End If

                        ' Clear Cache button
                        Dim btnClearCacheCMS As Button = CType(e.Item.FindControl("btnClearCacheCMS"), Button)

                        btnUploadCMS.CommandName = "PublishCMS"
                        btnUploadCMS.CommandArgument = "" +
                                        CType(chkSystemDefaults.Checked, String) + ";" +
                                        CType(chkRetailProducts.Checked, String) + ";" +
                                        CType(chkRetailNavigation.Checked, String) + ";" +
                                        CType(chkProductRelations.Checked, String) + ";" +
                                        CType(chkProductPersonalisation.Checked, String) + ";" +
                                        CType(chkPageContent.Checked, String) + ";" +
                                        CType(chkControlContent.Checked, String) + ";" +
                                        CType(chkFlashContent.Checked, String) + ";" +
                                        CType(chkTicketingMenus.Checked, String) + ";" +
                                        CType(chkPageTracking.Checked, String) + ";" +
                                        CType(chkEmailTemplates.Checked, String) + ";" +
                                        CType(chkAlerts.Checked, String) + ";" +
                                        CType(chkVouchers.Checked, String) + ";" +
                                        CType(chkQueryStrings.Checked, String) + ";" +
                                        CType(chkActivities.Checked, String)

                        ' Clear cache button
                        btnClearCacheCMS.CommandName = "ClearCacheCMS"

                        ' Hidden field(s)
                        Dim hidCMSName As HiddenField = CType(e.Item.FindControl("hidCMSName"), HiddenField)
                        hidCMSName.Value = Utilities.CheckForDBNull_String(itemData("NAME"))
                    End If

                Case Is = "FILES"
                    Dim pnlFileTypeWrapper As Panel = CType(e.Item.FindControl("pnlFileTypeWrapper"), Panel)
                    If Not pnlFileTypeWrapper Is Nothing Then

                        ' Panel visible
                        pnlFileTypeWrapper.Visible = True

                        ' Date label
                        Dim fileDate As Label = CType(e.Item.FindControl("fileDate"), Label)
                        fileDate.Text = Utilities.CheckForDBNull_String(itemData("LAST_UPDATED"))
                        If fileDate.Text.Trim.Length = 0 Then
                            Dim d As Date = Now
                            fileDate.Text = d.ToString("dd/MM/yyyy" + " 00:00:00")
                        End If
                        fileDate.Text = lblLastUpdated + fileDate.Text

                        ' Accordion text
                        Dim lblFileTypeTitle As Label = CType(e.Item.FindControl("lblFileTypeTitle"), Label)
                        If Not lblFileTypeTitle Is Nothing Then lblFileTypeTitle.Text = Utilities.CheckForDBNull_String(itemData("NAME"))
                        Dim ajaxFileType As AjaxControlToolkit.CollapsiblePanelExtender = CType(e.Item.FindControl("ajaxFileType"), AjaxControlToolkit.CollapsiblePanelExtender)
                        If Not ajaxFileType Is Nothing Then
                            ajaxFileType.Collapsed = True
                            ajaxFileType.CollapsedText = Utilities.CheckForDBNull_String(itemData("NAME"))
                            ajaxFileType.ExpandedText = Utilities.CheckForDBNull_String(itemData("NAME"))
                        End If

                        ' Upload button
                        Dim FileUploadLink As Button = CType(e.Item.FindControl("FileUploadLink"), Button)
                        Dim ImageOverwriteCheck As CheckBox = CType(e.Item.FindControl("ImageOverwriteCheck"), CheckBox)
                        FileUploadLink.CommandName = "UploadFiles"
                        FileUploadLink.CommandArgument = Utilities.CheckForDBNull_String(itemData("LOCAL_ROOT_DIRECTORY")) + ";" + _
                                                            Utilities.CheckForDBNull_String(itemData("MAX_FILE_UPLOAD_SIZE")) + ";" + _
                                                            Utilities.CheckForDBNull_String(itemData("ALLOWABLE_FILE_TYPES")) + ";" + _
                                                            CType(ImageOverwriteCheck.Checked, String)

                        'Publish button
                        Dim btnFtpHTML As Button = CType(e.Item.FindControl("btnFtpHTML"), Button)
                        btnFtpHTML.CommandName = "PublishFiles"
                        btnFtpHTML.CommandArgument = Utilities.CheckForDBNull_String(itemData("LOCAL_ROOT_DIRECTORY")) + ";" + _
                                                        Utilities.CheckForDBNull_String(itemData("REMOTE_ROOT_DIRECTORY"))

                        ' Hidden field(s)
                        Dim hidFilesName As HiddenField = CType(e.Item.FindControl("hidFilesName"), HiddenField)
                        hidFilesName.Value = Utilities.CheckForDBNull_String(itemData("NAME"))

                    End If

            End Select

        End If
    End Sub

#End Region

#Region "Private Functions"
    Private Function ExtractSQLServer(ByVal connectionString As String) As String
        Dim sqlServer As String = String.Empty
        Using sqlConn As New SqlConnection(connectionString)
            sqlServer = "Server: " + sqlConn.DataSource & ", Database: " & sqlConn.Database
        End Using
        Return sqlServer
    End Function

    Private Function ConvertLastUpdated(ByVal dateStr As String) As Date
        Dim returnDate As Date = Date.MinValue
        dateStr = dateStr.Replace(lblLastUpdated, "")
        If Not String.IsNullOrEmpty(dateStr) Then
            Try
                Dim day, month, year, hours, minutes, seconds As Integer
                day = CInt(dateStr.Substring(0, 2))
                month = CInt(dateStr.Substring(3, 2))
                year = CInt(dateStr.Substring(6, 4))
                hours = CInt(dateStr.Substring(11, 2))
                minutes = CInt(dateStr.Substring(14, 2))
                seconds = CInt(dateStr.Substring(17, 2))
                returnDate = New Date(year, month, day, hours, minutes, seconds)
            Catch ex As Exception
            End Try
        End If
        Return returnDate
    End Function

    Private Function UploadDirectory(ByVal localDirectory As String, ByVal remoteDirectory As String, ByVal lastUpdated As Date, ByVal ProcessChildDirectories As Boolean) As Boolean
        Dim success As Boolean = False

        Try
            '-----------------------------------------
            ' Create remote directory and change to it
            '-------------------------------xm----------
            _myFtp.FtpCreateDirectory(remoteDirectory)
            _myFtp.CurrentDirectory = remoteDirectory
            '------------------------------
            ' List files in local directory
            '------------------------------
            Dim strFileSize As String = ""
            Dim di As New IO.DirectoryInfo(localDirectory)
            Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
            Dim fi As IO.FileInfo
            Dim connectionError As Boolean = False
            '--------------------------------------
            ' Upload each one into remote directory
            '--------------------------------------
            For Each fi In aryFi
                If fi.LastWriteTime > lastUpdated AndAlso (Not (fi.Attributes And IO.FileAttributes.Hidden)) Then
                    If fi.Name.Trim <> "Thumbs.db" And fi.LastWriteTime > lastUpdated Then
                        success = _myFtp.Upload(fi.FullName)
                        If Not success Then
                            _errorMsg = _errorMsg & " Failed to upload " & fi.FullName.Trim & " to server " & _myFtp.Hostname & ". " & _myFtp.UploadErrorMessage & "<br />"
                            If _errorMsg.Contains("Unable to connect to the remote server") Then
                                connectionError = True
                                _errorMsg = _errorMsg.Remove(_errorMsg.Length - 6, 6) & ". ABORTED!"
                                Exit For
                            End If
                        End If
                    End If
                End If
            Next
            If Not connectionError Then
                If ProcessChildDirectories Then

                    '--------------------------------
                    ' List folders in local directory
                    '--------------------------------
                    Dim aryDi As IO.DirectoryInfo() = di.GetDirectories()
                    Dim dir As IO.DirectoryInfo
                    '----------------------------------
                    ' Upload recursively in each folder
                    '----------------------------------
                    For Each dir In aryDi
                        If remoteDirectory.Trim.EndsWith("/") Then
                            UploadDirectory(dir.FullName, remoteDirectory & dir.Name, lastUpdated, ProcessChildDirectories)
                        Else
                            UploadDirectory(dir.FullName, remoteDirectory & "/" & dir.Name, lastUpdated, ProcessChildDirectories)
                        End If
                    Next
                    If _errorMsg = String.Empty Then
                        success = True
                    Else
                        success = False
                    End If
                End If
            End If
        Catch ex As Exception
            _errorMsg = "Error: " + ex.Message.ToString
            success = False
        End Try

        Return success
    End Function

    Private Function UploadCMS(ByVal connectionString As String, ByVal sArg As String) As Boolean
        Dim sArgs As String() = sArg.Split(New Char() {";"c})
        Dim boolSystemDefaults As Boolean = CType(sArgs(0), Boolean)
        Dim boolRetailProducts As Boolean = CType(sArgs(1), Boolean)
        Dim boolRetailNavigation As Boolean = CType(sArgs(2), Boolean)
        Dim boolProductRelations As Boolean = CType(sArgs(3), Boolean)
        Dim boolProductPersonalisation As Boolean = CType(sArgs(4), Boolean)
        Dim boolPromotions As Boolean = CType(sArgs(5), Boolean)
        Dim boolPageContent As Boolean = CType(sArgs(6), Boolean)
        Dim boolControlContent As Boolean = CType(sArgs(7), Boolean)
        Dim boolFlashContent As Boolean = CType(sArgs(8), Boolean)
        Dim boolTicketingMenus As Boolean = CType(sArgs(9), Boolean)
        Dim boolPageTracking As Boolean = CType(sArgs(10), Boolean)
        Dim boolEmailTemplates As Boolean = CType(sArgs(11), Boolean)
        Dim boolAlerts As Boolean = CType(sArgs(12), Boolean)
        Dim boolVouchers As Boolean = CType(sArgs(13), Boolean)
        Dim boolQueryStrings As Boolean = CType(sArgs(14), Boolean)
        Dim boolActivities As Boolean = CType(sArgs(15), Boolean)
        Dim success As Boolean = False
        _errorMsg = String.Empty

        Try

            'tbl_vouchers_external table
            If boolVouchers Then
                Dim upHelper As New UploadAndPublishHelper("tbl_vouchers_external", connectionString, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString())
                upHelper.UploadAndPublishTableData()
                success = True
            End If

            'tbl_querystring table
            If boolQueryStrings Then
                success = DoExtractTable("tbl_querystring", "tbl_querystring_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_querystring(connectionString)
                    success = True
                End If
            End If

            ' ECommerceModuleDefaultsBU
            If boolSystemDefaults Then
                success = DoExtractTable("tbl_ecommerce_module_defaults_bu", "tbl_ecommerce_module_defaults_bu_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_ecommerce_module_defaults_bu(connectionString, _QSBusinessUnit)
                    success = True
                End If
            End If


            'Retail Products Tables
            If boolRetailProducts Then
                success = DoExtractTable("tbl_product", "tbl_product_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_product(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_product_stock", "tbl_product_stock_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_product_stock(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_product_option_defaults", "tbl_product_option_defaults_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_product_option_defaults(connectionString, _QSBusinessUnit)
                            success = DoExtractTable("tbl_product_option_definitions", "tbl_product_option_definitions_work", connectionString)
                            If success Then
                                DoUpdateTable_tbl_product_option_definitions(connectionString, _QSBusinessUnit)
                                success = DoExtractTable("tbl_product_option_definitions_lang", "tbl_product_option_definitions_lang_work", connectionString)
                                If success Then
                                    DoUpdateTable_tbl_product_option_definitions_lang(connectionString, _QSBusinessUnit)
                                    success = DoExtractTable("tbl_product_option_types", "tbl_product_option_types_work", connectionString)
                                    If success Then
                                        DoUpdateTable_tbl_product_option_types(connectionString, _QSBusinessUnit)
                                        success = DoExtractTable("tbl_product_option_types_lang", "tbl_product_option_types_lang_work", connectionString)
                                        If success Then
                                            DoUpdateTable_tbl_product_option_types_lang(connectionString, _QSBusinessUnit)
                                            success = DoExtractTable("tbl_product_options", "tbl_product_options_work", connectionString)
                                            If success Then
                                                DoUpdateTable_tbl_product_options(connectionString, _QSBusinessUnit)
                                                success = True
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            'Retail Navigation Tables
            If boolRetailNavigation Then
                success = DoExtractTable("tbl_group", "tbl_group_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_group(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_group_product", "tbl_group_product_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_group_product(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_group_lang", "tbl_group_lang_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_group_lang(connectionString, _QSBusinessUnit)
                            success = DoExtractTable("tbl_group_level_01", "tbl_group_level_01_work", connectionString)
                            If success Then
                                DoUpdateTable_tbl_group_level_01(connectionString, _QSBusinessUnit)
                                success = DoExtractTable("tbl_group_level_02", "tbl_group_level_02_work", connectionString)
                                If success Then
                                    DoUpdateTable_tbl_group_level_02(connectionString, _QSBusinessUnit)
                                    success = DoExtractTable("tbl_group_level_03", "tbl_group_level_03_work", connectionString)
                                    If success Then
                                        DoUpdateTable_tbl_group_level_03(connectionString, _QSBusinessUnit)
                                        success = DoExtractTable("tbl_group_level_04", "tbl_group_level_04_work", connectionString)
                                        If success Then
                                            DoUpdateTable_tbl_group_level_04(connectionString, _QSBusinessUnit)
                                            success = DoExtractTable("tbl_group_level_05", "tbl_group_level_05_work", connectionString)
                                            If success Then
                                                DoUpdateTable_tbl_group_level_05(connectionString, _QSBusinessUnit)
                                                success = DoExtractTable("tbl_group_level_06", "tbl_group_level_06_work", connectionString)
                                                If success Then
                                                    DoUpdateTable_tbl_group_level_06(connectionString, _QSBusinessUnit)
                                                    success = DoExtractTable("tbl_group_level_07", "tbl_group_level_07_work", connectionString)
                                                    If success Then
                                                        DoUpdateTable_tbl_group_level_07(connectionString, _QSBusinessUnit)
                                                        success = DoExtractTable("tbl_group_level_08", "tbl_group_level_08_work", connectionString)
                                                        If success Then
                                                            DoUpdateTable_tbl_group_level_08(connectionString, _QSBusinessUnit)
                                                            success = DoExtractTable("tbl_group_level_09", "tbl_group_level_09_work", connectionString)
                                                            If success Then
                                                                DoUpdateTable_tbl_group_level_09(connectionString, _QSBusinessUnit)
                                                                success = DoExtractTable("tbl_group_level_10", "tbl_group_level_10_work", connectionString)
                                                                If success Then
                                                                    DoUpdateTable_tbl_group_level_10(connectionString, _QSBusinessUnit)
                                                                    success = True
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            'Product Relations Tables
            If boolProductRelations Then
                success = DoExtractTable("tbl_product_relations", "tbl_product_relations_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_product_relations(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_product_relations_attribute_values", "tbl_product_relations_attribute_values_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_product_relations_attribute_values(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_product_relations_defaults", "tbl_product_relations_defaults_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_product_relations_defaults(connectionString, _QSBusinessUnit)
                            success = DoExtractTable("tbl_product_relations_text_lang", "tbl_product_relations_text_lang_work", connectionString)
                            If success Then
                                DoUpdateTable_tbl_product_relations_text_lang(connectionString, _QSBusinessUnit)
                                success = True
                            End If
                        End If
                    End If
                End If
            End If

            'Product Personalisation
            If boolProductPersonalisation Then
                success = DoExtractTable("tbl_product_personalisation", "tbl_product_personalisation_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_product_personalisation(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_product_personalisation_component", "tbl_product_personalisation_component_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_product_personalisation_component(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_product_personalisation_xref", "tbl_product_personalisation_xref_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_product_personalisation_xref(connectionString, _QSBusinessUnit)
                            success = True
                        End If
                    End If
                End If
            End If

            'Promotions
            If boolPromotions Then
                success = DoExtractTable("tbl_promotions", "tbl_promotions_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_promotions(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_promotions_discounts", "tbl_promotions_discounts_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_promotions_discounts(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_promotions_free_products", "tbl_promotions_free_products_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_promotions_free_products(connectionString, _QSBusinessUnit)
                            success = DoExtractTable("tbl_promotions_lang", "tbl_promotions_lang_work", connectionString)
                            If success Then
                                DoUpdateTable_tbl_promotions_lang(connectionString, _QSBusinessUnit)
                                success = DoExtractTable("tbl_promotions_required_products", "tbl_promotions_required_products_work", connectionString)
                                If success Then
                                    DoUpdateTable_tbl_promotions_required_products(connectionString, _QSBusinessUnit)
                                    success = True
                                End If
                            End If
                        End If
                    End If
                End If
            End If


            'Page Content Tables
            If boolPageContent Then
                success = DoExtractTable("tbl_page", "tbl_page_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_page(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_page_lang", "tbl_page_lang_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_page_lang(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_page_html", "tbl_page_html_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_page_html(connectionString, _QSBusinessUnit)
                            success = DoExtractTable("tbl_template_page", "tbl_template_page_work", connectionString)
                            If success Then
                                DoUpdateTable_tbl_template_page(connectionString, _QSBusinessUnit)
                                success = DoExtractTable("tbl_page_text_lang", "tbl_page_text_lang_work", connectionString)
                                If success Then
                                    DoUpdateTable_tbl_page_text_lang(connectionString, _QSBusinessUnit)
                                    success = DoExtractTable("tbl_page_attribute", "tbl_page_attribute_work", connectionString)
                                    If success Then
                                        DoUpdateTable_tbl_page_attribute(connectionString, _QSBusinessUnit)
                                        success = True
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            'Control Content Tables
            If boolControlContent Then
                success = DoExtractTable("tbl_control_attribute", "tbl_control_attribute_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_control_attribute(connectionString, "", _QSBusinessUnit)
                    success = DoExtractTable("tbl_control_text_lang", "tbl_control_text_lang_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_control_text_lang(connectionString, "", _QSBusinessUnit)
                        success = True
                    End If
                End If
            End If

            'Flash Tables
            If boolFlashContent Then
                success = DoExtractTable("tbl_flash_settings", "tbl_flash_settings_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_flash_settings(connectionString)
                    success = True
                End If
            End If

            'Ticketing Menus/Modules
            If boolTicketingMenus Then
                success = DoExtractTable("tbl_ticketing_products", "tbl_ticketing_products_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_ticketing_products(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_ticketing_products_lang", "tbl_ticketing_products_lang_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_ticketing_products_lang(connectionString, _QSBusinessUnit)
                        success = True
                    End If
                End If
            End If

            'Page Tracking
            If boolPageTracking Then
                success = DoExtractTable("tbl_tracking_providers", "tbl_tracking_providers_work", connectionString)
                If success Then
                    DoUpdateTable_tbl_tracking_providers(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_page_tracking", "tbl_page_tracking_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_page_tracking(connectionString, _QSBusinessUnit)
                        success = DoExtractTable("tbl_tracking_settings_values", "tbl_tracking_settings_values_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_tracking_settings_values(connectionString, _QSBusinessUnit)
                            success = DoExtractTable("tbl_tracking_user_details", "tbl_tracking_user_details_work", connectionString)
                            If success Then
                                DoUpdateTable_tbl_tracking_user_details(connectionString, _QSBusinessUnit)
                                success = True
                            End If
                        End If
                    End If
                End If
            End If

            'Email Templates
            If boolEmailTemplates Then
                Dim sIdentityField As String = "EMAILTEMPLATE_ID"
                Dim sSubClauseAttr As String = " AND Isnull(tbl_control_attribute_work.CONTROL_CODE,'') COLLATE DATABASE_DEFAULT LIKE 'TalentEmail.vb.%'"
                Dim sSubClauseControl As String = " AND Isnull(tbl_control_text_lang_work.CONTROL_CODE,'') COLLATE DATABASE_DEFAULT LIKE 'TalentEmail.vb.%'"
                success = DoExtractTable("tbl_email_templates", "tbl_email_templates_work", connectionString, sIdentityField)
                If success Then
                    DoUpdateTable_tbl_email_templates(connectionString, _QSBusinessUnit)
                    success = DoExtractTable("tbl_control_attribute", "tbl_control_attribute_work", connectionString)
                    If success Then
                        DoUpdateTable_tbl_control_attribute(connectionString, sSubClauseAttr, _QSBusinessUnit)
                        success = DoExtractTable("tbl_control_text_lang", "tbl_control_text_lang_work", connectionString)
                        If success Then
                            DoUpdateTable_tbl_control_text_lang(connectionString, sSubClauseControl, _QSBusinessUnit)
                            success = True
                        End If
                    End If
                End If
            End If

            'Alerts
            If boolAlerts Then
                Dim sIdentityField As String = "ID"
                ' Populate the works tables for definition and critera
                success = DoExtractTable("tbl_alert_definition", "tbl_alert_definition_work", connectionString, sIdentityField)
                If success Then
                    success = DoExtractTable("tbl_alert_critera", "tbl_alert_critera_work", connectionString, sIdentityField)
                    If success Then
                        ' Then perform the jiggory to delete tbl_alerts records - this requires work tables to be populated AND real tables before updates from work.
                        DoUpdateTable_tbl_alerts(connectionString, _QSBusinessUnit)
                        ' Then perfoorm updates to defintiona nd critera tables from work tables
                        DoUpdateTable_tbl_alert_definition(connectionString, _QSBusinessUnit)
                        DoUpdateTable_tbl_alert_critera(connectionString)
                        ' Then standard order for tbl_attribute_defintion
                        success = DoExtractTable("tbl_attribute_definition", "tbl_attribute_definition_work", connectionString, sIdentityField)
                        If success Then
                            DoUpdateTable_tbl_attribute_definition(connectionString, _QSBusinessUnit)
                            success = True
                        End If
                    End If
                End If
            End If

            'Activities
            If boolActivities Then
                success = DoExtractTable("tbl_activity_questions", "tbl_activity_questions_work", connectionString, "QUESTION_ID")
                If success Then
                    DoUpdateTable_tbl_activity_questions(connectionString)
                    success = DoExtractTable("tbl_activity_questions_answer_categories", "tbl_activity_questions_answer_categories_work", connectionString, "CATEGORY_ID")
                    If success Then
                        DoUpdateTable_tbl_activity_questions_answer_categories(connectionString)
                        success = DoExtractTable("tbl_activity_questions_answers", "tbl_activity_questions_answers_work", connectionString, "ANSWER_ID")
                        If success Then
                            DoUpdateTable_tbl_activity_questions_answers(connectionString)
                            success = DoExtractTable("tbl_activity_templates", "tbl_activity_templates_work", connectionString, "TEMPLATE_ID")
                            If success Then
                                DoUpdateTable_tbl_activity_templates(connectionString, _QSBusinessUnit)
                                success = DoExtractTable("tbl_activity_templates_detail", "tbl_activity_templates_detail_work", connectionString, "ID")
                                If success Then
                                    DoUpdateTable_tbl_activity_templates_detail(connectionString)
                                    success = DoExtractTable("tbl_activity_questions_with_answers", "tbl_activity_questions_with_answers_work", connectionString, "ID")
                                    If success Then
                                        DoUpdateTable_tbl_activity_questions_with_answers(connectionString)
                                        success = DoExtractTable("tbl_activity_status", "tbl_activity_status_work", connectionString, "ID")
                                        If success Then
                                            DoUpdateTable_tbl_activity_status(connectionString)
                                            success = DoExtractTable("tbl_activity_status_description", "tbl_activity_status_description_work", connectionString, "STATUS_ID")
                                            If success Then
                                                DoUpdateTable_tbl_activity_status_description(connectionString)
                                                success = DoExtractTable("tbl_activity_template_type", "tbl_activity_template_type_work", connectionString, "TYPE_ID")
                                                If success Then
                                                    DoUpdateTable_tbl_activity_template_type(connectionString)
                                                    success = True
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            If Not success Then
                _errorMsg = "An error has occurred during the publish of the changes. The process has stopped. Please report this issue to our support team."
            End If
        Catch ex As Exception
            _errorMsg = "Error: " + ex.Message.ToString
            success = False
        End Try
        Return success
    End Function

    Private Sub populateUrlList(ByRef urlList As Collection)
        Dim talentAdminUrl As String = "www.{0}.{1}.talent-admin.co.uk"
        Dim talentAdminUrlwp2 As String = "www.{0}.{1}.talent-admin.co.uk"
        Dim clientName As String = getClientName()
        Dim tDataObjects = New TalentDataObjects()
        Dim talAmdinDataObjects = New TalentAdminDataObjects()
        Dim TalentAdminSettings As DESettings = New DESettings()
        Dim localSettings As DESettings = New DESettings()

        localSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        localSettings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        tDataObjects.Settings = localSettings

        TalentAdminSettings.FrontEndConnectionString = localSettings.FrontEndConnectionString
        TalentAdminSettings.BackOfficeConnectionString = tDataObjects.AppVariableSettings.TblDatabaseVersion.TalentAdminDatabaseConnectionString()
        TalentAdminSettings.DestinationDatabase = GlobalConstants.TALENTADMINDESTINATIONDATABASE
        TalentAdminSettings.BusinessUnit = Request.QueryString("BU")
        talAmdinDataObjects.Settings = TalentAdminSettings

        Dim dtWebServers As New DataTable
        If Request.Url.AbsoluteUri.Contains("test") Then
            dtWebServers = talAmdinDataObjects.TalentAdminSettings.TblClientWebServers.GetWebServerDataByClientName(clientName, "Test")
            talentAdminUrl = "test.{0}.test_{1}.talent-admin.co.uk"
        Else
            dtWebServers = talAmdinDataObjects.TalentAdminSettings.TblClientWebServers.GetWebServerDataByClientName(clientName, "Live")
        End If
        For Each row As DataRow In dtWebServers.Rows
            If Not row("SQL_DATABASE_NAME").ToString.StartsWith("STAGE") Then
                urlList.Add(String.Format(talentAdminUrl, row("SERVER_NAME").ToString(), clientName))
            End If
        Next
        tDataObjects = Nothing
    End Sub

    Private Function getClientName() As String
        Dim clientName As String = String.Empty
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = Request.QueryString("BU")
        tDataObjects.Settings = settings
        clientName = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetDefaultNameValue(settings.BusinessUnit, GlobalConstants.STARALLPARTNER, "TALENT_ADMIN_CLIENT_NAME")
        tDataObjects = Nothing
        Return clientName
    End Function

    Private Function clearCacheForUrl(ByVal url As String, ByRef exceptionString As String) As Boolean
        Dim cacheCleared As Boolean = False
        Try
            url = formatUrl(url)
            Dim request As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = 0
            Dim stream As Stream = request.GetResponse().GetResponseStream()
            Dim reader As New StreamReader(stream)
            Dim response As String = String.Empty
            Dim postData As String = String.Empty
            Dim viewState As String = String.Empty
            While Not reader.EndOfStream
                response += reader.ReadLine()
            End While
            reader.Close()

            If response.Contains("ctl00$ContentPlaceHolder1$btnClearCache") Then
                Dim request2 As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
                request2.Method = "POST"
                request2.ContentType = "application/x-www-form-urlencoded"
                viewState = extractViewState(response)
                postData = "__VIEWSTATE=" & viewState & "&" & "ctl00$ContentPlaceHolder1$btnClearCache=Clear Cache"
                Dim data() As Byte = Encoding.ASCII.GetBytes(postData)
                request2.ContentLength = data.Length
                Dim newStream As Stream = request2.GetRequestStream()
                newStream.Write(data, 0, data.Length)
                newStream.Close()
                cacheCleared = True
            Else
                exceptionString = _wfrPage.Content("URLResponseError", _languageCode, True)
            End If
        Catch ex As Exception
            exceptionString = ex.Message
        End Try
        Return cacheCleared
    End Function

    Private Function formatUrl(ByVal url As String) As String
        Dim formattedUrl As String = String.Empty
        If Not url.StartsWith("http") Then
            formattedUrl = "http://" & url
        End If
        If formattedUrl.EndsWith("/") Then
            formattedUrl = formattedUrl & "PagesAdmin/ClearCache.aspx"
        Else
            formattedUrl = formattedUrl & "/PagesAdmin/ClearCache.aspx"
        End If
        Return formattedUrl
    End Function

    Private Function extractViewState(ByVal s As String) As String
        Dim viewStateNameDelimiter As String = "__VIEWSTATE"
        Dim valueDelimiter As String = "value="""
        Dim viewStateNamePosition As Integer = s.IndexOf(viewStateNameDelimiter)
        Dim viewStateValuePosition As Integer = s.IndexOf(valueDelimiter, viewStateNamePosition)
        Dim viewStateStartPosition As Integer = viewStateValuePosition + valueDelimiter.Length
        Dim viewStateEndPosition As Integer = s.IndexOf("""", viewStateStartPosition)
        Return HttpUtility.UrlEncode(s.Substring(viewStateStartPosition, viewStateEndPosition - viewStateStartPosition))
    End Function

#End Region

#Region "Private Methods"

    Private Function SaveLastUpdated(ByVal DateType As String, ByVal sDate As String) As Boolean

        Dim ret As Boolean = False

        Dim tDataObjects = New TalentDataObjects()
        Dim settings As Talent.Common.DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        tDataObjects.Settings = settings

        Dim affectedRows As Integer = 0
        affectedRows = tDataObjects.MaintenanceSettings.TblMaintenancePublishTypes.UpdateLastUpdated(_QSBusinessUnit, _QSPartner, DateType, sDate)
        If affectedRows > 0 Then
            ret = True
        End If

        Return ret

        ''DateType = DateType.Replace(" ", "")
        ''Dim myXml As New XmlDocument
        ''myXml.Load(Server.MapPath("RollOutLastUpdated.xml"))
        ''Dim myNode As XmlNode = myXml.SelectSingleNode("Updates")
        ''If myNode.HasChildNodes Then
        ''    'Select Case LCase(DateType)
        ''    '    Case "html"
        ''    '        myNode.SelectSingleNode("HtmlUpload").InnerText = myDate.ToString("dd/MM/yyyy HH:mm:ss")
        ''    '    Case "images"
        ''    '        myNode.SelectSingleNode("ImagesUpload").InnerText = myDate.ToString("dd/MM/yyyy HH:mm:ss")
        ''    '    Case "cms"
        ''    '        myNode.SelectSingleNode("CMSUpload").InnerText = myDate.ToString("dd/MM/yyyy HH:mm:ss")
        ''    'End Select
        ''    myNode.SelectSingleNode(DateType).InnerText = myDate.ToString("dd/MM/yyyy HH:mm:ss")
        ''End If
        ''myXml.Save(Server.MapPath("RollOutLastUpdated.xml"))
        ' ''DisplayLastUpdatedDate()
    End Function

    Private Function DoExtractTable(ByVal sourceTable As String, ByVal extractTable As String, ByVal targetConnectionString As String, Optional ByVal sIdentityField As String = "") As Boolean
        Dim hasError As Boolean = False
        Dim success As Boolean = True
        '======================================================
        ' DELETE ALL records from the EXTRACT Table
        '======================================================
        Dim sbDelete As New StringBuilder
        With sbDelete
            .Append("Delete from ")
            .Append(extractTable)
        End With

        Using con As New SqlConnection(targetConnectionString)
            Dim cmd As New SqlCommand(sbDelete.ToString(), con)
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        '======================================================
        ' COPY the SOURCE table to the EXTRACT table
        '======================================================
        Dim sbSelect As New StringBuilder
        sbSelect = buildExtractSQL(sourceTable, extractTable)

        ' Initialise the number of records copied to zero
        Dim RecordsCopied As Integer = 0

        ' Initialise Connection Objects
        Dim sqlTarget As SqlConnection = New SqlConnection(targetConnectionString)
        Dim sqlSource As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        ' Initialise Command Objects
        Dim sqlTargetCmd As SqlCommand = New SqlCommand()
        Dim sqlSourceCmd As SqlCommand = New SqlCommand()

        ' Initialise the Source Data Reader
        Dim sqlSourceDatareader As SqlDataReader

        ' Attach the connection objects to the commands
        sqlTargetCmd.Connection = sqlTarget
        sqlSourceCmd.Connection = sqlSource

        ' Set Source Command SQL
        sqlSourceCmd.CommandText = sbSelect.ToString()
        sqlSourceCmd.CommandType = CommandType.Text

        ' Open the Source Connection
        sqlSourceCmd.Connection.Open()
        Dim SQLBulkOp As SqlBulkCopy
        If sIdentityField = "" Then
            SQLBulkOp = New SqlBulkCopy(targetConnectionString, SqlBulkCopyOptions.UseInternalTransaction)
        Else
            SQLBulkOp = New SqlBulkCopy(targetConnectionString, SqlBulkCopyOptions.KeepIdentity)
        End If

        ' Set Destination Table Name and Timeout
        SQLBulkOp.DestinationTableName = extractTable
        SQLBulkOp.BulkCopyTimeout = 500000000

        ' Execute the From Reader
        sqlSourceDatareader = sqlSourceCmd.ExecuteReader()
        Try
            SQLBulkOp.WriteToServer(sqlSourceDatareader)
        Catch ex As Exception
            hasError = True
            _talentLogging.ExceptionLog("UploadAndPublish.aspx|DoExtractTable - From: " & sourceTable & " To: " & extractTable & " With query: " & sbSelect.ToString(), ex.Message)
        Finally
            sqlSourceDatareader.Close()
            sqlSourceCmd.Connection.Close()
        End Try

        If hasError Then
            success = False
        End If

        Return success
    End Function
    Private Shared Function buildExtractSQL(ByVal SourceTable As String, ByVal ExtractTable As String) As StringBuilder
        Dim sbExtract As New StringBuilder
        With sbExtract
            Select Case SourceTable

                Case Is = "tbl_ecommerce_module_defaults_bu"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, APPLICATION, MODULE, DEFAULT_NAME, VALUE ")
                Case Is = "tbl_product"
                    .Append("SELECT 0,PRODUCT_CODE,PRODUCT_DESCRIPTION_1,PRODUCT_DESCRIPTION_2,PRODUCT_DESCRIPTION_3,PRODUCT_DESCRIPTION_4,PRODUCT_DESCRIPTION_5,PRODUCT_LENGTH,PRODUCT_LENGTH_UOM,PRODUCT_WIDTH,PRODUCT_WIDTH_UOM,PRODUCT_DEPTH,PRODUCT_DEPTH_UOM,PRODUCT_HEIGHT,PRODUCT_HEIGHT_UOM,PRODUCT_SIZE,PRODUCT_SIZE_UOM,PRODUCT_WEIGHT,PRODUCT_WEIGHT_UOM,PRODUCT_VOLUME,PRODUCT_VOLUME_UOM,PRODUCT_COLOUR,PRODUCT_PACK_SIZE,PRODUCT_PACK_SIZE_UOM,PRODUCT_SUPPLIER_PART_NO,PRODUCT_CUSTOMER_PART_NO,PRODUCT_TASTING_NOTES_1,PRODUCT_TASTING_NOTES_2,PRODUCT_ABV,PRODUCT_VINTAGE,PRODUCT_SUPPLIER,PRODUCT_COUNTRY,PRODUCT_REGION,PRODUCT_AREA,PRODUCT_GRAPE,PRODUCT_CLOSURE,PRODUCT_CATALOG_CODE,PRODUCT_VEGETARIAN,PRODUCT_VEGAN,PRODUCT_ORGANIC,PRODUCT_BIODYNAMIC,PRODUCT_LUTTE,PRODUCT_MINIMUM_AGE, ")
                    .Append("PRODUCT_HTML_1, PRODUCT_HTML_2, PRODUCT_HTML_3, PRODUCT_SEARCH_KEYWORDS, PRODUCT_PAGE_TITLE, PRODUCT_META_DESCRIPTION, PRODUCT_META_KEYWORDS, PRODUCT_SEARCH_RANGE_01, PRODUCT_SEARCH_RANGE_02, PRODUCT_SEARCH_RANGE_03, PRODUCT_SEARCH_RANGE_04, PRODUCT_SEARCH_RANGE_05, PRODUCT_SEARCH_CRITERIA_01, PRODUCT_SEARCH_CRITERIA_02, PRODUCT_SEARCH_CRITERIA_03, PRODUCT_SEARCH_CRITERIA_04, PRODUCT_SEARCH_CRITERIA_05, PRODUCT_SEARCH_CRITERIA_06, PRODUCT_SEARCH_CRITERIA_07, PRODUCT_SEARCH_CRITERIA_08, PRODUCT_SEARCH_CRITERIA_09, PRODUCT_SEARCH_CRITERIA_10, PRODUCT_SEARCH_CRITERIA_11, PRODUCT_SEARCH_CRITERIA_12, PRODUCT_SEARCH_CRITERIA_13, PRODUCT_SEARCH_CRITERIA_14, PRODUCT_SEARCH_CRITERIA_15, PRODUCT_SEARCH_CRITERIA_16, PRODUCT_SEARCH_CRITERIA_17, PRODUCT_SEARCH_CRITERIA_18, PRODUCT_SEARCH_CRITERIA_19, PRODUCT_SEARCH_CRITERIA_20, ")
                    .Append("PRODUCT_SEARCH_SWITCH_01, PRODUCT_SEARCH_SWITCH_02, PRODUCT_SEARCH_SWITCH_03, PRODUCT_SEARCH_SWITCH_04, PRODUCT_SEARCH_SWITCH_05, PRODUCT_SEARCH_SWITCH_06, PRODUCT_SEARCH_SWITCH_07, PRODUCT_SEARCH_SWITCH_08, PRODUCT_SEARCH_SWITCH_09, PRODUCT_SEARCH_SWITCH_10, PRODUCT_SEARCH_DATE_01, PRODUCT_SEARCH_DATE_02, PRODUCT_SEARCH_DATE_03, PRODUCT_SEARCH_DATE_04, PRODUCT_SEARCH_DATE_05, PRODUCT_TARIFF_CODE, PRODUCT_OPTION_MASTER, ALTERNATE_SKU, AVAILABLE_ONLINE, PERSONALISABLE, DISCONTINUED")
                Case Is = "tbl_product_stock"
                    .Append("SELECT 0,PRODUCT,STOCK_LOCATION,QUANTITY,ALLOCATED_QUANTITY,AVAILABLE_QUANTITY,RESTOCK_CODE,WAREHOUSE")
                Case Is = "tbl_product_option_defaults"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,MASTER_PRODUCT,OPTION_TYPE,MATCH_ACTION,IS_DEFAULT,APPEND_SEQUENCE,DISPLAY_SEQUENCE,DISPLAY_TYPE")
                Case Is = "tbl_product_option_definitions"
                    .Append("SELECT 0,OPTION_CODE,DESCRIPTION")
                Case Is = "tbl_product_option_definitions_lang"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,OPTION_CODE,LANGUAGE_CODE,DISPLAY_NAME")
                Case Is = "tbl_product_option_types"
                    .Append("SELECT 0,OPTION_TYPE,DESCRIPTION")
                Case Is = "tbl_product_option_types_lang"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,OPTION_TYPE,LANGUAGE_CODE,DISPLAY_NAME,LABEL_TEXT")
                Case Is = "tbl_product_options"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,MASTER_PRODUCT,OPTION_TYPE,OPTION_CODE,PRODUCT_CODE,DISPLAY_ORDER")

                Case Is = "tbl_group"
                    .Append("SELECT 0,GROUP_NAME,GROUP_DESCRIPTION_1,GROUP_DESCRIPTION_2,GROUP_HTML_1,GROUP_HTML_2,GROUP_HTML_3,GROUP_PAGE_TITLE,GROUP_META_DESCRIPTION,GROUP_META_KEYWORDS,GROUP_ADHOC")
                Case Is = "tbl_group_product"
                    .Append("SELECT 0,GROUP_BUSINESS_UNIT,GROUP_PARTNER,GROUP_L01_GROUP,GROUP_L02_GROUP,GROUP_L03_GROUP,GROUP_L04_GROUP,GROUP_L05_GROUP,GROUP_L06_GROUP,GROUP_L07_GROUP,GROUP_L08_GROUP,GROUP_L09_GROUP,GROUP_L10_GROUP,PRODUCT,SEQUENCE,GROUP_ADHOC")
                Case Is = "tbl_group_lang"
                    .Append("SELECT 0,GROUP_CODE,GROUP_LANGUAGE,GROUP_DESCRIPTION_1,GROUP_DESCRIPTION_2,GROUP_HTML_1,GROUP_HTML_2,GROUP_HTML_3,GROUP_PAGE_TITLE,GROUP_META_DESCRIPTION,GROUP_META_KEYWORDS")
                Case Is = "tbl_group_level_01"
                    .Append("SELECT 0,GROUP_L01_BUSINESS_UNIT,GROUP_L01_PARTNER,GROUP_L01_L01_GROUP,GROUP_L01_SEQUENCE,GROUP_L01_DESCRIPTION_1,GROUP_L01_DESCRIPTION_2,GROUP_L01_HTML_1,GROUP_L01_HTML_2,GROUP_L01_HTML_3,GROUP_L01_PAGE_TITLE,GROUP_L01_META_DESCRIPTION,GROUP_L01_META_KEYWORDS,GROUP_L01_ADV_SEARCH_TEMPLATE,GROUP_L01_PRODUCT_PAGE_TEMPLATE,GROUP_L01_PRODUCT_LIST_TEMPLATE,GROUP_L01_SHOW_CHILDREN_AS_GROUPS,GROUP_L01_SHOW_PRODUCTS_AS_LIST,GROUP_L01_SHOW_IN_NAVIGATION,GROUP_L01_SHOW_IN_GROUPED_NAV,GROUP_L01_HTML_GROUP,GROUP_L01_HTML_GROUP_TYPE,GROUP_L01_ADHOC_GROUP,GROUP_L01_THEME,GROUP_L01_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_02"
                    .Append("SELECT 0,GROUP_L02_BUSINESS_UNIT,GROUP_L02_PARTNER,GROUP_L02_L01_GROUP,GROUP_L02_L02_GROUP,GROUP_L02_SEQUENCE,GROUP_L02_DESCRIPTION_1,GROUP_L02_DESCRIPTION_2,GROUP_L02_HTML_1,GROUP_L02_HTML_2,GROUP_L02_HTML_3,GROUP_L02_PAGE_TITLE,GROUP_L02_META_DESCRIPTION,GROUP_L02_META_KEYWORDS,GROUP_L02_ADV_SEARCH_TEMPLATE,GROUP_L02_PRODUCT_PAGE_TEMPLATE,GROUP_L02_PRODUCT_LIST_TEMPLATE,GROUP_L02_SHOW_CHILDREN_AS_GROUPS,GROUP_L02_SHOW_PRODUCTS_AS_LIST,GROUP_L02_SHOW_IN_NAVIGATION,GROUP_L02_SHOW_IN_GROUPED_NAV,GROUP_L02_HTML_GROUP,GROUP_L02_HTML_GROUP_TYPE,GROUP_L02_ADHOC_GROUP,GROUP_L02_THEME,GROUP_L02_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_03"
                    .Append("SELECT 0,GROUP_L03_BUSINESS_UNIT,GROUP_L03_PARTNER,GROUP_L03_L01_GROUP,GROUP_L03_L02_GROUP,GROUP_L03_L03_GROUP,GROUP_L03_SEQUENCE,GROUP_L03_DESCRIPTION_1,GROUP_L03_DESCRIPTION_2,GROUP_L03_HTML_1,GROUP_L03_HTML_2,GROUP_L03_HTML_3,GROUP_L03_PAGE_TITLE,GROUP_L03_META_DESCRIPTION,GROUP_L03_META_KEYWORDS,GROUP_L03_ADV_SEARCH_TEMPLATE,GROUP_L03_PRODUCT_PAGE_TEMPLATE,GROUP_L03_PRODUCT_LIST_TEMPLATE,GROUP_L03_SHOW_CHILDREN_AS_GROUPS,GROUP_L03_SHOW_PRODUCTS_AS_LIST,GROUP_L03_SHOW_IN_NAVIGATION,GROUP_L03_SHOW_IN_GROUPED_NAV,GROUP_L03_HTML_GROUP,GROUP_L03_HTML_GROUP_TYPE,GROUP_L03_ADHOC_GROUP,GROUP_L03_THEME,GROUP_L03_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_04"
                    .Append("SELECT 0,GROUP_L04_BUSINESS_UNIT,GROUP_L04_PARTNER,GROUP_L04_L01_GROUP,GROUP_L04_L02_GROUP,GROUP_L04_L03_GROUP,GROUP_L04_L04_GROUP,GROUP_L04_SEQUENCE,GROUP_L04_DESCRIPTION_1,GROUP_L04_DESCRIPTION_2,GROUP_L04_HTML_1,GROUP_L04_HTML_2,GROUP_L04_HTML_3,GROUP_L04_PAGE_TITLE,GROUP_L04_META_DESCRIPTION,GROUP_L04_META_KEYWORDS,GROUP_L04_ADV_SEARCH_TEMPLATE,GROUP_L04_PRODUCT_PAGE_TEMPLATE,GROUP_L04_PRODUCT_LIST_TEMPLATE,GROUP_L04_SHOW_CHILDREN_AS_GROUPS,GROUP_L04_SHOW_PRODUCTS_AS_LIST,GROUP_L04_SHOW_IN_NAVIGATION,GROUP_L04_SHOW_IN_GROUPED_NAV,GROUP_L04_HTML_GROUP,GROUP_L04_HTML_GROUP_TYPE,GROUP_L04_ADHOC_GROUP,GROUP_L04_THEME,GROUP_L04_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_05"
                    .Append("SELECT 0,GROUP_L05_BUSINESS_UNIT,GROUP_L05_PARTNER,GROUP_L05_L01_GROUP,GROUP_L05_L02_GROUP,GROUP_L05_L03_GROUP,GROUP_L05_L04_GROUP,GROUP_L05_L05_GROUP,GROUP_L05_SEQUENCE,GROUP_L05_DESCRIPTION_1,GROUP_L05_DESCRIPTION_2,GROUP_L05_HTML_1,GROUP_L05_HTML_2,GROUP_L05_HTML_3,GROUP_L05_PAGE_TITLE,GROUP_L05_META_DESCRIPTION,GROUP_L05_META_KEYWORDS,GROUP_L05_ADV_SEARCH_TEMPLATE,GROUP_L05_PRODUCT_PAGE_TEMPLATE,GROUP_L05_PRODUCT_LIST_TEMPLATE,GROUP_L05_SHOW_CHILDREN_AS_GROUPS,GROUP_L05_SHOW_PRODUCTS_AS_LIST,GROUP_L05_SHOW_IN_NAVIGATION,GROUP_L05_SHOW_IN_GROUPED_NAV,GROUP_L05_HTML_GROUP,GROUP_L05_HTML_GROUP_TYPE,GROUP_L05_ADHOC_GROUP,GROUP_L05_THEME,GROUP_L05_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_06"
                    .Append("SELECT 0,GROUP_L06_BUSINESS_UNIT,GROUP_L06_PARTNER,GROUP_L06_L01_GROUP,GROUP_L06_L02_GROUP,GROUP_L06_L03_GROUP,GROUP_L06_L04_GROUP,GROUP_L06_L05_GROUP,GROUP_L06_L06_GROUP,GROUP_L06_SEQUENCE,GROUP_L06_DESCRIPTION_1,GROUP_L06_DESCRIPTION_2,GROUP_L06_HTML_1,GROUP_L06_HTML_2,GROUP_L06_HTML_3,GROUP_L06_PAGE_TITLE,GROUP_L06_META_DESCRIPTION,GROUP_L06_META_KEYWORDS,GROUP_L06_ADV_SEARCH_TEMPLATE,GROUP_L06_PRODUCT_PAGE_TEMPLATE,GROUP_L06_PRODUCT_LIST_TEMPLATE,GROUP_L06_SHOW_CHILDREN_AS_GROUPS,GROUP_L06_SHOW_PRODUCTS_AS_LIST,GROUP_L06_SHOW_IN_NAVIGATION,GROUP_L06_SHOW_IN_GROUPED_NAV,GROUP_L06_HTML_GROUP,GROUP_L06_HTML_GROUP_TYPE,GROUP_L06_ADHOC_GROUP,GROUP_L06_THEME,GROUP_L06_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_07"
                    .Append("SELECT 0,GROUP_L07_BUSINESS_UNIT,GROUP_L07_PARTNER,GROUP_L07_L01_GROUP,GROUP_L07_L02_GROUP,GROUP_L07_L03_GROUP,GROUP_L07_L04_GROUP,GROUP_L07_L05_GROUP,GROUP_L07_L06_GROUP,GROUP_L07_L07_GROUP,GROUP_L07_SEQUENCE,GROUP_L07_DESCRIPTION_1,GROUP_L07_DESCRIPTION_2,GROUP_L07_HTML_1,GROUP_L07_HTML_2,GROUP_L07_HTML_3,GROUP_L07_PAGE_TITLE,GROUP_L07_META_DESCRIPTION,GROUP_L07_META_KEYWORDS,GROUP_L07_ADV_SEARCH_TEMPLATE,GROUP_L07_PRODUCT_PAGE_TEMPLATE,GROUP_L07_PRODUCT_LIST_TEMPLATE,GROUP_L07_SHOW_CHILDREN_AS_GROUPS,GROUP_L07_SHOW_PRODUCTS_AS_LIST,GROUP_L07_SHOW_IN_NAVIGATION,GROUP_L07_SHOW_IN_GROUPED_NAV,GROUP_L07_HTML_GROUP,GROUP_L07_HTML_GROUP_TYPE,GROUP_L07_ADHOC_GROUP,GROUP_L07_THEME,GROUP_L07_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_08"
                    .Append("SELECT 0,GROUP_L08_BUSINESS_UNIT,GROUP_L08_PARTNER,GROUP_L08_L01_GROUP,GROUP_L08_L02_GROUP,GROUP_L08_L03_GROUP,GROUP_L08_L04_GROUP,GROUP_L08_L05_GROUP,GROUP_L08_L06_GROUP,GROUP_L08_L07_GROUP,GROUP_L08_L08_GROUP,GROUP_L08_SEQUENCE,GROUP_L08_DESCRIPTION_1,GROUP_L08_DESCRIPTION_2,GROUP_L08_HTML_1,GROUP_L08_HTML_2,GROUP_L08_HTML_3,GROUP_L08_PAGE_TITLE,GROUP_L08_META_DESCRIPTION,GROUP_L08_META_KEYWORDS,GROUP_L08_ADV_SEARCH_TEMPLATE,GROUP_L08_PRODUCT_PAGE_TEMPLATE,GROUP_L08_PRODUCT_LIST_TEMPLATE,GROUP_L08_SHOW_CHILDREN_AS_GROUPS,GROUP_L08_SHOW_PRODUCTS_AS_LIST,GROUP_L08_SHOW_IN_NAVIGATION,GROUP_L08_SHOW_IN_GROUPED_NAV,GROUP_L08_HTML_GROUP,GROUP_L08_HTML_GROUP_TYPE,GROUP_L08_ADHOC_GROUP,GROUP_L08_THEME,GROUP_L08_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_09"
                    .Append("SELECT 0,GROUP_L09_BUSINESS_UNIT,GROUP_L09_PARTNER,GROUP_L09_L01_GROUP,GROUP_L09_L02_GROUP,GROUP_L09_L03_GROUP,GROUP_L09_L04_GROUP,GROUP_L09_L05_GROUP,GROUP_L09_L06_GROUP,GROUP_L09_L07_GROUP,GROUP_L09_L08_GROUP,GROUP_L09_L09_GROUP,GROUP_L09_SEQUENCE,GROUP_L09_DESCRIPTION_1,GROUP_L09_DESCRIPTION_2,GROUP_L09_HTML_1,GROUP_L09_HTML_2,GROUP_L09_HTML_3,GROUP_L09_PAGE_TITLE,GROUP_L09_META_DESCRIPTION,GROUP_L09_META_KEYWORDS,GROUP_L09_ADV_SEARCH_TEMPLATE,GROUP_L09_PRODUCT_PAGE_TEMPLATE,GROUP_L09_PRODUCT_LIST_TEMPLATE,GROUP_L09_SHOW_CHILDREN_AS_GROUPS,GROUP_L09_SHOW_PRODUCTS_AS_LIST,GROUP_L09_SHOW_IN_NAVIGATION,GROUP_L09_SHOW_IN_GROUPED_NAV,GROUP_L09_HTML_GROUP,GROUP_L09_HTML_GROUP_TYPE,GROUP_L09_ADHOC_GROUP,GROUP_L09_THEME,GROUP_L09_SHOW_PRODUCT_DISPLAY")
                Case Is = "tbl_group_level_10"
                    .Append("SELECT 0,GROUP_L10_BUSINESS_UNIT,GROUP_L10_PARTNER,GROUP_L10_L01_GROUP,GROUP_L10_L02_GROUP,GROUP_L10_L03_GROUP,GROUP_L10_L04_GROUP,GROUP_L10_L05_GROUP,GROUP_L10_L06_GROUP,GROUP_L10_L07_GROUP,GROUP_L10_L08_GROUP,GROUP_L10_L09_GROUP,GROUP_L10_L10_GROUP,GROUP_L10_SEQUENCE,GROUP_L10_DESCRIPTION_1,GROUP_L10_DESCRIPTION_2,GROUP_L10_HTML_1,GROUP_L10_HTML_2,GROUP_L10_HTML_3,GROUP_L10_PAGE_TITLE,GROUP_L10_META_DESCRIPTION,GROUP_L10_META_KEYWORDS,GROUP_L10_ADV_SEARCH_TEMPLATE,GROUP_L10_PRODUCT_PAGE_TEMPLATE,GROUP_L10_PRODUCT_LIST_TEMPLATE,GROUP_L10_SHOW_CHILDREN_AS_GROUPS,GROUP_L10_SHOW_PRODUCTS_AS_LIST,GROUP_L10_SHOW_IN_NAVIGATION,GROUP_L10_SHOW_IN_GROUPED_NAV,GROUP_L10_HTML_GROUP,GROUP_L10_HTML_GROUP_TYPE,GROUP_L10_ADHOC_GROUP,GROUP_L10_THEME,GROUP_L10_SHOW_PRODUCT_DISPLAY")

                Case Is = "tbl_product_personalisation"
                    .Append("SELECT 0,XML_CONFIG")
                Case Is = "tbl_product_personalisation_component"
                    .Append("SELECT 0,PRODUCT_PERSONALISATION_ID,LINKED_PRODUCT_CODE,QTY_RULE,COMP_TYPE,COMP_NAME,COMP_TEXT,COMP_FORCE_CASE,COMP_LABEL,COMP_FONT,COMP_FONT_COLOR,COMP_SIZE,COMP_MAX_CHARS,COMP_ARC_AMOUNT,COMP_ARC_ROTATION,COMP_ARC_WIDTH,COMP_POS_AT_PERCENT_WIDTH,COMP_POS_AT_PERCENT_HEIGHT,COMP_RESTRICT_INPUT,COMP_USE_IMAGE_TEXT")
                Case Is = "tbl_product_personalisation_xref"
                    .Append("SELECT PRODUCT_CODE,PRODUCT_PERSONALISATION_ID")

                Case Is = "tbl_product_relations"
                    .Append("SELECT 0, QUALIFIER, BUSINESS_UNIT, PARTNER, GROUP_L01_GROUP, GROUP_L02_GROUP, GROUP_L03_GROUP, GROUP_L04_GROUP, ")
                    .Append("GROUP_L05_GROUP, GROUP_L06_GROUP, GROUP_L07_GROUP, GROUP_L08_GROUP, GROUP_L09_GROUP, GROUP_L10_GROUP, ")
                    .Append("PRODUCT, RELATED_GROUP_L01_GROUP, RELATED_GROUP_L02_GROUP, RELATED_GROUP_L03_GROUP, RELATED_GROUP_L04_GROUP, ")
                    .Append("RELATED_GROUP_L05_GROUP, RELATED_GROUP_L06_GROUP, RELATED_GROUP_L07_GROUP, RELATED_GROUP_L08_GROUP, RELATED_GROUP_L09_GROUP, RELATED_GROUP_L10_GROUP, ")
                    .Append("RELATED_PRODUCT, SEQUENCE, TICKETING_PRODUCT_TYPE, TICKETING_PRODUCT_SUB_TYPE, RELATED_TICKETING_PRODUCT_TYPE, RELATED_TICKETING_PRODUCT_SUB_TYPE, ")
                    .Append("LINK_TYPE, TICKETING_PRODUCT_PRICE_CODE, RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE, RELATED_PRODUCT_MANDATORY, RELATED_TICKETING_PRODUCT_PRICE_CODE, ")
                    .Append("RELATED_TICKETING_PRODUCT_PRICE_BAND, RELATED_TICKETING_PRODUCT_PRICE_BAND_READONLY, RELATED_TICKETING_PRODUCT_STAND, RELATED_TICKETING_PRODUCT_STAND_READONLY, ")
                    .Append("RELATED_TICKETING_PRODUCT_AREA, RELATED_TICKETING_PRODUCT_AREA_READONLY, RELATED_TICKETING_PRODUCT_QTY, RELATED_TICKETING_PRODUCT_QTY_MIN, ")
                    .Append("RELATED_TICKETING_PRODUCT_QTY_MAX, RELATED_TICKETING_PRODUCT_QTY_READONLY, RELATED_TICKETING_PRODUCT_QTY_RATIO, RELATED_TICKETING_PRODUCT_QTY_ROUND_UP, ")
                    .Append("RELATED_TICKETING_PRODUCT_PRICE_BAND_VISIBLE, RELATED_CSS_CLASS, RELATED_INSTRUCTIONS, FOREIGN_PRODUCT_RELATIONS_ID, PACKAGE_COMPONENT_VALUE_01, PACKAGE_COMPONENT_VALUE_02, ")
                    .Append("PACKAGE_COMPONENT_VALUE_03, PACKAGE_COMPONENT_VALUE_04, PACKAGE_COMPONENT_VALUE_05, PACKAGE_COMPONENT_PRICE_BANDS")
                Case Is = "tbl_product_relations_attribute_values"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,PAGE_CODE,QUALIFIER,TEMPLATE_TYPE,PAGE_POSITION,ATTRIBUTE_CODE,ATTRIBUTE_VALUE")
                Case Is = "tbl_product_relations_defaults"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,PAGE_CODE,QUALIFIER,TEMPLATE_TYPE,PAGE_POSITION,ONOFF,SEQUENCE")
                Case Is = "tbl_product_relations_text_lang"
                    .Append("SELECT 0,BUSINESS_UNIT,PARTNER,PAGE_CODE,QUALIFIER,TEMPLATE_TYPE,PAGE_POSITION,LANGUAGE_CODE,TEXT_CODE,TEXT_VALUE")

                Case Is = "tbl_page"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, DESCRIPTION, PAGE_QUERYSTRING, USE_SECURE_URL, HTML_IN_USE, ")
                    .Append("PAGE_TYPE, SHOW_PAGE_HEADER, BCT_URL, BCT_PARENT, FORCE_LOGIN, IN_USE, CSS_PRINT, HIDE_IN_MAINTENANCE, ALLOW_GENERIC_SALES, RESTRICTING_ALERT_NAME, BODY_CSS_CLASS")
                Case Is = "tbl_page_lang"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, LANGUAGE_CODE, TITLE, META_KEY, META_DESC, PAGE_HEADER")
                Case Is = "tbl_page_html"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, PAGE_CODE, PAGE_QUERYSTRING, SECTION, SEQUENCE, HTML_1, HTML_2, HTML_3, HTML_LOCATION")
                Case Is = "tbl_template_page"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, PAGE_NAME, TEMPLATE_NAME")
                Case Is = "tbl_page_text_lang"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, TEXT_CONTENT")
                Case Is = "tbl_page_attribute"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, ATTR_NAME, ATTR_VALUE, DESCRIPTION")

                Case Is = "tbl_control_text_lang"
                    .Append("SELECT 0, LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT")
                Case Is = "tbl_control_attribute"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, CONTROL_CODE, ATTR_NAME, ATTR_VALUE, DESCRIPTION")
                Case Is = "tbl_flash_settings"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER_CODE, PAGE_CODE, SEQUENCE, ATTRIBUTE_NAME, ATTRIBUTE_VALUE, QUERYSTRING_PARAMETER")

                Case Is = "tbl_ticketing_products"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, PRODUCT_TYPE, DISPLAY_SEQUENCE, ACTIVE, LOCATION")
                Case Is = "tbl_ticketing_products_lang"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, PRODUCT_TYPE, LANGUAGE_CODE, DISPLAY_CONTENT, CSS_CLASS, NAVIGATE_URL, IMAGE_URL")

                Case Is = "tbl_tracking_providers"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, TRACKING_PROVIDER, SEQUENCE")
                Case Is = "tbl_page_tracking"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, PAGE_CODE, LANGUAGE_CODE, LOCATION, TRACKING_PROVIDER, TRACKING_CONTENT")
                Case Is = "tbl_tracking_settings_values"
                    .Append("SELECT 0, BUSINESS_UNIT, PARTNER, TRACKING_PROVIDER, SETTING_NAME, VALUE")

                Case Is = "tbl_email_templates"
                    .Append("SELECT EMAILTEMPLATE_ID, BUSINESS_UNIT, PARTNER, ACTIVE, NAME, DESCRIPTION, TEMPLATE_TYPE, EMAIL_HTML, EMAIL_FROM_ADDRESS, EMAIL_SUBJECT, EMAIL_BODY, ADDED_DATETIME, UPDATED_DATETIME, MASTER")

                Case Is = "tbl_alert_definition"
                    .Append("SELECT ID, BUSINESS_UNIT, PARTNER, NAME, DESCRIPTION, IMAGE_PATH, ACTION, ACTION_DETAILS, ACTIVATION_START_DATETIME, ACTIVATION_END_DATETIME, NON_STANDARD, ENABLED, SUBJECT, DELETED, ACTION_DETAILS_URL_OPTION")
                Case Is = "tbl_alert_critera"
                    .Append("SELECT ID, ALERT_ID, ATTR_ID, ALERT_OPERATOR, SEQUENCE, CLAUSE, CLAUSE_TYPE")
                Case Is = "tbl_attribute_definition"
                    .Append("SELECT ID, BUSINESS_UNIT, PARTNER, CATEGORY, NAME, DESCRIPTION, TYPE, FOREIGN_KEY, SOURCE")

                Case Is = "tbl_activity_questions"
                    .Append("SELECT QUESTION_ID, QUESTION_TEXT, ANSWER_TYPE, ALLOW_SELECT_OTHER_OPTION, MANDATORY, PRICE_BAND_LIST, REGULAR_EXPRESSION, HYPERLINK, REMEMBERED_ANSWER, ASK_QUESTION_PER_HOSPITALITY_BOOKING")
                Case Is = "tbl_activity_questions_answer_categories"
                    .Append("SELECT CATEGORY_ID, CATEGORY_NAME")
                Case Is = "tbl_activity_questions_answers"
                    .Append("SELECT ANSWER_ID, CATEGORY_ID, ANSWER_TEXT")
                Case Is = "tbl_activity_templates"
                    .Append("SELECT TEMPLATE_ID, BUSINESS_UNIT, NAME, TEMPLATE_TYPE, TEMPLATE_PER_PRODUCT")
                Case Is = "tbl_activity_templates_detail"
                    .Append("SELECT ID, TEMPLATE_ID, QUESTION_ID, SEQUENCE")
                Case Is = "tbl_activity_questions_with_answers"
                    .Append("SELECT ID, QUESTION_ID, ANSWER_ID, CATEGORY_ID")
                Case Is = "tbl_activity_status"
                    .Append("SELECT ID, STATUS_ID, TYPE_ID")
                Case Is = "tbl_activity_status_description"
                    .Append("SELECT STATUS_ID, BUSINESS_UNIT, DESCRIPTION")
                Case Is = "tbl_activity_template_type"
                    .Append("SELECT TYPE_ID, BUSINESS_UNIT, NAME, HIDE_IN_MAINTENANCE, HIDE_IN_ACTIVITIES_PAGE, LOCAL_ROOT_DIRECTORY, REMOTE_ROOT_DIRECTORY, MAX_FILE_UPLOAD_SIZE, ALLOWABLE_FILE_TYPES, ACTIVE, SAVE_TO_TALENT, SAVE_DEFAULTS")

            End Select
            .Append(" From ")
            .Append(SourceTable)
            .Append(" as ")
            .Append(ExtractTable)

        End With
        Return sbExtract
    End Function

    Private Shared Sub DoUpdateTable_tbl_ecommerce_module_defaults_bu(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_ecommerce_module_defaults_bu_work"
        Const destinationTable As String = "tbl_ecommerce_module_defaults_bu"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("APPLICATION", New sqlField("APPLICATION", "STRING"))
            .Add("MODULE", New sqlField("MODULE", "STRING"))
            .Add("DEFAULT_NAME", New sqlField("DEFAULT_NAME", "STRING"))
            .Add("VALUE", New sqlField("VALUE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("APPLICATION", New sqlField("APPLICATION", "STRING"))
            .Add("MODULE", New sqlField("MODULE", "STRING"))
            .Add("DEFAULT_NAME", New sqlField("DEFAULT_NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)

        update.Append(" AND " & destinationTable & ".DEFAULT_NAME NOT LIKE 'NOISE%' ")
        insert.Append(" AND " & sourceTable & ".DEFAULT_NAME NOT LIKE 'NOISE%' ")
        delete.Append(" AND " & destinationTable & ".DEFAULT_NAME NOT LIKE 'NOISE%' ")

        update.Append(" AND " & destinationTable & ".DEFAULT_NAME NOT IN ('ORDER_NUMBER','TEMP_ORDER_NUMBER') ")
        insert.Append(" AND " & sourceTable & ".DEFAULT_NAME NOT IN ('ORDER_NUMBER','TEMP_ORDER_NUMBER') ")
        delete.Append(" AND " & destinationTable & ".DEFAULT_NAME NOT IN ('ORDER_NUMBER','TEMP_ORDER_NUMBER') ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)

    End Sub

    Private Shared Sub DoUpdateTable_tbl_querystring(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_querystring_work"
        Const destinationTable As String = "tbl_querystring"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("ACTIVE", New sqlField("ACTIVE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("PAGE", New sqlField("PAGE", "STRING"))
            .Add("QUERYSTRING_VALUE", New sqlField("QUERYSTRING_VALUE", "STRING"))
            .Add("QUERYSTRING_OBFUSCATED", New sqlField("QUERYSTRING_OBFUSCATED", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ACTIVE", New sqlField("ACTIVE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("PAGE", New sqlField("PAGE", "STRING"))
            .Add("QUERYSTRING_VALUE", New sqlField("QUERYSTRING_VALUE", "STRING"))
            .Add("QUERYSTRING_OBFUSCATED", New sqlField("QUERYSTRING_OBFUSCATED", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, String.Empty)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, String.Empty)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, String.Empty)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_product(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_work"
        Const destinationTable As String = "tbl_product"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
            .Add("PRODUCT_DESCRIPTION_1", New sqlField("PRODUCT_DESCRIPTION_1", "STRING"))
            .Add("PRODUCT_DESCRIPTION_2", New sqlField("PRODUCT_DESCRIPTION_2", "STRING"))
            .Add("PRODUCT_DESCRIPTION_3", New sqlField("PRODUCT_DESCRIPTION_3", "STRING"))
            .Add("PRODUCT_DESCRIPTION_4", New sqlField("PRODUCT_DESCRIPTION_4", "STRING"))
            .Add("PRODUCT_DESCRIPTION_5", New sqlField("PRODUCT_DESCRIPTION_5", "STRING"))
            .Add("PRODUCT_LENGTH", New sqlField("PRODUCT_LENGTH", "DECIMAL"))
            .Add("PRODUCT_LENGTH_UOM", New sqlField("PRODUCT_LENGTH_UOM", "STRING"))
            .Add("PRODUCT_WIDTH", New sqlField("PRODUCT_WIDTH", "DECIMAL"))
            .Add("PRODUCT_WIDTH_UOM", New sqlField("PRODUCT_WIDTH_UOM", "STRING"))
            .Add("PRODUCT_DEPTH", New sqlField("PRODUCT_DEPTH", "DECIMAL"))
            .Add("PRODUCT_DEPTH_UOM", New sqlField("PRODUCT_DEPTH_UOM", "STRING"))
            .Add("PRODUCT_HEIGHT", New sqlField("PRODUCT_HEIGHT", "DECIMAL"))
            .Add("PRODUCT_HEIGHT_UOM", New sqlField("PRODUCT_HEIGHT_UOM", "STRING"))
            .Add("PRODUCT_SIZE", New sqlField("PRODUCT_SIZE", "DECIMAL"))
            .Add("PRODUCT_SIZE_UOM", New sqlField("PRODUCT_SIZE_UOM", "STRING"))
            .Add("PRODUCT_WEIGHT", New sqlField("PRODUCT_WEIGHT", "DECIMAL"))
            .Add("PRODUCT_WEIGHT_UOM", New sqlField("PRODUCT_WEIGHT_UOM", "STRING"))
            .Add("PRODUCT_VOLUME", New sqlField("PRODUCT_VOLUME", "DECIMAL"))
            .Add("PRODUCT_VOLUME_UOM", New sqlField("PRODUCT_VOLUME_UOM", "STRING"))
            .Add("PRODUCT_COLOUR", New sqlField("PRODUCT_COLOUR", "STRING"))
            .Add("PRODUCT_PACK_SIZE", New sqlField("PRODUCT_PACK_SIZE", "STRING"))
            .Add("PRODUCT_PACK_SIZE_UOM", New sqlField("PRODUCT_PACK_SIZE_UOM", "STRING"))
            .Add("PRODUCT_SUPPLIER_PART_NO", New sqlField("PRODUCT_SUPPLIER_PART_NO", "STRING"))
            .Add("PRODUCT_CUSTOMER_PART_NO", New sqlField("PRODUCT_CUSTOMER_PART_NO", "STRING"))
            .Add("PRODUCT_TASTING_NOTES_1", New sqlField("PRODUCT_TASTING_NOTES_1", "STRING"))
            .Add("PRODUCT_TASTING_NOTES_2", New sqlField("PRODUCT_TASTING_NOTES_2", "STRING"))
            .Add("PRODUCT_ABV", New sqlField("PRODUCT_ABV", "STRING"))
            .Add("PRODUCT_VINTAGE", New sqlField("PRODUCT_VINTAGE", "INT"))
            .Add("PRODUCT_SUPPLIER", New sqlField("PRODUCT_SUPPLIER", "STRING"))
            .Add("PRODUCT_COUNTRY", New sqlField("PRODUCT_COUNTRY", "STRING"))
            .Add("PRODUCT_REGION", New sqlField("PRODUCT_REGION", "STRING"))
            .Add("PRODUCT_AREA", New sqlField("PRODUCT_AREA", "STRING"))
            .Add("PRODUCT_GRAPE", New sqlField("PRODUCT_GRAPE", "STRING"))
            .Add("PRODUCT_CLOSURE", New sqlField("PRODUCT_CLOSURE", "STRING"))
            .Add("PRODUCT_CATALOG_CODE", New sqlField("PRODUCT_CATALOG_CODE", "STRING"))
            .Add("PRODUCT_VEGETARIAN", New sqlField("PRODUCT_VEGETARIAN", "BIT"))
            .Add("PRODUCT_VEGAN", New sqlField("PRODUCT_VEGAN", "BIT"))
            .Add("PRODUCT_ORGANIC", New sqlField("PRODUCT_ORGANIC", "BIT"))
            .Add("PRODUCT_BIODYNAMIC", New sqlField("PRODUCT_BIODYNAMIC", "BIT"))
            .Add("PRODUCT_LUTTE", New sqlField("PRODUCT_LUTTE", "BIT"))
            .Add("PRODUCT_MINIMUM_AGE", New sqlField("PRODUCT_MINIMUM_AGE", "INT"))
            .Add("PRODUCT_HTML_1", New sqlField("PRODUCT_HTML_1", "STRING"))
            .Add("PRODUCT_HTML_2", New sqlField("PRODUCT_HTML_2", "STRING"))
            .Add("PRODUCT_HTML_3", New sqlField("PRODUCT_HTML_3", "STRING"))
            .Add("PRODUCT_SEARCH_KEYWORDS", New sqlField("PRODUCT_SEARCH_KEYWORDS", "STRING"))
            .Add("PRODUCT_PAGE_TITLE", New sqlField("PRODUCT_PAGE_TITLE", "STRING"))
            .Add("PRODUCT_META_DESCRIPTION", New sqlField("PRODUCT_META_DESCRIPTION", "STRING"))
            .Add("PRODUCT_META_KEYWORDS", New sqlField("PRODUCT_META_KEYWORDS", "STRING"))
            .Add("PRODUCT_SEARCH_RANGE_01", New sqlField("PRODUCT_SEARCH_RANGE_01", "DECIMAL"))
            .Add("PRODUCT_SEARCH_RANGE_02", New sqlField("PRODUCT_SEARCH_RANGE_02", "DECIMAL"))
            .Add("PRODUCT_SEARCH_RANGE_03", New sqlField("PRODUCT_SEARCH_RANGE_03", "DECIMAL"))
            .Add("PRODUCT_SEARCH_RANGE_04", New sqlField("PRODUCT_SEARCH_RANGE_04", "DECIMAL"))
            .Add("PRODUCT_SEARCH_RANGE_05", New sqlField("PRODUCT_SEARCH_RANGE_05", "DECIMAL"))
            .Add("PRODUCT_SEARCH_CRITERIA_01", New sqlField("PRODUCT_SEARCH_CRITERIA_01", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_02", New sqlField("PRODUCT_SEARCH_CRITERIA_02", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_03", New sqlField("PRODUCT_SEARCH_CRITERIA_03", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_04", New sqlField("PRODUCT_SEARCH_CRITERIA_04", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_05", New sqlField("PRODUCT_SEARCH_CRITERIA_05", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_06", New sqlField("PRODUCT_SEARCH_CRITERIA_06", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_07", New sqlField("PRODUCT_SEARCH_CRITERIA_07", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_08", New sqlField("PRODUCT_SEARCH_CRITERIA_08", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_09", New sqlField("PRODUCT_SEARCH_CRITERIA_09", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_10", New sqlField("PRODUCT_SEARCH_CRITERIA_10", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_11", New sqlField("PRODUCT_SEARCH_CRITERIA_11", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_12", New sqlField("PRODUCT_SEARCH_CRITERIA_12", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_13", New sqlField("PRODUCT_SEARCH_CRITERIA_13", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_14", New sqlField("PRODUCT_SEARCH_CRITERIA_14", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_15", New sqlField("PRODUCT_SEARCH_CRITERIA_15", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_16", New sqlField("PRODUCT_SEARCH_CRITERIA_16", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_17", New sqlField("PRODUCT_SEARCH_CRITERIA_17", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_18", New sqlField("PRODUCT_SEARCH_CRITERIA_18", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_19", New sqlField("PRODUCT_SEARCH_CRITERIA_19", "STRING"))
            .Add("PRODUCT_SEARCH_CRITERIA_20", New sqlField("PRODUCT_SEARCH_CRITERIA_20", "STRING"))
            .Add("PRODUCT_SEARCH_SWITCH_01", New sqlField("PRODUCT_SEARCH_SWITCH_01", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_02", New sqlField("PRODUCT_SEARCH_SWITCH_02", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_03", New sqlField("PRODUCT_SEARCH_SWITCH_03", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_04", New sqlField("PRODUCT_SEARCH_SWITCH_04", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_05", New sqlField("PRODUCT_SEARCH_SWITCH_05", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_06", New sqlField("PRODUCT_SEARCH_SWITCH_06", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_07", New sqlField("PRODUCT_SEARCH_SWITCH_07", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_08", New sqlField("PRODUCT_SEARCH_SWITCH_08", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_09", New sqlField("PRODUCT_SEARCH_SWITCH_09", "BIT"))
            .Add("PRODUCT_SEARCH_SWITCH_10", New sqlField("PRODUCT_SEARCH_SWITCH_10", "BIT"))
            .Add("PRODUCT_SEARCH_DATE_01", New sqlField("PRODUCT_SEARCH_DATE_01", "STRING"))
            .Add("PRODUCT_SEARCH_DATE_02", New sqlField("PRODUCT_SEARCH_DATE_02", "STRING"))
            .Add("PRODUCT_SEARCH_DATE_03", New sqlField("PRODUCT_SEARCH_DATE_03", "STRING"))
            .Add("PRODUCT_SEARCH_DATE_04", New sqlField("PRODUCT_SEARCH_DATE_04", "STRING"))
            .Add("PRODUCT_SEARCH_DATE_05", New sqlField("PRODUCT_SEARCH_DATE_05", "STRING"))
            .Add("PRODUCT_TARIFF_CODE", New sqlField("PRODUCT_TARIFF_CODE", "STRING"))
            .Add("PRODUCT_OPTION_MASTER", New sqlField("PRODUCT_OPTION_MASTER", "BIT"))
            .Add("ALTERNATE_SKU", New sqlField("ALTERNATE_SKU", "STRING"))
            .Add("AVAILABLE_ONLINE", New sqlField("AVAILABLE_ONLINE", "BIT"))
            .Add("PERSONALISABLE", New sqlField("PERSONALISABLE", "BIT"))
            .Add("DISCONTINUED", New sqlField("DISCONTINUED", "BIT"))

        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)

    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_stock(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_stock_work"
        Const destinationTable As String = "tbl_product_stock"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("PRODUCT", New sqlField("PRODUCT", "STRING"))
            .Add("STOCK_LOCATION", New sqlField("STOCK_LOCATION", "STRING"))
            .Add("QUANTITY", New sqlField("QUANTITY", "DECIMAL"))
            .Add("ALLOCATED_QUANTITY", New sqlField("ALLOCATED_QUANTITY", "DECIMAL"))
            .Add("AVAILABLE_QUANTITY", New sqlField("AVAILABLE_QUANTITY", "DECIMAL"))
            .Add("RESTOCK_CODE", New sqlField("RESTOCK_CODE", "STRING"))
            .Add("WAREHOUSE", New sqlField("WAREHOUSE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("PRODUCT", New sqlField("PRODUCT", "STRING"))
            .Add("STOCK_LOCATION", New sqlField("STOCK_LOCATION", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_group(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_work"
        Const destinationTable As String = "tbl_group"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_NAME", New sqlField("GROUP_NAME", "STRING"))
            .Add("GROUP_DESCRIPTION_1", New sqlField("GROUP_DESCRIPTION_1", "STRING"))
            .Add("GROUP_DESCRIPTION_2", New sqlField("GROUP_DESCRIPTION_2", "STRING"))
            .Add("GROUP_HTML_1", New sqlField("GROUP_HTML_1", "STRING"))
            .Add("GROUP_HTML_2", New sqlField("GROUP_HTML_2", "STRING"))
            .Add("GROUP_HTML_3", New sqlField("GROUP_HTML_3", "STRING"))
            .Add("GROUP_PAGE_TITLE", New sqlField("GROUP_PAGE_TITLE", "STRING"))
            .Add("GROUP_META_DESCRIPTION", New sqlField("GROUP_META_DESCRIPTION", "STRING"))
            .Add("GROUP_META_KEYWORDS", New sqlField("GROUP_META_KEYWORDS", "STRING"))
            .Add("GROUP_ADHOC", New sqlField("GROUP_ADHOC", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_NAME", New sqlField("GROUP_NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_lang_work"
        Const destinationTable As String = "tbl_group_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_CODE", New sqlField("GROUP_CODE", "STRING"))
            .Add("GROUP_LANGUAGE", New sqlField("GROUP_LANGUAGE", "STRING"))
            .Add("GROUP_DESCRIPTION_1", New sqlField("GROUP_DESCRIPTION_1", "STRING"))
            .Add("GROUP_DESCRIPTION_2", New sqlField("GROUP_DESCRIPTION_2", "STRING"))
            .Add("GROUP_HTML_1", New sqlField("GROUP_HTML_1", "STRING"))
            .Add("GROUP_HTML_2", New sqlField("GROUP_HTML_2", "STRING"))
            .Add("GROUP_HTML_3", New sqlField("GROUP_HTML_3", "STRING"))
            .Add("GROUP_PAGE_TITLE", New sqlField("GROUP_PAGE_TITLE", "STRING"))
            .Add("GROUP_META_DESCRIPTION", New sqlField("GROUP_META_DESCRIPTION", "STRING"))
            .Add("GROUP_META_KEYWORDS", New sqlField("GROUP_META_KEYWORDS", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_CODE", New sqlField("GROUP_CODE", "STRING"))
            .Add("GROUP_LANGUAGE", New sqlField("GROUP_LANGUAGE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_group_product(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_product_work"
        Const destinationTable As String = "tbl_group_product"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_BUSINESS_UNIT", New sqlField("GROUP_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_PARTNER", New sqlField("GROUP_PARTNER", "STRING"))
            .Add("GROUP_L01_GROUP", New sqlField("GROUP_L01_GROUP", "STRING"))
            .Add("GROUP_L02_GROUP", New sqlField("GROUP_L02_GROUP", "STRING"))
            .Add("GROUP_L03_GROUP", New sqlField("GROUP_L03_GROUP", "STRING"))
            .Add("GROUP_L04_GROUP", New sqlField("GROUP_L04_GROUP", "STRING"))
            .Add("GROUP_L05_GROUP", New sqlField("GROUP_L05_GROUP", "STRING"))
            .Add("GROUP_L06_GROUP", New sqlField("GROUP_L06_GROUP", "STRING"))
            .Add("GROUP_L07_GROUP", New sqlField("GROUP_L07_GROUP", "STRING"))
            .Add("GROUP_L08_GROUP", New sqlField("GROUP_L08_GROUP", "STRING"))
            .Add("GROUP_L09_GROUP", New sqlField("GROUP_L09_GROUP", "STRING"))
            .Add("GROUP_L10_GROUP", New sqlField("GROUP_L10_GROUP", "STRING"))
            .Add("PRODUCT", New sqlField("PRODUCT", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
            .Add("GROUP_ADHOC", New sqlField("GROUP_ADHOC", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_BUSINESS_UNIT", New sqlField("GROUP_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_PARTNER", New sqlField("GROUP_PARTNER", "STRING"))
            .Add("PRODUCT", New sqlField("PRODUCT", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_01(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_01_work"
        Const destinationTable As String = "tbl_group_level_01"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L01_BUSINESS_UNIT", New sqlField("GROUP_L01_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L01_PARTNER", New sqlField("GROUP_L01_PARTNER", "STRING"))
            .Add("GROUP_L01_L01_GROUP", New sqlField("GROUP_L01_L01_GROUP", "STRING"))
            .Add("GROUP_L01_SEQUENCE", New sqlField("GROUP_L01_SEQUENCE", "STRING"))
            .Add("GROUP_L01_DESCRIPTION_1", New sqlField("GROUP_L01_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L01_DESCRIPTION_2", New sqlField("GROUP_L01_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L01_HTML_1", New sqlField("GROUP_L01_HTML_1", "STRING"))
            .Add("GROUP_L01_HTML_2", New sqlField("GROUP_L01_HTML_2", "STRING"))
            .Add("GROUP_L01_HTML_3", New sqlField("GROUP_L01_HTML_3", "STRING"))
            .Add("GROUP_L01_PAGE_TITLE", New sqlField("GROUP_L01_PAGE_TITLE", "STRING"))
            .Add("GROUP_L01_META_DESCRIPTION", New sqlField("GROUP_L01_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L01_META_KEYWORDS", New sqlField("GROUP_L01_META_KEYWORDS", "STRING"))
            .Add("GROUP_L01_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L01_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L01_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L01_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L01_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L01_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L01_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L01_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L01_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L01_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L01_SHOW_IN_NAVIGATION", New sqlField("GROUP_L01_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L01_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L01_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L01_HTML_GROUP", New sqlField("GROUP_L01_HTML_GROUP", "BIT"))
            .Add("GROUP_L01_HTML_GROUP_TYPE", New sqlField("GROUP_L01_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L01_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L01_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L01_ADHOC_GROUP", New sqlField("GROUP_L01_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L01_THEME", New sqlField("GROUP_L01_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L01_BUSINESS_UNIT", New sqlField("GROUP_L01_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L01_PARTNER", New sqlField("GROUP_L01_PARTNER", "STRING"))
            .Add("GROUP_L01_L01_GROUP", New sqlField("GROUP_L01_L01_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L01_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L01_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L01_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_02(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_02_work"
        Const destinationTable As String = "tbl_group_level_02"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L02_BUSINESS_UNIT", New sqlField("GROUP_L02_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L02_PARTNER", New sqlField("GROUP_L02_PARTNER", "STRING"))
            .Add("GROUP_L02_L01_GROUP", New sqlField("GROUP_L02_L01_GROUP", "STRING"))
            .Add("GROUP_L02_L02_GROUP", New sqlField("GROUP_L02_L02_GROUP", "STRING"))
            .Add("GROUP_L02_SEQUENCE", New sqlField("GROUP_L02_SEQUENCE", "STRING"))
            .Add("GROUP_L02_DESCRIPTION_1", New sqlField("GROUP_L02_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L02_DESCRIPTION_2", New sqlField("GROUP_L02_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L02_HTML_1", New sqlField("GROUP_L02_HTML_1", "STRING"))
            .Add("GROUP_L02_HTML_2", New sqlField("GROUP_L02_HTML_2", "STRING"))
            .Add("GROUP_L02_HTML_3", New sqlField("GROUP_L02_HTML_3", "STRING"))
            .Add("GROUP_L02_PAGE_TITLE", New sqlField("GROUP_L02_PAGE_TITLE", "STRING"))
            .Add("GROUP_L02_META_DESCRIPTION", New sqlField("GROUP_L02_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L02_META_KEYWORDS", New sqlField("GROUP_L02_META_KEYWORDS", "STRING"))
            .Add("GROUP_L02_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L02_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L02_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L02_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L02_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L02_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L02_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L02_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L02_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L02_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L02_SHOW_IN_NAVIGATION", New sqlField("GROUP_L02_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L02_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L02_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L02_HTML_GROUP", New sqlField("GROUP_L02_HTML_GROUP", "BIT"))
            .Add("GROUP_L02_HTML_GROUP_TYPE", New sqlField("GROUP_L02_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L02_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L02_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L02_ADHOC_GROUP", New sqlField("GROUP_L02_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L02_THEME", New sqlField("GROUP_L02_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L02_BUSINESS_UNIT", New sqlField("GROUP_L02_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L02_PARTNER", New sqlField("GROUP_L02_PARTNER", "STRING"))
            .Add("GROUP_L02_L01_GROUP", New sqlField("GROUP_L02_L01_GROUP", "STRING"))
            .Add("GROUP_L02_L02_GROUP", New sqlField("GROUP_L02_L02_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L02_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L02_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L02_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_03(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_03_work"
        Const destinationTable As String = "tbl_group_level_03"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L03_BUSINESS_UNIT", New sqlField("GROUP_L03_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L03_PARTNER", New sqlField("GROUP_L03_PARTNER", "STRING"))
            .Add("GROUP_L03_L01_GROUP", New sqlField("GROUP_L03_L01_GROUP", "STRING"))
            .Add("GROUP_L03_L02_GROUP", New sqlField("GROUP_L03_L02_GROUP", "STRING"))
            .Add("GROUP_L03_L03_GROUP", New sqlField("GROUP_L03_L03_GROUP", "STRING"))
            .Add("GROUP_L03_SEQUENCE", New sqlField("GROUP_L03_SEQUENCE", "STRING"))
            .Add("GROUP_L03_DESCRIPTION_1", New sqlField("GROUP_L03_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L03_DESCRIPTION_2", New sqlField("GROUP_L03_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L03_HTML_1", New sqlField("GROUP_L03_HTML_1", "STRING"))
            .Add("GROUP_L03_HTML_2", New sqlField("GROUP_L03_HTML_2", "STRING"))
            .Add("GROUP_L03_HTML_3", New sqlField("GROUP_L03_HTML_3", "STRING"))
            .Add("GROUP_L03_PAGE_TITLE", New sqlField("GROUP_L03_PAGE_TITLE", "STRING"))
            .Add("GROUP_L03_META_DESCRIPTION", New sqlField("GROUP_L03_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L03_META_KEYWORDS", New sqlField("GROUP_L03_META_KEYWORDS", "STRING"))
            .Add("GROUP_L03_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L03_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L03_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L03_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L03_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L03_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L03_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L03_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L03_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L03_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L03_SHOW_IN_NAVIGATION", New sqlField("GROUP_L03_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L03_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L03_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L03_HTML_GROUP", New sqlField("GROUP_L03_HTML_GROUP", "BIT"))
            .Add("GROUP_L03_HTML_GROUP_TYPE", New sqlField("GROUP_L03_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L03_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L03_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L03_ADHOC_GROUP", New sqlField("GROUP_L03_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L03_THEME", New sqlField("GROUP_L03_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L03_BUSINESS_UNIT", New sqlField("GROUP_L03_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L03_PARTNER", New sqlField("GROUP_L03_PARTNER", "STRING"))
            .Add("GROUP_L03_L01_GROUP", New sqlField("GROUP_L03_L01_GROUP", "STRING"))
            .Add("GROUP_L03_L02_GROUP", New sqlField("GROUP_L03_L02_GROUP", "STRING"))
            .Add("GROUP_L03_L03_GROUP", New sqlField("GROUP_L03_L03_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L03_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L03_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L03_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_04(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_04_work"
        Const destinationTable As String = "tbl_group_level_04"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L04_BUSINESS_UNIT", New sqlField("GROUP_L04_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L04_PARTNER", New sqlField("GROUP_L04_PARTNER", "STRING"))
            .Add("GROUP_L04_L01_GROUP", New sqlField("GROUP_L04_L01_GROUP", "STRING"))
            .Add("GROUP_L04_L02_GROUP", New sqlField("GROUP_L04_L02_GROUP", "STRING"))
            .Add("GROUP_L04_L03_GROUP", New sqlField("GROUP_L04_L03_GROUP", "STRING"))
            .Add("GROUP_L04_L04_GROUP", New sqlField("GROUP_L04_L04_GROUP", "STRING"))
            .Add("GROUP_L04_SEQUENCE", New sqlField("GROUP_L04_SEQUENCE", "STRING"))
            .Add("GROUP_L04_DESCRIPTION_1", New sqlField("GROUP_L04_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L04_DESCRIPTION_2", New sqlField("GROUP_L04_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L04_HTML_1", New sqlField("GROUP_L04_HTML_1", "STRING"))
            .Add("GROUP_L04_HTML_2", New sqlField("GROUP_L04_HTML_2", "STRING"))
            .Add("GROUP_L04_HTML_3", New sqlField("GROUP_L04_HTML_3", "STRING"))
            .Add("GROUP_L04_PAGE_TITLE", New sqlField("GROUP_L04_PAGE_TITLE", "STRING"))
            .Add("GROUP_L04_META_DESCRIPTION", New sqlField("GROUP_L04_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L04_META_KEYWORDS", New sqlField("GROUP_L04_META_KEYWORDS", "STRING"))
            .Add("GROUP_L04_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L04_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L04_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L04_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L04_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L04_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L04_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L04_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L04_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L04_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L04_SHOW_IN_NAVIGATION", New sqlField("GROUP_L04_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L04_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L04_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L04_HTML_GROUP", New sqlField("GROUP_L04_HTML_GROUP", "BIT"))
            .Add("GROUP_L04_HTML_GROUP_TYPE", New sqlField("GROUP_L04_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L04_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L04_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L04_ADHOC_GROUP", New sqlField("GROUP_L04_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L04_THEME", New sqlField("GROUP_L04_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L04_BUSINESS_UNIT", New sqlField("GROUP_L04_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L04_PARTNER", New sqlField("GROUP_L04_PARTNER", "STRING"))
            .Add("GROUP_L04_L01_GROUP", New sqlField("GROUP_L04_L01_GROUP", "STRING"))
            .Add("GROUP_L04_L02_GROUP", New sqlField("GROUP_L04_L02_GROUP", "STRING"))
            .Add("GROUP_L04_L03_GROUP", New sqlField("GROUP_L04_L03_GROUP", "STRING"))
            .Add("GROUP_L04_L04_GROUP", New sqlField("GROUP_L04_L04_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L04_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L04_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L04_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_05(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_05_work"
        Const destinationTable As String = "tbl_group_level_05"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L05_BUSINESS_UNIT", New sqlField("GROUP_L05_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L05_PARTNER", New sqlField("GROUP_L05_PARTNER", "STRING"))
            .Add("GROUP_L05_L01_GROUP", New sqlField("GROUP_L05_L01_GROUP", "STRING"))
            .Add("GROUP_L05_L02_GROUP", New sqlField("GROUP_L05_L02_GROUP", "STRING"))
            .Add("GROUP_L05_L03_GROUP", New sqlField("GROUP_L05_L03_GROUP", "STRING"))
            .Add("GROUP_L05_L04_GROUP", New sqlField("GROUP_L05_L04_GROUP", "STRING"))
            .Add("GROUP_L05_L05_GROUP", New sqlField("GROUP_L05_L05_GROUP", "STRING"))
            .Add("GROUP_L05_SEQUENCE", New sqlField("GROUP_L05_SEQUENCE", "STRING"))
            .Add("GROUP_L05_DESCRIPTION_1", New sqlField("GROUP_L05_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L05_DESCRIPTION_2", New sqlField("GROUP_L05_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L05_HTML_1", New sqlField("GROUP_L05_HTML_1", "STRING"))
            .Add("GROUP_L05_HTML_2", New sqlField("GROUP_L05_HTML_2", "STRING"))
            .Add("GROUP_L05_HTML_3", New sqlField("GROUP_L05_HTML_3", "STRING"))
            .Add("GROUP_L05_PAGE_TITLE", New sqlField("GROUP_L05_PAGE_TITLE", "STRING"))
            .Add("GROUP_L05_META_DESCRIPTION", New sqlField("GROUP_L05_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L05_META_KEYWORDS", New sqlField("GROUP_L05_META_KEYWORDS", "STRING"))
            .Add("GROUP_L05_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L05_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L05_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L05_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L05_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L05_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L05_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L05_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L05_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L05_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L05_SHOW_IN_NAVIGATION", New sqlField("GROUP_L05_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L05_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L05_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L05_HTML_GROUP", New sqlField("GROUP_L05_HTML_GROUP", "BIT"))
            .Add("GROUP_L05_HTML_GROUP_TYPE", New sqlField("GROUP_L05_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L05_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L05_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L05_ADHOC_GROUP", New sqlField("GROUP_L05_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L05_THEME", New sqlField("GROUP_L05_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L05_BUSINESS_UNIT", New sqlField("GROUP_L05_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L05_PARTNER", New sqlField("GROUP_L05_PARTNER", "STRING"))
            .Add("GROUP_L05_L01_GROUP", New sqlField("GROUP_L05_L01_GROUP", "STRING"))
            .Add("GROUP_L05_L02_GROUP", New sqlField("GROUP_L05_L02_GROUP", "STRING"))
            .Add("GROUP_L05_L03_GROUP", New sqlField("GROUP_L05_L03_GROUP", "STRING"))
            .Add("GROUP_L05_L04_GROUP", New sqlField("GROUP_L05_L04_GROUP", "STRING"))
            .Add("GROUP_L05_L05_GROUP", New sqlField("GROUP_L05_L05_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L05_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L05_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L05_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_06(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_06_work"
        Const destinationTable As String = "tbl_group_level_06"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L06_BUSINESS_UNIT", New sqlField("GROUP_L06_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L06_PARTNER", New sqlField("GROUP_L06_PARTNER", "STRING"))
            .Add("GROUP_L06_L01_GROUP", New sqlField("GROUP_L06_L01_GROUP", "STRING"))
            .Add("GROUP_L06_L02_GROUP", New sqlField("GROUP_L06_L02_GROUP", "STRING"))
            .Add("GROUP_L06_L03_GROUP", New sqlField("GROUP_L06_L03_GROUP", "STRING"))
            .Add("GROUP_L06_L04_GROUP", New sqlField("GROUP_L06_L04_GROUP", "STRING"))
            .Add("GROUP_L06_L05_GROUP", New sqlField("GROUP_L06_L05_GROUP", "STRING"))
            .Add("GROUP_L06_L06_GROUP", New sqlField("GROUP_L06_L06_GROUP", "STRING"))
            .Add("GROUP_L06_SEQUENCE", New sqlField("GROUP_L06_SEQUENCE", "STRING"))
            .Add("GROUP_L06_DESCRIPTION_1", New sqlField("GROUP_L06_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L06_DESCRIPTION_2", New sqlField("GROUP_L06_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L06_HTML_1", New sqlField("GROUP_L06_HTML_1", "STRING"))
            .Add("GROUP_L06_HTML_2", New sqlField("GROUP_L06_HTML_2", "STRING"))
            .Add("GROUP_L06_HTML_3", New sqlField("GROUP_L06_HTML_3", "STRING"))
            .Add("GROUP_L06_PAGE_TITLE", New sqlField("GROUP_L06_PAGE_TITLE", "STRING"))
            .Add("GROUP_L06_META_DESCRIPTION", New sqlField("GROUP_L06_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L06_META_KEYWORDS", New sqlField("GROUP_L06_META_KEYWORDS", "STRING"))
            .Add("GROUP_L06_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L06_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L06_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L06_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L06_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L06_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L06_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L06_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L06_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L06_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L06_SHOW_IN_NAVIGATION", New sqlField("GROUP_L06_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L06_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L06_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L06_HTML_GROUP", New sqlField("GROUP_L06_HTML_GROUP", "BIT"))
            .Add("GROUP_L06_HTML_GROUP_TYPE", New sqlField("GROUP_L06_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L06_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L06_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L06_ADHOC_GROUP", New sqlField("GROUP_L06_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L06_THEME", New sqlField("GROUP_L06_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L06_BUSINESS_UNIT", New sqlField("GROUP_L06_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L06_PARTNER", New sqlField("GROUP_L06_PARTNER", "STRING"))
            .Add("GROUP_L06_L01_GROUP", New sqlField("GROUP_L06_L01_GROUP", "STRING"))
            .Add("GROUP_L06_L02_GROUP", New sqlField("GROUP_L06_L02_GROUP", "STRING"))
            .Add("GROUP_L06_L03_GROUP", New sqlField("GROUP_L06_L03_GROUP", "STRING"))
            .Add("GROUP_L06_L04_GROUP", New sqlField("GROUP_L06_L04_GROUP", "STRING"))
            .Add("GROUP_L06_L05_GROUP", New sqlField("GROUP_L06_L05_GROUP", "STRING"))
            .Add("GROUP_L06_L06_GROUP", New sqlField("GROUP_L06_L06_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L06_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L06_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L06_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_07(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_07_work"
        Const destinationTable As String = "tbl_group_level_07"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L07_BUSINESS_UNIT", New sqlField("GROUP_L07_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L07_PARTNER", New sqlField("GROUP_L07_PARTNER", "STRING"))
            .Add("GROUP_L07_L01_GROUP", New sqlField("GROUP_L07_L01_GROUP", "STRING"))
            .Add("GROUP_L07_L02_GROUP", New sqlField("GROUP_L07_L02_GROUP", "STRING"))
            .Add("GROUP_L07_L03_GROUP", New sqlField("GROUP_L07_L03_GROUP", "STRING"))
            .Add("GROUP_L07_L04_GROUP", New sqlField("GROUP_L07_L04_GROUP", "STRING"))
            .Add("GROUP_L07_L05_GROUP", New sqlField("GROUP_L07_L05_GROUP", "STRING"))
            .Add("GROUP_L07_L06_GROUP", New sqlField("GROUP_L07_L06_GROUP", "STRING"))
            .Add("GROUP_L07_L07_GROUP", New sqlField("GROUP_L07_L07_GROUP", "STRING"))
            .Add("GROUP_L07_SEQUENCE", New sqlField("GROUP_L07_SEQUENCE", "STRING"))
            .Add("GROUP_L07_DESCRIPTION_1", New sqlField("GROUP_L07_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L07_DESCRIPTION_2", New sqlField("GROUP_L07_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L07_HTML_1", New sqlField("GROUP_L07_HTML_1", "STRING"))
            .Add("GROUP_L07_HTML_2", New sqlField("GROUP_L07_HTML_2", "STRING"))
            .Add("GROUP_L07_HTML_3", New sqlField("GROUP_L07_HTML_3", "STRING"))
            .Add("GROUP_L07_PAGE_TITLE", New sqlField("GROUP_L07_PAGE_TITLE", "STRING"))
            .Add("GROUP_L07_META_DESCRIPTION", New sqlField("GROUP_L07_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L07_META_KEYWORDS", New sqlField("GROUP_L07_META_KEYWORDS", "STRING"))
            .Add("GROUP_L07_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L07_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L07_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L07_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L07_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L07_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L07_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L07_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L07_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L07_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L07_SHOW_IN_NAVIGATION", New sqlField("GROUP_L07_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L07_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L07_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L07_HTML_GROUP", New sqlField("GROUP_L07_HTML_GROUP", "BIT"))
            .Add("GROUP_L07_HTML_GROUP_TYPE", New sqlField("GROUP_L07_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L07_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L07_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L07_ADHOC_GROUP", New sqlField("GROUP_L07_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L07_THEME", New sqlField("GROUP_L07_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L07_BUSINESS_UNIT", New sqlField("GROUP_L07_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L07_PARTNER", New sqlField("GROUP_L07_PARTNER", "STRING"))
            .Add("GROUP_L07_L01_GROUP", New sqlField("GROUP_L07_L01_GROUP", "STRING"))
            .Add("GROUP_L07_L02_GROUP", New sqlField("GROUP_L07_L02_GROUP", "STRING"))
            .Add("GROUP_L07_L03_GROUP", New sqlField("GROUP_L07_L03_GROUP", "STRING"))
            .Add("GROUP_L07_L04_GROUP", New sqlField("GROUP_L07_L04_GROUP", "STRING"))
            .Add("GROUP_L07_L05_GROUP", New sqlField("GROUP_L07_L05_GROUP", "STRING"))
            .Add("GROUP_L07_L06_GROUP", New sqlField("GROUP_L07_L06_GROUP", "STRING"))
            .Add("GROUP_L07_L07_GROUP", New sqlField("GROUP_L07_L07_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L07_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L07_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L07_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_08(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_08_work"
        Const destinationTable As String = "tbl_group_level_08"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L08_BUSINESS_UNIT", New sqlField("GROUP_L08_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L08_PARTNER", New sqlField("GROUP_L08_PARTNER", "STRING"))
            .Add("GROUP_L08_L01_GROUP", New sqlField("GROUP_L08_L01_GROUP", "STRING"))
            .Add("GROUP_L08_L02_GROUP", New sqlField("GROUP_L08_L02_GROUP", "STRING"))
            .Add("GROUP_L08_L03_GROUP", New sqlField("GROUP_L08_L03_GROUP", "STRING"))
            .Add("GROUP_L08_L04_GROUP", New sqlField("GROUP_L08_L04_GROUP", "STRING"))
            .Add("GROUP_L08_L05_GROUP", New sqlField("GROUP_L08_L05_GROUP", "STRING"))
            .Add("GROUP_L08_L06_GROUP", New sqlField("GROUP_L08_L06_GROUP", "STRING"))
            .Add("GROUP_L08_L07_GROUP", New sqlField("GROUP_L08_L07_GROUP", "STRING"))
            .Add("GROUP_L08_L08_GROUP", New sqlField("GROUP_L08_L08_GROUP", "STRING"))
            .Add("GROUP_L08_SEQUENCE", New sqlField("GROUP_L08_SEQUENCE", "STRING"))
            .Add("GROUP_L08_DESCRIPTION_1", New sqlField("GROUP_L08_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L08_DESCRIPTION_2", New sqlField("GROUP_L08_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L08_HTML_1", New sqlField("GROUP_L08_HTML_1", "STRING"))
            .Add("GROUP_L08_HTML_2", New sqlField("GROUP_L08_HTML_2", "STRING"))
            .Add("GROUP_L08_HTML_3", New sqlField("GROUP_L08_HTML_3", "STRING"))
            .Add("GROUP_L08_PAGE_TITLE", New sqlField("GROUP_L08_PAGE_TITLE", "STRING"))
            .Add("GROUP_L08_META_DESCRIPTION", New sqlField("GROUP_L08_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L08_META_KEYWORDS", New sqlField("GROUP_L08_META_KEYWORDS", "STRING"))
            .Add("GROUP_L08_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L08_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L08_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L08_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L08_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L08_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L08_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L08_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L08_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L08_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L08_SHOW_IN_NAVIGATION", New sqlField("GROUP_L08_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L08_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L08_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L08_HTML_GROUP", New sqlField("GROUP_L08_HTML_GROUP", "BIT"))
            .Add("GROUP_L08_HTML_GROUP_TYPE", New sqlField("GROUP_L08_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L08_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L08_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L08_ADHOC_GROUP", New sqlField("GROUP_L08_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L08_THEME", New sqlField("GROUP_L08_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L08_BUSINESS_UNIT", New sqlField("GROUP_L08_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L08_PARTNER", New sqlField("GROUP_L08_PARTNER", "STRING"))
            .Add("GROUP_L08_L01_GROUP", New sqlField("GROUP_L08_L01_GROUP", "STRING"))
            .Add("GROUP_L08_L02_GROUP", New sqlField("GROUP_L08_L02_GROUP", "STRING"))
            .Add("GROUP_L08_L03_GROUP", New sqlField("GROUP_L08_L03_GROUP", "STRING"))
            .Add("GROUP_L08_L04_GROUP", New sqlField("GROUP_L08_L04_GROUP", "STRING"))
            .Add("GROUP_L08_L05_GROUP", New sqlField("GROUP_L08_L05_GROUP", "STRING"))
            .Add("GROUP_L08_L06_GROUP", New sqlField("GROUP_L08_L06_GROUP", "STRING"))
            .Add("GROUP_L08_L07_GROUP", New sqlField("GROUP_L08_L07_GROUP", "STRING"))
            .Add("GROUP_L08_L08_GROUP", New sqlField("GROUP_L08_L08_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L08_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L08_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L08_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_09(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_09_work"
        Const destinationTable As String = "tbl_group_level_09"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L09_BUSINESS_UNIT", New sqlField("GROUP_L09_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L09_PARTNER", New sqlField("GROUP_L09_PARTNER", "STRING"))
            .Add("GROUP_L09_L01_GROUP", New sqlField("GROUP_L09_L01_GROUP", "STRING"))
            .Add("GROUP_L09_L02_GROUP", New sqlField("GROUP_L09_L02_GROUP", "STRING"))
            .Add("GROUP_L09_L03_GROUP", New sqlField("GROUP_L09_L03_GROUP", "STRING"))
            .Add("GROUP_L09_L04_GROUP", New sqlField("GROUP_L09_L04_GROUP", "STRING"))
            .Add("GROUP_L09_L05_GROUP", New sqlField("GROUP_L09_L05_GROUP", "STRING"))
            .Add("GROUP_L09_L06_GROUP", New sqlField("GROUP_L09_L06_GROUP", "STRING"))
            .Add("GROUP_L09_L07_GROUP", New sqlField("GROUP_L09_L07_GROUP", "STRING"))
            .Add("GROUP_L09_L08_GROUP", New sqlField("GROUP_L09_L08_GROUP", "STRING"))
            .Add("GROUP_L09_L09_GROUP", New sqlField("GROUP_L09_L09_GROUP", "STRING"))
            .Add("GROUP_L09_SEQUENCE", New sqlField("GROUP_L09_SEQUENCE", "STRING"))
            .Add("GROUP_L09_DESCRIPTION_1", New sqlField("GROUP_L09_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L09_DESCRIPTION_2", New sqlField("GROUP_L09_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L09_HTML_1", New sqlField("GROUP_L09_HTML_1", "STRING"))
            .Add("GROUP_L09_HTML_2", New sqlField("GROUP_L09_HTML_2", "STRING"))
            .Add("GROUP_L09_HTML_3", New sqlField("GROUP_L09_HTML_3", "STRING"))
            .Add("GROUP_L09_PAGE_TITLE", New sqlField("GROUP_L09_PAGE_TITLE", "STRING"))
            .Add("GROUP_L09_META_DESCRIPTION", New sqlField("GROUP_L09_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L09_META_KEYWORDS", New sqlField("GROUP_L09_META_KEYWORDS", "STRING"))
            .Add("GROUP_L09_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L09_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L09_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L09_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L09_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L09_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L09_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L09_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L09_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L09_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L09_SHOW_IN_NAVIGATION", New sqlField("GROUP_L09_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L09_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L09_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L09_HTML_GROUP", New sqlField("GROUP_L09_HTML_GROUP", "BIT"))
            .Add("GROUP_L09_HTML_GROUP_TYPE", New sqlField("GROUP_L09_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L09_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L09_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L09_ADHOC_GROUP", New sqlField("GROUP_L09_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L09_THEME", New sqlField("GROUP_L09_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L09_BUSINESS_UNIT", New sqlField("GROUP_L09_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L09_PARTNER", New sqlField("GROUP_L09_PARTNER", "STRING"))
            .Add("GROUP_L09_L01_GROUP", New sqlField("GROUP_L09_L01_GROUP", "STRING"))
            .Add("GROUP_L09_L02_GROUP", New sqlField("GROUP_L09_L02_GROUP", "STRING"))
            .Add("GROUP_L09_L03_GROUP", New sqlField("GROUP_L09_L03_GROUP", "STRING"))
            .Add("GROUP_L09_L04_GROUP", New sqlField("GROUP_L09_L04_GROUP", "STRING"))
            .Add("GROUP_L09_L05_GROUP", New sqlField("GROUP_L09_L05_GROUP", "STRING"))
            .Add("GROUP_L09_L06_GROUP", New sqlField("GROUP_L09_L06_GROUP", "STRING"))
            .Add("GROUP_L09_L07_GROUP", New sqlField("GROUP_L09_L07_GROUP", "STRING"))
            .Add("GROUP_L09_L08_GROUP", New sqlField("GROUP_L09_L08_GROUP", "STRING"))
            .Add("GROUP_L09_L09_GROUP", New sqlField("GROUP_L09_L09_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L09_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L09_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L09_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_group_level_10(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_group_level_10_work"
        Const destinationTable As String = "tbl_group_level_10"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("GROUP_L10_BUSINESS_UNIT", New sqlField("GROUP_L10_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L10_PARTNER", New sqlField("GROUP_L10_PARTNER", "STRING"))
            .Add("GROUP_L10_L01_GROUP", New sqlField("GROUP_L10_L01_GROUP", "STRING"))
            .Add("GROUP_L10_L02_GROUP", New sqlField("GROUP_L10_L02_GROUP", "STRING"))
            .Add("GROUP_L10_L03_GROUP", New sqlField("GROUP_L10_L03_GROUP", "STRING"))
            .Add("GROUP_L10_L04_GROUP", New sqlField("GROUP_L10_L04_GROUP", "STRING"))
            .Add("GROUP_L10_L05_GROUP", New sqlField("GROUP_L10_L05_GROUP", "STRING"))
            .Add("GROUP_L10_L06_GROUP", New sqlField("GROUP_L10_L06_GROUP", "STRING"))
            .Add("GROUP_L10_L07_GROUP", New sqlField("GROUP_L10_L07_GROUP", "STRING"))
            .Add("GROUP_L10_L08_GROUP", New sqlField("GROUP_L10_L08_GROUP", "STRING"))
            .Add("GROUP_L10_L09_GROUP", New sqlField("GROUP_L10_L09_GROUP", "STRING"))
            .Add("GROUP_L10_L10_GROUP", New sqlField("GROUP_L10_L10_GROUP", "STRING"))
            .Add("GROUP_L10_SEQUENCE", New sqlField("GROUP_L10_SEQUENCE", "STRING"))
            .Add("GROUP_L10_DESCRIPTION_1", New sqlField("GROUP_L10_DESCRIPTION_1", "STRING"))
            .Add("GROUP_L10_DESCRIPTION_2", New sqlField("GROUP_L10_DESCRIPTION_2", "STRING"))
            .Add("GROUP_L10_HTML_1", New sqlField("GROUP_L10_HTML_1", "STRING"))
            .Add("GROUP_L10_HTML_2", New sqlField("GROUP_L10_HTML_2", "STRING"))
            .Add("GROUP_L10_HTML_3", New sqlField("GROUP_L10_HTML_3", "STRING"))
            .Add("GROUP_L10_PAGE_TITLE", New sqlField("GROUP_L10_PAGE_TITLE", "STRING"))
            .Add("GROUP_L10_META_DESCRIPTION", New sqlField("GROUP_L10_META_DESCRIPTION", "STRING"))
            .Add("GROUP_L10_META_KEYWORDS", New sqlField("GROUP_L10_META_KEYWORDS", "STRING"))
            .Add("GROUP_L10_ADV_SEARCH_TEMPLATE", New sqlField("GROUP_L10_ADV_SEARCH_TEMPLATE", "STRING"))
            .Add("GROUP_L10_PRODUCT_PAGE_TEMPLATE", New sqlField("GROUP_L10_PRODUCT_PAGE_TEMPLATE", "STRING"))
            .Add("GROUP_L10_PRODUCT_LIST_TEMPLATE", New sqlField("GROUP_L10_PRODUCT_LIST_TEMPLATE", "STRING"))
            .Add("GROUP_L10_SHOW_CHILDREN_AS_GROUPS", New sqlField("GROUP_L10_SHOW_CHILDREN_AS_GROUPS", "BIT"))
            .Add("GROUP_L10_SHOW_PRODUCTS_AS_LIST", New sqlField("GROUP_L10_SHOW_PRODUCTS_AS_LIST", "BIT"))
            .Add("GROUP_L10_SHOW_IN_NAVIGATION", New sqlField("GROUP_L10_SHOW_IN_NAVIGATION", "BIT"))
            .Add("GROUP_L10_SHOW_IN_GROUPED_NAV", New sqlField("GROUP_L10_SHOW_IN_GROUPED_NAV", "BIT"))
            .Add("GROUP_L10_HTML_GROUP", New sqlField("GROUP_L10_HTML_GROUP", "BIT"))
            .Add("GROUP_L10_HTML_GROUP_TYPE", New sqlField("GROUP_L10_HTML_GROUP_TYPE", "STRING"))
            .Add("GROUP_L10_SHOW_PRODUCT_DISPLAY", New sqlField("GROUP_L10_SHOW_PRODUCT_DISPLAY", "BIT"))
            .Add("GROUP_L10_ADHOC_GROUP", New sqlField("GROUP_L10_ADHOC_GROUP", "BIT"))
            .Add("GROUP_L10_THEME", New sqlField("GROUP_L10_THEME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("GROUP_L10_BUSINESS_UNIT", New sqlField("GROUP_L10_BUSINESS_UNIT", "STRING"))
            .Add("GROUP_L10_PARTNER", New sqlField("GROUP_L10_PARTNER", "STRING"))
            .Add("GROUP_L10_L01_GROUP", New sqlField("GROUP_L10_L01_GROUP", "STRING"))
            .Add("GROUP_L10_L02_GROUP", New sqlField("GROUP_L10_L02_GROUP", "STRING"))
            .Add("GROUP_L10_L03_GROUP", New sqlField("GROUP_L10_L03_GROUP", "STRING"))
            .Add("GROUP_L10_L04_GROUP", New sqlField("GROUP_L10_L04_GROUP", "STRING"))
            .Add("GROUP_L10_L05_GROUP", New sqlField("GROUP_L10_L05_GROUP", "STRING"))
            .Add("GROUP_L10_L06_GROUP", New sqlField("GROUP_L10_L06_GROUP", "STRING"))
            .Add("GROUP_L10_L07_GROUP", New sqlField("GROUP_L10_L07_GROUP", "STRING"))
            .Add("GROUP_L10_L08_GROUP", New sqlField("GROUP_L10_L08_GROUP", "STRING"))
            .Add("GROUP_L10_L09_GROUP", New sqlField("GROUP_L10_L09_GROUP", "STRING"))
            .Add("GROUP_L10_L10_GROUP", New sqlField("GROUP_L10_L10_GROUP", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")

        update.Append(" AND " & destinationTable & ".GROUP_L10_BUSINESS_UNIT='" & sBU & "' ")
        insert.Append(" AND " & sourceTable & ".GROUP_L10_BUSINESS_UNIT='" & sBU & "' ")
        delete.Append(" AND " & destinationTable & ".GROUP_L10_BUSINESS_UNIT='" & sBU & "' ")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_product_option_defaults(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_option_defaults_work"
        Const destinationTable As String = "tbl_product_option_defaults"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("MASTER_PRODUCT", New sqlField("MASTER_PRODUCT", "STRING"))
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
            .Add("MATCH_ACTION", New sqlField("MATCH_ACTION", "STRING"))
            .Add("IS_DEFAULT", New sqlField("IS_DEFAULT", "BIT"))
            .Add("APPEND_SEQUENCE", New sqlField("APPEND_SEQUENCE", "INT"))
            .Add("DISPLAY_SEQUENCE", New sqlField("DISPLAY_SEQUENCE", "INT"))
            .Add("DISPLAY_TYPE", New sqlField("DISPLAY_TYPE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("MASTER_PRODUCT", New sqlField("MASTER_PRODUCT", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_option_definitions(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_option_definitions_work"
        Const destinationTable As String = "tbl_product_option_definitions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("OPTION_CODE", New sqlField("OPTION_CODE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("OPTION_CODE", New sqlField("OPTION_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_option_definitions_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_option_definitions_lang_work"
        Const destinationTable As String = "tbl_product_option_definitions_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("OPTION_CODE", New sqlField("OPTION_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("DISPLAY_NAME", New sqlField("DISPLAY_NAME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("OPTION_CODE", New sqlField("OPTION_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_option_types(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_option_types_work"
        Const destinationTable As String = "tbl_product_option_types"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_option_types_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_option_types_lang_work"
        Const destinationTable As String = "tbl_product_option_types_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("DISPLAY_NAME", New sqlField("DISPLAY_NAME", "STRING"))
            .Add("LABEL_TEXT", New sqlField("LABEL_TEXT", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_options(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_options_work"
        Const destinationTable As String = "tbl_product_options"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("MASTER_PRODUCT", New sqlField("MASTER_PRODUCT", "STRING"))
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
            .Add("OPTION_CODE", New sqlField("OPTION_CODE", "STRING"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
            .Add("DISPLAY_ORDER", New sqlField("DISPLAY_ORDER", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("MASTER_PRODUCT", New sqlField("MASTER_PRODUCT", "STRING"))
            .Add("OPTION_TYPE", New sqlField("OPTION_TYPE", "STRING"))
            .Add("OPTION_CODE", New sqlField("OPTION_CODE", "STRING"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_product_personalisation(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_personalisation_work"
        Const destinationTable As String = "tbl_product_personalisation"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("XML_CONFIG", New sqlField("XML_CONFIG", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "INT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_personalisation_component(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_personalisation_component_work"
        Const destinationTable As String = "tbl_product_personalisation_component"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("PRODUCT_PERSONALISATION_ID", New sqlField("PRODUCT_PERSONALISATION_ID", "INT"))
            .Add("LINKED_PRODUCT_CODE", New sqlField("LINKED_PRODUCT_CODE", "STRING"))
            .Add("QTY_RULE", New sqlField("QTY_RULE", "STRING"))
            .Add("COMP_TYPE", New sqlField("COMP_TYPE", "STRING"))
            .Add("COMP_NAME", New sqlField("COMP_NAME", "STRING"))
            .Add("COMP_TEXT", New sqlField("COMP_TEXT", "STRING"))
            .Add("COMP_FORCE_CASE", New sqlField("COMP_FORCE_CASE", "STRING"))
            .Add("COMP_LABEL", New sqlField("COMP_LABEL", "STRING"))
            .Add("COMP_FONT", New sqlField("COMP_FONT", "STRING"))
            .Add("COMP_FONT_COLOR", New sqlField("COMP_FONT_COLOR", "STRING"))
            .Add("COMP_SIZE", New sqlField("COMP_SIZE", "INT"))
            .Add("COMP_MAX_CHARS", New sqlField("COMP_MAX_CHARS", "INT"))
            .Add("COMP_ARC_AMOUNT", New sqlField("COMP_ARC_AMOUNT", "FLOAT"))
            .Add("COMP_ARC_ROTATION", New sqlField("COMP_ARC_ROTATION", "FLOAT"))
            .Add("COMP_ARC_WIDTH", New sqlField("COMP_ARC_WIDTH", "FLOAT"))
            .Add("COMP_POS_AT_PERCENT_WIDTH", New sqlField("COMP_POS_AT_PERCENT_WIDTH", "FLOAT"))
            .Add("COMP_POS_AT_PERCENT_HEIGHT", New sqlField("COMP_POS_AT_PERCENT_HEIGHT", "FLOAT"))
            .Add("COMP_RESTRICT_INPUT", New sqlField("COMP_RESTRICT_INPUT", "STRING"))
            .Add("COMP_USE_IMAGE_TEXT", New sqlField("COMP_USE_IMAGE_TEXT", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("PRODUCT_PERSONALISATION_ID", New sqlField("PRODUCT_PERSONALISATION_ID", "INT"))
            .Add("LINKED_PRODUCT_CODE", New sqlField("LINKED_PRODUCT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_personalisation_xref(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_personalisation_xref_work"
        Const destinationTable As String = "tbl_product_personalisation_xref"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
            .Add("PRODUCT_PERSONALISATION_ID", New sqlField("PRODUCT_PERSONALISATION_ID", "INT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
            .Add("PRODUCT_PERSONALISATION_ID", New sqlField("PRODUCT_PERSONALISATION_ID", "INT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_promotions(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_promotions_work"
        Const destinationTable As String = "tbl_promotions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_GROUP", New sqlField("PARTNER_GROUP", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("PROMOTION_TYPE", New sqlField("PROMOTION_TYPE", "STRING"))
            .Add("ACTIVATION_MECHANISM", New sqlField("ACTIVATION_MECHANISM", "STRING"))
            .Add("START_DATE", New sqlField("START_DATE", "STRING"))
            .Add("END_DATE", New sqlField("END_DATE", "STRING"))
            ' Do not sync this value!           .Add("REDEEM_COUNT", New sqlField("REDEEM_COUNT", "INT"))
            .Add("REDEEM_MAX", New sqlField("REDEEM_MAX", "INT"))
            .Add("MIN_SPEND", New sqlField("MIN_SPEND", "DECIMAL"))
            .Add("MIN_ITEMS", New sqlField("MIN_ITEMS", "INT"))
            .Add("NEW_PRICE", New sqlField("NEW_PRICE", "INT"))
            .Add("USER_REDEEM_MAX", New sqlField("USER_REDEEM_MAX", "INT"))
            .Add("PRIORITY_SEQUENCE", New sqlField("PRIORITY_SEQUENCE", "INT"))
            .Add("ACTIVE", New sqlField("ACTIVE", "BIT"))
            .Add("REQUIRED_USER_ATTRIBUTE", New sqlField("REQUIRED_USER_ATTRIBUTE", "STRING"))
            .Add("ADHOC_PROMOTION", New sqlField("ADHOC_PROMOTION", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_promotions_discounts(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_promotions_discounts_work"
        Const destinationTable As String = "tbl_promotions_discounts"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_GROUP", New sqlField("PARTNER_GROUP", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("IS_PERCENTAGE", New sqlField("IS_PERCENTAGE", "BIT"))
            .Add("VALUE", New sqlField("VALUE", "DECIMAL"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_promotions_free_products(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_promotions_free_products_work"
        Const destinationTable As String = "tbl_promotions_free_products"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_GROUP", New sqlField("PARTNER_GROUP", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
            .Add("QUANTITY", New sqlField("QUANTITY", "DECIMAL"))
            .Add("ALLOW_SELECT_OPTION", New sqlField("ALLOW_SELECT_OPTION", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_promotions_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_promotions_lang_work"
        Const destinationTable As String = "tbl_promotions_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_GROUP", New sqlField("PARTNER_GROUP", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("DISPLAY_NAME", New sqlField("DISPLAY_NAME", "STRING"))
            .Add("REQUIREMENTS_DESCRIPTION", New sqlField("REQUIREMENTS_DESCRIPTION", "STRING"))
            .Add("RULES_NOT_MET_ERROR", New sqlField("RULES_NOT_MET_ERROR", "STRING"))
            .Add("USER_REDEEM_MAX_EXCEEDED_ERROR", New sqlField("USER_REDEEM_MAX_EXCEEDED_ERROR", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_promotions_required_products(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_promotions_required_products_work"
        Const destinationTable As String = "tbl_promotions_required_products"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_GROUP", New sqlField("PARTNER_GROUP", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
            .Add("QUANTITY", New sqlField("QUANTITY", "DECIMAL"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PROMOTION_CODE", New sqlField("PROMOTION_CODE", "STRING"))
            .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_promotions_users(ByVal connectionString As String, ByVal sBU As String)
        'Dim insert As New StringBuilder
        'Dim update As New StringBuilder
        'Dim delete As New StringBuilder
        'Const sourceTable As String = "tbl_promotions_users_work"
        'Const destinationTable As String = "tbl_promotions_users"

        ''Build a dictionary of the field mappings
        'Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        'With fieldMappings
        '    .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        '    .Add("PRODUCT_PERSONALISATION_ID", New sqlField("PRODUCT_PERSONALISATION_ID", "INT"))
        'End With

        ''Build a dictionary of the KEY FIELDS
        'Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        'With keyFields
        '    .Add("PRODUCT_CODE", New sqlField("PRODUCT_CODE", "STRING"))
        '    .Add("PRODUCT_PERSONALISATION_ID", New sqlField("PRODUCT_PERSONALISATION_ID", "INT"))
        'End With

        ''Populate the update and insert stringbuilders
        'update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        'insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, "")
        'delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, "")
        'DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_product_relations(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_relations_work"
        Const destinationTable As String = "tbl_product_relations"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("GROUP_L01_GROUP", New sqlField("GROUP_L01_GROUP", "STRING"))
            .Add("GROUP_L02_GROUP", New sqlField("GROUP_L02_GROUP", "STRING"))
            .Add("GROUP_L03_GROUP", New sqlField("GROUP_L03_GROUP", "STRING"))
            .Add("GROUP_L04_GROUP", New sqlField("GROUP_L04_GROUP", "STRING"))
            .Add("GROUP_L05_GROUP", New sqlField("GROUP_L05_GROUP", "STRING"))
            .Add("GROUP_L06_GROUP", New sqlField("GROUP_L06_GROUP", "STRING"))
            .Add("GROUP_L07_GROUP", New sqlField("GROUP_L07_GROUP", "STRING"))
            .Add("GROUP_L08_GROUP", New sqlField("GROUP_L08_GROUP", "STRING"))
            .Add("GROUP_L09_GROUP", New sqlField("GROUP_L09_GROUP", "STRING"))
            .Add("GROUP_L10_GROUP", New sqlField("GROUP_L10_GROUP", "STRING"))
            .Add("PRODUCT", New sqlField("PRODUCT", "STRING"))
            .Add("RELATED_GROUP_L01_GROUP", New sqlField("RELATED_GROUP_L01_GROUP", "STRING"))
            .Add("RELATED_GROUP_L02_GROUP", New sqlField("RELATED_GROUP_L02_GROUP", "STRING"))
            .Add("RELATED_GROUP_L03_GROUP", New sqlField("RELATED_GROUP_L03_GROUP", "STRING"))
            .Add("RELATED_GROUP_L04_GROUP", New sqlField("RELATED_GROUP_L04_GROUP", "STRING"))
            .Add("RELATED_GROUP_L05_GROUP", New sqlField("RELATED_GROUP_L05_GROUP", "STRING"))
            .Add("RELATED_GROUP_L06_GROUP", New sqlField("RELATED_GROUP_L06_GROUP", "STRING"))
            .Add("RELATED_GROUP_L07_GROUP", New sqlField("RELATED_GROUP_L07_GROUP", "STRING"))
            .Add("RELATED_GROUP_L08_GROUP", New sqlField("RELATED_GROUP_L08_GROUP", "STRING"))
            .Add("RELATED_GROUP_L09_GROUP", New sqlField("RELATED_GROUP_L09_GROUP", "STRING"))
            .Add("RELATED_GROUP_L10_GROUP", New sqlField("RELATED_GROUP_L10_GROUP", "STRING"))
            .Add("RELATED_PRODUCT", New sqlField("RELATED_PRODUCT", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
            .Add("TICKETING_PRODUCT_TYPE", New sqlField("TICKETING_PRODUCT_TYPE", "STRING"))
            .Add("TICKETING_PRODUCT_SUB_TYPE", New sqlField("TICKETING_PRODUCT_SUB_TYPE", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_TYPE", New sqlField("RELATED_TICKETING_PRODUCT_TYPE", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_SUB_TYPE", New sqlField("RELATED_TICKETING_PRODUCT_SUB_TYPE", "STRING"))
            .Add("LINK_TYPE", New sqlField("LINK_TYPE", "STRING"))
            .Add("TICKETING_PRODUCT_PRICE_CODE", New sqlField("TICKETING_PRODUCT_PRICE_CODE", "STRING"))
            .Add("RELATED_PRODUCT_MANDATORY", New sqlField("RELATED_PRODUCT_MANDATORY", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_STAND", New sqlField("RELATED_TICKETING_PRODUCT_STAND", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_STAND_READONLY", New sqlField("RELATED_TICKETING_PRODUCT_STAND_READONLY", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_AREA", New sqlField("RELATED_TICKETING_PRODUCT_AREA", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_AREA_READONLY", New sqlField("RELATED_TICKETING_PRODUCT_AREA_READONLY", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_QTY", New sqlField("RELATED_TICKETING_PRODUCT_QTY", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_QTY_MIN", New sqlField("RELATED_TICKETING_PRODUCT_QTY_MIN", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_QTY_MAX", New sqlField("RELATED_TICKETING_PRODUCT_QTY_MAX", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_QTY_READONLY", New sqlField("RELATED_TICKETING_PRODUCT_QTY_READONLY", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_QTY_RATIO", New sqlField("RELATED_TICKETING_PRODUCT_QTY_RATIO", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_QTY_ROUND_UP", New sqlField("RELATED_TICKETING_PRODUCT_QTY_ROUND_UP", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE", New sqlField("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_PRICE_CODE", New sqlField("RELATED_TICKETING_PRODUCT_PRICE_CODE", "STRING"))
            .Add("RELATED_CSS_CLASS", New sqlField("RELATED_CSS_CLASS", "STRING"))
            .Add("RELATED_INSTRUCTIONS", New sqlField("RELATED_INSTRUCTIONS", "STRING"))
            .Add("FOREIGN_PRODUCT_RELATIONS_ID", New sqlField("FOREIGN_PRODUCT_RELATIONS_ID", "STRING"))
            .Add("PACKAGE_COMPONENT_VALUE_01", New sqlField("PACKAGE_COMPONENT_VALUE_01", "DECIMAL"))
            .Add("PACKAGE_COMPONENT_VALUE_02", New sqlField("PACKAGE_COMPONENT_VALUE_02", "DECIMAL"))
            .Add("PACKAGE_COMPONENT_VALUE_03", New sqlField("PACKAGE_COMPONENT_VALUE_02", "DECIMAL"))
            .Add("PACKAGE_COMPONENT_VALUE_04", New sqlField("PACKAGE_COMPONENT_VALUE_04", "DECIMAL"))
            .Add("PACKAGE_COMPONENT_VALUE_05", New sqlField("PACKAGE_COMPONENT_VALUE_05", "DECIMAL"))
            .Add("PACKAGE_COMPONENT_PRICE_BANDS", New sqlField("PACKAGE_COMPONENT_PRICE_BANDS", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PRODUCT", New sqlField("PRODUCT", "STRING"))
            .Add("RELATED_PRODUCT", New sqlField("RELATED_PRODUCT", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
            .Add("TICKETING_PRODUCT_SUB_TYPE", New sqlField("TICKETING_PRODUCT_SUB_TYPE", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_SUB_TYPE", New sqlField("RELATED_TICKETING_PRODUCT_SUB_TYPE", "STRING"))
            .Add("TICKETING_PRODUCT_PRICE_CODE", New sqlField("TICKETING_PRODUCT_PRICE_CODE", "STRING"))
            .Add("RELATED_TICKETING_PRODUCT_PRICE_CODE", New sqlField("RELATED_TICKETING_PRODUCT_PRICE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_relations_attribute_values(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_relations_attribute_values_work"
        Const destinationTable As String = "tbl_product_relations_attribute_values"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("PAGE_POSITION", New sqlField("PAGE_POSITION", "STRING"))
            .Add("ATTRIBUTE_CODE", New sqlField("ATTRIBUTE_CODE", "STRING"))
            .Add("ATTRIBUTE_VALUE", New sqlField("ATTRIBUTE_VALUE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("PAGE_POSITION", New sqlField("PAGE_POSITION", "STRING"))
            .Add("ATTRIBUTE_CODE", New sqlField("ATTRIBUTE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_relations_defaults(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_relations_defaults_work"
        Const destinationTable As String = "tbl_product_relations_defaults"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("PAGE_POSITION", New sqlField("PAGE_POSITION", "STRING"))
            .Add("ONOFF", New sqlField("ONOFF", "BIT"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "INT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("PAGE_POSITION", New sqlField("PAGE_POSITION", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_product_relations_text_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_product_relations_text_lang_work"
        Const destinationTable As String = "tbl_product_relations_text_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("PAGE_POSITION", New sqlField("PAGE_POSITION", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("TEXT_CODE", New sqlField("TEXT_CODE", "STRING"))
            .Add("TEXT_VALUE", New sqlField("TEXT_VALUE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("QUALIFIER", New sqlField("QUALIFIER", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("PAGE_POSITION", New sqlField("PAGE_POSITION", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("TEXT_CODE", New sqlField("TEXT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_page(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_page_work"
        Const destinationTable As String = "tbl_page"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("PAGE_QUERYSTRING", New sqlField("PAGE_QUERYSTRING", "STRING"))
            .Add("USE_SECURE_URL", New sqlField("USE_SECURE_URL", "STRING"))
            .Add("HTML_IN_USE", New sqlField("HTML_IN_USE", "STRING"))
            .Add("PAGE_TYPE", New sqlField("PAGE_TYPE", "STRING"))
            .Add("SHOW_PAGE_HEADER", New sqlField("SHOW_PAGE_HEADER", "STRING"))
            .Add("BCT_URL", New sqlField("BCT_URL", "STRING"))
            .Add("BCT_PARENT", New sqlField("BCT_PARENT", "STRING"))
            .Add("FORCE_LOGIN", New sqlField("FORCE_LOGIN", "STRING"))
            .Add("IN_USE", New sqlField("IN_USE", "STRING"))
            .Add("CSS_PRINT", New sqlField("CSS_PRINT", "STRING"))
            .Add("HIDE_IN_MAINTENANCE", New sqlField("HIDE_IN_MAINTENANCE", "STRING"))
            .Add("ALLOW_GENERIC_SALES", New sqlField("ALLOW_GENERIC_SALES", "STRING"))
            .Add("RESTRICTING_ALERT_NAME", New sqlField("RESTRICTING_ALERT_NAME", "STRING"))
            .Add("BODY_CSS_CLASS", New sqlField("BODY_CSS_CLASS", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_page_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_page_lang_work"
        Const destinationTable As String = "tbl_page_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("TITLE", New sqlField("TITLE", "STRING"))
            .Add("META_KEY", New sqlField("META_KEY", "STRING"))
            .Add("META_DESC", New sqlField("META_DESC", "STRING"))
            .Add("PAGE_HEADER", New sqlField("PAGE_HEADER", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_page_html(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_page_html_work"
        Const destinationTable As String = "tbl_page_html"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("PAGE_QUERYSTRING", New sqlField("PAGE_QUERYSTRING", "STRING"))
            .Add("SECTION", New sqlField("SECTION", "INTEGER"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "INTEGER"))
            .Add("HTML_1", New sqlField("HTML_1", "STRING"))
            .Add("HTML_2", New sqlField("HTML_2", "STRING"))
            .Add("HTML_3", New sqlField("HTML_3", "STRING"))
            .Add("HTML_LOCATION", New sqlField("HTML_LOCATION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("PAGE_QUERYSTRING", New sqlField("PAGE_QUERYSTRING", "STRING"))
            .Add("SECTION", New sqlField("SECTION", "INTEGER"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "INTEGER"))

        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_template_page(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_template_page_work"
        Const destinationTable As String = "tbl_template_page"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_NAME", New sqlField("PAGE_NAME", "STRING"))
            .Add("TEMPLATE_NAME", New sqlField("TEMPLATE_NAME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_NAME", New sqlField("PAGE_NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_page_text_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_page_text_lang_work"
        Const destinationTable As String = "tbl_page_text_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("TEXT_CODE", New sqlField("TEXT_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("TEXT_CONTENT", New sqlField("TEXT_CONTENT", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("TEXT_CODE", New sqlField("TEXT_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_page_attribute(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_page_attribute_work"
        Const destinationTable As String = "tbl_page_attribute"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("ATTR_NAME", New sqlField("ATTR_NAME", "STRING"))
            .Add("ATTR_VALUE", New sqlField("ATTR_VALUE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("ATTR_NAME", New sqlField("ATTR_NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_control_text_lang(ByVal connectionString As String, Optional ByVal subClause As String = "", Optional ByVal sBU As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_control_text_lang_work"
        Const destinationTable As String = "tbl_control_text_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("CONTROL_CODE", New sqlField("CONTROL_CODE", "STRING"))
            .Add("TEXT_CODE", New sqlField("TEXT_CODE", "STRING"))
            .Add("CONTROL_CONTENT", New sqlField("CONTROL_CONTENT", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("CONTROL_CODE", New sqlField("CONTROL_CODE", "STRING"))
            .Add("TEXT_CODE", New sqlField("TEXT_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)

        ' Apply subClause if specified
        If subClause <> "" Then
            update.Append(" " + subClause)
            insert.Append(" " + subClause)
            delete.Append(" " + subClause.Replace(sourceTable, destinationTable))
        End If

        ' Apply BU clause to delete SQL here rather than in BuildDeleteSQL() as too messy with subClause as well
        'If sBU <> "" Then
        '    delete.Append(" AND " & destinationTable & ".BUSINESS_UNIT IN ('*ALL','" & sBU & "') ")
        'End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_control_attribute(ByVal connectionString As String, Optional ByVal subClause As String = "", Optional ByVal sBU As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_control_attribute_work"
        Const destinationTable As String = "tbl_control_attribute"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("CONTROL_CODE", New sqlField("CONTROL_CODE", "STRING"))
            .Add("ATTR_NAME", New sqlField("ATTR_NAME", "STRING"))
            .Add("ATTR_VALUE", New sqlField("ATTR_VALUE", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("CONTROL_CODE", New sqlField("CONTROL_CODE", "STRING"))
            .Add("ATTR_NAME", New sqlField("ATTR_NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)

        ' Apply subClause if specified
        If subClause <> "" Then
            update.Append(" " + subClause)
            insert.Append(" " + subClause)
            delete.Append(" " + subClause.Replace(sourceTable, destinationTable))
        End If

        ' Apply BU clause to the delete SQL here rather than in BuildDeleteSQL() as too messy with subClause as well
        'If sBU <> "" Then
        '    delete.Append(" AND " & destinationTable & ".BUSINESS_UNIT IN ('*ALL','" & sBU & "') ")
        'End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)

    End Sub
    Private Shared Sub DoUpdateTable_tbl_flash_settings(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_flash_settings_work"
        Const destinationTable As String = "tbl_flash_settings"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
            .Add("ATTRIBUTE_NAME", New sqlField("ATTRIBUTE_NAME", "STRING"))
            .Add("ATTRIBUTE_VALUE", New sqlField("ATTRIBUTE_VALUE", "STRING"))
            .Add("QUERYSTRING_PARAMETER", New sqlField("QUERYSTRING_PARAMETER", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER_CODE", New sqlField("PARTNER_CODE", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("ATTRIBUTE_NAME", New sqlField("ATTRIBUTE_NAME", "STRING"))
            .Add("QUERYSTRING_PARAMETER", New sqlField("QUERYSTRING_PARAMETER", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_ticketing_products(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_ticketing_products_work"
        Const destinationTable As String = "tbl_ticketing_products"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PRODUCT_TYPE", New sqlField("PRODUCT_TYPE", "STRING"))
            .Add("DISPLAY_SEQUENCE", New sqlField("DISPLAY_SEQUENCE", "STRING"))
            .Add("ACTIVE", New sqlField("ACTIVE", "STRING"))
            .Add("LOCATION", New sqlField("LOCATION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PRODUCT_TYPE", New sqlField("PRODUCT_TYPE", "STRING"))
            .Add("LOCATION", New sqlField("LOCATION", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_ticketing_products_lang(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_ticketing_products_lang_work"
        Const destinationTable As String = "tbl_ticketing_products_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PRODUCT_TYPE", New sqlField("PRODUCT_TYPE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("DISPLAY_CONTENT", New sqlField("DISPLAY_CONTENT", "STRING"))
            .Add("CSS_CLASS", New sqlField("CSS_CLASS", "STRING"))
            .Add("NAVIGATE_URL", New sqlField("NAVIGATE_URL", "STRING"))
            .Add("IMAGE_URL", New sqlField("IMAGE_URL", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PRODUCT_TYPE", New sqlField("PRODUCT_TYPE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_tracking_providers(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_tracking_providers_work"
        Const destinationTable As String = "tbl_tracking_providers"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_page_tracking(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_page_tracking_work"
        Const destinationTable As String = "tbl_page_tracking"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("LOCATION", New sqlField("LOCATION", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
            .Add("TRACKING_CONTENT", New sqlField("TRACKING_CONTENT", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("LANGUAGE_CODE", New sqlField("LANGUAGE_CODE", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_tracking_user_details(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_tracking_user_details_work"
        Const destinationTable As String = "tbl_tracking_user_details"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
            .Add("LOCATION", New sqlField("LOCATION", "STRING"))
            .Add("TRACKING_TYPE", New sqlField("TRACKING_TYPE", "STRING"))
            .Add("CONTENT", New sqlField("CONTENT", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PAGE_CODE", New sqlField("PAGE_CODE", "STRING"))
            .Add("LOCATION", New sqlField("LOCATION", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub

    Private Shared Sub DoUpdateTable_tbl_tracking_settings_values(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_tracking_settings_values_work"
        Const destinationTable As String = "tbl_tracking_settings_values"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
            .Add("SETTING_NAME", New sqlField("SETTING_NAME", "STRING"))
            .Add("VALUE", New sqlField("VALUE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("TRACKING_PROVIDER", New sqlField("TRACKING_PROVIDER", "STRING"))
            .Add("SETTING_NAME", New sqlField("SETTING_NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString)
    End Sub
    Private Shared Sub DoUpdateTable_tbl_email_templates(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_email_templates_work"
        Const destinationTable As String = "tbl_email_templates"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("ACTIVE", New sqlField("ACTIVE", "BIT"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("EMAIL_HTML", New sqlField("EMAIL_HTML", "BIT"))
            .Add("EMAIL_FROM_ADDRESS", New sqlField("EMAIL_FROM_ADDRESS", "STRING"))
            .Add("EMAIL_SUBJECT", New sqlField("EMAIL_SUBJECT", "STRING"))
            .Add("EMAIL_BODY", New sqlField("EMAIL_BODY", "STRING"))
            .Add("ADDED_DATETIME", New sqlField("ADDED_DATETIME", "DATETIME"))
            .Add("UPDATED_DATETIME", New sqlField("UPDATED_DATETIME", "DATETIME"))
            .Add("MASTER", New sqlField("MASTER", "BIT"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("EMAILTEMPLATE_ID", New sqlField("EMAILTEMPLATE_ID", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("ACTIVE", New sqlField("ACTIVE", "BIT"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("EMAIL_HTML", New sqlField("EMAIL_HTML", "BIT"))
            .Add("EMAIL_FROM_ADDRESS", New sqlField("EMAIL_FROM_ADDRESS", "STRING"))
            .Add("EMAIL_SUBJECT", New sqlField("EMAIL_SUBJECT", "STRING"))
            .Add("EMAIL_BODY", New sqlField("EMAIL_BODY", "STRING"))
            .Add("ADDED_DATETIME", New sqlField("ADDED_DATETIME", "DATETIME"))
            .Add("UPDATED_DATETIME", New sqlField("UPDATED_DATETIME", "DATETIME"))
            .Add("MASTER", New sqlField("MASTER", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("EMAILTEMPLATE_ID", New sqlField("EMAILTEMPLATE_ID", "BIGINT"))
        End With


        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)

        ' Ensure that new records are inserted in the correct order as EMAILTEMPLATE_ID is an identity field 
        insert.Append(" order by EMAILTEMPLATE_ID")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_email_templates")

    End Sub

    Private Shared Sub DoUpdateTable_tbl_alert_definition(ByVal connectionString As String, ByVal sBU As String)

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_alert_definition_work"
        Const destinationTable As String = "tbl_alert_definition"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("IMAGE_PATH", New sqlField("IMAGE_PATH", "STRING"))
            .Add("ACTION", New sqlField("ACTION", "STRING"))
            .Add("ACTION_DETAILS", New sqlField("ACTION_DETAILS", "STRING"))
            .Add("ACTIVATION_START_DATETIME", New sqlField("ACTIVATION_START_DATETIME", "DATETIME"))
            .Add("ACTIVATION_END_DATETIME", New sqlField("ACTIVATION_END_DATETIME", "DATETIME"))
            .Add("NON_STANDARD", New sqlField("NON_STANDARD", "BIT"))
            .Add("ENABLED", New sqlField("ENABLED", "BIT"))
            .Add("SUBJECT", New sqlField("SUBJECT", "STRING"))
            .Add("DELETED", New sqlField("DELETED", "BIT"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ID", New sqlField("ID", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("IMAGE_PATH", New sqlField("IMAGE_PATH", "STRING"))
            .Add("ACTION", New sqlField("ACTION", "STRING"))
            .Add("ACTION_DETAILS", New sqlField("ACTION_DETAILS", "STRING"))
            .Add("ACTIVATION_START_DATETIME", New sqlField("ACTIVATION_START_DATETIME", "DATETIME"))
            .Add("ACTIVATION_END_DATETIME", New sqlField("ACTIVATION_END_DATETIME", "DATETIME"))
            .Add("NON_STANDARD", New sqlField("NON_STANDARD", "BIT"))
            .Add("ENABLED", New sqlField("ENABLED", "BIT"))
            .Add("SUBJECT", New sqlField("SUBJECT", "STRING"))
            .Add("DELETED", New sqlField("DELETED", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "BIGINT"))
        End With


        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)

        ' Ensure that new records are inserted in the correct order as ID is an identity field 
        insert.Append(" order by ID")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_alert_definition")

    End Sub
    Private Shared Sub DoUpdateTable_tbl_alert_critera(ByVal connectionString As String)

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_alert_critera_work"
        Const destinationTable As String = "tbl_alert_critera"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("ALERT_ID", New sqlField("ALERT_ID", "BIGINT"))
            .Add("ATTR_ID", New sqlField("ATTR_ID", "STRING"))
            .Add("ALERT_OPERATOR", New sqlField("ALERT_OPERATOR", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "BIGINT"))
            .Add("CLAUSE", New sqlField("CLAUSE", "BIGINT"))
            .Add("CLAUSE_TYPE", New sqlField("CLAUSE_TYPE", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ID", New sqlField("ID", "BIGINT"))
            .Add("ALERT_ID", New sqlField("ALERT_ID", "BIGINT"))
            .Add("ATTR_ID", New sqlField("ATTR_ID", "STRING"))
            .Add("ALERT_OPERATOR", New sqlField("ALERT_OPERATOR", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "BIGINT"))
            .Add("CLAUSE", New sqlField("CLAUSE", "BIGINT"))
            .Add("CLAUSE_TYPE", New sqlField("CLAUSE_TYPE", "STRING"))
        End With


        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "BIGINT"))
        End With


        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        ' Ensure that new records are inserted in the correct order as ID is an identity field 
        insert.Append(" order by ID")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_alert_critera")

    End Sub
    Private Shared Sub DoUpdateTable_tbl_attribute_definition(ByVal connectionString As String, ByVal sBU As String)

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_attribute_definition_work"
        Const destinationTable As String = "tbl_attribute_definition"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("CATEGORY", New sqlField("CATEGORY", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("TYPE", New sqlField("TYPE", "STRING"))
            .Add("FOREIGN_KEY", New sqlField("FOREIGN_KEY", "STRING"))
            .Add("SOURCE", New sqlField("SOURCE", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ID", New sqlField("ID", "BIGINT"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("PARTNER", New sqlField("PARTNER", "STRING"))
            .Add("CATEGORY", New sqlField("CATEGORY", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
            .Add("TYPE", New sqlField("TYPE", "STRING"))
            .Add("FOREIGN_KEY", New sqlField("FOREIGN_KEY", "STRING"))
            .Add("SOURCE", New sqlField("SOURCE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "BIGINT"))
        End With


        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)

        ' Ensure that new records are inserted in the correct order as ID is an identity field 
        insert.Append(" order by ID")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_attribute_definition")

    End Sub
    Private Shared Sub DoUpdateTable_tbl_alerts(ByVal connectionString As String, ByVal sBU As String)

        '' Temporary solution...
        'Dim sDeleteSQL As String = "delete from tbl_alert where BUSINESS_UNIT='" + sBU + "'"
        'DoUpdateTable("", "", sDeleteSQL, connectionString)


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '
        ' When alerts are published then tbl_alert records will require deletion based upon the following rules:
        '
        ' 1. If criteria are changed then 
        '       Standard alerts - all read and unread alerts for this alert_id are deleted.
        '       Non-standard alerts – n/a
        '
        ' 2. If subject Or description Is changed Then
        '       Standard alerts - all unread for this alert_id are deleted 
        '       Non-standard alerts - all read and unread alerts for this id within current activation dates are deleted
        '
        ' 3. If activation dates are changed then:
        '       Standard alerts – all unread for this alert_id are deleted
        '       Non-standard alerts – n/a
        '
        ' 4. If F&FBirthdayAlert has changed SendEmail email address (action detail) then:
        '       Standard alerts – n/a
        '       Non-standard alerts – all unread for this alert_id are deleted
        '

        '
        ' Open the connection
        Dim conTalent As SqlConnection = Nothing
        Dim connOpen As Boolean = False
        If Not connOpen Then
            conTalent = New SqlConnection(connectionString)
            conTalent.Open()
            connOpen = True
        End If

        ' Retrieve tbl_alert_definition records into dataTable
        Dim cmdAD As SqlCommand = Nothing
        Dim dtrAD As SqlDataReader = Nothing
        Dim dtAD As Data.DataTable = New DataTable
        cmdAD = New SqlCommand("select * from tbl_alert_definition", conTalent)
        dtrAD = cmdAD.ExecuteReader()
        dtAD.Load(dtrAD)
        dtrAD.Close()

        ' Retrieve tbl_alert_definition_work records into dataTable
        Dim cmdADW As SqlCommand = Nothing
        Dim dtrADW As SqlDataReader = Nothing
        Dim dtADW As Data.DataTable = New DataTable
        cmdADW = New SqlCommand("select * from tbl_alert_definition_work", conTalent)
        dtrADW = cmdADW.ExecuteReader()
        dtADW.Load(dtrADW)
        dtrADW.Close()

        ' Retrieve tbl_alert_critera records into dataTable
        Dim cmdAC As SqlCommand = Nothing
        Dim dtrAC As SqlDataReader = Nothing
        Dim dtAC As Data.DataTable = New DataTable
        cmdAC = New SqlCommand("select * from tbl_alert_critera", conTalent)
        dtrAC = cmdAC.ExecuteReader()
        dtAC.Load(dtrAC)
        dtrAC.Close()

        ' Retrieve tbl_alert_critera_work records into dataTable
        Dim cmdACW As SqlCommand = Nothing
        Dim dtrACW As SqlDataReader = Nothing
        Dim dtACW As Data.DataTable = New DataTable
        cmdACW = New SqlCommand("select * from tbl_alert_critera_work", conTalent)
        dtrACW = cmdACW.ExecuteReader()
        dtACW.Load(dtrACW)
        dtrACW.Close()

        ' Close the connection
        If connOpen Then
            conTalent.Close()
        End If

        ' Process
        '        Dim aDeleteAllAlertIDs As New ArrayList
        Dim aDeleteAllUnreadAlertIDs As New ArrayList
        Dim aDeleteAllReadAndUnreadAlertIDs As New ArrayList
        Dim aDeleteAllReadAndUnreadWithinDatesAlertIDs As New ArrayList

        For Each dr As DataRow In dtADW.Rows

            ' Check if criteria changed
            Dim boolCriteriaChanged As Boolean = False
            Dim aWorkRows As DataRow() = dtACW.Select("ALERT_ID=" + dr("ID").ToString, "SEQUENCE")
            Dim aLiveRows As DataRow() = dtAC.Select("ALERT_ID=" + dr("ID").ToString, "SEQUENCE")
            If aWorkRows.Length <> aLiveRows.Length Then
                If CType(dr("NON_STANDARD"), Boolean) = False Then
                    aDeleteAllReadAndUnreadAlertIDs.Add(dr("ID"))
                Else
                    ' do nothing
                End If
                boolCriteriaChanged = True
            Else
                For i = 0 To aWorkRows.Length - 1
                    If (aWorkRows(i)("ATTR_ID") <> aLiveRows(i)("ATTR_ID")) Or _
                        (aWorkRows(i)("ALERT_OPERATOR") <> aLiveRows(i)("ALERT_OPERATOR")) Or _
                        (aWorkRows(i)("CLAUSE") <> aLiveRows(i)("CLAUSE")) Or _
                        (aWorkRows(i)("CLAUSE_TYPE") <> aLiveRows(i)("CLAUSE_TYPE")) Then
                        If CType(dr("NON_STANDARD"), Boolean) = False Then
                            aDeleteAllReadAndUnreadAlertIDs.Add(dr("ID"))
                        Else
                            ' do nothing
                        End If
                        boolCriteriaChanged = True
                        Exit For
                    End If
                Next
            End If

            ' If criteria have not changed then check if description or activation date/times have changed.
            Dim boolOtherDetailsChanged As Boolean = False
            If Not boolCriteriaChanged Then

                ' Have subject, description or activation dates changed
                Dim aLiveRows2 As DataRow() = dtAD.Select("ID=" + dr("ID").ToString)
                If aLiveRows2.Length = 0 Then
                    ' Changed (actually is a new one)
                Else
                    Dim liveRow As DataRow = aLiveRows2(0)
                    If dr("SUBJECT") <> liveRow("SUBJECT") Or dr("DESCRIPTION") <> liveRow("DESCRIPTION") Then
                        If CType(dr("NON_STANDARD"), Boolean) = False Then
                            aDeleteAllUnreadAlertIDs.Add(dr("ID"))
                        Else
                            aDeleteAllReadAndUnreadWithinDatesAlertIDs.Add(dr("ID"))
                        End If
                        boolOtherDetailsChanged = True
                    End If
                    If dr("ACTIVATION_START_DATETIME") <> liveRow("ACTIVATION_START_DATETIME") Or dr("ACTIVATION_END_DATETIME") <> liveRow("ACTIVATION_END_DATETIME") Then
                        If CType(dr("NON_STANDARD"), Boolean) = False Then
                            aDeleteAllUnreadAlertIDs.Add(dr("ID"))
                        Else
                            ' do nothing
                        End If
                        boolOtherDetailsChanged = True
                    End If
                End If

            End If

            ' If criteria not cnaged and details not changed then check if email changed - only done for F&FBirthdayAlert
            If Not boolCriteriaChanged And Not boolOtherDetailsChanged Then
                If dr("NAME") = "FFBirthdayAlert" Then
                    Dim aLiveRows3 As DataRow() = dtAD.Select("ID=" + dr("ID").ToString)
                    If aLiveRows3.Length > 0 Then
                        Dim liveRow As DataRow = aLiveRows3(0)
                        If (dr("ACTION").ToString.Trim = "SendEmail") And Not (dr("ACTION_DETAILS").ToString.Trim = liveRow("ACTION_DETAILS").ToString.Trim) Then
                            aDeleteAllUnreadAlertIDs.Add(dr("ID"))
                        End If
                    End If
                End If
            End If

        Next

        ' Perform the deletes
        Dim sSQL As String = String.Empty
        For Each sID As String In aDeleteAllReadAndUnreadAlertIDs
            sSQL = "delete from tbl_alert where BUSINESS_UNIT='" + sBU + "' AND ALERT_ID = " + sID
            DoUpdateTable("", "", sSQL, connectionString)
        Next

        For Each sID As String In aDeleteAllUnreadAlertIDs
            sSQL = "delete from tbl_alert where BUSINESS_UNIT='" + sBU + "' AND ALERT_ID = " + sID + " AND [READ] <> 1"
            DoUpdateTable("", "", sSQL, connectionString)
        Next

        For Each sID As String In aDeleteAllReadAndUnreadWithinDatesAlertIDs

            sSQL = "delete from tbl_alert where BUSINESS_UNIT='" + sBU + "' AND ALERT_ID = " + sID + _
                    " AND [ACTIVATION_START_DATETIME] <= @START_DATETIME" + " AND [ACTIVATION_END_DATETIME] >= @END_DATETIME"

            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(sSQL, con)
                With cmd.Parameters
                    .Clear()
                    .Add("@START_DATETIME", SqlDbType.DateTime).Value = DateTime.Now
                    .Add("@END_DATETIME", SqlDbType.DateTime).Value = DateTime.Now
                End With
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        Next

    End Sub

    Private Shared Sub DoUpdateTable_tbl_activity_questions(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_questions_work"
        Const destinationTable As String = "tbl_activity_questions"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("QUESTION_TEXT", New sqlField("QUESTION_TEXT", "STRING"))
            .Add("ANSWER_TYPE", New sqlField("ANSWER_TYPE", "STRING"))
            .Add("ALLOW_SELECT_OTHER_OPTION", New sqlField("ALLOW_SELECT_OTHER_OPTION", "BIT"))
            .Add("MANDATORY", New sqlField("MANDATORY", "BIT"))
            .Add("PRICE_BAND_LIST", New sqlField("PRICE_BAND_LIST", "STRING"))
            .Add("REGULAR_EXPRESSION", New sqlField("REGULAR_EXPRESSION", "STRING"))
            .Add("HYPERLINK", New sqlField("HYPERLINK", "STRING"))
            .Add("REMEMBERED_ANSWER", New sqlField("REMEMBERED_ANSWER", "BIT"))
            .Add("ASK_QUESTION_PER_HOSPITALITY_BOOKING", New sqlField("ASK_QUESTION_PER_HOSPITALITY_BOOKING", "BIT"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("QUESTION_ID", New sqlField("QUESTION_ID", "STRING"))
            .Add("QUESTION_TEXT", New sqlField("QUESTION_TEXT", "STRING"))
            .Add("ANSWER_TYPE", New sqlField("ANSWER_TYPE", "STRING"))
            .Add("ALLOW_SELECT_OTHER_OPTION", New sqlField("ALLOW_SELECT_OTHER_OPTION", "BIT"))
            .Add("MANDATORY", New sqlField("MANDATORY", "BIT"))
            .Add("PRICE_BAND_LIST", New sqlField("PRICE_BAND_LIST", "STRING"))
            .Add("REGULAR_EXPRESSION", New sqlField("REGULAR_EXPRESSION", "STRING"))
            .Add("HYPERLINK", New sqlField("HYPERLINK", "STRING"))
            .Add("REMEMBERED_ANSWER", New sqlField("REMEMBERED_ANSWER", "BIT"))
            .Add("ASK_QUESTION_PER_HOSPITALITY_BOOKING", New sqlField("ASK_QUESTION_PER_HOSPITALITY_BOOKING", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("QUESTION_ID", New sqlField("QUESTION_ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_questions")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_questions_answer_categories(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_questions_answer_categories_work"
        Const destinationTable As String = "tbl_activity_questions_answer_categories"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("CATEGORY_NAME", New sqlField("CATEGORY_NAME", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("CATEGORY_ID", New sqlField("CATEGORY_ID", "STRING"))
            .Add("CATEGORY_NAME", New sqlField("CATEGORY_NAME", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("CATEGORY_ID", New sqlField("CATEGORY_ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_questions_answer_categories")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_questions_answers(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_questions_answers_work"
        Const destinationTable As String = "tbl_activity_questions_answers"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("CATEGORY_ID", New sqlField("CATEGORY_ID", "STRING"))
            .Add("ANSWER_TEXT", New sqlField("ANSWER_TEXT", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ANSWER_ID", New sqlField("ANSWER_ID", "STRING"))
            .Add("CATEGORY_ID", New sqlField("CATEGORY_ID", "STRING"))
            .Add("ANSWER_TEXT", New sqlField("ANSWER_TEXT", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ANSWER_ID", New sqlField("ANSWER_ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_questions_answers")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_templates(ByVal connectionString As String, ByVal sBU As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_templates_work"
        Const destinationTable As String = "tbl_activity_templates"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("TEMPLATE_PER_PRODUCT", New sqlField("TEMPLATE_PER_PRODUCT", "BIT"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("TEMPLATE_ID", New sqlField("TEMPLATE_ID", "STRING"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("TEMPLATE_TYPE", New sqlField("TEMPLATE_TYPE", "STRING"))
            .Add("TEMPLATE_PER_PRODUCT", New sqlField("TEMPLATE_PER_PRODUCT", "BIT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("TEMPLATE_ID", New sqlField("TEMPLATE_ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields, sBU)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields, sBU)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields, sBU)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_templates")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_templates_detail(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_templates_detail_work"
        Const destinationTable As String = "tbl_activity_templates_detail"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("TEMPLATE_ID", New sqlField("TEMPLATE_ID", "STRING"))
            .Add("QUESTION_ID", New sqlField("QUESTION_ID", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ID", New sqlField("ID", "STRING"))
            .Add("TEMPLATE_ID", New sqlField("TEMPLATE_ID", "STRING"))
            .Add("QUESTION_ID", New sqlField("QUESTION_ID", "STRING"))
            .Add("SEQUENCE", New sqlField("SEQUENCE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_templates_detail")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_questions_with_answers(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_questions_with_answers_work"
        Const destinationTable As String = "tbl_activity_questions_with_answers"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("QUESTION_ID", New sqlField("QUESTION_ID", "STRING"))
            .Add("ANSWER_ID", New sqlField("ANSWER_ID", "STRING"))
            .Add("CATEGORY_ID", New sqlField("CATEGORY_ID", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ID", New sqlField("ID", "STRING"))
            .Add("QUESTION_ID", New sqlField("QUESTION_ID", "STRING"))
            .Add("ANSWER_ID", New sqlField("ANSWER_ID", "STRING"))
            .Add("CATEGORY_ID", New sqlField("CATEGORY_ID", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_questions_with_answers")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_status(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_status_work"
        Const destinationTable As String = "tbl_activity_status"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("STATUS_ID", New sqlField("STATUS_ID", "BIGINT"))
            .Add("TYPE_ID", New sqlField("TYPE_ID", "BIGINT"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("ID", New sqlField("ID", "BIGINT"))
            .Add("STATUS_ID", New sqlField("STATUS_ID", "BIGINT"))
            .Add("TYPE_ID", New sqlField("TYPE_ID", "BIGINT"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("ID", New sqlField("ID", "BIGINT"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_status")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_status_description(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_status_description_work"
        Const destinationTable As String = "tbl_activity_status_description"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("STATUS_ID", New sqlField("STATUS_ID", "BIGINT"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("DESCRIPTION", New sqlField("DESCRIPTION", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("STATUS_ID", New sqlField("STATUS_ID", "BIGINT"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_status_description")
    End Sub
    Private Shared Sub DoUpdateTable_tbl_activity_template_type(ByVal connectionString As String)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Const sourceTable As String = "tbl_activity_template_type_work"
        Const destinationTable As String = "tbl_activity_template_type"

        'Build a dictionary of the field mappings
        Dim fieldMappings0 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings0
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("HIDE_IN_MAINTENANCE", New sqlField("HIDE_IN_MAINTENANCE", "STRING"))
            .Add("HIDE_IN_ACTIVITIES_PAGE", New sqlField("HIDE_IN_ACTIVITIES_PAGE", "STRING"))
            .Add("LOCAL_ROOT_DIRECTORY", New sqlField("LOCAL_ROOT_DIRECTORY", "STRING"))
            .Add("REMOTE_ROOT_DIRECTORY", New sqlField("REMOTE_ROOT_DIRECTORY", "STRING"))
            .Add("MAX_FILE_UPLOAD_SIZE", New sqlField("MAX_FILE_UPLOAD_SIZE", "STRING"))
            .Add("ALLOWABLE_FILE_TYPES", New sqlField("ALLOWABLE_FILE_TYPES", "STRING"))
            .Add("ACTIVE", New sqlField("ACTIVE", "STRING"))
        End With

        'Build a dictionary of the field mappings
        Dim fieldMappings1 As New Generic.Dictionary(Of String, sqlField)
        With fieldMappings1
            .Add("TYPE_ID", New sqlField("TYPE_ID", "BIGINT"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
            .Add("HIDE_IN_MAINTENANCE", New sqlField("HIDE_IN_MAINTENANCE", "STRING"))
            .Add("HIDE_IN_ACTIVITIES_PAGE", New sqlField("HIDE_IN_ACTIVITIES_PAGE", "STRING"))
            .Add("LOCAL_ROOT_DIRECTORY", New sqlField("LOCAL_ROOT_DIRECTORY", "STRING"))
            .Add("REMOTE_ROOT_DIRECTORY", New sqlField("REMOTE_ROOT_DIRECTORY", "STRING"))
            .Add("MAX_FILE_UPLOAD_SIZE", New sqlField("MAX_FILE_UPLOAD_SIZE", "STRING"))
            .Add("ALLOWABLE_FILE_TYPES", New sqlField("ALLOWABLE_FILE_TYPES", "STRING"))
            .Add("ACTIVE", New sqlField("ACTIVE", "STRING"))
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, sqlField)
        With keyFields
            .Add("TYPE_ID", New sqlField("TYPE_ID", "BIGINT"))
            .Add("BUSINESS_UNIT", New sqlField("BUSINESS_UNIT", "STRING"))
            .Add("NAME", New sqlField("NAME", "STRING"))
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings0, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings1, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString, connectionString, "tbl_activity_template_type")
    End Sub

    Private Shared Sub DoUpdateTable(ByVal updateCommand As String, ByVal insertCommand As String, ByVal deleteCommand As String, ByVal connectionString As String, Optional ByVal sIdentityTable As String = "")
        '=====================================================================================
        ' 1/. UPDATE the DESTINATION Records from the SOURCE Records
        '=====================================================================================
        If Not String.IsNullOrEmpty(updateCommand) Then
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(updateCommand, con)
                cmd.CommandTimeout = 120 'seconds
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If
        '=====================================================================================
        ' 2/. INSERT new Records into the DESTINATION Table
        '=====================================================================================
        If Not String.IsNullOrEmpty(insertCommand) Then
            If sIdentityTable = "" Then
                Using con As New SqlConnection(connectionString)
                    Dim cmd As New SqlCommand(insertCommand, con)
                    ' cmd.CommandTimeout = DefaultSQLTimeout()
                    con.Open()
                    cmd.ExecuteNonQuery()
                    con.Close()
                End Using
            Else
                Using con As New SqlConnection(connectionString)
                    Dim sCmd00 As String = "SET IDENTITY_INSERT " + sIdentityTable + " ON;"
                    Dim cmd00 As New SqlCommand(sCmd00.ToString(), con)
                    Dim sCmd99 As String = "SET IDENTITY_INSERT " + sIdentityTable + " ON;"
                    Dim cmd99 As New SqlCommand(sCmd99.ToString(), con)
                    Dim cmd As New SqlCommand(insertCommand, con)
                    ' cmd.CommandTimeout = DefaultSQLTimeout()
                    con.Open()
                    cmd00.ExecuteNonQuery()
                    cmd.ExecuteNonQuery()
                    cmd99.ExecuteNonQuery()
                    con.Close()
                End Using
            End If
        End If

        '=====================================================================================
        ' 3/. DELETE Records from the DESTINATION Table that do NOT Exist in the EXTRACT Table
        '=====================================================================================
        If Not String.IsNullOrEmpty(deleteCommand) Then
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(deleteCommand, con)
                ' cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If
    End Sub
    Private Shared Function BuildUpdateSQL(ByVal updateTableName As String, ByVal sourceTableName As String, ByVal fieldMappings As Collections.Generic.Dictionary(Of String, sqlField), _
                                ByVal keyFields As Collections.Generic.Dictionary(Of String, sqlField), _
                                Optional ByVal sBU As String = "") As StringBuilder
        Dim count As Integer = 0
        Dim update As New StringBuilder
        With update
            .Append("UPDATE " & updateTableName & " ")
            .Append("SET ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key & " = ( " & sourceTableName & "." & fieldMappings(key).FieldName & " ) ")
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append("	FROM " & sourceTableName & " ")
            .Append("		WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                Select Case keyFields(key).FieldType
                    Case Is = "STRING"
                        .Append("   Isnull(" & sourceTableName & "." & keyFields(key).FieldName & ",'') COLLATE DATABASE_DEFAULT = Isnull(" & updateTableName & "." & key & ",'') COLLATE DATABASE_DEFAULT ")
                    Case Else
                        .Append("   Isnull(" & sourceTableName & "." & keyFields(key).FieldName & ",'') = Isnull(" & updateTableName & "." & key & ",'') ")
                End Select
            Next
            .Append("		AND EXISTS ")
            .Append("			( ")
            .Append("				SELECT * ")
            .Append("				FROM " & updateTableName & "  ")
            .Append("				WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                Select Case keyFields(key).FieldType
                    Case Is = "STRING"
                        .Append("   Isnull(" & updateTableName & "." & key & ",'') COLLATE DATABASE_DEFAULT = Isnull(" & sourceTableName & "." & keyFields(key).FieldName & ",'') COLLATE DATABASE_DEFAULT ")
                    Case Else
                        .Append("   Isnull(" & updateTableName & "." & key & ",'') = Isnull(" & sourceTableName & "." & keyFields(key).FieldName & ",'')  ")
                End Select
            Next
            .Append("			) ")
            If sBU <> "" Then
                .Append(" AND " & sourceTableName & ".BUSINESS_UNIT IN ('*ALL','" & sBU & "') ")
            End If
        End With
        Return update

    End Function
    Private Shared Function BuildInsertSQL(ByVal insertTableName As String, ByVal sourceTableName As String, ByVal fieldMappings As Collections.Generic.Dictionary(Of String, sqlField), _
                                      ByVal keyFields As Collections.Generic.Dictionary(Of String, sqlField), _
                                      Optional ByVal sBU As String = "") As StringBuilder
        Dim insert As New StringBuilder
        Dim count As Integer = 0
        With insert
            'Add the INSERT columns            
            .Append("INSERT INTO " & insertTableName & "( ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key)
                If count = fieldMappings.Keys.Count Then .Append("   ) ") Else .Append("   , ")
            Next

            'Add the SOURCE columns
            .Append("   SELECT ")
            count = 0 'reset the counter
            For Each key As String In fieldMappings.Keys
                count += 1
                ' Rtrim removed as it doesn't work with ntext fields
                '.Append("           RTRIM(" & sourceTableName & "." & fieldMappings(key) & ") AS Expr" & count)
                .Append("           " & sourceTableName & "." & fieldMappings(key).FieldName & " AS Expr" & count)
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append("   FROM " & sourceTableName)
            .Append(" WHERE NOT EXISTS (")
            .Append("              SELECT * ")
            .Append("              FROM " & insertTableName & " ")
            .Append("              WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                Select Case keyFields(key).FieldType
                    Case Is = "STRING"
                        .Append("   Isnull(" & insertTableName & "." & key & ",'') COLLATE DATABASE_DEFAULT = Isnull(" & sourceTableName & "." & keyFields(key).FieldName & ",'') COLLATE DATABASE_DEFAULT ")
                    Case Else
                        .Append("   Isnull(" & insertTableName & "." & key & ",'') = Isnull(" & sourceTableName & "." & keyFields(key).FieldName & ",'') ")
                End Select
            Next
            .Append(" )")
            If sBU <> "" Then
                .Append(" AND " & sourceTableName & ".BUSINESS_UNIT IN ('*ALL','" & sBU & "') ")
            End If
        End With
        Return insert
    End Function
    Private Shared Function BuildDeleteSQL(ByVal deleteTableName As String, ByVal sourceTableName As String, _
                                       ByVal keyFields As Collections.Generic.Dictionary(Of String, sqlField), _
                                       Optional ByVal sBU As String = "") As StringBuilder

        Dim delete As New StringBuilder
        Dim count As Integer = 0
        With delete
            .Append("DELETE FROM " & deleteTableName)
            .Append("   WHERE NOT EXISTS (")
            .Append("           SELECT * ")
            .Append("               FROM " & sourceTableName)
            '            .Append("               FROM " & sourceTableName & ", " & deleteTableName)
            .Append("               WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                Select Case keyFields(key).FieldType
                    Case Is = "STRING"
                        .Append(" Isnull(" & sourceTableName & "." & key & ",'') COLLATE DATABASE_DEFAULT = Isnull(" & deleteTableName & "." & keyFields(key).FieldName & ",'') COLLATE DATABASE_DEFAULT ")
                    Case Else
                        .Append(" Isnull(" & sourceTableName & "." & key & ",'') = Isnull(" & deleteTableName & "." & keyFields(key).FieldName & ",'') ")
                End Select
            Next
            .Append(" )")
            If sBU <> "" Then
                .Append(" AND " & deleteTableName & ".BUSINESS_UNIT IN ('*ALL','" & sBU & "') ")
            End If
        End With
        Return delete
    End Function

    Private Sub LoadTypes()
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As Talent.Common.DESettings = New DESettings()
        Dim dtMaintenancePublishTypes As New DataTable
        Dim dtBusinessUnits As New DataTable
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        tDataObjects.Settings = settings

        dtMaintenancePublishTypes = tDataObjects.MaintenanceSettings.TblMaintenancePublishTypes.GetAllByBUPartner(_QSBusinessUnit, _QSPartner)

        If dtMaintenancePublishTypes IsNot Nothing AndAlso dtMaintenancePublishTypes.Rows.Count > 0 Then
            rptType.DataSource = dtMaintenancePublishTypes
            rptType.DataBind()
        End If
        tDataObjects = Nothing
    End Sub

    Private Sub RetrieveCMSSettings()
        '----------------------------------------------
        ' Check which hosts are defined for live update
        '----------------------------------------------
        If Not ConfigurationManager.AppSettings("liveUpdateServer01") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer01") <> String.Empty Then
            _liveUpdateServer01 = ConfigurationManager.AppSettings("liveUpdateServer01")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer02") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer02") <> String.Empty Then
            _liveUpdateServer02 = ConfigurationManager.AppSettings("liveUpdateServer02")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer03") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer03") <> String.Empty Then
            _liveUpdateServer03 = ConfigurationManager.AppSettings("liveUpdateServer03")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer04") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer04") <> String.Empty Then
            _liveUpdateServer04 = ConfigurationManager.AppSettings("liveUpdateServer04")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer05") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer05") <> String.Empty Then
            _liveUpdateServer05 = ConfigurationManager.AppSettings("liveUpdateServer05")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer06") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer06") <> String.Empty Then
            _liveUpdateServer06 = ConfigurationManager.AppSettings("liveUpdateServer06")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer07") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer07") <> String.Empty Then
            _liveUpdateServer07 = ConfigurationManager.AppSettings("liveUpdateServer07")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer08") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer08") <> String.Empty Then
            _liveUpdateServer08 = ConfigurationManager.AppSettings("liveUpdateServer08")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer09") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer09") <> String.Empty Then
            _liveUpdateServer09 = ConfigurationManager.AppSettings("liveUpdateServer09")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateServer10") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateServer10") <> String.Empty Then
            _liveUpdateServer10 = ConfigurationManager.AppSettings("liveUpdateServer10")
        End If

        '--------------------------------------------------
        ' Check which databases are defined for live update
        '---------------------------------------------------
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase01") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase01") <> String.Empty Then
            _liveUpdateDatabase01 = ConfigurationManager.AppSettings("liveUpdateDatabase01")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase02") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase02") <> String.Empty Then
            _liveUpdateDatabase02 = ConfigurationManager.AppSettings("liveUpdateDatabase02")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase03") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase03") <> String.Empty Then
            _liveUpdateDatabase03 = ConfigurationManager.AppSettings("liveUpdateDatabase03")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase04") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase04") <> String.Empty Then
            _liveUpdateDatabase04 = ConfigurationManager.AppSettings("liveUpdateDatabase04")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase05") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase05") <> String.Empty Then
            _liveUpdateDatabase05 = ConfigurationManager.AppSettings("liveUpdateDatabase05")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase06") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase06") <> String.Empty Then
            _liveUpdateDatabase06 = ConfigurationManager.AppSettings("liveUpdateDatabase06")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase07") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase07") <> String.Empty Then
            _liveUpdateDatabase07 = ConfigurationManager.AppSettings("liveUpdateDatabase07")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase08") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase08") <> String.Empty Then
            _liveUpdateDatabase08 = ConfigurationManager.AppSettings("liveUpdateDatabase08")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase09") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase09") <> String.Empty Then
            _liveUpdateDatabase08 = ConfigurationManager.AppSettings("liveUpdateDatabase08")
        End If
        If Not ConfigurationManager.AppSettings("liveUpdateDatabase10") Is Nothing AndAlso ConfigurationManager.AppSettings("liveUpdateDatabase10") <> String.Empty Then
            _liveUpdateDatabase10 = ConfigurationManager.AppSettings("liveUpdateDatabase010")
        End If
    End Sub

    Private Sub PublishFiles(ByVal sArg As String, ByRef ErrLabel As BulletedList, ByVal sType As String, ByRef lbLastUpdated As Label, ByVal ProcessChildDirectories As Boolean)
        Dim sPaths As String() = sArg.Split(New Char() {";"c})
        Dim localHtmlDirectory As String = sPaths(0).ToString
        Dim updateHtmlPath As String = sPaths(1).ToString
        If Not localHtmlDirectory.ToString.EndsWith("\") Then localHtmlDirectory += "\"
        If Not updateHtmlPath.ToString.EndsWith("/") Then updateHtmlPath += "/"
        Dim success As Boolean = False
        Dim lastUpdated As Date = ConvertLastUpdated(lbLastUpdated.Text)
        ErrLabel.Items.Clear()

        '-----------------------------------
        ' Use ftpClient for each live server
        '-----------------------------------
        '----
        ' 01 
        '----
        If _liveUpdateServer01 <> String.Empty Then
            Dim ftpUser01 As String = ConfigurationManager.AppSettings("liveUpdateServer01FtpUser")
            Dim ftpPassword01 As String = ConfigurationManager.AppSettings("liveUpdateServer01FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer01, ftpUser01, ftpPassword01)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer01, "File published successfully.")
        End If
        '----
        ' 02 
        '----
        If _liveUpdateServer02 <> String.Empty Then
            Dim ftpUser02 As String = ConfigurationManager.AppSettings("liveUpdateServer02FtpUser")
            Dim ftpPassword02 As String = ConfigurationManager.AppSettings("liveUpdateServer02FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer02, ftpUser02, ftpPassword02)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer02, "File published successfully.")
        End If
        '----
        ' 03 
        '----
        If _liveUpdateServer03 <> String.Empty Then
            Dim ftpUser03 As String = ConfigurationManager.AppSettings("liveUpdateServer03FtpUser")
            Dim ftpPassword03 As String = ConfigurationManager.AppSettings("liveUpdateServer03FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer03, ftpUser03, ftpPassword03)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer03, "File published successfully.")
        End If
        '----
        ' 04
        '----
        If _liveUpdateServer04 <> String.Empty Then
            Dim ftpUser04 As String = ConfigurationManager.AppSettings("liveUpdateServer04FtpUser")
            Dim ftpPassword04 As String = ConfigurationManager.AppSettings("liveUpdateServer04FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer04, ftpUser04, ftpPassword04)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer04, "File published successfully.")
        End If
        '----
        ' 05
        '----
        If _liveUpdateServer05 <> String.Empty Then
            Dim ftpUser05 As String = ConfigurationManager.AppSettings("liveUpdateServer05FtpUser")
            Dim ftpPassword05 As String = ConfigurationManager.AppSettings("liveUpdateServer05FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer05, ftpUser05, ftpPassword05)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer05, "File published successfully.")
        End If
        '----
        ' 06
        '----
        If _liveUpdateServer06 <> String.Empty Then
            Dim ftpUser06 As String = ConfigurationManager.AppSettings("liveUpdateServer06FtpUser")
            Dim ftpPassword06 As String = ConfigurationManager.AppSettings("liveUpdateServer06FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer06, ftpUser06, ftpPassword06)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer06, "File published successfully.")
        End If
        '----
        ' 07 
        '----
        If _liveUpdateServer07 <> String.Empty Then
            Dim ftpUser07 As String = ConfigurationManager.AppSettings("liveUpdateServer07FtpUser")
            Dim ftpPassword07 As String = ConfigurationManager.AppSettings("liveUpdateServer07FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer07, ftpUser07, ftpPassword07)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer07, "File published successfully.")
        End If
        '----
        ' 08 
        '----
        If _liveUpdateServer08 <> String.Empty Then
            Dim ftpUser08 As String = ConfigurationManager.AppSettings("liveUpdateServer08FtpUser")
            Dim ftpPassword08 As String = ConfigurationManager.AppSettings("liveUpdateServer08FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer08, ftpUser08, ftpPassword08)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer08, "File published successfully.")
        End If
        '----
        ' 09 
        '----
        If _liveUpdateServer09 <> String.Empty Then
            Dim ftpUser09 As String = ConfigurationManager.AppSettings("liveUpdateServer09FtpUser")
            Dim ftpPassword09 As String = ConfigurationManager.AppSettings("liveUpdateServer09FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer09, ftpUser09, ftpPassword09)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer09, "File published successfully.")
        End If
        '----
        ' 10 
        '----
        If _liveUpdateServer10 <> String.Empty Then
            Dim ftpUser10 As String = ConfigurationManager.AppSettings("liveUpdateServer10FtpUser")
            Dim ftpPassword10 As String = ConfigurationManager.AppSettings("liveUpdateServer10FtpPassword")
            _myFtp = New Talent.Maintenance.FTPclient(_liveUpdateServer10, ftpUser10, ftpPassword10)
            _errorMsg = String.Empty
            success = UploadDirectory(localHtmlDirectory, updateHtmlPath, lastUpdated, ProcessChildDirectories)
            UpdateErrorLabel(ErrLabel, success, "Server: " + _liveUpdateServer10, "File published successfully.")
        End If

        If success Then
            Dim sLastUpdated = Now.ToString("dd/MM/yyyy HH:mm:ss")
            Dim boolUpdate As Boolean = Me.SaveLastUpdated(sType, sLastUpdated)
            If boolUpdate Then
                lbLastUpdated.Text = sLastUpdated
            Else
                ErrLabel.Items.Add("(Last update date not saved correctly)")
            End If
        End If
    End Sub

    Private Sub UploadFiles(ByVal sArg As String, ByVal ctlFileUpload As FileUpload, ByVal boolOverwriteFile As Boolean, ByRef ErrLabel As BulletedList)
        Dim sPaths As String() = sArg.Split(New Char() {";"c})
        Dim sRootPath As String = sPaths(0)
        Dim maxFileUploadSize As Integer = CType(sPaths(1).ToString.Trim, Integer)
        Dim allowableFileTypes As String() = sPaths(2).ToString.Split(New Char() {","c})
        If Not sRootPath.ToString.EndsWith("\") Then sRootPath += "\"
        ErrLabel.Items.Clear()

        If Not ctlFileUpload.HasFile Then
            ErrLabel.Items.Add("Please enter a file name into the 'Upload File' box.")
        Else
            Try
                If IO.Directory.Exists(sRootPath) Then

                    'Check file is not 0 length
                    Dim hasContent As Boolean = False
                    Dim validSize As Boolean = False
                    If ctlFileUpload.PostedFile.ContentLength > 0 Then
                        hasContent = True
                    End If

                    If hasContent Then
                        If ctlFileUpload.PostedFile.ContentLength <= maxFileUploadSize Then
                            validSize = True
                        End If

                        If validSize Then
                            'Check for valid file extensions
                            Dim validFile As Boolean = False
                            Dim fileExe As String = IO.Path.GetExtension(ctlFileUpload.PostedFile.FileName).ToLower
                            For Each Str As String In allowableFileTypes
                                If fileExe = Str Then
                                    validFile = True
                                    Exit For
                                End If
                            Next
                            If Not validFile Then
                                ErrLabel.Items.Add("Files of type '" & fileExe & "' cannot be uploaded for security reasons.")
                            Else
                                If IO.File.Exists(sRootPath & ctlFileUpload.FileName) AndAlso Not boolOverwriteFile Then
                                    ErrLabel.Visible = True
                                    ErrLabel.Items.Add("The file already exists. Please re-name it, or tick the overwrite box and try again.")
                                Else
                                    ctlFileUpload.SaveAs(sRootPath & ctlFileUpload.FileName)
                                End If
                            End If
                        Else
                            ErrLabel.Visible = True
                            ErrLabel.Items.Add("File '" & ctlFileUpload.FileName & "' is bigger than the maximum upload file size. Max file size = " & (maxFileUploadSize / 1024).ToString & "KB")
                        End If
                    Else
                        ErrLabel.Visible = True
                        ErrLabel.Items.Add("File '" & ctlFileUpload.FileName & "' is empty. No file was uploaded.")
                    End If
                Else
                    ErrLabel.Visible = True
                    ErrLabel.Items.Add("The folder you are trying to upload files to does not exist. Please contact your administrator.")
                End If
            Catch ex As Exception
                ErrLabel.Visible = True
                ErrLabel.Items.Add("There was an error uploading the file, please try again.")
            End Try

            If ErrLabel.Items.Count = 0 Then
                ErrLabel.Visible = True
                ErrLabel.Items.Add("File " & ctlFileUpload.FileName & " successfully uploaded.")
            End If
        End If
    End Sub

    Private Sub UploadImages(ByVal sArg As String, ByVal ctlImageUpload As FileUpload, ByVal boolOverwriteFile As Boolean, ByRef ErrLabel As BulletedList)
        Dim sPaths As String() = sArg.Split(New Char() {";"c})
        Dim sRootPath As String = sPaths(0)
        Dim maxFileUploadSize As Integer = CType(sPaths(1).ToString.Trim, Integer)
        Dim allowableFileTypes As String() = sPaths(2).ToString.Split(New Char() {","c})
        If Not sRootPath.ToString.EndsWith("\") Then sRootPath += "\"
        ErrLabel.Items.Clear()

        If Not ctlImageUpload.HasFile Then
            ErrLabel.Items.Add("Please enter a file name into the 'Image Upload' box.")
        Else
            Try
                If IO.Directory.Exists(sRootPath) Then

                    'Check file is not 0 length
                    Dim hasContent As Boolean = False
                    Dim validSize As Boolean = False
                    If ctlImageUpload.PostedFile.ContentLength > 0 Then
                        hasContent = True
                    End If
                    If hasContent Then
                        If ctlImageUpload.PostedFile.ContentLength <= maxFileUploadSize Then
                            validSize = True
                        End If
                        If validSize Then
                            'Check for valid file extensions
                            Dim validFile As Boolean = False
                            Dim fileExe As String = IO.Path.GetExtension(ctlImageUpload.PostedFile.FileName).ToLower
                            For Each Str As String In allowableFileTypes
                                If fileExe = Str Then
                                    validFile = True
                                    Exit For
                                End If
                            Next
                            If Not validFile Then
                                ErrLabel.Visible = True
                                ErrLabel.Items.Add("Files of type '" & fileExe & "' cannot be uploaded for security reasons.")
                            Else
                                If IO.File.Exists(sRootPath & ctlImageUpload.FileName) AndAlso Not boolOverwriteFile Then
                                    ErrLabel.Visible = True
                                    ErrLabel.Items.Add("The image already exists. Please re-name it, or tick the overwrite box and try again.")
                                Else
                                    ctlImageUpload.SaveAs(sRootPath & ctlImageUpload.FileName)
                                End If
                            End If
                        Else
                            ErrLabel.Visible = True
                            ErrLabel.Items.Add("File '" & ctlImageUpload.FileName & "' is bigger than the maximum image size. Max image size = " & (maxFileUploadSize / 1024).ToString & "KB")
                        End If
                    Else
                        ErrLabel.Visible = True
                        ErrLabel.Items.Add("File '" & ctlImageUpload.FileName & "' is empty. No file was uploaded.")
                    End If
                Else
                    ErrLabel.Visible = True
                    ErrLabel.Items.Add("The folder you are trying to upload to does not exist. Please contact your administrator.")
                End If
            Catch ex As Exception
                ErrLabel.Visible = True
                ErrLabel.Items.Add("There was an error uploading the Image, please try again.")
            End Try

            If ErrLabel.Items.Count = 0 Then
                ErrLabel.Visible = True
                ErrLabel.Items.Add("Image " & ctlImageUpload.FileName & " successfully uploaded.")
            End If
        End If
    End Sub

    Private Sub ClearCache(ByRef ErrLabel As BulletedList)
        ErrLabel.Items.Clear()
        Dim urlList As New Collection
        populateUrlList(urlList)
        For Each url As String In urlList
            Dim cacheClearedOkString As String = _wfrPage.Content("CacheClearedOkText", _languageCode, True)
            Dim cacheClearedFaieldString As String = _wfrPage.Content("CacheClearedFailedText", _languageCode, True)
            Dim exceptionString As String = String.Empty
            If clearCacheForUrl(url, exceptionString) Then
                ErrLabel.Items.Add(cacheClearedOkString & url)
            Else
                cacheClearedFaieldString = cacheClearedFaieldString.Replace("<<URL>>", url)
                cacheClearedFaieldString = cacheClearedFaieldString.Replace("<<EXCEPTION>>", exceptionString)
                ErrLabel.Items.Add(cacheClearedFaieldString)
            End If
        Next
    End Sub

    Private Function PublishCMS(ByVal sArg As String, ByRef ErrLabel As BulletedList, ByVal sType As String, ByRef lbLastUpdated As Label) As Boolean
        Dim success As Boolean = False
        Dim lastUpdated As Date = ConvertLastUpdated(lbLastUpdated.Text)
        ErrLabel.Items.Clear()

        If Not sArg.IndexOf("True") > -1 Then
            ErrLabel.Items.Add("Please select at least one option to publish.")
            Return False
            Exit Function
        End If

        '--------------------------------------------------
        ' Check which databases are defined for live update
        '--------------------------------------------------
        If _liveUpdateDatabase01 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase01, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase01")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase02 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase02, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase02")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase03 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase03, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase03")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase04 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase04, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase04")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase05 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase05, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase05")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase06 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase06, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase06")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase07 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase07, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase07")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase08 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase08, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase08")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase09 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase09, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase09")), "CMS updated successfully.")
        End If
        If _liveUpdateDatabase10 <> String.Empty Then
            success = UploadCMS(_liveUpdateDatabase10, sArg)
            UpdateErrorLabel(ErrLabel, success, ExtractSQLServer(ConfigurationManager.AppSettings("liveUpdateDatabase10")), "CMS updated successfully.")
        End If

        If success Then
            Dim sLastUpdated = Now.ToString("dd/MM/yyyy HH:mm:ss")
            Dim boolUpdate As Boolean = Me.SaveLastUpdated(sType, sLastUpdated)
            If boolUpdate Then
                lbLastUpdated.Text = sLastUpdated
            Else
                ErrLabel.Items.Add("(Last update date not saved correctly)")
            End If
        End If
        Return success
    End Function

    Private Sub UpdateErrorLabel(ByVal ErrLabel As BulletedList, ByVal success As Boolean, ByVal sServer As String, ByVal sMessage As String)
        If Not success Then
            ErrLabel.Items.Add(_errorMsg & " (" + sServer + ")")
        Else
            ErrLabel.Items.Add(sMessage + " (" + sServer + ")")
        End If
    End Sub

#End Region

#Region "sqlField Class"

    Private Class sqlField

        Private _fieldName As String
        Private _fieldType As String

        Public Property FieldName() As String
            Get
                Return _fieldName
            End Get
            Set(ByVal value As String)
                _fieldName = value
            End Set
        End Property
        Public Property FieldType() As String
            Get
                Return _fieldType
            End Get
            Set(ByVal value As String)
                _fieldType = value
            End Set
        End Property

        Public Sub New(ByVal fieldName As String, ByVal fieldType As String)
            _fieldName = fieldName
            _fieldType = fieldType
        End Sub

    End Class

#End Region

End Class
