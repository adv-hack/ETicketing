Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce

Partial Class UserControls_FriendsAndFamilyDetails
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ds As DataSet = Nothing
    Private _languageCode As String = Nothing
    Private _ucr As UserControlResource = Nothing
    Private _customer As TalentCustomer = Nothing
    Private _settings As DESettings = Nothing
    Private _bDeleteError As Boolean = Nothing
    Private _err As ErrorObj = Nothing

#End Region


#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ds = New DataSet
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _ucr = New UserControlResource
        _customer = New TalentCustomer
        _err = New ErrorObj
        _settings = Talent.eCommerce.Utilities.GetSettingsObject()
        _customer.Settings() = _settings
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FriendsAndFamilyDetails.ascx"
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim err As New Talent.Common.ErrorObj
        Dim ds0 As New DataTable
        Dim ds1 As New DataTable
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        With _customer
            .DeV11 = deCustV11
            ' Set the settings data entity. 
            .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            _settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath

            'Set the customer values
            deCustV1.CustomerNumber = Profile.User.Details.LoginID.ToString()
            deCustV1.Source = "W"
            .ResultDataSet = Nothing
            err = .CustomerAssociations
        End With

        ' Initialise
        If _bDeleteError <> True Then
            ltlErrorLabel.Text = String.Empty
        End If
        _bDeleteError = False

        'Did the call complete successfully
        If err.HasError Or _customer.ResultDataSet Is Nothing Then
            showError("XX")
        Else
            'Ticketing error
            ds0 = _customer.ResultDataSet.Tables(0)
            If ds0.Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                'Display the message
                If ds0.Rows(0).Item("ReturnCode") = "NL" Then
                    ltlErrorLabel.Text = _ucr.Content("NoRecordsToDisplay", _languageCode, True)
                Else
                    showError(ds0.Rows(0).Item("ReturnCode"))
                End If
            Else
                'Set the data source to the returned data set
                ds1 = _customer.ResultDataSet.Tables(1)
                rptFriendsAndFamily.DataSource = ds1
            End If
        End If
        rptFriendsAndFamily.DataBind()
        plhErrorMessage.Visible = (ltlErrorLabel.Text.Length > 0)
        If rptFriendsAndFamily.Items.Count > 0 Then
            plhFFList.Visible = True
            plhNoFFInList.Visible = False
        Else
            plhFFList.Visible = False
            plhNoFFInList.Visible = True
            ltlNoFFInList.Text = _ucr.Content("NoFFToDisplay", _languageCode, True)
        End If
    End Sub

    Protected Sub rptFriendsAndFamily_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptFriendsAndFamily.ItemCommand
        Dim ltlCustomerNumber As Literal = CType(e.Item.FindControl("ltlCustomerNumber"), Literal)
        DeleteCustomerAssociation(Profile.User.Details.LoginID.ToString(), ltlCustomerNumber.Text)
    End Sub

    Protected Sub rptFriendsAndFamily_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptFriendsAndFamily.ItemDataBound
        If e.Item.ItemType = ListItemType.Header OrElse e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim ltlCustomerNumber As Literal = CType(e.Item.FindControl("ltlCustomerNumber"), Literal)
                ltlCustomerNumber.Text = e.Item.DataItem("AssociatedCustomerNumber").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)
            End If
            Dim plhPostCodeColumn As PlaceHolder = CType(e.Item.FindControl("plhPostCodeColumn"), PlaceHolder)
            If plhPostCodeColumn IsNot Nothing Then
                plhPostCodeColumn.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowPostCodeColumn"))
            End If
        End If
    End Sub
    Private Sub DeleteCustomerAssociation(ByVal customerNumber As String, ByVal FriendsAndFamilyID As String)
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        With _customer
            .DeV11 = deCustV11
            ' Set the settings data entity. 
            .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

            'Set the customer values
            deCustV1.FriendsAndFamilyId = FriendsAndFamilyID
            deCustV1.CustomerNumber = customerNumber
            deCustV1.Source = "W"
            deCustV1.FriendsAndFamilyMode = "D"
            .ResultDataSet = Nothing

            'Process
            _err = .DeleteCustomerAssociation

            'Did the call complete successfully
            If _err.HasError Or _customer.ResultDataSet Is Nothing Then
                showError("XX")
            Else
                'API error
                If _customer.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    showError(_customer.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
                Else
                    'Show success Message here
                    _customer.CustomerAssociationsClearSession()
                End If
            End If
        End With
    End Sub
#End Region

#Region "Public Methods"

    Public Function getText(ByVal ContentName As String) As String
        Return _ucr.Content(ContentName, _languageCode, True)
    End Function

#End Region

#Region "Private Methods"

    Private Sub showError(ByVal errCode As String)
        ' Find the errorLabel.
        Dim conControl As Control
        For Each conControl In Parent.Controls
            Select Case conControl.ID
                Case Is = "ErrorLabel"
                    ltlErrorLabel = CType(conControl, Literal)
                    Exit For
            End Select
        Next

        ' Update the errorLabel
        If errCode.Trim = "" Then
            ltlErrorLabel.Text = String.Empty
        Else
            ltlErrorLabel.Text = Talent.Common.Utilities.getErrorDescription(_ucr, _languageCode, errCode, True)
        End If
    End Sub

#End Region

End Class