
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Data.SqlClient
Imports System.IO
Imports System.Data
Imports System.Collections.Generic

Partial Class Redirect_Launcher
    Inherits Base01

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case LCase(Request.QueryString("function"))
            Case Is = "autologin"
                DoAutoLoginRedirect()
            Case Is = "basket"
                DoAddToBasket()
            Case Is = "removetransaction"
                DoRemoveTransaction()
            Case Else
                ReplaceQueryStrings()
        End Select

    End Sub

    Private Function trapSQLInjection(ByVal checkStr As String) As String
        Dim rtn As String = String.Empty
        If checkStr <> "" Then rtn = checkStr.Replace("select", "").Replace("drop", "").Replace("union", "").Replace("join", "").Replace("delete", "").Replace("from", "").Replace("'", "").Replace(";", "")
        Return rtn
    End Function

    Protected Sub DoRemoveTransaction()
        Dim transactionId As String = trapSQLInjection(Request.QueryString("transactionId"))
        Dim tbl As DataTable
        Dim productToDecrement As String = ""
        Dim transRowsToRemove As Collection = New Collection
        If Not Session("personalisationTransactions") Is Nothing Then
            tbl = Session("personalisationTransactions")
            For Each row As DataRow In tbl.Rows
                If row.Item("TRANSACTION_ID") = transactionId Then
                    transRowsToRemove.Add(row)
                    productToDecrement = row.Item("PRODUCT_CODE")
                End If
            Next

            For Each row As DataRow In transRowsToRemove
                Dim qtyToRem As Integer = 0
                If row.Item("QTY_RULE").ToString.ToLower = "n" Then
                    For Each ltr As String In row.Item("COMPONENT_VALUE").ToString
                        If ltr <> " " Then
                            qtyToRem += 1
                        End If
                    Next
                Else
                    qtyToRem = 1
                End If
                decrementProduct(row.Item("LINKED_PRODUCT_CODE"), qtyToRem)
            Next

            For Each row As DataRow In transRowsToRemove
                tbl.Rows.Remove(row)
            Next
            decrementProduct(productToDecrement)
            Session("personalisationTransactions") = tbl
        End If
        Response.Redirect("..\PagesPublic\Basket\Basket.aspx")
    End Sub
    Protected Sub decrementProduct(ByVal productCode As String, Optional ByVal qtyToRemove As Integer = 1)
        Dim tbisToRemove As ArrayList = New ArrayList
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If tbi.Product = productCode Then
                If tbi.Quantity > qtyToRemove Then
                    tbi.Quantity = tbi.Quantity - qtyToRemove
                    Profile.Basket.IsDirty = True
                Else
                    tbisToRemove.Add(tbi)
                End If
            End If
        Next
        For Each tbiToRemove As TalentBasketItem In tbisToRemove
            Profile.Basket.BasketItems.Remove(tbiToRemove)
            Profile.Basket.IsDirty = True
            Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            basketAdapter.Delete_Basket_Item(Profile.Basket.Basket_Header_ID, tbiToRemove.Product)
        Next
        Profile.Save()
    End Sub
    Protected Sub DoAddToBasket()
        Dim redirectUrl As String = trapSQLInjection(Request.QueryString("redirectUrl"))
        Dim product As String = trapSQLInjection(Request.QueryString("product"))
        Dim master As String = trapSQLInjection(Request.QueryString("master"))
        Dim transactionId As String = trapSQLInjection(Request.QueryString("transactionId"))
        Dim components As String = trapSQLInjection(Request.QueryString("components"))
        Dim displayInBasket As String = trapSQLInjection(Request.QueryString("displayInBasket"))
        Dim overrideXML As String = "<config>"

        If overrideXML <> "<config>" Then
            Session("overrideXML") = overrideXML & "</config>"
        Else
            Session("overrideXML") = ""
        End If

        Dim tbl As DataTable
        If Session("personalisationTransactions") Is Nothing Then
            tbl = New DataTable
            With tbl.Columns
                .Add(New DataColumn("TRANSACTION_ID"))
                .Add(New DataColumn("PRODUCT_CODE"))
                .Add(New DataColumn("COMPONENT_ID"))
                .Add(New DataColumn("COMPONENT_NAME"))
                .Add(New DataColumn("COMPONENT_VALUE"))
                .Add(New DataColumn("LINKED_PRODUCT_CODE"))
                .Add(New DataColumn("QTY_RULE"))
                .Add(New DataColumn("DISPLAY_IN_BASKET"))
            End With
        Else
            tbl = Session("personalisationTransactions")
        End If

        'Removing items which are in personalisation transaction from profile basket
        Dim personalisationRowsToRemove As Collection = New Collection
        If tbl.Rows.Count > 0 Then
            Dim alTalentBasketItemsToRemove As New List(Of TalentBasketItem)
            Dim personalisationProducts As New List(Of String)
            Dim found As Boolean = False
            Dim i As Integer = 0
            For Each personalisationRow As DataRow In tbl.Rows
                'products
                found = False
                i = 0
                While i < personalisationProducts.Count
                    If personalisationProducts(i) = personalisationRow("PRODUCT_CODE").ToString Then
                        found = True
                    End If
                    i += 1
                End While
                If Not found Then
                    personalisationProducts.Add(personalisationRow("PRODUCT_CODE").ToString)
                End If
                'subproducts
                found = False
                i = 0
                While i < personalisationProducts.Count
                    If personalisationProducts(i) = personalisationRow("LINKED_PRODUCT_CODE").ToString Then
                        found = True
                    End If
                    i += 1
                End While
                If Not found Then
                    personalisationProducts.Add(personalisationRow("LINKED_PRODUCT_CODE").ToString)
                End If
                'transaction id is not empty so remove the rows from tbl
                If transactionId <> "" Then
                    If personalisationRow("TRANSACTION_ID") = transactionId Then
                        personalisationRowsToRemove.Add(personalisationRow)
                    End If
                End If
            Next
            'remove from profile basket as well as from basket table
            i = 0
            While i < personalisationProducts.Count
                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                    If tbi.Product = personalisationProducts(i) Then
                        alTalentBasketItemsToRemove.Add(tbi)
                    End If
                Next
                i += 1
            End While
            Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
            For Each tbiToRemove As TalentBasketItem In alTalentBasketItemsToRemove
                basketDetails.Delete_Basket_Item(Profile.Basket.Basket_Header_ID, tbiToRemove.Product)
                Profile.Basket.BasketItems.Remove(tbiToRemove)
            Next
            basketDetails = Nothing
            Profile.Save()
        End If

        If transactionId <> "" Then
            'remove from tbl only the specific transaction
            If personalisationRowsToRemove.Count > 0 Then
                For Each rowToRemove As DataRow In personalisationRowsToRemove
                    tbl.Rows.Remove(rowToRemove)
                Next
            End If
        Else
            transactionId = System.Guid.NewGuid().ToString()
        End If

        'Recreate the rows with new values
        Dim charSeparators() As Char = {"~"c}
        Dim comp_names As Array = components.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)
        If comp_names.Length > 0 Then
            For Each comp As String In comp_names
                Dim newRow As DataRow = tbl.NewRow()
                newRow.Item("TRANSACTION_ID") = transactionId
                newRow.Item("PRODUCT_CODE") = product
                newRow.Item("COMPONENT_ID") = comp.Split("|")(0)
                newRow.Item("COMPONENT_NAME") = comp.Split("|")(1)
                newRow.Item("COMPONENT_VALUE") = comp.Split("|")(2)
                newRow.Item("LINKED_PRODUCT_CODE") = comp.Split("|")(3)
                newRow.Item("QTY_RULE") = comp.Split("|")(4)
                newRow.Item("DISPLAY_IN_BASKET") = displayInBasket
                tbl.Rows.Add(newRow)
            Next
        Else
            'Just add the Product as the Components must be blank
            Dim newRow As DataRow = tbl.NewRow()
            newRow.Item("TRANSACTION_ID") = transactionId
            newRow.Item("PRODUCT_CODE") = product
            newRow.Item("COMPONENT_ID") = ""
            newRow.Item("COMPONENT_NAME") = ""
            newRow.Item("COMPONENT_VALUE") = ""
            newRow.Item("LINKED_PRODUCT_CODE") = ""
            newRow.Item("QTY_RULE") = ""
            newRow.Item("DISPLAY_IN_BASKET") = ""
            tbl.Rows.Add(newRow)
        End If
        Session("personalisationTransactions") = tbl

        'Load distinct linked product codes into array
        Dim subProducts As ArrayList = New ArrayList
        For Each row As DataRow In tbl.Rows
            Dim found As Boolean = False
            Dim i As Integer = 0
            While i < subProducts.Count
                If subProducts.Item(i).ToString = row.Item("LINKED_PRODUCT_CODE").ToString Then
                    found = True
                End If
                i += 1
            End While
            If Not found Then
                subProducts.Add(row.Item("LINKED_PRODUCT_CODE"))
            End If
        Next

        Dim aUniqueTrans As ArrayList = New ArrayList
        For Each row As Data.DataRow In tbl.Rows
            Dim sTrans As String = row.Item("TRANSACTION_ID")
            If aUniqueTrans.IndexOf(sTrans) = -1 Then
                aUniqueTrans.Add(sTrans)
            End If
        Next

        Dim aProd As ArrayList = New ArrayList
        Dim aCount As ArrayList = New ArrayList
        For Each sUniqueTrans As String In aUniqueTrans
            For Each row As Data.DataRow In tbl.Rows
                Dim sTrans As String = row.Item("TRANSACTION_ID")
                Dim sProd As String = row.Item("PRODUCT_CODE")
                If sTrans = sUniqueTrans Then
                    Dim idx As Integer = aProd.IndexOf(sProd)
                    If idx = -1 Then
                        aProd.Add(sProd)
                        aCount.Add(1)
                    Else
                        Dim oldCount As Integer = aCount(idx)
                        aCount(idx) = oldCount + 1
                    End If
                    Exit For
                End If
            Next
        Next

        Dim iii As Integer = 0
        While iii < aProd.Count
            addToBasket(aProd(iii), master, "1", aCount(iii))
            iii += 1
        End While

        'Add component products to the basket
        For Each sProd As String In subProducts
            Dim times As Integer = 0
            For Each tblRow As DataRow In tbl.Rows
                If tblRow.Item("LINKED_PRODUCT_CODE") = sProd Then
                    If tblRow.Item("QTY_RULE").ToString.ToLower = "n" Then
                        For Each ltr As String In tblRow.Item("COMPONENT_VALUE").ToString
                            If ltr <> " " Then
                                times += 1
                            End If
                        Next
                    Else
                        times += 1
                    End If
                End If
            Next
            addToBasket(sProd, master, "2", times)
            Profile.Save()
        Next
        Response.Redirect("..\PagesPublic\Basket\Basket.aspx")
    End Sub

    Private Sub addToBasket(ByVal product As String, ByVal master As String, ByVal xmlConfig As String, ByVal qty As Integer)
        Dim moduleDefaults As ECommerceModuleDefaults
        moduleDefaults = New ECommerceModuleDefaults

        Dim def As ECommerceModuleDefaults.DefaultValues
        def = moduleDefaults.GetDefaults

        If product <> "" Then
            Dim tbi1 As New TalentBasketItem
            With tbi1
                .Product = product
                If Not master Is Nothing Then
                    Dim tDataObjects As Talent.Common.TalentDataObjects
                    tDataObjects = New Talent.Common.TalentDataObjects
                    tDataObjects.Settings = GetSettingsObject()
                    .MASTER_PRODUCT = tDataObjects.ProductsSettings.TblProductOptions.GetMasterProductCodeByOptionCode(TalentCache.GetBusinessUnit(), product)
                End If
                .Xml_Config = xmlConfig
                Dim products As Data.DataTable = Utilities.GetProductInfo(product)

                If products IsNot Nothing Then
                    If products.Rows.Count > 0 Then
                        .ALTERNATE_SKU = Utilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                        .PRODUCT_DESCRIPTION1 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                        .PRODUCT_DESCRIPTION2 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                        .PRODUCT_DESCRIPTION3 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                        .PRODUCT_DESCRIPTION4 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                        .PRODUCT_DESCRIPTION5 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                    End If
                End If
                .STOCK_ERROR_CODE = ""
                .Quantity = qty
                .Account_Code = ""
                .Cost_Centre = ""
                Select Case def.PricingType
                    Case 2
                        Dim prices As Data.DataTable = Talent.eCommerce.Utilities.GetChorusPrice(.Product, .Quantity)
                        If prices.Rows.Count > 0 Then
                            .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                            .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                            .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                        End If
                    Case Else
                        Dim deWp As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(.Product, .Quantity, .MASTER_PRODUCT)
                        .Gross_Price = deWp.Purchase_Price_Gross
                        .Net_Price = deWp.Purchase_Price_Net
                        .Tax_Price = deWp.Purchase_Price_Tax
                End Select
                .Cost_Centre = ""
                .Account_Code = ""
                If Not Profile.IsAnonymous Then
                    If Not Profile.PartnerInfo.Details.COST_CENTRE Is Nothing Then
                        .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                    End If
                    If Not Order.GetLastAccountNo(Profile.User.Details.LoginID) Is Nothing Then
                        .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                    End If
                End If
                Try
                    .GROUP_LEVEL_01 = Request.QueryString("group1")
                    .GROUP_LEVEL_02 = Request.QueryString("group2")
                    .GROUP_LEVEL_03 = Request.QueryString("group3")
                    .GROUP_LEVEL_04 = Request.QueryString("group4")
                    .GROUP_LEVEL_05 = Request.QueryString("group5")
                    .GROUP_LEVEL_06 = Request.QueryString("group6")
                    .GROUP_LEVEL_07 = Request.QueryString("group7")
                    .GROUP_LEVEL_08 = Request.QueryString("group8")
                    .GROUP_LEVEL_09 = Request.QueryString("group9")
                    .GROUP_LEVEL_10 = Request.QueryString("group10")
                Catch ex As Exception
                End Try
            End With
            If xmlConfig <> "" Then
                Profile.Basket.AddItem(tbi1, True)
            Else
                Profile.Basket.AddItem(tbi1)
            End If

        End If
    End Sub

    Protected Sub DoAutoLoginRedirect()
        Dim encType, usr, pass, redirectUrl As String
        encType = Request.QueryString("encryptionType")
        usr = Request.QueryString("userID")
        pass = Request.QueryString("pw")
        redirectUrl = Request.QueryString("redirectUrl")

        Select Case encType
            Case Is = "1"
                Dim key As String = ConfigurationManager.AppSettings("PrivateEncryptionKey").ToString
                usr = Talent.Common.Utilities.TripleDESDecode(usr, key)
                pass = Talent.Common.Utilities.TripleDESDecode(pass, key)
            Case Is = "2"
            Case Is = "3"
            Case Else
        End Select

        Talent.eCommerce.Utilities.loginUser(usr, pass)

        Response.Redirect(redirectUrl)
    End Sub

    Protected Sub ReplaceQueryStrings()
        Dim url As String = Request.QueryString("url")
        Dim urlType As String = Request.QueryString("urlType")
        Const prefix As String = "item"
        Dim listEnd As Boolean = False
        Dim count As Integer = 0
        Dim itemName As String = String.Empty
        Dim itemsList As New ArrayList

        'Create an arraylist of each of the items that have 
        'been passed via the QueryString
        While Not listEnd
            count += 1
            itemName = prefix & count.ToString
            If Not String.IsNullOrEmpty(Request(itemName)) Then
                itemsList.Add(Request(itemName))
            Else
                listEnd = True
            End If
        End While


        Select Case UCase(urlType)
            Case Is = "INTERNAL"
                If url.StartsWith("~") Then url.TrimStart("~")
                If url.StartsWith(".") Then url.TrimStart(".")
                If url.StartsWith("/") Then url.TrimStart("/")
                url = "~/" & BuildURL(itemsList, url)
                Response.Redirect(url)

            Case Is = "EXTERNAL"
                url = BuildURL(itemsList, url)
                Response.Redirect(url)

            Case Is = String.Empty
                url = BuildURL(itemsList, url)
                If Not url Is Nothing And url <> "" Then
                    Response.Redirect(url)
                End If
        End Select
    End Sub


    Protected Function BuildURL(ByVal itemsList As ArrayList, ByVal url As String) As String
        'If the QueryString contained items then 
        'populent where relevant
        If itemsList.Count > 0 Then

            'Add the QueryString marker
            url += "?"

            'Loop through the items
            For index As Integer = 0 To itemsList.Count - 1

                If index > 0 Then url += "&" 'If not the 1st item add the & symbol

                Select Case UCase(itemsList(index))
                    Case Is = "BU" 'Add the Business Unit
                        url += "bu=" & TalentCache.GetBusinessUnit

                    Case Is = "PARTNER" 'Add the Partner
                        url += "partner=" & TalentCache.GetPartner(Profile)

                    Case Is = "LOGINID" ' 'Add the UserName
                        url += "userID=" & Profile.UserName

                    Case Else
                        'Handle all other cases
                End Select
            Next

            'If we have added an extra & to the end, remove it
            If url.EndsWith("&") Then url = url.TrimEnd("&")

        End If

        Return url

    End Function
End Class
