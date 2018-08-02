Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common
Imports System.Web
Imports System.Linq

Partial Class Package_ComponentGroupDisplaySettings
    Inherits PageControlBase

#Region "Private Properties"
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()
    Private Shared _partner As String = String.Empty
    Private Shared _businessUnit As String = String.Empty
    Private Property LiveOrTest() As String
    Private errMsg As TalentErrorMessages
    Private dsComponentGroupdetails As New DataSet
    Private _userControlResource As New UserControlResource
    Private ComponentGroupId As String
    Private wfr As New WebFormResource
#End Region

#Region "Constants"
    Const PAGECODE As String = "*ALL"
    Const PAGECODE1 As String = "ComponentGroupDisplaySettings.aspx"
    Const PAGECODE2 As String = "ComponentSelection.aspx"
    Const STARTSYMBOL As String = "*"
    Const EDITCOMMANDNAME As String = "Edit"
    Const SETTINGCOMMANDNAME As String = "Setting"
    Const DELETECOMMANDNAME As String = "Delete"
    Const ALL As String = "*ALL"
    Const CACHINGDEPENDENCYNAMETOCLEAR = "Packages"
    Const CONTROLCODE As String = "TravelAndAccommodationGroup.ascx"
    Const PARTNERCODE = GlobalConstants.STARALLPARTNER
    Const HEADERTEXT As String = "_HeaderText"
#End Region

#Region "Public Properties"

    Public ReadOnly Property BusinessUnit() As String
        Get
            Return Utilities.CheckForDBNull_String(Request.QueryString("BU"))
        End Get
    End Property
    Public ReadOnly Property Partner() As String
        Get
            Return Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
        End Get
    End Property

    Public Property Caching As Boolean = False


#End Region

#Region "Protected Methods"
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'This web form resource is to insert the values (Component Header) here and retreive the values on componentselection page in ebusiness
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = PARTNERCODE
            .PageCode = PAGECODE2
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        'This user control resource is to insert the values here and retreive the values on TravelAndAccommodation control in ebusiness
        With _userControlResource
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PageCode = PAGECODE
            .KeyCode = CONTROLCODE
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = CONTROLCODE
        End With
        'This web form resource is to get the control texts from the database for this page.
        With wfr
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = PAGECODE1
            .KeyCode = PAGECODE1
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        blErrorMessages.Items.Clear()
        hlnkComponent.NavigateUrl = String.Format("~/Package/ComponentGroupList.aspx?BU={0}&Partner={1}", BusinessUnit, Partner)
        hlnkComponent.Text = wfr.Content("ComponentGroupListPage", _languageCode, True)
        errMsg = New TalentErrorMessages(_languageCode, GlobalConstants.MAINTENANCEBUSINESSUNIT, GlobalConstants.STARALLPARTNER, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
        Dim querystrings As String = "?" & GetQuerystrings()
        Dim okToRetrieveVouchers As Boolean = False
        If Request.QueryString("BU") IsNot Nothing Then
            _businessUnit = Utilities.CheckForDBNull_String(Request.QueryString("BU"))
            If Request.QueryString("Partner") IsNot Nothing Then
                _partner = Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
                okToRetrieveVouchers = True
            End If
        End If
        'caching should not be enabled as this page always post backs for an update and the change should be visible to the user.
        Caching = False
        ComponentGroupId = Request.QueryString("ComponentGroupId")
        If ComponentGroupId Is Nothing Then
            ComponentGroupId = Session("ComponentGroupId")
        End If

        If Not Page.IsPostBack Then
            GetComponentGroupDetails()
            SetPageText()
            BindComponentDropDown()
        End If
        ShowHideFieldsPanel()
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)


        If TypeOf sender Is Label Then
            DirectCast(sender, Label).Text = Utilities.CheckForDBNull_String(wfr.Content(DirectCast(sender, Label).ID, _languageCode, True))
        ElseIf TypeOf sender Is Button Then
            DirectCast(sender, Button).Text = Utilities.CheckForDBNull_String(wfr.Content(DirectCast(sender, Button).ID, _languageCode, True))
        ElseIf TypeOf sender Is Literal Then
            DirectCast(sender, Literal).Text = Utilities.CheckForDBNull_String(wfr.Content(DirectCast(sender, Literal).ID, _languageCode, True))
        End If
    End Sub

    Protected Sub ddlComponent_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlComponent.SelectedIndexChanged
        Dim ComponentId As Long = ddlComponent.SelectedValue
        RetreiveComponentFieldsValue(ComponentId)
    End Sub


    Protected Sub btnUpdateComponent_Click(sender As Object, e As System.EventArgs) Handles btnUpdateComponent.Click
        Dim errObj As ErrorObj
        Try
            Dim PrecedingAttributeName As String = ComponentGroupId + "_" + ddlComponent.SelectedValue
            _wfrPage.BusinessUnit = BUPL.BusinessUnit
            _userControlResource.BusinessUnit = BUPL.BusinessUnit

            errObj = TDataObjects.ControlSettings.TblControlAttribute.InsertOrUpdate(BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_ComponentHeader", Utilities.CheckForDBNull_String(txtComponentHeader.Text), String.Empty)
            errObj = TDataObjects.ControlSettings.TblControlAttribute.InsertOrUpdate(BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_MoreInfoUrl", Utilities.CheckForDBNull_String(txtComponentMoreInfoLink.Text), String.Empty)
            errObj = TDataObjects.ControlSettings.TblControlAttribute.InsertOrUpdate(BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_Image", Utilities.CheckForDBNull_String(txtComponentImage.Text), String.Empty)

            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_GroupText", Utilities.CheckForDBNull_String(txtComponentText.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_RadioButtonText", Utilities.CheckForDBNull_String(txtComponentRadioButtonText.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_MoreInfoButtonText", Utilities.CheckForDBNull_String(txtComponentMoreInfoButtonText.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_DestinationText", Utilities.CheckForDBNull_String(txtComponentDestination.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_ArrivalDateText", Utilities.CheckForDBNull_String(txtComponentArrivalDate.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_DepartureDateText", Utilities.CheckForDBNull_String(txtComponentDepartureDate.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_CarsText", Utilities.CheckForDBNull_String(txtComponentCarsText.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_RoomType1Text", Utilities.CheckForDBNull_String(txtComponentRoomType1.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_RoomType2Text", Utilities.CheckForDBNull_String(txtComponentRoomType2.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_Extra1Text", Utilities.CheckForDBNull_String(txtComponentExtra1.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_Extra2Text", Utilities.CheckForDBNull_String(txtComponentExtra2.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_Extra3Text", Utilities.CheckForDBNull_String(txtComponentExtra3.Text))
            errObj = TDataObjects.ControlSettings.TblControlTextLang.InsertOrUpdate(BUPL.Language, BUPL.BusinessUnit, PARTNERCODE, PAGECODE, CONTROLCODE, PrecedingAttributeName + "_Extra4Text", Utilities.CheckForDBNull_String(txtComponentExtra4.Text))
            RetreiveComponentFieldsValue(ddlComponent.SelectedValue)
        Catch ex As Exception
            ShowMessage("InsertFailed")
        End Try


    End Sub



    Protected Sub btnUpdateHeader_Click(sender As Object, e As System.EventArgs) Handles btnUpdateHeader.Click
        Dim errObj1 As ErrorObj
        Dim PrecedingAttributeName As String = ComponentGroupId
        errObj1 = TDataObjects.PageSettings.TblPageControl.InsertOrUpdate(BUPL.BusinessUnit, PARTNERCODE, PAGECODE2, PrecedingAttributeName + "_HeaderText", BUPL.Language, txtHeader.Text)
        If (errObj1.HasError) Then
            ShowMessage("ProblemUpdatingComponent")
        Else
        End If
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As System.EventArgs) Handles Me.LoadComplete
        Try
            If Not Page.IsPostBack Then
                If (Request.QueryString("ImageURL") <> Nothing) Then
                    BUPL.BusinessUnit = Session("BusinessUnit")
                    RestoreValues()
                ElseIf plhFields.Visible Then
                    RetreiveComponentFieldsValue(ddlComponent.SelectedValue)
                End If
                GetComponenGrouptHeaderText()
            ElseIf BUPL.PostBackByUserControl AndAlso plhFields.Visible Then
                GetComponenGrouptHeaderText()
                RetreiveComponentFieldsValue(ddlComponent.SelectedValue)
            End If
        Catch ex As Exception
            ShowMessage("FailedToRetrieveValues")
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As System.EventArgs) Handles btnCancel.Click
        Response.Redirect(String.Format("~/Package/ComponentGroupList.aspx?BU={0}&Partner={1}", BusinessUnit, Partner))
    End Sub

    Protected Sub btnSelectImage_Click(sender As Object, e As System.EventArgs) Handles btnSelectImage.Click
        RedirectToBrowseImage()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub RetreiveComponentFieldsValue(ByVal ComponentId As Long)
        Dim PrecedingAttributeName As String = ComponentGroupId + "_" + ComponentId.ToString()
        Try
            _userControlResource.BusinessUnit = BUPL.BusinessUnit
            _userControlResource.PartnerCode = PARTNERCODE
            _userControlResource.PageCode = PAGECODE
            _userControlResource.KeyCode = CONTROLCODE
            _userControlResource.ExactValuesOnly = True

            txtComponentHeader.Text = Utilities.CheckForDBNull_String(_userControlResource.Attribute(PrecedingAttributeName + "_ComponentHeader", Caching))
            txtComponentImage.Text = Utilities.CheckForDBNull_String(_userControlResource.Attribute(PrecedingAttributeName + "_Image", Caching))
            txtComponentMoreInfoLink.Text = Utilities.CheckForDBNull_String(_userControlResource.Attribute(PrecedingAttributeName + "_MoreInfoUrl", Caching))

            txtComponentText.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_GroupText", BUPL.Language, Caching))
            txtComponentRadioButtonText.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_RadioButtonText", BUPL.Language, Caching))
            txtComponentMoreInfoButtonText.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_MoreInfoButtonText", BUPL.Language, Caching))
            txtComponentDestination.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_DestinationText", BUPL.Language, Caching))
            txtComponentArrivalDate.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_ArrivalDateText", BUPL.Language, Caching))
            txtComponentDepartureDate.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_DepartureDateText", BUPL.Language, Caching))
            txtComponentCarsText.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_CarsText", BUPL.Language, Caching))
            txtComponentRoomType1.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_RoomType1Text", BUPL.Language, Caching))
            txtComponentRoomType2.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_RoomType2Text", BUPL.Language, Caching))
            txtComponentExtra1.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_Extra1Text", BUPL.Language, Caching))
            txtComponentExtra2.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_Extra2Text", BUPL.Language, Caching))
            txtComponentExtra3.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_Extra3Text", BUPL.Language, Caching))
            txtComponentExtra4.Text = Utilities.CheckForDBNull_String(_userControlResource.Content(PrecedingAttributeName + "_Extra4Text", BUPL.Language, Caching))
        Catch ex As Exception
            ShowMessage("RetrieveFailed")
        End Try

    End Sub

    Private Sub RestoreValues()
        'BUPL.BusinessUnit = Session("BusinessUnit")
        Try
            BUPL.BindLanguageDropDown()
            BUPL.Language = Session("Language")
            txtComponentHeader.Text = Session("txtComponentHeader")
            txtComponentText.Text = Session("txtComponentText")
            txtComponentMoreInfoLink.Text = Session("txtComponentMoreInfoLink")
            txtComponentRadioButtonText.Text = Session("txtComponentRadioButtonText")
            txtComponentMoreInfoButtonText.Text = Session("txtComponentMoreInfoButtonText")
            txtComponentDestination.Text = Session("txtComponentDestination")
            txtComponentArrivalDate.Text = Session("txtComponentArrivalDate")
            txtComponentDepartureDate.Text = Session("txtComponentDepartureDate")
            txtComponentCarsText.Text = Session("txtComponentCarsText")
            txtComponentRoomType1.Text = Session("txtComponentRoomType1")
            txtComponentRoomType2.Text = Session("txtComponentRoomType2")
            txtComponentExtra1.Text = Session("txtComponentExtra1")
            txtComponentExtra2.Text = Session("txtComponentExtra2")
            txtComponentExtra3.Text = Session("txtComponentExtra3")
            txtComponentExtra4.Text = Session("txtComponentExtra4")
            ddlComponent.SelectedValue = Session("Component")
            txtComponentMoreInfoLink.Text = Session("MoreInfoLink")
            txtComponentImage.Text = Request.QueryString("ImageURL")

        Catch ex As Exception
            ShowMessage("RestoreFailed")
        End Try
    End Sub

    Private Sub RedirectToBrowseImage()
        'store the values in session so once we return to this page, the value in controls could be filled.
        Session("BusinessUnit") = BUPL.BusinessUnit
        Session("Language") = BUPL.Language
        Session("txtComponentHeader") = txtComponentHeader.Text
        Session("txtComponentText") = txtComponentText.Text
        Session("txtComponentMoreInfoLink") = txtComponentMoreInfoLink.Text
        Session("txtComponentRadioButtonText") = txtComponentRadioButtonText.Text
        Session("txtComponentMoreInfoButtonText") = txtComponentMoreInfoButtonText.Text
        Session("txtComponentDestination") = txtComponentDestination.Text
        Session("txtComponentArrivalDate") = txtComponentArrivalDate.Text
        Session("txtComponentDepartureDate") = txtComponentDepartureDate.Text
        Session("txtComponentCarsText") = txtComponentCarsText.Text
        Session("txtComponentRoomType1") = txtComponentRoomType1.Text
        Session("txtComponentRoomType2") = txtComponentRoomType2.Text
        Session("txtComponentExtra1") = txtComponentExtra1.Text
        Session("txtComponentExtra2") = txtComponentExtra2.Text
        Session("txtComponentExtra3") = txtComponentExtra3.Text
        Session("txtComponentExtra4") = txtComponentExtra4.Text
        Session("Component") = ddlComponent.SelectedValue
        Session("ComponentGroupId") = ComponentGroupId
        Session("MoreInfoLink") = txtComponentMoreInfoLink.Text

        Dim browseImageUrl As New StringBuilder
        browseImageUrl.Append("~/Images/BrowseImages.aspx?")
        browseImageUrl.Append("BU=")
        browseImageUrl.Append(_businessUnit)
        browseImageUrl.Append("&PARTNER=")
        browseImageUrl.Append(_partner)

        Dim returnURL As String = String.Format("~/Package/ComponentGroupDisplaySettings.aspx?BU={0}", _businessUnit)
        browseImageUrl.Append("&ReturnURL=")
        browseImageUrl.Append(returnURL)
        browseImageUrl.Append("&ImageKey=ComponentGroupId=")
        browseImageUrl.Append(ComponentGroupId.ToString())

        Response.Redirect(browseImageUrl.ToString)
    End Sub

    Private Sub GetComponenGrouptHeaderText()
        Dim PrecedingAttributeName As String = ComponentGroupId
        _wfrPage.BusinessUnit = BUPL.BusinessUnit
        txtHeader.Text = Utilities.CheckForDBNull_String(_wfrPage.Content(PrecedingAttributeName + "_HeaderText", BUPL.Language, Caching))
    End Sub

    ''' <summary>
    ''' Retreives the BU and Partner values from the querystring.
    ''' </summary>
    ''' <returns>String containing BU and PARTNER values</returns>
    ''' <remarks></remarks>
    Private Function GetQuerystrings() As String
        Dim linkQuerystring As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString("BU")) Then
            linkQuerystring = "BU=" & Request.QueryString("BU").Trim.ToUpper
        Else
            linkQuerystring = "BU=*ALL"
        End If
        If Not String.IsNullOrWhiteSpace(Request.QueryString("Partner")) Then
            linkQuerystring = linkQuerystring & "&Partner=" & Request.QueryString("Partner").Trim.ToUpper
        Else
            linkQuerystring = linkQuerystring & "&Partner=*ALL"
        End If
        Return linkQuerystring
    End Function

    ''' <summary>
    ''' Set the text properties of the page controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetPageText()
        Page.Title = GetTextFromDatabase("PageTitle")
        lblComponentGroupDescription.Text = GetComponentGroupDescription()

    End Sub

    Private Function GetTextFromDatabase(ByVal sKey As String) As String
        Return wfr.Content(sKey, _languageCode, True)
    End Function

    Private Function GetAttributeFromDatabase(ByVal sKey As String) As String
        Return wfr.Attribute(sKey)
    End Function

    Private Function GetComponentGroupDescription() As String
        Dim strCompGroup As New StringBuilder
        strCompGroup.Append(wfr.Content("ComponentGroupText", _languageCode, True))

        Dim compGroupdescription = (From r In dsComponentGroupdetails.Tables("ComponentGroupDetails").AsEnumerable
                                   Select r.Field(Of String)("GroupDescription")).FirstOrDefault()

        strCompGroup.Append(compGroupdescription)
        Return strCompGroup.ToString()
    End Function

    Private Sub GetComponentGroupDetails()
        Dim result As Boolean = True
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj

        talPackage.Settings = Utilities.GetSettingsObject
        talPackage.Settings.Cacheing = Caching
        'talPackage.Settings.CacheTimeMinutes = CType(_wfrPage.Attribute("ComponentCacheTimeMinutes"), Integer)
        talPackage.DePackages.ComponentGroupID = ComponentGroupId

        err = talPackage.GetComponentGroupDetails()
        dsComponentGroupdetails = talPackage.ResultDataSet

        If err.HasError OrElse dsComponentGroupdetails.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            ShowMessage("FailedGettingComponentDetails")
        End If

    End Sub


    Private Sub BindComponentDropDown()

        Dim dicComponentDetails As New Dictionary(Of String, String)
        Dim componentDetails = (From r In dsComponentGroupdetails.Tables("ComponentDetails").AsEnumerable
                               Select New With
                                        {
                                        .ComponentId = r.Field(Of Long)("ComponentId"),
                                        .ComponentDescription = r.Field(Of String)("ComponentDescription")
                                        }).ToList()


        For Each c As Object In componentDetails
            dicComponentDetails.Add(c.ComponentId.ToString(), c.ComponentDescription)
        Next

        ddlComponent.DataSource = dicComponentDetails
        ddlComponent.DataTextField = "Value"
        ddlComponent.DataValueField = "Key"
        ddlComponent.DataBind()

    End Sub


    ''' <summary>
    ''' Shows Generic errors/messages on the page.
    ''' </summary>
    ''' <param name="ErrorKey">Key to retreive the error text.</param>
    ''' <remarks></remarks>
    Private Sub ShowMessage(ByVal ErrorKey As String)
        plhErrorList.Visible = True
        blErrorMessages.Items.Clear()
        blErrorMessages.Items.Add(GetTextFromDatabase(ErrorKey))
    End Sub

    ''' <summary>
    ''' Shows Specific errors/messages on the page.
    ''' </summary>
    ''' <param name="ErrorKey">Key to retreive the error text.</param>
    ''' <remarks></remarks>
    Private Sub ShowError(ByVal ErrorKey As String)
        Dim talentErrorMsg As TalentErrorMessage = errMsg.GetErrorMessage(ErrorKey)
        plhErrorList.Visible = True
        blErrorMessages.Items.Clear()
        blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
    End Sub

    ''' <summary>
    ''' The component specific configuration fields are visible only when there is a component in the group.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ShowHideFieldsPanel()
        If ddlComponent.Items.Count > 0 Then
            plhFields.Visible = True
        Else
            ShowMessage("NoComponentsInGroup")
            plhFields.Visible = False
        End If
    End Sub
#End Region

End Class
