Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_control_text_lang based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_control_text_lang
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_control_text_lang"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_control_text_lang" /> class.
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
            Dim sqlStatement As String = "DELETE TBL_CONTROL_TEXT_LANG " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_control_text_lang
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
                Dim sqlStatement As String = "INSERT INTO TBL_CONTROL_TEXT_LANG (" & _
                    "BUSINESS_UNIT, LANGUAGE_CODE, PARTNER_CODE, " & _
                    "PAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, LANGUAGE_CODE, PARTNER_CODE, " & _
                    "PAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT " & _
                    "FROM TBL_CONTROL_TEXT_LANG " & _
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

        Public Function CopyByControlCode(ByVal fromControlCode As String, ByVal toControlCode As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "INSERT INTO TBL_CONTROL_TEXT_LANG (" & _
                    "BUSINESS_UNIT, LANGUAGE_CODE, PARTNER_CODE, " & _
                    "PAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT) " & _
                    "SELECT " & _
                    " BUSINESS_UNIT, LANGUAGE_CODE, PARTNER_CODE, " & _
                    "PAGE_CODE, @ToControlCode As CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT " & _
                    "FROM TBL_CONTROL_TEXT_LANG " & _
                    "WHERE CONTROL_CODE = @FromControlCode"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@FromControlCode", fromControlCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ToControlCode", toControlCode))

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
        ''' Gets the records based on given control code, language code and by fetch hierachy for business unit, partner code and page code
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="languageCode">The language code.</param>
        ''' <param name="controlCode">The control code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>Data Table</returns>
        Public Function GetByControlCode(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal languageCode As String, ByVal controlCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByControlCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'LanguageCode - Always Given
            'ControlCode - Always Given
            'BusinessUnit   PartnerCode PageCode
            'Given          Given       Given
            'Given          Given       *ALL
            'Given          *ALL        Given
            'Given          *ALL        *ALL
            '*ALL           Given       Given
            '*ALL           Given       *ALL
            '*ALL           *ALL        *ALL

            Dim whereClauseFetchHierarchy(7) As String
            Dim cacheKeyHierarchyBased(7) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(2) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(4) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(4) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(5) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(5) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(6) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(6) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG WHERE LANGUAGE_CODE=@LanguageCode AND CONTROL_CODE=@ControlCode AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 6 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlCode) & ToUpper(languageCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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

        ''' <summary>
        ''' Gets the records based on given control code and by fetch hierachy for business unit, partner code and page code
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="controlCode">The control code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>Data Table</returns>
        Public Function GetByControlCode(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal controlCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByControlCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'ControlCode - Always Given
            'BusinessUnit   PartnerCode PageCode
            'Given          Given       Given
            'Given          Given       *ALL
            'Given          *ALL        Given
            'Given          *ALL        *ALL
            '*ALL           Given       Given
            '*ALL           Given       *ALL
            '*ALL           *ALL        *ALL

            Dim whereClauseFetchHierarchy(7) As String
            Dim cacheKeyHierarchyBased(7) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(2) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(4) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(4) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(5) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(5) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(6) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(6) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG WHERE CONTROL_CODE=@ControlCode AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 6 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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

        ''' <summary>
        ''' Gets the records based on given control code and by fetch hierachy for business unit and partner code
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="controlCode">The control code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>Data Table</returns>
        Public Function GetByControlCodeWithNoPageCode(ByVal businessUnit As String, ByVal partnerCode As String, ByVal controlCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByControlCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'ControlCode - Always Given
            'BusinessUnit   PartnerCode
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(7) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partnerCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG WHERE CONTROL_CODE=@ControlCode AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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

        ''' <summary>
        ''' Gets the control details based on the given control text code, language code and by fetch hierachy for business unit, partner code and page code
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="languageCode">The language code.</param>
        ''' <param name="controlTextCode">The control text code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns></returns>
        Public Function GetByControlTextCode(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal languageCode As String, ByVal controlTextCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByControlTextCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'LanguageCode - Always Given
            'TextCode - Always Given
            'BusinessUnit   PartnerCode PageCode
            'Given          Given       Given
            'Given          Given       *ALL
            'Given          *ALL        Given
            'Given          *ALL        *ALL
            '*ALL           Given       Given
            '*ALL           Given       *ALL
            '*ALL           *ALL        *ALL

            Dim whereClauseFetchHierarchy(7) As String
            Dim cacheKeyHierarchyBased(7) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(2) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(4) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(4) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(5) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(5) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(6) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(6) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlTextCode", controlTextCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG WHERE LANGUAGE_CODE=@LanguageCode AND TEXT_CODE=@ControlTextCode AND "
                Dim err As New ErrorObj
                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 6 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlTextCode) & ToUpper(languageCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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

        ''' <summary>
        ''' Gets the control text detail based on the given parameters
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="controlCode">The control code.</param>
        ''' <param name="controlTextCode">The control text code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetTextByExactValue(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal controlCode As String, ByVal controlTextCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTextByExactValue")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlTextCode", controlTextCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG "
                Dim whereClause As String = "WHERE BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode AND CONTROL_CODE=@ControlCode AND TEXT_CODE=@ControlTextCode"

                Dim err As New ErrorObj
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClause
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlTextCode)

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
        ''' Gets the control details based on the given control text code and by fetch hierachy for business unit, partner code and page code
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="controlTextCode">The control text code.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataTable</returns>
        Public Function GetByControlTextCode(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal controlTextCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByControlTextCode")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'TextCode - Always Given
            'BusinessUnit   PartnerCode PageCode
            'Given          Given       Given
            'Given          Given       *ALL
            'Given          *ALL        Given
            'Given          *ALL        *ALL
            '*ALL           Given       Given
            '*ALL           Given       *ALL
            '*ALL           *ALL        *ALL

            Dim whereClauseFetchHierarchy(7) As String
            Dim cacheKeyHierarchyBased(7) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(2) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(4) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(4) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(5) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(5) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(6) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(6) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlTextCode", controlTextCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG WHERE TEXT_CODE=@ControlTextCode AND "
                Dim err As New ErrorObj
                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 6 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlTextCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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

        ''' <summary>
        ''' Updates the specified control content in tbl_control_text_lang based on all other paramaters and returns true if success.
        ''' Passing Nothing to any parameter is equal to DBNull.Value
        ''' </summary>
        ''' <param name="languageCode">The language code.</param>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="controlCode">The control code.</param>
        ''' <param name="textCode">The text code.</param>
        ''' <param name="controlContent">Content of the control.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal languageCode As String, ByVal controlCode As String, ByVal textCode As String, ByVal controlContent As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheStringExtension = ""
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE TBL_CONTROL_TEXT_LANG SET CONTROL_CONTENT=@ControlContent " & _
                    "WHERE LANGUAGE_CODE=@LanguageCode AND BUSINESS_UNIT=@BusinessUnit " & _
                    "AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode " & _
                    "AND CONTROL_CODE=@ControlCode AND TEXT_CODE=@TextCode"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TextCode", textCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlContent", controlContent))

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
        ''' Updates the specified control content in tbl_control_text_lang based on all other paramaters and returns true if success.
        ''' Passing Nothing to any parameter is equal to DBNull.Value
        ''' </summary>
        ''' <param name="languageCode">The language code.</param>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="partnerCode">The partner code.</param>
        ''' <param name="pageCode">The page code.</param>
        ''' <param name="controlCode">The control code.</param>
        ''' <param name="textCode">The text code.</param>
        ''' <param name="controlContent">Content of the control.</param>
        ''' <returns>No of affected rows</returns>
        Public Function UpdateOrAdd(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal languageCode As String, ByVal controlCode As String, ByVal textCode As String, ByVal controlContent As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim talentSqlAccessDetail0 As New TalentDataAccess
            Dim talentSqlAccessDetail1 As New TalentDataAccess
            Dim talentSqlAccessDetail2 As New TalentDataAccess
            Dim affectedRows As Integer = 0
            Dim rowCount As Integer = 0
            Dim err As New ErrorObj

            ' Check whether the intended record exists
            Try
                'Construct The Call
                talentSqlAccessDetail0.Settings = _settings
                talentSqlAccessDetail0.Settings.Cacheing = False
                talentSqlAccessDetail0.Settings.CacheStringExtension = ""
                talentSqlAccessDetail0.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                Dim sqlStatement0 As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG " & _
                    "WHERE LANGUAGE_CODE=@LanguageCode AND BUSINESS_UNIT=@BusinessUnit " & _
                    "AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode " & _
                    "AND CONTROL_CODE=@ControlCode AND TEXT_CODE=@TextCode"

                talentSqlAccessDetail0.CommandElements.CommandText = sqlStatement0
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@TextCode", textCode))
                talentSqlAccessDetail0.CommandElements.CommandParameter.Add(ConstructParameter("@ControlContent", controlContent))

                'Execute
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail0.SQLAccess()
                Else
                    err = talentSqlAccessDetail0.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail0.ResultDataSet Is Nothing)) Then
                    rowCount = talentSqlAccessDetail0.ResultDataSet.Tables(0).Rows.Count
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail0 = Nothing
            End Try


            ' Either Update the record (if exists) else perform an Insert
            If Not (err.HasError) And rowCount > 0 Then
                Try
                    'Construct The Call
                    talentSqlAccessDetail1.Settings = _settings
                    talentSqlAccessDetail1.Settings.Cacheing = False
                    talentSqlAccessDetail1.Settings.CacheStringExtension = ""
                    talentSqlAccessDetail1.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                    Dim sqlStatement As String = "UPDATE TBL_CONTROL_TEXT_LANG SET CONTROL_CONTENT=@ControlContent " & _
                        "WHERE LANGUAGE_CODE=@LanguageCode AND BUSINESS_UNIT=@BusinessUnit " & _
                        "AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode " & _
                        "AND CONTROL_CODE=@ControlCode AND TEXT_CODE=@TextCode"

                    talentSqlAccessDetail1.CommandElements.CommandText = sqlStatement
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@TextCode", textCode))
                    talentSqlAccessDetail1.CommandElements.CommandParameter.Add(ConstructParameter("@ControlContent", controlContent))

                    'Execute
                    If (givenTransaction Is Nothing) Then
                        err = talentSqlAccessDetail1.SQLAccess()
                    Else
                        err = talentSqlAccessDetail1.SQLAccess(givenTransaction)
                    End If
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail1.ResultDataSet Is Nothing)) Then
                        affectedRows = talentSqlAccessDetail1.ResultDataSet.Tables(0).Rows(0)(0)
                    End If
                Catch ex As Exception
                    Throw
                Finally
                    talentSqlAccessDetail1 = Nothing
                End Try

                'Return the results 
                Return affectedRows
            Else
                If Not (err.HasError) And rowCount = 0 Then
                    Try
                        'Construct The Call
                        talentSqlAccessDetail2.Settings = _settings
                        talentSqlAccessDetail2.Settings.Cacheing = False
                        talentSqlAccessDetail2.Settings.CacheStringExtension = ""
                        talentSqlAccessDetail2.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                        Dim sqlStatement As String = "INSERT INTO TBL_CONTROL_TEXT_LANG " & _
                            "([LANGUAGE_CODE],[BUSINESS_UNIT],[PARTNER_CODE],[PAGE_CODE],[CONTROL_CODE],[TEXT_CODE],[CONTROL_CONTENT]) " & _
                            "VALUES(@LanguageCode,@BusinessUnit,@PartnerCode,@PageCode,@ControlCode,@TextCode,@ControlContent)"

                        talentSqlAccessDetail2.CommandElements.CommandText = sqlStatement
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@ControlCode", controlCode))
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@TextCode", textCode))
                        talentSqlAccessDetail2.CommandElements.CommandParameter.Add(ConstructParameter("@ControlContent", controlContent))

                        'Execute
                        If (givenTransaction Is Nothing) Then
                            err = talentSqlAccessDetail2.SQLAccess()
                        Else
                            err = talentSqlAccessDetail2.SQLAccess(givenTransaction)
                        End If
                        If (Not (err.HasError)) And (Not (talentSqlAccessDetail2.ResultDataSet Is Nothing)) Then
                            affectedRows = talentSqlAccessDetail2.ResultDataSet.Tables(0).Rows(0)(0)
                        End If
                    Catch ex As Exception
                        Throw
                    Finally
                        talentSqlAccessDetail2 = Nothing
                    End Try

                    'Return the results 
                    Return affectedRows

                End If
            End If

        End Function

        Public Function GetBasketSummaryDisplaySequence(ByVal businessUnit As String, ByVal partnerCode As String, ByVal pageCode As String, ByVal controlCode As String, ByVal languageCode As String, ByVal controlTextCode As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetBasketSummaryDisplaySequence")
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'LanguageCode - Always Given
            'TextCode - Always Given
            'BusinessUnit   PartnerCode PageCode
            'Given          Given       Given
            'Given          Given       *ALL
            'Given          *ALL        Given
            'Given          *ALL        *ALL
            '*ALL           Given       Given
            '*ALL           Given       *ALL
            '*ALL           *ALL        *ALL

            Dim whereClauseFetchHierarchy(7) As String
            Dim cacheKeyHierarchyBased(7) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(2) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(pageCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT=@BusinessUnit AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(4) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE=@PageCode"
            cacheKeyHierarchyBased(4) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(pageCode)

            whereClauseFetchHierarchy(5) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE=@PartnerCode AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(5) = ToUpper(Utilities.GetAllString) & ToUpper(partnerCode) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(6) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PARTNER_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND PAGE_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(6) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PartnerCode", partnerCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PageCode", pageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LanguageCode", languageCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ControlTextCode", controlTextCode))

                Dim sqlStatement As String = "SELECT * FROM TBL_CONTROL_TEXT_LANG WHERE LANGUAGE_CODE=@LanguageCode AND TEXT_CODE=@ControlTextCode AND "
                Dim err As New ErrorObj
                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 6 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(controlTextCode) & ToUpper(languageCode) & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
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

        Public Function InsertOrUpdate(ByVal LanguageCode As String, ByVal BusinessUnit As String, ByVal PartnerCode As String, ByVal PageCode As String,
                                       ByVal Control_Code As String, ByVal Text_Code As String, ByVal Control_Content As String,
                                       Optional ByRef affectedRows As Integer = 0) As ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim errObj As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheTimeMinutes = 0
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LANGUAGE_CODE", LanguageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BUSINESS_UNIT", BusinessUnit))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PARTNER_CODE", PartnerCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PAGE_CODE", PageCode))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CONTROL_CODE", Control_Code))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TEXT_CODE", Text_Code))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CONTROL_CONTENT", Control_Content))
            Dim sqlStatement As System.Text.StringBuilder = New System.Text.StringBuilder
            sqlStatement.Append("UPDATE tbl_control_text_lang ")
            sqlStatement.Append("SET CONTROL_CONTENT = @CONTROL_CONTENT ")
            sqlStatement.Append("WHERE BUSINESS_UNIT=@BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE AND LANGUAGE_CODE=@LANGUAGE_CODE ")
            sqlStatement.Append("AND PAGE_CODE=@PAGE_CODE AND CONTROL_CODE = @CONTROL_CODE AND TEXT_CODE=@TEXT_CODE")
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
            sqlStatement.Append("INSERT INTO tbl_control_text_lang(LANGUAGE_CODE, BUSINESS_UNIT, PARTNER_CODE, ")
            sqlStatement.Append(" PAGE_CODE, CONTROL_CODE, TEXT_CODE, CONTROL_CONTENT) VALUES ")
            sqlStatement.Append("(@LANGUAGE_CODE, @BUSINESS_UNIT, @PARTNER_CODE, @PAGE_CODE, @CONTROL_CODE, @TEXT_CODE, @CONTROL_CONTENT) ")
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

#End Region

    End Class

End Namespace
