Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities

Partial Class UserControls_OrderTemplatesHeader
    Inherits ControlBase

#Region "Class Level Fields"

    Private _usage As String
    Private _languageCode As String = TCUtilities.GetDefaultLanguage
    Private _ucr As New UserControlResource
    Private _pds As New PagedDataSource
    Private _customerNumber As String

#End Region

#Region "Public Properties"

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property ShowOptions() As Boolean

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            Select Case UCase(Usage)
                Case Is = "TEMPLATES"
                    .KeyCode = "OrderTemplatesHeader.ascx"
                Case Is = "SAVEDORDERS"
                    .KeyCode = "SavedOrdersHeader.ascx"
            End Select
            btnKeepItems.Text = .Content("KeepBasketButtonText", _languageCode, True)
            btnReplaceItems.Text = .Content("ReplaceBasketButtonText", _languageCode, True)
            lblReplaceBasketQuestion.Text = .Content("EmptyTheBasketContentsQuestion", _languageCode, True)
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            bindHeaderView()
            displayPagerNav(_pds)
        End If
    End Sub

    Protected Sub ChangePage(ByVal sender As Object, ByVal e As EventArgs)
        Dim lb As LinkButton = CType(sender, LinkButton)
        Dim pageIndex As Integer = 0
        Try
            pageIndex = CInt(lb.Text)
            pageIndex -= 1
        Catch ex As Exception
            If lb.Text = _ucr.Content("FirstPageNavigationText", _languageCode, True) Then
                pageIndex = 0
            ElseIf lb.Text = _ucr.Content("LastPageNavigationText", _languageCode, True) Then
                pageIndex = _pds.PageCount - 1
            End If
        End Try

        _pds.CurrentPageIndex = pageIndex
        bindHeaderView()
        displayPagerNav(_pds)
    End Sub

    Protected Sub rptOrderTemplateHeader_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOrderTemplateHeader.ItemDataBound
        If Not e.Item.ItemIndex = -1 Then
            Dim ri As RepeaterItem = e.Item
            SetUpRepeaterItem(ri, e)
        End If
    End Sub

    Protected Sub DeleteTemplate(ByVal sender As Object, ByVal e As EventArgs)
        Dim ri As RepeaterItem = CType(CType(sender, Button).Parent, RepeaterItem)
        Dim HeaderID As String = CType(ri.FindControl("column1Label"), Label).Text
        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                Dim DE_OrdTemplates As New Talent.Common.DEOrderTemplates("DELETE")
                Dim ordTemplate As New Talent.Common.DEOrderTemplate(CType(HeaderID, Long))
                DE_OrdTemplates.OrderTemplates.Add(ordTemplate)

                Dim DB_OrdTemplates As New Talent.Common.DBOrderTemplates(DE_OrdTemplates)
                DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                DB_OrdTemplates.AccessDatabase()
            Case Is = "SAVEDORDERS"
                Dim savedOrders As New SavedOrdersDataSetTableAdapters.tbl_order_saved_headerTableAdapter
                Dim savedOrderDetails As New SavedOrdersDataSetTableAdapters.tbl_order_saved_detailTableAdapter
                savedOrders.DeleteSavedOrder(HeaderID)
                savedOrderDetails.DeleteBy_HeaderID(HeaderID)
        End Select
        bindHeaderView()
    End Sub

    Protected Sub AddToBasket(ByVal sender As Object, ByVal e As EventArgs)
        Dim ri As RepeaterItem = CType(CType(sender, Button).Parent, RepeaterItem)
        Dim HeaderID As String = CType(ri.FindControl("column1Label"), Label).Text
        If Profile.Basket.IsEmpty Then
            plhReplaceBasketQuestion.Visible = False
            HiddenTemplateHeaderID.Value = HeaderID
            masterAddToBasket(HeaderID)
        Else
            plhReplaceBasketQuestion.Visible = True
            HiddenTemplateHeaderID.Value = HeaderID
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Dim ctrl As Control = CType(sender, Control)
        With _ucr
            Select Case UCase(Usage)
                Case Is = "TEMPLATES"
                    Select Case ctrl.ID
                        Case Is = "column1Header"
                            CType(ctrl, Label).Text = .Content("TemplateNameHeader", _languageCode, True)
                        Case Is = "column2Header"
                            CType(ctrl, Label).Text = .Content("TemplateDescriptionHeader", _languageCode, True)
                        Case Is = "column3Header"
                            CType(ctrl, Label).Text = .Content("DefaultTemplateHeader", _languageCode, True)
                        Case Is = "buttonsHeader"
                            CType(ctrl, Label).Text = .Content("ButtonsColumnHeader", _languageCode, True)
                        Case Is = "AddLink"
                            CType(ctrl, Button).Text = .Content("AddTemplateToBasketButtonText", _languageCode, True)
                        Case Is = "DeleteLink"
                            CType(ctrl, Button).Text = .Content("DeleteTemplateButtonText", _languageCode, True)
                            CType(ctrl, Button).Visible = .Attribute("ShowDeleteButton")
                    End Select

                Case Is = "SAVEDORDERS"
                    Select Case ctrl.ID
                        Case Is = "column1Header"
                            CType(ctrl, Label).Text = .Content("SavedOrderIDHeader", _languageCode, True)
                        Case Is = "column2Header"
                            CType(ctrl, Label).Text = .Content("UserIDHeader", _languageCode, True)
                        Case Is = "column3Header"
                            CType(ctrl, Label).Text = .Content("DateCreatedHeader", _languageCode, True)
                        Case Is = "buttonsHeader"
                            CType(ctrl, Label).Text = .Content("ButtonsColumnHeader", _languageCode, True)
                        Case Is = "AddLink"
                            CType(ctrl, Button).Text = .Content("AddToBasketButtonText", _languageCode, True)
                        Case Is = "DeleteLink"
                            CType(ctrl, Button).Text = .Content("DeleteOrderButtonText", _languageCode, True)
                            CType(ctrl, Button).Visible = .Attribute("ShowDeleteButton")
                    End Select
            End Select
        End With
    End Sub

    Protected Sub CheckBoxChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim cb As CheckBox = CType(sender, CheckBox)
        Dim ri As RepeaterItem = CType(cb.Parent, RepeaterItem)
        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                If cb.ID = "column3CheckBox" Then
                    Dim HeaderID As String = CType(ri.FindControl("column1Label"), Label).Text
                    Dim DE_OrdTemplates As New DEOrderTemplates("SET-AS-DEFAULT")
                    Dim userNumber As String = String.Empty
                    If String.IsNullOrEmpty(_customerNumber) Then
                        userNumber = Profile.UserName
                    Else
                        userNumber = _customerNumber
                    End If
                    Dim ordTemplate As New DEOrderTemplate(userNumber, TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, _
                                                                            Now, Now, Now, "", "", False, CType(HeaderID, Long))
                    DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
                    Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
                    DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                    DB_OrdTemplates.AccessDatabase()
                    bindHeaderView()
                End If
            Case Is = "SAVEDORDERS"
        End Select
    End Sub

    Protected Sub btnReplaceItems_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReplaceItems.Click
        plhReplaceBasketQuestion.Visible = False
        Profile.Basket.EmptyBasket()
        masterAddToBasket(HiddenTemplateHeaderID.Value)
    End Sub

    Protected Sub btnKeepItems_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnKeepItems.Click
        plhReplaceBasketQuestion.Visible = False
        masterAddToBasket(HiddenTemplateHeaderID.Value)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Bind the order template repeater
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bindHeaderView()
        If setUpPagedView().DataSource IsNot Nothing AndAlso _pds.Count > 0 Then
            rptOrderTemplateHeader.DataSource = setUpPagedView()
            rptOrderTemplateHeader.DataBind()
            plhNoOrderTemplates.Visible = False
            plhOrderTemplates.Visible = True
        Else
            plhNoOrderTemplates.Visible = True
            plhOrderTemplates.Visible = False
            ltlNoOrderTemplates.Text = _ucr.Content("NoOrderTemplates", _languageCode, True)
        End If
    End Sub

    ''' <summary>
    ''' Setup the pager navigation
    ''' </summary>
    ''' <param name="pds">Paged data source object</param>
    ''' <remarks></remarks>
    Private Sub displayPagerNav(ByVal pds As PagedDataSource)
        If pds.PageCount > 1 Then
            Dim startPage As Integer = 0
            Dim loopToPage As Integer = 0
            Dim currentPage As Integer = 0
            currentPage = pds.CurrentPageIndex + 1

            'If there are more than 10 pages we need to sort out which 10 pages either side
            'of the current page to display in the navigation:
            'E.g. if page = 8 and ther are 15 pages, the navigation should be:
            'First 3 4 5 6 7 8 9 10 11 12 Last
            If pds.PageCount > 10 Then
                If Not pds.CurrentPageIndex = 0 Then
                    For i As Integer = 3 To 0 Step -1
                        If currentPage + i <= pds.PageCount Then
                            startPage = currentPage - (9 - i)
                            If startPage < 0 Then
                                startPage = 0
                            End If
                            Exit For
                        End If
                    Next
                End If
                loopToPage = startPage + 9
            Else
                loopToPage = pds.PageCount
            End If

            If loopToPage = pds.PageCount Then
                If pds.PageCount > 10 Then
                    startPage -= 1
                    loopToPage -= 1
                Else
                    loopToPage -= 1
                End If
            End If

            'Set text against the "First" and "Last" links
            If pds.PageCount > 10 Then
                FirstTop.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True)
                FirstBottom.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True)
                LastTop.Text = _ucr.Content("LastItemNavigationText", _languageCode, True)
                LastBottom.Text = _ucr.Content("LastItemNavigationText", _languageCode, True)
            End If

            'Set text against each of the items shown
            Dim count As Integer = 0
            For i As Integer = startPage To loopToPage
                count += 1
                CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Text = (i + 1).ToString
                CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Text = (i + 1).ToString
            Next

            count += 1
            For i As Integer = count To 10
                CType(Me.FindControl("Nav" & i.ToString & "Top"), LinkButton).Text = String.Empty
                CType(Me.FindControl("Nav" & i.ToString & "Bottom"), LinkButton).Text = String.Empty
            Next

            'Determine which number of rows are currently being shown
            Dim arg0, arg1, arg2 As Integer
            arg0 = pds.PageSize * pds.CurrentPageIndex + 1
            If pds.IsLastPage Then
                arg1 = pds.DataSourceCount
            Else
                arg1 = (arg0 - 1) + pds.PageSize
            End If
            arg2 = pds.DataSourceCount

            'Set the "currently displaying" text
            lblCurrentResultsDisplaying.Text = String.Format(_ucr.Content("ResultsCurrentViewText", _languageCode, True), arg0.ToString, arg1.ToString, arg2.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Build the repeater display
    ''' </summary>
    ''' <param name="ri">Repeater item</param>
    ''' <param name="e">Repeat item event argument</param>
    ''' <remarks></remarks>
    Private Sub setUpRepeaterItem(ByVal ri As RepeaterItem, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim label1 As Label = ri.FindControl("column1Label")
        Dim hyper1 As HyperLink = ri.FindControl("column1HyperLink")
        Dim label2 As Label = ri.FindControl("column2Label")
        Dim hyper2 As HyperLink = ri.FindControl("column2HyperLink")
        Dim cb3 As CheckBox = ri.FindControl("column3CheckBox")
        Dim label3 As Label = ri.FindControl("column3Label")

        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                Dim row As Data.DataRow
                row = CType(e.Item.DataItem, Data.DataRowView).Row
                hyper1.Visible = True
                hyper1.Text = TEBUtilities.CheckForDBNull_String(row("NAME"))
                hyper1.NavigateUrl = "~/PagesLogin/Template/EditTemplate.aspx?source=edit&hid=" & TCUtilities.CheckForDBNull_BigInt(row("TEMPLATE_HEADER_ID")).ToString

                label1.Text = TCUtilities.CheckForDBNull_BigInt(row("TEMPLATE_HEADER_ID"))
                label1.Visible = False

                label2.Visible = True
                label2.Text = TEBUtilities.CheckForDBNull_String(row("DESCRIPTION"))

                cb3.Visible = True
                cb3.Checked = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("IS_DEFAULT"))
                cb3.AutoPostBack = True
            Case Is = "SAVEDORDERS"
                Dim item As SavedOrdersDataSet.tbl_order_saved_headerRow
                item = CType(CType(e.Item.DataItem, Data.DataRowView).Row, SavedOrdersDataSet.tbl_order_saved_headerRow)

                label1.Visible = True
                label1.Text = item.SAVED_HEADER_ID

                hyper2.Visible = True
                hyper2.Text = item.LOGINID
                hyper2.NavigateUrl = "~/PagesLogin/SavedOrders/SavedOrdersDetails.aspx?hid=" & item.SAVED_HEADER_ID

                label3.Visible = True
                label3.Text = item.CREATED_DATE
        End Select
    End Sub

    ''' <summary>
    ''' Add selected products to basket based on the given header id
    ''' </summary>
    ''' <param name="HeaderID">The order template header id</param>
    ''' <remarks></remarks>
    Private Sub masterAddToBasket(ByVal HeaderID As String)
        Dim detail As Data.DataTable
        Dim amendBasket As New DEAmendBasket
        Dim deaItem As DEAlerts

        With amendBasket
            .AddToBasket = True
            .BasketId = Profile.Basket.Basket_Header_ID
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = Profile.PartnerInfo.Details.Partner
            .UserID = Profile.UserName
        End With

        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                'Retrieve the information on the currently selected template
                Dim DE_OrdTemplates As New DEOrderTemplates("SELECT")
                Dim ordTemplate As New DEOrderTemplate(CType(HeaderID, Long))
                DE_OrdTemplates.OrderTemplates.Add(ordTemplate)

                Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
                DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                DB_OrdTemplates.AccessDatabase()
                detail = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesDetail")

            Case Is = "SAVEDORDERS"
                'Get the details of the template order
                Dim detailsAdapter As New SavedOrdersDataSetTableAdapters.tbl_order_saved_detailTableAdapter
                detail = detailsAdapter.GetBy_HeaderID(HeaderID)

            Case Else
                detail = New Data.DataTable
        End Select

        For Each row As Data.DataRow In detail.Rows
            If TCUtilities.CheckForDBNull_String(row("MASTER_PRODUCT")).Trim.Length > 0 Then
                deaItem = New DEAlerts
                deaItem.ProductCode = TCUtilities.CheckForDBNull_String(row("PRODUCT_CODE"))
                deaItem.Quantity = TCUtilities.CheckForDBNull_Decimal(row("QUANTITY"))

                ' Doesn't exist if saved orders
                If UCase(Usage) = "TEMPLATES" Then
                    deaItem.MasterProduct = Talent.Common.Utilities.CheckForDBNull_String(row("MASTER_PRODUCT"))
                End If

                deaItem.Price = TEBUtilities.GetWebPrices(deaItem.ProductCode, deaItem.Quantity, deaItem.MasterProduct).Purchase_Price_Gross
                If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                    deaItem.CostCentre = Profile.PartnerInfo.Details.COST_CENTRE
                    deaItem.AccountCode = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                End If
                amendBasket.CollDEAlerts.Add(deaItem)
            End If
        Next

        Dim DBAmend As New DBAmendBasket
        With DBAmend
            .Dep = amendBasket
            .Settings = TEBUtilities.GetSettingsObject()
            .AccessDatabase()
        End With

        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Retreive the data and return the paged data source object
    ''' </summary>
    ''' <returns>paged data source object</returns>
    ''' <remarks></remarks>
    Private Function setUpPagedView() As PagedDataSource
        _pds.AllowPaging = True
        Try
            Select Case UCase(Usage)
                Case Is = "TEMPLATES"
                    Dim templates As New OrderTemplatesDataSetTableAdapters.tbl_order_template_headerTableAdapter
                    If ModuleDefaults.OrderTemplatesPerPartner Then
                        _pds.DataSource = templates.Get_All_By_Partner(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner).DefaultView
                    Else
                        _pds.PageSize = TEBUtilities.CheckForDBNull_Int((_ucr.Content("ResultsViewPageSize", _languageCode, True)))

                        Dim DE_OrdTemplates As New DEOrderTemplates("SELECT")
                        Dim userNumber As String = String.Empty
                        If String.IsNullOrEmpty(_customerNumber) Then
                            userNumber = Profile.UserName
                        Else
                            userNumber = _customerNumber
                        End If
                        Dim ordTemplate As DEOrderTemplate = Nothing
                        If _customerNumber = Profile.UserName Then
                            ordTemplate = New DEOrderTemplate(userNumber, TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, Now, Now, Now)
                        Else
                            ordTemplate = New DEOrderTemplate(userNumber, TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, Now, Now, Now, String.Empty, String.Empty, False, True)
                        End If

                        DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
                        Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
                        DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                        DB_OrdTemplates.AccessDatabase()

                        Dim header As Data.DataTable = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesHeader")
                        _pds.DataSource = header.DefaultView
                    End If
                Case Is = "SAVEDORDERS"
                    Dim savedOrders As New SavedOrdersDataSetTableAdapters.tbl_order_saved_headerTableAdapter
                    _pds.DataSource = savedOrders.Get_Unprocessed_By_BU_Partner_LoginID(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, Profile.UserName).DefaultView
                    _pds.PageSize = 10
            End Select
        Catch ex As Exception

        End Try
        Return _pds
    End Function

#End Region

End Class