Imports MaintenancePortalDataSetTableAdapters
Imports Talent.Common

Partial Class ModuleOverview
    Inherits System.Web.UI.Page
    Dim objBUTableAdapter As New BUSINESS_UNIT_TableAdapter
    Dim objPTableAdapter As New PARTNER_TableAdapter
    Dim objPriceListTableAdapter As New tbl_price_list_headerTableAdapter
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _TDataObjects As Talent.Common.TalentDataObjects

    Protected Property TDataObjects() As Talent.Common.TalentDataObjects
        Get
            Return _TDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentDataObjects)
            _TDataObjects = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ModuleOverview.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "ModuleOverview.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        If _TDataObjects Is Nothing Then
            _TDataObjects = New Talent.Common.TalentDataObjects()
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            _TDataObjects.Settings = settings
        End If
        ltlHomeLink.Text = wfrPage.Content("HomeLink", _languageCode, True)
    End Sub
    Public Sub setLabel()
        titleLabel.Text = wfrPage.Content("titleLabel", _languageCode, True)
        instructionsLabel.Text = wfrPage.Content("instructionsLabel", _languageCode, True)
        businessunitLabel.Text = wfrPage.Content("businessunitLabel", _languageCode, True)
        partnerGroupLabel.Text = wfrPage.Content("partnerGroupLabel", _languageCode, True)
        partnerLabel.Text = wfrPage.Content("partnerLabel", _languageCode, True)
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If String.IsNullOrEmpty(Request.QueryString("BusinessUnit")) Then
            Response.Redirect("MaintenancePortal.aspx")
        End If

        If String.IsNullOrEmpty(Request.QueryString("Partner")) Then
            Response.Redirect("MaintenancePortal.aspx")
        End If

        If String.IsNullOrEmpty(Request.QueryString("usage")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If

        If Request.QueryString("usage") = "promotion" Then
            PromotionsOverview1.Visible = True
        Else
            PromotionsOverview1.Visible = False
        End If

        If IsPostBack = False Then
            setLabel()

            fillBusinessUnitDropDown()
            businessunitDropDownList.SelectedValue = Request.QueryString("BusinessUnit")
            fillPartnerGroupDropDown()
            fillPartner()
            partnerDropDownList.SelectedValue = Request.QueryString("Partner")
            'Session("BusinessUnit") = Request.QueryString("BusinessUnit")
            'Session("Partner") = Request.QueryString("Partner")
            PromotionsOverview1.DataFill()

        End If

    End Sub
    Public Sub fillPartner()
        Dim showPartnerDropDownList As String = _TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(wfrPage.BusinessUnit, "SHOW_PARTNER_DROPDOWNLIST", True)
        Try
            If CType(showPartnerDropDownList, Boolean) = False Then
                Dim starAllItem As New ListItem
                starAllItem.Text = "*ALL"
                starAllItem.Value = "*ALL"
                partnerDropDownList.Items.Clear()
                partnerDropDownList.Items.Add(starAllItem)
                partnerDropDownList.SelectedIndex = 0
            Else
                partnerDropDownList.DataSource = objPTableAdapter.GetDatadistinctPartner(businessunitDropDownList.SelectedValue.ToString)
                partnerDropDownList.DataTextField = "PARTNER"
                partnerDropDownList.DataValueField = "PARTNER"
                partnerDropDownList.DataBind()

                For Each item As ListItem In partnerDropDownList.Items
                    If UCase(item.Value) = UCase(Request("Partner")) Then
                        item.Selected = True
                    Else
                        item.Selected = False
                    End If
                Next
            End If
        Catch ex As Exception
            partnerDropDownList.DataSource = objPTableAdapter.GetDatadistinctPartner(businessunitDropDownList.SelectedValue.ToString)
            partnerDropDownList.DataTextField = "PARTNER"
            partnerDropDownList.DataValueField = "PARTNER"
            partnerDropDownList.DataBind()

            For Each item As ListItem In partnerDropDownList.Items
                If UCase(item.Value) = UCase(Request("Partner")) Then
                    item.Selected = True
                Else
                    item.Selected = False
                End If
            Next
        End Try
    End Sub
    Public Sub fillBusinessUnitDropDown()
        businessunitDropDownList.DataSource = objBUTableAdapter.GetDatadistinctBusinessUnit()
        businessunitDropDownList.DataTextField = "BUSINESS_UNIT"
        businessunitDropDownList.DataValueField = "BUSINESS_UNIT"
        businessunitDropDownList.DataBind()

        For Each item As ListItem In businessunitDropDownList.Items
            If UCase(item.Value) = UCase(Request("BusinessUnit")) Then
                item.Selected = True
            Else
                item.Selected = False
            End If
        Next

    End Sub
    Public Sub fillPartnerGroupDropDown()
        If UCase(Request.QueryString("BusinessUnit")) = UCase(Talent.Common.Utilities.GetAllString) Then
            partnerGroupDropDownList.Enabled = False
        Else
            Dim partnerGroupType As String = "1"
            ' Get Partner Group Type from ebusinessModule defaults using selected BU

            ' Populate drop type according to type
            Select Case partnerGroupType
                Case Is = "1"
                    '---------------------
                    ' Type 1 = Price lists
                    '---------------------
                    Dim showPartnerDropDownList As String = _TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(wfrPage.BusinessUnit, "SHOW_PARTNER_DROPDOWNLIST", True)
                    Try
                        If CType(showPartnerDropDownList, Boolean) = False Then
                            Dim starAllItem As New ListItem
                            starAllItem.Text = "*ALL"
                            starAllItem.Value = "*ALL"
                            partnerGroupDropDownList.Items.Clear()
                            partnerGroupDropDownList.Items.Add(starAllItem)
                            partnerGroupDropDownList.SelectedIndex = 0
                        Else
                            partnerGroupDropDownList.DataSource = objPriceListTableAdapter.GetDistinctPricelistHeaders
                            partnerGroupDropDownList.DataTextField = "PRICE_LIST"
                            partnerGroupDropDownList.DataValueField = "PRICE_LIST"
                            partnerGroupDropDownList.DataBind()
                            partnerGroupDropDownList.Items.Insert(0, Talent.Common.Utilities.GetAllString)

                            For Each item As ListItem In partnerGroupDropDownList.Items
                                If UCase(item.Value) = UCase(Request("PartnerGroup")) Then
                                    item.Selected = True
                                Else
                                    item.Selected = False
                                End If
                            Next
                        End If
                    Catch ex As Exception
                        partnerGroupDropDownList.DataSource = objPriceListTableAdapter.GetDistinctPricelistHeaders
                        partnerGroupDropDownList.DataTextField = "PRICE_LIST"
                        partnerGroupDropDownList.DataValueField = "PRICE_LIST"
                        partnerGroupDropDownList.DataBind()
                        partnerGroupDropDownList.Items.Insert(0, Talent.Common.Utilities.GetAllString)

                        For Each item As ListItem In partnerGroupDropDownList.Items
                            If UCase(item.Value) = UCase(Request("PartnerGroup")) Then
                                item.Selected = True
                            Else
                                item.Selected = False
                            End If
                        Next
                    End Try
                Case Else
            End Select
        End If
    End Sub


    Protected Sub businessunitDropDownList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles businessunitDropDownList.SelectedIndexChanged

        'Session("BusinessUnit") = businessunitDropDownList.SelectedValue
        'Session("Partner") = partnerDropDownList.SelectedValue
        fillPartner()
        'PromotionsOverview1.DataFill()

        If businessunitDropDownList.SelectedItem.ToString = "*ALL" Then
            partnerGroupDropDownList.Enabled = False
        End If


        Response.Redirect("ModuleOverview.aspx?usage=promotion&Partner=" + partnerDropDownList.SelectedValue + "&BusinessUnit=" + businessunitDropDownList.SelectedValue)

    End Sub

    Protected Sub partnerDropDownList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles partnerDropDownList.SelectedIndexChanged

        'Session("BusinessUnit") = businessunitDropDownList.SelectedValue
        'Session("Partner") = partnerDropDownList.SelectedValue

        'PromotionsOverview1.DataFill()

        'Request.QueryString.Set("BusinessUnit", businessunitDropDownList.SelectedValue.ToString)
        'Request.QueryString.Set("Partner", partnerDropDownList.SelectedValue.ToString)


        Response.Redirect("ModuleOverview.aspx?usage=promotion&Partner=" + partnerDropDownList.SelectedValue + "&BusinessUnit=" + businessunitDropDownList.SelectedValue & "&PartnerGroup=" & partnerGroupDropDownList.SelectedValue)

    End Sub

    Protected Sub partnerGroupDropDownList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles partnerGroupDropDownList.SelectedIndexChanged
        Response.Redirect("ModuleOverview.aspx?usage=promotion&Partner=" + partnerDropDownList.SelectedValue + "&BusinessUnit=" + businessunitDropDownList.SelectedValue & "&PartnerGroup=" & partnerGroupDropDownList.SelectedValue)
    End Sub
End Class
