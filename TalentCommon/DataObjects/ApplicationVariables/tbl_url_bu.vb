Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_url_bu based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_url_bu
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        Private _dicAllBusinessUnitURLDeviceList As Dictionary(Of String, DEBusinessUnitURLDevice) = Nothing

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_url_bu"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_url_bu" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Copies the records from one business unit to another business unit inside the table tbl_url_bu
        ''' irrespective of whether destination business exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromBusinessUnit">From business unit.</param>
        ''' <param name="toBusinessUnit">To business unit.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess


            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO TBL_URL_BU (BUSINESS_UNIT, APPLICATION, BU_GROUP, URL) " & _
                        "SELECT @ToBusinessUnit As BUSINESS_UNIT, APPLICATION, BU_GROUP, URL " & _
                        "FROM TBL_URL_BU " & _
                        "WHERE BUSINESS_UNIT = @FromBusinessUnit"

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromBusinessUnit", fromBusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToBusinessUnit", toBusinessUnit))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Gets all the business unit records from the table tbl_url_bu
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAll")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_URL_BU"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable

        End Function

        Public Function GetBUByURL(ByVal url As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetBUByURL_" & url)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@url", url))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_url_bu] WITH (NOLOCK) WHERE URL = @url AND (APPLICATION = 'eBusiness' OR APPLICATION='ecommerce' OR APPLICATION = '' OR APPLICATION IS NULL)"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable
        End Function

        Public Function GetDistinctEBusinessBUs(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDistinctEBusinessBUs")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT DISTINCT [BUSINESS_UNIT] FROM TBL_URL_BU WHERE APPLICATION = 'EBusiness'"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Gets the business unit records based on the given business unit.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBU(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBU")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_URL_BU Where BUSINESS_UNIT=@BusinessUnitName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Deletes the specified business unit.
        ''' </summary>
        ''' <param name="businessUnitToDelete">The business unit to delete.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Delete(ByVal businessUnitToDelete As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_URL_BU " & _
                "WHERE BUSINESS_UNIT=@BusinessUnitToDelete "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitToDelete", businessUnitToDelete))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        ''' <summary>
        ''' Insert the specified business unit and returns no of affected rows.
        ''' Passing Nothing to any parameter is equal to DBNull.Value
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="application">The application.</param>
        ''' <param name="buGroup">The bu group.</param>
        ''' <param name="url">The URL.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function Insert(ByVal businessUnit As String, ByVal application As String, ByVal buGroup As String, ByVal url As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT TBL_URL_BU " & _
                "(BUSINESS_UNIT, APPLICATION, BU_GROUP, URL) VALUES " & _
                "(@BusinessUnit, @Application, @BuGroup, @URL) "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Application", application))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BuGroup", buGroup))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@URL", url))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        '''<summary>
        ''' Update the specified business unit and returns no of affected rows.
        ''' Passing Nothing to any parameter is equal to DBNull.Value
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="url">The URL.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function Update(ByVal businessUnit As String, ByVal url As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE TBL_URL_BU " & _
                " SET URL = @URL " & _
                " WHERE BUSINESS_UNIT = @BusinessUnit"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@URL", url))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        ''' <summary>
        ''' Gets all business unit URL device.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function GetAllBusinessUnitURLDevice(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllBusinessUnitURLDeviceList")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_url_bu WHERE DEVICE_TYPE IS NOT NULL AND DEVICE_TYPE <> '' AND URL_GROUP IS NOT NULL AND URL_GROUP <>'' "

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Gets all business unit URL device list.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>Dictionary Object with key as URL_GROUP and DEVICE_TYPE</returns>
        Public Function GetAllBusinessUnitURLDeviceList(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Dictionary(Of String, DEBusinessUnitURLDevice)
            Dim dicBUURLDeviceList As New Dictionary(Of String, DEBusinessUnitURLDevice)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllBusinessUnitURLDeviceList")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT ub.*, emdb.VALUE as QUEUE_URL FROM tbl_url_bu ub " & _
                                " JOIN tbl_ecommerce_module_defaults_bu emdb ON emdb.BUSINESS_UNIT=ub.BUSINESS_UNIT AND emdb.DEFAULT_NAME='NOISE_URL' " & _
                                " WHERE ub.DEVICE_TYPE IS NOT NULL AND ub.DEVICE_TYPE <> '' " & _
                                " AND ub.URL_GROUP IS NOT NULL AND ub.URL_GROUP <>'' "

                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetAllBusinessUnitURLDeviceList")
                If dicObject IsNot Nothing Then
                    dicBUURLDeviceList = CType(dicObject, Dictionary(Of String, DEBusinessUnitURLDevice))
                End If
            Catch ex As Exception
                Throw
            End Try
            Return dicBUURLDeviceList
        End Function

        ''' <summary>
        ''' Populates the custom dictionary entities. Overridden method called from DBObjectBase
        ''' </summary>
        ''' <param name="dtSourceToPopulate">The dt source to populate.</param>
        Protected Overrides Sub PopulateCustomDictionaryEntities(ByVal dtSourceToPopulate As System.Data.DataTable, ByVal callingModuleName As String)
            If dtSourceToPopulate IsNot Nothing AndAlso dtSourceToPopulate.Rows.Count > 0 Then
                Dim dicBUURLDeviceList As New Dictionary(Of String, DEBusinessUnitURLDevice)
                For rowIndex As Integer = 0 To dtSourceToPopulate.Rows.Count - 1
                    Dim keyString As String = (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("URL_GROUP")) & Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("DEVICE_TYPE"))).Trim
                    If keyString.Length > 0 Then
                        Dim tempBUURLDeviceEntity As New DEBusinessUnitURLDevice
                        tempBUURLDeviceEntity.BusinessUnit = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("BUSINESS_UNIT")).Trim
                        tempBUURLDeviceEntity.BusinessUnitContent = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("BUSINESS_UNIT_FOR_CONTENT")).Trim
                        tempBUURLDeviceEntity.BusinessUnitGroup = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("BU_GROUP")).Trim
                        tempBUURLDeviceEntity.DeviceType = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("DEVICE_TYPE")).Trim
                        tempBUURLDeviceEntity.QueueURL = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("QUEUE_URL")).Trim
                        tempBUURLDeviceEntity.URL = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("URL")).Trim
                        tempBUURLDeviceEntity.URLGroup = Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("URL_GROUP")).Trim
                        If Not dicBUURLDeviceList.ContainsKey(keyString) Then
                            dicBUURLDeviceList.Add(keyString, tempBUURLDeviceEntity)
                        End If
                        tempBUURLDeviceEntity = Nothing
                    End If
                Next
                If dicBUURLDeviceList.Count > 0 Then
                    MyBase.CustomObject = dicBUURLDeviceList
                End If
            End If
        End Sub

        ''' <summary>
        ''' Gets the tbl_url_bu records based on the given url_bu_id.
        ''' </summary>
        ''' <param name="ID">The URL_BU_ID</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>false</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByID(ByVal ID As String, Optional ByVal cacheing As Boolean = False, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(ID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_URL_BU Where URL_BU_ID=@ID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", ID))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable

        End Function

        '''<summary>
        ''' Update the specified business DISABLED flag and returns no of affected rows.
        ''' Passing Nothing to any parameter is equal to DBNull.Value
        ''' </summary>
        ''' <param name="ID">The ID</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function UpdateDISABLED(ByVal ID As String, ByVal DISABLED As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE TBL_URL_BU " & _
                " SET DISABLED = @DISABLED " & _
                " WHERE URL_BU_ID = @ID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DISABLED", DISABLED))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", ID))

            'Execute
            Dim err As New ErrorObj
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

        '''<summary>
        ''' Gets the tbl_url_bu records 
        ''' Passing Nothing to any parameter is equal to DBNull.Value
        ''' </summary>
        ''' <param name="sDeviceType">The Device Type</param>
        ''' <param name="sURLGroup">The URL Group</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetChangeToBUs(ByVal sDeviceType As String, ByVal sURLGroup As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetChangeToBUs")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_URL_BU " & _
                                                                    "WHERE CHANGETO_ALLOWED=@BOOL " & _
                                                                    "AND DEVICE_TYPE=@DEVICE_TYPE " & _
                                                                    "AND URL_GROUP=@URL_GROUP"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BOOL", "True"))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DEVICE_TYPE", sDeviceType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@URL_GROUP", sURLGroup))


                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return outputDataTable

        End Function
#End Region

    End Class

End Namespace

