Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports System.Text

<Serializable()> _
Public Class TalentPackage
    Inherits TalentBase

    Public Property DePackages() As New DEPackages
    Public Property ResultDataSet() As DataSet
    Private Property DBPack() As New DBPackage
    Private Property err As New ErrorObj

    Const MODULEGETCOMPONENTGROUPLIST As String = "GetComponentGroupList"
    Const MODULEGETCOMPONENTLIST As String = "GetComponentList"
    Const MODULEGETCOMPONENTDETAILS As String = "GetComponentDetails"
    Const MODULEGETCOMPONENTGROUPDETAILS As String = "GetComponentGroupDetails"
    Const MODULEADDEDITDELETECOMPONENTGROUP As String = "AddEditDeleteComponentGroup"
    Const MODULEADDEDITDELETECOMPONENT As String = "AddEditDeleteComponent"
    Const MODULEUPDATECUSTOMERCOMPONENTDETAILS As String = "UpdateCustomerComponentDetails"
    Const MODULEDELETECUSTOMERPACKAGE As String = "DeleteCustomerPackage"
    Const MODULEGETCUSTOMERPACKAGEINFORMATION As String = "GetCustomerPackageInformation"
    Const MODULEUPDATETRAVELANDACCOMODATIONCOMPONENTDETAILS As String = "UpdateTravelAndAccomodationComponentDetails"
    Const MODULEGETCOMPONENTSEATS As String = "GetComponentSeats"
    Const MODULEGETHOSPITALITYBOOKINGS As String = "GetHospitalityBookings"
    Const MODULEGETPAACKAGESEATDETAILS As String = "GetPackageSeatDetails"
    Const MODULEUPDATEHOSPITALITYBOOKINGSTATUS As String = "UpdateHospitalityBookingStatus"
    Const MODULEGETSOLDHOSPITALITYBOOKINGDETAILS As String = "GetSoldHospitalityBookingDetails"
    Const MODULEPRINTHOSPITALITYBOOKINGS As String = "PrintHospitalityBookings"
    Const MODULECREATEHOSPITALITYBOOKINGDOCUMENT As String = "CreateHospitalityBookingDocument"

    Public Function GetPackageClearCache() As Boolean
        RemoveItemFromCache("Packages")
    End Function

    Public Function GetCustomerPackageSessionKey(ByVal packageId As Long, ByVal productCode As String, ByVal callId As Long) As String
        Return MODULEGETCUSTOMERPACKAGEINFORMATION & packageId.ToString().Trim() & productCode.Trim & callId.ToString().Trim() & Settings.BusinessUnit.Trim()
    End Function

    ''' <summary>
    ''' Remove the package details that are saved in session. The customer will typically have two sessions:
    ''' 1. the package ID and product code which is the current active basket
    ''' 2. the package ID and product code and call ID which is a combination of the above and also any potential enquiries being looked at
    ''' </summary>
    ''' <param name="packageId">The package ID value</param>
    ''' <param name="productCode">The ticketing product code</param>
    ''' <param name="callId">The booking reference call id</param>
    ''' <remarks></remarks>
    Public Sub RemoveCustomerPackageSession(ByVal packageId As Long, ByVal productCode As String, ByVal callId As Long)
        RemoveItemFromSession(GetCustomerPackageSessionKey(packageId.ToString().Trim(), productCode, callId.ToString().Trim()))
        RemoveItemFromSession(GetCustomerPackageSessionKey(packageId.ToString().Trim(), productCode, "0"))
    End Sub

    Public Function GetComponentDetailsCacheKey() As String
        Return MODULEGETCOMPONENTDETAILS & DePackages.ComponentGroupID & DePackages.ComponentID & DePackages.ProductCode & DePackages.AvailableToSell03 & DePackages.AvailableToSellAvailableTickets
    End Function

    Public Sub RemoveComponentDetailsCache()
        RemoveItemFromCache(GetComponentDetailsCacheKey())
    End Sub

    Public Shared Function RedirectUrl(ByVal Mode As Integer, ByVal ProductCode As String, ByVal PackageId As Long, ByVal LastStage As Boolean, ByVal Stage As Integer, ByVal productSubType As String) As String

        Dim url As New StringBuilder

        Try
            If LastStage And Mode = RedirectMode.Proceed Then
                url.Append("~/PagesPublic/Basket/Basket.aspx")
            Else
                url.Append(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path) & "?product=" & ProductCode & "&PackageId=" & PackageId.ToString)
                If Mode = RedirectMode.Back Then
                    url.Append("&Stage=" & (Stage - 1).ToString)
                Else
                    url.Append("&Stage=" & (Stage + 1).ToString)
                End If
                url.Append("&productsubtype=").Append(productSubType)
            End If
        Catch ex As Exception
        End Try

        Return url.ToString
    End Function

    Public Enum RedirectMode
        Proceed
        Back
    End Enum

    Public Function GetComponentGroupList() As ErrorObj
        Settings.ModuleName = MODULEGETCOMPONENTGROUPLIST
        Dim cacheKey As String = Settings.ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
            With DBPack
                .DePack = DePackages
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "Packages")
                End If
            End With
        End If
        Return err
    End Function

    Public Function GetComponentList() As ErrorObj
        Settings.ModuleName = MODULEGETCOMPONENTLIST
        Dim cacheKey As String = Settings.ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
            With DBPack
                .DePack = DePackages
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "Packages")
                End If
            End With
        End If
        Return err
    End Function

    Public Function GetComponentDetails() As ErrorObj
        Settings.ModuleName = MODULEGETCOMPONENTDETAILS
        Dim cacheKey As String = GetComponentDetailsCacheKey()
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
            With DBPack
                .DePack = DePackages
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "Packages")
                End If
            End With
        End If

        Return err
    End Function

    Public Function GetComponentGroupDetails() As ErrorObj
        Settings.ModuleName = MODULEGETCOMPONENTGROUPDETAILS
        Dim cacheKey As String = Settings.ModuleName & Settings.Company & DePackages.ComponentGroupID
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
            With DBPack
                .DePack = DePackages
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "Packages")
                End If
            End With
        End If
        Return err
    End Function

    Public Function AddEditDeleteComponentGroup() As ErrorObj
        Settings.ModuleName = MODULEADDEDITDELETECOMPONENTGROUP
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        With DBPack
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                Dim cacheKey As String = MODULEGETCOMPONENTGROUPDETAILS & Settings.Company & DePackages.ComponentGroupID
                If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    RemoveItemFromCache(cacheKey)
                End If
            End If
        End With
        Return err
    End Function

    Public Function AddEditDeleteComponent() As ErrorObj
        Settings.ModuleName = MODULEADDEDITDELETECOMPONENT
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        With DBPack
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function DeleteCustomerPackage() As ErrorObj
        Settings.ModuleName = MODULEDELETECUSTOMERPACKAGE
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        With DBPack
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                If ResultDataSet.Tables.Count >= 5 Then
                    RemoveCustomerPackageSession(DePackages.PackageID, DePackages.ProductCode, DePackages.CallId)
                Else
                    RemoveCustomerPackageSession(DePackages.PackageID, DePackages.ProductCode, DePackages.CallId)
                End If

                ' We need to refresh the basket.  We already have the first 10 basket items returned from WS036R via WS622R.
                ' These values are populated in the WS036RFirstParm property
                Dim basket As New Talent.Common.TalentBasket
                With basket
                    .Settings = Settings
                    .De.SessionId = DePackages.BasketId
                    .De.CustomerNo = Settings.LoginId
                    .ResultDataSet = Nothing
                    .WS036RFirstParm = DBPack.WS036RFirstParm
                    .De.Src = Settings.OriginatingSourceCode
                    err = .RetrieveTicketingItems
                End With
            End If
        End With
        Return err
    End Function

    Public Function UpdateCustomerComponentDetails(Optional doRetrieveBasket As Boolean = True) As ErrorObj
        Settings.ModuleName = MODULEUPDATECUSTOMERCOMPONENTDETAILS
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        With DBPack
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                Dim productCode As String = DePackages.ProductCode
                If Not String.IsNullOrWhiteSpace(DePackages.TicketingProductCode) Then
                    productCode = DePackages.TicketingProductCode
                End If
                If ResultDataSet.Tables.Count = 4 Then
                    AddItemToSession(GetCustomerPackageSessionKey(DePackages.PackageID, productCode, DePackages.CallId), ResultDataSet)
                Else
                    RemoveCustomerPackageSession(DePackages.PackageID, productCode, DePackages.CallId)
                End If

                If doRetrieveBasket Then
                    ' We need to refresh the basket.  We already have the first 10 basket items returned from WS036R via WS622R.
                    ' These values are populated in the WS036RFirstParm property
                    Dim basket As New Talent.Common.TalentBasket
                    With basket
                        .Settings = Settings
                        .De.SessionId = DePackages.BasketId
                        .De.CustomerNo = Settings.LoginId
                        .ResultDataSet = Nothing
                        .WS036RFirstParm = DBPack.WS036RFirstParm
                        .De.Src = Settings.OriginatingSourceCode
                        err = .RetrieveTicketingItems
                    End With
                End If
            End If
        End With
        Return err
    End Function

    Public Function UpdateCustomerComponentDetails(ByVal settingsObj As DESettings, ByVal packageDetails As DEPackages) As ErrorObj
        Settings = settingsObj
        DePackages = packageDetails
        err = UpdateCustomerComponentDetails()
        Return err
    End Function

    Public Function UpdateTravelAndAccomodationComponentDetails() As ErrorObj
        Settings.ModuleName = MODULEUPDATETRAVELANDACCOMODATIONCOMPONENTDETAILS
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        With DBPack
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                If ResultDataSet.Tables.Count >= 5 Then
                    AddItemToSession(GetCustomerPackageSessionKey(DePackages.PackageID, DePackages.TicketingProductCode, DePackages.CallId), ResultDataSet)
                Else
                    ' Remove the session
                    RemoveCustomerPackageSession(DePackages.PackageID, DePackages.TicketingProductCode, DePackages.CallId)
                End If

                ' We need to refresh the basket.  We already have the first 10 basket items returned from WS036R via WS622R.
                ' These values are populated in the WS036RFirstParm property
                Dim basket As New Talent.Common.TalentBasket
                With basket
                    .Settings = Settings
                    .De.SessionId = DePackages.BasketId
                    .De.CustomerNo = Settings.LoginId
                    .ResultDataSet = Nothing
                    .WS036RFirstParm = DBPack.WS036RFirstParm
                    .De.Src = Settings.OriginatingSourceCode
                    err = .RetrieveTicketingItems
                End With
            End If
        End With
        Return err
    End Function

    Public Function UpdateTravelAndAccomodationComponentDetails(ByVal settingsObj As DESettings, ByVal packageDetails As DEPackages) As ErrorObj
        Settings = settingsObj
        DePackages = packageDetails
        err = UpdateTravelAndAccomodationComponentDetails()
        Return err
    End Function


    Public Function GetCustomerPackageInformation(Optional CheckPrice As Boolean = False, Optional packageBasketPrice As Decimal = 0.0, Optional refreshBasket As Boolean = False) As ErrorObj
        Settings.ModuleName = MODULEGETCUSTOMERPACKAGEINFORMATION
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        Dim sessionKey As String = GetCustomerPackageSessionKey(DePackages.PackageID, DePackages.ProductCode, DePackages.CallId)
        Dim RetrieveFromDB As Boolean = True
        Dim ds As New DataSet

        'Retrieve from session and validate the contents.  The price may not equal the basket price because
        'further updates has happened on the backend.  In this case we will retrieve the contents from the db again
        Try
            If Utilities.IsSessionActive AndAlso HttpContext.Current.Session.Item(sessionKey) IsNot Nothing Then
                ds = CType(HttpContext.Current.Session.Item(sessionKey), DataSet)
                If Not ds.Tables("Package") Is Nothing Then
                    If CheckPrice Then
                        If CType(ds.Tables("Package").Rows(0).Item("PriceIncludingVAT"), Decimal) = packageBasketPrice Then
                            RetrieveFromDB = False
                        End If
                    Else
                        RetrieveFromDB = False
                    End If
                End If
            End If
        Catch ex As Exception
        End Try

        If RetrieveFromDB Then
            With DBPack
                .DePack = DePackages
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    If ResultDataSet.Tables.Count >= 5 Then
                        AddItemToSession(sessionKey, ResultDataSet)
                    End If
                End If
            End With

            If refreshBasket Then
                Dim basket As New Talent.Common.TalentBasket
                With basket
                    .Settings = Settings
                    .De.SessionId = DePackages.BasketId
                    .De.CustomerNo = Settings.LoginId
                    .ResultDataSet = Nothing
                    .De.Src = Settings.OriginatingSourceCode
                    err = .RetrieveTicketingItems
                End With
            End If
        Else
            ResultDataSet = ds
        End If

        Return err
    End Function

    Public Function GetCustomerPackageInformation(ByVal settingsObj As DESettings, ByVal BasketId As String, ByVal PackageId As Long, ByVal ProductCode As String, Optional CheckPrice As Boolean = False, Optional packageBasketPrice As Decimal = 0.0, Optional refreshBasket As Boolean = False) As ErrorObj
        Settings = settingsObj
        DePackages.BasketId = BasketId
        DePackages.PackageID = PackageId
        DePackages.ProductCode = ProductCode
        If Settings.IsAgent Then 'Need to set the boxoffice agent name to pick the correct template id for boxoffice from WS074R
            DePackages.BoxOfficeUser = Settings.AgentEntity.AgentUsername
        End If
        err = GetCustomerPackageInformation(CheckPrice, packageBasketPrice, refreshBasket)
        Return err
    End Function

    Public Function GetComponentGroupDetails(ByVal settingsObj As DESettings, ByVal ComponentGroupId As Long, ByRef ds1 As DataSet,
                                              Optional ByVal Caching As Boolean = True, Optional CacheTimeMinutes As Integer = 30) As ErrorObj
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        talPackage.Settings = settingsObj
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CacheTimeMinutes
        talPackage.DePackages.ComponentGroupID = ComponentGroupId
        err = talPackage.GetComponentGroupDetails()
        ds1 = talPackage.ResultDataSet
        Return err
    End Function

    Public Function GetComponentGroupDetails(ByVal settingsObj As DESettings, ByVal dePackage As DEPackages, ByRef ds1 As DataSet,
                                          Optional ByVal Caching As Boolean = True, Optional ByVal CacheTimeMinutes As Integer = 30) As ErrorObj
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        talPackage.Settings = settingsObj
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CacheTimeMinutes
        talPackage.DePackages = dePackage
        err = talPackage.GetComponentGroupDetails()
        ds1 = talPackage.ResultDataSet
        Return err
    End Function

    Public Function GetComponentDetails(ByVal settingsObj As DESettings, ByVal dePackage As DEPackages, ByRef ds1 As DataSet,
                                        Optional ByVal Caching As Boolean = True, Optional ByVal CacheTimeMinutes As Integer = 30) As ErrorObj
        Dim talPackage As New TalentPackage
        Dim err As ErrorObj
        talPackage.Settings = settingsObj
        talPackage.Settings.Cacheing = Caching
        talPackage.Settings.CacheTimeMinutes = CacheTimeMinutes
        talPackage.DePackages = dePackage
        err = talPackage.GetComponentDetails()
        ds1 = talPackage.ResultDataSet
        Return err
    End Function

    Public Function GetStands(ByVal dt As DataTable) As IDictionary(Of String, String)
        Dim dicPriceBands As New Dictionary(Of String, String)
        For Each row As DataRow In dt.Rows
            dicPriceBands.Add(Utilities.CheckForDBNull_String(row("StandCode")).TrimStart("0"),
                             Utilities.CheckForDBNull_String(row("StandDescription")).TrimStart("0"))
        Next
        Return dicPriceBands
    End Function

    Public Function GetComponentSeats() As ErrorObj
        Settings.ModuleName = MODULEGETCOMPONENTSEATS
        Me.GetConnectionDetails(Settings.BusinessUnit, "", Settings.ModuleName)
        With DBPack
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function GetPackageSeatDetails() As ErrorObj
        Settings.ModuleName = MODULEGETPAACKAGESEATDETAILS
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbPackage As New DBPackage
        With dbPackage
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function GetHospitalityBookings() As ErrorObj
        Settings.ModuleName = MODULEGETHOSPITALITYBOOKINGS
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbPackage As New DBPackage
        With dbPackage
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function UpdateHospitalityBookingStatus() As ErrorObj
        Settings.ModuleName = MODULEUPDATEHOSPITALITYBOOKINGSTATUS
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbPackage As New DBPackage
        With dbPackage
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve sold hospitality booking details for callId.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function GetSoldHospitalityBookingDetails() As ErrorObj
        Settings.ModuleName = MODULEGETSOLDHOSPITALITYBOOKINGDETAILS
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbPackage As New DBPackage
        With dbPackage
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Print Hospitality Bookings.
    ''' </summary>
    ''' <remarks></remarks>
    Public Function PrintHospitalityBookings() As ErrorObj
        Settings.ModuleName = MODULEPRINTHOSPITALITYBOOKINGS
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbPackage As New DBPackage
        With dbPackage
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Create Hospitality Booking Document
    ''' </summary>
    ''' <remarks></remarks>
    Public Function CreateHospitalityBookingDocument() As ErrorObj
        Settings.ModuleName = MODULECREATEHOSPITALITYBOOKINGDOCUMENT
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, Settings.ModuleName)
        Dim err As New ErrorObj
        Dim dbPackage As New DBPackage
        With dbPackage
            .DePack = DePackages
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

End Class