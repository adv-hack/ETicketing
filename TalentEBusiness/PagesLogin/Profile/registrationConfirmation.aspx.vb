Imports Talent.eCommerce

Partial Class PagesPublic_registrationConfirmation
    Inherits TalentBase01

    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = String.Empty
    Private _showCaptureButton As Boolean = False

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "registrationConfirmation.aspx"
            MessageLabel.Text = .Content("MessageText", _languageCode, True)
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        _showCaptureButton = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowCaptureButton"))
        If _showCaptureButton Then
            CameraButtonPanel.Visible = True
        Else
            CameraButtonPanel.Visible = False
        End If
        'Replace the place holders with real time data
        If Not Profile.IsAnonymous Then
            If Not String.IsNullOrEmpty(Request.QueryString("customer")) Then
                MessageLabel.Text = MessageLabel.Text.Replace("<<CustomerNumber>>", Request.QueryString("customer").ToString.Trim)
                MessageLabel.Text = MessageLabel.Text.Replace("<<FullName>>", "")
            Else
                MessageLabel.Text = MessageLabel.Text.Replace("<<CustomerNumber>>", Profile.User.Details.Account_No_1.ToString.Trim)
                MessageLabel.Text = MessageLabel.Text.Replace("<<FullName>>", Profile.User.Details.Full_Name.ToString.Trim)
            End If
            capturePhotoBtn.Value = _wfr.Content("capturePhotoBtn", _languageCode, True)
        End If
    End Sub

    Protected Sub capturePhotoBtn_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles capturePhotoBtn.Load
        If _showCaptureButton Then
            Dim photoProgramUrl As String = _wfr.Attribute("PhotoProgramCaptureUrl")
            If photoProgramUrl.Contains("http://") Then
                Dim querystring As New StringBuilder
                querystring.Append("fileName=")
                If Not String.IsNullOrEmpty(Request.QueryString("customer")) Then
                    querystring.Append(Request.QueryString("customer").ToString.Trim())
                Else
                    querystring.Append(Profile.User.Details.Account_No_1.ToString.Trim)
                End If
                querystring.Append("&folderPath=")
                querystring.Append(_wfr.Attribute("PhotoFolderPath"))
                querystring.Append("&photoEffects=")
                querystring.Append(_wfr.Attribute("PhotoEffects"))
                querystring.Append("&exposureMode=")
                querystring.Append(_wfr.Attribute("ExposureMode"))
                querystring.Append("&AFDistance=")
                querystring.Append(_wfr.Attribute("AFDistance"))
                querystring.Append("&meteringMode=")
                querystring.Append(_wfr.Attribute("MeteringMode"))
                querystring.Append("&imageSize=")
                querystring.Append(_wfr.Attribute("ImageSize"))
                querystring.Append("&imageQuality=")
                querystring.Append(_wfr.Attribute("ImageQuality"))
                querystring.Append("&zoomLevel=")
                querystring.Append(_wfr.Attribute("ZoomLevel"))
                querystring.Append("&brightnessLevel=")
                querystring.Append(_wfr.Attribute("BrightnessLevel"))
                Dim buttonClickValue As String = "location.href='" + photoProgramUrl + "?" + querystring.ToString + "'"
                capturePhotoBtn.Attributes.Add("onclick", buttonClickValue)
            End If
        End If
    End Sub

End Class
