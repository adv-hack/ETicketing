Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Competition Questions
'
'       Date                        Sept 2009
'
'       Author                      Craig Mcloughlin 
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base   
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_competitionQuestions
    Inherits TalentBase01

    Private wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private moduleName As String = "CompetitionQuestions"
    Public errMsg As Talent.Common.TalentErrorMessages

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "competitionQuestions.aspx"
        End With

        Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
        def = defaults.GetDefaults

        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                TalentCache.GetBusinessUnitGroup, _
                                                TalentCache.GetPartner(Profile), _
                                                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        PageHeaderTextLabel.Text = GetText("PageHeaderText")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Visible Then
            Try
                If Not Page.IsPostBack Then
                    displayQuestion()
                End If
            Catch ex As Exception
            End Try

        End If
    End Sub

    Protected Sub displayQuestion()
        '
        ' Retrieve Competition Question
        QuestionLabel.Text = GetText("CompetitionQuestionText")

        '
        ' Populate the Multiple choice answers radiobox selection 
        CompetitionAnswers.Items.Clear()

        Dim answerText As String = "CompetitionAnswerText"
        Dim count As Integer = 0

        Do While count <= 10
            count += 1
            Dim answer As String = GetText(answerText & count.ToString).Trim
            If (answer <> String.Empty And answer <> Nothing) Then
                CompetitionAnswers.Items.Add(answer)
            End If
        Loop
        CompetitionAnswers.ClearSelection()

    End Sub

    Protected Function GetText(ByVal PValue As String) As String
        Dim str As String = wfr.Content(PValue, _languageCode, True)
        Return str
    End Function


    Protected Sub showError(ByVal errCode As String)
        Dim eli As ListItem
        Dim myError As String = CStr(Session("TalentErrorCode"))
        eli = New ListItem(errMsg.GetErrorMessage(moduleName, _
                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                            errCode).ERROR_MESSAGE)

        If Not errorlist.Items.Contains(eli) Then
            errorlist.Items.Add(eli)
        End If
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SubmitButton.Click
        errorlist.Items.Clear()

        Try

            If CompetitionAnswers.SelectedItem Is Nothing Or CompetitionAnswers.SelectedItem Is String.Empty Then
                showError("NS")
            Else
                '
                ' If an answer has been selected, then create an email for the answer
                '
                Dim message As String
                message = GetText("CompetitionEmailHeaderText") & vbCrLf & vbCrLf & _
                          GetText("AccountDetailsText") & vbCrLf & _
                          CType(HttpContext.Current.Profile, TalentProfile).User.Details.Forename & " " & _
                          CType(HttpContext.Current.Profile, TalentProfile).User.Details.Surname & vbCrLf & _
                          GetText("LoginIDText") & CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID & vbCrLf & _
                          GetText("EmailText") & CType(HttpContext.Current.Profile, TalentProfile).User.Details.Email & vbCrLf & vbCrLf & _
                          GetText("CompetitionQuestionText") & vbCrLf & _
                          CompetitionAnswers.SelectedValue.ToString.Trim

                Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
                Talent.Common.Utilities.SMTPPortNumber = Utilities.GetSMTPPortNumber
                Talent.Common.Utilities.Email_Send(wfr.Attribute("CompetitionEmailFrom"), _
                                                   wfr.Attribute("CompetitionEmailTo"), _
                                                   GetText("CompetitionEmailSubject"), _
                                                   message)

                '
                ' Once email has been sent, forward onto confirmation Page
                '
                Response.Redirect("~/PagesLogin/Competition/CompetitionQuestionsConfirmation.aspx")

            End If

        Catch ex As Exception
            showError("XX")
        End Try
    End Sub

End Class
