Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports System.Xml
Imports System.Globalization
Imports System.Threading

'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Smartcard Maintenance
'
'       Date                        July '08
'
'       Author                      Craig Mcloughlin
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_SmartcardCardMaintenance
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public ucr As New Talent.Common.UserControlResource
    Public errMsg As Talent.Common.TalentErrorMessages
    Dim ds1 As New DataSet()

    Const _cStatusTable = 0
    Const _cHeaderTable = 1
    Const _cSalesTable = 2
    Const _cCardTable = 3
    Const _cErrLogTable = 4
    Const _cSaleFunc = "S"
    Const _cReprintFunc = "R"
    Const _cCancFunc = "C"
    Const _cSmartCardErrorPfx = "SmartcardCardErr"
    Const _cPgmCallErr = "99"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SmartcardCardMaintenance.ascx"
        End With
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        ' Initialize Page fields
        InitializeScrnFields()

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Visible Then
            Try
                If Not Page.IsPostBack Then
                    showData()
                End If
            Catch ex As Exception
            End Try

        End If
    End Sub

    Protected Sub showData()
        Try
            Dim smartcard As New Talent.Common.TalentSmartcard
            Dim settings As New Talent.Common.DESettings
            Dim err As New Talent.Common.ErrorObj
            Dim moduleDefaults As New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues
            def = moduleDefaults.GetDefaults()

            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)

            smartcard.Settings() = settings
            smartcard.Settings.OriginatingSource = Convert.ToString(Session.Item("Agent")).ToUpper

            Dim deSmartcard As New Talent.Common.DESmartcard

            With deSmartcard
                .CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
                .Src = "W"
                .ActiveProducts = True
                .InactiveProducts = True
                .CardProducts = True
            End With

            smartcard.DE = deSmartcard
            err = smartcard.RetrieveSmartcardCardDetails
            ds1 = smartcard.ResultDataSet()

            '
            ' Process if no connection issues to back end system
            '
            If Not err.HasError Then

                '
                ' Display any error or infomation messages returned from the call.
                '
                If ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString().Trim <> "" Then
                    showError(_cSmartCardErrorPfx & ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString().Trim)
                End If

                '
                ' Populate screen data if the call was successful
                '
                If ds1.Tables(_cStatusTable).Rows(0).Item("ErrorOccurred").ToString <> GlobalConstants.ERRORFLAG Then

                    '
                    ' Retrieve Header infomation:
                    '    - Options available to the user
                    '    - Valid smartcard printers
                    '
                    Dim hdrDT As DataTable = ds1.Tables(_cHeaderTable)
                    Dim hdrDTrw As DataRow = hdrDT.Rows.Item(0)
                    PrinterListDD.ClearSelection()

                    'Populate the drop down list with printers
                    PrinterListDD.DataSource = hdrDTrw("Printers")
                    PrinterListDD.DataBind()

                    ProduceCardButton.Enabled = hdrDTrw("ProduceCardOptionAvailable")
                    ReprintCardButton.Enabled = hdrDTrw("ReprintCardOptionAvailable")
                    CancelCardButton.Enabled = hdrDTrw("CancelCardOptionAvailable")

                    If ProduceCardButton.Enabled = False And ReprintCardButton.Enabled = False Then
                        PrinterListDD.Items.Clear()
                        PrinterListDD.Enabled = False
                    End If

                    '
                    ' Populate the TALENT Sales table
                    '
                    SaleRepeater.DataSource = ds1.Tables(_cSalesTable)
                    SaleRepeater.DataBind()

                    '
                    ' Populate the Card table
                    '
                    CardRepeater.DataSource = ds1.Tables(_cCardTable)
                    CardRepeater.DataBind()

                    '
                    ' Populate the Error Log table
                    '
                    ErrorLogRepeater.DataSource = ds1.Tables(_cErrLogTable)
                    ErrorLogRepeater.DataBind()

                End If
            Else
                showError("99")
                InitializeScrnFields()
            End If
            '----------------------------------------------
            ' Check if printer dropdown should be protected
            '----------------------------------------------
            If Not ucr.Attribute("enablePrinterDropdown") Is Nothing Then
                If PrinterListDD.Enabled AndAlso Not CBool(ucr.Attribute("enablePrinterDropdown")) Then
                    PrinterListDD.Enabled = False
                End If
            End If
        Catch ex As Exception
            showError("99")
            InitializeScrnFields()
        End Try
    End Sub

    Protected Sub InitializeScrnFields()
        errorlist.Items.Clear()

        PrinterListDD.ClearSelection()
        ProduceCardButton.Enabled = False
        ReprintCardButton.Enabled = False
        CancelCardButton.Enabled = False
        Dim profile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)

        CustomerLabel.Text = GetText("CustomerLabel") & " " & profile.User.Details.LoginID.PadLeft(12, "0") & "   " & profile.User.Details.Title.Trim() & " " & profile.User.Details.Forename.Trim() & " " & profile.User.Details.Surname.Trim()

        PrinterLabel.Text = GetText("PrinterLabelText")
        ProduceCardButton.Text = GetText("ProduceCardBtnText")
        ReprintCardButton.Text = GetText("ReprintCardBtnText")
        CancelCardButton.Text = GetText("CancelCardBtnText")

        SCFunctionHeaderLabel.Text = GetText("SmartcardFunctionHdrLabel")
        SaleDataHeaderLabel.Text = GetText("SaleDataHdrLabel")
        CardDataHeaderLabel.Text = GetText("CardDataHdrLabel")
        ErrorLogHeaderLabel.Text = GetText("ErrorLogHdrLabel")

    End Sub

    Protected Function GetText(ByVal PValue As String) As String
        Dim str As String = ucr.Content(PValue, _languageCode, True)
        Return str
    End Function

    Protected Sub showError(ByVal errCode As String)

        Dim eli As ListItem
        eli = New ListItem(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                            errCode).ERROR_MESSAGE)

        If Not errorlist.Items.Contains(eli) Then
            errorlist.Items.Add(eli)
        End If

    End Sub

    Protected Sub SaleRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles SaleRepeater.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim checkBox As CheckBox = CType(e.Item.FindControl("saleSelectionCBox"), CheckBox)
            Dim productLabel As Label = CType(e.Item.FindControl("saleProductLabel"), Label)
            Dim soldDateLabel As Label = CType(e.Item.FindControl("saleSoldDateLabel"), Label)
            Dim soldByLabel As Label = CType(e.Item.FindControl("saleSoldByLabel"), Label)
            Dim cancDateLabel As Label = CType(e.Item.FindControl("saleCancDateLabel"), Label)
            Dim cancByLabel As Label = CType(e.Item.FindControl("saleCancByLabel"), Label)
            Dim hfSaleRecId As HiddenField = CType(e.Item.FindControl("hfSaleRecID"), HiddenField)

            Dim dr As DataRow = e.Item.DataItem.Row

            productLabel.Text = dr("ProductCode")
            soldDateLabel.Text = dr("SoldDate")
            soldByLabel.Text = dr("SoldBy")
            cancDateLabel.Text = dr("CancelledDate")
            cancByLabel.Text = dr("CancelledBy")
            hfSaleRecId.Value = dr("SaleRecordID")

            ' Products can only be selected to produce cards when the sale records are
            ' active and the 'Produce card' button is enabled.
            Dim rowStyle As Panel = CType(e.Item.FindControl("rowStyle"), Panel)
            rowStyle.CssClass = "rowActive"
            checkBox.Checked = False
            checkBox.Visible = True

            If ProduceCardButton.Enabled <> True Then
                checkBox.Visible = False
            End If
            If dr("ActiveSale").ToString.Trim <> "A" Then
                checkBox.Visible = False
                rowStyle.CssClass = "rowInactive"
            End If
        End If
    End Sub

    Protected Sub CardRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CardRepeater.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim checkBox As CheckBox = CType(e.Item.FindControl("cardSelectionCBox"), CheckBox)
            Dim productLabel As Label = CType(e.Item.FindControl("cardProductLabel"), Label)
            Dim cardNumLabel As Label = CType(e.Item.FindControl("cardNumberLabel"), Label)
            Dim statusLabel As Label = CType(e.Item.FindControl("cardStatusLabel"), Label)
            Dim dateActvLabel As Label = CType(e.Item.FindControl("cardDateActivatedLabel"), Label)
            Dim dateDeactvLabel As Label = CType(e.Item.FindControl("cardDateDeactivatedLabel"), Label)
            Dim hfCardRecId As HiddenField = CType(e.Item.FindControl("hfCardRecID"), HiddenField)

            Dim dr As DataRow = e.Item.DataItem.Row

            productLabel.Text = dr("ProductCode")
            cardNumLabel.Text = dr("CardNumber")
            statusLabel.Text = dr("ActiveCard")
            dateActvLabel.Text = dr("Card_ActivatedDate")
            dateDeactvLabel.Text = dr("Card_DeactivatedDate")
            hfCardRecId.Value = dr("CardRecordID")

            ' Products can only be selected to produce cards when the card records are
            ' active and the 'Reprint card' or 'Cancel Card' buttons are enabled.
            Dim rowStyle As Panel = CType(e.Item.FindControl("rowStyle"), Panel)
            rowStyle.CssClass = "rowActive"
            checkBox.Checked = False
            checkBox.Visible = True

            If ReprintCardButton.Enabled = False And CancelCardButton.Enabled = False Then
                checkBox.Visible = False
            End If
            If dr("ActiveCard").ToString.Trim <> "A" Then
                checkBox.Visible = False
                rowStyle.CssClass = "rowInactive"
            End If
        End If
    End Sub

    Protected Sub ErrorLogRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles ErrorLogRepeater.ItemDataBound

        If e.Item.ItemIndex <> -1 Then

            Dim dateLabel As Label = CType(e.Item.FindControl("errLogDateLabel"), Label)
            Dim productLabel As Label = CType(e.Item.FindControl("errLogProductLabel"), Label)
            Dim cardNumLabel As Label = CType(e.Item.FindControl("errLogCardNumLabel"), Label)
            Dim actionLabel As Label = CType(e.Item.FindControl("errLogActionLabel"), Label)
            Dim errNumLabel As Label = CType(e.Item.FindControl("errLogNumberLabel"), Label)
            Dim errDescLabel As Label = CType(e.Item.FindControl("errLogDescLabel"), Label)

            Dim dr As DataRow = e.Item.DataItem.Row

            dateLabel.Text = dr("ErrorDate")
            productLabel.Text = dr("ProductCode")
            cardNumLabel.Text = dr("CardNumber")

            Dim action As String = GetText("Action" & dr("RequestAction").ToString.Trim).Trim
            If action <> "" Then
                actionLabel.Text = action
            Else
                actionLabel.Text = dr("RequestAction").ToString.Trim
            End If

            errNumLabel.Text = dr("ErrorNumber")
            errDescLabel.Text = dr("ErrorDescription")

            Dim rowStyle As Panel = CType(e.Item.FindControl("rowStyle"), Panel)
            rowStyle.CssClass = "rowInactive"

        End If
    End Sub

    Protected Sub ProduceCardButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ProduceCardButton.Click

        errorlist.Items.Clear()
        Dim itemsChecked As Boolean = False

        For Each ri As RepeaterItem In SaleRepeater.Items

            Try
                Dim checkBox As CheckBox = CType(ri.FindControl("saleSelectionCBox"), CheckBox)
                Dim productLabel As Label = CType(ri.FindControl("saleProductLabel"), Label)
                Dim soldDateLabel As Label = CType(ri.FindControl("saleSoldDateLabel"), Label)
                Dim soldByLabel As Label = CType(ri.FindControl("saleSoldByLabel"), Label)
                Dim cancDateLabel As Label = CType(ri.FindControl("saleCancDateLabel"), Label)
                Dim cancByLabel As Label = CType(ri.FindControl("saleCancByLabel"), Label)
                Dim hfSaleRecId As HiddenField = CType(ri.FindControl("hfSaleRecID"), HiddenField)

                '
                ' If a box has been checked, perform a sale request for the sale record.
                '
                If checkBox.Checked Then
                    itemsChecked = True
                    SmartcardFunction(_cSaleFunc, productLabel.Text, "", PrinterListDD.SelectedItem.Text, "", hfSaleRecId.Value)
                    Exit For
                End If

            Catch ex As Exception
                showError("99")
                Exit For
            End Try
        Next

        '
        ' If no items in the list have been checked send error back to the user.
        '
        If Not itemsChecked Then
            showError(_cSmartCardErrorPfx & "NS")
        End If

    End Sub

    Protected Sub ReprintCardButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReprintCardButton.Click

        errorlist.Items.Clear()
        Dim itemsChecked As Boolean = False

        For Each ri As RepeaterItem In CardRepeater.Items

            Try

                Dim checkBox As CheckBox = CType(ri.FindControl("cardSelectionCBox"), CheckBox)
                Dim productLabel As Label = CType(ri.FindControl("cardProductLabel"), Label)
                Dim cardNumLabel As Label = CType(ri.FindControl("cardNumberLabel"), Label)
                Dim statusLabel As Label = CType(ri.FindControl("cardStatusLabel"), Label)
                Dim dateActvLabel As Label = CType(ri.FindControl("cardDateActivatedLabel"), Label)
                Dim dateDeactvLabel As Label = CType(ri.FindControl("cardDateDeactivatedLabel"), Label)
                Dim hfCardRecId As HiddenField = CType(ri.FindControl("hfCardRecID"), HiddenField)

                '
                ' If a box has been checked, perform a reprint request for the sale record.
                '
                If checkBox.Checked Then
                    itemsChecked = True
                    SmartcardFunction(_cReprintFunc, productLabel.Text, cardNumLabel.Text, PrinterListDD.SelectedItem.Text, hfCardRecId.Value, "")
                    Exit For
                End If

            Catch ex As Exception
                showError("99")
                Exit For
            End Try
        Next

        '
        ' If no items in the list have been checked send error back to the user.
        '
        If Not itemsChecked Then
            showError(_cSmartCardErrorPfx & "NR")
        End If

    End Sub

    Protected Sub CancelCardButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelCardButton.Click

        errorlist.Items.Clear()
        Dim itemsChecked As Boolean = False

        For Each ri As RepeaterItem In CardRepeater.Items

            Try


                Dim checkBox As CheckBox = CType(ri.FindControl("cardSelectionCBox"), CheckBox)
                Dim productLabel As Label = CType(ri.FindControl("cardProductLabel"), Label)
                Dim cardNumLabel As Label = CType(ri.FindControl("cardNumberLabel"), Label)
                Dim statusLabel As Label = CType(ri.FindControl("cardStatusLabel"), Label)
                Dim dateActvLabel As Label = CType(ri.FindControl("cardDateActivatedLabel"), Label)
                Dim dateDeactvLabel As Label = CType(ri.FindControl("cardDateDeactivatedLabel"), Label)
                Dim hfCardRecId As HiddenField = CType(ri.FindControl("hfCardRecID"), HiddenField)

                '
                ' If a box has been checked, perform a cancellation request for the sale record.
                '
                If checkBox.Checked Then
                    itemsChecked = True
                    SmartcardFunction(_cCancFunc, productLabel.Text, cardNumLabel.Text, "", hfCardRecId.Value, "")
                    Exit For
                End If

            Catch ex As Exception
                showError("99")
                Exit For
            End Try
        Next

        '
        ' If no items in the list have been checked send error back to the user.
        '
        If Not itemsChecked Then
            showError(_cSmartCardErrorPfx & "NC")
        End If

    End Sub

    Protected Sub SmartcardFunction(ByVal requestType As String, ByVal productCode As String, ByVal cardNumber As String, ByVal printer As String, ByVal cardRecordID As String, ByVal saleRecordID As String)

        Dim smartcard As New Talent.Common.TalentSmartcard
        Dim settings As New Talent.Common.DESettings
        Dim err As New Talent.Common.ErrorObj
        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues
        def = moduleDefaults.GetDefaults()

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)

        smartcard.Settings() = settings
        smartcard.Settings.OriginatingSource = Convert.ToString(Session.Item("Agent")).ToUpper

        Dim deSmartcard As New Talent.Common.DESmartcard

        With deSmartcard
            .ProductCode = productCode
            .CardNumber = cardNumber
            .CardRecordID = cardRecordID
            .Printer = printer
            .RequestType = requestType
            .SaleRecordID = saleRecordID
            .CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
            .Src = "W"
        End With

        smartcard.DE = deSmartcard
        err = smartcard.CallSmartcardFunction
        ds1 = smartcard.ResultDataSet()
        '
        ' Process if no connection issues to back end system
        '
        If Not err.HasError Then

            '
            ' Display any error or infomation messages returned from the call.
            '
            If ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString().Trim <> "" Then
                showError(_cSmartCardErrorPfx & ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString().Trim)
            End If

            '
            ' Reload the screen which will display any changes in data.
            '
            showData()
        Else
            showError("99")
        End If

    End Sub

End Class
