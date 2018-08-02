Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page_text_lang based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_page_text_lang
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_page_text_lang" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Deletes the specified business unit.
        ''' </summary>
        ''' <param name="businessUnitToDelete">The business unit to delete.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Delete(ByVal businessUnitToDelete As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_PAGE_TEXT_LANG " & _
                "WHERE BUSINESS_UNIT=@BusinessUnitToDelete "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitToDelete", businessUnitToDelete))

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

        Public Function Delete(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                              ByVal TextCode As String, ByVal LanguageCode As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", page_attribute.ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE", TextCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LanguageCode))

            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("DELETE FROM TBL_PAGE_TEXT_LANG ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND TEXT_CODE = @TEXT_CODE AND  LANGUAGE_CODE = @LANGUAGE_CODE")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "PTLGEATTR-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try


            Return errObj

        End Function

        Public Function Delete_WithWildCard(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                             ByVal TextCode As String, ByVal LanguageCode As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", page_attribute.ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE", TextCode & "%"))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LanguageCode))

            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("DELETE FROM TBL_PAGE_TEXT_LANG ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND TEXT_CODE LIKE @TEXT_CODE AND  LANGUAGE_CODE = @LANGUAGE_CODE")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "PTLGEATTR-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try


            Return errObj

        End Function

        ''' <summary>
        ''' Updates or Inserts a record in the tbl_page_text_lang table
        ''' </summary>
        ''' <param name="BusinessUnit">Business Unit</param>
        ''' <param name="PartnerCode">Partner Code</param>
        ''' <param name="PageCode">Page Code</param>
        ''' <param name="TextCode">Text Code</param>
        ''' <param name="LanguageCode">Language Code</param>
        ''' <param name="TextContent">Text Content</param>
        ''' <param name="affectedRows">Optional parameter which returns the number of rows affected.</param>
        ''' <returns>Error Object.</returns>
        ''' <remarks></remarks>
        Public Function InsertOrUpdate(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                                       ByVal TextCode As String, ByVal LanguageCode As String, ByVal TextContent As String,
                                        Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            'talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", page_text_lang.ID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE", TextCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LanguageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CONTENT", TextContent))
            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("UPDATE TBL_PAGE_TEXT_LANG ")
            sqlStatement.Append("SET LANGUAGE_CODE = @LANGUAGE_CODE, TEXT_CONTENT = @TEXT_CONTENT ")
            sqlStatement.Append(" WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append(" AND PAGE_CODE=@PAGE_CODE AND TEXT_CODE = @TEXT_CODE ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("IF @@ROWCOUNT <> 0 BEGIN ")

            sqlStatement.Append("SELECT 0 ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("END ")

            sqlStatement.Append(vbLf)
            sqlStatement.Append("ELSE IF @@ROWCOUNT = 0")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("BEGIN ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("INSERT INTO TBL_PAGE_TEXT_LANG(BUSINESS_UNIT, PARTNER_CODE, ")
            sqlStatement.Append(" PAGE_CODE, TEXT_CODE, LANGUAGE_CODE, TEXT_CONTENT) VALUES ")
            sqlStatement.Append("(@BUSINESS_UNIT, @PARTNER_CODE, @PAGE_CODE, @TEXT_CODE, @LANGUAGE_CODE, @TEXT_CONTENT) ")
            sqlStatement.Append("END")

            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()
            Try
                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                Else
                    affectedRows = 0
                End If

            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorMessage = ex.Message
                errObj.ErrorNumber = "TPAGETEXTLANG-01"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return errObj

        End Function

        Public Function InsertOrUpdate(ByVal listDEPageTextLang As List(Of DEPageTextLang), Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim errObj As New ErrorObj

            For Each dptl As DEPageTextLang In listDEPageTextLang
                errObj = InsertOrUpdate(dptl.BusinessUnit, dptl.PartnerCode, dptl.PageCode, dptl.TextCode, dptl.LanguageCode, dptl.TextContent, affectedRows)
                If (errObj.HasError) Then
                    Return errObj
                End If
            Next
            Return errObj
        End Function

        ''' <summary>
        ''' Copies the records from one business unit to another business unit inside the table tbl_page_text_lang
        ''' irrespective of whether destination business exists or not and returns no of affected rows
        ''' </summary>
        ''' <param name="fromBusinessUnit">From business unit.</param>
        ''' <param name="toBusinessUnit">To business unit.</param>
        ''' <returns>No Of Affected Rows</returns>
        Public Function CopyByBU(ByVal fromBusinessUnit As String, ByVal toBusinessUnit As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "INSERT INTO TBL_PAGE_TEXT_LANG (" & _
                    "BUSINESS_UNIT, LANGUAGE_CODE, PAGE_CODE, " & _
                    "PARTNER_CODE, TEXT_CODE, TEXT_CONTENT) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, LANGUAGE_CODE, PAGE_CODE, " & _
                    "PARTNER_CODE, TEXT_CODE, TEXT_CONTENT " & _
                    "FROM TBL_PAGE_TEXT_LANG " & _
                    "WHERE BUSINESS_UNIT = @FromBusinessUnit"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromBusinessUnit", fromBusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToBusinessUnit", toBusinessUnit))

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

            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows

        End Function

#End Region

    End Class

End Namespace
