Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common

Partial Class PagesPublic_HopewiserSearch
    Inherits TalentBase01

    Private _wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _err As New Talent.Common.ErrorObj
    Private _ds1 As DataSet = Nothing

    Public SearchResults As String
    Public Property CountryString() As String
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ds1 = New DataSet
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = "HopewiserSearch.aspx"
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "HopewiserSearch.aspx"
        End With
        ButtonPrev.NavigateUrl = "../../PagesPublic/Hopewiser/HopewiserPostcodeAndCountry.aspx"
        If Request("country") IsNot Nothing AndAlso TypeOf (Request("country")) Is String Then
            CountryString = Request("country").Trim
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        DoHopewiserSearch()
        RenderSearchResults()
        lblErrorMessage.Text = GetErrorMessage()
        lblErrorMessage.Visible = (lblErrorMessage.Text.Length > 0)
    End Sub

    Protected Function GetPageText(ByVal sKey As String) As String
        Dim strString As String = _wfr.Content(sKey, _languageCode, True)
        Return strString.Trim
    End Function

    Protected Sub DoHopewiserSearch()
        Try
            Dim addressing As New Talent.Common.TalentAddressing
            Dim settings As New Talent.Common.DESettings
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            addressing.Settings = settings
            addressing.De.PostalCode = Request("postcode").Trim
            addressing.De.AddressingProvider = "Hopewiser"
            _err = addressing.AddressList()
            _ds1 = addressing.ResultDataSet
        Catch ex As Exception

        End Try
    End Sub

    Private Sub RenderSearchResults()
        Try
            Dim dt As DataTable
            Dim dr As DataRow
            Dim intCount As Integer = 0
            Dim sListItemString As String = String.Empty
            Dim sCompany As String = String.Empty
            Dim sAddress1 As String = String.Empty
            Dim sAddress2 As String = String.Empty
            Dim sTown As String = String.Empty
            Dim sCounty As String = String.Empty
            Dim sPostcode As String = String.Empty
            Dim sCountry As String = String.Empty

            Dim sValueString As String = String.Empty
            Dim sFuncString As String = String.Empty
            Dim sComma As String = ","
            Dim sSep As String = "', '"
            Dim sCRLF As String = vbCrLf

            If Not _err.HasError Then
                If _ds1.Tables(1).Rows.Count > 0 Then
                    dt = _ds1.Tables(1)
                    For Each dr In dt.Rows
                        sCompany = dr(0).ToString.Trim.Replace(sComma, String.Empty)
                        sAddress1 = dr(1).ToString.Trim.Replace(sComma, String.Empty)
                        sAddress2 = dr(2).ToString.Trim.Replace(sComma, String.Empty)
                        sTown = dr(3).ToString.Trim.Replace(sComma, String.Empty)
                        sCounty = dr(4).ToString.Trim.Replace(sComma, String.Empty)
                        sPostcode = dr(5).ToString.Trim & " " & dr(6).ToString.Trim
                        sCountry = Request("country").Trim
                        sListItemString = GetListItemString(dr)
                        If sListItemString <> "" Then
                            sValueString = sCompany & sComma & sAddress1 & sComma & sAddress2 & sComma & sTown & sComma & sCounty & sComma & sPostcode
                            sFuncString = sCompany & sSep & sAddress1 & sSep & sAddress2 & sSep & sTown & sSep & sCounty & sSep & sPostcode & sSep & sCountry
                            SearchResults += "<input type=""radio"" name=""address"" value=""" & sValueString & """> <a href=""javascript:DoOpener('" & sFuncString & "')"">" & sListItemString & "</a><br />"
                        End If
                        intCount = intCount + 1
                    Next
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Function GetListItemString(ByVal dr As DataRow) As String
        Dim sReturnString As String = String.Empty
        Dim sCompany As String = dr(0).ToString.Trim
        Dim sAddress1 As String = dr(1).ToString.Trim
        Dim sAddress2 As String = dr(2).ToString.Trim
        Dim sTown As String = dr(3).ToString.Trim
        Dim sCounty As String = dr(4).ToString.Trim
        Dim sPostcode1 As String = dr(5).ToString.Trim
        Dim sPostcode2 As String = dr(6).ToString.Trim

        If sCompany <> "" Then
            sReturnString = sCompany
        End If
        If sAddress1 <> "" Then
            If sReturnString.Trim <> "" Then
                sReturnString = sReturnString & ", "
            End If
            sReturnString = sReturnString & sAddress1
        End If
        If sAddress2 <> "" Then
            If sReturnString.Trim <> "" Then
                sReturnString = sReturnString & ", "
            End If
            sReturnString = sReturnString & sAddress2
        End If
        If sTown <> "" Then
            If sReturnString.Trim <> "" Then
                sReturnString = sReturnString & ", "
            End If
            sReturnString = sReturnString & sTown
        End If
        If sCounty <> "" Then
            If sReturnString.Trim <> "" Then
                sReturnString = sReturnString & ", "
            End If
            sReturnString = sReturnString & sCounty
        End If
        If sPostcode1 <> "" Then
            If sReturnString.Trim <> "" Then
                sReturnString = sReturnString & ", "
            End If
            sReturnString = sReturnString & sPostcode1 & " " & sPostcode2
        End If
        Return sReturnString
    End Function

    Protected Sub CreateJavascript()
        Dim sCRLF As String = vbCrLf
        Response.Write(sCRLF)
        Response.Write("<script language=""javascript"" type=""text/javascript"">" & sCRLF)
        Response.Write("function ShowSelectAddressMessage() {" & sCRLF)
        Response.Write("var errorLabel = document.forms[0]." & lblErrorMessage.ClientID & ";" & sCRLF)
        Response.Write("errorLabel.value=""" & _wfr.Content("selectaddressMessage", _languageCode, True) & """;" & sCRLF)
        Response.Write("}" & sCRLF)
        Response.Write("</script>" & sCRLF)
    End Sub

    Protected Function GetErrorMessage() As String
        Dim sReturnString As String = String.Empty
        If Not _err.HasError Then
            If _ds1.Tables(1).Rows.Count = 0 Then
                sReturnString = _wfr.Content("noresultsMessage", _languageCode, True)
            End If
        Else
            If _ds1.Tables(0).Rows(0).Item("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                sReturnString = _wfr.Content("noresultsMessage", _languageCode, True)
            End If
        End If
        Return sReturnString
    End Function
End Class






