
Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_tracking_providers based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_tracking_providers
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_tracking_providers"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_club_details" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Inserts the tracking provider to tbl_tracking_providers
        ''' </summary>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="partner">The partner .</param>
        ''' <param name="trackingProvider">The tracking provider.</param>
        ''' <param name="sequence">The sequence</param>
        ''' <returns></returns>
        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, _
                ByVal trackingProvider As String, ByVal sequence As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO tbl_tracking_providers " & _
                 " ([BUSINESS_UNIT] ,[PARTNER], [TRACKING_PROVIDER],[SEQUENCE])     VALUES  " & _
                 " (@BUSINESS_UNIT, @PARTNER,@TRACKING_PROVIDER,@SEQUENCE)"


                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PROVIDER", trackingProvider))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SEQUENCE", sequence))
                'Execute Insert

                err = talentSqlAccessDetail.SQLAccess()

                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        ''' <summary>
        ''' Updates the tracking provider details for the specified ID
        ''' </summary>
        ''' <param name="providerID">The provider ID.</param>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="partner">The partner.</param>
        ''' <param name="trackingProvider">Tracking provider.</param>
        ''' <param name="sequence">The sequence.</param>
        ''' <returns></returns>
        Public Function Update(ByVal providerID As String, ByVal businessUnit As String, _
                ByVal partner As String, ByVal trackingProvider As String, _
                 ByVal sequence As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "UPDATE TBL_TRACKING_PROVIDERS" & _
                    " SET BUSINESS_UNIT=@BusinessUnit, " & _
                    " PARTNER=@PARTNER, " & _
                    " TRACKING_PROVIDER=@TrackingProvider, " & _
                    " SEQUENCE=@Sequence WHERE ID=@ProviderID"


                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProviderID", providerID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TrackingProvider", trackingProvider))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Sequence", sequence))
                'Execute Insert

                err = talentSqlAccessDetail.SQLAccess()

                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function



        ''' <summary>
        ''' Deletes the provider for the given ID.
        ''' </summary>
        ''' <param name="providerID">The provider ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function DeleteByID(ByVal providerID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_TRACKING_PROVIDERS " & _
                "WHERE ID=@ProviderID "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProviderId", providerID))

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
        ''' Gets all the tracking provider details.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetTrackingProvidersAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTrackingProvidersAll")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_TRACKING_PROVIDERS"

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
        ''' Gets the tracking provider details for the given ID.
        ''' </summary>
        ''' <param name="providerID">The providerID.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetTrackingProvidersByID(ByVal providerID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTrackingProvidersByID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(providerID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProviderId", providerID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_TRACKING_PROVIDERS WHERE ID=@providerID"

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

