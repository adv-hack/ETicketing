Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_order_status
    ''' </summary>
    <Serializable()> _
    Public Class tbl_order_status
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_order_status"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_order_status" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Inserts a new record into tbl_order_status
        ''' </summary>
        ''' <param name="businessUnit">The business unit to delete.</param>
        ''' <param name="orderId">The retail temp order id</param>
        ''' <param name="status">The current order status level</param>
        ''' <param name="comment">A comment against this order level</param>
        ''' <returns>No of affected rows</returns>
        Public Function Insert(ByVal businessUnit As String, ByVal orderId As String, ByVal status As String, ByVal comment As String) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "INSERT INTO [tbl_order_status] (BUSINESS_UNIT, ORDER_ID, STATUS, TIMESTAMP, COMMENT) VALUES (@BusinessUnit, @OrderId, @Status, getdate(), @Comment)"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderId", orderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", status))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Comment", comment))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function

#End Region

    End Class
End Namespace