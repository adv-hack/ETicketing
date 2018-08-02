Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_flash_stadium_settings based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_flash_stadium_settings
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_flash_stadium_settings"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_flash_stadium_settings" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Deletes the specified stadium records.
        ''' </summary>
        ''' <param name="stadiumNameToDelete">The stadium name to delete.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Delete(ByVal stadiumNameToDelete As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_FLASH_STADIUM_SETTINGS " & _
                "WHERE STADIUM_NAME=@StadiumNameToDelete "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumNameToDelete", stadiumNameToDelete))

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
        ''' Deletes the specified stadium records.
        ''' </summary>
        ''' <param name="stadiumNameToDelete">The stadium name to delete.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteByStadiumAndAttribute(ByVal stadiumNameToDelete As String, ByVal typeToDelete As String, ByVal attrNameToDelete As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_FLASH_STADIUM_SETTINGS " & _
                "WHERE STADIUM_NAME=@StadiumNameToDelete " & _
                "AND TYPE=@Type " & _
                "AND ATTRIBUTE_NAME=@AttributeName"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumNameToDelete", stadiumNameToDelete))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Type", typeToDelete))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeName", attrNameToDelete))

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
        ''' Gets the all the Stadium Names.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns></returns>
        Public Function GetAllStadiumNames(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllStadiumNames")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select Distinct STADIUM_NAME From TBL_FLASH_STADIUM_SETTINGS"

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
        ''' Gets the records by Stadium name.
        ''' </summary>
        ''' <param name="stadiumName">The stadium name.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns></returns>
        Public Function GetByStadium(ByVal stadiumName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByStadiumAndType")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(stadiumName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumName", stadiumName))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_FLASH_STADIUM_SETTINGS WHERE STADIUM_NAME=@StadiumName"

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
        ''' Gets the records by Stadium name and attribute type.
        ''' </summary>
        ''' <param name="stadiumName">The stadium name.</param>
        ''' <param name="type">The attribute type.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns></returns>
        Public Function GetByStadiumAndType(ByVal stadiumName As String, ByVal type As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByStadiumAndType")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(stadiumName) & ToUpper(type)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumName", stadiumName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Type", type))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM TBL_FLASH_STADIUM_SETTINGS WHERE STADIUM_NAME=@StadiumName AND TYPE=@Type"

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
        ''' Copies the records from one stadium to another stadium inside the table tbl_flash_stadium_settings
        ''' irrespective of whether destination stadium exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromStadium">From Stadium.</param>
        ''' <param name="toStadium">To Stadium.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromStadium As String, ByVal toStadium As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "INSERT INTO TBL_FLASH_STADIUM_SETTINGS (" & _
                    "STADIUM_NAME, TYPE, ATTRIBUTE_NAME, ATTRIBUTE_VALUE) " & _
                    "SELECT " & _
                    "@ToStadum As STADIUM_NAME, TYPE, ATTRIBUTE_NAME, ATTRIBUTE_VALUE " & _
                    "FROM TBL_FLASH_STADIUM_SETTINGS " & _
                    "WHERE STADIUM_NAME = @FromStadium"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromStadium", fromStadium))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToStadum", toStadium))

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

        ''' <summary>
        ''' Inserts the specified attribute name and attribute value if attribute name is not exists for the
        ''' given stadium name and type and returns no of affected rows
        ''' </summary>
        ''' <param name="stadiumName">Name of the stadium.</param>
        ''' <param name="typeOfSetting">The type of setting.</param>
        ''' <param name="attributeName">Name of the attribute.</param>
        ''' <param name="attributeValue">The attribute value.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Insert(ByVal stadiumName As String, ByVal typeOfSetting As String, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Boolean

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                'Check Whether the given attribute name already exists or not
                Dim sqlStatement As String = "SELECT COUNT(STADIUM_NAME) AS ATTRIBUTEEXISTS FROM TBL_FLASH_STADIUM_SETTINGS WHERE " & _
                "STADIUM_NAME=@StadiumName AND TYPE=@TypeOfSetting AND ATTRIBUTE_NAME=@AttributeName"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumName", stadiumName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TypeOfSetting", typeOfSetting))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeName", attributeName))
                'Execute
                Dim noOfAffectedRows As Integer = 0
                Dim err As New ErrorObj
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    noOfAffectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    If (noOfAffectedRows = 0) Then
                        sqlStatement = "INSERT INTO TBL_FLASH_STADIUM_SETTINGS (" & _
                        "STADIUM_NAME, TYPE, ATTRIBUTE_NAME, ATTRIBUTE_VALUE) VALUES (" & _
                        "@StadiumName, @TypeOfSetting, @AttributeName, @AttributeValue) "
                        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeValue", attributeValue))
                        'Execute Insert
                        talentSqlAccessDetail.ResultDataSet.Tables.Clear()
                        If (givenTransaction Is Nothing) Then
                            err = talentSqlAccessDetail.SQLAccess()
                        Else
                            err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                        End If
                        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                            affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                        End If
                    Else
                        affectedRows = 0
                    End If
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
        ''' Updates the attribute value for the given stadium name, type and attribute name and returns no of affected rows
        ''' </summary>
        ''' <param name="stadiumName">Name of the stadium.</param>
        ''' <param name="typeOfSetting">The type of setting.</param>
        ''' <param name="attributeName">Name of the attribute.</param>
        ''' <param name="attributeValue">The attribute value.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal stadiumName As String, ByVal typeOfSetting As String, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Boolean

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE TBL_FLASH_STADIUM_SETTINGS SET ATTRIBUTE_VALUE=@AttributeValue " & _
                    "WHERE STADIUM_NAME=@StadiumName AND TYPE=@TypeOfSetting AND ATTRIBUTE_NAME=@AttributeName "
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumName", stadiumName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TypeOfSetting", typeOfSetting))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeName", attributeName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeValue", attributeValue))

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
