Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_partner_user based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_partner_user
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_partner_user"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_partner_user" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Inserts a record into the  table tbl_offline_printing table
        ''' </summary>
        ''' <param name="loginId">Login Id</param>
        ''' <param name="partner">Partner</param>
        ''' <param name="cacheing">Cacheing On/Off - Default=True</param>
        ''' <param name="cacheTimeMinutes">Cache Time In Minutes - Default=30</param>
        ''' <returns>DataTable</returns>
        Public Function GetByLoginId(ByVal loginId As String, _
                        Optional ByVal partner As String = "", _
                        Optional ByVal cacheing As Boolean = True, _
                        Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable


            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetById")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & loginId.ToUpper & partner.ToUpper
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "select * from tbl_partner_user where loginid = @loginid "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@loginid", loginId))
                If Not String.IsNullOrEmpty(partner) Then
                    talentSqlAccessDetail.CommandElements.CommandText += " and partner = @partner "
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@partner", partner))
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

        ''' <summary>
        ''' Updates the restrict payment method.
        ''' </summary>
        ''' <param name="restrictedPaymentMethod">The restricted payment method.</param>
        ''' <param name="loginId">The login id.</param>
        ''' <param name="partner">The partner.</param>
        ''' <param name="givenTransaction">The given transaction.</param><returns></returns>
        Public Function UpdateRestrictPaymentMethod(ByVal restrictedPaymentMethod As String, ByVal loginId As String, ByVal partner As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE tbl_partner_user " & _
                " SET RESTRICTED_PAYMENT_METHOD = @RestrictedPaymentMethod" & _
                " WHERE PARTNER = @Partner" & _
                " AND LOGINID = @LoginId"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RestrictedPaymentMethod", restrictedPaymentMethod))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", loginId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

            'Execute
            Dim err As New ErrorObj
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

#End Region

    End Class

End Namespace

