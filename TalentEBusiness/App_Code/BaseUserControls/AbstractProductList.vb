Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce

''' <summary>
''' Abstract class to contain common code for all product lists
''' 
''' TODO - Jason Courcoux 13/05/09 - We should pull out all common code
''' from the product list user controls and place into here.  eg the 
''' loadProduct subroutine
''' 
''' </summary>
Public MustInherit Class AbstractProductList
    Inherits AbstractTalentUserControl

#Region "VARIABLES"

    Protected _showAllProductsSet As Boolean = False
    Protected _showAllProducts As Boolean = False
    Protected sharedUcr As New Talent.Common.UserControlResource
    Protected _usage As String = Talent.Common.Utilities.GetAllString
    Protected _showAllButtonText As String = ""
    Protected _display As Boolean = True
    Protected _IsPaging As Boolean = False
    Protected _hasRows As Boolean = False
    Protected _caching As Boolean = True
    Protected _cacheTimeMinutes As Integer = 30
    Protected _PageNumber As Integer = 1
    Protected pageLevel() As String = {"browse01.aspx", "browse02.aspx", "browse03.aspx", "browse04.aspx", "browse05.aspx", _
                                     "browse06.aspx", "browse07.aspx", "browse08.aspx", "browse09.aspx", "browse10.aspx"}

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#End Region

#Region "PROPERTIES"

    ''' <summary>
    ''' Flag to determine whether we want to show all
    ''' products, or a paged view.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ShowAllItems() As Boolean
        Get

            If Session("ShowAll_Selected") Is Nothing Then
                If Not String.IsNullOrEmpty(ucr.Attribute("ShowAll")) Then
                    Session("ShowAll_Selected") = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ShowAll"))
                Else
                    Session("ShowAll_Selected") = False
                End If
            End If
            Return CBool(Session("ShowAll_Selected"))
        End Get
    End Property

    Public ReadOnly Property HasRows() As Boolean
        Get
            Return _HasRows
        End Get
    End Property


    ''' <summary>
    ''' Text to show on the show all button, this will change
    ''' depending whether we are currently showing all the products
    ''' or are showing them in paged mode.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowAllButtonText() As String
        Get
            Return _showAllButtonText
        End Get
        Set(ByVal value As String)
            _showAllButtonText = value
        End Set
    End Property

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property
    Public Property PageNumber() As Integer
        Get
            Return _PageNumber
        End Get
        Set(ByVal i As Integer)
            _PageNumber = i
        End Set
    End Property

    Public WriteOnly Property IsPaging() As Boolean
        Set(ByVal value As Boolean)
            _IsPaging = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Sets up the user control resource used by this class.  This should be called from
    ''' the page load subroutine from any child class.  As in:-
    '''   MyBase.Page_Load( sender, e )
    '''
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        configureSharedUCR()
        SetUpShowAllButtonText()
    End Sub

    ''' <summary>
    ''' Determines whether the showAll parameter is set in the request, and sets the button text
    ''' to be correct.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetUpShowAllButtonText()

        If ShowAllItems Then
            ShowAllButtonText = sharedUcr.Content("ShowPagedButtonText", _languageCode, True)
        Else
            ShowAllButtonText = sharedUcr.Content("ShowAllButtonText", _languageCode, True)
        End If

    End Sub

    ''' <summary>
    ''' Sets up the shared user control resource to be used accross all types of product list.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub configureSharedUCR()
        With sharedUcr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "abstractProductList.ascx"
        End With
    End Sub

    ''' <summary>
    ''' This sub routine should be called when ever the showall/show paginated button is clicked. It just
    ''' redirects the user to the same page, but adds the showAll flag into the request to determine
    ''' whether we want to show all the records.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub handleShowAllRedirect()
        If ShowAllItems Then
            Session("ShowAll_Selected") = False
        Else
            Session("ShowAll_Selected") = True
        End If
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub


    Public Function RetrieveProducts() As ProductListGen



        Dim completeProductsList As New ProductListGen
        Dim filePath As String = String.Empty
        Dim partnerParm As String = Nothing
        _hasRows = False
        '------------------------------------------------------------------------
        ' Determine whether navigation through product groups for this particular
        ' partner should be done using '*ALL' as the partner
        '------------------------------------------------------------------------
        If Talent.eCommerce.Utilities.GroupNavigationUsingAll() Then
            partnerParm = Talent.Common.Utilities.GetAllString
        Else
            partnerParm = _partner
        End If


        Dim intCount As Integer = 0
        Dim groupList As New Generic.Dictionary(Of String, String)
        Dim groupCacheKey As New StringBuilder
        While intCount <= 10
            If Not HttpContext.Current.Request("group" & intCount) Is Nothing Then
                groupList.Add("group" & intCount, HttpContext.Current.Request("group" & intCount))
                groupCacheKey.Append("group" & intCount & HttpContext.Current.Request("group" & intCount))
            Else
                groupList.Add("group" & intCount, String.Empty)
            End If
            intCount += 1
        End While
        Dim cacheKey As String = "RetrieveRetailProductList" & _businessUnit & partnerParm & groupCacheKey.ToString
        If _caching AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            completeProductsList = CType(HttpContext.Current.Cache.Item(cacheKey), ProductListGen)
        Else

            '----------------------
            ' Get Products for Page
            '----------------------
            Dim intMaxNoOfGroupLevels As Integer = ModuleDefaults.NumberOfGroupLevels
            Dim currentPageLevel As Integer = Array.IndexOf(pageLevel, Talent.eCommerce.Utilities.GetCurrentPageName())
            Dim _TDataObjects = New Talent.Common.TalentDataObjects()
            _TDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim dtResults As DataTable = _TDataObjects.ProductsSettings.GetProducts(_businessUnit, partnerParm, intMaxNoOfGroupLevels, currentPageLevel, groupList)
            If dtResults.Rows.Count > 0 Then

                Dim productCodes As New Generic.Dictionary(Of String, WebPriceProduct)
                For Each row As DataRow In dtResults.Rows
                    filePath = "" ' ImagePath.getImagePath("PRODTHUMB", dtrProducts("P4PRD").ToString.Trim, databaseImageRetrieval)
                    Dim p As New Product

                    p.Code = row("PRODUCT_CODE")
                    p.Description1 = row("PRODUCT_DESCRIPTION_1")
                    p.Description2 = row("PRODUCT_DESCRIPTION_2")
                    p.Country = row("PRODUCT_COUNTRY")
                    p.Colour = row("PRODUCT_COLOUR")
                    p.SequenceNo = row("SEQUENCE")
                    p.Group1 = row("GROUP_L01_GROUP")
                    p.Group2 = row("GROUP_L02_GROUP")
                    p.Group3 = row("GROUP_L03_GROUP")
                    p.Group4 = row("GROUP_L04_GROUP")
                    p.Group5 = row("GROUP_L05_GROUP")
                    p.Group6 = row("GROUP_L06_GROUP")
                    p.Group7 = row("GROUP_L07_GROUP")
                    p.Group8 = row("GROUP_L08_GROUP")
                    p.Group9 = row("GROUP_L09_GROUP")
                    p.Group10 = row("GROUP_L10_GROUP")
                    p.AvailableOnline = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(row("AVAILABLE_ONLINE"))

                    p.HTML_1 = Talent.eCommerce.Utilities.CheckForDBNull_String(row("PRODUCT_HTML_1"))
                    p.HTML_2 = Talent.eCommerce.Utilities.CheckForDBNull_String(row("PRODUCT_HTML_2"))
                    p.HTML_3 = Talent.eCommerce.Utilities.CheckForDBNull_String(row("PRODUCT_HTML_3"))
                    p.PackSize = Talent.eCommerce.Utilities.CheckForDBNull_String(row("PRODUCT_PACK_SIZE"))
                    p.Brand = Talent.eCommerce.Utilities.CheckForDBNull_String(row("PRODUCT_DESCRIPTION_5"))

                    '-----------------
                    ' Find thumb image
                    '-----------------                    
                    filePath = ImagePath.getImagePath("MAGICZOOM1M", p.Code, _businessUnit, _partner)
                    If filePath = ModuleDefaults.MissingImagePath Then
                        filePath = ImagePath.getImagePath("PRODMAIN", p.Code, _businessUnit, _partner)
                    End If
                    p.ImagePath = filePath
                    If p.ImagePath <> String.Empty And p.ImagePath <> ModuleDefaults.RetailMissingImagePath Then
                        p.AltText = p.Description1
                    Else
                        p.AltText = String.Empty
                    End If
                    '----------------------------
                    ' Build navigate query string
                    '----------------------------
                    Dim sbQry As New StringBuilder
                    With sbQry
                        .Append("../PagesPublic/ProductBrowse/product.aspx?group1=").Append(p.Group1)
                        If intMaxNoOfGroupLevels > 1 Then
                            .Append("&group2=").Append(p.Group2)
                        End If
                        If intMaxNoOfGroupLevels > 2 Then
                            .Append("&group3=").Append(p.Group3)
                        End If
                        If intMaxNoOfGroupLevels > 3 Then
                            .Append("&group4=").Append(p.Group4)
                        End If
                        If intMaxNoOfGroupLevels > 4 Then
                            .Append("&group5=").Append(p.Group5)
                        End If
                        If intMaxNoOfGroupLevels > 5 Then
                            .Append("&group6=").Append(p.Group6)
                        End If
                        If intMaxNoOfGroupLevels > 6 Then
                            .Append("&group7=").Append(p.Group7)
                        End If
                        If intMaxNoOfGroupLevels > 7 Then
                            .Append("&group8=").Append(p.Group8)
                        End If
                        If intMaxNoOfGroupLevels > 8 Then
                            .Append("&group9=").Append(p.Group9)
                        End If
                        If intMaxNoOfGroupLevels > 9 Then
                            .Append("&group10=").Append(p.Group10)
                        End If
                        .Append("&product=").Append(p.Code)
                    End With
                    p.NavigateURL = sbQry.ToString
                    p.LinkEnabled = Not ModuleDefaults.SuppressProductLinks
                    ''---------------------------------------------------------
                    '' Check it's not a product option - if it is don't display
                    ''---------------------------------------------------------
                    'dtProductOptions = productOptions.GetDataByProductCode(p.Code, TalentCache.GetBusinessUnit)
                    'If dtProductOptions.Rows.Count = 0 Then
                    '    completeProductsList.Add(p)
                    'End If
                    If p.AvailableOnline Then
                        '-----------------------------
                        ' Check it's not already added
                        '-----------------------------
                        If Not productCodes.ContainsKey(p.Code) Then
                            completeProductsList.Add(p)
                            productCodes.Add(p.Code, New WebPriceProduct(p.Code, 0, p.Code))
                        End If
                    End If
                Next


                Dim removeProducts As New Generic.List(Of Product)
                Dim productPrices As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
                productPrices = Talent.eCommerce.Utilities.GetWebPrices_WithPromoDetails(productCodes)

                'We don't want to get the prices for each product individually as
                'it is inefficient, so it is done in batch here
                For Each p As Product In completeProductsList.Products
                    Try
                        p.WebPrices = productPrices(p.Code)
                        If productPrices(p.Code).DisplayPrice_From > 0 Then
                            p.PriceForSorting = productPrices(p.Code).DisplayPrice_From
                        Else
                            p.PriceForSorting = productPrices(p.Code).DisplayPrice
                        End If
                    Catch ex As Exception
                        removeProducts.Add(p)
                    End Try
                Next

                If removeProducts.Count > 0 Then
                    For Each rp As Product In removeProducts
                        completeProductsList.Products.Remove(rp)
                    Next
                End If
            End If

            If _caching Then TalentCache.AddPropertyToCache(cacheKey, completeProductsList, _cacheTimeMinutes, TimeSpan.Zero, CacheItemPriority.Low)
        End If

        _hasRows = (completeProductsList.Count > 0)

        Return completeProductsList
    End Function

End Class
