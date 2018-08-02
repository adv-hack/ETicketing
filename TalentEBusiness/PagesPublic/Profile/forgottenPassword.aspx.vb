Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Profile
Imports Talent.Common
Partial Class PagesPublic_forgottenPassword
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _pageMode As GlobalConstants.CRUDOperationMode
    Private _viewModel As ForgottenPasswordViewModel
    Private _companyNumber As String = String.Empty

#End Region

#Region "Protected Page Events"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim inputModel As ForgottenPasswordInputModel = setInputModel()
        processController(inputModel)
        createView()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Setup the input model before passing it to the builder class
    ''' </summary>
    ''' <returns>The input model to be used</returns>
    ''' <remarks></remarks>
    Private Function setInputModel() As ForgottenPasswordInputModel
        Dim inputModel As ForgottenPasswordInputModel = New ForgottenPasswordInputModel()
        'Make sure user isn't signed in.
        If Profile.IsAnonymous Then
            inputModel.UserSignedIn = False
            inputModel.DoTokenHashing = ModuleDefaults.UseEncryptedPassword
            If IsPostBack Then
                'Check to see if the Forgotten password button has been clicked.
                If Not String.IsNullOrWhiteSpace(Request.Params(btnForgottenPassword.UniqueID)) Then
                    inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_RESPONSE
                    'Assign the customer number/email address from the text box
                    If txtForgottenPasswordTextBox.Text.Contains("@") Then
                        inputModel.EmailAddress = txtForgottenPasswordTextBox.Text
                        inputModel.CustomerNumber = String.Empty
                    Else
                        inputModel.CustomerNumber = txtForgottenPasswordTextBox.Text
                        inputModel.EmailAddress = String.Empty
                    End If
                Else
                    inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL
                End If
            Else
                inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL
            End If
        Else
            inputModel.UserSignedIn = True
            inputModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN
        End If
        inputModel.HashedToken = String.Empty
        inputModel.Token = String.Empty
        Return inputModel
    End Function

    ''' <summary>
    ''' Process the controller, pass the input model into the builder
    ''' </summary>
    ''' <param name="inputModel">The forgotten password input model</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModel As ForgottenPasswordInputModel)
        Dim forgottenPasswordBuilder As New PasswordModelBuilders()
        _viewModel = New ForgottenPasswordViewModel(True)
        _viewModel = forgottenPasswordBuilder.ForgottenPassword(inputModel)
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
    ''' Error message display. Show a generic error when the model has an error but doesn't have a message
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processErrors()
        blErrorMessages.Items.Clear()
        If _viewModel.Error IsNot Nothing AndAlso _viewModel.Error.HasError Then
            If _viewModel.Error.Count > 0 AndAlso _viewModel.Error.ErrorMessage.Length > 0 Then
                blErrorMessages.Items.Add(_viewModel.Error.ErrorMessage)
            ElseIf _viewModel.Error.ReturnCode = "DP" Then
                blErrorMessages.Items.Add(_viewModel.GetPageText("DuplicateEmailError"))
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
        lblForgottenPasswordLabel.Text = _viewModel.GetPageText("ForgottenPasswordLabel")
        btnForgottenPassword.Text = _viewModel.GetPageText("ForgottenPasswordButton")
        rfvForgottenPassword.ErrorMessage = _viewModel.GetPageText("NoFieldEntered")
        rgxForgottenPassword.ErrorMessage = _viewModel.GetPageText("InvalidCharacters")
        rgxForgottenPassword.ValidationExpression = _viewModel.GetPageAttribute("ForgottenPasswordRegex")
        ltlForgottenPasswordSuccessTitleLabel.Text = _viewModel.GetPageText("SuccessTitle")
        ltlForgottenPasswordSuccess.Text = _viewModel.GetPageText("SuccessMessage")
        ltlUserSignedInTitleLabel.Text = _viewModel.GetPageText("SignedInTitle")
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
