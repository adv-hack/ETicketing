Imports System.Data.SqlClient
Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Namespace DataObjects.TableObjects

    '   Error Code  -   TACTBLTDFL- (TAC -Talent Common, TBLTDFL - class name tbl_td_fontslaser)
    '   Next Error Code Starting is TACTBLTDFL-02

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_td_fontslaser based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_td_fontslaser
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
        Const CACHEKEY_CLASSNAME As String = "tbl_td_fontslaser"

        Private Const XMLDOCNAME As String = "@fontSimulateXmlDoc"
        Private Const XPATHSTRING As String = "/ArrayOfDEFontsLaser/DEFontsLaser"
        'Faster than Delete and also uses table level lock
        Private Const TRUNCATE_SQL_STRING As String = " TRUNCATE TABLE TBL_TD_FONTSLASER "
        Private Const INSERT_OPENXML_SQL_STRING As String = " INSERT INTO TBL_TD_FONTSLASER" & _
                                                        " (FONT, BITMAPSIZE)" & _
                                                        " SELECT FONT, BITMAPSIZE" & _
                                                        " FROM OPENXML( " & XMLDOCNAME & ", '" & XPATHSTRING & "', 2) " & _
                                                        " WITH (FONT nvarchar(50), BITMAPSIZE nvarchar(50))"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_td_fontslaser" /> class.
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
        ''' Convert the Generic list DEFontsLaser to XML string and create open xml sql string for truncating all records and insert all 
        ''' Coverted string can be acquired from the GenericListToXml property if no error
        ''' </summary>
        ''' <param name="listFontsLaser">The list fonts laser.</param>
        ''' <returns></returns>
        Public Function GenericListToSQLOpenXmlString(ByVal listFontsLaser As Generic.List(Of DEFontsLaser)) As ErrorObj
            Dim errObj As New ErrorObj
            Try
                'Get the sql statement for openxml
                Dim sqlStringBuilder As New StringBuilder
                sqlStringBuilder.Append(" DECLARE " & XMLDOCNAME & " INT ")
                sqlStringBuilder.AppendFormat(" EXEC sp_xml_preparedocument " & XMLDOCNAME & " OUTPUT, N'{0}'", (GenericListToXmlString(Of Generic.List(Of DEFontsLaser))(listFontsLaser)).Replace("'", "''"))
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
                errObj.ErrorNumber = "TACTBLTDFL-01"
            End Try
            Return errObj

        End Function
#End Region

    End Class
End Namespace