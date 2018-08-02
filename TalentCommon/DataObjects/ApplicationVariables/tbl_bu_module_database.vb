Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_bu_module_database based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_bu_module_database
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_bu_module_database"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_bu_module_database" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

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
            Dim sqlStatement As String = "DELETE TBL_BU_MODULE_DATABASE " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_bu_module_database
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
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO TBL_BU_MODULE_DATABASE (BUSINESS_UNIT, PARTNER, APPLICATION, " & _
                        "MODULE, DESTINATION_DATABASE, SECTION, SUB_SECTION, TIMEOUT)" & _
                        "SELECT @ToBusinessUnit As BUSINESS_UNIT, PARTNER, APPLICATION, MODULE, " & _
                        "DESTINATION_DATABASE, SECTION, SUB_SECTION, TIMEOUT FROM TBL_BU_MODULE_DATABASE " & _
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
        ''' Get the destination database based on the module
        ''' </summary>
        ''' <param name="ModuleName">The module name</param>
        ''' <param name="BusinessUnit">The business unit</param>
        ''' <param name="Partner">The partner</param>
        ''' <param name="Application">The application</param>
        ''' <param name="cacheing">The caching parameter option</param>
        ''' <param name="cacheTimeMinutes">The cache time</param>
        ''' <returns>The database version table</returns>
        ''' <remarks></remarks>
        Public Function GetByModuleName(ByVal ModuleName As String, ByVal BusinessUnit As String, ByVal Partner As String, ByVal Application As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByModuleName")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim destinationDatabase As String = String.Empty

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(ModuleName) & BusinessUnit & Partner & Application
                talentSqlAccessDetail.CommandElements.CommandType = CommandType.StoredProcedure
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "usp_ModuleDatabase_SelectBUPartnerApplicationModule"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_BusinessUnit", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Partner", Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Application", Application))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pa_Module", ModuleName))
                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    destinationDatabase = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0).Item("DestinationDatabase")
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return destinationDatabase

        End Function

#End Region

    End Class
End Namespace
