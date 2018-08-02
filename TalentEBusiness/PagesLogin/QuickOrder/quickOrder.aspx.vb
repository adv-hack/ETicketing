Imports Microsoft.VisualBasic
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Order Template
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      PLOTEMP- 
'                                    
'       User Controls
'           orderTemplate
'
'           Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class PagesLogin_QuickOrder_QuickOrder
    Inherits TalentBase01

#Region "Public Properties"

    Public Property ProductID() As String = String.Empty
    Public Property Quantity() As Integer = 0

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not String.IsNullOrEmpty(Request.QueryString("product")) Then
            ProductID = Request.QueryString("product")
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("quant")) Then
            Quantity = Integer.Parse(Request.QueryString("quant"))
        End If
        QuickOrder1.ProductID = ProductID
        QuickOrder1.Quantity = Quantity
    End Sub



#End Region
End Class
