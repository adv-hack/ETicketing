Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_payment_type_lang based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_payment_type_lang
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_payment_type_lang"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_payment_type_lang" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Private Methods"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the payment type records from the table tbl_payment_type_lang
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAll")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_payment_type_lang"

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
        ''' Gets all the payment type records from the table tbl_payment_type_lang
        ''' </summary>
        ''' <param name="paymentType">Payment type code</param>
        ''' <param name="languageCode">Language Code</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>Payment Description</returns>
        Public Function GetDescriptionByTypeAndLang(ByVal paymentType As String, ByVal languageCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim paymentTypeDescription As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDescriptionByTypeAndLang-" & paymentType & "-" & languageCode)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT PAYMENT_TYPE_DESCRIPTION FROM tbl_payment_type_lang WHERE PAYMENT_TYPE_CODE=@PaymentType AND PAYMENT_TYPE_LANGUAGE=@languageCode"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PaymentType", paymentType))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@languageCode", languageCode))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) AndAlso (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) AndAlso talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    paymentTypeDescription = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0).Item("PAYMENT_TYPE_DESCRIPTION")
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results
            Return paymentTypeDescription

        End Function

        ''' <summary>
        ''' Gets the payment type descriptions by Business unit, partner and language code.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partner">The partner.</param>
        ''' <param name="languageCode">The language code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param><returns></returns>
        Public Function GetByBUPartnerLangCode(ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBUPartnerLangCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'LanguageCode - Always Given
            'ControlCode - Always Given
            'BusinessUnit   PartnerCode
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(4) As String
            Dim cacheKeyHierarchyBased(4) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER=@Partner"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partner)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER=@Partner"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partner)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))

                Dim sqlStatement As String = "SELECT tptb.QUALIFIER, tptb.BUSINESS_UNIT, tptb.[PARTNER], tptb.PAYMENT_TYPE_CODE," & _
                                " tptl.PAYMENT_TYPE_LANGUAGE, tptl.PAYMENT_TYPE_DESCRIPTION FROM tbl_payment_type_bu tptb" & _
                                " INNER JOIN tbl_payment_type_lang tptl ON tptb.PAYMENT_TYPE_CODE = tptl.PAYMENT_TYPE_CODE " & _
                                " AND tptl.PAYMENT_TYPE_LANGUAGE = @LanguageCode WHERE "

                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(languageCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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