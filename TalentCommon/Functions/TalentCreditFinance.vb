Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common.Utilities

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Credit Finance
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentCreditFinance
    Inherits TalentBase


    Private _de As New DECreditFinance
    Public Property De() As DECreditFinance
        Get
            Return _de
        End Get
        Set(ByVal value As DECreditFinance)
            _de = value
        End Set
    End Property

    Private _resultDataSet As DataSet
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
 
    Public Function CreditFinanceOptions() As ErrorObj
        Const ModuleName As String = "CreditFinanceOptions"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.CreditFinanceCompanyCode)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & De.CreditFinanceCompanyCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbCreditFinance As New DBCreditFinance
            With dbCreditFinance
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    'Return the result set
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function CreditFinanceOptionDetails() As ErrorObj
        Const ModuleName As String = "CreditFinanceOptionDetails"
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.CreditFinanceCompanyCode & " ; " & De.CreditFinanceOptionCode)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & De.CreditFinanceCompanyCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbCreditFinance As New DBCreditFinance
            With dbCreditFinance
                .Settings = Settings
                .De = De
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    'Return the result set
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

End Class
