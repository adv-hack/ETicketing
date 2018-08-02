Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_basket_status based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_basket_status
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_basket_status"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_basket_status" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        Public Property BasketStatusEntity As DEBasketStatus = Nothing
#End Region

#Region "Public Methods"

        Public Function Insert(Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                If BasketStatusEntity.BasketConnectionString.Trim.Length <= 0 Then
                    talentSqlAccessDetail.Settings = _settings
                Else
                    Dim tempSettings As DESettings = _settings
                    tempSettings.FrontEndConnectionString = BasketStatusEntity.BasketConnectionString
                    talentSqlAccessDetail.Settings = tempSettings
                End If

                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO tbl_basket_status (" & _
                        "BUSINESS_UNIT, PARTNER, LOGINID, ORDER_ID, STATUS, COMMENT, BASKET_HEADER_ID," & _
                        "EXTERNAL_ORDER_NUMBER, GOOGLE_SERIAL_NUMBER, TIMESTAMP) VALUES (" & _
                        "@BusinessUnit, @Partner, @LoginID, @OrderId, @Status, " & _
                        "@Comment, @BasketHeaderId, @ExternalOrderNumber, @GoogleSerialNumber, @Timestamp) "
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", basketStatusEntity.BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", basketStatusEntity.Partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", basketStatusEntity.LoginId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@OrderId", basketStatusEntity.TempOrderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", basketStatusEntity.Status))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Comment", basketStatusEntity.Comment))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderId", basketStatusEntity.BasketHeaderId))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ExternalOrderNumber", basketStatusEntity.ExternalOrderNumber))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@GoogleSerialNumber", BasketStatusEntity.GoogleSerialNumber))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Timestamp", Now, SqlDbType.DateTime))
                'Execute Insert
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

        Public Function GetByBasketHeaderID(ByVal frontendConnString As String, ByVal basketHeaderID As String, ByVal externalOrderNumber As String) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBasketHeaderID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                Dim tempSettings As DESettings = _settings
                tempSettings.FrontEndConnectionString = frontendConnString
                talentSqlAccessDetail.Settings = tempSettings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = 30
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(basketHeaderID) & ToUpper(externalOrderNumber)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                If basketHeaderID.Trim.Length > 0 Then
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_basket_status WHERE BASKET_HEADER_ID=@BasketHeaderID"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BasketHeaderID", basketHeaderID))
                ElseIf externalOrderNumber.Trim.Length > 0 Then
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_basket_status WHERE EXTERNAL_ORDER_NUMBER=@ExternalOrderNumber"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ExternalOrderNumber", externalOrderNumber))
                End If

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable

        End Function


#End Region

    End Class

End Namespace

