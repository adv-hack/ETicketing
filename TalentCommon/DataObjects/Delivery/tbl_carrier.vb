Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_carrier based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_carrier
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_carrier"

#End Region

#Region "Constructors"

        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_carrier" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the carrier records from the table tbl_carrier
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
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_CARRIER"

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
        ''' Gets the carier records based on the given carrier.
        ''' </summary>
        ''' <param name="carrier">The carrier.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByCarrier(ByVal carrier As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByCarrier")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(carrier)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_CARRIER Where CARRIER_CODE=@CarrierCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CarrierCode", carrier))

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
        ''' Gets the specified carrier record from tbl_carrier by the given record id
        ''' </summary>
        ''' <param name="carrierId">The given record Id</param>
        ''' <param name="cacheing">Optional boolean value to enable caching, default true</param>
        ''' <param name="cacheTimeMinutes">Option integer value to represent cache time, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetCarrierRecordById(ByVal carrierId As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetCarrierRecordById")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_carrier] WHERE ID = @Id"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Id", carrierId))

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
        ''' Update the specified tbl_carrier record based on the given data and record id
        ''' </summary>
        ''' <param name="carrierId">The unique record id</param>
        ''' <param name="carrierCode">The carrier code string</param>
        ''' <param name="installationAvailable">The boolean value to indicate whether or not installation is available</param>
        ''' <param name="collectOldAvailable">The boolean value to indicate whether or not collection is available</param>
        ''' <param name="deliverMonday">Can deliver on Monday</param>
        ''' <param name="deliverTuesday">Can deliver on Tuesday</param>
        ''' <param name="deliverWednesday">Can deliver on Wednesday</param>
        ''' <param name="deliverThursday">Can deliver on Thursday</param>
        ''' <param name="deliverFriday">Can deliver on Friday</param>
        ''' <param name="deliverSaturday">Can deliver on Saturday</param>
        ''' <param name="deliverSunday">Can deliver on Sunday</param>
        ''' <returns>Number of records affected</returns>
        ''' <remarks></remarks>
        Public Function UpdateCarrierRecordById(ByVal carrierId As Integer, ByVal carrierCode As String, ByVal installationAvailable As Boolean, ByVal collectOldAvailable As Boolean, _
                                                ByVal deliverMonday As Boolean, ByVal deliverTuesday As Boolean, ByVal deliverWednesday As Boolean, ByVal deliverThursday As Boolean, _
                                                ByVal deliverFriday As Boolean, ByVal deliverSaturday As Boolean, ByVal deliverSunday As Boolean) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_carrier] SET CARRIER_CODE = @CarrierCode, INSTALLATION_AVAILABLE = @InstallationAvailable, ")
            sqlStatement.Append("COLLECT_OLD_AVAILABLE = @CollectOldAvailable, DELIVER_MONDAY = @DeliverMonday, DELIVER_TUESDAY = @DeliverTuesday, ")
            sqlStatement.Append("DELIVER_WEDNESDAY = @DeliverWednesday, DELIVER_THURSDAY = @DeliverThursday, DELIVER_FRIDAY = @DeliverFriday, ")
            sqlStatement.Append("DELIVER_SATURDAY = @DeliverSaturday, DELIVER_SUNDAY = @DeliverSunday ")
            sqlStatement.Append("WHERE ID = @CarrierId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CarrierCode", carrierCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@InstallationAvailable", installationAvailable))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CollectOldAvailable", collectOldAvailable))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverMonday", deliverMonday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverTuesday", deliverTuesday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverWednesday", deliverWednesday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverThursday", deliverThursday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverFriday", deliverFriday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverSaturday", deliverSaturday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverSunday", deliverSunday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CarrierId", carrierId))

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
        ''' Delete the record from tbl_carrier based on the given record id
        ''' </summary>
        ''' <param name="idToUseToDelete">The record id to delete</param>
        ''' <returns>The number of records affected</returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal idToUseToDelete As Integer) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_carrier] WHERE ID = @Id"
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

        ''' <summary>
        ''' Insert the specified tbl_carrier record based on the given data
        ''' </summary>
        ''' <param name="carrierCode">The carrier code string</param>
        ''' <param name="installationAvailable">The boolean value to indicate whether or not installation is available</param>
        ''' <param name="collectOldAvailable">The boolean value to indicate whether or not collection is available</param>
        ''' <param name="deliverMonday">Can deliver on Monday</param>
        ''' <param name="deliverTuesday">Can deliver on Tuesday</param>
        ''' <param name="deliverWednesday">Can deliver on Wednesday</param>
        ''' <param name="deliverThursday">Can deliver on Thursday</param>
        ''' <param name="deliverFriday">Can deliver on Friday</param>
        ''' <param name="deliverSaturday">Can deliver on Saturday</param>
        ''' <param name="deliverSunday">Can deliver on Sunday</param>
        ''' <returns>Number of records affected</returns>
        ''' <remarks></remarks>
        Public Function AddNewCarrierRecord(ByVal carrierCode As String, ByVal installationAvailable As Boolean, ByVal collectOldAvailable As Boolean, _
                                            ByVal deliverMonday As Boolean, ByVal deliverTuesday As Boolean, ByVal deliverWednesday As Boolean, ByVal deliverThursday As Boolean, _
                                            ByVal deliverFriday As Boolean, ByVal deliverSaturday As Boolean, ByVal deliverSunday As Boolean) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("INSERT INTO [tbl_carrier] (CARRIER_CODE, INSTALLATION_AVAILABLE, COLLECT_OLD_AVAILABLE, DELIVER_MONDAY, ")
            sqlStatement.Append("DELIVER_TUESDAY, DELIVER_WEDNESDAY, DELIVER_THURSDAY, DELIVER_FRIDAY, DELIVER_SATURDAY, DELIVER_SUNDAY) ")
            sqlStatement.Append("VALUES (@CarrierCode, @InstallationAvailable, @CollectOldAvailable, @DeliverMonday, @DeliverTuesday, ")
            sqlStatement.Append("@DeliverWednesday, @DeliverThursday, @DeliverFriday, @DeliverSaturday, @DeliverSunday)")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CarrierCode", carrierCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@InstallationAvailable", installationAvailable))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CollectOldAvailable", collectOldAvailable))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverMonday", deliverMonday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverTuesday", deliverTuesday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverWednesday", deliverWednesday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverThursday", deliverThursday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverFriday", deliverFriday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverSaturday", deliverSaturday))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DeliverSunday", deliverSunday))

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