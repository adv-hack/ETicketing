Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_authorized_users based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_authorized_users
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_authorized_users"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_authorized_users" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Update the authorized users record for the customer to indicate whether their password has been validated
        ''' </summary>
        ''' <param name="partner">The given partner</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="loginId">The customer login id</param>
        ''' <param name="passwordValidated">Updated password validated value as a boolean</param>
        ''' <returns>The number of rows affected as an integer value</returns>
        ''' <remarks></remarks>
        Public Function UpdatePasswordValidated(ByVal partner As String, ByVal businessUnit As String, ByVal loginId As String, ByVal passwordValidated As Boolean) As Integer
            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            Dim sqlStatement As New StringBuilder

            sqlStatement.Append("UPDATE [tbl_authorized_users] SET [PASSWORD_VALIDATED] = @PasswordValidated ")
            sqlStatement.Append("WHERE [PARTNER] = @Partner AND [BUSINESS_UNIT] = @BusinessUnit ")
            sqlStatement.Append("AND [LOGINID] = @LoginId")
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", loginId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PasswordValidated", passwordValidated))

            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            If rowsAffected > 0 Then
                Dim cachekey As New StringBuilder
                cachekey.Append("SQLAccess")
                cachekey.Append(GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "IsPasswordValidated"))
                cachekey.Append(partner).Append(loginId)
                RemoveKeyFromCache(cachekey.ToString())
            End If

            Return rowsAffected
        End Function

        ''' <summary>
        ''' Return a boolean value to indicate whether or not the password validated field is true or false for the given customer
        ''' </summary>
        ''' <param name="partner">The given partner</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="loginId">The customer number</param>
        ''' <param name="cacheing">Optional cache value, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A boolean to show if the password has been validated</returns>
        ''' <remarks></remarks>
        Public Function IsPasswordValidated(ByVal partner As String, ByVal businessUnit As String, ByVal loginId As String, _
                                            Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
            Dim passwordValid As Boolean = False
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "IsPasswordValidated")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As New StringBuilder

            Try
                sqlStatement.Append("SELECT [PASSWORD_VALIDATED] FROM [tbl_authorized_users] ")
                sqlStatement.Append("WHERE [PARTNER] = @Partner AND [BUSINESS_UNIT] = @BusinessUnit ")
                sqlStatement.Append("AND [LOGINID] = @LoginId")
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & partner.ToUpper & loginId.ToUpper
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginId", loginId))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        passwordValid = Utilities.CheckForDBNull_Boolean_DefaultFalse(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0))
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return passwordValid
        End Function

        ''' <summary>
        ''' Adds new entry into authorised user. This has now been changed so that the password is never stored on this table.
        ''' </summary>
        ''' <param name="loginId">The customer number</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Function AddAuthorisedUser(ByVal loginId As String, ByVal partner As String, ByVal businessUnit As String) As ErrorObj

            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            Dim insertStr As New StringBuilder
            insertStr.Append(" IF NOT EXISTS (SELECT * FROM tbl_authorized_users WHERE LOGINID = @LOGINID AND PARTNER = @PARTNER AND BUSINESS_UNIT = @BUSINESS_UNIT) ")
            insertStr.Append(" BEGIN ")
            insertStr.Append("   INSERT INTO [tbl_authorized_users] ")
            insertStr.Append("            ([BUSINESS_UNIT] ")
            insertStr.Append("            ,[PARTNER] ")
            insertStr.Append("            ,[LOGINID] ")
            insertStr.Append("            ,[PASSWORD] ")
            insertStr.Append("            ,[AUTO_PROCESS_DEFAULT_USER] ")
            insertStr.Append("            ,[IS_APPROVED] ")
            insertStr.Append("            ,[IS_LOCKED_OUT] ")
            insertStr.Append("            ,[CREATED_DATE] ")
            insertStr.Append("            ,[LAST_LOGIN_DATE] ")
            insertStr.Append("            ,[LAST_PASSWORD_CHANGED_DATE] ")
            insertStr.Append("            ,[LAST_LOCKED_OUT_DATE]) ")
            insertStr.Append("      VALUES ")
            insertStr.Append("            (@BUSINESS_UNIT ")
            insertStr.Append("            ,@PARTNER ")
            insertStr.Append("            ,@LOGINID ")
            insertStr.Append("            ,'' ")
            insertStr.Append("            ,@AUTO_PROCESS_DEFAULT_USER ")
            insertStr.Append("            ,@IS_APPROVED ")
            insertStr.Append("            ,@IS_LOCKED_OUT ")
            insertStr.Append("            ,GETDATE() ")
            insertStr.Append("            ,GETDATE() ")
            insertStr.Append("            ,GETDATE() ")
            insertStr.Append("            ,GETDATE()) ")
            insertStr.Append(" END ")
            insertStr.Append(" ELSE ")
            insertStr.Append(" BEGIN ")
            insertStr.Append("   UPDATE [tbl_authorized_users] SET ")
            insertStr.Append("           [PASSWORD] = ''")
            insertStr.Append("          ,[AUTO_PROCESS_DEFAULT_USER] = 0")
            insertStr.Append("          ,[IS_APPROVED] = 0")
            insertStr.Append("          ,[IS_LOCKED_OUT] = 0")
            insertStr.Append("          ,[LAST_LOGIN_DATE] = GETDATE()")
            insertStr.Append("   WHERE LOGINID = @LOGINID AND PARTNER = @PARTNER AND BUSINESS_UNIT = @BUSINESS_UNIT")
            insertStr.Append(" END ")



            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = insertStr.ToString()
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LOGINID", loginId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", businessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AUTO_PROCESS_DEFAULT_USER", False))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IS_APPROVED", False))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IS_LOCKED_OUT", False))


            Try
                err = talentSqlAccessDetail.SQLAccess()

                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
                talentSqlAccessDetail = Nothing

            Catch ex As Exception
                Const strError As String = "Error adding Authorised Users Record "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TACDBAU-01"
                    .HasError = True
                End With
            End Try

            Return err
        End Function
#End Region

    End Class

End Namespace

