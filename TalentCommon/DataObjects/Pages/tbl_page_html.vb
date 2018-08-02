Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_page_html based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_page_html
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "PageHTML"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_page_html" /> class.
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
            Dim sqlStatement As String = "DELETE TBL_PAGE_HTML " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_page_html
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
                Dim sqlStatement As String = "INSERT INTO TBL_PAGE_HTML (" & _
                    "BUSINESS_UNIT, HTML_1, HTML_2, HTML_3, " & _
                    "HTML_LOCATION, PAGE_CODE, PAGE_QUERYSTRING, " & _
                    "PARTNER, SECTION, SEQUENCE) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, HTML_1, HTML_2, HTML_3, " & _
                    "HTML_LOCATION, PAGE_CODE, PAGE_QUERYSTRING, " & _
                    "PARTNER, SECTION, SEQUENCE " & _
                    "FROM TBL_PAGE_HTML " & _
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
        ''' Get the HTML content records for the given page code and business unit
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="pageCode">The given page code</param>
        ''' <param name="givenTransaction">Optional transaction</param>
        ''' <param name="cacheing">Caching property</param>
        ''' <param name="cacheTimeMinutes">Cache time property</param>
        ''' <returns>Data table of records from the given page</returns>
        ''' <remarks></remarks>
        Public Function GetDataBy_BU_Partner_PageName(ByVal businessUnit As String, ByVal pageCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing,
                                                      Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = "GetHTMLStrings - " & "GetDataBy_BU_Partner_PageName - " & businessUnit & "|" & Utilities.GetAllString & "|" & pageCode
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT BUSINESS_UNIT, HTML_1, HTML_2, HTML_3, HTML_LOCATION, PAGE_CODE, PAGE_HTML_ID, PAGE_QUERYSTRING, PARTNER, SECTION, SEQUENCE FROM tbl_page_html WITH (NOLOCK)  WHERE " & _
                    "BUSINESS_UNIT=@BusinessUnit AND " & _
                    "PAGE_CODE=@Page_Code AND " & _
                    "[PARTNER]='*ALL'"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Page_Code", pageCode))

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
        ''' Get the HTML file records for the given page code and business unit
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="pageCode">The given page code</param>
        ''' <param name="givenTransaction">Optional transaction</param>
        ''' <param name="cacheing">Caching property</param>
        ''' <param name="cacheTimeMinutes">Cache time property</param>
        ''' <returns>Data table of records from the given page</returns>
        ''' <remarks></remarks>
        Public Function GetDataFILEBy_BU_Partner_PageName(ByVal businessUnit As String, ByVal pageCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing,
                                                          Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = "GetHTMLStrings - " & "GetDataFILEBy_BU_Partner_PageName - " & businessUnit & "|" & Utilities.GetAllString & "|" & pageCode
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement As String = "SELECT PAGE_HTML_ID, BUSINESS_UNIT, PARTNER, PAGE_CODE, SECTION, SEQUENCE, HTML_LOCATION, PAGE_QUERYSTRING FROM tbl_page_html WITH (NOLOCK) WHERE " & _
                    "PAGE_CODE=@Page_Code AND " & _
                    "BUSINESS_UNIT=@BusinessUnit AND " & _
                    "[PARTNER]='*ALL'"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Page_Code", pageCode))
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

