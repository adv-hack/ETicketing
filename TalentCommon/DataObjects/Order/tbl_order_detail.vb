Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_order_detail
    ''' </summary>
    <Serializable()> _
    Public Class tbl_order_detail
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_order_detail"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_order_detail" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get Order Details by TEMP_ORDER_ID
        ''' </summary>
        ''' <param name="tempOrderID">The temporary order id to retrieve</param>
        ''' <returns>Data Table</returns>
        Public Function GetOrderDetailRecordsByTempOrderID(ByVal tempOrderId As String) As Data.DataTable

            Dim outputDataTable As New Data.DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("Select * FROM [tbl_order_detail] AS d ")
            sqlStatement.Append("INNER Join [tbl_order_header] AS h ON h.TEMP_ORDER_ID = d.ORDER_ID ")
            sqlStatement.Append("WHERE d.ORDER_ID = @tempOrderId")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderID", tempOrderId))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return outputDataTable

        End Function

        ''' <summary>
        ''' Deletes the order detail by given temp order ID.
        ''' </summary>
        ''' <param name="tempOrderID">The temp order ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function DeleteByTempOrderID(ByVal tempOrderID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE tbl_order_detail " & _
                " WHERE ORDER_ID=@TempOrderID "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderID", tempOrderID))

            'Execute
            If (givenTransaction Is Nothing) Then
                err = talentSqlAccessDetail.SQLAccess()
            Else
                err = talentSqlAccessDetail.SQLAccess(givenTransaction)
            End If
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows

        End Function


        ''' <summary>
        ''' Updates the number of shipped products by Temp Order ID when they are marked as collected
        ''' </summary>
        ''' <param name="tempOrderID">The temp order ID.</param>
        Public Function UpdateShippedProductsByTempOrderID(ByVal tempOrderID As String) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_order_detail SET QUANTITY_SHIPPED = QUANTITY, DATE_SHIPPED = GETDATE() WHERE ORDER_ID=@TempOrderID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TempOrderID", tempOrderID))

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