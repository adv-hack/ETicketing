Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentCompany
    Inherits TalentBase

#Region "Class Level Fields"
    Private _companyDataEntity As New DECompany
    Private _customerDataEntity As New DECustomer
    Private _resultDataSet As DataSet
#End Region

#Region "Public Properties"
    Public Property Company() As DECompany
        Get
            Return _companyDataEntity
        End Get
        Set(ByVal value As DECompany)
            _companyDataEntity = value
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

    Public Property Customer() As DECustomer
        Get
            Return _customerDataEntity
        End Get
        Set(ByVal value As DECustomer)
            _customerDataEntity = value
        End Set
    End Property

#End Region

    Public Function CompanyOperations() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "CompanyOperations"
        Dim cacheKey As String = ModuleName
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbCompany As New DBCompany
        With dbCompany
            .Company = Company
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
        Return err
    End Function
    Public Function ParentCompanyOperations() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "ParentCompanyOperations"
        Dim cacheKey As String = ModuleName
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbCompany As New DBCompany
        With dbCompany
            .Company = Company
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
        Return err
    End Function
    Public Function ProcessCompanyContacts() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "ProcessCompanyContacts"
        Dim cacheKey As String = ModuleName
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbCompany As New DBCompany
        With dbCompany
            .Company = Company
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
        Return err
    End Function

    Public Function FilterCompanyByCustomerDetails() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "FilterCompanyByCustomerDetails"
        Dim cacheKey As String = ModuleName
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else

        End If
        Return err
    End Function

    Public Function RetrieveCustomersByCompanyId() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "CompanyContactsResults"
        Dim cacheKey As String = ModuleName
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbCompany As New DBCompany
        With DBCompany
            .Company = Company
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
        Return err
    End Function
End Class
