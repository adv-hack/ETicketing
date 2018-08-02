<Serializable()> _
Public Class TalentLoyalty
    Inherits TalentBase

    Private _deLoyaltyList As DELoyaltyList
    Public Property LoyaltyList As DELoyaltyList
        Get
            Return _deLoyaltyList
        End Get
        Set(ByVal value As DELoyaltyList)
            _deLoyaltyList = value
        End Set
    End Property

    Private _resultDataSet As DataSet
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Public Function AddLoyaltyPoints() As ErrorObj
        Dim err As New ErrorObj
        Const ModuleName As String = "AddLoyaltyPoints"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim loyalty As New DBLoyalty
        With loyalty
            .Settings = Settings
            .LoyaltyList = _deLoyaltyList
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
