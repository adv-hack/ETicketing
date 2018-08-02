Imports Talent.Common.Utilities
<Serializable()> _
Public Class TalentQueue
    Inherits TalentBase

    Private _resultDataSet As DataSet

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Public Function UpdateSiteActivityTotal() As ErrorObj
        Const ModuleName As String = "UpdateSiteActivityTotal"
        TalentCommonLog(ModuleName, "", "Talent.Common Queue DB Access Request")
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim dbProduct As New DBQueue
        With dbProduct
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
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function
End Class
