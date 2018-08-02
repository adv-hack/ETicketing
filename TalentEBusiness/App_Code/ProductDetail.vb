Imports Microsoft.VisualBasic
Imports System.Web.SessionState
Imports System.Data
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports Talent.Common
Imports TalentBusinessLogic.Models

''' <summary>
''' A helper and centralised class for productdetail.ascx.vb and it related controls
''' </summary>
Public Class ProductDetail
    Inherits ClassBase01

#Region "Class Level Fields"

    Private _productType As String = String.Empty
    Private _productSubType As String = String.Empty
    Private _httpSession As HttpSessionState = HttpContext.Current.Session
    Private _moduleDefaults As ECommerceModuleDefaults
    Private _moduleDefaultsValue As ECommerceModuleDefaults.DefaultValues
    Private _productCode As String = String.Empty
    Private _productDetailCode As String = String.Empty
    Private _isSingleProductFilter As Boolean = False
    Const EMPTYSTRING As String = ""

#End Region

#Region "Constructor"
    Public Sub New()
        _moduleDefaults = New ECommerceModuleDefaults()
        _moduleDefaultsValue = _moduleDefaults.GetDefaults
    End Sub
#End Region

#Region "Properties"

    Public WriteOnly Property ProductType() As String
        Set(ByVal value As String)
            _productType = value
        End Set
    End Property

    Public WriteOnly Property ProductSubType() As String
        Set(ByVal value As String)
            _productSubType = value
        End Set
    End Property

    Public WriteOnly Property ProductCode() As String
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Public WriteOnly Property ProductDetailCode() As String
        Set(ByVal value As String)
            _productDetailCode = value
        End Set
    End Property

    Public WriteOnly Property IsSingleProductFilter() As Boolean
        Set(ByVal value As Boolean)
            _isSingleProductFilter = value
        End Set
    End Property
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Gets the row filter condition for the product list
    ''' </summary>
    ''' <returns></returns>
    Public Function GetRowFilterCondition(Optional ByVal pageNumber As String = EMPTYSTRING) As String
        Dim rowFilterCondition As String = String.Empty
        If _productSubType Is Nothing Then _productSubType = String.Empty

        Select Case _productType
            Case GlobalConstants.HOMEPRODUCTTYPE
                rowFilterCondition = "(ProductType='" + _productType + "' OR IsProductBundle = 'True') AND (ProductHomeAsAway<>'Y')"
                If _productSubType.Length > 0 Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "')"
                Else
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "' OR ProductSubType IS NULL)"
                End If
                If Not String.IsNullOrEmpty(_httpSession("LocationSearch")) And _httpSession("LocationSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (location='" & _httpSession("LocationSearch") & "')"
                End If
                If Not String.IsNullOrEmpty(_httpSession("StadiumDescriptionSearch")) And _httpSession("StadiumDescriptionSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductStadiumDescription='" & _httpSession("StadiumDescriptionSearch") & "')"
                End If
                If Not String.IsNullOrEmpty(_httpSession("DateSearch")) And _httpSession("DateSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductMDTE08='" & _httpSession("DateSearch") & "')"
                End If

            Case GlobalConstants.AWAYPRODUCTTYPE
                rowFilterCondition = "((ProductType='" + _productType + "') OR (ProductType='H' AND ProductHomeAsAway='Y'))"
                If _productSubType.Length > 0 Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "')"
                Else
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "' OR ProductSubType IS NULL)"
                End If
                If Not String.IsNullOrEmpty(_httpSession("LocationSearch")) And _httpSession("LocationSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (location='" & _httpSession("LocationSearch") & "')"
                End If
                If Not String.IsNullOrEmpty(_httpSession("StadiumDescriptionSearch")) And _httpSession("StadiumDescriptionSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductStadiumDescription='" & _httpSession("StadiumDescriptionSearch") & "')"
                End If
                If Not String.IsNullOrEmpty(_httpSession("DateSearch")) And _httpSession("DateSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductMDTE08='" & _httpSession("DateSearch") & "')"
                End If

            Case GlobalConstants.TRAVELPRODUCTTYPE, GlobalConstants.EVENTPRODUCTTYPE
                rowFilterCondition = "ProductType='" + _productType + "' AND (ProductHomeAsAway<>'Y') "
                If _productSubType.Length > 0 Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "')"
                Else
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "' OR ProductSubType IS NULL)"
                End If
                If pageNumber = EMPTYSTRING Then
                    If Not String.IsNullOrEmpty(_httpSession("AgeSearch")) And _httpSession("AgeSearch") <> "0" Then
                        Dim age() As String = _httpSession("AgeSearch").ToString.Split(_httpSession("AgeRangeDividerText"))
                        rowFilterCondition = rowFilterCondition & " AND (ageRangeFrm='" & age(0) & "') AND (ageRangeTo='" & age(1) & "')"
                    End If
                    If Not String.IsNullOrEmpty(_httpSession("DescriptionSearch")) And _httpSession("DescriptionSearch") <> "0" Then
                        If _productType = "T" Then
                            rowFilterCondition = rowFilterCondition & " AND (ProductDescription2='" & _httpSession("DescriptionSearch") & "')"
                        ElseIf _productType = "E" Then
                            rowFilterCondition = rowFilterCondition & " AND (ProductDescription='" & _httpSession("DescriptionSearch") & "')"
                        End If
                    End If
                    If Not String.IsNullOrEmpty(_httpSession("LocationSearch")) And _httpSession("LocationSearch") <> "0" Then
                        rowFilterCondition = rowFilterCondition & " AND (location='" & _httpSession("LocationSearch") & "')"
                    End If
                    If Not String.IsNullOrEmpty(_httpSession("DateSearch")) And _httpSession("DateSearch") <> "0" Then
                        rowFilterCondition = rowFilterCondition & " AND (ProductMDTE08='" & _httpSession("DateSearch") & "')"
                    End If
                    If Not String.IsNullOrEmpty(_httpSession("DurationSearch")) And _httpSession("DurationSearch") <> "0" Then
                        rowFilterCondition = rowFilterCondition & " AND (duration='" & _httpSession("DurationSearch") & "')"
                    End If
                End If

            Case "CH"
                rowFilterCondition = "ProductType='" + "H" + "' AND (ProductHomeAsAway<>'Y')"

            Case GlobalConstants.SEASONTICKETPRODUCTTYPE
                rowFilterCondition = "(ProductType='" + _productType + "' AND IsProductBundle = 'False') AND (ProductHomeAsAway<>'Y')"
                If _productSubType.Length > 0 Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "')"
                Else
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "' OR ProductSubType IS NULL)"
                End If
                If Not String.IsNullOrEmpty(_httpSession("LocationSearch")) And _httpSession("LocationSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (location='" & _httpSession("LocationSearch") & "')"
                End If
                If Not String.IsNullOrEmpty(_httpSession("StadiumDescriptionSearch")) And _httpSession("StadiumDescriptionSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductStadiumDescription='" & _httpSession("StadiumDescriptionSearch") & "')"
                End If
                If Not String.IsNullOrEmpty(_httpSession("DateSearch")) And _httpSession("DateSearch") <> "0" Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductMDTE08='" & _httpSession("DateSearch") & "')"
                End If

            Case GlobalConstants.MEMBERSHIPPRODUCTTYPE
                rowFilterCondition = "ProductType='" + _productType + "' AND (ProductHomeAsAway<>'Y')"
                If _productSubType.Length > 0 Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "')"
                Else
                    rowFilterCondition = rowFilterCondition & " AND (ProductSubType='" & _productSubType & "' OR ProductSubType IS NULL)"
                End If
                If Not String.IsNullOrEmpty(ModuleDefaults.EPurseTopUpProductCode) Then 'Filter out the top up product
                    rowFilterCondition = rowFilterCondition & " AND (ProductCode <> '" & ModuleDefaults.EPurseTopUpProductCode & "')"
                End If

            Case Else
                rowFilterCondition = "ProductType='" + _productType + "' AND (ProductHomeAsAway<>'Y')"

        End Select

        If _isSingleProductFilter Then
            If rowFilterCondition.Length > 0 Then
                rowFilterCondition = rowFilterCondition & " AND (ProductCode='" & _productCode & "')"
            Else
                rowFilterCondition = "ProductCode='" & _productCode & "'"
            End If
            If _productType = "T" Then
                If _productDetailCode.Length > 0 Then
                    rowFilterCondition = rowFilterCondition & " AND (ProductDetailCode='" & _productDetailCode & "')"
                End If
            End If
            If HttpContext.Current.Session("PartnerPromotionCode") Is Nothing Then
                rowFilterCondition = rowFilterCondition & " AND (ExcludeProductFromWebSales = 'False')"
            End If
        Else
            rowFilterCondition = rowFilterCondition & " AND (ExcludeProductFromWebSales = 'False')"
        End If

        Return rowFilterCondition
    End Function

    ''' <summary>
    ''' Clears the advance product filter session variables when page querystring isn't being used
    ''' </summary>
    Public Sub ClearAdvPdtFilterSession()
        _httpSession("AdvancedSearchInUse") = False
        _httpSession("AgeSearch") = String.Empty
        _httpSession("DescriptionSearch") = String.Empty
        _httpSession("LocationSearch") = String.Empty
        _httpSession("StadiumDescriptionSearch") = String.Empty
        _httpSession("DateSearch") = String.Empty
        _httpSession("DateSearchForCalendar") = String.Empty
        _httpSession("DurationSearch") = String.Empty
        _httpSession("AgeRangeDividerText") = String.Empty
    End Sub

    ''' <summary>
    ''' Gets the page number as 1 if the page gets submitted from advance product filter
    ''' </summary>
    ''' <param name="pageNumber">The page number.</param>
    ''' <returns></returns>
    Public Function GetPageNumber(ByVal pageNumber As String) As String
        If String.IsNullOrEmpty(_httpSession("ClearPageNumberQueryString")) Then
            pageNumber = pageNumber
        Else
            _httpSession.Remove("ClearPageNumberQueryString")
            pageNumber = "1"
        End If
        Return pageNumber
    End Function

    ''' <summary>
    ''' Return a css class name if the product type is a product bundle
    ''' </summary>
    ''' <param name="isProductBundle">Is this a product bundle</param>
    ''' <returns>CSS class name string</returns>
    ''' <remarks></remarks>
    Public Shared Function GetProductBundleCSSClassName(ByVal isProductBundle As Boolean) As String
        Dim cssClassName As String = String.Empty
        If isProductBundle Then cssClassName = " ebiz-product-bundle"
        Return cssClassName
    End Function

    ''' <summary>
    ''' Get the image URL for the given product code and type
    ''' </summary>
    ''' <param name="p1Value">The image type</param>
    ''' <param name="productcode">The product Code</param>
    ''' <returns>The formatted URL</returns>
    ''' <remarks></remarks>
    Public Shared Function GetImageURL(ByVal p1Value As String, ByVal productcode As String) As String
        Dim str As String = ImagePath.getImagePath(p1Value, productcode, TalentCache.GetBusinessUnit, TalentCache.GetPartner(CType(HttpContext.Current.Profile, TalentProfile)))
        Return str
    End Function

    ''' <summary>
    ''' Get the sponsored by product text based on the given product type
    ''' </summary>
    ''' <param name="sProductType">The product type of the current product</param>
    ''' <returns>The formatted text</returns>
    ''' <remarks></remarks>
    Public Shared Function GetSponsoredByText(ByVal sProductType As String) As String
        Dim ucr As New Talent.Common.UserControlResource
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductDetail.ascx"
        End With
        Dim str As String = ucr.Content("sponsoredbyTextForProductType" & sProductType, Talent.Common.Utilities.GetDefaultLanguage, True)
        Return str
    End Function

    ''' <summary>
    ''' Get the availability total based on the given sales figures for a product
    ''' </summary>
    ''' <param name="capacityCount">The capacity value</param>
    ''' <param name="returnsCount">The number of returned tickets</param>
    ''' <param name="salesCount">The number of sold tickets</param>
    ''' <param name="unavailableCount">The number of unavailable tickets</param>
    ''' <param name="bookingsCount">The number of booked tickets</param>
    ''' <param name="reservationsCount">The number of reserved tickets</param>
    ''' <returns>The overall availability total value</returns>
    ''' <remarks></remarks>
    Public Function AvailabilityTotal(ByVal capacityCount As String, ByVal returnsCount As String, ByVal salesCount As String,
        ByVal unavailableCount As String, ByVal bookingsCount As String, ByVal reservationsCount As String) As Integer
        Dim total As Integer = 0
        Try
            Dim intCapacityCount As Integer = CInt(capacityCount)
            Dim intReturnsCount As Integer = CInt(returnsCount)
            Dim intSalesCount As Integer = CInt(salesCount)
            Dim intUnavailableCount As Integer = CInt(unavailableCount)
            Dim intBookingsCount As Integer = CInt(bookingsCount)
            Dim intReservationsCount As Integer = CInt(reservationsCount)
            If intCapacityCount > 0 Then
                total = intCapacityCount + intReturnsCount
                total = total - intSalesCount - intUnavailableCount - intBookingsCount - intReservationsCount
            End If
        Catch
        End Try
        Return total
    End Function

    ''' <summary>
    ''' Get the product availability figures for Travel, Event, Away, type Products.
    ''' </summary>
    Public Sub getProductAvailabilityFigures_TAE(ByVal productCode As String, ByVal productType As String,
                                                 ByVal priceCode As String, ByVal productItemCode As String,
                                                 ByRef availability As Integer,
                                                 ByRef percentageAvailable As Integer,
                                                 ByRef capacity As Integer,
                                                 ByRef returned As Integer,
                                                 ByRef sold As Integer,
                                                 ByRef reserved As Integer,
                                                 ByRef booked As Integer,
                                                 ByRef unavailable As Integer)
        Dim availabilityPercentage As Integer = 0
        Dim err As New ErrorObj
        Dim talProduct As New TalentProduct
        Dim de As New DEProductDetails
        Dim settings As DESettings = GetSettingsObject()
        de.Src = GlobalConstants.SOURCE
        talProduct.De = de
        talProduct.Settings = settings

        Select Case productType
            Case GlobalConstants.TRAVELPRODUCTTYPE
                err = talProduct.TravelProductAvailability()
                If Not err.HasError AndAlso talProduct.ResultDataSet IsNot Nothing AndAlso talProduct.ResultDataSet.Tables("TravelProductAvailability").Rows.Count > 0 Then
                    For Each row As DataRow In talProduct.ResultDataSet.Tables("TravelProductAvailability").Rows
                        If row("ProductCode").ToString.Trim = productCode.Trim AndAlso row("TravelItemCode").ToString = productItemCode.Trim Then
                            popAvailabilityResults(row, availability, percentageAvailable, capacity, returned, sold, reserved, booked, unavailable)
                            Exit For
                        End If
                    Next
                End If
            Case GlobalConstants.EVENTPRODUCTTYPE
                err = talProduct.EventProductAvailability()
                If Not err.HasError AndAlso talProduct.ResultDataSet IsNot Nothing AndAlso talProduct.ResultDataSet.Tables("EventProductAvailability").Rows.Count > 0 Then
                    For Each row As DataRow In talProduct.ResultDataSet.Tables("EventProductAvailability").Rows
                        If row("ProductCode").ToString.Trim = productCode.Trim Then
                            popAvailabilityResults(row, availability, percentageAvailable, capacity, returned, sold, reserved, booked, unavailable)
                            Exit For
                        End If
                    Next
                End If
            Case GlobalConstants.AWAYPRODUCTTYPE
                err = talProduct.AwayProductAvailability()
                If Not err.HasError AndAlso talProduct.ResultDataSet IsNot Nothing AndAlso talProduct.ResultDataSet.Tables("AwayProductAvailability").Rows.Count > 0 Then
                    For Each row As DataRow In talProduct.ResultDataSet.Tables("AwayProductAvailability").Rows
                        If row("ProductCode").ToString.Trim = productCode.Trim AndAlso row("PriceCode").ToString.Trim = priceCode.Trim Then
                            popAvailabilityResults(row, availability, percentageAvailable, capacity, returned, sold, reserved, booked, unavailable)
                            Exit For
                        End If
                    Next
                End If
        End Select
    End Sub

    Private Sub popAvailabilityResults(ByVal row As DataRow, ByRef availability As Integer,
                                       ByRef percentageAvailable As Integer, ByRef capacity As Integer,
                                       ByRef returned As Integer, ByRef sold As Integer,
                                       ByRef reserved As Integer, ByRef booked As Integer,
                                       ByRef unavailable As Integer)
        availability = row("Availability")
        percentageAvailable = row("Percentage")
        capacity = row("Capacity")
        returned = row("Returned")
        sold = row("Sold")
        reserved = row("Reserved")
        booked = row("Booked")
        unavailable = row("Unavailable")

        'When the capacity is too high and too few seats are left the percentage is calculated as 0.xx
        'And as the percentage is integer value, we receive 0. To prevent availability label to show 'Sold Out' 
        'set the percentage 1 if the available seats are > 0
        If percentageAvailable = 0 And availability > 0 Then
            percentageAvailable = 1
        End If

    End Sub

    Public Function setAvailabilityMask(ByVal availabilityMask As String, ByVal availabilityStatus As String, ByVal productType As String,
                                                  ByRef availability As Integer,
                                                  ByRef percentageAvailable As Integer,
                                                  ByRef capacity As Integer,
                                                  ByRef returned As Integer,
                                                  ByRef sold As Integer,
                                                  ByRef reserved As Integer,
                                                  ByRef booked As Integer,
                                                  ByRef unavailable As Integer,
                                                  ByRef productItemDec As String) As String

        Dim availabilityString As New String("")

        'check for null to avoide exception
        If availabilityStatus Is Nothing Then
            availabilityString = String.Empty
        Else
            'The status is the default availability
            availabilityString = availabilityStatus
        End If

        'If the status is already a mask, don not retrieve a product type mask.
        If Not availabilityString.Contains("<<") Then
            If Not availabilityMask = String.Empty Then
                availabilityString = availabilityMask.Replace("<<Status>>", availabilityStatus)
            End If
        End If

        'Is the status a mask to be populated now.
        If availabilityString.Contains("<<") Then
            availabilityString = availabilityString.Replace("<<Percentage>>", percentageAvailable.ToString)
            availabilityString = availabilityString.Replace("<<Returned>>", returned.ToString)
            availabilityString = availabilityString.Replace("<<Available>>", availability.ToString)
            availabilityString = availabilityString.Replace("<<Capacity>>", capacity.ToString)
            availabilityString = availabilityString.Replace("<<Sold>>", sold.ToString)
            availabilityString = availabilityString.Replace("<<Reserved>>", reserved.ToString)
            availabilityString = availabilityString.Replace("<<Booked>>", booked.ToString)
            availabilityString = availabilityString.Replace("<<Unavailable>>", unavailable.ToString)
            availabilityString = availabilityString.Replace("<<ProductItemDesc>>", productItemDec.ToString)
        End If

        Return availabilityString
    End Function

    ''' <summary>
    ''' Get the availability properties for this stadium and percentage.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub getAvailabilityProperties(ByVal businessUnit As String, ByRef availabilityText As String,
                                         ByRef availabilityCSS As String, ByRef availabilityColor As String,
                                         ByVal percent As Integer, ByVal stadiumCode As String)
        Dim dtStadiumAvailability As DataTable = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetStadiumAvailabilityColoursAndText(businessUnit, stadiumCode)
        If dtStadiumAvailability.Rows.Count > 0 Then
            Dim i As Integer = 0
            Dim dtRow As DataRow
            Do While i < dtStadiumAvailability.Rows.Count
                dtRow = dtStadiumAvailability.Rows(i)
                If percent >= CType(dtRow("MIN"), Integer) And percent <= CType(dtRow("MAX"), Integer) Then
                    availabilityText = dtRow("TEXT").ToString.Trim
                    availabilityCSS = dtRow("CSS_CLASS").ToString.Trim
                    availabilityColor = dtRow("COLOUR").ToString.Trim
                    Exit Do
                End If
                i = i + 1
            Loop
        End If
    End Sub

#End Region

End Class
