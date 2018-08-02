Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports System.Web

#Region "Enumerations"

''' <summary>
''' Provides the status types for the STATUS column in the table tbl_log_header
''' </summary>
Public Enum ActivityStatusEnum
    ''' <summary>
    ''' Start status
    ''' </summary>
    STARTED
    ''' <summary>
    ''' In progress status
    ''' </summary>
    INPROGRESS
    ''' <summary>
    ''' Success
    ''' </summary>
    SUCCESS
    ''' <summary>
    ''' Failed
    ''' </summary>
    FAILED
End Enum

''' <summary>
''' Provides the status types for the STATUS column in the table tbl_data_transfer_status
''' </summary>
Public Enum DataTransferStatusEnum
    ''' <summary>
    ''' Clearing work table (on insert)
    ''' </summary>
    CLEARING_WORK_TABLE
    ''' <summary>
    ''' In progress status (extracting - on update)
    ''' </summary>
    EXTRACTING_FROM_ISERIES
    ''' <summary>
    ''' Updating Tables (on update)
    ''' </summary>
    UPDATING_SQL_TABLES
    ''' <summary>
    ''' Success/Finished (on update)
    ''' </summary>
    FINISHED
    ''' <summary>
    ''' Failed - exception occurred (on update)
    ''' </summary>
    FAILED
End Enum

#End Region

''' <summary>
''' Provides the functionalities which are common across the data objects
''' </summary>
<Serializable()> _
Public MustInherit Class DBObjectBase

#Region "Class Level Fields"
    Private Const CLASSNAME As String = "DBObjectBase"
    ''' <summary>
    ''' Common Cache Key Prefix Start for Data Objects
    ''' </summary>
    Protected Const CACHEKEY_PREFIX As String = "DataObjects_"
    Private _talentLogger As TalentLogging = Nothing
    Private _customObject As Object = Nothing
    Private _agentProfile As DEAgent = Nothing
    Private _resultDataSet As DataSet = Nothing
#End Region

    Sub New()
        _agentProfile = New DEAgent
    End Sub
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
    Protected ReadOnly Property TalentLogger() As TalentLogging
        Get
            If _talentLogger Is Nothing Then
                _talentLogger = New TalentLogging
            End If
            Return _talentLogger
        End Get
    End Property

    Protected WriteOnly Property CustomObject() As Object
        Set(ByVal value As Object)
            _customObject = value
        End Set
    End Property

    Protected WriteOnly Property AgentProfile() As DEAgent
        Set(ByVal value As DEAgent)
            _agentProfile = value
        End Set
    End Property


#Region "Protected Methods"
    ''' <summary>
    ''' Constructs the parameter using the DESQLParamater instance
    ''' </summary>
    ''' <param name="paramName">Name of the parameter.</param>
    ''' <param name="paramValue">The parameter value.</param>
    ''' <param name="paramType">Type of the parameter. Default SqlDbType.NVarchar</param>
    ''' <param name="paramDirection">The parameter direction. Default ParameterDirection.Input</param>
    ''' <returns>DESQLParameter instance with details</returns>
    Protected Function ConstructParameter(ByVal paramName As String, ByVal paramValue As String, Optional ByVal paramType As SqlDbType = SqlDbType.NVarChar, Optional ByVal paramDirection As ParameterDirection = ParameterDirection.Input) As DESQLParameter
        Dim parameterPiece As New DESQLParameter
        parameterPiece.ParamDirection = paramDirection
        parameterPiece.ParamName = paramName
        parameterPiece.ParamType = paramType
        parameterPiece.ParamValue = paramValue
        Return parameterPiece
    End Function

    ''' <summary>
    ''' Change the given string to upper case
    ''' </summary>
    ''' <param name="stringToUpper">The string to upper.</param>
    ''' <returns>if nothing returns empty string otherwise upper case string</returns>
    Protected Function ToUpper(ByVal stringToUpper As String) As String
        If (Not (stringToUpper = Nothing)) And (stringToUpper.Trim().Length > 0) Then
            stringToUpper = stringToUpper.ToUpper()
        Else
            stringToUpper = String.Empty
        End If
        Return stringToUpper
    End Function

    ''' <summary>
    ''' Construct and return cache key prefix by including common prefix, given class name
    ''' and given method name.
    ''' </summary>
    ''' <param name="className">Name of the class.</param>
    ''' <param name="methodName">Name of the method.</param>
    ''' <returns></returns>
    Protected Function GetCacheKeyPrefix(ByVal className As String, ByVal methodName As String) As String
        Return CACHEKEY_PREFIX & className & methodName
    End Function

    ''' <summary>
    ''' Replaces the single quote to double single quote 
    ''' </summary>
    ''' <param name="stringToReplace">The string to replace.</param>
    ''' <returns>String</returns>
    Protected Function ReplaceSingleQuote(ByVal stringToReplace As String) As String
        If ((stringToReplace <> Nothing) And (stringToReplace.Length > 0)) Then
            stringToReplace = stringToReplace.Replace("'", "''")
        End If
        Return stringToReplace
    End Function

    ''' <summary>
    ''' Converts the Generic List to Xml String based on the given Type
    ''' </summary>
    ''' <typeparam name="T">Type of Object</typeparam>
    ''' <param name="genericListObject">Generic List of the given type.</param>
    ''' <returns>xml string</returns>
    Protected Function GenericListToXmlString(Of T)(ByVal genericListObject As T) As String
        Dim constructedString As String = String.Empty
        Try
            Dim genericSerialiser As New XmlSerializer(GetType(T))
            Dim memoryStreamer As New MemoryStream
            Dim xmlTextWriter As New XmlTextWriter(memoryStreamer, Text.Encoding.Unicode)
            genericSerialiser.Serialize(xmlTextWriter, genericListObject)
            memoryStreamer = xmlTextWriter.BaseStream
            Dim encoding As New UnicodeEncoding()
            constructedString = encoding.GetString(memoryStreamer.ToArray()).Trim()
            genericSerialiser = Nothing
            memoryStreamer = Nothing
        Catch ex As Exception
            Throw
        End Try
        Return constructedString
    End Function

    ''' <summary>
    ''' Remove the given cache key from the cache
    ''' </summary>
    ''' <param name="cacheKey">cache key string</param>
    Protected Sub RemoveKeyFromCache(ByVal cacheKey As String)
        Dim tBase As New TalentBase
        tBase.RemoveItemFromCache(cacheKey)
    End Sub

    Protected Function GetCustomDictionaryEntities(Of T)(ByVal destinationDatabase As DestinationDatabase, ByVal cacheing As Boolean, ByVal cacheTimeMinutes As Integer, ByVal talentSqlAccessDetail As TalentDataAccess, ByVal callingModuleName As String) As T
        Dim customDicObject As T = Nothing
        Dim tempSettingsCacheing As Boolean = False
        Try
            Dim cacheKey As String = talentSqlAccessDetail.Settings.CacheStringExtension
            If talentSqlAccessDetail.Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                customDicObject = CType(HttpContext.Current.Cache.Item(cacheKey), T)
            Else
                tempSettingsCacheing = talentSqlAccessDetail.Settings.Cacheing
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                If talentSqlAccessDetail.Settings.Cacheing Then
                    If (talentSqlAccessDetail.Settings.CacheTimeMinutes <= 0 AndAlso talentSqlAccessDetail.Settings.CacheTimeSeconds <= 0) Then
                        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                    End If
                End If
                Dim outputDataTable As DataTable = Nothing
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccessForDictionary(destinationDatabase)
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                    If outputDataTable IsNot Nothing AndAlso outputDataTable.Rows.Count > 0 Then
                        PopulateCustomDictionaryEntities(outputDataTable, callingModuleName)
                        customDicObject = _customObject
                        If talentSqlAccessDetail.Settings.Cacheing Then
                            If talentSqlAccessDetail.ModuleNameForCacheDependency.Trim.Length > 0 Then
                                Dim moduleName As String = talentSqlAccessDetail.Settings.ModuleName
                                talentSqlAccessDetail.Settings.ModuleName = talentSqlAccessDetail.ModuleNameForCacheDependency
                                talentSqlAccessDetail.AddItemToCache(cacheKey, customDicObject, talentSqlAccessDetail.Settings)
                                talentSqlAccessDetail.Settings.ModuleName = moduleName
                            Else
                                talentSqlAccessDetail.AddItemToCache(cacheKey, customDicObject, talentSqlAccessDetail.Settings)
                            End If
                        End If
                    ElseIf talentSqlAccessDetail.Settings.Cacheing Then
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                End If
            End If
        Catch ex As Exception
            TalentLogger.FrontEndConnectionString = talentSqlAccessDetail.Settings.FrontEndConnectionString
            TalentLogger.Logging(CLASSNAME, "GetCustomDictionaryEntities", "Exception Occured: Calling Module Name is " & callingModuleName, "TCDBOB-001", ex, LogTypeConstants.TCDBDATAOBJECTSLOG, talentSqlAccessDetail.Settings.BusinessUnit, talentSqlAccessDetail.Settings.Partner, talentSqlAccessDetail.Settings.LoginId)
            Throw
        Finally
            talentSqlAccessDetail.Settings.Cacheing = tempSettingsCacheing
            talentSqlAccessDetail = Nothing
        End Try
        Return customDicObject
    End Function
    
    ''' <summary>
    ''' OBSOLETE METHOD
    ''' Use this GetCustomDictionaryEntities(Of T)(ByVal destinationDatabase As DestinationDatabase, ByVal cacheing As Boolean, ByVal cacheTimeMinutes As Integer, ByVal talentSqlAccessDetail As TalentDataAccess, ByVal callingModuleName As String) As T
    ''' </summary>
    ''' <param name="talentSqlAccessDetail"></param>
    ''' <param name="callingModuleName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function GetCustomDictionaryEntities(ByVal talentSqlAccessDetail As TalentDataAccess, ByVal callingModuleName As String) As Object
        Dim customDicObject As Object = Nothing
        Try
            Dim cacheKey As String = "GetCustomDictionaryEntities" & talentSqlAccessDetail.Settings.CacheStringExtension
            cacheKey = cacheKey.ToLower
            If talentSqlAccessDetail.Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                customDicObject = CType(HttpContext.Current.Cache.Item(cacheKey), Object)
            Else
                Dim outputDataTable As DataTable = Nothing
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()

                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                    If outputDataTable IsNot Nothing AndAlso outputDataTable.Rows.Count > 0 Then
                        PopulateCustomDictionaryEntities(outputDataTable, callingModuleName)
                        customDicObject = _customObject
                        If talentSqlAccessDetail.Settings.Cacheing Then
                            talentSqlAccessDetail.AddItemToCache(cacheKey, customDicObject, talentSqlAccessDetail.Settings)
                        End If
                    ElseIf talentSqlAccessDetail.Settings.Cacheing Then
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                ElseIf talentSqlAccessDetail.Settings.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End If
        Catch ex As Exception
            TalentLogger.FrontEndConnectionString = talentSqlAccessDetail.Settings.FrontEndConnectionString
            TalentLogger.Logging(CLASSNAME, "GetCustomDictionaryEntities", "Exception Occured: Calling Module Name is " & callingModuleName, "TCDBOB-001", ex, LogTypeConstants.TCDBDATAOBJECTSLOG, talentSqlAccessDetail.Settings.BusinessUnit, talentSqlAccessDetail.Settings.Partner, talentSqlAccessDetail.Settings.LoginId)
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        Return customDicObject
    End Function

    ''' <summary>
    ''' OBSOLETE METHOD
    ''' Use this GetCustomDictionaryEntities(Of T)(ByVal destinationDatabase As DestinationDatabase, ByVal cacheing As Boolean, ByVal cacheTimeMinutes As Integer, ByVal talentSqlAccessDetail As TalentDataAccess, ByVal callingModuleName As String) As T
    ''' </summary>
    ''' <param name="sourceTableForDic"></param>
    ''' <param name="settingsEntity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function GetCustomDictionaryEntities(ByVal sourceTableForDic As DataTable, ByVal settingsEntity As DESettings) As Object
        Dim customDicObject As Object = Nothing
        Dim cacheKey As String = "GetCustomDictionaryEntities" & settingsEntity.CacheStringExtension
        cacheKey = cacheKey.ToLower
        If settingsEntity IsNot Nothing AndAlso settingsEntity.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            customDicObject = CType(HttpContext.Current.Cache.Item(cacheKey), Object)
        Else
            Try
                If sourceTableForDic IsNot Nothing AndAlso sourceTableForDic.Rows.Count > 0 Then
                    PopulateCustomDictionaryEntities(sourceTableForDic, settingsEntity.ModuleName)
                    customDicObject = _customObject
                    If settingsEntity.Cacheing Then
                        Dim talentSqlAccessDetail As New TalentDataAccess
                        talentSqlAccessDetail.AddItemToCache(cacheKey, customDicObject, settingsEntity)
                        talentSqlAccessDetail = Nothing
                    End If
                ElseIf settingsEntity.Cacheing Then
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            Catch ex As Exception
                TalentLogger.FrontEndConnectionString = settingsEntity.FrontEndConnectionString
                TalentLogger.Logging(CLASSNAME, "GetCustomDictionaryEntities", "Exception Occured: Calling Module Name is " & settingsEntity.ModuleName, "TCDBOB-002", ex, LogTypeConstants.TCDBDATAOBJECTSLOG, settingsEntity.BusinessUnit, settingsEntity.Partner, settingsEntity.LoginId)
                Throw
            End Try
        End If
        Return customDicObject
    End Function

    Protected Overridable Sub PopulateCustomDictionaryEntities(ByVal dtSourceToPopulate As DataTable, ByVal callingModuleName As String)

    End Sub

    Protected Function TryGetFromCache(Of T)(ByRef isCacheNotFound As Boolean, ByVal cacheing As Boolean, ByVal cacheKey As String) As T
        If cacheing Then
            If Utilities.IsCacheActive Then
                If Not HttpContext.Current.Cache.Item(cacheKey) Is Nothing Then
                    isCacheNotFound = False
                    Return CType(HttpContext.Current.Cache.Item(cacheKey), T)
                End If
            End If
        End If
        isCacheNotFound = True
        Return Nothing
    End Function

#End Region

End Class
