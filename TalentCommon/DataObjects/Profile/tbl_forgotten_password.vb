Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_forgotten_password based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_forgotten_password
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_forgotten_password"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_address" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"


        ''' <summary>
        ''' Create a log for the specified functionality in table and returns log header id as reference and no of rows affected
        ''' </summary>
        ''' <param name="businessUnit">The businessUnit.</param>
        ''' <param name="partner">The partner.</param>
        ''' <param name="status">The status of the token</param>
        ''' <param name="customerNumber">The login id.</param>
        ''' <param name="email">The customer email address</param>
        ''' <param name="hashedToken">The hashed token that is sent to the customer</param>
        ''' <param name="validTime">The amount of time the token will be valid for, in minutes</param>
        ''' <returns>no of affected rows</returns>
        Public Function Insert(ByVal businessUnit As String, ByVal partner As String, ByVal hashedToken As String, ByVal status As String, ByVal customerNumber As String, ByVal email As String, ByVal validTime As Integer) As Integer
            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "INSERT INTO TBL_FORGOTTEN_PASSWORD (" & _
                    "BUSINESS_UNIT, PARTNER, HASHED_TOKEN, STATUS, CUSTOMER_NUMBER, EMAIL_ADDRESS, TIMESTAMP, EXPIRE_TIMESTAMP, LAST_UPDATED) " & _
                    "VALUES (@BUSINESS_UNIT, @PARTNER, @HASHED_TOKEN, @STATUS, @CUSTOMER, @EMAIL, GETDATE(), DATEADD(minute, @VALIDTIME, GETDATE()), GETDATE())"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@HASHED_TOKEN", hashedToken))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@STATUS", status))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CUSTOMER", customerNumber))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@EMAIL", email))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VALIDTIME", validTime, SqlDbType.Int))
                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
                talentSqlAccessDetail = Nothing
            Catch ex As Exception
                Throw ex
            End Try
            'Return results
            Return rowsAffected
        End Function

        ''' <summary>
        ''' Sets all of a customer's tokens to expired.
        ''' </summary>
        ''' <param name="customer">Customer number</param>
        ''' <param name="status">New status to set record to</param>
        ''' <param name="businessUnit">Optional business unit</param>
        ''' <param name="partner">Optional partner</param>
        ''' <remarks></remarks>
        ''' <returns>Number of affected rows</returns>
        Public Function SetCustomerTokensAsUsed(ByVal customer As String, ByVal status As String, Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "") As Integer
            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

                Dim sqlStatement As String = "UPDATE tbl_forgotten_password SET EXPIRE_TIMESTAMP = GETDATE(), STATUS = @STATUS, LAST_UPDATED = GETDATE() WHERE CUSTOMER_NUMBER = @CUST"
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CUST", customer))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@STATUS", status))

                If Not String.IsNullOrEmpty(businessUnit) Then
                    talentSqlAccessDetail.CommandElements.CommandText += " AND BUSINESS_UNIT = @businessUnit "
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@businessUnit", businessUnit))
                End If
                If Not String.IsNullOrEmpty(partner) Then
                    talentSqlAccessDetail.CommandElements.CommandText += " AND partner = @partner "
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@partner", partner))
                End If

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
                talentSqlAccessDetail = Nothing
            Catch ex As Exception
                Throw ex
            End Try
            'return results
            Return rowsAffected
        End Function

        ''' <summary>
        ''' Used to find a record by it's hashed token
        ''' </summary>
        ''' <param name="hashedToken">the token after being hashed using SHA512</param>
        ''' <param name="businessUnit">The business unit, optional</param>
        ''' <param name="partner">the partner, also optional</param>
        ''' <returns>A data table of the results</returns>
        ''' <remarks></remarks>
        Public Function GetByHashedToken(ByVal hashedToken As String, Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "") As DataTable
            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_forgotten_password WHERE HASHED_TOKEN = @HASHED_TOKEN "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@HASHED_TOKEN", hashedToken))
                If Not String.IsNullOrEmpty(businessUnit) Then
                    talentSqlAccessDetail.CommandElements.CommandText += " AND BUSINESS_UNIT = @businessUnit "
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@businessUnit", businessUnit))
                End If
                If Not String.IsNullOrEmpty(partner) Then
                    talentSqlAccessDetail.CommandElements.CommandText += " AND partner = @partner "
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
        ''' Lets you mark a token as used. 
        ''' </summary>
        ''' <param name="hashedToken">the token after being hashed using SHA512</param>
        ''' <param name="businessUnit">Optional business unit</param>
        ''' <param name="partner">Optional partner</param>
        ''' <remarks>This will set the expiry to now. So once the record is used it will be deleted by TalentAdmin</remarks>
        Public Function SetTokenAsUsed(ByVal hashedToken As String, Optional ByVal businessUnit As String = "", Optional ByVal partner As String = "") As Integer
            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "UPDATE TBL_FORGOTTEN_PASSWORD SET EXPIRE_TIMESTAMP = GETDATE(), STATUS = 'EXPIRED', LAST_UPDATED = GETDATE() WHERE HASHED_TOKEN = @HASH"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@HASH", hashedToken))
            If Not String.IsNullOrEmpty(businessUnit) Then
                talentSqlAccessDetail.CommandElements.CommandText += " AND BUSINESS_UNIT = @businessUnit "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@businessUnit", businessUnit))
            End If
            If Not String.IsNullOrEmpty(partner) Then
                talentSqlAccessDetail.CommandElements.CommandText += " AND PARTNER = @partner "
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@partner", partner))
            End If

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing
            Return rowsAffected
        End Function

#End Region

    End Class

End Namespace