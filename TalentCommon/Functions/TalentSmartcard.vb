Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentSmartcard
    Inherits TalentBase

    Private _de As DESmartcard

    Public Property DE() As DESmartcard
        Get
            Return _de
        End Get
        Set(ByVal value As DESmartcard)
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

    Public Function RetrieveSmartcardCardDetails() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "RetrieveSmartcardCardDetails"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSC As New DBSmartcard
        With dbSC
            .DE = DE
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError Then
                'Retrieve the customers smartcard card data.
                err = .AccessDatabase()
                If Not err.HasError Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function CallSmartcardFunction() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "CallSmartcardFunction"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSC As New DBSmartcard
        With dbSC
            .DE = DE
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError Then
                'Retrieve the customers smartcard card data.
                err = .AccessDatabase()
                If Not err.HasError Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveSmartcardCards() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "RetrieveSmartcardCards"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")

        Dim err As New ErrorObj

        Dim cacheKey As String = ModuleName & Settings.Company & DE.CustomerNumber
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbSC As New DBSmartcard
            With dbSC
                .DE = DE
                .Settings = Settings
                'Retrieve the customers smartcard card data.
                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveDEXList() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "RetrieveDEXList"
        TalentCommonLog(ModuleName, DE.ProductCode & "-" & DE.DEXListMode, "")
        Dim err As New ErrorObj

        Dim cacheKey As String = ModuleName & Settings.Company & DE.ProductCode & DE.DEXListMode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbSC As New DBSmartcard
            With dbSC
                .DE = DE
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveAvailableFanIdList() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "RetrieveAvailableFanIdList"
        TalentCommonLog(ModuleName, DE.CustomerNumber, DE.CustomerNumber)
        Dim err As New ErrorObj

        Dim cacheKey As String = ModuleName & Settings.Company & DE.CustomerNumber
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbSC As New DBSmartcard
            With dbSC
                .DE = DE
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function UpdateCurrentFanId() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "UpdateCurrentFanId"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSC As New DBSmartcard
        With dbSC
            .DE = DE
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError Then
                err = .AccessDatabase()
                If Not err.HasError Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ClearAvailableFanIdListCache() As Boolean
        Dim cacheKey As New Text.StringBuilder
        cacheKey.Append("RetrieveAvailableFanIdList")
        cacheKey.Append(Settings.Company)
        cacheKey.Append(DE.CustomerNumber)
        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey.ToString()) Then
            'Remove the fan id cache
            HttpContext.Current.Cache.Remove(cacheKey.ToString())

            'Remove the smartcard cache
            cacheKey.Clear()
            cacheKey.Append("RetrieveAvailableFanIdList")
            cacheKey.Append(Settings.Company)
            cacheKey.Append(DE.CustomerNumber)
            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey.ToString()) Then
                HttpContext.Current.Cache.Remove(cacheKey.ToString())
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Function RequestPrintCard() As ErrorObj
        Const ModuleName As String = "RequestPrintCard"
        TalentCommonLog(ModuleName, _de.Mode, _de.SessionID)
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSC As New DBSmartcard
        With dbSC
            .DE = _de
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function ExternalSmartcardReprintRequest() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "ExternalSmartcardReprintRequest"
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSC As New DBSmartcard
        With dbSC
            .DE = DE
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With

        Return err
    End Function
End Class