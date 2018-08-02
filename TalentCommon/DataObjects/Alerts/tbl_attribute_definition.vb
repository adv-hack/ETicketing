Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data for the table tbl_attribute_definition
    ''' </summary>
    <Serializable()> _
    Public Class tbl_attribute_definition
        Inherits DBObjectBase
#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_attribute_definition"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_attribute_definition" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets all the alert definition records
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAttributeDescriptionByBUPartner(ByVal BusinessUnit As String, ByVal Partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAttributeDescriptionByBUPartner")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT (DESCRIPTION + ' (' + NAME + ')') as DESCRIPTION, FOREIGN_KEY FROM TBL_ATTRIBUTE_DEFINITION WHERE BUSINESS_UNIT='" + BusinessUnit + "' AND PARTNER='" + Partner + "' ORDER BY DESCRIPTION"

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
        ''' Gets all the alert definition records
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAttributeDescription(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAttributeDescription")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT (DESCRIPTION + ' (' + NAME + ')') as DESCRIPTION, FOREIGN_KEY FROM TBL_ATTRIBUTE_DEFINITION ORDER BY DESCRIPTION"

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
        ''' Gets the FOREIGNKEY from tbl_attribute_definition for the given alert name.
        ''' Used for the special alerts: 'BirthdayAlert', 'FFBirthdayAlert', 'CCExpiryAlertPPS' and 'CCExpiryAlertSAV'
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner code</param>
        ''' <param name="alertName">The special alert name</param>
        ''' <param name="cacheing">The boolean value to enable caching, default is true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default is 30</param>
        ''' <returns>The attribute FOREIGNKEY as integer</returns>
        ''' <remarks></remarks>
        Public Function GetSpecialAlertAttributeForeignKey(ByVal businessUnit As String, ByVal partner As String, ByVal alertName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim attributeForeignKey As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetSpecialAlertAttributeForeignKey")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim outputDataTable As New DataTable

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(2) As String
            Dim cacheKeyHierarchyBased(2) As String

            whereClauseFetchHierarchy(0) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "AND [BUSINESS_UNIT]=@BusinessUnit AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "AND [BUSINESS_UNIT]='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND [PARTNER]='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", alertName))
                Dim sqlStatement As String = "SELECT TOP 1 [FOREIGN_KEY] FROM [tbl_attribute_definition] WITH(NOLOCK) WHERE [NAME] = @Name "

                'Execute
                Dim err As New ErrorObj

                For whereClauseFetchHierarchyCounter As Integer = 0 To 2 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter) & alertName
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If outputDataTable.Rows.Count = 1 Then
                            attributeForeignKey = outputDataTable.Rows(0)(0).ToString
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try
            Return attributeForeignKey
        End Function

#End Region

    End Class
End Namespace