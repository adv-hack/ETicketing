Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class PagesPublic_Hospitality_HospitalityFixturePackages
    Inherits TalentBase01

#Region "Class Level Fields"
#End Region

#Region "Public Properties"
    Public Property ProductCode() As String = String.Empty
#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.QueryString("product") IsNot Nothing Then
            ProductCode = Request.QueryString("product").ToString()
        End If
        uscHospitalityFixturePackageHeader.ProductCode = ProductCode
        uscHospitalityPackages.ProductCode = ProductCode
    End Sub
#End Region
End Class