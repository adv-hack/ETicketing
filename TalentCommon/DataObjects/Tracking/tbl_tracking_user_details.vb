
Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_tracking_user_details based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_tracking_user_details
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_tracking_user_details"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_tracking_user_details" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets all the user tracking details.
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="location">The script location</param>
        ''' <param name="pageCode">The given page code</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetUserTrackingDetails(ByVal businessUnit As String, ByVal pageCode As String, ByVal location As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetUserTrackingDetails") & businessUnit & pageCode & location
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [TRACKING_TYPE], [CONTENT] FROM [tbl_tracking_user_details] WHERE [BUSINESS_UNIT] IN (@BusinessUnit, '*ALL') AND [PAGE_CODE] = @PageCode AND [LOCATION] = @Location"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Location", location))

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
        ''' Gets all the page tracking details.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetDistinctTrackingPage(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllByBUPartnerMaintenance")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT DISTINCT PAGE_CODE, LOCATION FROM tbl_tracking_user_details WHERE BUSINESS_UNIT=@BUSINESS_UNIT"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
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
        ''' Deletes the page tracking for the given tracking page name.
        ''' </summary>
        ''' <param name="pageName">The page name.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns></returns>
        Public Function DeleteByPageName(ByVal pageName As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_tracking_user_details " & _
                "WHERE PAGE_CODE=@PAGE_CODE"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", pageName))

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
        ''' Gets all the page tracking orders.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTrackingPage(ByVal trackingPage As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTrackingPage")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_tracking_user_details WHERE PAGE_CODE=@PAGE_CODE"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", trackingPage))

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
        ''' Inserts the page tracking into tbl_tracking_orders
        ''' </summary>
        ''' <param name="businessUnit">BU</param>
        ''' <param name="trackingPage">Tracking page</param>
        ''' <param name="trackingType">Tracking type</param>
        ''' <param name="trackingContent">Tracking content</param>
        ''' <returns></returns>
        Public Function Insert(ByVal businessUnit As String, ByVal trackingPage As String, ByVal trackingType As String, _
                ByVal trackingContent As String, ByVal location As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO tbl_tracking_user_details " & _
                 " ([BUSINESS_UNIT] ,[PAGE_CODE] ,[TRACKING_TYPE] ,[LOCATION] ,[CONTENT])  VALUES  " & _
                 " (@BUSINESS_UNIT ,@TRACKING_PAGE ,@TRACKING_TYPE, @LOCATION ,@TRACKING_CONTENT) "


                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PAGE", trackingPage))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_TYPE", trackingType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOCATION", location))
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
        ''' Updates the tracking content by tr
        ''' </summary>
        ''' <param name="businessUnit">The BU.</param>
        ''' <param name="trackingPage">Tracking page.</param>
        ''' <param name="trackingType">Tracking Type.</param>
        ''' <param name="trackingContent">Tracking Content.</param>
        ''' <returns></returns>
        Public Function UpdateContent(ByVal businessUnit As String, ByVal trackingPage As String, ByVal trackingType As String, ByVal trackingContent As String) As Integer
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
                Dim sqlStatement As String = "UPDATE tbl_tracking_user_details" & _
                    " SET CONTENT=@TRACKING_CONTENT " & _
                    " WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PAGE_CODE=@TRACKING_PAGE AND TRACKING_TYPE=@TRACKING_TYPE"


                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_CONTENT", trackingContent))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_PAGE", trackingPage))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TRACKING_TYPE", trackingType))
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
#End Region

    End Class
End Namespace

