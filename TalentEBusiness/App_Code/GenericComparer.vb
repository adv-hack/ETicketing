Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections
Imports System.Text
Imports System.Reflection
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    GenericComparer. Used for sorting arraylists and generic 
'                                   collections. Cribbed from http://www.vbdotnetheaven.com
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
Namespace Talent.eCommerce
    '---------------------------------------
    ' This class is used to compare any
    ' type(property) of a class for sorting.
    ' This class automatically fetches the
    ' type of the property and compares.
    '---------------------------------------
    Public NotInheritable Class GenericComparer(Of T)
        Implements IComparer(Of T)
        Public Enum SortOrder
            Ascending
            Descending
        End Enum

        Private sortColumn_Renamed As String
        Private sortingOrder_Renamed As SortOrder

        Public Sub New(ByVal sortColumn_Renamed As String, ByVal sortingOrder_Renamed As SortOrder)
            Me.sortColumn_Renamed = sortColumn_Renamed
            Me.sortingOrder_Renamed = sortingOrder_Renamed
        End Sub

        '--------------------------------------------------------
        ' Column Name(public property of the class) to be sorted.
        '--------------------------------------------------------
        Public ReadOnly Property SortColumn() As String
            Get
                Return sortColumn_Renamed
            End Get
        End Property
        '---------------
        ' Sorting order.
        '---------------
        Public ReadOnly Property SortingOrder() As SortOrder
            Get
                Return sortingOrder_Renamed
            End Get
        End Property
        '---------------------------------
        ' Compare interface implementation
        '---------------------------------
        Public Function Compare(ByVal x As T, ByVal y As T) As Integer Implements IComparer(Of T).Compare
            Dim propertyInfo As PropertyInfo = GetType(T).GetProperty(sortColumn_Renamed)
            Dim obj1 As IComparable = CType(propertyInfo.GetValue(x, Nothing), IComparable)
            Dim obj2 As IComparable = CType(propertyInfo.GetValue(y, Nothing), IComparable)
            If sortingOrder_Renamed = SortOrder.Ascending Then
                Return (obj1.CompareTo(obj2))
            Else
                Return (obj2.CompareTo(obj1))
            End If
        End Function
    End Class
End Namespace
