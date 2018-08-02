Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.Models
Imports System.Collections.Generic
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports System.Linq

Partial Class PagesPublic_Hospitality_HospitalityBundlePackages
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _viewModelProductGroup As HospitalityProductGroupViewModel = Nothing

#End Region

#Region "Public Properties"
    ''' <summary>
    ''' Product group code
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductGroupCode() As String = String.Empty
#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.QueryString("productGroup") IsNot Nothing Then
            ProductGroupCode = Request.QueryString("productGroup").ToString()
        End If

        uscHospitalityBundlePackageHeader.ProductGroupCode = ProductGroupCode
        uscHospitalityPackages.ProductGroupCode = ProductGroupCode
    End Sub

#End Region

End Class
