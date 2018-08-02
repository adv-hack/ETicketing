Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Orders_SeatPrintHistory
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _productDescription As String = Nothing
    Private _ticketsPrinted As Integer

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
            .KeyCode = "SeatPrintHistory.aspx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Request.QueryString("product") IsNot Nothing AndAlso Request.QueryString("seat") IsNot Nothing Then
            plhNoSeatPrintHistory.Visible = getSeatPrintHistoryData()
            If Not plhNoSeatPrintHistory.Visible Then setSeatHeaderInformation()
        Else
            plhNoSeatPrintHistory.Visible = True
        End If
        If plhNoSeatPrintHistory.Visible Then ltlNoSeatPrintHistory.Text = _wfrPage.Content("NoSeatPrintHistoryFound", _languageCode, True)

    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Get the seat print history from TALENT and check for errors
    ''' </summary>
    ''' <returns>True if the seat print history has returned and bound correctly</returns>
    ''' <remarks></remarks>
    Private Function getSeatPrintHistoryData() As Boolean
        Dim hasError As Boolean = True
        Dim err As New ErrorObj
        Dim tProduct As New TalentProduct
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim productDataEntity As New DEProductDetails
        Dim seatDetails As New DESeatDetails
        seatDetails.FormattedSeat = Request.QueryString("seat").ToUpper()
        productDataEntity.ProductCode = Request.QueryString("product").ToUpper()
        productDataEntity.FullSeatDetails = seatDetails
        settings.Company = AgentProfile.GetAgentCompany()
        tProduct.De = productDataEntity
        tProduct.Settings = settings
        err = tProduct.RetrieveSeatPrintHistory()

        If Not err.HasError AndAlso tProduct.ResultDataSet IsNot Nothing AndAlso tProduct.ResultDataSet.Tables.Count = 1 AndAlso tProduct.ResultDataSet.Tables(0).Rows.Count > 0 Then
            If tProduct.ResultDataSet.Tables(0).Rows(0)(0) <> String.Empty Then
                _productDescription = tProduct.ResultDataSet.Tables(0).Rows(0)("ProductDescription").ToString()
                For Each row As DataRow In tProduct.ResultDataSet.Tables(0).Rows
                    _ticketsPrinted += CInt(row("TicketsPrinted"))
                Next
                hasError = bindRepeater(tProduct.ResultDataSet.Tables(0))
            End If
        End If
        Return hasError
    End Function

    ''' <summary>
    ''' Bind the repeater with the seat print history data table
    ''' </summary>
    ''' <param name="dtSeatPrintHistory">The data table containing the seat print history</param>
    ''' <returns>True if the the repeater has been bound correctly</returns>
    ''' <remarks></remarks>
    Private Function bindRepeater(ByRef dtSeatPrintHistory As DataTable) As Boolean
        Dim hasError As Boolean = True
        Try
            rptSeatPrintHistory.DataSource = dtSeatPrintHistory
            rptSeatPrintHistory.DataBind()
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
            ltlCustomerValue.Text = Profile.User.Details.LoginID & nbsp & Profile.User.Details.Title & nbsp & Profile.User.Details.Forename & nbsp & Profile.User.Details.Surname
        End If
        ltlSeatLabel.Text = _wfrPage.Content("SeatLabel", _languageCode, True)
        ltlSeatValue.Text = Request.QueryString("seat").ToUpper()
        ltlTicketsPrintedLabel.Text = _wfrPage.Content("TicketsPrintedLabel", _languageCode, True)
        ltlTicketsPrintedValue.Text = _ticketsPrinted.ToString()
        plhSeatPrintHistoryHeader.Visible = True
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the real soft coded program name where the reprint was performed from based on the "MD040R" iSeries program name
    ''' </summary>
    ''' <param name="iSeriesProgramName">The iSeries program name to be translated</param>
    ''' <returns>The full translated name</returns>
    ''' <remarks></remarks>
    Public Function GetTranslatedProgramName(ByVal iSeriesProgramName As String) As String
        Dim translatedProgramName As String = "Program-"
        translatedProgramName = _wfrPage.Content(translatedProgramName & iSeriesProgramName, _languageCode, True)
        Return translatedProgramName
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
            Case Is = "CustomerNameColumn" : columnHeaderText = _wfrPage.Content("CustomerNameColumnText", _languageCode, True)
            Case Is = "ProgramNameColumn" : columnHeaderText = _wfrPage.Content("ProgramNameColumnText", _languageCode, True)
        End Select
        Return columnHeaderText
    End Function

#End Region

End Class