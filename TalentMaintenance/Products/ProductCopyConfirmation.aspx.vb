Imports System.Data
Imports System.Data.SqlClient

Partial Class Products_ProductCopyConfirmation
    Inherits System.Web.UI.Page

    Dim productID As String
    Dim products As New ProductsTableAdapters.tbl_productTableAdapter
    Private conTalent As SqlConnection = Nothing
    Private cmdUpdate As SqlCommand = Nothing

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        productID = Request.QueryString("ID")

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
        End If
    End Sub

    Public Sub setLabel()
        Dim dt As DataTable
        dt = products.GetDataByID(productID)
        product.Text = dt.Rows(0)("PRODUCT_CODE").ToString

        If Not Session("D1") Is Nothing Then
            textDescription1.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_1").ToString
            trD1.Visible = True
        End If
        If Not Session("D2") Is Nothing Then
            textDescription2.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_2").ToString
            trD2.Visible = True
        End If
        If Not Session("D3") Is Nothing Then
            textDescription3.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_3").ToString
            trD3.Visible = True
        End If
        If Not Session("D4") Is Nothing Then
            textDescription4.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_4").ToString
            trD4.Visible = True
        End If
        If Not Session("D5") Is Nothing Then
            textDescription5.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_5").ToString
            trD5.Visible = True
        End If
        If Not Session("H1") Is Nothing Then
            textHTML1.Text = dt.Rows(0)("PRODUCT_HTML_1").ToString
            trh1.Visible = True
        End If
        If Not Session("H2") Is Nothing Then
            textHTML2.Text = dt.Rows(0)("PRODUCT_HTML_2").ToString
            trH2.Visible = True
        End If
        If Not Session("H3") Is Nothing Then
            textHTML3.Text = dt.Rows(0)("PRODUCT_HTML_3").ToString
            trH3.Visible = True
        End If

        If Not Session("ProductsToCopy") Is Nothing Then
            labelProductsToCopy.Text = CType(Session("ProductsToCopy"), String)
        End If

    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
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

        Response.Redirect("ProductCopy.aspx" & queryString)
    End Sub

    Protected Sub ConfirmButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfirmButton.Click
        Dim productString As String = String.Empty
        Dim strUpdate As New StringBuilder
        Dim strUpdate2 As String = String.Empty
        Dim strUpdate3 As String = String.Empty
        Dim products() As String = CType(Session("ProductsToCopy"), String).Split(",")
        Dim first As Boolean = False
        Dim i As Integer
        For i = 0 To products.Length - 1
            Dim productToCheck = products(i).Trim
            If Not String.IsNullOrEmpty(productToCheck) Then
                If Not first Then
                    productString = "'" & products(i).Trim & "'"
                    first = True
                Else
                    productString &= ", '" & products(i).Trim & "'"
                End If
            End If
        Next

        'Open connection
        conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString)
        conTalent.Open()

        first = False
        With strUpdate
            .Append("UPDATE TBL_PRODUCT SET ")

            If Not Session("D1") Is Nothing Then
                .Append("PRODUCT_DESCRIPTION_1 = @PRODUCT_DESCRIPTION_1")
                first = True
            End If

            If Not Session("D2") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_DESCRIPTION_2 = @PRODUCT_DESCRIPTION_2")
            End If

            If Not Session("D3") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_DESCRIPTION_3 = @PRODUCT_DESCRIPTION_3")
            End If

            If Not Session("D4") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_DESCRIPTION_4 = @PRODUCT_DESCRIPTION_4")
            End If

            If Not Session("D5") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_DESCRIPTION_5 = @PRODUCT_DESCRIPTION_5")
            End If

            If Not Session("H1") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_HTML_1 = @PRODUCT_HTML_1")
            End If

            If Not Session("H2") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_HTML_2 = @PRODUCT_HTML_2")
            End If

            If Not Session("H3") Is Nothing Then
                If Not first Then
                    first = True
                Else
                    .Append(", ")
                End If
                .Append("PRODUCT_HTML_3 = @PRODUCT_HTML_3")
            End If

            .Append(" WHERE PRODUCT_CODE IN ")
            .Append("(")
            .Append(productString)
            .Append(")")
        End With

        Dim s As String = strUpdate.ToString

        cmdUpdate = New SqlCommand(strUpdate.ToString, conTalent)
        'cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_CODES", SqlDbType.Char)).Value = productString
        If Not Session("D1") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION_1", SqlDbType.Char)).Value = textDescription1.Text
        End If
        If Not Session("D2") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION_2", SqlDbType.Char)).Value = textDescription2.Text
        End If
        If Not Session("D3") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION_3", SqlDbType.Char)).Value = textDescription3.Text
        End If
        If Not Session("D4") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION_4", SqlDbType.Char)).Value = textDescription4.Text
        End If
        If Not Session("D5") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_DESCRIPTION_5", SqlDbType.Char)).Value = textDescription5.Text
        End If
        If Not Session("H1") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_HTML_1", SqlDbType.Char)).Value = textHTML1.Text
        End If
        If Not Session("H2") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_HTML_2", SqlDbType.Char)).Value = textHTML2.Text
        End If
        If Not Session("H3") Is Nothing Then
            cmdUpdate.Parameters.Add(New SqlParameter("@PRODUCT_HTML_3", SqlDbType.Char)).Value = textHTML3.Text
        End If

        Try
            cmdUpdate.ExecuteNonQuery()
        Catch ex As Exception
        End Try

        'Close connection
        conTalent.Close()

        'ErrLabel.Text = "Successfully copied"
        'hypProductList.Visible = True

        'Dim queryString As String = String.Empty
        'If Not Request.QueryString("find") Is Nothing Then
        '    queryString &= "?find=" & Request.QueryString("find")
        'End If
        'hypProductList.NavigateUrl = "~/Products/ProductAdmin.aspx" & queryString
        Response.Redirect("ProductAdmin.aspx?copied=" + product.Text & "&" & GetQuerystrings())

    End Sub

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
