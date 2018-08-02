Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports System.Text
Imports Talent.Common.Utilities
''' <summary>
''' This class holds the functionality for generating rss or atom feed xml string
''' </summary>
<Serializable()> _
Public Class TalentFeeds
    Inherits TalentBase

#Region "Class Level Fields"

    Private _feedsEntity As DEFeeds
    Private _feedXMLOutput As String = String.Empty
    Private _strHeaderXML As String = String.Empty
    Private _strItemsXML As String = String.Empty
    Private _strFooterXML As String = String.Empty
    Private _isProductsNotAvailable As Boolean = False
    Private _dicFeedsTextLang As Generic.Dictionary(Of String, String) = Nothing
    Private _dvProducts As DataView = Nothing
    Private _resultDataSet As DataSet
    Private _corporateStadiums() As String = Nothing
    Private Const PRODUCT_TYPE_ERROR As String = "ERR"
    Private Const NOGAMES As String = "NOGAMES"
    Private Const CLASSNAME As String = "TalentFeeds"
#End Region

#Region "Properties"

    Public Property FeedsEntity() As DEFeeds
        Get
            Return _feedsEntity
        End Get
        Set(ByVal value As DEFeeds)
            _feedsEntity = value
        End Set
    End Property

    Public ReadOnly Property XMLFeed() As String
        Get
            Return _feedXMLOutput
        End Get
    End Property

    Public ReadOnly Property ProductsDataView() As DataView
        Get
            Return _dvProducts
        End Get
    End Property

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Function GetXMLFeed() As ErrorObj
        Const ModuleName As String = "GetXMLFeed"
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj

        Dim cacheKey As String = ModuleName & _feedsEntity.Feed_Type & _feedsEntity.Product_Type & _feedsEntity.Product_Sub_Type & _feedsEntity.Product_Type_All_Filters

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            _feedXMLOutput = CType(HttpContext.Current.Cache.Item(cacheKey), String)
        Else
            _feedXMLOutput = GenerateXML()
            If Not String.IsNullOrWhiteSpace(_feedXMLOutput) Then
                'to make cache with file depedency as product list to get latest product in feeds
                Dim tempSettings As DESettings = Settings
                tempSettings.ModuleName = "PRODUCTLIST"
                AddItemToCache(cacheKey, _feedXMLOutput, tempSettings)
            Else
                RemoveItemFromCache(cacheKey)
            End If
        End If
        Return err
    End Function

    ''' <summary>
    ''' Get the products from the backend
    ''' </summary>
    Public Function ProductFeeds() As ErrorObj

        Const ModuleName As String = "ProductFeeds"
        TalentCommonLog(ModuleName, FeedsEntity.Feed_Type, "Talent.Common Request = DEFeeds=" & FeedsEntity.Feed_Type)
        Dim errObj As New ErrorObj
        Dim err As New ErrorObj
        'Dim cacheKey As String = ModuleName & FeedsEntity.Feed_Type & FeedsEntity.Product_Type
        Dim cacheKey As String = ModuleName & FeedsEntity.Feed_Type
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            resultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dataFeeds As New DBFeeds
            With dataFeeds
                .FeedsEntity = FeedsEntity
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    resultDataSet = .ResultDataSet
                    'to make cache with file depedency as product list to get latest product in feeds
                    Dim tempSettings As DESettings = Settings
                    tempSettings.ModuleName = "PRODUCTLIST"
                    AddItemToCache(cacheKey, resultDataSet, Settings)
                End If
                If Not err.HasError And resultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        resultDataSet = .ResultDataSet
                        'to make cache with file depedency as product list to get latest product in feeds
                        Dim tempSettings As DESettings = Settings
                        tempSettings.ModuleName = "PRODUCTLIST"
                        AddItemToCache(cacheKey, resultDataSet, Settings)
                    Else
                        RemoveItemFromCache(cacheKey)
                    End If
                End If
            End With
        End If

        'is it success check any records are available for given filter in a separate method and return it

        If Not isProductsExists(resultDataSet) Then
            _isProductsNotAvailable = True
        End If
        TalentCommonLog(ModuleName, FeedsEntity.Feed_Type & FeedsEntity.Product_Type, resultDataSet, err)

        Return err
    End Function

    Public Sub RemoveProductFeedsFromCache()
        Const ModuleName As String = "ProductFeeds"
        Dim cacheKey As String = ModuleName & FeedsEntity.Feed_Type
        RemoveItemFromCache(cacheKey)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Generates the XML by calling respective method
    ''' </summary><returns>xml string</returns>
    Private Function GenerateXML() As String
        Dim generatedXML As String = String.Empty
        Try
            If (_feedsEntity.Product_Type <> PRODUCT_TYPE_ERROR) Then
                ProductFeeds()
                If _isProductsNotAvailable Then
                    _feedsEntity.Product_Type = NOGAMES
                End If
            End If
            If GetFeedTemplate() Then
                If ReplaceCDATACode() Then
                    If ReplaceCodeByTextLang() Then
                        If PopulateItemSection() Then
                            generatedXML = _strHeaderXML & _strItemsXML & _strFooterXML
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        Return generatedXML
    End Function

    ''' <summary>
    ''' Gets the feed template from tbl_feeds_template and assign it to respective class level fields
    ''' </summary><returns></returns>
    Private Function GetFeedTemplate() As Boolean
        Dim isFound As Boolean = False
        _strHeaderXML = String.Empty
        _strItemsXML = String.Empty
        _strFooterXML = String.Empty
        Dim dtFeedTemplate As New DataTable
        Try
            TDataObjects.FeedsSettings.FeedsEntity = _feedsEntity
            dtFeedTemplate = TDataObjects.FeedsSettings.TblFeedsTemplate.GetTemplate()
            If dtFeedTemplate.Rows.Count <= 0 Then
                _feedsEntity.Product_Type = PRODUCT_TYPE_ERROR
                TDataObjects.FeedsSettings.FeedsEntity = _feedsEntity
                dtFeedTemplate = TDataObjects.FeedsSettings.TblFeedsTemplate.GetTemplate()
            End If
            If dtFeedTemplate IsNot Nothing AndAlso dtFeedTemplate.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dtFeedTemplate.Rows.Count - 1
                    Dim templateCode As String = dtFeedTemplate.Rows(rowIndex)("TEMPLATE_CODE")
                    Select Case templateCode
                        Case "HEADER"
                            _strHeaderXML = dtFeedTemplate.Rows(rowIndex)("TEMPLATE_TEXT")
                        Case "ITEMS"
                            _strItemsXML = dtFeedTemplate.Rows(rowIndex)("TEMPLATE_TEXT")
                        Case "FOOTER"
                            _strFooterXML = dtFeedTemplate.Rows(rowIndex)("TEMPLATE_TEXT")
                    End Select
                Next
                If (Not String.IsNullOrWhiteSpace(_strHeaderXML)) _
                    AndAlso (Not String.IsNullOrWhiteSpace(_strItemsXML)) _
                    AndAlso (Not String.IsNullOrWhiteSpace(_strFooterXML)) Then
                    isFound = True
                End If
            End If
        Catch ex As Exception
            Throw
        End Try

        Return isFound
    End Function

    ''' <summary>
    ''' Gets the text contents from tbl_feeds_text_lang
    ''' </summary><returns></returns>
    Private Function GetTextContents() As DataTable
        Dim dtFeedContents As New DataTable
        Try
            TDataObjects.FeedsSettings.FeedsEntity = _feedsEntity
            dtFeedContents = TDataObjects.FeedsSettings.TblFeedsTextLang.GetTextContents()
        Catch ex As Exception
            Throw
        End Try
        Return dtFeedContents
    End Function

    ''' <summary>
    ''' Gets the product type filter condition.
    ''' </summary>
    ''' <param name="ProductType">Type of the product.</param><returns></returns>
    Private Function GetProductTypeFilterCondition(ByVal ProductType As String) As String
        Dim filterCondition As String = ""
        Dim filterProductSubType As String = String.Empty
        If FeedsEntity.Product_Sub_Type.Length > 0 Then
            filterProductSubType = " AND ProductSubType = '" & FeedsEntity.Product_Sub_Type.Trim & "'"
        End If
        Select Case ProductType
            Case "H"
                filterCondition = "(ProductType='" + ProductType + "'" + filterProductSubType + " AND (ProductHomeAsAway<>'Y'))"
            Case "A"
                filterCondition = "(((ProductType='" + ProductType + "') OR (ProductType='H' AND ProductHomeAsAway='Y'))" + filterProductSubType + ")"
            Case "T"
                filterCondition = "(ProductType='" + ProductType + "'" + filterProductSubType + " AND (ProductHomeAsAway<>'Y')) "
            Case "E"
                filterCondition = "(ProductType='" + ProductType + "'" + filterProductSubType + " AND (ProductHomeAsAway<>'Y')) "
            Case "CH"
                filterCondition = "(ProductType='" + "H" + "'" + filterProductSubType + " AND (ProductHomeAsAway<>'Y'))"
            Case "S"
                filterCondition = "(ProductType='" + ProductType + "'" + filterProductSubType + " AND (ProductHomeAsAway<>'Y'))"
            Case "C"
                filterCondition = "(ProductType='" + ProductType + "'" + filterProductSubType + " AND (ProductHomeAsAway<>'Y'))"
            Case "ALL"
                filterCondition = ""
        End Select
        Return filterCondition
    End Function

    ''' <summary>
    ''' Gets the filter condition for product type ALL.
    ''' </summary><returns></returns>
    Private Function GetProductTypeAllFilterCondition() As String
        Dim filterCondition As String = ""
        If String.IsNullOrWhiteSpace(_feedsEntity.Product_Type_All_Filters) Then
            filterCondition = ""
        Else
            Dim arrProductType() As String = _feedsEntity.Product_Type_All_Filters.Split(New Char() {","c}, System.StringSplitOptions.RemoveEmptyEntries)
            For productTypeIndex As Integer = 0 To arrProductType.Length - 1
                If productTypeIndex = 0 Then
                    filterCondition = GetProductTypeFilterCondition(arrProductType(productTypeIndex))
                Else
                    filterCondition = filterCondition & " OR " & GetProductTypeFilterCondition(arrProductType(productTypeIndex))
                End If
            Next
            filterCondition = "(" & filterCondition & ")"
        End If
        Return filterCondition
    End Function

    ''' <summary>
    ''' Gets the row filter condition based on the product type and product stadium requested in feeds
    ''' </summary><returns></returns>
    Private Function GetRowFilterCondition() As String
        Dim filterCondition As String = ""
        Try
            Select Case _feedsEntity.Product_Type
                Case "ALL"
                    filterCondition = GetProductTypeAllFilterCondition()
                Case Else
                    filterCondition = GetProductTypeFilterCondition(_feedsEntity.Product_Type)
            End Select
            Dim stadiumFilter As String = FormatStadiumFilter()
            If stadiumFilter.Length > 0 Then
                If filterCondition.Length > 0 Then
                    filterCondition = filterCondition & " AND ProductStadium IN (" & stadiumFilter & ")"
                Else
                    filterCondition = filterCondition & "ProductStadium IN (" & stadiumFilter & ")"
                End If
            End If
        Catch ex As Exception
            Throw
        End Try
        Return filterCondition
    End Function

    ''' <summary>
    ''' Replaces the CDATA code with CDATA tags
    ''' </summary><returns></returns>
    Private Function ReplaceCDATACode() As Boolean
        Dim isReplaced As Boolean = False
        Try
            _strHeaderXML = _strHeaderXML.Replace("<<<CDATA_START>>>", "<![CDATA[")
            _strItemsXML = _strItemsXML.Replace("<<<CDATA_START>>>", "<![CDATA[")
            _strFooterXML = _strFooterXML.Replace("<<<CDATA_START>>>", "<![CDATA[")

            _strHeaderXML = _strHeaderXML.Replace("<<<CDATA_END>>>", "]]>")
            _strItemsXML = _strItemsXML.Replace("<<<CDATA_END>>>", "]]>")
            _strFooterXML = _strFooterXML.Replace("<<<CDATA_END>>>", "]]>")
            isReplaced = True
        Catch ex As Exception
            Throw
        End Try

        Return isReplaced
    End Function

    ''' <summary>
    ''' Replaces the code in the template using tbl_feeds_text_lang
    ''' </summary><returns></returns>
    Private Function ReplaceCodeByTextLang() As Boolean
        Dim isReplaced As Boolean = False
        Try
            Dim dtFeedTextLang As DataTable = GetTextContents()
            If dtFeedTextLang.Rows.Count > 0 Then
                Dim tempTextCode As String = String.Empty
                Dim tempTextContent As String = String.Empty
                _dicFeedsTextLang = New Dictionary(Of String, String)
                For rowIndex As Integer = 0 To dtFeedTextLang.Rows.Count - 1
                    tempTextCode = dtFeedTextLang.Rows(rowIndex)("TEXT_CODE")
                    tempTextContent = dtFeedTextLang.Rows(rowIndex)("TEXT_CONTENT")
                    PopulateDicFeedsTextLang(tempTextCode, tempTextContent)
                    tempTextCode = "<<<" & tempTextCode & ">>>"
                    _strHeaderXML = _strHeaderXML.Replace(tempTextCode, tempTextContent)
                    _strItemsXML = _strItemsXML.Replace(tempTextCode, tempTextContent)
                    _strFooterXML = _strFooterXML.Replace(tempTextCode, tempTextContent)
                    PopulateDicFeedsTextLang(tempTextCode, tempTextContent)
                    tempTextCode = String.Empty
                    tempTextContent = String.Empty
                Next
                'Replace club url with site url
                _strHeaderXML = _strHeaderXML.Replace("<<<CLUB_URL>>>", _feedsEntity.Site_Url)
                _strItemsXML = _strItemsXML.Replace("<<<CLUB_URL>>>", _feedsEntity.Site_Url)
                _strFooterXML = _strFooterXML.Replace("<<<CLUB_URL>>>", _feedsEntity.Site_Url)
                PopulateDicFeedsTextLang("CLUB_URL", _feedsEntity.Site_Url)

                isReplaced = True
            End If

        Catch ex As Exception
            Throw
        End Try

        Return isReplaced
    End Function

    ''' <summary>
    ''' Populates the item section using products data
    ''' </summary><returns></returns>
    Private Function PopulateItemSection() As Boolean
        Dim isPopulated As Boolean = False
        Try
            If _dvProducts IsNot Nothing AndAlso _dvProducts.Count > 0 AndAlso _feedsEntity.Product_Type <> PRODUCT_TYPE_ERROR Then
                Dim sbItems As New StringBuilder
                Dim tempItemSection As String = String.Empty
                Dim uniqueID As Integer = 1000
                For Each rowView As DataRowView In _dvProducts
                    tempItemSection = GetItemSectionByProductType(rowView)
                    tempItemSection = tempItemSection.Replace("{{{UNIQUE_ID}}}", uniqueID.ToString())
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_DESCRIPTION}}}", CheckForDBNull_String(rowView("ProductDescription")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{OPPOSITION_IMAGE}}}", GetImageAbsolutePath(rowView("ProductOppositionCode"), _dicFeedsTextLang("<<<OPPOSITION_IMAGE_PATH>>>")))
                    tempItemSection = tempItemSection.Replace("{{{COMPETITION}}}", CheckForDBNull_String(rowView("ProductCompetitionCode")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_YEAR}}}", CheckForDBNull_String(rowView("ProductYear")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_DATE}}}", CheckForDBNull_String(rowView("ProductDate")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_TIME}}}", CheckForDBNull_String(rowView("ProductTime")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_STADIUM}}}", CheckForDBNull_String(rowView("ProductStadium")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_CODE}}}", CheckForDBNull_String(rowView("ProductCode")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{CAMPAIGN_CODE}}}", CheckForDBNull_String(rowView("CampaignCode")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_TYPE}}}", CheckForDBNull_String(rowView("ProductType")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{PRODUCT_SUB_TYPE}}}", CheckForDBNull_String(rowView("ProductSubType")).Trim)
                    tempItemSection = tempItemSection.Replace("{{{COMPETITION_DESC}}}", CheckForDBNull_String(rowView("ProductCompetitionDesc")).Trim)
                    sbItems.Append(tempItemSection)
                    uniqueID += 10
                Next
                _strItemsXML = sbItems.ToString()
                isPopulated = True
            Else
                isPopulated = True
            End If
        Catch ex As Exception
            Throw
        End Try
        Return isPopulated
    End Function

    ''' <summary>
    ''' Gets the type of the item section by product.
    ''' </summary>
    ''' <param name="productRowView">The product row view.</param><returns></returns>
    Private Function GetItemSectionByProductType(ByVal productRowView As DataRowView) As String
        Dim tempItemSection As String = _strItemsXML
        Try
            Select Case CheckForDBNull_String(productRowView("ProductType")).Trim.ToUpper()
                Case "H"
                    If IsHospitality(CheckForDBNull_String(productRowView("ProductStadium")).Trim.ToUpper()) Then
                        tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("HOSPITALITY_DESC"))
                        tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("HOSPITALITY_LINK"))
                    Else
                        tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("HOME_GAMES_DESC"))
                        tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("HOME_GAMES_LINK"))
                    End If
                Case "A"
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("AWAY_GAMES_DESC"))
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("AWAY_GAMES_LINK"))
                Case "S"
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("SEASON_TICKET_DESC"))
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("SEASON_TICKET_LINK"))
                Case "C"
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("MEMBERSHIP_DESC"))
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("MEMBERSHIP_LINK"))
                Case "T"
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("TRAVEL_DESC"))
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("TRAVEL_LINK"))
                Case "E"
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", _dicFeedsTextLang("EVENTS_DESC"))
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", _dicFeedsTextLang("CLUB_URL") & _dicFeedsTextLang("EVENTS_LINK"))
                Case Else
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_DESC}}}", "")
                    tempItemSection = tempItemSection.Replace("{{{ALL_ITEM_PRODUCT_LINK}}}", "")
            End Select
        Catch ex As Exception

        End Try

        Return tempItemSection
    End Function

    Private Sub PopulateDicFeedsTextLang(ByVal textCode As String, ByVal textContent As String)
        If Not _dicFeedsTextLang.ContainsKey(textCode) Then
            _dicFeedsTextLang.Add(textCode, textContent)
        End If
    End Sub

    Private Function GetImageAbsolutePath(ByVal imageName As String, ByVal imagePath As String) As String
        Dim imageAbsolutePath As String = ""
        imageName = CheckForDBNull_String(imageName).Trim
        If Not String.IsNullOrWhiteSpace(imageName) Then
            imageAbsolutePath = "<img src=""" & _dicFeedsTextLang("AKAMAI_PATH") & imagePath & imageName & _dicFeedsTextLang("IMAGE_EXTENSION") & """ alt=""" & imageName & """ />"
        End If
        Return imageAbsolutePath
    End Function

    ''' <summary>
    ''' Determines whether [is products exists] [the specified result data set] for given product type
    ''' </summary>
    ''' <param name="resultDataSet">The result data set.</param><returns>
    '''   <c>true</c> if [is products exists] [the specified result data set]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function isProductsExists(ByVal resultDataSet As DataSet) As Boolean
        Dim isAvailable As Boolean = False
        If ((resultDataSet IsNot Nothing) AndAlso (resultDataSet.Tables.Count > 1) AndAlso (resultDataSet.Tables("FeedsProduct").Rows.Count > 0)) Then
            Dim filterCondition As String = GetRowFilterCondition()
            If filterCondition.Length > 0 Then
                resultDataSet.Tables("FeedsProduct").DefaultView.RowFilter = filterCondition
                _dvProducts = resultDataSet.Tables("FeedsProduct").DefaultView
                If _dvProducts.Count > 0 Then
                    _dvProducts.Sort = "ProductDateYear"
                    isAvailable = True
                End If
            Else
                _dvProducts = resultDataSet.Tables("FeedsProduct").DefaultView
                isAvailable = True
            End If
        End If
        Return isAvailable
    End Function

    ''' <summary>
    ''' Determines whether the specified product stadium is hospitality.
    ''' </summary>
    ''' <param name="productStadium">The product stadium.</param><returns>
    '''   <c>true</c> if the specified product stadium is hospitality; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsHospitality(ByVal productStadium As String) As Boolean
        Dim isHospitalityProduct As Boolean = False
        If _corporateStadiums IsNot Nothing Then
            For arrayIndex As Integer = 0 To _corporateStadiums.Length - 1
                If _corporateStadiums(arrayIndex).ToUpper() = productStadium Then
                    isHospitalityProduct = True
                    Exit For
                End If
            Next
        End If
        Return isHospitalityProduct
    End Function

    ''' <summary>
    ''' Formats the stadium filter.
    ''' </summary><returns></returns>
    Private Function FormatStadiumFilter() As String
        Dim formattedSTString As String = ""
        Dim formattedCorporateStadiumFilter As String = FormatCorporateStadiumFilter()
        Select Case _feedsEntity.Product_Type
            Case "CH"
                formattedSTString = formattedCorporateStadiumFilter
            Case "ALL"
                formattedSTString = ""
            Case Else
                formattedSTString = FormatTicketingStadiumFilter() & "''"
        End Select
        Return formattedSTString
    End Function

    ''' <summary>
    ''' Formats the ticketing stadium filter.
    ''' </summary><returns></returns>
    Private Function FormatTicketingStadiumFilter() As String
        Dim formattedSTString As String = ""
        If Not String.IsNullOrEmpty(_feedsEntity.Ticketing_Stadium) Then
            Dim tempArray() As String = Nothing
            tempArray = _feedsEntity.Ticketing_Stadium.Split(",")
            For arrayIndex As Integer = 0 To tempArray.Length - 1
                formattedSTString = formattedSTString & "'" & tempArray(arrayIndex) & "',"
            Next
        End If
        Return formattedSTString
    End Function

    ''' <summary>
    ''' Formats the corporate stadium filter.
    ''' </summary><returns></returns>
    Private Function FormatCorporateStadiumFilter() As String
        Dim formattedSTString As String = ""
        If Not String.IsNullOrWhiteSpace(_feedsEntity.Corporate_Stadium) Then
            _corporateStadiums = _feedsEntity.Corporate_Stadium.Split(",")
            For arrayIndex As Integer = 0 To _corporateStadiums.Length - 1
                formattedSTString = formattedSTString & "'" & _corporateStadiums(arrayIndex) & "',"
            Next
        End If
        Return formattedSTString
    End Function

#End Region

End Class
