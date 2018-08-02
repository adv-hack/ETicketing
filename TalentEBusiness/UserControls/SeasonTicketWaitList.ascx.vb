Imports System.Data
Imports Talent.Common

Partial Class UserControls_SeasonTicketWaitList
    Inherits ControlBase

    Dim ucr As New UserControlResource
    Dim _langCode As String = Talent.eCommerce.Utilities.GetCurrentLanguage



    Private _display As Boolean
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property


    Private _seasonTicket As String
    Public Property CurrentSeasonTicket() As String
        Get
            Return _seasonTicket
        End Get
        Set(ByVal value As String)
            _seasonTicket = value
        End Set
    End Property

    Private _nextSeasonTicket As String
    Public Property NextSeasonTicket() As String
        Get
            Return _nextSeasonTicket
        End Get
        Set(ByVal value As String)
            _nextSeasonTicket = value
        End Set
    End Property


    Private _productCode As String
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        ucr = New UserControlResource

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SeasonTicketWaitList.ascx"
        End With

        SetText()

        Dim ds As DataSet = GetStandsAndAreasDescriptions()
        Try
            CurrentSeasonTicket = ds.Tables("DtSeasonTicketResults").Rows(0)("CurrentSeasonTicket")
            NextSeasonTicket = ds.Tables("DtSeasonTicketResults").Rows(0)("NextSeasonTicket")
        Catch ex As Exception
        End Try

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
            settings.Cacheing = CType(ucr.Attribute("Cacheing"), Boolean)
            settings.CacheTimeMinutes = CType(ucr.Attribute("CacheTimeMinutes"), Integer)
            settings.CacheTimeMinutesSecondFunction = CType(ucr.Attribute("StandDescriptionsCacheTimeMinutes"), Integer)
            settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath

            depd.ProductCode = ""
            depd.Src = "W"
            depd.ProductType = ""
            depd.CampaignCode = ""
            depd.WaitList = True
            depd.StadiumCode = ModuleDefaults.TicketingStadium.Split(",")(0)

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


    Protected Sub LoadStandsAndAreas()
        Dim ds As DataSet = GetStandsAndAreasDescriptions()

        prefStand1DDL.Visible = True
        prefStand2DDL.Visible = True
        prefStand3DDL.Visible = True

        Dim dsHelper As New DataSetHelperST(New DataSet)

        Dim distinctTable As DataTable = dsHelper.SelectDistinct(ds.Tables(1).TableName, ds.Tables(1), "StandCode", "StandDescription")
        prefStand1DDL.DataSource = distinctTable
        prefStand1DDL.DataTextField = "StandDescription"
        prefStand1DDL.DataValueField = "StandCode"
        prefStand1DDL.DataBind()
        If prefStand1DDL.Items.Count > 0 Then
            hfStand1Selected.Value = prefStand1DDL.Items(0).Value
            StandDropDownFill(prefStand1DDL.SelectedValue, 1, ds)
        End If

        prefStand2DDL.DataSource = distinctTable
        prefStand2DDL.DataTextField = "StandDescription"
        prefStand2DDL.DataValueField = "StandCode"
        prefStand2DDL.DataBind()
        prefStand2DDL.Items.Insert(0, New ListItem("", ""))
        prefStand2DDL.SelectedIndex = 0

        prefStand3DDL.DataSource = distinctTable
        prefStand3DDL.DataTextField = "StandDescription"
        prefStand3DDL.DataValueField = "StandCode"
        prefStand3DDL.DataBind()
        prefStand3DDL.Items.Insert(0, New ListItem("", ""))
        prefStand3DDL.SelectedIndex = 0


    End Sub

    Public Sub StandDropDownFill(ByVal strStrnd As String, ByVal ddlNumber As Integer, ByVal ds As DataSet)
        Dim dv As DataView
        dv = ds.Tables(1).DefaultView
        dv.RowFilter = "StandCode='" + strStrnd + "'"

        Dim li As New ListItem
        li.Text = ucr.Content("AnyAreaText", _langCode, True)
        li.Value = "*ANY"

        Select Case ddlNumber
            Case 1
                prefArea1DDL.DataSource = dv
                prefArea1DDL.DataTextField = "AreaDescription"
                prefArea1DDL.DataValueField = "AreaCode"
                prefArea1DDL.DataBind()
                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) Then
                    prefArea1DDL.Items.Insert(0, li)
                End If
                If prefArea1DDL.Items.Count > 0 Then
                    hfArea1Selected.Value = prefArea1DDL.Items(0).Value
                End If
            Case 2
                prefArea2DDL.DataSource = dv
                prefArea2DDL.DataTextField = "AreaDescription"
                prefArea2DDL.DataValueField = "AreaCode"
                prefArea2DDL.DataBind()
                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) Then
                    prefArea2DDL.Items.Insert(0, li)
                End If
                If prefArea2DDL.Items.Count > 0 Then
                    hfArea2Selected.Value = prefArea2DDL.Items(0).Value
                End If
            Case 3
                prefArea3DDL.DataSource = dv
                prefArea3DDL.DataTextField = "AreaDescription"
                prefArea3DDL.DataValueField = "AreaCode"
                prefArea3DDL.DataBind()
                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) Then
                    prefArea3DDL.Items.Insert(0, li)
                End If
                If prefArea3DDL.Items.Count > 0 Then
                    hfArea3Selected.Value = prefArea3DDL.Items(0).Value
                End If

        End Select

    End Sub

    Protected Sub SetText()
        If Not Page.IsPostBack Then
            titleLabel.Text = ucr.Content("TitleText", _langCode, True)
            If quantityDDL.Enabled Then
                contentLabel.Text = ucr.Content("IntroText", _langCode, True)
            Else
                contentLabel.Text = ucr.Content("IntroText2", _langCode, True)
            End If

            hfProductCode.Value = NextSeasonTicket
            quantityLabel.Text = ucr.Content("QuantityLabel", _langCode, True)

            prefStand1Label.Text = ucr.Content("PreferredStand1Label", _langCode, True)
            prefStand2Label.Text = ucr.Content("PreferredStand2Label", _langCode, True)
            prefStand3Label.Text = ucr.Content("PreferredStand3Label", _langCode, True)
            prefArea1Label.Text = ucr.Content("PreferredArea1Label", _langCode, True)
            prefArea2Label.Text = ucr.Content("PreferredArea2Label", _langCode, True)
            prefArea3Label.Text = ucr.Content("PreferredArea3Label", _langCode, True)

            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("EmailAddress_InUse")) Then
                emailLabel.Text = ucr.Content("EmailAddressLabel", _langCode, True)
            Else
                emailRow.Visible = False
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("DaytimePhoneNumber_InUse")) Then
                daytimePhoneLabel.Text = ucr.Content("DaytimePhoneLabel", _langCode, True)
            Else
                dPhoneRow.Visible = False
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("EveningPhoneNumber_InUse")) Then
                eveningPhoneLabel.Text = ucr.Content("EveningPhoneLabel", _langCode, True)
            Else
                ePhoneRow.Visible = False
            End If
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("MobilePhoneNumber_InUse")) Then
                mobilePhoneLabel.Text = ucr.Content("MobilePhoneLabel", _langCode, True)
            Else
                mPhoneRow.Visible = False
            End If

            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AreaSelection_InUse")) Then
                paRow1.Visible = True
                paRow2.Visible = True
                paRow3.Visible = True
            Else
                paRow1.Visible = False
                paRow2.Visible = False
                paRow3.Visible = False
            End If

            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AreaSelection_IsMandatory")) Then
                dPhoneRegEx.ValidationExpression = ucr.Attribute("PhoneNoExpression")
                ePhoneRegEx.ValidationExpression = ucr.Attribute("PhoneNoExpression")
                mPhoneRegEx.ValidationExpression = ucr.Attribute("PhoneNoExpression")
                dPhoneRegEx.ErrorMessage = ucr.Content("DaytimePhoneValidationFailedError", _langCode, True)
                ePhoneRegEx.ErrorMessage = ucr.Content("EveningPhoneValidationFailedError", _langCode, True)
                mPhoneRegEx.ErrorMessage = ucr.Content("MobilePhoneValidationFailedError", _langCode, True)
            Else
                dPhoneRegEx.Enabled = False
                ePhoneRegEx.Enabled = False
                mPhoneRegEx.Enabled = False
            End If

            emailRegEx.ValidationExpression = ucr.Attribute("EmailExpression")
            emailRegEx.ErrorMessage = ucr.Content("EmailValidationFailedError", _langCode, True)

            commentLabel.Text = ucr.Content("CommentLabel", _langCode, True)

            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("DataProtectionCheck_InUse")) Then
                dataProtectionLabel.Text = ucr.Content("DataProtectionLabel", _langCode, True)
                dataProtectionCheck.Text = ucr.Content("DataProtectionStatement", _langCode, True)
            Else
                dataProtectionRow.Visible = False
            End If

            dataProtectionLabel.Text = ucr.Content("DataProtectionLabel", _langCode, True)
            dataProtectionCheck.Text = ucr.Content("DataProtectionStatement", _langCode, True)
            nextButton.Text = ucr.Content("NextButtonText", _langCode, True)
            BackButton.Text = ucr.Content("BackButtonText", _langCode, True)

            standLoadButton1.Text = ucr.Content("LoadAreasButton1Text", _langCode, True)
            standLoadButton2.Text = ucr.Content("LoadAreasButton2Text", _langCode, True)
            standLoadButton3.Text = ucr.Content("LoadAreasButton3Text", _langCode, True)

            Dim maxQty As Integer = 0
            If String.IsNullOrEmpty(ucr.Attribute("MaxQuantity")) Then
                maxQty = 8
            Else
                maxQty = Convert.ToInt16(ucr.Attribute("MaxQuantity"))
            End If

            For i As Integer = 0 To maxQty
                quantityDDL.Items.Add(New ListItem(i.ToString))
            Next

        End If
    End Sub



    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsPostBack Then
            Select Case CType(sender, Control).ID.ToLower

                Case "customersLabel"
                    CType(sender, Label).Text = ucr.Content("CustomersLabel", _langCode, True)

            End Select
        End If
    End Sub

    Protected Sub nextButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles nextButton.Click
        errorList.Items.Clear()
        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("DataProtectionCheck_InUse")) Then
            If Not dataProtectionCheck.Checked Then
                errorList.Items.Add(ucr.Content("DataProtectionCheck_NotCheckedError", _langCode, True))
            End If
        End If

        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("EmailAddress_InUse")) Then
            If String.IsNullOrEmpty(emailBox.Text) Then
                errorList.Items.Add(ucr.Content("EmailAddress_NotSuppliedError", _langCode, True))
            End If
        End If

        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AreaSelection_InUse")) Then
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) _
                AndAlso String.IsNullOrEmpty(hfArea1Selected.Value) Then
                errorList.Items.Add(ucr.Content("AreaSelection_NotSuppliedError", _langCode, True))
            End If
        End If

        If errorList.Items.Count = 0 Then
            Dim url As String = "~/Redirect/TicketingGateway.aspx?page=waitlist.aspx&function=addseasonticketwaitlist"
            url += "&stand1=" & Server.UrlEncode(hfStand1Selected.Value)
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("AreaSelection_InUse")) Then
                url += "&area1=" & Server.UrlEncode(hfArea1Selected.Value)
            Else
                url += "&area1=" & Server.UrlEncode("*ANY")
            End If

            If Not String.IsNullOrEmpty(hfStand2Selected.Value) Then
                url += "&stand2=" & Server.UrlEncode(hfStand2Selected.Value)
                If Not String.IsNullOrEmpty(hfArea2Selected.Value) Then
                    url += "&area2=" & Server.UrlEncode(hfArea2Selected.Value)
                Else
                    url += "&area2=" & Server.UrlEncode("*ANY")
                End If
            End If
            
            If Not String.IsNullOrEmpty(hfStand3Selected.Value) Then
                url += "&stand3=" & Server.UrlEncode(hfStand3Selected.Value)
                If Not String.IsNullOrEmpty(hfArea3Selected.Value) Then
                    url += "&area3=" & Server.UrlEncode(hfArea3Selected.Value)
                Else
                    url += "&area3=" & Server.UrlEncode("*ANY")
                End If
            End If
            

            url += "&qty=" & Server.UrlEncode(quantityDDL.SelectedValue)
            url += "&product=" & Server.UrlEncode(hfProductCode.Value)

            If Not String.IsNullOrEmpty(daytimePhoneBox.Text) Then
                url += "&dp=" & Server.UrlEncode(daytimePhoneBox.Text)
            End If
            If Not String.IsNullOrEmpty(eveningPhoneBox.Text) Then
                url += "&ep=" & Server.UrlEncode(eveningPhoneBox.Text)
            End If
            If Not String.IsNullOrEmpty(mobilePhoneBox.Text) Then
                url += "&mp=" & Server.UrlEncode(mobilePhoneBox.Text)
            End If
            If Not String.IsNullOrEmpty(emailBox.Text) Then
                url += "&email=" & Server.UrlEncode(emailBox.Text)
            End If
            If Not String.IsNullOrEmpty(commentBox1.Text) Then
                url += "&c1=" & Server.UrlEncode(commentBox1.Text)
            End If
            If Not String.IsNullOrEmpty(commentBox2.Text) Then
                url += "&c2=" & Server.UrlEncode(commentBox2.Text)
            End If

            Dim customers As String = String.Empty
            For i As Integer = 0 To customersRepeater.Items.Count - 1
                Dim ddl As DropDownList = customersRepeater.Items(i).FindControl("customerDDL")
                customers += ddl.SelectedValue
                If Not (i = customersRepeater.Items.Count - 1) Then
                    customers += ","
                End If
            Next
            url += "&customerList=" & Server.UrlEncode(customers)

            Response.Redirect(url)
        End If



    End Sub

    Protected Sub quantityDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles quantityDDL.SelectedIndexChanged
        If CInt(quantityDDL.SelectedValue) > 0 Then
            waitListDetailsPanel.Visible = True
            Dim dt As New DataTable
            For i As Integer = 0 To CInt(quantityDDL.SelectedValue) - 1
                dt.Rows.Add(dt.NewRow)
            Next
            customersRepeater.DataSource = dt
            customersRepeater.DataBind()
            LoadStandsAndAreas()
            quantityDDL.Enabled = False
            contentLabel.Text = ucr.Content("IntroText2", _langCode, True)
            daytimePhoneBox.Text = Profile.User.Details.Work_Number
            eveningPhoneBox.Text = Profile.User.Details.Telephone_Number
            mobilePhoneBox.Text = Profile.User.Details.Mobile_Number
            emailBox.Text = Profile.User.Details.Email

            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("UseEmailAndPhoneNoFromProfile")) Then
                emailBox.Enabled = False
                If Not String.IsNullOrEmpty(daytimePhoneBox.Text) Then daytimePhoneBox.Enabled = False
                If Not String.IsNullOrEmpty(eveningPhoneBox.Text) Then eveningPhoneBox.Enabled = False
                If Not String.IsNullOrEmpty(mobilePhoneBox.Text) Then mobilePhoneBox.Enabled = False
            End If

            Me.javascriptForStandDDLs.Text = Me.WriteDDLJavascript
        End If
    End Sub


    Protected Sub customersRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles customersRepeater.ItemDataBound
        If e.Item.ItemIndex > -1 Then

            Dim customerDDL As DropDownList = CType(e.Item.FindControl("customerDDL"), DropDownList)

            ' Set up the calls to add items to ticketing basket
            Dim _talentErrObj As New Talent.Common.ErrorObj
            Dim _deCustomer As New Talent.Common.DECustomer
            Dim _deSettings As New Talent.Common.DESettings

            ' Set the settings data entity. 
            _deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            _deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
            _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            _deSettings.Cacheing = True
            _deSettings.CacheTimeMinutes = CInt(ucr.Attribute("CacheTimeMinutes"))

            customerDDL.Items.Clear()
            customerDDL.Items.Add(New ListItem(Profile.UserName & " - " & Profile.User.Details.Full_Name, Profile.UserName))


            ' Set the customer defaults
            _deCustomer.UserName = Profile.UserName
            _deCustomer.CustomerNumber = Profile.User.Details.LoginID
            _deCustomer.Source = "W"

            ' Invoke.
            Dim _talentCustomer As New Talent.Common.TalentCustomer
            Dim _deCustV11 As New Talent.Common.DECustomerV11
            _deCustV11.DECustomersV1.Add(_deCustomer)

            _talentCustomer.DeV11 = _deCustV11
            _talentCustomer.Settings = _deSettings
            _talentErrObj = _talentCustomer.CustomerAssociations

            If Not _talentErrObj.HasError Then

                If _talentCustomer.ResultDataSet.Tables.Count > 1 Then
                    Dim tbl1 As DataTable = _talentCustomer.ResultDataSet.Tables(0), _
                        tbl2 As DataTable = _talentCustomer.ResultDataSet.Tables(1)
                    For Each rw As DataRow In tbl2.Rows
                        customerDDL.Items.Add(New ListItem(Utilities.CheckForDBNull_String(rw("AssociatedCustomerNumber")) & _
                                                            " - " & Utilities.CheckForDBNull_String(rw("Forename")) & _
                                                            " " & Utilities.CheckForDBNull_String(rw("Surname")), _
                                                            Utilities.CheckForDBNull_String(rw("AssociatedCustomerNumber"))))
                    Next
                End If
            End If
        End If

    End Sub


    Protected Function GetJavascriptStringStand1Change() As String
        Return "Javascript: document.getElementById('" & hfStand1Selected.ClientID & "').value=this.value; LoadAreasForDDL1(this.form); PopulateDDL1Hidden();"
    End Function
    Protected Function GetJavascriptStringStand2Change() As String
        Return "Javascript: document.getElementById('" & hfStand2Selected.ClientID & "').value=this.value; LoadAreasForDDL2(this.form); PopulateDDL2Hidden();"
    End Function
    Protected Function GetJavascriptStringStand3Change() As String
        Return "Javascript: document.getElementById('" & hfStand3Selected.ClientID & "').value=this.value; LoadAreasForDDL3(this.form); PopulateDDL3Hidden();"
    End Function

    Protected Function WriteDDLJavascript() As String

        ' Populate ready for WriteDDLJavascript
        Dim dtProduct As New DataTable

        Dim ds As DataSet = GetStandsAndAreasDescriptions()

        If ds IsNot Nothing Then
            If ds.Tables.Count > 1 Then
                dtProduct = ds.Tables(1)
            End If
        End If

        Dim sb As New StringBuilder
        Const sCrLf As String = vbCrLf

        Dim dr As DataRow
        Dim sStandCode As String = String.Empty
        Dim sStandDesc As String = String.Empty
        Dim sAreaCode As String = String.Empty
        Dim sAreaDesc As String = String.Empty
        Dim saveStandCode As String = String.Empty

        sb.Append(sCrLf)
        sb.Append("<script language=""javascript"" type=""text/javascript"">" & sCrLf)

        'Hide the 'Load Areas Buttons' if Javascript is on
        '--------------------------------------------------------
        sb.Append("document.getElementById('" & standLoadButton1.ClientID & "').style.display = 'none';")
        sb.Append("document.getElementById('" & standLoadButton2.ClientID & "').style.display = 'none';")
        sb.Append("document.getElementById('" & standLoadButton3.ClientID & "').style.display = 'none';")
        '--------------------------------------------------------

        sb.Append("function removeAllOptions(ddl){ ")
        sb.Append("var i; ")
        sb.Append("for(i=ddl.options.length-1;i>=0;i--)")
        sb.Append("{")
        sb.Append("	ddl.remove(i);")
        sb.Append("}")
        sb.Append("}")
        sb.Append("function addOption(selectbox, value, text){")
        sb.Append("var optn = document.createElement(""OPTION"");")
        sb.Append("optn.text = text;")
        sb.Append("optn.value = value;")
        sb.Append("selectbox.options.add(optn);")
        sb.Append("}")


        'DDLs 1
        '----------------------------------------------------
        sb.Append("function LoadAreasForDDL1(thisform){" & sCrLf)
        sb.Append("var ddl1 = document.getElementById('" & prefArea1DDL.ClientID & "');" & sCrLf)
        sb.Append("var newStandCode1 = document.getElementById('" & prefStand1DDL.ClientID & "').value;" & sCrLf)

        sb.Append("if (trim(newStandCode1) == """") {" & sCrLf)
        sb.Append("removeAllOptions(ddl1);" & sCrLf)
        sb.Append("addOption(ddl1, """", """");" & sCrLf)
        sb.Append("}" & sCrLf)
        For Each dr In dtProduct.Rows
            sStandCode = dr("StandCode").ToString.Trim
            sStandDesc = dr("StandDescription").ToString.Trim
            sAreaCode = dr("AreaCode").ToString
            sAreaDesc = dr("AreaDescription").ToString.Trim
            If Not (sStandCode = saveStandCode) Then
                If Not saveStandCode.Trim = "" Then
                    sb.Append("}" & sCrLf)
                End If
                sb.Append("if (trim(newStandCode1) == """ & sStandCode & """) {" & sCrLf)
                sb.Append("removeAllOptions(ddl1);" & sCrLf)
                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) Then
                    sb.Append("addOption(ddl1, """ & "*ANY" & """, """ & ucr.Content("AnyAreaText", _langCode, True) & """);" & sCrLf)
                End If
            End If
            sb.Append("addOption(ddl1, """ & sAreaCode & """, """ & sAreaDesc & """);" & sCrLf)
            saveStandCode = sStandCode
        Next
        sb.Append("}" & sCrLf)
        sb.Append("}" & sCrLf)

        sStandCode = ""
        sStandDesc = ""
        sAreaCode = ""
        sAreaDesc = ""
        saveStandCode = ""

        'DDLs 2
        '----------------------------------------------------
        sb.Append("function LoadAreasForDDL2(thisform){" & sCrLf)
        sb.Append("var ddl2 = document.getElementById('" & prefArea2DDL.ClientID & "');" & sCrLf)
        sb.Append("var newStandCode2 = document.getElementById('" & prefStand2DDL.ClientID & "').value;" & sCrLf)

        sb.Append("if (trim(newStandCode2) == """") {" & sCrLf)
        sb.Append("removeAllOptions(ddl2);" & sCrLf)
        sb.Append("addOption(ddl2, """", """");" & sCrLf)
        sb.Append("}" & sCrLf)
        For Each dr In dtProduct.Rows
            sStandCode = dr("StandCode").ToString.Trim
            sStandDesc = dr("StandDescription").ToString.Trim
            sAreaCode = dr("AreaCode").ToString
            sAreaDesc = dr("AreaDescription").ToString.Trim
            If Not (sStandCode = saveStandCode) Then
                If Not saveStandCode.Trim = "" Then
                    sb.Append("}" & sCrLf)
                End If
                sb.Append("if (trim(newStandCode2) == """ & sStandCode & """) {" & sCrLf)
                sb.Append("removeAllOptions(ddl2);" & sCrLf)
                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) Then
                    sb.Append("addOption(ddl2, """ & "*ANY" & """, """ & ucr.Content("AnyAreaText", _langCode, True) & """);" & sCrLf)
                End If
            End If
            sb.Append("addOption(ddl2, """ & sAreaCode & """, """ & sAreaDesc & """);" & sCrLf)
            saveStandCode = sStandCode
        Next
        sb.Append("}" & sCrLf)
        sb.Append("}" & sCrLf)


        sStandCode = ""
        sStandDesc = ""
        sAreaCode = ""
        sAreaDesc = ""
        saveStandCode = ""


        'DDLs 3
        '----------------------------------------------------
        sb.Append("function LoadAreasForDDL3(thisform){" & sCrLf)
        sb.Append("var ddl3 = document.getElementById('" & prefArea3DDL.ClientID & "');" & sCrLf)
        sb.Append("var newStandCode3 = document.getElementById('" & prefStand3DDL.ClientID & "').value;" & sCrLf)

        sb.Append("if (trim(newStandCode3) == """") {" & sCrLf)
        sb.Append("removeAllOptions(ddl3);" & sCrLf)
        sb.Append("addOption(ddl3, """", """");" & sCrLf)
        sb.Append("}" & sCrLf)
        For Each dr In dtProduct.Rows
            sStandCode = dr("StandCode").ToString.Trim
            sStandDesc = dr("StandDescription").ToString.Trim
            sAreaCode = dr("AreaCode").ToString
            sAreaDesc = dr("AreaDescription").ToString.Trim
            If Not (sStandCode = saveStandCode) Then
                If Not saveStandCode.Trim = "" Then
                    sb.Append("}" & sCrLf)
                End If
                sb.Append("if (trim(newStandCode3) == """ & sStandCode & """) {" & sCrLf)
                sb.Append("removeAllOptions(ddl3);" & sCrLf)
                If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("AreaSelection_IsMandatory")) Then
                    sb.Append("addOption(ddl3, """ & "*ANY" & """, """ & ucr.Content("AnyAreaText", _langCode, True) & """);" & sCrLf)
                End If
            End If
            sb.Append("addOption(ddl3, """ & sAreaCode & """, """ & sAreaDesc & """);" & sCrLf)
            saveStandCode = sStandCode
        Next
        sb.Append("}" & sCrLf)
        sb.Append("}" & sCrLf)

        sb.Append("function PopulateDDL1Hidden(){" & sCrLf)
        sb.Append("document.getElementById('" & hfArea1Selected.ClientID & "').value = document.getElementById('" & prefArea1DDL.ClientID & "').value;" & sCrLf)
        sb.Append("}" & sCrLf)

        sb.Append("function PopulateDDL2Hidden(){" & sCrLf)
        sb.Append("document.getElementById('" & hfArea2Selected.ClientID & "').value = document.getElementById('" & prefArea2DDL.ClientID & "').value;" & sCrLf)
        sb.Append("}" & sCrLf)

        sb.Append("function PopulateDDL3Hidden(){" & sCrLf)
        sb.Append("document.getElementById('" & hfArea3Selected.ClientID & "').value = document.getElementById('" & prefArea3DDL.ClientID & "').value;" & sCrLf)
        sb.Append("}" & sCrLf)

        sb.Append("function trim(s) { " & sCrLf & "var r=/\b(.*)\b/.exec(s); " & sCrLf & "return (r==null)?"""":r[1]; " & sCrLf & "}" & sCrLf)

        sb.Append("</script>" & sCrLf)

        Return sb.ToString
    End Function


    Protected Sub BackButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BackButton.Click
        quantityDDL.SelectedIndex = 0
        quantityDDL.Enabled = True
        contentLabel.Text = ucr.Content("IntroText", _langCode, True)
        waitListDetailsPanel.Visible = False
    End Sub

    Protected Sub standLoadButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles standLoadButton1.Click
        Dim ds As DataSet = GetStandsAndAreasDescriptions()
        StandDropDownFill(prefStand1DDL.SelectedValue, 1, ds)
    End Sub

    Protected Sub standLoadButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles standLoadButton2.Click
        Dim ds As DataSet = GetStandsAndAreasDescriptions()
        StandDropDownFill(prefStand2DDL.SelectedValue, 2, ds)
    End Sub

    Protected Sub standLoadButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles standLoadButton3.Click
        Dim ds As DataSet = GetStandsAndAreasDescriptions()
        StandDropDownFill(prefStand3DDL.SelectedValue, 3, ds)
    End Sub
End Class

Public Class DataSetHelperST
    Public ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public ds As DataSet
    Public Sub New(ByRef DataSet As DataSet)
        ds = DataSet
    End Sub
    Public Sub New()
        ds = Nothing
    End Sub
    Private Function ColumnEqual(ByVal A As Object, ByVal B As Object) As Boolean

        ' Compares two values to see if they are equal. Also compares DBNULL.Value.
        ' Note: If your DataTable contains object fields, then you must extend this
        ' function to handle them in a meaningful way if you intend to group on them.

        If A.Equals(DBNull.Value) AndAlso B.Equals(DBNull.Value) Then
            Return True
            '  both are DBNull.Value
        End If
        If A.Equals(DBNull.Value) OrElse B.Equals(DBNull.Value) Then
            Return False
            '  only one is DBNull.Value
        End If
        Return (A.Equals(B))
        ' value type standard comparison
    End Function
    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable, ByVal FieldName As String, ByVal FieldName2 As String) As DataTable
        Dim dt As New DataTable(TableName)

        dt.Columns.Add(FieldName, SourceTable.Columns(FieldName).DataType)
        dt.Columns.Add(FieldName2, SourceTable.Columns(FieldName2).DataType)

        Dim LastValue As Object = Nothing
        For Each dr As DataRow In SourceTable.[Select]("", FieldName)
            If LastValue Is Nothing OrElse Not (ColumnEqual(LastValue, dr(FieldName))) Then
                LastValue = dr(FieldName)
                dt.Rows.Add(LastValue, dr(FieldName2))
            End If
        Next
        If ds IsNot Nothing Then
            ds.Tables.Add(dt)
        End If
        Return dt
    End Function

End Class
