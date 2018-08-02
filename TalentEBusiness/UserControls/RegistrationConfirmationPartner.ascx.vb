Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class UserControls_RegistrationConfirmationPartner
    Inherits ControlBase
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public ucr As New Talent.Common.UserControlResource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()
        Dim pagename As String = Talent.eCommerce.Utilities.GetCurrentPageName()
        Dim businessUnit As String = TalentCache.GetBusinessUnit

        If Not Page.IsPostBack Then
            If Not Page.IsPostBack Then
                With ucr
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PageCode = ProfileHelper.GetPageName
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "RegistrationConfirmationPartner.ascx"

                    MessageLabel.Text = .Content("MessageText", _languageCode, True)

                    'Replace the place holders with real time data
                    If Not Profile.IsAnonymous Then
                        Try
                            MessageLabel.Text = MessageLabel.Text.Replace("<<CustomerNumber>>", Profile.PartnerInfo.Details.Account_No_1.ToString.Trim)
                            MessageLabel.Text = MessageLabel.Text.Replace("<<FullName>>", Profile.User.Details.Full_Name.ToString.Trim)
                        Catch ex As Exception
                            Dim logging As New TalentLogging
                            logging.FrontEndConnectionString = ucr.FrontEndConnectionString
                            logging.ExceptionLog("PartnerRegistration", ex)
                        End Try
                    End If
                End With
            End If
        End If

    End Sub
End Class
