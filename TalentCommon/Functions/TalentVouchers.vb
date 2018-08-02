Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web

<Serializable()> _
Public Class TalentVouchers
    Inherits TalentBase

    Public Property DeVouch() As New DEVouchers
    Public Property ResultDataSet() As DataSet

    Const ModuleName As String = "GetVoucherList"

    Public Function GetVoucherClearCache() As Boolean
        RemoveItemFromCache("VoucherDetails")
    End Function

    Public Function GetVoucherList() As ErrorObj
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.BusinessUnit & Settings.Partner & DeVouch.ShowActiveAndInActiveRecords
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim DBVouch As New DBVouchers

            With DBVouch
                .DeVouch = DeVouch
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "VoucherDetails")
                End If
            End With
        End If

        Return err
    End Function

    ''' <summary>
    ''' Creates or updates a voucher definition.
    ''' </summary>
    ''' <returns>An instance of ErrorObj.</returns>
    ''' <remarks></remarks>
    Public Function AddOrUpdateVoucherDetails() As ErrorObj
        Const ModuleName As String = "AddOrUpdateVoucherDetails"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
       
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBVouch As New DBVouchers

        With DBVouch
            .DeVouch = DeVouch
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With

        Return err
    End Function

    Public Function RedeemVoucher() As ErrorObj
        Const ModuleName As String = "RedeemVoucher"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBVouch As New DBVouchers
        With DBVouch
            .DeVouch = DeVouch
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With

        Return err
    End Function

    Public Function ConvertVoucherToOnAccount() As ErrorObj
        Const ModuleName As String = "ConvertVoucherToOnAccount"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBVouch As New DBVouchers
        With DBVouch
            .DeVouch = DeVouch
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With

        Return err
    End Function


    Public Function GetCustomerVoucherDetails() As ErrorObj
        Const ModuleName As String = "GetCustomerVoucherDetails"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBVouch As New DBVouchers

        With DBVouch
            .DeVouch = DeVouch
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With


        Return err
    End Function

End Class


