Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page_attribute based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_page_attribute
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
        ''' Initializes a new instance of the <see cref="tbl_page_attribute" /> class.
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
            Dim sqlStatement As String = "DELETE TBL_PAGE_ATTRIBUTE " & _
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

        ''' <summary>
        ''' Updates or Inserts a record in the tbl_page_attribute table
        ''' </summary>
        ''' <param name="BusinessUnit">Business Unit</param>
        ''' <param name="PartnerCode">Partner Code</param>
        ''' <param name="PageCode">Page Code</param>
        ''' <param name="AttributeName">Attribute Name</param>
        ''' <param name="AttributeValue">Attribute Value</param>
        ''' <param name="Description">Description</param>
        ''' <param name="affectedRows">Optional parameter which returns the number of rows affected.</param>
        ''' <returns>Error Object.</returns>
        ''' <remarks></remarks>
        Public Function InsertOrUpdate(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                                       ByVal AttributeName As String, ByVal AttributeValue As String, ByVal Description As String,
                                       Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ATTR_NAME", AttributeName))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ATTR_VALUE", AttributeValue))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DESCRIPTION", Description))
            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("UPDATE TBL_PAGE_ATTRIBUTE ")
            sqlStatement.Append("SET ATTR_VALUE = @ATTR_VALUE, DESCRIPTION = @DESCRIPTION ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND ATTR_NAME = @ATTR_NAME ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("IF @@ROWCOUNT <> 0 BEGIN ")

            sqlStatement.Append("SELECT 0 ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("END")

            sqlStatement.Append(vbLf)
            sqlStatement.Append("ELSE IF @@ROWCOUNT = 0")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("BEGIN ")
            sqlStatement.Append(vbLf)
            sqlStatement.Append("INSERT INTO TBL_PAGE_ATTRIBUTE(BUSINESS_UNIT, PARTNER_CODE, ")
            sqlStatement.Append(" PAGE_CODE, ATTR_NAME, ATTR_VALUE, DESCRIPTION) VALUES ")
            sqlStatement.Append("(@BUSINESS_UNIT, @PARTNER_CODE, @PAGE_CODE, @ATTR_NAME, @ATTR_VALUE, @DESCRIPTION) ")
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
                errObj.ErrorNumber = "TPAGEATTR-01"
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return errObj

        End Function

        Public Function InsertOrUpdate(ByVal listDEPageAttribute As List(Of DEPageAttribute), Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim errObj As New ErrorObj

            For Each dpa As DEPageAttribute In listDEPageAttribute
                errObj = InsertOrUpdate(dpa.BusinessUnit, dpa.PartnerCode, dpa.PageCode, dpa.AttributeName, dpa.AttributeValue, dpa.Description, affectedRows)
                If (errObj.HasError) Then
                    Return errObj
                End If
            Next
            Return errObj
        End Function

        Public Function Delete(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                               ByVal AttributeName As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ATTR_NAME", AttributeName))

            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("DELETE FROM TBL_PAGE_ATTRIBUTE ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND ATTR_NAME = @ATTR_NAME ")

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
                errObj.ErrorNumber = "TPAGEATTR-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try


            Return errObj

        End Function
        Public Function Delete_WithWildCard(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                            ByVal AttributeName As String, Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ATTR_NAME", AttributeName & "%"))

            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("DELETE FROM TBL_PAGE_ATTRIBUTE ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND ATTR_NAME LIKE @ATTR_NAME ")

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
                errObj.ErrorNumber = "TPAGEATTR-02"
            Finally
                talentSqlAccessDetail = Nothing
            End Try


            Return errObj

        End Function

        Public Function GetTotalLinksForVoucherHTML(ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                                                    ByVal attributeName As String, Optional ByRef LinkIds As List(Of Integer) = Nothing) As Integer
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim noOfLinkRecords As Integer = 0
            Dim errObj As New ErrorObj
            Dim LinkIDList As New List(Of Integer)
            'Construct The Call
            Try
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet

                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ATTR_NAME", attributeName))

                Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
                sqlStatement.Append("SELECT substring(attr_name,patindex('%k[0-9]%',ATTR_NAME)+1,len(attr_name)) AS LINKID FROM TBL_PAGE_ATTRIBUTE ")
                sqlStatement.Append(" WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE ")
                sqlStatement.Append(" AND PAGE_CODE=@PAGE_CODE AND ATTR_NAME LIKE  @ATTR_NAME ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append(" IF @@ROWCOUNT = 0 BEGIN ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append(" SELECT substring(attr_name,patindex('%k[0-9]%',ATTR_NAME)+1,len(attr_name)) AS LINKID FROM TBL_PAGE_ATTRIBUTE ")
                sqlStatement.Append(" WHERE BUSINESS_UNIT='*ALL' AND PARTNER_CODE = @PARTNER_CODE ")
                sqlStatement.Append(" AND PAGE_CODE=@PAGE_CODE AND ATTR_NAME LIKE  @ATTR_NAME ")
                sqlStatement.Append(vbLf)
                sqlStatement.Append(" END ")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                errObj = talentSqlAccessDetail.SQLAccess()
                If (Not (errObj.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    For Each r As DataRow In talentSqlAccessDetail.ResultDataSet.Tables(0).Rows
                        LinkIDList.Add(r("LINKID"))
                    Next
                    If LinkIDList.Count = 0 Then
                        For Each r As DataRow In talentSqlAccessDetail.ResultDataSet.Tables(1).Rows
                            LinkIDList.Add(r("LINKID"))
                        Next
                    End If
                    If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count = 0 AndAlso talentSqlAccessDetail.ResultDataSet.Tables.Count > 1 Then
                        noOfLinkRecords = talentSqlAccessDetail.ResultDataSet.Tables(1).Rows.Count
                    Else
                        noOfLinkRecords = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count
                    End If
                    'noOfLinkRecords = IIf(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count = 0 AndAlso talentSqlAccessDetail.ResultDataSet.Tables.Count > 1,
                    '                      talentSqlAccessDetail.ResultDataSet.Tables(1).Rows.Count,
                    '                      talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count)
                    LinkIds = LinkIDList
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return noOfLinkRecords
        End Function

        ''' <summary>
        ''' Copies the records from one business unit to another business unit inside the table tbl_page_attribute
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
                Dim sqlStatement As String = "INSERT INTO TBL_PAGE_ATTRIBUTE (" & _
                    "BUSINESS_UNIT, ATTR_NAME, ATTR_VALUE, DESCRIPTION, " & _
                    "PAGE_CODE, PARTNER_CODE) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, ATTR_NAME, ATTR_VALUE, DESCRIPTION, " & _
                    "PAGE_CODE, PARTNER_CODE " & _
                    "FROM TBL_PAGE_ATTRIBUTE " & _
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
