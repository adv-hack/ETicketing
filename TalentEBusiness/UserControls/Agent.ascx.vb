Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data

Partial Class UserControls_Agent
    Inherits ControlBase

    Public Display As Boolean = False
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Dim myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display Then
            initControls()
            loadValues()
        Else
            Me.Visible = False
        End If
    End Sub

    Private Sub initControls()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "RegistrationForm.ascx"

            If myDefs.ShowUserIDFields Then
                If myDefs.ShowPassportField Then passportLabel.Text = .Content("passportLabel", _languageCode, True)
                If myDefs.ShowPinField Then pinLabel.Text = .Content("pinLabel", _languageCode, True)
                If myDefs.ShowGreenCardField Then greenCardLabel.Text = .Content("greenCardLabel", _languageCode, True)
                If myDefs.ShowUserID4Field Then userID4Label.Text = .Content("userID4Label", _languageCode, True)
                If myDefs.ShowUserID5Field Then userID5Label.Text = .Content("userID5Label", _languageCode, True)
                If myDefs.ShowUserID6Field Then userID6Label.Text = .Content("userID6Label", _languageCode, True)
                If myDefs.ShowUserID7Field Then userID7Label.Text = .Content("userID7Label", _languageCode, True)
                If myDefs.ShowUserID8Field Then userID8Label.Text = .Content("userID8Label", _languageCode, True)
                If myDefs.ShowUserID9Field Then userID9Label.Text = .Content("userID9Label", _languageCode, True)
            End If
        End With
    End Sub

    Private Sub loadValues()
        If myDefs.ShowUserIDFields Then
            If Session("Passport") IsNot Nothing AndAlso _
                Not Session("Passport").Equals(String.Empty) AndAlso _
                myDefs.ShowPassportField Then
                passport.Text = CType(Session("Passport"), String)
            Else
                passportRow.Visible = False
            End If

            If Session("PIN") IsNot Nothing AndAlso _
                Not Session("PIN").Equals(String.Empty) AndAlso _
                myDefs.ShowPinField Then
                pin.Text = CType(Session("PIN"), String)
            Else
                pinRow.Visible = False
            End If

            If Session("Greencard") IsNot Nothing AndAlso _
                Not Session("Greencard").Equals(String.Empty) AndAlso _
                myDefs.ShowGreenCardField Then
                greenCard.Text = CType(Session("Greencard"), String)
            Else
                greenCardRow.Visible = False
            End If

            If Session("UserID4") IsNot Nothing AndAlso _
                Not Session("UserID4").Equals(String.Empty) AndAlso _
                myDefs.ShowUserID4Field Then
                userID4.Text = CType(Session("UserID4"), String)
            Else
                userID4Row.Visible = False
            End If

            If Session("UserID5") IsNot Nothing AndAlso _
                Not Session("UserID5").Equals(String.Empty) AndAlso _
                myDefs.ShowUserID5Field Then
                userID5.Text = CType(Session("UserID5"), String)
            Else
                userID5Row.Visible = False
            End If

            If Session("UserID6") IsNot Nothing AndAlso _
                Not Session("UserID6").Equals(String.Empty) AndAlso _
                myDefs.ShowUserID6Field Then
                userID6.Text = CType(Session("UserID6"), String)
            Else
                userID6Row.Visible = False
            End If

            If Session("UserID7") IsNot Nothing AndAlso _
                Not Session("UserID7").Equals(String.Empty) AndAlso _
                myDefs.ShowUserID7Field Then
                userID7.Text = CType(Session("UserID7"), String)
            Else
                userID7Row.Visible = False
            End If

            If Session("UserID8") IsNot Nothing AndAlso _
                Not Session("UserID8").Equals(String.Empty) AndAlso _
                myDefs.ShowUserID8Field Then
                userID8.Text = CType(Session("UserID8"), String)
            Else
                userID8Row.Visible = False
            End If

            If Session("UserID9") IsNot Nothing AndAlso _
                Not Session("UserID9").Equals(String.Empty) AndAlso _
                myDefs.ShowUserID9Field Then
                userID9.Text = CType(Session("UserID9"), String)
            Else
                userID9Row.Visible = False
            End If
        Else
            Me.Visible = False
        End If
    End Sub
End Class
