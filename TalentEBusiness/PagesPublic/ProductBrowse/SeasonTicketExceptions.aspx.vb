Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Data

Partial Class PagesPublic_ProductBrowse_SeasonTicketExceptions
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = String.Empty
    Private _dtExceptionSeatDetails As DataTable = Nothing
    Private _seasonTicketProductCode As String = String.Empty
    Private _seasonTicketCampaignCode As String = String.Empty
    Private _seasonTicketProductSubType As String = String.Empty
    Private _stadiumCode As String = String.Empty
    Private _tempNoAvailabilityFlag As Boolean = False

#End Region

#Region "Public Properties"

    Public SeasonTicketSeatColumnHeading As String
    Public ExceptionsColumnHeading As String
    Public ExceptionSeatColumnHeading As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _wfrPage = New WebFormResource
        _languageCode = TCUtilities.GetDefaultLanguage()
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = TEBUtilities.GetCurrentPageName()
            .PageCode = TEBUtilities.GetCurrentPageName()
        End With
        If Not Profile.IsAnonymous AndAlso Profile.Basket.BasketItems.Count > 0 AndAlso bindRepeaters() Then
            setPageText()
        Else
            If Profile.Basket.BasketItems.Count = 0 Then
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            Else
                rptSeasonTicketSeats.Visible = False
                rptExceptionProducts.Visible = False
                btnBasket.Visible = False
                uscBasketSummary.Visible = False
                ltlErrorMessage.Text = _wfrPage.Content("GenericExceptionsProcessingError", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (ltlErrorMessage.Text.Length > 0)
    End Sub

    Protected Sub rptSeasonTicketSeats_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptSeasonTicketSeats.ItemDataBound
        Select Case e.Item.ItemType
            Case ListItemType.Header
                Dim STEventImage As Image = CType(e.Item.FindControl("STEventImage"), Image)
                Dim ltlSTDescription1 As Literal = CType(e.Item.FindControl("ltlSTDescription1"), Literal)
                Dim ltlSTDescription2 As Literal = CType(e.Item.FindControl("ltlSTDescription2"), Literal)
                Dim plhSTDescription1 As PlaceHolder = CType(e.Item.FindControl("plhSTDescription1"), PlaceHolder)
                Dim plhSTDescription2 As PlaceHolder = CType(e.Item.FindControl("plhSTDescription2"), PlaceHolder)
                Dim seasonTicketProductDetails As DataTable = getSeasonTicketProductDetails()

                If seasonTicketProductDetails.Rows.Count > 0 Then
                    ltlSTDescription1.Text = TEBUtilities.CheckForDBNull_String(seasonTicketProductDetails.Rows(0)("ProductDescription"))
                    ltlSTDescription2.Text = TEBUtilities.CheckForDBNull_String(seasonTicketProductDetails.Rows(0)("ProductText1"))
                    STEventImage.ImageUrl = ImagePath.getImagePath("PRODCOMPETITION", TEBUtilities.CheckForDBNull_String(seasonTicketProductDetails.Rows(0)("Competition")), _wfrPage.BusinessUnit, TalentCache.GetPartner(Profile))
                    STEventImage.Visible = (STEventImage.ImageUrl <> ModuleDefaults.MissingImagePath)
                Else
                    STEventImage.Visible = False
                End If
                plhSTDescription1.Visible = (ltlSTDescription1.Text.Length > 0)
                plhSTDescription2.Visible = (ltlSTDescription2.Text.Length > 0)

            Case ListItemType.AlternatingItem, ListItemType.Item
                Dim basketItem As TalentBasketItem = CType(e.Item.DataItem, TalentBasketItem)
                Dim ltlSeasonTicketSeat As Literal = CType(e.Item.FindControl("ltlSeasonTicketSeat"), Literal)
                Dim ltlSeasonTicketSeatRestriction As Literal = CType(e.Item.FindControl("ltlSeasonTicketSeatRestriction"), Literal)
                Dim plhExceptionCount As PlaceHolder = CType(e.Item.FindControl("plhExceptionCount"), PlaceHolder)
                Dim ltlExceptionCount As Literal = CType(e.Item.FindControl("ltlExceptionCount"), Literal)
                Dim ltlNoExceptions As Literal = CType(e.Item.FindControl("ltlNoExceptions"), Literal)
                Dim rptDropDownExceptionGames As Repeater = CType(e.Item.FindControl("rptDropDownExceptionGames"), Repeater)
                Dim dvExSeatsForCurrentProduct As DataView = _dtExceptionSeatDetails.DefaultView
                Dim seasonTicketSeat As New DESeatDetails

                dvExSeatsForCurrentProduct.RowFilter = "SEASON_TICKET_SEAT = '" & basketItem.SEAT & "'"
                If dvExSeatsForCurrentProduct.Count > 0 Then
                    rptDropDownExceptionGames.DataSource = dvExSeatsForCurrentProduct
                    rptDropDownExceptionGames.DataBind()
                    plhExceptionCount.Visible = True
                    ltlExceptionCount.Text = _wfrPage.Content("ExceptionsOnCurrentSeatText", _languageCode, True).Replace("<<EXCEPTION_COUNT>>", dvExSeatsForCurrentProduct.Count)
                Else
                    plhExceptionCount.Visible = False
                    ltlNoExceptions.Text = _wfrPage.Content("NoExceptionSeatsText", _languageCode, True)
                End If

                seasonTicketSeat.Stand = basketItem.SEAT.Substring(0, 3)
                seasonTicketSeat.Area = basketItem.SEAT.Substring(3, 4)
                seasonTicketSeat.Row = basketItem.SEAT.Substring(7, 4)
                seasonTicketSeat.Seat = basketItem.SEAT.Substring(11, 5)
                ltlSeasonTicketSeat.Text = formatSeatString(seasonTicketSeat)
                ltlSeasonTicketSeatRestriction.Text = getSTSeatRestrictionDescription(seasonTicketSeat.UnFormattedSeat, _seasonTicketProductCode)
        End Select
    End Sub

    Protected Sub rptExceptionProducts_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptExceptionProducts.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim rptExceptionSeats As Repeater = CType(e.Item.FindControl("rptExceptionSeats"), Repeater)
            Dim EventImage As Image = CType(e.Item.FindControl("EventImage"), Image)
            Dim ltlDescription1 As Literal = CType(e.Item.FindControl("ltlDescription1"), Literal)
            Dim ltlDescription2 As Literal = CType(e.Item.FindControl("ltlDescription2"), Literal)
            Dim ltlProductDate As Literal = CType(e.Item.FindControl("ltlProductDate"), Literal)
            Dim ltlProductTime As Literal = CType(e.Item.FindControl("ltlProductTime"), Literal)
            Dim plhDescription1 As PlaceHolder = CType(e.Item.FindControl("plhDescription1"), PlaceHolder)
            Dim plhDescription2 As PlaceHolder = CType(e.Item.FindControl("plhDescription2"), PlaceHolder)
            Dim plhProductDate As PlaceHolder = CType(e.Item.FindControl("plhProductDate"), PlaceHolder)
            Dim plhProductTime As PlaceHolder = CType(e.Item.FindControl("plhProductTime"), PlaceHolder)
            Dim plhNoAvailability As PlaceHolder = CType(e.Item.FindControl("plhNoAvailability"), PlaceHolder)
            Dim ltlNoAvailability As Literal = CType(e.Item.FindControl("ltlNoAvailability"), Literal)

            'bind the repeater header information
            ltlDescription1.Text = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_DESCRIPTION1"))
            ltlDescription2.Text = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_DESCRIPTION2"))
            ltlProductDate.Text = TEBUtilities.GetFormattedDateAndTime(TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_DATE")), String.Empty, _wfrPage.Content("DateSeparator", _languageCode, True), ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
            ltlProductTime.Text = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_TIME"))
            plhDescription1.Visible = (ltlDescription1.Text.Length > 0)
            plhDescription2.Visible = (ltlDescription2.Text.Length > 0)
            plhProductDate.Visible = (ltlProductDate.Text.Length > 0)
            plhProductTime.Visible = (ltlProductTime.Text.Length > 0)
            EventImage.ImageUrl = ImagePath.getImagePath("PRODTICKETING", TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_OPPOSITION_CODE")), _wfrPage.BusinessUnit, TalentCache.GetPartner(Profile))
            EventImage.Visible = (EventImage.ImageUrl <> ModuleDefaults.MissingImagePath)

            'bind the repeater exception seats details
            Dim dvExSeatsForCurrentProduct As DataView = _dtExceptionSeatDetails.DefaultView
            dvExSeatsForCurrentProduct.Sort = "BASKET_DETAIL_EXCEPTIONS_ID DESC"
            dvExSeatsForCurrentProduct.RowFilter = "PRODUCT_CODE = '" & TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_CODE")) & "'"
            rptExceptionSeats.DataSource = dvExSeatsForCurrentProduct
            rptExceptionSeats.DataBind()

            'has exception seats caused - keep all seats togther option
            Dim plhKeepSeatsTogether As PlaceHolder = CType(e.Item.FindControl("plhKeepSeatsTogether"), PlaceHolder)
            Dim lblKeepSeatsTogether As Label = CType(e.Item.FindControl("lblKeepSeatsTogether"), Label)
            Dim btnKeepSeatsTogether As Button = CType(e.Item.FindControl("btnKeepSeatsTogether"), Button)
            Dim hdfKeepSeatsTogether As HiddenField = CType(e.Item.FindControl("hdfKeepSeatsTogether"), HiddenField)
            If rptSeasonTicketSeats.Items.Count > dvExSeatsForCurrentProduct.Count Then
                Dim hasUnallocatedSeat As Boolean = False
                For Each row As DataRow In dvExSeatsForCurrentProduct.ToTable.Rows
                    If TEBUtilities.CheckForDBNull_String(row("SEAT")).Trim().Length = 0 Then
                        hasUnallocatedSeat = True
                        Exit For
                    End If
                Next
                If hasUnallocatedSeat Then
                    plhKeepSeatsTogether.Visible = False
                Else
                    plhKeepSeatsTogether.Visible = True
                    lblKeepSeatsTogether.Text = _wfrPage.Content("KeepSeatsTogetherLabelText", _languageCode, True)
                    btnKeepSeatsTogether.Text = _wfrPage.Content("KeepSeatsTogetherButtonText", _languageCode, True)
                    btnKeepSeatsTogether.CommandArgument = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_CODE"))
                    hdfKeepSeatsTogether.Value = TEBUtilities.CheckForDBNull_String(dvExSeatsForCurrentProduct(0)("SEAT"))
                End If
            Else
                plhKeepSeatsTogether.Visible = False
            End If

            'If the game has no availability show the availability message
            If _tempNoAvailabilityFlag Then
                plhNoAvailability.Visible = True
                ltlNoAvailability.Text = _wfrPage.Content("NoAvailabilityMessage", _languageCode, True)
            Else
                plhNoAvailability.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptExceptionSeats_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim row As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim ltlSeasonTicketSeatForExceptionProduct As Literal = CType(e.Item.FindControl("ltlSeasonTicketSeatForExceptionProduct"), Literal)
            Dim ltlExceptionSeatForExceptionProduct As Literal = CType(e.Item.FindControl("ltlExceptionSeatForExceptionProduct"), Literal)
            Dim ltlSTSeatRestrictionForExceptionProduct As Literal = CType(e.Item.FindControl("ltlSTSeatRestrictionForExceptionProduct"), Literal)
            Dim ltlExSeatRestrictionForExceptionProduct As Literal = CType(e.Item.FindControl("ltlExSeatRestrictionForExceptionProduct"), Literal)
            Dim hdfExceptionSeatForExceptionProduct As HiddenField = CType(e.Item.FindControl("hdfExceptionSeatForExceptionProduct"), HiddenField)
            Dim hplChangeSeat As HyperLink = CType(e.Item.FindControl("hplChangeSeat"), HyperLink)
            Dim hplPickSeat As HyperLink = CType(e.Item.FindControl("hplPickSeat"), HyperLink)
            Dim btnRemove As Button = CType(e.Item.FindControl("btnRemove"), Button)
            Dim visualSeatSelctionUrl As String = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&oldseat={5}&stand={6}&area={7}"
            Dim productCode As String = TEBUtilities.CheckForDBNull_String(row("PRODUCT_CODE"))
            Dim exceptionSeat As New DESeatDetails
            Dim seasonTicketSeat As New DESeatDetails
            Dim fullExceptionSeatDetails As String = TEBUtilities.CheckForDBNull_String(row("SEAT"))
            Dim fullSeasonTicketSeatDetails As String = TEBUtilities.CheckForDBNull_String(row("SEASON_TICKET_SEAT"))
            _seasonTicketProductSubType = TEBUtilities.CheckForDBNull_String(row("PRODUCT_SUB_TYPE"))
            _tempNoAvailabilityFlag = False

            seasonTicketSeat.Stand = fullSeasonTicketSeatDetails.Substring(0, 3)
            seasonTicketSeat.Area = fullSeasonTicketSeatDetails.Substring(3, 4)
            seasonTicketSeat.Row = fullSeasonTicketSeatDetails.Substring(7, 4)
            seasonTicketSeat.Seat = fullSeasonTicketSeatDetails.Substring(11, 5)
            ltlSeasonTicketSeatForExceptionProduct.Text = formatSeatString(seasonTicketSeat)
            ltlSTSeatRestrictionForExceptionProduct.Text = getSTSeatRestrictionDescription(seasonTicketSeat.UnFormattedSeat, _seasonTicketProductCode)

            If fullExceptionSeatDetails.Trim().Length = 0 Then
                hplChangeSeat.Visible = False
                hplPickSeat.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(row("HAS_AVAILABILITY"))
                btnRemove.Visible = False
                ltlExceptionSeatForExceptionProduct.Text = _wfrPage.Content("UnallocatedSeatText", _languageCode, True)
                ltlExSeatRestrictionForExceptionProduct.Visible = False
                If hplPickSeat.Visible Then
                    visualSeatSelctionUrl = String.Format(visualSeatSelctionUrl, _stadiumCode, productCode, String.Empty, GlobalConstants.HOMEPRODUCTTYPE, _seasonTicketProductSubType, GlobalConstants.ST_EXCCEPTION_UNALLOCATED_SEAT, String.Empty, String.Empty)
                    hplPickSeat.NavigateUrl = visualSeatSelctionUrl
                    hplPickSeat.Text = _wfrPage.Content("PickASeatText", _languageCode, True)
                Else
                    _tempNoAvailabilityFlag = True
                End If
            Else
                hplChangeSeat.Visible = True
                hplPickSeat.Visible = False
                btnRemove.Visible = True
                exceptionSeat.Stand = fullExceptionSeatDetails.Substring(0, 3)
                exceptionSeat.Area = fullExceptionSeatDetails.Substring(3, 4)
                exceptionSeat.Row = fullExceptionSeatDetails.Substring(7, 4)
                exceptionSeat.Seat = fullExceptionSeatDetails.Substring(11, 5)
                ltlExceptionSeatForExceptionProduct.Text = formatSeatString(exceptionSeat)
                ltlExSeatRestrictionForExceptionProduct.Text = TEBUtilities.RestrictedSeatDescription(TEBUtilities.CheckForDBNull_String(row("RESTRICTION_CODE")))
                hdfExceptionSeatForExceptionProduct.Value = exceptionSeat.FormattedSeat
                visualSeatSelctionUrl = String.Format(visualSeatSelctionUrl, _stadiumCode, productCode, String.Empty, GlobalConstants.HOMEPRODUCTTYPE, _seasonTicketProductSubType, exceptionSeat.FormattedSeat, exceptionSeat.Stand.Trim(), exceptionSeat.Area.Trim())
                hplChangeSeat.NavigateUrl = visualSeatSelctionUrl
                hplChangeSeat.Text = _wfrPage.Content("ChangeSeatText", _languageCode, True)
                btnRemove.CommandArgument = productCode
                btnRemove.Text = _wfrPage.Content("RemoveSeatText", _languageCode, True)
            End If

        End If
    End Sub

    Protected Sub rptExceptionProducts_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptExceptionProducts.ItemCommand
        If e.CommandName = "KeepSeatsTogether" Then
            Dim hdfKeepSeatsTogether As HiddenField = CType(e.Item.FindControl("hdfKeepSeatsTogether"), HiddenField)
            Dim err As New ErrorObj
            err = doExceptionReprocessing(True, False, hdfKeepSeatsTogether.Value, e.CommandArgument)
            If err.HasError Then
                handleErrors(True, 5)
                ltlErrorMessage.Text = _wfrPage.Content("ProblemKeepingSeatsTogetherText", _languageCode, True)
            Else
                bindRepeaters()
            End If
        End If
    End Sub

    Protected Sub rptExceptionSeats_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "RemoveSeat" Then
            Dim hdfExceptionSeatForExceptionProduct As HiddenField = CType(e.Item.FindControl("hdfExceptionSeatForExceptionProduct"), HiddenField)
            Dim err As New ErrorObj
            err = doExceptionReprocessing(False, True, hdfExceptionSeatForExceptionProduct.Value, e.CommandArgument)
            If err.HasError Then
                handleErrors(True, 6)
                ltlErrorMessage.Text = _wfrPage.Content("ProblemRemovingSeatText", _languageCode, True)
            Else
                bindRepeaters()
            End If
        End If
    End Sub

    Protected Sub btnBasket_Click(sender As Object, e As EventArgs) Handles btnBasket.Click
        Dim redirectUrl As String = "~/PagesPublic/Basket/Basket.aspx"
        Dim priceCode As String = String.Empty
        Dim productHasRelatedProducts As Boolean = False
        Dim productHasMandatoryRelatedProducts As Boolean = False
        Dim linkedMasterProduct As String = String.Empty

        If ModuleDefaults.PPS_ENABLE_1 Then
            redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=1&product=" & _seasonTicketProductCode & "&pricecode=" & _seasonTicketCampaignCode
        ElseIf ModuleDefaults.PPS_ENABLE_2 Then
            redirectUrl = "~/PagesPublic/ProductBrowse/TicketingPrePayments.aspx?ppspage=2&product=" & _seasonTicketProductCode & "&pricecode=" & _seasonTicketCampaignCode
        Else
            Dim tGatewayFunctions As New TicketingGatewayFunctions
            linkedMasterProduct = TDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(Profile.Basket.Basket_Header_ID)
            If Not String.IsNullOrEmpty(linkedMasterProduct) AndAlso Not Profile.Basket.DoesBasketContainProductCode(linkedMasterProduct) Then
                TDataObjects.BasketSettings.TblBasketHeader.UpdateLinkedProductMaster(Profile.Basket.Basket_Header_ID, String.Empty)
            End If
            If String.IsNullOrEmpty(linkedMasterProduct) OrElse linkedMasterProduct = _seasonTicketProductCode Then
                tGatewayFunctions.CheckForLinkedProducts(_seasonTicketProductCode, priceCode, _seasonTicketCampaignCode, _seasonTicketProductSubType, productHasRelatedProducts, productHasMandatoryRelatedProducts)
            End If
            tGatewayFunctions.ProductHasRelatedProducts = productHasRelatedProducts
            redirectUrl = tGatewayFunctions.HandleRedirect(redirectUrl, True, _seasonTicketProductCode, priceCode, _seasonTicketProductSubType, False, False, 0, linkedMasterProduct)
        End If

        Response.Redirect(redirectUrl)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the text for the various static text items
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageText()
        SeasonTicketSeatColumnHeading = _wfrPage.Content("SeasonTicketSeatColumnHeading", _languageCode, True)
        ExceptionsColumnHeading = _wfrPage.Content("ExceptionsColumnHeading", _languageCode, True)
        ExceptionSeatColumnHeading = _wfrPage.Content("ExceptionSeatColumnHeading", _languageCode, True)
        ltlExceptionsDetails.Text = _wfrPage.Content("PageDetailsText", _languageCode, True).Replace("<<EXCEPTIONS_COUNT>>", _dtExceptionSeatDetails.Rows.Count)
        btnBasket.Text = _wfrPage.Content("BasketButtonText", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Handle any errors and based on the property values that are present
    ''' </summary>
    ''' <param name="functionNumber">The function number that has caused the error</param>
    ''' <param name="hasError">Has an error occurred</param>
    ''' <param name="ex">Exception object if available</param>
    ''' <remarks></remarks>
    Private Sub handleErrors(ByVal hasError As Boolean, ByVal functionNumber As Integer, Optional ByVal ex As Exception = Nothing)
        If hasError Then
            Dim log As New TalentLogging
            log.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString()
            Dim seasonTicketExceptionsvb As String = "SeasonTicketExceptions.aspx.vb"
            Dim seasonTicketExLog As String = "SeasonTicketExceptionsLog"
            Select Case functionNumber
                Case Is = 1 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-01", "Basket has items, but no season tickets", seasonTicketExLog)
                Case Is = 2 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-02", "Has season tickets but no exception records in table", seasonTicketExLog)
                Case Is = 3 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-03", "Exception: " & ex.Message & " | Stack Trace: " & ex.StackTrace, seasonTicketExLog)
                Case Is = 4 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-04", "Exception: " & ex.Message & " | Stack Trace: " & ex.StackTrace, seasonTicketExLog)
                Case Is = 5 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-05", "Error doing the exception keeping seats together", seasonTicketExLog)
                Case Is = 6 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-06", "Error doing the exception reprocessing for removing a seat", seasonTicketExLog)
                Case Is = 7 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-07", "Error getting the season ticket product details from WS007R", seasonTicketExLog)
                Case Is = 8 : log.GeneralLog(seasonTicketExceptionsvb, "ST Product: " & _seasonTicketProductCode & ", error-08", "Error getting the stand descriptions from WS118R", seasonTicketExLog)
            End Select
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Bind the season ticket seats repeater basket on Profile.BasketItems
    ''' Also, bind the list of products repeater based on unique exception seat products from tbl_basket_detail_exceptions
    ''' </summary>
    ''' <returns>True if all repeaters bound without issue</returns>
    ''' <remarks></remarks>
    Private Function bindRepeaters() As Boolean
        Dim repeatersBound As Boolean = False
        Dim seasonTicketBasketItems As New Generic.List(Of TalentBasketItem)
        Dim hasSeasonTickets As Boolean = False

        If Profile.Basket.BasketItems.Count > 0 Then
            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                If item.PRODUCT_TYPE = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                    seasonTicketBasketItems.Add(item)
                    _seasonTicketCampaignCode = item.PRICE_CODE
                    hasSeasonTickets = True
                End If
            Next
            If hasSeasonTickets Then
                Try
                    _dtExceptionSeatDetails = TDataObjects.BasketSettings.TblBasketDetailExceptions.GetByBasketDetailHeaderIDAndModule(Profile.Basket.Basket_Header_ID, GlobalConstants.BASKETMODULETICKETING)
                    _stadiumCode = TEBUtilities.CheckForDBNull_String(_dtExceptionSeatDetails.Rows(0)("STADIUM_CODE"))
                    _seasonTicketProductCode = TEBUtilities.CheckForDBNull_String(_dtExceptionSeatDetails.Rows(0)("SEASON_TICKET_PRODUCT"))
                    If _dtExceptionSeatDetails IsNot Nothing AndAlso _dtExceptionSeatDetails.Rows.Count > 0 Then
                        rptSeasonTicketSeats.DataSource = seasonTicketBasketItems
                        rptSeasonTicketSeats.DataBind()
                        repeatersBound = True
                    Else
                        repeatersBound = False
                        handleErrors(True, 2)
                    End If
                Catch ex As Exception
                    repeatersBound = False
                    handleErrors(True, 3, ex)
                End Try

                If repeatersBound Then
                    Dim listOfUniqueProducts As New Generic.List(Of String)
                    Dim dtExceptionProducts As New DataTable
                    Dim dRow As DataRow = Nothing
                    With dtExceptionProducts.Columns
                        .Add("PRODUCT_CODE", GetType(String))
                        .Add("PRODUCT_DESCRIPTION1", GetType(String))
                        .Add("PRODUCT_DESCRIPTION2", GetType(String))
                        .Add("PRODUCT_OPPOSITION_CODE", GetType(String))
                        .Add("PRODUCT_DATE", GetType(String))
                        .Add("PRODUCT_TIME", GetType(String))
                    End With

                    For Each row As DataRow In _dtExceptionSeatDetails.Rows
                        If Not listOfUniqueProducts.Contains(row("PRODUCT_CODE")) Then
                            dRow = dtExceptionProducts.NewRow
                            dRow("PRODUCT_CODE") = row("PRODUCT_CODE")
                            dRow("PRODUCT_DESCRIPTION1") = row("PRODUCT_DESCRIPTION1")
                            dRow("PRODUCT_DESCRIPTION2") = row("PRODUCT_DESCRIPTION2")
                            dRow("PRODUCT_OPPOSITION_CODE") = row("PRODUCT_OPPOSITION_CODE")
                            dRow("PRODUCT_DATE") = row("PRODUCT_DATE")
                            dRow("PRODUCT_TIME") = row("PRODUCT_TIME")
                            dtExceptionProducts.Rows.Add(dRow)
                            listOfUniqueProducts.Add(row("PRODUCT_CODE"))
                        End If
                    Next
                    Try
                        rptExceptionProducts.DataSource = dtExceptionProducts
                        rptExceptionProducts.DataBind()
                        repeatersBound = True
                    Catch ex As Exception
                        repeatersBound = False
                        handleErrors(True, 4, ex)
                    End Try
                End If
            Else
                handleErrors(True, 1)
            End If
        End If
        Return repeatersBound
    End Function

    ''' <summary>
    ''' Retrieve the product details for the season ticket product
    ''' </summary>
    ''' <returns>Datatable of product details from TALENT</returns>
    ''' <remarks></remarks>
    Private Function getSeasonTicketProductDetails() As DataTable
        Dim productDetails As New DataTable
        Dim talProduct As New TalentProduct
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim err As ErrorObj
        Dim logError As Boolean = True
        talProduct.Settings = settings
        talProduct.De.ProductCode = _seasonTicketProductCode
        talProduct.De.CampaignCode = _seasonTicketCampaignCode
        talProduct.De.AllowPriceException = False
        talProduct.ResultDataSet = Nothing
        err = talProduct.ProductDetails

        If Not err.HasError AndAlso talProduct.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString() <> GlobalConstants.ERRORFLAG Then
            If talProduct.ResultDataSet.Tables.Count > 1 Then
                productDetails = talProduct.ResultDataSet.Tables(2)
                logError = False
            End If
        End If
        If logError Then handleErrors(True, 7)
        Return productDetails
    End Function

    ''' <summary>
    ''' Format the seat for better display purposes
    ''' </summary>
    ''' <param name="seat">The seat being re-formatted</param>
    ''' <returns>The formatted seat</returns>
    ''' <remarks></remarks>
    Private Function formatSeatString(ByVal seat As DESeatDetails) As String
        Dim formattedSeat As String = _wfrPage.Content("SeatDisplayFormatText", _languageCode, True)
        If formattedSeat.Trim().Length > 0 Then
            Dim standDescription As String = String.Empty
            Dim areaDescription As String = String.Empty
            Dim err As New ErrorObj
            Dim settings As DESettings = TEBUtilities.GetSettingsObject()
            Dim product As New TalentProduct
            Dim dtStadiumDescriptions As New DataTable
            Dim logError As Boolean = True

            product.De.StadiumCode = _stadiumCode
            product.Settings = settings
            product.ResultDataSet = Nothing
            err = product.StandDescriptions()
            If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing Then
                If product.ResultDataSet.Tables.Count > 1 Then
                    If product.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            If product.ResultDataSet.Tables(1).Rows.Count > 0 Then
                                dtStadiumDescriptions = product.ResultDataSet.Tables(1)
                                logError = False
                            End If
                        End If
                    End If
                End If
            End If
            If logError Then handleErrors(True, 8)

            If dtStadiumDescriptions IsNot Nothing AndAlso dtStadiumDescriptions.Rows.Count > 0 Then
                For Each row As DataRow In dtStadiumDescriptions.Rows
                    If row("StandCode").ToString.Equals(seat.Stand.Trim()) AndAlso row("AreaCode").ToString.Equals(seat.Area.Trim()) Then
                        standDescription = row("StandDescription").ToString.Trim
                        areaDescription = row("AreaDescription").ToString.Trim
                        Exit For
                    End If
                Next
                formattedSeat = formattedSeat.Replace("<<STAND_DESCRIPTION>>", standDescription)
                formattedSeat = formattedSeat.Replace("<<STAND_CODE>>", seat.Stand.Trim())
                formattedSeat = formattedSeat.Replace("<<AREA_DESCRIPTION>>", areaDescription)
                formattedSeat = formattedSeat.Replace("<<AREA_CODE>>", seat.Area.Trim())
                formattedSeat = formattedSeat.Replace("<<ROW>>", seat.Row.Trim())
                formattedSeat = formattedSeat.Replace("<<SEAT_NUMBER>>", seat.Seat.Trim())
            Else
                formattedSeat = seat.FormattedSeat
            End If
        Else
            formattedSeat = seat.FormattedSeat
        End If
        Return formattedSeat
    End Function

    ''' <summary>
    ''' Function to call the add basket routine to rebuild the basket based on whether or not the seats are being reallocated to be kept together or the exception seat is being removed
    ''' 1. Keep seats together, seat information is passed in and the change/remove mode flag is blank
    ''' 2. Remove seats, seat information is passed in the seat array rather than stand/area properties. Remove flag is set to "R"
    ''' </summary>
    ''' <returns>Error object based on success or failure</returns>
    ''' <remarks></remarks>
    Private Function doExceptionReprocessing(ByVal isKeepingSeatsTogether As Boolean, ByVal isRemovingASeat As Boolean, ByVal rawSeatString As String, ByVal exceptionProductCode As String) As ErrorObj
        Dim basket As New Talent.Common.TalentBasket
        Dim deATI As New DEAddTicketingItems
        Dim err As New ErrorObj
        With deATI
            .SessionId = Profile.Basket.Basket_Header_ID
            .ProductCode = exceptionProductCode
            .STExceptionSeasonTicketProductCode = _seasonTicketProductCode
            .Source = GlobalConstants.SOURCE
            .ProductType = GlobalConstants.HOMEPRODUCTTYPE
            .CustomerNumber = Profile.User.Details.LoginID
            If isKeepingSeatsTogether Then
                .StandCode = rawSeatString.Substring(0, 3)
                .AreaCode = rawSeatString.Substring(3, 4)
            End If
            If isRemovingASeat Then
                .STExceptionChangeRemoveMode = GlobalConstants.ST_EXCCEPTION_REMOVE_SEAT
                Dim exceptionSeat As New DESeatDetails
                exceptionSeat.FormattedSeat = rawSeatString
                exceptionSeat.CATSeatStatus = CATHelper.SEAT_STATUS_CANCEL
                .SeatSelectionArray.Add(exceptionSeat)
            End If
        End With

        basket.DeAddTicketingItems = deATI
        basket.Settings = TEBUtilities.GetSettingsObject()
        basket.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        basket.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
        basket.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(Session.Item("Agent"))
        basket.Settings.PerformWatchListCheck = ModuleDefaults.PerformAgentWatchListCheck
        err = basket.AddTicketingItemsReturnBasket
        Return err
    End Function

    Private Function getSTSeatRestrictionDescription(ByVal seasonTicketSeat As String, ByVal seasonTicketProduct As String) As String
        Dim restrictionDescription As String = String.Empty
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.Product = seasonTicketProduct AndAlso item.SEAT = seasonTicketSeat Then
                If item.RESTRICTION_CODE.Trim.Length > 0 Then
                    restrictionDescription = TEBUtilities.RestrictedSeatDescription(item.RESTRICTION_CODE)
                    Exit For
                End If
            End If
        Next
        Return restrictionDescription
    End Function

#End Region

#Region "Publc Functions"

    ''' <summary>
    ''' Get the foundation drop down unique ID name for exception in the repeater
    ''' </summary>
    ''' <param name="index">The current item index</param>
    ''' <returns>The formatted ID name</returns>
    ''' <remarks></remarks>
    Public Function GetDropDownID(ByVal index As Integer) As String
        Dim id As String = "hover-drop-down-"
        id = id & index
        Return id
    End Function

#End Region

End Class
