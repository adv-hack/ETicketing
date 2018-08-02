Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_delivery_unavailable_dates based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_delivery_unavailable_dates
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_delivery_unavailable_dates"

#End Region

#Region "Constructors"

        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_delivery_unavailable_dates" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get all the records from tbl_delivery_unavailable_dates
        ''' </summary>
        ''' <param name="cacheing">Optional boolean value to enable caching, default true</param>
        ''' <param name="cacheTimeMinutes">Option integer value to represent cache time, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllUnavailableDates(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllUnavailableDates")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_delivery_unavailable_dates]"

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
        ''' Get records from tbl_delivery_unavailable_dates by carrier and date
        ''' </summary>
        ''' <param name="carriercode">Carrier code</param>
        ''' <param name="dtdate">date</param>
        ''' <param name="cacheing">Optional boolean value to enable caching, default true</param>
        ''' <param name="cacheTimeMinutes">Option integer value to represent cache time, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetUnavailableDatesByCarrierDate(ByVal carrierCode As String, ByVal dtDate As Date, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUnavailableDatesByCarrierDate" & carrierCode & dtDate)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@carrierCode", carrierCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@date", dtDate, SqlDbType.SmallDateTime))

                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_delivery_unavailable_dates] where CARRIER_CODE = @carrierCode and DATE = @date"

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
        ''' Inserts the new date into tbl_delivery_unavailable_dates
        ''' </summary>
        ''' <param name="dateToAdd">The date to add as a date format value</param>
        ''' <returns>The number of rows affected</returns>
        ''' <remarks></remarks>
        Public Function AddDate(ByVal dateToAdd As Date, ByVal carrierCode As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "INSERT INTO [tbl_delivery_unavailable_dates] (CARRIER_CODE,DATE) VALUES (@carrier_code, @Date)"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@carrier_code", carrierCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Date", dateToAdd, Global.System.Data.SqlDbType.SmallDateTime))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Deletes a record from tbl_delivery_unavailable_dates based on the given ID
        ''' </summary>
        ''' <param name="idToUseToDelete">The id of the record to delete</param>
        ''' <returns>The number of rows affected</returns>
        ''' <remarks></remarks>
        Public Function DeleteDateById(ByVal idToUseToDelete As Integer) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_delivery_unavailable_dates] WHERE ID = @Id"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Id", idToUseToDelete))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

#End Region

    End Class

End Namespace