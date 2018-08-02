Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Collections.Generic
Imports System.Linq
Imports Talent.eCommerce
Imports System
Imports System.Configuration
Imports System.Web

Public Class Vouchers
    Inherits ClassBase01
    'Shared ucr As New Talent.Common.UserControlResource
    'Shared _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private Const IMAGEPATH As String = "_IMAGEPATH"
    Private Const TEXT As String = "_TEXT"
    Private Const LINK As String = "_LINK"
    Private Const LINKTEXT As String = "_LINKTEXT"
    Private Const LINKBUTTONTEXT As String = "_LINKBUTTONTEXT"
    Private Const ONACCOUNTACTIVE As String = "_ONACCOUNTACTIVE"
    Private Const VOUCHERDETAILSPAGE As String = "VoucherDetails.aspx"
    Private Const SQLDATABASE As String = "SQL2005"
    Private Const CONNECTIONSTRING As String = "TalentEBusinessDBConnectionString"
    Private Const EXCHANGETEXT As String = "_ONACCOUNTTEXT"
    Private Const EXCHANGEBUTTONTEXT As String = "_ONACCOUNTBUTTONTEXT"
    Private _wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage


    Public Function GetText(ByVal controlID As String) As String
        Return _wfr.Content(controlID, _languageCode, True)
    End Function

    Public Property BUSINESSUNIT As String
    Public Sub New()
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .KeyCode = "Vouchers.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
    End Sub

    Public Sub PrepareSettings(ByRef settings As Talent.Common.DESettings)
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings(CONNECTIONSTRING).ToString
        settings.DestinationDatabase = SQLDATABASE
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.Partner = TalentCache.GetPartner(HttpContext.Current.Profile)
    End Sub

    Public Function GetCompanyDetails() As IList(Of String)
        Dim CompanyList As New List(Of String)
        Dim dtCompany As New DataTable
        dtCompany = TDataObjects.PageSettings.TblVouchersExternal.GetVoucherCompaniesByBU(_wfr.BusinessUnit, True)
        For Each dr As DataRow In dtCompany.Rows
            CompanyList.Add(dr(0))
        Next
        Return CompanyList
    End Function

    Public Function GetVoucherScheme(ByVal CustomerNumber As String, ByVal CompanyName As String, ByVal canShowInActive As Boolean) As DataTable
        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet
        Dim VoucherIdList As New List(Of Integer)

        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = False
        voucher.DeVouch.ShowActiveAndInActiveRecords = canShowInActive
        voucher.DeVouch.CustomerNumber = CustomerNumber

        err = voucher.GetVoucherList()
        ds1 = voucher.ResultDataSet

        Dim VoucherDetailsTable As DataTable = ds1.Tables("VoucherList")
        Dim dtVoucherIds As New DataTable
        dtVoucherIds = TDataObjects.PageSettings.TblVouchersExternal.GetVoucherIDByBUAndCompany(BUSINESSUNIT, CompanyName, False)
        For Each row As DataRow In dtVoucherIds.Rows
            VoucherIdList.Add(row(0))
        Next

        Dim result = (From r In VoucherDetailsTable.AsEnumerable()
                        Where VoucherIdList.Contains(r.Field(Of Integer)("VoucherId"))
                        Select r)

        Dim dtVoucherSchemes As New DataTable("VoucherSchemeList")
        With dtVoucherSchemes.Columns
            .Add("VoucherId", GetType(Long))
            .Add("Description", GetType(String))
        End With
        Dim dr As DataRow = Nothing
        For rowIndex As Integer = 0 To result.Count - 1
            dr = dtVoucherSchemes.NewRow
            dr("VoucherId") = Utilities.CheckForDBNull_Long(result(rowIndex)("VoucherId"))
            dr("Description") = Utilities.CheckForDBNull_String(result(rowIndex)("Description"))
            dtVoucherSchemes.Rows.Add(dr)
            dr = Nothing
        Next

        Return dtVoucherSchemes

    End Function

    Public Function GetVoucherPrice(ByVal CustomerNumber As String, ByVal VoucherId As Integer, ByVal CompanyName As String, ByVal canShowInActive As Boolean) As List(Of KeyValuePair(Of String, String))
        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet

        Dim PriceList As New List(Of KeyValuePair(Of String, String))

        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = If(String.IsNullOrEmpty(_wfr.Attribute("GetVoucherPriceCache", _languageCode, True)), False, Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("GetVoucherPriceCache", _languageCode, True)))
        voucher.DeVouch.ShowActiveAndInActiveRecords = canShowInActive
        voucher.DeVouch.CustomerNumber = CustomerNumber

        err = voucher.GetVoucherList()
        ds1 = voucher.ResultDataSet

        If ds1.Tables("VoucherList") IsNot Nothing AndAlso ds1.Tables("VoucherList").Rows.Count > 0 Then
            Dim VoucherDetailsTable As DataTable = ds1.Tables("VoucherList")
            VoucherId = (From r In VoucherDetailsTable.AsEnumerable()
                        Where r.Field(Of Integer)("VoucherId") = VoucherId
                        Select r.Field(Of Integer)("VoucherId")).FirstOrDefault()
        Else
            VoucherId = 0
        End If

        Dim dtVouchrPricesByBU As New DataTable
        dtVouchrPricesByBU = TDataObjects.PageSettings.TblVouchersExternal.GetVoucherPriceAndAgreement(BUSINESSUNIT, CompanyName, VoucherId, False)

        For Each item In dtVouchrPricesByBU.Rows
            PriceList.Add(New KeyValuePair(Of String, String)(Talent.eCommerce.Utilities.CheckForDBNull_String(item("AGREEMENT_CODE")) & " " & Talent.eCommerce.Utilities.CheckForDBNull_String(item("PRICE")), Talent.eCommerce.Utilities.CheckForDBNull_String(item("PRICE")) & "," & Talent.eCommerce.Utilities.CheckForDBNull_String(item("AGREEMENT_CODE"))))
        Next

        Return PriceList

    End Function

    Public Function RedeemGiftVoucherOrConvert(ByVal VoucherCode As String, ByVal CustomerNumber As String, ByVal RedeemMode As Talent.Common.RedeemMode _
                                        , ByRef VoucherPrice As String, ByRef AccountTotal As String, ByRef VoucherCodeReturned As String, ByRef returnCode As String) As Boolean
        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet
        Dim Result As Boolean = False
        Dim partner As String = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = False
        voucher.DeVouch.ShowActiveAndInActiveRecords = True
        voucher.DeVouch.CustomerNumber = CustomerNumber
        voucher.DeVouch.VoucherCode = VoucherCode
        voucher.DeVouch.RedeemMode = RedeemMode
        voucher.DeVouch.UniqueVoucherId = "0000000000000"

        err = voucher.RedeemVoucher()
        ds1 = voucher.ResultDataSet

        Dim RedeemVoucherTable As DataTable = ds1.Tables("GiftVoucherInfo")

        Dim voucherDetails = (From r In RedeemVoucherTable.AsEnumerable()
                                    Select New With {.VoucherPrice = r.Field(Of Decimal)("GiftVoucherPrice"),
                                                     .AccountTotal = r.Field(Of Decimal)("OnAccountTotal"),
                                                     .VoucherCodeReturned = r.Field(Of String)("VoucherCode")
                                                     }).FirstOrDefault
        If voucherDetails.VoucherPrice <> 0D Then
            VoucherPrice = TDataObjects.PaymentSettings.FormatCurrency(voucherDetails.VoucherPrice, TalentCache.GetBusinessUnit(), partner)
        Else
            VoucherPrice = String.Empty
        End If
        AccountTotal = TDataObjects.PaymentSettings.FormatCurrency(voucherDetails.AccountTotal, TalentCache.GetBusinessUnit(), partner)
        VoucherCodeReturned = voucherDetails.VoucherCodeReturned

        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
            returnCode = ds1.Tables("StatusResults").Rows(0).Item("ReturnCode")
        Else
            Result = True
        End If

        Return Result

    End Function

    Public Function RedeemExternalVoucher(ByVal ExternalCompany As String, ByVal CustomerNumber As String _
                                        , ByVal VoucherPrice As String, ByVal BoxOfficeUser As String, ByVal VoucherId As String _
                                        , ByRef AccountTotal As String, ByVal VoucherCode As String, ByVal ShowExternalVoucher As Boolean, ByVal AgreementCode As String, ByRef characterLimitError As Boolean) As Boolean
        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet
        Dim Result As Boolean = False

        If VoucherCode.Length > 30 Then
            characterLimitError = True
            Return False
        End If


        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = False
        voucher.DeVouch.ShowActiveAndInActiveRecords = True
        voucher.DeVouch.CustomerNumber = CustomerNumber
        voucher.DeVouch.ExternalCompany = ExternalCompany
        voucher.DeVouch.RedeemMode = Talent.Common.RedeemMode.Redeem
        voucher.DeVouch.VoucherPrice = Decimal.Parse(VoucherPrice)
        voucher.DeVouch.BoxOfficeUser = BoxOfficeUser
        voucher.DeVouch.VoucherCode = VoucherCode
        voucher.DeVouch.ExternalVoucherCodeFlag = ShowExternalVoucher
        voucher.DeVouch.AgreementCode = AgreementCode
        voucher.DeVouch.UniqueVoucherId = "0000000000000"
        voucher.DeVouch.VoucherDefinitionId = VoucherId

        err = voucher.RedeemVoucher()
        ds1 = voucher.ResultDataSet

        Dim RedemVoucherTable As DataTable = ds1.Tables("GiftVoucherInfo")

        Dim OnAccountTotal = (From r In RedemVoucherTable.AsEnumerable()
                        Select r.Field(Of Decimal)("OnAccountTotal")).FirstOrDefault()

        AccountTotal = TDataObjects.PaymentSettings.FormatCurrency(Decimal.Parse(OnAccountTotal), TalentCache.GetBusinessUnit(), TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
        Else
            Result = True
        End If

        Return Result

    End Function

    Public Function DeleteVoucher(ByVal VoucherCode As String, ByVal CustomerNumber As String, ByRef AccountTotal As String) As Boolean

        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet
        Dim Result As Boolean = False

        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = False
        voucher.DeVouch.ShowActiveAndInActiveRecords = True
        voucher.DeVouch.CustomerNumber = CustomerNumber
        voucher.DeVouch.UniqueVoucherId = VoucherCode
        voucher.DeVouch.RedeemMode = Talent.Common.RedeemMode.Delete

        err = voucher.RedeemVoucher()
        ds1 = voucher.ResultDataSet

        Dim RedemVoucherTable As DataTable = ds1.Tables("GiftVoucherInfo")

        Dim OnAccountTotal = (From r In RedemVoucherTable.AsEnumerable()
                        Select r.Field(Of Decimal)("OnAccountTotal")).FirstOrDefault()

        AccountTotal = TDataObjects.PaymentSettings.FormatCurrency(Decimal.Parse(OnAccountTotal), TalentCache.GetBusinessUnit(), TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))

        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
        Else
            Result = True
        End If

        Return Result

    End Function

    Public Function GetCustomerAccountBalance(ByVal CustomerNumber As String) As String
        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet
        Dim partner As String = TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile))
        Dim Result As String = TDataObjects.PaymentSettings.FormatCurrency(New Decimal(0.0), TalentCache.GetBusinessUnit(), partner)


        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = False
        voucher.DeVouch.CustomerNumber = CustomerNumber

        err = voucher.GetCustomerVoucherDetails()
        ds1 = voucher.ResultDataSet

        Dim VoucherDetailsTable As DataTable = ds1.Tables("UnusedVoucherList")
        If err.HasError OrElse ds1.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
        Else
            Dim OnAccountBalance = (From r In VoucherDetailsTable.AsEnumerable()
                                    Select r.Field(Of String)("OnAccountBalance")).FirstOrDefault()
            OnAccountBalance = If(OnAccountBalance = Nothing, "0.0", OnAccountBalance)
            Result = TDataObjects.PaymentSettings.FormatCurrency(Decimal.Parse(OnAccountBalance), TalentCache.GetBusinessUnit(), partner)
        End If

        Return Result
    End Function

    Public Function GetCompleteVoucherInformation(ByVal CustomerNumber As String, Optional ByVal AgentName As String = Nothing, Optional ByVal RetrieveUsedVoucher As Boolean = False) As DataTable
        Dim voucher As New Talent.Common.TalentVouchers
        Dim err As Talent.Common.ErrorObj
        Dim ds1 As DataSet
        Dim dt As New DataTable
        Dim dRow As DataRow = Nothing
        With dt.Columns
            .Add("VoucherId", GetType(Integer))
            .Add("Description", GetType(String))
            .Add("ExpiryDate", GetType(Date))
            .Add("SalePrice", GetType(String))
            .Add("OnAccountBalance", GetType(String))
            .Add("VoucherCode", GetType(String))
            .Add("Image", GetType(String))
            .Add("DescriptionText", GetType(String))
            .Add("IsExchangeToOnAccountActive", GetType(Boolean))
            .Add("ExchangeText", GetType(String))
            .Add("ExchangeButtonText", GetType(String))
            .Add("UniqueVoucherId", GetType(Integer))
            .Add("VoucherSource", GetType(String))
            .Add("ExternalCompanyName", GetType(String))
        End With

        voucher.Settings = Talent.eCommerce.Utilities.GetSettingsObject
        voucher.Settings.Cacheing = False
        voucher.DeVouch.CustomerNumber = CustomerNumber
        voucher.DeVouch.RetrieveUsedVoucher = RetrieveUsedVoucher
        If Not AgentName Is Nothing OrElse Not String.IsNullOrEmpty(AgentName) Then
            voucher.Settings.OriginatingSource = AgentName
        End If

        err = voucher.GetCustomerVoucherDetails()
        ds1 = voucher.ResultDataSet

        Dim VoucherDetailsTable As DataTable = ds1.Tables("UnusedVoucherList")
        Dim _wfrPage As New Talent.Common.WebFormResource

        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = VOUCHERDETAILSPAGE
            .KeyCode = VOUCHERDETAILSPAGE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings(CONNECTIONSTRING).ToString
        End With

        For Each dr As DataRow In VoucherDetailsTable.Rows
            If Not IsDBNull(dr("VoucherId")) Then
                dRow = Nothing
                dRow = dt.NewRow
                dRow("VoucherId") = dr("VoucherId")
                dRow("Description") = dr("Description")
                dRow("ExpiryDate") = Format(dr("ExpiryDate"), "dd/MM/yyyy")
                dRow("SalePrice") = dr("SalePrice")
                dRow("OnAccountBalance") = dr("OnAccountBalance")
                dRow("VoucherCode") = dr("VoucherCode")
                dRow("UniqueVoucherId") = dr("UniqueVoucherId")
                dRow("VoucherSource") = dr("VoucherSource")
                dRow("Image") = _wfrPage.Attribute(dr("VoucherId").ToString() + IMAGEPATH, _languageCode, True)
                dRow("DescriptionText") = _wfrPage.Content(dr("VoucherId").ToString() + TEXT, _languageCode, True)
                dRow("IsExchangeToOnAccountActive") = Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfrPage.Attribute(dr("VoucherId").ToString() + ONACCOUNTACTIVE, _languageCode, True))
                dRow("ExchangeText") = Utilities.CheckForDBNull_String(_wfrPage.Content(dr("VoucherId").ToString() + EXCHANGETEXT, _languageCode, True))
                dRow("ExchangeButtonText") = Utilities.CheckForDBNull_String(_wfrPage.Content(dr("VoucherId").ToString() + EXCHANGEBUTTONTEXT, _languageCode, True))
                dRow("ExternalCompanyName") = dr("ExternalCompanyName")
                dt.Rows.Add(dRow)
            End If

        Next

        Return dt

    End Function

    Public Function GetLinkDetails(ByVal VoucherId As Integer) As DataTable
        Dim _wfrPage As New Talent.Common.WebFormResource
        Dim attributeName As String = String.Concat(VoucherId.ToString(), LINK, "%")
        Dim dtLink As New DataTable
        Dim dRow As DataRow = Nothing
        With dtLink.Columns
            .Add("Link", GetType(String))
            .Add("LinkText", GetType(String))
            .Add("LinkButtonText", GetType(String))
        End With

        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = VOUCHERDETAILSPAGE
            .KeyCode = VOUCHERDETAILSPAGE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings(CONNECTIONSTRING).ToString
        End With

        Dim iTotalLinks As Integer = TDataObjects.PageSettings.TblPageAttribute.GetTotalLinksForVoucherHTML(_wfrPage.BusinessUnit, _wfrPage.PartnerCode, _wfrPage.PageCode, attributeName)
        For COUNT As Integer = 1 To iTotalLinks
            dRow = Nothing
            dRow = dtLink.NewRow
            dRow("Link") = _wfrPage.Attribute(VoucherId.ToString() & LINK & COUNT.ToString(), _languageCode, True)
            dRow("LinkText") = _wfrPage.Content(VoucherId.ToString() & LINKTEXT & COUNT.ToString(), _languageCode, True)
            dRow("LinkButtonText") = _wfrPage.Content(VoucherId.ToString() & LINKBUTTONTEXT & COUNT.ToString(), _languageCode, True)

            dtLink.Rows.Add(dRow)
        Next

        Return dtLink

    End Function

    Public Function GetVoucherDetails(ByVal CustomerNumber As String, ByVal VoucherId As Integer, ByRef ImagePath As String,
                                        ByRef VoucherTitle As String, ByRef VoucherDescription As String, ByRef RetrieveUsedVoucher As Boolean) As Boolean
        Dim result As Boolean = False
        Try


            Dim voucher = (From r In GetCompleteVoucherInformation(CustomerNumber, RetrieveUsedVoucher:=RetrieveUsedVoucher).AsEnumerable()
                           Where r.Field(Of Integer)("VoucherId") = VoucherId
                           Select New With {.VoucherTitle = r.Field(Of String)("Description"),
                                              .ImagePath = r.Field(Of String)("Image"),
                                              .VoucherDescription = r.Field(Of String)("DescriptionText")
                                              }).FirstOrDefault
            ImagePath = voucher.ImagePath
            VoucherTitle = voucher.VoucherTitle
            VoucherDescription = voucher.VoucherDescription

            result = True

        Catch ex As Exception
            result = False
        End Try

        Return result

    End Function

End Class

