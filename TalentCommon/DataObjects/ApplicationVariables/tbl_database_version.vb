Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_database_version based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_database_version
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_database_version"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_database_version" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the dataase version record by the given destination database name
        ''' </summary>
        ''' <param name="destinationDatabase">The destination database name</param>
        ''' <param name="cacheing">The caching parameter option</param>
        ''' <param name="cacheTimeMinutes">The cache time</param>
        ''' <returns>The database version table</returns>
        ''' <remarks></remarks>
        Public Function GetByDestinationDatabase(ByVal destinationDatabase As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByDestinationDatabase")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(destinationDatabase)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_DATABASE_VERSION WHERE DESTINATION_DATABASE=@DestinationDatabase"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DestinationDatabase", destinationDatabase))

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
        ''' Get the Talent Admin connection string
        ''' </summary>
        ''' <param name="cacheing">The caching parameter option</param>
        ''' <param name="cacheTimeMinutes">The cache time</param>
        ''' <returns>Talent Admin connection string</returns>
        ''' <remarks></remarks>
        Public Function TalentAdminDatabaseConnectionString(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "TalentAdminDatabaseConnectionString")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim talentAdminConnectionString As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [CONNECTION_STRING] FROM [TBL_DATABASE_VERSION] WHERE [DESTINATION_DATABASE]='TalentAdmin'"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        talentAdminConnectionString = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)("CONNECTION_STRING").ToString()
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return talentAdminConnectionString
        End Function

        ''' <summary>
        ''' Get the TalentDefinitions connection string
        ''' </summary>
        ''' <param name="cacheing">The caching parameter option</param>
        ''' <param name="cacheTimeMinutes">The cache time</param>
        ''' <returns>TalentDefinitions connection string</returns>
        ''' <remarks></remarks>
        Public Function TalentDefinitionsDatabaseConnectionString(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "TalentDefinitionsDatabaseConnectionString")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim talentDefinitionsConnectionString As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [CONNECTION_STRING] FROM [TBL_DATABASE_VERSION] WHERE [DESTINATION_DATABASE]='TalentDefinitions'"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        talentDefinitionsConnectionString = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)("CONNECTION_STRING").ToString()
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return talentDefinitionsConnectionString
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
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_DATABASE_VERSION " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_database_version
        ''' irrespective of whether destination business exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromBusinessUnit">From business unit.</param>
        ''' <param name="toBusinessUnit">To business unit.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

                Dim sqlStatement As String = "INSERT INTO TBL_DATABASE_VERSION (BUSINESS_UNIT, PARTNER, DESTINATION_DATABASE, " & _
                            "DATABASE_VERSION, DATABASE_TYPE1, DATABASE_TYPE2, CONNECTION_STRING, ENCRYPTION_KEY_KEY) " & _
                            "SELECT @ToBusinessUnit As BUSINESS_UNIT, PARTNER, DESTINATION_DATABASE, DATABASE_VERSION, " & _
                            "DATABASE_TYPE1, DATABASE_TYPE2, CONNECTION_STRING, ENCRYPTION_KEY_KEY FROM TBL_DATABASE_VERSION " & _
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
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

#End Region

    End Class

End Namespace

