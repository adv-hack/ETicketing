Imports System.Text
<Serializable()> _
Public Class DECompany
    Public Property AgentName() As String = String.Empty
    'Company details
    Public Property CompanyID() As String = String.Empty
    Public Property CompanyNumber() As String = String.Empty
    Public Property ParentCompanyNumber() As String = String.Empty
    Public Property ParentCompanyName() As String = String.Empty
    Public Property ChildCompanyNumber() As String = String.Empty
    Public Property SearchType() As String = String.Empty
    Public Property CompanyName() As String = String.Empty
    Public Property TelephoneNumber1() As String = String.Empty
    Public Property Telephone1Use() As Boolean
    Public Property TelephoneNumber2() As String = String.Empty
    Public Property Telephone2Use() As Boolean
    Public Property TelephoneNumber3() As String = String.Empty
    Public Property Telephone3Use() As Boolean
    Public Property SalesLedgerAccount As String = String.Empty
    Public Property OwningAgent As String = String.Empty
    Public Property VATCodeID As String = String.Empty
    Public Property WebAddress() As String = String.Empty
    Public Property RegisteredDate() As String = String.Empty

    'Company address details
    Public Property AddressID() As String = String.Empty
    Public Property AddressType() As String = String.Empty
    Public Property AddressLine1() As String = String.Empty
    Public Property AddressLine2() As String = String.Empty
    Public Property AddressLine3() As String = String.Empty
    Public Property PostCode() As String = String.Empty
    Public Property County() As String = String.Empty
    Public Property Country() As String = String.Empty
    'Utilities
    Public Property Source() As String
    Public ReadOnly Property LogString() As String
        Get
            Dim logStringBuilder As New StringBuilder
            With logStringBuilder
                .Append(CompanyName)
                .Append(AddressLine1)
                .Append(PostCode)
                .Append(WebAddress)
                .Append(TelephoneNumber1)
                .Append(Source)
            End With

            Return logStringBuilder.ToString
        End Get
    End Property
    Public Property CompanyOperationMode() As GlobalConstants.CRUDOperationMode
    Public Property CustomerNumber As String
    Public Property IsParentCompany As Boolean = False

    Public Property Start As Integer
    Public Property Length As Integer

End Class
