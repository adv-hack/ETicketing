
Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_page_tracking based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_page_tracking
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_page_tracking"

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
        ''' Inserts the page tracking into tbl_page_Tracking
        ''' </summary>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="partner">The partner .</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="languageCode">The language code</param>
        ''' <param name="location">The location</param>
        ''' <param name="trackingProvider">The tracking provider</param>
        ''' <param name="trackingContent">The tracking content</param>
        ''' <returns></returns>
        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, ByVal pageCode As String, ByVal languageCode As String, _
                ByVal location As String, ByVal trackingProvider As String, ByVal trackingContent As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO tbl_page_tracking " & _
                 " ([BUSINESS_UNIT],[PARTNER],[PAGE_CODE] ,[LANGUAGE_CODE] ,[LOCATION] ,[TRACKING_PROVIDER] ,[TRACKING_CONTENT] )     VALUES  " & _
                 " (@BUSINESS_UNIT,@PARTNER,@PAGE_CODE ,@LANGUAGE_CODE ,@LOCATION ,@TRACKING_PROVIDER ,@TRACKING_CONTENT)"


                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOCATION", location))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PROVIDER", trackingProvider))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_CONTENT", trackingContent))
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
        ''' Updates the page tracking details for the specified ID
        ''' </summary>
        ''' <param name="pageTrackingID">The provider ID.</param>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="partner">The partner .</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="languageCode">The language code</param>
        ''' <param name="location">The location</param>
        ''' <param name="trackingProvider">The tracking provider</param>
        ''' <param name="trackingContent">The tracking content</param>
        ''' <returns></returns>
        Public Function Update(ByVal pageTrackingID As String, ByVal businessUnit As String, ByVal partner As String, ByVal pageCode As String, ByVal languageCode As String, _
                ByVal location As String, ByVal trackingProvider As String, ByVal trackingContent As String) As Integer
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
                Dim sqlStatement As String = "UPDATE TBL_PAGE_TRACKING" & _
                    " SET BUSINESS_UNIT=@BUSINESS_UNIT, " & _
                    " PARTNER=@PARTNER, " & _
                    " PAGE_CODE=@PAGE_CODE, " & _
                    " LANGUAGE_CODE=@LANGUAGE_CODE, " & _
                    " LOCATION=@LOCATION, " & _
                    " TRACKING_PROVIDER = @TRACKING_PROVIDER, " & _
                    " TRACKING_CONTENT = @TRACKING_CONTENT WHERE ID=@pageTrackingID"


                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@pageTrackingID", pageTrackingID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOCATION", location))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PROVIDER", trackingProvider))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_CONTENT", trackingContent))
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
        ''' <param name="pageTrackingID">The page tracking ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function DeleteByID(ByVal pageTrackingID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_PAGE_TRACKING " & _
                "WHERE ID=@ID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", pageTrackingID))

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
        ''' Gets all the page tracking details.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetPageTrackingAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPageTrackingAll")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_PAGE_TRACKING"

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
        ''' <param name="pageTrackingID">The page tracking ID.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetPageTrackingByID(ByVal pageTrackingID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPageTrackingByID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(pageTrackingID)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", pageTrackingID))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_PAGE_TRACKING WHERE ID=@ID"
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

