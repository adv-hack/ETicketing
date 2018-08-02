
Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_tracking_settings_values based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_tracking_settings_values
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_tracking_settings_values"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_tracking_settings_values" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Inserts the page tracking into tbl_page_Tracking
        ''' </summary>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="partner">The partner .</param>
        ''' <param name="trackingProvider">The tracking provider</param>
        ''' <returns></returns>
        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, ByVal trackingProvider As String, _
                            ByVal settingName As String, ByVal value As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO tbl_tracking_settings_values" & _
                 " ([BUSINESS_UNIT],[PARTNER],[TRACKING_PROVIDER] ,[SETTING_NAME],[VALUE] )     VALUES  " & _
                 " (@BUSINESS_UNIT,@PARTNER,@TRACKING_PROVIDER ,@SETTING_NAME,@VALUE )"


                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PROVIDER", trackingProvider))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SETTING_NAME", settingName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VALUE", value))
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
        ''' Updates the tracking settings for the specified ID
        ''' </summary>
        ''' <param name="trackingSettingsValuesID">The ID.</param>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="partner">The partner .</param>
        ''' <param name="trackingProvider">The tracking provider</param>
        ''' <param name="settingName">The setting name</param>
        ''' <param name="value">The value</param>
        ''' <returns></returns>
        Public Function Update(ByVal trackingSettingsValuesID As String, ByVal businessUnit As String, ByVal partner As String, _
                ByVal trackingProvider As String, ByVal settingName As String, ByVal value As String) As Integer
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
                Dim sqlStatement As String = "UPDATE TBL_TRACKING_SETTINGS_VALUES" & _
                    " SET BUSINESS_UNIT=@BUSINESS_UNIT, " & _
                    " PARTNER=@PARTNER, " & _
                    " TRACKING_PROVIDER = @TRACKING_PROVIDER, " & _
                    " SETTING_NAME = @SETTING_NAME , " & _
"VALUE = @VALUE WHERE ID=@trackingSettingsValuesID"


                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@trackingSettingsValuesID", trackingSettingsValuesID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PROVIDER", trackingProvider))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SETTING_NAME", settingName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VALUE", value))
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
        ''' Deletes the page tracking for the given ID.
        ''' </summary>
        ''' <param name="trackingSettingsValuesID">The trackingSettingsValuesID.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function DeleteByID(ByVal trackingSettingsValuesID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_TRACKING_SETTINGS_VALUES " & _
                "WHERE ID=@ID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", trackingSettingsValuesID))

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
        ''' Gets all the tracking settings values details.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetTrackingSettingsValuesAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTrackingSettingsValues")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_TRACKING_SETTINGS_VALUES"

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
        ''' Gets the page tracking details for the given ID.
        ''' </summary>
        ''' <param name="trackingSettingValuesID">The trackingSettingValuesID.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetPageTrackingByID(ByVal trackingSettingValuesID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTrackingSettingsValuesByID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(trackingSettingValuesID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", trackingSettingValuesID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_TRACKING_SETTINGS_VALUES WHERE ID=@ID"
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

