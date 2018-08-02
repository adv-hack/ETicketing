Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with language text and desscriptions
'
'       Date                        6th Dec 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBDS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBDescriptions
    Inherits DBAccess

    Public Property FrontEndConnectionString() As String
        Get
            Return Settings.FrontEndConnectionString
        End Get
        Set(ByVal value As String)
            Settings.FrontEndConnectionString = value
        End Set
    End Property
    Public Property DestinationDatabase() As String
        Get
            Return Settings.DestinationDatabase
        End Get
        Set(ByVal value As String)
            Settings.DestinationDatabase = value
        End Set
    End Property
    Public Property Language()
        Get
            Return Settings.Language
        End Get
        Set(ByVal value)
            Settings.Language = value
        End Set
    End Property

    Private _des As DEDescriptions
    Private _collection As New Collection

    Public ReadOnly Property Collection() As Collection
        Get
            Return _collection
        End Get
    End Property

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Const ModuleName As String = "AccessDataBaseSQL2005"
        Dim err As New ErrorObj
        '---------------------------------------------------------------------
        If Not err.HasError Then
            Try
                Const Eng As String = "ENG"
                Const strSelect As String = "SELECT * FROM tbl_descriptions_detail WITH (NOLOCK)  " & _
                                            " WHERE LANGUAGE = 'ENG' OR LANGUAGE = @PARAM1 "

                Dim cmdLanguage As SqlCommand = New SqlCommand(strSelect.Trim, conSql2005)
                Dim dtrLanguage As SqlDataReader = Nothing

                With cmdLanguage
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char, 20))
                    If Settings.Language.Length > 0 Then
                        .Parameters(Param1).Value = Settings.Language.Trim
                    Else
                        .Parameters(Param1).Value = Eng
                    End If
                    dtrLanguage = .ExecuteReader()
                End With
                '--------------------------------------------------------------------------------
                With dtrLanguage
                    If .HasRows Then
                        While .Read()
                            err = Add(.Item("LANGUAGE").Trim, _
                                        .Item("TYPE").Trim, _
                                        .Item("CODE").Trim, _
                                        .Item("DESCRIPTION").Trim)
                            If err.HasError Then _
                                Exit While
                        End While
                    End If
                    .Close()
                End With
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & Read("ERR024")
                    .ErrorNumber = "TACDBDS-05"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Public Function Add(ByVal language As String, _
                        ByVal type As String, _
                        ByVal code As String, _
                        ByVal description As String) As ErrorObj
        Const ModuleName As String = "Add"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Try
            _des = New DEDescriptions
            With _des
                .Language = language
                .Type = type
                .Code = code
                .Description = description
                .Key = language & code
            End With
            Collection.Add(_des, _des.Key)
            _des = Nothing
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & Read("ERR000")
                .HasError = True
                .ErrorNumber = "TACDBDS-07"
            End With
        End Try
        Return err
    End Function
    Public Sub Clear()
        Collection.Clear()
    End Sub
    Public ReadOnly Property Count() As Integer
        Get
            Return Collection.Count
        End Get
    End Property
    Public Function Read(ByVal code As String) As String
        '
        _des = Collection.Item(Settings.Language & code)
        If _des.Description.Length = 0 Then
            Const _english As String = "ENG"
            _des = Collection.Item(_english & code)
        End If
        Return _des.Description
        '
    End Function
    Public Function Remove(ByVal language As String, _
                        ByVal code As String) As ErrorObj
        Const ModuleName As String = "Remove"
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Try
            Collection.Remove(language & code)
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & Read("ERR000")
                .HasError = True
                .ErrorNumber = "TACDBDS-08"
            End With
        End Try
        Return err

    End Function

End Class
