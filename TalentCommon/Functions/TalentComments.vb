
''' <summary>
''' Function to add ticketing order line comments or Corporate sale comments at package level  
''' Max comment length is 120 for Corporate   
''' For corporate comments Populate the following CommentsDataEntity properties (CommentText,Settings,SessionID,CustomerID,CorporatePackageNumericID) 
''' For ticketing comments Populate the following CommentsDataEntity properties( CommentText,Settings,SessionID,CustomerID,ProductCode,Seat)
''' </summary>
''' <remarks></remarks>
''' 
<Serializable()> _
Public Class TalentComments
    Inherits TalentBase

    Private _deComments As DEComments

    Public Property CommentsDataEntity As DEComments
        Get
            Return _deComments
        End Get
        Set(ByVal value As DEComments)
            _deComments = value
        End Set

    End Property


    Public Function AddOrderComments() As ErrorObj
        Dim err As New ErrorObj

        Dim myDBComments As New DBComments

        Const ModuleName As String = "AddOrderComments"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim Comments As New DBComments
        With Comments
            .Settings = Settings
            .deComments = _deComments
            err = .AccessDatabase()
        End With


        Return err
    End Function

   

End Class
