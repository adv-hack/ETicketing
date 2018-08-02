Imports Microsoft.VisualBasic
Imports System.data
Imports System.Data.SqlClient
Imports System.Web
Imports Talent.Common.Utilities

''' <summary>
''' Provides the gateway to access the Data Access Layer
''' </summary>
<Serializable()> _
Public Class TalentDataAccess
    Inherits TalentBase

#Region "Class Level Fields"
    ''' <summary>
    ''' DESQLCommand Instance
    ''' </summary>
    Private _commandElements As New DESQLCommand
    Private _resultDataSet As DataSet
    Private _moduleNameForCacheDependency As String = ""
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the result data set.
    ''' </summary>
    ''' <value>The result data set.</value>
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the command elements of type DESQLCommand
    ''' </summary>
    ''' <value>The command elements.</value>
    Public Property CommandElements() As DESQLCommand
        Get
            Return _commandElements
        End Get
        Set(ByVal value As DESQLCommand)
            _commandElements = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the module name for cache dependency.
    ''' Set this value only Table/Data Objects requires caching with dependency file
    ''' </summary>
    ''' <value>
    ''' The module name for cache dependency.
    ''' </value>
    Public Property ModuleNameForCacheDependency() As String
        Get
            Return _moduleNameForCacheDependency
        End Get
        Set(ByVal value As String)
            _moduleNameForCacheDependency = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' OBSOLETE METHOD WILL REPLACE AS EARLY AS POSSIBLE
    ''' Functionality to provide the gateway to access the Data Access Layer for Talent Admin
    ''' </summary>
    ''' <returns>An error object</returns>
    Public Function TalentAdminAccess() As ErrorObj
        Dim err As New ErrorObj
        'Get the cache key from DESettings
        Dim cacheKey As String = GlobalConstants.DBACCESS_TALENT_ADMIN & Settings.CacheStringExtension
        cacheKey = cacheKey.ToLower
        'Check is exists in cache else call DB
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", GlobalConstants.DBACCESS_TALENT_ADMIN)
            Settings.ModuleName = GlobalConstants.DBACCESS_TALENT_ADMIN
            Dim DBDataAccessEntity As New DBDataAccess
            With DBDataAccessEntity
                .Settings = Settings
                .CommandElement = _commandElements
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If Settings.Cacheing Then
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If

        Return err
    End Function

    ''' <summary>
    ''' This has to be used only when get custom dictionary method is used
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SQLAccessForDictionary(ByVal destinationDatabase As DestinationDatabase) As ErrorObj
        Dim ModuleName As String = GetModuleNameByDestinationDB(destinationDatabase)
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Functionality to provide the gateway to access the Data Access Layer
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SQLAccess(ByVal destinationDatabase As DestinationDatabase) As ErrorObj
        Dim err As New ErrorObj
        err = SQLAccess(destinationDatabase, False, 0)
        Return err
    End Function
    ''' <summary>
    ''' Functionality to provide the gateway to access the Data Access Layer
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <param name="cacheing"></param>
    ''' <param name="cacheTimeMinutes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SQLAccess(ByVal destinationDatabase As DestinationDatabase, ByVal cacheing As Boolean, ByVal cacheTimeMinutes As Integer) As ErrorObj
        Dim ModuleName As String = GetModuleNameByDestinationDB(destinationDatabase)
        Dim err As New ErrorObj
        Dim cacheKey As String = Settings.CacheStringExtension
        Dim tempSettingCacheing As Boolean = Settings.Cacheing

        Settings.Cacheing = cacheing
        If Settings.Cacheing Then
            If (Settings.CacheTimeMinutes <= 0 AndAlso Settings.CacheTimeSeconds <= 0) Then
                Settings.CacheTimeMinutes = cacheTimeMinutes
            End If
        End If

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim DBDataAccessEntity As New DBDataAccess
            With DBDataAccessEntity
                .Settings = Settings
                .CommandElement = _commandElements
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If Settings.Cacheing Then
                        If Me.ModuleNameForCacheDependency.Trim.Length > 0 Then
                            Settings.ModuleName = ModuleNameForCacheDependency
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                            Settings.ModuleName = ModuleName
                        Else
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                        End If
                    End If
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        Settings.Cacheing = tempSettingCacheing
        Return err
    End Function

    ''' <summary>
    ''' Use this for Insert, Update and Delete
    ''' Functionality to provide the gateway to access the Data Access Layer with Transaction object
    ''' If any exception transaction will be rollbacked here
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <returns></returns>
    Public Function SQLAccess(ByVal destinationDatabase As DestinationDatabase, ByVal givenTransaction As SqlTransaction) As ErrorObj
        Dim ModuleName As String = GetModuleNameByDestinationDB(destinationDatabase)
        Dim err As New ErrorObj
        'Get the cache key from DESettings
        Dim cacheKey As String = ModuleName & Settings.CacheStringExtension
        cacheKey = cacheKey.ToLower
        'Check is exists in cache else call DB
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim DBDataAccessEntity As New DBDataAccess
            With DBDataAccessEntity
                .Settings = Settings
                .CommandElement = _commandElements
                err = .AccessWithTransaction(givenTransaction)
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If Settings.Cacheing Then
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                Else
                    givenTransaction.Rollback()
                    'before call this get the previous error details
                    Dim errMessage As String = err.ErrorMessage
                    err = EndTransaction(destinationDatabase, givenTransaction)
                    If err.HasError Then
                        errMessage = errMessage & ";" & err.ErrorMessage
                    End If
                    'whether end transaction gives error or not
                    'always assign err object has error
                    err.HasError = True
                    err.ErrorMessage = errMessage
                End If
            End With
        End If

        Return err
    End Function

    ''' <summary>
    ''' Begins the transaction and returns the transaction object with open state connection.
    ''' Make sure to call EndTransaction method to close transaction
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <param name="err">The err.</param>
    ''' <param name="givenIsolationLevel">The given isolation level.</param>
    ''' <returns></returns>
    Public Function BeginTransaction(ByVal destinationDatabase As DestinationDatabase, ByRef err As ErrorObj, Optional ByVal givenIsolationLevel As IsolationLevel = Nothing) As SqlTransaction
        Dim ModuleName As String = GetModuleNameByDestinationDB(destinationDatabase)
        Dim SqlTrans As SqlTransaction = Nothing
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            If (givenIsolationLevel = Nothing) Then
                SqlTrans = .BeginTransaction(destinationDatabase, err)
            Else
                SqlTrans = .BeginTransaction(destinationDatabase, err)
            End If
            If err.HasError Then
                SqlTrans = Nothing
            End If
        End With
        ''End If
        Return SqlTrans
    End Function

    ''' <summary>
    ''' Ends the transaction by passing the object to DBDataAccess
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <returns>Error Object</returns>
    Public Function EndTransaction(ByVal destinationDatabase As DestinationDatabase, ByVal givenTransaction As SqlTransaction) As ErrorObj
        Dim ModuleName As String = GetModuleNameByDestinationDB(destinationDatabase)
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            .EndTransaction(destinationDatabase, err, givenTransaction)
        End With
        Return err
    End Function

    ''' <summary>
    ''' Ends the transaction and close the reader by passing the object to DBDataAccess
    ''' </summary>
    ''' <param name="destinationDatabase"></param>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <param name="readerToClose">The reader to close.</param>
    ''' <returns>Error Object</returns>
    Public Function EndTransaction(ByVal destinationDatabase As DestinationDatabase, ByVal givenTransaction As SqlTransaction, ByVal readerToClose As SqlDataReader) As ErrorObj
        Dim ModuleName As String = GetModuleNameByDestinationDB(destinationDatabase)
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            .EndTransaction(err, givenTransaction, readerToClose)
        End With
        Return err
    End Function

    ''' <summary>
    ''' Functionality to provide the gateway to access the Data Access Layer for Talent Definitions
    ''' </summary>
    ''' <returns>An error object</returns>
    Public Function TalentDefinitionsAccess() As ErrorObj
        Dim err As New ErrorObj
        'Get the cache key from DESettings
        Dim cacheKey As String = Settings.ModuleName & Settings.CacheStringExtension
        cacheKey = cacheKey.ToLower
        'Check is exists in cache else call DB
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
            Dim DBDataAccessEntity As New DBDataAccess
            With DBDataAccessEntity
                .Settings = Settings
                .CommandElement = _commandElements
                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If Settings.Cacheing Then
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If

        Return err

    End Function


#Region "Obsolete Method"
    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Use SQLAccess(destinationDatabase, cacheing, cacheTimeMinutes)
    ''' Deprecated functionality to provide the gateway to access the Data Access Layer
    ''' </summary>
    ''' <returns></returns>
    Public Function SQLAccess() As ErrorObj
        Dim err As New ErrorObj
        'Get the cache key from DESettings
        Dim cacheKey As String = GlobalConstants.DBACCESS_SQL & Settings.CacheStringExtension
        cacheKey = cacheKey.ToLower
        'Check is exists in cache else call DB
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", GlobalConstants.DBACCESS_SQL)
            Settings.ModuleName = GlobalConstants.DBACCESS_SQL
            Dim DBDataAccessEntity As New DBDataAccess
            With DBDataAccessEntity
                .Settings = Settings
                .CommandElement = _commandElements
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If Settings.Cacheing Then
                        If Me.ModuleNameForCacheDependency.Trim.Length > 0 Then
                            Settings.ModuleName = ModuleNameForCacheDependency
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                            Settings.ModuleName = GlobalConstants.DBACCESS_SQL
                        Else
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                        End If
                    End If
                ElseIf Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If

        Return err
    End Function

    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Use this SQLAccess(ByVal destinationDatabase As DestinationDatabase, ByVal givenTransaction As SqlTransaction) 
    ''' Use this for Insert, Update and Delete
    ''' Functionality to provide the gateway to access the Data Access Layer with Transaction object
    ''' If any exception transaction will be rollbacked here
    ''' </summary>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <returns></returns>
    Public Function SQLAccess(ByVal givenTransaction As SqlTransaction) As ErrorObj
        Dim err As New ErrorObj
        'Get the cache key from DESettings
        Dim cacheKey As String = GlobalConstants.DBACCESS_SQL & Settings.CacheStringExtension
        cacheKey = cacheKey.ToLower
        'Check is exists in cache else call DB
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", GlobalConstants.DBACCESS_SQL)
            Settings.ModuleName = GlobalConstants.DBACCESS_SQL
            Dim DBDataAccessEntity As New DBDataAccess
            With DBDataAccessEntity
                .Settings = Settings
                .CommandElement = _commandElements
                err = .AccessWithTransaction(givenTransaction)
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If Settings.Cacheing Then
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                Else
                    givenTransaction.Rollback()
                    'before call this get the previous error details
                    Dim errMessage As String = err.ErrorMessage
                    err = EndTransaction(givenTransaction)
                    If err.HasError Then
                        errMessage = errMessage & ";" & err.ErrorMessage
                    End If
                    'whether end transaction gives error or not
                    'always assign err object has error
                    err.HasError = True
                    err.ErrorMessage = errMessage
                End If
            End With
        End If

        Return err
    End Function

    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Begins the transaction and returns the transaction object with open state connection.
    ''' Make sure to call EndTransaction method to close transaction
    ''' </summary>
    ''' <param name="err">The err.</param>
    ''' <param name="givenIsolationLevel">The given isolation level.</param>
    ''' <returns></returns>
    Public Function BeginTransaction(ByRef err As ErrorObj, Optional ByVal givenIsolationLevel As IsolationLevel = Nothing) As SqlTransaction
        Const ModuleName As String = "BeginTransaction"
        Dim SqlTrans As SqlTransaction = Nothing
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            If (givenIsolationLevel = Nothing) Then
                SqlTrans = .BeginTransaction(err)
            Else
                SqlTrans = .BeginTransaction(err)
            End If
            If err.HasError Then
                SqlTrans = Nothing
            End If
        End With
        ''End If
        Return SqlTrans
    End Function

    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Ends the transaction by passing the object to DBDataAccess
    ''' </summary>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <returns>Error Object</returns>
    Public Function EndTransaction(ByVal givenTransaction As SqlTransaction) As ErrorObj
        Const ModuleName As String = "EndTransaction"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            .EndTransaction(err, givenTransaction)
        End With
        Return err
    End Function

    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Ends the transaction and close the reader by passing the object to DBDataAccess
    ''' </summary>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <param name="readerToClose">The reader to close.</param>
    ''' <returns>Error Object</returns>
    Public Function EndTransaction(ByVal givenTransaction As SqlTransaction, ByVal readerToClose As SqlDataReader) As ErrorObj
        Const ModuleName As String = "EndTransaction"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim DBDataAccessEntity As New DBDataAccess
        With DBDataAccessEntity
            .Settings = Settings
            .CommandElement = _commandElements
            .EndTransaction(err, givenTransaction, readerToClose)
        End With
        Return err
    End Function

#End Region

#End Region

#Region "Private Methods"
    Private Function GetModuleNameByDestinationDB(ByVal destinationDatabase As DestinationDatabase) As String
        Dim moduleName As String = GlobalConstants.DBACCESS_SQL
        If destinationDatabase = Common.DestinationDatabase.SQL2005 Then
            moduleName = GlobalConstants.DBACCESS_SQL
        ElseIf destinationDatabase = Common.DestinationDatabase.TALENT_ADMIN Then
            moduleName = GlobalConstants.DBACCESS_TALENT_ADMIN
        ElseIf destinationDatabase = Common.DestinationDatabase.TALENT_DEFINITION Then
            moduleName = GlobalConstants.DBACCESS_TALENT_DEFINITIONS
        End If
        Return moduleName
    End Function
#End Region

End Class
