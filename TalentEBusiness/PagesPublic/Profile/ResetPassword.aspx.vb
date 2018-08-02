Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Profile
Imports Talent.Common

Partial Class PagesPublic_ResetPassword
    Inherits TalentBase01

#Region "Class level fields"

    Private _viewModel As ResetPasswordViewModel

#End Region

#Region "Protected Page Events"
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        blErrorMessages.Items.Clear()
        _viewModel = New ResetPasswordViewModel(True)
        Dim inputModel As ResetPasswordInputModel = setInputModel()
        processController(inputModel)
        createView()
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Setup the input model before passing it to the builder class
    ''' </summary>
    ''' <returns>The input model to be used</returns>
    ''' <remarks></remarks>
    Private Function setInputModel() As ResetPasswordInputModel
        Dim inputModel As ResetPasswordInputModel = New ResetPasswordInputModel()
        'Make sure user isn't signed in.
        If Profile.IsAnonymous Then
            If IsPostBack AndAlso Not String.IsNullOrWhiteSpace(Request.Params(btnResetPassword.UniqueID)) Then
                If Request.QueryString("t") IsNot Nothing AndAlso Request.QueryString("c") IsNot Nothing Then
                    inputModel.Token = Request.QueryString("t")
                    inputModel.CustomerNumber = Request.QueryString("c")
                    inputModel.DateNow = Date.Now
                    inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_RESPONSE
                    inputModel.NewPassword = txtResetPassword.Text
                Else
                    inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL
                    blErrorMessages.Items.Add(_viewModel.GetPageText("GenericError"))
                End If
            Else
                inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL
            End If
        Else
            inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN
        End If
        Return inputModel
    End Function

    ''' <summary>
    ''' Process the controller, pass the input model into the builder
    ''' </summary>
    ''' <param name="inputModel">The reset password input model</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModel As ResetPasswordInputModel)
        Dim resetPasswordBuilder As New PasswordModelBuilders()
        _viewModel = resetPasswordBuilder.ResetPassword(inputModel)
    End Sub

    ''' <summary>
    ''' Create the view from the model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        processErrors()
        populateText()
        renderView()
    End Sub

#End Region

#Region "Presentation"

    ''' <summary>
    ''' Error message display. If there is no user friendly message then display a Generic message. The actual error will be logged by the builder.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processErrors()
        If _viewModel.Error IsNot Nothing AndAlso _viewModel.Error.HasError Then
            If _viewModel.Error.ErrorMessage IsNot Nothing AndAlso _viewModel.Error.ErrorMessage.Length > 0 Then
                blErrorMessages.Items.Add(_viewModel.Error.ErrorMessage)
            Else
                blErrorMessages.Items.Add(_viewModel.GetPageText("UnspecifiedError"))
            End If
        End If
    End Sub

    ''' <summary>
    ''' Populate the text elements on the page from the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateText()
        lblResetPassword.Text = _viewModel.GetPageText("ResetPasswordLabel")
        lblResetPasswordConfirm.Text = _viewModel.GetPageText("PasswordConfirmLabel")
        btnResetPassword.Text = _viewModel.GetPageText("ResetPasswordButton")
        rfvResetPasswordConfirm.Text = _viewModel.GetPageText("NoFieldEntered")
        rgxResetPasswordConfirm.Text = _viewModel.GetPageText("RegexLabel")
        rgxResetPasswordConfirm.ValidationExpression = _viewModel.GetPageAttribute("ResetPasswordRegex")
        rfvResetPassword.Text = _viewModel.GetPageText("NoFieldEntered")
        rgxResetPassword.Text = _viewModel.GetPageText("RegexLabel")
        rgxResetPassword.ValidationExpression = _viewModel.GetPageAttribute("ResetPasswordRegex")
        cvNewPasswordCompare.Text = _viewModel.GetPageText("PasswordMismatch")
        ltlResetPasswordSuccessTitle.Text = _viewModel.GetPageText("SuccessTitle")
        ltlResetPasswordSuccess.Text = _viewModel.GetPageText("SuccessMessage")
        ltlUserSignedInTitle.Text = _viewModel.GetPageText("SignedInTitle")
        ltlUserSignedIn.Text = _viewModel.GetPageText("SignedInError")
    End Sub

    ''' <summary>
    ''' Populate the display on the page based on the mode currently being used
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub renderView()
        If _viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL Then
            plhInitialPage.Visible = True
            HTMLInclude1.Visible = True
            plhSuccessPage.Visible = False
            plhUserSignedIn.Visible = False
        ElseIf _viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_RESPONSE Then
            plhInitialPage.Visible = False
            plhSuccessPage.Visible = True
            HTMLInclude1.Visible = False
            plhUserSignedIn.Visible = False
        ElseIf _viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN Then
            plhInitialPage.Visible = False
            plhSuccessPage.Visible = False
            HTMLInclude1.Visible = False
            plhUserSignedIn.Visible = True
        End If
    End Sub

#End Region

End Class
