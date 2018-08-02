
''' <summary>
''' remarks></remarks>
''' 
<Serializable()> _
Public Class TalentPromotionHistory
    Inherits TalentBase
    Public Property Company As String
    Public Property Member As String
    Public Property ResultDataSet() As DataSet

    Public Function RetrieveTalentPromotionHistory() As ErrorObj
        Dim ph As New DBPromotionHistory
        Const ModuleName As String = "RetrieveTalentPromotionHistory"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company

        ' TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.CustomerNumber)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        '     Dim cacheKey As String = RetrieveOnAccountDetailsCacheKey()
        '     If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        ' ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        ' Else

        With ph
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Member = Me.Member
            err = .AccessDatabase()

            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                AddItemToCache(cacheKey, ResultDataSet, Settings)
            ElseIf Settings.Cacheing Then
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
            End If

        End With

        Return err
    End Function
   
End Class
