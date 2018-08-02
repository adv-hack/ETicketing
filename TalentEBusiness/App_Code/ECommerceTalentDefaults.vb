Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common

Namespace Talent.eCommerce

    Public Class ECommerceTalentDefaults
        Public Class DefaultValues
            Public Property IsOrderLevelFulfilmentEnabled() As Boolean = False
            Public Property PriceBreaksEnabled() As Boolean = False
            Public Property TicketExchangeEnabled() As Boolean = False
            Public Property IsDespatchInPrintMode() As Boolean = False
            Public Property ParentalConsentCeiling() As Integer = 0
            Public Property ParentalConsentCeilingDoB() As Date = Today
        End Class

        Public Function GetDefaults(ByVal partner As String, ByVal businessUnit As String) As DefaultValues
            Dim talentDefaultValues As New DefaultValues
            talentDefaultValues = GetDefaultValues(partner, businessUnit)
            Return talentDefaultValues
        End Function

        Public Function GetDefaults() As DefaultValues
            Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
            Dim businessUnit As String = TalentCache.GetBusinessUnit()
            Dim talentDefaultValues As New DefaultValues
            talentDefaultValues = GetDefaultValues(partner, businessUnit)
            Return talentDefaultValues
        End Function

        Private Function GetDefaultValues(ByVal partner As String, ByVal businessUnit As String) As DefaultValues
            ' Declare this first! Used for Logging function duration
            Dim timeSpan As TimeSpan = Now.TimeOfDay
            Dim def As New DefaultValues
            Dim cacheKey As String = "ECommerceTalentDefaults" & Talent.Common.Utilities.FixStringLength(businessUnit, 50) & Talent.Common.Utilities.FixStringLength(partner, 50)

            If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                def = CType(HttpContext.Current.Cache.Item(cacheKey), DefaultValues)
            Else
                Dim dtTalentDefaults As DataTable = Nothing
                Dim err As New ErrorObj
                Dim talDefaults As New TalentDefaults
                talDefaults.Settings = TEBUtilities.GetSettingsObject
                talDefaults.Settings.Cacheing = True
                talDefaults.Settings.CacheTimeMinutes = 30
                err = talDefaults.RetrieveTalentDefaults()

                If talDefaults.ResultDataSet IsNot Nothing AndAlso talDefaults.ResultDataSet.Tables("TalentDefaults") IsNot Nothing Then
                    dtTalentDefaults = talDefaults.ResultDataSet.Tables("TalentDefaults")
                    def = PopulateDefaults(def, dtTalentDefaults)
                End If

                ' Add to cache
                HttpContext.Current.Cache.Insert(cacheKey, def, Nothing, System.DateTime.Now.AddMinutes(CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes"))), Caching.Cache.NoSlidingExpiration)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)

            End If
            Utilities.TalentLogging.LoadTestLog("ECommerceTalentDefaults.vb", "GetDefaultValues", timeSpan)
            Return def
        End Function

        Private Function PopulateDefaults(ByVal def As DefaultValues, ByVal dtTalentDefaults As DataTable) As DefaultValues
            If dtTalentDefaults IsNot Nothing AndAlso dtTalentDefaults.Rows.Count > 0 Then
                def.IsOrderLevelFulfilmentEnabled = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtTalentDefaults.Rows(0)("OrderLevelFulfilment"))
                def.PriceBreaksEnabled = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtTalentDefaults.Rows(0)("PriceBreaksEnabled"))
                def.TicketExchangeEnabled = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtTalentDefaults.Rows(0)("TicketExchangeEnabled"))
                def.IsDespatchInPrintMode = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtTalentDefaults.Rows(0)("DespatchPrintEnabled"))
                def.ParentalConsentCeiling = TEBUtilities.CheckForDBNull_Int(dtTalentDefaults.Rows(0)("ParentalConsentCeiling"))
                def.ParentalConsentCeilingDoB = Today.AddYears(def.ParentalConsentCeiling * -1)
            End If
            Return def
        End Function

    End Class
End Namespace
