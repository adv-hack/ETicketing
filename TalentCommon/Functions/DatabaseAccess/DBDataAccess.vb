Imports System.Data.SqlClient
''' <summary>
''' Provides the functionality to execute the command object against the given connection string
''' by using DBAccess Class
''' </summary>
Public Class DBDataAccess
    Inherits DBAccess

#Region "Class Level Fields"
    Private Const CLASSNAME As String = "DBDataAccess"
    Private _commandElement As DESQLCommand
    Const ERROR_COMMANDTYPE As String = "Command type is empty or Invalid type"
    Const ERROR_COMMANDTEXT As String = "Command text is empty or Having invalid characters"
    Const ERROR_COMMANDPARAMETERNAME As String = "Parameter name is empty or Having invalid characters"
    Const ERROR_COMMANDPARAMETERTYPE As String = "Parameter type is empty or Having invalid characters"
    Const ERROR_COMMANDPARAMETERTYPEVALUE As String = "Parameter value is not matching with type or Having invalid characters"
#End Region

#Region "Properties"
    ''' <summary>
    ''' Sets the command element of type DESQLCommand
    ''' </summary>
    ''' <value>The command element.</value>
    Public WriteOnly Property CommandElement() As DESQLCommand
        Set(ByVal value As DESQLCommand)
            _commandElement = value
        End Set
    End Property
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Opens the connection and create and returns the transaction object
    ''' </summary>
    ''' <param name="err">The error object as ref</param>
    ''' <param name="givenIsolationLevel">The given isolation level.</param>
    ''' <returns>SQLTransaction instance</returns>
    Public Function BeginTransaction(ByVal destinationDatabase As DestinationDatabase, ByRef err As ErrorObj, Optional ByVal givenIsolationLevel As IsolationLevel = Nothing) As SqlTransaction
        Dim SqlTrans As SqlTransaction = Nothing
        err = ConnectionByDestinationDBOpen(destinationDatabase)
        If Not err.HasError Then
            If (givenIsolationLevel = Nothing) Then
                SqlTrans = conSql2005.BeginTransaction()
            Else
                SqlTrans = conSql2005.BeginTransaction(givenIsolationLevel)
            End If
        Else
            SqlTrans = Nothing
        End If
        Return SqlTrans
    End Function

    ''' <summary>
    ''' Ends the transaction and disposes any unhandled transaction object
    ''' </summary>
    ''' <param name="err">The error object as ref</param>
    ''' <param name="givenTransaction">The given transaction.</param>
    Public Sub EndTransaction(ByVal destinationDatabase As DestinationDatabase, ByRef err As ErrorObj, ByVal givenTransaction As SqlTransaction)
        If Not (givenTransaction.Connection Is Nothing) Then
            givenTransaction.Dispose()
            givenTransaction = Nothing
        End If
        err = ConnectionByDestinationDBClose(destinationDatabase)
    End Sub

    ''' <summary>
    ''' Ends the transaction and disposes any unhandled transaction object and closes the reader object
    ''' </summary>
    ''' <param name="err">The error object as ref</param>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <param name="readerToClose">The reader to close.</param>
    Public Sub EndTransaction(ByVal destinationDatabase As DestinationDatabase, ByRef err As ErrorObj, ByVal givenTransaction As SqlTransaction, ByVal readerToClose As SqlDataReader)
        If Not (givenTransaction.Connection Is Nothing) Then
            givenTransaction.Dispose()
            givenTransaction = Nothing
        End If
        If Not (readerToClose Is Nothing) Then
            If Not (readerToClose.IsClosed) Then
                readerToClose.Close()
            End If
            readerToClose.Dispose()
        End If
        err = ConnectionByDestinationDBClose(destinationDatabase)
    End Sub

    ''' <summary>
    ''' Execute the command obejct with transaction
    ''' </summary>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <returns>Error Object</returns>
    Public Function AccessWithTransaction(ByVal givenTransaction As SqlTransaction) As ErrorObj
        Dim err As New ErrorObj
        Dim SqlCommandEntity As New SqlCommand
        SqlCommandEntity.Transaction = givenTransaction
        SqlCommandEntity.Connection = givenTransaction.Connection
        err = PrepareCommandElement(SqlCommandEntity)
        If (Not (err.HasError)) Then
            Try
                Select Case _commandElement.CommandExecutionType
                    Case CommandExecution.ExecuteDataSet
                        Me.ResultDataSet = ExecuteDataSet(SqlCommandEntity)
                    Case CommandExecution.ExecuteNonQuery
                        Me.ResultDataSet = ExecuteNonQuery(SqlCommandEntity)
                    Case CommandExecution.ExecuteReader
                    Case CommandExecution.ExecuteScalar
                End Select
            Catch sqlEx As SqlException
                err.HasError = True
                err.ErrorMessage = sqlEx.Message & "; Err Code:" & sqlEx.ErrorCode & "; Line:" & sqlEx.LineNumber & "; Name:" & sqlEx.Procedure
            Catch ex As Exception
                err.HasError = True
                err.ErrorMessage = ex.Message
            End Try
        End If
        Return err
    End Function

#Region "Obsolete Methods"
    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Opens the connection and create and returns the transaction object
    ''' </summary>
    ''' <param name="err">The error object as ref</param>
    ''' <param name="givenIsolationLevel">The given isolation level.</param>
    ''' <returns>SQLTransaction instance</returns>
    Public Function BeginTransaction(ByRef err As ErrorObj, Optional ByVal givenIsolationLevel As IsolationLevel = Nothing) As SqlTransaction
        Dim SqlTrans As SqlTransaction = Nothing
        err = Sql2005Open()
        If Not err.HasError Then
            If (givenIsolationLevel = Nothing) Then
                SqlTrans = conSql2005.BeginTransaction()
            Else
                SqlTrans = conSql2005.BeginTransaction(givenIsolationLevel)
            End If
        Else
            SqlTrans = Nothing
        End If
        Return SqlTrans
    End Function

    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Ends the transaction and disposes any unhandled transaction object
    ''' </summary>
    ''' <param name="err">The error object as ref</param>
    ''' <param name="givenTransaction">The given transaction.</param>
    Public Sub EndTransaction(ByRef err As ErrorObj, ByVal givenTransaction As SqlTransaction)
        If Not (givenTransaction.Connection Is Nothing) Then
            givenTransaction.Dispose()
            givenTransaction = Nothing
        End If
        err = Sql2005Close()
    End Sub

    ''' <summary>
    ''' OBSOLETE METHOD DO NOT USE THIS
    ''' Ends the transaction and disposes any unhandled transaction object and closes the reader object
    ''' </summary>
    ''' <param name="err">The error object as ref</param>
    ''' <param name="givenTransaction">The given transaction.</param>
    ''' <param name="readerToClose">The reader to close.</param>
    Public Sub EndTransaction(ByRef err As ErrorObj, ByVal givenTransaction As SqlTransaction, ByVal readerToClose As SqlDataReader)
        If Not (givenTransaction.Connection Is Nothing) Then
            givenTransaction.Dispose()
            givenTransaction = Nothing
        End If
        If Not (readerToClose Is Nothing) Then
            If Not (readerToClose.IsClosed) Then
                readerToClose.Close()
            End If
            readerToClose.Dispose()
        End If
        err = Sql2005Close()
    End Sub

#End Region

#End Region

#Region "Protected Methods"
    ''' <summary>
    ''' Access the data base SQL 2005
    ''' This is called by DBAccess Class
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Dim SqlCommandEntity As New SqlCommand
        SqlCommandEntity.Connection = conSql2005
        Try
            err = PrepareCommandElement(SqlCommandEntity)
            If (Not (err.HasError)) Then

                Select Case _commandElement.CommandExecutionType
                    Case CommandExecution.ExecuteDataSet
                        Me.ResultDataSet = ExecuteDataSet(SqlCommandEntity)
                    Case CommandExecution.ExecuteNonQuery
                        Me.ResultDataSet = ExecuteNonQuery(SqlCommandEntity)
                    Case CommandExecution.ExecuteReader
                    Case CommandExecution.ExecuteScalar
                End Select
            End If
        Catch ex As SqlException
            err.HasError = True
            err.ErrorMessage = ex.Message
            If _commandElement.CommandType = CommandType.StoredProcedure Then
                err.ErrorMessage = err.ErrorMessage + " (Server=" + ex.Server.ToString + ", Procedure=" + ex.Procedure.ToString + ", LineNumber=" + ex.LineNumber.ToString + ")"
            End If
            TalentLogger.Logging(CLASSNAME, "AccessDataBaseSQL2005", "SQL Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        Catch ex As Exception
            err.HasError = True
            err.ErrorMessage = ex.Message
            TalentLogger.Logging(CLASSNAME, "AccessDataBaseSQL2005", "Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try

        Return err
    End Function

    ''' <summary>
    ''' Access the data base TalentAdmin
    ''' This is called by DBAccess Class
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function AccessDataBaseTalentAdmin() As ErrorObj
        Dim err As New ErrorObj
        Dim SqlCommandEntity As New SqlCommand
        SqlCommandEntity.Connection = conSql2005
        Try
            err = PrepareCommandElement(SqlCommandEntity)
            If (Not (err.HasError)) Then
                Select Case _commandElement.CommandExecutionType
                    Case CommandExecution.ExecuteDataSet
                        Me.ResultDataSet = ExecuteDataSet(SqlCommandEntity)
                    Case CommandExecution.ExecuteNonQuery
                        Me.ResultDataSet = ExecuteNonQuery(SqlCommandEntity)
                    Case CommandExecution.ExecuteReader
                    Case CommandExecution.ExecuteScalar
                End Select
            End If
        Catch ex As SqlException
            err.HasError = True
            err.ErrorMessage = ex.Message
            If _commandElement.CommandType = CommandType.StoredProcedure Then
                err.ErrorMessage = err.ErrorMessage + " (Server=" + ex.Server.ToString + ", Procedure=" + ex.Procedure.ToString + ", LineNumber=" + ex.LineNumber.ToString + ")"
            End If
            TalentLogger.Logging(CLASSNAME, "AccessDataBaseTalentAdmin", "SQL Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        Catch ex As Exception
            err.HasError = True
            err.ErrorMessage = ex.Message
            TalentLogger.Logging(CLASSNAME, "AccessDataBaseTalentAdmin", "Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try

        Return err
    End Function

    ''' <summary>
    ''' Access the data base TalentAdmin
    ''' This is called by DBAccess Class
    ''' </summary>
    ''' <returns></returns>
    Protected Overrides Function AccessDataBaseTalentDefinitions() As ErrorObj
        Dim err As New ErrorObj
        Dim SqlCommandEntity As New SqlCommand
        SqlCommandEntity.Connection = conSql2005
        Try
            err = PrepareCommandElement(SqlCommandEntity)
            If (Not (err.HasError)) Then
                Select Case _commandElement.CommandExecutionType
                    Case CommandExecution.ExecuteDataSet
                        Me.ResultDataSet = ExecuteDataSet(SqlCommandEntity)
                    Case CommandExecution.ExecuteNonQuery
                        Me.ResultDataSet = ExecuteNonQuery(SqlCommandEntity)
                    Case CommandExecution.ExecuteReader
                    Case CommandExecution.ExecuteScalar
                End Select
            End If
        Catch ex As SqlException
            err.HasError = True
            err.ErrorMessage = ex.Message
            If _commandElement.CommandType = CommandType.StoredProcedure Then
                err.ErrorMessage = err.ErrorMessage + " (Server=" + ex.Server.ToString + ", Procedure=" + ex.Procedure.ToString + ", LineNumber=" + ex.LineNumber.ToString + ")"
            End If
            TalentLogger.Logging(CLASSNAME, "AccessDataBaseTalentDefinitions", "SQL Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        Catch ex As Exception
            err.HasError = True
            err.ErrorMessage = ex.Message
            TalentLogger.Logging(CLASSNAME, "AccessDataBaseTalentDefinitions", "Exception Occured" & GetStackFrameDetails(), err, ex, LogTypeConstants.TCDBDATAOBJECTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try

        Return err
    End Function

#End Region

#Region "Private Methods"

    Private Function ConnectionByDestinationDBOpen(ByVal destinationDatabase As DestinationDatabase) As ErrorObj
        Dim err As New ErrorObj
        If destinationDatabase = Common.DestinationDatabase.SQL2005 Then
            err = Sql2005Open()
        ElseIf destinationDatabase = Common.DestinationDatabase.TALENT_ADMIN Then
            err = TalentAdminOpen()
        ElseIf destinationDatabase = Common.DestinationDatabase.TALENT_DEFINITION Then
            err = TalentDefinitionsOpen()
        End If
        Return err
    End Function

    Private Function ConnectionByDestinationDBClose(ByVal destinationDatabase As DestinationDatabase) As ErrorObj
        Dim err As New ErrorObj
        If destinationDatabase = Common.DestinationDatabase.SQL2005 Then
            err = Sql2005Close()
        ElseIf destinationDatabase = Common.DestinationDatabase.TALENT_ADMIN Then
            err = TalentAdminClose()
        ElseIf destinationDatabase = Common.DestinationDatabase.TALENT_DEFINITION Then
            err = TalentDefinitionsClose()
        End If
        Return err
    End Function

    Private Function GetStackFrameDetails() As String
        Dim loggingContent As String = String.Empty
        Try
            Dim stTrace As New StackTrace()
            loggingContent = stTrace.ToString
        Catch ex As Exception

        End Try
        Return loggingContent
    End Function

    ''' <summary>
    ''' Executes the command object using SQLDataAdapter to fill the dataset and return
    ''' </summary>
    ''' <param name="commandToExecute">The command to execute.</param>
    ''' <returns>DataSet</returns>
    Private Function ExecuteDataSet(ByVal commandToExecute As SqlCommand) As DataSet
        Dim outputDataSet As DataSet = Nothing
        Dim sqlAdapater As New SqlDataAdapter
        Using SqlDataAdapterExecuter As New SqlDataAdapter(commandToExecute)
            outputDataSet = New DataSet()
            SqlDataAdapterExecuter.Fill(outputDataSet)
        End Using
        Return outputDataSet
    End Function

    ''' <summary>
    ''' Executes the command object using executenonquery method and 
    ''' returns a dataset with a table with single column and row ROWS_AFFECTED
    ''' </summary>
    ''' <param name="commandToExecute">The command to execute.</param>
    ''' <returns>A dataset with a table with single column and row ROWS_AFFECTED</returns>
    Private Function ExecuteNonQuery(ByVal commandToExecute As SqlCommand) As DataSet
        Dim outputDataSet As DataSet = Nothing
        Dim rowsAffected As Integer = commandToExecute.ExecuteNonQuery()
        Dim tempDataTable As New DataTable
        tempDataTable.Columns.Add("ROWS_AFFECTED", GetType(String))
        Dim tempDataRow As DataRow = Nothing
        tempDataRow = tempDataTable.NewRow
        tempDataRow("ROWS_AFFECTED") = rowsAffected
        tempDataTable.Rows.Add(tempDataRow)
        outputDataSet = New DataSet()
        outputDataSet.Tables.Add(tempDataTable)
        tempDataTable = Nothing
        Return outputDataSet
    End Function

    ''' <summary>
    ''' Prepares the command element
    ''' Assign the command instance properties from DESQLCommand instance
    ''' </summary>
    ''' <param name="givenSqlCommand">The given SQL command.</param>
    ''' <returns>ErrorObj</returns>
    Private Function PrepareCommandElement(ByVal givenSqlCommand As SqlCommand) As ErrorObj
        Dim err As New ErrorObj
        If (Not (err.HasError)) And HasValue(_commandElement.CommandText) Then
            givenSqlCommand.CommandText = _commandElement.CommandText
        Else
            err.ErrorMessage = ERROR_COMMANDTEXT
            err.HasError = True
        End If

        If (Not (err.HasError)) And HasValue(_commandElement.CommandType) Then
            givenSqlCommand.CommandType = _commandElement.CommandType
        Else
            err.ErrorMessage = ERROR_COMMANDTYPE
            err.HasError = True
        End If
        If (Not (err.HasError)) Then
            err = AttachParametersToCommand(givenSqlCommand)
        End If
        Return err
    End Function

    ''' <summary>
    ''' Attaches the parameters to the given SQL command instance
    ''' </summary>
    ''' <param name="givenSqlCommand">The given SQL command.</param>
    ''' <returns></returns>
    Private Function AttachParametersToCommand(ByVal givenSqlCommand As SqlCommand) As ErrorObj
        Dim err As New ErrorObj
        If Not (_commandElement.CommandParameter Is Nothing) Then
            If (_commandElement.CommandParameter.Count > 0) Then
                Dim paramaterList As Generic.List(Of DESQLParameter) = _commandElement.CommandParameter
                Dim parameterPiece As DESQLParameter

                For Each parameterPiece In paramaterList
                    Dim tempSqlParameter As New SqlParameter
                    If (Not (err.HasError)) And HasValue(parameterPiece.ParamName) Then
                        tempSqlParameter.ParameterName = parameterPiece.ParamName
                    Else
                        err.HasError = True
                        err.ErrorMessage = ERROR_COMMANDPARAMETERNAME
                        Exit For
                    End If

                    'If paramValue is nothing considering it as DBNull.Value
                    If (parameterPiece.ParamValue Is Nothing) Then
                        tempSqlParameter.Value = DBNull.Value
                    Else
                        If (Not (err.HasError)) And HasValue(parameterPiece.ParamValue, parameterPiece.ParamType) Then
                            tempSqlParameter.SqlDbType = parameterPiece.ParamType
                            tempSqlParameter.Value = parameterPiece.ParamValue
                        Else
                            err.HasError = True
                            err.ErrorMessage = ERROR_COMMANDPARAMETERTYPEVALUE
                            Exit For
                        End If
                    End If

                    If (Not (err.HasError)) Then
                        If (parameterPiece.ParamDirection = Nothing) Then
                            parameterPiece.ParamDirection = ParameterDirection.Input
                        End If
                        tempSqlParameter.Direction = parameterPiece.ParamDirection
                    End If
                    If (Not (err.HasError)) Then
                        givenSqlCommand.Parameters.Add(tempSqlParameter)

                    End If
                    tempSqlParameter = Nothing
                Next
                parameterPiece = Nothing
                'tempSqlParameter = Nothing
            End If
        End If
        Return err
    End Function

    ''' <summary>
    ''' Determines whether the specified given value has value.
    ''' </summary>
    ''' <param name="givenValueToCheck">The given value to check.</param>
    ''' <returns>
    ''' <c>true</c> if the specified given value to check has value; otherwise, <c>false</c>.
    ''' </returns>
    Private Function HasValue(ByVal givenValueToCheck As String) As Boolean
        Dim tempIsExists As Boolean = False
        If ((Not (givenValueToCheck Is Nothing)) And (givenValueToCheck.Length > 0)) Then
            tempIsExists = True
        Else
            tempIsExists = False
        End If
        Return tempIsExists
    End Function

    ''' <summary>
    ''' Determines whether the specified given type to has any CommandType.
    ''' </summary>
    ''' <param name="givenTypeToCheck">The given type to check.</param>
    ''' <returns>
    ''' <c>true</c> if the specified given type has value; otherwise, <c>false</c>.
    ''' </returns>
    Private Function HasValue(ByVal givenTypeToCheck As CommandType) As Boolean
        Dim tempIsExists As Boolean = False
        If Not (givenTypeToCheck = Nothing) Then
            tempIsExists = True
        Else
            tempIsExists = False
        End If
        Return tempIsExists
    End Function

    ''' <summary>
    ''' Determines whether the specified given value is matching with given SQLDbType.
    ''' </summary>
    ''' <param name="givenValueToCheck">The given value to check.</param>
    ''' <param name="givenTypeToCheck">The given type to check.</param>
    ''' <returns>
    ''' <c>true</c> if the specified given value is matching with given SQLDbType; otherwise, <c>false</c>.
    ''' </returns>
    Private Function HasValue(ByVal givenValueToCheck As String, ByVal givenTypeToCheck As SqlDbType) As Boolean
        Dim tempIsExists As Boolean = False
        If (givenValueToCheck.Equals(DBNull.Value)) Then
            tempIsExists = True
        Else
            Select Case givenTypeToCheck

                Case SqlDbType.VarChar
                    If Not (givenValueToCheck = Nothing) Then
                        tempIsExists = True
                    ElseIf (givenValueToCheck.Equals(String.Empty)) Then
                        tempIsExists = True
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.NVarChar
                    If Not (givenValueToCheck = Nothing) Then
                        tempIsExists = True
                    ElseIf (givenValueToCheck.Equals(String.Empty)) Then
                        tempIsExists = True
                    Else
                        tempIsExists = False
                    End If

                    'IsNumeric
                Case SqlDbType.Int
                    If Not (givenValueToCheck = Nothing) Then
                        If IsNumeric(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.Decimal
                    If Not (givenValueToCheck = Nothing) Then
                        If IsNumeric(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.Float
                    If Not (givenValueToCheck = Nothing) Then
                        If IsNumeric(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.TinyInt
                    If Not (givenValueToCheck = Nothing) Then
                        If IsNumeric(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.SmallInt
                    If Not (givenValueToCheck = Nothing) Then
                        If IsNumeric(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.BigInt
                    If Not (givenValueToCheck = Nothing) Then
                        If IsNumeric(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                    'IsDate
                Case SqlDbType.Date
                    If Not (givenValueToCheck = Nothing) Then
                        If IsDate(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.DateTime
                    If Not (givenValueToCheck = Nothing) Then
                        If IsDate(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                Case SqlDbType.SmallDateTime
                    If Not (givenValueToCheck = Nothing) Then
                        If IsDate(givenValueToCheck) Then
                            tempIsExists = True
                        Else
                            tempIsExists = False
                        End If
                    Else
                        tempIsExists = False
                    End If

                    'Case SqlDbType.Binary
                    'Case SqlDbType.Bit
                    'Case SqlDbType.Char
                    'Case SqlDbType.DateTime2
                    'Case SqlDbType.DateTimeOffset
                    'Case SqlDbType.Image
                    'Case SqlDbType.Money
                    'Case SqlDbType.NChar
                    'Case SqlDbType.NText
                    'Case SqlDbType.NVarChar
                    'Case SqlDbType.Real
                    'Case SqlDbType.SmallMoney
                    'Case SqlDbType.Structured
                    'Case SqlDbType.Text
                    'Case SqlDbType.Time
                    'Case SqlDbType.Timestamp
                    'Case SqlDbType.Udt
                    'Case SqlDbType.UniqueIdentifier
                    'Case SqlDbType.VarBinary
                    'Case SqlDbType.Variant
                    'Case SqlDbType.Xml

                Case Else
                    If Not (givenValueToCheck = Nothing) Then
                        tempIsExists = True
                    Else
                        tempIsExists = False
                    End If

            End Select
        End If

        Return tempIsExists
    End Function

#End Region

End Class
