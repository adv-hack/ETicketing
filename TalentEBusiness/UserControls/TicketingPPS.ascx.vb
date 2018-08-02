Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class UserControls_TicketingPPS
    Inherits ControlBase

    Dim product As New TalentProduct
    Dim settings As New DESettings
    Dim err As New ErrorObj
    Public ucr As New UserControlResource
    Public StandLabel As String = String.Empty
    Public AreaLabel As String = String.Empty
    Public RowLabel As String = String.Empty
    Public SeatLabel As String = String.Empty
    Public UpdatePaymentsButtonText As String = String.Empty
    Public CancelPPSEnrollmentButtonText As String = String.Empty
    Private _showAmendPPSFFHeader As Boolean = True
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _amendPPSEnrolmentInUse As Boolean = False
  
    Public ReadOnly Property SeasonTicketsList() As CheckBoxList
        Get
            Return Me.PPSList
        End Get
    End Property
    Public ReadOnly Property NoSchemesRequired() As CheckBox
        Get
            Return Me.noSchemes
        End Get
    End Property
    Public Property Errors() As BulletedList
        Get
            Return Me.ErrorList
        End Get
        Set(ByVal value As BulletedList)
            Me.ErrorList = value
        End Set
    End Property

    Private _Enabled As Boolean
    Public Property Enabled() As Boolean
        Get
            Return _Enabled
        End Get
        Set(ByVal value As Boolean)
            _Enabled = value
        End Set
    End Property
    Private _productCode As String
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property
    Private _priceCode As String
    Public Property PriceCode() As String
        Get
            Return _priceCode
        End Get
        Set(ByVal value As String)
            _priceCode = value
        End Set
    End Property
    Public Property AmendPPSEnrolmentInUse() As Boolean
        Get
            Return _amendPPSEnrolmentInUse
        End Get
        Set(ByVal value As Boolean)
            _amendPPSEnrolmentInUse = value
        End Set
    End Property
    Public ReadOnly Property HiddenProductCode() As HiddenField
        Get
            Return hfProductCode
        End Get
    End Property
    Public Property AmendPPSDetails As Repeater
        Get
            Return Me.rptAmendPPSDetails
        End Get
        Set(ByVal value As Repeater)
            Me.rptAmendPPSDetails = value
        End Set
    End Property
    Public Property SchemeLockedMessageplh As PlaceHolder
        Get
            Return Me.plhSchemeLockedMessage
        End Get
        Set(ByVal value As PlaceHolder)
            Me.plhSchemeLockedMessage = value
        End Set
    End Property
    Public Sub setHiddenFields()
        ' Set the hidden fields
        hfProductCode.Value = ProductCode
        hfPriceCode.Value = PriceCode
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TicketingPPS.ascx"
        End With
        UpdatePaymentsButtonText = ucr.Content("UpdatePaymentsButtonText", _languageCode, True)
        CancelPPSEnrollmentButtonText = ucr.Content("CancelPPSEnrollmentButtonText", _languageCode, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Enabled Then
            Dim myLibrary As String = ucr.Attribute("websalesVLDLLibrary")
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Enabled Then
            Dim myDefs As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            'Decide whether the no scheme checkbox is enabled
            If myDefs.PPS_MUST_TICK_FOR_NO_SCHEMES Then
                noSchemes.Visible = True
                noSchemes.Text = ucr.Content("noTicketsText", _languageCode, True)
            Else
                noSchemes.Visible = False
                noSchemes.Enabled = False
            End If
        End If

        If AmendPPSEnrolmentInUse Then
            plhPPSErrors.Visible = False
            plhNoSchemes.Visible = False
            plhPPSList.Visible = False
            plhAmendPPSDetails.Visible = True
            StandLabel = ucr.Content("StandLabelText", _languageCode, True)
            AreaLabel = ucr.Content("AreaLabelText", _languageCode, True)
            RowLabel = ucr.Content("RowLabelText", _languageCode, True)
            SeatLabel = ucr.Content("SeatLabelText", _languageCode, True)
            SchemeLockedMessage.Text = ucr.Content("SchemeLockedMessage", _languageCode, True)
        Else
            plhSchemeLockedMessage.Visible = False
        End If

        plhPPSErrors.Visible = (ErrorList.Items.Count > 0)
        plhPPSList.Visible = (PPSList.Items.Count > 0)
    End Sub

    Protected Sub rptAmendPPSDetails_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptAmendPPSDetails.ItemCommand
        If e.CommandName = "Update" Then
            Dim customerNumber As String = CType(e.Item.FindControl("hdfAmendPPSCustomerNumber"), HiddenField).Value
            Dim seatDetails As String = CType(e.Item.FindControl("hdfAmendPPSSeatDetails"), HiddenField).Value
            Dim redirectUrl As New StringBuilder
            redirectUrl.Append("~/PagesPublic/ProductBrowse/AmendPPSPayments.aspx?customer=")
            redirectUrl.Append(customerNumber)
            redirectUrl.Append("&seat=")
            redirectUrl.Append(seatDetails)
            redirectUrl.Append("&product=")
            redirectUrl.Append(HiddenProductCode.Value)
            Response.Redirect(redirectUrl.ToString)
        End If
        If e.CommandName = "CancelPPSEnrollment" Then
            Dim dePPS As New DEPPS
            Dim talentPPS As New TalentPPS
            dePPS.CustomerNumber = CType(e.Item.FindControl("hdfAmendPPSCustomerNumber"), HiddenField).Value
            Dim seatDetails As String = CType(e.Item.FindControl("hdfAmendPPSSeatDetails"), HiddenField).Value
            dePPS.ProductCode = ProductCode
            dePPS.SeatDetails = seatDetails
            dePPS.UpdateMode = False
            dePPS.RetrieveMode = False
            dePPS.CancelEnrolMode = True
            talentPPS.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            talentPPS.DEPPS = dePPS
            err = talentPPS.CancelPPSEnrollment
            If err.HasError Then
            Else
                If talentPPS.ResultDataSet.Tables("StatusResults") IsNot Nothing AndAlso talentPPS.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
                    Dim errorcode As String = talentPPS.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode").ToString.Trim
                    Dim _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ucr.FrontEndConnectionString)
                    Dim errorMessage As String = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                                    Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                   errorcode).ERROR_MESSAGE
                    Me.ErrorList.Items.Add(errorMessage)
                Else
                    Response.Redirect(Request.Url.ToString())
                End If
            End If
        End If
    End Sub

    Public Function SetAmendPPSEnrolEnabled(enrolled As Boolean) As Boolean
        If enrolled Then
            Return False
        Else
            Return True
        End If
    End Function

    Protected Sub rptAmendPPSDetails_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptAmendPPSDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim reservationItem As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim a As String = String.Empty
            Dim di As DataRow = CType(e.Item.DataItem, DataRowView).Row
            Dim ri As RepeaterItem = e.Item
            Dim plhAmendPPSUpdate As PlaceHolder = CType(ri.FindControl("plhAmendPPSUpdate"), PlaceHolder)
            Dim plhCancelPPSEnrollment As PlaceHolder = CType(ri.FindControl("plhCancelPPSEnrollment"), PlaceHolder)
            Dim chkAmendPPSEnrol As CheckBox = CType(ri.FindControl("chkAmendPPSEnrol"), CheckBox)
            Dim plhButtonGroup As PlaceHolder = CType(ri.FindControl("plhButtonGroup"), PlaceHolder)
            Dim customerEnrolled As Boolean = True
            plhAmendPPSUpdate.Visible = True
            plhCancelPPSEnrollment.Visible = True
            plhButtonGroup.Visible = False

            If CheckForDBNull_Boolean_DefaultFalse(di("schemeLocked")) Then
                chkAmendPPSEnrol.Enabled = False
            End If
            If CheckForDBNull_Boolean_DefaultFalse(di("schemeLocked")) Or Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(di("enrolled")) Then
                plhAmendPPSUpdate.Visible = False
                customerEnrolled = False
            Else
                plhButtonGroup.Visible = True
            End If
            If CheckForDBNull_Boolean_DefaultFalse(di("schemeLocked")) Or Not CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("CancelPPSEnrollmentAllowed")) Or Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(di("enrolled")) Then
                plhCancelPPSEnrollment.Visible = False
                customerEnrolled = False
            Else
                plhButtonGroup.Visible = True
            End If
        End If
    End Sub
    
End Class
