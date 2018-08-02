Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Namespace DataObjects.TableObjects

    '   Error Code  -   TACTBLTDLP- (TAC -Talent Common, TBLTDLP - class name tbl_td_labelproperties)
    '   Next Error Code Starting is TACTBLTDLP-02

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_td_labelproperties based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_td_labelproperties
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
        Const CACHEKEY_CLASSNAME As String = "tbl_td_labelproperties"

        Private Const XMLDOCNAME As String = "@labelXmlDoc"
        Private Const XPATHSTRING As String = "/ArrayOfDELabelProperties/DELabelProperties"
        Private Const DELETE_OPENXML_SQL_STRING As String = " DELETE TBL_TD_LABELPROPERTIES" & _
                                                        " WHERE TBL_TD_LABELPROPERTIES.LABELLNAME IN" & _
                                                        " (SELECT LABELLNAME" & _
                                                        " FROM OPENXML( " & XMLDOCNAME & ", '" & XPATHSTRING & "', 2) " & _
                                                        " WITH (LABELLNAME nvarchar(50)))   "
        Private Const INSERT_OPENXML_SQL_STRING As String = " INSERT INTO TBL_TD_LABELPROPERTIES" & _
                                                        " (LABELLNAME, LABELWIDTH, LABELHEIGHT, HORSHIFT," & _
                                                        " VERSHIFT, HOFFSET, VOFFSET, FORMOFFSET, NRROW," & _
                                                        " HEADVOLTAGE, FEEDBEFORE, FEEDAFTER, SETUPNAME," & _
                                                        " LVERSION, BACKGROUNDIMAGE, ROTATION)" & _
                                                        " SELECT LABELLNAME, LABELWIDTH, LABELHEIGHT, HORSHIFT," & _
                                                        " VERSHIFT, HOFFSET, VOFFSET, FORMOFFSET, NRROW," & _
                                                        " HEADVOLTAGE, FEEDBEFORE, FEEDAFTER, SETUPNAME," & _
                                                        " LVERSION, BACKGROUNDIMAGE, ROTATION" & _
                                                        " FROM OPENXML( " & XMLDOCNAME & ", '" & XPATHSTRING & "', 2) " & _
                                                        " WITH (LABELLNAME nvarchar(50), LABELWIDTH nvarchar(50)," & _
                                                        " LABELHEIGHT nvarchar(50), HORSHIFT nvarchar(50)," & _
                                                        " VERSHIFT nvarchar(50), HOFFSET nvarchar(50)," & _
                                                        " VOFFSET nvarchar(50), FORMOFFSET nvarchar(50), NRROW nvarchar(50)," & _
                                                        " HEADVOLTAGE nvarchar(50), FEEDBEFORE nvarchar(50)," & _
                                                        " FEEDAFTER nvarchar(50), SETUPNAME nvarchar(50)," & _
                                                        " LVERSION nvarchar(50), BACKGROUNDIMAGE nvarchar(50), ROTATION int)"
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

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_td_labelproperties" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Convert the Generic list DELabelProperties to XML string and create open xml sql string for deleting given labelnames if exists and insert all the give labelnames 
        ''' Coverted string can be acquired from the GenericListToXml property if no error
        ''' </summary>
        ''' <param name="listLabelProperties">The list label properties.</param>
        ''' <returns></returns>
        Public Function GenericListToSQLOpenXmlString(ByVal listLabelProperties As Generic.List(Of DELabelProperties)) As ErrorObj
            Dim errObj As New ErrorObj
            Try
                'Get the sql statement for openxml
                Dim sqlStringBuilder As New StringBuilder
                sqlStringBuilder.Append(" DECLARE " & XMLDOCNAME & " INT ")
                sqlStringBuilder.AppendFormat(" EXEC sp_xml_preparedocument " & XMLDOCNAME & " OUTPUT, N'{0}'", (GenericListToXmlString(Of Generic.List(Of DELabelProperties))(listLabelProperties)).Replace("'", "''"))
                sqlStringBuilder.Append(DELETE_OPENXML_SQL_STRING)
                sqlStringBuilder.Append(INSERT_OPENXML_SQL_STRING)
                sqlStringBuilder.Append(" EXEC sp_xml_removedocument " & XMLDOCNAME)
                'Assign this to class level variable
                _genericListToXmlString = sqlStringBuilder.ToString()
                errObj.HasError = False
                sqlStringBuilder = Nothing
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TACTBLTDLP-01"
            End Try
            Return errObj

        End Function

        ''' <summary>
        ''' Gets the labelproperties details based on the given label name.
        ''' </summary>
        ''' <param name="labelName">Name of the label.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetByLabelName(ByVal labelName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByLabelName")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(labelName)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * from TBL_TD_LABELPROPERTIES WITH (NOLOCK) where LABELLNAME = @LabelName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LabelName", labelName))

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
