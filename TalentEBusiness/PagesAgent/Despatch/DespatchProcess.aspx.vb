Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Web.Script.Serialization
Imports System.Data

Partial Class PagesAgent_Despatch_DespatchProcess
    Inherits TalentBase01
    Private _wfr As WebFormResource = Nothing

    Protected Sub Page_Init1(sender As Object, e As EventArgs) Handles Me.Init
        _wfr = New WebFormResource
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = TEBUtilities.GetCurrentPageName()
            .PageCode = TEBUtilities.GetCurrentPageName()
        End With
    End Sub

    Protected Sub Page_Load1(sender As Object, e As EventArgs) Handles Me.Load
        If Request.QueryString("payref") IsNot Nothing Then
            uscDespatchProcess.PaymentReference = Request.QueryString("payref")
            uscDespatchProcess.Usage = UserControls_DespatchProcess.UsageType.PURCHASEHISTORY
        End If
    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function CallSmartCardRequest(ByVal SessionID As String, ByVal Mode As String, ByVal ProductCode As String, _
                                                ByVal Stand As String, ByVal Area As String, ByVal Row As String, ByVal Seat As String, ByVal Alpha As String, ByVal PaymentReference As String, ByVal ExistingCardNumber As String) As String
        Dim talentSmartCard As New TalentSmartcard

        Dim de As New DESmartcard
        With de
            .SessionID = SessionID
            .Mode = Mode
            .ProductCode = ProductCode
            .Stand = Stand
            .Area = Area
            .Row = Row
            .Seat = Seat
            .Alpha = Alpha
            .PaymentReference = PaymentReference
            .ExistingCardNumber = ExistingCardNumber
        End With
        talentSmartCard.DE = de
        talentSmartCard.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        Dim err As ErrorObj = talentSmartCard.RequestPrintCard
        Dim viewModel As String = String.Empty
        If Not err.HasError AndAlso talentSmartCard.ResultDataSet.Tables(0).Rows(0).Item("ErrorFlag") <> "E" _
                AndAlso talentSmartCard.ResultDataSet.Tables.Count >= 2 Then
            Dim result As DESmartcard = ReturnViewModel(New DESmartcard, talentSmartCard.ResultDataSet)
            Dim serializer As New JavaScriptSerializer
            viewModel = serializer.Serialize(result)
            viewModel = "[" & viewModel & "]"
        End If

        Return viewModel
    End Function


    Private Shared Function ReturnViewModel(ByRef smartcard As DESmartcard, ByVal resultSet As DataSet) As DESmartcard
        Dim rowResult As DataRow = resultSet.Tables(1).Rows(0)
        If rowResult IsNot Nothing Then
            smartcard.ExistingCardNumber = rowResult("ExistingCardNumberID")
            smartcard.NewCardNumberID = rowResult("NewCardNumberID")
        End If
        Dim rowError As DataRow = resultSet.Tables(0).Rows(0)
        If rowError IsNot Nothing Then
            smartcard.ErrorCode = rowError("ReturnCode")
            smartcard.ErrorFlag = rowError("ErrorFlag")
        End If
        Return smartcard
    End Function
End Class