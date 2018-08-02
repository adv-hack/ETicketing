Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_partner_user_club based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_partner_user_club
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_partner_user_club"

        ''' <summary>
        ''' To decide to continue the loop of dictionary object in InsertMultiple method
        ''' </summary>
        Private _continueInsertMultiple As Boolean = True

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_partner_user_club" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Insert the specified parameter to tbl_partner_user_club
        ''' </summary>
        ''' <param name="partner">The partner.</param>
        ''' <param name="loginID">The login ID.</param>
        ''' <param name="clubCode">The club code.</param>
        ''' <param name="available">if set to <c>true</c> [available].</param>
        ''' <param name="isDefault">if set to <c>true</c> [is default].</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>Affected Rows</returns>
        Public Function Insert(ByVal partner As String, ByVal loginID As String, ByVal clubCode As String, ByVal available As Boolean, ByVal isDefault As Boolean, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                'Execute
                Dim err As New ErrorObj
                Dim sqlStatement As String = "INSERT INTO TBL_PARTNER_USER_CLUB (" & _
                        "PARTNER, LOGINID, CLUB_CODE, AVAILABLE, IS_DEFAULT) VALUES (" & _
                        "@Partner, @LoginID, @ClubCode, @Available, @IsDefault) "
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClubCode", clubCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Available", available))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@IsDefault", isDefault))
                'Execute Insert
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    'this is to decide whether to continue the for loop in DeleteAndInsertMultiple method
                    'if any error exit the for loop
                    _continueInsertMultiple = False
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

        ''' <summary>
        ''' Inserts the specified parameter to tbl_partner_user_club.
        ''' </summary>
        ''' <param name="partner">The partner.</param>
        ''' <param name="loginID">The login ID.</param>
        ''' <param name="listDEPartnerUserClub">list of type DEPartnerUserClub.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>Affected Rows</returns>
        Public Function InsertMultiple(ByVal partner As String, ByVal loginID As String, ByVal listDEPartnerUserClub As Generic.List(Of DEPartnerUserClub), Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim totalAffectedRows As Integer = 0

            'Loops the dictionary if exists with key value pairs
            If Not (listDEPartnerUserClub Is Nothing) Then
                If (listDEPartnerUserClub.Count > 0) Then
                    _continueInsertMultiple = True
                    Dim affectedRows As Integer = 0
                    If (_continueInsertMultiple) Then
                        For Each partnerUserClub As DEPartnerUserClub In listDEPartnerUserClub
                            'to make sure there is no error in the Insert method
                            If (_continueInsertMultiple) Then
                                affectedRows = Insert(partner, loginID, partnerUserClub.ClubCode, partnerUserClub.Available, partnerUserClub.IsDefault, givenTransaction)
                                totalAffectedRows = totalAffectedRows + affectedRows
                            Else
                                Exit For
                            End If
                        Next
                    Else
                    End If
                End If
            End If
            'Return the results 
            Return totalAffectedRows
        End Function

        ''' <summary>
        ''' Deletes all the clubs for given login ID and partner in tbl_partner_user_club
        ''' </summary>
        ''' <param name="partner">The partner.</param>
        ''' <param name="loginID">The login ID.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>Affected Rows</returns>
        Public Function DeleteClubsByLoginIDAndPartner(ByVal partner As String, ByVal loginID As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_PARTNER_USER_CLUB " & _
                "WHERE PARTNER=@Partner AND LOGINID=@LoginID"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LoginID", loginID))

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
        ''' Deletes all the clubs for given loginID and insert multiple partner user club.
        ''' </summary>
        ''' <param name="partner">The partner.</param>
        ''' <param name="loginID">The login ID.</param>
        ''' <param name="listDEPartnerUserClub">The list of type DEPartnerUserClub.</param>
        ''' <returns></returns>
        Public Function DeleteAndInsertMultiplePartnerUserClub(ByVal partner As String, ByVal loginID As String, ByVal listDEPartnerUserClub As Generic.List(Of DEPartnerUserClub)) As Integer
            Dim affectedRows As Integer = 0
            Dim isErrorinTransaction As Boolean = False
            Dim sqlTrans As SqlTransaction
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'create and get the transaction object
            talentSqlAccessDetail.Settings = _settings
            sqlTrans = talentSqlAccessDetail.BeginTransaction(errObj)
            If (Not (errObj.HasError)) Then
                'Delete all before insert
                If (Not (sqlTrans.Connection Is Nothing)) Then
                    affectedRows = DeleteClubsByLoginIDAndPartner(partner, loginID, sqlTrans)
                Else
                    isErrorinTransaction = True
                End If
                'to make sure there is no error in the DeleteClubsByLoginIDAndPartner method
                If (Not (sqlTrans.Connection Is Nothing)) Then
                    affectedRows = InsertMultiple(partner, loginID, listDEPartnerUserClub, sqlTrans)
                Else
                    isErrorinTransaction = True
                End If
                'to make sure there is no error in the InsertMultiple method
                If Not (isErrorinTransaction) Then
                    If (Not (sqlTrans.Connection Is Nothing)) Then
                        sqlTrans.Commit()
                        talentSqlAccessDetail.EndTransaction(sqlTrans)
                    End If
                End If
            End If
            'Return the results 
            Return affectedRows
        End Function

#End Region

    End Class
End Namespace
