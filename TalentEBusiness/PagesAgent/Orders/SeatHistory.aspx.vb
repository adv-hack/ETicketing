Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Orders_SeatHistory
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _currentSeatStatus As String = Nothing
    Private _productDescription As String = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _wfrPage = New WebFormResource
        _languageCode = TCUtilities.GetDefaultLanguage()
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SeatHistory.aspx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Request.QueryString("product") IsNot Nothing AndAlso Request.QueryString("seat") IsNot Nothing AndAlso Request.QueryString("paymentref") IsNot Nothing Then
            plhNoSeatHistory.Visible = getSeatHistoryData()
            If Not plhNoSeatHistory.Visible Then setSeatHeaderInformation()
        Else
            plhNoSeatHistory.Visible = True
        End If
        If plhNoSeatHistory.Visible Then ltlNoSeatHistory.Text = _wfrPage.Content("NoSeatHistoryFound", _languageCode, True)

    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Get the seat history from TALENT and check for errors
    ''' </summary>
    ''' <returns>True if the seat history has returned and bound correctly</returns>
    ''' <remarks></remarks>
    Private Function getSeatHistoryData() As Boolean
        Dim hasError As Boolean = True
        Dim err As New ErrorObj
        Dim tProduct As New TalentProduct
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim productDataEntity As New DEProductDetails
        Dim seatDetails As New DESeatDetails
        seatDetails.FormattedSeat = Request.QueryString("seat").ToUpper()
        productDataEntity.ProductCode = Request.QueryString("product").ToUpper()
        productDataEntity.FullSeatDetails = seatDetails
        productDataEntity.PaymentReference = Request.QueryString("paymentref")
        settings.Company = AgentProfile.GetAgentCompany()
        tProduct.De = productDataEntity
        tProduct.Settings = settings
        err = tProduct.RetrieveSeatHistory()

        If Not err.HasError AndAlso tProduct.ResultDataSet IsNot Nothing AndAlso tProduct.ResultDataSet.Tables.Count = 2 _
            AndAlso tProduct.ResultDataSet.Tables(0).Rows.Count > 0 AndAlso tProduct.ResultDataSet.Tables(1).Rows.Count > 0 Then
            If tProduct.ResultDataSet.Tables(0).Rows(0)(0) = String.Empty Then
                _productDescription = tProduct.ResultDataSet.Tables(1).Rows(0)("ProductDescription").ToString()
                _currentSeatStatus = tProduct.ResultDataSet.Tables(1).Rows(0)("CurrentSeatStatus").ToString()
                hasError = bindRepeater(tProduct.ResultDataSet.Tables(1))
            End If
        End If
        Return hasError
    End Function

    ''' <summary>
    ''' Bind the repeater with the seat print history data table
    ''' </summary>
    ''' <param name="dtSeatHistory">The data table containing the seat print history</param>
    ''' <returns>True if the the repeater has been bound correctly</returns>
    ''' <remarks></remarks>
    Private Function bindRepeater(ByRef dtSeatHistory As DataTable) As Boolean
        Dim hasError As Boolean = True
        Try
            Dim dvSeatHistory As New DataView
            dvSeatHistory.Table = dtSeatHistory
            dvSeatHistory.Sort = "Date Desc, Time Desc"
            rptSeatHistory.DataSource = dvSeatHistory.ToTable()
            rptSeatHistory.DataBind()
            hasError = False
        Catch ex As Exception
            hasError = True
        End Try
        Return hasError
    End Function

    ''' <summary>
    ''' Set the seat header information shown before the repeater, this includes a softcoded string for seat status
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setSeatHeaderInformation()
        Const nbsp As String = "&nbsp;"
        ltlProductLabel.Text = _wfrPage.Content("ProductLabel", _languageCode, True)
        ltlProductValue.Text = Request.QueryString("product").ToUpper() & nbsp & _productDescription
        If Profile.IsAnonymous Then
            plhCustomerDetails.Visible = False
        Else
            plhCustomerDetails.Visible = True
            ltlCustomerLabel.Text = _wfrPage.Content("CustomerLabel", _languageCode, True)
            ltlCustomerValue.Text = Profile.User.Details.LoginID.TrimStart(GlobalConstants.LEADING_ZEROS) & nbsp & Profile.User.Details.Title & nbsp & Profile.User.Details.Forename & nbsp & Profile.User.Details.Surname
        End If
        ltlSeatLabel.Text = _wfrPage.Content("SeatLabel", _languageCode, True)
        ltlSeatValue.Text = Request.QueryString("seat").ToUpper()
        ltlSeatStatusLabel.Text = _wfrPage.Content("SeatStatusLabel", _languageCode, True)
        ltlSeatStatusValue.Text = _wfrPage.Content("SeatStatus-" & _currentSeatStatus.Replace(" ", "_"), _languageCode, True)
        plhSeatHistoryHeader.Visible = True
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the real soft coded action name for the iSeries abbreviated action code.
    ''' The iseries action may contain spaces, these are replaced with underscores, before the softcoded text is retrieved
    ''' </summary>
    ''' <param name="iSeriesActionName">The iSeries action name to be translated</param>
    ''' <returns>The full translated name</returns>
    ''' <remarks></remarks>
    Public Function GetTranslatedActionName(ByVal iSeriesActionName As String) As String
        Dim translatedActionName As String = "Action-"
        translatedActionName = _wfrPage.Content(translatedActionName & iSeriesActionName, _languageCode, True)
        Return translatedActionName
    End Function

    ''' <summary>
    ''' Get the real soft coded comment line for the iSeries comment line (which is abbreviated).
    ''' The iseries comment may contain spaces, these are replaced with underscores
    ''' </summary>
    ''' <param name="iSeriesCommentLine">The full iSeries comment line</param>
    ''' <returns>The full translated name</returns>
    ''' <remarks></remarks>
    Public Function GetFormattedCommentLine(ByVal iSeriesCommentLine As String) As String
        Const commentPrefix As String = "Comment-"
        Dim translatedCommentLine As String = iSeriesCommentLine
        Dim listOfCommentStrings As New Generic.List(Of String)
        listOfCommentStrings.Add("Price_Code:")
        listOfCommentStrings.Add("Price_Band:")
        listOfCommentStrings.Add("Value:")
        listOfCommentStrings.Add("Transferred_from_Seat:")
        listOfCommentStrings.Add("Season_Ticket_Seat:")
        listOfCommentStrings.Add("Buyback_Ref:")
        listOfCommentStrings.Add("Transferred_To_Seat:")
        listOfCommentStrings.Add("Reservation_Comment:")
        listOfCommentStrings.Add("Booked_Seat_transferred_to:")
        listOfCommentStrings.Add("Comment:")
        listOfCommentStrings.Add("P/Sales_Release_Comment:")
        For Each commentString In listOfCommentStrings
            translatedCommentLine = translatedCommentLine.Replace(commentString, "&nbsp <strong>" & _wfrPage.Content(commentPrefix & commentString, _languageCode, True) & "</strong>")
        Next
        translatedCommentLine = translatedCommentLine.Replace("_", " ")
        Return translatedCommentLine
    End Function

    ''' <summary>
    ''' Get the text for the column heading
    ''' </summary>
    ''' <param name="columnName">The column name to retrieve the text heading for</param>
    ''' <returns>The heading text for the column</returns>
    ''' <remarks></remarks>
    Public Function GetText(ByVal columnName As String) As String
        Dim columnHeaderText As String = String.Empty
        Select Case columnName
            Case Is = "AgentColumn" : columnHeaderText = _wfrPage.Content("AgentColumnText", _languageCode, True)
            Case Is = "DateColumn" : columnHeaderText = _wfrPage.Content("DateColumnText", _languageCode, True)
            Case Is = "TimeColumn" : columnHeaderText = _wfrPage.Content("TimeColumnText", _languageCode, True)
            Case Is = "CustomerNumberColumn" : columnHeaderText = _wfrPage.Content("CustomerNumberColumnText", _languageCode, True)
            Case Is = "ActionColumn" : columnHeaderText = _wfrPage.Content("ActionColumnnText", _languageCode, True)
            Case Is = "BatchColumn" : columnHeaderText = _wfrPage.Content("BatchColumnText", _languageCode, True)
            Case Is = "PaymentReferenceColumn" : columnHeaderText = _wfrPage.Content("PaymentReferenceColumnText", _languageCode, True)
            Case Is = "PriceCodeColumn" : columnHeaderText = _wfrPage.Content("PriceCodeColumnText", _languageCode, True)
            Case Is = "PriceBandColumn" : columnHeaderText = _wfrPage.Content("PriceBandColumnText", _languageCode, True)
            Case Is = "ValueColumn" : columnHeaderText = _wfrPage.Content("ValueColumnText", _languageCode, True)
            Case Is = "CommentsLabel" : columnHeaderText = _wfrPage.Content("CommentsLabelText", _languageCode, True)
        End Select
        Return columnHeaderText
    End Function

#End Region

End Class