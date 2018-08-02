Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class PagesPublic_ProductBrowse_PackageSeatDetails
    Inherits TalentBase01

#Region "Class Level Fields"
    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _basketID As String
    Private _bulkID As String
    Private _callID As String
    Private _componentID As String
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
            .KeyCode = "PackageSeatDetails.aspx"
        End With
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim hasError As Boolean = True
        plhRowSeats.Visible = False
        plhComponentBulkSeats.Visible = False
        plhNoSeatDetails.Visible = False

        'Called during sales process (ComponentSeats.ascx)
        If Request.QueryString("basketID") IsNot Nothing AndAlso Request.QueryString("BulkID") IsNot Nothing Then
            _basketID = Request.QueryString("basketID")
            _bulkID = Request.QueryString("BulkID")
            hasError = setSeatDetails()
            If Not hasError Then
                plhRowSeats.Visible = True
            End If
            ' Called from purchase History
        ElseIf Request.QueryString("CallID") IsNot Nothing AndAlso Request.QueryString("ComponentID") IsNot Nothing Then
            _callID = Request.QueryString("CallID")
            _componentID = Request.QueryString("ComponentID")
            hasError = getComponentBulkSeats()
            If Not hasError Then
                plhComponentBulkSeats.Visible = True
            End If
        Else
            hasError = True
        End If

        If hasError Then
            If Not ltlNoSeatDetails.Text Is Nothing Then
                plhNoSeatDetails.Visible = True
            End If
        End If
    End Sub
#End Region

#Region "Private Functions"

    Private Function setSeatDetails() As Boolean
        Dim tP As New Talent.Common.TalentPackage
        Dim err As New ErrorObj
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim deP As New DEPackages
        Dim seatAllocation As New SeatAllocation
        Dim hasError As Boolean = True

        deP.BasketId = _basketID
        seatAllocation.BulkId = _bulkID
        deP.SeatAllocations.Add(seatAllocation)
        tP.Settings = settings
        tP.DePackages = deP

        err = tP.GetPackageSeatDetails()

        If tP.ResultDataSet IsNot Nothing Then
            formatSeats(tP.ResultDataSet.Tables("PackageSeatDetails"))
            hasError = BindRepeater(tP.ResultDataSet.Tables("PackageSeatDetails"), rptRowSeats)
        End If
        Return hasError
    End Function

    Private Function getComponentBulkSeats() As Boolean
        Dim tOrder As New Talent.Common.TalentOrder
        Dim err As New ErrorObj
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim deO As New DEOrder
        Dim hasError As Boolean = True

        deO.CallId = _callID
        deO.ComponentId = _componentID

        tOrder.Settings = settings
        tOrder.Dep = deO

        err = tOrder.GetComponentBulkSeats()

        If tOrder.ResultDataSet IsNot Nothing Then
            formatSeats(tOrder.ResultDataSet.Tables("ComponentBulkSeatDetails"))
            formatCustomer(tOrder.ResultDataSet.Tables("ComponentBulkSeatDetails"))
            hasError = BindRepeater(tOrder.ResultDataSet.Tables("ComponentBulkSeatDetails"), rptComponentBulkSeats)
        End If
        Return hasError
    End Function

    Private Function BindRepeater(dT As DataTable, rp As Repeater) As Boolean
        Dim hasError As Boolean = True
        Try
            rp.DataSource = dT
            rp.DataBind()
            hasError = False
        Catch ex As Exception
            hasError = True
            ltlNoSeatDetails.Text = ex.Message.ToString
        End Try
        Return hasError
    End Function

#End Region

#Region "Public Functions"
    ''' <summary>
    ''' Get the text for the column heading
    ''' </summary>
    ''' <param name="columnName">The column name to retrieve the text heading for</param>
    ''' <returns>The heading text for the column</returns>
    ''' <remarks></remarks>
    Public Function GetText(ByVal columnName As String) As String
        Dim columnHeaderText As String = String.Empty
        columnHeaderText = _wfrPage.Content(columnName, _languageCode, True)
        Return columnHeaderText
    End Function

#End Region

#Region "Private Subs"
    Private Sub formatSeats(ByRef dt As DataTable)
        dt.Columns.Add("Seats_Formatted")
        For Each row In dt.Rows
            If dt.Columns.Contains("Is_Roving") AndAlso row("Is_Roving").ToString = "Y" Then
                Dim rovingText As String = Utilities.CheckForDBNull_String(_wfrPage.Content("RovingText", _languageCode, True))
                row("Stand") = If(Not rovingText Is String.Empty, rovingText, "Roving")
                row("Area") = String.Empty
                row("Row_Num") = String.Empty
                row("Seats") = String.Empty
            ElseIf dt.Columns.Contains("Is_Unreserved") AndAlso row("Is_Unreserved").ToString = "Y" Then
                Dim unreservedText As String = Utilities.CheckForDBNull_String(_wfrPage.Content("UnreservedText", _languageCode, True))
                row("Stand") = If(Not unreservedText Is String.Empty, unreservedText, "Unreserved")
                row("Stand") = _wfrPage.Content("Unreserved", _languageCode, True)
                row("Area") = String.Empty
                row("Row_Num") = String.Empty
                row("Seats") = String.Empty
            ElseIf dt.Columns.Contains("Seats") Then
                row("Seats_Formatted") = TCUtilities.FormatBulkSeats(row("Seats"))
            End If
        Next
    End Sub

    Private Sub formatCustomer(ByRef dt As DataTable)
        dt.Columns.Add("Customer")
        For Each row In dt.Rows
            If dt.Columns.Contains("Customer_Number") And dt.Columns.Contains("Customer_Name") Then
                row("Customer") = row("Customer_Number").ToString.TrimStart("0").Trim + " " + row("Customer_Name").ToString.Trim
            End If
        Next
    End Sub
#End Region
End Class
