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
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWFRE- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class WebFormResource
    Inherits Cms

    Public ReadOnly Property Title(ByVal LCID As Integer) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & LCID.ToString & _uScore & KeyCode

                If MultilingualTitles.ContainsKey(strHashKey) Then
                    strWrk = MultilingualTitles(strHashKey)
                Else
                    '---------------------------------------------------------------
                    '   Call LoadCulturalPagePropertiesFromDb to add the page properties
                    '   for this page an culture to their respective hashtables. Load
                    '   Tile, Meta keywords and meta description to avoid unnecessary
                    '   trips to the db.
                    '  
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(LCID)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(LCID)
                    End Select
                    '---------------------------------------------------------------
                    '   If the call to LoadCulturalPagePropertiesFromDb produced results
                    '   we can now retrieve the data. Otherwise return empty string.
                    '  
                    If MultilingualTitles.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualTitles(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property Title(ByVal languageCode As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & languageCode & _uScore & KeyCode

                If MultilingualTitles.ContainsKey(strHashKey) Then
                    strWrk = MultilingualTitles(strHashKey)
                Else
                    '---------------------------------------------------------------
                    '   Call LoadCulturalPagePropertiesFromDb to add the page properties
                    '   for this page an culture to their respective hashtables. Load
                    '   Tile, Meta keywords and meta description to avoid unnecessary
                    '   trips to the db.
                    '  
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(languageCode)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(languageCode)
                    End Select
                    '---------------------------------------------------------------
                    '   If the call to LoadCulturalPagePropertiesFromDb produced results
                    '   we can now retrieve the data. Otherwise return empty string.
                    '  
                    If MultilingualTitles.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualTitles(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    'Public ReadOnly Property HeaderText(ByVal LCID As Integer) As String
    '    Get
    '        Dim strWrk As String = String.Empty
    '        Try
    '            Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & LCID.ToString & _uScore & KeyCode

    '            If MultilingualHeaderTexts.ContainsKey(strHashKey) Then
    '                strWrk = MultilingualHeaderTexts(strHashKey)
    '            Else
    '                '---------------------------------------------------------------
    '                '   Call LoadCulturalPagePropertiesFromDb to add the page properties
    '                '   for this page an culture to their respective hashtables. Load
    '                '   Tile, Meta keywords and meta description to avoid unnecessary
    '                '   trips to the db.
    '                '  
    '                Select Case CMSUseStoredProcedures
    '                    Case Is = True
    '                        LoadCulturalPagePropertiesFromDb_SP(LCID)
    '                    Case Is = False
    '                        LoadCulturalPagePropertiesFromDb(LCID)
    '                End Select
    '                '---------------------------------------------------------------
    '                '   If the call to LoadCulturalPagePropertiesFromDb produced results
    '                '   we can now retrieve the data. Otherwise return empty string.
    '                '  
    '                If MultilingualHeaderTexts.ContainsKey(strHashKey) Then _
    '                    strWrk = MultilingualHeaderTexts(strHashKey)
    '            End If
    '        Catch ex As Exception
    '        End Try
    '        Return strWrk
    '    End Get
    'End Property
    Public ReadOnly Property HeaderText(ByVal languageCode As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & languageCode & _uScore & KeyCode

                If MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                    strWrk = MultilingualHeaderTexts(strHashKey)
                Else
                    '---------------------------------------------------------------
                    '   Call LoadCulturalPagePropertiesFromDb to add the page properties
                    '   for this page an culture to their respective hashtables. Load
                    '   Tile, Meta keywords and meta description to avoid unnecessary
                    '   trips to the db.
                    '  
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(languageCode)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(languageCode)
                    End Select
                    '---------------------------------------------------------------
                    '   If the call to LoadCulturalPagePropertiesFromDb produced results
                    '   we can now retrieve the data. Otherwise return empty string.
                    '  
                    If MultilingualHeaderTexts.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualHeaderTexts(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property MetaKeywords(ByVal LCID As Integer) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & LCID.ToString & _uScore & KeyCode

                If MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                    strWrk = MultilingualMetaKeywords(strHashKey)
                Else
                    ' ---------------------------------------------------------------
                    ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                    ' for this page an culture to their respective hashtables. Load
                    ' Tile, Meta keywords and meta description to avoid unnecessary
                    ' trips to the db.
                    ' 
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(LCID)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(LCID)
                    End Select
                    ' ---------------------------------------------------------------
                    ' If the call to LoadCulturalPagePropertiesFromDb produced results
                    ' we can now retrieve the data. Otherwise return empty string.
                    '  
                    If MultilingualMetaKeywords.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualMetaKeywords(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property MetaKeywords(ByVal languageCode As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & languageCode & _uScore & KeyCode

                If MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                    strWrk = MultilingualMetaKeywords(strHashKey)
                Else
                    ' ---------------------------------------------------------------
                    ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                    ' for this page an culture to their respective hashtables. Load
                    ' Tile, Meta keywords and meta description to avoid unnecessary
                    ' trips to the db.
                    ' 
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(languageCode)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(languageCode)
                    End Select
                    ' ---------------------------------------------------------------
                    ' If the call to LoadCulturalPagePropertiesFromDb produced results
                    ' we can now retrieve the data. Otherwise return empty string.
                    '  
                    If MultilingualMetaKeywords.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualMetaKeywords(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property MetaDescription(ByVal LCID As Integer) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & LCID.ToString & _uScore & KeyCode
                '
                ' MultilingualMetaDescription.Add(strHashKey,    dtr.Item("META_DESC"))  
                '
                If MultilingualMetaDescription.ContainsKey(strHashKey) Then
                    strWrk = MultilingualMetaDescription(strHashKey)
                Else
                    ' ---------------------------------------------------------------
                    ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                    ' for this page an culture to their respective hashtables. Load
                    ' Tile, Meta keywords and meta description to avoid unnecessary
                    ' trips to the db.
                    ' 
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(LCID)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(LCID)
                    End Select
                    ' ---------------------------------------------------------------
                    ' If the call to LoadCulturalPagePropertiesFromDb produced results
                    ' we can now retrieve the data. Otherwise return empty string.
                    ' 
                    If MultilingualMetaDescription.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualMetaDescription(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property MetaDescription(ByVal languageCode As String) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & languageCode & _uScore & KeyCode
                '
                ' MultilingualMetaDescription.Add(strHashKey,    dtr.Item("META_DESC"))  
                '
                If MultilingualMetaDescription.ContainsKey(strHashKey) Then
                    strWrk = MultilingualMetaDescription(strHashKey)
                Else
                    ' ---------------------------------------------------------------
                    ' Call LoadCulturalPagePropertiesFromDb to add the page properties
                    ' for this page an culture to their respective hashtables. Load
                    ' Tile, Meta keywords and meta description to avoid unnecessary
                    ' trips to the db.
                    ' 
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            LoadCulturalPagePropertiesFromDb_SP(languageCode)
                        Case Is = False
                            LoadCulturalPagePropertiesFromDb(languageCode)
                    End Select
                    ' ---------------------------------------------------------------
                    ' If the call to LoadCulturalPagePropertiesFromDb produced results
                    ' we can now retrieve the data. Otherwise return empty string.
                    ' 
                    If MultilingualMetaDescription.ContainsKey(strHashKey) Then _
                        strWrk = MultilingualMetaDescription(strHashKey)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Private Function LoadCulturalPagePropertiesFromDb_SP(ByVal LCID As Integer) As ErrorObj
        Try
            '---------------------------------------------------------------------------------------
            '   Retrieve the properties of the page requested qualified by the locale ID LCID. 
            '   Properties are stored in their respective hashtables keyed as <LCID>_<page code>
            ' 
            Err = Sql2005Open()
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader
            Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & LCID.ToString & _uScore & KeyCode
            '------------------------------------------------------------------------------------------
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
                    Const SQLSelect As String = "EXEC spCMS_pagePropertiesCultural " & _
                                               " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                               " AND    PARTNER_CODE	= @Param2   " & _
                                               " AND    CULTURE_ID		= @Param3   " & _
                                               " AND    PAGE_CODE       = @Param4   "
                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = LCID
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                        dtr = .ExecuteReader()
                    End With
                    '------------------------------------------------------------------------------
                    If dtr.Read Then
                        '--------------------------------------------------------------------------
                        '   Unsynchronised so possible that multiple versions of the page will  
                        '   do this(causing) an exception adding an existing key. Catch the 
                        '   exception but do nothing
                        ' 
                        If Not MultilingualTitles.ContainsKey(strHashKey) Then
                            Try
                                MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If

                    End If
                    dtr.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-07"
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
        Return Err
    End Function
    Private Function LoadCulturalPagePropertiesFromDb_SP(ByVal languageCode As String) As ErrorObj
        Try
            '---------------------------------------------------------------------------------------
            '   Retrieve the properties of the page requested qualified by the locale ID LCID. 
            '   Properties are stored in their respective hashtables keyed as <LCID>_<page code>
            ' 
            Err = Sql2005Open()
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader
            Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & languageCode & _uScore & KeyCode
            '------------------------------------------------------------------------------------------
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
                    Const SQLSelect As String = "EXEC spCMS_pagePropertiesLanguage" & _
                                               " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                               " AND    PARTNER_CODE	= @Param2   " & _
                                               " AND    LANGUAGE_CODE	= @Param3   " & _
                                               " AND    PAGE_CODE       = @Param4   "
                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = languageCode
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                        dtr = .ExecuteReader()
                    End With
                    '------------------------------------------------------------------------------
                    If dtr.Read Then
                        '--------------------------------------------------------------------------
                        '   Unsynchronised so possible that multiple versions of the page will  
                        '   do this(causing) an exception adding an existing key. Catch the 
                        '   exception but do nothing
                        ' 
                        If Not MultilingualTitles.ContainsKey(strHashKey) Then
                            Try
                                MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If

                    End If
                    dtr.Close()
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-07"
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
        Return Err
    End Function
    Private Function LoadCulturalPagePropertiesFromDb(ByVal LCID As Integer) As ErrorObj
        '---------------------------------------------------------------------------------------
        '   Retrieve the properties of the page requested qualified by the locale ID LCID. 
        '   Properties are stored in their respective hashtables keyed as <LCID>_<page code>
        ' 
        '------------------------------------------------------------------------------------------
        '   Open sequel version
        '
        Err = Sql2005Open()
        '------------------------------------------------------------------------------------------
        Dim strWrk As String = String.Empty
        Dim cmdSelect As SqlCommand = Nothing
        Dim dtr As SqlDataReader
        Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & LCID.ToString & _uScore & KeyCode
        '------------------------------------------------------------------------------------------
        If Not Err.HasError Then
            Try
                Const SQLSelect As String = "SELECT * FROM tbl_page_lang WITH (NOLOCK)  " & _
                                               " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                               " AND    PARTNER_CODE	= @Param2   " & _
                                               " AND    CULTURE_ID		= @Param3   " & _
                                               " AND    PAGE_CODE       = @Param4   "
                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = LCID
                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                    dtr = .ExecuteReader()
                End With
                '------------------------------------------------------------------------------
                If dtr.Read Then
                    '--------------------------------------------------------------------------
                    '   Unsynchronised so possible that multiple versions of the page will  
                    '   do this(causing) an exception adding an existing key. Catch the 
                    '   exception but do nothing
                    ' 
                    If Not MultilingualTitles.ContainsKey(strHashKey) Then
                        Try
                            MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                    If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                        Try
                            MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                    '
                    If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                        Try
                            MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                    '
                    If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                        Try
                            MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                Else
                    dtr.Close()
                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = LCID
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                        dtr = .ExecuteReader()
                    End With
                    '------------------------------------------------------------------------------
                    If dtr.Read Then
                        If Not MultilingualTitles.ContainsKey(strHashKey) Then
                            Try
                                MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                            Try
                                MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                    Else
                        dtr.Close()
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = LCID
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            If Not MultilingualTitles.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                            If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                            '
                            If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                            '
                            If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                        Else
                            dtr.Close()
                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = "*ALL"
                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.BigInt)).Value = LCID
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = "*ALL"
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                If Not MultilingualTitles.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                                '
                                If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                                '
                                If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                            End If
                        End If
                    End If
                End If
                '------------------------------------------------------------------------------
                dtr.Close()
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TTPWFRE-01"
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
        Return Err

    End Function
    Private Function LoadCulturalPagePropertiesFromDb(ByVal languageCode As String) As ErrorObj
        '---------------------------------------------------------------------------------------
        '   Retrieve the properties of the page requested qualified by the locale ID LCID. 
        '   Properties are stored in their respective hashtables keyed as <LCID>_<page code>
        ' 
        '------------------------------------------------------------------------------------------
        '   Open sequel version
        '
        Err = Sql2005Open()
        '------------------------------------------------------------------------------------------
        Dim strWrk As String = String.Empty
        Dim cmdSelect As SqlCommand = Nothing
        Dim dtr As SqlDataReader
        Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & languageCode & _uScore & KeyCode
        '------------------------------------------------------------------------------------------
        If Not Err.HasError Then
            Try
                Const SQLSelect As String = "SELECT * FROM tbl_page_lang WITH (NOLOCK)  " & _
                                               " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                               " AND    PARTNER_CODE	= @Param2   " & _
                                               " AND    LANGUAGE_CODE	= @Param3   " & _
                                               " AND    PAGE_CODE       = @Param4   "
                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = languageCode
                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                    dtr = .ExecuteReader()
                End With
                '------------------------------------------------------------------------------
                If dtr.Read Then
                    '--------------------------------------------------------------------------
                    '   Unsynchronised so possible that multiple versions of the page will  
                    '   do this(causing) an exception adding an existing key. Catch the 
                    '   exception but do nothing
                    ' 
                    If Not MultilingualTitles.ContainsKey(strHashKey) Then
                        Try
                            MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                    If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                        Try
                            MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                    '
                    If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                        Try
                            MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                    '
                    If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                        Try
                            MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                Else
                    dtr.Close()
                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = languageCode
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                        dtr = .ExecuteReader()
                    End With
                    '------------------------------------------------------------------------------
                    If dtr.Read Then
                        If Not MultilingualTitles.ContainsKey(strHashKey) Then
                            Try
                                MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                            Try
                                MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                        '
                        If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                            Try
                                MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
                        End If
                    Else
                        dtr.Close()
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = languageCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = "*ALL"
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            If Not MultilingualTitles.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                            If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                            '
                            If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                            '
                            If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                                Try
                                    MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                                Catch ex As Exception
                                    Err.ErrorMessage = ex.Message
                                End Try
                            End If
                        Else
                            dtr.Close()
                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = "*ALL"
                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = languageCode
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = PageCode
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                If Not MultilingualTitles.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                                If Not MultilingualHeaderTexts.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualHeaderTexts.Add(strHashKey, dtr.Item("PAGE_HEADER"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                                '
                                If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                                '
                                If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                                    Try
                                        MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                                    Catch ex As Exception
                                        Err.ErrorMessage = ex.Message
                                    End Try
                                End If
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = "*ALL"
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = "*ALL"
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = languageCode
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = "*ALL"
                                    dtr = .ExecuteReader()
                                End With
                                '------------------------------------------------------------------------------
                                If dtr.Read Then
                                    If Not MultilingualTitles.ContainsKey(strHashKey) Then
                                        Try
                                            MultilingualTitles.Add(strHashKey, dtr.Item("TITLE"))
                                        Catch ex As Exception
                                            Err.ErrorMessage = ex.Message
                                        End Try
                                    End If
                                    '
                                    If Not MultilingualMetaKeywords.ContainsKey(strHashKey) Then
                                        Try
                                            MultilingualMetaKeywords.Add(strHashKey, dtr.Item("META_KEY"))
                                        Catch ex As Exception
                                            Err.ErrorMessage = ex.Message
                                        End Try
                                    End If
                                    '
                                    If Not MultilingualMetaDescription.ContainsKey(strHashKey) Then
                                        Try
                                            MultilingualMetaDescription.Add(strHashKey, dtr.Item("META_DESC"))
                                        Catch ex As Exception
                                            Err.ErrorMessage = ex.Message
                                        End Try
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
                '------------------------------------------------------------------------------
                dtr.Close()
            Catch ex As Exception
                Const strError8 As String = "Error during database access"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8
                    .ErrorNumber = "TTPWFRE-01"
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
        Return Err

    End Function

    Public ReadOnly Property Content(ByVal strKey As String, ByVal LCID As Integer, ByVal fromCache As Boolean) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '----------------------------------------------------------
                ' get the appropriate culture specific cache
                '
                If fromCache Then
                    Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & KeyCode & PageCode & strKey & LCID

                    If MultiLingualContent.ContainsKey(LCID.ToString) Then
                        WfrHashtable = MultiLingualContent(LCID.ToString)
                    Else
                        WfrHashtable = New Hashtable
                        MultiLingualContent(LCID.ToString) = WfrHashtable
                    End If
                    '----------------------------------------------------------
                    If WfrHashtable.ContainsKey(strHashKey) Then
                        strWrk = WfrHashtable(strHashKey)
                    Else
                        Select Case CMSUseStoredProcedures
                            Case Is = True
                                strWrk = ContentFromDb_SP(strKey, LCID)
                            Case Is = False
                                strWrk = ContentFromDb(strKey, LCID)
                        End Select
                        WfrHashtable(strHashKey) = strWrk
                    End If
                    '----------------------------------------------------------
                Else
                    strWrk = ContentFromDb(strKey, LCID)
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property
    Public ReadOnly Property Content(ByVal strKey As String, ByVal languageCode As String, ByVal fromCache As Boolean) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                '----------------------------------------------------------
                ' get the appropriate culture specific cache
                '
                If fromCache Then
                    Dim strHashKey As String = WFR_CACHE_NAME & BusinessUnit & PartnerCode & KeyCode & PageCode & strKey & languageCode

                    If MultiLingualContent.ContainsKey(languageCode) Then
                        WfrHashtable = MultiLingualContent(languageCode)
                    Else
                        WfrHashtable = New Hashtable
                        MultiLingualContent(languageCode) = WfrHashtable
                    End If
                    '----------------------------------------------------------
                    If WfrHashtable.ContainsKey(strHashKey) Then
                        strWrk = WfrHashtable(strHashKey)
                    Else
                        Select Case CMSUseStoredProcedures
                            Case Is = True
                                strWrk = ContentFromDb_SP(strKey, languageCode)
                            Case Is = False
                                strWrk = ContentFromDb(strKey, languageCode)
                        End Select
                        WfrHashtable(strHashKey) = strWrk
                    End If
                    '----------------------------------------------------------
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
                        Const strCmd As String = "EXEC spCMS_getPageContentByCulture " & _
                                                " @Param1, @Param2, @Param3, @Param4 , @Param5  "
                        cmdSelect = New SqlCommand(strCmd, conSql2005)
                        '
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.SmallInt)).Value = LCID
                            dtr = .ExecuteReader
                        End With
                        '
                        If dtr.Read Then strWrk = dtr("TEXTCONTENT")
                        dtr.Close()
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
                        Const strCmd As String = "EXEC spCMS_getPageContentByLanguage" & _
                                                " @Param1, @Param2, @Param3, @Param4 , @Param5  "
                        cmdSelect = New SqlCommand(strCmd, conSql2005)
                        '
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = PageCode
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = languageCode
                            dtr = .ExecuteReader
                        End With
                        '
                        If dtr.Read Then strWrk = dtr("TEXTCONTENT")
                        dtr.Close()
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
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        Const SQLSelect As String = "SELECT TEXT_CONTENT FROM tbl_page_text_lang WITH (NOLOCK)  " & _
                                                       " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                       " AND    PARTNER_CODE	= @Param2   " & _
                                                       " AND    TEXT_CODE	    = @Param3   " & _
                                                       " AND    CULTURE_ID		= @Param4   " & _
                                                       " AND    PAGE_CODE       = @Param5   "
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = LCID
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = PageCode
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            strWrk = dtr.Item("TEXT_CONTENT")
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
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = LCID
                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = PageCode
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                strWrk = dtr.Item("TEXT_CONTENT")
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.BigInt)).Value = LCID
                                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = PageCode
                                    dtr = .ExecuteReader()
                                End With
                                If dtr.Read Then strWrk = dtr.Item("TEXT_CONTENT")
                            End If
                        End If
                        dtr.Close()
                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With Err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "TTPWFRE-01"
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
                '------------------------------------------------------------------------------------------
                Dim cmdSelect As SqlCommand = Nothing
                Dim dtr As SqlDataReader
                '------------------------------------------------------------------------------------------
                If Not Err.HasError Then
                    Try
                        Const SQLSelect As String = "SELECT TEXT_CONTENT FROM tbl_page_text_lang WITH (NOLOCK)  " & _
                                                       " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                       " AND    PARTNER_CODE	= @Param2   " & _
                                                       " AND    TEXT_CODE	    = @Param3   " & _
                                                       " AND    LANGUAGE_CODE	= @Param4   " & _
                                                       " AND    PAGE_CODE       = @Param5   "
                        cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                        With cmdSelect
                            .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                            .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = languageCode
                            .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = PageCode
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            strWrk = dtr.Item("TEXT_CONTENT")
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
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = languageCode
                                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = PageCode
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                strWrk = dtr.Item("TEXT_CONTENT")
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = languageCode
                                    .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = Utilities.GetAllString
                                    dtr = .ExecuteReader()
                                End With
                                '------------------------------------------------------------------------------
                                If dtr.Read Then
                                    strWrk = dtr.Item("TEXT_CONTENT")
                                Else
                                    dtr.Close()
                                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                    With cmdSelect
                                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = strKey
                                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = languageCode
                                        .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = PageCode
                                        dtr = .ExecuteReader()
                                    End With
                                    '------------------------------------------------------------------------------
                                    If dtr.Read Then strWrk = dtr.Item("TEXT_CONTENT")
                                End If
                            End If
                        End If
                        dtr.Close()
                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With Err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "TTPWFRE-01"
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
            Dim strWrk As String = String.Empty
            Dim strHashKey As String = BusinessUnit & PartnerCode & PageCode & strKey
            Try
                ' ----------------------------------------------------------
                '   Return a webform attribute. If the _Attributes cache. If
                '   the key does not exist, attempt to retrieve the value from
                '   the database, if found add the key/value to the local cache
                '   and return
                '  

                If AttHashtable.ContainsKey(strHashKey) Then
                    strWrk = AttHashtable(strHashKey)
                Else
                    Select Case CMSUseStoredProcedures
                        Case Is = True
                            strWrk = AttributeFromDB_SP(strKey)
                        Case Is = False
                            strWrk = AttributeFromDB(strKey)
                    End Select

                    If strWrk.Length > 0 Then
                        ' --------------------------------------
                        '   Unsynchronised so possible that multiple
                        '   versions of the page will do this causing
                        '   an exception adding an existing key.
                        '   Catch the exception but do nothing
                        '  
                        Try
                            AttHashtable.Add(strHashKey, strWrk)
                        Catch ex As Exception
                            Err.ErrorMessage = ex.Message
                        End Try
                    End If
                End If
            Catch ex As Exception
            End Try
            Return strWrk
        End Get
    End Property

    ''' <summary>
    ''' Gets the Attribute value from bl_page_attribute table 
    ''' </summary>
    ''' <param name="strKey">Key for which value is fetched.</param>
    ''' <param name="fromCache"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Overload property to get the value from the database or from cache. This method uses the language
    ''' in the hashkey. It may be discarded if we don't want to use the overload property.</remarks>
    Public ReadOnly Property Attribute(ByVal strKey As String, ByVal languageCode As String, ByVal fromCache As Boolean) As String
        Get
            Dim strWrk As String = String.Empty
            Try
                If fromCache Then
                    Dim strHashKey As String = BusinessUnit & PartnerCode & PageCode & strKey & languageCode

                    ' ----------------------------------------------------------
                    '   Return a webform attribute. If the _Attributes cache. If
                    '   the key does not exist, attempt to retrieve the value from
                    '   the database, if found add the key/value to the local cache
                    '   and return
                    '  
                    If MultiLingualContent.ContainsKey(languageCode) Then
                        AttHashtable = MultiLingualContent(languageCode)
                    Else
                        AttHashtable = New Hashtable
                        MultiLingualContent(languageCode) = AttHashtable
                    End If

                    If AttHashtable.ContainsKey(strHashKey) Then
                        strWrk = AttHashtable(strHashKey)
                    Else
                        Select Case CMSUseStoredProcedures
                            Case Is = True
                                strWrk = AttributeFromDB_SP(strKey)
                            Case Is = False
                                strWrk = AttributeFromDB(strKey)
                        End Select

                        If strWrk.Length > 0 Then
                            ' --------------------------------------
                            '   Unsynchronised so possible that multiple
                            '   versions of the page will do this causing
                            '   an exception adding an existing key.
                            '   Catch the exception but do nothing
                            '  
                            Try
                                AttHashtable.Add(strHashKey, strWrk)
                            Catch ex As Exception
                                Err.ErrorMessage = ex.Message
                            End Try
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
        Get '------------------------------------------------------------------------------------------
            '   Open stored procedure version
            '
            Err = Sql2005Open()
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader
            '------------------------------------------------------------------------------------------
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
                    Const strCmd1 As String = "EXEC spCMS_getPageAttribute " & _
                                                " @Param1, @Param2, @Param3, @Param4  "
                    cmdSelect = New SqlCommand(strCmd1, conSql2005)
                    '
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = PageCode
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                        dtr = .ExecuteReader
                    End With

                    If dtr.Read Then strWrk = dtr("ATTRVALUE")
                    '------------------------------------------------------------------------------
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-02"
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
            Return strWrk
        End Get
    End Property
    Private ReadOnly Property AttributeFromDB(ByVal strKey As String) As String
        Get '------------------------------------------------------------------------------------------
            '   Open sequel version
            '
            '
            Err = Sql2005Open()
            '------------------------------------------------------------------------------------------
            Dim strWrk As String = String.Empty
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtr As SqlDataReader
            Const SQLSelect As String = "SELECT ATTR_VALUE  FROM tbl_page_attribute WITH (NOLOCK)      " & _
                                                   " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                   " AND    PARTNER_CODE	= @Param2   " & _
                                                   " AND    PAGE_CODE	    = @Param3   " & _
                                                   " AND    ATTR_NAME		= @Param4   "
            '------------------------------------------------------------------------------------------
            If Not Err.HasError Then
                Try
                    cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                    With cmdSelect
                        .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                        .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                        .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                        dtr = .ExecuteReader()
                    End With
                    '------------------------------------------------------------------------------
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
                            .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                            dtr = .ExecuteReader()
                        End With
                        '------------------------------------------------------------------------------
                        If dtr.Read Then
                            strWrk = dtr.Item("ATTR_VALUE")
                        Else
                            dtr.Close()
                            cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                            With cmdSelect
                                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                                .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                                dtr = .ExecuteReader()
                            End With
                            '------------------------------------------------------------------------------
                            If dtr.Read Then
                                strWrk = dtr.Item("ATTR_VALUE")
                            Else
                                dtr.Close()
                                cmdSelect = New SqlCommand(SQLSelect, conSql2005)
                                With cmdSelect
                                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = Utilities.GetAllString
                                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = KeyCode
                                    .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = strKey
                                    dtr = .ExecuteReader()
                                End With
                                '------------------------------------------------------------------------------
                                If dtr.Read Then strWrk = dtr.Item("ATTR_VALUE")
                            End If
                        End If
                    End If
                    dtr.Close()
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With Err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TTPWFRE-03"
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
            Return strWrk
        End Get
    End Property

    Public Function RefreshPageProperties(ByVal LCID As Integer) As ErrorObj
        ' --------------------------------------------------------------------------------------------------
        ' Remove any cached properties for thie page and culture, reload from db.
        ' 
        Dim strHashKey As String = WFR_CACHE_NAME & LCID.ToString & _uScore & KeyCode
        Try
            MultilingualTitles.Remove(strHashKey)
        Catch ex As Exception
            Err.ErrorMessage = ex.Message
        End Try
        Try
            MultilingualMetaKeywords.Remove(strHashKey)
        Catch ex As Exception
            Err.ErrorMessage = ex.Message
        End Try
        Try
            MultilingualMetaDescription.Remove(strHashKey)
        Catch ex As Exception
            Err.ErrorMessage = ex.Message
        End Try
        Err = LoadCulturalPagePropertiesFromDb(LCID)
        Return Err
    End Function
    Public Function RefreshPageProperties(ByVal languageCode As String) As ErrorObj
        ' --------------------------------------------------------------------------------------------------
        ' Remove any cached properties for thie page and culture, reload from db.
        ' 
        Dim strHashKey As String = WFR_CACHE_NAME & languageCode & _uScore & KeyCode
        Try
            MultilingualTitles.Remove(strHashKey)
        Catch ex As Exception
            Err.ErrorMessage = ex.Message
        End Try
        Try
            MultilingualMetaKeywords.Remove(strHashKey)
        Catch ex As Exception
            Err.ErrorMessage = ex.Message
        End Try
        Try
            MultilingualMetaDescription.Remove(strHashKey)
        Catch ex As Exception
            Err.ErrorMessage = ex.Message
        End Try
        Err = LoadCulturalPagePropertiesFromDb(languageCode)
        Return Err
    End Function

End Class
