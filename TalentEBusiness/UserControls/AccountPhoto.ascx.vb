Imports System.IO
Imports SD = System.Drawing
Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
Imports Talent.eCommerce
Imports Talent.Common
Partial Class UserControls_AccountPhoto
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _isValidImageFile As Boolean = False
    Private _isCropRequired As Boolean = False
    Private _isResizeBeforeCrop As Boolean = False
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _imgHeighttMin As Integer = 0
    Private _imgHeighttMax As Integer = 0
    Private _imgWidthMin As Integer = 0
    Private _imgWidthMax As Integer = 0
    Private _imgFileSize As Integer = 0
    Private _imgScreenWidth As Integer = 0
    Private _imgScreenHeight As Integer = 0
    Private _fileExtension As String = String.Empty
    Private _errMessage As String = String.Empty

#End Region

#Region "Public Methods"

    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        If Not String.IsNullOrWhiteSpace(_ucr.Content(rfv.ControlToValidate & "ErrMessRFV", _languageCode, True)) Then
            rfv.Enabled = True
            rfv.ErrorMessage = _ucr.Content(rfv.ControlToValidate & "ErrMessRFV", _languageCode, True)
        Else
            rfv.Enabled = False
        End If
    End Sub

    Public Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        Dim controlRegExpression As String = _ucr.Attribute(rev.ControlToValidate & "RegExp")
        If String.IsNullOrEmpty(controlRegExpression) Then
            rev.Enabled = False
        Else
            rev.ValidationExpression = controlRegExpression
            rev.ErrorMessage = _ucr.Content(rev.ControlToValidate & "ErrMessREV", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AccountPhoto.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        pHValidationSummary.Visible = False
        AssignConfigurations()
        LoadProfilePhoto()
        If Session("BackendError") IsNot Nothing Then
            ShowError("ErrorMessBackendFail", Session("BackendError").ToString())
            Session("BackendError") = Nothing
            Session.Remove("BackendError")
        End If
    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpload.Click
        Dim isRedirect As Boolean = False
        If chkTermsAndCond.Checked Then
            If Page.IsValid Then
                Try
                    ValidateImageAssignFlags()
                    If _isValidImageFile Then
                        Dim fileNameForUser As String = GetFileNameForUser()
                        Dim tempFilePhysicalPath As String = SaveReturnFilePath(fileNameForUser)
                        If tempFilePhysicalPath.Length > 0 Then
                            If _isCropRequired Then
                                Session("WorkingImage") = fileNameForUser & _fileExtension
                                ResizeImageForScreen(tempFilePhysicalPath & fileNameForUser & _fileExtension)
                                isRedirect = True
                            Else
                                'move the file to permanent path as it satisfy all rules without cropping
                                MoveFileToPermanentPath(tempFilePhysicalPath, fileNameForUser)
                                If _errMessage.Length = 0 Then
                                    isRedirect = True
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    ShowError("ErrorMessFileSave", "")
                End Try
                If isRedirect Then
                    Response.Redirect("ProfilePhoto.aspx")
                End If
            End If
        Else
            ShowError("ErrorMessTandCs", "")
        End If
    End Sub

    Protected Sub btnCrop_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCrop.Click
        'use try catch on exception don't delete existing image
        Dim ImageName As String = String.Empty
        If chkTermsAndCond.Checked Then
            If Session("WorkingImage") IsNot Nothing Then
                Try
                    ImageName = Session("WorkingImage").ToString()
                    Session("WorkingImage") = Nothing
                    Session.Remove("WorkingImage")
                    If (W.Value.Trim = "" OrElse H.Value.Trim = "" OrElse X.Value.Trim = "" OrElse Y.Value.Trim = "") Then
                        W.Value = _imgWidthMin
                        H.Value = _imgHeighttMin
                        X.Value = "0"
                        Y.Value = "0"
                    End If
                    Dim w__1 As Integer = Convert.ToInt32(W.Value)
                    Dim h__2 As Integer = Convert.ToInt32(H.Value)
                    Dim x__3 As Integer = Convert.ToInt32(X.Value)
                    Dim y__4 As Integer = Convert.ToInt32(Y.Value)
                    Dim tempImagePath As String = Talent.eCommerce.Utilities.GetPhotoPathByType("TEMP_PHYSICAL") & ImageName
                    Dim CropImage As Byte() = Crop(tempImagePath, w__1, h__2, x__3, y__4)
                    Using ms As New MemoryStream(CropImage, 0, CropImage.Length)
                        ms.Write(CropImage, 0, CropImage.Length)
                        Using CroppedImage As SD.Image = SD.Image.FromStream(ms, True)
                            'If isValidHeightWidthPhoto(CroppedImage, True) Then
                            If Talent.eCommerce.Utilities.DeleteImageFromPath(GetFileNameForUser(), "PERM_PHYSICAL") Then
                                Dim SaveTo As String = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_PHYSICAL") & "temp_" & ImageName
                                CroppedImage.Save(SaveTo, CroppedImage.RawFormat)
                                ResizeCropped(SaveTo, ImageName)
                                UpdateBackendFlag()
                                chkTermsAndCond.Enabled = True
                                chkTermsAndCond.Checked = False
                            Else
                                ShowError("ErrorMessFileSave", "")
                            End If
                            'Else
                            '    pnlUpload.Visible = True
                            '    pnlUploadButton.Visible = True
                            'End If
                            pnlCrop.Visible = False
                            pnlCropButton.Visible = False
                        End Using
                    End Using
                Catch ex As Exception
                    pnlCrop.Visible = False
                    pnlCropButton.Visible = False
                    pnlUpload.Visible = True
                    pnlUploadButton.Visible = True
                    ShowError("ErrorMessCropImage", "")
                End Try
                If _errMessage.Length = 0 Then
                    Response.Redirect("ProfilePhoto.aspx")
                End If
            Else
                ShowError("ErrorMessFileSave", "")
            End If
        Else
            ShowError("ErrorMessTandCs", "")
            pnlCrop.Visible = True
            pnlCropButton.Visible = True
            pnlUpload.Visible = False
            pnlUploadButton.Visible = False
        End If
    End Sub

#End Region
   
#Region "Private Methods"

    Private Function Crop(ByVal Img As String, ByVal Width As Integer, ByVal Height As Integer, ByVal X As Integer, ByVal Y As Integer) As Byte()
        Try
            Using OriginalImage As SD.Image = SD.Image.FromFile(Img)
                Using bmp As New SD.Bitmap(Width, Height)
                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution)
                    Using Graphic As SD.Graphics = SD.Graphics.FromImage(bmp)
                        Graphic.SmoothingMode = SmoothingMode.AntiAlias
                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic
                        Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality
                        Graphic.DrawImage(OriginalImage, New SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, _
                         SD.GraphicsUnit.Pixel)
                        Dim ms As New MemoryStream()
                        bmp.Save(ms, OriginalImage.RawFormat)
                        Return ms.GetBuffer()
                    End Using
                End Using
            End Using
        Catch Ex As Exception
            Throw
        End Try
    End Function

    Private Sub ResizeCropped(ByVal imgPath As String, ByVal imageName As String)
        Try
            Using OriginalImage As SD.Image = SD.Image.FromFile(imgPath)
                Dim dummyCallBack As New System.Drawing.Image.GetThumbnailImageAbort(AddressOf ThumbnailCallback)
                Dim thumbNailImg As System.Drawing.Image = OriginalImage.GetThumbnailImage(_imgWidthMin, _imgHeighttMin, dummyCallBack, IntPtr.Zero)
                Dim SaveTo As String = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_PHYSICAL") & imageName
                thumbNailImg.Save(SaveTo, OriginalImage.RawFormat)
                thumbNailImg.Dispose()
            End Using
        Catch Ex As Exception
            Throw
        End Try
    End Sub

    Private Sub ResizeImageForScreen(ByVal OriginalFile As String)
        If _isResizeBeforeCrop Then
            Dim resizeValue As Integer = 0
            Dim originalImage As System.Drawing.Image = System.Drawing.Image.FromFile(OriginalFile)
            Dim ResizedImage As System.Drawing.Image = Nothing
            ' Prevent using images internal thumbnail
            originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone)
            originalImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone)

            'Width based resize
            If originalImage.Width > _imgScreenWidth Then
                resizeValue = _imgScreenWidth
                Dim NewHeight As Integer = originalImage.Height * resizeValue \ originalImage.Width
                ResizedImage = originalImage.GetThumbnailImage(resizeValue, NewHeight, Nothing, IntPtr.Zero)
                originalImage.Dispose()
                ResizedImage.Save(OriginalFile)
            ElseIf originalImage.Height > _imgScreenHeight Then
                resizeValue = _imgScreenHeight
                Dim NewWidth As Integer = originalImage.Width * resizeValue \ originalImage.Height
                ResizedImage = originalImage.GetThumbnailImage(NewWidth, resizeValue, Nothing, IntPtr.Zero)
                originalImage.Dispose()
                ResizedImage.Save(OriginalFile)
            End If

            'is resized image has valid height and width if not calls this method again
            If (ResizedImage.Width > _imgScreenWidth) OrElse (ResizedImage.Height > _imgScreenHeight) Then
                ResizedImage.Dispose()
                ResizeImageForScreen(OriginalFile)
            Else
                ResizedImage.Dispose()
            End If
        End If
    End Sub

    Public Function ThumbnailCallback() As Boolean
        Return False
    End Function

    Private Function SaveReturnFilePath(ByVal fileNameForUser As String) As String
        Dim tempSavePath As String = ModuleDefaults.ImageUploadTempPath
        Dim fileSavedPhysicalPath As String = String.Empty
        If (Not String.IsNullOrWhiteSpace(tempSavePath)) Then
            Try
                If Talent.eCommerce.Utilities.DeleteImageFromPath(GetFileNameForUser(), "TEMP_PHYSICAL") Then
                    tempSavePath = Talent.eCommerce.Utilities.GetPhotoPathByType("TEMP_PHYSICAL")
                    Upload.PostedFile.SaveAs(tempSavePath & fileNameForUser & _fileExtension)
                    fileSavedPhysicalPath = tempSavePath
                Else
                    fileSavedPhysicalPath = String.Empty
                    ShowError("ErrorMessFileSave", "")
                End If
            Catch ex As Exception
                fileSavedPhysicalPath = String.Empty
                ShowError("ErrorMessFileSave", "")
            End Try
        End If
        Return fileSavedPhysicalPath
    End Function

    Private Function GetFileNameForUser() As String
        Dim userNumber As String = String.Empty
        If Me.Page.User.Identity.IsAuthenticated Then
            If Profile.User.Details.LoginID IsNot Nothing AndAlso Profile.User.Details.LoginID.Length > 0 Then
                userNumber = Profile.User.Details.LoginID
            Else
                Response.Redirect("~/pagespublic/login/login.aspx")
            End If
            userNumber = userNumber.PadLeft(12, "0")
        Else
            Response.Redirect("~/pagespublic/login/login.aspx")
        End If
        Return userNumber
    End Function

    Private Function isValidHeightWidthPhoto(ByVal profileImage As System.Drawing.Image, ByVal isCropped As Boolean) As Boolean
        Dim isValid As Boolean = False
        If profileImage.Height <= _imgHeighttMax AndAlso profileImage.Height >= _imgHeighttMin _
                            AndAlso profileImage.Width <= _imgWidthMax AndAlso profileImage.Width >= _imgWidthMin Then
            If isCropped Then
                'image has to match the minimum height and width after cropping to prevent javascript hacking
                If profileImage.Height = _imgHeighttMin AndAlso profileImage.Width = _imgWidthMin Then
                    isValid = True
                Else
                    ShowError("ErrorMessImageSize", "")
                End If
            Else
                isValid = True
            End If
        Else
            ShowError("ErrorMessImageSize", "")
        End If
        Return isValid
    End Function

    Private Sub AssignConfigurations()
        lblUpload.Text = _ucr.Content("LabelUpload", _languageCode, True)
        lblProfilePhoto.Text = _ucr.Content("LabelProfilePhoto", _languageCode, True)
        lblUploadInformation.Text = _ucr.Content("LabelUploadDetails", _languageCode, True)
        btnUpload.Text = _ucr.Content("ButtonUploadText", _languageCode, True)
        btnCrop.Text = _ucr.Content("ButtonCropText", _languageCode, True)
        chkTermsAndCond.Text = _ucr.Content("TandCsText", _languageCode, True)
        _imgHeighttMin = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageHeightMinimum"))
        _imgHeighttMax = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageHeightMaximum"))
        _imgWidthMin = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageWidthMinimum"))
        _imgWidthMax = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageWidthMaximum"))
        _imgFileSize = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageFileSizeMaximum"))
        _imgScreenWidth = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageScreenWidth"))
        _imgScreenHeight = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ImageScreenHeight"))
        cvTermsAndCond.ErrorMessage = _ucr.Content("ChkTermsAndCondErrMessCV", _languageCode, True)
        Dim scriptText As String = _ucr.Content("JCropScript", _languageCode, True)
        If Not String.IsNullOrWhiteSpace(scriptText) Then
            scriptText = scriptText.Replace("<<<ACCOUNTPHOTO_X_CLIENTID>>>", X.ClientID)
            scriptText = scriptText.Replace("<<<ACCOUNTPHOTO_Y_CLIENTID>>>", Y.ClientID)
            scriptText = scriptText.Replace("<<<ACCOUNTPHOTO_W_CLIENTID>>>", W.ClientID)
            scriptText = scriptText.Replace("<<<ACCOUNTPHOTO_H_CLIENTID>>>", H.ClientID)
            scriptText = scriptText.Replace("<<<SELECT_COORDINATES_VALUES>>>", "0,0," & _imgWidthMin & "," & _imgHeighttMin & "")
            scriptText = scriptText.Replace("<<<CROPIMAGE_CLIENTID>>>", imgCrop.ClientID)
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "JCorpScript", scriptText, False)
        End If
    End Sub

    Private Sub LoadProfilePhoto()
        If Session("WorkingImage") IsNot Nothing Then
            pnlProfilePhoto.Visible = False
            imgProfilePhoto.Visible = False
            pnlUpload.Visible = False
            pnlUploadButton.Visible = False
            pnlCrop.Visible = True
            pnlCropButton.Visible = True
            imgCrop.ImageUrl = Talent.eCommerce.Utilities.GetPhotoPathByType("TEMP_VIRTUAL") & Session("WorkingImage").ToString() & "?" & Now.Millisecond.ToString() 'to make sure always get the latest one
            chkTermsAndCond.Checked = True
            chkTermsAndCond.Enabled = False
            cvTermsAndCond.Enabled = False
            cvTermsAndCond.Visible = False
        Else
            Dim photoPath As String = ModuleDefaults.ProfilePhotoPermanentPath
            If photoPath.Length > 0 Then
                Dim imgName As String = Talent.eCommerce.Utilities.GetPhotoNameWithExt(GetFileNameForUser)
                If imgName.Length > 0 Then
                    imgProfilePhoto.ImageUrl = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_VIRTUAL") & imgName & "?" & Now.Millisecond.ToString() 'to make sure always get the latest one
                Else
                    imgProfilePhoto.ImageUrl = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_VIRTUAL") & _ucr.Attribute("NoProfilePhotoImage")
                End If
                pnlProfilePhoto.Visible = True
                imgProfilePhoto.Visible = True
            End If
            Dim jsContent As String = "<script type=""text/javascript""> function isTermsAndCondAccepted(oSrc, args){ "
            jsContent = jsContent & " var chkTermsAndCond = document.getElementById('" & chkTermsAndCond.ClientID & "');"
            jsContent = jsContent & " if (args.Value!="""") { if (chkTermsAndCond.checked == true) {"
            jsContent = jsContent & " args.IsValid = true; } "
            jsContent = jsContent & " else { "
            jsContent = jsContent & " args.IsValid = false; "
            jsContent = jsContent & " } } } </script> "
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "TAndCsValidationJS", jsContent)
        End If
    End Sub

    Private Sub MoveFileToPermanentPath(ByVal tempFilePhysicalPath As String, ByVal fileNameForUser As String)
        Dim permPhotoPhysicalPath As String = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_PHYSICAL")
        If permPhotoPhysicalPath.Length > 0 Then
            If Talent.eCommerce.Utilities.DeleteImageFromPath(GetFileNameForUser(), "PERM_PHYSICAL") Then
                File.Move(tempFilePhysicalPath & fileNameForUser & _fileExtension, permPhotoPhysicalPath & fileNameForUser & _fileExtension)
                If File.Exists(permPhotoPhysicalPath & fileNameForUser & _fileExtension) Then
                    UpdateBackendFlag()
                Else
                    ShowError("ErrorMessFileSave", "")
                End If
            Else
                ShowError("ErrorMessFileSave", "")
            End If
        End If
    End Sub

    Private Sub ValidateImageAssignFlags()
        Dim isValidImage As Boolean = False
        Dim isValidHeightWidth As Boolean = False
        Dim stFileContent As System.IO.Stream = Nothing
        If Upload.HasFile Then
            _fileExtension = Path.GetExtension(Upload.FileName.ToString()).ToLower()
            If Upload.PostedFile.ContentLength <= _imgFileSize Then
                stFileContent = Upload.FileContent
                If Talent.eCommerce.Utilities.IsValidImageFile(stFileContent, _fileExtension) Then
                    isValidImage = True
                    Dim profileImage As System.Drawing.Image = Nothing
                    Try
                        profileImage = System.Drawing.Image.FromStream(Upload.FileContent)
                        If isValidHeightWidthPhoto(profileImage, False) Then
                            isValidHeightWidth = True
                        End If
                        If isValidHeightWidth Then
                            If profileImage.Height > _imgHeighttMin OrElse profileImage.Width > _imgWidthMin Then
                                _isCropRequired = True
                            End If
                            If (profileImage.Width > _imgScreenWidth) OrElse (profileImage.Height > _imgScreenHeight) Then
                                _isResizeBeforeCrop = True
                            End If
                        End If
                    Catch generatedExceptionName As OutOfMemoryException
                        isValidHeightWidth = False
                    Finally
                        profileImage = Nothing
                    End Try
                Else
                    ShowError("ErrorMessImageFile", "")
                End If
            Else
                ShowError("ErrorMessFileSize", "")
            End If
        Else
            ShowError("ErrorMessNoFile", "")
        End If
        If isValidImage AndAlso isValidHeightWidth Then
            _isValidImageFile = True
        End If
    End Sub

    Private Sub UpdateBackendFlag()
        Dim deCustomerDetailsV11 As New DECustomerV11
        Dim deCustomerDetails As New Generic.List(Of DECustomer)
        deCustomerDetails.Add(New DECustomer)
        deCustomerDetails(0).CustomerNumber = Profile.User.Details.LoginID
        deCustomerDetails(0).SingleFieldMode = Talent.Common.SingleModeFieldsEnum.PHOTO
        deCustomerDetailsV11.DECustomersV1 = deCustomerDetails
        Dim err As New ErrorObj
        Dim talentCust As New TalentCustomer
        With talentCust
            .DeV11 = deCustomerDetailsV11
            With .Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Partner = TalentCache.GetPartner(Profile)
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            End With
            err = .UpdateCustomerDetailsSingleMode()
        End With

        If (Not err.HasError) AndAlso talentCust.ResultDataSet IsNot Nothing Then
            If talentCust.ResultDataSet.Tables.Count > 0 AndAlso talentCust.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Session("BackendError") = talentCust.ResultDataSet.Tables(0).Rows(0)("ReturnCode").ToString()
                pnlUpload.Visible = True
                pnlUploadButton.Visible = True
            End If
        Else
            Session("BackendError") = ""
            pnlUpload.Visible = True
            pnlUploadButton.Visible = True
        End If
        deCustomerDetailsV11 = Nothing
        deCustomerDetails = Nothing
        talentCust = Nothing
        DeleteTempFiles()
    End Sub

    Private Sub DeleteTempFiles()
        Dim ImageName As String = GetFileNameForUser()
        'Delete temp image created from crop before resize
        Talent.eCommerce.Utilities.DeleteImageFromPath("temp_" & ImageName, "PERM_PHYSICAL")
        'Delete uploaded image before cropped from temporary folder
        Talent.eCommerce.Utilities.DeleteImageFromPath(ImageName, "TEMP_PHYSICAL")
    End Sub

    Private Sub ShowError(ByVal errMessageCode As String, ByVal backendErrorCode As String)
        _errMessage = _ucr.Content(errMessageCode, _languageCode, True)
        If backendErrorCode.Length > 0 Then
            Dim errMsg As Talent.Common.TalentErrorMessages
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            _errMessage = _errMessage & " - " & errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                    Talent.eCommerce.Utilities.GetCurrentPageName, _
                                    backendErrorCode).ERROR_MESSAGE
        End If
        Dim customErrorMessage As New CustomValidator
        pHValidationSummary.Visible = True
        customErrorMessage.ValidationGroup = ValidationSummary1.ValidationGroup
        customErrorMessage.IsValid = False
        customErrorMessage.ErrorMessage = _errMessage
        Page.Validators.Add(customErrorMessage)
    End Sub

#End Region

End Class
