Imports System.Web

<Serializable()> _
Public Class TalentStopcodes
    Inherits TalentBase
    Public Property Company As String
    Public Property Customer As String
    Public Property ResultDataSet() As DataSet

    Public Function RetrieveTalentStopcodes() As ErrorObj
        Dim tsc As New DBStopcodes
        Const ModuleName As String = "RetrieveTalentStopCodes"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With tsc
                .Settings = Settings
                .Settings.ModuleName = ModuleName
                .Company = Company
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        Return err
    End Function

    Public Function RetrieveTalentStopCodeAudit() As ErrorObj
        Dim tsc As New DBStopcodes
        Const ModuleName As String = "RetrieveTalentStopCodeAudit"
        Dim err As New ErrorObj
        Dim dtPriceBands As New DataTable
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tsc
            .Settings = Settings
            .Settings.ModuleName = "RetrieveTalentStopCodeAudit"
            .Customer = Me.Customer
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
End Class
