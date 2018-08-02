Imports Talent.Common
Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Security.Cryptography
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Punchout to required URL
'
'       Date                        30/04/11
'
'       Author                      Ben Ford
'
'       CS Group 2009               All rights reserved.
'
'       Error Number Code base       
'
'       User Controls
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class Redirect_Punchout
    Inherits Base01

    Private _defaults As New Talent.eCommerce.ECommerceModuleDefaults
    Private _def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Public formActionUrl As String = ""
    Public formTarget As String = ""
    Public formVisible As String = "False"
    Public formStyle As String = "display:none"
    Public inputType As String = "hidden"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' TODO : COMMENT FOR GOING LIVE
        'formVisible = "True"
        'formStyle = ""
        'inputType = "text"

        ' TODO ENd

        _def = _defaults.GetDefaults()
        If _def.ValidatePartnerDirectAccess AndAlso _def.SapOciPartner = TalentCache.GetPartner(HttpContext.Current.Profile).ToString() Then
            If HttpContext.Current.Session("SAP_OCI_HOOK_URL") IsNot Nothing Then
                ' TODO : UNCOMMENT FOR GOING LIVE
                formActionUrl = Session("SAP_OCI_HOOK_URL").ToString
                ' TODO End
                FormsAuthentication.SignOut()
                Session.Remove("SAP_OCI_HOOK_URL")
                Session.Remove("SAP_OCI_REFERER_URL")
            Else
                HttpContext.Current.Response.Redirect("~/PagesPublic/Error/SessionError.aspx?errortype=SAPOCIERROR")
            End If
        End If
    End Sub


    '----------------------------------------------------------------------
    ' Write out form using normal HTML tags 
    ' (can't use form runat=server as this always posts back to the server)
    '----------------------------------------------------------------------
    Function WriteForm() As String

        'TODO : MARK BASKET AS PROCESSED IF EVERYTHING IS FINE BEFORE WRITE THE FORM

        Dim returnString As String = ""
        Dim sb As New StringBuilder

        '' Form variables
        Dim httpVersion As String = "2.0"

        'NEW_ITEM-DESCRIPTION[n]
        'NEW_ITEM-QUANTITY[n]
        'NEW_ITEM-UNIT[n] ???
        'NEW_ITEM-PRICE[n]
        'NEW_ITEM-CURRENCY[n]
        'NEW_ITEM-LONGTEXT_n:132[]
        'NEW_ITEM-VENDORMAT[n]
        'NEW_ITEM-MATGROUP[n]

        Dim productData As New TalentProductInformationTableAdapters.tbl_productTableAdapter
        Dim dt As DataTable
        Dim productUOM As String = String.Empty
        Dim productUNSPSC As String = String.Empty
        Dim productLongDescription As String = String.Empty
        Dim productShortDescription As String = String.Empty
        Dim productAlternateSKU As String = String.Empty
        Dim tDataObjects As New TalentDataObjects
        Dim currencyCode As String = String.Empty
        Dim count As Integer = 1

        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        currencyCode = tDataObjects.PaymentSettings.GetCurrencyCode(TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner())

        ' Loop through basket populating form
        For Each basketItem As TalentBasketItem In Profile.Basket.BasketItems
            ' Get UOM, alt sku & UNSPSC off product definition
            productUOM = String.Empty
            productUNSPSC = String.Empty
            productLongDescription = String.Empty
            productAlternateSKU = String.Empty

            dt = productData.GetDataByProduct_Code(basketItem.Product)
            If dt.Rows.Count > 0 Then
                productUOM = dt.Rows(0)("PRODUCT_SIZE_UOM").ToString
                productUNSPSC = dt.Rows(0)("PRODUCT_CATALOG_CODE").ToString
                productLongDescription = dt.Rows(0)("PRODUCT_DESCRIPTION_5").ToString
                productShortDescription = dt.Rows(0)("PRODUCT_DESCRIPTION_1").ToString
                'productAlternateSKU = dt.Rows(0)("ALTERNATE_SKU").ToString
                'productAlternateSKU = dt.Rows(0)("ALTERNATE_SKU").ToString
                If productLongDescription = String.Empty Then
                    productLongDescription = productShortDescription
                End If
            End If


            'With sb
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-DESCRIPTION[" & count.ToString & "]' name='NEW_ITEM-DESCRIPTION[" & count.ToString & _
            '                "]' readonly='readonly' value='" & productShortDescription & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-QUANTITY[" & count.ToString & "]' name='NEW_ITEM-QUANTITY[" & count.ToString & _
            '                "]' readonly='readonly' value='" & basketItem.Quantity & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-UNIT[" & count.ToString & "]' name='NEW_ITEM-UNIT[" & count.ToString & _
            '                "]' readonly='readonly' value='" & productUOM & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-PRICE[" & count.ToString & "]' name='NEW_ITEM-PRICE[" & count.ToString & _
            '               "]' readonly='readonly' value='" & basketItem.Net_Price.ToString & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-CURRENCY[" & count.ToString & "]' name='NEW_ITEM-CURRENCY[" & count.ToString & _
            '               "]' readonly='readonly' value='" & currencyCode & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-LONGTEXT_" & count.ToString & ":132[]' name='NEW_ITEM-LONGTEXT_" & count.ToString & _
            '                ":132[]' readonly='readonly' value='" & productLongDescription & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-VENDORMAT[" & count.ToString & "]' name='NEW_ITEM-VENDORMAT[" & count.ToString & _
            '               "]' readonly='readonly' value='" & basketItem.Product & "' />").Append("<br />")
            '    .Append("<input type='" & inputType & " ' id='NEW_ITEM-MATGROUP[" & count.ToString & "]' name='NEW_ITEM-MATGROUP[" & count.ToString & _
            '              "]' readonly='readonly' value='" & productUNSPSC & "' />").Append("<br />")
            'End With
            With sb
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-DESCRIPTION[" & count.ToString & "]' name='NEW_ITEM-DESCRIPTION[" & count.ToString & _
                            "]' readonly='readonly' value='" & productLongDescription & "' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-QUANTITY[" & count.ToString & "]' name='NEW_ITEM-QUANTITY[" & count.ToString & _
                            "]' readonly='readonly' value='" & basketItem.Quantity & "' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-UNIT[" & count.ToString & "]' name='NEW_ITEM-UNIT[" & count.ToString & _
                            "]' readonly='readonly' value='" & productUOM & "' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-PRICE[" & count.ToString & "]' name='NEW_ITEM-PRICE[" & count.ToString & _
                           "]' readonly='readonly' value='" & basketItem.Net_Price.ToString & "' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-CURRENCY[" & count.ToString & "]' name='NEW_ITEM-CURRENCY[" & count.ToString & _
                           "]' readonly='readonly' value='" & currencyCode & "' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-LONGTEXT_" & count.ToString & ":132[]' name='NEW_ITEM-LONGTEXT_" & count.ToString & _
                            ":132[]' readonly='readonly' value='' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-VENDORMAT[" & count.ToString & "]' name='NEW_ITEM-VENDORMAT[" & count.ToString & _
                           "]' readonly='readonly' value='" & basketItem.Product & "' />").Append("<br />")
                .Append("<input type='" & inputType & " ' id='NEW_ITEM-MATGROUP[" & count.ToString & "]' name='NEW_ITEM-MATGROUP[" & count.ToString & _
                          "]' readonly='readonly' value='" & productUNSPSC & "' />").Append("<br />")
            End With
            count += 1
        Next

        returnString = sb.ToString

        MarkBasketAsProcessed()
        Return returnString
    End Function
    Private Sub MarkBasketAsProcessed()

        Try
            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
            For i As Integer = 0 To i = 3
                Try
                    basket.Mark_As_Processed(Profile.Basket.Basket_Header_ID)
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        'Logging.WriteLog(Profile.UserName, "UCCCPL-020", ex.Message, "Failed to set basket as processed!", "", "", ProfileHelper.GetPageName)
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub
End Class



