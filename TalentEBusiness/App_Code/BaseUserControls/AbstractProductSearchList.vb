Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.UI

Public MustInherit Class AbstractProductSearchList
    Inherits AbstractProductList

    Protected _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Protected completeSearchProductsList As ProductListGen

    Protected requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2", _
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4", _
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6", _
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8", _
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}

    Protected emptyString As String = String.Empty

    Protected conTalent As SqlConnection = Nothing
    Protected cmdSelect As SqlCommand = Nothing
    Protected dtrSearch As SqlDataReader = Nothing

    Protected Const priceAsc As String = "priceasc", _
         priceDesc As String = "pricedesc", _
         nameAsc As String = "nameasc", _
         nameDesc As String = "namedesc", _
         bestSeller As String = "bestseller"

    Protected Overrides Sub setupUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "productSearchResultsList.ascx"
        End With
    End Sub

    Public Function NowShowingResultsString(ByVal type As String) As String
        Dim str As String = ""
        If Display Then
            Dim startAmount As Integer = 0
            Dim endAmount As Integer = 0

            Dim displaying1 As String = ucr.Content("displayingText1", _languageCode, True)
            Dim displaying2 As String = ucr.Content("displayingText2", _languageCode, True)
            Dim displaying3 As String = ucr.Content("displayingText3", _languageCode, True)
            Dim noProductsFound As String = ucr.Content("NoProductsFoundText", _languageCode, True)

            If Not completeSearchProductsList Is Nothing AndAlso completeSearchProductsList.Count > 0 Then
                startAmount = ((_PageNumber - 1) * completeSearchProductsList.PageSize) + 1
                If completeSearchProductsList.Count < (_PageNumber * completeSearchProductsList.PageSize) Then
                    endAmount = completeSearchProductsList.Count
                Else
                    endAmount = (_PageNumber * completeSearchProductsList.PageSize)
                End If
                str = displaying1 & " " & startAmount & " " & displaying2 & " " & endAmount & " " & displaying3 & " " & completeSearchProductsList.Count
            Else
                If type = "TOP" Then
                    str = noProductsFound
                Else
                    str = String.Empty
                End If
            End If
        End If


        Return str

    End Function

    Public Function PagingString(ByVal type As String) As String

        Dim str As String = ""

        If Display Then
            Dim linkPage As String = _currentPage & "?" & Request.QueryString.ToString
            Dim firstText As String = ucr.Content("firstText", _languageCode, True)
            Dim previousText As String = ucr.Content("previousText", _languageCode, True)
            Dim nextText As String = ucr.Content("nextText", _languageCode, True)
            Dim lastText As String = ucr.Content("lastText", _languageCode, True)
            Dim pageSeperator As String = ucr.Attribute("pageSeperator")
            Dim numberOfLinks As Integer = CType(ucr.Attribute("numberOfLinks"), Integer)
            Dim showFirstLast As Boolean = CType(ucr.Attribute("showFirstLast"), Boolean)
            Dim showSeperator As Boolean = CType(ucr.Attribute("showSeperator"), Boolean)
            Dim nextPreviousAsImages As Boolean = CType(ucr.Attribute("nextPreviousAsImages"), Boolean)
            Dim previousImage As String = ucr.Attribute("previousImage")
            Dim nextImage As String = ucr.Attribute("nextImage")

            If linkPage.Contains("&page=") Then
                linkPage = linkPage.Substring(0, linkPage.IndexOf("&page"))
            End If
            '----------------------------------------------------------------
            'If there is only one page of data, don't display any paging info
            '----------------------------------------------------------------

            If Not completeSearchProductsList Is Nothing Then

                If completeSearchProductsList.Count <= completeSearchProductsList.PageSize Then
                    Return ""
                End If
                '--------------------------------------------------
                'Is this a valid page for the amount of order lines
                '--------------------------------------------------
                If _PageNumber > completeSearchProductsList.NumberOfPages Then
                    '----------------------------------------------------------------------
                    'The requested page number is to high for the amount of pages available
                    'Re-set the page to page 1.
                    '----------------------------------------------------------------------
                    _PageNumber = 1
                End If
                '--------------------------------------------
                'Display the 'First' Link and the 'Next' Link
                '--------------------------------------------
                If _PageNumber > 1 Then
                    If showFirstLast Then
                        str = str & "<a href=""" & linkPage & "&page=1"">" & firstText & "</a> "
                    End If
                    If nextPreviousAsImages Then
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & """>" & "<img src=""" & previousImage & """>" & "</a> "
                    Else
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & """>" & previousText & "</a> "
                    End If
                End If
                '-----------------------------------------------------------------------------------------------
                'Display the page links
                'No more than number_of_links Links should be displayed
                'Note: if number_of_links is an ever number and the current pagenumber is greater
                'than number_of_links then the the actual number of links displayed will be number_of_links + 1
                'this is ensure that the current page appears in the centre.
                '-----------------------------------------------------------------------------------------------
                Dim counter As Integer = 1
                If completeSearchProductsList.NumberOfPages <= numberOfLinks Then
                    '----------------------------------------------------------
                    'List out the pages, current page does not have a hyperlink
                    '----------------------------------------------------------
                    For counter = 1 To completeSearchProductsList.NumberOfPages
                        If counter = _PageNumber Then
                            If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " " & counter & " "
                            Else
                                str = str & " " & counter & " " & pageSeperator & " "
                            End If
                        Else
                            If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                            Else
                                str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                            End If
                        End If
                    Next
                Else
                    '------------------------------------------------------------------------------
                    'If the current page is greater than number_of_links then the current page will
                    'be shown in the centre of the list of links.
                    '------------------------------------------------------------------------------
                    If _PageNumber <= numberOfLinks Then
                        For counter = 1 To numberOfLinks
                            If counter = _PageNumber Then
                                If counter = numberOfLinks Or Not showSeperator Then
                                    str = str & " " & counter & " "
                                Else
                                    str = str & " " & counter & " " & pageSeperator & " "
                                End If
                            Else
                                If counter = numberOfLinks Or Not showSeperator Then
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                                Else
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                                End If
                            End If
                        Next
                    ElseIf _PageNumber > (completeSearchProductsList.NumberOfPages - numberOfLinks) Then
                        '--------------------------------------------------
                        'display the last 'number_of_links number' of links
                        '--------------------------------------------------
                        For counter = (completeSearchProductsList.NumberOfPages - numberOfLinks) To completeSearchProductsList.NumberOfPages
                            If counter = _PageNumber Then
                                If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                    str = str & " " & counter & " "
                                Else
                                    str = str & " " & counter & " " & pageSeperator & " "
                                End If
                            Else
                                If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                                Else
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                                End If
                            End If
                        Next
                    Else
                        Dim halfWay As Integer = numberOfLinks / 2
                        For counter = 1 To halfWay - 1
                            str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & """>" & (_PageNumber - halfWay + counter) & "</a> "
                        Next
                        str = str & " " & _PageNumber & " "
                        For counter = 1 To halfWay
                            If counter = halfWay Or Not showSeperator Then
                                str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & """>" & (_PageNumber + counter) & "</a> "
                            Else
                                str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & """>" & (_PageNumber + counter) & "</a> " & " " & pageSeperator
                            End If
                        Next
                    End If
                End If
                '-------------------------------------------
                'Display the 'Next' link and the 'Last' link
                '-------------------------------------------
                If _PageNumber < completeSearchProductsList.NumberOfPages Then
                    If nextPreviousAsImages Then
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & """>" & "<img src=""" & nextImage & """>" & "</a> "
                    Else
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & """>" & nextText & "</a> "
                    End If
                    If showFirstLast Then
                        str = str & "<a href=""" & linkPage & "&page=" & completeSearchProductsList.NumberOfPages & """>" & lastText & "</a>"
                    End If
                End If
            End If
        End If


        Return str

    End Function

    Function IsValidID(ByVal strID As String) As Boolean
        Dim intID As Int32
        Try
            intID = Int32.Parse(strID)
        Catch
            Return False
        End Try
        '----------------------
        ' don't allow negatives
        '----------------------
        If intID < 0 Then
            Return False
        End If
        Return True
    End Function

    Protected MustOverride Sub displayProductsList(ByVal pageProductsList As IList)

    Protected Sub LoadProducts()
        Dim productManager As ProductManager = New ProductManager()

        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()
        Dim intMaxNoOfGroupLevels As Integer = def.NumberOfGroupLevels

        Dim filePath As String = String.Empty
        Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString

        If Not _IsPaging Or Session("completeSearchProductsList") Is Nothing Then
            productManager.loadProducts(queryString)
            completeSearchProductsList = productManager.getAsProductListGen()
            completeSearchProductsList.PageSize = CType(ucr.Attribute("PageSize"), Integer)

            If Session("completeSearchProductsList") Is Nothing Then
                Session.Add("completeSearchProductsList", completeSearchProductsList)
            Else
                Session.Remove("completeSearchProductsList")
                Session.Add("completeSearchProductsList", completeSearchProductsList)
            End If
        End If

        Dim pageProductsList As IList = getPageProductsList(queryString)
        displayProductsList(pageProductsList)

    End Sub

    Protected MustOverride Sub setSearchResultsLabelText(ByVal text As String)

    Protected MustOverride Function getOrderDropDownListSelectedValue() As String

    Protected Function getPageProductsList(ByVal queryString As NameValueCollection) As IList
        Dim pageProductsList As IList = Nothing
        ' -----------------------------------------------------------
        ' Retrieve the complete products list from the session and fill a 
        ' page from the complete products list
        ' ------------------------------------------------------------
        completeSearchProductsList = CType(Session("completeSearchProductsList"), ProductListGen)
        '-----------------------------------------------
        ' If from quick search then show search criteria
        '-----------------------------------------------
        setSearchResultsLabelText(String.Empty)
        Dim sbSearchString As New StringBuilder

        If Request("source") = "quick" Then
            Dim i As Integer
            i = 1
            While Not queryString.Item("keyword" & i.ToString) Is Nothing
                sbSearchString.Append(queryString.Item("keyword" & i.ToString))
                sbSearchString.Append(" ")
                i += 1
            End While

            If Not completeSearchProductsList Is Nothing Then
                setSearchResultsLabelText(ucr.Content("YourSearchReturnedText1", _languageCode, True) & _
                                              sbSearchString.ToString.Trim & _
                                             ucr.Content("YourSearchReturnedText2", _languageCode, True) & _
                                             completeSearchProductsList.Count & _
                                             ucr.Content("YourSearchReturnedText3", _languageCode, True))
            End If
        End If

        If Not completeSearchProductsList Is Nothing Then

            'order the product list
            Select Case getOrderDropDownListSelectedValue()
                Case Is = bestSeller
                    completeSearchProductsList.SortProductsByBestSeller("A")
                Case Is = priceAsc
                    completeSearchProductsList.SortProductsByPrice("A")
                Case Is = priceDesc
                    completeSearchProductsList.SortProductsByPrice("D")
                Case Is = nameAsc
                    completeSearchProductsList.SortProductsByName("A")
                Case Is = nameDesc
                    completeSearchProductsList.SortProductsByName("D")
            End Select

            If ShowAllItems() Then
                pageProductsList = completeSearchProductsList.Products
            Else
                pageProductsList = completeSearchProductsList.GetPageProducts(_PageNumber)
            End If
        End If
        Return pageProductsList
    End Function

End Class
