Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Price Break Table
'                                   Written as part of Simon Jersey new development
'
'       Date                        29/09/09
'
'       Author                      Ben
'
'       CS Group 2009               All rights reserved.
'
'       Error Number Code base      - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------


Partial Class UserControls_PriceBreakTable
    Inherits ControlBase

    Private _display As Boolean
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PriceBreakTable.ascx"
        End With
        Dim productPrice As New Talent.Common.DEWebPrice
        If Display Then
            productPrice = Talent.eCommerce.Utilities.GetWebPrices(Request("product"), 0, Request("product"))
            '-----------------------------------------------------------------------------------
            ' Check if there are price breaks - have to do it this way as Simon Jersey set a lot 
            ' of their products so all price breaks are 1
            '-----------------------------------------------------------------------------------
            If productPrice.PRICE_BREAK_QUANTITY_2 > productPrice.PRICE_BREAK_QUANTITY_1 Then
                BuildPriceBreakTable(productPrice)
            Else
                pnlPriceBreakTable.Visible = False
            End If
        Else
            pnlPriceBreakTable.Visible = False
        End If
    End Sub

    Private Sub BuildPriceBreakTable(ByVal prc As Talent.Common.DEWebPrice)
        Dim sb As New System.Text.StringBuilder
        Dim price1, price2, price3, price4, price5, price6, price7, price8, price9, price10 As Decimal

        '-------------------------------------------------
        ' Set each unit price break depending on gross/net
        '-------------------------------------------------
        If ModuleDefaults.ShowPricesExVAT Then
            price1 = prc.NET_PRICE
            price2 = prc.NET_PRICE_2
            price3 = prc.NET_PRICE_3
            price4 = prc.NET_PRICE_4
            price5 = prc.NET_PRICE_5
            price6 = prc.NET_PRICE_6
            price7 = prc.NET_PRICE_7
            price8 = prc.NET_PRICE_8
            price9 = prc.NET_PRICE_9
            price10 = prc.NET_PRICE_10
        Else
            price1 = prc.GROSS_PRICE
            price2 = prc.GROSS_PRICE_2
            price3 = prc.GROSS_PRICE_3
            price4 = prc.GROSS_PRICE_4
            price5 = prc.GROSS_PRICE_5
            price6 = prc.GROSS_PRICE_6
            price7 = prc.GROSS_PRICE_7
            price8 = prc.GROSS_PRICE_8
            price9 = prc.GROSS_PRICE_9
            price10 = prc.GROSS_PRICE_10
        End If

        '----------------------------------
        ' Build Multibuy table as per below
        '----------------------------------

        '  <table cellspacing="0">
        '    <tr>
        '      <th scope="row">Quantity</th>
        '      <td>1&ndash;4</td>
        '      <td>5&ndash;9</td>
        '      <td>10&ndash;19</td>
        '      <td>20+</td>
        '    </tr>
        '    <tr>
        '      <th scope="row">Unit price</th>
        '      <td>&pound;14.99</td>
        '      <td>&pound;12.99</td>
        '      <td>&pound;11.99</td>
        '      <td>&pound;9.99</td>
        '    </tr>
        '  </table>
        With sb
            '  .Append("<p class=""header"">").Append(ucr.Content("priceBreakTableHeader", _languageCode, True)).Append("</p>")
            .Append(" <table cellspacing=""0"">")
            .Append("<tr>")
            '-------------
            ' Quantity Row
            '-------------
            .Append("<th scope=""row"">").Append(ucr.Content("quantityLabel", _languageCode, True)).Append("</th>")
            If prc.PRICE_BREAK_QUANTITY_2 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_2)).Append("</td>")
            End If

            If prc.PRICE_BREAK_QUANTITY_3 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_2 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_3)).Append("</td>")

            Else
                If prc.PRICE_BREAK_QUANTITY_2 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_2 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_4 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_3 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_4)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_3 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_3 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_5 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_4 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_5)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_4 Then

                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_4 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_6 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_5 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_6)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_5 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_5 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_7 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_6 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_7)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_6 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_6 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_8 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_7 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_8)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_7 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_7 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_9 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_8 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_9)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_8 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_8 + 1)).Append("+").Append("</td>")
                End If
            End If

            If prc.PRICE_BREAK_QUANTITY_10 > 0 Then
                .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_9 + 1)).Append("&ndash;").Append(CInt(prc.PRICE_BREAK_QUANTITY_10)).Append("</td>")
            Else
                If prc.PRICE_BREAK_QUANTITY_9 > 0 Then
                    .Append("<td>").Append(CInt(prc.PRICE_BREAK_QUANTITY_9 + 1)).Append("+").Append("</td>")
                End If
            End If

            .Append("</tr>")
            '---------------
            ' Unit Price Row
            '---------------
            .Append("<tr>")
            .Append("<th scope=""row"">").Append(ucr.Content("unitPriceLabel", _languageCode, True)).Append("</th>")
            If price1 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price1, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price2 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price2, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price3 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price3, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price4 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price4, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price5 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price5, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price6 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price6, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price7 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price7, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price8 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price8, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price9 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price9, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            If price10 > 0 Then
                .Append("<td>").Append(TDataObjects.PaymentSettings.FormatCurrency(price10, ucr.BusinessUnit, ucr.PartnerCode)).Append("</td>")
            End If
            .Append("</tr>")

            .Append("</table>")
        End With

        lblPriceBreakTable.Text = sb.ToString
        lblPriceBreakHeader.Text = ucr.Content("priceBreakTableHeader", _languageCode, True)
        lblDisclaimerHeader.Text = ucr.Content("disclaimerHeader", _languageCode, True)
        lblDisclaimerText.Text = ucr.Content("disclaimerText", _languageCode, True)

    End Sub

End Class
