Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Web Form Resources
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2006             All rights reserved.
'
'       Error Number Code base      TTPWFRE- 
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class WebFormResource

        Private Const _delim1 As String = "', '"
        Private Const _delim2 As String = "', "
        Private Const _uScore As String = "_"

        Private _businessUnit As String = String.Empty
        Private _Code As String = String.Empty
        Private _partnerCode As String = String.Empty
        Private _attributes As Hashtable = New Hashtable
        Private _err As ErrorObj                                    ' Expose scope to outside world so values are not lost
        Private _multiLingualContent As Hashtable = New Hashtable   ' a HashTable of hashTables keyed by LCID as a string.
        Private _multilingualTitles As Hashtable = New Hashtable
        Private _multilingualMetaKeywords As Hashtable = New Hashtable
        Private _multilingualMetaDescription As Hashtable = New Hashtable
        Private Const SQL2005 As String = "SQL2005"
        '
        Private Const Param1 As String = "@Param1"
        Private Const Param2 As String = "@Param2"
        Private Const Param3 As String = "@Param3"
        Private Const Param4 As String = "@Param4"
        Private Const Param5 As String = "@Param5"

        Public Property BusinessUnit() As String
            Get
                Return _businessUnit
            End Get
            Set(ByVal value As String)
                _businessUnit = value
            End Set
        End Property
        Public Property Code() As String
            Get
                Return _Code
            End Get
            Set(ByVal value As String)
                _Code = value
            End Set
        End Property
        Public Property PartnerCode() As String
            Get
                Return _partnerCode
            End Get
            Set(ByVal value As String)
                _partnerCode = value
            End Set
        End Property
        Public Property Err() As ErrorObj
            Get
                Return _err
            End Get
            Set(ByVal value As ErrorObj)
                _err = value
            End Set
        End Property
        Public ReadOnly Property Attributes() As Hashtable
            Get
                Return _attributes
            End Get
        End Property
        Public ReadOnly Property Content() As Hashtable
            Get
                Return _multiLingualContent
            End Get
        End Property
        Public Function GetTitle(ByVal LCID As Integer) As String
            Dim strWrk As String = String.Empty
            Dim key As String = LCID.ToString & _uScore & _Code
            If _multilingualTitles.ContainsKey(key) Then
                strWrk = _multilingualTitles(key)
            Else
                ' ---------------------------------------------------------------
                ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                ' for this page an culture to their respective hashtables. Load
                ' Tile, Meta keywords and meta description to avoid unnecessary
                ' trips to the db.
                ' ---------------------------------------------------------------
                LoadCulturalPagePropertiesFromDb(LCID)

                ' ---------------------------------------------------------------
                ' If the call to LoadCulturalPagePropertiesFromDb produced results
                ' we can now retrieve the data. Otherwise return empty string.
                ' ---------------------------------------------------------------
                If _multilingualTitles.ContainsKey(LCID.ToString & _uScore & _Code) Then
                    strWrk = _multilingualTitles(LCID.ToString & _uScore & _Code)
                End If

            End If
            Return strWrk
        End Function
        Public Function GetMetaKeywords(ByVal LCID As Integer) As String
            Dim strWrk As String = String.Empty
            Dim key As String = LCID.ToString & _uScore & _Code
            If _multilingualMetaKeywords.ContainsKey(key) Then
                strWrk = _multilingualMetaKeywords(key)
            Else
                ' ---------------------------------------------------------------
                ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                ' for this page an culture to their respective hashtables. Load
                ' Tile, Meta keywords and meta description to avoid unnecessary
                ' trips to the db.
                ' ---------------------------------------------------------------
                LoadCulturalPagePropertiesFromDb(LCID)
                ' ---------------------------------------------------------------
                ' If the call to LoadCulturalPagePropertiesFromDb produced results
                ' we can now retrieve the data. Otherwise return empty string.
                ' ---------------------------------------------------------------
                If _multilingualMetaKeywords.ContainsKey(LCID.ToString & _uScore & _Code) Then
                    strWrk = _multilingualMetaKeywords(LCID.ToString & _uScore & _Code)
                End If
            End If
            Return strWrk
        End Function
        Public Function GetMetaDescription(ByVal LCID As Integer) As String
            Dim strWrk As String = String.Empty
            Dim key As String = LCID.ToString & _uScore & _Code
            If _multilingualMetaDescription.ContainsKey(key) Then
                strWrk = _multilingualMetaDescription(key)
            Else
                ' ---------------------------------------------------------------
                ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                ' for this page an culture to their respective hashtables. Load
                ' Tile, Meta keywords and meta description to avoid unnecessary
                ' trips to the db.
                ' ---------------------------------------------------------------
                LoadCulturalPagePropertiesFromDb(LCID)

                ' ---------------------------------------------------------------
                ' If the call to LoadCulturalPagePropertiesFromDb produced results
                ' we can now retrieve the data. Otherwise return empty string.
                ' ---------------------------------------------------------------
                If _multilingualMetaDescription.ContainsKey(LCID.ToString & _uScore & _Code) Then
                    strWrk = _multilingualMetaDescription(LCID.ToString & _uScore & _Code)
                End If
            End If
            Return strWrk
        End Function
        Public Function GetContent(ByVal strKey As String, ByVal LCID As Integer, ByVal fromCache As Boolean) As String
            Dim strWrk As String = String.Empty
            If fromCache Then
                ' get the appropriate culture specific cache
                Dim htCultureContent As Hashtable
                If _multiLingualContent.ContainsKey(LCID.ToString) Then
                    htCultureContent = _multiLingualContent(LCID.ToString)
                Else
                    htCultureContent = New Hashtable
                    _multiLingualContent(LCID.ToString) = htCultureContent
                End If
                If htCultureContent.ContainsKey(strKey) Then
                    strWrk = htCultureContent(strKey)
                Else
                    strWrk = GetContentFromDb(strKey, LCID)
                    htCultureContent(strKey) = strWrk
                End If
            Else
                strWrk = GetContentFromDb(strKey, LCID)
            End If
            Return strWrk
        End Function
        Public Function GetContentFromDb_SP(ByVal strKey As String, ByVal LCID As Integer) As String
            '------------------------------------------------------------------------------------------
            '   Open stored procedure version
            '
            Dim conTalent As SqlConnection = Nothing
            Try
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPWFRE-61"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrReader As SqlDataReader

            If Not Err.HasError Then
                Try '-------------------------------------------------------------------------------
                    'PROCEDURE [dbo].[spCMS_getPageContentByCulture]
                    '	    @BUSINESSUNIT VARCHAR(30),
                    '	    @PARTNERCODE  VARCHAR(30),
                    '	    @PAGECODE     VARCHAR(30),
                    '	    @TEXTCODE     VARCHAR(50),
                    '	    @CULTUREID    INT,
                    '	    @TEXTCONTENT  NTEXT OUTPUT
                    '-------------------------------------------------------------------------------
                    Dim strCmd As String = "EXEC spCMS_getPageContentByCulture  " & _
                                    Utilities.Quote(BusinessUnit) & ", " & _
                                    Utilities.Quote(PartnerCode) & ", " & _
                                    Utilities.Quote(Code) & ", " & _
                                    Utilities.Quote(strKey) & ", " & _
                                    LCID.ToString
                    cmdSelect = New SqlCommand(strCmd, conTalent)
                    '
                    dtrReader = cmdSelect.ExecuteReader
                    If dtrReader.Read Then _
                        strWrk = dtrReader("TEXTCONTENT")
                    dtrReader.Close()
                    '-----------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err()
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-62"
                        .HasError = True
                    End With
                End Try
            End If
            '----------------------------------------------------------------------------------
            '   Close
            '
            Try
                dtrReader = Nothing
                cmdSelect = Nothing
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With Err()
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TTPWFRE-63"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Public Function GetContentFromDb(ByVal strKey As String, ByVal LCID As Integer) As String
            '------------------------------------------------------------------------------------------
            '   Open sequel version
            '
            Dim cmdSelect As SqlCommand
            Dim dtrReader As SqlDataReader
            Dim strWrk As String = String.Empty
            '-------------------------------------------------------------------------------------------
            '   Open connection to DB
            '
            Dim conTalent As SqlConnection = Nothing
            Try
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
                conTalent.Open()            '
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPWFRE-11"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            If Not Err.HasError Then
                Try
                    If BusinessUnit.Length > 1 And PartnerCode.Length > 1 Then
                        Const SQLString1 As String = "SELECT TEXT_CONTENT FROM tbl_page_text_lang " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	= @Param2   " & _
                                                    " AND    TEXT_CODE	    = @Param3   " & _
                                                    " AND    CULTURE_ID		= @Param4   "

                        cmdSelect = New SqlCommand(SQLString1, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char, 30)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 50)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = LCID
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    ElseIf BusinessUnit.Length > 1 Then
                        Const SQLString2 As String = "SELECT TEXT_CONTENT FROM tbl_page_text_lang  " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    TEXT_CODE	    = @Param3   " & _
                                                    " AND    CULTURE_ID		= @Param4   "
                        cmdSelect = New SqlCommand(SQLString2, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 50)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = LCID
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    Else
                        Const SQLString3 As String = "SELECT TEXT_CONTENT FROM tbl_page_text_lang  " & _
                                                    " WHERE  BUSINESS_UNIT	< ' '       " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    TEXT_CODE	    = @Param3   " & _
                                                    " AND    CULTURE_ID		= @Param4   "
                        cmdSelect = New SqlCommand(SQLString3, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 50)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = LCID
                            dtrReader = .ExecuteReader()
                        End With
                    End If
                    '------------------------------------------------------------------------------
                    If dtrReader.Read Then _
                        strWrk = dtrReader.Item("TEXT_CONTENT")

                    dtrReader.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-12"
                        .HasError = True
                    End With
                End Try
            End If
            '----------------------------------------------------------------------------------
            '   Close
            '
            Try
                dtrReader = Nothing
                cmdSelect = Nothing
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TTPWFRE-13"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Public Function GetAttribute(ByVal strKey As String) As String
            ' ----------------------------------------------------------
            ' Return a webform attribute. If the _Attributes cache. If
            ' the key does not exist, attempt to retrieve the value from
            ' the database, if found add the key/value to the local cache
            ' and return
            ' ----------------------------------------------------------
            Dim strWrk As String = String.Empty

            If _attributes.ContainsKey(strKey) Then
                strWrk = _attributes(strKey)
            Else
                strWrk = GetAttributeFromDB(strKey)
                If strWrk <> String.Empty Then
                    ' --------------------------------------
                    ' Unsynchronised so possible that multiple
                    ' versions of the page will do this causing
                    ' an exception adding an existing key.
                    '  Catch the exception but do nothing
                    ' --------------------------------------
                    Try
                        _attributes.Add(strKey, strWrk)
                    Catch ex As Exception
                        Err.ErrorMessage = ex.Message
                    End Try
                End If
            End If
            Return strWrk
        End Function
        Public Function GetAttributeFromDB_SP(ByVal strKey As String) As String
            '------------------------------------------------------------------------------------------
            '   Open stored procedure version
            '
            Dim conTalent As SqlConnection = Nothing
            Try
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPWFRE-71"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrReader As SqlDataReader

            If Not Err.HasError Then
                Try
                    '-------------------------------------------------------------------------------
                    'ALTER PROCEDURE [dbo].[spCMS_getPageAttribute]
                    '	    @BUSINESSUNIT VARCHAR(255),
                    '	    @PARTNERCODE  VARCHAR(255),
                    '	    @PAGECODE     VARCHAR(255),
                    '	    @ATTRNAME     VARCHAR(50),
                    '	    @ATTRVALUE    VARCHAR(255) OUTPUT
                    '-------------------------------------------------------------------------------
                    Const strCmd1 As String = "EXEC spCMS_getPageAttribute '"
                    cmdSelect = New SqlCommand(strCmd1 & _
                                    Me.BusinessUnit & _delim1 & _
                                    Me.PartnerCode & _delim1 & _
                                    Me.Code & _delim1 & _
                                    strKey & "'", conTalent)
                    '
                    dtrReader = cmdSelect.ExecuteReader
                    If dtrReader.Read Then
                        strWrk = dtrReader("ATTRVALUE")
                    End If
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-65"
                        .HasError = True
                    End With
                End Try
            End If
            '--------------------------------------------------------------------------------------
            '   Close
            '
            Try
                dtrReader = Nothing
                cmdSelect = Nothing
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TTPWFRE-66"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Public Function GetAttributeFromDB(ByVal strKey As String) As String
            '------------------------------------------------------------------------------------------
            '   Open sequel version
            '
            Dim cmdSelect As SqlCommand
            Dim dtrReader As SqlDataReader
            Dim strWrk As String = String.Empty
            '-------------------------------------------------------------------------------------------
            '   Open connection to DB
            '
            Dim conTalent As SqlConnection = Nothing
            Try
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
                conTalent.Open()            '
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPWFRE-11"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            If Not Err.HasError Then
                Try
                    If BusinessUnit.Length > 1 And PartnerCode.Length > 1 Then
                        Const SQLString1 As String = "SELECT ATTR_VALUE  FROM tbl_page_attribute  " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	= @Param2   " & _
                                                    " AND    PAGE_CODE	    = @Param3   " & _
                                                    " AND    ATTR_NAME		= @Param4   "

                        cmdSelect = New SqlCommand(SQLString1, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char, 30)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 30)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    ElseIf BusinessUnit.Length > 1 Then
                        Const SQLString2 As String = "SELECT ATTR_VALUE  FROM tbl_page_attribute   " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    PAGE_CODE	    = @Param3   " & _
                                                    " AND    ATTR_NAME		= @Param4   "
                        cmdSelect = New SqlCommand(SQLString2, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 30)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    Else
                        Const SQLString3 As String = "SELECT ATTR_VALUE  FROM tbl_page_attribute   " & _
                                                    " WHERE  BUSINESS_UNIT	< ' '       " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    PAGE_CODE	    = @Param3   " & _
                                                    " AND    ATTR_NAME		= @Param4   "
                        cmdSelect = New SqlCommand(SQLString3, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 30)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            dtrReader = .ExecuteReader()
                        End With
                    End If
                    '------------------------------------------------------------------------------
                    If dtrReader.Read Then _
                        strWrk = dtrReader.Item("ATTR_VALUE ")

                    dtrReader.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-12"
                        .HasError = True
                    End With
                End Try
            End If
            '----------------------------------------------------------------------------------
            '   Close
            '
            Try
                dtrReader = Nothing
                cmdSelect = Nothing
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TTPWFRE-13"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Private Function LoadCulturalPagePropertiesFromDb(ByVal LCID As Integer) As ErrorObj
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrReader As SqlDataReader
            Dim key As String = LCID.ToString & _uScore & _Code
            Dim conTalent As SqlConnection = Nothing
            '----------------------------------------------------------------------------------
            '   Open
            '
            Try
                Const SqlServer2005 As String = "SqlServer2005"
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPWFRE-67"
                    .HasError = True
                End With
            End Try
            '---------------------------------------------------------------------------------------
            '   Retrieve the properties of the page requested qualified by the locale ID LCID. 
            '   Properties are stored in their respective hashtables keyed as <LCID>_<page code>
            ' 
            If Not Err.HasError Then
                Try '-------------------------------------------------------------------------------
                    'PROCEDURE [dbo].[spCMS_pagePropertiesCultural]
                    '       @BUSINESSUNIT VARCHAR(30) ,
                    '       @PARTNERCODE  VARCHAR(30) ,
                    '       @PAGECODE     VARCHAR(255),
                    '       @CULTUREID    INT,
                    '	    @TITLE		  VARCHAR(50) OUTPUT,
                    '	    @METAKEY	  NTEXT OUTPUT,
                    '	    @METADESC     NTEXT OUTPUT 
                    '-------------------------------------------------------------------------------
                    Const strCmd1 As String = "EXEC spCMS_pagePropertiesCultural '"
                    cmdSelect = New SqlCommand(strCmd1 & _
                                    Me.BusinessUnit & _delim1 & _
                                    Me.PartnerCode & _delim1 & _
                                    Me.Code & "', " & _
                                    LCID.ToString, conTalent)
                    dtrReader = cmdSelect.ExecuteReader
                    If dtrReader.Read Then
                        '--------------------------------------------------------------------------
                        '   Unsynchronised so possible that multiple versions of the page will  
                        '   do this(causing) an exception adding an existing key. Catch the 
                        '   exception but do nothing
                        ' 
                        If Not _multilingualTitles.ContainsKey(key) Then
                            Try
                                _multilingualTitles.Add(key, dtrReader.Item("TITLE"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not _multilingualMetaKeywords.ContainsKey(key) Then
                            Try
                                _multilingualMetaKeywords.Add(key, dtrReader.Item("METAKEY"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not _multilingualMetaDescription.ContainsKey(key) Then
                            Try
                                _multilingualMetaDescription.Add(key, dtrReader.Item("METADESC"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If

                    End If
                    dtrReader.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-68"
                        .HasError = True
                    End With
                End Try
            End If
            '----------------------------------------------------------------------------------
            '   Close
            '
            Try
                dtrReader = Nothing
                cmdSelect = Nothing
                conTalent.Close()
            Catch ex As Exception
                Const strError9 As String = "Failed to close database connection"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError9
                    .ErrorNumber = "TTPWFRE-69"
                    .HasError = True
                End With
            End Try
            Return Err
        End Function
        Public Function RefreshPageProperties(ByVal LCID As Integer) As ErrorObj
            ' --------------------------------------------------------------------------------------------------
            ' Remove any cached properties for thie page and culture, reload from db.
            ' 
            Dim key As String = LCID.ToString & _uScore & _Code
            Try
                _multilingualTitles.Remove(key)
            Catch ex As Exception
                Err.ErrorMessage = ex.Message
            End Try
            Try
                _multilingualMetaKeywords.Remove(key)
            Catch ex As Exception
                Err.ErrorMessage = ex.Message
            End Try
            Try
                _multilingualMetaDescription.Remove(key)
            Catch ex As Exception
                Err.ErrorMessage = ex.Message
            End Try
            Err = LoadCulturalPagePropertiesFromDb(LCID)
            Return Err
        End Function
    End Class

End Namespace