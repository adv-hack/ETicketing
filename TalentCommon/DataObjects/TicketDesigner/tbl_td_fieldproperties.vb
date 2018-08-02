Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Namespace DataObjects.TableObjects

    '   Error Code  -   TACTBLTDFP- (TAC -Talent Common, TBLTDFP - class name tbl_td_fieldproperties)
    '   Next Error Code Starting is TACTBLTDFP-02

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_td_fieldproperties based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_td_fieldproperties
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
        Const CACHEKEY_CLASSNAME As String = "tbl_td_fieldproperties"

        Private Const XMLDOCNAME As String = "@fieldXmlDoc"
        Private Const XPATHSTRING As String = "/ArrayOfDEFieldProperties/DEFieldProperties"
        Private Const DELETE_OPENXML_SQL_STRING As String = " DELETE TBL_TD_FIELDPROPERTIES" & _
                                                        " WHERE TBL_TD_FIELDPROPERTIES.LABELNAME IN" & _
                                                        " (SELECT LABELNAME" & _
                                                        " FROM OPENXML( " & XMLDOCNAME & ", '" & XPATHSTRING & "', 2) " & _
                                                        " WITH (LABELNAME nvarchar(50)))"
        Private Const INSERT_OPENXML_SQL_STRING As String = " INSERT INTO TBL_TD_FIELDPROPERTIES" & _
                                                        " (LABELNAME, FIELDTYPE, XLEFT, YTOP," & _
                                                        " XRIGHT, YBOTTOM, THICK, THIN," & _
                                                        " HEIGHT, SLANT, FONT, ANGLE, ALIGN," & _
                                                        " CONTENTS, [LENGTH], [NAME], PROMPT," & _
                                                        " INPUTTYPE, START, INCREMENT, [REPEAT]," & _
                                                        " OFFSET, FORMAT, LINESPACE, [FILE]," & _
                                                        " INVERTED, HUMANREADABLE, USEREALTIMECLOCK," & _
                                                        " PRINTONLABEL, MAG, TTF)" & _
                                                        " SELECT LABELNAME, FIELDTYPE, XLEFT, YTOP," & _
                                                        " XRIGHT, YBOTTOM, THICK, THIN," & _
                                                        " HEIGHT, SLANT, FONT, ANGLE, ALIGN," & _
                                                        " CONTENTS, [LENGTH], [NAME], PROMPT," & _
                                                        " INPUTTYPE, START, INCREMENT, [REPEAT]," & _
                                                        " OFFSET, FORMAT, LINESPACE, [FILE]," & _
                                                        " INVERTED, HUMANREADABLE, USEREALTIMECLOCK," & _
                                                        " PRINTONLABEL, MAG, TTF" & _
                                                        " FROM OPENXML( " & XMLDOCNAME & ", '" & XPATHSTRING & "', 2) " & _
                                                        " WITH (LABELNAME nvarchar(50), FIELDTYPE nvarchar(50), XLEFT nvarchar(50)," & _
                                                        " YTOP nvarchar(50), XRIGHT nvarchar(50), YBOTTOM nvarchar(50)," & _
                                                        " THICK nvarchar(50), THIN nvarchar(50), HEIGHT nvarchar(50)," & _
                                                        " SLANT nvarchar(50), FONT nvarchar(50), ANGLE nvarchar(50)," & _
                                                        " ALIGN nvarchar(50), CONTENTS nvarchar(255), [LENGTH] nvarchar(50)," & _
                                                        " [NAME] nvarchar(50), PROMPT nvarchar(50), INPUTTYPE nvarchar(50)," & _
                                                        " START nvarchar(50), INCREMENT nvarchar(50), [REPEAT] nvarchar(50)," & _
                                                        " OFFSET nvarchar(50), FORMAT nvarchar(50), LINESPACE nvarchar(50)," & _
                                                        " [FILE] nvarchar(50), INVERTED nvarchar(50), HUMANREADABLE nvarchar(50)," & _
                                                        " USEREALTIMECLOCK nvarchar(50), PRINTONLABEL nvarchar(50), MAG nvarchar(50)," & _
                                                        " TTF nvarchar(50))"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_td_fieldproperties" /> class.
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
        ''' Convert the Generic list DEFieldProperties to XML string and create open xml sql string for deleting given labelnames if exists and insert all the give labelnames 
        ''' Coverted string can be acquired from the GenericListToXml property if no error
        ''' </summary>
        ''' <param name="listFieldProperties">The list field properties.</param>
        ''' <returns>Error Object</returns>
        Public Function GenericListToSQLOpenXmlString(ByVal listFieldProperties As Generic.List(Of DEFieldProperties)) As ErrorObj
            Dim errObj As New ErrorObj
            Try
                'Get the sql statement for openxml
                Dim sqlStringBuilder As New StringBuilder
                sqlStringBuilder.Append(" DECLARE " & XMLDOCNAME & " INT ")
                sqlStringBuilder.AppendFormat(" EXEC sp_xml_preparedocument " & XMLDOCNAME & " OUTPUT, N'{0}'", (GenericListToXmlString(Of Generic.List(Of DEFieldProperties))(listFieldProperties)).Replace("'", "''"))
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
                errObj.ErrorNumber = "TACTBLTDFP-01"
            End Try
            Return errObj
        End Function

        ''' <summary>
        ''' Gets the field properties based on the given label name.
        ''' </summary>
        ''' <param name="labelName">Name of the label.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
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
                talentSqlAccessDetail.CommandElements.CommandText = "Select * from TBL_TD_FIELDPROPERTIES WITH (NOLOCK) where LABELNAME = @LabelName"
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
