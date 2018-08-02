Imports System.Web

<Serializable()> _
Public Class TalentCRM
    Inherits TalentBase

#Region "Class Level Fields"

    Private _de As New DETalentCRM
    Private _resultDataSet As DataSet

#End Region

#Region "Public Properties"

    Public Property De() As DETalentCRM
        Get
            Return _de
        End Get
        Set(ByVal value As DETalentCRM)
            _de = value
        End Set
    End Property

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(value As DataSet)
            _resultDataSet = value
        End Set
    End Property

#End Region

#Region "Public Function"

    Public Function RetrieveAttributeCategories() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "RetrieveAttributeCategories"

        Dim cacheKey As String = ModuleName & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCRM As New DBCRM
            With dbCRM
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
        Return err
    End Function

    Public Function RetrieveAttributeDefinition() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "RetrieveAttributeDefinitions"

        Dim cacheKey As String = ModuleName & Settings.Company
        If De.AttributeCategoryCode.Trim <> "" Then
            cacheKey = ModuleName & "Category" & De.AttributeCategoryCode & Settings.Company
        End If
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCRM As New DBCRM
            With dbCRM
                .DeCRM.AttributeCategoryCode = De.AttributeCategoryCode
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
        Return err
    End Function

    Public Function RetrieveCustomerAttributes() As ErrorObj

        Dim err As New ErrorObj
        Const ModuleName As String = "RetrieveCustomerAttributes"

        Dim cacheKey As String = RetrieveCustomerAttributesCacheKey(De.CustomerNumber)
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbCRM As New DBCRM
            With dbCRM
                .DeCRM = De
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

        'TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)

        ' End If

        Return err
    End Function

    'Public Function ProcessAttribute() As ErrorObj
    '    Dim err As ErrorObj
    '    Const ModuleName As String = "ProcessAttribute"

    '    'If DeV11.DECustomersV1.Count > 0 Then
    '    '    Dim de As DECustomer = DeV11.DECustomersV1(0)
    '    '    TalentCommonLog(ModuleName, de.CustomerNumber, "Talent.Common Request = DE=" & de.LogString)

    '    '--------------------------------------------------------------------------
    '    '   Cache key should be constructed Type od cache, Company name and all relevent 
    '    '   incoming unique keys, If cacheing enabled for this web service and there is 
    '    '   something contained within the cache, use it instead of going back to the database
    '    '
    '    Dim cacheKey As String = ModuleName & Settings.Company
    '    If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
    '        ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
    '    Else
    '        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
    '        Settings.ModuleName = ModuleName
    '        Dim dbCRM As New DBCRM
    '        With dbCRM
    '            .DeCRM = De
    '            .Settings = Settings
    '            err = .ValidateAgainstDatabase()
    '            If Not .ResultDataSet Is Nothing Then
    '                ResultDataSet = .ResultDataSet
    '                AddItemToCache(cacheKey, ResultDataSet, Settings)
    '            End If
    '            If Not err.HasError And ResultDataSet Is Nothing Then
    '                err = .AccessDatabase()
    '                If Not err.HasError And Not .ResultDataSet Is Nothing Then
    '                    ResultDataSet = .ResultDataSet
    '                    AddItemToCache(cacheKey, ResultDataSet, Settings)
    '                End If
    '            End If
    '        End With

    '    End If
    '    Return err
    'End Function


    Public Sub RetrieveCustomerAttributesClearCache()
        Dim cacheKey As String = RetrieveCustomerAttributesCacheKey(De.CustomerNumber)
        HttpContext.Current.Cache.Remove(cacheKey)
    End Sub

    ''' <summary>
    ''' Publish the email templates from tbl_email_templates to T#EMID in CRM
    ''' </summary>
    ''' <returns>Error object based on the results of the call</returns>
    ''' <remarks></remarks>
    Public Function RefreshEmailTemplates() As ErrorObj
        Const ModuleName As String = "RefreshEmailTemplates"
        Dim err As New ErrorObj
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbCRM As New DBCRM
        With dbCRM
            .Settings = Settings
            .DeCRM = De
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
        End With
        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function RetrieveCustomerAttributesCacheKey(ByVal customer As String) As String
        Return "RetrieveCustomerAttributes" & Settings.Company & customer
    End Function

#End Region
End Class
