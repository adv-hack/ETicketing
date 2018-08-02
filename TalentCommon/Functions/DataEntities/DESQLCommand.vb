
#Region "CommandExecution Enum"
''' <summary>
''' Provides the execution type for the command instance
''' </summary>
Public Enum CommandExecution
    ''' <summary>
    ''' Execute Command and return DataSet
    ''' </summary>
    ExecuteDataSet
    ''' <summary>
    ''' Execute Command and return rows affected
    ''' </summary>
    ExecuteNonQuery
    ''' <summary>
    ''' Execute Command and return single value or value of first row, first column
    ''' </summary>
    ExecuteScalar
    ''' <summary>
    ''' Execute Command and return DataReader instance
    ''' </summary>
    ExecuteReader
End Enum
#End Region

''' <summary>
''' Class Provides the Properties for building up the SQLCommand instance
''' </summary>
<Serializable()> _
Public Class DESQLCommand

#Region "Class Lavel Fields"
    ''' <summary>
    ''' Command Text
    ''' </summary>
    Private _commandText As String
    ''' <summary>
    ''' Command Type
    ''' </summary>
    Private _commandType As CommandType = Data.CommandType.Text
    ''' <summary>
    ''' Command Parameter of type DESQLParameter (SqlParameter)
    ''' </summary>
    Private _commandParameter As Generic.List(Of DESQLParameter)
    ''' <summary>
    ''' Command Execution Method of type CommandExecution Enum
    ''' </summary>
    Private _commandExecutionType As CommandExecution
#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the command text Transact SQL Statement 
    ''' or Parameterised SQL Statement
    ''' or Stored Procedure Name
    ''' </summary>
    ''' <value>The command text.</value>
    Public Property CommandText() As String
        Get
            Return _commandText
        End Get
        Set(ByVal value As String)
            _commandText = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the type of the command based on Data.CommandType
    ''' </summary>
    ''' <value>The type of the command.</value>
    Public Property CommandType() As CommandType
        Get
            Return _commandType
        End Get
        Set(ByVal value As CommandType)
            _commandType = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the execution type for the command based on Talent.Common.CommandExecution
    ''' </summary>
    ''' <value>The type of the command execution.</value>
    Public Property CommandExecutionType() As CommandExecution
        Get
            Return _commandExecutionType
        End Get
        Set(ByVal value As CommandExecution)
            _commandExecutionType = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the command parameter details 
    ''' through Generic List collection of DESQLParameter instance 
    ''' </summary>
    ''' <value>The command parameter.</value>
    Public Property CommandParameter() As Generic.List(Of DESQLParameter)
        Get
            If (_commandParameter Is Nothing) Then
                _commandParameter = New Generic.List(Of DESQLParameter)
            End If
            Return _commandParameter
        End Get
        Set(ByVal value As Generic.List(Of DESQLParameter))
            _commandParameter = value
        End Set
    End Property

#End Region

End Class