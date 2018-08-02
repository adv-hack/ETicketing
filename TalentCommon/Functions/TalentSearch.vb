Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common.Utilities
<Serializable()> _
Public Class TalentSearch
    Inherits TalentBase

#Region "Public Properties"

    Public Property ResultDataSet() As DataSet
    Public Property CustomerSearch As DECustomerSearch
    Public Property CompanySearch As DECompanySearch
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Retrieve customer details by search criteria defined in DECustomer object
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function PerformCustomerSearch() As ErrorObj
        Const ModuleName As String = "CustomerSearch"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, CustomerSearch.Customer.CustomerNo, "Talent.Common Request = DE=" & CustomerSearch.Customer.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSearch As New DBSearch
        With dbSearch
            .CustomerSearch = CustomerSearch
            If .CustomerSearch.Customer.Source <> GlobalConstants.SOURCESUPPLYNET Then
                .CustomerSearch.Customer.Source = GlobalConstants.SOURCE
            End If
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
        TalentCommonLog(ModuleName, CustomerSearch.Customer.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function PerformCompanySearch() As ErrorObj
        Const ModuleName As String = "CompanySearch"
        Dim err As New ErrorObj
        TalentCommonLog(ModuleName, CompanySearch.Company.CompanyName, "Talent.Common Request = DE=" & CompanySearch.Company.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim dbSearch As New DBSearch
        With dbSearch
            .CompanySearch = CompanySearch
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
        TalentCommonLog(ModuleName, CompanySearch.Company.CompanyName, ResultDataSet, err)
        Return err
    End Function
#End Region

End Class
