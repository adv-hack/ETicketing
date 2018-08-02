Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_event_category based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_event_category
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_event_category"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_event_category" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get tbl_event_category data by Business Unit and Partner
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="cacheing">The caching option default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllEventCategoriesByBUAndPartner(ByVal businessUnit As String, ByVal partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllEventCategoriesByBUAndPartner")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(3) As String
            Dim orderByClause As String = " ORDER BY CATEGORY_DESCRIPTION"

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner "
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner "
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

                Dim sqlStatement As String = "SELECT * FROM [tbl_event_category] WHERE "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & orderByClause
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If (outputDataTable.Rows.Count > 0) Then
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

            'Return the results 
            Return outputDataTable
        End Function

#End Region

    End Class

End Namespace