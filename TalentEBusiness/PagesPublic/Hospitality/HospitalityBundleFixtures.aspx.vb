
Partial Class PagesPublic_Hospitality_HospitalityBundleFixtures
    Inherits TalentBase01

#Region "Class Variables"

    Private _pageCode As String = String.Empty

#End Region

#Region "Public Properties"

    ''' <summary>
    ''' Product group code
    ''' </summary>
    ''' <returns></returns>
    Public Property ProductGroupCode() As String = String.Empty

    ''' <summary>
    ''' Package Id
    ''' </summary>
    ''' <returns></returns>
    Public Property PackageId() As String = String.Empty

    ''' <summary>
    ''' Availability Component Id
    ''' </summary>
    ''' <returns></returns>
    Public Property AvailabilityComponentId() As Long = 0

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _pageCode = "HospitalityBundleFixtures.aspx"
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.QueryString("productGroupCode") IsNot Nothing Then
            ProductGroupCode = Request.QueryString("productGroupCode").ToString()
        End If

        If Request.QueryString("packageId") IsNot Nothing Then
            PackageId = Request.QueryString("packageId").ToString()
        End If

        If Request.QueryString("availabilitycomponentid") IsNot Nothing Then
            AvailabilityComponentId = Request.QueryString("availabilitycomponentid").ToString()
        End If

        createView()

    End Sub


#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Create the hospitality bundle fixtures view
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()

        'HTML Includes
        uscHospitalityBundlePackageHeader.ProductGroupCode = ProductGroupCode
        uscHospitalityPackageHTMLInclude1.PackageID = PackageId
        uscHospitalityPackageHTMLInclude1.ProductCode = ProductGroupCode
        uscHospitalityPackageHTMLInclude2.PackageID = PackageId
        uscHospitalityPackageHTMLInclude2.ProductCode = ProductGroupCode
        uscHospitalityPackageHTMLInclude3.PackageID = PackageId
        uscHospitalityPackageHTMLInclude3.ProductCode = ProductGroupCode
        uscHospitalityPackageHTMLInclude4.PackageID = PackageId
        uscHospitalityPackageHTMLInclude4.ProductCode = ProductGroupCode

        'HospitalityFixtures
        uscHospitalityFixtures.ProductType = GlobalConstants.HOMEPRODUCTTYPE
        uscHospitalityFixtures.PackageId = PackageId
        uscHospitalityFixtures.ProductGroupCode = ProductGroupCode
        uscHospitalityFixtures.Usage = _pageCode

    End Sub

#End Region

End Class
