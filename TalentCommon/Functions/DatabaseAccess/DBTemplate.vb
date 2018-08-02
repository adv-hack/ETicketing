Imports IBM.Data.DB2.iSeries

<Serializable()>
Public Class DBTemplate
    Inherits DBAccess

#Region "Class Level Fields"

    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Public Properties"

    Public Property DeTemp() As DETemplate
    Public Property ErrorMessage As String = "Error during database Access"

#End Region

#Region "Protected Methods"

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Return New ErrorObj
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Return New ErrorObj
    End Function

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = "GetTemplateOverrideList" : err = AccessDatabaseTM001S1()
            Case Is = "CreateTemplateOverride" : err = AccessDatabaseTM001S1()
            Case Is = "GetTemplateOverrideCriterias" : err = AccessDatabaseTM001S2()
            Case Is = "UpdateTemplateOverride" : err = AccessDatabaseTM001S1()
            Case Is = "DeleteTemplateOverride" : err = AccessDatabaseTM001S1()
        End Select
        Return err
    End Function

#End Region

#Region "Private Functions"
    Private Function AccessDatabaseTM001S1() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet
        TemplateOverrideDataSet(ResultDataSet)
        Try
            If DeTemp.Mode = GlobalConstants.TEMPLATE_OVERRIDE_READ_MODE OrElse DeTemp.Mode = GlobalConstants.TEMPLATE_OVERRIDE_DELETE_MODE Then
                CallTM001S1()
            ElseIf DeTemp.Mode = GlobalConstants.TEMPLATE_OVERRIDE_CREATE_MODE OrElse DeTemp.Mode = GlobalConstants.TEMPLATE_OVERRIDE_UPDATE_MODE Then
                Dim callCount As Decimal = 1
                Dim templateHeaderId As Decimal = 0
                For Each criteria As TemplateOverrideCriteria In DeTemp.TemplateOverrideCriterias
                    CallTM001S1(criteria.CriteriaType, criteria.CriteriaValue, callCount, templateHeaderId)
                    callCount = callCount + 1
                Next
            End If

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ErrorMessage
                .ErrorNumber = "DBTemplate-TM001S1"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function AccessDatabaseTM001S2() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet
        TemplateOverrideCriteriaDataSet(ResultDataSet)
        Try
            CallTM001S2()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ErrorMessage
                .ErrorNumber = "DBTemplate-TM001S2"
                .HasError = True
            End With
        End Try
        Return err
    End Function
#End Region

#Region "Private Methods"

    ''' <summary>
    '''  Create template override And get New template override list from the TM001 And TM002 files based on the given parameters
    ''' </summary>
    ''' <param name="CriteriaType">Template override criteria type</param>
    ''' <param name="CriteriaValue">TTemplate override criteria value</param>
    ''' <param name="CallCount">Call count for create mode</param>
    Private Sub CallTM001S1(Optional ByVal CriteriaType As String = "", Optional ByVal CriteriaValue As String = "", Optional ByVal CallCount As Decimal = 0.0, Optional ByRef TemplateHeaderId As Decimal = 0.0)
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL TM001S1(@ErrorCode, @Source, @PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10,@PARAM11,@PARAM12,@PARAM13,@PARAM14)"
        _cmd.CommandType = CommandType.Text

        Dim pBusinessUnit As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter
        Dim pMode As iDB2Parameter
        Dim pTemplateOverrideId As iDB2Parameter
        Dim pTemplateDescription As iDB2Parameter
        Dim pBoxOfficeUser As iDB2Parameter
        Dim pSaleConfirmationEmailId As iDB2Parameter
        Dim pSaleConfirmationEmailDesc As iDB2Parameter
        Dim pQATemplateId As iDB2Parameter
        Dim pQATemplateDesc As iDB2Parameter
        Dim pDataCaptureTemplateId As iDB2Parameter
        Dim pDataCaptureTemplateDesc As iDB2Parameter
        Dim pAutoExpandQAndA As iDB2Parameter
        Dim pCriteriaType As iDB2Parameter
        Dim pCriteriaValue As iDB2Parameter
        Dim pCallCount As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pBusinessUnit = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 20)
        pMode = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pTemplateOverrideId = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 13)
        pTemplateDescription = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 50)
        pBoxOfficeUser = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 10)
        pSaleConfirmationEmailId = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Decimal, 13)
        pSaleConfirmationEmailDesc = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 100)
        pQATemplateId = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Decimal, 13)
        pQATemplateDesc = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 50)
        pDataCaptureTemplateId = _cmd.Parameters.Add(Param9, iDB2DbType.iDB2Decimal, 13)
        pDataCaptureTemplateDesc = _cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 50)
        pAutoExpandQAndA = _cmd.Parameters.Add(Param11, iDB2DbType.iDB2Char, 1)
        pCriteriaType = _cmd.Parameters.Add(Param12, iDB2DbType.iDB2Char, 2)
        pCriteriaValue = _cmd.Parameters.Add(Param13, iDB2DbType.iDB2Char, 13)
        pCallCount = _cmd.Parameters.Add(Param14, iDB2DbType.iDB2Decimal, 13)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pTemplateOverrideId.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = DeTemp.Source
        pBusinessUnit.Value = DeTemp.BusinessUnit
        pMode.Value = DeTemp.Mode
        pTemplateOverrideId.Value = IIf(TemplateHeaderId > 0, TemplateHeaderId, DeTemp.TemplateOverrideId)
        pTemplateDescription.Value = IIf(DeTemp.Description Is Nothing, String.Empty, DeTemp.Description)
        pBoxOfficeUser.Value = IIf(DeTemp.BoxOfficeUser Is Nothing, String.Empty, DeTemp.BoxOfficeUser)
        pSaleConfirmationEmailId.Value = DeTemp.SaleConfirmationEmailId
        pSaleConfirmationEmailDesc.Value = IIf(DeTemp.SaleConfirmationEmailDescription Is Nothing, String.Empty, DeTemp.SaleConfirmationEmailDescription)
        pQATemplateId.Value = DeTemp.QAndATemplateID
        pQATemplateDesc.Value = IIf(DeTemp.QAndATemplateDescription Is Nothing, String.Empty, DeTemp.QAndATemplateDescription)
        pDataCaptureTemplateId.Value = DeTemp.DataCaptureTemplateId
        pDataCaptureTemplateDesc.Value = IIf(DeTemp.DataCaptureTemplateDescription Is Nothing, String.Empty, DeTemp.DataCaptureTemplateDescription)
        pAutoExpandQAndA.Value = DeTemp.AutoExpandQAndA
        pCriteriaType.Value = CriteriaType
        pCriteriaValue.Value = CriteriaValue
        pCallCount.Value = CallCount

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.TableMappings.Add("Table", "TemplateOverrideHeader")
        _cmdAdapter.TableMappings.Add("Table1", "TemplateOverrideCriterias")
        _cmdAdapter.SelectCommand = _cmd
        ResultDataSet.Tables("TemplateOverrideHeader").Rows.Clear()
        ResultDataSet.Tables("TemplateOverrideCriterias").Rows.Clear()
        _cmdAdapter.Fill(ResultDataSet)
        TemplateHeaderId = pTemplateOverrideId.Value

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    '''  Get template override criterias to bind in dropdown
    ''' </summary>
    Private Sub CallTM001S2()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL TM001S2(@ErrorCode, @Source)"
        _cmd.CommandType = CommandType.Text

        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = DeTemp.Source

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.TableMappings.Add("Table", "TicketingOverrideCriteria")
        _cmdAdapter.TableMappings.Add("Table1", "PackageOverrideCriteria")
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub
    Private Sub TemplateOverrideDataSet(ByRef resultdataSet As DataSet)
        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        resultdataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtTemplatOverrideHeader As New DataTable("TemplateOverrideHeader")
        resultdataSet.Tables.Add(DtTemplatOverrideHeader)

        Dim DtTemplateOverrideCriterias As New DataTable("TemplateOverrideCriterias")
        resultdataSet.Tables.Add(DtTemplateOverrideCriterias)
    End Sub
    Private Sub TemplateOverrideCriteriaDataSet(ByRef resultdataSet As DataSet)
        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        resultdataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtTicketingOverrideCriteria As New DataTable("TicketingOverrideCriteria")
        resultdataSet.Tables.Add(DtTicketingOverrideCriteria)

        Dim DtPackageOverrideCriteria As New DataTable("PackageOverrideCriteria")
        resultdataSet.Tables.Add(DtPackageOverrideCriteria)
    End Sub
#End Region

End Class
