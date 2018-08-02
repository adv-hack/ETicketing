Imports System.Data
Imports System.Data.SqlClient

Partial Class Products_ProductCopy
    Inherits System.Web.UI.Page

    Dim productID As String
    Dim products As New ProductsTableAdapters.tbl_productTableAdapter
    Dim savedD1 As Boolean = False
    Dim savedD2 As Boolean = False
    Dim savedD3 As Boolean = False
    Dim savedD4 As Boolean = False
    Dim savedD5 As Boolean = False
    Dim savedH1 As Boolean = False
    Dim savedH2 As Boolean = False
    Dim savedH3 As Boolean = False
    Dim savedProductsToCopy As String = String.Empty
    Private conTalent As SqlConnection = Nothing
    Private cmdSelect As SqlCommand = Nothing
    Private dtrProduct As SqlDataReader = Nothing

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        productID = Request.QueryString("ID")

        If Not Session("D1") Is Nothing Then
            savedD1 = CType(Session("D1"), Boolean)
        End If
        If Not Session("D2") Is Nothing Then
            savedD2 = CType(Session("D2"), Boolean)
        End If
        If Not Session("D3") Is Nothing Then
            savedD3 = CType(Session("D3"), Boolean)
        End If
        If Not Session("D4") Is Nothing Then
            savedD4 = CType(Session("D4"), Boolean)
        End If
        If Not Session("D5") Is Nothing Then
            savedD5 = CType(Session("D5"), Boolean)
        End If
        If Not Session("H1") Is Nothing Then
            savedH1 = CType(Session("H1"), Boolean)
        End If
        If Not Session("H2") Is Nothing Then
            savedH2 = CType(Session("H2"), Boolean)
        End If
        If Not Session("H3") Is Nothing Then
            savedH3 = CType(Session("H3"), Boolean)
        End If
        If Not Session("ProductsToCopy") Is Nothing Then
            savedProductsToCopy = Session("ProductsToCopy").ToString
        End If

        ClearSession()

        If Page.IsPostBack Then
            ErrLabel.Text = ""
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Copy Product Descriptions"
        titleLabel.Text = master.HeaderText

        If IsPostBack = False Then
            setLabel()
            If savedD1 Then
                description1Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textDescription1.Text) Then
                    description1Check.Checked = True
                End If
            End If
            If savedD2 Then
                description2Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textDescription2.Text) Then
                    description2Check.Checked = True
                End If
            End If
            If savedD3 Then
                description3Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textDescription3.Text) Then
                    description3Check.Checked = True
                End If
            End If
            If savedD4 Then
                description4Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textDescription4.Text) Then
                    description4Check.Checked = True
                End If
            End If
            If savedD5 Then
                description5Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textDescription5.Text) Then
                    description5Check.Checked = True
                End If
            End If
            If savedH1 Then
                HTML1Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textHTML1.Text) Then
                    HTML1Check.Checked = True
                End If
            End If
            If savedH2 Then
                HTML2Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textHTML2.Text) Then
                    HTML2Check.Checked = True
                End If
            End If
            If savedH3 Then
                HTML3Check.Checked = True
            Else
                If Not String.IsNullOrEmpty(textHTML3.Text) Then
                    HTML3Check.Checked = True
                End If
            End If
            If Not String.IsNullOrEmpty(savedProductsToCopy) Then
                textProductsToCopy.Text = savedProductsToCopy
            End If
        End If
    End Sub

    Public Sub setLabel()
        Dim dt As DataTable
        dt = products.GetDataByID(productID)

        product.Text = dt.Rows(0)("PRODUCT_CODE").ToString
        textDescription1.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_1").ToString
        textDescription2.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_2").ToString
        textDescription3.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_3").ToString
        textDescription4.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_4").ToString
        textDescription5.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_5").ToString
        TextHTML1.Text = dt.Rows(0)("PRODUCT_HTML_1").ToString
        TextHTML2.Text = dt.Rows(0)("PRODUCT_HTML_2").ToString
        textHTML3.Text = dt.Rows(0)("PRODUCT_HTML_3").ToString

    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        'If Request.QueryString("find") Is Nothing Then
        Response.Redirect("ProductAdmin.aspx?" & GetQuerystrings())
        'Else
        '    Response.Redirect("ProductAdmin.aspx?find=" & Request.QueryString("find"))
        'End If
    End Sub

    Protected Sub ClearFormButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearForm.Click
        description1Check.Checked = False
        description2Check.Checked = False
        description3Check.Checked = False
        description4Check.Checked = False
        description5Check.Checked = False
        HTML1Check.Checked = False
        HTML2Check.Checked = False
        HTML3Check.Checked = False
    End Sub

    Protected Sub ConfirmButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfirmButton.Click
        Dim bError As Boolean = False
        Dim err As Boolean = False

        If String.IsNullOrEmpty(textProductsToCopy.Text) Then
            err = True
            ErrLabel.Text = "You must specify at least one product to copy to"
        Else
            If Not description1Check.Checked And Not description2Check.Checked And Not description3Check.Checked And Not description4Check.Checked And Not description5Check.Checked And Not HTML1Check.Checked And Not HTML2Check.Checked And Not HTML3Check.Checked Then
                err = True
                ErrLabel.Text = "You must check at least one box"
            Else
                ErrLabel.Text = ValidateProducts(textProductsToCopy.Text)
                If Not String.IsNullOrEmpty(ErrLabel.Text) Then
                    err = True
                End If
            End If
        End If

        If Not err Then
            Dim queryString As String = String.Empty

            If Not Request.QueryString("ID") Is Nothing Then
                queryString &= "?ID=" & Request.QueryString("ID")
                queryString &= "&" & GetQuerystrings()
            Else
                queryString &= "?" & GetQuerystrings()
            End If
            'If Not Request.QueryString("find") Is Nothing Then
            '    queryString &= "&find=" & Request.QueryString("find")
            'End If

            If description1Check.Checked Then
                Session("D1") = "true"
            End If
            If description2Check.Checked Then
                Session("D2") = "true"
            End If
            If description3Check.Checked Then
                Session("D3") = "true"
            End If
            If description4Check.Checked Then
                Session("D4") = "true"
            End If
            If description5Check.Checked Then
                Session("D5") = "true"
            End If
            If HTML1Check.Checked Then
                Session("H1") = "true"
            End If
            If HTML2Check.Checked Then
                Session("H2") = "true"
            End If
            If HTML3Check.Checked Then
                Session("H3") = "true"
            End If
            If Not String.IsNullOrEmpty(textProductsToCopy.Text) Then
                Session("ProductsToCopy") = textProductsToCopy.Text
            End If

            Response.Redirect("ProductCopyConfirmation.aspx" & queryString)
        End If
    End Sub

    Private Sub ClearSession()
        Session("D1") = Nothing
        Session("D2") = Nothing
        Session("D3") = Nothing
        Session("D4") = Nothing
        Session("D5") = Nothing
        Session("H1") = Nothing
        Session("H2") = Nothing
        Session("H3") = Nothing
        Session("ProductsToCopy") = Nothing
    End Sub

    Private Function ValidateProducts(ByVal s As String) As String
        Dim errText As String = String.Empty
        Dim connOpened As Boolean = False
        Dim errTextFilled As Boolean = False
        Dim strSelect As String = String.Empty

        Dim products() As String = s.Split(",")
        Dim i As Integer
        For i = 0 To products.Length - 1
            Dim productToCheck = products(i).Trim
            If Not String.IsNullOrEmpty(productToCheck) Then
                If productToCheck = product.Text Then
                    'The product to copy to list contains the product code of the product you are copying from
                    errText = "You cannot copy " + product.Text + " to itself"
                    errTextFilled = True
                    Return errText
                Else
                    'Open connection
                    If Not connOpened Then
                        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString)
                        conTalent.Open()
                        connOpened = True
                    End If

                    'Check product
                    strSelect = "SELECT * FROM TBL_PRODUCT WITH (NOLOCK) WHERE PRODUCT_CODE = @PRODUCT_CODE"
                    cmdSelect = New SqlCommand(strSelect, conTalent)
                    cmdSelect.Parameters.Add(New SqlParameter("@PRODUCT_CODE", SqlDbType.Char, 50)).Value = products(i).Trim
                    dtrProduct = cmdSelect.ExecuteReader()

                    If Not dtrProduct.HasRows() Then
                        If Not errTextFilled Then
                            errText = "The following product(s) do not exist: " & products(i).Trim
                            errTextFilled = True
                        Else
                            errText &= ", " & products(i).Trim
                        End If
                    End If
                    dtrProduct.Close()
                End If
            End If
        Next

        If connOpened Then
            'Close connection
            conTalent.Close()
        End If

        Return errText
    End Function

    Private Function GetQuerystrings() As String
        Dim linkQuerystring As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString("BU")) Then
            linkQuerystring = "BU=" & Request.QueryString("BU").Trim.ToUpper
        Else
            linkQuerystring = "BU=*ALL"
        End If
        If Not String.IsNullOrWhiteSpace(Request.QueryString("Partner")) Then
            linkQuerystring = linkQuerystring & "&Partner=" & Request.QueryString("Partner").Trim.ToUpper
        Else
            linkQuerystring = linkQuerystring & "&Partner=*ALL"
        End If
        Return linkQuerystring
    End Function
End Class
