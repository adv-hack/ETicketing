Imports System.Data.SqlClient

' This Enumeration is outside the namespace DataObjects to make sure that it is accessed 
' whereever Talent.Common namespace is refered
#Region "Enumerations"
''' <summary>
''' Each memeber in this enum is value for the column DEFAULT_NAME in the table tbl_ecommerce_module_defaults_bu
''' </summary>
Public Enum TblEcommerceModuleDefaultsBuEnum
    ''' <summary>
    ''' Abolute path for cache
    ''' ex: C:\foldername\foldername\.....
    ''' </summary>
    CACHE_DEPENDENCY_PATH
    ''' <summary>
    ''' Valid email address
    ''' </summary>
    CONTACT_US_FROM_EMAIL
    ''' <summary>
    ''' Valid email address for receipient
    ''' </summary>
    CONTACT_US_TO_EMAIL
    ''' <summary>
    ''' Valid email address for cc
    ''' </summary>
    CONTACT_US_TO_EMAIL_CC
    ''' <summary>
    ''' Valid email address
    ''' </summary>
    CUSTOMER_SERVICES_EMAIL
    ''' <summary>
    ''' Valid email address
    ''' </summary>
    DEFAULT_TICKETING_EMAIL_ADDRESS
    ''' <summary>
    ''' New Business Unit Stadium name
    ''' </summary>
    FLASH_STADIUM_NAME
    ''' <summary>
    ''' Abolute path for html files
    ''' ex: C:\foldername\foldername\.....
    ''' </summary>
    HTML_PATH_ABSOLUTE
    ''' <summary>
    ''' Abolute path for image files
    ''' ex: C:\foldername\foldername\.....
    ''' </summary>
    IMAGE_PATH_ABSOLUTE
    ''' <summary>
    ''' key for encrypting the NOISE service
    ''' </summary>
    NOISE_ENCRYPTION_KEY
    ''' <summary>
    ''' URL for NOISE service
    ''' </summary>
    NOISE_URL
    ''' <summary>
    ''' Valid email address
    ''' </summary>
    REGISTRATION_CONFIRMATION_FROM_EMAIL
    ''' <summary>
    ''' valid group name
    ''' </summary>
    STORED_PROCEDURE_GROUP
    ''' <summary>
    ''' valid theme name
    ''' </summary>
    THEME
    ''' <summary>
    ''' Ticketing stadium name
    ''' </summary>
    TICKETING_STADIUM
    ''' <summary>
    ''' Authority user profile - Backend Internet user
    ''' </summary>
    AUTHORITY_USER_PROFILE
    ''' <summary>
    ''' Payment 3D Secure Attrubute 1
    ''' </summary>
    PAYMENT_3DSECURE_DETAILS_1
    ''' <summary>
    ''' Payment 3D Secure Attrubute 2
    ''' </summary>
    PAYMENT_3DSECURE_DETAILS_2
    ''' <summary>
    ''' Payment 3D Secure Attrubute 3
    ''' </summary>
    PAYMENT_3DSECURE_DETAILS_3
    ''' <summary>
    ''' Payment 3D Secure, to pass the enrollment check if the return code is U (boolean value)
    ''' </summary>
    PAYMENT_3DSECURE_PASS_IF_ENROLLED_CODE_U
    ''' <summary>
    ''' Payment 3D Secure, to pass the enrollment check if the response times out (boolean value)
    ''' </summary>
    PAYMENT_3DSECURE_PASS_IF_RESPONSE_TIMEOUT
End Enum
#End Region

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_ecommerce_module_defaults_bu based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_ecommerce_module_defaults_bu
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_ecommerce_module_defaults_bu"

        ''' <summary>
        ''' To decide to continue the loop of dictionary object in UpdateMultiple method
        ''' </summary>
        Private _continueUpdateMultiple As Boolean = True

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_ecommerce_module_defaults_bu" /> class.
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
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "DELETE TBL_ECOMMERCE_MODULE_DEFAULTS_BU " & _
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
        ''' Copies the records from one business unit to another business unit inside the table tbl_ecommerce_module_defaults_bu
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
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery

                Dim sqlStatement As String = "INSERT INTO TBL_ECOMMERCE_MODULE_DEFAULTS_BU (" & _
                    "BUSINESS_UNIT, APPLICATION, DEFAULT_NAME, " & _
                    "MODULE, PARTNER, VALUE) " & _
                    "SELECT " & _
                    "@ToBusinessUnit As BUSINESS_UNIT, APPLICATION, DEFAULT_NAME, " & _
                    "MODULE, PARTNER, VALUE " & _
                    "FROM TBL_ECOMMERCE_MODULE_DEFAULTS_BU " & _
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
        ''' Gets the business unit records based on the given business unit.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByBU(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBU")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_ECOMMERCE_MODULE_DEFAULTS_BU Where BUSINESS_UNIT=@BusinessUnitName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))

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

        Public Function GetDefaultValues(ByVal partner As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBU" & partner)
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From TBL_ECOMMERCE_MODULE_DEFAULTS_BU Where BUSINESS_UNIT=@BusinessUnitName AND PARTNER=@Partner"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))

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
        ''' Gets a single VALUE from  the given BUSINESS_UNIT and DEFAULT_NAME.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetValueByDefaultNameAndBU(ByVal businessUnit As String, ByVal defaultName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim value As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetValueByDefaultNameAndBU")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & defaultName.ToUpper
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP 1 [VALUE] FROM [TBL_ECOMMERCE_MODULE_DEFAULTS_BU] WHERE [BUSINESS_UNIT] = @BusinessUnitName AND [DEFAULT_NAME] = @DefaultName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultName", defaultName))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 Then
                        If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                            value = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0).Item(0).ToString
                        End If
                    End If

                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return value

        End Function

        Public Function GetDefaultNameValue(ByVal businessUnit As String, ByVal partner As String, ByVal defaultName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim value As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDefaultNameValue")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(businessUnit) & ToUpper(partner) & defaultName.ToUpper
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP 1 [VALUE] FROM [TBL_ECOMMERCE_MODULE_DEFAULTS_BU] WHERE [BUSINESS_UNIT] = @BusinessUnitName AND [PARTNER] = @Partner AND [DEFAULT_NAME] = @DefaultName"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnitName", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Partner", partner))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultName", defaultName))

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    If talentSqlAccessDetail.ResultDataSet.Tables.Count > 0 Then
                        If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                            value = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0).Item(0).ToString
                        End If
                    End If

                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return value

        End Function

        ''' <summary>
        ''' Updates the value for the specified business unit and default name
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="defaultName">The default name.</param>
        ''' <param name="defaultValue">The default value.</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>No of affected rows</returns>
        Public Function UpdateByBUAndDefaultName(ByVal businessUnit As String, ByVal defaultName As String, ByVal defaultValue As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer

            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                Dim sqlStatement As String = "UPDATE TBL_ECOMMERCE_MODULE_DEFAULTS_BU " & _
                    "SET [VALUE] = @DefaultValue " & _
                    "WHERE BUSINESS_UNIT=@BusinessUnit AND DEFAULT_NAME = @DefaultName"

                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultName", defaultName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@DefaultValue", defaultValue))


                'Execute
                Dim err As New ErrorObj
                If (givenTransaction Is Nothing) Then
                    err = talentSqlAccessDetail.SQLAccess()
                Else
                    err = talentSqlAccessDetail.SQLAccess(givenTransaction)
                End If
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                Else
                    ' this is to decide whether to continue the for loop in updatemultiple method
                    'if any error exit the for loop
                    _continueUpdateMultiple = False
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
        ''' Updates the default name of the multiple by BU and.
        ''' </summary>
        ''' <param name="businessUnit">The business unit.</param>
        ''' <param name="defaultNamesAndValues">The default names and values generic dictionary collection</param>
        ''' <param name="givenTransaction">The given transaction.</param>
        ''' <returns>total number of affected rows</returns>
        Public Function UpdateMultipleByBUAndDefaultName(ByVal businessUnit As String, ByVal defaultNamesAndValues As Dictionary(Of TblEcommerceModuleDefaultsBuEnum, String), Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
            Dim totalAffectedRows As Integer = 0

            'Loops the dictionary if exists with key value pairs
            If Not (defaultNamesAndValues Is Nothing) Then
                If (defaultNamesAndValues.Count > 0) Then
                    _continueUpdateMultiple = True
                    Dim keyValue As KeyValuePair(Of TblEcommerceModuleDefaultsBuEnum, String)
                    Dim affectedRows As Integer = 0
                    For Each keyValue In defaultNamesAndValues
                        'to make sure there is no error in the UpdateByBUAndDefaultName method
                        If (_continueUpdateMultiple) Then
                            affectedRows = UpdateByBUAndDefaultName(businessUnit, System.Enum.GetName(GetType(TblEcommerceModuleDefaultsBuEnum), keyValue.Key), keyValue.Value, givenTransaction)
                            totalAffectedRows = totalAffectedRows + affectedRows
                        Else
                            Exit For
                        End If
                    Next
                End If
            End If
            'Return the results 
            Return totalAffectedRows
        End Function

#End Region

    End Class

End Namespace
