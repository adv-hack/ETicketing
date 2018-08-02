Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_flash_stadiums based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_flash_stadiums
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_flash_stadiums"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_flash_stadiums" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Returns true if the current stadium code has an associated flash movie
        ''' </summary>
        ''' <param name="stadiumCode">The stadium code to select from.</param>
        ''' <param name="cacheing">The caching option, default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30.</param>
        ''' <returns>Boolean value to indicate whether or not the stadium has a corresponding flash movie</returns>
        Public Function DoesStadiumCodeHaveFlashMovie(ByVal stadiumCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "DoesStadiumCode" & stadiumCode & "HaveFlashMovie")

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT COUNT(*) FROM [tbl_flash_stadiums] WHERE [STADIUM_CODE]=@StadiumCode"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return results
            Return (affectedRows > 0)
        End Function

#End Region

    End Class

End Namespace
