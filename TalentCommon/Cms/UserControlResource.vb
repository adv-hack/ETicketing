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
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPUCRE- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class UserControlResource
    Inherits Cms


    Public ReadOnly Property Content(ByVal strKey As String, ByVal LCID As Integer, ByVal fromCache As Boolean) As String
        Get
            Dim strWrk As String = String.Empty
            Try

                '---------------------------------------------------------------------------------------
                '   get the appropriate culture specific cache
                ' Only get from cache if it's active; it should only be active if we are being called 
                ' from browser code.
                '
                If fromCache AndAlso Utilities.IsCacheActive Then
                    Dim strHashKey As String = UCR_CACHE_NAME & BusinessUnit & PartnerCode & KeyCode & PageCode & strKey & LCID

                    If MultiLingualContent.ContainsKey(LCID.ToString) Then
                        UcrHashtable = MultiLingualContent(LCID.ToString)
                    Else
                        UcrHashtable = New Hashtable
                        MultiLingualContent(LCID.ToString) = UcrHashtable
                    End If
                    '
                    If UcrHashtable.ContainsKey(strHashKey) Then
                        strWrk = UcrHashtable(strHashKey)
                    Else
                        Select Case CMSUseStoredProcedures
                            Case Is = True
                                strWrk = ContentFromDb_SP(strKey, LCID)
                            Case Is = False
                                strWrk = ContentFromDb(strKey, LCID)
                        End Select
                        'If strWrk <> String.Empty Then _
                        UcrHashtable(strHashKey) = strWrk
                    End If
                Else
                    strWrk = ContentFromDb(strKey, LCID)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property Content(ByVal strKey As String, ByVal languageCode As String, ByVal fromCache As Boolean, ByVal defaultValue As String) As String
        Get
            Dim strWrk As String = Content(strKey, languageCode, fromCache)
            If String.IsNullOrWhiteSpace(strWrk) Then
                strWrk = defaultValue
            End If
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property Content(ByVal strKey As String, ByVal languageCode As String, ByVal fromCache As Boolean) As String
        Get
            Dim strWrk As String = String.Empty
            Try

                '---------------------------------------------------------------------------------------
                '   get the appropriate culture specific cache
                ' Only get from cache if it's active; it should only be active if we are being called 
                ' from browser code.
                '
                If fromCache AndAlso Utilities.IsCacheActive Then
                    Dim strHashKey As String = UCR_CACHE_NAME & BusinessUnit & PartnerCode & KeyCode & PageCode & strKey & languageCode

                    If MultiLingualContent.ContainsKey(languageCode) Then
                        UcrHashtable = MultiLingualContent(languageCode)
                    Else
                        UcrHashtable = New Hashtable
                        MultiLingualContent(languageCode) = UcrHashtable
                    End If
                    '
                    If UcrHashtable.ContainsKey(strHashKey) Then
                        strWrk = UcrHashtable(strHashKey)
                    Else
                        Select Case CMSUseStoredProcedures
                            Case Is = True
                                strWrk = ContentFromDb_SP(strKey, languageCode)
                            Case Is = False
                                strWrk = ContentFromDb(strKey, languageCode)
                        End Select
                        'If strWrk <> String.Empty Then _
                        UcrHashtable(strHashKey) = strWrk
                    End If
                Else
                    strWrk = ContentFromDb(strKey, languageCode)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Private ReadOnly Property ContentFromDb_SP(ByVal strKey As String, ByVal LCID As Integer) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '------------------------------------------------------------------------------------------
                '   Open stored procedure version
                '
                Err = Sql2005Open()
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                '   ALTER PROCEDURE [dbo].[spCMS_getUserControlContentByCulture]
                '       @BUSINESSUNIT nvarchar(30),
                '       @PARTNERCODE nvarchar(30),
                '       @CONTROLCODE nvarchar(255),
                '       @PAGECODE nvarchar(255),
                '       @TEXTCODE	 nvarchar(50),
                '       @CULTUREID  INT,
                '           @CONTROLCONTENT NTEXT OUTPUT
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        Const strCmd As String = "EXEC spCMS_getUserControlContentByCulture " & _
                                               "@Param1, @Param2, @Param3, @Param4, @Param5, @Param6 "

                        cmdSelect = New SqlCommand(strCmd, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param6, SqlDbType.SmallInt)).Value = LCID
                            dtr = .ExecuteReader
                        End With
                        If dtr.Read Then
                            strWrk = dtr.Item("CONTENT")
                        End If
                        dtr.Close()
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
                dtr = Nothing
                cmdSelect = Nothing
                Sql2005Close()
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Private ReadOnly Property ContentFromDb_SP(ByVal strKey As String, ByVal languageCode As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '------------------------------------------------------------------------------------------
                '   Open stored procedure version
                '
                Err = Sql2005Open()
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                '   ALTER PROCEDURE [dbo].[spCMS_getUserControlContentByCulture]
                '       @BUSINESSUNIT nvarchar(30),
                '       @PARTNERCODE nvarchar(30),
                '       @CONTROLCODE nvarchar(255),
                '       @PAGECODE nvarchar(255),
                '       @TEXTCODE	 nvarchar(50),
                '       @CULTUREID  INT,
                '           @CONTROLCONTENT NTEXT OUTPUT
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        Const strCmd As String = "EXEC spCMS_getUserControlContentByLanguage " & _
                                               "@Param1, @Param2, @Param3, @Param4, @Param5, @Param6 "

                        cmdSelect = New SqlCommand(strCmd, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                            dtr = .ExecuteReader
                        End With
                        If dtr.Read Then
                            strWrk = dtr.Item("CONTENT")
                        End If
                        dtr.Close()
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
                dtr = Nothing
                cmdSelect = Nothing
                Sql2005Close()
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Private ReadOnly Property ContentFromDb(ByVal strKey As String, ByVal LCID As Integer) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '------------------------------------------------------------------------------------------
                '   Open sequel version
                '
                Err = Sql2005Open()
                Const SQLSelect As String = "SELECT CONTROL_CONTENT FROM tbl_control_text_lang WITH (NOLOCK)  " & _
                                                       " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                       " AND    PARTNER_CODE	= @Param2   " & _
                                                       " AND    CONTROL_CODE	= @Param3   " & _
                                                       " AND    PAGE_CODE       = @Param4   " & _
                                                       " AND	TEXT_CODE	    = @Param5   " & _
                                                       " AND    CULTURE_ID		= @Param6   "
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param6, SqlDbType.SmallInt)).Value = LCID
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            strWrk = dtr.Item("CONTROL_CONTENT")
                        ElseIf ExactValuesOnly Then
                            'If only the exact values are needed and there is no data in the database against the conditions
                            'then return an empty string
                            strWrk = String.Empty
                        Else
                            dtr.Close()
                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                .Parameters.Add(New SqlParameter(Param6, SqlDbType.SmallInt)).Value = LCID
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                strWrk = dtr.Item("CONTROL_CONTENT")
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                    .Parameters.Add(New SqlParameter(Param6, SqlDbType.SmallInt)).Value = LCID
                                    dtr = .ExecuteReader()
                                End With
                                ''------------------------------------------------------------------------------
                                If dtr.Read Then
                                    strWrk = dtr.Item("CONTROL_CONTENT")
                                Else
                                    dtr.Close()
                                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                    With cmdSelect
                                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                        .Parameters.Add(New SqlParameter(Param6, SqlDbType.SmallInt)).Value = LCID
                                        dtr = .ExecuteReader()
                                    End With
                                    '------------------------------------------------------------------------------
                                    If dtr.Read Then
                                        strWrk = dtr.Item("CONTROL_CONTENT")
                                    Else
                                        dtr.Close()
                                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                        With cmdSelect
                                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                            .Parameters.Add(New SqlParameter(Param6, SqlDbType.SmallInt)).Value = 0
                                            dtr = .ExecuteReader()
                                        End With
                                        '------------------------------------------------------------------------------
                                        If dtr.Read Then strWrk = dtr.Item("CONTROL_CONTENT")
                                    End If
                                End If
                            End If
                        End If
                        dtr.Close()
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
                dtr = Nothing
                cmdSelect = Nothing
                Sql2005Close()
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Private ReadOnly Property ContentFromDb(ByVal strKey As String, ByVal languageCode As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '------------------------------------------------------------------------------------------
                '   Open sequel version
                '
                Err = Sql2005Open()
                Const SQLSelect As String = "SELECT CONTROL_CONTENT FROM tbl_control_text_lang WITH (NOLOCK)  " & _
                                                       " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                       " AND    PARTNER_CODE	= @Param2   " & _
                                                       " AND    CONTROL_CODE	= @Param3   " & _
                                                       " AND    PAGE_CODE       = @Param4   " & _
                                                       " AND	TEXT_CODE	    = @Param5   " & _
                                                       " AND    LANGUAGE_CODE   = @Param6   "
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            strWrk = dtr.Item("CONTROL_CONTENT")
                        ElseIf ExactValuesOnly Then
                            'If only the exact values are needed and there is no data in the database against the conditions
                            'then return an empty string
                            strWrk = String.Empty
                        Else
                            dtr.Close()
                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                strWrk = dtr.Item("CONTROL_CONTENT")
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                    .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                                    dtr = .ExecuteReader()
                                End With
                                '------------------------------------------------------------------------------
                                If dtr.Read Then
                                    strWrk = dtr.Item("CONTROL_CONTENT")
                                Else
                                    dtr.Close()
                                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                    With cmdSelect
                                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                        .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                                        dtr = .ExecuteReader()
                                    End With
                                    ''------------------------------------------------------------------------------
                                    If dtr.Read Then
                                        strWrk = dtr.Item("CONTROL_CONTENT")
                                    Else
                                        dtr.Close()
                                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                        With cmdSelect
                                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                            .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                                            dtr = .ExecuteReader()
                                        End With
                                        '------------------------------------------------------------------------------
                                        If dtr.Read Then
                                            strWrk = dtr.Item("CONTROL_CONTENT")
                                        Else
                                            dtr.Close()
                                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                            With cmdSelect
                                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                                .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = String.Empty
                                                dtr = .ExecuteReader()
                                            End With
                                            '------------------------------------------------------------------------------
                                            If dtr.Read Then
                                                strWrk = dtr.Item("CONTROL_CONTENT")
                                            Else
                                                dtr.Close()
                                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                                With cmdSelect
                                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Utilities.GetAllString
                                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                                    .Parameters.Add(New SqlParameter(Param6, SqlDbType.Char)).Value = languageCode
                                                    dtr = .ExecuteReader()
                                                End With
                                                If dtr.Read Then strWrk = dtr.Item("CONTROL_CONTENT")
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        dtr.Close()
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
                dtr = Nothing
                cmdSelect = Nothing
                Sql2005Close()
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property

    Public ReadOnly Property Attribute(ByVal strKey As String) As String
        Get
            Return Me.Attribute(strKey, True)
        End Get
    End Property

    Public ReadOnly Property Attribute(ByVal strKey As String, ByVal FromCache As Boolean) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '
                ' Only get from cache if it's active; it should only be active if we are being called 
                ' from browser code.
                '
                If FromCache AndAlso Utilities.IsCacheActive Then
                    '----------------------------------------------------------------------------------
                    '   Return a user control attribute. If the key does not exist, attempt to retrieve 
                    '   the value from the database, if found add the key/value to the local cache
                    '   and return
                    '  
                    Dim strHashKey As String = UCR_CACHE_NAME & BusinessUnit & PartnerCode & KeyCode & PageCode & strKey

                    If AttHashtable.ContainsKey(strHashKey) Then
                        strWrk = AttHashtable(strHashKey)
                    Else
                        Select Case CMSUseStoredProcedures
                            Case Is = True
                                strWrk = AttributeFromDB_SP(strKey)
                            Case False
                                strWrk = AttributeFromDB(strKey)
                        End Select
                        If Not AttHashtable.ContainsKey(strHashKey) Then
                            AttHashtable.Add(strHashKey, strWrk)
                        Else
                            AttHashtable(strHashKey) = strWrk
                        End If
                    End If
                Else
                    strWrk = AttributeFromDB(strKey)
                End If

            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property

    Private ReadOnly Property AttributeFromDB_SP(ByVal strKey As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '------------------------------------------------------------------------------------------
                '   Open stored procedure version
                '
                Err = Sql2005Open()
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        Const strCmd As String = "EXEC spCMS_getUserControlAttribute  " & _
                                                 " @Param1, @Param2 ,@Param3, @Param4 "

                        cmdSelect = New SqlCommand(strCmd, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Int)).Value = strKey
                            dtr = .ExecuteReader()
                        End With

                        If dtr.Read Then
                            strWrk = dtr.Item("ATTRVALUE")
                        End If
                        dtr.Close()
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
                End If
                '----------------------------------------------------------------------------------
                '   Close
                '
                dtr = Nothing
                cmdSelect = Nothing
                Sql2005Close()
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Private ReadOnly Property AttributeFromDB(ByVal strKey As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try '------------------------------------------------------------------------------------------
                '   Open sequel version
                '
                Err = Sql2005Open()
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                Const SQLSelect As String = "SELECT ATTR_VALUE FROM tbl_control_attribute WITH (NOLOCK)  " & _
                                                       " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                       " AND    PARTNER_CODE	= @Param2   " & _
                                                       " AND    CONTROL_CODE	= @Param3   " & _
                                                       " AND    PAGE_CODE       = @Param4   " & _
                                                       " AND    ATTR_NAME       = @Param5   "
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                            dtr = .ExecuteReader()
                        End With
                        '----------------------------------------------------------------------------------
                        If dtr.Read Then
                            strWrk = dtr.Item("ATTR_VALUE")
                        ElseIf ExactValuesOnly Then
                            'If only the exact values are needed and there is no data in the database against the conditions
                            'then return an empty string
                            strWrk = String.Empty
                        Else
                            dtr.Close()
                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                dtr = .ExecuteReader()
                            End With
                            '----------------------------------------------------------------------------------
                            If dtr.Read Then
                                strWrk = dtr.Item("ATTR_VALUE")
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                    dtr = .ExecuteReader()
                                End With
                                '----------------------------------------------------------------------------------
                                If dtr.Read Then
                                    strWrk = dtr.Item("ATTR_VALUE")
                                Else
                                    dtr.Close()
                                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                    With cmdSelect
                                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                        dtr = .ExecuteReader()
                                    End With
                                    '----------------------------------------------------------------------------------
                                    If dtr.Read Then
                                        strWrk = dtr.Item("ATTR_VALUE")
                                    Else
                                        dtr.Close()
                                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                        With cmdSelect
                                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                            dtr = .ExecuteReader()
                                        End With
                                        '----------------------------------------------------------------------------------
                                        If dtr.Read Then
                                            strWrk = dtr.Item("ATTR_VALUE")
                                        Else
                                            dtr.Close()
                                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                            With cmdSelect
                                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = Utilities.GetAllString
                                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = strKey
                                                dtr = .ExecuteReader()
                                            End With
                                            '----------------------------------------------------------------------------------
                                            If dtr.Read Then strWrk = dtr.Item("ATTR_VALUE")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                        dtr.Close()
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
                '----------------------------------------------------------------------------------
                '   Close
                '
                dtr = Nothing
                cmdSelect = Nothing
                Sql2005Close()
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property

End Class
