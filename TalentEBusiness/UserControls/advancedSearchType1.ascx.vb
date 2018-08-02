'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Advanced Search Type 1
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
Partial Class UserControls_advancedSearchType1
    Inherits ControlBase

    Private _usage As String = Talent.Common.Utilities.GetAllString
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
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
            .KeyCode = "advancedSearchType1.ascx"
            .PageCode = UCase(Usage)
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)

            keywordLabel.Text = .Content("Keyword", _languageCode, True)
            priceLabel.Text = .Content("Price", _languageCode, True)
            toLabel.Text = .Content("To", _languageCode, True)
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
        args.IsValid = checkPositiveNumeric(priceFrom.Text)
    End Sub

    Protected Sub checkPositiveNumericTo_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles checkPositiveNumericTo.ServerValidate
        If priceFrom.Text.Equals(String.Empty) And Not priceTo.Text.Equals(String.Empty) Then
            priceFrom.Text = "0"
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
            If Not String.IsNullOrEmpty(priceTo.Text) Then
                valueTo = Decimal.Parse(priceTo.Text)
                If valueFrom > valueTo Then
                    args.IsValid = False
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

End Class
