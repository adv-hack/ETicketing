Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDB
Imports System.Text
Public Class DataTransfer

    Public Shared _TDataObjects As New Talent.Common.TalentDataObjects

    Private Shared _SQLConnectionString As String
    Public Shared Property SQLConnectionString()
        Get
            Return _SQLConnectionString
        End Get
        Set(ByVal value)
            _SQLConnectionString = value
        End Set
    End Property

    Private Shared _OleDbConnectionString As String
    Public Shared Property OleDbConnectionString()
        Get
            Return _OleDbConnectionString
        End Get
        Set(ByVal value)
            _OleDbConnectionString = value
        End Set
    End Property

    Private Shared _stockLocation As String = "AL"
    Public Shared Property StockLocation() As String
        Get
            Return _stockLocation
        End Get
        Set(ByVal value As String)
            _stockLocation = value
        End Set
    End Property

    Private Shared _defaultSQLTimout As Integer = 36000
    Public Shared Property DefaultSQLTimeout() As Integer
        Get
            Return _defaultSQLTimout
        End Get
        Set(ByVal value As Integer)
            _defaultSQLTimout = value
        End Set
    End Property

    Private Shared _defaultOleDBTimout As Integer = 36000
    Public Shared Property DefaultOleDBTimeout() As Integer
        Get
            Return _defaultOleDBTimout
        End Get
        Set(ByVal value As Integer)
            _defaultOleDBTimout = value
        End Set
    End Property

    Private Shared _businessUnit As String = String.Empty
    Public Shared Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property

    Public Shared Function BuildUpdateSQL(ByVal updateTableName As String, _
                                        ByVal sourceTableName As String, _
                                        ByVal fieldMappings As Collections.Generic.Dictionary(Of String, String), _
                                        ByVal keyFields As Collections.Generic.Dictionary(Of String, String) _
                                        ) As StringBuilder

        Dim count As Integer = 0
        Dim update As New StringBuilder

        With update
            .Append("UPDATE " & updateTableName & " ")
            .Append("SET ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key & " = RTrim(( " & sourceTableName & "." & fieldMappings(key) & " )) ")
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append("	FROM " & sourceTableName & " ")
            .Append("		WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append("   " & sourceTableName & "." & keyFields(key) & " = " & updateTableName & "." & key & " ")
            Next

            .Append("		AND EXISTS ")
            .Append("			( ")
            .Append("				SELECT * ")
            .Append("				FROM " & updateTableName & "  ")
            .Append("				WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append("   " & updateTableName & "." & key & " = " & sourceTableName & "." & keyFields(key) & " ")
            Next

            .Append("			) ")
        End With

        Return update
    End Function

    Private Shared Function BuildPartialUpdateSQL(ByVal updateTableName As String, _
                                            ByVal fieldMappings As Collections.Generic.Dictionary(Of String, String), _
                                            ByVal keyFields As Collections.Generic.Dictionary(Of String, String), _
                                            ByVal r As DataRow) As StringBuilder

        Dim count As Integer = 0
        Dim update As New StringBuilder

        With update
            .Append("SET DATEFORMAT DMY ")
            .Append("UPDATE " & updateTableName & " ")
            .Append("SET ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key & " = " & "'" & r(fieldMappings(key)) & "'")
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append("		WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append("   " & keyFields(key) & " = " & "'" & r(keyFields(key)) & "'")
            Next

            .Append("		AND EXISTS ")
            .Append("			( ")
            .Append("				SELECT * ")
            .Append("				FROM " & updateTableName & "  ")
            .Append("				WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append("   " & key & " = " & "'" & r(keyFields(key)) & "'")
            Next

            .Append("			) ")
        End With

        Return update
    End Function

    Public Shared Function BuildInsertSQL(ByVal insertTableName As String, _
                                        ByVal sourceTableName As String, _
                                        ByVal fieldMappings As Collections.Generic.Dictionary(Of String, String), _
                                        ByVal keyFields As Collections.Generic.Dictionary(Of String, String) _
                                        ) As StringBuilder

        Dim insert As New StringBuilder
        Dim count As Integer = 0

        With insert
            'Add the INSERT columns            

            .Append("INSERT INTO " & insertTableName & "( ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key)
                If count = fieldMappings.Keys.Count Then .Append("   ) ") Else .Append("   , ")
            Next

            'Add the SOURCE columns
            .Append("   SELECT ")

            count = 0 'reset the counter
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("           RTRIM(" & sourceTableName & "." & fieldMappings(key) & ") AS Expr" & count)
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append("   FROM " & sourceTableName)
            .Append(" WHERE NOT EXISTS (")
            .Append("              SELECT * ")
            .Append("              FROM " & insertTableName & " ")
            .Append("              WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append("   " & insertTableName & "." & key & " = " & sourceTableName & "." & keyFields(key) & " ")
            Next
            .Append(" )")
        End With

        Return insert
    End Function

    Private Shared Function BuildPartialInsertSQL(ByVal insertTableName As String, _
                                           ByVal fieldMappings As Collections.Generic.Dictionary(Of String, String), _
                                           ByVal keyFields As Collections.Generic.Dictionary(Of String, String), _
                                           ByVal r As DataRow) As StringBuilder

        Dim insert As New StringBuilder
        Dim count As Integer = 0

        With insert
            'Add the INSERT columns
            .Append("SET DATEFORMAT DMY ")
            .Append(" IF NOT EXISTS (")
            .Append("              SELECT * ")
            .Append("              FROM " & insertTableName & " ")
            .Append("              WHERE ")
            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append("   " & key & " = " & "'" & r(keyFields(key)) & "'" & " ")
            Next
            .Append(")")

            count = 0 'reset the counter
            .Append("INSERT INTO " & insertTableName & "( ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key)
                If count = fieldMappings.Keys.Count Then .Append("   ) ") Else .Append("   , ")
            Next

            'Add the SOURCE columns
            .Append("   VALUES (")

            count = 0 'reset the counter
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("'" & r(fieldMappings(key)) & "'")
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append(")")

        End With

        Return insert
    End Function

    Private Shared Function BuildReplaceInsertSQL(ByVal insertTableName As String, _
                                    ByVal sourceTableName As String, _
                                    ByVal fieldMappings As Collections.Generic.Dictionary(Of String, String), _
                                    ByVal keyFields As Collections.Generic.Dictionary(Of String, String) _
                                    ) As StringBuilder

        Dim insert As New StringBuilder
        Dim count As Integer = 0

        With insert
            'Add the INSERT columns            
            .Append("INSERT INTO " & insertTableName & "( ")
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("   " & key)
                If count = fieldMappings.Keys.Count Then .Append("   ) ") Else .Append("   , ")
            Next

            'Add the SOURCE columns
            .Append("   SELECT ")

            count = 0 'reset the counter
            For Each key As String In fieldMappings.Keys
                count += 1
                .Append("           RTRIM(" & sourceTableName & "." & fieldMappings(key) & ") AS Expr" & count)
                If Not count = fieldMappings.Keys.Count Then .Append("   , ")
            Next
            .Append("   FROM " & sourceTableName)
        End With

        Return insert
    End Function

    Public Shared Function BuildDeleteSQL(ByVal deleteTableName As String, _
                                       ByVal sourceTableName As String, _
                                       ByVal keyFields As Collections.Generic.Dictionary(Of String, String) _
                                       ) As StringBuilder

        Dim delete As New StringBuilder
        Dim count As Integer = 0

        With delete
            .Append("DELETE FROM " & deleteTableName)
            .Append("   WHERE NOT EXISTS (")
            .Append("           SELECT * ")
            .Append("               FROM " & sourceTableName)
            .Append("               WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append(" " & sourceTableName & "." & key & " = " & deleteTableName & "." & keyFields(key))
            Next

            .Append(" )")
        End With

        Return delete
    End Function

    Public Shared Function BuildDeleteSQL_IgnoreAdhoc(ByVal deleteTableName As String, _
                                       ByVal sourceTableName As String, _
                                       ByVal keyFields As Collections.Generic.Dictionary(Of String, String), _
                                     ByVal adhocField As String) As StringBuilder

        Dim delete As New StringBuilder
        Dim count As Integer = 0

        With delete
            .Append("DELETE FROM " & deleteTableName)
            .Append("   WHERE NOT EXISTS (")
            .Append("           SELECT * ")
            .Append("               FROM " & sourceTableName)
            .Append("               WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append(" " & sourceTableName & "." & key & " = " & deleteTableName & "." & keyFields(key))
            Next
            .Append(" )")
            .Append(" AND (").Append(adhocField).Append(" <> 'True' OR ").Append(adhocField).Append(" IS NULL)")
        End With

        Return delete
    End Function

    Public Shared Function BuildDeleteSQL_IgnoreAdhocPromotions(ByVal deleteTableName As String, _
                                        ByVal sourceTableName As String, _
                                        ByVal keyFields As Collections.Generic.Dictionary(Of String, String), _
                                        ByVal adhocField As String, _
                                        ByVal adhocTable As String, _
                                        ByVal adhocKeyFields As Collections.Generic.Dictionary(Of String, String)) As StringBuilder

        Dim delete As New StringBuilder
        Dim count As Integer = 0

        'DELETE FROM [tbl_promotions_discounts] 
        'WHERE NOT EXISTS
        '(
        '	SELECT * FROM [tbl_promotions_discounts_work]
        '	WHERE tbl_promotions_discounts_work.business_unit = tbl_promotions_discounts.business_unit
        '	AND tbl_promotions_discounts_work.PARTNER_GROUP = tbl_promotions_discounts.PARTNER_GROUP
        '	AND tbl_promotions_discounts_work.PARTNER = tbl_promotions_discounts.PARTNER
        '	AND tbl_promotions_discounts_work.PROMOTION_CODE = tbl_promotions_discounts.PROMOTION_CODE
        ')
        'AND 
        '(
        '	SELECT COUNT(*)
        '	FROM tbl_promotions 
        '	WHERE tbl_promotions.PROMOTION_CODE = tbl_promotions_discounts.PROMOTION_CODE
        '	AND tbl_promotions.PARTNER_GROUP = tbl_promotions_discounts.PARTNER_GROUP
        '	AND tbl_promotions.PARTNER = tbl_promotions_discounts.PARTNER
        '	AND tbl_promotions.BUSINESS_UNIT = tbl_promotions_discounts.BUSINESS_UNIT
        '	AND isnull(tbl_promotions.ADHOC_PROMOTION,0) = 0
        ') > 0

        With delete
            .Append("DELETE FROM " & deleteTableName & " WHERE NOT EXISTS ")
            .Append(" ( ")
            .Append("	SELECT * FROM " & sourceTableName)
            .Append("   WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append("   AND ")
                .Append(" " & sourceTableName & "." & key & " = " & deleteTableName & "." & keyFields(key))
            Next

            .Append(" ) ")
            .Append("AND")
            .Append(" ( ")
            .Append("   SELECT COUNT(*) FROM " & adhocTable)
            .Append("   WHERE ")

            count = 0
            For Each key As String In adhocKeyFields.Keys
                count += 1
                If count > 1 Then .Append("   AND ")
                .Append(" " & adhocTable & "." & key & " = " & deleteTableName & "." & adhocKeyFields(key))
            Next
            If count > 1 Then .Append("   AND ")
            .Append("ISNULL(" & adhocTable & "." & adhocField & ",0) = 0")
            .Append(" ) > 0")
        End With

        Return delete
    End Function
    Private Shared Function BuildDeleteSQL_BU_Partner(ByVal deleteTableName As String, _
                                       ByVal sourceTableName As String, _
                                       ByVal keyFields As Collections.Generic.Dictionary(Of String, String), _
                                       ByVal business_unit As String, _
                                       ByVal partner As String, _
                                       ByVal business_unit_field As String, _
                                       ByVal partner_field As String) As StringBuilder

        Dim delete As New StringBuilder
        Dim count As Integer = 0

        With delete
            .Append("DELETE FROM " & deleteTableName)
            .Append("   WHERE NOT EXISTS (")
            .Append("           SELECT * ")
            .Append("               FROM " & sourceTableName)
            .Append("               WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append(" " & sourceTableName & "." & key & " = " & deleteTableName & "." & keyFields(key))
            Next

            .Append(" )")
            .Append(" AND ")
            .Append(business_unit_field).Append(" = '").Append(business_unit).Append("'  AND ")
            .Append(partner_field).Append(" = '").Append(partner).Append("'")


        End With

        Return delete
    End Function

    Private Shared Function BuildPartialDeleteSQL(ByVal deleteTableName As String, _
                                       ByVal keyFields As Collections.Generic.Dictionary(Of String, String), _
                                       ByVal r As DataRow) As StringBuilder

        Dim delete As New StringBuilder
        Dim count As Integer = 0

        With delete
            .Append("DELETE FROM " & deleteTableName)
            .Append("   WHERE ")

            count = 0
            For Each key As String In keyFields.Keys
                count += 1
                If count > 1 Then .Append(" AND ")
                .Append(" " & key & " = " & "'" & r(keyFields(key)) & "'")
            Next

        End With

        Return delete
    End Function
#Region "2/. DoExtract()"

    '===================================================================================================
    '
    '   2/. DoExtract()
    '
    '===================================================================================================

    Public Shared Sub DoExtract_All(ByVal replace As Boolean, ByVal updateProductDescriptions As Boolean)

        DoExtract_EBGPPR()
        DoExtract_EBGL01()
        DoExtract_EBGL02()
        DoExtract_EBGL03()
        DoExtract_EBGL04()
        DoExtract_EBGL05()
        DoExtract_EBGL06()
        DoExtract_EBGL07()
        DoExtract_EBGL08()
        DoExtract_EBGL09()
        DoExtract_EBGL10()
        DoExtract_EBPROD(updateProductDescriptions)
        DoExtract_EBPRRL()
        DoExtract_EBPLDT(replace)
        DoExtract_EBPLHD()
        DoExtract_EBGROU()
        DoExtract_EBPRST()
        DoExtract_EBORDDT()
        DoExtract_EBORDHT()
        DoExtract_EBINVHT()
        DoExtract_EBPRDO()
        DoExtract_EBPRDD()
        DoExtract_EBCNEHT()
        DoExtract_EBOTHD()
        DoExtract_EBOTDT()
        DoExtract_EBPM()
        DoExtract_EBPML()
        DoExtract_EBPMD()
        DoExtract_EBPMR()
        DoExtract_EBPMF()
    End Sub

    Public Shared Sub DoExtract_EBGPPR()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGPPR"
        ExtractTable = "tbl_group_product_work"
        DestinationTable = "dbo.tbl_group_product"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGPPR()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL01()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL01"
        ExtractTable = "tbl_group_level_01_work"
        DestinationTable = "dbo.tbl_group_level_01"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL01()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL02()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL02"
        ExtractTable = "tbl_group_level_02_work"
        DestinationTable = "dbo.tbl_group_level_02"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL02()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL03()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL03"
        ExtractTable = "tbl_group_level_03_work"
        DestinationTable = "dbo.tbl_group_level_03"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL03()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL04()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL04"
        ExtractTable = "tbl_group_level_04_work"
        DestinationTable = "dbo.tbl_group_level_04"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL04()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL05()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL05"
        ExtractTable = "tbl_group_level_05_work"
        DestinationTable = "dbo.tbl_group_level_05"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL05()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL06()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL06"
        ExtractTable = "tbl_group_level_06_work"
        DestinationTable = "dbo.tbl_group_level_06"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL06()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGL07()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL07"
        ExtractTable = "tbl_group_level_07_work"
        DestinationTable = "dbo.tbl_group_level_07"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL07()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

    End Sub

    Public Shared Sub DoExtract_EBGL08()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL08"
        ExtractTable = "tbl_group_level_08_work"
        DestinationTable = "dbo.tbl_group_level_08"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL08()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

    End Sub

    Public Shared Sub DoExtract_EBGL09()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL09"
        ExtractTable = "tbl_group_level_09_work"
        DestinationTable = "dbo.tbl_group_level_09"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL09()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

    End Sub

    Public Shared Sub DoExtract_EBGL10()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGL10"
        ExtractTable = "tbl_group_level_10_work"
        DestinationTable = "dbo.tbl_group_level_10"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGL10()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

    End Sub

    Public Shared Sub DoExtract_EBPROD(ByVal updateProductDescriptions As Boolean)

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPROD"
        ExtractTable = "tbl_product_work"
        DestinationTable = "dbo.tbl_product"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPROD(updateProductDescriptions)
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPRDO()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPRDO"
        ExtractTable = "tbl_product_options_work"
        DestinationTable = "dbo.tbl_product_options"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPRDO()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPRDD()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPRDD"
        ExtractTable = "tbl_product_option_defaults_work"
        DestinationTable = "dbo.tbl_product_option_defaults"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPRDD()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBUSERT(ByVal DestinationDataBase As String, ByVal CreatePartnerOnAddCustomer As Boolean)

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBUSERT"
        ExtractTable = "tbl_user_work"
        DestinationTable = "dbo.tbl_user"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBUSERT(DestinationDataBase, CreatePartnerOnAddCustomer)
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPRRL()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPRRL"
        ExtractTable = "tbl_product_relations_work"
        DestinationTable = "dbo.tbl_product_relations"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPRRL()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPLDT(ByVal replace As Boolean)

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPLDT"
        ExtractTable = "tbl_price_list_detail_work"
        DestinationTable = "dbo.tbl_price_list_detail"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPLDT(replace)
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPLDTP()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPLDTP"
        ExtractTable = "tbl_price_list_detail_work"
        DestinationTable = "dbo.tbl_price_list_detail"

        MarkSource(SourceTable)

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPLDTP()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

        DeleteSource(SourceTable)

    End Sub

    Public Shared Sub DoExtract_EBPLHD()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPLHD"
        ExtractTable = "tbl_price_list_header_work"
        DestinationTable = "dbo.tbl_price_list_header"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPLHD()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBGROU()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBGROU"
        ExtractTable = "tbl_group_work"
        DestinationTable = "dbo.tbl_group"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBGROU()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPRST()


        'Setup a timer to time out the request after x milliseconds
        '----------------------------------------------------------
        Dim tim As New System.Timers.Timer
        tim.Interval = 1500000
        tim.AutoReset = False
        AddHandler tim.Elapsed, AddressOf handleTimer
        tim.Start()
        '----------------------------------------------------------


        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPRST"
        ExtractTable = "tbl_product_stock_work"
        DestinationTable = "dbo.tbl_product_stock"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPRST()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

        'Try
        '    SetStockLocationToAL()
        'Catch ex As Exception
        'End Try
    End Sub

    Public Shared Sub handleTimer(ByVal sender As Object, ByVal e As Timers.ElapsedEventArgs)
        System.Threading.Thread.CurrentThread.Abort()
    End Sub

    ' this sets the product stock location to the same location (from defaults) for every product
    ' this has caused problem with norwich, so is removed
    Public Shared Sub SetStockLocationToAL()
        If Not String.IsNullOrEmpty(StockLocation()) Then
            Dim cmd As New System.Data.SqlClient.SqlCommand
            cmd.Connection = New System.Data.SqlClient.SqlConnection(_SQLConnectionString)

            cmd.CommandText = "UPDATE tbl_product_stock SET STOCK_LOCATION = '" & StockLocation() & "'"
            cmd.CommandTimeout = DefaultSQLTimeout()

            Try
                cmd.Connection.Open()
                cmd.ExecuteNonQuery()
            Catch ex As Exception
            Finally
                cmd.Connection.Close()
            End Try
        End If
    End Sub

    Public Shared Sub DoExtract_EBORDDT()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBORDDT"
        ExtractTable = "tbl_order_detail_work"
        DestinationTable = "dbo.tbl_order_detail"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBORDDT()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBORDHT()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBORDHT"
        ExtractTable = "tbl_order_header_work"
        DestinationTable = "dbo.tbl_order_header"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBORDHT()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBINVHT()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBINVHT"
        ExtractTable = "tbl_invoice_header_work"
        DestinationTable = "dbo.tbl_invoice_header"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBINVHT()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBCNEHT()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBCNEHT"
        ExtractTable = "tbl_creditnote_header_work"
        DestinationTable = "dbo.tbl_creditnote_header"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBCNEHT()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBECMB()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBECMB"
        ExtractTable = "tbl_ecommerce_module_defaults_bu_work"
        DestinationTable = "dbo.tbl_ecommerce_module_defaults_bu"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBECMB()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBOTHD()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBOTHD"
        ExtractTable = "tbl_order_template_header_work"
        DestinationTable = "dbo.tbl_order_template_header"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBOTHD()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

    End Sub

    Public Shared Sub DoExtract_EBOTDT()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBOTDT"
        ExtractTable = "tbl_order_template_detail_work"
        DestinationTable = "dbo.tbl_order_template_detail"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBOTDT()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try

    End Sub

    Public Shared Sub DoExtract_EBPM()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPM"
        ExtractTable = "tbl_promotions_work"
        DestinationTable = "dbo.tbl_promotions"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPM()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPML()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPML"
        ExtractTable = "tbl_promotions_lang_work"
        DestinationTable = "dbo.tbl_promotions_lang"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPML()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPMD()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPMD"
        ExtractTable = "tbl_promotions_discounts_work"
        DestinationTable = "dbo.tbl_promotions_discounts"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPMD()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPMR()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPMR"
        ExtractTable = "tbl_promotions_required_products_work"
        DestinationTable = "dbo.tbl_promotions_required_products"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPMR()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

    Public Shared Sub DoExtract_EBPMF()

        Dim SourceTable As String
        Dim ExtractTable As String
        Dim DestinationTable As String
        Dim ExtractCompleted As Boolean = True

        '--------------------------------------------------------
        ' 2.0   OleDb to SQLwork to SQL
        '--------------------------------------------------------
        SourceTable = "EBPMF"
        ExtractTable = "tbl_promotions_free_products_work"
        DestinationTable = "dbo.tbl_promotions_free_products"

        Dim dataTransferStatusId As Integer = 0
        Dim affectedRows As Integer = 0
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = SQLConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = BusinessUnit
        _TDataObjects.Settings = settings
        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Insert(SourceTable, DataTransferStatusEnum.CLEARING_WORK_TABLE, dataTransferStatusId)
        Try
            ExtractCompleted = DoExtractTable(SourceTable, ExtractTable, DestinationTable, dataTransferStatusId)
            If ExtractCompleted Then
                DoUpdateTable_EBPMF()
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FINISHED, Date.Now, True)
                End If
            Else
                If affectedRows > 0 And dataTransferStatusId > 0 Then
                    affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                            DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, "Failed copying files from 'work' table")
                End If
            End If
        Catch ex As Exception
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, SourceTable, ex.Message)
            End If
        End Try
    End Sub

#End Region

#Region "3/. DoExtractTable(SourceTable, ExtractTable, DestinationTable)"

    '===================================================================================================
    '
    '   3/. DoExtractTable(SourceTable, ExtractTable, DestinationTable)
    '
    '===================================================================================================
    Private Shared Function DoExtractTable( _
        ByVal SourceTable As String, _
        ByVal ExtractTable As String, _
        ByVal DestinationTable As String, ByVal dataTransferStatusId As Integer)

        Dim ExtractCompleted As Boolean = True

        '======================================================
        ' DELETE ALL records from the EXTRACT Table
        '======================================================
        Dim sbDelete As New StringBuilder
        With sbDelete
            .Append("Delete from ")
            .Append(ExtractTable)
        End With

        'Insert record into tbl_data_transfer_status
        Dim affectedRows As Integer = 0
        Try
            'Dim connectionString As String = ConfigurationManager.ConnectionStrings("SQLConnectionString").ToString
            Dim connectionString As String = SQLConnectionString()
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(sbDelete.ToString(), con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using

            '======================================================
            ' COPY the SOURCE table to the EXTRACT table
            '======================================================
            'Update tbl_data_transfer_status
            If dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, DataTransferStatusEnum.EXTRACTING_FROM_ISERIES, Date.MinValue, False)
            End If

            Dim sbSelect As New StringBuilder
            sbSelect = buildExtractSQL(SourceTable, ExtractTable)

            ' Initialise the number of records copied to zero
            Dim RecordsCopied As Integer = 0

            ' Initialise Connection Objects
            Dim OleDbCon As OleDbConnection = New OleDbConnection()
            Dim SQLCon As SqlConnection = New SqlConnection()

            ' Initialise Command Objects
            Dim OleDbCmd As OleDbCommand = New OleDbCommand()
            Dim SQLCmd As SqlCommand = New SqlCommand()

            ' Initialise the Source Data Reader
            Dim OleDbReader As OleDbDataReader

            ' Attach the connection strings to the connection objects
            'OleDbCon.ConnectionString = ConfigurationManager.ConnectionStrings("OleDbConnectionString").ToString
            'SQLCon.ConnectionString = ConfigurationManager.ConnectionStrings("SQLConnectionString").ToString
            OleDbCon.ConnectionString = OleDbConnectionString()
            SQLCon.ConnectionString = SQLConnectionString()

            ' Attach the connection objects to the commands
            OleDbCmd.Connection = OleDbCon
            SQLCmd.Connection = SQLCon
            OleDbCmd.CommandTimeout = DefaultSQLTimeout()

            ' Set Source Command SQL
            OleDbCmd.CommandTimeout = DefaultOleDBTimeout()
            OleDbCmd.CommandText = sbSelect.ToString()
            OleDbCmd.CommandType = CommandType.Text

            ' Open the Source Connection
            OleDbCmd.Connection.Open()

            Dim SQLBulkOp As SqlBulkCopy
            'SQLBulkOp = New SqlBulkCopy(ConfigurationManager.ConnectionStrings("SQLConnectionString").ToString, SqlBulkCopyOptions.UseInternalTransaction)
            SQLBulkOp = New SqlBulkCopy(SQLConnectionString(), SqlBulkCopyOptions.UseInternalTransaction)

            ' Set Destination Table Name and Timeout
            SQLBulkOp.DestinationTableName = ExtractTable
            SQLBulkOp.BulkCopyTimeout = 500000000

            ' Execute the From Reader
            OleDbReader = OleDbCmd.ExecuteReader()

            Try
                SQLBulkOp.WriteToServer(OleDbReader)
            Catch ex As Exception
                ExtractCompleted = False
                'WriteLog(ex.Message)
            Finally
                OleDbReader.Close()
                OleDbCmd.Connection.Close()
            End Try

            ' Now check to see whether any records have been written to the work table
            Dim checkString As String = "SELECT COUNT(*) FROM " & ExtractTable
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(checkString, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                Dim numOfRows As String = String.Empty
                numOfRows = cmd.ExecuteScalar.ToString
                If numOfRows = 0 Then
                    ExtractCompleted = False
                Else
                    If affectedRows > 0 And dataTransferStatusId > 0 Then
                        affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                                DataTransferStatusEnum.UPDATING_SQL_TABLES, Date.MinValue, False, _
                                SourceTable, String.Empty, numOfRows)
                    End If
                End If
                con.Close()
            End Using
        Catch ex As Exception
            ExtractCompleted = False
            If affectedRows > 0 And dataTransferStatusId > 0 Then
                affectedRows = _TDataObjects.LoggingSettings.TblDataTransferStatus.Update(dataTransferStatusId, _
                        DataTransferStatusEnum.FAILED, Date.Now, False, _
                        SourceTable, ex.Message)
            End If
        End Try

        Return ExtractCompleted
    End Function

    Private Shared Function buildExtractSQL(ByVal SourceTable As String, ByVal ExtractTable As String) As StringBuilder

        Dim sbExtract As New StringBuilder

        With sbExtract
            Select Case SourceTable
                Case Is = "EBGPPR"
                    .Append("Select GPPIDT, GPPBUS,GPPPTR,GPPL01,GPPL02,GPPL03,GPPL04,GPPL05,GPPL06,GPPL07,GPPL08,GPPL09,GPPL10,GPPCOD,GPPSEQ ")
                Case Is = "EBGL01"
                    .Append("Select G01IDT,G01BUS,G01PTR,G01L01,G01SEQ,G01DS1,G01DS2,G01HT1,G01HT2,G01HT3,G01PGT,G01MDS,G01MKY,G01SCH,G01PPT,'','false','false','false','false','false','' ")
                Case Is = "EBGL02"
                    .Append("Select G02IDT,G02BUS,G02PTR,G02L01,G02L02,G02SEQ,G02DS1,G02DS2,G02HT1,G02HT2,G02HT3,G02PGT,G02MDS,G02MKY,'','','','false','false','false','false','false',''  ")
                Case Is = "EBGL03"
                    .Append("Select G03IDT,G03BUS,G03PTR,G03L01,G03L02,G03L03,G03SEQ,G03DS1,G03DS2,G03HT1,G03HT2,G03HT3,G03PGT,G03MDS,G03MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL04"
                    .Append("Select G04IDT,G04BUS,G04PTR,G04L01,G04L02,G04L03,G04L04,G04SEQ,G04DS1,G04DS2,G04HT1,G04HT2,G04HT3,G04PGT,G04MDS,G04MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL05"
                    .Append("Select G05IDT,G05BUS,G05PTR,G05L01,G05L02,G05L03,G05L04,G05L05,G05SEQ,G05DS1,G05DS2,G05HT1,G05HT2,G05HT3,G05PGT,G05MDS,G05MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL06"
                    .Append("Select G06IDT,G06BUS,G06PTR,G06L01,G06L02,G06L03,G06L04,G06L05,G06L06,G06SEQ,G06DS1,G06DS2,G06HT1,G06HT2,G06HT3,G06PGT,G06MDS,G06MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL07"
                    .Append("Select G07IDT,G07BUS,G07PTR,G07L01,G07L02,G07L03,G07L04,G07L05,G07L06,G07L07,G07SEQ,G07DS1,G07DS2,G07HT1,G07HT2,G07HT3,G07PGT,G07MDS,G07MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL08"
                    .Append("Select G08IDT,G08BUS,G08PTR,G08L01,G08L02,G08L03,G08L04,G08L05,G08L06,G08L07,G08L08,G08SEQ,G08DS1,G08DS2,G08HT1,G08HT2,G08HT3,G08PGT,G08MDS,G08MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL09"
                    .Append("Select G09IDT,G09BUS,G09PTR,G09L01,G09L02,G09L03,G09L04,G09L05,G09L06,G09L07,G09L08,G09L09,G09SEQ,G09DS1,G09DS2,G09HT1,G09HT2,G09HT3,G09PGT,G09MDS,G09MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBGL10"
                    .Append("Select G10IDT,G10BUS,G10PTR,G10L01,G10L02,G10L03,G10L04,G10L05,G10L06,G10L07,G10L08,G10L09,G10L10,G10SEQ,G10DS1,G10DS2,G10HT1,G10HT2,G10HT3,G10PGT,G10MDS,G10MKY,'','','','false','false','false','false','false','' ")
                Case Is = "EBPROD"
                    .Append("Select PRDIDT,PRDCOD,PRDDS1,PRDDS2,PRDDS3,PRDDS4,PRDDS5,PRDLEN,PRDLUN,PRDWTH,PRDWTU,PRDDTH,PRDDTU,PRDHGT,PRDHGU,PRDSIZ,PRDSZU,PRDWGT,PRDWGU,PRDVOL, ")
                    .Append("PRDVLU,PRDCOL,PRDPCK,PRDPKU,PRDSPN,PRDCPN,PRDTN1,PRDTN2,PRDABV,PRDVIN,PRDSUP,PRDCNY,PRDRGN,PRDARA,PRDGRP,PRDCLO,PRDCAT, ")
                    .Append("CASE WHEN PRDVEG = '1' THEN 'True' WHEN PRDVEG = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDVEE = '1' THEN 'True' WHEN PRDVEE = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDORG = '1' THEN 'True' WHEN PRDORG = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDBIO = '1' THEN 'True' WHEN PRDBIO = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDLUT = '1' THEN 'True' WHEN PRDLUT = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("PRMINA,PRDHT1,PRDHT2,PRDHT3,PRDSCH,PRDPGT,PRDMDS,PRDMKY,PRDSR1,PRDSR2,PRDSR3,PRDSR4,PRDSR5,PRDC01,PRDC02,PRDC03,PRDC04,PRDC05,PRDC06,PRDC07,  ")
                    .Append("PRDC08,PRDC09,PRDC10,PRDC11,PRDC12,PRDC13,PRDC14,PRDC15,PRDC16,PRDC17,PRDC18,PRDC19,PRDC20, ")
                    .Append("CASE WHEN PRDS01 = '1' THEN 'True' WHEN PRDS01 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS02 = '1' THEN 'True' WHEN PRDS02 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS03 = '1' THEN 'True' WHEN PRDS03 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS04 = '1' THEN 'True' WHEN PRDS04 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS05 = '1' THEN 'True' WHEN PRDS05 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS06 = '1' THEN 'True' WHEN PRDS06 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS07 = '1' THEN 'True' WHEN PRDS07 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS08 = '1' THEN 'True' WHEN PRDS08 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS09 = '1' THEN 'True' WHEN PRDS09 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDS10 = '1' THEN 'True' WHEN PRDS10 = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("DATE(PRDTS1),DATE(PRDTS2),DATE(PRDTS3),DATE(PRDTS4),DATE(PRDTS5), ")
                    .Append("PRDTRF, ")
                    .Append("CASE WHEN PRDOPM = '1' THEN 'True' WHEN PRDOPM = 'Y' THEN 'True' WHEN PRDOPM = ' ' THEN 'True' ELSE 'False' END, ")
                    .Append("PRDASK, ")
                    .Append("CASE WHEN PRDAOL = '1' THEN 'True' WHEN PRDAOL = 'Y' THEN 'True' WHEN PRDAOL = ' ' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDPER = '1' THEN 'True' WHEN PRDPER = 'Y' THEN 'True' ELSE 'False' END, ")
                    .Append("CASE WHEN PRDDIS = '1' THEN 'True' WHEN PRDDIS = 'Y' THEN 'True' ELSE 'False' END ")

                Case Is = "EBPRRL"
                    .Append("Select PRRIDT,PRRQFY,PRRBUS,PRRPTR,PRRL01,PRRL02,PRRL03,PRRL04,PRRL05,PRRL06,PRRL07,PRRL08,PRRL09,PRRL10,PRRCOD,PRRR01,PRRR02,PRRR03,PRRR04,PRRR05, ")
                    .Append(" PRRR06,PRRR07,PRRR08,PRRR09,PRRR10,PRRREL,PRRSEQ ")
                Case Is = "EBPLDT"
                    .Append("Select PLDIDT,PLDTYP,DATE(PLDADT),HOUR(PLDADT),MINUTE(PLDADT),SECOND(PLDADT),MICROSECOND(PLDADT),PLDCOD,PLDPRD,DATE(PLDSTR),DATE(PLDEND),PLDNPC,PLDGPC,PLDTAX,PLDSNP,PLDSGP,PLDSTX,PLDDNP,PLDDGP,PLDDTX,PLDPC1,PLDPC2,PLDPC3,PLDPC4,PLDPC5,PLDTXC,PLDTRC,PLDPBC,PDPQ01,PDSQ01,PDPN02,PDPG02,PDPT02,PDPQ02,PDSN02,PDSG02,PDST02,PDSQ02,PDPN03,PDPG03,PDPT03,PDPQ03,PDSN03,PDSG03,PDST03,PDSQ03,PDPN04,PDPG04,PDPT04,PDPQ04,PDSN04,PDSG04,PDST04,PDSQ04,PDPN05,PDPG05,PDPT05,PDPQ05,PDSN05,PDSG05,PDST05,PDSQ05,PDPN06,PDPG06,PDPT06,PDPQ06,PDSN06,PDSG06,PDST06,PDSQ06,PDPN07,PDPG07,PDPT07,PDPQ07,PDSN07,PDSG07,PDST07,PDSQ07,PDPN08,PDPG08,PDPT08,PDPQ08,PDSN08,PDSG08,PDST08,PDSQ08,PDPN09,PDPG09,PDPT09,PDPQ09,PDSN09,PDSG09,PDST09,PDSQ09,PDPN10,PDPG10,PDPT10,PDPQ10,PDSN10,PDSG10,PDST10,PDSQ10,PDDN02,PDDG02,PDDT02,PDLN02,PDLG02,PDLT02,PDDN03,PDDG03,PDDT03,PDLN03,PDLG03,PDLT03,PDDN04,PDDG04,PDDT04,PDLN04,PDLG04,PDLT04,PDDN05,PDDG05,PDDT05,PDLN05,PDLG05,PDLT05,PDDN06,PDDG06,PDDT06,PDLN06,PDLG06,PDLT06,PDDN07,PDDG07,PDDT07,PDLN07,PDLG07,PDLT07,PDDN08,PDDG08,PDDT08,PDLN08,PDLG08,PDLT08,PDDN09,PDDG09,PDDT09,PDLN09,PDLG09,PDLT09,PDDN10,PDDG10,PDDT10,PDLN10,PDLG10,PDLT10 ")
                Case Is = "EBPLHD"
                    .Append("Select PLHIDT,PLHCOD,PLHDES,PLHCUR,PLHFDV,PLHMDV,DATE(PLHSTR),DATE(PLHEND) ")
                Case Is = "EBGROU"
                    .Append("Select GRPIDT,GRPCOD,GRPDS1,GRPDS2,GRPHT1,GRPHT2,GRPHT3,GRPPGT,GRPMDS,GRPMKY ")
                Case Is = "EBPRST"
                    .Append("Select PSTIDT,PSTPRD,PSTSLC,PSTQTY,PSTALC,PSTAVL,PSTRSC,PSTWHS ")
                Case Is = "EBORDDT"
                    .Append("SELECT ")
                    .Append("0, ODORID, ODLINE, ODGL01, ODGL02, ODGL03, ODGL04, ODGL05, ODGL06, ODGL07, ODGL08, ODGL09, ")
                    .Append("ODGL10, ODPROD, ODQTTY, ODSHQT, DATE(ODSHDT), ODPDS1, ODPDS2, ODPSUP, ODPPRG, ODPPRN, ")
                    .Append("ODPPRT, ODDLVG, ODDLVN, ODDLVT, ODTAXC, ODLING, ODLINN, ODLINT, ODSHPN, ODTRAK, ODBAKS, ")
                    .Append("ODCNNM, ODADR1, ODADR2, ODADR3, ODADR4, ODADR5, ODPOST, ODCOUN, ODCNTL, ODCNEM, ODLANG, ")
                    .Append("ODWRHS, ODCURR, ODSHIP ")
                Case Is = "EBORDHT"
                    .Append("SELECT ")
                    .Append("OHORID, OHORID, OHBUSU, OHPTNR, OHPTNM, OHLOGN, OHUSNM, OHSTTS, OHCOMM, OHCNNM, OHADR1, OHADR2, ")
                    .Append("OHADR3, OHADR4, OHADR5, OHPOST, OHCOUN, OHCNTL, OHCNEM, DATE(OHORDT), DATE(OHSHDT), DATE(OHLSAC), ")
                    .Append("OHORIG, OHORIN, OHORIT, OHDELG, OHDELN, OHDELT, OHPROM, OHPRMV, OHTTOV, OHTOTC, OHSIN1, OHSIN2, ")
                    .Append("OHTRAK, OHBOOD, OHBOST, OHBORF, OHLANG, OHWRHS, OHCURR, OHPAYT, OHSHIP ")
                Case Is = "EBINVHT"
                    .Append("SELECT ")
                    .Append("IHINNO, IHORNO, IHBUSN, IHPTNR, IHPTNO, IHLGID, DATE(IHDATE), IHAMNT, IHVATA, IHOUTA, ")
                    .Append("IHCUSR, IHSTTS, IHCSPO, IHOORN, DATE(IHOORD), IHDSSQ, IHACNO, IHCNAM, IHCATT, IHCAD1, ")
                    .Append("IHCAD2, IHCAD3, IHCAD4, IHCAD5, IHCAD6, IHCAD7, IHSTNM, IHSTAT, IHSTA1, IHSTA2, ")
                    .Append("IHSTA3, IHSTA4, IHSTA5, IHSTA6, IHSTA7, IHSFNM, IHSFAT, IHSFA1, IHSFA2, IHSFA3, ")
                    .Append("IHSFA4, IHSFA5, IHSFA6, IHSFA7, IHVATN, IHPTRF, IHPTPR, IHPTDY, ")
                    .Append("CASE WHEN IHFLAG = '1' THEN 'True' WHEN IHFLAG = 'Y' THEN 'True' WHEN IHFLAG= ' ' THEN 'True' ELSE 'False' END ")
                Case Is = "EBCNEHT"
                    .Append("SELECT ")
                    .Append("CHINNO, CHORNO, CHBUSN, CHPTNR, CHPTNO, CHLGID, DATE(CHDATE), CHAMNT, CHVATA, CHOUTA, ")
                    .Append("CHCUSR, CHSTTS, CHCSPO, CHOORN, DATE(CHOORD), CHDSSQ, CHACNO, CHCNAM, CHCATT, CHCAD1, ")
                    .Append("CHCAD2, CHCAD3, CHCAD4, CHCAD5, CHCAD6, CHCAD7, CHSTNM, CHSTAT, CHSTA1, CHSTA2, ")
                    .Append("CHSTA3, CHSTA4, CHSTA5, CHSTA6, CHSTA7, CHSFNM, CHSFAT, CHSFA1, CHSFA2, CHSFA3, ")
                    .Append("CHSFA4, CHSFA5, CHSFA6, CHSFA7, CHVATN, CHPTRF, CHPTPR, CHPTDY ")

                Case Is = "EBPRDO"
                    .Append("SELECT ")
                    .Append("0, PROBUS, PROPTR, PROPRD, PROTYP, PROCOD, PROOPR, PROPOR ")
                Case Is = "EBPRDD"
                    .Append("SELECT ")
                    .Append("0, PRFBUS, PRFPTR, PRFPRD, PRFTYP, PRFMAC, CASE WHEN PRFDFT = '1' THEN 'True' WHEN PRFDFT = 'Y' THEN 'True' ELSE 'False' END, PRFASQ, PRFDSQ, PRFDTP ")
                Case Is = "EBUSERT"
                    .Append("SELECT ")
                    .Append("0,USRBUS, USRNUM, USRNAM, USRAD1, USRAD2, USRAD3, USRAD4, USRCCD, USRPCD, USRFNM, USRSNM, USRTIT, ")
                    .Append("USREML, USRPWD, USRTEL, USRMOD, USRWTL, USRACT, USRBCN, USRPCN, USRPLN, USRMNQ, USRMNV, USRCMP, USRCCT ")
                Case Is = "EBECMB"
                    .Append("SELECT ")
                    .Append("MDBIDT, MDBBUS, MDBPTN, MDBAPP, MDBMOD, MDBDFT, MDBVAL ")
                Case Is = "EBPLDTP"
                    .Append("Select PLDIDT,PLDTYP,DATE(PLDADT),HOUR(PLDADT),MINUTE(PLDADT),SECOND(PLDADT),MICROSECOND(PLDADT),PLDCOD,PLDPRD,DATE(PLDSTR),DATE(PLDEND),PLDNPC,PLDGPC,PLDTAX,PLDSNP,PLDSGP,PLDSTX,PLDDNP,PLDDGP,PLDDTX,PLDPC1,PLDPC2,PLDPC3,PLDPC4,PLDPC5,PLDTXC,PLDTRC,PLDPBC,PDPQ01,PDSQ01,PDPN02,PDPG02,PDPT02,PDPQ02,PDSN02,PDSG02,PDST02,PDSQ02,PDPN03,PDPG03,PDPT03,PDPQ03,PDSN03,PDSG03,PDST03,PDSQ03,PDPN04,PDPG04,PDPT04,PDPQ04,PDSN04,PDSG04,PDST04,PDSQ04,PDPN05,PDPG05,PDPT05,PDPQ05,PDSN05,PDSG05,PDST05,PDSQ05,PDPN06,PDPG06,PDPT06,PDPQ06,PDSN06,PDSG06,PDST06,PDSQ06,PDPN07,PDPG07,PDPT07,PDPQ07,PDSN07,PDSG07,PDST07,PDSQ07,PDPN08,PDPG08,PDPT08,PDPQ08,PDSN08,PDSG08,PDST08,PDSQ08,PDPN09,PDPG09,PDPT09,PDPQ09,PDSN09,PDSG09,PDST09,PDSQ09,PDPN10,PDPG10,PDPT10,PDPQ10,PDSN10,PDSG10,PDST10,PDSQ10,PDDN02,PDDG02,PDDT02,PDLN02,PDLG02,PDLT02,PDDN03,PDDG03,PDDT03,PDLN03,PDLG03,PDLT03,PDDN04,PDDG04,PDDT04,PDLN04,PDLG04,PDLT04,PDDN05,PDDG05,PDDT05,PDLN05,PDLG05,PDLT05,PDDN06,PDDG06,PDDT06,PDLN06,PDLG06,PDLT06,PDDN07,PDDG07,PDDT07,PDLN07,PDLG07,PDLT07,PDDN08,PDDG08,PDDT08,PDLN08,PDLG08,PDLT08,PDDN09,PDDG09,PDDT09,PDLN09,PDLG09,PDLT09,PDDN10,PDDG10,PDDT10,PDLN10,PDLG10,PDLT10 ")
                Case Is = "EBOTHD"
                    .Append("Select THORID, THNAME, THDESC, THBUSU, THPTNR, THLOGN, DATE(THCDAT), DATE(THLDAT), ")
                    .Append(" DATE(THUDAT), CASE WHEN THDFLT= '1' THEN 'True'  WHEN THDFLT= 'Y' THEN 'True' ELSE 'False' END ")
                Case Is = "EBOTDT"
                    .Append("Select TDTDID, TDTHID, TDPROD, TDQTTY, TDMAST, TDNAME ")
                Case Is = "EBPM"
                    .Append("Select PMIDT, PMBUS, PMPTG, PMPTR, PMCOD, PMTYP, PMACM, DATE(PMSDT), DATE(PMEDT), PMRCT, PMRMX, PMMNS, PMMNI, PMNPC, PMURM, PMPSQ, ")
                    .Append("CASE WHEN PMACT = '1' THEN 'True' WHEN PMACT = 'Y' THEN 'True' WHEN PMACT = 'y' THEN 'True' WHEN PMACT = ' '  THEN 'False' ELSE 'False' END, PMRAT ")
                Case Is = "EBPML"
                    .Append("Select PMLIDT, PMLBUS, PMLPTG, PMLPTR, PMLCOD, PMLLNG, PMLDNM, PMLRQD, PMLER1, PMLER2 ")
                Case Is = "EBPMD"
                    .Append("Select PMDIDT, PMDBUS, PMDPTG, PMDPTR, PMDCOD, ")
                    .Append("CASE WHEN PMDPFG = '1' THEN 'True' WHEN PMDPFG = 'Y' THEN 'True' WHEN PMDPFG = 'y' THEN 'True' WHEN PMDPFG = ' '  THEN 'False' ELSE 'False' END, ")
                    .Append("PMDVAL, PMDPRD ")
                Case Is = "EBPMR"
                    .Append("Select PMRIDT, PMRBUS, PMRPTG, PMRPTR, PMRCOD, PMRPRD, PMRQTY ")
                Case Is = "EBPMF"
                    .Append("Select PMFIDT, PMFBUS, PMFPTR, PMFCOD, PMFPRD, PMFQTY, PMFPTG, ")
                    .Append("CASE WHEN PMFALO = '1' THEN 'True' WHEN PMFALO = 'Y' THEN 'True' PMFALO = 'y' THEN 'True' WHEN PMFALO = ' ' THEN 'False' ELSE 'False' END ")
            End Select
            .Append("From ")

            Dim CopySourceTable As String = SourceTable
            Select Case CopySourceTable
                Case Is = "EBPLDTP"
                    CopySourceTable = "EBPLDT"
            End Select

            .Append(CopySourceTable)
            .Append(" as ")
            .Append(ExtractTable)

            Select Case SourceTable
                Case Is = "EBPLDTP"
                    .Append(" Where PLDPRC = 'Y'")
                Case Is = "EBPLDT"
                    .Append(" Where PLDTYP = ' '")
                Case Is = "EBPM"
                    .Append(" Where PMADH <> 'Y'")
            End Select

        End With
        Return sbExtract
    End Function

#End Region

#Region "40/. DoUpdateTable_EBPTNR()"

    '===================================================================================================
    '
    '   40/. DoUpdateTable_EBPTNR()
    '
    '===================================================================================================
    'Private Shared Sub DoUpdateTable_EBPTNR()

    '    'Dim connectionString As String = ConfigurationManager.ConnectionStrings("SQLConnectionString").ToString
    '    Dim connectionString As String = SQLConnectionString()

    '    '=====================================================================================
    '    ' 1/. UPDATE the DESTINATION Records from the SOURCE Records
    '    '=====================================================================================

    '    Dim sbUpdate As New StringBuilder
    '    With sbUpdate
    '        .Append("Update tbl_partner_test ")
    '        .Append("Set ")
    '        .Append("PARTNER_DESC = RTrim(( SELECT tbl_partner_work.PARTNER_DESC From tbl_partner_work Where tbl_partner_work.PARTNER = tbl_partner_test.PARTNER)) ")
    '        .Append("Where Exists ")
    '        .Append("(")
    '        .Append("Select tbl_partner_work.PARTNER ")
    '        .Append("From tbl_partner_work ")
    '        .Append("Where tbl_partner_work.PM0SIT = tbl_partner_test.PARTNER ")
    '        .Append(")")
    '    End With

    '    Using con As New SqlConnection(connectionString)
    '        Dim cmd As New SqlCommand(sbUpdate.ToString(), con)
    '        con.Open()
    '        cmd.ExecuteNonQuery()
    '        con.Close()
    '    End Using

    '    '=====================================================================================
    '    ' 2/. INSERT new Records into the DESTINATION Table
    '    '=====================================================================================
    '    Dim sbInsert As New StringBuilder
    '    With sbInsert
    '        .Append("INSERT INTO tbl_partner_test ")
    '        .Append("(")
    '        .Append("PARTNER, ")
    '        .Append("PARTNER_DESC, ")
    '        .Append("DESTINATION_DATABASE, ")
    '        .Append("CACHEING_ENABLED, ")
    '        .Append("CACHE_TIME_MINUTES, ")
    '        .Append("LOGGING_ENABLED, ")
    '        .Append("STORE_XML, ")
    '        .Append("ACCOUNT_NO_1, ")
    '        .Append("ACCOUNT_NO_2, ")
    '        .Append("ACCOUNT_NO_3, ")
    '        .Append("ACCOUNT_NO_4, ")
    '        .Append("ACCOUNT_NO_5, ")
    '        .Append("EMAIL, ")
    '        .Append("TELEPHONE_NUMBER, ")
    '        .Append("FAX_NUMBER, ")
    '        .Append("PARTNER_URL, ")
    '        .Append("PARTNER_NUMBER, ")
    '        .Append("ORIGINATING_BUSINESS_UNIT ")
    '        .Append(")")
    '        .Append("SELECT ")
    '        .Append("RTrim(tbl_partner_work.PARTNER) AS Expr1, ")
    '        .Append("RTrim(tbl_partner_work.PARTNER_DESC) AS Expr2, ")
    '        .Append("RTrim(tbl_partner_work.DESTINATION_DATABASE) AS Expr3, ")
    '        .Append("RTrim(tbl_partner_work.CACHEING_ENABLED) AS Expr4, ")
    '        .Append("RTrim(tbl_partner_work.LOGGING_ENABLED) AS Expr5, ")
    '        .Append("RTrim(tbl_partner_work.STORE_XML) AS Expr6, ")
    '        .Append("RTrim(tbl_partner_work.ACCOUNT_NO_1) AS Expr7, ")
    '        .Append("RTrim(tbl_partner_work.ACCOUNT_NO_2) AS Expr8, ")
    '        .Append("RTrim(tbl_partner_work.ACCOUNT_NO_3) AS Expr9, ")
    '        .Append("RTrim(tbl_partner_work.ACCOUNT_NO_4) AS Expr10, ")
    '        .Append("RTrim(tbl_partner_work.ACCOUNT_NO_5) AS Expr11, ")
    '        .Append("RTrim(tbl_partner_work.EMAIL) AS Expr12, ")
    '        .Append("RTrim(tbl_partner_work.TELEPHONE_NUMBER) AS Expr13 ")
    '        .Append("RTrim(tbl_partner_work.FAX_NUMBER) AS Expr14 ")
    '        .Append("RTrim(tbl_partner_work.PARTNER_URL) AS Expr15 ")
    '        .Append("RTrim(tbl_partner_work.PARTNER_NUMBER) AS Expr16 ")
    '        .Append("RTrim(tbl_partner_work.ORIGINATING_BUSINESS_UNIT) AS Expr17 ")
    '        .Append("FROM tbl_partner_work ")
    '        .Append("WHERE tbl_partner_work.PARTNER Not In (SELECT PARTNER from tbl_partner_test)")
    '    End With

    '    Using con As New SqlConnection(connectionString)
    '        Dim cmd As New SqlCommand(sbInsert.ToString(), con)
    '        con.Open()
    '        cmd.ExecuteNonQuery()
    '        con.Close()
    '    End Using

    '    '=====================================================================================
    '    ' 3/. DELETE Records from the DESTINATION Table that do NOT Exist in the EXTRACT Table
    '    '=====================================================================================
    '    Dim sbDelete As New StringBuilder
    '    With sbDelete
    '        .Append("Delete from tbl_partner_test ")
    '        .Append("Where PARTNER Not In (Select PARTNER From tbl_partner_work)")
    '    End With

    '    Using con As New SqlConnection(connectionString)
    '        Dim cmd As New SqlCommand(sbDelete.ToString(), con)
    '        con.Open()
    '        cmd.ExecuteNonQuery()
    '        con.Close()
    '    End Using

    'End Sub

#End Region

    Private Shared Sub DoUpdateTable(ByVal updateCommand As String, _
                                ByVal insertCommand As String, _
                                ByVal deleteCommand As String)

        'Dim connectionString As String = ConfigurationManager.ConnectionStrings("SQLConnectionString").ToString
        Dim connectionString As String = SQLConnectionString()

        '=====================================================================================
        ' 1/. UPDATE the DESTINATION Records from the SOURCE Records
        '=====================================================================================
        If Not String.IsNullOrEmpty(updateCommand) Then
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(updateCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If


        '=====================================================================================
        ' 2/. INSERT new Records into the DESTINATION Table
        '=====================================================================================
        If Not String.IsNullOrEmpty(insertCommand) Then
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(insertCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If


        '=====================================================================================
        ' 3/. DELETE Records from the DESTINATION Table that do NOT Exist in the EXTRACT Table
        '=====================================================================================
        If Not String.IsNullOrEmpty(deleteCommand) Then
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(deleteCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If


    End Sub

    Private Shared Sub DoReplaceUpdateTable(ByVal deleteCommand As String, ByVal insertCommand As String)

        Dim connectionString As String = SQLConnectionString()

        If Not String.IsNullOrEmpty(insertCommand) Then

            'Delete all first
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(deleteCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using

            'Insert all
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(insertCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If

    End Sub

    Private Shared Sub DoReplaceTruncInsertTable(ByVal truncateCommand As String, ByVal insertCommand As String)

        Dim connectionString As String = SQLConnectionString()

        If Not String.IsNullOrEmpty(insertCommand) Then
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(truncateCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using

            'Insert all
            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(insertCommand, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If

    End Sub

    Private Shared Sub DoUpdateTableNoDelete(ByVal updateCommand As String, _
                                            ByVal insertCommand As String)

        'Dim connectionString As String = ConfigurationManager.ConnectionStrings("SQLConnectionString").ToString
        Dim connectionString As String = SQLConnectionString()

        '=====================================================================================
        ' 1/. UPDATE the DESTINATION Records from the SOURCE Records
        '=====================================================================================

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updateCommand, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        '=====================================================================================
        ' 2/. INSERT new Records into the DESTINATION Table
        '=====================================================================================
        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertCommand, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

    End Sub

    'Private Shared Sub DoUpdateTable_EBBUSN()
    '    Dim insert As New StringBuilder
    '    Dim update As New StringBuilder
    '    Dim delete As New StringBuilder

    '    With update
    '        .Append("UPDATE tbl_business_unit ")
    '        .Append("SET ")
    '        .Append("   BUSINESS_UNIT = RTrim(( ")
    '        .Append("                         SELECT tbl_business_unit_work.BUSCOD ")
    '        .Append("                         FROM tbl_business_unit_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   DESCRIPTION = RTrim(( ")
    '        .Append("                         SELECT tbl_business_unit_work.BUSDES ")
    '        .Append("                         FROM tbl_business_unit_work ")
    '        .Append("                         ))")
    '        .Append("WHERE EXISTS (")
    '        .Append("              SELECT tbl_business_unit_work.BUSCOD ")
    '        .Append("              FROM tbl_business_unit_work ")
    '        .Append("              WHERE tbl_business_unit_work.BUSCOD = tbl_business_unit.BUSINESS_UNIT) ")
    '    End With

    '    With insert
    '        .Append("INSERT INTO tbl_business_unit( ")
    '        .Append("                   BUSINESS_UNIT, ")
    '        .Append("                   DESCRIPTION) ")
    '        .Append("   SELECT ")
    '        .Append("           RTRIM(tbl_business_unit_work.BUSCOD) AS Expr1,")
    '        .Append("           RTRIM(tbl_business_unit_work.BUSDES) AS Expr2")
    '        .Append("   FROM tbl_business_unit_work ")
    '        .Append("   WHERE tbl_business_unit_work.BUSCOD NOT IN (")
    '        .Append("                                               SELECT BUSINESS_UNIT ")
    '        .Append("                                               FROM tbl_business_unit) ")
    '    End With

    '    With delete
    '        .Append("DELETE FROM tbl_business_unit ")
    '        .Append("WHERE tbl_business_unit.BUSINESS_UNIT ")
    '        .Append("           NOT IN (")
    '        .Append("                   SELECT tbl_business_unit_work.BUSCOD ")
    '        .Append("                   FROM tbl_business_unit_work )")
    '    End With

    '    DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    'End Sub

    'Private Shared Sub DoUpdateTable_EBECMD()
    '    Dim insert As New StringBuilder
    '    Dim update As New StringBuilder
    '    Dim delete As New StringBuilder

    '    With update
    '        .Append("UPDATE tbl_ecommerce_module_defaults ")
    '        .Append("SET ")
    '        .Append("   BUSINESS_UNIT = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPBUS ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   PARTNER = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPTR ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   CONFIRMATION_EMAIL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   CUSTOMER_NUMBER_PREFIX = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPCPR ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   DEFAULT_BU = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPDFT ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   DELIVERY_COST_PERC = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPDPC ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   DELIVERY_ROUNDING_MASK = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPDRM ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   HTML_PATH_ABSOLUTE = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   HTML_PER_PAGE = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDHTPP ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   HTML_PER_PAGE_TYPE = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDHTPT ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   IMAGE_PATH_ABSOLUTE = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   IMAGE_PATH_VIRTUAL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   NUMBER_OF_GROUP_LEVELS = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPNGR ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   ORDER_NUMBER = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   ORDER_NUMBER_PREFIX = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPOPR ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   PARTNER_NUMBER = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPTNM ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   PAYMENT_GATEWAY_TYPE = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   PAYMENT_DETAILS_1 = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPD1 ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   PAYMENT_DETAILS_2 = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPD2 ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   PAYMENT_DETAILS_3 = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPD3 ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   PAYMENT_DETAILS_4 = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPD4 ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   PAYMENT_DETAILS_5 = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPD5 ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   PAYMENT_URL_1 = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_URL_2 = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_URL_3 = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_REJECT_ADDRESS_AVS = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_REJECT_POSTCODE_AVS = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_REJECT_CSC = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_ALLOW_PARTIAL_AVS = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_DEBUG = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PAYMENT_CALL_BANK_API = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   PRICE_LIST = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDPPCL ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        '.Append("   STOCK_LOCATION = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   TEMP_ORDER_NUMBER = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        .Append("   THEME = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDTHEM ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        .Append("   , ")
    '        .Append("   USER_NUMBER = RTrim(( ")
    '        .Append("                         SELECT tbl_ecommerce_module_defaults_work.MDUSNM ")
    '        .Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   USE_AGE_CHECK = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   ORDER_DESTINATION_DATABASE = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   CUSTOMER_DESTINATION_DATABASE = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   STORED_PROCEDURE_GROUP = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PRODUCT_HTML = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   PRODUCT_HTML_TYPE = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   MIN_ADD_QUANTITY = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   DEFAULT_ADD_QUANTITY = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   REGISTRATION_CONFIRMATION_FROM_EMAIL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   ORDERS_FROM_EMAIL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   FORGOTTEN_PASSWORD_FROM_EMAIL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   CONTACT_US_TO_EMAIL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   CONTACT_US_TO_EMAIL_CC = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        '.Append("   , ")
    '        '.Append("   CONTACT_US_FROM_EMAIL = RTrim(( ")
    '        '.Append("                         SELECT tbl_ecommerce_module_defaults_work.?????? ")
    '        '.Append("                         FROM tbl_ecommerce_module_defaults_work ")
    '        '.Append("                         ))")
    '        .Append("WHERE EXISTS (")
    '        .Append("              SELECT tbl_ecommerce_module_defaults_work.MDPBUS ")
    '        .Append("              FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("              WHERE tbl_ecommerce_module_defaults_work.MDPBUS = tbl_ecommerce_module_defaults.BUSINESS_UNIT ")
    '        .Append("              AND tbl_ecommerce_module_defaults_work.MDPPTR = tbl_ecommerce_module_defaults.PARTNER) ")
    '    End With

    '    With insert
    '        .Append("INSERT INTO tbl_ecommerce_module_defaults( ")
    '        .Append("                   BUSINESS_UNIT ")
    '        .Append(",                   PARTNER ")
    '        '.Append(",                   CONFIRMATION_EMAIL ")
    '        .Append(",                   CUSTOMER_NUMBER_PREFIX ")
    '        .Append(",                   DEFAULT_BU ")
    '        .Append(",                   DELIVERY_COST_PERC ")
    '        .Append(",                   DELIVERY_ROUNDING_MASK ")
    '        '.Append(",                   HTML_PATH_ABSOLUTE ")
    '        .Append(",                   HTML_PER_PAGE ")
    '        .Append(",                   HTML_PER_PAGE_TYPE ")
    '        '.Append(",                   IMAGE_PATH_ABSOLUTE ")
    '        '.Append(",                   IMAGE_PATH_VIRTUAL ")
    '        .Append(",                   NUMBER_OF_GROUP_LEVELS ")
    '        '.Append(",                   ORDER_NUMBER ")
    '        .Append(",                   ORDER_NUMBER_PREFIX ")
    '        .Append(",                   PARTNER_NUMBER ")
    '        '.Append(",                   PAYMENT_GATEWAY_TYPE ")
    '        .Append(",                   PAYMENT_DETAILS_1 ")
    '        .Append(",                   PAYMENT_DETAILS_2 ")
    '        .Append(",                   PAYMENT_DETAILS_3 ")
    '        .Append(",                   PAYMENT_DETAILS_4 ")
    '        .Append(",                   PAYMENT_DETAILS_5 ")
    '        '.Append(",                   PAYMENT_URL_1 ")
    '        '.Append(",                   PAYMENT_URL_2 ")
    '        '.Append(",                   PAYMENT_URL_3 ")
    '        '.Append(",                   PAYMENT_REJECT_ADDRESS_AVS ")
    '        '.Append(",                   PAYMENT_REJECT_POSTCODE_AVS ")
    '        '.Append(",                   PAYMENT_REJECT_CSC ")
    '        '.Append(",                   PAYMENT_ALLOW_PARTIAL_AVS ")
    '        '.Append(",                   PAYMENT_DEBUG ")
    '        '.Append(",                   PAYMENT_CALL_BANK_API ")
    '        .Append(",                   PRICE_LIST ")
    '        '.Append(",                   STOCK_LOCATION ")
    '        '.Append(",                   TEMP_ORDER_NUMBER ")
    '        .Append(",                   THEME ")
    '        .Append(",                   USER_NUMBER ")
    '        '.Append(",                   USE_AGE_CHECK ")
    '        '.Append(",                   ORDER_DESTINATION_DATABASE ")
    '        '.Append(",                   CUSTOMER_DESTINATION_DATABASE ")
    '        '.Append(",                   STORED_PROCEDURE_GROUP ")
    '        '.Append(",                   PRODUCT_HTML ")
    '        '.Append(",                   PRODUCT_HTML_TYPE ")
    '        '.Append(",                   MIN_ADD_QUANTITY ")
    '        '.Append(",                   DEFAULT_ADD_QUANTITY ")
    '        '.Append(",                   REGISTRATION_CONFIRMATION_FROM_EMAIL ")
    '        '.Append(",                   ORDERS_FROM_EMAIL ")
    '        '.Append(",                   FORGOTTEN_PASSWORD_FROM_EMAIL ")
    '        '.Append(",                   CONTACT_US_TO_EMAIL ")
    '        '.Append(",                   CONTACT_US_TO_EMAIL_CC ")
    '        '.Append(",                   CONTACT_US_FROM_EMAIL ")
    '        .Append("       )")
    '        .Append("   SELECT ")
    '        .Append("           RTRIM(tbl_ecommerce_module_defaults_work.MDPBUS) AS Expr1")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPTR) AS Expr2")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr3")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPCPR) AS Expr4")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPDFT) AS Expr5")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPDPC) AS Expr6")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPDRM) AS Expr7")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr8")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDHTPP) AS Expr9")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDHTPT) AS Expr10")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr11")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr12")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPNGR) AS Expr13")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr14")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPOPR) AS Expr15")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPTNM) AS Expr16")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr17")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPD1) AS Expr18")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPD2) AS Expr19")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPD3) AS Expr20")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPD4) AS Expr21")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPD5) AS Expr22")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr23")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr24")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr25")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr26")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr27")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr28")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr29")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr30")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr31")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDPPCL) AS Expr32")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr33")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr34")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDTHEM) AS Expr35")
    '        .Append(",           RTRIM(tbl_ecommerce_module_defaults_work.MDUSNM) AS Expr36")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr37")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr38")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr39")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr40")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr41")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr42")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr43")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr44")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr45")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr46")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr47")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr48")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr49")
    '        '.Append(",           RTRIM(tbl_ecommerce_module_defaults_work.??????) AS Expr50")
    '        .Append("   FROM tbl_ecommerce_module_defaults_work ")
    '        .Append("   WHERE (RTRIM(tbl_ecommerce_module_defaults_work.MDPBUS) +")
    '        .Append("               RTRIM(tbl_ecommerce_module_defaults_work.MDPPTR)) NOT IN (")
    '        .Append("                                               SELECT (BUSINESS_UNIT + PARTNER) ")
    '        .Append("                                               FROM tbl_ecommerce_module_defaults) ")
    '    End With

    '    With delete
    '        .Append("DELETE FROM tbl_ecommerce_module_defaults ")
    '        .Append("   WHERE  (BUSINESS_UNIT + PARTNER)  NOT IN (")
    '        .Append("               SELECT (RTRIM(tbl_ecommerce_module_defaults_work.MDPBUS) +")
    '        .Append("                           RTRIM(tbl_ecommerce_module_defaults_work.MDPPTR))")
    '        .Append("               FROM tbl_ecommerce_module_defaults_work) ")
    '    End With


    '    DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    'End Sub

    Public Shared Sub DoUpdateTable_EBGL01(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_01_work"
        Const destinationTable As String = "tbl_group_level_01"
        'Build a dictionary of the field mappings for insert
        Dim insertFieldMappings As New Generic.Dictionary(Of String, String)
        With insertFieldMappings
            .Add("GROUP_L01_BUSINESS_UNIT", "GROUP_L01_BUSINESS_UNIT")
            .Add("GROUP_L01_PARTNER", "GROUP_L01_PARTNER")
            .Add("GROUP_L01_L01_GROUP", "GROUP_L01_L01_GROUP")
            .Add("GROUP_L01_SEQUENCE", "GROUP_L01_SEQUENCE")
            .Add("GROUP_L01_DESCRIPTION_1", "GROUP_L01_DESCRIPTION_1")
            .Add("GROUP_L01_DESCRIPTION_2", "GROUP_L01_DESCRIPTION_2")
            .Add("GROUP_L01_HTML_1", "GROUP_L01_HTML_1")
            .Add("GROUP_L01_HTML_2", "GROUP_L01_HTML_2")
            .Add("GROUP_L01_HTML_3", "GROUP_L01_HTML_3")
            .Add("GROUP_L01_PAGE_TITLE", "GROUP_L01_PAGE_TITLE")
            .Add("GROUP_L01_META_DESCRIPTION", "GROUP_L01_META_DESCRIPTION")
            .Add("GROUP_L01_META_KEYWORDS", "GROUP_L01_META_KEYWORDS")
            .Add("GROUP_L01_THEME", "GROUP_L01_THEME")
            .Add("GROUP_L01_ADV_SEARCH_TEMPLATE", "GROUP_L01_ADV_SEARCH_TEMPLATE")
            .Add("GROUP_L01_PRODUCT_PAGE_TEMPLATE", "GROUP_L01_PRODUCT_PAGE_TEMPLATE")
            .Add("GROUP_L01_PRODUCT_LIST_TEMPLATE", "GROUP_L01_PRODUCT_LIST_TEMPLATE")
            .Add("GROUP_L01_SHOW_CHILDREN_AS_GROUPS", "GROUP_L01_SHOW_CHILDREN_AS_GROUPS")
            .Add("GROUP_L01_SHOW_PRODUCTS_AS_LIST", "GROUP_L01_SHOW_PRODUCTS_AS_LIST")
            .Add("GROUP_L01_SHOW_IN_NAVIGATION", "GROUP_L01_SHOW_IN_NAVIGATION")
            .Add("GROUP_L01_SHOW_IN_GROUPED_NAV", "GROUP_L01_SHOW_IN_GROUPED_NAV")
            .Add("GROUP_L01_HTML_GROUP", "GROUP_L01_HTML_GROUP")
            .Add("GROUP_L01_HTML_GROUP_TYPE", "GROUP_L01_HTML_GROUP_TYPE")
        End With
        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_L01_ID", "GROUP_L01_ID")
            .Add("GROUP_L01_BUSINESS_UNIT", "GROUP_L01_BUSINESS_UNIT")
            .Add("GROUP_L01_PARTNER", "GROUP_L01_PARTNER")
            .Add("GROUP_L01_L01_GROUP", "GROUP_L01_L01_GROUP")
            .Add("GROUP_L01_SEQUENCE", "GROUP_L01_SEQUENCE")
            .Add("GROUP_L01_DESCRIPTION_1", "GROUP_L01_DESCRIPTION_1")
            .Add("GROUP_L01_DESCRIPTION_2", "GROUP_L01_DESCRIPTION_2")
            .Add("GROUP_L01_HTML_1", "GROUP_L01_HTML_1")
            .Add("GROUP_L01_HTML_2", "GROUP_L01_HTML_2")
            .Add("GROUP_L01_HTML_3", "GROUP_L01_HTML_3")
            .Add("GROUP_L01_PAGE_TITLE", "GROUP_L01_PAGE_TITLE")
            .Add("GROUP_L01_META_DESCRIPTION", "GROUP_L01_META_DESCRIPTION")
            .Add("GROUP_L01_META_KEYWORDS", "GROUP_L01_META_KEYWORDS")
            .Add("GROUP_L01_THEME", "GROUP_L01_THEME")

            If business_unit <> "" Then
                '-----------------------------------------------------
                ' If called from Supplynet then set additional fields
                '-----------------------------------------------------
                .Add("GROUP_L01_ADV_SEARCH_TEMPLATE", "GROUP_L01_ADV_SEARCH_TEMPLATE")
                .Add("GROUP_L01_PRODUCT_PAGE_TEMPLATE", "GROUP_L01_PRODUCT_PAGE_TEMPLATE")
                .Add("GROUP_L01_PRODUCT_LIST_TEMPLATE", "GROUP_L01_PRODUCT_LIST_TEMPLATE")
                .Add("GROUP_L01_SHOW_CHILDREN_AS_GROUPS", "GROUP_L01_SHOW_CHILDREN_AS_GROUPS")
                .Add("GROUP_L01_SHOW_PRODUCTS_AS_LIST", "GROUP_L01_SHOW_PRODUCTS_AS_LIST")
                .Add("GROUP_L01_SHOW_IN_NAVIGATION", "GROUP_L01_SHOW_IN_NAVIGATION")
                .Add("GROUP_L01_SHOW_IN_GROUPED_NAV", "GROUP_L01_SHOW_IN_GROUPED_NAV")
                .Add("GROUP_L01_HTML_GROUP", "GROUP_L01_HTML_GROUP")
                .Add("GROUP_L01_HTML_GROUP_TYPE", "GROUP_L01_HTML_GROUP_TYPE")
            End If
            '.Add("GROUP_L01_ADV_SEARCH_TEMPLATE", "GROUP_L01_ADV_SEARCH_TEMPLATE")
            '.Add("GROUP_L01_PRODUCT_PAGE_TEMPLATE", "GROUP_L01_PRODUCT_PAGE_TEMPLATE")
            '.Add("GROUP_L01_PRODUCT_LIST_TEMPLATE", "GROUP_L01_PRODUCT_LIST_TEMPLATE")
            '.Add("GROUP_L01_SHOW_CHILDREN_AS_GROUPS", "GROUP_L01_SHOW_CHILDREN_AS_GROUPS")
            '.Add("GROUP_L01_SHOW_PRODUCTS_AS_LIST", "GROUP_L01_SHOW_PRODUCTS_AS_LIST")
            '.Add("GROUP_L01_SHOW_IN_NAVIGATION", "GROUP_L01_SHOW_IN_NAVIGATION")
            '.Add("GROUP_L01_SHOW_IN_GROUPED_NAV", "GROUP_L01_SHOW_IN_GROUPED_NAV")
            '.Add("GROUP_L01_HTML_GROUP", "GROUP_L01_HTML_GROUP")
            '.Add("GROUP_L01_HTML_GROUP_TYPE", "GROUP_L01_HTML_GROUP_TYPE")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L01_BUSINESS_UNIT", "GROUP_L01_BUSINESS_UNIT")
            .Add("GROUP_L01_PARTNER", "GROUP_L01_PARTNER")
            .Add("GROUP_L01_L01_GROUP", "GROUP_L01_L01_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, insertFieldMappings, keyFields)

        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L01_BUSINESS_UNIT", "GROUP_L01_PARTNER")
        Else
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L01_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)

        '---------------------------------------------------------------------------
        ' Update any default values which would otherwise cause the site to fall over
        ' Eg. overwrite any NULL values with valid values that the site is expecting.
        '---------------------------------------------------------------------------

        If update.Length > 0 Then
            update.Remove(0, update.Length)
        End If

        update.Append("UPDATE " & destinationTable & " SET ")
        update.Append("GROUP_L01_SHOW_PRODUCTS_AS_LIST = 0 WHERE GROUP_L01_SHOW_PRODUCTS_AS_LIST IS NULL;")

        update.Append("UPDATE " & destinationTable & " SET ")
        update.Append("GROUP_L01_HTML_GROUP = 0 WHERE GROUP_L01_HTML_GROUP IS NULL;")

        update.Append("UPDATE " & destinationTable & " SET ")
        update.Append("GROUP_L01_PRODUCT_PAGE_TEMPLATE = ")
        update.Append("(SELECT TOP (1) GROUP_L01_PRODUCT_PAGE_TEMPLATE ")
        update.Append("FROM " & destinationTable & " ORDER BY ")
        update.Append("GROUP_L01_PRODUCT_PAGE_TEMPLATE DESC) WHERE GROUP_L01_PRODUCT_PAGE_TEMPLATE IS NULL")
        update.Append(" OR GROUP_L01_PRODUCT_PAGE_TEMPLATE = '';")

        update.Append("UPDATE " & destinationTable & " SET ")
        update.Append("GROUP_L01_PRODUCT_LIST_TEMPLATE = ")
        update.Append("(SELECT TOP (1) GROUP_L01_PRODUCT_LIST_TEMPLATE ")
        update.Append("FROM " & destinationTable & " ORDER BY ")
        update.Append("GROUP_L01_PRODUCT_LIST_TEMPLATE DESC) WHERE GROUP_L01_PRODUCT_LIST_TEMPLATE IS NULL")
        update.Append(" OR GROUP_L01_PRODUCT_LIST_TEMPLATE = '';")

        DoUpdateTable(update.ToString, String.Empty, String.Empty)

    End Sub

    Public Shared Sub DoUpdateTable_EBGL02(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_02_work"
        Const destinationTable As String = "tbl_group_level_02"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_L02_ID", "GROUP_L02_ID")
            .Add("GROUP_L02_BUSINESS_UNIT", "GROUP_L02_BUSINESS_UNIT")
            .Add("GROUP_L02_PARTNER", "GROUP_L02_PARTNER")
            .Add("GROUP_L02_L01_GROUP", "GROUP_L02_L01_GROUP")
            .Add("GROUP_L02_L02_GROUP", "GROUP_L02_L02_GROUP")
            .Add("GROUP_L02_SEQUENCE", "GROUP_L02_SEQUENCE")
            .Add("GROUP_L02_DESCRIPTION_1", "GROUP_L02_DESCRIPTION_1")
            .Add("GROUP_L02_DESCRIPTION_2", "GROUP_L02_DESCRIPTION_2")
            .Add("GROUP_L02_HTML_1", "GROUP_L02_HTML_1")
            .Add("GROUP_L02_HTML_2", "GROUP_L02_HTML_2")
            .Add("GROUP_L02_HTML_3", "GROUP_L02_HTML_3")
            .Add("GROUP_L02_PAGE_TITLE", "GROUP_L02_PAGE_TITLE")
            .Add("GROUP_L02_META_DESCRIPTION", "GROUP_L02_META_DESCRIPTION")
            .Add("GROUP_L02_META_KEYWORDS", "GROUP_L02_META_KEYWORDS")
            .Add("GROUP_L02_THEME", "GROUP_L02_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L02_BUSINESS_UNIT", "GROUP_L02_BUSINESS_UNIT")
            .Add("GROUP_L02_PARTNER", "GROUP_L02_PARTNER")
            .Add("GROUP_L02_L01_GROUP", "GROUP_L02_L01_GROUP")
            .Add("GROUP_L02_L02_GROUP", "GROUP_L02_L02_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)

        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L02_BUSINESS_UNIT", "GROUP_L02_PARTNER")
        Else
            ' delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L02_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL03(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_03_work"
        Const destinationTable As String = "tbl_group_level_03"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_L03_ID", "GROUP_L03_ID")
            .Add("GROUP_L03_BUSINESS_UNIT", "GROUP_L03_BUSINESS_UNIT")
            .Add("GROUP_L03_PARTNER", "GROUP_L03_PARTNER")
            .Add("GROUP_L03_L01_GROUP", "GROUP_L03_L01_GROUP")
            .Add("GROUP_L03_L02_GROUP", "GROUP_L03_L02_GROUP")
            .Add("GROUP_L03_L03_GROUP", "GROUP_L03_L03_GROUP")
            .Add("GROUP_L03_SEQUENCE", "GROUP_L03_SEQUENCE")
            .Add("GROUP_L03_DESCRIPTION_1", "GROUP_L03_DESCRIPTION_1")
            .Add("GROUP_L03_DESCRIPTION_2", "GROUP_L03_DESCRIPTION_2")
            .Add("GROUP_L03_HTML_1", "GROUP_L03_HTML_1")
            .Add("GROUP_L03_HTML_2", "GROUP_L03_HTML_2")
            .Add("GROUP_L03_HTML_3", "GROUP_L03_HTML_3")
            .Add("GROUP_L03_PAGE_TITLE", "GROUP_L03_PAGE_TITLE")
            .Add("GROUP_L03_META_DESCRIPTION", "GROUP_L03_META_DESCRIPTION")
            .Add("GROUP_L03_META_KEYWORDS", "GROUP_L03_META_KEYWORDS")
            .Add("GROUP_L03_THEME", "GROUP_L03_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L03_BUSINESS_UNIT", "GROUP_L03_BUSINESS_UNIT")
            .Add("GROUP_L03_PARTNER", "GROUP_L03_PARTNER")
            .Add("GROUP_L03_L01_GROUP", "GROUP_L03_L01_GROUP")
            .Add("GROUP_L03_L02_GROUP", "GROUP_L03_L02_GROUP")
            .Add("GROUP_L03_L03_GROUP", "GROUP_L03_L03_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L03_BUSINESS_UNIT", "GROUP_L03_PARTNER")
        Else
            'delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L03_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL04(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_04_work"
        Const destinationTable As String = "tbl_group_level_04"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_L04_ID", "GROUP_L04_ID")
            .Add("GROUP_L04_BUSINESS_UNIT", "GROUP_L04_BUSINESS_UNIT")
            .Add("GROUP_L04_PARTNER", "GROUP_L04_PARTNER")
            .Add("GROUP_L04_L01_GROUP", "GROUP_L04_L01_GROUP")
            .Add("GROUP_L04_L02_GROUP", "GROUP_L04_L02_GROUP")
            .Add("GROUP_L04_L03_GROUP", "GROUP_L04_L03_GROUP")
            .Add("GROUP_L04_L04_GROUP", "GROUP_L04_L04_GROUP")
            .Add("GROUP_L04_SEQUENCE", "GROUP_L04_SEQUENCE")
            .Add("GROUP_L04_DESCRIPTION_1", "GROUP_L04_DESCRIPTION_1")
            .Add("GROUP_L04_DESCRIPTION_2", "GROUP_L04_DESCRIPTION_2")
            .Add("GROUP_L04_HTML_1", "GROUP_L04_HTML_1")
            .Add("GROUP_L04_HTML_2", "GROUP_L04_HTML_2")
            .Add("GROUP_L04_HTML_3", "GROUP_L04_HTML_3")
            .Add("GROUP_L04_PAGE_TITLE", "GROUP_L04_PAGE_TITLE")
            .Add("GROUP_L04_META_DESCRIPTION", "GROUP_L04_META_DESCRIPTION")
            .Add("GROUP_L04_META_KEYWORDS", "GROUP_L04_META_KEYWORDS")
            .Add("GROUP_L04_THEME", "GROUP_L04_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L04_BUSINESS_UNIT", "GROUP_L04_BUSINESS_UNIT")
            .Add("GROUP_L04_PARTNER", "GROUP_L04_PARTNER")
            .Add("GROUP_L04_L01_GROUP", "GROUP_L04_L01_GROUP")
            .Add("GROUP_L04_L02_GROUP", "GROUP_L04_L02_GROUP")
            .Add("GROUP_L04_L03_GROUP", "GROUP_L04_L03_GROUP")
            .Add("GROUP_L04_L04_GROUP", "GROUP_L04_L04_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L04_BUSINESS_UNIT", "GROUP_L04_PARTNER")
        Else
            '  delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L04_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL05(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_05_work"
        Const destinationTable As String = "tbl_group_level_05"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_L05_ID", "GROUP_L05_ID")
            .Add("GROUP_L05_BUSINESS_UNIT", "GROUP_L05_BUSINESS_UNIT")
            .Add("GROUP_L05_PARTNER", "GROUP_L05_PARTNER")
            .Add("GROUP_L05_L01_GROUP", "GROUP_L05_L01_GROUP")
            .Add("GROUP_L05_L02_GROUP", "GROUP_L05_L02_GROUP")
            .Add("GROUP_L05_L03_GROUP", "GROUP_L05_L03_GROUP")
            .Add("GROUP_L05_L04_GROUP", "GROUP_L05_L04_GROUP")
            .Add("GROUP_L05_L05_GROUP", "GROUP_L05_L05_GROUP")
            .Add("GROUP_L05_SEQUENCE", "GROUP_L05_SEQUENCE")
            .Add("GROUP_L05_DESCRIPTION_1", "GROUP_L05_DESCRIPTION_1")
            .Add("GROUP_L05_DESCRIPTION_2", "GROUP_L05_DESCRIPTION_2")
            .Add("GROUP_L05_HTML_1", "GROUP_L05_HTML_1")
            .Add("GROUP_L05_HTML_2", "GROUP_L05_HTML_2")
            .Add("GROUP_L05_HTML_3", "GROUP_L05_HTML_3")
            .Add("GROUP_L05_PAGE_TITLE", "GROUP_L05_PAGE_TITLE")
            .Add("GROUP_L05_META_DESCRIPTION", "GROUP_L05_META_DESCRIPTION")
            .Add("GROUP_L05_META_KEYWORDS", "GROUP_L05_META_KEYWORDS")
            .Add("GROUP_L05_THEME", "GROUP_L05_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L05_BUSINESS_UNIT", "GROUP_L05_BUSINESS_UNIT")
            .Add("GROUP_L05_PARTNER", "GROUP_L05_PARTNER")
            .Add("GROUP_L05_L01_GROUP", "GROUP_L05_L01_GROUP")
            .Add("GROUP_L05_L02_GROUP", "GROUP_L05_L02_GROUP")
            .Add("GROUP_L05_L03_GROUP", "GROUP_L05_L03_GROUP")
            .Add("GROUP_L05_L04_GROUP", "GROUP_L05_L04_GROUP")
            .Add("GROUP_L05_L05_GROUP", "GROUP_L05_L05_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L05_BUSINESS_UNIT", "GROUP_L05_PARTNER")
        Else
            ' delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L05_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL06(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_06_work"
        Const destinationTable As String = "tbl_group_level_06"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("GROUP_L06_BUSINESS_UNIT", "GROUP_L06_BUSINESS_UNIT")
            .Add("GROUP_L06_PARTNER", "GROUP_L06_PARTNER")
            .Add("GROUP_L06_L01_GROUP", "GROUP_L06_L01_GROUP")
            .Add("GROUP_L06_L02_GROUP", "GROUP_L06_L02_GROUP")
            .Add("GROUP_L06_L03_GROUP", "GROUP_L06_L03_GROUP")
            .Add("GROUP_L06_L04_GROUP", "GROUP_L06_L04_GROUP")
            .Add("GROUP_L06_L05_GROUP", "GROUP_L06_L05_GROUP")
            .Add("GROUP_L06_L06_GROUP", "GROUP_L06_L06_GROUP")
            .Add("GROUP_L06_SEQUENCE", "GROUP_L06_SEQUENCE")
            .Add("GROUP_L06_DESCRIPTION_1", "GROUP_L06_DESCRIPTION_1")
            .Add("GROUP_L06_DESCRIPTION_2", "GROUP_L06_DESCRIPTION_2")
            .Add("GROUP_L06_HTML_1", "GROUP_L06_HTML_1")
            .Add("GROUP_L06_HTML_2", "GROUP_L06_HTML_2")
            .Add("GROUP_L06_HTML_3", "GROUP_L06_HTML_3")
            .Add("GROUP_L06_PAGE_TITLE", "GROUP_L06_PAGE_TITLE")
            .Add("GROUP_L06_META_DESCRIPTION", "GROUP_L06_META_DESCRIPTION")
            .Add("GROUP_L06_META_KEYWORDS", "GROUP_L06_META_KEYWORDS")
            .Add("GROUP_L06_THEME", "GROUP_L06_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L06_BUSINESS_UNIT", "GROUP_L06_BUSINESS_UNIT")
            .Add("GROUP_L06_PARTNER", "GROUP_L06_PARTNER")
            .Add("GROUP_L06_L01_GROUP", "GROUP_L06_L01_GROUP")
            .Add("GROUP_L06_L02_GROUP", "GROUP_L06_L02_GROUP")
            .Add("GROUP_L06_L03_GROUP", "GROUP_L06_L03_GROUP")
            .Add("GROUP_L06_L04_GROUP", "GROUP_L06_L04_GROUP")
            .Add("GROUP_L06_L05_GROUP", "GROUP_L06_L05_GROUP")
            .Add("GROUP_L06_L06_GROUP", "GROUP_L06_L06_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L06_BUSINESS_UNIT", "GROUP_L06_PARTNER")
        Else
            '  delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L06_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL07(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_07_work"
        Const destinationTable As String = "tbl_group_level_07"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("GROUP_L07_BUSINESS_UNIT", "GROUP_L07_BUSINESS_UNIT")
            .Add("GROUP_L07_PARTNER", "GROUP_L07_PARTNER")
            .Add("GROUP_L07_L01_GROUP", "GROUP_L07_L01_GROUP")
            .Add("GROUP_L07_L02_GROUP", "GROUP_L07_L02_GROUP")
            .Add("GROUP_L07_L03_GROUP", "GROUP_L07_L03_GROUP")
            .Add("GROUP_L07_L04_GROUP", "GROUP_L07_L04_GROUP")
            .Add("GROUP_L07_L05_GROUP", "GROUP_L07_L05_GROUP")
            .Add("GROUP_L07_L06_GROUP", "GROUP_L07_L06_GROUP")
            .Add("GROUP_L07_L07_GROUP", "GROUP_L07_L07_GROUP")
            .Add("GROUP_L07_SEQUENCE", "GROUP_L07_SEQUENCE")
            .Add("GROUP_L07_DESCRIPTION_1", "GROUP_L07_DESCRIPTION_1")
            .Add("GROUP_L07_DESCRIPTION_2", "GROUP_L07_DESCRIPTION_2")
            .Add("GROUP_L07_HTML_1", "GROUP_L07_HTML_1")
            .Add("GROUP_L07_HTML_2", "GROUP_L07_HTML_2")
            .Add("GROUP_L07_HTML_3", "GROUP_L07_HTML_3")
            .Add("GROUP_L07_PAGE_TITLE", "GROUP_L07_PAGE_TITLE")
            .Add("GROUP_L07_META_DESCRIPTION", "GROUP_L07_META_DESCRIPTION")
            .Add("GROUP_L07_META_KEYWORDS", "GROUP_L07_META_KEYWORDS")
            .Add("GROUP_L07_THEME", "GROUP_L07_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L07_BUSINESS_UNIT", "GROUP_L07_BUSINESS_UNIT")
            .Add("GROUP_L07_PARTNER", "GROUP_L07_PARTNER")
            .Add("GROUP_L07_L01_GROUP", "GROUP_L07_L01_GROUP")
            .Add("GROUP_L07_L02_GROUP", "GROUP_L07_L02_GROUP")
            .Add("GROUP_L07_L03_GROUP", "GROUP_L07_L03_GROUP")
            .Add("GROUP_L07_L04_GROUP", "GROUP_L07_L04_GROUP")
            .Add("GROUP_L07_L05_GROUP", "GROUP_L07_L05_GROUP")
            .Add("GROUP_L07_L06_GROUP", "GROUP_L07_L06_GROUP")
            .Add("GROUP_L07_L07_GROUP", "GROUP_L07_L07_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L07_BUSINESS_UNIT", "GROUP_L07_PARTNER")
        Else
            ' delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L07_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL08(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_08_work"
        Const destinationTable As String = "tbl_group_level_08"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("GROUP_L08_BUSINESS_UNIT", "GROUP_L08_BUSINESS_UNIT")
            .Add("GROUP_L08_PARTNER", "GROUP_L08_PARTNER")
            .Add("GROUP_L08_L01_GROUP", "GROUP_L08_L01_GROUP")
            .Add("GROUP_L08_L02_GROUP", "GROUP_L08_L02_GROUP")
            .Add("GROUP_L08_L03_GROUP", "GROUP_L08_L03_GROUP")
            .Add("GROUP_L08_L04_GROUP", "GROUP_L08_L04_GROUP")
            .Add("GROUP_L08_L05_GROUP", "GROUP_L08_L05_GROUP")
            .Add("GROUP_L08_L06_GROUP", "GROUP_L08_L06_GROUP")
            .Add("GROUP_L08_L07_GROUP", "GROUP_L08_L07_GROUP")
            .Add("GROUP_L08_L08_GROUP", "GROUP_L08_L08_GROUP")
            .Add("GROUP_L08_SEQUENCE", "GROUP_L08_SEQUENCE")
            .Add("GROUP_L08_DESCRIPTION_1", "GROUP_L08_DESCRIPTION_1")
            .Add("GROUP_L08_DESCRIPTION_2", "GROUP_L08_DESCRIPTION_2")
            .Add("GROUP_L08_HTML_1", "GROUP_L08_HTML_1")
            .Add("GROUP_L08_HTML_2", "GROUP_L08_HTML_2")
            .Add("GROUP_L08_HTML_3", "GROUP_L08_HTML_3")
            .Add("GROUP_L08_PAGE_TITLE", "GROUP_L08_PAGE_TITLE")
            .Add("GROUP_L08_META_DESCRIPTION", "GROUP_L08_META_DESCRIPTION")
            .Add("GROUP_L08_META_KEYWORDS", "GROUP_L08_META_KEYWORDS")
            .Add("GROUP_L08_THEME", "GROUP_L08_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L08_BUSINESS_UNIT", "GROUP_L08_BUSINESS_UNIT")
            .Add("GROUP_L08_PARTNER", "GROUP_L08_PARTNER")
            .Add("GROUP_L08_L01_GROUP", "GROUP_L08_L01_GROUP")
            .Add("GROUP_L08_L02_GROUP", "GROUP_L08_L02_GROUP")
            .Add("GROUP_L08_L03_GROUP", "GROUP_L08_L03_GROUP")
            .Add("GROUP_L08_L04_GROUP", "GROUP_L08_L04_GROUP")
            .Add("GROUP_L08_L05_GROUP", "GROUP_L08_L05_GROUP")
            .Add("GROUP_L08_L06_GROUP", "GROUP_L08_L06_GROUP")
            .Add("GROUP_L08_L07_GROUP", "GROUP_L08_L07_GROUP")
            .Add("GROUP_L08_L08_GROUP", "GROUP_L08_L08_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L08_BUSINESS_UNIT", "GROUP_L08_PARTNER")
        Else
            'delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L08_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL09(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_09_work"
        Const destinationTable As String = "tbl_group_level_09"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("GROUP_L09_BUSINESS_UNIT", "GROUP_L09_BUSINESS_UNIT")
            .Add("GROUP_L09_PARTNER", "GROUP_L09_PARTNER")
            .Add("GROUP_L09_L01_GROUP", "GROUP_L09_L01_GROUP")
            .Add("GROUP_L09_L02_GROUP", "GROUP_L09_L02_GROUP")
            .Add("GROUP_L09_L03_GROUP", "GROUP_L09_L03_GROUP")
            .Add("GROUP_L09_L04_GROUP", "GROUP_L09_L04_GROUP")
            .Add("GROUP_L09_L05_GROUP", "GROUP_L09_L05_GROUP")
            .Add("GROUP_L09_L06_GROUP", "GROUP_L09_L06_GROUP")
            .Add("GROUP_L09_L07_GROUP", "GROUP_L09_L07_GROUP")
            .Add("GROUP_L09_L08_GROUP", "GROUP_L09_L08_GROUP")
            .Add("GROUP_L09_L09_GROUP", "GROUP_L09_L09_GROUP")
            .Add("GROUP_L09_SEQUENCE", "GROUP_L09_SEQUENCE")
            .Add("GROUP_L09_DESCRIPTION_1", "GROUP_L09_DESCRIPTION_1")
            .Add("GROUP_L09_DESCRIPTION_2", "GROUP_L09_DESCRIPTION_2")
            .Add("GROUP_L09_HTML_1", "GROUP_L09_HTML_1")
            .Add("GROUP_L09_HTML_2", "GROUP_L09_HTML_2")
            .Add("GROUP_L09_HTML_3", "GROUP_L09_HTML_3")
            .Add("GROUP_L09_PAGE_TITLE", "GROUP_L09_PAGE_TITLE")
            .Add("GROUP_L09_META_DESCRIPTION", "GROUP_L09_META_DESCRIPTION")
            .Add("GROUP_L09_META_KEYWORDS", "GROUP_L09_META_KEYWORDS")
            .Add("GROUP_L09_THEME", "GROUP_L09_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L09_BUSINESS_UNIT", "GROUP_L09_BUSINESS_UNIT")
            .Add("GROUP_L09_PARTNER", "GROUP_L09_PARTNER")
            .Add("GROUP_L09_L01_GROUP", "GROUP_L09_L01_GROUP")
            .Add("GROUP_L09_L02_GROUP", "GROUP_L09_L02_GROUP")
            .Add("GROUP_L09_L03_GROUP", "GROUP_L09_L03_GROUP")
            .Add("GROUP_L09_L04_GROUP", "GROUP_L09_L04_GROUP")
            .Add("GROUP_L09_L05_GROUP", "GROUP_L09_L05_GROUP")
            .Add("GROUP_L09_L06_GROUP", "GROUP_L09_L06_GROUP")
            .Add("GROUP_L09_L07_GROUP", "GROUP_L09_L07_GROUP")
            .Add("GROUP_L09_L08_GROUP", "GROUP_L09_L08_GROUP")
            .Add("GROUP_L09_L09_GROUP", "GROUP_L09_L09_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)

        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L09_BUSINESS_UNIT", "GROUP_L09_PARTNER")
        Else
            'delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L09_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGL10(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_level_10_work"
        Const destinationTable As String = "tbl_group_level_10"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("GROUP_L10_BUSINESS_UNIT", "GROUP_L10_BUSINESS_UNIT")
            .Add("GROUP_L10_PARTNER", "GROUP_L10_PARTNER")
            .Add("GROUP_L10_L01_GROUP", "GROUP_L10_L01_GROUP")
            .Add("GROUP_L10_L02_GROUP", "GROUP_L10_L02_GROUP")
            .Add("GROUP_L10_L03_GROUP", "GROUP_L10_L03_GROUP")
            .Add("GROUP_L10_L04_GROUP", "GROUP_L10_L04_GROUP")
            .Add("GROUP_L10_L05_GROUP", "GROUP_L10_L05_GROUP")
            .Add("GROUP_L10_L06_GROUP", "GROUP_L10_L06_GROUP")
            .Add("GROUP_L10_L07_GROUP", "GROUP_L10_L07_GROUP")
            .Add("GROUP_L10_L08_GROUP", "GROUP_L10_L08_GROUP")
            .Add("GROUP_L10_L09_GROUP", "GROUP_L10_L09_GROUP")
            .Add("GROUP_L10_L10_GROUP", "GROUP_L10_L10_GROUP")
            .Add("GROUP_L10_SEQUENCE", "GROUP_L10_SEQUENCE")
            .Add("GROUP_L10_DESCRIPTION_1", "GROUP_L10_DESCRIPTION_1")
            .Add("GROUP_L10_DESCRIPTION_2", "GROUP_L10_DESCRIPTION_2")
            .Add("GROUP_L10_HTML_1", "GROUP_L10_HTML_1")
            .Add("GROUP_L10_HTML_2", "GROUP_L10_HTML_2")
            .Add("GROUP_L10_HTML_3", "GROUP_L10_HTML_3")
            .Add("GROUP_L10_PAGE_TITLE", "GROUP_L10_PAGE_TITLE")
            .Add("GROUP_L10_META_DESCRIPTION", "GROUP_L10_META_DESCRIPTION")
            .Add("GROUP_L10_META_KEYWORDS", "GROUP_L10_META_KEYWORDS")
            .Add("GROUP_L10_THEME", "GROUP_L10_THEME")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_L10_BUSINESS_UNIT", "GROUP_L10_BUSINESS_UNIT")
            .Add("GROUP_L10_PARTNER", "GROUP_L10_PARTNER")
            .Add("GROUP_L10_L01_GROUP", "GROUP_L10_L01_GROUP")
            .Add("GROUP_L10_L02_GROUP", "GROUP_L10_L02_GROUP")
            .Add("GROUP_L10_L03_GROUP", "GROUP_L10_L03_GROUP")
            .Add("GROUP_L10_L04_GROUP", "GROUP_L10_L04_GROUP")
            .Add("GROUP_L10_L05_GROUP", "GROUP_L10_L05_GROUP")
            .Add("GROUP_L10_L06_GROUP", "GROUP_L10_L06_GROUP")
            .Add("GROUP_L10_L07_GROUP", "GROUP_L10_L07_GROUP")
            .Add("GROUP_L10_L08_GROUP", "GROUP_L10_L08_GROUP")
            .Add("GROUP_L10_L09_GROUP", "GROUP_L10_L09_GROUP")
            .Add("GROUP_L10_L10_GROUP", "GROUP_L10_L10_GROUP")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_L10_BUSINESS_UNIT", "GROUP_L10_PARTNER")
        Else
            '    delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_L10_ADHOC_GROUP")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGROU()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_work"
        Const destinationTable As String = "tbl_group"


        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_ID", "GROUP_ID")
            .Add("GROUP_NAME", "GROUP_NAME")
            .Add("GROUP_DESCRIPTION_1", "GROUP_DESCRIPTION_1")
            .Add("GROUP_DESCRIPTION_2", "GROUP_DESCRIPTION_2")
            .Add("GROUP_HTML_1", "GROUP_HTML_1")
            .Add("GROUP_HTML_2", "GROUP_HTML_2")
            .Add("GROUP_HTML_3", "GROUP_HTML_3")
            .Add("GROUP_PAGE_TITLE", "GROUP_PAGE_TITLE")
            .Add("GROUP_META_DESCRIPTION", "GROUP_META_DESCRIPTION")
            .Add("GROUP_META_KEYWORDS", "GROUP_META_KEYWORDS")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_NAME", "GROUP_NAME")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_ADHOC")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBSTOCK()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_stock_work"
        Const destinationTable As String = "tbl_product_stock"


        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("PRODUCT", "PRODUCT")
            .Add("STOCK_LOCATION", "STOCK_LOCATION")
            .Add("QUANTITY", "QUANTITY")
            .Add("ALLOCATED_QUANTITY", "ALLOCATED_QUANTITY")
            .Add("AVAILABLE_QUANTITY", "AVAILABLE_QUANTITY")
            .Add("RESTOCK_CODE", "RESTOCK_CODE")
            .Add("WAREHOUSE", "WAREHOUSE")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PRODUCT", "PRODUCT")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_tbl_group_lang()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_lang_work"
        Const destinationTable As String = "tbl_group_lang"


        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("GROUP_CODE", "GROUP_CODE")
            .Add("GROUP_LANGUAGE", "GROUP_LANGUAGE")
            .Add("GROUP_DESCRIPTION_1", "GROUP_DESCRIPTION_1")
            .Add("GROUP_DESCRIPTION_2", "GROUP_DESCRIPTION_2")
            .Add("GROUP_HTML_1", "GROUP_HTML_1")
            .Add("GROUP_HTML_2", "GROUP_HTML_2")
            .Add("GROUP_HTML_3", "GROUP_HTML_3")
            .Add("GROUP_PAGE_TITLE", "GROUP_PAGE_TITLE")
            .Add("GROUP_META_DESCRIPTION", "GROUP_META_DESCRIPTION")
            .Add("GROUP_META_KEYWORDS", "GROUP_META_KEYWORDS")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_CODE", "GROUP_CODE")
            .Add("GROUP_LANGUAGE", "GROUP_LANGUAGE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBGPPR(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_group_product_work"
        Const destinationTable As String = "tbl_group_product"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("GROUP_PRODUCT_ID", "GROUP_PRODUCT_ID")
            .Add("GROUP_BUSINESS_UNIT", "GROUP_BUSINESS_UNIT")
            .Add("GROUP_PARTNER", "GROUP_PARTNER")
            .Add("GROUP_L01_GROUP", "GROUP_L01_GROUP")
            .Add("GROUP_L02_GROUP", "GROUP_L02_GROUP")
            .Add("GROUP_L03_GROUP", "GROUP_L03_GROUP")
            .Add("GROUP_L04_GROUP", "GROUP_L04_GROUP")
            .Add("GROUP_L05_GROUP", "GROUP_L05_GROUP")
            .Add("GROUP_L06_GROUP", "GROUP_L06_GROUP")
            .Add("GROUP_L07_GROUP", "GROUP_L07_GROUP")
            .Add("GROUP_L08_GROUP", "GROUP_L08_GROUP")
            .Add("GROUP_L09_GROUP", "GROUP_L09_GROUP")
            .Add("GROUP_L10_GROUP", "GROUP_L10_GROUP")
            .Add("PRODUCT", "PRODUCT")
            .Add("SEQUENCE", "SEQUENCE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("GROUP_BUSINESS_UNIT", "GROUP_BUSINESS_UNIT")
            .Add("GROUP_PARTNER", "GROUP_PARTNER")
            .Add("GROUP_L01_GROUP", "GROUP_L01_GROUP")
            .Add("GROUP_L02_GROUP", "GROUP_L02_GROUP")
            .Add("GROUP_L03_GROUP", "GROUP_L03_GROUP")
            .Add("GROUP_L04_GROUP", "GROUP_L04_GROUP")
            .Add("GROUP_L05_GROUP", "GROUP_L05_GROUP")
            .Add("GROUP_L06_GROUP", "GROUP_L06_GROUP")
            .Add("GROUP_L07_GROUP", "GROUP_L07_GROUP")
            .Add("GROUP_L08_GROUP", "GROUP_L08_GROUP")
            .Add("GROUP_L09_GROUP", "GROUP_L09_GROUP")
            .Add("GROUP_L10_GROUP", "GROUP_L10_GROUP")
            .Add("PRODUCT", "PRODUCT")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "GROUP_BUSINESS_UNIT", "GROUP_PARTNER")
        Else
            delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "GROUP_ADHOC")
        End If

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBORDHT()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_order_header_work"
        Const destinationTable As String = "tbl_order_header"

        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("PROCESSED_ORDER_ID", "PROCESSED_ORDER_ID")
            .Add("BACK_OFFICE_ORDER_ID", "BACK_OFFICE_ORDER_ID")
            .Add("BACK_OFFICE_STATUS", "BACK_OFFICE_STATUS")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PROCESSED_ORDER_ID", "PROCESSED_ORDER_ID")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        Dim s As String = update.ToString

        Dim fieldMappings2 As New Generic.Dictionary(Of String, String)
        With fieldMappings2
            .Add("PROCESSED_ORDER_ID", "PROCESSED_ORDER_ID")
            .Add("TEMP_ORDER_ID", "TEMP_ORDER_ID")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("PARTNER_NUMBER", "PARTNER_NUMBER")
            .Add("LOGINID", "LOGINID")
            .Add("USER_NUMBER", "USER_NUMBER")
            .Add("STATUS", "STATUS")
            .Add("COMMENT", "COMMENT")
            .Add("CONTACT_NAME", "CONTACT_NAME")
            .Add("ADDRESS_LINE_1", "ADDRESS_LINE_1")
            .Add("ADDRESS_LINE_2", "ADDRESS_LINE_2")
            .Add("ADDRESS_LINE_3", "ADDRESS_LINE_3")
            .Add("ADDRESS_LINE_4", "ADDRESS_LINE_4")
            .Add("ADDRESS_LINE_5", "ADDRESS_LINE_5")
            .Add("POSTCODE", "POSTCODE")
            .Add("COUNTRY", "COUNTRY")
            .Add("CONTACT_PHONE", "CONTACT_PHONE")
            .Add("CONTACT_EMAIL", "CONTACT_EMAIL")
            .Add("CREATED_DATE", "CREATED_DATE")
            .Add("SHIPPED_DATE", "SHIPPED_DATE")
            .Add("LAST_ACTIVITY_DATE", "LAST_ACTIVITY_DATE")
            .Add("TOTAL_ORDER_ITEMS_VALUE_GROSS", "TOTAL_ORDER_ITEMS_VALUE_GROSS")
            .Add("TOTAL_ORDER_ITEMS_VALUE_NET", "TOTAL_ORDER_ITEMS_VALUE_NET")
            .Add("TOTAL_ORDER_ITEMS_TAX", "TOTAL_ORDER_ITEMS_TAX")
            .Add("TOTAL_DELIVERY_GROSS", "TOTAL_DELIVERY_GROSS")
            .Add("TOTAL_DELIVERY_NET", "TOTAL_DELIVERY_NET")
            .Add("TOTAL_DELIVERY_TAX", "TOTAL_DELIVERY_TAX")
            .Add("PROMOTION_DESCRIPTION", "PROMOTION_DESCRIPTION")
            .Add("PROMOTION_VALUE", "PROMOTION_VALUE")
            .Add("TOTAL_ORDER_VALUE", "TOTAL_ORDER_VALUE")
            .Add("TOTAL_AMOUNT_CHARGED", "TOTAL_AMOUNT_CHARGED")
            .Add("SPECIAL_INSTRUCTIONS_1", "SPECIAL_INSTRUCTIONS_1")
            .Add("SPECIAL_INSTRUCTIONS_2", "SPECIAL_INSTRUCTIONS_2")
            .Add("TRACKING_NO", "TRACKING_NO")
            .Add("BACK_OFFICE_ORDER_ID", "BACK_OFFICE_ORDER_ID")
            .Add("BACK_OFFICE_STATUS", "BACK_OFFICE_STATUS")
            .Add("BACK_OFFICE_REFERENCE", "BACK_OFFICE_REFERENCE")
            .Add("LANGUAGE", "LANGUAGE")
            .Add("WAREHOUSE", "WAREHOUSE")
            .Add("CURRENCY", "CURRENCY")
            .Add("PAYMENT_TYPE", "PAYMENT_TYPE")
            .Add("SHIPPING_CODE", "SHIPPING_CODE")
            .Add("TAX_INCLUSIVE_1", "TAX_INCLUSIVE_1")
            .Add("TAX_DISPLAY_1", "TAX_DISPLAY_1")
            .Add("TAX_INCLUSIVE_2", "TAX_INCLUSIVE_2")
            .Add("TAX_DISPLAY_2", "TAX_DISPLAY_2")
            .Add("TAX_INCLUSIVE_3", "TAX_INCLUSIVE_3")
            .Add("TAX_DISPLAY_3", "TAX_DISPLAY_3")
            .Add("TAX_INCLUSIVE_4", "TAX_INCLUSIVE_4")
            .Add("TAX_DISPLAY_4", "TAX_DISPLAY_4")
            .Add("TAX_INCLUSIVE_5", "TAX_INCLUSIVE_5")
            .Add("TAX_DISPLAY_5", "TAX_DISPLAY_5")
            .Add("TAX_CODE_1", "TAX_CODE_1")
            .Add("TAX_AMOUNT_1", "TAX_AMOUNT_1")
            .Add("TAX_CODE_2", "TAX_CODE_2")
            .Add("TAX_AMOUNT_2", "TAX_AMOUNT_2")
            .Add("TAX_CODE_3", "TAX_CODE_3")
            .Add("TAX_AMOUNT_3", "TAX_AMOUNT_3")
            .Add("TAX_CODE_4", "TAX_CODE_4")
            .Add("TAX_AMOUNT_4", "TAX_AMOUNT_4")
            .Add("TAX_CODE_5", "TAX_CODE_5")
            .Add("TAX_AMOUNT_5", "TAX_AMOUNT_5")
            .Add("GIFT_MESSAGE", "GIFT_MESSAGE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields2 As New Generic.Dictionary(Of String, String)
        With keyFields2
            .Add("PROCESSED_ORDER_ID", "PROCESSED_ORDER_ID")
        End With

        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings2, keyFields2)

        DoUpdateTableNoDelete(update.ToString, insert.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBORDDT()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_order_detail_work"
        Const destinationTable As String = "tbl_order_detail"
        '-------------------------------------------
        ' Only update fields which change on backend
        '-------------------------------------------
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("QUANTITY_SHIPPED", "QUANTITY_SHIPPED")
            .Add("SHIPMENT_NUMBER", "SHIPMENT_NUMBER")
            .Add("TRACKING_NO", "TRACKING_NO")
            .Add("BACK_OFFICE_STATUS", "BACK_OFFICE_STATUS")
            .Add("DATE_SHIPPED ", "DATE_SHIPPED")
        End With

        Dim fieldMappings2 As New Generic.Dictionary(Of String, String)
        With fieldMappings2
            '.Add("ORDER_DETAIL_ID", "ORDER_DETAIL_ID")
            .Add("ORDER_ID", "ORDER_ID")
            .Add("LINE_NUMBER", "LINE_NUMBER")
            .Add("GROUP_LEVEL_01", "GROUP_LEVEL_01")
            .Add("GROUP_LEVEL_02", "GROUP_LEVEL_02")
            .Add("GROUP_LEVEL_03", "GROUP_LEVEL_03")
            .Add("GROUP_LEVEL_04", "GROUP_LEVEL_04")
            .Add("GROUP_LEVEL_05", "GROUP_LEVEL_05")
            .Add("GROUP_LEVEL_06", "GROUP_LEVEL_06")
            .Add("GROUP_LEVEL_07", "GROUP_LEVEL_07")
            .Add("GROUP_LEVEL_08", "GROUP_LEVEL_08")
            .Add("GROUP_LEVEL_09", "GROUP_LEVEL_09")
            .Add("GROUP_LEVEL_10", "GROUP_LEVEL_10")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("QUANTITY", "QUANTITY")
            .Add("QUANTITY_SHIPPED", "QUANTITY_SHIPPED")
            .Add("DATE_SHIPPED", "DATE_SHIPPED")
            .Add("PRODUCT_DESCRIPTION_1", "PRODUCT_DESCRIPTION_1")
            .Add("PRODUCT_DESCRIPTION_2", "PRODUCT_DESCRIPTION_2")
            .Add("PRODUCT_SUPPLIER", "PRODUCT_SUPPLIER")
            .Add("PURCHASE_PRICE_GROSS", "PURCHASE_PRICE_GROSS")
            .Add("PURCHASE_PRICE_NET", "PURCHASE_PRICE_NET")
            .Add("PURCHASE_PRICE_TAX", "PURCHASE_PRICE_TAX")
            .Add("DELIVERY_GROSS", "DELIVERY_GROSS")
            .Add("DELIVERY_NET", "DELIVERY_NET")
            .Add("DELIVERY_TAX", "DELIVERY_TAX")
            .Add("TAX_CODE", "TAX_CODE")
            .Add("LINE_PRICE_GROSS", "LINE_PRICE_GROSS")
            .Add("LINE_PRICE_NET", "LINE_PRICE_NET")
            .Add("LINE_PRICE_TAX", "LINE_PRICE_TAX")
            .Add("SHIPMENT_NUMBER", "SHIPMENT_NUMBER")
            .Add("TRACKING_NO", "TRACKING_NO")
            .Add("BACK_OFFICE_STATUS", "BACK_OFFICE_STATUS")
            .Add("CONTACT_NAME", "CONTACT_NAME")
            .Add("ADDRESS_LINE_1", "ADDRESS_LINE_1")
            .Add("ADDRESS_LINE_2", "ADDRESS_LINE_2")
            .Add("ADDRESS_LINE_3", "ADDRESS_LINE_3")
            .Add("ADDRESS_LINE_4", "ADDRESS_LINE_4")
            .Add("ADDRESS_LINE_5", "ADDRESS_LINE_5")
            .Add("POSTCODE", "POSTCODE")
            .Add("COUNTRY", "COUNTRY")
            .Add("CONTACT_PHONE", "CONTACT_PHONE")
            .Add("CONTACT_EMAIL", "CONTACT_EMAIL")
            .Add("LANGUAGE", "LANGUAGE")
            .Add("WAREHOUSE", "WAREHOUSE")
            .Add("CURRENCY", "CURRENCY")
            .Add("SHIPPING_CODE", "SHIPPING_CODE")
            .Add("TAX_AMOUNT_1", "TAX_AMOUNT_1")
            .Add("TAX_AMOUNT_2", "TAX_AMOUNT_2")
            .Add("TAX_AMOUNT_3", "TAX_AMOUNT_3")
            .Add("TAX_AMOUNT_4", "TAX_AMOUNT_4")
            .Add("TAX_AMOUNT_5", "TAX_AMOUNT_5")
            .Add("TARIFF_CODE", "TARIFF_CODE")
            .Add("HEADER_ORDER_ID", "ORDER_ID")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("HEADER_ORDER_ID", "ORDER_ID")
            .Add("LINE_NUMBER", "LINE_NUMBER")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings2, keyFields)
        'delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTableNoDelete(update.ToString, insert.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPLHD()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_price_list_header_work"
        Const destinationTable As String = "tbl_price_list_header"


        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("PRICE_LIST_ID", "PRICE_LIST_ID")
            .Add("PRICE_LIST", "PRICE_LIST")
            .Add("PRICE_LIST_DESCRIPTION", "PRICE_LIST_DESCRIPTION")
            .Add("CURRENCY_CODE", "CURRENCY_CODE")
            .Add("FREE_DELIVERY_VALUE", "FREE_DELIVERY_VALUE")
            .Add("MIN_DELIVERY_VALUE", "MIN_DELIVERY_VALUE")
            .Add("START_DATE", "START_DATE")
            .Add("END_DATE", "END_DATE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PRICE_LIST", "PRICE_LIST")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPLDT(ByVal replace As Boolean, Optional ByVal priceList As String = "")

        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Dim truncate As New StringBuilder

        Const sourceTable As String = "tbl_price_list_detail_work"
        Const destinationTable As String = "tbl_price_list_detail"

        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("PRICE_LIST_DETAIL_ID", "PRICE_LIST_DETAIL_ID")
            .Add("PRICE_LIST", "PRICE_LIST")
            .Add("PRODUCT", "PRODUCT")
            .Add("FROM_DATE", "FROM_DATE")
            .Add("TO_DATE", "TO_DATE")
            .Add("NET_PRICE", "NET_PRICE")
            .Add("GROSS_PRICE", "GROSS_PRICE")
            .Add("TAX_AMOUNT", "TAX_AMOUNT")
            .Add("SALE_NET_PRICE", "SALE_NET_PRICE")
            .Add("SALE_GROSS_PRICE", "SALE_GROSS_PRICE")
            .Add("SALE_TAX_AMOUNT", "SALE_TAX_AMOUNT")
            .Add("DELIVERY_NET_PRICE", "DELIVERY_NET_PRICE")
            .Add("DELIVERY_GROSS_PRICE", "DELIVERY_GROSS_PRICE")
            .Add("DELIVERY_TAX_AMOUNT", "DELIVERY_TAX_AMOUNT")
            .Add("PRICE_1", "PRICE_1")
            .Add("PRICE_2", "PRICE_2")
            .Add("PRICE_3", "PRICE_3")
            .Add("PRICE_4", "PRICE_4")
            .Add("PRICE_5", "PRICE_5")
            .Add("TAX_CODE", "TAX_CODE")
            .Add("TARIFF_CODE", "TARIFF_CODE")
            ' bf - reinstate price break code
            .Add("PRICE_BREAK_CODE", "PRICE_BREAK_CODE")
            .Add("PRICE_BREAK_QUANTITY_1", "PRICE_BREAK_QUANTITY_1")
            .Add("SALE_PRICE_BREAK_QUANTITY_1", "SALE_PRICE_BREAK_QUANTITY_1")
            .Add("NET_PRICE_2", "NET_PRICE_2")
            .Add("GROSS_PRICE_2", "GROSS_PRICE_2")
            .Add("TAX_AMOUNT_2", "TAX_AMOUNT_2")
            .Add("PRICE_BREAK_QUANTITY_2", "PRICE_BREAK_QUANTITY_2")
            .Add("SALE_NET_PRICE_2", "SALE_NET_PRICE_2")
            .Add("SALE_GROSS_PRICE_2", "SALE_GROSS_PRICE_2")
            .Add("SALE_TAX_AMOUNT_2", "SALE_TAX_AMOUNT_2")
            .Add("SALE_PRICE_BREAK_QUANTITY_2", "SALE_PRICE_BREAK_QUANTITY_2")
            .Add("NET_PRICE_3", "NET_PRICE_3")
            .Add("GROSS_PRICE_3", "GROSS_PRICE_3")
            .Add("TAX_AMOUNT_3", "TAX_AMOUNT_3")
            .Add("PRICE_BREAK_QUANTITY_3", "PRICE_BREAK_QUANTITY_3")
            .Add("SALE_NET_PRICE_3", "SALE_NET_PRICE_3")
            .Add("SALE_GROSS_PRICE_3", "SALE_GROSS_PRICE_3")
            .Add("SALE_TAX_AMOUNT_3", "SALE_TAX_AMOUNT_3")
            .Add("SALE_PRICE_BREAK_QUANTITY_3", "SALE_PRICE_BREAK_QUANTITY_3")
            .Add("NET_PRICE_4", "NET_PRICE_4")
            .Add("GROSS_PRICE_4", "GROSS_PRICE_4")
            .Add("TAX_AMOUNT_4", "TAX_AMOUNT_4")
            .Add("PRICE_BREAK_QUANTITY_4", "PRICE_BREAK_QUANTITY_4")
            .Add("SALE_NET_PRICE_4", "SALE_NET_PRICE_4")
            .Add("SALE_GROSS_PRICE_4", "SALE_GROSS_PRICE_4")
            .Add("SALE_TAX_AMOUNT_4", "SALE_TAX_AMOUNT_4")
            .Add("SALE_PRICE_BREAK_QUANTITY_4", "SALE_PRICE_BREAK_QUANTITY_4")
            .Add("NET_PRICE_5", "NET_PRICE_5")
            .Add("GROSS_PRICE_5", "GROSS_PRICE_5")
            .Add("TAX_AMOUNT_5", "TAX_AMOUNT_5")
            .Add("PRICE_BREAK_QUANTITY_5", "PRICE_BREAK_QUANTITY_5")
            .Add("SALE_NET_PRICE_5", "SALE_NET_PRICE_5")
            .Add("SALE_GROSS_PRICE_5", "SALE_GROSS_PRICE_5")
            .Add("SALE_TAX_AMOUNT_5", "SALE_TAX_AMOUNT_5")
            .Add("SALE_PRICE_BREAK_QUANTITY_5", "SALE_PRICE_BREAK_QUANTITY_5")
            .Add("NET_PRICE_6", "NET_PRICE_6")
            .Add("GROSS_PRICE_6", "GROSS_PRICE_6")
            .Add("TAX_AMOUNT_6", "TAX_AMOUNT_6")
            .Add("PRICE_BREAK_QUANTITY_6", "PRICE_BREAK_QUANTITY_6")
            .Add("SALE_NET_PRICE_6", "SALE_NET_PRICE_6")
            .Add("SALE_GROSS_PRICE_6", "SALE_GROSS_PRICE_6")
            .Add("SALE_TAX_AMOUNT_6", "SALE_TAX_AMOUNT_6")
            .Add("SALE_PRICE_BREAK_QUANTITY_6", "SALE_PRICE_BREAK_QUANTITY_6")
            .Add("NET_PRICE_7", "NET_PRICE_7")
            .Add("GROSS_PRICE_7", "GROSS_PRICE_7")
            .Add("TAX_AMOUNT_7", "TAX_AMOUNT_7")
            .Add("PRICE_BREAK_QUANTITY_7", "PRICE_BREAK_QUANTITY_7")
            .Add("SALE_NET_PRICE_7", "SALE_NET_PRICE_7")
            .Add("SALE_GROSS_PRICE_7", "SALE_GROSS_PRICE_7")
            .Add("SALE_TAX_AMOUNT_7", "SALE_TAX_AMOUNT_7")
            .Add("SALE_PRICE_BREAK_QUANTITY_7", "SALE_PRICE_BREAK_QUANTITY_7")
            .Add("NET_PRICE_8", "NET_PRICE_8")
            .Add("GROSS_PRICE_8", "GROSS_PRICE_8")
            .Add("TAX_AMOUNT_8", "TAX_AMOUNT_8")
            .Add("PRICE_BREAK_QUANTITY_8", "PRICE_BREAK_QUANTITY_8")
            .Add("SALE_NET_PRICE_8", "SALE_NET_PRICE_8")
            .Add("SALE_GROSS_PRICE_8", "SALE_GROSS_PRICE_8")
            .Add("SALE_TAX_AMOUNT_8", "SALE_TAX_AMOUNT_8")
            .Add("SALE_PRICE_BREAK_QUANTITY_8", "SALE_PRICE_BREAK_QUANTITY_8")
            .Add("NET_PRICE_9", "NET_PRICE_9")
            .Add("GROSS_PRICE_9", "GROSS_PRICE_9")
            .Add("TAX_AMOUNT_9", "TAX_AMOUNT_9")
            .Add("PRICE_BREAK_QUANTITY_9", "PRICE_BREAK_QUANTITY_9")
            .Add("SALE_NET_PRICE_9", "SALE_NET_PRICE_9")
            .Add("SALE_GROSS_PRICE_9", "SALE_GROSS_PRICE_9")
            .Add("SALE_TAX_AMOUNT_9", "SALE_TAX_AMOUNT_9")
            .Add("SALE_PRICE_BREAK_QUANTITY_9", "SALE_PRICE_BREAK_QUANTITY_9")
            .Add("NET_PRICE_10", "NET_PRICE_10")
            .Add("GROSS_PRICE_10", "GROSS_PRICE_10")
            .Add("TAX_AMOUNT_10", "TAX_AMOUNT_10")
            .Add("PRICE_BREAK_QUANTITY_10", "PRICE_BREAK_QUANTITY_10")
            .Add("SALE_NET_PRICE_10", "SALE_NET_PRICE_10")
            .Add("SALE_GROSS_PRICE_10", "SALE_GROSS_PRICE_10")
            .Add("SALE_TAX_AMOUNT_10", "SALE_TAX_AMOUNT_10")
            .Add("SALE_PRICE_BREAK_QUANTITY_10", "SALE_PRICE_BREAK_QUANTITY_10")
            .Add("DELIVERY_NET_PRICE_2", "DELIVERY_NET_PRICE_2")
            .Add("DELIVERY_GROSS_PRICE_2", "DELIVERY_GROSS_PRICE_2")
            .Add("DELIVERY_TAX_AMOUNT_2", "DELIVERY_TAX_AMOUNT_2")
            .Add("DELIVERY_SALE_NET_PRICE_2", "DELIVERY_SALE_NET_PRICE_2")
            .Add("DELIVERY_SALE_GROSS_PRICE_2", "DELIVERY_SALE_GROSS_PRICE_2")
            .Add("DELIVERY_SALE_TAX_AMOUNT_2", "DELIVERY_SALE_TAX_AMOUNT_2")
            .Add("DELIVERY_NET_PRICE_3", "DELIVERY_NET_PRICE_3")
            .Add("DELIVERY_GROSS_PRICE_3", "DELIVERY_GROSS_PRICE_3")
            .Add("DELIVERY_TAX_AMOUNT_3", "DELIVERY_TAX_AMOUNT_3")
            .Add("DELIVERY_SALE_NET_PRICE_3", "DELIVERY_SALE_NET_PRICE_3")
            .Add("DELIVERY_SALE_GROSS_PRICE_3", "DELIVERY_SALE_GROSS_PRICE_3")
            .Add("DELIVERY_SALE_TAX_AMOUNT_3", "DELIVERY_SALE_TAX_AMOUNT_3")
            .Add("DELIVERY_NET_PRICE_4", "DELIVERY_NET_PRICE_4")
            .Add("DELIVERY_GROSS_PRICE_4", "DELIVERY_GROSS_PRICE_4")
            .Add("DELIVERY_TAX_AMOUNT_4", "DELIVERY_TAX_AMOUNT_4")
            .Add("DELIVERY_SALE_NET_PRICE_4", "DELIVERY_SALE_NET_PRICE_4")
            .Add("DELIVERY_SALE_GROSS_PRICE_4", "DELIVERY_SALE_GROSS_PRICE_4")
            .Add("DELIVERY_SALE_TAX_AMOUNT_4", "DELIVERY_SALE_TAX_AMOUNT_4")
            .Add("DELIVERY_NET_PRICE_5", "DELIVERY_NET_PRICE_5")
            .Add("DELIVERY_GROSS_PRICE_5", "DELIVERY_GROSS_PRICE_5")
            .Add("DELIVERY_TAX_AMOUNT_5", "DELIVERY_TAX_AMOUNT_5")
            .Add("DELIVERY_SALE_NET_PRICE_5", "DELIVERY_SALE_NET_PRICE_5")
            .Add("DELIVERY_SALE_GROSS_PRICE_5", "DELIVERY_SALE_GROSS_PRICE_5")
            .Add("DELIVERY_SALE_TAX_AMOUNT_5", "DELIVERY_SALE_TAX_AMOUNT_5")
            .Add("DELIVERY_NET_PRICE_6", "DELIVERY_NET_PRICE_6")
            .Add("DELIVERY_GROSS_PRICE_6", "DELIVERY_GROSS_PRICE_6")
            .Add("DELIVERY_TAX_AMOUNT_6", "DELIVERY_TAX_AMOUNT_6")
            .Add("DELIVERY_SALE_NET_PRICE_6", "DELIVERY_SALE_NET_PRICE_6")
            .Add("DELIVERY_SALE_GROSS_PRICE_6", "DELIVERY_SALE_GROSS_PRICE_6")
            .Add("DELIVERY_SALE_TAX_AMOUNT_6", "DELIVERY_SALE_TAX_AMOUNT_6")
            .Add("DELIVERY_NET_PRICE_7", "DELIVERY_NET_PRICE_7")
            .Add("DELIVERY_GROSS_PRICE_7", "DELIVERY_GROSS_PRICE_7")
            .Add("DELIVERY_TAX_AMOUNT_7", "DELIVERY_TAX_AMOUNT_7")
            .Add("DELIVERY_SALE_NET_PRICE_7", "DELIVERY_SALE_NET_PRICE_7")
            .Add("DELIVERY_SALE_GROSS_PRICE_7", "DELIVERY_SALE_GROSS_PRICE_7")
            .Add("DELIVERY_SALE_TAX_AMOUNT_7", "DELIVERY_SALE_TAX_AMOUNT_7")
            .Add("DELIVERY_NET_PRICE_8", "DELIVERY_NET_PRICE_8")
            .Add("DELIVERY_GROSS_PRICE_8", "DELIVERY_GROSS_PRICE_8")
            .Add("DELIVERY_TAX_AMOUNT_8", "DELIVERY_TAX_AMOUNT_8")
            .Add("DELIVERY_SALE_NET_PRICE_8", "DELIVERY_SALE_NET_PRICE_8")
            .Add("DELIVERY_SALE_GROSS_PRICE_8", "DELIVERY_SALE_GROSS_PRICE_8")
            .Add("DELIVERY_SALE_TAX_AMOUNT_8", "DELIVERY_SALE_TAX_AMOUNT_8")
            .Add("DELIVERY_NET_PRICE_9", "DELIVERY_NET_PRICE_9")
            .Add("DELIVERY_GROSS_PRICE_9", "DELIVERY_GROSS_PRICE_9")
            .Add("DELIVERY_TAX_AMOUNT_9", "DELIVERY_TAX_AMOUNT_9")
            .Add("DELIVERY_SALE_NET_PRICE_9", "DELIVERY_SALE_NET_PRICE_9")
            .Add("DELIVERY_SALE_GROSS_PRICE_9", "DELIVERY_SALE_GROSS_PRICE_9")
            .Add("DELIVERY_SALE_TAX_AMOUNT_9", "DELIVERY_SALE_TAX_AMOUNT_9")
            .Add("DELIVERY_NET_PRICE_10", "DELIVERY_NET_PRICE_10")
            .Add("DELIVERY_GROSS_PRICE_10", "DELIVERY_GROSS_PRICE_10")
            .Add("DELIVERY_TAX_AMOUNT_10", "DELIVERY_TAX_AMOUNT_10")
            .Add("DELIVERY_SALE_NET_PRICE_10", "DELIVERY_SALE_NET_PRICE_10")
            .Add("DELIVERY_SALE_GROSS_PRICE_10", "DELIVERY_SALE_GROSS_PRICE_10")
            .Add("DELIVERY_SALE_TAX_AMOUNT_10", "DELIVERY_SALE_TAX_AMOUNT_10")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PRICE_LIST", "PRICE_LIST")
            .Add("PRODUCT", "PRODUCT")
        End With

        'Populate the update and insert stringbuilders
        If Not replace Then
            update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
            insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
            delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

            DoUpdateTable(update.ToString, insert.ToString, delete.ToString)

        Else
            insert = BuildReplaceInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
            '----------------------------------------------------------
            ' If price list is passed, only delete from that price list
            '----------------------------------------------------------
            If priceList <> String.Empty Then
                With delete
                    .Append("Delete From ")
                    .Append(destinationTable)
                    .Append(" WHERE PRICE_LIST = '" & priceList & "'")
                End With
                DoReplaceUpdateTable(delete.ToString, insert.ToString)
            Else
                With truncate
                    .Append("TRUNCATE TABLE ")
                    .Append(destinationTable)
                End With
                DoReplaceTruncInsertTable(truncate.ToString, insert.ToString)
            End If
        End If
    End Sub

    Private Shared Sub DoUpdateTable_EBPLDTP()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder
        Dim selectString As New StringBuilder

        Const sourceTable As String = "tbl_price_list_detail_work"
        Const destinationTable As String = "tbl_price_list_detail"

        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("PRICE_LIST", "PRICE_LIST")
            .Add("PRODUCT", "PRODUCT")
            .Add("FROM_DATE", "FROM_DATE")
            .Add("TO_DATE", "TO_DATE")
            .Add("NET_PRICE", "NET_PRICE")
            .Add("GROSS_PRICE", "GROSS_PRICE")
            .Add("TAX_AMOUNT", "TAX_AMOUNT")
            .Add("SALE_NET_PRICE", "SALE_NET_PRICE")
            .Add("SALE_GROSS_PRICE", "SALE_GROSS_PRICE")
            .Add("SALE_TAX_AMOUNT", "SALE_TAX_AMOUNT")
            .Add("DELIVERY_NET_PRICE", "DELIVERY_NET_PRICE")
            .Add("DELIVERY_GROSS_PRICE", "DELIVERY_GROSS_PRICE")
            .Add("DELIVERY_TAX_AMOUNT", "DELIVERY_TAX_AMOUNT")
            .Add("PRICE_1", "PRICE_1")
            .Add("PRICE_2", "PRICE_2")
            .Add("PRICE_3", "PRICE_3")
            .Add("PRICE_4", "PRICE_4")
            .Add("PRICE_5", "PRICE_5")
            .Add("TAX_CODE", "TAX_CODE")
            .Add("TARIFF_CODE", "TARIFF_CODE")
            ' bf - reinstate price break code
            .Add("PRICE_BREAK_CODE", "PRICE_BREAK_CODE")
            .Add("PRICE_BREAK_QUANTITY_1", "PRICE_BREAK_QUANTITY_1")
            .Add("SALE_PRICE_BREAK_QUANTITY_1", "SALE_PRICE_BREAK_QUANTITY_1")
            .Add("NET_PRICE_2", "NET_PRICE_2")
            .Add("GROSS_PRICE_2", "GROSS_PRICE_2")
            .Add("TAX_AMOUNT_2", "TAX_AMOUNT_2")
            .Add("PRICE_BREAK_QUANTITY_2", "PRICE_BREAK_QUANTITY_2")
            .Add("SALE_NET_PRICE_2", "SALE_NET_PRICE_2")
            .Add("SALE_GROSS_PRICE_2", "SALE_GROSS_PRICE_2")
            .Add("SALE_TAX_AMOUNT_2", "SALE_TAX_AMOUNT_2")
            .Add("SALE_PRICE_BREAK_QUANTITY_2", "SALE_PRICE_BREAK_QUANTITY_2")
            .Add("NET_PRICE_3", "NET_PRICE_3")
            .Add("GROSS_PRICE_3", "GROSS_PRICE_3")
            .Add("TAX_AMOUNT_3", "TAX_AMOUNT_3")
            .Add("PRICE_BREAK_QUANTITY_3", "PRICE_BREAK_QUANTITY_3")
            .Add("SALE_NET_PRICE_3", "SALE_NET_PRICE_3")
            .Add("SALE_GROSS_PRICE_3", "SALE_GROSS_PRICE_3")
            .Add("SALE_TAX_AMOUNT_3", "SALE_TAX_AMOUNT_3")
            .Add("SALE_PRICE_BREAK_QUANTITY_3", "SALE_PRICE_BREAK_QUANTITY_3")
            .Add("NET_PRICE_4", "NET_PRICE_4")
            .Add("GROSS_PRICE_4", "GROSS_PRICE_4")
            .Add("TAX_AMOUNT_4", "TAX_AMOUNT_4")
            .Add("PRICE_BREAK_QUANTITY_4", "PRICE_BREAK_QUANTITY_4")
            .Add("SALE_NET_PRICE_4", "SALE_NET_PRICE_4")
            .Add("SALE_GROSS_PRICE_4", "SALE_GROSS_PRICE_4")
            .Add("SALE_TAX_AMOUNT_4", "SALE_TAX_AMOUNT_4")
            .Add("SALE_PRICE_BREAK_QUANTITY_4", "SALE_PRICE_BREAK_QUANTITY_4")
            .Add("NET_PRICE_5", "NET_PRICE_5")
            .Add("GROSS_PRICE_5", "GROSS_PRICE_5")
            .Add("TAX_AMOUNT_5", "TAX_AMOUNT_5")
            .Add("PRICE_BREAK_QUANTITY_5", "PRICE_BREAK_QUANTITY_5")
            .Add("SALE_NET_PRICE_5", "SALE_NET_PRICE_5")
            .Add("SALE_GROSS_PRICE_5", "SALE_GROSS_PRICE_5")
            .Add("SALE_TAX_AMOUNT_5", "SALE_TAX_AMOUNT_5")
            .Add("SALE_PRICE_BREAK_QUANTITY_5", "SALE_PRICE_BREAK_QUANTITY_5")
            .Add("NET_PRICE_6", "NET_PRICE_6")
            .Add("GROSS_PRICE_6", "GROSS_PRICE_6")
            .Add("TAX_AMOUNT_6", "TAX_AMOUNT_6")
            .Add("PRICE_BREAK_QUANTITY_6", "PRICE_BREAK_QUANTITY_6")
            .Add("SALE_NET_PRICE_6", "SALE_NET_PRICE_6")
            .Add("SALE_GROSS_PRICE_6", "SALE_GROSS_PRICE_6")
            .Add("SALE_TAX_AMOUNT_6", "SALE_TAX_AMOUNT_6")
            .Add("SALE_PRICE_BREAK_QUANTITY_6", "SALE_PRICE_BREAK_QUANTITY_6")
            .Add("NET_PRICE_7", "NET_PRICE_7")
            .Add("GROSS_PRICE_7", "GROSS_PRICE_7")
            .Add("TAX_AMOUNT_7", "TAX_AMOUNT_7")
            .Add("PRICE_BREAK_QUANTITY_7", "PRICE_BREAK_QUANTITY_7")
            .Add("SALE_NET_PRICE_7", "SALE_NET_PRICE_7")
            .Add("SALE_GROSS_PRICE_7", "SALE_GROSS_PRICE_7")
            .Add("SALE_TAX_AMOUNT_7", "SALE_TAX_AMOUNT_7")
            .Add("SALE_PRICE_BREAK_QUANTITY_7", "SALE_PRICE_BREAK_QUANTITY_7")
            .Add("NET_PRICE_8", "NET_PRICE_8")
            .Add("GROSS_PRICE_8", "GROSS_PRICE_8")
            .Add("TAX_AMOUNT_8", "TAX_AMOUNT_8")
            .Add("PRICE_BREAK_QUANTITY_8", "PRICE_BREAK_QUANTITY_8")
            .Add("SALE_NET_PRICE_8", "SALE_NET_PRICE_8")
            .Add("SALE_GROSS_PRICE_8", "SALE_GROSS_PRICE_8")
            .Add("SALE_TAX_AMOUNT_8", "SALE_TAX_AMOUNT_8")
            .Add("SALE_PRICE_BREAK_QUANTITY_8", "SALE_PRICE_BREAK_QUANTITY_8")
            .Add("NET_PRICE_9", "NET_PRICE_9")
            .Add("GROSS_PRICE_9", "GROSS_PRICE_9")
            .Add("TAX_AMOUNT_9", "TAX_AMOUNT_9")
            .Add("PRICE_BREAK_QUANTITY_9", "PRICE_BREAK_QUANTITY_9")
            .Add("SALE_NET_PRICE_9", "SALE_NET_PRICE_9")
            .Add("SALE_GROSS_PRICE_9", "SALE_GROSS_PRICE_9")
            .Add("SALE_TAX_AMOUNT_9", "SALE_TAX_AMOUNT_9")
            .Add("SALE_PRICE_BREAK_QUANTITY_9", "SALE_PRICE_BREAK_QUANTITY_9")
            .Add("NET_PRICE_10", "NET_PRICE_10")
            .Add("GROSS_PRICE_10", "GROSS_PRICE_10")
            .Add("TAX_AMOUNT_10", "TAX_AMOUNT_10")
            .Add("PRICE_BREAK_QUANTITY_10", "PRICE_BREAK_QUANTITY_10")
            .Add("SALE_NET_PRICE_10", "SALE_NET_PRICE_10")
            .Add("SALE_GROSS_PRICE_10", "SALE_GROSS_PRICE_10")
            .Add("SALE_TAX_AMOUNT_10", "SALE_TAX_AMOUNT_10")
            .Add("SALE_PRICE_BREAK_QUANTITY_10", "SALE_PRICE_BREAK_QUANTITY_10")
            .Add("DELIVERY_NET_PRICE_2", "DELIVERY_NET_PRICE_2")
            .Add("DELIVERY_GROSS_PRICE_2", "DELIVERY_GROSS_PRICE_2")
            .Add("DELIVERY_TAX_AMOUNT_2", "DELIVERY_TAX_AMOUNT_2")
            .Add("DELIVERY_SALE_NET_PRICE_2", "DELIVERY_SALE_NET_PRICE_2")
            .Add("DELIVERY_SALE_GROSS_PRICE_2", "DELIVERY_SALE_GROSS_PRICE_2")
            .Add("DELIVERY_SALE_TAX_AMOUNT_2", "DELIVERY_SALE_TAX_AMOUNT_2")
            .Add("DELIVERY_NET_PRICE_3", "DELIVERY_NET_PRICE_3")
            .Add("DELIVERY_GROSS_PRICE_3", "DELIVERY_GROSS_PRICE_3")
            .Add("DELIVERY_TAX_AMOUNT_3", "DELIVERY_TAX_AMOUNT_3")
            .Add("DELIVERY_SALE_NET_PRICE_3", "DELIVERY_SALE_NET_PRICE_3")
            .Add("DELIVERY_SALE_GROSS_PRICE_3", "DELIVERY_SALE_GROSS_PRICE_3")
            .Add("DELIVERY_SALE_TAX_AMOUNT_3", "DELIVERY_SALE_TAX_AMOUNT_3")
            .Add("DELIVERY_NET_PRICE_4", "DELIVERY_NET_PRICE_4")
            .Add("DELIVERY_GROSS_PRICE_4", "DELIVERY_GROSS_PRICE_4")
            .Add("DELIVERY_TAX_AMOUNT_4", "DELIVERY_TAX_AMOUNT_4")
            .Add("DELIVERY_SALE_NET_PRICE_4", "DELIVERY_SALE_NET_PRICE_4")
            .Add("DELIVERY_SALE_GROSS_PRICE_4", "DELIVERY_SALE_GROSS_PRICE_4")
            .Add("DELIVERY_SALE_TAX_AMOUNT_4", "DELIVERY_SALE_TAX_AMOUNT_4")
            .Add("DELIVERY_NET_PRICE_5", "DELIVERY_NET_PRICE_5")
            .Add("DELIVERY_GROSS_PRICE_5", "DELIVERY_GROSS_PRICE_5")
            .Add("DELIVERY_TAX_AMOUNT_5", "DELIVERY_TAX_AMOUNT_5")
            .Add("DELIVERY_SALE_NET_PRICE_5", "DELIVERY_SALE_NET_PRICE_5")
            .Add("DELIVERY_SALE_GROSS_PRICE_5", "DELIVERY_SALE_GROSS_PRICE_5")
            .Add("DELIVERY_SALE_TAX_AMOUNT_5", "DELIVERY_SALE_TAX_AMOUNT_5")
            .Add("DELIVERY_NET_PRICE_6", "DELIVERY_NET_PRICE_6")
            .Add("DELIVERY_GROSS_PRICE_6", "DELIVERY_GROSS_PRICE_6")
            .Add("DELIVERY_TAX_AMOUNT_6", "DELIVERY_TAX_AMOUNT_6")
            .Add("DELIVERY_SALE_NET_PRICE_6", "DELIVERY_SALE_NET_PRICE_6")
            .Add("DELIVERY_SALE_GROSS_PRICE_6", "DELIVERY_SALE_GROSS_PRICE_6")
            .Add("DELIVERY_SALE_TAX_AMOUNT_6", "DELIVERY_SALE_TAX_AMOUNT_6")
            .Add("DELIVERY_NET_PRICE_7", "DELIVERY_NET_PRICE_7")
            .Add("DELIVERY_GROSS_PRICE_7", "DELIVERY_GROSS_PRICE_7")
            .Add("DELIVERY_TAX_AMOUNT_7", "DELIVERY_TAX_AMOUNT_7")
            .Add("DELIVERY_SALE_NET_PRICE_7", "DELIVERY_SALE_NET_PRICE_7")
            .Add("DELIVERY_SALE_GROSS_PRICE_7", "DELIVERY_SALE_GROSS_PRICE_7")
            .Add("DELIVERY_SALE_TAX_AMOUNT_7", "DELIVERY_SALE_TAX_AMOUNT_7")
            .Add("DELIVERY_NET_PRICE_8", "DELIVERY_NET_PRICE_8")
            .Add("DELIVERY_GROSS_PRICE_8", "DELIVERY_GROSS_PRICE_8")
            .Add("DELIVERY_TAX_AMOUNT_8", "DELIVERY_TAX_AMOUNT_8")
            .Add("DELIVERY_SALE_NET_PRICE_8", "DELIVERY_SALE_NET_PRICE_8")
            .Add("DELIVERY_SALE_GROSS_PRICE_8", "DELIVERY_SALE_GROSS_PRICE_8")
            .Add("DELIVERY_SALE_TAX_AMOUNT_8", "DELIVERY_SALE_TAX_AMOUNT_8")
            .Add("DELIVERY_NET_PRICE_9", "DELIVERY_NET_PRICE_9")
            .Add("DELIVERY_GROSS_PRICE_9", "DELIVERY_GROSS_PRICE_9")
            .Add("DELIVERY_TAX_AMOUNT_9", "DELIVERY_TAX_AMOUNT_9")
            .Add("DELIVERY_SALE_NET_PRICE_9", "DELIVERY_SALE_NET_PRICE_9")
            .Add("DELIVERY_SALE_GROSS_PRICE_9", "DELIVERY_SALE_GROSS_PRICE_9")
            .Add("DELIVERY_SALE_TAX_AMOUNT_9", "DELIVERY_SALE_TAX_AMOUNT_9")
            .Add("DELIVERY_NET_PRICE_10", "DELIVERY_NET_PRICE_10")
            .Add("DELIVERY_GROSS_PRICE_10", "DELIVERY_GROSS_PRICE_10")
            .Add("DELIVERY_TAX_AMOUNT_10", "DELIVERY_TAX_AMOUNT_10")
            .Add("DELIVERY_SALE_NET_PRICE_10", "DELIVERY_SALE_NET_PRICE_10")
            .Add("DELIVERY_SALE_GROSS_PRICE_10", "DELIVERY_SALE_GROSS_PRICE_10")
            .Add("DELIVERY_SALE_TAX_AMOUNT_10", "DELIVERY_SALE_TAX_AMOUNT_10")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PRICE_LIST", "PRICE_LIST")
            .Add("PRODUCT", "PRODUCT")
        End With

        With selectString
            .Append("SELECT * FROM ")
            .Append(sourceTable)
            .Append(" ORDER BY AMENDED_DATE, AMENDED_HOUR, AMENDED_MINUTE, AMENDED_SECOND, AMENDED_MILLI")
        End With

        'Read through the work table, add/amend or delete line by line
        Dim con As SqlConnection = Nothing
        con = New SqlConnection(SQLConnectionString())
        con.Open()

        Try

            Dim cmd As SqlCommand = Nothing
            cmd = New SqlCommand(selectString.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            Dim dt As New DataTable

            dt.Load(cmd.ExecuteReader())
            con.Close()

            For Each r As DataRow In dt.Rows

                Dim type As String = Talent.Common.Utilities.CheckForDBNull_String(r("TYPE"))
                Dim connectionString As String = SQLConnectionString()

                Select Case type
                    Case Is = "AMEND"

                        update = BuildPartialUpdateSQL(destinationTable, fieldMappings, keyFields, r)
                        insert = BuildPartialInsertSQL(destinationTable, fieldMappings, keyFields, r)

                        Using connection As New SqlConnection(connectionString)
                            Dim command As New SqlCommand(update.ToString, connection)
                            command.CommandTimeout = DefaultSQLTimeout()
                            connection.Open()
                            command.ExecuteNonQuery()
                            connection.Close()
                        End Using

                        Using connection As New SqlConnection(connectionString)
                            Dim command As New SqlCommand(insert.ToString, connection)
                            command.CommandTimeout = DefaultSQLTimeout()
                            connection.Open()
                            command.ExecuteNonQuery()
                            connection.Close()
                        End Using

                    Case Is = "DELETE"

                        delete = BuildPartialDeleteSQL(destinationTable, keyFields, r)

                        Using connection As New SqlConnection(connectionString)
                            Dim command As New SqlCommand(delete.ToString, connection)
                            command.CommandTimeout = DefaultSQLTimeout()
                            connection.Open()
                            command.ExecuteNonQuery()
                            connection.Close()
                        End Using

                End Select

            Next

        Catch
        End Try


    End Sub

    Private Shared Sub DoUpdateTable_EBPROD(ByVal updateProductDescriptions As Boolean)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_work"
        Const destinationTable As String = "tbl_product"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        Dim fieldMappingsUPDATE As New Generic.Dictionary(Of String, String)

        With fieldMappings
            '.Add("PRODUCT_ID", "PRODUCT_ID")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("PRODUCT_DESCRIPTION_1", "PRODUCT_DESCRIPTION_1")
            .Add("PRODUCT_DESCRIPTION_2", "PRODUCT_DESCRIPTION_2")
            .Add("PRODUCT_DESCRIPTION_3", "PRODUCT_DESCRIPTION_3")
            .Add("PRODUCT_DESCRIPTION_4", "PRODUCT_DESCRIPTION_4")
            .Add("PRODUCT_DESCRIPTION_5", "PRODUCT_DESCRIPTION_5")
            .Add("PRODUCT_LENGTH", "PRODUCT_LENGTH")
            .Add("PRODUCT_LENGTH_UOM", "PRODUCT_LENGTH_UOM")
            .Add("PRODUCT_WIDTH", "PRODUCT_WIDTH")
            .Add("PRODUCT_WIDTH_UOM", "PRODUCT_WIDTH_UOM")
            .Add("PRODUCT_DEPTH", "PRODUCT_DEPTH")
            .Add("PRODUCT_DEPTH_UOM", "PRODUCT_DEPTH_UOM")
            .Add("PRODUCT_HEIGHT", "PRODUCT_HEIGHT")
            .Add("PRODUCT_HEIGHT_UOM", "PRODUCT_HEIGHT_UOM")
            .Add("PRODUCT_SIZE", "PRODUCT_SIZE")
            .Add("PRODUCT_SIZE_UOM", "PRODUCT_SIZE_UOM")
            .Add("PRODUCT_WEIGHT", "PRODUCT_WEIGHT")
            .Add("PRODUCT_WEIGHT_UOM", "PRODUCT_WEIGHT_UOM")
            .Add("PRODUCT_VOLUME", "PRODUCT_VOLUME")
            .Add("PRODUCT_VOLUME_UOM", "PRODUCT_VOLUME_UOM")
            .Add("PRODUCT_COLOUR", "PRODUCT_COLOUR")
            .Add("PRODUCT_PACK_SIZE", "PRODUCT_PACK_SIZE")
            .Add("PRODUCT_PACK_SIZE_UOM", "PRODUCT_PACK_SIZE_UOM")
            .Add("PRODUCT_SUPPLIER_PART_NO", "PRODUCT_SUPPLIER_PART_NO")
            .Add("PRODUCT_CUSTOMER_PART_NO", "PRODUCT_CUSTOMER_PART_NO")
            .Add("PRODUCT_TASTING_NOTES_1", "PRODUCT_TASTING_NOTES_1")
            .Add("PRODUCT_TASTING_NOTES_2", "PRODUCT_TASTING_NOTES_2")
            .Add("PRODUCT_ABV", "PRODUCT_ABV")
            .Add("PRODUCT_VINTAGE", "PRODUCT_VINTAGE")
            .Add("PRODUCT_SUPPLIER", "PRODUCT_SUPPLIER")
            .Add("PRODUCT_COUNTRY", "PRODUCT_COUNTRY")
            .Add("PRODUCT_REGION", "PRODUCT_REGION")
            .Add("PRODUCT_AREA", "PRODUCT_AREA")
            .Add("PRODUCT_GRAPE", "PRODUCT_GRAPE")
            .Add("PRODUCT_CLOSURE", "PRODUCT_CLOSURE")
            .Add("PRODUCT_CATALOG_CODE", "PRODUCT_CATALOG_CODE")
            .Add("PRODUCT_VEGETARIAN", "PRODUCT_VEGETARIAN")
            .Add("PRODUCT_VEGAN", "PRODUCT_VEGAN")
            .Add("PRODUCT_ORGANIC", "PRODUCT_ORGANIC")
            .Add("PRODUCT_BIODYNAMIC", "PRODUCT_BIODYNAMIC")
            .Add("PRODUCT_LUTTE", "PRODUCT_LUTTE")
            .Add("PRODUCT_MINIMUM_AGE", "PRODUCT_MINIMUM_AGE")
            .Add("PRODUCT_HTML_1", "PRODUCT_HTML_1")
            .Add("PRODUCT_HTML_2", "PRODUCT_HTML_2")
            .Add("PRODUCT_HTML_3", "PRODUCT_HTML_3")

            .Add("PRODUCT_SEARCH_KEYWORDS", "PRODUCT_SEARCH_KEYWORDS")
            .Add("PRODUCT_PAGE_TITLE", "PRODUCT_PAGE_TITLE")
            .Add("PRODUCT_META_DESCRIPTION", "PRODUCT_META_DESCRIPTION")
            .Add("PRODUCT_META_KEYWORDS", "PRODUCT_META_KEYWORDS")
            .Add("PRODUCT_SEARCH_RANGE_01", "PRODUCT_SEARCH_RANGE_01")
            .Add("PRODUCT_SEARCH_RANGE_02", "PRODUCT_SEARCH_RANGE_02")
            .Add("PRODUCT_SEARCH_RANGE_03", "PRODUCT_SEARCH_RANGE_03")
            .Add("PRODUCT_SEARCH_RANGE_04", "PRODUCT_SEARCH_RANGE_04")
            .Add("PRODUCT_SEARCH_RANGE_05", "PRODUCT_SEARCH_RANGE_05")
            .Add("PRODUCT_SEARCH_CRITERIA_01", "PRODUCT_SEARCH_CRITERIA_01")
            .Add("PRODUCT_SEARCH_CRITERIA_02", "PRODUCT_SEARCH_CRITERIA_02")
            .Add("PRODUCT_SEARCH_CRITERIA_03", "PRODUCT_SEARCH_CRITERIA_03")
            .Add("PRODUCT_SEARCH_CRITERIA_04", "PRODUCT_SEARCH_CRITERIA_04")
            .Add("PRODUCT_SEARCH_CRITERIA_05", "PRODUCT_SEARCH_CRITERIA_05")
            .Add("PRODUCT_SEARCH_CRITERIA_06", "PRODUCT_SEARCH_CRITERIA_06")
            .Add("PRODUCT_SEARCH_CRITERIA_07", "PRODUCT_SEARCH_CRITERIA_07")
            .Add("PRODUCT_SEARCH_CRITERIA_08", "PRODUCT_SEARCH_CRITERIA_08")
            .Add("PRODUCT_SEARCH_CRITERIA_09", "PRODUCT_SEARCH_CRITERIA_09")
            .Add("PRODUCT_SEARCH_CRITERIA_10", "PRODUCT_SEARCH_CRITERIA_10")
            .Add("PRODUCT_SEARCH_CRITERIA_11", "PRODUCT_SEARCH_CRITERIA_11")
            .Add("PRODUCT_SEARCH_CRITERIA_12", "PRODUCT_SEARCH_CRITERIA_12")
            .Add("PRODUCT_SEARCH_CRITERIA_13", "PRODUCT_SEARCH_CRITERIA_13")
            .Add("PRODUCT_SEARCH_CRITERIA_14", "PRODUCT_SEARCH_CRITERIA_14")
            .Add("PRODUCT_SEARCH_CRITERIA_15", "PRODUCT_SEARCH_CRITERIA_15")
            .Add("PRODUCT_SEARCH_CRITERIA_16", "PRODUCT_SEARCH_CRITERIA_16")
            .Add("PRODUCT_SEARCH_CRITERIA_17", "PRODUCT_SEARCH_CRITERIA_17")
            .Add("PRODUCT_SEARCH_CRITERIA_18", "PRODUCT_SEARCH_CRITERIA_18")
            .Add("PRODUCT_SEARCH_CRITERIA_19", "PRODUCT_SEARCH_CRITERIA_19")
            .Add("PRODUCT_SEARCH_CRITERIA_20", "PRODUCT_SEARCH_CRITERIA_20")
            .Add("PRODUCT_SEARCH_SWITCH_01", "PRODUCT_SEARCH_SWITCH_01")
            .Add("PRODUCT_SEARCH_SWITCH_02", "PRODUCT_SEARCH_SWITCH_02")
            .Add("PRODUCT_SEARCH_SWITCH_03", "PRODUCT_SEARCH_SWITCH_03")
            .Add("PRODUCT_SEARCH_SWITCH_04", "PRODUCT_SEARCH_SWITCH_04")
            .Add("PRODUCT_SEARCH_SWITCH_05", "PRODUCT_SEARCH_SWITCH_05")
            .Add("PRODUCT_SEARCH_SWITCH_06", "PRODUCT_SEARCH_SWITCH_06")
            .Add("PRODUCT_SEARCH_SWITCH_07", "PRODUCT_SEARCH_SWITCH_07")
            .Add("PRODUCT_SEARCH_SWITCH_08", "PRODUCT_SEARCH_SWITCH_08")
            .Add("PRODUCT_SEARCH_SWITCH_09", "PRODUCT_SEARCH_SWITCH_09")
            .Add("PRODUCT_SEARCH_SWITCH_10", "PRODUCT_SEARCH_SWITCH_10")
            .Add("PRODUCT_SEARCH_DATE_01", "PRODUCT_SEARCH_DATE_01")
            .Add("PRODUCT_SEARCH_DATE_02", "PRODUCT_SEARCH_DATE_02")
            .Add("PRODUCT_SEARCH_DATE_03", "PRODUCT_SEARCH_DATE_03")
            .Add("PRODUCT_SEARCH_DATE_04", "PRODUCT_SEARCH_DATE_04")
            .Add("PRODUCT_SEARCH_DATE_05", "PRODUCT_SEARCH_DATE_05")
            .Add("PRODUCT_TARIFF_CODE", "PRODUCT_TARIFF_CODE")
            .Add("PRODUCT_OPTION_MASTER", "PRODUCT_OPTION_MASTER")
            .Add("ALTERNATE_SKU", "ALTERNATE_SKU")
            .Add("AVAILABLE_ONLINE", "AVAILABLE_ONLINE")
            .Add("PERSONALISABLE", "PERSONALISABLE")
            .Add("DISCONTINUED", "DISCONTINUED")


        End With

        With fieldMappingsUPDATE
            '.Add("PRODUCT_ID", "PRODUCT_ID")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            If updateProductDescriptions Then
                .Add("PRODUCT_DESCRIPTION_1", "PRODUCT_DESCRIPTION_1")
                .Add("PRODUCT_DESCRIPTION_2", "PRODUCT_DESCRIPTION_2")
                .Add("PRODUCT_DESCRIPTION_3", "PRODUCT_DESCRIPTION_3")
                .Add("PRODUCT_DESCRIPTION_4", "PRODUCT_DESCRIPTION_4")
                .Add("PRODUCT_DESCRIPTION_5", "PRODUCT_DESCRIPTION_5")
            End If
            .Add("PRODUCT_LENGTH", "PRODUCT_LENGTH")
            .Add("PRODUCT_LENGTH_UOM", "PRODUCT_LENGTH_UOM")
            .Add("PRODUCT_WIDTH", "PRODUCT_WIDTH")
            .Add("PRODUCT_WIDTH_UOM", "PRODUCT_WIDTH_UOM")
            .Add("PRODUCT_DEPTH", "PRODUCT_DEPTH")
            .Add("PRODUCT_DEPTH_UOM", "PRODUCT_DEPTH_UOM")
            .Add("PRODUCT_HEIGHT", "PRODUCT_HEIGHT")
            .Add("PRODUCT_HEIGHT_UOM", "PRODUCT_HEIGHT_UOM")
            .Add("PRODUCT_SIZE", "PRODUCT_SIZE")
            .Add("PRODUCT_SIZE_UOM", "PRODUCT_SIZE_UOM")
            .Add("PRODUCT_WEIGHT", "PRODUCT_WEIGHT")
            .Add("PRODUCT_WEIGHT_UOM", "PRODUCT_WEIGHT_UOM")
            .Add("PRODUCT_VOLUME", "PRODUCT_VOLUME")
            .Add("PRODUCT_VOLUME_UOM", "PRODUCT_VOLUME_UOM")
            .Add("PRODUCT_COLOUR", "PRODUCT_COLOUR")
            .Add("PRODUCT_PACK_SIZE", "PRODUCT_PACK_SIZE")
            .Add("PRODUCT_PACK_SIZE_UOM", "PRODUCT_PACK_SIZE_UOM")
            .Add("PRODUCT_SUPPLIER_PART_NO", "PRODUCT_SUPPLIER_PART_NO")
            .Add("PRODUCT_CUSTOMER_PART_NO", "PRODUCT_CUSTOMER_PART_NO")
            .Add("PRODUCT_TASTING_NOTES_1", "PRODUCT_TASTING_NOTES_1")
            .Add("PRODUCT_TASTING_NOTES_2", "PRODUCT_TASTING_NOTES_2")
            .Add("PRODUCT_ABV", "PRODUCT_ABV")
            .Add("PRODUCT_VINTAGE", "PRODUCT_VINTAGE")
            .Add("PRODUCT_SUPPLIER", "PRODUCT_SUPPLIER")
            .Add("PRODUCT_COUNTRY", "PRODUCT_COUNTRY")
            .Add("PRODUCT_REGION", "PRODUCT_REGION")
            .Add("PRODUCT_AREA", "PRODUCT_AREA")
            .Add("PRODUCT_GRAPE", "PRODUCT_GRAPE")
            .Add("PRODUCT_CLOSURE", "PRODUCT_CLOSURE")
            .Add("PRODUCT_CATALOG_CODE", "PRODUCT_CATALOG_CODE")
            .Add("PRODUCT_VEGETARIAN", "PRODUCT_VEGETARIAN")
            .Add("PRODUCT_VEGAN", "PRODUCT_VEGAN")
            .Add("PRODUCT_ORGANIC", "PRODUCT_ORGANIC")
            .Add("PRODUCT_BIODYNAMIC", "PRODUCT_BIODYNAMIC")
            .Add("PRODUCT_LUTTE", "PRODUCT_LUTTE")
            .Add("PRODUCT_MINIMUM_AGE", "PRODUCT_MINIMUM_AGE")
            If updateProductDescriptions Then
                .Add("PRODUCT_HTML_1", "PRODUCT_HTML_1")
                .Add("PRODUCT_HTML_2", "PRODUCT_HTML_2")
                .Add("PRODUCT_HTML_3", "PRODUCT_HTML_3")
                .Add("PRODUCT_HTML_4", "PRODUCT_HTML_4")
                .Add("PRODUCT_HTML_5", "PRODUCT_HTML_5")
                .Add("PRODUCT_HTML_6", "PRODUCT_HTML_6")
                .Add("PRODUCT_HTML_7", "PRODUCT_HTML_7")
                .Add("PRODUCT_HTML_8", "PRODUCT_HTML_8")
                .Add("PRODUCT_HTML_9", "PRODUCT_HTML_9")
            End If

            .Add("PRODUCT_SEARCH_KEYWORDS", "PRODUCT_SEARCH_KEYWORDS")
            .Add("PRODUCT_PAGE_TITLE", "PRODUCT_PAGE_TITLE")
            .Add("PRODUCT_META_DESCRIPTION", "PRODUCT_META_DESCRIPTION")
            .Add("PRODUCT_META_KEYWORDS", "PRODUCT_META_KEYWORDS")
            .Add("PRODUCT_SEARCH_RANGE_01", "PRODUCT_SEARCH_RANGE_01")
            .Add("PRODUCT_SEARCH_RANGE_02", "PRODUCT_SEARCH_RANGE_02")
            .Add("PRODUCT_SEARCH_RANGE_03", "PRODUCT_SEARCH_RANGE_03")
            .Add("PRODUCT_SEARCH_RANGE_04", "PRODUCT_SEARCH_RANGE_04")
            .Add("PRODUCT_SEARCH_RANGE_05", "PRODUCT_SEARCH_RANGE_05")
            .Add("PRODUCT_SEARCH_CRITERIA_01", "PRODUCT_SEARCH_CRITERIA_01")
            .Add("PRODUCT_SEARCH_CRITERIA_02", "PRODUCT_SEARCH_CRITERIA_02")
            .Add("PRODUCT_SEARCH_CRITERIA_03", "PRODUCT_SEARCH_CRITERIA_03")
            .Add("PRODUCT_SEARCH_CRITERIA_04", "PRODUCT_SEARCH_CRITERIA_04")
            .Add("PRODUCT_SEARCH_CRITERIA_05", "PRODUCT_SEARCH_CRITERIA_05")
            .Add("PRODUCT_SEARCH_CRITERIA_06", "PRODUCT_SEARCH_CRITERIA_06")
            .Add("PRODUCT_SEARCH_CRITERIA_07", "PRODUCT_SEARCH_CRITERIA_07")
            .Add("PRODUCT_SEARCH_CRITERIA_08", "PRODUCT_SEARCH_CRITERIA_08")
            .Add("PRODUCT_SEARCH_CRITERIA_09", "PRODUCT_SEARCH_CRITERIA_09")
            .Add("PRODUCT_SEARCH_CRITERIA_10", "PRODUCT_SEARCH_CRITERIA_10")
            .Add("PRODUCT_SEARCH_CRITERIA_11", "PRODUCT_SEARCH_CRITERIA_11")
            .Add("PRODUCT_SEARCH_CRITERIA_12", "PRODUCT_SEARCH_CRITERIA_12")
            .Add("PRODUCT_SEARCH_CRITERIA_13", "PRODUCT_SEARCH_CRITERIA_13")
            .Add("PRODUCT_SEARCH_CRITERIA_14", "PRODUCT_SEARCH_CRITERIA_14")
            .Add("PRODUCT_SEARCH_CRITERIA_15", "PRODUCT_SEARCH_CRITERIA_15")
            .Add("PRODUCT_SEARCH_CRITERIA_16", "PRODUCT_SEARCH_CRITERIA_16")
            .Add("PRODUCT_SEARCH_CRITERIA_17", "PRODUCT_SEARCH_CRITERIA_17")
            .Add("PRODUCT_SEARCH_CRITERIA_18", "PRODUCT_SEARCH_CRITERIA_18")
            .Add("PRODUCT_SEARCH_CRITERIA_19", "PRODUCT_SEARCH_CRITERIA_19")
            .Add("PRODUCT_SEARCH_CRITERIA_20", "PRODUCT_SEARCH_CRITERIA_20")
            .Add("PRODUCT_SEARCH_SWITCH_01", "PRODUCT_SEARCH_SWITCH_01")
            .Add("PRODUCT_SEARCH_SWITCH_02", "PRODUCT_SEARCH_SWITCH_02")
            .Add("PRODUCT_SEARCH_SWITCH_03", "PRODUCT_SEARCH_SWITCH_03")
            .Add("PRODUCT_SEARCH_SWITCH_04", "PRODUCT_SEARCH_SWITCH_04")
            .Add("PRODUCT_SEARCH_SWITCH_05", "PRODUCT_SEARCH_SWITCH_05")
            .Add("PRODUCT_SEARCH_SWITCH_06", "PRODUCT_SEARCH_SWITCH_06")
            .Add("PRODUCT_SEARCH_SWITCH_07", "PRODUCT_SEARCH_SWITCH_07")
            .Add("PRODUCT_SEARCH_SWITCH_08", "PRODUCT_SEARCH_SWITCH_08")
            .Add("PRODUCT_SEARCH_SWITCH_09", "PRODUCT_SEARCH_SWITCH_09")
            .Add("PRODUCT_SEARCH_SWITCH_10", "PRODUCT_SEARCH_SWITCH_10")
            .Add("PRODUCT_SEARCH_DATE_01", "PRODUCT_SEARCH_DATE_01")
            .Add("PRODUCT_SEARCH_DATE_02", "PRODUCT_SEARCH_DATE_02")
            .Add("PRODUCT_SEARCH_DATE_03", "PRODUCT_SEARCH_DATE_03")
            .Add("PRODUCT_SEARCH_DATE_04", "PRODUCT_SEARCH_DATE_04")
            .Add("PRODUCT_SEARCH_DATE_05", "PRODUCT_SEARCH_DATE_05")
            .Add("PRODUCT_TARIFF_CODE", "PRODUCT_TARIFF_CODE")
            .Add("PRODUCT_OPTION_MASTER", "PRODUCT_OPTION_MASTER")
            .Add("ALTERNATE_SKU", "ALTERNATE_SKU")
            .Add("AVAILABLE_ONLINE", "AVAILABLE_ONLINE")
            If updateProductDescriptions Then
                .Add("PERSONALISABLE", "PERSONALISABLE")
            End If

            .Add("DISCONTINUED", "DISCONTINUED")

        End With


        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappingsUPDATE, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPRDO()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_options_work"
        Const destinationTable As String = "tbl_product_options"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("MASTER_PRODUCT", "MASTER_PRODUCT")
            .Add("OPTION_TYPE", "OPTION_TYPE")
            .Add("OPTION_CODE", "OPTION_CODE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("DISPLAY_ORDER", "DISPLAY_ORDER")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("MASTER_PRODUCT", "MASTER_PRODUCT")
            .Add("OPTION_TYPE", "OPTION_TYPE")
            .Add("OPTION_CODE", "OPTION_CODE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("DISPLAY_ORDER", "DISPLAY_ORDER")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPRDO_definitions()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_option_definitions_work"
        Const destinationTable As String = "tbl_product_option_definitions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("OPTION_CODE", "OPTION_CODE")
            .Add("DESCRIPTION", "DESCRIPTION")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("OPTION_CODE", "OPTION_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPRDO_definitions_lang()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_option_definitions_lang_work"
        Const destinationTable As String = "tbl_product_option_definitions_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("OPTION_CODE", "OPTION_CODE")
            .Add("LANGUAGE_CODE", "LANGUAGE_CODE")
            .Add("DISPLAY_NAME", "DISPLAY_NAME")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("OPTION_CODE", "OPTION_CODE")
            .Add("LANGUAGE_CODE", "LANGUAGE_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPRDO_types()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_option_types_work"
        Const destinationTable As String = "tbl_product_option_types"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("OPTION_TYPE", "OPTION_TYPE")
            .Add("DESCRIPTION", "DESCRIPTION")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("OPTION_TYPE", "OPTION_TYPE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPRDO_types_lang()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_option_types_lang_work"
        Const destinationTable As String = "tbl_product_option_types_lang"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("OPTION_TYPE", "OPTION_TYPE")
            .Add("LANGUAGE_CODE", "LANGUAGE_CODE")
            .Add("DISPLAY_NAME", "DISPLAY_NAME")
            .Add("LABEL_TEXT", "LABEL_TEXT")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("OPTION_TYPE", "OPTION_TYPE")
            .Add("LANGUAGE_CODE", "LANGUAGE_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPRDD(Optional ByVal doDelete As Boolean = False)
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_option_defaults_work"
        Const destinationTable As String = "tbl_product_option_defaults"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("MASTER_PRODUCT", "MASTER_PRODUCT")
            .Add("OPTION_TYPE", "OPTION_TYPE")
            .Add("MATCH_ACTION", "MATCH_ACTION")
            .Add("IS_DEFAULT", "IS_DEFAULT")
            .Add("APPEND_SEQUENCE", "APPEND_SEQUENCE")
            .Add("DISPLAY_SEQUENCE", "DISPLAY_SEQUENCE")
            .Add("DISPLAY_TYPE", "DISPLAY_TYPE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("MASTER_PRODUCT", "MASTER_PRODUCT")
            .Add("OPTION_TYPE", "OPTION_TYPE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)

        If doDelete Then
            delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
            DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
        Else
            DoUpdateTableNoDelete(update.ToString, insert.ToString)
        End If

    End Sub

    Public Shared Sub DoUpdateTable_EBPM()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_promotions_work"
        Const destinationTable As String = "tbl_promotions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("PROMOTION_TYPE", "PROMOTION_TYPE")
            .Add("ACTIVATION_MECHANISM", "ACTIVATION_MECHANISM")
            .Add("START_DATE", "START_DATE")
            .Add("END_DATE", "END_DATE")
            .Add("REDEEM_COUNT", "REDEEM_COUNT")
            .Add("REDEEM_MAX", "REDEEM_MAX")
            .Add("MIN_SPEND", "MIN_SPEND")
            .Add("MIN_ITEMS", "MIN_ITEMS")
            .Add("NEW_PRICE", "NEW_PRICE")
            .Add("USER_REDEEM_MAX", "USER_REDEEM_MAX")
            .Add("PRIORITY_SEQUENCE", "PRIORITY_SEQUENCE")
            .Add("ACTIVE", "ACTIVE")
            .Add("REQUIRED_USER_ATTRIBUTE", "REQUIRED_USER_ATTRIBUTE")
            .Add("ADHOC_PROMOTION", "ADHOC_PROMOTION")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL_IgnoreAdhoc(destinationTable, sourceTable, keyFields, "ADHOC_PROMOTION")

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPML()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_promotions_lang_work"
        Const destinationTable As String = "tbl_promotions_lang"
        Const adhocField As String = "ADHOC_PROMOTION"
        Const adhocTable As String = "tbl_promotions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("LANGUAGE_CODE", "LANGUAGE_CODE")
            .Add("DISPLAY_NAME", "DISPLAY_NAME")
            .Add("REQUIREMENTS_DESCRIPTION", "REQUIREMENTS_DESCRIPTION")
            .Add("RULES_NOT_MET_ERROR", "RULES_NOT_MET_ERROR")
            .Add("USER_REDEEM_MAX_EXCEEDED_ERROR", "USER_REDEEM_MAX_EXCEEDED_ERROR")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("LANGUAGE_CODE", "LANGUAGE_CODE")
        End With

        'Build a dictionary of the field mappings
        Dim deletekeyFields As New Generic.Dictionary(Of String, String)
        With deletekeyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL_IgnoreAdhocPromotions(destinationTable, sourceTable, keyFields, adhocField, adhocTable, deletekeyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPMD()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_promotions_discounts_work"
        Const destinationTable As String = "tbl_promotions_discounts"
        Const adhocField As String = "ADHOC_PROMOTION"
        Const adhocTable As String = "tbl_promotions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("IS_PERCENTAGE", "IS_PERCENTAGE")
            .Add("VALUE", "VALUE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL_IgnoreAdhocPromotions(destinationTable, sourceTable, keyFields, adhocField, adhocTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPMR()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_promotions_required_products_work"
        Const destinationTable As String = "tbl_promotions_required_products"
        Const adhocField As String = "ADHOC_PROMOTION"
        Const adhocTable As String = "tbl_promotions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("QUANTITY", "QUANTITY")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
        End With

        'Build a dictionary of the field mappings for delete separately as tbl_promotions table
        'not having product_code
        Dim deleteKeyFields As New Generic.Dictionary(Of String, String)
        With deleteKeyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL_IgnoreAdhocPromotions(destinationTable, sourceTable, keyFields, adhocField, adhocTable, deleteKeyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Public Shared Sub DoUpdateTable_EBPMF()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_promotions_free_products_work"
        Const destinationTable As String = "tbl_promotions_free_products"
        Const adhocField As String = "ADHOC_PROMOTION"
        Const adhocTable As String = "tbl_promotions"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("QUANTITY", "QUANTITY")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("ALLOW_SELECT_OPTION", "ALLOW_SELECT_OPTION")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
        End With

        'Build a dictionary of the field mappings for delete separately as tbl_promotions table
        'not having product_code
        Dim deleteKeyFields As New Generic.Dictionary(Of String, String)
        With deleteKeyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER_GROUP", "PARTNER_GROUP")
            .Add("PARTNER", "PARTNER")
            .Add("PROMOTION_CODE", "PROMOTION_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL_IgnoreAdhocPromotions(destinationTable, sourceTable, keyFields, adhocField, adhocTable, deleteKeyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBOTHD()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_order_template_header_work"
        Const destinationTable As String = "tbl_order_template_header"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("NAME", "NAME")
            .Add("DESCRIPTION", "DESCRIPTION")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("LOGINID", "LOGINID")
            .Add("CREATED_DATE", "CREATED_DATE")
            .Add("LAST_MODIFIED_DATE", "LAST_MODIFIED_DATE")
            .Add("LAST_USED_DATE", "LAST_USED_DATE")
            .Add("IS_DEFAULT", "IS_DEFAULT")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("NAME", "NAME")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("LOGINID", "LOGINID")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBOTDT()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_order_template_detail_work"
        Const destinationTable As String = "tbl_order_template_detail"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("TEMPLATE_HEADER_ID", "TEMPLATE_HEADER_ID")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
            .Add("QUANTITY", "QUANTITY")
            .Add("MASTER_PRODUCT", "MASTER_PRODUCT")
            .Add("NAME", "NAME")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("NAME", "NAME")
            .Add("PRODUCT_CODE", "PRODUCT_CODE")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        'DoUpdateTableNoDelete(update.ToString, insert.ToString)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBUSERT(ByVal DestinationDataBase As String, ByVal CreatePartnerOnAddCustomer As Boolean)

        Select Case DestinationDataBase
            Case "ADL"
                DoDunhillUserUpdate()
            Case "CHORUS"
                DoChorusUserUpdate(CreatePartnerOnAddCustomer)
                DoChorusUserImport(CreatePartnerOnAddCustomer)
            Case "CHORUSERP"
                DoChorusERPUserUpdate()
                DoChorusErpUserImport()

        End Select

    End Sub

    Private Shared Sub DoChorusUserImport(ByVal CreatePartnerOnAddCustomer As Boolean)

        Dim insertPartnerUser As String = ""
        Dim insertPartner As String = ""
        Dim insertAddress As String = ""
        Dim insertAuthorisedUsers As String = ""

        Dim connectionString As String = SQLConnectionString()


        insertAuthorisedUsers = " INSERT INTO [tbl_authorized_users] " & _
                                "            ([BUSINESS_UNIT] " & _
                                "            ,[PARTNER] " & _
                                "            ,[LOGINID] " & _
                                "            ,[PASSWORD] " & _
                                "            ,[AUTO_PROCESS_DEFAULT_USER] " & _
                                "            ,[IS_APPROVED] " & _
                                "            ,[IS_LOCKED_OUT] " & _
                                "            ,[CREATED_DATE] " & _
                                "            ,[LAST_LOGIN_DATE] " & _
                                "            ,[LAST_PASSWORD_CHANGED_DATE] " & _
                                "            ,[LAST_LOCKED_OUT_DATE]) " & _
                                "      SELECT uw.BUSINESS_UNIT " & _
                                "            ,uw.USER_NUMBER " & _
                                "            ,uw.USER_NUMBER " & _
                                "            ,uw.PASSWORD " & _
                                "            ,'False' " & _
                                "            ,CASE uw.ACTIVE WHEN 'N' THEN 'False' ELSE 'True' END  " & _
                                "            ,CASE uw.ACTIVE WHEN 'N' THEN 'False' ELSE 'True' END " & _
                                "            ,GETDATE() " & _
                                "            ,GETDATE() " & _
                                "            ,GETDATE() " & _
                                "            ,GETDATE() " & _
                                "           FROM tbl_user_work as uw WITH (NOLOCK)  " & _
                                "           WHERE uw.USER_NUMBER NOT IN ( " & _
                                "                                     SELECT au.LOGINID " & _
                                "                                     FROM tbl_authorized_users as au WITH (NOLOCK)  " & _
                                "                                     ) " & _
                                "           "

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertAuthorisedUsers, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using


        insertPartnerUser = " INSERT INTO tbl_partner_user " & _
                            "            ([PARTNER] " & _
                            "            ,[LOGINID] " & _
                            "            ,[EMAIL] " & _
                            "            ,[TITLE] " & _
                            "            ,[INITIALS] " & _
                            "            ,[FORENAME] " & _
                            "            ,[SURNAME] " & _
                            "            ,[FULL_NAME] " & _
                            "            ,[SALUTATION] " & _
                            "            ,[POSITION] " & _
                            "            ,[DOB] " & _
                            "            ,[MOBILE_NUMBER] " & _
                            "            ,[TELEPHONE_NUMBER] " & _
                            "            ,[WORK_NUMBER] " & _
                            "            ,[FAX_NUMBER] " & _
                            "            ,[OTHER_NUMBER] " & _
                            "            ,[MESSAGING_ID] " & _
                            "            ,[USER_NUMBER] " & _
                            "            ,[ORIGINATING_BUSINESS_UNIT] " & _
                            "            ,[ACCOUNT_NO_1] " & _
                            "            ,[ACCOUNT_NO_2] " & _
                            "            ,[ACCOUNT_NO_3] " & _
                            "            ,[ACCOUNT_NO_4] " & _
                            "            ,[ACCOUNT_NO_5] " & _
                            "            ,[SUBSCRIBE_NEWSLETTER] " & _
                            "            ,[HTML_NEWSLETTER] " & _
                            "            ,[BIT1] " & _
                            "            ,[BIT2] " & _
                            "            ,[BIT3] " & _
                            "            ,[BIT4] " & _
                            "            ,[BIT5] " & _
                            "            ,[SEX] " & _
                            "            ,[USER_NUMBER_PREFIX] " & _
                            "            ,[PREFERRED_BUSINESS_UNIT] " & _
                            "            ,[PREFERRED_LANGUAGE] " & _
                            "            ,[RESTRICTED_PAYMENT_METHOD]" & _
                            "            ,[ENABLE_PRICE_VIEW]" & _
                            "            ,[MINIMUM_PURCHASE_QUANTITY]" & _
                            "            ,[MINIMUM_PURCHASE_AMOUNT]" & _
                            "            ,[USE_MINIMUM_PURCHASE_QUANTITY]" & _
                            "            ,[USE_MINIMUM_PURCHASE_AMOUNT]" & _
                            "            ,[COMPANYNAME]" & _
                            "       ) " & _
                            "      SELECT uw.USER_NUMBER, " & _
                            "           uw.USER_NUMBER, " & _
                            "           uw.EMAIL, " & _
                            "           uw.TITLE, " & _
                            "           '', " & _
                            "           uw.FORENAME, " & _
                            "           uw.SURNAME, " & _
                            "           uw.FULL_NAME, " & _
                            "           '', " & _
                            "           '', " & _
                            "           '1/1/1900', " & _
                            "           uw.MOBILE_NO,  " & _
                            "           uw.TEL_NO,  " & _
                            "           uw.WORK_NO,  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           CASE WHEN CHARINDEX('-',uw.USER_NUMBER) > 0 THEN SUBSTRING(uw.USER_NUMBER, 0, CHARINDEX('-',uw.USER_NUMBER)) ELSE uw.USER_NUMBER END,  " & _
                            "           CASE WHEN CHARINDEX('-',uw.USER_NUMBER) > 0 THEN SUBSTRING(uw.USER_NUMBER, CHARINDEX('-',uw.USER_NUMBER) + 1, LEN(uw.USER_NUMBER) - 1) ELSE '' END,  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           '',  " & _
                            "           '', " & _
                            "           uw.PREFERRED_BUSINESS_UNIT, " & _
                            "           uw.PREFERRED_LANGUAGE,  " & _
                            "           '',  " & _
                            "           'True',  " & _
                            "           uw.MINIMUM_ORDER_QUANTITY,  " & _
                            "           uw.MINIMUM_ORDER_VALUE,  " & _
                            "           case when isnull(uw.MINIMUM_ORDER_QUANTITY, 0) > 0 then 'True' else 'False' end,  " & _
                            "           case when isnull(uw.MINIMUM_ORDER_VALUE, 0) > 0 then 'True' else 'False' end,   " & _
                            "           '' " & _
                            "           FROM tbl_user_work as uw WITH (NOLOCK)  " & _
                            "           WHERE  " & _
                            "           uw.USER_NUMBER NOT IN ( " & _
                            "                               SELECT pu.LOGINID " & _
                            "                               FROM tbl_partner_user as pu WITH (NOLOCK)  " & _
                            "                               ) " & _
                            "            " & _
                            ""

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertPartnerUser, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        insertAddress = " INSERT INTO tbl_address " & _
                        "            ([PARTNER] " & _
                        "            ,[LOGINID] " & _
                        "            ,[TYPE] " & _
                        "            ,[REFERENCE] " & _
                        "            ,[SEQUENCE] " & _
                        "            ,[DEFAULT_ADDRESS] " & _
                        "            ,[ADDRESS_LINE_1] " & _
                        "            ,[ADDRESS_LINE_2] " & _
                        "            ,[ADDRESS_LINE_3] " & _
                        "            ,[ADDRESS_LINE_4] " & _
                        "            ,[ADDRESS_LINE_5] " & _
                        "            ,[POST_CODE] " & _
                        "            ,[COUNTRY]) " & _
                        "      SELECT uw.USER_NUMBER " & _
                        "            ,uw.USER_NUMBER " & _
                        "            ,'' " & _
                        "            ,uw.ADDRESS_1 + ' ' + uw.POST_CODE " & _
                        "            ,0 " & _
                        "            ,'True' " & _
                        "            ,uw.ADDRESS_1 " & _
                        "            ,uw.ADDRESS_2 " & _
                        "            ,uw.ADDRESS_3 " & _
                        "            ,uw.ADDRESS_4 " & _
                        "            ,'' " & _
                        "            ,uw.POST_CODE " & _
                        "            ,uw.COUNTRY " & _
                        "           FROM tbl_user_work as uw WITH (NOLOCK)  " & _
                        "           WHERE uw.USER_NUMBER NOT IN ( " & _
                        "                                   SELECT a.LOGINID " & _
                        "                                   FROM tbl_address as a WITH (NOLOCK)  " & _
                        "                                   ) " & _
                        "            "

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertAddress, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        If CreatePartnerOnAddCustomer Then

            insertPartner = "INSERT INTO tbl_partner" & _
             "  ([PARTNER] " & _
             "  ,[PARTNER_DESC] " & _
             "  ,[DESTINATION_DATABASE] " & _
             "  ,[CACHEING_ENABLED] " & _
             "  ,[CACHE_TIME_MINUTES] " & _
             "  ,[LOGGING_ENABLED] " & _
             "  ,[STORE_XML] " & _
             "  ,[ACCOUNT_NO_1] " & _
             "  ,[ACCOUNT_NO_2] " & _
             "  ,[ACCOUNT_NO_3] " & _
             "  ,[ACCOUNT_NO_4] " & _
             "  ,[ACCOUNT_NO_5] " & _
             "  ,[EMAIL] " & _
             "  ,[TELEPHONE_NUMBER]  " & _
             "  ,[FAX_NUMBER] " & _
             "  ,[PARTNER_URL] " & _
             "  ,[PARTNER_NUMBER] " & _
             "  ,[ORIGINATING_BUSINESS_UNIT] " & _
             "  ,[CRM_BRANCH] " & _
             "  ,[VAT_NUMBER] " & _
             "  ,[ENABLE_PRICE_VIEW]" & _
             "  ,[ORDER_ENQUIRY_SHOW_PARTNER_ORDERS]" & _
             "  ,[ENABLE_ALTERNATE_SKU]" & _
             "  ,[MINIMUM_PURCHASE_QUANTITY]" & _
             "  ,[MINIMUM_PURCHASE_AMOUNT]" & _
             "  ,[USE_MINIMUM_PURCHASE_QUANTITY]" & _
             "  ,[USE_MINIMUM_PURCHASE_AMOUNT]" & _
             "  ,[COST_CENTRE]" & _
             " ) " & _
             " Select uw.USER_NUMBER " & _
             "  ,uw.USER_NUMBER " & _
             "  ,uw.COMPANY_NAME " & _
             "  ,1 " & _
             "  ,8 " & _
             "  ,0 " & _
             "  ,0 " & _
             "  ,CASE WHEN CHARINDEX('-',uw.USER_NUMBER) > 0 THEN SUBSTRING(uw.USER_NUMBER, 0, CHARINDEX('-',uw.USER_NUMBER)) ELSE uw.USER_NUMBER END  " & _
             "  ,CASE WHEN CHARINDEX('-',uw.USER_NUMBER) > 0 THEN SUBSTRING(uw.USER_NUMBER, CHARINDEX('-',uw.USER_NUMBER) + 1, LEN(uw.USER_NUMBER) - 1) ELSE '' END  " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,0  " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,'' " & _
             "  ,1 " & _
             "  ,0 " & _
             "  ,0 " & _
             "  ,uw.MINIMUM_ORDER_QUANTITY  " & _
             "  ,uw.MINIMUM_ORDER_VALUE  " & _
            "   ,case when isnull(uw.MINIMUM_ORDER_QUANTITY, 0) > 0 then 'True' else 'False' end  " & _
            "   ,case when isnull(uw.MINIMUM_ORDER_VALUE, 0) > 0 then 'True' else 'False' end  " & _
            "   ,uw.COST_CENTRE " & _
            " from tbl_user_work as uw with (NOLOCK) " & _
            "  where uw.USER_NUMBER not in (select partner from tbl_partner with (NOLOCK))  "

            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(insertPartner, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If

    End Sub

    Private Shared Sub DoChorusUserUpdate(ByVal CreatePartnerOnAddCustomer As Boolean)

        Dim updatePartnerUser As New StringBuilder
        Dim updateAddress As New StringBuilder
        Dim updatePartner As New StringBuilder

        Dim connectionString As String = SQLConnectionString()

        With updatePartnerUser
            .Append("UPDATE tbl_partner_user ")
            .Append("SET ")
            .Append("EMAIL = tbl_user_work.EMAIL, ")
            .Append("TITLE = tbl_user_work.title, ")
            .Append("FORENAME = tbl_user_work.forename, ")
            .Append("SURNAME = tbl_user_work.surname, ")
            .Append("FULL_NAME = tbl_user_work.full_name, ")
            .Append("MOBILE_NUMBER = tbl_user_work.mobile_no, ")
            .Append("TELEPHONE_NUMBER = tbl_user_work.tel_no, ")
            .Append("WORK_NUMBER = tbl_user_work.work_no, ")
            .Append("ACCOUNT_NO_1 = CASE WHEN CHARINDEX('-',tbl_user_work.USER_NUMBER) > 0 THEN SUBSTRING(tbl_user_work.USER_NUMBER, 0, CHARINDEX('-',tbl_user_work.USER_NUMBER)) ELSE tbl_user_work.USER_NUMBER END, ")
            .Append("ACCOUNT_NO_2 = CASE WHEN CHARINDEX('-',tbl_user_work.USER_NUMBER) > 0 THEN SUBSTRING(tbl_user_work.USER_NUMBER, CHARINDEX('-',tbl_user_work.USER_NUMBER) + 1, LEN(tbl_user_work.USER_NUMBER) - 1) ELSE '' END, ")
            .Append("PREFERRED_BUSINESS_UNIT = tbl_user_work.PREFERRED_BUSINESS_UNIT, ")
            .Append("PREFERRED_LANGUAGE = tbl_user_work.PREFERRED_LANGUAGE, ")
            .Append("MINIMUM_PURCHASE_QUANTITY = tbl_user_work.MINIMUM_ORDER_QUANTITY, ")
            .Append("MINIMUM_PURCHASE_AMOUNT = tbl_user_work.MINIMUM_ORDER_VALUE, ")
            .Append("USE_MINIMUM_PURCHASE_QUANTITY = case when isnull(tbl_user_work.MINIMUM_ORDER_QUANTITY,0) > 0 then 1 else 0 end, ")
            .Append("USE_MINIMUM_PURCHASE_AMOUNT = case when isnull(tbl_user_work.MINIMUM_ORDER_VALUE,0) > 0 then 1 else 0 end, ")
            .Append("COMPANYNAME = '' ")
            .Append("FROM tbl_user_work WITH (NOLOCK)  ")
            .Append("WHERE tbl_user_work.USER_NUMBER = tbl_partner_user.LOGINID")
        End With

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updatePartnerUser.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        With updateAddress
            .Append("UPDATE tbl_address ")
            .Append("SET ")
            .Append("ADDRESS_LINE_1 = tbl_user_work.address_1, ")
            .Append("ADDRESS_LINE_2 = tbl_user_work.address_2, ")
            .Append("ADDRESS_LINE_3 = tbl_user_work.address_3, ")
            .Append("ADDRESS_LINE_4 = tbl_user_work.address_4, ")
            .Append("POST_CODE = tbl_user_work.post_code, ")
            .Append("COUNTRY = tbl_user_work.country, ")
            .Append("REFERENCE = tbl_user_work.address_1 + ' ' + tbl_user_work.post_code ")
            .Append("FROM tbl_user_work WITH (NOLOCK)   ")
            .Append("WHERE tbl_user_work.USER_NUMBER = tbl_address.LOGINID")
        End With

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updateAddress.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        If CreatePartnerOnAddCustomer Then

            With updatePartner
                .Append("UPDATE tbl_partner ")
                .Append("SET ")
                .Append(" [PARTNER] = uw.USER_NUMBER")
                .Append(",[PARTNER_DESC] = uw.USER_NUMBER")
                .Append(",[DESTINATION_DATABASE] = ''")
                .Append(",[CACHEING_ENABLED] = 1")
                .Append(",[CACHE_TIME_MINUTES] = 8")
                .Append(",[LOGGING_ENABLED] = 0")
                .Append(",[STORE_XML] = 0")
                .Append(",[ACCOUNT_NO_1] = uw.USER_NUMBER")
                .Append(",[ACCOUNT_NO_2] = ''")
                .Append(",[ACCOUNT_NO_3] = ''")
                .Append(",[ACCOUNT_NO_4] = ''")
                .Append(",[ACCOUNT_NO_5] = ''")
                .Append(",[EMAIL] = ''")
                .Append(",[TELEPHONE_NUMBER] = ''")
                .Append(",[FAX_NUMBER] = ''")
                .Append(",[PARTNER_URL] = ''")
                .Append(",[PARTNER_NUMBER] = 0")
                .Append(",[ORIGINATING_BUSINESS_UNIT] = ''")
                .Append(",[CRM_BRANCH] = ''")
                .Append(",[VAT_NUMBER] = ''")
                .Append(",[MINIMUM_PURCHASE_QUANTITY] = uw.MINIMUM_ORDER_QUANTITY ")
                .Append(",[MINIMUM_PURCHASE_AMOUNT] = uw.MINIMUM_ORDER_VALUE ")
                .Append(",[USE_MINIMUM_PURCHASE_QUANTITY] = case when isnull(uw.MINIMUM_ORDER_QUANTITY,0) > 0 then 'True' else 'False' end ")
                .Append(",[USE_MINIMUM_PURCHASE_AMOUNT] = case when isnull(uw.MINIMUM_ORDER_VALUE,0) > 0 then 'True' else 'False' end ")
                .Append(",[COST_CENTRE] = uw.COST_CENTRE")
                .Append(" ")
                .Append("FROM tbl_partner ")
                .Append("INNER JOIN tbl_user_work uw ")
                .Append("ON tbl_partner.partner = uw.user_number ")
            End With

            Using con As New SqlConnection(connectionString)
                Dim cmd As New SqlCommand(updatePartner.ToString, con)
                cmd.CommandTimeout = DefaultSQLTimeout()
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End If

    End Sub

    Private Shared Sub DoChorusErpUserImport()

        Dim insertPartnerUser As String = ""
        Dim insertAddress As String = ""
        Dim insertAuthorisedUsers As String = ""
        Dim insertPartner As String = ""

        Dim connectionString As String = SQLConnectionString()


        insertAuthorisedUsers = " INSERT INTO [tbl_authorized_users] " & _
                                "            ([BUSINESS_UNIT] " & _
                                "            ,[PARTNER] " & _
                                "            ,[LOGINID] " & _
                                "            ,[PASSWORD] " & _
                                "            ,[AUTO_PROCESS_DEFAULT_USER] " & _
                                "            ,[IS_APPROVED] " & _
                                "            ,[IS_LOCKED_OUT] " & _
                                "            ,[CREATED_DATE] " & _
                                "            ,[LAST_LOGIN_DATE] " & _
                                "            ,[LAST_PASSWORD_CHANGED_DATE] " & _
                                "            ,[LAST_LOCKED_OUT_DATE]) " & _
                                "      SELECT uw.BUSINESS_UNIT " & _
                                "            ,uw.USER_NUMBER " & _
                                "            ,uw.BACK_OFFICE_NUMBER " & _
                                "            ,uw.PASSWORD " & _
                                "            ,'False' " & _
                                "            ,CASE uw.ACTIVE WHEN 'N' THEN 'False' ELSE 'True' END  " & _
                                "            ,CASE uw.ACTIVE WHEN 'N' THEN 'False' ELSE 'True' END " & _
                                "            ,GETDATE() " & _
                                "            ,GETDATE() " & _
                                "            ,GETDATE() " & _
                                "            ,GETDATE() " & _
                                "           FROM tbl_user_work as uw WITH (NOLOCK)  " & _
                                "           WHERE uw.BACK_OFFICE_NUMBER NOT IN ( " & _
                                "                                     SELECT au.LOGINID " & _
                                "                                     FROM tbl_authorized_users as au WITH (NOLOCK)  " & _
                                "                                     ) " & _
                                "           "

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertAuthorisedUsers, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using


        insertPartnerUser = " INSERT INTO tbl_partner_user " & _
                            "            ([PARTNER] " & _
                            "            ,[LOGINID] " & _
                            "            ,[EMAIL] " & _
                            "            ,[TITLE] " & _
                            "            ,[INITIALS] " & _
                            "            ,[FORENAME] " & _
                            "            ,[SURNAME] " & _
                            "            ,[FULL_NAME] " & _
                            "            ,[SALUTATION] " & _
                            "            ,[POSITION] " & _
                            "            ,[DOB] " & _
                            "            ,[MOBILE_NUMBER] " & _
                            "            ,[TELEPHONE_NUMBER] " & _
                            "            ,[WORK_NUMBER] " & _
                            "            ,[FAX_NUMBER] " & _
                            "            ,[OTHER_NUMBER] " & _
                            "            ,[MESSAGING_ID] " & _
                            "            ,[USER_NUMBER] " & _
                            "            ,[ORIGINATING_BUSINESS_UNIT] " & _
                            "            ,[ACCOUNT_NO_1] " & _
                            "            ,[ACCOUNT_NO_2] " & _
                            "            ,[ACCOUNT_NO_3] " & _
                            "            ,[ACCOUNT_NO_4] " & _
                            "            ,[ACCOUNT_NO_5] " & _
                            "            ,[SUBSCRIBE_NEWSLETTER] " & _
                            "            ,[HTML_NEWSLETTER] " & _
                            "            ,[BIT1] " & _
                            "            ,[BIT2] " & _
                            "            ,[BIT3] " & _
                            "            ,[BIT4] " & _
                            "            ,[BIT5] " & _
                            "            ,[SEX] " & _
                            "            ,[USER_NUMBER_PREFIX] " & _
                            "            ,[PREFERRED_BUSINESS_UNIT] " & _
                            "            ,[PREFERRED_LANGUAGE] " & _
                            "            ,[RESTRICTED_PAYMENT_METHOD] " & _
                            "            ,[SUBSCRIBE_2] " & _
                            "            ,[SUBSCRIBE_3] " & _
                            "            ,[TICKETING_LOYALTY_POINTS] " & _
                            "            ,[ATTRIBUTES_LIST], " & _
                            "            ,[BOND_HOLDER], " & _
                            "            ,[MINIMUM_PURCHASE_QUANTITY]" & _
                            "            ,[MINIMUM_PURCHASE_AMOUNT]" & _
                            "            ,[USE_MINIMUM_PURCHASE_QUANTITY]" & _
                            "            ,[USE_MINIMUM_PURCHASE_AMOUNT]" & _
                            "            ,[COMPANYNAME]" & _
                            ")      SELECT uw.USER_NUMBER, " & _
                            "           uw.BACK_OFFICE_NUMBER, " & _
                            "           uw.EMAIL, " & _
                            "           uw.TITLE, " & _
                            "           '', " & _
                            "           uw.FORENAME, " & _
                            "           uw.SURNAME, " & _
                            "           uw.FULL_NAME, " & _
                            "           '', " & _
                            "           '', " & _
                            "           '1/1/1900', " & _
                            "           uw.MOBILE_NO,  " & _
                            "           uw.TEL_NO,  " & _
                            "           uw.WORK_NO,  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           uw.USER_NUMBER, " & _
                            "           '', " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           '',  " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           'False', " & _
                            "           '',  " & _
                            "           '', " & _
                            "           uw.PREFERRED_BUSINESS_UNIT, " & _
                            "           uw.PREFERRED_LANGUAGE,  " & _
                            "           '',  " & _
                            "           'False',  " & _
                            "           'False',  " & _
                            "           0,  " & _
                            "           '',  " & _
                            "           'False',  " & _
                            "           uw.MINIMUM_ORDER_QUANTITY,  " & _
                            "           uw.MINIMUM_ORDER_VALUE,  " & _
                            "           case when isnull(uw.MINIMUM_ORDER_QUANTITY, 0) > 0 then 'True' else 'False' end,  " & _
                            "           case when isnull(uw.MINIMUM_ORDER_VALUE, 0) > 0 then 'True' else 'False' end,  " & _
                            "           '' " & _
                            "           FROM tbl_user_work as uw WITH (NOLOCK)  " & _
                            "           WHERE  " & _
                            "           uw.BACK_OFFICE_NUMBER NOT IN ( " & _
                            "                               SELECT pu.LOGINID " & _
                            "                               FROM tbl_partner_user as pu WITH (NOLOCK)  " & _
                            "                               ) " & _
                            "            " & _
                            ""

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertPartnerUser, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        insertAddress = " INSERT INTO tbl_address " & _
                        "            ([PARTNER] " & _
                        "            ,[LOGINID] " & _
                        "            ,[TYPE] " & _
                        "            ,[REFERENCE] " & _
                        "            ,[SEQUENCE] " & _
                        "            ,[DEFAULT_ADDRESS] " & _
                        "            ,[ADDRESS_LINE_1] " & _
                        "            ,[ADDRESS_LINE_2] " & _
                        "            ,[ADDRESS_LINE_3] " & _
                        "            ,[ADDRESS_LINE_4] " & _
                        "            ,[ADDRESS_LINE_5] " & _
                        "            ,[POST_CODE] " & _
                        "            ,[COUNTRY]) " & _
                        "      SELECT uw.USER_NUMBER " & _
                        "            ,uw.BACK_OFFICE_NUMBER " & _
                        "            ,'' " & _
                        "            ,uw.ADDRESS_1 + ' ' + uw.POST_CODE " & _
                        "            ,0 " & _
                        "            ,'True' " & _
                        "            ,uw.ADDRESS_1 " & _
                        "            ,uw.ADDRESS_2 " & _
                        "            ,uw.ADDRESS_3 " & _
                        "            ,uw.ADDRESS_4 " & _
                        "            ,'' " & _
                        "            ,uw.POST_CODE " & _
                        "            ,uw.COUNTRY " & _
                        "           FROM tbl_user_work as uw WITH (NOLOCK)  " & _
                        "           WHERE uw.BACK_OFFICE_NUMBER NOT IN ( " & _
                        "                                   SELECT a.LOGINID " & _
                        "                                   FROM tbl_address as a WITH (NOLOCK)  " & _
                        "                                   ) " & _
                        "            "

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertAddress, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        insertPartner = "INSERT INTO tbl_partner" & _
         "  ([PARTNER] " & _
         "  ,[PARTNER_DESC] " & _
         "  ,[DESTINATION_DATABASE] " & _
         "  ,[CACHEING_ENABLED] " & _
         "  ,[CACHE_TIME_MINUTES] " & _
         "  ,[LOGGING_ENABLED] " & _
         "  ,[STORE_XML] " & _
         "  ,[ACCOUNT_NO_1] " & _
         "  ,[ACCOUNT_NO_2] " & _
         "  ,[ACCOUNT_NO_3] " & _
         "  ,[ACCOUNT_NO_4] " & _
         "  ,[ACCOUNT_NO_5] " & _
         "  ,[EMAIL] " & _
         "  ,[TELEPHONE_NUMBER]  " & _
         "  ,[FAX_NUMBER] " & _
         "  ,[PARTNER_URL] " & _
         "  ,[PARTNER_NUMBER] " & _
         "  ,[ORIGINATING_BUSINESS_UNIT] " & _
         "  ,[CRM_BRANCH] " & _
         "  ,[VAT_NUMBER] " & _
         "  ,[COST_CENTRE]) " & _
         " Select uw.USER_NUMBER " & _
         "  ,uw.COMPANY_NAME " & _
         "  ,'' " & _
         "  ,1 " & _
         "  ,8 " & _
         "  ,0 " & _
         "  ,0 " & _
         "  ,uw.USER_NUMBER " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,0  " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,'' " & _
         "  ,uw.COST_CENTRE " & _
        " from tbl_user_work as uw with (NOLOCK) " & _
        "  where uw.USER_NUMBER not in (select partner from tbl_partner with (NOLOCK))  "

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(insertPartner, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

    End Sub

    Private Shared Sub DoChorusERPUserUpdate()

        Dim updatePartnerUser As New StringBuilder
        Dim updateAddress As New StringBuilder

        Dim connectionString As String = SQLConnectionString()

        With updatePartnerUser
            .Append("UPDATE tbl_partner_user ")
            .Append("SET ")
            .Append("EMAIL = tbl_user_work.EMAIL, ")
            .Append("TITLE = tbl_user_work.title, ")
            .Append("FORENAME = tbl_user_work.forename, ")
            .Append("SURNAME = tbl_user_work.surname, ")
            .Append("FULL_NAME = tbl_user_work.full_name, ")
            .Append("MOBILE_NUMBER = tbl_user_work.mobile_no, ")
            .Append("TELEPHONE_NUMBER = tbl_user_work.tel_no, ")
            .Append("WORK_NUMBER = tbl_user_work.work_no, ")
            .Append("ACCOUNT_NO_1 = SUBSTRING(tbl_user_work.USER_NUMBER, 0, CHARINDEX('-',tbl_user_work.USER_NUMBER)), ")
            .Append("ACCOUNT_NO_2 = SUBSTRING(tbl_user_work.USER_NUMBER, CHARINDEX('-',tbl_user_work.USER_NUMBER) + 1, LEN(tbl_user_work.USER_NUMBER) - 1), ")
            .Append("PREFERRED_BUSINESS_UNIT = tbl_user_work.PREFERRED_BUSINESS_UNIT, ")
            .Append("PREFERRED_LANGUAGE = tbl_user_work.PREFERRED_LANGUAGE ")
            .Append("FROM tbl_user_work WITH (NOLOCK)  ")
            .Append("WHERE tbl_user_work.BACK_OFFICE_NUMBER = tbl_partner_user.LOGINID")
        End With

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updatePartnerUser.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        With updateAddress
            .Append("UPDATE tbl_address ")
            .Append("SET ")
            .Append("ADDRESS_LINE_1 = tbl_user_work.address_1, ")
            .Append("ADDRESS_LINE_2 = tbl_user_work.address_2, ")
            .Append("ADDRESS_LINE_3 = tbl_user_work.address_3, ")
            .Append("ADDRESS_LINE_4 = tbl_user_work.address_4, ")
            .Append("POST_CODE = tbl_user_work.post_code, ")
            .Append("COUNTRY = tbl_user_work.country, ")
            .Append("REFERENCE = tbl_user_work.address_1 + ' ' + tbl_user_work.post_code ")
            .Append("FROM tbl_user_work WITH (NOLOCK)   ")
            .Append("WHERE tbl_user_work.BACK_OFFICE_NUMBER = tbl_address.LOGINID")
        End With

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updateAddress.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

    End Sub

    Private Shared Sub DoDunhillUserUpdate()

        Dim updatePartnerUser As New StringBuilder
        Dim updateAddress As New StringBuilder

        Dim connectionString As String = SQLConnectionString()

        With updatePartnerUser
            .Append("UPDATE tbl_partner_user ")
            .Append("SET ")
            .Append("TITLE = tbl_user_work.title, ")
            .Append("FORENAME = tbl_user_work.forename, ")
            .Append("SURNAME = tbl_user_work.surname, ")
            .Append("FULL_NAME = tbl_user_work.full_name, ")
            .Append("MOBILE_NUMBER = tbl_user_work.mobile_no, ")
            .Append("TELEPHONE_NUMBER = tbl_user_work.tel_no, ")
            .Append("WORK_NUMBER = tbl_user_work.work_no, ")
            .Append("ACCOUNT_NO_1 = tbl_user_work.BACK_OFFICE_NUMBER, ")
            .Append("PREFERRED_BUSINESS_UNIT = tbl_user_work.PREFERRED_BUSINESS_UNIT, ")
            .Append("PREFERRED_LANGUAGE = tbl_user_work.PREFERRED_LANGUAGE ")
            .Append("FROM tbl_user_work WITH (NOLOCK)  ")
            .Append("WHERE SUBSTRING(tbl_user_work.USER_NUMBER, 1, 1) = tbl_partner_user.USER_NUMBER_PREFIX ")
            .Append("AND CAST(SUBSTRING(tbl_user_work.USER_NUMBER, 2, 9) AS INT) = tbl_partner_user.USER_NUMBER ")
            .Append("AND EXISTS ")
            .Append("(SELECT * FROM tbl_partner_user WITH (NOLOCK)  ")
            .Append("WHERE SUBSTRING(tbl_user_work.USER_NUMBER, 1, 1) = tbl_partner_user.USER_NUMBER_PREFIX ")
            .Append("AND CAST(SUBSTRING(tbl_user_work.USER_NUMBER, 2, 9) AS INT) = tbl_partner_user.USER_NUMBER)")
        End With

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updatePartnerUser.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

        With updateAddress
            .Append("UPDATE tbl_address ")
            .Append("SET ")
            .Append("ADDRESS_LINE_1 = tbl_user_work.address_1, ")
            .Append("ADDRESS_LINE_2 = tbl_user_work.address_2, ")
            .Append("ADDRESS_LINE_3 = tbl_user_work.address_3, ")
            .Append("ADDRESS_LINE_4 = tbl_user_work.address_4, ")
            .Append("POST_CODE = tbl_user_work.post_code, ")
            .Append("COUNTRY = tbl_user_work.country, ")
            .Append("REFERENCE = tbl_user_work.address_1 + ' ' + tbl_user_work.address_2 ")
            .Append("FROM tbl_user_work WITH (NOLOCK)  join tbl_partner_user WITH (NOLOCK)  on ")
            .Append("SUBSTRING(tbl_user_work.USER_NUMBER, 1, 1) = tbl_partner_user.USER_NUMBER_PREFIX ")
            .Append("AND CAST(SUBSTRING(tbl_user_work.USER_NUMBER, 2, 9) AS INT) = tbl_partner_user.USER_NUMBER ")
            .Append("WHERE tbl_address.partner = tbl_partner_user.partner ")
            .Append("AND tbl_address.loginid = tbl_partner_user.loginid ")
            .Append("AND tbl_address.sequence = 0 ")
            .Append("AND EXISTS ")
            .Append("(SELECT * FROM tbl_partner_user WITH (NOLOCK)  ")
            .Append("WHERE SUBSTRING(tbl_user_work.USER_NUMBER, 1, 1) = tbl_partner_user.USER_NUMBER_PREFIX ")
            .Append("AND CAST(SUBSTRING(tbl_user_work.USER_NUMBER, 2, 9) AS INT) = tbl_partner_user.USER_NUMBER) ")
        End With

        Using con As New SqlConnection(connectionString)
            Dim cmd As New SqlCommand(updateAddress.ToString, con)
            cmd.CommandTimeout = DefaultSQLTimeout()
            con.Open()
            cmd.ExecuteNonQuery()
            con.Close()
        End Using

    End Sub

    Public Shared Sub DoUpdateTable_EBPRRL(Optional ByVal business_unit As String = "", _
                                            Optional ByVal partner As String = "")
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_relations_work"
        Const destinationTable As String = "tbl_product_relations"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("PRODUCT_RELATIONS_ID", "PRODUCT_RELATIONS_ID")
            .Add("QUALIFIER", "QUALIFIER")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("GROUP_L01_GROUP", "GROUP_L01_GROUP")
            .Add("GROUP_L02_GROUP", "GROUP_L02_GROUP")
            .Add("GROUP_L03_GROUP", "GROUP_L03_GROUP")
            .Add("GROUP_L04_GROUP", "GROUP_L04_GROUP")
            .Add("GROUP_L05_GROUP", "GROUP_L05_GROUP")
            .Add("GROUP_L06_GROUP", "GROUP_L06_GROUP")
            .Add("GROUP_L07_GROUP", "GROUP_L07_GROUP")
            .Add("GROUP_L08_GROUP", "GROUP_L08_GROUP")
            .Add("GROUP_L09_GROUP", "GROUP_L09_GROUP")
            .Add("GROUP_L10_GROUP", "GROUP_L10_GROUP")
            .Add("PRODUCT", "PRODUCT")
            .Add("RELATED_GROUP_L01_GROUP", "RELATED_GROUP_L01_GROUP")
            .Add("RELATED_GROUP_L02_GROUP", "RELATED_GROUP_L02_GROUP")
            .Add("RELATED_GROUP_L03_GROUP", "RELATED_GROUP_L03_GROUP")
            .Add("RELATED_GROUP_L04_GROUP", "RELATED_GROUP_L04_GROUP")
            .Add("RELATED_GROUP_L05_GROUP", "RELATED_GROUP_L05_GROUP")
            .Add("RELATED_GROUP_L06_GROUP", "RELATED_GROUP_L06_GROUP")
            .Add("RELATED_GROUP_L07_GROUP", "RELATED_GROUP_L07_GROUP")
            .Add("RELATED_GROUP_L08_GROUP", "RELATED_GROUP_L08_GROUP")
            .Add("RELATED_GROUP_L09_GROUP", "RELATED_GROUP_L09_GROUP")
            .Add("RELATED_GROUP_L10_GROUP", "RELATED_GROUP_L10_GROUP")
            .Add("RELATED_PRODUCT", "RELATED_PRODUCT")
            .Add("SEQUENCE", "SEQUENCE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("QUALIFIER", "QUALIFIER")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("GROUP_L01_GROUP", "GROUP_L01_GROUP")
            .Add("GROUP_L02_GROUP", "GROUP_L02_GROUP")
            .Add("GROUP_L03_GROUP", "GROUP_L03_GROUP")
            .Add("GROUP_L04_GROUP", "GROUP_L04_GROUP")
            .Add("GROUP_L05_GROUP", "GROUP_L05_GROUP")
            .Add("GROUP_L06_GROUP", "GROUP_L06_GROUP")
            .Add("GROUP_L07_GROUP", "GROUP_L07_GROUP")
            .Add("GROUP_L08_GROUP", "GROUP_L08_GROUP")
            .Add("GROUP_L09_GROUP", "GROUP_L09_GROUP")
            .Add("GROUP_L10_GROUP", "GROUP_L10_GROUP")
            .Add("PRODUCT", "PRODUCT")
            .Add("RELATED_GROUP_L01_GROUP", "RELATED_GROUP_L01_GROUP")
            .Add("RELATED_GROUP_L02_GROUP", "RELATED_GROUP_L02_GROUP")
            .Add("RELATED_GROUP_L03_GROUP", "RELATED_GROUP_L03_GROUP")
            .Add("RELATED_GROUP_L04_GROUP", "RELATED_GROUP_L04_GROUP")
            .Add("RELATED_GROUP_L05_GROUP", "RELATED_GROUP_L05_GROUP")
            .Add("RELATED_GROUP_L06_GROUP", "RELATED_GROUP_L06_GROUP")
            .Add("RELATED_GROUP_L07_GROUP", "RELATED_GROUP_L07_GROUP")
            .Add("RELATED_GROUP_L08_GROUP", "RELATED_GROUP_L08_GROUP")
            .Add("RELATED_GROUP_L09_GROUP", "RELATED_GROUP_L09_GROUP")
            .Add("RELATED_GROUP_L10_GROUP", "RELATED_GROUP_L10_GROUP")
            .Add("RELATED_PRODUCT", "RELATED_PRODUCT")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        ' If business unit is filled then only delete for BU/PARTNER combo
        If business_unit <> "" Then
            delete = BuildDeleteSQL_BU_Partner(destinationTable, sourceTable, keyFields, _
                                               business_unit, partner, _
                                               "BUSINESS_UNIT", "PARTNER")
        Else
            delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)
        End If


        'HttpContext.Current.Response.Write(update.ToString)
        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBPRST()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_product_stock_work"
        Const destinationTable As String = "tbl_product_stock"


        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("PRODUCT_STOCK_ID", "PRODUCT_STOCK_ID")
            .Add("PRODUCT", "PRODUCT")
            .Add("STOCK_LOCATION", "STOCK_LOCATION")
            .Add("QUANTITY", "QUANTITY")
            .Add("ALLOCATED_QUANTITY", "ALLOCATED_QUANTITY")
            .Add("AVAILABLE_QUANTITY", "AVAILABLE_QUANTITY")
            .Add("RESTOCK_CODE", "RESTOCK_CODE")
            .Add("WAREHOUSE", "WAREHOUSE")
        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("PRODUCT", "PRODUCT")
            .Add("STOCK_LOCATION", "STOCK_LOCATION")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBINVHT()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_invoice_header_work"
        Const destinationTable As String = "tbl_invoice_header"

        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("INVOICE_NO", "INVOICE_NO")
            .Add("ORDER_NO", "ORDER_NO")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("PARTNER_NUMBER", "PARTNER_NUMBER")
            .Add("LOGINID", "LOGINID")
            .Add("INVOICE_DATE", "INVOICE_DATE")
            .Add("INVOICE_AMOUNT", "INVOICE_AMOUNT")
            .Add("VAT_AMOUNT", "VAT_AMOUNT")
            .Add("OUTSTANDING_AMOUNT", "OUTSTANDING_AMOUNT")
            .Add("CUSTOMER_REF", "CUSTOMER_REF")
            .Add("INVOICE_STATUS", "INVOICE_STATUS")
            .Add("CUSTOMER_PO", "CUSTOMER_PO")
            .Add("ORIGINAL_ORDER_NO", "ORIGINAL_ORDER_NO")
            .Add("ORIGINAL_ORDER_DATE", "ORIGINAL_ORDER_DATE")
            .Add("DISPATCH_SEQUENCE", "DISPATCH_SEQUENCE")
            .Add("ACCOUNT_NUMBER", "ACCOUNT_NUMBER")
            .Add("CUSTOMER_NAME", "CUSTOMER_NAME")
            .Add("CUSTOMER_ATTENTION", "CUSTOMER_ATTENTION")
            .Add("CUSTOMER_ADDRESS_1", "CUSTOMER_ADDRESS_1")
            .Add("CUSTOMER_ADDRESS_2", "CUSTOMER_ADDRESS_2")
            .Add("CUSTOMER_ADDRESS_3", "CUSTOMER_ADDRESS_3")
            .Add("CUSTOMER_ADDRESS_4", "CUSTOMER_ADDRESS_4")
            .Add("CUSTOMER_ADDRESS_5", "CUSTOMER_ADDRESS_5")
            .Add("CUSTOMER_ADDRESS_6", "CUSTOMER_ADDRESS_6")
            .Add("CUSTOMER_ADDRESS_7", "CUSTOMER_ADDRESS_7")
            .Add("SHIPTO_NAME", "SHIPTO_NAME")
            .Add("SHIPTO_ATTENTION", "SHIPTO_ATTENTION")
            .Add("SHIPTO_ADDRESS_1", "SHIPTO_ADDRESS_1")
            .Add("SHIPTO_ADDRESS_2", "SHIPTO_ADDRESS_2")
            .Add("SHIPTO_ADDRESS_3", "SHIPTO_ADDRESS_3")
            .Add("SHIPTO_ADDRESS_4", "SHIPTO_ADDRESS_4")
            .Add("SHIPTO_ADDRESS_5", "SHIPTO_ADDRESS_5")
            .Add("SHIPTO_ADDRESS_6", "SHIPTO_ADDRESS_6")
            .Add("SHIPTO_ADDRESS_7", "SHIPTO_ADDRESS_7")
            .Add("SHIPFROM_NAME", "SHIPFROM_NAME")
            .Add("SHIPFROM_ATTENTION", "SHIPFROM_ATTENTION")
            .Add("SHIPFROM_ADDRESS_1", "SHIPFROM_ADDRESS_1")
            .Add("SHIPFROM_ADDRESS_2", "SHIPFROM_ADDRESS_2")
            .Add("SHIPFROM_ADDRESS_3", "SHIPFROM_ADDRESS_3")
            .Add("SHIPFROM_ADDRESS_4", "SHIPFROM_ADDRESS_4")
            .Add("SHIPFROM_ADDRESS_5", "SHIPFROM_ADDRESS_5")
            .Add("SHIPFROM_ADDRESS_6", "SHIPFROM_ADDRESS_6")
            .Add("SHIPFROM_ADDRESS_7", "SHIPFROM_ADDRESS_7")
            .Add("VAT_NUMBER", "VAT_NUMBER")
            .Add("PAYMENT_TERMS_TYPE", "PAYMENT_TERMS_TYPE")
            .Add("PAYMENT_TERMS_PERIOD", "PAYMENT_TERMS_PERIOD")
            .Add("PAYMENT_TERMS_DAYS", "PAYMENT_TERMS_DAYS")
            .Add("FINALISED", "FINALISED")

        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("INVOICE_NO", "INVOICE_NO")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBCNEHT()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_creditnote_header_work"
        Const destinationTable As String = "tbl_creditnote_header"

        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            .Add("CREDITNOTE_NO", "CREDITNOTE_NO")
            .Add("ORDER_NO", "ORDER_NO")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("PARTNER_NUMBER", "PARTNER_NUMBER")
            .Add("LOGINID", "LOGINID")
            .Add("CREDITNOTE_DATE", "CREDITNOTE_DATE")
            .Add("CREDITNOTE_AMOUNT", "CREDITNOTE_AMOUNT")
            .Add("VAT_AMOUNT", "VAT_AMOUNT")
            .Add("OUTSTANDING_AMOUNT", "OUTSTANDING_AMOUNT")
            .Add("CUSTOMER_REF", "CUSTOMER_REF")
            .Add("CREDITNOTE_STATUS", "CREDITNOTE_STATUS")
            .Add("CUSTOMER_PO", "CUSTOMER_PO")
            .Add("ORIGINAL_ORDER_NO", "ORIGINAL_ORDER_NO")
            .Add("ORIGINAL_ORDER_DATE", "ORIGINAL_ORDER_DATE")
            .Add("DISPATCH_SEQUENCE", "DISPATCH_SEQUENCE")
            .Add("ACCOUNT_NUMBER", "ACCOUNT_NUMBER")
            .Add("CUSTOMER_NAME", "CUSTOMER_NAME")
            .Add("CUSTOMER_ATTENTION", "CUSTOMER_ATTENTION")
            .Add("CUSTOMER_ADDRESS_1", "CUSTOMER_ADDRESS_1")
            .Add("CUSTOMER_ADDRESS_2", "CUSTOMER_ADDRESS_2")
            .Add("CUSTOMER_ADDRESS_3", "CUSTOMER_ADDRESS_3")
            .Add("CUSTOMER_ADDRESS_4", "CUSTOMER_ADDRESS_4")
            .Add("CUSTOMER_ADDRESS_5", "CUSTOMER_ADDRESS_5")
            .Add("CUSTOMER_ADDRESS_6", "CUSTOMER_ADDRESS_6")
            .Add("CUSTOMER_ADDRESS_7", "CUSTOMER_ADDRESS_7")
            .Add("SHIPTO_NAME", "SHIPTO_NAME")
            .Add("SHIPTO_ATTENTION", "SHIPTO_ATTENTION")
            .Add("SHIPTO_ADDRESS_1", "SHIPTO_ADDRESS_1")
            .Add("SHIPTO_ADDRESS_2", "SHIPTO_ADDRESS_2")
            .Add("SHIPTO_ADDRESS_3", "SHIPTO_ADDRESS_3")
            .Add("SHIPTO_ADDRESS_4", "SHIPTO_ADDRESS_4")
            .Add("SHIPTO_ADDRESS_5", "SHIPTO_ADDRESS_5")
            .Add("SHIPTO_ADDRESS_6", "SHIPTO_ADDRESS_6")
            .Add("SHIPTO_ADDRESS_7", "SHIPTO_ADDRESS_7")
            .Add("SHIPFROM_NAME", "SHIPFROM_NAME")
            .Add("SHIPFROM_ATTENTION", "SHIPFROM_ATTENTION")
            .Add("SHIPFROM_ADDRESS_1", "SHIPFROM_ADDRESS_1")
            .Add("SHIPFROM_ADDRESS_2", "SHIPFROM_ADDRESS_2")
            .Add("SHIPFROM_ADDRESS_3", "SHIPFROM_ADDRESS_3")
            .Add("SHIPFROM_ADDRESS_4", "SHIPFROM_ADDRESS_4")
            .Add("SHIPFROM_ADDRESS_5", "SHIPFROM_ADDRESS_5")
            .Add("SHIPFROM_ADDRESS_6", "SHIPFROM_ADDRESS_6")
            .Add("SHIPFROM_ADDRESS_7", "SHIPFROM_ADDRESS_7")
            .Add("VAT_NUMBER", "VAT_NUMBER")
            .Add("PAYMENT_TERMS_TYPE", "PAYMENT_TERMS_TYPE")
            .Add("PAYMENT_TERMS_PERIOD", "PAYMENT_TERMS_PERIOD")
            .Add("PAYMENT_TERMS_DAYS", "PAYMENT_TERMS_DAYS")
            '  .Add("FINALISED", "FINALISED")

        End With

        'Build a dictionary of the field mappings
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("CREDITNOTE_NO", "CREDITNOTE_NO")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)

        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub DoUpdateTable_EBECMB()
        Dim insert As New StringBuilder
        Dim update As New StringBuilder
        Dim delete As New StringBuilder

        Const sourceTable As String = "tbl_ecommerce_module_defaults_bu_work"
        Const destinationTable As String = "tbl_ecommerce_module_defaults_bu"

        'Build a dictionary of the field mappings
        Dim fieldMappings As New Generic.Dictionary(Of String, String)
        With fieldMappings
            '.Add("ID", "ID")
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("APPLICATION", "APPLICATION")
            .Add("MODULE", "MODULE")
            .Add("DEFAULT_NAME", "DEFAULT_NAME")
            .Add("VALUE", "VALUE")
        End With

        'Build a dictionary of the KEY FIELDS
        Dim keyFields As New Generic.Dictionary(Of String, String)
        With keyFields
            .Add("BUSINESS_UNIT", "BUSINESS_UNIT")
            .Add("PARTNER", "PARTNER")
            .Add("APPLICATION", "APPLICATION")
            .Add("MODULE", "MODULE")
            .Add("DEFAULT_NAME", "DEFAULT_NAME")
        End With

        'Populate the update and insert stringbuilders
        update = BuildUpdateSQL(destinationTable, sourceTable, fieldMappings, keyFields)
        insert = BuildInsertSQL(destinationTable, sourceTable, fieldMappings, keyFields)

        'DO NOT PERFORM DELETES FOR THE ECommerceModuleDefualts
        'delete = BuildDeleteSQL(destinationTable, sourceTable, keyFields)


        DoUpdateTable(update.ToString, insert.ToString, delete.ToString)
    End Sub

    Private Shared Sub MarkSource(ByVal SourceTable As String)

        Dim CopySourceTable As String = SourceTable
        Select Case CopySourceTable
            Case Is = "EBPLDTP"
                CopySourceTable = "EBPLDT"
        End Select

        '======================================================
        ' COPY the SOURCE table to the EXTRACT table
        '======================================================
        Dim sbSelect As New StringBuilder

        With sbSelect
            .Append("UPDATE ")
            .Append(CopySourceTable)
            .Append(" SET PLDPRC = 'Y' WHERE PLDTYP <> ' '")
        End With

        ' Initialise Connection Objects
        Dim OleDbCon As OleDbConnection = New OleDbConnection()

        ' Initialise Command Objects
        Dim OleDbCmd As OleDbCommand = New OleDbCommand()

        ' Attach the connection strings to the connection objects
        OleDbCon.ConnectionString = OleDbConnectionString()

        ' Attach the connection objects to the commands
        OleDbCmd.Connection = OleDbCon

        ' Set Source Command SQL
        OleDbCmd.CommandTimeout = DefaultOleDBTimeout()
        OleDbCmd.CommandText = sbSelect.ToString()
        OleDbCmd.CommandType = CommandType.Text

        ' Open the Source Connection
        OleDbCmd.Connection.Open()

        Try
            OleDbCmd.ExecuteNonQuery()
        Catch ex As Exception
            'WriteLog(ex.Message)
        Finally
            OleDbCmd.Connection.Close()
        End Try

    End Sub

    Private Shared Sub DeleteSource(ByVal SourceTable As String)

        Dim CopySourceTable As String = SourceTable
        Select Case CopySourceTable
            Case Is = "EBPLDTP"
                CopySourceTable = "EBPLDT"
        End Select

        '======================================================
        ' COPY the SOURCE table to the EXTRACT table
        '======================================================
        Dim sbSelect As New StringBuilder

        With sbSelect
            .Append("DELETE FROM ")
            .Append(CopySourceTable)
            .Append(" WHERE PLDPRC = 'Y'")
        End With

        ' Initialise Connection Objects
        Dim OleDbCon As OleDbConnection = New OleDbConnection()

        ' Initialise Command Objects
        Dim OleDbCmd As OleDbCommand = New OleDbCommand()

        ' Attach the connection strings to the connection objects
        OleDbCon.ConnectionString = OleDbConnectionString()

        ' Attach the connection objects to the commands
        OleDbCmd.Connection = OleDbCon

        ' Set Source Command SQL
        OleDbCmd.CommandTimeout = DefaultOleDBTimeout()
        OleDbCmd.CommandText = sbSelect.ToString()
        OleDbCmd.CommandType = CommandType.Text

        ' Open the Source Connection
        OleDbCmd.Connection.Open()

        Try
            OleDbCmd.ExecuteNonQuery()
        Catch ex As Exception
            'WriteLog(ex.Message)
        Finally
            OleDbCmd.Connection.Close()
        End Try

    End Sub

End Class
