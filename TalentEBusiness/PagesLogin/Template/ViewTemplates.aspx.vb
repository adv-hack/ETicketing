Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data

Partial Class PagesLogin_Template_ViewTemplates
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty

#End Region

#Region "Public Properties"

    Public OrderTemplateForText As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .PageCode = TEBUtilities.GetCurrentPageName
        End With
        _languageCode = TCUtilities.GetDefaultLanguage()
        OrderTemplatesHeader1.CustomerNumber = Profile.UserName

        If ModuleDefaults.FriendsAndFamily AndAlso ModuleDefaults.ShowFFOrderTemplates Then
            OrderTemplateForText = _wfrPage.Content("OrderTemplateFor", _languageCode, True)
            rptOrderTemplatesForFandF.DataSource = getFriendsAndFamilyDataTable()
            rptOrderTemplatesForFandF.DataBind()
        Else
            rptOrderTemplatesForFandF.Visible = False
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Retrieve the friends and family data for the current logged in customer
    ''' </summary>
    ''' <returns>A data table of friends and family customers</returns>
    ''' <remarks></remarks>
    Private Function getFriendsAndFamilyDataTable() As DataTable
        Dim dtFriendsAndFamily As New DataTable
        Dim err As New Talent.Common.ErrorObj
        Dim customer As New Talent.Common.TalentCustomer
        Dim deCustV11 As New Talent.Common.DECustomerV11
        Dim deCustV1 As New Talent.Common.DECustomer

        deCustV11.DECustomersV1.Add(deCustV1)
        With customer
            .DeV11 = deCustV11
            .Settings = TEBUtilities.GetSettingsObject()
            .Settings.Cacheing = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("FAndFCaching"))
            .Settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("FAndFCacheTimeInMins"))
            deCustV1.CustomerNumber = Profile.User.Details.LoginID.ToString()
            deCustV1.Source = GlobalConstants.SOURCE
            err = .CustomerAssociations
        End With

        If Not err.HasError AndAlso customer.ResultDataSet.Tables.Count = 2 Then
            dtFriendsAndFamily = customer.ResultDataSet.Tables("FriendsAndFamily")
        End If
        Return dtFriendsAndFamily
    End Function

#End Region

End Class