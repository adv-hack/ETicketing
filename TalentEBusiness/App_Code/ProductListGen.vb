'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    ProductListGen. Product List as a generic collection
'                                   (faster than an array list)
'
'       Date                        28th Feb 2007
'
'       Author                      Ben Ford
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

    Public Class ProductListGen
        Private _Products As List(Of Product)
        Private _PageSize As Integer = 10 'DEFAULT

        Public Sub New()
            _Products = New List(Of Product)()
        End Sub

        Public ReadOnly Property Count() As Integer
            Get
                Return _Products.Count
            End Get
        End Property

        Public ReadOnly Property NumberOfPages() As Integer
            Get
                If Count = 0 Then
                    Return 0
                End If
                Dim prod_count As Integer = Count
                If prod_count <= _PageSize Then
                    Return 1
                ElseIf prod_count Mod _PageSize = 0 Then
                    Return prod_count \ _PageSize
                Else
                    Return (prod_count \ _PageSize) + 1
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

        Public ReadOnly Property Products() As IList
            Get
                Return _Products
            End Get
        End Property

        Public Sub Add(ByVal p_product As Product)
            _Products.Add(p_product)
        End Sub

        Public Function GetPageProducts(ByVal p_page As Integer) As IList
            ' returs the 'page' of products as indicated by the p_page argument and the
            ' _PageSize variable.
            If p_page > NumberOfPages Then
                'possible attempt to break the code, return an empty ArrayList
                Return New ArrayList
            End If
            Dim startPos As Integer
            startPos = (p_page * _PageSize) - _PageSize
            If (p_page * _PageSize) < Count Then
                Return _Products.GetRange(startPos, _PageSize)
            Else
                Return _Products.GetRange(startPos, Count - (startPos))
            End If
        End Function

        Public Sub SortProductsByPrice(ByVal order As String)
            If order = "A" Then
                _Products.Sort(New GenericComparer(Of Product)("PriceForSorting", GenericComparer(Of Product).SortOrder.Ascending))
            Else
                _Products.Sort(New GenericComparer(Of Product)("PriceForSorting", GenericComparer(Of Product).SortOrder.Descending))

            End If
        End Sub

        Public Sub SortProductsByName(ByVal order As String)
            If order = "A" Then
                _Products.Sort(New GenericComparer(Of Product)("Description1", GenericComparer(Of Product).SortOrder.Ascending))
            Else
                _Products.Sort(New GenericComparer(Of Product)("Description1", GenericComparer(Of Product).SortOrder.Descending))
            End If
        End Sub
        Public Sub SortProductsByBestSeller(ByVal order As String)
            If order = "A" Then
                _Products.Sort(New GenericComparer(Of Product)("SequenceNo", GenericComparer(Of Product).SortOrder.Ascending))
            Else
                _Products.Sort(New GenericComparer(Of Product)("SequenceNo", GenericComparer(Of Product).SortOrder.Descending))

            End If
        End Sub

        Public ReadOnly Property ProductsEnum() As IEnumerator
            Get
                Return _Products.GetEnumerator()
            End Get
        End Property

        Public Sub Clear()
            _Products.Clear()
        End Sub

    End Class

End Namespace

