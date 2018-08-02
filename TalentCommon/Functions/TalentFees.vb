Imports System.Data
Imports System.Text
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentFees
    Inherits TalentBase

#Region "Helper Class"
    Friend Class TalentFeesCacheHolder

        Public Property ResultDataSet() As DataSet = Nothing
        Public Property FeesCategoryList() As List(Of String) = Nothing
        Public Property Fees() As Dictionary(Of String, List(Of DEFees)) = Nothing
        Public Property FeeCodes() As Dictionary(Of String, String) = Nothing
        Public Property FeesApplyToWeb() As Dictionary(Of String, List(Of DEFees)) = Nothing
        Public Property FeeCodesApplyToWeb() As Dictionary(Of String, String) = Nothing
        Public Property ProductExcludeFees() As Dictionary(Of String, String) = Nothing
        Public Property WebSalesDepartment() As String = ""

    End Class
#End Region

#Region "Class Level Fields"
    Private Const CLASSNAME As String = "TalentFees"
    Private _productExcludeFees As Dictionary(Of String, String) = Nothing
    Private _fees As Dictionary(Of String, List(Of DEFees)) = Nothing
    Private _feesApplyToWeb As Dictionary(Of String, List(Of DEFees)) = Nothing
    Private _feesCategory As List(Of String) = Nothing
    Private _feeCodes As Dictionary(Of String, String) = Nothing
    Private _feeCodesApplyToWeb As Dictionary(Of String, String) = Nothing
    Private _webSalesDepartment As String = ""
#End Region

#Region "Properties"

    Public Property ResultDataSet() As DataSet = Nothing

    Public ReadOnly Property FeesCategoryList() As List(Of String)
        Get
            Return _feesCategory
        End Get
    End Property

    Public ReadOnly Property Fees() As Dictionary(Of String, List(Of DEFees))
        Get
            Return _fees
        End Get
    End Property

    Public ReadOnly Property FeeCodes() As Dictionary(Of String, String)
        Get
            Return _feeCodes
        End Get
    End Property

    Public ReadOnly Property FeesApplyToWeb() As Dictionary(Of String, List(Of DEFees))
        Get
            Return _feesApplyToWeb
        End Get
    End Property

    Public ReadOnly Property FeeCodesApplyToWeb() As Dictionary(Of String, String)
        Get
            Return _feeCodesApplyToWeb
        End Get
    End Property

    Public ReadOnly Property ProductExcludeFees() As Dictionary(Of String, String)
        Get
            Return _productExcludeFees
        End Get
    End Property

    Public ReadOnly Property WebSalesDepartment() As String
        Get
            Return _webSalesDepartment
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Function FeesList() As ErrorObj
        Const ModuleName As String = "FeesList"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & "")
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj

        Dim cacheKey As String = ModuleName & Settings.Company & Settings.OriginatingSourceCode & Settings.BusinessUnit

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            GetAllItemFromCache(cacheKey)
        Else
            Dim dbFees As New DBFees
            With dbFees
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    err = AddAllItemToCache(cacheKey)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        err = AddAllItemToCache(cacheKey)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function IsBookingFeesPercentageBased() As Boolean
        Dim isPercentageBased As Boolean = False
        Dim err As ErrorObj = FeesList()
        If Not err.HasError Then
            If Settings.IsAgent Then
                isPercentageBased = IsBookFeePercentBasedForAgent()
            Else
                isPercentageBased = IsBookFeePercentBasedForWebSales()
            End If
        End If
        Return isPercentageBased
    End Function

    Private Function IsBookFeePercentBasedForAgent() As Boolean
        Dim isPercentageBased As Boolean = False
        Dim feeEntityList As List(Of DEFees) = Nothing
        If Fees IsNot Nothing _
            AndAlso Fees.Count > 0 _
            AndAlso Fees.TryGetValue(GlobalConstants.FEECATEGORY_BOOKING, feeEntityList) _
            AndAlso feeEntityList.Count > 0 Then
            If (Not String.IsNullOrWhiteSpace(feeEntityList.Item(0).ChargeType)) AndAlso (feeEntityList.Item(0).ChargeType = GlobalConstants.FEECHARGETYPE_PERCENTAGE) Then
                isPercentageBased = True
            End If
        End If
        Return isPercentageBased
    End Function

    Private Function IsBookFeePercentBasedForWebSales() As Boolean
        Dim isPercentageBased As Boolean = False
        Dim feeEntityList As List(Of DEFees) = Nothing
        If FeesApplyToWeb IsNot Nothing _
            AndAlso FeesApplyToWeb.Count > 0 _
            AndAlso FeesApplyToWeb.TryGetValue(GlobalConstants.FEECATEGORY_BOOKING, feeEntityList) _
            AndAlso feeEntityList.Count > 0 Then
            If (Not String.IsNullOrWhiteSpace(feeEntityList.Item(0).ChargeType)) AndAlso (feeEntityList.Item(0).ChargeType = GlobalConstants.FEECHARGETYPE_PERCENTAGE) Then
                isPercentageBased = True
            End If
        End If
        Return isPercentageBased
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Populate the list of fees based on the setup in TALENT. Retrieve all the data in FeesList from WS175R and move it to the unique fees category list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateFeesObjects()
        Try
            If CanCreateFeesObjects() Then
                _feesCategory = New List(Of String)
                _fees = New Dictionary(Of String, List(Of DEFees))
                _feesApplyToWeb = New Dictionary(Of String, List(Of DEFees))
                _feeCodes = New Dictionary(Of String, String)
                _feeCodesApplyToWeb = New Dictionary(Of String, String)
                Dim tempFeesEntity As DEFees = Nothing
                Dim tempFeesEntityList As List(Of DEFees) = Nothing
                Dim dtFeesList As DataTable = ResultDataSet.Tables("FeesList")
                Dim tempFeeCategoryName As String = String.Empty
                For rowIndex As Integer = 0 To dtFeesList.Rows.Count - 1
                    tempFeesEntityList = Nothing
                    tempFeeCategoryName = GetFeeCategoryName(dtFeesList.Rows(rowIndex))
                    tempFeesEntity = GetFeesEntity(dtFeesList.Rows(rowIndex))

                    'populate distinct fees codes 
                    PopulateFeeCodes(tempFeesEntity)

                    'all fees
                    If _fees.TryGetValue(tempFeeCategoryName, tempFeesEntityList) Then
                        tempFeesEntityList.Add(tempFeesEntity)
                        _fees(tempFeeCategoryName) = tempFeesEntityList
                    Else
                        'new fee category
                        _feesCategory.Add(tempFeeCategoryName)
                        tempFeesEntityList = New List(Of DEFees)
                        tempFeesEntityList.Add(tempFeesEntity)
                        _fees.Add(tempFeeCategoryName, tempFeesEntityList)
                    End If

                    'fees apply to web
                    If tempFeesEntity.ApplyWebSales Then
                        tempFeesEntityList = Nothing
                        If _feesApplyToWeb.TryGetValue(tempFeeCategoryName, tempFeesEntityList) Then
                            tempFeesEntityList.Add(tempFeesEntity)
                            _feesApplyToWeb(tempFeeCategoryName) = tempFeesEntityList
                        Else
                            'new fee category
                            tempFeesEntityList = Nothing
                            tempFeesEntityList = New List(Of DEFees)
                            tempFeesEntityList.Add(tempFeesEntity)
                            _feesApplyToWeb.Add(tempFeeCategoryName, tempFeesEntityList)
                        End If
                    End If

                Next
                PopulateProductExcludeFees()
                PopulateWebSalesDepartment()
            End If
        Catch ex As Exception
            Throw
        End Try

    End Sub

    Private Sub PopulateFeeCodes(ByVal feeEntity As DEFees)
        If Not _feeCodes.ContainsKey(feeEntity.FeeCode) Then
            _feeCodes.Add(feeEntity.FeeCode, feeEntity.FeeCode)
        End If
        If feeEntity.ApplyWebSales Then
            If Not _feeCodesApplyToWeb.ContainsKey(feeEntity.FeeCode) Then
                _feeCodesApplyToWeb.Add(feeEntity.FeeCode, feeEntity.FeeCode)
            End If
        End If
    End Sub

    Private Sub PopulateProductExcludeFees()
        _productExcludeFees = New Dictionary(Of String, String)
        If ResultDataSet.Tables("FeesExcludedProducts") IsNot Nothing _
                AndAlso ResultDataSet.Tables("FeesExcludedProducts").Rows.Count > 0 Then
            Dim dtProductExcludeFees As DataTable = ResultDataSet.Tables("FeesExcludedProducts")
            For rowIndex As Integer = 0 To dtProductExcludeFees.Rows.Count - 1
                If Not _productExcludeFees.ContainsKey(dtProductExcludeFees.Rows(rowIndex)("ProductCode")) Then
                    _productExcludeFees.Add(dtProductExcludeFees.Rows(rowIndex)("ProductCode"), dtProductExcludeFees.Rows(rowIndex)("ProductCode"))
                End If
            Next
        End If
    End Sub

    Private Sub PopulateWebSalesDepartment()
        If ResultDataSet.Tables("WebSalesDepartment") IsNot Nothing _
                AndAlso ResultDataSet.Tables("WebSalesDepartment").Rows.Count > 0 Then
            _webSalesDepartment = Utilities.CheckForDBNull_String(ResultDataSet.Tables("WebSalesDepartment").Rows(0)("WebSalesDepartment"))
        End If
    End Sub

    Private Function AddAllItemToCache(ByVal cacheKey As String) As ErrorObj
        Dim err As New ErrorObj
        Try
            PopulateFeesObjects()
            Dim talFeesCacheHolder As New TalentFeesCacheHolder
            talFeesCacheHolder.FeeCodes = FeeCodes
            talFeesCacheHolder.FeeCodesApplyToWeb = FeeCodesApplyToWeb
            talFeesCacheHolder.Fees = Fees
            talFeesCacheHolder.FeesApplyToWeb = FeesApplyToWeb
            talFeesCacheHolder.FeesCategoryList = FeesCategoryList
            talFeesCacheHolder.ProductExcludeFees = ProductExcludeFees
            talFeesCacheHolder.WebSalesDepartment = WebSalesDepartment
            talFeesCacheHolder.ResultDataSet = ResultDataSet
            AddItemToCache(cacheKey, talFeesCacheHolder, Settings)
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACTF-001"
            err.ErrorMessage = ex.Message
            TalentLogger.Logging(CLASSNAME, "AddAllItemToCache", "Error while building fees cache object", err, ex, LogTypeConstants.TCBMFEESLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Sub GetAllItemFromCache(ByVal cacheKey As String)
        Try
            Dim talFeesCacheHolder As TalentFeesCacheHolder = CType(HttpContext.Current.Cache.Item(cacheKey), TalentFeesCacheHolder)
            If talFeesCacheHolder IsNot Nothing Then
                _feeCodes = talFeesCacheHolder.FeeCodes
                _feeCodesApplyToWeb = talFeesCacheHolder.FeeCodesApplyToWeb
                _fees = talFeesCacheHolder.Fees
                _feesApplyToWeb = talFeesCacheHolder.FeesApplyToWeb
                _feesCategory = talFeesCacheHolder.FeesCategoryList
                _productExcludeFees = talFeesCacheHolder.ProductExcludeFees
                _webSalesDepartment = talFeesCacheHolder.WebSalesDepartment
                ResultDataSet = talFeesCacheHolder.ResultDataSet
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Function CanCreateFeesObjects() As Boolean
        Dim canCreate As Boolean = False
        If ResultDataSet IsNot Nothing _
            AndAlso ResultDataSet.Tables.Count > 0 _
            AndAlso ResultDataSet.Tables("ResultStatus").Rows.Count > 0 _
            AndAlso ResultDataSet.Tables("ResultStatus").Rows(0)("ErrorOccurred").ToString().Trim = "" _
            AndAlso ResultDataSet.Tables.Count > 1 _
            AndAlso ResultDataSet.Tables("FeesList").Rows.Count > 0 Then
            canCreate = True
        End If
        Return canCreate
    End Function

    ''' <summary>
    ''' Get the fee category name providing the fee code isn't a fulfilment fee. The fulfilment fees need to be grouped together under the "FULFILMENT" category instead of using the original fee category.
    ''' </summary>
    ''' <param name="drFees">The current fee row</param>
    ''' <returns>The formatted category name</returns>
    ''' <remarks></remarks>
    Private Function GetFeeCategoryName(ByVal drFees As DataRow) As String
        Dim feeCategoryName As String = Nothing
        If FulfilmentFeeCategory.ContainsValue(drFees("FeeCode")) Then
            feeCategoryName = GlobalConstants.FEECATEGORY_FULFILMENT
        Else
            feeCategoryName = drFees("FeeCategory")
        End If
        Return feeCategoryName
    End Function

    Private Function GetFeesEntity(ByVal drFees As DataRow) As DEFees
        Dim feesEntity As New DEFees
        feesEntity.FeeCode = drFees("FeeCode")
        feesEntity.FeeCategory = drFees("FeeCategory")
        feesEntity.FeeDescription = drFees("FeeDescription")
        feesEntity.FeeDepartment = drFees("FeeDepartment")
        feesEntity.FeeType = drFees("FeeType")
        feesEntity.FeeValue = drFees("FeeValue")
        feesEntity.CardType = drFees("CardType")
        feesEntity.ProductType = drFees("ProductType")
        feesEntity.ProductCode = drFees("ProductCode")
        feesEntity.ApplyWebSales = drFees("ApplyWebSales")
        feesEntity.IsNegativeFee = drFees("IsNegativeFee")
        feesEntity.IsSystemFee = drFees("IsSystemFee")
        feesEntity.IsChargeable = drFees("ChargeFee")
        feesEntity.FeeFunction = drFees("FeeFunction")
        feesEntity.ApplyFeeTo = drFees("ApplyFeeTo")
        feesEntity.ChargeType = drFees("ChargeType")
        feesEntity.GeographicalZone = drFees("GeographicalZone")
        Return feesEntity
    End Function

#End Region

End Class
