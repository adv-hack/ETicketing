Imports System.Data
Imports Talent.Common

Partial Class PagesAdmin_TalentToEBusiness_Tester
    Inherits System.Web.UI.Page
    Dim _talDataObjects As New Talent.Common.TalentDataObjects
    Dim _settingsEntity As New DESettings

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Initialise()
        CallPurchaseHistory()
    End Sub
    Private Sub GetCardTypeCVCAndAVS()
        Dim dic As DECVCAndAVSAuthorization = _talDataObjects.PaymentSettings.TblCreditCard.GetCardTypeCVCAndAVS("VISA")
    End Sub
    Private Sub Initialise()
        '_settingsEntity.
        With _settingsEntity
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .DestinationDatabase = "SQL2005"
            .BusinessUnit = TalentCache.GetBusinessUnit
            .Partner = TalentCache.GetDefaultPartner
        End With
        _settingsEntity = Talent.eCommerce.Utilities.GetSettingsObject
        _talDataObjects.Settings = _settingsEntity
    End Sub

    Private Sub TestFunctionality()
        'CALLWS175R()
        CallVG003S()
    End Sub

    Private Sub CALLWS175R()
        Dim errObj As New ErrorObj
        Try
            Dim talFees As New TalentFees
            talFees.Settings = _settingsEntity
            'talFees.FeeCategoryApplyType=_talDataObjects.FeesSettings.TblFeesRelations.g
            talFees.CardTypeFeeCategory = _talDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talFees.Settings.BusinessUnit)
            talFees.FulfilmentFeeCategory = _talDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talFees.Settings.BusinessUnit)
            errObj = talFees.FeesList()

            If Not errObj.HasError Then

            End If
        Catch ex As Exception
            errObj.HasError = True
            errObj.ErrorMessage = ex.Message
        End Try

    End Sub

    Private Sub CallVG003S()
        Dim err As ErrorObj
        Try
            Dim talVG As New TalentVanguard
            talVG.Settings = _settingsEntity
            'Dim dt As datatable = talVG.GetVanguardConfigurations(1)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub CallPurchaseHistory()
        Dim str As String = String.Empty
        str = Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " ---- "
        Dim order As New TalentOrder
        Dim settings As DESettings = _settingsEntity
        Dim err As New ErrorObj
        settings.AccountNo1 = "000000002009"
        order.Dep.CallId = "0"
        order.Dep.PaymentReference = 0
        order.Dep.CustNumberPayRefShouldMatch = False
        settings.Cacheing = False
        settings.OriginatingSource = "ECC001"
        settings.AuthorityUserProfile = "INTERNET"
        order.Dep.FromDate = ""
        order.Dep.ToDate = ""
        order.Dep.OrderStatus = ""
        order.Dep.CorporateProductsOnly = False
        order.Dep.LastRecordNumber = 0
        order.Dep.TotalRecords = 0
        order.Settings() = settings
        err = order.OrderEnquiryDetails()
        str = str & Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & "<br/>"
        If Not err.HasError AndAlso order.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            str = str & RowsCount(order.ResultDataSet, "StatusResults")
            str = str & RowsCount(order.ResultDataSet, "PaymentOwnerDetails")
            str = str & RowsCount(order.ResultDataSet, "OrderEnquiryDetails")
            str = str & RowsCount(order.ResultDataSet, "PaymentDetails")
            str = str & RowsCount(order.ResultDataSet, "PackageHistory")
            str = str & RowsCount(order.ResultDataSet, "PackageDetail")
            str = str & RowsCount(order.ResultDataSet, "ComponentSummary")
            ltlPurchaseHistoryStatus.Text = str & "***SUCCESS***"
        Else
            ltlPurchaseHistoryStatus.Text = "***FAILED***"
        End If
    End Sub

    Private Function RowsCount(ByVal resultset As DataSet, ByVal dtTableName As String) As String
        If resultset.Tables(dtTableName) IsNot Nothing Then
            Return dtTableName & " : " & resultset.Tables(dtTableName).Rows.Count & "<br/>"
        Else
            Return dtTableName & " : Nothing <br/>"
        End If
    End Function

End Class
