Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_stadiums based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_stadiums
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_stadiums"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_stadiums" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Returns the associated Stadium Name
        ''' </summary>
        ''' <param name="stadiumCode">The stadium code to select from.</param>
        ''' <param name="businessUnit">The business unit code</param>
        ''' <param name="cacheing">The caching option, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30.</param>
        ''' <returns>Returns the associated Stadium Name</returns>
        Public Function GetStadiumNameByStadiumCode(ByVal stadiumCode As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim stadiumName As String = String.Empty
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetStadiumNameByStadiumCode" & stadiumCode & businessUnit)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [STADIUM_NAME] FROM [tbl_stadiums] WHERE [STADIUM_CODE]=@StadiumCode AND [BUSINESS_UNIT] IN (@BusinessUnit, '*ALL')"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        stadiumName = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return stadiumName
        End Function

        ''' <summary>
        ''' Returns the associated Stadium based on name and stadium code
        ''' </summary>
        ''' <param name="stadiumCode">The stadium code to select from.</param>
        ''' <param name="stadiumName">The stadium name to select from</param>
        ''' <param name="businessUnit">The business unit code</param>
        ''' <param name="cacheing">The caching option, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30.</param>
        ''' <returns>Returns the associated Stadium Name</returns>
        Public Function GetStadiumByStadiumCodeAndName(ByVal stadiumCode As String, ByVal stadiumName As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim stadium As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetStadiumByStadiumCodeAndName")
            Dim whereClauseFetchHierarchy(1) As String
            Dim cacheKeyHierarchyBased(1) As String

            whereClauseFetchHierarchy(0) = "AND STADIUM_CODE=@StadiumCode AND STADIUM_NAME=@StadiumName"
            cacheKeyHierarchyBased(0) = ToUpper(stadiumCode) & ToUpper(stadiumName)

            whereClauseFetchHierarchy(1) = "AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumName", stadiumName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim sqlStatement As String = "SELECT TOP(1) * FROM [tbl_stadiums] WHERE [BUSINESS_UNIT] IN (@BusinessUnit, '*ALL') "

                'Execute
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 1 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0) Then
                        stadium = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return stadium
        End Function



        ''' <summary>
        ''' Get the stadium definitions based on the given stadium code
        ''' </summary>
        ''' <param name="stadiumCode">The stadium code</param>
        ''' <param name="businessUnit">The business unit</param>
        ''' <param name="cacheing">The caching option, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30.</param>
        ''' <returns>Data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetStadiumByStadiumCode(ByVal stadiumCode As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim stadium As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetStadiumByStadiumCode")
            Dim whereClauseFetchHierarchy(1) As String
            Dim cacheKeyHierarchyBased(1) As String

            whereClauseFetchHierarchy(0) = "AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(0) = ToUpper(stadiumCode)

            whereClauseFetchHierarchy(1) = "AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim sqlStatement As String = "SELECT TOP(1) * FROM [tbl_stadiums] WHERE [BUSINESS_UNIT] IN (@BusinessUnit, '*ALL') "

                'Execute
                Dim err As New ErrorObj
                For whereClauseFetchHierarchyCounter As Integer = 0 To 1 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) AndAlso (talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0) Then
                        stadium = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return stadium
        End Function

        ''' <summary>
        ''' Get the stadium that is set to use favourite seat
        ''' </summary>
        ''' <param name="businessUnit">The business unit code</param>
        ''' <param name="cacheing">The caching option, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30.</param>
        ''' <returns>Returns the stadium code that is set to use favourite seat</returns>
        Public Function GetFavouriteSeatStadiumCode(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim stadiumCode As String = String.Empty
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetFavouriteSeatStadiumCode" & businessUnit)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP (1) [STADIUM_CODE] FROM [tbl_stadiums] WHERE [FAVOURITE_SEAT]=1 AND [BUSINESS_UNIT] IN (@BusinessUnit, '*ALL')"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        stadiumCode = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return stadiumCode
        End Function

        ''' <summary>
        ''' Returns all Stadium Name
        ''' </summary>
        ''' <param name="businessUnit">The business unit</param>
        ''' <param name="cacheing">The caching option, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30.</param>
        ''' <returns>Returns the associated Stadium Name</returns>
        Public Function GetAllStadiums(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim result As DataTable = Nothing
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllStadiums")

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_stadiums] WHERE [BUSINESS_UNIT] IN (@BusinessUnit, '*ALL')"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        result = talentSqlAccessDetail.ResultDataSet.Tables(0)
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return result
        End Function

#End Region

    End Class

End Namespace
