Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_creditcard based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_creditcard
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_creditcard"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_creditcard" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Private Methods"

#End Region

#Region "Public Methods"

        Public Function GetAll(ByVal cardType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim moduleName As String = "GetAll"
            Dim dtOutput As DataTable = Nothing
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner)
            Dim isCacheNotFound As Boolean = False
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Try
                Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
                If isCacheNotFound Then
                    'Construct The Call
                    talentSqlAccessDetail = New TalentDataAccess
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_creditcard] WHERE CARD_CODE=@CardCode"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardCode", cardType))
                    Dim err As New ErrorObj
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, cacheing, cacheTimeMinutes)
                    If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                        Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                    End If
                End If
                If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 Then
                    dtOutput = Me.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return dtOutput

        End Function

        ''' <summary>
        ''' Check to see if the card type can use 3D secure
        ''' </summary>
        ''' <param name="cardType">The card type we are checking</param>
        ''' <param name="cacheing">An optional boolean value to represent caching, default true</param>
        ''' <param name="cacheTimeMinutes">An optional cache time value, default 30 minss</param>
        ''' <returns>Returns true if the card type can use 3D Secure</returns>
        ''' <remarks></remarks>
        Public Function CanCardTypeUse3DSecure(ByVal cardType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean

            Dim cardCanUse3DSecure As Boolean = True

            Dim moduleName As String = "CanCardTypeUse3DSecure"
            Dim dtOutput As DataTable = Nothing
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner & cardType)
            Dim isCacheNotFound As Boolean = False
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Try
                Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
                If isCacheNotFound Then
                    'Construct The Call
                    talentSqlAccessDetail = New TalentDataAccess
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT BYPASS_3DSECURE FROM [tbl_creditcard] WHERE CARD_CODE=@CardCode"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardCode", cardType))
                    Dim err As New ErrorObj
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, cacheing, cacheTimeMinutes)
                    If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                        Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                    End If
                End If

                If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 AndAlso Me.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    cardCanUse3DSecure = Not Me.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return cardCanUse3DSecure

        End Function

        Public Function GetCardTypeCodeForVanguard(ByVal vgSchemeID As String, ByVal vgSchemeName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim cardTypeCode As String = String.Empty
            If String.IsNullOrWhiteSpace(vgSchemeID) Then vgSchemeID = ""
            If String.IsNullOrWhiteSpace(vgSchemeName) Then vgSchemeName = ""

            Dim moduleName As String = "GetCardTypeCodeForVanguard"
            Dim dtOutput As DataTable = Nothing
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner & vgSchemeID & vgSchemeName)
            Dim isCacheNotFound As Boolean = False
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Try
                Me.ResultDataSet = TryGetFromCache(Of DataSet)(isCacheNotFound, cacheing, cacheKey)
                If isCacheNotFound Then
                    'Construct The Call
                    talentSqlAccessDetail = New TalentDataAccess
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT CARD_CODE FROM [tbl_creditcard] WHERE (VG_SCHEME_ID = @VGSchemeID OR VG_SCHEME_NAME = @VGSchemeName)"
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VGSchemeID", vgSchemeID))
                    talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VGSchemeName", vgSchemeName))
                    Dim err As New ErrorObj
                    err = talentSqlAccessDetail.SQLAccess(DestinationDatabase.SQL2005, cacheing, cacheTimeMinutes)
                    If (Not (err.HasError)) AndAlso (talentSqlAccessDetail.ResultDataSet IsNot Nothing) Then
                        Me.ResultDataSet = talentSqlAccessDetail.ResultDataSet
                    End If
                End If

                If Me.ResultDataSet IsNot Nothing AndAlso Me.ResultDataSet.Tables.Count > 0 AndAlso Me.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    cardTypeCode = Utilities.CheckForDBNull_String(Me.ResultDataSet.Tables(0).Rows(0)("CARD_CODE")).ToUpper
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return cardTypeCode

        End Function

        Public Function GetCardTypeCVCAndAVS(ByVal cardType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DECVCAndAVSAuthorization

            Dim cardTypeCVCAndAVS As DECVCAndAVSAuthorization = Nothing
            Dim dicCardTypeCVCAndAVS As Dictionary(Of String, DECVCAndAVSAuthorization) = Nothing
            If (Not String.IsNullOrWhiteSpace(cardType)) Then cardType = cardType.ToUpper

            Dim moduleName As String = "GetCardTypeCVCAndAVS"
            Dim cacheKey As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, moduleName & _settings.BusinessUnit & _settings.Partner)
            Dim isCacheNotFound As Boolean = False
            Dim talentSqlAccessDetail As TalentDataAccess = Nothing
            Try
                dicCardTypeCVCAndAVS = TryGetFromCache(Of Dictionary(Of String, DECVCAndAVSAuthorization))(isCacheNotFound, cacheing, cacheKey)
                If isCacheNotFound Then
                    talentSqlAccessDetail = New TalentDataAccess
                    talentSqlAccessDetail.Settings = _settings
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKey
                    talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                    talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_creditcard]"
                    dicCardTypeCVCAndAVS = GetCustomDictionaryEntities(Of Dictionary(Of String, DECVCAndAVSAuthorization))(DestinationDatabase.SQL2005, cacheing, cacheTimeMinutes, talentSqlAccessDetail, moduleName)
                End If

                If dicCardTypeCVCAndAVS IsNot Nothing AndAlso dicCardTypeCVCAndAVS.Count > 0 AndAlso dicCardTypeCVCAndAVS.ContainsKey(cardType) Then
                    cardTypeCVCAndAVS = dicCardTypeCVCAndAVS(cardType)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return cardTypeCVCAndAVS

        End Function

#End Region

#Region "Protected Methods"
        ''' <summary>
        ''' Populates the custom dictionary entities. Overridden method called from DBObjectBase
        ''' </summary>
        ''' <param name="dtSourceToPopulate">The dt source to populate.</param>
        Protected Overrides Sub PopulateCustomDictionaryEntities(ByVal dtSourceToPopulate As System.Data.DataTable, ByVal callingModuleName As String)
            If dtSourceToPopulate IsNot Nothing AndAlso dtSourceToPopulate.Rows.Count > 0 Then
                Select Case callingModuleName
                    Case "GetCardTypeCVCAndAVS"
                        Dim dicCardTypeCVCAndAVS As Dictionary(Of String, DECVCAndAVSAuthorization) = PopulateCardTypeCVCAndAVS(dtSourceToPopulate)
                        If dicCardTypeCVCAndAVS IsNot Nothing AndAlso dicCardTypeCVCAndAVS.Count > 0 Then
                            MyBase.CustomObject = dicCardTypeCVCAndAVS
                        End If
                End Select
            End If
        End Sub
#End Region

#Region "Private Methods"
        Private Function PopulateCardTypeCVCAndAVS(ByVal dtCardTypeCVCAndAVS As DataTable) As Dictionary(Of String, DECVCAndAVSAuthorization)
            Dim dicCardTypeCVCAndAVS As New Dictionary(Of String, DECVCAndAVSAuthorization)
            For rowIndex As Integer = 0 To dtCardTypeCVCAndAVS.Rows.Count - 1
                Dim keyString As String = (Utilities.CheckForDBNull_String(dtCardTypeCVCAndAVS.Rows(rowIndex)("CARD_CODE"))).Trim.ToUpper
                If keyString.Length > 0 Then
                    If Not dicCardTypeCVCAndAVS.ContainsKey(keyString) Then
                        Dim tempDECVCAndAVSAuthorization As New DECVCAndAVSAuthorization
                        tempDECVCAndAVSAuthorization.CVCEnabled = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("CVC_ENABLED"))
                        tempDECVCAndAVSAuthorization.CVCMandatory = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("CVC_MANDATORY"))
                        If tempDECVCAndAVSAuthorization.CVCEnabled Then
                            tempDECVCAndAVSAuthorization.CVCAccept_Matched = True
                        End If
                        tempDECVCAndAVSAuthorization.AVS_Addr_Enabled = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_ADDR_ENABLED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_Addr_Matched = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_ADDR_MATCHED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_Addr_NotChecked = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_ADDR_NOTCHECKED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_Addr_NotMatched = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_ADDR_NOTMATCHED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_Addr_NotProvided = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_ADDR_NOTPROVIDED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_Addr_PartialMatch = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_ADDR_PARTIALMATCH"))
                        tempDECVCAndAVSAuthorization.AVS_PC_Enabled = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_PC_ENABLED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_PC_Matched = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_PC_MATCHED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_PC_NotChecked = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_PC_NOTCHECKED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_PC_NotMatched = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_PC_NOTMATCHED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_PC_NotProvided = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_PC_NOTPROVIDED"))
                        tempDECVCAndAVSAuthorization.AVSAccept_PC_PartialMatch = Utilities.CheckForDBNull_Boolean_DefaultFalse(dtCardTypeCVCAndAVS.Rows(rowIndex)("AVS_PC_PARTIALMATCH"))
                        dicCardTypeCVCAndAVS.Add(keyString, tempDECVCAndAVSAuthorization)
                    End If
                End If
            Next
            Return dicCardTypeCVCAndAVS
        End Function
#End Region

#Region "unused to delete"

        ''' <summary>
        ''' Check to see if the card type can use 3D secure
        ''' </summary>
        ''' <param name="cardType">The card type we are checking</param>
        ''' <param name="cacheing">An optional boolean value to represent caching, default true</param>
        ''' <param name="cacheTimeMinutes">An optional cache time value, default 30 minss</param>
        ''' <returns>Returns true if the card type can use 3D Secure</returns>
        ''' <remarks></remarks>
        Private Function CanCardTypeUse3DSecureOLD(ByVal cardType As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Boolean
            Dim cardCanUse3DSecure As Boolean = True
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "CanCardTypeUse3DSecure-" & cardType)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CardCode", cardType))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT BYPASS_3DSECURE FROM [tbl_creditcard] WHERE CARD_CODE=@CardCode"

                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    cardCanUse3DSecure = Not talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return cardCanUse3DSecure
        End Function

        Private Function GetCardTypeCodeForVanguardOLD(ByVal vgSchemeID As String, ByVal vgSchemeName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As String

            If String.IsNullOrWhiteSpace(vgSchemeID) Then vgSchemeID = ""
            If String.IsNullOrWhiteSpace(vgSchemeName) Then vgSchemeName = ""

            Dim cardTypeCode As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetCardTypeCodeForVG" & _settings.BusinessUnit & _settings.Partner & vgSchemeID & vgSchemeName)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim tempSettingsCache As Boolean = False
            Try
                'Construct The Call
                tempSettingsCache = _settings.Cacheing
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                If talentSqlAccessDetail.Settings.Cacheing Then
                    If (_settings.CacheTimeMinutes <= 0 AndAlso _settings.CacheTimeSeconds <= 0) Then
                        talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                    End If
                End If
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT CARD_CODE FROM [tbl_creditcard] WHERE (VG_SCHEME_ID = @VGSchemeID OR VG_SCHEME_NAME = @VGSchemeName)"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VGSchemeID", vgSchemeID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@VGSchemeName", vgSchemeName))
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    cardTypeCode = Utilities.CheckForDBNull_String(talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)("CARD_CODE")).ToUpper
                End If

            Catch ex As Exception
                Throw
            Finally
                _settings.Cacheing = tempSettingsCache
                talentSqlAccessDetail = Nothing
            End Try
            Return cardTypeCode
        End Function

#End Region

    End Class
End Namespace