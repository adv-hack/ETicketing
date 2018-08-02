''' <summary>
''' Update Direct debit details or show balances 
''' </summary>
''' <remarks></remarks>
<Serializable()> _
Public Class TalentDirectDebitDetails
    Inherits TalentBase

    Public Property Company As String
    Public Property Customer As String
    Public Property ResultDataSet() As DataSet
    Public Property deDirectDebitDetails As New DEDirectDebitDetails

    Public Function RetrieveTalentDirectDebitDetails() As ErrorObj
        Dim tdd As New DBDirectDebits
        Const ModuleName As String = "RetrieveTalentDirectDebitDetails"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tdd
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Customer = Me.Customer
            .DirectDebitDetails = deDirectDebitDetails
            err = .AccessDatabase()
        End With
        Return err
    End Function

    Public Function RetrieveTalentDirectDebitBalances() As ErrorObj
        Dim tdd As New DBDirectDebits
        Const ModuleName As String = "RetrieveTalentDirectDebitBalances"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tdd
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Customer = Me.Customer
            .DirectDebitDetails = deDirectDebitDetails
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function RetrieveTalentDirectDebitOnAccount() As ErrorObj
        Dim tdd As New DBDirectDebits
        Const ModuleName As String = "RetrieveTalentDirectDebitOnAccount"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tdd
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Customer = Me.Customer
            .DirectDebitDetails = deDirectDebitDetails
            err = .AccessDatabase()
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function UpdateTalentDirectDebitDetails() As ErrorObj
        Dim tdd As New DBDirectDebits
        Const ModuleName As String = "UpdateTalentDirectDebitDetails"
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tdd
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Customer = Me.Customer
            .DirectDebitDetails = deDirectDebitDetails
            err = .AccessDatabase()
            .DirectDebitDetails = deDirectDebitDetails
        End With
        Return err
    End Function

    Public Function ChangeTalentDirectDebitOnAccountStatus() As ErrorObj
        Const ModuleName As String = "ChangeTalentDirectDebitOnAccountStatus"
        Dim err As New ErrorObj
        Dim tdd As New DBDirectDebits
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tdd
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Customer = Me.Customer
            .DirectDebitDetails = Me.deDirectDebitDetails
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function

    Public Function DeleteTalentDirectDebitOnAccount() As ErrorObj
        Const ModuleName As String = "DeleteTalentDirectDebitOnAccount"
        Dim err As New ErrorObj
        Dim tdd As New DBDirectDebits
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With tdd
            .Settings = Settings
            .Settings.ModuleName = ModuleName
            .Company = Me.Company
            .Customer = Me.Customer
            .DirectDebitDetails = Me.deDirectDebitDetails
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function

End Class
