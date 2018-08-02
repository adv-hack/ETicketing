Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentBasketProcessor
    Inherits TalentBase

    Public Property ResultDataSet() As DataSet

    Public Function ProcessBasket(ByVal basketHeaderID As String, ByVal dsBasketFromBackend As DataSet) As ErrorObj
        Dim errObj As New ErrorObj
        Try
            'get the tables from dataset
            If dsBasketFromBackend.Tables.Contains("BasketDetailExceptions") Then
                errObj = TDataObjects.BasketSettings.ProcessNewBasket(basketHeaderID, dsBasketFromBackend.Tables("BasketHeader"), dsBasketFromBackend.Tables("BasketDetail"), dsBasketFromBackend.Tables("BasketDetailExceptions"))
            Else
                errObj = TDataObjects.BasketSettings.ProcessNewBasket(basketHeaderID, dsBasketFromBackend.Tables("BasketHeader"), dsBasketFromBackend.Tables("BasketDetail"))
            End If
        Catch ex As Exception
            errObj.HasError = True
            errObj.ErrorNumber = "TACTBP-001"
            errObj.ErrorStatus = "Failed while calling ProcessNewBasket with basket from backend"
        End Try

        If errObj.HasError Then
            dsBasketFromBackend.Tables.Add(BindErrorToResultSet(errObj))
        Else
            ProcessNewBasketSummary(basketHeaderID, False)
        End If

        Me.ResultDataSet = dsBasketFromBackend
        Return errObj
    End Function

    Public Function ProcessSummaryForUpdatedBasket(ByVal basketHeaderID As String) As ErrorObj
        Dim errObj As New ErrorObj
        Try
            ProcessNewBasketSummary(basketHeaderID, True)
        Catch ex As Exception
            errObj.HasError = True
            errObj.ErrorNumber = "TACTBP-002"
            errObj.ErrorStatus = "Failed while calling ProcessNewBasketSummary with retail basket from frontend"
        End Try
        Return errObj
    End Function

    Private Sub ProcessNewBasketSummary(ByVal basketHeaderID As String, ByVal isUpdatedBasketProcess As Boolean)
        'remove the cashback cache
        Dim payment As New TalentPayment
        Dim paySettings As DESettings = Me.Settings
        paySettings.Cacheing = True
        payment.Settings = paySettings
        payment.RemoveItemFromCache("RetrieveCashback" & paySettings.Company & paySettings.LoginId)
        'calculate basket summary - get basketheader entity, basketdetail entity
        Dim basketEntity As New DEBasket
        Dim basketItemEntity As New DEBasketItem
        Dim dtBasket As New DataTable
        dtBasket = TDataObjects.BasketSettings.TblBasketHeader.GetHeaderByBasketHeaderID(basketHeaderID)
        Dim properties As ArrayList = Utilities.GetPropertyNames(basketEntity)
        basketEntity = PopulateProperties(properties, dtBasket, basketEntity, 0)
        properties = Utilities.GetPropertyNames(basketItemEntity)
        dtBasket = TDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderID(basketHeaderID)
        If dtBasket.Rows.Count > 0 Then
            For rowIndex As Integer = 0 To dtBasket.Rows.Count - 1
                basketItemEntity = New DEBasketItem
                basketItemEntity = PopulateProperties(properties, dtBasket.Rows(rowIndex), basketItemEntity)
                basketEntity.BasketItems.Add(basketItemEntity)
            Next
        End If
        Dim talBasketSummary As New TalentBasketSummary
        talBasketSummary.Settings = Settings
        talBasketSummary.BasketEntity = basketEntity
        talBasketSummary.BasketItemEntityList = basketEntity.BasketItems
        talBasketSummary.CardTypeFeeCategory = CardTypeFeeCategory
        talBasketSummary.FulfilmentFeeCategory = FulfilmentFeeCategory
        talBasketSummary.IsUpdatedBasketProcess = isUpdatedBasketProcess
        Dim errObj As ErrorObj = talBasketSummary.ProcessBasketForSummary()
    End Sub

    Private Function BindErrorToResultSet(ByVal errObj As ErrorObj) As DataTable
        Dim dtBasketProcessorStatus As New DataTable("BasketProcessorStatus")
        With dtBasketProcessorStatus.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("ErrorStatus", GetType(String))
        End With
        Dim dr As DataRow = dtBasketProcessorStatus.NewRow
        dr("ErrorOccurred") = "E"
        dr("ReturnCode") = errObj.ErrorNumber
        dr("ErrorStatus") = errObj.ErrorStatus
        dtBasketProcessorStatus.Rows.Add(dr)
        dr = Nothing
        Return dtBasketProcessorStatus
    End Function

End Class
