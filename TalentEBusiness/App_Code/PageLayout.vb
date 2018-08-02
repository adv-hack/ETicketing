Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Page Left Navigation
'
'       Date                        19th Jan 2007
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      ACPLN- 
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       14/02/07            adw     also used by UserControls_PageFooterNav
'       14/02/07            adw     also used by UserControls_PageTopNav
'       14/02/07            adw     also used by UserControls_PageHeaderNav
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.eCommerce

    Public Class PageLayout

        Private _productNav As Boolean = False
        Private _grouping As Boolean = False
        Private _myAccountNav As Boolean = False

        Private _LeftNav1 As Boolean = False
        Private _LeftNav2 As Boolean = False
        Private _LeftNav3 As Boolean = False
        Private _LeftNav4 As Boolean = False
        Private _LeftNav5 As Boolean = False

        Private _StandardNav1 As Boolean = False
        Private _StandardNav2 As Boolean = False
        Private _StandardNav3 As Boolean = False
        Private _StandardNav4 As Boolean = False
        Private _StandardNav5 As Boolean = False
        Private _StandardNav6 As Boolean = False

        Private _RightNav1 As Boolean = False
        Private _RightNav2 As Boolean = False
        Private _RightNav3 As Boolean = False
        Private _RightNav4 As Boolean = False
        Private _RightNav5 As Boolean = False

        Private _basket As Integer = 0
        Private _checkOut As Integer = 0
        Private _contactUs As Integer = 0
        Private _logout As Integer = 0
        Private _myAccount As Integer = 0
        Private _quickOrder As Integer = 0

        Public Property ProductNav() As Boolean
            Get
                Return _productNav
            End Get
            Set(ByVal value As Boolean)
                _productNav = value
            End Set
        End Property
        Public Property Grouping() As Boolean
            Get
                Return _grouping
            End Get
            Set(ByVal value As Boolean)
                _grouping = value
            End Set
        End Property

        Public Property MyAccountNav() As Boolean
            Get
                Return _myAccountNav
            End Get
            Set(ByVal value As Boolean)
                _myAccountNav = value
            End Set
        End Property

        Public Property LeftNav1() As Boolean
            Get
                Return _LeftNav1
            End Get
            Set(ByVal value As Boolean)
                _LeftNav1 = value
            End Set
        End Property
        Public Property LeftNav2() As Boolean
            Get
                Return _LeftNav2
            End Get
            Set(ByVal value As Boolean)
                _LeftNav2 = value
            End Set
        End Property
        Public Property LeftNav3() As Boolean
            Get
                Return _LeftNav3
            End Get
            Set(ByVal value As Boolean)
                _LeftNav3 = value
            End Set
        End Property
        Public Property LeftNav4() As Boolean
            Get
                Return _LeftNav4
            End Get
            Set(ByVal value As Boolean)
                _LeftNav4 = value
            End Set
        End Property
        Public Property LeftNav5() As Boolean
            Get
                Return _LeftNav5
            End Get
            Set(ByVal value As Boolean)
                _LeftNav5 = value
            End Set
        End Property

        Public Property StandardNav1() As Boolean
            Get
                Return _StandardNav1
            End Get
            Set(ByVal value As Boolean)
                _StandardNav1 = value
            End Set
        End Property
        Public Property StandardNav2() As Boolean
            Get
                Return _StandardNav2
            End Get
            Set(ByVal value As Boolean)
                _StandardNav2 = value
            End Set
        End Property
        Public Property StandardNav3() As Boolean
            Get
                Return _StandardNav3
            End Get
            Set(ByVal value As Boolean)
                _StandardNav3 = value
            End Set
        End Property
        Public Property StandardNav4() As Boolean
            Get
                Return _StandardNav4
            End Get
            Set(ByVal value As Boolean)
                _StandardNav4 = value
            End Set
        End Property
        Public Property StandardNav5() As Boolean
            Get
                Return _StandardNav5
            End Get
            Set(ByVal value As Boolean)
                _StandardNav5 = value
            End Set
        End Property
        Public Property StandardNav6() As Boolean
            Get
                Return _StandardNav6
            End Get
            Set(ByVal value As Boolean)
                _StandardNav6 = value
            End Set
        End Property

        Public Property RightNav1() As Boolean
            Get
                Return _RightNav1
            End Get
            Set(ByVal value As Boolean)
                _RightNav1 = value
            End Set
        End Property
        Public Property RightNav2() As Boolean
            Get
                Return _RightNav2
            End Get
            Set(ByVal value As Boolean)
                _RightNav2 = value
            End Set
        End Property
        Public Property RightNav3() As Boolean
            Get
                Return _RightNav3
            End Get
            Set(ByVal value As Boolean)
                _RightNav3 = value
            End Set
        End Property
        Public Property RightNav4() As Boolean
            Get
                Return _RightNav4
            End Get
            Set(ByVal value As Boolean)
                _RightNav4 = value
            End Set
        End Property
        Public Property RightNav5() As Boolean
            Get
                Return _RightNav5
            End Get
            Set(ByVal value As Boolean)
                _RightNav5 = value
            End Set
        End Property

        Public Property Basket() As Integer
            Get
                Return _basket
            End Get
            Set(ByVal value As Integer)
                _basket = value
            End Set
        End Property
        Public Property CheckOut() As Integer
            Get
                Return _checkOut
            End Get
            Set(ByVal value As Integer)
                _checkOut = value
            End Set
        End Property
        Public Property ContactUs() As Integer
            Get
                Return _contactUs
            End Get
            Set(ByVal value As Integer)
                _contactUs = value
            End Set
        End Property
        Public Property Logout() As Integer
            Get
                Return _logout
            End Get
            Set(ByVal value As Integer)
                _logout = value
            End Set
        End Property
        Public Property MyAccount() As Integer
            Get
                Return _myAccount
            End Get
            Set(ByVal value As Integer)
                _myAccount = value
            End Set
        End Property
        Public Property QuickOrder() As Integer
            Get
                Return _quickOrder
            End Get
            Set(ByVal value As Integer)
                _quickOrder = value
            End Set
        End Property

    End Class
End Namespace
