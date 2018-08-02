Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Namespace DataObjects.TableObjects

    '   Error Code  -   TACTBLTDFS- (TAC -Talent Common, TBLTDFS - class name tbl_td_fontsimulate)
    '   Next Error Code Starting is TACTBLTDFS-02

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_td_fontsimulate based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_td_fontsimulate
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
        Const CACHEKEY_CLASSNAME As String = "tbl_td_fontsimulate"

        Private Const XMLDOCNAME As String = "@fontSimulateXmlDoc"
        Private Const XPATHSTRING As String = "/ArrayOfDEFontSimulate/DEFontSimulate"
        'Faster than Delete and also uses table level lock
        Private Const TRUNCATE_SQL_STRING As String = " TRUNCATE TABLE TBL_TD_FONTSIMULATE "
        Private Const INSERT_OPENXML_SQL_STRING As String = " INSERT INTO TBL_TD_FONTSIMULATE" & _
                                                        " (FONT, SIMULATE)" & _
                                                        " SELECT FONT, SIMULATE" & _
                                                        " FROM OPENXML( " & XMLDOCNAME & ", '" & XPATHSTRING & "', 2) " & _
                                                        " WITH (FONT nvarchar(50), SIMULATE nvarchar(50))"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_td_fontsimulate" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Gets the generic list to XML.
        ''' Call GenericListToSQLOpenXmlString Method If no error gets this property value
        ''' </summary>
        ''' <value>The generic list to XML.</value>
        Public ReadOnly Property GenericListToXml() As String
            Get
                Return _genericListToXmlString
            End Get
        End Property
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Convert the Generic list DEFontSimulate to XML string and create open xml sql string for truncating all records and insert all 
        ''' Coverted string can be acquired from the GenericListToXml property if no error
        ''' </summary>
        ''' <param name="listFontSimulate">The list font simulate.</param>
        ''' <returns></returns>
        Public Function GenericListToSQLOpenXmlString(ByVal listFontSimulate As Generic.List(Of DEFontSimulate)) As ErrorObj
            Dim errObj As New ErrorObj
            Try
                'Get the sql statement for openxml
                Dim sqlStringBuilder As New StringBuilder
                sqlStringBuilder.Append(" DECLARE " & XMLDOCNAME & " INT ")
                sqlStringBuilder.AppendFormat(" EXEC sp_xml_preparedocument " & XMLDOCNAME & " OUTPUT, N'{0}'", (GenericListToXmlString(Of Generic.List(Of DEFontSimulate))(listFontSimulate)).Replace("'", "''"))
                sqlStringBuilder.Append(TRUNCATE_SQL_STRING)
                sqlStringBuilder.Append(INSERT_OPENXML_SQL_STRING)
                sqlStringBuilder.Append(" EXEC sp_xml_removedocument " & XMLDOCNAME)
                'Assign this to class level variable
                _genericListToXmlString = sqlStringBuilder.ToString()
                errObj.HasError = False
                sqlStringBuilder = Nothing
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TACTBLTDFS-01"
            End Try
            Return errObj

        End Function

        ''' <summary>
        ''' Gets the name of the simulated font based on the given font name.
        ''' </summary>
        ''' <param name="fontName">Name of the font.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetByFontName(ByVal fontName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByFontName")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(fontName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select SIMULATE from TBL_TD_FONTSIMULATE WITH (NOLOCK) where FONT = @FontName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FontName", fontName))

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
