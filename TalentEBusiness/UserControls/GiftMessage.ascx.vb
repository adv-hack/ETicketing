Imports System.Data.SqlClient

Partial Class UserControls_GiftMessage
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim ucr As New Talent.Common.UserControlResource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "GiftMessage.ascx"
        End With
        If Not Page.IsPostBack Then
            setupText()
            LoadMessage()
        End If
    End Sub

    Protected Sub LoadMessage()
        Const SelectStr As String = " SELECT * " & _
                                    " FROM tbl_gift_message WITH (NOLOCK)  " & _
                                    " WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    " AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    " AND PARTNER = @PARTNER "

        Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Try
            cmd.Connection.Open()
            With cmd.Parameters
                .Clear()
                .Add("@TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = Profile.Basket.TempOrderID
                .Add("@BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
            End With

            Dim dr As SqlDataReader = cmd.ExecuteReader

            If dr.HasRows Then
                dr.Read()
                toBox.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("RECIPIENT_NAME"))
                msgBox.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("MESSAGE"))
            End If
            dr.Close()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

    End Sub

    Protected Sub setupText()
        With ucr
            titleLabel.Text = ucr.Content("titleLabel", _languageCode, True)
            instructionsLabel.Text = ucr.Content("instructionsLabel", _languageCode, True)
            toLabel.Text = ucr.Content("toNameLabel", _languageCode, True)
            msgLabel.Text = ucr.Content("messageLabel", _languageCode, True)
            submitBtn.Text = ucr.Content("submitButton", _languageCode, True)

            toRequired.ErrorMessage = ucr.Content("toNameRequiredError", _languageCode, True)
            toRequired.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("toNameRequiredEnabled"))
            toRegEx.ErrorMessage = ucr.Content("toNameInvalidError", _languageCode, True)
            toRegEx.ValidationExpression = ucr.Attribute("toNameRegularExpression")
            toRegEx.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("toNameRegularExpressionEnabled"))

            msgRequired.ErrorMessage = ucr.Content("messageRequiredError", _languageCode, True)
            msgRequired.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("messageRequiredEnabled"))
            msgRegEx.ErrorMessage = ucr.Content("messageInvalidError", _languageCode, True)
            msgRegEx.ValidationExpression = ucr.Attribute("messageRegularExpression")
            msgRegEx.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("messageRegularExpressionEnabled"))
        End With

    End Sub

    Protected Sub submitBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitBtn.Click
        Const SelectStr As String = " IF EXISTS( " & _
                                    "           SELECT * FROM tbl_gift_message WITH (NOLOCK)  " & _
                                    "           WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    "           AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "           AND PARTNER = @PARTNER) " & _
                                    "   BEGIN " & _
                                    "       UPDATE tbl_gift_message " & _
                                    "       SET RECIPIENT_NAME = @RECIPIENT_NAME, " & _
                                    "           MESSAGE = @MESSAGE " & _
                                    "       WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                                    "       AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "       AND PARTNER = @PARTNER " & _
                                    "   END " & _
                                    " ELSE " & _
                                    "   BEGIN " & _
                                    "       INSERT INTO tbl_gift_message " & _
                                    "       VALUES( " & _
                                    "               @BUSINESS_UNIT, " & _
                                    "               @PARTNER, " & _
                                    "               @TEMP_ORDER_ID, " & _
                                    "               @RECIPIENT_NAME, " & _
                                    "               @MESSAGE) " & _
                                    "   END "

        Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Try
            cmd.Connection.Open()
            With cmd.Parameters
                .Clear()
                .Add("@TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = Profile.Basket.TempOrderID
                .Add("@BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                .Add("@PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                .Add("@RECIPIENT_NAME", Data.SqlDbType.NVarChar).Value = toBox.Text
                .Add("@MESSAGE", Data.SqlDbType.NVarChar).Value = msgBox.Text
            End With

            cmd.ExecuteNonQuery()

        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        If Not String.IsNullOrEmpty(Request("RedirectTarget")) Then
            Response.Redirect(Request("RedirectTarget"))
        ElseIf Not String.IsNullOrEmpty(ucr.Attribute("RedirectTarget")) Then
            Response.Redirect(ucr.Attribute("RedirectTarget"))
        Else
            Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
        End If
    End Sub
End Class
