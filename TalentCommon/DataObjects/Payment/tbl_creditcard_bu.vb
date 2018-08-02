Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_creditcard_bu based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_creditcard_bu
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_creditcard_bu"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_creditcard_bu" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Private Methods"

#End Region

#Region "Public Methods"

        Public Function GetDefaultCardTypeCode(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim cardTypeCode As String = String.Empty

            Dim moduleName As String = "GetDefaultCardTypeCode"
            Dim dtOutput As DataTable = Nothing
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner)
            Dim isCacheNotFound As Boolean = False
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Try
                Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
                If isCacheNotFound Then
                    'Construct The Call
                    talentSqlAccessDetail = New TalentDataAccess
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP 1 * FROM [tbl_creditcard_bu] WHERE IS_DEFAULT = 1"
                    Dim err As New ErrorObj
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, cacheing, cacheTimeMinutes)
                    If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                        Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                    End If
                End If

                If Me.ResultDataSet.Tables(0) IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 AndAlso Me.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    cardTypeCode = Utilities.CheckForDBNull_String(Me.ResultDataSet.Tables(0).Rows(0)("CARD_CODE")).ToUpper
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return cardTypeCode
        End Function

#End Region

    End Class
End Namespace