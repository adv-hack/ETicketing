Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentWaitList
    Inherits TalentBase


    Private _de As DEWaitList
    Public Property DE() As DEWaitList
        Get
            Return _de
        End Get
        Set(ByVal value As DEWaitList)
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

    Public Function RetrieveCustomerWaitListHistory() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "RetrieveCustomerWaitListHistory"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company & "Cust=" & DE.CustomerNumber
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbWL As New DBWaitList
            With dbWL
                .DE = DE
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError Then
                    'Retrieve the available stands and areas
                    err = .AccessDatabase()
                    If Not err.HasError Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function


    Public Function WithdrawCustomerWaitListRequest() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "WithdrawCustomerWaitListRequest"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")

        Dim err As New ErrorObj
      
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbWL As New DBWaitList
        With dbWL
            .DE = DE
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError Then
                'Retrieve the available stands and areas
                err = .AccessDatabase()
                If Not err.HasError Then
                    ResultDataSet = .ResultDataSet

                    'Remove the History Cache when a Withdrawal is made
                    Dim cacheKey As String = ModuleName & Settings.Company & "Cust=" & DE.CustomerNumber
                    HttpContext.Current.Cache.Remove(cacheKey)
                End If
            End If
        End With

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function AddCustomerWaitListRequest() As ErrorObj
        ResultDataSet = New DataSet
        Const ModuleName As String = "AddCustomerWaitListRequest"
        TalentCommonLog(ModuleName, DE.CustomerNumber, "")

        Dim err As New ErrorObj
      
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbWL As New DBWaitList
        With dbWL
            .DE = DE
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError Then
                'Retrieve the available stands and areas
                err = .AccessDatabase()
                If Not err.HasError Then
                    ResultDataSet = .ResultDataSet

                    'Remove the History Cache when an ADD is made
                    Dim cacheKey As String = ModuleName & Settings.Company & "Cust=" & DE.CustomerNumber
                    HttpContext.Current.Cache.Remove(cacheKey)
                End If
            End If
        End With

        TalentCommonLog(ModuleName, DE.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

End Class
