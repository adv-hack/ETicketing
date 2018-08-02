Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentVatCodes
    Inherits TalentBase

#Region "Class Level Fields"
    Private _resultDataSet As DataSet
#End Region

#Region "Public Properties"
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
#End Region

    Public Function RetrieveVatCodes() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "RetrieveVatCodes"

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim cacheKey As String = ModuleName & Settings.BusinessUnit & Settings.Partner
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbVatCodes As New DBVatCodes
            With dbVatCodes
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings, "VatCodeList")
                    End If
                End If
            End With
        End If

        Return err
    End Function
End Class
