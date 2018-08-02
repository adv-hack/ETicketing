Imports System.Web

<Serializable()> _
Public Class TalentAgent
    Inherits TalentBase

#Region "Public Properties"

    Public Property AgentDataEntity() As DEAgent
    Public Property ResultDataSet() As DataSet

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Agent Login Function
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function AgentLogin() As ErrorObj
        Const ModuleName As String = "AgentLogin"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With agent
            .AgentDataEntity = AgentDataEntity
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve all active agents in the system
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveAllAgents() As ErrorObj
        Const ModuleName As String = "RetrieveAllAgents"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With agent
                .AgentDataEntity = AgentDataEntity
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        Return err
    End Function

    ''' <summary>
    ''' Retrieve printers for a given agent
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveAgentPrinters() As ErrorObj
        Const ModuleName As String = "RetrieveAgentPrinters"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With agent
            .AgentDataEntity = AgentDataEntity
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve approved agent reservation codes
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveApprovedReservationCodes() As ErrorObj
        Const ModuleName As String = "RetrieveApprovedReservationCode"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.Company & AgentDataEntity.AgentUsername
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            TalDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), TalentDataSet)
        Else
            With agent
                .AgentDataEntity = AgentDataEntity
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    TalDataSet.ResultDataSet = .ResultDataSet
                    TalDataSet.DictionaryDataSet = New Generic.Dictionary(Of String, String)
                    If TalDataSet.ResultDataSet.Tables.Count = 2 Then

                        'Sell available tickets
                        If TalDataSet.ResultDataSet.Tables.Count = 2 AndAlso Not TalDataSet.ResultDataSet.Tables("StatusResults") Is Nothing Then
                            If TalDataSet.ResultDataSet.Tables("StatusResults").Rows(0).Item("SellAvailableTickets") = "Y" Then
                                TalDataSet.DictionaryDataSet.Add(GlobalConstants.AGENT_SELL_AVAILABLE, "Y")
                            End If
                        End If

                        'Add the reservation codes
                        For Each dr As DataRow In TalDataSet.ResultDataSet.Tables("ApprovedReservationCodes").Rows
                            TalDataSet.DictionaryDataSet.Add(dr("ReservationCode"), dr("ReservationCode"))
                        Next
                    End If
                    AddItemToCache(cacheKey, TalDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        Return err
    End Function

    ''' <summary>
    ''' Update the agent printers
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function UpdateAgentPrinters() As ErrorObj
        Const ModuleName As String = "UpdateAgentPrinters"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With agent
            .AgentDataEntity = AgentDataEntity
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the agent saved searches
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveSavedSearch() As ErrorObj
        Const ModuleName As String = "RetrieveSavedSearch"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ConstructSavedSearchesCacheKey(Settings.Company, AgentDataEntity.AgentUsername, ModuleName)

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With agent
                .AgentDataEntity = AgentDataEntity
                .AgentDataEntity.SavedSearchMode = "R"
                .Settings = Settings

                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        Return err
    End Function

    ''' <summary>
    ''' Create a new agent saved search
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function CreateNewSavedSearch() As ErrorObj
        Const ModuleName As String = "CreateNewSavedSearch"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName

        With agent
            .AgentDataEntity = AgentDataEntity
            .AgentDataEntity.SavedSearchMode = "C"
            .Settings = Settings
            err = .AccessDatabase()

            Dim cacheKey As String = ConstructSavedSearchesCacheKey(Settings.Company, AgentDataEntity.AgentUsername)
            If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                RemoveItemFromCache(cacheKey)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                ResultDataSet = .ResultDataSet
                AddItemToCache(cacheKey, ResultDataSet, Settings)
            Else
                RemoveItemFromCache(cacheKey)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Delete an agent saved search
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function DeleteSavedSearch() As ErrorObj
        Const ModuleName As String = "DeleteSavedSearch"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName

        With agent
            .AgentDataEntity = AgentDataEntity
            .AgentDataEntity.SavedSearchMode = "D"
            .Settings = Settings
            err = .AccessDatabase()

            Dim cacheKey As String = ConstructSavedSearchesCacheKey(Settings.Company, AgentDataEntity.AgentUsername)
            If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                RemoveItemFromCache(cacheKey)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                ResultDataSet = .ResultDataSet
                AddItemToCache(cacheKey, ResultDataSet, Settings)
            Else
                RemoveItemFromCache(cacheKey)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve agent departments
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveAgentDepartments() As ErrorObj
        Const ModuleName As String = "RetrieveAgentDepartment"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ConstructAgentDepartmentsCacheKey(Settings.Company, Settings.BusinessUnit)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With agent
                .AgentDataEntity = AgentDataEntity
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                Else
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        Return err
    End Function

    ''' <summary>
    ''' Agent Logout Functionality
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function AgentLogout() As ErrorObj
        Const ModuleName As String = "AgentLogout"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With agent
            .AgentDataEntity = AgentDataEntity
            .Settings = Settings
            err = .AccessDatabase()
        End With
        Return err
    End Function

    ''' <summary>
    ''' Remove the agents data from cache
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RetrieveAllAgentsClearCache()
        Dim cacheKey As String = "RetrieveAllAgents" & Settings.Company
        RemoveItemFromCache(cacheKey)
    End Sub

    Public Function AgentCopy() As ErrorObj
        Const ModuleName As String = "AgentCopy"
        Dim err As New ErrorObj
        Dim agent As New DBAgent
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        
        With agent
            .AgentDataEntity = AgentDataEntity
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Construct the cache key for saved agent searches
    ''' </summary>
    ''' <param name="Company">The agent company code</param>
    ''' <param name="AgentName">The agent name</param>
    ''' <param name="ModuleName">The cache module name</param>
    ''' <returns>A formatted cache string</returns>
    ''' <remarks></remarks>
    Private Function ConstructSavedSearchesCacheKey(ByVal Company As String, ByVal AgentName As String, Optional ByVal ModuleName As String = "RetrieveSavedSearch") As String
        Return ModuleName & Company & AgentName
    End Function

    ''' <summary>
    ''' Construct the cache key for Retrieve Agent Departments
    ''' </summary>
    ''' <param name="Company">The agent company code</param>
    ''' <param name="BusinessUnit">The Business Unit</param>
    ''' <param name="ModuleName">The cache module name</param>
    ''' <returns>A formatted cache string</returns>
    ''' <remarks></remarks>
    Private Function ConstructAgentDepartmentsCacheKey(ByVal Company As String, ByVal BusinessUnit As String, Optional ByVal ModuleName As String = "RetrieveAgentDepartment") As String
        Return ModuleName & Company & BusinessUnit
    End Function

#End Region

End Class