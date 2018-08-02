Imports Talent.Common
Imports TalentEcommerce = Talent.eCommerce
Partial Class Redirect_TradePlaceGetTimestamp
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim username As String = String.Empty
        Dim signatureReceived As String = String.Empty
        If Request.QueryString("userid") IsNot Nothing _
            And Request.QueryString("signature") IsNot Nothing Then
            username = Request.QueryString("userid")
            signatureReceived = Request.QueryString("signature").Replace(" ", "+")

            Dim signatureCreated As String = String.Empty
            Dim ecomModuleDefaults As TalentEcommerce.ECommerceModuleDefaults = New TalentEcommerce.ECommerceModuleDefaults
            Dim ecomModuleDefaultValues As TalentEcommerce.ECommerceModuleDefaults.DefaultValues = ecomModuleDefaults.GetDefaults
            Dim keyToEncrypt As String = ecomModuleDefaultValues.TradePlaceEncryptionKey

            'ToDo: Encrypt username based on key
            'Get the key from ecommercemoduledefaults
            signatureCreated = Utilities.SHA1TripleDESEncode(username, keyToEncrypt)

            If signatureReceived = signatureCreated Then
                Dim membershipProvider As New TalentMembershipProvider
                'Check user is exists or not
                If membershipProvider.ValidateUserNoPassword(username) Then
                    'Getting the next value for TradingPortalTicket from tbl_ecommerce_module_defaults_bu table     
                    Dim tradingPortalTicketNumber As Long = Utilities.GetNextTradingPortalTicket(TalentCache.GetBusinessUnit(), _
                                                                                                    TalentCache.GetDefaultPartner(), _
                                                                                                    ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
                    If tradingPortalTicketNumber > 0 Then
                        Dim isAgent As Boolean = False
                        'Getting the DESettings object with default values
                        Dim DESettingsValues As DESettings = TalentEcommerce.Utilities.GetSettingsObject
                        DESettingsValues.LoginId = username
                        'TalentNoise initialised with DESettings
                        Dim talentNoiseRecorder As New TalentNoise(DESettingsValues, Session.SessionID, _
                                                                    Now, _
                                                                    Now.AddMinutes(-ecomModuleDefaultValues.NOISE_THRESHOLD_MINUTES), _
                                                                    ecomModuleDefaultValues.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES, _
                                                                    isAgent, _
                                                                     TblActiveNoiseSessions_Usage.TRADEPLACE)
                        'Checks and add the given session in tbl_active_noise_session
                        If ecomModuleDefaultValues.NOISE_IN_USE Then
                            talentNoiseRecorder.AddOrUpdateNoiseSession(ecomModuleDefaultValues.NOISE_MAX_CONCURRENT_USERS)
                        Else
                            talentNoiseRecorder.AddOrUpdateNoiseSession()
                        End If
                        If talentNoiseRecorder.SuccessfullCall AndAlso talentNoiseRecorder.RowsAffected > 0 Then
                            Response.Write("ts=" & Session.SessionID)
                        End If
                    End If
                End If
            End If
        End If
    End Sub
End Class
