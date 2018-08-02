Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Namespace DataObjects.TableObjects

    '   Error Code  -   TACTBLTDFRMR- (TAC -Talent Common, TBLTDFRMR - class name tbl_td_formattingrules)
    '   Next Error Code Starting is TACTBLTDFRMR-01

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_td_formattingrules based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_td_formattingrules
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        Private _genericListToXmlString As String = String.Empty

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_td_formattingrules"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_td_formattingrules" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the records from tbl_td_formattingrules
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
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
                talentSqlAccessDetail.CommandElements.CommandText = "Select * from TBL_TD_FORMATTINGRULES"

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


