Imports Talent.eCommerce
Imports System.Data
Imports System.Data.SqlClient

Partial Class UserControls_MarketingCampaigns
    Inherits ControlBase

#Region "Class Level Fields"

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _display As Boolean = False

#End Region

#Region "Public Properties"

    Public btnContinueText As String = String.Empty

    Public ReadOnly Property MarketingCampaign() As String
        Get
            Return MarketingCampaignsDDL.SelectedValue
        End Get
    End Property

    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "MarketingCampaigns.ascx"
        End With
        btnContinueText = ucr.Content("ContinueButtonText", _languageCode, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Are we displaying the marketing campaigns option
        If Me.Display Then
            Me.Display = False
            If ModuleDefaults.MARKETING_CAMPAIGNS_ACTIVE Then
                Select Case Profile.Basket.BasketContentType
                    Case "M"
                        If ModuleDefaults.MARKETING_CAMPAIGNS_ACTIVE_EBUSINESS Then Me.Display = True
                    Case "C"
                        If ModuleDefaults.MARKETING_CAMPAIGNS_ACTIVE_EBUSINESS Then
                            Me.Display = True
                        Else
                            Me.Display = Profile.Basket.MARKETING_CAMPAIGN
                        End If
                    Case "T"
                        Me.Display = Profile.Basket.MARKETING_CAMPAIGN
                End Select
            End If
        End If

        If Session("CAMPAIGN_CODE") IsNot Nothing Then
            If Session("CAMPAIGN_CODE").ToString.Length > 0 Then Me.Display = False
        End If

        plhDisplayMarketingCampaigns.Visible = Me.Display
    End Sub

    Protected Sub SetLabelText()
        With ucr
            ltlMarketingCampaignsTopText.Text = .Content("MarketingCampaignsTopText", _languageCode, True)
            plhMarketingCampaignsTopText.Visible = (ltlMarketingCampaignsTopText.Text.Length > 0)
            lblMarketingCampaignsSideText.Text = .Content("MarketingCampaignsSideText", _languageCode, True)
            rfvMarketingCampaign.Enabled = Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("CampaignEnableRFV"))
            rfvMarketingCampaign.ErrorMessage = .Content("MarketingCampaignRFVErrorMessage", _languageCode, True)
            btnContinueText = .Content("ContinueButtonText", _languageCode, True)
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            SetLabelText()
            If Not Page.IsPostBack Then
                SetDDLValues()
            End If
        End If
    End Sub

    Protected Sub SetDDLValues()

        'Retrieve the valid payment methods from the list
        MarketingCampaignsDDL.Items.Clear()
        Dim lic As ListItemCollection = GetDropDownControlText()

        'Add the valid payment methods to the drop down list
        Dim li As ListItem = New ListItem(ucr.Content("PleaseSelectText", _languageCode, True), String.Empty)
        MarketingCampaignsDDL.Items.Add(li)
        For Each li In lic
            MarketingCampaignsDDL.Items.Add(li)
        Next

    End Sub

#End Region

#Region "Private Functions"

    Private Function GetDropDownControlText() As ListItemCollection

        Dim dt As New DataTable
        Dim lic As New ListItemCollection
        Dim cacheKey As String = "MarketingCampaignsTable" & Talent.eCommerce.Utilities.GetCurrentLanguage & TalentCache.GetBusinessUnit & TalentCache.GetPartner(HttpContext.Current.Profile)

        'First check cache for the payment stages datatable
        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            lic = HttpContext.Current.Cache.Item(cacheKey)
        Else

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim SelectStr As String = " SELECT MARKETING_CAMPAIGN_CODE, MARKETING_CAMPAIGN_DESCRIPTION " & _
                                        " FROM tbl_marketing_campaigns_lang " & _
                                        " WHERE (LANGUAGE_CODE = @LANGUAGE_CODE) AND " & _
                                        " (BUSINESS_UNIT = @BUSINESS_UNIT) AND (PARTNER_CODE = @PARTNER_CODE)"

            cmd.CommandText = SelectStr

            With cmd.Parameters
                .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER_CODE", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentLanguage
            End With

            Try
                cmd.Connection.Open()

                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dt)

                If dt.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER_CODE").Value = Talent.Common.Utilities.GetAllString
                    da.Fill(dt)
                    If dt.Rows.Count = 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = Talent.Common.Utilities.GetAllString
                        da.Fill(dt)
                    End If
                End If

                da.Dispose()
            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
            End Try

            Dim li As ListItem
            Dim exists As Boolean = False

            For Each row As Data.DataRow In dt.Rows
                exists = False
                For Each item As ListItem In lic
                    If item.Value = row("MARKETING_CAMPAIGN_CODE") Then
                        exists = True
                        Exit For
                    End If
                Next
                If Not exists Then
                    li = New ListItem(row("MARKETING_CAMPAIGN_DESCRIPTION"), row("MARKETING_CAMPAIGN_CODE"))
                    lic.Add(li)
                End If
            Next

            'Cache the results
            HttpContext.Current.Cache.Add(cacheKey, lic, Nothing, Now.AddMinutes(30), TimeSpan.Zero, CacheItemPriority.Normal, Nothing)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        End If

        Return lic

    End Function

#End Region

End Class