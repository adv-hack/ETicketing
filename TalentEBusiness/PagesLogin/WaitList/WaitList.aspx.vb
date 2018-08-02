Imports System.Data
Imports Talent.Common

Partial Class PagesLogin_WaitList_WaitList
    Inherits TalentBase01

    Dim _langCode As String = Talent.eCommerce.Utilities.GetCurrentLanguage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not Page.IsPostBack Then
            Dim errMsg As New Talent.Common.TalentErrorMessages(_langCode, _
                                                                   TalentCache.GetBusinessUnitGroup, _
                                                                   TalentCache.GetPartner(Profile), _
                                                                   ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

            'Report any TG errors
            '----------------------------------------------
            If Session("TicketingGatewayError") IsNot Nothing Then
                errorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                    Talent.eCommerce.Utilities.GetCurrentPageName, _
                                    CStr(Session("TicketingGatewayError"))).ERROR_MESSAGE)
                If Session("TalentErrorCode") = Session("TicketingGatewayError") Then
                    Session("TalentErrorCode") = Nothing
                End If
                Session("TicketingGatewayError") = Nothing
            End If
            If Session("TalentErrorCode") IsNot Nothing Then
                errorList.Items.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                   Talent.eCommerce.Utilities.GetCurrentPageName, _
                                   CStr(Session("TalentErrorCode"))).ERROR_MESSAGE)
                Session("TalentErrorCode") = Nothing
            End If
            '----------------------------------------------

            Dim de As New Talent.Common.DEWaitList
            de.CustomerNumber = Profile.UserName
            de.Src = "W"
            Dim twl As New TalentWaitList
            twl.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            twl.DE = de
            With twl.Settings
                .BusinessUnit = TalentCache.GetBusinessUnit
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
            End With

            Dim err As ErrorObj = twl.RetrieveCustomerWaitListHistory

            If Not err.HasError Then
                Dim ds As DataSet = twl.ResultDataSet
                If ds IsNot Nothing AndAlso ds.Tables.Count > 1 Then

                    Dim hTbl As DataTable = ds.Tables("DtWaitListHeaderResults")
                    If hTbl.Rows.Count > 0 Then
                        If CStr(hTbl.Rows(0)("WaitListRequests")).ToUpper = "Y" Then
                            SeasonTicketWaitList1.Display = False
                            SeasonTicketWaitList1.Visible = False
                            SeasonTicketWaitListSummary1.Visible = True
                            SeasonTicketWaitListSummary1.Display = True
                        Else
                            SeasonTicketWaitList1.Display = True
                            SeasonTicketWaitList1.Visible = True
                            If CStr(hTbl.Rows(0)("Status")).ToUpper = "W" Then
                                SeasonTicketWaitListSummary1.Visible = True
                                SeasonTicketWaitListSummary1.Display = True
                            Else
                                SeasonTicketWaitListSummary1.Visible = False
                                SeasonTicketWaitListSummary1.Display = False
                            End If
                            
                        End If
                    Else
                        SeasonTicketWaitList1.Display = True
                        SeasonTicketWaitList1.Visible = True
                        SeasonTicketWaitListSummary1.Visible = False
                        SeasonTicketWaitListSummary1.Display = False
                    End If
                Else
                    SeasonTicketWaitList1.Display = True
                    SeasonTicketWaitList1.Visible = True
                    SeasonTicketWaitListSummary1.Visible = False
                    SeasonTicketWaitListSummary1.Display = False
                End If
            End If
        End If
    End Sub

End Class
