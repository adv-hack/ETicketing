Imports System.Data.SqlClient
Imports System.Text
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_product_relations_text_lang
    ''' </summary>
    <Serializable()> _
    Public Class tbl_product_relations_text_lang
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_product_relations_text_lang"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_product_relations_text_lang" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Get tbl_product_relations_text_lang data by Business Unit, Partner, Qualifier and Page Code
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="partner">The given partner</param>
        ''' <param name="pageCode">The given page code</param>
        ''' <param name="qualifier">The given qualifier</param>
        ''' <param name="cacheing">The caching option default true</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetTextByBUPartnerPageCodeQualifier(ByVal businessUnit As String, ByVal partner As String, ByVal pageCode As String, ByVal qualifier As String, _
                                                            Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTextByBUPartnerPageCodeQualifier")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Partner     Page Code
            'Given          Given       Given
            'Given          *ALL        Given
            'Given          *ALL        *ALL
            '*ALL           Given       *ALL
            '*ALL           *ALL        *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner AND PAGE_CODE=@PageCode "
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner) & ToUpper(pageCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE=@PageCode "
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(2) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(partner) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(4) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' "
            cacheKeyHierarchyBased(4) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Qualifier", qualifier))

                Dim sqlStatement As String = "SELECT * FROM [tbl_product_relations_text_lang] WHERE QUALIFIER=@Qualifier AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 4 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
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