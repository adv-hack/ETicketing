Imports Talent.eCommerce
Imports System.Data
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Advanced Search Type 2
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
Partial Class UserControls_advancedSearchType2
    Inherits ControlBase

    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private businessUnit As String = TalentCache.GetBusinessUnit()
    Private partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
    Private _group1 As String = String.Empty

    Public Property Group1() As String
        Get
            Return _group1
        End Get
        Set(ByVal value As String)
            _group1 = value
        End Set
    End Property

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Private _queryString As String
    Public Property QueryString() As String
        Get
            Return _queryString
        End Get
        Set(ByVal value As String)
            _queryString = value
        End Set
    End Property

    Public Sub BuildPage()

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "advancedSearchType2.ascx"
            .PageCode = UCase(Usage)
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)

            keywordLabel.Text = .Content("Keyword", _languageCode, True)
            priceLabel.Text = .Content("Price", _languageCode, True)
            toLabel.Text = .Content("To", _languageCode, True)
            countryLabel.Text = .Content("Country", _languageCode, True)
            regionLabel.Text = .Content("Region", _languageCode, True)
            grapeLabel.Text = .Content("Grape", _languageCode, True)
            ABVLabel.Text = .Content("ABV", _languageCode, True)
            typeLabel.Text = .Content("Type", _languageCode, True)
            areaLabel.Text = .Content("Area", _languageCode, True)
            producerLabel.Text = .Content("Producer", _languageCode, True)
            checkPositiveNumericFrom.ErrorMessage = .Content("FromPositiveErrorMessage", _languageCode, True)
            checkPositiveNumericTo.ErrorMessage = .Content("ToPositiveErrorMessage", _languageCode, True)
            checkRangeOK.ErrorMessage = .Content("RangeErrorMessage", _languageCode, True)
        End With

        'Extract from query string - in case "search again" used or want to book mark search page
        Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString

        ' Keywords
        Dim i As Integer = 1
        keyword.Text = String.Empty
        While Not queryString.Item("keyword" & i.ToString) Is Nothing
            If i <> 1 Then
                keyword.Text &= " "
            End If
            keyword.Text &= queryString.Item("keyword" & i.ToString)
            i += 1
        End While
        ' Price Range
        If Not queryString.Item("pricefr") Is Nothing Then
            Try
                priceFrom.Text = queryString.Item("pricefr")
            Catch ex As Exception
            End Try
        End If
        If Not queryString.Item("priceto") Is Nothing Then
            Try
                priceTo.Text = queryString.Item("priceto")
            Catch ex As Exception
            End Try
        End If

        ' Get criteria to display
        Dim useAll As Boolean = False

        ' Criteria 1 = Type
        Dim ps As New ProductSearch

        ' Try default partner first
        Dim dt As DataTable = ps.GetCriteria(businessUnit, partner, Group1(), 1)

        If dt.Rows.Count < 1 Then
            ' Then try *ALL partner
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 1)
            useAll = True
        End If

        i = 1
        ' Add the All section
        type.Items.Clear()
        type.Items.Add(New ListItem(ucr.Content("TypeAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    type.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria01") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria01")) Then
                            type.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

        ' Criteria 2 = Country
        If useAll Then
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 2)
        Else
            dt = ps.GetCriteria(businessUnit, partner, Group1(), 2)
            If dt.Rows.Count < 1 Then
                dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 2)
            End If
        End If
        ' Country
        i = 1
        ' Add the All section
        country.Items.Clear()
        country.Items.Add(New ListItem(ucr.Content("CountryAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    country.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria02") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria02")) Then
                            country.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

        ' Criteria 3 = Region
        If useAll Then
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 3)
        Else
            dt = ps.GetCriteria(businessUnit, partner, Group1(), 3)
            If dt.Rows.Count < 1 Then
                dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 3)
            End If
        End If
        ' Country
        i = 1
        ' Add the All section
        region.Items.Clear()
        region.Items.Add(New ListItem(ucr.Content("RegionAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    region.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria03") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria03")) Then
                            region.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

        ' Criteria 4 = Area
        If useAll Then
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 4)
        Else
            dt = ps.GetCriteria(businessUnit, partner, Group1(), 4)
            If dt.Rows.Count < 1 Then
                dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 4)
            End If
        End If
        ' Country
        i = 1
        ' Add the All section
        area.Items.Clear()
        area.Items.Add(New ListItem(ucr.Content("AreaAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    area.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria04") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria04")) Then
                            area.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

        ' Criteria 5 = Grape
        If useAll Then
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 5)
        Else
            dt = ps.GetCriteria(businessUnit, partner, Group1(), 5)
            If dt.Rows.Count < 1 Then
                dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 5)
            End If
        End If
        ' Country
        i = 1
        ' Add the All section
        grape.Items.Clear()
        grape.Items.Add(New ListItem(ucr.Content("GrapeAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    grape.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria05") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria05")) Then
                            grape.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

        ' Criteria 6 = ABV
        If useAll Then
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 6)
        Else
            dt = ps.GetCriteria(businessUnit, partner, Group1(), 6)
            If dt.Rows.Count < 1 Then
                dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 6)
            End If
        End If
        ' Country
        i = 1
        ' Add the All section
        abv.Items.Clear()
        abv.Items.Add(New ListItem(ucr.Content("ABVAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    abv.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria06") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria06")) Then
                            abv.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

        ' Criteria 7 = Producer
        If useAll Then
            dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 7)
        Else
            dt = ps.GetCriteria(businessUnit, partner, Group1(), 7)
            If dt.Rows.Count < 1 Then
                dt = ps.GetCriteria(businessUnit, Talent.Common.Utilities.GetAllString, Group1(), 7)
            End If
        End If
        ' Country
        i = 1
        ' Add the All section
        producer.Items.Clear()
        producer.Items.Add(New ListItem(ucr.Content("ProducerAllText", _languageCode, True), i.ToString))

        ' And the rest of the items
        If dt.Rows.Count > 0 Then
            For Each row As DataRow In dt.Rows
                i += 1
                If row("CRITERIA").Equals(String.Empty) Or _
                    row("CRITERIA") Is Nothing Then
                Else
                    producer.Items.Add(New ListItem(row("CRITERIA"), i.ToString))
                    ' Set selected item from query string
                    If Not queryString.Item("criteria07") Is Nothing Then
                        If row("CRITERIA").Equals(queryString.Item("criteria07")) Then
                            producer.Items(i - 1).Selected = True
                        End If
                    End If
                End If
            Next
        End If

    End Sub

    Public Function BuildQueryString() As String

        If Not keyword.Text.Equals(String.Empty) Then
            Dim keywordArray() As String = keyword.Text.Split(" ")
            Dim i As Integer = 0
            For i = 0 To keywordArray.Length - 1
                QueryString &= AppendQueryItem("keyword" & (i + 1).ToString, keywordArray(i))
            Next
        End If

        If Not priceFrom.Text.Equals(String.Empty) Then
            QueryString &= AppendQueryItem("pricefr", priceFrom.Text)
        End If

        If Not priceTo.Text.Equals(String.Empty) Then
            QueryString &= AppendQueryItem("priceto", priceTo.Text)
        End If

        If type.SelectedValue > 1 Then
            QueryString &= "&criteria01=" & type.SelectedItem.Text
        End If

        If country.SelectedValue > 1 Then
            QueryString &= "&criteria02=" & country.SelectedItem.Text
        End If

        If region.SelectedValue > 1 Then
            QueryString &= "&criteria03=" & region.SelectedItem.Text
        End If

        If area.SelectedValue > 1 Then
            QueryString &= "&criteria04=" & area.SelectedItem.Text
        End If

        If grape.SelectedValue > 1 Then
            QueryString &= "&criteria05=" & grape.SelectedItem.Text
        End If

        If abv.SelectedValue > 1 Then
            QueryString &= "&criteria06=" & abv.SelectedItem.Text
        End If

        If producer.SelectedValue > 1 Then
            QueryString &= "&criteria07=" & producer.SelectedItem.Text
        End If

        Return QueryString()
    End Function

    Private Function AppendQueryItem(ByVal name As String, ByVal value As String) As String
        Dim item As String = String.Empty
        item &= "&" & name & "=" & value
        Return item
    End Function

    Protected Sub checkPositiveNumericFrom_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles checkPositiveNumericFrom.ServerValidate
        If priceFrom.Text.Equals(String.Empty) And Not priceTo.Text.Equals(String.Empty) Then
            priceFrom.Text = "0"
        End If
        If priceTo.Text.Equals(String.Empty) And Not priceFrom.Text.Equals(String.Empty) Then
            priceTo.Text = "0"
        End If
        args.IsValid = checkPositiveNumeric(priceFrom.Text)
    End Sub

    Protected Sub checkPositiveNumericTo_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles checkPositiveNumericTo.ServerValidate
        If priceFrom.Text.Equals(String.Empty) And Not priceTo.Text.Equals(String.Empty) Then
            priceFrom.Text = "0"
        End If
        If priceTo.Text.Equals(String.Empty) And Not priceFrom.Text.Equals(String.Empty) Then
            priceTo.Text = "0"
        End If
        args.IsValid = checkPositiveNumeric(priceTo.Text)
    End Sub

    Private Function checkPositiveNumeric(ByVal text As String) As Boolean
        Dim valid As Boolean = True
        Dim value As Decimal
        Try
            value = Decimal.Parse(text)
        Catch
            valid = False
        End Try
        If valid And value < 0 Then
            valid = False
        End If
        Return valid
    End Function

    Protected Sub checkRangeOK_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles checkRangeOK.ServerValidate
        Dim valueTo As Decimal
        Dim valueFrom As Decimal
        Try
            valueFrom = Decimal.Parse(priceFrom.Text)
            valueTo = Decimal.Parse(priceTo.Text)
            If valueFrom > valueTo Then
                args.IsValid = False
            End If
        Catch ex As Exception
        End Try

    End Sub

End Class
