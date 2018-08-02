Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports Talent.eCommerce
Imports TEUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Smartcard_ActivateSmartcard
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = TCUtilities.GetDefaultLanguage
    Private _errMsg As TalentErrorMessages
    Private _activeCards As Integer

#End Region

#Region "Public Properties"

    Public AvailableFanIdLabel As String
    Public FanIdLastFourNumbersLabel As String
    Public FanIdIssueNumberLabel As String


#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ActivateSmartcard.aspx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _errMsg = New TalentErrorMessages(_languageCode, _wfrPage.BusinessUnit, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
        ltlErrorMessage.Text = String.Empty
        If Not Page.IsPostBack Then
            bindAvailableFanIds()
        End If
    End Sub

    Protected Sub btnUpdateFanId_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateFanId.Click

        Try
            Dim settings As DESettings = TEUtilities.GetSettingsObject()
            Dim smartcards As New TalentSmartcard
            Dim smartcardsDataEntity As New DESmartcard
            Dim err As ErrorObj

            ' Retrieve the new fan id information
            smartcardsDataEntity.Src = GlobalConstants.SOURCE
            smartcardsDataEntity.CustomerNumber = Profile.User.Details.LoginID
            For Each item As RepeaterItem In rptFanIdList.Items
                Dim rbtnFanId As RadioButton = CType(item.FindControl("rbtnFanId"), RadioButton)
                If rbtnFanId.Checked Then
                    smartcardsDataEntity.NewFanId = CType(item.FindControl("lblAvailableFanId"), Label).Text
                    smartcardsDataEntity.New4Digits = CType(item.FindControl("lblAvailable4digits"), Label).Text
                    smartcardsDataEntity.NewIssueNumber = CType(item.FindControl("lblAvailableIssueNumber"), Label).Text
                    Exit For
                End If
            Next

            ltlErrorMessage.Text = String.Empty
            If Not String.IsNullOrEmpty(smartcardsDataEntity.NewFanId) Then

                'Retrieve the old fan id information and perform the required action
                smartcardsDataEntity.OldFanId = ltlCurrentFanIdValue.Text
                smartcardsDataEntity.Old4Digits = ltlCurrent4DigitsValue.Text
                smartcardsDataEntity.OldIssueNumber = ltlCurrentIssueValue.Text
                smartcardsDataEntity.CardNumber = ltlCurrentSmartcardValue.Text
                smartcards.Settings = settings
                smartcards.DE = smartcardsDataEntity
                err = smartcards.UpdateCurrentFanId()

                'We always need to clear the cache at this stage.  The request process happens in two stages so
                'we can find ourselves in situation were the first request works and then the second fails.  We 
                'need to update the data either way
                smartcards.ClearAvailableFanIdListCache()
                Dim successfulBind As Boolean = bindAvailableFanIds()

                'Output any errors
                plhErrorMessage.Visible = True
                If Not err.HasError AndAlso smartcards.ResultDataSet.Tables.Count = 1 Then
                    If smartcards.ResultDataSet.Tables("StatusResults").Rows.Count > 0 AndAlso smartcards.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                        Dim returnCode As String = smartcards.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode").ToString()
                        ltlErrorMessage.Text = _errMsg.GetErrorMessage(returnCode).ERROR_MESSAGE
                    Else
                        If successfulBind Then
                            ltlErrorMessage.Text = _errMsg.GetErrorMessage(GlobalConstants.FANIDUPDATED).ERROR_MESSAGE
                        Else
                            ltlErrorMessage.Text = _errMsg.GetErrorMessage(GlobalConstants.FANIDNOTUPDATED).ERROR_MESSAGE
                        End If
                    End If
                Else
                    Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(GlobalConstants.FANIDUPDATED)
                    ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
                End If
            Else
                ltlErrorMessage.Text = _wfrPage.Content("PleaseSelectAFanId", _languageCode, True)
            End If

            If Not String.IsNullOrEmpty(ltlErrorMessage.Text) Then
                plhErrorMessage.Visible = True
            Else
                plhErrorMessage.Visible = False
            End If
        Catch ex As Exception
            ltlErrorMessage.Text = _errMsg.GetErrorMessage(GlobalConstants.FANIDNOTUPDATED).ERROR_MESSAGE
            pageLogging.ExceptionLog("Smartcard.aspx-PopulateSmartcards", ex.Message)
        End Try

    End Sub

    Protected Sub rptFanIdList_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptFanIdList.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            If ltlCurrentFanIdValue.Text <> e.Item.DataItem("AvailableFanId").ToString OrElse _
                ltlCurrent4DigitsValue.Text <> e.Item.DataItem("LastFourCardDigits").ToString OrElse _
                ltlCurrentIssueValue.Text <> e.Item.DataItem("CardIssueNumber").ToString Then
                CType(e.Item.FindControl("plhItemRow"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("lblAvailableFanId"), Label).Text = e.Item.DataItem("AvailableFanId").ToString
                CType(e.Item.FindControl("lblAvailable4digits"), Label).Text = e.Item.DataItem("LastFourCardDigits").ToString
                CType(e.Item.FindControl("lblAvailableIssueNumber"), Label).Text = e.Item.DataItem("CardIssueNumber").ToString
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Retrieves the list of available fan id's and binds them to the page
    ''' </summary>
    ''' <returns>True if there are available fan ids bound to the list</returns>
    ''' <remarks></remarks>
    Private Function bindAvailableFanIds() As Boolean
        Dim fanIdsBoundToList As Boolean = False
        Dim availableFanIds As New DataTable
        Dim settings As DESettings = TEUtilities.GetSettingsObject()
        Dim smartcards As New TalentSmartcard
        Dim smartcardsDataEntity As New DESmartcard
        Dim err As ErrorObj
        Dim currentSmartCard As String = String.Empty
        Dim currentFanId As String = String.Empty

        'Retrieve the list of fan ids
        smartcardsDataEntity.Src = GlobalConstants.SOURCE
        smartcardsDataEntity.CustomerNumber = Profile.User.Details.LoginID
        settings.Cacheing = TEUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfrPage.Attribute("Caching"))
        settings.CacheTimeMinutes = TEUtilities.CheckForDBNull_Int(_wfrPage.Attribute("CacheTimeInMins"))
        smartcards.Settings = settings
        smartcards.DE = smartcardsDataEntity
        err = smartcards.RetrieveAvailableFanIdList()

        If Not err.HasError AndAlso smartcards.ResultDataSet.Tables.Count = 2 Then
            If smartcards.ResultDataSet.Tables("StatusResults").Rows.Count > 0 AndAlso smartcards.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim returnCode As String = smartcards.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode").ToString()
                Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(returnCode)
                ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
            Else
                If PopulateSmartcards(smartcards.ResultDataSet.Tables("ListOfFanIds")) Then
                    If smartcards.ResultDataSet.Tables("ListOfFanIds").Rows.Count > 0 Then
                        AvailableFanIdLabel = _wfrPage.Content("AvailableFanIdLabel", _languageCode, True)
                        FanIdLastFourNumbersLabel = _wfrPage.Content("FanIdLastFourNumbersLabel", _languageCode, True)
                        FanIdIssueNumberLabel = _wfrPage.Content("FanIdIssueNumberLabel", _languageCode, True)
                        rptFanIdList.DataSource = smartcards.ResultDataSet.Tables("ListOfFanIds")
                        rptFanIdList.DataBind()
                    End If
                End If
            End If
        Else
            Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _wfrPage.PageCode, GlobalConstants.NOFANIDSFOUND)
            ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
        End If
        fanIdsBoundToList = setTextAndPlaceHolders()
        Return fanIdsBoundToList
    End Function

    ''' <summary>
    ''' Set the text and placeholders for page display
    ''' </summary>
    ''' <returns>True if the list of fan id's is bound to the page</returns>
    ''' <remarks></remarks>
    Private Function setTextAndPlaceHolders() As Boolean
        Dim pageSet As Boolean = False
        If rptFanIdList.Items.Count > 0 Then
            pageSet = True
            plhFanIdList.Visible = True
            ltlFanIdDescription.Text = _wfrPage.Content("FanIdDescriptiveText", _languageCode, True)
            plhFanIdDescription.Visible = (ltlFanIdDescription.Text.Length > 0)
            ltlFanIdListLegend.Text = _wfrPage.Content("FanIdFieldSetLegend", _languageCode, True)
            plhUpdateButton.Visible = True
            If String.IsNullOrWhiteSpace(ltlCurrent4DigitsValue.Text) Then
                btnUpdateFanId.Text = _wfrPage.Content("UpdateButtonText", _languageCode, True)
            Else
                btnUpdateFanId.Text = _wfrPage.Content("TransferButtonText", _languageCode, True)
            End If
        Else
            If _activeCards > 1 Then
                ltlErrorMessage.Text = _errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _wfrPage.PageCode, GlobalConstants.MULTIPLESMARTCARDS).ERROR_MESSAGE
            Else
                ltlErrorMessage.Text = _errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _wfrPage.PageCode, GlobalConstants.NOFANIDSFOUND).ERROR_MESSAGE
            End If
            pageSet = False
            plhFanIdList.Visible = False
            plhUpdateButton.Visible = False
        End If
        plhErrorMessage.Visible = (ltlErrorMessage.Text.Length > 0)
        Return pageSet
    End Function

    Private Function PopulateSmartcards(ByRef dtFanIds As DataTable) As Boolean

        PopulateSmartcards = False
        Try
            'Retrieve the active smartcards
            Dim smartcards As New TalentSmartcard
            Dim smartcardsDataEntity As New DESmartcard
            Dim err As ErrorObj
            smartcardsDataEntity.Src = GlobalConstants.SOURCE
            smartcardsDataEntity.CustomerNumber = Profile.User.Details.LoginID
            smartcards.Settings = TEUtilities.GetSettingsObject()
            smartcards.Settings.Cacheing = TEUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfrPage.Attribute("Caching"))
            smartcards.Settings.CacheTimeMinutes = TEUtilities.CheckForDBNull_Int(_wfrPage.Attribute("CacheTimeInMins"))
            smartcards.DE = smartcardsDataEntity
            err = smartcards.RetrieveSmartcardCards()
            If Not err.HasError AndAlso smartcards.ResultDataSet.Tables.Count = 2 Then
                If smartcards.ResultDataSet.Tables("StatusResults").Rows.Count > 0 AndAlso smartcards.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    Dim returnCode As String = smartcards.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode").ToString()
                    Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(returnCode)
                    ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
                Else

                    'Populate the drop down list or retrieve the active card 
                    Dim dt As DataTable = smartcards.ResultDataSet.Tables(1)
                    _activeCards = 0
                    For Each dr As DataRow In dt.Rows
                        If dr("SeasonCard") <> "Y" AndAlso Not String.IsNullOrWhiteSpace(dr("FanId")) Then
                            If _activeCards = 0 Then
                                ltlCurrentSmartcardValue.Text = dr("CardNumber")
                                ltlCurrentFanIdValue.Text = dr("FanId")
                                ltlCurrent4DigitsValue.Text = dr("Last4Digits")
                                ltlCurrentIssueValue.Text = dr("IssueNumber")
                            End If
                            _activeCards += 1
                        End If
                    Next

                    If _activeCards <= 1 Then
                        PopulateSmartcards = True
                        If _activeCards = 1 Then
                            plhCurrentSmartcard.Visible = True
                            ltlCurrentSmartcardLabel.Text = _wfrPage.Content("CurrentSmartCardText", _languageCode, True)
                            ltlCurrentFanIdLabel.Text = _wfrPage.Content("CurrentFanIdText", _languageCode, True)
                            ltlCurrent4DigitsLabel.Text = _wfrPage.Content("Current4DigitsText", _languageCode, True)
                            ltlCurrentIssueLabel.Text = _wfrPage.Content("CurrentCardIssueText", _languageCode, True)
                            ltlCurrentCardDetailsH2.Text = _wfrPage.Content("CurrentCardDetailsTitle", _languageCode, True)
                            PopulateSmartcards = True
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            pageLogging.ExceptionLog("Smartcard.aspx-PopulateSmartcards", ex.Message)
        End Try
        Return PopulateSmartcards
    End Function

#End Region


End Class