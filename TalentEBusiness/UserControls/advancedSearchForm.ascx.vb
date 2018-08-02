Imports Talent.eCommerce
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Advanced Search 
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCFPWD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_advancedSearchForm
    Inherits ControlBase

    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private businessUnit As String = TalentCache.GetBusinessUnit()
    Private partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then

            ' **TODO** try with *ALL/partner   
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                .KeyCode = "advancedSearchForm.ascx"
                .PageCode = UCase(Usage)
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)

                sectionLabel.Text = .Content("Section", _languageCode, True)
                searchButton.Text = .Content("Submit", _languageCode, True)

                ' Add the All section
                section.Items.Add(New ListItem(ucr.Content("AllText", _languageCode, True), "1*ALL"))
            End With

            ' Get groups to display - try default partner first
            Dim egd As New ECommerceGroupDefaults
            Dim dt As DataTable = egd.GetGroupDefaults(businessUnit, partner)

            ' Try *ALL partner
            If dt.Rows.Count < 1 Then
                dt = egd.GetGroupDefaults(businessUnit, Talent.Common.Utilities.GetAllString)
            End If

            Dim i As Integer = 1
            If dt.Rows.Count > 0 Then
                For Each row As DataRow In dt.Rows
                    i += 1
                    If row("GROUP_L01_ADV_SEARCH_TEMPLATE").Equals(String.Empty) Or _
                        row("GROUP_L01_ADV_SEARCH_TEMPLATE").Equals("0") Then
                    Else
                        section.Items.Add(New ListItem(row("GROUP_L01_DESCRIPTION_1"), row("GROUP_L01_ADV_SEARCH_TEMPLATE") & row("GROUP_L01_L01_GROUP")))
                    End If
                Next
            End If

            ' Extract from query string - in case "search again" used or want to book mark search page
            Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString

            ' Set which group is selected
            If Not queryString.Item("group01") Is Nothing Then
                For Each li As ListItem In section.Items
                    If li.Value.Substring(1, li.Value.Length - 1).Equals(queryString.Item("group01")) Then
                        li.Selected = True
                    End If
                Next
            Else
                section.Items(0).Selected = True
            End If

            ' Display the relevant form
            If Not queryString.Item("type") Is Nothing Then
                Select Case queryString.Item("type")
                    Case Is = "2"
                        DisplaySearch2()
                    Case Is = "3"
                        DisplaySearch3()
                    Case Is = "4"
                        DisplaySearch4()
                    Case Else
                        DisplaySearch1()
                End Select
            ElseIf ModuleDefaults.AdvancedSearchType = 2 Then
                DisplaySearch4()
            Else
                DisplaySearch1()
            End If

        End If

    End Sub
    Protected Sub section_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles section.SelectedIndexChanged
        Select Case section.SelectedValue.Substring(0, 1)
            Case Is = 1
                DisplaySearch1()
            Case Is = 2
                DisplaySearch2()
            Case Is = 3
                DisplaySearch3()
            Case Is = 4
                DisplaySearch4()
        End Select
    End Sub

    Protected Sub DisplaySearch1()
        AdvancedSearchType1.Visible = True
        AdvancedSearchType1.BuildPage()
        AdvancedSearchType2.Visible = False
        AdvancedSearchType3.Visible = False
        AdvancedSearchType4.Visible = False
    End Sub
    Protected Sub DisplaySearch2()
        AdvancedSearchType1.Visible = False
        AdvancedSearchType2.Visible = True
        AdvancedSearchType2.Group1 = section.SelectedValue.Substring(1, section.SelectedValue.Length - 1)
        AdvancedSearchType2.BuildPage()
        AdvancedSearchType3.Visible = False
        AdvancedSearchType4.Visible = False
    End Sub
    Protected Sub DisplaySearch3()
        AdvancedSearchType1.Visible = False
        AdvancedSearchType2.Visible = False
        AdvancedSearchType3.Visible = True
        AdvancedSearchType3.Group1 = section.SelectedValue.Substring(1, section.SelectedValue.Length - 1)
        AdvancedSearchType3.BuildPage()
        AdvancedSearchType4.Visible = False
    End Sub
    Protected Sub DisplaySearch4()
        AdvancedSearchType1.Visible = False
        AdvancedSearchType2.Visible = False
        AdvancedSearchType3.Visible = False
        AdvancedSearchType4.Visible = True        
    End Sub

    Protected Sub searchButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles searchButton.Click

        If Page.IsValid Then

            Dim queryString As String = String.Empty
            If AdvancedSearchType1.Visible Then
                queryString = AdvancedSearchType1.BuildQueryString()
            End If
            If AdvancedSearchType2.Visible Then
                queryString = AdvancedSearchType2.BuildQueryString()
            End If
            If AdvancedSearchType3.Visible Then
                queryString = AdvancedSearchType3.BuildQueryString()
            End If
            If AdvancedSearchType4.Visible Then
                queryString = AdvancedSearchType4.BuildQueryString()
            End If

            Dim responseString As New StringBuilder
            With responseString
                .Append("~/PagesPublic/Search/searchResults.aspx?source=advanced&type=")
                .Append(section.SelectedValue.Substring(0, 1))
                If Not section.SelectedValue.Substring(0, 1).Equals("1") Then
                    .Append("&group01=")
                    ' I'm not sure why this was "Substring(1" but I've changed
                    ' it to "Substring(0" to resolve JIRA Bug: MBSTEB-117
                    .Append(section.SelectedValue.ToString())
                End If
                .Append(queryString)
            End With
            Dim s As String = responseString.ToString
            If s.Contains(":") Then s = s.Replace(":", "%3A")
            Response.Redirect(s.ToString)

        End If

    End Sub
End Class
