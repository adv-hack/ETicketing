Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Web

Partial Class PagesPublic_HopewiserPostcodeAndCountry
    Inherits TalentBase01

    Private _wfr As Talent.Common.WebFormResource = Nothing
    Private _languageCode As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _wfr = New Talent.Common.WebFormResource
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = "HopewiserPostcodeAndCountry.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "HopewiserPostcodeAndCountry.aspx"
        End With
        lblPostCode.Text = _wfr.Content("postcodeLabel", _languageCode, True)
        lblCountry.Text = _wfr.Content("countryLabel", _languageCode, True)
    End Sub

    Protected Function GetPageText(ByVal sKey As String) As String
        Dim strString As String = _wfr.Content(sKey, _languageCode, True)
        Return strString.Trim
    End Function

    Protected Sub ButtonNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonNext.Click
        Dim sPostCode As String = String.Empty
        Dim sCountry As String = String.Empty
        sPostCode = Me.postcode.Text
        sCountry = Me.country.Text
        Server.Transfer("../../PagesPublic/Hopewiser/HopewiserSearch.aspx?postcode=" & sPostCode & "&country=" & sCountry)
    End Sub

    Protected Sub PopulateCountriesDropDownList()
        country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "HOPEWISER", "COUNTRY")
        country.DataTextField = "Text"
        country.DataValueField = "Value"
        country.DataBind()
        If ModuleDefaults.UseDefaultCountryOnRegistration Then
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            If defaultCountry <> String.Empty Then
                country.SelectedValue = defaultCountry
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        PopulateCountriesDropDownList()
        lblErrorMessage.Visible = (lblErrorMessage.Text.Length > 0)
    End Sub

    Protected Sub CreateJavascript()
        Dim sCRLF As String = vbCrLf
        Response.Write(sCRLF)
        Response.Write("<script language=""javascript"" type=""text/javascript"">" & sCRLF)
        Response.Write("function submitForm() {" & sCRLF)
        Response.Write("var postcode = document.forms[0]." & postcode.ClientID & ";" & sCRLF)
        Response.Write("var country = document.forms[0]." & country.ClientID & ";" & sCRLF)
        Response.Write("var errorLabel = document.forms[0]." & lblErrorMessage.ClientID & ";" & sCRLF)
        Response.Write("var postcodevalue = postcode.value;" & sCRLF)
        Response.Write("var countryvalue = country.value;" & sCRLF)
        Response.Write("if (trim(postcodevalue) == """"){" & sCRLF)
        Response.Write("errorLabel.value=""" & _wfr.Content("nopostcodeMessage", _languageCode, True) & """;" & sCRLF)
        Response.Write("postcode.focus();" & sCRLF)
        Response.Write("return false;" & sCRLF)
        Response.Write("}" & sCRLF)
        Response.Write("if (trim(countryvalue) == """"){" & sCRLF)
        Response.Write("errorLabel.text=""" & _wfr.Content("nocountryMessage", _languageCode, True) & """;" & sCRLF)
        Response.Write("country.focus();" & sCRLF)
        Response.Write("return false;" & sCRLF)
        Response.Write("}" & sCRLF)
        Response.Write("document.forms[0].hiddenCountry.value = countryvalue;")
        Response.Write("return true;" & sCRLF)
        Response.Write("}" & sCRLF)
        Response.Write("function trim(s) { " & vbCrLf & "var r=/\b(.*)\b/.exec(s); " & vbCrLf & "return (r==null)?"""":r[1]; " & vbCrLf & "}")
        Response.Write("</script>" & sCRLF)
    End Sub

End Class






