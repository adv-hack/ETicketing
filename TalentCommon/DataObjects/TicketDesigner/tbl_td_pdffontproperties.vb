Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Namespace DataObjects.TableObjects

    '   Error Code  -   TACTBLTDPFP- (TAC -Talent Common, TBLTDPFP - class name tbl_td_pdffontproperties)
    '   Next Error Code Starting is TACTBLTDPFP-01

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_td_pdffontproperties based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_td_pdffontproperties
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
        Const CACHEKEY_CLASSNAME As String = "tbl_td_pdffontproperties"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_td_pdffontproperties" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets the font properties based on the given simulated font name.
        ''' </summary>
        ''' <param name="simulatedFontName">Name of the simulated font.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetBySimulatedFontName(ByVal simulatedFontName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetBySimulatedFontName")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(simulatedFontName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * from TBL_TD_PDFFONTPROPERTIES where SIMULATE = @SimulatedFontName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SimulatedFontName", simulatedFontName))

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


