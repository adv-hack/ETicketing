Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common

Partial Class Products_ProductRelationships
    Inherits PageControlBase

#Region "Constants"

    Const PAGECODE As String = "ProductRelationships.aspx"
    Const STARTSYMBOL As String = "*"
    Const DELETECOMMANDNAME As String = "Delete"
    Const EDITCOMMANDNAME As String = "Edit"
    Const DASHDASH As String = "--"

#End Region

#Region "Private Properties"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()
    Private Shared _partner As String = String.Empty
    Private Shared _businessUnit As String = String.Empty
    Private _ticketingProductTypes() As String = {DASHDASH, "H", "A", "C", "E", "T", "S"}

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
    Public ProductColumnHeader As String
    Public TypeSubTypeColumnHeader As String
    Public DateColumnHeader As String
    Public PriceCodesColumnHeader As String
    Public LinkedToColumnHeader As String
    Public DeleteColumnHeader As String
    Public DeleteButtonText As String
    Public EditButtonText As String
    Public Shared NothingFoundText As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        plhRelationshipDeleted.Visible = False

        Dim okToRetrieveProductRelations As Boolean = False
        If Request.QueryString("BU") IsNot Nothing Then
            _businessUnit = Utilities.CheckForDBNull_String(Request.QueryString("BU"))
            If Request.QueryString("Partner") IsNot Nothing Then
                _partner = Utilities.CheckForDBNull_String(Request.QueryString("Partner"))
                okToRetrieveProductRelations = True
            End If
        End If

        ProductColumnHeader = _wfrPage.Content("ProductColumnHeaderText", _languageCode, True)
        TypeSubTypeColumnHeader = _wfrPage.Content("TypeSubTypeColumnHeaderText", _languageCode, True)
        DateColumnHeader = _wfrPage.Content("DateColumnHeaderText", _languageCode, True)
        PriceCodesColumnHeader = _wfrPage.Content("PriceCodesColumnHeaderText", _languageCode, True)
        LinkedToColumnHeader = _wfrPage.Content("LinkedToColumnHeaderText", _languageCode, True)
        DeleteColumnHeader = _wfrPage.Content("DeleteColumnHeaderText", _languageCode, True)
        DeleteButtonText = _wfrPage.Content("DeleteButtonText", _languageCode, True)
        EditButtonText = _wfrPage.Content("EditButtonText", _languageCode, True)
        Dim linkTypeBoth As New ListItem
        linkTypeBoth.Value = GlobalConstants.PRODUCTLINKTYPEBOTH
        linkTypeBoth.Text = _wfrPage.Content("ProductLinkTypeBothText", _languageCode, True)
        Dim linkTypePhase1 As New ListItem
        linkTypePhase1.Value = GlobalConstants.PRODUCTLINKTYPEPHASE1ONLY
        linkTypePhase1.Text = _wfrPage.Content("ProductLinkTypePhase1", _languageCode, True)
        Dim linkTypePhase2 As New ListItem
        linkTypePhase2.Value = GlobalConstants.PRODUCTLINKTYPEPHASE2ONLY
        linkTypePhase2.Text = _wfrPage.Content("ProductLinkTypePhase2", _languageCode, True)
        Dim linkTypePhase3 As New ListItem
        linkTypePhase3.Value = GlobalConstants.PRODUCTLINKTYPEPHASE3ONLY
        linkTypePhase3.Text = _wfrPage.Content("ProductLinkTypePhase3", _languageCode, True)

        If Not Page.IsPostBack Then
            ddlLinkTypeSearch.Items.Clear()
            ddlLinkTypeSearch.Items.Add(DASHDASH)
            ddlLinkTypeSearch.Items.Add(linkTypePhase1)
            ddlLinkTypeSearch.Items.Add(linkTypePhase2)
            ddlLinkTypeSearch.Items.Add(linkTypeBoth)
            ddlLinkTypeSearch.Items.Add(linkTypePhase3)
            If okToRetrieveProductRelations Then
                ddlSearchTicketingProductType.DataSource = _ticketingProductTypes
                ddlSearchTicketingProductType.DataBind()
                plhProductSearch.Visible = True
                btnAddNewRelationShip.PostBackUrl = String.Format("~/Products/EditProductRelationships.aspx?BU={0}&Partner=*ALL", _businessUnit)
                setPageText()
                If Not String.IsNullOrEmpty(Session("SearchType")) Then
                    ddlLinkTypeSearch.SelectedValue = Session("SearchType")
                End If
                If loadProductRelations(String.Empty, String.Empty, String.Empty, ddlLinkTypeSearch.SelectedValue) Then
                    ltlProductRelationshipsCount.Text = _wfrPage.Content("AllRelationshipsText", _languageCode, True).Replace("<<NUMBER>>", rptProductRelationships.Items.Count.ToString())
                Else
                    ltlProductRelationshipsCount.Text = _wfrPage.Content("CurrentlyNoRelationshipsText", _languageCode, True).Replace("<<NUMBER>>", rptProductRelationships.Items.Count.ToString())
                End If
            Else
                pageinstructionsLabel.Text = _wfrPage.Content("PageInstructions_NoBusinessUnit", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub btnAddNewRelationShip_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddNewRelationShip.Click
        Dim redirectUrl As New StringBuilder
        redirectUrl.Append("EditProductRelationships.aspx?BU=")
        redirectUrl.Append(_businessUnit)
        redirectUrl.Append("&Partner=")
        redirectUrl.Append(_partner)
        Response.Redirect(redirectUrl.ToString())
    End Sub

    Protected Sub btnSearchOptions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearchOptions.Click
        Session("SearchType") = ddlLinkTypeSearch.SelectedValue
        findProducts()
    End Sub

    Protected Sub rptProductRelationships_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProductRelationships.ItemCommand
        If e.CommandName = DELETECOMMANDNAME Then
            Dim productRelationsId As Integer = 0
            Dim linkType As Integer = 0
            productRelationsId = Utilities.CheckForDBNull_Int(e.CommandArgument)
            Dim tDataObjects As New TalentDataObjects()
            Dim settings As DESettings = New DESettings()
            Dim err As New ErrorObj
            Dim hasError As Boolean = False
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            settings.BusinessUnit = _businessUnit
            settings.ConnectionStringList = Utilities.GetConnectionStringList()
            tDataObjects.Settings = settings
            plhRelationshipDeleted.Visible = True
            linkType = tDataObjects.ProductsSettings.TblProductRelations.GetLinkTypeByProductRelationshipID(productRelationsId)

            If linkType <> 3 Then
                If tDataObjects.ProductsSettings.TblProductRelations.DeleteProductRelation(productRelationsId) < 1 Then
                    hasError = True
                End If
            Else
                Dim foreignProductRelationsID As String = tDataObjects.ProductsSettings.TblProductRelations.GetForiegnProductRelationsID(productRelationsId)
                If Not foreignProductRelationsID.Equals("0") Then
                    Dim talProduct As New TalentProduct
                    Dim deProductDetails As New DEProductDetails
                    With deProductDetails
                        .ProductRelationsID = foreignProductRelationsID
                    End With
                    talProduct.Settings = settings
                    talProduct.De = deProductDetails
                    err = talProduct.DeleteLinkedProductPackage()
                End If
                If Not err.HasError Then
                    If tDataObjects.ProductsSettings.TblProductRelations.DeleteProductRelationByForiegnId(foreignProductRelationsID) < 1 Then
                        hasError = True
                    End If
                Else
                    hasError = True
                End If
            End If

            If Not hasError Then
                If txtSearchCode.Text.Length > 0 OrElse txtSearchDescription.Text.Length > 0 OrElse ddlSearchTicketingProductType.SelectedValue <> DASHDASH OrElse ddlLinkTypeSearch.SelectedValue <> DASHDASH Then
                    findProducts()
                Else
                    If loadProductRelations(String.Empty, String.Empty, String.Empty, String.Empty) Then
                        ltlProductRelationshipsCount.Text = _wfrPage.Content("AllRelationshipsText", _languageCode, True).Replace("<<NUMBER>>", rptProductRelationships.Items.Count.ToString())
                    Else
                        ltlProductRelationshipsCount.Text = _wfrPage.Content("CurrentlyNoRelationshipsText", _languageCode, True).Replace("<<NUMBER>>", 0)
                        plhProductRelationships.Visible = False
                    End If
                End If
                lblRelationshipDeleted.Text = _wfrPage.Content("RelationshipDeleted", _languageCode, True)
            Else
                lblRelationshipDeleted.Text = _wfrPage.Content("RelationshipNotDeleted", _languageCode, True)
            End If

        ElseIf e.CommandName = EDITCOMMANDNAME Then
            Dim redirectUrl As New StringBuilder
            redirectUrl.Append("EditProductRelationships.aspx?id=").Append(e.CommandArgument)
            redirectUrl.Append("&BU=").Append(Request.QueryString("BU"))
            redirectUrl.Append("&Partner=").Append(Request.QueryString("Partner"))
            Response.Redirect(redirectUrl.ToString())
        End If
    End Sub

    Protected Sub btnRefreshProductList_Click(sender As Object, e As System.EventArgs) Handles btnRefreshProductList.Click
        Dim settings As DESettings = New DESettings()
        Dim deFeeds As New DEFeeds
        Dim feeds As New TalentFeeds
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        TDataObjects.Settings = settings
        settings.OriginatingSource = "W"
        settings.StoredProcedureGroup = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
        DEFeeds.Corporate_Stadium = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "CORPORATE_STADIUM")
        DEFeeds.Ticketing_Stadium = TDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "TICKETING_STADIUM")
        deFeeds.Product_Type = GlobalConstants.STARALLPARTNER
        deFeeds.Online_Products_Only = False
        feeds.FeedsEntity = DEFeeds
        feeds.Settings = settings
        feeds.RemoveProductFeedsFromCache()
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the text properties of the page controls
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageText()
        Page.Title = _wfrPage.Content("PageTitle", _languageCode, True)
        pagetitleLabel.Text = _wfrPage.Content("PageTitle", _languageCode, True)
        pageinstructionsLabel.Text = _wfrPage.Content("PageInstructions", _languageCode, True)
        lblSearchDescription.Text = _wfrPage.Content("SearchDescriptionText", _languageCode, True)
        lblSearchCode.Text = _wfrPage.Content("SearchCodeText", _languageCode, True)
        lblSearchTicketingProductType.Text = _wfrPage.Content("SearchTicketingProductTypeText", _languageCode, True)
        lblLinkTypeSearch.Text = _wfrPage.Content("lblLinkTypeSearchText", _languageCode, True)
        btnSearchOptions.Text = _wfrPage.Content("SearchOptionsButtonText", _languageCode, True)
        btnAddNewRelationShip.Text = _wfrPage.Content("AddNewRelationshipButtonText", _languageCode, True)
        NothingFoundText = _wfrPage.Content("NothingFoundText", _languageCode, True)
        aceCodeSearch.CompletionSetCount = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("ACECompletionSetCount"))
        aceSearch.CompletionSetCount = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("ACECompletionSetCount"))
        btnRefreshProductList.Text = _wfrPage.Content("RefreshProductListButtonText", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Perform the product relationship search
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub findProducts()
        If txtSearchCode.Text.Length > 0 OrElse txtSearchDescription.Text.Length > 0 OrElse ddlSearchTicketingProductType.SelectedValue <> DASHDASH OrElse ddlLinkTypeSearch.SelectedValue <> DASHDASH Then
            If loadProductRelations(txtSearchDescription.Text, txtSearchCode.Text, ddlSearchTicketingProductType.SelectedValue, ddlLinkTypeSearch.SelectedValue) Then
                plhNoProductRelationships.Visible = False
                plhProductRelationships.Visible = True
                ltlProductRelationshipsCount.Text = _wfrPage.Content("NumberOfRelationshipsText", _languageCode, True).Replace("<<NUMBER>>", rptProductRelationships.Items.Count.ToString())
            Else
                plhNoProductRelationships.Visible = True
                plhProductRelationships.Visible = False
                lblNoProductRelationships.Text = _wfrPage.Content("NoProductsFound", _languageCode, True)
            End If
        Else
            plhNoProductRelationships.Visible = True
            plhProductRelationships.Visible = False
            lblNoProductRelationships.Text = _wfrPage.Content("EnterSearchOptionsText", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Create the product relations temp table
    ''' </summary>
    ''' <returns>True if product relations have been bound to the repeater</returns>
    ''' <remarks></remarks>
    Private Function loadProductRelations(ByVal productDescription As String, ByVal productCode As String, ByVal productType As String, ByVal linkType As String) As Boolean
        Dim hasProductRelations As Boolean = False
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings

        Dim fullProductRelationsTable As DataTable = tDataObjects.ProductsSettings.TblProductRelations.GetAllProductRelationsByBUAndPartner(_businessUnit, _partner, False)
        If fullProductRelationsTable.Rows.Count > 0 Then
            Try
                Dim retailTable As DataTable = getRetailData()
                Dim ticketingTable As DataTable = getTicketingData()
                Dim allProductsTable As DataTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
                Dim sortByProduct As String = "PRODUCT ASC"
                If allProductsTable.Rows.Count > 0 Then
                    Dim matchedRows1() As DataRow = Nothing
                    Dim matchedRows2() As DataRow = Nothing
                    Dim filteredProducts As New DataTable
                    If productDescription.Length > 0 Then
                        matchedRows1 = allProductsTable.Select("PRODUCT_DESCRIPTION = '" & productDescription & "'", "PRODUCT_CODE DESC")
                        If matchedRows1.Length > 0 Then
                            productCode = matchedRows1(0)("PRODUCT_CODE").ToString()
                            txtSearchCode.Text = productCode
                            matchedRows2 = fullProductRelationsTable.Select("PRODUCT = '" & productCode & "'")
                            hasProductRelations = bindRepeater(matchedRows2, allProductsTable)
                        End If
                    ElseIf productCode.Length > 0 Then
                        matchedRows1 = allProductsTable.Select("PRODUCT_CODE = '" & productCode & "'", "PRODUCT_CODE DESC")
                        If matchedRows1.Length > 0 Then
                            matchedRows2 = fullProductRelationsTable.Select("PRODUCT = '" & productCode & "'")
                            hasProductRelations = bindRepeater(matchedRows2, allProductsTable)
                            txtSearchDescription.Text = getProductDescriptionByProductCode(allProductsTable, productCode)
                        End If
                    ElseIf productType <> DASHDASH AndAlso productType.Length > 0 Then
                        Dim updatedTable As DataTable = removeProductsNoLongerAvailable(fullProductRelationsTable, ticketingTable, retailTable)
                        matchedRows1 = updatedTable.Select("TICKETING_PRODUCT_TYPE = '" & productType & "'")
                        hasProductRelations = bindRepeater(matchedRows1, allProductsTable)
                    ElseIf linkType <> DASHDASH AndAlso linkType.Length > 0 Then
                        Dim updatedTable As DataTable = removeProductsNoLongerAvailable(fullProductRelationsTable, ticketingTable, retailTable)
                        If linkType = 0 Then
                            matchedRows1 = updatedTable.Select("LINK_TYPE = '1' OR LINK_TYPE = '2'", sortByProduct)
                        Else
                            matchedRows1 = updatedTable.Select("LINK_TYPE = '" & linkType & "'", sortByProduct)
                        End If

                        hasProductRelations = bindRepeater(matchedRows1, allProductsTable)
                    Else
                        Dim updatedTable As DataTable = removeProductsNoLongerAvailable(fullProductRelationsTable, ticketingTable, retailTable)
                        matchedRows1 = updatedTable.Select("", sortByProduct)
                        hasProductRelations = bindRepeater(matchedRows1, allProductsTable)
                    End If
                End If
            Catch ex As Exception
                hasProductRelations = False
            End Try
        End If
        Return hasProductRelations
    End Function

    ''' <summary>
    ''' Bind the repeater based on the given temp table
    ''' </summary>
    ''' <param name="rowsToBind">rows required to bind to the repeater</param>
    ''' <param name="allProductsTable">All ticketing and retail products combined with descriptions</param>
    ''' <returns>True if product relations have been bound to the repeater</returns>
    ''' <remarks></remarks>
    Private Function bindRepeater(ByVal rowsToBind As DataRow(), ByVal allProductsTable As DataTable) As Boolean
        Dim repeaterBoundSuccessfully As Boolean = False
        Try
            Dim tempTableForRepeater As New DataTable
            Dim tempRow As DataRow = Nothing
            With tempTableForRepeater.Columns
                .Add("PRODUCT_RELATIONS_ID", GetType(Integer))
                .Add("PRODUCT_CODE", GetType(String))
                .Add("PRODUCT_DATE", GetType(String))
                .Add("TICKETING_PRODUCT_TYPE", GetType(String))
                .Add("TICKETING_PRODUCT_SUB_TYPE", GetType(String))
                .Add("TICKETING_PRODUCT_PRICE_CODE", GetType(String))
                .Add("PRODUCT_DESCRIPTION", GetType(String))
                .Add("LINKED_TO", GetType(String))
                .Add("RELATED_PRODUCT_CODE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_TYPE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_SUB_TYPE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_PRICE_CODE", GetType(String))
                .Add("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE", GetType(String))
                .Add("LINK_TYPE", GetType(String))
            End With
            For Each row As DataRow In rowsToBind
                tempRow = Nothing
                tempRow = tempTableForRepeater.NewRow
                tempRow("PRODUCT_RELATIONS_ID") = row("PRODUCT_RELATIONS_ID")
                tempRow("PRODUCT_CODE") = row("PRODUCT").ToString()
                tempRow("PRODUCT_DATE") = getProductDateByProductCode(allProductsTable, row("PRODUCT").ToString())
                tempRow("PRODUCT_DESCRIPTION") = getProductDescriptionByProductCode(allProductsTable, row("PRODUCT").ToString())
                tempRow("TICKETING_PRODUCT_TYPE") = row("TICKETING_PRODUCT_TYPE").ToString()
                tempRow("TICKETING_PRODUCT_PRICE_CODE") = row("TICKETING_PRODUCT_PRICE_CODE").ToString()
                tempRow("TICKETING_PRODUCT_SUB_TYPE") = row("TICKETING_PRODUCT_SUB_TYPE").ToString()
                tempRow("LINKED_TO") = getProductDescriptionByProductCode(allProductsTable, row("RELATED_PRODUCT").ToString())
                tempRow("RELATED_PRODUCT_CODE") = row("RELATED_PRODUCT").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_TYPE") = row("RELATED_TICKETING_PRODUCT_TYPE").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_SUB_TYPE") = row("RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_PRICE_CODE") = row("RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString()
                tempRow("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE") = row("RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString()
                tempRow("LINK_TYPE") = row("LINK_TYPE").ToString()
                tempTableForRepeater.Rows.Add(tempRow)
            Next
            If tempTableForRepeater.Rows.Count > 0 Then
                repeaterBoundSuccessfully = True
                rptProductRelationships.DataSource = tempTableForRepeater
                rptProductRelationships.DataBind()
            End If
        Catch ex As Exception
            repeaterBoundSuccessfully = False
        End Try
        Return repeaterBoundSuccessfully
    End Function

    ''' <summary>
    ''' Retrieves the product description by the given product code
    ''' </summary>
    ''' <param name="allProductsTable">The table of product codes and descriptions</param>
    ''' <param name="productCode">The product code the description is needed for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getProductDescriptionByProductCode(ByRef allProductsTable As DataTable, ByVal productCode As String) As String
        Dim productDescription As String = String.Empty
        For Each row As DataRow In allProductsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                productDescription = row("PRODUCT_DESCRIPTION").ToString()
                Exit For
            End If
        Next
        Return productDescription
    End Function

    ''' <summary>
    ''' Retrieves the product date by the given product code
    ''' </summary>
    ''' <param name="allProductsTable">The table of product codes and descriptions</param>
    ''' <param name="productCode">The product code the description is needed for</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getProductDateByProductCode(ByRef allProductsTable As DataTable, ByVal productCode As String) As String
        Dim productDate As String = String.Empty
        For Each row As DataRow In allProductsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                productDate = row("PRODUCT_DATE").ToString()
                Exit For
            End If
        Next
        Return productDate
    End Function

    ''' <summary>
    ''' Get the retail data (product codes and descriptions)
    ''' </summary>
    ''' <returns>A data table of product codes and descriptions</returns>
    ''' <remarks></remarks>
    Private Shared Function getRetailData() As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        Dim productRelationsWithDescriptionsTable As DataTable = tDataObjects.ProductsSettings.TblProductRelations.GetProductDescriptionsByBUAndPartner(_businessUnit, _partner, False)
        tDataObjects = Nothing
        Return productRelationsWithDescriptionsTable
    End Function

    ''' <summary>
    ''' Get the ticketing data (product codes and descriptions)
    ''' </summary>
    ''' <returns>A data table of product codes and descriptions</returns>
    ''' <remarks></remarks>
    Private Shared Function getTicketingData(Optional ByVal caching As Boolean = True) As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim feeds As New TalentFeeds
        Dim deFeeds As New DEFeeds
        Dim err As New ErrorObj
        Dim topUpProduct As String = String.Empty
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        settings.OriginatingSource = "W"
        settings.StoredProcedureGroup = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "STORED_PROCEDURE_GROUP")
        deFeeds.Corporate_Stadium = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "CORPORATE_STADIUM")
        deFeeds.Ticketing_Stadium = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "TICKETING_STADIUM")
        deFeeds.Product_Type = "ALL"
        deFeeds.Online_Products_Only = False
        feeds.FeedsEntity = deFeeds
        settings.Cacheing = caching
        settings.CacheTimeMinutes = 10080 '(7 days)
        feeds.Settings = settings
        err = feeds.GetXMLFeed
        If err.HasError Then
            tDataObjects = Nothing
            Return New DataTable
        Else
            If feeds.ProductsDataView IsNot Nothing Then
                topUpProduct = tDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(_businessUnit, "EPURSE_TOP_UP_PRODUCT_CODE")
                Dim dvFeeds As DataView = feeds.ProductsDataView
                If topUpProduct.Length > 0 Then
                    If dvFeeds.RowFilter.Length > 0 Then
                        dvFeeds.RowFilter = dvFeeds.RowFilter & " AND ProductCode <> '" & topUpProduct & "'"
                    Else
                        dvFeeds.RowFilter = "ProductCode <> '" & topUpProduct & "'"
                    End If
                End If
                tDataObjects = Nothing
                Return dvFeeds.ToTable
            Else
                tDataObjects = Nothing
                Return New DataTable
            End If
        End If
    End Function

    ''' <summary>
    ''' Combine the ticketing and retail data tables together
    ''' </summary>
    ''' <param name="retailTable">retail data table</param>
    ''' <param name="ticketingTable">ticketing data table</param>
    ''' <returns>combined data table</returns>
    ''' <remarks></remarks>
    Private Shared Function combineTicketingAndRetailTables(ByVal retailTable As DataTable, ByVal ticketingTable As DataTable) As DataTable
        Dim allProductsTable As New DataTable
        Dim dRow As DataRow = Nothing
        With allProductsTable.Columns
            .Add("PRODUCT_CODE", GetType(String))
            .Add("PRODUCT_DATE", GetType(String))
            .Add("PRODUCT_DESCRIPTION", GetType(String))
            .Add("TICKETING_PRODUCT_TYPE", GetType(String))
            .Add("TICKETING_PRODUCT_SUB_TYPE", GetType(String))
        End With
        For Each row As DataRow In retailTable.Rows
            dRow = allProductsTable.NewRow
            dRow("PRODUCT_CODE") = row("PRODUCT_CODE").ToString()
            dRow("PRODUCT_DATE") = String.Empty
            dRow("PRODUCT_DESCRIPTION") = row("PRODUCT_DESCRIPTION_1").ToString()
            dRow("TICKETING_PRODUCT_TYPE") = String.Empty
            dRow("TICKETING_PRODUCT_SUB_TYPE") = String.Empty
            allProductsTable.Rows.Add(dRow)
        Next
        For Each row As DataRow In ticketingTable.Rows
            dRow = allProductsTable.NewRow
            dRow("PRODUCT_CODE") = row("ProductCode").ToString()
            dRow("PRODUCT_DATE") = CDate(row("ProductDateYear")).ToShortDateString()
            dRow("PRODUCT_DESCRIPTION") = row("ProductDescription").ToString()
            dRow("TICKETING_PRODUCT_TYPE") = row("ProductType").ToString()
            dRow("TICKETING_PRODUCT_SUB_TYPE") = row("ProductSubType").ToString()
            allProductsTable.Rows.Add(dRow)
        Next
        Return allProductsTable
    End Function

    ''' <summary>
    ''' Remove any product relations to products that are no longer on sale
    ''' </summary>
    ''' <param name="productRelationsTable">The product relations table</param>
    ''' <param name="ticketingTable">The current available ticketing product table</param>
    ''' <param name="retailTable">The current available retail product table</param>
    ''' <returns>A modified product relations table</returns>
    ''' <remarks></remarks>
    Private Function removeProductsNoLongerAvailable(ByVal productRelationsTable As DataTable, ByRef ticketingTable As DataTable, ByRef retailTable As DataTable) As DataTable
        For Each row As DataRow In productRelationsTable.Rows
            Dim productFound As Boolean = False
            For Each ticketingRow As DataRow In ticketingTable.Rows
                If row("RELATED_PRODUCT") = ticketingRow("ProductCode") Then
                    productFound = True
                    Exit For
                End If
            Next
            If Not productFound Then
                For Each retailRow As DataRow In retailTable.Rows
                    If row("RELATED_PRODUCT") = retailRow("PRODUCT_CODE") Then
                        productFound = True
                        Exit For
                    End If
                Next
            End If
            If Not productFound Then row.Delete()
        Next
        productRelationsTable.AcceptChanges()
        Return productRelationsTable
    End Function
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the list of product descriptions for the auto complete extender
    ''' </summary>
    ''' <param name="prefixText">The text to filter on</param>
    ''' <param name="count">The number of items in the list</param>
    ''' <returns>List of strings to use</returns>
    ''' <remarks></remarks>
    <System.Web.Script.Services.ScriptMethod(), System.Web.Services.WebMethod()> _
    Public Shared Function GetProductDescList(ByVal prefixText As String, ByVal count As Integer) As List(Of String)
        Dim productDescList As List(Of String) = New List(Of String)
        Dim retailTable As DataTable = getRetailData()
        Dim ticketingTable As DataTable = getTicketingData()
        Dim allProductsTable As DataTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
        If allProductsTable IsNot Nothing AndAlso allProductsTable.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            If prefixText.StartsWith(STARTSYMBOL) Then
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_DESCRIPTION").Select("PRODUCT_DESCRIPTION LIKE'%" & prefixText.Replace(STARTSYMBOL, "") & "%'")
            Else
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_DESCRIPTION").Select("PRODUCT_DESCRIPTION LIKE'%" & prefixText & "%'")
            End If
            If matchedRows.Length > 0 Then
                Dim itemsToBind As Integer = matchedRows.Length - 1
                If count < matchedRows.Length - 1 Then itemsToBind = count - 1
                For rowIndex As Integer = 0 To itemsToBind
                    productDescList.Add(matchedRows(rowIndex)("PRODUCT_DESCRIPTION").ToString())
                Next
            Else
                productDescList.Add(NothingFoundText)
            End If
        End If
        Return productDescList
    End Function

    ''' <summary>
    ''' Get the list of product codes for the auto complete extender
    ''' </summary>
    ''' <param name="prefixText">The text to filter on</param>
    ''' <param name="count">The number of items in the list</param>
    ''' <returns>List of strings to use</returns>
    ''' <remarks></remarks>
    <System.Web.Script.Services.ScriptMethod(), System.Web.Services.WebMethod()> _
    Public Shared Function GetProductCodeList(ByVal prefixText As String, ByVal count As Integer) As List(Of String)
        Dim productCodeList As List(Of String) = New List(Of String)
        Dim retailTable As DataTable = getRetailData()
        Dim ticketingTable As DataTable = getTicketingData()
        Dim allProductsTable As DataTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
        If allProductsTable IsNot Nothing AndAlso allProductsTable.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            If prefixText.StartsWith(STARTSYMBOL) Then
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_CODE").Select("PRODUCT_CODE LIKE'%" & prefixText.Replace(STARTSYMBOL, "") & "%'")
            Else
                matchedRows = allProductsTable.DefaultView.ToTable(True, "PRODUCT_CODE").Select("PRODUCT_CODE LIKE'%" & prefixText & "%'")
            End If
            If matchedRows.Length > 0 Then
                Dim itemsToBind As Integer = matchedRows.Length - 1
                If count < matchedRows.Length - 1 Then itemsToBind = count - 1
                For rowIndex As Integer = 0 To itemsToBind
                    productCodeList.Add(matchedRows(rowIndex)("PRODUCT_CODE").ToString())
                Next
            Else
                productCodeList.Add(NothingFoundText)
            End If
        End If
        Return productCodeList
    End Function

    ''' <summary>
    ''' Format 2 strings with a forward slash if there are two strings
    ''' </summary>
    ''' <param name="string1">string 1</param>
    ''' <param name="string2">string 2</param>
    ''' <returns>Formatted concatenated string with a slash between the two</returns>
    ''' <remarks></remarks>
    Public Function Format2Strings(ByVal string1 As String, ByVal string2 As String) As String
        Dim formattedString As String = String.Empty
        Dim separator As String = "/"
        If string1.Length > 0 AndAlso string2.Length > 0 Then
            formattedString = string1 & separator & string2
        Else
            If string1.Length > 0 Then formattedString = string1
            If string2.Length > 0 Then formattedString = string2
        End If
        Return formattedString
    End Function

    ''' <summary>
    ''' If the product is no longer available, due to it no longer being for sale or has expired there is no description shown,
    ''' therefore show a generic description. No desciption is returned when the product is available.
    ''' When there is no product code, we have to assume that it is a link that has been created based on product type or sub type
    ''' therefore show an "N/A" type message.
    ''' </summary>
    ''' <param name="productDescription">The current product description</param>
    ''' <param name="productCode">The product code used for display purposes</param>
    ''' <returns>The new product description</returns>
    ''' <remarks></remarks>
    Public Function GetDescription(ByVal productDescription As String, ByVal productCode As String) As String
        Dim newProductDescription As String = _wfrPage.Content("ProductNotAvailableText", _languageCode, True).Replace("<<PRODUCT_CODE>>", productCode)
        If productDescription.Length > 0 Then
            newProductDescription = String.Empty
        End If
        If productCode.Length = 0 Then
            newProductDescription = _wfrPage.Content("NoProductCodeSpecifiedText", _languageCode, True)
        End If
        Return newProductDescription
    End Function

#End Region

End Class