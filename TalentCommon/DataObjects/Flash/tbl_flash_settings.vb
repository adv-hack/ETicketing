Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_flash_settings based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_flash_settings
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_flash_settings"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_flash_settings" /> class.
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
            Dim sqlStatement As String = "DELETE TBL_FLASH_SETTINGS " & _
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
        ''' Gets the all the records by the given Business Unit, Partner and Page Code.
        ''' </summary>
        ''' <param name="businessUnit">the given business unit</param>
        ''' <param name="partner">the given partner</param>
        ''' <param name="pageCode">the given page code</param>
        ''' <param name="attributeName">The given attribute name</param>
        ''' <param name="queryString">The querystring value</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns></returns>
        Public Function GetAttributeByBUPartnerPageCodeAttributeQueryString(ByVal businessUnit As String, ByVal partner As String, ByVal pageCode As String, _
                        ByVal attributeName As String, ByVal queryString As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim outputValue As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAttributeByBUPartnerPageCodeAttributeQueryString")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT ATTRIBUTE_VALUE FROM TBL_FLASH_SETTINGS WHERE BUSINESS_UNIT IN (@BusinessUnit, @StarALL) AND")
                sqlStatement.Append(" PARTNER_CODE IN (@Partner, @StarALL) AND PAGE_CODE IN (@PageCode, @StarALL) ")
                sqlStatement.Append(" AND ATTRIBUTE_NAME = @AttributeName")
                sqlStatement.Append(" AND QUERYSTRING_PARAMETER = @QueryString")

                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner) & ToUpper(pageCode) & ToUpper(attributeName) & ToUpper(queryString)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeName", attributeName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QueryString", queryString))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StarALL", Utilities.GetAllString))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 AndAlso talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        outputValue = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0).ToString()
                    End If
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputValue
        End Function

        ''' <summary>
        ''' Copies the records from one business unit to another business unit inside the table tbl_flash_settings
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
                Dim sqlStatement As String = "INSERT INTO TBL_FLASH_SETTINGS (" & _
                    "BUSINESS_UNIT, ATTRIBUTE_NAME, ATTRIBUTE_VALUE, " & _
                    "PAGE_CODE, PARTNER_CODE, SEQUENCE, QUERYSTRING_PARAMETER) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, ATTRIBUTE_NAME, ATTRIBUTE_VALUE, " & _
                    "PAGE_CODE, PARTNER_CODE, SEQUENCE, QUERYSTRING_PARAMETER " & _
                    "FROM TBL_FLASH_SETTINGS " & _
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

        ''' <summary>
        ''' Updates the attribute value for the given business unit, partner code, page code and attribute name
        '''  and returns no of affected rows
        ''' </summary>
        ''' <param name="businessUnit">Name of the stadium.</param>
        ''' <param name="partnerCode">Partner Code</param>
        ''' <param name="pageCode">Page Code</param>
        ''' <param name="attributeName">Name of the attribute.</param>
        ''' <param name="attributeValue">The attribute value.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE TBL_FLASH_SETTINGS SET ATTRIBUTE_VALUE=@AttributeValue " & _
                    "WHERE BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode " & _
                    "AND PAGE_CODE=@PageCode AND ATTRIBUTE_NAME=@AttributeName "
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeName", attributeName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AttributeValue", attributeValue))

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
