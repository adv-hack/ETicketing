
Partial Class UserControls_UpdateDetailsConfirmation
    Inherits ControlBase

    Private _languageCode As String = Nothing
    Private _ucr As Talent.Common.UserControlResource = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "UpdateDetailsConfirmation.ascx"
        End With
        '---------------------------------------------------
        ' Check whether to display Add auto membership panel
        '---------------------------------------------------
        If Not String.IsNullOrEmpty(Request.QueryString("addauto")) _
            AndAlso Request.QueryString("addauto").ToLower = "false" Then
            With _ucr
                If Not Profile.User.Details.OWNS_AUTO_MEMBERSHIP _
                    AndAlso Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_ShowAddAutoMembership")) Then
                    Me.addMembershipCheck.Checked = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_CheckedByDefault"))
                    Me.contentLabel.Text = .Content("AddMembership_ContentLabel", _languageCode, True)
                    Me.addMembershipCheck.Text = .Content("AddMembership_CheckboxLabel", _languageCode, True)
                    Me.returnButton.Text = .Content("AddMembership_ReturnToHomepageButtonText", _languageCode, True)
                    Me.addButton.Text = .Content("AddMembership_AddAutoMembershipButtonText", _languageCode, True)
                Else
                    pnlAddMembership.Visible = False
                End If
            End With
        Else
            pnlAddMembership.Visible = False
        End If
        '---------------------------------------------
        ' Check whether to display Capture Photo Panel
        '---------------------------------------------
        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("CapturePhoto_ShowCapturePhoto")) Then
            pnlCapturePhoto.Visible = True
            capturePhotoBtn.Value = _ucr.Content("CapturePhoto_CapturePhotoButtonText", _languageCode, True)
        Else
            pnlCapturePhoto.Visible = False
        End If
    End Sub

    Protected Sub returnButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles returnButton.Click
        Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
    End Sub

    Protected Sub addButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles addButton.Click
        If Me.addMembershipCheck.Checked Then
            Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Registration.aspx&function=AddFreeMembership")
        End If
    End Sub

    Protected Sub btnCapturePhoto_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles capturePhotoBtn.Load
        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("CapturePhoto_ShowCapturePhoto")) Then
            Dim photoProgramUrl As String = _ucr.Attribute("PhotoProgramCaptureUrl")
            If photoProgramUrl.Contains("http://") Then
                Dim querystring As New StringBuilder
                querystring.Append("fileName=")
                querystring.Append(Profile.User.Details.Account_No_1.ToString.Trim)
                querystring.Append("&folderPath=")
                querystring.Append(_ucr.Attribute("PhotoFolderPath"))
                querystring.Append("&photoEffects=")
                querystring.Append(_ucr.Attribute("PhotoEffects"))
                querystring.Append("&exposureMode=")
                querystring.Append(_ucr.Attribute("ExposureMode"))
                querystring.Append("&AFDistance=")
                querystring.Append(_ucr.Attribute("AFDistance"))
                querystring.Append("&meteringMode=")
                querystring.Append(_ucr.Attribute("MeteringMode"))
                querystring.Append("&imageSize=")
                querystring.Append(_ucr.Attribute("ImageSize"))
                querystring.Append("&imageQuality=")
                querystring.Append(_ucr.Attribute("ImageQuality"))
                querystring.Append("&zoomLevel=")
                querystring.Append(_ucr.Attribute("ZoomLevel"))
                querystring.Append("&brightnessLevel=")
                querystring.Append(_ucr.Attribute("BrightnessLevel"))
                Dim buttonClickValue As String = "location.href='" + photoProgramUrl + "?" + querystring.ToString + "'"
                capturePhotoBtn.Attributes.Add("onclick", buttonClickValue)
            End If
        End If
    End Sub
    
End Class
