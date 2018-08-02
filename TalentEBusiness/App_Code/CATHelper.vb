Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.HttpServerUtility
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities


Namespace Talent.eCommerce

    Public Class CATHelper

        Public Const SEAT_STATUS_NEW As String = "N"
        Public Const SEAT_STATUS_TRANSFER As String = "T"
        Public Const SEAT_STATUS_CANCEL As String = "C"

        Public Shared Function GetPackageComponentId(ByVal product As String, ByVal callID As String) As String
            'Dim callID As String = HttpContext.Current.Request("callid")
            If Not String.IsNullOrWhiteSpace(callID) AndAlso callID <> "0" Then
                If Not HttpContext.Current.Session(callID.ToString & "ComponentID") Is Nothing Then
                    Return HttpContext.Current.Session(callID.ToString & "ComponentID")
                Else
                    GetPackageComponentSeats(Nothing, callID, product)
                    If Not HttpContext.Current.Session(callID.ToString & "ComponentID") Is Nothing Then
                        Return HttpContext.Current.Session(callID.ToString & "ComponentID")
                    Else
                        Return "0"
                    End If
                End If
            Else
                Return "0"
            End If
            Return String.Empty
        End Function

        Public Shared Function GetPackageComponentSeats(ByRef dtPackageSeats As DataTable, ByVal callID As String, ByVal product As String) As ErrorObj
            Dim err As New ErrorObj
            If Not String.IsNullOrWhiteSpace(callID) AndAlso Not String.IsNullOrWhiteSpace(product) Then
                Dim package As New Talent.Common.TalentPackage
                package.DePackages.PackageID = callID
                package.Settings() = Utilities.GetSettingsObject()
                package.DePackages.ProductCode = product
                package.DePackages.BasketId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
                package.DePackages.BoxOfficeUser = package.Settings.AgentEntity.AgentUsername
                err = package.GetComponentSeats()
                If Not err.HasError AndAlso package.ResultDataSet IsNot Nothing AndAlso package.ResultDataSet.Tables("Seat") IsNot Nothing Then
                    dtPackageSeats = package.ResultDataSet.Tables("Seat")
                    If dtPackageSeats.Rows.Count > 0 Then
                        HttpContext.Current.Session(callID.ToString & "ComponentID") = dtPackageSeats.Rows(0)("ComponentId")
                    End If
                End If
            End If
            Return err
        End Function

        Public Shared Function GetFormattedSeatList(ByVal seatList As List(Of DESeatDetails), ByVal oldExceptionSeat As String, ByVal changeAllSeats As Boolean) As List(Of DESeatDetails)
            Dim formattedSeatList As New List(Of DESeatDetails)

            If IsPackageTransferRequested() Then
                Dim dtPackageSeats As New DataTable
                Dim err As ErrorObj = GetPackageComponentSeats(dtPackageSeats, HttpContext.Current.Request("callid"), HttpContext.Current.Request("product"))
                If seatList IsNot Nothing AndAlso seatList.Count > 0 Then
                    If Not err.HasError AndAlso dtPackageSeats IsNot Nothing Then
                        If dtPackageSeats.Rows.Count > 0 Then
                            Dim removedSeatList As New List(Of DESeatDetails)
                            Dim tempSeatNotFound As Boolean = True
                            'loop through each existing seat in package and check whether they are matching with the current seat list
                            For rowIndex As Integer = 0 To dtPackageSeats.Rows.Count - 1
                                tempSeatNotFound = True
                                For seatListIndex As Integer = 0 To seatList.Count - 1
                                    If seatList(seatListIndex).Stand.Trim.ToUpper = dtPackageSeats.Rows(rowIndex)("Stand").ToString.Trim.ToUpper _
                                        AndAlso seatList(seatListIndex).Area.Trim.ToUpper = dtPackageSeats.Rows(rowIndex)("Area").ToString.Trim.ToUpper _
                                        AndAlso seatList(seatListIndex).Row.Trim.ToUpper = dtPackageSeats.Rows(rowIndex)("Row").ToString.Trim.ToUpper _
                                        AndAlso seatList(seatListIndex).Seat.Trim.ToUpper = dtPackageSeats.Rows(rowIndex)("Seat").ToString.Trim.ToUpper _
                                        AndAlso seatList(seatListIndex).AlphaSuffix.Trim.ToUpper = dtPackageSeats.Rows(rowIndex)("AlphaSuffix").ToString.Trim.ToUpper Then
                                        'yes it is an existing seat in package which user try to transfer
                                        seatList(seatListIndex).CATSeatStatus = SEAT_STATUS_TRANSFER
                                        tempSeatNotFound = False
                                        Exit For
                                    End If
                                Next
                                If tempSeatNotFound Then
                                    Dim removedSeat As New DESeatDetails
                                    removedSeat.Stand = dtPackageSeats.Rows(rowIndex)("Stand").ToString.Trim.ToUpper
                                    removedSeat.Area = dtPackageSeats.Rows(rowIndex)("Area").ToString.Trim.ToUpper
                                    removedSeat.Row = dtPackageSeats.Rows(rowIndex)("Row").ToString.Trim.ToUpper
                                    removedSeat.Seat = dtPackageSeats.Rows(rowIndex)("Seat").ToString.Trim.ToUpper
                                    removedSeat.AlphaSuffix = dtPackageSeats.Rows(rowIndex)("AlphaSuffix").ToString.Trim.ToUpper
                                    removedSeat.CATSeatStatus = SEAT_STATUS_CANCEL
                                    removedSeatList.Add(removedSeat)
                                End If
                            Next
                            'transfered seats are marked, removed seats are collected
                            'now make new list without transfer seats and then add the removed seats

                            For seatListIndex As Integer = 0 To seatList.Count - 1
                                If seatList(seatListIndex).CATSeatStatus.Trim = "" Then
                                    seatList(seatListIndex).CATSeatStatus = SEAT_STATUS_NEW
                                    formattedSeatList.Add(seatList(seatListIndex))
                                End If
                            Next
                            If removedSeatList.Count > 0 Then
                                formattedSeatList.AddRange(removedSeatList)
                            End If
                        End If
                    End If
                Else
                    If Not err.HasError AndAlso dtPackageSeats IsNot Nothing Then
                        If dtPackageSeats.Rows.Count > 0 Then
                            For rowIndex As Integer = 0 To dtPackageSeats.Rows.Count - 1
                                Dim removedSeat As New DESeatDetails
                                removedSeat.Stand = dtPackageSeats.Rows(rowIndex)("Stand").ToString.Trim.ToUpper
                                removedSeat.Area = dtPackageSeats.Rows(rowIndex)("Area").ToString.Trim.ToUpper
                                removedSeat.Row = dtPackageSeats.Rows(rowIndex)("Row").ToString.Trim.ToUpper
                                removedSeat.Seat = dtPackageSeats.Rows(rowIndex)("Seat").ToString.Trim.ToUpper
                                removedSeat.AlphaSuffix = dtPackageSeats.Rows(rowIndex)("AlphaSuffix").ToString.Trim.ToUpper
                                removedSeat.CATSeatStatus = SEAT_STATUS_CANCEL
                                formattedSeatList.Add(removedSeat)
                            Next
                        End If
                    End If
                End If
            ElseIf changeAllSeats Then
                Dim callId As String = HttpContext.Current.Request("callid")
                Dim packageId As String = HttpContext.Current.Request("packageid")
                Dim componentID As String = HttpContext.Current.Request("componentid")
                Dim productCode As String = HttpContext.Current.Request("product")

                If Not String.IsNullOrEmpty(callId) AndAlso Not String.IsNullOrEmpty(packageId) AndAlso Not String.IsNullOrEmpty(componentID) AndAlso Not String.IsNullOrEmpty(productCode) Then
                    Dim package As New TalentPackage
                    Dim componentSeat As New DESeatDetails
                    Dim talProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
                    Dim err As New ErrorObj
                    Dim dtComponentSeats As DataTable
                    package.Settings() = TEBUtilities.GetSettingsObject()
                    package.DePackages.PackageID = packageId
                    package.DePackages.CallId = callId
                    package.DePackages.ProductCode = productCode
                    package.DePackages.BasketId = talProfile.Basket.Basket_Header_ID
                    err = package.GetCustomerPackageInformation()

                    If Not err.HasError AndAlso package.ResultDataSet IsNot Nothing AndAlso package.ResultDataSet.Tables("Seat").Rows.Count > 0 Then
                        Dim dvSeats As New DataView(package.ResultDataSet.Tables("Seat"))
                        Dim rowFilterString As New StringBuilder
                        rowFilterString.Append("ComponentID = ").Append(componentID)
                        dvSeats.RowFilter = rowFilterString.ToString()
                        dtComponentSeats = dvSeats.ToTable()
                        Dim transferredSeatsList As New List(Of String)

                        'Try to find each component seat within the current selection in seatList
                        Dim componentSeatInSeatList As Boolean = False
                        For Each row As DataRow In dtComponentSeats.Rows
                            componentSeat.UnFormattedSeat = row("SeatDetails").ToString()
                            componentSeatInSeatList = False
                            For Each seat As DESeatDetails In seatList
                                If seat.FormattedSeat.Equals(componentSeat.FormattedSeat) Then
                                    componentSeatInSeatList = True
                                    transferredSeatsList.Add(seat.FormattedSeat)
                                    Exit For
                                End If
                            Next
                            'If the component seat is not in Seat list then add it to formattedSeatList with CATStatus='C'
                            If Not componentSeatInSeatList Then
                                Dim removedSeat As New DESeatDetails
                                removedSeat.Stand = componentSeat.Stand
                                removedSeat.Area = componentSeat.Area
                                removedSeat.Row = componentSeat.Row
                                removedSeat.Seat = componentSeat.Seat
                                removedSeat.AlphaSuffix = componentSeat.AlphaSuffix
                                removedSeat.CATSeatStatus = SEAT_STATUS_CANCEL
                                formattedSeatList.Add(removedSeat)
                            End If
                        Next

                        'Iterate through all seats in SeatList and if it doesn't exist in the componentSeats datatable then add it to formattedSeatList with CATStatus='N'
                        Dim selectedSeatFoundInComponentSeats As Boolean = False
                        For Each seat As DESeatDetails In seatList
                            If Not transferredSeatsList.Contains(seat.FormattedSeat) Then
                                seat.CATSeatStatus = SEAT_STATUS_NEW
                                formattedSeatList.Add(seat)
                            End If
                        Next
                    End If
                End If
            ElseIf oldExceptionSeat.Trim.Length > 0 Then
                If oldExceptionSeat = GlobalConstants.ST_EXCCEPTION_UNALLOCATED_SEAT Then
                    seatList.Item(0).CATSeatStatus = SEAT_STATUS_NEW
                    formattedSeatList.Add(seatList.Item(0))
                Else
                    Dim removeOldExceptionSeat As New DESeatDetails
                    removeOldExceptionSeat.FormattedSeat = oldExceptionSeat
                    removeOldExceptionSeat.CATSeatStatus = SEAT_STATUS_CANCEL
                    formattedSeatList.Add(removeOldExceptionSeat)
                    seatList.Item(0).CATSeatStatus = SEAT_STATUS_NEW
                    formattedSeatList.Add(seatList.Item(0))
                End If
            Else
                formattedSeatList = seatList
            End If
            Return formattedSeatList
        End Function

        Public Shared Sub AssignPackageTransferDetail(ByRef callID As String, ByRef productCode As String)
            If IsPackageTransferRequested() Then
                callID = HttpContext.Current.Request("callid")
                productCode = HttpContext.Current.Request("product").Trim
            End If
        End Sub

        ''' <summary>
        ''' Returns true if we're not cancelling a transaction
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function IsBasketNotInCancelMode() As Boolean
            Dim isCATBasketCancelMode As Boolean = True
            Dim tempProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If tempProfile.Basket.BasketItems.Count > 0 Then
                If tempProfile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCELALL OrElse tempProfile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL Then
                    isCATBasketCancelMode = False
                    HttpContext.Current.Session("TemplateIDs") = Nothing
                End If
            End If
            Return isCATBasketCancelMode
        End Function

        Public Shared Function IsBasketInTransferMode() As Boolean
            Dim isCATBasketTransferMode As Boolean = False
            Dim tempProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If tempProfile.Basket.BasketItems.Count > 0 Then
                If tempProfile.Basket.CAT_MODE = GlobalConstants.CATMODE_TRANSFER Then
                    isCATBasketTransferMode = True
                End If
            End If
            If Not isCATBasketTransferMode Then isCATBasketTransferMode = IsPackageTransferRequested()
            Return isCATBasketTransferMode
        End Function

        ''' <summary>
        ''' Check to see if there are any CAT parameters missing if this is currently a CAT
        ''' </summary>
        ''' <returns>True to indicate a missing parameter</returns>
        ''' <remarks></remarks>
        Public Shared Function IsCATParamMissing() As Boolean
            Dim isMissing As Boolean = False
            'CAT Seat selection variables
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                If String.IsNullOrWhiteSpace(HttpContext.Current.Session("istrnxenq")) Then
                    isMissing = True
                ElseIf String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                    isMissing = True
                ElseIf String.IsNullOrWhiteSpace(HttpContext.Current.Session("payref")) Then
                    isMissing = True
                ElseIf String.IsNullOrWhiteSpace(HttpContext.Current.Session("catseatcustomerno")) Then
                    isMissing = True
                ElseIf (String.IsNullOrWhiteSpace(HttpContext.Current.Session("catseat")) AndAlso String.IsNullOrWhiteSpace(HttpContext.Current.Session("callid"))) Then
                    isMissing = True
                End If
            End If
            Return isMissing
        End Function

        ''' <summary>
        ''' Get the CAT ticketing item details object based on the session values
        ''' </summary>
        ''' <returns>The CAT ticketing details object</returns>
        ''' <remarks></remarks>
        Public Shared Function GetCATTicketingItemDetails(ByVal isFromSeatSelection As Boolean) As DEAddTicketingItems
            Dim deAddTicketItemCATDetails As New DEAddTicketingItems
            With deAddTicketItemCATDetails
                'is it CAT seat selection
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                    If HttpContext.Current.Session("catmode").ToString = GlobalConstants.CATMODE_TRANSFER Then
                        'yes it is for transfer
                        'is it normal seat transfer or package
                        If ((HttpContext.Current.Session("callid") IsNot Nothing) _
                            AndAlso (Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("callid").ToString))) Then
                            'it is package
                            Dim DEsd As New DESeatDetails
                            Dim DEtid As New DETicketingItemDetails
                            .CATMode = HttpContext.Current.Session("catmode").ToString
                            .CATSeatCustomerNo = HttpContext.Current.Session("catseatcustomerno").ToString
                            .CATPayRef = ""
                            .CallID = TEBUtilities.CheckForDBNull_Long(HttpContext.Current.Session("callid").ToString)
                        Else
                            Dim DEsd As New DESeatDetails
                            Dim DEtid As New DETicketingItemDetails
                            DEtid.SeatDetails1.FormattedSeat = HttpContext.Current.Session("catseat").ToString
                            .CATMode = HttpContext.Current.Session("catmode").ToString
                            .CATSeatDetails = TCUtilities.FixStringLength(HttpContext.Current.Session("product").ToString, 6) & _
                                                TCUtilities.FixStringLength(DEtid.SeatDetails1.Stand, 3) & _
                                                TCUtilities.FixStringLength(DEtid.SeatDetails1.Area, 4) & _
                                                TCUtilities.FixStringLength(DEtid.SeatDetails1.Row, 4) & _
                                                TCUtilities.FixStringLength(DEtid.SeatDetails1.Seat, 4) & _
                                                TCUtilities.FixStringLength(DEtid.SeatDetails1.AlphaSuffix, 1) & _
                                                TCUtilities.PadLeadingZeros(HttpContext.Current.Session("payref").ToString, 15) & _
                                                TCUtilities.FixStringLength("", 3)
                            .CATSeatCustomerNo = HttpContext.Current.Session("catseatcustomerno").ToString
                            .CATPayRef = ""
                        End If
                    End If
                End If

            End With
            Return deAddTicketItemCATDetails
        End Function

        Public Shared Function IsNotCATRequestOrBasketNotHasCAT(ByVal pageName As String, ByVal functionName As String, ByVal seatCount As Integer) As Boolean
            Dim isValidRequest As Boolean = False
            If Not IsBasketHasCAT(pageName, functionName) Then
                If Not IsItCATRequest(seatCount) Then
                    isValidRequest = True
                End If
            End If
            Return isValidRequest
        End Function
         Private Shared Function isRestrictGraphical(ByVal productCode As String) As Boolean
            Dim restricted As Boolean = False
            Dim talProduct As New TalentProduct
            Dim err As ErrorObj
            err = talProduct.ProductDetails(Talent.eCommerce.Utilities.GetSettingsObject, productCode)
            If Not err.HasError AndAlso talProduct.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" _
                AndAlso talProduct.ResultDataSet.Tables.Count >= 3 Then
                restricted = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(talProduct.ResultDataSet.Tables(2).Rows(0).Item("RestrictGraphical"))
            End If
            Return restricted
        End Function

        Public Shared Function GetCATTransferSeatSelectionURL(ByVal stadiumName As String, ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String, _
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String, Optional ByVal cacheing As Boolean = True, _
                                               Optional ByVal cacheTimeMinutes As Integer = 30) As String

            Dim restrictGraphical As Boolean = isRestrictGraphical(productType)
            Dim redirectUrl As String = String.Empty
            If String.IsNullOrWhiteSpace(stadiumName) OrElse restrictGraphical Then
                redirectUrl = "~/PagesPublic/ProductBrowse/StandAndAreaSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
            Else
                redirectUrl = "~/PagesPublic/ProductBrowse/VisualSeatSelection.aspx?stadium={0}&product={1}&campaign={2}&type={3}&productsubtype={4}&productIsHomeAsAway={5}"
            End If
            redirectUrl = String.Format(redirectUrl, stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway)
            Return redirectUrl
        End Function

        Public Shared Function IsCATSessionsRemoved() As Boolean
            Dim isRemoved As Boolean = False
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catmode")) Then
                HttpContext.Current.Session.Remove("callid")
                HttpContext.Current.Session.Remove("catmode")
                HttpContext.Current.Session.Remove("product")
                HttpContext.Current.Session.Remove("seat1")
                HttpContext.Current.Session.Remove("payref")
                HttpContext.Current.Session.Remove("istrnxenq")
                HttpContext.Current.Session.Remove("seatcustomerno")
                HttpContext.Current.Session.Remove("catseat")
                isRemoved = True
            End If
            Return isRemoved
        End Function

        Public Shared Function IsItCATRequest(ByVal selectedSeatCount As Integer) As Boolean
            Dim product As String = GetCATParamValue("product", False)
            Dim catMode As String = GetCATParamValue("catmode", True)
            Dim catSeatCustomerNo As String = GetCATParamValue("catseatcustomerno", True)
            Dim payRef As String = GetCATParamValue("payref", True)
            Dim isTransationEnquiry As String = GetCATParamValue("istrnxenq", True)
            Dim catSeat As String = GetCATParamValue("catseat", True)
            Dim stand As String = GetCATParamValue("stand", False)
            Dim area As String = GetCATParamValue("area", False)
            Dim callID As String = GetCATParamValue("callid", False)
            Dim isAllCATParamExists As Boolean = True
            If Not String.IsNullOrWhiteSpace(catMode) Then
                If String.IsNullOrWhiteSpace(isTransationEnquiry) Then
                    isAllCATParamExists = False
                ElseIf String.IsNullOrWhiteSpace(catMode) Then
                    isAllCATParamExists = False
                ElseIf String.IsNullOrWhiteSpace(payRef) Then
                    isAllCATParamExists = False
                ElseIf catMode <> GlobalConstants.CATMODE_CANCELALL Then
                    If String.IsNullOrWhiteSpace(catSeatCustomerNo) Then
                        isAllCATParamExists = False
                    ElseIf String.IsNullOrWhiteSpace(product) Then
                        isAllCATParamExists = False
                    ElseIf (String.IsNullOrWhiteSpace(catSeat) AndAlso String.IsNullOrWhiteSpace(callID)) Then
                        isAllCATParamExists = False
                    End If
                End If
                If catMode = GlobalConstants.CATMODE_TRANSFER Then
                    If selectedSeatCount = -2 Then
                        'don't do anything
                    ElseIf selectedSeatCount = -1 Then
                        'this is transfer request called from stand and area selection
                        'user not chosen the seat but chosen stand and area or price and area
                        If String.IsNullOrWhiteSpace(stand) Then
                            isAllCATParamExists = False
                        ElseIf String.IsNullOrWhiteSpace(area) Then
                            isAllCATParamExists = False
                        End If
                    Else
                        If selectedSeatCount < 0 Then
                            isAllCATParamExists = False
                        End If
                    End If
                End If
            Else
                isAllCATParamExists = False
            End If
            Return isAllCATParamExists
        End Function

        Public Shared Function ProcessCATSeatValidation() As Boolean
            Dim landedForCAT As Boolean = False
            ValidateCATSeatValidity()
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catmode")) AndAlso Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catseat")) Then
                HttpContext.Current.Session("catmode") = HttpContext.Current.Request.QueryString("catmode")
                HttpContext.Current.Session("product") = HttpContext.Current.Request.QueryString("product")
                HttpContext.Current.Session("catseat") = HttpContext.Current.Request.QueryString("catseat")
                HttpContext.Current.Session("payref") = HttpContext.Current.Request.QueryString("payref")
                HttpContext.Current.Session("istrnxenq") = HttpContext.Current.Request.QueryString("istrnxenq")
                HttpContext.Current.Session("catseatcustomerno") = HttpContext.Current.Request.QueryString("catseatcustomerno")
                landedForCAT = True
            ElseIf Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catmode")) _
                AndAlso Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("callid")) _
                AndAlso IsNumeric(HttpContext.Current.Request.QueryString("callid")) _
                AndAlso CLng(HttpContext.Current.Request.QueryString("callid").Trim) > 0 Then
                HttpContext.Current.Session("callid") = HttpContext.Current.Request.QueryString("callid")
                HttpContext.Current.Session("catmode") = HttpContext.Current.Request.QueryString("catmode")
                HttpContext.Current.Session("product") = HttpContext.Current.Request.QueryString("product")
                HttpContext.Current.Session("catseat") = ""
                HttpContext.Current.Session("payref") = HttpContext.Current.Request.QueryString("payref")
                HttpContext.Current.Session("istrnxenq") = HttpContext.Current.Request.QueryString("istrnxenq")
                HttpContext.Current.Session("catseatcustomerno") = HttpContext.Current.Request.QueryString("catseatcustomerno")
                landedForCAT = True
            Else
                IsCATSessionsRemoved()
            End If
            Return landedForCAT
        End Function

        Public Shared Function GetAmendCATSeatPriceBand() As String
            Dim amendCatSeatPriceBand As String = String.Empty, _
                callID As String = String.Empty
            If CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE = GlobalConstants.CATMODE_AMEND Then
                Dim catSeatPriceBand As String = String.Empty
                Dim amendCATSeat As String = GetAmendCATSeat()
                If amendCATSeat.Trim.Length > 0 Then
                    Dim tempSeat As String = amendCATSeat.Substring(6, 3).Trim & "/" & amendCATSeat.Substring(9, 4).Trim & "/" & amendCATSeat.Substring(13, 4).Trim & "/" & amendCATSeat.Substring(17, 4).Trim
                    If amendCATSeat.Substring(21, 1).Trim.Length > 0 Then
                        tempSeat = tempSeat & "/" & amendCATSeat.Substring(21, 1).Trim
                    End If
                    Dim dvCATSeatDetails As DataView = Nothing
                    dvCATSeatDetails = GetCATSeatDetailsView(amendCATSeat.Substring(0, 6), tempSeat, "", amendCATSeat.Substring(22, 15), callID)
                    If dvCATSeatDetails Is Nothing OrElse dvCATSeatDetails.Count <= 0 Then
                        dvCATSeatDetails = GetCATSeatDetailsView(amendCATSeat.Substring(0, 6), tempSeat, amendCATSeat.Substring(22, 15), amendCATSeat.Substring(22, 15), callID)
                    End If
                    If dvCATSeatDetails IsNot Nothing AndAlso dvCATSeatDetails.Count > 0 Then
                        amendCatSeatPriceBand = Utilities.CheckForDBNull_String(dvCATSeatDetails.Item(0).Row("PriceBand"))
                    End If
                End If
            End If
            Return amendCatSeatPriceBand
        End Function

        Public Shared Sub ValidateCATSeatValidity()
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catmode")) AndAlso Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catseat")) Then
                Dim catMode As String = String.Empty, _
                    productCode As String = String.Empty, _
                    catSeat As String = String.Empty, _
                    catSeatCustomerNo As String = String.Empty, _
                    paymentReference As String = String.Empty, _
                    callID As String = String.Empty

                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catmode")) Then
                    catMode = HttpContext.Current.Request.QueryString("catmode")
                End If
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("product")) Then
                    productCode = HttpContext.Current.Request.QueryString("product")
                End If
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catseat")) Then
                    catSeat = HttpContext.Current.Request.QueryString("catseat")
                End If
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("payref")) Then
                    paymentReference = HttpContext.Current.Request.QueryString("payref")
                End If
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catseatcustomerno")) Then
                    catSeatCustomerNo = HttpContext.Current.Request.QueryString("catseatcustomerno")
                End If
                Dim isNotValid As Boolean = True
                Dim dvCATSeatDetails As DataView = Nothing
                dvCATSeatDetails = GetCATSeatDetailsView(productCode, catSeat, paymentReference, paymentReference, callID)
                If dvCATSeatDetails IsNot Nothing AndAlso dvCATSeatDetails.Count > 0 Then
                    isNotValid = False
                End If
                If isNotValid Then
                    HttpContext.Current.Session("TalentErrorCode") = "MF"
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
            End If
        End Sub

        Public Shared Function GetFormattedCATPackageDetail() As String
            Dim formattedCATPackageDetail As String = String.Empty
            If Not String.IsNullOrWhiteSpace(CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE) Then
                Dim packageID As String = String.Empty
                Dim CATPackageDetail As String = String.Empty
                For Each tbi As TalentBasketItem In CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems
                    If UCase(tbi.MODULE_) = "TICKETING" AndAlso Not tbi.IS_FREE Then
                        If Not TEBUtilities.IsTicketingFee(tbi.MODULE_, tbi.Product.Trim, tbi.FEE_CATEGORY) Then
                            If tbi.PACKAGE_ID > 0 AndAlso TEBUtilities.CheckForDBNull_String(tbi.PRODUCT_TYPE_ACTUAL).Trim.ToUpper = "P" Then
                                packageID = tbi.PACKAGE_ID
                                CATPackageDetail = tbi.CAT_SEAT_DETAILS
                            End If
                        End If
                    End If
                Next

                If CATPackageDetail.Trim.Length > 0 Then
                    'packageid|matchcode|callid|payment reference
                    formattedCATPackageDetail = packageID & "|" & CATPackageDetail.Substring(0, 6) & "|" & CATPackageDetail.Substring(6, 13) & "|" & CATPackageDetail.Substring(22, 15)
                End If
            End If
            Return formattedCATPackageDetail
        End Function

        Public Shared Function GetCATPackageURLWithParam(ByVal redirectURL As String, ByVal catPackageDetail As String) As String
            If Not String.IsNullOrWhiteSpace(catPackageDetail) Then
                Dim params() As String = catPackageDetail.Split("|")
                Dim catQuerystring As String = String.Empty
                If params.Length > 0 AndAlso params.Length = 4 Then
                    'catmode=T&payref=23274&istrnxenq=True&catseatcustomerno=000000010740&callid=987
                    catQuerystring = catQuerystring & "&catmode=T&istrnxenq=True"
                    catQuerystring = catQuerystring & "&payref=" & params(3)
                    If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                        catQuerystring = catQuerystring & "&catseatcustomerno=" & CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                    End If
                    catQuerystring = catQuerystring & "&callid=" & params(2)
                End If
                redirectURL = redirectURL & catQuerystring
            End If
            Return redirectURL
        End Function

        Public Shared Function GetCATPackageURLParamFromQueryString(ByVal redirectURL As String) As String
            If HttpContext.Current.Request("catmode") IsNot Nothing Then
                If HttpContext.Current.Request("callid") IsNot Nothing AndAlso IsNumeric(HttpContext.Current.Request("callid")) Then
                    If HttpContext.Current.Session("catpayref") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("catpayref")) AndAlso IsNumeric(HttpContext.Current.Session("catpayref")) Then
                        Dim catQuerystring As String = String.Empty
                        catQuerystring = catQuerystring & "&catmode=" & HttpContext.Current.Request("catmode") & "&istrnxenq=True"
                        catQuerystring = catQuerystring & "&payref=" & HttpContext.Current.Session("catpayref")
                        HttpContext.Current.Session.Remove("catpayref")
                        If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                            catQuerystring = catQuerystring & "&catseatcustomerno=" & CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
                        End If
                        catQuerystring = catQuerystring & "&callid=" & HttpContext.Current.Request("callid")
                        If redirectURL.ToLower().Contains("aspx?") Then
                            redirectURL = redirectURL & catQuerystring
                        Else
                            redirectURL = redirectURL & "?" & catQuerystring
                        End If
                    End If
                End If
            End If
            Return redirectURL
        End Function

        Public Shared Sub AssignPackageCATSeatDetail(ByVal controls As System.Web.UI.ControlCollection)
            HttpContext.Current.Session("CATSeatsInSeatSelection") = Nothing
            If IsPackageTransferRequested() Then
                Dim hdfSelectedSeats As HiddenField = Utilities.FindWebControl("hdfSelectedSeats", controls)
                If hdfSelectedSeats IsNot Nothing AndAlso hdfSelectedSeats.Value.Trim.Length > 0 Then
                    Dim data As String = hdfSelectedSeats.Value
                    Dim responseString As String = String.Empty
                    Dim seatsList As New List(Of DESeatDetails)
                    Dim temp As New Control
                    data = data.Replace("-", "/").ToUpper()
                    For Each item As String In data.Split(",")
                        Dim seat As New DESeatDetails
                        seat.FormattedSeat = item
                        seatsList.Add(seat)
                    Next
                    HttpContext.Current.Session("CATSeatsInSeatSelection") = seatsList
                End If
            End If
        End Sub

        ''' <summary>
        ''' Retrieve the CAT payment reference (the payment reference from the original sale) from the basket with the tickets being cancelled
        ''' </summary>
        ''' <returns>The payment reference string</returns>
        ''' <remarks></remarks>
        Public Shared Function GetCATPaymentReference() As String
            Dim paymentRefence As String = String.Empty
            Dim tempProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            Dim seatDetails As String = String.Empty
            If tempProfile.Basket.CAT_MODE.Length > 0 Then
                For Each tbi As TalentBasketItem In tempProfile.Basket.BasketItems
                    If UCase(tbi.MODULE_) = "TICKETING" AndAlso Not tbi.IS_FREE Then
                        If Not Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product.Trim, tbi.FEE_CATEGORY) Then
                            seatDetails = tbi.CAT_SEAT_DETAILS
                            Exit For
                        End If
                    End If
                Next
            End If
            If seatDetails.Length = 37 Then
                'Seat Details stored in tbl_basket_detail.CAT_SEAT_DETAILS as "HCL152E  BF  Z   0003 000000000028215"
                paymentRefence = seatDetails.Substring(22, 15)
                paymentRefence = paymentRefence.TrimStart(GlobalConstants.LEADING_ZEROS)
            End If
            Return paymentRefence
        End Function

        Private Shared Function IsBasketHasCAT(ByVal pageName As String, ByVal functionName As String) As Boolean
            IsResetedUnUsedCATSessions()
            Dim isCATBasketExists As Boolean = False
            Dim tempProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If tempProfile.Basket.BasketItems.Count > 0 _
                AndAlso pageName.ToLower <> "basket.aspx" _
                AndAlso pageName.ToLower <> "catconfirm.aspx" _
                AndAlso pageName.ToLower <> "checkoutpaymentdetails.aspx" _
                AndAlso Not IsValidCATBasketMigration(pageName, functionName) Then
                If Not String.IsNullOrWhiteSpace(tempProfile.Basket.CAT_MODE) Then
                    isCATBasketExists = True
                    If tempProfile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL Then
                        HttpContext.Current.Session("TicketingGatewayError") = "AddFailedAsBasketInCATMode_Cancel"
                        HttpContext.Current.Session("TalentErrorCode") = "AddFailedAsBasketInCATMode_Cancel"
                    ElseIf tempProfile.Basket.CAT_MODE = GlobalConstants.CATMODE_AMEND Then
                        HttpContext.Current.Session("TicketingGatewayError") = "AddFailedAsBasketInCATMode_Amend"
                        HttpContext.Current.Session("TalentErrorCode") = "AddFailedAsBasketInCATMode_Amend"
                    ElseIf tempProfile.Basket.CAT_MODE = GlobalConstants.CATMODE_TRANSFER Then
                        HttpContext.Current.Session("TicketingGatewayError") = "AddFailedAsBasketInCATMode_Transfer"
                        HttpContext.Current.Session("TalentErrorCode") = "AddFailedAsBasketInCATMode_Transfer"
                    End If
                    HttpContext.Current.Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
            End If
            Return isCATBasketExists
        End Function

        Private Shared Sub IsResetedUnUsedCATSessions()
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString("catmode")) _
                AndAlso HttpContext.Current.Request.QueryString("catmode") <> GlobalConstants.CATMODE_TRANSFER Then
                IsCATSessionsRemoved()
            End If
        End Sub

        Private Shared Function GetCATParamValue(ByVal paramName As String, ByVal canCheckSession As Boolean) As String
            Dim paramValue As String = String.Empty
            paramValue = HttpContext.Current.Request.QueryString(paramName)
            If canCheckSession AndAlso String.IsNullOrWhiteSpace(paramValue) Then
                paramValue = HttpContext.Current.Session(paramName)
            End If
            Return paramValue
        End Function

        Private Shared Function IsValidCATBasketMigration(ByVal pageName As String, ByVal functionName As String) As Boolean
            Dim isValidCATMigration As Boolean = False
            If Not String.IsNullOrWhiteSpace(CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE) Then
                If CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE = GlobalConstants.CATMODE_AMEND _
                    OrElse CType(HttpContext.Current.Profile, TalentProfile).Basket.CAT_MODE = GlobalConstants.CATMODE_TRANSFER Then
                    If pageName.ToLower = "login.aspx" AndAlso functionName.ToLower = "clearandadd" Then
                        isValidCATMigration = True
                    End If
                End If
            End If
            Return isValidCATMigration
        End Function

        Private Shared Function GetAmendCATSeat() As String
            Dim amendCATSeat As String = String.Empty
            Dim tempProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            For Each tbi As TalentBasketItem In tempProfile.Basket.BasketItems
                If UCase(tbi.MODULE_) = "TICKETING" AndAlso Not tbi.IS_FREE Then
                    If Not Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product.Trim, tbi.FEE_CATEGORY) Then
                        amendCATSeat = tbi.CAT_SEAT_DETAILS
                        Exit For
                    End If
                End If
            Next
            Return amendCATSeat
        End Function

        Private Shared Function GetCATSeatDetailsView(ByVal productCode As String, ByVal catSeat As String, ByVal paymentReference As String, ByVal filterPaymentReference As String, ByVal callID As String) As DataView
            Dim dvCATSeatDetails As DataView = Nothing
            Dim dtOrderDetails As DataTable = GetCATSeatStatus(paymentReference, callID)
            If (dtOrderDetails IsNot Nothing) Then
                If dtOrderDetails.Rows.Count > 0 Then
                    Dim dvOrderDetails As DataView = dtOrderDetails.DefaultView
                    dtOrderDetails.DefaultView.RowFilter = GetRowFilterCondition(productCode, catSeat, filterPaymentReference)
                    If dtOrderDetails.DefaultView.Count > 0 Then
                        dvCATSeatDetails = dtOrderDetails.DefaultView
                    End If
                End If
            End If
            Return dvCATSeatDetails
        End Function

        Private Shared Function GetCATSeatStatus(ByVal paymentReference As String, ByVal callID As String) As DataTable
            Dim dsOrderHistory As New DataSet
            Dim dtOrderDetails As DataTable = Nothing
            Try
                Dim moduleDefaultsValue As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
                Dim order As New Talent.Common.TalentOrder
                Dim err As New Talent.Common.ErrorObj
                Dim settings As Talent.Common.DESettings = Utilities.GetSettingsObject()
                If HttpContext.Current.Profile.IsAnonymous Then
                    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
                Else
                    settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(CType(HttpContext.Current.Profile, TalentProfile).User.Details.Account_No_1, 12)
                End If
                settings.Cacheing = False
                settings.AuthorityUserProfile = moduleDefaultsValue.AuthorityUserProfile
                order.Dep.PaymentReference = paymentReference
                If (String.IsNullOrWhiteSpace(callID) OrElse Not (IsNumeric(callID))) Then callID = "0"
                order.Dep.CallId = callID
                order.Dep.CustNumberPayRefShouldMatch = False
                order.Settings() = settings
                err = order.OrderEnquiryDetails()
                If (Not err.HasError) AndAlso (order.ResultDataSet IsNot Nothing) Then
                    dsOrderHistory = order.ResultDataSet()
                    If dsOrderHistory.Tables.Count > 1 Then
                        dtOrderDetails = dsOrderHistory.Tables("OrderEnquiryDetails")
                    Else
                        'order detail table is missing or error
                        If dsOrderHistory.Tables.Count > 0 AndAlso dsOrderHistory.Tables("StatusResults").Rows.Count > 0 Then
                            'ProcessErrorMessage(True, CheckForDBNull_String(dsOrderHistory.Tables("StatusResults").Rows(0)("ReturnCode")), False)
                        Else
                            'status table not having any records consider unexpected error
                            'ProcessErrorMessage(False, "ErrMess_NoTableFound", False)
                        End If
                    End If
                Else
                    'ProcessErrorMessage(False, "ErrMess_UnExpected", False)
                End If
            Catch ex As Exception
                'ProcessErrorMessage(False, "ErrMess_Exception", False)
            End Try
            Return dtOrderDetails
        End Function

        Private Shared Function GetRowFilterCondition(ByVal productCode As String, ByVal catSeat As String, ByVal paymentReference As String) As String
            Dim filterCondition As New StringBuilder
            filterCondition.Append("(PaymentReference='" & paymentReference.PadLeft(15, "0") & "'")
            filterCondition.Append(" AND ProductCode='" & productCode & "'")
            filterCondition.Append(" AND Seat='" & catSeat & "'")
            filterCondition.Append(" AND StatusCode <> 'CANCEL')")
            Return filterCondition.ToString()
        End Function

        Public Shared Function IsPackageTransferRequested() As Boolean
            Dim IsTransferRequested As Boolean = False
            If Not IsTransferRequested Then
                'are user trying to transfer the package
                If HttpContext.Current.Request("catmode") IsNot Nothing AndAlso HttpContext.Current.Request("catmode") = GlobalConstants.CATMODE_TRANSFER Then
                    If HttpContext.Current.Request("callid") IsNot Nothing AndAlso IsNumeric(HttpContext.Current.Request("callid")) Then
                        If HttpContext.Current.Request("product") IsNot Nothing AndAlso HttpContext.Current.Request("product").Trim.Length > 0 Then
                            IsTransferRequested = True
                        End If
                    End If
                Else
                    If HttpContext.Current.Session("catmode") IsNot Nothing AndAlso HttpContext.Current.Session("catmode") = GlobalConstants.CATMODE_TRANSFER Then
                        If HttpContext.Current.Session("callid") IsNot Nothing AndAlso IsNumeric(HttpContext.Current.Session("callid")) Then
                            If HttpContext.Current.Session("product") IsNot Nothing AndAlso HttpContext.Current.Session("product").Trim.Length > 0 Then
                                IsTransferRequested = True
                            End If
                        End If
                    End If
                End If
            End If
            Return IsTransferRequested
        End Function

    End Class

End Namespace
