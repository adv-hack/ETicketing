

<Serializable()> _
Public Class TalentCustomerText
    Inherits TalentBase
    Public Property ResultDataSet() As DataSet
    Public Property deCustomerText As New DECustomerText

    Public Function RetrieveTalentCustomerText() As ErrorObj
        Dim tct As New DBCustomerText
        Const ModuleName As String = "RetrieveTalentCustomerText"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        With tct
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .DeCustomerText = Me.deCustomerText
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

    Public Function UpdateTalentCustomerText() As ErrorObj
        Dim tct As New DBCustomerText
        Const ModuleName As String = "UpdateTalentCustomerText"
        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tct
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .DeCustomerText = Me.deCustomerText
            err = .AccessDatabase()
        End With
        Return err
    End Function
End Class
