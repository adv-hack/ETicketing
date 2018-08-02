Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_Hospitality_HospitalityPackageHTMLInclude
    Inherits ControlBase

#Region "Class Level Fields"
    Private _businessUnit As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _isPackageHtmlIncluded As Boolean = False
#End Region

#Region "Public Properties"
    Public Property Sequence() As String
    Public Property PackageID() As String
    Public Property PackageCode() As String
    Public Property ProductCode() As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        loadPackageHtmlInclude()
    End Sub
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Load the HTML content for the fixture/package combination
    ''' </summary>    
    ''' <remarks>
    ''' The HTML Include file is fetched from the location: <![CDATA[<Path specified by HTML_PATH_ABSOLUTE in tbl_ecommerce_module_defaults_bu> \ <BusinessUnit> \ <PartnerCode> \ Product \ Corporate]]>
    ''' File search is done by the many if statements and the order of file search is as follows
    ''' 1) [package_code]_[product_code]_[sequence].htm/html
    ''' 2) [package_id]_[product_code]_[sequence].htm/html
    ''' 3) [package_id]_[sequence].htm/html
    ''' 4) [package_code]_[sequence].htm/html
    ''' The htm variant of the file is searched before the html one in each case
    ''' </remarks>
    Private Sub loadPackageHtmlInclude()
        Const pathSeperator As String = "/"
        Const htmlPath As String = "Product/Corporate"
        Const htmExtension As String = ".htm"
        Const htmlExtension As String = ".html"
        Const fileInfoSeperator As String = "_"
        Dim sbHTMLPath As New StringBuilder

        'Search for the HTML Include file
        If Not _isPackageHtmlIncluded AndAlso PackageCode <> String.Empty AndAlso ProductCode <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageCode).Append(fileInfoSeperator).Append(ProductCode).Append(fileInfoSeperator).Append(Sequence).Append(htmExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageCode <> String.Empty AndAlso ProductCode <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageCode).Append(fileInfoSeperator).Append(ProductCode).Append(fileInfoSeperator).Append(Sequence).Append(htmlExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageID <> String.Empty AndAlso ProductCode <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageID).Append(fileInfoSeperator).Append(ProductCode).Append(fileInfoSeperator).Append(Sequence).Append(htmExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageID <> String.Empty AndAlso ProductCode <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageID).Append(fileInfoSeperator).Append(ProductCode).Append(fileInfoSeperator).Append(Sequence).Append(htmlExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageID <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageID).Append(fileInfoSeperator).Append(Sequence).Append(htmExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageID <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageID).Append(fileInfoSeperator).Append(Sequence).Append(htmlExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageCode <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageCode).Append(fileInfoSeperator).Append(Sequence).Append(htmExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        If Not _isPackageHtmlIncluded AndAlso PackageCode <> String.Empty Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append(PackageCode).Append(fileInfoSeperator).Append(Sequence).Append(htmlExtension)
            includePackageHtml(sbHTMLPath.ToString())
        End If
        'Following is temporary code which loads a default HTML Include for a specific Package and Fixture
        'It is added for testing purposes and can be removed once the HTMLs for the Fixtures and Product are placed in the assets folder
        If Not _isPackageHtmlIncluded Then
            sbHTMLPath.Clear()
            sbHTMLPath.Append(_businessUnit).Append(pathSeperator).Append(_partnerCode).Append(pathSeperator).Append(htmlPath).Append(pathSeperator)
            sbHTMLPath.Append("defaultHospitalityPackageHTMLInclude").Append(fileInfoSeperator).Append(Sequence).Append(htmlExtension)
            includePackageHtml(sbHTMLPath.ToString())
            ltlHtmlIncludeLabel.Text = ltlHtmlIncludeLabel.Text.Replace("<<PackageID>>", PackageID)
            ltlHtmlIncludeLabel.Text = ltlHtmlIncludeLabel.Text.Replace("<<ProductCode>>", ProductCode)
        End If
    End Sub

    ''' <summary>
    ''' Retrieve the HTML content specified at the given path if it exists
    ''' </summary>
    ''' <param name="htmlPath">HTML path of the HTML Include</param>    
    ''' <remarks></remarks>
    Private Sub includePackageHtml(ByVal htmlPath As String)
        Dim packageHtmlContent As String = String.Empty
        packageHtmlContent = GetHtmlFromFile(htmlPath)
        If Not (String.IsNullOrEmpty(packageHtmlContent)) Then
            ltlHtmlIncludeLabel.Text = packageHtmlContent
            _isPackageHtmlIncluded = True
        Else
            _isPackageHtmlIncluded = False
        End If
    End Sub
#End Region

End Class
