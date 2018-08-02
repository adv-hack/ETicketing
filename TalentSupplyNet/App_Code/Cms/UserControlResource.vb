Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with User Control Resource
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2006             All rights reserved.
'
'       Error Number Code base      TTPUCRE- 
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class UserControlResource

        Private _businessUnit As String = String.Empty
        Private _code As String = String.Empty
        Private _partnerCode As String = String.Empty
        Private _attributes As Hashtable = New Hashtable
        Private _err As ErrorObj                                    ' Expose scope to outside world so values are not lost
        Private _multiLingualContent As Hashtable = New Hashtable   ' a HashTable of hashTables keyed by LCID as a string.
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
                Return _code
            End Get
            Set(ByVal value As String)
                _code = value
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
        Public ReadOnly Property Attributes() As Hashtable
            Get
                Return _attributes
            End Get
        End Property
        Public Property Err() As ErrorObj
            Get
                Return _err
            End Get
            Set(ByVal value As ErrorObj)
                _err = value
            End Set
        End Property
        Public ReadOnly Property Content() As Hashtable
            Get
                Return _multiLingualContent
            End Get
        End Property
        Public Function GetContent(ByVal strKey As String, ByVal LCID As Integer, ByVal fromCache As Boolean) As String
            Dim strWrk As String = String.Empty
            '------------------------------------------------------------------------------------------
            '   get the appropriate culture specific cache
            '
            Dim htCultureContent As Hashtable
            '
            If fromCache Then
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
        Private Function GetContentFromDb_SP(ByVal strKey As String, ByVal LCID As Integer) As String
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
                    .ErrorNumber = "TTPUCRE-01"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrReader As SqlDataReader
            If Not Err.HasError Then
                Try '-------------------------------------------------------------------------------
                    'PROCEDURE [dbo].[spCMS_getUserControlContentByCulture]
                    '       @BUSINESSUNIT VARCHAR(255),
                    '       @PARTNERCODE  VARCHAR(255),
                    '       @CONTROLCODE  VARCHAR(255),
                    '       @CODE         VARCHAR(50),
                    '       @CULTUREID    INT,
                    '	    @CONTROLCONTENT NTEXT OUTPUT
                    '-------------------------------------------------------------------------------
                    Const comma As String = ","
                    Dim strCmd As String = "EXEC spCMS_getUserControlContentByCulture " & _
                                    Utilities.Quote(Me.BusinessUnit) & comma & _
                                    Utilities.Quote(Me.PartnerCode) & comma & _
                                    Utilities.Quote(Me.Code) & comma & _
                                    Utilities.Quote(strKey) & comma & _
                                    LCID.ToString

                    cmdSelect = New SqlCommand(strCmd, conTalent)
                    '
                    dtrReader = cmdSelect.ExecuteReader
                    If dtrReader.Read Then
                        strWrk = dtrReader.Item("CONTENT")
                    End If
                    dtrReader.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPUCRE-02"
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
                    .ErrorNumber = "TTPUCRE-03"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Private Function GetContentFromDb(ByVal strKey As String, ByVal LCID As Integer) As String
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
                    .ErrorNumber = "TTPUCRE-11"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            If Not Err.HasError Then
                Try
                    If BusinessUnit.Length > 1 And PartnerCode.Length > 1 Then
                        Const SQLString1 As String = "SELECT CONTROL_CONTENT FROM tbl_control_text_lang " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	= @Param2   " & _
                                                    " AND    CONTROL_CODE	= @Param3   " & _
                                                    " AND	 TEXT_CODE	    = @Param4   " & _
                                                    " AND    CULTURE_ID		= @Param5   "

                        cmdSelect = New SqlCommand(SQLString1, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char, 30)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 255)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.SmallInt)).Value = LCID
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    ElseIf BusinessUnit.Length > 1 Then
                        Const SQLString2 As String = "SELECT CONTROL_CONTENT FROM tbl_control_text_lang  " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    CONTROL_CODE   = @Param3   " & _
                                                    " AND	 TEXT_CODE	    = @Param4   " & _
                                                    " AND    CULTURE_ID		= @Param5   "
                        cmdSelect = New SqlCommand(SQLString2, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 255)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.SmallInt)).Value = LCID
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    Else
                        Const SQLString3 As String = "SELECT CONTROL_CONTENT FROM tbl_control_text_lang  " & _
                                                    " WHERE  BUSINESS_UNIT	< ' '       " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    CONTROL_CODE   = @Param3   " & _
                                                    " AND	 TEXT_CODE	    = @Param4   " & _
                                                    " AND    CULTURE_ID		= @Param5   "
                        cmdSelect = New SqlCommand(SQLString3, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 255)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.SmallInt)).Value = LCID
                            dtrReader = .ExecuteReader()
                        End With
                    End If
                    '------------------------------------------------------------------------------
                    If dtrReader.Read Then _
                        strWrk = dtrReader.Item("CONTENT")

                    dtrReader.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPUCRE-12"
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
                    .ErrorNumber = "TTPUCRE-13"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Public Function GetAttribute(ByVal strKey As String) As String
            '----------------------------------------------------------------------------------
            '   Return a user control attribute. If the key does not exist, attempt to retrieve 
            '   the value from the database, if found add the key/value to the local cache
            '   and return
            '  
            Dim strWrk As String = String.Empty

            If _attributes.ContainsKey(strKey) Then
                strWrk = _attributes(strKey)
            Else
                strWrk = GetAttributeFromDB(strKey)
                If strWrk <> String.Empty Then
                    _attributes.Add(strKey, strWrk)
                End If
            End If
            Return strWrk
        End Function
        Private Function GetAttributeFromDB_SP(ByVal strKey As String) As String
            '------------------------------------------------------------------------------------------
            '   Open stored procedure version
            '
            Dim conTalent As SqlConnection = Nothing
            Try
                Const SqlServer2005 As String = "SqlServer2005"
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()
            Catch ex As Exception
                Const strError1 As String = "Could not establish connection to the database"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError1
                    .ErrorNumber = "TTPUCRE-50"
                    .HasError = True
                End With
            End Try
            '----------------------------------------------------------------------------------
            '   Get the attribute value keyed by strKey. Query the database for it and add it to 
            '   the(Hashtable) before returning. If it can not be found in the database
            '   then return the string 'mt'.  
            '
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrReader As SqlDataReader
            Try '-------------------------------------------------------------------------------
                'ALTER PROCEDURE [dbo].[spCMS_getUserControlAttribute]
                '       @BUSINESSUNIT VARCHAR(255),
                '       @PARTNERCODE  VARCHAR(255),
                '       @CONTROLCODE  VARCHAR(255),
                '	    @ATTRNAME     VARCHAR(50),  
                '	    @ATTRVALUE    VARCHAR(255) OUTPUT
                '-------------------------------------------------------------------------------
                Const strCmd1 As String = "EXEC spCMS_getUserControlAttribute '"
                cmdSelect = New SqlCommand(strCmd1 & _
                                Utilities.Quote(Me.BusinessUnit) & ", " & _
                                Utilities.Quote(Me.PartnerCode) & ", " & _
                                Utilities.Quote(Me.Code) & ", " & _
                                Utilities.Quote(strKey), conTalent)
                '
                dtrReader = cmdSelect.ExecuteReader
                If dtrReader.Read Then
                    strWrk = dtrReader.Item("ATTRVALUE")
                End If
                dtrReader.Close()
                '-----------------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TTPUCRE-55"
                    .HasError = True
                End With
            End Try
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
                    .ErrorNumber = "TTPUCRE-56"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function
        Private Function GetAttributeFromDB(ByVal strKey As String) As String
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
                    .ErrorNumber = "TTPUCRE-01"
                    .HasError = True
                End With
            End Try
            '------------------------------------------------------------------------------------------
            If Not Err.HasError Then
                Try
                    If BusinessUnit.Length > 1 And PartnerCode.Length > 1 Then
                        Const SQLString1 As String = "SELECT ATTR_VALUE FROM tbl_control_attribute " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	= @Param2   " & _
                                                    " AND    CONTROL_CODE	= @Param3   " & _
                                                    " AND	 ATTR_NAME     	= @Param4   "

                        cmdSelect = New SqlCommand(SQLString1, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char, 30)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 255)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    ElseIf BusinessUnit.Length > 1 Then
                        Const SQLString2 As String = "SELECT ATTR_VALUE FROM tbl_control_attribute  " & _
                                                    " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    CONTROL_CODE   = @Param3   " & _
                                                    " AND	 ATTR_NAME     	= @Param4   "
                        cmdSelect = New SqlCommand(SQLString2, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 30)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 255)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            dtrReader = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                    Else
                        Const SQLString3 As String = "SELECT ATTR_VALUE FROM tbl_control_attribute  " & _
                                                    " WHERE  BUSINESS_UNIT	< ' '       " & _
                                                    " AND    PARTNER_CODE	< ' '       " & _
                                                    " AND    CONTROL_CODE   = @Param3   " & _
                                                    " AND	 ATTR_NAME     	= @Param4   "
                        cmdSelect = New SqlCommand(SQLString3, conTalent)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char, 255)).Value = Code
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char, 50)).Value = strKey
                            dtrReader = .ExecuteReader()
                        End With
                    End If
                    '----------------------------------------------------------------------------------
                    If dtrReader.Read Then _
                        strWrk = dtrReader.Item("ATTR_VALUE")
                    dtrReader.Close()
                    '----------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPUCRE-65"
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
                    .ErrorNumber = "TTPUCRE-66"
                    .HasError = True
                End With
            End Try
            Return strWrk
        End Function

    End Class

End Namespace