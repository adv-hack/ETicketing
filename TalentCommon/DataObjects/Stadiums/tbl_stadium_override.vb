
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_stadium_seat_colours based on business functionality
    ''' </summary>
    <Serializable()>
    Public Class tbl_stadium_override
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_stadium_override"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_stadium_seat_colours" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the stadium override records based on the prodect Code
        ''' </summary>
        ''' <param name="productCode">The given prodect Code</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByProductCode(ByVal productCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByProductCode")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & productCode
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_stadium_override] WHERE [PRODUCT]=@ProductCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ProductCode", productCode))

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
        ''' Delete records matching the given ID from tbl_stadium_override
        ''' </summary>
        ''' <param name="ID">tbl_stadium_override ID</param>
        ''' <returns>No of affected rows</returns>
        Public Function DeleteByID(ByVal ID As Integer) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_stadium_override] WHERE [ID] = @ID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", ID))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        '''  Delete any existing record with a matching product code and insert the stadium override record with the given product code and stadium name
        ''' </summary>
        ''' <param name="productCode">The given product code</param>
        ''' <param name="stadiumName">The given stadium name</param>
        ''' <returns>The identity of the new inserted record</returns>
        ''' <remarks></remarks>
        Public Function InsertProductStadium(ByVal productCode As String, ByVal stadiumName As String) As Integer

            Dim outputDataTable As New DataTable
            Dim outputDelete As Integer = 0
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            outputDataTable = GetByProductCode(productCode)

            'Delete if product code exists
            If outputDataTable.Rows.Count > 0 Then
                outputDelete = DeleteByID(outputDataTable.Rows(0)(0))

                If outputDelete > 0 Then

                    'Insert record
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.Cacheing = False
                    talentSqlAccessDetail.Settings.CacheStringExtension = ""
                    Dim sqlStatement As String = "INSERT INTO [tbl_stadium_override] ([PRODUCT], [STADIUM_NAME]) VALUES (@Product, @Stadium) SELECT SCOPE_IDENTITY()"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Stadium", stadiumName))
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

                    'Execute
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                    talentSqlAccessDetail = Nothing

                    'Return results
                    Return output
                End If

            Else
                'Insert record
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                Dim sqlStatement As String = "INSERT INTO [tbl_stadium_override] ([PRODUCT], [STADIUM_NAME]) VALUES (@Product, @Stadium) SELECT SCOPE_IDENTITY()"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Product", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Stadium", stadiumName))
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
                talentSqlAccessDetail = Nothing

                'Return results
                Return output
            End If
        End Function

        Public Function GetOverridenStadiumNameForProduct(ByVal productCode As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim stadiumName As String = String.Empty
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetOverridenStadiumNameForProduct" & productCode & businessUnit)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@productCode", productCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [STADIUM_NAME] FROM [tbl_stadium_override] WHERE [PRODUCT]=@productCode"

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
        ''' Remove the given cache key from the cache
        ''' </summary>
        ''' <param name="productCode">productCode key string</param>
        ''' <param name="businessUnit">businessUnit key string</param>
        Public Sub RemoveOverridenStadiumCacheKey(ByVal productCode As String, ByVal businessUnit As String)
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetOverridenStadiumNameForProduct" & productCode & businessUnit)
            RemoveKeyFromCache(cacheKey.ToString())
        End Sub

#End Region

    End Class
End Namespace