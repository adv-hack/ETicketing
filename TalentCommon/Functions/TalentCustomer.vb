Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentCustomer
    Inherits TalentBase

#Region "Class Level Fields"

    Private _de As New DECustomerV11
    Private _resultDataSet As DataSet
    Private _deAddTicketingItems As DEAddTicketingItems

#End Region

#Region "Public Properties"

    Public Property DeV11() As DECustomerV11
        Get
            Return _de
        End Get
        Set(ByVal value As DECustomerV11)
            _de = value
        End Set
    End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Property DeAddTicketingItems() As DEAddTicketingItems
        Get
            Return _deAddTicketingItems
        End Get
        Set(ByVal value As DEAddTicketingItems)
            _deAddTicketingItems = value
        End Set
    End Property
    Public Property UseSaveMyCard() As Boolean = False
    Public Property RefreshUserAttributesOnLogin() As Boolean = False
    Public Property AgentDataEntity() As DEAgent
    Public Property CourseDetailsFanFlag() As Boolean = False
    Public Property CourseDetailsContactName() As String
    Public Property CourseDetailsContactNumber() As String
    Public Property CourseDetailsMedicalInfo() As String
    Public Property PaymenntReference() As String

#End Region

#Region "Public Functions"

    Public Function SetCustomer() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "SetCustomer"

        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveReservedAndSoldSeats() As ErrorObj
        Const ModuleName As String = "RetrieveReservedAndSoldSeats"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & "CustomerNumber=" & DeAddTicketingItems.CustomerNumber & "ProductCode=" & DeAddTicketingItems.ProductCode
        Dim dbCustomer As New DBCustomer
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            With dbCustomer
                .DeAddTicketingItems = DeAddTicketingItems
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "ReservedSeats" & DeAddTicketingItems.ProductCode)
                End If
            End With
        End If
        Return err
    End Function

    Public Function CustomerRetrieval() As ErrorObj
        Const ModuleName As String = "CustomerRetrieval"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)

            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function CustomerMembershipsRetrieval() As ErrorObj
        Const ModuleName As String = "CustomerMembershipsRetrieval"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function CustomerProfileRetrieval() As ErrorObj
        Const ModuleName As String = "CustomerProfileRetrieval"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)

            '--------------------------------------------------------------------------
            '   Cache key should be constructed Type od cache, Company name and all relevent 
            '   incoming unique keys, If cacheing enabled for this web service and there is 
            '   something contained within the cache, use it instead of going back to the database
            '
            Dim cacheKey As String = ModuleName & Settings.Company
            If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            Else
                Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
                Settings.ModuleName = ModuleName
                Dim dbCustomer As New DBCustomer
                With dbCustomer
                    .DeV11 = DeV11
                    .Settings = Settings
                    err = .ValidateAgainstDatabase()
                    If Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                    If Not err.HasError And ResultDataSet Is Nothing Then
                        err = .AccessDatabase()
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                        End If
                    End If
                End With
            End If
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function CustomerActivitiesRetrieval() As ErrorObj
        Const ModuleName As String = "CustomerActivitiesRetrieval"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Dim cacheKey As String = ModuleName & Settings.Company & de.CustomerNumber

            If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            Else
                Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
                Settings.ModuleName = ModuleName
                Dim dbCustomer As New DBCustomer
                With dbCustomer
                    .DeV11 = DeV11
                    .Settings = Settings
                    err = .ValidateAgainstDatabase()
                    If Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                    If Not err.HasError And ResultDataSet Is Nothing Then
                        err = .AccessDatabase()
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                        End If
                    End If
                End With
            End If
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function VerifyAndRetrieveCustomerDetails() As ErrorObj
        Const ModuleName As String = "VerifyAndRetrieveCustomerDetails"
        Const ModuleNameRetrieveMySavedCards As String = "RetrieveMySavedCards"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            de.Agent = Settings.IsAgent
            de.IncludeBoxOfficeLinks = Settings.IsAgent

            Dim sessionKey As String = CustomerAssociationsSessionKey(Utilities.PadLeadingZeros(de.CustomerNumber, 12), de.IncludeBoxOfficeLinks)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                .RefreshUserAttributesOnLogin = RefreshUserAttributesOnLogin
                err = .AccessDatabase()
                If Not err.HasError AndAlso Not .ResultDataSet Is Nothing Then

                    'Return the full result set
                    ResultDataSet = .ResultDataSet

                    If .ResultDataSet.Tables.Count > 2 Then

                        'Extract the customer associations from the result set and cache
                        Dim rs As New DataSet
                        Dim DtStatusResults As New DataTable
                        DtStatusResults = .ResultDataSet.Tables(0).Copy
                        rs.Tables.Add(DtStatusResults)
                        If .ResultDataSet.Tables.Count > 2 Then
                            Dim DtFriendsAndFamilyResults As New DataTable
                            DtFriendsAndFamilyResults = .ResultDataSet.Tables("FriendsAndFamily").Copy
                            rs.Tables.Add(DtFriendsAndFamilyResults)
                        End If
                        AddItemToSession(sessionKey, rs)

                        If ResultDataSet.Tables("CardDetails") IsNot Nothing Then
                            If UseSaveMyCard Then
                                Dim savedCardDetailsDataSet As New DataSet
                                Dim dtCardDetails, dtStatusTable As New DataTable
                                dtStatusTable = ResultDataSet.Tables(0).Copy
                                dtCardDetails = ResultDataSet.Tables("CardDetails").Copy
                                savedCardDetailsDataSet.Tables.Add(dtStatusTable)
                                savedCardDetailsDataSet.Tables.Add(dtCardDetails)
                                AddItemToCache(ModuleNameRetrieveMySavedCards & PadLeadingZeros(de.CustomerNumber, 12), savedCardDetailsDataSet, Settings)
                            End If
                            ResultDataSet.Tables.Remove("CardDetails")
                        End If

                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function VerifyPassword() As ErrorObj
        Const ModuleName As String = "VerifyPassword"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function UpdatePassword() As ErrorObj
        Const ModuleName As String = "UpdatePassword"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function
    Public Function ResetPassword() As ErrorObj
        Const ModuleName As String = "ResetPassword"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function RetrievePassword() As ErrorObj
        Const ModuleName As String = "RetrievePassword"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function GeneratePassword() As ErrorObj
        Const ModuleName As String = "GeneratePassword"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function ValidateCustomer() As ErrorObj
        Const ModuleName As String = "ValidateCustomer"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)

            '--------------------------------------------------------------------------
            '   Cache key should be constructed Type od cache, Company name and all relevent 
            '   incoming unique keys, If cacheing enabled for this web service and there is 
            '   something contained within the cache, use it instead of going back to the database
            '
            Dim cacheKey As String = ModuleName & Settings.Company
            If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            Else
                Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
                Settings.ModuleName = ModuleName
                Dim dbCustomer As New DBCustomer
                With dbCustomer
                    .DeV11 = DeV11
                    .Settings = Settings
                    err = .ValidateAgainstDatabase()
                    If Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                    If Not err.HasError And ResultDataSet Is Nothing Then
                        err = .AccessDatabase()
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                            AddItemToCache(cacheKey, ResultDataSet, Settings)
                        End If
                    End If
                End With
            End If
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function AddCustomerAssociation() As ErrorObj
        Const ModuleName As String = "AddCustomerAssociation"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            '-------------------------------------------------------------------------------------
            '   No Cache possible due to way the process works
            '
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function DeleteCustomerAssociation() As ErrorObj
        Const ModuleName As String = "DeleteCustomerAssociation"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            '-------------------------------------------------------------------------------------
            '   No Cache possible due to way the process works
            '
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function CustomerAssociations() As ErrorObj
        Const ModuleName As String = "CustomerAssociations"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Dim sessionKey As String = CustomerAssociationsSessionKey(de.CustomerNumber, de.IncludeBoxOfficeLinks)

            If Utilities.IsSessionActive AndAlso HttpContext.Current.Session.Item(sessionKey) IsNot Nothing Then
                ResultDataSet = CType(HttpContext.Current.Session.Item(sessionKey), DataSet)
            Else
                Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
                Settings.ModuleName = ModuleName
                Dim dbCustomer As New DBCustomer
                With dbCustomer
                    .DeV11 = DeV11
                    .Settings = Settings
                    err = .ValidateAgainstDatabase()
                    If Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToSession(sessionKey, ResultDataSet)
                    End If
                    If Not err.HasError And ResultDataSet Is Nothing Then
                        err = .AccessDatabase()
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                            AddItemToSession(sessionKey, ResultDataSet)
                        End If
                    End If
                End With
            End If
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function CustomerAssociationsClearSession() As ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim sessionKey As String = CustomerAssociationsSessionKey(DeV11.DECustomersV1(0).CustomerNumber, False)
            RemoveItemFromSession(sessionKey)
            sessionKey = CustomerAssociationsSessionKey(DeV11.DECustomersV1(0).CustomerNumber, True)
            RemoveItemFromSession(sessionKey)
        End If
        Return New ErrorObj
    End Function

    Public Function CustomerAssociationsClearSessionFromTheirList() As ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim sessionKey As String = CustomerAssociationsSessionKey(DeV11.DECustomersV1(0).FriendsAndFamilyId, False)
            RemoveItemFromSession(sessionKey)
            sessionKey = CustomerAssociationsSessionKey(DeV11.DECustomersV1(0).FriendsAndFamilyId, True)
            RemoveItemFromSession(sessionKey)
        End If
        Return New ErrorObj
    End Function

    Public Function GetCustomerAssociationDictionary(ByVal _Settings As DESettings, ByVal CustomerNumber As String,
                                                        ByVal FullName As String, ByVal customer As DECustomer) As IDictionary(Of String, String)

        Dim dicCustomers As New Dictionary(Of String, String)
        Dim _deCustomer As New Talent.Common.DECustomer
        _deCustomer = customer
        Dim _deCustV11 As New Talent.Common.DECustomerV11
        _deCustomer.UserName = FullName
        _deCustomer.CustomerNumber = CustomerNumber
        _deCustomer.Source = "W"
        _deCustV11.DECustomersV1.Add(_deCustomer)
        Dim err As ErrorObj
        Dim ds1 As DataSet
        Settings = _Settings
        DeV11 = _deCustV11
        err = CustomerAssociations()
        ds1 = ResultDataSet()
        dicCustomers.Add(CustomerNumber, FullName)

        For Each row As DataRow In ds1.Tables(1).Rows
            dicCustomers.Add(Utilities.CheckForDBNull_String(row("AssociatedCustomerNumber")),
                             Utilities.CheckForDBNull_String(row("AssociatedCustomerNumber")).TrimStart("0") & _
                                                                        " - " & Utilities.CheckForDBNull_String(row("Forename")).Trim & _
                                                                        " " & Utilities.CheckForDBNull_String(row("Surname")).Trim())
        Next

        Return dicCustomers

    End Function

    Public Function CanMoreCustomerAssociationsCanBeAdded() As ErrorObj
        Const ModuleName As String = "CanMoreCustomerAssociationsCanBeAdded"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    ''' <summary>
    ''' To add /update customers in friends and family and then add the ticket to basket 
    ''' </summary>
    ''' <returns></returns>
    Public Function SetParticipantsAndBasket() As ErrorObj
        Const ModuleName As String = "SetParticipantsAndBasket"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            TalentCommonLog(ModuleName, _deAddTicketingItems.SignedInCustomer, "Talent.Common Request = DE=" & _deAddTicketingItems.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .DeAddTicketingItems = DeAddTicketingItems
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, _deAddTicketingItems.SignedInCustomer, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function UpdateCustomerDetailsSingleMode() As ErrorObj
        Const ModuleName As String = "UpdateCustomerDetailsSingleMode"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function AttributeDefinitionRetrieval() As ErrorObj
        Const ModuleName As String = "AttributeDefinitionRetrieval"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)

            '--------------------------------------------------------------------------
            '   Cache key should be constructed Type od cache, Company name and all relevent 
            '   incoming unique keys, If cacheing enabled for this web service and there is 
            '   something contained within the cache, use it instead of going back to the database
            '
            Dim cacheKey As String = ModuleName & Settings.Company
            If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
            Else
                Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
                Settings.ModuleName = ModuleName
                Dim dbCustomer As New DBCustomer

                With dbCustomer
                    .DeV11 = DeV11
                    .Settings = Settings
                    err = .ValidateAgainstDatabase()
                    If Not err.HasError And ResultDataSet Is Nothing Then
                        err = .AccessDatabase()
                        If Not err.HasError And Not .ResultDataSet Is Nothing Then
                            ResultDataSet = .ResultDataSet
                        End If
                    End If
                End With
            End If
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    ''' <summary>
    ''' 'Retrieve Address Capture - WS140R
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveAddressCapture() As ErrorObj
        Const ModuleName As String = "RetrieveAddressCapture"
        Dim err As New ErrorObj
        Dim dbCustomer As New DBCustomer
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbCustomer
            .DeV11 = DeV11
            .AgentDataEntity = AgentDataEntity
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function RetrieveCourseDetails() As ErrorObj
        Const ModuleName As String = "RetrieveCourseDetails"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                    If Not err.HasError Then
                        CourseDetailsFanFlag = .CourseDetailsFanFlag
                        CourseDetailsContactName = .CourseDetailsContactName
                        CourseDetailsContactNumber = .CourseDetailsContactNumber
                        CourseDetailsMedicalInfo = .CourseDetailsMedicalInfo
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function UpdateCourseDetails() As ErrorObj
        Const ModuleName As String = "UpdateCourseDetails"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    .CourseDetailsFanFlag = CourseDetailsFanFlag
                    .CourseDetailsContactName = CourseDetailsContactName
                    .CourseDetailsContactNumber = CourseDetailsContactNumber
                    .CourseDetailsMedicalInfo = CourseDetailsMedicalInfo
                    err = .AccessDatabase()
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function RetrieveCustomersAtAddress() As ErrorObj
        Const ModuleName As String = "RetrieveCustomersatAddress"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

    Public Function UpdateCustomerAddresses() As ErrorObj
        Const ModuleName As String = "UpdateCustomerAddresses"
        Dim err As New ErrorObj
        If DeV11.DECustomersV1.Count > 0 Then
            Dim de As DECustomer = DeV11.DECustomersV1(0)
            TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCustomer As New DBCustomer
            With dbCustomer
                .DeV11 = DeV11
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                    End If
                End If
            End With
            TalentCommonLog(ModuleName, de.CustomerNumber, ResultDataSet, err)
        End If
        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function CustomerAssociationsSessionKey(ByVal customer As String, ByVal includeBoxOfficeLink As String) As String
        Return "CustomerAssociations" & Settings.Company & customer & includeBoxOfficeLink
    End Function

#End Region

End Class