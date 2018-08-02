Imports System.Data
Imports Talent.Common

Partial Class UserControls_SeasonTicketWaitListSummary
    Inherits System.Web.UI.UserControl

    Public Enum Location
        WaitListPage = 1
        ConfirmationPage = 2
    End Enum

    Dim ucr As New UserControlResource
    Dim _langCode As String = Talent.eCommerce.Utilities.GetCurrentLanguage

    Dim moduleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
    Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues

    Private _display As Boolean
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property


    Private _usage As Location
    Public Property Usage() As Location
        Get
            Return _usage
        End Get
        Set(ByVal value As Location)
            _usage = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        def = moduleDefaults.GetDefaults()
        ucr = New UserControlResource

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SeasonTicketWaitListSummary.ascx"
        End With

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        SetText()
        If Not Page.IsPostBack Then
            PopulateSummary()
        End If
    End Sub

    Protected Sub PopulateSummary()
        Dim de As New Talent.Common.DEWaitList
        de.CustomerNumber = Profile.UserName
        de.Src = "W"
        Dim twl As New TalentWaitList
        twl.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        twl.DE = de
        With twl.Settings
            .BusinessUnit = TalentCache.GetBusinessUnit
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        End With

        Dim err As ErrorObj = twl.RetrieveCustomerWaitListHistory

        If Not err.HasError Then
            Dim ds As DataSet = twl.ResultDataSet
            If ds IsNot Nothing AndAlso ds.Tables.Count > 1 Then

                Dim hTbl As DataTable = ds.Tables("DtWaitListHeaderResults")
                If hTbl.Rows.Count > 0 Then
                    'If CStr(hTbl.Rows(0)("WaitListRequests")).ToUpper = "Y" Then
                    hfWaitListID.Value = Utilities.CheckForDBNull_String(hTbl.Rows(0)("WaitListID"))
                    Select Case Utilities.CheckForDBNull_String(hTbl.Rows(0)("Status")).ToUpper
                        Case "W"
                            statusVal.Text = ucr.Content("WithdrawnText", _langCode, True)
                            withdrawButton.Enabled = False
                            withdrawButton.Visible = False
                        Case "F"
                            statusVal.Text = ucr.Content("FulfilledText", _langCode, True)
                            withdrawButton.Enabled = False
                            withdrawButton.Visible = False
                        Case "P"
                            statusVal.Text = ucr.Content("PendingText", _langCode, True)
                        Case "R"
                            statusVal.Text = ucr.Content("RejectedText", _langCode, True)
                            withdrawButton.Enabled = False
                            withdrawButton.Visible = False
                    End Select

                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedDate"))) _
                        AndAlso Convert.ToString(hTbl.Rows(0)("AddedDate")).Length = 7 Then
                        dateVal.Text = Utilities.ISeriesDate(hTbl.Rows(0)("AddedDate")).ToString("dd/MM/yyyy")
                    End If

                    addedVal.Text = Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedBy_CustomerNo")) & _
                                    " - " & _
                                    Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedBy_FirstName")) & _
                                    " " & _
                                    Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedBy_Surname"))

                    PopulateStandDescriptions(Utilities.CheckForDBNull_String(hTbl.Rows(0)("PreferredStand1")), _
                                                Utilities.CheckForDBNull_String(hTbl.Rows(0)("PreferredArea1")), _
                                                Utilities.CheckForDBNull_String(hTbl.Rows(0)("PreferredStand2")), _
                                                Utilities.CheckForDBNull_String(hTbl.Rows(0)("PreferredArea2")), _
                                                Utilities.CheckForDBNull_String(hTbl.Rows(0)("PreferredStand3")), _
                                                Utilities.CheckForDBNull_String(hTbl.Rows(0)("PreferredArea3")))

                    Dim dTbl As DataTable = ds.Tables("DtWaitListDetailResults")
                    If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                        WaitListRepeater.DataSource = dTbl
                        WaitListRepeater.DataBind()
                    End If
                    If Usage = Location.ConfirmationPage Then
                        Dim htmlFormat As Boolean = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ConfirmationEmail_IsHTML"))
                        Dim emailBody As String = String.Empty
                        If htmlFormat Then
                            emailBody = ucr.Content("ConfirmationEmail_BodyTextHTML", _langCode, True)
                            emailBody = emailBody.Replace(vbCrLf, "<br>")
                        Else
                            emailBody = ucr.Content("ConfirmationEmail_BodyText", _langCode, True)
                        End If
                        emailBody = emailBody.Replace("<<WaitListID>>", hfWaitListID.Value.Trim)
                        emailBody = emailBody.Replace("<<AddedDate>>", dateVal.Text.Trim)
                        emailBody = emailBody.Replace("<<AddedBy_FirstName>>", Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedBy_FirstName")).Trim)
                        emailBody = emailBody.Replace("<<AddedBy_Surname>>", Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedBy_Surname")).Trim)
                        emailBody = emailBody.Replace("<<AddedBy_CustomerNo>>", Utilities.CheckForDBNull_String(hTbl.Rows(0)("AddedBy_CustomerNo")).Trim)
                        emailBody = emailBody.Replace("<<PreferredStand1>>", prefStand1Val.Text.Trim)
                        emailBody = emailBody.Replace("<<PreferredStand2>>", prefStand2Val.Text.Trim)
                        emailBody = emailBody.Replace("<<PreferredStand3>>", prefStand3Val.Text.Trim)
                        emailBody = emailBody.Replace("<<PreferredArea1>>", prefArea1Val.Text.Trim)
                        emailBody = emailBody.Replace("<<PreferredArea2>>", prefArea2Val.Text.Trim)
                        emailBody = emailBody.Replace("<<PreferredArea3>>", prefArea3Val.Text.Trim)
                        emailBody = emailBody.Replace("<<CustomersList>>", CustomerListForEmail(dTbl))

                        Dim email As String = ""
                        If String.IsNullOrEmpty(Request.QueryString("email")) Then
                            email = Profile.User.Details.Email
                        Else
                            email = Request.QueryString("email")
                        End If

                        Utilities.Email_Send(def.OrdersFromEmail, _
                                                email, _
                                                ucr.Content("ConfirmationEmail_SubjectText", _langCode, True), _
                                                emailBody, "", False, _
                                                htmlFormat)
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub PopulateStandDescriptions(ByVal s1 As String, ByVal a1 As String, ByVal s2 As String, ByVal a2 As String, ByVal s3 As String, ByVal a3 As String)

        Dim descs As DataSet = GetStandsAndAreasDescriptions()

        If descs.Tables.Count > 1 Then
            If descs.Tables(1).Rows.Count > 0 Then
                FormatStandAndArea(descs, s1, a1, prefStand1Val.Text, prefArea1Val.Text)
                FormatStandAndArea(descs, s2, a2, prefStand2Val.Text, prefArea2Val.Text)
                FormatStandAndArea(descs, s3, a3, prefStand3Val.Text, prefArea3Val.Text)

                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AreaSelection_InUse")) Then
                    prefArea1Val.Visible = False
                    prefArea2Val.Visible = False
                    prefArea3Val.Visible = False
                    prefArea1Label.Visible = False
                    prefArea2Label.Visible = False
                    prefArea3Label.Visible = False
                    pnlPreferredAreaRow1.Visible = False
                    pnlPreferredAreaRow2.Visible = False
                    pnlPreferredAreaRow3.Visible = False
                End If
            End If
        End If

    End Sub

    Protected Sub FormatStandAndArea(ByVal descs As DataSet, _
                                        ByVal stand As String, _
                                        ByVal area As String, _
                                        ByRef standDesc As String, _
                                        ByRef areaDesc As String)

        Dim dv1() As DataRow
        If area.Trim.Equals("*ANY") OrElse area.Trim.Equals(String.Empty) Then
            dv1 = descs.Tables(1).Select("StandCode='" + stand.Trim + "'")
        Else
            dv1 = descs.Tables(1).Select("StandCode='" + stand.Trim + "' and AreaCode='" & area.Trim & "'")
        End If
        If dv1.Length > 0 Then
            standDesc = Utilities.CheckForDBNull_String(dv1(0)("StandDescription"))
            If area.Trim.Equals("*ANY") OrElse area.Trim.Equals(String.Empty) Then
                areaDesc = ucr.Content("AnyAreaText", _langCode, True)
            Else
                areaDesc = Utilities.CheckForDBNull_String(dv1(0)("AreaDescription"))
            End If
        End If

    End Sub

    Protected Function GetStandsAndAreasDescriptions() As DataSet
        GetStandsAndAreasDescriptions = New DataSet
        Try

            Dim product As New TalentProduct
            Dim settings As New DESettings
            Dim err As New ErrorObj
            Dim depd As New Talent.Common.DEProductDetails

            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.CacheDependencyPath = def.CacheDependencyPath

            depd.ProductCode = ""
            depd.Src = "W"
            depd.ProductType = ""
            depd.CampaignCode = ""
            depd.WaitList = True
            depd.StadiumCode = def.TicketingStadium.Split(",")(0)

            product.Settings() = settings
            product.De = depd

            err = product.StandDescriptions()
            GetStandsAndAreasDescriptions = product.ResultDataSet


            If Not err.HasError Then
                Return GetStandsAndAreasDescriptions
            Else
                Return New DataSet
            End If
        Catch ex As Exception
            Return New DataSet
        End Try
    End Function


    Protected Function CustomerListForEmail(ByVal dTbl As DataTable) As String
        CustomerListForEmail = ""
        If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
            For Each dr As DataRow In dTbl.Rows
                CustomerListForEmail += ucr.Content("ConfirmationEmailText_CustomerListFormat", _langCode, True) & vbCrLf
                Select Case Utilities.CheckForDBNull_String(dr("Status")).ToUpper
                    Case "W"
                        CustomerListForEmail = CustomerListForEmail.Replace("<<Status>>", ucr.Content("WithdrawnText", _langCode, True))
                    Case "F"
                        CustomerListForEmail = CustomerListForEmail.Replace("<<Status>>", ucr.Content("FulfilledText", _langCode, True))
                    Case "P"
                        CustomerListForEmail = CustomerListForEmail.Replace("<<Status>>", ucr.Content("PendingText", _langCode, True))
                    Case "R"
                        CustomerListForEmail = CustomerListForEmail.Replace("<<Status>>", ucr.Content("RejectedText", _langCode, True))
                End Select
                CustomerListForEmail = CustomerListForEmail.Replace("<<FirstName>>", Utilities.CheckForDBNull_String(dr("FirstName")).Trim)
                CustomerListForEmail = CustomerListForEmail.Replace("<<Surname>>", Utilities.CheckForDBNull_String(dr("Surname")).Trim)
                CustomerListForEmail = CustomerListForEmail.Replace("<<CustomerNumber>>", Utilities.CheckForDBNull_String(dr("CustomerNumber")).Trim)
            Next
        End If
        Return CustomerListForEmail
    End Function

    Protected Sub SetText()
        If Not Page.IsPostBack Then
            titleLabel.Text = ucr.Content("TitleText", _langCode, True)
            contentLabel.Text = ucr.Content("IntroText", _langCode, True)
            statusLabel.Text = ucr.Content("StatusLabel", _langCode, True)
            dateLabel.Text = ucr.Content("DateLabel", _langCode, True)
            addedLabel.Text = ucr.Content("AddedByLabel", _langCode, True)
            prefStand1Label.Text = ucr.Content("PreferredStand1Label", _langCode, True)
            prefStand2Label.Text = ucr.Content("PreferredStand2Label", _langCode, True)
            prefStand3Label.Text = ucr.Content("PreferredStand3Label", _langCode, True)
            prefArea1Label.Text = ucr.Content("PreferredArea1Label", _langCode, True)
            prefArea2Label.Text = ucr.Content("PreferredArea2Label", _langCode, True)
            prefArea3Label.Text = ucr.Content("PreferredArea3Label", _langCode, True)
            withdrawButton.Text = ucr.Content("WithdrawButtonText", _langCode, True)
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            Select Case CType(sender, Control).ID.ToLower

                Case "statusheader"
                    CType(sender, Label).Text = ucr.Content("StatusHeaderLabelText", _langCode, True)

                Case "custnameheader"
                    CType(sender, Label).Text = ucr.Content("CustomerNameHeaderLabelText", _langCode, True)

                Case "custnumberheader"
                    CType(sender, Label).Text = ucr.Content("CustomerNumberHeaderLabelText", _langCode, True)


            End Select
        End If
    End Sub

    Protected Sub WaitListRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles WaitListRepeater.ItemDataBound
        If e.Item.ItemIndex > -1 Then
            Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim statusLbl As Label = CType(e.Item.FindControl("statusValLabel"), Label)
            Dim nameLbl As Label = CType(e.Item.FindControl("custNameValLabel"), Label)
            Dim custNoLbl As Label = CType(e.Item.FindControl("custNumberValLabel"), Label)

            Select Case Utilities.CheckForDBNull_String(dr("Status")).ToUpper
                Case "W"
                    statusLbl.Text = ucr.Content("WithdrawnText", _langCode, True)
                Case "F"
                    statusLbl.Text = ucr.Content("FulfilledText", _langCode, True)
                Case "P"
                    statusLbl.Text = ucr.Content("PendingText", _langCode, True)
                Case "R"
                    statusLbl.Text = ucr.Content("RejectedText", _langCode, True)
            End Select

            nameLbl.Text = Utilities.CheckForDBNull_String(dr("FirstName")) & " " & _
                            Utilities.CheckForDBNull_String(dr("Surname"))
            custNoLbl.Text = Utilities.CheckForDBNull_String(dr("CustomerNumber"))
        End If
    End Sub

    Protected Sub withdrawButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles withdrawButton.Click
        Response.Redirect("~/Redirect/TicketingGateway.aspx?page=waitlist.aspx&function=withdrawseasonticketwaitlist&wlid=" & hfWaitListID.Value)
    End Sub
End Class
