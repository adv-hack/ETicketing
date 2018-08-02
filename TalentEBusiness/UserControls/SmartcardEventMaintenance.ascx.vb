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
Partial Class UserControls_SmartcardEventMaintenance
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
            .KeyCode = "SmartcardEventMaintenance.ascx"
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
                .InactiveProducts = False
                .CardProducts = False
                .EventProducts = True
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
        Catch ex As Exception
            showError("99")
            InitializeScrnFields()
        End Try
    End Sub

    Protected Sub InitializeScrnFields()
        errorlist.Items.Clear()

        Dim profile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)

        CustomerLabel.Text = GetText("CustomerLabel") & " " & profile.User.Details.LoginID.PadLeft(12, "0") & "   " & profile.User.Details.Title.Trim() & " " & profile.User.Details.Forename.Trim() & " " & profile.User.Details.Surname.Trim()

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

            '   Dim checkBox As CheckBox = CType(e.Item.FindControl("saleSelectionCBox"), CheckBox)
            Dim productLabel As Label = CType(e.Item.FindControl("saleProductLabel"), Label)
            Dim soldDateLabel As Label = CType(e.Item.FindControl("saleSoldDateLabel"), Label)
            Dim soldByLabel As Label = CType(e.Item.FindControl("saleSoldByLabel"), Label)

            Dim statusLabel As Label = CType(e.Item.FindControl("saleStatusLabel"), Label)
            Dim saleUploadButton As Button = CType(e.Item.FindControl("saleUploadButton"), Button)
            Dim salePrintTicketButton As Button = CType(e.Item.FindControl("salePrintTicketButton"), Button)

            'Dim cancDateLabel As Label = CType(e.Item.FindControl("saleCancDateLabel"), Label)
            'Dim cancByLabel As Label = CType(e.Item.FindControl("saleCancByLabel"), Label)
            Dim hfSaleRecId As HiddenField = CType(e.Item.FindControl("hfSaleRecID"), HiddenField)
            Dim hfSCRecordId As HiddenField = CType(e.Item.FindControl("hfSCRecordId"), HiddenField)

            Dim dr As DataRow = e.Item.DataItem.Row

            saleUploadButton.Text = ucr.Content("UploadButtonText", _languageCode, True)
            salePrintTicketButton.Text = ucr.Content("PrintTicketButtonText", _languageCode, True)

            productLabel.Text = dr("ProductCode")
            soldDateLabel.Text = dr("SoldDate")
            soldByLabel.Text = dr("SoldBy")
            'cancDateLabel.Text = dr("CancelledDate")
            'cancByLabel.Text = dr("CancelledBy")
            hfSaleRecId.Value = dr("SaleRecordID")


            Select Case Talent.eCommerce.Utilities.CheckForDBNull_String(dr("SCUploadType"))
                Case "0"
                    statusLabel.Text = ucr.Content("Status0", _languageCode, True)
                Case "2"
                    statusLabel.Text = ucr.Content("Status2", _languageCode, True)
                Case "3"
                    statusLabel.Text = ucr.Content("Status3", _languageCode, True)
                Case "8"
                    statusLabel.Text = ucr.Content("Status8", _languageCode, True)
                Case Else
                    statusLabel.Text = ucr.Content("StatusOther", _languageCode, True)
            End Select

            saleUploadButton.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dr("SCUploadEnabled"))
            salePrintTicketButton.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dr("SCPrintEnabled"))

            hfSCRecordId.Value = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("SCRecordID"))

            ' Products can only be selected to produce cards when the sale records are
            ' active and the 'Produce card' button is enabled.
            Dim rowStyle As Panel = CType(e.Item.FindControl("rowStyle"), Panel)
            rowStyle.CssClass = "rowActive"
            'checkBox.Checked = False
            'checkBox.Visible = True

            If dr("ActiveSale").ToString.Trim <> "A" Then
                'checkBox.Visible = False
                rowStyle.CssClass = "rowInactive"
            End If
        End If
    End Sub

    Protected Sub SaleRepeater_ItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
        Dim productCode As String = CType(e.Item.FindControl("saleProductLabel"), Label).Text
        Dim cardNo As String = ""
        Dim cardRecordID As String = CType(e.Item.FindControl("hfSCRecordId"), HiddenField).Value
        Dim saleRecordID As String = CType(e.Item.FindControl("hfSaleRecID"), HiddenField).Value
        Dim mediaType As String = String.Empty
        If e.CommandArgument = "Print" Then
            mediaType = "8"
        Else
            mediaType = "0"
        End If

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
            .CardNumber = hdnActiveCard.Text
            .CardRecordID = cardRecordID
            .SaleRecordID = saleRecordID
            .RequestType = "S"
            ' .Printer = printer
            '.RequestType = requestType
            .MediaType = mediaType
            .AgentFilter = True
            .ActiveProducts = True
            .EventProducts = True
            .CardProducts = False
            '.CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
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

    Protected Sub CardRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CardRepeater.ItemDataBound

        If e.Item.ItemIndex <> -1 Then


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
          

            If dr("ActiveCard").ToString.Trim <> "A" Then

                rowStyle.CssClass = "rowInactive"
            Else
                hdnActiveCard.Text = dr("CardNumber").ToString.Trim
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

    'Protected Sub SmartcardFunction(ByVal requestType As String, ByVal productCode As String, ByVal cardNumber As String, ByVal printer As String, ByVal cardRecordID As String, ByVal saleRecordID As String)

    '    Dim smartcard As New Talent.Common.TalentSmartcard
    '    Dim settings As New Talent.Common.DESettings
    '    Dim err As New Talent.Common.ErrorObj
    '    Dim moduleDefaults As New ECommerceModuleDefaults
    '    Dim def As ECommerceModuleDefaults.DefaultValues
    '    def = moduleDefaults.GetDefaults()

    '    settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
    '    settings.BusinessUnit = TalentCache.GetBusinessUnit()
    '    settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
    '    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)

    '    smartcard.Settings() = settings
    '    smartcard.Settings.OriginatingSource = Convert.ToString(Session.Item("Agent")).ToUpper

    '    Dim deSmartcard As New Talent.Common.DESmartcard

    '    With deSmartcard
    '        .ProductCode = productCode
    '        .CardNumber = cardNumber
    '        .CardRecordID = cardRecordID
    '        .Printer = printer
    '        .RequestType = requestType
    '        .SaleRecordID = saleRecordID
    '        .CustomerNumber = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID.PadLeft(12, "0")
    '        .Src = "W"
    '    End With

    '    smartcard.DE = deSmartcard
    '    err = smartcard.CallSmartcardFunction
    '    ds1 = smartcard.ResultDataSet()
    '    '
    '    ' Process if no connection issues to back end system
    '    '
    '    If Not err.HasError Then

    '        '
    '        ' Display any error or infomation messages returned from the call.
    '        '
    '        If ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString().Trim <> "" Then
    '            showError(_cSmartCardErrorPfx & ds1.Tables(_cStatusTable).Rows(0).Item("ReturnCode").ToString().Trim)
    '        End If

    '        '
    '        ' Reload the screen which will display any changes in data.
    '        '
    '        showData()
    '    Else
    '        showError("99")
    '    End If

    'End Sub

End Class
