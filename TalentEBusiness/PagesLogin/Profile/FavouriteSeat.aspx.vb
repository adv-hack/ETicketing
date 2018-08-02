Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports Talent.eCommerce
Imports TEUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_FavouriteSeat
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _product As New TalentProduct
    Private _settings As New DESettings
    Private _standAndAreaDescriptions As New DataTable

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "FavouriteSeat.aspx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim defaults As New ECommerceModuleDefaults
        Dim hasNoFavouriteSeatDetails As Boolean = True
        If ModuleDefaults.FavouriteSeatFunction And Not Profile.IsAnonymous Then
            Try
                If Profile.User.Details.Favourite_Seat.Trim().Length > 0 Then
                    hasNoFavouriteSeatDetails = False
                    retrieveStandAndAreaDescriptions()
                    getStandAndAreaDescriptions(TEUtilities.GetStandFromSeatDetails(Profile.User.Details.Favourite_Seat).Trim, TEUtilities.GetAreaFromSeatDetails(Profile.User.Details.Favourite_Seat).Trim)
                    ltlFavouriteRowValue.Text = TEUtilities.GetRowFromSeatDetails(Profile.User.Details.Favourite_Seat).Trim
                    ltlFavouriteSeatValue.Text = TEUtilities.GetSeatFromSeatDetails(Profile.User.Details.Favourite_Seat).TrimStart("0")
                    ltlFavouriteStandLabel.Text = _wfr.Content("FavouriteStandLabel", _languageCode, True)
                    ltlFavouriteAreaLabel.Text = _wfr.Content("FavouriteAreaLabel", _languageCode, True)
                    ltlFavouriteRowLabel.Text = _wfr.Content("FavouriteRowLabel", _languageCode, True)
                    ltlFavouriteSeatLabel.Text = _wfr.Content("FavouriteSeatLabel", _languageCode, True)
                    ltlFavouriteSeatIntro.Text = _wfr.Content("FavouriteSeatIntroText", _languageCode, True)
                    If ltlFavouriteSeatIntro.Text.Length > 0 Then plhFavouriteSeatIntro.Visible = True
                    plhFavouriteSeatDetails.Visible = True
                    plhNoFavouriteSeatDetails.Visible = False
                End If
            Catch ex As Exception
                hasNoFavouriteSeatDetails = True
            End Try
        End If

        If hasNoFavouriteSeatDetails Then
            plhNoFavouriteSeatDetails.Visible = True
            plhFavouriteSeatDetails.Visible = False
            ltlNoFavouriteSeatDetailsMessage.Text = _wfr.Content("NoFavouriteSeatDetailsAvailable", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Call TALENT to retrieve the stand and area descriptions
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub retrieveStandAndAreaDescriptions()
        Dim err As New ErrorObj
        _settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _settings.BusinessUnit = TalentCache.GetBusinessUnit()
        _settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        _settings.Cacheing = CType(_wfr.Attribute("Cacheing"), Boolean)
        _settings.CacheTimeMinutes = CType(_wfr.Attribute("CacheTimeMinutes"), Integer)
        _settings.CacheTimeMinutesSecondFunction = CType(_wfr.Attribute("StandDescriptionsCacheTimeMinutes"), Integer)
        _settings.OriginatingSource = TEUtilities.GetOriginatingSource(Session.Item("Agent"))
        _product.Settings = _settings

        _product.De.StadiumCode = TDataObjects.StadiumSettings.TblStadiums.GetFavouriteSeatStadiumCode(_settings.BusinessUnit)
        If _product.De.StadiumCode IsNot Nothing AndAlso _product.De.StadiumCode.Length > 0 Then
            _product.ResultDataSet = Nothing
            err = _product.StandDescriptions()
            If Not err.HasError AndAlso _product.ResultDataSet IsNot Nothing Then
                If _product.ResultDataSet.Tables.Count > 1 Then
                    If _product.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(_product.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            If _product.ResultDataSet.Tables(1).Rows.Count > 0 Then
                                _standAndAreaDescriptions.Merge(_product.ResultDataSet.Tables(1))
                            End If
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Gets the stand description based on the given stand code
    ''' </summary>
    ''' <param name="standCode">The stand code as string</param>
    ''' <param name="areaCode">The area code as string</param>
    ''' <remarks></remarks>
    Private Sub getStandAndAreaDescriptions(ByVal standCode As String, ByVal areaCode As String)
        If Not String.IsNullOrWhiteSpace(standCode) AndAlso Not String.IsNullOrWhiteSpace(areaCode) Then
            For Each row As DataRow In _standAndAreaDescriptions.Rows
                If row("StandCode").ToString.Equals(standCode) AndAlso row("AreaCode").ToString.Equals(areaCode) Then
                    ltlFavouriteStandValue.Text = row("StandDescription").ToString.Trim
                    ltlFavouriteAreaValue.Text = row("AreaDescription").ToString.Trim
                    Exit For
                End If
            Next
        End If
    End Sub

#End Region

End Class
