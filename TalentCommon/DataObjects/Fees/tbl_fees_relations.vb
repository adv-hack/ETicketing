Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_fees_relations based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_fees_relations
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_fees_relations"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_fees_relations" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

#End Region

#Region "Public Methods"

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
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_fees_relations WHERE ACTIVE = 1 AND PARTNER = '*ALL' AND BUSINESS_UNIT = @BusinessUnit"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", _settings.BusinessUnit))

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

        Public Function GetFulfilmentFeeCategoryList(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Dictionary(Of String, String)
            If String.IsNullOrWhiteSpace(businessUnit) Then
                businessUnit = _settings.BusinessUnit
            End If
            Dim fulfilmentFeeCategoryList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetFulfilmentFeeCategoryList") & businessUnit
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = True
                If cacheTimeMinutes <= 0 Then cacheTimeMinutes = 30
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_fees_relations WHERE ACTIVE = 1 AND PARTNER = '*ALL' AND BUSINESS_UNIT = @BusinessUnit AND FEE_APPLY_TYPE = " & GlobalConstants.FEEAPPLYTYPE_FULFILMENT
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetFulfilmentFeeCategoryList")
                If dicObject IsNot Nothing Then
                    fulfilmentFeeCategoryList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            Return fulfilmentFeeCategoryList

        End Function

        Public Function GetCardTypeFeeCategoryList(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Dictionary(Of String, String)
            If String.IsNullOrWhiteSpace(businessUnit) Then
                businessUnit = _settings.BusinessUnit
            End If
            Dim cardTypeFeeCategoryList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetCardTypeFeeCategoryList") & businessUnit
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.ModuleName = "GetCardTypeFeeCategoryList"
                talentSqlAccessDetail.Settings.Cacheing = True
                If cacheTimeMinutes <= 0 Then cacheTimeMinutes = 30
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_fees_relations WHERE ACTIVE = 1 AND PARTNER = '*ALL' AND BUSINESS_UNIT = @BusinessUnit AND FEE_APPLY_TYPE = " & GlobalConstants.FEEAPPLYTYPE_BOOKING
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetCardTypeFeeCategoryList")
                If dicObject IsNot Nothing Then
                    cardTypeFeeCategoryList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            Return cardTypeFeeCategoryList

        End Function

        Public Function GetPayTypeFeeCategoryList(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Dictionary(Of String, DEFeesRelations)
            If String.IsNullOrWhiteSpace(businessUnit) Then
                businessUnit = _settings.BusinessUnit
            End If
            Dim payTypeFeeCategoryList As New Dictionary(Of String, DEFeesRelations)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPayTypeFeeCategoryList") & businessUnit
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = True
                If cacheTimeMinutes <= 0 Then cacheTimeMinutes = 30
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_fees_relations WHERE ACTIVE = 1 AND PARTNER = '*ALL' AND BUSINESS_UNIT = @BusinessUnit AND FEE_APPLY_TYPE = " & GlobalConstants.FEEAPPLYTYPE_PAYMENTTYPE
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetPayTypeFeeCategoryList")
                If dicObject IsNot Nothing Then
                    payTypeFeeCategoryList = CType(dicObject, Dictionary(Of String, DEFeesRelations))
                End If
            Catch ex As Exception
                Throw
            End Try
            Return payTypeFeeCategoryList
        End Function

        Public Function GetConsiderCATDetailsStatusFeeCategoryList(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Dictionary(Of String, String)
            If String.IsNullOrWhiteSpace(businessUnit) Then
                businessUnit = _settings.BusinessUnit
            End If
            Dim considerCATDetailsFlagFeeCategoryList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetConsiderCATDetailsFlagFeeCategoryList") & businessUnit
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = True
                If cacheTimeMinutes <= 0 Then cacheTimeMinutes = 30
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_fees_relations WHERE ACTIVE = 1 AND PARTNER = '*ALL' AND BUSINESS_UNIT = @BusinessUnit AND FEE_APPLY_TYPE = " & GlobalConstants.FEEAPPLYTYPE_CONSIDERCATDETAILS
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetConsiderCATDetailsFlagFeeCategoryList")
                If dicObject IsNot Nothing Then
                    considerCATDetailsFlagFeeCategoryList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            Return considerCATDetailsFlagFeeCategoryList
        End Function

        Public Function GetPartPaymentFlagFeeCategoryList(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Dictionary(Of String, String)
            If String.IsNullOrWhiteSpace(businessUnit) Then
                businessUnit = _settings.BusinessUnit
            End If
            Dim partPayFlagFeeCategoryList As New Dictionary(Of String, String)
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetPartPaymentFlagFeeCategoryList") & businessUnit
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = True
                If cacheTimeMinutes <= 0 Then cacheTimeMinutes = 30
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM tbl_fees_relations WHERE ACTIVE = 1 AND PARTNER = '*ALL' AND BUSINESS_UNIT = @BusinessUnit AND FEE_APPLY_TYPE = " & GlobalConstants.FEEAPPLYTYPE_PARTPAYMENT
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim dicObject As Object = GetCustomDictionaryEntities(talentSqlAccessDetail, "GetPartPaymentFlagFeeCategoryList")
                If dicObject IsNot Nothing Then
                    partPayFlagFeeCategoryList = CType(dicObject, Dictionary(Of String, String))
                End If
            Catch ex As Exception
                Throw
            End Try
            Return partPayFlagFeeCategoryList
        End Function

        Public Function GetPartPaymentFlag(ByVal feeCategory As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim dicPartPaymentFlags As Generic.Dictionary(Of String, String) = GetPartPaymentFlagFeeCategoryList(businessUnit)
            Dim partPaymentFlagCode As String = String.Empty
            If Not dicPartPaymentFlags.TryGetValue(feeCategory, partPaymentFlagCode) Then
                partPaymentFlagCode = String.Empty
            End If
            Return partPaymentFlagCode
        End Function


        ''' <summary>
        ''' Populates the custom dictionary entities. Overridden method called from DBObjectBase
        ''' </summary>
        ''' <param name="dtSourceToPopulate">The dt source to populate.</param>
        Protected Overrides Sub PopulateCustomDictionaryEntities(ByVal dtSourceToPopulate As System.Data.DataTable, ByVal callingModuleName As String)
            If dtSourceToPopulate IsNot Nothing AndAlso dtSourceToPopulate.Rows.Count > 0 Then
                Select Case callingModuleName
                    Case "GetPayTypeFeeCategoryList"
                        Dim applyTypeCategoryList As Dictionary(Of String, DEFeesRelations) = PopulatePayTypeFeeCategoryList(dtSourceToPopulate)
                        If applyTypeCategoryList.Count > 0 Then
                            MyBase.CustomObject = applyTypeCategoryList
                        End If
                    Case "GetConsiderCATDetailsFlagFeeCategoryList"
                        Dim applyTypeCategoryList As Dictionary(Of String, String) = PopulateApplyTypeFlagForFeeCategoryList(dtSourceToPopulate)
                        If applyTypeCategoryList.Count > 0 Then
                            MyBase.CustomObject = applyTypeCategoryList
                        End If
                    Case "GetPartPaymentFlagFeeCategoryList"
                        Dim applyTypeCategoryList As Dictionary(Of String, String) = PopulateApplyTypeFlagForFeeCategoryList(dtSourceToPopulate)
                        If applyTypeCategoryList.Count > 0 Then
                            MyBase.CustomObject = applyTypeCategoryList
                        End If
                    Case Else
                        Dim applyTypeCategoryList As New Dictionary(Of String, String)
                        For rowIndex As Integer = 0 To dtSourceToPopulate.Rows.Count - 1
                            Dim keyString As String = (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("APPLY_RELATED_CODE"))).Trim.ToUpper
                            If keyString.Length > 0 Then
                                If Not applyTypeCategoryList.ContainsKey(keyString) Then
                                    applyTypeCategoryList.Add(keyString, (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("FEE_CODE"))).Trim)
                                End If
                            End If
                        Next
                        If applyTypeCategoryList.Count > 0 Then
                            MyBase.CustomObject = applyTypeCategoryList
                        End If
                End Select
            End If
        End Sub

        Private Function PopulateApplyTypeFlagForFeeCategoryList(ByVal dtSourceToPopulate As System.Data.DataTable) As Dictionary(Of String, String)
            Dim applyTypeCategoryList As New Dictionary(Of String, String)
            For rowIndex As Integer = 0 To dtSourceToPopulate.Rows.Count - 1
                Dim keyString As String = (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("FEE_CATEGORY"))).Trim.ToUpper
                If keyString.Length > 0 Then
                    If Not applyTypeCategoryList.ContainsKey(keyString) Then
                        applyTypeCategoryList.Add(keyString, Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("APPLY_RELATED_CODE")))
                    End If
                End If
            Next
            Return applyTypeCategoryList
        End Function

        Private Function PopulatePayTypeFeeCategoryList(ByVal dtSourceToPopulate As System.Data.DataTable) As Dictionary(Of String, DEFeesRelations)
            Dim applyTypeCategoryList As New Dictionary(Of String, DEFeesRelations)
            For rowIndex As Integer = 0 To dtSourceToPopulate.Rows.Count - 1
                Dim keyString As String = (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("APPLY_RELATED_CODE"))).Trim.ToUpper
                If keyString.Length > 0 Then
                    If Not applyTypeCategoryList.ContainsKey(keyString) Then
                        applyTypeCategoryList.Add(keyString, New DEFeesRelations(Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("FEE_CODE")).Trim,
                                                                (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("FEE_CATEGORY"))).Trim,
                                                                Utilities.CheckForDBNull_Int(dtSourceToPopulate.Rows(rowIndex)("FEE_APPLY_TYPE")),
                                                                (Utilities.CheckForDBNull_String(dtSourceToPopulate.Rows(rowIndex)("APPLY_RELATED_CODE"))).Trim))
                    End If
                End If
            Next
            Return applyTypeCategoryList
        End Function
#End Region

    End Class

End Namespace


