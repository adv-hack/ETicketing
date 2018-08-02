Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Web.Services
Imports System.Web.Script.Services
Imports Talent.Common
Imports System.Data
Imports System.Collections.Generic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Update Profile
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'
'       Error Number Code base      PLUPRO- 
'       
'       User Controls
'           registrationDetails(6)
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_updateProfile
    Inherits TalentBase01

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Not AgentProfile.IsAgent OrElse (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAddOrAmendCustomer) Then
            Select Case MyBase.ModuleDefaults.UpdateDetailsTemplate
                Case Is = "2"
                    plhUpdateDetailsForm2.Visible = True
                    plhUpdateDetailsForm1.Visible = False
                Case Else
                    plhUpdateDetailsForm2.Visible = False
                    plhUpdateDetailsForm1.Visible = True
            End Select
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Select Case MyBase.ModuleDefaults.UpdateDetailsTemplate
            Case Is = "2"
                UpdateDetailsForm1.Visible = False
            Case Else
                UpdateDetailsForm2.Visible = False
        End Select
    End Sub
    <WebMethod(), ScriptMethod()>
    Public Shared Function GetCityTownList(ByVal prefixText As String, ByVal count As Integer) As List(Of String)
        Dim wfrPage As New WebFormResource
        Dim townAndCityList As List(Of String) = New List(Of String)
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()

        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = "Registration.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Registration.aspx"
        End With
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = Talent.Common.GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = wfrPage.BusinessUnit
        tDataObjects.Settings = settings

        Dim dtTownAndCities As DataTable = tDataObjects.ProfileSettings.TblAddressFormat1Data.GetDistinctTownCity()

        If dtTownAndCities IsNot Nothing AndAlso dtTownAndCities.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            matchedRows = dtTownAndCities.Select("TOWN LIKE '%" & prefixText & "%'" & " or CITY LIKE '%" & prefixText & "%'")
            If matchedRows.Length > 0 Then
                For rowIndex As Integer = 0 To matchedRows.Length - 1
                    townAndCityList.Add(matchedRows(rowIndex)("TOWN").ToString & "," & matchedRows(rowIndex)("CITY").ToString)
                Next
            End If

            Dim turkishI As String = wfrPage.Content("iText", Talent.Common.Utilities.GetDefaultLanguage(), True)
            Dim turkishI2 As String = wfrPage.Content("iText2", Talent.Common.Utilities.GetDefaultLanguage(), True)


            If prefixText.Contains("i") OrElse prefixText.Contains("I") Then

                prefixText = prefixText.Replace("i", turkishI)
                prefixText = prefixText.Replace("I", turkishI)

                matchedRows = dtTownAndCities.Select("TOWN LIKE '%" & prefixText & "%'" & " or CITY LIKE '%" & prefixText & "%'")
                If matchedRows.Length > 0 Then

                    For rowIndex As Integer = 0 To matchedRows.Length - 1
                        townAndCityList.Add(matchedRows(rowIndex)("TOWN").ToString & "," & matchedRows(rowIndex)("CITY").ToString)
                    Next
                End If
            End If
            If prefixText.Contains(turkishI2) Then
                prefixText = prefixText.Replace(turkishI2, "I")

                matchedRows = dtTownAndCities.Select("TOWN LIKE '%" & prefixText & "%'" & " or CITY LIKE '%" & prefixText & "%'")
                If matchedRows.Length > 0 Then

                    For rowIndex As Integer = 0 To matchedRows.Length - 1
                        townAndCityList.Add(matchedRows(rowIndex)("TOWN").ToString & "," & matchedRows(rowIndex)("CITY").ToString)
                    Next


                End If
            End If

            If townAndCityList.Count = 0 Then

                townAndCityList.Add(wfrPage.Content("NothingFoundText", Talent.Common.Utilities.GetDefaultLanguage(), True))
            End If

        End If

        Return townAndCityList

    End Function


End Class
