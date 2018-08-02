Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Talent.Common.Utilities
Imports System.Linq
Imports System.Collections

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with various requests
'                                   such as checking connections

'       Date                        10/07/08
'
'       Author                      Ben Ford
'
'       @ CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAUT - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentUtiltities

    Inherits TalentBase
    Private Const CLASSNAME As String = "TalentUtiltities"
    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Private _businessUnit As String = String.Empty
    Private _descriptionKey As String = String.Empty
    Private _customerNumber As String = String.Empty
    Private _paymentRef As Integer = 0

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property

    Public Property DescriptionKey() As String
        Get
            Return _descriptionKey
        End Get
        Set(ByVal value As String)
            _descriptionKey = value
        End Set
    End Property

    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property PaymentRef() As Integer
        Get
            Return _paymentRef
        End Get
        Set(ByVal value As Integer)
            _paymentRef = value
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

    Public Property CacheDependencyName() As String

    Public Function CheckBackEndDatabase() As ErrorObj
        Const ModuleName As String = "CheckBackEndDatabase"
        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim dbUtility As New DBUtilities(ModuleName)
        With dbUtility
            .Settings = Settings
            err = .AccessDatabase
        End With

        Return err
    End Function

    Public Function RetrieveDescriptionEntries() As ErrorObj
        Const ModuleName As String = "RetrieveDescriptionEntries"
        TalentCommonLog(ModuleName, _descriptionKey, "Talent.Common Request = Key=" & _descriptionKey)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & _descriptionKey
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbUtility As New DBUtilities(ModuleName)
            With dbUtility
                .Settings = Settings
                .DescriptionKey = _descriptionKey
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If

        Return err
    End Function

    Public Function VerifyPaymentReference() As ErrorObj
        Const ModuleName As String = "VerifyPaymentReference"

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.BusinessUnit & CustomerNumber & PaymentRef.ToString
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbUtility As New DBUtilities(ModuleName)
            With dbUtility
                .Settings = Settings
                .CustomerNumber = CustomerNumber
                .PaymentRef = PaymentRef
                .UtilityCode = 1
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If

        Return err
    End Function


End Class


