Imports System.Data.SqlClient
Imports System.Text

Public Class DBOrderTemplates
    Inherits DBAccess

    Const empty As String = "EMPTY"

    Private _deOTs As DEOrderTemplates
    Public Property DE_OrderTemplates() As DEOrderTemplates
        Get
            Return _deOTs
        End Get
        Set(ByVal value As DEOrderTemplates)
            _deOTs = value
        End Set
    End Property

    Private ReadOnly Property DtHeader() As DataTable
        Get
            Return TemplatesRecordSet.Tables("OrderTemplatesHeader")
        End Get
    End Property

    Private ReadOnly Property DtDetail() As DataTable
        Get
            Return TemplatesRecordSet.Tables("OrderTemplatesDetail")
        End Get
    End Property

    Private _templatesRecordSet As DataSet
    Public Property TemplatesRecordSet() As DataSet
        Get
            Return _templatesRecordSet
        End Get
        Set(ByVal value As DataSet)
            _templatesRecordSet = value
        End Set
    End Property

    Public Sub New(ByVal DE_OrdTemps As DEOrderTemplates)
        MyBase.New()
        DE_OrderTemplates = DE_OrdTemps
    End Sub

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim conTalent As SqlConnection = Nothing
        Dim cmd As New SqlCommand

        err = BuildOrderTemplateDataSetFromDataEntity()

        Try
            'Create a new connection with the SQL connection string
            conTalent = New SqlConnection(Settings.FrontEndConnectionString)
            cmd.Connection = conTalent
            cmd.Connection.Open()
        Catch ex As Exception
            If Not err.HasError Then
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ""
                    .ErrorNumber = "TACDBOR07"
                    .HasError = True
                End With
            End If
        End Try

        If Not err.HasError Then
            Select Case UCase(DE_OrderTemplates.Purpose)
                Case Is = "SELECT"
                    err = SelectTemplatesSQL2005(cmd)
                Case Is = "INSERT"
                    err = InsertTemplatesSQL2005(cmd)
                Case Is = "UPDATE"
                    err = UpdateTemplatesSQL2005(cmd)
                Case Is = "DELETE"
                    err = DeleteTemplatesSQL2005(cmd)
                Case Is = "DELETE-LINE"
                    err = DeleteTemplatesLineSQL2005(cmd)
                Case Is = "EMPTY-TEMPLATE"
                    err = EmptyTemplatesSQL2005(cmd)
                Case Is = "SET-AS-DEFAULT"
                    err = SetDefaultTemplatesSQL2005(cmd)
            End Select
        End If

        Try
            ' Close the connection
            cmd.Connection.Close()
        Catch ex As Exception
            If Not err.HasError Then
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ""
                    .ErrorNumber = "TACDBOR07"
                    .HasError = True
                End With
            End If
        End Try

        Return err
    End Function

    Protected Overrides Function ValidateAgainstDatabaseSql2005() As ErrorObj
        Dim err As New ErrorObj
        Return err
    End Function

    Private Function BuildOrderTemplateDataSetFromDataEntity() As ErrorObj
        Dim err As New ErrorObj
        If DE_OrderTemplates.OrderTemplates.Count = 0 Then
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = "Order Templates Object contains no items"
            Return err
        End If

        Return BuildOrderTemplateHeader(err)
    End Function

    Private Function BuildOrderTemplateHeader(ByVal err As ErrorObj) As ErrorObj
        Try
            Dim dth As DataTable = GetNewOrderTemplatesHeaderTable()
            Dim dtd As DataTable = GetNewOrderTemplatesDetailTable()

            For i As Integer = 0 To DE_OrderTemplates.OrderTemplates.Count - 1
                Dim row As DataRow = dth.NewRow
                row("TEMP_HEADER_KEY") = i
                If Not DE_OrderTemplates.OrderTemplates(i).Template_Header_ID = 0 Then
                    row("TEMPLATE_HEADER_ID") = DE_OrderTemplates.OrderTemplates(i).Template_Header_ID
                Else
                    row("TEMPLATE_HEADER_ID") = empty
                End If
                row("NAME") = DE_OrderTemplates.OrderTemplates(i).Template_Name
                row("DESCRIPTION") = DE_OrderTemplates.OrderTemplates(i).Description
                row("BUSINESS_UNIT") = DE_OrderTemplates.OrderTemplates(i).Business_Unit
                row("PARTNER") = DE_OrderTemplates.OrderTemplates(i).Partner
                row("LOGINID") = DE_OrderTemplates.OrderTemplates(i).LoginID
                row("CREATED_DATE") = DE_OrderTemplates.OrderTemplates(i).Created_Date
                row("LAST_MODIFIED_DATE") = DE_OrderTemplates.OrderTemplates(i).Last_Modified_Date
                row("LAST_USED_DATE") = DE_OrderTemplates.OrderTemplates(i).Last_Used_Date
                row("IS_DEFAULT") = DE_OrderTemplates.OrderTemplates(i).Is_Default_Template
                row("ALLOW_FF_TO_VIEW") = DE_OrderTemplates.OrderTemplates(i).AllowFFToView
                dth.Rows.Add(row)
                dtd = BuildOrderTemplateDetail(dtd, i, DE_OrderTemplates.OrderTemplates(i))
            Next

            TemplatesRecordSet = New DataSet
            TemplatesRecordSet.Tables.Add(dth)
            TemplatesRecordSet.Tables.Add(dtd)
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = "Error creating order templates dataset"
        End Try

        Return err
    End Function

    Private Function BuildOrderTemplateDetail(ByVal dtd As DataTable, _
                                                ByVal headerKey As String, _
                                                ByVal template As DEOrderTemplate) As DataTable

        If Not template.OrderTemplateItems Is Nothing Then
            For i As Integer = 0 To template.OrderTemplateItems.Count - 1
                Dim row As DataRow = dtd.NewRow
                row("TEMP_HEADER_KEY") = headerKey
                If Not template.OrderTemplateItems(i).Template_Detail_ID = 0 Then
                    row("TEMPLATE_DETAIL_ID") = template.OrderTemplateItems(i).Template_Detail_ID
                Else
                    row("TEMPLATE_DETAIL_ID") = empty
                End If
                If Not template.OrderTemplateItems(i).Template_Header_ID = 0 Then
                    row("TEMPLATE_HEADER_ID") = template.OrderTemplateItems(i).Template_Header_ID
                Else
                    row("TEMPLATE_HEADER_ID") = empty
                End If
                row("PRODUCT_CODE") = template.OrderTemplateItems(i).Product_Code
                row("QUANTITY") = template.OrderTemplateItems(i).Quantity
                row("MASTER_PRODUCT") = template.OrderTemplateItems(i).MasterProduct
                dtd.Rows.Add(row)
            Next
        End If

        Return dtd
    End Function

    Function GetNewOrderTemplatesHeaderTable() As DataTable
        Dim dth As New DataTable("OrderTemplatesHeader")
        dth.Columns.Add("TEMP_HEADER_KEY", GetType(String))
        dth.Columns.Add("TEMPLATE_HEADER_ID", GetType(String))
        dth.Columns.Add("NAME", GetType(String))
        dth.Columns.Add("DESCRIPTION", GetType(String))
        dth.Columns.Add("BUSINESS_UNIT", GetType(String))
        dth.Columns.Add("PARTNER", GetType(String))
        dth.Columns.Add("LOGINID", GetType(String))
        dth.Columns.Add("CREATED_DATE", GetType(DateTime))
        dth.Columns.Add("LAST_MODIFIED_DATE", GetType(DateTime))
        dth.Columns.Add("LAST_USED_DATE", GetType(DateTime))
        dth.Columns.Add("IS_DEFAULT", GetType(Boolean))
        dth.Columns.Add("ALLOW_FF_TO_VIEW", GetType(Boolean))
        Return dth
    End Function

    Function GetNewOrderTemplatesDetailTable() As DataTable
        Dim dtd As New DataTable("OrderTemplatesDetail")
        dtd.Columns.Add("TEMP_HEADER_KEY", GetType(String))
        dtd.Columns.Add("TEMPLATE_DETAIL_ID", GetType(String))
        dtd.Columns.Add("TEMPLATE_HEADER_ID", GetType(String))
        dtd.Columns.Add("PRODUCT_CODE", GetType(String))
        dtd.Columns.Add("QUANTITY", GetType(Decimal))
        dtd.Columns.Add("MASTER_PRODUCT", GetType(String))
        dtd.Columns.Add("NAME", GetType(String))
        Return dtd
    End Function

    Private Function WriteToDatabase(Optional ByVal opendb As Boolean = True) As ErrorObj
        Dim err As New ErrorObj
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                '-------------------------------------------------------------------
                '   The communications error maybe caused by the database not being 
                '   open so we could change to :
                '
                '   If Not err.HasError and conSystem21.State = ConnectionState.Open Then _
                '       err = WriteDataBaseSystem21()
                '   else
                '       throw err
                '   End if
                '
                '-------------------------------------------------------------------
                If Not err.HasError() Then _
                    err = WriteDataBaseSystem21()
                '
                If opendb Then System21Close()
            Case Is = SQL2005
                err = WriteDataBaseSQL2005()
        End Select
        Return err
    End Function

    Private Function WriteDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        Return err
    End Function

    Private Function WriteDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Return err
    End Function

    Private Function SelectTemplatesSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj
        Dim dth As DataTable = GetNewOrderTemplatesHeaderTable()
        Dim dtd As DataTable = GetNewOrderTemplatesDetailTable()

        Dim strSelect As New StringBuilder
        strSelect.append("SELECT * FROM tbl_order_template_header WITH (NOLOCK)  ")

        Try
            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtHeader.Rows

                'If the OrderTemplate DE contained a TemplateHeaderID then get the specific record,
                'otherwise get all records for the specified BU, Partner and LoginID.
                If row("TEMPLATE_HEADER_ID") = empty OrElse String.IsNullOrEmpty(row("TEMPLATE_HEADER_ID")) Then
                    strSelect.Append(" WHERE BUSINESS_UNIT = @BUSINESS_UNIT ")
                    strSelect.Append("   AND PARTNER = @PARTNER ")
                    strSelect.Append("   AND LOGINID = @LOGINID ")

                    If Utilities.CheckForDBNull_Boolean_DefaultFalse(row("ALLOW_FF_TO_VIEW")) Then
                        strSelect.Append("   AND ALLOW_FF_TO_VIEW = @ALLOW_FF_TO_VIEW ")
                    End If
                    cmd.CommandText = strSelect.ToString()
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar)).Value = row("BUSINESS_UNIT")
                        .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar)).Value = row("PARTNER")
                        .Add(New SqlParameter("@LOGINID", SqlDbType.NVarChar)).Value = row("LOGINID")
                        If Utilities.CheckForDBNull_Boolean_DefaultFalse(row("ALLOW_FF_TO_VIEW")) Then
                            .Add(New SqlParameter("@ALLOW_FF_TO_VIEW", SqlDbType.Bit)).Value = row("ALLOW_FF_TO_VIEW")
                        End If
                    End With
                Else
                    cmd.CommandText = strSelect.ToString() & " WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID "
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.NVarChar)).Value = row("TEMPLATE_HEADER_ID")
                    End With
                End If

                Dim dr As SqlDataReader = cmd.ExecuteReader
                If dr.HasRows Then
                    While dr.Read
                        Dim newRow As DataRow = dth.NewRow
                        newRow("TEMPLATE_HEADER_ID") = Utilities.CheckForDBNull_String(dr("TEMPLATE_HEADER_ID"))
                        newRow("NAME") = Utilities.CheckForDBNull_String(dr("NAME"))
                        newRow("DESCRIPTION") = Utilities.CheckForDBNull_String(dr("DESCRIPTION"))
                        newRow("BUSINESS_UNIT") = Utilities.CheckForDBNull_String(dr("BUSINESS_UNIT"))
                        newRow("PARTNER") = Utilities.CheckForDBNull_String(dr("PARTNER"))
                        newRow("LOGINID") = Utilities.CheckForDBNull_String(dr("LOGINID"))
                        newRow("CREATED_DATE") = Utilities.CheckForDBNull_DateTime(dr("CREATED_DATE"))
                        newRow("LAST_MODIFIED_DATE") = Utilities.CheckForDBNull_DateTime(dr("LAST_MODIFIED_DATE"))
                        newRow("LAST_USED_DATE") = Utilities.CheckForDBNull_DateTime(dr("LAST_USED_DATE"))
                        newRow("IS_DEFAULT") = Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("IS_DEFAULT"))
                        newRow("ALLOW_FF_TO_VIEW") = Utilities.CheckForDBNull_Boolean_DefaultFalse(dr("ALLOW_FF_TO_VIEW"))
                        dth.Rows.Add(newRow)
                    End While
                End If
                dr.Close()
                For Each hrow As DataRow In dth.Rows
                    If Not String.IsNullOrEmpty(hrow("TEMPLATE_HEADER_ID")) OrElse Not String.IsNullOrEmpty(hrow("NAME")) Then
                        GetTemplateDetails(cmd, hrow("TEMPLATE_HEADER_ID"), dtd, hrow("NAME"))
                    End If
                Next
            Next
            Me.ResultDataSet = New DataSet
            Me.ResultDataSet.Tables.Add(dth)
            Me.ResultDataSet.Tables.Add(dtd)
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = ""
        End Try

        Return err
    End Function

    Protected Function GetTemplateDetails(ByVal cmd As SqlCommand, ByVal templateHeaderID As Long, ByVal dtd As DataTable, ByVal templateName As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        'Const strSelectByID As String = "SELECT * FROM tbl_order_template_detail WITH (NOLOCK)  " & _
        '                            "   WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID"
        'Const strSelectByName As String = "SELECT * FROM tbl_order_template_detail WITH (NOLOCK)  " & _
        '                           "   WHERE NAME = @NAME"

        'JDW We only want to display products that are valid in the system!
        '---------------------------------------------------------------------
        Const strSelectByID As String = " SELECT otd.*, pd.*  " & _
                                        " FROM tbl_order_template_detail otd WITH (NOLOCK) " & _
                                        " INNER JOIN tbl_product pd WITH (NOLOCK) " & _
                                        " ON otd.PRODUCT_CODE = pd.PRODUCT_CODE " & _
                                        " WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID " & _
                                         " ORDER BY otd.PRODUCT_CODE "

        Const strSelectByName As String = " SELECT otd.*, pd.*  " & _
                                        " FROM tbl_order_template_detail otd WITH (NOLOCK) " & _
                                        " INNER JOIN tbl_product pd WITH (NOLOCK) " & _
                                        " ON otd.PRODUCT_CODE = pd.PRODUCT_CODE " & _
                                        "   WHERE NAME = @NAME " & _
                                         " ORDER BY otd.PRODUCT_CODE "

        Try

            '----------------------------------
            ' Try to link to details 
            ' by order template name first...
            ' then for backwards compatibility, 
            ' try by order header id
            '----------------------------------

            cmd.CommandText = strSelectByName
            With cmd.Parameters
                .Clear()
                .Add(New SqlParameter("@NAME", SqlDbType.NVarChar)).Value = templateName
            End With
            Dim dr As SqlDataReader = cmd.ExecuteReader
            If dr.HasRows Then
                While dr.Read
                    Dim newRow As DataRow = dtd.NewRow
                    newRow("TEMPLATE_DETAIL_ID") = Utilities.CheckForDBNull_String(dr("TEMPLATE_DETAIL_ID"))
                    newRow("TEMPLATE_HEADER_ID") = Utilities.CheckForDBNull_String(dr("TEMPLATE_HEADER_ID"))
                    newRow("PRODUCT_CODE") = Utilities.CheckForDBNull_String(dr("PRODUCT_CODE"))
                    newRow("QUANTITY") = Utilities.CheckForDBNull_Decimal(dr("QUANTITY"))
                    newRow("MASTER_PRODUCT") = Utilities.CheckForDBNull_String(dr("MASTER_PRODUCT"))
                    newRow("NAME") = Utilities.CheckForDBNull_String(dr("NAME"))
                    dtd.Rows.Add(newRow)
                End While
                dr.Close()
            Else
                dr.Close()
                cmd = New SqlCommand(strSelectByID, conSql2005)

                With cmd.Parameters
                    .Clear()
                    .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.NVarChar)).Value = templateHeaderID
                End With
                dr = cmd.ExecuteReader
                If dr.HasRows Then
                    While dr.Read
                        Dim newRow As DataRow = dtd.NewRow
                        newRow("TEMPLATE_DETAIL_ID") = Utilities.CheckForDBNull_String(dr("TEMPLATE_DETAIL_ID"))
                        newRow("TEMPLATE_HEADER_ID") = Utilities.CheckForDBNull_String(dr("TEMPLATE_HEADER_ID"))
                        newRow("PRODUCT_CODE") = Utilities.CheckForDBNull_String(dr("PRODUCT_CODE"))
                        newRow("QUANTITY") = Utilities.CheckForDBNull_Decimal(dr("QUANTITY"))
                        newRow("MASTER_PRODUCT") = Utilities.CheckForDBNull_String(dr("MASTER_PRODUCT"))
                        newRow("NAME") = Utilities.CheckForDBNull_String(dr("NAME"))
                        dtd.Rows.Add(newRow)
                    End While
                End If
                dr.Close()

            End If
            
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ""
                .ErrorNumber = "TACDBOR06"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function InsertTemplatesSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        Const strInsert As String = "INSERT INTO tbl_order_template_header " & _
                                    "       (NAME, DESCRIPTION, BUSINESS_UNIT, " & _
                                    "       PARTNER, LOGINID, CREATED_DATE, " & _
                                    "       LAST_MODIFIED_DATE, LAST_USED_DATE, " & _
                                    "       IS_DEFAULT, ALLOW_FF_TO_VIEW) " & _
                                    "   VALUES( @NAME,@DESCRIPTION,@BUSINESS_UNIT, " & _
                                    "           @PARTNER, @LOGINID, @CREATED_DATE, " & _
                                    "           @LAST_MODIFIED_DATE, @LAST_USED_DATE, " & _
                                    "           @IS_DEFAULT, @ALLOW_FF_TO_VIEW) "

        Const strSelect As String = "SELECT * FROM tbl_order_template_header WITH (NOLOCK)  " & _
                                    "WHERE  NAME = @NAME " & _
                                    "   AND DESCRIPTION = @DESCRIPTION " & _
                                    "   AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                    "   AND PARTNER = @PARTNER " & _
                                    "   AND LOGINID = @LOGINID " & _
                                    "   AND CREATED_DATE = @CREATED_DATE " & _
                                    "   AND LAST_MODIFIED_DATE = @LAST_MODIFIED_DATE " & _
                                    "   AND LAST_USED_DATE = @LAST_USED_DATE "

        Try

            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtHeader.Rows
                cmd.CommandText = strInsert

                'Add the corresponding characters from the current row to the command
                With cmd.Parameters
                    .Clear()
                    .Add(New SqlParameter("@NAME", SqlDbType.NVarChar)).Value = row("NAME")
                    .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = row("DESCRIPTION")
                    .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar)).Value = row("BUSINESS_UNIT")
                    .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar)).Value = row("PARTNER")
                    .Add(New SqlParameter("@LOGINID", SqlDbType.NVarChar)).Value = row("LOGINID")
                    .Add(New SqlParameter("@CREATED_DATE", SqlDbType.DateTime)).Value = CDate(row("CREATED_DATE"))
                    .Add(New SqlParameter("@LAST_MODIFIED_DATE", SqlDbType.DateTime)).Value = CDate(row("LAST_MODIFIED_DATE"))
                    .Add(New SqlParameter("@LAST_USED_DATE", SqlDbType.DateTime)).Value = CDate(row("LAST_USED_DATE"))
                    .Add(New SqlParameter("@IS_DEFAULT", SqlDbType.Bit)).Value = CBool(row("IS_DEFAULT"))
                    .Add(New SqlParameter("@ALLOW_FF_TO_VIEW", SqlDbType.Bit)).Value = CBool(row("ALLOW_FF_TO_VIEW"))
                End With

                'Excute the insert
                cmd.ExecuteNonQuery()

                'Retrieve the Header ID of the newly inserted record
                cmd.CommandText = strSelect
                Dim templateHeaderID As Long = 0
                Dim dr As SqlDataReader = cmd.ExecuteReader
                If dr.HasRows Then
                    While dr.Read
                        templateHeaderID = CType(dr("TEMPLATE_HEADER_ID"), Long)
                    End While
                End If
                dr.Close()

                If templateHeaderID > 0 Then
                    'If the Header ID was successfully retrieved, add the detail
                    'lines that correspond to the HEADER we have just added
                    err = AddDetails(cmd, row("TEMP_HEADER_KEY"), templateHeaderID, row("NAME").ToString)
                End If
            Next

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ""
                .ErrorNumber = "TACDBOR06"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AddDetails(ByVal cmd As SqlCommand, ByVal HeaderKey As String, ByVal templateHeaderID As Long, ByVal templateName As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        Const strInsert As String = "INSERT INTO tbl_order_template_detail " & _
                                    "       (TEMPLATE_HEADER_ID, " & _
                                    "       PRODUCT_CODE, QUANTITY, MASTER_PRODUCT,NAME) " & _
                                    "   VALUES( @TEMPLATE_HEADER_ID, " & _
                                    "           @PRODUCT_CODE, @QUANTITY, @MASTER_PRODUCT, @NAME) "

        Try
            'Change the command text
            cmd.CommandText = strInsert

            'Loop through each row in the Detail Table
            For Each row As DataRow In DtDetail.Rows

                'If the row corresponds to the Header then add the parameters
                If row("TEMP_HEADER_KEY") = HeaderKey Then
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.BigInt)).Value = templateHeaderID
                        .Add(New SqlParameter("@PRODUCT_CODE", SqlDbType.NVarChar)).Value = row("PRODUCT_CODE")
                        .Add(New SqlParameter("@QUANTITY", SqlDbType.Decimal)).Value = CDec(row("QUANTITY"))
                        .Add(New SqlParameter("@MASTER_PRODUCT", SqlDbType.NVarChar)).Value = row("MASTER_PRODUCT")
                        .Add(New SqlParameter("@NAME", SqlDbType.NVarChar)).Value = templateName
                    End With

                    'Insert the detail record
                    cmd.ExecuteNonQuery()
                End If

            Next

        Catch ex As Exception

            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ""
                .ErrorNumber = "TACDBOR06"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function UpdateTemplatesSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj

        Const strUpdate As String = "UPDATE tbl_order_template_header " & _
                                    "   SET NAME = @NAME, " & _
                                    "       DESCRIPTION = @DESCRIPTION, " & _
                                    "       BUSINESS_UNIT = @BUSINESS_UNIT, " & _
                                    "       PARTNER = @PARTNER, " & _
                                    "       LOGINID = @LOGINID, " & _
                                    "       CREATED_DATE = @CREATED_DATE, " & _
                                    "       LAST_USED_DATE = @LAST_USED_DATE, " & _
                                    "       LAST_MODIFIED_DATE = @LAST_MODIFIED_DATE, " & _
                                    "       IS_DEFAULT = @IS_DEFAULT " & _
                                    " WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID "

        Try
            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtHeader.Rows

                cmd.CommandText = strUpdate
                With cmd.Parameters
                    .Clear()
                    .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.NVarChar)).Value = CType(row("TEMPLATE_HEADER_ID"), Long)
                    .Add(New SqlParameter("@NAME", SqlDbType.NVarChar)).Value = row("NAME")
                    .Add(New SqlParameter("@DESCRIPTION", SqlDbType.NVarChar)).Value = row("DESCRIPTION")
                    .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar)).Value = row("BUSINESS_UNIT")
                    .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar)).Value = row("PARTNER")
                    .Add(New SqlParameter("@LOGINID", SqlDbType.NVarChar)).Value = row("LOGINID")
                    .Add(New SqlParameter("@CREATED_DATE", SqlDbType.DateTime)).Value = CDate(row("CREATED_DATE"))
                    .Add(New SqlParameter("@LAST_MODIFIED_DATE", SqlDbType.DateTime)).Value = CDate(row("LAST_MODIFIED_DATE"))
                    .Add(New SqlParameter("@LAST_USED_DATE", SqlDbType.DateTime)).Value = CDate(row("LAST_USED_DATE"))
                    .Add(New SqlParameter("@IS_DEFAULT", SqlDbType.Bit)).Value = CBool(row("IS_DEFAULT"))
                End With

                cmd.ExecuteNonQuery()
                err = DeleteTemplateDetails(cmd, CType(row("TEMPLATE_HEADER_ID"), Long), Utilities.CheckForDBNull_String(row("NAME")))
                If Not err.HasError Then
                    err = AddDetails(cmd, row("TEMP_HEADER_KEY"), CType(row("TEMPLATE_HEADER_ID"), Long), row("NAME").ToString)
                End If
            Next

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = ""
        End Try

        Return err
    End Function

    Private Function SetDefaultTemplatesSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj

        Const strSetDefault As String = "UPDATE tbl_order_template_header " & _
                                        "   SET IS_DEFAULT = 'False' " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "   AND PARTNER = @PARTNER " & _
                                        "   AND LOGINID = @LOGINID ;" & _
                                        " " & _
                                        "UPDATE tbl_order_template_header " & _
                                        "   SET IS_DEFAULT = 'True' " & _
                                        " WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID "


        Try
            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtHeader.Rows
                If row("TEMPLATE_HEADER_ID") = empty OrElse String.IsNullOrEmpty(row("TEMPLATE_HEADER_ID")) Then
                    'If we have not been supplied with a header ID we cannot set the default 
                    'template value so do nothing - could return error??
                Else
                    cmd.CommandText = strSetDefault
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.NVarChar)).Value = CType(row("TEMPLATE_HEADER_ID"), Long)
                        .Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar)).Value = row("BUSINESS_UNIT")
                        .Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar)).Value = row("PARTNER")
                        .Add(New SqlParameter("@LOGINID", SqlDbType.NVarChar)).Value = row("LOGINID")
                    End With

                    cmd.ExecuteNonQuery()
                End If
                
            Next

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = ""
        End Try

        Return err
    End Function

    Private Function EmptyTemplatesSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj

        Try
            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtHeader.Rows
                If row("TEMPLATE_HEADER_ID") = empty OrElse String.IsNullOrEmpty(row("TEMPLATE_HEADER_ID")) Then
                    'If we have not been supplied with a header ID we cannot set the default 
                    'template value so do nothing - could return error??
                Else
                    err = DeleteTemplateDetails(cmd, CType(row("TEMPLATE_HEADER_ID"), Long), Utilities.CheckForDBNull_String(row("NAME")))
                End If
            Next

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = ""
        End Try

        Return err
    End Function

    Private Function DeleteTemplatesLineSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj

        Try
            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtDetail.Rows
                If row("TEMPLATE_DETAIL_ID") = empty OrElse String.IsNullOrEmpty(row("TEMPLATE_DETAIL_ID")) Then
                    'If we have not been supplied with a header ID we cannot set the default 
                    'template value so do nothing - could return error??
                Else
                    err = DeleteTemplateDetail(cmd, CType(row("TEMPLATE_DETAIL_ID"), Long))
                End If
            Next

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = ""
        End Try

        Return err
    End Function

    Private Function DeleteTemplatesSQL2005(ByVal cmd As SqlCommand) As ErrorObj
        Dim err As New ErrorObj

        Const strSetDefault As String = "DELETE FROM tbl_order_template_header " & _
                                        " WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID; " & _
                                        " " & _
                                        " DELETE FROM tbl_order_template_detail " & _
                                        " WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID; "

        Try
            'Loop through each row in the InputHeader file
            For Each row As DataRow In DtHeader.Rows
                If row("TEMPLATE_HEADER_ID") = empty OrElse String.IsNullOrEmpty(row("TEMPLATE_HEADER_ID")) Then
                    'If we have not been supplied with a header ID we cannot set the default 
                    'template value so do nothing - could return error??
                Else
                    cmd.CommandText = strSetDefault
                    With cmd.Parameters
                        .Clear()
                        .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.NVarChar)).Value = CType(row("TEMPLATE_HEADER_ID"), Long)
                    End With

                    cmd.ExecuteNonQuery()
                End If

            Next

        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = ""
            err.ErrorMessage = ""
        End Try

        Return err
    End Function

    Protected Function DeleteTemplateDetails(ByVal cmd As SqlCommand, ByVal templateHeaderID As Long, ByVal templateName As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        Const strDeleteByName As String = "DELETE FROM tbl_order_template_detail " & _
                                    "   WHERE NAME = @NAME"

        Const strDeleteById As String = "DELETE FROM tbl_order_template_detail " & _
                                    "   WHERE TEMPLATE_HEADER_ID = @TEMPLATE_HEADER_ID"

        Try
            '------------------------------------------------
            ' Link to details by Template Name first then try 
            ' by ID for backwards compatibility
            '------------------------------------------------
            If templateName <> String.Empty Then
                cmd = New SqlCommand(strDeleteByName, conSql2005)
                With cmd.Parameters
                    .Clear()
                    .Add(New SqlParameter("@NAME", SqlDbType.NVarChar)).Value = templateName
                End With
                cmd.ExecuteNonQuery()

            End If
            If templateHeaderID <> 0 Then
                cmd.CommandText = strDeleteById
                With cmd.Parameters
                    .Clear()
                    .Add(New SqlParameter("@TEMPLATE_HEADER_ID", SqlDbType.NVarChar)).Value = templateHeaderID
                End With
                cmd.ExecuteNonQuery()

            End If
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ""
                .ErrorNumber = "TACDBOR06"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function DeleteTemplateDetail(ByVal cmd As SqlCommand, ByVal templateDetailID As Long) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------
        Const strDelete As String = "DELETE FROM tbl_order_template_detail " & _
                                    "   WHERE TEMPLATE_DETAIL_ID = @TEMPLATE_DETAIL_ID"

        Try
            'Change the command text
            cmd.CommandText = strDelete
            With cmd.Parameters
                .Clear()
                .Add(New SqlParameter("@TEMPLATE_DETAIL_ID", SqlDbType.NVarChar)).Value = templateDetailID
            End With
            cmd.ExecuteNonQuery()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ""
                .ErrorNumber = "TACDBOR06"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function ReadTemplateOrder() As ErrorObj
        Return New ErrorObj
    End Function


End Class
