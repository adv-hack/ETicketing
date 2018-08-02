Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports Talent.Common
Partial Class UserControls_AccountWindow
    Inherits ControlBase

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _sbAccountWindowListText As StringBuilder

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AccountWindow.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Profile.IsAnonymous OrElse Profile.User Is Nothing OrElse Profile.User.Details Is Nothing Then
            Response.Redirect("~/PagesPublic/Login/Login.aspx")
        Else
            DisplayMessage()
            LoadMenus()
        End If
    End Sub

    Private Sub DisplayMessage()
        If Not String.IsNullOrWhiteSpace(Session("UpdateDetailsConfirmMessageDisplay")) Then
            plhMessage.Visible = True
            ltlMessage.Text = _ucr.Content("UpdateDetailsConfirmMessage", _languageCode, True)
            Session.Remove("UpdateDetailsConfirmMessageDisplay")
        End If
    End Sub

    Private Sub LoadMenus()
        _sbAccountWindowListText = New StringBuilder
        MenuHeaderTag()
        Dim sequenceNotFound As Boolean = True
        'sequence is controlled by MenuSequence UPD;FAF;FAV;LOY;BBK;SCD;ORH;PPH
        Dim sequenceString As String = CheckForDBNull_String(_ucr.Attribute("MenuDisplaySequence")).Trim()
        If sequenceString.Length > 0 Then
            Dim sequenceMenuItems() As String = sequenceString.Split(";")
            If sequenceMenuItems.Length > 0 Then
                For seqMenuIndex As Integer = 0 To sequenceMenuItems.Length - 1
                    Select Case sequenceMenuItems(seqMenuIndex)
                        Case "UPD"
                            UpdateDetails()
                            sequenceNotFound = False
                        Case "FAF"
                            FriendsAndFamily()
                            sequenceNotFound = False
                        Case "FAV"
                            FavouriteSeat()
                            sequenceNotFound = False
                        Case "LOY"
                            LoyaltyPoints()
                            sequenceNotFound = False
                        Case "BBK"
                            Buyback()
                            sequenceNotFound = False
                        Case "SCD"
                            TotalSavedCards()
                            sequenceNotFound = False
                        Case "ORH"
                            OrderHistory()
                            sequenceNotFound = False
                        Case "PPH"
                            ProfilePhoto()
                            sequenceNotFound = False
                        Case "GLT"
                            GenericLinksText()
                            sequenceNotFound = False
                    End Select
                Next
            End If
        End If
        If sequenceNotFound Then
            UpdateDetails()
            FriendsAndFamily()
            OrderHistory()
            ProfilePhoto()
            LoyaltyPoints()
            FavouriteSeat()
            Buyback()
            TotalSavedCards()
            GenericLinksText()
        End If
        MenuFooterTag()
        ltlAccountWindowList.Text = _sbAccountWindowListText.ToString()
    End Sub

    Private Sub MenuHeaderTag()
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("MenuHeaderTag", _languageCode, True))
        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub

    Private Sub MenuFooterTag()
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("MenuFooterTag", _languageCode, True))
        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub

    Private Sub UpdateDetails()
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("UpdateDetailsText", _languageCode, True))
        tempString = Replacer(tempString, "<<<TITLE>>>", Profile.User.Details.Title)
        tempString = Replacer(tempString, "<<<FORENAME>>>", Profile.User.Details.Forename)
        tempString = Replacer(tempString, "<<<SURNAME>>>", Profile.User.Details.Surname)
        If Profile.User.Addresses.Count > 0 Then
            Dim defaultProfileAddress As New TalentProfileAddress
            defaultProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
            tempString = Replacer(tempString, "<<<POSTCODE>>>", defaultProfileAddress.Post_Code)
            defaultProfileAddress = Nothing
        Else
            tempString = Replacer(tempString, "<<<POSTCODE>>>", "")
        End If

        tempString = Replacer(tempString, "<<<EMAIL>>>", Profile.User.Details.Email)
        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub

    Private Sub FriendsAndFamily()
        If (ModuleDefaults.AddCustomerToMyFriendsAndFamily OrElse ModuleDefaults.AddMeToTheirMyFriendsAndFamily OrElse ModuleDefaults.RegisterCustomerForFriendsAndFamily) Then
            Dim err As New Talent.Common.ErrorObj
            Dim deCustV11 As New DECustomerV11
            Dim deCustV1 As New DECustomer
            Dim customer As New TalentCustomer
            deCustV11.DECustomersV1.Add(deCustV1)
            Dim totalFriendsAndFamily As Integer = 0
            With customer
                .DeV11 = deCustV11
                ' Set the settings data entity. 
                .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
                .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .Settings.Cacheing = True
                'Set the customer values
                deCustV1.CustomerNumber = Profile.User.Details.LoginID.ToString()
                deCustV1.Source = "W"
                .ResultDataSet = Nothing
                'Process
                err = .CustomerAssociations
            End With
            If (Not err.HasError) AndAlso customer.ResultDataSet IsNot Nothing Then
                If customer.ResultDataSet.Tables.Count > 1 Then
                    If String.IsNullOrWhiteSpace(customer.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                        totalFriendsAndFamily = customer.ResultDataSet.Tables("FriendsAndFamily").Rows.Count
                    End If
                End If
            End If
            Dim tempString As String = CheckForDBNull_String(_ucr.Content("FriendsAndFamilyText", _languageCode, True))
            tempString = Replacer(tempString, "<<<TOTALFRIENDSANDFAMILY>>>", totalFriendsAndFamily)
            _sbAccountWindowListText.Append(tempString)
            tempString = Nothing
        End If
    End Sub

    Private Sub LoyaltyPoints()
        If (ModuleDefaults.Loyalty_Points_In_Use) Then
            Dim tempString As String = CheckForDBNull_String(_ucr.Content("LoyaltyPointsText", _languageCode, True))
            tempString = Replacer(tempString, "<<<LOYALTYPOINTS>>>", Profile.User.Details.Ticketing_Loyalty_Points.ToString())
            _sbAccountWindowListText.Append(tempString)
            tempString = Nothing
        End If
    End Sub

    Private Sub FavouriteSeat()
        If (ModuleDefaults.FavouriteSeatFunction) Then
            Dim tempString As String = String.Empty
            If String.IsNullOrWhiteSpace(Profile.User.Details.Favourite_Seat) Then
                tempString = CheckForDBNull_String(_ucr.Content("FavouriteSeatNoText", _languageCode, True))
            Else
                tempString = CheckForDBNull_String(_ucr.Content("FavouriteSeatText", _languageCode, True))
                tempString = Replacer(tempString, "<<<FAVOURITESEAT>>>", Profile.User.Details.Favourite_Seat)
            End If
            _sbAccountWindowListText.Append(tempString)
            tempString = Nothing
        End If
    End Sub

    Private Sub TotalSavedCards()
        If (ModuleDefaults.UseSaveMyCard) Then
            Dim err As New ErrorObj
            Dim payment As New TalentPayment
            Dim settings As New DESettings
            Dim dePayment As New DEPayments
            With settings
                .FrontEndConnectionString = _ucr.FrontEndConnectionString
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .Cacheing = True
                .OriginatingSourceCode = "W"
            End With
            If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
            End If
            dePayment.SessionId = Profile.Basket.Basket_Header_ID
            dePayment.CustomerNumber = Profile.UserName
            dePayment.Source = "W"
            payment.De = dePayment
            payment.Settings = settings
            err = payment.RetrieveMySavedCards
            Dim totalSaved As Integer = 0
            If (Not err.HasError) AndAlso (payment.ResultDataSet IsNot Nothing) Then
                If payment.ResultDataSet.Tables.Count > 1 Then
                    If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            If payment.ResultDataSet.Tables("CardDetails") IsNot Nothing Then
                                totalSaved = payment.ResultDataSet.Tables("CardDetails").Rows.Count
                            End If
                        End If
                    Else
                        If payment.ResultDataSet.Tables("CardDetails") IsNot Nothing Then
                            totalSaved = payment.ResultDataSet.Tables("CardDetails").Rows.Count
                        End If
                    End If
                End If
            End If

            Dim tempString As String = CheckForDBNull_String(_ucr.Content("SaveMyCardText", _languageCode, True))
            tempString = Replacer(tempString, "<<<TOTALSAVEDCARDS>>>", totalSaved)
            _sbAccountWindowListText.Append(tempString)
            tempString = Nothing
        End If
    End Sub

    Private Sub Buyback()
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("BuybackText", _languageCode, True))
        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub

    Private Sub ProfilePhoto()

        ' Determine Image Url
        Dim imgUrl As String = String.Empty
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("ProfilePhotoText", _languageCode, True))
        If Session("WorkingImage") IsNot Nothing Then
            imgUrl = Talent.eCommerce.Utilities.GetPhotoPathByType("TEMP_VIRTUAL") & Session("WorkingImage").ToString() & "?" & Now.Millisecond.ToString() 'to make sure always get the latest one
        Else
            Dim photoPath As String = ModuleDefaults.ProfilePhotoPermanentPath
            If photoPath.Length > 0 Then
                Dim imgName As String = Talent.eCommerce.Utilities.GetPhotoNameWithExt(GetFileNameForUser)
                If imgName.Length > 0 Then
                    imgUrl = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_VIRTUAL") & imgName & "?" & Now.Millisecond.ToString() 'to make sure always get the latest one
                Else
                    imgUrl = Talent.eCommerce.Utilities.GetPhotoPathByType("PERM_VIRTUAL") & _ucr.Attribute("NoProfilePhotoImage")
                End If
                imgUrl = Replacer(imgUrl, "~", "../..")
            End If
        End If

        ' Construct the <img> tag 
        '
        ' Remember that following style is required for this to be rendered correctly: 
        '    .change-profile-picture{position:relative;}
        '    .change-profile-picture img{position:absolute;right:5px;top:5px;}
        '
        If imgUrl = String.Empty Then
            tempString = Replacer(tempString, "<<<PHOTOIMAGE>>>", "")
        Else
            Dim sb As New StringBuilder
            sb.Append("<img src=""")
            sb.Append(imgUrl)
            sb.Append(""" height=""48"" width=""48"" />")
            tempString = Replacer(tempString, "<<<PHOTOIMAGE>>>", sb.ToString)
        End If

        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub

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

    Private Sub OrderHistory()
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("OrderHistoryText", _languageCode, True))
        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub

    Private Function Replacer(ByVal mainString As String, ByVal oldString As String, ByVal newString As String) As String
        If String.IsNullOrWhiteSpace(newString) Then
            mainString = mainString.Replace(oldString, "")
        Else
            mainString = mainString.Replace(oldString, newString)
        End If
        Return mainString
    End Function

    Private Sub GenericLinksText()
        Dim tempString As String = CheckForDBNull_String(_ucr.Content("GenericLinksText", _languageCode, True))
        _sbAccountWindowListText.Append(tempString)
        tempString = Nothing
    End Sub
End Class
