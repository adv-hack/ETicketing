'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Country Of Delivery Drop down
'
'       Date                        12/09/07
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCCOD - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

Partial Class UserControls_CountryOfDelivery
    Inherits ControlBase

    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
       
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '------------------------
        ' Language not yet needed
        '------------------------
        ddlLanguage.Visible = False
        lblLanguage.Visible = False

        If Not Page.IsPostBack Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                .KeyCode = "CountryOfDelivery.ascx"
                .PageCode = UCase(Usage)
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)

                lblCountry.Text = .Content("CountryLabel", _languageCode, True)
                lblLanguage.Text = .Content("LanguageLabel", _languageCode, True)

            End With

            SetDDLValuesCountry()
            If ddlLanguage.Visible Then
                SetDDLValuesLanguage()
            End If

        End If
    End Sub

    Protected Sub SetDDLValuesCountry()
        '-------------------
        ' Set up country ddl
        '-------------------
        ddlCountry.Items.Clear()
        Dim bu As String = TalentCache.GetBusinessUnit
        Dim dt As Data.DataTable = TalentCache.GetAvailableBusinessUnits()
        Dim li As ListItem
        For Each dr As Data.DataRow In dt.Rows
            li = New ListItem(dr("BUSINESS_UNIT_DESC"), dr("BUSINESS_UNIT"))
            '---------------------------------------
            ' Set current BU as the selected country
            '---------------------------------------
            '   Response.Write(bu)
            If li.Value = bu Then
                li.Selected = True
            End If
            ddlCountry.Items.Add(li)

        Next
        
    End Sub
    Protected Sub SetDDLValuesLanguage()
        '--------------------
        ' Set up language ddl
        '--------------------
        ddlLanguage.Items.Clear()
        Dim dt2 As Data.DataTable = TalentCache.GetAvailableLanguagesForBU(ddlCountry.SelectedValue.ToString)
        For Each dr As Data.DataRow In dt2.Rows
            '  ddlLanguage.Items.Add(dr("LANGUAGE_DESC"))
            ddlLanguage.Items.Add(New ListItem(dr("LANGUAGE_DESC"), dr("LANGUAGE")))
        Next

    End Sub

    Protected Sub ddlCountry_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCountry.SelectedIndexChanged
        If ddlLanguage.Visible Then
            SetDDLValuesLanguage()
        End If
        '--------------------------------------------------
        ' Check if current BU is in the path..if so replace
        '--------------------------------------------------
        Dim currentBU As String = TalentCache.GetBusinessUnit()
        Dim currentURL As String = Request.Url.OriginalString
        Dim newURL As String = String.Empty
        If currentURL.Contains("/" & currentBU.Trim & "/") Then
            newURL = currentURL.Replace("/" & currentBU.Trim & "/", "/" & ddlCountry.SelectedValue.Trim & "/")
            Try
                Response.Redirect(newURL)
            Catch tae As System.Threading.ThreadAbortException
                ' Catch exception caused by redirection
            End Try
        End If


    End Sub

    Protected Sub ddlLanguage_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlLanguage.SelectedIndexChanged

    End Sub
End Class
