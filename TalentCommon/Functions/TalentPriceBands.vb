Imports System.Web

<Serializable()> _
Public Class TalentPriceBands
    Inherits TalentBase
    Public Property Company As String
    Public Property DePriceBands As DEPriceBands
    Public Property ResultDataSet() As DataSet

    Public Function RetrieveTalentPriceBands() As ErrorObj
        Dim tpb As New DBPriceBands
        Const ModuleName As String = "RetrieveTalentPriceBands"
        Dim err As New ErrorObj
        Dim dtPriceBands As New DataTable
        Dim cacheKey As String = ModuleName & Settings.Company

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With tpb
                .Settings = Settings
                .Settings.ModuleName = ModuleName
                .company = Company
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End With
        End If
        Return err
    End Function

End Class
