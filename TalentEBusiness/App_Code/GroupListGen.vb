'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    GroupListGen. Group List as a generic collection
'                                   (faster than an array list)
'
'       Date                        22 March 2007
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base       
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------

'--------------------------------------------------------------------------------------------------
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports Talent.eCommerce
Imports System.Collections.Generic
Imports System.Text
Imports System.Reflection
Namespace Talent.eCommerce

    Public Class GroupListGen
        Private _Groups As List(Of GroupLevelDetails.GroupDetails)
        Private _PageSize As Integer = 10 'DEFAULT

        Public Sub New()
            _Groups = New List(Of GroupLevelDetails.GroupDetails)()
        End Sub

        Public ReadOnly Property Count() As Integer
            Get
                Return _Groups.Count
            End Get
        End Property

        Public ReadOnly Property NumberOfPages() As Integer
            Get
                If Count = 0 Then
                    Return 0
                End If
                Dim group_count As Integer = Count
                If group_count <= _PageSize Then
                    Return 1
                ElseIf group_count Mod _PageSize = 0 Then
                    Return group_count \ _PageSize
                Else
                    Return (group_count \ _PageSize) + 1
                End If
            End Get
        End Property

        Public Property PageSize() As Integer
            Get
                Return _PageSize
            End Get
            Set(ByVal Value As Integer)
                _PageSize = Value
            End Set
        End Property

        Public ReadOnly Property Groups() As IList
            Get
                Return _Groups
            End Get
        End Property

        Public Sub Add(ByVal p_group As GroupLevelDetails.GroupDetails)
            _Groups.Add(p_group)
        End Sub

        Public Function GetPageGroups(ByVal p_page As Integer) As IList
            ' returs the 'page' of products as indicated by the p_page argument and the
            ' _PageSize variable.
            If p_page > NumberOfPages Then
                'possible attempt to break the code, return an empty ArrayList
                Return New ArrayList
            End If
            Dim startPos As Integer
            startPos = (p_page * _PageSize) - _PageSize
            If (p_page * _PageSize) < Count Then
                Return _Groups.GetRange(startPos, _PageSize)
            Else
                Return _Groups.GetRange(startPos, Count - (startPos))
            End If
        End Function
        Public Sub SortGroupsByName(ByVal order As String)
            If order = "A" Then
                _Groups.Sort(New GenericComparer(Of Product)("GroupName", GenericComparer(Of GroupLevelDetails.GroupDetails).SortOrder.Ascending))
            Else
                _Groups.Sort(New GenericComparer(Of Product)("GroupName", GenericComparer(Of GroupLevelDetails.GroupDetails).SortOrder.Descending))
            End If
        End Sub
        Public Sub SortGroupsByDescription(ByVal order As String)
            If order = "A" Then
                _Groups.Sort(New GenericComparer(Of Product)("GroupDescription1", GenericComparer(Of GroupLevelDetails.GroupDetails).SortOrder.Ascending))
            Else
                _Groups.Sort(New GenericComparer(Of Product)("GroupDescription1", GenericComparer(Of GroupLevelDetails.GroupDetails).SortOrder.Descending))

            End If
        End Sub

        Public ReadOnly Property ProductsEnum() As IEnumerator
            Get
                Return _Groups.GetEnumerator()
            End Get
        End Property

        Public Sub Clear()
            _Groups.Clear()
        End Sub

    End Class

End Namespace

