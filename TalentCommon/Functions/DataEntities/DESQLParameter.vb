''' <summary>
''' Provides the properties to build sql paramater object
''' </summary>
<Serializable()> _
Public Class DESQLParameter

#Region "Class Level Fields"
    ''' <summary>
    ''' Paramater Name
    ''' </summary>
    Private _paramName As String
    ''' <summary>
    ''' Parameter Value
    ''' </summary>
    Private _paramValue As Object
    ''' <summary>
    ''' Parameter Type
    ''' </summary>
    Private _paramType As SqlDbType
    ''' <summary>
    ''' Parameter Direction
    ''' </summary>
    Private _paramDirection As ParameterDirection
#End Region

#Region "Properties"

    ''' <summary>
    ''' Gets or sets the name of the parameter
    ''' </summary>
    ''' <value>The name of the param.</value>
    Public Property ParamName() As String
        Get
            Return _paramName
        End Get
        Set(ByVal value As String)
            _paramName = value
        End Set
    End Property
    ''' <summary>
    ''' Gets or sets the parameter value
    ''' </summary>
    ''' <value>The param value.</value>
    Public Property ParamValue() As Object
        Get
            Return _paramValue
        End Get
        Set(ByVal value As Object)
            _paramValue = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the type of the parameter (SqlDbType)  
    ''' </summary>
    ''' <value>The type of the param.</value>
    Public Property ParamType() As SqlDbType
        Get
            Return _paramType
        End Get
        Set(ByVal value As SqlDbType)
            _paramType = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the parameter direction of type Data.ParameterDirection
    ''' </summary>
    ''' <value>The param direction.</value>
    Public Property ParamDirection() As ParameterDirection
        Get
            Return _paramDirection
        End Get
        Set(ByVal value As ParameterDirection)
            _paramDirection = value
        End Set
    End Property
#End Region

End Class
